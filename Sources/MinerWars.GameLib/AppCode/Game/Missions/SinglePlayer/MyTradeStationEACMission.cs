using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
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
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.Resources;


namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyTradeStationEACMission: MyMission
    {
        private enum EntityID
        {
            StartLocation = 185180,
            Hospital_Dummy = 184908,
            Casino_Dummy = 185736,
            Return_Dummy = 186659,
            
            PC_Red = 190983,
            PC_Green = 191347,
            Influence_War = 189376,
            Central_Defense_System = 129402,
        }
        private List<uint> m_turrets = new List<uint>
       {
           129341, 129333, 129329, 129321, 129317, 129313, 129346, 129353, 129371, 129362, 129374, 129406, 129404, 190212, 190215, 190218
       };
        private List<uint> m_PC = new List<uint>
        {
            189835, 189836, 189837, 189834, 189833, 189818, 189822, 189823, 189830, 189831, 189832, 190586, 190584, 190585, 190583, 191347,
        };
        /*private List<uint> m_civilSpawnPoint = new List<uint>
        {
            44050, 22882, 128938, 128939, 128937, 128936, 44199, 180199
        };*/
        private List<uint> m_finalSpawn = new List<uint>
        {
            16778879, 16778892, 
        };
        private List<uint> m_warSpawn = new List<uint>
        {
            188372, 189347
        };




        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (List<uint> list in new List<List<uint>> { m_turrets, m_PC, m_warSpawn, m_finalSpawn })
            {
                foreach (var item in list)
                {
                    MyScriptWrapper.GetEntity(item);
                }
            }
            
        }


        private MyObjectiveEnablePrefabs m_hospital;
        private MyObjective m_casino;


        public MyTradeStationEACMission()
        {
            ID = MyMissionID.TRADE_STATION_EAC; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Trade station - EAC"); // Nazev mise
            Name = MyTextsWrapperEnum.TRADE_STATION_EAC;
            Description = MyTextsWrapperEnum.EmptyDescription;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(5944222,0,-3414281); // Story sector of the script

            Location = new MyMissionLocation(baseSector,(uint)EntityID.StartLocation ); // Startmy point129158

            RequiredMissions = new MyMissionID[] { }; // mise ktere musi byt splneny pred prijetim teto mise
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.TRADE_STATION_EAC_RETURN };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            // START DEFINICE SUBMISE



            m_casino = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
               new StringBuilder("Get into the station"), // nazev submise
               MyMissionID.TRADE_STATION_EAC_CASINO, // id submise
               new StringBuilder(""), // popis submise
               null,
               this,
               new MyMissionID[] { }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
               new MyMissionLocation(baseSector, (uint)EntityID.Casino_Dummy) // ID of dummy point of checkpoint
               ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudStation }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(m_casino); // pridani do seznamu submisi
            
            


            var talkWith = new MyObjectiveDialog(
          (MyTextsWrapperEnum.TRADE_STATION_EAC_DIALOGUE_Name),
          MyMissionID.TRADE_STATION_EAC_DIALOGUE,
          (MyTextsWrapperEnum.TRADE_STATION_EAC_DIALOGUE_Description),
          null,
          this,
          new MyMissionID[] { MyMissionID.TRADE_STATION_EAC_CASINO }
          ) { SaveOnSuccess = false };
            m_objectives.Add(talkWith);

            m_hospital = new MyObjectiveEnablePrefabs(
                new StringBuilder("Activate the central defense system"),
                MyMissionID.TRADE_STATION_EAC_HOSPITAL,
                new StringBuilder(""),
                null,
                this,
                new MyMissionID[] { MyMissionID.TRADE_STATION_EAC_DIALOGUE },
                null,
                new List<uint> { (uint)EntityID.PC_Red },
                new List<uint> { (uint)EntityID.Central_Defense_System }
            ) { HudName = MyTextsWrapperEnum.HudSecurityHub };
            m_objectives.Add(m_hospital);
        

           /* m_hospital = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
               new StringBuilder("Reset station central defense system"), // nazev submise
               MyMissionID.TRADE_STATION_EAC_HOSPITAL, // id submise
               new StringBuilder("Reset station central defense system"), // popis submise
               null,
               this,
               new MyMissionID[] { MyMissionID.TRADE_STATION_EAC_DIALOGUE }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
               new MyMissionLocation(baseSector, (uint)EntityID.PC_Red) // ID of dummy point of checkpoint
               ) { SaveOnSuccess = false }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(m_hospital); // pridani do seznamu submisi*/

            var Return = new MyObjective( // One member of that list - je to MySubmission takze cilem je doletet do checkpointu
              new StringBuilder("Return to Madelyn"), // nazev submise
              MyMissionID.TRADE_STATION_EAC_RETURN, // id submise
              new StringBuilder(""), // popis submise
              null,
              this,
              new MyMissionID[] { MyMissionID.TRADE_STATION_EAC_HOSPITAL }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
              new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR), // ID of dummy point of checkpoint
              radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS
              ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudMadelynsSapho }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(Return); // pridani do seznamu submisi

            m_hospital.OnMissionSuccess += DefenseON;
            m_casino.OnMissionSuccess += AttackBegin;
        }
  

        void AttackBegin(MyMissionBase sender)
        {
            foreach (var turret in m_turrets)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(turret), false);
            }
            MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity((uint)EntityID.Central_Defense_System), false);

            foreach (var Spawn in m_warSpawn)
            {
                MyScriptWrapper.ActivateSpawnPoint(Spawn);
            }
           
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.PC_Green));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.PC_Red));
        }




        void DefenseON(MyMissionBase sender)
        {

            foreach (var turret in m_turrets)
            {
                MyScriptWrapper.SetEntityEnabled(MyScriptWrapper.GetEntity(turret), true);
            }
            foreach (var Spawn in m_warSpawn)
            {
                MyScriptWrapper.DeactivateSpawnPoint(Spawn);
            }
            foreach (var Spawn in m_finalSpawn)
            {
                MyScriptWrapper.ActivateSpawnPoint(Spawn);
            }
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.PC_Red));
            MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity((uint)EntityID.PC_Green));
        }

    

        public override void Load() // vykona se jednou na zacatku
        {

            base.Load();

            foreach (var PC in m_PC)
            {
                MyScriptWrapper.RemoveEntityMark(MyScriptWrapper.GetEntity(PC));
            }
            
       
            MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity((uint)EntityID.PC_Red));
     
           
            
           
            
        }

        public override void Update()//vola se v kazdem snimku
        {
            
            base.Update();
                  
        }

     

    }
       
}
