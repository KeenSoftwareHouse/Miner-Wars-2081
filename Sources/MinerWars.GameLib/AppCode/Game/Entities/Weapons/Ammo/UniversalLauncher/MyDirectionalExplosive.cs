using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using Explosions;
    using MinerWarsMath;
    using Models;
    using Utils;
    using Voxels;
    using KeenSoftwareHouse.Library.Memory;
    using MinerWars.AppCode.Game.Decals;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Game.Managers.Session;

    class MyDirectionalExplosive : MyAmmoBase, IUniversalLauncherShell
    {
        private MyEntity m_collidedEntity;

        public void Init()
        {
            Init(MyModelsEnum.DirectionalExplosive, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.DirectionalExplosiveHud));

            if (!TimeToActivate.HasValue)
            {
                TimeToActivate = MyDirectionalExplosiveConstants.TIME_TO_ACTIVATE;
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

        //  This explosive collides with something after which it must explode
        public override void Explode()
        {
            if (m_isExploded)
                return;

            base.Explode();
            MarkForClose();

            if (m_isExploded)
            {
                const int stepsCount = 5;
                Vector3 directionDelta = WorldMatrix.Forward * (7 * m_ammoProperties.ExplosionRadius / stepsCount);

                for (int i = 0; i < stepsCount; i++)
                {
                    MyExplosion newExplosion = MyExplosions.AddExplosion();
                    if (newExplosion != null)
                    {
                        Vector3 explosionPos = GetPosition() + directionDelta * i;
                        BoundingSphere boundingSphere = new BoundingSphere(explosionPos, m_ammoProperties.ExplosionRadius);
                        newExplosion.Start(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, MyExplosionTypeEnum.BOMB_EXPLOSION, boundingSphere, MyExplosionsConstants.EXPLOSION_LIFESPAN, CascadedExplosionLevel, ownerEntity: OwnerEntity, hitEntity: m_collidedEntity, playSound: i == 0 ? true : false);
                    }
                }
            }
        }

        protected override void OnContactStart(MyContactEventInfo contactInfo)
        {
            base.OnContactStart(contactInfo);

            m_collidedEntity = contactInfo.GetOtherEntity(this);

            if (this.OwnerEntity is MySmallShip && (MySmallShip)this.OwnerEntity == MySession.PlayerShip && m_collidedEntity is MyStaticAsteroid && !m_collidedEntity.IsDestructible)
            {
                HUD.MyHud.ShowIndestructableAsteroidNotification();
            }

            Explode();
        }

        public override void Close()
        {
            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
