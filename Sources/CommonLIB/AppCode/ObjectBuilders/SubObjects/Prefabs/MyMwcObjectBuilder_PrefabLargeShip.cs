using System;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Text;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabLargeShip_TypesEnum : ushort
    {
        LARGESHIP_KAI = 378,
        LARGESHIP_SAYA = 379,
        LARGESHIP_ARDANT = 380,
        FOURTH_REICH_MOTHERSHIP = 392,
        FOURTH_REICH_MOTHERSHIP_B = 393,
        RUS_MOTHERSHIP = 394,
        RUSSIAN_MOTHERSHIP_HUMMER = 395,
        MSHIP_BODY = 396,
        MSHIP_ENGINE = 397,
        MSHIP_SHIELD_BACK_LARGE_LEFT = 398,
        MSHIP_SHIELD_BACK_LARGE_RIGHT = 399,
        MSHIP_SHIELD_BACK_SMALL_LEFT = 400,
        MSHIP_SHIELD_BACK_SMALL_RIGHT = 401,
        MSHIP_SHIELD_FRONT_LARGE_LEFT = 402,
        MSHIP_SHIELD_FRONT_LARGE_RIGHT = 403,
        MSHIP_SHIELD_FRONT_SMALL_LEFT = 404,
        MSHIP_SHIELD_FRONT_SMALL_RIGHT = 405,
        MSHIP_SHIELD_FRONT_SMALL02_LEFT = 406,
        MSHIP_SHIELD_FRONT_SMALL02_RIGHT = 407,
    }

    public class MyMwcObjectBuilder_PrefabLargeShip : /*MyMwcObjectBuilder_Prefab*/ MyMwcObjectBuilder_PrefabBase
    {
        internal MyMwcObjectBuilder_PrefabLargeShip()
            : base()
        {
        }

        /// <summary>
        /// c-tor
        /// </summary>
        /// <param name="prefabType"></param>
        /// <param name="position"></param>
        /// <param name="anglesInContainer"></param>
        public MyMwcObjectBuilder_PrefabLargeShip(MyMwcObjectBuilder_PrefabLargeShip_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabLargeShip;
        }

        public MyMwcObjectBuilder_PrefabLargeShip_TypesEnum PrefabLargeShipType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabLargeShip_TypesEnum) GetObjectBuilderId().Value;
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
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();            

            return true;
        }
    }
}
