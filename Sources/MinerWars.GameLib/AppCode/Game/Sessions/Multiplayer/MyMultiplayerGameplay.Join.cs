using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using Lidgren.Network;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using System.Diagnostics;
using KeenSoftwareHouse.Library.Trace;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Sessions
{
    partial class MyMultiplayerGameplay
    {
        private List<MyPlayerInfo> m_playerListToSend = new List<MyPlayerInfo>(16);
        private List<MyEntity> m_entitiesToTransfer = new List<MyEntity>(64);

        private byte m_lastPlayerId = 0;


        private MyMwcObjectBuilder_Checkpoint GetCheckpoint()
        {
            var checkpoint = MySession.Static.GetCheckpointBuilder(true);

            MyGuiScreenGamePlay.Static.Checkpoint.CopyCoopPlayers(checkpoint);

            var ship = CreateShip(MySession.PlayerShip.ShipType, MySession.Static.Player.Faction);
            ship.PositionAndOrientation = new MyMwcPositionAndOrientation(MySession.PlayerShip.WorldMatrix);
            ship.EntityId = MySession.PlayerShip.EntityId.Value.NumericValue;
            ship.DisplayName = MyClientServer.LoggedPlayer.GetDisplayName().ToString();
            ship.ShipHealthRatio = MySession.PlayerShip.HealthRatio;
            ship.Inventory = GetInventory(MySession.PlayerShip, true);
            ship.PersistentFlags = MySession.PlayerShip.PersistentFlags;
            checkpoint.SectorObjectBuilder.SectorObjects.Add(ship);

            foreach (var player in Peers.Players)
            {
                if (player.Ship != null)
                {
                    var builder = player.Ship.GetObjectBuilder(true);
                    checkpoint.SectorObjectBuilder.SectorObjects.Add(builder);
                }
            }

            return checkpoint;
        }
        
        public void ReloadCheckpoint()
        {
            Log("ReloadCheckpoint");
            Debug.Assert(IsHost);

            foreach (var player in Peers.Players)
            {
                SendCheckpoint(player.Connection);
            }
        }

        void SendCheckpoint(NetConnection sendTo)
        {
            Log("SendCheckpoint");
            var checkpointEvent = new MyEventCheckpoint();
            checkpointEvent.Checkpoint = GetCheckpoint();

            Peers.NetworkClient.Send(ref checkpointEvent, sendTo, NetDeliveryMethod.ReliableOrdered, 0, 1024 * 1024);

            // Send missiles, cannon shots etc.
            m_entitiesToTransfer.Clear();
            MyEntities.FindEntities(AmmoEntitiesPredicate, m_entitiesToTransfer);
            foreach (var e in m_entitiesToTransfer)
            {
                var ammo = (MyAmmoBase)e;

                var weapon = MyGuiSmallShipHelpers.GetFirstWeaponType(ammo.AmmoType);
                if (!weapon.HasValue) continue;

                MyEventShoot shootMsg = new MyEventShoot();
                shootMsg.Ammo = ammo.AmmoType;
                shootMsg.Position = new MyMwcPositionAndOrientation(ammo.WorldMatrix);
                shootMsg.ProjectileEntityId = MyEntityIdentifier.ToNullableInt(ammo.EntityId);
                shootMsg.ShooterEntityId = (ammo.OwnerEntity != null && ammo.OwnerEntity.EntityId.HasValue) ? ammo.OwnerEntity.EntityId.Value.NumericValue : 0;
                shootMsg.TargetEntityId = null;
                shootMsg.Weapon = weapon.Value;
                Peers.NetworkClient.Send(ref shootMsg, sendTo, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        public event Action NewPlayer;
        void OnNewPlayer(ref MyEventNewPlayer msg)
        {
            Log("OnNewPlayer");

            if (NewPlayer != null)
            {
                NewPlayer();
            }

            if (IsHost)
            {
                SendCheckpoint(msg.SenderConnection);

                var playerLeftMsg = new MyEventPlayerStateChanged();
                playerLeftMsg.UserId = msg.PlayerInfo.UserId;
                playerLeftMsg.NewState = MyMultiplayerStateEnum.Playing;
                Peers.SendServer(ref playerLeftMsg);

                UpdateMission();
            }
            else
            {
                var player = new MyPlayerRemote(new StringBuilder(msg.PlayerInfo.DisplayName), msg.PlayerInfo.UserId, msg.PlayerInfo.PlayerId);
                player.Connection = msg.SenderConnection;
                player.Connection.Tag = player;
                Peers.Players.Add(player);
                Notify(MyTextsWrapperEnum.MP_XHasJoined, msg.PlayerInfo.DisplayName);
            }
            LogPlayers();
        }

        [Conditional("DEBUG")]
        void LogPlayers()
        {
            MyMwcLog.WriteLine("PLAYER LOG:");
            foreach (var player in Peers.Players)
            {
                string message = String.Format("User: {0,4} - {1,15}, GameId: {2,3}, EP: {3}, {4}", player.UserId, player.GetDisplayName(), player.PlayerId, player.Connection.RemoteEndpoint, player.Faction);
                MyTrace.Send(TraceWindow.MultiplayerAlerts, message);
                MyMwcLog.WriteLine(message);
            }
        }

        bool AmmoEntitiesPredicate(MyEntity entity)
        {
            return entity is MyAmmoBase;
        }


        byte GeneratePlayerId()
        {
            bool exists = true;
            while (exists)
            {
                unchecked
                {
                    m_lastPlayerId++;
                }
                if (m_lastPlayerId == 0)
                    m_lastPlayerId++;

                exists = false;
                foreach (var p in Peers.Players)
                {
                    if (p.PlayerId == m_lastPlayerId)
                    {
                        exists = true;
                        break;
                    }
                }
            }
            return m_lastPlayerId;
        }

        // Client is joining to ME and I'm host
        private void OnEnterGame(ref MyEventEnterGame msg)
        {
            Log("OnEnterGame");

            if (JoinMode == MyJoinMode.Open)
            {
                AllowEnter(ref msg);
            }
            else if (JoinMode == MyJoinMode.Closed)
            {
                DenyEnter(ref msg);
            }

        }

        public void DenyEnter(ref MyEventEnterGame msg)
        {
            var response = new MyEventEnterGameResponse();
            response.Allowed = false;
            Peers.NetworkClient.Send(ref response, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void AllowEnter(ref MyEventEnterGame msg)
        {
            var playerList = msg.ConnectedPlayers;
            bool allConnected = MyMultiplayerPeers.Static.Players.All(s => playerList.Contains(s.UserId));
            if (allConnected)
            {
                // Generate game user id and send back
                var response = new MyEventEnterGameResponse();
                response.Allowed = true;
                response.PlayerId = GeneratePlayerId();

                var newPlayer = new MyPlayerRemote(new StringBuilder(msg.PlayerInfo.DisplayName), msg.PlayerInfo.UserId, response.PlayerId);
                newPlayer.Connection = msg.SenderConnection;
                msg.SenderConnection.Tag = newPlayer;
                newPlayer.Faction = MyMwcObjectBuilder_FactionEnum.None;

                MyMultiplayerPeers.Static.Players.Add(newPlayer);

                Peers.NetworkClient.Send(ref response, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
            }
            else
            {
                SendPlayerList(msg.SenderConnection);
            }
        }

        // Client wants player list
        private void OnGetPlayerList(ref MyEventGetPlayerList msg)
        {
            SendPlayerList(msg.SenderConnection);
        }

        private void SendPlayerList(NetConnection connection)
        {
            var response = new MyEventGetPlayerListResponse();
            response.PlayerList = new List<MyPlayerInfo>(MyMultiplayerPeers.Static.Players.Select(s => RemotePlayerToInfo(s)));

            var me = new MyPlayerInfo()
            {
                DisplayName = MyClientServer.LoggedPlayer.GetDisplayName().ToString(),
                Faction = MySession.Static.Player.Faction,
                PlayerId = MyEntityIdentifier.CurrentPlayerId,
                UserId = MyClientServer.LoggedPlayer.GetUserId(),
            };
            response.PlayerList.Add(me);

            LogPlayerList(ref response);

            Peers.NetworkClient.Send(ref response, connection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        [Conditional("DEBUG")]
        public static void LogPlayerList(ref MyEventGetPlayerListResponse msg)
        {
            for (int i = 0; i < msg.PlayerList.Count; i++)
            {
                var p = msg.PlayerList[i];
                Log(String.Format("PlayerList[0] {0}, UserId: {3}, PlayerId: {2}, Faction: {1}", p.DisplayName, MyFactionConstants.GetFactionProperties(p.Faction).Name, p.PlayerId, p.UserId));
            }
        }

        private MyPlayerInfo RemotePlayerToInfo(MyPlayerRemote player)
        {
            return new MyPlayerInfo()
            {
                DisplayName = player.GetDisplayName().ToString(),
                PlayerId = player.PlayerId,
                UserId = player.UserId,
                Faction = player.Faction,
            };
        }
    }
}
