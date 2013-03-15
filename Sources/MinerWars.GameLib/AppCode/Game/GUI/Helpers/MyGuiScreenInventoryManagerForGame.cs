using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using System.Diagnostics;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.Entities.Ships.AI;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    static class MyGuiScreenInventoryManagerForGame
    {
        public delegate void OpenInventoryHandler(MyEntity entity, MySmallShipInteractionActionEnum interactionAction);
        public delegate void CloseInventoryHandler(MyGuiScreenBase screen);

        public static event OpenInventoryHandler OpeningInventoryScreen;
        public static event CloseInventoryHandler InventoryScreenClosed;

        private static MyGuiScreenInventory m_currentInventoryScreen;        
        private static MyEntity m_detectedEntity;
        private static MySmallShipInteractionActionEnum? m_detectedAction;
        private static MyEntityDetector m_entityDetector;               
        private static MyPlayer m_player;
        private static IMyInventory m_shipsInventoryOwner;
        private static IMyInventory m_otherSideInventoryOwner;
        private static MyGuiScreenInventoryType m_inventoryScreenType;
        private static bool m_tradeForMoney;
        private static int m_curentIndex;

        public static MyGuiScreenInventory OpenInventory(MyGuiScreenInventoryType inventoryScreenType, MyEntity otherSide = null, MySmallShipInteractionActionEnum? action = null)
        {
            MyMwcLog.WriteLine("OpenInventory()");


            // return harvester when opening inventory, harvester can update inventory, which would not propagete to inventory screen and closing inventory would override those changes
            var harvester = MySession.PlayerShip.Weapons.GetMountedHarvestingDevice();
            if (harvester != null && harvester.CurrentState != MyHarvestingDeviceEnum.InsideShip)
                harvester.Shot(null);

            StringBuilder otherSideInventoryName = null;
            MyInventory otherSideInventory = null;
            List<MySmallShipBuilderWithName> shipsObjectBuilders = null;
            bool closeOtherSideInventory = false;
            
            m_inventoryScreenType = inventoryScreenType;
            m_shipsInventoryOwner = MySession.Static;
            m_player = MySession.Static.Player;
            m_detectedEntity = otherSide;
            m_detectedAction = action;
            shipsObjectBuilders = new List<MySmallShipBuilderWithName>();
            shipsObjectBuilders.Add(new MySmallShipBuilderWithName(MySession.PlayerShip.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Player));

            switch(m_inventoryScreenType)
            {            
                case MyGuiScreenInventoryType.GodEditor:
                    LoadGodEditorInventory(ref otherSideInventory, ref otherSideInventoryName, ref shipsObjectBuilders);
                    closeOtherSideInventory = true;
                    break;
                case MyGuiScreenInventoryType.InGameEditor:
                    LoadIngameEditorInventory(ref otherSideInventory, ref otherSideInventoryName);
                    break;
                case MyGuiScreenInventoryType.Game:
                    LoadGameInventory(ref otherSideInventory, ref otherSideInventoryName, ref shipsObjectBuilders);
                    break;
            }
            
            var currentShipBuilder = shipsObjectBuilders[0];            
            shipsObjectBuilders.Sort((x, y) => ((int)x.Builder.ShipType).CompareTo((int)y.Builder.ShipType));
            m_curentIndex = shipsObjectBuilders.IndexOf(currentShipBuilder);
            MyMwcObjectBuilder_Inventory otherSideInventoryBuilder = otherSideInventory != null ? otherSideInventory.GetObjectBuilder(true) : null;
            if (closeOtherSideInventory) 
            {
                Debug.Assert(otherSideInventory != null);
                otherSideInventory.Close();
            }
            m_currentInventoryScreen = new MyGuiScreenInventory(shipsObjectBuilders, m_curentIndex,
                                                                      m_player.Money, m_tradeForMoney,
                                                                      otherSideInventoryBuilder, otherSideInventoryName,
                                                                      m_inventoryScreenType);
            m_currentInventoryScreen.OnSave += Save;
            m_currentInventoryScreen.Closed += OnInventoryScreenClosed;
            MyGuiScreenGamePlay.Static.HideSelectAmmo();
            return m_currentInventoryScreen;
        }

        private static void LoadGodEditorInventory(ref MyInventory otherSideInventory, ref StringBuilder otherSideInventoryName, ref List<MySmallShipBuilderWithName> shipsObjectBuilders) 
        {
            otherSideInventory = new MyInventory();
            otherSideInventory.FillInventoryWithAllItems(null, 100, true);
            otherSideInventoryName = MyTextsWrapper.Get(MyTextsWrapperEnum.AllItemsInventory);
            foreach (MyMwcObjectBuilder_SmallShip_TypesEnum shipType in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_TypesEnum)))
            {
                if (MySession.PlayerShip.ShipType != shipType)
                {
                    MyMwcObjectBuilder_SmallShip_Player playerShipBuilder = MyMwcObjectBuilder_SmallShip_Player.CreateObjectBuilderWithAllItems(shipType, MySession.PlayerShip.Faction, MyShipTypeConstants.GetShipTypeProperties(shipType).GamePlay.CargoCapacity);
                    shipsObjectBuilders.Add(new MySmallShipBuilderWithName(playerShipBuilder));
                }
            }
        }

        private static void LoadIngameEditorInventory(ref MyInventory otherSideInventory, ref StringBuilder otherSideInventoryName) 
        {
            m_otherSideInventoryOwner = MyEditor.Static.FoundationFactory.PrefabContainer;

            otherSideInventory = m_otherSideInventoryOwner.Inventory;
            otherSideInventoryName = new StringBuilder();
            otherSideInventoryName.Append(MyEditor.Static.FoundationFactory.PrefabContainer.DisplayName);
            //otherSideInventoryName.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.OtherSideInventory)); 
        }

        private static void LoadGameInventory(ref MyInventory otherSideInventory, ref StringBuilder otherSideInventoryName, ref List<MySmallShipBuilderWithName> shipsObjectBuilders) 
        {
            if (MySession.PlayerShip.TradeDetector != null && m_detectedEntity == null)
            {
                m_detectedEntity = MySession.PlayerShip.TradeDetector.GetNearestEntity();
            }            

            if (m_detectedEntity != null)
            {
                if (m_detectedAction == null)
                {
                    m_detectedAction = (MySmallShipInteractionActionEnum)MySession.PlayerShip.TradeDetector.GetNearestEntityCriterias();
                }                
                m_otherSideInventoryOwner = m_detectedEntity as IMyInventory;

                otherSideInventory = m_otherSideInventoryOwner.Inventory;
                string entityName;
                if (m_detectedEntity is MyPrefabHangar)
                {
                    entityName = ((m_detectedEntity) as MyPrefabHangar).GetOwner().GetCorrectDisplayName();
                }
                else
                {
                    entityName = m_detectedEntity.GetCorrectDisplayName();
                }

                switch (m_detectedAction)
                {
                    case MySmallShipInteractionActionEnum.TradeForFree:
                        if (IsTradingWithMothership())
                        {
                            List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
                            foreach (MyInventoryItem inventoryItem in m_shipsInventoryOwner.Inventory.GetInventoryItems())
                            {
                                if (inventoryItem.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Player)
                                {
                                    shipsObjectBuilders.Add(new MySmallShipBuilderWithName(inventoryItem.GetInventoryItemObjectBuilder(false) as MyMwcObjectBuilder_SmallShip_Player));
                                }
                                else if (inventoryItem.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool)
                                {
                                    inventoryItem.CanBeMoved = false;
                                }
                                else 
                                {
                                    continue;
                                }
                                inventoryItem.IsTemporaryItem = true;
                                inventoryItems.Add(inventoryItem);
                            }

                            //m_shipsInventoryOwner.Inventory.RemoveInventoryItems(inventoryItems);
                            //m_shipsInventoryOwner.Inventory.RemoveInventoryItems(MyMwcObjectBuilderTypeEnum.SmallShip_Player, null);
                            //m_shipsInventoryOwner.Inventory.ClearInventoryItems(false);
                            m_shipsInventoryOwner.Inventory.RemoveInventoryItems(inventoryItems, false);
                            otherSideInventory.AddInventoryItems(inventoryItems);
                        }
                        break;
                    case MySmallShipInteractionActionEnum.TradeForMoney:
                        m_tradeForMoney = true;
                        if (m_detectedEntity is MySmallShipBot) 
                        {
                            otherSideInventory.PriceCoeficient = 3f;
                        }
                        break;
                    case MySmallShipInteractionActionEnum.Blocked:
                    case MySmallShipInteractionActionEnum.Build:
                    case MySmallShipInteractionActionEnum.Loot:
                    case MySmallShipInteractionActionEnum.Examine:
                    case MySmallShipInteractionActionEnum.ExamineEmpty:
                        break;
                    default:
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                }

                otherSideInventoryName = new StringBuilder();
                if (string.IsNullOrEmpty(entityName))
                {
                    otherSideInventoryName.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.OtherSide));
                }
                else
                {
                    otherSideInventoryName.Append(entityName);
                    //otherSideInventoryName.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.OtherSideInventory));
                }

                if (OpeningInventoryScreen != null)
                {
                    OpeningInventoryScreen(m_detectedEntity, m_detectedAction.Value);
                }
            }
        }

        static void OnEntityLeave(MyEntityDetector sender, MyEntity entity)
        {
            if (entity == m_detectedEntity)
            {
                m_currentInventoryScreen.CancelTransfering();
            }
        }        

        private static void OnInventoryScreenClosed(MyGuiScreenBase screen)
        {
            Clear();

            if (InventoryScreenClosed != null)
            {
                InventoryScreenClosed(screen);
            }
        }

        private static bool IsTradingWithMothership() 
        {
            return m_inventoryScreenType == MyGuiScreenInventoryType.Game &&
                   m_detectedAction != null &&
                   m_detectedAction.Value == MySmallShipInteractionActionEnum.TradeForFree &&
                   m_detectedEntity is MyPrefabHangar &&
                   ((MyPrefabHangar)m_detectedEntity).PrefabHangarType == MyMwcObjectBuilder_PrefabHangar_TypesEnum.HANGAR;
        }

        private static void Save(MyGuiScreenInventory sender, MyGuiScreenInventorySaveResult saveResult)
        {
            // save money
            if (m_tradeForMoney)
            {
                m_player.Money = saveResult.Money;
            }

            bool isTradingWithMotherShip = IsTradingWithMothership();
                        
            List<MyInventoryItem> itemsToMothership = new List<MyInventoryItem>();

            // save ships            
            for (int i = 0; i < saveResult.SmallShipsObjectBuilders.Count; i++)
            {
                MyMwcObjectBuilder_SmallShip shipBuilder = saveResult.SmallShipsObjectBuilders[i].Builder;
                if (i == saveResult.CurrentIndex)
                {
                    if (m_curentIndex == saveResult.CurrentIndex)
                    {
                        // we want init weapons only when weapons are not same
                        if (!WeaponBuildersAreSame(m_player.Ship.Weapons.GetWeaponsObjectBuilders(true), shipBuilder.Weapons))
                        {
                            m_player.Ship.Weapons.Init(shipBuilder.Weapons, shipBuilder.AssignmentOfAmmo);
                        }
                        m_player.Ship.Inventory.Init(shipBuilder.Inventory);
                        m_player.Ship.Armor = shipBuilder.Armor;
                        m_player.Ship.Engine = shipBuilder.Engine;
                    }
                    else
                    {
                        var oldShip = m_player.Ship;
                        m_player.Ship.MarkForClose();

                        if (MyMultiplayerGameplay.IsRunning)
                        {
                            MyMultiplayerGameplay.Static.Respawn(shipBuilder, m_player.Ship.WorldMatrix);
                        }
                        else
                        {
                            var ship = MyEntities.CreateFromObjectBuilderAndAdd(null, shipBuilder, m_player.Ship.WorldMatrix);
                        }

                        // Update bots - bot logic runs on host
                        MyBotCoordinator.ChangeShip(oldShip, m_player.Ship);
                    }
                }
                else
                {
                    if (isTradingWithMotherShip)
                    {
                        MyInventoryItem shipInventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(shipBuilder);
                        itemsToMothership.Add(shipInventoryItem);
                        //m_shipsInventoryOwner.Inventory.AddInventoryItem(shipInventoryItem);
                    }
                }
            }            

            // save other side inventory
            if (m_inventoryScreenType != MyGuiScreenInventoryType.GodEditor)
            {
                if (m_otherSideInventoryOwner != null)
                {                    
                    if (isTradingWithMotherShip)
                    {
                        for (int i = saveResult.OtherSideInventoryObjectBuilder.InventoryItems.Count - 1; i >= 0; i--) 
                        {
                            MyMwcObjectBuilder_InventoryItem itemBuilder = saveResult.OtherSideInventoryObjectBuilder.InventoryItems[i];
                            if (itemBuilder.IsTemporaryItem) 
                            {                                
                                saveResult.OtherSideInventoryObjectBuilder.InventoryItems.RemoveAt(i);
                                // because smallships was added when ships were saved
                                if (itemBuilder.ItemObjectBuilder.GetObjectBuilderType() != MyMwcObjectBuilderTypeEnum.SmallShip_Player)
                                {
                                    itemsToMothership.Add(MyInventory.CreateInventoryItemFromInventoryItemObjectBuilder(itemBuilder));
                                }
                            }
                        }                        
                    }
                    m_otherSideInventoryOwner.Inventory.Init(saveResult.OtherSideInventoryObjectBuilder);
                }
            }

            if (isTradingWithMotherShip) 
            {
                m_shipsInventoryOwner.Inventory.AddInventoryItems(itemsToMothership);
            }
        }

        private static bool WeaponBuildersAreSame(List<MyMwcObjectBuilder_SmallShip_Weapon> weapons1, List<MyMwcObjectBuilder_SmallShip_Weapon> weapons2) 
        {
            if (weapons1.Count != weapons2.Count) 
            {
                return false;
            }

            foreach (var weapon1 in weapons1) 
            {
                if (!weapons2.Contains(weapon1)) 
                {
                    return false;
                }
            }

            foreach (var weapon2 in weapons2) 
            {
                if (!weapons1.Contains(weapon2)) 
                {
                    return false;
                }
            }

            return true;
        }

        private static void Clear()
        {                        
            m_otherSideInventoryOwner = null;
            m_player = null;
            m_shipsInventoryOwner = null;
            m_tradeForMoney = false;            
            m_entityDetector = null;
            m_detectedAction = null;
            m_currentInventoryScreen.OnSave -= Save;
            m_currentInventoryScreen.Closed -= OnInventoryScreenClosed;
            m_currentInventoryScreen = null;
        }

        public static bool IsInventoryOpen() 
        {
            return m_currentInventoryScreen != null &&
                   m_currentInventoryScreen.IsInventoryLocked();
        }

        public static bool IsOtherSideInventory(MyInventory inventory) 
        {
            return m_otherSideInventoryOwner != null && m_otherSideInventoryOwner.Inventory == inventory;
        }
    }
}
