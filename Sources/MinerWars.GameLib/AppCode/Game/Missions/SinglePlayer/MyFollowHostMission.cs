using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Missions.Objectives;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyFollowHostMission : MyMission
    {
        public MyFollowHostObjective MainObjective { get; private set; }

        public MyFollowHostMission()
        {
            ID = MyMissionID.COOP_FOLLOW_HOST; /* ID must be added to MyMissions.cs */
            DebugName = new StringBuilder("Coop follow host mission"); // Nazev mise
            Flags = MyMissionFlags.Story;

            Location = null;

            RequiredMissions = new MyMissionID[] { }; // mise ktere musi byt splneny pred prijetim teto mise
            RequiredMissionsForSuccess = new MyMissionID[] { MyMissionID.COOP_FOLLOW_HOST_OBJECTIVE };
            RequiredActors = new MyActorEnum[] { };

            m_objectives = new List<MyObjective>(); // Creating of list of submissions

            MainObjective = new MyFollowHostObjective(this);
            m_objectives.Add(MainObjective);

            SetObjectives(null);
        }

        public void SetHudName(StringBuilder hudName)
        {
            HudNameTemp = hudName;
            MainObjective.HudNameTemp = hudName;
        }

        public void SetObjectives(int? missionId/*, List<MyMissionID> optionalObjectives*/)
        {
            MyObjective objective = null;

            if (missionId.HasValue && (objective = MyMissions.GetMissionByID((MyMissionID)missionId.Value) as MyObjective) != null)
            {
                RequiredActors = objective.ParentMission.RequiredActors;
                MainObjective.RequiredActors = objective.RequiredActors;
                InsertRequiredActors();
                objective.InsertRequiredActors();

                Name = objective.ParentMission.Name;
                NameTemp = objective.ParentMission.NameTemp;
                Description = objective.ParentMission.Description;
                DescriptionTemp = objective.ParentMission.DescriptionTemp;
            }
            else
            {
                Name = MyTextsWrapperEnum.EmptyDescription;
                NameTemp = new StringBuilder();
                Description = MyTextsWrapperEnum.EmptyDescription;
                DescriptionTemp = new StringBuilder();
            }

            MainObjective.SetObjectives(objective);

            // Only one objective is active
            //m_objectives.Clear();
            //m_objectives.Add(MainObjective);

            //foreach (var objective in optionalObjectives)
            //{
            //    var baseObjective = MyMissions.GetMissionByID(objective);

            //    if (baseObjective != null)
            //    {
            //        var o = new MyFollowHostObjective(this);
            //        o.SetObjectives(baseObjective);
            //        m_objectives.Add(o);
            //    }
            //}
        }
    }
}
