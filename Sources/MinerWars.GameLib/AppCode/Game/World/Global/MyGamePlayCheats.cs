using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.LargeShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using SysUtils.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Entities.SubObjects;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Gameplay
{
    [Flags]
    internal enum MyGameplayCheatsEnum
    {
        PLAYER_SHIP_INDESTRUCTIBLE =    1 << 0,
        PLAYER_HEALTH_INDESTRUCTIBLE =  1 << 1,
        INFINITE_AMMO =                 1 << 2,
        INFINITE_FUEL =                 1 << 3,
        INFINITE_ELECTRICITY =          1 << 4,
        ALL_WEAPONS =                   1 << 5,
        ALL_BLUEPRINTS =                1 << 6,
        KILL_ALL =                      1 << 7,
        SEE_ALL =                       1 << 8,
        ENEMY_CANT_DIE =                1 << 9,
        FRIEND_NEUTRAL_CANT_DIE =       1 << 10,
        //ALL_CHEATS_TO_COOP =            1 << 11,
        EXTRA_MONEY =                   1 << 12,
        INSTANT_BUILDING =              1 << 13,
        INFINITE_OXYGEN =               1 << 14,
        REMOVE_ALL_BLUEPRINTS =         1 << 15,
        CLEAR_INVENTORY =               1 << 16,
        FF_IN_INVENTORY =               1 << 17,
        UNLIMITED_TRADING =             1 << 18,
        INCREASE_CARGO_CAPACITY =       1 << 19,
    }

    internal struct MyGameplayCheat
    {
        public delegate void CheatEnabledDelegate(MyGameplayCheat cheat);

        public CheatEnabledDelegate OnCheatEnabled;
        public CheatEnabledDelegate OnCheatDisabled;
        public MyGameplayCheatsEnum CheatEnum;
        public MyTextsWrapperEnum CheatName;
        public bool IsButton;
        public bool IsImplemented;
    }

    static class MyGameplayCheats
    {
        static MyGameplayCheatsEnum m_enabledCheats;

        internal static readonly List<MyGameplayCheat> AllCheats = new List<MyGameplayCheat>()
        {
            new MyGameplayCheat() 
            {
                CheatEnum = MyGameplayCheatsEnum.PLAYER_SHIP_INDESTRUCTIBLE,
                CheatName = MyTextsWrapperEnum.PlayerShipIndestructible,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat() 
            {
                CheatEnum = MyGameplayCheatsEnum.PLAYER_HEALTH_INDESTRUCTIBLE,
                CheatName = MyTextsWrapperEnum.PlayerHealthIndestructible,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat() 
            {
                CheatEnum = MyGameplayCheatsEnum.INFINITE_AMMO,
                CheatName = MyTextsWrapperEnum.InfiniteAmmo,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat() 
            {
                CheatEnum = MyGameplayCheatsEnum.INFINITE_FUEL,
                CheatName = MyTextsWrapperEnum.InfiniteFuel,
                IsButton = false,
                IsImplemented = true,
            },
            //new MyGameplayCheat() 
            //{
            //    CheatEnum = MyGameplayCheatsEnum.INFINITE_ELECTRICITY,
            //    CheatName = MyTextsWrapperEnum.InfiniteElectricity,
            //    IsButton = false,
            //    IsImplemented = true,
            //},      
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.SEE_ALL,
                CheatName = MyTextsWrapperEnum.SeeAll,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.ENEMY_CANT_DIE,
                CheatName = MyTextsWrapperEnum.EnemyCantDie,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.FRIEND_NEUTRAL_CANT_DIE,
                CheatName = MyTextsWrapperEnum.FriendNeutralCantDie,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat() 
            {
                CheatEnum = MyGameplayCheatsEnum.INFINITE_OXYGEN,
                CheatName = MyTextsWrapperEnum.InfiniteOxygen,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.INSTANT_BUILDING,
                CheatName = MyTextsWrapperEnum.InstantBuilding,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat() 
            {
                CheatEnum = MyGameplayCheatsEnum.UNLIMITED_TRADING,
                CheatName = MyTextsWrapperEnum.UnlimitedTrading,
                IsButton = false,
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.INCREASE_CARGO_CAPACITY,
                CheatName = MyTextsWrapperEnum.IncreaseCargoCapacity,
                IsButton = false,
                IsImplemented = true,
                OnCheatEnabled = IncreaseCargoCapacityEnabled,
                OnCheatDisabled = IncreaseCargoCapacityDisabled,
            },
            new MyGameplayCheat() 
            {
                CheatEnum = MyGameplayCheatsEnum.ALL_WEAPONS,
                CheatName = MyTextsWrapperEnum.AllWeapons,
                IsButton = true,
                OnCheatEnabled = AllWeaponsEnabled, 
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.ALL_BLUEPRINTS,
                CheatName = MyTextsWrapperEnum.AllBlueprints,
                IsButton = true,
                OnCheatEnabled = AllBluePrintsEnabled,
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.REMOVE_ALL_BLUEPRINTS,
                CheatName = MyTextsWrapperEnum.RemoveAllBlueprints,
                IsButton = true,
                OnCheatEnabled = RemoveAllBluePrintsEnabled,
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.KILL_ALL,
                CheatName = MyTextsWrapperEnum.KillAll,
                IsButton = true,
                OnCheatEnabled = KillAllEnabled, 
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.EXTRA_MONEY,
                CheatName = MyTextsWrapperEnum.ExtraMoney,
                IsButton = true,
                OnCheatEnabled = ExtraMoneyEnabled,
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.CLEAR_INVENTORY,
                CheatName = MyTextsWrapperEnum.ClearInventory,
                IsButton = true,
                OnCheatEnabled = ClearInventoryEnabled,
                IsImplemented = true,
            },
            new MyGameplayCheat()
            {
                CheatEnum = MyGameplayCheatsEnum.FF_IN_INVENTORY,
                CheatName = MyTextsWrapperEnum.FoundationFactoryInInventory,
                IsButton = true,
                OnCheatEnabled = FFInInventoryEnabled,
                IsImplemented = true,
            }
        };

        public static bool IsCheatEnabled(MyGameplayCheatsEnum cheat)
        {      
            return (m_enabledCheats & cheat) != 0;
        }

        public static void EnableCheat(MyGameplayCheatsEnum cheat, bool enable)
        {
            MyGameplayCheat? foundedCheatItem = GetCheat(cheat);
            Debug.Assert(foundedCheatItem != null);
            MyGameplayCheat cheatItem = foundedCheatItem.Value;

            if (enable)
            {
                m_enabledCheats |= cheat;
                
                if (cheatItem.OnCheatEnabled != null)
                    cheatItem.OnCheatEnabled(cheatItem);
            }
            else
            {
                m_enabledCheats &= ~cheat;

                if (cheatItem.OnCheatDisabled != null)
                    cheatItem.OnCheatDisabled(cheatItem);
            }
        }

        private static MyGameplayCheat? GetCheat(MyGameplayCheatsEnum cheat) 
        {
            foreach (MyGameplayCheat cheatItem in AllCheats)
            {
                if ((cheat & cheatItem.CheatEnum) != 0)
                {
                    return cheatItem;
                }
            }
            return null;
        }


        public static void LoadData()
        {
            MyGuiScreenGamePlay.OnGameLoaded += new EventHandler(MyGuiScreenGamePlay_OnGameLoaded);
        }

        public static void UnloadData()
        {
            MyGuiScreenGamePlay.OnGameLoaded -= new EventHandler(MyGuiScreenGamePlay_OnGameLoaded);
        }

        static void MyGuiScreenGamePlay_OnGameLoaded(object sender, EventArgs e)
        {
            if (MyFakes.DEFAULT_CHEATS != null)
            {
                EnableCheat(MyFakes.DEFAULT_CHEATS.Value, true);
            }
        }

        /// <summary>
        /// Enable increase cargo capacity cheat implementation
        /// </summary>
        /// <param name="cheat"></param>
        static void IncreaseCargoCapacityEnabled(MyGameplayCheat cheat) 
        {
            if (MySession.PlayerShip == null)
                return;

            //MySession.PlayerShip.Inventory.UnlimitedCapacity = true;
            MySession.PlayerShip.Inventory.MaxItems = MyGamePlayCheatsConstants.CHEAT_INCREASE_CARGO_CAPACITY_MAX_ITEMS;
        }

        /// <summary>
        /// Disabled increase cargo capacity cheat implementation
        /// </summary>
        /// <param name="cheat"></param>
        static void IncreaseCargoCapacityDisabled(MyGameplayCheat cheat) 
        {
            if (MySession.PlayerShip == null)
                return;

            MySession.PlayerShip.Inventory.MaxItems = MySession.PlayerShip.ShipTypeProperties.GamePlay.CargoCapacity;
            //MySession.PlayerShip.Inventory.UnlimitedCapacity = false;            
        }

        /// <summary>
        /// All weapons cheat implementation
        /// </summary>
        static void AllWeaponsEnabled(MyGameplayCheat cheat)
        {
            if (MySession.PlayerShip == null)
                return;

            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            List<MyMwcObjectBuilder_AssignmentOfAmmo> ammoAssignment = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();
            List<MyMwcObjectBuilder_InventoryItem> inventoryItems = new List<MyMwcObjectBuilder_InventoryItem>();

            // weapons
            foreach (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weapon in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum)))
            {
                weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(weapon));
                // we want have 2x autocanon
                if (weapon == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon)
                {
                    weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(weapon));
                }
            }

            // ammo assignment
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Third, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fourth, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fifth, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));



            MySession.PlayerShip.Weapons.Init(weapons, ammoAssignment);
            
            foreach (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)))
            {
                MyMwcObjectBuilder_InventoryItem item = new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(ammo), MyGameplayConstants.GetGameplayProperties(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)ammo, MySession.PlayerShip.Faction).MaxAmount);
                if(!MySession.PlayerShip.Inventory.IsFull)
                    MySession.PlayerShip.Inventory.AddInventoryItem(MyInventory.CreateInventoryItemFromInventoryItemObjectBuilder(item));
            }

        }

        /// <summary>
        /// Extra money cheat implementation
        /// </summary>
        static void ExtraMoneyEnabled(MyGameplayCheat cheat)
        {
            if (MySession.Static != null && MySession.Static.Player != null)
            {
                MySession.Static.Player.Money += 1000000;
            }
        }

        /// <summary>
        /// All blueprints cheat implementation
        /// </summary>
        static void AllBluePrintsEnabled(MyGameplayCheat cheat)
        {
            if (MyGuiScreenGamePlay.Static != null)
            {
                MyGuiScreenGamePlay.Static.AddAllBlueprints();
            }
        }

        /// <summary>
        /// Remove all blueprints cheat implementation
        /// </summary>
        static void RemoveAllBluePrintsEnabled(MyGameplayCheat cheat)
        {
            if (MyGuiScreenGamePlay.Static != null)
            {
                MyGuiScreenGamePlay.Static.RemoveAllBlueprints();
            }
        }

        /// <summary>
        /// Kill all cheat implementation
        /// </summary>
        static void KillAllEnabled(MyGameplayCheat cheat)
        {
            // We need player ship to recognize friends
            if (MySession.PlayerShip == null)
            {
                return;
            }

            foreach (var entity in MyEntities.GetEntities().ToArray())
            {
                MySmallShip smallShip = entity as MySmallShip;
                if (smallShip != null &&
                    smallShip.Visible &&
                    smallShip != MySession.PlayerShip &&
                    MyFactions.GetFactionsRelation(MySession.PlayerShip, smallShip) == MyFactionRelationEnum.Enemy)
                {
                    entity.DoDamage(0, 1000000, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
                }

                MyPrefabContainer container = entity as MyPrefabContainer;
                if (container != null &&
                    MyFactions.GetFactionsRelation(MySession.PlayerShip, container) == MyFactionRelationEnum.Enemy)
                {
                    foreach (var prefab in container.GetPrefabs().ToArray())
                    {
                        MyPrefabLargeWeapon largeWeapon = prefab as MyPrefabLargeWeapon;
                        if (largeWeapon != null)
                        {
                            prefab.DoDamage(0, 1000000, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear inventory cheat enabled
        /// </summary>
        static void ClearInventoryEnabled(MyGameplayCheat cheat)
        {
            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Inventory.ClearInventoryItems(true);
            }
        }

        /// <summary>
        /// Clear inventory cheat enabled
        /// </summary>
        static void FFInInventoryEnabled(MyGameplayCheat cheat)
        {
            if (MySession.PlayerShip != null)
            {
                if (!MySession.PlayerShip.Inventory.Contains(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT))
                {
                    MySession.PlayerShip.Inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT, 1.0f, true);
                }
            }
        }

        
    }
}
