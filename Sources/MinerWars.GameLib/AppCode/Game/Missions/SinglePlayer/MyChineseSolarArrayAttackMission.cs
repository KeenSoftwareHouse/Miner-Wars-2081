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
    class MyChineseSolarArrayAttackMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 289,
            //FlyTarget = 290,
            //DestroyPrefab1Id = 261,
            //DestroyPrefab2Id = 5771,
            //DestroyPrefab3Id = 5773,
            //DestroyPrefab4Id = 5776,
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

        public MyChineseSolarArrayAttackMission()
        {
            ID = MyMissionID.SOLAR_PLANT_CHINA; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Destroy the Chinese solar plant"); // Name of mission
            Name = MyTextsWrapperEnum.SOLAR_PLANT_CHINA;
            Description = MyTextsWrapperEnum.SOLAR_PLANT_CHINA_Description;
            //Flags = MyMissionFlags.Story;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(-1185103, 0, 3822834); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { /*MyMissionID.ALIEN_GATE*/ }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.SOLAR_PLANT_CHINA_FLY_TARGET };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            //var destroy1 = new MyObjectiveDestroy(
            //    new StringBuilder("Destroy the comm station"),
            //    MyMissionID.SOLAR_PLANT_CHINA_DESTROY_TARGET_1,
            //    new StringBuilder("Destroy the satellite communication station to prevent the Chinese from calling reinforcements.\n"),
            //    null,
            //    this,
            //    new MyMissionID[] { },
            //    new List<uint>() { (uint)EntityID.DestroyPrefab1Id }
            //) { SaveOnSuccess = false };
            //m_objectives.Add(destroy1);

            //var destroy2 = new MyObjectiveDestroy(
            //    new StringBuilder("Destroy the accumulators"),
            //    MyMissionID.SOLAR_PLANT_CHINA_DESTROY_TARGET_2,
            //    new StringBuilder("Destroying three accumulators will seriously disrupt the energy supply of the Chinese.\n"),
            //    null,
            //    this,
            //    new MyMissionID[] { MyMissionID.SOLAR_PLANT_CHINA_DESTROY_TARGET_1 },
            //    new List<uint>() { (uint)EntityID.DestroyPrefab2Id, (uint)EntityID.DestroyPrefab3Id, (uint)EntityID.DestroyPrefab4Id }
            //) { SaveOnSuccess = false };
            //m_objectives.Add(destroy2);

            //// START OF REACH OBJECTIVE SUBMISSION DEFINITION
            //var flyInside = new MyObjective( // One member of that list - its type defines the type of submission - MySubmission means fly to the dummypoint to success. Here it is var so you cannot call methods on it
            //    new StringBuilder("Return to Madelyn"), // Name of the submission
            //    MyMissionID.SOLAR_PLANT_CHINA_FLY_TARGET, // ID of the submission - must be added to MyMissions.cs
            //    new StringBuilder("Fly to your mothership to get away.\n"), // Description of the submission
            //    null,
            //    this,
            //    new MyMissionID[] { MyMissionID.SOLAR_PLANT_CHINA_DESTROY_TARGET_2 }, // ID of submissions required to make this submission available - it is clear so this submission is the starting submission
            //    new MyMissionLocation(baseSector, (uint)EntityID.FlyTarget) // ID of dummy point of checkpoint
            //) { SaveOnSuccess = false }; // False means do not save game in that checkpoint
            //m_objectives.Add(flyInside); // Adding this submission to the list of submissions of current mission
            //// END OF REACH OBJECTIVE SUBMISSION DEFINITION
        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere); // Sets music group to be played in the sector - no matter if the mission is running or not

            MyScriptWrapper.FixBotNames();
        }
    }

}
