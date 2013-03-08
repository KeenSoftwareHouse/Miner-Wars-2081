using System;

namespace SharpDX.Toolkit
{
    /// <summary>
    /// Current timing used for variable-step (real time) or fixed-step (game time) games.
    /// </summary>
    public class GameTime
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTime" /> class.
        /// </summary>
        public GameTime()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTime" /> class.
        /// </summary>
        /// <param name="totalGameTime">The total game time since the start of the game.</param>
        /// <param name="elapsedGameTime">The elapsed game time since the last update.</param>
        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
        {
            TotalGameTime = totalGameTime;
            ElapsedGameTime = elapsedGameTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTime" /> class.
        /// </summary>
        /// <param name="totalGameTime">The total game time since the start of the game.</param>
        /// <param name="elapsedGameTime">The elapsed game time since the last update.</param>
        /// <param name="isRunningSlowly">True if the game is running unexpectedly slowly.</param>
        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime, bool isRunningSlowly)
        {
            TotalGameTime = totalGameTime;
            ElapsedGameTime = elapsedGameTime;
            IsRunningSlowly = isRunningSlowly;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the elapsed game time since the last update
        /// </summary>
        /// <value>The elapsed game time.</value>
        public TimeSpan ElapsedGameTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the game is running slowly than its TargetElapsedTime. This can be used for example to render less details...etc.
        /// </summary>
        /// <value><c>true</c> if this instance is running slowly; otherwise, <c>false</c>.</value>
        public bool IsRunningSlowly { get; private set; }

        /// <summary>
        /// Gets the amount of game time since the start of the game.
        /// </summary>
        /// <value>The total game time.</value>
        public TimeSpan TotalGameTime { get; private set; }

        /// <summary>
        /// Gets the current frame count since the start of the game.
        /// </summary>
        public int FrameCount { get; internal set; }

        internal void Update(TimeSpan totalGameTime, TimeSpan elapsedGameTime, bool isRunningSlowly)
        {
            TotalGameTime = totalGameTime;
            ElapsedGameTime = elapsedGameTime;
            IsRunningSlowly = isRunningSlowly;
        }

        #endregion
    }
}