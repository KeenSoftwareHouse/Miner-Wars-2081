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
    class MyMinerUprisingtMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 7,
         
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }



        public MyMinerUprisingtMission()
        {
            ID = MyMissionID.MINER_UPRISING; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Miner Uprising"); // Name of mission
            Name = MyTextsWrapperEnum.MINER_UPRISING;
            Description = MyTextsWrapperEnum.MINER_UPRISING_Description;
            MyMwcVector3Int baseSector = new MyMwcVector3Int(-2610975, 0, -290706); // Story sector of the script - i.e. (-2465,0,6541)
            RequiredMissions = new MyMissionID[] { };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };
           

            m_objectives = new List<MyObjective>(); // Creating of list of submissions


        }


        private float m_oldRelation;

        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3); // Sets music group to be played in the sector - no matter if the mission is running or not

            m_oldRelation = MyFactions.GetFactionsStatus(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Russian_KGB);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Russian_KGB, MyFactions.RELATION_WORST);
        }

        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Russian_KGB, m_oldRelation);
        }
    }
}