using System;
  /*
using MinerWarsMath;

using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input
using MinerWarsMath.Media;
*/

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.VideoMode;
using System.Text;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Localization;

using DShowNET;
using SharpDX.Direct3D9;

//  IMPORTANT: It seems that even if my computer (Marek's) can handle 1280x720 in high bit-rate, a lot
//  of people has problem with it, even if they have high-end computer. Looks to be XNA/DirectX/Windows
//  problem. But by testing I have found that 

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

    class MyGuiScreenIntroVideo : MyGuiScreenBase
    {
        struct Subtitle
        {
            public Subtitle(int startMs, int lengthMs, MyTextsWrapperEnum textEnum)
            {
                this.StartTime = TimeSpan.FromMilliseconds(startMs);
                this.Length = TimeSpan.FromMilliseconds(lengthMs);
                this.Text = MyTextsWrapper.Get(textEnum);
            }

            public TimeSpan StartTime;
            public TimeSpan Length;
            public StringBuilder Text;
        }

//        Video m_video;
        VideoPlayer m_videoPlayer;
        bool m_playbackStarted;

        string[] m_videos;
        string m_currentVideo;
        List<Subtitle> m_subtitles = new List<Subtitle>();
        int m_currentSubtitleIndex = 0;

        Vector4 m_colorMultiplier = Vector4.One;

        private float m_volume;

        private bool m_loop = true;

        private StringBuilder m_subtitleToDraw;

        public MyGuiScreenIntroVideo(string[] videos)
            : base(Vector2.Zero, null, null)
        {
            DrawMouseCursor = false;
            m_closeOnEsc = false;
            m_drawEvenWithoutFocus = true;
            m_videos = videos;
        }

        public static MyGuiScreenIntroVideo CreateBackgroundScreen()
        {
            var result = new MyGuiScreenIntroVideo(new string[] { "Videos\\Background01_720p.wmv", "Videos\\Background02_720p.wmv", "Videos\\MinerWarsIntro_720p.wmv", "Videos\\MinerWarsLaunchTrailer.wmv" });
            result.m_volume = 0;
            result.m_colorMultiplier = new Vector4(0.5f, 0.5f, 0.5f, 1);
            return result;
        }

        public static MyGuiScreenIntroVideo CreateIntroScreen(Action onVideoFinished)
        {
            var result = new MyGuiScreenIntroVideo(new string[] { "Videos\\MinerWarsIntro_720p.wmv" });
            result.m_volume = MyConfig.MusicVolume;
            result.m_loop = false;
            AddCloseEvent(onVideoFinished, result);
            result.m_subtitles.Add(new Subtitle(11000, 4000, MyTextsWrapperEnum.Intro01)); // In 2070, Project Genesis was launched.
            result.m_subtitles.Add(new Subtitle(15500, 4500, MyTextsWrapperEnum.Intro02)); // An experimental project aiming to harness the energy of the sun.
            result.m_subtitles.Add(new Subtitle(30500, 4000, MyTextsWrapperEnum.Intro03)); // However, the experiment resulted in a temporary quantum change
            result.m_subtitles.Add(new Subtitle(34500, 5400, MyTextsWrapperEnum.Intro04)); // within gravitational laws and subatomic particle rotations.
            result.m_subtitles.Add(new Subtitle(41800, 6000, MyTextsWrapperEnum.Intro05)); // The gravitational quantum collapse tore all the larger objects + 1
            result.m_subtitles.Add(new Subtitle(48600, 6700, MyTextsWrapperEnum.Intro06)); // Planets, moons and even larger asteroids and comets + 1
            result.m_subtitles.Add(new Subtitle(52000, 8000, MyTextsWrapperEnum.Intro07)); // Creating new asteroid belts, marking their former planet's orbits.
            result.m_subtitles.Add(new Subtitle(60750, 4000, MyTextsWrapperEnum.Intro08)); // Together with the rest of the planets and moons,
            result.m_subtitles.Add(new Subtitle(65600, 4000, MyTextsWrapperEnum.Intro09)); // Billions of people died together along with our planet Earth.
            result.m_subtitles.Add(new Subtitle(69800, 5500, MyTextsWrapperEnum.Intro10)); // Concurrent massive solar storms killed additional + 1
            result.m_subtitles.Add(new Subtitle(76400, 5500, MyTextsWrapperEnum.Intro11)); // The Euro-American Confederation is held responsible for the celestial accident, 
            result.m_subtitles.Add(new Subtitle(81400, 3500, MyTextsWrapperEnum.Intro12)); // especially by its major opposition China.
            result.m_subtitles.Add(new Subtitle(86800, 3500, MyTextsWrapperEnum.Intro13)); // 2081 – eleven years after the Solar Event
            result.m_subtitles.Add(new Subtitle(91300, 5800, MyTextsWrapperEnum.Intro14)); // Here we are. According to the triangulation, + 1
            result.m_subtitles.Add(new Subtitle(103500, 7700, MyTextsWrapperEnum.Intro15)); // So this is the source of the signal. I must admit I wasn’t + 1
            result.m_subtitles.Add(new Subtitle(111500, 1800, MyTextsWrapperEnum.Intro16)); // Oh my God!
            result.m_subtitles.Add(new Subtitle(114000, 4800, MyTextsWrapperEnum.Intro17)); // This is the moment I’ve waited for my entire life.
            result.m_subtitles.Add(new Subtitle(119000, 2500, MyTextsWrapperEnum.Intro18)); // Any idea what it is?
            result.m_subtitles.Add(new Subtitle(125000, 3000, MyTextsWrapperEnum.Intro19)); // Three weeks earlier...
            return result;
        }

        private static void AddCloseEvent(Action onVideoFinished, MyGuiScreenIntroVideo result)
        {
            result.Closed += (screen) => onVideoFinished();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenIntroVideo";
        }

        private void LoadRandomVideo()
        {
            int index = MyMwcUtils.GetRandomInt(0, m_videos.Length);
            m_currentVideo = m_videos[index];
        }

        public override void LoadContent()
        {
            m_playbackStarted = false;

            LoadRandomVideo();

            //  Base load content must be called after child's load content
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            //  Stop playback
            if (m_videoPlayer != null)
            {
                m_videoPlayer.Stop();
                m_videoPlayer.Dispose();
                m_videoPlayer = null;
            }

            m_currentVideo = "";

            base.UnloadContent();

            //This one causes leaks in D3D
            //GC.Collect();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if ((input.IsAnyKeyPress() == true) || (input.IsAnyMousePress() == true))
            {
                CloseScreen();
            }
        }

        void Loop()
        {
            // loop video
            m_currentSubtitleIndex = 0;
            LoadRandomVideo();
            TryPlayVideo();
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            if (m_playbackStarted == false)
            {
                TryPlayVideo();

                m_playbackStarted = true;
            }
            else
            {
                if ((m_videoPlayer != null) && (m_videoPlayer.CurrentState != VideoState.Playing))
                {
                    if (m_loop)
                    {
                        Loop();
                    }
                    else
                    {
                        CloseScreen();
                    }
                }
                  
                // when playback started and player is null, video can't be played, so close screen
                if (m_playbackStarted && m_videoPlayer == null)
                {
                    CloseScreen();
                }

                if (m_state == MyGuiScreenState.CLOSING)
                {
                    //  Update volume, so during exiting it's fading out as alpha
                    if (m_videoPlayer != null)
                    {
                        m_videoPlayer.Volume = m_transitionAlpha;
                    }
                } 
            }

            if (MyConfig.Subtitles)
            {
                UpdateSubtitles();
            }
            else
            {
                m_subtitleToDraw = null;
            }

            return true;
        }

        void UpdateSubtitles()
        {
            if(m_subtitles == null || m_subtitles.Count == 0 || m_videoPlayer == null)
                return;

            if(m_currentSubtitleIndex >= m_subtitles.Count)
                return;

            var subtitle = m_subtitles[m_currentSubtitleIndex];

            
            while((subtitle.StartTime + subtitle.Length).TotalSeconds < m_videoPlayer.CurrentPosition)
            {
                m_currentSubtitleIndex++;
                if(m_currentSubtitleIndex == m_subtitles.Count)
                {
                    return;
                }
                subtitle = m_subtitles[m_currentSubtitleIndex];
            }

            if (subtitle.StartTime.TotalSeconds < m_videoPlayer.CurrentPosition)
            {
                m_subtitleToDraw = subtitle.Text;
            }
            else
            {
                m_subtitleToDraw = null;
            }    
        }

        void LoadSubtitle()
        {
        }

        void TryPlayVideo()
        {
            //  On some computers video playback can fail. In the moment we call m_videoPlayer.Play
            //  So I try/catched it. Better no video than crashing game.
            try
            {
                if (m_videoPlayer != null)
                    m_videoPlayer.Dispose();

                //  Start playback
                m_videoPlayer = new VideoPlayer(MyMinerGame.Static.RootDirectory + "\\" + m_currentVideo, MyMinerGame.Static.GraphicsDevice);
                m_videoPlayer.Play();
                m_videoPlayer.Volume = m_volume * 2;
            }
            catch (Exception ex)
            {
                //  Log this exception but then ignore it
                MyMwcLog.WriteLine(ex);

                //  Because of this exception, video player must be invalidated. We don't know what it contains.
                //  This will close this screen
                m_videoPlayer = null;
            }
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            if (ret)
            {
                if (m_videoPlayer != null)
                {
                    m_videoPlayer.Stop();
                    m_videoPlayer.Dispose();
                    m_videoPlayer = null;
                }
            }

            return ret;
        }

        public override bool Draw(float backgroundFadeAlpha)
        {
            if(base.Draw(backgroundFadeAlpha) == false) return false;

            if (m_videoPlayer != null)
            {
                m_videoPlayer.Update();

                Texture texture = m_videoPlayer.OutputFrame;

                Vector4 color = m_colorMultiplier * m_transitionAlpha;

                Rectangle videoRect;
                MyGuiManager.GetSafeAspectRatioFullScreenPictureSize(new MyMwcVector2Int(texture.GetLevelDescription(0).Width, texture.GetLevelDescription(0).Height), out videoRect);

                MyGuiManager.DrawSpriteBatch(texture, videoRect, new Color(color));

                if (m_subtitleToDraw != null)
                {
                    MyGuiManager.DrawStringCentered(MyGuiManager.GetFontMinerWarsWhite(), m_subtitleToDraw,
                            new Vector2(0.5f, 0.85f), 1.6f, Color.White, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);
                }
            }

            return true;
        }
    }
}
