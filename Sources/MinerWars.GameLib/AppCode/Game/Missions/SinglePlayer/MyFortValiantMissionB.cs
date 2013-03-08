using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Missions.Objectives;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Missions.Submissions;
using MinerWars.Resources;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyFortValiantMissionB : MyFortValiantMissionBase
    {


        #region fields





        private MyObjective m_visitVendor;

        private MyEntity m_madelynDummy;
        private MyEntity m_madelyn;
        #endregion

        public override void Load()
        {
            base.Load();
            m_madelynDummy = MyScriptWrapper.GetEntity((uint) EntityID.MadelynDummy);
            m_madelyn = MyScriptWrapper.GetEntity("Madelyn");
            m_shootWarningSent = false;
            MyScriptWrapper.EntityDeath += OnEntityDeath;
        }



        public MyFortValiantMissionB()
            : base()
        {
            ID = MyMissionID.FORT_VALIANT_B; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("09c-Fort Valiant B"); // Name of mission
            Name = MyTextsWrapperEnum.FORT_VALIANT_B;
            Description = MyTextsWrapperEnum.FORT_VALIANT_B_Description;
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN, MyActorEnum.MARCUS, MyActorEnum.TARJA, MyActorEnum.VALENTIN };
            RequiredMissions = new MyMissionID[] { MyMissionID.SLAVER_BASE_1 };
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.FORT_VALIANT_B_VISIT_VENDOR };
            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            
            var flyOne = new MyObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_B_FLY_ONE_Name),
                MyMissionID.FORT_VALIANT_B_FLY_ONE,
                (MyTextsWrapperEnum.FORT_VALIANT_B_FLY_ONE_Description),
                null,
                this,
                new MyMissionID[] { },
                new MyMissionLocation(baseSector, (uint)EntityID.DetecorGateKeeper),
                null,
                null,
                MyDialogueEnum.FORT_VALIANT_B_0100) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.HudFortValiant };
            m_objectives.Add(flyOne);

            /*
            var speakGateKeeper = new MyMeetObjective(
                new StringBuilder("Reach the gate of Fort Valiant"),
                MyMissionID.FORT_VALIANT_B_SPEAK_GATE_KEEPER,
                new StringBuilder(""),
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_FLY_ONE },
                (uint)EntityID.DetecorGateKeeper,
                (uint)EntityID.GateKeeper,
                25,
                0.25f,
                dialog3,
                dialog2
                ) { SaveOnSuccess = true };
            m_objectives.Add(speakGateKeeper);

            */

            var speakCaptain = new MyMeetObjective(
                (MyTextsWrapperEnum.FORT_VALIANT_B_MEET_CAPTAIN_Name),
                MyMissionID.FORT_VALIANT_B_MEET_CAPTAIN,
                (MyTextsWrapperEnum.FORT_VALIANT_B_MEET_CAPTAIN_Description),
                this,
                new MyMissionID[] { MyMissionID.FORT_VALIANT_B_FLY_ONE },
                (uint)EntityID.DetectorCaptain,
                (uint)EntityID.Captain,
                50,
                0.25f,
                MyDialogueEnum.FORT_VALIANT_B_0200,
                null,
                false
                ) { SaveOnSuccess = true, FollowMe = false};
            m_objectives.Add(speakCaptain);
            speakCaptain.OnMissionSuccess += SpeakCaptainOnOnMissionSuccess;

            m_visitVendor = new MyObjectiveEnterInventory(
               (MyTextsWrapperEnum.FORT_VALIANT_B_VISIT_VENDOR_Name),
               MyMissionID.FORT_VALIANT_B_VISIT_VENDOR,
               (MyTextsWrapperEnum.FORT_VALIANT_B_VISIT_VENDOR_Description),
               null,
               this,
               new MyMissionID[] { MyMissionID.FORT_VALIANT_B_MEET_CAPTAIN },
               (uint)EntityID.VendorB,
               null,
               null
               );
            m_objectives.Add(m_visitVendor);
            m_visitVendor.OnMissionLoaded += MVisitVendorOnOnMissionLoaded; 
                         /*
            var flyToMadelyn = new MyObjective(
                   new StringBuilder("Meet your mothership close to the Templar cargo bay"),
                   MyMissionID.FORT_VALIANT_B_FLY_BACK_MADELYN,
                   new StringBuilder(""),
                   null,
                   this,
                   new MyMissionID[] { MyMissionID.FORT_VALIANT_B_VISIT_VENDOR },
                   new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR)
             );
            m_objectives.Add(flyToMadelyn);
                           */


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
                if (true) // which things
                {
                    Fail(MyTextsWrapperEnum.DontShoot);
                }
            }
        }

        private void MVisitVendorOnOnMissionLoaded(MyMissionBase sender)
        {
            MyScriptWrapper.AddMoney(10000);

            m_captain.SetWaypointPath("Captain");
            m_captain.PatrolMode = MyPatrolMode.CYCLE;
            m_captain.SpeedModifier = 0.25f;
            m_captain.Patrol();
        }

        private void SpeakCaptainOnOnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.AddNotification(MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.NewMissionRecieved, MyHudConstants.FRIEND_FONT, 10000));
            var position = m_madelynDummy.GetPosition();
            var rotation = m_madelynDummy.GetOrientation();
            m_madelyn.MoveAndRotate(position, rotation);

            //m_madelyn.SetWorldMatrix(m_madelynDummy.GetWorldMatrixForDraw());
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint) EntityID.PrefabContainer));
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Scanner1),false);
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Scanner2), false);
        }

        public override void Unload()
        {   
            base.Unload();
            MyScriptWrapper.EntityDeath -= OnEntityDeath;
            
            m_madelynDummy = null;
            m_madelyn = null;
            
            //MyScriptWrapper.RunGlobalEvent(World.Global.MyGlobalEventEnum.SunWind);
            //MyScriptWrapper.RunGlobalEvent(World.Global.MyGlobalEventEnum.MeteorWind);
            //MyScriptWrapper.RunGlobalEvent(World.Global.MyGlobalEventEnum.IceStorm);
        }

    }

}
