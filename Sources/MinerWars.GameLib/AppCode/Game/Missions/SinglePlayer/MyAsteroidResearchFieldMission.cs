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
    class MyAsteroidResearchFieldMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 194,
         
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }



        public MyAsteroidResearchFieldMission()
        {
            ID = MyMissionID.ASTEROID_FIELD; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Asteroid Research Field"); // Name of mission
            Name = MyTextsWrapperEnum.ASTEROID_FIELD;
            Description = MyTextsWrapperEnum.ASTEROID_FIELD_Description;
            MyMwcVector3Int baseSector = new MyMwcVector3Int(27037966, 0, 81020050); // Story sector of the script - i.e. (-2465,0,6541)
            RequiredMissions = new MyMissionID[] { };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };
           

            m_objectives = new List<MyObjective>(); // Creating of list of submissions


        }


        private float m_oldRelation;


        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyAudio.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere, 3); // Sets music group to be played in the sector - no matter if the mission is running or not
            m_oldRelation = MyFactions.GetFactionsStatus(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
        }

        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers, m_oldRelation);
        }
    }
}