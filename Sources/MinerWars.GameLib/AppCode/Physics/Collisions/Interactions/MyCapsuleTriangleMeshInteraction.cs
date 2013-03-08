#region Using Statements


#endregion

using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// capsule vs trianglemesh interaction
    /// </summary>

    class MyRBCapsuleElementTriangleMeshElementInteraction : MyRBElementInteraction
    {
        protected override bool Interact(bool staticCollision)
        {
            MyCommonDebugUtils.AssertDebug(false);
            return false;
        }

        public override MyRBElementInteraction CreateNewInstance() { return new MyRBCapsuleElementTriangleMeshElementInteraction(); }
    }

}