#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.World.Global;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Networking;
using MinerWars.AppCode.Game.Entities.WayPoints;

#endregion


namespace MinerWars.AppCode.Game.Missions
{
    delegate void MissionHandler(MyMissionBase sender);

    abstract class MyMissionBase
    {
        bool IsLoaded = false; //DEBUG!!

        /// <summary>
        /// Name of achievement, when mission/objective is finished achievement is granted
        /// </summary>
        public string AchievementName { get; set; }

        public struct MyMissionLocationEntityIdentifier
        {
            public uint? LocationEntityId;
            public string LocationEntityName;

            private MyMissionLocationEntityIdentifier(uint? locationEntityId, string locationEntityName)
            {
                LocationEntityId = locationEntityId;
                LocationEntityName = locationEntityName;
            }

            public MyMissionLocationEntityIdentifier(uint locationEntityId)
                : this(locationEntityId, null)
            {
            }

            public MyMissionLocationEntityIdentifier(string locationEntityName)
                : this(null, locationEntityName)
            {
            }

            public override string ToString()
            {
                if (LocationEntityId != null)
                {
                    return "ID: " + LocationEntityId.Value;
                }
                else
                {
                    return "NAME: " + LocationEntityName;
                }
            }
        }

        public class MyMissionLocation
        {
            public const string MADELYN_HANGAR = "MDHangar";
            public const float MADELYN_HANGAR_RADIUS = 50;

            public MyMwcVector3Int Sector { get; protected set; }
            public MyMissionLocationEntityIdentifier LocationEntityIdentifier { get; protected set; }
            public MyEntity Entity;

            public MyMissionLocation(MyMwcVector3Int sector, uint locationEntityId)
            {
                Sector = sector;
                LocationEntityIdentifier = new MyMissionLocationEntityIdentifier(locationEntityId);
            }

            public MyMissionLocation(MyMwcVector3Int sector, string locationEntityName)
            {
                Sector = sector;
                LocationEntityIdentifier = new MyMissionLocationEntityIdentifier(locationEntityName);
            }
        }

        public StringBuilder DebugName { get; protected set; }
        public MyMissionID ID { get; protected set; }

        [Obsolete]
        public StringBuilder NameTemp; //TODO: Remove when all mission has proper description set
        private MyTextsWrapperEnum m_name;
        public MyTextsWrapperEnum Name
        {
            get { return m_name; }
            protected set { m_name = value; NameTemp = MyTextsWrapper.Get(value); }
        }

        public StringBuilder HudNameTemp = new StringBuilder("");
        private MyTextsWrapperEnum? m_hudName = null;
        public MyTextsWrapperEnum? HudName
        {
            get { return m_hudName; }
            set { m_hudName = value; HudNameTemp = (m_hudName == null) ? new StringBuilder("") : MyTextsWrapper.Get(m_hudName.Value); }
        }

        public void ReloadName()
        {
            NameTemp = MyTextsWrapper.Get(Name);
            HudNameTemp = (m_hudName == null) ? new StringBuilder("") : MyTextsWrapper.Get(m_hudName.Value);
        }


        [Obsolete]
        public StringBuilder DescriptionTemp; //TODO: Remove when all mission has proper description set
        private MyTextsWrapperEnum _description;
        public MyTextsWrapperEnum Description
        {
            get { return _description; }
            protected set { _description = value; DescriptionTemp = MyTextsWrapper.Get(value); }
        }
        public MyTexture2D Icon { get; protected set; }
        public MyMissionID[] RequiredMissions { get; set; }
        public MyMissionID[] RequiredUncompletedMissions { get; set; }
        public MyMissionID[] RequiredMissionsForSuccess { get; set; }
        public MyActorEnum[] RequiredActors { get; set; }   //Actors will be put into the scene while Load
        public MyMissionLocation Location { get; set; }
        public DateTime RequiredTime { get; protected set; }
        public MyMwcVector3Int SectorLocation { get; protected set; }
        public List<uint> MissionEntityIDs { get; protected set; }
        public List<uint> RequiredEntityIDs { get; protected set; }
        public Audio.Dialogues.MyDialogueEnum? SuccessDialogId { get; protected internal set; }
        public Audio.Dialogues.MyDialogueEnum? StartDialogId { get; protected internal set; }
        protected float? m_radiusOverride;

        public bool ShowNavigationMark { get; set; }
        public bool IsSideMission { get; set; }
        public MyGuitargetMode GuiTargetMode { get; set; }
        public bool MovePlayerToMadelynHangar { get; set; }

        /// <summary>
        /// Mission timer is started automatically on mission start.
        /// </summary>
        public MyMissionTimer MissionTimer;

        /// <summary>
        /// Countdown timer has to be started manually when it is needed.
        /// </summary>
        public MyCountdownTimer CountdownTimer = new MyCountdownTimer();

        public event MissionHandler OnMissionSuccess;
        public event MissionHandler OnMissionLoaded;
        public event MissionHandler OnMissionCleanUp;
        public event MissionHandler OnMissionUpdate;

        public List<MyMissionComponent> Components { get; set; }

        public MyMissionBase()
        {
            ShowNavigationMark = true;
            IsSideMission = false;
            MissionTimer = new MyMissionTimer();
            RequiredUncompletedMissions = new MyMissionID[0];
            RequiredMissionsForSuccess = new MyMissionID[0];
            MissionEntityIDs = new List<uint>();
            RequiredEntityIDs = new List<uint>();
            GuiTargetMode = MyGuitargetMode.Objective;
            MovePlayerToMadelynHangar = true;
            RequiredActors = new MyActorEnum[0];

            Components = new List<MyMissionComponent>();
        }

        public virtual bool IsCompleted()
        {
            Debug.Assert(MySession.Static.EventLog != null);
            return MySession.Static.EventLog != null && MySession.Static.EventLog.IsMissionFinished(ID);
        }

        public virtual bool IsAvailable()
        {
            Debug.Assert(MySession.Static.EventLog != null);
            return MySession.Static.EventLog != null &&
                MySession.Static.GameDateTime >= RequiredTime &&
                !MySession.Static.EventLog.IsMissionFinished(ID) &&
                MySession.Static.EventLog.AreMissionsFinished(RequiredMissions) &&
                MySession.Static.EventLog.GetFinishedMissionCount(RequiredUncompletedMissions) == 0;
        }


        public virtual void Load()
        {
            Debug.Assert(MyGuiScreenGamePlay.Static != null, "Loading mission from menu!");
            Debug.Assert(IsLoaded == false);
            IsLoaded = true;

            //AddActorsToPlayerFriends();

            MissionTimer.Reset();
            MissionTimer.Start();

            if (OnMissionLoaded != null)
            {
                OnMissionLoaded(this);
            }

            foreach (var component in Components)
            {
                component.Load(this);
            }

            if (StartDialogId != null)
            {
                MyScriptWrapper.PlayDialogue(StartDialogId.Value);
            }
        }

        public virtual void Update()
        {
            Debug.Assert(IsLoaded == true);

            MissionTimer.Update();
            if (CountdownTimer != null)
            {
                CountdownTimer.Update();
            }

            var handler = OnMissionUpdate;
            if (handler != null)
            {
                handler(this);
            }

            foreach (var component in Components)
            {
                component.Update(this);
            }
        }

        /// <summary>
        /// Causes mission to success
        /// </summary>
        /// <param name="userId">user that causes this success of this mission</param>
        public virtual void Success()
        {
            if (OnMissionSuccess != null)
            {
                OnMissionSuccess(this);
            }
            MissionTimer.End();

            foreach (var component in Components)
            {
                component.Success(this);
            }

            if (!String.IsNullOrEmpty(AchievementName))
            {
                MySteamStats.SetAchievement(AchievementName);
            }

            //Unloaded in Mission.UpdateActiveObjectives
        }

        public virtual void Fail(MyTextsWrapperEnum? customMessage = null)
        {
            if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost)
            {
                MyMultiplayerGameplay.Static.SendMissionProgress(this, CommonLIB.AppCode.Networking.Multiplayer.MyMissionProgressType.Fail);
            }

            //Unloaded in Mission.UpdateActiveObjectives
        }

        public virtual void Unload()
        {
            Debug.Assert(IsLoaded == true);
            IsLoaded = false;

            SetLocationVisibility(false);


            var handler = OnMissionCleanUp;
            if (handler != null)
            {
                handler(this);
            }
            MissionTimer.ClearActions();

            foreach (var component in Components)
            {
                component.Unload(this);
            }

            if (Location != null)
                Location.Entity = null;
        }

        public override string ToString()
        {
            return NameTemp.ToString();
        }

        public virtual bool IsMissionEntity(MyEntity target)
        {
            // Show hud marker for location dummy in mission colors
            return !MySession.Static.EventLog.IsMissionFinished(ID) &&
                (HasLocationEntity() && Location.Entity == target ||
                MissionEntityIDs != null && target.EntityId.HasValue && MissionEntityIDs.Contains(target.EntityId.Value.NumericValue));
        }

        public virtual bool IsMissionEntityNotification(MyEntity entity, MySmallShipInteractionActionEnum action)
        {
            return false;
        }

        public virtual void AddSolarMapMarks(SolarSystem.MySolarSystemMapData data)
        {

        }

        public bool HasLocationEntity()
        {
            return MyGuiScreenGamePlay.Static != null && Location != null && MyGuiScreenGamePlay.Static.IsCurrentSector(Location.Sector) && Location.Entity != null;
        }

        public virtual void SetLocationVisibility(bool visible)
        {
            // Show or hide location dummy
            if (Location != null)
            {
                if (MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.IsCurrentSector(Location.Sector))
                {
                    Location.Entity = MyEntities.GetEntityByMissionLocationIdentifier(Location.LocationEntityIdentifier);
                    if (Location.Entity != null)
                    {
                        SetLocationVisibility(visible, Location.Entity, GuiTargetMode);
                    }
                }
            }
        }

        public void SetLocationVisibility(bool visible, MyEntity entity, MyGuitargetMode guiTargetMode)
        {
            if (visible)
            {
                MyHud.RemoveText(entity);
                MyHud.AddText(entity, HudNameTemp ?? NameTemp, guiTargetMode);
            }
            else
            {
                entity.UpdateHudMarker(true);
            }
        }

        public void RemoveComponent(MyMissionComponent component)
        {
            Components.Remove(component);
            component.Unload(this);
        }

        public void InsertRequiredActors()
        {
            MyMwcLog.WriteLine("InsertRequiredActors - START");

            foreach (MyActorEnum actor in RequiredActors)
            {
                switch (actor)
                {
                    //Insert Madelyn
                    case MyActorEnum.MADELYN:
                        {
                            if (!MyEntities.EntityExists("Madelyn"))
                            {
                                MyMwcLog.WriteLine("Insert Madelyne - START");

                                // Holds ids for remap
                                IMyEntityIdRemapContext remapContext = new MyEntityIdRemapContext();

                                //MyMwcObjectBuilder_SectorObjectGroups groups = MySectorGenerator.LoadSectorGroups(MyTemplateGroups.GetGroupSector(MyTemplateGroupEnum.Madelyn));
                                MyMwcObjectBuilder_SectorObjectGroups groups = MySession.Static.LoadSectorGroups(MyTemplateGroups.GetGroupSector(MyTemplateGroupEnum.Madelyn));
                                System.Diagnostics.Debug.Assert(groups.Groups.Count > 0);

                                MyMwcObjectBuilder_ObjectGroup madelynGroup = groups.Groups[0];
                                groups.RemapEntityIds(remapContext);
                                IEnumerable<MyMwcObjectBuilder_Base> rootObjects = madelynGroup.GetRootBuilders(groups.Entities);

                                List<MyMwcObjectBuilder_Base> clonedList = new List<MyMwcObjectBuilder_Base>();
                                foreach (MyMwcObjectBuilder_Base o in rootObjects)
                                {
                                    // Clone
                                    var clone = o.Clone() as MyMwcObjectBuilder_Base;
                                    // we want Madelyn's prefab container as first builder
                                    if (clone is MyMwcObjectBuilder_PrefabContainer)
                                    {
                                        clonedList.Insert(0, clone);
                                    }
                                    else
                                    {
                                        clonedList.Add(clone);
                                    }
                                }

                                System.Diagnostics.Debug.Assert(clonedList.Count > 0 && clonedList[0] is MyMwcObjectBuilder_PrefabContainer);

                                // create Madelyn's prefab container
                                MyEntity madelynMothership = MyEntities.CreateFromObjectBuilder(MyTextsWrapper.Get(MyTextsWrapperEnum.Actor_Madelyn).ToString(), clonedList[0], ((MyMwcObjectBuilder_Object3dBase)clonedList[0]).PositionAndOrientation.GetMatrix());
                                madelynMothership.FindChild(MyMissionLocation.MADELYN_HANGAR).DisplayName = MyTextsWrapper.Get(MyTextsWrapperEnum.Sapho).ToString();
                                madelynMothership.SetName(MyActorConstants.GetActorName(MyActorEnum.MADELYN));
                                Matrix madelynMothershipWorldInv = Matrix.Invert(madelynMothership.WorldMatrix);

                                List<MinerWars.AppCode.Game.Entities.WayPoints.MyWayPoint> waypoints = new List<Entities.WayPoints.MyWayPoint>();

                                // create other entities as children of Madelyn's prefab container
                                for (int i = 1; i < clonedList.Count; i++)
                                {
                                    System.Diagnostics.Debug.Assert(clonedList[i] is MyMwcObjectBuilder_Object3dBase);
                                    MyEntity childEntity = MyEntities.CreateFromObjectBuilder(null, clonedList[i], ((MyMwcObjectBuilder_Object3dBase)clonedList[i]).PositionAndOrientation.GetMatrix());
                                    childEntity.SetLocalMatrix(childEntity.WorldMatrix * madelynMothershipWorldInv);
                                    madelynMothership.Children.Add(childEntity);
                                    if (childEntity is MinerWars.AppCode.Game.Entities.WayPoints.MyWayPoint)
                                    {
                                        waypoints.Add(childEntity as MinerWars.AppCode.Game.Entities.WayPoints.MyWayPoint);
                                    }
                                }

                                // connect waypoints of Madelyn's prefab container
                                foreach (var waypoint in waypoints)
                                {
                                    waypoint.ResolveLinks();
                                }

                                // set correct Madelyn's position and add to scene
                                madelynMothership.SetWorldMatrix(MyPlayer.FindMothershipPosition());
                                madelynMothership.Link();
                                MyEntities.Add(madelynMothership);

                                MyMwcLog.WriteLine("Insert Madelyne - END");
                            }
                            else
                            {
                                MyMwcLog.WriteLine("Insert Madelyne - Madelyne already loaded");
                            }

                            MyWayPointGraph.RecreateWaypointsAroundMadelyn();
                        }
                        break;

                    //Insert Marcus
                    case MyActorEnum.MARCUS:
                        InsertFriend(MyActorEnum.MARCUS);
                        break;

                    //Insert RavenGirl
                    case MyActorEnum.TARJA:
                        InsertFriend(MyActorEnum.TARJA);
                        break;

                    //Insert RavenGuy
                    case MyActorEnum.VALENTIN:
                        InsertFriend(MyActorEnum.VALENTIN, MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER);
                        break;

                    default:
                        System.Diagnostics.Debug.Assert(false, "Uknown actor to insert!");
                        break;
                }
            }

            MyMwcLog.WriteLine("InsertRequiredActors - END");
        }

        public void MovePlayerAndFriendsToHangar()
        {
            if (MovePlayerToMadelynHangar && IsPlayerShipNearMadelyn())
            {
                MovePlayerAndFriendsToHangar(RequiredActors);
            }
        }

        public static void MovePlayerAndFriendsToHangar(MyActorEnum[] requiredActors)
        {
          //  if (IsPlayerShipNearMadelyn())
            {
                MoveEntityToHangar(MySession.PlayerShip, 30);

                float offset = 50;
                foreach (MyActorEnum actor in requiredActors)
                {
                    if (actor == MyActorEnum.MADELYN)
                        continue;

                    MyEntity actorEntity;
                    if (MyEntities.TryGetEntityByName(MyActorConstants.GetActorName(actor), out actorEntity))
                    {
                        MoveEntityToHangar(actorEntity, offset);
                        offset += 20;
                    }
                }
            }
        }

        public static void RemoveFriends()
        {
            while (MySession.PlayerFriends.Count > 0)
            {
                MySession.PlayerFriends[0].MarkForClose();
            }
        }

        public static MySmallShipBot InsertFriend(MyActorEnum actorEnum, MyMwcObjectBuilder_SmallShip_TypesEnum? shipType = null)
        {
            MyMwcObjectBuilder_SmallShip_TypesEnum selectedShipType;
            if (shipType.HasValue)
            {
                selectedShipType = shipType.Value;
            }
            else
            {
                switch (actorEnum)
                {
                    case MyActorEnum.TARJA:
                        selectedShipType = MyMwcObjectBuilder_SmallShip_TypesEnum.DOON;
                        break;
                    case MyActorEnum.VALENTIN:
                        selectedShipType = MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER;
                        break;
                    default:
                        selectedShipType = MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG;
                        break;
                }
            }

            MySmallShipBot bot;

            string actorSystemName = MyActorConstants.GetActorName(actorEnum);

            if (!MyEntities.EntityExists(actorSystemName))
            {
                MyMwcLog.WriteLine("Insert " + actorSystemName + " - START");

                bot = MyGuiScreenGamePlay.Static.CreateFriend(MyTextsWrapper.Get(MyActorConstants.GetActorProperties(actorEnum).DisplayName).ToString(), 100000, 1, selectedShipType);
                bot.SetName(actorSystemName);
                bot.Save = true;
                bot.LeaderLostEnabled = true;
                bot.IsDestructible = false;
                bot.SetWorldMatrix(Matrix.CreateWorld(bot.GetPosition(), MySession.PlayerShip.WorldMatrix.Forward, MySession.PlayerShip.WorldMatrix.Up));
                bot.Faction = MyMwcObjectBuilder_FactionEnum.Rainiers;
                bot.AIPriority = -5;

                MyMwcLog.WriteLine("Insert " + actorSystemName + " - END");
            }
            else
            {
                bot = MyEntities.GetEntityByName(actorSystemName) as MySmallShipBot;
                bot.Save = true;
                bot.LeaderLostEnabled = true;     // Not persisted for now
                MyMwcLog.WriteLine("Insert " + actorSystemName + " - already loaded");
            }

            //Init smaller box physics because of following player problems
            bot.InitPhysics(1 / MySmallShipConstants.FRIEND_SMALL_SHIP_MODEL_SCALE, bot.ShipTypeProperties.Visual.MaterialType);
            bot.Follow(MySession.PlayerShip);

            if (!MySession.PlayerFriends.Contains(bot))
            {
                MySession.PlayerFriends.Add(bot);
            }

            Debug.Assert(bot.Save, "Bot must have save flag to work in coop");

            return bot;
        }

        private static void MoveEntityToHangar(MyEntity entity, float offset)
        {
            MyEntity hangarEntity;
            MyEntities.TryGetEntityByName(MyMissionLocation.MADELYN_HANGAR, out hangarEntity);
            MyPrefabHangar madelynHangar = hangarEntity as MyPrefabHangar;
            if (madelynHangar != null)
            {
                entity.SetPosition(madelynHangar.WorldVolume.Center - offset * madelynHangar.WorldMatrix.Forward);
            }
        }

        public static bool IsPlayerShipNearMadelyn()
        {
            MyEntity hangarEntity;
            MyEntities.TryGetEntityByName(MyMissionLocation.MADELYN_HANGAR, out hangarEntity);
            MyPrefabHangar madelynHangar = hangarEntity as MyPrefabHangar;
            if (madelynHangar != null)
            {
                float distance = Vector3.Distance(MySession.PlayerShip.GetPosition(), madelynHangar.GetPosition());
                if (distance < 2000)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
