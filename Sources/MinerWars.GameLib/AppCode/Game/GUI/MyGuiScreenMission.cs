using System.Diagnostics;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenMission : MyGuiScreenBase
    {
        private MyMission m_mission;

        public MyGuiScreenMission(MyMission missionBase, bool canDecline = true)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.75f, 0.75f))
        {
            Debug.Assert(missionBase != null);
            m_mission = missionBase;
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\MissionAcceptBackground", flags: TextureFlags.IgnoreQuality);
            m_size = new Vector2(1010/1600f,855/1200f);

            // Title
            var titleLabel = new MyGuiControlLabel(
                            this,
                            new Vector2(0, -0.3052f),
                            null,
                            MyTextsWrapperEnum.Mission,
                            MyGuiConstants.LABEL_TEXT_COLOR,
                            MyGuiConstants.SCREEN_CAPTION_TEXT_SCALE,
                            MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            // append mission name to the title of the screen
            titleLabel.UpdateParams(new[] { missionBase.Name });
            Controls.Add(titleLabel);

            // mission description on the left
            Controls.Add(new MyGuiControlLabel(
                                this, 
                                new Vector2(-m_size.Value.X / 4.5f, -0.2501f),
                                null, MyTextsWrapperEnum.Description,
                                MyGuiConstants.LABEL_TEXT_COLOR, .8f,
                                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));



            var multiLineText = new MyGuiControlMultilineText(
                this,
                new Vector2(-m_size.Value.X/4.5f - 28/1600f, -0.0167f),
                new Vector2(370/1600f, 489/1200f),
                null,
                MyGuiManager.GetFontMinerWarsBlue(), .66f,
                MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                m_mission.DescriptionTemp, false, true);


            Controls.Add(multiLineText);
            multiLineText.ScrollbarOffset = 71/1600f;

  
            // mission objectives (submissions) on the right
            Controls.Add(new MyGuiControlLabel(
                                this,
                                new Vector2(m_size.Value.X / 4.5f, -0.2501f),
                                null, MyTextsWrapperEnum.Objectives,
                                MyGuiConstants.LABEL_TEXT_COLOR, .8f,
                                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

            var objectives = new StringBuilder();
            foreach (MyObjective t in m_mission.Objectives)
            {

                    objectives.Append("- ");
                    objectives.Append(t.DescriptionTemp.ToString());
                    objectives.AppendLine("\n");
                    //objectives.AppendLine();
            }

            var textControl = new MyGuiControlMultilineText(
                this,
                new Vector2(m_size.Value.X/4.5f - 28/1600f, -0.0167f),
                new Vector2(370/1600f, 489/1200f),
                null,
                MyGuiManager.GetFontMinerWarsBlue(), .66f,
                MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                objectives, false, true) {ScrollbarOffset = 64/1600f};


            Controls.Add(textControl);

            textControl.Clear();
            textControl.AppendText(objectives);


            var okButton = new MyGuiControlButton(this, new Vector2(-0.1379f, 0.2489f), MyGuiConstants.OK_BUTTON_SIZE,
               MyGuiConstants.BUTTON_BACKGROUND_COLOR,
               MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Accept,
               MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnAccept_Click,
               true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(okButton);




            // decline button
            if (canDecline)
            {
                var declineButton = new MyGuiControlButton(this, new Vector2(0.1379f, 0.2489f), MyGuiConstants.OK_BUTTON_SIZE,
                   MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                   MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Decline,
                   MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnDecline_Click,
                   true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
                Controls.Add(declineButton);


            }
            else
            {
                m_closeOnEsc = canDecline;
            }
        }

        public void OnAccept_Click(MyGuiControlButton sender)
        {
            m_mission.Accept();

            CloseScreen();

            var session = MySession.Static;
            if (session != null && !MyFakes.DISABLE_AUTO_SAVE)
                session.SaveLastCheckpoint();
        }

        public void OnDecline_Click(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        public override bool CloseScreen()
        {
            bool result = base.CloseScreen();
            MyMissions.HandleCloseMissionScreen();
            return result;
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            //Journal removed
            /*if (input.IsNewGameControlPressed(MyGameControlEnums.MISSION_DIALOG))
            {
                CloseScreen();
            } */
        }

        public override void CloseScreenNow()
        {
            base.CloseScreenNow();
            MyMissions.HandleCloseMissionScreen();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenMission";
        }
    }
}
