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


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MySmallPirateBase2: MyMission
    {
        private MyObjectiveDestroy DestroyEnemies;
        private MyObjectiveDestroy DestroyGenerator;

        public MySmallPirateBase2()
        {
            ID = MyMissionID.SMALL_PIRATE_BASE_2; /* ID must be added to MyMissions.cs */
            Name = new StringBuilder("Small pirate base 2"); // Nazev mise
            Description = new StringBuilder( // popis mise
                "Destroy the station\n"
            );

            MyMwcVector3Int baseSector = new MyMwcVector3Int(9708260,0,-2101810); // Story sector of the script

            Location = new MyMissionLocation(baseSector, 1749); // Starting dummy point

            RequiredMissions = new MyMissionID[] { }; // mise ktere musi byt splneny pred prijetim teto mise

            m_submissions = new List<MyObjective>(); // Creating of list of submissions
            // START DEFINICE SUBMISE

            DestroyEnemies = new MyObjectiveDestroy( // Var is used to call functions on that member
                    new StringBuilder("Destroy the mothership"),
                    MyMissionID.SMALL_PIRATE_BASE_2_DESTROY_ENEMIES,
                    new StringBuilder(""),
                    null,
                    this,
                    new MyMissionID[] { },
                    new List<uint> { 5293 },
                    new List<uint> { },
                    false
            ) { SaveOnSuccess = false };
            m_submissions.Add(DestroyEnemies); // pridani do seznamu submisi
           
        
            DestroyGenerator = new MyObjectiveDestroy( // Var is used to call functions on that member
                new StringBuilder("Destroy the generator"),
                MyMissionID.SMALL_PIRATE_BASE_2_DESTROY_GENERATOR,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] {  },
                new List<uint> { 812 },
                new List<uint> {  },
                false
            ) { SaveOnSuccess = false };
            m_submissions.Add(DestroyGenerator); // pridani do seznamu submisi

            var escape = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
               new StringBuilder("Return to the mothership"), // nazev submise
               MyMissionID.PIRATE_BASE_1_ESCAPE, // id submise
               new StringBuilder(""), // popis submise
               null,
               this,
               new MyMissionID[] { MyMissionID.PIRATE_BASE_1_DESTROY_GENERATOR, }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
               new MyMissionLocation(baseSector, 2475) // ID of dummy point of checkpoint
           ) { SaveOnSuccess = false }; // nastaveni save na checkpointu nebo ne
            m_submissions.Add(escape); // pridani do seznamu submisi
        }
        
       

        public override void Load() // vykona se jednou na zacatku
        {
            base.Load();
            
        }

       

        public override void Update()//vola se v kazdem snimku
        {
            base.Update();
             
        

            
        }

    }
       
}
