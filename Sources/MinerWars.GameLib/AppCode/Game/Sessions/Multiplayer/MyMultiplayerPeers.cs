using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Entities;
using System.Net;
using System.Threading;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers.Session;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Networking;

namespace MinerWars.AppCode.Game.Sessions.Multiplayer
{
    delegate void ConnectionHandler(NetConnection connection);
    delegate void PlayerHandler(MyPlayerRemote player);

    enum MyUpdateTypeId
    {
        Mission = 1,
        MissionTimer = 2,
    }

    class MyMultiplayerPeers
    {
        private static MyMultiplayerPeers m_instance;

        public static MyMultiplayerPeers Static
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MyMultiplayerPeers();
                }
                return m_instance;
            }
        }

        public readonly List<MyPlayerRemote> Players = new List<MyPlayerRemote>();

        public MyLidgrenPeer NetworkClient
        {
            get { return m_peer; }
        }

        public int HostUserId { get; set; }

        public bool IsStarted
        {
            get
            {
                return m_peer != null;
            }
        }

        public bool IsConnectedToServer
        {
            get
            {
                return m_serverConnection != null && m_serverConnection.Status != NetConnectionStatus.Disconnected && m_serverConnection.Status != NetConnectionStatus.None;
            }
        }

        NetConnection m_serverConnection;

        NetPeerConfiguration m_netConfig;
        MyLidgrenPeer m_peer;
        IPAddress m_localIp;

        List<Action> m_bufferedMessages = new List<Action>();
        List<NetConnection> m_playerConnections = new List<NetConnection>(32);
        Dictionary<UInt64, DateTime> m_lastUpdates = new Dictionary<UInt64, DateTime>(128);

        public event ConnectionHandler ServerConnected;
        public event ConnectionHandler ServerDisconnected;

        public event PlayerHandler PlayerDisconnected;

        public MyMultiplayerPeers()
        {
            IPAddress mask;
            m_localIp = NetUtility.GetMyAddress(out mask);

            m_netConfig = new NetPeerConfiguration(MyMwcNetworkingConstants.NETWORKING_MULTIPLAYER_ID);
            m_netConfig.Port = MyMwcNetworkingConstants.NETWORKING_PORT_MULTIPLAYER_PEER;
            m_netConfig.AcceptIncomingConnections = true;
            m_netConfig.SetMessageTypeEnabled(NetIncomingMessageType.NatIntroductionSuccess, true);
            m_netConfig.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true); // For NAT punch
            m_netConfig.MaximumTransmissionUnit = 1350; // just to be sure
            if (MyFakes.MULTIPLAYER_LONG_TIMEOUT)
            {
                m_netConfig.PingInterval = 60 * 10 - 2;
                m_netConfig.ConnectionTimeout = 60 * 10;
            }

// Because of lidgren
#if DEBUG
            if (MyFakes.MULTIPLAYER_SIMULATE_LAGS)
            {
                m_netConfig.SimulatedMinimumLatency = 0.1f; // 100ms minimum lag
                m_netConfig.SimulatedRandomLatency = 0.2f; // +/-200ms lag
                m_netConfig.SimulatedLoss = 0.05f; // 5% loss
                m_netConfig.SimulatedDuplicatesChance = 0.03f; // 3% duplicates
            }
#endif
        }

        public void Start()
        {
            Debug.Assert(m_peer == null, "Already started!");
            m_peer = new MyLidgrenPeer(m_netConfig);
            //m_peer.LogAll();
            m_peer.Log(MyEventEnum.MISSION_PROGRESS, MyLoggingTypeEnum.NAME);
            m_peer.MessageFilter = new MessageFilterHandler(MessageFilter);
            m_peer.Start();
            m_peer.PeerConnected += new Action<NetConnection>(m_peer_PeerConnected);
            m_peer.PeerDisconnected += new Action<NetConnection>(m_peer_PeerDisconnected);
        }

        /// <summary>
        /// Shuts down multiplayer networking (game, lobby, everything)
        /// </summary>
        /// <param name="waitForCompletion">Wait for completion (close socket). When you will initialize instance again immediatelly, set to true to prevent "Socket already bound exception".</param>
        public void Shutdown(bool waitForCompletion = false)
        {
            if (m_peer != null)
            {
                m_peer.Close();
            }

            if (m_peer != null && m_peer.Status != NetPeerStatus.ShutdownRequested && m_peer.Status != NetPeerStatus.NotRunning)
            {
                m_peer.Shutdown(String.Empty);

                if (waitForCompletion)
                {
                    m_peer.WaitForStatusCleared(NetPeerStatus.ShutdownRequested, TimeSpan.FromMilliseconds(200));
                }
            }
            m_bufferedMessages.Clear();
            ServerConnected = null;
            ServerDisconnected = null;
            PlayerDisconnected = null;
            m_serverConnection = null;
            m_peer = null;
        }

        internal void Restart()
        {
            HostUserId = -1;
            Shutdown(true);
            MyMultiplayerPeers.Static.Players.Clear();
            Start();
        }

        /// <summary>
        /// Connects to server
        /// </summary>
        private void ConnectServer()
        {
            if (m_serverConnection != null)
            {
                if (m_serverConnection.Status == NetConnectionStatus.Connected || m_serverConnection.Status == NetConnectionStatus.InitiatedConnect || m_serverConnection.Status == NetConnectionStatus.RespondedConnect)
                {
                    return; // Connection OK
                }
                // When disconnecting, wait
                m_serverConnection.WaitForStatusCleared(NetConnectionStatus.Disconnecting);
            }

            Debug.Assert(m_peer.Status == NetPeerStatus.Running, "Invalid networking state");

            var host = MyMwcFinalBuildConstants.MULTIPLAYER_HOST_ADDRESS ?? MyMwcFinalBuildConstants.MASTER_SERVER_ADDRESS;

            if (MySteam.IsActive)
            {
                var loginMsg = new MyEventLoginSteam();
                loginMsg.AppVersion = MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION;
                loginMsg.DisplayName = MySteam.UserName;
                loginMsg.SteamTicket = MySteam.SessionTicket;
                loginMsg.SteamUserId = MySteam.UserId;
                loginMsg.InternalEndpoint = new IPEndPoint(m_localIp, ((IPEndPoint)m_peer.Socket.LocalEndPoint).Port);
                m_serverConnection = m_peer.Connect(host, MyMwcNetworkingConstants.NETWORKING_PORT_MASTER_SERVER, ref loginMsg);
            }
            else
            {
                var loginMsg = new MyEventLogin();
                loginMsg.AppVersion = MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION;
                loginMsg.Username = MyClientServer.LoggedPlayer.UserName.ToString();
                loginMsg.PasswordHash = MyClientServer.LoggedPlayer.PasswordHash.ToString();
                loginMsg.InternalEndpoint = new IPEndPoint(m_localIp, ((IPEndPoint)m_peer.Socket.LocalEndPoint).Port);
                m_serverConnection = m_peer.Connect(host, MyMwcNetworkingConstants.NETWORKING_PORT_MASTER_SERVER, ref loginMsg);
            }
        }

        /// <summary>
        /// Disconnects from server
        /// </summary>
        public void DisconnectServer()
        {
            m_serverConnection.Disconnect(String.Empty);
            m_serverConnection = null;
        }

        public void DisconnectExceptServer()
        {
            foreach (var c in m_peer.Connections)
            {
                if (c != m_serverConnection && c.Status != NetConnectionStatus.Disconnected && c.Status != NetConnectionStatus.Disconnecting && c.Status != NetConnectionStatus.None)
                {
                    c.Disconnect(String.Empty);
                }
            }
        }

        public void RemovePlayer(int userId)
        {
            var player = Players.FirstOrDefault(p => p.UserId == userId);
            if (player != null)
            {
                Players.Remove(player);
            }
        }

        public MyPlayerRemote this[int userId]
        {
            get
            {
                return Players.First(p => p.GetUserId() == userId);
            }
        }

        public MyPlayerRemote this[byte playerId]
        {
            get
            {
                return Players.First(p => p.PlayerId == playerId);
            }
        }

        public bool TryGetPlayer(byte playerId, out MyPlayerRemote player)
        {
            player = Players.FirstOrDefault(p => p.PlayerId == playerId);
            return player != null;
        }

        public bool TryGetPlayer(int userId, out MyPlayerRemote player)
        {
            player = Players.FirstOrDefault(p => p.UserId == userId);
            return player != null;
        }

        /// <summary>
        /// Sends message to server, when not connected, it connects.
        /// Register ServerDisconnected to handle connection errors.
        /// Register ServerConnected to handle connection success.
        /// </summary>
        public NetSendResult SendServer<T>(ref T multiplayerEvent)
            where T : struct, IMyEvent
        {
            ConnectServer();
            if (m_serverConnection != null)
            {
                return m_peer.Send(ref multiplayerEvent, m_serverConnection, NetDeliveryMethod.ReliableOrdered, 0);
            }
            return NetSendResult.FailedNotConnected;
        }

        public NetSendResult SendHost<T>(ref T multiplayerEvent, int maxSize = MyLidgrenPeer.DEFAULT_MAX_SIZE)
            where T : struct, IMyEvent
        {
            var hostConnection = this[HostUserId].Connection;
            return m_peer.Send(ref multiplayerEvent, hostConnection, NetDeliveryMethod.ReliableOrdered, 0, maxSize);
        }

        public NetSendResult TrySendHost<T>(ref T multiplayerEvent, int maxSize = MyLidgrenPeer.DEFAULT_MAX_SIZE)
            where T : struct, IMyEvent
        {
            MyPlayerRemote host;
            if (TryGetPlayer(HostUserId, out host))
            {
                return m_peer.Send(ref multiplayerEvent, host.Connection, NetDeliveryMethod.ReliableOrdered, 0, maxSize);
            }
            return NetSendResult.FailedNotConnected;
        }

        //public NetSendResult SendHostRelayed<T>(ref T multiplayerEvent, NetDeliveryMethod delivery = NetDeliveryMethod.Unreliable, int sequenceChannel = 0, int maxSize = 512)
        //    where T : struct, IMyEvent
        //{
        //    ConnectServer();
        //    return m_peer.SendRelayed(ref multiplayerEvent, HostUserId, m_serverConnection, delivery, sequenceChannel, maxSize);
        //}

        public void SendToAll<T>(ref T multiplayerEvent, NetDeliveryMethod delivery = NetDeliveryMethod.Unreliable, int sequenceChannel = 0, int maxSize = 512)
            where T : struct, IMyEvent
        {
            m_playerConnections.Clear();
            foreach (var p in Players)
            {
                var connection = p.Connection;
                if (connection != null)
                {
                    if (connection is MyRelayedConnection)
                    {
                        m_peer.Send(ref multiplayerEvent, connection, delivery, sequenceChannel, maxSize);
                    }
                    else
                    {
                        m_playerConnections.Add(connection);
                    }
                }
            }
            if (m_playerConnections.Count > 0)
            {
                m_peer.SendToAll(ref multiplayerEvent, m_playerConnections, delivery, sequenceChannel, maxSize);
            }
        }

        public void SendToTeam<T>(ref T multiplayerEvent, MyMwcObjectBuilder_FactionEnum senderFaction, NetDeliveryMethod delivery = NetDeliveryMethod.Unreliable, int sequenceChannel = 0, int maxSize = 512)
            where T : struct, IMyEvent
        {
            m_playerConnections.Clear();
            foreach (var p in Players)
            {
                var connection = p.Connection;
                if (connection != null && MyFactions.GetFactionsRelation(p.Faction, senderFaction) == MyFactionRelationEnum.Friend)
                {
                    if (connection is MyRelayedConnection)
                    {
                        m_peer.Send(ref multiplayerEvent, connection, delivery, sequenceChannel, maxSize);
                    }
                    else
                    {
                        m_playerConnections.Add(connection);
                    }
                }
            }
            if (m_playerConnections.Count > 0)
            {
                m_peer.SendToAll(ref multiplayerEvent, m_playerConnections, delivery, sequenceChannel);
            }
        }

        public void Update()
        {
            if (IsStarted)
            {
                // Slowly clear update dictionary
                var now = DateTime.Now;
                UInt64? keyToRemove = null;
                foreach (var pair in m_lastUpdates)
                {
                    if ((now - pair.Value).TotalSeconds > 1)
                    {
                        keyToRemove = pair.Key;
                    }
                }
                if (keyToRemove.HasValue)
                {
                    m_lastUpdates.Remove(keyToRemove.Value);
                }

                m_peer.Receive();
            }
        }

        /// <summary>
        /// Sends message to all players, but only when it does not exceed maxSendRate.
        /// MaxSendRate is number of updates of this entity per second.
        /// Note: MessageId is no taken into account
        /// </summary>
        public void SendToAll<T>(ref T multiplayerEvent, MyEntityIdentifier entityId, float maxSendRate, NetDeliveryMethod delivery = NetDeliveryMethod.Unreliable, int sequenceChannel = 0)
            where T : struct, IMyEvent
        {
            if (CanUpdate(entityId.NumericValue, null, maxSendRate))
            {
                SendToAll(ref multiplayerEvent, delivery, sequenceChannel);
            }
        }

        /// <summary>
        /// Sends message to all players, but only when it does not exceed maxSendRate.
        /// MaxSendRate is number of updates of this entity per second.
        /// Note: MessageId is no taken into account
        /// </summary>
        public void SendToAll<T>(ref T multiplayerEvent, MyUpdateTypeId updateTypeId, uint internalId, float maxSendRate, NetDeliveryMethod delivery = NetDeliveryMethod.Unreliable, int sequenceChannel = 0)
            where T : struct, IMyEvent
        {
            if (CanUpdate(internalId, updateTypeId, maxSendRate))
            {
                SendToAll(ref multiplayerEvent, delivery, sequenceChannel);
            }
        }

        /// <summary>
        /// True when message should be passed to handler.
        /// False to prevent handler.
        /// </summary>
        bool MessageFilter(NetConnection connection, IPEndPoint endpoint, MyEventEnum eventType)
        {
            MyPlayerRemote player = connection.Tag as MyPlayerRemote;
            if (player != null)
            {
                // Accept known players
                return true;
            }
            else if (connection == m_serverConnection)
            {
                // Accept server messages
                return true;
            }
            else if (connection != null && (eventType == MyEventEnum.NEW_PLAYER || eventType == MyEventEnum.ENTER_GAME || eventType == MyEventEnum.GET_PLAYER_LIST || eventType == MyEventEnum.GET_PLAYER_LIST_RESPONSE))
            {
                // Accept incoming new players
                return true;
            }
            return false;
        }

        UInt64 PackKey(uint? entityId, MyUpdateTypeId? lastUpdateId)
        {
            UInt64 id = entityId ?? 0;
            id *= UInt32.MaxValue;
            id += (uint)(lastUpdateId ?? 0);
            return id;
        }

        bool CanUpdate(uint? entityId, MyUpdateTypeId? updateId, float updateRate = 20)
        {
            float minTimespanMs = 1000.0f / updateRate;

            var key = PackKey(entityId, updateId);

            DateTime now = DateTime.Now;
            DateTime lastUpdate;
            if (m_lastUpdates.TryGetValue(key, out lastUpdate))
            {
                if ((now - lastUpdate).TotalMilliseconds > minTimespanMs)
                {
                    m_lastUpdates[key] = now;
                    return true;
                }
                return false;
            }
            else
            {
                m_lastUpdates[key] = now;
                return true;
            }
        }

        void m_peer_PeerConnected(NetConnection obj)
        {
            if (obj == m_serverConnection)
            {
                NetworkClient.RelayServerConnection = m_serverConnection;

                var handler = ServerConnected;
                if (handler != null)
                {
                    handler(obj);
                }
            }
        }

        void m_peer_PeerDisconnected(NetConnection connection)
        {
            if (connection == null) return;

            MyPlayerRemote player = connection.Tag as MyPlayerRemote;

            if (connection == m_serverConnection)
            {
                NetworkClient.RelayServerConnection = null;
                var handler = ServerDisconnected;
                if (handler != null)
                {
                    handler(connection);
                }
            }
            else if (player != null && Players.Contains(player))
            {
                Players.Remove(player);

                var handler = PlayerDisconnected;
                if (handler != null)
                {
                    handler(player);
                }
            }
        }
    }
}
