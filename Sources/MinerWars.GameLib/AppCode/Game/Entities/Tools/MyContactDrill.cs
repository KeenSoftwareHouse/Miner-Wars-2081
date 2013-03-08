using System.Text;
using KeenSoftwareHouse.Library.Memory;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Entities
{
    abstract class MyContactDrill : MyDrillBase
    {
        protected float m_rotatingSpeed;
        protected float m_maxRotatingSpeedDrilling;
        protected float m_maxRotatingSpeedIdle;

        protected Vector3 m_fakeSpherePositionLocal;  //  Fake sphere position relative to ship
        protected Vector3 m_fakeSpherePositionTransformed;  //  Fake sphere position in world coordinates
        protected BoundingSphere m_fakeCollisionSphere;

        protected float m_lastTimeDrilled;
        protected float m_lastTimeDrillNotCollidedWithVoxelMapInMiliseconds;
        protected float m_lastTimeParticlesAdded;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, parentObject,
                      position, forwardVector, upVector,
                      objectBuilder);

            m_rotatingSpeed = 0;
            m_ejectedDistance = 0;
        }

        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (!base.Shot(usedAmmo))
                return false;


            return true;
        }

        public override bool Eject()
        {
            if (!base.Eject())
                return false;

            m_rotatingSpeed = 0;

            return true;
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            CalculateRotationSpeed();

            //  Transform head position into world space
            Matrix worldMatrix = Parent.WorldMatrix;
            m_fakeSpherePositionLocal = LocalMatrix.Translation + 1.2f * m_range * m_modelLod0.BoundingBox.Size().Z * LocalMatrix.Forward;
            m_fakeSpherePositionTransformed = MyUtils.GetTransform(m_fakeSpherePositionLocal, ref worldMatrix);

            if (CurrentState != MyDrillStateEnum.Drilling)
                m_lastTimeDrillNotCollidedWithVoxelMapInMiliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        protected override void Drill()
        {
            //  Sphere which is used to make tunnel to voxel and sphere for testing collision with voxel
            m_fakeCollisionSphere = new BoundingSphere(m_fakeSpherePositionTransformed, m_radius);
            //  Check for collision with drill and world
            //MyEntity collisionResult = MyEntities.GetIntersectionWithSphere(ref m_fakeCollisionSphere, this, Parent, false, true);
            // bSphere collision doesn't work - the sphere is tested against LOD0 model, but it is hidden inside the COL model and the bSphere is too small to reach it - so I use line instead

            MyEntity collisionResult = null;
            MyLine line;

            if (MySession.Is25DSector)
            {
                line = new MyLine(m_positionMuzzleInWorldSpace - 10 * WorldMatrix.Forward, m_positionMuzzleInWorldSpace + 20 * WorldMatrix.Forward, true);
            }
            else
                line = new MyLine(m_positionMuzzleInWorldSpace - 10 * WorldMatrix.Forward, m_positionMuzzleInWorldSpace + 5 * WorldMatrix.Forward, true);

            MyIntersectionResultLineTriangleEx? intersection = MyEntities.GetIntersectionWithLine(ref line, Parent, this, true, true);
            if (intersection != null && intersection.Value.Entity.Physics != null)
            {
                collisionResult = intersection.Value.Entity;
            }

            if (!(collisionResult is MyVoxelMap))
            {
                m_lastTimeDrillNotCollidedWithVoxelMapInMiliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;

                if (!MySession.Is25DSector)
                {
                    ((MySmallShip)Parent).IncreaseHeadShake(MyDrillDeviceConstants.SHAKE_DURING_ROTATION);
                }

                StopDustEffect();

                if (collisionResult != null)
                {
                    var effect = MyParticlesManager.CreateParticleEffect((int) MyParticleEffectsIDEnum.MaterialHit_Autocannon_Metal);
                    effect.WorldMatrix = Matrix.CreateTranslation(m_fakeCollisionSphere.Center);
                    collisionResult.DoDamage(0, m_damage * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS, 0, MyDamageType.Drill, MyAmmoType.Basic, Parent);
                }
            }
            //  Display particles when we are in contact with voxel
            else
            {
                if (m_dustEffect == null)
                {
                    m_dustEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_DrillDust);
                }
                m_dustEffect.WorldMatrix = Matrix.CreateTranslation(m_fakeSpherePositionTransformed);
                m_dustEffect.UserScale = MySession.Is25DSector ? 3 : 1;
                ((MySmallShip)Parent).IncreaseHeadShake(MyDrillDeviceConstants.SHAKE_DURING_IN_VOXELS);
            }

            //  Play sound if there is collision with voxel 
            if (collisionResult != null)
            {
                if (collisionResult is MyStaticAsteroid)
                {
                    if (!collisionResult.IsDestructible)
                    {
                        MinerWars.AppCode.Game.HUD.MyHud.ShowIndestructableAsteroidNotification();
                    }
                }
                StartDrillingCue(collisionResult is MyVoxelMap);
                StopMovingCue();
            }
            else
            {
                StartMovingCue();
                StopDrillingCue();                
            }

            //  We found voxel so lets make tunel into it
            using (var voxelMapsFound = PoolList<MyVoxelMap>.Get())
            {
                bool drilled = false;
                bool drilledSomeDestructibleContent = false;
                MyVoxelMaps.GetListOfVoxelMapsWhoseBoundingSphereIntersectsSphere(ref m_fakeCollisionSphere, voxelMapsFound, null);

                int drillInterval = MySession.Is25DSector ? 100 : (int)MyDrillDeviceConstants.DRILL_INTERVAL_IN_MILISECONDS;
                int timerToDrillInterval = MySession.Is25DSector ? 100 : (int)MyDrillDeviceConstants.TIME_TO_DRILL_VOXEL_IN_MILISECONDS;

                foreach (MyVoxelMap voxelMap in voxelMapsFound)
                {

                    if ((collisionResult is MyVoxelMap) && ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeDrilled) > drillInterval)
                        && (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeDrillNotCollidedWithVoxelMapInMiliseconds) > timerToDrillInterval)
                    {
                        drilled = true;
                        float rangeStep = 0;
                        float radius = GetRadiusNeededForTunel();
                        BoundingSphere bigSphereForTunnel = new BoundingSphere(m_fakeCollisionSphere.Center, radius);
                        
                        while (rangeStep < m_range)
                        {
                            MyMwcVector3Int exactCenterOfDrilling = voxelMap.GetVoxelCoordinateFromMeters(bigSphereForTunnel.Center);

                            // we don't want to drill indestructible voxels or empty space
                            if (voxelMap.IsVoxelInVoxelMap(ref exactCenterOfDrilling) && voxelMap.GetVoxelMaterialIndestructibleContent(ref exactCenterOfDrilling) == MyVoxelConstants.VOXEL_CONTENT_FULL)
                            {
                                break;
                            }
                            else
                            {
                                drilledSomeDestructibleContent = true;
                            }
                            
                            CutOutFromVoxel(voxelMap, ref bigSphereForTunnel);

                            bigSphereForTunnel.Center += 2 * WorldMatrix.Forward;

                            rangeStep += 1;
                        }
                    }
                }
                if (drilled)
                {
                    m_lastTimeDrilled = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    if (!drilledSomeDestructibleContent)
                    {
                        HUD.MyHud.ShowIndestructableAsteroidNotification();
                    }
                }
            }
        }

        public override bool DebugDraw()
        {
            if (!base.DebugDraw())
                return false;

            //MyDebugDraw.DrawSphereWireframe(m_fakeCollisionSphere.Center, m_fakeCollisionSphere.Radius, Vector3.One, 1.0f);

            //MyLine line = new MyLine(m_positionMuzzleInWorldSpace - 10 * WorldMatrix.Forward, m_positionMuzzleInWorldSpace + 5 * WorldMatrix.Forward, true);
            //MyDebugDraw.DrawLine3D(line.From, line.To, Color.White, Color.White);

            return true;
        }

        protected void CalculateRotationSpeed()
        {
            if (MySession.Is25DSector)
                m_maxRotatingSpeedIdle = 10.4f;

            switch (CurrentState)
            {
                case MyDrillStateEnum.Activated:
                    {
                        if (m_rotatingSpeed < m_maxRotatingSpeedIdle)
                        {
                            m_rotatingSpeed += MyDrillDeviceConstants.ROTATION_ACCELERATION;
                            m_rotatingSpeed = MathHelper.Clamp(m_rotatingSpeed, 0, m_maxRotatingSpeedIdle);
                        }
                        else
                        {
                            m_rotatingSpeed -= MyDrillDeviceConstants.ROTATION_DECELERATION;
                            m_rotatingSpeed = MathHelper.Max(m_rotatingSpeed, m_maxRotatingSpeedIdle);
                        }

                        if (m_rotatingSpeed > m_maxRotatingSpeedIdle)
                        {
                            StartMovingCue();
                        }
                        else
                        {
                            StopMovingCue();
                            if (!MySession.Is25DSector)
                            {
                                StartIdleCue();
                            }
                            else
                            {
                                StopAllSounds();

                                //  if (MyFakes.MW25D)
                                    CurrentState = MyDrillStateEnum.Deactivated;

                            }
                        }
                        break;
                    }
                case MyDrillStateEnum.Drilling:
                    {
                        m_rotatingSpeed += MyDrillDeviceConstants.ROTATION_ACCELERATION;
                        m_rotatingSpeed = MathHelper.Clamp(m_rotatingSpeed, 0, m_maxRotatingSpeedDrilling);
                        break;
                    }
                case MyDrillStateEnum.Deactivated:
                    {
                        m_rotatingSpeed -= MyDrillDeviceConstants.ROTATION_DECELERATION;
                        m_rotatingSpeed = MathHelper.Clamp(m_rotatingSpeed, 0, m_maxRotatingSpeedDrilling);
                        break;
                    }
            }
        }
    }
}
