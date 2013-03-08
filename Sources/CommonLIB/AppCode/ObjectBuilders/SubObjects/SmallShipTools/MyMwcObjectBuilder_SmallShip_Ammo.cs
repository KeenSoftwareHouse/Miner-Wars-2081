using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum : ushort
    {
        Automatic_Rifle_With_Silencer_High_Speed = 1,
        Automatic_Rifle_With_Silencer_SAPHEI = 2,
        Automatic_Rifle_With_Silencer_BioChem = 3,
        Sniper_High_Speed = 4,
        Sniper_SAPHEI = 5,
        Sniper_BioChem = 6,
        Autocannon_Basic = 7,
        Autocannon_High_Speed = 8,
        Autocannon_Armor_Piercing_Incendiary = 9,
        Autocannon_SAPHEI = 10,
        Autocannon_BioChem = 11,
        Machine_Gun_Basic = 12,
        Machine_Gun_High_Speed = 13,
        Machine_Gun_Armor_Piercing_Incendiary = 14,
        Machine_Gun_SAPHEI = 15,
        Machine_Gun_BioChem = 16,
        Shotgun_Basic = 17,
        Shotgun_High_Speed = 18,
        Shotgun_Explosive = 19,
        Shotgun_Armor_Piercing = 20,
        Cannon_Basic = 21,
        Cannon_High_Speed = 22,
        Cannon_Armor_Piercing_Incendiary = 23,
        Cannon_SAPHEI = 24,
        Cannon_Proximity_Explosive = 25,
        Cannon_Tunnel_Buster = 26,
        Universal_Launcher_Sphere_Explosive = 27,
        Universal_Launcher_Directional_Explosive = 28,
        Universal_Launcher_Time_Bomb = 29,
        Universal_Launcher_Remote_Bomb = 30,
        Universal_Launcher_Asteroid_Killer = 31,
        Universal_Launcher_Mine_Smart = 32,
        Universal_Launcher_Mine_Basic = 33,
        Universal_Launcher_Gravity_Bomb = 34,
        Universal_Launcher_Hologram = 35,
        Universal_Launcher_Decoy_Flare = 36,
        Universal_Launcher_Flash_Bomb = 37,
        Universal_Launcher_Illuminating_Shell = 38,
        Universal_Launcher_Smoke_Bomb = 39,
        Universal_Launcher_Remote_Camera = 40,
        Guided_Missile_Visual_Detection = 41,
        Guided_Missile_Engine_Detection = 42,
        Guided_Missile_Radar_Detection = 43,
        Missile_Basic = 44,
        Universal_Launcher_Mine_BioChem = 45,
        Missile_BioChem = 46,
        Cannon_BioChem = 47,
        Autocannon_EMP = 48,
        Sniper_EMP = 49,
        Machine_Gun_EMP = 50,
        Missile_EMP = 51,
        Cannon_EMP = 52,
        Universal_Launcher_EMP_Bomb = 53,
    }

    public class MyMwcObjectBuilder_SmallShip_Ammo : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType;

        internal MyMwcObjectBuilder_SmallShip_Ammo()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            AmmoType = ammoType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_Ammo;
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

            //  Ammo Type
            MyMwcLog.IfNetVerbose_AddToLog("AmmoType: " + AmmoType);
            MyMwcMessageOut.WriteObjectBuilderSmallShipAmmoTypesEnum(AmmoType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Ammo Type
            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum? ammoType = MyMwcMessageIn.ReadObjectBuilderSmallShipAmmoTypesEnumEx(binaryReader, senderEndPoint);
            if (ammoType == null) return NetworkError();
            AmmoType = ammoType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AmmoType: " + AmmoType);

            return true;
        }
    }
}
