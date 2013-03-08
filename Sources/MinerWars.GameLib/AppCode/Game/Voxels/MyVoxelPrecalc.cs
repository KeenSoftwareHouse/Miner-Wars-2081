using System.Collections.Generic;
//using System.Threading;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Collections.Concurrent;
using ParallelTasks;

//  This class server for precalculating voxels to triangles. Primary voxels from data cells, but also is used for converting whole data cells when LOD is calculated.
//  This class is static and uses all available cores to work in parallel, thus if user has quad core, work is calculated in four parallel threads.
//  This class works in single-core mode (if machine has only 1 core) or in multi-core mode (using worked threads)
//  In multi-core mode this class creates and starts parallel threads (e.g. four), sets them into waiting. Then when work is needed to do, queue Tasks is filled
//  with task and we send signal to all threads so they wake up, do they work (go over queue, do each job). When they are done, every thread signal he is done and goes to waiting again.
//  Method PrecalcQueue() finishes only after all threads are finished and queue is completely done (empty).
//
//  IMPORTANT: This class assumess all other classes are used in read-only mode (e.g. traversing voxels).
//  
//  Thread - represents classical C# thread
//  Tasks - piece of work to do, e.g. calculate one data cell


namespace MinerWars.AppCode.Game.Voxels
{
    struct MyVoxelPrecalcTaskItem
    {
        public MyLodTypeEnum Type;
        public MyVoxelMap VoxelMap;
        public MyVoxelCacheCellData Cache;
        public MyMwcVector3Int VoxelStart;

        public MyVoxelPrecalcTaskItem(MyLodTypeEnum type, MyVoxelMap voxelMap, MyVoxelCacheCellData cache, MyMwcVector3Int voxelStart)
        {
            Type = type;
            VoxelMap = voxelMap;
            Cache = cache;
            VoxelStart = voxelStart;
        }
    }

    static class MyVoxelPrecalc
    {
        //  Don't use threads if no more than one core is available, instead use this one task. Or if you don't need to precalculate in parallel.
        static MyVoxelPrecalcTask m_singleCoreTask;

        //  Use threads if more than one core is available
        //  Good tutorials on thread synchronization events: 
        //      http://www.codeproject.com/KB/threads/AutoManualResetEvents.aspx
        //      http://www.albahari.com/threading/part2.aspx#_ProducerConsumerQWaitHandle
        //static Thread[] m_threads;
        //static AutoResetEvent[] m_workToDo;          //  Signals to worker threads that there is work to do
        //static AutoResetEvent[] m_workIsDone;        //  Signals to main thread that all tasks are calculated
        //public static object Locker;

        static Task[] m_tasks;
        static MyVoxelPrecalcWork[] m_precalWorks;

        //  This is needed in single-core mode and in multi-core too
        public static ConcurrentQueue<MyVoxelPrecalcTaskItem> Tasks;

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelPrecalc.LoadData");

            MyMwcLog.WriteLine("MyVoxelPrecalc.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            //  For calculating on main thread
            m_singleCoreTask = new MyVoxelPrecalcTask();

            Tasks = new ConcurrentQueue<MyVoxelPrecalcTaskItem>();
            if (MyMinerGame.NumberOfCores > 1)
            {
                m_tasks = new Task[MyMinerGame.NumberOfCores];
                m_precalWorks = new MyVoxelPrecalcWork[MyMinerGame.NumberOfCores];

                for (int i = 0; i < MyMinerGame.NumberOfCores; i++)
                    m_precalWorks[i] = new MyVoxelPrecalcWork();
            }
       
            /*
            //  For calculating in parallel threads
            if (MyMinerGame.NumberOfCores > MyConstants.ONE_CORE)
            {
                Locker = new object();
                m_threads = new Thread[MyMinerGame.NumberOfCores];
                m_workToDo = new AutoResetEvent[MyMinerGame.NumberOfCores];
                m_workIsDone = new AutoResetEvent[MyMinerGame.NumberOfCores];
                for (int i = 0; i < MyMinerGame.NumberOfCores; i++)
                {
                    //  Signal events for each thread
                    m_workToDo[i] = new AutoResetEvent(false);
                    m_workIsDone[i] = new AutoResetEvent(false);

                    //  Thread
                    MyVoxelPrecalcThread precalcThread = new MyVoxelPrecalcThread(m_workToDo[i], m_workIsDone[i]);
                    m_threads[i] = new Thread(new ThreadStart(precalcThread.Run));
                    m_threads[i].Name = "MyVoxelPrecalcTask " + i.ToString();
                    m_threads[i].IsBackground = true;
                    m_threads[i].Start();
                }
            }     */

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelPrecalc.LoadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {             /*
            //  If there are running threads, we need to abort them, otherwise application will still live
            if (MyMinerGame.NumberOfCores > MyConstants.ONE_CORE)
            {
                if (m_threads != null)
                {
                    for (int i = 0; i < m_threads.Length; i++)
                    {
                        if (m_threads[i] != null)
                        {
                            //  Abort won't stop the thread immediately (it just throws exception inside it), so we use Join to wait until that thread is really finished                
                            m_threads[i].Abort();
                            m_threads[i].Join();
                        }
                    }
                }
            }       */
            m_precalWorks = null;
            m_singleCoreTask = null;
        }

        public static void AddToQueue(
            MyLodTypeEnum type,
            MyVoxelMap voxelMap,
            MyVoxelCacheCellData cache,
            int voxelStartX, int voxelStartY, int voxelStartZ)
        {
            MyVoxelPrecalcTaskItem a = new MyVoxelPrecalcTaskItem();
            a.Type = type;
            a.VoxelMap = voxelMap;
            a.Cache = cache;
            a.VoxelStart = new MyMwcVector3Int(voxelStartX, voxelStartY, voxelStartZ);

            //  IMPORTANT: Don't need to lock Tasks, because at this point no other thread should access it.
            Tasks.Enqueue(a);
        }

        //  Precalculate voxel cell into cache (makes triangles and vertex buffer from voxels)
        //  Doesn't use threads, just main thread. Use when you don't want to precalculate many cells in parallel.
        public static void PrecalcImmediatelly(MyVoxelPrecalcTaskItem task)
        {
            m_singleCoreTask.Precalc(task);
        }

        //  Precalculate voxel cell into cache (makes triangles and vertex buffer from voxels)
        //  Uses threads (if more cores), calculates all cells in the queue.
        public static void PrecalcQueue()
        {
            //  Don't bother with this if queque isn't empty. 
            //  This is especially important in multi-core mode, because we don't want to uselessly signal worker threads!
            //  IMPORTANT: Don't need to lock Tasks, because at this point no other thread should access it.
            if (MyVoxelPrecalc.Tasks.Count <= 0) return;

            if (MyMinerGame.NumberOfCores == 1)
            {
                //  Precalculate all cells in the queue (do it in main thread)
                while (MyVoxelPrecalc.Tasks.Count > 0)
                {
                    MyVoxelPrecalcTaskItem newTask;
                    MyVoxelPrecalc.Tasks.TryDequeue(out newTask);
                    m_singleCoreTask.Precalc(newTask);
                }
            }
            else
            {
                for (int i = 0; i < MyMinerGame.NumberOfCores; i++)
                {
                    m_tasks[i] = Parallel.Start(m_precalWorks[i]);
                }

                for (int i = 0; i < MyMinerGame.NumberOfCores; i++)
                {
                    m_tasks[i].Wait();
                }
            }
        }
    }
}
