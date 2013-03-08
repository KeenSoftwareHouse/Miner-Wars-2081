using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs
{
    public class MyMwcObjectBuilder_EntityUseProperties : MyMwcObjectBuilder_SubObjectBase
    {
        public int UseType { get; set; }
        public int HackType { get; set; }
        public int HackingLevel { get; set; }
        public int HackingTime { get; set; }
        public bool IsHacked { get; set; }

        internal MyMwcObjectBuilder_EntityUseProperties()
            : base()
        {
        }

        public MyMwcObjectBuilder_EntityUseProperties(int useType, int hackType, int hackingLevel, int hackingTime, bool isHacked)
        {
            UseType = useType;
            HackType = hackType;
            HackingLevel = hackingLevel;
            HackingTime = hackingTime;
            IsHacked = isHacked;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.EntityUseProperties;
        }        

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Use Type
            MyMwcLog.IfNetVerbose_AddToLog("UseType: " + UseType);
            MyMwcMessageOut.WriteInt32(UseType, binaryWriter);

            //  Hack Type
            MyMwcLog.IfNetVerbose_AddToLog("HackType: " + HackType);
            MyMwcMessageOut.WriteInt32(HackType, binaryWriter);

            //  Hacking Level
            MyMwcLog.IfNetVerbose_AddToLog("HackingLevel: " + HackingLevel);
            MyMwcMessageOut.WriteInt32(HackingLevel, binaryWriter);

            //  Hacking Time
            MyMwcLog.IfNetVerbose_AddToLog("HackingTime: " + HackingTime);
            MyMwcMessageOut.WriteInt32(HackingTime, binaryWriter);

            //  Is Hacked
            MyMwcLog.IfNetVerbose_AddToLog("IsHacked: " + IsHacked);
            MyMwcMessageOut.WriteBool(IsHacked, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Use Type
            int? useType = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (useType == null) return NetworkError();
            UseType = useType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("UseType: " + UseType);

            //  Hack Type
            int? hackType = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (hackType == null) return NetworkError();
            HackType = hackType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("HackType: " + HackType);

            //  Hacking Level
            int? hackingLevel = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (hackingLevel == null) return NetworkError();
            HackingLevel = hackingLevel.Value;
            MyMwcLog.IfNetVerbose_AddToLog("HackingLevel: " + HackingLevel);

            //  Hacking Time
            int? hackingTime = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (hackingTime == null) return NetworkError();
            HackingTime = hackingTime.Value;
            MyMwcLog.IfNetVerbose_AddToLog("HackingTime: " + HackingTime);

            //  Is Hacked
            bool? isHacked = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isHacked == null) return NetworkError();
            IsHacked = isHacked.Value;
            MyMwcLog.IfNetVerbose_AddToLog("IsHacked: " + IsHacked);

            return true;
        }
    }
}
