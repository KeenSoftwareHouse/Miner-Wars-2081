using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using CommonLIB.AppCode.Utils;
    using Lights;
    using MinerWarsMath;
    using Models;
    using TransparentGeometry;
    using Utils;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.AppCode.Game.Entities.Ships.AI;
    using MinerWars.AppCode.Game.Render;

    class MyIlluminatingShell : MyAmmoBase, IUniversalLauncherShell
    {
        MyLight m_light;
        MyParticleEffect m_particleEffect;

        public void Init()
        {
            base.Init(MyModelsEnum.IlluminatingShell, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.IlluminatingShellHud));

            this.Physics.AddForce(
                MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                initialVelocity * 2,
                position,
                null);  //  Setting a torque here make trouble for recycled mines... so for now we don't use it. Maybe in future in other physics engine than JLX.

            m_light = MyLights.AddLight();
            if (m_light != null)
            {
                m_light.Start(MyLight.LightTypeEnum.PointLight, position, MyIlluminatingShellsConstants.LIGHT_COLOR, 1, MyIlluminatingShellsConstants.LIGHT_RADIUS);
            }

            m_particleEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.UniversalLauncher_IlluminatingShell);
            m_particleEffect.WorldMatrix = WorldMatrix;
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (m_elapsedMiliseconds > MyIlluminatingShellsConstants.MAX_LIVING_TIME + MyIlluminatingShellsConstants.DIYNG_TIME)
            {
                if (m_particleEffect != null)
                {
                    m_particleEffect.Stop();
                    m_particleEffect = null;
                }

                //  Free the light
                if (m_light != null)
                {
                    MyLights.RemoveLight(m_light);
                    m_light = null;
                }

                MarkForClose();
            }


            //  Update light position
            if (m_light != null)
            {
                // Aggro near bots
                if (m_light.LightOn)
                {
                    if (this.WorldMatrix.Translation != m_previousPosition)
                    {
                        MyLine line = new MyLine(m_previousPosition, WorldMatrix.Translation);
                        MyDangerZones.Instance.Notify(line, OwnerEntity);
                    }
                }

                if ((m_elapsedMiliseconds > MyIlluminatingShellsConstants.MAX_LIVING_TIME) &&
                    (m_elapsedMiliseconds < MyIlluminatingShellsConstants.MAX_LIVING_TIME +  MyIlluminatingShellsConstants.DIYNG_TIME))
                {
                    m_light.LightOn = MyMwcUtils.GetRandomBool(2);
                    m_particleEffect.UserScale = m_light.LightOn ? 1.0f : 0.001f;
                }

                m_light.SetPosition(GetPosition());
                m_light.Color = MyIlluminatingShellsConstants.LIGHT_COLOR;
                m_light.Range = MyIlluminatingShellsConstants.LIGHT_RADIUS;
            }
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            if (m_particleEffect != null)
            {
                // Update effect position
                m_particleEffect.WorldMatrix = WorldMatrix;
            }
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;

            return true;
        }

        public override void Close()
        {
            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }
    }
}
