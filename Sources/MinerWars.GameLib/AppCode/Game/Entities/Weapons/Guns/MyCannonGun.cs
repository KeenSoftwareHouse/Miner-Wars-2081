using System;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;

    class MyCannonGun : MySmallShipGunBase
    {
        int m_lastTimeShoot;                            //  When was this gun last time shooting
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
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < MyCannonConstants.SHOT_INTERVAL_IN_MILISECONDS && !IsDummy) return false;

            //  Throw ship backward (by deviated vector)
            this.Parent.Physics.AddForce(MyPhysicsForceType.ADD_BODY_FORCE_AND_BODY_TORQUE,
                MyUtilRandomVector3ByDeviatingVector.GetRandom(Vector3.Backward, MathHelper.ToRadians(5)) * MyMwcUtils.GetRandomFloat(40000, 50000), null, null);

            //  Play missile launch cue (one-time)
            //AddWeaponCue(MySoundCuesEnum.WepMissileLaunch3d);
            AddWeaponCue(MySoundCuesEnum.WepCannon3d);

            Vector3 forwardVelocity = MyMath.ForwardVectorProjection(this.WorldMatrix.Forward, GetParentMinerShip().Physics.LinearVelocity);
            //Vector3 forwardVelocity = this.WorldMatrix.Forward;

            Vector3 deviatedVector = GetDeviatedVector(MinerWars.AppCode.Game.Gameplay.MyAmmoConstants.GetAmmoProperties(usedAmmo.AmmoType));

            var cannonShotStartPos = GetPosition() + WorldMatrix.Forward * WorldVolume.Radius;
            cannonShotStartPos = CorrectPosition(cannonShotStartPos, deviatedVector, Parent);

            //  Create and fire missile - but deviate missile direction be random angle
            MyCannonShots.Add(cannonShotStartPos, forwardVelocity, deviatedVector, usedAmmo, (MySmallShip) Parent);

            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            m_lastSmokePosition = GetSmokePosition();

            //  We shot one missile
            return true;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {                
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);                
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
            var predictedTrajectory = new MyLine(
                position - MyMissileConstants.DISTANCE_TO_CHECK_MISSILE_CORRECTION * direction,
                position + MyMissileConstants.DISTANCE_TO_CHECK_MISSILE_CORRECTION * direction,
                false);
            var intersection = MyEntities.GetIntersectionWithLine(ref predictedTrajectory, this, viewerEntity);
            if (intersection != null)
            {
                position = viewerEntity.GetPosition();
            }
            return position;
        }
    }
}