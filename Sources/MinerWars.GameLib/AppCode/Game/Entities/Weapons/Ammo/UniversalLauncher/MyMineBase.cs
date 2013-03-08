using System.Text;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Explosions;
using MinerWarsMath;
using MinerWars.AppCode.Game.Models;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    abstract class MyMineBase : MyAmmoBase, IUniversalLauncherShell
    {
        protected MyCurrentState m_state;
        private MyEntity m_collidedEntity;
        protected MyExplosionTypeEnum m_explosionType = MyExplosionTypeEnum.MISSILE_EXPLOSION;

        public enum MyCurrentState
        {
            //  Mine was fired and it is moving to its destination point - few meters in front of its starting position
            MOVING_TO_DESTINATION_POINT,
            //  Mine reached its destination point and now is waiting for collision to explode
            ACTIVATED,
        }

        public abstract void Init();

        public abstract void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner);

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public override void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner, StringBuilder displayName)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, displayName);
            m_state = MyCurrentState.MOVING_TO_DESTINATION_POINT;
            m_collidedEntity = null;

            Faction = owner.Faction;
            Physics.LinearDamping = 0.5f;

            var ownerSmallShip = owner as MySmallShip;
            if (ownerSmallShip != null)
                Physics.GroupMask = ownerSmallShip.GroupMask;
        }

        public override void UpdateBeforeSimulation()
        {
            if (m_isExploded)
            {
                MarkForClose();
                MyExplosion newExplosion = MyExplosions.AddExplosion();
                if (newExplosion != null)
                {
                    float radius = m_ammoProperties.ExplosionRadius;
                    newExplosion.Start(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, m_explosionType, new BoundingSphere(GetPosition(), radius), MyExplosionsConstants.EXPLOSION_LIFESPAN, CascadedExplosionLevel, hitEntity:m_collidedEntity, ownerEntity: OwnerEntity);
                }
                return;
            }
            base.UpdateBeforeSimulation();

            if (m_elapsedMiliseconds > MyRemoteCameraConstants.TIME_TO_ACTIVATE_GROUP_MASK && m_state == MyCurrentState.MOVING_TO_DESTINATION_POINT)
            {
                m_state = MyCurrentState.ACTIVATED;
                Physics.GroupMask = MyGroupMask.Empty;
            }
        }

        protected override void OnContactStart(MyContactEventInfo contactInfo)
        {
            base.OnContactStart(contactInfo);

            if (m_state == MyCurrentState.ACTIVATED)
            {
                m_collidedEntity = contactInfo.GetOtherEntity(this);

                if (m_collidedEntity is MySmallShip && MyFactions.GetFactionsRelation(this, m_collidedEntity) == MyFactionRelationEnum.Enemy)
                {
                    if (this.OwnerEntity is MySmallShip && (MySmallShip)this.OwnerEntity == MySession.PlayerShip && m_collidedEntity is MyStaticAsteroid && !m_collidedEntity.IsDestructible)
                    {
                        HUD.MyHud.ShowIndestructableAsteroidNotification();
                    }

                    //  Create explosion and close;
                    Explode();
                }
            }
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            base.DoDamageInternal(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);

            Explode();
        }

        public override void Close()
        {
            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
