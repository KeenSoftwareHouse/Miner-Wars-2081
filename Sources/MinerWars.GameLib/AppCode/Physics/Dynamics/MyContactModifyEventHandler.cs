using MinerWarsMath;

namespace MinerWars.AppCode.Physics
{
    interface IMyContactModifyNotifications
    {
        // return false in case of contact refuse
        // Must be thread safe, careful what you are changing!
        bool OnContact(ref MyRBSolverConstraint constraint);
    }
}
