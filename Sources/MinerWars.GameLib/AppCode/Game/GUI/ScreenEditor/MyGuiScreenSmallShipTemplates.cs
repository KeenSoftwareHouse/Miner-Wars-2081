using System.Diagnostics;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Others;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.Collections.Generic;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    delegate void SelectTemplateCallback(ScreenResult result, MySmallShipTemplate selectedTemplate);

    class MyGuiScreenSmallShipTemplates : MyGuiScreenEditorDialogBase
    {
        private MyGuiControlCombobox m_shipTypeCombobox;
        private MyGuiControlListbox m_templatesListbox;
        private string m_newName;
        private MySmallShipTemplate m_selectedTemplate;
        private readonly SelectTemplateCallback m_selectCallback;

        /// <summary>
        /// Screen for editing or selecting small ship templates.
        /// </summary>
        /// <param name="selectCallback">If not null, creates a template selection screen.
        /// If null, creates a template editing screen.</param>
        public MyGuiScreenSmallShipTemplates(SelectTemplateCallback selectCallback = null)
            : base(new Vector2(0.5f), new Vector2(0.7f, 0.85f))
        {
            m_enableBackgroundFade = true;

            AddCaption(MyTextsWrapperEnum.SmallShipTemplates, new Vector2(0,0.005f));

            InitCombobox();

            InitListBox();

            if (selectCallback != null)
            {
                m_selectCallback = selectCallback;
                AddOkAndCancelButtonControls(new Vector2(0,-0.015f));
            }
            else
            {
                InitButtons();
                AddBackButtonControl(new Vector2(0, -0.015f));
            }

            UpdateControls();
        }

        public override string GetFriendlyName()
        {
            return "SmallShipTemplates";
        }

        #region Init

        private void InitCombobox()
        {
            m_shipTypeCombobox = new MyGuiControlCombobox(
                this,
                new Vector2(0, -0.3f),
                MyGuiControlPreDefinedSize.MEDIUM,
                MyGuiConstants.COMBOBOX_BACKGROUND_COLOR,
                MyGuiConstants.COMBOBOX_TEXT_SCALE);

            foreach (MyMwcObjectBuilder_SmallShip_TypesEnum shipType in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues)
            {
                int shipTypeNumber = (int) shipType;
                var helper = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, shipTypeNumber);
                m_shipTypeCombobox.AddItem(shipTypeNumber, helper.Name);
            }

            m_shipTypeCombobox.SelectItemByIndex(0);

            m_shipTypeCombobox.OnSelect += Combobox_OnSelect;

            Controls.Add(m_shipTypeCombobox);
        }

        private void InitListBox()
        {
            m_templatesListbox = new MyGuiControlListbox(
                this, 
                new Vector2(0, 0.0f),
                new Vector2(0.25f, 0.075f),
                MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
                MyTextsWrapper.Get(MyTextsWrapperEnum.SmallShipTemplates),
                MyGuiConstants.LABEL_TEXT_SCALE,
                1, 6, 1, false, true, false,
                null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 2, 1, MyGuiConstants.LISTBOX_BACKGROUND_COLOR_BLUE, 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);

            Controls.Add(m_templatesListbox);
        }

        private void InitButtons()
        {
            var newButton = new MyGuiControlButton(
                this, new Vector2(0.23f, -0.2f),
                new Vector2(0.1f, 0.0475f),
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Add,
                MyGuiConstants.BUTTON_TEXT_COLOR,
                MyGuiConstants.BUTTON_TEXT_SCALE,
                OnAddButtonClick,
                MyGuiControlButtonTextAlignment.CENTERED,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true);

            Controls.Add(newButton);

            var renameButton = new MyGuiControlButton(
                this, new Vector2(0.23f, -0.15f),
                new Vector2(0.1f, 0.0475f),
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Rename,
                MyGuiConstants.BUTTON_TEXT_COLOR,
                MyGuiConstants.BUTTON_TEXT_SCALE,
                OnRenameButtonClick,
                MyGuiControlButtonTextAlignment.CENTERED,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true);

            Controls.Add(renameButton);

            var editButton = new MyGuiControlButton(
                this, new Vector2(0.23f, -0.1f),
                new Vector2(0.1f, 0.0475f),
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Edit,
                MyGuiConstants.BUTTON_TEXT_COLOR,
                MyGuiConstants.BUTTON_TEXT_SCALE,
                OnEditButtonClick,
                MyGuiControlButtonTextAlignment.CENTERED,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true);

            Controls.Add(editButton);

            var deleteButton = new MyGuiControlButton(
                this, new Vector2(0.23f, -0.05f),
                new Vector2(0.1f, 0.0475f),
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Delete,
                MyGuiConstants.BUTTON_TEXT_COLOR,
                MyGuiConstants.BUTTON_TEXT_SCALE,
                OnDeleteButtonClick,
                MyGuiControlButtonTextAlignment.CENTERED,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true);

            Controls.Add(deleteButton);

            var saveToServerButton = new MyGuiControlButton(
                this, new Vector2(0.23f, 0.2f),
                new Vector2(0.15f, 0.0475f),
                Color.Green.ToVector4(),
                MyTextsWrapperEnum.SaveToServer,
                MyGuiConstants.BUTTON_TEXT_COLOR,
                MyGuiConstants.BUTTON_TEXT_SCALE,
                OnSaveToServerButton,
                MyGuiControlButtonTextAlignment.CENTERED,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true);

            Controls.Add(saveToServerButton);

            var reloadFromServerButton = new MyGuiControlButton(
                this, new Vector2(0.23f, 0.27f),
                new Vector2(0.15f, 0.0475f),
                Color.Green.ToVector4(),
                MyTextsWrapperEnum.LoadFromServer,
                MyGuiConstants.BUTTON_TEXT_COLOR,
                MyGuiConstants.BUTTON_TEXT_SCALE,
                OnReloadFromServerButton,
                MyGuiControlButtonTextAlignment.CENTERED,
                true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true);

            Controls.Add(reloadFromServerButton);
        }

        #endregion

        #region Updating controls' layout

        private void Combobox_OnSelect()
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            var selectedType = (MyMwcObjectBuilder_SmallShip_TypesEnum) m_shipTypeCombobox.GetSelectedKey();
            FillListBox(selectedType);
        }

        private void FillListBox(MyMwcObjectBuilder_SmallShip_TypesEnum selectedType)
        {
            m_templatesListbox.RemoveAllItems();
            var templates = MySmallShipTemplates.GetTemplatesForType(selectedType);
            for (int i = 0; i < templates.Count; i++)
            {
                var template = templates[i];
                m_templatesListbox.AddItem(i, template.Name);
            }
        }

        #endregion

        private static void WarnNotSelected()
        {
            //MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR,
            //                                                 MyTextsWrapperEnum.YouHaveToSelect,
            //                                                 MyTextsWrapperEnum.Failure,
            //                                                 MyTextsWrapperEnum.Ok, null));
        }

        protected override void OnOkClick(MyGuiControlButton sender)
        {
            var selectedItem = m_templatesListbox.GetSelectedItem();
            if (selectedItem != null)
            {
                var selectedType = (MyMwcObjectBuilder_SmallShip_TypesEnum) m_shipTypeCombobox.GetSelectedKey();
                var selectedTemplate = MySmallShipTemplates.GetTemplate(selectedType, selectedItem.Value);
                
                base.OnOkClick(sender);
                
                Debug.Assert(m_selectCallback != null);
                m_selectCallback(ScreenResult.Ok, selectedTemplate);
            }
            else
            {
                WarnNotSelected();
            }
        }

        protected override void OnCancelClick(MyGuiControlButton sender)
        {
            base.OnCancelClick(sender);

            Debug.Assert(m_selectCallback != null);
            m_selectCallback(ScreenResult.Cancel, null);
        }

        #region Add

        private void OnAddButtonClick(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenInputString(NewNameChosen, MyTextsWrapperEnum.NewTemplateName));
        }

        private void NewNameChosen(ScreenResult result, string resultText)
        {
            if (result == ScreenResult.Ok)
            {
                m_newName = resultText;

                var selectedType = (MyMwcObjectBuilder_SmallShip_TypesEnum) m_shipTypeCombobox.GetSelectedKey();

                var builders = new List<MySmallShipBuilderWithName>
                {
                    new MySmallShipBuilderWithName(MyMwcObjectBuilder_SmallShip_Bot.CreateObjectBuilderWithAllItems(selectedType, MyShipTypeConstants.GetShipTypeProperties(selectedType).GamePlay.CargoCapacity))
                };

                var inventory = new MyInventory();                
                inventory.FillInventoryWithAllItems(null, 100);
                var gui = new MyGuiScreenInventory(
                    builders,
                    0,
                    inventory.GetObjectBuilder(false),
                    MyTextsWrapper.Get(MyTextsWrapperEnum.AllItemsInventory));

                gui.OnSave += AddTemplate;
                MyGuiManager.AddScreen(gui);
            }
        }

        private void AddTemplate(MyGuiScreenInventory sender, MyGuiScreenInventorySaveResult saveresult)
        {
            var newID = MySmallShipTemplates.GenerateNewID();
            MySmallShipTemplates.AddTemplate(new MySmallShipTemplate(
                newID, 
                new StringBuilder(m_newName), 
                saveresult.SmallShipsObjectBuilders[saveresult.CurrentIndex].Builder, 
                false));

            UpdateControls();
            sender.OnSave -= AddTemplate;
        }

        #endregion

        #region Edit

        private void OnEditButtonClick(MyGuiControlButton sender)
        {
            var selectedItem = m_templatesListbox.GetSelectedItem();
            if (selectedItem != null)
            {
                var selectedType = (MyMwcObjectBuilder_SmallShip_TypesEnum) m_shipTypeCombobox.GetSelectedKey();
                m_selectedTemplate = MySmallShipTemplates.GetTemplate(selectedType, selectedItem.Value);

                var builders = new List<MySmallShipBuilderWithName> { new MySmallShipBuilderWithName(m_selectedTemplate.Builder) };

                var inventory = new MyInventory();
                inventory.FillInventoryWithAllItems(null, 100);
                var gui = new MyGuiScreenInventory(
                    builders,
                    0,
                    inventory.GetObjectBuilder(false),
                    MyTextsWrapper.Get(MyTextsWrapperEnum.AllItemsInventory));

                gui.OnSave += UpdateTemplate;
                MyGuiManager.AddScreen(gui);
            }
            else
            {
                WarnNotSelected();
            }
        }

        private void UpdateTemplate(MyGuiScreenInventory sender, MyGuiScreenInventorySaveResult saveresult)
        {
            Debug.Assert(m_selectedTemplate != null);

            m_selectedTemplate.Builder = saveresult.SmallShipsObjectBuilders[saveresult.CurrentIndex].Builder;
            sender.OnSave -= UpdateTemplate;
        }

        #endregion

        #region Rename

        private void OnRenameButtonClick(MyGuiControlButton sender)
        {
            var selectedItem = m_templatesListbox.GetSelectedItem();
            if (selectedItem != null)
            {
                var selectedType = (MyMwcObjectBuilder_SmallShip_TypesEnum)m_shipTypeCombobox.GetSelectedKey();
                m_selectedTemplate = MySmallShipTemplates.GetTemplate(selectedType, selectedItem.Value);

                MyGuiManager.AddScreen(new MyGuiScreenInputString(NameForRenameChosen, MyTextsWrapperEnum.NewTemplateName, m_selectedTemplate.Name));
            }
        }

        private void NameForRenameChosen(ScreenResult result, string resultText)
        {
            if (result == ScreenResult.Ok)
            {
                if (!string.IsNullOrEmpty(resultText))
                {
                    m_selectedTemplate.Name = new StringBuilder(resultText);
                    UpdateControls();
                }
                else
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR,
                                                                     MyTextsWrapperEnum.EmptyDescription,
                                                                     MyTextsWrapperEnum.Failure,
                                                                     MyTextsWrapperEnum.Ok, null));
                }
            }
        }

        #endregion

        #region Delete

        private void OnDeleteButtonClick(MyGuiControlButton sender)
        {
            var selectedItem = m_templatesListbox.GetSelectedItem();
            if (selectedItem != null)
            {
                Debug.Assert(selectedItem != null);

                var selectedType = (MyMwcObjectBuilder_SmallShip_TypesEnum) m_shipTypeCombobox.GetSelectedKey();
                var selectedTemplate = MySmallShipTemplates.GetTemplate(selectedType, selectedItem.Value);

                if (selectedTemplate.SavedToServer)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR,
                                                                     MyTextsWrapperEnum.CannotDeleteSavedTemplate,
                                                                     MyTextsWrapperEnum.Failure, 
                                                                     MyTextsWrapperEnum.Ok, null));
                }
                else
                {
                    MySmallShipTemplates.DeleteTemplate(selectedTemplate);
                    UpdateControls();
                }
            }
            else
            {
                WarnNotSelected();
            }
        }

        #endregion

        #region Save to server

        private void OnSaveToServerButton(MyGuiControlButton sender)
        {
            MySmallShipTemplates.SaveToServer(true);
        }

        private void OnReloadFromServerButton(MyGuiControlButton sender) 
        {
            MySmallShipTemplates.Load(true);
        }

        #endregion
    }
}