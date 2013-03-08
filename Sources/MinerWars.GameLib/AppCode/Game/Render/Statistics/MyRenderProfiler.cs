#region Using

using System;
using System.Text;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using SysUtils.Utils;
using KeenSoftwareHouse.Library.Profiler;
using MinerWars.AppCode.Toolkit.Input;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI;
using System.Threading;

#endregion


namespace MinerWars.AppCode.Game.Render
{
    class MyRenderProfiler
    {
        static KeenSoftwareHouse.Library.Profiler.MyProfiler.MyProfilerBlock m_fpsBlock = new KeenSoftwareHouse.Library.Profiler.MyProfiler.MyProfilerBlock("FPS");
        
        static Color[] m_colors = { Color.Aqua, Color.Orange, Color.BlueViolet, Color.BurlyWood, Color.Chartreuse,
                                  Color.CornflowerBlue, Color.Cyan, Color.ForestGreen, Color.Fuchsia,
                                  Color.Gold, Color.GreenYellow, Color.LightBlue, Color.LightGreen, Color.LimeGreen,
                                  Color.Magenta, Color.Navy, Color.Orchid, Color.PeachPuff, Color.Purple };

        public static bool Paused = false;

        static Dictionary<Thread, MyProfiler> m_threadProfilers = new Dictionary<Thread, MyProfiler>();
        static MyProfiler m_selectedProfiler;
        static int m_selectedProfilerIndex = -1;
        static Thread m_selectedThread;

        static MyLineBatch m_lineBatch = null;
        static bool m_enabled = false;
        static int m_index = 0;     // Index of newest frame, so we know where to add new values.
        static int m_selectedFrame = 0;   // Index of selected frame. It will be showed in text legend.
        static int m_levelLimit = -1;
        static bool m_useCustomFrame = false;

        

        static float memoryRange = 0.001f; // cca. 1KB
        static float m_milisecondRange = 25;

        static float m_stackCheckingDuration = 0;

        static object m_lock = new object();

        static Color UintToColor(uint number)
        {
            Color color = new Color();
            color.PackedValue = number;
            return color;
        }

        public void DrawPerfEvents()
        {    
            Vector3 v0 = Vector3.Zero;
            Vector3 v1 = Vector3.Zero;

            float x_start = 0.5f;
            float y_start = 0;
            float x_scale = (2f - x_start) / 2f;
            float y_scale = 0.9f;
                    
            float invMilisecondRange = 1f / m_milisecondRange;
            
            // Draw graphs for selected events
            foreach (MyProfiler.MyProfilerBlock profilerBlock in m_selectedProfiler.ProfilingBlocks.Values)
            {
                if (!profilerBlock.DrawGraph)
                    continue;

                for (int i = 1; i < MyProfiler.MAX_FRAMES; i++)
                {
                    v0.X = -1.0f + x_start + x_scale * (i - 1) / 512.0f;
                    v0.Y = profilerBlock.Miliseconds[i - 1] * y_scale * invMilisecondRange;
                    v0.Z = 0;

                    v1.X = -1.0f + x_start + x_scale * i / 512.0f;
                    v1.Y = profilerBlock.Miliseconds[i] * y_scale * invMilisecondRange;
                    v1.Z = 0;

                    if (v0.Y > 1e-3f || v1.Y > 1e-3f)
                        m_lineBatch.DrawOnScreenLine(v0, v1, UintToColor(profilerBlock.color));
                }
            }         

                     
            // Draw legend
            float x_legend_start = x_start - 0.1f;
            float x_legend_ms_size = 0.01f;
            m_lineBatch.DrawOnScreenLine(new Vector3(-1.0f + x_legend_start, 0, 0), new Vector3(-1.0f + x_legend_start, y_scale, 0), Color.Silver);

            const int legendMsCount = 5;

            StringBuilder text = new StringBuilder(10);

            float viewportWidth = MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.Viewport.Width;
            float viewportHeight = MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.Viewport.Height;

            for (int i = 0; i <= legendMsCount; i++)
            {
                m_lineBatch.DrawOnScreenLine(new Vector3(-1.0f + x_legend_start, y_scale * (float)i / legendMsCount, 0),
                                            new Vector3(-1.0f + x_legend_start + x_legend_ms_size, y_scale * (float)i / legendMsCount, 0), Color.Silver);

                // Miliseconds legend
                text.Clear();
                text.Append((i * m_milisecondRange / legendMsCount).ToString());
                MyDebugDraw.DrawText(new Vector2(0.5f * viewportWidth * x_legend_start - 25f + 3 * x_legend_ms_size, -10 + 0.5f * viewportHeight - y_scale * 0.5f * viewportHeight * ((float)i / legendMsCount)), text, Color.Silver, 0.7f);
            }

            text.Clear();
            text.Append("[ms]");
            MyDebugDraw.DrawText(new Vector2(0.5f * viewportWidth * x_legend_start - 25f + 3 * x_legend_ms_size, -10 + 0.5f * viewportHeight - y_scale * 0.5f * viewportHeight * 1.05f), text, Color.Silver, 0.7f);

            // Memory legend
            x_legend_start = x_start + 1.48f;
            x_start = 0.5f;
            y_start = -0.7f;
            x_scale = (2f - x_start) / 2f;
            
            x_legend_ms_size = -0.01f;
            y_scale = 0.6f;

            for (int i = 0; i <= legendMsCount; i++)
            {
                text.Clear();
                text.Append((i * memoryRange / legendMsCount).ToString("#,###.###000"));
                MyDebugDraw.DrawText(new Vector2(0.5f * viewportWidth * x_legend_start - 25f + 3 * x_legend_ms_size,
                    -10 + 0.85f * viewportHeight - y_scale * 0.5f * viewportHeight * ((float)i / legendMsCount)), 
                    text, Color.Yellow, 0.7f);

                if (i == 0)
                {
                    text.Clear();
                    text.Append("[MB]");
                    MyDebugDraw.DrawText(new Vector2(0.5f * viewportWidth * x_legend_start - 25f + 3 * x_legend_ms_size,
                        -30 + 0.85f * viewportHeight - y_scale * 0.5f * viewportHeight * ((float)i / legendMsCount)),
                        text, Color.Yellow, 0.7f);
                }
            }

            float invMemoryRange = 1f / memoryRange;
            
            foreach (MyProfiler.MyProfilerBlock profilerBlock in m_selectedProfiler.ProfilingBlocks.Values)
            {
                if (!profilerBlock.DrawGraph)
                    continue;

                // process memory
                Color processColor = UintToColor(profilerBlock.color);

#if MEMORY_PROFILING
                for (int i = 1; i < MyProfiler.MAX_FRAMES; i++)
                {
                    v0.X = -1.0f + x_start + x_scale * (i - 1) / 512.0f;
                    v0.Y = y_start + profilerBlock.ProcessMemory[i - 1] * y_scale * invMemoryRange;
                    v0.Z = 0;

                    v1.X = -1.0f + x_start + x_scale * i / 512.0f;
                    v1.Y = y_start + profilerBlock.ProcessMemory[i] * y_scale * invMemoryRange;
                    v1.Z = 0;

                    if (v0.Y - y_start > 1e-3f || v1.Y - y_start > 1e-3f)
                        lineBatch.DrawOnScreenLine(v0, v1, processColor);
                }
#else 
                

                // managed memory
                Color managedColor = UintToColor(profilerBlock.color);
                for (int i = 1; i < MyProfiler.MAX_FRAMES; i++)
                {
                    v0.X = -1.0f + x_start + x_scale * (i - 1) / 512.0f;
                    v0.Y = y_start + profilerBlock.ManagedMemory[i - 1] * y_scale * invMemoryRange;
                    v0.Z = 0;

                    v1.X = -1.0f + x_start + x_scale * i / 512.0f;
                    v1.Y = y_start + profilerBlock.ManagedMemory[i] * y_scale * invMemoryRange;
                    v1.Z = 0;

                    //if (v0.Y - y_start > 1e-3f || v1.Y - y_start > 1e-3f)
                    m_lineBatch.DrawOnScreenLine(v0, v1, managedColor);
                }
#endif

            }

            // Draw selected frame
            if (m_useCustomFrame)
            {
                if (m_selectedFrame >= 0 && m_selectedFrame < MyProfiler.MAX_FRAMES)
                {
                    v0.X = -1.0f + x_start + x_scale * (m_selectedFrame) / 512.0f;
                    v0.Y = y_start;
                    v0.Z = 0;

                    v1.X = v0.X;
                    v1.Y = 0.9f;
                    v1.Z = 0;

                    m_lineBatch.DrawOnScreenLine(v0, v1, Color.Yellow);
                }
            }     
        }

        public static void HandleInput(MyGuiInput input)
        {
            if (input.IsAnyAltPress())
            {
                for (int i = 0; i <= 9; i++)
                {
                    var key = (Keys)((int)Keys.NumPad0 + i);
                    if (input.IsNewKeyPress(key))
                    {
                        PressedKey(key, input.IsAnyCtrlKeyPressed());
                    }
                }

                if (input.IsNewKeyPress(Keys.Add))
                    PressedKey(Keys.Add);

                if (input.IsNewKeyPress(Keys.Subtract))
                    PressedKey(Keys.Subtract);

                if (input.IsNewKeyPress(Keys.Enter))
                    PressedKey(Keys.Enter);

                if (input.IsNewKeyPress(Keys.Delete))
                    PressedKey(Keys.Delete);

                if (input.IsKeyPress(Keys.PageDown))
                    MyRenderProfiler.PreviousFrame();

                if (input.IsKeyPress(Keys.PageUp))
                    MyRenderProfiler.NextFrame();

                if (input.IsKeyPress(Keys.Multiply))
                    PressedKey(Keys.Multiply);

                if (input.IsKeyPress(Keys.Divide))
                    PressedKey(Keys.Divide);

                if ((((input.IsKeyPress(Keys.PageDown)) ||
                    (input.IsKeyPress(Keys.PageUp))) && input.IsAnyCtrlKeyPressed())
                    ||
                    (input.IsKeyPress(Keys.Multiply) || input.IsKeyPress(Keys.Divide))
                    )

                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }


        public static void PressedKey(Keys key, bool isCtrlPressed = false)
        {
            int index = key - Keys.NumPad0;
            if (index >= 0 && index <= 9)
            {
                if (isCtrlPressed)
                    index += 10;

                // Enable or Disable profiler drawing
                if (m_enabled && m_selectedProfiler.SelectedRoot == null)
                {
                    if (index == 0)
                    {
                        m_enabled = false;
                        m_useCustomFrame = false;
                        return;
                    }
                }
                else if (!m_enabled && index == 0)
                {
                    m_enabled = true;
                    return;
                }

                // Enter child node
                if (index >= 1 && index <= 19)
                {
                    if (m_selectedProfiler.SelectedRoot == null)
                    {
                        int cnt = 0;
                        foreach (MyProfiler.MyProfilerBlock profilerBlock in m_selectedProfiler.ProfilingBlocks.Values)
                        {
                            if (profilerBlock.Parent == null)
                            {
                                cnt++;

                                if (index == cnt)
                                {
                                    m_selectedProfiler.SelectedRoot = profilerBlock;
                                    return;
                                }
                            }
                        }
                    }
                    else if (m_selectedProfiler.SelectedRoot.Children.Count >= index)
                    {
                        int cnt = 0;
                        int flagIndex = 0;
                        foreach (MyProfiler.MyProfilerBlock profilerBlock in m_selectedProfiler.SelectedRoot.Children)
                        {
                            cnt++;
                            //if ((profilerBlock.flag & (int)filterMask) != 0)
                            {
                                flagIndex++;
                                if (index == flagIndex)
                                {
//                                     m_selectedRoot = profilerBlock;
//                                     return;

                                    m_selectedProfiler.SelectedRoot = m_selectedProfiler.SelectedRoot.Children[cnt - 1];
                                    return;
                                }
                            }
                        }
                        
                    }
                }

                // Go to parent node
                if (index == 0 && m_selectedProfiler.SelectedRoot != null)
                {
                    m_selectedProfiler.SelectedRoot = m_selectedProfiler.SelectedRoot.Parent;
                }
            }

            if (key == Keys.Enter)
            {
                Paused = !Paused;
                m_useCustomFrame = false; // Turf-off custom frame after ALT + ENTER
            }

            if(key == Keys.Delete)
            {
                // This will enable stack checking for StartProfilingblock and EndProfilingblock for some period of time.
                // It will check whether the StartProfilingblock and EndProfilingblock is called within the same function.
                
                m_stackCheckingDuration = 1;    // set duration to 1s 
            }

            if (key == Keys.Add)
            {
                List<MyProfiler> profilers = new List<MyProfiler>();
                foreach (var t in m_threadProfilers.Values)
                {
                    profilers.Add(t);
                }

                int profilerIndex = profilers.IndexOf(m_selectedProfiler);
                profilerIndex++;
                if (profilerIndex >= profilers.Count)
                    profilerIndex = 0;

                int i = 0;
                foreach (var t in m_threadProfilers)
                {
                    if (i == profilerIndex)
                    {
                        m_selectedProfiler = t.Value;
                        m_selectedThread = t.Key;
                        m_selectedProfilerIndex = i;
                        break;
                    }
                    i++;
                }
            }

            if (key == Keys.Subtract)
            {
                List<MyProfiler> profilers = new List<MyProfiler>();
                foreach (var t in m_threadProfilers.Values)
                {
                    profilers.Add(t);
                }

                int profilerIndex = profilers.IndexOf(m_selectedProfiler);
                profilerIndex--;
                if (profilerIndex < 0)
                    profilerIndex = profilers.Count - 1;

                int i = 0;
                foreach (var t in m_threadProfilers)
                {
                    if (i == profilerIndex)
                    {
                        m_selectedProfiler = t.Value;
                        m_selectedThread = t.Key;
                        m_selectedProfilerIndex = i;
                        break;
                    }
                    i++;
                }
            }

            if (key == Keys.Multiply)
            {
                m_levelLimit++;
            }

            if (key == Keys.Divide)
            {
                m_levelLimit--;
                if (m_levelLimit < -1)
                    m_levelLimit = -1;
            }
        }

        static public void PreviousFrame()
        {
            m_useCustomFrame = true;

            m_selectedFrame--;
            if (m_selectedFrame < 0)
                m_selectedFrame = MyProfiler.MAX_FRAMES - 1;
        }

        static public void NextFrame()
        {
            m_useCustomFrame = true;

            m_selectedFrame++;
            if (m_selectedFrame >= MyProfiler.MAX_FRAMES)
                m_selectedFrame = 0;
        }

        public void DrawEvent(int textOffsetY, MyProfiler.MyProfilerBlock profilerBlock, int index = -1)
        {
            float Y_TEXT_POSITION = MyMinerGame.Static.GraphicsDevice.Viewport.Height / 2;

            float textScale = 0.7f;

            StringBuilder text = new StringBuilder(100);
            text.Clear();
            text.Append((textOffsetY + 1).ToString("0 "));
            text.Append(profilerBlock.Name);

            MyDebugDraw.DrawText(new Vector2(20, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);

            float length = 500;

            text.Clear();
            text.Append(profilerBlock.Children.Count.ToString("(0) "));
            MyDebugDraw.DrawText(new Vector2(20 + length, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);
            length += 50 * textScale;

            text.Clear();
            //text.Append(((index != -1 ? profilerBlock.TimePercentage[index] : profilerBlock.averagePctg)).ToString("#,#0.0%"));
            //MyDebugDraw.DrawText(new Vector2(20 + length, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);
            length += 155 * textScale;

            text.Clear();
            text.Append((index != -1 ? profilerBlock.Miliseconds[index] : profilerBlock.averageMiliseconds).ToString("#,##0.00ms"));
            MyDebugDraw.DrawText(new Vector2(20 + length, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);
            length += 155 * textScale;

            text.Clear();
            text.Append((index != -1 ? profilerBlock.ManagedMemory[index] : profilerBlock.TotalManagedMB).ToString("#,###0.000 GC"));
            MyDebugDraw.DrawText(new Vector2(20 + length, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);
            length += 40 + 158 * textScale;

            text.Clear();
#if MEMORY_PROFILING
            text.Append((index != -1 ? profilerBlock.ProcessMemory[index] : profilerBlock.totalProcessMB).ToString("#,###0.000 MB"));
            MyDebugDraw.DrawText(new Vector2(20 + length, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);
            length += 158 * textScale;

            text.Clear();
#endif

            length += 40 + 40 * textScale;
            text.Append(profilerBlock.NumCallsArray[index != -1 ? index : 0].ToString());
            text.Append(profilerBlock.NumCalls.ToString(" calls"));
            MyDebugDraw.DrawText(new Vector2(20 + length, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);

            length += 150 * textScale;
            text.Clear();
            text.Append("Custom: ");
            text.Append((index != -1 ? profilerBlock.CustomValues[index] : profilerBlock.CustomValue).ToString("#,###0.000"));
            MyDebugDraw.DrawText(new Vector2(20 + length, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);
            



//             if (profilerBlock.NumCalls > 1)
//                 text.Append("s");

            length += MyDebugDraw.DrawText(new Vector2(20 + length, Y_TEXT_POSITION + textOffsetY * 20), text, UintToColor(profilerBlock.color), textScale);
        }

        [Conditional("RENDER_PROFILING")]
        public void Draw()
        {
            if (!m_enabled)
                return;

            if (MyProfiler.EnableAsserts)
                MyCommonDebugUtils.AssertDebug(m_selectedProfiler.CurrentProfilingStack.Count == 0, "Stack size must be 0!");

            // Init linebatch
            if (m_lineBatch == null)
            {            
                SharpDX.Direct3D9.Device device = MyMinerGame.Static.GraphicsDevice;
                m_lineBatch = new MyLineBatch(
                    Matrix.Identity,
                    Matrix.CreateOrthographicOffCenter(0.0F, device.Viewport.Width, device.Viewport.Height, 0.0F, 0.0F, -1.0F),
                    50000);
                           
                m_fpsBlock.Stopwatch.Start();
            }


            // Handle FPS timer
            m_fpsBlock.End(false);

            float elapsedTime = (float)m_fpsBlock.Stopwatch.Elapsed.TotalSeconds;
            float invElapsedTime = elapsedTime > 0 ? 1 / elapsedTime : 0;
            m_fpsBlock.averagePctg = 0.9f * m_fpsBlock.averagePctg + 0.1f * invElapsedTime;

#if MEMORY_PROFILING
            // Handle memory usage for frame
            float processDeltaMB = m_fpsBlock.ProcessDeltaMB;
            m_fpsBlock.ProcessMemory[m_index] = processDeltaMB;
#endif

            float managedDeltaMB = m_fpsBlock.ManagedDeltaMB;
            m_fpsBlock.ManagedMemory[m_index] = managedDeltaMB;
            m_fpsBlock.CustomValues[m_index] = m_fpsBlock.CustomValue;

            m_fpsBlock.Stopwatch.Reset();
            
            m_fpsBlock.Start(false);

            int numCalls = 0;  // number of calls of StartProfilingBlock

            // Add new measured time to each graph
            if (!Paused)
            {
                foreach (MyProfiler.MyProfilerBlock profilerBlock in m_selectedProfiler.ProfilingBlocks.Values)
                {
                    float dt;

                    profilerBlock.ManagedMemory[m_index] = profilerBlock.ManagedDeltaMB;
#if MEMORY_PROFILING
                    profilerBlock.ProcessMemory[m_index] = profilerBlock.ProcessDeltaMB;
#endif
                    profilerBlock.NumCallsArray[m_index] = profilerBlock.NumCalls;
                    profilerBlock.CustomValues[m_index] = profilerBlock.CustomValue;

                    dt = (float)profilerBlock.Stopwatch.Elapsed.TotalSeconds * 1000f;

                    profilerBlock.Miliseconds[m_index] = (float)profilerBlock.Stopwatch.Elapsed.TotalMilliseconds;
                    float pctg = (0.001f * dt) * invElapsedTime;
                    profilerBlock.averagePctg = 0.9f * profilerBlock.averagePctg + (0.1f * pctg);
                    profilerBlock.averagePctg = System.Math.Min(profilerBlock.averagePctg, 1.0f); // block cannot take more than 100% of elapsed fram time
                    profilerBlock.averageMiliseconds = 0.9f * profilerBlock.averageMiliseconds + 0.1f * (float)profilerBlock.Stopwatch.Elapsed.TotalMilliseconds;
                    profilerBlock.Stopwatch.Reset();

                    if (profilerBlock.ManagedDeltaMB > memoryRange)
                        memoryRange = 2 * profilerBlock.ManagedDeltaMB;

#if MEMORY_PROFILING
                    if (profilerBlock.ProcessDeltaMB > memoryRange)
                    {
                        memoryRange = 2 * profilerBlock.ProcessDeltaMB;
                    }
#endif
                    numCalls += profilerBlock.NumCalls;
                    profilerBlock.NumChildCalls = profilerBlock.GetNumChildCalls();
                }
            }

            if (m_enabled)
            {
                // Draw events as text 
                float Y_TEXT_POSITION = MyMinerGame.Static.GraphicsDevice.Viewport.Height / 2;
                StringBuilder text = new StringBuilder(100);
                int textOffsetY = 0;

                if (m_selectedProfiler.SelectedRoot == null)
                {
                    foreach (MyProfiler.MyProfilerBlock profilerBlock in m_selectedProfiler.ProfilingBlocks.Values)
                    {
                        if (profilerBlock.Parent != null)
                            continue;

                        profilerBlock.color = textOffsetY < m_colors.Length ? m_colors[textOffsetY].PackedValue : Color.Green.PackedValue;

                        if (profilerBlock.NumCalls == 0)
                            profilerBlock.Cooldown--;
                        else
                            profilerBlock.Cooldown = 1000;

                        profilerBlock.DrawGraph = true;

                        if(m_useCustomFrame)
                            DrawEvent(textOffsetY, profilerBlock, m_selectedFrame);
                        else
                            DrawEvent(textOffsetY, profilerBlock, Paused ? (m_selectedFrame - 1) % MyProfiler.MAX_FRAMES : m_selectedFrame);

                        textOffsetY++;
                    }
                }
                else
                {
                    for (int i = 0; i < m_selectedProfiler.SelectedRoot.Children.Count; i++)
                    {
                        MyProfiler.MyProfilerBlock profilerBlock = m_selectedProfiler.SelectedRoot.Children[i];

                        profilerBlock.color = i < m_colors.Length ? m_colors[i].PackedValue : Color.Green.PackedValue;

                        if (profilerBlock.NumCalls == 0)
                            profilerBlock.Cooldown--;
                        else
                            profilerBlock.Cooldown = 1000;

                        profilerBlock.DrawGraph = true;

                        if (m_useCustomFrame)
                            DrawEvent(textOffsetY, profilerBlock, m_selectedFrame);
                        else
                            DrawEvent(textOffsetY, profilerBlock);
                        textOffsetY++;
                    }
                }

                // Draw thread name
                text.Clear();
                text.Append("Thread (" + m_selectedProfilerIndex + "/" + m_threadProfilers.Count + "): " + m_selectedThread.Name);
                text.Append(", Level limit: " + m_levelLimit.ToString());
                MyDebugDraw.DrawText(new Vector2(20, Y_TEXT_POSITION + (-2) * 20), text, Color.Gray, 1);

                // Draw fps
                text.Clear();
                text.Append(m_fpsBlock.Name);
                if (m_useCustomFrame && m_selectedFrame >= 0 && m_selectedFrame < MyProfiler.MAX_FRAMES)
                {
                    //text.Append(m_fpsBlock.TimePercentage[SelectedFrame].ToString(" #,###0.000"));
                    MyDebugDraw.DrawText(new Vector2(20, Y_TEXT_POSITION + (-1) * 20), text, Color.Red, 1);
                }
                else
                {
                    text.Append(m_fpsBlock.averagePctg.ToString(" #,###0.000"));
                    MyDebugDraw.DrawText(new Vector2(20, Y_TEXT_POSITION + (-1) * 20), text, Color.Red, 1);
                }
                

                // Total calls
                text.Clear();
                text.Append(numCalls.ToString("Total calls 0"));
                MyDebugDraw.DrawText(new Vector2(20, Y_TEXT_POSITION + (textOffsetY) * 20), text, Color.Red, 1);

                if (m_useCustomFrame)
                {
                    text.Clear();
                    text.Append(m_selectedFrame.ToString("Selected frame: 0"));
                    MyDebugDraw.DrawText(new Vector2(20, Y_TEXT_POSITION + 2*(textOffsetY) * 20), text, Color.Yellow, 1);
                }

                // Handle stack checking
                if (m_stackCheckingDuration > 0)
                {
                    text.Clear();
                    text.Append("Checking Profiler Stack");
                    MyDebugDraw.DrawText(new Vector2(20, Y_TEXT_POSITION + (-3) * 20), text, Color.Orange, 1);

                    m_stackCheckingDuration -= elapsedTime;
                    m_stackCheckingDuration = Math.Max(m_stackCheckingDuration, 0);

                    MyProfiler.StackChecking = true;   // enable checking for next frame
                }
                else
                    MyProfiler.StackChecking = false;
                
                                    
                // Draw graphs
                m_lineBatch.Begin();
                DrawPerfEvents();
                m_lineBatch.End();
            }

            // Update horizontal offset
            if (!Paused)
            {
                m_index = (++m_index) % MyProfiler.MAX_FRAMES;

                if (!m_useCustomFrame)
                    m_selectedFrame = m_index;
            }

            // Reset childs before next run
            foreach (MyProfiler.MyProfilerBlock profilerBlock in m_selectedProfiler.ProfilingBlocks.Values)
            {
                profilerBlock.StackChildren.Clear();
                profilerBlock.StackParent = null;
                profilerBlock.Stopwatch.Reset();
                profilerBlock.DrawGraph = false;
                profilerBlock.NumCalls = 0;
                profilerBlock.NumChildCalls = -1; // -1 means it needs to be computed first.

                profilerBlock.StartManagedMB = 0;
                profilerBlock.EndManagedMB = 0;
                profilerBlock.DeltaManagedB = 0;

#if MEMORY_PROFILING
                profilerBlock.startProcessMB = 0;
                profilerBlock.endProcessMB = 0;
                profilerBlock.deltaProcessB = 0;
#endif

                profilerBlock.CustomValue = 0;
            }
        }

        [Conditional("RENDER_PROFILING")]
        public void StartProfilingBlock(string name)
        {
            int blockId = -1;
            StartProfilingBlock(name, ref blockId);
        }

        [Conditional("RENDER_PROFILING")]
        public void StartProfilingBlock(string name, ref int blockId)
        {
            lock (m_lock)
            {
                if (!m_threadProfilers.ContainsKey(Thread.CurrentThread))
                    m_threadProfilers.Add(Thread.CurrentThread, new MyProfiler());

                MyProfiler currentProfiler = m_threadProfilers[Thread.CurrentThread];

                if (m_selectedProfiler == null)
                {
                    m_selectedProfiler = currentProfiler;
                    m_selectedThread = Thread.CurrentThread;
                    m_selectedProfilerIndex = 0;
                }

                if (m_levelLimit != -1 && currentProfiler.CurrentProfilingStack.Count >= m_levelLimit)
                {
                    currentProfiler.LevelSkipCount++;
                    return;
                }

#if MEMORY_PROFILING
                blockId = currentProfiler.StartMyProfilingBlock(name, true);
#else
                blockId = currentProfiler.StartMyProfilingBlock(name, false);
#endif
            }
        }

        [Conditional("RENDER_PROFILING")]
        public void EndProfilingBlock(int id = -1)
        {
            lock (m_lock)
            {
#if MEMORY_PROFILING
                m_threadProfilers[Thread.CurrentThread].EndMyProfilingBlock(id, true);
#else
                m_threadProfilers[Thread.CurrentThread].EndMyProfilingBlock(id, false);
#endif
            }
        }

        // same as EndProfilingBlock(); StartProfilingBlock(string name);
        [Conditional("RENDER_PROFILING")]
        public void StartNextBlock(string name)
        {
            lock (m_lock)
            {
#if MEMORY_PROFILING
                EndMyProfilingBlock(-1, true);
                StartMyProfilingBlock(name, true);
#else
                EndProfilingBlock();
                StartProfilingBlock(name);
#endif
            }
        }

        [Conditional("RENDER_PROFILING")]
        public void InitMemoryHack(string name)
        {
            lock (m_lock)
            {
                StartProfilingBlock(name);
                EndProfilingBlock();

                MyProfiler.MyProfilerBlock profilingBlock;
                if (m_threadProfilers[Thread.CurrentThread].ProfilingBlocks.TryGetValue(name.GetHashCode(), out profilingBlock))
                {
                    profilingBlock.StartManagedMB = 0;
                    profilingBlock.EndManagedMB = System.GC.GetTotalMemory(true);

#if MEMORY_PROFILING
                    profilingBlock.startProcessMB = 0;
                    profilingBlock.endProcessMB = System.Environment.WorkingSet;
#endif
                }
            }
        }

        [Conditional("RENDER_PROFILING")]
        public void ProfileCustomValue(string name, float value)
        {
            if (!m_threadProfilers.ContainsKey(Thread.CurrentThread))
                m_threadProfilers.Add(Thread.CurrentThread, new MyProfiler());

            if (m_levelLimit != -1)
            {
                return;
            }

#if MEMORY_PROFILING
            MyProfileCustomValue(name, value, true);
#else
            m_threadProfilers[Thread.CurrentThread].MyProfileCustomValue(name, value, false);
#endif
        }
    }
}