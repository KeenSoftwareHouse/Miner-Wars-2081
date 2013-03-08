using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Networking;
using MinerWars.AppCode.Game.World;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenProfile : MyGuiScreenBase
    {
        public MyGuiScreenProfile()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(574f / 1600f, 850 / 1200f))
        {
            m_enableBackgroundFade = true;
            bool displayLogoutButton = !(MyGuiScreenGamePlay.Static != null && (MyGuiScreenGamePlay.Static.IsGameActive() || MyGuiScreenGamePlay.Static.IsEditorActive()));
            displayLogoutButton = displayLogoutButton && !MySteam.IsActive && (MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.GetUserId() != MyPlayerLocal.OFFLINE_MODE_USERID);

            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ProfileBackground", flags: TextureFlags.IgnoreQuality);

            if (displayLogoutButton)
            {
                m_size += new Vector2(0, MyGuiConstants.BACK_BUTTON_SIZE.Y);
            }

            AddCaption(MyTextsWrapperEnum.Profile, new Vector2(0, 0.005f));

            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.145f);
            MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            float positionMultiplierY = 0;

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + positionMultiplierY * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.UserInfo, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnInfoClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            positionMultiplierY++;

            if (displayLogoutButton)
            {
                //  Don't display LOGOUT button while player is in active game - because I don't know what to do after logout in this case and it's not needed there
                Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + positionMultiplierY * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Logout, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnLogoutClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                positionMultiplierY++;
            }
            
            var backButton = new MyGuiControlButton(this, new Vector2(0, 0.2695f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Back,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignement, OnBackClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(backButton);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenProfile";
        }

        public void OnInfoClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenUserInfo());
        }

        public void OnLogoutClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.AreYouSureYouWantToLogout, MyTextsWrapperEnum.MessageBoxLogoutQuestion, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No, OnLogoutMessageBoxCallback));
        }

        public void OnLogoutMessageBoxCallback(MyGuiScreenMessageBoxCallbackEnum callbackReturn)
        {
            if (callbackReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
            {
                MyConfig.Username = "";
                MyConfig.Password = "";
                MyConfig.LastLoginWasSuccessful = false;
                MyConfig.Save();
                MyGuiManager.AddScreen(new MyGuiScreenLogoutProgress(OnLogoutProgressClosed));
            }
        }

        public void OnLogoutProgressClosed()
        {
            //  User can't be in profile after logout so we close it
            CloseScreen();
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            //  Just close the screen
            CloseScreen();
        }
    }
}
