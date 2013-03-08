using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Cockpit;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Render;

//  This explosion is started when missile hits something.
//  During it start, it creates explosion and smoke particles that live on their own. This object than lives only for lighting and physics.
//  After explosion's energy is burned, it dies.

namespace MinerWars.AppCode.Game.Explosions
{
    using Managers.Session;
    using System.Collections.Generic;

    //  This tells us what exploded. It doesn't say what was hit, just what exploded so then explosion can be made for that exploded thing.
    internal enum MyExplosionTypeEnum : byte
    {
        MISSILE_EXPLOSION,          //  When missile explodes. By collision or by itself (e.g. timer).
        SMALL_SHIP_EXPLOSION,       //  When small ship explodes by itself
        BOMB_EXPLOSION,             //  When mine/bomb explodes
        AMMO_EXPLOSION,
        BLASTER_EXPLOSION,
        BIOCHEM_EXPLOSION,
        EMP_EXPLOSION,
        METEOR_EXPLOSION,
        PLASMA_EXPLOSION,
        NUCLEAR_EXPLOSION,
        SMALL_EXPLOSION,
        GRAVITY_EXPLOSION,
        FLASH_EXPLOSION,
        LARGE_SHIP_EXPLOSION,
        LARGE_PREFAB_EXPLOSION,
        MEDIUM_PREFAB_EXPLOSION,
        ASTEROID_EXPLOSION,
    }

    //  How will look explosion particles
    internal enum MyExplosionParticlesTypeEnum
    {
        EXPLOSIVE_AND_DIRTY,        //  When rocks (voxels or static asteroids) aare involved in explosion
        EXPLOSIVE_ONLY              //  When metalic only... usually just missile explosion
    }

    [Flags]
    internal enum MyExplosionFlags
    {
        CREATE_DEBRIS = 1 << 0,
        AFFECT_VOXELS = 1 << 1,
        APPLY_FORCE_AND_DAMAGE = 1 << 2,
        CREATE_DECALS = 1 << 3,
        FORCE_DEBRIS = 1 << 4,
        CREATE_PARTICLE_EFFECT = 1 << 5,
    }

    internal struct MyExplosionInfo
    {
        public MyExplosionInfo(float playerDamage, float damage, float empDamage, BoundingSphere explosionSphere, MyExplosionTypeEnum type, bool playSound, MyDummyPoint customEffect = null, bool checkIntersection = true)
        {
            PlayerDamage = playerDamage;
            Damage = damage;
            EmpDamage = empDamage;
            ExplosionSphere = explosionSphere;
            ExplosionForceDirection = MyExplosionForceDirection.EXPLOSION;
            StrengthImpulse = StrengthAngularImpulse = 0.0f;
            ExcludedEntity = OwnerEntity = HitEntity = null;
            CascadeLevel = 0;
            ExplosionFlags = MyExplosionFlags.AFFECT_VOXELS | MyExplosionFlags.APPLY_FORCE_AND_DAMAGE | MyExplosionFlags.CREATE_DEBRIS | MyExplosionFlags.CREATE_DECALS | MyExplosionFlags.CREATE_PARTICLE_EFFECT;
            ExplosionType = type;
            LifespanMiliseconds = MyExplosionsConstants.EXPLOSION_LIFESPAN;
            GroupMask = MyGroupMask.Empty;
            ParticleScale = 1.0f;
            VoxelCutoutScale = 1.0f;
            Direction = null;
            VoxelExplosionCenter = explosionSphere.Center;
            PlaySound = playSound;
            CustomEffect = customEffect;
            CheckIntersections = checkIntersection;
        }

        public float PlayerDamage;
        public float Damage;
        public float EmpDamage;
        public BoundingSphere ExplosionSphere;
        public MyExplosionForceDirection ExplosionForceDirection;
        public float StrengthImpulse;
        public float StrengthAngularImpulse;
        public MyEntity ExcludedEntity;
        public MyEntity OwnerEntity;
        public MyEntity HitEntity;
        public int CascadeLevel;
        public MyExplosionFlags ExplosionFlags;
        public MyExplosionTypeEnum ExplosionType;
        public int LifespanMiliseconds;
        public MyGroupMask GroupMask;
        public float ParticleScale;
        public float VoxelCutoutScale;
        public Vector3? Direction;
        public Vector3 VoxelExplosionCenter;
        public bool PlaySound;
        public MyDummyPoint CustomEffect;
        public bool CheckIntersections;

        private void SetFlag(MyExplosionFlags flag, bool value)
        {
            if (value) ExplosionFlags |= flag;
            else ExplosionFlags &= ~flag;
        }

        private bool HasFlag(MyExplosionFlags flag)
        {
            return (ExplosionFlags & flag) == flag;
        }

        public bool AffectVoxels { get { return HasFlag(MyExplosionFlags.AFFECT_VOXELS); } set { SetFlag(MyExplosionFlags.AFFECT_VOXELS, value); } }
        public bool CreateDebris { get { return HasFlag(MyExplosionFlags.CREATE_DEBRIS); } set { SetFlag(MyExplosionFlags.CREATE_DEBRIS, value); } }
        public bool ApplyForceAndDamage { get { return HasFlag(MyExplosionFlags.APPLY_FORCE_AND_DAMAGE); } set { SetFlag(MyExplosionFlags.APPLY_FORCE_AND_DAMAGE, value); } }
        public bool CreateDecals { get { return HasFlag(MyExplosionFlags.CREATE_DECALS); } set { SetFlag(MyExplosionFlags.CREATE_DECALS, value); } }
        public bool ForceDebris { get { return HasFlag(MyExplosionFlags.FORCE_DEBRIS); } set { SetFlag(MyExplosionFlags.FORCE_DEBRIS, value); } }
        public bool CreateParticleEffect { get { return HasFlag(MyExplosionFlags.CREATE_PARTICLE_EFFECT); } set { SetFlag(MyExplosionFlags.CREATE_PARTICLE_EFFECT, value); } }
    }

    class MyExplosion
    {
        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  So don't initialize members here, do it in Start()
        BoundingSphere m_explosionSphere;
        MyLight m_light;
        int m_elapsedMiliseconds;
        int m_lifespanInMiliseconds;
        MySoundCue? m_explosionCue;
        private List<MyElement> m_destroyHelper = new List<MyElement>();

        //Use this bool to enable debug draw and better support for debugging explosions
        public static bool DEBUG_EXPLOSIONS = false;

        //  Start explosion at specified position, with radius and lifespan.
        //      type - specifies what type of object exploded
        //      explosionSphere - position and radius
        //      lifespanInMiliseconds - explosion life span
        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        public void Start(float playerDamage, float damage, float empDamage, MyExplosionTypeEnum type, BoundingSphere explosionSphere, int lifespanInMiliseconds, int cascadeLevel = 0, MyEntity hitEntity = null, float particleScale = 1.0f, MyEntity ownerEntity = null, bool forceDebris = false, bool createDecals = true, float voxelCutoutScale = 1.0f, bool playSound = true, bool checkIntersections = true)
        {
            //  Call main explosion starter
            MyExplosionInfo info = new MyExplosionInfo(playerDamage, damage, empDamage, explosionSphere, type, playSound)
            {
                LifespanMiliseconds = lifespanInMiliseconds,
                ExplosionForceDirection = MyExplosionForceDirection.EXPLOSION,
                GroupMask = MyGroupMask.Empty,
                ExplosionFlags = MyExplosionFlags.CREATE_DEBRIS | MyExplosionFlags.AFFECT_VOXELS | MyExplosionFlags.APPLY_FORCE_AND_DAMAGE | MyExplosionFlags.CREATE_PARTICLE_EFFECT,
                CascadeLevel = cascadeLevel,
                HitEntity = hitEntity,
                ParticleScale = particleScale,
                OwnerEntity = ownerEntity,
                Direction = null,
                VoxelCutoutScale = voxelCutoutScale,
                CheckIntersections = checkIntersections,
            };
            info.ForceDebris = forceDebris;
            info.CreateDecals = createDecals;
            info.VoxelExplosionCenter = explosionSphere.Center;
            Start(ref info);
        }

        public void Start(float playerDamage, float damage, float empDamage, MyExplosionTypeEnum type, BoundingSphere explosionSphere, int lifespanInMiliseconds, MyExplosionForceDirection explosionForceDirection, MyGroupMask groupMask, bool createExplosionDebris, int cascadeLevel = 0, MyEntity hitEntity = null, float particleScale = 1.0f, MyEntity ownerEntity = null, bool affectVoxels = true, bool applyForceAndDamage = true, bool createDecals = true, Vector3? direction = null, bool forceDebris = false, bool playSound = false)
        {
            MyExplosionInfo info = new MyExplosionInfo(playerDamage, damage, empDamage, explosionSphere, type, playSound)
            {
                LifespanMiliseconds = lifespanInMiliseconds,
                ExplosionForceDirection = explosionForceDirection,
                GroupMask = groupMask,
                ExplosionFlags = MyExplosionFlags.CREATE_PARTICLE_EFFECT,
                CascadeLevel = cascadeLevel,
                HitEntity = hitEntity,
                ParticleScale = particleScale,
                OwnerEntity = ownerEntity,
                Direction = direction,
                VoxelCutoutScale = 1.0f,
            };
            info.AffectVoxels = affectVoxels;
            info.ApplyForceAndDamage = applyForceAndDamage;
            info.CreateDebris = createExplosionDebris;
            info.CreateDecals = createDecals;
            info.ForceDebris = forceDebris;
            info.VoxelExplosionCenter = explosionSphere.Center;
            Start(ref info);
        }

        /// <summary>
        /// Starts the explosion.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="type"></param>
        /// <param name="explosionSphere"></param>
        /// <param name="lifespanInMiliseconds"></param>
        /// <param name="explosionForceDirection"></param>
        /// <param name="groupMask"></param>
        /// <param name="createExplosionDebris"></param>
        /// <param name="cascadeLevel"></param>
        /// <param name="hitEntity"></param>
        /// <param name="particleScale"></param>
        /// <param name="ownerEntity"></param>
        /// <param name="affectVoxels"></param>
        /// <param name="applyForceAndDamage"></param>
        /// <param name="createDecals"></param>
        /// <param name="direction">If applicable, gives the direction of the explosion, e.g. when it was caused by a missile (with its moving direction).</param>
        public void Start(ref MyExplosionInfo explosionInfo)
        {
            //MyCommonDebugUtils.AssertDebug(explosionInfo.ExplosionSphere.Radius <= MyExplosionsConstants.EXPLOSION_RADIUS_MAX);
            MyCommonDebugUtils.AssertDebug(explosionInfo.ExplosionSphere.Radius > 0);

            MyRender.GetRenderProfiler().StartProfilingBlock("MyExplosion.Start");

            m_explosionSphere = explosionInfo.ExplosionSphere;
            m_elapsedMiliseconds = 0;
            m_lifespanInMiliseconds = explosionInfo.LifespanMiliseconds;

            if (explosionInfo.PlaySound)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("Sound");
                //  Play explosion sound            
                if (m_explosionCue != null && m_explosionCue.Value.IsPlaying)
                {
                    m_explosionCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
                m_explosionCue = MyAudio.AddCue3D(GetCueEnumByExplosionType(explosionInfo.ExplosionType), m_explosionSphere.Center, Vector3.Zero, Vector3.Zero, Vector3.Zero);
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MyRender.GetRenderProfiler().StartProfilingBlock("Light");
            //  Light of explosion
            /*
            m_light = MyLights.AddLight();
            if (m_light != null)
            {
                m_light.Start(MyLight.LightTypeEnum.PointLight, m_explosionSphere.Center, MyExplosionsConstants.EXPLOSION_LIGHT_COLOR, 1, Math.Min(m_explosionSphere.Radius * 8.0f, MyLightsConstants.MAX_POINTLIGHT_RADIUS));
                m_light.Intensity = 2.0f;
            } */
            MyRender.GetRenderProfiler().EndProfilingBlock();

            // close explosion check
            bool close = IsExplosionClose(explosionInfo.ExplosionSphere);

            MyParticleEffectsIDEnum newParticlesType;

            switch (explosionInfo.ExplosionType)
            {
                case MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION:
                    //  Create metal debris objects thrown from the explosion
                    //  This must be called before ApplyExplosionForceAndDamage (because there we apply impulses to the debris)
                    //  Throw a lot of debrises, more than only if some metalic object is hit (because this is destruction of a ship)
                    //MyPhysObjectExplosionDebrises.CreateExplosionDebris(m_explosionSphere.Center, 1);
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Smallship;
                    break;
                case MyExplosionTypeEnum.MISSILE_EXPLOSION:
                    newParticlesType =
                        // ? MyParticleEffectsIDEnum.Explosion_Missile_Close
                                          MyParticleEffectsIDEnum.Explosion_Missile;
                    break;

                case MyExplosionTypeEnum.BOMB_EXPLOSION:
                case MyExplosionTypeEnum.GRAVITY_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Bomb;
                    break;

                case MyExplosionTypeEnum.AMMO_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Ammo;
                    break;

                case MyExplosionTypeEnum.BLASTER_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Blaster;
                    break;

                case MyExplosionTypeEnum.BIOCHEM_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_BioChem;
                    break;

                case MyExplosionTypeEnum.EMP_EXPLOSION:
                case MyExplosionTypeEnum.FLASH_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_EMP;
                    break;
                case MyExplosionTypeEnum.METEOR_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Meteor;
                    break;
                case MyExplosionTypeEnum.NUCLEAR_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Nuclear;
                    break;
                case MyExplosionTypeEnum.PLASMA_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Plasma;
                    break;
                case MyExplosionTypeEnum.SMALL_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_SmallPrefab;
                    break;
                case MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Huge;
                    break;
                case MyExplosionTypeEnum.LARGE_PREFAB_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Large;
                    break;
                case MyExplosionTypeEnum.MEDIUM_PREFAB_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Medium;
                    break;
                case MyExplosionTypeEnum.ASTEROID_EXPLOSION:
                    newParticlesType = MyParticleEffectsIDEnum.Explosion_Asteroid;
                    break;

                default:
                    throw new System.NotImplementedException();
                    break;
            }


            if (explosionInfo.Damage > 0)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("Voxel or collision");

                //  If explosion sphere intersects a voxel map, we need to cut out a sphere, spawn debrises, etc
                MyVoxelMap voxelMap = explosionInfo.AffectVoxels && explosionInfo.EmpDamage == 0 ? MyVoxelMaps.GetOverlappingWithSphere(ref m_explosionSphere) : null;
                if (voxelMap != null)
                {
                    //  Dirty explosion with a lot of dust

                    MyMwcVoxelMaterialsEnum? voxelMaterial = null;
                    float voxelContentRemovedInPercent = 0;

                    bool createDebris = true; // We want to create debris

                    if (explosionInfo.HitEntity != null) // but not when we hit prefab
                    {
                        createDebris &= explosionInfo.HitEntity is MyVoxelMap;
                    }


                    //cut off 
                    BoundingSphere voxelExpSphere = new BoundingSphere(explosionInfo.VoxelExplosionCenter, m_explosionSphere.Radius * explosionInfo.VoxelCutoutScale);
                    if (MyVoxelGenerator.CutOutSphereFast(voxelMap, voxelExpSphere, out voxelContentRemovedInPercent, out voxelMaterial, (explosionInfo.OwnerEntity is MySmallShip && explosionInfo.OwnerEntity == Managers.Session.MySession.PlayerShip), MyFakes.VOXELS_REMOVE_RATIO))
                    {
                        if (explosionInfo.HitEntity is MyVoxelMap)
                        {
                            HUD.MyHud.ShowIndestructableAsteroidNotification();
                        }
                        createDebris = false; // and no debris when voxel is indestructible
                    }

                    //  Only if at least something was removed from voxel map
                    //  If voxelContentRemovedInPercent is more than zero than also voxelMaterial shouldn't be null, but I rather check both of them.
                    if ((voxelContentRemovedInPercent > 0) && (voxelMaterial != null))
                    {
                        //remove decals
                        MyDecals.HideTrianglesAfterExplosion(voxelMap, ref voxelExpSphere);

                        MyRender.GetRenderProfiler().StartProfilingBlock("CreateDebris");

                        if (explosionInfo.CreateDebris && (createDebris || explosionInfo.ForceDebris) && MyRenderConstants.RenderQualityProfile.ExplosionDebrisCountMultiplier > 0)
                        {
                            //  Create debris rocks thrown from the explosion
                            //  This must be called before ApplyExplosionForceAndDamage (because there we apply impulses to the debris)
                            MyExplosionDebrisVoxel.CreateExplosionDebris(ref voxelExpSphere, voxelContentRemovedInPercent, voxelMaterial.Value, explosionInfo.GroupMask, voxelMap);
                        }

                        MyRender.GetRenderProfiler().EndProfilingBlock();

                        MyRender.GetRenderProfiler().StartProfilingBlock("CreateParticleEffect");

                        MyParticleEffect explosionEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.MaterialExplosion_Destructible);
                        explosionEffect.WorldMatrix = Matrix.CreateTranslation(voxelExpSphere.Center);
                        explosionEffect.UserRadiusMultiplier = voxelExpSphere.Radius;

                        MyRender.GetRenderProfiler().EndProfilingBlock();
                    }

                }
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            if (explosionInfo.Damage > 0)
            {
                //  Create dirt decals in player's cockpit glass
                MyRender.GetRenderProfiler().StartProfilingBlock("Cockpit Decals");
                CreateDirtDecalOnCockpitGlass(ref m_explosionSphere);
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            if (DEBUG_EXPLOSIONS)
            {
                MyRender.GetRenderProfiler().EndProfilingBlock();
                return;
            }

            if (explosionInfo.Damage > 0)
            {
                BoundingSphere influenceExplosionSphere = m_explosionSphere;
                influenceExplosionSphere.Radius *= MyExplosionsConstants.EXPLOSION_RADIUS_MULTPLIER_FOR_IMPULSE;
                for (int i = 0; i < explosionInfo.CascadeLevel; i++)
                {
                    influenceExplosionSphere.Radius *= MyExplosionsConstants.EXPLOSION_CASCADE_FALLOFF;
                }

                //  Throws surrounding objects away from centre of the explosion.
                if (explosionInfo.ApplyForceAndDamage)
                {
                    if (explosionInfo.ExplosionType == MyExplosionTypeEnum.LARGE_PREFAB_EXPLOSION ||
                        explosionInfo.ExplosionType == MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION ||
                        explosionInfo.ExplosionType == MyExplosionTypeEnum.MEDIUM_PREFAB_EXPLOSION)
                        DisableContainedDummyParticles(ref explosionInfo);

                    explosionInfo.StrengthImpulse = MyExplosionsConstants.EXPLOSION_STRENGTH_IMPULSE * m_explosionSphere.Radius / 20;
                    explosionInfo.StrengthAngularImpulse = MyExplosionsConstants.EXPLOSION_STRENGTH_ANGULAR_IMPULSE;
                    explosionInfo.HitEntity = explosionInfo.HitEntity != null ? explosionInfo.HitEntity.GetBaseEntity() : null;

                    MyRender.GetRenderProfiler().StartProfilingBlock("ApplyExplosionForceAndDamage");
                    MyEntities.ApplyExplosionForceAndDamage(ref explosionInfo);
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                //  Look for objects in explosion radius
                BoundingBox boundingBox;
                BoundingBox.CreateFromSphere(ref influenceExplosionSphere, out boundingBox);

                //if (explosionInfo.CreateDecals && explosionInfo.Direction.HasValue && explosionInfo.EmpDamage == 0)
                //{
                //    CreateDecals(explosionInfo.Direction.Value);
                //}
            }

            if (explosionInfo.CreateParticleEffect)
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("Particles");

                if (explosionInfo.CustomEffect != null)
                {
                    if (explosionInfo.CustomEffect.ParticleID == 0)
                        explosionInfo.CustomEffect.ParticleID = (int)newParticlesType;
                    //Reload effect
                    explosionInfo.CustomEffect.Enabled = false;
                    explosionInfo.CustomEffect.Enabled = true;
                }
                else
                {
                    //  Explosion particles
                    GenerateExplosionParticles(newParticlesType, m_explosionSphere, explosionInfo.ParticleScale);
                }

                MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MyRender.GetRenderProfiler().EndProfilingBlock();

            /*
                // When MyAmmoBase entity is closed to explosion it will explode
                if (entity is MyAmmoBase)
                {
                    (entity as MyAmmoBase).ExplodeCascade(cascadeLevel + 1);
                }
                */

            //  Smut decals - must be called after the explosion, after voxels are cutted out
            /*if ((intersection.PhysObject is MyVoxelMap) == false)
            {
                if (intersection.PhysObject is MyCockpitGlass)
                {
                    //  Change phys object so rest of the code will think we hit the parent
                    //  Same fix is in projectile too - because cockpit glass is only helper object, we don't use it for real rendering and stuff
                    //  And if not changed, it can make problem in "phys object decals"
                    intersection.PhysObject = intersection.PhysObject.Parent;
                }

                //  Create explosion smut decal on model we hit by this missile                
                MyDecals.Add(
                    MyDecalTexturesEnum.ExplosionSmut,
                    MyMwcUtils.GetRandomFloat(m_explosionSphere.Radius * 0.7f, m_explosionSphere.Radius * 1.3f),
                    MyMwcUtils.GetRandomRadian(),
                    GetSmutDecalRandomColor(),
                    true,
                    ref intersection);
            }
            else
            {
                //  Creating explosion smut decal on voxel is more complicated than on voxel. We will project few lines
                //  from explosion epicentrum to the surounding world (random directions) and place decal where intersection detected.
                //if (knownMissileDirection != null)
                //{
                //    MyLine linePrologned = new MyLine(knownIntersection.Value.IntersectionPointInObjectSpace,
                //        knownIntersection.Value.IntersectionPointInObjectSpace + knownMissileDirection.Value * MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MAX * 2,
                //        true);
                //    MyLineTriangleIntersectionResult intersectionForSmut = knownIntersection.Value.VoxelMap.GetIntersectionWithLine(ref linePrologned);
                //    if (intersectionForSmut.Found == true)
                //    {
                //        MyDecals.Add(
                //            MyDecalTexturesEnum.ExplosionSmut,
                //            MyMwcUtils.GetRandomFloat(m_explosionSphere.Radius * 0.5f, m_explosionSphere.Radius * 1.0f),
                //            MyMwcUtils.GetRandomRadian(),
                //            GetSmutDecalRandomColor(),
                //            false,
                //            ref intersectionForSmut);
                //    }
                //}
            }*/

            //  Generate dust particles that will stay in place of the explosion
            //doesnt look good in final 
            //GenerateStatisDustParticles(m_explosionSphere);
        }

        private void CreateDecals(Vector3 direction)
        {
            MyRender.GetRenderProfiler().StartProfilingBlock("Collisions");

            MyRender.GetRenderProfiler().StartProfilingBlock("Raycast");

            var intersectionEndPoint = m_explosionSphere.Center + 1.5f * m_explosionSphere.Radius * direction;
            var intersectionStartPoint = m_explosionSphere.Center - 1.5f * m_explosionSphere.Radius * direction;
            var line = new MyLine(intersectionStartPoint, intersectionEndPoint);

            var result = MyEntities.GetIntersectionWithLine(ref line, null, null, true, true, false, false, true, AppCode.Physics.Collisions.IntersectionFlags.ALL_TRIANGLES, true);

            MyRender.GetRenderProfiler().EndProfilingBlock();
            MyRender.GetRenderProfiler().StartProfilingBlock("Add decal");

            if (result.HasValue)
            {
                MyIntersectionResultLineTriangleEx intersection = result.Value;

                var radius = m_explosionSphere.Radius * (result.Value.Entity is MyVoxelMap ? 1.0f : MyMwcUtils.GetRandomFloat(0.4f, 0.6f));

                MyDecals.Add(
                    MyDecalTexturesEnum.ExplosionSmut,
                    radius,
                    MyMwcUtils.GetRandomRadian(),
                    GetSmutDecalRandomColor(),
                    true,
                    ref intersection,
                    0,
                    0, 
                    MyDecalsConstants.DECAL_OFFSET_BY_NORMAL_FOR_SMUT_DECALS);
            }

            MyRender.GetRenderProfiler().EndProfilingBlock();
            MyRender.GetRenderProfiler().EndProfilingBlock();

            //var elements = MyEntities.GetElementsInBox(ref boundingBox);

            //foreach (MyRBElement element in elements)
            //{
            //    var rigidBody = (MyPhysicsBody)element.GetRigidBody().m_UserData;
            //    var entity = rigidBody.Entity;

            //    if (entity is MyExplosionDebrisBase || entity is MyPrefabContainer)
            //        continue;

            //    // Making interesection of line from the explosion center to every object closed to explosion
            //    // and placing smut decals

            //    // FIX : when hitting another samll boat explosion and entity position are equal !!!
            //    //if (m_explosionSphere.Center == entity.GetPosition())
            //    //    continue;

            //    // FIX : when hitting another samll boat explosion and direction is < Epsilon !!!
            //    if ((entity.GetPosition() - m_explosionSphere.Center).LengthSquared() <
            //        2 * MyMwcMathConstants.EPSILON_SQUARED)
            //        continue;

            //    MyRender.GetRenderProfiler().StartProfilingBlock("Line intersection");
            //    MyIntersectionResultLineTriangleEx? intersection = null;
            //    if (direction.HasValue)
            //    {
            //        var intersectionEndPoint = m_explosionSphere.Center + 1.5f * m_explosionSphere.Radius * direction.Value;
            //        var intersectionStartPoint = m_explosionSphere.Center - 1.5f * m_explosionSphere.Radius * direction.Value;
            //        MyLine intersectionLine = new MyLine(intersectionStartPoint, intersectionEndPoint, true);
            //        entity.GetIntersectionWithLine(ref intersectionLine, out intersection);
            //    }
            //    else if (intersection == null && entity is MyVoxelMap)
            //    {
            //        // fall back if we dont have direction
            //        var intersectionEndPoint = entity.GetPosition();
            //        MyLine intersectionLine = new MyLine(m_explosionSphere.Center, intersectionEndPoint, true);
            //        entity.GetIntersectionWithLine(ref intersectionLine, out intersection);
            //    }
            //    MyRender.GetRenderProfiler().EndProfilingBlock();

            //    if (intersection == null)
            //        continue;

            //    MyIntersectionResultLineTriangleEx intersectionValue = intersection.Value;

            //    if (entity is MyVoxelMap)
            //    {
            //        MyRender.GetRenderProfiler().StartProfilingBlock("Decals");

            //        MyDecals.Add(
            //            MyDecalTexturesEnum.ExplosionSmut,
            //            m_explosionSphere.Radius,
            //            MyMwcUtils.GetRandomRadian(),
            //            GetSmutDecalRandomColor(),
            //            true,
            //            ref intersectionValue,
            //            0,
            //            0, MyDecalsConstants.DECAL_OFFSET_BY_NORMAL_FOR_SMUT_DECALS);

            //        MyRender.GetRenderProfiler().EndProfilingBlock();
            //    }
            //    else if (((entity is MySmallShip) == false) &&
            //             ((entity is MySmallDebris) == false)
            //             && ((entity is MyAmmoBase) == false))
            //    {
            //        //  Create explosion smut decal on model we hit by this missile                
            //        MyDecals.Add(
            //            MyDecalTexturesEnum.ExplosionSmut,
            //            MyMwcUtils.GetRandomFloat(m_explosionSphere.Radius * 0.4f, m_explosionSphere.Radius * 0.6f),
            //            MyMwcUtils.GetRandomRadian(),
            //            GetSmutDecalRandomColor(),
            //            true,
            //            ref intersectionValue,
            //            0,
            //            0, MyDecalsConstants.DECAL_OFFSET_BY_NORMAL);
            //    }
            //}
            //elements.Clear();
        }

        private static bool IsExplosionClose(BoundingSphere explosionSphere)
        {
            float distance = (explosionSphere.Center - MyCamera.Position).Length();

            return distance < MyExplosionsConstants.CLOSE_EXPLOSION_DISTANCE;
        }

        //  If explosion was with voxels, crease dirt decals in player's cockpit glass
        //  We don't throw random number of debris lines from explosion (because it will be waste). Instead we get intersection line from explosion center to player head,
        //  which should intersect the cockpit glass. Plus we move player head by random vector.
        void CreateDirtDecalOnCockpitGlass(ref BoundingSphere explosionSphere)
        {
            MySmallShip player = MySession.PlayerShip;
            float maxDistance = m_explosionSphere.Radius * MyExplosionsConstants.EXPLOSION_RADIUS_MULTPLIER_FOR_DIRT_GLASS_DECALS;
            float distance = Vector3.Distance(player.GetPosition(), explosionSphere.Center) - player.ModelLod0.BoundingSphere.Radius;

            //  Decal interpolator - based on distance to explosion, range <0..1>
            //  But then increased because we aren't able to reach max distance so we need to help it little bit
            float interpolator = 1 - MathHelper.Clamp(distance / maxDistance, 0, 1);
            interpolator = (float)Math.Pow(interpolator, 3f);

            //  Don't create dirt decal if we are too far
            if (interpolator <= 0.0f) return;

            //  Chech intersection between explosion and player's head. BUT move the line in player's head direction, because we don't want to make intersection with object which caused the explosion
            //MyLine line = new MyLine(intersection.IntersectionPointInWorldSpace, player.GetPosition(), true);
            //MyLine line = new MyLine(intersection.IntersectionPointInWorldSpace, MyCamera.m_initialSunWindPosition, true);
            //Vector3 playerHeadPositionWorld = MyUtils.GetTransform(MyFakes.PLAYER_HEAD_FOR_COCKPIT_INTERIOR_FAKE_TRANSLATION * -1, ref player.WorldMatrix);
            Vector3 playerHeadPositionWorld = player.GetPlayerHeadForCockpitInterior();
            MyLine line = new MyLine(explosionSphere.Center, playerHeadPositionWorld, true);
            line.From += line.Direction * MyExplosionsConstants.OFFSET_LINE_FOR_DIRT_DECAL;

            MyIntersectionResultLineTriangleEx? glassIntersection = MyEntities.GetIntersectionWithLine_IgnoreOtherThanSpecifiedClass(ref line, new Type[] { typeof(MySmallShip) });

            if ((glassIntersection != null) && (glassIntersection.Value.Entity is MyCockpitGlassEntity))
            {
                //  Decal alpha (never is 1.0f, because we want to see through the dirt)
                float alpha = MathHelper.Clamp(MathHelper.Lerp(0.2f, 1.0f, interpolator) - 0.1f, 0, 1);
                //const float ALPHA_INCREASE = 0.4f;
                //float alpha = 1 - (float)Math.Pow(MathHelper.Clamp(distance / maxDistance, 0, 1), 5);
                //float alpha = (float)MathHelper.SmoothStep(0, 1, 1 - MathHelper.Clamp(distance / maxDistance, 0, 1));
                //float alpha = MathHelper.Clamp(1 - MathHelper.Clamp(distance / maxDistance, 0, 1) + ALPHA_INCREASE, ALPHA_INCREASE, 1);

                //  Decal size
                float size = MathHelper.Lerp(2.5f, 4f, interpolator);

                MyIntersectionResultLineTriangleEx glassIntersection2 = glassIntersection.Value;

                MyCockpitGlassDecals.Add(MyCockpitGlassDecalTexturesEnum.DirtOnGlass, size, MyMwcUtils.GetRandomRadian(), alpha, ref glassIntersection2, true);
            }
        }

        MySoundCuesEnum GetCueEnumByExplosionType(MyExplosionTypeEnum explosionType)
        {
            MySoundCuesEnum? cueEnum = null;
            switch (explosionType)
            {
                case MyExplosionTypeEnum.MISSILE_EXPLOSION:
                    cueEnum = MySoundCuesEnum.WepMissileExplosion;
                    break;

                case MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION:
                    cueEnum = MySoundCuesEnum.SfxShipSmallExplosion;
                    break;

                case MyExplosionTypeEnum.BOMB_EXPLOSION:
                    cueEnum = MySoundCuesEnum.WepBombExplosion;
                    break;

                case MyExplosionTypeEnum.METEOR_EXPLOSION:
                    cueEnum = MySoundCuesEnum.SfxMeteorExplosion;
                    break;

                case MyExplosionTypeEnum.FLASH_EXPLOSION:
                    cueEnum = MySoundCuesEnum.WepBombFlash;
                    break;

                case MyExplosionTypeEnum.GRAVITY_EXPLOSION:
                    cueEnum = MySoundCuesEnum.WepBombGravSuck;
                    break;

                case MyExplosionTypeEnum.EMP_EXPLOSION:
                    cueEnum = MySoundCuesEnum.WepEpmExplosion;
                    break;

                case MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION:
                    cueEnum = MySoundCuesEnum.SfxShipLargeExplosion;
                    break;

                default:
                    cueEnum = MySoundCuesEnum.WepMissileExplosion;
                    break;
            }

            return cueEnum.Value;
        }

        private Vector4 GetSmutDecalRandomColor()
        {
            float randomColor = MyMwcUtils.GetRandomFloat(0.2f, 0.3f);
            return new Vector4(randomColor, randomColor, randomColor, 1);
        }

        //  We have only Update method for explosions, because drawing of explosion is mantained by particles and lights itself
        public bool Update()
        {
            if (m_explosionCue != null && m_explosionCue.Value.IsPlaying)
            {
                MyAudio.CalculateOcclusion(m_explosionCue.Value, m_explosionSphere.Center);
            }

            m_elapsedMiliseconds += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
            if (m_elapsedMiliseconds >= m_lifespanInMiliseconds)
            {
                if (DEBUG_EXPLOSIONS)
                {
                    return true;
                }
                else
                {
                    Close();
                    return false;
                }
            }

            if (m_light != null)
            {
                float normalizedTimeElapsed = 1 - (float)m_elapsedMiliseconds / (float)m_lifespanInMiliseconds;

                m_light.Color = MyExplosionsConstants.EXPLOSION_LIGHT_COLOR * normalizedTimeElapsed;
                m_light.Range = /*MyMwcUtils.GetRandomFloat(0.9f, 1.0f) */ Math.Min(m_explosionSphere.Radius * 2.0f, MyLightsConstants.MAX_POINTLIGHT_RADIUS);
            }

            return true;
        }

        public void DebugDraw()
        {
            //if (DEBUG_EXPLOSIONS)
            {
                //BoundingSphere boundingSphere = new BoundingSphere(m_explosionInfo.HitEntity is MyVoxelMap ? m_explosionInfo.VoxelExplosionCenter : m_explosionInfo.ExplosionSphere.Center, m_explosionInfo.ExplosionSphere.Radius);
                //MyDebugDraw.DrawSphereWireframe(m_explosionSphere.Center, m_explosionSphere.Radius, Color.Red.ToVector3(), 1);
                //MyDebugDraw.DrawSphereWireframe(boundingSphere.Center, boundingSphere.Radius, Color.Red.ToVector3(), 1);
            }
        }

        public void Close()
        {
            m_explosionCue = null;
            if (m_light != null)
            {
                MyLights.RemoveLight(m_light);
                m_light = null;
            }
        }

        //  Generate explosion particles. These will be smoke, explosion and some polyline particles.
        void GenerateExplosionParticles(MyParticleEffectsIDEnum newParticlesType, BoundingSphere explosionSphere, float particleScale)
        {
            Vector3 dirToCamera = MyCamera.Position - explosionSphere.Center;

            if (MyMwcUtils.IsZero(dirToCamera))
                dirToCamera = MyCamera.ForwardVector;
            else
                dirToCamera = MyMwcUtils.Normalize(dirToCamera);

            //  Move explosion particles in the direction of camera, so we won't see billboards intersecting the large ship
            BoundingSphere tempExplosionSphere = m_explosionSphere;
            tempExplosionSphere.Center = m_explosionSphere.Center + dirToCamera * 0.9f;

            MyParticleEffect explosionEffect = MyParticlesManager.CreateParticleEffect((int)newParticlesType);
            explosionEffect.WorldMatrix = Matrix.CreateTranslation(tempExplosionSphere.Center);
            explosionEffect.UserRadiusMultiplier = tempExplosionSphere.Radius;
            explosionEffect.UserScale = particleScale;
        }


        private void DisableContainedDummyParticles(ref MyExplosionInfo explosionInfo)
        {
            BoundingBox aabb = BoundingBox.CreateFromSphere(explosionInfo.ExplosionSphere);
            MyRender.GetEntitiesFromPrunningStructure(ref aabb, m_destroyHelper);

            foreach (var elem in m_destroyHelper)
            {
                var entity = ((MyRenderObject)elem).Entity;

                var dummyPoint = entity as MyDummyPoint;
                if (dummyPoint != null && !dummyPoint.CanSurvivePrefabDestruction())
                {
                    var position = dummyPoint.GetPosition();
                    dummyPoint.DisableParticleEffect();
                }
            }
        }
    }
}