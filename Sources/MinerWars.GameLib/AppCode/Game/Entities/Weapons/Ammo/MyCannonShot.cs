using System;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using SysUtils.Utils;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using UniversalLauncher;
    using MinerWars.AppCode.Game.Entities.Prefabs;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Entities.Ships.AI;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Game.Managers.Session;

    class MyCannonShot : MyAmmoBase
    {
        MyLight m_light;
        MySoundCue? m_thrusterCue;

        bool m_wasPenetration;
        bool m_hasExplosion = false;
        MyExplosionTypeEnum m_explosionType;
        MyMwcObjectBuilder_SmallShip_Ammo m_usedAmmo;
        Vector3 m_penetrationOrigin;
        MyVoxelMap m_penetratedVoxelMap;
        BoundingSphere m_cuttingSphere;

        MyParticleEffect m_smokeEffect;
        private MyEntity m_collidedEntity;

        Vector3? m_collisionPoint;
        Action<MyEntity> m_collidedEntityClosedHandler;

        public MyCannonShot()
        {
            m_collidedEntityClosedHandler = new Action<MyEntity>(collidedEntity_OnClose);
        }

        //  Parameter-less constructor - because missiles are stored in object pool
        public virtual void Init()
        {
            base.Init(MyModelsEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic);

            m_cuttingSphere = new BoundingSphere(GetPosition(), 2.0f);

            this.Physics.CollisionLayer = MyConstants.COLLISION_LAYER_MISSILE;
            m_canByAffectedByExplosionForce = false;
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 directionNormalized, MyMwcObjectBuilder_SmallShip_Ammo usedAmmo, MySmallShip minerShip)
        {
            m_usedAmmo = usedAmmo;
            m_ammoProperties = MyAmmoConstants.GetAmmoProperties(usedAmmo.AmmoType);
            m_gameplayProperties = MyGameplayConstants.GetGameplayProperties(m_usedAmmo, Faction);
            m_penetratedVoxelMap = null;
            m_wasPenetration = false;
            m_hasExplosion = false;
            m_isExploded = false;
            m_collidedEntity = null;
            m_collisionPoint = null;
                
            Matrix orientation = GetWorldRotation();
            Vector3 pos = position;

            //  Play missile thrust cue (looping)
            m_thrusterCue = MyAudio.AddCue3D(MySoundCuesEnum.WepMissileFly, pos, orientation.Forward, orientation.Up, this.Physics.LinearVelocity);

            m_light = MyLights.AddLight();
            if (m_light != null)
            {
                m_light.Start(MyLight.LightTypeEnum.PointLight, GetPosition(), MyMissileHelperUtil.GetCannonShotLightColor(), 1, MyMissileConstants.MISSILE_LIGHT_RANGE);
            }

            m_diffuseColor = m_ammoProperties.TrailColor;

            switch (usedAmmo.AmmoType)
            {
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Armor_Piercing_Incendiary:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive:
                    m_explosionType = MyExplosionTypeEnum.MISSILE_EXPLOSION;
                    break;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem:
                    m_explosionType = MyExplosionTypeEnum.BIOCHEM_EXPLOSION;
                    break;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP:
                    m_explosionType = MyExplosionTypeEnum.EMP_EXPLOSION;
                    break;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster:
                    m_explosionType = MyExplosionTypeEnum.BLASTER_EXPLOSION;
                    break;
                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                    break;
            }

            this.Physics.Mass = m_gameplayProperties.WeightPerUnit;

            Vector3? correctedDirection = null;
            if (MyGameplayConstants.GameplayDifficultyProfile.EnableAimCorrection)
            {
                if (minerShip == MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip)
                {
                    correctedDirection = MyEntities.GetDirectionFromStartPointToHitPointOfNearestObject(minerShip, position, m_ammoProperties.MaxTrajectory);
                }
            }

            if (correctedDirection != null)
                directionNormalized = correctedDirection.Value;

            base.Start(position, initialVelocity, directionNormalized, m_ammoProperties.DesiredSpeed, minerShip);

            if (correctedDirection != null) //override the base class behaviour, update the missile direction
            {
                Matrix ammoWorld = minerShip.WorldMatrix;
                ammoWorld.Translation = position;
                ammoWorld.Forward = correctedDirection.Value;

                SetWorldMatrix(ammoWorld);
            }

            m_smokeEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_CannonShot);
            m_smokeEffect.AutoDelete = false;
            m_smokeEffect.WorldMatrix = WorldMatrix;
        }

        public override void UpdateBeforeSimulation()
        {
            try
            {
                MyRender.GetRenderProfiler().StartProfilingBlock("MyCannonShot.UpdateBeforeSimulation");

                if (this.WorldMatrix.Translation != m_previousPosition)
                {
                    MyLine line = new MyLine(this.WorldMatrix.Translation, m_previousPosition);
                    MyDangerZones.Instance.Notify(line, OwnerEntity);
                }

                //  Kill this missile
                if (m_isExploded && !m_wasPenetration)
                {
                    //  Create explosion
                    MyExplosion newExplosion = MyExplosions.AddExplosion();
                    if (newExplosion != null)
                    {
                        float radius = MyMwcUtils.GetRandomFloat(m_ammoProperties.ExplosionRadius - 2, m_ammoProperties.ExplosionRadius + 2);
                        BoundingSphere explosionSphere = new BoundingSphere((m_collisionPoint.HasValue ? m_collisionPoint.Value : GetPosition()), radius);
                        MyExplosionInfo info = new MyExplosionInfo(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, explosionSphere, m_explosionType, true)
                        {
                            GroupMask = Physics.GroupMask,
                            CascadeLevel = CascadedExplosionLevel,
                            HitEntity = m_collidedEntity,
                            OwnerEntity = this.OwnerEntity,
                            Direction = WorldMatrix.Forward,
                            ParticleScale = 1.5f,
                            VoxelExplosionCenter = explosionSphere.Center + radius * WorldMatrix.Forward * 0.6f,
                        };
                        info.CreateParticleEffect = !m_hasExplosion;
                        newExplosion.Start(ref info);
                    }

                    if (m_collidedEntity != null && !m_collidedEntity.IsExploded())
                    {
                        m_collidedEntity.Physics.AddForce(
                            MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                            WorldMatrix.Forward * MyMissileConstants.HIT_STRENGTH_IMPULSE,
                            GetPosition() + MyMwcUtils.GetRandomVector3Normalized() * 2,
                            MyMissileConstants.HIT_STRENGTH_IMPULSE * MyMwcUtils.GetRandomVector3Normalized());
                    }

                    MarkForClose();

                    return;
                }

                base.UpdateBeforeSimulation();

                //  Chech timeout and max distance
                if ((m_elapsedMiliseconds > MyCannonConstants.SHOT_TIMEOUT) || (Vector3.Distance(this.WorldMatrix.Translation, m_origin) >= m_ammoProperties.MaxTrajectory))
                {
                    MarkForClose();
                    return;
                }



                Matrix orientation = GetWorldRotation();

                //  Update thruster cue/sound
                MyAudio.UpdateCuePosition(m_thrusterCue, this.WorldMatrix.Translation, orientation.Forward, orientation.Up, this.Physics.LinearVelocity);

                Vector3 pos = this.WorldMatrix.Translation;

                if (m_penetratedVoxelMap == null)
                {
                    if (m_smokeEffect != null)
                        m_smokeEffect.WorldMatrix = WorldMatrix;
                }
                /*
          if (m_wasPenetration)
          {
              //  Create explosion
              MyExplosion newExplosion = MyExplosions.AddExplosion();
              if (newExplosion != null)
              {
                  float radius = MyMwcUtils.GetRandomFloat(1, 2);
                  float particleScale = 2.2f; // must be large enough to cover the hole
                  newExplosion.StartWithPositionOffset(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, m_explosionType, m_penetrationOrigin - WorldMatrix.Forward * 2, radius, MyExplosionsConstants.EXPLOSION_LIFESPAN, CascadedExplosionLevel, particleScale: particleScale, hitEntity: m_collidedEntity, ownerEntity: m_ownerEntity);
              }
              m_wasPenetration = false;
              m_hasExplosion = true;
          }        */

                if (m_usedAmmo.AmmoType == MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive)
                {
                    //  Look for small ships in shots's proximity
                    BoundingSphere boundingSphere = new BoundingSphere(GetPosition(), MyCannonShotConstants.PROXIMITY_DETECTION_RADIUS);
                    BoundingBox boundingBox = new BoundingBox();
                    BoundingBox.CreateFromSphere(ref boundingSphere, out boundingBox);

                    var elements = MyEntities.GetElementsInBox(ref boundingBox);
                    for (int i = 0; i < elements.Count; i++)
                    {
                        var rigidBody = (MyPhysicsBody)elements[i].GetRigidBody().m_UserData;
                        var entity = rigidBody.Entity;


                        if (!(entity is MinerWars.AppCode.Game.Entities.MySmallShip))
                            continue;
                        if (entity == OwnerEntity)
                            continue;
                        if (entity == this)
                            continue;

                        Explode(entity);
                        break;
                    }
                    elements.Clear();
                }

                /*
                if (m_usedAmmo.AmmoType == MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster)
                {
                    m_cuttingSphere.Center = GetPosition();

                    //  We found voxel so lets make tunel into it
                    MyPhysObjectBase collisionResult = MyEntities.GetIntersectionWithSphere(ref m_cuttingSphere, this, (MySmallShip)Parent);
                    if (collisionResult is MyVoxelMap)
                    {
                        MyVoxelMap voxelMap = collisionResult as MyVoxelMap;
                        if (m_penetratedVoxelMap == null)
                        {
                            m_penetratedVoxelMap = voxelMap;
                            m_penetrationOrigin = GetPosition();
                        }

                        Game.Voxels.MyVoxelGenerator.CutOutSphereFast(voxelMap, m_cuttingSphere);
                    }
                } */
                /*
          if (m_usedAmmo.AmmoType == MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster)
          {
              if (m_penetratedVoxelMap != null)
              {
                  //MyCannonShotConstants.BUSTER_PENETRATION_LENGTH
                  float busterPenetrationLength = m_ammoProperties.ExplosionRadius * 0.75f;
                  if (Vector3.Distance(m_penetrationOrigin, GetPosition()) >= busterPenetrationLength)
                  {
                      m_collisionPoint = GetPosition(); //We want to explode inside voxel, not on collision point
                      Explode(m_penetratedVoxelMap);
                  }
              }
          }    */
            }
            finally
            {
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);

            //  Update light position
            if (m_light != null)
            {
                m_light.SetPosition(GetPosition());
                m_light.Color = MyMissileHelperUtil.GetCannonShotLightColor();
                m_light.Range = MyMissileConstants.MISSILE_LIGHT_RANGE;
            }
        }

        //  Kills this missile. Must be called at her end (after explosion or timeout)
        public override void Close()
        {
            base.Close();

            if (m_collidedEntity != null)
            {
                m_collidedEntity.OnClose -= m_collidedEntityClosedHandler;
                m_collidedEntity = null;
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

            if (m_smokeEffect != null)
            {
                m_smokeEffect.Stop();
                m_smokeEffect = null;
            }

            MyCannonShots.Remove(this);
        }

        /// <summary>
        /// Called on some contact with this entity.
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        protected override void OnContactStart(MyContactEventInfo contactInfo)
        {
            base.OnContactStart(contactInfo);

            var collidedEntity = contactInfo.GetOtherEntity(this);

            /*
            if (IsExploded || contactInfo.GetOtherEntity(this) != m_collidedEntity || m_wasPenetration)
            {
                return;
            } */

            /*
            if (Vector3.Dot(WorldMatrix.Forward, contactInfo.m_ContactNormal) < MyMissileConstants.MISSILE_MINIMAL_HIT_DOT)
            {
                //Hit wasnt facing the target, just slide or something
                return;
            } */

            if (collidedEntity is MySmallShip
              && OwnerEntity == MySession.PlayerShip
              && MySession.PlayerFriends.Contains(collidedEntity as MySmallShip))
            {
                //missiles wont hit out friends
                return;
            }

            m_collidedEntity = collidedEntity;
            m_collidedEntity.OnClose += m_collidedEntityClosedHandler;
            m_collisionPoint = contactInfo.m_ContactPoint;


            if (this.OwnerEntity is MySmallShip && (MySmallShip)this.OwnerEntity == MySession.PlayerShip && m_collidedEntity is MyStaticAsteroid && !m_collidedEntity.IsDestructible)
            {
                HUD.MyHud.ShowIndestructableAsteroidNotification();
            }
              /*
            if (m_usedAmmo.AmmoType == MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster)
            {
                if (m_collidedEntity is MyVoxelMap)
                {
                    return;
                }
            }   */

            Explode(m_collidedEntity);
        }

        void collidedEntity_OnClose(MyEntity obj)
        {
            m_collidedEntity = null;
            obj.OnClose -= m_collidedEntityClosedHandler;
        }

        protected override bool OnContact(ref MyRBSolverConstraint constraint)
        {
            MyEntity collidedEntity = constraint.GetOtherEntity(this);

            /*
            // we take the closest collided entity
            if (m_collidedEntity == null || Vector3.Distance(this.GetPosition(), constraint.m_ContactPoint) < Vector3.Distance(this.GetPosition(), m_collisionPoint.Value))
            {
                m_collisionPoint = constraint.m_ContactPoint;
                if (collidedEntity != m_collidedEntity)
                {
                    m_collidedEntity = collidedEntity;
                    if (m_collidedEntity is MyVoxelMap && m_usedAmmo.AmmoType == MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster)
                    {
                        if (m_penetratedVoxelMap == null)
                        {
                            m_penetratedVoxelMap = m_collidedEntity as MyVoxelMap;
                            m_penetrationOrigin = GetPosition();
                            m_wasPenetration = true;
                        }

                        return false;
                    }
                }
            }

            if (m_penetratedVoxelMap != null)
                return false;
              */
            return true;
        }

        public void Explode(MyEntity collideEntity)
        {
            if (m_usedAmmo.AmmoType == MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive)
            { //make flying shrapnels
                int shrapnels = MyCannonShotConstants.PROXIMITY_SHRAPNELS_COUNT;
                while (shrapnels-- > 0)
                {
                    Vector3 projectileForwardVector = MyUtilRandomVector3ByDeviatingVector.GetRandom(WorldMatrix.Forward, MathHelper.TwoPi);
                    //MyProjectiles.Add(
                      //  MyAmmoConstants.GetAmmoProperties(MyAmmoPropertiesEnum.Shrapnel),
                        //Parent, GetPosition(), Vector3.Zero, projectileForwardVector, false, 3, this);

                    MyProjectiles.Add(
                        MyAmmoConstants.GetAmmoProperties(MyAmmoPropertiesEnum.Shrapnel),
                        Parent, GetPosition(), Vector3.Zero, projectileForwardVector, false, 3, this);
                }
            }

            Explode();
        }
    }
}