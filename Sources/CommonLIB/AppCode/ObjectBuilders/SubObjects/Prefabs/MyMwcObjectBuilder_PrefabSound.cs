using System.IO;
using System.Net;
using System.Data.SqlClient;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabSound_TypesEnum : ushort
    {
        DEFAULT_SOUND_PREFAB_0 = 269,
        P561_A01_SOUND = 270,
        P561_B01_SOUND = 271,
        P561_C01_SOUND = 272,
        P561_D01_SOUND = 273,
        MOTHERSHIP_SOUND = 535,
        MADELINE_MOTHERSHIP_SOUND = 536,
    }

    public class MyMwcObjectBuilder_PrefabSound : /*MyMwcObjectBuilder_Prefab*/ MyMwcObjectBuilder_PrefabBase
    {
        public enum MySoundEffectTypeEnum
        {
            NORMAL = 0,//default
            //todo
        };

        internal MyMwcObjectBuilder_PrefabSound()
            : base()
        {
        }

        public MyMwcObjectBuilder_PrefabSound(MyMwcObjectBuilder_PrefabSound_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
            
        }


        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabSound;
        }

        public MyMwcObjectBuilder_PrefabSound_TypesEnum PrefabSoundType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabSound_TypesEnum) GetObjectBuilderId().Value;
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
