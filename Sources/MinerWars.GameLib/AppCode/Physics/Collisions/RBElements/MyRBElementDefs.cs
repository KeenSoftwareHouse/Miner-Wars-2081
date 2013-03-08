#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// element types
    /// </summary>
    public enum MyRBElementType
    {        
        ET_SPHERE = 0,
        ET_BOX = 1,
        ET_CAPSULE = 2,
        ET_TRIANGLEMESH = 3,
        ET_VOXEL = 4,
        ET_LAST = 5,
        ET_UNKNOWN,
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// element flags
    /// </summary>
    public enum MyElementFlag
    {        
        EF_AABB_DIRTY = 1 << 1,
        EF_SENSOR_ELEMENT = 1 << 2,
        EF_RB_ELEMENT = 1 << 3,
        EF_MODEL_PREFER_LOD0 = 1 << 4,
    }
}