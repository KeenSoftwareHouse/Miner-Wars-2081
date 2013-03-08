#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.Utils;
using System;
using MinerWars.AppCode.App;

#endregion

namespace MinerWars.AppCode.Game.Render
{
    public enum MyOcclusionQueryID
    {
        MAIN_RENDER = 0,
        CASCADE_1 = 1,
        CASCADE_2 = 2,
        CASCADE_3 = 3,
        CASCADE_4 = 4,
    }

    public enum MyOcclusionQueryRenderType
    {
        HWDepth,
        CustomDepth
    }

    internal class MyOcclusionQueryIssue
    {
        public MyOcclusionQueryIssue(MyCullableRenderObject cullObject)
        {
            CullObject = cullObject;
        }

        public MyCullableRenderObject CullObject { get; private set; }
        public MyOcclusionQuery OcclusionQuery { get; set; }
        public bool OcclusionQueryVisible { get; set; }
        public bool OcclusionQueryIssued { get; set; }
        public MyOcclusionQueryRenderType RenderType { get; set; }

    }

    /// <summary>
    /// Sensor element used for sensors
    /// </summary>
    internal class MyCullableRenderObject : MyRenderObject
    {
        MyOcclusionQueryIssue[] m_queries = new MyOcclusionQueryIssue[Enum.GetValues(typeof(MyOcclusionQueryID)).Length];
        
        public int EntitiesContained { get; set; }

        public MyDynamicAABBTree CulledObjects { get; private set; }

        public MyCullableRenderObject(MyEntity entity) : base (entity, null)
        {
            CulledObjects = new MyDynamicAABBTree(MyRender.PrunningExtension);
            EntitiesContained = 0;

            for (int i = 0; i < Enum.GetValues(typeof(MyOcclusionQueryID)).Length; i++)
            {
                m_queries[i] = new MyOcclusionQueryIssue(this);
                m_queries[i].RenderType = MyOcclusionQueryRenderType.HWDepth;
            }

            m_queries[(int)MyOcclusionQueryID.MAIN_RENDER].RenderType = MyOcclusionQueryRenderType.CustomDepth;
        }

        public MyCullableRenderObject(BoundingBox aabb)
            : this(null)
        {
            m_AABB = aabb;
        }

        public override void UpdateAABB()
        {
            if (Entity != null)
            {
                base.UpdateAABB();
            }
        }

        public MyOcclusionQueryIssue GetQuery(MyOcclusionQueryID id)
        {
            return m_queries[(int)id];
        }

        public void InitQueries()
        {
            for (int i = 0; i < Enum.GetValues(typeof(MyOcclusionQueryID)).Length; i++)
            {
                MyOcclusionQuery occlusionQuery = MyOcclusionQueries.Get();

                m_queries[i].OcclusionQueryIssued = false;
                m_queries[i].OcclusionQueryVisible = true;

                m_queries[i].OcclusionQuery = occlusionQuery;
            }
        }

        public void DestroyQueries()
        {
            for (int i = 0; i < Enum.GetValues(typeof(MyOcclusionQueryID)).Length; i++)
            {
                m_queries[i].OcclusionQueryIssued = false;
                if (m_queries[i].OcclusionQuery != null)
                {
                    MyOcclusionQueries.Return(m_queries[i].OcclusionQuery);
                    m_queries[i].OcclusionQuery = null;
                }
            }

            if (CulledObjects != null)
            {
                CulledObjects.Clear();
            }
        }
    }
}
