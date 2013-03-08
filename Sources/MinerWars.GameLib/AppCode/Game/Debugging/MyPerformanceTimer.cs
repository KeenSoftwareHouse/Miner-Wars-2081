using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System.Diagnostics;

//  This static class is used for measuring time spent be some methods.
//
//  If you need to measure some new method, add new member of type MyStatsManagerCounter to class MyStatsManager, add loging into WriteToLog().
//  Result will be written into log at the application end.

//  IMPORTANT: Use this class only for profiling / debuging. Don't use it for real game code.

namespace MinerWars.AppCode.Game.Managers
{
    //  Set counting by Start. End by End. To get all time spent between each call of Start and End, call GetTimeSpent
    internal class MyPerformanceTimerObject
    {
        #region Members
        long m_startCount;
        double m_totalTimeSpent = 0; //total time spent in seconds
        double m_currentTimeSpent = 0; //time spent in last measurement
        double m_averageTimeSpent = 0; //average time spent in this block
        int m_averageCounter = 0;

        //  This variable is only for ensuring that we called End without calling Start before, or calling start twice without calling End
        bool m_isTimerRunning = false;   

        #endregion

        [Conditional("PROFILING")]
        public void Start()
        {
#if PROFILING
            //  You called Start twice, without calling End. Proper way is to call Start and than End.
            MyCommonDebugUtils.AssertDebug(m_isTimerRunning == false);
            
            m_isTimerRunning = true;
            m_startCount = QueryPerformanceCounter();
            m_averageCounter++;
#endif //PROFILING
        }

        [Conditional("PROFILING")]
        public void End()
        {
#if PROFILING
            //  You called End without first calling Start. Proper way is to call Start and than End.
            MyCommonDebugUtils.AssertDebug(m_isTimerRunning == true);
            m_isTimerRunning = false;

            long stopCount = QueryPerformanceCounter();

            long elapsedCount = stopCount - m_startCount;
            m_currentTimeSpent = (double)elapsedCount / QueryPerformanceFrequency();

            m_totalTimeSpent += m_currentTimeSpent;
            m_averageTimeSpent += m_currentTimeSpent;
#endif //PROFILING
        }

#if PROFILING
        //  Return total count if miliseconds spent between Start/End. This number contains sum of all calls, not only the last.
        public double GetTotalTimeSpent()
        {
            return m_totalTimeSpent;
        }

        //  Return total count if miliseconds spent between Start/End. This number contains sum of all calls, not only the last.
        public string GetTotalTimeSpentFormated()
        {
            return MyValueFormatter.GetFormatedDouble(GetTotalTimeSpent(), 10);
        }

        // returns time spent in this measurement session
        public double GetCurrentTimeSpent()
        {
            return m_currentTimeSpent;
        }

        public double GetAverageTimeSpent(bool reset)
        {
            double averageTime = m_averageTimeSpent / m_averageCounter;
            if (reset)
            {
                m_averageCounter = 0;
                m_averageTimeSpent = 0;
            }
            return averageTime;
        }

        /// <summary>
        /// Gets the current 'Ticks' on the performance counter
        /// </summary>
        /// <returns>Long indicating the number of ticks on the performance counter</returns>
        public static long QueryPerformanceCounter()
        {
            long perfcount;
            MyWindowsAPIWrapper.QueryPerformanceCounter(out perfcount);
            return perfcount;
        }

        /// <summary>
        /// Gets the number of performance counter ticks that occur every second
        /// </summary>
        /// <returns>The number of performance counter ticks that occur every second</returns>
        public static long QueryPerformanceFrequency()
        {
            long freq;
            MyWindowsAPIWrapper.QueryPerformanceFrequency(out freq);
            return freq;
        }

#endif //PROFILING

    }

    //  This is just holder for objects of type MyStatsManagerCounter.
    internal static class MyPerformanceTimer
    {
        public static MyPerformanceTimerObject GuiScreenGamePlay_RunBackgroundThread;
        public static MyPerformanceTimerObject GuiScreenGamePlay_LoadContent;
        public static MyPerformanceTimerObject GuiScreenGamePlay_LoadData;
        public static MyPerformanceTimerObject VoxelLoad;
        public static MyPerformanceTimerObject VoxelContentMerge;
        public static MyPerformanceTimerObject VoxelMaterialMerge;
        public static MyPerformanceTimerObject VoxelHandLoad;
        public static MyPerformanceTimerObject VoxelGpuBuffersBuild;
        public static MyPerformanceTimerObject CalcAverageDataCellMaterials;
        public static MyPerformanceTimerObject PrepareRenderCellCache;
        public static MyPerformanceTimerObject OctreeBuilding;

        static bool m_loaded = false;

        [Conditional("PROFILING")]
        public static void LoadData()
        {
            GuiScreenGamePlay_RunBackgroundThread = new MyPerformanceTimerObject();
            GuiScreenGamePlay_LoadContent = new MyPerformanceTimerObject();
            GuiScreenGamePlay_LoadData = new MyPerformanceTimerObject();
            VoxelLoad = new MyPerformanceTimerObject();
            VoxelContentMerge = new MyPerformanceTimerObject();
            VoxelMaterialMerge = new MyPerformanceTimerObject();
            VoxelHandLoad = new MyPerformanceTimerObject();
            VoxelGpuBuffersBuild = new MyPerformanceTimerObject();
            CalcAverageDataCellMaterials = new MyPerformanceTimerObject();
            PrepareRenderCellCache = new MyPerformanceTimerObject();
            OctreeBuilding = new MyPerformanceTimerObject();
            
            m_loaded = true;
        }

        public static void UnloadData()
        {
        }

        [Conditional("PROFILING")]
        public static void WriteToLog()
        {
#if PROFILING
            MyMwcLog.WriteLine("MyPerformanceTimer.WriteToLog - Start");
            MyMwcLog.IncreaseIndent();

            if (m_loaded == false)
            {
                MyMwcLog.WriteLine("MyPerformanceTimer wasn't loaded");
            }
            else
            {
                MyMwcLog.WriteLine("MyPerformanceTimer.GuiScreenGamePlay_RunBackgroundThread: " + MyPerformanceTimer.GuiScreenGamePlay_RunBackgroundThread.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.GuiScreenGamePlay_LoadContent: " + MyPerformanceTimer.GuiScreenGamePlay_LoadContent.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.GuiScreenGamePlay_LoadData: " + MyPerformanceTimer.GuiScreenGamePlay_LoadData.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.VoxelLoad: " + MyPerformanceTimer.VoxelLoad.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.VoxelContentMerge: " + MyPerformanceTimer.VoxelContentMerge.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.VoxelMaterialMerge: " + MyPerformanceTimer.VoxelMaterialMerge.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.VoxelHandLoad: " + MyPerformanceTimer.VoxelHandLoad.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.VoxelGpuBuffersBuild: " + MyPerformanceTimer.VoxelGpuBuffersBuild.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.CalcAverageDataCellMaterials: " + MyPerformanceTimer.CalcAverageDataCellMaterials.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.PrepareRenderCellCache: " + MyPerformanceTimer.PrepareRenderCellCache.GetTotalTimeSpentFormated());
                MyMwcLog.WriteLine("MyPerformanceTimer.OctreeBuilding: " + MyPerformanceTimer.OctreeBuilding.GetTotalTimeSpentFormated());
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyPerformanceTimer.WriteToLog - End");
#endif //PROFILING
        }
    }
}
