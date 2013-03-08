using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.CommonLIB.AppCode.Networking;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Net;
using SysUtils;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using KeenSoftwareHouse.Library.Trace;
using SysUtils.Utils;
using System.Diagnostics;
using System.Timers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Networking.SectorService;

namespace MinerWars.AppCode.Game.Sessions
{
    delegate void GameCreatedHandler(MyResultCodeEnum resultCode);
    delegate void GameListReceivedHandler(List<MyGameInfo> games);
    delegate void GameJoinedHandler(MyGameInfo game, MyResultCodeEnum resultCode, MyMwcObjectBuilder_Checkpoint checkpointBuilder);

    class MyMultiplayerLobby
    {
        private GameCreatedHandler m_createGameAction;
        private GameListReceivedHandler m_gameListReceivedAction;
        private GameJoinedHandler m_gameJoinedAction;
        private Action m_gameEnterDisallowedAction;
        private Action m_gameDownloadingSector;

        private MyNatIntroduction m_natIntroduction;

        private MyMwcObjectBuilder_FactionEnum m_faction;

        private int m_introductionTryCount = 0;
        private Timer m_introductionTimer = new Timer();
        private Timer m_hostTimer = new Timer();

        private static MyMultiplayerLobby m_instance;

        private MyGameInfo m_game;

        private object m_syncRoot = new object();

        public static MyMultiplayerLobby Static
        {
            get
            {
                return m_instance ?? (m_instance = new MyMultiplayerLobby());
            }
        }

        public void HostGame(string sectorName, string password, MyGameTypes gameType, GameCreatedHandler onGameCreated, MyJoinMode joinMode, MyGameplayDifficultyEnum difficulty)
        {
            Debug.Assert(!MyMultiplayerGameplay.IsRunning);

            MyMultiplayerPeers.Static.Restart();
            m_introductionTimer.Dispose();
            m_hostTimer.Dispose();

            m_createGameAction = onGameCreated;

            MyEventCreateGame createGameRequest = new MyEventCreateGame();
            createGameRequest.SectorName = sectorName;
            createGameRequest.Password = password;
            createGameRequest.Type = gameType;
            createGameRequest.JoinMode = joinMode;
            createGameRequest.Difficulty = difficulty;
            MyMultiplayerGameplay.Static.JoinMode = joinMode;
            SetCallback<MyEventCreateGameResponse>(OnCreateGameResponse);
            MyMultiplayerPeers.Static.NetworkClient.RegisterCallback<MyEventLoginResponse>(OnLoginResponse);

            m_hostTimer = new Timer();
            m_hostTimer.AutoReset = false;
            m_hostTimer.Elapsed += new ElapsedEventHandler(m_hostTimer_Elapsed);
            m_hostTimer.Interval = 10000;
            m_hostTimer.Start();

            MyMultiplayerGameplay.Static.LastCreateGameRequest = createGameRequest;
            MyMultiplayerPeers.Static.SendServer(ref createGameRequest);
        }

        void m_hostTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_hostTimer.Dispose();

            var handler = m_createGameAction;
            if (handler != null)
            {
                handler(MyResultCodeEnum.TIMEOUT);
            }
        }

        public void GetGames(GameListReceivedHandler onGameListReceived, string filter, MyGameTypes gameTypeFilter)
        {
            MyMultiplayerPeers.Static.Restart();
            m_introductionTimer.Dispose();
            m_hostTimer.Dispose();

            m_gameListReceivedAction = onGameListReceived;

            MyEventGetGames gamesRequest = new MyEventGetGames();
            gamesRequest.NameFilter = filter;
            gamesRequest.GameTypeFilter = gameTypeFilter;
            SetCallback<MyEventGetGamesResponse>(OnGetGamesResponse);

            MyMultiplayerPeers.Static.SendServer(ref gamesRequest);
        }

        public void CancelGetGames()
        {
            m_gameListReceivedAction = null;
        }

        public void OnLoginResponse(ref MyEventLoginResponse msg)
        {
            if (msg.MultiplayerUserId != 0)
            {
                MyMwcLog.WriteLine("Setting multiplayer user id: " + msg.MultiplayerUserId);
                MyClientServer.LoggedPlayer.SetMultiplayerUserId(msg.MultiplayerUserId);
            }
        }

        public void JoinGame(MyGameInfo gameInfo, string password, GameJoinedHandler onGameJoined, Action onGameEnterDisallowed, Action onDownloadingSector)
        {
            MyMultiplayerPeers.Static.Restart();
            m_introductionTimer.Dispose();
            m_hostTimer.Dispose();

            MyMultiplayerPeers.Static.HostUserId = -1;
            m_game = gameInfo;

            m_gameJoinedAction = onGameJoined;
            m_gameDownloadingSector = onDownloadingSector;
            m_gameEnterDisallowedAction = onGameEnterDisallowed;
            MyMultiplayerPeers.Static.NetworkClient.ClearCallbacks();
            MyMultiplayerPeers.Static.DisconnectExceptServer();

            MyMultiplayerPeers.Static.Players.Clear();

            MyEventJoinGame joinRequest = new MyEventJoinGame();
            joinRequest.GameId = gameInfo.GameId;
            joinRequest.Password = password;

            SetCallback<MyEventJoinGameResponse>(OnJoinGameResponse);
            MyMultiplayerPeers.Static.NetworkClient.RegisterCallback<MyEventLoginResponse>(OnLoginResponse);

            MyMultiplayerPeers.Static.NetworkClient.NatIntroductionSuccess = NetworkClient_NatIntroductionSuccess;

            m_natIntroduction = new MyNatIntroduction(MyMultiplayerPeers.Static.NetworkClient);
            MyMultiplayerPeers.Static.NetworkClient.PeerConnected += new Action<NetConnection>(NetworkClient_PeerConnected);
            MyMultiplayerPeers.Static.SendServer(ref joinRequest);

            MyMultiplayerGameplay.Log("JOIN 1. - Join game");
        }

        void NetworkClient_PeerConnected(NetConnection obj)
        {
            MyMultiplayerGameplay.Log("Connection successful, EP: " + obj.RemoteEndpoint.ToString());
            UpdateLobby();
        }

        #region CALLBACKS
        private void ClearCallbacks()
        {
            MyMultiplayerPeers.Static.NetworkClient.RemoveCallback<MyEventCreateGameResponse>();
            MyMultiplayerPeers.Static.NetworkClient.RemoveCallback<MyEventGetGamesResponse>();
            MyMultiplayerPeers.Static.NetworkClient.RemoveCallback<MyEventJoinGameResponse>();
            MyMultiplayerPeers.Static.NetworkClient.RemoveCallback<MyEventGetPlayerListResponse>();
            MyMultiplayerPeers.Static.NetworkClient.RemoveCallback<MyEventEnterGameResponse>();
            MyMultiplayerPeers.Static.NetworkClient.RemoveCallback<MyEventCheckpoint>();
        }

        private void SetCallback<T>(EventCallback<T> callback)
            where T : struct, IMyEvent
        {
            ClearCallbacks();
            MyMultiplayerPeers.Static.NetworkClient.RegisterCallback<T>(callback);
        }

        private void OnCreateGameResponse(ref MyEventCreateGameResponse msg)
        {
            m_hostTimer.Dispose();

            var handler = m_createGameAction;
            if (handler != null)
            {
                handler(MyResultCodeEnum.OK);
            }
        }

        private void OnGetGamesResponse(ref MyEventGetGamesResponse msg)
        {
            var handler = m_gameListReceivedAction;
            if (handler != null)
            {
                handler(msg.Games);
            }
        }

        private void OnJoinGameResponse(ref MyEventJoinGameResponse msg)
        {
            // Report errors
            if (msg.ResultCode != MyResultCodeEnum.OK)
            {
                var handler = m_gameJoinedAction;
                if (handler != null)
                {
                    handler(m_game, msg.ResultCode, null);
                }
                return;
            }

            MyMultiplayerGameplay.Log("JOIN 2.a - Join game success (#JoinGameResponse)");

            MyMultiplayerPeers.Static.NetworkClient.NatIntroductionSuccess = NetworkClient_NatIntroductionSuccessHost;
            MyMultiplayerPeers.Static.HostUserId = msg.HostUserId;

            lock (m_syncRoot)
            {
                m_introductionTryCount = 0;
                m_introductionTimer = new Timer();
                m_introductionTimer.AutoReset = true;
                m_introductionTimer.Elapsed += new ElapsedEventHandler(m_hostIntroductionTimer_Elapsed);
                m_introductionTimer.Interval = 900; // 900ms
                m_introductionTimer.Start();
            }

            RequestHostIntroduction(m_game.GameId, false);
        }

        void m_hostIntroductionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (m_syncRoot)
            {
                m_introductionTryCount++;
                if (m_introductionTryCount > 5)
                {
                    m_introductionTimer.Dispose();

                    if (MyFakes.ENABLE_MULTIPLAYER_RELAY)
                    {
                        // Use relaying, need to get host public EP first
                        MyMultiplayerPeers.Static.NetworkClient.NatIntroductionSuccess = null;
                        MyMultiplayerPeers.Static.NetworkClient.RegisterCallback<MyEventDirectIntroductionResponse>(OnHostDirectIntroduction);
                        RequestHostIntroduction(m_game.GameId, true);
                    }
                }
                else
                {
                    RequestHostIntroduction(m_game.GameId, false);
                }
            }
        }

        void RequestHostIntroduction(uint gameId, bool relay)
        {
            // Allow only relayed introduction
            if (MyFakes.MULTIPLAYER_RELAY_TEST && !relay)
                return;

            var introRequest = new MyEventRequestIntroduction();
            introRequest.GameId = gameId;
            introRequest.UserList = new List<int>();
            introRequest.RelayIntroduction = relay;
            MyMultiplayerPeers.Static.SendServer(ref introRequest);

            MyMultiplayerGameplay.Log("JOIN 2.b - Requesting NAT introduction to host, try no: " + m_introductionTryCount);
        }

        void NetworkClient_NatIntroductionSuccessHost(IPEndPoint endpoint, string token)
        {
            int userId;
            if (!int.TryParse(token, out userId))
                return;

            if (userId == MyMultiplayerPeers.Static.HostUserId)
            {
                // NAT intro from host received
                m_introductionTimer.Dispose();

                MyMultiplayerPeers.Static.NetworkClient.NatIntroductionSuccess = NetworkClient_NatIntroductionSuccess;

                MyEventGetPlayerList msg = new MyEventGetPlayerList();
                SetCallback<MyEventGetPlayerListResponse>(OnGetPlayerListResponse);
                MyMultiplayerPeers.Static.NetworkClient.Connect(endpoint, ref msg);
                MyMultiplayerGameplay.Log("JOIN 3. - Host NAT introduction success, sending player list request");
            }
        }

        void OnHostDirectIntroduction(ref MyEventDirectIntroductionResponse msg)
        {
            MyMultiplayerPeers.Static.NetworkClient.NatIntroductionSuccess = NetworkClient_NatIntroductionSuccess;
            MyEventGetPlayerList getPlayersMsg = new MyEventGetPlayerList();
            SetCallback<MyEventGetPlayerListResponse>(OnGetPlayerListResponse);

            var hostRelayConnection = MyMultiplayerPeers.Static.NetworkClient.GetRelayedConnection(msg.EndPoint);
            MyMultiplayerPeers.Static.NetworkClient.Send(ref getPlayersMsg, hostRelayConnection, NetDeliveryMethod.ReliableOrdered, 0);

            MyMultiplayerGameplay.Log("JOIN 3. - Host NAT introduction failure, RELAYING, sending player list request");
        }

        void OnGetPlayerListResponse(ref MyEventGetPlayerListResponse msg)
        {
            MyMultiplayerGameplay.LogPlayerList(ref msg);

            // Disconnect from players (connection to host is not dropped)
            foreach (var player in MyMultiplayerPeers.Static.Players)
            {
                if (player.UserId != MyMultiplayerPeers.Static.HostUserId)
                {
                    player.Connection.Disconnect(String.Empty);
                }
            }
            MyMultiplayerPeers.Static.Players.Clear();

            // Start connecting players again
            foreach (var playerInfo in msg.PlayerList)
            {
                var playerRemote = new MyPlayerRemote(new StringBuilder(playerInfo.DisplayName), playerInfo.UserId, playerInfo.PlayerId);
                if (playerRemote.UserId == MyMultiplayerPeers.Static.HostUserId)
                {
                    playerRemote.Connection = msg.SenderConnection;
                    msg.SenderConnection.Tag = playerRemote;
                }
                MyMultiplayerPeers.Static.Players.Add(playerRemote);
            }

            MyMultiplayerGameplay.Log("JOIN 4.a - Player list received (#OnGetPlayerListResponse)");

            m_natIntroduction = new MyNatIntroduction(MyMultiplayerPeers.Static.NetworkClient);
            m_natIntroduction.SetRequiredPlayers(MyMultiplayerPeers.Static.Players);

            ClearCallbacks();

            var userListCopy = MyMultiplayerPeers.Static.Players.Where(s => s != null).Select(s => s.UserId).ToList();

            StartClientIntroductionTimer(userListCopy);
            RequestPlayerIntroduction(userListCopy, false);
            UpdateLobby();
        }

        private void StartClientIntroductionTimer(List<int> userList)
        {
            lock (m_syncRoot)
            {
                m_introductionTryCount = 0;
                m_introductionTimer = new Timer();
                m_introductionTimer.AutoReset = true;
                m_introductionTimer.Elapsed += new ElapsedEventHandler((sender, e) => m_clientIntroductionTimer_Elapsed(sender, e, userList));
                m_introductionTimer.Interval = 900; // 900ms
                m_introductionTimer.Start();
            }
        }

        void m_clientIntroductionTimer_Elapsed(object sender, ElapsedEventArgs e, List<int> userList)
        {
            lock (m_syncRoot)
            {
                m_introductionTryCount++;
                if (m_introductionTryCount > 5)
                {
                    m_introductionTimer.Dispose();

                    if (MyFakes.ENABLE_MULTIPLAYER_RELAY)
                    {
                        // Use relaying, need to get player public EP first
                        MyMultiplayerPeers.Static.NetworkClient.NatIntroductionSuccess = null;
                        MyMultiplayerPeers.Static.NetworkClient.RegisterCallback<MyEventDirectIntroductionResponse>(OnClientDirectIntroduction);
                        RequestPlayerIntroduction(userList, true);
                    }
                }
                else
                {
                    RequestPlayerIntroduction(userList, false);
                }
            }
        }

        void RequestPlayerIntroduction(List<int> userList, bool relay)
        {
            // Allow only relayed introduction
            if (MyFakes.MULTIPLAYER_RELAY_TEST && !relay)
                return;

            MyEventRequestIntroduction request = new MyEventRequestIntroduction();
            request.GameId = m_game.GameId;
            request.UserList = userList;
            request.RelayIntroduction = relay;
            MyMultiplayerPeers.Static.SendServer(ref request);

            MyMultiplayerGameplay.Log("JOIN 4.b - Requesting player introduction from server, try no: " + m_introductionTryCount);
        }

        void NetworkClient_NatIntroductionSuccess(IPEndPoint endpoint, string token)
        {
            int userId;
            if (!int.TryParse(token, out userId))
                return;

            if (m_natIntroduction != null && userId != MyMultiplayerPeers.Static.HostUserId)
            {
                m_natIntroduction.OnIntroduce(endpoint, token);
                MyMultiplayerGameplay.Log("JOIN 4.c - Nat introduced to " + token + ", EP: " + endpoint.ToString());
            }
        }

        private void OnClientDirectIntroduction(ref MyEventDirectIntroductionResponse msg)
        {
            if (m_natIntroduction != null && msg.UserId != MyMultiplayerPeers.Static.HostUserId)
            {
                m_natIntroduction.OnDirectIntroduce(msg.UserId, MyMultiplayerPeers.Static.NetworkClient.GetRelayedConnection(msg.EndPoint));
                MyMultiplayerGameplay.Log("JOIN 4.c - RELAYED, Directly introduced to " + msg.UserId + ", EP: " + msg.EndPoint);
            }
            UpdateLobby();
        }

        private void UpdateLobby()
        {
            if (m_natIntroduction != null && m_natIntroduction.IsAllConnected())
            {
                OnAllPeersConnected();
                m_natIntroduction = null;
            }
        }

        private void OnAllPeersConnected()
        {
            m_introductionTimer.Dispose();

            ClearCallbacks();
            // When somebody was faster, I can receive new player list
            MyMultiplayerPeers.Static.NetworkClient.RegisterCallback<MyEventGetPlayerListResponse>(OnGetPlayerListResponse);
            MyMultiplayerPeers.Static.NetworkClient.RegisterCallback<MyEventEnterGameResponse>(OnEnterGameResponse);

            var msg = new MyEventEnterGame();
            msg.ConnectedPlayers = MyMultiplayerPeers.Static.Players.Select(s => s.UserId).ToList();
            msg.PlayerInfo = new MyPlayerInfo()
            {
                DisplayName = MyClientServer.LoggedPlayer.GetDisplayName().ToString(),
                UserId = MyClientServer.LoggedPlayer.GetUserId(),
                Faction = MyMwcObjectBuilder_FactionEnum.None,
                PlayerId = 0, // Host will assign
            };

            MyMultiplayerPeers.Static.SendHost(ref msg);
            MyMultiplayerGameplay.Log("JOIN 5. - All peers connected, entering game (#EnterGame)");
        }

        private void OnGameEnterDisallowed()
        {
            ClearCallbacks();
            MyMultiplayerGameplay.Log("JOIN 6b. - Game enter disallowed (#EnterGameResponse)");

            if (m_gameEnterDisallowedAction != null)
                m_gameEnterDisallowedAction();
        }

        private void OnEnterGameResponse(ref MyEventEnterGameResponse msg)
        {
            SetCallback<MyEventCheckpoint>(OnReceiveCheckpoint);

            if (!msg.Allowed)
            {
                OnGameEnterDisallowed();
                return;
            }

            MyEntityIdentifier.CurrentPlayerId = msg.PlayerId;

            var newPlayerEvent = new MyEventNewPlayer();
            newPlayerEvent.PlayerInfo = new MyPlayerInfo()
            {
                DisplayName = MyClientServer.LoggedPlayer.GetDisplayName().ToString(),
                UserId = MyClientServer.LoggedPlayer.GetUserId(),
                Faction = MyMwcObjectBuilder_FactionEnum.None,
                PlayerId = msg.PlayerId,
            };

            MyMultiplayerPeers.Static.SendToAll(ref newPlayerEvent, NetDeliveryMethod.ReliableOrdered, 0);
            MyMultiplayerGameplay.Log("JOIN 6. - Game entered (#EnterGameResponse), sending NewPlayer, requesting sector");
            MyMultiplayerGameplay.Log("MTU: " + msg.SenderConnection.GetMTU());

            var handler = m_gameDownloadingSector;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnReceiveCheckpoint(ref MyEventCheckpoint sectorData)
        {
            MyMultiplayerGameplay.Log("JOIN 7. - Checkpoint received (#SectorData), join completed, loading");
            MyMultiplayerGameplay.Log("MTU: " + sectorData.SenderConnection.GetMTU());
            if (MySession.PlayerShip != null)
            {
                MyMultiplayerGameplay.StoredShip = (MyMwcObjectBuilder_SmallShip)MySession.PlayerShip.GetObjectBuilder(false);
            }

            var loadedPlayer = sectorData.Checkpoint.LoadCoopPlayer(MyClientServer.LoggedPlayer.GetDisplayName().ToString());
            if (loadedPlayer != null)
            {
                var faction = sectorData.Checkpoint.PlayerObjectBuilder.ShipObjectBuilder.Faction;
                sectorData.Checkpoint.PlayerObjectBuilder = loadedPlayer;
                sectorData.Checkpoint.PlayerObjectBuilder.ShipObjectBuilder = new MyMwcObjectBuilder_SmallShip_Player(sectorData.Checkpoint.PlayerObjectBuilder.ShipObjectBuilder);
                sectorData.Checkpoint.PlayerObjectBuilder.ShipObjectBuilder.Faction = faction;
            }
            else
            {
                // Set default values for playership (coop)
                sectorData.Checkpoint.PlayerObjectBuilder.ShipConfigObjectBuilder = null;
                sectorData.Checkpoint.PlayerObjectBuilder.ShipObjectBuilder.ReflectorLight = true;
                sectorData.Checkpoint.PlayerObjectBuilder.ShipObjectBuilder.ReflectorLongRange = false;
                sectorData.Checkpoint.PlayerObjectBuilder.ShipObjectBuilder.IsDummy = false;
            }

            if (sectorData.Checkpoint.CurrentSector.SectorType == MyMwcSectorTypeEnum.SANDBOX && !MinerWars.AppCode.Networking.MySteam.IsActive)
            {
                sectorData.Checkpoint.PlayerObjectBuilder.Money = (float)MySectorServiceClient.GetCheckedInstance().GetGameMoney();
                MySectorServiceClient.SafeClose();
            }
            else
            {
                // Keep money
                sectorData.Checkpoint.PlayerObjectBuilder.Money = MySession.Static != null ? MySession.Static.Player.Money : 0;
            }

            MyMultiplayerGameplay.StartBufferingGameEvents();

            var handler = m_gameJoinedAction;
            if (handler != null)
            {
                handler(m_game, MyResultCodeEnum.OK, sectorData.Checkpoint); // Will handle sector loading
            }
        }
        
        #endregion
    }
}
