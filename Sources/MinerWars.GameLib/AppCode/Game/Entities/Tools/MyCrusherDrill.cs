namespace MinerWars.AppCode.Game.Entities
{
    using System;
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.AppCode.Game.Decals;
    using MinerWarsMath;
    using Models;
    using TransparentGeometry;
    using TransparentGeometry.Particles;
    using SubObjects;
    using Utils;
    using Voxels;
    using KeenSoftwareHouse.Library.Memory;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;

    class MyCrusherDrill : MyContactDrill
    {
        private const int DRILL_BIT_COUNT = 9;

        MyDrillBit[] m_drillBits;
        Matrix m_rotationMatrix = Matrix.Identity;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            m_model = MyModelsEnum.Drill_Base;
            m_movingCueEnum = MySoundCuesEnum.VehToolCrusherDrillLoop3d;
            m_movingCueReleaseEnum = MySoundCuesEnum.VehToolCrusherDrillRelease3d;
            m_drillCueEnum = MySoundCuesEnum.VehToolCrusherDrillColliding3d;
            m_drillOtherCueEnum = MySoundCuesEnum.VehToolCrusherDrillCollidingOther3d;
            m_drillOtherCueReleaseEnum = MySoundCuesEnum.VehToolCrusherDrillCollidingOtherRelease3d;
            m_drillCueReleaseEnum = MySoundCuesEnum.VehToolCrusherDrillCollidingRelease3d;
            m_idleCueEnum = MySoundCuesEnum.VehToolCrusherDrillIdle3d;

            base.Init(hudLabelText, parentObject, position, forwardVector, upVector, objectBuilder);

            m_minDrillingDuration = MyCrusherDrillDeviceConstants.MIN_DRILLING_DURATION;
            m_range = MyCrusherDrillDeviceConstants.RANGE;
            m_radius = MyCrusherDrillDeviceConstants.RADIUS;
            m_damage = MyCrusherDrillDeviceConstants.DAMAGE_PER_SECOND;
            m_maxRotatingSpeedDrilling = MyCrusherDrillDeviceConstants.MAX_ROTATING_SPEED_DRILLING;
            m_maxRotatingSpeedIdle = MyCrusherDrillDeviceConstants.MAX_ROTATING_SPEED_IDLE;

            //Create Drill Bits
            m_drillBits = new MyDrillBit[DRILL_BIT_COUNT];
            for (int i = 0; i < DRILL_BIT_COUNT; i++)
            {
                m_drillBits[i] = new MyDrillBit();
            }

            //Init Drill Bits
            Vector3 gear1Offset = new Vector3(0, 0, 0.05f);
            Vector3 gear2Offset = new Vector3(0, 0, 0.15f);
            Vector3 gear3Offset = new Vector3(0, 0, 0.30f);

            const float FIRST_ROTATE_SPEED = 0.2f * MathHelper.TwoPi * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            const float SECOND_ROTATE_SPEED = 0.5f * -FIRST_ROTATE_SPEED;
            const float THIRD_ROTATE_SPEED = 1.25f * FIRST_ROTATE_SPEED;

            Matrix bit01 = MyMath.NormalizeMatrix(ModelLod0.Dummies["BIT01"].Matrix);
            Matrix bit02 = MyMath.NormalizeMatrix(ModelLod0.Dummies["BIT02"].Matrix);
            Matrix bit03 = MyMath.NormalizeMatrix(ModelLod0.Dummies["BIT03"].Matrix);

            m_drillBits[0].Init(null, bit01, gear1Offset, FIRST_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear1);
            m_drillBits[1].Init(null, bit01, gear2Offset, SECOND_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear2);
            m_drillBits[2].Init(null, bit01, gear3Offset, THIRD_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear3);
            m_drillBits[3].Init(null, bit02, gear1Offset, FIRST_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear1);
            m_drillBits[4].Init(null, bit02, gear2Offset, SECOND_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear2);
            m_drillBits[5].Init(null, bit02, gear3Offset, THIRD_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear3);
            m_drillBits[6].Init(null, bit03, gear1Offset, FIRST_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear1);
            m_drillBits[7].Init(null, bit03, gear2Offset, SECOND_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear2);
            m_drillBits[8].Init(null, bit03, gear3Offset, THIRD_ROTATE_SPEED, this, MyModelsEnum.Drill_Gear3);
        }

        public override void DebugDrawNormalVectors()
        {
            if (IsVisible())
            {
                base.DebugDrawNormalVectors();

                if (m_drillBits == null)
                    return;

                for (int i = 0; i < DRILL_BIT_COUNT; i++)
                {
                    m_drillBits[i].DebugDrawNormalVectors();
                }
            }
        }

        public override void UpdateBeforeSimulation()
        {
            if (Visible && Activated)
            {
                base.UpdateBeforeSimulation();
            }
        }

        public override void UpdateAfterSimulation()
        {
            //  Change world matrix by rotation (order of matrix multiplications is important!!!)
            var rotationMatrix = Matrix.CreateRotationZ(m_rotatingSpeed * MyDrillDeviceConstants.ROTATION_SPEED_PER_SECOND * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
            LocalMatrix = rotationMatrix * LocalMatrix;
            m_rotationMatrix = rotationMatrix * m_rotationMatrix;

            base.UpdateAfterSimulation();

            if (CurrentState == MyDrillStateEnum.InsideShip)
                return;

            MyAudio.UpdateCueRotationSpeed(m_drillCue, ((m_rotatingSpeed - m_maxRotatingSpeedIdle) / (m_maxRotatingSpeedDrilling - m_maxRotatingSpeedIdle) * 100));
            MyAudio.UpdateCueRotationSpeed(m_movingCue, ((m_rotatingSpeed - m_maxRotatingSpeedIdle) / (m_maxRotatingSpeedDrilling - m_maxRotatingSpeedIdle) * 100));
            MyAudio.UpdateCueRotationSpeed(m_movingCueRelease, ((m_rotatingSpeed - m_maxRotatingSpeedIdle) / (m_maxRotatingSpeedDrilling - m_maxRotatingSpeedIdle) * 100));
            MyAudio.UpdateCueRotationSpeed(m_idleCue, ((m_rotatingSpeed - m_maxRotatingSpeedIdle) / (m_maxRotatingSpeedDrilling - m_maxRotatingSpeedIdle) * 100));

            if (m_drillBits != null)
            {
                for (int i = 0; i < DRILL_BIT_COUNT; i++)
                {
                    m_drillBits[i].SetWorldMatrixForCockpit(ref m_worldMatrixForRenderingFromCockpitView);
                }
            }
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(WeaponType = MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher);
            }
            return objectBuilder;
        }

        protected override Matrix GetLocalMatrixForCockpitView()
        {
            return m_rotationMatrix * base.GetLocalMatrixForCockpitView();
        }
    }
}