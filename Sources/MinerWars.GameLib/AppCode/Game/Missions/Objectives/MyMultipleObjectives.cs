using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Textures;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Missions
{

    
     abstract class MyMultipleObjectives : MyObjective
        {
            private bool m_displayObjectivesCount;
            private int m_objectivesCountLastUpdate;

            public MyMultipleObjectives(MyTextsWrapperEnum Name, MyMissionID ID, MyTextsWrapperEnum Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, MyMissionLocation Location, List<uint> MissionEntityIDs = null, Audio.Dialogues.MyDialogueEnum? successDialogId = null, Audio.Dialogues.MyDialogueEnum? startDialogId = null, bool displayObjectivesCount = true)
                : base(Name, ID, Description, Icon, ParentMission, RequiredMissions, Location, MissionEntityIDs, successDialogId, startDialogId)
            {
                m_displayObjectivesCount = displayObjectivesCount;
                AdditionalHudInformation = new StringBuilder();
            }

            [Obsolete]
            public MyMultipleObjectives(StringBuilder Name, MyMissionID ID, StringBuilder Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, MyMissionLocation Location, List<uint> MissionEntityIDs = null, Audio.Dialogues.MyDialogueEnum? successDialogId = null, Audio.Dialogues.MyDialogueEnum? startDialogId = null, bool displayObjectivesCount = true)
                : base(Name, ID, Description, Icon, ParentMission, RequiredMissions, Location, MissionEntityIDs, successDialogId, startDialogId)
            {
                m_displayObjectivesCount = displayObjectivesCount;
                AdditionalHudInformation = new StringBuilder();
            }

            public override void Load()
            {
                base.Load();
                ReloadAdditionalHubInfo();
            }

            public override void Update()
            {
                base.Update();
                if (m_objectivesCountLastUpdate != GetObjectivesCompletedCount())
                {
                    ReloadAdditionalHubInfo();
                    m_objectivesCountLastUpdate = GetObjectivesCompletedCount();
                }
            }

            protected void ReloadAdditionalHubInfo()
            {
                AdditionalHudInformation.Clear();
                if (m_displayObjectivesCount)
                {
                    AdditionalHudInformation.AppendInt32(GetObjectivesCompletedCount());
                    AdditionalHudInformation.Append('/');
                    AdditionalHudInformation.AppendInt32(GetObjectivesTotalCount());
                }
            }

            protected abstract int GetObjectivesTotalCount();
            protected abstract int GetObjectivesCompletedCount();
        }
    
}
