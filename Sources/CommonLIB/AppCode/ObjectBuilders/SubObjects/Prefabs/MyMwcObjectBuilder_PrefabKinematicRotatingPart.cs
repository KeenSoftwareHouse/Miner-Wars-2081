using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum : ushort
    {
        DEFAULT = 0,
    }

    public class MyMwcObjectBuilder_PrefabKinematicRotatingPart : MyMwcObjectBuilder_PrefabBase
    {
        internal MyMwcObjectBuilder_PrefabKinematicRotatingPart()
            : base()
        {
        }

        public MyMwcObjectBuilder_PrefabKinematicRotatingPart(MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance,
            MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {            
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabKinematicRotatingPart;
        }

        public MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum PrefabKinematicRotatingPartType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum)GetObjectBuilderId().Value;
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
