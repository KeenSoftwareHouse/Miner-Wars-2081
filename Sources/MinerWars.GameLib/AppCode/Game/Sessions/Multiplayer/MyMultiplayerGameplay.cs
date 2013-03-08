using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Localization;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Prefabs;
using SysUtils.Utils;
using System.Diagnostics;
using System.Net;
using MinerWars.AppCode.Game.Managers.Session;
using Lidgren.Network;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities.CargoBox;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Missions.SinglePlayer;
using MinerWars.AppCode.Game.HUD;
using SysUtils;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.Sessions
{
    partial class MyMultiplayerGameplay
    {
        public static bool IsWaiting { get; set; }

        public const int DEFAULT_LARGE_MESSAGE_SIZE = 16 * 1024;
        public static int MaxPlayers = 16;

        public static bool CanPauseGame
        {
            get
            {
                return !OtherPlayersConnected || IsWaiting;
            }
        }

        public static bool OtherPlayersConnected
        {
            get
            {
                return (Peers != null && Peers.Players.Count > 0);
            }
        }
        public static bool IsRunning { get; private set; }

        public static bool IsHosting
        {
            get
            {
                return IsRunning && Static.IsHost;
            }
        }

        private bool m_savePlayer = false;

        private static MyMultiplayerGameplay m_static;
        public static MyMultiplayerGameplay Static
        {
            get { return m_static ?? (m_static = new MyMultiplayerGameplay()); }
        }

        public bool IsHost;
        public MyJoinMode JoinMode;
        public MyEventCreateGame LastCreateGameRequest;

        public MyPlayerStatistics PlayerStatistics
        {
            get
            {
                return MyClientServer.LoggedPlayer.Statistics;
            }
        }

        public static MyMultiplayerPeers Peers
        {
            get { return MyMultiplayerPeers.Static; }
        }

        public Action<MyNotificationType, MyTextsWrapperEnum, object[]> OnNotification;
        public Action OnShutdown;

        MyMultiplayerConfiguration m_multiplayerConfig; // private so far
        Dictionary<int, List<MyDummyPoint>> m_respawnPoints = new Dictionary<int, List<MyDummyPoint>>(100);
        Dictionary<int, int> m_factions = new Dictionary<int, int>(4);
        List<int> m_possibleFactionCache = new List<int>(4);
        Action<MyEntity> m_playerShipClose;

        object[] m_textArgs = new object[4];
        public static MyMwcObjectBuilder_SmallShip StoredShip;

        bool m_processingBuffer = false;

        MyFollowHostMission m_followMission;

        static MyMultiplayerGameplay()
        {
            InitFactionShipTypes();
        }

        public MyMultiplayerGameplay()
        {
            Init();
        }

        private void Init()
        {
            m_multiplayerConfig = new MyMultiplayerConfiguration();
            m_onEntityDie = new DieHandler(Die);
            m_onConfigChanged = new Action<MySmallShip>(Config_ConfigChanged);
            m_onInventoryChanged = new Action<MyShip>(Playership_InventoryChanged);
            m_unlockOnClosing = new Action<MyEntity>(UnlockOnClosing);
            IsRunning = false;
            IsHost = true;
            IsWaiting = false;
        }

        public static MyGameTypes GameType
        {
            get
            {
                switch (MyGuiScreenGamePlay.Static.GetSessionType())
                {
                    case MyMwcStartSessionRequestTypeEnum.SANDBOX_RANDOM:
                    case MyMwcStartSessionRequestTypeEnum.SANDBOX_OWN:
                    case MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS:
                    case MyMwcStartSessionRequestTypeEnum.JOIN_SANDBOX_FRIEND:
                        return MyGameTypes.Deathmatch;
                        break;

                    case MyMwcStartSessionRequestTypeEnum.JOIN_FRIEND_STORY:
                    case MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT:
                    case MyMwcStartSessionRequestTypeEnum.NEW_STORY:
                        return MyGameTypes.Story;
                        break;
                    case MyMwcStartSessionRequestTypeEnum.EDITOR_MMO:
                    case MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX:
                    case MyMwcStartSessionRequestTypeEnum.EDITOR_STORY:
                    case MyMwcStartSessionRequestTypeEnum.MMO:
                    default:
                        return MyGameTypes.Deathmatch;
                        break;
                }
            }
        }

        public void UpdateGameInfo()
        {
            MyEventUpdateGame msg = new MyEventUpdateGame();

            msg.Name = MyGuiScreenGamePlay.Static.GetGameName(GameType, MyGameplayConstants.GetGameplayDifficulty());
            msg.Password = null;
            msg.JoinMode = JoinMode;

            //if (JoinMode == MyJoinMode.Open)
            //{
            //    MyEventCreateGame createGameRequest = new MyEventCreateGame();
            //    createGameRequest.SectorName = msg.Name;
            //    createGameRequest.Password = String.Empty;
            //    createGameRequest.Type = GameType;
            //    createGameRequest.JoinMode = JoinMode;
            //    createGameRequest.Difficulty = MyGameplayConstants.GetGameplayDifficulty();
            //    Peers.SendServer(ref createGameRequest);
            //}

            Peers.SendServer(ref msg);
        }

        public void Suspend()
        {
            if (MyMultiplayerGameplay.IsRunning && IsHost)
            {
                MyMultiplayerGameplay.Static.SendNotification(MyNotificationType.WaitStart, MyTextsWrapperEnum.NotificationHostLoadingSector);
            }

            MyGuiManager.CloseIngameScreens();

            //if (IsHost)
            {
                foreach (var player in Peers.Players)
                {
                    player.Ship = null;
                }
            }

            if (!IsHost)
            {
                StartBufferingGameEvents();
            }
            else
            {
                UnregisterCallbacks();
                Peers.NetworkClient.RegisterBuffering<MyEventGetPlayerList>();
            }
        }

        public void Resume()
        {
            if (MyFakes.MULTIPLAYER_DISABLED)
                return;

            IsWaiting = false;

            if (!IsHost)
            {
                // When host is disconnected in resume, he disconnected in load, so shutdown MP
                MyPlayerRemote host;
                if (!Peers.TryGetPlayer(Peers.HostUserId, out host))
                {
                    var handler = OnShutdown;
                    if (handler != null)
                    {
                        handler();
                    }
                    return;
                }
            }

            if (!IsHost)
            {
                MyGlobalEvents.DisableAllGlobalEvents();
            }

            if (!IsHost || OtherPlayersConnected) //TODO: remove
            {
                RemoveUnsupportedEntities();
            }

            if (!IsHost)
            {
                MakeAllEntitiesDummy();
            }

            if (IsSandBox())
            {
                LoadRespawnPoints();
            }

            RegisterCallbacks();

            if (!IsHost)
            {
                foreach (var p in Peers.Players)
                {
                    if (p.Ship == null) // Ship can be already assigned (player has respawned before we finished loading)
                    {
                        var newShip = FindPlayerShip(p.PlayerId);
                        if (newShip != null)
                        {
                            p.Ship = newShip;
                            p.Faction = newShip.Faction;
                            OnNewPlayerShip(p.Ship);
                        }
                    }
                }
            }

            if (!IsHost)
            {
                if (IsStory())
                {
                    MyMissions.Unload();
                    m_followMission = MyMissions.GetMissionByID(MyMissionID.COOP_FOLLOW_HOST) as MyFollowHostMission;
                    if (m_followMission == null)
                    {
                        m_followMission = new MyFollowHostMission();
                        m_followMission.Location = new MyMissionBase.MyMissionLocation(MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position, 0);
                        MyMissions.AddMission(m_followMission);
                    }
                    m_followMission.SetHudName(Peers[Peers.HostUserId].GetDisplayName());
                    UpdateCoopTarget();
                    m_followMission.Accept();
                }
            }

            m_processingBuffer = true;
            Peers.NetworkClient.ProcessBuffered();
            m_processingBuffer = false;

            if (IsSandBox())
            {
                if (IsHost)
                {
                    MySession.Static.Player.Faction = ChooseFaction();
                }
            }

            //DisableCheats();

            if (!IsHost)
            {
                if (IsSandBox())
                {
                    // No faction, prevents respawn until server assigns faction
                    MySession.Static.Player.Faction = MyMwcObjectBuilder_FactionEnum.None;
                }
                RequestFaction(MyMwcObjectBuilder_FactionEnum.None);
            }

            if (!IsStory() || !IsHost)
            {
                // Player's ship is dummy
                MySession.PlayerShip.IsDummy = true;
                m_respawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
            else
            {
                m_respawnTime = Int32.MaxValue;
            }

            if (IsHost)
            {
                // Why is this set only for host?
                MySession.PlayerShip.ConfigChanged += m_onConfigChanged;
                MySession.PlayerShip.OnDie += m_onEntityDie;
                MySession.PlayerShip.InventoryChanged += m_onInventoryChanged;
            }

            // Hook entities (hook required events)
            foreach (var entity in MyEntities.GetEntities().OfType<MyCargoBox>())
            {
                HookEntity(entity);
            }
            foreach (var entity in MyEntities.GetEntities().OfType<MyPrefabContainer>())
            {
                HookEntity(entity);
            }

            SendGlobalFlag(MyGlobalFlagsEnum.REQUEST_INFO);
        }

        MySmallShip FindPlayerShip(byte playerId)
        {
            var ships = MyEntities.GetEntities().OfType<MySmallShip>().Where(s => (!(s is MySmallShipBot)) && s.EntityId.HasValue && s.EntityId.Value.PlayerId == playerId && !s.IsHologram);
            Debug.Assert(ships.Count() <= 1, "There's multiple possible playerships in the scene!");
            return ships.FirstOrDefault();
        }

        public void Shutdown()
        {
            Debug.Assert(IsRunning);

            SavePlayer();

            OnShutdown = null;

            IsHost = false;
            ClearCountdownNotification();
            UnregisterCallbacks();
            Peers.Shutdown();
            NewPlayer -= MyMultiplayerGameplay_NewPlayer;
            MyEntityIdentifier.CurrentPlayerId = 0;

            m_lastAmmoAssignment = null;

            StoredShip = null;

            Init();
        }

        public static void Log(string message)
        {
            MyTrace.Send(TraceWindow.Multiplayer, message);
            MyMwcLog.WriteLine("MP - " + message);
        }

        public static void LogDevelop(string message)
        {
            if (MyMwcFinalBuildConstants.IS_DEVELOP)
                Log(message);
        }

        MyHudNotification.MyNotification m_timer;

        private void Notify(MyTextsWrapperEnum text, object[] args, MyNotificationType notificationType = MyNotificationType.Text)
        {
            var handler = OnNotification;
            if (handler != null)
            {
                handler(notificationType, text, args);
            }
        }

        private void Notify(MyTextsWrapperEnum text, object arg0 = null, object arg1 = null, object arg2 = null, object arg3 = null, MyNotificationType notificationType = MyNotificationType.Text)
        {
            var handler = OnNotification;
            if (handler != null)
            {
                m_textArgs[0] = arg0;
                m_textArgs[1] = arg1;
                m_textArgs[2] = arg2;
                m_textArgs[3] = arg3;

                handler(notificationType, text, m_textArgs);
            }
        }

        private bool IsEntityUnsupported(MyEntity entity, Type[] disabledTypes)
        {
            var entityType = entity.GetType();
            return disabledTypes.Any(s => s.IsAssignableFrom(entityType));
        }

        private void RemoveUnsupportedEntities()
        {
            CategoryTypesEnum[] disabledCategories = new CategoryTypesEnum[]
            {
                //CategoryTypesEnum.DOORS,
                //CategoryTypesEnum.DOOR_CASES,
                //CategoryTypesEnum.ALARM,
                // Damage enabled, so barrels too
                //CategoryTypesEnum.BARRELS,
                //CategoryTypesEnum.BANK_NODE,
                //CategoryTypesEnum.CAMERA,
                //CategoryTypesEnum.FOUNDATION_FACTORY,
                //CategoryTypesEnum.SCANNER,
                //CategoryTypesEnum.SECURITY_CONTROL_HUB,
            };

            Type[] disabledEntityTypes = new Type[]
            {

            };

            foreach (var entity in MyEntities.GetEntities())
            {
                if (IsEntityUnsupported(entity, disabledEntityTypes))
                {
                    entity.MarkForClose();
                }
                else if (entity is MyPrefabContainer)
                {
                    foreach (var e in ((MyPrefabContainer)entity).GetPrefabs())
                    {
                        var category = ((MyPrefabBase)e).PrefabCategory;
                        // We don't want indestructible doors and barrels, so close them
                        if (disabledCategories.Contains(category))
                        {
                            e.MarkForClose();
                        }
                        // Damage enabled, prefabs are destructible
                        //else
                        //{
                        //    e.IsDestructible = false;
                        //}
                    }
                }
            }
        }

        public static bool IsStory()
        {
            return MyGuiScreenGamePlay.Static.GetSessionType() == MyMwcStartSessionRequestTypeEnum.NEW_STORY || MyGuiScreenGamePlay.Static.GetSessionType() == MyMwcStartSessionRequestTypeEnum.JOIN_FRIEND_STORY || MyGuiScreenGamePlay.Static.GetSessionType() == MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT;
        }
        public static bool IsEditor()
        {
            return MyGuiScreenGamePlay.Static.GetSessionType() == MyMwcStartSessionRequestTypeEnum.EDITOR_STORY || MyGuiScreenGamePlay.Static.GetSessionType() == MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX;
        }
        public static bool IsSandBox()
        {
            return MyGuiScreenGamePlay.Static.GetSessionType() == MyMwcStartSessionRequestTypeEnum.SANDBOX_OWN || MyGuiScreenGamePlay.Static.GetSessionType() == MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS || MyGuiScreenGamePlay.Static.GetSessionType() == MyMwcStartSessionRequestTypeEnum.SANDBOX_RANDOM;
        }

        // Player is in sector, everything loaded, can process messages
        public void Start()
        {
            if (MyFakes.MULTIPLAYER_DISABLED)
                return;

            Debug.Assert(MyGuiScreenGamePlay.Static != null);
            Debug.Assert(!IsRunning);

            if (IsEditor())
            {
                return;
            }

            MyMwcLog.WriteLine("MyMultiplayerGameplay.Start - START");
            MyMwcLog.IncreaseIndent();

            m_lastPlayerId = 0;

            m_lastConfig = null;
            m_lastAmmoAssignment = null;
            m_lastCamera = MyCameraAttachedToEnum.PlayerMinerShip;

            IsRunning = true;


            if (IsSandBox())
            {
                // Everybody is enemy to every other faction
                var values = Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum));
                foreach (MyMwcObjectBuilder_FactionEnum f1 in values)
                {
                    foreach (MyMwcObjectBuilder_FactionEnum f2 in values)
                    {
                        // Traders are neutral to all
                        if (f1 == MyMwcObjectBuilder_FactionEnum.Traders || f2 == MyMwcObjectBuilder_FactionEnum.Traders)
                        {
                            MyFactions.SetFactionStatus(f1, f2, MyFactions.RELATION_NEUTRAL, false);
                        }
                        else if (f1 != f2 && f1 != MyMwcObjectBuilder_FactionEnum.None)
                        {
                            MyFactions.SetFactionStatus(f1, f2, MyFactions.RELATION_WORST, false);
                        }
                    }
                }

                // Freelancers are enemies to self, it allows classic deathmatch
                MyFactions.SetFactionStatus(MyMwcObjectBuilder_FactionEnum.Freelancers, MyMwcObjectBuilder_FactionEnum.Freelancers, MyFactions.RELATION_WORST, false);
            }

            if (IsHost)
            {
                MyEntityIdentifier.CurrentPlayerId = GeneratePlayerId();

                // Make playership proper id
                MyEntityIdentifier.RemoveEntity(MySession.PlayerShip.EntityId.Value);
                MySession.PlayerShip.EntityId = MyEntityIdentifier.AllocatePlayershipId();
                MyEntityIdentifier.AddEntityWithId(MySession.PlayerShip);

                NewPlayer += MyMultiplayerGameplay_NewPlayer;
            }


            MyClientServer.LoggedPlayer.Statistics = new MyPlayerStatistics();
            Peers.PlayerDisconnected += Peers_PlayerDisconnected;

            Resume();


            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMultiplayerGameplay.Start - END");
        }

        void Inventory_OnInventoryContentChange(MyInventory sender, MyEntity entity)
        {
            UpdateInventory(entity, false);
        }

        private void MakeAllEntitiesDummy()
        {
            foreach (var e in MyEntities.GetEntities())
            {
                MakeDummy(e);
            }
        }

        void MakeDummy(MyEntity e)
        {
            e.IsDummy = true;
            foreach (var c in e.Children)
            {
                MakeDummy(c);
            }
        }

        void MyMultiplayerGameplay_NewPlayer()
        {
            if (!m_removedUnsupportedEntities)
            {
                RemoveUnsupportedEntities();
                m_removedUnsupportedEntities = true;
            }
        }

        bool m_removedUnsupportedEntities = false;



        private void LoadRespawnPoints()
        {
            Debug.Assert(IsSandBox()); // Respawn points are only in sandbox, in coop players respawn near other players

            m_respawnPoints.Clear();

            foreach (var e in MyEntities.GetEntities())
            {
                var dummy = e as MyDummyPoint;
                if (dummy != null && ((dummy.DummyFlags & MyDummyPointFlags.RESPAWN_POINT) != 0))
                {
                    if (dummy.RespawnPointFaction == MyMwcObjectBuilder_FactionEnum.None)
                    {
                        // Respawn points with faction 'none' are not allowed
                        Debug.Fail("Faction cannot be none, fix it in editor");
                        continue;
                    }

                    var factionNum = (int)dummy.RespawnPointFaction;
                    if (!m_respawnPoints.ContainsKey(factionNum))
                    {
                        m_respawnPoints.Add(factionNum, new List<MyDummyPoint>(100));
                    }
                    m_respawnPoints[factionNum].Add(dummy);
                }
            }
        }

        bool PlayerStartPredicate(MyEntity entity)
        {
            var dummy = entity as MyDummyPoint;
            return dummy != null && ((dummy.DummyFlags & MyDummyPointFlags.RESPAWN_POINT) != 0);
        }

        public void HookEntity(MyEntity entity)
        {
            if (entity is MyCargoBox)
            {
                var box = (MyCargoBox)entity;
                HookInventoryChange(box, box.Inventory);
                box.IsDummy = !IsHost;
                box.RespawnTime = TimeSpan.FromSeconds(30);
            }
            else if (entity is MyPrefabContainer)
            {
                var container = (MyPrefabContainer)entity;
                HookInventoryChange(container, container.Inventory);
                container.IsDummy = true;
            }
            else if (entity is MyMeteor)
            {
                // Controlled by host
                entity.IsDummy = !IsHost;
            }
        }

        private void HookInventoryChange(MyEntity entity, MyInventory inventory)
        {
            inventory.OnInventoryContentChange += (i) => Inventory_OnInventoryContentChange(i, entity);
        }

        public static void StartBufferingGameEvents()
        {
            // Called before loading game
            // Here's messages which makes permanent changes to world, it needs to be buffered
            Peers.NetworkClient.RegisterBuffering<MyEventNewPlayer>();
            Peers.NetworkClient.RegisterBuffering<MyEventRespawn>();
            Peers.NetworkClient.RegisterBuffering<MyEventShoot>();
            Peers.NetworkClient.RegisterBuffering<MyEventAmmoExplosion>();
            Peers.NetworkClient.RegisterBuffering<MyEventDoDamage>();
            Peers.NetworkClient.RegisterBuffering<MyEventProjectileHit>();
            Peers.NetworkClient.RegisterBuffering<MyEventDie>();
            Peers.NetworkClient.RegisterBuffering<MyEventSetFaction>();
            Peers.NetworkClient.RegisterBuffering<MyEventShipConfigUpdate>();
            Peers.NetworkClient.RegisterBuffering<MyEventSpeacialWeapon>();
            Peers.NetworkClient.RegisterBuffering<MyEventInventoryUpdate>();
            Peers.NetworkClient.RegisterBuffering<MyEventMissionProgress>();
            Peers.NetworkClient.RegisterBuffering<MyEventLock>();
            Peers.NetworkClient.RegisterBuffering<MyEventHealthUpdate>();
            Peers.NetworkClient.RegisterBuffering<MyEventSpawnBot>();
            Peers.NetworkClient.RegisterBuffering<MyEventAfterburner>();
            Peers.NetworkClient.RegisterBuffering<MyEventPilotDie>();
            Peers.NetworkClient.RegisterBuffering<MyEventNotification>();
            Peers.NetworkClient.RegisterBuffering<MyEventCutOut>();
            Peers.NetworkClient.RegisterBuffering<MyEventFlags>();
            Peers.NetworkClient.RegisterBuffering<MyEventNewEntity>();
            Peers.NetworkClient.RegisterBuffering<MyEventMissionUpdateVars>();
            Peers.NetworkClient.RegisterBuffering<MyEventAddExplosion>();
            Peers.NetworkClient.RegisterBuffering<MyEventDummyFlags>();
            Peers.NetworkClient.RegisterBuffering<MyEventUpdatePositionFast>();
            Peers.NetworkClient.RegisterBuffering<MyEventAddVoxelHand>();
            Peers.NetworkClient.RegisterBuffering<MyEventSetActorFaction>();
            Peers.NetworkClient.RegisterBuffering<MyEventSetFactionRelation>();
            Peers.NetworkClient.RegisterBuffering<MyEventUpdateRotationFast>();
            Peers.NetworkClient.RegisterBuffering<MyEventFriendlyFire>();
            Peers.NetworkClient.RegisterBuffering<MyEventSetEntityFaction>();
            Peers.NetworkClient.RegisterBuffering<MyEventMusicTransition>();
            Peers.NetworkClient.RegisterBuffering<MyEventEvent>();
            Peers.NetworkClient.RegisterBuffering<MyEventGlobalFlag>();
        }

        private void RegisterCallbacks()
        {
            Peers.NetworkClient.RegisterCallback<MyEventNewPlayer>(OnNewPlayer);
            Peers.NetworkClient.RegisterCallback<MyEventUpdatePosition>(OnUpdatePosition);
            Peers.NetworkClient.RegisterCallback<MyEventUpdatePositionFast>(OnUpdatePositionFast);
            Peers.NetworkClient.RegisterCallback<MyEventRespawn>(OnRespawn);
            Peers.NetworkClient.RegisterCallback<MyEventShoot>(OnShoot);
            Peers.NetworkClient.RegisterCallback<MyEventAmmoExplosion>(OnAmmoExplosion);
            Peers.NetworkClient.RegisterCallback<MyEventDoDamage>(OnDoDamage);
            Peers.NetworkClient.RegisterCallback<MyEventProjectileHit>(OnProjectileHit);
            Peers.NetworkClient.RegisterCallback<MyEventDie>(OnDie);
            Peers.NetworkClient.RegisterCallback<MyEventStatsUpdate>(OnStatsUpdate);
            Peers.NetworkClient.RegisterCallback<MyEventSetFaction>(OnSetFaction);
            Peers.NetworkClient.RegisterCallback<MyEventAmmoUpdate>(OnAmmoUpdate);
            Peers.NetworkClient.RegisterCallback<MyEventSpeacialWeapon>(OnSpecialWeaponEvent);
            Peers.NetworkClient.RegisterCallback<MyEventChat>(OnChatMessage);
            Peers.NetworkClient.RegisterCallback<MyEventChooseFactionResponse>(OnChooseFactionResponse);
            Peers.NetworkClient.RegisterCallback<MyEventShipConfigUpdate>(OnUpdateConfig);
            Peers.NetworkClient.RegisterCallback<MyEventInventoryUpdate>(OnInventoryUpdate);
            Peers.NetworkClient.RegisterCallback<MyEventLock>(OnLock);
            Peers.NetworkClient.RegisterCallback<MyEventLockResult>(OnLockResult);
            Peers.NetworkClient.RegisterCallback<MyEventHealthUpdate>(OnHealthUpdate);
            Peers.NetworkClient.RegisterCallback<MyEventAfterburner>(OnAfterburner);
            Peers.NetworkClient.RegisterCallback<MyEventPilotDie>(OnPilotDie);
            Peers.NetworkClient.RegisterCallback<MyEventNotification>(OnNotify);
            Peers.NetworkClient.RegisterCallback<MyEventCutOut>(OnCutOut);
            Peers.NetworkClient.RegisterCallback<MyEventFlags>(OnUpdateFlags);
            Peers.NetworkClient.RegisterCallback<MyEventNewEntity>(OnNewEntity);
            Peers.NetworkClient.RegisterCallback<MyEventAddExplosion>(OnAddExplosion);
            Peers.NetworkClient.RegisterCallback<MyEventDummyFlags>(OnUpdateDummyFlags);
            Peers.NetworkClient.RegisterCallback<MyEventAddVoxelHand>(OnAddVoxelHand);
            Peers.NetworkClient.RegisterCallback<MyEventPlayDialogue>(OnPlayDialogue);
            Peers.NetworkClient.RegisterCallback<MyEventPlaySound>(OnPlaySound);
            Peers.NetworkClient.RegisterCallback<MyEventHeadshake>(OnHeadshake);
            Peers.NetworkClient.RegisterCallback<MyEventSetActorFaction>(OnSetActorFaction);
            Peers.NetworkClient.RegisterCallback<MyEventSetFactionRelation>(OnSetFactionRelation);
            Peers.NetworkClient.RegisterCallback<MyEventUpdateRotationFast>(OnUpdateRotationFast);
            Peers.NetworkClient.RegisterCallback<MyEventFriendlyFire>(OnFriendlyFire);
            Peers.NetworkClient.RegisterCallback<MyEventSetEntityFaction>(OnSetEntityFaction);
            Peers.NetworkClient.RegisterCallback<MyEventMusicTransition>(OnMusicTransition);
            Peers.NetworkClient.RegisterCallback<MyEventEvent>(OnEvent);
            Peers.NetworkClient.RegisterCallback<MyEventGlobalFlag>(OnGlobalFlag);

            if (!IsHost) // Messages only send by host
            {
                Peers.NetworkClient.RegisterCallback<MyEventSpawnBot>(OnSpawnBot);
                Peers.NetworkClient.RegisterCallback<MyEventMissionProgress>(OnMissionProgress);
                Peers.NetworkClient.RegisterCallback<MyEventMissionUpdateVars>(OnMissionUpdateVars);
                Peers.NetworkClient.RegisterCallback<MyEventCountdown>(OnCountdown);
            }

            if (IsHost) // Messages only received by host
            {
                Peers.NetworkClient.RegisterCallback<MyEventEnterGame>(OnEnterGame);
                Peers.NetworkClient.RegisterCallback<MyEventGetPlayerList>(OnGetPlayerList);
                Peers.NetworkClient.RegisterCallback<MyEventChooseFaction>(OnChooseFaction);
                Peers.NetworkClient.RegisterCallback<MyEventSavePlayer>(OnSavePlayer);
            }
        }

        private void UnregisterCallbacks()
        {
            Peers.NetworkClient.RemoveCallback<MyEventNewPlayer>();
            Peers.NetworkClient.RemoveCallback<MyEventUpdatePosition>();
            Peers.NetworkClient.RemoveCallback<MyEventUpdatePositionFast>();
            Peers.NetworkClient.RemoveCallback<MyEventRespawn>();
            Peers.NetworkClient.RemoveCallback<MyEventShoot>();
            Peers.NetworkClient.RemoveCallback<MyEventAmmoExplosion>();
            Peers.NetworkClient.RemoveCallback<MyEventDoDamage>();
            Peers.NetworkClient.RemoveCallback<MyEventProjectileHit>();
            Peers.NetworkClient.RemoveCallback<MyEventDie>();
            Peers.NetworkClient.RemoveCallback<MyEventStatsUpdate>();
            Peers.NetworkClient.RemoveCallback<MyEventSetFaction>();
            Peers.NetworkClient.RemoveCallback<MyEventAmmoUpdate>();
            Peers.NetworkClient.RemoveCallback<MyEventSpeacialWeapon>();
            Peers.NetworkClient.RemoveCallback<MyEventChat>();
            Peers.NetworkClient.RemoveCallback<MyEventChooseFactionResponse>();
            Peers.NetworkClient.RemoveCallback<MyEventShipConfigUpdate>();
            Peers.NetworkClient.RemoveCallback<MyEventInventoryUpdate>();
            Peers.NetworkClient.RemoveCallback<MyEventLock>();
            Peers.NetworkClient.RemoveCallback<MyEventLockResult>();
            Peers.NetworkClient.RemoveCallback<MyEventHealthUpdate>();
            Peers.NetworkClient.RemoveCallback<MyEventAfterburner>();
            Peers.NetworkClient.RemoveCallback<MyEventPilotDie>();
            Peers.NetworkClient.RemoveCallback<MyEventNotification>();
            Peers.NetworkClient.RemoveCallback<MyEventCutOut>();
            Peers.NetworkClient.RemoveCallback<MyEventFlags>();
            Peers.NetworkClient.RemoveCallback<MyEventNewEntity>();
            Peers.NetworkClient.RemoveCallback<MyEventAddExplosion>();
            Peers.NetworkClient.RemoveCallback<MyEventDummyFlags>();
            Peers.NetworkClient.RemoveCallback<MyEventAddVoxelHand>();
            Peers.NetworkClient.RemoveCallback<MyEventPlayDialogue>();
            Peers.NetworkClient.RemoveCallback<MyEventPlaySound>();
            Peers.NetworkClient.RemoveCallback<MyEventHeadshake>();
            Peers.NetworkClient.RemoveCallback<MyEventSetActorFaction>();
            Peers.NetworkClient.RemoveCallback<MyEventSetFactionRelation>();
            Peers.NetworkClient.RemoveCallback<MyEventUpdateRotationFast>();
            Peers.NetworkClient.RemoveCallback<MyEventFriendlyFire>();
            Peers.NetworkClient.RemoveCallback<MyEventSetEntityFaction>();
            Peers.NetworkClient.RemoveCallback<MyEventMusicTransition>();
            Peers.NetworkClient.RemoveCallback<MyEventEvent>();
            Peers.NetworkClient.RemoveCallback<MyEventGlobalFlag>();

            if (!IsHost)
            {
                Peers.NetworkClient.RemoveCallback<MyEventSpawnBot>();
                Peers.NetworkClient.RemoveCallback<MyEventMissionUpdateVars>();
                Peers.NetworkClient.RemoveCallback<MyEventMissionProgress>();
                Peers.NetworkClient.RemoveCallback<MyEventCountdown>();
            }
            if (IsHost)
            {
                Peers.NetworkClient.RemoveCallback<MyEventEnterGame>();
                Peers.NetworkClient.RemoveCallback<MyEventGetPlayerList>();
                Peers.NetworkClient.RemoveCallback<MyEventChooseFaction>();
                Peers.NetworkClient.RemoveCallback<MyEventSavePlayer>();
            }
        }

        void Peers_PlayerDisconnected(MyPlayerRemote player)
        {
            if (player.UserId == Peers.HostUserId)
            {
                // TODO: Connection to host lost, shutdown
                var handler = OnShutdown;
                if (handler != null)
                {
                    handler();
                }
            }
            else
            {
                // Close all entities of disconnected player
                foreach (var e in MyEntities.GetEntities())
                {
                    if (e.EntityId.HasValue && e.EntityId.Value.PlayerId == player.PlayerId)
                    {
                        e.MarkForClose();
                    }
                }
            }

            if (player.Ship != null)
            {
                player.Ship.MarkForClose();
                player.Ship = null;
            }            

            if (IsHost)
            {
                ClearLocks(player.PlayerId);

                var playerLeftMsg = new MyEventPlayerStateChanged();
                playerLeftMsg.UserId = player.UserId;
                playerLeftMsg.NewState = MyMultiplayerStateEnum.Disconnected;
                Peers.SendServer(ref playerLeftMsg);
            }

            Notify(MyTextsWrapperEnum.MP_XHasLeft, player.GetDisplayName());
        }

        private void DisableCheats()
        {
            foreach (var c in MyGameplayCheats.AllCheats)
            {
                MyGameplayCheats.EnableCheat(c.CheatEnum, false);
            }
        }

        // Sends and receives messages
        public void Update()
        {
            Debug.Assert(MyGuiScreenGamePlay.Static != null);

            if (IsRunning)
            {
                TestRespawn();
                UpdateMissionVars();
                SetPriority();
                AnnounceGame();

                if (m_savePlayer)
                {
                    SavePlayer();
                    m_savePlayer = false;
                }
            }
        }

        private void AnnounceGame()
        {
            if (!Peers.IsConnectedToServer)
            {
                try
                {
                    Peers.NetworkClient.RemoveCallback<MyEventLoginResponse>();
                    Peers.NetworkClient.RemoveCallback<MyEventCreateGameResponse>();
                    LastCreateGameRequest.JoinMode = JoinMode;
                    LastCreateGameRequest.SectorName = MyGuiScreenGamePlay.Static.GetGameName(GameType, MyGameplayConstants.GetGameplayDifficulty());
                    Peers.SendServer(ref LastCreateGameRequest);
                }
                catch
                {
                }
            }
        }

        private void SetPriority()
        {
            // All players in coop has same priority as host
            if (IsHost && IsStory() && MySession.PlayerShip != null)
            {
                foreach (var peer in Peers.Players)
                {
                    if (peer.Ship != null)
                    {
                        peer.Ship.AIPriority = MySession.PlayerShip.AIPriority;
                    }
                }
            }
        }

        public int GetFactionPlayerCount(MyMwcObjectBuilder_FactionEnum faction)
        {
            int count = 0;
            foreach (var p in Peers.Players)
            {
                if (p.Faction == faction)
                    count++;
            }
            if (MySession.Static.Player.Faction == faction) count++;
            return count;
        }

        public MyMwcObjectBuilder_FactionEnum ChooseFaction(MyMwcObjectBuilder_FactionEnum preferredFaction = MyMwcObjectBuilder_FactionEnum.None)
        {
            if (IsStory())
            {
                Debug.Assert(IsHost);
                return MySession.Static.Player.Faction;
            }
            else
            {
                m_factions.Clear();

                foreach (var k in m_respawnPoints)
                {
                    m_factions[k.Key] = 0;
                }

                foreach (var p in Peers.Players)
                {
                    if (m_factions.ContainsKey((int)p.Faction))
                    {
                        m_factions[(int)p.Faction]++;
                    }
                }
                if (m_factions.ContainsKey((int)MySession.Static.Player.Faction))
                {
                    m_factions[(int)MySession.Static.Player.Faction]++;
                }

                m_possibleFactionCache.Clear();

                int minFactionCount = int.MaxValue;
                foreach (var f in m_factions)
                {
                    if (f.Value < minFactionCount)
                    {
                        m_possibleFactionCache.Clear();
                        m_possibleFactionCache.Add(f.Key);
                        minFactionCount = f.Value;
                    }
                    else if (f.Value == minFactionCount)
                    {
                        m_possibleFactionCache.Add(f.Key);
                    }
                }

                if (m_possibleFactionCache.Contains((int)preferredFaction))
                    return preferredFaction;
                else if (m_possibleFactionCache.Count > 0)
                    return (MyMwcObjectBuilder_FactionEnum)MyMwcUtils.GetRandomItem(m_possibleFactionCache);
                else
                    return MyMwcObjectBuilder_FactionEnum.Euroamerican; // Default faction
            }
        }

        bool IsControlledByMe(MyEntity entity)
        {
            if (entity == null || !entity.EntityId.HasValue)
                return false;

            bool hasMyId = entity.EntityId.Value.PlayerId == MyEntityIdentifier.CurrentPlayerId || (entity.EntityId.Value.PlayerId == 0 && IsHost);
            bool isLockedByMe = IsLockedByMe(entity);
            bool isLockedByOther = IsLockedByOtherPlayer(entity);

            return (hasMyId && !isLockedByOther) || isLockedByMe;
        }

        bool CheckSenderId<T>(T msg, uint id)
            where T : struct, IMyEvent
        {
            if (msg.SenderConnection == null)
            {
                return false;
            }

            var senderPlayer = (MyPlayerRemote)msg.SenderConnection.Tag;
            var entityId = new MyEntityIdentifier(id);

            if (senderPlayer.UserId == Peers.HostUserId && entityId.PlayerId == 0)
                return true;

            if (m_lockedEntities.ContainsKey(id))
            {
                return true;
            }

            MyPlayerRemote ownerPlayer;
            bool playerExists = Peers.TryGetPlayer(entityId.PlayerId, out ownerPlayer);
            if (!playerExists)
            {
                Alert("Owner of this entity does not exist!", msg.SenderEndpoint, msg.EventType);
                return false;
            }

            if (entityId.PlayerId == 0 && Peers.HostUserId != ownerPlayer.UserId)
            {
                Alert("Only host can update neutral entities!", msg.SenderEndpoint, msg.EventType);
                return false;
            }

            if (senderPlayer.PlayerId != entityId.PlayerId)
            {
                Alert("Entity Id send by sender is not his!", msg.SenderEndpoint, msg.EventType);
                return false;
            }

            return true;
        }

        void Alert(string alertFormat, IPEndPoint endpoint, MyEventEnum eventType)
        {
            AlertVerbose(alertFormat, endpoint, eventType);
        }

        [Conditional("DEBUG")]
        void AlertVerbose(string alertFormat, IPEndPoint endpoint, MyEventEnum eventType)
        {
            var player = Peers.Players.FirstOrDefault(p => p.Connection.RemoteEndpoint == endpoint);
            if (player != null)
            {
                string playerInfo = String.Format(" UserId: {0}, GameUserId: {1}, EndPoint: {2}", player.UserId, player.PlayerId, player.Connection.RemoteEndpoint);
                MyTrace.Send(TraceWindow.MultiplayerAlerts, eventType.ToString() + ": " + alertFormat + playerInfo);
            }
        }
    }
}
