/*
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Trailer;
using MinerWars.AppCode.Game.Utils;

//  This is screen containing combobox with all trailer animations configured in Trailer.xmlx file
namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenFlyThrough : MyGuiScreenBase
    {
        MyGuiControlCombobox m_flyThroughAnimationCombobox;
        MyGuiControlCheckbox m_displayCreditsCheckBox;
        MyGuiControlCheckbox m_displayFPSCheckBox;

        public MyGuiScreenFlyThrough()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;

            m_size = new Vector2(0.57f, 0.4f);
            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.05f, -m_size.Value.Y / 2.0f + 0.145f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.25f, -m_size.Value.Y / 2.0f + 0.145f);

            AddCaption(MyTextsWrapperEnum.CreditsCaption);

            // Choose Flythrough animation combobox
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 0 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Animation, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_flyThroughAnimationCombobox = new MyGuiControlCombobox(this, controlsOriginRight + 0 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
            for (int i = 0; i < MyTrailerLoad.Animations.Length; i++)
            {
                MyTrailerXmlAnimation animation = MyTrailerLoad.Animations[i];
                if (animation.BenchmarkAnimation)
                {
                    m_flyThroughAnimationCombobox.AddItem(i, new StringBuilder(animation.Name));
                }
            }
            m_flyThroughAnimationCombobox.SelectItemByKey(0);
            m_flyThroughAnimationCombobox.OnSelect += OnFlyThroughAnimationSelect;
            Controls.Add(m_flyThroughAnimationCombobox);

            // CheckBox if we have to display credits
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.DisplayCredits, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_displayCreditsCheckBox = new MyGuiControlCheckbox(this, controlsOriginRight + 1 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_displayCreditsCheckBox);

            // CheckBox if we want to display FPS
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 2 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.DisplayFPS, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_displayFPSCheckBox = new MyGuiControlCheckbox(this, controlsOriginRight + 2 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            m_displayFPSCheckBox.Checked = false; // False by default
            Controls.Add(m_displayFPSCheckBox);

            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f);
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenFlyThrough";
        }

        void SetSelectedAnimation()
        {
            MyTrailerLoad.TrailerAnimation = MyTrailerLoad.Animations[m_flyThroughAnimationCombobox.GetSelectedKey()];
            MyTrailerLoad.AnimationSelectedFromMenu = true;
        }

        //  This is called always when user selects item in flyThroughAnimation combobox
        public void OnFlyThroughAnimationSelect()
        {
            SetSelectedAnimation();
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            SetSelectedAnimation();

            //if (m_displayCreditsCheckBox.Checked)
            //{
            //    MyGuiManager.AddScreen(new MyGuiScreenLoading(new MyGuiScreenGameCredits(MyTrailerConstants.DEFAULT_SECTOR_IDENTIFIER,
            //        m_displayFPSCheckBox.Checked), MyGuiScreenGamePlay.Static));
            //    CloseScreenNow();
            //    MyGuiManager.CloseScreenNow(typeof(MyGuiScreenMainMenu));
            //}
            //else
            //{
            //    PlayTrailer();
            //}
            MyGuiManager.AddScreen(
                new MyGuiScreenLoading(
                    new MyGuiScreenGameCredits(
                        MyTrailerConstants.DEFAULT_SECTOR_IDENTIFIER,
                        MyTrailerConstants.DEFAULT_SECTOR_VERSION,
                        m_displayFPSCheckBox.Checked,
                        m_displayCreditsCheckBox.Checked),
                    MyGuiScreenGamePlay.Static));
            CloseScreenNow();
            MyGuiManager.CloseScreenNow(typeof(MyGuiScreenMainMenu));
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        void PlayTrailer()
        {
            MyGuiManager.AddScreen(new MyGuiScreenLoading(
                new MyGuiScreenGamePlay(MyGuiScreenGamePlayType.PURE_FLY_THROUGH, null, MyTrailerConstants.DEFAULT_SECTOR_IDENTIFIER, MyTrailerConstants.DEFAULT_SECTOR_VERSION, null),
                MyGuiScreenGamePlay.Static));
            CloseScreenNow();
            MyGuiManager.CloseScreenNow(typeof(MyGuiScreenMainMenu));
        }
    }
}
*/