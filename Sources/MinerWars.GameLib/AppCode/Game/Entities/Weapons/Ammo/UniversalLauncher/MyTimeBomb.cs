using System;
using System.Diagnostics;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Audio;

using MinerWars.AppCode.Game.Render;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    internal class MyTimeBomb : MyAmmoBase, IUniversalLauncherShell
    {
        private static readonly StringBuilder m_ammoSpecialText = new StringBuilder();

        private static readonly object[] m_timeBombArgs = new object[1];
        private static readonly StringBuilder m_timeBombHUDBuilder = new StringBuilder();
        protected MySoundCue? m_timingCue;

        static MyTimeBomb()
        {
        }

        public void Init()
        {
            Init(MyModelsEnum.TimeBomb, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            m_elapsedMiliseconds = 0;
            if (!TimeToActivate.HasValue)
            {
                //TimeToActivate = MyTimeBombConstants.TIMEOUT_ARRAY[m_ammoTimeoutIndex] * 1000;

                var ownerShip = owner as MySmallShip;
                Debug.Assert(ownerShip != null);
                if (ownerShip != null)
                {
                    TimeToActivate = ownerShip.Config.TimeBombTimer.CurrentValue * 1000;
                }
            }

            m_timingCue = MyAudio.AddCue2dOr3d(this, MySoundCuesEnum.WepBombSmartTimer,
            GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);


            FormatName();

            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, m_timeBombHUDBuilder);
        }

        private void FormatName()
        {
            m_timeBombArgs[0] = String.Format("{0:0.0}", (TimeToActivate - m_elapsedMiliseconds) / 1000f);
            var timeBombDisplay = MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.TimeBombHud, m_timeBombArgs);
            m_timeBombHUDBuilder.Clear();
            m_timeBombHUDBuilder.Append(timeBombDisplay);
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            FormatName();
            DisplayName = m_timeBombHUDBuilder.ToString();

            if (m_elapsedMiliseconds >= TimeToActivate)
            {
                Explode();
                TimeToActivate = null;
            }

            if (m_isExploded)
            {
                MarkForClose();
                MyExplosion newExplosion = MyExplosions.AddExplosion();
                newExplosion.Start(m_ammoProperties.HealthDamage, m_ammoProperties.ShipDamage, m_ammoProperties.EMPDamage, MyExplosionTypeEnum.BOMB_EXPLOSION,
                                   new BoundingSphere(GetPosition(), m_ammoProperties.ExplosionRadius),
                                   MyExplosionsConstants.EXPLOSION_LIFESPAN, CascadedExplosionLevel, ownerEntity: OwnerEntity);
            }                                                                                                      
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;
            return true;
        }

        private static void UpdateAmmoSpecialText()
        {
            m_ammoSpecialText.Clear();

            if (!MySession.PlayerShip.HasFiredRemoteBombs())
            {
                m_ammoSpecialText.AppendFormat(MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.TimeBombHelp), MySession.PlayerShip.Config.TimeBombTimer.CurrentValue, MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.WEAPON_SPECIAL));
            }
        }

        public static StringBuilder GetAmmoSpecialText()
        {
            UpdateAmmoSpecialText();
            return m_ammoSpecialText;
        }

        public override void Close()
        {
            if ((m_timingCue != null) && m_timingCue.Value.IsPlaying)
            {
                m_timingCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }

            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
