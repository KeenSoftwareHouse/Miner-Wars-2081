#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// RBElement that is making the shape of a rigid body. This is what defines the collision of a rigid body.
    /// </summary>
    abstract class MyRBElement : MyElement
    {
        #region interface

        public MyRigidBody GetRigidBody() { return m_RigidBody; }

        public virtual MyRBElementType GetElementType() { return MyRBElementType.ET_UNKNOWN; }

        public override bool LoadFromDesc(MyElementDesc desc)
        {
            base.LoadFromDesc(desc);

            CollisionLayer = ((MyRBElementDesc)desc).m_CollisionLayer;

            LocalTransformation = ((MyRBElementDesc)desc).m_Matrix;
            RBElementMaterial = ((MyRBElementDesc)desc).m_RBMaterial;

            return true;
        }

        public MyRBMaterial RBElementMaterial { get { return this.m_RBMaterial; } set { this.m_RBMaterial = value; } }

        /// <summary>
        /// Group mask used for collision filtering
        /// </summary>
        public MyGroupMask GroupMask { get { return this.m_GroupMask; } set { this.m_GroupMask = value; } }

        public override Matrix GetGlobalTransformation() 
        { 
            return m_Matrix * m_RigidBody.Matrix; 
        }

        /// <summary>
        /// Closest point computation
        /// </summary>
        public virtual void GetClosestPoint(Vector3 point, ref Vector3 closestPoint, ref Vector3 normal, ref bool penetration, ref uint customData)
        {

        }

        #endregion

        #region members

        private MyRBMaterial m_RBMaterial;
        private MyGroupMask m_GroupMask;
        private MyRigidBody m_RigidBody;
        private List<MyRBElementInteraction> m_ElementInteractions;

        #endregion

        #region implementation

        public List<MyRBElementInteraction> GetRBElementInteractions() { return m_ElementInteractions; }

        public MyRBElement()
        {
            m_RBMaterial = new MyRBMaterial(0.5f, 0.5f, 0.7f, 0);
            m_RigidBody = null;
            m_ElementInteractions = new List<MyRBElementInteraction>(2);

            Flags = MyElementFlag.EF_RB_ELEMENT | MyElementFlag.EF_AABB_DIRTY;
            m_GroupMask = new MyGroupMask();
        }

        public void SetRigidBody(MyRigidBody rbo)
        {
            m_RigidBody = rbo;
        }

        #endregion
    }
}