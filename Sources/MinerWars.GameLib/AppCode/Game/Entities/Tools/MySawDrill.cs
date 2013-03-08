namespace MinerWars.AppCode.Game.Entities
{
    using System.Text;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using MinerWarsMath;
    using Models;
    using Utils;
    using CommonLIB.AppCode.ObjectBuilders;

    class MySawDrill : MyDirectionalDrill
    {
        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            m_model = MyModelsEnum.SawDrill;
            m_movingCueEnum = MySoundCuesEnum.VehToolSawLoop3d;
            m_movingCueReleaseEnum = MySoundCuesEnum.VehToolSawRelease3d;
            m_drillCueEnum = MySoundCuesEnum.VehToolSawCut3d;
            m_drillOtherCueEnum = MySoundCuesEnum.VehToolSawCutOther3d;
            m_drillOtherCueReleaseEnum = MySoundCuesEnum.VehToolSawCutOtherRelease3d;
            m_drillCueReleaseEnum = MySoundCuesEnum.VehToolSawCutRelease3d;
            m_idleCueEnum = MySoundCuesEnum.VehToolSawIdle3d;

            base.Init(hudLabelText, parentObject, position, forwardVector, upVector, objectBuilder);

            m_minDrillingDuration = MySawDrillDeviceConstants.MIN_DRILLING_DURATION;
            m_range = MySawDrillDeviceConstants.RANGE;
            m_radius = MySawDrillDeviceConstants.RADIUS;
            m_damage = MySawDrillDeviceConstants.DAMAGE_PER_SECOND;
            m_directionalEffectID = MyParticleEffectsIDEnum.Drill_Saw;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {                
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw);                
            }
            return objectBuilder;
        }
    }
}