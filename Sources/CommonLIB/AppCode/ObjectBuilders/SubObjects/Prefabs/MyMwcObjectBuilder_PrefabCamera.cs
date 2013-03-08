using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabCamera_TypesEnum : ushort
    {
        DEFAULT = 0
    }

    public class MyMwcObjectBuilder_PrefabCamera : MyMwcObjectBuilder_PrefabBase
    {
        //public bool On { get; set; }

        internal MyMwcObjectBuilder_PrefabCamera()
            : base()
        {
            //On = false;
        }

        public MyMwcObjectBuilder_PrefabCamera(MyMwcObjectBuilder_PrefabCamera_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority/*, bool on*/)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
            //On = on;
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabCamera;
        }

        public MyMwcObjectBuilder_PrefabCamera_TypesEnum PrefabCameraType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabCamera_TypesEnum)GetObjectBuilderId().Value;
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

            ////  On
            //MyMwcLog.IfNetVerbose_AddToLog("On: " + On);
            //MyMwcMessageOut.WriteBool(On, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();


            ////  On
            //bool? on = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            //if (on == null) return NetworkError();
            //On = on.Value;
            //MyMwcLog.IfNetVerbose_AddToLog("On: " + On);

            return true;
        }
    }
}
