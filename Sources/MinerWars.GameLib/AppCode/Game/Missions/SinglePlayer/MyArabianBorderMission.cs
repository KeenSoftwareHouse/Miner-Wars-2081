using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.World.Global;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyArabianBorderMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 301,
            Cargo1 = 690,
            Cargo2 = 691,
            Cargo3 = 692,
            Cargo4 = 693,
            Cargo5 = 694,
            Cargo6 = 695,
            Cargo7 = 696,
            Cargo8 = 697,
            Cargo9 = 698,
            Cargo10 = 699,
            Cargo11 = 700,
            Cargo12 = 701,
            Detector1 = 1398,
            Detector2 = 1396,
            Detector3 = 1394,
            Spawn1 = 1388,
            Spawn2 = 1390,
            Spawn3 = 1393,
            
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }


        private MyEntityDetector m_Detector1;
        private MyEntityDetector m_Detector2;
        private MyEntityDetector m_Detector3;
        private MyEntityDetector m_Detector4;

        public MyArabianBorderMission()
        {
            ID = MyMissionID.ARABIAN_BORDER; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Arabian border"); // Name of mission
            Name = MyTextsWrapperEnum.ARABIAN_BORDER;
            Description = MyTextsWrapperEnum.ARABIAN_BORDER_Description;
            MyMwcVector3Int baseSector = new MyMwcVector3Int(6477920, 0, -2331108); // Story sector of the script - i.e. (-2465,0,6541)
            RequiredMissions = new MyMissionID[] { };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };


            m_objectives = new List<MyObjective>(); // Creating of list of submissions
        }




        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();

            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3); // Sets music group to be played in the sector - no matter if the mission is running or not

            TimeSpan respawnTime = TimeSpan.FromSeconds(60);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo1, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo2, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo3, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo4, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo5, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo6, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo7, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo8, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo9, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo10, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo11, respawnTime);
            MyScriptWrapper.SetCargoRespawn((uint)EntityID.Cargo12, respawnTime);


            m_Detector1 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector1));
            m_Detector1.OnEntityEnter += Detector1Action;
            m_Detector1.On();

            m_Detector2 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector2));
            m_Detector2.OnEntityEnter += Detector2Action;
            m_Detector2.On();

            m_Detector3 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector3));
            m_Detector3.OnEntityEnter += Detector3Action;
            m_Detector3.On();

        }


        public override void Unload()
        {
            base.Unload();

            if (m_Detector1 != null)
            {
                m_Detector1.OnEntityEnter -= Detector1Action;
            }
            if (m_Detector2 != null)
            {
                m_Detector2.OnEntityEnter -= Detector2Action;
            } 
            if (m_Detector3 != null)
            {
                m_Detector3.OnEntityEnter -= Detector3Action;
            } 
           
        }

        public void Detector1Action(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn1);
                m_Detector1.Off();
            }
        }

        public void Detector2Action(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn2);
                m_Detector2.Off();
            }
        }

        public void Detector3Action(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (MySession.IsPlayerShip(entity))
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn3);
                m_Detector3.Off();
            }
        }
    }
}