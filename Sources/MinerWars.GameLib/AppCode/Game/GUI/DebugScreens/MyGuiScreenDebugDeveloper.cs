using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;
using System.Linq;

namespace MinerWars.AppCode.Game.GUI
{
    using System.Diagnostics;
    using MinerWars.AppCode.Game.GUI.DebugScreens;
    using MinerWars.AppCode.Game.TransparentGeometry.Particles;
    using MinerWars.AppCode.Game.Entities;
    using MinerWars.CommonLIB.AppCode.Networking;
    using MinerWars.AppCode.Game.World;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.SolarSystem;
    using MinerWars.AppCode.Game.Missions;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.App;
    using MinerWars.AppCode.Game.Journal;
    using MinerWars.AppCode.Physics;
    using MinerWars.AppCode.Game.World.Global;

    class MyGuiScreenDebugDeveloper : MyGuiScreenDebugBase
    {
        //Render
        static MyGuiScreenDebugRender s_debugRenderScreen;
        static MyGuiScreenDebugRenderLights s_debugRenderLightsScreen;
        static MyGuiScreenDebugRenderOptimizations s_debugRenderOptsScreen;
        static MyGuiScreenDebugRenderPruning s_debugRenderPruningScreen;
        static MyGuiScreenDebugRenderModelFX s_debugRenderModelFXScreen;
        static MyGuiScreenDebugRenderGlobalFX s_debugRenderGlobalFXScreen;
        static MyGuiScreenDebugRenderSectorFX s_debugRenderSectorFXScreen;
        static MyGuiScreenDebugRenderHDR s_debugRenderHDRScreen;
        static MyGuiScreenDebugColors s_debugColorsScreen;
        static MyGuiScreenDebugVolumetricSSAO s_debugVolumetricSSAOScreen;
        //static MyGuiScreenRenderModules s_renderModulesScreen = new MyGuiScreenRenderModules();
        static MyGuiScreenRenderModule s_renderModulePrepareForDraw;
        static MyGuiScreenRenderModule s_renderModuleBackground;
        static MyGuiScreenRenderModule s_renderModuleAlphaBlendPreHDR;
        static MyGuiScreenRenderModule s_renderModuleDebugDraw;

        //Game
        static MyGuiScreenDebugShip s_debugShipScreen;
        static MyGuiScreenDebugGame s_debugGameScreen;
        static MyGuiScreenDebugDebris s_debugDebrisScreen;
        static MyGuiScreenDebugSystem s_debugSystemScreen;
        static MyGuiScreenDebugPlayerShake s_debugPlayerShakeScreen;
        static MyGuiScreenDebugPlayerCameraSpring s_debugPlayerCameraSpringScreen;
        static MyGuiScreenDebugSmallShipWeaponsOffset s_debugSmallShipWeaponsOffsetScreen;
        static MyGuiScreenDebugFillSector s_debugFillSectorScreen;
        static MyGuiScreenDebugBot s_debugBotScreen;
        static MyGuiScreenDebugGlobalEvents s_debugGameGlobalEvents;

        static List<MyGuiControlCheckbox> s_groupList = new List<MyGuiControlCheckbox>();

        private MyMissionID m_missionIdToStart;
        private MySolarSystemAreaEnum m_areaToTravel;


        class MyDevelopGroup
        {
            public MyDevelopGroup(string name)
            {
                Name = name;
                Item2 = new List<MyGuiControlBase>();
            }
            public string Name;
            public MyGuiControlBase Item1;
            public List<MyGuiControlBase> Item2;
        };


        //Main groups
        static MyDevelopGroup s_debugDrawGroup = new MyDevelopGroup("Debug draw");
        static MyDevelopGroup s_performanceGroup = new MyDevelopGroup("Performance");
        static List<MyDevelopGroup> s_mainGroups = new List<MyDevelopGroup>()
        {
            s_debugDrawGroup,
            s_performanceGroup,
        };
        static MyDevelopGroup s_activeMainGroup = s_debugDrawGroup;


        //Develop groups
        static MyDevelopGroup s_renderGroup = new MyDevelopGroup("Render");
        static MyDevelopGroup s_gameGroup = new MyDevelopGroup("Game");
        //static MyDevelopGroup s_testsGroup = new MyDevelopGroup("Tests");
        static MyDevelopGroup s_travelGroup = new MyDevelopGroup("Travel");
        static MyDevelopGroup s_missionsGroup = new MyDevelopGroup("Missions");
        static List<MyDevelopGroup> s_developerGroups = new List<MyDevelopGroup>()
        {
            s_renderGroup,
            s_gameGroup,
            s_travelGroup,
            s_missionsGroup,
        };
        static MyDevelopGroup s_activeGroup = s_missionsGroup;

        static MyGuiScreenDebugDeveloper()
        {
            if (!MyFakes.SIMPLE_DEBUG_SCREEN)
            {
                s_debugRenderScreen = new MyGuiScreenDebugRender();
                s_debugRenderLightsScreen = new MyGuiScreenDebugRenderLights();
                s_debugRenderOptsScreen = new MyGuiScreenDebugRenderOptimizations();
                s_debugRenderPruningScreen = new MyGuiScreenDebugRenderPruning();
                s_debugRenderModelFXScreen = new MyGuiScreenDebugRenderModelFX();
                s_debugRenderGlobalFXScreen = new MyGuiScreenDebugRenderGlobalFX();
                s_debugRenderSectorFXScreen = new MyGuiScreenDebugRenderSectorFX();
                s_debugRenderHDRScreen = new MyGuiScreenDebugRenderHDR();
                s_debugColorsScreen = new MyGuiScreenDebugColors();
                s_debugVolumetricSSAOScreen = new MyGuiScreenDebugVolumetricSSAO();
                //static MyGuiScreenRenderModules s_renderModulesScreen = new MyGuiScreenRenderModules();
                s_renderModulePrepareForDraw = new MyGuiScreenRenderModule(MyRenderStage.PrepareForDraw, "Prepare for draw");
                s_renderModuleBackground = new MyGuiScreenRenderModule(MyRenderStage.Background, "Background");
                s_renderModuleAlphaBlendPreHDR = new MyGuiScreenRenderModule(MyRenderStage.AlphaBlendPreHDR, "Pre-HDR Aplha blend");
                s_renderModuleDebugDraw = new MyGuiScreenRenderModule(MyRenderStage.DebugDraw, "Debug draw");

                //Game
                s_debugShipScreen = new MyGuiScreenDebugShip();
                s_debugGameScreen = new MyGuiScreenDebugGame();
                s_debugDebrisScreen = new MyGuiScreenDebugDebris();
                s_debugSystemScreen = new MyGuiScreenDebugSystem();
                s_debugPlayerShakeScreen = new MyGuiScreenDebugPlayerShake();
                s_debugPlayerCameraSpringScreen = new MyGuiScreenDebugPlayerCameraSpring();
                s_debugSmallShipWeaponsOffsetScreen = new MyGuiScreenDebugSmallShipWeaponsOffset();
                s_debugFillSectorScreen = new MyGuiScreenDebugFillSector();
                s_debugBotScreen = new MyGuiScreenDebugBot();
                s_debugGameGlobalEvents = new MyGuiScreenDebugGlobalEvents();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyGuiScreenDebugDeveloper"/> class.
        /// </summary>
        public MyGuiScreenDebugDeveloper()
            : base(new Vector2(.5f, .5f), new Vector2(0.35f, 0.9f), 0.35f * Color.Yellow.ToVector4(), true)
        {
            if (MyFakes.SIMPLE_DEBUG_SCREEN)
            {
                m_currentPosition.Y = -0.25f;
                Controls.Clear();
                CreateControlsSimple();
            }
            else
            {
                CreateControls();
            }

            // This disable drawing of the background image as well:
            m_backgroundColor = null;

            m_enableBackgroundFade = true;
            m_backgroundFadeColor = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);
        }

        private void CreateControlsSimple()
        {
            float groupStartPosition = m_currentPosition.Y;
            //CreateMissionControls(null, 0, 1000);

            m_currentPosition.Y = groupStartPosition;
            m_buttonXOffset = -0.4f;
            CreateMissionControls(s_missionsGroup.Item2, 0, 15);

            m_currentPosition.Y = groupStartPosition;
            m_buttonXOffset = -0.2f;
            CreateMissionControls(s_missionsGroup.Item2, 15, 30);

            m_currentPosition.Y = groupStartPosition;
            m_buttonXOffset = 0;
            CreateMissionControls(s_missionsGroup.Item2, 30, 45);

            m_currentPosition.Y = groupStartPosition;
            m_buttonXOffset = 0.2f;
            CreateMissionControls(s_missionsGroup.Item2, 45, 60);

            m_currentPosition.Y = groupStartPosition;
            m_buttonXOffset = 0.4f;
            CreateMissionControls(s_missionsGroup.Item2, 60, 75);
        }

        private void CreateControls()
        {
            Controls.Clear();
            foreach (MyDevelopGroup developerGroup in s_developerGroups)
            {
                if (developerGroup.Item2.Count > 0)
                {
                    EnableGroup(developerGroup.Item2, false);
                    developerGroup.Item2.Clear();
                }
            }
            foreach (MyDevelopGroup mainGroup in s_mainGroups)
            {
                if (mainGroup.Item2.Count > 0)
                {
                    EnableGroup(mainGroup.Item2, false);
                    mainGroup.Item2.Clear();
                }
            }

            AddCaption(MyTextsWrapperEnum.DeveloperCaption, Color.Yellow.ToVector4());

            m_scale = 0.9f;
            m_closeOnEsc = true;
            m_checkBoxOffset = 0.015f;

            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.03f, /*0.15f*//*0.05f*/0.15f);


            float buttonOffset = 0;
            foreach (MyDevelopGroup mainGroup in s_mainGroups)
            {
                mainGroup.Item1 = new MyGuiControlButton(this, new Vector2(-0.03f + m_currentPosition.X + buttonOffset, m_currentPosition.Y), new Vector2(0.09f, 0.03f), new Vector4(1, 1, 0.5f, 1), new StringBuilder(mainGroup.Name), null, Color.Yellow.ToVector4(), MyGuiConstants.DEBUG_BUTTON_TEXT_SCALE * MyGuiConstants.DEBUG_LABEL_TEXT_SCALE * m_scale * 1.2f, OnClickMainGroup, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true);
                buttonOffset += 0.09f;
                Controls.Add(mainGroup.Item1);
            }

            m_currentPosition.Y += 0.04f * m_scale;

            float mainStartPosition = m_currentPosition.Y;

            CreateDebugDrawControls();

            m_currentPosition.Y = mainStartPosition;
            CreatePerformanceControls();

            foreach (MyDevelopGroup mainGroup in s_mainGroups)
            {
                EnableGroup(mainGroup.Item2, false);
            }
            EnableGroup(s_activeMainGroup.Item2, true);


            //Screens
            AddLabel(new StringBuilder("Debug screens"), Color.Yellow.ToVector4(), 1.2f);

            buttonOffset = 0;
            foreach (MyDevelopGroup developerGroup in s_developerGroups)
            {
                developerGroup.Item1 = new MyGuiControlButton(this, new Vector2(-0.03f + m_currentPosition.X + buttonOffset, m_currentPosition.Y), new Vector2(0.09f, 0.03f), new Vector4(1,1,0.5f,1), new StringBuilder(developerGroup.Name), null, Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * MyGuiConstants.DEBUG_BUTTON_TEXT_SCALE * m_scale * 1.2f, OnClickGroup, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true);
                buttonOffset += 0.09f;
                Controls.Add(developerGroup.Item1);
            }

            m_currentPosition.Y += 0.04f * m_scale;

            float groupStartPosition = m_currentPosition.Y;

            AddGroupBox(new StringBuilder("Overall settings"), s_debugRenderScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Lights settings"), s_debugRenderLightsScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Color settings"), s_debugColorsScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Optimizations"), s_debugRenderOptsScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Pruning and culling"), s_debugRenderPruningScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Model FX"), s_debugRenderModelFXScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Global FX"), s_debugRenderGlobalFXScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Sector FX"), s_debugRenderSectorFXScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("HDR"), s_debugRenderHDRScreen, s_renderGroup.Item2);
            //AddGroupBox(new StringBuilder("Render modules"), s_renderModulesScreen, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Render module - Prepare for draw"), s_renderModulePrepareForDraw, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Render module - Background"), s_renderModuleBackground, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Render module - Pre-HDR Alpha blend"), s_renderModuleAlphaBlendPreHDR, s_renderGroup.Item2);
            AddGroupBox(new StringBuilder("Render module - Debug draw"), s_renderModuleDebugDraw, s_renderGroup.Item2);
            MyPostProcessBase volumetricSsao2PP = MyRender.GetPostProcess(MyPostProcessEnum.VolumetricSSAO2);
            if (volumetricSsao2PP != null)
                AddGroupBox(new StringBuilder("SSAO"), s_debugVolumetricSSAOScreen, s_renderGroup.Item2);

            m_currentPosition.Y = groupStartPosition;

            AddGroupBox(new StringBuilder("Game"), s_debugGameScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("System"), s_debugSystemScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("Player Ship"), s_debugShipScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("Player Shake"), s_debugPlayerShakeScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("Player Camera Spring"), s_debugPlayerCameraSpringScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("Small Ship Weapons"), s_debugSmallShipWeaponsOffsetScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("Debris"), s_debugDebrisScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("Filling sector"), s_debugFillSectorScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("Bots"), s_debugBotScreen, s_gameGroup.Item2);
            AddGroupBox(new StringBuilder("Global Events"), s_debugGameGlobalEvents, s_gameGroup.Item2);
            AddValidateAllMissionsButton();

            m_currentPosition.Y = groupStartPosition;
            //AddButton(new StringBuilder("Particle stress"), OnParticleStressTest, s_testsGroup.Item2);
            m_buttonXOffset = -0.2f;
            CreateTravelControls(s_travelGroup.Item2, 0, 15);

            m_currentPosition.Y = groupStartPosition;
            m_buttonXOffset = 0.0f;
            CreateTravelControls(s_travelGroup.Item2, 15, 30);

            m_currentPosition.Y = groupStartPosition;
            m_buttonXOffset = 0.2f;
            CreateTravelControls(s_travelGroup.Item2, 30, 45);

            /*
            AddButton(new StringBuilder("Mercury"), OnTravelToMercury, s_travelGroup.Item2);
            AddButton(new StringBuilder("Venus"), OnTravelToVenus, s_travelGroup.Item2);
            AddButton(new StringBuilder("Earth"), OnTravelToEarth, s_travelGroup.Item2);
            AddButton(new StringBuilder("Mars"), OnTravelToMars, s_travelGroup.Item2);
            AddButton(new StringBuilder("Jupiter"), OnTravelToJupiter, s_travelGroup.Item2);
            AddButton(new StringBuilder("Saturn"), OnTravelToSaturn, s_travelGroup.Item2);
            AddButton(new StringBuilder("Uran"), OnTravelToUran, s_travelGroup.Item2);
            AddButton(new StringBuilder("Neptun"), OnTravelToNeptun, s_travelGroup.Item2);
              */
            m_currentPosition.Y = groupStartPosition;

            CreateControlsSimple();
            
            foreach (MyDevelopGroup developerGroup in s_developerGroups)
            {
                EnableGroup(developerGroup.Item2, false);
            }
            EnableGroup(s_activeGroup.Item2, true);
            this.SetControlIndex(Controls.IndexOf(s_activeGroup.Item1));
        }

        private void AddValidateAllMissionsButton()
        {
            AddButton(new StringBuilder("Validate all missions"), new MyGuiControlButton.OnButtonClick((bnt) => ValidateAllMissionsClick()), s_gameGroup.Item2);
        }

        void CreateMissionControls(List<MyGuiControlBase> group, int minIndex, int maxIndex)
        {
            int i = 0;
            foreach (var mission in MyMissions.Missions.Values.OfType<MyMission>().OrderBy(x => x.DebugName.ToString()))
            {
                if ((i >= minIndex) && (i < maxIndex))
                {
                    var missionId = mission.ID; // local copy for lambda
                    MyGuiControlButton.OnButtonClick handler = new MyGuiControlButton.OnButtonClick((bnt) => ButtonClickAction(missionId));

                    AddButton(mission.DebugName, handler, group, MyGuiControlButtonTextAlignment.LEFT);
                }
                i++;
            }
        }


        void CreateTravelControls(List<MyGuiControlBase> group, int minIndex, int maxIndex)
        {
            int i = 0;

            foreach (var solarSystemAreaPair in MySolarSystemConstants.Areas.OrderBy(x => x.Value.Name))
            {
                if ((i >= minIndex) && (i < maxIndex))
                {
                    var sectorID = solarSystemAreaPair.Key;
                    MyGuiControlButton.OnButtonClick handler = new MyGuiControlButton.OnButtonClick((bnt) => ButtonClickAction(sectorID));
                    AddButton(new StringBuilder(solarSystemAreaPair.Value.Name), handler, group);
                }
                i++;
            }
        }


        private void ButtonClickAction(MySolarSystemAreaEnum solarSystemArea)
        {
            m_areaToTravel= solarSystemArea;
            if (MyClientServer.LoggedPlayer != null)
            {
                StartTravelCallback();
            }
            else
            {
                MyGuiManager.RemoveScreen(this, true);
                MyGuiScreenMainMenu.AddLoginScreenDrmFree(new Action(StartTravelCallback));
            }
        }

        private void ButtonClickAction(MyMissionID missionId)
        {
            if (MinerWars.AppCode.Game.Sessions.MyMultiplayerGameplay.IsRunning)
                MinerWars.AppCode.Game.Sessions.MyMultiplayerGameplay.Static.Shutdown();

            m_missionIdToStart = missionId;
            if (MyClientServer.LoggedPlayer != null)
            {
                StartMissionCallback();
            }
            else
            {
                MyGuiManager.RemoveScreen(this, true);                
                MyGuiScreenMainMenu.AddLoginScreenDrmFree(new Action(StartMissionCallback));
            }
        }

        public void StartTravelCallback()
        {
            OnTravelToAnywhere(MySolarSystemUtils.KmToSectors(MySolarSystemConstants.Areas[m_areaToTravel].GetCenter()));
        }

        public void StartMissionCallback()
        {
            MinerWars.AppCode.Game.GUI.MyGuiScreenMainMenu.StartMission(m_missionIdToStart);
            this.CloseScreen();
        }

        private void ValidateAllMissionsClick()
        {
            MyGuiManager.RemoveScreen(this, true);
            MyGuiScreenMainMenu.AddLoginScreenDrmFree(new Action(ValidateAllMissionsCallback));
        }

        public void ValidateAllMissionsCallback()
        {
            ValidateAllMissionsStartingFrom(0);
        }

        void ValidateAllMissionsStartingFrom(int index)
        {
            var allMissions = MyMissions.Missions.Values.OfType<MyMission>().ToList();
            if (index >= allMissions.Count) return;

            var mission = allMissions[index];
            MyMissionID missionId = mission.ID;
            // each mission is automatically validated after start
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

            var startSessionScreen = new MyGuiScreenStartSessionProgress(MyMwcStartSessionRequestTypeEnum.NEW_STORY,
                MyTextsWrapperEnum.StartGameInProgressPleaseWait, null, MyGameplayDifficultyEnum.EASY, null, null);

            startSessionScreen.OnSuccessEnter = new Action<MyGuiScreenGamePlayType, MyMwcStartSessionRequestTypeEnum, MyMwcObjectBuilder_Checkpoint>((screenType, sessionType, checkpoint) =>
            {
                Action<MyMwcObjectBuilder_Sector, Vector3> enterSuccessAction = new Action<MyMwcObjectBuilder_Sector, Vector3>((sector, newPosition) =>
                {
                    MyMwcVector3Int sectorPosition;
                    sectorPosition = sector.Position;

                    MyMwcSectorIdentifier newSectorIdentifier = new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.STORY, MyClientServer.LoggedPlayer.GetUserId(), sectorPosition, null);
                    var newScreen = new MyGuiScreenGamePlay(MyGuiScreenGamePlayType.GAME_STORY, null, newSectorIdentifier, sector.Version, MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT);
                    var loadScreen = new MyGuiScreenLoading(newScreen, MyGuiScreenGamePlay.Static);
                    newScreen.OnGameReady += new ScreenHandler((screen) =>
                    {
                        ((MyMission)MyMissions.GetMissionByID(missionId)).Accept();
                        ValidateAllMissionsStartingFrom(index + 1);  // validate the next mission
                    });

                    loadScreen.AnnounceLeaveToServer = true;
                    loadScreen.LeaveSectorReason = MyMwcLeaveSectorReasonEnum.TRAVEL;

                    // Current sector and sector object builder has changed
                    checkpoint.ActiveMissionID = -1; // Manually deactivate mission
                    checkpoint.PlayerObjectBuilder.ShipObjectBuilder.PositionAndOrientation.Position = newPosition;
                    checkpoint.EventLogObjectBuilder.Clear(); // Or just clear mission start/finish

                    // Make prereq missions completed
                    foreach (var prereq in mission.RequiredMissions)
                    {
                        var start = new MyEventLogEntry() { EventType = EventTypeEnum.MissionStarted, EventTypeID = (int)prereq }.GetObjectBuilder();
                        var end = new MyEventLogEntry() { EventType = EventTypeEnum.MissionFinished, EventTypeID = (int)prereq }.GetObjectBuilder();
                        checkpoint.EventLogObjectBuilder.Add(start);
                        checkpoint.EventLogObjectBuilder.Add(end);
                    }

                    checkpoint.SectorObjectBuilder = sector;
                    checkpoint.CurrentSector.Position = sector.Position;
                    loadScreen.AddEnterSectorResponse(checkpoint, missionId);

                    MyGuiManager.AddScreen(loadScreen);

                    if (MyMinerGame.IsPaused())
                        MyMinerGame.SwitchPause();
                });

                //  Load neighbouring sector 
                MyGuiManager.AddScreen(new MyGuiScreenEnterSectorProgress(MyMwcTravelTypeEnum.SOLAR, mission.Location.Sector, Vector3.Zero, enterSuccessAction));

                //var newGameplayScreen = new MyGuiScreenGamePlay(screenType, null, checkpoint.CurrentSector, sessionType);
                //var loadScreen = new MyGuiScreenLoading(newGameplayScreen, MyGuiScreenGamePlay.Static);

                //loadScreen.AddEnterSectorResponse(checkpoint);
                //MyGuiManager.AddScreen(loadScreen);
            });

            MyGuiManager.AddScreen(startSessionScreen);
        }

        //Because of edit and continue
        void CreateDebugDrawControls()
        {
            //Debug draw
            AddCheckBox(MyTextsWrapperEnum.DrawPhysicsPrimitives, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.DrawJLXCollisionPrimitives), true, s_debugDrawGroup.Item2);
            AddCheckBox(MyTextsWrapperEnum.DrawHelperPrimitives, null, MemberHelper.GetMember(() => MyMwcFinalBuildConstants.DrawHelperPrimitives), true, s_debugDrawGroup.Item2);
            AddCheckBox(new StringBuilder("Wireframe"), null, MemberHelper.GetMember(() => MyRender.Wireframe), true, s_debugDrawGroup.Item2);
            AddCheckBox(new StringBuilder("Prefab kinematic"), null, MemberHelper.GetMember(() => MinerWars.AppCode.Game.Entities.SubObjects.MyPrefabKinematic.DRAW_DEBUG_INFORMATION), true, s_debugDrawGroup.Item2);
            
            m_currentPosition.Y += 0.01f;
        }


        //Because of edit and continue
        void CreatePerformanceControls()
        {
            AddCheckBox(new StringBuilder("Bot Logic"), null, MemberHelper.GetMember(() => MySmallShipBot.BotLogic), true, s_performanceGroup.Item2);
            AddCheckBox(new StringBuilder("Physics Simulation"), null, MemberHelper.GetMember(() => MyPhysics.PhysicsSimulation), true, s_performanceGroup.Item2);
            AddCheckBox(new StringBuilder("Particles"), null, MemberHelper.GetMember(() => MyParticlesManager.Enabled), true, s_performanceGroup.Item2);
            AddCheckBox(new StringBuilder("Large Weapons"), null, MemberHelper.GetMember(() => MyLargeShipWeaponsConstants.Enabled), true, s_performanceGroup.Item2);

            m_currentPosition.Y += 0.01f;
        }

        protected void AddGroupBox(StringBuilder text, MyGuiScreenDebugBase screen, List<MyGuiControlBase> controlGroup = null)
        {
            MyGuiControlCheckbox checkBox = AddCheckBox(text, screen, controlGroup);
            s_groupList.Add(checkBox);

            checkBox.OnCheck += delegate(MyGuiControlCheckbox sender)
            {
                if (sender.Checked)
                {
                    foreach (MyGuiControlCheckbox chb in s_groupList)
                    {
                        if (chb != sender)
                            chb.Checked = false;
                    }
                }
            };
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugDeveloper";
        }


        void OnClickGroup(MyGuiControlButton sender)
        {
            EnableGroup(s_activeGroup.Item2, false);

            foreach (MyDevelopGroup developerGroup in s_developerGroups)
            {
                if (developerGroup.Item1 == sender)
                {
                    s_activeGroup = developerGroup;
                    break;
                }
            }

            EnableGroup(s_activeGroup.Item2, true);
        }


        void OnClickMainGroup(MyGuiControlButton sender)
        {
            EnableGroup(s_activeMainGroup.Item2, false);

            foreach (MyDevelopGroup mainGroup in s_mainGroups)
            {
                if (mainGroup.Item1 == sender)
                {
                    s_activeMainGroup = mainGroup;
                    break;
                }
            }

            EnableGroup(s_activeMainGroup.Item2, true);
        }

        void EnableGroup(List<MyGuiControlBase> group, bool enable)
        {
            if (!enable)
            {
                foreach (MyGuiControlBase control in group)
                {
                    Controls.Remove(control);
                }
            }
            else
            {
                foreach (MyGuiControlBase control in group)
                {
                    Controls.Add(control);
                }
            }
        }

        void OnParticleStressTest(MyGuiControlButton sender)
        {
            MyEntities.CloseAll(false);


            Vector3 position = MyCamera.Position + MyCamera.ForwardVector * 300;

            int explosionCount = 50;
            float particlesDistance = 60;

            Vector3 particlePosition = position + MyCamera.LeftVector * particlesDistance * (explosionCount / 2);

            for (int i = 0; i < explosionCount; i++)
            {
                MyParticleEffect effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_Blaster);
                effect.WorldMatrix = Matrix.CreateTranslation(particlePosition);

                effect.OnDelete += new EventHandler(effect_OnDelete);

                particlePosition += -MyCamera.LeftVector * particlesDistance;
            }
        }

        void OnTravelToAnywhere(MyMwcVector3Int sector)
        {
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
            m_sectorToTravel = sector;
            MyGuiScreenMainMenu.AddLoginScreenDrmFree(new Action(TravelToSectorCallback));            
        }

        private MyMwcVector3Int m_sectorToTravel;

        public void TravelToSectorCallback()
        {
            MyGuiManager.AddScreen(new MyGuiScreenStartSessionProgress(
                                                        MyMwcStartSessionRequestTypeEnum.SANDBOX_OWN,
                                                        MyTextsWrapperEnum.StartGameInProgressPleaseWait,
                                                        new MyMwcSectorIdentifier(
                                                            MyMwcSectorTypeEnum.STORY, MyClientServer.LoggedPlayer.GetUserId(),
                                                            m_sectorToTravel, null),
                                                            MyGameplayDifficultyEnum.EASY,
                                                            null,
                                                        null));
        }

                             

        void effect_OnDelete(object sender, EventArgs e)
        {     /*
            MyParticleEffect deletedEffect = (MyParticleEffect)sender;
            MyParticleEffect effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_Blaster);
            effect.WorldMatrix = Matrix.CreateTranslation(deletedEffect.WorldMatrix.Translation);
            effect.EnableLods = false;

            effect.OnDelete += new EventHandler(effect_OnDelete);
               * */
        }

    }
}
