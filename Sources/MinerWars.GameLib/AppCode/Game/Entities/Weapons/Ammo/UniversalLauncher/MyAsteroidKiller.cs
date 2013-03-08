using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Explosions;
using MinerWarsMath;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    class MyAsteroidKiller : MyAmmoBase, IUniversalLauncherShell
    {
        private MyEntity m_collidedEntity;

        public void Init()
        {
            base.Init(MyModelsEnum.AsteroidKiller, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.AsteroidKillerHud));
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
                    newExplosion.Start(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, MyExplosionTypeEnum.BOMB_EXPLOSION, new BoundingSphere(GetPosition(), m_ammoProperties.ExplosionRadius), MyExplosionsConstants.EXPLOSION_LIFESPAN, CascadedExplosionLevel, ownerEntity: OwnerEntity, hitEntity: m_collidedEntity);
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
