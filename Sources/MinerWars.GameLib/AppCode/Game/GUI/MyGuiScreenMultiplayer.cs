using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenMultiplayer : MyGuiScreenBase
    {
        readonly MyGuiScreenBase m_closeAfterSuccessfulEnter;

        public MyGuiScreenMultiplayer(MyGuiScreenBase closeAfterSuccessfulEnter)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(574f / 1600f, 480 / 1200f), false, MyGuiManager.GetMultiplayerBackground())
        {
            m_closeAfterSuccessfulEnter = closeAfterSuccessfulEnter;
            m_enableBackgroundFade = true;
            AddCaption(MyTextsWrapperEnum.PlaySandbox, new Vector2(0, 0.005f));

            Debug.Assert(m_size != null, "m_size != null");
            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.135f);
            const MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            MyTextsWrapperEnum? buttonsForbidden = null;
            if (MyClientServer.LoggedPlayer != null)
            {
                if ((MyClientServer.LoggedPlayer.GetCanAccessDemo() == false) && (MyClientServer.LoggedPlayer.GetCanSave() == false))
                {
                    buttonsForbidden = MyTextsWrapperEnum.NotAccessRightsToTestBuild;
                }
            }

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 0 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.HostGame, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnHostGameClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, buttonsForbidden));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 1 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.JoinGame, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnJoinGameClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, buttonsForbidden));

            var backButton = new MyGuiControlButton(this, new Vector2(0, 0.095f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Back,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignement, OnBackClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(backButton);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenMultiplayer";
        }

        void OnHostGameClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenHostGame(m_closeAfterSuccessfulEnter));
        }

        void OnJoinGameClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenJoinGame(m_closeAfterSuccessfulEnter, MyGameTypes.Deathmatch | MyGameTypes.Story));
        }

        void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }
}