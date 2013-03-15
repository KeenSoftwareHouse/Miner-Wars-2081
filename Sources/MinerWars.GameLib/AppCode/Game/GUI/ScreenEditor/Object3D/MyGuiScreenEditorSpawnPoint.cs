using System;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using System.Text;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.Others;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{

    // gui screen
    class MyGuiScreenEditorSpawnPoint : MyGuiScreenEditorObject3DBase
    {
        private const int TEXTBOX_NUMBERS_MAX_LENGTH = 6;
        public const int TEMPLATE_INDEX_OFFSET = 10000;

        MySpawnPoint m_spawnPoint;

        MyGuiControlCombobox m_selectShipFactionCombobox;
        MyGuiControlCombobox m_waypointPathCombobox;
        MyGuiControlCombobox m_patrolModeCombobox;

        MyGuiControlSlider m_radiusSlider;
        MyGuiControlSlider m_firstSpawnTimeSlider;
        MyGuiControlSlider m_respawnTimeSlider;
        MyGuiControlListbox m_selectShipsListbox;

        MyGuiControlLabel m_radiusLabel;
        MyGuiControlLabel m_firstSpawnLabel;
        MyGuiControlLabel m_respawnLabel;

        MyGuiControlCheckbox m_spawnInGroupsCheckbox;
        MyGuiControlCheckbox m_activeCheckbox;
        MyGuiControlTextbox m_spawnedBotsTextbox;

        Dictionary<int, BotTemplate> m_bots;

        public MyGuiScreenEditorSpawnPoint()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.SpawnPoint)
        {
            Init();
        }

        public MyGuiScreenEditorSpawnPoint(MySpawnPoint spawnPoint)
            : base(spawnPoint, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.SpawnPoint)
        {
            m_spawnPoint = spawnPoint;
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorSpawnPoint";
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.97f, 0.85f);
            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize() + new Vector2(0.04f, 0);

            m_bots = new Dictionary<int, BotTemplate>();

            // Add screen title
            AddCaption(new Vector2(0, 0.028f));

            //Faction
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1 * CONTROLS_DELTA, null, MyTextsWrapperEnum.SetShipFaction, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_selectShipFactionCombobox = new MyGuiControlCombobox(this, (new Vector2(0.31f, 0)) + controlsOriginLeft + 1 * CONTROLS_DELTA,
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 8);

            foreach (MyMwcObjectBuilder_FactionEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_ShipFactionNationalityEnumValues)
            {
                MyGuiHelperBase factionNationalityHelper = MyGuiSmallShipHelpers.GetMyGuiSmallShipFactionNationality(enumValue);
                m_selectShipFactionCombobox.AddItem((int)enumValue, null, factionNationalityHelper.Description);
            }

            m_selectShipFactionCombobox.SelectItemByKey((int)MyMwcObjectBuilder_FactionEnum.China);//hopefuly china
            Controls.Add(m_selectShipFactionCombobox);

            //radius slider
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 2 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Radius, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_radiusSlider = new MyGuiControlSlider(this, (new Vector2(0.25f, 0) + controlsOriginLeft) + 2 * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                15, 200, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_radiusSlider.SetNormalizedValue(0.2f);
            m_radiusSlider.OnChange = OnComponentChange;
            Controls.Add(m_radiusSlider);
            Controls.Add(m_radiusLabel = new MyGuiControlLabel(this, new Vector2(m_radiusSlider.GetPosition().X + m_radiusSlider.GetSize().Value.X / 2 + 0.01f, controlsOriginLeft.Y) + 2 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Radius, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            //first spawn timer
            Controls.Add(new MyGuiControlLabel(this, 
                controlsOriginLeft + 3 * CONTROLS_DELTA, null, MyTextsWrapperEnum.FirstSpawn, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_firstSpawnTimeSlider = new MyGuiControlSlider(this, 
                (new Vector2(0.25f, 0) + controlsOriginLeft) + 3 * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0, 10 * 60 * 1000, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_firstSpawnTimeSlider.OnChange = OnComponentChange;
            m_firstSpawnTimeSlider.SetNormalizedValue(0.0f);
            Controls.Add(m_firstSpawnTimeSlider);
            Controls.Add(m_firstSpawnLabel = new MyGuiControlLabel(this,
                new Vector2(m_firstSpawnTimeSlider.GetPosition().X + m_firstSpawnTimeSlider.GetSize().Value.X / 2 + 0.01f, controlsOriginLeft.Y) + 3 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Respawn, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            //respawn timer
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 4 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Respawn, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_respawnTimeSlider = new MyGuiControlSlider(this, controlsOriginLeft + new Vector2(0.25f, 0) + 4 * CONTROLS_DELTA, MyGuiConstants.SLIDER_WIDTH,
                0, 10 * 60 * 1000, MyGuiConstants.SLIDER_BACKGROUND_COLOR,
                new StringBuilder(), MyGuiConstants.SLIDER_WIDTH_LABEL, 0, MyGuiConstants.LABEL_TEXT_SCALE * 0.85f);
            m_respawnTimeSlider.OnChange = OnComponentChange;
            m_respawnTimeSlider.SetNormalizedValue(0.0f);
            Controls.Add(m_respawnTimeSlider);
            Controls.Add(m_respawnLabel = new MyGuiControlLabel(this, new Vector2(m_respawnTimeSlider.GetPosition().X + m_respawnTimeSlider.GetSize().Value.X / 2 + 0.01f, controlsOriginLeft.Y) + 4 * CONTROLS_DELTA, null, MyTextsWrapperEnum.Respawn, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            //waypoints
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 5 * CONTROLS_DELTA, null, MyTextsWrapperEnum.WayPointPath, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_waypointPathCombobox = new MyGuiControlCombobox(this, new Vector2(0.31f, 0) + controlsOriginLeft + 5 * CONTROLS_DELTA,
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 8);

            int k = 0;
            int selected = 0;
            m_waypointPathCombobox.AddItem(k++, null, MyTextsWrapper.Get(MyTextsWrapperEnum.None));
            foreach (var path in MyWayPointGraph.StoredPaths)
            {
                if (HasEntity() && String.Compare(path.Name, m_spawnPoint.GetWaypointPath()) == 0)
                {
                    selected = k;
                }
                m_waypointPathCombobox.AddItem(k++, null, new StringBuilder(path.Name));
            }

            m_waypointPathCombobox.SelectItemByKey(selected);// 
            Controls.Add(m_waypointPathCombobox);

            // patrol mode
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 6 * CONTROLS_DELTA, null, MyTextsWrapperEnum.PatrolMode, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            m_patrolModeCombobox = new MyGuiControlCombobox(this, new Vector2(0.31f, 0) + controlsOriginLeft + 6 * CONTROLS_DELTA,
                MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 8);

            foreach (MyPatrolMode enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_PatrolModes)
            {
                MyGuiHelperBase patrolModeHelper = MyGuiSmallShipHelpers.GetMyGuiSmallShipPatrolMode(enumValue);
                m_patrolModeCombobox.AddItem((int)enumValue, null, patrolModeHelper.Description);
            } 
            
            m_patrolModeCombobox.SelectItemByKey(HasEntity() ? (int)m_spawnPoint.PatrolMode : 0);
            Controls.Add(m_patrolModeCombobox);

            #region Smallship Bots To Spawn
            //MyGuiControlLabel smallShipLabel = new MyGuiControlLabel(this, controlsOriginLeft + 6 * CONTROLS_DELTA, null, MyTextsWrapperEnum.ChooseModel, MyGuiConstants.LABEL_TEXT_COLOR,
            //    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            //Controls.Add(smallShipLabel);

            m_selectShipsListbox = new MyGuiControlListbox(this, controlsOriginLeft + 7 * CONTROLS_DELTA + new Vector2(MyGuiConstants.COMBOBOX_LARGE_SIZE.X / 2.0f + MyGuiConstants.LISTBOX_SCROLLBAR_WIDTH / 2.0f, MyGuiConstants.COMBOBOX_LARGE_SIZE.Y * 2.5f), MyGuiConstants.LISTBOX_LONGMEDIUM_SIZE,
                MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, null, MyGuiConstants.LABEL_TEXT_SCALE, 1, 6, 1, false, true, false);
                

            //m_selectShipsListbox.ItemSelect = OnItemSelect;
            m_selectShipsListbox.ItemDoubleClick += OnDoubleClick;

            Controls.Add(m_selectShipsListbox);


            Vector2 columnOriginLeft = new Vector2(0.178f, controlsOriginLeft.Y);
            Vector2 controlsOriginRight = new Vector2(m_size.Value.X / 2.0f - 0.05f, controlsOriginLeft.Y);

            //  Activated
            Controls.Add(new MyGuiControlLabel(this, columnOriginLeft + MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.Active, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_activeCheckbox = new MyGuiControlCheckbox(this, controlsOriginRight + MyGuiConstants.CONTROLS_DELTA - new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f + 0.02f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_activeCheckbox);

            //  Spawn in groups
            Controls.Add(new MyGuiControlLabel(this, columnOriginLeft + 2 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.SpawnInGroups, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_spawnInGroupsCheckbox = new MyGuiControlCheckbox(this, controlsOriginRight + 2 * MyGuiConstants.CONTROLS_DELTA - new Vector2(MyGuiConstants.CHECKBOX_SIZE.X / 2.0f + 0.02f, 0), true, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
            Controls.Add(m_spawnInGroupsCheckbox);

            Controls.Add(new MyGuiControlLabel(this, columnOriginLeft + 3 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.SpawnedBots,
                                               MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_spawnedBotsTextbox = new MyGuiControlTextbox(this, columnOriginLeft + 4 * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2 - 0.01f, 0), MyGuiControlPreDefinedSize.MEDIUM,
                                               string.Empty, 
                                               TEXTBOX_NUMBERS_MAX_LENGTH,
                                               MyGuiConstants.TEXTBOX_BACKGROUND_COLOR,
                                               MyGuiConstants.LABEL_TEXT_SCALE,
                                               MyGuiControlTextboxType.DIGITS_ONLY);
            Controls.Add(m_spawnedBotsTextbox);

            #endregion

            #region Bots Listbox Buttons
            Controls.Add(new MyGuiControlButton(this, controlsOriginLeft + 7 * CONTROLS_DELTA + new Vector2(0.77f, -0.005f), MyGuiConstants.PROGRESS_CANCEL_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Add, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnAddClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, controlsOriginLeft + 8 * CONTROLS_DELTA + new Vector2(0.77f, -0.005f), MyGuiConstants.PROGRESS_CANCEL_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Edit, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnEditClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, controlsOriginLeft + 9 * CONTROLS_DELTA + new Vector2(0.77f, -0.005f), MyGuiConstants.PROGRESS_CANCEL_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Inventory, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnInventoryClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, controlsOriginLeft + 10 * CONTROLS_DELTA + new Vector2(0.77f, -0.005f), MyGuiConstants.PROGRESS_CANCEL_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Copy, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnDuplicateClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, controlsOriginLeft + 11 * CONTROLS_DELTA + new Vector2(0.77f, -0.005f), MyGuiConstants.PROGRESS_CANCEL_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Delete, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnDeleteClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            #endregion

            AddOkAndCancelButtonControls(new Vector2(0, -0.038f));

            if (HasEntity())
            {
                m_radiusSlider.SetValue( m_spawnPoint.BoundingSphereRadius );
                m_spawnInGroupsCheckbox.Checked = m_spawnPoint.SpawnInGroups;
                m_spawnedBotsTextbox.Text = m_spawnPoint.LeftToSpawn >= 0 ? m_spawnPoint.LeftToSpawn.ToString() : string.Empty;
                m_firstSpawnTimeSlider.SetValue(m_spawnPoint.FirstSpawnTimer);
                m_respawnTimeSlider.SetValue(m_spawnPoint.RespawnTimer);

                m_selectShipFactionCombobox.SelectItemByKey((int)m_spawnPoint.Faction);
                m_bots.Clear();

                foreach (BotTemplate bt in m_spawnPoint.GetBotTemplates())
                {
                    if (bt.m_builder.ShipTemplateID != null)                     
                    {
                        AddBot(bt.m_builder, MySmallShipTemplates.GetTemplate(bt.m_builder.ShipTemplateID.Value));
                    }
                    else
                    {
                        AddBot(bt.m_builder);
                    }
                }

                m_activeCheckbox.Checked = m_spawnPoint.IsActive();
            }

            // Just UI update
            OnComponentChange(null);
        }

        private String GenerateNameFromBotTemplate(BotTemplate btmp)
        {
            return btmp.m_name + " "/* + btmp.m_minCount.ToString() + " - " + btmp.m_maxCount.ToString()*/;
        }

        private void AddBot(int key, BotTemplate btmp)
        {
            String bld = GenerateNameFromBotTemplate(btmp);
            m_bots[key] = btmp;
            m_selectShipsListbox.AddItem(key, new StringBuilder(bld), null);
        }

        private void AddBot(MyMwcObjectBuilder_SmallShip bldr)
        {
            BotTemplate newTemplate = new BotTemplate();
            MyMwcObjectBuilder_SmallShip_Bot botBuilder = bldr as MyMwcObjectBuilder_SmallShip_Bot;
            System.Diagnostics.Debug.Assert(botBuilder != null);

            newTemplate.m_builder = new MyMwcObjectBuilder_SmallShip_Bot(
                bldr.ShipType,
                bldr.Inventory,
                bldr.Weapons,
                bldr.Engine,
                bldr.AssignmentOfAmmo,
                bldr.Armor,
                bldr.Radar,
                bldr.ShipMaxHealth,
                bldr.ShipHealthRatio,
                bldr.ArmorHealth,
                bldr.Oxygen,
                bldr.Fuel,
                bldr.ReflectorLight,
                bldr.ReflectorLongRange,
                bldr.Faction,
                botBuilder != null ? botBuilder.AITemplate : MyAITemplateEnum.DEFAULT,
                botBuilder != null ? botBuilder.Aggressivity : 0,
                botBuilder != null ? botBuilder.SeeDistance : 1000,
                botBuilder != null ? botBuilder.SleepDistance : 1000,
                MyPatrolMode.CYCLE,
                null,
                BotBehaviorType.IDLE,
                MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, 0, false, true); // faction will be assigned after spawnpoint genertation
            MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)bldr.ShipType);
            //m_comboItems.AddItem(i, MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, bldr.ShipType);

            //  MyMwcObjectBuilderTypeEnum.SmallShip, (int)m_inventory.SmallShips[i]).Description);

            newTemplate.m_name = ((MyGuiSmallShipHelperSmallShip)MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)bldr.ShipType)).Name.ToString();//bldr.ShipType.ToString();

            // find nearest new non template index
            int index = -1;
            foreach (int key in m_bots.Keys)
            {
                if (index < key && key < TEMPLATE_INDEX_OFFSET)
                {
                    index = key;
                }
            }
            index++;

            AddBot(index, newTemplate);

        }

        private void AddBot(MyMwcObjectBuilder_SmallShip bldr, MySmallShipTemplate smallShipTemplate)
        {
            BotTemplate newTemplate = new BotTemplate();
            MyMwcObjectBuilder_SmallShip_Bot botBuilder = bldr as MyMwcObjectBuilder_SmallShip_Bot;            

            newTemplate.m_builder = new MyMwcObjectBuilder_SmallShip_Bot(
                smallShipTemplate.Builder.ShipType,
                null,
                null,
                null,
                smallShipTemplate.Builder.AssignmentOfAmmo,
                null,
                null,
                smallShipTemplate.Builder.ShipMaxHealth,
                smallShipTemplate.Builder.ShipHealthRatio,
                smallShipTemplate.Builder.ArmorHealth,
                smallShipTemplate.Builder.Oxygen,
                smallShipTemplate.Builder.Fuel,
                smallShipTemplate.Builder.ReflectorLight,
                smallShipTemplate.Builder.ReflectorLongRange,
                smallShipTemplate.Builder.Faction,
                botBuilder != null ? botBuilder.AITemplate : MyAITemplateEnum.DEFAULT,
                botBuilder != null ? botBuilder.Aggressivity : 0,
                botBuilder != null ? botBuilder.SeeDistance : 1000,
                botBuilder != null ? botBuilder.SleepDistance : 1000,
                MyPatrolMode.CYCLE,
                null,
                BotBehaviorType.IDLE,
                MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, 0, false, true); // faction will be assigned after spawnpoint genertation
            newTemplate.m_builder.ShipTemplateID = smallShipTemplate.ID;            
            
            newTemplate.m_name = smallShipTemplate.Name.ToString();

            // find nearest new non template index
            int index = -1;
            foreach (int key in m_bots.Keys)
            {
                if (index < key && key < TEMPLATE_INDEX_OFFSET)
                {
                    index = key;
                }
            }
            index++;

            AddBot(index, newTemplate);

        }

        private int GetSpawnCount()
        {
            int spawnCount;
            return (!string.IsNullOrEmpty(m_spawnedBotsTextbox.Text) && int.TryParse(m_spawnedBotsTextbox.Text, out spawnCount)) ? spawnCount : -1;
        }

        private void OnEditBotFromScreen(MyGuiScreenInventory sender, MyGuiScreenInventorySaveResult saveResult)
        {
            bool isTemplate = !saveResult.WasAnythingTransfered && saveResult.SmallShipsObjectBuilders[saveResult.CurrentIndex].UserData != null;
            MySmallShipBuilderWithName resultBuilder = saveResult.SmallShipsObjectBuilders[saveResult.CurrentIndex];
            if (isTemplate)
            {
                OnEditBot(resultBuilder.Builder, (MySmallShipTemplate)resultBuilder.UserData);
            }
            else
            {
                OnEditBot(resultBuilder.Builder);
            }
        }

        private void OnAddBotFromScreen(MyGuiScreenInventory sender, MyGuiScreenInventorySaveResult saveResult)
        {
            bool isTemplate = !saveResult.WasAnythingTransfered && saveResult.SmallShipsObjectBuilders[saveResult.CurrentIndex].UserData != null;
            MySmallShipBuilderWithName resultBuilder = saveResult.SmallShipsObjectBuilders[saveResult.CurrentIndex];
            if (isTemplate)
            {
                AddBot(resultBuilder.Builder, (MySmallShipTemplate)resultBuilder.UserData);
            }
            else
            {
                AddBot(resultBuilder.Builder);
            }
        }        

        private void OnEditBot(MyMwcObjectBuilder_SmallShip bldr)
        {
            BotTemplate newTemplate = new BotTemplate();
            MyMwcObjectBuilder_SmallShip_Bot botBuilder = bldr as MyMwcObjectBuilder_SmallShip_Bot;
            System.Diagnostics.Debug.Assert(botBuilder != null);

            newTemplate.m_builder = new MyMwcObjectBuilder_SmallShip_Bot(
                bldr.ShipType,
                bldr.Inventory,
                bldr.Weapons,
                bldr.Engine,
                bldr.AssignmentOfAmmo,
                bldr.Armor,
                bldr.Radar,
                bldr.ShipMaxHealth,
                bldr.ShipHealthRatio,
                bldr.ArmorHealth,
                bldr.Oxygen,
                bldr.Fuel,
                bldr.ReflectorLight,
                bldr.ReflectorLongRange,
                bldr.Faction,
                botBuilder != null ? botBuilder.AITemplate : MyAITemplateEnum.DEFAULT,
                botBuilder != null ? botBuilder.Aggressivity : 0,
                botBuilder != null ? botBuilder.SeeDistance : 1000,
                botBuilder != null ? botBuilder.SleepDistance : 1000,
                MyPatrolMode.CYCLE,
                null,
                BotBehaviorType.IDLE,
                MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, bldr.AIPriority, false, true); // faction will be assigned after spawnpoint genertation
            newTemplate.m_name = bldr.ShipType.ToString();
            int key = m_selectShipsListbox.GetSelectedItem().Key;
            m_selectShipsListbox.RemoveItem(key);

            var itemName = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)newTemplate.m_builder.ShipType).Description;
            m_selectShipsListbox.AddItem(key, itemName);

            m_bots[key] = newTemplate;
        }

        private void OnEditBot(MyMwcObjectBuilder_SmallShip bldr, MySmallShipTemplate smallShipTemplate)
        {
            BotTemplate newTemplate = new BotTemplate();
            MyMwcObjectBuilder_SmallShip_Bot botBuilder = bldr as MyMwcObjectBuilder_SmallShip_Bot;

            newTemplate.m_builder = new MyMwcObjectBuilder_SmallShip_Bot(
                smallShipTemplate.Builder.ShipType,
                null,
                null,
                null,
                smallShipTemplate.Builder.AssignmentOfAmmo,
                null,
                null,
                smallShipTemplate.Builder.ShipMaxHealth,
                smallShipTemplate.Builder.ShipHealthRatio,
                smallShipTemplate.Builder.ArmorHealth,
                smallShipTemplate.Builder.Oxygen,
                smallShipTemplate.Builder.Fuel,
                smallShipTemplate.Builder.ReflectorLight,
                smallShipTemplate.Builder.ReflectorLongRange,
                smallShipTemplate.Builder.Faction,
                botBuilder != null ? botBuilder.AITemplate : MyAITemplateEnum.DEFAULT,
                botBuilder != null ? botBuilder.Aggressivity : 0,
                botBuilder != null ? botBuilder.SeeDistance : 1000,
                botBuilder != null ? botBuilder.SleepDistance : 1000,
                MyPatrolMode.CYCLE,
                null,
                BotBehaviorType.IDLE,
                MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, bldr.AIPriority, false, true); // faction will be assigned after spawnpoint genertation
            newTemplate.m_builder.ShipTemplateID = smallShipTemplate.ID;
            newTemplate.m_name = smallShipTemplate.Name.ToString();
            int key = m_selectShipsListbox.GetSelectedItem().Key;
            m_selectShipsListbox.RemoveItem(key);
            
            m_selectShipsListbox.AddItem(key, smallShipTemplate.Name);

            m_bots[key] = newTemplate;
        }

        private void OnDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            OnInventoryClick(null);
        }

        private void OnDuplicateClick(MyGuiControlButton sender)
        {

            if (m_selectShipsListbox.GetSelectedItem() == null)
                return;
            BotTemplate btmp;
            m_bots.TryGetValue(m_selectShipsListbox.GetSelectedItem().Key, out btmp);
            if (btmp.m_builder.ShipTemplateID != null)
            {
                AddBot(btmp.m_builder, MySmallShipTemplates.GetTemplate(btmp.m_builder.ShipTemplateID.Value));
            }
            else
            {
                AddBot(btmp.m_builder);
            }

        }

        private void OnDeleteClick(MyGuiControlButton sender)
        {
            if (m_selectShipsListbox.GetSelectedItem() == null)
                return;
            BotTemplate btmp;
            m_bots.TryGetValue(m_selectShipsListbox.GetSelectedItem().Key, out btmp);
            int indextodelete = m_selectShipsListbox.GetSelectedItem().Key;
            m_selectShipsListbox.RemoveItem(indextodelete);
            //m_selectShipsListbox.RemoveEmptyRows();
            m_bots.Remove(indextodelete);
        }

        private void OnEditClick(MyGuiControlButton sender)
        {
            var selected = m_selectShipsListbox.GetSelectedItem();
            if (selected != null)
            {
                BotTemplate template = m_bots[selected.Key];
                int? selectedIndex = null;
                var builders = GetTemplatesForCombobox(template.m_builder, out selectedIndex);
                Debug.Assert(selectedIndex != null, "This shouldn't happen!");
                if (selectedIndex == null) 
                {
                    selectedIndex = 0;
                }
                MyGuiScreenEditorSmallShip screen = new MyGuiScreenEditorSmallShip(template.m_builder, builders, selectedIndex.Value);
                screen.OnOk += delegate 
                {
                    if (template.m_builder.ShipTemplateID != null)
                    {
                        OnEditBot(template.m_builder, MySmallShipTemplates.GetTemplate(template.m_builder.ShipTemplateID.Value));
                    }
                    else
                    {
                        OnEditBot(template.m_builder);
                    }
                };

                MyGuiManager.AddScreen(screen);
            }
        }

        private List<MySmallShipBuilderWithName> GetTemplatesForCombobox() 
        {
            int? selectedIndex = null;
            return GetTemplatesForCombobox(null, out selectedIndex);
        }

        private List<MySmallShipBuilderWithName> GetTemplatesForCombobox(MyMwcObjectBuilder_SmallShip_Bot selectedBuilder, out int? selectedIndex) 
        {            
            int? foundedIndex = null;
            int currentIndex = 0;
            List<MySmallShipBuilderWithName> templatesForCombobox = new List<MySmallShipBuilderWithName>();            
            for (int i = 0; i < MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues.Length; i++)
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum shipType = (MyMwcObjectBuilder_SmallShip_TypesEnum)MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues.GetValue(i);                
                StringBuilder templateName;
                // insert no teplate       
                templateName = GetTemplatePrefix(shipType);                
                templateName.Append("NO TEMPLATE");
                MyMwcObjectBuilder_SmallShip_Bot builderToAdd = null;
                if (selectedBuilder != null && selectedBuilder.ShipTemplateID == null && selectedBuilder.ShipType == shipType) 
                {
                    builderToAdd = selectedBuilder;
                    foundedIndex = currentIndex;
                }
                else
                {
                    builderToAdd = MyMwcObjectBuilder_SmallShip_Bot.CreateObjectBuilderWithAllItems(shipType, MyShipTypeConstants.GetShipTypeProperties(shipType).GamePlay.CargoCapacity);
                    if (selectedBuilder != null) 
                    {
                        builderToAdd.CopyBotParameters(selectedBuilder);
                    }
                }
                templatesForCombobox.Add(new MySmallShipBuilderWithName(templateName, builderToAdd));
                currentIndex++;

                // real templates
                foreach (MySmallShipTemplate template in MySmallShipTemplates.GetTemplatesForType(shipType)) 
                {
                    if (selectedBuilder != null && selectedBuilder.ShipTemplateID != null && selectedBuilder.ShipTemplateID.Value == template.ID) 
                    {
                        foundedIndex = currentIndex;
                    }
                    builderToAdd = new MyMwcObjectBuilder_SmallShip_Bot(template.Builder);
                    if (selectedBuilder != null)
                    {
                        builderToAdd.CopyBotParameters(selectedBuilder);
                    }
                    templateName = GetTemplatePrefix(shipType);
                    MyMwcUtils.AppendStringBuilder(templateName, template.Name);
                    templatesForCombobox.Add(new MySmallShipBuilderWithName(templateName, builderToAdd, template));
                    currentIndex++;
                }
            }
            selectedIndex = foundedIndex;
            return templatesForCombobox;
        }

        private StringBuilder GetTemplatePrefix(MyMwcObjectBuilder_SmallShip_TypesEnum shipType) 
        {
            StringBuilder prefix = new StringBuilder();
            MyMwcUtils.AppendStringBuilder(prefix, ((MyGuiSmallShipHelperSmallShip)MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)shipType)).Name);
            prefix.Append(" - ");
            return prefix;
        }

        private void OnInventoryClick(MyGuiControlButton sender)
        {
            var selected = m_selectShipsListbox.GetSelectedItem();
            if (selected != null)
            {
                BotTemplate template = m_bots[selected.Key];
                int? selectedIndex = null;
                var builders = GetTemplatesForCombobox(template.m_builder, out selectedIndex);
                Debug.Assert(selectedIndex != null, "This shouldn't happen!");
                if (selectedIndex == null) 
                {
                    selectedIndex = 0;
                }

                MyInventory inventory = new MyInventory();
                inventory.FillInventoryWithAllItems(null, 100);

                MyGuiScreenInventory inventoryScreen = new MyGuiScreenInventory(builders, selectedIndex.Value, inventory.GetObjectBuilder(false), null, true);
                inventoryScreen.OnSave += OnEditBotFromScreen;                
                MyGuiManager.AddScreen(inventoryScreen);
            }
        }        

        private void OnAddClick(MyGuiControlButton sender)
        {
            //List<MySmallShipBuilderWithName> builders = new List<MySmallShipBuilderWithName>();            
            //for (int i = 0; i < MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues.Length; i++)
            //{
            //    builders.Add(new MySmallShipBuilderWithName(MyMwcObjectBuilder_SmallShip_Bot.CreateObjectBuilderWithAllItems(
            //        (MyMwcObjectBuilder_SmallShip_TypesEnum)MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues.GetValue(i))));
            //}            
            List<MySmallShipBuilderWithName> builders = GetTemplatesForCombobox();
            MyInventory inventory = new MyInventory();
            inventory.FillInventoryWithAllItems(null, 100);
            MyGuiScreenInventory inventoryScreen = new MyGuiScreenInventory(builders, 0, inventory.GetObjectBuilder(false), MyTextsWrapper.Get(MyTextsWrapperEnum.AllItemsInventory), true);
            inventoryScreen.OnSave += OnAddBotFromScreen;            
            MyGuiManager.AddScreen(inventoryScreen);
        }
        
        void OnComponentChange(MyGuiControlSlider sender)
        {
            m_radiusLabel.UpdateText(string.Format("{0:#,###0} " + MyTextsWrapper.Get(MyTextsWrapperEnum.MetersLong).ToString(), m_radiusSlider.GetValue()));
            m_firstSpawnLabel.UpdateText(string.Format("{0:#,###0} " + MyTextsWrapper.Get(MyTextsWrapperEnum.SecondsLong).ToString(), m_firstSpawnTimeSlider.GetValue() / 1000));
            m_respawnLabel.UpdateText(string.Format("{0:#,###0} " + MyTextsWrapper.Get(MyTextsWrapperEnum.SecondsLong).ToString(), m_respawnTimeSlider.GetValue() / 1000));   
        }

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);

            Debug.Assert(m_radiusSlider.GetValue() > 30 || m_bots.Count <= 1, "Spawnpoint radius is too small, you will probably get failed spawn attempts!");
            
            if (!HasEntity())
            {
                MyMwcObjectBuilder_SpawnPoint builder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.SpawnPoint, null) as MyMwcObjectBuilder_SpawnPoint;
                builder.BoundingRadius = m_radiusSlider.GetValue();

                float cameraDistance = builder.BoundingRadius / (float)Math.Sin(MathHelper.ToRadians(MyCamera.FieldOfViewAngle / 2)) * 1.2f;

                m_spawnPoint = MyEntities.CreateFromObjectBuilderAndAdd(null, builder, Matrix.CreateWorld(MyCamera.Position + cameraDistance * MyCamera.ForwardVector, Vector3.Forward, Vector3.Up)) as MySpawnPoint;
            }

            MyMwcObjectBuilder_FactionEnum shipFaction = (MyMwcObjectBuilder_FactionEnum)
                Enum.ToObject(typeof(MyMwcObjectBuilder_FactionEnum), m_selectShipFactionCombobox.GetSelectedKey()); 

            List<BotTemplate> templates = new List<BotTemplate>();
            foreach (int key in m_bots.Keys)
            {
                BotTemplate btmp;
                m_bots.TryGetValue(key, out btmp);
                btmp.m_builder.Faction = shipFaction;
                templates.Add(btmp);
            }

            m_spawnPoint.SpawnInGroups = m_spawnInGroupsCheckbox.Checked;
            m_spawnPoint.LeftToSpawn = GetSpawnCount();
            m_spawnPoint.MaxSpawnCount = m_spawnPoint.LeftToSpawn;
            m_spawnPoint.FirstSpawnTimer = m_firstSpawnTimeSlider.GetValue();
            m_spawnPoint.RespawnTimer = m_respawnTimeSlider.GetValue();

            m_spawnPoint.Faction = shipFaction;
            m_spawnPoint.SetWayPointPath(m_waypointPathCombobox.GetSelectedValue().ToString());
            m_spawnPoint.PatrolMode = (MyPatrolMode)m_patrolModeCombobox.GetSelectedKey();
            m_spawnPoint.ApplyBotTemplates(templates);

            m_spawnPoint.BoundingSphereRadius = m_radiusSlider.GetValue();
            if (m_activeCheckbox.Checked && !m_spawnPoint.IsActive())
            {
                m_spawnPoint.Activate();
            }
            else if (!m_activeCheckbox.Checked && m_spawnPoint.IsActive())
            {
                m_spawnPoint.Deactivate();
            }
            
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
        }

        protected override Vector2 GetControlsOriginLeftFromScreenSize()
        {
            return base.GetControlsOriginLeftFromScreenSize() + new Vector2(0.002f, 0);
        }
    }
}
