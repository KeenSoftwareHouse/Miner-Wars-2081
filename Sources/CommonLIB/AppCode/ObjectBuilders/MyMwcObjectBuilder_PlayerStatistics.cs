using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_PlayerStatistics : MyMwcObjectBuilder_Base
    {
        public int PlayersKilled { get; set; }
        public int BulletsShot { get; set; }
        public int HarvestedOre { get; set; }
        public int TunnelsDug { get; set; }
        public int TraveledDistance { get; set; }
        public int RescuedPlayers { get; set; }
        public float GamePlayTime { get; set; }
        public float OxygenSpent { get; set; }
        public int FriendlyFire { get; set; }
        public int Deaths { get; set; }

        internal MyMwcObjectBuilder_PlayerStatistics()
            : base()
        {
        }

        public MyMwcObjectBuilder_PlayerStatistics(int playersKilled, int bulletsShot, int harvestedOre, int tunnelsDug, int traveledDistance, int rescuedPlayers, 
            float gamePlayTime, float oxygenSpent, int friendlyFire, int deaths)
        {
            PlayersKilled = playersKilled;
            BulletsShot = bulletsShot;
            HarvestedOre = harvestedOre;
            TunnelsDug = tunnelsDug;
            TraveledDistance = traveledDistance;
            RescuedPlayers = rescuedPlayers;
            GamePlayTime = gamePlayTime;
            OxygenSpent = oxygenSpent;
            FriendlyFire = friendlyFire;
            Deaths = deaths;
        }

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Players killed
            MyMwcLog.IfNetVerbose_AddToLog("PlayersKilled: " + PlayersKilled);
            MyMwcMessageOut.WriteInt32(PlayersKilled, binaryWriter);

            // Bullets shot
            MyMwcLog.IfNetVerbose_AddToLog("BulletsShot: " + BulletsShot);
            MyMwcMessageOut.WriteInt32(BulletsShot, binaryWriter);

            // Harvested ore
            MyMwcLog.IfNetVerbose_AddToLog("HarvestedOre: " + HarvestedOre);
            MyMwcMessageOut.WriteInt32(HarvestedOre, binaryWriter);

            // Tunnels dug
            MyMwcLog.IfNetVerbose_AddToLog("TunnelsDug: " + TunnelsDug);
            MyMwcMessageOut.WriteInt32(TunnelsDug, binaryWriter);

            // Traveled distance
            MyMwcLog.IfNetVerbose_AddToLog("TraveledDistance: " + TraveledDistance);
            MyMwcMessageOut.WriteInt32(TraveledDistance, binaryWriter);

            // Rescued players
            MyMwcLog.IfNetVerbose_AddToLog("RescuedPlayers: " + RescuedPlayers);
            MyMwcMessageOut.WriteInt32(RescuedPlayers, binaryWriter);

            // Gameplay time
            MyMwcLog.IfNetVerbose_AddToLog("GamePlayTime: " + GamePlayTime);
            MyMwcMessageOut.WriteFloat(GamePlayTime, binaryWriter);

            // Oxygen spent
            MyMwcLog.IfNetVerbose_AddToLog("OxygenSpent: " + OxygenSpent);
            MyMwcMessageOut.WriteFloat(OxygenSpent, binaryWriter);

            // Friendly fire
            MyMwcLog.IfNetVerbose_AddToLog("FriendlyFire: " + FriendlyFire);
            MyMwcMessageOut.WriteInt32(FriendlyFire, binaryWriter);

            // Deads
            MyMwcLog.IfNetVerbose_AddToLog("Deaths: " + Deaths);
            MyMwcMessageOut.WriteInt32(Deaths, binaryWriter);
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Players killed
            int? playersKilled = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (playersKilled == null) return NetworkError();
            PlayersKilled = playersKilled.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PlayersKilled: " + PlayersKilled);

            // Bullets shot
            int? bulletsShot = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (bulletsShot == null) return NetworkError();
            BulletsShot = bulletsShot.Value;
            MyMwcLog.IfNetVerbose_AddToLog("BulletsShot: " + BulletsShot);

            // Harvested ore
            int? harvestedOre = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (harvestedOre == null) return NetworkError();
            HarvestedOre = harvestedOre.Value;
            MyMwcLog.IfNetVerbose_AddToLog("HarvestedOre: " + HarvestedOre);

            // Tunnels dug
            int? tunnelsDug = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (tunnelsDug == null) return NetworkError();
            TunnelsDug = tunnelsDug.Value;
            MyMwcLog.IfNetVerbose_AddToLog("TunnelsDug: " + TunnelsDug);

            // Traveled distance
            int? traveledDistance = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (traveledDistance == null) return NetworkError();
            TraveledDistance = traveledDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("TraveledDistance: " + TraveledDistance);

            // Rescued players
            int? rescuedPlayers = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (rescuedPlayers == null) return NetworkError();
            RescuedPlayers = rescuedPlayers.Value;
            MyMwcLog.IfNetVerbose_AddToLog("RescuedPlayers: " + RescuedPlayers);

            // Gameplay time
            float? gamePlayTime = MyMwcMessageIn.ReadFloat(binaryReader);
            if (gamePlayTime == null) return NetworkError();
            GamePlayTime = gamePlayTime.Value;
            MyMwcLog.IfNetVerbose_AddToLog("GamePlayTime: " + GamePlayTime);

            // Oxygen spent
            float? oxygenSpent = MyMwcMessageIn.ReadFloat(binaryReader);
            if (oxygenSpent == null) return NetworkError();
            OxygenSpent = oxygenSpent.Value;
            MyMwcLog.IfNetVerbose_AddToLog("OxygenSpent: " + OxygenSpent);

            // Friendly fire
            int? friendlyFire = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (friendlyFire == null) return NetworkError();
            FriendlyFire = friendlyFire.Value;
            MyMwcLog.IfNetVerbose_AddToLog("FriendlyFire: " + FriendlyFire);

            // Deads
            int? deaths = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (deaths == null) return NetworkError();
            Deaths = deaths.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Deaths: " + Deaths);

            return true;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.PlayerStatistics;
        }
    }
}
