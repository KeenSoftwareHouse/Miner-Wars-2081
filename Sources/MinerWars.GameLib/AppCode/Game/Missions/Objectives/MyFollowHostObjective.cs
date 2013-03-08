using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Missions.Objectives
{
    class MyFollowHostObjective : MyObjective
    {
        public MyFollowHostObjective(MyMission parentMission)
            : base(MyTextsWrapperEnum.FollowTarget, MyMissionID.COOP_FOLLOW_HOST_OBJECTIVE, MyTextsWrapperEnum.FollowTarget, null, parentMission, new MyMissionID[] { }, null)
        {
        }

        public override bool IsSuccess()
        {
            return false;
        }

        public void SetObjectives(MyMissionBase mission)
        {
            if (mission != null)
            {
                this.Name = mission.Name;
                this.Description = mission.Description;

                this.NameTemp = mission.NameTemp;
                this.DescriptionTemp = mission.DescriptionTemp;
            }
            else
            {
                Name = MyTextsWrapperEnum.EmptyDescription;
                NameTemp = new StringBuilder();
                Description = MyTextsWrapperEnum.EmptyDescription;
                DescriptionTemp = new StringBuilder();
            }
        }
    }
}
