using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using System;
    using System.Collections.Generic;
    using App;
    using CommonLIB.AppCode.Utils;
    using MinerWarsMath;
    using Models;
    using Utils;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using Ships.AI;
    using MinerWars.AppCode.Game.Audio;
    

    class MyDecoyFlare : MyAmmoBase, IUniversalLauncherShell
    {
        class FlareParticle
        {
            public Vector3 Position;
            public Vector3 Dir;
            public float FlyTime;
            public float Life;
            public MyParticleEffect ParticleEffect;
        }

        float m_startParticleTime;

        readonly List<FlareParticle> m_particles = new List<FlareParticle>(MyDecoyFlareConstants.FLARES_COUNT);

        bool m_flaresFired;

        protected MySoundCue? m_flareLoop;
        protected MySoundCue? m_flareDeploy;

        public void Init()
        {
            base.Init(MyModelsEnum.DecoyFlare, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);

            InitParticles();
        }

        private void InitParticles()
        {
            m_particles.Clear();
            for (int i = 0; i < MyDecoyFlareConstants.FLARES_COUNT; i++)
            {
                m_particles.Add(new FlareParticle());
            }

            m_flaresFired = false;
        }

        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.DecoyFlareHud));

            if (!TimeToActivate.HasValue)
            {
                TimeToActivate = MyDecoyFlareConstants.TIME_TO_ACTIVATE;
            }
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            bool decoyLiving = true;

            if (m_elapsedMiliseconds > TimeToActivate)
            {
                decoyLiving = UpdateParticles();
                this.Physics.LinearVelocity = Vector3.Zero;

                if (this.Visible)
                {
                    this.Visible = false;
                    m_flareLoop = MyAudio.AddCue2dOr3d(this, MySoundCuesEnum.SfxFlareLoop01,
                    GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);
                }
            }

            if (!decoyLiving)
            {
                MarkForClose();
            }
            else
            {
                // Aggro near bots
                MyDangerZones.Instance.NotifyArea(GetPosition(), 300, OwnerEntity);

                if (m_flareLoop != null && m_flareLoop.Value.IsPlaying)
                {
                    MyAudio.UpdateCuePosition(m_flareLoop, WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);
                }
            }

        }

        //  This mine collides with something after which it must explode
        bool UpdateParticles()
        {
            if (!m_flaresFired)
            {
                FireFlares();
            }

            bool decoyLiving = false;

            foreach (FlareParticle particle in m_particles)
            {
                if (MyMinerGame.TotalGamePlayTimeInMilliseconds < particle.FlyTime + m_startParticleTime)
                {
                    UpdateFlareParticlePosition(particle);
                }

                MyParticleEffect effect = particle.ParticleEffect;
                if (effect != null)
                {
                    effect.WorldMatrix = Matrix.CreateTranslation(particle.Position);

                    if (MyMinerGame.TotalGamePlayTimeInMilliseconds + 3000 > particle.Life + m_startParticleTime)
                    {
                        effect.Stop();
                        particle.ParticleEffect = null;
                    }
                }

                //Detect decoy alive
                if (MyMinerGame.TotalGamePlayTimeInMilliseconds + 3000 < particle.Life + m_startParticleTime)
                {
                    decoyLiving = true;
                }
            }

            return decoyLiving;
        }

        /// <summary>
        /// Initialize flares to fire
        /// </summary>
        private void FireFlares()
        {
            m_flareDeploy = MyAudio.AddCue2dOr3d(this, MySoundCuesEnum.SfxFlareDeploy,
            GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Physics.LinearVelocity);

            MyAudio.UpdateCuePosition(m_flareDeploy, GetPosition(), GetForward(), GetUp(), Physics.LinearVelocity);

            foreach (FlareParticle particle in m_particles)
            {
                particle.Position = WorldMatrix.Translation;
                Matrix rot = Matrix.CreateFromAxisAngle(WorldMatrix.Forward, MyMwcUtils.GetRandomFloat(0.0f, MathHelper.TwoPi));
                particle.Dir = Vector3.TransformNormal(MyUtilRandomVector3ByDeviatingVector.GetRandom(WorldMatrix.Up, 0.2f), rot);
                particle.FlyTime = MyMwcUtils.GetRandomFloat(500, 8000);
                particle.Life = MyMwcUtils.GetRandomFloat(8000, MyDecoyFlareConstants.MAX_LIVING_TIME);

                var effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.UniversalLauncher_DecoyFlare);
                effect.UserBirthMultiplier = 0.3f;
                particle.ParticleEffect = effect;
            }

            m_startParticleTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_flaresFired = true;
        }

        private void UpdateFlareParticlePosition(FlareParticle particle)
        {
            float delta = (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_startParticleTime) / particle.FlyTime;
            particle.Position += particle.Dir * MyMwcUtils.GetRandomFloat(0.2f, 0.4f) * (float) Math.Pow(1.0 - delta, 3);
        }

        public override void Close()
        {
            foreach (var particle in m_particles)
            {
                if (particle.ParticleEffect != null)
                {
                    particle.ParticleEffect.Stop();
                    particle.ParticleEffect = null;
                }
            }

            if ((m_flareLoop != null) && m_flareLoop.Value.IsPlaying)
            {
                m_flareLoop.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }

            m_flaresFired = false;
            Visible = true;
            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
