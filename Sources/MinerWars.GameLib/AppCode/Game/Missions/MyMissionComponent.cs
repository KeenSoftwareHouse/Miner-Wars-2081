using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Game.Missions
{
    class MyMissionComponent
    {
        public virtual void Load(MyMissionBase sender) { }

        public virtual void Unload(MyMissionBase sender) { }

        public virtual void Update(MyMissionBase sender) { }

        public virtual void Success(MyMissionBase sender) { }
    }
}
