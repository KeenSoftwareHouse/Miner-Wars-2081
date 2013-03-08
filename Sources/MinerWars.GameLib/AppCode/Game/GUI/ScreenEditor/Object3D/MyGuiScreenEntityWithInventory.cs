using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Inventory;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI.Helpers;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Entities.Prefabs;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEntityWithInventory : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlListbox m_allItemsInventoryListbox;
        MyGuiControlListbox m_entityInventoryListbox;
        MyGuiControlListboxDragAndDrop m_dragAndDrop;        

        MyInventoryItemsRepository m_inventoryItemsRepository;
        MyInventory m_allItemsInventory;

        MyGuiControlCombobox m_inventoryTemplatesCombobox;
        MyGuiControlTextbox m_priceCoeficientTextbox;
        MyGuiControlTextbox m_refillTimeTextbox;

        private const int ROWS = 5;
        private const int COLUMNS = 5;

        private IMyInventory EntityWithInventory { get; set; }
        private MyPrefabContainer PrefabContainer { get; set; }
        private bool m_isPrefabContainer;
        private List<MyInventoryItem> m_itemsAdded;
        private List<MyInventoryItem> m_itemsRemoved;

        public MyGuiScreenEntityWithInventory(MyEntity entityWithInventory)
            : base(entityWithInventory, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditInventory)
        {
            Debug.Assert(entityWithInventory is IMyInventory);
            EntityWithInventory = entityWithInventory as IMyInventory;
            if (entityWithInventory is MyPrefabHangar)
            {
                PrefabContainer = ((MyPrefabHangar)entityWithInventory).GetOwner();
            }
            else 
            {
                PrefabContainer = entityWithInventory as MyPrefabContainer;
            }
            m_itemsAdded = new List<MyInventoryItem>();
            m_itemsRemoved = new List<MyInventoryItem>();
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEntityWithInventory";
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.99f, 0.95f);

            // Add screen title
            AddCaption();
            
            m_inventoryItemsRepository = new MyInventoryItemsRepository();
            m_allItemsInventory = new MyInventory();
            m_allItemsInventory.FillInventoryWithAllItems(null, 100, true);

            InitControls();
            AddOkAndCancelButtonControls();
        }

        public void InitControls()
        {           
            StringBuilder otherSideInventoryName = new StringBuilder();
            otherSideInventoryName.Append(string.IsNullOrEmpty(m_entity.DisplayName) ? m_entity.GetFriendlyName() : m_entity.DisplayName);
            otherSideInventoryName.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.OtherSideInventory));

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 4f + 0.03f, 0.1f);
            Vector2 controlsOriginRight = new Vector2(+m_size.Value.X / 4f - 0.03f, 0.1f);
            List<MyGuiControlListbox> listboxesToDragAndDrop = new List<MyGuiControlListbox>();

            m_allItemsInventoryListbox = new MyGuiControlListbox(this, controlsOriginLeft, MyGuiConstants.LISTBOX_SMALL_SIZE, MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
                null, MyGuiConstants.LABEL_TEXT_SCALE, COLUMNS, ROWS, COLUMNS, true, true, false, null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 4, 2, MyGuiConstants.LISTBOX_ITEM_COLOR, 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);
            AddInventoryListbox(m_allItemsInventoryListbox, m_allItemsInventory, ref listboxesToDragAndDrop);

            m_entityInventoryListbox = new MyGuiControlListbox(this, controlsOriginRight, MyGuiConstants.LISTBOX_SMALL_SIZE, MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
                null, MyGuiConstants.LABEL_TEXT_SCALE, COLUMNS, ROWS, COLUMNS, true, true, false, null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 4, 2, MyGuiConstants.LISTBOX_ITEM_COLOR, 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);
            AddInventoryListbox(m_entityInventoryListbox, EntityWithInventory.Inventory, ref listboxesToDragAndDrop);

            m_dragAndDrop = new MyGuiControlListboxDragAndDrop(this, listboxesToDragAndDrop, MyGuiControlPreDefinedSize.SMALL, MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, Vector2.Zero, true);
            m_dragAndDrop.ListboxItemDropped += DragAndDropListboxItemDropped;
            Controls.Add(m_dragAndDrop);

            Vector2 labelOffset = new Vector2(0.05f, -0.05f);            

            Controls.Add(new MyGuiControlLabel(this, m_allItemsInventoryListbox.GetPosition() - m_allItemsInventoryListbox.GetSize().Value / 2f + labelOffset,
                null, MyTextsWrapperEnum.AllItemsInventory, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, m_entityInventoryListbox.GetPosition() - m_entityInventoryListbox.GetSize().Value / 2f + labelOffset,
                null, otherSideInventoryName, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));


            Vector2 otherSettingsPosition = new Vector2(-m_size.Value.X / 2f + 0.1f, -0.35f);
                                        
            Controls.Add(new MyGuiControlLabel(this, otherSettingsPosition,
                null, MyTextsWrapperEnum.PriceCoeficient, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_priceCoeficientTextbox = new MyGuiControlTextbox(this, otherSettingsPosition + new Vector2(0.5f, 0f), MyGuiControlPreDefinedSize.MEDIUM, string.Empty, 3, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);
            m_priceCoeficientTextbox.Text = MyValueFormatter.GetFormatedFloat(EntityWithInventory.Inventory.PriceCoeficient, 2, string.Empty);
            Controls.Add(m_priceCoeficientTextbox);

            if (PrefabContainer != null) 
            {
                otherSettingsPosition += MyGuiConstants.CONTROLS_DELTA;

                Controls.Add(new MyGuiControlLabel(this, otherSettingsPosition,
                    null, MyTextsWrapperEnum.InventoryTemplates, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_inventoryTemplatesCombobox = new MyGuiControlCombobox(this, otherSettingsPosition + new Vector2(0.5f, 0f), MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
                m_inventoryTemplatesCombobox.OnSelect += new MyGuiControlCombobox.OnComboBoxSelectCallback(m_inventoryTemplatesCombobox_OnSelect);
                m_inventoryTemplatesCombobox.AddItem(0, MyTextsWrapperEnum.None);
                foreach (MyMwcInventoryTemplateTypeEnum inventoryTemplateType in MyGuiInventoryTemplateTypeHelpers.MyInventoryTemplateTypeValues)
                {
                    if (MyInventoryTemplates.ContainsAnyItems(inventoryTemplateType))
                    {
                        m_inventoryTemplatesCombobox.AddItem((int)inventoryTemplateType, MyGuiInventoryTemplateTypeHelpers.GetInventoryTemplateTypeHelper(inventoryTemplateType).Description);
                    }
                }
                Controls.Add(m_inventoryTemplatesCombobox);

                otherSettingsPosition += MyGuiConstants.CONTROLS_DELTA;

                Controls.Add(new MyGuiControlLabel(this, otherSettingsPosition,
                null, MyTextsWrapperEnum.RefillTime, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                m_refillTimeTextbox = new MyGuiControlTextbox(this, otherSettingsPosition + new Vector2(0.5f, 0f), MyGuiControlPreDefinedSize.MEDIUM, string.Empty, 9, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.DIGITS_ONLY);                
                Controls.Add(m_refillTimeTextbox);

                RefillTime = PrefabContainer.RefillTime;
                SelectedTemplateType = EntityWithInventory.Inventory.TemplateType;
            }            
        }

        void m_inventoryTemplatesCombobox_OnSelect()
        {
            if(PrefabContainer != null)
            {
                if (SelectedTemplateType == null)
                {
                    RefillTime = null;                    
                }
                else 
                {
                    if (RefillTime == null) 
                    {
                        RefillTime = MyPrefabContainer.DEFAULT_REFILL_TIME_IN_SEC;
                    }
                }
                m_refillTimeTextbox.Enabled = RefillTime != null;
            }
        }

        private int? RefillTime
        {
            get
            {
                int? refillTime = PrefabContainer != null && !string.IsNullOrEmpty(m_refillTimeTextbox.Text) ? int.Parse(m_refillTimeTextbox.Text) : (int?)null;
                return refillTime;
            }
            set 
            {
                Debug.Assert(PrefabContainer != null);
                if (value == null)
                {
                    m_refillTimeTextbox.Text = string.Empty;
                }
                else 
                {
                    m_refillTimeTextbox.Text = value.Value.ToString();
                }
            }
        }

        private MyMwcInventoryTemplateTypeEnum? SelectedTemplateType 
        {
            get
            {
                MyMwcInventoryTemplateTypeEnum? templateType = null;
                if (m_inventoryTemplatesCombobox != null) 
                {
                    int selectedTemplateTypeKey = m_inventoryTemplatesCombobox.GetSelectedKey();
                    templateType = selectedTemplateTypeKey == 0 ? (MyMwcInventoryTemplateTypeEnum?)null : (MyMwcInventoryTemplateTypeEnum)selectedTemplateTypeKey;
                }
                                
                return templateType;
            }
            set 
            {
                if (value == null)
                {
                    m_inventoryTemplatesCombobox.SelectItemByKey(0);
                }
                else 
                {
                    m_inventoryTemplatesCombobox.SelectItemByKey((int)value.Value);
                }
            }
        }

        private void AddInventoryListbox(MyGuiControlListbox listbox, MyInventory inventory, ref List<MyGuiControlListbox> listboxesToDragAndDrop) 
        {
            Controls.Add(listbox);
            listboxesToDragAndDrop.Add(listbox);
            listbox.ItemDrag += ListboxItemDrag;
            listbox.ItemDoubleClick += ListboxItemDoubleClick;

            foreach (MyInventoryItem inventoryItem in inventory.GetInventoryItems()) 
            {
                listbox.AddItem(CreateListboxItemAndAddToRepository(inventoryItem));
            }
        }

        private MyGuiControlListboxItem CreateListboxItemAndAddToRepository(MyInventoryItem inventoryItem) 
        {
            int inventoryItemKey = m_inventoryItemsRepository.AddItem(inventoryItem);            

            StringBuilder description = null;
            if (inventoryItem.Icon == null)
            {
                description = inventoryItem.MultiLineDescription;
            }

            MyToolTips toolTips = new MyToolTips();
            toolTips.AddToolTip(inventoryItem.MultiLineDescription, Color.White, 0.7f);            

            MyGuiControlListboxItem listboxItem = new MyGuiControlListboxItem(inventoryItemKey, description, inventoryItem.Icon, toolTips, MyGuiConstants.LABEL_TEXT_SCALE);
            listboxItem.IconTexts = new MyIconTexts();

            // add amount icon's text
            if (inventoryItem.Amount != 1f || inventoryItem.Amount != inventoryItem.MaxAmount)
            {
                StringBuilder amount = new StringBuilder();
                amount.Append(inventoryItem.Amount.ToString());
                MyGuiDrawAlignEnum align;
                Vector2 offset;
                if (inventoryItem.AmountTextAlign == MyInventoryAmountTextAlign.MiddleRight)
                {
                    align = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER;
                    offset = new Vector2(-0.004f, 0.0038f);
                }
                else if (inventoryItem.AmountTextAlign == MyInventoryAmountTextAlign.BottomRight)
                {
                    align = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM;
                    offset = new Vector2(-0.004f, -0.0025f);
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }
                listboxItem.IconTexts[align] = new MyColoredText(amount, Color.White, Color.White, MyGuiManager.GetFontMinerWarsWhite(), 0.5f, offset);
            }

            return listboxItem;
        }

        private void ListboxItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            MyGuiControlListbox senderListbox = (MyGuiControlListbox)sender;
            if (senderListbox == m_allItemsInventoryListbox)
            {
                MyGuiControlListboxItem listBoxItem = CreateCopy(m_allItemsInventoryListbox.GetItem(eventArgs.Key));
                MoveItemToListbox(m_entityInventoryListbox, listBoxItem);                
            }
            else 
            {
                m_entityInventoryListbox.RemoveItem(eventArgs.Key, false);
                RemoveCopy(eventArgs.Key);
            }
        }

        private void RemoveCopy(int itemKey) 
        {
            MyInventoryItem itemToRemove = m_inventoryItemsRepository.GetItem(itemKey);
            if (itemToRemove.Owner != EntityWithInventory.Inventory)
            {                
                bool wasInAdded = m_itemsAdded.Remove(itemToRemove);
                m_inventoryItemsRepository.RemoveItem(itemKey, true);
                Debug.Assert(wasInAdded);
            }
            else 
            {
                m_itemsRemoved.Add(itemToRemove);
            }
        }

        private MyGuiControlListboxItem CreateCopy(MyGuiControlListboxItem original) 
        {
            MyInventoryItem templateInventoryItem = m_inventoryItemsRepository.GetItem(original.Key);
            MyInventoryItem newInventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(templateInventoryItem.GetInventoryItemObjectBuilder(true));
            newInventoryItem.Amount = templateInventoryItem.Amount;
            MyGuiControlListboxItem newItem =  CreateListboxItemAndAddToRepository(newInventoryItem);
            newItem.IconTexts = original.IconTexts;
            m_itemsAdded.Add(newInventoryItem);
            return newItem;
        }

        private void ListboxItemDrag(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            MyGuiControlListbox senderListbox = (MyGuiControlListbox)sender;            

            MyDragAndDropInfo dragAndDropInfo = new MyDragAndDropInfo();
            dragAndDropInfo.ItemIndex = eventArgs.ItemIndex;
            dragAndDropInfo.RowIndex = eventArgs.RowIndex;
            dragAndDropInfo.Listbox = senderListbox;

            MyGuiControlListboxItem listBoxItem;
            if (senderListbox == m_allItemsInventoryListbox)
            {
                listBoxItem = CreateCopy(senderListbox.GetItem(eventArgs.Key));
            }
            else 
            {
                listBoxItem = senderListbox.GetItem(eventArgs.Key);
                senderListbox.RemoveItem(eventArgs.Key, false);
            }

            m_dragAndDrop.StartDragging(MyDropHandleType.LeftMousePressed, listBoxItem, dragAndDropInfo);            
        }

        private void MoveItemToListbox(MyGuiControlListbox moveTo, MyGuiControlListboxItem item, int? rowIndex = null, int? itemIndex = null) 
        {
            if (moveTo == m_allItemsInventoryListbox)
            {
                RemoveCopy(item.Key);                
            }
            else 
            {
                if (rowIndex != null && itemIndex != null)
                {
                    moveTo.AddItem(item, rowIndex.Value, itemIndex.Value);
                }
                else
                {
                    moveTo.AddItem(item);
                }
            }
        }

        private void DragAndDropListboxItemDropped(object sender, MyDragAndDropEventArgs eventArgs)
        {
            if (eventArgs.DropTo == null)
            {
                MoveItemToListbox(eventArgs.DragFrom.Listbox, eventArgs.ListboxItem, eventArgs.DragFrom.RowIndex, eventArgs.DragFrom.ItemIndex);
            }
            else 
            {
                MoveItemToListbox(eventArgs.DropTo.Listbox, eventArgs.ListboxItem);
            }
            m_dragAndDrop.Stop();
        }

        private bool SaveChangesToInventory() 
        {
            float? priceCoeficient = MyValueFormatter.GetFloatFromString(m_priceCoeficientTextbox.Text, 2, string.Empty);
            if (priceCoeficient == null) 
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MessageYouMustSetPriceCoeficient, MyTextsWrapperEnum.Error, MyTextsWrapperEnum.Ok, null));
                return false;
            }
            if (priceCoeficient < 1f) 
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MessagePriceCoeficientMustBeGreaterThanOrEqualToOne, MyTextsWrapperEnum.Error, MyTextsWrapperEnum.Ok, null));
                return false;
            }

            MyMwcInventoryTemplateTypeEnum? templateType = SelectedTemplateType;

            int? refillTime = RefillTime;

            if (refillTime != null && templateType == null || refillTime == null && templateType != null) 
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MessageYouMustSetTemplateIfYouWantRefillInventory, MyTextsWrapperEnum.Error, MyTextsWrapperEnum.Ok, null));
                return false;
            }

            EntityWithInventory.Inventory.PriceCoeficient = priceCoeficient.Value;
            EntityWithInventory.Inventory.ClearInventoryItems(false);
            foreach (int itemKey in m_entityInventoryListbox.GetItemsKeys()) 
            {
                EntityWithInventory.Inventory.AddInventoryItem(m_inventoryItemsRepository.GetItem(itemKey));
                //m_inventoryItemsRepository.RemoveItem(itemKey, false);
            }            
            EntityWithInventory.Inventory.TemplateType = templateType;
            if (PrefabContainer != null) 
            {
                PrefabContainer.RefillTime = refillTime;
            }
            return true;
        }

        private void CloseItems(ref List<MyInventoryItem> itemsToClose) 
        {
            foreach (MyInventoryItem itemToClose in itemsToClose) 
            {
                MyInventory.CloseInventoryItem(itemToClose);
            }
            itemsToClose.Clear();
        }

        public override void OnOkClick(MyGuiControlButton sender)
        {
            bool isSaveSuccess = SaveChangesToInventory();
            if (!isSaveSuccess) 
            {
                return;
            }

            base.OnOkClick(sender);            
            //m_inventoryItemsRepository.Close();            
            m_allItemsInventory.Close();
            CloseItems(ref m_itemsRemoved);
            m_itemsAdded.Clear();

            this.CloseScreen();
        }

        public override void OnCancelClick(MyGuiControlButton sender)
        {
            base.OnCancelClick(sender);
            m_allItemsInventory.Close();
            CloseItems(ref m_itemsAdded);
            m_itemsRemoved.Clear();
        }
    }
}
