using System;
using System.Diagnostics;


namespace MinerWars.AppCode.Game.Entities
{
    using System.Text;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using MinerWarsMath;
    using Models;
    using Utils;
    using CommonLIB.AppCode.ObjectBuilders;
    using TransparentGeometry.Particles;
    using SubObjects;
    using MinerWars.AppCode.Game.GUI;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.CommonLIB.AppCode.Import;

    class MyNuclearDrill : MyContactDrill
    {
        MyDrillHead m_drillHead;

        MyParticleEffect m_nuclearEffect;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            m_model = MyModelsEnum.NuclearDrill;

            //We want nuclear drills only in 2.5D
            System.Diagnostics.Debug.Assert(MySession.Is25DSector);


            /*
            m_movingCueEnum = MySoundCuesEnum.VehToolNuclearDrillLoop3d;
            m_movingCueReleaseEnum = MySoundCuesEnum.VehToolNuclearDrillRelease3d;
            m_drillCueEnum = MySoundCuesEnum.VehToolNuclearDrillColliding3d;
            m_drillOtherCueEnum = MySoundCuesEnum.VehToolNuclearDrillCollidingOther3d;
            m_drillOtherCueReleaseEnum = MySoundCuesEnum.VehToolNuclearDrillCollidingOtherRelease3d;
            m_drillCueReleaseEnum = MySoundCuesEnum.VehToolNuclearDrillCollidingRelease3d;
            m_idleCueEnum = MySoundCuesEnum.VehToolNuclearDrillIdle3d;
              */

            m_movingCueEnum = MySoundCuesEnum.VehToolSawIdle3d;
            m_movingCueReleaseEnum = MySoundCuesEnum.VehToolLaserDrillRelease3d;
            m_drillCueEnum = MySoundCuesEnum.VehToolLaserDrillColliding3d;
            m_drillOtherCueEnum = MySoundCuesEnum.VehToolLaserDrillColliding3d;
            m_drillOtherCueReleaseEnum = MySoundCuesEnum.VehToolLaserDrillCollidingRelease3d;
            m_drillCueReleaseEnum = MySoundCuesEnum.VehToolLaserDrillCollidingRelease3d;
            m_idleCueEnum = MySoundCuesEnum.VehToolSawCut3d;

            base.Init(hudLabelText, parentObject, position, forwardVector, upVector, objectBuilder);

            m_minDrillingDuration = MyNuclearDrillDeviceConstants.MIN_DRILLING_DURATION;
            m_range = MyNuclearDrillDeviceConstants.RANGE;
            m_radius = MyNuclearDrillDeviceConstants.RADIUS;
            m_damage = MyNuclearDrillDeviceConstants.DAMAGE_PER_SECOND;
            m_maxRotatingSpeedDrilling = MyNuclearDrillDeviceConstants.MAX_ROTATING_SPEED_DRILLING;
            m_maxRotatingSpeedIdle = MyNuclearDrillDeviceConstants.MAX_ROTATING_SPEED_IDLE;

            // drill head:
            Matrix matrix = Matrix.Identity;
            MyModelDummy dummy;
            if (ModelLod0.Dummies.TryGetValue("head", out dummy))
                matrix = dummy.Matrix;
            else
                Debug.Assert(false, "Dummy 'head' in nuclear drill model is missing.");

            m_drillHead = new MyDrillHead();
            m_drillHead.Init(matrix.Translation, this, MyModelsEnum.NuclearDrillHead);
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            if (CurrentState == MyDrillStateEnum.Drilling)
            {
                if (m_nuclearEffect == null)
                {
                    m_nuclearEffect = MyParticlesManager.CreateParticleEffect((int) MyParticleEffectsIDEnum.Drill_Nuclear);

                    //Must be done with handler because we need to set world matrix until effect totally dies
                    m_nuclearEffect.OnDelete += OnNuclearEffectDeleted;
                }
            }
            else
            {
                if (m_nuclearEffect != null)
                {
                    m_nuclearEffect.Stop();
                    m_nuclearEffect.OnDelete -= OnNuclearEffectDeleted;
                    m_nuclearEffect = null;
                }
            }

            if (m_nuclearEffect != null)
            {
                Matrix effectMatrix = Matrix.CreateWorld(m_positionMuzzleInWorldSpace + MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE * 2 * WorldMatrix.Forward, WorldMatrix.Forward, WorldMatrix.Up);
                m_nuclearEffect.WorldMatrix = effectMatrix;
            }

            if (m_drillHead != null)
            {
                m_drillHead.SetWorldMatrixForCockpit(ref m_worldMatrixForRenderingFromCockpitView);
                m_drillHead.RotationSpeed = m_rotatingSpeed;
            }
        }

        private void OnNuclearEffectDeleted(object sender, EventArgs e)
        {
            m_nuclearEffect = null;
        }

        public override void Close()
        {
            base.Close();

            if (m_nuclearEffect != null)
            {
                m_nuclearEffect.Stop();
                m_nuclearEffect = null;
            }
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear);
            }
            return objectBuilder;
        }

        protected override float GetRadiusNeededForTunel()
        {
            return (Vector3.Distance(Parent.ModelLod0.BoundingSphere.Center, this.LocalMatrix.Translation) +
                ModelLod0.BoundingSphere.Radius) * (MySession.Is25DSector ? MyDrillDeviceConstants.BIG_SPHERE_RADIUS_MULTIPLIER * 1.8f : MyDrillDeviceConstants.BIG_SPHERE_RADIUS_MULTIPLIER);
        }
    }
}