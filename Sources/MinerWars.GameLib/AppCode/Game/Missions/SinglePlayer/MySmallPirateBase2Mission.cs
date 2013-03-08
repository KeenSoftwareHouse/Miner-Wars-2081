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
    class MySmallPirateBase2Mission: MyMission
    {
        private enum EntityID
        {
            StartLocation = 1749,
            Mothership = 5293,
            Generator = 812,
            Generator_2 = 15452,
            Generator_3 = 15454,
            Escape = 18414,
        }
        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
        }
        private List<uint> generators = new List<uint>
        {
            (uint)EntityID.Generator, (uint)EntityID.Generator_2, (uint)EntityID.Generator_3
        };
        private MyObjectiveDestroy DestroyEnemies;
        private MyObjectiveDestroy KillGeneral;
        private MyObjectiveDestroy DestroyGenerator;

        public MySmallPirateBase2Mission()
        {
            ID = MyMissionID.SMALL_PIRATE_BASE_2; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Small pirate base 2"); // Nazev mise
            Name = MyTextsWrapperEnum.SMALL_PIRATE_BASE_2;
            Description = MyTextsWrapperEnum.SMALL_PIRATE_BASE_2_Description;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(9708260,0,-2101810); // Story sector of the script

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point

            RequiredMissions = new MyMissionID[] { }; // mise ktere musi byt splneny pred prijetim teto mise
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.SMALL_PIRATE_BASE_2_KILL_GENERAL };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            // START DEFINICE SUBMISE
            
           /* DestroyEnemies = new MyObjectiveDestroy( // Var is used to call functions on that member
                    new StringBuilder("Destroy the mothership"),
                    MyMissionID.SMALL_PIRATE_BASE_2_DESTROY_MOTHERSHIP,
                    new StringBuilder(""),
                    null,
                    this,
                    new MyMissionID[] { },
                    new List<uint> { (uint)EntityID.Mothership },
                    new List<uint> { },
                    false
            ) { SaveOnSuccess = false };
            m_objectives.Add(DestroyEnemies); // pridani do seznamu submisi*/
        
           DestroyGenerator = new MyObjectiveDestroy( // Var is used to call functions on that member
                new StringBuilder("Destroy the generators"),
                MyMissionID.SMALL_PIRATE_BASE_2_DESTROY_GENERATOR,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { },
                generators,
                false
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudGenerator };
            m_objectives.Add(DestroyGenerator); // pridani do seznamu submisi
            /*DestroyGenerator.MissionEntityIDs.Add((uint)EntityID.Generator_3);
            DestroyGenerator.MissionEntityIDs.Add((uint)EntityID.Generator_2);
            DestroyGenerator.MissionEntityIDs.Add((uint)EntityID.Generator);*/

            var ReturnToMothership = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
               new StringBuilder("Return to your mothership"), // nazev submise
               MyMissionID.SMALL_PIRATE_BASE_2_KILL_GENERAL, // id submise
               new StringBuilder(""), // popis submise
               null,
               this,
               new MyMissionID[] { MyMissionID.SMALL_PIRATE_BASE_2_DESTROY_GENERATOR, }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
               new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR), // ID of dummy point of checkpoint
               radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
           ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudMadelynsSapho }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(ReturnToMothership); // pridani do seznamu submisi

        
        }


        public override void Load() // vykona se jednou na zacatku
        {
            base.Load();


          
        }
       

        public override void Update()//vola se v kazdem snimku
        {
            base.Update();
             
         //MyScriptWrapper.MarkEntity(Generator, "Generator");
          
          
            
        }

    }
       
}
