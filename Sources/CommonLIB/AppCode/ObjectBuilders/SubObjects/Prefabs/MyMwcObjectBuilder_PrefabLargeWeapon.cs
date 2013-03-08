using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum : ushort
    {
        P352_A01_LARGESHIP_AUTOCANNON = 274,
        P352_A01_LARGESHIP_MACHINEGUN = 275,
        P352_A01_LARGESHIP_CIWS = 276,
        P352_A01_LARGESHIP_MISSILE_BASIC4 = 277,
        P352_A01_LARGESHIP_MISSILE_BASIC6 = 278,
        P352_A01_LARGESHIP_MISSILE_BASIC9 = 279,
        P352_A02_LARGESHIP_MISSILE_GUIDED4 = 280,
        P352_A02_LARGESHIP_MISSILE_GUIDED6 = 281,
        P352_A02_LARGESHIP_MISSILE_GUIDED9 = 282,
    }

    public class MyMwcObjectBuilder_PrefabLargeWeapon : MyMwcObjectBuilder_PrefabBase
    {
        public float AimingDistance;
        public float SearchingDistance;

        internal MyMwcObjectBuilder_PrefabLargeWeapon()
            : base()
        {
            AimingDistance = 1000f;
            SearchingDistance = 2000f;
        }

        public MyMwcObjectBuilder_PrefabLargeWeapon(MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, float aimingDistance, float searchingDistance, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
            AimingDistance = aimingDistance;
            SearchingDistance = searchingDistance;
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon;
        }

        public MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum PrefabLargeWeaponType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum)GetObjectBuilderId().Value;
            }
            set
            {
                SetObjectBuilderId((int)value);
            }
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Aiming distance
            MyMwcLog.IfNetVerbose_AddToLog("AimingDistance: " + AimingDistance);
            binaryWriter.Write(AimingDistance);

            // Searching distance
            MyMwcLog.IfNetVerbose_AddToLog("SearchingDistance: " + SearchingDistance);
            binaryWriter.Write(SearchingDistance);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Aiming distance
            float? aimingDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (aimingDistance == null) return NetworkError();
            AimingDistance = aimingDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AimingDistance: " + AimingDistance);

            // Searching distance
            float? searchingDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (searchingDistance == null) return NetworkError();
            SearchingDistance = searchingDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("SearchingDistance: " + SearchingDistance);

            return true;
        }
    }
}
