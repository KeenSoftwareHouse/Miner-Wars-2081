using System.Text;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.App;

using MinerWarsCustomContentImporters;

namespace MinerWars.AppCode.Game.Managers.EntityManager.Entities.Weapons          
{
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders;
/*
    class MyLargeShipMissileLauncher4Gun : MyLargeShipGunBase
    {
        public override void Init(StringBuilder hudLabelText, MyLargeShip parentObject, Vector3 position, Vector3 forwardVector, Vector3 upVector, MyMwcObjectBuilder_Base objectBuilder)
        {
            base.Init(hudLabelText,  MyModelsEnum.LargeShipMissileLauncher4Base, MyMaterialType.METAL, parentObject, position, forwardVector, upVector, objectBuilder);

            Matrix barrelMatrix = MyMath.NormalizeMatrix(ModelLod0.Dummies["axis"].Matrix);
            MyLargeShipMissileLauncher4Barrel barrel = new MyLargeShipMissileLauncher4Barrel();
            
            barrel.Init(hudLabelText, barrelMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic, this);
            MountBarrel(barrel);

            // User settings:
            m_predictionIntervalConst_ms = 250;
            m_checkTargetIntervalConst_ms = 150;
            m_randomStandbyChangeConst_ms = 4000;
            m_rotationSpeed = MathHelper.Pi / 2000.0f;
            m_elevationSpeed = MathHelper.Pi / 2000.0f;
        }

        public override void Update()
        {
            base.Update();
        }
    }


    class MyLargeShipMissileLauncher4Barrel : MyLargeShipBarrelBase
    {
        private int m_burstFireTime_ms = 0;
        private int m_burstFireTimeLoadingIntervalConst_ms = 2000;
        private bool m_burstFinish = false;
        private const int m_burstFireCount = 4;
        private int m_burstToFire = 0;

        public void Init(StringBuilder hudLabelText, Matrix localMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType, MyLargeShipGunBase parentObject)
        {
            base.Init(hudLabelText, MyModelsEnum.LargeShipMissileLauncher4Barrel, null, localMatrix, AmmoType, parentObject);

            m_burstToFire = m_burstFireCount;
            m_burstFireTime_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            // User settings:
            m_searchingDistanceMax = 2000.0f;
            m_aimingDistanceMax = 1000.0f;
            m_burstFireTimeLoadingIntervalConst_ms = 2000;

            // This is imoprtant for missile launchers (they are not able to lauchching rackets on safe trajectory)
            m_barrelElevationMin = 0.1571f;
        }

        public override bool StartShooting()
        {            
            if (!m_burstFinish)
            {
                int missileShotInterval = 0;

                MyLargeShipGunBase.GetMissileAmmoParams(GetAmmoType(), ref missileShotInterval);

                if (!base.StartShooting())
                    return false;

                if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < missileShotInterval) return false;                

                MySmallShip target = ((MyLargeShipGunBase)Parent).GetTarget();

                List<MyModelDummy> muzzles = GetMuzzleFlashMatrix();
                m_activeMuzzle = --m_burstToFire;

                Matrix worldMatrix = WorldMatrix;
                Vector3 muzzleFlashPosition = MyUtils.GetTransform(muzzles[m_activeMuzzle].Matrix.Translation, ref worldMatrix);


                if (target != null)
                {
                    MySoundCue? shootingSound = GetWeaponBase().UnifiedWeaponCueGet(MySoundCuesEnum.WepMissileLaunch3d);
                    if (shootingSound == null || !shootingSound.Value.IsPlaying)
                    {
                        GetWeaponBase().UnifiedWeaponCueSet(Audio.MySoundCuesEnum.WepMissileLaunch3d,
                            MyAudio.AddCue2dOr3d(this, Audio.MySoundCuesEnum.WepMissileLaunch3d, muzzleFlashPosition, WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                    }

                    //MyMissiles.Add(missileType, muzzleFlashPosition, Vector3.Zero, WorldMatrix.Forward, this, target);
                    MyMissiles.Add(GetAmmoType(), muzzleFlashPosition, WorldMatrix.Forward * 2.0f, WorldMatrix.Forward, this, target);
                }
            }
            
            if (m_burstToFire <= 0)
            {
                m_burstFireTime_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                m_burstToFire = m_burstFireCount;
                m_burstFinish = true;
            }
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_burstFireTime_ms) > m_burstFireTimeLoadingIntervalConst_ms)
            {
                m_burstFinish = false;
            }
            
            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            return true;
        }

        public override void StopShooting()
        {
            base.StopShooting();

            MySoundCue? shootingSound = GetWeaponBase().UnifiedWeaponCueGet(MySoundCuesEnum.WepMissileLaunch3d);
            if (shootingSound != null)
            {
                shootingSound.Value.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
            }
        }

        public override void Update()
        {
            base.Update();

            if (!GetWeaponBase().IsActivate())
            {
                MySoundCue? shootingSound = GetWeaponBase().UnifiedWeaponCueGet(MySoundCuesEnum.WepMissileLaunch3d);
                if (shootingSound != null)
                {
                    shootingSound.Value.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
                }
            }
        }
    }  */
}
