using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Audio.Dialogues;

namespace MinerWars.AppCode.Game.Missions
{
    class MyObjectiveEnablePrefabs : MyObjective
    {
        private List<uint> m_toEnablePrefabs;

        public MyObjectiveEnablePrefabs(
            MyTextsWrapperEnum name,
            MyMissionID id,
            MyTextsWrapperEnum description,
            MyTexture2D icon,
            MyMission parentMission,
            MyMissionID[] requiredMissions,
            MyMissionLocation location,
            List<uint> missionEntityIDs,
            List<uint> toEnablePrefabs, 
            MyDialogueEnum? successDialogId = null, 
            MyDialogueEnum? startDialogId = null)
            : base(name, id, description, icon, parentMission, requiredMissions, location, missionEntityIDs, successDialogId, startDialogId)
        {
            m_toEnablePrefabs = toEnablePrefabs;
        }

        [Obsolete]
        public MyObjectiveEnablePrefabs(
            StringBuilder name,
            MyMissionID id,
            StringBuilder description,
            MyTexture2D icon,
            MyMission parentMission,
            MyMissionID[] requiredMissions,
            MyMissionLocation location,
            List<uint> missionEntityIDs,
            List<uint> toEnablePrefabs,
            MyDialogueEnum? successDialogId = null,
            MyDialogueEnum? startDialogId = null)
            : base(name, id, description, icon, parentMission, requiredMissions, location, missionEntityIDs, successDialogId, startDialogId)
        {
            m_toEnablePrefabs = toEnablePrefabs;
        }

        public override bool IsSuccess()
        {
            foreach (var id in m_toEnablePrefabs)
            {
                var entity = MyScriptWrapper.TryGetEntity(id);
                if (entity != null && !entity.Enabled)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
