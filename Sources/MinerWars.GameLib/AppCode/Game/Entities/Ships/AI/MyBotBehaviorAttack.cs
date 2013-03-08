#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

#endregion

namespace MinerWars.AppCode.Game.Entities.Ships.AI
{
    public class MyBotBehaviorAttack : MyBotBehaviorBase
    {
        #region Enums

        enum StateEnum
        {
            CLOSING,
            CLOSING_IN_FORMATION,
            FLYING_AROUND,
            ATTACKING,
            ATACKING_SEARCHING,
        }

        enum FireStateEnum
        {
            WAIT,
            FIRE,
            HIT,
            COOLDOWN,
        }

        #endregion

        #region Members

        // TODO: change to constants
        static float MANEUVERING_RANGE = 300;
        static float FIRING_RANGE_MAX = 600;
        static float FIRING_RANGE_MIN = 200;
        static float ATTACK_DISTANCE_MIN = 45;
        static float ATTACK_DISTANCE_MAX = 70;
        static float SHOOT_CONE = MathHelper.Pi / 180 * 25.5f;
        static float HIT_CONE = MathHelper.Pi / 180 * 2.5f;
        static float CLOSING_FORMATION_TIME = 2.0f;
        static float FORCED_CLOSING_TIME = 4.0f;

        MyEntity m_target;
        Vector3 m_flyToTarget;

        bool m_targetVisible;
        float m_visibilityCheckTimer;

        private StateEnum m_state;
        float m_attackTimer;
        float m_flyAroundTimer;
        bool m_strafe;
        float m_closingTimer;
        float m_forcedClosingTimer;
        FireStateEnum m_fireState;
        float m_fireTimer;

        float m_hologramTargetTimer;
        bool m_isInvalid;

        int m_planedMissileTime;

        float m_timeToAlarmCheck;

        float m_playerAttackDecisionTimer;

        MyFindSmallshipHelper findSmallship;

        bool m_canShootWithAutocanon;
        bool m_canShootWithShotGun;
        bool m_canShootWithSniper;
        bool m_canShootMissile;
        bool m_canShootCannon;
        bool m_canShootFlash;
        bool m_canShootSmoke;
        bool m_canShootHologram;

        float m_weaponChangeTimer;
        MyMwcObjectBuilder_FireKeyEnum m_currentWeapon;

        float m_smokeBombTimer;
        float m_flashBombTimer;
        float m_hologramTimer;

        #endregion

        #region Init & close

        public MyBotBehaviorAttack()
        {
        }

        internal override void Init(MySmallShipBot bot)
        {
            base.Init(bot);
            
            m_isInvalid = false;
            m_target = SourceDesire.GetEnemy();
            if (m_target != null)
            {
                Debug.Assert(m_target != bot);
            }
            findSmallship = new MyFindSmallshipHelper();

            m_visibilityCheckTimer = 0;       // force visibility test
            m_closingTimer = 0;
            m_forcedClosingTimer = 0;
            m_hologramTargetTimer = 0;

            SwitchToClosing();
            
            PlanNextMissile();

            m_timeToAlarmCheck = 2;
            
            m_playerAttackDecisionTimer = 0;

            MyScriptWrapper.OnEntityAttackedByBot(bot, m_target);

            m_canShootWithAutocanon = bot.CanShootWith(MyMwcObjectBuilder_FireKeyEnum.Primary);
            m_canShootWithShotGun = bot.CanShootWith(MyMwcObjectBuilder_FireKeyEnum.Secondary);
            m_canShootWithSniper = bot.CanShootWith(MyMwcObjectBuilder_FireKeyEnum.Third);
            m_canShootMissile = bot.CanShootWith(MyMwcObjectBuilder_FireKeyEnum.Fourth);
            m_canShootCannon = bot.CanShootWith(MyMwcObjectBuilder_FireKeyEnum.Fifth);

            m_canShootFlash = bot.CanShootWith(MyMwcObjectBuilder_FireKeyEnum.FlashBombFront) && MyFakes.BOT_USE_FLASH_BOMBS;
            m_canShootSmoke = bot.CanShootWith(MyMwcObjectBuilder_FireKeyEnum.SmokeBombFront) && MyFakes.BOT_USE_SMOKE_BOMBS;
            m_canShootHologram = bot.CanShootWith(MyMwcObjectBuilder_FireKeyEnum.HologramFront) && MyFakes.BOT_USE_HOLOGRAMS;

            m_weaponChangeTimer = 0;
            m_currentWeapon = SelectWeapon((bot.GetPosition() - m_target.GetPosition()).Length());

            m_smokeBombTimer = MyMwcUtils.GetRandomFloat(0.5f, 3.0f);
            m_flashBombTimer = MyMwcUtils.GetRandomFloat(0.5f, 3.0f);
            m_hologramTimer = MyMwcUtils.GetRandomFloat(0.5f, 3.0f);
        }

        internal override void Close(MySmallShipBot bot)
        {
            base.Close(bot);

            if (m_state == StateEnum.CLOSING_IN_FORMATION)
            {
                MyAttackFormations.Instance.RemoveEntity(bot);
            }

            MyBotCoordinator.RemoveAttacker(bot);
        }

        #endregion

        #region Update

        internal override void Update(MySmallShipBot bot)
        {
            base.Update(bot);

            if (m_target != null && !m_target.IsDead())
            {
                MySmallShip smallShipTarget = m_target as MySmallShip;
                MyPrefabLargeWeapon largeWeaponTarget = m_target as MyPrefabLargeWeapon;

                if (largeWeaponTarget != null && !largeWeaponTarget.IsWorking())
                {
                    m_isInvalid = true;
                    return;
                }

                if (m_timeToAlarmCheck >= 0)
                {
                    m_timeToAlarmCheck -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    // When transition from curious behavior try raise alarm (if this doesn't work pass some flag curious behavior)
                    if (m_timeToAlarmCheck < 0 && smallShipTarget != null && smallShipTarget.HasRadarJammerActive())
                    {
                        bot.LaunchAlarm(smallShipTarget);
                    }
                }

                UpdateVisibility(bot);

                if (m_targetVisible)
                {
                    AttackTarget(bot, m_targetVisible);
                    findSmallship.Init(bot);

                    // Attack player to prevent his passivity when we have invicible wingmans
                    m_playerAttackDecisionTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    if (smallShipTarget != null && smallShipTarget.Leader == MySession.PlayerShip && m_playerAttackDecisionTimer <= 0)
                    {
                        m_playerAttackDecisionTimer = 2.5f;
                        if (MyMwcUtils.GetRandomFloat(0, 1.0f) < 0.7f)
                        {
                            m_target = smallShipTarget.Leader;
                        }
                    }
                }
                else
                {
                    m_state = StateEnum.CLOSING;

                    findSmallship.Update(bot, m_target);

                    if (findSmallship.PathNotFound)
                    {
                        bot.IsSleeping = true;
                        m_isInvalid = true;
                    }
                    else if (!findSmallship.GotPosition())
                    {
                        AttackTarget(bot, false);
                    }
                }
            }
        }

        private void UpdateVisibility(MySmallShipBot bot)
        {
            if (m_visibilityCheckTimer <= 0)
            {
                if (bot.GetPosition() == m_target.GetPosition())
                {
                    m_targetVisible = true;
                }
                else
                {
                    MyLine line = new MyLine(bot.GetPosition(), m_target.GetPosition(), true);
                    var result = MyEntities.GetIntersectionWithLine(ref line, bot, m_target, true, ignoreChilds: true);
                    m_targetVisible = !result.HasValue;
                }

                m_visibilityCheckTimer = 0.5f;
            }
            else
            {
                m_visibilityCheckTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            }
        }

        #endregion

        #region Attack

        private void AttackTarget(MySmallShipBot bot, bool targetVisible)
        {
            Vector3 botToTarget = m_target.WorldVolume.Center - bot.GetPosition();
            float distance = botToTarget.Length() - m_target.WorldVolume.Radius;
            Vector3 botToTargetDirection = botToTarget / distance;
            float angleToTarget = (float)Math.Acos(MathHelper.Clamp(Vector3.Dot(bot.WorldMatrix.Forward, botToTargetDirection), -1, 1));

            switch (m_state)
            {
                case StateEnum.CLOSING:
                    if (distance > MANEUVERING_RANGE || m_forcedClosingTimer > 0)
                    {
                        bot.Move(GetTargetPositionByDamageRatio(bot), m_target.GetPosition(), GetUpPlane(), true);                        
                        m_closingTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                        m_forcedClosingTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

                        if (distance < FIRING_RANGE_MAX)
                        {
                            HandleShooting(bot, distance, angleToTarget, m_targetVisible);
                        }

                        if (m_closingTimer >= CLOSING_FORMATION_TIME)
                        {
                            SwitchToClosingInFormation();
                        }
                    }
                    else
                    {
                        SwitchToAttack(bot);
                    }

                    break;
                case StateEnum.CLOSING_IN_FORMATION:
                    if (distance > MANEUVERING_RANGE || m_forcedClosingTimer > 0)
                    {
                        Vector3 formationTarget = MyAttackFormations.Instance.GetFormationPosition(bot, m_target);
                        m_closingTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                        m_forcedClosingTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                        
                        if (distance < FIRING_RANGE_MAX)
                        {
                            bot.Move(formationTarget, m_target.GetPosition(), GetUpPlane(), true);
                            HandleShooting(bot, distance, angleToTarget, m_targetVisible);
                        }
                        else
	                    {
                            bot.Move(formationTarget, formationTarget, GetUpPlane(), true);
	                    }
                    }
                    else
                    {
                        MyAttackFormations.Instance.RemoveEntity(bot);
                        SwitchToAttack(bot);
                    }

                    if (distance < FIRING_RANGE_MAX)
                    {
                        HandleShooting(bot, distance, angleToTarget, m_targetVisible);
                    }
                    break;
                case StateEnum.FLYING_AROUND:
                    float flyToDistance = Vector3.Distance(bot.GetPosition(), m_flyToTarget);

                    if (m_strafe)
                    {
                        float rspeed = (1.0f - Math.Abs(MathHelper.Clamp(angleToTarget / MathHelper.Pi, -1, 1))) * 900 + 100;

                        //bot.Move(m_flyToTarget, m_target.GetPosition(), GetUpPlane(), false, customDamping: 0.35f,
                        //    rotationSpeed: rspeed);
                        bot.Move(bot.GetPosition(), GetTargetPositionByDamageRatio(bot), GetUpPlane(), false, customDamping: 0.35f,
                            rotationSpeed: rspeed);
                    }
                    else
                    {
                        float factor = MathHelper.Clamp(flyToDistance / 20, 0.15f, 0.45f);
                        //factor = factor * factor * factor;
                        bot.Move(m_flyToTarget, m_flyToTarget, GetUpPlane(), false, 1, 5, factor, slowRotation: true);
                    }

                    HandleShooting(bot, distance, angleToTarget, targetVisible);

                    if (flyToDistance < 10 || m_flyAroundTimer < 0)
                    {
                        if (distance > MANEUVERING_RANGE)
                        {
                            SwitchToClosing();
                        }
                        else
                        {
                            SwitchToAttack(bot);
                        }
                    }

                    m_flyAroundTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    break;
                case StateEnum.ATTACKING:
                    // Handle hologram target, if bot is atacking hologram for 5 seconds he will get it's a hologram
                    MySmallShip smallShip = m_target as MySmallShip;
                    if (smallShip != null && smallShip.IsHologram)
                    {
                        m_hologramTargetTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                        if (m_hologramTargetTimer > 5.0f)
                        {
                            bot.SpoilHologram(smallShip);
                        }
                    }

                    //bot.Move(bot.GetPosition(), m_target.GetPosition(), GetUpPlane(), false, rotationSpeed: 100);
                    bot.Move(bot.GetPosition(), GetTargetPositionByDamageRatio(bot), GetUpPlane(), false, rotationSpeed: 100);                    

                    if (m_attackTimer < 0)
                    {
                        if (distance > MANEUVERING_RANGE)
                        {
                            SwitchToClosing();
                        }
                        else
                        {
                            if (!MyFakes.DISABLE_BOT_MANEUVERING)
                            {
                                TrySwitchToAttackSearch(bot);
                            }
                        }
                    }
                    m_attackTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

                    HandleShooting(bot, distance, angleToTarget, targetVisible);
                    break;
                case StateEnum.ATACKING_SEARCHING:
                    //bot.Move(bot.GetPosition(), m_target.GetPosition(), GetUpPlane(), false, rotationSpeed: 100);
                    bot.Move(bot.GetPosition(), GetTargetPositionByDamageRatio(bot), GetUpPlane(), false, rotationSpeed: 100);

                    TrySwitchToFlyAround(bot);

                    HandleShooting(bot, distance, angleToTarget, targetVisible);
                    break;
            }
        }

        private Vector3 GetTargetPositionByDamageRatio(MySmallShipBot bot)         
        {
            Vector3 targetPos = m_target.GetPosition();
            if (MySession.PlayerShip != null && MyFactions.GetFactionsRelation(bot, MySession.PlayerShip) == MyFactionRelationEnum.Enemy)
            {
                Vector3 botPos = bot.GetPosition();
                Vector3 botToTarget = targetPos - botPos;
                Vector3 dir = Vector3.Normalize(botToTarget);                
                float degrees = (float)Math.Pow(120, bot.GetDamageRatio() * 1.5 - 1.2) * 4f;
                float deviatingAngle = MathHelper.ToRadians(degrees);
                dir = MyUtilRandomVector3ByDeviatingVector.GetRandom(dir, deviatingAngle);
                targetPos = botPos + botToTarget.Length() * dir * 3;                
            }
            return targetPos;
        }

        #endregion

        #region Switching

        bool TrySwitchToAttackSearch(MySmallShipBot bot)
        {
            Vector3 newFlyToTarget = m_target.WorldVolume.Center
                + m_target.WorldMatrix.Forward * (MyMwcUtils.GetRandomFloat(ATTACK_DISTANCE_MIN, ATTACK_DISTANCE_MAX) + m_target.WorldVolume.Radius)
                + m_target.WorldMatrix.Up * MyMwcUtils.GetRandomFloat(-50, 50)
                + m_target.WorldMatrix.Left * MyMwcUtils.GetRandomFloat(-50, 50);
            if (bot.TryTestPosition(newFlyToTarget, bot.GetPosition()))
            {
                m_state = StateEnum.ATACKING_SEARCHING;
                return true;
            }
            return false;
        }
        
        void TrySwitchToFlyAround(MySmallShipBot bot)
        {
            Vector3? result = null;
            if (bot.TryGetTestPositionResult(ref result))
            {
                if (result.HasValue)
                {
                    m_flyToTarget = result.Value;
                    m_flyAroundTimer = MyMwcUtils.GetRandomFloat(2, 3);
                    m_strafe = MyMwcUtils.GetRandomBool(4); //Vector3.DistanceSquared(flyToTarget, bot.GetPosition()) < STRAFE_DISTANCE_SQR;
                    m_state = StateEnum.FLYING_AROUND;
                }
                else
                {
                    // Position failed, try again
                    bool success = TrySwitchToAttackSearch(bot);
                    Debug.Assert(success);
                }
            }
        }

        void SwitchToClosing()
        {
            m_fireState = FireStateEnum.WAIT;
            m_state = StateEnum.CLOSING;
            m_closingTimer = 0;
        }

        void SwitchToClosingInFormation()
        {
            m_fireState = FireStateEnum.WAIT;
            m_state = StateEnum.CLOSING_IN_FORMATION;
        }

        void SwitchToAttack(MySmallShipBot bot)
        {
            m_state = StateEnum.ATTACKING;
            m_attackTimer = MyMwcUtils.GetRandomFloat(2, 4);

            MyBotCoordinator.AddAttacker(bot, m_target);
        }

        #endregion

        #region Shooting

        void HandleShooting(MySmallShipBot bot, float distance, float angleToTarget, bool targetVisible)
        {
            if (bot.InitTime > 0)
                return;

            if (!MyBotCoordinator.IsAllowedToAttack(bot, m_target))
            {      
                return;
            }

            m_smokeBombTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            m_flashBombTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            m_hologramTimer -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            // Change weapon - 2sec CD
            if (m_weaponChangeTimer < 2)
            {
                m_weaponChangeTimer = 0;

                var oldWeapon = m_currentWeapon;
                m_currentWeapon = SelectWeapon(distance);
                
                if (m_currentWeapon != oldWeapon)
                {
                    m_fireState = FireStateEnum.WAIT;
                }
            }

            // Handle weapon shooting
            switch (m_currentWeapon)
            {
                case MyMwcObjectBuilder_FireKeyEnum.Primary:
                    ShootAutocannon(bot, targetVisible, distance, angleToTarget);
                    break;
                case MyMwcObjectBuilder_FireKeyEnum.Secondary:
                    ShootShotgun(bot, targetVisible, distance, angleToTarget);
                    break;
                case MyMwcObjectBuilder_FireKeyEnum.Third:
                    ShootSniper(bot, targetVisible, distance, angleToTarget);
                    break;
            }

            if (MyFakes.ENABLE_BOT_MISSILES_AND_CANNON)
            {
                if (m_canShootFlash &&
                    m_flashBombTimer <= 0 &&
                    targetVisible &&
                    distance > 125 && distance < 250 &&
                    angleToTarget < SHOOT_CONE)
                {
                    m_flashBombTimer = MyMwcUtils.GetRandomFloat(30, 60);
                    bot.Shoot(MyMwcObjectBuilder_FireKeyEnum.FlashBombFront);
                }

                if (m_canShootSmoke &&
                    m_smokeBombTimer <= 0 &&
                    targetVisible &&
                    distance > 150 && distance < 300 &&
                    angleToTarget < SHOOT_CONE)
                {
                    m_smokeBombTimer = MyMwcUtils.GetRandomFloat(30, 60);
                    bot.Shoot(MyMwcObjectBuilder_FireKeyEnum.SmokeBombFront);
                }

                if (m_canShootHologram &&
                    m_hologramTimer <= 0 &&
                    distance > 100 && distance < 500)
                {
                    m_hologramTimer = MyMwcUtils.GetRandomFloat(30, 60);
                    bot.Shoot(MyMwcObjectBuilder_FireKeyEnum.HologramFront);
                }

                // Handle missile and cannon shooting
                if (m_canShootMissile || m_canShootCannon)
                {
                    MyMwcObjectBuilder_FireKeyEnum firekey;
                    if (m_canShootMissile)
                    {
                        firekey = MyMwcObjectBuilder_FireKeyEnum.Fourth;
                    }
                    else
                    {
                        firekey = MyMwcObjectBuilder_FireKeyEnum.Fifth;
                    }

                    int missileFireTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_planedMissileTime;
                    if (missileFireTime > 0)
                    {

                        if (missileFireTime > 1500)
                        {
                            // if bot can't shoot in 1.5 sec, wait for next time window
                            PlanNextMissile();
                        }
                        else if (targetVisible &&
                            distance >= MyFakes.BOT_MISSILE_FIRING_RANGE_MIN &&
                            distance <= FIRING_RANGE_MAX &&
                            angleToTarget < SHOOT_CONE)
                        {
                            // Successful shot
                            bot.Shoot(firekey);
                            PlanNextMissile();
                        }
                    }
                }
            }
        }

        private MyMwcObjectBuilder_FireKeyEnum SelectWeapon(float distance)
        {
            if (m_canShootWithShotGun && distance < 200)
            {
                return MyMwcObjectBuilder_FireKeyEnum.Secondary;
            }
            else if (m_canShootWithSniper && distance > 400)
            {
                return MyMwcObjectBuilder_FireKeyEnum.Third;
            }
            else
            {
                return MyMwcObjectBuilder_FireKeyEnum.Primary;
            }
        }

        void ShootAutocannon(MySmallShipBot bot, bool targetVisible, float distance, float angleToTarget)
        {
            switch (m_fireState)
            {
                case FireStateEnum.WAIT:
                    m_fireTimer = 0;
                    if (angleToTarget < SHOOT_CONE && targetVisible && distance <= FIRING_RANGE_MAX)
                    {
                        m_fireState = FireStateEnum.FIRE;
                    }
                    break;
                case FireStateEnum.FIRE:
                    if (targetVisible && distance <= FIRING_RANGE_MAX && angleToTarget < SHOOT_CONE)
                    {
                        m_fireTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                        bot.Shoot(m_currentWeapon);
                        if (m_fireTimer > 0.5f)
                        {
                            m_fireTimer = 0;
                            m_fireState = FireStateEnum.COOLDOWN;
                        }
                        else if (angleToTarget < HIT_CONE)
                        {
                            m_fireTimer = 0;
                            m_fireState = FireStateEnum.HIT;
                        }
                    }
                    else
                    {
                        m_fireState = FireStateEnum.WAIT;
                    }
                    break;
                case FireStateEnum.HIT:
                    m_fireTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    bot.Shoot(m_currentWeapon);
                    if (m_fireTimer > 0.5f)
                    {
                        m_fireState = FireStateEnum.COOLDOWN;
                        m_fireTimer = 0;
                    }
                    break;
                case FireStateEnum.COOLDOWN:
                    m_fireTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    if (m_fireTimer > 0.5f)
                    {
                        m_fireTimer = 0;
                        m_fireState = FireStateEnum.FIRE;
                    }
                    break;
            }
        }

        void ShootShotgun(MySmallShipBot bot, bool targetVisible, float distance, float angleToTarget)
        {
            switch (m_fireState)
            {
                case FireStateEnum.WAIT:
                    m_fireTimer = 0;
                    if (angleToTarget < SHOOT_CONE && targetVisible && distance <= FIRING_RANGE_MAX)
                    {
                        m_fireState = FireStateEnum.FIRE;
                    }
                    break;
                case FireStateEnum.FIRE:
                    if (targetVisible && distance <= FIRING_RANGE_MAX && angleToTarget < SHOOT_CONE)
                    {
                        m_fireTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                        bot.Shoot(m_currentWeapon);
                        if (m_fireTimer > 0.5f)
                        {
                            m_fireTimer = 0;
                            m_fireState = FireStateEnum.COOLDOWN;
                        }
                        else if (angleToTarget < HIT_CONE)
                        {
                            m_fireTimer = 0;
                            m_fireState = FireStateEnum.HIT;
                        }
                    }
                    else
                    {
                        m_fireState = FireStateEnum.WAIT;
                    }
                    break;
                case FireStateEnum.HIT:
                    m_fireTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    bot.Shoot(m_currentWeapon);
                    if (m_fireTimer > 0.5f)
                    {
                        m_fireState = FireStateEnum.COOLDOWN;
                        m_fireTimer = 0;
                    }
                    break;
                case FireStateEnum.COOLDOWN:
                    m_fireTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    if (m_fireTimer > 0.5f)
                    {
                        m_fireTimer = 0;
                        m_fireState = FireStateEnum.FIRE;
                    }
                    break;
            }
        }

        void ShootSniper(MySmallShipBot bot, bool targetVisible, float distance, float angleToTarget)
        {
            switch (m_fireState)
            {
                case FireStateEnum.WAIT:
                    m_fireTimer = 0;
                    if (angleToTarget < HIT_CONE && targetVisible && distance <= FIRING_RANGE_MAX)
                    {
                        m_fireState = FireStateEnum.HIT;
                    }
                    break;
                case FireStateEnum.HIT:
                    m_fireTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    bot.Shoot(m_currentWeapon);
                    if (m_fireTimer > 0.5f)
                    {
                        m_fireState = FireStateEnum.COOLDOWN;
                        m_fireTimer = 0;
                    }
                    break;
                case FireStateEnum.COOLDOWN:
                    m_fireTimer += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
                    if (m_fireTimer > 0.5f)
                    {
                        m_fireTimer = 0;
                        m_fireState = FireStateEnum.WAIT;
                    }
                    break;
            }
        }

        void PlanNextMissile()
        {
            m_planedMissileTime = MyMinerGame.TotalGamePlayTimeInMilliseconds + MyMwcUtils.GetRandomInt(3000, 6000);
        }

        internal override bool IsInvalid()
        {
            return m_isInvalid;
        }

        internal override BotBehaviorType GetBehaviorType()
        {
            return BotBehaviorType.ATTACK;
        }

        internal Vector3 GetUpPlane()
        {
            if(MySession.PlayerShip != null)
            {
                return MySession.PlayerShip.WorldMatrix.Up;
            }

            if (m_target != null)
	        {
                return m_target.WorldMatrix.Up;
	        }

            return Vector3.Up;
        }

        #endregion

        internal override string GetDebugHudString()
        {
            if (m_targetVisible)
            {
                return string.Format("{0} {1}", base.GetDebugHudString(), m_state);
            }
            else
            {
                return string.Format("{0} {1} Can't see target", base.GetDebugHudString(), m_state);
            }
        }
    }
}
