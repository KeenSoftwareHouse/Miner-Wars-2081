using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;


namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using App;
    using Audio;
    using MinerWarsMath;
    
    using Models;
    using Utils;
    using CommonLIB.AppCode.Utils;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;

    class MyMineSmart : MyMineBase
    {
        MySmallShip m_currentTarget;
        int m_lastTimeSearchedForTarget;
        MySoundCue? m_movingCue;

        public override void Init()
        {
            Init(MyModelsEnum.MineSmart, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);
        }

        public override void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.MineSmartHud));

            EntityId = MyEntityIdentifier.AllocateId();
            MyEntityIdentifier.AddEntityWithId(this);

            m_currentTarget = null;
            m_movingCue = null;
            m_lastTimeSearchedForTarget = MyConstants.FAREST_TIME_IN_PAST;
            Faction = owner.Faction;
            GuidedInMultiplayer = true;
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (m_state == MyCurrentState.ACTIVATED)
            {
                if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeSearchedForTarget) > MyMineSmartConstants.INTERVAL_TO_SEARCH_FOR_ENEMY_IN_MILISECONDS)
                {
                    m_lastTimeSearchedForTarget = MyMinerGame.TotalGamePlayTimeInMilliseconds;

                    //  Look for small ships in mine's proximity
                    BoundingSphere boundingSphere = new BoundingSphere(GetPosition(), m_ammoProperties.MaxTrajectory);
                    BoundingBox boundingBox;
                    BoundingBox.CreateFromSphere(ref boundingSphere, out boundingBox);                

                    MySmallShip newTarget = null;

                    var elements = MyEntities.GetElementsInBox(ref boundingBox);
                    for (int i = 0; i < elements.Count; i++)
                    {
                        var rigidBody = (MyPhysicsBody)elements[i].GetRigidBody().m_UserData;
                        var entity = rigidBody.Entity;

                        if (entity is MySmallShip && MyFactions.GetFactionsRelation(Faction, entity.Faction) == MyFactionRelationEnum.Enemy)
                        {
                            float dist = Vector3.Distance(entity.GetPosition(), GetPosition());
                            if (dist <= m_ammoProperties.MaxTrajectory)
                            {
                                newTarget = (MySmallShip)entity;
                                break;
                            }
                        }
                    }
                    elements.Clear();

                    //  If not target found at all
                    if (newTarget == null) m_currentTarget = null;

                    //  We assign new target only if we do not have any previous (that's a protection agains possible jumping over targets if more targets in proximity)
                    if (m_currentTarget == null) m_currentTarget = newTarget;
                }

                //  There is some enemy so lets go to him and explode
                if (m_currentTarget != null)
                {
                    //  Move towards the target
                    Vector3 engineForce = MyMwcUtils.Normalize(m_currentTarget.GetPosition() - GetPosition()) * m_ammoProperties.InitialSpeed;
                    engineForce *= MyMineSmartConstants.CHASE_SPEED_MULTIPLIER;
                    this.Physics.AddForce(
                        MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                        engineForce,
                        GetPosition(),
                        Vector3.Zero);

                    StartMovingCue();
                }
                else
                {
                    StopMovingCue();
                }

                UpdateMovingCue();
            }
        }

        void StartMovingCue()
        {
            if ((m_movingCue == null) || (m_movingCue.Value.IsPlaying == false))
            {
                m_movingCue = MyAudio.AddCue3D(MySoundCuesEnum.WepMineMoveALoop, GetPosition(),
                    WorldMatrix.Forward, WorldMatrix.Up, this.Physics.LinearVelocity);
            }
        }

        void StopMovingCue()
        {
            if ((m_movingCue != null) && (m_movingCue.Value.IsPlaying == true))
            {
                m_movingCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_movingCue = null;
            }
        }

        void UpdateMovingCue()
        {
            if ((m_movingCue != null) && (m_movingCue.Value.IsPlaying == true))
            {
                MyAudio.UpdateCuePosition(m_movingCue, GetPosition(), WorldMatrix.Forward, WorldMatrix.Up, this.Physics.LinearVelocity);
            }
        }

        public override void Close()
        {
            base.Close();
            StopMovingCue();
        }
    }
}
