using System;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_SmallShip_Engine_TypesEnum : ushort
    {
        Nuclear_1 = 1,
        Nuclear_2 = 2,
        Nuclear_3 = 3,
        Nuclear_4 = 4,
        Nuclear_5 = 5,
        Chemical_1 = 6,
        Chemical_2 = 7,
        Chemical_3 = 8,
        Chemical_4 = 9,
        Chemical_5 = 10,
        PowerCells_1 = 11,
        PowerCells_2 = 12,
        PowerCells_3 = 13,
        PowerCells_4 = 14,
        PowerCells_5 = 15
    }

    public class MyMwcObjectBuilder_SmallShip_Engine : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_SmallShip_Engine_TypesEnum EngineType;

        internal MyMwcObjectBuilder_SmallShip_Engine()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum engineType)
        {
            EngineType = engineType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_Engine;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)EngineType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            EngineType = (MyMwcObjectBuilder_SmallShip_Engine_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Engine Type
            MyMwcLog.IfNetVerbose_AddToLog("EngineType: " + EngineType);
            MyMwcMessageOut.WriteObjectBuilderSmallShipEngineTypesEnum(EngineType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Engine Type
            MyMwcObjectBuilder_SmallShip_Engine_TypesEnum? engineType = MyMwcMessageIn.ReadObjectBuilderSmallShipEngineTypesEnumEx(binaryReader, senderEndPoint);
            if (engineType == null) return NetworkError();
            EngineType = engineType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("EngineType: " + EngineType);

            return true;
        }
    }
}
