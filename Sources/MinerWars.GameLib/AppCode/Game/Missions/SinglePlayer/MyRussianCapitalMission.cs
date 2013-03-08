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
    class MyRussianCapitalMission : MyMissionSandboxBase
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 86,
         
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }



        public MyRussianCapitalMission()
        {
            ID = MyMissionID.RUSSIAN_CAPITAL; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("New Moscow - Russian capital"); // Name of mission
            Name = MyTextsWrapperEnum.RUSSIAN_CAPITAL;
            Description = MyTextsWrapperEnum.RUSSIAN_CAPITAL_Description;
            MyMwcVector3Int baseSector = new MyMwcVector3Int(-4839140, 0, -1490646); // Story sector of the script - i.e. (-2465,0,6541)
            RequiredMissions = new MyMissionID[] { };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredActors = new MyActorEnum[] { MyActorEnum.MADELYN };
           

            m_objectives = new List<MyObjective>(); // Creating of list of submissions


        }


        private float m_oldRelation;


        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyScriptWrapper.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3);
            m_oldRelation = MyFactions.GetFactionsStatus(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Russian);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Russian, MyFactions.RELATION_WORST);
        }


        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Russian, m_oldRelation);
        }
    }
}