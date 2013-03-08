using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Missions
{
    abstract class MyMissionSandboxBase : MyMission
    {
        public override void Load()
        {
            Debug.Assert(Objectives == null || Objectives.Count == 0, "Sandbox mission can't has any objective!");
            base.Load();
            Success();
        }

        public override bool IsValid()
        {
            return Objectives == null || Objectives.Count == 0;
        }
    }
}
