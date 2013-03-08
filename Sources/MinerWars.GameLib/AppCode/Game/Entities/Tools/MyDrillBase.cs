using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Voxels;

namespace MinerWars.AppCode.Game.Entities
{
    using System;
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWarsMath;
    
    using Models;
    using TransparentGeometry.Particles;
    using Utils;
    using Weapons;
    using Sessions;
    using CommonLIB.AppCode.Networking.Multiplayer;
    using MinerWars.AppCode.Game.GUI;
    using MinerWars.AppCode.Game.Managers.Session;

    public enum MyDrillStateEnum
    {
        Drilling,       //  Drill is ejected and ship is drilling (shooting)
        Activated,      //  Drill was just activated and it starts ejecting and rotating
        Deactivated,    //  Drill was just deacivated and it starts pulling back and stops rotating
        InsideShip,     //  Drill is inside miner ship - default state
    }


    /// <summary>
    /// Base class for drilling devices
    /// </summary>
    abstract class MyDrillBase : MySmallShipGunBase
    {
        protected int m_lastTimeStarted;
        protected int m_lastTimeEjected;
        private MyDrillStateEnum m_currentState;

        protected MySoundCue? m_movingCue;
        protected MySoundCue? m_movingCueRelease;
        protected MySoundCue? m_drillCue;
        protected MySoundCue? m_drillCueRelease;
        protected MySoundCue? m_idleCue;
        protected MySoundCue? m_voxelCutCue;

        protected MySoundCuesEnum? m_movingCueEnum;
        protected MySoundCuesEnum? m_movingCueReleaseEnum;
        protected MySoundCuesEnum? m_drillCueEnum;
        protected MySoundCuesEnum? m_drillCueReleaseEnum;
        protected MySoundCuesEnum? m_drillOtherCueEnum;
        protected MySoundCuesEnum? m_drillOtherCueReleaseEnum;
        protected MySoundCuesEnum? m_idleCueEnum;

        protected MyModelsEnum? m_model;
        protected float m_range;
        protected float m_radius;
        protected float m_damage; // can mean dps or damage per hit, depends on type of drill

        protected MyParticleEffect m_dustEffect;
        protected float m_ejectedDistance;
        protected float m_maxEjectDistance;
        protected bool m_fullyEjected;
        protected int m_minDrillingDuration; // in milliseconds

        readonly Matrix m_localMatrixForDrillCockpit;
        float m_drillScale = 1;

        protected MyDrillBase()
        {
            // Here we set a constant value for the matrix that is used as the local transform
            // for cockpit-view of all drills
            m_localMatrixForDrillCockpit = new Matrix(
                -1, 0, 0, 0,
                0, -1, 0, 0,
                0, 0, 1, 0,
                0, -2.024051f, -2.577045f, 1);
        }

        public MyDrillStateEnum CurrentState
        {
            set
            {
                if (m_currentState != value)
                {
                    m_currentState = value;
                    Visible = m_currentState != MyDrillStateEnum.InsideShip;

                    if (MyMultiplayerGameplay.IsRunning && !IsDummy)
                    {
                        switch (m_currentState)
                        {
                            case MyDrillStateEnum.Activated:
                                MyMultiplayerGameplay.Static.SpeacialWeaponEvent(MySpecialWeaponEventEnum.DRILL_ACTIVATED, this.WeaponType);
                                break;

                            case MyDrillStateEnum.Deactivated:
                                MyMultiplayerGameplay.Static.SpeacialWeaponEvent(MySpecialWeaponEventEnum.DRILL_DEACTIVATED, this.WeaponType);
                                break;

                            case MyDrillStateEnum.Drilling:
                                MyMultiplayerGameplay.Static.SpeacialWeaponEvent(MySpecialWeaponEventEnum.DRILL_DRILLING, this.WeaponType);
                                break;
                        }
                    }
                }
            }
            get
            {
                return m_currentState;
            }

        }

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            Init(hudLabelText, m_model, MyMaterialType.METAL, parentObject,
                position, forwardVector, upVector, objectBuilder);

            m_lastTimeStarted = MyConstants.FAREST_TIME_IN_PAST;
            m_lastTimeEjected = MyConstants.FAREST_TIME_IN_PAST;

            CurrentState = MyDrillStateEnum.InsideShip;

            m_maxEjectDistance = MyDrillDeviceConstants.EJECT_DISTANCE_MULTIPLIER * ModelLod0.BoundingBoxSize.Z;
            m_ejectedDistance = 0;
        }


        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (MySession.Is25DSector)
            {
                CurrentState = MyDrillStateEnum.Activated;
            }

            if (!m_fullyEjected || GetParentMinerShip().Fuel <= 0 || !GetParentMinerShip().Config.Engine.On)
                return false;

            m_lastTimeStarted = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            CurrentState = MyDrillStateEnum.Drilling;

            return true;
        }

        public virtual bool Eject()
        {
            var ejectedLongAgoEnough = (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeEjected) >
                                       MyDrillDeviceConstants.DRILL_EJECT_INTERVAL_IN_MILISECONDS;

            if (CurrentState == MyDrillStateEnum.InsideShip && ejectedLongAgoEnough)
            {
                CurrentState = MyDrillStateEnum.Activated;
                m_lastTimeEjected = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                SetDrillScale(0.01f);
                this.Visible = false;
                return true;
            }

            if (CurrentState == MyDrillStateEnum.Activated && m_fullyEjected && ejectedLongAgoEnough)
            {
                CurrentState = MyDrillStateEnum.Deactivated;
                m_lastTimeEjected = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }

            return false;
        }

        void SetDrillScale(float scale)
        {
            m_drillScale = Math.Max(scale, 0.01f);
            Scale = scale;
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            // HACK: Multiplayer
            if (IsDummy && CurrentState == MyDrillStateEnum.Drilling)
            {
                Shot(null);
            }

            switch (CurrentState)
            {
                case MyDrillStateEnum.InsideShip:
                    // If drill isnt active we stop sound that might not be stopped before
                    StopDustEffect();
                    StopIdleCue();
                    return;
                case MyDrillStateEnum.Deactivated:
                    PullDrillBack();
                    return;
                case MyDrillStateEnum.Activated:
                    EjectDrill();
                    break;
                case MyDrillStateEnum.Drilling:
                    Drill();
                    break;
            }

            if ((m_drillCue != null) && m_drillCue.Value.IsPlaying)
            {
                MyAudio.UpdateCuePosition(m_drillCue, WorldMatrix.Translation,
                    WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
            }
                 
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeStarted) > 100)
            {
                CurrentState = MyDrillStateEnum.Activated;
                StopDrillingCue();
                StopDustEffect();
                //StopMovingCue();
            }    
        }

        protected void StopDustEffect()
        {
            if (m_dustEffect != null)
            {
                m_dustEffect.Stop();
                m_dustEffect = null;
            }
        }

        protected abstract void Drill();

        protected virtual void EjectDrill()
        {
            this.Visible = true;
            m_ejectedDistance += MyDrillDeviceConstants.DRILL_EJECTING_SPEED;
            if (m_ejectedDistance < m_maxEjectDistance)
            {
                SetDrillScale(m_ejectedDistance / m_maxEjectDistance);
            }
            else
            {
                m_ejectedDistance = m_maxEjectDistance;
                if (!m_fullyEjected)
                {
                    StartIdleCue();
                }
                m_fullyEjected = true;
                
                SetDrillScale(1);
            }
        }

        protected virtual void PullDrillBack()
        {
            StopDrillingCue();
            StopMovingCue();

            m_fullyEjected = false;
            m_ejectedDistance -= MyDrillDeviceConstants.DRILL_EJECTING_SPEED;
            if (m_ejectedDistance > 0.01f)
            {
                SetDrillScale(m_ejectedDistance / m_maxEjectDistance);
            }
            else
            {
                m_ejectedDistance = 0;
                CurrentState = MyDrillStateEnum.InsideShip;
            }
        }

        protected void StartIdleCue()
        {
            if (m_idleCueEnum != null && (m_idleCue == null || m_idleCue.Value.IsPlaying == false))
            {
                m_idleCue = MyAudio.AddCue2dOr3d(Parent, m_idleCueEnum.Value, WorldMatrix.Translation,
                    WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
            }
            /*if ((m_drillCueRelease != null) && m_drillCueRelease.Value.IsPlaying)
            {
                m_drillCueRelease.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }*/
            StopDrillingCue();
            
        }

        protected void StopIdleCue()
        {
            if ((m_idleCue != null) && m_idleCue.Value.IsPlaying)
            {
                if (!MySession.Is25DSector)
                    m_idleCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
        }

        protected void StartDrillingCue(bool mining)
        {
            if (!mining && (m_drillOtherCueEnum.HasValue && (m_drillCue == null || !m_drillCue.Value.IsPlaying)))
            {
                m_drillCue = MyAudio.AddCue2dOr3d(Parent, m_drillOtherCueEnum.Value, WorldMatrix.Translation,
                    WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
            }
            else if (m_drillCueEnum.HasValue && (m_drillCue == null || !m_drillCue.Value.IsPlaying) || MySession.Is25DSector)
            {
                if (!MySession.Is25DSector)
                {
                    m_drillCue = MyAudio.AddCue2dOr3d(Parent, m_drillCueEnum.Value, WorldMatrix.Translation,
                        WorldMatrix.Forward, WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
                }
            }    
            StopIdleCue();
        }

        protected void StartMovingCue()
        {
            if (m_movingCueEnum != null && (m_movingCue == null) || !m_movingCue.Value.IsPlaying)
            {
                m_movingCue = MyAudio.AddCue2dOr3d(Parent, m_movingCueEnum.Value,
                    Parent.GetPosition(), Parent.WorldMatrix.Forward, Parent.WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }
            if ((m_movingCueRelease != null) && m_movingCueRelease.Value.IsPlaying)
            {
                m_movingCueRelease.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
            StopIdleCue();
        }

        protected void StopDrillingCue()
        {
            if (m_drillCue != null && m_drillCue.Value.IsPlaying)
            {
                m_drillCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                if (m_drillCueReleaseEnum != null)
                {
                    MySoundCuesEnum releaseCue;
                    if ( (m_drillCue.Value.CueEnum == m_drillOtherCueEnum || 
                          m_drillCue.Value.CueEnum == MyAudio.GetVersion2D(m_drillOtherCueEnum.Value)) &&
                        m_drillOtherCueReleaseEnum.HasValue)
                    {
                        releaseCue = m_drillOtherCueReleaseEnum.Value;
                    }
                    else
                    {
                        releaseCue = m_drillCueReleaseEnum.Value;
                    }

                    if (m_drillCueRelease == null || !m_drillCueRelease.Value.IsPlaying)
                    {
                        m_drillCueRelease = MyAudio.AddCue2dOr3d(Parent, releaseCue, WorldMatrix.Translation, WorldMatrix.Forward,
                            WorldMatrix.Up, this.Parent.Physics.LinearVelocity);
                    }
                }
            }
        }

        protected void StopMovingCue()
        {
            if ((m_movingCue != null)/* && m_movingCue.Value.IsPlaying*/)
            {
                m_movingCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                if (m_movingCueReleaseEnum != null)
                {
                    m_movingCueRelease = MyAudio.AddCue2dOr3d(Parent, m_movingCueReleaseEnum.Value,
                        Parent.GetPosition(), Parent.WorldMatrix.Forward, Parent.WorldMatrix.Up, Parent.Physics.LinearVelocity);
                }
            }
        }

        public override void StopAllSounds()
        {
            base.StopAllSounds();

            if ((m_movingCue != null) && m_movingCue.Value.IsPlaying)
            {
                m_movingCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            if ((m_movingCueRelease != null) && m_movingCueRelease.Value.IsPlaying)
            {
                m_movingCueRelease.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            if ((m_drillCue != null) && m_drillCue.Value.IsPlaying)
            {
                m_drillCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            if ((m_drillCueRelease != null) && m_drillCueRelease.Value.IsPlaying)
            {
                m_drillCueRelease.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            if ((m_idleCue != null) && m_idleCue.Value.IsPlaying)
            {
                m_idleCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            if ((m_voxelCutCue != null) && m_voxelCutCue.Value.IsPlaying)
            {
                m_voxelCutCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
        }

        public override void Close()
        {
            //  Stop sound cues
            StopAllSounds();

            StopDustEffect();

            base.Close();
        }

        protected virtual float GetRadiusNeededForTunel()
        {
            return (Vector3.Distance(Parent.WorldVolume.Center, this.WorldMatrix.Translation) + ModelLod0.BoundingSphere.Radius)
                * (MySession.Is25DSector ? MyDrillDeviceConstants.BIG_SPHERE_RADIUS_MULTIPLIER * 1.8f : MyDrillDeviceConstants.BIG_SPHERE_RADIUS_MULTIPLIER);
        }

        protected static void CreateImpactEffect(Vector3 position, Vector3 normal, MyParticleEffectsIDEnum effectID)
        {
            var effect = MyParticlesManager.CreateParticleEffect((int)effectID);
            Vector3 tangent;
            MyUtils.GetPerpendicularVector(ref normal, out tangent);
            effect.WorldMatrix = Matrix.CreateWorld(position, normal, tangent);
        }

        protected override Matrix GetLocalMatrixForCockpitView()
        {
            return Matrix.CreateScale(m_drillScale) * Matrix.CreateTranslation(((MySmallShip)Parent).GetHeadPosition()) * m_localMatrixForDrillCockpit;
        }

        void PlayVoxelCutCue()
        {
            if (m_voxelCutCue == null || !m_voxelCutCue.Value.IsPlaying || MySession.Is25DSector)
            {
                m_voxelCutCue = MyAudio.AddCue2dOr3d(Parent, MySoundCuesEnum.SfxVoxelCrack, Parent.GetPosition(), Parent.WorldMatrix.Forward, Parent.WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }
        }
        
        protected void CutOutFromVoxel(MyVoxelMap voxelMap, ref BoundingSphere bigSphereForTunnel)
        {
            if (!IsDummy)
            {
                if (MyMultiplayerGameplay.IsRunning)
                {
                    MyMultiplayerGameplay.Static.CutOut(voxelMap, ref bigSphereForTunnel);
                }

                //remove decals
                MyDecals.HideTrianglesAfterExplosion(voxelMap, ref bigSphereForTunnel);

                //cut off 
                MyVoxelGenerator.CutOutSphereFast(voxelMap, bigSphereForTunnel);


                if (MySession.Is25DSector)
                {
                    //  Create debris rocks thrown from the explosion
                    MyExplosionDebrisVoxel.CreateExplosionDebris(ref bigSphereForTunnel, 1, CommonLIB.AppCode.Networking.MyMwcVoxelMaterialsEnum.Ice_01, MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip.GroupMask, voxelMap);

                    BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
                    BoundingBoxHelper.AddSphere(ref bigSphereForTunnel, ref boundingBox);

                    // we need local list because this method can be called from inside of the loop

                    var elements = MyEntities.GetElementsInBox(ref boundingBox);
                    foreach (var el in elements)
                    {
                        MyEntity entity = ((MinerWars.AppCode.Game.Physics.MyPhysicsBody)el.GetRigidBody().m_UserData).Entity;
                        MyExplosionDebrisVoxel debris = entity as MyExplosionDebrisVoxel;

                        if (debris == null)
                            continue;

                        Vector3 awayDirection = debris.GetPosition() - bigSphereForTunnel.Center;

                        debris.Physics.AddForce(
                               MinerWars.AppCode.Game.Physics.MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                               awayDirection * MyExplosionsConstants.EXPLOSION_FORCE_RADIUS_MULTIPLIER * 100000,
                               bigSphereForTunnel.Center,
                               MinerWars.CommonLIB.AppCode.Utils.MyMwcUtils.GetRandomVector3Normalized() * 10000);
                    }

                    elements.Clear();
                }

                PlayVoxelCutCue();
            }
        }
    }
}