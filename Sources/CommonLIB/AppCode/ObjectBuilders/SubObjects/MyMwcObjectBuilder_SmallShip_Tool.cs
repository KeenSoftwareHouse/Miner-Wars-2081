using System;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_SmallShip_Tool_TypesEnum : ushort
    {
        REAR_CAMERA = 1,
        LASER_POINTER = 2,
        AUTO_TARGETING = 3,
        NIGHT_VISION = 4,
        NANO_REPAIR_TOOL = 5,
        MEDIKIT = 6,
        XRAY = 7,
        ANTIRADIATION_MEDICINE = 8,
        RADAR_JAMMER = 9,
        PERFORMANCE_ENHANCING_MEDICINE = 10,
        HEALTH_ENHANCING_MEDICINE = 11,
        EXTRA_FUEL_CONTAINER_DISABLED = 12,
        EXTRA_ELECTRICITY_CONTAINER = 13,
        EXTRA_OXYGEN_CONTAINER_DISABLED = 14,
        OXYGEN_CONVERTER = 15,
        FUEL_CONVERTER = 16,
        SOLAR_PANEL = 17,
        BOOBY_TRAP = 18,
        SENSOR = 19,
        REMOTE_CAMERA = 20,
        REMOTE_CAMERA_ON_DRONE = 21,
        ALIEN_OBJECT_DETECTOR = 22,
        RADAR_UNUSED = 23,
        HEALTH_KIT = 24,
        REPAIR_KIT = 25,
        OXYGEN_KIT = 26,
        FUEL_KIT = 27,
        ELECTRICITY_KIT = 28,
    }

    public class MyMwcObjectBuilder_SmallShip_Tool : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_SmallShip_Tool_TypesEnum ToolType;

        internal MyMwcObjectBuilder_SmallShip_Tool()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShip_Tool(MyMwcObjectBuilder_SmallShip_Tool_TypesEnum toolType)
        {
            ToolType = toolType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_Tool;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)ToolType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            ToolType = (MyMwcObjectBuilder_SmallShip_Tool_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }


        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Tool Type
            MyMwcLog.IfNetVerbose_AddToLog("ToolType: " + ToolType);
            MyMwcMessageOut.WriteObjectBuilderSmallShipToolTypesEnum(ToolType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Tool Type
            MyMwcObjectBuilder_SmallShip_Tool_TypesEnum? toolType = MyMwcMessageIn.ReadObjectBuilderSmallShipToolTypesEnumEx(binaryReader, senderEndPoint);
            if (toolType == null) return NetworkError();
            ToolType = toolType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ToolType: " + ToolType);

            return true;
        }

        public override string ToString()
        {
            return base.GetType().Name + "(" + ToolType.ToString() + ")";
        }
    }
}