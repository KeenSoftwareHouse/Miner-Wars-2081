using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
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
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;




namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyNewJerusalemMission : MyMissionSandboxBase
    {
        private enum EntityID
        {
            StartLocation = 16779508,
           
        }
       



        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (List<uint> list in new List<List<uint>> {  })
            {
                foreach (var item in list)
                {
                    MyScriptWrapper.GetEntity(item);
                }
            }
            
        }


       


        public MyNewJerusalemMission()
        {
            ID = MyMissionID.NEW_JERUSALEM_MISSION; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("New Jerusalem"); // Nazev mise
            Name = MyTextsWrapperEnum.NEW_JERUSALEM_MISSION;
            Description = MyTextsWrapperEnum.NEW_JERUSALEM_MISSION_Description;


            MyMwcVector3Int baseSector = new MyMwcVector3Int(-491618, 0, -2765857); // Story sector of the script

            Location = new MyMissionLocation(baseSector,(uint)EntityID.StartLocation ); // Startmy point129158

            RequiredMissions = new MyMissionID[] {  }; // mise ktere musi byt splneny pred prijetim teto mise
            RequiredMissionsForSuccess = new MyMissionID[] {  };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            // START DEFINICE SUBMISE



           
           
        }
  

       

    

        public override void Load() // vykona se jednou na zacatku
        {

            base.Load();
            

            
            
        }

        public override void Update()//vola se v kazdem snimku
        {
            
            base.Update();
                  
        }

     

    }
       
}
