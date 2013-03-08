using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorPrefab : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlLabel m_onLabel;
        MyGuiControlCheckbox m_on;

        MyGuiControlLabel m_alarmLabel;
        MyGuiControlCombobox m_alarmComboBox;

        /// <summary>
        /// Edit prefab
        /// </summary>
        /// <param name="prefab">Prefab</param>
        public MyGuiScreenEditorPrefab(MyPrefabBase prefab)
            : base(prefab, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditPrefab)
        {
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorPrefab";
        }

        private MyPrefabBase Prefab
        {
            get 
            {
                return (MyPrefabBase)m_entity;
            }
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.8f, 0.4f);

            // Add screen title
            AddCaption();

            Vector2 originDelta = new Vector2(0.05f, 0.02f);
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + originDelta;

            AddActivatedCheckbox(controlsOriginLeft, Prefab.Activated);

            m_onLabel = new MyGuiControlLabel(this, controlsOriginLeft + 1 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.On, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_onLabel);
            m_on = new MyGuiControlCheckbox(this, controlsOriginLeft + 1 * MyGuiConstants.CONTROLS_DELTA + new Vector2(0.1f, 0f), Prefab.Enabled, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_on);

            m_alarmLabel = new MyGuiControlLabel(this, controlsOriginLeft + 2 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Alarm, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_alarmLabel);

            var alarmComboboxPredefinedSize = MyGuiControlPreDefinedSize.MEDIUM;
            var alarmComboboxSize = MyGuiControlCombobox.GetPredefinedControlSize(alarmComboboxPredefinedSize);

            m_alarmComboBox = new MyGuiControlCombobox(
                this,
                controlsOriginLeft + 2 * MyGuiConstants.CONTROLS_DELTA +
                new Vector2(0.075f + alarmComboboxSize.X / 2, 0f),
                alarmComboboxPredefinedSize,
                MyGuiConstants.COMBOBOX_BACKGROUND_COLOR,
                MyGuiConstants.COMBOBOX_TEXT_SCALE);

            m_alarmComboBox.AddItem(0, MyTextsWrapperEnum.Default);
            m_alarmComboBox.AddItem(1, MyTextsWrapperEnum.On);
            m_alarmComboBox.AddItem(2, MyTextsWrapperEnum.Off);
            if (Prefab.DefaultCausesAlarm())
            {
                m_alarmComboBox.SelectItemByKey(0);
            }
            else
            {
                m_alarmComboBox.SelectItemByKey(Prefab.CausesAlarm ? 1 : 2);
            }
            Controls.Add(m_alarmComboBox);

            AddOkAndCancelButtonControls();       
        }                

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);

            Prefab.Enabled = m_on.Checked;            

            int alarmKey = m_alarmComboBox.GetSelectedKey();
            bool? causesAlarm = null;
            if (alarmKey == 1)
            {
                causesAlarm = true;
            }
            else if (alarmKey == 2)
            {
                causesAlarm = false;
            }
            Prefab.SetCausesAlarm(causesAlarm);

            Prefab.Activate(m_activatedCheckbox.Checked, false);

            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
        }
    }
}
