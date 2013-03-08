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
    class MyHippieOutpostMission : MyMission
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 96,
            Spawn1 = 172,
            Spawn2 = 173,
            Spawn3 = 167,
            Spawn4 = 170,
            Spawn5 = 169,
            Spawn6 = 168,
            Spawn7 = 171, 
            reach = 1234,
           
            
           
            
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }




        public MyHippieOutpostMission()
        {
            ID = MyMissionID.HIPPIE_OUTPOST; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Hippie Outpost"); // Name of mission
            Name = MyTextsWrapperEnum.HIPPIE_OUTPOST;
            Description = MyTextsWrapperEnum.HIPPIE_OUTPOST_Description;
            MyMwcVector3Int baseSector = new MyMwcVector3Int(238307, 0, -2545498); // Story sector of the script - i.e. (-2465,0,6541)
            RequiredMissions = new MyMissionID[] { };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };


            m_objectives = new List<MyObjective>(); // Creating of list of submissions



            var reach_outpost = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
               new StringBuilder("See what's wrong!"), // Name of the submission
               MyMissionID.REACH_OUTPOST, // ID of the submission - must be added to MyMissions.cs
               new StringBuilder("\n"), // Description of the submission
               null,
               this,
               new MyMissionID[] { }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
               new MyMissionLocation(baseSector, (uint)EntityID.reach) // ID of dummy point of checkpoint
           ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.Nothing }; // False means do not save game in that checkpoint                  
            m_objectives.Add(reach_outpost);


            // START OF DESTROY SUBMISSION DEFINITION
            var KillKharmaSpoilers = new MyObjectiveDestroy( // MySubmissionDestroy means mission with objective to destroy something - here it is class member so you can call methods on it
              new StringBuilder("Place the karma spoilers"), //Name of the submission
              MyMissionID.KILL_ALL, // ID of the submission
              new StringBuilder("Don't let them spread negative energy!\n"), // Description of the submission
              null,
              this,
              new MyMissionID[] { MyMissionID.REACH_OUTPOST }, // ID of submissions required to make this submission available - these declares the sequence of submissions
                         
              null,
              new List<uint> { (uint)EntityID.Spawn1, (uint)EntityID.Spawn2, (uint)EntityID.Spawn3, (uint)EntityID.Spawn4, (uint)EntityID.Spawn5, (uint)EntityID.Spawn6, (uint)EntityID.Spawn7 }, // ID of objects to be destroyed as a mission objective
              true
          ) { SaveOnSuccess = true, HudName = MyTextsWrapperEnum.Nothing };
            m_objectives.Add(KillKharmaSpoilers);
            // END OF DESTROY SUBMISSION DEFINITION
        }


        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.Mystery, 3); // Sets music group to be played in the sector - no matter if the mission is running or not
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
           
            }
        
      
       

        public override void Update() //Code in that block will be called in each frame
        {
            base.Update();

        }
    }
}