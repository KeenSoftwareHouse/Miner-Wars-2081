using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Missions
{
    class MyObjectiveDefend : MyObjectiveDestroy
    {
        private List<uint> m_toDefend;

        public MyObjectiveDefend(StringBuilder name, MyMissionID id, StringBuilder description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, List<uint> toKill, List<uint> toDefend, bool displayObjectivesCount = true)
            : this(name, id, description, icon, parentMission, requiredMissions, toKill, new List<uint>(), toDefend, true, displayObjectivesCount)
        {
        }

        public MyObjectiveDefend(StringBuilder name, MyMissionID id, StringBuilder description, MyTexture2D icon, MyMission parentMission, MyMissionID[] requiredMissions, List<uint> toKill, List<uint> toKillSpawnpoints, List<uint> toDefend, bool showMarks = true, bool displayObjectivesCount = true)
            : base(name, id, description, icon, parentMission, requiredMissions, toKill, toKillSpawnpoints, showMarks, displayObjectivesCount)
        {
            m_toDefend = toDefend;
        }

        public override bool IsSuccess()
        {
            return base.IsSuccess() && IsAllEntitiesToDefendAlive();
        }

        public override void Update()
        {
            base.Update();
            if (!IsAllEntitiesToDefendAlive())
            {
                Fail(MyTextsWrapperEnum.Fail_ObjectiveDestroyed);
            }
        }

        private bool IsAllEntitiesToDefendAlive()
        {
            foreach (var entityId in m_toDefend)
            {
                if (MyScriptWrapper.IsEntityDead(entityId))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
