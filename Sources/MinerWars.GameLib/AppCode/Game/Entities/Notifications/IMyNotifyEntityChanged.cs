namespace MinerWars.AppCode.Game.Managers.EntityManager.Notifications
{
    /// <summary>
    /// Provides interface that notifies about changes of shared values.
    /// </summary>
    public interface IMyNotifyEntityChanged
    {
        /// <summary>
        /// Called when [world position changed].
        /// </summary>
        /// <param name="source">The source object that caused this event.</param>
        void OnWorldPositionChanged(object source);
    }
}