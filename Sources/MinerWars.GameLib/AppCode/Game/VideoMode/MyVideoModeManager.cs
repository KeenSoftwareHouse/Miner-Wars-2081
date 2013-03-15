using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
//using System.Threading;
using KeenSoftwareHouse.Library.Debugging;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using SysUtils;
using SysUtils.Utils;

//using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


namespace MinerWars.AppCode.Game.VideoMode
{
    /// <summary>
    /// 
    /// </summary>
    static class MyVideoModeManager
    {
        /// <summary>
        /// Represent asynchronous change mode operation.
        /// </summary>
        public class MyVideoModeChangeOperation
        {
            #region Fields

            private bool callApplyChanges;
            private bool fullScreen;
            private bool verticalSync;
            private bool hardwareCursor;
            private float fieldOfView;
            private int videoAdapter;
            private MyVideoModeEx videoMode;
            private MyRenderQualityEnum renderQuality;
            private readonly bool first;

            /// <summary>
            /// Result of operation.
            /// </summary>
            public bool Changed { get; private set; }

            public bool SomethingChanged { get; private set; }




            /// <summary>
            /// Callback.
            /// </summary>
            private Action<MyVideoModeChangeOperation> requestCallback;

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public MyVideoModeChangeOperation(bool callApplyChanges, bool fullScreen, bool verticalSync, bool hardwareCursor,
                                              float fieldOfView,
                                                int videoAdapter, MyVideoModeEx videoMode,
                                              MyRenderQualityEnum renderQuality, bool first, Action<MyVideoModeChangeOperation> requestCallback)
            {
                this.callApplyChanges = callApplyChanges;
                this.fullScreen = fullScreen;
                this.verticalSync = verticalSync;
                this.hardwareCursor = hardwareCursor;
                this.fieldOfView = fieldOfView;
                this.videoAdapter = videoAdapter;
                this.videoMode = videoMode;
                this.renderQuality = renderQuality;
                this.first = first;
                this.requestCallback = requestCallback;
            }

            /// <summary>
            /// Processes this operation.
            /// </summary>
            internal void Process()
            {
                MyMwcLog.WriteLine("MyVideoModeManager.ChangeVideoMode - START");
                MyMwcLog.IncreaseIndent();

                bool modeChange = !videoMode.Equals(m_videoMode) || m_videoAdapter != videoAdapter || m_fullScreen != fullScreen ||
                                  m_verticalSync != verticalSync,
                     qualityChange = MyRenderConstants.RenderQualityProfile.RenderQuality != renderQuality,
                     fovChange = m_fieldOfView != fieldOfView,
                     hardwareCursorChange = m_hardwareCursor != hardwareCursor;

                SomethingChanged = modeChange || qualityChange || fovChange || hardwareCursorChange;

                bool needReloadContent = false;

                m_fullScreen = fullScreen;
                m_videoMode = videoMode;
                m_videoAdapter = videoAdapter;
                m_verticalSync = verticalSync;
                m_hardwareCursor = hardwareCursor;
                m_fieldOfView = fieldOfView;

                //  User provided values
                MyMwcLog.WriteLine("Width: " + videoMode.Width);
                MyMwcLog.WriteLine("Height: " + videoMode.Height);
                MyMwcLog.WriteLine("FullScreen: " + m_fullScreen);
                MyMwcLog.WriteLine("VerticalSync: " + m_verticalSync);
                MyMwcLog.WriteLine("HardwareCursor: " + m_hardwareCursor);
                MyMwcLog.WriteLine("RenderQuality: " + (int)renderQuality);
                                                 /*
                foreach (GraphicsAdapter adapter in GraphicsAdapter.Adapters)
                {
                    MyMwcLog.WriteLine("adapter.Description: " + adapter.Description.Description);
                    MyMwcLog.WriteLine("adapter.VendorId: " + adapter.Description.VendorId);
                    MyMwcLog.WriteLine("adapter.DeviceId: " + adapter.Description.DeviceId);
                    MyMwcLog.WriteLine("adapter.DeviceName: " + adapter.Name);
                    MyMwcLog.WriteLine("adapter.IsDefaultAdapter: " + adapter.IsDefaultAdapter);
                    //MyMwcLog.WriteLine("adapter.IsWideScreen: " + adapter.Description.);
                    MyMwcLog.WriteLine("adapter.Revision: " + adapter.Description.Revision);
                    MyMwcLog.WriteLine("adapter.SubSystemId: " + adapter.Description.SubsystemId);
                    MyMwcLog.WriteLine("adapter.CurrentDisplayMode.Width: " + adapter.CurrentDisplayMode.Width);
                    MyMwcLog.WriteLine("adapter.CurrentDisplayMode.Height: " + adapter.CurrentDisplayMode.Height);
                    MyMwcLog.WriteLine("adapter.CurrentDisplayMode.AspectRatio: " +
                                       adapter.CurrentDisplayMode.AspectRatio);
                    MyMwcLog.WriteLine("adapter.CurrentDisplayMode.Format: " + adapter.CurrentDisplayMode.Format);
                }
                                       */
                //  System values
                /*
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.Description: " +
                                   GraphicsAdapter.Default.Description.Description);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.VendorId: " + GraphicsAdapter.Default.Description.VendorId);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.DeviceId: " + GraphicsAdapter.Default.Description.DeviceId);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.DeviceName: " +
                                   GraphicsAdapter.Default.Name);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.IsDefaultAdapter: " +
                                   GraphicsAdapter.Default.IsDefaultAdapter);
//                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.IsWideScreen: " +
  //                                 GraphicsAdapter.Default.IsWideScreen);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.Revision: " + GraphicsAdapter.Default.Description.Revision);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.SubSystemId: " +
                                   GraphicsAdapter.Default.Description.SubsystemId);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width: " +
                                   GraphicsAdapter.Default.CurrentDisplayMode.Width);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height: " +
                                   GraphicsAdapter.Default.CurrentDisplayMode.Height);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio: " +
                                   GraphicsAdapter.Default.CurrentDisplayMode.AspectRatio);
                MyMwcLog.WriteLine("GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format: " +
                                   GraphicsAdapter.Default.CurrentDisplayMode.Format);
                MyMwcLog.WriteLine("PreferredBackBufferFormat: " +
                                   MyMinerGameDX.GraphicsDeviceManager.PreferredBackBufferFormat);
                MyMwcLog.WriteLine("PreferredDepthStencilFormat: " +
                                   MyMinerGameDX.GraphicsDeviceManager.PreferredDepthStencilFormat);
                MyMwcLog.WriteLine("PreferredBackBufferWidth: " +
                                   MyMinerGameDX.GraphicsDeviceManager.PreferredBackBufferWidth);
                MyMwcLog.WriteLine("PreferredBackBufferHeight: " +
                                   MyMinerGameDX.GraphicsDeviceManager.PreferredBackBufferHeight);
                MyMwcLog.WriteLine("PreferMultiSampling: " + MyMinerGameDX.GraphicsDeviceManager.PreferMultiSampling);
                MyMwcLog.WriteLine("SynchronizeWithVerticalRetrace: " +
                                   MyMinerGameDX.GraphicsDeviceManager.SynchronizeWithVerticalRetrace);
//                MyMwcLog.WriteLine("GraphicsProfile: " + MyMinerGameDX.GraphicsDeviceManager.fea);
                                */
                Changed = false;

                if (IsSupportedDisplayMode(videoAdapter, videoMode.Width, videoMode.Height, m_fullScreen) == true)
                {
                    // The mode is supported, so set the buffer formats, apply changes and return
                    //MyMinerGameDX.GraphicsDeviceManager.PreferMultiSampling = false;
                    MyMinerGame.GraphicsDeviceManager.PreferredVideoAdapter = videoAdapter;

                    MyMinerGame.GraphicsDeviceManager.PreferredBackBufferWidth = videoMode.Width;
                    MyMinerGame.GraphicsDeviceManager.PreferredBackBufferHeight = videoMode.Height;
#if RENDER_PROFILING || GPU_PROFILING
                    MyMinerGame.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
#else
                    MyMinerGame.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = m_verticalSync;
#endif
                    MyMinerGame.GraphicsDeviceManager.IsFullScreen = m_fullScreen;
                    MyMinerGame.GraphicsDeviceManager.PreferredDepthStencilFormat = Format.D24S8;
                        //  We need stencil in backbuffer because 
                    Changed = true;
                }

                try
                {

                    if (callApplyChanges)
                    {
                        if (modeChange)
                        {
                            //  ApplyChanges() should be called only if we change screen resolution during game. It shouldn't be called when we
                            //  set up resolution at game start up (because Game class Initialize is handling that for us and if we also do it - it can
                            //  have bad consequences on dual monitor setup).
                            MyMinerGame.GraphicsDeviceManager.ApplyChanges();
                            //MyRender.ResetStates();
                        }

                        if (qualityChange)
                        {
                            needReloadContent = MyRenderConstants.RenderQualityProfile.NeedReloadContent;
                            MyRenderConstants.SwitchRenderQuality(renderQuality);
                            needReloadContent |= MyRenderConstants.RenderQualityProfile.NeedReloadContent;
                        }

                        if (fovChange)
                        {
                            if (MinerWars.CommonLIB.AppCode.Utils.MyMwcUtils.IsZero(fieldOfView - MinerWarsMath.MathHelper.ToRadians(70)))
                            { //replace old default value
                                fieldOfView = MinerWarsMath.MathHelper.ToRadians(60);
                            }

                            MyCamera.FieldOfView = fieldOfView;

                            if (MyCamera.Zoom != null)
                            {
                                MyCamera.Zoom.Update();
                            }
                            MyCamera.ChangeFov(MyCamera.FieldOfView);
                        }

                       // if (hardwareCursorChange)
                        {
                            // do whatever is necessary to switch cursor between hardware/software
                            MyMinerGame.Static.IsMouseVisible = m_hardwareCursor;
                            MinerWars.AppCode.Game.GUI.Core.MyGuiManager.SetMouseCursorVisibility(m_hardwareCursor);
                        }

                        if (needReloadContent && MyMinerGame.Static.IsActive && !first)
                        {
                            //MyMinerGame.Static.ReloadContent();
                            MyRender.LoadContent();
                            MyModels.ReloadContent();
                            MinerWars.AppCode.Game.Voxels.MyVoxelMaterials.ReloadContent();
                            MyTextureManager.ReloadTextures();
                        }
                    }

                    if (Changed)
                    {
                        UpdateScreenSize();
                    }
                }
                catch (Exception e)
                {
                    Debug.Fail("Failed to change resolution");
                    MyMwcLog.WriteLine("Failed to update screen size: " + e.ToString());
                    Changed = false;
                }

                if (!Changed || needReloadContent)
                {
                    // There was error when changing resolution, so we try to restore previous state - so recreate render targets with old size
                    try
                    {
                        // reset render targets, or it will crash in next draw, before reverting the settings
                      //  MyRender.CreateRenderTargets();
                      //  MyRender.CreateEnvironmentMapsRT(MyRenderConstants.ENVIRONMENT_MAP_SIZE);
                    }
                    catch(Exception)
                    {
                        Debug.Fail("Failed to reload content");
                        Changed = false;
                        MyMwcLog.WriteLine("Failed to reload content");
                    }
                }

                if (this.requestCallback != null)
                {
                    this.requestCallback(this);
                }

                MyMinerGame.IsGameReady = true;

                MyMwcLog.DecreaseIndent();
                MyMwcLog.WriteLine("MyVideoModeManager.ChangeVideoMode - END");
            }

            #endregion
        }

        //  Here we store width/height of windows desktop, because later it can change (if we switch to fullscreen) and we want to
        //  have these original values.
        static int m_originalWindowsDesktopWidth;
        static int m_originalWindowsDesktopHeight;

        //  These are actual settings
        //static MyAntiAliasingEnum m_antiAliasing; Removed in deferred render
        static bool m_verticalSync;
        static bool m_fullScreen;
        static bool m_hardwareCursor;
        static MyVideoModeEx m_videoMode;
        static int m_videoAdapter;
        static float m_fieldOfView;

        //  These are releted in storing and book-keeping system supported video modes
        static Dictionary<int, List<MyVideoModeEx>> m_videoModeList;
        static Dictionary<int, Dictionary<int, Dictionary<int, MyVideoModeEx>>> m_resolutionMap;
        static Dictionary<int, Dictionary<float, SortedDictionary<int, MyVideoModeEx>>> m_aspectRatioMap;
        static Dictionary<int, MyAspectRatioEx> m_recommendedAspectRatio;


        private static Queue<MyVideoModeChangeOperation> m_changeOperations;

        public static readonly MyVideoModeEx DEFAULT_FALL_BACK_4_3_800_600 = new MyVideoModeEx(800, 600, 800f / 600f);


        //  Changed from static constructor to LoadContent because I want to make sure it is called at the right time,
        //  therefore when GraphicsAdapter
        public static void Initialize()
        {
            m_videoModeList = new Dictionary<int, List<MyVideoModeEx>>();
            m_resolutionMap = new Dictionary<int, Dictionary<int, Dictionary<int, MyVideoModeEx>>>();
            m_aspectRatioMap = new Dictionary<int, Dictionary<float, SortedDictionary<int, MyVideoModeEx>>>();
            m_changeOperations = new Queue<MyVideoModeChangeOperation>();
            m_recommendedAspectRatio = new Dictionary<int, MyAspectRatioEx>();

            MyMinerGame.GraphicsDeviceManager.DeviceCreated += GraphicsDevice_DeviceCreated;
            MyRenderConstants.OnRenderQualityChange += new EventHandler(MyRenderConstants_OnRenderQualityChange);

            UpdateVideoModes();

            MyVideoModeManager.InitFromConfig();
        }

        static void UpdateVideoModes()
        {
            m_resolutionMap.Clear();
            m_videoModeList.Clear(); 
            m_aspectRatioMap.Clear();
            m_recommendedAspectRatio.Clear();

            for (int adapterIndex = 0; adapterIndex < GraphicsAdapter.Adapters.Length; adapterIndex++)
            {
                GraphicsAdapter adapter = GraphicsAdapter.Adapters[adapterIndex];

                m_recommendedAspectRatio.Add(adapterIndex, MyAspectRatioExList.Get(MyAspectRatioExList.GetWindowsDesktopClosestAspectRatio(adapterIndex)));

                foreach (SharpDX.Direct3D9.DisplayMode dm in adapter.SupportedDisplayModes)
                {
                    RegisterVideoMode(adapterIndex, dm.Width, dm.Height, dm.AspectRatio);
                }

                if (MyMwcFinalBuildConstants.IS_DEVELOP)
                {
                    RegisterVideoMode(adapterIndex, 1600, 600);   // for testing windowed mode dual/triple screens
                    RegisterVideoMode(adapterIndex, 1920, 480);   // for testing windowed mode dual/triple screens
                    //RegisterVideoMode(4000, 2728);  // for making large-screen screenshots on our A3 printer (it needs to be exactly this aspect ratio, otherwise it won't fit on paper)
                }
            }
        }

        static void MyRenderConstants_OnRenderQualityChange(object sender, EventArgs e)
        {
            MyTextureManager.Quality = MyRenderConstants.RenderQualityProfile.TextureQuality;
            //graphicsManager.ReloadResources(); // Skipped, this will be done in MyMinerGame.LoadContent();
        }

        static void RegisterVideoMode(int adapterIndex, int width, int height)
        {
            RegisterVideoMode(adapterIndex, width, height, (float)width / (float)height);
        }

        static void RegisterVideoMode(int adapterIndex, int width, int height, float ratio)
        {
            if ((width > MyMinerGame.GraphicsDeviceManager.MaxTextureSize) || (height > MyMinerGame.GraphicsDeviceManager.MaxTextureSize))
            {
                MyMwcLog.WriteLine("VideoMode " + width.ToString() + " x " + height.ToString() + " requires texture size which is not supported by this HW (this HW supports max " + MyMinerGame.GraphicsDeviceManager.MaxTextureSize.ToString() + ")");
            }

            MyVideoModeEx newVideoMode = null;

            if (!m_resolutionMap.ContainsKey(adapterIndex))
                m_resolutionMap.Add(adapterIndex, new Dictionary<int, Dictionary<int, MyVideoModeEx>>());

            if (!m_videoModeList.ContainsKey(adapterIndex))
                m_videoModeList.Add(adapterIndex, new List<MyVideoModeEx>());

            if (!m_aspectRatioMap.ContainsKey(adapterIndex))
                m_aspectRatioMap.Add(adapterIndex, new Dictionary<float, SortedDictionary<int, MyVideoModeEx>>());


            if (m_resolutionMap[adapterIndex].ContainsKey(width) == false)
            {
                m_resolutionMap[adapterIndex][width] = new Dictionary<int, MyVideoModeEx>();
                newVideoMode = new MyVideoModeEx(width, height, ratio);
            }
            else if (m_resolutionMap[adapterIndex][width].ContainsKey(height) == false)
            {
                newVideoMode = new MyVideoModeEx(width, height, ratio);
            }

            if (newVideoMode != null)  // So there is a new video mode added, add it to maps and populate related fields
            {
                newVideoMode.IsRecommended = newVideoMode.AspectRatioEnum == m_recommendedAspectRatio[adapterIndex].AspectRatioEnum;
                m_resolutionMap[adapterIndex][width][height] = newVideoMode;
                m_videoModeList[adapterIndex].Add(newVideoMode);

                if (m_aspectRatioMap[adapterIndex].ContainsKey(newVideoMode.AspectRatio) == false)
                {
                    m_aspectRatioMap[adapterIndex][newVideoMode.AspectRatio] = new SortedDictionary<int, MyVideoModeEx>();
                    m_aspectRatioMap[adapterIndex][newVideoMode.AspectRatio].Add(newVideoMode.Width, newVideoMode);
                }
                else if (m_aspectRatioMap[adapterIndex][newVideoMode.AspectRatio].ContainsKey(width) == false)
                {
                    m_aspectRatioMap[adapterIndex][newVideoMode.AspectRatio].Add(newVideoMode.Width, newVideoMode);
                }
            }
        }

        public static MyAspectRatioEx GetRecommendedAspectRatio(int adapterIndex)
        {
            return m_recommendedAspectRatio[adapterIndex];
        }

        public static MyVideoModeEx GetVideoModeByIndex(int adapterIndex, int index)
        {
            if (index >= m_videoModeList[adapterIndex].Count || index < 0)
                return DEFAULT_FALL_BACK_4_3_800_600;

            return m_videoModeList[adapterIndex][index];
        }

        public static MyVideoModeEx GetFirstVideoModeWithClosestAspectRatio(int adapterIndex, float aspectRatio)
        {
            if (m_aspectRatioMap[adapterIndex].ContainsKey(aspectRatio) == true)
            {
                foreach (var vm in m_aspectRatioMap[adapterIndex][aspectRatio])
                {
                    return vm.Value;
                }
            }
            return null;
        }

        //  This video mode is used when user has empty config and we need to set some default resolution
        //  The idea is to find resolution which is close to 1280x720 because that one is high-res enough
        //  and runs smoothly on all sorts of computers
        public static MyVideoModeEx GetDefaultVideoModeForEmptyConfigWithClosestAspectRatio(int adapterIndex, float aspectRatio)
        {
            List<MyVideoModeEx> sortByClosestAspectRatio = new List<MyVideoModeEx>();

            for (int i = 0; i < m_videoModeList[adapterIndex].Count; i++)
            {
                MyVideoModeEx videoMode = m_videoModeList[adapterIndex][i];
                if (videoMode.Width <= 1280)
                {
                    sortByClosestAspectRatio.Add(videoMode);                    
                }
            }

            if (sortByClosestAspectRatio.Count > 0)
            {
                sortByClosestAspectRatio.Sort(
                    delegate(MyVideoModeEx p1, MyVideoModeEx p2)
                    {
                        float deltaP1 = Math.Abs(aspectRatio - p1.AspectRatio);
                        float deltaP2 = Math.Abs(aspectRatio - p2.AspectRatio);
                        return deltaP1.CompareTo(deltaP2);
                    }
                    );

                MyAspectRatioEnum thisAspectRatio = sortByClosestAspectRatio[0].AspectRatioEnum;

                //  Now look for highest resolution (we have guaranteed that none is more than 1280x***)
                MyVideoModeEx maxVideoMode = null;
                for (int i = 0; i < sortByClosestAspectRatio.Count; i++)
                {
                    MyVideoModeEx videoMode = sortByClosestAspectRatio[i];
                    if (videoMode.AspectRatioEnum == thisAspectRatio)
                    {
                        if ((maxVideoMode == null) || (videoMode.Width > maxVideoMode.Width))
                        {
                            maxVideoMode = videoMode;
                        }
                    }
                }

                return maxVideoMode;
            }
            else
            {
                return null;
            }
        }

        public static MyVideoModeEx GetDefaultVideoModeForEmptyConfig(int adapterIndex)
        {
            for (int i = 0; i < m_videoModeList[adapterIndex].Count; i++)
            {
                MyVideoModeEx videoMode = m_videoModeList[adapterIndex][i];
                if (videoMode.Width == System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width
                    &&
                    videoMode.Height == System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)
                {
                    return videoMode;
                }
            }

            return null;
        }

        public static int GetVideoModeIndexByWidthAndHeight(int adapterIndex, int width, int height)
        {
            for (int i = 0; i < m_videoModeList[adapterIndex].Count; i++)
            {
                if (m_videoModeList[adapterIndex][i].Width == width && m_videoModeList[adapterIndex][i].Height == height)
                    return i;
            }
            return 0;
        }

        public static MyVideoModeEx GetVideoModeByWidthAndHeight(int adapterIndex, int width, int height)
        {
            if (m_resolutionMap[adapterIndex].ContainsKey(width) == true && m_resolutionMap[adapterIndex][width].ContainsKey(height) == true)
                return m_resolutionMap[adapterIndex][width][height];
            
            return DEFAULT_FALL_BACK_4_3_800_600;
        }

        public static List<MyVideoModeEx> GetAllSupportedVideoModes(int adapterIndex)
        {
            return m_videoModeList[adapterIndex];
        }

        //  Call this at the application start. It will look into config file and get video parameters. If not found, default will be chosen. Then video mode will be initialized.
        static void InitFromConfig()
        {
            MyMwcLog.WriteLine("MyVideoModeManager.InitFromConfig START");

            //  Here we store width/height of windows desktop - becuase later they can 
            //  change after we switch to fullscreen and we want to have these original values.
            m_originalWindowsDesktopWidth = GraphicsAdapter.Default.CurrentDisplayMode.Width;
            m_originalWindowsDesktopHeight = GraphicsAdapter.Default.CurrentDisplayMode.Height;

            //  Read from config or use default/recommended values
            bool configFullscreen = MyConfig.FullScreen;
            int configVideoAdapter = MyConfig.VideoAdapter;
            MyVideoModeEx configVideoMode = MyConfig.VideoMode;
            bool configVerticalSync = MyConfig.VerticalSync;
            bool configHardwareCursor = MyConfig.HardwareCursor;
            MyRenderQualityEnum configRenderQuality = MyConfig.RenderQuality;
            float configFieldOfView = MyConfig.FieldOfView;

            //  Save values to config
            MyConfig.VideoAdapter = configVideoAdapter;
            MyConfig.VideoMode = configVideoMode;
            MyConfig.FullScreen = configFullscreen;
            MyConfig.VerticalSync = configVerticalSync;
            MyConfig.HardwareCursor = configHardwareCursor;
            MyConfig.RenderQuality = configRenderQuality;
            MyConfig.FieldOfView = configFieldOfView;
            MyConfig.Save();

            //  Finally change/init the video mode
            BeginChangeVideoMode(true, configVideoAdapter, configVideoMode, configFullscreen, configVerticalSync, configHardwareCursor, configRenderQuality, configFieldOfView, true, null);
            ApplyChanges();

            MyMwcLog.WriteLine("MyVideoModeManager.InitFromConfig END");
        }

        /// <summary>
        /// Apply video manager changes should be called from render, because it is all render operations.
        /// </summary>
        public static void ApplyChanges()
        {
            while (m_changeOperations.Count > 0)
            {
                var operation = m_changeOperations.Dequeue();

                operation.Process();
            }
        }

        public static bool IsThereAnyChangeToApply
        {
            get { return m_changeOperations != null && m_changeOperations.Count > 0; }
        }

        /// <summary>
        /// Begins the change video mode.
        /// </summary>
        /// <param name="callApplyChanges">if set to <c>true</c> [call apply changes].</param>
        /// <param name="videoMode">The video mode.</param>
        /// <param name="fullScreen">if set to <c>true</c> [full screen].</param>
        /// <param name="verticalSync">if set to <c>true</c> [vertical sync].</param>
        /// <param name="hardwareCursor">if set to <c>true</c> [hardware cursor].</param>
        /// <param name="renderQuality">The render quality.</param>
        /// <param name="fieldOfView">The field of view.</param>
        /// <param name="requestCallback">The request callback.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public static void BeginChangeVideoMode(bool callApplyChanges, int videoAdapter, MyVideoModeEx videoMode, bool fullScreen, bool verticalSync, bool hardwareCursor, MyRenderQualityEnum renderQuality, float fieldOfView, bool first, Action<MyVideoModeChangeOperation> requestCallback)
        {
            var result = new MyVideoModeChangeOperation(callApplyChanges, fullScreen, verticalSync, hardwareCursor, fieldOfView,
                                                        videoAdapter, videoMode, renderQuality, first, requestCallback);

            m_changeOperations.Enqueue(result);
        }

        /// <summary>
        /// Ends the change video mode.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool EndChangeVideoMode(MyVideoModeChangeOperation changeModeOperation)
        {
            Exceptions.ThrowIf<ArgumentException>(changeModeOperation == null, "Result of uknown type.");

            return changeModeOperation.Changed;
        }

        public static bool HasAnythingChanged(MyVideoModeChangeOperation changeModeOperation)
        {
            return changeModeOperation.SomethingChanged;
        }

        public static void UpdateScreenSize()
        {

            MyMwcLog.WriteLine("MyVideoModeManager.UpdateScreenSize - START");
            MyMwcLog.IncreaseIndent();

            //  Update or reload everything that depends on screen resolution
            MyMinerGame.UpdateScreenSize();
            MyGuiManager.UpdateScreenSize();
            MyGuiManager.RecreateMainMenuControls();
           
            
            MyCamera.UpdateScreenSize();
            MyHud.UpdateScreenSize();
            MySunGlare.UpdateScreenSize();

            if (MyGuiScreenGamePlay.Static != null) 
                MyGuiScreenGamePlay.Static.UpdateScreenSize();
            
            CenterizeWindowPosition();

               /*
            MyRender.CreateRenderTargets();
            MyRender.CreateEnvironmentMapsRT(MyRenderConstants.ENVIRONMENT_MAP_SIZE);
                 */
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.UpdateScreenSize - END");
        }

        private static void CenterizeWindowPosition()
        {
            var screenResolution = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            int windowWidth = MyMinerGame.GraphicsDeviceManager.PreferredBackBufferWidth;
            int windowHeight = MyMinerGame.GraphicsDeviceManager.PreferredBackBufferHeight;

            System.Windows.Forms.Control form = MyMinerGame.Static.Window.NativeWindow;
            form.ClientSize = new System.Drawing.Size(windowWidth, windowHeight);
            
            form.Location = new System.Drawing.Point(screenResolution.Width / 2 - windowWidth / 2, screenResolution.Height / 2 - windowHeight / 2);
            
        }

        public static bool IsSupportedDisplayMode(int width, int height, bool fullScreen)
        {
            return IsSupportedDisplayMode(m_videoAdapter, width, height, fullScreen);
        }

        public static bool IsSupportedDisplayMode(int videoAdapter, int width, int height, bool fullScreen)
        {
            MyMwcLog.WriteLine("MyVideoModeManager.IsSupportedDisplayMode - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.WriteLine("Width: " + width);
            MyMwcLog.WriteLine("Height: " + height);
            MyMwcLog.WriteLine("FullScreen: " + fullScreen);

            bool ret = false;

            if (fullScreen == true)
            {
                foreach (SharpDX.Direct3D9.DisplayMode dm in GraphicsAdapter.Adapters[videoAdapter].SupportedDisplayModes)
                {
                    
                    if ((dm.Width == width) && (dm.Height == height))
                    {
                        ret = true;
                        MyMwcLog.WriteLine("Supported display mode: " + dm.Width + " x " + dm.Height + " Fullscreen");
                        //  Don't "break" here, continue in this loop, so we can list ALL supported resolutions
                    }
                }
            }
            else
            {
                ret = true;

                //  We want to allow window of any size in desktop mode (otherwise there's no way how to create extreme-size screenshots)
                /*if ((width <= m_originalWindowsDesktopWidth) && (height <= m_originalWindowsDesktopHeight))
                {
                    ret = true;
                }*/
            }

            if ((width > MyMinerGame.GraphicsDeviceManager.MaxTextureSize) || (height > MyMinerGame.GraphicsDeviceManager.MaxTextureSize))
            {
                MyMwcLog.WriteLine("VideoMode " + width.ToString() + " x " + height.ToString() + " requires texture size which is not supported by this HW (this HW supports max " + MyMinerGame.GraphicsDeviceManager.MaxTextureSize.ToString() + ")");
                ret = false;
                /*
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                        ret = true;
                        MyMwcLog.WriteLine("Supported display mode: " + dm.Width + " x " + dm.Height + " Fullscreen");
                        //  Don't "break" here, continue in this loop, so we can list ALL supported resolutions

                } */
            }



            MyMwcLog.WriteLine("Ret: " + ret);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.IsSupportedDisplayMode - END");

            return ret;
        }

        //  Used to set desired antialiasing type. Doesn't check if type is available. We assume that
        //  m_antiAliasingType contains only valid types. And they are if ther were obtained from GetAvailableAntiAliasingTypes()
        static void GraphicsDeviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            MyMwcLog.WriteLine("MyVideoModeManager.GraphicsPreparingDeviceSettings - START");
            MyMwcLog.IncreaseIndent();

            //  This is for enabling profiling in NVIDIA PerfHUD. It's compiled only for developer build. We don't want it in release build.
            if (MyMwcFinalBuildConstants.ENABLE_PERFHUD == true)
            {      /*
                foreach (GraphicsAdapter adapter in GraphicsAdapter.Adapters)
                {
                    MyMwcLog.WriteLine(adapter.Description.Description);
                    if (adapter.Description.Description.Contains("PerfHUD"))
                    {
                        e.GraphicsDeviceInformation.Adapter = adapter;
                        //GraphicsAdapter.UseReferenceDevice = true;
                        break;
                    }
                }    */
            }
                                /*
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.MultiSampleCount: " + e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount);
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.PresentationInterval: " + e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval);
            //MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.DisplayOrientation: " + e.GraphicsDeviceInformation.PresentationParameters.DisplayOrientation);
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage: " + e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage);
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.DepthStencilFormat: " + e.GraphicsDeviceInformation.PresentationParameters.DepthStencilFormat);
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.IsFullScreen: " + e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen);
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.BackBufferWidth: " + e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth);
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.BackBufferHeight: " + e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight);
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.BackBufferFormat: " + e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat);
            //MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.Bounds: " + e.GraphicsDeviceInformation.PresentationParameters.b.Bounds);
            MyMwcLog.WriteLine("GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage: " + e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage);
                              */
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.GraphicsPreparingDeviceSettings - END");
        }

        public static void HookEventHandlers()
        {
            //  Used for setting antialiasing
            //MyMinerGame.GraphicsDeviceManager.PreparingDeviceSettings += GraphicsDeviceManager_PreparingDeviceSettings;
        }

        public static void LogInformation()
        {
            MyMwcLog.WriteLine("MyVideoModeManager.LogInformation - START");
            MyMwcLog.IncreaseIndent();

            try
            {
              //  MyMwcLog.WriteLine("MyMinerGame.Static.Content.RootDirectory: " + MyMinerGameDX.Static.Content.RootDirectory);
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine("Error occured during this method. Application will still continue. Detail description: " + e.ToString());
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.LogInformation - END");        
        }

        public static void LogEnvironmentInformation()
        {
            MyMwcLog.WriteLine("MyVideoModeManager.LogEnvironmentInformation - START");
            MyMwcLog.IncreaseIndent();

            try
            {
                
                ManagementObjectSearcher mos =
  new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                if (mos != null)
                {
                    foreach (ManagementObject mo in mos.Get())
                    {
                        MyMwcLog.WriteLine("Environment.ProcessorName: " + mo["Name"]);
                    }
                }

                MyMwcLog.WriteLine("Environment.ProcessorCount: " + Environment.ProcessorCount);
                MyMwcLog.WriteLine("Environment.OSVersion: " + Environment.OSVersion);
                MyMwcLog.WriteLine("Environment.Is64BitOperatingSystem: " + Environment.Is64BitOperatingSystem);
                MyMwcLog.WriteLine("Environment.CommandLine: " + Environment.CommandLine);
                MyMwcLog.WriteLine("Environment.CurrentDirectory: " + Environment.CurrentDirectory);
                MyMwcLog.WriteLine("Environment.Version: " + Environment.Version);
                MyMwcLog.WriteLine("Environment.WorkingSet: " + MyValueFormatter.GetFormatedLong(Environment.WorkingSet) + " bytes");

                //  Get info about memory
                var memory = new MEMORYSTATUSEX();
                GlobalMemoryStatusEx(memory);

                MyMwcLog.WriteLine("ComputerInfo.TotalPhysicalMemory: " + MyValueFormatter.GetFormatedLong((long)memory.ullTotalPhys) + " bytes");
                MyMwcLog.WriteLine("ComputerInfo.TotalVirtualMemory: " + MyValueFormatter.GetFormatedLong((long)memory.ullTotalVirtual) + " bytes");
                MyMwcLog.WriteLine("ComputerInfo.AvailablePhysicalMemory: " + MyValueFormatter.GetFormatedLong((long)memory.ullAvailPhys) + " bytes");
                MyMwcLog.WriteLine("ComputerInfo.AvailableVirtualMemory: " + MyValueFormatter.GetFormatedLong((long)memory.ullAvailVirtual) + " bytes");

                //  Get info about hard drives
                ConnectionOptions oConn = new ConnectionOptions();
                ManagementScope oMs = new ManagementScope("\\\\localhost", oConn);
                ObjectQuery oQuery = new ObjectQuery("select FreeSpace,Size,Name from Win32_LogicalDisk where DriveType=3");
                using (ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery))
                {
                    ManagementObjectCollection oReturnCollection = oSearcher.Get();
                    foreach (ManagementObject oReturn in oReturnCollection)
                    {
                        string capacity = MyValueFormatter.GetFormatedLong(Convert.ToInt64(oReturn["Size"]));
                        string freeSpace = MyValueFormatter.GetFormatedLong(Convert.ToInt64(oReturn["FreeSpace"]));
                        string name = oReturn["Name"].ToString();
                        MyMwcLog.WriteLine("Drive " + name + " | Capacity: " + capacity + " bytes | Free space: " + freeSpace + " bytes");
                    }
                    oReturnCollection.Dispose();
                }
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine("Error occured during enumerating environment information. Application is continuing. Exception: " + e.ToString());
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.LogEnvironmentInformation - END");
        }

        public static void LogApplicationInformation()
        {
            MyMwcLog.WriteLine("MyVideoModeManager.LogApplicationInformation - START");
            MyMwcLog.IncreaseIndent();

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                MyMwcLog.WriteLine("Assembly.GetName: " + assembly.GetName().ToString());
                MyMwcLog.WriteLine("Assembly.FullName: " + assembly.FullName);
                MyMwcLog.WriteLine("Assembly.Location: " + assembly.Location);
                MyMwcLog.WriteLine("Assembly.ImageRuntimeVersion: " + assembly.ImageRuntimeVersion);
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine("Error occured during enumerating application information. Application will still continue. Detail description: " + e.ToString());
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.LogApplicationInformation - END");
        }
     
        public static bool IsTripleHead()
        {
            return m_videoMode != null && m_videoMode.IsTripleHead;
        }

        public static bool IsHardwareCursorUsed()
        {
            // Never use hardware cursor in the exteral editor
            if (MinerWars.AppCode.ExternalEditor.MyEditorBase.Static != null) return false;

            return m_hardwareCursor;
        }

        static void GraphicsDevice_DeviceCreated(object sender, EventArgs e)
        {
            MyMwcLog.WriteLine("MyVideoModeManager.GraphicsDevice_DeviceCreated - START");
            MyMwcLog.IncreaseIndent();

            //MyMinerGameDX.GraphicsDeviceManager.GraphicsDevice.DeviceLost += new EventHandler<EventArgs>(GraphicsDevice_DeviceLost);

            GraphicsDevice_DeviceReset(sender, e);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.GraphicsDevice_DeviceCreated - END");
        }

        static void GraphicsDevice_DeviceLost(object sender, EventArgs e)
        {
            MyMwcLog.WriteLine("MyVideoModeManager.GraphicsDevice_DeviceLost - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.GraphicsDevice_DeviceLost - END");
        }

        static void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            MyMwcLog.WriteLine("MyVideoModeManager.GraphicsDevice_DeviceReset - START");
            MyMwcLog.IncreaseIndent();

            MyMinerGame.IsDeviceResetted = true;

            //Solves bad screen size when changed minimized-maximized state of game window
           // MyVideoModeManager.UpdateScreenSize();

            //MyTextureManager.ReloadTextures(false);
            //MyGuiManager.UpdateAfterDeviceReset();

            MyMinerGame.ResetSleep = 10;

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.GraphicsDevice_DeviceReset - END");
        }


        public static void UpdateAfterDeviceReset()
        {
            MyMwcLog.WriteLine("MyVideoModeManager.UpdateAfterDeviceReset - START");
            MyMwcLog.IncreaseIndent();

            //Solves bad screen size when changed minimized-maximized state of game window
            //MyVideoModeManager.UpdateScreenSize();

            for (int i = 0; i < 4; i++)
            {
                // Reset vertex textures and vertex textures sampler states
                //MyMinerGameDX.Static.GraphicsDevice.VertexTextures[i] = null;
                //MyMinerGameDX.Static.GraphicsDevice.VertexSamplerStates[i] = SamplerState.PointClamp;
            }

            //Because we use unmanaged resources here 
            //MyTextureManager.ReloadTextures(false);
            /*
            MyOcclusionQuery.ReloadOcclusionQueries(MyMinerGame.GraphicsDeviceManager.GraphicsDevice);
            if (MyGuiManager.GetFullscreenQuad() != null)
            {
                MyGuiManager.GetFullscreenQuad().CreateFullScreenQuad(MyMinerGame.GraphicsDeviceManager.GraphicsDevice);
            } */

            //MyGuiManager.UpdateAfterDeviceReset();

            //MyMinerGame.IsDeviceResetted = false;

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVideoModeManager.UpdateAfterDeviceReset - END");
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    }
}