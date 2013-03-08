namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using App;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using GUI.Helpers;
    using MinerWarsMath;
    using Models;
    using SubObjects;
    using SysUtils.Utils;
    using UniversalLauncher;
    using Utils;
    using MinerWars.AppCode.Game.Inventory;
    using MinerWars.AppCode.Game.Entities.Weapons.Ammo;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using System.Runtime.Serialization;
    using System.Reflection;
    using KeenSoftwareHouse.Library.Extensions;
    using System.Diagnostics;
    using MinerWars.AppCode.Game.Sessions;
    using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
    using MinerWars.AppCode.Game.GUI;
    using MinerWars.AppCode.Game.Managers.Session;

    internal class MySmallShipWeapons
    {
        /// <summary>
        /// This factory class generate new instances of weapons from weapon type
        /// </summary>
        static class MySmallShipWeaponFactory
        {
            /// <summary>
            /// Returns new instance of weapon
            /// </summary>
            /// <param name="weaponType">Weapon type</param>
            /// <returns></returns>
            public static MySmallShipGunBase GenerateWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType)
            {
                MySmallShipGunBase newGun;
                switch (weaponType)
                {
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon:
                        newGun = new MyAutocanonGun();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer:
                        newGun = new MyAutomaticRifleSilencerGun();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher:
                        newGun = new MyCrusherDrill();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser:
                        newGun = new MyLaserDrill();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear:
                        newGun = new MyNuclearDrill();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw:
                        newGun = new MySawDrill();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal:
                        newGun = new MyThermalDrill();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure:
                        newGun = new MyPressureDrill();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher:
                        newGun = new MyMissileLauncherGun();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device:
                        newGun = new MyHarvestingDevice();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun:
                        newGun = new MyMachineGun();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon:
                        newGun = new MyCannonGun();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun:
                        newGun = new MyShotGun();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper:
                        newGun = new MySniperGun();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back:
                        newGun = new MyUniversalLauncher();
                        break;

                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front:
                        newGun = new MyUniversalLauncher();
                        break;

                    default:
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                        break;
                }
                return newGun;
            }
        }

        #region Static and consts
        private static readonly Dictionary<string, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum> m_weaponsModelNames;
        private const string GUN_PREFIX_NAME = "GUN_";
        private const string DRILL_NAME = "DRILL";
        private const string HARVESTER_NAME = "HARVESTER";


        static MySmallShipWeapons()
        {
            m_weaponsModelNames = new Dictionary<string, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum>()
            {
                {"AUTOCANNON", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon},
                {"SHOTGUN", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun},
                {"MACHINEGUN", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun},
                {"RIFLE", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer},
                {"SNIPER", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper},
                {"MISSILE", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher},
                {"CANNON", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon},
                {"UNIVERSAL_FRONT", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front},
                {"UNIVERSAL_BACK", MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back},                    
            };
        }
        #endregion

        #region Fields
        private List<MyWeaponSlot> m_weaponSlots;       // weapon's slots

        private MyWeaponSlot m_drillSlot;               // drill's slot

        private MyWeaponSlot m_harvestingDeviceSlot;    // harvesting device's slot

        private MySmallShip m_ship;                     // reference to ship

        private MyAmmoAssignments m_ammoAssignments;    // ammo assignmens  

        private List<MySmallShipGunBase> m_allMountedWeapons;    // all mounted weapons

        private List<MyInventoryItem> m_helperInventoryItems = new List<MyInventoryItem>();

        private bool m_isClosed;

        private Weapons.OnWeaponMounting m_weaponMounting;
        private Weapons.OnWeaponDismouting m_weaponDismounting;
        #endregion

        #region Ctors
        /// <summary>
        /// Creates new intance of small ship weapons
        /// </summary>
        /// <param name="ship">Ship</param>
        /// <param name="shipTypeProperties">Ship's properties</param>
        /// <param name="maxSlots">Max weapon slots which can be mounted with a gun</param>
        public MySmallShipWeapons(MySmallShip ship, MyShipTypeProperties shipTypeProperties, int maxSlots)
        {
            m_weaponMounting = new Weapons.OnWeaponMounting(OnWeaponMounting);
            m_weaponDismounting = new Weapons.OnWeaponDismouting(OnWeaponDismounting);

            Ship = ship;
            AmmoAssignments = new MyAmmoAssignments(this);
            AmmoInventoryItems = new MyAmmoInventoryItems();
            Ship.Inventory.OnInventoryContentChange += OnInventoryContentChange;
            FillAmmoInventoryItemsFromInventory();
            MuzzleFlashLastTime = 0;
            Visible = true;
            InitSlots(shipTypeProperties, maxSlots);

            m_allMountedWeapons = new List<MySmallShipGunBase>();
            m_isClosed = false;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Ammo assignments
        /// </summary>
        public MyAmmoAssignments AmmoAssignments
        {
            get
            {
                return m_ammoAssignments;
            }
            private set
            {
                m_ammoAssignments = value;
            }
        }

        /// <summary>
        /// Ammo inventory items
        /// </summary>
        public MyAmmoInventoryItems AmmoInventoryItems { get; private set; }

        /// <summary>
        /// Last time of muzzle flash
        /// </summary>
        public int MuzzleFlashLastTime { get; set; }

        private MyWeaponSlot DrillSlot
        {
            get
            {
                return m_drillSlot;
            }
            set
            {
                m_drillSlot = value;
            }
        }

        private MyWeaponSlot HarvestingDeviceSlot
        {
            get
            {
                return m_harvestingDeviceSlot;
            }
            set
            {
                m_harvestingDeviceSlot = value;
            }
        }

        private MySmallShip Ship
        {
            get
            {
                return m_ship;
            }
            set
            {
                m_ship = value;
            }
        }

        /// <summary>
        /// Returns all weapon's slots
        /// </summary>
        /// <returns></returns>
        public List<MyWeaponSlot> WeaponSlots
        {
            get
            {
                return m_weaponSlots;
            }
            private set
            {
                m_weaponSlots = value;
            }
        }

        public bool Visible { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Initialize small ship's weapons from object builders
        /// </summary>
        /// <param name="weaponObjectBuilders">Weapon's objectbuilders</param>
        /// <param name="assignmentOfAmmoObjectBuilders">Assignment of ammo objectbuilders</param>
        public void Init(List<MyMwcObjectBuilder_SmallShip_Weapon> weaponObjectBuilders, List<MyMwcObjectBuilder_AssignmentOfAmmo> assignmentOfAmmoObjectBuilders)
        {
            Debug.Assert(!m_isClosed);
            RemoveAllWeapons();
            if (weaponObjectBuilders != null)
            {
                foreach (MyMwcObjectBuilder_SmallShip_Weapon objectBuilder in weaponObjectBuilders)
                {
                    if (!MyMwcObjectBuilder_InventoryItem.IsDisabled(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)objectBuilder.GetObjectBuilderId()))
                    {
                        AddWeapon(objectBuilder);
                    }
                }
            }

            if (assignmentOfAmmoObjectBuilders != null)
            {
                AmmoAssignments.Init(assignmentOfAmmoObjectBuilders);
            }
        }

        /// <summary>
        /// Adds weapon. First try find first empty preferred slot and then try find first empty slot
        /// </summary>
        /// <param name="weaponObjectBuilder">Weapon's object builder</param>
        public MySmallShipGunBase AddWeapon(MyMwcObjectBuilder_SmallShip_Weapon weaponObjectBuilder)
        {
            if (!MySession.Is25DSector)
            {
                if (weaponObjectBuilder.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear)
                    weaponObjectBuilder.WeaponType = MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher;
            }


            MySmallShipGunBase newWeapon = MySmallShipWeaponFactory.GenerateWeapon(weaponObjectBuilder.WeaponType);
            if (newWeapon is MyDrillBase)
            {
                AddDrill(newWeapon as MyDrillBase, weaponObjectBuilder);
            }
            else if (newWeapon is MyHarvestingDevice)
            {
                AddHarvestingDevice(newWeapon as MyHarvestingDevice, weaponObjectBuilder);
            }
            else
            {
                AddWeapon(newWeapon, weaponObjectBuilder);
            }
            var parentShip = newWeapon.Parent as MySmallShip;
            if(parentShip != null)
            {
                newWeapon.NearFlag = parentShip.IsCameraInsideMinerShip();
            }
            return newWeapon;
        }

        /// <summary>
        /// Adds drill to drill's slot
        /// </summary>
        /// <param name="drillObjectBuilder">Drill's object builder</param>
        public void AddDrill(MyMwcObjectBuilder_SmallShip_Weapon drillObjectBuilder)
        {
            MyDrillBase newDrill = MySmallShipWeaponFactory.GenerateWeapon(drillObjectBuilder.WeaponType) as MyDrillBase;
            if (newDrill == null)
            {
                throw new ArgumentException("Object builder is not drill object builder");
            }
            AddDrill(newDrill, drillObjectBuilder);
        }

        /// <summary>
        /// Adds harvesting device to harvesting device's slot
        /// </summary>
        /// <param name="harvestingDeviceObjectBuilder">Harvesting device's object builder</param>
        public void AddHarvestingDevice(MyMwcObjectBuilder_SmallShip_Weapon harvestingDeviceObjectBuilder)
        {
            MyHarvestingDevice newHarvestingDevice = MySmallShipWeaponFactory.GenerateWeapon(harvestingDeviceObjectBuilder.WeaponType) as MyHarvestingDevice;
            if (newHarvestingDevice == null)
            {
                throw new ArgumentException("Object builder is not harvesting device object builder");
            }
            AddHarvestingDevice(newHarvestingDevice, harvestingDeviceObjectBuilder);
        }

        /// <summary>
        /// Removes all weapons from ship (if remove drill or remove harvesting device is true, then remove them too)
        /// </summary>
        /// <param name="removeDrill">If true, removes drill from ship</param>
        /// <param name="removeHarvestingDevice">If true, removes harvesting device from ship</param>
        public void RemoveAllWeapons(bool removeDrill = true, bool removeHarvestingDevice = true)
        {
            if (removeDrill)
            {
                RemoveDrill();
            }
            if (removeHarvestingDevice)
            {
                RemoveHarvestingDevice();
            }
            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                RemoveWeapon(i);
            }
        }

        /// <summary>
        /// Removes weapon from ship
        /// </summary>
        /// <param name="index">Index of slot</param>
        public void RemoveWeapon(int index)
        {
            if (index >= WeaponSlots.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MyWeaponSlot weaponSlot = WeaponSlots[index];
            if (weaponSlot.IsMounted())
            {
                RemoveWeaponFromSlotAndFromShip(weaponSlot);
            }
        }

        /// <summary>
        /// Removes drill from ship
        /// </summary>
        public void RemoveDrill()
        {
            if (DrillSlot.IsMounted())
            {
                RemoveWeaponFromSlotAndFromShip(DrillSlot);
            }
        }

        /// <summary>
        /// Removes harvesting device from ship
        /// </summary>
        public void RemoveHarvestingDevice()
        {
            if (HarvestingDeviceSlot.IsMounted())
            {
                RemoveWeaponFromSlotAndFromShip(HarvestingDeviceSlot);
            }
        }

        Queue<int> m_keysToShot = new Queue<int>(8);        
        /// <summary>
        /// Shot from weapon which is bind at fire key. Fire is delayed because we want to shoot from already simulated ship
        /// </summary>
        /// <param name="key">Fire key</param>
        public void Fire(MyMwcObjectBuilder_FireKeyEnum key)
        {
            if(!m_keysToShot.Contains((int)key))
            {
                m_keysToShot.Enqueue((int)key);
            }
        }

        /// <summary>
        /// Returns if weapon with this type is mounted
        /// </summary>
        /// <param name="weaponType">Weapon's type</param>
        /// <returns></returns>
        public bool IsMounted(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType) 
        {
            foreach (MySmallShipGunBase gun in GetMountedWeaponsWithHarvesterAndDrill()) 
            {
                if (gun.WeaponType == weaponType) 
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Shot from weapon which is bind at fire key
        /// </summary>
        /// <param name="key">Fire key</param>
        void FirePrivate(MyMwcObjectBuilder_FireKeyEnum key)
        {
            //Do nothing if there are no ammo or weapons or assignments


            this.m_ship.InitGroupMaskIfNeeded();

            var drill = GetMountedDrill();
            if ((!MySession.Is25DSector && (drill != null && (drill.CurrentState == MyDrillStateEnum.Activated || drill.CurrentState == MyDrillStateEnum.Drilling)))
                || (MySession.Is25DSector && drill != null && key == MyMwcObjectBuilder_FireKeyEnum.Secondary))
            {
                drill.Shot(null);
                return;
            }

            List<MySmallShipGunBase> mountedGuns = GetMountedWeaponsWithHarvesterAndDrill();
            /*if (AmmoInventoryItems.Count() == 0 || AmmoAssignments.Count() == 0 || mountedGuns.Count == 0)
                return;*/

            MyAmmoAssignment ammoAssignment = AmmoAssignments.GetAmmoAssignment(key);

            //If type or group is not found in object builders, do nothing
            if (ammoAssignment == null) return;

            //If type and group is bad combination, do nothing
            if (!MyGuiSmallShipHelpers.GetWeaponType(ammoAssignment.AmmoType, ammoAssignment.AmmoGroup).HasValue) return;

            MyInventoryItem usedAmmoInventoryItem = null;
            foreach (MyInventoryItem ammoItem in AmmoInventoryItems.GetAmmoInventoryItems(ammoAssignment.AmmoType))
            {
                if (ammoItem.Amount > 0)
                {
                    usedAmmoInventoryItem = ammoItem;
                    break;
                }
            }

            bool thereWasShot = false;
            bool wasCountedMuzzleTime = false;

            //  Shot from gun
            foreach (MySmallShipGunBase gun in mountedGuns)
            {
                if (MyGuiSmallShipHelpers.GetWeaponType(ammoAssignment.AmmoType, ammoAssignment.AmmoGroup).Value == gun.WeaponType)
                {
                    bool shot = false;
                    if (usedAmmoInventoryItem != null && usedAmmoInventoryItem.Amount > 0)
                    {
                        shot = gun.Shot((MyMwcObjectBuilder_SmallShip_Ammo)usedAmmoInventoryItem.GetInventoryItemObjectBuilder(false));
                        if (shot && MyMultiplayerGameplay.IsRunning && !m_ship.IsDummy)
                        {
                            var ammo = ((MyMwcObjectBuilder_SmallShip_Ammo)usedAmmoInventoryItem.GetInventoryItemObjectBuilder(false)).AmmoType;
                            MyMultiplayerGameplay.Static.Shoot(this.m_ship, this.m_ship.WorldMatrix, gun.WeaponType, ammo, this.Ship.TargetEntity, gun.LastShotId);
                        }
                    }
                    else
                    {
                        if (m_noAmmoSoundDelay + 175 < MyMinerGame.TotalGamePlayTimeInMilliseconds)
                        {
                            Audio.MyAudio.AddCue2dOr3d(Ship, Audio.MySoundCuesEnum.WepNoAmmo, Ship.GetPosition(), Ship.WorldMatrix.Forward, Ship.WorldMatrix.Up, Ship.Physics.LinearVelocity);
                            m_noAmmoSoundDelay = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        }
                    }

                    if (shot)
                    {
                        Ship.Inventory.RemoveInventoryItemAmount(ref usedAmmoInventoryItem, 1f);
                        thereWasShot = true;
                        Ship.WeaponShot(gun, ammoAssignment);
                    }

                    //  If we shot from gun that uses projectiles, we need to remember it as last time of muzzle flash
                    if ((shot == true) &&
                        MyGuiSmallShipHelpers.IsAmmoInGroup(ammoAssignment.AmmoType, MyMwcObjectBuilder_AmmoGroupEnum.Bullet) &&
                        !wasCountedMuzzleTime)
                    {
                        MuzzleFlashLastTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        wasCountedMuzzleTime = true;
                    }
                }
            }

            if (thereWasShot == true)
            {
                Ship.IncreaseHeadShake(MyHeadShakeConstants.HEAD_SHAKE_AMOUNT_AFTER_GUN_SHOT);
            }
        }

        private float m_noAmmoSoundDelay = 0;

        /// <summary>
        /// Shots from harvester
        /// </summary>
        public void FireHarvester()
        {
            var drill = GetMountedDrill();
            if (drill != null && drill.CurrentState != MyDrillStateEnum.InsideShip)
                return;

            MyHarvestingDevice harvester = GetMountedHarvestingDevice();
            if (harvester != null)
            {
                if (MyMultiplayerGameplay.IsRunning && !harvester.IsDummy)
                {
                    MyMultiplayerGameplay.Static.SpeacialWeaponEvent(MySpecialWeaponEventEnum.HARVESTER_FIRE, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device);
                }

                this.m_ship.InitGroupMaskIfNeeded();
                harvester.Shot(null);
            }
        }

        /// <summary>
        /// Shots from drill
        /// </summary>
        public void FireDrill()
        {
            var harvester = GetMountedHarvestingDevice();
            if (harvester != null && harvester.IsHarvesterActive)
                return;

            MyDrillBase drill = GetMountedDrill();
            if (drill != null)
            {
                this.m_ship.InitGroupMaskIfNeeded();
                drill.Eject();
            }
        }

        /// <summary>
        /// Returns all mounted weapons and drill and harvesting device
        /// </summary>
        /// <returns></returns>
        public List<MySmallShipGunBase> GetMountedWeaponsWithHarvesterAndDrill()
        {
            return m_allMountedWeapons;
        }

        /// <summary>
        /// Returns mounted harvesting device
        /// </summary>
        /// <returns></returns>
        public MyHarvestingDevice GetMountedHarvestingDevice()
        {
            if (HarvestingDeviceSlot.IsMounted())
            {
                return HarvestingDeviceSlot.MountedWeapon as MyHarvestingDevice;
            }
            return null;
        }

        /// <summary>
        /// Returns mounted drill
        /// </summary>
        /// <returns></returns>
        public MyDrillBase GetMountedDrill()
        {
            if (DrillSlot.IsMounted())
            {
                return DrillSlot.MountedWeapon as MyDrillBase;
            }
            return null;
        }

        /// <summary>
        /// Returns interstection with line
        /// </summary>
        /// <param name="line">Line</param>
        /// <returns></returns>
        public MyIntersectionResultLineTriangleEx? GetIntersectionWithLine(ref MyLine line)
        {
            MyIntersectionResultLineTriangleEx? result = null;

            //  Test against childs of this phys object (in this case guns)
            foreach (MyGunBase gun in GetMountedWeaponsWithHarvesterAndDrill())
            {
                MyIntersectionResultLineTriangleEx? intersectionGun;
                gun.GetIntersectionWithLine(ref line, out intersectionGun);
                result = MyIntersectionResultLineTriangleEx.GetCloserIntersection(ref result, ref intersectionGun);
            }

            return result;
        }

        /// <summary>
        /// Updates all weapons after integration
        /// </summary>
        public void UpdateAfterSimulation()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySmallShipWeapons.UAI fire");            
            while(m_keysToShot.Count > 0)
            {
                MyMwcObjectBuilder_FireKeyEnum keyToShot = (MyMwcObjectBuilder_FireKeyEnum)m_keysToShot.Dequeue();
                FirePrivate(keyToShot);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySmallShipWeapons.UAI weapons");
            
            /*
            foreach (MySmallShipGunBase gun in GetMountedWeaponstWithHarvesterAndDrill())
            {
                gun.UpdateAfterSimulation();
            }
            */

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Draws all weapons
        /// </summary>
        public void Draw()
        {
            if (Visible)
            {
                foreach (MySmallShipGunBase gun in GetMountedWeaponsWithHarvesterAndDrill())
                {
                    if (gun.Visible)
                        gun.Draw();
                }
            }
        }

        /// <summary>
        /// Draws all weapons normals
        /// </summary>
        public void DrawNormalVectors()
        {
            foreach (MySmallShipGunBase gun in GetMountedWeaponsWithHarvesterAndDrill())
            {
                gun.DebugDrawNormalVectors();
            }
        }

        /// <summary>
        /// Close all weapons
        /// </summary>
        public void Close()
        {
            //foreach (MySmallShipGunBase gun in GetMountedWeaponstWithHarvesterAndDrill())
            //{
            //    gun.Close();
            //}
            foreach (MyWeaponSlot weaponSlot in WeaponSlots) 
            {
                if (weaponSlot != null) 
                {
                    weaponSlot.OnWeaponDismouting -= m_weaponDismounting;
                    weaponSlot.OnWeaponMounting -= m_weaponMounting;
                    weaponSlot.Close();                    
                }
            }
            WeaponSlots.Clear();
            WeaponSlots = null;

            DrillSlot.OnWeaponDismouting -= m_weaponDismounting;
            DrillSlot.OnWeaponMounting -= m_weaponMounting;
            DrillSlot.Close();
            DrillSlot = null;

            HarvestingDeviceSlot.OnWeaponDismouting -= m_weaponDismounting;
            HarvestingDeviceSlot.OnWeaponMounting -= m_weaponMounting;
            HarvestingDeviceSlot.Close();
            HarvestingDeviceSlot = null;

            m_allMountedWeapons.Clear();
            m_allMountedWeapons = null;

            AmmoAssignments.Close();
            AmmoAssignments = null;

            AmmoInventoryItems.Close();
            AmmoAssignments = null;

            Ship.Inventory.OnInventoryContentChange -= OnInventoryContentChange;
            Ship = null;

            m_helperInventoryItems.Clear();
            m_helperInventoryItems = null;

            m_isClosed = true;
        }

        /// <summary>
        /// Returns amount of ammo which is bind at fire key
        /// </summary>
        /// <param name="key">Fire key</param>
        /// <returns></returns>
        public int GetAmountOfAmmo(MyMwcObjectBuilder_FireKeyEnum key)
        {
            if (AmmoInventoryItems.Count() == 0 || AmmoAssignments.Count() == 0) return 0;

            //Get ammo type
            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType = AmmoAssignments.GetAmmoType(key);
            if (ammoType == 0) return 0;

            //Get amount
            int result = 0;
            foreach (MyInventoryItem ammoInventoryItem in AmmoInventoryItems.GetAmmoInventoryItems(ammoType))
            {

                result += (int)ammoInventoryItem.Amount;
            }
            return result;
        }

        /// <summary>
        /// Returns weapons objectbuilders
        /// </summary>
        /// <returns></returns>
        public List<MyMwcObjectBuilder_SmallShip_Weapon> GetWeaponsObjectBuilders(bool getExactCopy)
        {
            List<MyMwcObjectBuilder_SmallShip_Weapon> weaponsObjectBuilders = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            foreach (MySmallShipGunBase weapon in GetMountedWeaponsWithHarvesterAndDrill())
            {
                weaponsObjectBuilders.Add((MyMwcObjectBuilder_SmallShip_Weapon)weapon.GetObjectBuilder(getExactCopy));
            }
            return weaponsObjectBuilders;
        }

        /// <summary>
        /// Returns max weapon's slots from ship's model enum
        /// </summary>
        /// <param name="shipModelEnum">Ship's model enum</param>
        /// <returns>Max weapon's slots</returns>
        public static int GetMaxWeaponsSlots(MyModelsEnum shipModelEnum) 
        {
            int slots = 0;
            List<MyModelSubObject> shipsSubObjects = MyModelSubObjects.GetModelSubObjects(shipModelEnum);
            while (true) 
            {
                string gunPrefix = GUN_PREFIX_NAME + (slots + 1).ToString("##00");

                // try find guns subobject by prefix (GUN_XX)
                MyModelSubObject subObjectGun = shipsSubObjects.Find(x => x.Name.StartsWith(gunPrefix));

                // if not found, then stop adding gun's positions, because there are no gun's subobjects
                if (subObjectGun == null)
                {
                    break;
                }
                slots++;
            }
            return slots;
        }

        public bool IsShooting()
        {
            return m_keysToShot.Count > 0;
        }

        public bool HasMountedWeapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType)
        {
            foreach (var weapon in m_allMountedWeapons)
	        {
                if (weapon.WeaponType == weaponType)
	            {
                    return true;
	            }
	        }
            return false;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Initialize slots by small ship's model and preferred weapon's positions
        /// </summary>
        /// <param name="shipTypeProperties">Type properties of ship</param>
        /// <param name="maxSlots">Maximum weapons that can be mounted</param>
        private void InitSlots(MyShipTypeProperties shipTypeProperties, int maxSlots)
        {
            // initialize slot for drill
            DrillSlot = new MyWeaponSlot();
            DrillSlot.WeaponSubObject = MyModelSubObjects.GetModelSubObject(shipTypeProperties.Visual.ModelLod0Enum, DRILL_NAME);
            DrillSlot.OnWeaponMounting += m_weaponMounting;
            DrillSlot.OnWeaponDismouting += m_weaponDismounting;

            // initialize slot for harvester
            HarvestingDeviceSlot = new MyWeaponSlot();
            HarvestingDeviceSlot.WeaponSubObject = MyModelSubObjects.GetModelSubObject(shipTypeProperties.Visual.ModelLod0Enum, HARVESTER_NAME);
            HarvestingDeviceSlot.OnWeaponMounting += m_weaponMounting;
            HarvestingDeviceSlot.OnWeaponDismouting += m_weaponDismounting;

            // initialize slots for weapons
            WeaponSlots = new List<MyWeaponSlot>();
            AddGunsPositionsFromShipModel(shipTypeProperties.Visual.ModelLod0Enum, maxSlots);
        }

        /// <summary>
        /// Creates gun's positions from ship's model. If find preferred position, then mark it as preffered.
        /// </summary>
        /// <param name="modelEnum">Ship's model</param>
        /// <param name="maxSlots">Weapons maximum on ship</param>
        /// <param name="maxWeapons"> </param>
        private void AddGunsPositionsFromShipModel(MyModelsEnum modelEnum, int maxSlots)
        {
            List<MyModelSubObject> shipsSubObjects = MyModelSubObjects.GetModelSubObjects(modelEnum);
            if (shipsSubObjects != null)
            {
                for (int i = 0; i < maxSlots; i++)
                {
                    string gunPrefix = GUN_PREFIX_NAME + (i + 1).ToString("##00");

                    // try find guns subobject by prefix (GUN_XX)
                    MyModelSubObject subObjectGun = shipsSubObjects.Find(x => x.Name.StartsWith(gunPrefix));

                    // if not found, then stop adding gun's positions, because there are no gun's subobjects
                    if (subObjectGun == null)
                    {
                        return;
                    }

                    // if not exists slot at this index, then create new one
                    if (WeaponSlots.Count <= i)
                    {
                        MyWeaponSlot newWeaponSlot = new MyWeaponSlot();
                        WeaponSlots.Add(newWeaponSlot);
                        newWeaponSlot.OnWeaponMounting += m_weaponMounting;
                        newWeaponSlot.OnWeaponDismouting += m_weaponDismounting;
                    }

                    WeaponSlots[i].WeaponSubObject = subObjectGun;

                    // if this gun's position is preffered by any gun
                    if (subObjectGun.Name.Length != gunPrefix.Length)
                    {
                        string prefferedGunName = subObjectGun.Name.Substring(gunPrefix.Length + 1, subObjectGun.Name.Length - gunPrefix.Length - 1);
                        MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weaponType;
                        if (m_weaponsModelNames.TryGetValue(prefferedGunName, out weaponType))
                        {
                            WeaponSlots[i].WeaponSubObject.AuxiliaryParam0 = (int)weaponType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns first empty slot by preferred position, if not founded then returns first empty slot
        /// </summary>
        /// <param name="weaponBuilder"></param>
        /// <returns></returns>
        private MyWeaponSlot GetEmptySlotToAdd(MyMwcObjectBuilder_SmallShip_Weapon weaponBuilder)
        {
            // first, try to find a slot that fits both type and location
            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                if (!WeaponSlots[i].IsMounted() &&
                    WeaponSlots[i].GetPreferedWeaponType() == weaponBuilder.WeaponType && 
                    WeaponSlots[i].IsSlotEligibleForLocation(GetSlotLocation(weaponBuilder)))
                {
                    return WeaponSlots[i];
                }
            }

            // then, try to find a slot that fits the location
            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                if (!WeaponSlots[i].IsMounted() &&
                    WeaponSlots[i].IsSlotEligibleForLocation(GetSlotLocation(weaponBuilder)))
                {
                    return WeaponSlots[i];
                }
            }

            /*
            // if preffered weapon's slot not founded, then try find first empty slot
            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                if (!WeaponSlots[i].IsMounted())
                {
                    return WeaponSlots[i];
                }
            }
            */

            return null;
        }

        MySlotLocationEnum GetSlotLocation(MyMwcObjectBuilder_SmallShip_Weapon weaponBuilder)
        {
            if (weaponBuilder.AutoMountLeft)
            {
                return MySlotLocationEnum.LeftSide;
            }

            if (weaponBuilder.AutoMountRight)
            {
                return MySlotLocationEnum.RightSide;
            }

            return MySlotLocationEnum.None;
        }

        public int GetNormalWeaponCount(MySlotLocationEnum? location)
        {
            int result = 0;

            foreach (var slot in WeaponSlots)
            {
                if ((location == null || slot.SlotLocation == location.Value) &&
                    slot.IsMounted() &&
                    slot.MountedWeapon.IsNormalWeapon)
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds weapon
        /// </summary>
        /// <param name="weapon">Weapon</param>
        /// <param name="weaponObjectBuilder">Weapon's object builder</param>
        /// <param name="maxWeapons"> </param>
        private void AddWeapon(MySmallShipGunBase weapon, MyMwcObjectBuilder_SmallShip_Weapon weaponObjectBuilder)
        {
            SetAutoMountIfExceededLimit(weaponObjectBuilder);

            MyWeaponSlot emptyWeaponSlot = GetEmptySlotToAdd(weaponObjectBuilder);
            if (emptyWeaponSlot != null)
            {
                switch (emptyWeaponSlot.SlotLocation)
                {
                    case MySlotLocationEnum.None:
                        break;
                    case MySlotLocationEnum.LeftSide:
                        weaponObjectBuilder.SetAutoMountLeft();
                        break;
                    case MySlotLocationEnum.RightSide:
                        weaponObjectBuilder.SetAutoMountRight();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                AddWeaponToSlot(emptyWeaponSlot, weapon, weaponObjectBuilder);
            }
        }

        void SetAutoMountIfExceededLimit(MyMwcObjectBuilder_SmallShip_Weapon weaponObjectBuilder)
        {
            if (!weaponObjectBuilder.IsNormalWeapon)
            {
                return;
            }

            const int universalLauncherCount = 2;
            var maxNormalWeapons = Ship.ShipTypeProperties.GamePlay.MaxWeapons - universalLauncherCount;
            var maxNormalWeaponsOnOneWing = maxNormalWeapons / 2;

            var tooManyLeft = GetNormalWeaponCount(MySlotLocationEnum.LeftSide) >= maxNormalWeaponsOnOneWing;
            var tooManyRight = GetNormalWeaponCount(MySlotLocationEnum.RightSide) >= maxNormalWeaponsOnOneWing;

            if (tooManyLeft && tooManyRight)
            {
                weaponObjectBuilder.SetAutoMount();
                return;
            }

            if (tooManyLeft)
            {
                weaponObjectBuilder.SetAutoMountRight();
            }

            if (tooManyRight)
            {
                weaponObjectBuilder.SetAutoMountLeft();
            }
        }

        /// <summary>
        /// Adds drill
        /// </summary>
        /// <param name="weapon">Drill</param>
        /// <param name="weaponObjectBuilder">Drill's object builder</param>
        private void AddDrill(MyDrillBase drill, MyMwcObjectBuilder_SmallShip_Weapon drillObjectBuilder)
        {
            if (DrillSlot.IsMounted())
            {
                RemoveWeaponFromSlotAndFromShip(DrillSlot);
            }
            AddWeaponToSlot(DrillSlot, drill, drillObjectBuilder);
        }

        /// <summary>
        /// Adds harvesting device
        /// </summary>
        /// <param name="weapon">Harvesting device</param>
        /// <param name="weaponObjectBuilder">Harvesting device's object builder</param>
        private void AddHarvestingDevice(MyHarvestingDevice harvestingDevice, MyMwcObjectBuilder_SmallShip_Weapon harsvestingDeviceObjectBuilder)
        {
            if (HarvestingDeviceSlot.IsMounted())
            {
                RemoveWeaponFromSlotAndFromShip(HarvestingDeviceSlot);
            }
            AddWeaponToSlot(HarvestingDeviceSlot, harvestingDevice, harsvestingDeviceObjectBuilder);
        }

        /// <summary>
        /// Adds weapon to slot
        /// </summary>
        /// <param name="weaponSlot">Weapon's slot</param>
        /// <param name="weapon">Weapon</param>
        /// <param name="weaponObjectBuilder">Weapon's object builder</param>
        private void AddWeaponToSlot(MyWeaponSlot weaponSlot, MySmallShipGunBase weapon, MyMwcObjectBuilder_SmallShip_Weapon weaponObjectBuilder)
        {
            weaponSlot.InitAndMount(weapon, weaponObjectBuilder, Ship);
        }

        /// <summary>
        /// Removes weapons from slot and from ship child entities
        /// </summary>
        /// <param name="weaponSlot">Weapon's slot</param>
        private void RemoveWeaponFromSlotAndFromShip(MyWeaponSlot weaponSlot)
        {
            weaponSlot.MountedWeapon.MarkForClose();
            weaponSlot.Dismount();
        }

        private void OnWeaponMounting(MyWeaponSlot sender, MySmallShipGunBase weapon)
        {
            m_allMountedWeapons.Add(weapon);
        }

        private void OnWeaponDismounting(MyWeaponSlot sender, MySmallShipGunBase weapon)
        {
            m_allMountedWeapons.Remove(weapon);
        }

        /// <summary>
        /// Fills ammo inventory items from ship inventory
        /// </summary>
        private void FillAmmoInventoryItemsFromInventory()
        {
            m_helperInventoryItems.Clear();
            Ship.Inventory.GetInventoryItems(ref m_helperInventoryItems, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, null);
            AmmoInventoryItems.Init(m_helperInventoryItems);
        }

        /// <summary>
        /// Calls when inventory content changed, and then update ammo inventory items from actual inventory content
        /// </summary>
        /// <param name="sender"></param>
        private void OnInventoryContentChange(MyInventory sender)
        {
            FillAmmoInventoryItemsFromInventory();
        }

        public void StopAllSounds()
        {
            foreach (var weapon in m_allMountedWeapons)
            {
                weapon.StopAllSounds();
            }
        }

        ///// <summary>
        ///// Called when [deserialized].
        ///// </summary>
        //[OnDeserialized]
        //private void OnDeserialized()
        //{
        //    AmmoInventoryItems = new MyAmmoInventoryItems();
        //    Ship.Inventory.OnInventoryContentChange += OnInventoryContentChange;
        //    FillAmmoInventoryItemsFromInventory();
        //}
        #endregion
    }
}
