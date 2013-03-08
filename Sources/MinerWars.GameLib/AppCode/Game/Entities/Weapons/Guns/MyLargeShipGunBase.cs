#region Using
using System;
using System.Text;
using MinerWarsMath;

using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;

#endregion

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.Gameplay;
    using SysUtils.Utils;
    using MinerWars.AppCode.Game.World.Global;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.GUI;
    using System.Diagnostics;
    using MinerWars.AppCode.Game.TransparentGeometry;
    using System.Collections.Generic;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Game.Sessions;
    using MinerWars.AppCode.Physics.Collisions;
    using MinerWars.AppCode.Game.Managers;


    abstract class MyLargeShipGunBase : MyGunBase
    {
        #region BASE VARIABLES
        private MyLargeShipBarrelBase       m_barrel;
        private MyLargeShipGunStatus        m_status = MyLargeShipGunStatus.MyWeaponStatus_Deactivated;
        private int                         m_predictionInterval_ms;
        protected int                       m_predictionIntervalConst_ms;
        private int                         m_checkTargetInterval_ms;
        protected int                       m_checkTargetIntervalConst_ms;
        private float                       m_rotation;
        private float                       m_elevation;
        private float                       m_rotationLast;
        private float                       m_elevationLast;
        protected float                     m_rotationSpeed;
        protected float                     m_elevationSpeed;
        private int                         m_rotationInterval_ms;
        private int                         m_elevationInterval_ms;
        private int                         m_randomStandbyChange_ms;
        protected int                       m_randomStandbyChangeConst_ms;
        private float                       m_randomStandbyRotation;
        private float                       m_randomStandbyElevation;
        private Vector3                     m_predictedTarget;
        private MyLargeShipWeaponPrediction m_prediction;
        private int                         m_shootDelayIntervalConst_ms;
        private int                         m_shootIntervalConst_ms;
        private int                         m_shootStatusChanged_ms;
        private int                         m_shootDelayInterval_ms;
        private int                         m_shootInterval_ms;
        private int                         m_shootIntervalVarianceConst_ms;
        private bool                        m_onlyPotentialTargetDetected;



        public Matrix                       InitializationMatrix { get; private set; }
        public Matrix                       InitializationBarrelMatrix { get; set; } 
        public MyPrefabLargeWeapon          PrefabParent { get; set; }
        public MyEntity Target;

        // When large ship is controlled, owner is set to player ship which controls this large ship
        public MyEntity WeaponOwner { get; set; }

        // Weapons sounds:
        MySoundCue?[] m_unifiedWeaponCues = new MySoundCue?[MyAudio.GetNumberOfSounds()];

        protected MySoundCuesEnum? m_shootingSound;
        protected MySoundCuesEnum? m_shootingSoundRelease;

        static int m_intervalShift = 0; //use this to avoid updating all weapons in one frame

        public override bool Enabled 
        {
            set
            {
                base.Enabled = value;
                NeedsUpdate = value;
                if (!Enabled) 
                {
                    StopShootingSound();
                    StopAimingSound();
                    m_barrel.RemoveSmoke();
                }
            }
        }
        private bool m_playerFire = false;

        #endregion

        public enum MyLargeShipGunStatus
        {
            MyWeaponStatus_Deactivated,
            MyWeaponStatus_Searching,
            MyWeaponStatus_Shoothing,
            MyWeaponStatus_ShootDelaying,
        }

        public override MyMwcObjectBuilder_FactionEnum Faction
        {
            get
            {
                return PrefabParent.Faction;
            }
            set
            {
                //  System.Diagnostics.Debug.Assert(false, "Prefab always takes faction from container");
            }
        }

        public MyLargeShipGunBase()
            :base()
        {
            m_status = MyLargeShipGunStatus.MyWeaponStatus_Deactivated;
            m_predictionInterval_ms = 0;
            m_predictionIntervalConst_ms = 250;
            m_checkTargetInterval_ms = 0;
            m_checkTargetIntervalConst_ms = 150;
            m_randomStandbyChange_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_randomStandbyChangeConst_ms = 4000;
            m_randomStandbyRotation = 0.0f;
            m_randomStandbyElevation = 0.0f;
            m_rotation = 0.0f;
            m_elevation = 0.0f;
            m_rotationSpeed = MyLargeShipWeaponsConstants.ROTATION_SPEED;
            m_elevationSpeed = MyLargeShipWeaponsConstants.ELEVATION_SPEED;
            m_rotationInterval_ms = 0;
            m_elevationInterval_ms = 0;
            m_predictedTarget = Vector3.Zero;
            m_prediction = null;
            m_shootDelayIntervalConst_ms = 200;
            m_shootIntervalConst_ms = 1200;
            m_shootIntervalVarianceConst_ms = 500;
            m_shootStatusChanged_ms = 0;
            m_onlyPotentialTargetDetected = false;

            //NeedsUpdate = true;
        }

        public virtual void Init(StringBuilder hudLabelText, MyEntity parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_Base objectBuilder)
        {
        }

        public override void Init(StringBuilder hudLabelText, MyModelsEnum? modelEnum, MyMaterialType materialType,
            MyEntity parentObject, Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_Base objectBuilder, MyModelsEnum? collisionModel = null)
        {
            base.Init(null, modelEnum, materialType, parentObject, position, forwardVector, upVector, null, collisionModel);
            //InitSpherePhysics(MyMaterialType.METAL, ModelLod0, 9999999.0f, 1.0f, MyConstants.COLLISION_LAYER_ALL, RigidBodyFlag.RBF_RBO_STATIC);
            //if (this.Physics != null)
            //{
            //    this.Physics.Enabled = true;
            //    this.Physics.Update();
            //}

            InitTrianglePhysics(materialType, 9999999.0f, ModelCollision, null);

            m_checkTargetInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_predictionInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_rotationInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_elevationInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            m_prediction = new MyLargeShipWeaponPrediction();

            InitializationMatrix = LocalMatrix;
            InitializationBarrelMatrix = Matrix.Identity;
        }

        Matrix InitializationMatrixWorld
        {
            get
            {
                return InitializationMatrix * Parent.WorldMatrix;
            }
        }

        public void SetRandomRotation()
        {
            // Start with random rotation
            m_rotation = MyMwcUtils.GetRandomFloat(0, MathHelper.Pi*2);
            RotateModels();
        }

        public void SetRotation(float angle, float elevation)
        {
            m_rotation = angle;
            m_elevation = elevation;
            RotateModels();
        }

        public void MountBarrel(MyLargeShipBarrelBase barrel)
        {
            m_barrel = barrel;
        }

        public MyLargeShipGunStatus GetStatus()
        {
            return m_status;
        }

        public override void UpdateBeforeSimulation()
        {
            // Don't call base, children don't use UpdateBeforeSimulation
            //base.UpdateBeforeSimulation();
        }

        private bool HasElevationOrRotationChanged()
        {
            if(Math.Abs(m_rotationLast - m_rotation) > MyLargeShipWeaponsConstants.ROTATION_AND_ELEVATION_MIN_CHANGE)
            {
                return true;
            }
            if (Math.Abs(m_elevationLast - m_elevation) > MyLargeShipWeaponsConstants.ROTATION_AND_ELEVATION_MIN_CHANGE)
            {
                return true;
            }
            return false;
        }

        private float m_stopShootingTime = 0;

        //Shooting must be handled here because only here is the correct start position for projectile (because of physics)
        public override void UpdateAfterSimulation()
        {
            System.Diagnostics.Debug.Assert(!Closed);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLargeShipGunBase::UpdateAfterSimulation");

            float maxActiveDistanceSq = 3500 * 3500;
            var distance = MySession.Static.DistanceToPlayersSquared(this);
            bool active = IsActive();

            bool isRemoteControlled = MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsLockedByOtherPlayer(this.PrefabParent);
            bool isControlledByPlayer = MyGuiScreenGamePlay.Static.IsControlledByPlayer(PrefabParent);

            if ((distance < maxActiveDistanceSq || (isRemoteControlled || isControlledByPlayer)) && active && Enabled && MyLargeShipWeaponsConstants.Enabled && this.PrefabParent.IsWorking())
            {
                MyPerformanceCounter.PerCameraDraw.Increment("Active large weapons");
                
                if (isControlledByPlayer && MyMultiplayerGameplay.IsRunning)
                {
                    MyMultiplayerGameplay.Static.UpdateRotationFast(this.PrefabParent, new Vector3(m_elevation, m_rotation, 0));
                }

                if (isControlledByPlayer || isRemoteControlled)
                {
                    base.UpdateAfterSimulation();
                    UpdateControlledWeapon();
                }
                else
                {
                    float activationDistance = m_barrel.SearchingDistance * 1.3f * m_barrel.SearchingDistance * 1.3f;

                    if (distance < activationDistance)
                    {
                        MyPerformanceCounter.PerCameraDraw.Increment("Active large weapons updated by AI");

                        base.UpdateAfterSimulation();
                        UpdateAiWeapon();
                    }
                }
            }

            if (!active)
            {
                StopShootingSound();
            }


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void UpdateAiWeapon()
        {
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_checkTargetInterval_ms) > 1500)
            {
                CheckNearTargets();

                m_checkTargetInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }

            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_predictionInterval_ms) > m_predictionIntervalConst_ms && Target != null)
            {
                UpdatePrediction(MyMinerGame.TotalGamePlayTimeInMilliseconds);

                m_predictionInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }


            float targetDistance = GetTargetDistance();

            if (targetDistance < m_barrel.SearchingDistance)
            {
                bool isAimed = RotationAndElevation() && Target != null;

                if (isAimed && !m_onlyPotentialTargetDetected)
                {
                    UpdateShootStatus();

                    if (m_status == MyLargeShipGunStatus.MyWeaponStatus_Shoothing)
                    {
                        // Dummy weapons can't shoot
                        if (!IsDummy)
                        {
                            m_canStopShooting = m_barrel.StartShooting();
                            LastShotId = m_barrel.LastShotId;
                            if (MyMultiplayerGameplay.IsRunning)
                            {
                                MyMultiplayerGameplay.Static.Shoot(this.Parent, this.GetBarell().WorldMatrix, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic, GetTarget(), LastShotId);
                            }
                        }
                    }
                    else
                    {
                        if (m_canStopShooting || (m_shootingSound.HasValue && UnifiedWeaponCueGet(m_shootingSound.Value).HasValue && UnifiedWeaponCueGet(m_shootingSound.Value).Value.IsPlaying))
                        {
                            m_barrel.StopShooting();
                            m_canStopShooting = false;
                        }
                    }
                }
                else
                {
                    m_status = MyLargeShipGunStatus.MyWeaponStatus_Searching;
                    if (m_canStopShooting || (m_shootingSound.HasValue && UnifiedWeaponCueGet(m_shootingSound.Value).HasValue && UnifiedWeaponCueGet(m_shootingSound.Value).Value.IsPlaying))
                    {
                        m_barrel.StopShooting();
                        m_canStopShooting = false;
                    }
                }
            }
            else
            {
                m_status = MyLargeShipGunStatus.MyWeaponStatus_Deactivated;
                StopAimingSound();
                if (m_canStopShooting || (m_shootingSound.HasValue && UnifiedWeaponCueGet(m_shootingSound.Value).HasValue && UnifiedWeaponCueGet(m_shootingSound.Value).Value.IsPlaying))
                {
                    m_barrel.StopShooting();
                    m_canStopShooting = false;
                }

                if (MyCamera.GetDistanceWithFOV(Vector3.Distance(MyCamera.Position, GetPosition())) <= MyLodConstants.MAX_DISTANCE_FOR_RANDOM_ROTATING_LARGESHIP_GUNS)
                {
                    RandomMovement();
                }
            }
        }

        private void UpdateControlledWeapon()
        {
            MySoundCue? movingSound = UnifiedWeaponCueGet(MySoundCuesEnum.WepLargeShipAutocannonRotate);

            if (HasElevationOrRotationChanged())
            {
                if (movingSound == null || movingSound.Value.IsPlaying == false)
                {
                    UnifiedWeaponCueSet(MySoundCuesEnum.WepLargeShipAutocannonRotate,
                        MyAudio.AddCue2dOr3d(PrefabParent, Audio.MySoundCuesEnum.WepLargeShipAutocannonRotate, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                    //MyAudio.AddCue3D(MySoundCuesEnum.WepLargeShipAutocannonRotate, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                }
                m_stopShootingTime = 0;
            }
            else
            {
                if (m_stopShootingTime <= 0)
                {
                    m_stopShootingTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                }
                else
                {
                    if (m_stopShootingTime + MyLargeShipWeaponsConstants.AIMING_SOUND_DELAY < MyMinerGame.TotalGamePlayTimeInMilliseconds)
                    {
                        StopAimingSound();
                    }
                }
            }

            m_rotationLast = m_rotation;
            m_elevationLast = m_elevation;

            RotateModels();


            if (m_playerFire/* && m_status != MyLargeShipGunStatus.MyWeaponStatus_Shoothing*/)
            {
                var old = m_barrel.IsDummy;
                this.WeaponOwner = MySession.PlayerShip;
                m_barrel.IsDummy = false;
                m_barrel.StartShooting();
                m_barrel.IsDummy = old;
                this.WeaponOwner = null;
                m_status = MyLargeShipGunStatus.MyWeaponStatus_Shoothing;

                LastShotId = m_barrel.LastShotId;
                if (MyMultiplayerGameplay.IsRunning)
                {
                    MyMultiplayerGameplay.Static.Shoot(this.Parent, this.GetBarell().WorldMatrix, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic, null, LastShotId);
                }
            }

            if (!m_playerFire && m_status == MyLargeShipGunStatus.MyWeaponStatus_Shoothing)
            {
                m_barrel.StopShooting();
                m_status = MyLargeShipGunStatus.MyWeaponStatus_Searching;
            }

            //m_playerFire = false;
        }

        private bool m_canStopShooting = false;

        private void SetShootInterval(ref int shootInterval, ref int shootIntervalConst)
        {
            shootInterval = shootIntervalConst + MyMwcUtils.GetRandomInt(-m_shootIntervalVarianceConst_ms, m_shootIntervalVarianceConst_ms);
        }

        private void UpdateShootStatus()
        {
            switch (m_status)
            {
                case MyLargeShipGunStatus.MyWeaponStatus_Shoothing:
                    {
                        if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_shootStatusChanged_ms) > m_shootInterval_ms)
                        {
                            StartShootDelaying();
                        }
                    }
                    break;

                case MyLargeShipGunStatus.MyWeaponStatus_ShootDelaying:
                    {
                        if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_shootStatusChanged_ms) > m_shootDelayInterval_ms)
                        {
                            StartShooting();
                        }
                    }
                    break;

                case MyLargeShipGunStatus.MyWeaponStatus_Searching:
                case MyLargeShipGunStatus.MyWeaponStatus_Deactivated:
                    {
                        StartShootDelaying();
                    }
                    break;
            }
        }

        private void StartShooting()
        {
            m_status = MyLargeShipGunStatus.MyWeaponStatus_Shoothing;
            m_shootStatusChanged_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            SetShootInterval(ref m_shootInterval_ms, ref m_shootIntervalConst_ms);
        }

        private void StartShootDelaying()
        {
            m_status = MyLargeShipGunStatus.MyWeaponStatus_ShootDelaying;
            m_shootStatusChanged_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            SetShootInterval(ref m_shootDelayInterval_ms, ref m_shootDelayIntervalConst_ms);
        }

        private void ResetRandomAiming()
        {
            m_rotationInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_rotationInterval_ms;
            m_elevationInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_elevationInterval_ms;

            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_randomStandbyChange_ms) > m_randomStandbyChangeConst_ms)
            {
                m_randomStandbyRotation = MyMath.NormalizeAngle(MyMwcUtils.GetRandomFloat(-MathHelper.Pi * 2.0f, MathHelper.Pi * 2.0f));
                m_randomStandbyElevation = MyMath.NormalizeAngle(MyMwcUtils.GetRandomFloat(0.0f, MathHelper.PiOver2));
                m_randomStandbyChange_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }

        public void RandomMovement()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLargeShipGunBase::RandomMovement");

            ResetRandomAiming();

            // real rotation:
            float needRotation = m_randomStandbyRotation;
            float needElevation = m_randomStandbyElevation;
            float step = m_rotationSpeed * m_rotationInterval_ms;
            float diffRot = needRotation - m_rotation;
            float diffRotAbs = Math.Abs(diffRot);

            if (diffRot > float.Epsilon)
            {                          
                float value = MathHelper.Clamp(step, float.Epsilon, needRotation - m_rotation);
                m_rotation += value;
            }
            else if (diffRot < float.Epsilon)
            {
                float value = MathHelper.Clamp(step, float.Epsilon, Math.Abs(needRotation - m_rotation));
                m_rotation -= value; 
            }
                                    
            bool canElevate = false;
            step = m_elevationSpeed * m_elevationInterval_ms;

            if (needElevation > m_barrel.BarrelElevationMin)
            {
                canElevate = true;
            }
            else
            {
                canElevate = false;
            }

            if (canElevate)
            {   
                if (needElevation > m_elevation - float.Epsilon)
                {
                    float value = MathHelper.Clamp(step, float.Epsilon, needElevation - m_elevation);
                    m_elevation += value;
                }
                else if (needElevation < m_elevation + float.Epsilon)
                {
                    float value = MathHelper.Clamp(step, float.Epsilon, Math.Abs(needElevation - m_elevation));
                    m_elevation -= value;
                } 
            }    
            m_elevationInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_rotationInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            // rotate models by rotation & elevation:
            RotateModels();

            // if is properly rotated:
            /*
            if (m_targetEntity != null && !m_targetEntity.IsDead())
            {
                // test intervals of the aiming:
                float stapR = Math.Abs(Math.Abs(needRotation) - Math.Abs(m_rotation));
                float stapE = Math.Abs(Math.Abs(needElevation) - Math.Abs(m_elevation));
                if (stapR <= float.Epsilon && stapE <= float.Epsilon)
                {
                    m_randomStandbyChange_ms = 0;
                    ResetRandomAiming();
                }
            } */

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void CheckNearTargets()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLargeShipGunBase::CheckNearTargets");
            MySmallShip nearestShip = null;
            m_onlyPotentialTargetDetected = false;
            PrefabParent.GetClosestSmallShipInSearchDistance(out nearestShip);

            if (nearestShip == null)
            {
                PrefabParent.GetClosestPotentialSmallShipInSearchDistance(out nearestShip);
                m_onlyPotentialTargetDetected = nearestShip != null;
            }
            else
            {
            }
            

            if (nearestShip != Target)
            {
                m_prediction.Clear();
            }

            if (Target == null || Target is MySmallShip) // If we dont have target or we are targeting some ship we can retarget
            {
                Target = nearestShip;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void UpdatePrediction(int Time)
        {
            if (m_status == MyLargeShipGunStatus.MyWeaponStatus_Deactivated)
            {
                m_prediction.Clear();
            }

            if (Target != null)
            {
                m_prediction.AddRecord(Target.GetPosition(), MyMinerGame.TotalGamePlayTimeInMilliseconds);
            }

            m_predictionInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        public float GetTargetDistance()
        {
            if (Target != null)
            {
                return (Target.GetPosition() - m_barrel.GetPosition()).Length();
            }
            return m_barrel.SearchingDistance;
        }

        public float GetActualTargetSpeed()
        {
            if (Target != null)
            {
                return m_prediction.GetLastRecordSpeed();
            }
            return 0.0f;
        }

        public bool IsActive()
        {
            return (!MyGuiScreenGamePlay.Static.IsEditorActive() || MyGuiScreenGamePlay.Static.IsIngameEditorActive()) && IsVisible();
        }

        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            // Ammo is never changed, it uses always same ammo as in init
            m_canStopShooting = m_barrel.StartShooting();
            return m_canStopShooting;
        }

        private void RotateModels()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLargeShipGunBase::RotateModels");

            Matrix m = InitializationMatrixWorld;
            Vector3 trans = m.Translation;

            m *= Matrix.CreateFromAxisAngle(InitializationMatrixWorld.Up, m_rotation);
            //m.Translation = new Vector3(WorldMatrix.Translation.X, WorldMatrix.Translation.Y, WorldMatrix.Translation.Z);
            m.Translation = trans;
            WorldMatrix = m;

            Matrix.CreateRotationX(m_elevation, out m);
            m.Translation = new Vector3(m_barrel.LocalMatrix.Translation.X, m_barrel.LocalMatrix.Translation.Y, m_barrel.LocalMatrix.Translation.Z);
            m_barrel.LocalMatrix = m;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public void SetRotationAndElevation(Vector3 rotationAndElevation)
        {
            m_rotation = rotationAndElevation.Y;
            m_elevation = rotationAndElevation.X;
        }

        public void RotateImmediately(Vector3 target)
        {
            var angles = LookAt(target);
            m_rotation = angles.Y;
            m_elevation = angles.X;
        }

        private Vector3 LookAt(Vector3 Target)
        {
            Matrix m = Matrix.CreateLookAt(m_barrel.GetPosition(), Target, WorldMatrix.Up);

            m = Matrix.Invert(m);
            m = MyMath.NormalizeMatrix(m);
            m *= Matrix.Invert(MyMath.NormalizeMatrix(InitializationMatrixWorld));

            Quaternion rot = Quaternion.CreateFromRotationMatrix(m);
            return MyMath.QuaternionToEuler(rot);
        }

        private int CalculatePredictedTargetTime()
        {
            int predicatedTime = 0;

            // calculate time dif based on the distance & speed of the ammo:
            float targetDistance = GetTargetDistance();
            float targetSpeed = m_prediction.GetLastRecordSpeed();

            MyAmmoProperties ammoProperties = MyAmmoConstants.GetAmmoProperties(m_barrel.GetAmmoType());

            float timeToTarget = targetDistance / ammoProperties.DesiredSpeed;
            timeToTarget *= 1000;

            // ass actual time:
            float predictionStepDiff = m_predictionIntervalConst_ms - (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_predictionInterval_ms);
            predicatedTime = MyMinerGame.TotalGamePlayTimeInMilliseconds + (int)timeToTarget - 100;
            return predicatedTime;
        }

        public Vector3 GetPredictedPosition()
        {
            return m_predictedTarget;
        }

        public bool RotationAndElevation(float rotSpeed = 0.01f)
        {
            bool baseMovingSoundPlay = false;

            Vector3 lookAtPositionEuler = Vector3.Zero;

            if (Target is MySmallShip) // we predict position only for moving targets
            {
                // calculate rotation & elevation in real with speed:
                m_predictedTarget = m_prediction.GetPredictedPosition(CalculatePredictedTargetTime());
            }
            else
            {
                m_predictedTarget = Target.GetPosition();
            }

            if (Target != null)
            {
                lookAtPositionEuler = LookAt(m_predictedTarget);
            }

            // real rotation:
            float needRotation = lookAtPositionEuler.Y;
            float needElevation = lookAtPositionEuler.X;
            float step = m_rotationSpeed * (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_rotationInterval_ms);
            
            float diffRot = needRotation - m_rotation;

            if (diffRot > MathHelper.Pi)
                diffRot = diffRot - MathHelper.TwoPi;
            else
                if (diffRot < -MathHelper.Pi)
                    diffRot = diffRot + MathHelper.TwoPi;
            

            float diffRotAbs = Math.Abs(diffRot);

            //large weapon is too far to calculate rotation and similar mess
            //Cannot use because large weapons then shoot immediatelly
            //bool largeShipTooFar = (Vector3.DistanceSquared(MyGuiScreenGamePlay.Static.ControlledEntity.GetPosition(), this.GetPosition()) > MyLargeShipWeaponsConstants.MAX_ROTATION_UPDATE_DISTANCE * MyLargeShipWeaponsConstants.MAX_ROTATION_UPDATE_DISTANCE);

            bool needUpdateMatrix = false;
            
            if (diffRotAbs > 0.001f)
            {  /*
                if (largeShipTooFar)
                {
                    m_rotation += diffRot; 
                    baseMovingSoundPlay = false;
                }
                else*/
                {
                    float value = MathHelper.Clamp(step, float.Epsilon, diffRotAbs);
                    m_rotation += diffRot > 0 ? value : -value;
                    baseMovingSoundPlay = true;
                }

                needUpdateMatrix = true;
            }
            else
            {
                m_rotation = needRotation;
                baseMovingSoundPlay = false;
            }

            if (m_rotation > MathHelper.Pi)
                m_rotation = m_rotation - MathHelper.TwoPi;
            else
                if (m_rotation < -MathHelper.Pi)
                    m_rotation = m_rotation + MathHelper.TwoPi;


            // real elevation:
            step = m_elevationSpeed * (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_elevationInterval_ms);

            float diffElev = needElevation - m_elevation;
            float diffElevAbs = Math.Abs(diffElev);

            if (needElevation > m_barrel.BarrelElevationMin)
            {
                if (diffElevAbs > 0.001f)
                {     /*
                    if (largeShipTooFar)
                    {
                        m_elevation += diffElev;
                    }
                    else*/
                    {
                        float value = MathHelper.Clamp(step, float.Epsilon, diffElevAbs);
                        m_elevation += diffElev > 0 ? value : -value;
                    }

                    needUpdateMatrix = true;
                }
                else
                {
                    m_elevation = needElevation;
                    //baseMovingSoundPlay = false;
                }
            }
           

            m_elevationInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_rotationInterval_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            if (needUpdateMatrix)
            {
                // rotate models by rotation & elevation:
                RotateModels();
            }

            // Sounds:
            MySoundCue? movingSound = null;
            {
                movingSound = UnifiedWeaponCueGet(MySoundCuesEnum.WepLargeShipAutocannonRotate);

                if (baseMovingSoundPlay)
                {
                    if (movingSound == null || movingSound.Value.IsPlaying == false)
                    {
                        UnifiedWeaponCueSet(MySoundCuesEnum.WepLargeShipAutocannonRotate,
                            MyAudio.AddCue2dOr3d(PrefabParent, Audio.MySoundCuesEnum.WepLargeShipAutocannonRotate, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                            //MyAudio.AddCue3D(MySoundCuesEnum.WepLargeShipAutocannonRotate, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                    }
                }
                else
                {
                    if (movingSound != null)
                    {
                        StopAimingSound();
                    }
                }
            }

            if (movingSound != null && movingSound.Value.IsPlaying)
            {
                MyAudio.UpdateCuePosition(movingSound, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero);
            }
            
            // if is properly rotated:
            if (Target != null && !Target.IsDead())
            {
                // test intervals of the aiming:
                float stapR = Math.Abs(Math.Abs(needRotation) - Math.Abs(m_rotation));
                float stapE = Math.Abs(Math.Abs(needElevation) - Math.Abs(m_elevation));
                if (stapR <= float.Epsilon && stapE <= float.Epsilon)
                {
                    //baseMovingSoundPlay = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public MyEntity GetTarget()
        {
            return Target;
        }

        public override bool DebugDraw()
        {
            if (MyMwcFinalBuildConstants.DrawHelperPrimitives)
            {
                float radius = 0.0f;
                if (ModelLod0 != null)
                    radius = ModelLod0.BoundingSphere.Radius;
                else if (ModelLod1 != null)
                    radius = ModelLod1.BoundingSphere.Radius;

                Vector3 statusColor = new Vector3();
                switch (m_status)
                {
                    case MyLargeShipGunStatus.MyWeaponStatus_Deactivated:
                        {
                            statusColor = Color.Green.ToVector3();
                        }
                        break;
                    case MyLargeShipGunStatus.MyWeaponStatus_Searching:
                        {
                            statusColor = Color.Red.ToVector3();
                        }
                        break;
                    case MyLargeShipGunStatus.MyWeaponStatus_Shoothing:
                        {
                            statusColor = Color.White.ToVector3();
                        }
                        break;
                }
                Color from = new Color(statusColor);
                Color to = new Color(statusColor);
                if (Target != null)
                {
                    MyDebugDraw.DrawLine3D(m_barrel.GetPosition(), Target.GetPosition(), from, to);
                }
                // Try to draw prediction:
                if (Target != null)
                {
                    Color supPreColor = Color.LightSeaGreen;
                    MyDebugDraw.DrawLine3D(m_barrel.GetPosition(), m_predictedTarget, supPreColor, supPreColor);
                }
                m_prediction.DebugDraw(m_barrel.GetPosition(), new Color(statusColor));
            }
            return base.DebugDraw();
        }

        public override bool Draw(MyRenderObject renderObject)
        {
           // return false;
            if (!MyGuiScreenGamePlay.Static.IsControlledByPlayer(PrefabParent))
            {
                return base.Draw(renderObject);
            }
            return true;
        }
        
        public override void Close()
        {
            base.Close();

            // close sounds:
            foreach (MySoundCue? soundCue in m_unifiedWeaponCues)
            {
                if (soundCue != null && soundCue.Value.IsValid)
                {
                    soundCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
            }
            m_unifiedWeaponCues = null;
            m_barrel = null;
        }

        public void UnifiedWeaponCueSet(MySoundCuesEnum cueEnum, MySoundCue? value)
        {
            m_unifiedWeaponCues[(int)cueEnum] = value;
        }

        public MySoundCue? UnifiedWeaponCueGet(MySoundCuesEnum cueEnum)
        {
            return m_unifiedWeaponCues[(int)cueEnum];
        }

        // helping static functions:
        public static StringBuilder GetHudLabelText(MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum type)
        {
            switch (type)
            {
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a01_largeship_autocannon);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a01_largeship_machinegun);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_CIWS:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a01_largeship_ciws);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a01_largeship_missile_gun4);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a01_largeship_missile_gun6);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a01_largeship_missile_gun9);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided4);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided6);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided9);
                    break;
                default:
                    return null;
            }
            return null;
        }

        public static bool GetWeaponModels(MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum type, ref MyModel baseModel, ref MyModel barrelModel)
        {
            switch (type)
            {
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON:
                    baseModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipAutocannonBase);
                    barrelModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipAutocannonBarrel);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_CIWS:
                    baseModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipCiwsBase);
                    barrelModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipCiwsBarrel);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN:
                    baseModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipMachineGunBase);
                    barrelModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipMachineGunBarrel);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4:
                    baseModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipMissileLauncher4Base);
                    barrelModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipMissileLauncher4Barrel);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6:
                    baseModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipMissileLauncher6Base);
                    barrelModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipMissileLauncher6Barrel);
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9:
                    baseModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipMissileLauncher9Base);
                    barrelModel = MyModels.GetModelForDraw(MyModelsEnum.LargeShipMissileLauncher9Barrel);
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static bool GetModelEnums(MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum prefab, out MyModelsEnum baseModel, out MyModelsEnum barrelModel)
        {
            switch (prefab)
            {
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON:
                    baseModel = MyModelsEnum.LargeShipAutocannonBase;
                    barrelModel = MyModelsEnum.LargeShipAutocannonBarrel;
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_CIWS:
                    baseModel = MyModelsEnum.LargeShipCiwsBase;
                    barrelModel = MyModelsEnum.LargeShipCiwsBarrel;
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN:
                    baseModel = MyModelsEnum.LargeShipMachineGunBase;
                    barrelModel = MyModelsEnum.LargeShipMachineGunBarrel;
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4:
                    baseModel = MyModelsEnum.LargeShipMissileLauncher4Base;
                    barrelModel = MyModelsEnum.LargeShipMissileLauncher4Barrel;
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6:
                    baseModel = MyModelsEnum.LargeShipMissileLauncher6Base;
                    barrelModel = MyModelsEnum.LargeShipMissileLauncher6Barrel;
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9:
                    baseModel = MyModelsEnum.LargeShipMissileLauncher9Base;
                    barrelModel = MyModelsEnum.LargeShipMissileLauncher9Barrel;
                    break;
                //case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P351_A01_WEAPON_MOUNT:
                //    baseModel = MyModelsEnum.p351_a01_weapon_mount;
                //    barrelModel = MyModelsEnum.p351_a01_weapon_mount;
                //    break;
                default:
                    baseModel = MyModelsEnum.LargeShipAutocannonBase;
                    barrelModel = MyModelsEnum.LargeShipAutocannonBarrel;
                    return false;
            }
            return true;
        }

        public static bool GetVisualPreviewData(MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum prefab,
            ref MyModel baseModel, ref MyModel barrelModel, ref Matrix baseMatrix, ref Matrix barrelMatrix)
        {
            MyModelsEnum baseModelEnum, barelModelEnum;
            if (!GetModelEnums(prefab, out baseModelEnum, out barelModelEnum))
            {
                return false;
            }

            baseModel = MyModels.GetModelForDraw(baseModelEnum);
            barrelModel = MyModels.GetModelForDraw(barelModelEnum);

            baseMatrix = Matrix.Identity;
            baseMatrix.Translation = -baseModel.BoundingSphere.Center;
            baseMatrix.Translation -= new Vector3(0, 0.2f * baseModel.BoundingSphere.Radius, 0);
            baseMatrix *= Matrix.CreateRotationY(MathHelper.PiOver2 + MathHelper.PiOver4 + MathHelper.PiOver4 * 0.2f);
            baseMatrix *= Matrix.CreateRotationX(.35f * MathHelper.PiOver4);
            float distanceMultiplier = baseModelEnum == MyModelsEnum.LargeShipCiwsBase ? 3.0f : 2.3f;
            baseMatrix.Translation += new Vector3(0.0f, 0.0f, -baseModel.BoundingSphere.Radius * distanceMultiplier);
            barrelMatrix = baseMatrix;
            Matrix barrelMatrixLocal = baseModel.Dummies.ContainsKey("axis") ? MyMath.NormalizeMatrix(baseModel.Dummies["axis"].Matrix) : Matrix.Identity;
            barrelMatrix.Translation += barrelMatrixLocal.Translation;

            return true;
        }

        public static void CreateAloneWeapon(ref MyLargeShipGunBase weapon, string hudLabelText, Vector3 position, Matrix localOrientation,
            MyMwcObjectBuilder_PrefabLargeWeapon objectBuilder, bool activated)
        {
            StringBuilder sb = new StringBuilder("Large weapon");
            MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum tmp = (MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum)objectBuilder.GetObjectBuilderId().Value;
            switch (tmp)
            {
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN:
                    weapon = new MyLargeShipMachineGun();
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_CIWS:
                    weapon = new MyLargeShipCIWS();
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON:
                    weapon = new MyLargeShipAutocannon();
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9:
                    weapon = new MyLargeShipMissileLauncherGun();
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                    break;
            }

            weapon.Init(null, null, position, localOrientation.Forward, localOrientation.Up, objectBuilder);

            Matrix world = Matrix.CreateWorld(position, localOrientation.Forward, localOrientation.Up);
            weapon.WorldMatrix = world;

            if (activated)
            {
                weapon.PersistentFlags &= ~MyPersistentEntityFlags.Deactivated;
                weapon.GetBarell().PersistentFlags &= ~MyPersistentEntityFlags.Deactivated;
            }
            else
            {
                weapon.PersistentFlags |= MyPersistentEntityFlags.Deactivated;
                weapon.GetBarell().PersistentFlags |= MyPersistentEntityFlags.Deactivated;
            }
        }

        public static void GetMissileAmmoParams(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType, ref int MissileShotInterval)
        {
            MissileShotInterval = MyMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
            switch (AmmoType)
            {
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic:
                    break;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection:
                    MissileShotInterval = MyGuidedMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                    break;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection:
                    MissileShotInterval = MyGuidedMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                    break;
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection:
                    MissileShotInterval = MyGuidedMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                    break;
                default:
                    break;
            }
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out MyIntersectionResultLineTriangleEx? t, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            System.Diagnostics.Debug.Assert(!Closed);

            if (base.GetIntersectionWithLine(ref line, out t))
                return true;

            return m_barrel.GetIntersectionWithLine(ref line, out t);
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            System.Diagnostics.Debug.Assert(!Closed);

            if (base.GetIntersectionWithLine(ref line, out v, useCollisionModel))
                return true;

            return m_barrel.GetIntersectionWithLine(ref line, out v, useCollisionModel);
        }

        public override MyEntity GetBaseEntity()
        {
            return PrefabParent;
        }

        public override bool IsThisGunFriendly()
        {
            return ((Parent is MyPrefabBase) &&
                MyFactions.GetFactionsRelation(((Parent as MyPrefabBase).Parent as MyPrefabContainer), MySession.Static.Player) == MyFactionRelationEnum.Friend);
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            if (Parent != null) //Because in objects got for explosion damage can be parent and children together
            {
                Parent.DoDamage(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
            }
        }

        public override float Health
        {
            get
            {
                if (Parent == null) //parent is already dead and removed
                    return 0;

                return Parent.Health; 
            }
            set
            {
                Parent.Health = value;
            }
        }

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);

            if (m_shootingSound.HasValue)
            {
                // update rotation sound position:
                MySoundCue? updateSound = UnifiedWeaponCueGet(m_shootingSound.Value);
                if (updateSound != null)
                {
                    MyAudio.UpdateCuePosition(updateSound, GetPosition(), GetWorldRotation().Forward, GetWorldRotation().Up, Vector3.Zero);
                }
            }
        }

        public void MoveAndRotate(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator, bool afterburner)
        {
            Debug.Assert(MyGuiScreenGamePlay.Static.IsControlledByPlayer(PrefabParent));

            float rotationSpeed = MathHelper.Pi / 180 * 3;
            float elevationSpeed = MathHelper.Pi / 180 * 3;

            m_rotation += -rotationIndicator.Y * rotationSpeed * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            m_elevation = MathHelper.Clamp(m_elevation - rotationIndicator.X * elevationSpeed * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS,
                m_barrel.BarrelElevationMin, MathHelper.PiOver2 - MathHelper.Pi / 180);
        }

        public Matrix GetViewMatrix()
        {
            return m_barrel.GetViewMatrix();
        }

        public void Fire()
        {
            m_playerFire = true;
        }

        public void StopFire() 
        {
            m_playerFire = false;
        }

        public override Vector3 GetHUDMarkerPosition()
        {
            if (m_barrel != null)
            {
                return m_barrel.GetPosition();
            }
            else 
            {
                return GetPosition();
            }
        }

        public MyLargeShipBarrelBase GetBarell()
        {
            return m_barrel;
        }

        internal void SetTarget(MyEntity entity)
        {
            Target = entity;
        }

        internal void PlayShootingSound()
        {
            if (m_shootingSound.HasValue)
            {
                MySoundCue? shootingSound = UnifiedWeaponCueGet(m_shootingSound.Value);
                if (shootingSound == null || !shootingSound.Value.IsPlaying)
                {
                    UnifiedWeaponCueSet(m_shootingSound.Value,
                        MyAudio.AddCue2dOr3d(PrefabParent, m_shootingSound.Value, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                }
            }
        }

        internal void StopShootingSound()
        {
            if (m_shootingSound.HasValue)
            {
                MySoundCue? shootingSound = UnifiedWeaponCueGet(m_shootingSound.Value);
                if (shootingSound != null && shootingSound.Value.IsPlaying)
                {
                    shootingSound.Value.Stop(SharpDX.XACT3.StopFlags.Release);

                    if (m_shootingSoundRelease.HasValue)
                    {
                        UnifiedWeaponCueSet(m_shootingSoundRelease.Value,
                            MyAudio.AddCue2dOr3d(PrefabParent, m_shootingSoundRelease.Value, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
                    }
                }
            }
        }

        internal void StopAimingSound()
        {
            MySoundCue? movingSound = UnifiedWeaponCueGet(MySoundCuesEnum.WepLargeShipAutocannonRotate);

            if (movingSound.HasValue && movingSound.Value.IsPlaying)
            {
                movingSound.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                UnifiedWeaponCueSet(MySoundCuesEnum.WepLargeShipAutocannonRotateRelease,
                    MyAudio.AddCue2dOr3d(PrefabParent, MySoundCuesEnum.WepLargeShipAutocannonRotateRelease, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero));
            }
        }
    }
}
