using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.Utils
{
    class MySmallShipEngineTypeProperties
    {
        public float Force;
        public float FuelConsumption;
        public float MaxSpeed;
        public float AfterburnerSpeedMultiplier;
        public float AfterburnerConsumptionMultiplier;
        public MyMwcObjectBuilder_Ore_TypesEnum? FuelType;
        public Vector4 ThrustsColor;
        public MySoundCuesEnum IdleCue2d;
        public MySoundCuesEnum IdleCue3d;
        public MySoundCuesEnum HighCue2d;
        public MySoundCuesEnum HighCue3d;
        public MySoundCuesEnum OnCue;        
        public MySoundCuesEnum OffCue;
        public MySoundCuesEnum Thrust2d;
        public MySoundCuesEnum Thrust3d;
    }

    static class MySmallShipEngineTypeConstants
    {
        public static MySmallShipEngineTypeProperties ShipWithoutEngineProperties;

        static Dictionary<int, MySmallShipEngineTypeProperties> EngineProperties = new Dictionary<int, MySmallShipEngineTypeProperties>();

        static MySmallShipEngineTypeConstants()
        {
            Vector4 chemicalThrustsColor = new Vector4(0.44f, 0.96f, 0, 1.0f);
            Vector4 nuclearThrustsColor = new Vector4(0, 0.44f, 0.96f, 1.0f);
            Vector4 electricThrustsColor = new Vector4(0.75f, 0.75f, 0.75f, 1.0f);
            Vector4 withoutEngineThrustsColor = new Vector4(0.96f, 0.96f, 0f, 1.0f);

            ShipWithoutEngineProperties = new MySmallShipEngineTypeProperties
            {
                Force = 100,
                FuelConsumption = 0,
                MaxSpeed = 50,
                AfterburnerSpeedMultiplier = 1.0f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = null,
                ThrustsColor = withoutEngineThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehShipaEngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehShipaEngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehShipaEngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehShipaEngineHigh3d,
                OnCue = MySoundCuesEnum.VehShipaEngineOn,                                
                OffCue = MySoundCuesEnum.VehShipaEngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            };

            // modifies all engine fuel consumption
            float fuelConsumptionModifier = 0.75f;

            // modifies all engines force
            float forceModifier = 3;
            
            // Chemical Engines
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1, new MySmallShipEngineTypeProperties
            {
                Force = 900 * forceModifier,
                FuelConsumption = 0.1f * fuelConsumptionModifier,
                MaxSpeed = 100,
                AfterburnerSpeedMultiplier = 1.4f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.HELIUM,
                ThrustsColor = chemicalThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehCH1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehCH1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehCH1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehCH1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehCH1EngineOn,
                OffCue = MySoundCuesEnum.VehCH1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_2, new MySmallShipEngineTypeProperties
            {
                Force = 1000 * forceModifier,
                FuelConsumption = 0.15f * fuelConsumptionModifier,
                MaxSpeed = 105,
                AfterburnerSpeedMultiplier = 1.5f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.HELIUM,
                ThrustsColor = chemicalThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehCH1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehCH1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehCH1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehCH1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehCH1EngineOn,
                OffCue = MySoundCuesEnum.VehCH1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_3, new MySmallShipEngineTypeProperties
            {
                Force = 1100 * forceModifier,
                FuelConsumption = 0.2f * fuelConsumptionModifier,
                MaxSpeed = 110,
                AfterburnerSpeedMultiplier = 1.6f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.HELIUM,
                ThrustsColor = chemicalThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehCH1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehCH1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehCH1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehCH1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehCH1EngineOn,
                OffCue = MySoundCuesEnum.VehCH1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_4, new MySmallShipEngineTypeProperties
            {
                Force = 1200 * forceModifier,
                FuelConsumption = 0.25f * fuelConsumptionModifier,
                MaxSpeed = 115,
                AfterburnerSpeedMultiplier = 1.7f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.HELIUM,
                ThrustsColor = chemicalThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehCH1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehCH1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehCH1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehCH1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehCH1EngineOn,
                OffCue = MySoundCuesEnum.VehCH1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_5, new MySmallShipEngineTypeProperties
            {
                Force = 1300 * forceModifier,
                FuelConsumption = 0.3f * fuelConsumptionModifier,
                MaxSpeed = 120,
                AfterburnerSpeedMultiplier = 1.8f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.HELIUM,
                ThrustsColor = chemicalThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehCH1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehCH1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehCH1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehCH1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehCH1EngineOn,
                OffCue = MySoundCuesEnum.VehCH1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });

            // Nuclear Engines
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_1, new MySmallShipEngineTypeProperties
            {
                Force = 1100 * forceModifier,
                FuelConsumption = 0.1f * fuelConsumptionModifier,
                MaxSpeed = 140,
                AfterburnerSpeedMultiplier = 2.1f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.URANITE,
                ThrustsColor = nuclearThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehNU1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehNU1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehNU1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehNU1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehNU1EngineOn,
                OffCue = MySoundCuesEnum.VehNU1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_2, new MySmallShipEngineTypeProperties
            {
                Force = 1200 * forceModifier,
                FuelConsumption = 0.2f * fuelConsumptionModifier,
                MaxSpeed = 145,
                AfterburnerSpeedMultiplier = 2.2f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.URANITE,
                ThrustsColor = nuclearThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehNU1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehNU1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehNU1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehNU1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehNU1EngineOn,
                OffCue = MySoundCuesEnum.VehNU1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_3, new MySmallShipEngineTypeProperties
            {
                Force = 1300 * forceModifier,
                FuelConsumption = 0.3f * fuelConsumptionModifier,
                MaxSpeed = 150,
                AfterburnerSpeedMultiplier = 2.3f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.URANITE,
                ThrustsColor = nuclearThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehNU1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehNU1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehNU1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehNU1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehNU1EngineOn,
                OffCue = MySoundCuesEnum.VehNU1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_4, new MySmallShipEngineTypeProperties
            {
                Force = 1400 * forceModifier,
                FuelConsumption = 0.35f * fuelConsumptionModifier,
                MaxSpeed = 155,
                AfterburnerSpeedMultiplier = 2.4f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.URANITE,
                ThrustsColor = nuclearThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehNU1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehNU1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehNU1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehNU1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehNU1EngineOn,
                OffCue = MySoundCuesEnum.VehNU1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_5, new MySmallShipEngineTypeProperties
            {
                Force = 1500 * forceModifier,
                FuelConsumption = 0.4f * fuelConsumptionModifier,
                MaxSpeed = 160,
                AfterburnerSpeedMultiplier = 2.5f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.URANITE,
                ThrustsColor = nuclearThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehNU1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehNU1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehNU1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehNU1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehNU1EngineOn,
                OffCue = MySoundCuesEnum.VehNU1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });

            // Electricity Engines
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1, new MySmallShipEngineTypeProperties
            {
                Force = 1000 * forceModifier,
                FuelConsumption = 0.15f * fuelConsumptionModifier,
                MaxSpeed = 130,
                AfterburnerSpeedMultiplier = 1.7f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.ICE,
                ThrustsColor = electricThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehEL1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehEL1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehEL1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehEL1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehEL1EngineOn,
                OffCue = MySoundCuesEnum.VehEL1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_2, new MySmallShipEngineTypeProperties
            {
                Force = 1075 * forceModifier,
                FuelConsumption = 0.25f * fuelConsumptionModifier,
                MaxSpeed = 135,
                AfterburnerSpeedMultiplier = 1.8f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.ICE,
                ThrustsColor = electricThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehEL1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehEL1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehEL1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehEL1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehEL1EngineOn,
                OffCue = MySoundCuesEnum.VehEL1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_3, new MySmallShipEngineTypeProperties
            {
                Force = 1150 * forceModifier,
                FuelConsumption = 0.35f * fuelConsumptionModifier,
                MaxSpeed = 140,
                AfterburnerSpeedMultiplier = 1.9f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.ICE,
                ThrustsColor = electricThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehEL1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehEL1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehEL1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehEL1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehEL1EngineOn,
                OffCue = MySoundCuesEnum.VehEL1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_4, new MySmallShipEngineTypeProperties
            {
                Force = 1225 * forceModifier,
                FuelConsumption = 0.45f * fuelConsumptionModifier,
                MaxSpeed = 145,
                AfterburnerSpeedMultiplier = 2.0f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.ICE,
                ThrustsColor = electricThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehEL1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehEL1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehEL1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehEL1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehEL1EngineOn,
                OffCue = MySoundCuesEnum.VehEL1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
            EngineProperties.Add((int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_5, new MySmallShipEngineTypeProperties
            {
                Force = 1300 * forceModifier,
                FuelConsumption = 0.55f * fuelConsumptionModifier,
                MaxSpeed = 150,
                AfterburnerSpeedMultiplier = 2.1f,
                AfterburnerConsumptionMultiplier = 2.0f,
                FuelType = MyMwcObjectBuilder_Ore_TypesEnum.ICE,
                ThrustsColor = electricThrustsColor,
                IdleCue2d = MySoundCuesEnum.VehEL1EngineIdle2d,
                IdleCue3d = MySoundCuesEnum.VehEL1EngineIdle3d,
                HighCue2d = MySoundCuesEnum.VehEL1EngineHigh2d,
                HighCue3d = MySoundCuesEnum.VehEL1EngineHigh3d,
                OnCue = MySoundCuesEnum.VehEL1EngineOn,
                OffCue = MySoundCuesEnum.VehEL1EngineOff,
                Thrust2d = MySoundCuesEnum.VehShipaThrust2d,
                Thrust3d = MySoundCuesEnum.VehShipaThrust3d
            });
        }

        public static MySmallShipEngineTypeProperties GetProperties(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum engineType)
        {
            return EngineProperties[(int)engineType];
        }
    }
}
