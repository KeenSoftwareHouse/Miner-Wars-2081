using System;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    public class MyMwcObjectBuilder_Event : MyMwcObjectBuilder_SubObjectBase
    {
        public byte Status { get; set; }
        public byte EventType { get; set; }
        public DateTime Time { get; set; }
        public int EventTypeID { get; set; }

        internal MyMwcObjectBuilder_Event()
            : base()
        {
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Event;
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Status
            MyMwcLog.IfNetVerbose_AddToLog("Status: " + Status);
            MyMwcMessageOut.WriteByte(Status, binaryWriter);

            //  EventType
            MyMwcLog.IfNetVerbose_AddToLog("EventType: " + EventType);
            MyMwcMessageOut.WriteByte(EventType, binaryWriter);

            //  Time
            MyMwcLog.IfNetVerbose_AddToLog("Time: " + Time);
            MyMwcMessageOut.WriteDateTime(Time, binaryWriter);

            //  EventTypeID
            MyMwcLog.IfNetVerbose_AddToLog("EventTypeID: " + EventTypeID);
            MyMwcMessageOut.WriteInt32(EventTypeID, binaryWriter);
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion))
                return NetworkError();

            //  Status
            byte? status = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (status == null) return NetworkError();
            Status = status.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Status: " + Status);

            //  EventType
            byte? eventType = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (eventType == null) return NetworkError();
            EventType = eventType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("EventType: " + EventType);

            // Time
            DateTime? time = MyMwcMessageIn.ReadDateTimeEx(binaryReader, senderEndPoint);
            if (time == null) return NetworkError();
            Time = time.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Time: " + Time);

            // EventTypeID
            int? eventTypeID = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (eventTypeID == null) return NetworkError();
            EventTypeID = eventTypeID.Value;
            MyMwcLog.IfNetVerbose_AddToLog("EventTypeID: " + EventTypeID);

            return true;
        }
    }
}
