using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Managers
{
    /// <summary>
    /// Texture loading mode
    /// </summary>
    internal enum LoadingMode
    {
        /// <summary>
        /// Texture is loaded texture immidiately.
        /// </summary>
        Immediate,

        /// <summary>
        /// Texture is scheduled for load on background thread.
        /// </summary>
        Background,

        /// <summary>
        /// Texture is loaded on first access.
        /// </summary>
        Lazy,

        /// <summary>
        /// Texture is loaded on first access on background thread.
        /// </summary>
        LazyBackground,
    }

    /// <summary>
    /// Loading state of texture.
    /// </summary>
    internal enum LoadState
    {
        Loaded,
        LoadYourself,
        Pending,
        Unloaded,
        Error,
        Loading,
        LoadYourselfBackground,
    }
}