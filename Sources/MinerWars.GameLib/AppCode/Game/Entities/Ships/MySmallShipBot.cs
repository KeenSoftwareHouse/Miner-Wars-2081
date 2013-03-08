#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using KeenSoftwareHouse.Library.Memory;
using KeenSoftwareHouse.Library.Trace;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using MinerWars.AppCode.Game.Entities.Ships.AI.Jobs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;

using ParallelTasks;
using SysUtils;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Radar;
using KeenSoftwareHouse.Library.Parallelization.Threading;
using MinerWars.CommonLIB.AppCode.Import;

#endregion

namespace MinerWars.AppCode.Game.Entities
{
    class MySmallShipBot : MySmallShip
    {
        public static bool BotLogic = true;
        public static int TotalAliveBots = 0;

        public int InitTime = 0; //Initialization time in ms 

        #region OldWeapons
        /// <summary>
        /// Contains current bot's burst times for each weapon slot, based on ammo types.
        /// For each weapon slot we have some important values:
        /// 0 - Firing duration tuner    - less for rockets and low amount of ammo weapons, basic value is 10x shot interval for tuner = 1.0
        /// 1 - Firing frequency tuner   - more for rockets and low amount of ammo weapons
        /// 2 - Firing Cone tuner        - bot should launch its bigger weapons when it has more chances for hit
        /// 3 - Firing Distance          - maximal shoot distance for weapon
        /// 4 - Firing distance tuner    - tuner for shooting range
        /// 5 - Shot interval            - interval between two shots from weapon
        /// </summary>
        private enum MyBotWeaponsParametrsEnum
        {
            FiringDurationTuner = 0,
            FiringFrequencyTuner = 1,
            FiringConeTuner = 2,
            FiringDistance = 3,
            FiringDistanceTuner = 4,
            ShotIntervalMilliseconds = 5,
        }

        /// <summary>
        /// Params of mounted weapons, so bot know when to fire each slot etc.
        /// </summary>
        private struct MyBotParams
        {
            public float[] WeaponsParams;
            public MyMwcObjectBuilder_FireKeyEnum FireKey;
            public MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum WeaponType;
            public bool Enabled;
            public float LastShotMilliseconds;
            public bool ForceUse;
            public bool HasAmmo;
        }

        /// <summary>
        /// Weapon parameters for all weapon slots
        /// </summary>
        private MyBotParams[] m_botWeaponParamsAllSlots;

        public MySmallShipBot()
        {
            m_includeCargoWeight = true;
        }

        public void ForceWeaponUse(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType)
        {
            for (int i = 0; i < m_botWeaponParamsAllSlots.Length; i++)
            {
                if (m_botWeaponParamsAllSlots[i].WeaponType == weaponType)
                {
                    m_botWeaponParamsAllSlots[i].ForceUse = true;
                    break;
                }
            }
        }

        private bool TryGetWeaponParams(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType, ref MyBotParams weaponParams)
        {
            for (int i = 0; i < m_botWeaponParamsAllSlots.Length; i++)
            {
                if (m_botWeaponParamsAllSlots[i].WeaponsParams == null)
                    continue;

                if (m_botWeaponParamsAllSlots[i].WeaponType == weaponType)
                {
                    weaponParams = m_botWeaponParamsAllSlots[i];
                    return true;
                }
            }
            return false;
        }

        public bool CanShootWith(MyMwcObjectBuilder_FireKeyEnum fireKey)
        {
            var parameters = m_botWeaponParamsAllSlots[(int)fireKey - 1];
            return parameters.Enabled && parameters.HasAmmo;
        }

        public void Shoot(MyMwcObjectBuilder_FireKeyEnum fireKey)
        {
            if (m_botWeaponParamsAllSlots == null)
                return;

            MyBotParams weaponParams = m_botWeaponParamsAllSlots[(int)fireKey - 1];
            
            if (!weaponParams.Enabled || !weaponParams.HasAmmo)
            {
                return;
            }

            weaponParams.LastShotMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            Weapons.Fire(fireKey);
        }

        /// <summary>
        /// Constants for all mountable weapons
        /// TODO: numeric params should be taken from editor later
        /// </summary>
        private MyBotParams SettingsForWeaponType(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            var settings = new MyBotParams { WeaponsParams = new float[Enum.GetNames(typeof(MyBotWeaponsParametrsEnum)).Length] };

            switch (weaponType)
            {
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 10.0f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.06f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyAutocanonConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.5f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.3f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 1.4f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyShotgunConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.3f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.4f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyARSConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.15f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.25f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyCannonConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.6f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.6f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyMachineGunConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.25f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.45f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.2f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.05f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MySniperConstants.SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        break;
                    }
                case (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back):
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.15f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.05f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 1.0f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = false;
                        break;
                    }
                case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front:
                    {
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDurationTuner] = 0.15f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringFrequencyTuner] = 0.05f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringConeTuner] = 0.5f;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.ShotIntervalMilliseconds] = MyMissileConstants.MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS;
                        settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistanceTuner] = 0.9f;
                        settings.Enabled = true;
                        break;
                    }
            }

            //Keep max trajectory and weapon type
            settings.WeaponsParams[(int)MyBotWeaponsParametrsEnum.FiringDistance] = MyAmmoConstants.GetAmmoProperties(ammoType).MaxTrajectory;

            settings.WeaponType = weaponType;

            settings.LastShotMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            return settings;
        }
        #endregion

        #region Properties

        // how much will bot strafe during combat  (not its speed)
        public float DifficultyStrafingSpeed { get; set; }

        // ship speed 
        public float DifficultyMovingSpeed { get; set; }

        // affect speed of shooting (delays between all shooting)
        public float DifficultyFireRatio { get; set; }

        // what guns/slots will be used during fight / shoting
        public float DifficultyGunUsageRatio { get; set; }

        // how often will be raid attack executed
        public float DifficultyRaidAttackOccurrence { get; set; }

        // how often will be fly around exectued
        public float DifficultyFlyAroundTargetOccurrence { get; set; }

        //how far will bot react on enemy presence and from what distance he will shoot ( harder - more far )
        public float DifficultyAttackReactDistance { get; set; }

        //use of fire burst 0 - 1
        public float DifficultyUseFireBurst { get; set; }

        private static MySoundCue? m_friendlyFireCue;

        private static int m_friendlyFireTimeFromLastPlayed;

        public MyWayPointPath WaypointPath = null;        

        public MyWayPoint CurrentWaypoint = null;

        public float RunAwayDistance = 1200;

        public float DrillDistance = 20;

        public float Aggressivity = 0;

        public float SeeDistance = 1000;
        
        public float SleepDistance = 1000;

        public MyPatrolMode PatrolMode = MyPatrolMode.CYCLE;

        public bool SuspendPatrol;

        /// <summary>
        /// Modifies bots speed 1.0 means normal speed, 0.0 means bot wont move
        /// </summary>
        public float SpeedModifier = 1.0f;

        private static string m_pilotDeadString;

        protected MyBotAITemplate m_aiTemplate = MyBotAITemplates.GetTemplate(MyAITemplateEnum.DEFAULT);
        public MyBotAITemplate AITemplate
        {
            get { return m_aiTemplate; }
            set { m_aiTemplate = value; }
        }

        bool m_isSleeping = false;
        public bool IsSleeping
        {
            get { return m_isSleeping; }
            set
            {
                if (value != m_isSleeping)
                {
                    if (value)
                        TotalAliveBots--;
                    else
                        TotalAliveBots++;
                }

                m_isSleeping = value;

                if (m_isSleeping)
                {
                    if (Weapons != null)
                        Weapons.StopAllSounds();
                }
            }
        }

        List<MyBotDesire> m_desires = new List<MyBotDesire>();

        MyBotBehaviorBase m_currentBehavior;

        MyDetectEnemiesJob m_detectEnemiesJob = new MyDetectEnemiesJob();
        bool m_detectingEnemies = false;
        Task m_detectEnemiesTask;
        int m_lastDetect = 0;

        MyTestPositionJob m_testPositionJob = new MyTestPositionJob();
        bool m_testingPosition = false;
        Task m_testPositionTask;

        Vector3 m_debugMovePosition;

        private MyParticleEffect m_biochemEffect;
        private Matrix? m_biochemEffectLocalMatrix;

        /// <summary>
        /// Indicates whether the AI controlling the ship is active.
        /// </summary>
        public bool ActiveAI { get; set; }

        private float m_shock_time;

        private List<MySmallShip> m_spoiledHolograms;

        private object m_hologramLock = new object();

        public bool LeaderLostEnabled { get; set; }

        private int m_dangerZoneId = -1;


        public MyEntity LookTarget;

        private float m_crippleTime;

        private uint? m_leaderId;

        private bool m_wasAfterBurnerOn = false;
        private int m_afterBurnerOnTime = 0;
        private int m_afterBurnerOffTime = 0;

        #endregion

        #region Init

        static MySmallShipBot()
        {
            m_pilotDeadString = MyTextsWrapper.Get(MyTextsWrapperEnum.SmallShipPilotDead).ToString();
        }

        public override string GetFriendlyName()
        {
            return "MySmallShipBot";
        }

        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_SmallShip_Bot objectBuilder)
        {
            System.Diagnostics.Debug.Assert(objectBuilder.Faction != 0);

            //StringBuilder label = new StringBuilder(hudLabelText);
            TotalAliveBots++;

            string fixedHudLabelText = hudLabelText;
            if (string.IsNullOrEmpty(hudLabelText) || hudLabelText == GetFriendlyName())
            {
                fixedHudLabelText = MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.Ship);
            }

            base.Init(fixedHudLabelText, objectBuilder);

            Faction = objectBuilder.Faction;
            m_aiTemplate = MyBotAITemplates.GetTemplate(objectBuilder.AITemplate);
            Aggressivity = objectBuilder.Aggressivity;
            SeeDistance = objectBuilder.SeeDistance == 0 ? 1000 : objectBuilder.SeeDistance;
            SleepDistance = objectBuilder.SleepDistance == 0 ? 1000 : objectBuilder.SleepDistance;
            PatrolMode = objectBuilder.PatrolMode;
            ActiveAI = true;
            m_leaderId = objectBuilder.Leader;
            AITemplate.SetIdleBehavior(objectBuilder.IdleBehavior);
            LeaderLostEnabled = objectBuilder.LeaderLostEnabled;
            ActiveAI = ActiveAI;

            SetupWeapons(objectBuilder);

            SetupDifficulty();

            //if (hudLabelText == GetFriendlyName())
            //{
            //    label = new StringBuilder("");
            //}

            //if (string.IsNullOrEmpty(hudLabelText) || hudLabelText == GetFriendlyName())
            //{
            //    DisplayName = MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.Ship);
            //}

            //MyHud.ChangeText(this, label, null, 10000, MyHudIndicatorFlagsEnum.SHOW_ALL);


            MyModelDummy dummy;
            if (GetModelLod0().Dummies.TryGetValue("destruction", out dummy))
            {
                m_biochemEffectLocalMatrix = dummy.Matrix;
            }

            m_shock_time = -1;

            MyBotCoordinator.AddBot(this);
            
            InitSpoiledHolograms();

            MyEntities.OnEntityRemove += MyEntities_OnEntityRemove;

            m_dangerZoneId = MyDangerZones.Instance.Register(this);
            
            MySession.Static.LinkEntities += OnLinkEntities;
        }


        void OnLinkEntities()
        {
            if (m_leaderId.HasValue)
            {
                //var leader = MyEntities.GetEntityById(new MyEntityIdentifier(m_leaderId.Value)) as MySmallShip;
                //Debug.Assert(leader != null);
                var leader = MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier(m_leaderId.Value)) as MySmallShip;
                Follow(leader);
            }
            MySession.Static.LinkEntities -= OnLinkEntities;
        }

        public override void Link()
        {
            base.Link();

            if (OwnerEntity != null)
            {
                if (OwnerEntity is MySpawnPoint)
                {
                    ((MySpawnPoint)OwnerEntity).LinkShip(this);
                }
            }
        }

        protected void InitSpoiledHolograms()
        {
            m_spoiledHolograms = new List<MySmallShip>();
        }

        #endregion

        #region Weapons

        private bool TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType, MyMwcObjectBuilder_AmmoGroupEnum ammoGroup, MyMwcObjectBuilder_FireKeyEnum fireKey, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] ammoPriorities)
        {
            for (int i = 0; i < ammoPriorities.Length; i++)
            {
                if (Weapons.AmmoInventoryItems.GetAmmoInventoryItems(ammoPriorities[i]).Count > 0)
                {
                    Weapons.AmmoAssignments.AssignAmmo(fireKey, ammoGroup, ammoPriorities[i]);

                    MyBotParams botParamsOneSlot = SettingsForWeaponType(weaponType, ammoPriorities[i]);
                    botParamsOneSlot.FireKey = fireKey;
                    botParamsOneSlot.HasAmmo = true;
                    m_botWeaponParamsAllSlots[(int)fireKey - 1] = botParamsOneSlot;
                    return true;
                }
            }
            return false;
        }

        private void SetupWeapons(MyMwcObjectBuilder_SmallShip_Bot objectBuilder)
        {
            m_botWeaponParamsAllSlots = new MyBotParams[Enum.GetNames(typeof(MyMwcObjectBuilder_FireKeyEnum)).Length];

            bool autoCannonMounted = Weapons.HasMountedWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon);
            bool machineGunMounted = Weapons.HasMountedWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun);
            bool shotgunMounted = Weapons.HasMountedWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun);
            bool sniperMounted = Weapons.HasMountedWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper);
            bool missileLauncherMounted = Weapons.HasMountedWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher);
            bool cannonMounted = Weapons.HasMountedWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);
            bool frontLauncherMounted = Weapons.HasMountedWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);

            if (machineGunMounted)  // Will be overwritten when autoCannonm mounted
            {
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] ammoPriorities = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP };
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_FireKeyEnum.Primary, ammoPriorities);
            }
            
            if (autoCannonMounted)
	        {
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] ammoPriorities = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP };
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_FireKeyEnum.Primary, ammoPriorities);
	        }

            if (shotgunMounted)
            {
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] ammoPriorities = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic };
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_FireKeyEnum.Secondary, ammoPriorities);
            }

            if (sniperMounted)
            {
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] ammoPriorities = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP };
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_FireKeyEnum.Third, ammoPriorities);
            }

            if (missileLauncherMounted)
            {
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] ammoPriorities = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP };
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_FireKeyEnum.Fourth, ammoPriorities);
            }

            if (cannonMounted)
            {
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] ammoPriorities = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP };
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon, MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_FireKeyEnum.Fifth, ammoPriorities);
            }

            if (frontLauncherMounted)
            {
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] flashAmmo = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb };
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_FireKeyEnum.FlashBombFront, flashAmmo);

                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] hologramAmmo = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram };
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_FireKeyEnum.HologramFront, hologramAmmo);

                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum[] smokeAmmo = { MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb};
                TryAssignAmmo(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_FireKeyEnum.SmokeBombFront, smokeAmmo);
            }
        }

        private MyGameplayDifficultyEnum GetBotDifficulty()
        {
            MyFactionRelationEnum relationToPlayer = MyFactions.GetFactionsRelation(MySession.PlayerShip, this);
            if (relationToPlayer == MyFactionRelationEnum.Friend)
            {
                switch (MyGameplayConstants.GameplayDifficultyProfile.GameplayDifficulty)
                {
                    case MyGameplayDifficultyEnum.EASY:
                        return MyGameplayDifficultyEnum.HARD;
                    case MyGameplayDifficultyEnum.NORMAL:
                        return MyGameplayDifficultyEnum.NORMAL;
                    case MyGameplayDifficultyEnum.HARD:
                        return MyGameplayDifficultyEnum.EASY;
                }
            }

            return MyGameplayConstants.GameplayDifficultyProfile.GameplayDifficulty;
        }

        private void SetupDifficulty()
        {
            //always 0 super easy 1 max hard
            MyGameplayDifficultyEnum difficultyOfThisBot = GetBotDifficulty();

            DifficultyStrafingSpeed = MyGameplayConstants.GetGameplayDifficultyProfile(difficultyOfThisBot).BotStrafingSpeed;

            DifficultyMovingSpeed = MyGameplayConstants.GetGameplayDifficultyProfile(difficultyOfThisBot).BotMovingSpeed;

            DifficultyFireRatio = MyGameplayConstants.GetGameplayDifficultyProfile(difficultyOfThisBot).BotFireRatio;

            //HACK BECAUSE OF TEST BUILD..
            DifficultyGunUsageRatio = 0.5f;
            //DifficultyGunUsageRatio = MyGameplayConstants.GetGameplayDifficulty(difficultyOfThisBot).BotGunUsageRatio;

            DifficultyRaidAttackOccurrence = MyGameplayConstants.GetGameplayDifficultyProfile(difficultyOfThisBot).BotRaidAttackOccurrence;

            DifficultyFlyAroundTargetOccurrence = MyGameplayConstants.GetGameplayDifficultyProfile(difficultyOfThisBot).BotFlyAroundTargetOccurrence;

            DifficultyAttackReactDistance = MyGameplayConstants.GetGameplayDifficultyProfile(difficultyOfThisBot).BotAttackReactDistance;

            DifficultyUseFireBurst = 1;
        }

        private void RefillAmmoAndResources()
        {
            Oxygen = MaxOxygen;
            Fuel = MaxFuel;

            foreach (Inventory.MyInventoryItem item in Weapons.AmmoInventoryItems.GetAmmoInventoryItems())
            {
                item.Amount = item.MaxAmount;
            }
        }

        #endregion

        #region Damage & death

        void MyEntities_OnEntityRemove(MyEntity entity)
        {
            MySmallShip smallShip = entity as MySmallShip;
            if (smallShip != null && smallShip.IsHologram)
            {
                lock (m_hologramLock)
                {
                    m_spoiledHolograms.Remove(smallShip);
                }
            }

            foreach (var desire in m_desires)
            {
                desire.OnEntityRemove(entity);
            }
        }

        public override void Close()
        {
            if (m_dangerZoneId != -1)
            {
                MyDangerZones.Instance.Unregister(this);
                m_dangerZoneId = -1;
            }

            MyEntities.OnEntityRemove -= MyEntities_OnEntityRemove;
            MySession.Static.LinkEntities -= OnLinkEntities;

            if (Followers.Count > 0)
            {
                MySmallShipBot newLeader = Followers[0];
                Debug.Assert(newLeader != this);

                newLeader.Leader = null;
                Debug.Assert(!newLeader.Closed);

                if (m_aiTemplate.IsPatroling() && newLeader.WaypointPath != null)
                {
                    SetWaypointPath(newLeader.WaypointPath.Name);
                    newLeader.Patrol();
                }
                else
                {
                    Idle();
                }

                var followers = Followers.ToArray();
                for (int i = 1; i < followers.Length; i++)
                {
                    followers[i].Follow(newLeader);
                }
                Followers.Remove(newLeader);
                Debug.Assert(Followers.Count == 0);
            }
            
            StopFollow();

            if (m_currentBehavior != null)
            {
                m_currentBehavior.Close(this);
                m_currentBehavior = null;
            }
            
            if (m_biochemEffect != null)
            {
                m_biochemEffect.Stop();
                m_biochemEffect = null;
            }
            WaypointPath = null;

            if (!IsSleeping)
                MySmallShipBot.TotalAliveBots--;
            
            base.Close();
        }

        private bool CanDie(MyEntity damageSource)
        {
            if (!IsDestructible)
            {
                return false;
            }
            if (MyGuiScreenGamePlay.Static != null && MySession.PlayerShip != null && damageSource != null)
            {
                var playerFactionRelation = MyFactions.GetFactionsRelation(this, MySession.PlayerShip);

                // Cheat enemy can't die
                if (playerFactionRelation == MyFactionRelationEnum.Enemy &&
                    MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.ENEMY_CANT_DIE))
                {
                    return false;
                }

                // Cheat neutral can't die
                if ((playerFactionRelation == MyFactionRelationEnum.Friend || playerFactionRelation == MyFactionRelationEnum.Neutral) &&
                    MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.FRIEND_NEUTRAL_CANT_DIE))
                {
                    return false;
                }
            }

            return true;
        }

        protected override void DoDamageInternal(float healthDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            bool wasDead = IsDead();
            bool pilotWasDead = IsPilotDead();
            if (CanDie(damageSource))
            {
                PilotHealth -= healthDamage;
                PilotHealth = MathHelper.Clamp(PilotHealth, 0, MyGameplayConstants.HEALTH_BASIC);

                base.DoDamageInternal(healthDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);

                if (IsDamagedForWarnignAlert())
                {
                    LowHealth();
                }
            }
            bool died = !wasDead && IsDead();
            bool pilotDied = !pilotWasDead && IsPilotDead();

            // if player's ship fired
            if (damageSource == MySession.PlayerShip)
            {
                MyFactionRelationEnum? factionRelation = MyFactions.GetFactionsRelation(this, damageSource);

                // friendly fire warning
                if (factionRelation == MyFactionRelationEnum.Friend)
                {
                    if (m_friendlyFireCue == null ||
                        !m_friendlyFireCue.Value.IsPlaying && MyMinerGame.TotalGamePlayTimeInMilliseconds - m_friendlyFireTimeFromLastPlayed >= MySmallShipConstants.WARNING_FRIENDLY_FIRE_INTERVAL)
                    {
                        m_friendlyFireCue = MyAudio.AddCue2D(MySoundCuesEnum.HudFriendlyFireWarning);
                        m_friendlyFireTimeFromLastPlayed = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    }
                }

                // targed destroyed notification
                if (factionRelation == MyFactionRelationEnum.Enemy && died)
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.HudTargetDestroyed);
                }

                // Only when pilot
                if (died && MyMultiplayerGameplay.IsRunning && MinerWars.AppCode.Game.World.MyClientServer.LoggedPlayer != null)
                {
                    MyMultiplayerGameplay.Static.PlayerStatistics.PlayersKilled++;
                    MyMultiplayerGameplay.Static.UpdateStats();
                }
            }

            if (pilotDied)
            {
                Save = false;
                DisplayName = m_pilotDeadString;
            }

            if (damageType == MyDamageType.Explosion)
            {
                Shock(MySmallShipConstants.EXPLOSION_SHOCK_TIME);
            }

            UpdateBioChemEffect(pilotDied && ammoType == MyAmmoType.Biochem);
        }

        public void Shock(float shockTime)
        {
            m_shock_time = shockTime * (1 + MyMwcUtils.GetRandomFloat(-MySmallShipConstants.SHOCK_TIME_VAR, MySmallShipConstants.SHOCK_TIME_VAR));
        }


        private void LowHealth()
        {
            MyBotDesire desire = null;
            foreach (var item in m_desires)
            {
                if (item.DesireType == BotDesireType.LOW_HEALTH)
                {
                    desire = item;
                    break;
                }
            }

            if (desire == null)
            {
                desire = new MyLowHealthDesire();
                AddDesire(desire);
            }
        }

        #endregion

        #region Update

        protected override void UpdateMultiplayerPosition()
        {
            if (MyMultiplayerGameplay.IsRunning && !IsDummy && !this.IsSleeping)
            {
                MyMultiplayerGameplay.Static.UpdatePosition(this);
            }
        }

        public override void UpdateBeforeSimulation()
        {
            //MyHud.ChangeText(this, new StringBuilder(Health.ToString()), null, 10000, MyHudIndicatorFlagsEnum.SHOW_ALL);
            
            if (IsDummy)
                return;

            if (InitTime > 0)
            {
                InitTime -= MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                if (InitTime < 0)
                    InitTime = 0;
            }

            // Disable slowdown when bot is shocked by explosion
            m_shock_time -= MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            Config.MovementSlowdown.SetValue(!IsShocked());

            if (IsDead())
            {
                IsSleeping = false;
            }

            if (IsSleeping)
            {
                // HACK: When sleeping, make them stop here, because their update is not called
                Physics.AngularVelocity *= 0.93f;
                Physics.LinearVelocity *= 0.93f;

                if (m_currentBehavior != null)
                {
                    IsSleeping = m_currentBehavior.ShouldFallAsleep(this);
                }
                else
                {
                    IsSleeping = IsFarEnoughToBeAsleep();  // no behaviour: wake up based on player distance
                }
                return;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("SmallShipBot base.Update");
            base.UpdateBeforeSimulation();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (MyGuiScreenGamePlay.Static.IsEditorActive() || 
                MyMwcFinalBuildConstants.DisableEnemyBots ||
                !BotLogic)
            {
                return;
            }

            if (!IsDead() && 
                ActiveAI)
            {
                RefillAmmoAndResources();

                // Don't detect enemies when leader is too far
                if (!IsLeaderFar())
                {
                    DetectEnemies();
                }

                UpdateBehavior();

                if (m_currentBehavior != null)
                {
                    m_currentBehavior.Update(this);
                }
            }

            if (m_biochemEffect != null)
            {
                m_biochemEffect.UserBirthMultiplier = MathHelper.Clamp(0.1f * Physics.LinearVelocity.Length(), 0.5f, 2f);
                m_biochemEffect.UserRadiusMultiplier = MathHelper.Clamp(0.01f * Physics.LinearVelocity.Length(), 0.75f, 1.5f);

                m_biochemEffect.WorldMatrix = m_biochemEffectLocalMatrix.HasValue ? m_biochemEffectLocalMatrix.Value * WorldMatrix : WorldMatrix;
            }
        }

        public bool IsFarEnoughToBeAsleep()
        {
            if (MySession.PlayerShip == null)  // don't fall asleep if there's no player
                return false;
            var distanceSquaredFromNearestPointOfInterest =
                MyGuiScreenGamePlay.Static.GetDistanceSquaredFromNearestPointOfInterest(GetPosition());
            return distanceSquaredFromNearestPointOfInterest > SleepDistance * SleepDistance;
        }

        #endregion

        #region Movement

        private float TuneRotation(float angle)
        {
            return MathHelper.Clamp(angle * angle * angle, -0.75f, 0.75f);
        }

        public void Move(Vector3 moveTo, Vector3 lookTarget, Vector3 targetUp, bool afterburner, float deadDistance = 5.0f, float dampDistance = 5, float customDamping = 1.0f, float rotationSpeed = 100, bool slowRotation = false)
        {
            if (LookTarget != null)
            {
                lookTarget = LookTarget.GetPosition();
                targetUp = LookTarget.WorldMatrix.Up;
            }

            if (IsShocked())
            {
                return;
            }

            float MAX_SPEED = 150;
            float ROLL_SPEED = 10;
            float SPEED_CORRECTION_WEIGHT = 4;
            float ROTATION_DAMPING = 0.3f;

            Matrix tempInvertedWorldMatrix = GetWorldMatrixInverted();
            tempInvertedWorldMatrix.Translation = Vector3.Zero;
            Vector3 localAngularVelocity = MyUtils.GetTransform(Physics.AngularVelocity, ref tempInvertedWorldMatrix);

            Vector3 botToLookTarget = lookTarget - GetPosition();
            Vector3 normalizedBotToTarget = Vector3.Normalize(botToLookTarget);

            // ROTATE
            float fp = Vector3.Dot(WorldMatrix.Forward, botToLookTarget);
            float up = Vector3.Dot(WorldMatrix.Up, botToLookTarget);
            float lp = Vector3.Dot(WorldMatrix.Left, botToLookTarget);

            Vector2 rotationIndicator;
            float rollIndicator;
            if (slowRotation)
            {
                rotationIndicator = new Vector2(
                    -TuneRotation((float)Math.Atan2(up, fp)) * rotationSpeed + localAngularVelocity.X * 30,
                    -TuneRotation((float)Math.Atan2(lp, fp)) * rotationSpeed + localAngularVelocity.Y * 30);
                rollIndicator = (-TuneRotation((float)Math.Atan2(
                    Vector3.Dot(targetUp, WorldMatrix.Left),
                    Vector3.Dot(targetUp, WorldMatrix.Up))) + localAngularVelocity.Z * ROTATION_DAMPING) * ROLL_SPEED;
            }
            else
            {
                rotationIndicator = new Vector2(
                    -(float)Math.Atan2(up, fp) * rotationSpeed + localAngularVelocity.X * 30,
                    -(float)Math.Atan2(lp, fp) * rotationSpeed + localAngularVelocity.Y * 30);
                rollIndicator = (-(float)Math.Atan2(
                    Vector3.Dot(targetUp, WorldMatrix.Left),
                    Vector3.Dot(targetUp, WorldMatrix.Up)) + localAngularVelocity.Z * ROTATION_DAMPING) * ROLL_SPEED;
            }
            
            // MOVE
            // Correct actual speed to adjust direction
            Vector3 moveToVector = moveTo - GetPosition();
            float moveToDistance = moveToVector.Length();

            Vector3 normalizedMoveTo = moveToDistance == 0 ? WorldMatrix.Forward : moveToVector / moveToDistance;
            Vector3 speedCorrection = normalizedMoveTo * MAX_SPEED * SpeedModifier - (Physics.LinearVelocity - Vector3.Dot(Physics.LinearVelocity, normalizedMoveTo) * normalizedMoveTo) * SPEED_CORRECTION_WEIGHT;

            if (speedCorrection.LengthSquared() > 0)
            {
                float damping = MathHelper.Clamp((moveToDistance - deadDistance) / dampDistance, 0, 1) * customDamping * SpeedModifier;

                // Convert to local coordinates
                Vector3 moveIndicator = Vector3.Transform(Vector3.Normalize(speedCorrection), tempInvertedWorldMatrix);

                bool useAfterburner =
                    (afterburner && moveIndicator.Z < -0.7f && (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_afterBurnerOffTime > MyAIConstants.MIN_AFTERBURNER_OFF_TIME)) ||
                    ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_afterBurnerOnTime) < MyAIConstants.MIN_AFTERBURNER_ON_TIME);

                if (useAfterburner)
                {
                //    moveIndicator = new Vector3(moveIndicator.X * 4, moveIndicator.Y * 4, moveIndicator.Z / (afterburner ? 1f : GetEngineProperties().AfterburnerSpeedMultiplier));
                }

                if (m_wasAfterBurnerOn && !useAfterburner)
                {
                    m_afterBurnerOffTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                }
                
                if (!m_wasAfterBurnerOn && useAfterburner)
                {
                    m_afterBurnerOnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                }

                m_wasAfterBurnerOn = useAfterburner;

                MoveAndRotate(moveIndicator * damping, rotationIndicator, rollIndicator, useAfterburner);
            }

            m_debugMovePosition = moveTo;
        }

        #endregion

        #region Finding route

        private bool m_findingRoute = false;
        public bool TryFindRoute(MySmallShip target)
        {
            if (m_findingRoute) 
                return false;

            m_findingRoute = true;

            MyRoutefindingHelper.FindRouteInBackground(this, target.RouteMemory, RouteFoundCallback, null);
            return true;
        }

        private Vector3? m_routePosition;
        private void RouteFoundCallback(Vector3? routePosition, object userData)
        {
            m_routePosition = routePosition;
            m_findingRoute = false;
        }

        public bool TryGetRoute(out Vector3? routePosition)
        {
            if (m_findingRoute == false)
            {
                routePosition = m_routePosition; 
                return true;
            }
            else
            {
                routePosition = null; 
                return false;
            }
        }

        private bool m_findingPath = false;
        public bool TryFindPath(Vector3 goal)
        {
            if (m_findingPath) return false;  // already finding the path
            m_findingPath = true;
            MyPathfindingHelper.FindPathInBackground(WorldAABB.GetCenter(), goal, PathFoundCallback, null, useRaycasts: MySession.PlayerFriends.Contains(this));
            return true;
        }

        private List<Vector3> m_path = null;
        private void PathFoundCallback(List<Vector3> path, object userData)
        {
            m_path = path;
            m_findingPath = false;
        }

        public bool TryGetPath(out List<Vector3> path)
        {
            if (m_findingPath == false) { path = m_path; return true; }
            else { path = null; return false; }
        }
        
        public bool TryTestPosition(Vector3 position, Vector3 targetPosition)
        {
            if (m_testPositionTask.IsComplete)
            {
                m_testingPosition = false;
            }

            if (!m_testingPosition)
	        {
                m_testPositionJob.Start(this, position, targetPosition);
                m_testPositionTask = Parallel.Start(m_testPositionJob);
                m_testingPosition = true;

                return true;
	        }
            
            return false;
        }

        public bool TryGetTestPositionResult(ref Vector3? result)
        {
            if (m_testingPosition && m_testPositionTask.IsComplete)
	        {
                result = m_testPositionJob.Result;
                m_testingPosition = false;
                return true;
	        }
            
            return false;
        }

        #endregion

        #region Desires & behaviors

        private void AddDesire(MyBotDesire desire)
        {
            m_desires.Add(desire);
            m_desires.Sort(m_aiTemplate.CompareDesires);
        }

        private void UpdateBehavior()
        {
            // Remove topmost ignored desires
            while (m_desires.Count > 0 && m_aiTemplate.IsIgnored(m_desires[m_desires.Count - 1].DesireType))
            {
                m_desires.RemoveAt(m_desires.Count - 1);
            }

            foreach (MyBotDesire desire in m_desires)
            {
                desire.Update();
            }

            // Remove invalid behavior
            if (m_currentBehavior != null && (
                m_currentBehavior.IsInvalid() || (m_currentBehavior.SourceDesire != null && m_currentBehavior.SourceDesire.IsInvalid(this))))   // invalid desire or behavior
            {
                m_desires.Remove(m_currentBehavior.SourceDesire);
                m_currentBehavior.Close(this);
                m_currentBehavior = null;
            }


            if (m_desires.Count == 0)
            {
                if (m_currentBehavior == null ||
                    m_aiTemplate.GetBehaviorType(BotDesireType.IDLE) != m_currentBehavior.GetBehaviorType() ||    // if AItemplate is changed
                    m_currentBehavior.SourceDesire != null)                                     // current behavior is not idle behavior
                {
                    m_currentBehavior = m_aiTemplate.GetBehavior(BotDesireType.IDLE);
                    m_currentBehavior.Init(this);
                }
            }
            else
            {
                // Select first desire (sorted)
                MyBotDesire desire = m_desires[m_desires.Count - 1];

                // Don't update current behavior if it's the same
                if (m_currentBehavior != null && m_currentBehavior.SourceDesire != null &&
                    m_currentBehavior.GetBehaviorType() == m_aiTemplate.GetBehaviorType(m_currentBehavior.SourceDesire.DesireType) && 
                    m_currentBehavior.SourceDesire == desire)
                {
                    return;
                }

                if (m_currentBehavior != null)
                {
                    m_currentBehavior.Close(this);
                }
                
                m_currentBehavior = m_aiTemplate.GetBehavior(desire.DesireType);
                if (m_currentBehavior != null)
                {
                    m_currentBehavior.SourceDesire = desire;
                    m_currentBehavior.Init(this);
                }
            }
        }        

        #endregion 

        #region Debug draw

        public StringBuilder GetDebugHudString()
        {
            return new StringBuilder(string.Format("{0}{1}",
                IsSleeping ? "Sleeping " : "", 
                m_currentBehavior != null ? m_currentBehavior.GetDebugHudString() : "no behavior"));
        }

        public override bool DebugDraw()
        {
            if (MyGuiManager.GetScreenDebugBot() == null)
                return base.DebugDraw();

            MyDebugDraw.DrawAxis(WorldMatrix, 10, 1);
            MyDebugDraw.DrawLine3D(GetPosition(), m_debugMovePosition, Color.Orange, Color.Orange);
            MyDebugDraw.DrawLine3D(GetPosition(), GetPosition() + WorldMatrix.Forward * 1000, Color.Orange, Color.Yellow);
            MyDebugDraw.DrawSphereWireframe(m_debugMovePosition, 1, Vector3.One, 1);

            if (m_path != null)
            {
                MyDebugDraw.DrawSphereWireframe(m_path[0], 2, new Vector3(0, 1, 1), 1);
                for (int i = 1; i < m_path.Count; i++)
                {
                    MyDebugDraw.DrawSphereWireframe(m_path[i], 2, new Vector3(0, 1, 1), 1);
                    MyDebugDraw.DrawLine3D(m_path[i - 1], m_path[i], Color.Blue, Color.Cyan);
                }
            }

            if (m_currentBehavior != null)
            {
                m_currentBehavior.DebugDraw();
                if (m_currentBehavior.SourceDesire != null && m_currentBehavior.SourceDesire.GetEnemy() != null)
                {
                    MyDebugDraw.DrawLine3D(GetPosition(), m_currentBehavior.SourceDesire.GetEnemy().GetPosition(), Color.Red, Color.Red);
                }
            }

            /*
            // Draw visual detection frustrum
            float tgFovOver2 = (float)Math.Tan(MySmallShipConstants.BOT_FOV / 2);
            float normalRadius = tgFovOver2 * MySmallShipConstants.BOT_FOV_RANGE;
            float hiddenRadius = tgFovOver2 * MySmallShipConstants.BOT_FOV_RANGE_HIDDEN;

            Vector3? prevHiddenRangePoint = null;
            Vector3? prevNormalRangePoint = null;
            
            Vector3 normalCenter = WorldMatrix.Translation + WorldMatrix.Forward * MySmallShipConstants.BOT_FOV_RANGE;
            Vector3 hiddenCenter = WorldMatrix.Translation + WorldMatrix.Forward * MySmallShipConstants.BOT_FOV_RANGE_HIDDEN;

            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 6)
            {
                float x = (float)Math.Cos(i);
                float y = (float)Math.Sin(i);

                Vector3 hiddenRangePoint = hiddenCenter + WorldMatrix.Up * y * hiddenRadius + WorldMatrix.Right * x * hiddenRadius;
                Vector3 normalRangePoint = normalCenter + WorldMatrix.Up * y * normalRadius + WorldMatrix.Right * x * normalRadius;

                //MyDebugDraw.DrawLine3D(GetPosition(), hiddenRangePoint, Color.Red, Color.Red);
                //MyDebugDraw.DrawLine3D(hiddenRangePoint, normalRangePoint, Color.Green, Color.Green);

                MyDebugDraw.DrawLine3D(hiddenCenter, hiddenRangePoint, Color.Red, Color.Red);
                MyDebugDraw.DrawLine3D(normalCenter, normalRangePoint, Color.Green, Color.Green);

                if (prevHiddenRangePoint.HasValue && prevNormalRangePoint.HasValue)
                {
                    MyDebugDraw.DrawLine3D(prevHiddenRangePoint.Value, hiddenRangePoint, Color.Red, Color.Red);
                    MyDebugDraw.DrawLine3D(prevNormalRangePoint.Value, normalRangePoint, Color.Green, Color.Green);
                }

                prevHiddenRangePoint = hiddenRangePoint;
                prevNormalRangePoint = normalRangePoint;
            }
            */
            return base.DebugDraw();
        }

        #endregion

        #region Enemies


        /// <summary>
        /// Finds nearby enemy ship
        /// </summary>
        /// <returns></returns>
        private void DetectEnemies()
        {
            const int DETECT_TIME_MS = 1000;

            if (!m_detectingEnemies && MyMinerGame.TotalGamePlayTimeInMilliseconds > (m_lastDetect + DETECT_TIME_MS))
            {
                m_detectEnemiesJob.Start(this);
                m_detectEnemiesTask = Parallel.Start(m_detectEnemiesJob);
                m_detectingEnemies = true;
            }
            else if (m_detectEnemiesTask.IsComplete && m_detectingEnemies)
            {
                // To prevent same detection time for bots spawned in one moment
                m_lastDetect = MyMinerGame.TotalGamePlayTimeInMilliseconds + MyMwcUtils.GetRandomInt(DETECT_TIME_MS / 3);
                m_detectingEnemies = false;

                // Detected entity
                if (m_detectEnemiesJob.ClosestEnemy != null)
                {
                    AddSeenEnemy(m_detectEnemiesJob.ClosestEnemy);
                    m_detectEnemiesJob.ClosestEnemy = null;
                }

                // Visualy detected entity
                if (m_detectEnemiesJob.ClosestVisual != null)
                {
                    AddCuriousLocation(m_detectEnemiesJob.ClosestVisual.GetPosition(), m_detectEnemiesJob.ClosestVisual);
                    m_detectEnemiesJob.ClosestVisual = null;
                }
                m_detectEnemiesJob.Finish();
            }
        }

        public void AddSeenEnemy(MyEntity enemy)
        {
            if (enemy != null)
            {
                Debug.Assert(!enemy.Closed);

                if (LeaderLostEnabled && Leader != null && Vector3.DistanceSquared(Leader.GetPosition(), enemy.GetPosition()) > MyAIConstants.FAR_LEADER_DISTANCE_SQR)
                    return;

                MyBotDesire desire = null;
                foreach (var item in m_desires)
                {
                    if (item.DesireType == BotDesireType.SEE_ENEMY && item.GetEnemy() == enemy)
                    {
                        desire = item;
                        break;
                    }
                }

                if (desire == null)
                {
                    desire = new MySeeEnemyDesire(enemy);
                    AddDesire(desire);
                }
            }
        }

        public void AddCuriousLocation(Vector3 location, MyEntity source)
        {
            Debug.Assert(source == null || !source.Closed);

            MyBotDesire desire = null;
            foreach (var item in m_desires)
            {
                if (item.DesireType == BotDesireType.CURIOUS && item.GetEnemy() == source)
                {
                    desire = item;
                    break;
                }
            }

            if (desire == null)
            {
                desire = new MyCuriousDesire(source, location);
                AddDesire(desire);
            }
        }

        public void Flash()
        {
            MyBotDesire desire = null;
            foreach (var item in m_desires)
            {
                if (item.DesireType == BotDesireType.FLASHED)
                {
                    desire = item;
                    break;
                }
            }

            if (desire == null)
            {
                desire = new MyFlashedDesire(3 * MyFlashBombConstants.FLASH_TIME);
                AddDesire(desire);
            }
        }

        public override void AtackedByEnemy(MyEntity damageSource)
        {
            // Leaders is far - ignore enemy attack
            if (IsLeaderFar() || (LeaderLostEnabled && Vector3.DistanceSquared(Leader.GetPosition(), damageSource.GetPosition()) > MyAIConstants.FAR_LEADER_DISTANCE_SQR))
            {
                return;
            }

            MyBotDesire desire = null;
            foreach (var item in m_desires)
            {
                if (item.DesireType == BotDesireType.ATACKED_BY_ENEMY && item.GetEnemy() == damageSource)
                {
                    desire = item;
                    break;
                }
            }

            if (desire == null)
            {
                desire = new MyAttackedByEnemyDesire(damageSource);
                AddDesire(desire);
            }
        }

        public MyEntity GetEnemy()
        {
            for (int i = m_desires.Count - 1; i >= 0; i--)
            {
                MyEntity desireEnemy = m_desires[i].GetEnemy();
                if (desireEnemy != null)
                {
                    return desireEnemy;
                }
            }
            return null;
        }

        public MyEntity GetClosestEnemy()
        {
            float closestDistanceSqr = float.PositiveInfinity;
            MyEntity closestEnemy = null;

            foreach (var desire in m_desires)
            {
                MyEntity enemy = desire.GetEnemy();
                if (enemy != null)
                {
                    float distanceSqr = Vector3.DistanceSquared(GetPosition(), enemy.GetPosition());
                    if (distanceSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceSqr;
                        closestEnemy = enemy;
                    }
                }
            }

            return closestEnemy;
        }

        #endregion

        #region Attacking

        public bool CanSeeTarget(MyEntity target)
        {
            bool canSeeTarget = true;
            if (MyFakes.ENABLE_BOTS_FOV_WHEN_RADAR_JAMMER)
            {
                MySmallShip smallShipTarget = target as MySmallShip;
                if (smallShipTarget != null && !smallShipTarget.IsHologram && smallShipTarget.HasRadarJammerActive())
                {
                    float distanceSqr = (GetPosition() - GetPosition()).LengthSquared();
                    float rangeOfViewSqr = smallShipTarget.IsHiddenFromBots() ? MyAIConstants.BOT_FOV_RANGE_HIDDEN : MyAIConstants.BOT_FOV_RANGE;
                    float targetRange = Vector3.Dot(WorldMatrix.Forward, target.GetPosition() - GetPosition());

                    canSeeTarget =
                        targetRange <= rangeOfViewSqr &&
                        Vector3.Dot(WorldMatrix.Forward, Vector3.Normalize(target.GetPosition() - GetPosition())) >= MyAIConstants.BOT_FOV_COS;
                }
            }
            return canSeeTarget;
        }


        private void UpdateBioChemEffect(bool start)
        {
            if (m_biochemEffect == null && start)
            {
                m_biochemEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Damage_SmokeBiochem);
                m_biochemEffect.AutoDelete = false;
                m_biochemEffect.UserBirthMultiplier = 0;
                m_biochemEffect.WorldMatrix = m_biochemEffectLocalMatrix.HasValue ? m_biochemEffectLocalMatrix.Value * WorldMatrix : WorldMatrix;
            }
        }

        public void Attack(MyEntity target)
        {
            Debug.Assert(!this.Closed);

            Debug.Assert(target != this);
            if (target == this)
                return;

            bool alreadyThere = false;
            
            // Invalidate all kill desires (maybe there's only one, but whatever)
            foreach (var desire in m_desires)
            {
                var killDesire = desire as MyKillDesire;
                if (killDesire == null) continue;

                if (target == killDesire.Target) alreadyThere = true;  // I already want to kill the target :)
                else killDesire.Invalidate();
            }

            // Create a new kill desire
            if (!alreadyThere)
                AddDesire(new MyKillDesire(target));
        }

        public void Idle()
        {
            m_aiTemplate.SetIdleBehavior(BotBehaviorType.IDLE);
        }

        public void Follow(MyEntity leader)
        {
            Debug.Assert(!this.Closed);

            Debug.Assert(leader != this);
            var smallShipLeader = Leader as MySmallShip;
            if (smallShipLeader != null)
            {
                smallShipLeader.Followers.Remove(this);
            }

            Leader = leader;

            var newSmallShipLeader = leader as MySmallShip;
            if (newSmallShipLeader != null)
            {
                newSmallShipLeader.Followers.Add(this);
            }
            
            m_aiTemplate.SetIdleBehavior(BotBehaviorType.FOLLOW);
        }

        public void StopFollow()
        {
            Debug.Assert(!this.Closed);

            MySmallShip leaderShip = Leader as MySmallShip;
            if (leaderShip != null)
            {
                leaderShip.Followers.Remove(this);
            }
            Leader = null;
            Idle();
        }

        public void Patrol()
        {
            Debug.Assert(!this.Closed);

            m_aiTemplate.SetIdleBehavior(BotBehaviorType.PATROL);
        }

        public override bool IsDead()
        {
            return base.IsDead() || IsPilotDead();
        }

        public override bool IsPilotDead()
        {
            return IsDestructible ? PilotHealth / PilotMaxHealth <= MyGameplayConstants.HEALTH_RATIO_DEATH : false;
        }

        public bool IsShocked()
        {
            return m_shock_time > 0;
        }

        public void SetWaypointPath(string name)
        {
            CurrentWaypoint = null;
            WaypointPath = MyWayPointGraph.GetPath(name);
            
            if (WaypointPath != null && WaypointPath.WayPoints.Count > 0)
            {
                CurrentWaypoint = WaypointPath.WayPoints[0];
            }
        }

        public void SpoilHologram(MySmallShip smallShip)
        {
            Debug.Assert(smallShip.IsHologram);
            lock (m_hologramLock)
            {
                m_spoiledHolograms.Add(smallShip);
            }
        }

        public bool IsSpoiledHologram(MyEntity entity)
        {
            MySmallShip smallShip = entity as MySmallShip;
            if (smallShip != null && smallShip.IsHologram)
            {
                lock (m_hologramLock)
                {
                    return m_spoiledHolograms.Contains(smallShip);
                }
            }
            return false;
        }

        public void SetAITemplate(MyAITemplateEnum template)
        {
            MyBotAITemplate oldTemplate = m_aiTemplate;
            m_aiTemplate = MyBotAITemplates.GetTemplate(template);
            m_aiTemplate.SetIdleBehavior(oldTemplate.GetBehaviorType(BotDesireType.IDLE));
        }

        public bool IsLeaderLost()
        {
            if (LeaderLostEnabled && Leader != null)
            {
                float leaderDistanceSqr = (Leader.GetPosition() - GetPosition()).LengthSquared();
                if (leaderDistanceSqr > MyAIConstants.MAX_LEADER_DISTANCE_SQR)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLeaderFar()
        {
            if (LeaderLostEnabled && Leader != null)
            {
                float leaderDistanceSqr = (Leader.GetPosition() - GetPosition()).LengthSquared();
                if (leaderDistanceSqr > MyAIConstants.FAR_LEADER_DISTANCE_SQR)
                {
                    return true;
                }
            }
            return false;
        }


        public void LaunchAlarm(MySmallShip source)
        {
            foreach (var entity in MyEntities.GetEntities())
            {
                var container = entity as MyPrefabContainer;
                if (container != null)
                {
                    container.LaunchAlarm(this, source);
                }
            }
        }

        #endregion

        /// <summary>
        /// For debugging purposes
        /// </summary>
        public override string ToString()
        {
            return "Bot: " + DisplayName.ToString() + ", ID: " + (EntityId.HasValue ? EntityId.Value.NumericValue.ToString() : "null");
        }

        #region Gets

        public int GetDangerZoneID()
        {
            return m_dangerZoneId;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base baseBuilder = base.GetObjectBuilderInternal(getExactCopy);

            var builder = baseBuilder as MyMwcObjectBuilder_SmallShip_Bot;
            if (builder != null)
            {
                builder.AITemplate = m_aiTemplate.TemplateId;
                builder.Aggressivity = Aggressivity;
                builder.SeeDistance = SeeDistance;
                builder.SleepDistance = SleepDistance;
                builder.PatrolMode = PatrolMode;
                builder.Leader = Leader != null && Leader.EntityId.HasValue ? Leader.EntityId.Value.NumericValue : (uint?)null;
                builder.IdleBehavior = AITemplate.GetIdleBehavior();
                builder.LeaderLostEnabled = LeaderLostEnabled;
                builder.ActiveAI = ActiveAI;
                builder.SlowDown = SpeedModifier;
            }
            return baseBuilder;
        }


       
        public bool IsParked() 
        {
            return AIPriority == -1 && IsHiddenFromBots() && !ActiveAI;
        }

        public override MyShipTypePhysicsProperties GetPhysicsProperties()
        {
            return m_shipTypeProperties.PhysicsForBot;
        }

        #endregion

        public void SetEquip(List<MyMwcObjectBuilder_SmallShip_Weapon> weapons, List<MyMwcObjectBuilder_InventoryItem> inventoryItems)
        {
            // Cleanup Weapons and inventory
            Weapons.RemoveAllWeapons(false, false);
            List<MyInventoryItem> removeItems = new List<MyInventoryItem>();
            foreach (var item in Inventory.GetInventoryItems())
            {
                if (item.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Weapon || item.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Ammo)
                {
                    removeItems.Add(item);
                }
            }
            
            Inventory.RemoveInventoryItems(removeItems, true);

            Weapons.Init(weapons, new List<MyMwcObjectBuilder_AssignmentOfAmmo>());

            foreach (var item in inventoryItems)
            {
                Inventory.AddInventoryItem(MyInventory.CreateInventoryItemFromInventoryItemObjectBuilder(item));
            }

            SetupWeapons(GetObjectBuilderInternal(true) as MyMwcObjectBuilder_SmallShip_Bot);
        }

        public static void PlayFriendlyFireCue()
        {
            if (m_friendlyFireCue == null || !m_friendlyFireCue.Value.IsPlaying)
            {
                m_friendlyFireCue = MyAudio.AddCue2D(MySoundCuesEnum.HudFriendlyFireWarning);
                m_friendlyFireTimeFromLastPlayed = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }
    }
}
