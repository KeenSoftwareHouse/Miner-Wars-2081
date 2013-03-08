using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Reflection;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_Ore_TypesEnum : ushort
    {
        STONE = 1,
        IRON = 2,
        URANITE = 3,
        HELIUM = 4,
        ICE = 5,
        GOLD = 6,
        SILVER = 7,
        //INDESTRUCTIBLE = 8,
        SILICON = 9,
        PLATINUM = 10,
        NICKEL = 11,
        COBALT = 12,
        MAGNESIUM = 13,
        TREASURE = 14,
        ORGANIC = 15,
        XENON = 16,
        LAVA = 17,
        SNOW = 18,
        SANDSTONE = 19,
        CONCRETE = 20,
    }

    public class MyMwcObjectBuilder_Ore : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_Ore_TypesEnum OreType { get; set; }

        internal MyMwcObjectBuilder_Ore()
            : base()
        {
        }

        public MyMwcObjectBuilder_Ore(MyMwcObjectBuilder_Ore_TypesEnum oreType)
        {
            OreType = oreType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Ore;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)OreType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            OreType = (MyMwcObjectBuilder_Ore_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Ore Type
            MyMwcLog.IfNetVerbose_AddToLog("OreType: " + OreType);
            MyMwcMessageOut.WriteObjectBuilderOreTypesEnum(OreType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Ore Type
            MyMwcObjectBuilder_Ore_TypesEnum? oreType = MyMwcMessageIn.ReadObjectBuilderOreTypesEnumEx(binaryReader, senderEndPoint);
            if (oreType == null) return NetworkError();
            OreType = oreType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("OreType: " + OreType);

            return true;
        }

        public override string ToString()
        {
            return base.GetType().Name + "(" + OreType.ToString() + ")";
        }
    }
}
