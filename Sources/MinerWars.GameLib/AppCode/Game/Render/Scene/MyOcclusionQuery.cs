using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.Render
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MathHelper = MinerWarsMath.MathHelper;
    using SysUtils.Utils;

    static class MyOcclusionQueries
    {

        /*internal class Friend
        {
            public void Add(MyOcclusionQuery query)
            {
                MyOcclusionQueries.Add(query);
            }

            public void Remove(MyOcclusionQuery query)
            {
                MyOcclusionQueries.Remove(query);
            }
        } */

        static HashSet<MyOcclusionQuery> m_existingQueries = new HashSet<MyOcclusionQuery>(); 
        static Stack<MyOcclusionQuery> m_queriesStack = new Stack<MyOcclusionQuery>(256);
        static Device m_device;

        public static MyOcclusionQuery Get()
        {
            MyOcclusionQuery query = null;
            if (m_queriesStack.Count > 0)
            {
                query = m_queriesStack.Pop();
            }
            else
            {
                query = MyOcclusionQuery.CreateQuery();
                query.LoadContent(m_device);
                m_existingQueries.Add(query);
            }

            return query;
        }

        public static void Return(MyOcclusionQuery query)
        {
            m_queriesStack.Push(query);
        }

        public static void LoadContent(Device device)
        {
            MyMwcLog.WriteLine("MyOcclusionQueries.UnloadEffects - START");

            m_device = device;
            m_queriesStack.Clear();

            foreach (var q in m_existingQueries)
            {
                q.LoadContent(device);
                m_queriesStack.Push(q);
            }

            MyMwcLog.WriteLine("MyOcclusionQueries.UnloadEffects - END");
        }

        public static void UnloadContent()
        {
            m_queriesStack.Clear();

            foreach (var q in m_existingQueries)
            {
                q.UnloadContent();
            }

            m_device = null;
        }
    }


    class MyOcclusionQuery :/* MyOcclusionQueries.Friend,*/ IDisposable
    {
        Query dxQuery;

        // Because Xna OcclusionQuery returns IsComplete = false when query not started
        bool started = false;


        public static MyOcclusionQuery CreateQuery()
        {
            return new MyOcclusionQuery();
        }

        private MyOcclusionQuery()
        {
        }

        public void Begin()
        {
            started = true;
            dxQuery.Issue(Issue.Begin);
        }

        public void End()
        {
            dxQuery.Issue(Issue.End);
        }

        public void LoadContent(Device device)
        {
            System.Diagnostics.Debug.Assert(dxQuery == null);
            dxQuery = new Query(device, QueryType.Occlusion);
        }

        public void UnloadContent()
        {
            if (dxQuery != null && !dxQuery.IsDisposed)
            {
                dxQuery.Dispose();
                dxQuery = null;
            }
        }

        public int PixelCount
        {
            get
            {
                if (!started) return 0;
                int pixels = 0;

                if (dxQuery.GetData<int>(out pixels, false))
                    return pixels;

                return 0;
            }
        }

        public bool IsComplete
        {
            get
            {
                if (!started) return true; // Because of XNA
                return CheckStatus(false);
            }
        }

        private bool CheckStatus(bool flush)
        {
            int data;
            if (dxQuery != null) //TODO:can be called from PrepareEntitiesTask...
                return dxQuery.GetData<int>(out data, flush);

            return false;
        }

        public object Tag { get; set; }

        public void Dispose()
        {
            UnloadContent();
        }
    }
}
