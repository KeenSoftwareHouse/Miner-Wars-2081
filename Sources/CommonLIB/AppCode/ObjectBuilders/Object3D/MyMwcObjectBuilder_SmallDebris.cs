using System;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Collections.Generic;
using SysUtils.Utils;
using System.Data.SqlClient;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_SmallDebris_TypesEnum : ushort
    {
        pipe_bundle = 1,
        //sat_01 = 2,
        Debris1 = 3,
        Debris2 = 4,
        Debris3 = 5,
        Debris4 = 6,
        Debris5 = 7,
        Debris6 = 8,
        Debris7 = 9,
        Debris8 = 10,
        Debris9 = 11,
        Debris10 = 12,
        Debris11 = 13,
        Debris12 = 14,
        Debris13 = 15,
        Debris14 = 16,
        Debris15 = 17,
        Debris16 = 18,
        Debris17 = 19,
        Debris18 = 20,
        Debris19 = 21,
        Debris20 = 22,
        Debris21 = 23,
        Debris22 = 24,
        Debris23 = 25,
        Debris24 = 26,
        Debris25 = 27,
        Debris26 = 28,
        Debris27 = 29,
        Debris28 = 30,
        Debris29 = 31,
        Debris30 = 32,
        Debris31 = 33,
        Debris32_pilot = 34,
        UtilityVehicle_1 = 35,
        //UtilityVehicle_2 = 36,
        Cistern = 37,
        Standard_Container_1 = 38,
        Standard_Container_2 = 39,
        Standard_Container_3 = 40,
        Standard_Container_4 = 41,
        //DerelictShip01 = 42
        //MechPlate1 = 43,
        //MechPlate2 = 44,
        //MechPlate3 = 45,
        //ScrapPlate1 = 46,
        //ScrapPlate2 = 47
    }

    enum MyMwcObjectBuilder_SmallDebris_PropertiesEnum : short
    {
        Immovable = 1
    }
    
    public class MyMwcObjectBuilder_SmallDebris : MyMwcObjectBuilder_Object3dBase
    {
        public MyMwcObjectBuilder_SmallDebris_TypesEnum DebrisType;
        public bool Immovable;
        public float Mass;

        internal MyMwcObjectBuilder_SmallDebris()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallDebris(MyMwcObjectBuilder_SmallDebris_TypesEnum debrisType, bool immovable, float mass)
        {
            DebrisType = debrisType;
            Immovable = immovable;
            Mass = mass;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)DebrisType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            DebrisType = (MyMwcObjectBuilder_SmallDebris_TypesEnum) Convert.ToUInt16(objectBuilderId);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallDebris;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Debris Type
            MyMwcLog.IfNetVerbose_AddToLog("DebrisType: " + DebrisType);
            MyMwcMessageOut.WriteObjectBuilderSmallDebrisTypesEnum(DebrisType, binaryWriter);

            //Immovable
            MyMwcLog.IfNetVerbose_AddToLog("Immovable: " + Immovable);
            MyMwcMessageOut.WriteBool(Immovable, binaryWriter);

        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Ship Type
            MyMwcObjectBuilder_SmallDebris_TypesEnum? debrisType = MyMwcMessageIn.ReadObjectBuilderSmallDebrisTypesEnumEx(binaryReader, senderEndPoint);
            if (debrisType == null) return NetworkError();
            DebrisType = debrisType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("DebrisType: " + DebrisType);

            //Immovable
            bool? immovable = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (immovable == null) return NetworkError();
            Immovable = immovable.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Immovable: " + Immovable);

            return true;
        }
    }
}
