#region Using
using System;
using System.Linq;
using MinerWars.AppCode.Game.Journal;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Missions.SinglePlayer;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using System.Collections.Generic;
using MinerWarsMath;
using System.Reflection;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.World;
using System.Runtime.Serialization;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Entities.WayPoints;
using System.Diagnostics;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Gameplay;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using SysUtils;
using System.Text;
#endregion

namespace MinerWars.AppCode.Game.Managers.Session
{
    /// <summary>
    /// Base class for all session types (single, coop, mmo, sandbox)
    /// </summary>
    internal abstract class MySession : MyResource, IMyInventory
    {
        public static MySession Static { get; set; }

        /// <summary>
        /// Gets or sets player's inventory (now use for ship's hangar)
        /// </summary>
        public MyInventory Inventory { get; set; }

        /// <summary>
        /// Gets or sets the player
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public MyPlayer Player { get; private set; }

        public DateTime GameDateTime { get; set; }

        public static MySmallShip PlayerShip { get; set; }

        public static bool IsPlayerShip(MyEntity entity)
        {
            if (entity == null)
                return false;
            if (!(entity is MySmallShip))
                return false;
            if (PlayerShip == entity)
                return true;
            if (MyMultiplayerGameplay.IsRunning)
            {
                foreach (var player in MinerWars.AppCode.Game.Sessions.Multiplayer.MyMultiplayerPeers.Static.Players)
                {
                    if (player.Ship == entity)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static MyPlayerFriends PlayerFriends { get; set; }

        public MyEventLog EventLog { get; protected set; }

        public string CheckpointName { get; private set; }

        public MyFactionRelationChanges FactionRelationChanges { get; private set; }

        public MyMotherShipPosition MotherShipPosition { get; private set; }

        /// <summary>
        /// Occures when all entities are loaded and are ready to be linked together (references).
        /// </summary>
        public event Action LinkEntities;


        public bool Is2DSector;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySession"/> class.
        /// </summary>
        protected MySession()
        {
            GameDateTime = new DateTime(2081, 1, 1);
            //EventLog = new MyEventLog();
        }

        public virtual void Init()
        {
            //Create player if it was not loaded from DB
            if (Player == null)
            {
                Player = new MyPlayer();
            }

            //Create player if it was not loaded from DB
            if (EventLog == null)
            {
                EventLog = new MyEventLog();
            }

            if (FactionRelationChanges == null)
            {
                FactionRelationChanges = new MyFactionRelationChanges();
            }

            GameDateTime = new DateTime(2081, 1, 1);

            if (Inventory == null)
            {
                Inventory = new MyInventory();
            }

            if (PlayerFriends == null)
            {
                PlayerFriends = new MyPlayerFriends();
            }

            if (MotherShipPosition == null)
            {
                MotherShipPosition = new MyMotherShipPosition();
            }

            Is2DSector = false;

            ////Create playership if it was not loaded from DB
            //if (Player.Ship == null)
            //{
            //    Player.Ship = new MySmallShip(1.0f, MyClientServer.LoggedPlayer.GetDisplayName().ToString());
            //    Player.Ship.Init("MyShip", MySmallShip.CreateDefaultSmallShipObjectBuilder(Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up)));
            //    //Player.Ship.WorldMatrix = Matrix.CreateTranslation(0, 0, 0);
            //    Player.Ship.Inventory.MaxItems = 1500;
            //    Player.Ship.Inventory.FillInventoryWithAllItems();
            //}            
        }

        public void BeforeLoad(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            MyEntityIdentifier.AllocationSuspended = true;

            //if (MyMultiplayerGameplay.IsRunning && !MyMultiplayerGameplay.IsHost)
            //{
            //    MyMwcPositionAndOrientation position = MyMultiplayerGameplay.Static.GetSafeRespawnPositionNearPlayer(checkpoint.PlayerObjectBuilder.ShipObjectBuilder.ShipType);
            //    checkpoint.PlayerObjectBuilder.ShipObjectBuilder.PositionAndOrientation = position;
            //    return;
            //}

            // Try find player start location and set player location
            if (checkpoint.SectorObjectBuilder != null && checkpoint.SectorObjectBuilder.SectorObjects != null)
            {
                int playerStartsCount = 0;
                foreach (var builder in checkpoint.SectorObjectBuilder.SectorObjects)
                {
                    MyMwcObjectBuilder_DummyPoint dummyBuilder = builder as MyMwcObjectBuilder_DummyPoint;
                    if (dummyBuilder != null)
                    {
                        if ((dummyBuilder.DummyFlags & MyDummyPointFlags.PLAYER_START) > 0)
                        {
                            checkpoint.PlayerObjectBuilder.ShipObjectBuilder.PositionAndOrientation = dummyBuilder.PositionAndOrientation;
                            playerStartsCount++;
                        }
                    }
                }
            }

            MotherShipPosition.Load(checkpoint.Dictionary);
        }

        /// <summary>
        /// This method is called when sector and all entities are loaded (created from object builder - or generated other way).
        /// It raises LinkEntities event, and calls method MyEntities.Link.
        /// </summary>
        public void AfterLoad(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            CheckpointName = checkpoint.CheckpointName;

            MyEntityIdentifier.AllocationSuspended = false;

            var handler = LinkEntities;
            if (handler != null)
            {
                handler();
            }

            MyEntities.Link();
            MyWayPointGraph.DeleteNullVerticesFromPaths();
            MyWayPointGraph.RemoveWaypointsAroundLargeStaticObjects();
            FactionRelationChanges.Init(checkpoint.FactionRelationChangesBuilder);
            //  GPS waypoints must be created after the first physics pass: see end of MyGuiScreenGamePlay.Update()
        }

        // Calculates squared distance from entity to closest player or player friend
        public float DistanceToPlayersSquared(MyEntity entity)
        {
            float dist = Vector3.DistanceSquared(MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static.ControlledEntity.GetPosition(), entity.GetPosition());

            for (int i = 0; i < PlayerFriends.Count; i++)
            {
                var friend = PlayerFriends[i];
                float newdist = Vector3.DistanceSquared(friend.GetPosition(), entity.GetPosition());
                if (newdist < dist)
                {
                    dist = newdist;
                }
            }

            return dist;
        }

        public float DistanceToPlayers(MyEntity entity)
        {
            return (float)Math.Sqrt(DistanceToPlayersSquared(entity));
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public virtual void Update()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMissions.Update");
            MyMissions.Update();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyGlobalEvents.Update()");
            //Update global events in the game. This should be driven and synchronized
            //by sector server
            World.Global.MyGlobalEvents.Update();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("RefreshAvailableMissions");

            // update global game time
            GameDateTime = GameDateTime + new TimeSpan(0, 0, 0, 0, MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS);

            // check if some new mission are available (because time has advanced)
            MyMissions.RefreshAvailableMissions();
            MotherShipPosition.Update();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public bool CanSaveAndLoadSessionInventory
        {
            get
            {
                if (MyGuiScreenGamePlay.Static != null)
                {
                    var gameType = MyGuiScreenGamePlay.Static.GetGameType();
                    return gameType == MyGuiScreenGamePlayType.EDITOR_STORY || gameType == MyGuiScreenGamePlayType.GAME_STORY;
                }
                return false;
            }
        }

        public static bool CanBeSaved(bool asTemplate, MyMwcSectorIdentifier sectorId, bool fromEditor)
        {
            bool isLogged = MyClientServer.LoggedPlayer != null; // Must be logged
            bool isNotDemo = !MyClientServer.LoggedPlayer.IsDemoUser(); // Cannot be demo user
            bool sectorCanBeSaved = sectorId.CanBeCheckpointSaved(); // Sector must be savable (for story only zero plane)
            bool canSaveObjectives = MyMissions.CanSaveObjectives();

            // Story sectors can be only on "zero" plane (Y coordinate is 0)
            return
                isLogged && isNotDemo && sectorCanBeSaved && canSaveObjectives
                && (asTemplate == false || MyClientServer.LoggedPlayer.GetCanAccessEditorForStory()) // When saving story template, must have story editor access
                && (fromEditor == false || sectorId.SectorType != MyMwcSectorTypeEnum.STORY || MyClientServer.LoggedPlayer.GetCanAccessEditorForStory()); // To save STORY sector from editor, it's necessary to have rights
        }

        public MyGuiScreenEditorSaveProgress SaveCheckpointTemplate(string checkpointTemplateName)
        {
            return Save(true, checkpointTemplateName, true, false, false);
        }

        /// <summary>
        /// Creates checkpoint
        /// </summary>
        public MyGuiScreenEditorSaveProgress Save(bool saveSector = true, bool saveVisible = false, bool pause = false)
        {
            return Save(saveSector, CheckpointName, false, saveVisible, pause);
        }

        private MyGuiScreenEditorSaveProgress Save(bool saveSector, string checkpointName, bool asTemplate, bool visibleSave, bool pause)
        {
            MyMwcSectorIdentifier sectorId = MyGuiScreenGamePlay.Static.GetSectorIdentifier();
            bool isEditor = MyGuiScreenGamePlay.Static.IsEditorActive();

            if (CanBeSaved(asTemplate, sectorId, isEditor))
            {
                MyMwcObjectBuilder_Checkpoint checkpoint = GetCheckpointBuilder(saveSector);
                checkpoint.CheckpointName = checkpointName;

                // Need to store actual checkpoint...because when we travel, we receive only sector, not checkpoint
                MyGuiScreenGamePlay.Static.AddEnterSectorResponse(checkpoint, null);

                bool savePlayerShip = MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.EDITOR_STORY ? MyEditor.SavePlayerShip : true;

                if (savePlayerShip && saveSector)
                {
                    UpdatePlayerStartDummy(checkpoint);
                }

                MyHudNotification.MyNotification notification = null;
                if (!visibleSave)
                {
                    notification = new MyHudNotification.MyNotification(Localization.MyTextsWrapperEnum.SavingSectorToServer, 2500);
                    MyHudNotification.AddNotification(notification);
                }

                StringBuilder errors = null;
                if (isEditor)
                {
                    errors = CheckMissingObject(checkpoint);
                }

                MyGuiScreenEditorSaveProgress screen = new MyGuiScreenEditorSaveProgress(sectorId, checkpoint, savePlayerShip, visibleSave, pause);
                //screen.Closed += new MyGuiScreenBase.ScreenHandler((s) => { if (notification != null) notification.Disappear(); });

                CheckErrors(errors, screen);
                return screen;
            }

            return null;
        }

        private static void CheckErrors(StringBuilder errors, MyGuiScreenEditorSaveProgress screen)
        {
            if (errors != null)
            {
                var caption = new StringBuilder("Please check missing entities (full list in log). Really save?");
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, errors, caption, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No, (result) =>
                {
                    if (result == MyGuiScreenMessageBoxCallbackEnum.YES)
                    {
                        MyGuiManager.AddScreen(screen);
                    }
                    else
                    {
                        screen.CloseScreenNow();
                    }
                }));
            }
            else
            {
                MyGuiManager.AddScreen(screen);
            }
        }

        /// <summary>
        /// Checks missing objects, returns true when save sector, false when abort saving
        /// </summary>
        StringBuilder CheckMissingObject(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            if (MyMwcFinalBuildConstants.IS_DEVELOP && (MyFakes.DUMP_MISSING_OBJECTS || MyFakes.SHOW_MISSING_OBJECTS))
            {
                var missing = MyGuiScreenGamePlay.FindMissingObjectBuilders(checkpoint);
                if (MyFakes.DUMP_MISSING_OBJECTS)
                {
                    DumpMissingObjects(missing);
                }
                if (MyFakes.SHOW_MISSING_OBJECTS)
                {
                    return GetMissingObjects(missing); // false to abort saving
                }
            }
            return null;
        }

        void DumpMissingObjects(List<MyMwcObjectBuilder_Base> missingObjects)
        {
            if (missingObjects.Count > 0)
            {
                MyMwcLog.WriteLine("MISSING OBJECTS DUMP: ");
            }

            foreach (var obj in missingObjects)
            {
                MyMwcLog.WriteLine(String.Format("  EntityID: {0}, Name: {1}, Type: {2}", obj.EntityId.Value, obj.Name, obj.GetType().Name));
            }
        }

        /// <summary>
        /// Shows missing objects, returns true when save sector, false when abort saving
        /// </summary>
        StringBuilder GetMissingObjects(List<MyMwcObjectBuilder_Base> missingObjects)
        {
            if (missingObjects.Count == 0)
            {
                return null;
            }

            StringBuilder text = new StringBuilder();

            foreach (var obj in missingObjects.Take(20))
            {
                text.AppendFormat("  EntityID: {0}, Name: {1}, Type: {2}", obj.EntityId.Value, obj.Name, obj.GetType().Name);
                text.AppendLine();
            }
            return text;
        }

        private static void UpdatePlayerStartDummy(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            var dummyBuilder = checkpoint.SectorObjectBuilder.SectorObjects.OfType<MyMwcObjectBuilder_DummyPoint>().FirstOrDefault(s => (s.DummyFlags & MyDummyPointFlags.PLAYER_START) > 0);
            if (dummyBuilder == null)
            {
                dummyBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.DummyPoint, null) as MyMwcObjectBuilder_DummyPoint;
                dummyBuilder.DummyFlags = MyDummyPointFlags.PLAYER_START;
                checkpoint.SectorObjectBuilder.SectorObjects.Add(dummyBuilder);

                var dummyEntity = new MyDummyPoint();
                dummyEntity.Init(String.Empty, dummyBuilder, MySession.PlayerShip.WorldMatrix);
                MyEntities.Add(dummyEntity);
            }

            MyGuiScreenGamePlay.Static.ClampPlayerToBorderSafeArea();

            dummyBuilder.PositionAndOrientation.Position = MySession.PlayerShip.WorldMatrix.Translation;
            dummyBuilder.PositionAndOrientation.Forward = MySession.PlayerShip.WorldMatrix.Forward;
            dummyBuilder.PositionAndOrientation.Up = MySession.PlayerShip.WorldMatrix.Up;
        }

        private void CheckEntityIds(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            if (checkpoint.SectorObjectBuilder == null)
                return;

            foreach (var builder in checkpoint.SectorObjectBuilder.SectorObjects)
            {
                if (builder.EntityId.HasValue)
                {
                    if (builder.EntityId.Value == checkpoint.PlayerObjectBuilder.ShipObjectBuilder.EntityId
                        || checkpoint.PlayerObjectBuilder.ShipObjectBuilder.Inventory.InventoryItems.Select(s => s.EntityId).Any(id => id == builder.EntityId))
                    {
                        Debug.Fail("Entity ID of object is same as playership or something in playership inventory");
                    }
                }
            }
        }

        protected virtual MyMwcObjectBuilder_Session GetObjectBuilder()
        {
            return null;
        }

        /// <summary>
        /// This method is used to gather all object builders of all objects in sector
        /// </summary>
        /// <returns></returns>
        List<MyMwcObjectBuilder_Base> GetSectorObjectBuilders()
        {
            return MyEntities.Save() ?? new List<MyMwcObjectBuilder_Base>();
        }


        public MyMwcObjectBuilder_Checkpoint GetCheckpointBuilder(bool includeSector)
        {
            MyMwcObjectBuilder_Checkpoint checkpoint = new MyMwcObjectBuilder_Checkpoint();

            if (includeSector)
            {
                checkpoint.SectorObjectBuilder = new MyMwcObjectBuilder_Sector();

                List<MyMwcObjectBuilder_Base> sectorObjectBuilders = GetSectorObjectBuilders();
                foreach (MyMwcObjectBuilder_Base objectBuilder in sectorObjectBuilders)
                {
                    System.Diagnostics.Debug.Assert(objectBuilder != null, "If object is not to be saved, unset his EntityFlags::Save!");
                }
                checkpoint.SectorObjectBuilder.SectorObjects = sectorObjectBuilders;
                checkpoint.SectorObjectBuilder.ObjectGroups = MyEditor.Static.ObjectGroups.ConvertAll(a => a.GetObjectBuilder());
                checkpoint.SectorObjectBuilder.SnapPointLinks = MyEditor.Static.GetSnapPointLinkBuilders();
                if (MyGuiScreenGamePlay.Static.Checkpoint.SectorObjectBuilder != null)
                {
                    checkpoint.SectorObjectBuilder.Name = MyGuiScreenGamePlay.Static.Checkpoint.SectorObjectBuilder.Name;
                    checkpoint.SectorObjectBuilder.Position = MyGuiScreenGamePlay.Static.Checkpoint.SectorObjectBuilder.Position;
                }
            }

            checkpoint.CurrentSector = MyGuiScreenGamePlay.Static.GetSectorIdentifier();
            checkpoint.CheckpointName = null;
            checkpoint.PlayerObjectBuilder = MySession.Static.Player.GetObjectBuilder(true);
            checkpoint.SessionObjectBuilder = GetObjectBuilder();
            checkpoint.EventLogObjectBuilder = EventLog.GetObjectBuilder();
            checkpoint.FactionRelationChangesBuilder = FactionRelationChanges.GetObjectBuilders();
            checkpoint.GameTime = GameDateTime;
            checkpoint.ActiveMissionID = MyMissions.ActiveMission == null ? -1 : (int)MyMissions.ActiveMission.ID;
            if (CanSaveAndLoadSessionInventory)
            {
                checkpoint.InventoryObjectBuilder = Inventory.GetObjectBuilder(true);
            }
            else
            {
                checkpoint.InventoryObjectBuilder = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), MyInventory.DEFAULT_MAX_ITEMS);
            }
            MotherShipPosition.Save(checkpoint.Dictionary);

            if (MyMultiplayerGameplay.GameType == MyGameTypes.Story)
            {
                MyGuiScreenGamePlay.Static.Checkpoint.CopyCoopPlayers(checkpoint);
            }

            CheckEntityIds(checkpoint);
            return checkpoint;
        }

        public void GameOver()
        {
            GameOver(MyTextsWrapperEnum.MP_YouHaveBeenKilled);
        }


        public void GameOver(MyTextsWrapperEnum? customMessage)
        {
            if (!MyGuiManager.IsScreenOfTypeOpen(typeof(MyGuiScreenGameOver)))
            {
                MyGuiManager.AddScreen(new MyGuiScreenGameOver(1.0f, customMessage));
            }
        }

        ///////////////////// NEW STUFF //////////////////////////
        #region NEW_GAME

        /// <summary>
        /// Initializes a new single player session and start new game
        /// </summary>
        public static MyServerAction StartNewGame(MyGameplayDifficultyEnum difficulty, MyMissionID startMission = MyMissionID.EAC_SURVEY_SITE)
        {
            MySession.Static = new MySinglePlayerSession(difficulty);
            MySession.Static.Init();
            return MySession.Static.NewGame(startMission);
        }

        private static void TryPause()
        {
            if (!MyMinerGame.IsPaused() && MyGuiScreenGamePlay.Static != null)
            {
                MyMinerGame.SwitchPause();
            }
        }

        public MyServerAction NewGame(MyMissionID startMission = MyMissionID.EAC_SURVEY_SITE)
        {
            TryPause();

            MyMwcVector3Int? startSector = null;
            startSector = MyMissions.GetMissionByID(startMission).Location.Sector;

            var checkpoint = MyLocalCache.NewGameCheckpoint();
            if (checkpoint == null)
            {
                throw new MyDataCorruptedException("Checkpoint cannot be loaded from content");
            }
            // Set proper start sector
            checkpoint.CurrentSector.Position = startSector.Value;

            NewGameStarted(checkpoint, startMission);
            return null; // Finished synchronously
        }

        private MyServerAction NewGameStarted(MyMwcObjectBuilder_Checkpoint checkpoint, MyMissionID missionId)
        {
            MyLocalCache.ClearCurrentSave();
            MyClientServer.LoggedPlayer.HasAnyCheckpoints = false;

            Debug.Assert(checkpoint.CurrentSector.UserId == null, "New game checkpoint.CurrentSector must be story sector");

            var cachedSector = MyLocalCache.LoadSector(checkpoint.CurrentSector);
            if (cachedSector != null)
            {
                checkpoint.SectorObjectBuilder = cachedSector;
                checkpoint.CurrentSector.UserId = MyClientServer.LoggedPlayer.GetUserId();
                ReloadGameplayNewGame(checkpoint, missionId);
                return null;
            }
            throw new MyDataCorruptedException("New game story checkpoint does not contain first sector!");
        }

        private void NewGameSectorLoaded(MyMwcObjectBuilder_Checkpoint checkpoint, byte[] sectorData, MyMissionID missionId)
        {
            checkpoint.SectorObjectBuilder = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Sector>(sectorData);

            // Save sector to cache
            MyLocalCache.Save(null, checkpoint.SectorObjectBuilder, checkpoint.CurrentSector);

            checkpoint.CurrentSector.UserId = MyClientServer.LoggedPlayer.GetUserId(); //TODO: should this be send by server?
            ReloadGameplayNewGame(checkpoint, missionId);
        }

        private void ReloadGameplayNewGame(MyMwcObjectBuilder_Checkpoint checkpoint, MyMissionID missionId)
        {
            MyGuiScreenGamePlay.ReloadGameplayScreen(checkpoint, MyMwcStartSessionRequestTypeEnum.NEW_STORY, MyGuiScreenGamePlayType.GAME_STORY, missionId, MyMwcTravelTypeEnum.SOLAR);
        }

        #endregion

        #region LOAD_GAME

        /// <summary>
        /// Initialized new single player session and loads load checkpoint
        /// </summary>
        public static MyServerAction StartLastCheckpoint()
        {
            if (MyClientServer.LoggedPlayer.IsDemoUser())
            {
                return MySession.StartNewGame(MyGameplayConstants.GameplayDifficultyProfile.GameplayDifficulty);
            }
            else
            {
                // Difficulty doesn't matter, will be rewritten by checkpoint
                MySession.Static = new MySinglePlayerSession(MyGameplayDifficultyEnum.NORMAL);
                MySession.Static.Init();
                try
                {
                    return MySession.Static.LoadLastCheckpoint();
                }
                catch (MyDataCorruptedException)
                {
                    var screen = new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SaveCorruptedText, MyTextsWrapperEnum.SaveCorruptedCaption, MyTextsWrapperEnum.Ok, OnErrorClick);
                    MyGuiManager.AddScreen(screen);
                    return null;
                }
            }
        }

        private static void OnErrorClick(MyGuiScreenMessageBoxCallbackEnum result)
        {
            MyGuiManager.BackToMainMenu();
        }

        public MyServerAction LoadLastCheckpoint()
        {
            TryPause();

            var checkpoint = MyLocalCache.LoadCheckpoint();
            if (checkpoint != null)
            {
                CheckpointLoaded(checkpoint);
            }
            else
            {
                throw new MyDataCorruptedException("Last checkpoint corrupted");
            }
            return null;
        }


        public static MyGuiScreenLoading StartJoinMultiplayerSession(MyGameTypes gameType, MyGameplayDifficultyEnum difficulty, MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            // Temporary
            MySession.Static = new MySinglePlayerSession(difficulty);
            MySession.Static.Init();
            return MySession.Static.JoinMultiplayerSession(gameType, checkpoint);
        }

        private MyGuiScreenLoading JoinMultiplayerSession(MyGameTypes gameType, MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            MyMultiplayerGameplay.Static.IsHost = false;
            //var checkpoint = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.Checkpoint, null) as MyMwcObjectBuilder_Checkpoint;

            // This is just dummy ship for load
            //var ship = MyMwcObjectBuilder_SmallShip_Player.CreateDefaultShip(MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, MySession.Static.Player.Faction);
            MySession.Static.Player.Faction = checkpoint.PlayerObjectBuilder.ShipObjectBuilder.Faction;

            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

            MyGuiScreenGamePlayType? gameplayType = null;
            MyMwcStartSessionRequestTypeEnum? sessionType = null;
            switch (gameType)
            {
                case MyGameTypes.Story:
                    gameplayType = MyGuiScreenGamePlayType.GAME_STORY;
                    sessionType = MyMwcStartSessionRequestTypeEnum.JOIN_FRIEND_STORY;
                    break;
                case MyGameTypes.Deathmatch:
                    gameplayType = MyGuiScreenGamePlayType.GAME_SANDBOX;
                    sessionType = MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS;
                    break;
                default:
                    break;
            }

            return MyGuiScreenGamePlay.ReloadGameplayScreen(checkpoint, sessionType, gameplayType);
        }

        private MyServerAction CheckpointLoaded(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            MyGameplayConstants.SetGameplayDifficulty(checkpoint.SessionObjectBuilder.Difficulty);

            Debug.Assert((checkpoint.CheckpointName == null && checkpoint.CurrentSector.UserId != null) || (checkpoint.CheckpointName != null && checkpoint.CurrentSector.UserId == null));

            var cacheSector = MyLocalCache.LoadSector(checkpoint.CurrentSector);
            if (cacheSector != null)
            {
                checkpoint.SectorObjectBuilder = cacheSector;
                MyGuiScreenGamePlay.ReloadGameplayScreen(checkpoint, MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT, MyGuiScreenGamePlayType.GAME_STORY);
                return null;
            }
            else
            {
                // Checkpoint and sector is stored on filesystem
                throw new MyDataCorruptedException("Last checkpoint sector corrupted");

                //MyServerAction loadAction = new MyServerAction(MyTextsWrapperEnum.EnterSectorInProgressPleaseWait);
                //loadAction.BeginAction = c => c.BeginLoadSector(null, c);
                //loadAction.EndAction = (c, r) => LastCheckpointSectorLoaded(checkpoint, c.EndLoadSector(r));
                //loadAction.Start();
                //return loadAction;
            }
        }

        private void LastCheckpointSectorLoaded(MyMwcObjectBuilder_Checkpoint checkpoint, byte[] sectorData)
        {
            checkpoint.SectorObjectBuilder = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Sector>(sectorData);
            MyGuiScreenGamePlay.ReloadGameplayScreen(checkpoint, MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT, MyGuiScreenGamePlayType.GAME_STORY);
        }

        // This method works on foreground, on current thread
        public MyMwcObjectBuilder_SectorObjectGroups LoadSectorGroups(MyMwcVector3Int sectorPosition)
        {
            var sectorId = new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.STORY, null, sectorPosition, String.Empty);
            var sector = MyLocalCache.LoadSector(sectorId);
            int? version = sector != null ? (int?)sector.Version : null;

            var groupBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.SectorObjectGroups, null) as MyMwcObjectBuilder_SectorObjectGroups;

            if (sector == null)
            {
                throw new MyDataCorruptedException("Sector cannot be loaded");
            }

            // Server said: use cache
            groupBuilder.Generated = sector.Generated;
            groupBuilder.Entities = sector.SectorObjects;
            groupBuilder.Groups = sector.ObjectGroups;
            return groupBuilder;
        }

        #endregion

        #region TRAVEL

        public MyServerAction Travel(MyMwcTravelTypeEnum travelType, MyMwcSectorIdentifier targetSector)
        {
            Debug.Assert(targetSector.SectorType == MyMwcSectorTypeEnum.STORY, "Travel is allowed only in story");

            TryPause();

            var sector = MyLocalCache.LoadSector(targetSector);

            if (sector == null)
            {
                var storySectorId = targetSector;
                storySectorId.UserId = null;
                sector = MyLocalCache.LoadSector(storySectorId);
            }

            if (sector != null)
            {
                TravelSectorLoaded(travelType, targetSector.Position, null, sector);
                return null;
            }
            else // Not using server...so sector is "from generator"
            {
                TravelSectorLoaded(travelType, targetSector.Position, null, MyMwcObjectBuilder_Sector.UseGenerator());
                return null;
            }
        }

        private void TravelSectorLoaded(MyMwcTravelTypeEnum travelType, MyMwcVector3Int targetSector, byte[] sectorDataResponse, MyMwcObjectBuilder_Sector cachedSector)
        {
            var checkpoint = GetCheckpointBuilder(false);
            checkpoint.SectorObjectBuilder = cachedSector;

            // Overwrite current sector
            checkpoint.CurrentSector.Position = targetSector;
            UpdatePlayerPosition(travelType, ref checkpoint.PlayerObjectBuilder.ShipObjectBuilder.PositionAndOrientation.Position);

            if (sectorDataResponse != null)
            {
                checkpoint.SectorObjectBuilder = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Sector>(sectorDataResponse);
                MyLocalCache.Save(null, checkpoint.SectorObjectBuilder, checkpoint.CurrentSector);
            }

            // Change NEW_STORY to LOAD_CHECKPOINT, because it's necessary, travel is never new story
            MyMwcStartSessionRequestTypeEnum? sessionStart = null;
            if (MyGuiScreenGamePlay.Static != null)
            {
                sessionStart = MyGuiScreenGamePlay.Static.GetSessionType();
                if (sessionStart.HasValue && sessionStart.Value == MyMwcStartSessionRequestTypeEnum.NEW_STORY)
                {
                    sessionStart = MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT;
                }
            }

            MyGuiScreenGamePlay.ReloadGameplayScreen(checkpoint, sessionStart, null, null, travelType);
        }

        private void UpdatePlayerPosition(MyMwcTravelTypeEnum travelType, ref Vector3 position)
        {
            switch (travelType)
            {
                case MyMwcTravelTypeEnum.SOLAR:
                    position = new Vector3(0, 0, 0);
                    break;

                case MyMwcTravelTypeEnum.NEIGHBOUR:
                    position = MyMwcUtils.GetNeighbourSectorShipPosition(position);
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        #endregion

        #region SAVE_GAME

        public MyServerAction SaveLastCheckpoint(bool createChapter = false)
        {
            if (MyMultiplayerGameplay.IsRunning && !MyMultiplayerGameplay.Static.IsHost)
                return null;

            var lastChapterTime = MyLocalCache.GetLastChapterTimestamp();
            var nextChapterTime = lastChapterTime + TimeSpan.FromHours(3);

            if (MyMissions.ActiveMission == null && DateTime.Now > nextChapterTime)
            {
                createChapter = true;
            }

            MyMwcSectorIdentifier sectorId = MyGuiScreenGamePlay.Static.GetSectorIdentifier();

            if (CanBeSaved(false, sectorId, false))
            {
                var notification = new MyHudNotification.MyNotification(Localization.MyTextsWrapperEnum.SavingSectorToServer, 2500);
                MyHudNotification.AddNotification(notification);

                var checkpoint = GetCheckpointBuilder(true);
                checkpoint.CurrentSector.UserId = MyClientServer.LoggedPlayer.GetUserId(); // Saving players checkpoint
                
                if (MySession.PlayerShip != null && checkpoint.PlayerObjectBuilder.ShipObjectBuilder != null)
                {
                    float refilRatio = 0.5f;

                    var ship = checkpoint.PlayerObjectBuilder.ShipObjectBuilder;
                    ship.ArmorHealth = MathHelper.Clamp(ship.ArmorHealth, refilRatio * MySession.PlayerShip.MaxArmorHealth, MySession.PlayerShip.MaxArmorHealth);
                    ship.ShipHealthRatio = MathHelper.Clamp(ship.ShipHealthRatio, refilRatio, 1.0f);
                    ship.Fuel = MathHelper.Clamp(ship.Fuel, refilRatio * MySession.PlayerShip.MaxFuel, MySession.PlayerShip.MaxFuel);
                    ship.Oxygen = MathHelper.Clamp(ship.Oxygen, refilRatio * MySession.PlayerShip.MaxOxygen, MySession.PlayerShip.MaxOxygen);
                    checkpoint.PlayerObjectBuilder.Health = MathHelper.Clamp(checkpoint.PlayerObjectBuilder.Health, refilRatio * 100, 100);
                }

                // Need to store actual checkpoint...because when we travel, we receive only sector, not checkpoint
                MyGuiScreenGamePlay.Static.AddEnterSectorResponse(checkpoint, null);

                Debug.Assert(checkpoint.CurrentSector.UserId != null, "Saving last checkpoint as story");
                UpdatePlayerStartDummy(checkpoint);
                
                MyLocalCache.SaveCheckpoint(checkpoint, createChapter);
                checkpoint.SectorObjectBuilder = null; // Don't save sector
            }
            return null;
        }

        #endregion

        #region SANDBOX
        public static MyServerAction StartSandbox(MyMwcVector3Int position, int? userId)
        {
            MySession.Static = new MySinglePlayerSession(MyGameplayDifficultyEnum.NORMAL);
            MySession.Static.Init();
            return MySession.Static.LoadSandbox(position, userId);
        }

        public static MyServerAction StartSandboxMission(MyMissionID startMission)
        {
            MySession.Static = new MySinglePlayerSession(MyGameplayDifficultyEnum.NORMAL);
            MySession.Static.Init();
            return MySession.Static.LoadSandboxMission(startMission);
        }

        public MyServerAction LoadSandbox(MyMwcVector3Int position, int? userId)
        {
            return LoadSandbox(position, userId, null);
        }

        public MyServerAction LoadSandboxMission(MyMissionID startMission)
        {
            var startSector = MyMissions.GetMissionByID(startMission).Location.Sector;
            return LoadSandbox(startSector, null, startMission);
        }

        private MyServerAction LoadSandbox(MyMwcVector3Int position, int? userId, MyMissionID? startMission)
        {
            var sectorId = new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.SANDBOX, userId, position, null);
            var sector = MyLocalCache.LoadSector(sectorId);
            int? version = sector != null ? (int?)sector.Version : null;

            MyMwcStartSessionRequestTypeEnum startSessionType = userId == MyClientServer.LoggedPlayer.GetUserId() ? MyMwcStartSessionRequestTypeEnum.SANDBOX_OWN : MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS;

            if (userId != null)
            {
                MyServerAction loadAction = new MyServerAction(MyTextsWrapperEnum.EnterSectorInProgressPleaseWait, TimeSpan.FromSeconds(360));
                loadAction.BeginAction = c => c.BeginLoadSandbox(position, userId, version, null, c);
                loadAction.EndAction = (c, r) => SandboxSectorLoaded(position, MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Checkpoint>(c.EndLoadSandbox(r)), sector, startSessionType, startMission);
                loadAction.Start();
                return loadAction;
            }
            else if (sector != null)
            {
                var checkpoint = MyLocalCache.MultiplayerCheckpoint();
                if (checkpoint == null)
                {
                    throw new MyDataCorruptedException("Cannot load MP checkpoint");
                }

                if (!MySteam.IsActive && MyClientServer.IsMwAccount)
                {
                    checkpoint.PlayerObjectBuilder.Money = (float)MySectorServiceClient.GetCheckedInstance().GetGameMoney();
                    MySectorServiceClient.SafeClose();
                }

                checkpoint.SectorObjectBuilder = sector;
                SandboxSectorLoaded(position, checkpoint, sector, startSessionType, startMission);
                return null;
            }
            else
            {
                throw new MyDataCorruptedException("Cannot load sandbox/MP sector");
            }
        }

        private void SandboxSectorLoaded(MyMwcVector3Int targetSector, MyMwcObjectBuilder_Checkpoint checkpoint, MyMwcObjectBuilder_Sector cachedSector, MyMwcStartSessionRequestTypeEnum startSessionType, MyMissionID? startMission)
        {
            if (checkpoint.SectorObjectBuilder == null) // Server said, use cache
            {
                checkpoint.SectorObjectBuilder = cachedSector;
            }
            else
            {
                MyLocalCache.SaveCheckpoint(checkpoint);
            }

            MyGuiScreenGamePlay.ReloadGameplayScreen(checkpoint, startSessionType, MyGuiScreenGamePlayType.GAME_SANDBOX, startMission);
        }

        public static bool Is25DSector
        {
            get
            {
                return MyClientServer.MW25DEnabled && (Static != null) && Static.Is2DSector;
            }
        }


        #endregion

    }
}