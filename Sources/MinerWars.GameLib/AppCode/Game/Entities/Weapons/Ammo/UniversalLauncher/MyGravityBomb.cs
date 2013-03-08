using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using Explosions;
    using MinerWarsMath;
    using Models;
    using Utils;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.AppCode.Game.Render;

    class MyGravityBomb : MyAmmoBase, IUniversalLauncherShell
    {
        public void Init()
        {
            Init(MyModelsEnum.GravityBomb, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.GravityBombHud));

            if (!TimeToActivate.HasValue)
            {
                TimeToActivate = MySphereExplosiveConstants.TIME_TO_ACTIVATE;
            }
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (m_elapsedMiliseconds > TimeToActivate)
            {
                Explode();
            }
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;
            return true;
        }

        //  This mine collides with something after which it must explode
        public override void Explode()
        {
            MyExplosion newExplosion = MyExplosions.AddExplosion();
            if (newExplosion != null)
            {
                newExplosion.Start(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, MyExplosionTypeEnum.GRAVITY_EXPLOSION, new BoundingSphere(GetPosition(), m_ammoProperties.ExplosionRadius), MyExplosionsConstants.EXPLOSION_LIFESPAN, MyExplosionForceDirection.IMPLOSION, AppCode.Physics.MyGroupMask.Empty, true, CascadedExplosionLevel, ownerEntity: OwnerEntity);
            }
            MarkForClose();
        }

        public override void Close()
        {
            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
