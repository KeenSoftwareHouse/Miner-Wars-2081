#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Explosions;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Missions.Submissions;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Localization;
using MinerWars.Resources;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Networking;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyRiftMission : MyMission
    {
        #region Enums

        private enum EntityID
        {
            StartLocation = 3934,
            Detector_ShopLocation = 24107,
            Objective_RiftEntraceLocation = 24109,
            Objective_Uranite = 42060,
            Spawnpoint_Pirates = 54418,
            Spawnpoint_Pirates2 = 16780919,
            Detector_Pirates = 54416,

            Deterctor_shakes = 60632,

            Objective_Vendor = 24276,

            Detector_Dialogue_Base = 16779420,
            Detector_Dialogue_Mining = 16779422,
        }

        public override void ValidateIds()
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (var value in m_eruptionDetectors)
            {
                MyScriptWrapper.GetEntity(value);
            }
            foreach (var value in m_meteorDetectors)
            {
                MyScriptWrapper.GetEntity(value);
            }
        }

        private List<uint> m_eruptionDetectors = new List<uint>
        {
            60634, 60781, 60783, 60814, 60816, 60818, 60820, 60822, 60981, 60987, 60989, 60991, 60993, 60995, 60997, 60999, 61001, 61003, 61005, 61007, 61009, 61011, 61013, 61015, 61017,
            16777422, 16777424, 16777426, 16777428, 16777434, 16777436, 16777438, 16777458, 16777460, 16777462, 16777464, 16777466, 16777468, 16777470, 16777472, 16777474, 16777476, 16777478,
            16777480, 16777482, 16777484, 16777486, 16777488, 16777490, 16777492, 16777494, 16777496, 16777498, 16777500, 16777502, 16777504, 16777506, 16777508, 16777510, 16777512
        };

        private List<uint> m_meteorDetectors = new List<uint>
        {
            16777754, 16777756, 16777758, 16777760, 16777762, 16777764, 16777766, 16777768, 16777770, 16777772, 16777774, 16777800, 16777802, 16777804, 16777806, 16777808, 16777810, 16777812, 16777814, 16777816
        };

        private MyTimerActionDelegate m_subShakeAction;
        private MyTimerActionDelegate m_farExplosionAction;
        private bool m_riftShake;
        private int m_miningquotes;

        readonly float URANITE_TO_OBTAIN = 2.0f;

        #endregion

        public MyRiftMission()
        {
            m_subShakeAction = new MyTimerActionDelegate(SubShake);
            m_farExplosionAction = new MyTimerActionDelegate(FarExplosion);
            
            ID = MyMissionID.RIFT; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("14-Rift");
            Name = Localization.MyTextsWrapperEnum.RIFT;
            Description = Localization.MyTextsWrapperEnum.RIFT_Description;
            Flags = MyMissionFlags.Story;
            AchievementName = MySteamAchievementNames.Mission21_Rift;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-56700, 0, 4276);

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.JUNKYARD_EAC_AMBUSH };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.RIFT_GOTO_30 };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>();

            MySpawnpointSmartWaves spawnPointSmartWaves = new MySpawnpointSmartWaves(null, null, 2);

            var intro = new MyObjectiveDialog(
                MyMissionID.RIFT_INTRO,
                null,
                this,
                new MyMissionID[] { },
                MyDialogueEnum.RIFT_0050_INTRO
            )
            {
                SaveOnSuccess = true,
            };
            m_objectives.Add(intro);


            //Cannot see dialogues over inv.screen
            /*
            var getSupplies = new MyObjectiveEnterInventroy(
                new StringBuilder("Get supplies for the journey to the Rift"),
                MyMissionID.RIFT_GOTO_GETSUPPLIES1,
                new StringBuilder("Buy whatever useful."),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIFT_INTRO },
                new List<uint>() {  (uint)EntityID.Objective_Vendor }
            )
            {
                SaveOnSuccess = true,
            };*/
            var getSupplies = new MyObjective(
                (MyTextsWrapperEnum.RIFT_GOTO_GETSUPPLIES1_Name),
                MyMissionID.RIFT_GOTO_GETSUPPLIES1,
                (MyTextsWrapperEnum.RIFT_GOTO_GETSUPPLIES1_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIFT_INTRO },
                new MyMissionLocation(baseSector, (uint)EntityID.Objective_Vendor),
                radiusOverride: 30
            )
            {
                SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudSupplies
            };
            getSupplies.OnMissionSuccess += GetSuppliesSubmissionSuccess;
            getSupplies.OnMissionLoaded += GetSuppliesSubmissionLoaded;
            m_objectives.Add(getSupplies);

            var reachTheRiftSubmission = new MyObjective(
                (MyTextsWrapperEnum.RIFT_GOTO_10_Name),
                MyMissionID.RIFT_GOTO_10,
                (MyTextsWrapperEnum.RIFT_GOTO_10_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIFT_GOTO_GETSUPPLIES1 },
                new MyMissionLocation(baseSector, (uint)EntityID.Objective_RiftEntraceLocation)
            )
            {
                SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudRift
            };
            reachTheRiftSubmission.OnMissionLoaded += ReachTheRiftSubmissionLoaded;
            reachTheRiftSubmission.OnMissionSuccess += ReachTheRiftSubmissionSuccess;
            m_objectives.Add(reachTheRiftSubmission);

            var getOreSubmission = new MyHarvestOreSubmission(
                (MyTextsWrapperEnum.RIFT_URANITE_Name),
                MyMissionID.RIFT_URANITE,
                (MyTextsWrapperEnum.RIFT_URANITE_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIFT_GOTO_10 },
                new MyMissionLocation(baseSector, (uint)EntityID.Objective_Uranite),
                MyMwcObjectBuilder_Ore_TypesEnum.URANITE,
                URANITE_TO_OBTAIN,
                successDialogId: MyDialogueEnum.RIFT_1000_MINING_DONE
            )
            {
                SaveOnSuccess = true
            };
            getOreSubmission.Components.Add(spawnPointSmartWaves);
            getOreSubmission.OnMissionLoaded += GetOreSubmissionLoaded;
            getOreSubmission.OnMissionSuccess += GetOreSubmissionSuccess;
            m_objectives.Add(getOreSubmission);

            var returnToMothershipSubmission = new MyObjective(
                (MyTextsWrapperEnum.RIFT_GOTO_30_Name),
                MyMissionID.RIFT_GOTO_30,
                (MyTextsWrapperEnum.RIFT_GOTO_30_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.RIFT_URANITE },
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR),
                radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
            )
            {
                SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudMadelynsSapho
            };
            returnToMothershipSubmission.OnMissionLoaded += ReturnSubmissionLoaded;
            returnToMothershipSubmission.OnMissionSuccess += ReturnSubmissionSuccess;
            m_objectives.Add(returnToMothershipSubmission);

            //m_subShakeAction = SubShake;
            //m_farExplosionAction = FarExplosion;
        }

        public override void Load()
        {
            base.Load();

            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(40000, 50000), SolarwindAction, false);
            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(4000, 5000), MeteorAction, false);

            m_riftShake = false;
            MyEntityDetector shakeDetector = MyScriptWrapper.GetDetector((uint)EntityID.Deterctor_shakes);
            shakeDetector.OnEntityEnter += RiftReached;
            shakeDetector.OnEntityLeave += RiftLeft;
            shakeDetector.On();

            foreach (var detector in m_eruptionDetectors)
            {
                MyEntityDetector eruptionDetector = MyScriptWrapper.GetDetector(detector);
                eruptionDetector.OnEntityEnter += Erupt;
                eruptionDetector.On();
            }
            /*
            foreach (var detector in m_meteorDetectors)
            {
                MyEntityDetector MeteorDetector = MyScriptWrapper.GetDetector(detector);
                MeteorDetector.OnEntityEnter += Meteorite;
                MeteorDetector.On();
            }
            */

            MySmallShipBot ravenGirl = (MySmallShipBot)MyScriptWrapper.GetEntity("RavenGirl");
            MySmallShipBot ravenGuy = (MySmallShipBot)MyScriptWrapper.GetEntity("RavenGuy");

            MyScriptWrapper.StopFollow(ravenGirl);
            MyScriptWrapper.StopFollow(ravenGuy);

            ravenGirl.LookTarget = MySession.PlayerShip;
            ravenGuy.LookTarget = MySession.PlayerShip;

            MyAudio.MusicAllowed = false; //to forbid action music caused by bot fight
            MissionTimer.RegisterTimerAction(1000, PlayActionMusic, false);
        }

        private void PlayActionMusic()
        {
            MyAudio.MusicAllowed = true;
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere, 100, "KA03");
        }

        private void SolarwindAction()
        {
            MyScriptWrapper.EnableGlobalEvent(World.Global.MyGlobalEventEnum.SunWind, true);
            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(5000, 40000), SolarwindAction, false);
        }

        private void MeteorAction()
        {
            MyScriptWrapper.EnableGlobalEvent(World.Global.MyGlobalEventEnum.MeteorWind, true);
            MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(2000, 4000), MeteorAction, false);

            //MyScriptWrapper.GenerateMeteor(100, MySession.PlayerShip.GetPosition() + MySession.PlayerShip.WorldMatrix.Forward * 500, MyMwcVoxelMaterialsEnum.Lava_01, MySession.PlayerShip.WorldMatrix.Forward * -1000, MyParticleEffectsIDEnum.MeteorTrail_FireAndSmoke);
        }


        private void GetSuppliesSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.RIFT_0100_INTRO2);

            MyEntityDetector DialogueDetector_Base = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Dialogue_Base);
            DialogueDetector_Base.OnEntityEnter += DialogueBase;
            DialogueDetector_Base.On();
        }

        private void GetSuppliesSubmissionSuccess(MyMissionBase sender)
        {
            
        }

        private void ReachTheRiftSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.RIFT_0400_SHOPPINGDONE);
            MissionTimer.RegisterTimerAction(20000, PlayRiftMusic, false);
        }

        private void PlayRiftMusic()
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Special, 100, "KA04loop");
        }


        private void ReachTheRiftSubmissionSuccess(MyMissionBase sender)
        {
            
        }

        private void GetOreSubmissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.RIFT_0500_ENTERINGRIFT);

            MyEntityDetector DialogueDetector_Mining = MyScriptWrapper.GetDetector((uint)EntityID.Detector_Dialogue_Mining);
            DialogueDetector_Mining.OnEntityEnter += DialogueMining;
            DialogueDetector_Mining.On();
            m_miningquotes = 1;
        }

        private void GetOreSubmissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.EntityInventoryItemAmountChanged -= OreAmountChanged;

            // remove the mined plutonium to prevend stupid players from breaking the game
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.PlutoniumStored, MyGuiManager.GetFontMinerWarsBlue(), 5000, new object[] { URANITE_TO_OBTAIN }));
            MyScriptWrapper.RemoveInventoryItemAmount(MyScriptWrapper.GetPlayerInventory(), MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.URANITE, URANITE_TO_OBTAIN);
        }

        private void ReturnSubmissionLoaded(MyMissionBase sender)
        {
        }

        private void ReturnSubmissionSuccess(MyMissionBase sender)
        {
        }

        private void RiftReached(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                //MyAudio.ApplyTransition(MyMusicTransitionEnum.Mystery);

                // start shakes
                m_riftShake = true;
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(4000, 12000), m_farExplosionAction, false);
            }
        }

        private void RiftLeft(MyEntityDetector sender, MyEntity entity)
        {
            if (entity == MySession.PlayerShip)
            {
                // stop shakes
                m_riftShake = false;

                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.RIFT_1100_LEAVING);
            }
        }

        private void DialogueBase(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.RIFT_0200_STATION);
                MissionTimer.RegisterTimerAction(24000, DialogueBaseCont, false);
                sender.Off();
            }
        }

        private void DialogueBaseCont()
        {
            MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.RIFT_0300_TOURISTS);
        }

        private void DialogueMining(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.PlayDialogue(Audio.Dialogues.MyDialogueEnum.RIFT_0600_MINING);
                MyScriptWrapper.EntityInventoryItemAmountChanged += OreAmountChanged;
                sender.Off();
            }
        }

        void OreAmountChanged(MyEntity entity, MyInventory inventory, MyInventoryItem item, float number)
        {
            if (MyScriptWrapper.IsPlayerShip(entity))
            {
                if (item.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.Ore)
                {
                    if (!MyScriptWrapper.IsMissionFinished(MyMissionID.RIFT_URANITE))
                    {
                        if (item.ObjectBuilderId == (int)MyMwcObjectBuilder_Ore_TypesEnum.URANITE)
                        {
                            float ammount = MyScriptWrapper.GetInventoryItemAmount(MyScriptWrapper.GetPlayerInventory(), MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.URANITE);
                            if (ammount > 0.5f && m_miningquotes == 1)
                            {
                                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RIFT_0700_MINING_COLOR);
                                m_miningquotes++;
                            }
                            else if (ammount > 1.0f && m_miningquotes == 2)
                            {
                                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RIFT_0800_MINING_TUNE);
                                m_miningquotes++;
                            }
                            else if (ammount > 1.5f && m_miningquotes == 3)
                            {
                                MyScriptWrapper.PlayDialogue(MyDialogueEnum.RIFT_0900_MINING_TUNE_2);
                                m_miningquotes++;
                            }
                        }
                    }
                }
            }
        }

        private void Erupt(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.AddExplosion(sender, MyExplosionTypeEnum.BOMB_EXPLOSION, MyMwcUtils.GetRandomFloat(40f, 60f), MyMwcUtils.GetRandomFloat(25f, 40f), true);
                MyScriptWrapper.IncreaseHeadShake(MyMwcUtils.GetRandomFloat(5f, 15f));
                sender.Off();
            }
        }
        /*
        private void Meteorite(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.GenerateMeteor(100, MySession.PlayerShip.GetPosition() + MySession.PlayerShip.WorldMatrix.Forward * 500, MyMwcVoxelMaterialsEnum.Lava_01, MySession.PlayerShip.WorldMatrix.Forward * -1000, MyParticleEffectsIDEnum.MeteorTrail_FireAndSmoke);
                sender.Off();
            }
        }
        */
        private void SubShake()
        {
            MyScriptWrapper.IncreaseHeadShake(MyMwcUtils.GetRandomInt(3, 4));
        }

        private void FarExplosion()
        {
            if (m_riftShake)
            {
                // MainShake
                MyScriptWrapper.IncreaseHeadShake(MyMwcUtils.GetRandomInt(7, 10));
                MyScriptWrapper.AddAudioImpShipQuake();

                // Register sub shakes
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(200, 400), m_subShakeAction, false);
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(600, 800), m_subShakeAction, false);
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(800, 1200), m_subShakeAction, false);
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(1200, 1400), m_subShakeAction, false);

                // Register next far explosion
                MissionTimer.RegisterTimerAction(MyMwcUtils.GetRandomInt(4000, 12000), m_farExplosionAction, false);
            }
        }

        public override void Update()
        {
            base.Update();

            if (MyMwcUtils.GetRandomFloat(0, 1) > 0.97f)
            {
                Vector3 position = MySession.PlayerShip.GetPosition() + MyMwcUtils.GetRandomFloat(0, 10) * MySession.PlayerShip.WorldMatrix.Forward * 500 + Vector3.Up * 2000 + MyMwcUtils.GetRandomFloat(0, 1) * MySession.PlayerShip.WorldMatrix.Right * 4000 - MyMwcUtils.GetRandomFloat(0, 1) * MySession.PlayerShip.WorldMatrix.Right * 4000;
                MyScriptWrapper.GenerateMeteor(MyMwcUtils.GetRandomFloat(0.0001f, 2) * 100, position, MyMwcVoxelMaterialsEnum.Lava_01, -800 * Vector3.Up * MyMwcUtils.GetRandomFloat(0.7f, 1.0f), MyParticleEffectsIDEnum.MeteorTrail_FireAndSmoke);
            }
        }

        public override void Unload()
        {
            base.Unload();
            MyAudio.MusicAllowed = true;
        }
    }
}
