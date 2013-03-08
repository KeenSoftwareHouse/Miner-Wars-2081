using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Render;

//  This class manages virtual dust field surounding the camera. Field doesn't exist, it's generated every draw call.
//  It's variance is random, but persistent.

namespace MinerWars.AppCode.Game.TransparentGeometry
{
    static class MyDebrisField
    {
        static float[][][] m_random;
        static BoundingSphere m_helperBoundingSphere;
        static List<MyRBElement> m_list = new List<MyRBElement>();
        static List<MyMwcVector3Int> m_entitiesToRemove = new List<MyMwcVector3Int>(128);

        //  Color+Alpha based on distance to camera (we use pre-multiplied alpha)

        public static float DustFieldCountInDirectionHalf { get { return MySector.DebrisProperties.CountInDirectionHalf; } }
        public static float DistanceBetween { get { return MySector.DebrisProperties.DistanceBetween; } }
        public static float FullScaleDistance { get { return MySector.DebrisProperties.FullScaleDistance; } }
        public static float MaxDistance { get { return MySector.DebrisProperties.MaxDistance; } }

        private static int DustFieldCountInDirection
        {
            get
            {
                return (int)MySector.DebrisProperties.CountInDirectionHalf * 2 + 1;
            }
        }

        private static float DistanceBetweenHalf
        {
            get
            {
                return MySector.DebrisProperties.DistanceBetween * 0.5f;
            }
        }

        private static float lastDustFieldCountInDirectionHalf;
        private static float lastDistanceBetween;

        static Dictionary<MyMwcVector3Int, MyEntity> m_usedCoords = new Dictionary<MyMwcVector3Int, MyEntity>();

        static MyDebrisField()
        {
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.DebrisField, "Debris field", Draw, Render.MyRenderStage.PrepareForDraw);
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.DebrisField, "Debris field", DebugDraw, Render.MyRenderStage.DebugDraw);
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyDebrisField.LoadData");

            MyMwcLog.WriteLine("MyDebrisField.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            Array DebrisEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_SmallDebris_TypesEnum));
            foreach (MyMwcObjectBuilder_SmallDebris_TypesEnum debrisEnum in DebrisEnumValues)
            {
                MyModelsEnum modelEnum = MySmallDebris.GetModelForType(debrisEnum);
                MyModel model = MyModels.GetModelForDraw(modelEnum); //Loads model data
                model.PreloadTextures(Managers.LoadingMode.Immediate);
            }

            MyEntities.OnCloseAll += MyEntities_OnCloseAll;

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyDebrisField.LoadContent() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void LoadContent()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyDebrisField.LoadContent");

            MyVoxelMaterial voxelMaterial = MyVoxelMaterials.Get(MyMwcVoxelMaterialsEnum.Indestructible_01);
            MyVoxelMaterialTextures voxelTexture = voxelMaterial.GetTextures();

         
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            m_usedCoords.Clear();
            m_entitiesToRemove.Clear();
            m_list.Clear();
            MyEntities.OnCloseAll -= MyEntities_OnCloseAll;
        }

        static void MyEntities_OnCloseAll()
        {
            UnloadData();
        }


        public static void Update()
        {
                                    /*
            foreach (var pair in m_usedCoords)
            {
                pair.Value.MarkForClose();
            }

            m_usedCoords.Clear(); */
            if (!MySector.DebrisProperties.Enabled || !MinerWars.AppCode.Game.Render.MyRenderConstants.RenderQualityProfile.EnableFlyingDebris)
                return;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyDebrisField.Update");

            if ((lastDustFieldCountInDirectionHalf != DustFieldCountInDirectionHalf)
            ||
                (lastDistanceBetween != DistanceBetween))
            {
                lastDistanceBetween = DistanceBetween;
                lastDustFieldCountInDirectionHalf = DustFieldCountInDirectionHalf;

                //  Fill 3D array with random values from interval <0..1>
                m_random = new float[DustFieldCountInDirection][][];
                for (int x = 0; x < m_random.Length; x++)
                {
                    m_random[x] = new float[DustFieldCountInDirection][];

                    for (int y = 0; y < m_random.Length; y++)
                    {
                        m_random[x][y] = new float[DustFieldCountInDirection];

                        for (int z = 0; z < m_random.Length; z++)
                        {
                            m_random[x][y][z] = MyMwcUtils.GetRandomFloat(0, 1);
                        }
                    }
                }
            }

            //  Update helper frustum and then its bounding box
            m_helperBoundingSphere = new BoundingSphere(MySession.PlayerShip.GetPosition(), MaxDistance);
            BoundingBox helperBoundingBox = BoundingBox.CreateFromSphere(m_helperBoundingSphere);

            MyMwcVector3Int minCoord = GetMetersToDustFieldCoord(ref helperBoundingBox.Min);
            MyMwcVector3Int maxCoord = GetMetersToDustFieldCoord(ref helperBoundingBox.Max);

            m_entitiesToRemove.Clear();
            m_entitiesToRemove.AddRange(m_usedCoords.Keys);

            BoundingSphere collisionBoundingSphere = new BoundingSphere(MySession.PlayerShip.GetPosition(), MaxDistance / 3);
            BoundingBox helperCollisionBoundingBox = BoundingBox.CreateFromSphere(m_helperBoundingSphere);


            MyEntities.GetCollisionsInBoundingBox(ref helperCollisionBoundingBox, m_list);

            bool newDebrisAllowed = true;
            foreach (MyRBElement element in m_list)
            {
                MyEntity entity = ((MinerWars.AppCode.Game.Physics.MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;
                if ((entity is MyVoxelMap)
                    ||
                    (entity is MinerWars.AppCode.Game.Prefabs.MyPrefabBase))
                    newDebrisAllowed = false;
            }

            MyMwcVector3Int tempCoord;
            for (tempCoord.X = minCoord.X; tempCoord.X <= maxCoord.X; tempCoord.X++)
            {
                for (tempCoord.Y = minCoord.Y; tempCoord.Y <= maxCoord.Y; tempCoord.Y++)
                {
                    for (tempCoord.Z = minCoord.Z; tempCoord.Z <= maxCoord.Z; tempCoord.Z++)
                    {
                        //  Position of this particle
                        Vector3 position;
                        position.X = tempCoord.X * DistanceBetween;
                        position.Y = tempCoord.Y * DistanceBetween;
                        position.Z = tempCoord.Z * DistanceBetween;


                        //  Get pseudo-random number. It's randomness is based on 3D position, so values don't change between draw calls.
                        float pseudoRandomVariationMod = m_random[Math.Abs(tempCoord.X) % m_random.Length][Math.Abs(tempCoord.Y) % m_random.Length][Math.Abs(tempCoord.Z) % m_random.Length];

                        //  Alter position by randomness
                        position.X += MathHelper.Lerp(-DistanceBetweenHalf, +DistanceBetweenHalf, pseudoRandomVariationMod);
                        position.Y += MathHelper.Lerp(-DistanceBetweenHalf, +DistanceBetweenHalf, pseudoRandomVariationMod);
                        position.Z += MathHelper.Lerp(-DistanceBetweenHalf, +DistanceBetweenHalf, pseudoRandomVariationMod);

                        //  Distance to particle
                        float distance;
                        Vector3 center = MySession.PlayerShip.GetPosition();

                        Vector3.Distance(ref center, ref position, out distance);

                        if (distance > MaxDistance)
                            continue;

                        //  Pseudo-random color and alpha
                        float pseudoRandomColor = MathHelper.Lerp(0.1f, 0.2f, pseudoRandomVariationMod); //MathHelper.Lerp(0.2f, 0.3f, pseudoRandomVariationMod);
                        //float pseudoRandomAlpha = 0.5f; //0.4f;  // 0.2f;// MathHelper.Lerp(0.2f, 0.3f, pseudoRandomVariationMod);


                        //Remove only entities outside distance, not frustum (looks better)
                        m_entitiesToRemove.Remove(tempCoord);

                        if (MyCamera.GetBoundingFrustum().Contains(position) == ContainmentType.Disjoint)
                            continue;

                        float alpha = 0;

                        if (distance < FullScaleDistance)
                        {
                            alpha = 1;
                        }
                        else if ((distance >= FullScaleDistance) && (distance < MaxDistance))
                        {
                            alpha = 1 - MathHelper.Clamp((distance - FullScaleDistance) / (MaxDistance - FullScaleDistance), 0, 1);
                        }
                        else
                        {
                            alpha = 0;
                        }

                        MyEntity entity;
                        m_usedCoords.TryGetValue(tempCoord, out entity);

                        float scale = MathHelper.Lerp(0.2f, 1.0f, alpha);

                        if (entity == null)
                        {
                            if (!newDebrisAllowed)
                                continue;

                            if (alpha > 0.2f)
                                continue;  //it would be popping

                            MinerWars.CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_Base debrisBuilder = null;

                            if (MyMwcUtils.GetRandomInt(2) % 2 == 0)
                            {
                                entity = MyExplosionDebrisVoxel.Allocate();
                                if (entity == null)
                                    continue;

                                int voxelMatEnumIndex = (int)MyMwcUtils.GetRandomShort((short)0, (short)(MySector.DebrisProperties.DebrisVoxelMaterials.Length));
                                MyMwcVoxelMaterialsEnum voxelMatEnum = (MyMwcVoxelMaterialsEnum)MySector.DebrisProperties.DebrisVoxelMaterials.GetValue(voxelMatEnumIndex);

                                ((MyExplosionDebrisVoxel)entity).Start(position, 1, voxelMatEnum, MyGroupMask.Empty, false);
                                MyEntities.Add(entity);
                            }
                            else
                            {
                                int debrisEnumIndex = (int)MyMwcUtils.GetRandomShort((short)0, (short)(MySector.DebrisProperties.DebrisEnumValues.Length));
                                MyMwcObjectBuilder_SmallDebris_TypesEnum debrisEnum = (MyMwcObjectBuilder_SmallDebris_TypesEnum)MySector.DebrisProperties.DebrisEnumValues.GetValue(debrisEnumIndex);

                                debrisBuilder = new MyMwcObjectBuilder_SmallDebris(debrisEnum, true, 0);
                                //MyMwcObjectBuilder_SmallDebris debrisBuilder = new MyMwcObjectBuilder_SmallDebris(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris32_pilot, true, 0);
                                entity = MyEntities.CreateFromObjectBuilderAndAdd(null, debrisBuilder, Matrix.CreateWorld(position, MyMwcUtils.GetRandomVector3Normalized(), MyMwcUtils.GetRandomVector3Normalized()));
                            }

                            entity.Save = false;
                            entity.CastShadows = false;

                            m_usedCoords.Add(tempCoord, entity);
                        }


                        if (entity.Physics != null && entity.Physics.Enabled == true)
                            entity.Physics.Enabled = false;


                        /*
                        if (!(entity is MyExplosionDebrisVoxel) && (distance < FullScaleDistance / 2.0f))
                        {
                            if (entity.Physics == null)
                            {
                                entity.InitBoxPhysics(MyMaterialType.METAL, entity.ModelCollision, 500, 0, MyConstants.COLLISION_LAYER_MODEL_DEBRIS, RigidBodyFlag.RBF_DEFAULT);
                            }
                            if (entity.Physics.Enabled == false)
                            {
                                entity.Physics.Clear();
                                entity.Physics.Enabled = true;
                            }
                        }*/
                            /*
                        else
                        {
                            if (entity.Physics != null && entity.Physics.Enabled)
                            {
                                entity.Physics.Enabled = false;
                            }
                        }     */


                        if (entity is MyExplosionDebrisVoxel)
                        {
                            scale *= 0.08f;
                        }

                        entity.Scale = scale;

                        /*
                        if (entity.Physics == null && distance < FullScaleDistance / 3)
                        {
                            entity.InitBoxPhysics(MyMaterialType.METAL, entity.ModelLod0, 100, MyPhysicsConfig.DefaultAngularDamping, MyConstants.COLLISION_LAYER_ALL, RigidBodyFlag.RBF_DEFAULT);
                            entity.Physics.Enabled = true;
                        } */
                    }
                }
            }


            foreach (MyMwcVector3Int positionToRemove in m_entitiesToRemove)
            {
                MyEntity entity = m_usedCoords[positionToRemove];
                /*
                if (entity.Physics != null && entity.Physics.LinearVelocity.LengthSquared() > 0.1f)
                {
                    //  Particles.MyParticleEffect effect = Particles.MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_Missile);
                    //  effect.WorldMatrix = entity.WorldMatrix;
                } */

                entity.MarkForClose();

                m_usedCoords.Remove(positionToRemove);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        static MyMwcVector3Int GetMetersToDustFieldCoord(ref Vector3 position)
        {
            return new MyMwcVector3Int(
                (int)(position.X / DistanceBetween),
                (int)(position.Y / DistanceBetween),
                (int)(position.Z / DistanceBetween));
        }

        //  This method doesn't really draw. It just creates billboards that are later drawn in MyParticles.Draw()
        public static void Draw()
        {

        }

        public static void DebugDraw()
        {
            return;
            //  Update helper frustum and then its bounding box
            m_helperBoundingSphere = new BoundingSphere(MySession.PlayerShip.GetPosition(), MaxDistance);
            BoundingBox helperBoundingBox = BoundingBox.CreateFromSphere(m_helperBoundingSphere);

            MyMwcVector3Int minCoord = GetMetersToDustFieldCoord(ref helperBoundingBox.Min);
            MyMwcVector3Int maxCoord = GetMetersToDustFieldCoord(ref helperBoundingBox.Max);

            List<MyMwcVector3Int> entitiesToRemove = new List<MyMwcVector3Int>(m_usedCoords.Keys);

            MyMwcVector3Int tempCoord;
            for (tempCoord.X = minCoord.X; tempCoord.X <= maxCoord.X; tempCoord.X++)
            {
                for (tempCoord.Y = minCoord.Y; tempCoord.Y <= maxCoord.Y; tempCoord.Y++)
                {
                    for (tempCoord.Z = minCoord.Z; tempCoord.Z <= maxCoord.Z; tempCoord.Z++)
                    {
                        //  Position of this particle
                        Vector3 position;
                        position.X = tempCoord.X * DistanceBetween;
                        position.Y = tempCoord.Y * DistanceBetween;
                        position.Z = tempCoord.Z * DistanceBetween;

                        float distance;
                        Vector3 center = MySession.PlayerShip.GetPosition();
                        //Vector3 center = new Vector3(m_random[0][0][0], m_random[0][0][1], m_random[0][0][2]);

                        Vector3.Distance(ref center, ref position, out distance);

                        if (distance > MaxDistance)
                            continue;

                        /*
                        BoundingBox bbox = new BoundingBox(position - new Vector3(DistanceBetweenHalf),position + new Vector3(DistanceBetweenHalf));
                        Vector4 color = new Vector4(0,1,0,1);
                        MyDebugDraw.DrawAABBLowRes(ref bbox, ref color, 1.0f);*/

                        MyDebugDraw.DrawSphereWireframe(position, 1, Vector3.One, 1);
                    }
                }
            }
        }
    }
}
