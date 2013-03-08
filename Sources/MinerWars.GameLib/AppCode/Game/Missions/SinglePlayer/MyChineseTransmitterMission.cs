using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.Resources;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
   
    class MyChineseTransmitterMission: MyMission
    {
        #region EntityIDs

        enum EntityID
        {
            StartLocation = 15029,
            CargoDummy = 1281,
            SecurityDummy = 7578,
            FindCICDummy = 1285,
            HackDummy = 1283,
            InsideCICDummy = 1284,
            EscapeDummy = 1273,

            EscapeDummy3 = 17191,
            Escape_Spawn1 = 1260,
            Escape_Spawn2 = 1265,
            Escape_Spawn3 = 1272,
            Escape_Spawn4 = 1278,
            Security_Spawn1 = 8294,
            Security_Spawn = 8295,
            PC1 = 7552,
            PC2 = 7553,
            PC3 = 7554,
            PC4 = 7569,
            PC5 = 7570,
            PC6 = 7571,
            PC7 = 7572,
            PC8 = 7573,
            PC9 = 7574,
            PC10 = 7557,
            PC11 = 7558,
            PC12 = 7559,
            PC14 = 9070,
            PC15 = 9026,
            PC16 = 9028,
            PC17 = 9030,
            PC18 = 9032,
            PC19 = 9034,
            PC20 = 9045,
            PC21 = 9043,
            PC22 = 9041,
            PC23 = 9039,
            PC24 = 9037,
            PC25 = 9036,
            PC26 = 9073,
            PC27 = 9048,
            PC28 = 9050,
            PC29 = 9052,
            PC30 = 9054,
            PC31 = 9056,

            PC33 = 9092,
            PC34 = 9094,
            PC35 = 9083,
            PC36 = 9086,
            PC37 = 9088,
            PC38 = 9099,
            PC39 = 9097,
            PC40 = 9096,
            PC42 = 9105,
            PC43 = 9103,
            PC44 = 9101,
            PC45 = 9147,
            PC46 = 9115,
            PC47 = 9117,
            PC48 = 9119,
            PC49 = 9121,
            PC50 = 9123,
            PC51 = 9125,
            PC52 = 9126,
            PC53 = 9128,
            PC54 = 9130,
            PC55 = 9132,
            PC56 = 9134,
            PC57 = 9145,
            PC58 = 9143,
            PC59 = 9141,
            PC60 = 9139,
            PC61 = 9137,
            PC62 = 9150,
            PC63 = 9090,
            PC64 = 241,
            Generator = 9174,
            Loot_Spawn = 9414,
            FindSecurity_Spawn = 9415,
            DestroyGenerator_Spawn = 9416,
            FindCIC_Spawn = 9419,
            PlaceDevice_Spawn = 9420,
            checkpoint = 9847,
            HUB = 226,
            Detector = 15567,
            CargoDetector = 16801,
            CIC_Charge = 16627,
            Detector_Hide = 17153,
            Detector_Unhide = 16976,
            Spawn_Unhide = 17189,
            CICDoors_1 = 234,
            CICDoors_2 = 227,
            Spawn_cargo = 16779562,
            Radar = 12383,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof (EntityID)))
            {
                MyScriptWrapper.GetEntity((uint) ((value as EntityID?).Value));
            }
        }

        List<uint> m_pc = new List<uint>
            {
                (uint) EntityID.PC1,
                (uint) EntityID.PC2,
                (uint) EntityID.PC3,
                (uint) EntityID.PC4,
                (uint) EntityID.PC5,
                (uint) EntityID.PC6,
                (uint) EntityID.PC7,
                (uint) EntityID.PC8,
                (uint) EntityID.PC9,
                (uint) EntityID.PC10,
                (uint) EntityID.PC11,
                (uint) EntityID.PC12,
                (uint) EntityID.PC14,
                (uint) EntityID.PC15,
                (uint) EntityID.PC16,
                (uint) EntityID.PC17,
                (uint) EntityID.PC18,
                (uint) EntityID.PC19,
                (uint) EntityID.PC20,
                (uint) EntityID.PC21,
                (uint) EntityID.PC22,
                (uint) EntityID.PC23,
                (uint) EntityID.PC24,
                (uint) EntityID.PC25,
                (uint) EntityID.PC26,
                (uint) EntityID.PC27,
                (uint) EntityID.PC28,
                (uint) EntityID.PC29,
                (uint) EntityID.PC30,
                (uint) EntityID.PC31,
                (uint) EntityID.PC33,
                (uint) EntityID.PC34,
                (uint) EntityID.PC35,
                (uint) EntityID.PC36,
                (uint) EntityID.PC37,
                (uint) EntityID.PC38,
                (uint) EntityID.PC39,
                (uint) EntityID.PC40,
                (uint) EntityID.PC42,
                (uint) EntityID.PC43,
                (uint) EntityID.PC44,
                (uint) EntityID.PC45,
                (uint) EntityID.PC46,
                (uint) EntityID.PC47,
                (uint) EntityID.PC48,
                (uint) EntityID.PC49,
                (uint) EntityID.PC50,
                (uint) EntityID.PC51,
                (uint) EntityID.PC52,
                (uint) EntityID.PC53,
                (uint) EntityID.PC54,
                (uint) EntityID.PC55,
                (uint) EntityID.PC56,
                (uint) EntityID.PC57,
                (uint) EntityID.PC58,
                (uint) EntityID.PC59,
                (uint) EntityID.PC60,
                (uint) EntityID.PC61,
                (uint) EntityID.PC62,
                (uint) EntityID.PC63,
                // (uint)EntityID.PC64
            };

        #endregion

        private MyObjectiveDestroy m_destroyEnemies;
        private MyObjectiveDialog m_talkWith5;
        private MyEntity m_hub;
        private MyObjective m_hackCIC;
        private MyObjective m_escape;
        private MyEntityDetector m_detector;
        private MyUseObjective m_placeRadarMission;
        private MyEntity m_ravengirl;
        private MyEntity m_ravenguy;
        private MyEntityDetector m_cargodetector;
        private MyEntityDetector m_hidedetector;
        private MyEntityDetector m_unhidedetector;
        private MyObjectiveDialog m_talkWith7;


        public MyChineseTransmitterMission()
        {
            ID = MyMissionID.CHINESE_TRANSMITTER; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.CHINESE_TRANSMITTER;
            Description = MyTextsWrapperEnum.CHINESE_TRANSMITTER_Description;
            DebugName = new StringBuilder("16-PRC transmitter"); // Nazev mise
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(2329559, 0, 4612446); // Story sector of the script

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point

            RequiredMissions = new MyMissionID[] { MyMissionID.BARTHS_MOON_PLANT }; // mise ktere musi byt splneny pred prijetim teto mise
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_ESCAPE3 };
            RequiredActors = new MyActorEnum[] {  MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            // START DEFINICE SUBMISE

           var lootCargo = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
              (MyTextsWrapperEnum.CHINESE_TRANSMITTER_LOOT_CARGO_Name), // nazev submise
              MyMissionID.CHINESE_TRANSMITTER_LOOT_CARGO, // id submise
              (MyTextsWrapperEnum.CHINESE_TRANSMITTER_LOOT_CARGO_Description), // popis submise
              null,
              this,
              new MyMissionID[] { }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
              new MyMissionLocation(baseSector, (uint)EntityID.CargoDummy),
                  startDialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0100_INTRODUCE
          ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudCargo }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(lootCargo);
            lootCargo.OnMissionSuccess += LootSucces;


            //var spawnPointSmartWaves =
            //    new MySpawnpointSmartWaves(
            //        new[] { (uint) EntityID.Security_Spawn1, (uint) EntityID.Loot_Spawn }, null, 2);

            //Components.Add(spawnPointSmartWaves);

            var talkWith = new MyObjectiveDialog(
            MyTextsWrapperEnum.CHINESE_TRANSMITTER_DIALOGUE_1_Name,
            MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_1,
            MyTextsWrapperEnum.CHINESE_TRANSMITTER_DIALOGUE_1_Description,
            null,
            this,
            new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_LOOT_CARGO },
            dialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0200_CARGO_BAY
            ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith);

            var checkpoint = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
               (MyTextsWrapperEnum.CHINESE_TRANSMITTER_CHECKPOINT_Name), // nazev submise
               MyMissionID.CHINESE_TRANSMITTER_CHECKPOINT, // id submise
               (MyTextsWrapperEnum.CHINESE_TRANSMITTER_CHECKPOINT_Description), // popis submise
               null,
               this,
               new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_1 }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
               new MyMissionLocation(baseSector, (uint)EntityID.checkpoint)
                //successDialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0300_CARGO_BAY_2// ID of dummy point of checkpoint
           ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudStation }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(checkpoint); // pridani do seznamu submisi

           

            var findSecurityControl = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
                 (MyTextsWrapperEnum.CHINESE_TRANSMITTER_FIND_SECURITY_CONTROL_Name), // nazev submise
                 MyMissionID.CHINESE_TRANSMITTER_FIND_SECURITY_CONTROL, // id submise
                 (MyTextsWrapperEnum.CHINESE_TRANSMITTER_FIND_SECURITY_CONTROL_Description), // popis submise
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_CHECKPOINT, }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
                 new MyMissionLocation(baseSector, (uint)EntityID.SecurityDummy), // ID of dummy point of checkpoint
                 startDialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0300_CARGO_BAY_2
             ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudControlCenter }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(findSecurityControl);
            findSecurityControl.OnMissionSuccess += FindSecurityControl_OnMissionSuccess;

            var talkWith3 = new MyObjectiveDialog(
            MyTextsWrapperEnum.CHINESE_TRANSMITTER_DIALOGUE_3_Name,
            MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_3,
            MyTextsWrapperEnum.CHINESE_TRANSMITTER_DIALOGUE_3_Description,
            null,
            this,
            new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_FIND_SECURITY_CONTROL},
            dialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0400_SECURITY_ROOM
            ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith3);

            var destroyGenerator = new MyObjectiveDestroy( // Var is used to call functions on that member
                 (MyTextsWrapperEnum.CHINESE_TRANSMITTER_DESTROY_SECURITY_CONTROL_Name),
                 MyMissionID.CHINESE_TRANSMITTER_DESTROY_SECURITY_CONTROL,
                 (MyTextsWrapperEnum.CHINESE_TRANSMITTER_DESTROY_SECURITY_CONTROL_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_3, },
                 new List<uint> { (uint)EntityID.Generator },
                 new List<uint>(),
                 true
             ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudGenerator };
            destroyGenerator.MissionEntityIDs.Add((uint)EntityID.Generator);
            m_objectives.Add(destroyGenerator); // pridani do seznamu submisi
            destroyGenerator.OnMissionSuccess += DestroyMSSucces;

           /*  var talkWith4 = new MyObjectiveDialog(
            new StringBuilder("Find the comand center"),
            MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_4,
            new StringBuilder(""),
            null,
            this,
            new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_DESTROY_SECURITY_CONTROL, },
            dialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0500_GENERATOR_DESTROYED
            ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith4);*/


            var findCIC = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
                 (MyTextsWrapperEnum.CHINESE_TRANSMITTER_FIND_CIC_Name), // nazev submise
                 MyMissionID.CHINESE_TRANSMITTER_FIND_CIC, // id submise
                 (MyTextsWrapperEnum.CHINESE_TRANSMITTER_FIND_CIC_Description), // popis submise
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_DESTROY_SECURITY_CONTROL, }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
                 new MyMissionLocation(baseSector, (uint)EntityID.FindCICDummy), // ID of dummy point of checkpoint
                 startDialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0500_GENERATOR_DESTROYED
             ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudCommandCenter }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(findCIC); // pridani do seznamu submisi
            findCIC.OnMissionSuccess += findCICSucces;

            m_talkWith5 = new MyObjectiveDialog(
            (MyTextsWrapperEnum.CHINESE_TRANSMITTER_DIALOGUE_5_Name),
            MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_5,
            (MyTextsWrapperEnum.CHINESE_TRANSMITTER_DIALOGUE_5_Description),
            null,
            this,
            new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_FIND_CIC,},
            dialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0600_CIC_FOUND
            ) { SaveOnSuccess = false };
            m_objectives.Add(m_talkWith5);

            m_hackCIC = new MyObjectiveEnablePrefabs(
             (MyTextsWrapperEnum.CHINESE_TRANSMITTER_OPEN_CIC_Name),
             MyMissionID.CHINESE_TRANSMITTER_OPEN_CIC,
             (MyTextsWrapperEnum.CHINESE_TRANSMITTER_OPEN_CIC_Description),
             null,
             this,
             new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_5 },
             null,
             new List<uint> { (uint)EntityID.HUB },
             new List<uint> { (uint)EntityID.CICDoors_1, (uint)EntityID.CICDoors_2 }
             ) { HudName = MyTextsWrapperEnum.HudSecurityHub };
            m_objectives.Add(m_hackCIC);

         
            var talkWith6 = new MyObjectiveDialog(
            (MyTextsWrapperEnum.CHINESE_TRANSMITTER_DIALOGUE_6_Name),
            MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_6,
            (MyTextsWrapperEnum.CHINESE_TRANSMITTER_DIALOGUE_6_Description),
            null,
            this,
            new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_OPEN_CIC},
            dialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0700_HUB_HACKED
            ) { SaveOnSuccess = false };
            talkWith6.MissionEntityIDs.Add((uint)EntityID.InsideCICDummy);
            m_objectives.Add(talkWith6);

            m_placeRadarMission = new MyUseObjective(
               (MyTextsWrapperEnum.CHINESE_TRANSMITTER_PLACE_DEVICE_Name),
               MyMissionID.CHINESE_TRANSMITTER_PLACE_DEVICE,
               (MyTextsWrapperEnum.CHINESE_TRANSMITTER_PLACE_DEVICE_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_6, },
               new MyMissionLocation(baseSector, (uint)EntityID.InsideCICDummy),
               MyTextsWrapperEnum.PressToPlaceDetector,
               MyTextsWrapperEnum.DetectorPlacement,
               MyTextsWrapperEnum.DeployingInProgress,
               5000
                //StartDialogId: MyDialogueEnum.CHINESETRANSMITTER_0700_HUB_HACKED
           ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            m_placeRadarMission.OnMissionSuccess += PlaceRadarSucces;
            m_objectives.Add(m_placeRadarMission);

          /*   talkWith7 = new MyObjectiveDialog(
            new StringBuilder("Help Tarja and Valentin"),
            MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_7,
            new StringBuilder(""),
            null,
            this,
            new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_PLACE_DEVICE},
            dialogId: MyDialogueEnum.CHINESE_TRANSMITTER_1000_DETECTOR_PLACED
            ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith7);*/

            m_escape = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
                (MyTextsWrapperEnum.CHINESE_TRANSMITTER_ESCAPE_Name), // nazev submise
                MyMissionID.CHINESE_TRANSMITTER_ESCAPE, // id submise
                (MyTextsWrapperEnum.CHINESE_TRANSMITTER_ESCAPE_Description), // popis submise
                null,
                this,
                new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_PLACE_DEVICE }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
                new MyMissionLocation(baseSector, (uint)EntityID.EscapeDummy),
                startDialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0800_TRANSMITTER_PLACED// ID of dummy point of checkpoint
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.Nothing }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(m_escape); // pridani do seznamu submisi
            m_escape.OnMissionSuccess += Escape2MSSuccess;

             /*var talkWith8 = new MyObjectiveDialog(
            new StringBuilder("Help Tarja and Valentin"),
            MyMissionID.CHINESE_TRANSMITTER_DIALOGUE_8,
            new StringBuilder("Help Tarja and Valentin"),
            null,
            this,
            new MyMissionID[] {MyMissionID.CHINESE_TRANSMITTER_ESCAPE},
            dialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0800_ESCAPE_1
            ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith8);

            */

            var escape3 = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
                 (MyTextsWrapperEnum.CHINESE_TRANSMITTER_ESCAPE3_Name), // nazev submise
                 MyMissionID.CHINESE_TRANSMITTER_ESCAPE3, // id submise
                 (MyTextsWrapperEnum.CHINESE_TRANSMITTER_ESCAPE3_Description), // popis submise
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.CHINESE_TRANSMITTER_ESCAPE }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
                 new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR), // ID of dummy point of checkpoint
                 startDialogId: MyDialogueEnum.CHINESE_TRANSMITTER_0900_ESCAPE_2,
                 successDialogId: MyDialogueEnum.CHINESE_TRANSMITTER_1000_MISSION_COMPLETE,
                 radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
             ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudMadelynsSapho }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(escape3); // pridani do seznamu submisi

            m_talkWith5.OnMissionSuccess += TimeRush; 
        }
       

        void LootSucces(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Loot_Spawn);
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.TarjaAndValentinStaying, MyGuiManager.GetFontMinerWarsGreen(), 5000));
        }

        void FindSecurityControl_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Security_Spawn);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Security_Spawn1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.FindSecurity_Spawn);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn_cargo);
        }

        void DestroyMSSucces(MyMissionBase sender)
        {
            foreach (var pc in m_pc)
            {
                 MyScriptWrapper.SetEntityEnabled(pc, true);
            }
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.DestroyGenerator_Spawn);
        }
       
        void findCICSucces(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.FindCIC_Spawn);
        }

        void Escape2MSSuccess(MyMissionBase sender)
        {
            
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Escape_Spawn1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Escape_Spawn2);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Escape_Spawn3);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Escape_Spawn4);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.CIC_Charge);
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush);
            MyScriptWrapper.Follow(MySession.PlayerShip, m_ravengirl);
            MyScriptWrapper.Follow(MySession.PlayerShip, m_ravenguy);
            MyScriptWrapper.StopTransition(3);
        }

        void PlaceRadarSucces(MyMissionBase sender)
        {
            MyScriptWrapper.Highlight((uint) EntityID.Radar, false, this);
            MyScriptWrapper.EnablePhysics((uint) EntityID.Radar, true);
            MyScriptWrapper.DeactivateSpawnPoint((uint) EntityID.Spawn_cargo);
            //HackCIC.Success();
            //MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity("RavenGuy"));
            //MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity("RavenGirl"));

            MyScriptWrapper.ActivateSpawnPoint((uint) EntityID.Spawn_Unhide);

            //var inventory = MyScriptWrapper.GetPlayerInventory();
            // don't remove radar!!! alien object detector is smallship tool not radar, and you must close inventory item, when you revemo it from inventory
            //var items = new List<MyInventoryItem>();
            //inventory.GetInventoryItems(ref items, MyMwcObjectBuilderTypeEnum.SmallShip_Radar, null);//SmallShip_AlienObjectDetector,

            //inventory.RemoveInventoryItems(items, false);            
            //inventory.RemoveInventoryItems(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ALIEN_OBJECT_DETECTOR, true);
            m_unhidedetector.On();
        }

        void DetectorAction(MyEntity detector, MyEntity bot, int meetCriterias)
        {
            if (detector == m_detector && MyScriptWrapper.IsPlayerShip(bot) && m_hackCIC.IsAvailable())
            {
                m_hackCIC.Success();
                
               // m_detector.OnEntityEnter -= DetectorAction;
            }
            if (detector == m_cargodetector && bot == MyScriptWrapper.GetEntity("RavenGirl"))
            {
                if (!MyScriptWrapper.IsMissionFinished(MyMissionID.CHINESE_TRANSMITTER_ESCAPE))
                {
                    MyScriptWrapper.StopFollow(m_ravengirl);
                    MyScriptWrapper.StopFollow(m_ravenguy);
                }
            }
            if (detector == m_hidedetector && MyScriptWrapper.IsPlayerShip(bot))
            {
                //MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity("RavenGuy"));
                //MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity("RavenGirl"));
            }
        }

        void TimeRush(MyMissionBase sender)
        {
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.StressOrTimeRush, 3);
        }


        public override void Load() // vykona se jednou na zacatku
        {            
            MyScriptWrapper.EnsureInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2);
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.PC64)); // Hiding old useless HUB just for case we will need it again
            m_detector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector));
            m_detector.On();
            m_detector.OnEntityEnter += DetectorAction;
            m_cargodetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.CargoDetector));
            m_cargodetector.On();
            m_cargodetector.OnEntityEnter += DetectorAction;
            m_hidedetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector_Hide));
            m_hidedetector.On();
            m_hidedetector.OnEntityEnter += DetectorAction;
            m_unhidedetector = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector_Unhide));
            
            m_unhidedetector.OnEntityEnter += DetectorAction;
            m_ravengirl = MyScriptWrapper.GetEntity("RavenGirl");
            m_ravenguy = MyScriptWrapper.GetEntity("RavenGuy");
            
           
            foreach (var pc in m_pc)
            {
                MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity(pc));
            }
            
            MyScriptWrapper.Highlight((uint)EntityID.Radar, true, this);
            MyScriptWrapper.EnablePhysics((uint)EntityID.Radar, false);
            
            base.Load();
        }

        public override void Unload()
        {
            m_detector = null;
            m_cargodetector = null;
            m_hidedetector = null;
            m_unhidedetector = null;
            m_ravengirl = null;
            m_ravenguy = null;

            base.Unload();
        }

    }
}
