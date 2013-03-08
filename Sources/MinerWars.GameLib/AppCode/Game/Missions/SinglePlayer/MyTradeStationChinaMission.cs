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
    class MyTradeStationChinaMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 851,
            Inside = 904,
            
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

        public MyTradeStationChinaMission()
        {
            ID = MyMissionID.TRADE_STATION_CHINA; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Trade station - China"); // Name of mission
            Name = MyTextsWrapperEnum.TRADE_STATION_CHINA;
            Description = MyTextsWrapperEnum.TRADE_STATION_CHINA_Description;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(2069219, 0, 5949238); // Story sector of the script - i.e. (-2465,0,6541)

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum

            RequiredMissions = new MyMissionID[] { }; // IDs of missions required to make this mission available
            RequiredMissionsForSuccess = new MyMissionID[] { };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions

         

            //Here you can add another submissions
        }

        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.CalmAtmosphere); // Sets music group to be played in the sector - no matter if the mission is running or not
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
        }

        public override void Update() //Code in that block will be called in each frame
        {
            base.Update();

        }

    }

}
