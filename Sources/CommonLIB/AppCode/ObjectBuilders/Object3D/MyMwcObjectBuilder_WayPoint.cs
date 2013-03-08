using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using MinerWars.AppCode.Game;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public class MyMwcObjectBuilder_WaypointNew : MyMwcObjectBuilder_Object3dBase
    {
        public int? ParentEntityId;
        public List<int> NeighborEntityIds;
        public List<string> GroupNames;
        public List<int> GroupPlacings;

        internal MyMwcObjectBuilder_WaypointNew()
            : base()
        {
            ParentEntityId = null;
            NeighborEntityIds = new List<int>();
            GroupNames = new List<string>();
            GroupPlacings = new List<int>();
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            ParentEntityId = (int?)remapContext.RemapEntityId((uint?)ParentEntityId);
            for (int i = 0; i < NeighborEntityIds.Count; i++)
            {
                NeighborEntityIds[i] = (int)remapContext.RemapEntityId((uint)NeighborEntityIds[i]).Value;
            }
            for (int i = 0; i < GroupNames.Count; i++)
            {
                GroupNames[i] = remapContext.RemapWaypointGroupName(GroupNames[i]);
            }
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.WaypointNew;
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcMessageOut.WriteBool(ParentEntityId.HasValue, binaryWriter);
            if (ParentEntityId.HasValue)
            {
                MyMwcLog.IfNetVerbose_AddToLog("Waypoint parent id: " + ParentEntityId.Value.ToString());
                MyMwcMessageOut.WriteInt32(ParentEntityId.Value, binaryWriter);
            }

            MyMwcMessageOut.WriteInt32(NeighborEntityIds.Count, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Neighbor count: " + NeighborEntityIds.Count.ToString());
            for (int i = 0; i < NeighborEntityIds.Count; i++)
            {
                MyMwcMessageOut.WriteInt32(NeighborEntityIds[i], binaryWriter);
            }

            System.Diagnostics.Debug.Assert(GroupPlacings.Count == GroupNames.Count);
            MyMwcMessageOut.WriteInt32(GroupPlacings.Count, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Group count: " + GroupPlacings.Count.ToString());
            for (int i = 0; i < GroupPlacings.Count; i++)
            {
                MyMwcMessageOut.WriteString(GroupNames[i], binaryWriter);
                MyMwcMessageOut.WriteInt32(GroupPlacings[i], binaryWriter);
            }
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            bool? hasParent = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint); if (!hasParent.HasValue) return NetworkError();
            ParentEntityId = null;
            if (hasParent.Value)
            {
                int? id = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint); if (!id.HasValue) return NetworkError();
                ParentEntityId = id.Value;
                MyMwcLog.IfNetVerbose_AddToLog("Waypoint parent id: " + ParentEntityId.Value.ToString());
            }

            int? nCount = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint); if (!nCount.HasValue) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("Neighbor count: " + nCount.Value.ToString());
            NeighborEntityIds = new List<int>();
            for (int i = 0; i < nCount.Value; i++)
            {
                int? index = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint); if (!index.HasValue) return NetworkError();
                NeighborEntityIds.Add(index.Value);
            }

            int? gCount = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint); if (!nCount.HasValue) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("Group count: " + gCount.Value.ToString());
            GroupNames = new List<string>();
            GroupPlacings = new List<int>();
            for (int i = 0; i < gCount.Value; i++)
            {
                string name = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint); if (name == null) return NetworkError();
                GroupNames.Add(name);
                int? placing = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint); if (!placing.HasValue) return NetworkError();
                GroupPlacings.Add(placing.Value);
            }

            return true;
        }
    }
}
