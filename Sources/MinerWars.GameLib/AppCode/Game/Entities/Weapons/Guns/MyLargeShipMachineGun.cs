using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using MinerWarsMath;

using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Prefabs;

using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWarsMath;
    
    using Models;
    using TransparentGeometry;
    using SysUtils;
    using Utils;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.CommonLIB.AppCode.Import;


    class MyLargeShipMachineGun : MyLargeShipGunBase
    {
         //public override void Init(StringBuilder hudLabelText, MyLargeShip parentObject,
         //   Vector3 position, Vector3 forwardVector, Vector3 upVector,
         //   MyMwcObjectBuilder_LargeShip_Weapon objectBuilder)
        public override void Init(StringBuilder hudLabelText, MyEntity parentObject, Vector3 position, Vector3 forwardVector, Vector3 upVector, MyMwcObjectBuilder_Base objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.LargeShipMachineGunBase, MyMaterialType.METAL, parentObject, position, forwardVector, upVector, objectBuilder, MyModelsEnum.LargeShipMachineGunBase_COL);

            Matrix barrelMatrix = ModelLod0 != null
                                      ? MyMath.NormalizeMatrix(ModelLod0.Dummies["axis"].Matrix)
                                      : Matrix.Identity;

            MyLargeShipMachineBarrel barrel = new MyLargeShipMachineBarrel();
            barrel.Init(hudLabelText, barrelMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed, this);
            MountBarrel(barrel);

            // User settings:
            m_predictionIntervalConst_ms = 250;
            m_checkTargetIntervalConst_ms = 150;
            m_randomStandbyChangeConst_ms = 3000;

            m_shootingSound = MySoundCuesEnum.WepMachineGunHighFire3d;
            m_shootingSoundRelease = MySoundCuesEnum.WepMachineGunHighRel3d;
        }
    }

    // Barrel for machine gun:
    class MyLargeShipMachineBarrel : MyLargeShipBarrelBase
    {
        #region MACHINE GUN BARREL VARIABLES
        // distances for the testing:
        private Vector3 m_muzzleFlashStartPosition;  // Position of the barrel muzzle flashes from the dummy
        private Vector3 m_muzzleFlashPosition;
        #endregion

        public void Init(StringBuilder hudLabelText, Matrix localMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType, MyLargeShipGunBase parentObject)
        {
            base.Init(hudLabelText, MyModelsEnum.LargeShipMachineGunBarrel, null, localMatrix, AmmoType, parentObject);


            // Muzzle flash position from the dummy on the model:
            Matrix muzzleFlashDummy = ModelLod0 != null ? ModelLod0.Dummies["MUZZLE_FLASH"].Matrix : Matrix.Identity;
            m_muzzleFlashStartPosition = muzzleFlashDummy.Translation;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;
            //if (IsControlledByPlayer()) return true;

            Matrix worldMatrix = WorldMatrix;
            m_muzzleFlashPosition = MyUtils.GetTransform(m_muzzleFlashStartPosition, ref worldMatrix);

            // Draw muzzle flash:
            int dt = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot;
            if (dt <= MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN)
            {
                MyParticleEffects.GenerateMuzzleFlash(m_muzzleFlashPosition, worldMatrix.Forward, m_muzzleFlashRadius, m_muzzleFlashLength);
            }
 
            // Draw smoke:
            if (m_shotSmoke != null)
            {
                m_shotSmoke.UserBirthMultiplier = m_smokeToGenerate;
                m_shotSmoke.WorldMatrix = Matrix.CreateTranslation(m_muzzleFlashPosition);
            }

            return true;
        }

        public override bool StartShooting()
        {
            if (!base.StartShooting())
                return false;

            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < MyAutocanonConstants.SHOT_INTERVAL_IN_MILISECONDS)
                return false;

            // Set muzzle flashes:
            m_muzzleFlashLength = MyMwcUtils.GetRandomFloat(4, 6);
            m_muzzleFlashRadius = MyMwcUtils.GetRandomFloat(1.2f, 2.0f);

            // Increse smoke to generate
            IncreaseSmoke();

            // Make random trajectories for the bullet:
            Matrix worldMatrix = WorldMatrix;

            // get muzzel flash positions:
            List<MyModelDummy> muzzles = GetMuzzleFlashMatrix();

            Vector3 muzzleFlashPosition = MyUtils.GetTransform(muzzles[0].Matrix.Translation, ref worldMatrix);
            m_muzzleFlashPosition = MyUtils.GetTransform(m_muzzleFlashStartPosition, ref worldMatrix);

            if (m_shotSmoke == null)
            {
                m_shotSmoke = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_LargeGunShot);
                m_shotSmoke.AutoDelete = false;
            }

            m_shotSmoke.UserEmitterScale = m_smokeToGenerate;
            m_shotSmoke.UserScale = 5;
            m_shotSmoke.WorldMatrix = Matrix.CreateTranslation(m_muzzleFlashPosition);

            GetWeaponBase().PlayShootingSound();

            // Shoot projectiles
            AddProjectile(MyAmmoConstants.GetAmmoProperties(GetAmmoType()), m_muzzleFlashPosition);

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            return true;
        }

        public override Matrix GetViewMatrix()
        {
            Vector3 lookPosition = WorldMatrix.Translation + WorldMatrix.Backward * 0f + WorldMatrix.Up * 3f;
            Vector3 lookTarget = WorldMatrix.Translation + WorldMatrix.Forward * 1000000f;
            Vector3 lookDirection = Vector3.Normalize(lookTarget - lookPosition);
            Vector3 up = Vector3.Cross(WorldMatrix.Right, lookDirection);
            return Matrix.CreateLookAt(lookPosition, lookTarget, up);
        }
    }
}
