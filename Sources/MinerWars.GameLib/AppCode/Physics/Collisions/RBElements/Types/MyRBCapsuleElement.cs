#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Capsule element
    /// </summary>
    class MyRBCapsuleElement: MyRBElement
    {        
        #region interface

        public MyRBCapsuleElement()
        {
        }

        public MyRBCapsuleElement(float radius, float height)
        {
            m_Radius = radius;
            m_Height = height;
        }

        public override bool LoadFromDesc(MyElementDesc desc)
        {
            if (!desc.IsValid())
                return false;

            if (!base.LoadFromDesc(desc))
                return false;

            SetRadius(((MyRBCapsuleElementDesc)desc).m_Radius);
            SetHeight(((MyRBCapsuleElementDesc)desc).m_Height);
            return true;
        }

        public float Radius { get { return this.m_Radius; } set { this.SetRadius(value); } }
        public float Height { get { return this.m_Height; } set { this.SetHeight(value); } }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_CAPSULE; }

        #endregion

        #region members
        private float m_Radius;
        private float m_Height;
        #endregion

        #region implementation
        void SetRadius(float radius)
        {
            m_Radius = radius;

            Flags |= MyElementFlag.EF_AABB_DIRTY;            

            //if (GetRigidBody() != null)
                //MyPhysics.physicsSystem.GetRigidBodyModule().AddActiveRigid(GetRigidBody());
        }

        void SetHeight(float height)
        {
            m_Height = height;

            Flags |= MyElementFlag.EF_AABB_DIRTY;            

            //if (GetRigidBody() != null)
                //MyPhysics.physicsSystem.GetRigidBodyModule().AddActiveRigid(GetRigidBody());
        }

        public override void UpdateAABB()
        {
            Vector3 origin = GetGlobalTransformation().Translation;

            m_AABB.Max = origin;
            m_AABB.Min = origin;

            Vector3 extent = new Vector3();
            extent.X = m_Height + m_Radius;
            extent.Y = m_Height + m_Radius;
            extent.Z = m_Height + m_Radius;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB, extent);

            base.UpdateAABB();
        }

        #endregion
    }
}