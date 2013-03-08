using System;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabParticles_TypesEnum : ushort
    {
        DEFAULT_PARTICLE_PREFAB_0 = 264,
        P551_A01_PARTICLES = 265,
        P551_B01_PARTICLES = 266,
        P551_C01_PARTICLES = 267,
        P551_D01_PARTICLES = 268,
    }

    public class MyMwcObjectBuilder_PrefabParticles : /*MyMwcObjectBuilder_Prefab*/ MyMwcObjectBuilder_PrefabBase
    {
        internal MyMwcObjectBuilder_PrefabParticles()
            : base()
        {
        }

        public MyMwcObjectBuilder_PrefabParticles(MyMwcObjectBuilder_PrefabParticles_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabParticles;
        }

        public MyMwcObjectBuilder_PrefabParticles_TypesEnum PrefabParticleType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabParticles_TypesEnum) GetObjectBuilderId().Value;
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

