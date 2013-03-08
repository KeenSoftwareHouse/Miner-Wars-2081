using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using MinerWarsMath;

using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.TransparentGeometry;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.CommonLIB.AppCode.Import;

    class MyLargeShipAutocannon : MyLargeShipGunBase
    {
        public override void Init(StringBuilder hudLabelText, MyEntity parentObject, Vector3 position, Vector3 forwardVector, Vector3 upVector, MyMwcObjectBuilder_Base objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.LargeShipAutocannonBase, MyMaterialType.METAL, parentObject, position, forwardVector, upVector, objectBuilder, MyModelsEnum.LargeShipAutocannonBase);

            Matrix barrelMatrix = MyMath.NormalizeMatrix(ModelLod0.Dummies["axis"].Matrix);

            MyLargeShipAutocannonBarrel barrel = new MyLargeShipAutocannonBarrel();
            barrel.Init(hudLabelText, barrelMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed, this);
            MountBarrel(barrel);

            // User settings:
            m_predictionIntervalConst_ms = 250;
            m_checkTargetIntervalConst_ms = 150;
            m_randomStandbyChangeConst_ms = 4000;

            m_shootingSound = MySoundCuesEnum.WepAutocanonFire3d;
            m_shootingSoundRelease = MySoundCuesEnum.WepAutocanonRel3d;
        }
    }


    class MyLargeShipAutocannonBarrel : MyLargeShipBarrelBase
    {
        //private Vector3 m_muzzleFlashStartPosition;  // Position of the barrel muzzle flashes from the dummy
        private Vector3 m_muzzleFlashPosition;
        float m_projectileMaxTrajectory;
        Vector3 m_projectileColor;

        public void Init(StringBuilder hudLabelText, Matrix localMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType, MyLargeShipGunBase parentObject)
        {
            base.Init(hudLabelText, MyModelsEnum.LargeShipAutocannonBarrel, null, localMatrix, AmmoType, parentObject);
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;
          //  if (IsControlledByPlayer()) return true;

            Matrix worldMatrix = WorldMatrix;
            List<MyModelDummy> muzzles = GetMuzzleFlashMatrix();
            Vector3 muzzleFlashPosition = MyUtils.GetTransform(muzzles[m_activeMuzzle].Matrix.Translation, ref worldMatrix);

            // Draw muzzle flash:
            int dt = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot;
            if (dt <= MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN && m_muzzleFlashLength > 0)
            {
                MyParticleEffects.GenerateMuzzleFlash(muzzleFlashPosition, worldMatrix.Forward, m_muzzleFlashRadius, m_muzzleFlashLength);
            }

            // Draw smoke:
            if (m_shotSmoke != null)
            {
                m_shotSmoke.UserBirthMultiplier = m_smokeToGenerate;
                m_shotSmoke.WorldMatrix = Matrix.CreateTranslation(muzzleFlashPosition);
            }

            return true;
        }

        // Start shooting on the presented target in the queue:
        public override bool StartShooting()
        {
            // start shooting this kind of ammo ...
            if (!base.StartShooting())
                return false;

            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < (MyMachineGunConstants.SHOT_INTERVAL_IN_MILISECONDS/* * 0.75f*/))
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
            m_activeMuzzle = muzzles.Count == 1 ? 0 : MyMwcUtils.GetRandomInt(muzzles.Count);

            m_muzzleFlashPosition = MyUtils.GetTransform(muzzles[m_activeMuzzle].Matrix.Translation, ref worldMatrix);

            if (m_shotSmoke == null)
            {
                m_shotSmoke = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_LargeGunShot);
                m_shotSmoke.AutoDelete = false;
            }
            m_shotSmoke.UserEmitterScale = m_smokeToGenerate;
            m_shotSmoke.WorldMatrix = Matrix.CreateTranslation(m_muzzleFlashPosition);
            m_shotSmoke.UserScale = 5;

            GetWeaponBase().PlayShootingSound();

            // Shoot projectiles
            AddProjectile(MyAmmoConstants.GetAmmoProperties(GetAmmoType()), m_muzzleFlashPosition);

            // dont decrease ammo count ...
            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            return true;
        }

        public override Matrix GetViewMatrix()
        {
            Vector3 lookPosition = WorldMatrix.Translation + WorldMatrix.Backward * 0.5f + WorldMatrix.Up * 3f;
            Vector3 lookTarget = WorldMatrix.Translation + WorldMatrix.Forward * 1000000f;
            Vector3 lookDirection = Vector3.Normalize(lookTarget - lookPosition);
            Vector3 up = Vector3.Cross(WorldMatrix.Right, lookDirection);
            return Matrix.CreateLookAt(lookPosition, lookTarget, up);
        }
    }
}
