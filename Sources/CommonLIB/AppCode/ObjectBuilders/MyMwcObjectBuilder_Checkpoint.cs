using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_Checkpoint : MyMwcObjectBuilder_Base
    {
        public MyMwcObjectBuilder_Checkpoint()
            : base()
        {
            Dictionary = new Dictionary<string, string>();
        }

        /// <summary>
        /// Is null when loading sector which is not in database
        /// </summary>
        public MyMwcObjectBuilder_Sector SectorObjectBuilder { get; set; }
        public MyMwcObjectBuilder_Player PlayerObjectBuilder { get; set; }

        public List<MyMwcObjectBuilder_Event> EventLogObjectBuilder { get; set; }
        public List<MyMwcObjectBuilder_FactionRelationChange> FactionRelationChangesBuilder { get; set; }

        public MyMwcObjectBuilder_Inventory InventoryObjectBuilder { get; set; }

        public Dictionary<string, string> Dictionary { get; set; }

        /// <summary>
        /// Gets or sets object builder of session which is parent of this checkpoint.
        /// </summary>
        public MyMwcObjectBuilder_Session SessionObjectBuilder { get; set; }

        public DateTime GameTime { get; set; }

        public int ActiveMissionID { get; set; }

        public MyMwcSectorIdentifier CurrentSector;

        [Obsolete("Checkpoint name is no longer used and is always null")]
        public string CheckpointName { get { return null; } set { } }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            SectorObjectBuilder.RemapEntityIds(remapContext);
            PlayerObjectBuilder.RemapEntityIds(remapContext);
            InventoryObjectBuilder.RemapEntityIds(remapContext);
            SessionObjectBuilder.RemapEntityIds(remapContext);
            foreach (var evnt in EventLogObjectBuilder)
            {
                evnt.RemapEntityIds(remapContext);
            }
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Checkpoint;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void WriteTrace()
        {
            if (SectorObjectBuilder != null)
            {
                SectorObjectBuilder.WriteTrace();
            }
            else
            {
                MyTrace.Send(TraceWindow.Saving, "No sector object builder");
            }
            InventoryObjectBuilder.WriteTrace();
        }

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Sector ob
            bool sectorObjectBuilderExists = SectorObjectBuilder != null;
            MyMwcMessageOut.WriteBool(sectorObjectBuilderExists, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Sector exists " + sectorObjectBuilderExists);

            if (sectorObjectBuilderExists)
            {
                SectorObjectBuilder.Write(binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("Sector written");
            }

            // Gametime
            MyMwcMessageOut.WriteDateTime(this.GameTime, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Gametime: " + this.GameTime);

            // Player ob
            bool playerObjectBuilderExists = PlayerObjectBuilder != null;
            MyMwcMessageOut.WriteBool(playerObjectBuilderExists, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Player exists " + playerObjectBuilderExists);

            if (playerObjectBuilderExists)
            {
                PlayerObjectBuilder.Write(binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("Player written");
            }

            // Session ob
            bool sessionObjectBuilderExists = SessionObjectBuilder != null;
            MyMwcMessageOut.WriteBool(sessionObjectBuilderExists, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Session exists " + sessionObjectBuilderExists);

            if (sessionObjectBuilderExists)
            {
                SessionObjectBuilder.Write(binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("Session written");
            }

            // Active Mission
            MyMwcMessageOut.WriteInt32(ActiveMissionID, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Active Mission ID: " + this.ActiveMissionID);

            //  Events
            int countEvents = EventLogObjectBuilder == null ? 0 : EventLogObjectBuilder.Count;
            MyMwcLog.IfNetVerbose_AddToLog("Count Events: " + countEvents);
            MyMwcMessageOut.WriteInt32(countEvents, binaryWriter);
            for (int i = 0; i < countEvents; i++)
            {
                EventLogObjectBuilder[i].Write(binaryWriter);
            }

            //  Faction relation changes
            int countFactionRelationChanges = FactionRelationChangesBuilder == null ? 0 : FactionRelationChangesBuilder.Count;
            MyMwcLog.IfNetVerbose_AddToLog("Faction Relation Changes : " + countFactionRelationChanges);
            MyMwcMessageOut.WriteInt32(countFactionRelationChanges, binaryWriter);
            for (int i = 0; i < countFactionRelationChanges; i++)
            {
                FactionRelationChangesBuilder[i].Write(binaryWriter);
            }

            // Inventory
            bool inventoryObjectBuilderExists = InventoryObjectBuilder != null;
            MyMwcMessageOut.WriteBool(inventoryObjectBuilderExists, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Inventory exists " + inventoryObjectBuilderExists);

            if (inventoryObjectBuilderExists)
            {
                InventoryObjectBuilder.Write(binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("Inventory written");
            }

            // Current sector identifier
            MyMwcMessageOut.WriteSectorIdentifier(this.CurrentSector, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Current sector: " + CurrentSector.ToString());

            // Checkpoint name
            bool hasName = CheckpointName != null;
            MyMwcMessageOut.WriteBool(hasName, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Has name: " + hasName);
            if (hasName)
            {
                MyMwcMessageOut.WriteString(CheckpointName, binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("Name: " + CheckpointName.ToString());
            }

            MyMessageHelper.WriteStringDictionary(Dictionary, binaryWriter);
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            // Sector ob
            bool? sectorBuilderExists = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!sectorBuilderExists.HasValue) return NetworkError();

            MyMwcLog.IfNetVerbose_AddToLog("Sector exists " + (SectorObjectBuilder != null));

            if (sectorBuilderExists.Value)
            {
                SectorObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_Sector;
                if (SectorObjectBuilder == null || !SectorObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("Sector read");
            }

            // Gametime
            DateTime? dateTime = MyMwcMessageIn.ReadDateTimeEx(binaryReader, senderEndPoint);
            if (!dateTime.HasValue) return NetworkError();
            this.GameTime = dateTime.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Gametime: " + this.GameTime);

            // Player ob
            bool? playerBuilderExists = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!playerBuilderExists.HasValue) return NetworkError();

            MyMwcLog.IfNetVerbose_AddToLog("Player exists " + (PlayerObjectBuilder != null));

            if (playerBuilderExists.Value)
            {
                PlayerObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_Player;
                if (PlayerObjectBuilder == null || !PlayerObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("Player read");
            }

            // Session ob
            bool? sessionBuilderExists = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!sessionBuilderExists.HasValue) return NetworkError();

            MyMwcLog.IfNetVerbose_AddToLog("Session exists " + (SessionObjectBuilder != null));

            if (sessionBuilderExists.Value)
            {
                SessionObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_Session;
                if (SessionObjectBuilder == null || !SessionObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("Session read");
            }

            // Active mission
            int? activeMissionID = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!activeMissionID.HasValue) return NetworkError();
            this.ActiveMissionID = activeMissionID.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Active Mission ID: " + this.ActiveMissionID);

            // Events
            int? countEvents = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countEvents == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("Count Events: " + countEvents);
            EventLogObjectBuilder = new List<MyMwcObjectBuilder_Event>(countEvents.Value);
            for (int i = 0; i < countEvents; i++)
            {
                var eventItem = ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_Event;
                if (eventItem == null) return NetworkError();
                if (eventItem.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                EventLogObjectBuilder.Add(eventItem);
            }

            // Faction relation changes
            int? countFactionRelationChanges = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countFactionRelationChanges == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("Factoin Relation Changes: " + countFactionRelationChanges);
            FactionRelationChangesBuilder = new List<MyMwcObjectBuilder_FactionRelationChange>(countFactionRelationChanges.Value);
            for (int i = 0; i < countFactionRelationChanges; i++)
            {
                var factionRelationChangeItem = ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_FactionRelationChange;
                if (factionRelationChangeItem == null) return NetworkError();
                if (factionRelationChangeItem.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                FactionRelationChangesBuilder.Add(factionRelationChangeItem);
            }

            // Inventory
            bool? inventoryBuilderExists = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!inventoryBuilderExists.HasValue) return NetworkError();

            MyMwcLog.IfNetVerbose_AddToLog("Inventory exists " + (InventoryObjectBuilder != null));

            if (inventoryBuilderExists.Value)
            {
                InventoryObjectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_Inventory;
                if (InventoryObjectBuilder == null || !InventoryObjectBuilder.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("Inventory read");
            }

            // Current sector identifier
            MyMwcSectorIdentifier? currentSector = MyMwcMessageIn.ReadSectorIdentifierEx(binaryReader, senderEndPoint);
            if (!currentSector.HasValue) return NetworkError();
            this.CurrentSector = currentSector.Value;

            // Checkpoint name
            bool? hasName = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasName.HasValue) return NetworkError();

            if (hasName.Value)
            {
                CheckpointName = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
                if (CheckpointName == null) return NetworkError(); // Has name set and name null? That's can't happen
            }
            else
            {
                CheckpointName = null;
            }

            Dictionary = MyMessageHelper.ReadStringDictionary(binaryReader, senderEndPoint);
            if (Dictionary == null) return NetworkError();

            return true;
        }
    }
}
