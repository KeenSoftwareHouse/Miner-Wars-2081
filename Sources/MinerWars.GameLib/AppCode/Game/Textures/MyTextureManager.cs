using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using Timer = System.Timers.Timer;
using KeenSoftwareHouse.Library.Parallelization.Threading;

namespace MinerWars.AppCode.Game.Textures
{
    /// <summary>
    /// Reresent loading quality for textures.
    /// This works only for dds textures with mipmaps. Other textures will retains their original properties.
    /// </summary>
    internal enum TextureQuality
    {
        /// <summary>
        /// Full quality.
        /// </summary>
        Full,

        /// <summary>
        /// 1/2 quality.
        /// </summary>
        Half,

        /// <summary>
        /// 1/4 quality
        /// </summary>
        OneFourth,

        /// <summary>
        /// 1/8 quality
        /// </summary>
        OneEighth,

        /// <summary>
        /// 1/16 quality
        /// </summary>
        OneSixteenth,
    }

    /// <summary>
    /// Represent texture manager that handles all texture usage.
    /// </summary>
    internal static class MyTextureManager //: MyManager
    {
        /// <summary>
        /// Collection of managed texture.
        /// </summary>
        private static readonly Dictionary<string, MyTexture> m_textures;

        /// <summary>
        /// Queue of textures to load.
        /// </summary>
        private static readonly ConcurrentQueue<MyTexture> m_loadingQueue;

        /// <summary>
        /// Dbg watch send timer
        /// </summary>
        private static readonly Timer m_dbgSendTimer;

        /// <summary>
        /// Gets or sets the quality of loaded texturs. Selecting diffrent mipmaps as base level.
        /// </summary>
        /// <value>
        /// The quality.
        /// </value>
        public static TextureQuality Quality { get; set; }

        static ParallelTasks.Task m_loadTextureTask;


        static bool Enabled;
        public static LoadingMode? OverrideLoadingMode = null;

        public static FastResourceLock TexturesLock = new FastResourceLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        static MyTextureManager() 
        {
            Quality = TextureQuality.Half;
            m_textures = new Dictionary<string, MyTexture>();
            m_loadingQueue = new ConcurrentQueue<MyTexture>();
            m_dbgSendTimer = new Timer(1500);
            m_dbgSendTimer.Elapsed += DbgWatchLoadedTexturesDelayed;
            Enabled = true;
            
            //Task.Factory.StartNew(BackgroundLoader, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Gets and loads the texture.
        /// Texture is unloaded when nobody is using it.
        /// </summary>
        /// <typeparam name="T">MyTexture2D or MyTextureCube</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="loadedCallback">Callback that is invoked when texture is really loaded and ready for use.</param>
        /// <param name="loadingMode">The loading mode viz. LoadingMode.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static T GetTexture<T>(string path, TextureLoadedHandler loadedCallback = null, LoadingMode loadingMode = (MyFakes.LOAD_TEXTURES_IMMEDIATELY ? LoadingMode.Immediate : LoadingMode.Lazy)
            , TextureFlags flags = TextureFlags.None) where T : MyTexture
        {
            if (OverrideLoadingMode != null)
                loadingMode = OverrideLoadingMode.Value;
            
            MyTexture texture;

            TexturesLock.AcquireShared();
            
            if (!m_textures.TryGetValue(path, out texture))
            {
                TexturesLock.ReleaseShared();
                return LoadTexture<T>(path, loadedCallback, loadingMode, flags);
            }
            TexturesLock.ReleaseShared();

            DbgWatchLoadedTextures();

            return (T) texture;
        }

        /// <summary>
        /// Loads the texture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <param name="loadedCallback">The loaded callback.</param>
        /// <param name="loadingMode">The loading mode.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        private static T LoadTexture<T>(string path, TextureLoadedHandler loadedCallback, LoadingMode loadingMode, TextureFlags flags) where T : MyTexture
        {
            using (TexturesLock.AcquireExclusiveUsing())
            {
                MyTexture texture;
                if (typeof(T) == typeof(MyTexture2D))
                {
                    texture = new MyTexture2D(path, GetLoadMethod(loadingMode), flags);
                }
                else if (typeof(T) == typeof(MyTextureCube))
                {
                    texture = new MyTextureCube(path, GetLoadMethod(loadingMode), flags);
                }
                else
                {
                    throw new ArgumentException("Unsupported texture type", "T");
                }

                if (loadedCallback != null)
                {
                    texture.TextureLoaded += loadedCallback;
                }

                switch (loadingMode)
                {
                    case LoadingMode.Immediate:
                        {
                            if (!texture.Load(Quality))
                            {
                                return null;
                            }

                            break;
                        }
                    case LoadingMode.Background:
                        {
                            LoadTextureInBackground(texture);
                        }
                        break;
                }



                if (m_textures.ContainsKey(path))
                {
                    m_textures[path] = texture;
                }
                else
                {
                    m_textures.Add(path, texture);
                }

                DbgWatchLoadedTextures();

                return (T)texture;
            }
        }

        private static LoadMethod GetLoadMethod(LoadingMode loadingMode)
        {
            LoadMethod loadMethod;
            switch (loadingMode)
            {
                case LoadingMode.Lazy:
                    loadMethod = LoadMethod.Lazy;
                    break;
                case LoadingMode.LazyBackground:
                    loadMethod = LoadMethod.LazyBackground;
                    break;
                default:
                    loadMethod = LoadMethod.External;
                    break;
            }
            return loadMethod;
        }

        internal static void LoadTextureInBackground(MyTexture texture)
        {
            m_loadingQueue.Enqueue(texture);
            //m_loadTextureEvent.Set();

            ParallelTasks.Parallel.Start(LoadTextureInBackground);
        }


        static void LoadTextureInBackground()
        {
            MyTexture textureToLoad;
            if (m_loadingQueue.TryDequeue(out textureToLoad))
            {
                textureToLoad.Load(Quality);
            }
        }

        /// <summary>
        /// Reloads the textures.
        /// </summary>
        internal static void ReloadTextures(bool keepValidTextures = true)
        {
            MyMwcLog.WriteLine("ReloadTextures - START");

            if (!Enabled)
            {
                MyMwcLog.WriteLine("!Enabled - END");
                return;
            }

            
            using (TexturesLock.AcquireExclusiveUsing())            
            {
                List<MyTexture> texturesToLoad = new List<MyTexture>();

                foreach (var loadedTexture in m_textures)
                {
                    var texture = (MyTexture) loadedTexture.Value;

                    if (texture == null)
                    {
                        continue;
                    }

                    if (texture.LoadState != LoadState.Loaded)
                        continue;

                    if (keepValidTextures && texture.IgnoreQuality && texture.IsValid)
                    {
                        continue;
                    } 

                    texture.Unload();

                    texturesToLoad.Add(texture);
                }

                GC.Collect();

                foreach (var textureToLoad in texturesToLoad)
                {
                    textureToLoad.Load(Quality);
                }
            }

            DbgWatchLoadedTextures();

            MyMwcLog.WriteLine("ReloadTextures - END");
        }

        /// <summary>
        /// Unloads the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        internal static void UnloadTexture(MyTexture texture)
        {
            try
            {
                using (TexturesLock.AcquireExclusiveUsing())
                {
                    if (texture.LoadState == LoadState.Loaded)
                    {
                        texture.Unload();
                    }
                }
            }
            finally
            {
                DbgWatchLoadedTextures();
            }
        }

        /// <summary>
        /// DBGs Send loaded textures watch.
        /// </summary>
        [Conditional("DEBUGING_TEXTURE")]
        private static void DbgWatchLoadedTextures()
        {
            try
            {
                m_dbgSendTimer.Stop();
                m_dbgSendTimer.Start();
            }
            catch (ObjectDisposedException) {}
        }

        /// <summary>
        /// DBGs the update stats.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void DbgUpdateStats()
        {
            MyMwcLog.WriteLine("MyTextureManager::DbgUpdateStats - START");

            MyPerformanceCounter.PerAppLifetime.Textures2DCount = 0;
            MyPerformanceCounter.PerAppLifetime.Textures2DSizeInPixels = 0;
            MyPerformanceCounter.PerAppLifetime.Textures2DSizeInMb = 0;
            MyPerformanceCounter.PerAppLifetime.NonMipMappedTexturesCount = 0;
            MyPerformanceCounter.PerAppLifetime.NonDxtCompressedTexturesCount = 0;
            MyPerformanceCounter.PerAppLifetime.DxtCompressedTexturesCount = 0;
            MyPerformanceCounter.PerAppLifetime.TextureCubesCount = 0;
            MyPerformanceCounter.PerAppLifetime.TextureCubesSizeInPixels = 0;
            MyPerformanceCounter.PerAppLifetime.TextureCubesSizeInMb = 0;

            lock (m_textures)
            {
                foreach (var loadedTexture in m_textures)
                {
                    var texture = (MyTexture) loadedTexture.Value;

                    if (texture == null || !texture.IsValid || texture.LoadState != LoadState.Loaded)
                    {
                        continue;
                    }

                    var texture2D = texture as MyTexture2D;
                    if (texture2D != null)
                    {
                        MyPerformanceCounter.PerAppLifetime.Textures2DCount++;
                        MyPerformanceCounter.PerAppLifetime.Textures2DSizeInPixels += texture2D.Width*texture2D.Height;
                        MyPerformanceCounter.PerAppLifetime.Textures2DSizeInMb += (decimal) texture2D.Memory;

                        if (MyUtils.IsTextureMipMapped(texture2D) == false)
                        {
                            MyPerformanceCounter.PerAppLifetime.NonMipMappedTexturesCount++;
                        }

                        if (MyUtils.IsTextureDxtCompressed(texture2D) == false)
                        {
                            MyPerformanceCounter.PerAppLifetime.NonDxtCompressedTexturesCount++;
                        }
                        else
                        {
                            MyPerformanceCounter.PerAppLifetime.DxtCompressedTexturesCount++;
                        }
                    }

                    var textureCube = texture as MyTextureCube;
                    if (textureCube != null)
                    {
                        MyPerformanceCounter.PerAppLifetime.TextureCubesCount++;
                        MyPerformanceCounter.PerAppLifetime.TextureCubesSizeInPixels += textureCube.Size * textureCube.Size;
                        MyPerformanceCounter.PerAppLifetime.TextureCubesSizeInMb += (decimal)textureCube.Memory;
                    }
                }
            }

            MyMwcLog.WriteLine("MyTextureManager::DbgUpdateStats - END");
        }

        /// <summary>
        /// DBGs Send loaded textures watch (delayed)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsekdEventArgs"/> instance containing the event data.</param>
        private static void DbgWatchLoadedTexturesDelayed(object sender, ElapsedEventArgs e)
        {
            lock (m_textures)
            {
                float size = 0;
                foreach (var loadedTexture in m_textures)
                {
                    var texture = (MyTexture)loadedTexture.Value;

                    if (texture == null)
                    {
                        continue;
                    }

                    size += texture.Memory;
                }

                MyTrace.SendWatch("Textures size (MB)", size);
                MyTrace.SendWatch("Loaded textures", m_textures, 4);
            }
            
            m_dbgSendTimer.Stop();
        }

        [Conditional("DEBUG")]
        public static void DbgDumpLoadedTextures(bool orderedBySize = false)
        {
            lock (m_textures)
            {
                List<MyTexture> dump = new List<MyTexture>();
                float totalMemory = 0f;

                foreach (var loadedTexture in m_textures)
                {
                    var texture = (MyTexture)loadedTexture.Value;

                    if (texture == null)
                    {
                        continue;
                    }

                    dump.Add(texture);
                }
                
                if (orderedBySize)
                {
                    dump = dump.OrderByDescending(ri => ri.Memory).ToList();
                }
               
                foreach (var texture in dump)
                {
                    Debug.WriteLine(string.Format("{0} size: {1}MB", texture.Name, texture.Memory));
                    totalMemory += texture.Memory;
                }
                Debug.WriteLine("Total memory: " + totalMemory);
            }
        }


        static List<MyTexture> SortByMemory(List<MyTexture> stats)
        {
            return stats.OrderByDescending(ri => ri.Memory).ToList();
        }

        [Conditional("DEBUG")]
        public static void DbgDumpLoadedTexturesBetter(bool orderedBySize = false)
        {
            lock (m_textures)
            {
                List<MyTexture> dump = new List<MyTexture>();

                foreach (var loadedTexture in m_textures)
                {
                    var texture = (MyTexture)loadedTexture.Value;

                    if (texture == null)
                    {
                        continue;
                    }
                    if (texture.LoadState == LoadState.Loaded)
                    {
                        dump.Add(texture);
                    }
                }

                if (orderedBySize)
                {
                    dump = SortByMemory(dump);
                }

                foreach (var texture in dump)
                {
                    MyMwcLog.WriteLine(string.Format("{0} size: {1}MB", texture.Name, texture.Memory));
                }
            }
        }

        /*
        public static void DebugDrawStatistics()
        {
            lock (m_textures)
            {
                List<MyTexture> dump = new List<MyTexture>();

                foreach (var loadedTexture in m_textures)
                {
                    var texture = (MyTexture)loadedTexture.Value.Target;

                    if (texture == null)
                    {
                        continue;
                    }
                    if (texture.LoadState == LoadState.Loaded)
                    {
                        dump.Add(texture);
                    }
                }

                dump = SortByMemory(dump);

                float totalMem = 0;
                List<MyTexture> voxels = new List<MyTexture>();
                foreach (var texture in dump)
                {
                    if (texture.Name.ToLower().Contains("voxels"))
                    {
                        voxels.Add(texture);
                        totalMem += texture.Memory;
                    }
                }
                dump = voxels;

                float topOffset = 70;
                int itemsInRow = 35;
                Vector2 offset = new Vector2(100, topOffset);
                MyDebugDraw.DrawText(offset, new System.Text.StringBuilder("Textures statistics [" + voxels.Count + "x] " + totalMem +" MB"), Color.Yellow, 1.5f);

                float scale = 0.5f;
                offset.Y += 50;
                for (int i = 0; i < Math.Min(itemsInRow, dump.Count); i++)
                {
                    var texture = dump[i];
                    MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(texture.Name + ": " + texture.Memory + " MB"), Color.Yellow, scale);
                    offset.Y += 20;
                }

                offset = new Vector2(550, topOffset + 50);
                for (int i = itemsInRow; i < Math.Min(2*itemsInRow, dump.Count); i++)
                {
                    var texture = dump[i];
                    MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(texture.Name + ": " + texture.Memory + " MB"), Color.Yellow, scale);
                    offset.Y += 20;
                }

                offset = new Vector2(1000, topOffset + 50);
                for (int i = 2*itemsInRow; i < Math.Min(3 * itemsInRow, dump.Count); i++)
                {
                    var texture = dump[i];
                    MyDebugDraw.DrawText(offset, new System.Text.StringBuilder(texture.Name + ": " + texture.Memory + " MB"), Color.Yellow, scale);
                    offset.Y += 20;
                }

            }         
        }    */

        internal static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyTextureManager.UnloadContent - START");

            if (!Enabled)
            {
                MyMwcLog.WriteLine("!Enabled - END");
                return;
            }

            using (TexturesLock.AcquireExclusiveUsing())
            {
                foreach (var loadedTexture in m_textures)
                {
                    var texture = (MyTexture)loadedTexture.Value;

                    if (texture == null)
                    {
                        continue;
                    }

                    if (texture.LoadState != LoadState.Loaded)
                        continue;

                    texture.Unload();
                }

                GC.Collect();
            }

            MyMwcLog.WriteLine("MyTextureManager.UnloadContent - END");
        }
    }
}