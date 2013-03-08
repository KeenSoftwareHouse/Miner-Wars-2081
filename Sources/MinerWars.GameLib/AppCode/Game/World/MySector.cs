using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWarsMath;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.AppCode.Game.World
{
    /// <summary>
    /// Current sector
    /// </summary>
    class MySector
    {
        static MySector()
        {
            // Set defaults - that's for example for menu animations, trailers etc...
            SetDefaults();
        }

        public static MySolarSystemAreaEnum? Area;

        public static MySunProperties SunProperties;
        public static MyFogProperties FogProperties;
        public static MyDebrisProperties DebrisProperties;
        public static MyParticleDustProperties ParticleDustProperties;
        public static MyGodRaysProperties GodRaysProperties;
        public static MyImpostorProperties[] ImpostorProperties;
        public static string BackgroundTexture;
        public static bool UseGenerator = false;
        public static List<int> PrimaryMaterials;
        public static List<int> SecondaryMaterials;
        public static List<int> AllowedMaterials;

        public static float FogMultiplierForSun
        {
            get
            {
                const float maxSunFog = 0.7f; // 70% of fog applied to sun
                return FogProperties.FogMultiplier * maxSunFog;
            }
        }

        public static Vector3 SunColorWithFog
        {
            get
            {
                return Vector3.Lerp(SunProperties.SunDiffuse, FogProperties.FogColor, FogMultiplierForSun);
            }
        }

        public static void SetDefaults()
        {
            MySolarSystemArea defaults = MySolarSystemConstants.GetDefaultArea();
            SunProperties = defaults.SectorData.SunProperties;
            FogProperties = defaults.SectorData.FogProperties;
            DebrisProperties = defaults.SectorData.DebrisProperties;
            ImpostorProperties = defaults.SectorData.ImpostorProperties;
            ParticleDustProperties = defaults.SectorData.ParticleDustProperties;
            GodRaysProperties = defaults.SectorData.GodRaysProperties;
            BackgroundTexture = defaults.SectorData.BackgroundTexture;
        }
    }
}
