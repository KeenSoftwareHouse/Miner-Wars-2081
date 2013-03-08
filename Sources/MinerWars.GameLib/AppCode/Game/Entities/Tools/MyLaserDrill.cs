namespace MinerWars.AppCode.Game.Entities
{
    using System.Text;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using MinerWarsMath;
    using Models;
    using Utils;
    using CommonLIB.AppCode.ObjectBuilders;

    class MyLaserDrill : MyDirectionalDrill
    {
        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            m_model = MyModelsEnum.LaserDrill;
            m_movingCueEnum = MySoundCuesEnum.VehToolLaserDrillLoop3d;
            m_movingCueReleaseEnum = MySoundCuesEnum.VehToolLaserDrillRelease3d;
            m_drillCueEnum = MySoundCuesEnum.VehToolLaserDrillColliding3d;
            m_drillOtherCueEnum = MySoundCuesEnum.VehToolLaserCollidingOther3d;
            m_drillOtherCueReleaseEnum = MySoundCuesEnum.VehToolLaserCollidingOtherRelease3d;
            m_drillCueReleaseEnum = MySoundCuesEnum.VehToolLaserDrillCollidingRelease3d;
            m_idleCueEnum = MySoundCuesEnum.VehToolLaserDrillIdle3d;

            base.Init(hudLabelText, parentObject, position, forwardVector, upVector, objectBuilder);

            m_minDrillingDuration = MyLaserDrillDeviceConstants.MIN_DRILLING_DURATION;
            m_range = MyLaserDrillDeviceConstants.RANGE;
            m_radius = MyLaserDrillDeviceConstants.RADIUS;
            m_damage = MyLaserDrillDeviceConstants.DAMAGE_PER_SECOND;
            m_directionalEffectID = MyParticleEffectsIDEnum.Drill_Laser;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {                
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser);
            }
            return objectBuilder;
        }
    }
}