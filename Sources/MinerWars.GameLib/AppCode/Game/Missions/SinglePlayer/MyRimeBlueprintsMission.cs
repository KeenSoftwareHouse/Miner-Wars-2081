using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.WayPoints;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyRimeBlueprintsMission : MyMission
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 9691,
            EntranceL = 9688,
            EntranceR = 15904,
            HUB = 8572,
            Exit = 9690,
            Return = 40235,
            ReturnToMothership = 43179,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }

        public MyObjective m_entranceL;
        public MyObjective m_entranceR;
        public MyObjective m_target;
        public MyObjective m_flyExit;
        public MyEntity m_hub;
    

        public MyRimeBlueprintsMission()
        {
            ID = MyMissionID.RIME_BLUEPRINTS; /* ID must be added to MyMissions.cs */
            Name = new StringBuilder("11-Rime:Blueprints"); // Name of mission
            Description = new StringBuilder("Choose one of the entrances and sneak through the laboratory to steal the blueprints\n"); // Description of mission - do not forget to \n at the end of each line
            Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-1922856, 0, -2867519); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { MyMissionID.RIME_CONVINCE }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.RIME_BLUEPRINTS_RETURN_TO_MOTHERSHIP };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            // START OF REACH OBJECTIVE SUBMISSION DEFINITION
          m_entranceL = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                new StringBuilder("Enter the quarter"), // Name of the submission
                MyMissionID.RIME_BLUEPRINTS_ENTRANCE1, // ID of the submission - must be added to MyMissions.cs
                new StringBuilder(""), // Description of the submission
                null,
                this,
                new MyMissionID[] { }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                new MyMissionLocation(baseSector, (uint)EntityID.EntranceL) // ID of dummy point of checkpoint
            ) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            m_objectives.Add(m_entranceL); // Adding this submission to the list of submissions of current mission
            // END OF REACH OBJECTIVE SUBMISSION DEFINITION

            // START OF REACH OBJECTIVE SUBMISSION DEFINITION
            m_entranceR = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                new StringBuilder("Entry the quarter with the drone"), // Name of the submission
                MyMissionID.RIME_BLUEPRINTS_ENTRANCE2, // ID of the submission - must be added to MyMissions.cs
                new StringBuilder(""), // Description of the submission
                null,
                this,
                new MyMissionID[] { }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                new MyMissionLocation(baseSector, (uint)EntityID.EntranceR) // ID of dummy point of checkpoint
            ) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            m_objectives.Add(m_entranceR); // Adding this submission to the list of submissions of current mission
            // END OF REACH OBJECTIVE SUBMISSION DEFINITION

            // START OF REACH OBJECTIVE SUBMISSION DEFINITION
            m_target =  new MyObjectiveOptionalRequirements( 
                new StringBuilder("Get the blueprints"),
                MyMissionID.RIME_BLUEPRINTS_TARGET,
                new StringBuilder("Sneak through the base and get the blueprints!\n"),
                null,
                this,
                new MyMissionID[] { },
                null,
                new MyMissionID[] { MyMissionID.RIME_BLUEPRINTS_ENTRANCE1, MyMissionID.RIME_BLUEPRINTS_ENTRANCE2 },
                1,
                new List<uint>() {(uint)EntityID.HUB }
                ) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            m_objectives.Add(m_target);
            // END OF REACH OBJECTIVE SUBMISSION DEFINITION

            m_flyExit = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                new StringBuilder("Leave the laboratory"), // Name of the submission
                MyMissionID.RIME_BLUEPRINTS_EXIT, // ID of the submission - must be added to MyMissions.cs
                new StringBuilder("Use the emergency exit.\n"), // Description of the submission
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_BLUEPRINTS_TARGET }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                new MyMissionLocation(baseSector, (uint)EntityID.Exit) // ID of dummy point of checkpoint
                ) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            m_objectives.Add(m_flyExit);

           var returnToFrancis = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
                new StringBuilder("Get back to Francis"), // Name of the submission
                MyMissionID.RIME_BLUEPRINTS_RETURN_TO_FRANCIS, // ID of the submission - must be added to MyMissions.cs
                new StringBuilder("Bring the blueprints back to Francis Reef\n"), // Description of the submission
                null,
                this,
                new MyMissionID[] { MyMissionID.RIME_BLUEPRINTS_EXIT }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
                new MyMissionLocation(baseSector, (uint)EntityID.Return) // ID of dummy point of checkpoint
                ) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            m_objectives.Add(returnToFrancis);

            var returnToMothership = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
               new StringBuilder("Return to your mothership"), // Name of the submission
               MyMissionID.RIME_BLUEPRINTS_RETURN_TO_MOTHERSHIP, // ID of the submission - must be added to MyMissions.cs
               new StringBuilder(""), // Description of the submission
               null,
               this,
               new MyMissionID[] { MyMissionID.RIME_BLUEPRINTS_RETURN_TO_FRANCIS }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
               new MyMissionLocation(baseSector, (uint)EntityID.ReturnToMothership) // ID of dummy point of checkpoint
               ) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            m_objectives.Add(returnToMothership);
        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            m_entranceR.OnMissionSuccess += entranceR_OnMissionSuccess;
            m_entranceL.OnMissionSuccess += entranceL_OnMissionSuccess;
            m_flyExit.OnMissionSuccess += flyExit_OnMissionSuccess;
            //MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.EntityHacked += MyScriptWrapper_EntityHacked;
            m_hub = MyScriptWrapper.GetEntity((uint)EntityID.HUB);
            MySession.PlayerShip.Inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2, 1f, true);
            MySession.PlayerShip.Inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneUS, 1f, true);
            MyAudio.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere); // Sets music group to be played in the sector - no matter if the mission is running or not

        }

         void MyScriptWrapper_EntityHacked(MyEntity entity)
        {
            if (entity == m_hub)
            {
                m_target.Success();
                MyScriptWrapper.EntityHacked -= MyScriptWrapper_EntityHacked;
            }
         }
             
             
        void flyExit_OnMissionSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Omnicorp, 100);
            sender.OnMissionSuccess -= flyExit_OnMissionSuccess;
        }

        void entranceR_OnMissionSuccess(MyMissionBase sender)
        {
            if (m_entranceL.IsAvailable())
            {
                m_entranceL.Success();
            }
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Omnicorp, -100);
            sender.OnMissionSuccess -= entranceR_OnMissionSuccess;
        }

        void entranceL_OnMissionSuccess(MyMissionBase sender)
        {
            if (m_entranceR.IsAvailable())
            {
                m_entranceR.Success();
            }
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Omnicorp, -100);
            sender.OnMissionSuccess -= entranceL_OnMissionSuccess;
        }

        public override void Update() //Code in that block will be called in each frame
        {
            base.Update();

        }

    }

}
