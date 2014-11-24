#region Using

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using SysUtils;
using SysUtils.Utils;
using ParallelTasks;

using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.FoundationFactory;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Networking;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;

//using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Direct3D;
using SharpDX.Direct3D9;
using System.Drawing;
using SharpDX.Toolkit.Graphics;
using MinerWars.AppCode.Toolkit.Input;
using System.IO;
using MinerWars.GameServices;

#endregion

namespace MinerWars.AppCode.App
{
    public delegate void OnInitEvent(SharpDX.Toolkit.Game game);
    public delegate void OnDrawEvent(GameTime gt);

    public enum SetDepthTargetEnum
    {
        NoChange,
        RestoreDefault,
    }

    public class MyMinerGame : SharpDX.Toolkit.Game
    {
        public const string ContentDir = "Content";

        public static MyMinerGame Static;
        public static MyMwcVector2Int ScreenSize;
        public static MyMwcVector2Int ScreenSizeHalf;
        public static bool IsGameReady = true;
        public static bool IsDeviceResetted = false;

        //events for external editors/scripts
        internal static event OnInitEvent OnGameInit;
        internal static event OnDrawEvent OnGameUpdate;
        internal static event OnDrawEvent OnGameDraw;

        SharpDX.Direct3D9.Font m_debugFont;

        public string RootDirectory;
        public string RootDirectoryDebug;
        public string RootDirectoryEffects;
        public static Surface DefaultSurface { get; private set; }
        public static Surface DefaultDepth { get; private set; }

        public static ServiceContainer Services { get; private set; }

        //  Total GAME-PLAY time in milliseconds. It doesn't change while game is paused.  Use it only for game-play 
        //  stuff (e.g. game logic, particles, etc). Do not use it for GUI or not game-play stuff.
        public static int TotalGamePlayTimeInMilliseconds;
        
        //  Total time independent of whether game is paused. It increments all the time, no matter if game is paused.
        public static int TotalTimeInMilliseconds;

        //  Helpers for knowing when pauses started and total time spent in pause mode (even if there were many pauses)
        static int m_pauseStartTimeInMilliseconds;
        static int m_totalPauseTimeInMilliseconds = 0;
        static bool m_pauseActive;

        public static int NumberOfCores;

        private bool m_isGraphicsSupported;

        private static string m_gameDir;

        // Properties
        public static string GameDir
        {
            get
            {
                if (m_gameDir == null)
                {
                    m_gameDir = Services.AppDir;
                }
                return m_gameDir;
            }
        }

        protected override void EndRun()
        {
            base.EndRun();

#if DEBUG
            var o = SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();
            System.Diagnostics.Debug.Assert(o.Count == 0, "Unreleased DX objects!");
            Console.WriteLine(SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects());
#endif
        }

        
        /// <summary>
        /// Queue of actions to be invoked on xna game thread.
        /// </summary>
        private readonly ConcurrentQueue<Tuple<ManualResetEvent,Action>> m_InvokeQueue;

        /// <summary>
        /// Main xna init thread.
        /// </summary>
        private readonly Thread m_MainThread;

        #region Properties

        public static MyCustomGraphicsDeviceManagerDX GraphicsDeviceManager { get; private set; }
        
        public static bool IsGpuSupported()
        {
            foreach (int deviceId in MyMwcFinalBuildConstants.UNSUPPORTED_GPU_DEVICE_IDS)
            {
                //if (GraphicsDeviceManager.GraphicsDevice.id .Adapter.Description.DeviceId == deviceId)
                {
                  //  return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether [invoke is required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invoke required]; otherwise, <c>false</c>.
        /// </value>
        public bool InvokeRequired
        {
            get
            {
                return Thread.CurrentThread != m_MainThread;
            }
        }

        #endregion

        public MyMinerGame(ServiceContainer services)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame()::constructor");
            MyMwcLog.WriteLine("MyMinerGame.Constructor() - START");
            MyMwcLog.IncreaseIndent();

            Services = services;

            // we want check objectbuilders, prefab's configurations, gameplay constants and building specifications
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Checks");
            MyMwcObjectBuilder_Base.Check();
            MyPrefabConstants.Check();
            MyGameplayConstants.Check();
            MyBuildingSpecifications.Check();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Preallocate");

            Preallocate();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("IsAdmin");
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);
            bool IsAdmin = windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
            MyMwcLog.WriteLine("IsAdmin " + IsAdmin.ToString());
            MyMwcLog.WriteLine("Game dir: " + GameDir);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyCustomGraphicsDeviceManagerDX");
#if !DEBUG
            try
            {
#endif

            this.Exiting += MyMinerGame_Exiting;
            this.Activated += MyMinerGame_OnActivated;
            this.Deactivated += MyMinerGame_OnDeactivated;
            this.m_InvokeQueue = new ConcurrentQueue<Tuple<ManualResetEvent, Action>>();
            this.m_MainThread = Thread.CurrentThread;

            GraphicsDeviceManager = new MyCustomGraphicsDeviceManagerDX(this);

            m_isGraphicsSupported = GraphicsDeviceManager.ChangeProfileSupport();
            m_isGraphicsSupported = true;

            if (m_isGraphicsSupported)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyVideoModeManager.HookEventHandlers");

                MyVideoModeManager.HookEventHandlers();
                
                //Content = new MyCustomContentManager(Services, ContentDir);
              //  Content = new SharpDX.Toolkit.Content.ContentManager(Services);

                RootDirectory = Path.Combine(GameDir, "Content");
                RootDirectoryDebug =  Path.GetFullPath(System.IO.Path.Combine(GameDir, "..\\..\\..\\Content"));

                RootDirectoryEffects = RootDirectory;

                Static = this;

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("InitNumberOfCores");
                InitNumberOfCores();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyVideoModeManager.LogApplicationInformation");

                
                MyVideoModeManager.LogApplicationInformation();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyVideoModeManager.LogInformation");

                MyVideoModeManager.LogInformation();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyVideoModeManager.LogEnvironmentInformation");

                MyVideoModeManager.LogEnvironmentInformation();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyPlugins.LoadContent");

                MyPlugins.LoadContent();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyConfig.Load");

                MyConfig.Load();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyMath.Init");

                MyMath.Init();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyTextsWrapper.Init");

                MyTextsWrapper.Init();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyDialoguesWrapper.Init");

                MyDialoguesWrapper.Init();

                //  If I don't set TargetElapsedTime, default value will be used, which is 60 times per second, and it will be more precise than if I calculate 
                //  it like below - SO I MUST BE DOING THE WRONG CALCULATION !!!
                //  We use fixed timestep. Update() is called at this precise timesteps. If Update or Draw takes more time, Update will be called more time. Draw is called only after Update.
#if RENDER_PROFILING || GPU_PROFILING
                IsFixedTimeStep = false;
                MyMinerGame.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
#else
                IsFixedTimeStep = MyFakes.FIXED_TIMESTEP;
#endif

            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("InitMultithreading");

            InitMultithreading();
#if !DEBUG
            }
            catch (Exception ex)
            {
                //  We are catching exceptions in constructor, because if error occures here, it app will start unloading
                //  so we skip to UnloadContent and there we will get another exception (because app wasn't really loaded when unload started)
                //  So we want original exception in log.
                MyMwcLog.WriteLine(ex);
                throw;
            }
#endif

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMinerGame.Constructor() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        static void InitNumberOfCores()
        {
            //  Get number of cores of local machine. As I don't know what values it can return, I clamp it to <1..4> (min 1 core, max 4 cores). That are tested values. I can't test eight cores...
            NumberOfCores = Environment.ProcessorCount;
            MyMwcLog.WriteLine("Found processor count: " + NumberOfCores);       //  What we found
            NumberOfCores = MyMwcUtils.GetClampInt(NumberOfCores, 1, 16);
            MyMwcLog.WriteLine("Using processor count: " + NumberOfCores);       //  What are we really going use
        }

        /// <summary>
        /// Inits the multithreading.
        /// </summary>
        private void InitMultithreading()
        {
            Parallel.Scheduler = new WorkStealingScheduler(NumberOfCores);
            //Parallel.Scheduler = new SimpleScheduler(NumberOfCores);
        }

        /// <summary>
        /// Inicializes the quick launche.
        /// </summary>
        private void InitQuickLaunch()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame()::InitQuickLaunch");

            MyMwcQuickLaunchType? quickLaunch = MyMwcFinalBuildConstants.QUICK_LAUNCH;

#if AUTOBUILD
            quickLaunch = MyMwcQuickLaunchType.NEW_STORY;
#endif
            //  This will work as auto-login, but it will not depend on auto-login config variable
            //  It just always tries to auto-login. If auto-login fails, normal login screen will be displayed.
            string username = MyConfig.Username;
            string password = MyConfig.Password;

            if (quickLaunch != null)
            {                
                switch (quickLaunch.Value)
                {
                    case MyMwcQuickLaunchType.EDITOR_SANDBOX:
                        {
                            MyGuiManager.AddScreen(new MyGuiScreenLoginProgress(username, password,
                                                                new MyGuiScreenStartQuickLaunch(
                                                                    quickLaunch.Value, MyTextsWrapperEnum.StartEditorInProgressPleaseWait), null));
                        }
                        break;
                    case MyMwcQuickLaunchType.LAST_SANDBOX:
                    case MyMwcQuickLaunchType.NEW_STORY:
                        {
                            MyGuiManager.AddScreen(new MyGuiScreenLoginProgress(username, password,
                                                                new MyGuiScreenStartQuickLaunch(
                                                                    quickLaunch.Value, MyTextsWrapperEnum.StartGameInProgressPleaseWait), null));

                        }
                        break;
                    case MyMwcQuickLaunchType.LOAD_CHECKPOINT:
                        {
                            MyGuiManager.AddScreen(new MyGuiScreenLoginProgress(username, password,
                                                                new MyGuiScreenStartQuickLaunch(
                                                                    quickLaunch.Value, MyTextsWrapperEnum.StartGameInProgressPleaseWait), null));
                        }
                        break;
                    case MyMwcQuickLaunchType.SANDBOX_RANDOM:
                        {
                            MyGuiManager.AddScreen(new MyGuiScreenLoginProgress(username, password,
                                                                new MyGuiScreenStartQuickLaunch(
                                                                    quickLaunch.Value, MyTextsWrapperEnum.StartGameInProgressPleaseWait), null));
                        }
                        break;
                    default:
                        {
                            throw new MyMwcExceptionApplicationShouldNotGetHere();
                        }
                }
                
            }
            else
            {
                if (MyFakes.MWBUILDER)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenLoginProgress(username, password,
                                    new MyGuiScreenStartQuickLaunch(
                                        MyMwcQuickLaunchType.EDITOR_SANDBOX, MyTextsWrapperEnum.StartEditorInProgressPleaseWait), null));
                }
                else
                {
                    if (MyFakes.ENABLE_LOGOS)
                    {
                        MyGuiManager.BackToIntroLogos(new Action(AfterLogos));
                    }
                    else
                    {
                        AfterLogos();
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void AfterLogos()
        {
            MyGuiManager.BackToMainMenu();

            if (MySteam.IsActive)
            {
                MyGuiScreenMainMenu.AddLoginScreenDrmFree((MyGuiScreenBase)null);
            }
            else
            {
                MyGuiScreenMainMenu.AddAutologinScreen();
            }
        }

        public bool IsGraphicsSupported()
        {
            return m_isGraphicsSupported;
        }

        //  Allows the game to perform any initialization it needs to before starting to run.
        //  This is where it can query for any required services and load any non-graphic
        //  related content.  Calling base.Initialize will enumerate through any components
        //  and initialize them as well.
        protected override void Initialize()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::Initialize");
            MyMwcLog.WriteLine("MyMinerGame.Initialize() - START");
            MyMwcLog.IncreaseIndent();
                   
            Window.NativeWindow.Text = "MinerWars 2081";
            ((System.Windows.Forms.Form)Window.NativeWindow).Icon = new System.Drawing.Icon("MinerWars.ico");

            if (OnGameInit != null)
                OnGameInit(this);

            MyVideoModeManager.Initialize();

#if !DEBUG
            try
            {
#endif
                // Load data
                LoadData();

                // Load content
                base.Initialize();

                InitQuickLaunch();
                
#if !DEBUG
            }
            catch (Exception ex)
            {
                //  We are catching exceptions in LoadContent, because if error occures here, it app will start unloading
                //  so we skip to UnloadContent and there we will get another exception (because app wasn't really loaded when unload started)
                //  So we want original exception in log.
                MyMwcLog.WriteLine(ex);
                throw;
            }
#endif
            
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMinerGame.Initialize() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UpdateScreenSize()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::UpdateScreenSize");

            ScreenSize = new MyMwcVector2Int((int)MyMinerGame.Static.GraphicsDevice.Viewport.Width, (int)MyMinerGame.Static.GraphicsDevice.Viewport.Height);
            ScreenSizeHalf = new MyMwcVector2Int(ScreenSize.X / 2, ScreenSize.Y / 2);

            if (MyGuiManager.GetScreenshot() != null)
            {
                ScreenSize.X = (int)(ScreenSize.X * MyGuiManager.GetScreenshot().SizeMultiplier);
                ScreenSize.Y = (int)(ScreenSize.Y * MyGuiManager.GetScreenshot().SizeMultiplier);
                ScreenSizeHalf = new MyMwcVector2Int(ScreenSize.X / 2, ScreenSize.Y / 2);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Decrease fragmentation of the Large Object Heap by forcing static class constructors to run.
        /// </summary>
        private static void Preallocate()
        {
            Type[] typesToForceStaticCtor =
            {
                typeof(MyEntities),
                typeof(MyMwcObjectBuilder_Base),
                typeof(MyPrefabConstants),
                typeof(MyGameplayConstants),
                typeof(MyBuildingSpecifications),
                typeof(MyVoxelCacheCellRenderHelper),
                typeof(MyTransparentGeometry),
                typeof(MyExplosionDebrisModel),
                typeof(Physics.MyContactInfoCache),
                typeof(Physics.MyTriangleCache),
                typeof(Physics.MyContactConstraintModule),
                typeof(Physics.MySensorModule),
                typeof(Physics.MySensorInteractionModule),
                typeof(Physics.MyRBInteractionModule),
                typeof(Physics.MyRigidBodyModule),
            };
            foreach (var type in typesToForceStaticCtor)
            {
                // this won't call the static ctor if it was already called
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData()
        {
            int block1 = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::LoadData", ref block1);
            MyMwcLog.WriteLine("MyMinerGame.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyPerformanceTimer.LoadData");
            MyPerformanceTimer.LoadData();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyVoxelFiles.LoadData");
            MyVoxelFiles.LoadData();            
			// Not using now
            //MyClientServer.LoadData();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyAudio.LoadData");
            MyAudio.LoadData();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyGuiManager.LoadData");
            MyGuiManager.LoadData();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyGameplayCheats.LoadData");
            MyGameplayCheats.LoadData();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMinerGame.LoadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(block1);
        }


        /// <summary>
        /// Unloads the data.
        /// </summary>
        private void UnloadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::UnloadData");
            MyMwcLog.WriteLine("MyMinerGame.UnloadData() - START");
            MyMwcLog.IncreaseIndent();

            // TODO: Unload data probably not necessery because all data are loaded at start and dies at the end. No partial unload.
            
            //  We must unload XACT sounds here, not in the background thread, because on Windows XP every XACT sound loaded in
            //  not-main thread is then not player (I can't hear it).
            MyAudio.UnloadData();

            MyGameplayCheats.UnloadData();

			// Not using now
            //MyClientServer.UnloadData();
            MyPerformanceTimer.UnloadData();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMinerGame.UnloadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Load graphics resources content.
        /// </summary>
        public override void LoadContent()
        {
            

            int block1 = -1;
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::LoadContent", ref block1);
            base.LoadContent();
                                  
            DefaultSurface = MyMinerGame.Static.GraphicsDevice.GetRenderTarget(0);
            DefaultSurface.DebugName = "DefaultSurface";
            DefaultDepth = MyMinerGame.Static.GraphicsDevice.DepthStencilSurface;
            DefaultDepth.DebugName = "DefaultDepth";
                                    
            MyMwcLog.WriteLine("MyMinerGame.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            
            MyVideoModeManager.UpdateScreenSize();

            System.Drawing.Font systemfont = new System.Drawing.Font("Tahoma", 12f, FontStyle.Regular);
            m_debugFont = new SharpDX.Direct3D9.Font(GraphicsDevice, systemfont);

            // GUI
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiManager.LoadContent()"); 
            MyGuiManager.LoadContent();

            // Models
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyModels.LoadContent()");
            MyModels.LoadContent();

            // Render
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyRender.LoadContent();");
            MyRender.LoadContent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyDebugDraw.LoadContent();");
            MyDebugDraw.LoadContent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyDebugDrawCachedLines.LoadContent()");
            MyDebugDrawCachedLines.LoadContent();

              
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MySunGlare.LoadContent()");
            MySunGlare.LoadContent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
      
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMinerGame.LoadContent() - END");
                  
            GC.Collect();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(block1);
        }

        /// <summary>
        /// Called when graphics resources need to be unloaded. Override this method to unload any game-specific graphics resources.
        /// </summary>
        public override void UnloadContent()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::UnloadContent");
            MyMwcLog.WriteLine("MyMinerGame.UnloadContent() - START");
            MyMwcLog.IncreaseIndent();

            if (m_debugFont != null)
            {
                m_debugFont.Dispose();
                m_debugFont = null;
            }
                     
            if (DefaultSurface != null)
            {
                DefaultSurface.Dispose();
                DefaultSurface = null;
            }
            if (DefaultDepth != null)
            {
                DefaultDepth.Dispose();
                DefaultDepth = null;
            }      

            // GUI
            MyGuiManager.UnloadContent();

            MyRender.UnloadContent();

            MyTextureManager.UnloadContent();
            MyModels.UnloadContent();

            // Global content
            //Content.Unload();

            // Render
            MySunGlare.UnloadContent();

            MyDebugDrawCachedLines.UnloadContent();
            MyDebugDraw.UnloadContent();

            //MyRender.UnloadContent(); //it is unloaded in gui gameplay screen
            
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMinerGame.UnloadContent() - END");

            GraphicsDevice.SetStreamSource(0, null, 0, 0);
            GraphicsDevice.Indices = null;
            GraphicsDevice.VertexDeclaration = null;
            GraphicsDevice.PixelShader = null;
            GraphicsDevice.VertexShader = null;
            for (int i = 0; i < 16; i++)
            {
                GraphicsDevice.SetTexture(i, null);
            }

            base.UnloadContent();

            GC.Collect();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }
     
        static void UpdateTimes(GameTime gameTime)
        {
            if (!IsPaused())
            {
                TotalGamePlayTimeInMilliseconds = ((int) gameTime.TotalGameTime.TotalMilliseconds) - m_totalPauseTimeInMilliseconds;
            }
            TotalTimeInMilliseconds = (int)gameTime.TotalGameTime.TotalMilliseconds;
        }

        //  Switch pause on or off, depending on what mode is now
        public static void SwitchPause()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::SwitchPause");
            if (m_pauseActive)
            {
                //  Going from PAUSED game to non-paused game
                m_totalPauseTimeInMilliseconds += TotalTimeInMilliseconds - m_pauseStartTimeInMilliseconds;
                m_pauseActive = false;
                MyAudio.Resume();
            }
            else
            {
                //  Going from non-paused game to PAUSED game
                m_pauseStartTimeInMilliseconds = TotalTimeInMilliseconds;
                m_pauseActive = true;
                MyAudio.Pause();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

         //  Set pause on/off
        public static void SetPause(bool set)
        {
            m_pauseActive = set; 
        }

        //  True if pause is active right now (game-play is paused)
        public static bool IsPaused()
        {
            return m_pauseActive && MyMultiplayerGameplay.CanPauseGame;
        }
        
        //  Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            if (m_debugFont == null)
                return;

            // Apply video mode changes.
            MyVideoModeManager.ApplyChanges();

            //  Update times in static member variables
            UpdateTimes(gameTime);         

            //if (MyMinerGame.IsDeviceResetted)
              //  MyVideoModeManager.UpdateAfterDeviceReset();
                
            MyRender.GetRenderProfiler().StartProfilingBlock("Particles wait");
            MyParticlesManager.WaitUntilUpdateCompleted();
            MyRender.GetRenderProfiler().EndProfilingBlock();

            MyRender.GetRenderProfiler().StartProfilingBlock("Receive Multiplayer Messages");
            MyMultiplayerPeers.Static.Update();
            MyRender.GetRenderProfiler().EndProfilingBlock();
                  
            int updateBlock = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("Update", ref updateBlock);

            if (MyMwcFinalBuildConstants.EnableLoggingInDrawAndUpdateAndGuiLoops == true)
            {
                MyMwcLog.WriteLine("MyMinerGame.Update() - START");
                MyMwcLog.IncreaseIndent();
                MyMwcLog.WriteLine("Update - gameTime.ElapsedGameTime: " + gameTime.ElapsedGameTime.ToString());
                MyMwcLog.WriteLine("Update - gameTime.TotalGameTime: " + gameTime.TotalGameTime.ToString());

                MyMwcLog.WriteLine("Max Garbage Generation: " + GC.MaxGeneration.ToString());
                for (int i = 0; i <= GC.MaxGeneration; ++i)
                {
                    MyMwcLog.WriteLine("Generation " + i.ToString() + ": " + GC.CollectionCount(i).ToString() +
                                       " collections");
                }
                MyMwcLog.WriteLine("Total Memory: " + MyValueFormatter.GetFormatedLong(GC.GetTotalMemory(false)) +
                                   " bytes");
            }

            //  Inform us if there were some garbage collection
            if (MyMwcFinalBuildConstants.EnableLoggingGarbageCollectionCalls)
            {
                int newGc = MyGarbageCollectionManager.GetGarbageCollectionsCountFromLastCall();
                if (newGc > 0)
                {
                    MyMwcLog.WriteLine("####### Garbage collections from the last call: " + newGc + " #######");
                }
            }


            int updateManagersBlock = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("UpdateManagers", ref updateManagersBlock);

            MyRender.GetRenderProfiler().EndProfilingBlock(updateManagersBlock);

            //  Now I think that it's better if HandleInput is called after Update, because then input methods
            //  such as Shot() have up-to-date values such as position, forward vector, etc
            int guiManagerBlock = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("GuiManager", ref guiManagerBlock);
            MyGuiManager.Update();
            MyRender.GetRenderProfiler().EndProfilingBlock(guiManagerBlock);

            //After guimanager update because of object world matrices updates of objects
            MyParticlesManager.Update();
              
            int inputBlock = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("Input", ref inputBlock);
            MyGuiManager.HandleInput();
            MyRender.GetRenderProfiler().EndProfilingBlock(inputBlock);

            int serverUpdateBlock = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("MyClientServer.Update", ref serverUpdateBlock);
            //MyClientServer.Update();       ti
            MyRender.GetRenderProfiler().EndProfilingBlock(serverUpdateBlock);
                
            if (MyMwcFinalBuildConstants.SimulateSlowUpdate)
            {
                System.Threading.Thread.Sleep(7);
            }
                  
            int audioUpdateBlock = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("MyAudio.Update", ref audioUpdateBlock);
            MyAudio.Update();
            MyDialogues.Update();
            MyRender.GetRenderProfiler().EndProfilingBlock(audioUpdateBlock);
                           
            int othersBlock = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("Others", ref othersBlock);
            
            if (MyMwcFinalBuildConstants.EnableLoggingInDrawAndUpdateAndGuiLoops == true)
            {
                if (MyMwcLog.IsIndentKeyIncreased())
                {
                    MyMwcLog.DecreaseIndent();
                }
                MyMwcLog.WriteLine("MyMinerGame.Update() - END");
            }
                          
			ProcessInvoke();

            if (OnGameUpdate != null)
                OnGameUpdate(gameTime);
                    
            base.Update(gameTime);

            MyRender.GetRenderProfiler().EndProfilingBlock(othersBlock);

            MyRender.GetRenderProfiler().EndProfilingBlock(updateBlock);
        }


        public static int ResetSleep = 0;

        protected override void Draw(GameTime gameTime)
        {
            if (m_debugFont == null)
                return;

//            MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::Draw");

           // if (MyMinerGame.IsDeviceResetted)
             //   return;
                    /*
            if (ResetSleep > 0)
            {
                ResetSleep--;
                GraphicsDevice.Clear(Color.Black);
                if (ResetSleep == 0)
                {
                    MyMwcLog.WriteLine("Reset sleep - START");
                    MyTextureManager.ReloadTextures(false);
                    MyGuiManager.UpdateAfterDeviceReset();
                    MyMwcLog.WriteLine("Reset sleep - END");
                }
                return;
            }     */    
                      
            if (MyMwcFinalBuildConstants.EnableLoggingInDrawAndUpdateAndGuiLoops)
            {
                MyMwcLog.WriteLine("MyMinerGame.Draw() - START");
                MyMwcLog.IncreaseIndent();
                MyMwcLog.WriteLine("Draw - gameTime.ElapsedGameTime: " + gameTime.ElapsedGameTime);
                MyMwcLog.WriteLine("Draw - gameTime.TotalGameTime: " + gameTime.TotalGameTime);
            }       

            UpdateTimes(gameTime);
            MyFpsManager.Update();

            GraphicsDevice.Clear(ClearFlags.Target, new ColorBGRA(0.0f), 1, 0);

            if (GraphicsDevice.IsDisposed )
            {
                //MyMwcLog.WriteLine("MyMinerGame.Draw() - MyFakes.ALLOW_RENDER_HACK == false... skipping Draw");
                //MyRender.GetRenderProfiler().EndProfilingBlock();
                return;
            } 
                 /*
            //  This hack is for PerfHUD. It will allow us to see correct PerfHUD GUI (not corrupted by some error), so we will see colors on graph menu.
            if (MyMwcFinalBuildConstants.ENABLE_PERFHUD == true)
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }  */
                   
            if (MyMwcFinalBuildConstants.SimulateSlowDraw == true)
            {
                System.Threading.Thread.Sleep(60);
            }    

          //  MyRender.ResetStates();

            MyGuiManager.Draw(); 
                
            //  This hack is for PerfHUD. It will allow us to see correct PerfHUD GUI (not corrupted by some error), so we will see colors on graph menu.
            /*if (MyMwcFinalBuildConstants.ENABLE_PERFHUD == true)
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.None;
            }      */


            DepthStencilState.None.Apply();
            RasterizerState.CullNone.Apply();
            BlendState.Opaque.Apply();

            //m_debugFont.DrawText(null, MyFpsManager.GetFps().ToString() + " (" + MyFpsManager.FrameTimeAvg.ToString() + "ms) ", 0, 0, new ColorBGRA(1.0f, 1.0f, 1.0f, 1.0f));


            if (MyMwcFinalBuildConstants.EnableLoggingInDrawAndUpdateAndGuiLoops == true)
            {
                MyMwcLog.DecreaseIndent();
                MyMwcLog.WriteLine("MyMinerGame.Draw() - END");
            }   

            base.Draw(gameTime);

            if (OnGameDraw != null)
                OnGameDraw(gameTime);  
        }

        protected override bool BeginDraw()
        {
            MyRender.GetRenderProfiler().StartProfilingBlock("BeginDraw");
            bool ret = base.BeginDraw();
            MyRender.GetRenderProfiler().EndProfilingBlock();
            return ret;
        }

        protected override void EndDraw()
        {
            MyRender.GetRenderProfiler().StartProfilingBlock("EndDraw");
            base.EndDraw();
            MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        /// <summary>
        /// Invokes the specified action on main xna game thread and block until it is completed.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Invoke(Action action, bool waitForInvoke)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::Invoke");

            var waiter = new ManualResetEvent(false);

            this.m_InvokeQueue.Enqueue(new Tuple<ManualResetEvent, Action>(waiter, action));

            if (waitForInvoke)
                waiter.WaitOne();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Processes the invoke queue.
        /// </summary>
        private void ProcessInvoke()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::ProcessInvoke");
            Tuple<ManualResetEvent, Action> result;
            while (this.m_InvokeQueue.TryDequeue(out result))
            {
                result.Item2();
                result.Item1.Set();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        static void MyMinerGame_OnDeactivated(object sender, EventArgs e)
        {
            MyMwcLog.WriteLine("MyMinerGame.MyMinerGame_OnDeactivated");
        }

        static void MyMinerGame_OnActivated(object sender, EventArgs e)
        {
            MyMwcLog.WriteLine("MyMinerGame.MyMinerGame_OnActivated");
        }

        //  We are unloading the game here (in this event), because when player presses Alt+F4 during loading phase, sometimes
        //  graphic device is disposed during we are unloading - with is weird. But this event finishes always before even
        //  graphic device disposing starts, so it's safe.
        //  Add any code that must execute before the game ends (e.g. when player suddenly presses Alt+F4)
        void MyMinerGame_Exiting(object sender, EventArgs e)
        {
            MyMwcLog.WriteLine("MyMinerGame.MyMinerGame_Exiting");
            MySectorServiceClient.ClearAndClose(); // In case of exception on different thread
            IsMouseVisible = true;
            UnloadData();

//            MinerWars.AppCode.Game.Render.MyOcclusionQuery.UnloadAllQueries();
        }

        static int m_renderTargetsCount = 0;

        public void SetDeviceViewport(Viewport viewport)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMinerGame::SetDeviceViewport");
            if (m_renderTargetsCount == 0)
            {
                if ((MyMinerGame.Static.GraphicsManager.PreferredBackBufferHeight >= (viewport.Height + viewport.Y))
                    && (MyMinerGame.Static.GraphicsManager.PreferredBackBufferWidth >= (viewport.Width + viewport.X)))
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("set viewport");
                    MyMinerGame.Static.GraphicsDevice.Viewport = viewport;
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                else
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("change screen size");
                    MyVideoModeManager.UpdateScreenSize();
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
            }
            else  
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("set viewport");
                MyMinerGame.Static.GraphicsDevice.Viewport = viewport;
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                
        }


        private static void RestoreDefaultTargets()
        {
            MyMinerGame.Static.GraphicsDevice.SetRenderTarget(0, DefaultSurface);
            MyMinerGame.Static.GraphicsDevice.SetRenderTarget(1, null);
            MyMinerGame.Static.GraphicsDevice.SetRenderTarget(2, null);
            MyMinerGame.Static.GraphicsDevice.DepthStencilSurface = DefaultDepth;
            m_renderTargetsCount = 0;
        }

        public static void SetRenderTarget(Texture rt, Texture depth, SetDepthTargetEnum depthOp = SetDepthTargetEnum.NoChange)
        {
            if (rt == null)
            {
                RestoreDefaultTargets();
            }
            else
            {
                Surface surface = rt.GetSurfaceLevel(0);
                MyMinerGame.Static.GraphicsDevice.SetRenderTarget(0, surface);
                surface.Dispose();
                MyMinerGame.Static.GraphicsDevice.SetRenderTarget(1, null);
                MyMinerGame.Static.GraphicsDevice.SetRenderTarget(2, null);
                if (depth != null)
                {
                    Surface dsurface = depth.GetSurfaceLevel(0);
                    MyMinerGame.Static.GraphicsDevice.DepthStencilSurface = dsurface;
                    dsurface.Dispose();
                }
                else if (depthOp == SetDepthTargetEnum.RestoreDefault)
                {
                    MyMinerGame.Static.GraphicsDevice.DepthStencilSurface = DefaultDepth;
                }

                m_renderTargetsCount = 1;
            }
        }

        public static void SetRenderTargets(Texture[] rts, Texture depth, SetDepthTargetEnum depthOp = SetDepthTargetEnum.NoChange)
        {
            if (rts == null)
            {
                RestoreDefaultTargets();
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i < rts.Length)
                    {
                        Surface surface = rts[i].GetSurfaceLevel(0);
                        MyMinerGame.Static.GraphicsDevice.SetRenderTarget(i, surface);
                        surface.Dispose();
                    }
                    else
                        MyMinerGame.Static.GraphicsDevice.SetRenderTarget(i, null);
                }
                if (depth != null)
                {
                    Surface surface = depth.GetSurfaceLevel(0);
                    MyMinerGame.Static.GraphicsDevice.DepthStencilSurface = surface;
                    surface.Dispose();
                }
                else if (depthOp == SetDepthTargetEnum.RestoreDefault)
                {
                    MyMinerGame.Static.GraphicsDevice.DepthStencilSurface = DefaultDepth;
                }
                    
                m_renderTargetsCount = rts.Length;
            }
        }                                    

        public static bool IsMainThread()
        {
            return Thread.CurrentThread == Static.m_MainThread;
        }
    }
}
