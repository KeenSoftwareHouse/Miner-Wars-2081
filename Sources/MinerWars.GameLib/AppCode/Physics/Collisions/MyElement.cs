#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Element used as a base class for the prunning structure. 
    /// </summary>
    abstract class MyElement
    {
        public BoundingBox GetWorldSpaceAABB()
        {
            if ((Flags & MyElementFlag.EF_AABB_DIRTY) > 0)
            {
                UpdateAABB();
            }

            return m_AABB;
        }

        public virtual bool LoadFromDesc(MyElementDesc desc)
        {
            m_Guid = GUID_COUNTER;

            if (GUID_COUNTER > 65000)
            {
                GUID_COUNTER = 0;
            }
            GUID_COUNTER++;

            return true;
        }

        public virtual Matrix GetGlobalTransformation() { return m_Matrix; }

        public Matrix LocalTransformation { get { return this.m_Matrix; } set { this.m_Matrix = value; } }

        public Vector3 LocalPosition { get { return this.m_Matrix.Translation; } set { this.m_Matrix.Translation = value; } }

        public System.UInt16 CollisionLayer { get { return this.m_CollisionLayer; } set { this.m_CollisionLayer = value; } }

        public MyElementFlag Flags;// { get { return m_flags; } }

        /// <summary>
        /// Update of aabb if necessary, implementation in shape elements
        /// </summary>
        public virtual void UpdateAABB()
        {
            Flags &= ~MyElementFlag.EF_AABB_DIRTY;
        }

        public int GUID { get { return m_Guid; } set { m_Guid = value; } }

        protected Matrix m_Matrix;
        private System.UInt16 m_CollisionLayer;
        protected BoundingBox m_AABB;
        private int m_ProxyData;
        private int m_ShadowProxyData;
        private int m_Guid;
        private static int GUID_COUNTER = 0;
        public static int PROXY_UNASSIGNED = int.MaxValue;

        public MyElement()
        {            
            m_Matrix = Matrix.Identity;
            m_CollisionLayer = 0;
            Flags = MyElementFlag.EF_AABB_DIRTY;
            m_AABB = new BoundingBox();        
            m_ProxyData = PROXY_UNASSIGNED;
            m_ShadowProxyData = PROXY_UNASSIGNED;

            m_Guid = GUID_COUNTER;

            if (GUID_COUNTER == int.MaxValue)
            {
                GUID_COUNTER = 0;
            }
            GUID_COUNTER++;
        }

        public int ProxyData { get { return m_ProxyData; } set { m_ProxyData = value; } }
        public int ShadowProxyData { get { return m_ShadowProxyData; } set { m_ShadowProxyData = value; } }

    }
}
