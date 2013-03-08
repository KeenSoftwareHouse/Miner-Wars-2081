using System;
using System.Collections.Generic;
using System.Threading;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Textures;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.AppCode.Game.Managers;
using SysUtils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.World.Global;
using System.Diagnostics;
using System.Text;

using SharpDX.Direct3D9;

//  This screen is special because it is drawn during we load some other screen - LoadContent - in another thread.
//  Player sees this "...Loading..." screen.

namespace MinerWars.AppCode.Game.GUI
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;

    class MyGuiScreenLoading : MyGuiScreenBase
    {
        //We have to ensure there is always only one loading screen instance
        public static MyGuiScreenLoading Static;


        MyGuiScreenGamePlay m_screenToLoad;
        readonly MyGuiScreenGamePlay m_screenToUnload;
        
        Thread m_drawLoadingThread;
        
        MyTexture2D m_backgroundScreenTexture;
        MyTexture2D m_backgroundTextureFromConstructor;
        MyTexture2D m_rotatingWheelTexture;


        public static MyTexture2D LastBackgroundTexture;
        
        volatile bool m_loadInDrawFinished;
        EventWaitHandle m_backgroundThreadExit;

        bool m_loadFinished;
        MyTextsWrapperEnum m_currentQuoteOfTheDay;
        
        public MyGuiScreenLoading(MyGuiScreenGamePlay screenToLoad, MyGuiScreenGamePlay screenToUnload, MyTexture2D textureFromConstructor)
            : base(Vector2.Zero, null, null)
        {
            MyLoadingPerformance.Instance.StartTiming();

            System.Diagnostics.Debug.Assert(Static == null);
            Static = this;

            m_screenToLoad = screenToLoad;
            m_screenToUnload = screenToUnload;
            m_closeOnEsc = false;
            DrawMouseCursor = false;
            m_loadInDrawFinished = false;
            m_drawEvenWithoutFocus = true;
            m_currentQuoteOfTheDay = MyQuoteOfTheDay.GetRandomQuote();
            
            Vector2 loadingTextSize = MyGuiManager.GetNormalizedSize(MyGuiManager.GetFontMinerWarsBlue(),
                MyTextsWrapper.Get(MyTextsWrapperEnum.LoadingPleaseWait), MyGuiConstants.LOADING_PLEASE_WAIT_SCALE);
            m_rotatingWheelTexture = MyGuiManager.GetLoadingTexture();
            Controls.Add(new MyGuiControlRotatingWheel(this, MyGuiConstants.LOADING_PLEASE_WAIT_POSITION - new Vector2(0, 0.075f + loadingTextSize.Y),
                MyGuiConstants.ROTATING_WHEEL_COLOR, MyGuiConstants.ROTATING_WHEEL_DEFAULT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, m_rotatingWheelTexture));
            m_backgroundTextureFromConstructor = textureFromConstructor;

            m_loadFinished = false;
            
            if (m_screenToLoad != null)
            {
                MyMinerGame.IsGameReady = false;
            }
        }

        public MyGuiScreenLoading(MyGuiScreenGamePlay screenToLoad, MyGuiScreenGamePlay screenToUnload)
            : this(screenToLoad, screenToUnload, null)
        {
        }
        
        public override string GetFriendlyName()
        {
            return "MyGuiScreenLoading";
        }

        /// <summary>
        /// Sends server a message that player has left a sector
        /// </summary>
        public bool AnnounceLeaveToServer { get; set; }
        public MyMwcLeaveSectorReasonEnum LeaveSectorReason { get; set; }

        public override void LoadContent()
        {
            MyMwcLog.WriteLine("MyGuiScreenLoading.LoadContent - START");
            MyMwcLog.IncreaseIndent();
            AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenLoading::LoadContent");

            //  We must stop XACT sounds here, not in the background thread, because on Windows XP is safer if only
            //  main thread uses XACT. If we do it from background thread, we won't probably hear no sound.
            if (MyAudio.GetMusicState() != MyMusicState.Stopped) 
            {
                MyAudio.Stop();
            }


            m_backgroundScreenTexture = m_backgroundTextureFromConstructor ?? MyTextureManager.GetTexture<MyTexture2D>(GetRandomBackgroundTexture(), flags: TextureFlags.IgnoreQuality);
            m_minerWarsLogoTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MinerWarsLogoLarge", null, LoadingMode.Immediate, flags: TextureFlags.IgnoreQuality);
            //m_minerWarsActorsTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MinerWarsActors", flags: TextureFlags.IgnoreQuality);

            if (m_screenToUnload != null)
            {
                //  If there is existing screen we are trying to replace (e.g. gameplay screen), we will mark this one as unloaded, so
                //  then remove screen method won't try to unload it and we can do it in our thread. This is also the reason why we do it
                //  here, becasue changing IsLoaded in paralel thread can't tell us when it happens - and this must be serial.
                m_screenToUnload.IsLoaded = false;
                m_screenToUnload.CloseScreenNow();
            }

            m_currentQuoteOfTheDay = MyQuoteOfTheDay.GetRandomQuote();

            //  Base load content must be called after child's load content
            base.LoadContent();


            if (m_screenToLoad != null && !m_loadInDrawFinished && m_loadFinished)
            {
                m_screenToLoad.SetState(MyGuiScreenState.OPENING);
                m_screenToLoad.LoadContent();
            }
            else
                m_loadFinished = false;

            AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiScreenLoading.LoadContent - END");
        }

        static string GetRandomBackgroundTexture()
        {
            int randomNumber = MyMwcUtils.GetRandomInt(MyGuiConstants.LOADING_RANDOM_SCREEN_INDEX_MIN, MyGuiConstants.LOADING_RANDOM_SCREEN_INDEX_MAX + 1);
            string paddedNumber = randomNumber.ToString().PadLeft(3, '0');
            return "Textures\\GUI\\LoadingScreen\\Background" + paddedNumber;
        }

        public override void UnloadContent()
        {
            AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenLoading::UnloadContent");
            //  This is just for case that whole application is quiting after Alt+F4 or something
            //  Don't try to unload content in that thread or something - we don't know in what state it is. Just abort it.

            if (m_drawLoadingThread != null && m_drawLoadingThread.IsAlive == true)
            {
                m_backgroundThreadExit.Set();
                m_drawLoadingThread.Join();
            }

            if (m_backgroundScreenTexture != null) MyTextureManager.UnloadTexture(m_backgroundScreenTexture);
            if (m_backgroundTextureFromConstructor!=null) MyTextureManager.UnloadTexture(m_backgroundTextureFromConstructor);
            if (m_backgroundScreenTexture!=null) MyTextureManager.UnloadTexture(m_rotatingWheelTexture);
            if (m_currentSectorDescription != null && m_currentSectorDescription.Item2 != null && m_currentSectorDescription.Item2.Item3 != null) MyTextureManager.UnloadTexture(m_currentSectorDescription.Item2.Item3);


            if (m_screenToLoad != null && !m_loadFinished && m_loadInDrawFinished)
            {
                //  Call unload because there might be running precalc threads and we need to stop them
                //m_screenToLoad.UnloadObjects();
                m_screenToLoad.UnloadContent();
                m_screenToLoad.UnloadData();
               
                //m_screenToLoad.UnloadData();
                m_screenToLoad = null;
            }

            
            if (m_screenToLoad != null && !m_loadInDrawFinished)
            {
                m_screenToLoad.UnloadContent();
            }
            

            base.UnloadContent();

            System.Diagnostics.Debug.Assert(Static == this);
            Static = null;

            AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public void AddEnterSectorResponse(MyMwcObjectBuilder_Checkpoint checkpoint, MyMissionID? missionID)
        {
            m_screenToLoad.AddEnterSectorResponse(checkpoint, missionID);
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            int blockUpdate = -1;
            AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenLoading::Update()", ref blockUpdate);

            if (this.GetState() == MyGuiScreenState.OPENED)
            {
                if (!m_loadFinished)
                {
                    int blockLoad = -1;
                    AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("LoadInBackgroundThread", ref blockLoad);

                    m_backgroundThreadExit = new ManualResetEvent(false);

                    m_drawLoadingThread = new Thread(BackgroundWorkerThread);
                    m_drawLoadingThread.Name = "LoadInBackground";
                    m_drawLoadingThread.IsBackground = true; 
                    m_drawLoadingThread.Start();

                    MyGuiManager.AllowedThread = m_drawLoadingThread;

                    if (m_screenToLoad != null)
                    {
                        MyMwcLog.WriteLine("RunBackgroundThread - START");
                        m_screenToLoad.RunBackgroundThread();
                        MyMwcLog.WriteLine("RunBackgroundThread - END");                        
                    }

                    m_backgroundThreadExit.Set();
                    m_drawLoadingThread.Join();

                    MyGuiManager.AllowedThread = Thread.CurrentThread;

                    m_loadFinished = true;

                    AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(blockLoad);
                }
            }

            if (m_loadFinished)
            {
                CloseScreen();
            }

            AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(blockUpdate);

            return true;
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            if (ret == true && m_screenToLoad != null)
            {
                //  Screen is loaded so now we can add it to other thread
                MyGuiManager.AddScreen(m_screenToLoad);
                m_screenToLoad = null;
            }

            return ret;
        }

        static long lastEnvWorkingSet = 0;
        static long lastGc = 0;
        static long lastVid = 0;
        
        void BackgroundWorkerThread()
        {
            long lastTime = MyMinerGame.TotalTimeInMilliseconds;

            // EventWaitHandle.WaitOne will return true if the exit signal has
            // been triggered, or false if the timeout has expired. We use the
            // timeout to update at regular intervals, then break out of the
            // loop when we are signalled to exit.

            while (!m_backgroundThreadExit.WaitOne(1000 / 30))
            {
                //GameTime gameTime = GetGameTime(ref lastTime);

                DrawLoadAnimation();
            }
        }

        void DrawLoadAnimation(/*GameTime gameTime*/)
        {
            if ((MyMinerGame.Static.GraphicsDevice == null) || MyMinerGame.Static.GraphicsDevice.IsDisposed)
                return;

            try
            {
                try
                {
                    MyMinerGame.Static.GraphicsDevice.BeginScene();

                    MyGuiManager.BeginSpriteBatch();
                    DrawLoading(0.0f);
                    MyGuiScreenGamePlay.DrawLoadsCount();
                }
                catch (Exception)
                {
                    // PetrM told to comment out
                    //Debug.Fail("Loading screen draw crashed: ");

                    // If anything went wrong (for instance the graphics device was lost
                    // or reset) we don't have any good way to recover while running on a
                    // background thread. Setting the device to null will stop us from
                    // rendering, so the main game can deal with the problem later on.
                    //MyMinerGameDX.Static.GraphicsDevice = null;
                }
                finally
                {
                    MyGuiManager.EndSpriteBatch();
                    MyMinerGame.Static.GraphicsDevice.EndScene();
                }

                //  Slow down drawing of "loading..." screens, so background thread who is actualy loading content will have more time (CPU resources) to do his job.
                //  Plus these two threads won't fight for graphic device. It's not big difference, just 10% or so.
                Thread.Sleep(MyGuiConstants.LOADING_THREAD_DRAW_SLEEP_IN_MILISECONDS);

                var res =MyMinerGame.Static.GraphicsDevice.TestCooperativeLevel();
                if (res.Code == ResultCode.Success.Result.Code)
                {
                    MyMinerGame.Static.GraphicsDevice.Present();
                }
            }
            catch (Exception)
            {
                Debug.Fail("Loading screen draw crashed: ");
            }            
        }

        public bool DrawLoading(float backgroundFadeAlpha)
        {
            //MyAudio.Update(true);
            m_transitionAlpha = 1.0f;

            MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.Target, new SharpDX.ColorBGRA(0) , 0, 0);

            Color colorQuote = new Color(255, 255, 255, 250);     //  White
            colorQuote.A = (byte)(colorQuote.A * m_transitionAlpha);
            //if (m_backgroundTextureFromConstructor == null)
            {
                //////////////////////////////////////////////////////////////////////
                //  Normal loading screen
                //////////////////////////////////////////////////////////////////////

                MyTextsWrapperEnum sectorName = MyTextsWrapperEnum.Null;
                MyTextsWrapperEnum sectorDescription = MyTextsWrapperEnum.Null;
                MyTexture2D sectorTexture = null;

                bool descriptionAvailable = false;
                if (m_screenToLoad != null)
                {
                    var position = m_screenToLoad.GetSectorIdentifier().Position;
                    descriptionAvailable = GetSectorDescription(position, out sectorName, out sectorDescription, out sectorTexture);
                }
                else if (MyGuiScreenGamePlay.Static != null)
                {
                    var position = MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position;
                    descriptionAvailable = GetSectorDescription(position, out sectorName, out sectorDescription, out sectorTexture);
                }
                else
                {
                    descriptionAvailable = false;
                }
                    
                

                //  Random background texture
                Rectangle backgroundRectangle;
                MyGuiManager.GetSafeHeightFullScreenPictureSize(MyGuiConstants.LOADING_BACKGROUND_TEXTURE_REAL_SIZE, out backgroundRectangle);
                MyGuiManager.DrawSpriteBatch(descriptionAvailable ? sectorTexture : m_backgroundScreenTexture, backgroundRectangle, new Color(new Vector4(0.95f, 0.95f, 0.95f, m_transitionAlpha)));

                //  Miner Wars logo
                DrawMinerWarsLogo();

                //  Current version:
                //DrawAppVersion();
                //DrawGlobalVersionText();

                if (descriptionAvailable)
                {
                    MyGuiManager.DrawStringCentered(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(sectorName),
                        new Vector2(0.5f, 0.4f), 1.6f, colorQuote, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                    MyGuiManager.DrawStringCentered(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(sectorDescription),
                        new Vector2(0.5f, 0.5f), 1, colorQuote, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                }
                else
                {
                    //  Quote of the day
                    MyGuiManager.DrawStringCentered(MyGuiManager.GetFontMinerWarsBlue(), (new StringBuilder()).Append(MyTextsWrapper.Get(MyTextsWrapperEnum.Tip)).Append(" ").Append(MyTextsWrapper.Get(m_currentQuoteOfTheDay)),
                        new Vector2(0.5f, 0.5f), 1.15f, colorQuote, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                }
            }
           /* else
            {
                //////////////////////////////////////////////////////////////////////
                //  Loading screen when there is specified background texture is little customized
                //////////////////////////////////////////////////////////////////////

                MyGuiManager.DrawSpriteBatch(m_backgroundScreenTexture, Vector2.Zero, new Color(new Vector4(0.95f, 0.95f, 0.95f, m_transitionAlpha)));
            }*/

            LastBackgroundTexture = m_backgroundScreenTexture;

            //  Loading Please Wait
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.LoadingPleaseWait),
                MyGuiConstants.LOADING_PLEASE_WAIT_POSITION, MyGuiConstants.LOADING_PLEASE_WAIT_SCALE, new Color(MyGuiConstants.LOADING_PLEASE_WAIT_COLOR * GetTransitionAlpha()), 
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);

            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.LongerLoadingTimes),
                MyGuiConstants.LOADING_PLEASE_WAIT_POSITION + new Vector2(0, 0.025f), MyGuiConstants.LOADING_PLEASE_WAIT_SCALE * 0.8f, new Color(MyGuiConstants.LOADING_PLEASE_WAIT_COLOR * GetTransitionAlpha()),
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);


            if (base.Draw(backgroundFadeAlpha) == false)
                return false;

           
            return true;
        }

        private static Dictionary<MyMwcVector3Int, Tuple<MyTextsWrapperEnum, MyTextsWrapperEnum, string>> m_sectorDescriptions = new Dictionary<MyMwcVector3Int, Tuple<MyTextsWrapperEnum, MyTextsWrapperEnum, string>>()
            {
                { new MyMwcVector3Int(-913818, 0, -790076), Tuple.Create(MyTextsWrapperEnum.EAC_SURVEY_SITE, MyTextsWrapperEnum.EAC_SURVEY_SITE_SectorDescription, "1EACSurveySite") },
                { new MyMwcVector3Int(-3851812, 0, -2054500), Tuple.Create(MyTextsWrapperEnum.LAIKA, MyTextsWrapperEnum.LAIKA_SectorDescription, "2Laika") },
                { new MyMwcVector3Int(3360719, 0, -27890037), Tuple.Create(MyTextsWrapperEnum.BARTHS_MOON_CONVINCE, MyTextsWrapperEnum.BARTHS_MOON_SectorDescription, "3BarthsMoon") },
                { new MyMwcVector3Int(190921, 0, 2152692), Tuple.Create(MyTextsWrapperEnum.PIRATE_BASE, MyTextsWrapperEnum.PIRATE_BASE_SectorDescription, "4PiratePase") },
                { new MyMwcVector3Int(-7420630, 0, 388170), Tuple.Create(MyTextsWrapperEnum.RUSSIAN_WAREHOUSE, MyTextsWrapperEnum.RUSSIAN_WAREHOUSE_SectorDescription, "5RussianWarehouse") },
                { new MyMwcVector3Int(-190694, 0,-18204363), Tuple.Create(MyTextsWrapperEnum.LAST_HOPE, MyTextsWrapperEnum.LAST_HOPE_SectorDescription, "6LastHope") },
                { new MyMwcVector3Int(2567538, 0,-172727), Tuple.Create(MyTextsWrapperEnum.JUNKYARD_RETURN, MyTextsWrapperEnum.JUNKYARD_SectorDescription, "7Junkyard") },
                { new MyMwcVector3Int(-4274372, 0, 4874227), Tuple.Create(MyTextsWrapperEnum.CHINESE_TRANSPORT, MyTextsWrapperEnum.CHINESE_MINES_SectorDescription, "8ChineseTransport") },
                { new MyMwcVector3Int(-2716080, 0, 4951053), Tuple.Create(MyTextsWrapperEnum.CHINESE_REFINERY, MyTextsWrapperEnum.CHINESE_REFINERY_SectorDescription, "9ChineseRefinery") },
                { new MyMwcVector3Int(-1919599,0, 5268734), Tuple.Create(MyTextsWrapperEnum.CHINESE_ESCAPE, MyTextsWrapperEnum.CHINESE_ESCAPE_SectorDescription, "10ChineseEscape") },
                { new MyMwcVector3Int(-588410, 0, -3425542), Tuple.Create(MyTextsWrapperEnum.FORT_VALIANT_SectorName, MyTextsWrapperEnum.FORT_VALIANT_SectorDescription, "11FortValiant") },
                { new MyMwcVector3Int(4169480, 0, -8216683), Tuple.Create(MyTextsWrapperEnum.SLAVER_BASE_DELTA_EARNINGS, MyTextsWrapperEnum.SLAVER_BASE_1_SectorDescription, "12SlaverBaseWipeout") },
                { new MyMwcVector3Int(2052452, 0, -10533886), Tuple.Create(MyTextsWrapperEnum.SLAVER_BASE_DELTA_EARNINGS, MyTextsWrapperEnum.SLAVER_BASE_2_SectorDescription, "13SlaverBase") },
                { new MyMwcVector3Int(-1922856, 0, -2867519), Tuple.Create(MyTextsWrapperEnum.RIME_CONVINCE, MyTextsWrapperEnum.RIME_SectorDescription, "14Rim") },
                { new MyMwcVector3Int(4189723, 0, -2201402), Tuple.Create(MyTextsWrapperEnum.RESEARCH_VESSEL, MyTextsWrapperEnum.RESEARCH_VESSEL_SectorDescription, "15ResearchVessel") },
                { new MyMwcVector3Int(-56700, 0, 4276), Tuple.Create(MyTextsWrapperEnum.RIFT, MyTextsWrapperEnum.RIFT_SectorDescription, "16Rift") },
                { new MyMwcVector3Int(2329559, 0, 4612446), Tuple.Create(MyTextsWrapperEnum.CHINESE_TRANSMITTER, MyTextsWrapperEnum.CHINESE_TRANSMITTER_SectorDescription, "17ChineseTransmitter") },
                { new MyMwcVector3Int(-4988032, 0, -865747), Tuple.Create(MyTextsWrapperEnum.RUSSIAN_TRANSMITTER, MyTextsWrapperEnum.RUSSIAN_TRANSMITTER_SectorDescritpion, "18RussianTransmitter") },
                { new MyMwcVector3Int(-2325831, 0, -7186381), Tuple.Create(MyTextsWrapperEnum.REICHSTAG, MyTextsWrapperEnum.REICHSTAG_SectorDescription, "19Reichstag") },
                { new MyMwcVector3Int(-4081250, 0, -6815625), Tuple.Create(MyTextsWrapperEnum.WHITE_WOLVES, MyTextsWrapperEnum.WHITE_WOLVES_RESEARCH_LAB_SectorDescription, "20BioResearch") },
                { new MyMwcVector3Int(-2809328, 0, -4609055), Tuple.Create(MyTextsWrapperEnum.TWIN_TOWERS, MyTextsWrapperEnum.DOPPELBURG_SectorDescription, "21TwinTowers") },
                { new MyMwcVector3Int(5480055, 0, -5077310), Tuple.Create(MyTextsWrapperEnum.EAC_PRISON, MyTextsWrapperEnum.EAC_PRISON_SectorDescription, "22EACPrison") },
                { new MyMwcVector3Int(3818505, 0, -4273800), Tuple.Create(MyTextsWrapperEnum.EAC_TRANSMITTER, MyTextsWrapperEnum.EAC_TRANSMITTER_SectorDescription, "23EACTransmitter") },
                { new MyMwcVector3Int(-1202900, 0, -112652), Tuple.Create(MyTextsWrapperEnum.ALIEN_GATE, MyTextsWrapperEnum.ALIEN_GATE_SectorDescription, "24AlienGate") },
            };
        private Tuple<MyMwcVector3Int, Tuple<MyTextsWrapperEnum, MyTextsWrapperEnum, MyTexture2D>> m_currentSectorDescription = new Tuple<MyMwcVector3Int, Tuple<MyTextsWrapperEnum, MyTextsWrapperEnum, MyTexture2D>>(default(MyMwcVector3Int), null);

        private bool GetSectorDescription(MyMwcVector3Int sectorPosition, out MyTextsWrapperEnum sectorName, out MyTextsWrapperEnum sectorDescription, out MyTexture2D sectorTexture)
        {
            if (m_currentSectorDescription.Item1 != sectorPosition)
            {
                Tuple<MyTextsWrapperEnum, MyTextsWrapperEnum, MyTexture2D> description = null;

                Tuple<MyTextsWrapperEnum, MyTextsWrapperEnum, string> result;
                if (m_sectorDescriptions.TryGetValue(sectorPosition, out result))
                {
                    string textureName = "Textures\\GUI\\LoadingScreen\\" + result.Item3;
                    var texture = MyTextureManager.GetTexture<MyTexture2D>(textureName, flags: TextureFlags.IgnoreQuality);
                    description = Tuple.Create(result.Item1, result.Item2, texture);
                }

                m_currentSectorDescription = Tuple.Create(sectorPosition, description);
            }

            if (m_currentSectorDescription.Item2 != null)
            {
                sectorName = m_currentSectorDescription.Item2.Item1;
                sectorDescription = m_currentSectorDescription.Item2.Item2;
                sectorTexture = m_currentSectorDescription.Item2.Item3;
                return true;
            }
            else
            {
                sectorName = MyTextsWrapperEnum.Null;
                sectorDescription = MyTextsWrapperEnum.Null;
                sectorTexture = null;
                return false;
            }
        }                

        public override bool Draw(float backgroundFadeAlpha)
        {
            return DrawLoading(backgroundFadeAlpha);
        }
    }
}
