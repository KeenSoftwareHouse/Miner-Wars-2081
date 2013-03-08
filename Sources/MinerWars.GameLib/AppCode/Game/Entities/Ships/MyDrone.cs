using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Entities
{
    class MyDrone : MySmallShipBot, IMyUseableEntity
    {
        // Inherits from bot because we need AI for drone as well. 
        // Perhaps in the future it would be better to separate AI logic without inheritance and 
        // then drone could inherit from MyShip and only use some AI controller.

        private const int MIN_TIME_TO_ACTIVATE_SHELL = 20;

        private enum MyDroneFunction
        {
            None,
            Explosive,
            EMP,
            NonExplosive,
            //Biochem bomb?,
            //Blaster bomb?,
            Hacking,
        }

        private MyDroneFunction m_function;
        private MyMwcObjectBuilder_Drone m_droneObjectBuilder;

        private MySmallShip m_ownerShip;
        private bool m_fired;

        public MySmallShip OwnerShip
        {
            get { return m_ownerShip; }
            set
            {
                m_ownerShip = value;
                SetFollow();
            }
        }

        public bool HoldPosition { get; set; }

        public MyDrone()
        {
            Inventory.MaxItems = 1;
        }

        public void Init(StringBuilder hudLabelText, MyMwcObjectBuilder_Drone droneBuilder, MyModelsEnum? modelCollision = null, MyModelsEnum? modelLod2 = null, float? scale = null)
        {
            var modelLod0Enum = GetModelLod0Enum(droneBuilder.DroneType);

            MyEntity owner = null;

            base.Init(hudLabelText, modelLod0Enum, null, owner, scale, droneBuilder, modelCollision, modelLod2);

            SetWorldMatrix(droneBuilder.PositionAndOrientation.GetMatrix());

            m_fired = false;

            m_shipTypeProperties = new MyShipTypeProperties();

            m_reflectorProperies = new MyReflectorConfig(this);

            Faction = droneBuilder.Faction;

            Save = true;

            m_function = MyDroneFunction.None;

            InitWeapons();

            InitEntityDetector();

            InitPhysics(droneBuilder.DroneType);

            InitAI();

            InitSpoiledHolograms();

            InitInventory(droneBuilder.Inventory);

            // back camera is turned off by default
            Config.BackCamera.SetOff();

            m_droneObjectBuilder = droneBuilder;

            UseProperties = new MyUseProperties(MyUseType.Solo, MyUseType.None, MyTextsWrapperEnum.NotificationYouCanTake);
            UseProperties.Init(MyUseType.Solo, MyUseType.None, 0, 1, false);

            HackingTool = new Tools.MyHackingTool(this, MySession.PlayerShip.HackingTool.HackingLevel);
        }

        private static MyModelsEnum GetModelLod0Enum(MyMwcObjectBuilder_Drone_TypesEnum droneType)
        {
            MyModelsEnum modelLod0Enum;
            switch (droneType)
            {
                case MyMwcObjectBuilder_Drone_TypesEnum.DroneUS:
                    modelLod0Enum = MyModelsEnum.DroneUS;
                    break;
                case MyMwcObjectBuilder_Drone_TypesEnum.DroneCN:
                    modelLod0Enum = MyModelsEnum.DroneCN;
                    break;
                case MyMwcObjectBuilder_Drone_TypesEnum.DroneSS:
                    modelLod0Enum = MyModelsEnum.DroneSS;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return modelLod0Enum;
        }

        private void InitInventory(MyMwcObjectBuilder_Inventory inventory)
        {
            if (inventory != null)
            {
                Inventory.Init(inventory);
                Inventory_OnInventoryContentChange(Inventory);
            }
            Inventory.MaxItems = 1;
            Inventory.OnInventoryContentChange += Inventory_OnInventoryContentChange;
        }

        public override void Link()
        {
            base.Link();


            if (OwnerEntity != null)
            {
                Debug.Assert(OwnerEntity is MySmallShip);
                OwnerShip = (MySmallShip)OwnerEntity;
                OwnerShip.AddDrone(this);
            }
        }

        private void InitWeapons()
        {
            Weapons = new MySmallShipWeapons(this, m_shipTypeProperties, 0);
        }

        private void InitEntityDetector()
        {
            List<IMyEntityDetectorCriterium> useableEntityCriterias = new List<IMyEntityDetectorCriterium>
            {
                new MyEntityDetectorCriterium<MyPrefabBase>(
                    (int)MySmallShipInteractionActionEnum.Use, 
                    MySmallShipInteraction.CanUse, true, this),
                new MyEntityDetectorCriterium<MyPrefabBase>(
                    (int)MySmallShipInteractionActionEnum.Hack, 
                    MySmallShipInteraction.CanHack, true, this),
            };

            UseableEntityDetector = new MyEntityDetector();
            UseableEntityDetector.Init(null, new MyMwcObjectBuilder_EntityDetector(new Vector3(MySmallShipConstants.DETECT_SHIP_RADIUS * 2f, 0f, 0f), MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere), this, WorldMatrix, useableEntityCriterias);
            UseableEntityDetector.OnNearestEntityChange += OnNearestDetectedEntityChanged;
            UseableEntityDetector.On();
        }

        void Inventory_OnInventoryContentChange(MyInventory sender)
        {
            m_function = GetFunctionFromInventory(this.Inventory);
        }

        private void InitAI()
        {
            Faction = MySession.PlayerShip.Faction;
            m_aiTemplate = MyBotAITemplates.GetTemplate(MyAITemplateEnum.DRONE);
            Aggressivity = 0;
            RouteMemory = new MyPositionMemory(MySmallShipConstants.POSITION_MEMORY_SIZE, 1);

            ActiveAI = true;

            SetFollow();
        }

        private void SetFollow()
        {
            if (OwnerShip != null)
            {
                Follow(OwnerShip);
            }
            else
            {
                Follow(MySession.PlayerShip);
            }
        }

        private void InitPhysics(MyMwcObjectBuilder_Drone_TypesEnum droneType)
        {
            // TODO constants

            switch (droneType)
            {
                case MyMwcObjectBuilder_Drone_TypesEnum.DroneUS:
                    m_shipTypeProperties.Physics = new MyShipTypePhysicsProperties
                    {
                        Mass = 750,
                        MultiplierMovement = 1f,
                        MultiplierForwardBackward = 420,
                        MultiplierStrafe = 420,
                        MultiplierStrafeRotation = 1.2f,
                        MultiplierUpDown = 420,
                        MultiplierRoll = 0.15f,
                        MultiplierRotation = 0.14f,
                        MultiplierRotationEffect = 0.05f,
                        MultiplierRotationDecelerate = 1.85f,
                        MultiplierHorizontalAngleStabilization = 21f,
                        MaxAngularVelocity = 20f,
                    };
                    break;
                case MyMwcObjectBuilder_Drone_TypesEnum.DroneCN:
                    m_shipTypeProperties.Physics = new MyShipTypePhysicsProperties
                    {
                        Mass = 1000,
                        MultiplierMovement = 1f,
                        MultiplierForwardBackward = 420,
                        MultiplierStrafe = 420,
                        MultiplierStrafeRotation = 1.2f,
                        MultiplierUpDown = 420,
                        MultiplierRoll = 0.15f,
                        MultiplierRotation = 0.14f,
                        MultiplierRotationEffect = 0.05f,
                        MultiplierRotationDecelerate = 1.85f,
                        MultiplierHorizontalAngleStabilization = 21f,
                        MaxAngularVelocity = 20f,
                    };
                    break;
                case MyMwcObjectBuilder_Drone_TypesEnum.DroneSS:
                    m_shipTypeProperties.Physics = new MyShipTypePhysicsProperties
                    {
                        Mass = 500,
                        MultiplierMovement = 1f,
                        MultiplierForwardBackward = 420,
                        MultiplierStrafe = 420,
                        MultiplierStrafeRotation = 1.2f,
                        MultiplierUpDown = 420,
                        MultiplierRoll = 0.30f,
                        MultiplierRotation = 0.05f,
                        MultiplierRotationEffect = 0.04f,
                        MultiplierRotationDecelerate = 0.85f,
                        MultiplierHorizontalAngleStabilization = 21f,
                        MaxAngularVelocity = 20f,
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException("droneType");
            }

            m_shipTypeProperties.PhysicsForBot = m_shipTypeProperties.Physics;

            var center = ModelLod0.BoundingBox.GetCenter();
            var size = MyDroneConstants.DRONE_PHYSICS_SIZE_MULTIPLIER * ModelLod0.BoundingBoxSize;
            InitBoxPhysics(MyMaterialType.METAL, center, size, m_shipTypeProperties.Physics.Mass,
                           MyPhysicsConfig.DefaultAngularDamping,
                           MyConstants.COLLISION_LAYER_DEFAULT, RigidBodyFlag.RBF_DEFAULT);

            this.Physics.Type = MyConstants.RIGIDBODY_TYPE_SHIP;
            this.Physics.PlayCollisionCueEnabled = true;
            //this.Physics.MaxLinearVelocity = m_shipTypeProperties.Physics.MaxSpeed;
            this.Physics.LinearDamping = MyPhysicsConfig.DefaultLinearDamping;
            this.Physics.Enabled = true;

            Config.AutoLeveling.SetOn();
        }

        public override void UpdateBeforeSimulation()
        {
            if (IsDead())
            {
                if (!m_fired)
                {
                    Fire();
                }

            }

            base.UpdateBeforeSimulation();
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (MyGuiScreenGamePlay.IsRenderingInsideEntity(this))
            {
                return false;
            }

            return base.Draw(renderObject);
        }

        //public static bool IsRenderingInsideControlledDrone()
        //{
        //    var isMainRenderInsideControlledDrone = MyGuiScreenGamePlay.Static.CameraAttachedTo ==
        //                                            MyCameraAttachedToEnum.Drone &&
        //                                            !MySecondaryCamera.Instance.IsCurrentlyRendering;

        //    var isSecondaryRenderInsideControlledDrone = MySecondaryCamera.Instance.SecondaryCameraAttachedTo ==
        //                                                 MySecondaryCameraAttachedTo.Drone &&
        //                                                 MySecondaryCamera.Instance.IsCurrentlyRendering;

        //    return isMainRenderInsideControlledDrone || isSecondaryRenderInsideControlledDrone;
        //}

        public override void Close()
        {
            Inventory.OnInventoryContentChange -= Inventory_OnInventoryContentChange;
            if (OwnerShip != null)
                OwnerShip.RemoveDrone(this);

            base.Close();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            var objectBuilder = (MyMwcObjectBuilder_Drone)base.GetObjectBuilderInternal(getExactCopy);

            return objectBuilder;
        }

        public void Fire()
        {
            switch (m_function)
            {
                case MyDroneFunction.None:
                case MyDroneFunction.Hacking:
                    // do nothing
                    break;

                case MyDroneFunction.NonExplosive:
                    UseDroneItem();
                    // just closes the drone
                    this.MarkForClose();
                    break;

                case MyDroneFunction.Explosive:
                case MyDroneFunction.EMP:
                    UseDroneItem();
                    // closes it and also makes it explode
                    this.Kill(null);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void UseDroneItem()
        {
            m_fired = true;

            // use universal launcher here:
            var inventoryItem = Inventory.GetInventoryItems()[0];
            var ammoBuilder = (MyMwcObjectBuilder_SmallShip_Ammo) inventoryItem.GetInventoryItemObjectBuilder(false);

            // explode the shell immediately
            Shot(MIN_TIME_TO_ACTIVATE_SHELL, ammoBuilder);
        }

        #region More or less copied from MyUniversalLauncher.cs

        private void Shot(int timeToActivateShell, MyMwcObjectBuilder_SmallShip_Ammo ammoBuilder)
        {
            switch (ammoBuilder.AmmoType)
            {
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart:
                    Shot<MyMineSmart>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic:
                    Shot<MyMineBasic>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem:
                    Shot<MyMineBioChem>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive:
                    Shot<MySphereExplosive>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare:
                    Shot<MyDecoyFlare>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb:
                    Shot<MyFlashBomb>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell:
                    Shot<MyIlluminatingShell>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb:
                    Shot<MySmokeBomb>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer:
                    Shot<MyAsteroidKiller>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive:
                    Shot<MyDirectionalExplosive>(this.GetPosition(), timeToActivateShell);
                    break;

                //case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb:
                //    Shot<MyTimeBomb>(this.GetPosition(), 3000);
                //    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb:
                    Shot<MyRemoteBomb>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram:
                    Shot<MyHologram>(this.GetPosition(), timeToActivateShell);
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb:
                    Shot<MyGravityBomb>(this.GetPosition(), timeToActivateShell);
                    break;

                //case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera:
                //    Shot<MyRemoteCamera>(this.GetPosition(), timeToActivateShell);
                //    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb:
                    Shot<MyEMPBomb>(this.GetPosition(), timeToActivateShell);
                    break;
            }
        }

        void Shot<T>(Vector3 position, int? timeToActivateShell = null)
            where T : class, IUniversalLauncherShell, new()
        {
            T shell = MyUniversalLauncherShells.Allocate<T>(m_ownerShip.EntityId.Value.PlayerId);
            if (shell != null)
            {
                shell.TimeToActivate = timeToActivateShell;
                shell.Start(position, Vector3.Zero, WorldMatrix.Forward, 0, OwnerShip);
            }
        }

        #endregion

        private static MyDroneFunction GetFunctionFromInventory(MyInventory inventory)
        {
            var inventoryItems = inventory.GetInventoryItems();

            if (inventoryItems.Count > 0)
            {
                Debug.Assert(inventoryItems.Count == 1, "Max capacity of drone inventory is 1.");

                var inventoryItem = inventoryItems[0];

                return GetFunctionFromInventoryItem(inventoryItem);
            }

            return MyDroneFunction.None;
        }

        private static MyDroneFunction GetFunctionFromInventoryItem(MyInventoryItem inventoryItem)
        {
            switch (inventoryItem.ObjectBuilderType)
            {
                case MyMwcObjectBuilderTypeEnum.SmallShip_Ammo:
                    Debug.Assert(inventoryItem.ObjectBuilderId != null);
                    var ammoBuilderType = (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)inventoryItem.ObjectBuilderId;
                    return GetFunctionFromAmmoType(ammoBuilderType);

                case MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool:
                    return MyDroneFunction.Hacking;

                default:
                    return MyDroneFunction.None;
            }
        }

        private static MyDroneFunction GetFunctionFromAmmoType(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoBuilderType)
        {
            switch (ammoBuilderType)
            {
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb:
                    return MyDroneFunction.Explosive;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb:
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram:
                    return MyDroneFunction.NonExplosive;

                default:
                    return MyDroneFunction.None;
            }
        }

        #region IMyUseableEntity
        public MyUseProperties UseProperties { get; set; }

        public bool CanBeUsed(MySmallShip usedBy)
        {
            return usedBy == Leader;
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return false;
        }

        public void Use(MySmallShip useBy)
        {
            //m_droneObjectBuilder.DroneType
            // return the drone to inventory
            bool addedSuccess = useBy.Inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.Drone, (int?)m_droneObjectBuilder.DroneType, 1, false) == 0f;
            if (!addedSuccess)
            {
                MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryFullWarning);
            }
            else
            {
                MyAudio.AddCue2D(MySoundCuesEnum.SfxTakeAllUniversal);
                MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryTransfer);
            }
            this.MarkForClose();
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference)
        {
        }
        #endregion
    }
}