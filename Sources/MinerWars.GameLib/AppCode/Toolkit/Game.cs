
using System;
using System.Collections.Generic;

using SharpDX.Collections;
using SharpDX.Toolkit.Graphics;

using SharpDX.Direct3D9;

namespace SharpDX.Toolkit
{

    /// <summary>
    /// The game.
    /// </summary>
    public class Game : Component
    {
        #region Fields

        private readonly GameTime gameTime;
        private readonly int[] lastUpdateCount;
        private readonly float updateCountAverageSlowLimit;
        public readonly GamePlatformDesktop GamePlatform;
        private GraphicsDeviceManager graphicsDeviceManager;
        private bool isEndRunRequired;
        private bool isExiting;
        private bool isFirstUpdateDone;
        private bool suppressDraw;

        private TimeSpan totalGameTime;
        private TimeSpan inactiveSleepTime;
        private TimeSpan maximumElapsedTime;
        private TimeSpan accumulatedElapsedGameTime;
        private TimeSpan lastFrameElapsedGameTime;
        private int nextLastUpdateCountIndex;
        private bool drawRunningSlowly;
        private bool forceElapsedTimeToZero;
        private bool isInitialzing;
        public bool ContentLoaded = false;

        private readonly TimerTick timer;

        private bool isMouseVisible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Game" /> class.
        /// </summary>
        public Game()
        {
            // Internals
            gameTime = new GameTime();
            totalGameTime = new TimeSpan();
            timer = new TimerTick();
            IsFixedTimeStep = true;
            maximumElapsedTime = TimeSpan.FromMilliseconds(500.0);
            TargetElapsedTime = TimeSpan.FromTicks(10000000 / 60); // target elapsed time is by default 60Hz
            lastUpdateCount = new int[4];
            nextLastUpdateCountIndex = 0;

            // Calculate the updateCountAverageSlowLimit (assuming moving average is >=3 )
            // Example for a moving average of 4:
            // updateCountAverageSlowLimit = (2 * 2 + (4 - 2)) / 4 = 1.5f
            const int BadUpdateCountTime = 2; // number of bad frame (a bad frame is a frame that has at least 2 updates)
            var maxLastCount = 2 * Math.Min(BadUpdateCountTime, lastUpdateCount.Length);
            updateCountAverageSlowLimit = (float)(maxLastCount + (lastUpdateCount.Length - maxLastCount)) / lastUpdateCount.Length;

            //GameSystems = new GameSystemCollection();

            // Create Platform
            GamePlatform = new GamePlatformDesktop();
            GamePlatform.Activated += gamePlatform_Activated;
            GamePlatform.Deactivated += gamePlatform_Deactivated;
            GamePlatform.Exiting += gamePlatform_Exiting;

            // By default, add a FileResolver for the ContentManager
          //  Content.Resolvers.Add(new FileSystemContentResolver(gamePlatform.DefaultAppDirectory));

            // Setup registry
            //Services.AddService(typeof(IServiceRegistry), Services);
            //Services.AddService(typeof(IContentManager), Content);
            //Services.AddService(typeof(IGamePlatform), GamePlatform);

            // Register events on GameSystems.
            //GameSystems.ItemAdded += GameSystems_ItemAdded;
            //GameSystems.ItemRemoved += GameSystems_ItemRemoved;

            IsActive = true;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when [activated].
        /// </summary>
        public event EventHandler<EventArgs> Activated;

        /// <summary>
        /// Occurs when [deactivated].
        /// </summary>
        public event EventHandler<EventArgs> Deactivated;

        /// <summary>
        /// Occurs when [exiting].
        /// </summary>
        public event EventHandler<EventArgs> Exiting;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        public Device GraphicsDevice
        {
            get
            {
                return graphicsDeviceManager.GraphicsDevice;
            }
        }

        public GraphicsDeviceManager GraphicsManager
        {
            get { return graphicsDeviceManager; }
            set { graphicsDeviceManager = value; }
        }

        /// <summary>
        /// Gets or sets the inactive sleep time.
        /// </summary>
        /// <value>The inactive sleep time.</value>
        public TimeSpan InactiveSleepTime { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed time step.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed time step; otherwise, <c>false</c>.</value>
        public bool IsFixedTimeStep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse should be visible.
        /// </summary>
        /// <value><c>true</c> if the mouse should be visible; otherwise, <c>false</c>.</value>
        public bool IsMouseVisible
        {
            get
            {
                return isMouseVisible;
            }

            set
            {
                isMouseVisible = value;
                if (Window != null)
                {
                    Window.IsMouseVisible = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets or sets the target elapsed time.
        /// </summary>
        /// <value>The target elapsed time.</value>
        public TimeSpan TargetElapsedTime { get; set; }

        /// <summary>
        /// Gets the abstract window.
        /// </summary>
        /// <value>The window.</value>
        public GameWindow Window
        {
            get
            {
                if (GamePlatform != null)
                {
                    return GamePlatform.MainWindow;
                }
                return null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Exits the game.
        /// </summary>
        public void Exit()
        {
            isExiting = true;
            GamePlatform.Exit();
            if (IsRunning && isEndRunRequired)
            {
                EndRun();
                IsRunning = false;
            }
        }

        /// <summary>
        /// Resets the elapsed time counter.
        /// </summary>
        public void ResetElapsedTime()
        {
            forceElapsedTimeToZero = true;
            drawRunningSlowly = false;
            Array.Clear(lastUpdateCount, 0, lastUpdateCount.Length);
            nextLastUpdateCountIndex = 0;
        }

        private void InitializeBeforeRun()
        {
            if (graphicsDeviceManager == null)
            {
                graphicsDeviceManager = new GraphicsDeviceManager(this);
            }

            // Make sure that the device is already created
            graphicsDeviceManager.CreateDevice();

            // Checks the graphics device
            if (graphicsDeviceManager.GraphicsDevice == null)
            {
                throw new InvalidOperationException("No GraphicsDevice found");
            }

            // Initialize this instance and all game systems
            //LoadContent is called in base.Initialize
            Initialize();

            IsRunning = true;

            BeginRun();

            timer.Reset();
            gameTime.Update(totalGameTime, TimeSpan.Zero, false);
            gameTime.FrameCount = 0;

            // Run the first time an update
            Update(gameTime);

            isFirstUpdateDone = true;
        }

        /// <summary>
        /// Call this method to initialize the game, begin running the game loop, and start processing events for the game.
        /// </summary>
        /// <param name="windowContext">The window Context.</param>
        /// <exception cref="System.InvalidOperationException">Cannot run this instance while it is already running</exception>
        public void Run(object windowContext = null)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Cannot run this instance while it is already running");
            }

            try
            {
                // Run the game, loop depending on the platform/window.
                GamePlatform.Run(windowContext, InitializeBeforeRun, Tick);

                if (GamePlatform.IsBlockingRun)
                {
                    // If the previous call was blocking, then we can call Endrun
                    EndRun();
                }
                else
                {
                    // EndRun will be executed on Game.Exit
                    isEndRunRequired = true;
                }
            }
            finally
            {
                if (!isEndRunRequired)
                {
                    IsRunning = false;
                }
            }
        }

        /// <summary>
        /// Prevents calls to Draw until the next Update.
        /// </summary>
        public void SuppressDraw()
        {
            suppressDraw = true;
        }

        /// <summary>
        /// Updates the game's clock and calls Update and Draw.
        /// </summary>
        public void Tick()
        {
            // If this instance is existing, then don't make any further update/draw
            if (isExiting)
            {
                return;
            }

            // If this instance is not active, sleep for an inactive sleep time
            if (!IsActive)
            {
                Utilities.Sleep(inactiveSleepTime);
            }

            // Update the timer
            timer.Tick();

            var elapsedAdjustedTime = timer.ElapsedAdjustedTime;

            if (forceElapsedTimeToZero)
            {
                elapsedAdjustedTime = TimeSpan.Zero;
                forceElapsedTimeToZero = false;
            }

            if (elapsedAdjustedTime > maximumElapsedTime)
            {
                elapsedAdjustedTime = maximumElapsedTime;
            }

            bool suppressNextDraw = true;
            int updateCount = 1;
            var singleFrameElapsedTime = elapsedAdjustedTime;

            if (IsFixedTimeStep)
            {
                // If the rounded TargetElapsedTime is equivalent to current ElapsedAdjustedTime
                // then make ElapsedAdjustedTime = TargetElapsedTime. We take the same internal rules as XNA 
                if (Math.Abs(elapsedAdjustedTime.Ticks - TargetElapsedTime.Ticks) < (TargetElapsedTime.Ticks >> 6))
                {
                    elapsedAdjustedTime = TargetElapsedTime;
                }

                // Update the accumulated time
                accumulatedElapsedGameTime += elapsedAdjustedTime;

                // Calculate the number of update to issue
                updateCount = (int)(accumulatedElapsedGameTime.Ticks / TargetElapsedTime.Ticks);

                // If there is no need for update, then exit
                if (updateCount == 0)
                {
                    return;
                }

                // Calculate a moving average on updateCount
                lastUpdateCount[nextLastUpdateCountIndex] = updateCount;
                float updateCountMean = 0;
                for (int i = 0; i < lastUpdateCount.Length; i++)
                {
                    updateCountMean += lastUpdateCount[i];
                }

                updateCountMean /= lastUpdateCount.Length;
                nextLastUpdateCountIndex = (nextLastUpdateCountIndex + 1) % lastUpdateCount.Length;

                // Test when we are running slowly
                drawRunningSlowly = updateCountMean > updateCountAverageSlowLimit;

                // We are going to call Update updateCount times, so we can substract this from accumulated elapsed game time
                accumulatedElapsedGameTime = new TimeSpan(accumulatedElapsedGameTime.Ticks - (updateCount * TargetElapsedTime.Ticks));
                singleFrameElapsedTime = TargetElapsedTime;
            }
            else
            {
                Array.Clear(lastUpdateCount, 0, lastUpdateCount.Length);
                nextLastUpdateCountIndex = 0;
                drawRunningSlowly = false;
            }

            // Reset the time of the next frame
            for (lastFrameElapsedGameTime = TimeSpan.Zero; updateCount > 0 && !isExiting; updateCount--)
            {
                gameTime.Update(totalGameTime, singleFrameElapsedTime, drawRunningSlowly);
                try
                {
                    Update(gameTime);

                    // If there is no exception, then we can draw the frame
                    suppressNextDraw &= suppressDraw;
                    suppressDraw = false;
                }
                finally
                {
                    lastFrameElapsedGameTime += singleFrameElapsedTime;
                    totalGameTime += singleFrameElapsedTime;
                }
            }

            if (!suppressNextDraw)
            {
                DrawFrame();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw.
        /// </summary>
        /// <returns><c>true</c> to continue drawing, false to not call <see cref="Draw"/> and <see cref="EndDraw"/></returns>
        protected virtual bool BeginDraw()
        {
            if (!graphicsDeviceManager.BeginDraw())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called after all components are initialized but before the first update in the game loop.
        /// </summary>
        protected virtual void BeginRun()
        {
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                lock (this)
                {
                    var disposableGraphicsManager = graphicsDeviceManager as IDisposable;
                    if (disposableGraphicsManager != null)
                    {
                        disposableGraphicsManager.Dispose();
                    }

                    DisposeGraphicsDeviceEvents();
                }
            }

            base.Dispose(disposeManagedResources);
        }

        /// <summary>
        /// Reference page contains code sample.
        /// </summary>
        /// <param name="gameTime">
        /// Time passed since the last call to Draw.
        /// </param>
        protected virtual void Draw(GameTime gameTime)
        {
          
        }

        /// <summary>Ends the drawing of a frame. This method is preceeded by calls to Draw and BeginDraw.</summary>
        protected virtual void EndDraw()
        {
            if (graphicsDeviceManager != null)
            {
                graphicsDeviceManager.EndDraw();
            }
        }

        /// <summary>Called after the game loop has stopped running before exiting.</summary>
        protected virtual void EndRun()
        {
        }

        /// <summary>Called after the Game and GraphicsDevice are created, but before LoadContent.  Reference page contains code sample.</summary>
        protected virtual void Initialize()
        {
            // Setup the graphics device if it was not already setup.
            SetupGraphicsDeviceEvents();

            // Load the content of the game
            isInitialzing = true;
            LoadContent();
            isInitialzing = false;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public virtual void LoadContent()
        {
            System.Diagnostics.Debug.Assert(ContentLoaded == false);
            ContentLoaded = true;

            // When initializing, we don't need to call explicitly the LoadContent for each GameSystem
            // as they were already called by the GameSystem.Initialize method
            // Otherwise, in case of a GraphicsDevice reset, we need to call GameSystem.LoadContent
            if (isInitialzing)
            {
                return;
            }
        }

        /// <summary>
        /// Raises the Activated event. Override this method to add code to handle when the game gains focus.
        /// </summary>
        /// <param name="sender">The Game.</param>
        /// <param name="args">Arguments for the Activated event.</param>
        protected virtual void OnActivated(object sender, EventArgs args)
        {
            var handler = Activated;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the Deactivated event. Override this method to add code to handle when the game loses focus.
        /// </summary>
        /// <param name="sender">The Game.</param>
        /// <param name="args">Arguments for the Deactivated event.</param>
        protected virtual void OnDeactivated(object sender, EventArgs args)
        {
            var handler = Deactivated;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises an Exiting event. Override this method to add code to handle when the game is exiting.
        /// </summary>
        /// <param name="sender">The Game.</param>
        /// <param name="args">Arguments for the Exiting event.</param>
        protected virtual void OnExiting(object sender, EventArgs args)
        {
            var handler = Exiting;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// This is used to display an error message if there is no suitable graphics device or sound card.
        /// </summary>
        /// <param name="exception">The exception to display.</param>
        /// <returns>The <see cref="bool" />.</returns>
        protected virtual bool ShowMissingRequirementMessage(Exception exception)
        {
            return true;
        }

        /// <summary>
        /// Called when graphics resources need to be unloaded. Override this method to unload any game-specific graphics resources.
        /// </summary>
        public virtual void UnloadContent()
        {
            ContentLoaded = false;
        }

        /// <summary>
        /// Reference page contains links to related conceptual articles.
        /// </summary>
        /// <param name="gameTime">
        /// Time passed since the last call to Update.
        /// </param>
        protected virtual void Update(GameTime gameTime)
        {
            isFirstUpdateDone = true;
        }

        private void gamePlatform_Activated(object sender, EventArgs e)
        {
            if (!IsActive)
            {
                IsActive = true;
                OnActivated(this, EventArgs.Empty);
            }
        }

        private void gamePlatform_Deactivated(object sender, EventArgs e)
        {
            if (IsActive)
            {
                IsActive = false;
                OnDeactivated(this, EventArgs.Empty);
            }
        }

        private void gamePlatform_Exiting(object sender, EventArgs e)
        {
            UnloadContent();

            OnExiting(this, EventArgs.Empty);
        }

        private static bool AddGameSystem<T>(T gameSystem, List<T> gameSystems, IComparer<T> comparer, bool removePreviousSystem = false)
        {
            lock (gameSystems)
            {
                // If we are updating the order
                if (removePreviousSystem)
                {
                    gameSystems.Remove(gameSystem);
                }

                // Find this gameSystem
                int index = gameSystems.BinarySearch(gameSystem, comparer);
                if (index < 0)
                {
                    // If index is negative, that is the bitwise complement of the index of the next element that is larger than item 
                    // or, if there is no larger element, the bitwise complement of Count.
                    index = ~index;

                    // Iterate until the order is different or we are at the end of the list
                    while ((index < gameSystems.Count) && (comparer.Compare(gameSystems[index], gameSystem) == 0))
                    {
                        index++;
                    }

                    gameSystems.Insert(index, gameSystem);

                    // True, the system was inserted
                    return true;
                }
            }

            // False, it is already in the list
            return false;
        }

        private void DrawFrame()
        {
            try
            {
                if (!isExiting && isFirstUpdateDone && !Window.IsMinimized && BeginDraw())
                {
                    gameTime.Update(totalGameTime, lastFrameElapsedGameTime, drawRunningSlowly);
                    gameTime.FrameCount++;

                    Draw(gameTime);

                    EndDraw();
                }
            }
            finally
            {
                lastFrameElapsedGameTime = TimeSpan.Zero;
            }
        }

        private void SetupGraphicsDeviceEvents()
        {                                                                                     /*
            graphicsDeviceManager.DeviceCreated += graphicsDeviceService_DeviceCreated;
            graphicsDeviceManager.DeviceResetting += graphicsDeviceService_DeviceResetting;
            graphicsDeviceManager.DeviceReset += graphicsDeviceService_DeviceReset;
            graphicsDeviceManager.DeviceDisposing += graphicsDeviceService_DeviceDisposing; */
        }

        private void DisposeGraphicsDeviceEvents()
        {
            if (graphicsDeviceManager != null)
            {                                                                                      /*
                graphicsDeviceManager.DeviceCreated -= graphicsDeviceService_DeviceCreated;
                graphicsDeviceManager.DeviceResetting -= graphicsDeviceService_DeviceResetting;
                graphicsDeviceManager.DeviceReset -= graphicsDeviceService_DeviceReset;
                graphicsDeviceManager.DeviceDisposing -= graphicsDeviceService_DeviceDisposing;  */
            }
        }
               /*
        private void graphicsDeviceService_DeviceCreated(object sender, EventArgs e)
        {
            LoadContent();
        }

        private void graphicsDeviceService_DeviceDisposing(object sender, EventArgs e)
        {
            if (ContentLoaded)
            {
                UnloadContent();
            }
        }    

        private void graphicsDeviceService_DeviceReset(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void graphicsDeviceService_DeviceResetting(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
            */
        #endregion
    }
}