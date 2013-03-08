using System;
using System.Collections.Generic;
using System.Threading;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Localization;
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
using SharpDX.Direct3D9;

//  This screen is special because it is drawn during we load some other screen - LoadContent - in another thread.
//  Player sees this "...Loading..." screen.

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenUnloading : MyGuiScreenBase
    {
        MyGuiScreenGamePlay m_screenToUnload;
        MyGuiScreenBase m_screenToShowAfter;
        
        //Thread m_loadingThread;
        Thread m_drawLoadingThread;

        Thread m_loadInDrawThread;
        
        MyTexture2D m_backgroundScreenTexture;
        MyTexture2D m_rotatingWheelTexture;


        public static MyTexture2D LastBackgroundTexture;

        volatile bool m_loadInDrawFinished;
        EventWaitHandle m_backgroundThreadExit;

        bool m_loadFinished;
        MyTextsWrapperEnum m_currentQuoteOfTheDay;

        public MyGuiScreenUnloading(MyGuiScreenGamePlay screenToUnload, MyGuiScreenBase screenToShowAfter)
            : base(Vector2.Zero, null, null)
        {
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

            m_loadFinished = false;

            MyMinerGame.IsGameReady = false;
            m_screenToShowAfter = screenToShowAfter;
        }
        
        public override string GetFriendlyName()
        {
            return "MyGuiScreenUnloading";
        }

        /// <summary>
        /// Sends server a message that player has left a sector
        /// </summary>
        public bool AnnounceLeaveToServer { get; set; }
        public MyMwcLeaveSectorReasonEnum LeaveSectorReason { get; set; }

        public override void LoadContent()
        {
            MyMwcLog.WriteLine("MyGuiScreenUnloading.LoadContent - START");
            MyMwcLog.IncreaseIndent();
            AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenUnloading::LoadContent");

            //  We must stop XACT sounds here, not in the background thread, because on Windows XP is safer if only
            //  main thread uses XACT. If we do it from background thread, we won't probably hear no sound.
            MyAudio.Stop();           

            m_backgroundScreenTexture = null ?? MyTextureManager.GetTexture<MyTexture2D>(GetRandomBackgroundTexture(), flags: TextureFlags.IgnoreQuality);
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


            m_loadFinished = false;

            AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiScreenUnloading.LoadContent - END");
        }

        static string GetRandomBackgroundTexture()
        {
            int randomNumber = MyMwcUtils.GetRandomInt(MyGuiConstants.LOADING_RANDOM_SCREEN_INDEX_MIN, MyGuiConstants.LOADING_RANDOM_SCREEN_INDEX_MAX + 1);
            string paddedNumber = randomNumber.ToString().PadLeft(3, '0');
            return "Textures\\GUI\\LoadingScreen\\Background" + paddedNumber;
        }

        public override void UnloadContent()
        {
            AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenUnloading::UnloadContent");
            //  This is just for case that whole application is quiting after Alt+F4 or something
            //  Don't try to unload content in that thread or something - we don't know in what state it is. Just abort it.

            //if (m_loadingThread != null && m_loadingThread.IsAlive == true)
            {
                //  Abort won't stop the thread immediately (it just throws exception inside it), so we use Join to wait until that thread is really finished                
              //  m_loadingThread.Abort();
               // m_loadingThread.Join();
            }

            if (m_backgroundScreenTexture != null) MyTextureManager.UnloadTexture(m_backgroundScreenTexture);
            if (m_backgroundScreenTexture!=null) MyTextureManager.UnloadTexture(m_rotatingWheelTexture);
            

            base.UnloadContent();

            AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            int blockUpdate = -1;
            AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiScreenUnloading::Update()", ref blockUpdate);

            if (this.GetState() == MyGuiScreenState.OPENED)
            {
                if (!m_loadFinished)
                {
                    int blockLoad = -1;
                    AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("LoadInBackgroundThread", ref blockLoad);

                    m_backgroundThreadExit = new ManualResetEvent(false);

                    m_drawLoadingThread = new Thread(BackgroundWorkerThread);
                    m_drawLoadingThread.Name = "Unloading thread";
                    m_drawLoadingThread.IsBackground = true;
                    MyGuiManager.AllowedThread = m_drawLoadingThread;
                    m_drawLoadingThread.Start();

                    if (m_screenToUnload != null)
                    {
                        MyMwcLog.WriteLine("RunBackgroundThread - START");
                        m_screenToUnload.UnloadContent();
                        m_screenToUnload.UnloadData();
                        m_screenToUnload.CloseScreenNow();
                        m_screenToUnload = null;
                        MyMwcLog.WriteLine("RunBackgroundThread - END");                        
                    }

                    m_backgroundThreadExit.Set();
                    m_drawLoadingThread.Join();

                    m_loadFinished = true;
                    MyGuiManager.AllowedThread = Thread.CurrentThread;

                    AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(blockLoad);
                }
            }
            if(m_loadFinished){
                MyMinerGame.IsGameReady = true;
                CloseScreen();
            }

            AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(blockUpdate);

            return true;
        }

        void OnLoadingDone()
        {
            
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            if (ret == true && m_screenToShowAfter != null)
            {
                //  Screen is loaded so now we can add it to other thread
                MyGuiManager.AddScreen(m_screenToShowAfter);
                m_screenToShowAfter = null;
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
                    MyGuiManager.BeginSpriteBatch();
                    DrawLoading(0.0f);
                }
                catch (Exception)
                {
                    Debug.Fail("Loading screen draw crashed: ");
                    // If anything went wrong (for instance the graphics device was lost
                    // or reset) we don't have any good way to recover while running on a
                    // background thread. Setting the device to null will stop us from
                    // rendering, so the main game can deal with the problem later on.
                    //MyMinerGame.Static.GraphicsDevice = null;
                }
                finally
                {
                    MyGuiManager.EndSpriteBatch();
                }


                //  Slow down drawing of "loading..." screens, so background thread who is actualy loading content will have more time (CPU resources) to do his job.
                //  Plus these two threads won't fight for graphic device. It's not big difference, just 10% or so.
                Thread.Sleep(MyGuiConstants.LOADING_THREAD_DRAW_SLEEP_IN_MILISECONDS);

                MyGuiScreenGamePlay.DrawLoadsCount();

                MyMinerGame.Static.GraphicsDevice.Present();
            }
            catch (Exception)
            {
                Debug.Fail("Loading screen draw crashed: ");
            }            
        }

        public bool DrawLoading(float backgroundFadeAlpha)
        {
            m_transitionAlpha = 1.0f;

            MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.Target, new SharpDX.ColorBGRA(0) , 0, 0);

            Color colorQuote = new Color(255, 255, 255, 250);     //  White
            colorQuote.A = (byte)(colorQuote.A * m_transitionAlpha);
            //if (m_backgroundTextureFromConstructor == null)
            {
                //////////////////////////////////////////////////////////////////////
                //  Normal loading screen
                //////////////////////////////////////////////////////////////////////

                //  Random background texture
                Rectangle backgroundRectangle;
                MyGuiManager.GetSafeHeightFullScreenPictureSize(MyGuiConstants.LOADING_BACKGROUND_TEXTURE_REAL_SIZE, out backgroundRectangle);
                MyGuiManager.DrawSpriteBatch(m_backgroundScreenTexture, backgroundRectangle, new Color(new Vector4(0.95f, 0.95f, 0.95f, m_transitionAlpha)));

                //  Miner Wars logo
                DrawMinerWarsLogo();

                //  Current version:
                //DrawAppVersion();
                //DrawGlobalVersionText();

                //  Quote of the day or something
                MyGuiManager.DrawStringCentered(MyGuiManager.GetFontMinerWarsBlue(), (new System.Text.StringBuilder()).Append(MyTextsWrapper.Get(MyTextsWrapperEnum.Tip)).Append(" ").Append(MyTextsWrapper.Get(m_currentQuoteOfTheDay)),
                    new Vector2(0.5f, 0.55f), 1.15f, colorQuote, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }

            LastBackgroundTexture = m_backgroundScreenTexture;

            //  Loading Please Wait
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.LoadingPleaseWait),
                MyGuiConstants.LOADING_PLEASE_WAIT_POSITION, MyGuiConstants.LOADING_PLEASE_WAIT_SCALE, new Color(MyGuiConstants.LOADING_PLEASE_WAIT_COLOR * GetTransitionAlpha()), 
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);

            if (base.Draw(backgroundFadeAlpha) == false)
                return false;

           
            return true;
        }       

        public override bool Draw(float backgroundFadeAlpha)
        {
            //  This must be in Draw method, because I have found that writing data to GPU doesn't work when game is not active for a while.
            //  For example when user pressed alt-tab during loading screen, some models were not loaded, but game didn't crash. Probably XNA error.
            //  What we do is: in background we process only data, don't touch the GPU. Then here when all data is loaded or processed,
            //  here in Draw we write everything to GPU, even though it stops main thread for a while. Reason is that Draw checks if
            //  graphics device isn't lost and doesn't call Draw if it is lost.

            return DrawLoading(backgroundFadeAlpha);
        }
    }
}
