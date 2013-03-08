using System.Diagnostics;

#if DEBUG || DEVELOP
    using TraceTool;
#endif

namespace KeenSoftwareHouse.Library.Trace
{
    /// <summary>
    /// Allows managing watch windows.
    /// </summary>
    public static class Watch
    {
        /// <summary>
        /// Add object to wach window.
        /// </summary>
        /// <param name="watchName">Name of the watch.</param>
        /// <param name="watchValue">The watch value.</param>
        /// <param name="depth">The depth.</param>
        [Conditional("DEBUG"), Conditional("DEVELOP")]
        public static void Send(string watchName, object watchValue, int depth = 1)
        {
#if DEBUG || DEVELOP
            lock (TTrace.Watches)
            {
                int oldDepth = TTrace.Options.ObjectTreeDepth;

                TTrace.Options.ObjectTreeDepth = depth;
                TTrace.Watches.Send(watchName, watchValue);
                TTrace.Options.ObjectTreeDepth = oldDepth;
            }
#endif
        }
    }
}