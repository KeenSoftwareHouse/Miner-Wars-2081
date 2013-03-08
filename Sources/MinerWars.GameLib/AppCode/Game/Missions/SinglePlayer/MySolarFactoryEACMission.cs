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
using MinerWars.AppCode.Game.Missions.Objectives;



namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MySolarfactoryEACMission : MyMission
    {
        private List<List<uint>> m_ValidateIDlists;
        private enum EntityID
        {
            StartLocation = 1670,
            Transformator_1 = 2139,
            Transformator_2 = 2137,
            Transformator_3 = 2140,
            Transformator_4 = 2138,
            Transformator_5 = 2057,
            Transformator_6 = 2275,
            Transformator_7 = 2056,
            Transformator_8 = 2058,
            Return_Dummy = 4134,
        }
        private List<uint> m_greenLights = new List<uint>
        {
            2753, 2741, 2742, 2697, 2752
        };
        private List<uint> m_redLights = new List<uint>
        {
            4224, 4227, 4226, 4225, 4228
        };


        public override void ValidateIds() // checks if all IDs in enum are loaded correctly
        {
            foreach (var value in Enum.GetValues(typeof(EntityID)))
            {
                MyScriptWrapper.GetEntity((uint)((value as EntityID?).Value));
            }
            foreach (List<uint> list in m_ValidateIDlists)
            {
                foreach (var item in list)
                {
                    MyScriptWrapper.GetEntity(item);
                }
            }

        }

        private List<uint> Transformators = new List<uint>
        {
            (uint)EntityID.Transformator_1, (uint)EntityID.Transformator_2, (uint)EntityID.Transformator_3, (uint)EntityID.Transformator_4, 
           // (uint)EntityID.Transformator_5, (uint)EntityID.Transformator_6, (uint)EntityID.Transformator_7, (uint)EntityID.Transformator_8,
        };


        public MySolarfactoryEACMission()
        {
            m_ValidateIDlists = new List<List<uint>>();
            m_ValidateIDlists.Add(m_greenLights);
            m_ValidateIDlists.Add(m_redLights);


            ID = MyMissionID.SOLAR_PLANT_EAC; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Solar factory - EAC"); // Nazev mise
            Name = MyTextsWrapperEnum.SOLAR_PLANT_EAC;
            Description = MyTextsWrapperEnum.EmptyDescription;

            MyMwcVector3Int baseSector = new MyMwcVector3Int(2700837, 0, -1774663); // Story sector of the script

            Location = new MyMissionLocation(baseSector, (uint)EntityID.StartLocation); // Startmy point

            RequiredMissions = new MyMissionID[] {  }; // mise ktere musi byt splneny pred prijetim teto mise
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.SOLAR_PLANT_EAC_RETURN };
            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN, MyActorEnum.TARJA, MyActorEnum.VALENTIN };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions
            // START DEFINICE SUBMISE


            var DestroyTransformator = new MyObjectiveDestroy( // Var is used to call functions on that member
                 new StringBuilder("Destroy the transformer"),
                 MyMissionID.SOLAR_PLANT_EAC_DESTROY,
                 new StringBuilder(""),
                 null,
                 this,
                 new MyMissionID[] { },
                 Transformators,
                 false
             ) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudTransformer };
            DestroyTransformator.OnMissionSuccess += new Missions.MissionHandler(DestroyedTransformator);
            m_objectives.Add(DestroyTransformator); // pridani do seznamu submisi            

            var ReturnToMothership = new MyObjective( // One member of that list - je to MyObjective takze cilem je doletet do checkpointu
               new StringBuilder("Return to your mothership"), // nazev submise
               MyMissionID.SOLAR_PLANT_EAC_RETURN, // id submise
               new StringBuilder(""), // popis submise
               null,
               this,
               new MyMissionID[] { MyMissionID.SOLAR_PLANT_EAC_DESTROY, }, // ID submisi ktere musi byt splneny - je to prazdne takze je to prvni submise
               new MyMissionLocation(baseSector, MyMissionLocation.MADELYN_HANGAR), // ID of dummy point of checkpoin
                //StartDialogId: MyDialogueEnum.SLAVERBASE_0900_NUKE_HACKED,
               radiusOverride: MyMissionLocation.MADELYN_HANGAR_RADIUS) { SaveOnSuccess = false, HudName = MyTextsWrapperEnum.HudMadelynsSapho }; // nastaveni save na checkpointu nebo ne
            m_objectives.Add(ReturnToMothership); //

        }
        void DestroyedTransformator(MyMissionBase sender)
        {
            foreach (var Light in m_greenLights)
            {
               var item = MyScriptWrapper.TryGetEntity(Light);
                if (item != null)
                {
                    item.MarkForClose();
                }
            }
            foreach (var Light in m_redLights)
            {
                MyScriptWrapper.UnhideEntity(MyScriptWrapper.GetEntity(Light));
            }
        }



        public override void Load() // vykona se jednou na zacatku
        {
            base.Load();
            /* MyScriptWrapper.SetPlayerFaction(MyMwcObjectBuilder_FactionEnum.Rainiers);
             MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Rainiers, MyMwcObjectBuilder_FactionEnum.Euroamerican, -1000, false);*/
            foreach (var Light in m_redLights)
            {
                MyScriptWrapper.HideEntity(MyScriptWrapper.GetEntity(Light));
            }

        }

        public override void Update()//vola se v kazdem snimku
        {
            base.Update();


        }


    }    
       
}
