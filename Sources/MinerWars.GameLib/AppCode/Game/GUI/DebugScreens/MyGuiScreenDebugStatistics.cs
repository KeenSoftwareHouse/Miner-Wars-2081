using System;
using System.Text;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using SysUtils;
using System.Globalization;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Toolkit.Input;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugStatistics : MyGuiScreenDebugBase
    {
        static StringBuilder m_frameDebugText = new StringBuilder(1024);
        static StringBuilder m_frameDebugTextRA = new StringBuilder(2048);
        static List<StringBuilder> m_texts = new List<StringBuilder>(32);
        static List<StringBuilder> m_rightAlignedtexts = new List<StringBuilder>(32);

        List<Toolkit.Input.Keys> m_pressedKeys = new List<Toolkit.Input.Keys>(10);

        public MyGuiScreenDebugStatistics()
            : base(new Vector2(0.5f, 0.5f), new Vector2(), null, true)
        {
            m_isTopMostScreen = true;
            m_drawEvenWithoutFocus = true;
            m_canHaveFocus = false;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugStatistics";
        }

        //  Frame Debug Text - is cleared at the begining of Update and rendered at the end of Draw
        public void AddToFrameDebugText(string s)
        {
            m_frameDebugText.AppendLine(s);
        }

        //  Frame Debug Text - is cleared at the begining of Update and rendered at the end of Draw
        public void AddToFrameDebugText(StringBuilder s)
        {
            m_frameDebugText.GetStrings(s);
            m_frameDebugText.AppendLine();
        }

        //  Frame Debug Text Right Aligned - is cleared at the begining of Update and rendered at the end of Draw
        public void AddDebugTextRA(string s)
        {
            m_frameDebugTextRA.Append(s);
            m_frameDebugTextRA.AppendLine();
        }
        //  Frame Debug Text Right Aligned - is cleared at the begining of Update and rendered at the end of Draw
        public void AddDebugTextRA(StringBuilder s)
        {
            m_frameDebugTextRA.GetStrings(s);
            m_frameDebugTextRA.AppendLine();
        }

        //  Frame Debug Text - is cleared at the begining of Update and rendered at the end of Draw
        public void ClearFrameDebugText()
        {
            MyMwcUtils.ClearStringBuilder(m_frameDebugText);
            MyMwcUtils.ClearStringBuilder(m_frameDebugTextRA);
            //MyAudio.WriteDebugInfo(m_frameDebugTextRA);
        }

        public Vector2 GetScreenLeftTopPosition()
        {
            float deltaPixels = 25 * MyGuiManager.GetSafeScreenScale();
            Rectangle fullscreenRectangle = MyGuiManager.GetSafeFullscreenRectangle();
            return MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(deltaPixels, deltaPixels));
        }

        public Vector2 GetScreenRightTopPosition()
        {
            float deltaPixels = 25 * MyGuiManager.GetSafeScreenScale();
            Rectangle fullscreenRectangle = MyGuiManager.GetSafeFullscreenRectangle();
            return MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(fullscreenRectangle.Width - deltaPixels, deltaPixels));
        }

        static List<StringBuilder> m_statsStrings = new List<StringBuilder>();
        static int m_stringIndex = 0;

        public static StringBuilder StringBuilderCache
        {
            get
            {
                if (m_stringIndex >= m_statsStrings.Count)
                    m_statsStrings.Add(new StringBuilder(1024));

                StringBuilder sb = m_statsStrings[m_stringIndex++];
                return sb.Clear();
            }
        }

        
        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            float rowDistance = MyGuiConstants.DEBUG_STATISTICS_ROW_DISTANCE;
            float textScale = MyGuiConstants.DEBUG_STATISTICS_TEXT_SCALE;

            m_stringIndex = 0;
            m_texts.Clear();
            m_rightAlignedtexts.Clear();
           
            
            m_texts.Add(StringBuilderCache.GetFormatedFloat("FPS: ", MyFpsManager.GetFps()));

            m_texts.Add(MyGuiManager.GetGuiScreensForDebug());
            
            m_texts.Add(StringBuilderCache.GetFormatedBool("Paused: ", MyMinerGame.IsPaused()));
            m_texts.Add(StringBuilderCache.GetFormatedDateTimeOffset("System Time: ", TimeUtil.LocalTime));
            m_texts.Add(StringBuilderCache.GetFormatedTimeSpan("Total MISSION Time: ", MyMissions.ActiveMission == null ? TimeSpan.Zero : MyMissions.ActiveMission.MissionTimer.GetElapsedTime()));
            m_texts.Add(StringBuilderCache.GetFormatedTimeSpan("Total GAME-PLAY Time: ", TimeSpan.FromMilliseconds(MyMinerGame.TotalGamePlayTimeInMilliseconds)));
            m_texts.Add(StringBuilderCache.GetFormatedTimeSpan("Total Time: ", TimeSpan.FromMilliseconds(MyMinerGame.TotalTimeInMilliseconds)));
            
            m_texts.Add(StringBuilderCache.GetFormatedLong("GC.GetTotalMemory: ", GC.GetTotalMemory(false), " bytes"));

//#if MEMORY_PROFILING
            //TODO: I am unable to show this without allocations
            m_texts.Add(StringBuilderCache.GetFormatedLong("Environment.WorkingSet: ", MyWindowsAPIWrapper.WorkingSet, " bytes"));

            m_texts.Add(StringBuilderCache.GetFormatedFloat("Available videomemory: ", MyMinerGame.Static.GraphicsDevice.AvailableTextureMemory / (1024.0f * 1024.0f), " MB"));
            // TODO: Videomem
            //m_texts.Add(StringBuilderCache.GetFormatedFloat("Allocated videomemory: ", MyProgram.GetResourcesSizeInMB(), " MB"));
//#endif

#if PHYSICS_SENSORS_PROFILING
            if (MinerWars.AppCode.Physics.MyPhysics.physicsSystem != null)
            {
                m_texts.Add(StringBuilderCache.GetFormatedInt("Physics sensor - new allocated interactions: ", MinerWars.AppCode.Physics.MyPhysics.physicsSystem.GetSensorInteractionModule().GetNewAllocatedInteractionsCount()));
                m_texts.Add(StringBuilderCache.GetFormatedInt("Physics sensor - interactions in use: ", MinerWars.AppCode.Physics.MyPhysics.physicsSystem.GetSensorInteractionModule().GetInteractionsInUseCount()));
                m_texts.Add(StringBuilderCache.GetFormatedInt("Physics sensor - interactions in use MAX: ", MinerWars.AppCode.Physics.MyPhysics.physicsSystem.GetSensorInteractionModule().GetInteractionsInUseCountMax()));
                m_texts.Add(StringBuilderCache.GetFormatedInt("Physics sensor - all sensors: ", MinerWars.AppCode.Physics.MyPhysics.physicsSystem.GetSensorModule().SensorsCount()));
                m_texts.Add(StringBuilderCache.GetFormatedInt("Physics sensor - active sensors: ", MinerWars.AppCode.Physics.MyPhysics.physicsSystem.GetSensorModule().ActiveSensors.Count));
            }
#endif
                      
            m_texts.Add(StringBuilderCache.GetFormatedInt("Sound Instances Total 2D: ", MyAudio.GetSoundInstancesTotal2D()));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Sound Instances Total 3D: ", MyAudio.GetSoundInstancesTotal3D()));
            m_texts.Add(MyAudio.GetCurrentTransitionForDebug());
            if (MyAudio.GetMusicCue() != null)
            {
                //m_texts.Add(StringBuilderCache.GetStrings("Currently Playing Music Cue: ", MyAudio.GetMusicCue().Value.GetCue().Name));
            }
                
            m_texts.Add(StringBuilderCache.Clear());
            m_texts.Add(StringBuilderCache.GetFormatedInt("Textures 2D Count: ", MyPerformanceCounter.PerAppLifetime.Textures2DCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Textures 2D Size In Pixels: ", MyPerformanceCounter.PerAppLifetime.Textures2DSizeInPixels));
            m_texts.Add(StringBuilderCache.GetFormatedDecimal("Textures 2D Size In Mb: ", MyPerformanceCounter.PerAppLifetime.Textures2DSizeInMb, 3));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Dxt Compressed Textures Count: ", MyPerformanceCounter.PerAppLifetime.DxtCompressedTexturesCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Non Dxt Compressed Textures Count: ", MyPerformanceCounter.PerAppLifetime.NonDxtCompressedTexturesCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Non Mip Mapped Textures Count: ", MyPerformanceCounter.PerAppLifetime.NonMipMappedTexturesCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Texture Cubes Count: ", MyPerformanceCounter.PerAppLifetime.TextureCubesCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Texture Cubes Size In Pixels: ", MyPerformanceCounter.PerAppLifetime.TextureCubesSizeInPixels));
            m_texts.Add(StringBuilderCache.GetFormatedDecimal("Texture Cubes Size In Mb: ", MyPerformanceCounter.PerAppLifetime.TextureCubesSizeInMb, 3));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Non MyModels Count: ", MyPerformanceCounter.PerAppLifetime.ModelsCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("MyModels Count: ", MyPerformanceCounter.PerAppLifetime.MyModelsCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("MyModels Meshes Count: ", MyPerformanceCounter.PerAppLifetime.MyModelsMeshesCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("MyModels Vertexes Count: ", MyPerformanceCounter.PerAppLifetime.MyModelsVertexesCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("MyModels Triangles Count: ", MyPerformanceCounter.PerAppLifetime.MyModelsTrianglesCount));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Size of Model Vertex Buffers in Mb: ", MyPerformanceCounter.PerAppLifetime.ModelVertexBuffersSize / 1024 / 1024));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Size of Model Index Buffers in Mb: ", MyPerformanceCounter.PerAppLifetime.ModelIndexBuffersSize / 1024 / 1024));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Size of Voxel Vertex Buffers in Mb: ", MyPerformanceCounter.PerAppLifetime.VoxelVertexBuffersSize / 1024 / 1024));
            m_texts.Add(StringBuilderCache.GetFormatedInt("Size of Voxel Index Buffers in Mb: ", MyPerformanceCounter.PerAppLifetime.VoxelIndexBuffersSize / 1024 / 1024));
            m_texts.Add(StringBuilderCache.GetFormatedBool("Paused: ", MyMinerGame.IsPaused()));
            MyGuiManager.GetInput().GetPressedKeys(m_pressedKeys);
            AddPressedKeys("Current keys              : ", m_pressedKeys);

            m_texts.Add(StringBuilderCache.Clear());
            m_texts.Add(m_frameDebugText);

            
            //This ensures constant max length of right block to avoid flickering when correct max line is changing too often
            m_frameDebugTextRA.AppendLine();
            m_frameDebugTextRA.Append("                                                                  ");
            ////

            m_rightAlignedtexts.Add(m_frameDebugTextRA);
              
            Vector2 origin = GetScreenLeftTopPosition();
            Vector2 rightAlignedOrigin = GetScreenRightTopPosition();

            for (int i = 0; i < m_texts.Count; i++)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_texts[i], origin + new Vector2(0, i * rowDistance), textScale,
                    Color.Yellow, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }
            for (int i = 0; i < m_rightAlignedtexts.Count; i++)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_rightAlignedtexts[i], rightAlignedOrigin + new Vector2(0.0f, i * rowDistance), textScale,
                    Color.Yellow, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);
            }

            ClearFrameDebugText();

            return true;
        }

        private void AddPressedKeys(string groupName, List<Toolkit.Input.Keys> keys)
        {
            var text = StringBuilderCache;
            text.Append(groupName);
            for (int i = 0; i < keys.Count; i++)
            {
                if (i > 0)
                {
                    text.Append(", ");
                }
                text.Append(MyKeysToString.GetKeyName((Keys)keys[i]));
            }
            m_texts.Add(text);
        }
    }
}
