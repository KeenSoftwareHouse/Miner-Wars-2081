using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System;
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
    using SysUtils;
    using MinerWars.AppCode.Game.Render;

    class MyMachineGun : MySmallShipGunBase
    {
        int m_lastTimeShoot;

        MySoundCuesEnum m_cueEnum;

        MyParticleEffect m_smokeEffect;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.MachineGun, MyMaterialType.METAL, parentObject, position,
                forwardVector, upVector, objectBuilder);
        }

        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (usedAmmo == null || (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < MyMachineGunConstants.SHOT_INTERVAL_IN_MILISECONDS && !IsDummy) return false;

            if (m_smokeEffect == null)
            {
                if (MyCamera.GetDistanceWithFOV(GetPosition()) < 150)
                {
                    m_smokeEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_Autocannon);
                    m_smokeEffect.WorldMatrix = WorldMatrix;
                    m_smokeEffect.OnDelete += OnSmokeEffectDelete;
                }
            }

            MyAmmoProperties ammoProperties = MyAmmoConstants.GetAmmoProperties(usedAmmo.AmmoType);

            if (MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE)
            {
                MinerWars.AppCode.Game.Trailer.MyTrailerSave.UpdateGunShot(this.Parent, Trailer.MyTrailerGunsShotTypeEnum.PROJECTILE);
            }

            AddProjectile(ammoProperties, this);

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            StartLoopSound(ammoProperties.ShotSound);

            return true;
        }

        private void OnSmokeEffectDelete(object sender, EventArgs eventArgs)
        {
            m_smokeEffect = null;
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            CorrectMuzzlePositionHack();

            if (m_smokeEffect != null)
            {
                float smokeOffset = 0.2f;
                if ((GUI.MyGuiScreenGamePlay.Static.CameraAttachedTo == GUI.MyCameraAttachedToEnum.PlayerMinerShip) &&
                    (Parent == MySession.PlayerShip))
                {
                    smokeOffset = 0.0f;
                }

                m_smokeEffect.WorldMatrix = Matrix.CreateTranslation(m_positionMuzzleInWorldSpace + WorldMatrix.Forward * smokeOffset);
                m_smokeEffect.UserBirthMultiplier = 50;
            }

            if (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot > MyMachineGunConstants.RELEASE_TIME_AFTER_FIRE)
            {
                StopLoopSound();

                if (m_smokeEffect != null)
                {
                    m_smokeEffect.Stop(false);
                }
            }
        }

        private void CorrectMuzzlePositionHack()
        {
            m_positionMuzzleInWorldSpace += WorldMatrix.Up * 0.25f;
            float offset = 0.6f;
            if ((GUI.MyGuiScreenGamePlay.Static.CameraAttachedTo == GUI.MyCameraAttachedToEnum.PlayerMinerShip) &&
                (Parent == MySession.PlayerShip))
            {
                offset = 1.0f;
            }
            m_positionMuzzleInWorldSpace += offset * WorldMatrix.Forward;
        }

        public void StartLoopSound(MySoundCuesEnum cueEnum)
        {
            MySoundCue? weaponCue = GetWeaponCue(cueEnum);
            if ((weaponCue == null) || (weaponCue.Value.IsPlaying == false))
            {
                m_cueEnum = cueEnum;
                AddWeaponCue(cueEnum);
            }
        }

        public void StopLoopSound()
        {
            MySoundCue? weaponCue = GetWeaponCue(m_cueEnum);
            if ((weaponCue != null) && weaponCue.Value.IsValid && (weaponCue.Value.IsPlaying == true))
            {
                MySoundCuesEnum? cueEnum = null;
                if (m_cueEnum == MySoundCuesEnum.WepMachineGunHighFire3d)
                {
                    cueEnum = MySoundCuesEnum.WepMachineGunHighRel3d;
                }
                else
                {
                    cueEnum = MySoundCuesEnum.WepMachineGunNormRel3d;
                }
                weaponCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                AddWeaponCue(cueEnum.Value);
            }
        }

        //  Draw muzzle flash not matter if in frustum (it's because it's above the frustum)
        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;

            //  Draw muzzle flash
            int deltaTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot;
            if (deltaTime <= MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN)
            {
                MyParticleEffects.GenerateMuzzleFlash(m_positionMuzzleInWorldSpace + WorldMatrix.Forward, WorldMatrix.Forward, 0.4f, 0.7f);
            }

            return true;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(WeaponType = MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun);
            }
            return objectBuilder;
        }

        public override void Close()
        {
            if (m_smokeEffect != null)
            {
                m_smokeEffect.Stop();
                m_smokeEffect = null;
            }
            base.Close();
        }
    }
}
