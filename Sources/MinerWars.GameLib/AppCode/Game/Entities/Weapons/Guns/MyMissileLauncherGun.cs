using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

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
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using Models;
    using Utils;

    class MyMissileLauncherGun : MySmallShipGunBase
    {
        int m_lastTimeShoot;                            //  When was this gun last time shooting
        //int m_lastTimeSmoke;                            //  When was this gun last time generated smoke
        Vector3 m_lastSmokePosition;


        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.MissileLauncher, MyMaterialType.METAL, parentObject,
                position, forwardVector, upVector, objectBuilder);

            m_lastTimeShoot = MyConstants.FAREST_TIME_IN_PAST;
        }

        //  Every child of this base class must implement Shot() method, which shots projectile or missile.
        //  Method returns true if something was shot. False if not (because interval between two shots didn't pass)
        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            LastShotId = null;
            float missileLauncherShotInterval = float.MaxValue;

            switch (usedAmmo.AmmoType)
            {
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection:
                    missileLauncherShotInterval = MyGuidedMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < missileLauncherShotInterval && !IsDummy)
                return false;

            //  Throw ship backward (by deviated vector)
            this.Parent.Physics.AddForce(MyPhysicsForceType.ADD_BODY_FORCE_AND_BODY_TORQUE,
                MyUtilRandomVector3ByDeviatingVector.GetRandom(Vector3.Backward, MathHelper.ToRadians(5)) * MyMwcUtils.GetRandomFloat(40000, 50000), null, null);

            //  Play missile launch cue (one-time)
            AddWeaponCue(MySoundCuesEnum.WepMissileLaunch3d);

            Vector3 forwardVelocity = MyMath.ForwardVectorProjection(this.WorldMatrix.Forward, GetParentMinerShip().Physics.LinearVelocity);


            //  Create and fire missile - but deviate missile direction be random angle
            Vector3 deviatedVector = GetDeviatedVector(MinerWars.AppCode.Game.Gameplay.MyAmmoConstants.GetAmmoProperties(usedAmmo.AmmoType));

            // correct missile start position if it would intersect some entity
            Vector3 missileStartPosition = m_positionMuzzleInWorldSpace;
            missileStartPosition = CorrectPosition(missileStartPosition, deviatedVector, Parent);

            if (MinerWars.AppCode.Game.Managers.Session.MySession.Static.Is2DSector)
            {
                forwardVelocity.Y = 0;
                deviatedVector.Y = 0;
            }

            var missile = MyMissiles.Add(usedAmmo.AmmoType, missileStartPosition, forwardVelocity, deviatedVector, LocalMatrix.Translation, Parent, ((MySmallShip)Parent).TargetEntity, isDummy: this.IsDummy);
            if (missile != null)
            {
                missile.EntityId = MyEntityIdentifier.AllocateId();
                MyEntityIdentifier.AddEntityWithId(missile);
                LastShotId = missile.EntityId;
                GetParentMinerShip().LastMissileFired = missile;
            }
            //MyUtilRandomVector3ByDeviatingVector.GetRandom(WorldMatrix.Forward

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_lastSmokePosition = GetSmokePosition();

            if (SysUtils.MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE)
            {
                MinerWars.AppCode.Game.Trailer.MyTrailerSave.UpdateGunShot(this.Parent, Trailer.MyTrailerGunsShotTypeEnum.MISSILE);
            }

            //  We shot one missile
            return true;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher);
            }
            return objectBuilder;
        }

        Vector3 GetSmokePosition()
        {
            return m_positionMuzzleInWorldSpace - WorldMatrix.Forward * 0.5f;
        }


        /// <summary>
        /// Check if the missile does not collide too close to ship
        /// after shooting and correct its starting position if it does.
        /// </summary>
        private Vector3 CorrectPosition(Vector3 position, Vector3 direction, MyEntity viewerEntity)
        {
            //var predictedTrajectory = new MyLine(
            //    position - (MyMissileConstants.DISTANCE_TO_CHECK_MISSILE_CORRECTION) * direction,
            //    position + (MyMissileConstants.DISTANCE_TO_CHECK_MISSILE_CORRECTION) * direction,
            //    true);
            //var intersection = MyEntities.GetIntersectionWithLine(ref predictedTrajectory, this, viewerEntity);


            // This is fix for missles hitting voxels when you are near them (missles do not use per triangle collision as optimalization)
            float radius = MyModels.GetModel(MyModelsEnum.Missile).BoundingSphere.Radius;

            BoundingSphere boundingSphere = viewerEntity.WorldVolume;
            boundingSphere.Center += viewerEntity.WorldMatrix.Up * 1;

            var intersection = MyEntities.GetIntersectionWithSphere(ref boundingSphere, this, viewerEntity);

            if (intersection != null && !(intersection is MyMissile))
            {
                position = viewerEntity.GetPosition() + 2 * Vector3.One;
            }

            return position;
        }

    }
}
