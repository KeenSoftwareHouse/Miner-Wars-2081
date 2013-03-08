using System.Collections.Generic;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.LargeShipTools;
using System.Data.SqlClient;
using System;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_LargeShip_TypesEnum : ushort
    {
        KAI = 1,
        MOTHERSHIP_SAYA = 3,
        JEROMIE_INTERIOR_STATION = 6, //temporary for test mission!!
        CRUISER_SHIP = 7, //temporary for testing!!
        ARDANT = 8
    }

    public class MyMwcObjectBuilder_LargeShip : MyMwcObjectBuilder_Ship
    {
        public MyMwcObjectBuilder_LargeShip_TypesEnum ShipType;

        protected MyMwcObjectBuilder_LargeShip() : base()
        {
        }

        public MyMwcObjectBuilder_LargeShip(MyMwcObjectBuilder_LargeShip_TypesEnum shipType)
        {
            ShipType = shipType;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)ShipType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            ShipType = (MyMwcObjectBuilder_LargeShip_TypesEnum) Convert.ToUInt16(objectBuilderId);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.LargeShip;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Ship Type
            MyMwcLog.IfNetVerbose_AddToLog("ShipType: " + ShipType);
            MyMwcMessageOut.WriteObjectBuilderLargeShipTypesEnum(ShipType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Ship Type
            MyMwcObjectBuilder_LargeShip_TypesEnum? shipType = MyMwcMessageIn.ReadObjectBuilderLargeShipTypesEnumEx(binaryReader, senderEndPoint);
            if (shipType == null) return NetworkError();
            ShipType = shipType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ShipType: " + ShipType);

            return true;
        }
    }
}
