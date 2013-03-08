#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Utils;
#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Box element
    /// </summary>
    class MyRBBoxElement: MyRBElement
    {
        #region interface

        public MyRBBoxElement()
        {
            m_points = new Vector3[8];
        }

        public MyRBBoxElement(Vector3 size)
            : this()
        {
            m_Size = size;
        }

        public override bool LoadFromDesc(MyElementDesc desc)
        {
            if (!desc.IsValid())
                return false;

            if (!base.LoadFromDesc(desc))
                return false;

            SetSize(((MyRBBoxElementDesc)desc).m_Size);
            return true;
        }

        public Vector3 Size { get { return this.m_Size * 2.0f; } set { this.SetSize(value); } }

        public override MyRBElementType GetElementType() { return MyRBElementType.ET_BOX; }

        #endregion

        #region members
        private Vector3 m_Size;
        //private Vector3 m_Extent;
        private Vector3[] m_points;
        #endregion

        #region implementation

        public override void UpdateAABB()
        {
            Matrix matrix = GetGlobalTransformation();            

            m_AABB = m_AABB.CreateInvalid();
            
            BoundingBox box = new BoundingBox(-m_Size, m_Size);
            box.GetCorners(m_points);
            Vector3 point2;
            Vector3 point1;

            foreach (Vector3 point in m_points)
            {
                point1 = point;
                Vector3.Transform(ref point1, ref matrix, out point2);
                MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB, point2);
            }



            /*
            Vector3 origin = matrix.Translation;
            m_AABB.Min = origin;
            m_AABB.Max = origin;

            Vector3 rotSize = Vector3.TransformNormal(m_Size, GetGlobalTransformation());

            m_Extent.X = rotSize.X;
            m_Extent.Y = rotSize.Y;
            m_Extent.Z = rotSize.Z;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB, origin + m_Extent);

            m_Extent.X = -rotSize.X;
            m_Extent.Y = rotSize.Y;
            m_Extent.Z = rotSize.Z;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB, origin+m_Extent);

            m_Extent.X = -rotSize.X;
            m_Extent.Y = -rotSize.Y;
            m_Extent.Z = rotSize.Z;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB, origin+m_Extent);

            m_Extent.X = -rotSize.X;
            m_Extent.Y = -rotSize.Y;
            m_Extent.Z = -rotSize.Z;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB, origin+m_Extent);

            m_Extent.X = rotSize.X;
            m_Extent.Y = -rotSize.Y;
            m_Extent.Z = rotSize.Z;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB,origin+ m_Extent);

            m_Extent.X = rotSize.X;
            m_Extent.Y = -rotSize.Y;
            m_Extent.Z = -rotSize.Z;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB,origin+ m_Extent);

            m_Extent.X = rotSize.X;
            m_Extent.Y = rotSize.Y;
            m_Extent.Z = -rotSize.Z;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB,origin+ m_Extent);

            m_Extent.X = -rotSize.X;
            m_Extent.Y = rotSize.Y;
            m_Extent.Z = -rotSize.Z;

            MyPhysicsUtils.BoundingBoxAddPoint(ref m_AABB,origin+ m_Extent);
            */
            base.UpdateAABB();
        }

        void SetSize(Vector3 size)
        {
            m_Size = size * 0.5f;
            Flags |= MyElementFlag.EF_AABB_DIRTY;            

            //if (GetRigidBody() != null)
              //  MyPhysics.physicsSystem.GetRigidBodyModule().AddActiveRigid(GetRigidBody());
        }

        public override void GetClosestPoint(Vector3 point, ref Vector3 closestPoint, ref Vector3 normal, ref bool penetration, ref uint customData)
        {
            closestPoint = point;
            bool p = true;

            Vector3 corner = m_Size*0.5f;

            if (point.X < -corner.X)
            {
                closestPoint.X = -corner.X;
                p = false;
            }
            else if (point.X > corner.X)
            {
                closestPoint.X = corner.X;
                p = false;
            }

            if (point.Y < -corner.Y)
            {
                closestPoint.Y = -corner.Y;
                p = false;
            }
            else if (point.Y > corner.Y)
            {
                closestPoint.Y = corner.Y;
                p = false;
            }

            if (point.Z < -corner.Z)
            {
                closestPoint.Z = -corner.Z;
                p = false;
            }
            else if (point.Z > corner.Z)
            {
                closestPoint.Z = corner.Z;
                p = false;
            }

            if (p)
            {
                float pX = corner.X - System.Math.Abs(closestPoint.X);
                float pY = corner.Y - System.Math.Abs(closestPoint.Y);
                float pZ = corner.Z - System.Math.Abs(closestPoint.Z);
                if (pX < pY)
                {
                    if (pX < pZ)
                    {
                        if (closestPoint.X > 0) closestPoint.X += pX;
                        else closestPoint.X -= pX;
                    }
                    else
                    {
                        if (closestPoint.Z > 0) closestPoint.Z += pZ;
                        else closestPoint.Z -= pZ;
                    }
                }
                else
                {
                    if (pY < pZ)
                    {
                        if (closestPoint.Y > 0) closestPoint.Y += pY;
                        else closestPoint.Y -= pY;
                    }
                    else
                    {
                        if (closestPoint.Z > 0) closestPoint.Z += pZ;
                        else closestPoint.Z -= pZ;
                    }
                }
            }

            penetration = p;

            if (p)
                normal = closestPoint - point;
            else
                normal = point - closestPoint;
        }

        #endregion
    }
}