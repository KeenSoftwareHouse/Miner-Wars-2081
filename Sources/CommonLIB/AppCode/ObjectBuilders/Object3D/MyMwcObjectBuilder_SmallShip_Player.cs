using System;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using System.Data.SqlClient;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public class MyMwcObjectBuilder_SmallShip_Player : MyMwcObjectBuilder_SmallShip
    {
        internal MyMwcObjectBuilder_SmallShip_Player()
            : base()
        {
            DisplayName = "Player";
        }

        public MyMwcObjectBuilder_SmallShip_Player(MyMwcObjectBuilder_SmallShip_TypesEnum shipType,
            MyMwcObjectBuilder_Inventory inventory,
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons,
            MyMwcObjectBuilder_SmallShip_Engine engine,
            List<MyMwcObjectBuilder_AssignmentOfAmmo> assignmentOfAmmo,
            MyMwcObjectBuilder_SmallShip_Armor armor,
            MyMwcObjectBuilder_SmallShip_Radar radar,
            float? shipMaxHealth,
            float shipHealthRatio,
            float armorHealth,
            float electricity,
            float oxygen,
            float fuel,
            bool reflectorLight,
            bool reflectorLongRange,
            float reflectorShadowDistance,
            int aiPriority)
            : base(shipType, inventory, weapons, engine, assignmentOfAmmo, armor, radar, shipMaxHealth, shipHealthRatio, armorHealth, oxygen, fuel, reflectorLight, reflectorLongRange, reflectorShadowDistance, aiPriority)
        {
            DisplayName = "Player";
        }

        public MyMwcObjectBuilder_SmallShip_Player(MyMwcObjectBuilder_SmallShip ship)
            : this(ship.ShipType, ship.Inventory, ship.Weapons, ship.Engine, ship.AssignmentOfAmmo, ship.Armor, ship.Radar, ship.ShipMaxHealth, ship.ShipHealthRatio, ship.ArmorHealth,
            0, ship.Oxygen, ship.Fuel, ship.ReflectorLight, ship.ReflectorLongRange, ship.ReflectorShadowDistance, ship.AIPriority)
        {
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_Player;
        }

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            // because we don't want save playership's entity id, we set to null
            EntityId = null;

            base.Write(binaryWriter);
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion))
            {
                return false;
            }

            // because we don't want load playership's entity id, we set to null
            EntityId = null;

            return true;
        }

        public static MyMwcObjectBuilder_SmallShip_Player CreateDefaultShip(MyMwcObjectBuilder_SmallShip_TypesEnum shipType, MyMwcObjectBuilder_FactionEnum faction, int maxItems)
        {
            var result = new MyMwcObjectBuilder_SmallShip_Player(shipType,
                new MyMwcObjectBuilder_Inventory(
                    new List<MyMwcObjectBuilder_InventoryItem>()
                    {
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic), 1000f),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic), 1000f),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic), 1000f),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 1000f),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 1000f),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 1000f),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI), 1000f),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI), 1000f),
                        new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI), 1000f),
                    },
                    maxItems),
                new List<MyMwcObjectBuilder_SmallShip_Weapon>()
                {
                    new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon) { MountedDescriptor = MyMwcObjectBuilder_SmallShip_Weapon.LEFT_SIDE_MOUNT_DESCRIPTOR },
                    new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon) { MountedDescriptor = MyMwcObjectBuilder_SmallShip_Weapon.RIGHT_SIDE_MOUNT_DESCRIPTOR },
                    new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper) { MountedDescriptor = MyMwcObjectBuilder_SmallShip_Weapon.AUTO_MOUNT_DESCRIPTOR },
                    new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher) { MountedDescriptor = MyMwcObjectBuilder_SmallShip_Weapon.AUTO_MOUNT_DESCRIPTOR },
                },
                new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_3),
                new List<MyMwcObjectBuilder_AssignmentOfAmmo>()
                {
                    new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic),
                    new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic),
                    new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Third, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic),
                    new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fourth, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart),
                    new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fifth, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic),
                },
                new MyMwcObjectBuilder_SmallShip_Armor(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Advanced),
                new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1),
                null, 1f, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue,
                true,
                false,
                200, 0);
            result.Faction = faction;
            result.PositionAndOrientation = new MyMwcPositionAndOrientation(Matrix.Identity);
            return result;
        }

        public static MyMwcObjectBuilder_SmallShip_Player CreateObjectBuilderWithAllItems(MyMwcObjectBuilder_SmallShip_TypesEnum shipType, MyMwcObjectBuilder_FactionEnum faction, int maxInventoryItems)
        {
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            List<MyMwcObjectBuilder_AssignmentOfAmmo> ammoAssignment = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();
            List<MyMwcObjectBuilder_InventoryItem> inventoryItems = new List<MyMwcObjectBuilder_InventoryItem>();

            // weapons
            foreach (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weapon in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum)))
            {
                var weaponBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(weapon);
                weaponBuilder.SetAutoMount();
                weapons.Add(weaponBuilder);
                // we want have 2x autocanon
                if (weapon == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon)
                {
                    var autocannonBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(weapon);
                    autocannonBuilder.SetAutoMount();
                    weapons.Add(autocannonBuilder);
                }
            }

            // ammo assignment
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Third, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fourth, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fifth, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));

            // inventory items
            // ammo
            foreach (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)))
            {
                inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(ammo), 1000f));
            }

            // tools
            foreach (MyMwcObjectBuilder_SmallShip_Tool_TypesEnum tool in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Tool_TypesEnum)))
            {
                inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Tool(tool), 1f));
            }

            // radars
            foreach (MyMwcObjectBuilder_SmallShip_Radar_TypesEnum radar in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum)))
            {
                inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Radar(radar), 1f));
            }

            // engines
            foreach (MyMwcObjectBuilder_SmallShip_Engine_TypesEnum engine in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum)))
            {
                if (engine != MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1)
                {
                    inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Engine(engine), 1f));
                }
            }

            // armors
            foreach (MyMwcObjectBuilder_SmallShip_Armor_TypesEnum armor in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum)))
            {
                if (armor != MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic)
                {
                    inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Armor(armor), 1f));
                }
            }

            // foundation factory
            var foundationFactory = new MyMwcObjectBuilder_PrefabFoundationFactory();
            foundationFactory.PrefabHealthRatio = 1f;
            foundationFactory.PrefabMaxHealth = null;
            inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(foundationFactory, 1f));
            inventoryItems.RemoveAll(x => MyMwcObjectBuilder_InventoryItem.IsDisabled(x));

            if (inventoryItems.Count > maxInventoryItems)
            {
                inventoryItems = inventoryItems.GetRange(0, maxInventoryItems);
            }

            MyMwcObjectBuilder_SmallShip_Player builder =
                new MyMwcObjectBuilder_SmallShip_Player(
                    shipType,
                    new MyMwcObjectBuilder_Inventory(inventoryItems, maxInventoryItems),
                    weapons,
                    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1),
                    ammoAssignment,
                    new MyMwcObjectBuilder_SmallShip_Armor(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic),
                    new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1),
                    null, 1f, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue,
                    true, false, 200f, 0);
            builder.Faction = faction;

            return builder;
        }
    }
}
