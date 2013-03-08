using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using System.Collections.Generic;
    using Explosions;
    using GUI;
    using MinerWarsMath;
    using Models;
    using Renders;
    using Utils;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Game.TransparentGeometry;

    class MyFlashBomb : MyAmmoBase, IUniversalLauncherShell
    {
        static List<MyEntity> m_targetEntities = new List<MyEntity>(16);

        public void Init()
        {
            base.Init(MyModelsEnum.FlashBomb, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.FlashBombHud));

            if (!TimeToActivate.HasValue)
            {
                TimeToActivate = MyFlashBombConstants.TIME_TO_ACTIVATE;
            }
        }

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
                newExplosion.Start(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, MyExplosionTypeEnum.FLASH_EXPLOSION, new BoundingSphere(GetPosition(), m_ammoProperties.ExplosionRadius), MyExplosionsConstants.EXPLOSION_LIFESPAN, CascadedExplosionLevel, ownerEntity: OwnerEntity, particleScale: 0.001f);
            }

            bool isBotFlashBomb = OwnerEntity as MySmallShipBot != null;

            //If ship is another player, add flash to his view
            BoundingSphere flashSphere = new BoundingSphere(GetPosition(), MyFlashBombConstants.FLASH_RADIUS);
            m_targetEntities.Clear();
            MyEntities.GetIntersectionWithSphere(ref flashSphere, this, null, true, false, ref m_targetEntities);
            foreach (MyEntity ob in m_targetEntities)
            {
                if (ob is MySmallShip)
                {
                    if (MyEnemyTargeting.CanSee(this, ob) == null)
                    {
                        Vector3 bombPosition = GetPosition();
                        if (ob == MySession.PlayerShip)
                        {
                            if (MyCamera.IsInFrustum(ref bombPosition))
                            {
                                MyFlashes.MakeFlash();
                            }
                        }
                        else if (!isBotFlashBomb && ob is MySmallShipBot)  //If this flashbomb is from bot, dont flash bots
                        {
                            //If it is bot, make him panic
                            MySmallShipBot bot = ob as MySmallShipBot;
                            bot.Flash();
                        }
                    }
                }
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
