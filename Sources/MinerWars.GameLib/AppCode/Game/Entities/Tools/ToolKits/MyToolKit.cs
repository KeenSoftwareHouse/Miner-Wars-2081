using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Managers.Session;
using System.Linq.Expressions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Entities.Tools.ToolKits
{    
    class MyToolKit
    {
        delegate float GetAmountDelegate(MySmallShip smallShipOwner, MyPlayer playerOwner);
        delegate void AddAmountDelegate(MySmallShip smallShipOwner, MyPlayer playerOwner, float amount);    
    
        class MyToolKitDelegates
        {
            public GetAmountDelegate AmountDelegate { get; set; }
            public GetAmountDelegate MaxAmountDelegate { get; set; }
            public AddAmountDelegate AddAmountDelegate { get; set; }

            public MyToolKitDelegates(GetAmountDelegate amountDelegate, GetAmountDelegate maxAmountDelegate, AddAmountDelegate addAmountDelegate)
            {
                AmountDelegate = amountDelegate;
                MaxAmountDelegate = maxAmountDelegate;
                AddAmountDelegate = addAmountDelegate;
            }
        }

        #region Predefined delegates
        // Player health
        private static GetAmountDelegate GetPlayerHealth = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return playerOwner.Health; };
        private static GetAmountDelegate GetPlayerMaxHealth = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return playerOwner.MaxHealth; };
        private static AddAmountDelegate AddPlayerHealth = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner, float amount) { playerOwner.AddHealth(amount, null); };
        private static MyToolKitDelegates PlayerHealthDelegates = new MyToolKitDelegates(GetPlayerHealth, GetPlayerMaxHealth, AddPlayerHealth);

        // Ship health
        private static GetAmountDelegate GetShipHealth = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return smallShipOwner.Health; };
        private static GetAmountDelegate GetShipMaxHealth = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return smallShipOwner.MaxHealth; };
        private static AddAmountDelegate AddShipHealth = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner, float amount) { smallShipOwner.Health += amount; };
        private static MyToolKitDelegates ShipHealthDelegates = new MyToolKitDelegates(GetShipHealth, GetShipMaxHealth, AddShipHealth);

        // Ship armor
        private static GetAmountDelegate GetShipArmor = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return smallShipOwner.ArmorHealth; };
        private static GetAmountDelegate GetShipMaxArmor = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return smallShipOwner.MaxArmorHealth; };
        private static AddAmountDelegate AddShipArmor = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner, float amount) { smallShipOwner.ArmorHealth += amount; };
        private static MyToolKitDelegates ShipArmorDelegates = new MyToolKitDelegates(GetShipArmor, GetShipMaxArmor, AddShipArmor);

        // Ship oxygen
        private static GetAmountDelegate GetShipOxygen = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return smallShipOwner.Oxygen; };
        private static GetAmountDelegate GetShipMaxOxygen = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return smallShipOwner.MaxOxygen; };
        private static AddAmountDelegate AddShipOxygen = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner, float amount) { smallShipOwner.Oxygen += amount; };
        private static MyToolKitDelegates ShipOxygenDelegates = new MyToolKitDelegates(GetShipOxygen, GetShipMaxOxygen, AddShipOxygen);

        // Ship fuel
        private static GetAmountDelegate GetShipFuel = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return smallShipOwner.Fuel; };
        private static GetAmountDelegate GetShipMaxFuel = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner) { return smallShipOwner.MaxFuel; };
        private static AddAmountDelegate AddShipFuel = delegate(MySmallShip smallShipOwner, MyPlayer playerOwner, float amount) { smallShipOwner.Fuel += amount; };
        private static MyToolKitDelegates ShipFuelDelegates = new MyToolKitDelegates(GetShipFuel, GetShipMaxFuel, AddShipFuel);
        #endregion        

        #region delegates for tool kits
        private static Dictionary<int, MyToolKitDelegates[]> m_delegatesPerToolKitType = new Dictionary<int, MyToolKitDelegates[]>()
        {
            { (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_KIT, new MyToolKitDelegates[]{ PlayerHealthDelegates} },
            { (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REPAIR_KIT, new MyToolKitDelegates[]{ ShipHealthDelegates, ShipArmorDelegates} },
            { (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_KIT, new MyToolKitDelegates[]{ ShipOxygenDelegates} },
            { (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_KIT, new MyToolKitDelegates[]{ ShipFuelDelegates} },
        };
        #endregion

        private MySmallShip m_smallShipOwner;
        private MyPlayer m_playerOwner;
        private MyInventoryItem m_toolInventoryItem;
        private MyToolKitDelegates[] m_delegates;

        private MyToolKit(MySmallShip smallShipOwner, MyPlayer playerOwner, MyInventoryItem toolInventoryItem, MyToolKitDelegates[] delegates) 
        {
            m_smallShipOwner = smallShipOwner;
            m_playerOwner = playerOwner;
            m_toolInventoryItem = toolInventoryItem;
            m_delegates = delegates;
        }

        public void Update() 
        {
            float maxAmountPercentageAdded = 0f;
            foreach (MyToolKitDelegates delegates in m_delegates) 
            {
                float maxAmount = delegates.MaxAmountDelegate(m_smallShipOwner, m_playerOwner);                
                float amount = delegates.AmountDelegate(m_smallShipOwner, m_playerOwner);
                if (amount < maxAmount) 
                {
                    float amountPercentageToAdd = 1f - amount / maxAmount;
                    amountPercentageToAdd = Math.Min(m_toolInventoryItem.Amount, amountPercentageToAdd);
                    delegates.AddAmountDelegate(m_smallShipOwner, m_playerOwner, maxAmount * amountPercentageToAdd);
                    maxAmountPercentageAdded = Math.Max(amountPercentageToAdd, maxAmountPercentageAdded);
                }
            }
            if (maxAmountPercentageAdded > 0f)
            {
                m_toolInventoryItem.Amount -= maxAmountPercentageAdded;
            }
        }

        public bool IsEmpty() 
        {
            return m_toolInventoryItem.Amount <= 0f;
        }

        public MyInventoryItem GetToolInventoryItem() 
        {
            return m_toolInventoryItem;
        }

        public static bool IsSupportedToolKitItem(MyInventoryItem toolInventoryItem) 
        {
            return toolInventoryItem != null &&
                   toolInventoryItem.Amount > 0f &&
                   toolInventoryItem.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Tool &&
                   toolInventoryItem.ObjectBuilderId != null &&
                   m_delegatesPerToolKitType.ContainsKey(toolInventoryItem.ObjectBuilderId.Value);
        }

        public static MyToolKit CreateInstance(MySmallShip smallShipOwner, MyPlayer playerOwner, MyInventoryItem toolInventoryItem) 
        {
            Debug.Assert(smallShipOwner != null);
            Debug.Assert(playerOwner != null);
            Debug.Assert(IsSupportedToolKitItem(toolInventoryItem));

            MyToolKitDelegates[] delegates = m_delegatesPerToolKitType[toolInventoryItem.ObjectBuilderId.Value];
            Debug.Assert(delegates.Length > 0);
            return new MyToolKit(smallShipOwner, playerOwner, toolInventoryItem, delegates);
        }
    }    
}
