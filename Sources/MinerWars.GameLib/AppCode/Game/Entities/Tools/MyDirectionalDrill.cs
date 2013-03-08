using MinerWarsMath;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Entities
{
    abstract class MyDirectionalDrill : MyDrillBase
    {
        protected MyParticleEffect m_directionalEffect;
        protected MyParticleEffectsIDEnum m_directionalEffectID;

        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (!base.Shot(usedAmmo))
                return false;

            StartMovingCue();

            return true;
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation() ;

            if (CurrentState == MyDrillStateEnum.Drilling)
            {
                if (m_directionalEffect == null)
                {
                    m_directionalEffect = MyParticlesManager.CreateParticleEffect((int) m_directionalEffectID);
                }
                m_directionalEffect.WorldMatrix = Matrix.CreateWorld(
                    m_positionMuzzleInWorldSpace + MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE * WorldMatrix.Forward, WorldMatrix.Forward, WorldMatrix.Up);
            }
            else
            {
                if (m_directionalEffect != null)
                {
                    m_directionalEffect.Stop();
                    m_directionalEffect = null;
                }
                StopDustEffect();
            }
        }

        protected override void Drill()
        {
            //  Check for collision with drill and world
            MyLine line = new MyLine(m_positionMuzzleInWorldSpace, m_positionMuzzleInWorldSpace + 50 * WorldMatrix.Forward, true);
            //m_positionMuzzleInWorldSpace

            MyIntersectionResultLineTriangleEx? intersection = MyEntities.GetIntersectionWithLine(ref line, Parent, null);
            if (intersection != null && intersection.Value.Entity.Physics != null)
            {
                bool drillUsedForDestructibleContent = false;
                MyVoxelMap voxelMap = intersection.Value.Entity as MyVoxelMap;
                StartDrillingCue(voxelMap != null);

                if (voxelMap != null)
                {
                    ((MySmallShip)Parent).IncreaseHeadShake(MyDrillDeviceConstants.SHAKE_DURING_IN_VOXELS);

                    //  We found voxel so lets make tunel into it
                    BoundingSphere bigSphereForTunnel = new BoundingSphere(GetPosition() + 10 * WorldMatrix.Forward, m_radius);

                    for (int i = 0; i < (int)m_range; i++)
                    {
                        bigSphereForTunnel.Center = GetPosition() + (10 + i) * WorldMatrix.Forward;
                        bigSphereForTunnel.Radius = MyMwcUtils.GetRandomFloat(1, MyDrillDeviceConstants.MAX_RADIUS_RANDOM_MULTIPLIER) * m_radius;
                            
                        MyMwcVector3Int exactCenterOfDrilling = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(bigSphereForTunnel.Center.X, bigSphereForTunnel.Center.Y, bigSphereForTunnel.Center.Z));
                            
                        // we don't want drill indestructible voxels
                        if (voxelMap.GetVoxelMaterialIndestructibleContent(ref exactCenterOfDrilling) > MyVoxelConstants.VOXEL_CONTENT_EMPTY)                            
                        {                                                                    
                            break;
                        }

                        CutOutFromVoxel(voxelMap, ref bigSphereForTunnel);
                        
                        drillUsedForDestructibleContent = true;                            
                    }

                    if (drillUsedForDestructibleContent)
                    {
                        if (m_dustEffect == null)
                        {
                            m_dustEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_DrillDust);
                        }
                        m_dustEffect.WorldMatrix = Matrix.CreateTranslation(intersection.Value.IntersectionPointInWorldSpace);
                    }
                }
                else
                {
                    StopDustEffect();

                    CreateImpactEffect(intersection.Value.IntersectionPointInWorldSpace, intersection.Value.NormalInWorldSpace, MyParticleEffectsIDEnum.MaterialHit_Autocannon_Metal);

                    intersection.Value.Entity.DoDamage(0, m_damage * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS, 0, MyDamageType.Drill, MyAmmoType.Piercing, Parent);
                }

                if (!drillUsedForDestructibleContent)                    
                {
                    ((MySmallShip)Parent).IncreaseHeadShake(MyDrillDeviceConstants.SHAKE_DURING_ROTATION);
                    StopDustEffect();

                    if (intersection.Value.Entity != null && !intersection.Value.Entity.IsDestructible)
                    {
                        HUD.MyHud.ShowIndestructableAsteroidNotification();                        
                    }
                }
            }
            else
            {
                StopDrillingCue();
                StopDustEffect();
            }
        }

        public override void Close()
        {
            base.Close();

            if (m_directionalEffect != null)
            {
                m_directionalEffect.Stop();
                m_directionalEffect = null;
            }
        }
    }
}
