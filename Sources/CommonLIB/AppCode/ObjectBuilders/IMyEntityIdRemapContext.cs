using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public interface IMyEntityIdRemapContext
    {
        uint? RemapEntityId(uint? currentId);
        string RemapWaypointGroupName(string currentName);
    }
}
