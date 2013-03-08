#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sphere element
    /// </summary>
    class MyRBSphereElement: MyRBElement
    {
        #region interface

        public MyRBSphereElement()
        {

        }

        public MyRBSphereElement(float radius)
        {
            m_Radius = radius;
        }

        public override bool LoadFromDesc(MyElementDesc desc)
        {
            if(!desc.IsValid())
                return false;

            if(!base.LoadFromDesc(desc))
                return false;

            SetRadius(((MyRBSphereElementDesc) desc).m_Radius);
            return true;
        }

        public float Radius { get { return this.m_Radius; } set { this.SetRadius(value); } }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_SPHERE; }

        #endregion

        public override void UpdateAABB()
        {            
            Vector3 origin = GetGlobalTransformation().Translation;
            m_AABB.Min = origin - m_Extent;
            m_AABB.Max = origin + m_Extent;
            base.UpdateAABB();
        }

        #region members
        private float   m_Radius;
        private Vector3 m_Extent;
        #endregion

        #region implementation

        void SetRadius(float radius)
        {
            m_Radius = radius;
            m_Extent.X = m_Radius;
            m_Extent.Y = m_Radius;
            m_Extent.Z = m_Radius;

            Flags |= MyElementFlag.EF_AABB_DIRTY;            

            //if(GetRigidBody() != null)
              //  MyPhysics.physicsSystem.GetRigidBodyModule().AddActiveRigid(GetRigidBody());
        }

        // in local space!
        public override void GetClosestPoint(Vector3 point, ref Vector3 closestPoint, ref Vector3 normal, ref bool penetration, ref uint customData)
        {
            float dist = point.Length();
            penetration = (dist < m_Radius);

            //Handle case when point is in sphere origin
            if (dist == 0)
            {
                normal.X = 1.0f;
                normal.Y = 0.0f;
                normal.Z = 0.0f;
                closestPoint.X = m_Radius;
                closestPoint.Y = 0.0f;
                closestPoint.Z = 0.0f;
            }
            else
            {
                float d = dist;
                normal = point/d;
                closestPoint = point*(m_Radius / d);
            }

            customData = 0;
        }

#endregion
    }
}