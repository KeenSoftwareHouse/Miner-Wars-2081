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
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Localization;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyNewSingaporeIndustryMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 3985,
            //FlyInside = 4604,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }

        //BLOCK FOR CLASS MEMBERS
        private MyObjectiveDestroy m_DestroyEnemies;
        //END OF BLOCK

        public MyNewSingaporeIndustryMission()
        {
            ID = MyMissionID.NEW_SINGAPOUR; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("New Singapore industry"); // Name of mission
            Name = MyTextsWrapperEnum.NEW_SINGAPOUR;
            Description = MyTextsWrapperEnum.NEW_SINGAPOUR_Description;
            //Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-1032721, 0, 5706607); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { /*MyMissionID.ALIEN_GATE*/ }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { /*MyMissionID.NEW_SINGAPOUR_FLY*/ };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions



            //// START OF REACH OBJECTIVE SUBMISSION DEFINITION
            //var FlyInside = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
            //    new StringBuilder("fly\n"), // Name of the submission
            //    MyMissionID.NEW_SINGAPOUR_FLY, // ID of the submission - must be added to MyMissions.cs
            //    new StringBuilder("fly\n"), // Description of the submission
            //    null,
            //    this,
            //    new MyMissionID[] { }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
            //    new MyMissionLocation(baseSector, (uint)EntityID.FlyInside) // ID of dummy point of checkpoint
            //) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            //m_objectives.Add(FlyInside); // Adding this submission to the list of submissions of current mission
            //// END OF REACH OBJECTIVE SUBMISSION DEFINITION




            //Here you can add another submissions
        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere); // Sets music group to be played in the sector - no matter if the mission is running or not
        }

        public override void Update() //Code in that block will be called in each frame
        {
            base.Update();

        }

    }

}

