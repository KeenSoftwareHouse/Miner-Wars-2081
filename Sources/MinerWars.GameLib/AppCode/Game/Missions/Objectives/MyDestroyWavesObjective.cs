using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.Missions.Objectives
{
    class MyDestroyWavesObjective : MyObjective
    {
        private List<MyEntity> m_spawnedBots;
        private List<List<uint>> m_waves;
        private int m_currentWave;
        private int m_botsToNextWave;
        private int m_waveShipCount;
        private List<uint> m_spawnPointsIds;
  
        [Obsolete]
        public MyDestroyWavesObjective(StringBuilder Name, MyMissionID ID, StringBuilder Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, List<uint> spawnpoints = null, MyMissionLocation Location = null, Audio.Dialogues.MyDialogueEnum? successDialogId = null, Audio.Dialogues.MyDialogueEnum? startDialogId = null, int botsToNextWave = 3, float? radiusOverride = null)
            : base(Name, ID, Description, Icon, ParentMission, RequiredMissions, Location, null, successDialogId, startDialogId, null, radiusOverride)
        {
            m_waves = new List<List<uint>>();
            m_botsToNextWave = botsToNextWave;
            m_spawnPointsIds = spawnpoints;
        }


        public MyDestroyWavesObjective(MyTextsWrapperEnum Name, MyMissionID ID, MyTextsWrapperEnum Description, MyTexture2D Icon, MyMission ParentMission, MyMissionID[] RequiredMissions, List<uint> spawnpoints = null, MyMissionLocation Location = null, Audio.Dialogues.MyDialogueEnum? successDialogId = null, Audio.Dialogues.MyDialogueEnum? startDialogId = null, int botsToNextWave = 3,float? radiusOverride = null)
            : base(Name, ID, Description, Icon, ParentMission, RequiredMissions, Location, null, successDialogId, startDialogId, null, radiusOverride)
        {
            m_waves = new List<List<uint>>();
            m_botsToNextWave = botsToNextWave;
            m_spawnPointsIds = spawnpoints;
        }


        public void AddWave(List<uint> spawnpoints)
        {
            m_waves.Add(spawnpoints);
        }

        public override void Load()
        {
            base.Load();

            m_currentWave = 0;
            m_spawnedBots = new List<MyEntity>();

            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
            MyScriptWrapper.EntityDeath += EntityDeath;

            //m_currentWave = -1;
            if (m_spawnPointsIds!=null){
            foreach (var missionEntityID in m_spawnPointsIds)
            {
                var spawnpoint = MyScriptWrapper.GetEntity(missionEntityID) as MySpawnPoint;
                if (spawnpoint != null)
                {
                    var bots = spawnpoint.GetBots();
                    foreach (var bot in bots)
                    {
                        if (bot.Ship != null)
                        {
                            m_spawnedBots.Add(bot.Ship);
                        }
                    }

                }
            }
            }
            SpawnNewWave(0);

        }

        public override void Unload()
        {
            base.Unload();

            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
            MyScriptWrapper.EntityDeath -= EntityDeath;
        }

        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            if (m_currentWave >= m_waves.Count)
            {
                return;
            }

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

        public override void Update()
        {
            base.Update();


        }
        void EntityDeath(MyEntity who, MyEntity by)
        {
            if (m_spawnedBots.Remove(who) &&
                m_currentWave < m_waves.Count &&
                m_spawnedBots.Count <= m_botsToNextWave /*&&
                m_waveShipCount == 0*/)
            {
                ++m_currentWave;
                SpawnNewWave(m_currentWave);
            }
        }

        public override bool IsSuccess()
        {
            //return base.IsSuccess() || (m_waves.Count == m_currentWave && m_spawnedBots.Count == 0);

            if (Location!=null)
            {
                return base.IsSuccess() && (m_waves.Count == m_currentWave && m_spawnedBots.Count == 0);
            }
            return (m_waves.Count == m_currentWave && m_spawnedBots.Count == 0);

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
                        m_waveShipCount += spawnpoint.GetShipCount();
                    }
                }
            }
        }

        public override bool IsMissionEntity(MyEntity target)
        {
            if (base.IsMissionEntity(target)) return true;
            else if (m_spawnedBots != null)
                return m_spawnedBots.Contains(target);
            return false;
        }
    }
}
