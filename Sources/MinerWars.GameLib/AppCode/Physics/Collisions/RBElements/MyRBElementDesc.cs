#region Using Statements

using MinerWarsMath;
using MinerWars.AppCode.Game.Models;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// RBElement desctriptor
    /// </summary>
    abstract class MyRBElementDesc: MyElementDesc
    {        
        public Matrix           m_Matrix;
        public ushort           m_CollisionLayer;
        public MyRBMaterial     m_RBMaterial;

        public virtual MyRBElementType GetElementType() { return MyRBElementType.ET_UNKNOWN; }

        public virtual void SetToDefault()
        {
            m_Matrix = Matrix.Identity;
            m_CollisionLayer = 0;
            m_RBMaterial = null;
        }

        public override bool IsValid()
        {
            if(m_CollisionLayer > 31)
                return false;

            if (m_RBMaterial == null)
                return false;

            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Box element descriptor
    /// </summary>
    class MyRBBoxElementDesc: MyRBElementDesc
    {
        public Vector3 m_Size;

        public override void SetToDefault()
        {
            base.SetToDefault();

            m_Size.X = 1.0f;
            m_Size.Y = 1.0f;
            m_Size.Z = 1.0f;
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            if (m_Size.X <= 0.0f)
                return false;

            if (m_Size.Y <= 0.0f)
                return false;

            if (m_Size.Z <= 0.0f)
                return false;

            return true;
        }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_BOX; }
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sphere element descriptor
    /// </summary>
    class MyRBSphereElementDesc: MyRBElementDesc
    {
        public float m_Radius;

        public override void SetToDefault()
        {
            base.SetToDefault();
            m_Radius = 1.0f;
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            if (m_Radius <= 0.0f)
                return false;

            return true;
        }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_SPHERE; }
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Capsule element descriptor - capsules are not implemented now
    /// </summary>
    class MyRBCapsuleElementDesc : MyRBElementDesc
    {
        public float m_Radius;
        public float m_Height;

        public override void SetToDefault()
        {
            base.SetToDefault();
            m_Radius = 1.0f;
            m_Height = 1.0f;
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;
            if (m_Radius <= 0.0f)
                return false;

            if (m_Height <= 0.0f)
                return false;

            return true;
        }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_CAPSULE; }
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Triangle mesh descriptor
    /// </summary>
    class MyRBTriangleMeshElementDesc: MyRBElementDesc
    {

        public override void SetToDefault()
        {
            base.SetToDefault();

            Model = null;
            ModelLOD0 = null;
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            if(Model == null)
                return false;

            return true;
        }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_TRIANGLEMESH; }

        public MyModel Model;
        public MyModel ModelLOD0; 
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Voxel map element descriptor
    /// </summary>
    class MyRBVoxelElementDesc : MyRBElementDesc
    {
        public Vector3 m_Size;

        public override void SetToDefault()
        {
            base.SetToDefault();

        }

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            return true;
        }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_VOXEL; }
    }

}