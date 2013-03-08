#define USE_SERIAL_MODEL_LOAD

using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Entities;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System;
using MinerWars.AppCode.Game.Utils;


namespace MinerWars.AppCode.Game.Models
{
    static partial class MyModels
    {
        static MyModel[] m_models;
        static Dictionary<string, MyModel> m_modelsByAssertName;

        //Enables entity objects to do custom work after content is loaded (ie. device reset)
        public delegate void ContentLoadedDelegate();
        public static event ContentLoadedDelegate OnContentLoaded;

        /// <summary>
        /// Queue of textures to load.
        /// </summary>
        private static readonly ConcurrentQueue<MyModel> m_loadingQueue;

        /// <summary>
        /// Event that occures when some model needs to be loaded.
        /// </summary>
        private static readonly AutoResetEvent m_loadModelEvent;


        static MyModels()
        {
            m_loadingQueue = new ConcurrentQueue<MyModel>();
            m_loadModelEvent = new AutoResetEvent(false);
            //Task.Factory.StartNew(BackgroundLoader, TaskCreationOptions.LongRunning);

            InitModels();
        }

        /// <summary>
        /// Backgrounds the loader.
        /// </summary>
        private static void BackgroundLoader()
        {
            while (true)
            {
                try
                {
                    MyModel modelToLoadInDraw;
                    if (m_loadingQueue.TryDequeue(out modelToLoadInDraw))
                    {
                        modelToLoadInDraw.LoadInDraw(Managers.LoadingMode.Immediate);
                    }
                    else
                    {
                        m_loadModelEvent.WaitOne();
                    }
                }
                catch (ObjectDisposedException)
                {
                    // NOTE: This will happend when game is disposed while loading so skip load of this model.
                }
            }
        }

        internal static void LoadModelInDrawInBackground(MyModel model)
        {
            if (MyFakes.LOAD_MODELS_IMMEDIATELY)
            {
                model.LoadInDraw(Managers.LoadingMode.Immediate);
            }
            else
            {
                m_loadingQueue.Enqueue(model);
                m_loadModelEvent.Set();
            }
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyModels.LoadData");
            MyMwcLog.WriteLine(string.Format("MyModels.LoadData - START"));

            MyMwcLog.WriteLine(string.Format("MyModels.LoadData - END"));
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        public static void ReloadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyModels.ReloadData");
            MyMwcLog.WriteLine(string.Format("MyModels.ReloadData - START"));

            if (m_models != null)
            {
                List<int> loadedModels = new List<int>();
#if USE_SERIAL_MODEL_LOAD
                for (int i = 0; i < m_models.Length; i++)
                {
                    if (m_models[i].UnloadData())
                        loadedModels.Add(i);
                }
#else
                Parallel.For(0, m_models.Length, i =>
                {
                    if (m_models[i].UnloadData())
                        loadedModels.Add(i);
                });
#endif
                //load only previously loaded models
#if USE_SERIAL_MODEL_LOAD
                foreach (int i in loadedModels)
                {
                    m_models[i].LoadData();
                }
#else
                Parallel.ForEach(loadedModels, i =>
                {
                    m_models[i].LoadData();
                });
#endif
            }

            if (MyEntities.GetEntities() != null)
            {
                foreach (MyEntity entity in MyEntities.GetEntities())
                {
                    entity.InitDrawTechniques();
                }
            }

            MyMwcLog.WriteLine(string.Format("MyModels.ReloadData - END"));
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        public static void ReloadContent()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyModels.ReloadData");
            MyMwcLog.WriteLine(string.Format("MyModels.ReloadData - START"));

            if (m_models != null)
            {
                List<int> contentloadedModels = new List<int>();
                for (int i = 0; i < m_models.Length; i++)
                {
                    if (m_models[i].LoadedContent)
                        contentloadedModels.Add(i);
                }

                ReloadData();

                foreach (int i in contentloadedModels)
                {
                    MyModel model = MyModels.GetModelForDraw((MyModelsEnum)i);
                }
            }

            MyMwcLog.WriteLine(string.Format("MyModels.ReloadData - END"));
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyModels.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyModels::LoadContent");

            if (OnContentLoaded != null)
                OnContentLoaded();

            if (m_models != null)
            {
                for (int i = 0; i < m_models.Length; i++)
                {
                    if (m_models[i] != null)
                    {
                        m_models[i].LoadContent();
                    }
                }

            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyModels.LoadContent() - END");
        }

        public static void UnloadData()
        {
            if (m_models != null)
            {
                for (int i = 0; i < m_models.Length; i++)
                {
                    if (m_models[i] != null)
                    {
                        // Unload data which was previously loaded
                        m_models[i].UnloadData();
                    }
                }
            }
        }

        public static void UnloadContent()
        {
            if (m_models != null)
            {
                for (int i = 0; i < m_models.Length; i++)
                {
                    if (m_models[i] != null)
                    {
                        m_models[i].UnloadContent();
                    }
                }
            }
        }


        //	Special method that loads data into GPU, and can be called only from Draw method, never from LoadContent or from background thread.
        //	Because that would lead to empty vertex/index buffers if they are filled/created while game is minimized 
        //  (remember the issue - alt-tab during loading screen)
        public static void LoadInDraw()
        {
            MyMwcLog.WriteLine("MyModels.LoadInDraw - START");
            MyMwcLog.IncreaseIndent();

            for (int i = 0; i < m_models.Length; i++)
            {
                //  Because this LoadInDraw will stop normal update calls, we might not be able to send keep alive
                //  messages to server for some time. This will help it - it will make networking be up-to-date.
                //MyClientServer.Update();

                MyModel model = m_models[i];
                if (model != null)
                {
                    //  We can call this on every model because MyModel.LoadInDraw checks if model's data
                    //  were loaded, and that can happen only if some phys object used it for its initialization
                    //  Summary: here only models that are used by some phys object are loaded
                    model.LoadInDraw();
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyModels.LoadInDraw - END");
        }

        public static void UnloadExcept(HashSet<int> keepModels)
        {
            if (m_models != null)
            {
                for (int i = 0; i < m_models.Length; i++)
                {
                    if (m_models[i] != null /*&& !m_models[i].KeepInMemory*/ && !keepModels.Contains((int)m_models[i].ModelEnum))
                    {
                        // Unload data which was previously loaded
                        m_models[i].UnloadContent();
                        if(m_models[i].UnloadData())
                        {
                            MyMwcLog.WriteLine("Unloading model: " + m_models[i].AssetName);
                        }
                    }
                }
            }
        }

        //  Lazy-loading and then returning reference to model
        //  Doesn't load vertex/index shader and doesn't touch GPU. Use it when you need model data - vertex, triangles, octre...
        public static MyModel GetModelOnlyData(MyModelsEnum modelEnum)
        {
            int modelInt = (int)modelEnum;
            m_models[modelInt].LoadData();
            return m_models[modelInt];
        }

        //  Lazy-loading and then returning reference to model
        //  Doesn't load vertex/index shader and doesn't touch GPU. Use it when you need model data - vertex, triangles, octre...
        public static MyModel GetModelOnlyDummies(MyModelsEnum modelEnum)
        {
            int modelInt = (int)modelEnum;
            m_models[modelInt].LoadOnlyDummies();
            return m_models[modelInt];
        }

        public static MyModel GetModelOnlyModelInfo(MyModelsEnum modelEnum)
        {
            int modelInt = (int)modelEnum;
            m_models[modelInt].LoadOnlyModelInfo();
            return m_models[modelInt];
        }

        //  Lazy-loading and then returning reference to model
        //  IMPORTANT: Loads data into GPU, should be called only from Draw method on main thread
        //	Special method that loads data into GPU, and can be called only from Draw method, never from LoadContent or from background thread.
        //	Because that would lead to empty vertex/index buffers if they are filled/created while game is minimized (remember the issue - alt-tab during loading screen)
        public static MyModel GetModelForDraw(MyModelsEnum modelEnum)
        {       /*
            System.Collections.Generic.List<MyModel> mds = new System.Collections.Generic.List<MyModel>();
            foreach (MyModel model in m_models)
            {
                if (model.Vertexes != null)
                mds.Add(model);
            }
              */

            int modelInt = (int)modelEnum;
            m_models[modelInt].LoadData();
            m_models[modelInt].LoadInDraw();
            return m_models[modelInt];
        }

        // Returns asset name of model
        public static string GetModelAssetName(MyModelsEnum modelEnum)
        {
            return m_models[(int)modelEnum].AssetName;
        }

        public static MyModel GetModel(MyModelsEnum modelEnum)
        {
            return m_models.Length > (int)modelEnum ? m_models[(int)modelEnum] : null;
        }

        //  Release textures, vertex buffer and all content related to this model. It's good to use it only
        //  for models that have their own texture (texture not shared through many models). E.g. cockpit glass.
        public static void ReleaseModel(MyModelsEnum modelEnum)
        {
            //  TODO!!!
        }

        public static MyModelsEnum GetModelEnumByAssetName(string assetName)
        {
            string correctAssetName = assetName.Replace(".FBX", string.Empty);
            if (correctAssetName.StartsWith("\\"))
            {
                correctAssetName = correctAssetName.Remove(0, 1);
            }

            return m_modelsByAssertName[correctAssetName].ModelEnum;
        }

        //for use in LogUsageInformationFunction
        private static Dictionary<MyModelsEnum, int> m_counts;
        private static List<WLentry> m_wrong_models;

        private static void ParseEntityChildrensForLog(KeenSoftwareHouse.Library.Collections.ObservableCollection<MyEntity> entityCollection)
        {
            if (entityCollection == null) return;
            foreach (MyEntity e in entityCollection)
            {
                if (MyModelsStatisticsConstants.MODEL_STATISTICS_WRONG_LODS_ONLY)
                {
                    if (e.ModelLod0 != null && e.ModelLod1 != null && e.ModelLod0.GetTrianglesCount() < e.ModelLod1.GetTrianglesCount() * 2)
                    {
                        bool founded = false;
                        foreach (WLentry entry in m_wrong_models)
                        {
                            if (entry.lod1Asset == e.ModelLod1.AssetName && entry.lod0Asset == e.ModelLod0.AssetName)
                            {
                                founded = true;
                                break;
                            }
                        }
                        if (!founded)
                        {
                            WLentry entry = new WLentry();
                            entry.lod1Asset = e.ModelLod1.AssetName;
                            entry.lod1Triangles = MyModels.m_modelsByAssertName[e.ModelLod1.AssetName].GetTrianglesCount().ToString();
                            entry.lod1Type = "LOD1";
                            entry.lod0Asset = e.ModelLod0.AssetName;
                            entry.lod0Triangles = MyModels.m_modelsByAssertName[e.ModelLod0.AssetName].GetTrianglesCount().ToString();
                            entry.lod0Type = "LOD0";
                            m_wrong_models.Add(entry);
                        }
                    }
                    if (e.ModelLod1 != null && e.ModelLod2 != null && e.ModelLod1.GetTrianglesCount() < e.ModelLod2.GetTrianglesCount() * 2)
                    {
                        bool founded = false;
                        foreach (WLentry entry in m_wrong_models)
                        {
                            if (entry.lod1Asset == e.ModelLod2.AssetName && entry.lod0Asset == e.ModelLod1.AssetName)
                            {
                                founded = true;
                                break;
                            }
                        }
                        if (!founded)
                        {
                            WLentry entry = new WLentry();
                            entry.lod1Asset = e.ModelLod2.AssetName;
                            entry.lod1Triangles = MyModels.m_modelsByAssertName[e.ModelLod2.AssetName].GetTrianglesCount().ToString();
                            entry.lod1Type = "LOD2";
                            entry.lod0Asset = e.ModelLod1.AssetName;
                            entry.lod0Triangles = MyModels.m_modelsByAssertName[e.ModelLod1.AssetName].GetTrianglesCount().ToString();
                            entry.lod0Type = "LOD1";
                            m_wrong_models.Add(entry);
                        }
                    }
                    if (e.ModelLod0 != null && e.ModelCollision != null && e.ModelLod0.GetTrianglesCount() < e.ModelCollision.GetTrianglesCount() * 2)
                    {
                        bool founded = false;
                        foreach (WLentry entry in m_wrong_models)
                        {
                            if (entry.lod1Asset == e.ModelCollision.AssetName && entry.lod0Asset == e.ModelLod0.AssetName)
                            {
                                founded = true;
                                break;
                            }
                        }
                        if (!founded)
                        {
                            WLentry entry = new WLentry();
                            entry.lod1Asset = e.ModelCollision.AssetName;
                            entry.lod1Triangles = MyModels.m_modelsByAssertName[e.ModelCollision.AssetName].GetTrianglesCount().ToString();
                            entry.lod1Type = "COL";
                            entry.lod0Asset = e.ModelLod0.AssetName;
                            entry.lod0Triangles = MyModels.m_modelsByAssertName[e.ModelLod0.AssetName].GetTrianglesCount().ToString();
                            entry.lod0Type = "LOD0";
                            m_wrong_models.Add(entry);
                        }
                    }
                }
                if (e.ModelLod0 != null)
                {
                    MyModelsEnum en = e.ModelLod0.ModelEnum;
                    if (m_counts.ContainsKey(en)) ++m_counts[en];
                    else m_counts[en] = 1;
                }
                if (e.ModelLod1 != null)
                {
                    MyModelsEnum en = e.ModelLod1.ModelEnum;
                    if (m_counts.ContainsKey(en)) ++m_counts[en];
                    else m_counts[en] = 1;
                }
                if (e.ModelLod2 != null)
                {
                    MyModelsEnum en = e.ModelLod2.ModelEnum;
                    if (m_counts.ContainsKey(en)) ++m_counts[en];
                    else m_counts[en] = 1;
                }
                if (e.ModelCollision != null)
                {
                    MyModelsEnum en = e.ModelCollision.ModelEnum;
                    if (m_counts.ContainsKey(en)) ++m_counts[en];
                    else m_counts[en] = 1;
                }
                ParseEntityChildrensForLog(e.Children);
            }
        }

        public static void LogUsageInformation()
        {
            if (MinerWars.AppCode.Game.Missions.MyMissions.ActiveMission == null) return;
            m_counts = new Dictionary<MyModelsEnum, int>();
            m_wrong_models = new List<WLentry>();
            foreach (MyEntity e in MyEntities.GetEntities())
            {
                if (MyModelsStatisticsConstants.MODEL_STATISTICS_WRONG_LODS_ONLY)
                {
                    if (e.ModelLod0 != null && e.ModelLod1 != null && e.ModelLod0.GetTrianglesCount() < e.ModelLod1.GetTrianglesCount() * 2)
                    {
                        bool founded = false;
                        foreach (WLentry entry in m_wrong_models)
                        {
                            if (entry.lod1Asset == e.ModelLod1.AssetName && entry.lod0Asset == e.ModelLod0.AssetName)
                            {
                                founded = true;
                                break;
                            }
                        }
                        if (!founded)
                        {
                            WLentry entry = new WLentry();
                            entry.lod1Asset = e.ModelLod1.AssetName;
                            entry.lod1Triangles = MyModels.m_modelsByAssertName[e.ModelLod1.AssetName].GetTrianglesCount().ToString();
                            entry.lod1Type = "LOD1";
                            entry.lod0Asset = e.ModelLod0.AssetName;
                            entry.lod0Triangles = MyModels.m_modelsByAssertName[e.ModelLod0.AssetName].GetTrianglesCount().ToString();
                            entry.lod0Type = "LOD0";
                            m_wrong_models.Add(entry);
                        }
                    }
                    if (e.ModelLod1 != null && e.ModelLod2 != null && e.ModelLod1.GetTrianglesCount() < e.ModelLod2.GetTrianglesCount() * 2)
                    {
                        bool founded = false;
                        foreach (WLentry entry in m_wrong_models)
                        {
                            if (entry.lod1Asset == e.ModelLod2.AssetName && entry.lod0Asset == e.ModelLod1.AssetName)
                            {
                                founded = true;
                                break;
                            }
                        }
                        if (!founded)
                        {
                            WLentry entry = new WLentry();
                            entry.lod1Asset = e.ModelLod2.AssetName;
                            entry.lod1Triangles = MyModels.m_modelsByAssertName[e.ModelLod2.AssetName].GetTrianglesCount().ToString();
                            entry.lod1Type = "LOD2";
                            entry.lod0Asset = e.ModelLod1.AssetName;
                            entry.lod0Triangles = MyModels.m_modelsByAssertName[e.ModelLod1.AssetName].GetTrianglesCount().ToString();
                            entry.lod0Type = "LOD1";
                            m_wrong_models.Add(entry);
                        }
                    }
                    if (e.ModelLod0 != null && e.ModelCollision != null && e.ModelLod0.GetTrianglesCount() < e.ModelCollision.GetTrianglesCount() * 2)
                    {
                        bool founded = false;
                        foreach (WLentry entry in m_wrong_models)
                        {
                            if (entry.lod1Asset == e.ModelCollision.AssetName && entry.lod0Asset == e.ModelLod0.AssetName)
                            {
                                founded = true;
                                break;
                            }
                        }
                        if (!founded)
                        {
                            WLentry entry = new WLentry();
                            entry.lod1Asset = e.ModelCollision.AssetName;
                            entry.lod1Triangles = MyModels.m_modelsByAssertName[e.ModelCollision.AssetName].GetTrianglesCount().ToString();
                            entry.lod1Type = "COL";
                            entry.lod0Asset = e.ModelLod0.AssetName;
                            entry.lod0Triangles = MyModels.m_modelsByAssertName[e.ModelLod0.AssetName].GetTrianglesCount().ToString();
                            entry.lod0Type = "LOD0";
                            m_wrong_models.Add(entry);
                        }
                    }
                }
                if (e.ModelLod0 != null)
                {
                    MyModelsEnum en = e.ModelLod0.ModelEnum;
                    if (m_counts.ContainsKey(en)) ++m_counts[en];
                    else m_counts[en] = 1;
                }
                if (e.ModelLod1 != null)
                {
                    MyModelsEnum en = e.ModelLod1.ModelEnum;
                    if (m_counts.ContainsKey(en)) ++m_counts[en];
                    else m_counts[en] = 1;
                }
                if (e.ModelLod2 != null)
                {
                    MyModelsEnum en = e.ModelLod2.ModelEnum;
                    if (m_counts.ContainsKey(en)) ++m_counts[en];
                    else m_counts[en] = 1;
                }
                if (e.ModelCollision != null)
                {
                    MyModelsEnum en = e.ModelCollision.ModelEnum;
                    if (m_counts.ContainsKey(en)) ++m_counts[en];
                    else m_counts[en] = 1;
                }
                ParseEntityChildrensForLog(e.Children);
            }

            foreach (MyModel m in MyModels.m_models)
            {
                if (!m_counts.ContainsKey(m.ModelEnum)) m_counts[m.ModelEnum] = 0;
            }

            string activeMissionName = MinerWars.AppCode.Game.Missions.MyMissions.ActiveMission != null ? MinerWars.AppCode.Game.Missions.MyMissions.ActiveMission.DebugName.ToString() : MyModelsStatisticsConstants.ACTUAL_MISSION_FOR_MODEL_STATISTICS.ToString();
            activeMissionName = activeMissionName.Replace(' ', '-').ToString().ToLower();

            if (MyModelsStatisticsConstants.MODEL_STATISTICS_WRONG_LODS_ONLY)
            {
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(System.IO.File.Open("models-usage-statistics-WL_" + activeMissionName + ".csv", System.IO.FileMode.Create)))
                {
                    output.WriteLine("lodX AssetName" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX Triangles" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX Type" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX-1 AssetName" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX-1 Triangles" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX-1 Type");
                    foreach (WLentry entry in m_wrong_models)
                    {
                        output.WriteLine(entry.lod1Asset + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod1Triangles + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod1Type + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod0Asset + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod0Triangles + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod0Type);
                    }
                    output.Flush();
                    output.Close();
                }
            }

            using (System.IO.StreamWriter output = new System.IO.StreamWriter(System.IO.File.Open("models-usage-statistics-N_" + activeMissionName + ".csv", System.IO.FileMode.Create)))
            {
                output.WriteLine("Model name" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "Used X-times" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "VB size [MB]" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "IB size [MB]" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "LoadData/LoadContent");
                foreach (KeyValuePair<MyModelsEnum, int> kvp in m_counts)
                {
                    string assetName = MyModels.GetModelAssetName(kvp.Key);
                    string LS;
                    if (!MyModels.m_modelsByAssertName[assetName].LoadedData)
                    {
                        continue;
                        LS = "X";
                    }
                    else
                    {
                        if (!MyModels.m_modelsByAssertName[assetName].LoadedContent)
                        {
                            LS = "LD";
                        }
                        else
                        {
                            LS = "LC";
                        }
                    }
                    output.WriteLine(assetName + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + kvp.Value.ToString() + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + Math.Round(MyModels.m_modelsByAssertName[assetName].GetVBSize / 1024.0 / 1024.0, 4).ToString() + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + Math.Round(MyModels.m_modelsByAssertName[assetName].GetIBSize / 1024.0 / 1024.0, 4).ToString() + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + LS);
                }
                output.Flush();
                output.Close();
            }
            m_counts = null;
            GC.Collect();

            if (MyModelsStatisticsConstants.GET_MODEL_STATISTICS_AUTOMATICALLY && MyModelsStatisticsConstants.MISSIONS_TO_GET_MODEL_STATISTICS_FROM.Length > (++MyModelsStatisticsConstants.ACTUAL_MISSION_FOR_MODEL_STATISTICS + 1))
            {
                GUI.MyGuiScreenMainMenu.StartMission(MyModelsStatisticsConstants.MISSIONS_TO_GET_MODEL_STATISTICS_FROM[MyModelsStatisticsConstants.ACTUAL_MISSION_FOR_MODEL_STATISTICS]);
            }
            else
            {
                MergeLogFiles();
            }
        }

        struct WLentry
        {
            public string lod0Asset;
            public string lod1Asset;
            public string lod0Triangles;
            public string lod1Triangles;
            public string lod0Type;
            public string lod1Type;
        }

        public static void MergeLogFiles()
        {
            List<WLentry> entriesList = new List<WLentry>();
            string[] files = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath));
            foreach (string file in files)
            {
                if (IsNeededFile(file))
                {
                    using (System.IO.StreamReader input = new System.IO.StreamReader(System.IO.File.Open(file, System.IO.FileMode.Open)))
                    {
                        bool firstLine = true;
                        while (input.Peek() >= 0)
                        {
                            string line = input.ReadLine();
                            if (firstLine)
                            {
                                firstLine = false;
                                continue;
                            }
                            string[] collumns = line.Split(MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR);
                            System.Diagnostics.Debug.Assert(collumns.Length == 6, "Wrong columns count: " + collumns.Length);
                            bool founded = false;
                            foreach (WLentry entry in entriesList)
                            {
                                if (entry.lod1Asset == collumns[0] && entry.lod0Asset == collumns[3])
                                {
                                    founded = true;
                                    break;
                                }
                            }
                            if (!founded)
                            {
                                WLentry newEntry = new WLentry();
                                newEntry.lod1Asset = collumns[0];
                                newEntry.lod1Triangles = collumns[1];
                                newEntry.lod1Type = collumns[2];
                                newEntry.lod0Asset = collumns[3];
                                newEntry.lod0Triangles = collumns[4];
                                newEntry.lod0Type = collumns[5];
                                entriesList.Add(newEntry);
                            }
                        }
                        input.Close();
                    }
                }
            }

            using (System.IO.StreamWriter output = new System.IO.StreamWriter(System.IO.File.Open("models-usage-statistics-WL_ALL-MISSIONS-MERGED.csv", System.IO.FileMode.Create)))
            {
                output.WriteLine("lodX AssetName" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX Triangles" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX Type" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX-1 AssetName" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX-1 Triangles" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "lodX-1 Type");
                foreach (WLentry entry in entriesList)
                {
                    output.WriteLine(entry.lod1Asset + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod1Triangles + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod1Type + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod0Asset + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod0Triangles + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + entry.lod0Type);
                }
                output.Flush();
                output.Close();
            }
        }

        private static bool IsNeededFile(string file)
        {
            if (System.IO.Path.GetExtension(file) != ".csv") return false;

            string fileName = System.IO.Path.GetFileNameWithoutExtension(file);

            string[] parts1 = fileName.Split('_');

            if (parts1.Length != 2) return false;

            string[] parts2 = parts1[0].Split('-');

            if (parts2.Length != 4) return false;

            return parts2[3] == "WL";
        }

        public static void CheckAllModels()
        {
            Dictionary<string, MyModel> modelsLod0 = new Dictionary<string, MyModel>();
            Dictionary<string, MyModel> modelsLod1 = new Dictionary<string, MyModel>();
            Dictionary<string, MyModel> modelsCol = new Dictionary<string, MyModel>();
            foreach (MyModel model in m_models)
            {
                model.LoadOnlyModelInfo();
                string smallAsset = model.AssetName;
                if (smallAsset.EndsWith("_COL"))
                {
                    modelsCol.Add(smallAsset.Substring(0, smallAsset.Length - 4), model);
                }
                else if (smallAsset.EndsWith("_LOD1"))
                {
                    modelsLod1.Add(smallAsset.Substring(0, smallAsset.Length - 5), model);
                }
                else
                {
                    modelsLod0.Add(smallAsset, model);
                }
            }
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(System.IO.File.Open("All_models.csv", System.IO.FileMode.Create)))
            {
                output.WriteLine("lod0 AssetName" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "LOD0 Triangles" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "LOD1 AssetName" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "LOD1 Triangles" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "COL AssetName" + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + "COL Triangles");

                foreach (KeyValuePair<string, MyModel> lod0 in modelsLod0)
                {
                    string part2 = MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR.ToString(), part3 = MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR.ToString();
                    if (modelsLod1.ContainsKey(lod0.Key))
                    {
                        MyModel lod1 = modelsLod1[lod0.Key];
                        if (lod0.Value.ModelInfo.TrianglesCount < lod1.ModelInfo.TrianglesCount * 4 && lod1.ModelInfo.TrianglesCount > 50)
                        {
                            part2 = lod1.AssetName + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + lod1.ModelInfo.TrianglesCount;
                        }
                    }
                    if (modelsCol.ContainsKey(lod0.Key))
                    {
                        MyModel col = modelsCol[lod0.Key];
                        if (lod0.Value.ModelInfo.TrianglesCount < col.ModelInfo.TrianglesCount * 4 && col.ModelInfo.TrianglesCount > 50)
                        {
                            part3 = col.AssetName + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + col.ModelInfo.TrianglesCount;
                        }
                    }

                    if (part2 != MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR.ToString() || part3 != MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR.ToString())
                    {
                        output.WriteLine(lod0.Value.AssetName + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + lod0.Value.ModelInfo.TrianglesCount + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + part2 + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + part3);
                    }
                }
                output.WriteLine();
                foreach (KeyValuePair<string, MyModel> lod0 in modelsLod0)
                {
                    if ((!modelsLod1.ContainsKey(lod0.Key) || !modelsCol.ContainsKey(lod0.Key)) && lod0.Value.ModelInfo.TrianglesCount > 50)
                    {
                        if (lod0.Value.AssetName.Contains("Prefab"))
                        {
                            output.WriteLine(lod0.Value.AssetName + MyModelsStatisticsConstants.MODEL_STATISTICS_CSV_SEPARATOR + lod0.Value.ModelInfo.TrianglesCount);
                        }
                    }
                }
                output.Flush();
                output.Close();
            }
        }

    }
}