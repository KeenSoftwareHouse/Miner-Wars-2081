using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Render;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    internal class MyRemoteBomb : MyAmmoBase, IUniversalLauncherShell
    {
        private static readonly StringBuilder m_ammoSpecialText = new StringBuilder();

        public void Init()
        {
            Init(MyModelsEnum.RemoteBomb, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.RemoteBombHud));

            var ownerShip = owner as MySmallShip;
            if (ownerShip != null)
            {
                ownerShip.AddRemoteBomb(this);
            }

            if (!TimeToActivate.HasValue)
            {
                TimeToActivate = MyRemoteBombConstants.MAXIMUM_LIVING_TIME;
            }
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;
            return true;
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (m_elapsedMiliseconds >= TimeToActivate)
            {
                Explode();
            }

            if (m_isExploded)
            {
                MyExplosion newExplosion = MyExplosions.AddExplosion();
                if (newExplosion != null)
                {
                    newExplosion.Start(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, MyExplosionTypeEnum.BOMB_EXPLOSION,
                                       new BoundingSphere(GetPosition(), m_ammoProperties.ExplosionRadius),
                                       MyExplosionsConstants.EXPLOSION_LIFESPAN, CascadedExplosionLevel, ownerEntity: OwnerEntity);
                }
                MarkForClose();
            }
        }

        public static StringBuilder GetAmmoSpecialText()
        {
            m_ammoSpecialText.Clear();

            var remoteBombCount = MySession.PlayerShip.RemoteBombCount;
            if (remoteBombCount > 0)
            {
                m_ammoSpecialText.AppendFormat(MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.RemoteBombHelp), MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.WEAPON_SPECIAL), remoteBombCount);
            }
            return m_ammoSpecialText;
        }

        public override void Close()
        {
            var ownerShip = OwnerEntity as MySmallShip;
            if (ownerShip != null)
            {
                ownerShip.RemoveRemoteBomb(this);
            }

            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
