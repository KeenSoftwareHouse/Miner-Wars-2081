using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools
{    
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_SmallShip_Radar_TypesEnum : ushort
    { 
        Radar_1 = 1,
        Radar_2 = 2,
        Radar_3 = 3,
    }

    public class MyMwcObjectBuilder_SmallShip_Radar : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_SmallShip_Radar_TypesEnum RadarType;
        //public float RadarRange;        // in meters

        internal MyMwcObjectBuilder_SmallShip_Radar()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum radarType/*, float radarRange*/)
        {
            RadarType = radarType;
            //RadarRange = radarRange;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_Radar;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)RadarType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            RadarType = (MyMwcObjectBuilder_SmallShip_Radar_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Radar Type
            MyMwcLog.IfNetVerbose_AddToLog("RadarType: " + RadarType);
            MyMwcMessageOut.WriteObjectBuilderSmallShipRadarTypesEnum(RadarType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Radar Type
            MyMwcObjectBuilder_SmallShip_Radar_TypesEnum? radarType = MyMwcMessageIn.ReadObjectBuilderSmallShipRadarTypesEnumEx(binaryReader, senderEndPoint);
            if (radarType == null) return NetworkError();
            RadarType = radarType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("RadarType: " + RadarType);

            return true;
        }
    }
}
