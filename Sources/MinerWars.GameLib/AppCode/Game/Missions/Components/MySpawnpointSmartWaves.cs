#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Sessions;

#endregion

namespace MinerWars.AppCode.Game.Missions.Components
{
    class MySpawnpointSmartWaves : MyMissionComponent
    {
        #region Members

        private uint[] m_spawnPointIDs;
        private uint[] m_excludedSpawnPointIDs;
        private List<MySpawnPoint> m_spawnPoints = new List<MySpawnPoint>();
        private int m_maxBotsCount = 0;
        private int m_currentBotsCount = 0;

        private List<MyEntity> m_spawnedBots = new List<MyEntity>();
        SortedDictionary<float, MySpawnPoint> m_sortedSpawnPoints = new SortedDictionary<float, MySpawnPoint>();

        bool m_spawnInGroups;
        int m_spawnInterval;

        #endregion

        #region Constructor

        public MySpawnpointSmartWaves(uint[] spawnPointIDs, uint[] excludedSpawnPointIDs, int maxBotsCount)
        {
            m_spawnPointIDs = spawnPointIDs;
            m_excludedSpawnPointIDs = excludedSpawnPointIDs ?? new uint[0];
            m_maxBotsCount = maxBotsCount;
        }

        #endregion

        #region Overrides

        public override void Load(MyMissionBase sender)
        {
            base.Load(sender);
            m_spawnPoints.Clear();

            if (m_spawnPointIDs == null)
            {   //Add all spawnpoints in mission
                List<uint> list = new List<uint>();
                foreach (MyEntity entity in MyEntities.GetEntities())
                {
                    MySpawnPoint spawnPoint = entity as MySpawnPoint;
                    if (spawnPoint != null && MyFactions.GetFactionsRelation(MySession.PlayerShip.Faction, spawnPoint.Faction) == MyFactionRelationEnum.Enemy && !m_excludedSpawnPointIDs.Contains(spawnPoint.EntityId.Value.NumericValue))
                    {
                        list.Add(spawnPoint.EntityId.Value.NumericValue);
                        spawnPoint.OnActivatedChanged += new Action<MySpawnPoint>(spawnPoint_OnActivatedChanged);
                    }
                }
                m_spawnPointIDs = list.ToArray();
            }

            foreach (uint spawnPointID in m_spawnPointIDs)
            {
                m_spawnPoints.Add(MyEntities.GetEntityById(new MyEntityIdentifier(spawnPointID)) as MySpawnPoint);
            }

            MyScriptWrapper.OnSpawnpointBotSpawned += OnSpawnpointBotSpawned;
            
            m_currentBotsCount = 0;

            foreach (MySpawnPoint spawnPoint in m_spawnPoints)
            {
                spawnPoint.LeftToSpawn = 0;
                spawnPoint.FirstSpawnTimer = 0;
                spawnPoint.RespawnTimer = 0;

                m_currentBotsCount += spawnPoint.GetShipCount();
                spawnPoint.Deactivate();
            }

            UpdateCurrentBotCount();
        }

        void spawnPoint_OnActivatedChanged(MySpawnPoint obj)
        {
            UpdateCurrentBotCount();
        }

        public override void Unload(MyMissionBase sender)
        {
            base.Unload(sender);

            m_spawnPoints.Clear();
            m_sortedSpawnPoints.Clear();

            MyScriptWrapper.OnSpawnpointBotSpawned -= OnSpawnpointBotSpawned;
        }


        void OnSpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            if (m_spawnPoints.Contains(spawnpoint))
            {
                m_currentBotsCount++;

                bot.OnClose += new Action<MyEntity>(bot_OnClose);

                MySmallShipBot botShip = bot as MySmallShipBot;
                botShip.SleepDistance = 10000;
                botShip.Attack(ChooseClosestPlayer(bot));
            }
        }

        MyEntity ChooseClosestPlayer(MyEntity bot)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                var closestEntity = MySession.PlayerShip;
                var distSquared = Vector3.DistanceSquared(bot.GetPosition(), MySession.PlayerShip.GetPosition());

                foreach (var player in MyMultiplayerGameplay.Peers.Players)
                {
                    if (player.Ship != null && !player.Ship.IsDead() && !player.Ship.IsPilotDead())
                    {
                        var newDistSquared = Vector3.DistanceSquared(player.Ship.GetPosition(), closestEntity.GetPosition());
                        if (newDistSquared < distSquared)
                        {
                            distSquared = newDistSquared;
                            closestEntity = player.Ship;
                        }
                    }
                }

                return closestEntity;
            }
            else
            {
                return MySession.PlayerShip;
            }
        }

        void bot_OnClose(MyEntity obj)
        {
            m_currentBotsCount--;

            UpdateCurrentBotCount();
        }

        void UpdateCurrentBotCount()
        {
            bool canSpawn = SpawnInGroups ? m_currentBotsCount == 0 : m_currentBotsCount < MaxBotsCount;

            if (canSpawn)
            {
                //Get closest spawnpoint not closer than 700m
                float minAllowedDistance = 700; //m

                m_sortedSpawnPoints.Clear();

                foreach (MySpawnPoint spawnPoint in m_spawnPoints)
                {
                    //if (!spawnPoint.IsActive())
                      //  continue;

                    float distance = Vector3.Distance(spawnPoint.GetPosition(), MySession.PlayerShip.GetPosition());
                    m_sortedSpawnPoints.Add(distance, spawnPoint);
                }

                int needSpawn = m_maxBotsCount - m_currentBotsCount;
                while (needSpawn > 0 && m_sortedSpawnPoints.Count > 0)
                {
                    KeyValuePair<float, MySpawnPoint>? idealSpawnPoint = null;

                    foreach (var pair in m_sortedSpawnPoints)
                    {
                        if (pair.Key >= minAllowedDistance)
                        {
                            idealSpawnPoint = pair;
                            break;
                        }
                    }

                    if (idealSpawnPoint == null)
                    {   //Pick farthest spawn point
                        foreach (var pair in m_sortedSpawnPoints)
                        {
                            idealSpawnPoint = pair;
                        }
                    }

                    if (idealSpawnPoint != null)
                    {
                        int spawnBotsCount = Math.Min(needSpawn, idealSpawnPoint.Value.Value.GetBots().Count - idealSpawnPoint.Value.Value.GetShipCount());
                        idealSpawnPoint.Value.Value.LeftToSpawn = spawnBotsCount;
                        idealSpawnPoint.Value.Value.SpawnInGroups = false;
                        idealSpawnPoint.Value.Value.Activate();

                        idealSpawnPoint.Value.Value.FirstSpawnTimer = SpawnIntervalInMS;
                        idealSpawnPoint.Value.Value.RespawnTimer = SpawnIntervalInMS;

                        needSpawn -= spawnBotsCount;
                        m_sortedSpawnPoints.Remove(idealSpawnPoint.Value.Key);
                    }
                }
            }
        }

        public override void Update(MyMissionBase sender)
        {
            base.Update(sender);
        }

        public override void Success(MyMissionBase sender)
        {
            base.Success(sender);
        }

        #endregion

        #region Properties

        public bool SpawnInGroups
        {
            get { return m_spawnInGroups; }
            set
            {
                m_spawnInGroups = value;
                UpdateCurrentBotCount();
            }
        }

        public int MaxBotsCount
        {
            get { return m_maxBotsCount; }
            set
            {
                m_maxBotsCount = value;
                UpdateCurrentBotCount();
            }
        }

        public int SpawnIntervalInMS
        {
            get { return m_spawnInterval; }
            set
            {
                m_spawnInterval = value;
                UpdateCurrentBotCount();
            }
        }

#endregion
    }
}
