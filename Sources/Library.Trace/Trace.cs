using System.Diagnostics;
using System;

#if DEBUG || DEVELOP
    using TraceTool;
#endif

namespace KeenSoftwareHouse.Library.Trace
{   
    /// <summary>
    /// Wrapper around independent trace service.
    /// </summary>
    public static class Trace
    {
        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
#if DEBUG || DEVELOP 
        internal static WinTrace Default 
        { 
        get; 
        set; 
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        static Trace()
        {
#if DEBUG || DEVELOP
            TTrace.Options.SendMode = SendMode.WinMsg;
            TTrace.Options.SendInherited = true;

            var processName = Process.GetCurrentProcess().ProcessName;

            Default = Default ?? new WinTrace(processName, processName);
            Default.ClearAll();
#endif
        }
    }
}