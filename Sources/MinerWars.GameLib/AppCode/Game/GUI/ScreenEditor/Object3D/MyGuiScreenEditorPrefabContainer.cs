using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorPrefabContainer : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlLabel m_label;
        MyGuiControlCheckbox m_alarmOn;

        /// <summary>
        /// asteroid has to be either MyVoxelMap or MyStaticAsteroid.
        /// </summary>
        /// <param name="asteroid">Has to be either MyVoxelMap or MyStaticAsteroid!</param>
        public MyGuiScreenEditorPrefabContainer(MyPrefabContainer prefabContainer)
            : base(prefabContainer, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditPrefabContainer)
        {
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorPrefabContainer";
        }

        private MyPrefabContainer PrefabContainer 
        {
            get 
            {
                return (MyPrefabContainer)m_entity;
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
            Vector2 controlsOriginRight = GetControlsOriginRightFromScreenSize() + originDelta;

            AddActivatedCheckbox(controlsOriginLeft, PrefabContainer.Activated);

            m_label = new MyGuiControlLabel(this, controlsOriginLeft + 1 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Alarm, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_label);
            m_alarmOn = new MyGuiControlCheckbox(this, controlsOriginLeft + 1 * CONTROLS_DELTA + new Vector2(0.1f, 0f), PrefabContainer.AlarmOn, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_alarmOn);

            AddOkAndCancelButtonControls();            
        }                

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);

            PrefabContainer.AlarmOn = m_alarmOn.Checked;
            PrefabContainer.Activate(m_activatedCheckbox.Checked, false);

            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

        }
    }
}
