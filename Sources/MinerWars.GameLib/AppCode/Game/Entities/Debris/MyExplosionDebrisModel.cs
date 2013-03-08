using System.Collections.Generic;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Physics;
using SysUtils.Utils;

using System;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Import;

namespace MinerWars.AppCode.Game.Entities
{
    class MyExplosionDebrisModel : MyExplosionDebrisBase
    {
        private const MyMaterialType MODEL_DEBRIS_MATERIAL_TYPE = MyMaterialType.METAL;
        static MyObjectsPool<MyExplosionDebrisModel> m_objectPool = null;

        static readonly List<BoundingSphere> m_debugVoxelSpheres = new List<BoundingSphere>();

        static private readonly List<Vector3> m_positions = new List<Vector3>(MyExplosionsConstants.APPROX_NUMBER_OF_DEBRIS_OBJECTS_PER_MODEL_EXPLOSION);


        public virtual void Init(List<MyRBElementDesc> collisionPrimitives, MyModelsEnum modelEnum)
        {
            float randomSize = MyMwcUtils.GetRandomFloat(0, 1);
            float mass = MathHelper.Lerp(2500, 5000, randomSize); //  The mass can't be too low, because then debris objects are too fast. This values aren't real, but works.
            base.Init(modelEnum, null, MODEL_DEBRIS_MATERIAL_TYPE, 1.0f, collisionPrimitives, mass);
            Physics.MaxLinearVelocity = 200.0f;

            Physics.CollisionLayer = MyConstants.COLLISION_LAYER_MODEL_DEBRIS;

            MyModels.OnContentLoaded += InitDrawTechniques;
        }

        protected override void Start(Vector3 position, float scale, MyGroupMask groupMask, bool explosionType)
        {
            base.Start(position, scale, groupMask, explosionType);

            //apply random rotation impulse
            Physics.AngularVelocity = new Vector3(MyMwcUtils.GetRandomRadian(), MyMwcUtils.GetRandomRadian(), MyMwcUtils.GetRandomRadian());
            Physics.Enabled = true;

            InitDrawTechniques();
        }

        //  This method must be called when this object dies or is removed
        //  E.g. it removes lights, sounds, etc
        public override void Close()
        {
            MyModels.OnContentLoaded -= InitDrawTechniques;
            m_objectPool.Deallocate(this);
            base.Close();
        }

        public override void InitDrawTechniques()
        {
            InitDrawTechniques(MyMeshDrawTechnique.MESH);
        }

        public static void CreateExplosionDebris(ref BoundingSphere explosionSphere, MyGroupMask groupMask, MyEntity entity, MyVoxelMap voxelMap)
        {
            CreateExplosionDebris(ref explosionSphere, groupMask, entity, voxelMap, ref entity.GetModelLod0().BoundingBox);
        }

        public static void CreateExplosionDebris(ref BoundingSphere explosionSphere, MyGroupMask groupMask, MyEntity entity, MyVoxelMap voxelMap, ref BoundingBox bb)
        {
            //  Number of debris is random, but not more than size of the offset array
            float scaleMul = explosionSphere.Radius / 4.0f;

            GeneratePositions(bb);

            foreach (Vector3 positionInLocalSpace in m_positions)
            {
                var positionInWorldSpace = Vector3.Transform(positionInLocalSpace, entity.WorldMatrix);

                MyExplosionDebrisModel newObj = m_objectPool.Allocate(true);

                if (newObj == null) continue;

                //  Check if new object won't intersect any existing triangle - because if yes, then it will decrease JLX performace a lot
                float randomNewScale = MyMwcUtils.GetRandomFloat(scaleMul / 4, scaleMul);
                var sphere = new BoundingSphere(positionInWorldSpace, newObj.m_modelLod0.BoundingSphere.Radius * randomNewScale);

                MyEntity myEntitiesGetIntersectionWithSphere = MyEntities.GetIntersectionWithSphere(ref sphere);
                if ((myEntitiesGetIntersectionWithSphere == null || myEntitiesGetIntersectionWithSphere == entity) && (voxelMap == null || !voxelMap.DoOverlapSphereTest(sphere.Radius, sphere.Center)))
                {
                    if (Vector3.DistanceSquared(positionInWorldSpace, explosionSphere.Center) > MyMwcMathConstants.EPSILON_SQUARED)
                    {
                        newObj.Start(positionInWorldSpace, randomNewScale, groupMask, true);
                    newObj.Physics.LinearVelocity = GetDirection(positionInWorldSpace, explosionSphere.Center) * MyExplosionsConstants.EXPLOSION_DEBRIS_SPEED;
                    MyEntities.Add(newObj);

                        if (MyExplosion.DEBUG_EXPLOSIONS)
                            m_debugVoxelSpheres.Add(sphere);
                    }
                }
                else
                {
                    // Put back to object pool
                    newObj.Close();
                }
            }
        }

        private static Vector3 GetDirection(Vector3 position, Vector3 sphereCenter)
        {
            Vector3 dist = position - sphereCenter;
            if (MyUtils.IsValid(dist) && MyMwcUtils.HasValidLength(dist))
            {
                return Vector3.Normalize(dist);
            }
            else
            {
                return MyMwcUtils.GetRandomVector3Normalized();
            }
        }

        private static void GeneratePositions(BoundingBox boundingBox)
        {
            m_positions.Clear();

            Vector3 minMax = boundingBox.Max - boundingBox.Min;
            float product = minMax.X * minMax.Y * minMax.Z;

            float a3 = MyExplosionsConstants.APPROX_NUMBER_OF_DEBRIS_OBJECTS_PER_MODEL_EXPLOSION / product;

            float a = (float)Math.Pow(a3, 1f / 3.0f);

            Vector3 minMaxScaled = minMax * a;

            int maxX = (int)Math.Ceiling(minMaxScaled.X);
            int maxY = (int)Math.Ceiling(minMaxScaled.Y);
            int maxZ = (int)Math.Ceiling(minMaxScaled.Z);

            Vector3 offset = new Vector3(minMax.X / maxX, minMax.Y / maxY, minMax.Z / maxZ);

            Vector3 origin = boundingBox.Min + 0.5f * offset;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        Vector3 pos = origin + new Vector3(x * offset.X, y * offset.Y, z * offset.Z);

                        //  Phys object created near/inside the external 'explosion force' are making JLX freeze, so we don't create debris too close to the center of explosion
                        if (Vector3.Distance(pos, Vector3.Zero) >= MyConstants.SAFE_DISTANCE_FOR_ADD_WORLD_FORCE_JLX)
                        {
                            m_positions.Add(pos);
                        }
                    }
                }
            }
        }

        internal static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyExplosionDebrisModel.LoadData");

            MyMwcLog.WriteLine("MyExplosionDebrisModel.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            PreallocateObjects();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyExplosionDebrisModel.LoadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData() 
        {
        }

        static void PreallocateObjects()
        {
            if (m_objectPool == null)
                m_objectPool = new MyObjectsPool<MyExplosionDebrisModel>(MyExplosionsConstants.MAX_EXPLOSION_DEBRIS_OBJECTS);
            m_objectPool.DeallocateAll();
            
            List<MyRBElementDesc> collisionPrimitives = new List<MyRBElementDesc>();

            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();
            MyRBSphereElementDesc sphereDesc = physobj.GetRBSphereElementDesc();
            sphereDesc.SetToDefault();
            sphereDesc.m_RBMaterial = MyMaterialsConstants.GetMaterialProperties(MODEL_DEBRIS_MATERIAL_TYPE).PhysicsMaterial;

            collisionPrimitives.Add(sphereDesc);

            int counter = 0;
            foreach (var item in m_objectPool.GetPreallocatedItemsArray())
            {
                MyModelsEnum modelEnum = (MyModelsEnum)((int)MyModelsEnum.Debris1 + counter % 31);
                MyModel model = MyModels.GetModelOnlyData(modelEnum);
                BoundingSphere boundingSphere = model.BoundingSphere;

                sphereDesc.m_Radius = boundingSphere.Radius;
                sphereDesc.m_Matrix.Translation = boundingSphere.Center;

                item.Value.Init(collisionPrimitives, modelEnum);
                counter++;
            }     
        }

        public override bool DebugDraw()
        {
            foreach (BoundingSphere sphere in m_debugVoxelSpheres)
            {
                MyDebugDraw.DrawSphereWireframe(sphere.Center, sphere.Radius, Color.Green.ToVector3(), 1);
            }
            return base.DebugDraw();
        }
    }
}
