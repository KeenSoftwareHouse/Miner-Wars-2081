using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using Audio;
    using MinerWars.AppCode.Physics;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.CommonLIB.AppCode.Import;

    class MyLargeShipMissileLauncherBarrel : MyLargeShipBarrelBase
    {
        private int m_burstFireTime_ms = 0;
        private int m_burstFireTimeLoadingIntervalConst_ms = 2000;
        private bool m_burstFinish = false;
        private int m_burstFireCount = 0;
        private int m_burstToFire = 0;

        MyGroupMask m_groupMask;

        public void Init(StringBuilder hudLabelText, MyModelsEnum modelEnum, int burstFireCount, Matrix localMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType, MyLargeShipGunBase parentObject)
        {
            base.Init(hudLabelText, modelEnum, null, localMatrix, ammoType, parentObject);

            m_burstFireCount = burstFireCount;
            m_burstToFire = m_burstFireCount;
            m_burstFireTime_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            // User settings:            
            m_burstFireTimeLoadingIntervalConst_ms = 2000;

            // This is imoprtant for missile launchers (they are not able to lauchching rackets on safe trajectory)
            //BarrelElevationMin = 0.1571f;
            BarrelElevationMin = -0.6f;
        }

        public override bool StartShooting()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLargeShipMissileLauncherBarrel::StartShooting");

            LastShotId = null;

            int missileShotInterval = 0;
            MyLargeShipGunBase.GetMissileAmmoParams(GetAmmoType(), ref missileShotInterval);
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < missileShotInterval)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                return false;
            }

            m_burstFinish = false;
            while (!m_burstFinish)
            {
                if (!base.StartShooting())
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    return false;
                }

                if (m_groupMask == null)
                    MyPhysics.physicsSystem.GetRigidBodyModule().GetGroupMaskManager().GetGroupMask(ref m_groupMask);

                MyEntity target = ((MyLargeShipGunBase)Parent).GetTarget();

                List<MyModelDummy> muzzles = GetMuzzleFlashMatrix();
                m_activeMuzzle = --m_burstToFire;

                Matrix worldMatrix = WorldMatrix;
                Vector3 muzzleFlashPosition = MyUtils.GetTransform(muzzles[m_activeMuzzle].Matrix.Translation, ref worldMatrix);


                if (IsControlledByPlayer())
                {
                    //target = MySession.PlayerShip.TargetEntity as MySmallShip;
                    target = MyEnemyTargeting.SwitchNextTarget(true);
                }
                
                if (target != null || IsControlledByPlayer())
                {
                    MySoundCue? shootingSound = GetWeaponBase().UnifiedWeaponCueGet(MySoundCuesEnum.WepMissileLaunch3d);
                    if (shootingSound == null || !shootingSound.Value.IsPlaying)
                    {
                        GetWeaponBase().UnifiedWeaponCueSet(Audio.MySoundCuesEnum.WepMissileLaunch3d,
                            MyAudio.AddCue2dOr3d(this.GetWeaponBase().PrefabParent, Audio.MySoundCuesEnum.WepMissileLaunch3d, muzzleFlashPosition, WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                        //MyAudio.AddCue2dOr3d(this.GetWeaponBase().PrefabParent, Audio.MySoundCuesEnum.WepMissileLaunch3d, muzzleFlashPosition, WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero);
                    }

                    Vector3 deviateVector = GetDeviatedVector(MyAmmoConstants.GetAmmoProperties(GetAmmoType()));

                    var missile = MyMissiles.Add(GetAmmoType(), muzzleFlashPosition + 2 * WorldMatrix.Forward, WorldMatrix.Forward * 2.0f, deviateVector, Vector3.Zero, this, target, SearchingDistance, isLightWeight: true);
                    if (missile.GuidedInMultiplayer)
                    {
                        missile.EntityId = MyEntityIdentifier.AllocateId();
                        MyEntityIdentifier.AddEntityWithId(missile);
                        LastShotId = missile.EntityId;
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

                if (!IsControlledByPlayer())
                    m_burstFinish = true;
            }

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return true;
        }

        public override void StopShooting()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLargeShipMissileLauncherBarrel::StopShooting");

            base.StopShooting();

            if (m_groupMask != null)
                MyPhysics.physicsSystem.GetRigidBodyModule().GetGroupMaskManager().PushBackGroupMask(m_groupMask);

            /*
            MySoundCue? shootingSound = GetWeaponBase().UnifiedWeaponCueGet(MySoundCuesEnum.WepMissileLaunch3d);
            if (shootingSound != null)
            {
                shootingSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }*/

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public override void  UpdateAfterSimulation()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLargeShipMissileLauncherBarrel::UpdateAfterSimulation");

            base.UpdateAfterSimulation();

            if (!GetWeaponBase().IsActive())
            {
                MySoundCue? shootingSound = GetWeaponBase().UnifiedWeaponCueGet(MySoundCuesEnum.WepMissileLaunch3d);
                if (shootingSound != null)
                {
                    shootingSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public override Matrix GetViewMatrix()
        {
            Vector3 lookPosition = WorldMatrix.Translation + WorldMatrix.Backward * 3f + WorldMatrix.Up * 5f;
            Vector3 lookTarget = WorldMatrix.Translation + WorldMatrix.Forward * 1000000f;
            Vector3 lookDirection = Vector3.Normalize(lookTarget - lookPosition);
            Vector3 up = Vector3.Cross(WorldMatrix.Right, lookDirection);
            return Matrix.CreateLookAt(lookPosition, lookTarget, up);
        }
    }
}
