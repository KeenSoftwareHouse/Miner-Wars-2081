using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using Explosions;
    using MinerWarsMath;
    using Models;
    using Utils;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.AppCode.Game.Render;

    class MyEMPBomb : MyAmmoBase, IUniversalLauncherShell
    {
        // Boxes are better for mines, because when they collide with environment, they start rotating. Sphere just slide.
        const bool MINE_COLLISION_PRIMITIVE_IS_SPHERE = false;

        public void Init()
        {
            Init(MyModelsEnum.SphereExplosive, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, 100, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.EMPBombHud));

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
            if (m_isExploded)
                return;

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
            if (m_isExploded)
                return;

            base.Explode();

            MarkForClose();

            if (m_isExploded)
            {
                MyExplosion newExplosion = MyExplosions.AddExplosion();
                if (newExplosion != null)
                {
                    newExplosion.Start(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, MyExplosionTypeEnum.EMP_EXPLOSION, new BoundingSphere(GetPosition(), m_ammoProperties.ExplosionRadius), MyExplosionsConstants.EXPLOSION_LIFESPAN, CascadedExplosionLevel, ownerEntity: OwnerEntity);
                }
            }
        }

        public override void Close()
        {
            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
