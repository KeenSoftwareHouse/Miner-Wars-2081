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
    class MySmallPirateBaseMission: MyMission
    {
        private enum EntityID
        {
            StartLocation = 1649,
            Generator = 11241,
            Generator_2 = 16012,
            Escape_Dummy = 2475,
            Escape_Spawn_1 = 2679,
            Escape_Spawn_2 = 2943,
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
            (uint)EntityID.Generator, (uint)EntityID.Generator_2
        };
        private MyObjectiveDestroy DestroyPipes1;
        private MyObjectiveDestroy DestroyGenerator;
       

        public MySmallPirateBaseMission()
        {
            ID = MyMissionID.PIRATE_BASE_1; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Small pirate base"); // Nazev mise
            Name = MyTextsWrapperEnum.PIRATE_BASE_1;
            Description = MyTextsWrapperEnum.PIRATE_BASE_1_Description;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(14393995,0,2939339); // Story sector of the script

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Starting dummy point

            RequiredMissions = new MyMissionID[] { }; // mise ktere musi byt splneny pred prijetim teto mise
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.PIRATE_BASE_1_ESCAPE };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            // START DEFINICE SUBMISE

        
            DestroyGenerator = new MyObjectiveDestroy( // Var is used to call functions on that member
                new StringBuilder("Destroy the generator"),
                MyMissionID.PIRATE_BASE_1_DESTROY_GENERATOR,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] {  },
                generators,
                false
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudGenerator };
            m_objectives.Add(DestroyGenerator); // pridani do seznamu submisi
            DestroyGenerator.OnMissionSuccess += new Missions.MissionHandler(DestroyMSSuccess);

            var escape = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
                new StringBuilder("Escape"), // nazev submise
                MyMissionID.PIRATE_BASE_1_ESCAPE, // id submise
                new StringBuilder(""), // popis submise
                null,
                this,
                new MyMissionID[] { MyMissionID.PIRATE_BASE_1_DESTROY_GENERATOR, }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
                new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR), // ID of dummy point of checkpoint
                radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
            ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudMadelynsSapho }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(escape); // pridani do seznamu submisi

                  


           
        }
        void DestroyMSSuccess(MyMissionBase sender)
        {
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Escape_Spawn_1);
            MyScriptWrapper.ActivateSpawnPoint((uint)EntityID.Escape_Spawn_2);
        }
      
       

        public override void Load() // vykona se jednou na zacatku
        {
            base.Load();
           // MyScriptWrapper.MarkEntity(MyEntities.GetEntityById((MyEntityIdentifier)MyEntityIdentifier.FromNullableInt((uint)EntityID.Generator)), "Generator");           
        }

       

        public override void Update()//vola se v kazdem snimku
        {
            base.Update();
          
            
          
             
        

            
        }

    }
       
}
