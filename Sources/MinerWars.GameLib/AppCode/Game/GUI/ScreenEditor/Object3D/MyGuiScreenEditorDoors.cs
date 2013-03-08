using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.SubObjects;
using MinerWars.AppCode.Game.GUI.Core;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorDoors : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlLabel m_label;
        MyGuiControlCheckbox m_on;
        Vector2? m_screenPosition;                

        /// <summary>
        /// Edit kinematic prefab
        /// </summary>
        /// <param name="prefabKinematic">Kinematic prefab</param>
        public MyGuiScreenEditorDoors(MyPrefabKinematic prefabKinematic)
            : base(prefabKinematic, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditPrefabKinematic)
        {
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorDoors";
        }

        private MyPrefabKinematic PrefabKinematic
        {
            get 
            {
                return (MyPrefabKinematic)m_entity;
            }
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.92f, 0.95f);

            // Add screen title
            AddCaption();

            Vector2 originDelta = new Vector2(0.02f, 0);
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + originDelta;
            Vector2 controlsOriginRight = GetControlsOriginRightFromScreenSize() + originDelta;

            m_label = new MyGuiControlLabel(this, controlsOriginLeft, null, MyTextsWrapperEnum.On, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_label);
            m_on = new MyGuiControlCheckbox(this, controlsOriginLeft + new Vector2(0.1f, 0f), PrefabKinematic.CanOpen, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_on);

            AddOkAndCancelButtonControls();            
        }                

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);

            if (PrefabKinematic.CanOpen != m_on.Checked)
            {
                PrefabKinematic.CanOpen = m_on.Checked;
            }

            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

        }
    }
}
