using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Missions.SideMissions
{
    class MySideMissionAssassinationKillSubmission : MyObjective
    {
        public MySmallShipBot Target { get; set; }
        public bool Aborted { get; private set; }

        public MySideMissionAssassinationKillSubmission(MyMission parent)
            :base(new StringBuilder("Kill the enemy"), MyMissionID.SIDE_MISSION_01_ASSASSINATION_KILL, new StringBuilder("Your enemy needs to be terminated"), null, parent, new MyMissionID[0], null) 
        {
            this.IsSideMission = true;
        }

        public override bool IsSuccess()
        {
            if (Target == null)
            {
                // Target already dead/not exists
                Aborted = true;
                return true;
            }
            return Target.IsDead();
        }
    }

    class MySideMissionAssassination : MyMission
    {
        MySideMissionAssassinationKillSubmission killSubmission;

        public MySideMissionAssassination(MyMissionLocation missionLocation)
        {
            this.ID = MyMissionID.SIDE_MISSION_01_ASSASSINATION;
            this.Description = Localization.MyTextsWrapperEnum.SIDE_MISSION_01_ASSASSINATION_Description;
            this.Location = missionLocation;
            this.DebugName = new StringBuilder("Assassination mission");
            killSubmission = new MySideMissionAssassinationKillSubmission(this);
            this.m_objectives = new List<MyObjective>()
            {
                killSubmission
            };
            this.RequiredMissions = new MyMissionID[0];
            this.IsSideMission = true;
        }

        public override void Accept()
        {
            float dist = float.MaxValue;
            MySpawnPoint closestSpawn = null;
            foreach (var spawn in MyEntities.GetEntities().OfType<MySpawnPoint>())
            {
                var d = (this.Location.Entity.GetPosition() - spawn.GetPosition()).LengthSquared();
                if (d < dist)
                {
                    dist = d;
                    closestSpawn = spawn;
                }
            }

            // Spawnpoint exists and has templates
            if (closestSpawn != null && closestSpawn.GetBotTemplates().Any())
            {
                // HACK: This is little hack (manually adding bot to scene), because spawnpoints are broken
                var template = closestSpawn.GetBotTemplates().First();
                killSubmission.Target = (MySmallShipBot)MyEntities.CreateFromObjectBuilderAndAdd("Assassination target", template.m_builder, closestSpawn.WorldMatrix);
            }

            base.Accept();
        }

        public override void Success()
        {
            // Reward player
            if (!killSubmission.Aborted)
            {
                MyScriptWrapper.AddMoney(500);
            }
            base.Success();
        }
    }
}
