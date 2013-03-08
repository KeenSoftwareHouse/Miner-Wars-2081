using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

namespace MinerWars.AppCode.Game.Missions
{
    static class MyActorProgress
    {
        public static void SetupActors(MyMissionID missionID)
        {
            int missionNumber = MyMissions.GetMissionNumber(missionID);
            if (missionNumber < 0)
            {
                return;
            }

            MyEntity entity;
            if (MyEntities.TryGetEntityByName("Marcus", out entity))
            {
                var marcus = entity as MySmallShipBot;
                SetupMarcus(missionNumber, marcus);
            }

            if (MyEntities.TryGetEntityByName("RavenGirl", out entity))
            {
                var tarja = entity as MySmallShipBot;
                SetupTarja(missionNumber, tarja);
            }

            if (MyEntities.TryGetEntityByName("RavenGuy", out entity))
            {
                var valentin = entity as MySmallShipBot;
                SetupValentin(missionNumber, valentin);
            }
        }

        private static void SetupMarcus(int missionNumber, MySmallShipBot marcus)
        {
            if (marcus != null)
            {
                marcus.Engine = new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_1);
                if (missionNumber <= 1)
                {
                    marcus.SetEquip(new List<MyMwcObjectBuilder_SmallShip_Weapon>
                        {
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                        },
                    new List<MyMwcObjectBuilder_InventoryItem>
                        {
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic), 100),
                        });
                }
                else if (missionNumber == 2)
                {
                    marcus.SetEquip(new List<MyMwcObjectBuilder_SmallShip_Weapon>
                        {
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun),
                        },
                    new List<MyMwcObjectBuilder_InventoryItem>
                        {
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic), 100),
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic), 100),
                        });
                }
                else if (missionNumber == 3)
                {
                    marcus.SetEquip(new List<MyMwcObjectBuilder_SmallShip_Weapon>
                        {
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher),
                        },
                    new List<MyMwcObjectBuilder_InventoryItem>
                        {
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic), 100),
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic), 100),
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic), 10),
                        });
                }
                else if (missionNumber >= 4)
                {
                    marcus.SetEquip(new List<MyMwcObjectBuilder_SmallShip_Weapon>
                        {
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher),
                            new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper),
                        },
                    new List<MyMwcObjectBuilder_InventoryItem>
                        {
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic), 100),
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic), 100),
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection), 10),
                            new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed), 10),
                        });
                }
            }
        }

        private static void SetupTarja(int missionNumber, MySmallShipBot tarja)
        {
            if (tarja != null)
            {
                tarja.Engine = new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_1);
                tarja.SetEquip(new List<MyMwcObjectBuilder_SmallShip_Weapon>
                    {
                        new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon),
                        new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon),
                        new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher),
                    },
                    new List<MyMwcObjectBuilder_InventoryItem>
                    {
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 100),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection), 10),
                    });
            }
        }

        private static void SetupValentin(int missionNumber, MySmallShipBot valentin)
        {
            if (valentin != null)
            {
                valentin.Engine = new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_1);
                valentin.SetEquip(new List<MyMwcObjectBuilder_SmallShip_Weapon>
                    {
                        new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                        new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun),
                        new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher),
                    },
                    new List<MyMwcObjectBuilder_InventoryItem>
                    {
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic), 100),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection), 10),
                    });
            }
        }
    }
}
