using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    class MyDangerZones
    {
        public const float DANGER_ZONE_SIZE = 200;
        public const float EXTRA_EXPLOSION_RADIUS = 300;
        
        public static MyDangerZones Instance = new MyDangerZones();

        private List<MyDangerZoneElement> results = new List<MyDangerZoneElement>();
        private List<MyLineSegmentOverlapResult<MyDangerZoneElement>> lineResults = new List<MyLineSegmentOverlapResult<MyDangerZoneElement>>();

        private static MyDynamicAABBTree m_dangerZoneStructure = new MyDynamicAABBTree(MyConstants.GAME_PRUNING_STRUCTURE_AABB_EXTENSION);
        private EventHandler OnPositionChangedHandler;

        public MyDangerZones()
        {
            OnPositionChangedHandler = new EventHandler(OnEntityPositionChanged);
        }

        public int Register(MySmallShipBot entity)
        {
            System.Diagnostics.Debug.Assert(entity == null || !entity.Closed);

            entity.OnPositionChanged += OnPositionChangedHandler;

            MyDangerZoneElement element = new MyDangerZoneElement(entity);

            BoundingBox bbox = new BoundingBox(entity.WorldAABB.Min - new Vector3(DANGER_ZONE_SIZE), entity.WorldAABB.Max + new Vector3(DANGER_ZONE_SIZE));

            int proxyId = m_dangerZoneStructure.AddProxy(ref bbox, element, 0);

            return proxyId;
        }

        public void Unregister(MySmallShipBot entity)
        {
            System.Diagnostics.Debug.Assert(entity == null || !entity.Closed);

            entity.OnPositionChanged -= OnPositionChangedHandler;
            m_dangerZoneStructure.RemoveProxy(entity.GetDangerZoneID());
        }

        void OnEntityPositionChanged(object sender, EventArgs e)
        {
            MySmallShipBot smallShip = sender as MySmallShipBot;
            if (smallShip != null)
	        {
                BoundingBox bbox = new BoundingBox(smallShip.WorldAABB.Min - new Vector3(DANGER_ZONE_SIZE), smallShip.WorldAABB.Max + new Vector3(DANGER_ZONE_SIZE));
                m_dangerZoneStructure.MoveProxy(smallShip.GetDangerZoneID(), ref bbox, Vector3.Zero);
	        }
        }

        public void Notify(MyLine line, MyEntity source)
        {
            System.Diagnostics.Debug.Assert(source == null || !source.Closed);

            lineResults.Clear();
            m_dangerZoneStructure.OverlapAllLineSegment<MyDangerZoneElement>(ref line, lineResults);

            foreach (var resultItem in lineResults)
            {
                if (source == null || (resultItem.Element.Bot != source && MyFactions.GetFactionsRelation(resultItem.Element.Bot, source) == MyFactionRelationEnum.Enemy))
                {
                    resultItem.Element.Bot.AddCuriousLocation(source != null ? source.GetPosition() : line.From, source);
                }
            }
        }

        public void Notify(BoundingBox bbox, MyEntity source)
        {
            System.Diagnostics.Debug.Assert(source == null || !source.Closed);

            results.Clear();
            m_dangerZoneStructure.OverlapAllBoundingBox<MyDangerZoneElement>(ref bbox, results);

            foreach (var dangerZone in results)
            {
                if (source == null || (dangerZone.Bot != source && MyFactions.GetFactionsRelation(dangerZone.Bot, source) == MyFactionRelationEnum.Enemy))
                {
                    dangerZone.Bot.AddCuriousLocation(source != null ? source.GetPosition() : bbox.GetCenter(), source);
                }
            }
        }

        public void NotifyExplosion(Vector3 position, float radius, MyEntity source)
        {
            System.Diagnostics.Debug.Assert(source == null || !source.Closed);

            BoundingBox bbox = new BoundingBox(position - new Vector3(radius + EXTRA_EXPLOSION_RADIUS), position + new Vector3(radius + EXTRA_EXPLOSION_RADIUS));

            results.Clear();
            m_dangerZoneStructure.OverlapAllBoundingBox<MyDangerZoneElement>(ref bbox, results);

            foreach (var dangerZone in results)
            {
                if (source == null || (dangerZone.Bot != source && MyFactions.GetFactionsRelation(dangerZone.Bot, source) == MyFactionRelationEnum.Enemy))
                {
                    dangerZone.Bot.AddCuriousLocation(position, null);
                }
            }
        }

        public void NotifyArea(Vector3 position, float area, MyEntity source)
        {
            System.Diagnostics.Debug.Assert(source == null || !source.Closed);

            BoundingBox bbox = new BoundingBox(position - new Vector3(area), position + new Vector3(area));

            results.Clear();
            m_dangerZoneStructure.OverlapAllBoundingBox<MyDangerZoneElement>(ref bbox, results);

            foreach (var dangerZone in results)
            {
                if (source == null || (dangerZone.Bot != source && MyFactions.GetFactionsRelation(dangerZone.Bot, source) == MyFactionRelationEnum.Enemy))
                {
                    dangerZone.Bot.AddCuriousLocation(position, null);
                }
            }
        }

        public void Aggro(BoundingBox bbox, MySmallShip source)
        {
            System.Diagnostics.Debug.Assert(source == null || !source.Closed);

            results.Clear();
            m_dangerZoneStructure.OverlapAllBoundingBox<MyDangerZoneElement>(ref bbox, results);

            foreach (var dangerZone in results)
            {
                dangerZone.Bot.Attack(source);
            }
        }

        public void UnloadData()
        {
            results.Clear();
            lineResults.Clear();
            m_dangerZoneStructure.Clear();
        }
    }

    class MyDangerZoneElement : MyElement
    {
        public MySmallShipBot Bot { get; set; }

        public MyDangerZoneElement(MySmallShipBot bot)
        {
            Bot = bot;
        }
    }

}
