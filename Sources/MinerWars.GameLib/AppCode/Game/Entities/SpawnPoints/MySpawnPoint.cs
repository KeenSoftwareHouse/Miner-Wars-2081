#region Using

using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Missions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Utils;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI;
using KeenSoftwareHouse.Library.Memory;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Helpers;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Text;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Managers.Others;
using System.Diagnostics;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Physics.Collisions;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using MinerWars.AppCode.Game.Missions.Components;

#endregion

namespace MinerWars.AppCode.Game.Entities
{
    //templates 
    public class BotTemplate
    {
        public String m_name;
        public MyMwcObjectBuilder_SmallShip_Bot m_builder;
    }

    class MySpawnPoint : MyEntity
    {
        private const int MIN_TIME_BETWEEN_BOT_SPAWN = 50;
        private static int m_lastSpawnTime = 0;

        public event Action<MySpawnPoint> OnActivatedChanged;

        public class Bot
        {
            public MySmallShipBot Ship;
            public MyMwcObjectBuilder_SmallShip_Bot Builder;
            public int SpawnTime;
            public bool FirstSpawned;
            public bool DoSpawn;
        };

        public float FirstSpawnTimer { get; set; }  //in ms
        public float RespawnTimer { get; set; }     //in ms
        public bool SpawnInGroups { get; set; }
        public int MaxSpawnCount { get; set; }
        public int LeftToSpawn { get; set; }         // -1 means infinity
        public float BoundingSphereRadius { get; set; }   // world radius for spawing / we generate ships within this radius randomly
        public MyPatrolMode PatrolMode { get; set; }
        public bool FirstSpawnDone { get; private set; }
        //public bool IsDummy;

        List<BotTemplate> m_botTemplates;
        List<Bot> m_botShips; // templates or holders for ships we generate

        String m_wayPointPath = "";

        float m_followerChance = 0.7f;

        bool m_spawnActivated = true;

        int? m_spawnFailedTime;


        /// <summary>
        /// 0..1 chance to spawn follower -> 0 never 1 always
        /// </summary>
        public float FollowerChance
        {
            get
            {
                return m_followerChance;
            }
            set
            {
                m_followerChance = value;
            }
        }

        private MySmallShip m_leader;

        public MySmallShip Leader
        {
            get { return m_leader; }
            set
            {
                m_leader = value;
                foreach (var bot in m_botShips)
                {
                    if (bot.Ship != null)
                    {
                        bot.Ship.Follow(value);
                    }
                }
            }
        }

        private bool m_allKilledEventRaised;

        private const int MAX_SPAWN_ATTEMPTS = 5;// usualy
        private const int MAX_SPAWN_COUNT = 5;

        #region Limiters
        private List<MySpawnpointLimiter> m_limiters = new List<MySpawnpointLimiter>();
        
        public void AddLimiter(MySpawnpointLimiter limiter)
        {
            m_limiters.Add(limiter);
        }

        public void RemoveLimiter(MySpawnpointLimiter limiter)
        {
            m_limiters.Remove(limiter);
        }
        #endregion

        public MySpawnPoint()
            : base(true)
        {
            LeftToSpawn = -1;
            MaxSpawnCount = LeftToSpawn;
            m_botShips = new List<Bot>();
        }

        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_SpawnPoint objectBuilder, Matrix matrix)
        {
            Visible = MyGuiScreenGamePlay.Static == null || !MyGuiScreenGamePlay.Static.IsGameActive();

            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);
            base.Init(hudLabelTextSb, objectBuilder);
            m_botTemplates = new List<BotTemplate>();
            CastShadows = false;

            SpawnInGroups = objectBuilder.SpawnInGroups;
            LeftToSpawn = objectBuilder.SpawnCount;
            MaxSpawnCount = LeftToSpawn;
            FirstSpawnTimer = objectBuilder.FirstSpawnTimer;
            RespawnTimer = objectBuilder.RespawnTimer;
            BoundingSphereRadius = objectBuilder.BoundingRadius;
            PatrolMode = objectBuilder.PatrolMode;
            m_wayPointPath = objectBuilder.WayPointPath;
            m_allKilledEventRaised = false;

            foreach (MyMwcObjectBuilder_SmallShip_Bot shipBuilder in objectBuilder.ShipTemplates)
            {
                // Disable names on spawned bots (in this way, thay can be changed when faction changes)
                shipBuilder.DisplayName = null;

                Bot nb = new Bot();
                nb.Ship = null;
                nb.Builder = shipBuilder;

                nb.SpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                nb.FirstSpawned = false;
                m_botShips.Add(nb);
                BotTemplate bt = new BotTemplate();
                bt.m_builder = shipBuilder;
                bt.m_name = "";
                m_botTemplates.Add(bt);
            }

            this.LocalVolume = new BoundingSphere(Vector3.Zero, objectBuilder.BoundingRadius);

            if (Physics == null)
            {
                base.InitSpherePhysics(MyMaterialType.GLASS, WorldMatrix.Translation, BoundingSphereRadius, 1.0f, 1.0f, MyConstants.COLLISION_LAYER_UNCOLLIDABLE, RigidBodyFlag.RBF_RBO_STATIC);
            }

            VisibleInGame = false;
            Flags |= EntityFlags.EditableInEditor;

            Faction = objectBuilder.Faction;

            SetWorldMatrix(matrix);
            NeedsUpdate = true;

            if (objectBuilder.Activated)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }

            m_spawnFailedTime = null;
        }

        public List<BotTemplate> GetBotTemplates()
        {
            return m_botTemplates;
        }

        public List<BotTemplate> ApplyBotTemplates(List<BotTemplate> templates)
        {
            m_botTemplates.Clear();
            m_botTemplates.AddRange(templates);
            //here also delete / destroy all spawned ships 
            m_botShips.Clear();
            foreach (BotTemplate tmp in m_botTemplates)
            {
                Bot nb = new Bot();
                nb.Ship = null;
                nb.Builder = tmp.m_builder;
                nb.SpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                nb.FirstSpawned = false;
                m_botShips.Add(nb);
            }
            return m_botTemplates;
        }

        public void Reset()
        {
            // Remove also dead bots
            foreach (var bot in MyBotCoordinator.GetBots())
            {
                if (EntityId.HasValue && bot.OwnerId == EntityId.Value.NumericValue && bot.IsDead())
                {
                    bot.MarkForClose();
                }
            }

            foreach (var bot in m_botShips)
            {
                if (bot.Ship != null && !bot.Ship.Closed)
                {
                    bot.Ship.MarkForClose();
                }
                bot.Ship = null;
                bot.SpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                bot.FirstSpawned = false;
            }
            LeftToSpawn = MaxSpawnCount;

            m_allKilledEventRaised = false;
            FirstSpawnDone = false;
        }

        public void ResetBotsSpawnTime()
        {
            foreach (var bot in m_botShips)
            {
                bot.SpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }

        private MySmallShipBot CreateBotFromBuilder(MyMwcObjectBuilder_SmallShip_Bot bldr, Vector3 position)
        {
            //hax
            bldr.ArmorHealth = MyGameplayConstants.HEALTH_BASIC;
            bldr.ShipMaxHealth = MyGameplayConstants.HEALTH_BASIC;
            bldr.Fuel = float.MaxValue;
            bldr.Oxygen = float.MaxValue;
            bldr.OwnerId = this.EntityId.Value.NumericValue;
            if (bldr.ShipTemplateID != null) 
            {
                var shipTemplate = MySmallShipTemplates.GetTemplateForSpawn(bldr.ShipTemplateID.Value);
                Debug.Assert(shipTemplate != null, string.Format("ShipTemplate {0} was not found!", bldr.ShipTemplateID.Value));
                if (shipTemplate != null)
                {
                    shipTemplate.ApplyToSmallShipBuilder(bldr);
                }
            }
            bldr.Faction = this.Faction;

            MySmallShipBot bot = (MySmallShipBot)MyEntities.CreateFromObjectBuilder(null, bldr, Matrix.CreateWorld(position, Vector3.Backward, Vector3.Up));

            return bot;
        }

        private bool GetSafePositionForSpawn(MyMwcObjectBuilder_SmallShip_TypesEnum shipType, out Vector3? safePosition)
        {
            var shipRadius = MyModels.GetModelOnlyData(MyShipTypeConstants.GetShipTypeProperties(shipType).Visual.ModelLod0Enum).BoundingSphere.Radius;

            for (int c = MAX_SPAWN_ATTEMPTS; c-- != 0; )
            {
                Vector3 randomPointInSphere = MyMwcUtils.GetRandomVector3Normalized() * MyMwcUtils.GetRandomFloat(0, 1) * BoundingSphereRadius; // Random point in sphere
                Vector3 newTestPos = GetPosition() + randomPointInSphere;

                BoundingSphere bsphere = new BoundingSphere(newTestPos, shipRadius);
                MyEntity col = MyEntities.GetIntersectionWithSphere(ref bsphere);

                if (col == null)
                {
                    safePosition = newTestPos;
                    return true;
                }
            }

            safePosition = null;
            return false; 
        }

        private MySmallShipBot CreateShip(MyMwcObjectBuilder_SmallShip_Bot bldr, Vector3 position)
        {
            MySmallShipBot bot = CreateBotFromBuilder(bldr, position);
            MyEntities.Add(bot);

            SetBotPath(bot);

            return bot;
        }

        private void SetBotPath(MySmallShipBot bot)
        {
            bot.WaypointPath = MyWayPointGraph.GetPath(m_wayPointPath);
            bot.PatrolMode = PatrolMode;

            //check if any leader present
            Bot leader = m_botShips.Find(s => (s.Ship != null && s.Ship.Leader == null));
            if (leader != null)
            {
                bot.Follow(leader.Ship);
            }
            else if (bot.WaypointPath != null)
            {
                bot.Patrol();
            }

            if (Leader != null)
            {
                bot.Follow(Leader);
            }
        }


        public void LinkShip(MySmallShipBot bot)
        {
            SetBotPath(bot);

            foreach (var botShip in m_botShips)
            {
                if (botShip.Builder.ShipType == bot.ShipType)
                {
                    if (botShip.Ship == null)
                    {
                        botShip.Ship = bot;
                        break;
                    }
                }
            }
        }

        public void SpawnShip(int botsIdx, Vector3? position = null, uint? desiredBotId = null)
        {
            Bot bots = m_botShips[botsIdx];

            if (position == null)
            {
                if (m_spawnFailedTime.HasValue && MyMinerGame.TotalGamePlayTimeInMilliseconds - m_spawnFailedTime.Value < 1000)
                {
                    return;
                }
                else if (!GetSafePositionForSpawn(bots.Builder.ShipType, out position))
                {
                    // This should not happen
                    m_spawnFailedTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                   // System.Diagnostics.Debug.Assert(false, string.Format("Spawnpoint \"{0}\" failed on bot placement, try increase spawnpoint radius (atm. {1})!", EntityId.HasValue ? EntityId.Value.NumericValue.ToString() : "no EntityID", BoundingSphereRadius));
                    return;
                }
            }

            Debug.Assert(position != null);
            
            bots.Builder.EntityId = desiredBotId ?? MyEntityIdentifier.AllocateId(0).NumericValue;
            MySmallShipBot bot = CreateShip(bots.Builder, position.Value);
            bots.Builder.ClearEntityId();

            if (MyMultiplayerGameplay.IsRunning && !IsDummy)
            {
                MyMultiplayerGameplay.Static.SpawnBot(this, bot, botsIdx, position.Value);
            }

            bot.IsDummy = this.IsDummy;
            bot.InitTime = 3000;

            m_lastSpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            bots.DoSpawn = false;

            LeftToSpawn--;
            bots.Ship = bot; // Bot can be null (bot cant be placed, so won't be spawned)
            bots.FirstSpawned = true;

            MyScriptWrapper.SpawnpointBotSpawned(this, bot);

        }

        public override void UpdateBeforeSimulation()
        {
            if (MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive())
            {
                base.UpdateBeforeSimulation();
                return;
            }

            if (!IsDummy)
            {
                // normal spawn or if we can spawn whole group
                bool spawnAllowed = m_spawnActivated;

                // Spawn in groups
                if (SpawnInGroups && spawnAllowed)
                {
                    foreach (Bot bot in m_botShips)
                    {
                        if ((bot.Ship != null && !bot.DoSpawn) || MyMinerGame.TotalGamePlayTimeInMilliseconds - bot.SpawnTime <= GetRespawnTime(bot))
                        {
                            spawnAllowed = false;
                            break;
                        }
                    }

                    if (spawnAllowed)
                    {
                        foreach (Bot bots in m_botShips) bots.DoSpawn = true;
                    }
                    else
                    {
                        foreach (Bot bots in m_botShips)
                        {
                            if (bots.DoSpawn)
                            {
                                spawnAllowed = true;
                                break;
                            }
                        }
                    }
                }

                // Check if spawnpoint is visible
                if (MyFakes.ENABLE_VISIBLE_SPAWNPOINT_DEACTIVATION &&
                    spawnAllowed && LeftToSpawn != 0)
                {
                    MyLine line = new MyLine(MySession.PlayerShip.GetPosition(), GetPosition());
                    if (line.Length < 1000)
                    {
                        BoundingSphere boundingSphere = WorldVolume;
                        if (MyCamera.IsInFrustum(ref boundingSphere))
                        {
                            var intersection = MyEntities.GetAnyIntersectionWithLine(ref line, MySession.PlayerShip, null, true, true, false, true);

                            spawnAllowed = intersection.HasValue;
                        }
                    }
                }

                // Apply limiters
                foreach (var limiter in m_limiters)
                {
                    if (limiter.CurrentBotCount >= limiter.MaxBotCount)
                        spawnAllowed = false;
                }
            
                //Limit max count of spawned ships to 5
                for (int c = 0; c < Math.Min(m_botShips.Count, MAX_SPAWN_COUNT); c++)
                {
                    Bot bot = m_botShips[c];
                    // Only spawn when ships can be spawned
                    spawnAllowed &= LeftToSpawn != 0;

                    if (spawnAllowed)
                    {
                        //check if we can respawn ship
                        if (m_botShips[c].Ship == null &&
                            MyMinerGame.TotalGamePlayTimeInMilliseconds - bot.SpawnTime > GetRespawnTime(bot) &&
                            MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastSpawnTime > MIN_TIME_BETWEEN_BOT_SPAWN)
                        {
                            SpawnShip(c);

                            //Reset also when no bot was created, to avoid creation in each frame again and again..
                            if (m_botShips[c].Ship == null)
                            {
                                bot.SpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds; // reset timer
                            }
                        }
                    }
                }
            }

            for (int c = 0; c < m_botShips.Count; c++)
            {
                Bot bot = m_botShips[c];

                //reset spawning time
                if (bot.Ship != null)
                {
                    bot.SpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds; // reset timer
                }

                //dereference dead ship
                if (bot.Ship != null && bot.Ship.IsDead())
                {
                    bot.Ship = null;
                    bot.SpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                }
            }
            FirstSpawnDone = true;


            if (!m_allKilledEventRaised && LeftToSpawn == 0)
            {
                bool allKilled = true;
                foreach (var bot in m_botShips)
                {
                    if (bot.Ship != null && !bot.Ship.IsDead())
                    {
                        allKilled = false;
                    }
                }
                if (allKilled)
                {
                    MyScriptWrapper.OnSpawnpointBotsKilled(this);
                    m_allKilledEventRaised = true;
                }
            }

            base.UpdateBeforeSimulation();
        }


        public override bool Draw(MyRenderObject renderObject)
        {
            return base.Draw(renderObject);
        }

        public override bool DebugDraw()
        {
            if (MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive())
            {
                Matrix worldMatrix = WorldMatrix;
                Vector4 sphereColor = new Vector4(0, 0.6f, 0.75f, 0.4f);
                BoundingSphere localSphere = new BoundingSphere(WorldMatrix.Translation, BoundingSphereRadius);
                MySimpleObjectDraw.DrawTransparentSphere(ref worldMatrix, localSphere.Radius, ref sphereColor, true, 12);
            }
            return base.DebugDraw();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_SpawnPoint objectBuilder = (MyMwcObjectBuilder_SpawnPoint)base.GetObjectBuilderInternal(getExactCopy);

            objectBuilder.SpawnInGroups = SpawnInGroups;
            objectBuilder.SpawnCount = LeftToSpawn;
            objectBuilder.FirstSpawnTimer = FirstSpawnTimer;
            objectBuilder.RespawnTimer = RespawnTimer;
            objectBuilder.BoundingRadius = BoundingSphereRadius;
            objectBuilder.ShipTemplates = new List<MyMwcObjectBuilder_SmallShip_Bot>();
            objectBuilder.Faction = Faction;
            objectBuilder.WayPointPath = m_wayPointPath;
            objectBuilder.Activated = m_spawnActivated;
            objectBuilder.PatrolMode = PatrolMode;

            foreach (Bot bot in m_botShips)
            {
                if (bot.Builder.Inventory == null)
                    bot.Builder.Inventory = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), MyInventory.DEFAULT_MAX_ITEMS);
                objectBuilder.ShipTemplates.Add(bot.Builder);
            }

            return objectBuilder;
        }

        private float GetRespawnTime(Bot bot)
        {
            return bot.FirstSpawned ? RespawnTimer : FirstSpawnTimer;
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            BoundingSphere boundingSphere = new BoundingSphere(GetPosition(), BoundingSphereRadius);
            Ray ray = new Ray(line.From, line.Direction);

            float? result = ray.Intersects(boundingSphere);
            v = result.HasValue ? ray.Position + ray.Direction * result.Value : (Vector3?)null;

            return result.HasValue;
        }

        public void SetWayPointPath(String name)
        {
            m_wayPointPath = name;
        }

        public string GetWaypointPath()
        {
            return m_wayPointPath;
        }

        public void Activate()
        {
            bool changed = m_spawnActivated != true;
            m_spawnActivated = true;
            FirstSpawnDone = false;
            foreach (var botShip in m_botShips)
            {
                botShip.SpawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }

            if (changed && OnActivatedChanged != null)
                OnActivatedChanged(this);
        }

        public void Deactivate()
        {
            bool changed = m_spawnActivated != false;
            m_spawnActivated = false;

            if (changed && OnActivatedChanged != null)
                OnActivatedChanged(this);
        }

        public bool IsActive()
        {
            return m_spawnActivated;
        }

        public List<Bot> GetBots()
        {
            return m_botShips;
        }

        public MySmallShip GetLeader() 
        {            
            foreach (Bot bot in m_botShips) 
            {
                if (bot.Ship != null && !bot.Ship.IsDead() && bot.Ship.Leader == null) 
                {
                    return bot.Ship;
                }
            }
            return null;
        }

        public bool AllBotsKilled()
        {
            return m_allKilledEventRaised;
        }

        public override void PreloadForDraw()
        {
            base.PreloadForDraw();

            foreach (Bot bot in m_botShips)
            {
                MySmallShipBot ship = CreateBotFromBuilder(bot.Builder, Vector3.Zero);
                ship.PreloadForDraw();
                ship.Physics.Enabled = false;
                ship.MarkForClose();
            }
        }

        public int GetShipCount()
        {
         //   if (!firstSpawnDone)
           //     return m_botShips.Count;

            int count = 0;
            foreach (var botShip in m_botShips)
            {
                if (botShip.Ship != null && !botShip.Ship.IsDead())
                {
                    ++count;
                }
            }

            return count;
        }

        public override void Link()
        {
            base.Link();

            foreach (var nb in m_botShips)
            {
                nb.Builder.OwnerId = this.EntityId.Value.NumericValue;
            }
        }
    }

}