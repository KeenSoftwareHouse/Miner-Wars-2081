using System.Diagnostics;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using MinerWarsMath;
    using Models;
    using Utils;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.AppCode.Game.Entities.Ships.AI;
    using MinerWars.AppCode.Game.Audio;
    

    class MySmokeBomb : MyAmmoBase, IUniversalLauncherShell
    {
        bool m_smokeFired;
        bool m_smokeEnded;
        protected MySoundCue? m_smokeCue;

        public void Init()
        {
            base.Init(MyModelsEnum.SmokeBomb, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.SmokeBombHud));

            if (!TimeToActivate.HasValue)
            {
                TimeToActivate = MySmokeBombConstants.TIME_TO_ACTIVATE;
            }

            m_smokeFired = false;
            m_smokeEnded = false;
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (m_elapsedMiliseconds > TimeToActivate && !m_smokeFired)
            {
                StartSmokeEffect();
                Physics.LinearVelocity = Vector3.Zero;
                Explode();

                m_smokeCue = MyAudio.AddCue2dOr3d(this, MySoundCuesEnum.WepBombSmartSmoke,
                    GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);
            }
            
            if (m_isExploded)
            {
                var effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_SmallGunShot);
                effect.WorldMatrix = this.WorldMatrix;
                effect.UserScale = 2f;
                MarkForClose();
            }

            if (m_smokeEnded)
            {
                MarkForClose();
            }
            else if (m_smokeFired)
            {
                // Aggro near bots
                MyDangerZones.Instance.NotifyArea(GetPosition(), 300, OwnerEntity);
            }
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();
            if (m_smokeCue != null && m_smokeCue.Value.IsPlaying) 
            {
                MyAudio.UpdateCuePosition(m_smokeCue, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);
            }
        }

        //  This mine collides with something after which it must explode
        public void StartSmokeEffect()
        {
            Debug.Assert(!m_smokeFired);
            {
                m_smokeFired = true;

                var effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.UniversalLauncher_SmokeBomb);
                effect.WorldMatrix = WorldMatrix;
                effect.OnDelete += delegate { m_smokeEnded = true; };
            }
        }

        public override void Close()
        {
            if ((m_smokeCue != null) && m_smokeCue.Value.IsPlaying)
            {
                m_smokeCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }

            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
