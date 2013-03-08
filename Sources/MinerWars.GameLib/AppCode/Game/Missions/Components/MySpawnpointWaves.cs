using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.Missions.Components
{
    class MySpawnpointWaves : MyMissionComponent
    {
        private uint m_detectorID;
        private MyEntityDetector m_detector;

        private List<MyEntity> m_spawnedBots;
        private List<uint[]> m_waves;

        private int m_currentWave;
        private int m_botsToNextWave;
        private int m_waveShipCount;

        public MySpawnpointWaves(uint detectorID, int botsToNextWave)
            : this(detectorID, botsToNextWave, new List<uint[]>())
        {
        }

        public MySpawnpointWaves(uint detectorID, int botsToNextWave, uint spawnpointID)
            : this(detectorID, botsToNextWave, new List<uint[]>() { new uint[] { spawnpointID } })
        {
        }

        public MySpawnpointWaves(uint detectorID, int botsToNextWave, List<uint[]> waves)
        {
            m_detectorID = detectorID;
            m_waves = waves;
            m_botsToNextWave = botsToNextWave;
        }

        public void AddWave(uint[] spawnpoints)
        {
            m_waves.Add(spawnpoints);
        }

        public override void Load(MyMissionBase sender)
        {
            base.Load(sender);

            m_detector = MyScriptWrapper.GetDetector(m_detectorID);
            if (m_detector != null)
            {
                m_detector.OnEntityEnter += OnDetector;
                m_detector.On();
            }

            m_currentWave = 0;
            m_spawnedBots = new List<MyEntity>();
        }

        public override void Unload(MyMissionBase sender)
        {
            base.Unload(sender);

            if (m_detector != null)
            {
                m_detector.OnEntityEnter -= OnDetector;
                m_detector.Off();
            }

            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
            MyScriptWrapper.EntityDeath -= EntityDeath;
        }

        void OnDetector(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity != null && entity == MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip)
            {
                MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
                MyScriptWrapper.EntityDeath += EntityDeath;

                SpawnNewWave(0);

                m_detector.OnEntityEnter -= OnDetector;
            }
        }

        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            if (m_currentWave >= m_waves.Count)
            {
                return;
            }

            foreach (var spawnpoints in m_waves)
            {
                foreach (var spawnPointId in m_waves[m_currentWave])
                {
                    if (MyScriptWrapper.GetEntityId(spawnpoint) == spawnPointId)
                    {
                        --m_waveShipCount;
                        m_spawnedBots.Add(bot);
                        return;
                    }
                }
            }
        }

        void EntityDeath(MyEntity who, MyEntity by)
        {
            if (m_spawnedBots.Remove(who) &&
                m_currentWave < m_waves.Count &&
                m_spawnedBots.Count <= m_botsToNextWave &&
                m_waveShipCount == 0)
            {
                ++m_currentWave;
                SpawnNewWave(m_currentWave);
            }
        }

        private void SpawnNewWave(int waveIndex)
        {
            if (m_currentWave < m_waves.Count)
            {
                m_waveShipCount = 0;
                foreach (var spawnpointId in m_waves[m_currentWave])
                {
                    MyScriptWrapper.ActivateSpawnPoint(spawnpointId);

                    MySpawnPoint spawnpoint = MyScriptWrapper.TryGetEntity(spawnpointId) as MySpawnPoint;
                    if (spawnpoint != null)
                    {
                        m_waveShipCount += spawnpoint.LeftToSpawn + spawnpoint.GetShipCount();
                    }
                }
            }
        }
    }
}
