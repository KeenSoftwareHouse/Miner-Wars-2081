using System.Collections.Generic;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public enum MyMwcObjectBuilder_Drone_TypesEnum : byte
    {
        DroneUS = 0,
        DroneCN = 1,
        DroneSS = 2,
    }

    public class MyMwcObjectBuilder_Drone : MyMwcObjectBuilder_SmallShip_Bot
    {
        public MyMwcObjectBuilder_Drone_TypesEnum DroneType;

        public MyMwcObjectBuilder_Drone()
        {
            Inventory = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 1);
            Faction = MyMwcObjectBuilder_FactionEnum.None;
        }

        public MyMwcObjectBuilder_Drone(
            MyMwcObjectBuilder_Drone_TypesEnum droneType = MyMwcObjectBuilder_Drone_TypesEnum.DroneUS, 
            MyMwcObjectBuilder_FactionEnum faction = MyMwcObjectBuilder_FactionEnum.None)
            :this()
        {
            DroneType = droneType;
            Faction = faction;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Drone;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            DroneType = (MyMwcObjectBuilder_Drone_TypesEnum)objectBuilderId;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)DroneType;
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyCommonDebugUtils.AssertDebug(Inventory != null);

            // Drone type
            MyMwcLog.IfNetVerbose_AddToLog("DroneType: " + DroneType);
            MyMwcMessageOut.WriteByte((byte) DroneType, binaryWriter);
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            if (gameVersion > 01085002)
            {
                return ReadCurrent(binaryReader, senderEndPoint, gameVersion);
            }
            else
            {
                return Read01085002(binaryReader, senderEndPoint, gameVersion);
            }


            
        }
        private bool ReadCurrent(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            var droneType = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (droneType == null) return false;
            DroneType = (MyMwcObjectBuilder_Drone_TypesEnum)droneType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("DroneType: " + DroneType);

            return true;
        }

        private bool Read01085002(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            var droneType = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (droneType == null) return false;
            DroneType = (MyMwcObjectBuilder_Drone_TypesEnum) droneType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("DroneType: " + DroneType);

            // Owner Ship
            bool? hasId = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasId.HasValue) return NetworkError(); // Cannot read bool - whether owner entity id is null or not
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Drone.OwnerEntityId.HasValue: " + hasId.Value);

            // Testing whether owner entity id is null
            if (hasId.Value)
            {
                // entity id has value - read the value
                uint? ownerEntityID = MyMwcMessageIn.ReadUInt32Ex(binaryReader, senderEndPoint);
                if (!ownerEntityID.HasValue) return NetworkError(); // Cannot read owner entity ID

                MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_Drone.OwnerEntityId.Value: " + ownerEntityID.Value);
                this.OwnerId = ownerEntityID.Value;
            }
            else
            {
                this.OwnerId = null;
            }

            return true;
        }
    }
}