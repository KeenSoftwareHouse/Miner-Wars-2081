using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabHangar_TypesEnum : ushort
    {
        VENDOR = 111,
        HANGAR = 112,
    }

    public class MyMwcObjectBuilder_PrefabHangar : /*MyMwcObjectBuilder_Prefab*/ MyMwcObjectBuilder_PrefabBase
    {
        internal MyMwcObjectBuilder_PrefabHangar()
            : base()
        {
        }

        public MyMwcObjectBuilder_PrefabHangar(MyMwcObjectBuilder_PrefabHangar_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabHangar;
        }

        public MyMwcObjectBuilder_PrefabHangar_TypesEnum PrefabHangarType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabHangar_TypesEnum)GetObjectBuilderId().Value;
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
