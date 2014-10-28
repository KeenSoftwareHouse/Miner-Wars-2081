using MinerWarsMath;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Textures;
using KeenSoftwareHouse.Library.Extensions;

//  This main menu server for different scenarios. Right after the game is started it displays default main menu.
//  During game or fly-through it displays different buttons and may behave differently.

namespace MinerWars.AppCode.Game.GUI
{
    using System;
    using System.Text;
    using Networking.SectorService;
    using Missions;
    using Managers.Session;
    using Sessions;
    using CommonLIB.AppCode.Networking.Multiplayer;
    using SysUtils;
    using MinerWars.AppCode.Networking;
    using MinerWars.AppCode.Networking.MasterService;
    using System.ServiceModel;
    using System.Threading;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;

    class MyGuiScreenMainMenu : MyGuiScreenBase
    {
        static int m_autologinAttempts = 0;
        //MyLowFPSDetection m_lowFpsDetection;
        //static MyTexture2D m_MainMenuPanelBackground;
        private static MyTexture2D m_MainmenuOverlay;
        private Vector2 m_mainMenuPanelSize;
        private Vector2 m_mainMenuPanelPosition;
        private bool m_musicPlayed = false;
        private int m_timeFromMenuLoadedMS = 0;
        private const int PLAY_MUSIC_AFTER_MENU_LOADED_MS = 1000;

        private static bool m_loginInProgress = false;

        MyGuiControlButton m_btnJoinMode;

        private StringBuilder m_playerNameString = new StringBuilder(40);
        private bool m_showVideoOptions = true;

        delegate void OnLoginVerified();

        public override string GetFriendlyName()
        {
            return "MyGuiScreenMainMenu";
        }

        public static void SkipAutologin()
        {
            m_autologinAttempts = 1;
        }

        //  This is for adding main menu the easy way
        public static void AddMainMenu(bool showVideoOptions)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMainMenu(showVideoOptions));
        }

        //  This is when we need to open screen that needs logged in user. So first we display login screen, make sure user is logged in,
        //  and then continue do desired screen
        //  It is for adding login screen the easy way
        public static void AddLoginScreen(MyGuiScreenBase openAfterSuccessfulLogin)
        {
            AddLoginScreen(GetAction(openAfterSuccessfulLogin));
        }

        public static void AddLoginScreen(Action callAfterSuccessfulLogin)
        {
            if (!MyClientServer.IsMwAccount)
            {
                if (!HandleSteamLogin(callAfterSuccessfulLogin))
                {
                    MyGuiManager.AddScreen(new MyGuiScreenLogin(MyConfig.Username, MyConfig.Password, callAfterSuccessfulLogin));
                }
            }
            else
            {
                callAfterSuccessfulLogin();
            }
        }

        public static void AddLoginScreenDrmFree(MyGuiScreenBase openAfterSuccessfulLogin)
        {
            AddLoginScreenDrmFree(GetAction(openAfterSuccessfulLogin));
        }

        // For DRM free version, it logins user without connecting to server
        public static void AddLoginScreenDrmFree(Action callAfterSuccessfulLogin)
        {
            if (MyClientServer.LoggedPlayer == null)
            {
                if (MySteam.IsActive)
                {
                    // Allow cheats and 2.5D to all Steam users
                    MyClientServer.LoggedPlayer = new MyPlayerLocal(new StringBuilder(MySteam.UserName), MyPlayerLocal.OFFLINE_MODE_USERID, true, true, false, false, false,
                        true, false, new StringBuilder(MySteam.UserName), new StringBuilder(String.Empty), true)
                    {
                        AdditionalInfo = new MyAdditionalUserInfo()
                    };

                    callAfterSuccessfulLogin();
                }
                else
                {
                    AddLoginScreen(callAfterSuccessfulLogin);
                }
            }
            else
            {
                //  If we are already logged in, we can start desired screen
                callAfterSuccessfulLogin();
            }
        }

        private static Action GetAction(MyGuiScreenBase openAfterSuccessfulLogin)
        {
            var action = new Action(delegate() { if (openAfterSuccessfulLogin != null) MyGuiManager.AddScreen(openAfterSuccessfulLogin); });
            return action;
        }

        private static bool HandleSteamLogin(Action loginSuccessAction)
        {
            if (MySteam.IsActive)
            {
                if (m_loginInProgress)
                {
                    Debug.Fail("Login is already in progress!");
                    MyMwcLog.WriteLine("Login called twice, second call stack trace:");
                    MyMwcLog.WriteLine(Environment.StackTrace);

                    // Just do nothing, another login is already in progress
                    return true;
                }

                m_loginInProgress = true;

                MyMasterServerAction steamLogin = new MyMasterServerAction(MyTextsWrapperEnum.LoginInProgressPleaseWait);
                steamLogin.SetPublicCredentials();
                if (MyMwcFinalBuildConstants.STEAM_DEMO)
                {
                    steamLogin.BeginAction = (c) => c.BeginSteamDemoLogin(MySteam.UserId, MySteam.SessionTicket, null, c);
                }
                else
                {
                    steamLogin.BeginAction = (c) => c.BeginSteamLogin(MySteam.UserId, MySteam.SessionTicket, null, c);
                }
                steamLogin.EndAction = (c, r) => OnSteamLoginEnd(c, r, loginSuccessAction);
                steamLogin.Start();
                return true;
            }
            return false;
        }

        private static void OnSteamLoginEnd(MyMasterServiceClient client, IAsyncResult result, Action loginSuccessAction)
        {
            try
            {
                string username, token;
                if (MyMwcFinalBuildConstants.STEAM_DEMO)
                {
                    username = client.EndSteamDemoLogin(out token, result);
                }
                else
                {
                    username = client.EndSteamLogin(out token, result);
                }
                var screen = new MyGuiScreenLoginProgress(username, token, () =>
                    {
                        MyClientServer.LoggedPlayer.LoggedUsingSteam = true;
                        MyClientServer.LoggedPlayer.SetDisplayName(new StringBuilder(MySteam.UserName));
                        loginSuccessAction();
                    }, null);
                screen.CloseScreenBeforeCallingHandler = true;
                screen.PasswordHash = token;
                MyGuiManager.AddScreen(screen);

                m_loginInProgress = false;
            }
            catch (FaultException<MySteamFault> e)
            {
                if (e.Detail.SteamFaultCode == SteamFaultCode.NotRegistered)
                {
                    MyMasterServerAction registerAction = new MyMasterServerAction(MyTextsWrapperEnum.LoginInProgressPleaseWait);
                    registerAction.SetPublicCredentials();
                    if (String.IsNullOrEmpty(MySteam.SerialKey))
                    {
                        m_loginInProgress = false;
                        OnSteamRegisterError(new InvalidOperationException("Cannot obtain CD key from Steam"));
                    }
                    else
                    {
                        registerAction.BeginAction = (c) => c.BeginSteamRegister(MySteam.UserId, MySteam.SessionTicket, MySteam.SerialKey, MySteam.UserName, null, c);
                        registerAction.EndAction = (c, r) => c.EndSteamRegister(r);
                        registerAction.ActionSuccess += () => OnSteamRegister(loginSuccessAction);
                        registerAction.ActionFailed += OnSteamRegisterError;
                        registerAction.ShowErrorMessage = false;
                        registerAction.Start();
                    }
                }
                else if (e.Detail.SteamFaultCode == SteamFaultCode.NoMW1ProductsOwned)
                {
                    m_loginInProgress = false;
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SteamNoProductsText, MyTextsWrapperEnum.SteamNoProductsCaption, MyTextsWrapperEnum.Ok, null));
                }
                else if (e.Detail.SteamFaultCode == SteamFaultCode.InvalidTicket)
                {
                    m_loginInProgress = false;
                    MySteam.RefreshSessionTicket();
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SteamInvalidTicketText, MyTextsWrapperEnum.SteamInvalidTicketCaption, MyTextsWrapperEnum.Ok, null));
                }
                else
                {
                    m_loginInProgress = false;
                    throw;
                }
            }
            finally
            {
                m_loginInProgress = false;
            }
        }

        private static void OnSteamRegister(Action loginSuccessAction)
        {
            m_loginInProgress = false;
            HandleSteamLogin(loginSuccessAction);
        }

        private static void OnSteamRegisterError(Exception e)
        {
            m_loginInProgress = false;
            string errorMessage = e.Message;
            if (e is FaultException<MySteamFault>)
            {
                var code = ((FaultException<MySteamFault>)e).Detail.SteamFaultCode;
                errorMessage = "Known code " + (int)code + ", " + code.ToString();
            }

            var text = MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.SteamRegisterErrorText, new object[] { MySteam.UserId, errorMessage, MySteam.SerialKey ?? String.Empty });
            var caption = MyTextsWrapper.Get(MyTextsWrapperEnum.SteamRegisterErrorCaption);

            var thread = new Thread(() => System.Windows.Forms.Clipboard.SetText(text));
            thread.Name = "SteamRegisterError";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, new StringBuilder(text), caption, MyTextsWrapperEnum.Ok, null));
        }

        //  This is for adding auto-login screen the easy way
        public static void AddAutologinScreen()
        {
            if (!MyClientServer.IsMwAccount)
            {
                if (MyMwcFinalBuildConstants.IS_CLOUD_GAMING)
                {
                    // Allow cheats and 2.5D to all Steam users
                    MyClientServer.LoggedPlayer = new MyPlayerLocal(new StringBuilder("Player"), MyPlayerLocal.OFFLINE_MODE_USERID, true, true, false, false, false,
                        true, false, new StringBuilder("Player"), new StringBuilder(String.Empty), true)
                    {
                        AdditionalInfo = new MyAdditionalUserInfo()
                    };
                }
                else if ((MyConfig.Autologin == true) && (MyConfig.LastLoginWasSuccessful == true))
                {
                    //  We will try autologin only one time - at the beginning of application life. Not later when user gets disconnected during gameplay.
                    if (m_autologinAttempts == 0)
                    {
                        m_autologinAttempts++;

                        string username = MyConfig.Username;
                        string password = MyConfig.Password;
                        MyGuiManager.AddScreen(new MyGuiScreenLoginProgress(username, password, (MyGuiScreenBase)null, null));
                    }
                }
            }
        }

        public MyGuiScreenMainMenu(bool showVideoOptions)
            : base(Vector2.Zero, null, null)
        {
            m_showVideoOptions = showVideoOptions;

            if (MyGuiScreenGamePlay.Static == null)
            {
                m_closeOnEsc = false;
            }
            else if (MyGuiScreenGamePlay.Static.IsPausable())
            {
                MyMinerGame.SwitchPause();
            }

            //if (MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.MAIN_MENU) m_closeOnEsc = false;
            //if (MyGuiScreenGamePlay.Static.IsPausable()) MyMinerGame.SwitchPause();

            //Because then it is visible under credits, help, etc..
            m_drawEvenWithoutFocus = false;

            MyClientServer.OnLoggedPlayerChanged += new EventHandler(MyClientServer_OnLoggedPlayerChanged);
        }

        void MyClientServer_OnLoggedPlayerChanged(object sender, EventArgs e)
        {
            RecreateControls(false);
        }

        //  Because only main menu's controla depends on fullscreen pixel coordinates (not normalized), after we change
        //  screen resolution we need to recreate controls too. Otherwise they will be still on old/bad positions, and
        //  for example when changing from 1920x1200 to 800x600 they would be out of screen
        public override void RecreateControls(bool constructor)
        {
            Controls.Clear();

            MyGuiControlButtonTextAlignment menuButtonTextAlignment = MyGuiControlButtonTextAlignment.CENTERED;

            Vector2 leftMenuPositionOrigin = GetMenuLeftBottomPosition() + new Vector2(MyGuiConstants.MAIN_MENU_BUTTON_SIZE.X / 2f, -MyGuiConstants.MAIN_MENU_BUTTON_SIZE.Y / 2f);

            if (MyGuiScreenGamePlay.Static == null || MyGuiScreenGamePlay.Static.IsMainMenuActive())
            {
                // Left main menu part

                MyTexture2D buttonTexture = MyGuiManager.GetButtonTexture();

                //buttons background
                m_mainMenuPanelPosition = leftMenuPositionOrigin - 4.1f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA;


                m_mainMenuPanelSize = new Vector2(574f / 1600f, 901.0f / 1200f);

                var a = MyGuiManager.GetSafeFullscreenRectangle();
                var fullScreenSize = new Vector2(a.Width / (a.Height * (4 / 3f)), 1f);

                MyTextsWrapperEnum? isDemo = null;
                if (MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.IsDemoUser()) isDemo = MyTextsWrapperEnum.NotAvailableInDemoMode;


                Controls.Add(new MyGuiControlPanel(this, new Vector2(0.5f, 0.5f), fullScreenSize, MyGuiConstants.SCREEN_BACKGROUND_COLOR,
                        m_MainmenuOverlay, null, null, null,
                        MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

                //leftMenuPositionOrigin -= new Vector2(-0.0012f, 0.072f) - 0.87f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA - new Vector2(0, 0.035f);

                // if (!MyClientServer.MW25DEnabled)
                {
                    // Story
                    Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 6f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                        buttonTexture, null, null,
                        MyTextsWrapperEnum.PlayStory, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignment, OnPlayStoryClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyFakes.PLAY_STORY_BUTTON_IMPLEMENTED, true, false));
                }

                bool multiplayerEnabled = true;
                // if (MyClientServer.MW25DEnabled && ((MyClientServer.LoggedPlayer != null) && (MyClientServer.LoggedPlayer.GetCanSave() == false)))
                //   multiplayerEnabled = false;

                // Sandbox
                MyGuiControlButton btn2 = new MyGuiControlButton(this, leftMenuPositionOrigin - 5f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    buttonTexture, null, null,
                    MyTextsWrapperEnum.PlaySandbox, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignment, OnMultiplayerClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, multiplayerEnabled, true, false);
                Controls.Add(btn2);
                if (!MyClientServer.HasFullGame)
                {
                    btn2.AccessForbiddenReason = isDemo;
                }
                btn2.DrawRedTextureWhenDisabled = false;

                // Editor
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 4f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    buttonTexture, null, null,
                    MyTextsWrapperEnum.Editor, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignment, OnEditorClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyFakes.EDITOR_BUTTON_IMPLEMENTED, true, false));

                // Options
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 3f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    buttonTexture, null, null,
                    MyTextsWrapperEnum.Options, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignment, OnOptionsClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true, false));

                // Help
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 2f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    buttonTexture, null, null,
                    MyTextsWrapperEnum.Help, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignment, OnHelpClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true, false));

                // Credits
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 1f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    buttonTexture, null, null,
                    MyTextsWrapperEnum.Credits, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignment, OnFlyThroughAnimationClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyFakes.CREDITS_BUTTON_IMPLEMENTED, true, false));

                // Exit to windows
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 0f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    buttonTexture, null, null,
                    MyTextsWrapperEnum.ExitToWindows, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignment, OnExitToWindowsClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true, false));


                Vector2 textRightTopPosition = MyGuiManager.GetScreenTextRightTopPosition();
                Vector2 position = textRightTopPosition + 8f * MyGuiConstants.CONTROLS_DELTA + new Vector2(-.1f, .06f);

                // Profile
                AddProfileButton(menuButtonTextAlignment, position);

                if (MyFakes.FAKE_SCREEN_ENABLED)
                {
                    Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 7f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    buttonTexture, null, null,
                    MyTextsWrapperEnum.FakeScreen, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignment, OnFakeScreenClick, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true, false));
                }

                //  Set mouse cursor near first button so it will loke nicer and won't be in the middle of screen
                SetMouseCursorPosition(constructor, leftMenuPositionOrigin - 6.7f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA + new Vector2(MyGuiConstants.MAIN_MENU_BUTTON_SIZE.X * 0.3f, 0));
            }
            else if (MyGuiScreenGamePlay.Static.GetGameType() != MyGuiScreenGamePlayType.GAME_STORY)
            {
                //buttons background
                m_mainMenuPanelPosition = leftMenuPositionOrigin - 0.5f * 4 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA;


                var a = MyGuiManager.GetSafeFullscreenRectangle();
                var fullScreenSize = new Vector2(a.Width / (a.Height * (4 / 3f)), 1f);
                Controls.Add(new MyGuiControlPanel(this, new Vector2(0.5f, 0.5f), fullScreenSize, MyGuiConstants.SCREEN_BACKGROUND_COLOR,
                    m_MainmenuOverlay, null, null, null,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 3 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.ContinueUppercase, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnContinueClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

                Vector2 textRightTopPosition = MyGuiManager.GetScreenTextRightTopPosition();
                Vector2 position = textRightTopPosition + 8f * MyGuiConstants.CONTROLS_DELTA + new Vector2(-.1f, .06f);

                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 2 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Options, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOptionsClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 1 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Help, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnHelpClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 0 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.ExitToMainMenu, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnExitToMainMenuClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

                AddProfileButton(menuButtonTextAlignment, position);

                //  Set mouse cursor near first button so it will loke nicer and won't be in the middle of screen
                SetMouseCursorPosition(constructor, leftMenuPositionOrigin - 4f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA + new Vector2(MyGuiConstants.MAIN_MENU_BUTTON_SIZE.X * 0.3f, 0));
            }
            else if (MyGuiScreenGamePlay.Static.IsGameActive())
            {
                bool canEditStory = MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.GetCanAccessEditorForStory();
                int buttonCount = canEditStory ? 6 : 5;


                bool isDemo = MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.IsDemoUser();
                if (isDemo)
                {
                    buttonCount--;
                }

                bool canLoad = (!MyMultiplayerGameplay.IsRunning || MyMultiplayerGameplay.Static.IsHost);

                //buttons background
                m_mainMenuPanelPosition = leftMenuPositionOrigin - 0.5f * buttonCount * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA;

                var a = MyGuiManager.GetSafeFullscreenRectangle();
                var fullScreenSize = new Vector2(a.Width / (a.Height * (4 / 3f)), 1f);
                Controls.Add(new MyGuiControlPanel(this, new Vector2(0.5f, 0.5f), fullScreenSize, MyGuiConstants.SCREEN_BACKGROUND_COLOR,
                    m_MainmenuOverlay, null, null, null,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

                //leftMenuPositionOrigin.Y += 0.04f;
                int buttonIndex = buttonCount;
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - ((float)(--buttonIndex)) * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.ContinueUppercase, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnContinueClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                if (!isDemo && canLoad)
                {
                    Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - ((float)(--buttonIndex)) * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                        MyTextsWrapperEnum.LoadCheckpoint, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnRestartClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                }

                Vector2 textRightTopPosition = MyGuiManager.GetScreenTextRightTopPosition();
                Vector2 position = textRightTopPosition + 8f * MyGuiConstants.CONTROLS_DELTA + new Vector2(-.1f, .06f);


                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - ((float)(--buttonIndex)) * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Options, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOptionsClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - ((float)(--buttonIndex)) * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Help, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnHelpClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - ((float)(--buttonIndex)) * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.ExitToMainMenu, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnExitToMainMenuClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

                AddProfileButton(menuButtonTextAlignment, position);

                if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost) //TODO: this will be removed later
                {
                    position = textRightTopPosition + 10f * MyGuiConstants.CONTROLS_DELTA + new Vector2(-.025f, .06f);
                    if (MyMultiplayerGameplay.Static.JoinMode == MyJoinMode.Closed)
                    {
                        Controls.Add(new MyGuiControlLabel(this, position, null, MyTextsWrapperEnum.JoinModeStateClosed, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));

                        position = textRightTopPosition + 11f * MyGuiConstants.CONTROLS_DELTA + new Vector2(-.10f, .06f);
                        m_btnJoinMode = new MyGuiControlButton(this, position, MyGuiConstants.BACK_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR, MyTextsWrapper.Get(MyTextsWrapperEnum.EnableCoop), new StringBuilder(""), MyGuiConstants.BACK_BUTTON_TEXT_COLOR, MyGuiConstants.BACK_BUTTON_TEXT_SCALE, cbJoinMode_OnSelect, MyGuiControlButtonTextAlignment.CENTERED, false, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
                        Controls.Add(m_btnJoinMode);

                    }
                    else if (MyMultiplayerGameplay.Static.JoinMode == MyJoinMode.Open)
                    {

                        Controls.Add(new MyGuiControlLabel(this, position, null, MyTextsWrapperEnum.JoinModeStateOpen, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));

                        position = textRightTopPosition + 11f * MyGuiConstants.CONTROLS_DELTA + new Vector2(-.1f, .06f);
                        m_btnJoinMode = new MyGuiControlButton(this, position, MyGuiConstants.BACK_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR, MyTextsWrapper.Get(MyTextsWrapperEnum.DisableCoop), new StringBuilder(""), MyGuiConstants.BACK_BUTTON_TEXT_COLOR, MyGuiConstants.BACK_BUTTON_TEXT_SCALE, cbJoinMode_OnSelect, MyGuiControlButtonTextAlignment.CENTERED, false, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
                        Controls.Add(m_btnJoinMode);

                    }
                }

                //  Set mouse cursor near first button so it will loke nicer and won't be in the middle of screen
                SetMouseCursorPosition(constructor, leftMenuPositionOrigin - 4f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA + new Vector2(MyGuiConstants.MAIN_MENU_BUTTON_SIZE.X * 0.3f, 0));
            }
            else if (MyGuiScreenGamePlay.Static.IsFlyThroughActive())
            {
                //buttons background
                m_mainMenuPanelPosition = leftMenuPositionOrigin - 1.5f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA;


                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 3 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.ContinueUppercase, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnContinueClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 2 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Options, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOptionsClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 1 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Help, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnHelpClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 0 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.ExitToMainMenu, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnExitToMainMenuClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

                //  Set mouse cursor near first button so it will loke nicer and won't be in the middle of screen
                SetMouseCursorPosition(constructor, leftMenuPositionOrigin - 3f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA + new Vector2(MyGuiConstants.MAIN_MENU_BUTTON_SIZE.X * 0.3f, 0));
            }
            else if (MyGuiScreenGamePlay.Static.IsEditorActive())
            {
                //buttons background
                var a = MyGuiManager.GetSafeFullscreenRectangle();
                var fullScreenSize = new Vector2(a.Width / (a.Height * (4 / 3f)), 1f);
                Controls.Add(new MyGuiControlPanel(this, new Vector2(0.5f, 0.5f), fullScreenSize, MyGuiConstants.SCREEN_BACKGROUND_COLOR,
                    m_MainmenuOverlay, null, null, null,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

                //leftMenuPositionOrigin.Y -= 0.0114f;
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 3 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.ContinueUppercase, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnContinueClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 2 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Options, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOptionsClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 1 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Help, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnHelpClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, leftMenuPositionOrigin - 0 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.ExitToMainMenu, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnExitToMainMenuClick, menuButtonTextAlignment, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));


                Vector2 textRightTopPosition = MyGuiManager.GetScreenTextRightTopPosition();
                Vector2 position = textRightTopPosition + 8f * MyGuiConstants.CONTROLS_DELTA + new Vector2(-.1f, .06f);

                AddProfileButton(menuButtonTextAlignment, position);

                //  Set mouse cursor near first button so it will loke nicer and won't be in the middle of screen
                SetMouseCursorPosition(constructor, leftMenuPositionOrigin - 3f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA + new Vector2(MyGuiConstants.MAIN_MENU_BUTTON_SIZE.X * 0.3f, 0));
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        void cbJoinMode_OnSelect(MyGuiControlButton sender)
        {
            //var joinMode = (MyJoinMode)m_btnJoinMode.GetSelectedKey();

            var joinMode = MyMultiplayerGameplay.Static.JoinMode == MyJoinMode.Closed ? MyJoinMode.Open : MyJoinMode.Closed;
            MyMultiplayerGameplay.Static.JoinMode = joinMode;
            MyMultiplayerGameplay.Static.UpdateGameInfo();
            Canceling();
        }

        void AddProfileButton(MyGuiControlButtonTextAlignment menuButtonTextAlignment, Vector2 position)
        {
            //if (MySectorServiceClient.IsConnected && !(MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.LoggedUsingSteam))
            //position += new Vector2(0, -0.02f);

            MyTextsWrapperEnum text = MyTextsWrapperEnum.Profile;
            if (!MySteam.IsActive && !MyClientServer.IsMwAccount)
            {
                // When demo and in-game, don't show profile/login button
                if (!(MyGuiScreenGamePlay.Static == null || MyGuiScreenGamePlay.Static.IsMainMenuActive()))
                {
                    return;
                }

                text = MyTextsWrapperEnum.Login;
            }

            Controls.Add(
                new MyGuiControlButton(
                    this,
                    position,
                    MyGuiConstants.BACK_BUTTON_SIZE,
                    MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    text,
                    MyGuiConstants.BUTTON_TEXT_COLOR,
                    MyGuiConstants.BUTTON_TEXT_SCALE,
                    OnProfileClick,
                    menuButtonTextAlignment,
                    true,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                    MyFakes.PROFILE_BUTTON_IMPLEMENTED));
        }

        public void SetMouseCursorPosition(bool constructor, Vector2 position)
        {
            //  Set mouse cursor near first button so it will loke nicer and won't be in the middle of screen
            if (constructor == true)
            {
                MyGuiManager.MouseCursorPosition = position;
            }
        }

        //  When dual-head - this will draw the menu to the left side of left monitor. That's OK.
        //  Only in triple-head it's moved to the center monitor.
        public static Vector2 GetMenuLeftBottomPosition()
        {
            float deltaPixels = 70 * MyGuiManager.GetSafeScreenScale();
            Rectangle fullscreenRectangle = MyGuiManager.GetSafeFullscreenRectangle();
            return MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(deltaPixels, fullscreenRectangle.Height - deltaPixels));
        }

        public Vector2 GetMenuRightBottomPosition()
        {
            float deltaPixels = 100 * MyGuiManager.GetSafeScreenScale();
            Rectangle fullscreenRectangle = MyGuiManager.GetSafeFullscreenRectangle();
            return GetMenuLeftBottomPosition() + MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(fullscreenRectangle.Width - deltaPixels * 2, 0));
        }

        /*public void OnPlayMissionPlaygroundClick(MyGuiControlButton sender)
        {
            AddLoginScreen(StartPlayground);
        }

        public static void StartPlayground()
        {
            AddLoginScreen(StartPlaygroundAfterLogin);
        }

        static void StartPlaygroundAfterLogin()
        {
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
            MySession.StartSandbox(MyHubShowcaseMission.BaseSector, null);
        }*/

        public void OnPlayStoryClick(MyGuiControlButton sender)
        {
            AddLoginScreenDrmFree(new MyGuiScreenSelectStory(this));
        }

        public void OnMultiplayerClick(MyGuiControlButton sender)
        {
            AddLoginScreenDrmFree(new MyGuiScreenMultiplayer(this));
        }

        public void OnEditorClick(MyGuiControlButton sender)
        {
            //if (MyClientServer.LoggedPlayer == null || MyClientServer.LoggedPlayer.GetCanAccessEditorForStory() || MyClientServer.LoggedPlayer.GetCanAccessEditorForMMO())
            //{
            //    AddLoginScreen(new MyGuiScreenSelectEditor(this));
            //}
            //else if (!MySteam.IsActive && !MyClientServer.IsMwAccount) // Demo user...generate sector 0, 0, 0. User cant save.
            //{
                MyGuiManager.CloseAllScreensNowExcept(MyGuiScreenGamePlay.Static);

                MySession.Static = new MySandboxSession();
                MySession.Static.Init();

                var sector = new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.SANDBOX, MyPlayerLocal.OFFLINE_MODE_USERID, new CommonLIB.AppCode.Utils.MyMwcVector3Int(0, 0, 0), String.Empty);
                var newGameplayScreen = new MyGuiScreenGamePlay(MyGuiScreenGamePlayType.EDITOR_SANDBOX, null, sector, 0, MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX);
                var loadScreen = new MyGuiScreenLoading(newGameplayScreen, MyGuiScreenGamePlay.Static);

                var checkpoint = new MyMwcObjectBuilder_Checkpoint();
                checkpoint.CurrentSector = sector;
                checkpoint.SectorObjectBuilder = new MyMwcObjectBuilder_Sector();
                checkpoint.SectorObjectBuilder.FromGenerator = true;
                checkpoint.SectorObjectBuilder.Position = sector.Position;
                checkpoint.SessionObjectBuilder = new MyMwcObjectBuilder_Session(MyGameplayDifficultyEnum.EASY);

                loadScreen.AddEnterSectorResponse(checkpoint, null);

                MyGuiManager.AddScreen(loadScreen);
            //}
            //else
            //{
            //    MyMwcSectorTypeEnum sectorType = MyMwcClientServer.GetSectorTypeFromSessionType(MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX);
            //    AddLoginScreen(
            //        new MyGuiScreenLoadSectorIdentifiersProgress(sectorType, false, new MyGuiScreenEnterSectorMap(this, MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX, MyTextsWrapperEnum.StartEditorInProgressPleaseWait, MyConfig.LastSandboxSector)));
            //}
        }

        public void OnRestartClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MessageAreYouSureYouWantLoadCheckpoint, MyTextsWrapperEnum.LoadCheckpoint, MyTextsWrapperEnum.Ok, MyTextsWrapperEnum.No, Restart));
        }

        private void Restart(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
                MySession.StartLastCheckpoint();
            }
        }

        public void OnContinueClick(MyGuiControlButton sender)
        {
            MyGuiScreenGamePlay.Static.DrawHud = true;
            CloseScreen();
        }

        public static void OnExitToMainMenuClick(MyGuiControlButton sender)
        {
            MyGuiManager.GetMainMenuScreen().m_screenCanHide = false;
            var messageBox = new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.AreYouSureYouWantToExit, MyTextsWrapperEnum.MessageBoxExitQuestion, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No, OnExitToMainMenuMessageBoxCallback);
            messageBox.SkipTransition = true;
            messageBox.InstantClose = false;
            MyGuiManager.AddScreen(messageBox);
        }

        public static void OnExitToMainMenuMessageBoxCallback(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                // No mission active...ask whether save or not
                if (MySession.Static != null && MyMissions.ActiveMission == null && MyGuiScreenGamePlay.Static.IsGameStoryActive() && MyClientServer.LoggedPlayer.GetCanSave())
                {
                    var messageBox = new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SaveCurrentProgress, MyTextsWrapperEnum.SaveCurrentProgressCaption, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No, OnSaveGameResponse);
                    messageBox.SkipTransition = true;
                    messageBox.InstantClose = false;
                    MyGuiManager.AddScreen(messageBox);
                }
                else
                {
                    UnloadAndExitToMenu();
                }
            }
            else
            {
                MyGuiManager.GetMainMenuScreen().m_screenCanHide = true;
            }
        }

        private static void OnSaveGameResponse(MyGuiScreenMessageBoxCallbackEnum result)
        {
            if (result == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                MySession.Static.SaveLastCheckpoint();
            }
            UnloadAndExitToMenu();
        }

        public static void UnloadAndExitToMenu()
        {
            //  Leave current sector
            if (MyGuiScreenGamePlay.Static.IsGameActive())
            {
                //MyClientServer.SendMessageLeaveSectorRequest(MyMwcLeaveSectorReasonEnum.EXIT_TO_MAIN_MENU, null);
                //TODO - until I find how to make interactive music to stop in game when exit smoothly, I put this here
                //MyGuiScreenGamePlay.Static.StopMusic();
                MyMissions.Unload();
                //MyAudio.StopMusic();
                MyAudio.Stop();
            }

            //  If credits screen is open we must close it immediately so it won't make mess on screen during loading - the transition will look better
            if (MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.CREDITS) MyGuiManager.CloseScreenNow(typeof(MyGuiScreenGameCredits));

            //  This will quit actual game-play screen and move us to fly-through with main menu on top
            MyGuiManager.BackToMainMenu();

            //  We must close this screen immediately so it won't make mess on screen during loading - the transition will look better
            MyGuiManager.CloseScreen(typeof(MyGuiScreenMainMenu));
        }

        public void OnFlyThroughAnimationClick(MyGuiControlButton sender)
        {
            //opens dialog screen with list of trailers, where could be selected animation to play
            //MyGuiManager.AddScreen(new MyGuiScreenFlyThrough());
            MyGuiManager.AddScreen(new MyGuiScreenGameCredits());
        }

        public void OnProfileClick(MyGuiControlButton sender)
        {
            if (!MySteam.IsActive && !MyClientServer.IsMwAccount)
            {
                AddLoginScreen(new MyGuiScreenProfile());
            }
            else
            {
                AddLoginScreenDrmFree(new MyGuiScreenProfile());
            }
        }

        public void OnOptionsClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenOptions(m_showVideoOptions));
        }

        public void OnHelpClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenHelp());
        }

        public void OnCreditsClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenLoading(new MyGuiScreenGamePlay(MyGuiScreenGamePlayType.CREDITS, null,
                MyTrailerConstants.DEFAULT_SECTOR_IDENTIFIER, MyTrailerConstants.DEFAULT_SECTOR_VERSION, null), MyGuiScreenGamePlay.Static));

            //  We must close this screen immediately so it won't make fun on monitor during loading - the transition will look better
            CloseScreenNow();
        }

        public void OnFakeScreenClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenFake());
        }

        public void OnExitToWindowsClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.AreYouSureYouWantToExit, MyTextsWrapperEnum.MessageBoxExitQuestion, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No, OnExitToWindowsMessageBoxCallback));
        }

        public void OnExitToWindowsMessageBoxCallback(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                MyGuiManager.AddScreen(new MyGuiScreenLogoutProgress(OnLogoutProgressClosed));
            }
        }

        public void OnLogoutProgressClosed()
        {
            MyMwcLog.WriteLine("Application closed by user");
            //  Exit application
            MyMinerGame.Static.Invoke(() => { MyMinerGame.Static.Exit(); }, false);
        }

        public override void LoadContent()
        {
            m_timeFromMenuLoadedMS = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            base.LoadContent();

            if (MyGuiScreenGamePlay.Static == null)
            {
                m_MainmenuOverlay = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\MainMenuOverlay", flags: TextureFlags.IgnoreQuality);
            }
            else if (MyGuiScreenGamePlay.Static.IsMainMenuActive())
            {
            }
            else if (MyGuiScreenGamePlay.Static.IsGameActive())
            {
                m_MainmenuOverlay = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\MainMenuOverlay", flags: TextureFlags.IgnoreQuality);
            }
            else if (MyGuiScreenGamePlay.Static.IsFlyThroughActive())
            {

            }
            else if (MyGuiScreenGamePlay.Static.IsEditorActive())
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("load MainMenuOverlay");
                m_MainmenuOverlay = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\MainMenuOverlay", flags: TextureFlags.IgnoreQuality);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("load logo and banners");
            m_minerWarsLogoTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MinerWarsLogoLarge", loadingMode: Managers.LoadingMode.Immediate, flags: TextureFlags.IgnoreQuality);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            RecreateControls(true);
        }

        private void PlayMusic()
        {
            if (MyAudio.GetMusicState() == MyMusicState.Stopped &&
                    MyAudio.GetMusicCue() == null &&
                    !MyAudio.HasAnyTransition())
            {
                MyAudio.ApplyTransition(MyMusicTransitionEnum.MainMenu);
                //MyAudio.AddCue2D(MySoundCuesEnum.MenuWelcome);
            }
        }

        public override void UnloadContent()
        {
            MyClientServer.OnLoggedPlayerChanged -= MyClientServer_OnLoggedPlayerChanged;

            base.UnloadContent();
            if (m_MainmenuOverlay != null && m_MainmenuOverlay.LoadState == Managers.LoadState.Loaded)
            {
                MyTextureManager.UnloadTexture(m_MainmenuOverlay);
                m_MainmenuOverlay = null;
            }
        }

        private void OpenInternetBrowser(string url, MyTextsWrapperEnum messageFailed)
        {
            try
            {
                try
                {
                    System.Diagnostics.Process.Start(url);
                }
                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
                // then attempt to open the URL in IE instead.
                catch (System.ComponentModel.Win32Exception)
                {
                    // sometimes throws exception so we have to just ignore
                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
                    // the URL using IE if we can.
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(MyMainMenuConstants.IE_PROCESS, MyMainMenuConstants.BUY_NOW_URL);
                    System.Diagnostics.Process.Start(startInfo);
                    startInfo = null;
                }
            }
            catch (Exception)
            {
                // oper browser failed
                StringBuilder sbMessage = new StringBuilder();
                sbMessage.AppendFormat(MyTextsWrapper.GetFormatString(messageFailed), url);
                StringBuilder sbTitle = MyTextsWrapper.Get(MyTextsWrapperEnum.TitleFailedToStartInternetBrowser);
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, sbMessage, sbTitle, MyTextsWrapperEnum.Ok, null));
            }
        }

        protected override void Canceling()
        {
            if (MyGuiScreenGamePlay.Static != null && (MyGuiScreenGamePlay.Static.IsGameActive() || MyGuiScreenGamePlay.Static.IsEditorActive()))
            {
                MyGuiScreenGamePlay.Static.DrawHud = true;
                base.Canceling();
            }
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if (MyGuiScreenGamePlay.Static == null || MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.MAIN_MENU)
            {
                if (input.IsNewKeyPress(Keys.Escape))
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                    OnExitToWindowsClick(null);
                }
            }

            //if (input.IsNewKeyPress(Keys.Enter))
            //{
            //    MyGuiManager.AddScreen(new MyGuiScreenLoading(Vector2.Zero, null, null, null, new MyGuiScreenGamePlay(Vector2.Zero, null, null, null, false), MyGuiScreenGamePlay.Static));
            //    CloseScreen();
            //}

            //if (input.IsNewKeyPress(Keys.Escape))
            //{
            //    CloseScreen();
            //}

            //if (input.IsNewKeyPress(MinerWarsMath.Input.Keys.P))
            //{
            //    MyMinerGame.Static.UnloadContent();
            //}
        }

        void DrawLoggedPlayerName()
        {
            Vector2 textRightTopPosition = MyGuiManager.GetScreenTextRightTopPosition();
            Vector2 position = textRightTopPosition + 8f * MyGuiConstants.CONTROLS_DELTA;
            position.X -= 0.03f;

            if (MyClientServer.LoggedPlayer == null)
            {
                //  Draw "You are not logged in."
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.NotLoggedIn), position, MyGuiConstants.LOGED_PLAYER_NAME_TEXT_SCALE,
                    new Color(MyGuiConstants.LABEL_TEXT_COLOR * m_transitionAlpha), MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);
            }
            else
            {
                //  Draw player's name - from right side
                MyRectangle2D rect = MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyClientServer.LoggedPlayer.GetDisplayName(), position, MyGuiConstants.LOGED_PLAYER_NAME_TEXT_SCALE,
                    Color.White * m_transitionAlpha, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);

                m_playerNameString.Clear();

                if (!MySectorServiceClient.IsInstanceValid)
                {
                    m_playerNameString.Append("[");
                    m_playerNameString.AppendStringBuilder(MyTextsWrapper.Get(MyTextsWrapperEnum.Disconnected));
                    m_playerNameString.Append("]");
                }
                if (MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.LoggedUsingSteam)
                {
                    m_playerNameString.Append("[Steam]");
                }

                if (m_playerNameString.Length > 0)
                {
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_playerNameString, position, MyGuiConstants.LOGED_PLAYER_NAME_TEXT_SCALE,
                    Color.White * m_transitionAlpha, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);
                }

                //  Draw constant string "Logged Player"
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.LoggedPlayer), rect.LeftTop + new Vector2(-0.0075f, rect.Size.Y - 0.001f), MyGuiConstants.LOGED_PLAYER_NAME_TEXT_SCALE /** 0.8f*/,
                    new Color(MyGuiConstants.LABEL_TEXT_COLOR * m_transitionAlpha), MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);
            }
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            if (ret == true)
            {
                if (MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.IsPausable() && MyMinerGame.IsPaused())
                    MyMinerGame.SwitchPause();
            }
            return ret;
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (MyMinerGame.IsPaused() && m_closeOnEsc)
            {
                DrawBlackSemiTransparentBackground();
            }
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            DrawMinerWarsLogo();
            //DrawActorsPhotos();
            DrawLoggedPlayerName();
            DrawAppVersion();
            DrawGlobalVersionText();

            MyGuiScreenBase screenWithFocus = MyGuiManager.GetScreenWithFocus();

            return true;
        }

        private void DrawBlackSemiTransparentBackground()
        {
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), new Color(0, 0, 0, 120));
        }

        bool IsPurchaseBannerEnabled()
        {
            MyGuiScreenBase screenWithFocus = MyGuiManager.GetScreenWithFocus();
            return MyClientServer.LoggedPlayer == null || MyClientServer.LoggedPlayer.GetCanSave() == false && MyGuiManager.IsScreenOfTypeOpen(typeof(MyGuiScreenProgressBase)) == false;

        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            //MyMinerGame.GraphicsDeviceManager.DbgDumpLoadedResources(true);
            //MyTextureManager.DbgDumpLoadedTextures(true);

            if (!m_musicPlayed && MyMinerGame.TotalGamePlayTimeInMilliseconds - m_timeFromMenuLoadedMS >= PLAY_MUSIC_AFTER_MENU_LOADED_MS)
            {
                if (MyGuiScreenGamePlay.Static == null || MyGuiScreenGamePlay.Static.IsMainMenuActive())
                {
                    PlayMusic();
                }
                m_musicPlayed = true;
            }

            return true;
        }

        public static void StartMission(MyMissionID missionId)
        {
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

            var mission = MyMissions.GetMissionByID(missionId);

            Action loadedHandler = () =>
            {
                MyMissions.ActiveMission = null;
                MySession.Static.EventLog.Events.Clear();

                // Make prereq missions completed
                foreach (var prereq in mission.RequiredMissions)
                {
                    MySession.Static.EventLog.AddMissionStarted(prereq);
                    MySession.Static.EventLog.MissionFinished(prereq);
                }
            };

            var action = MySession.StartNewGame(MyFakes.DIFFICULTY_FOR_F12_MISSIONS, missionId);
            if (action != null)
            {
                action.ActionSuccess += loadedHandler;
            }
            else
            {
                loadedHandler();
            }
        }
    }
}