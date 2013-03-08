/*
using System;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Editor;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorLargeShip : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlCombobox m_selectLargeShipCombobox;

        public MyGuiScreenEditorLargeShip()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.CreateLargeShip)
        {
            Init();
        }

        public MyGuiScreenEditorLargeShip(MyLargeShip largeShip)
            : base(largeShip, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditLargeShip)
        {
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorLargeShip";
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.73f, 0.95f);
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize();

            // Add screen title
            AddCaption();

            // Decide if screen is for editing existing phys object or adding new
            if (IsPhysObject())
            {
                AddEditPositionControls();   
            }
            else
            {
                #region Large Ship Selection
                //choose large ship
                Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 0 * CONTROLS_DELTA, null, MyTextsWrapperEnum.ChooseModel, MyGuiConstants.LABEL_TEXT_COLOR,
                    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

                //COMBOBOX - large ship
                m_selectLargeShipCombobox = new MyGuiControlCombobox(this, controlsOriginLeft + 2 * CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_LARGE_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.LARGE,
                    MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, 4, true, false, true);

                foreach (MyMwcObjectBuilder_LargeShip_TypesEnum enumValue in MyGuiLargeShipHelpers.MyMwcLargeShipTypesEnumValues)
                {
                    if (enumValue != MyMwcObjectBuilder_LargeShip_TypesEnum.JEROMIE_INTERIOR_STATION && enumValue != MyMwcObjectBuilder_LargeShip_TypesEnum.CRUISER_SHIP)
                    {
                        MyGuiLargeShipHelper largeShipHelper = MyGuiLargeShipHelpers.GetMyGuiLargeShipHelper(enumValue);
                        m_selectLargeShipCombobox.AddItem((int)enumValue, largeShipHelper.Icon, largeShipHelper.Description);
                    }
                }

                m_selectLargeShipCombobox.SelectItemByKey(1);
                m_selectLargeShipCombobox.OnSelectItemDoubleClick = OnOkClick;
                Controls.Add(m_selectLargeShipCombobox);
                #endregion
            }

            AddOkAndCancelButtonControls();
        }

        public override void OnOkClick()
        {
            base.OnOkClick();

            MyMwcObjectBuilder_LargeShip_TypesEnum shipType = (MyMwcObjectBuilder_LargeShip_TypesEnum)
                Enum.ToObject(typeof(MyMwcObjectBuilder_LargeShip_TypesEnum), m_selectLargeShipCombobox.GetSelectedKey());
            MyMwcObjectBuilder_LargeShip largeShipBuilder = new MyMwcObjectBuilder_LargeShip(shipType);
            MyEditor.Static.CreateFromObjectBuilder(largeShipBuilder, Matrix.CreateWorld(m_newObjectPosition, Vector3.Forward, Vector3.Up));

            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
        }

        protected override Vector2 GetControlsOriginLeftFromScreenSize()
        {
            return base.GetControlsOriginLeftFromScreenSize() + new Vector2(0.002f, 0);
        }
    }
}
    */