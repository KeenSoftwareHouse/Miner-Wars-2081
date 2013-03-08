using System.Net;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using System;
using MinerWars.AppCode.Game.Managers.Session;
using System.Text;
using System.ServiceModel.Security;
using System.ServiceModel;
using MinerWars.AppCode.Networking.MasterService;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.CommonLIB.AppCode.Networking.Services;
using MinerWars.AppCode.Networking;

//  This screen is displayed during we wait for login response. It runs in main thread (no background thread needed in this case).

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenLoginProgress : MyGuiScreenProgressBaseAsync<object>
    {
        string m_playerName;
        string m_password;
        MyGuiScreenBase m_openAfterSuccessfulLogin;
        MyGuiScreenBase m_closeAfterSuccesfulLogin;
        Action m_callAfterSuccessfulLogin = null;

        public bool CloseScreenBeforeCallingHandler { get; set; }

        //  Using this static public property client-server tells us about login response
        public static MyGuiScreenLoginProgress CurrentScreen = null;    //  This is always filled with reference to actual instance of this scree. If there isn't, it's null.
        public MyMwcLoginResponseResultEnum? LoginResponse;

        IAsyncResult m_loginResult;

        public MyGuiScreenLoginProgress(string playerName, string password, Action callAfterSuccessfulLogin, MyGuiScreenBase closeAfterSuccesfulLogin)
            : this(playerName, password, (MyGuiScreenBase)null, closeAfterSuccesfulLogin)
        {
            m_callAfterSuccessfulLogin = callAfterSuccessfulLogin;
        }

        public MyGuiScreenLoginProgress(string playerName, string password, MyGuiScreenBase openAfterSuccessfulLogin, MyGuiScreenBase closeAfterSuccesfulLogin)
            : base(MyTextsWrapperEnum.LoginInProgressPleaseWait, true)
        {
            m_openAfterSuccessfulLogin = openAfterSuccessfulLogin;
            m_closeAfterSuccesfulLogin = closeAfterSuccesfulLogin;

            CurrentScreen = this;

            //  Reset everytime
            LoginResponse = null;

            m_playerName = playerName;
            m_password = password;
            PasswordHash = MyMwcUtils.GetHashedPassword(m_password);
        }

        public string PlayerName
        {
            get
            {
                return m_playerName;
            }
        }

        public string PasswordHash
        {
            get;
            set;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenLoginProgress";
        }

        protected override void ProgressStart()
        {
            if (String.IsNullOrEmpty(m_playerName) || String.IsNullOrEmpty(m_password))
            {
                LoginResponse = MyMwcLoginResponseResultEnum.USERNAME_OR_PASSWORD_EMPTY;
            }
            else
            {
                MyMasterServiceClient.SetCredentials(m_playerName, PasswordHash);
                MySectorServiceClient.SetCredentials(m_playerName, PasswordHash);

                MyMwcLog.WriteLine("Login request: " + m_playerName);

                var client = MyMasterServiceClient.GetCheckedInstance();
                AddAction(client.BeginCheckVersion(MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION, null, client), OnCheckVersion);
            }
        }

        void OnCheckVersion(IAsyncResult result, object asyncState)
        {
            try
            {
                var client = (MyMasterServiceClient)result.AsyncState;
                client.EndCheckVersion(result);

                AddAction(client.BeginGetSectorServer(null, client), OnGetSectorServer);
            }
            catch (Exception e)
            {
                HandleLoginError(e);
            }
        }

        void OnGetSectorServer(IAsyncResult result, object asyncState)
        {
            try
            {
                var client = (MyMasterServiceClient)result.AsyncState;
                string sectorServerUrl = client.EndGetSectorServer(result);
                client.Close();
                MySectorServiceClient.SetUrl(sectorServerUrl);

                AddAction(MySectorServiceClient.BeginLoginStatic(), OnLoginCompleted);
            }
            catch (Exception e)
            {
                HandleLoginError(e);
            }
        }

        void OnLoginCompleted(IAsyncResult result, object asyncState)
        {
            try
            {
                MyUserInfo userInfo = MySectorServiceClient.EndLoginStatic(result);

                MyMwcLog.WriteLine("Login successful, userId: " + userInfo.UserId);

                // 2.5D and cheats for everyone
                var player = new MyPlayerLocal(new StringBuilder(userInfo.DisplayName), userInfo.UserId, userInfo.CanAccessDemo, userInfo.CanSave,
                    userInfo.CanAccessEditorForStory, userInfo.CanAccessEditorForMMO, userInfo.HasAnyCheckpoints, true, userInfo.CanAccessMMO,
                        new StringBuilder(m_playerName), new StringBuilder(PasswordHash), true);

                player.AdditionalInfo = userInfo.AdditionalInfo;

                MyClientServer.LoggedPlayer = player;
                LoginResponse = MyMwcLoginResponseResultEnum.OK;
            }
            catch (Exception e)
            {
                HandleLoginError(e);
            }
        }

        void HandleLoginError(Exception exception)
        {
            MyMwcLog.WriteLine("Login failed:" + exception);

            if (exception is MessageSecurityException)
            {
                var inner = exception.InnerException as FaultException;

                if (inner != null)
                {
                    MyMwcLog.WriteLine("Login fail details: " + inner.Code.Name);

                    switch (inner.Code.Name)
                    {
                        case MyServiceConstants.FAULT_RESTRICTED:
                            LoginResponse = MyMwcLoginResponseResultEnum.ACCESS_RESTRICTED;
                            break;

                        case MyServiceConstants.FAULT_CREDENTIALS:
                            LoginResponse = MyMwcLoginResponseResultEnum.WRONG_USERNAME_OR_PASSWORD;
                            break;

                        default:
                            LoginResponse = MyMwcLoginResponseResultEnum.GENERAL_FAILURE;
                            break;
                    }
                }

                if (exception.Message.StartsWith("The security timestamp")) // There's no other way to check for invalid timestamp
                {
                    LoginResponse = MyMwcLoginResponseResultEnum.INVALID_TIME;
                }
            }
            else if (exception is FaultException<MyVersionFault>)
            {
                LoginResponse = MyMwcLoginResponseResultEnum.WRONG_CLIENT_VERSION;
            }
            else if (exception is FaultException<MyAlreadyLoggedInFault>)
            {
                LoginResponse = MyMwcLoginResponseResultEnum.USER_ALREADY_LOGGED_IN;
            }
            else if (exception.Message.StartsWith("The request for security token has invalid or malformed elements"))
            {
                // This is also caused by invalid time
                LoginResponse = MyMwcLoginResponseResultEnum.INVALID_TIME;
            }
            else
            {
                LoginResponse = MyMwcLoginResponseResultEnum.GENERAL_FAILURE;
            }
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            //  Only continue if this screen is really open (not closing or closed)
            if (GetState() != MyGuiScreenState.OPENED) return false;

            if (LoginResponse.HasValue)
            {
                MyMwcLog.WriteLine("Login Response: " + LoginResponse.Value.ToString());

                if (LoginResponse.Value == MyMwcLoginResponseResultEnum.OK)
                {
                    //  Login successful!
                    MyConfig.LastLoginWasSuccessful = true;
                    MyConfig.Save();

                    if (m_closeAfterSuccesfulLogin != null)
                    {
                        m_closeAfterSuccesfulLogin.CloseScreen();
                    }

                    if (CloseScreenBeforeCallingHandler)
                    {
                        CloseScreenNow();
                    }

                    if (m_openAfterSuccessfulLogin != null)
                    {
                        MyGuiManager.AddScreen(m_openAfterSuccessfulLogin);
                    }
                    else if (m_callAfterSuccessfulLogin != null)
                    {
                        m_callAfterSuccessfulLogin();
                    }

                    if (!CloseScreenBeforeCallingHandler)
                    {
                        CloseScreen();
                    }
                }
                else
                {
                    if (LoginResponse.Value == MyMwcLoginResponseResultEnum.USER_ALREADY_LOGGED_IN)
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.CantLoginPlayerAlreadyLoggedIn, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    }
                    else if (LoginResponse.Value == MyMwcLoginResponseResultEnum.WRONG_USERNAME_OR_PASSWORD)
                    {
                        MyTextsWrapperEnum wrongMessage = MyTextsWrapperEnum.CantLoginWrongPlayerNameOrPassword;
                        if (MyMwcFinalBuildConstants.IS_PUBLIC) wrongMessage = MyTextsWrapperEnum.CantLoginWrongPlayerNameOrPasswordPublic;
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, wrongMessage, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    }
                    else if (LoginResponse.Value == MyMwcLoginResponseResultEnum.USERNAME_OR_PASSWORD_EMPTY)
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.CantLoginEmptyUsernameOrPassword, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    }
                    else if (LoginResponse.Value == MyMwcLoginResponseResultEnum.ACCESS_RESTRICTED && !MyMwcFinalBuildConstants.IS_PUBLIC)
                    {
                        MyTextsWrapperEnum restrictedMessage = MyTextsWrapperEnum.CantLoginAccessRestrictedDevelop;
                        if (MyMwcFinalBuildConstants.IS_TEST) restrictedMessage = MyTextsWrapperEnum.CantLoginAccessRestrictedTest;
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, restrictedMessage, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    }
                    else if (LoginResponse.Value == MyMwcLoginResponseResultEnum.WRONG_CLIENT_VERSION)
                    {
                        MyTextsWrapperEnum message = MyTextsWrapperEnum.CantLoginClientVersionIsWrong;
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, message, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    }
                    else if (LoginResponse.Value == MyMwcLoginResponseResultEnum.INVALID_TIME)
                    {
                        MyTextsWrapperEnum message = MyTextsWrapperEnum.InvalidTime;
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, message, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
                    }
                    else
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseTryAgain, MyTextsWrapperEnum.MessageBoxNetworkErrorCaption, MyTextsWrapperEnum.Ok, null));
                    }

                    // This is used for quick launch - login progress screen is not created from login screen
                    if (LoginResponse.Value != MyMwcLoginResponseResultEnum.OK && !MyGuiManager.IsLoginScreenOpened())
                    {
                        AddLoginScreen();
                    }

                    CloseScreen();
                }

                MySectorServiceClient.SafeClose();
                return false;
            }

            return true;
        }

        void AddLoginScreen()
        {
            if (MySteam.IsActive)
            {
                //MyGuiManager.BackToMainMenu();
            }
            else
            {
                MyGuiManager.AddScreen(new MyGuiScreenLogin(m_playerName, m_password, m_openAfterSuccessfulLogin));
            }
        }

        protected override void Canceling()
        {
            base.Canceling();
            if (!MyGuiManager.IsLoginScreenOpened())
            {
                AddLoginScreen();
            }
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            if (ret == true)
            {
                CurrentScreen = null;
            }

            return ret;
        }
    }
}
