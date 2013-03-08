using System;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.SubObjects;


namespace MinerWars.AppCode.Game.Entities
{
    using System.Text;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using MinerWarsMath;
    using Models;
    using TransparentGeometry.Particles;
    using Utils;
    using CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.CommonLIB.AppCode.Import;

    class MyThermalDrill : MyContactDrill
    {
        MyDrillHead m_drillHead;

        MyParticleEffect m_trailEffect;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            m_model = MyModelsEnum.ThermalDrill;
            m_movingCueEnum = MySoundCuesEnum.VehToolThermalDrillLoop3d;
            m_movingCueReleaseEnum = MySoundCuesEnum.VehToolThermalDrillRelease3d;
            m_drillCueEnum = MySoundCuesEnum.VehToolThermalDrillColliding3d;
            m_drillOtherCueEnum = MySoundCuesEnum.VehToolThermalDrillCollidingOther3d;
            m_drillOtherCueReleaseEnum = MySoundCuesEnum.VehToolThermalDrillCollidingOtherRelease3d;
            m_drillCueReleaseEnum = MySoundCuesEnum.VehToolThermalDrillCollidingRelease3d;
            m_idleCueEnum = MySoundCuesEnum.VehToolThermalDrillIdle3d;

            base.Init(hudLabelText, parentObject, position, forwardVector, upVector, objectBuilder);

            m_minDrillingDuration = MyThermalDrillDeviceConstants.MIN_DRILLING_DURATION;
            m_range = MyThermalDrillDeviceConstants.RANGE;
            m_radius = MyThermalDrillDeviceConstants.RADIUS;
            m_damage = MyThermalDrillDeviceConstants.DAMAGE_PER_SECOND;
            m_maxRotatingSpeedDrilling = MyThermalDrillDeviceConstants.MAX_ROTATING_SPEED_DRILLING;
            m_maxRotatingSpeedIdle = MyThermalDrillDeviceConstants.MAX_ROTATING_SPEED_IDLE;

            // drill head:
            Matrix matrix = Matrix.Identity;
            MyModelDummy dummy;
            if (ModelLod0.Dummies.TryGetValue("head", out dummy))
                matrix = dummy.Matrix;
            else
                Debug.Assert(false, "Dummy 'head' in thermal drill model is missing.");

            m_drillHead = new MyDrillHead();
            m_drillHead.Init(matrix.Translation, this, MyModelsEnum.ThermalDrillHead);
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            if (CurrentState == MyDrillStateEnum.Drilling)
            {
                if (m_trailEffect == null)
                {
                    m_trailEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Drill_Thermal);

                    //Must be done with handler because we need to set world matrix until effect totally dies
                    m_trailEffect.OnDelete += OnTrailEffectDeleted;
                }
            }
            else
            {
                if (m_trailEffect != null)
                {
                    m_trailEffect.Stop();
                    m_trailEffect = null;
                }
            }

            if (m_trailEffect != null)
            {
                Matrix effectMatrix = Matrix.CreateWorld(m_positionMuzzleInWorldSpace + MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE * 2 * WorldMatrix.Forward, WorldMatrix.Forward, WorldMatrix.Up);
                m_trailEffect.WorldMatrix = effectMatrix;
            }

            if (m_drillHead != null)
            {
                m_drillHead.SetWorldMatrixForCockpit(ref m_worldMatrixForRenderingFromCockpitView);
                m_drillHead.RotationSpeed = -m_rotatingSpeed;
            }
        }

        private void OnTrailEffectDeleted(object sender, EventArgs e)
        {
            m_trailEffect = null;
        }

        public override void Close()
        {
            if (m_trailEffect != null)
            {
                m_trailEffect.Stop();
                m_trailEffect = null;
            }
            base.Close();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal);
            }
            return objectBuilder;
        }
    }
}