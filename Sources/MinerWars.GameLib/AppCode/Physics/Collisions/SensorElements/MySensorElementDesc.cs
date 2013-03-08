#region Using Statements

using MinerWarsMath;
using MinerWars.AppCode.Game.Models;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sensor descriptor
    /// </summary>
    abstract class MySensorElementDesc : MyElementDesc
    {
        public Matrix m_Matrix;
        public System.UInt32 m_CollisionLayer;

        public virtual MySensorElementType GetElementType() { return MySensorElementType.ET_UNKNOWN; }

        public virtual void SetToDefault()
        {
            m_Matrix = Matrix.Identity;
            m_CollisionLayer = 0;            
        }

        public override bool IsValid()
        {
            if (m_CollisionLayer > 31)
                return false;


            return true;
        }
    }



    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sensor sphere element descriptor
    /// </summary>
    class MySphereSensorElementDesc : MySensorElementDesc
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

        public override MySensorElementType GetElementType() { return MySensorElementType.ET_SPHERE; }
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Box sphere element descriptor
    /// </summary>
    class MyBoxSensorElementDesc : MySensorElementDesc
    {
        public Vector3 Size;

        public override void SetToDefault()
        {
            base.SetToDefault();
            Size = new Vector3(1f, 1f, 1f);
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            if (Size.Length() == 0.0f)
                return false;

            return true;
        }

        public override MySensorElementType GetElementType() { return MySensorElementType.ET_BOX; }
    }
}