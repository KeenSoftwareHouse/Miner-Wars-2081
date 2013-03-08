#region Using Statements

using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Box vs capsule interaction
    /// </summary>

    class MyRBBoxElementCapsuleElementInteraction: MyRBElementInteraction
    {
        protected override bool Interact(bool staticCollision)
        {
            MyCommonDebugUtils.AssertDebug(false);
            return false;
        }

        public override MyRBElementInteraction CreateNewInstance() { return new MyRBBoxElementCapsuleElementInteraction(); }
    }

}