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
    class MySmallPirateBase: MyMission
    {
        private MyObjectiveDestroy DestroyPipes1;
        private MyObjectiveDestroy DestroyGenerator;

        public MySmallPirateBase()
        {
            ID = MyMissionID.PIRATE_BASE_1; /* ID must be added to MyMissions.cs */
            Name = new StringBuilder("Small pirate base"); // Nazev mise
            Description = new StringBuilder( // popis mise
                "Destroy the station\n"
            );

            MyMwcVector3Int baseSector = new MyMwcVector3Int(14393995,0,2939339); // Story sector of the script

            Location = new MyMissionLocation(baseSector, 1649); // Starting dummy point

            RequiredMissions = new MyMissionID[] { }; // mise ktere musi byt splneny pred prijetim teto mise

            m_submissions = new List<MyObjective>(); // Creating of list of submissions
            // START DEFINICE SUBMISE


            /*DestroyPipes1 = new MySubmissionDestroy( // Var is used to call functions on that member
                new StringBuilder("Destroy pipes"),
                MyMissionID.NEW_STORY_M230_DESTROY_PIPES_1,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { },
                new List<uint> { 563, 1859, 415 },
                new List<uint> { },
                false
            ) { SaveOnSuccess = false };
            m_submissions.Add(DestroyPipes1); // pridani do seznamu submisi
            DestroyPipes1.OnMissionSuccess += new Missions.OnMissionSuccess(DestroyMSSuccess);*/

            var findCIC = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
                new StringBuilder("Find the generator"), // nazev submise
                MyMissionID.PIRATE_BASE_1_DESTROY_PIPES_1, // id submise
                new StringBuilder(""), // popis submise
                null,
                this,
                new MyMissionID[] { }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
                new MyMissionLocation(baseSector, 2271) // ID of dummy point of checkpoint
            ) { SaveOnSuccess = false }; // nastaveni save na checkpointu nebo ne
            m_submissions.Add(findCIC); // pridani do seznamu submisi
        
            DestroyGenerator = new MyObjectiveDestroy( // Var is used to call functions on that member
                new StringBuilder("Destroy the generator"),
                MyMissionID.PIRATE_BASE_1_DESTROY_GENERATOR,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] {  },
                new List<uint> { 328 },
                new List<uint> {  },
                false
            ) { SaveOnSuccess = false };
            m_submissions.Add(DestroyGenerator); // pridani do seznamu submisi
            DestroyGenerator.OnMissionSuccess += new MissionHandler(DestroyMSSuccess);

            var escape = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
                new StringBuilder("Return to your mothership"), // nazev submise
                MyMissionID.SMALL_PIRATE_BASE_2_KILL_GENERAL, // id submise
                new StringBuilder(""), // popis submise
                null,
                this,
                new MyMissionID[] {MyMissionID.PIRATE_BASE_1_DESTROY_GENERATOR,MyMissionID.PIRATE_BASE_1_DESTROY_PIPES_1, }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
                new MyMissionLocation(baseSector, 18414)
            ) { SaveOnSuccess = false }; // nastaveni save na checkpointu nebo ne
            m_submissions.Add(escape); // pridani do seznamu submisi

                  


           
        }
        void DestroyMSSuccess(MyMissionBase sender)
        {

            MyScriptWrapper.ActivateSpawnPoint(2679);
            MyScriptWrapper.ActivateSpawnPoint(2943);
            sender.OnMissionSuccess -= DestroyMSSuccess;
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
