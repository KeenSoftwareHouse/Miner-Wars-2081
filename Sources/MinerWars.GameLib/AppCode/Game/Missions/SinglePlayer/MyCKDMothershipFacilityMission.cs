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
    class MyCKDMothershipFacilityMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 3499,
            Detector1 = 4752,
            Detector2 = 4754,
            Detector3 = 4757,
            Detector4 = 4759,
            Spawn1 = 3918,
            Spawn2 = 4756,
            Spawn3 = 3916,
            Spawn4 = 4761,
         
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

        public MyCKDMothershipFacilityMission()
        {

                    RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };


            ID = MyMissionID.CKD_MOTHERSHIP_FACILITY; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("CKD Mothership Facility"); // Name of mission
            Name = MyTextsWrapperEnum.CKD_MOTHERSHIP_FACILITY;
            Description = MyTextsWrapperEnum.CKD_MOTHERSHIP_FACILITY_Description;
            MyMwcVector3Int baseSector = new MyMwcVector3Int(1320253, 0, -2846330); // Story sector of the script - i.e. (-2465,0,6541)
            RequiredMissions = new MyMissionID[] { };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };


            m_objectives = new List<MyObjective>(); // Creating of list of submissions


        }




        public override void Load() // Code in that block will be called on the load of the sector
        {
             base.Load();

             m_Detector1 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector1));
            m_Detector1.OnEntityEnter += Detector1Action;
            m_Detector1.On();

            m_Detector2 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector2));
            m_Detector2.OnEntityEnter += Detector2Action;
            m_Detector2.On();

            m_Detector3 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector3));
            m_Detector3.OnEntityEnter += Detector3Action;
            m_Detector3.On();

            m_Detector4 = MyScriptWrapper.GetDetector(MyScriptWrapper.GetEntity((uint)EntityID.Detector4));
            m_Detector4.OnEntityEnter += Detector4Action;
            m_Detector4.On();



            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3); // Sets music group to be played in the sector - no matter if the mission is running or not
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);


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
            if (m_Detector4 != null)
            {
                m_Detector4.OnEntityEnter -= Detector4Action;
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
        public void Detector3Action(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn3);
                m_Detector3.Off();
            }
        }
        public void Detector4Action(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Spawn4);
                m_Detector4.Off();
            }
        }



     
        public override void Update() //Code in that block will be called in each frame
        {
            base.Update();

        }

    }

}
