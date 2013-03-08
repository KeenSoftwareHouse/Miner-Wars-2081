#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
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
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.Resources;
using MinerWars.AppCode.Networking;

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyReichstagCMission : MyMission
    {
        private enum EntityID
        {
            StartLocation = 485,
            MoveMS = 1,
            MovePlayer = 2,
            Meetpoint = 307,
            Bot_ShipyardOfficer = 32,
            Bot_TransporterCaptain = 4,
            Bot_WaltherStauffenberg = 74692,
            Bot_Hans = 74717,
            Bot_Karl = 74742,
            Objective_Shipchange = 83406,
            Objective_Mothership = 9,
            Bot_Disabled_01 = 88111,
            Bot_Disabled_02 = 88136,
            Bot_Disabled_03 = 88048,
            Bot_Disabled_04 = 88086,
            Bot_Disabled_05 = 88061,
            Detector_Shipyard = 3,
            Detector_Weaponry = 7,      
        }
        
        public override void ValidateIds()
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }

        private MyEntityDetector m_detectorWeaponry;
        private MyEntityDetector m_detectorShipyard;
        private MyTimerActionDelegate m_SolarwindAction;

        #region Submissions
        
        public MyReichstagCMission()
        {
            ID = MyMissionID.REICHSTAG_C; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("18c-Reichstag C");
            Name = MyTextsWrapperEnum.REICHSTAG_C;
            Description = MyTextsWrapperEnum.REICHSTAG_C_Description;
            Flags = MyMissionFlags.Story;
            AchievementName = MySteamAchievementNames.Mission27_Reichstag2;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-2325831, 0, -7186381);

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.NAZI_BIO_LAB };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.REICHSTAG_C_MOTHERSHIP };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>();


            var MeetSubmission = new MyMeetObjective(
                 (MyTextsWrapperEnum.REICHSTAG_C_FOR_Name),
                MyMissionID.REICHSTAG_C_FOR,
                (MyTextsWrapperEnum.REICHSTAG_C_FOR_Description),
                this,
                new MyMissionID[] { },
                null,
                (uint)EntityID.Bot_WaltherStauffenberg,
                100,
                0.25f,
                null
               ) { SaveOnSuccess = true, FollowMe = false };
          /*  MeetSubmission.OnMissionSuccess += MeetSubmissionSuccess;
            MeetSubmission.OnMissionLoaded += MeetSubmissionLoaded;*/
            m_objectives.Add(MeetSubmission);

            var colonelDialogue = new MyObjectiveDialog(
                MyMissionID.REICHSTAG_C_COLONEL_DIALOGUE,
                null,
                this,
                new MyMissionID[] {MyMissionID.REICHSTAG_C_FOR },
                dialogId: MyDialogueEnum.REICHSTAG_C_0100_OFFICER_TALK
                ) { SaveOnSuccess = true };
            m_objectives.Add(colonelDialogue);

            var goToShipyard = new MyMeetObjective(
                (MyTextsWrapperEnum.REICHSTAG_C_GO_TO_SHIPYARD_Name),
                MyMissionID.REICHSTAG_C_GO_TO_SHIPYARD,
                (MyTextsWrapperEnum.REICHSTAG_C_GO_TO_SHIPYARD_Description),
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_C_COLONEL_DIALOGUE },
                null,
                (uint)EntityID.Bot_ShipyardOfficer,
                100,
                0.25f,
                null,
                startDialogueId: MyDialogueEnum.REICHSTAG_C_0200_ON_THE_WAY
               ) { SaveOnSuccess = true, FollowMe = false };
            m_objectives.Add(goToShipyard);
            goToShipyard.OnMissionLoaded += GoToShipyardLoaded;

            var talkToSupplyOfficer = new MyObjectiveDialog(
                MyMissionID.REICHSTAG_C_TALK_TO_SUPPLY_OFFICER,
                null,
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_C_GO_TO_SHIPYARD },
                dialogId: MyDialogueEnum.REICHSTAG_C_0400_SUPPLY_OFFICER
                ) { SaveOnSuccess = true };
            m_objectives.Add(talkToSupplyOfficer);

            var ChangeShip = new MyUseObjective(
                (MyTextsWrapperEnum.REICHSTAG_C_CHANGESHIP_Name),
                MyMissionID.REICHSTAG_C_CHANGESHIP,
                (MyTextsWrapperEnum.REICHSTAG_C_CHANGESHIP_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_C_TALK_TO_SUPPLY_OFFICER },
                new MyMissionLocation(baseSector, (uint)EntityID.Bot_Disabled_03),
                MyTextsWrapperEnum.PressToBoardShip,
                MyTextsWrapperEnum.Ship,
                MyTextsWrapperEnum.BoardingInProgress,
                3000,
                radiusOverride:30,
                startDialogId: MyDialogueEnum.REICHSTAG_C_0500_REACHING_SHIPS
            );
            m_objectives.Add(ChangeShip);
            ChangeShip.OnMissionSuccess += ChangeShipOnOnMissionSuccess;

            var ShipChangedDialogue = new MyObjectiveDialog(
                MyMissionID.REICHSTAG_C_SHIP_CHANGED_DIALOGUE,
                null,
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_C_CHANGESHIP },
                dialogId: MyDialogueEnum.REICHSTAG_C_0600_SHIPS_PICKUPED
                ) { SaveOnSuccess = true };
            m_objectives.Add(ShipChangedDialogue);
           
            var GetArmed  = new MyObjectiveEnterInventory(
                (MyTextsWrapperEnum.REICHSTAG_C_WEAPONS_Name),
                MyMissionID.REICHSTAG_C_WEAPONS,
                (MyTextsWrapperEnum.REICHSTAG_C_WEAPONS_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_C_SHIP_CHANGED_DIALOGUE },
                (uint)EntityID.Objective_Shipchange,
                successDialogId: MyDialogueEnum.REICHSTAG_C_0800_SHOPPING_FINISHED
            );
            m_objectives.Add(GetArmed);
            GetArmed.OnMissionLoaded += GetArmedLoaded;

            var MeetTransporterCaptain = new MyMeetObjective(
                (MyTextsWrapperEnum.REICHSTAG_C_GO_TO_HANGAR_Name),
                MyMissionID.REICHSTAG_C_GO_TO_HANGAR,
                (MyTextsWrapperEnum.REICHSTAG_C_GO_TO_HANGAR_Description),
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_C_WEAPONS},
                null,
                (uint)EntityID.Bot_TransporterCaptain,
                100,
                0.25f,
                null
               ) { SaveOnSuccess = true, FollowMe = false };
            m_objectives.Add(MeetTransporterCaptain);

            var TalkToTransporterCaptain = new MyObjectiveDialog(
                MyMissionID.REICHSTAG_C_TRANSPORTER_CAPTAIN_DIALOGUE,
                null,
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_C_GO_TO_HANGAR },
                dialogId: MyDialogueEnum.REICHSTAG_C_0900_TRANSPORTER_REACHED
                ) { SaveOnSuccess = true };
            m_objectives.Add(TalkToTransporterCaptain);

            var GetWWMothership = new MyObjective(
                 (MyTextsWrapperEnum.REICHSTAG_C_MOTHERSHIP_Name),
                 MyMissionID.REICHSTAG_C_MOTHERSHIP,
                 (MyTextsWrapperEnum.REICHSTAG_C_MOTHERSHIP_Description),
                 null,
                 this,
                 new MyMissionID[] { MyMissionID.REICHSTAG_C_TRANSPORTER_CAPTAIN_DIALOGUE },
                 new MyMissionLocation(baseSector, (uint)EntityID.Objective_Mothership),
                 radiusOverride: 30
             ) { HudName = MyTextsWrapperEnum.HudMothership };
            m_objectives.Add(GetWWMothership);
        }

        private void GetArmedLoaded(MyMissionBase sender)
        {
            m_detectorWeaponry = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector_Weaponry));
            m_detectorWeaponry.On();
            m_detectorWeaponry.OnEntityEnter += WeaponryDetectorEntered;
        }

        private void WeaponryDetectorEntered(MyEntity detector, MyEntity bot, int meetcriterias)
        {
            if (detector == (MyEntity)m_detectorShipyard && bot == MySession.PlayerShip)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.REICHSTAG_C_0700_SHIPYARD_SHOP);
                m_detectorWeaponry.Off();
                m_detectorWeaponry.OnEntityEnter -= WeaponryDetectorEntered;
            }
        }

        private void GoToShipyardLoaded(MyMissionBase sender)
        {
            m_detectorShipyard = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector_Shipyard));
            m_detectorShipyard.On();
            m_detectorShipyard.OnEntityEnter += ShipyardDetectorEntered;
        }

        private void ShipyardDetectorEntered(MyEntity detector, MyEntity bot, int meetcriterias)
        {
            if (detector == (MyEntity)m_detectorShipyard && bot == MySession.PlayerShip)
            {
                MyScriptWrapper.PlayDialogue(MyDialogueEnum.REICHSTAG_C_0300_REACHING_SHIPYARD);
                m_detectorShipyard.Off();
                m_detectorShipyard.OnEntityEnter -= ShipyardDetectorEntered;
            }
        }

        private void ChangeShipOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.FadedOut += MyScriptWrapper_FadedOut;
            MyScriptWrapper.FadeOut();
        }

        private void MyScriptWrapper_FadedOut()
        {
            MyScriptWrapper.FadedOut -= MyScriptWrapper_FadedOut;
            var ship = MyScriptWrapper.GetEntity((uint)EntityID.Bot_Disabled_03) as MySmallShipBot;

            MyScriptWrapper.ChangeShip(ship);

            MySmallShip vitolino = (MySmallShip)MyScriptWrapper.GetEntity("RavenGuy");
            vitolino.SetName("Disabled_vitolino");
            MyScriptWrapper.DisableShip(vitolino);

            MySmallShip tanja = (MySmallShip)MyScriptWrapper.GetEntity("RavenGirl");
            tanja.SetName("Disabled_tanja");
            MyScriptWrapper.DisableShip(tanja);

            MySmallShip vitolinoNewShip = (MySmallShip)MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Disabled_04);
            if (vitolinoNewShip != null)
            {
                vitolinoNewShip.MarkForClose();
            }
            MyScriptWrapper.InsertFriend(MyActorEnum.VALENTIN, vitolinoNewShip != null ? vitolinoNewShip.ShipType : MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER);

            MySmallShip tanjaNewShip = (MySmallShip)MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Disabled_05);
            if (tanjaNewShip != null)
            {
                tanjaNewShip.MarkForClose();
            }
            MyScriptWrapper.InsertFriend(MyActorEnum.TARJA, tanjaNewShip != null ? tanjaNewShip.ShipType : MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER);

            MyScriptWrapper.FadeIn();
        }

        #endregion

        public override void Load()
        {
            // HACK: ...adding dead entities (dead by solar wind or anything)
            // These entities probably died after Reichstag A mission was successful, so they were not indestructible, solar wind came and eat them
            var colonel = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_WaltherStauffenberg);
            var valkyrie = MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Disabled_03);

            if (colonel == null || valkyrie == null)
            {
                var sector = MyLocalCache.LoadSector(new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.STORY, null, this.GetCurrentSector(), String.Empty));
                var bots = sector.SectorObjects.OfType<MyMwcObjectBuilder_SmallShip_Bot>();
                foreach (var bot in bots)
                {
                    if (colonel == null && bot.EntityId == (uint)EntityID.Bot_WaltherStauffenberg
                        || valkyrie == null && bot.EntityId == (uint)EntityID.Bot_Disabled_03)
                    {
                        MyEntities.CreateFromObjectBuilderAndAdd(bot.DisplayName, bot, bot.PositionAndOrientation.GetMatrix());
                    }
                }
            }

            base.Load();            

            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_BEST);

            MySmallShipBot FoRrepresentative = (MySmallShipBot)MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_WaltherStauffenberg);
            MyScriptWrapper.ChangeFaction(FoRrepresentative, MyMwcObjectBuilder_FactionEnum.FourthReich);
            FoRrepresentative.IsDestructible = false;
            MySmallShipBot Hans = (MySmallShipBot)MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Hans);
            MySmallShipBot Karl = (MySmallShipBot)MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Karl);
            Hans.IsDestructible = false;
            Karl.IsDestructible = false;
            Hans.Follow(FoRrepresentative);
            Karl.Follow(FoRrepresentative);

            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.Bot_Disabled_01), true, false);
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.Bot_Disabled_02), true, false);
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.Bot_Disabled_03), true, false);
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.Bot_Disabled_04), true, false);
            MyScriptWrapper.DisableShip(MyScriptWrapper.GetEntity((uint)EntityID.Bot_Disabled_05), true, false);
        }


        public override void Unload()
        {
            base.Unload();

            if (MyScriptWrapper.IsMissionFinished(this.ID))
            {
                MyScriptWrapper.TravelToMission(MyMissionID.TWIN_TOWERS);
            }
        }

    }
}
