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


namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools
{
    public class MyMwcObjectBuilder_SpawnPoint : MyMwcObjectBuilder_Object3dBase
    {
        public bool SpawnInGroups;      // whole group spawn
        public int SpawnCount;          // count of spawned bots
        public float FirstSpawnTimer;   // time in ms for respawn
        public float RespawnTimer;      // time in ms for respawn
        public float BoundingRadius;    // world radius for spawning / we generate ships within this radius randomly
        public bool Activated;

        private int m_numberOfTemplates;
        public List<MyMwcObjectBuilder_SmallShip_Bot> ShipTemplates;
        public MyMwcObjectBuilder_FactionEnum Faction;
        public String WayPointPath;
        public MyPatrolMode PatrolMode { get; set; }

        internal MyMwcObjectBuilder_SpawnPoint()
            : base()
        {
            SpawnInGroups = false;
            SpawnCount = -1;
            FirstSpawnTimer = 1.0f;
            RespawnTimer = 1.0f;
            BoundingRadius = 1.0f;
            ShipTemplates = new List<MyMwcObjectBuilder_SmallShip_Bot>();
            Faction = MyMwcObjectBuilder_FactionEnum.China;
            WayPointPath = "None";
            Activated = true;
            PatrolMode = MyPatrolMode.CYCLE;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            foreach (var template in ShipTemplates)
            {
                template.RemapEntityIds(remapContext);
            }
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SpawnPoint;
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // NumberOfTemplates (why?)
            int ?tempcount = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (tempcount == null) return NetworkError();
            m_numberOfTemplates = tempcount.Value;
            MyMwcLog.IfNetVerbose_AddToLog("NumberOfTemplates: " + m_numberOfTemplates);

            // Bot Templates
            for (int c = 0; c < m_numberOfTemplates; c++)
            {
                MyMwcObjectBuilder_SmallShip_Bot nb = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_SmallShip_Bot;
                if (nb == null || nb.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                ShipTemplates.Add(nb);
            }

            // WayPointPath
            string pWPP = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
            if (pWPP == null) return NetworkError();
            WayPointPath = pWPP;
            MyMwcLog.IfNetVerbose_AddToLog("WayPointPath: " + WayPointPath);

            // Faction
            int ?pFaction = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (pFaction == null) return NetworkError();
            Faction = (MyMwcObjectBuilder_FactionEnum)pFaction.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Faction: " + Faction);

            // SpawnInGroups
            bool? spawnInGroups = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (spawnInGroups == null) return NetworkError();
            SpawnInGroups = spawnInGroups.Value;
            MyMwcLog.IfNetVerbose_AddToLog("SpawnInGroups: " + SpawnInGroups);

            // SpawnCount
            int? spawnCount = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (spawnCount == null) return NetworkError();
            SpawnCount = spawnCount.Value;
            MyMwcLog.IfNetVerbose_AddToLog("SpawnCount: " + SpawnCount);

            // FirstSpawnTimer
            float? firstSpawnTimer = MyMwcMessageIn.ReadFloat(binaryReader);
            if (firstSpawnTimer == null) return NetworkError();
            FirstSpawnTimer = firstSpawnTimer.Value;
            MyMwcLog.IfNetVerbose_AddToLog("FirstSpawnTimer: " + FirstSpawnTimer);

            // RespawnTimer
            float? respawnTimer = MyMwcMessageIn.ReadFloat(binaryReader);
            if (respawnTimer == null) return NetworkError();
            RespawnTimer = respawnTimer.Value;
            MyMwcLog.IfNetVerbose_AddToLog("RespawnTimer: " + RespawnTimer);

            // BoundingRadius
            float? pBR = MyMwcMessageIn.ReadFloat(binaryReader);
            if (pBR == null) return NetworkError();
            BoundingRadius = pBR.Value;
            MyMwcLog.IfNetVerbose_AddToLog("BoundingRadius: " + BoundingRadius);

            // Activated
            bool? activated = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (activated == null) return NetworkError();
            Activated = activated.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Activated: " + Activated);

            // Patrol mode
            int? patrolMode = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (patrolMode == null) return NetworkError();
            PatrolMode = (MyPatrolMode)patrolMode.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PatrolMode: " + PatrolMode);
            
            return true;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            m_numberOfTemplates = ShipTemplates.Count;

            base.Write(binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("NumberOfTemplates: " + m_numberOfTemplates);
            MyMwcMessageOut.WriteInt32(m_numberOfTemplates, binaryWriter);

            for (int c = 0; c < m_numberOfTemplates; c++)
            {
                ShipTemplates[c].Write(binaryWriter);
            }

            // WayPointPath
            MyMwcLog.IfNetVerbose_AddToLog("WayPointPath: " + WayPointPath);
            MyMwcMessageOut.WriteString(WayPointPath, binaryWriter);

            // Faction
            MyMwcLog.IfNetVerbose_AddToLog("Faction: " + Faction);
            MyMwcMessageOut.WriteInt32((int)Faction, binaryWriter);

            // SpawnInGroups
            MyMwcLog.IfNetVerbose_AddToLog("SpawnInGroups: " + SpawnInGroups);
            MyMwcMessageOut.WriteBool(SpawnInGroups, binaryWriter);

            // SpawnCount
            MyMwcLog.IfNetVerbose_AddToLog("SpawnCount: " + SpawnCount);
            MyMwcMessageOut.WriteInt32(SpawnCount, binaryWriter);

            // FirstSpawnTimer
            MyMwcLog.IfNetVerbose_AddToLog("FirstSpawnTimer: " + FirstSpawnTimer);
            MyMwcMessageOut.WriteFloat(FirstSpawnTimer, binaryWriter);

            // RespawnTimer
            MyMwcLog.IfNetVerbose_AddToLog("RespawnTimer: " + RespawnTimer);
            MyMwcMessageOut.WriteFloat(RespawnTimer, binaryWriter);

            // BoundingRadius
            MyMwcLog.IfNetVerbose_AddToLog("BoundingRadius: " + BoundingRadius);
            MyMwcMessageOut.WriteFloat(BoundingRadius, binaryWriter);

            // Activated
            MyMwcLog.IfNetVerbose_AddToLog("Activated: " + Activated);
            MyMwcMessageOut.WriteBool(Activated, binaryWriter);

            // PatrolMode
            MyMwcLog.IfNetVerbose_AddToLog("PatrolMode: " + PatrolMode);
            MyMwcMessageOut.WriteByte((byte)PatrolMode, binaryWriter);
        }
    }
}


