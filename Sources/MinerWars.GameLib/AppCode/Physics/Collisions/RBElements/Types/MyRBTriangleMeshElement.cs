#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Triangle mesh element using the model data
    /// </summary>
    class MyRBTriangleMeshElement: MyRBElement
    {
        #region interface

        public MyRBTriangleMeshElement()
        {
         
        }

        public MyModel Model { get { return m_Model; } set { SetModel(value); } }
        public MyModel ModelLOD0 { get { return m_ModelLOD0; } set { SetModelLOD0(value); } }

        protected void SetModel(MyModel model) 
        { 
            m_Model = model;
            Flags |= MyElementFlag.EF_AABB_DIRTY;            

            //if (GetRigidBody() != null)
              //  MyPhysics.physicsSystem.GetRigidBodyModule().AddActiveRigid(GetRigidBody());
        }


        protected void SetModelLOD0(MyModel model)
        {
            m_ModelLOD0 = model;
            Flags |= MyElementFlag.EF_AABB_DIRTY;

            //if (GetRigidBody() != null)
            //  MyPhysics.physicsSystem.GetRigidBodyModule().AddActiveRigid(GetRigidBody());
        }
        
        public override MyRBElementType GetElementType() { return MyRBElementType.ET_TRIANGLEMESH; }

        #endregion

        #region members
        MyModel m_Model;
        MyModel m_ModelLOD0;
        #endregion

        #region implementation

        public override void UpdateAABB()
        {
            base.UpdateAABB();

            m_AABB = MyMath.CreateInvalidAABB();
            
            Matrix worldMat = GetGlobalTransformation();
            
            Vector3 center = m_Model.BoundingBox.GetCenter();
            Vector3 extent = (m_Model.BoundingBox.Max - m_Model.BoundingBox.Min) * 0.5f;

            Vector3 vec = center;
            vec.X += extent.X;
            vec.Y += extent.Y;
            vec.Z += extent.Z;

            Vector3 vecTmp = Vector3.Transform(vec, worldMat);
            m_AABB = m_AABB.Include(ref vecTmp);

            vec = center;
            vec.X += -extent.X;
            vec.Y += extent.Y;
            vec.Z += extent.Z;

            vecTmp = Vector3.Transform(vec, worldMat);
            m_AABB = m_AABB.Include(ref vecTmp);

            vec = center;
            vec.X += -extent.X;
            vec.Y += -extent.Y;
            vec.Z += extent.Z;

            vecTmp = Vector3.Transform(vec, worldMat);
            m_AABB = m_AABB.Include(ref vecTmp);

            vec = center;
            vec.X += -extent.X;
            vec.Y += -extent.Y;
            vec.Z += -extent.Z;

            vecTmp = Vector3.Transform(vec, worldMat);
            m_AABB = m_AABB.Include(ref vecTmp);

            vec = center;
            vec.X += extent.X;
            vec.Y += -extent.Y;
            vec.Z += extent.Z;

            vecTmp = Vector3.Transform(vec, worldMat);
            m_AABB = m_AABB.Include(ref vecTmp);

            vec = center;
            vec.X += extent.X;
            vec.Y += -extent.Y;
            vec.Z += -extent.Z;

            vecTmp = Vector3.Transform(vec, worldMat);
            m_AABB = m_AABB.Include(ref vecTmp);

            vec = center;
            vec.X += extent.X;
            vec.Y += extent.Y;
            vec.Z += -extent.Z;

            vecTmp = Vector3.Transform(vec, worldMat);
            m_AABB = m_AABB.Include(ref vecTmp);

            vec = center;
            vec.X += -extent.X;
            vec.Y += extent.Y;
            vec.Z += -extent.Z;

            vecTmp = Vector3.Transform(vec, worldMat);
            m_AABB = m_AABB.Include(ref vecTmp);
        }

        public override bool LoadFromDesc(MyElementDesc desc)
        {
            if (!desc.IsValid())
                return false;

            if (!base.LoadFromDesc(desc))
                return false;

            MyRBTriangleMeshElementDesc tmDesc = (MyRBTriangleMeshElementDesc)desc;

            SetModel(tmDesc.Model);
            if (tmDesc.ModelLOD0 == null)
                SetModelLOD0(tmDesc.Model);
            else
                SetModelLOD0(tmDesc.ModelLOD0);
            return true;
        }

        #endregion
    }
}