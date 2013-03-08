using MinerWars.AppCode.Game.TransparentGeometry.Particles;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWarsMath;
    using Models;
    using TransparentGeometry;
    using Utils;

    class MyUniversalLauncher : MySmallShipGunBase
    {
        int m_lastTimeShoot;
        const float UNIVERSAL_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS = 500;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject, Vector3 position, 
            Vector3 forwardVector, Vector3 upVector, 
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.UniversalLauncher, MyMaterialType.METAL, parentObject, position, 
                forwardVector, upVector, objectBuilder);
            m_lastTimeShoot = MyConstants.FAREST_TIME_IN_PAST;
        }

        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (usedAmmo == null || (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < UNIVERSAL_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS && !IsDummy) return false;

            bool shot = false;
            switch (usedAmmo.AmmoType)
            {
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart:
                    shot = Shot<MyMineSmart>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic:
                    shot = Shot<MyMineBasic>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem:
                    shot = Shot<MyMineBioChem>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive:
                    shot = Shot<MySphereExplosive>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare:
                    shot = Shot<MyDecoyFlare>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb:
                    shot = Shot<MyFlashBomb>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell:
                    shot = Shot<MyIlluminatingShell>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb:
                    shot = Shot<MySmokeBomb>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer:
                    shot = Shot<MyAsteroidKiller>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive:
                    shot = Shot<MyDirectionalExplosive>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb:
                    shot = Shot<MyTimeBomb>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb:
                    shot = Shot<MyRemoteBomb>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram:
                    shot = Shot<MyHologram>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb:
                    shot = Shot<MyGravityBomb>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera:
                    shot = Shot<MyRemoteCamera>();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb:
                    shot = Shot<MyEMPBomb>();
                    break;
            }

            if (!shot)
                return false;

            AddWeaponCue(MySoundCuesEnum.WepUnivLaunch3d);

            var smokeEffect = MyParticlesManager.CreateParticleEffect((int) MyParticleEffectsIDEnum.Smoke_SmallGunShot);
            smokeEffect.WorldMatrix = Matrix.CreateWorld(m_positionMuzzleInWorldSpace, WorldMatrix.Forward, WorldMatrix.Up);

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            return true;
        }

        bool Shot<T>() where T : class, IUniversalLauncherShell, new()
        {
            T shell = MyUniversalLauncherShells.Allocate<T>(Parent.EntityId.Value.PlayerId);
            if (shell != null)
            {
                Vector3 forwardVelocity = MyMath.ForwardVectorProjection(this.WorldMatrix.Forward,
                                                                         GetParentMinerShip().Physics.LinearVelocity);

                if (MinerWars.AppCode.Game.Managers.Session.MySession.Is25DSector)
                    m_positionMuzzleInWorldSpace.Y = 0;

                shell.Start(m_positionMuzzleInWorldSpace, forwardVelocity, WorldMatrix.Forward, 100, Parent);

                return true;
            }

            return false;
        }
    }
}