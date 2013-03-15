//#define DEBUG_MISSILE

using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System;
    using System.Collections.Generic;
    using Audio;
    using CommonLIB.AppCode.Utils;
    using Explosions;
    using Lights;
    using MinerWarsMath;
    
    using Models;
    using TransparentGeometry;
    using UniversalLauncher;
    using Utils;
    using MinerWars.AppCode.Game.Entities.Prefabs;
    using MinerWars.AppCode.Game.TransparentGeometry.Particles;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using SysUtils.Utils;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.Entities.Ships.AI;
    using MinerWars.AppCode.Game.Render;
    using System.Diagnostics;

    class MyMissile : MyAmmoBase
    {
        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  So don't initialize members here, do it in Start()
        MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum m_missileType;

        protected Vector3 m_predicatedPosition; //predicated position of the target
        protected Vector3 m_targetVelocity; //velocity of the target


        MyLight m_light;
        MySoundCue? m_thrusterCue; //sound
        protected MyEntity m_targetEntity; //locked target entity

        protected Vector3 m_desiredVelocity; //velocity missile should achieve
        protected float m_actualSpeed; //current speed of missile

        protected float m_maxTrajectory; //max trajectory for missile

#if DEBUG_MISSILE
        List<Vector3> m_trailDebug = new List<Vector3>();
#endif

        protected float m_initTime = MyMissileConstants.MISSILE_INIT_TIME;
        protected Vector3 m_initDir = MyMissileConstants.MISSILE_INIT_DIR;
        protected float m_blendVelocities = MyMissileConstants.MISSILE_BLEND_VELOCITIES_IN_MILISECONDS;
        protected float m_missileTimeout = MyMissileConstants.MISSILE_TIMEOUT;
        protected float m_turnSpeed = 0;

        MyParticleEffect m_smokeEffect;
        MyExplosionTypeEnum m_explosionType;
        private MyEntity m_collidedEntity;
        Vector3? m_collisionPoint;
        int m_missileTargetUpdate;
        static BoundingFrustum m_visualFrustum = new BoundingFrustum(Matrix.Identity);
        static List<MyEntity> m_targetEntities = new List<MyEntity>(16);

        public static Predicate<MyEntity> CanBeTargetedPredicate = new Predicate<MyEntity>(CanBeTargeted);
        
        public MyMissile()
        {
            m_collidedEntity_OnClose = collidedEntity_OnClose;
        }

        public static bool CanBeTargeted(MyEntity entity)
        {
            return (entity is MySmallShip || entity is MyPrefabLargeWeapon);
        }

        public bool CanTarget(MyEntity entity)
        {
            return CanBeTargeted(entity) && (MyFactions.GetFactionsRelation(Faction, entity.Faction) == MyFactionRelationEnum.Enemy);
        }

        //  Parameter-less init - because guided missiles are stored in object pool
        public virtual void Init()
        {
            Init(MyModelsEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic);
            // Missles will hit voxels if you fire near them and line bellow is commented, however position of missle start is corrected in MyMissleLauncherGun.CorrectPosition
            //this.Physics.RigidBody.RaiseFlag(RigidBodyFlag.RBF_COLDET_THROUGH_VOXEL_TRIANGLES);
            this.Physics.CollisionLayer = MyConstants.COLLISION_LAYER_MISSILE;
            m_canByAffectedByExplosionForce = false;
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public virtual void Start(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum type, Vector3 position, Vector3 initialVelocity, Vector3 direction, Vector3 relativePos, MyEntity minerShip, MyEntity target, float customMaxDistance, bool isDummy, bool isLightWeight)
        {
            m_ammoProperties = MyAmmoConstants.GetAmmoProperties(type);
            m_missileType = type;
            m_isExploded = false;
            m_collidedEntity = null;
            m_collisionPoint = null;
            m_maxTrajectory = customMaxDistance > 0 ? customMaxDistance : m_ammoProperties.MaxTrajectory;
            IsDummy = isDummy;
            Faction = minerShip.Faction;

            Vector3? correctedDirection = null;
            if (MyGameplayConstants.GameplayDifficultyProfile.EnableAimCorrection)
            {
                if (minerShip == MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip)
                {
                    correctedDirection = MyEntities.GetDirectionFromStartPointToHitPointOfNearestObject(minerShip, position, m_ammoProperties.MaxTrajectory);
                }
            }

            if (correctedDirection != null)
                direction = correctedDirection.Value;

            base.Start(position, initialVelocity, direction, 0, minerShip);

            if (correctedDirection != null) //override the base class behaviour, update the missile direction
            {
                Matrix ammoWorld = minerShip.WorldMatrix;
                ammoWorld.Translation = position;
                ammoWorld.Forward = correctedDirection.Value;

                SetWorldMatrix(ammoWorld);
            }

            switch (m_missileType)
            {
                //just going forward (deprecated)
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic:
                    m_initTime = MyMissileConstants.MISSILE_INIT_TIME;
                    m_initDir = MyMissileConstants.MISSILE_INIT_DIR;
                    m_blendVelocities = MyMissileConstants.MISSILE_BLEND_VELOCITIES_IN_MILISECONDS;
                    m_missileTimeout = MyMissileConstants.MISSILE_TIMEOUT;
                    m_explosionType = MyExplosionTypeEnum.MISSILE_EXPLOSION;
                    break;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem:
                    m_initTime = MyMissileConstants.MISSILE_INIT_TIME;
                    m_initDir = MyMissileConstants.MISSILE_INIT_DIR;
                    m_blendVelocities = MyMissileConstants.MISSILE_BLEND_VELOCITIES_IN_MILISECONDS;
                    m_missileTimeout = MyMissileConstants.MISSILE_TIMEOUT;
                    m_explosionType = MyExplosionTypeEnum.BIOCHEM_EXPLOSION;
                    break;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP:
                    m_initTime = MyMissileConstants.MISSILE_INIT_TIME;
                    m_initDir = MyMissileConstants.MISSILE_INIT_DIR;
                    m_blendVelocities = MyMissileConstants.MISSILE_BLEND_VELOCITIES_IN_MILISECONDS;
                    m_missileTimeout = MyMissileConstants.MISSILE_TIMEOUT;
                    m_explosionType = MyExplosionTypeEnum.EMP_EXPLOSION;
                    break;

                //Missile is guided to the nearest enemy in the radius
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection:

                //Missile is guided to the closest enemy in the visible spot
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection:

                //Missile is guided to actual selected target by smallship radar
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection:
                    m_initDir.X = 5.0f * MathHelper.Clamp(relativePos.X, -1, 1);
                    m_blendVelocities = MyGuidedMissileConstants.MISSILE_BLEND_VELOCITIES_IN_MILISECONDS;
                    m_missileTimeout = MyGuidedMissileConstants.MISSILE_TIMEOUT;
                    m_turnSpeed = MyGuidedMissileConstants.MISSILE_TURN_SPEED;
                    m_explosionType = MyExplosionTypeEnum.MISSILE_EXPLOSION;
                    GuidedInMultiplayer = true;
                    break;
                default:
                    throw new NotImplementedException();
            }


            UpdateTarget(target);


            if (!isLightWeight)
            {
                //  Play missile thrust cue (looping)
                m_thrusterCue = MyAudio.AddCue3D(m_ammoProperties.ShotSound, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, m_initialVelocity);

                m_light = MyLights.AddLight();
                if (m_light != null)
                {
                    m_light.Start(MyLight.LightTypeEnum.PointLight, GetPosition(), MyMissileHelperUtil.GetMissileLightColor(), 1, MyMissileConstants.MISSILE_LIGHT_RANGE);
                }
            }

#if DEBUG_MISSILE
            m_trailDebug.Clear();
#endif

            if (m_missileType == MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic)
            {
                /*
                MyParticleEffect startEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_MissileStart);
                startEffect.WorldMatrix = WorldMatrix;
                 */
                m_smokeEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_Missile);
                m_smokeEffect.WorldMatrix = WorldMatrix;
                m_smokeEffect.AutoDelete = false;
            }
        }

        void UpdateTarget(MyEntity newTarget)
        {
            if (MySession.PlayerShip.EntityId != null && m_ownerEntityID.NumericValue == MySession.PlayerShip.EntityId.Value.NumericValue)
            {
                if (m_targetEntity != null)
                    MySession.PlayerShip.SideTargets.Remove(m_targetEntity);

                if (newTarget != null)
                    MySession.PlayerShip.SideTargets.Add(newTarget);
            }

            m_targetEntity = newTarget;
        }


        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            try
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("MyMissile.UpdateBeforeSimulation");

                //Large ship weapons wont make bots curious
                if ((!(OwnerEntity is MyLargeShipMissileLauncherBarrel)) && MyMwcUtils.HasValidLength(this.WorldMatrix.Translation - m_previousPosition))
                {
                    MyLine line = new MyLine(this.WorldMatrix.Translation, m_previousPosition);
                    MyDangerZones.Instance.Notify(line, OwnerEntity);
                }

                if (m_isExploded)
                {
                    //  Create explosion
                    MyExplosion newExplosion = MyExplosions.AddExplosion();
                    if (newExplosion != null)
                    {
                        float radius = m_ammoProperties.ExplosionRadius;

                        // Explicitly on Marek's request (ticket 4740)
                        bool amplifyRadius = m_collidedEntity != null ? MyFactions.GetFactionsRelation(m_collidedEntity.Faction, Faction) != MyFactionRelationEnum.Friend : false;
                        if (amplifyRadius)
                        {
                            radius *= 2;
                        }

                        BoundingSphere explosionSphere = new BoundingSphere(m_collisionPoint.HasValue ? m_collisionPoint.Value : GetPosition(), radius);

                        //  Call main explosion starter
                        MyExplosionInfo info = new MyExplosionInfo()
                        {
                            PlayerDamage = m_ammoProperties.HealthDamage,
                            Damage = m_ammoProperties.ShipDamage,
                            EmpDamage = m_ammoProperties.EMPDamage,
                            ExplosionType = m_explosionType,
                            ExplosionSphere = explosionSphere,
                            LifespanMiliseconds = MyExplosionsConstants.EXPLOSION_LIFESPAN,
                            ExplosionForceDirection = MyExplosionForceDirection.EXPLOSION,
                            GroupMask = Physics.GroupMask,
                            CascadeLevel = CascadedExplosionLevel,
                            HitEntity = m_collidedEntity,
                            ParticleScale = 1.5f,
                            OwnerEntity = this.OwnerEntity,
                            Direction = WorldMatrix.Forward,
                            VoxelExplosionCenter = explosionSphere.Center + m_ammoProperties.ExplosionRadius * WorldMatrix.Forward * 0.5f,
                            ExplosionFlags = MyExplosionFlags.AFFECT_VOXELS | MyExplosionFlags.APPLY_FORCE_AND_DAMAGE | MyExplosionFlags.CREATE_DEBRIS | MyExplosionFlags.CREATE_DECALS | MyExplosionFlags.CREATE_PARTICLE_EFFECT,
                            VoxelCutoutScale = amplifyRadius ? 0.5f : 1.0f,
                            PlaySound = true,
                        };

                        newExplosion.Start(ref info);
                    }

                    if (m_collidedEntity != null && !m_collidedEntity.IsExploded())
                    {
                        m_collidedEntity.Physics.AddForce(
                            MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                            WorldMatrix.Forward * MyMissileConstants.HIT_STRENGTH_IMPULSE,
                            GetPosition() + MyMwcUtils.GetRandomVector3Normalized() * 2,
                            MyMissileConstants.HIT_STRENGTH_IMPULSE * MyMwcUtils.GetRandomVector3Normalized());
                        m_collidedEntity.OnClose -= m_collidedEntity_OnClose;
                    }

                    MarkForClose();

                    return;
                }

                bool firstTargetting = m_elapsedMiliseconds == 0;

                base.UpdateBeforeSimulation();

                m_missileTargetUpdate += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;

                if (m_missileTargetUpdate >= MyGuidedMissileConstants.MISSILE_TARGET_UPDATE_INTERVAL_IN_MS || firstTargetting)
                {
                    m_missileTargetUpdate = 0;

                    switch (m_missileType)
                    {
                        case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection:
                            {
                                MySmallShip targetShip = m_targetEntity as MySmallShip;
                                if (targetShip != null && targetShip.IsRadarJammed())
                                {
                                    m_targetEntity = null;
                                }
                            }
                            break;
                        case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection:
                            {
                                m_targetEntities.Clear();

                                Matrix proj = Matrix.CreateOrthographic(MyGuidedMissileConstants.ENGINE_GUIDED_MISSILE_RADIUS, MyGuidedMissileConstants.ENGINE_GUIDED_MISSILE_RADIUS, 0, 1000);
                                Matrix view = Matrix.CreateLookAt(GetPosition(), GetPosition() + WorldMatrix.Forward, WorldMatrix.Up);
                                m_visualFrustum.Matrix = view * proj;

                                MyEntities.GetAllIntersectionWithBoundingFrustum(ref m_visualFrustum, m_targetEntities);

                                if (m_targetEntities.Contains(m_targetEntity))
                                    break;

                                MyEntity target = null;
                                float closestToMissileDirection = float.MaxValue;

                                foreach (MyEntity entity in m_targetEntities)
                                {
                                    if (CanTarget(entity))
                                    {
                                        MySmallShip targetShip = entity as MySmallShip;
                                        if (targetShip != null)
                                        {
                                            if ((targetShip.IsEngineTurnedOff()))
                                                continue;
                                        }

                                        Vector3 targetPos = entity.GetPosition();
                                        Vector3 missilePos = this.GetPosition();
                                        Vector3 missilePosEnd = this.GetPosition() + this.WorldMatrix.Forward * 10000;
                                        Vector3 closestPos = MyUtils.GetClosestPointOnLine(ref missilePos, ref missilePosEnd, ref targetPos);

                                        float distance = Vector3.Distance(closestPos, targetPos);
                                        if (distance < closestToMissileDirection)
                                        {
                                            closestToMissileDirection = distance;
                                            target = entity;
                                        }
                                    }
                                }

                                UpdateTarget(target);
                            }
                            break;

                        case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection:
                            {
                                Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(MyGuidedMissileConstants.VISUAL_GUIDED_MISSILE_FOV), 1, 10, MyGuidedMissileConstants.VISUAL_GUIDED_MISSILE_RANGE);
                                m_visualFrustum.Matrix = Matrix.Invert(WorldMatrix) * projectionMatrix;

                                m_targetEntities.Clear();
                                MyEntities.GetAllIntersectionWithBoundingFrustum(ref m_visualFrustum, m_targetEntities);
                                int testsLimit = 8;

                                if (m_targetEntities.Contains(m_targetEntity))
                                    break;

                                MyEntity target = null; //looks better if missile gets "lost"

                                float closestToMissileDirection = float.MaxValue;

                                foreach (MyEntity entity in m_targetEntities)
                                {
                                    if (!CanTarget(entity))
                                        continue;

                                    if (testsLimit-- == 0)
                                        break;

                                    if (MyEnemyTargeting.CanSee(this, entity) == null)
                                    {
                                        Vector3 targetPos = entity.GetPosition();
                                        Vector3 missilePos = this.GetPosition();
                                        Vector3 missilePosEnd = this.GetPosition() + this.WorldMatrix.Forward * 10000;
                                        Vector3 closestPos = MyUtils.GetClosestPointOnLine(ref missilePos, ref missilePosEnd, ref targetPos);

                                        float distance = Vector3.Distance(closestPos, targetPos);
                                        if (distance < closestToMissileDirection)
                                        {
                                            closestToMissileDirection = distance;
                                            target = entity;
                                        }
                                    }
                                }

                                UpdateTarget(target);
                            }
                            break;
                    }
                }

                if ((m_initTime - m_elapsedMiliseconds) > 0)
                {   //simulating missile launch and engine ignition
                    MyEntity owner = OwnerEntity;
                    if (owner != null)
                    {
                        Vector3 transformedInitDir = Vector3.TransformNormal(m_initDir, owner.WorldMatrix); //
                        Vector3 initialVelocity = Vector3.Zero;
                        if (owner.Physics != null)
                        {
                            initialVelocity = owner.Physics.LinearVelocity;
                        }
                        Physics.LinearVelocity = transformedInitDir * m_ammoProperties.InitialSpeed + initialVelocity;
                    }
                }
                else
                {
                    //  This will help blend "initial velocity" and "thrust velocity" so at the beginning missile is powered by initiatal velocity only, but later 
                    float velocityBlend = MathHelper.Clamp((float)(m_elapsedMiliseconds - m_initTime) / m_blendVelocities, 0, 1);

                    if (velocityBlend == 1.0f)
                    {
                        m_actualSpeed = m_ammoProperties.DesiredSpeed;
                    }
                    else
                    {
                        float initialSpeed = 0.0f;
                        MyEntity owner = OwnerEntity;
                        if (owner != null)
                        {
                            if (owner.Physics != null)
                            {
                                initialSpeed = owner.Physics.LinearVelocity.Length();
                            }
                        }

                        m_actualSpeed = velocityBlend * m_ammoProperties.DesiredSpeed + ((1.0f - velocityBlend) * (m_ammoProperties.InitialSpeed + initialSpeed));

                        if (m_missileType != MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic && m_smokeEffect == null)
                        {
                            // if (MyCamera.GetDistanceWithFOV(GetPosition()) < 150)
                            {
                                /*
                                MyParticleEffect startEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_MissileStart);
                                startEffect.WorldMatrix = WorldMatrix;
                                 */
                                m_smokeEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_Missile);
                                m_smokeEffect.WorldMatrix = WorldMatrix;
                                m_smokeEffect.AutoDelete = false;
                            }
                        }
                    }

                    m_desiredVelocity = GetDesiredVelocity(m_targetEntity);

                    Physics.LinearVelocity = m_desiredVelocity * m_actualSpeed * 1.0f;
                }

                Physics.AngularVelocity = Vector3.Zero;

                if ((m_elapsedMiliseconds > m_missileTimeout) || (Vector3.Distance(GetPosition(), m_origin) >= m_maxTrajectory))
                {
                    Explode();
                    return;
                }


                if (m_smokeEffect != null)
                {
                    Matrix smokeMatrix = Matrix.CreateWorld(WorldMatrix.Translation - 0.5f * WorldMatrix.Forward, WorldMatrix.Forward, WorldMatrix.Up);
                    m_smokeEffect.WorldMatrix = smokeMatrix;
                }

                if (m_targetEntity != null)
                {
                    if (m_targetEntity.Physics != null)
                        m_targetVelocity = m_targetEntity.Physics.LinearVelocity;
                    else
                        m_targetVelocity = Vector3.Zero;
                }
            }
            finally
            {
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        /// <summary>
        /// Called when [world position changed].
        /// </summary>
        /// <param name="source">The source object that caused this event.</param>
        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);

            if (Physics != null && m_thrusterCue != null)
            {
                //  Update thruster cue/sound
                MyAudio.UpdateCuePosition(m_thrusterCue, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up,
                                          Physics.LinearVelocity);
            }

            //  Update light position
            if (m_light != null)
            {
                m_light.SetPosition(GetPosition());
                m_light.Color = MyMissileHelperUtil.GetMissileLightColor();
                m_light.Range = MyMissileConstants.MISSILE_LIGHT_RANGE;
            }
        }

        //  Kills this missile. Must be called at her end (after explosion or timeout)
        //  This method must be called when this object dies or is removed
        //  E.g. it removes lights, sounds, etc
        public override void Close()
        {
            UpdateTarget(null);

            base.Close();

            this.Physics.Clear();

            MyMissiles.Remove(this);

            if (m_collidedEntity != null)
            {
                m_collidedEntity.OnClose -= m_collidedEntity_OnClose;
                m_collidedEntity = null;
            }

            if (m_smokeEffect != null)
            {
                m_smokeEffect.Stop();
                m_smokeEffect = null;
            }

            //  Free the light
            if (m_light != null)
            {
                MyLights.RemoveLight(m_light);
                m_light = null;
            }

            //  Stop thruster cue
            if ((m_thrusterCue != null) && (m_thrusterCue.Value.IsPlaying == true))
            {
                m_thrusterCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
            m_thrusterCue = null;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;

            //  Draw flare at the end or back of the missile
            //  Color and range is same as for missile's dynamic light
            if (m_light != null)
            {
                if ((m_initTime - m_elapsedMiliseconds) <= 0)
                {
                    MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, m_light.Color,
                        WorldMatrix.Translation - 1.1f * WorldMatrix.Forward * ModelLod0.BoundingSphere.Radius, m_light.Range * 0.02f, 0);
                }
                else
                {
                    float delta = m_elapsedMiliseconds / m_initTime;

                    MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, m_light.Color,
                        WorldMatrix.Translation - WorldMatrix.Forward * ModelLod0.BoundingSphere.Radius, m_light.Range * 0.005f * delta * delta, 0);
                }
            }
            return true;
        }

        public override bool DebugDraw()
        {
            /*
        m_targetEntities.Clear();

        Matrix proj = Matrix.CreateOrthographic(300, 300, 0, 1000);
        Matrix view = Matrix.CreateLookAt(GetPosition(), GetPosition() + WorldMatrix.Forward, WorldMatrix.Up);
        m_visualFrustum.Matrix = view * proj;

        //MyEntities.GetIntersectionWithSphere(ref sphere, this, OwnerEntity, false, false, ref m_targetEntities);
        MyEntities.GetAllIntersectionWithBoundingFrustum(ref m_visualFrustum, m_targetEntities);
           
        MyEntity target = null;
        float closestToMissileDirection = float.MaxValue;

        foreach (MyEntity entity in m_targetEntities)
        {
            if (CanTarget(entity))
            {
                MySmallShip targetShip = entity as MySmallShip;
                if (targetShip != null)
                {
                    if ((targetShip.IsEngineTurnedOff()))
                        continue;
                }

                Vector3 targetPos = entity.GetPosition();
                Vector3 missilePos = this.GetPosition();
                Vector3 missilePosEnd = this.GetPosition() + this.WorldMatrix.Forward * 10000;
                Vector3 closestPos = MyUtils.GetClosestPointOnLine(ref missilePos, ref missilePosEnd, ref targetPos);

                float distance = Vector3.Distance(closestPos, targetPos);

                MyDebugDraw.DrawText(targetPos, new System.Text.StringBuilder(distance.ToString()), Color.White, 1);

                if (distance < closestToMissileDirection)
                {
                    closestToMissileDirection = distance;
                    target = entity;
                }
            }
        }

          /*
        MyEntity target = null; //looks better if missile gets "lost"

        float closestToMissileDirection = float.MaxValue;

        foreach (MyEntity entity in m_targetEntities)
        {
            if (!CanTarget(entity))
                continue;

            if (MyEnemyTargeting.CanSee(this, entity) == null)
            {
                Vector3 targetPos = entity.GetPosition();
                Vector3 missilePos = this.GetPosition();
                Vector3 missilePosEnd = this.GetPosition() + this.WorldMatrix.Forward * 10000;
                Vector3 closestPos = MyUtils.GetClosestPointOnLine(ref missilePos, ref missilePosEnd, ref targetPos);

                float distance = Vector3.Distance(closestPos, targetPos);


                MyDebugDraw.DrawText(targetPos, new System.Text.StringBuilder(distance.ToString()), Color.White, 1);

                if (distance < closestToMissileDirection)
                {
                    closestToMissileDirection = distance;
                    target = entity;
                }
            }
        }
            */


            return base.DebugDraw();
            //MyDebugDraw.DrawBoundingFrustum(m_visualFrustum, Color.White);
            return true;
            //  bool retVal = base.DebugDraw();
            //if (!retVal)
            //  return false;

            //Matrix tempInvertedWorldMatrix = Matrix.Invert(Body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * Physics.Matrix);

            //  Transform world's Y axis to body space
            //Vector3 forward = MyUtils.GetTransformNormal(WorldMatrix.Forward, ref tempInvertedWorldMatrix);

#if DEBUG_MISSILE
            if (m_targetShip != null)
            {
                //Vector3 torque = MyUtils.GetTransformNormal(MyMwcUtils.Normalize(m_targetEntity.WorldMatrix.Translation - WorldMatrix.Translation), ref tempInvertedWorldMatrix);
                MyDebugDraw.DrawLine3D(WorldMatrix.Translation, WorldMatrix.Translation + (m_targetShip.WorldMatrix.Translation - WorldMatrix.Translation), Color.Pink, Color.Pink);

                MyDebugDraw.DrawLine3D(WorldMatrix.Translation, WorldMatrix.Translation + (m_predicatedPosition - WorldMatrix.Translation), Color.Yellow, Color.Yellow);

                
            }
#endif
            Vector3 pos = WorldMatrix.Translation;
            MyDebugDraw.DrawSphereWireframe(pos, WorldVolume.Radius, Vector3.One, 1);
            //Matrix matrix = Physics.Matrix;

            /*
MyDebugDraw.DrawLine3D(pos, pos + matrix.Right * 1000, Color.Red, Color.Red);
MyDebugDraw.DrawLine3D(pos, pos + matrix.Up * 1000, Color.Green, Color.Green);
MyDebugDraw.DrawLine3D(pos, pos + matrix.Forward * 1000, Color.Blue, Color.Blue);
          */
            /*
            pos = WorldMatrix.Translation;
            matrix = Physics.Matrix;
            matrix.Right *= m_targetsRelativePosition.X;
            matrix.Up *= m_targetsRelativePosition.Y;
            matrix.Forward *= m_targetsRelativePosition.Z;

            MyDebugDraw.DrawLine3D(pos, pos + matrix.Right * 1000, Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(pos, pos + matrix.Up * 1000, Color.Green, Color.Green);
            MyDebugDraw.DrawLine3D(pos, pos + matrix.Forward * 1000, Color.Blue, Color.Blue);
              */

#if DEBUG_MISSILE
            for (int i = 0; i < m_trailDebug.Count - 1; i++)
            {
                MyDebugDraw.DrawLine3D(m_trailDebug[i], m_trailDebug[i + 1] , Color.White, Color.White);
            }
#endif

            //if (this.Physics.CollisionSkin.Collisions.Count > 0)
            //{
            //    MyDebugDraw.DrawLine3D(GetPosition(), GetPosition() + (Vector3.Negate(this.Physics.CollisionSkin.Collisions[0].DirToBody0) * 1000), Color.White, Color.White);
            //}

            return true;
        }

        private Vector3? DetectDecoys()
        {
            var bBox = new BoundingBox(GetPosition() - new Vector3(.5f * MyDecoyFlareConstants.DECOY_ATTRACT_RADIUS),
                                       GetPosition() + new Vector3(.5f * MyDecoyFlareConstants.DECOY_ATTRACT_RADIUS));
            Vector3? result = null;
            var elements = MyEntities.GetElementsInBox(ref bBox);

            foreach (var rbElement in elements)
            {
                var rigidBody = rbElement.GetRigidBody();
                if (rigidBody == null) continue;

                var decoy = ((MyPhysicsBody)rigidBody.m_UserData).Entity as MyDecoyFlare;
                if (decoy == null) continue;

                //if (MyFactions.GetFactionsRelation(this.OwnerEntity.Faction, decoy.OwnerEntity.Faction) != MyFactionRelationEnum.Enemy)
                //    continue; // only attracts enemy missiles

                var decoyDistanceSquared = (decoy.GetPosition() - this.GetPosition()).LengthSquared();
                if (decoyDistanceSquared < MyDecoyFlareConstants.DECOY_ATTRACT_RADIUS * MyDecoyFlareConstants.DECOY_ATTRACT_RADIUS)
                {
                    result = decoy.GetPosition();
                    break;
                }
            }
            elements.Clear();
            return result;
        }


        protected Vector3 GetDesiredVelocity(MyEntity targetEntity)
        {
            switch (m_missileType)
            {
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP:
                    //fly straight ahead
                    return WorldMatrix.Forward;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection:
                //Seems to be buggy when dont hit
                //return GetPredictedVelocityToTargetShipPrecise(targetShip);
                //return WorldMatrix.Forward;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection:
                    return GetPredictedVelocityToTargetEntity(targetEntity);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsInMyHemisphere(MyEntity targetEntity)
        {
            var direction = targetEntity.GetPosition() - GetPosition();
            direction.Normalize();
            var dot = Vector3.Dot(direction, WorldMatrix.Forward);
            return dot > 0.5f;
        }

        /// <summary>
        /// Simple prediction, not used
        /// </summary>
        /// <param name="targetEntity"></param>
        /// <returns></returns>
        protected Vector3 GetPredictedVelocityToTargetEntity(MyEntity targetEntity)
        {
            Vector3? targetDir = null;

            Vector3? decoyPosition = DetectDecoys();
            if (decoyPosition.HasValue)
            {
                var decoyToMissile = decoyPosition.Value - GetPosition();
                if (decoyToMissile.LengthSquared() < MyDecoyFlareConstants.DECOY_KILL_RADIUS * MyDecoyFlareConstants.DECOY_KILL_RADIUS)
                {
                    Explode();
                    return Vector3.Zero;
                }

                targetDir = decoyToMissile;
                targetDir = MyMwcUtils.Normalize(targetDir.Value);
            }
            else if (targetEntity != null)
            {
                //calculate position to navigate missile to
                targetDir = targetEntity.WorldMatrix.Translation - WorldMatrix.Translation; //simple solution

                float distance = (this.WorldMatrix.Translation - targetEntity.WorldMatrix.Translation).Length();
                float time = distance / m_actualSpeed;

                if (time > MyGuidedMissileConstants.MISSILE_PREDICATION_TIME_TRESHOLD)
                {
                    m_predicatedPosition = time * m_targetVelocity + targetEntity.WorldMatrix.Translation;
                    targetDir = m_predicatedPosition - WorldMatrix.Translation; //simple solution
                }

                targetDir = MyMwcUtils.Normalize(targetDir.Value);
            }

            if (targetDir.HasValue)
            {
                //create new navigation direction
                Vector3 actualDir = WorldMatrix.Forward;

                Vector3 rotationAxis = Vector3.Cross(actualDir, targetDir.Value);
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(rotationAxis, m_turnSpeed * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                rotationMatrix.Right = MyMwcUtils.Normalize(rotationMatrix.Right);
                rotationMatrix.Up = MyMwcUtils.Normalize(rotationMatrix.Up);
                rotationMatrix.Forward = MyMwcUtils.Normalize(rotationMatrix.Forward);

                Matrix newBodyMatrix = WorldMatrix * rotationMatrix;

                newBodyMatrix.Translation = WorldMatrix.Translation;

                SetWorldMatrix(newBodyMatrix);

                return newBodyMatrix.Forward;
            }

            //no target ship, fly straight ahead
            return WorldMatrix.Forward;
        }

        /// <summary>
        /// Advanced prediction based on current target and missile velocity
        /// </summary>
        /// <param name="targetShip"></param>
        /// <returns></returns>
        protected Vector3 GetPredictedVelocityToTargetShipPrecise(MySmallShip targetShip)
        { //create new dir navigated to target ship.
            if (targetShip != null)
            {
                //calculate position to navigate missile to
                Vector3 idealDir = targetShip.WorldMatrix.Translation - WorldMatrix.Translation; //simple solution

                float distance = (this.WorldMatrix.Translation - targetShip.WorldMatrix.Translation).Length();
                float time = distance / m_actualSpeed;

                if (time > MyGuidedMissileConstants.MISSILE_PREDICATION_TIME_TRESHOLD)
                {
                    m_predicatedPosition = time * m_targetVelocity + targetShip.WorldMatrix.Translation;
                    idealDir = m_predicatedPosition - WorldMatrix.Translation; //simple solution
                }

                idealDir = MyMwcUtils.Normalize(idealDir);


                Vector3 targetVelocity = targetShip.Physics.LinearVelocity;
                Vector3 targetPosition = targetShip.WorldMatrix.Translation;
                Vector3 position = this.WorldMatrix.Translation;
                float speed = this.Physics.LinearVelocity.Length();

                //These variables are named by linear equation constants. Solution
                //of the equation is the time when missile and target collide
                float a = targetVelocity.LengthSquared() - speed * speed;
                float b = 2 * (Vector3.Dot(targetVelocity, targetPosition) - Vector3.Dot(targetVelocity, position));
                float c = (position.LengthSquared() + targetPosition.LengthSquared() - 2 * (Vector3.Dot(targetPosition, position)));
                float d = (b * b) - (4 * a * c);
                if (d >= 0)
                    d = (float)Math.Sqrt(d);
                else
                {
                    return idealDir;
                }

                //We test both solutions and find better one
                float t = (-b + d) / (2 * a);
                Vector3 v1 = (targetVelocity * t + targetPosition - position) / (speed * t);
                Vector3 pr1 = this.Physics.LinearVelocity * t;
                v1 = MyMwcUtils.Normalize(v1);
                t = (-b - d) / (2 * a);
                Vector3 v2 = (targetVelocity * t + targetPosition - position) / (speed * t);
                Vector3 pr2 = this.Physics.LinearVelocity * t;
                v2 = MyMwcUtils.Normalize(v2);

                float dot1 = Vector3.Dot(this.WorldMatrix.Forward, v1);
                float dot2 = Vector3.Dot(this.WorldMatrix.Forward, v2);

                //Which solution is closer to current missile dir?
                if ((dot1 - 1) * (dot1 - 1) < (dot2 - 1) * (dot2 - 1))
                {
                    idealDir = v1;
                    m_predicatedPosition = pr1 + this.GetPosition();
                }
                else
                {
                    idealDir = v2;
                    m_predicatedPosition = pr2 + this.GetPosition();
                }

                //create new navigation direction
                Vector3 actualDir = WorldMatrix.Forward;

                Vector3 rotationAxis = Vector3.Cross(actualDir, idealDir);
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(rotationAxis, m_turnSpeed * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                rotationMatrix.Right = MyMwcUtils.Normalize(rotationMatrix.Right);
                rotationMatrix.Up = MyMwcUtils.Normalize(rotationMatrix.Up);
                rotationMatrix.Forward = MyMwcUtils.Normalize(rotationMatrix.Forward);

                Matrix newBodyMatrix = WorldMatrix * rotationMatrix;

                newBodyMatrix.Translation = WorldMatrix.Translation;

                SetWorldMatrix(newBodyMatrix);

                return newBodyMatrix.Forward;
            }

            //no target ship, fly straight ahead
            return WorldMatrix.Forward;
        }

        /// <summary>
        /// Called on some contact start with this entity.
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        protected override void OnContactStart(MyContactEventInfo contactInfo)
        {
            MyEntity collidedEntity = contactInfo.GetOtherEntity(this);
            Debug.Assert(!collidedEntity.Closed);

            MyMissile missile = collidedEntity as MyMissile;

            if ((missile != null) && (OwnerEntity == missile.OwnerEntity))
            {   //We have hit another missile of ours
                return;
            }

            if (collidedEntity is MySmallShip
                && OwnerEntity == MySession.PlayerShip 
                && MySession.PlayerFriends.Contains(collidedEntity as MySmallShip))
            {
                //missiles wont hit out friends
                return;
            }

            base.OnContactStart(contactInfo);

            m_collidedEntity = collidedEntity;
            m_collidedEntity.OnClose += m_collidedEntity_OnClose;
            m_collisionPoint = contactInfo.m_ContactPoint;

            if (this.OwnerEntity is MySmallShip && (MySmallShip)this.OwnerEntity == MySession.PlayerShip && m_collidedEntity is MyStaticAsteroid && !m_collidedEntity.IsDestructible)
            {
                HUD.MyHud.ShowIndestructableAsteroidNotification();
            }

            Explode();
        }

        public Action<MyEntity> m_collidedEntity_OnClose;
        void collidedEntity_OnClose(MyEntity obj)
        {
            m_collidedEntity = null;
            obj.OnClose -= m_collidedEntity_OnClose;
        }

        public override void Explode()
        {
            base.Explode();

            MyDangerZones.Instance.NotifyExplosion(WorldMatrix.Translation, m_ammoProperties.ExplosionRadius, OwnerEntity);
        }
    }
}
