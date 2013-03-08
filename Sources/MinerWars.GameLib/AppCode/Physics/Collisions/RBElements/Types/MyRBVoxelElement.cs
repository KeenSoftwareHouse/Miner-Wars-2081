#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Voxel element
    /// </summary>
    class MyRBVoxelElement: MyRBElement
    {
        #region interface

        public MyRBVoxelElement()
        {
            m_Size = Vector3.Zero;
        }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_VOXEL; }

        #endregion

        #region members

        private Vector3 m_Size;

        #endregion

        #region implementation

        public override bool LoadFromDesc(MyElementDesc desc)
        {
            if (!desc.IsValid())
                return false;

            if (!base.LoadFromDesc(desc))
                return false;

            MyRBVoxelElementDesc vd = (MyRBVoxelElementDesc) desc;

            Size = vd.m_Size;

            return true;
        }

        public Vector3 Size
        {
            set { m_Size = value; Flags |= MyElementFlag.EF_AABB_DIRTY; }
            get { return m_Size; }
        }

        public override void UpdateAABB()
        {
            Vector3 origin = GetGlobalTransformation().Translation;
            Vector3 sizeHalf = m_Size/2;
            m_AABB.Min = origin - sizeHalf;
            m_AABB.Max = origin + sizeHalf;            
            base.UpdateAABB();
        }

        #endregion
    }
}