using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using App;
    using CommonLIB.AppCode.Utils;
    using MinerWars.AppCode.Game.Managers.Session;

    /// <summary>
    /// 
    /// </summary>
    class MyGuiScreenSelectEditor : MyGuiScreenBase 
    {
        MyGuiScreenBase m_closeAfterSuccessfulEnter;

        /// <summary>
        /// Tmp old
        /// </summary>
        private MyGuiScreenGamePlay gamePlayScreen;

        /// <summary>
        /// Selected session
        /// </summary>
        private MySession session;

        public MyGuiScreenSelectEditor(MyGuiScreenBase closeAfterSuccessfulEnter)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(574f/1600f, 695/1200f), false, MyGuiManager.GetSelectEditorBackground())
        {
            m_enableBackgroundFade = true;
            m_closeAfterSuccessfulEnter = closeAfterSuccessfulEnter;

            AddCaption(MyTextsWrapperEnum.Editor, new Vector2(0, 0.005f));
        }

        //  Buttons must be created here because in this screen's constructor we might have not been logged in
        public override void LoadContent()
        {
            base.LoadContent();

            Vector2 menuPositionOrigin = new Vector2(0.0f, -m_size.Value.Y / 2.0f + 0.145f);
            const MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            MyTextsWrapperEnum? yourSectorsButtonForbidden = null;
            MyTextsWrapperEnum? storySectorsButtonsForbidden = null;
            MyTextsWrapperEnum? mmoSectorsButtonsForbidden = null;
            if (MyClientServer.LoggedPlayer != null)
            {
                //if (true/*(MyClientServer.LoggedPlayer.GetCanAccessDemo() == false) && (MyClientServer.LoggedPlayer.GetCanAccessStory() == false)*/)
                //{
                //    yourSectorsButtonForbidden = MyTextsWrapperEnum.NotAccessRightsToTestBuild;
                //}

                if (!MyClientServer.LoggedPlayer.GetCanAccessEditorForStory())
                {
                    storySectorsButtonsForbidden = MyTextsWrapperEnum.AvailableOnlyForAdministrators;
                }

                if (true /*MyClientServer.LoggedPlayer.GetCanAccessEditorForMMO() == false*/)
                {
                    mmoSectorsButtonsForbidden = MyTextsWrapperEnum.AvailableOnlyForAdministrators;
                }
            }

            //  Buttons My Sectors and Friends Sectors
            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 0 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.SandboxSectors, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnSandboxSectorsClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, storySectorsButtonsForbidden));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 1 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.YourSectors, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnYourSectorsClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, yourSectorsButtonForbidden));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 2 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.StorySectors, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnStorySectorsClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, storySectorsButtonsForbidden));

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 3 * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA,
                MyGuiConstants.MAIN_MENU_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.MmoSectors, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnMmoSectorsClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true, mmoSectorsButtonsForbidden));
            var exitButton = new MyGuiControlButton(this, new Vector2(0,0.1460f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetConfirmButton(), null, null, MyTextsWrapperEnum.Back,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, menuButtonTextAlignement, OnBackClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(exitButton);


            /*

            Controls.Add(new MyGuiControlButton(this, menuPositionOrigin + 3.5f * MyGuiConstants.MENU_BUTTONS_POSITION_DELTA, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Back, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnBackClick, menuButtonTextAlignement, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
             * */
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenSelectEditor";
        }

        public void OnYourSectorsClick(MyGuiControlButton sender)
        {
            MyMwcLog.WriteLine("OnYourSectorsClick - Start"); 
            Run(MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX, false);
            MyMwcLog.WriteLine("OnYourSectorsClick - End"); 
        }

        public void OnStorySectorsClick(MyGuiControlButton sender)
        {
            MyMwcLog.WriteLine("OnStorySectorsClick - Start"); 
            Run(MyMwcStartSessionRequestTypeEnum.EDITOR_STORY, true);
            MyMwcLog.WriteLine("OnStorySectorsClick - End"); 
        }

        public void OnSandboxSectorsClick(MyGuiControlButton sender)
        {
            MyMwcLog.WriteLine("OnSandboxSectorsClick - Start");
            Run(MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX, true);
            MyMwcLog.WriteLine("OnSectorsClick - End"); 
        }

        public void OnMmoSectorsClick(MyGuiControlButton sender)
        {
            MyMwcLog.WriteLine("OnMmoSectorsClick - Start"); 
            Run(MyMwcStartSessionRequestTypeEnum.EDITOR_MMO, true);
            MyMwcLog.WriteLine("OnMmoSectorsClick - End"); 
        }

        //  TODO: This is just temporary method that launches editor. Later must be differentiate according to what user picked
        void Run(MyMwcStartSessionRequestTypeEnum sessionRequestType, bool global)
        {
            MyMwcSectorTypeEnum sectorType = MyMwcClientServer.GetSectorTypeFromSessionType(sessionRequestType);
            MyGuiManager.AddScreen(new MyGuiScreenLoadSectorIdentifiersProgress(sectorType, global, new MyGuiScreenEnterSectorMap(m_closeAfterSuccessfulEnter,
                sessionRequestType, MyTextsWrapperEnum.StartEditorInProgressPleaseWait, MyConfig.LastSandboxSector)));
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
     }
}
