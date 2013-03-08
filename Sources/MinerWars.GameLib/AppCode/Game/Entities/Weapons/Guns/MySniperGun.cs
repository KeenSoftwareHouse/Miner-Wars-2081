using MinerWars.AppCode.Game.TransparentGeometry.Particles;

namespace MinerWars.AppCode.Game.Entities.Weapons
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
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.CommonLIB.AppCode.Utils;

    class MySniperGun : MySmallShipGunBase
    {
        int m_lastTimeShoot;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.Sniper, MyMaterialType.METAL, parentObject, position,
                forwardVector, upVector, objectBuilder);
        }

        Vector3 GetMuzzlePosition()
        {
            return m_positionMuzzleInWorldSpace + WorldMatrix.Forward * 0.1f + WorldMatrix.Up * 0.3f;
        }

        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (usedAmmo == null || (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < MySniperConstants.SHOT_INTERVAL_IN_MILISECONDS && !IsDummy) 
                return false;

            MyAmmoProperties ammoProperties = MyAmmoConstants.GetAmmoProperties(usedAmmo.AmmoType);

            AddProjectile(ammoProperties, this);

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            AddWeaponCue(ammoProperties.ShotSound);

            if (SysUtils.MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE)
            {
                MinerWars.AppCode.Game.Trailer.MyTrailerSave.UpdateGunShot(this.Parent, Trailer.MyTrailerGunsShotTypeEnum.PROJECTILE);
            }

            MyParticleEffect startEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_SmallGunShot);
            startEffect.WorldMatrix = Matrix.CreateWorld(GetMuzzlePosition(), GetForward(), GetUp());

            return true;
        }

        public override bool Draw(Render.MyRenderObject renderObject = null)
        {
            if (base.Draw(renderObject))
            {

                //  Draw muzzle flash
                int deltaTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot;
                if (deltaTime <= MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN)
                {
                    MyParticleEffects.GenerateMuzzleFlash(GetMuzzlePosition(), WorldMatrix.Forward, MyMwcUtils.GetRandomFloat(0.8f, 1.2f), MyMwcUtils.GetRandomFloat(1, 2), MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD_NEAR);
                }

                return true;
            }

            return false;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {                
                objectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int?)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper);
            }
            return objectBuilder;
        }
    }
}
