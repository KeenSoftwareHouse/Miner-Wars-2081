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


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyMedina622Mission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 1483,
            //FlyTarget = 1484,
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }

        //BLOCK FOR CLASS MEMBERS

        //END OF BLOCK

        public MyMedina622Mission()
        {
            ID = MyMissionID.TRADE_STATION_ARABS; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Medina 622 mission"); // Name of mission
            Name = Localization.MyTextsWrapperEnum.MEDINA_622;
            Description = Localization.MyTextsWrapperEnum.MEDINA_622_Description; // Description of mission - do not forget to \n at the end of each line
            //Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(4027778, 0, 861110); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { /*MyMissionID.ALIEN_GATE*/ }; // IDs of missions required to make this mission available
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            //// START OF REACH OBJECTIVE SUBMISSION DEFINITION
            //var flyInside = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
            //    new StringBuilder("Fly over there"), // Name of the submission
            //    MyMissionID.TRADE_STATION_ARABS_FLY_TARGET, // ID of the submission - must be added to MyMissions.cs
            //    new StringBuilder(""), // Description of the submission
            //    null,
            //    this,
            //    new MyMissionID[] { }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
            //    new MyMissionLocation(baseSector, (uint)EntityID.FlyTarget) // ID of dummy point of checkpoint
            //) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            //m_objectives.Add(flyInside); // Adding this submission to the list of submissions of current mission
            //// END OF REACH OBJECTIVE SUBMISSION DEFINITION
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
