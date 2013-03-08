using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    using System.Collections.Generic;
    using CommonLIB.AppCode.Networking;
    using CommonLIB.AppCode.Utils;
    using MinerWarsMath;
    using Models;
    using SysUtils.Utils;
    using Utils;
    
    using KeenSoftwareHouse.Library.Trace;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.CommonLIB.AppCode.Import;


    /// <summary>
    /// This class has two functions: it encapsulated one instance of voxel debris object, and also holds static prepared/preallocated 
    /// object pool for those instances - and if needed adds them to main phys objects collection for short life.
    /// It's because initializing JLX object is slow and we don't want to initialize 20 (or so) objects for every explosion.
    /// </summary>
    class MyExplosionDebrisVoxel : MyExplosionDebrisBase
    {
        class MyPositionOffsetGroup
        {
            public BoundingBox LocalBoundingBox;
            public List<Vector3> Positions;

            public MyPositionOffsetGroup(BoundingBox localBoundingBox)
            {
                LocalBoundingBox = localBoundingBox;
                Positions = new List<Vector3>();
            }
        }

        //  Number of debris objects in one line / row / column
        const int EXPLOSION_DEBRIS_OFFSET_COUNT = 3;

        const MyModelsEnum VOXEL_DEBRIS_MODEL_ENUM = MyModelsEnum.ExplosionDebrisVoxel;

        public static float DebrisScaleLower = 0.0068f; // In size of explosion
        public static float DebrisScaleUpper = 0.0155f; // In size of explosion
        public static float DebrisScaleClamp = 0.5f; // Max size is half size of model

        static MyObjectsPool<MyExplosionDebrisVoxel> m_objectPool = null;

        //  Per-debris object randomization parameters, they influence how debris looks when rendered
        float m_randomizedTextureCoordRandomPositionOffset;
        float m_randomizedTextureCoordScale;

        // List of precalculated position offsets of individual pieces of debris (relative to center of explosion)
        protected static List<Vector3> m_positionOffsets;

        //private const int POSITION_GROUPS_COUNT = 2;
        //private const float POSITION_GROUPS_SPACE = EXPLOSION_DEBRIS_OFFSET_SIZE / (float)(POSITION_GROUPS_COUNT - 1);
        //private static List<MyPositionOffsetGroup> m_positionOffsetGroups;

        static List<BoundingSphere> m_debugVoxelSpheres = new List<BoundingSphere>();


        public virtual void Init(List<MyRBElementDesc> collisionPrimitives)
        {
            float randomSize = MyMwcUtils.GetRandomFloat(0, 1);
            float mass = MathHelper.Lerp(2400, 3700, randomSize);     //  The mass can't be too low, because then debris objects are too fast. This values aren't real, but works.
            mass /= 2.0f;

            base.Init(VOXEL_DEBRIS_MODEL_ENUM, null, MyMaterialType.ROCK, 1.0f, collisionPrimitives, mass);
            this.Physics.MaxLinearVelocity = 200.0f;//added  0001003: Bug A - pool (7-8 Missile Basic)

            this.Physics.CollisionLayer = MyConstants.COLLISION_LAYER_VOXEL_DEBRIS;

            m_randomizedTextureCoordRandomPositionOffset = MathHelper.Lerp(5, 15, randomSize);
            m_randomizedTextureCoordScale = MathHelper.Lerp(8f, 12f, randomSize);

            MyModels.OnContentLoaded += InitDrawTechniques;
        }

        public void Start(Vector3 position, float scale, MyMwcVoxelMaterialsEnum voxelMaterial, MyGroupMask groupMask, bool explosionType)
        {
            base.Start(position, scale, groupMask, explosionType);

            if (explosionType)
            {
                //apply random rotation impulse
                base.Physics.AngularVelocity = new Vector3(MyMwcUtils.GetRandomRadian(), MyMwcUtils.GetRandomRadian(), MyMwcUtils.GetRandomRadian()) * 0.7f;
                if (base.Physics.AngularVelocity.Length() == 0)
                    Debug.Assert(false);

                if (!Physics.Enabled)
                    Physics.Enabled = true;

            }
            else
            {
                if (Physics.Enabled)
                    Physics.Enabled = false;
            }

            VoxelMaterial = voxelMaterial;

            InitDrawTechniques();

            RenderObjects[0].NeedsResolveCastShadow = true;
            RenderObjects[0].FastCastShadowResolve = true;
        }

        public override void InitDrawTechniques()
        {
            InitDrawTechniques(MyMeshDrawTechnique.VOXELS_DEBRIS);
        }

        public float GetRandomizedTextureCoordRandomPositionOffset()
        {
            return m_randomizedTextureCoordRandomPositionOffset;
        }

        public float GetRandomizedTextureCoordScale()
        {
            return m_randomizedTextureCoordScale;
        }

        //  This method must be called when this object dies or is removed
        //  E.g. it removes lights, sounds, etc
        public override void Close()
        {
            MyModels.OnContentLoaded -= InitDrawTechniques;
            m_objectPool.Deallocate(this);
            base.Close();
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyExplosionDebrisVoxel.LoadData");

            MyMwcLog.WriteLine("MyExplosionDebrisVoxel.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            PreallocatePositionOffsets();
            PreallocateObjects();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyExplosionDebrisVoxel.LoadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
        }

        static void PreallocateObjects()
        {
            if (m_objectPool == null)
                m_objectPool = new MyObjectsPool<MyExplosionDebrisVoxel>(MyExplosionsConstants.MAX_EXPLOSION_DEBRIS_OBJECTS);
            m_objectPool.DeallocateAll();

            //  This collision primitive is used by every instance of this class
            MyModel model = MyModels.GetModelOnlyData(VOXEL_DEBRIS_MODEL_ENUM);
            BoundingSphere boundingSphere = model.BoundingSphere;
            List<MyRBElementDesc> collisionPrimitives = new List<MyRBElementDesc>();

            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();
            MyRBSphereElementDesc sphereDesc = physobj.GetRBSphereElementDesc();
            MyMaterialType materialType = MyMaterialType.ROCK;
            sphereDesc.SetToDefault();
            sphereDesc.m_RBMaterial = MyMaterialsConstants.GetMaterialProperties(materialType).PhysicsMaterial;
            sphereDesc.m_Radius = boundingSphere.Radius;
            sphereDesc.m_Matrix.Translation = model.BoundingSphere.Center;

            collisionPrimitives.Add(sphereDesc);


            int counter = 0;
            foreach (LinkedListNode<MyExplosionDebrisVoxel> item in m_objectPool.GetPreallocatedItemsArray())
            {
                item.Value.Init(collisionPrimitives);
                counter++;
            }
        }

        //  Prepare offset positions for explosion debris voxels
        protected static void PreallocatePositionOffsets()
        {
            //Vector3 origin = new Vector3(-EXPLOSION_DEBRIS_OFFSET_SIZE / 2, -EXPLOSION_DEBRIS_OFFSET_SIZE / 2, -EXPLOSION_DEBRIS_OFFSET_SIZE / 2);
            const float normalizedSize = 0.7f; // This should be 1, but we need to take into account size of debris, size of debris is 30% of explosion radius at max
            Vector3 origin = new Vector3(-normalizedSize);

            const float spacing = normalizedSize * 2.0f / (EXPLOSION_DEBRIS_OFFSET_COUNT - 1); // calculate spacing between debris (from -1 to 1)

            m_positionOffsets = new List<Vector3>(EXPLOSION_DEBRIS_OFFSET_COUNT * EXPLOSION_DEBRIS_OFFSET_COUNT * EXPLOSION_DEBRIS_OFFSET_COUNT);
            for (int x = 0; x < EXPLOSION_DEBRIS_OFFSET_COUNT; x++)
            {
                for (int y = 0; y < EXPLOSION_DEBRIS_OFFSET_COUNT; y++)
                {
                    for (int z = 0; z < EXPLOSION_DEBRIS_OFFSET_COUNT; z++)
                    {
                        Vector3 pos = origin + new Vector3(x * spacing, y * spacing, z * spacing);

                        //  Phys object created near/inside the external 'explosion force' are making JLX freeze, so we don't create debris too close to the center of explosion
                        if (Vector3.Distance(pos, Vector3.Zero) >= MyConstants.SAFE_DISTANCE_FOR_ADD_WORLD_FORCE_JLX)
                        {
                            m_positionOffsets.Add(pos);
                        }
                    }
                }
            }
        }

        static Vector3 GetRandomScale(float randomSize)
        {
            float basisScale = MathHelper.Lerp(0.3f, 0.7f, randomSize);
            Vector3 ret = new Vector3(basisScale + MyMwcUtils.GetRandomFloat(-0.1f, +0.1f), basisScale + MyMwcUtils.GetRandomFloat(-0.1f, +0.1f), basisScale + MyMwcUtils.GetRandomFloat(-0.1f, +0.1f));
            ret.X = MathHelper.Clamp(ret.X, 0, 1);
            ret.Y = MathHelper.Clamp(ret.Y, 0, 1);
            ret.Z = MathHelper.Clamp(ret.Z, 0, 1);
            return ret;
        }

        public static void CreateExplosionDebris(ref BoundingSphere explosionSphere, float voxelsCountInPercent, MyMwcVoxelMaterialsEnum voxelMaterial, MyGroupMask groupMask, MyVoxelMap voxelMap)
        {
            MyCommonDebugUtils.AssertDebug((voxelsCountInPercent >= 0.0f) && (voxelsCountInPercent <= 1.0f));
            MyCommonDebugUtils.AssertDebug(explosionSphere.Radius > 0);

            Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateExplosionDebris");

            Render.MyRender.GetRenderProfiler().StartProfilingBlock("Matrices");

            //  This matrix will rotate all newly created debrises, so they won't apper as alligned with coordinate system
            Matrix randomRotationMatrix = Matrix.CreateRotationX(MyMwcUtils.GetRandomRadian()) * Matrix.CreateRotationY(MyMwcUtils.GetRandomRadian());

            float highScale = MathHelper.Clamp(explosionSphere.Radius * DebrisScaleUpper, 0, DebrisScaleClamp);
            float lowScale = highScale * (DebrisScaleLower / DebrisScaleUpper);

            int objectsToGenerate = (int)(m_positionOffsets.Count * voxelsCountInPercent * MyRenderConstants.RenderQualityProfile.ExplosionDebrisCountMultiplier);

            long dbgObjectsGenerated = 0;

            Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            Render.MyRender.GetRenderProfiler().StartProfilingBlock("m_positionOffsets");

            for (int i = 0; i < m_positionOffsets.Count; i++)
            {
                //  IMPORTANT: If you place explosion debris exactly in the center of an explosion, JLX will fail or end in endless loop
                //  Probably it's because it can't handle external force acting from inside the object.
                if (dbgObjectsGenerated >= objectsToGenerate)
                    break;

                const float cubeInsideSphereMod = 1 / 1.73f; // Resize sphere to fit inside cube

                Vector3 position = m_positionOffsets[i] * explosionSphere.Radius * cubeInsideSphereMod;
                Vector3.Transform(ref position, ref randomRotationMatrix, out position);
                position += explosionSphere.Center;

                MyExplosionDebrisVoxel newObj = Allocate();

                if (newObj != null)
                {
                    //  Check if new object won't intersect any existing triangle - because if yes, then it will decrease JLX performace a lot
                    float randomNewScale = MyMwcUtils.GetRandomFloat(lowScale, highScale);
                    BoundingSphere sphere = new BoundingSphere(position, newObj.m_modelLod0.BoundingSphere.Radius * randomNewScale);

                    Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetIntersectionWithSphere");

                  //This takes 4-5ms, is it necessary?  
                   // if (MyEntities.GetIntersectionWithSphere(ref sphere) == null)
                    //if (false)
                    {
                        Render.MyRender.GetRenderProfiler().StartProfilingBlock("newObj.Start");
                        newObj.Start(position, randomNewScale, voxelMaterial, groupMask, true);
                        MyEntities.Add(newObj);
                        Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        /*
                        Vector3 imp = position - explosionSphere.Center;
                        imp.Normalize();
                        imp *= MyExplosionsConstants.EXPLOSION_STRENGTH_IMPULSE;

                        newObj.Physics.AddForce(PhysicsManager.Physics.MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE, imp, explosionSphere.Center, Vector3.Zero);
                          */
                        if (MinerWars.AppCode.Game.Explosions.MyExplosion.DEBUG_EXPLOSIONS)
                            m_debugVoxelSpheres.Add(sphere);

                        dbgObjectsGenerated++;
                    }
                    //else
                    {
                        //  Put back to object pool
                        //newObj.Close();
                    }

                    Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    //if (newObj.Physics.Enabled)
                    //  newObj.Physics.Enabled = false;
                    // newObj.Physics.CollisionLayer = MyConstants.COLLISION_LAYER_UNCOLLIDABLE;
                }
            }

            Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static MyExplosionDebrisVoxel Allocate()
        {
            return m_objectPool.Allocate(true);
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
