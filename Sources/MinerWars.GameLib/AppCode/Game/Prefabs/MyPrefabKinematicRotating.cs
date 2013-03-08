using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Managers.PhysicsManager.Physics;
using MinerWarsCustomContentImporters;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabKinematicRotating : MyPrefabBase
    {
        private List<MyPrefabKinematicRotatingPart> m_parts;

        //private bool m_on;

        //public bool On 
        //{
        //    get { return m_on; }
        //    set 
        //    {
        //        m_on = value;
        //        foreach (MyPrefabKinematicRotatingPart part in m_parts) 
        //        {
        //            part.On = value;
        //        }
        //    }
        //}

        protected override void EnabledChanged()
        {
            base.EnabledChanged();
            foreach (MyPrefabKinematicRotatingPart part in m_parts)
            {
                part.On = Enabled;
            }
        }

        public MyPrefabKinematicRotating(MyPrefabContainer owner) 
            : base(owner)
        {
            m_parts = new List<MyPrefabKinematicRotatingPart>();
        }

        public override void Init(string displayName, Microsoft.Xna.Framework.Vector3 relativePosition, Microsoft.Xna.Framework.Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {                        
            m_config = prefabConfig;
            MyPrefabConfigurationKinematicRotating config = (MyPrefabConfigurationKinematicRotating)prefabConfig;                        

            base.Init(displayName, relativePosition, localOrientation, objectBuilder, prefabConfig);
            Physics.RemoveAllElements();                                         

            // create the box
            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();
            MyRBTriangleMeshElementDesc trianglemeshDesc = physobj.GetRBTriangleMeshElementDesc();

            trianglemeshDesc.SetToDefault();
            trianglemeshDesc.m_Model = ModelLod0;
            trianglemeshDesc.m_RBMaterial = MyMaterialsConstants.GetMaterialProperties(config.MaterialType).PhysicsMaterial; ;


            MyRBTriangleMeshElement trEl = (MyRBTriangleMeshElement)physobj.CreateRBElement(trianglemeshDesc);

            this.Physics = new MyPhysicsBody(this, 1.0f, RigidBodyFlag.RBF_RBO_STATIC) { MaterialType = config.MaterialType };
            this.Physics.Enabled = true;            
            this.Physics.CollisionLayer = MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC;
            this.Physics.AddElement(trEl, true);

            MyModel model = MyModels.GetModelOnlyDummies(m_config.ModelLod0Enum);

            foreach (var dummyKVP in model.Dummies) 
            {
                if (dummyKVP.Key.StartsWith("Dummy")) 
                {
                    MyModelDummy dummy = dummyKVP.Value;
                    MyModelsEnum rotatingPartModel = MyModels.GetModelEnumByAssetName(dummy.CustomData["LINKEDMODEL"].ToString());
                    MyPrefabKinematicRotatingPart rotatingPart = new MyPrefabKinematicRotatingPart(this.GetOwner());
                    rotatingPart.Init(this, rotatingPartModel, config.MaterialType, dummy.Matrix, config.RotatingVelocity, true, config.SoundLooping, config.SoundOpening, config.SoundClosing);
                    m_parts.Add(rotatingPart);
                }
            }
            
            AppCode.Physics.MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC, MyConstants.COLLISION_LAYER_MISSILE, true);
            AppCode.Physics.MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC, MyConstants.COLLISION_LAYER_ALL, true);
            AppCode.Physics.MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC, MyConstants.COLLISION_LAYER_MODEL_DEBRIS, true);
            AppCode.Physics.MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC, MyConstants.COLLISION_LAYER_VOXEL_DEBRIS, true);
            AppCode.Physics.MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC, MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC_PART, false);                        

            NeedsUpdate = false;
            EnabledChanged();
            //Enabled = true;
        }

        public void RemovePart(MyPrefabKinematicRotatingPart part) 
        {
            m_parts.Remove(part);
        }
    }
}
