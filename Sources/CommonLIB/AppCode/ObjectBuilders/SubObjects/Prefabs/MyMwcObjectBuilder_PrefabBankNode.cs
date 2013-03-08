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
    public enum MyMwcObjectBuilder_PrefabBankNode_TypesEnum : ushort
    {
        DEFAULT = 0,
    }

    public class MyMwcObjectBuilder_PrefabBankNode : MyMwcObjectBuilder_PrefabBase
    {
        public float Cash { get; set; }

        internal MyMwcObjectBuilder_PrefabBankNode()
            : base()
        {            
        }

        public MyMwcObjectBuilder_PrefabBankNode(MyMwcObjectBuilder_PrefabBankNode_TypesEnum bankNodeType,
            MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyMwcVector3Short position, Vector3 anglesInContainer, float? prefabMaxHealth,
            float prefabHealthRatio, string displayName, float electricCapacity, bool? causesAlarm, int aiPriority, float cash)
            : base((int)bankNodeType, appearance, position, anglesInContainer, prefabMaxHealth, prefabHealthRatio, displayName, electricCapacity, causesAlarm, aiPriority)
        {
            Cash = cash;
        }

        /// <summary>
        /// GetObjectBuilderType
        /// </summary>
        /// <returns></returns>
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PrefabBankNode;
        }

        public MyMwcObjectBuilder_PrefabBankNode_TypesEnum BankNodeType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabBankNode_TypesEnum)GetObjectBuilderId().Value;
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
            
            //  Cash
            MyMwcLog.IfNetVerbose_AddToLog("Cash: " + Cash);
            MyMwcMessageOut.WriteFloat(Cash, binaryWriter);            
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();           

            //  Cash
            float? cash = MyMwcMessageIn.ReadFloat(binaryReader);
            if (cash == null) return NetworkError();
            Cash = cash.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Cash: " + Cash);

            return true;
        }
    }
}
