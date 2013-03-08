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
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.World.Global;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyMilitaryOutpostMission : MyMission
    {
        private enum EntityID // list of IDs used in script
        {
            StartLocation = 1365,
         
        }

        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }



        public MyMilitaryOutpostMission()
        {
            ID = MyMissionID.MILITARY_OUTPOST; /* ID must be added to MyMissions.cs */
            Name = new StringBuilder("Military outpost"); // Name of mission
            Description = new StringBuilder("Conquer the outposts.\n  ");
            MyMwcVector3Int baseSector = new MyMwcVector3Int(-3210402, 0, 2812826); // Story sector of the script - i.e. (-2465,0,6541)
            RequiredMissions = new MyMissionID[] { };
            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point - must by typecasted to uint and referenced from EntityID enum
            RequiredActors = new MyActorEnum[] { MyActorEnum.Madelyn };
           

            m_objectives = new List<MyObjective>(); // Creating of list of submissions


        }




        public override void Load() // Code in that block will be called on the load of the sector
        {
            base.Load();
            MyAudio.ApplyTransition(MyMusicTransitionEnum.HeavyFight, 3); // Sets music group to be played in the sector - no matter if the mission is running or not
            MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
            MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.China, -100);
           

       
         
               }
      
       

        public override void Update() //Code in that block will be called in each frame
        {
            base.Update();

        }
    }
}