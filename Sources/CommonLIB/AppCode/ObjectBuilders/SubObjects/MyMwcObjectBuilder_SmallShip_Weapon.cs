using System;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum : ushort
    {
        //Guided_Missile_Launcher = 1,
        Cannon = 2,
        Autocanon = 3,
        //Flamethrower = 4, // future release
        Shotgun = 5,
        Machine_Gun = 6,
        Sniper = 7,
        Automatic_Rifle_With_Silencer = 8,
        Universal_Launcher_Back = 9,
        Universal_Launcher_Front = 10,

        Harvesting_Device = 11,
        Drilling_Device_Crusher = 12,
        Drilling_Device_Thermal = 13,
        Drilling_Device_Saw = 14,
        Drilling_Device_Nuclear = 15,
        Drilling_Device_Laser = 16,
        Drilling_Device_Pressure = 18,

        Missile_Launcher = 17,
    }

    public class MyMwcObjectBuilder_SmallShip_Weapon : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum WeaponType;

        internal MyMwcObjectBuilder_SmallShip_Weapon()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType)
        {
            WeaponType = weaponType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_Weapon;
        }

        public override int? GetObjectBuilderId()
        {
            return (int) WeaponType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            WeaponType = (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum) Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  WeaponType
            MyMwcLog.IfNetVerbose_AddToLog("WeaponType: " + WeaponType);
            MyMwcMessageOut.WriteObjectBuilderSmallShipWeaponTypesEnum(WeaponType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false)
                return NetworkError();

            //  WeaponType
            MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum? weaponType =
                MyMwcMessageIn.ReadObjectBuilderSmallShipWeaponTypesEnumEx(binaryReader, senderEndPoint);
            if (weaponType == null)
                return NetworkError();
            WeaponType = weaponType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("WeaponType: " + WeaponType);

            return true;
        }

        public override string ToString()
        {
            return base.GetType().Name + "(" + WeaponType.ToString() + ")";
        }

        public bool IsHarvester
        {
            get { return WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device; }
        }

        public bool IsDrill
        {
            get
            {
                switch (WeaponType)
                {
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure:
                        return true;
                }

                return false;
            }
        }

        public bool IsUniversalLauncher
        {
            get
            {
                return WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front ||
                       WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back;
            }
        }

        public bool IsNormalWeapon
        {
            get { return !IsDrill && !IsHarvester && !IsUniversalLauncher; }
        }
    }
}
