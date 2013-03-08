using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.CargoBox;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.GUI.Helpers;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorCargoBox : MyGuiScreenEditorObject3DBase
    {        
        MyGuiControlCombobox m_cargoBoxTypeCombobox;        

        private MyCargoBox CargoBox { get { return (MyCargoBox)m_entity; } }

        public MyGuiScreenEditorCargoBox(MyCargoBox cargoBox)
            : base(cargoBox, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditCargoBox)
        {
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorCargoBox";
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.5f, 0.6f);

            // Add screen title
            AddCaption();                        

            InitControls();
            AddOkAndCancelButtonControls();
        }

        public void InitControls()
        {
            AddActivatedCheckbox(new Vector2(-0.07f,-0.1f), CargoBox.Activated);

            AddIdTextBox(new Vector2(-0.16f, -0.15f), CargoBox.EntityId.Value.NumericValue);

            m_cargoBoxTypeCombobox = new MyGuiControlCombobox(
                this,
                new Vector2(0, -0.2f),
                MyGuiControlPreDefinedSize.LONGMEDIUM,
                MyGuiConstants.COMBOBOX_BACKGROUND_COLOR,
                MyGuiConstants.COMBOBOX_TEXT_SCALE,
                6,
                false,
                false,
                false);

            foreach (MyMwcObjectBuilder_CargoBox_TypesEnum cargoBoxType in Enum.GetValues(typeof(MyMwcObjectBuilder_CargoBox_TypesEnum)))
            {
                var cargoBoxHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(
                    MyMwcObjectBuilderTypeEnum.CargoBox, (int) cargoBoxType);
                m_cargoBoxTypeCombobox.AddItem((int)cargoBoxType, MyGuiManager.GetBlankTexture(), cargoBoxHelper.Description);
            }
            m_cargoBoxTypeCombobox.SortItemsByValueText();

            m_cargoBoxTypeCombobox.SelectItemByKey((int) CargoBox.CargoBoxType);

            Controls.Add(m_cargoBoxTypeCombobox);            
        }        

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);         

            CargoBox.CargoBoxType = (MyMwcObjectBuilder_CargoBox_TypesEnum) m_cargoBoxTypeCombobox.GetSelectedKey();
            CargoBox.Activate(m_activatedCheckbox.Checked, false);

            this.CloseScreen();
        }        
    }
}
