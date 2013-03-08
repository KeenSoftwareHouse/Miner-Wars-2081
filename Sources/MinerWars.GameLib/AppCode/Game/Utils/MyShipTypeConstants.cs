using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.Diagnostics;
using MinerWars.AppCode.Game.Models;

namespace MinerWars.AppCode.Game.Utils
{
    internal struct MyShipTypeProperties
    {
        public MyShipTypeGamePlayProperties GamePlay;
        public MyShipTypePhysicsProperties Physics;
        public MyShipTypePhysicsProperties PhysicsForBot;
        public MyShipTypeVisualProperties Visual;
    }

    internal struct MyShipTypeGamePlayProperties
    {
        /// <summary>
        /// Number of weapons slots. These include universal launcher slots.
        /// </summary>
        public int MaxWeapons;

        public int MaxDevices;
        // Supports harvesting tube?
        public bool HarvestingSupported;
        // Supports drill?
        public bool DrillSupported;
        // Number of cargo slots. From 1x1x to 8x8x8 (biggest ships). Dimension of any item is defined in slots – for simplicity.
        public int CargoCapacity;
        // Fuel capacity in liters (xenon, helium, uranium)
        public float FuelCapacity;
        // In kWh
        public float EletricityCapacity;
        // In seconds
        public float OxygenCapacity;

        public float ExtraFuelCapacity;
        public float ExtraEletricityCapacity;
        public float ExtraOxygenCapacity;

        public float ElectricityConsuption;
        public float LightsElectricityConsuption;

        // Defines duration of afterburner usage 
        public float AfterBurnerDurationTime;
        // Defines time to refill afterburner
        public float AfterBurnerRefillTime;

        public int MaxLeftWeapons { get { return (MaxWeapons - 2) / 2; } }
        public int MaxRightWeapons { get { return (MaxWeapons - 2) / 2; } }
    }

    internal struct MyShipTypePhysicsProperties
    {
        // Mass of bot ship. Bot default: 20000.
        public float Mass;
        // Movement force adjustor. Bot default: 120.0f
        public float MultiplierHorizontalAngleStabilization;
        // Movement force adjustor. Bot default: 195
        public float MultiplierForwardBackward;
        // Movement force adjustor. Bot default: 180
        public float MultiplierStrafe;
        // Movement force adjustor. Bot default: 13
        public float MultiplierStrafeRotation;
        // Movement force adjustor. Bot default: 180
        public float MultiplierUpDown;
        public float MultiplierRoll;
        // Movement force adjustor. Bot default: 3
        public float MultiplierRotation;
        // Movement force adjustor. Bot default: 5.0f
        public float MultiplierRotationEffect;
        // Movement force adjustor. Bot default: 40
        public float MultiplierRotationDecelerate;
        // Movement force adjustor. Bot default: 3
        public float MultiplierMovement;
        // Max angular velocity multiplier.
        public float MaxAngularVelocity;
    }

    internal struct MyShipTypeVisualProperties
    {
        public MyModelsEnum ModelLod0Enum;
        public MyModelsEnum? ModelLod1Enum;
        public MyModelsEnum CockpitGlassModel;
        public MyModelsEnum CockpitInteriorModel;
        public MyMaterialType MaterialType;
    }
    
    static class MyShipTypeConstants
    {
        static Dictionary<int, MyShipTypeProperties> ShipTypeProperties = new Dictionary<int, MyShipTypeProperties>();

        static MyShipTypeConstants()
        {
            var gamePlay = new MyShipTypeGamePlayProperties
            {
                //CargoCapacity = 8 * 8 * 8,
                CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY,
                DrillSupported = true,
                HarvestingSupported = true,
                MaxDevices = 16,
                MaxWeapons = 8,

                FuelCapacity = 60,
                ExtraFuelCapacity = 60,
                EletricityCapacity = 5000,
                ExtraEletricityCapacity = 5000,
                OxygenCapacity = 1800,
                ExtraOxygenCapacity = 600,
                ElectricityConsuption = 1,
                LightsElectricityConsuption = 2,
                AfterBurnerDurationTime = 4,
                AfterBurnerRefillTime = 6,
            };

            var shipPhysics = new MyShipTypePhysicsProperties
            {
                Mass = 6600,
                MultiplierMovement = 1f,
                MultiplierForwardBackward = 420,
                MultiplierStrafe = 420,
                MultiplierStrafeRotation = 12,
                MultiplierUpDown = 420,
                MultiplierRoll = 1.8f,
                MultiplierRotation = 10f,
                MultiplierRotationEffect = 0.68f,
                MultiplierRotationDecelerate = 42f,
                MultiplierHorizontalAngleStabilization = 42f,
                MaxAngularVelocity = 20f,
            };

            var defaultPhysicsForBot = new MyShipTypePhysicsProperties
            {
                Mass = 4000,
                MultiplierMovement = 1f,
                MultiplierForwardBackward = 420,
                MultiplierStrafe = 420,
                MultiplierStrafeRotation = 12,
                MultiplierUpDown = 420,
                MultiplierRoll = 0.79f,
                MultiplierRotation = 2.0f,
                MultiplierRotationEffect = 0.68f,
                MultiplierRotationDecelerate = 18.5f,
                MultiplierHorizontalAngleStabilization = 42f,
                MaxAngularVelocity = 20f,
            };

            const float massMultiplier = 4;

            shipPhysics.Mass = massMultiplier * 7000;
            gamePlay.CargoCapacity = MySmallShipConstants.LARGE_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 8;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Liberator,
                    ModelLod1Enum = MyModelsEnum.Liberator_LOD1,
                    CockpitGlassModel = MyModelsEnum.EAC05_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.EAC05_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 7500;
            gamePlay.CargoCapacity = MySmallShipConstants.LARGE_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 8;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Enforcer,
                    ModelLod1Enum = MyModelsEnum.Enforcer_LOD1,
                    CockpitGlassModel = MyModelsEnum.OmniCorp04_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.OmniCorp04_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 8000;
            gamePlay.CargoCapacity = MySmallShipConstants.LARGE_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 8;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Kammler,
                    ModelLod1Enum = MyModelsEnum.Kammler_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_SS_04_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_SS_04,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 6600;
            gamePlay.CargoCapacity = MySmallShipConstants.LARGE_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 8;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Gettysburg,
                    ModelLod1Enum = MyModelsEnum.Gettysburg_LOD1,
                    CockpitGlassModel = MyModelsEnum.EAC02_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.EAC02_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 5000;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 6;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Virginia,
                    ModelLod1Enum = MyModelsEnum.Virginia_LOD1,
                    CockpitGlassModel = MyModelsEnum.EAC02_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.EAC02_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 5500;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 6;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.BAER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Baer,
                    ModelLod1Enum = MyModelsEnum.Baer_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_SS_04_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_SS_04,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 4500;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 4;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Hewer,
                    ModelLod1Enum = MyModelsEnum.Hewer_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_SS_04_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_SS_04,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 4800;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 6;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Razorclaw,
                    ModelLod1Enum = MyModelsEnum.Razorclaw_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_Razorclaw_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_Razorclaw,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 6200;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 8;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Greiser,
                    ModelLod1Enum = MyModelsEnum.Greiser_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_SS_04_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_SS_04,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 5900;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 8;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.Tracer,
                    ModelLod1Enum = MyModelsEnum.Tracer_LOD1,
                    CockpitGlassModel = MyModelsEnum.EAC03_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.EAC03_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 6100;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 8;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Jacknife,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Jacknife_LOD1,
                    CockpitGlassModel = MyModelsEnum.OmniCorp01_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.OmniCorp01_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 5200;
            shipPhysics.MultiplierRotationDecelerate = 25;
            shipPhysics.MultiplierRotation = 6;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 6;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.DOON, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Doon,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Doon_LOD1,
                    CockpitGlassModel = MyModelsEnum.MinerShip_Generic_CockpitGlass,
                    CockpitInteriorModel = MyModelsEnum.MinerShip_Generic_CockpitInterior,
                    MaterialType = MyMaterialType.SHIP
                }
            });
            shipPhysics.MultiplierRotationDecelerate = 42;
            shipPhysics.MultiplierRotation = 10;

            shipPhysics.Mass = massMultiplier * 7500;
            gamePlay.CargoCapacity = MySmallShipConstants.LARGE_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 10;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Hammer,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Hammer_LOD1,
                    CockpitGlassModel = MyModelsEnum.EAC04_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.EAC04_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 9000;
            gamePlay.CargoCapacity = MySmallShipConstants.LARGE_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 12;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.ORG, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_ORG,
                    ModelLod1Enum = MyModelsEnum.SmallShip_ORG_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_CN_03_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_CN_03,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 7500;
            gamePlay.CargoCapacity = MySmallShipConstants.SMALL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 10;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.YG, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_YG_Closed,
                    ModelLod1Enum = MyModelsEnum.SmallShip_YG_Closed_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_CN_03_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_CN_03,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 6600;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 10;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Hawk,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Hawk_LOD1,
                    CockpitGlassModel = MyModelsEnum.OmniCorp01_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.OmniCorp01_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 6300;
            gamePlay.CargoCapacity = MySmallShipConstants.SMALL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 10;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Phoenix,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Phoenix_LOD1,
                    CockpitGlassModel = MyModelsEnum.OmniCorp03_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.OmniCorp03_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 8500;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 10;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Leviathan,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Leviathan_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_CN_03_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_CN_03,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 5500;
            gamePlay.CargoCapacity = MySmallShipConstants.SMALL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 6;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Rockheater,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Rockheater_LOD1,
                    CockpitGlassModel = MyModelsEnum.OmniCorp_EAC01_Cockpit_glass,
                    CockpitInteriorModel = MyModelsEnum.OmniCorp_EAC01_Cockpit,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 6000;
            gamePlay.CargoCapacity = MySmallShipConstants.LARGE_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 8;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_SteelHead,
                    ModelLod1Enum = MyModelsEnum.SmallShip_SteelHead_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_CN_03_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_CN_03,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 11000;
            gamePlay.CargoCapacity = MySmallShipConstants.NORMAL_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 12;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Talon,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Talon_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_SS_04_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_SS_04,
                    MaterialType = MyMaterialType.SHIP
                }
            });

            shipPhysics.Mass = massMultiplier * 10500;
            gamePlay.CargoCapacity = MySmallShipConstants.LARGE_CARGO_CAPACITY;
            gamePlay.MaxWeapons = 12;
            ShipTypeProperties.Add((int)MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV, new MyShipTypeProperties
            {
                GamePlay = gamePlay,
                Physics = shipPhysics,
                PhysicsForBot = defaultPhysicsForBot,
                Visual = new MyShipTypeVisualProperties
                {
                    ModelLod0Enum = MyModelsEnum.SmallShip_Stanislav,
                    ModelLod1Enum = MyModelsEnum.SmallShip_Stanislav_LOD1,
                    CockpitGlassModel = MyModelsEnum.Cockpit_CN_03_glass,
                    CockpitInteriorModel = MyModelsEnum.Cockpit_CN_03,
                    MaterialType = MyMaterialType.SHIP
                }
            });
        }

        public static MyShipTypeProperties GetShipTypeProperties(MyMwcObjectBuilder_SmallShip_TypesEnum shipType)
        {
            Debug.Assert(ShipTypeProperties.ContainsKey((int)shipType));
            return ShipTypeProperties[(int)shipType];
        }
    }
}
