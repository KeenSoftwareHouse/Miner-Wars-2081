using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using ParallelTasks;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Entities.Ships.AI.Jobs
{
    class MyTestPositionJob : IWork
    {
        MySmallShipBot m_bot;
        Vector3 m_position;
        Vector3 m_targetPosition;
        Vector3 m_up;

        Vector3[] m_points;

        public Vector3? Result;

        public MyTestPositionJob()
        {
            m_points = new Vector3[4];
        }

        public void Start(MySmallShipBot bot, Vector3 position, Vector3 targetPosition)
        {
            m_bot = bot;
            m_position = position;
            m_targetPosition = targetPosition;
            m_up = bot.WorldMatrix.Up;

            float delta = 1.0f;
            m_points[0] = new Vector3(m_bot.LocalAABB.Max.X * delta, m_bot.LocalAABB.Max.Y * delta, m_bot.LocalAABB.Max.Z);
            m_points[1] = new Vector3(m_bot.LocalAABB.Min.X * delta, m_bot.LocalAABB.Max.Y * delta, m_bot.LocalAABB.Max.Z);
            m_points[2] = new Vector3(m_bot.LocalAABB.Max.X * delta, m_bot.LocalAABB.Min.Y, m_bot.LocalAABB.Max.Z);
            m_points[3] = new Vector3(m_bot.LocalAABB.Min.X * delta, m_bot.LocalAABB.Min.Y, m_bot.LocalAABB.Max.Z);

            Result = null;
            m_bot.OnClose += m_bot_OnClose;
        }

        void m_bot_OnClose(MyEntity obj)
        {
            m_bot = null;
        }

        public void DoWork()
        {
            try
            {
                MyEntities.EntityCloseLock.AcquireShared();

                if (m_bot == null)
                {
                    return;
                }

                BoundingSphere boundingSphere = new BoundingSphere(m_position, m_bot.WorldVolume.Radius * 2.0f);
                if (MyEntities.GetIntersectionWithSphere(ref boundingSphere) != null)
                {
                    return;
                }

                Matrix transform = Matrix.CreateWorld(m_position, MyMwcUtils.Normalize(m_targetPosition - m_position), m_up);
                float distanceToRoutePoint = Vector3.Dot(m_targetPosition - m_position, transform.Forward);

                for (int i = 0; i < m_points.Length; i++)
                {
                    Vector3 transformedPoint = Vector3.Transform(m_points[i], transform);
                    MyLine line = new MyLine(transformedPoint, transformedPoint + transform.Forward * distanceToRoutePoint, true);

                    var result = MyEntities.GetIntersectionWithLine(ref line, m_bot, null, true);
                    if (result.HasValue)
                    {
                        // Collision detected
                        return;
                    }
                }

                Result = m_position;
            }
            finally
            {
                if (m_bot != null)
                {
                    m_bot.OnClose -= m_bot_OnClose;
                }
                MyEntities.EntityCloseLock.ReleaseShared();
            }
        }

        public WorkOptions Options
        {
            get { return new WorkOptions() { MaximumThreads = 1 }; }
        }
    }
}
