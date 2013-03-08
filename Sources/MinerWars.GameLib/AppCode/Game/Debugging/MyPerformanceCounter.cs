using MinerWars.AppCode.Game.GUI;
using SysUtils.Utils;
using System.Text;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.World;
using System.Linq;
using System;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Collections.Generic;
using System.Diagnostics;

//  This class is used for measurements like drawn triangles, number of textures loaded, etc.
//  IMPORTANT: Use this class only for profiling / debuging. Don't use it for real game code.

namespace MinerWars.AppCode.Game.Managers
{
    static class MyPerformanceCounter
    {
        struct Timer
        {
            public static readonly Timer Empty = new Timer() { Runtime = 0, StartTime = long.MaxValue };

            public long StartTime;
            public long Runtime;
            //public string StackTrace;

            public bool IsRunning { get { return StartTime != long.MaxValue; } }
        }

        static Stopwatch m_timer = new Stopwatch();

        static MyPerformanceCounter()
        {
            m_timer.Start();
        }

        public static double TicksToMs(long ticks)
        {
            return ticks / (double)Stopwatch.Frequency * 1000.0;
        }

        public static long ElapsedTicks
        {
            get
            {
                return m_timer.ElapsedTicks;
            }
        }

        public const int NoSplit = MinerWars.AppCode.Game.Render.MyShadowRenderer.NumSplits;

        //  These counters are "reseted" before every camera draw
        public class MyPerCameraDraw
        {
            public int RenderCellsInFrustum_LOD0;        //  Count of render cells visible and drawn in the frustum (LOD0)
            public int RenderCellsInFrustum_LOD1;        //  Count of render cells visible and drawn in the frustum (LOD1)
            public int VoxelTrianglesInFrustum;          //  Count of really drawn voxel triangles visible and drawn in the frustum (if triangleVertexes is multi-textured, in this number is more times)
            public int EntitiesRendered;             //  Count of models visible and drawn in the frustum        
            public int ModelTrianglesInFrustum_LOD0;     //  Count of model triangles visible and drawn in the frustum LOD0
            public int ModelTrianglesInFrustum_LOD1;     //  Count of model triangles visible and drawn in the frustum LOD1   
            public int DecalsForVoxelsInFrustum;         //  Count of voxel decals visible and drawn in the frustum
            public int DecalsForEntitiesInFrustum;    //  Count of phys object decals visible and drawn in the frustum
            public int DecalsForCockipGlassInFrustum;    //  Count of glass cockpit decals visible and drawn in the frustum
            public int BillboardsInFrustum;              //  Count of billboards visible and drawn in the frustum
            public int BillboardsDrawCalls;              //  Count of billboard draw calls in the frustum (usually due to switching different particle textures or shaders)
            public int BillboardsSorted;              //  Count of sorted billboards
            public int OldParticlesInFrustum;               //  Count of particles visible and drawn in the frustum
            public int NewParticlesCount;               //  Count of new animated particles visible and drawn in the frustum
            public int ParticleEffectsTotal;            // Count of all living particle effects
            public int ParticleEffectsDrawn;            // Count of drawn particle effects
            public int EntitiesOccluded;                // Count of entities occluded by hw occ. queries
            public int[] ShadowEntitiesOccluded;        // Count of entities occluded by hw occ. queries
            public int QueriesCount;                    // Count of occlusion queries issued to GC
            public int LightsCount;                     //Count of lights rendered in frame
            public int RenderElementsInFrustum;         //Count of rendered elements in frustum
            public int RenderElementsIBChanges;         //Count of IB changes per frame
            public int RenderElementsInShadows;         //Count of rendered elements for shadows
            public int RayCastCount;                    // Count of ray casts
            public int RayCastModelsProcessed;          // Count of processed models by ray casts
            public int RayCastVoxelMapProcessed;        // Count of processed ray casts to voxel map
            public int RayCastTrianglesProcessed;       // Count of triangles processed by ray casts
            public int[] ShadowDrawCalls;
            public int TotalDrawCalls;

            // Per lod
            public int[] MaterialChanges;
            public int[] TechniqueChanges;
            public int[] VertexBufferChanges;
            public int[] EntityChanges;

            // Custom counters which can be added at runtime, useful when optimizing performance and using Edit&Continue
            public readonly Dictionary<string, int> CustomCounters = new Dictionary<string, int>(5);

                        // Custom timers which can be added at runtime, useful when profiling performance and using Edit&Continue
            readonly Dictionary<string, Timer> m_customTimers = new Dictionary<string, Timer>(5);

            readonly List<string> m_keys = new List<string>(5);

            public MyPerCameraDraw()
            {
                ShadowEntitiesOccluded = new int[MinerWars.AppCode.Game.Render.MyShadowRenderer.NumSplits + 1];
                ShadowDrawCalls = new int[MinerWars.AppCode.Game.Render.MyShadowRenderer.NumSplits + 1];

                int lodCount = MyMwcUtils.GetMaxValueFromEnum<MyLodTypeEnum>() + 1;

                MaterialChanges = new int[lodCount];
                TechniqueChanges = new int[lodCount];
                VertexBufferChanges = new int[lodCount];
                EntityChanges = new int[lodCount];
            }

            public void StartTimer(string name)
            {
                if (!MinerWars.AppCode.App.MyMinerGame.IsMainThread())
                    return;

                Timer t;
                bool exists = m_customTimers.TryGetValue(name, out t);
                Debug.Assert(!(exists && t.IsRunning), "Timer already started! Timers are no reentrant!");
                t.StartTime = m_timer.ElapsedTicks;
                m_customTimers[name] = t;
            }

            public void StopTimer(string name)
            {
                if (!MinerWars.AppCode.App.MyMinerGame.IsMainThread())
                    return;

                Timer t;
                if (m_customTimers.TryGetValue(name, out t))
                {
                    t.Runtime += m_timer.ElapsedTicks - t.StartTime;
                    t.StartTime = long.MaxValue;
                    m_customTimers[name] = t;
                }
            }

            public void Increment(string counterName)
            {
                if (!MinerWars.AppCode.App.MyMinerGame.IsMainThread())
                    return;

                int currentVal;
                if (CustomCounters.TryGetValue(counterName, out currentVal))
                    CustomCounters[counterName] = currentVal + 1;
                else
                    CustomCounters.Add(counterName, 1);
            }

            public int this[string counterName]
            {
                get
                {
                    if (!CustomCounters.ContainsKey(counterName))
                    {
                        return 0;
                    }
                    else
                    {
                        return CustomCounters[counterName];
                    }
                }
                set
                {
                    if (!CustomCounters.ContainsKey(counterName))
                    {
                        CustomCounters.Add(counterName, value);
                    }
                    else
                    {
                        CustomCounters[counterName] = value;
                    }
                }
            }

            public void Reset()
            {
                RenderCellsInFrustum_LOD1 = 0;
                RenderCellsInFrustum_LOD0 = 0;
                VoxelTrianglesInFrustum = 0;
                EntitiesRendered = 0;
                ModelTrianglesInFrustum_LOD0 = 0;
                ModelTrianglesInFrustum_LOD1 = 0;
                DecalsForVoxelsInFrustum = 0;
                DecalsForEntitiesInFrustum = 0;
                DecalsForCockipGlassInFrustum = 0;
                BillboardsInFrustum = 0;
                BillboardsDrawCalls = 0;
                BillboardsSorted = 0;
                OldParticlesInFrustum = 0;
                NewParticlesCount = 0;
                //EntitiesOccluded = 0;
                for (int i = 0; i < ShadowEntitiesOccluded.Length; i++)
                {
                    ShadowEntitiesOccluded[i] = 0;
                }
                QueriesCount = 0;
                LightsCount = 0;
                RenderElementsInFrustum = 0;
                RenderElementsIBChanges = 0;
                RenderElementsInShadows = 0;
                //RayCastCount = 0;
                //RayCastModelsProcessed = 0;
                //RayCastVoxelMapProcessed = 0;
                //RayCastTrianglesProcessed = 0;
                for (int i = 0; i < ShadowDrawCalls.Length; i++)
                {
                    ShadowDrawCalls[i] = 0;
                }

                for (int i = 0; i < MaterialChanges.Length; i++)
                {
                    MaterialChanges[i] = 0;
                    TechniqueChanges[i] = 0;
                    VertexBufferChanges[i] = 0;
                    EntityChanges[i] = 0;
                }

                TotalDrawCalls = 0;
            }

            private static StringBuilder StringBuilderCache
            {
                get { return MyGuiScreenDebugStatistics.StringBuilderCache; }
            }

            // Formats text into one line for cascades
            //private StringBuilder GetShadowText(string text, int[] sunValues)
            //{
            //    var sb = StringBuilderCache;
            //    sb.Clear();
            //    sb.ConcatFormat("{0}", text);
            //    for (int i = 0; i < sunValues.Length - 1; i++)
            //    {
            //        sb.Concat(sunValues[i], 4, ' ', 10, false);
            //        sb.ConcatFormat("{0}", ", ");
            //    }
            //    sb.Concat(sunValues[sunValues.Length - 1], 4, ' ', 10, false);
            //    return sb;
            //}

            private StringBuilder GetShadowText(string text, int cascade, int value)
            {
                var sb = StringBuilderCache;
                sb.Clear();
                sb.ConcatFormat("{0} (c {1}): ", text, cascade);
                sb.Concat(value);
                return sb;
            }

            private StringBuilder GetLodText(string text, int lod, int value)
            {
                var sb = StringBuilderCache;
                sb.Clear();
                sb.ConcatFormat("{0}_LOD{1}: ", text, lod);
                sb.Concat(value);
                return sb;
            }

            //  Show only draw/rendering statistics, because these counts are reseted in a Draw call
            public void AddToFrameDebugText(MyGuiScreenDebugStatistics debugScreen)
            {
                debugScreen.AddDebugTextRA("MyPerformanceCounter");
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RenderCellsInFrustum_LOD0: ", RenderCellsInFrustum_LOD0));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RenderCellsInFrustum_LOD1: ", RenderCellsInFrustum_LOD1));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   VoxelTrianglesInFrustum: ", VoxelTrianglesInFrustum));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   Entities rendered: ", EntitiesRendered));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   Entities occluded: ", EntitiesOccluded));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   Drawcalls: ", TotalDrawCalls));
                //debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   Shadow entities occluded: ", ShadowEntitiesOccluded));
                //debugScreen.AddDebugTextRA(GetShadowText("  Shadow entities occluded:", ShadowEntitiesOccluded));

                for (int i = 0; i < ShadowEntitiesOccluded.Length - 1; i++)
                {
                    debugScreen.AddDebugTextRA(GetShadowText("   Shadow entities occluded", i, ShadowEntitiesOccluded[i]));
                }
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   Shadow entities occluded (other):", ShadowEntitiesOccluded[ShadowEntitiesOccluded.Length - 1]));

                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   Queries count: ", QueriesCount));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   ModelTrianglesInFrustum_LOD0: ", ModelTrianglesInFrustum_LOD0));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   ModelTrianglesInFrustum_LOD1: ", ModelTrianglesInFrustum_LOD1));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   DecalsForVoxelsInFrustum: ", DecalsForVoxelsInFrustum));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   DecalsForEntitiesInFrustum: ", DecalsForEntitiesInFrustum));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   DecalsForCockipGlassInFrustum: ", DecalsForCockipGlassInFrustum));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   BillboardsInFrustum: ", BillboardsInFrustum));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   BillboardsDrawCalls: ", BillboardsDrawCalls));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   BillboardsSorted: ", BillboardsSorted));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   OldParticlesInFrustum: ", OldParticlesInFrustum));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   ParticleEffects total: ", ParticleEffectsTotal));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   ParticleEffects drawn: ", ParticleEffectsDrawn));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   NewParticles count: ", NewParticlesCount));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   Lights count: ", LightsCount));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RenderElementsInFrustum: ", RenderElementsInFrustum));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RenderElementsIBChanges: ", RenderElementsIBChanges));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RenderElementsInShadows: ", RenderElementsInShadows));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RayCastCount: ", RayCastCount));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RayCastModelsProcessed: ", RayCastModelsProcessed));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RayCastVoxelMapProcessed: ", RayCastVoxelMapProcessed));
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   RayCastTrianglesProcessed: ", RayCastTrianglesProcessed));
                //debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   ShadowDrawCalls: ", ShadowDrawCalls));
                //debugScreen.AddDebugTextRA(GetShadowText("  ShadowDrawCalls:", ShadowDrawCalls));
                for (int i = 0; i < ShadowDrawCalls.Length - 1; i++)
                {
                    debugScreen.AddDebugTextRA(GetShadowText("   ShadowDrawCalls", i, ShadowDrawCalls[i]));
                }
                debugScreen.AddDebugTextRA(StringBuilderCache.GetFormatedInt("   ShadowDrawCalls (other):", ShadowDrawCalls[ShadowDrawCalls.Length - 1]));

                debugScreen.AddDebugTextRA("Render states");
                for (int i = 0; i < MaterialChanges.Length; i++)
                {
                    int lodNum;
                    var lod = (MyLodTypeEnum)i;
                    if (lod == MyLodTypeEnum.LOD0)
                        lodNum = 0;
                    else if (lod == MyLodTypeEnum.LOD1)
                        lodNum = 1;
                    else
                        continue;

                    debugScreen.AddDebugTextRA(GetLodText("   MaterialChanges", lodNum, MaterialChanges[i]));
                    debugScreen.AddDebugTextRA(GetLodText("   TechniqueChanges", lodNum, TechniqueChanges[i]));
                    debugScreen.AddDebugTextRA(GetLodText("   VertexBufferChanges", lodNum, VertexBufferChanges[i]));
                    debugScreen.AddDebugTextRA(GetLodText("   EntityChanges", lodNum, EntityChanges[i]));
                }

                RayCastCount = 0;
                RayCastModelsProcessed = 0;
                RayCastVoxelMapProcessed = 0;
                RayCastTrianglesProcessed = 0;
            }

            public void AppendCustomCounters(StringBuilder builder)
            {
                foreach (var custom in CustomCounters)
                {
                    builder.Append(custom.Key);
                    builder.Append(": ");
                    builder.AppendInt32(custom.Value);
                    builder.AppendLine();
                }

                m_keys.Clear();
                foreach (var custom in CustomCounters)
                {
                    m_keys.Add(custom.Key);
                }
                foreach (var item in m_keys)
                {
                    CustomCounters[item] = 0;
                }
            }

            public void AppendCustomTimers(StringBuilder builder, bool reset = true)
            {
                foreach (var custom in m_customTimers)
                {
                    builder.Append(custom.Key);
                    builder.Append(": ");
                    builder.AppendDecimal((float)TicksToMs(custom.Value.Runtime), 3);
                    builder.Append(" ms");
                    builder.AppendLine();
                }

                if (reset)
                {
                    m_keys.Clear();
                    foreach (var custom in m_customTimers)
                    {
                        m_keys.Add(custom.Key);
                    }
                    foreach (var key in m_keys)
                    {
                        m_customTimers[key] = Timer.Empty;
                    }
                }
            }
        }

        //  These counters are "reseted" before every game-play screen load
        public class MyPerGamePlayScreen
        {
        }

        //  These counters are never "reseted", they keep increasing during the whole app lifetime
        public class MyPerAppLifetime
        {
            //  Texture2D loading statistics
            public int Textures2DCount;            //  Total number of all loaded textures since application start (it will increase after game-screen load)
            public int Textures2DSizeInPixels;     //  Total number of pixels in all loaded textures (it will increase after game-screen load)
            public decimal Textures2DSizeInMb;         //  Total size in Mb of all loaded textures (it will increase after game-screen load)
            public int NonMipMappedTexturesCount;
            public int NonDxtCompressedTexturesCount;
            public int DxtCompressedTexturesCount;

            //  TextureCube loading statistics
            public int TextureCubesCount;            //  Total number of all loaded textures since application start (it will increase after game-screen load)
            public int TextureCubesSizeInPixels;     //  Total number of pixels in all loaded textures (it will increase after game-screen load)
            public decimal TextureCubesSizeInMb;         //  Total size in Mb of all loaded textures (it will increase after game-screen load)

            //  Model loading statistics (this is XNA's generic model - not the one we use)
            public int ModelsCount;

            //  MyModel loading statistics (this is our custom model class)
            public int MyModelsCount;
            public int MyModelsMeshesCount;
            public int MyModelsVertexesCount;
            public int MyModelsTrianglesCount;

            // Sizes of model and voxel buffers
            public int ModelVertexBuffersSize;          // Size of model vertex buffers in bytes
            public int ModelIndexBuffersSize;           // Size of model index buffers in bytes
            public int VoxelVertexBuffersSize;          // Size of voxel vertex buffers in bytes
            public int VoxelIndexBuffersSize;           // Size of voxel index buffers in bytes
        }


        public static readonly MyPerCameraDraw PerCameraDraw = new MyPerCameraDraw();
        public static readonly MyPerGamePlayScreen PerGamePlayScreen = new MyPerGamePlayScreen();
        public static readonly MyPerAppLifetime PerAppLifetime = new MyPerAppLifetime();
    }
}
