using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using System.Diagnostics;
using MinerWars.AppCode.Networking.MasterService;
using System;
using System.ServiceModel;

//  This screen is displayed during we wait for register response. It runs in main thread (no background thread needed in this case).

namespace MinerWars.AppCode.Game.GUI
{
    enum MyGuiScreenRegisterProgressPhases
    {
        NOTHING,
        WAITING_FOR_REGISTER_RESPONSE,
    }

    class MyGuiScreenRegisterProgress : MyGuiScreenMasterServiceCallProgress
    {
        MyGuiScreenRegisterProgressPhases m_phase;
        int m_lastTimeMessage;

        public static MyGuiScreenRegisterProgress CurrentScreen = null;    //  This is always filled with reference to actual instance of this screen. If there isn't, it's null.
        MyGuiScreenBase m_openAfterSuccesfullRegistration;
        MyGuiScreenBase m_parentScreen;
        string m_playerName;
        string m_password;
        string m_email;
        bool m_sendMeNewsletters;

        public MyGuiScreenRegisterProgress(string playerName, string password, string email, bool sendMeNewsletters, MyGuiScreenBase openAfterSuccesfullRegistration, MyGuiScreenBase parentScreen)
            : base(MyTextsWrapperEnum.RegistrationInProgressPleaseWait, true)
        {
            CurrentScreen = this;
            m_playerName = playerName;
            m_password = password;
            m_email = email;
            m_sendMeNewsletters = sendMeNewsletters;
            m_openAfterSuccesfullRegistration = openAfterSuccesfullRegistration;
            m_parentScreen = parentScreen;

            m_phase = MyGuiScreenRegisterProgressPhases.NOTHING;
            m_lastTimeMessage = MyMinerGame.TotalTimeInMilliseconds;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenRegisterProgress";
        }

        public override void LoadContent()
        {
            m_phase = MyGuiScreenRegisterProgressPhases.WAITING_FOR_REGISTER_RESPONSE;
            MyMasterServiceClient.SetCredentials(MyMwcNetworkingConstants.WCF_MS_PUBLIC_USERNAME, MyMwcNetworkingConstants.WCF_MS_PUBLIC_PASSWORD);

            // This must be last, call progress start
            base.LoadContent();
        }

        protected override void ServiceProgressStart(MyMasterServiceClient client)
        {
            AddAction(client.BeginRegister(m_playerName, MyMwcUtils.GetHashedPassword(m_password), m_email, m_sendMeNewsletters, null, client), OnRegister);
        }
        
        void OnRegister(IAsyncResult result, MyMasterServiceClient client)
        {
            try
            {
                client.EndRegister(result);
                client.Close();
                HandleRegister();
            }
            catch (FaultException<MyRegistrationFault> fault)
            {
                HandleRegisterError(fault.Detail.ErrorCode);
            }
            catch (Exception exception)
            {
                client.Abort();
                MyMwcLog.WriteLine(exception);
                HandleRegisterError(MyMwcRegisterResponseResultEnum.UNKNOWN_ERROR);
            }
            CloseScreen();
        }

        void HandleRegister()
        {
            if (m_parentScreen != null)
            {
                m_parentScreen.CloseScreen();
            }

            if (m_openAfterSuccesfullRegistration != null)
            {
                if (m_openAfterSuccesfullRegistration is MyGuiScreenLogin)
                {
                    MyGuiScreenLogin loginScreen = (MyGuiScreenLogin)m_openAfterSuccesfullRegistration;
                    loginScreen.SetUsernameAndPassword(m_playerName, m_password);
                    loginScreen.OnOkClick(null);
                }
            }
        }

        void HandleRegisterError(MyMwcRegisterResponseResultEnum faultCode)
        {
            MyMwcLog.WriteLine("Error registering. Fault code: " + (int)faultCode + ", Fault code as string: " + faultCode.ToString());
            MyTextsWrapperEnum? errorMessage = null;
            if (faultCode == MyMwcRegisterResponseResultEnum.USERNAME_FORMAT_INVALID)
            {
                errorMessage = MyTextsWrapperEnum.ValidationUsername;
            }
            else if (faultCode == MyMwcRegisterResponseResultEnum.PASSWORD_FORMAT_INVALID)
            {
                errorMessage = MyTextsWrapperEnum.ValidationPasswordWrong;
            }
            else if (faultCode == MyMwcRegisterResponseResultEnum.EMAIL_FORMAT_INVALID)
            {
                errorMessage = MyTextsWrapperEnum.ValidationEmailWrong;
            }
            else if (faultCode == MyMwcRegisterResponseResultEnum.USERNAME_ALREADY_USED)
            {
                errorMessage = MyTextsWrapperEnum.ValidationUsernameAlreadyUsed;
            }
            else if (faultCode == MyMwcRegisterResponseResultEnum.WRONG_CLIENT_VERSION)
            {
                errorMessage = MyTextsWrapperEnum.CantLoginClientVersionIsWrong;
            }
            else // MyMwcRegisterResponseResultEnum.UNKNOWN_ERROR or anything else
            {
                errorMessage = MyTextsWrapperEnum.CantRegisterServerIsUnavailable;
            }

            if (errorMessage.HasValue)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, errorMessage.Value,
                    MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null));
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
