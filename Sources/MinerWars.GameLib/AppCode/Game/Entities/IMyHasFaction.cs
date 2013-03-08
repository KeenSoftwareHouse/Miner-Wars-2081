using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.AppCode.Game.Entities
{
    interface IMyHasFaction
    {
        MyMwcObjectBuilder_FactionEnum Faction { get; }
    }
}
