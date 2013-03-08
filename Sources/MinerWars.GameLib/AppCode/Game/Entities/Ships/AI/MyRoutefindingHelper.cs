using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
using MinerWarsMath;
using System.Collections.Concurrent;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Parallelization.Threading;
using ParallelTasks;

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    internal delegate void RouteFoundHandler(Vector3? routePosition, object userData);

    static class MyRoutefindingHelper
    {
        static BackgroundTaskWork m_routefindingHelper = new BackgroundTaskWork();

        class BackgroundTaskWork : IWork
        {
            public void DoWork()
            {
                MyRoutefindingHelper.BackgroundTask();
            }

            public WorkOptions Options
            {
                get { return Parallel.DefaultOptions; }
            }
        }


        public static void FindRouteInBackground(MySmallShipBot bot, MyPositionMemory route, RouteFoundHandler routeFoundHandler, object userData)
        {
            m_queue.Enqueue(new RouteToBeFound(bot, route, routeFoundHandler, userData));
        }

        private const int MAX_ROUTES_UPDATED_PER_ADVANCE = 8;
        private const int MAX_POINTS_TESTED_PER_ROUTE_UPDATE = 10;

        static Task m_advanceRouteTask;

        public static void AdvanceRoutefinding()
        {
            if (m_queue.Count > 0)
                //   m_event.Set();
                m_advanceRouteTask = Parallel.Start(m_routefindingHelper);
        }

        #region Implementation

        static MyRoutefindingHelper()
        {
            m_queue = new ConcurrentQueue<RouteToBeFound>();
            //m_event = new AutoResetEvent(false);
           // Task.Factory.StartNew(BackgroundTask, TaskCreationOptions.PreferFairness);
        }

        private struct RouteToBeFound
        {
            public MySmallShipBot Bot;
            void OnCloseBot(MyEntity obj) { ClearHandlers(); Bot = null; }
            Vector3 Up;
            Vector3 Position;
            Vector3[] Points;
            MyPositionMemory PositionMemory;
            int PositionMemoryIndex;

            public RouteFoundHandler RouteFoundHandler;
            public object UserData;

            public void ClearHandlers() 
            {
                if (Bot != null)
                {
                    Bot.OnClose -= OnCloseBot;
                }
            }

            public RouteToBeFound(MySmallShipBot bot, MyPositionMemory positionMemory, RouteFoundHandler routeFoundHandler, object userData)
            {
                PositionMemory = positionMemory;
                PositionMemoryIndex = PositionMemory.GetCount() - 1;

                Bot = bot;
                Up = bot.WorldMatrix.Up;
                Position = bot.WorldMatrix.Translation;

                Points = new Vector3[4];
                float delta = 1.0f;
                float scale = 1.0f / MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE;
                Points[0] = new Vector3(Bot.LocalAABB.Max.X * delta * scale, Bot.LocalAABB.Max.Y * delta * scale, Bot.LocalAABB.Max.Z * scale);
                Points[1] = new Vector3(Bot.LocalAABB.Min.X * delta * scale, Bot.LocalAABB.Max.Y * delta * scale, Bot.LocalAABB.Max.Z * scale);
                Points[2] = new Vector3(Bot.LocalAABB.Max.X * delta * scale, Bot.LocalAABB.Min.Y * scale, Bot.LocalAABB.Max.Z * scale);
                Points[3] = new Vector3(Bot.LocalAABB.Min.X * delta * scale, Bot.LocalAABB.Min.Y * scale, Bot.LocalAABB.Max.Z * scale);

                RouteFoundHandler = routeFoundHandler;
                UserData = userData;

                Bot.OnClose += OnCloseBot;
            }

            /// <summary>
            /// Advance routefinding.
            /// </summary>
            /// <returns>True when routefinding is finished.</returns>
            public bool AdvanceRoutefinding(out Vector3? result)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AdvanceRoutefinding");

                using (MyEntities.EntityCloseLock.AcquireSharedUsing())
                {
                    if (Bot == null)
                    {
                        result = null;
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        return true;
                    }  // bot was closed, no need to continue

                    using (PositionMemory.RouteMemoryLock.AcquireSharedUsing())
                    {
                        for (int pointsTested = 0; pointsTested < MAX_POINTS_TESTED_PER_ROUTE_UPDATE; PositionMemoryIndex--, pointsTested++)
                        {
                            if (PositionMemoryIndex == -1)
                            {
                                result = null;
                                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                                return true;
                            }  // all points done, no route found

                            Vector3 routePosition = PositionMemory.GetItem(PositionMemoryIndex);
                            Matrix transform = Matrix.CreateWorld(Position, MyMwcUtils.Normalize(routePosition - Position), Up);
                            float distanceToRoutePoint = Vector3.Dot(routePosition - Position, transform.Forward);

                            if ((routePosition - Position).LengthSquared() < 5 * 5) continue;  // too close for comfort, try next point

                            bool collisionFound = false;
                            for (int i = 0; i < Points.Length; i++)
                            {
                                Vector3 transformedPoint = Vector3.Transform(Points[i], transform);
                                MyLine line = new MyLine(transformedPoint, transformedPoint + transform.Forward * distanceToRoutePoint, true);
                                var intersectionResult = MyEntities.GetAnyIntersectionWithLine(ref line, Bot, null, true, false, true, true);
                                if (intersectionResult.HasValue)
                                {
                                    collisionFound = true;  // collision: try next point
                                    break;
                                }
                            }
                            if (!collisionFound)
                            {
                                result = routePosition;
                                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                                return true;
                            }
                        }
                    }
                }

                result = null;

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                return false;
            }
        }

        private static readonly ConcurrentQueue<RouteToBeFound> m_queue;
        //private static readonly AutoResetEvent m_event;
        private static void BackgroundTask()
        {
            RouteToBeFound next;
            int routesUpdated = 0;
            while (++routesUpdated <= MAX_ROUTES_UPDATED_PER_ADVANCE && m_queue.TryDequeue(out next))
            {
                Vector3? result;
                bool complete = next.AdvanceRoutefinding(out result);
                if (complete)
                {
                    if (next.Bot != null)  // bot closed - routefinding canceled
                        next.RouteFoundHandler(result, next.UserData);  // finished, notify the callback
                    next.ClearHandlers();
                }
                else
                {
                    m_queue.Enqueue(next);  // not finished yet, insert it back to the queue
                }
            }
        }

        #endregion
    }
}
