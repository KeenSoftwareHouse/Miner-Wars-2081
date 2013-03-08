using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Localization;
using MinerWars.Resources;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{

    class MyFortValiantMission : MyFortValiantMissionBase
    {   


        #region fields


        private MyObjective m_visitVendor;

        #endregion




        public MyFortValiantMission():base()
        {
            ID = MyMissionID.FORT_VALIANT; /* ID must be added to MyMissions.cs */
            Name = MyTextsWrapperEnum.FORT_VALIANT;
            Description = MyTextsWrapperEnum.FORT_VALIANT_Description;
            DebugName = new StringBuilder("09a-Fort Valiant A"); // Name of mission
            RequiredMissions = new MyMissionID[] { MyMissionID.JUNKYARD_RETURN };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.MARCUS, MyActorEnum.TARJA, MyActorEnum.VALENTIN };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.FORT_VALIANT_VISIT_VENDOR };

            #region Objectives
            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            var flyOne = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_FLY_ONE_Name),
                MyMissionID.FORT_VALIANT_FLY_ONE,
                (MyTextsWrapperEnum.FORT_VALIANT_FLY_ONE_Description),
                null,
                this,
                new MyMissionID[] {},
                new MyMissionLocation(baseSector, (uint) EntityID.VendorSpeakDetector1),
                null,
                null,
                MyDialogueEnum.FORT_VALIANT_A_0100
            ){ SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudFortValiant };
            m_objectives.Add(flyOne);


            var speakGateKeeper = new MyMeetObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_SPEAK_GATE_KEEPER_Name),
                MyMissionID.FORT_VALIANT_SPEAK_GATE_KEEPER,
                (MyTextsWrapperEnum.FORT_VALIANT_SPEAK_GATE_KEEPER_Description),
                this,
                new MyMissionID[] {MyMissionID.FORT_VALIANT_FLY_ONE},
                (uint) EntityID.DetecorGateKeeper,
                (uint) EntityID.GateKeeper,
                25,
                0.25f,
                
                MyDialogueEnum.FORT_VALIANT_A_0200,
                null,
                false
            ){SaveOnSuccess =  true, FollowMe = false};
            m_objectives.Add(speakGateKeeper);
            
            
            var speakCaptain = new MyMeetObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_MEET_CAPTAIN_Name),
                MyMissionID.FORT_VALIANT_MEET_CAPTAIN,
                (MyTextsWrapperEnum.FORT_VALIANT_MEET_CAPTAIN_Description),
                this,
                new MyMissionID[] {MyMissionID.FORT_VALIANT_SPEAK_GATE_KEEPER},
                (uint) EntityID.DetectorCaptain,
                (uint) EntityID.Captain,
                50,
                0.25f,
                MyDialogueEnum.FORT_VALIANT_A_0300,
                null,
                false
                ){SaveOnSuccess = true, FollowMe = false};
            speakCaptain.OnMissionLoaded += SpeakCaptainOnOnMissionLoaded;
            speakCaptain.OnMissionSuccess += SpeakCaptainOnOnMissionSuccess;
            m_objectives.Add(speakCaptain);

            m_visitVendor = new MyObjectiveEnterInventory(
                (MyTextsWrapperEnum.FORT_VALIANT_VISIT_VENDOR_Name),
                MyMissionID.FORT_VALIANT_VISIT_VENDOR,
                (MyTextsWrapperEnum.FORT_VALIANT_VISIT_VENDOR_Description),
                null,
                this,
                new MyMissionID[] {MyMissionID.FORT_VALIANT_MEET_CAPTAIN},
                (uint)EntityID.Vendor
                );
            m_visitVendor.OnMissionLoaded += MVisitVendorOnOnMissionLoaded;
            m_objectives.Add(m_visitVendor);
            #endregion
        }


        private bool m_shootWarningSent = false;
        void OnEntityDeath(MyEntity entity, MyEntity killedBy)
        {
            if (killedBy == MySession.PlayerShip && !m_shootWarningSent)
            {
                m_shootWarningSent = true;
                MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.DontShoot, MyHudConstants.ENEMY_FONT, 10000));
                return;
            }

            if (killedBy == MySession.PlayerShip && !MyScriptWrapper.IsMissionFinished(MyMissionID.TWIN_TOWERS_ASSAULT))
            {
                Fail(MyTextsWrapperEnum.DontShoot);
            }
        }


        private void MVisitVendorOnOnMissionLoaded(MyMissionBase sender)
        {
            m_captain.SetWaypointPath("Captain");
            m_captain.PatrolMode = MyPatrolMode.CYCLE;
            m_captain.SpeedModifier = 0.25f;
            m_captain.Patrol();
        }

        private void SpeakCaptainOnOnMissionLoaded(MyMissionBase sender)
        {
            m_gateKeeper.SetWaypointPath("GateKeep");
            m_gateKeeper.PatrolMode = MyPatrolMode.CYCLE;
            m_gateKeeper.SpeedModifier = 0.25f;
            m_gateKeeper.Patrol();
        }


        private void SpeakCaptainOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.NewMissionRecieved, MyHudConstants.FRIEND_FONT, 10000));
        }


        public override void Load()
        {
            MyScriptWrapper.EntityDeath += OnEntityDeath;
            base.Load();
        }

        public override void Unload()
        {
            MyScriptWrapper.EntityDeath -= OnEntityDeath;
            base.Unload();
        }

    }

}
