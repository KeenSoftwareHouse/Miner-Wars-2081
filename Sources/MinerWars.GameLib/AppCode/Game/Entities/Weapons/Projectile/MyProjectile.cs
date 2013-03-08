using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using Audio;
    using Cockpit;
    using CommonLIB.AppCode.ObjectBuilders.Object3D;
    using CommonLIB.AppCode.Utils;
    using Decals;
    using MinerWarsMath;
    using TransparentGeometry;
    using UniversalLauncher;
    using Utils;
    using System;
    using Prefabs;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Entities.Ships.AI;
    using MinerWars.AppCode.Game.Sessions;

    internal delegate void MyCustomProjectileMethod(MySmallShip bot, float health);
    internal delegate void MyCustomHitParticlesMethod(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MyEntity weapon, MyEntity ownerEntity = null);
    internal delegate void MyCustomHitMaterialMethod(ref Vector3 hitPoint, ref Vector3 normal, ref Vector3 direction, MyEntity physObject, MySurfaceImpactEnum surfaceImpact, MyEntity weapon);

    public enum MySurfaceImpactEnum
    {
        METAL,
        DESTRUCTIBLE,
        INDESTRUCTIBLE,
    }

    class MyProjectile
    {
        //  Projectiles are killes in two states. First we get collision/timeout in update, but still need to draw
        //  trail polyline, so we can't remove it from buffer. Second state is after 'killed' projectile is drawn
        //  and only then we remove it from buffer.
        enum MyProjectileStateEnum : byte
        {
            ACTIVE,
            KILLED,
            KILLED_AND_DRAWN
        }

        const int CHECK_INTERSECTION_INTERVAL = 5; //projectile will check for intersection each n-th frame with n*longer line

        class MyProjectileGroupInfo
        {
            public bool Killed;
        }

        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  So don't initialize members here, do it in Start()

        MyProjectileStateEnum m_state;
        Vector3 m_origin;
        Vector3 m_velocity;
        Vector3 m_externalVelocity;
        Vector3 m_directionNormalized;
        float m_speed;
        float m_externalAddition;
        float m_maxTrajectory;

        Vector3 m_position;
        MyEntity m_ignorePhysObject;
        MyEntity m_weapon;

        public MyTransparentMaterialEnum? FrontBillboardMaterial = null;
        public float FrontBillboardSize = 1;
        public bool BlendByCameraDirection = false;
        public float LengthMultiplier = 1;

        //  Type of this projectile
        MyAmmoProperties m_ammoProperties;

        public MyEntity OwnerEntity = null;

        /*
        MyCustomHitParticlesMethod m_particlesMethod;
        MyCustomHitMaterialMethod m_materialMethod;
        float m_maxTrajectoryLength;
        Vector3 m_trailColor;
        float m_healthDamage;
        bool m_explosive;
         */ 
        float m_thicknessMultiplier;

        bool m_groupStart;

        public bool IsDummy { get; private set; }

        int m_checkIntersectionIndex; //actual index to keep distributed checking intersection
        static int checkIntersectionCounter = 0; //counter of started projectiles
        //Vector3 m_positionToCheck; //position which needs to be tested for line intersection
        bool m_positionChecked;

        MyProjectileGroupInfo m_ownGroup = new MyProjectileGroupInfo();
        MyProjectileGroupInfo m_sharedGroup;
        static MyProjectileGroupInfo LastProjectileGroup;

        MyParticleEffect m_trailEffect;
    
        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(MyAmmoProperties ammoProperties, MyEntity ignoreEntity, Vector3 origin, Vector3 initialVelocity, Vector3 directionNormalized, 
            bool groupStart, float thicknessMultiplier, MyEntity weapon
            )
        {
            if (MySession.Is25DSector)
            {
                directionNormalized.Y = 0;
                directionNormalized.Normalize();
                initialVelocity.Y = 0;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Projectile.Start");
            m_ammoProperties = ammoProperties;
            m_state = MyProjectileStateEnum.ACTIVE;
            m_ignorePhysObject = ignoreEntity;
            m_origin = origin;
            m_position = origin;
            m_externalAddition = 1.0f;
            m_weapon = weapon;

            IsDummy = weapon != null && weapon.IsDummy;

            LengthMultiplier = 1;
            FrontBillboardMaterial = null;
            FrontBillboardSize = 1;
            BlendByCameraDirection = false;

            Vector3? correctedDirection = null;

            if (MyGameplayConstants.GameplayDifficultyProfile.EnableAimCorrection)
            {
                MyEntity entityToCheck;
                if (MyGuiScreenGamePlay.Static.ControlledEntity is MyPrefabLargeWeapon)
                {
                    entityToCheck = (MyGuiScreenGamePlay.Static.ControlledEntity as MyPrefabLargeWeapon).GetGun();
                }
                else
                {
                    entityToCheck = MyGuiScreenGamePlay.Static.ControlledEntity;
                }
                // TODO: Make proper test that source off projectile is player ship, testing ignore object is STUPID!
                if (m_ammoProperties.AllowAimCorrection && (ignoreEntity == entityToCheck)) // Autoaim only available for player
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Projectile.Start autoaim generic");
                    //Intersection ignores children of "ignoreEntity", thus we must not hit our own barrels
                    correctedDirection = MyEntities.GetDirectionFromStartPointToHitPointOfNearestObject(ignoreEntity, origin, m_ammoProperties.MaxTrajectory);
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
            }

            if (correctedDirection != null)
            {
                m_directionNormalized = correctedDirection.Value;
            }
            else
            {
                m_directionNormalized = directionNormalized;
            }

            m_speed = ammoProperties.DesiredSpeed * (ammoProperties.SpeedVar > 0.0f ? MyMwcUtils.GetRandomFloat(1 - ammoProperties.SpeedVar, 1 + ammoProperties.SpeedVar) : 1.0f);
            m_externalVelocity = initialVelocity;
            m_velocity = m_directionNormalized * m_speed;
            m_maxTrajectory = ammoProperties.MaxTrajectory * MyMwcUtils.GetRandomFloat(0.8f, 1.2f); // +/- 20%

           
            m_thicknessMultiplier = thicknessMultiplier;

            m_checkIntersectionIndex = checkIntersectionCounter % CHECK_INTERSECTION_INTERVAL;
            checkIntersectionCounter += 3;
            m_positionChecked = false;
            m_groupStart = groupStart;

            if (groupStart)
            {
                m_trailEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Trail_Shotgun);
                m_trailEffect.AutoDelete = false;
                m_trailEffect.WorldMatrix = Matrix.CreateTranslation(m_position);
            }

            if (groupStart)
            {
                LastProjectileGroup = m_ownGroup;
                m_ownGroup.Killed = false;
            }

            if (LastProjectileGroup != null && LastProjectileGroup.Killed == false)
            {
                m_sharedGroup = LastProjectileGroup;
            }
            else
                m_sharedGroup = null;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        
        //  Update position, check collisions, etc.
        //  Return false if projectile dies/timeouts in this tick.
        public bool Update()
        {
            //  Projectile was killed , but still not last time drawn, so we don't need to do update (we are waiting for last draw)
            if (m_state == MyProjectileStateEnum.KILLED) 
                return true;

            //  Projectile was killed and last time drawn, so we can finally remove it from buffer
            if (m_state == MyProjectileStateEnum.KILLED_AND_DRAWN)
            {
                if (m_trailEffect != null)
                {
                    // stop the trail effect
                    m_trailEffect.Stop();
                    m_trailEffect = null;
                }

                return false;
            }

            Vector3 position = m_position;
            m_position += m_velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            m_velocity = m_externalVelocity * m_externalAddition + m_directionNormalized * m_speed;
            if (m_externalAddition < 1.0f)
                m_externalAddition *= 0.5f;

            //  Distance timeout
            float trajectoryLength = Vector3.Distance(m_position, m_origin);
            if (trajectoryLength >= m_maxTrajectory)
            {
                if (m_trailEffect != null)
                {
                    // stop the trail effect
                    m_trailEffect.Stop();
                    m_trailEffect = null;
                }

                m_state = MyProjectileStateEnum.KILLED;
                return true;
            }

            if (m_trailEffect != null)
                m_trailEffect.WorldMatrix = Matrix.CreateTranslation(m_position);

            m_checkIntersectionIndex++;
            m_checkIntersectionIndex = m_checkIntersectionIndex % CHECK_INTERSECTION_INTERVAL;
                                 
            //check only each n-th intersection
            if (m_checkIntersectionIndex != 0)
                return true;

            //  Calculate hit point, create decal and throw debris particles
            Vector3 lineEndPosition = position + CHECK_INTERSECTION_INTERVAL * (m_velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);

            MyLine line = new MyLine(m_positionChecked ? position : m_origin, lineEndPosition, true);
            m_positionChecked = true;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEntities.GetIntersectionWithLine()");
            MyIntersectionResultLineTriangleEx? intersection = MyEntities.GetIntersectionWithLine(ref line, m_ignorePhysObject, null, false);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyEntity physObject = intersection != null ? intersection.Value.Entity : null;
            if (physObject != null)
            {
                while (physObject.Physics == null && physObject.Parent != null)
                {
                    physObject = physObject.Parent;
                }
            }

            if ((intersection != null) && (physObject != null) && (physObject.Physics.CollisionLayer != MyConstants.COLLISION_LAYER_UNCOLLIDABLE) && m_ignorePhysObject != physObject)
            {
                MyIntersectionResultLineTriangleEx intersectionValue = intersection.Value;

                bool isPlayerShip = MySession.PlayerShip == physObject;

                MyMaterialType materialType = isPlayerShip ? MyMaterialType.PLAYERSHIP : physObject.Physics.MaterialType;

                //material properties
                MyMaterialTypeProperties materialProperties = MyMaterialsConstants.GetMaterialProperties(materialType);

                bool isProjectileGroupKilled = false;

                if (m_sharedGroup != null)
                {
                    isProjectileGroupKilled = m_sharedGroup.Killed;
                    m_sharedGroup.Killed = true;
                }

                if (!isProjectileGroupKilled)
                {
                    //  Play bullet hit cue 
                    MyAudio.AddCue3D(m_ammoProperties.IsExplosive ? materialProperties.ExpBulletHitCue : materialProperties.BulletHitCue, intersectionValue.IntersectionPointInWorldSpace, Vector3.Zero, Vector3.Zero, Vector3.Zero);
                }

                float decalAngle = MyMwcUtils.GetRandomRadian();

                //  If we hit the glass of a miner ship, we need to create special bullet hole decals
                //  drawn from inside the cockpit and change phys object so rest of the code will think we hit the parent
                //  IMPORTANT: Intersection between projectile and glass is calculated only for mining ship in which player sits. So for enemies this will be never calculated.
                if (intersection.Value.Entity is MyCockpitGlassEntity)
                {
                    if (!isProjectileGroupKilled)
                    {
                        MyCockpitGlassDecalTexturesEnum bulletHoleDecalTexture;
                        float bulletHoleDecalSize;

                        if (MyMwcUtils.GetRandomBool(3))
                        {
                            bulletHoleDecalTexture = MyCockpitGlassDecalTexturesEnum.BulletHoleOnGlass;
                            bulletHoleDecalSize = 0.25f;
                        }
                        else
                        {
                            bulletHoleDecalTexture = MyCockpitGlassDecalTexturesEnum.BulletHoleSmallOnGlass;
                            bulletHoleDecalSize = 0.1f;
                        }

                        //  Place bullet hole decal on player's cockpit glass (seen from inside the ship)
                        MyCockpitGlassDecals.Add(bulletHoleDecalTexture, bulletHoleDecalSize, decalAngle, 1.0f, ref intersectionValue, false);

                        //  Create hit particles throwed into the cockpit (it's simulation of broken glass particles)
                        //  IMPORTANT: This particles will be relative to miner ship, so we create them in object space coordinates and update them by object WorldMatrix every time we draw them
                        //MyParticleEffects.CreateHitParticlesGlass(ref intersectionValue.IntersectionPointInObjectSpace, ref intersectionValue.NormalInWorldSpace, ref line.Direction, physObject.Parent);
                    }
                }

                //  If this was "mine", it must explode
                else if (physObject is MyMineBase)
                {
                    m_state = MyProjectileStateEnum.KILLED;
                    if (!IsDummy)
                        (physObject as MyAmmoBase).Explode();
                    return true;
                }

                //  If this was missile, cannon shot, it must explode if it is not mine missile
                else if (physObject is MyAmmoBase)
                {
                    if (((MyAmmoBase)physObject).OwnerEntity == m_ignorePhysObject)
                    {
                        m_state = MyProjectileStateEnum.KILLED;
                        if (!IsDummy)
                            (physObject as MyAmmoBase).Explode();
                        return true;
                    }
                }

                else if (this.OwnerEntity is MySmallShip && (MySmallShip)this.OwnerEntity == MySession.PlayerShip && physObject is MyStaticAsteroid && !physObject.IsDestructible)
                {
                    if (this.m_ammoProperties.IsExplosive || (this.m_ammoProperties.AmmoType == MyAmmoType.Explosive && this.m_weapon is Weapons.MyShotGun))
                    {
                        HUD.MyHud.ShowIndestructableAsteroidNotification();
                    }
                }

                else if (!isProjectileGroupKilled && !isPlayerShip)
                {
                    //  Create smoke and debris particle at the place of voxel/model hit
                    m_ammoProperties.OnHitParticles(ref intersectionValue.IntersectionPointInWorldSpace, ref intersectionValue.Triangle.InputTriangleNormal, ref line.Direction, physObject, m_weapon, OwnerEntity);

                    MySurfaceImpactEnum surfaceImpact;
                    if (intersectionValue.Entity is MyVoxelMap)
                    {
                        var voxelMap = intersectionValue.Entity as MyVoxelMap;
                        var voxelCoord = voxelMap.GetVoxelCenterCoordinateFromMeters(ref intersectionValue.IntersectionPointInWorldSpace);
                        var material = voxelMap.GetVoxelMaterial(ref voxelCoord);
                        if (material == MyMwcVoxelMaterialsEnum.Indestructible_01 ||
                            material == MyMwcVoxelMaterialsEnum.Indestructible_02 ||
                            material == MyMwcVoxelMaterialsEnum.Indestructible_03 ||
                            material == MyMwcVoxelMaterialsEnum.Indestructible_04 ||
                            material == MyMwcVoxelMaterialsEnum.Indestructible_05_Craters_01)
                            surfaceImpact = MySurfaceImpactEnum.INDESTRUCTIBLE;
                        else
                            surfaceImpact = MySurfaceImpactEnum.DESTRUCTIBLE;
                    }
                    else if (intersectionValue.Entity is MyStaticAsteroid)
                        surfaceImpact = MySurfaceImpactEnum.INDESTRUCTIBLE;
                    else surfaceImpact = MySurfaceImpactEnum.METAL;

                    m_ammoProperties.OnHitMaterialSpecificParticles(ref intersectionValue.IntersectionPointInWorldSpace, ref intersectionValue.Triangle.InputTriangleNormal, ref line.Direction, physObject, surfaceImpact, m_weapon);
                }

                if (!(physObject is MyExplosionDebrisBase) && physObject != MySession.PlayerShip)
                {
                    //  Decal size depends on material. But for mining ship create smaller decal as original size looks to large on the ship.
                    float decalSize = MyMwcUtils.GetRandomFloat(materialProperties.BulletHoleSizeMin,
                                                                materialProperties.BulletHoleSizeMax);

                    //  Place bullet hole decal
                    float randomColor = MyMwcUtils.GetRandomFloat(0.5f, 1.0f);

                    MyDecals.Add(
                        materialProperties.BulletHoleDecal,
                        decalSize,
                        decalAngle,
                        new Vector4(randomColor, randomColor, randomColor, 1),
                        false,
                        ref intersectionValue, 
                        0.0f,
                        m_ammoProperties.DecalEmissivity, MyDecalsConstants.DECAL_OFFSET_BY_NORMAL);
                }

                if (!(physObject is MyVoxelMap) && !IsDummy)
                {
                    ApplyProjectileForce(physObject, intersectionValue.IntersectionPointInWorldSpace, m_directionNormalized, isPlayerShip);
                }


                //  If this object is miner ship, then shake his head little bit
                if (physObject is MySmallShip && !IsDummy)
                {
                    MySmallShip minerShip = (MySmallShip)physObject;
                    minerShip.IncreaseHeadShake(MyHeadShakeConstants.HEAD_SHAKE_AMOUNT_AFTER_PROJECTILE_HIT);
                }



                //Handle damage

                MyEntity damagedObject = intersectionValue.Entity;

                // not a very nice way to damage actual prefab associated with the large ship weapon (if MyPrefabLargeWeapon is reworked, it might change)
                if (damagedObject is MyLargeShipBarrelBase)
                {
                    damagedObject = damagedObject.Parent;
                }
                if (damagedObject is MyLargeShipGunBase)
                {
                    MyLargeShipGunBase physObj = damagedObject as MyLargeShipGunBase;
                    if (physObj.PrefabParent != null)
                        damagedObject = physObj.PrefabParent;
                }

                //  Decrease health of stricken object
                if (!IsDummy)
                {
                    damagedObject.DoDamage(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, m_ammoProperties.DamageType, m_ammoProperties.AmmoType, m_ignorePhysObject);
                    if (MyMultiplayerGameplay.IsRunning)
                    {
                        var ammo = MyAmmoConstants.FindAmmo(m_ammoProperties);
                        MyMultiplayerGameplay.Static.ProjectileHit(damagedObject, intersectionValue.IntersectionPointInWorldSpace, this.m_directionNormalized, ammo, this.OwnerEntity);
                    }
                }

                if (m_trailEffect != null)
                {
                    // stop the trail effect
                    m_trailEffect.Stop();
                    m_trailEffect = null;
                }

                //  Kill this projectile (set the position to intersection point, so we draw trail polyline only up to this point)
                m_position = intersectionValue.IntersectionPointInWorldSpace;
                m_state = MyProjectileStateEnum.KILLED;

                return true;
            }
                
            return true;
        }

        public static void ApplyProjectileForce(MyEntity physObject, Vector3 intersectionPosition, Vector3 normalizedDirection, bool isPlayerShip)
        {
            float impulseMultiplier = isPlayerShip ? MyMinerShipConstants.MINER_SHIP_PLAYER_PROJECTILE_HIT_MULTIPLIER : (MyMinerShipConstants.MINER_SHIP_PROJECTILE_HIT_MULTIPLIER);

            // To make it look good, bots gets impulse which rotates them
            if (physObject is MySmallShipBot)
            {
                // These are constants to make proper bot behaviour to physics
                const float torqueMult = 4;
                const float impulseMult = 20;

                impulseMultiplier *= impulseMult;
                var centerToIntersection = intersectionPosition - physObject.GetPosition();
                intersectionPosition = physObject.GetPosition() + centerToIntersection * torqueMult;
            }

            //  If we hit model that belongs to physic object, apply some force to it (so it's thrown in the direction of shoting)
            physObject.Physics.AddForce(
                    MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                    normalizedDirection * MyProjectilesConstants.HIT_STRENGTH_IMPULSE * impulseMultiplier,
                    intersectionPosition, Vector3.Zero);
        }

        //  Draw the projectile but only if desired polyline trail distance can fit in the trajectory (otherwise we will see polyline growing from the origin and it's ugly).
        //  Or draw if this is last draw of this projectile (useful for short-distance shots).
        public void Draw()
        {
            const float PROJECTILE_POLYLINE_DESIRED_LENGTH = 120;

            float trajectoryLength = Vector3.Distance(m_position, m_origin);
            if ((trajectoryLength > 0) || (m_state == MyProjectileStateEnum.KILLED))
            {
                if (m_state == MyProjectileStateEnum.KILLED)
                {
                    m_state = MyProjectileStateEnum.KILLED_AND_DRAWN;
                }

                if (!m_positionChecked)
                    return;
                
              
                    
                //  If we calculate previous position using normalized direction (insted of velocity), projectile trails will 
                //  look like coming from cannon, and that is desired. Even during fast movement, acceleration, rotation or changes in movement directions.
                //Vector3 previousPosition = m_position - m_directionNormalized * projectileTrailLength * 1.05f;
                Vector3 previousPosition = m_position - m_directionNormalized * PROJECTILE_POLYLINE_DESIRED_LENGTH * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                //Vector3 previousPosition = m_previousPosition;
                //Vector3 previousPosition = m_initialSunWindPosition - MyMwcUtils.Normalize(m_desiredVelocity) * projectileTrailLength;

                Vector3 direction = Vector3.Normalize(m_position - previousPosition);

                float projectileTrailLength = 40 * LengthMultiplier;// PROJECTILE_POLYLINE_DESIRED_LENGTH;

                projectileTrailLength *= MyMwcUtils.GetRandomFloat(0.6f, 0.8f);

                if (trajectoryLength < projectileTrailLength)
                {
                    projectileTrailLength = trajectoryLength;
                }

                previousPosition = m_position - projectileTrailLength * direction;
                if (m_externalAddition >= 1.0f)
                    m_externalAddition = 0.5f;


                //float color = MyMwcUtils.GetRandomFloat(1, 2);
                float color = MyMwcUtils.GetRandomFloat(1, 2);
                float thickness = m_thicknessMultiplier * MyMwcUtils.GetRandomFloat(0.2f, 0.3f);

                //  Line particles (polyline) don't look good in distance. Start and end aren't rounded anymore and they just
                //  look like a pieces of paper. Especially when zoom-in.
                thickness *= MathHelper.Lerp(0.2f, 0.8f, MyCamera.Zoom.GetZoomLevel());

                float alphaCone = 1;
                float alphaGlare = 1;

                if (BlendByCameraDirection)
                {
                    float angle = 1 - Math.Abs(Vector3.Dot(MyMwcUtils.Normalize(MyCamera.ForwardVector), direction));
                    alphaGlare = (float)Math.Pow(1 - angle, 2);
                    alphaCone = (1 - (float)Math.Pow(1 - angle, 30));
                }

                MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.ProjectileTrailLine, new Vector4(m_ammoProperties.TrailColor * color, 1) * alphaCone,
                    previousPosition, direction, projectileTrailLength, thickness);

                if (FrontBillboardMaterial.HasValue)
                {
                    MyTransparentGeometry.AddPointBillboard(FrontBillboardMaterial.Value, new Vector4(m_ammoProperties.TrailColor * color, 1) * alphaGlare, m_position, 0.8f * FrontBillboardSize, 0);
                }                   
            }
        }

        public void Close()
        {
            OwnerEntity = null;
            m_ignorePhysObject = null;
            m_weapon = null;
        }

    }
}