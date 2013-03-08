using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    public class MyMwcObjectBuilder_ObjectToBuild : MyMwcObjectBuilder_SubObjectBase
    {
        public const float DEFAULT_AMOUNT = 1f;

        public MyMwcObjectBuilder_Base ObjectBuilder { get; set; }
        public float Amount { get; set; }

        internal MyMwcObjectBuilder_ObjectToBuild()
            : base()
        {

        }

        public MyMwcObjectBuilder_ObjectToBuild(MyMwcObjectBuilder_Base objectBuilder, float amount)
        {
            ObjectBuilder = objectBuilder;
            Amount = amount;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            ObjectBuilder.RemapEntityIds(remapContext);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.ObjectToBuild;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);            

            //  Amount
            if (float.IsNaN(Amount) || float.IsInfinity(Amount))
            {
                System.Diagnostics.Debug.Fail("Amount is: " + Amount);
                Amount = DEFAULT_AMOUNT;
            }
            MyMwcLog.IfNetVerbose_AddToLog("Amount: " + Amount);
            MyMwcMessageOut.WriteFloat(Amount, binaryWriter);

            //  Object builder
            bool isObjectBuilder = ObjectBuilder != null;
            MyMwcMessageOut.WriteBool(isObjectBuilder, binaryWriter);
            if (isObjectBuilder) ObjectBuilder.Write(binaryWriter);            
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            
            //  Amount
            float? amount = MyMwcMessageIn.ReadFloat(binaryReader);
            if (amount == null) return NetworkError();
            Amount = amount.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Amount: " + Amount);
            if (float.IsNaN(Amount) || float.IsInfinity(Amount))
            {
                System.Diagnostics.Debug.Fail("Amount is: " + Amount);
                Amount = DEFAULT_AMOUNT;
            }

            //  Object builder
            bool? isObjectBuilder = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isObjectBuilder == null) return NetworkError();
            if (isObjectBuilder.Value)
            {
                ObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint);
                if (ObjectBuilder == null) return NetworkError();
                if (ObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                ObjectBuilder = null;
            }            

            return true;
        }
    }
}
