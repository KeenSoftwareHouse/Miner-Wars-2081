#region Using

using System;
using System.Globalization;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Audio.Dialogues;

#endregion

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenDebugDialogues : MyGuiScreenBase
    {
        private MyGuiControlCombobox m_selectDialogueCombobox;
        float m_controlsAdded;

        public static readonly Vector2 CONTROLS_DELTA = new Vector2(0, 0.0525f);

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugDialogues";
        }

        public MyGuiScreenDebugDialogues(Vector2? screenPosition)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.7f, 0.9f))
        {
            Init();
        }

        protected void AddOkAndCancelButtonControls(Vector2 offset)
        {
            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.1f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.01f) + offset;
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.PlayDemo, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnPlayClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
        }


        public virtual void OnPlayClick(MyGuiControlButton sender)
        {
            int key = m_selectDialogueCombobox.GetSelectedKey();
            MyDialogueEnum dialogue = (MyDialogueEnum)key;
            MyDialogues.Play(dialogue);
        }

        public virtual void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        protected virtual Vector2 GetControlsOriginLeftFromScreenSize()
        {
            return new Vector2(-m_size.Value.X / 2.0f + 0.03f, -m_size.Value.Y / 2.0f + 0.09f);
        }

        #region Init

        private void Init()
        {
            m_size = new Vector2(0.7f, 1);

            m_enableBackgroundFade = true;

            AddCaption(new StringBuilder("Debug dialogues"), Vector4.One, new Vector2(0, 0.01f));

            AddOkAndCancelButtonControls(new Vector2(0.03f, -0.02f));

            var controlsOrigin = GetControlsOriginLeftFromScreenSize() - new Vector2(-0.018f, 0.025f);
            m_controlsAdded = 1;

            CreateControlsSound(controlsOrigin);

            if (MyGuiScreenGamePlay.Static == null)
            {
                MinerWars.AppCode.Game.HUD.MyHud.LoadData();
                MinerWars.AppCode.Game.HUD.MyHud.LoadContent(MyGuiManager.GetMainMenuScreen());
            }

            MyGuiScreenDebugDeveloper devscreen = MyGuiManager.GetScreenDebugDeveloper();
            if (devscreen != null)
                devscreen.CloseScreen();
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            MyDialogues.Stop();
        }

        private void CreateControlsSound(Vector2 controlsOrigin)
        {
            AddSeparator(controlsOrigin);

            Controls.Add(new MyGuiControlLabel(this, controlsOrigin + m_controlsAdded * CONTROLS_DELTA + new Vector2(0.05f, 0), null, MyTextsWrapperEnum.SoundInfluenceSphereType, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_controlsAdded++;
            
            m_selectDialogueCombobox = new MyGuiControlCombobox(this,
                                                             controlsOrigin + m_controlsAdded++ * CONTROLS_DELTA +
                                                             new Vector2(MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2.0f, 0),
                                                             MyGuiControlPreDefinedSize.LONGMEDIUM,
                                                             MyGuiConstants.COMBOBOX_BACKGROUND_COLOR,
                                                             MyGuiConstants.COMBOBOX_TEXT_SCALE, 6, false, false, false);

            System.Collections.Generic.SortedDictionary<string, int> dialogues = new System.Collections.Generic.SortedDictionary<string, int>();

            foreach (MyDialogueEnum dialogue in Enum.GetValues(typeof(MyDialogueEnum)))
            {
                dialogues.Add(dialogue.ToString(), (int)dialogue);
            }

            foreach (var dialogue in dialogues)
            {
                m_selectDialogueCombobox.AddItem(dialogue.Value, null, new StringBuilder(dialogue.Key));
            }

            m_selectDialogueCombobox.SelectItemByIndex(0);

            Controls.Add(m_selectDialogueCombobox);
        }

        private void AddSeparator(Vector2 controlsOrigin)
        {
            var pos = controlsOrigin + m_controlsAdded * CONTROLS_DELTA + new Vector2(0.3f, -0.01f);

            Controls.Add(new MyGuiControlPanel(this, pos,
                                               new Vector2(0.6f, 0.005f), Vector4.Zero, 2,
                                               MyGuiConstants.DEFAULT_CONTROL_NONACTIVE_COLOR));

            m_controlsAdded += 0.5f;
        }

        #endregion

        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false)
                return false;

            MinerWars.AppCode.Game.HUD.MyHud.Draw(HUD.MyHudDrawPassEnum.SECOND, MyCameraAttachedToEnum.Camera, false);

            return true;
        }
    }
}