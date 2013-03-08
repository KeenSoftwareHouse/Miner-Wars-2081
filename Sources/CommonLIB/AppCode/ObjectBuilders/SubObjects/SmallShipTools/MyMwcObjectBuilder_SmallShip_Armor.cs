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
    public enum MyMwcObjectBuilder_SmallShip_Armor_TypesEnum : ushort
    {
        Basic = 1,
        Advanced = 2,
        High_Endurance = 3,
        Solar_Wind = 4        
    }

    public class MyMwcObjectBuilder_SmallShip_Armor : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_SmallShip_Armor_TypesEnum ArmorType;

        internal MyMwcObjectBuilder_SmallShip_Armor()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShip_Armor(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum armorType)
        {
            ArmorType = armorType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_Armor;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)ArmorType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            ArmorType = (MyMwcObjectBuilder_SmallShip_Armor_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Armor Type
            MyMwcLog.IfNetVerbose_AddToLog("ArmorType: " + ArmorType);
            MyMwcMessageOut.WriteObjectBuilderSmallShipArmorTypesEnum(ArmorType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Armor Type
            MyMwcObjectBuilder_SmallShip_Armor_TypesEnum? armorType = MyMwcMessageIn.ReadObjectBuilderSmallShipArmorTypesEnumEx(binaryReader, senderEndPoint);
            if (armorType == null) return NetworkError();
            ArmorType = armorType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ArmorType: " + ArmorType);

            return true;
        }
    }
}
