using System;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Inventory;
using System.Text;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Managers.Others;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorSmallShip : MyGuiScreenEditorObject3DBase
    {
        public Action OnOk = null;

        MyGuiControlCombobox m_selectSmallShipCombobox;
        MyGuiControlCombobox m_selectShipFactionCombobox;
        
        MyGuiControlCombobox m_selectAITemplateCombobox;

        MyGuiControlSlider m_aggresivitySlider;
        MyGuiControlLabel m_aggresivityLabel;

        MyGuiControlSlider m_seeDistanceSlider;
        MyGuiControlLabel m_seeDistanceLabel;

        MyMwcObjectBuilder_SmallShip_Bot m_botBuilder;
        MySmallShipBot m_bot;
        List<MySmallShipBuilderWithName> m_templatesBuilders;
        MyInventory m_inventory;

        MyMwcObjectBuilder_SmallShip_Bot m_newBotBuilderToInit;
        Matrix m_newBotWorldMatrixToInit;
        
        public MyGuiScreenEditorSmallShip(Vector2? screenPosition)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.CreateSmallShip, screenPosition)
        {
            Init();
        }

        public MyGuiScreenEditorSmallShip(MySmallShip smallShip)
            : base(smallShip, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditSmallShip)
        {
            m_bot = smallShip as MySmallShipBot;

            Init();

            m_selectShipFactionCombobox.SelectItemByKey((int)m_bot.Faction);
            m_selectAITemplateCombobox.SelectItemByKey((int)m_bot.AITemplate.TemplateId);
            m_aggresivitySlider.SetValue(m_bot.Aggressivity);
            m_seeDistanceSlider.SetValue(m_bot.SeeDistance);
            m_selectSmallShipCombobox.SelectItemByKey((int)m_bot.ShipType);
        }

        public MyGuiScreenEditorSmallShip(MyMwcObjectBuilder_SmallShip_Bot botBuilder, List<MySmallShipBuilderWithName> templateBuilders, int selectedIndex)
            : base(null, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.EditSmallShip)
        {
            m_botBuilder = botBuilder;
            m_templatesBuilders = templateBuilders;

            Init();

            m_selectShipFactionCombobox.SelectItemByKey((int)m_botBuilder.Faction);
            m_selectAITemplateCombobox.SelectItemByKey((int)m_botBuilder.AITemplate);
            m_aggresivitySlider.SetValue(m_botBuilder.Aggressivity);
            m_seeDistanceSlider.SetValue(m_botBuilder.SeeDistance);
            m_selectSmallShipCombobox.SelectItemByIndex(selectedIndex);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorSmallShip";
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.73f, 0.93f);
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + new Vector2(0.02f, 0.01f);
            Vector2 controlsOriginRight = GetControlsOriginRightFromScreenSize();

            // Add screen title
            AddCaption();            

            #region Faction nationality

            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1 * CONTROLS_DELTA, null, MyTextsWrapperEnum.SetShipFaction, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_selectShipFactionCombobox = new MyGuiControlCombobox(this, controlsOriginRight + 1 * CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0),
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 5);

            foreach (MyMwcObjectBuilder_FactionEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_ShipFactionNationalityEnumValues)
            {
                MyGuiHelperBase factionNationalityHelper = MyGuiSmallShipHelpers.GetMyGuiSmallShipFactionNationality(enumValue);
                m_selectShipFactionCombobox.AddItem((int)enumValue, null, factionNationalityHelper.Description);
            }

            m_selectShipFactionCombobox.SelectItemByKey(1);
            Controls.Add(m_selectShipFactionCombobox);

            #endregion

            // AI Template
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 2 * CONTROLS_DELTA, null, MyTextsWrapperEnum.AITemplate, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_selectAITemplateCombobox = new MyGuiControlCombobox(this, controlsOriginRight + 2 * CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_MEDIUM_SIZE.X / 2.0f, 0),
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 5);

            foreach (MyAITemplateEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_BotAITemplateValues)
            {
                MyGuiHelperBase aiTemplateHelper = MyGuiSmallShipHelpers.GetMyGuiSmallShipBotAITemplate(enumValue);
                if (aiTemplateHelper != null)
                    m_selectAITemplateCombobox.AddItem((int)enumValue, null, aiTemplateHelper.Description);
            }

            m_selectAITemplateCombobox.SelectItemByKey(0);
            Controls.Add(m_selectAITemplateCombobox);

            // Aggresivity
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 3 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Aggressivity, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_aggresivitySlider = new MyGuiControlSlider(this, controlsOriginRight + 3 * CONTROLS_DELTA + new Vector2(MyGuiConstants.SLIDER_WIDTH / 2, 0), MyGuiConstants.SLIDER_WIDTH,
                                        0, 1, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                                        new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);

            m_aggresivityLabel = new MyGuiControlLabel(this, controlsOriginRight + 3 * CONTROLS_DELTA + new Vector2(MyGuiConstants.SLIDER_WIDTH, 0), null, MyTextsWrapperEnum.SetShipFaction, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            Controls.Add(m_aggresivitySlider);
            Controls.Add(m_aggresivityLabel);

            m_aggresivitySlider.OnChange += OnAggresivityChanged;
            OnAggresivityChanged(m_aggresivitySlider);

            // See distance
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 4 * CONTROLS_DELTA, null, MyTextsWrapperEnum.SeeDistance, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_seeDistanceSlider = new MyGuiControlSlider(this, controlsOriginRight + 4 * CONTROLS_DELTA + new Vector2(MyGuiConstants.SLIDER_WIDTH / 2, 0), MyGuiConstants.SLIDER_WIDTH,
                                        0, 1000, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                                        new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);

            m_seeDistanceLabel = new MyGuiControlLabel(this, controlsOriginRight + 4 * CONTROLS_DELTA + new Vector2(MyGuiConstants.SLIDER_WIDTH, 0), null, MyTextsWrapperEnum.SetShipFaction, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            Controls.Add(m_seeDistanceSlider);
            Controls.Add(m_seeDistanceLabel);

            m_seeDistanceSlider.OnChange += OnSeeDistanceChanged;
            OnSeeDistanceChanged(m_seeDistanceSlider);

            #region Smallship Model

            MyGuiControlLabel smallShipLabel = new MyGuiControlLabel(this, controlsOriginLeft + 5 * CONTROLS_DELTA, null, MyTextsWrapperEnum.ChooseModel, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(smallShipLabel);

            //COMBOBOX - small ship
            var comboboxPredefinedSize = MyGuiControlPreDefinedSize.LONGMEDIUM;
            var comboboxSize = MyGuiControlCombobox.GetPredefinedControlSize(comboboxPredefinedSize);
            m_selectSmallShipCombobox = new MyGuiControlCombobox(
                this,
                controlsOriginLeft + 5 * CONTROLS_DELTA + 0.5f * new Vector2(comboboxSize.X, 0) + new Vector2(0.19f, 0.01f),
                comboboxPredefinedSize,
                MyGuiConstants.COMBOBOX_BACKGROUND_COLOR,
                MyGuiConstants.COMBOBOX_TEXT_SCALE,
                6,
                true,
                false,
                true);

            if (m_templatesBuilders == null)
            {
                foreach (MyMwcObjectBuilder_SmallShip_TypesEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues)
                {
                    //MyGuiSmallShipHelperSmallShip smallShipHelper = MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperSmallShip(enumValue);
                    MyGuiSmallShipHelperSmallShip smallShipHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)enumValue) as MyGuiSmallShipHelperSmallShip;
                    m_selectSmallShipCombobox.AddItem((int)enumValue, smallShipHelper.Icon, smallShipHelper.Name);
                }
                m_selectSmallShipCombobox.SelectItemByKey(1);
            }
            else 
            {
                for(int i = 0; i < m_templatesBuilders.Count; i++)
                {
                    var templateBuilder = m_templatesBuilders[i];
                    m_selectSmallShipCombobox.AddItem(i, templateBuilder.Name);
                }                
            }
            
            m_selectSmallShipCombobox.OnSelectItemDoubleClick += OnOkClick;
            Controls.Add(m_selectSmallShipCombobox);
            #endregion

            if (m_bot != null)
            {
                Controls.Add(new MyGuiControlButton(this, controlsOriginLeft + 10 * CONTROLS_DELTA + new Vector2(0.07f, 0.02f), MyGuiConstants.PROGRESS_CANCEL_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Inventory, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnInventoryClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

                AddActivatedCheckbox(controlsOriginLeft, m_bot.Activated);
            }

            AddOkAndCancelButtonControls(new Vector2(0, -0.02f));
        }

        private void OnInventoryClick(MyGuiControlButton sender)
        {
            MyInventory inventory = new MyInventory();
            inventory.FillInventoryWithAllItems(null, 100);

            List<MySmallShipBuilderWithName> tempList = new List<MySmallShipBuilderWithName>();
            tempList.Add(new MySmallShipBuilderWithName(m_bot.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Bot));

            MyGuiScreenInventory inventoryScreen = new MyGuiScreenInventory(tempList, 0, inventory.GetObjectBuilder(false), null, true);
            inventoryScreen.OnSave += OnEditBotFromScreen;
            MyGuiManager.AddScreen(inventoryScreen);
        }

        private void OnEditBotFromScreen(MyGuiScreenInventory sender, MyGuiScreenInventorySaveResult saveResult)
        {
            MySmallShipBuilderWithName resultBuilder = saveResult.SmallShipsObjectBuilders[saveResult.CurrentIndex];
            m_inventoryBuilder = resultBuilder.Builder;
        }

        private void OnAggresivityChanged(MyGuiControlSlider sender)
        {
            m_aggresivityLabel.UpdateText(string.Format(" {0:0.00}", m_aggresivitySlider.GetValue()));
        }

        private void OnSeeDistanceChanged(MyGuiControlSlider sender)
        {
            m_seeDistanceLabel.UpdateText(string.Format(" {0:#,###0} " + MyTextsWrapper.Get(MyTextsWrapperEnum.MetersLong).ToString(), m_seeDistanceSlider.GetValue()));
        }

        private void OnOkClick()
        {
            OnOkClick(null);
        }

        private MyMwcObjectBuilder_SmallShip m_inventoryBuilder;

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);

            if (m_botBuilder != null)
            {
                m_botBuilder.Faction = (MyMwcObjectBuilder_FactionEnum)m_selectShipFactionCombobox.GetSelectedKey();
                m_botBuilder.AITemplate = (MyAITemplateEnum)m_selectAITemplateCombobox.GetSelectedKey();
                m_botBuilder.Aggressivity = m_aggresivitySlider.GetValue();
                m_botBuilder.SeeDistance = m_seeDistanceSlider.GetValue();
                var templateBuilder = m_templatesBuilders[m_selectSmallShipCombobox.GetSelectedKey()];
                bool isTemplate = templateBuilder.UserData != null;
                if (isTemplate)
                {
                    var template = templateBuilder.UserData as MySmallShipTemplate;                    
                    if (m_botBuilder.ShipTemplateID == null)
                    {
                        if (m_inventoryBuilder == null)
                        {
                            m_botBuilder.Inventory = null;
                            m_botBuilder.Weapons = null;
                            m_botBuilder.Engine = null;
                            m_botBuilder.Armor = null;
                            m_botBuilder.Radar = null;
                        }
                        else
                        {
                            m_botBuilder.Inventory = m_inventoryBuilder.Inventory;
                            m_botBuilder.Weapons = m_inventoryBuilder.Weapons;
                            m_botBuilder.Engine = m_inventoryBuilder.Engine;
                            m_botBuilder.Armor = m_inventoryBuilder.Armor;
                            m_botBuilder.Radar = m_inventoryBuilder.Radar;
                        }
                    }                    
                    m_botBuilder.ShipTemplateID = template.ID;
                }
                else
                {
                    var builderWithAllItems = MyMwcObjectBuilder_SmallShip_Bot.CreateObjectBuilderWithAllItems(templateBuilder.Builder.ShipType, MyShipTypeConstants.GetShipTypeProperties(templateBuilder.Builder.ShipType).GamePlay.CargoCapacity);
                    if (m_botBuilder.ShipTemplateID != null) 
                    {
                        m_botBuilder.Inventory = builderWithAllItems.Inventory;
                        m_botBuilder.Weapons = builderWithAllItems.Weapons;
                        m_botBuilder.Engine = builderWithAllItems.Engine;
                        m_botBuilder.Armor = builderWithAllItems.Armor;
                        m_botBuilder.Radar = builderWithAllItems.Radar;
                    }
                    m_botBuilder.ShipTemplateID = null;
                }
                m_botBuilder.ShipType = templateBuilder.Builder.ShipType;
                CloseAndCallOnOk();
            }
            else if (m_bot != null) // edit SmallShip
            {
                Matrix matrix = m_bot.WorldMatrix;
                MyMwcObjectBuilder_SmallShip_Bot botBuilder = m_bot.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Bot;
                System.Diagnostics.Debug.Assert(botBuilder != null);

                botBuilder.Faction = (MyMwcObjectBuilder_FactionEnum)m_selectShipFactionCombobox.GetSelectedKey();
                botBuilder.AITemplate = (MyAITemplateEnum)m_selectAITemplateCombobox.GetSelectedKey();
                botBuilder.Aggressivity = m_aggresivitySlider.GetValue();
                botBuilder.SeeDistance = m_seeDistanceSlider.GetValue();
                botBuilder.ShipType = (MyMwcObjectBuilder_SmallShip_TypesEnum)m_selectSmallShipCombobox.GetSelectedKey();

                if (m_inventoryBuilder != null)
                {
                    botBuilder.Inventory = m_inventoryBuilder.Inventory;
                    botBuilder.Weapons = m_inventoryBuilder.Weapons;
                    botBuilder.Engine = m_inventoryBuilder.Engine;
                    botBuilder.Armor = m_inventoryBuilder.Armor;
                    botBuilder.Radar = m_inventoryBuilder.Radar;
                }

                MyEditorGizmo.ClearSelection();
                m_newBotBuilderToInit = botBuilder;
                m_newBotWorldMatrixToInit = matrix;
                m_bot.MarkForClose();
                m_bot.OnClose += OnOldBodClose;
            }
            else // create SmallShip
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum shipType = (MyMwcObjectBuilder_SmallShip_TypesEnum)
                    Enum.ToObject(typeof(MyMwcObjectBuilder_SmallShip_TypesEnum), m_selectSmallShipCombobox.GetSelectedKey());

                MyMwcObjectBuilder_FactionEnum shipFaction = (MyMwcObjectBuilder_FactionEnum)
                    Enum.ToObject(typeof(MyMwcObjectBuilder_FactionEnum), m_selectShipFactionCombobox.GetSelectedKey());

                MyMwcPositionAndOrientation positionAndOrientation = new MyMwcPositionAndOrientation(m_newObjectPosition, Vector3.Forward, Vector3.Up);
                MyMwcObjectBuilder_SmallShip_Bot botBuilder = MyEditor.CreateDefaultBotObjectBuilder(positionAndOrientation.Position, positionAndOrientation.Forward,
                    positionAndOrientation.Up, shipType, shipFaction);

                botBuilder.AITemplate = (MyAITemplateEnum)m_selectAITemplateCombobox.GetSelectedKey();
                botBuilder.Aggressivity = m_aggresivitySlider.GetValue();
                botBuilder.SeeDistance = m_seeDistanceSlider.GetValue();

                MyEditor.Static.CreateFromObjectBuilder(botBuilder, Matrix.CreateWorld(m_newObjectPosition, Vector3.Forward, Vector3.Up), m_screenPosition);
                CloseAndCallOnOk();
            }            
        }

        private void OnOldBodClose(MyEntity entity) 
        {
            var newBot = MyEntities.CreateFromObjectBuilderAndAdd(null, m_newBotBuilderToInit, m_newBotWorldMatrixToInit);
            newBot.Activate(m_activatedCheckbox.Checked, false);
            MyEditorGizmo.AddEntityToSelection(newBot);
            CloseAndCallOnOk();
        }

        private void CloseAndCallOnOk() 
        {
            CloseScreen();

            if (OnOk != null)
            {
                OnOk();
            }
        }

        protected override Vector2 GetControlsOriginLeftFromScreenSize()
        {
            return base.GetControlsOriginLeftFromScreenSize() + new Vector2(0.01f, 0);
        }

    }
}
