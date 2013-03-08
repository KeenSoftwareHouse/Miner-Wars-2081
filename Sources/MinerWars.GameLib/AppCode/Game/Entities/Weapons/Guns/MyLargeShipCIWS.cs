using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;

using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.CommonLIB.AppCode.Import;

    class MyLargeShipCIWS : MyLargeShipGunBase
    {

        public override void Init(StringBuilder hudLabelText, MyEntity parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_Base objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.LargeShipCiwsBase, MyMaterialType.METAL, parentObject, position, forwardVector, upVector, objectBuilder, MyModelsEnum.LargeShipCiwsBase_COL);

            Matrix barrelMatrix = MyMath.NormalizeMatrix(ModelLod0.Dummies["axis"].Matrix);
            MyLargeShipCIWSBarrel barrel = new MyLargeShipCIWSBarrel();

            barrel.Init(hudLabelText, barrelMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed, this);
            MountBarrel(barrel);

            // User settings:
            m_predictionIntervalConst_ms = 250;
            m_checkTargetIntervalConst_ms = 150;
            m_randomStandbyChangeConst_ms = 4000;
            
            m_shootingSound = MySoundCuesEnum.WepMachineGunNormFire3d;
            m_shootingSoundRelease = MySoundCuesEnum.WepMachineGunNormRel3d;
        }
    }


    // Barrel for missile launcher:
    class MyLargeShipCIWSBarrel : MyLargeShipBarrelBase
    {
        float m_projectileSpeed;
        float m_ammoHealthDamage;
        float m_projectileMaxTrajectory;
        Vector3 muzzleFlashPosition1;
        Vector3 muzzleFlashPosition2;
        MyParticleEffect m_shotSmoke2;

        public void Init(StringBuilder hudLabelText, Matrix localMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType, MyLargeShipGunBase parentObject)
        {
            base.Init(hudLabelText, MyModelsEnum.LargeShipCiwsBarrel, null, localMatrix, AmmoType, parentObject);
        }


        public override bool Draw(MyRenderObject renderObject)
        {
            if (!base.Draw(renderObject)) return false;
            //if (IsControlledByPlayer()) return true;

            Matrix worldMatrix = WorldMatrix;
            List<MyModelDummy> muzzles = GetMuzzleFlashMatrix();

            Vector3 m_muzzleFlashPosition1 = MyUtils.GetTransform(muzzles[0].Matrix.Translation, ref worldMatrix);
            m_muzzleFlashPosition1 -= WorldMatrix.Forward * 0.1f;
            Vector3 m_muzzleFlashPosition2 = MyUtils.GetTransform(muzzles[1].Matrix.Translation, ref worldMatrix);
            m_muzzleFlashPosition2 -= WorldMatrix.Forward * 0.1f;

            // Draw muzzle flash:
            int dt = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot;
            if (dt <= MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN)
            {
                MyParticleEffects.GenerateMuzzleFlash(m_muzzleFlashPosition1, worldMatrix.Forward, m_muzzleFlashRadius, m_muzzleFlashLength);
                MyParticleEffects.GenerateMuzzleFlash(m_muzzleFlashPosition2, worldMatrix.Forward, m_muzzleFlashRadius, m_muzzleFlashLength);
            }

            // Draw smoke:
            if (m_shotSmoke != null)
            {
                m_shotSmoke.UserBirthMultiplier = m_smokeToGenerate / 5;
                m_shotSmoke.UserScale = 10;
                m_shotSmoke.WorldMatrix = Matrix.CreateTranslation(m_muzzleFlashPosition1);
            }
            if (m_shotSmoke2 != null)
            {
                m_shotSmoke2.UserBirthMultiplier = m_smokeToGenerate / 5;
                m_shotSmoke2.UserScale = 10;
                m_shotSmoke2.WorldMatrix = Matrix.CreateTranslation(m_muzzleFlashPosition2);
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
            // get muzzle flashes:
            List<MyModelDummy> muzzles = GetMuzzleFlashMatrix();

            muzzleFlashPosition1 = MyUtils.GetTransform(muzzles[0].Matrix.Translation, ref worldMatrix);
            muzzleFlashPosition2 = MyUtils.GetTransform(muzzles[1].Matrix.Translation, ref worldMatrix);

           // if (!IsControlledByPlayer())
            {
                if (m_shotSmoke == null)
                {
                    m_shotSmoke = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_LargeGunShot);
                    m_shotSmoke.AutoDelete = false;
                }
                m_shotSmoke.UserEmitterScale = m_smokeToGenerate;
                m_shotSmoke.WorldMatrix = Matrix.CreateTranslation(muzzleFlashPosition1);

                if (m_shotSmoke2 == null)
                {
                    m_shotSmoke2 = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_LargeGunShot);
                    m_shotSmoke2.AutoDelete = false;
                }
                m_shotSmoke2.UserEmitterScale = m_smokeToGenerate;
                m_shotSmoke2.WorldMatrix = Matrix.CreateTranslation(muzzleFlashPosition2);
            }

            int randSoundSource = MyMwcUtils.GetRandomInt(2);

            MySoundCue? shootingSound = GetWeaponBase().UnifiedWeaponCueGet(MySoundCuesEnum.WepMachineGunNormFire3d);
            if (shootingSound == null || !shootingSound.Value.IsPlaying)
            {
                GetWeaponBase().UnifiedWeaponCueSet(Audio.MySoundCuesEnum.WepMachineGunNormFire3d,
                    MyAudio.AddCue2dOr3d(this.GetWeaponBase().PrefabParent, Audio.MySoundCuesEnum.WepMachineGunNormFire3d, randSoundSource == 1 ? muzzleFlashPosition1 : muzzleFlashPosition2, WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                    //MyAudio.AddCue3D(Audio.MySoundCuesEnum.WepAutocanonFire3d, randSoundSource == 1 ? muzzleFlashPosition1 : muzzleFlashPosition2, WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
            }

            AddProjectile(MyAmmoConstants.GetAmmoProperties(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), muzzleFlashPosition1);
            AddProjectile(MyAmmoConstants.GetAmmoProperties(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), muzzleFlashPosition2);

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            return true;
        }

        public override void Close()
        {
            base.Close();

            if (m_shotSmoke2 != null)
            {
                MyParticlesManager.RemoveParticleEffect(m_shotSmoke2);
                m_shotSmoke2 = null;
            }
        }

        public override Matrix GetViewMatrix()
        {
            Vector3 lookPosition = WorldMatrix.Translation + WorldMatrix.Backward * 1.65f + WorldMatrix.Up * 3f;
            Vector3 lookTarget = WorldMatrix.Translation + WorldMatrix.Forward * 1000000f;
            Vector3 lookDirection = Vector3.Normalize(lookTarget - lookPosition);
            Vector3 up = Vector3.Cross(WorldMatrix.Right, lookDirection);
            return Matrix.CreateLookAt(lookPosition, lookTarget, up);
        }
    }
}
