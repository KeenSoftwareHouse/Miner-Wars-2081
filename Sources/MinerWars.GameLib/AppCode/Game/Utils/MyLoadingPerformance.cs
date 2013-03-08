using System;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Utils
{
    class MyLoadingPerformance
    {
        #region Singleton

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static MyLoadingPerformance Instance
        {
            get { return m_instance ?? (m_instance = new MyLoadingPerformance()); }
        }

        static MyLoadingPerformance m_instance;

        #endregion

        public string LoadingName { get; set; }

        private int m_voxelHandCount;
        private Dictionary<uint, Tuple<int, string>> m_voxelCounts = new Dictionary<uint, Tuple<int, string>>(); 
        private TimeSpan m_loadingTime;

        public bool IsTiming { get; private set; }

        Stopwatch m_stopwatch;

        private void Reset()
        {
            m_voxelHandCount = 0;
            LoadingName = null;
            m_loadingTime = TimeSpan.Zero;
            m_voxelCounts.Clear();
        }

        public void StartTiming()
        {
            if (IsTiming)
            {
                return;
            }

            Reset();

            IsTiming = true;
            m_stopwatch = Stopwatch.StartNew();
        }

        public void AddVoxelHandCount(int count,uint entityID, string name)
        {
            if (IsTiming)
            {
                m_voxelHandCount += count;
                if(!m_voxelCounts.ContainsKey(entityID)) m_voxelCounts.Add(entityID,new Tuple<int, string>(count,name));
            }
        }

        public void FinishTiming()
        {
            m_stopwatch.Stop();
            IsTiming = false;
            m_loadingTime = m_stopwatch.Elapsed;

            var myMwcStartSessionRequestTypeEnum = MyGuiScreenGamePlay.Static.GetSessionType();
            if (myMwcStartSessionRequestTypeEnum != null &&  myMwcStartSessionRequestTypeEnum.Value == MyMwcStartSessionRequestTypeEnum.NEW_STORY)
            {
               Debug.Assert(m_voxelHandCount<=200 ,"You have " + m_voxelHandCount + " voxel hands in sector, please export all of them. See the Log for more info. Section:LOADING REPORT FOR. ");
            }
            WriteToLog();
        }

        public void WriteToLog()
        {
            MyMwcLog.WriteLine("LOADING REPORT FOR: " + LoadingName);
            MyMwcLog.IncreaseIndent();
            {
                MyMwcLog.WriteLine("Loading time: " + m_loadingTime);
                MyMwcLog.WriteLine("Number of voxel hands total: " + m_voxelHandCount);
                MyMwcLog.IncreaseIndent();
                {
                    foreach (var voxelCount in m_voxelCounts)
                    {
                        if (voxelCount.Value.Item1 > 0) MyMwcLog.WriteLine("Asteroid: " + voxelCount.Key + " voxel hands: " + voxelCount.Value.Item1+ ". Voxel File: "+voxelCount.Value.Item2);
                    }
                }
                MyMwcLog.DecreaseIndent();
            }
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("END OF LOADING REPORT");
        }
    }
}