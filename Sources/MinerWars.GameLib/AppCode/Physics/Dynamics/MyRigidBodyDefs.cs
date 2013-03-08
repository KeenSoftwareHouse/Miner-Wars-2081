#region Using Statements
using System;
#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    [Flags]
    public enum RigidBodyFlag
    {
        RBF_DEFAULT                             = (0), // Default flag
	    RBF_KINEMATIC		                    = (1 << 1), //< Rigid body is kinematic (has to be updated (matrix) per frame, velocity etc is then computed..)
	    RBF_RBO_STATIC                          = (1 << 2), //< Rigid body is static
	    RBF_DISABLED                            = (1 << 3), //< Rigid body is collisions are deactivated and rbo is set active false
	    RBF_DISABLE_COLLISION_RESPONCE          = (1 << 6), //< Rigid body has no collision response
        RBF_INSERTED                            = (1 << 15), //< Rigid body inserted and simulated
        RBF_DIRTY                               = (1 << 16), //< Rigid body is dirty
        RBF_ACTIVE = (1 << 17), //< Rigid body is active

        /// <summary>
        /// If this is false, sphere vs. voxel map collision detection is done using voxel values and not triangles (it is faster).
        /// If true, it uses triangles.
        /// </summary>
        RBF_COLDET_THROUGH_VOXEL_TRIANGLES = (1 << 18),
    }
}
