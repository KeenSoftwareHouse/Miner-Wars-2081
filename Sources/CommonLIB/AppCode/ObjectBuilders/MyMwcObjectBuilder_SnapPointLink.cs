using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Data.SqlClient;
using System.Net;
using System.IO;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_SnapPointLink : MyMwcObjectBuilder_Base
    {
        public class LinkElement
        {
            public uint EntityId { get; set; }
            public short Index { get; set; }
            public string SnapPointName { get; set; }

            public LinkElement(uint entityId, short index, string snapPointName)
            {
                EntityId = entityId;
                Index = index;
                SnapPointName = snapPointName;
            }
        }

        public List<LinkElement> Links { get; set; }

        internal MyMwcObjectBuilder_SnapPointLink()
            : base()
        {
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            foreach (var element in Links)
            {
                element.EntityId = remapContext.RemapEntityId(element.EntityId).Value;
            }
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  ObjectList
            int countLinks = Links == null ? 0 : Links.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countLinks: " + countLinks);
            MyMwcMessageOut.WriteInt32(countLinks, binaryWriter);
            for (int i = 0; i < countLinks; i++)
            {
                MyMwcLog.IfNetVerbose_AddToLog(string.Format("Links[{0}].EntityId: {1}", i, Links[i].EntityId));
                MyMwcMessageOut.WriteInt32((int)Links[i].EntityId, binaryWriter);

                MyMwcLog.IfNetVerbose_AddToLog(string.Format("Links[{0}].Index: {1}", i, Links[i].Index));
                MyMwcMessageOut.WriteInt16(Links[i].Index, binaryWriter);
                if (Links[i].SnapPointName != null)
                {
                    MyMwcMessageOut.WriteBool(true, binaryWriter);
                    MyMwcLog.IfNetVerbose_AddToLog(string.Format("Links[{0}].SnapPointName: {1}", i, Links[i].SnapPointName));
                    MyMwcMessageOut.WriteString(Links[i].SnapPointName, binaryWriter);
                }
                else
                {
                    MyMwcMessageOut.WriteBool(false, binaryWriter);
                    MyMwcLog.IfNetVerbose_AddToLog(string.Format("Links[{0}].SnapPointName: {1}", i, "null"));
                }
            }
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            // ObjectList
            int? countLinks = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countLinks == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countLinks: " + countLinks);
            Links = new List<LinkElement>(countLinks.Value);
            for (int i = 0; i < countLinks; i++)
            {
                int? entityId = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
                if (entityId == null) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog(string.Format("Links[{0}].EntityId: {1}", i, entityId));

                short? index = MyMwcMessageIn.ReadInt16Ex(binaryReader, senderEndPoint);
                if (index == null) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog(string.Format("Links[{0}].Index: {1}", i, index));

                bool? hasSnapPointName = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
                if (!hasSnapPointName.HasValue) return NetworkError();
                if (hasSnapPointName.Value)
                {
                    string snapPointName = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
                    if (snapPointName == null) return NetworkError();
                    MyMwcLog.IfNetVerbose_AddToLog(string.Format("Links[{0}].SnapPointName: {1}", i, snapPointName));
                    Links.Add(new LinkElement((uint)entityId.Value, index.Value, snapPointName));
                }
                else
                {
                    MyMwcLog.IfNetVerbose_AddToLog("Links[{0}].SnapPointName: null");
                    Links.Add(new LinkElement((uint)entityId.Value, index.Value, null));
                }
            }

            return true;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SnapPointLink;
        }
    }
}
