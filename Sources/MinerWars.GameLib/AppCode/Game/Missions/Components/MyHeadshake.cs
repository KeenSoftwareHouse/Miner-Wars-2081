using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Missions.Components
{
    class MyHeadshake : MyMissionComponent
    {
        public static List<int> DefaultShaking = new List<int> { 0, 350, 550, 800, 1050, 1400, 1900 };

        public int? Time { get; set; }
        public float FirstShake { get; set; }
        public float NextShakesMin { get; set; }
        public float NextShakesMax { get; set; }
        public float Damping { get; set; }

        private int m_shakeIndex;
        
        private List<int> m_shakeWaves;


        public MyHeadshake(int? time, List<int> shakeWaves, float firstShake = 8, float nextShakesMin = 3, float nextShakesMax = 4, float damping = 0.07f)
        {
            Time = time;
            FirstShake = firstShake;
            NextShakesMin = nextShakesMin;
            NextShakesMax = nextShakesMax;
            Damping = damping;
            m_shakeWaves = shakeWaves;
        }

        public override void Update(MyMissionBase sender)
        {
            base.Update(sender);

            if (Time.HasValue)
            {
                if (m_shakeIndex < m_shakeWaves.Count &&
                    sender.MissionTimer.GetElapsedTime().TotalMilliseconds > Time.Value + m_shakeWaves[m_shakeIndex])
                {
                    if (m_shakeIndex == 0)
                    {
                        MyScriptWrapper.IncreaseHeadShake(FirstShake);
                    }
                    else
                    {
                        MyScriptWrapper.IncreaseHeadShake(MyMwcUtils.GetRandomFloat(NextShakesMin, NextShakesMax) * MathHelper.Max(0, 1.0f - Damping * m_shakeIndex));
                    }
                    ++m_shakeIndex;
                }
            }
        }
    }
}
