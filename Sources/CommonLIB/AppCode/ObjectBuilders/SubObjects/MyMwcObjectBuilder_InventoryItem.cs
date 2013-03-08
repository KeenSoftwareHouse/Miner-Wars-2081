using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    [Flags]
    public enum MyInventoryItemTemporaryFlags 
    {
        NONE = 0,
        CANT_BE_MOVED = 1 << 0,
        TEMPORARY_ITEM = 1 << 1,
        NOT_ENOUGH_MONEY = 1 << 2,
    }

    public class MyMwcObjectBuilder_InventoryItem : MyMwcObjectBuilder_SubObjectBase
    {
        private static Dictionary<int, List<int>> DisabledItems = new Dictionary<int, List<int>>();

        static MyMwcObjectBuilder_InventoryItem() 
        {
            
        }

        public MyMwcObjectBuilder_Base ItemObjectBuilder { get; set; }
        public float Amount { get; set; }
        public MyInventoryItemTemporaryFlags TemporaryFlags { get; set; }

        internal MyMwcObjectBuilder_InventoryItem()
            : base()
        {
        }

        public static void ReloadDisabledItems(bool enable25D)
        {
            DisabledItems.Clear();

            DisabledItems.Add((int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool,
                new List<int>{ 
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NIGHT_VISION,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.AUTO_TARGETING,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.XRAY,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.LASER_POINTER,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.SOLAR_PANEL,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.BOOBY_TRAP,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REMOTE_CAMERA,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REMOTE_CAMERA_ON_DRONE,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_UNUSED,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REAR_CAMERA,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ELECTRICITY_KIT,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_ELECTRICITY_CONTAINER,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ALIEN_OBJECT_DETECTOR,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_CONVERTER,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_CONVERTER,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_OXYGEN_CONTAINER_DISABLED,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_FUEL_CONTAINER_DISABLED,
                    (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.SENSOR,

                });

            DisabledItems.Add((int)MyMwcObjectBuilderTypeEnum.Ore,
                new List<int>{
                    (int)MyMwcObjectBuilder_Ore_TypesEnum.XENON
                });

            DisabledItems.Add((int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,
                new List<int>{
                    (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Armor_Piercing_Incendiary,
                    (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Armor_Piercing_Incendiary,
                    (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Armor_Piercing_Incendiary,
                    (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb,
                });

            DisabledItems.Add((int)MyMwcObjectBuilderTypeEnum.SmallShip_Radar,
                new List<int>{
                    (int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1,
                    (int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_2,
                    (int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_3,
                });

            DisabledItems.Add((int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon,
                new List<int>{
                    (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser,
                    (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure,
                    (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw,
                    (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal,
                });

            if (!enable25D)
            {
                DisabledItems[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon].Add(
                    (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear
                    );
            }

            DisabledItems.Add((int)MyMwcObjectBuilderTypeEnum.Blueprint,
                new List<int>{
                    (int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit,
                    (int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit,
                    (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit,
                    (int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit,
                    (int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit,
                    (int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit,
                    (int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit,
                });

            DisabledItems.Add((int)MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory,
                new List<int>{
                    0,
                });

            DisabledItems.Add((int)MyMwcObjectBuilderTypeEnum.FalseId, new List<int>(MyMwcFactionsByIndex.GetFactionsIndexes()));
        }

        public MyMwcObjectBuilder_InventoryItem(MyMwcObjectBuilder_Base itemObjectBuilder, float amount)
        {
            ItemObjectBuilder = itemObjectBuilder;
            Amount = amount;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            ItemObjectBuilder.RemapEntityIds(remapContext);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.InventoryItem;
        }        

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Amount
            MyMwcLog.IfNetVerbose_AddToLog("Amount: " + Amount);
            MyMwcMessageOut.WriteFloat(Amount, binaryWriter);

            // Item's objectbuilder            
            bool isItemsObjectBuilder = ItemObjectBuilder != null;
            MyMwcMessageOut.WriteBool(isItemsObjectBuilder, binaryWriter);
            if (isItemsObjectBuilder) ItemObjectBuilder.Write(binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Amount
            float? amount = MyMwcMessageIn.ReadFloat(binaryReader);
            if (amount == null) return NetworkError();
            Amount = amount.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Amount: " + Amount);

            //  Item's objectbuilder
            bool? isItemObjectBuilder = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isItemObjectBuilder == null) return NetworkError();
            if (isItemObjectBuilder.Value)
            {
                ItemObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_Base;
                if (ItemObjectBuilder == null) return NetworkError();
                if (ItemObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                ItemObjectBuilder = null;
            }

            return true;
        }

        public bool CanBeMoved
        {
            get { return (TemporaryFlags & MyInventoryItemTemporaryFlags.CANT_BE_MOVED) == 0; }
        }

        public bool IsTemporaryItem
        {
            get { return (TemporaryFlags & MyInventoryItemTemporaryFlags.TEMPORARY_ITEM) != 0; }
        }

        public static bool IsDisabled(MyMwcObjectBuilder_InventoryItem inventoryItem) 
        {
            int? builderId = inventoryItem.ItemObjectBuilder.GetObjectBuilderId();
            MyMwcObjectBuilderTypeEnum builderType = inventoryItem.ItemObjectBuilder.GetObjectBuilderType();
            return IsDisabled(builderType, builderId);
        }

        public static bool IsDisabled(MyMwcObjectBuilderTypeEnum builderType, int? builderId) 
        {
            if (DisabledItems.ContainsKey((int)builderType))
            {
                List<int> disabledIds = DisabledItems[(int)builderType];
                if (disabledIds == null)
                {
                    return builderId == null;
                }
                else
                {
                    if (builderId != null)
                    {
                        return disabledIds.Contains(builderId.Value);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
