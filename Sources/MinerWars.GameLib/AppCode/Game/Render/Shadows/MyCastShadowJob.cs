#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath.Graphics;
using System;
using ParallelTasks;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Utils;

#endregion

namespace MinerWars.AppCode.Game.Render
{
    /// <summary>
    /// Sensor element used for sensors
    /// </summary>
    class MyCastShadowJob : IWork
    {
        MyEntity m_entity;

        public bool VisibleFromSun;

        public MyCastShadowJob(MyEntity entity)
        {
            m_entity = entity;
            VisibleFromSun = false;
            m_entity.OnClose += m_entity_OnMarkForClose;
        }

        void m_entity_OnMarkForClose(MyEntity obj)
        {
            m_entity = null;
        }

        public void DoWork()
        {
            try
            {
                MyEntities.EntityCloseLock.AcquireShared();

                if (m_entity == null)
                    return;

                //if (m_entity.EntityId.HasValue && m_entity.EntityId.Value.NumericValue == 119150)
               // {
              //  }

              //  if (m_entity == MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip)
              //  {
              //  }

                Vector3 directionToSunNormalized = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();

                VisibleFromSun = false;

                MyLine line2 = new MyLine(m_entity.WorldAABB.GetCenter(), m_entity.WorldAABB.GetCenter() + directionToSunNormalized * MyShadowRenderer.SHADOW_MAX_OFFSET * 0.5f, true);
                var result2 = MyEntities.GetIntersectionWithLine(ref line2, m_entity, null, true, true, true);
                VisibleFromSun |= IsVisibleFromSun(result2);

                if (m_entity.RenderObjects != null && m_entity.RenderObjects[0].FastCastShadowResolve)
                    return;

                Vector3[] corners = new Vector3[8];
                m_entity.LocalAABB.GetCorners(corners);
                for (int i = 0; i < 8; i++)
                {
                    corners[i] = Vector3.Transform(corners[i], m_entity.WorldMatrix);
                }

                for (int i = 0; i < 8; i++)
                {
                    MyLine line = new MyLine(corners[i], corners[i] + directionToSunNormalized * MyShadowRenderer.SHADOW_MAX_OFFSET * 0.5f, true);
                    var result = MyEntities.GetIntersectionWithLine(ref line, m_entity, null, true, true, true);

                    VisibleFromSun |= IsVisibleFromSun(result);

                    if (VisibleFromSun)
                        break;
                }
            }
            finally
            {
                if (m_entity != null)
                {
                    m_entity.OnClose -= m_entity_OnMarkForClose;
                }

                MyEntities.EntityCloseLock.ReleaseShared();
            }
        }

        private bool IsVisibleFromSun(MyIntersectionResultLineTriangleEx? result)
        {
            if (!MyRender.EnableAsteroidShadows)
            {
                if (result.HasValue && (result.Value.Entity is MyStaticAsteroid) && (result.Value.Entity as MyStaticAsteroid).IsGenerated)
                    return true;
                else
                    return !result.HasValue;
            }
            else
                return !result.HasValue;
        }

        public WorkOptions Options
        {
            get { return new WorkOptions() { MaximumThreads = 1 }; }
        }
    }
}
