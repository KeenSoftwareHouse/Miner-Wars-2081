using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum : ushort
    {
        P415_A02_DOOR_LEFT = 80,
        P415_A02_DOOR_RIGHT = 81,
        P415_C02_DOOR1_RIGHT = 354,
        P415_C02_DOOR1_LEFT = 355,
        P415_C03_DOOR2_A = 356,
        P415_C03_DOOR2_B = 357,
        P415_C03_DOOR2_A_LEFT = 358,
        P415_C03_DOOR2_A_RIGHT = 359,
        P415_C03_DOOR2_B_LEFT = 360,
        P415_C03_DOOR2_B_RIGHT = 361,
        P341_A01_OPEN_DOCK_VARIATION1_DOORLEFT = 362,
        P341_A01_OPEN_DOCK_VARIATION1_DOORRIGHT = 363,
    }

    public class MyMwcObjectBuilder_PrefabKinematicPart : MyMwcObjectBuilder_PrefabBase
    {
        internal MyMwcObjectBuilder_PrefabKinematicPart()
            : base()
        {
        }

        public MyMwcObjectBuilder_PrefabKinematicPart(MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum prefabType, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance,
            MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth, float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority)
            : base((int)prefabType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {            
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabKinematicPart;
        }

        public MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum PrefabKinematicPartType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum)GetObjectBuilderId().Value;
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
