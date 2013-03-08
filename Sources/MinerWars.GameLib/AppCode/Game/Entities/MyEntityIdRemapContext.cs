using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities
{
    class MyEntityIdRemapContext : IMyEntityIdRemapContext
    {
        Dictionary<uint, uint> m_remapTable = new Dictionary<uint, uint>();

        // Used for checking when something tries to remap same EntityId twice
        HashSet<uint> m_targetIds = new HashSet<uint>();

        public uint? RemapEntityId(uint? currentId)
        {
            if(!currentId.HasValue) return null;

            //Debug.Assert(!m_targetIds.Contains(currentId.Value), "Id is being remapped twice!");

            uint newValue;
            if (!m_remapTable.TryGetValue(currentId.Value, out newValue))
            {
                newValue = MyEntityIdentifier.AllocateId().NumericValue;
                m_targetIds.Add(newValue);
                m_remapTable.Add(currentId.Value, newValue);
            }
            return newValue;
        }

        public string RemapWaypointGroupName(string currentName)
        {
            string name = currentName;
            uint endNumber = 0;

            // if the group exists, make another group name by adding _1, _2, _3, ...
            while (MyWayPointGraph.GetPath(name) != null)
            {
                endNumber++;
                if (endNumber == 1)
                    name += "_1";
                else
                {
                    int last_ = name.LastIndexOf('_');
                    name = name.Substring(0, last_+1) + endNumber.ToString();
                }
                        
                // name length cap: hashing
                if (name.Length > MyWaypointConstants.MAXIMUM_WAYPOINT_PATH_NAME_LENGTH)
                {
                    endNumber = (uint)name.GetHashCode();
                    name = "G_" + endNumber.ToString();
                }
            }
            return name;
        }
    }
}
