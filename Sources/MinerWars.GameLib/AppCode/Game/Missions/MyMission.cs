using System.Collections.Generic;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.World.Global;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.World;
using System;
using System.Linq;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Prefabs;

namespace MinerWars.AppCode.Game.Missions
{
    [Flags]
    public enum MyMissionFlags
    {
        None  = 0,
        Story = 1 << 0,
        HiddenInSolarMap = 1 << 1,
    }

    abstract class MyMission : MyMissionBase
    {
        public MyMissionFlags Flags;
        protected List<MyObjective> m_objectives;
        protected List<MyObjective> m_activeObjectives;
        public static List<MyObjective> m_activeObjectiveBuffer = new List<MyObjective>();

        public List<MyObjective> Objectives
        {
            get { return m_objectives; }
        }

        public List<MyObjective> ActiveObjectives
        {
            get { return m_activeObjectives; }
        }

        public MyMission()
        {
            m_activeObjectives = new List<MyObjective>();
        }

        public virtual bool IsValid() 
        {
            return Objectives != null && Objectives.Count > 0 && Location != null;
        }

        /// <summary>
        /// Returns true when current sector is mission location
        /// </summary>
        public bool IsMainSector
        {
            get
            {
                return Location.Sector.Equals(MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position);
            }
        }

        public MyMwcVector3Int GetCurrentSector()
        {
            return MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position;
        }

        public override bool IsAvailable()
        {
            return (MyMissions.ActiveMission == null) && base.IsAvailable();
        }

        public virtual void ValidateIds()
        {
        }

        private void RefillMerchants()
        {
            foreach (var c in MyEntities.GetEntities().OfType<MyPrefabContainer>())
            {
                if (c.Inventory.TemplateType == null)
                    continue;

                foreach (var p in c.GetPrefabs(CategoryTypesEnum.OTHER))
                {
                    if (p.PrefabTypeFlag == PrefabTypesFlagEnum.Vendor)
                    {
                        c.RefillInventory();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Accept is called just once, only when player Accepts mission by button, or mission is Accepted from game. By calling Accept, player starts to play the mission.
        /// </summary>
        public virtual void Accept()
        {
            MyScriptWrapper.Log("Mission {0} started.", NameTemp);

            Debug.Assert(MyMissions.ActiveMission == null);

            MySession.Static.EventLog.AddMissionStarted(ID);

            RefillMerchants();
            InsertRequiredActors();
            Load();

            ValidateIds();

            MyMissions.CreateChapter = true;
            MyMissions.NeedsSave = true;
        }

        /// <summary>
        ///  Is called whenever the mission/submission is loaded or started. Initialize your members here, adjust properties of existing scene.
        /// </summary>
        public override void Load()
        {
            MyMwcLog.WriteLine("MyMission.Load - START");

            base.Load();

            if (MyFakes.SET_ACTOR_PROGRESS)
            {
                MyActorProgress.SetupActors(ID);
            }
            
            MyScriptWrapper.Log("Mission {0} loaded.", DebugName);

            MyMissions.ActiveMission = this;

            m_activeObjectives.Clear();

            UpdateActiveObjectives();

            MyGuiScreenGamePlay.Static.RefillMadelyn();

            //MyGlobalEvents.DisableAllGlobalEvents();

            MakeAllRequiredEntitiesIndestructible();

            if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost) // Change name of the game based on current mission
            {
                MyMultiplayerGameplay.Static.UpdateGameInfo();
            }

            MyMwcLog.WriteLine("MyMission.Load - END");
        }


        private void MakeAllRequiredEntitiesIndestructible()
        {
            foreach (var objective in Objectives)
            {
                if (objective.MakeEntityIndestructible && !MySession.Static.EventLog.IsMissionFinished(objective.ID))
                {
                    SetEntitiesIndestructible(objective.RequiredEntityIDs);
                    SetEntitiesIndestructible(objective.MissionEntityIDs);
                }
            }
        }

        private void SetEntitiesIndestructible(List<uint> entityIDs)
        {
            foreach (uint entityID in entityIDs)
            {
                MyEntity entity = MyEntities.GetEntityById(new MyEntityIdentifier(entityID));
                MyScriptWrapper.SetEntityDestructible(entity, false);
            }
        }

        /// <summary>
        /// Is called whenever mission/Submission gets obsolete. Clean your allocations/handlers here.
        /// </summary>
        public override void Unload()
        {
            MyScriptWrapper.Log("Mission {0} unloaded.", DebugName);
            System.Diagnostics.Debug.Assert(MyMissions.ActiveMission == this);

            base.Unload();
            foreach (var objective in m_activeObjectives)
            {
                objective.Unload();
            }

            m_activeObjectiveBuffer.Clear();

            MyMissions.ActiveMission = null;

            if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost) // Change name of the game based on current mission
            {
                MyMultiplayerGameplay.Static.UpdateGameInfo();
            }
            MarkedForUnload = false;
            m_failReason = null;
            //MyGlobalEvents.EnableAllGlobalEvents();
        }

        public override void Update()
        {
            m_activeObjectiveBuffer.Clear();
            foreach (var m in m_activeObjectives)
            {
                m_activeObjectiveBuffer.Add(m);
            }

            base.Update();
            foreach (var objective in m_activeObjectiveBuffer)
            {
                if (m_activeObjectives.Contains(objective))
                {
                    objective.Update();
                }
            }

            if (m_activeObjectiveBuffer.Count == 0 && !MarkedForUnload)
            {
                TestSuccess();
            }
        }

        public override void Success()
        {
            if (MarkedForUnload)
                return;

            MySession.Static.EventLog.MissionFinished(ID);

            if (MyMultiplayerGameplay.IsRunning)
            {
                MyMultiplayerGameplay.Static.SendMissionProgress(this, CommonLIB.AppCode.Networking.Multiplayer.MyMissionProgressType.Success);
            }

            base.Success();
            // temporary disabled
            //MyAudio.AddCue2D(MySoundCuesEnum.SfxMissionComplete);

            MyScriptWrapper.Log("Mission {0} success.", DebugName);

            MarkedForUnload = true;

            MyMissions.NeedsSave = true;
        }

        public bool MarkedForUnload { get; private set; }
        
        private MyTextsWrapperEnum? m_failReason;

        public override void Fail(MyTextsWrapperEnum? customMessage = null)
        {
            if (customMessage == null)
                customMessage = MyTextsWrapperEnum.MP_YouHaveBeenKilled;
            if (m_failReason != null)
                customMessage = m_failReason;  // first fail reason has priority (usually a custom one from the script)

            MyScriptWrapper.Log("Mission {0} fail.", DebugName);
            base.Fail(customMessage);

            m_failReason = customMessage;
            MySession.Static.GameOver(m_failReason);

            MarkedForUnload = true;
        }

        public void ObjectiveCompleted(MyObjective completedObjective)
        {
            UpdateActiveObjectives();

            //if (m_activeSubmissions.Count == 0)
            TestSuccess();

            if (completedObjective.ShowNotificationOnSuccess)
            {
                ShowObjectiveCompleted();
            }
        }

        private void TestSuccess()
        {
            if (MySession.Static.EventLog.AreMissionsFinished(RequiredMissionsForSuccess))
            {
                Success();
            }
        }

        public static void ShowObjectiveCompleted()
        {
            var notification = new MyHudNotification.MyNotification(
                MyTextsWrapperEnum.NotificationObjectiveComplete,
                MyHudNotification.GetCurrentScreen(),
                .85f,
                MyHudConstants.MISSION_FONT,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                15000); // TODO constant

            MyHudNotification.AddNotification(notification);
        }

        protected void UpdateActiveObjectives()
        {
            foreach (var objective in m_objectives)
            {
                if (objective.IsAvailable())
                {
                    if (!m_activeObjectives.Contains(objective))
                    {
                        // New submission add to active subbmissions
                        m_activeObjectives.Add(objective);

                        objective.InsertRequiredActors();
                        objective.SetLocationVisibility(true);

                        objective.Load();

                        if (MyMultiplayerGameplay.IsHosting)
                        {
                            MyMultiplayerGameplay.Static.SendMissionProgress(objective, MyMissionProgressType.NewObjective);
                        }
                    }
                }
                else
                {
                    if (m_activeObjectives.Remove(objective))
                    {
                        //This is the only place where objective is unloaded
                        objective.Unload();
                    }
                }
            }

            Debug.Assert(m_activeObjectives.Count <= 1, "Now there can be only one active objective");
        }

        public override void AddSolarMapMarks(MySolarSystemMapData data)
        {
            foreach (var objective in ActiveObjectives)
            {
                if (objective.Location != null && !MyGuiScreenGamePlay.Static.IsCurrentSector(objective.Location.Sector) && objective.ShowNavigationMark)
                {
                    var missionMark = new MySolarSystemMapNavigationMark(objective.Location.Sector, objective.NameTemp.ToString(), null, MyHudConstants.MISSION_MARKER_COLOR, TransparentGeometry.MyTransparentMaterialEnum.SolarMapNavigationMark) { VerticalLineColor = MyHudConstants.MISSION_MARKER_COLOR.ToVector4(), DirectionalTexture = MyHudTexturesEnum.DirectionIndicator_white };
                    data.NavigationMarks.Add(missionMark);
                }

                objective.AddSolarMapMarks(data);
            }

            if (Location != null && !MyGuiScreenGamePlay.Static.IsCurrentSector(Location.Sector) && ShowNavigationMark)
            {
                var missionMark = new MySolarSystemMapNavigationMark(Location.Sector, NameTemp.ToString(), null, MyHudConstants.ACTIVE_MISSION_SOLAR_MAP_COLOR, TransparentGeometry.MyTransparentMaterialEnum.SolarMapNavigationMark) { VerticalLineColor = MyHudConstants.MISSION_MARKER_COLOR.ToVector4(), DirectionalTexture = MyHudTexturesEnum.DirectionIndicator_white };
                data.NavigationMarks.Add(missionMark);
            }
        }

        public override bool IsMissionEntity(MyEntity target)
        {
            foreach (var objective in ActiveObjectives)
            {
                if (objective.IsMissionEntity(target))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsMissionEntityNotification(MyEntity entity, MySmallShipInteractionActionEnum action)
        {
            foreach (var objective in ActiveObjectives)
            {
                if (objective.IsMissionEntityNotification(entity, action))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
