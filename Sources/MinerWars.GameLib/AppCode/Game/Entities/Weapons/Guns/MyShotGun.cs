namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using CommonLIB.AppCode.Utils;
    using MinerWarsMath;
    using Models;
    using TransparentGeometry;
    using SysUtils.Utils;
    using Utils;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.Gameplay;
    using System;
    using MinerWars.AppCode.Game.Render;

    class MyShotGun : MySmallShipGunBase
    {
        int m_lastTimeShoot;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.Shotgun, MyMaterialType.METAL, parentObject, position,
                forwardVector, upVector, objectBuilder);
        }

        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (usedAmmo == null || (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < MyShotgunConstants.SHOT_INTERVAL_IN_MILISECONDS && !IsDummy) return false;

            MyAmmoProperties ammoProperties = MyAmmoConstants.GetAmmoProperties(usedAmmo.AmmoType);
          
            FireShotGunShot(ammoProperties);
            
            return true;
        }

        private void FireShotGunShot(MyAmmoProperties ammoProperties)
        {
            int currentRound = ammoProperties.ProjectileGroupSize;
            float deviateAngle = ammoProperties.DeviateAngle;

            //  Create one projectile - but deviate projectile direction be random angle
            if (IsThisGunFriendly())
                deviateAngle += MyGameplayConstants.GameplayDifficultyProfile.DeviatingAnglePlayerOnEnemy;
            else
                deviateAngle += MyGameplayConstants.GameplayDifficultyProfile.DeviatingAngleEnemyBotOnPlayer;

            while (currentRound-- > 0)
            {
                Vector3 projectileForwardVector = MyUtilRandomVector3ByDeviatingVector.GetRandom(WorldMatrix.Forward, deviateAngle);

                float billboardSize = ammoProperties.AmmoType == MyAmmoType.Explosive ? 2 : 1;

                MyProjectiles.AddShotgun(ammoProperties, Parent, m_positionMuzzleInWorldSpace, Vector3.Zero/* Parent.Physics.LinearVelocity*/,
                                  projectileForwardVector, currentRound % MyShotgunConstants.PROJECTILE_GROUP_SIZE == 0, 2, this, billboardSize, Parent);
            }

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            AddWeaponCue(ammoProperties.ShotSound);
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {                
                objectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int?)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun);
            }
            return objectBuilder;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            base.Draw(renderObject);

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

                Vector3 muzzleInWorldSpace = m_positionMuzzleInWorldSpace + WorldMatrix.Up * 0.2f;

                MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.MuzzleFlashMachineGunSide, color, muzzleInWorldSpace - WorldMatrix.Forward * 1.0f,
                    WorldMatrix.Forward, FAKE_LENGTH, FAKE_THICKNESS);
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.MuzzleFlashMachineGunFront, color, muzzleInWorldSpace, FAKE_RADIUS, FAKE_ANGLE);
            }
            return true;
        }
    }
}
