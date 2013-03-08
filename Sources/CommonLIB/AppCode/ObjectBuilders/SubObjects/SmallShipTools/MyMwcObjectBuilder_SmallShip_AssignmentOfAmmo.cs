using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.IO;
using System.Net;
using System.Data.SqlClient;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_FireKeyEnum : byte
    {
        Primary = 1,
        Secondary = 2,
        Third = 3,
        Fourth = 4,
        Fifth = 5,
        HologramFront = 6,
        HologramBack = 7,
        BasicMineFront = 8,
        BasicMineBack = 9,
        SmartMineFront = 10,
        SmartMineBack = 11,
        FlashBombFront = 12,
        FlashBombBack = 13,
        DecoyFlareFront = 14,
        DecoyFlareBack = 15,
        SmokeBombFront = 16,
        SmokeBombBack = 17,
    }

    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_AmmoGroupEnum : byte
    {
        Bullet = 1,
        Missile = 2,
        Cannon = 3,
        UniversalLauncherFront = 4,
        UniversalLauncherBack = 5
    }

    public class MyMwcObjectBuilder_AssignmentOfAmmo : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_FireKeyEnum FireKey;
        public MyMwcObjectBuilder_AmmoGroupEnum Group;
        public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType;

        internal MyMwcObjectBuilder_AssignmentOfAmmo()
            : base()
        {
        }

        public MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum fireKey, 
            MyMwcObjectBuilder_AmmoGroupEnum group, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            FireKey = fireKey;
            Group = group;
            AmmoType = ammoType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_AssignmentOfAmmo;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)AmmoType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            AmmoType = (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  FireKey
            MyMwcLog.IfNetVerbose_AddToLog("FireKey: " + FireKey);
            MyMwcMessageOut.WriteObjectBuilderSmallShipAssignmentOfAmmoFireKeyEnum(FireKey, binaryWriter);

            //  Group
            MyMwcLog.IfNetVerbose_AddToLog("Group: " + Group);
            MyMwcMessageOut.WriteObjectBuilderSmallShipAssignmentOfAmmoGroupEnum(Group, binaryWriter);

            //  Ammo Type
            MyMwcLog.IfNetVerbose_AddToLog("AmmoType: " + AmmoType);
            MyMwcMessageOut.WriteObjectBuilderSmallShipAmmoTypesEnum(AmmoType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  FireKey
            MyMwcObjectBuilder_FireKeyEnum? fireKey = MyMwcMessageIn.ReadObjectBuilderSmallShipAssignmentOfAmmoFireKeyEnumEx(binaryReader, senderEndPoint);
            if (fireKey == null) return NetworkError();
            FireKey = fireKey.Value;
            MyMwcLog.IfNetVerbose_AddToLog("FireKey: " + FireKey);

            //  FireKey
            MyMwcObjectBuilder_AmmoGroupEnum? group = MyMwcMessageIn.ReadObjectBuilderSmallShipAssignmentOfAmmoGroupEnumEx(binaryReader, senderEndPoint);
            if (group == null) return NetworkError();
            Group = group.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Group: " + Group);

            //  Ammo Type
            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum? ammoType = MyMwcMessageIn.ReadObjectBuilderSmallShipAmmoTypesEnumEx(binaryReader, senderEndPoint);
            if (ammoType == null) return NetworkError();
            AmmoType = ammoType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AmmoType: " + AmmoType);

            return true;
        }
    }
}
