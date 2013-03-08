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

#endregion

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyReichstagAMission : MyMission
    {
        private enum EntityID
        {
            StartLocation = 485,
            Bot_WaltherStauffenberg = 74692,
            Bot_Hans = 74717,
            Bot_Karl = 74742,
            MainBuilding = 307,
        }
        
        public override void ValidateIds()
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }
        
        #region Submissions

        public MyReichstagAMission()
        {
            ID = MyMissionID.REICHSTAG_A; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("18a-Reichstag A");
            Name = MyTextsWrapperEnum.REICHSTAG_A;
            Description = MyTextsWrapperEnum.REICHSTAG_A_Description;
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-2325831, 0, -7186381);

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation);

            RequiredMissions = new MyMissionID[] { MyMissionID.RUSSIAN_TRANSMITTER };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.REICHSTAG_A_COLONEL_DIALOGUE };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>();

            var introduction = new MyObjectiveDialog(
                MyMissionID.REICHSTAG_A_INTRODUCTION,
                null,
                this,
                new MyMissionID[] { },
                dialogId: MyDialogueEnum.REICHSTAG_A_0100_INTRODUCTION
                ) { SaveOnSuccess = true };
            m_objectives.Add(introduction);

            var getCloserToStation = new MyObjective(
                (MyTextsWrapperEnum.REICHSTAG_A_GET_TO_MAIN_BUILDING_Name),
                MyMissionID.REICHSTAG_A_GET_TO_MAIN_BUILDING,
                (MyTextsWrapperEnum.REICHSTAG_A_GET_TO_MAIN_BUILDING_Description),
                null,
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_A_INTRODUCTION },
                new MyMissionLocation(baseSector, (uint)EntityID.MainBuilding)
              ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudMainBuilding };
            getCloserToStation.OnMissionLoaded += GetCloserToStationLoaded;
            m_objectives.Add(getCloserToStation);

            var meetColonel = new MyMeetObjective(
                (MyTextsWrapperEnum.REICHSTAG_A_MEET_COLONEL_Name),
                MyMissionID.REICHSTAG_A_MEET_COLONEL,
                (MyTextsWrapperEnum.REICHSTAG_A_MEET_COLONEL_Description),
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_A_GET_TO_MAIN_BUILDING },
                null,
                (uint)EntityID.Bot_WaltherStauffenberg,
                100,
                0.25f,
                null,
                startDialogueId: MyDialogueEnum.REICHSTAG_A_0300_REACHING_REICHSTAG
               ) { SaveOnSuccess = false, FollowMe = false, HudName = MyTextsWrapperEnum.HudColonelVonStauffenberg };
            m_objectives.Add(meetColonel);


            var colonelDialogue = new MyObjectiveDialog(
                MyMissionID.REICHSTAG_A_COLONEL_DIALOGUE,
                null,
                this,
                new MyMissionID[] { MyMissionID.REICHSTAG_A_MEET_COLONEL },
                dialogId: MyDialogueEnum.REICHSTAG_A_0400_OFFICER_DIALOGUE
                ) { SaveOnSuccess = true };
            m_objectives.Add(colonelDialogue);
            colonelDialogue.OnMissionSuccess += ColonelDialogueSuccess;

        }
        #endregion

        private void GetCloserToStationLoaded(MyMissionBase sender)
        {
            sender.MissionTimer.RegisterTimerAction(5000, TimerActionDelegate, false);
        }

        private void TimerActionDelegate()
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.REICHSTAG_A_0200_ON_THE_WAY);
        }

        private void ColonelDialogueSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.PlayDialogue(MyDialogueEnum.REICHSTAG_A_0500_ON_THE_WAY_BACK);
        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();

            MySmallShipBot FoRrepresentative = (MySmallShipBot)MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_WaltherStauffenberg);
            MyScriptWrapper.ChangeFaction(FoRrepresentative, MyMwcObjectBuilder_FactionEnum.FourthReich);
            FoRrepresentative.IsDestructible = false;
            MySmallShipBot Hans = (MySmallShipBot)MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Hans);
            MySmallShipBot Karl = (MySmallShipBot)MyScriptWrapper.TryGetEntity((uint)EntityID.Bot_Karl);
            Hans.IsDestructible = false;
            Karl.IsDestructible = false;
            Hans.Follow(FoRrepresentative);
            Karl.Follow(FoRrepresentative);

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere); // Sets music group to be played in the sector - no matter if the mission is running or not

            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_NEUTRAL);
        }
    }

}
