namespace MinerWars.AppCode.Game.Entities
{
    using System;
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWarsMath;
    using Models;
    using TransparentGeometry.Particles;
    using Utils;
    using CommonLIB.AppCode.ObjectBuilders;
    

    class MyPressureDrill : MyDrillBase
    {
        MySoundCue? m_drillChargingCue;
        MySoundCue? m_drillBlastCue;
        MySoundCue? m_pressureIdleCue;

        enum PressureState
        {
            Init,
            Charging,
            Ready,
            Pushed
        }

        PressureState m_state = PressureState.Pushed;

        private MyParticleEffect m_chargingPressureEffect;
        private MyParticleEffect m_pressureFireEffect;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            m_model = MyModelsEnum.PressureDrill;
            m_movingCueEnum = null;
            m_movingCueReleaseEnum = null;
            m_drillCueEnum = null;
            m_drillOtherCueEnum = null;
            m_drillCueReleaseEnum = null;
            m_drillOtherCueReleaseEnum = null;

            m_radius = MyPressureDrillDeviceConstants.RADIUS;
            m_damage = MyPressureDrillDeviceConstants.DAMAGE;

            base.Init(hudLabelText, parentObject, position, forwardVector, upVector, objectBuilder);
        }

        //  Every child of this base class must implement Shot() method, which shots projectile or missile.
        //  Method returns true if something was shot. False if not (because interval between two shots didn't pass)
        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeStarted) < MyPressureDrillDeviceConstants.SHOT_INTERVAL_IN_MILISECONDS)
                return false;

            if (!base.Shot(usedAmmo))
                return false;

            m_state = PressureState.Init;

            return true;
        }

        public override bool Eject()
        {
            switch (m_state)
            {
                case PressureState.Init:
                case PressureState.Ready:
                case PressureState.Pushed:
                    return base.Eject();
                case PressureState.Charging:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartPressureIdleCue()
        {
            if (m_pressureIdleCue == null || m_pressureIdleCue.Value.IsPlaying == false)
            {
                m_pressureIdleCue = MyAudio.AddCue2dOr3d(Parent, MySoundCuesEnum.VehToolPressureDrillIdle3d, WorldMatrix.Translation,
                    WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
            }
        }

        private void StopPressureIdleCue()
        {
            if (m_pressureIdleCue != null &&  m_pressureIdleCue.Value.IsPlaying)
            {
                m_pressureIdleCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }
        }


        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            var muzzlePosition = m_positionMuzzleInWorldSpace;
            muzzlePosition += NearFlag
                                     ? MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE * WorldMatrix.Forward
                                     : .5f * WorldMatrix.Forward;
            if (m_state == PressureState.Pushed && CurrentState == MyDrillStateEnum.Activated)
            {
                StartPressureIdleCue();
            }
            else
            {
                StopPressureIdleCue();
            }

            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeStarted) >= MyPressureDrillDeviceConstants.SHOT_INTERVAL_IN_MILISECONDS)
            {
                StopDrillingCue();
                StopMovingCue();
                m_state = PressureState.Pushed;

                return;
            }

            if (m_pressureFireEffect != null && !m_pressureFireEffect.IsStopped)
            {
                m_pressureFireEffect.WorldMatrix = Matrix.CreateWorld(
                    muzzlePosition + 1.5f * WorldMatrix.Forward,
                    WorldMatrix.Forward,
                    WorldMatrix.Up);
            }

            switch (m_state)
            {
                case PressureState.Init:
                    StartDrillChargingCue();
                    m_state = PressureState.Charging;

                    m_chargingPressureEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Drill_Pressure_Charge);
                    m_chargingPressureEffect.WorldMatrix = Matrix.CreateWorld(muzzlePosition, WorldMatrix.Forward, WorldMatrix.Up);
                    break;

                case PressureState.Charging:
                    {
                        ((MySmallShip)Parent).IncreaseHeadShake(MyDrillDeviceConstants.SHAKE_DURING_ROTATION);

                        if (m_chargingPressureEffect != null)
                        {
                            m_chargingPressureEffect.WorldMatrix = Matrix.CreateWorld(muzzlePosition, WorldMatrix.Forward, WorldMatrix.Up);
                        }

                        if (m_drillChargingCue == null || !m_drillChargingCue.Value.IsPlaying)
                            m_state = PressureState.Ready;
                    }
                    break;

                case PressureState.Ready:
                    {
                        
                        m_chargingPressureEffect.Stop();
                        m_chargingPressureEffect = null;

                        m_pressureFireEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Drill_Pressure_Fire);
                        m_pressureFireEffect.OnDelete += OnPressureFireEffectDelete;
                        m_pressureFireEffect.WorldMatrix = Matrix.CreateWorld(muzzlePosition, WorldMatrix.Forward, WorldMatrix.Up);

                        //  Check for collision with drill and world
                        MyLine line = new MyLine(GetPosition(), GetPosition() + 2 * m_radius * WorldMatrix.Forward, true);

                        MyIntersectionResultLineTriangleEx? intersection = MyEntities.GetIntersectionWithLine(ref line, Parent, null);

                        if (intersection != null && intersection.Value.Entity.Physics != null)
                        {
                            var voxelMap = intersection.Value.Entity as MyVoxelMap;

                            StartDrillingCue(voxelMap != null);

                            if (voxelMap != null)
                            {

                                ((MySmallShip)Parent).IncreaseHeadShake(MyDrillDeviceConstants.SHAKE_DURING_IN_VOXELS);

                                //  We found voxel so lets make tunel into it
                                BoundingSphere bigSphereForTunnel = new BoundingSphere(GetPosition() + 20 * WorldMatrix.Forward, m_radius);

                                for (int i = 1; i < (int)m_radius; i++)
                                {
                                    bigSphereForTunnel.Center = GetPosition() + (20 + i) * WorldMatrix.Forward;
                                    bigSphereForTunnel.Radius = i * 2;

                                    CutOutFromVoxel(voxelMap, ref bigSphereForTunnel);
                                }

                                CreateImpactEffect(intersection.Value.IntersectionPointInWorldSpace, intersection.Value.NormalInWorldSpace, MyParticleEffectsIDEnum.Drill_Pressure_Impact);
                                StartDrillBlastRockCue();
                            }
                            //  Display particles when we are in contact with voxel
                            else
                            {
                                ((MySmallShip)Parent).IncreaseHeadShake(MyDrillDeviceConstants.SHAKE_DURING_ROTATION);

                                CreateImpactEffect(intersection.Value.IntersectionPointInWorldSpace, intersection.Value.NormalInWorldSpace, MyParticleEffectsIDEnum.Drill_Pressure_Impact_Metal);

                                intersection.Value.Entity.DoDamage(0, m_damage, 0, MyDamageType.Drill, MyAmmoType.Explosive, Parent);

                                StartDrillBlastOtherCue();
                            }

                        }
                        else
                        {
                            StartDrillBlastCue();
                        }
                        m_state = PressureState.Pushed;
                    }
                    break;

                case PressureState.Pushed:
                    {
                        StopDrillingBlastCue();
                        StopMovingCue();
                    }
                    break;
            }
        }

        private void OnPressureFireEffectDelete(object sender, EventArgs e)
        {
            m_pressureFireEffect = null;
        }

        protected override void Drill() { }

        public override void Close()
        {
            base.Close();

            if (m_chargingPressureEffect != null)
            {
                m_chargingPressureEffect.Stop();
                m_chargingPressureEffect = null;
            }
            if (m_pressureFireEffect != null)
            {
                m_pressureFireEffect.Stop();
                m_pressureFireEffect = null;
            }
            if (m_drillChargingCue != null)
            {
                m_drillChargingCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                m_drillChargingCue = null;
            }
        }

        void StartDrillChargingCue()
        {
            if ((m_drillChargingCue == null) || (m_drillChargingCue.Value.IsPlaying == false))
            {
                m_drillChargingCue = MyAudio.AddCue2dOr3d(Parent, MySoundCuesEnum.VehToolPressureDrillRecharge3d, WorldMatrix.Translation,
                    WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
            }
        }

        void StartDrillBlastCue()
        {
            if ((m_drillBlastCue == null) || (m_drillBlastCue.Value.IsPlaying == false))
            {
                m_drillBlastCue = MyAudio.AddCue2dOr3d(Parent, MySoundCuesEnum.VehToolPressureDrillBlast3d, WorldMatrix.Translation,
                WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
            }
        }

        void StartDrillBlastRockCue()
        {
            if ((m_drillBlastCue == null) || (m_drillBlastCue.Value.IsPlaying == false))
            {
                m_drillBlastCue = MyAudio.AddCue2dOr3d(Parent, MySoundCuesEnum.VehToolPressureDrillBlastRock3d, WorldMatrix.Translation,
                WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
            }
        }

        void StartDrillBlastOtherCue()
        {
            if ((m_drillBlastCue == null) || (m_drillBlastCue.Value.IsPlaying == false))
            {
                m_drillBlastCue = MyAudio.AddCue2dOr3d(Parent, MySoundCuesEnum.VehToolPressureDrillBlastOther3d, WorldMatrix.Translation,
                WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
            }
        }

        void StopDrillingBlastCue()
        {
            if ((m_drillBlastCue != null) && m_drillBlastCue.Value.IsPlaying)
            {
                //m_drillBlastCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }
        }



        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure);
            }
            return objectBuilder;
        }
    }
}