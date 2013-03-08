using MinerWars.AppCode.Game.TransparentGeometry.Particles;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System;
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using CommonLIB.AppCode.Utils;
    using MinerWarsMath;
    using Models;
    using TransparentGeometry;
    using Utils;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Render;

    class MyAutomaticRifleSilencerGun : MySmallShipGunBase
    {
        int m_lastTimeShoot;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.Rifle, MyMaterialType.METAL, parentObject, position,
                forwardVector, upVector, objectBuilder);
        }

        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (usedAmmo == null || (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < MyARSConstants.SHOT_INTERVAL_IN_MILISECONDS) 
                return false;

            m_positionMuzzleInWorldSpace += WorldMatrix.Forward + WorldMatrix.Up * 0.2f;

            MyAmmoProperties ammoProperties = MyAmmoConstants.GetAmmoProperties(usedAmmo.AmmoType);
            AddProjectile(ammoProperties, this);

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            AddWeaponCue(ammoProperties.ShotSound);

            if (SysUtils.MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE)
            {
                MinerWars.AppCode.Game.Trailer.MyTrailerSave.UpdateGunShot(this.Parent, Trailer.MyTrailerGunsShotTypeEnum.PROJECTILE);
            }

            MyParticleEffect startEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_SmallGunShot);
            startEffect.WorldMatrix = Matrix.CreateTranslation(m_positionMuzzleInWorldSpace);

            return true;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (!base.Draw(renderObject))
                return false;
            /*
            //  Draw muzzle flash
            int deltaTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot;
            if (deltaTime <= MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN)
            {
                float FAKE_RADIUS = MyMwcUtils.GetRandomFloat(0.5f, 1.5f);
                float FAKE_THICKNESS = MyMwcUtils.GetRandomFloat(FAKE_RADIUS - 0.1f, FAKE_RADIUS);
                float FAKE_LENGTH = MyMwcUtils.GetRandomFloat(7, 8);
                float FAKE_ANGLE = MyMwcUtils.GetRandomFloat(0, MathHelper.PiOver2);

                //float colorComponent = 1;
                float colorComponent = 1 - (float)deltaTime / (float)MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN;
                colorComponent = 1 - (float)Math.Pow(colorComponent, 5);
                colorComponent *= 1.3f;
                //Vector4 color = new Vector4(1.0f, 1.0f, 1.0f, 1);
                Vector4 color = new Vector4(colorComponent, colorComponent, colorComponent, 1);

                MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.MuzzleFlashMachineGunSide, color, m_positionMuzzleInWorldSpace + GetUp() * 0.4f,
                    WorldMatrix.Forward, FAKE_LENGTH, FAKE_THICKNESS);
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.MuzzleFlashMachineGunFront, color, m_positionMuzzleInWorldSpace + WorldMatrix.Forward + GetUp() * 0.4f, FAKE_RADIUS, FAKE_ANGLE);
            }
              */

            return true;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(WeaponType = MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer);
    }
            return objectBuilder;
}
    }
}
