using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.LargeShipTools
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum : ushort
    {
        CIWS_Armor_Piercing_Incendiary = 1,
        CIWS_High_Explosive_Incendiary = 2,
        CIWS_SAPHEI = 3,
    }

    public class MyMwcObjectBuilder_LargeShip_Ammo : MyMwcObjectBuilder_LargeShip_ToolBase
    {
        public int Amount;
        public MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum AmmoType;

        internal MyMwcObjectBuilder_LargeShip_Ammo()
            : base()
        {
        }

        public MyMwcObjectBuilder_LargeShip_Ammo(int amount, MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum ammoType)
        {
            Amount = amount;
            AmmoType = ammoType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.LargeShip_Ammo;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)AmmoType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            AmmoType = (MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Amount
            MyMwcLog.IfNetVerbose_AddToLog("Amount: " + Amount);
            MyMwcMessageOut.WriteInt32(Amount, binaryWriter);

            //  Ammo Type
            MyMwcLog.IfNetVerbose_AddToLog("AmmoType: " + AmmoType);
            MyMwcMessageOut.WriteObjectBuilderLargeShipAmmoTypesEnum(AmmoType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            int? amount = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (amount == null) return NetworkError();
            Amount = amount.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Amount: " + Amount);

            //  Ammo Type
            MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum? ammoType = MyMwcMessageIn.ReadObjectBuilderLargeShipAmmoTypesEnumEx(binaryReader, senderEndPoint);
            if (ammoType == null) return NetworkError();
            AmmoType = ammoType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AmmoType: " + AmmoType);

            return true;
        }
    }
}
