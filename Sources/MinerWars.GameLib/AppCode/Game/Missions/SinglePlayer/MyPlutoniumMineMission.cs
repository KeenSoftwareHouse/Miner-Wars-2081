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
    class MyPlutoniumMineMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 31787,
            Detector1 = 34770,
            Detector2 = 34779,
            Spawn1 = 34769,
            Spawn2 = 34778,


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

        public MyPlutoniumMineMission()
        {
            ID = MyMissionID.URANITE_MINE; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Plutonium mine"); // Name of mission
            Name = MyTextsWrapperEnum.URANITE_MINE;
            Description = MyTextsWrapperEnum.URANITE_MINE_Description;
            MyMwcVector3Int baseSector = new MyMwcVector3Int(-7271990, 0, 1111311); // Story sector of the script - i.e. (-2465,0,6541)
            RequiredMissions = new MyMissionID[] { };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };



            m_objectives = new List<MyObjective>(); // Creating of list of submissions


        }




        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3); // Sets music group to be played in the sector - no matter if the mission is running or not

            base.Load();

            m_Detector1 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector1));
            m_Detector1.OnEntityEnter += Detector1Action;
            m_Detector1.On();

            m_Detector2 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector2));
            m_Detector2.OnEntityEnter += Detector2Action;
            m_Detector2.On();

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
        }

        public void Detector1Action(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn1);
                m_Detector1.Off();
            }
        }

        public void Detector2Action(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn2);
                m_Detector2.Off();
            }
        }

        public override void Update() //Code in that block will be called in each frame
        {
            base.Update();

        }
    }
}