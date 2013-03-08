#region Using

using System;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Collections.Generic;
using System.Linq;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Audio;
using System.Diagnostics;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.Resources;
using MinerWars.AppCode.Networking;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyWhiteWolvesResearchMission : MyMission
    {
        MyMwcVector3Int AcceptSector = new MyMwcVector3Int(-2325831, 0, -7186381);
        MyMwcVector3Int BaseSector = new MyMwcVector3Int(-4081250, 0, -6815625);
        private List<uint> m_particlesExplosion1 = new List<uint>() { 563933, 563935, 563934, 563932, 563927 };
        private List<uint> m_particlesExplosion2 = new List<uint>() { 563928, 563930, 563929, 563931, 563919, 563936 };
        private List<uint> m_particlesExplosion3 = new List<uint>() { 563903, 563937, 16777478, 16777479, 16777480, 16777481, 16777482, 16778169 };
 

        private enum AcceptSectorEntityIDs
        {
            AcceptLocationInReichstag = 18967,
        }

        private enum EntityID
        {
            TargetUse01 = 562905,
            Target02 = 562906,
            TargetUse03 = 562907,
            TargetUse04 = 562908,
            TargetUse05 = 562909,
            //06
            Target06 = 16777477,
            Door1 = 299653,
            Door2 = 299656,
            WayPoint1 = 299663,
            WayPoint2 = 299572,
            WayPoint3 = 299571,
            WayPoint4 = 299683,
            Target07 = 562911,
            Target08 = 54512,
            Target09 = 562912,

            BlinkLight1 = 354880,
            BlinkLight2 = 355701,
            BlinkLight3 = 355423,
            BlinkLight4 = 355420,
            BlinkLight5 = 356318,

            DetectorBeforeGetOut = 564178,
            DetectorBeforeEnteringWarehouseAtHigh = 564182,
            DetectorBeforeEnteringWarehouseAtLow = 564186,
            DetectorBeforeEnteringBlueprintLocation = 564189,
            DetectorBeforeEnteringMainConnectionRoom = 564191,
            DetectorBeforeEnteringBioBrainsCircularRoom = 564193,
            DetectorBeforeTakingBrainSample = 564195,
            DetectroBeforeEnteringLowerPartOfBiomachine = 564198,
            DetectorBeforeEnteringHigherPartOfBiomachine = 564201,

            SpawnpointAtGettingOut1 = 564180,
            SpawnpointAtGettingOut2 = 564181,
            SpawnpointAtGettingOut3 = 564243,
            SpawnpointAtGettingOut4 = 564242,
            SpawnpointAtWarehouse1 = 564184,
            SpawnpointAtWarehouse2 = 564185,
            SpawnpointUponWarehouse = 564188,
            SpawnpointAtBlueprintLocation = 361629,
            SpawnpointAtConnectionRoom1 = 361627,
            SpawnpointAtConnectionRoom2 = 564245,
            SpawnpointAtBrainsConnection = 361628,
            SpawnpointAtBrains = 564197,
            SpawnpointAtLowerPartOfBiomach = 564200,
            SpawnpointAtHigherPartOfBiomach1 = 564203,
            SpawnpointAtHigherPartOfBiomach2 = 564244,
            SpawnpointAtVeryStart1 = 354972,
            SpawnpointAtVeryStart2 = 564204,
            SpawnpointAtVeryStart3 = 564232,
            
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            if (!IsMainSector) return;
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }

            var list = new List<uint>();
            list.AddRange(m_particlesExplosion1);
            list.AddRange(m_particlesExplosion2);
            list.AddRange(m_particlesExplosion3);

            foreach (var value in list)
            {
                MyScriptWrapper.GetEntity(value);
            }


        }

        public MyWhiteWolvesResearchMission()
        {
            ID = MyMissionID.NAZI_BIO_LAB; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("18b-White Wolves bio research lab");
            Name = MyTextsWrapperEnum.NAZI_BIO_LAB;
            Description = MyTextsWrapperEnum.NAZI_BIO_LAB_Description;
            Flags = MyMissionFlags.Story;
            AchievementName = MySteamAchievementNames.Mission26_BioResearch;

            /* sector where the mission is located */
            Location = new MyMissionLocation(BaseSector, (uint)EntityID.TargetUse01); // Posledne cislo - ID dummy pointu kde prijimam misiu (v tomto pripade tiez 'player start')

            RequiredMissions = new MyMissionID[] { MyMissionID.REICHSTAG_A };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.NAZI_BIO_LAB_REACH_MEETING_POINT };
            RequiredActors = new MyActorEnum[] { MyActorEnum.TARJA, MyActorEnum.VALENTIN, MyActorEnum.MADELYN };


            m_objectives = new List<MyObjective>();

            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeGetOut, 0, (uint)EntityID.SpawnpointAtGettingOut1));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeGetOut, 0, (uint)EntityID.SpawnpointAtGettingOut2));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeGetOut, 0, (uint)EntityID.SpawnpointAtGettingOut3));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeGetOut, 0, (uint)EntityID.SpawnpointAtGettingOut4));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringWarehouseAtHigh, 0, (uint)EntityID.SpawnpointAtWarehouse1));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringWarehouseAtHigh, 0, (uint)EntityID.SpawnpointAtWarehouse2));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringWarehouseAtLow, 0, (uint)EntityID.SpawnpointUponWarehouse));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringBlueprintLocation, 0, (uint)EntityID.SpawnpointAtBlueprintLocation));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringMainConnectionRoom, 0, (uint)EntityID.SpawnpointAtConnectionRoom1));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringMainConnectionRoom, 0, (uint)EntityID.SpawnpointAtConnectionRoom2));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringBioBrainsCircularRoom, 0, (uint)EntityID.SpawnpointAtBrainsConnection));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeTakingBrainSample, 0, (uint)EntityID.SpawnpointAtBrains));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectroBeforeEnteringLowerPartOfBiomachine, 0, (uint)EntityID.SpawnpointAtLowerPartOfBiomach));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringHigherPartOfBiomachine, 0, (uint)EntityID.SpawnpointAtHigherPartOfBiomach1));
            Components.Add(new MySpawnpointWaves((uint)EntityID.DetectorBeforeEnteringHigherPartOfBiomachine, 0, (uint)EntityID.SpawnpointAtHigherPartOfBiomach2));

            var m_01GetBiomachSamples1 = new MyUseObjective(
                (MyTextsWrapperEnum.NAZI_BIO_LAB_SAMPLES_BIOMACH_1_Name),
                MyMissionID.NAZI_BIO_LAB_SAMPLES_BIOMACH_1,
                (MyTextsWrapperEnum.NAZI_BIO_LAB_SAMPLES_BIOMACH_1_Description),
                null,
                this,
                new MyMissionID[] {},
                new MyMissionLocation(BaseSector, (uint)EntityID.TargetUse01),
                MyTextsWrapperEnum.PressToPickSamples,
                MyTextsWrapperEnum.Samples,
                MyTextsWrapperEnum.PickingInProgress,
                3000, 
                MyUseObjectiveType.Hacking,
                MyDialogueEnum.WHITEWOLVES_RESEARCH_0200_ENTER
               ) { SaveOnSuccess = true };
            // destroySolarDefence.OnMissionSuccess += ToHangarSubmissionSuccess;
            m_objectives.Add(m_01GetBiomachSamples1);
            m_01GetBiomachSamples1.OnMissionLoaded += new MissionHandler(m_01GetBiomachSamples1_OnMissionLoaded);
            m_01GetBiomachSamples1.OnSuccessDialogueStarted += new MissionHandler(m_01GetBiomachSamples1_OnSuccessDialogueStarted);

            var m_02GetInsideBiomach = new MyObjective(
                (MyTextsWrapperEnum.NAZI_BIO_LAB_GET_INSIDE_Name),
                MyMissionID.NAZI_BIO_LAB_GET_INSIDE,
                (MyTextsWrapperEnum.NAZI_BIO_LAB_GET_INSIDE_Description),
                null,
                this,
                new MyMissionID[] {MyMissionID.NAZI_BIO_LAB_SAMPLES_BIOMACH_1},
                new MyMissionLocation(BaseSector, (uint) EntityID.Target02)
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudStation };
            // destroySolarDefence.OnMissionSuccess += ToHangarSubmissionSuccess;
            m_objectives.Add(m_02GetInsideBiomach);
                            

            var m_04GetOrganicSamples = new MyUseObjective(
                (MyTextsWrapperEnum.NAZI_BIO_LAB_SAMPLES_ORGANIC_Name),
                MyMissionID.NAZI_BIO_LAB_SAMPLES_ORGANIC,
                (MyTextsWrapperEnum.NAZI_BIO_LAB_SAMPLES_ORGANIC_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.NAZI_BIO_LAB_GET_INSIDE },
                new MyMissionLocation(BaseSector, (uint) EntityID.TargetUse04),
                MyTextsWrapperEnum.PressToPickSamples,
                MyTextsWrapperEnum.Samples,
                MyTextsWrapperEnum.PickingInProgress,
                3000,
                MyUseObjectiveType.Hacking,
                MyDialogueEnum.WHITEWOLVES_RESEARCH_0300_COLLECT2
                ) { SaveOnSuccess = true };
            // destroySolarDefence.OnMissionSuccess += ToHangarSubmissionSuccess;
            m_04GetOrganicSamples.OnMissionLoaded += new MissionHandler(m_04GetOrganicSamples_OnMissionLoaded);
            m_04GetOrganicSamples.OnSuccessDialogueStarted += new MissionHandler(m_04GetOrganicSamples_OnSuccessDialogueStarted);
            m_objectives.Add(m_04GetOrganicSamples);

            var m_05GetBiomachBlueprints = new MyUseObjective(
                (MyTextsWrapperEnum.NAZI_BIO_LAB_BLUEPRINTS_BIOMACH_Name),
                MyMissionID.NAZI_BIO_LAB_BLUEPRINTS_BIOMACH,
                (MyTextsWrapperEnum.NAZI_BIO_LAB_BLUEPRINTS_BIOMACH_Description),
                null,
                this,
                new MyMissionID[] {MyMissionID.NAZI_BIO_LAB_SAMPLES_ORGANIC},
                new MyMissionLocation(BaseSector, (uint) EntityID.TargetUse05),
                MyTextsWrapperEnum.PressToDownloadBluePrints,
                MyTextsWrapperEnum.DataTransfer,
                MyTextsWrapperEnum.DownloadingData, 
                3000
                ) { SaveOnSuccess = true };
            // destroySolarDefence.OnMissionSuccess += ToHangarSubmissionSuccess;
            m_objectives.Add(m_05GetBiomachBlueprints);
            m_05GetBiomachBlueprints.OnMissionLoaded += new MissionHandler(m_05GetBiomachBlueprints_OnMissionLoaded);
            m_05GetBiomachBlueprints.OnSuccessDialogueStarted += new MissionHandler(m_05GetBiomachBlueprints_OnSuccessDialogueStarted);

            var m_07GetOut = new MyObjective(
            (MyTextsWrapperEnum.NAZI_BIO_LAB_GET_OUT_Name),
            MyMissionID.NAZI_BIO_LAB_GET_OUT,
            (MyTextsWrapperEnum.NAZI_BIO_LAB_GET_OUT_Description),
            null,
            this,
            new MyMissionID[] { MyMissionID.NAZI_BIO_LAB_BLUEPRINTS_BIOMACH },
            new MyMissionLocation(BaseSector, (uint)EntityID.Target07)
        ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            // destroySolarDefence.OnMissionSuccess += ToHangarSubmissionSuccess;
            m_07GetOut.OnMissionLoaded += new MissionHandler(m_07GetOut_OnMissionLoaded);
            m_objectives.Add(m_07GetOut);


            var m_08Destroy = new MyObjectiveDestroy(
                (MyTextsWrapperEnum.NAZI_BIO_LAB_DESTROY_Name),
                MyMissionID.NAZI_BIO_LAB_DESTROY,
                (MyTextsWrapperEnum.NAZI_BIO_LAB_DESTROY_Description),
                null,
                this,
                new MyMissionID[] {MyMissionID.NAZI_BIO_LAB_GET_OUT,},
                new List<uint>() {(uint) EntityID.Target08},
                null,
                true,
                false
                ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };

            m_objectives.Add(m_08Destroy);
            m_08Destroy.OnMissionLoaded += M08DestroyOnMissionLoaded;
            m_08Destroy.OnMissionSuccess += M08DestroyOnMissionSuccess;

            var returnToMothership = new MyObjective(
               (MyTextsWrapperEnum.NAZI_BIO_LAB_REACH_MEETING_POINT_Name),
               MyMissionID.NAZI_BIO_LAB_REACH_MEETING_POINT,
               (MyTextsWrapperEnum.NAZI_BIO_LAB_REACH_MEETING_POINT_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.NAZI_BIO_LAB_DESTROY },
                //new MyMissionLocation(baseSector, (uint)EntityID.HangerEscapeLocation)
               new MyMissionLocation(BaseSector, MyMissionLocation.MADELYN_HANGAR),
               radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
           ) { HudName = MyTextsWrapperEnum.HudMadelynsSapho };
            returnToMothership.OnMissionLoaded += new MissionHandler(returnToMothership_OnMissionLoaded);
            m_objectives.Add(returnToMothership);

        }

        void returnToMothership_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.WHITEWOLVES_RESEARCH_0700_RETURN);
        }

        void m_01GetBiomachSamples1_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointAtVeryStart1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointAtVeryStart2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.SpawnpointAtVeryStart3);

            MyScriptWrapper.Highlight((uint)EntityID.BlinkLight1, true, this);

            MyScriptWrapper.PlayDialogue(MyDialogueEnum.WHITEWOLVES_RESEARCH_0100_COLLECT);
        }

        void m_01GetBiomachSamples1_OnSuccessDialogueStarted(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.BlinkLight1, false, this);
        }

       
        void m_04GetOrganicSamples_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.BlinkLight3, true, this);
            MyScriptWrapper.Highlight((uint)EntityID.BlinkLight4, true, this);

            SickensDialog();
            //MissionTimer.RegisterTimerAction(3000, SickensDialog, false);
        }


        void m_04GetOrganicSamples_OnSuccessDialogueStarted(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.BlinkLight3, false, this);
            MyScriptWrapper.Highlight((uint)EntityID.BlinkLight4, false, this);
        }


        void m_05GetBiomachBlueprints_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.BlinkLight5, true, this);
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.WHITEWOLVES_RESEARCH_0400_FIND);
        }


        void m_05GetBiomachBlueprints_OnSuccessDialogueStarted(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.BlinkLight5, false, this);
        }


        void m_07GetOut_OnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.WHITEWOLVES_RESEARCH_0500_GET_OUT);
        }

        void M08DestroyOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint)EntityID.Target08, true, this);

            MyScriptWrapper.PlayDialogue(MyDialogueEnum.WHITEWOLVES_RESEARCH_0550_GET_OUT_SUCCESS);
        }

        private void M08DestroyOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.WHITEWOLVES_RESEARCH_0600_DESTROY);
            Boom1();
            MissionTimer.RegisterTimerAction(250, Boom1Hide, false);
            MissionTimer.RegisterTimerAction(1000, Boom2, false);
            MissionTimer.RegisterTimerAction(1250, Boom2Hide, false);
            MissionTimer.RegisterTimerAction(2020, Boom3, false);
            MissionTimer.RegisterTimerAction(2450, Boom3Hide, false);
        }


    
       
        void SickensDialog()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.WHITEWOLVES_RESEARCH_0350_SICKEN);
        }

     

        private void Boom1()
        {
            //EnableEntities(m_particlesExplosion1);
            //MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_particlesExplosion1[0]), MySoundCuesEnum.SfxShipLargeExplosion);
            MyScriptWrapper.AddExplosions(m_particlesExplosion1, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 1000000);
            Debug.WriteLine("BOOM1 " + MissionTimer.ElapsedTime);
        }

        private void Boom1Hide()
        {
           // TryHideEntities(m_prefabsExplosion1);
            
        }
        private void Boom2()
        {
            //EnableEntities(m_particlesExplosion2);
            //MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_particlesExplosion2[0]), MySoundCuesEnum.SfxShipLargeExplosion);
            MyScriptWrapper.AddExplosions(m_particlesExplosion2, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 1000000);
            Debug.WriteLine("BOOM2 " + MissionTimer.ElapsedTime);
        }

        private void Boom2Hide()
        {
            //TryHideEntities(m_prefabsExplosion2);
        }
        
        private void Boom3()
        {
            //MyScriptWrapper.DestroyEntities(m_prefabsExplosion3);
            //EnableEntities(m_particlesExplosion3);
            //DisableEntities(m_particlesClose);
            //MyScriptWrapper.PlaySound3D(MyScriptWrapper.GetEntity(m_particlesExplosion3[0]), MySoundCuesEnum.SfxShipLargeExplosion);
            MyScriptWrapper.AddExplosions(m_particlesExplosion3, Explosions.MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION, 100000);
            Debug.WriteLine("BOOM3 " + MissionTimer.ElapsedTime);
        }

        private void Boom3Hide()
        {
            //TryHideEntities(m_prefabsExplosion3);

        }

        public override void Load()
        {
            if (!IsMainSector) return;

            base.Load();

            // Change player faction to Rainiers and set relations between them
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyFactions.RELATION_WORST);
            MyScriptWrapper.FixBotNames();

            // Set musicmood right from script start
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Horror);

            //TODO: this should be set in editor
            foreach (uint id in m_particlesExplosion1)
            {
                MyDummyPoint dummy = MyScriptWrapper.TryGetEntity(id) as MyDummyPoint;
                if (dummy != null)
                {
                    dummy.DummyFlags |= MyDummyPointFlags.SURIVE_PREFAB_DESTRUCTION;
                }
            }
            foreach (uint id in m_particlesExplosion2)
            {
                MyDummyPoint dummy = MyScriptWrapper.TryGetEntity(id) as MyDummyPoint;
                if (dummy != null)
                {
                    dummy.DummyFlags |= MyDummyPointFlags.SURIVE_PREFAB_DESTRUCTION;
                }
            }
            foreach (uint id in m_particlesExplosion3)
            {
                MyDummyPoint dummy = MyScriptWrapper.TryGetEntity(id) as MyDummyPoint;
                if (dummy != null)
                {
                    dummy.DummyFlags |= MyDummyPointFlags.SURIVE_PREFAB_DESTRUCTION;
                }
            }
        }

        public override void Unload()
        {
            if (!IsMainSector) return;
            base.Unload();
        }

        protected static void TryHideEntities(List<uint> toHide)
        {
            foreach (var u in toHide)
            {
                MyScriptWrapper.TryHide(u);
            }
        }

        protected static void UnHideEntities(List<uint> toHide)
        {
            foreach (var u in toHide)
            {
                MyScriptWrapper.TryUnhide(u);
            }
        }
    }
}
