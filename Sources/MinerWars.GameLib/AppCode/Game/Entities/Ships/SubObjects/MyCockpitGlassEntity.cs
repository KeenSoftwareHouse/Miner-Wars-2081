namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    using System;
    using MinerWars.AppCode.Game.Utils;
    using MinerWars.AppCode.Physics;

    /// <summary>
    /// This phys object is not used for drawing. 
    /// It's whole purpose is only detect intersection between projectiles or dirt and cockpit glass.
    /// </summary>
    class MyCockpitGlassEntity : MyEntity
    {
        public MyCockpitGlassEntity()
        {
            IsDestructible = true;
        }

        public override void Init(System.Text.StringBuilder hudLabelText, Models.MyModelsEnum? modelLod0Enum, Models.MyModelsEnum? modelLod1Enum, MyEntity parentObject, float? scale, CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_Base objectBuilder, Models.MyModelsEnum? modelCollision = null, Models.MyModelsEnum? modelLod2 = null)
        {
            base.Init(hudLabelText, modelLod0Enum, modelLod1Enum, parentObject, scale, objectBuilder, modelCollision, modelLod2);

            //InitSpherePhysics(MyMaterialType.GLASS, ModelLod0, 1.0f, 0, MyConstants.COLLISION_LAYER_DEFAULT, RigidBodyFlag.RBF_DEFAULT);

            Visible = false;
            Save = false;
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            Parent.DoDamage(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
        }
    }
}
