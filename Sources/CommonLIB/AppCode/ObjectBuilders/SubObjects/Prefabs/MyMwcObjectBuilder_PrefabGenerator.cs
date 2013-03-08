using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Net;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabGenerator_TypesEnum : ushort
    {
        P321C01_INERTIA_GENERATOR = 0,
        P321C03_CENTRIFUGE = 1,
        P321C04_BOX_GENERATOR = 2,
        P321C05_CENTRIFUGE_BIG = 3,
        P321C02_GENERATOR_WALL_BIG = 4,
        P321C06_INERTIA_GENERATOR_B = 5,
        P321C07_GENERATOR = 6,
    }

    public class MyMwcObjectBuilder_PrefabGenerator : MyMwcObjectBuilder_PrefabBase
    {
        //public bool On { get; set; }

        internal MyMwcObjectBuilder_PrefabGenerator()
            : base()
        {            
        }

        public MyMwcObjectBuilder_PrefabGenerator(MyMwcObjectBuilder_PrefabGenerator_TypesEnum generatorType,
            MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth,
            float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority, /*bool on,*/ float range)
            : base((int)generatorType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
            //On = on;
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabGenerator;
        }


        public MyMwcObjectBuilder_PrefabGenerator_TypesEnum PrefabGeneratorType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabGenerator_TypesEnum)GetObjectBuilderId().Value;
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
