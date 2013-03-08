using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.RichControls;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorLargeWeapon : MyGuiScreenEditorObject3DBase
    {
        private MyGuiControlSize m_searchingDistance;
        private MyGuiControlLabel m_label;
        private MyGuiControlCheckbox m_on;

        private float m_originalSearchingDistance;

        private List<MyPrefabLargeWeapon> m_prefabLargeWeapons;        

        public MyGuiScreenEditorLargeWeapon(List<MyPrefabLargeWeapon> prefabLargeWeapons)
            : base(null, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditPrefabLargeWeapon)
        {
            Debug.Assert(prefabLargeWeapons != null);
            Debug.Assert(prefabLargeWeapons.Count > 0);
            m_prefabLargeWeapons = prefabLargeWeapons;
            m_originalSearchingDistance = prefabLargeWeapons[0].SearchingDistance;
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorLargeWeapon";
        }        

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.92f, 0.45f);
            Vector2 size = new Vector2(0.8f, MyGuiConstants.SLIDER_HEIGHT);

            // Add screen title
            AddCaption(new Vector2(0,0.015f));

            Vector2 originDelta = new Vector2(0.45f, 0.00f);
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + originDelta;            

            m_searchingDistance = new MyGuiControlSize(
                this,
                controlsOriginLeft + 1 * MyGuiConstants.CONTROLS_DELTA,
                size,
                Vector4.Zero,
                MyTextsWrapper.Get(MyTextsWrapperEnum.SearchingDistance),
                m_prefabLargeWeapons[0].SearchingDistance,
                MyLargeShipWeaponsConstants.MIN_SEARCHING_DISTANCE,
                MyLargeShipWeaponsConstants.MAX_SEARCHING_DISTANCE,
                MyTextsWrapper.Get(MyTextsWrapperEnum.SearchingDistance),
                MyGuiSizeEnumFlags.All,
                0.3f);
            Controls.Add(m_searchingDistance);

            m_label = new MyGuiControlLabel(this, GetControlsOriginLeftFromScreenSize() + new Vector2(0.25f,0) + 3 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.On, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(m_label);
            m_on = new MyGuiControlCheckbox(this, GetControlsOriginLeftFromScreenSize() + new Vector2(0.25f, 0) + 3 * MyGuiConstants.CONTROLS_DELTA + new Vector2(0.07f, 0f), m_prefabLargeWeapons[0].Enabled, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_on);

            AddActivatedCheckbox(GetControlsOriginLeftFromScreenSize() + 3 * MyGuiConstants.CONTROLS_DELTA + new Vector2(0.5f, 0f), m_prefabLargeWeapons[0].Activated);
            //m_searchingDistance.OnValueChange += OnSearchingDistanceChanged;

            AddOkAndCancelButtonControls(new Vector2(0, -0.02f));            
        }

        private void OnSearchingDistanceChanged(MyGuiControlBase sender)
        {
            foreach (MyPrefabLargeWeapon prefabLargeWeapon in m_prefabLargeWeapons)
            {
                prefabLargeWeapon.SearchingDistance = m_searchingDistance.GetValue();
            }
        }

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);
            foreach (MyPrefabLargeWeapon prefabLargeWeapon in m_prefabLargeWeapons)
            {
                prefabLargeWeapon.SearchingDistance = m_searchingDistance.GetValue();
                if (prefabLargeWeapon.Enabled != m_on.Checked)                 
                {
                    prefabLargeWeapon.Enabled = m_on.Checked;
                }
                prefabLargeWeapon.Activate(m_activatedCheckbox.Checked, false);
            }
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
        }

        public override void OnCancelClick(MyGuiControlButton sender)
        {
            base.OnCancelClick(sender);
            //foreach (MyPrefabLargeWeapon prefabLargeWeapon in m_prefabLargeWeapons) 
            //{
            //    prefabLargeWeapon.AimingDistance = m_originalAimingDistance;
            //    prefabLargeWeapon.SearchingDistance = m_originalSearchingDistance;
            //}
        }
    }
}
