#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// RBMaterial for material properties.
    /// </summary>
    class MyRBMaterial
    {
        #region interface
        public float NominalStaticFriction { get { return this.m_NominalStaticFriction; } set { this.m_NominalStaticFriction = value; } }			

		public float NominalDynamicFriction { get { return this.m_NominalDynamicFriction; } set { this.m_NominalDynamicFriction = value; } }

		public float NominalRestitution { get { return this.m_NominalRestitution; } set { this.m_NominalRestitution = value; } }

		public System.Int32 MaterialGUID { get { return this.m_MaterialGUID; } set { this.m_MaterialGUID = value; } }

        public MyRBMaterial(float nsf, float ndf, float nr, System.Int32 guid)
        {
            m_NominalDynamicFriction = ndf;
            m_NominalRestitution = nr;
            m_NominalStaticFriction = nsf;
            m_MaterialGUID = guid;
        }

        #endregion

        private float		    m_NominalStaticFriction;
        private float		    m_NominalDynamicFriction; 
		private float		    m_NominalRestitution;
        private System.Int32    m_MaterialGUID;
    }
}