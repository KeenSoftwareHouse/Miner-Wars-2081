using System.Collections.Generic;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.TransparentGeometry
{
    //  This enums must have same name as source texture files used to create texture atlas
    //  And only ".tga" files are supported.
    //  IMPORTANT: If you change order or names in this enum, update it also in MyEnumsToStrings
    public enum MyTransparentGeometryTexturesEnum : byte
    {
        Explosion = 0,
        ExplosionSmokeDebrisLine,
        Smoke,
        Test,
        EngineThrustMiddle,
        ReflectorCone,
        ReflectorGlareAdditive,
        ReflectorGlareAlphaBlended,
        MuzzleFlashMachineGunFront,
        MuzzleFlashMachineGunSide,
        ProjectileTrailLine,
        ContainerBorder,
        Dust,
        Crosshair,
        Sun,
        LightRay,
        LightGlare,

        SolarMapOrbitLine,
        SolarMapSun,
        SolarMapAsteroidField,
        SolarMapFactionMap,
        SolarMapAsteroid,
        SolarMapZeroPlaneLine,
        SolarMapSmallShip,
        SolarMapLargeShip,
        SolarMapOutpost,

        Grid,
        ContainerBorderSelected,

        FactionRussia,
        FactionChina,
        FactionJapan,
        FactionUnitedKorea,
        FactionFreeAsia,
        FactionSaudi,
        FactionEAC,
        FactionCSR,
        FactionIndia,
        FactionChurch,
        FactionOmnicorp,
        FactionFourthReich,
        FactionSlavers,

        Smoke_b,
        Smoke_c,
        Sparks_a,
        Sparks_b,
        particle_stone,
        Stardust,
        particle_trash_a,
        particle_trash_b,
        particle_glare,
        smoke_field,
        Explosion_pieces,
        particle_laser,
        particle_nuclear,
        Explosion_line,
        particle_flash_a,
        particle_flash_b,
        particle_flash_c,
        snap_point,

        SolarMapNavigationMark,

        Impostor_StaticAsteroid20m_A,
        Impostor_StaticAsteroid20m_C,
        Impostor_StaticAsteroid50m_D,
        Impostor_StaticAsteroid50m_E,

        GPS,
        GPSBack,

        ShotgunParticle,

        ObjectiveDummyFace,
        ObjectiveDummyLine,

        SunDisk,

        scanner_01,
        Smoke_square,
        Smoke_lit,

        SolarMapSideMission,
        SolarMapStoryMission,
        SolarMapTemplateMission,
        SolarMapPlayer,
    }

    public class MyTransparentMaterialProperties
    {
        public MyTransparentMaterialProperties 
            (
                MyTransparentGeometryTexturesEnum Texture,
                float SoftParticleDistanceScale,
                bool CanBeAffectedByOtherLights,
                bool AlphaMistingEnable,
                bool IgnoreDepth = false,
                bool NeedSort = true,
                bool UseAtlas = false,
                float Emissivity = 0,
                float AlphaMistingStart = 1,
                float AlphaMistingEnd = 4,
                float AlphaSaturation = 1
            ) 
        {
            this.Texture = Texture;
            this.SoftParticleDistanceScale = SoftParticleDistanceScale;
            this.CanBeAffectedByOtherLights = CanBeAffectedByOtherLights;
            this.AlphaMistingEnable = AlphaMistingEnable;
            this.IgnoreDepth = IgnoreDepth;
            this.NeedSort = NeedSort;
            this.UseAtlas = UseAtlas;
            this.Emissivity = Emissivity;
            this.AlphaMistingStart = AlphaMistingStart;
            this.AlphaMistingEnd = AlphaMistingEnd;
            this.AlphaSaturation = AlphaSaturation;

            UVOffset = new Vector2(0, 0);
            UVSize = new Vector2(1, 1);
        }

        public MyTransparentGeometryTexturesEnum Texture { get; private set; }

        //  If true, then we calculate sun shadow value for a particle, and also per-pixel lighting. Set it to true only if really unneceserary as it
        //  will slow down the rendering.
        public bool CanBeAffectedByOtherLights;

        public bool AlphaMistingEnable;
        public bool IgnoreDepth;
        public bool NeedSort;
        public bool UseAtlas;

        public float AlphaMistingStart;
        public float AlphaMistingEnd;

        public Vector2 UVOffset;
        public Vector2 UVSize;

        public float SoftParticleDistanceScale;

        public float Emissivity;

        public float AlphaSaturation;
    }

    public enum MyTransparentMaterialEnum
    {
        Explosion = 0,
        ExplosionSmokeDebrisLine = 1,
        Smoke = 2,
        CockpitSmoke = 3,
        GunSmoke = 4,
        Test = 5,
        EngineThrustMiddle = 6,
        ReflectorCone = 7,
        ReflectorGlareAdditive = 8,
        ReflectorGlareAlphaBlended = 9,
        IlluminatingShell = 10,
        MuzzleFlashMachineGunFront = 11,
        MuzzleFlashMachineGunSide = 12,
        ProjectileTrailLine = 13,
        DebrisTrailLine = 14,
        ContainerBorder = 15,
        Dust = 16,
        Crosshair = 17,
        Sun = 18,
        LightRay = 19,
        LightGlare = 20,

        // This is objects for solar map, all must ignore depth
        SolarMapOrbitLine = 21,
        SolarMapSun = 22,
        SolarMapAsteroidField = 23,
        SolarMapFactionMap = 24,
        SolarMapAsteroid = 25,
        SolarMapZeroPlaneLine = 26,
        SolarMapSmallShip = 27,
        SolarMapLargeShip = 28,
        SolarMapOutpost = 29,

        Grid = 30,
        ContainerBorderSelected = 31,

        // Factions
        FactionRussia = 32,
        FactionChina = 33,
        FactionJapan = 34,
        FactionUnitedKorea = 35,
        FactionFreeAsia = 36,
        FactionSaudi = 37,
        FactionEAC = 38,
        FactionCSR = 39,
        FactionIndia = 40,
        FactionChurch = 41,
        FactionOmnicorp = 42,
        FactionFourthReich = 43,


        Smoke_b = 44,
        Smoke_c = 45,
        Sparks_a = 46,
        Sparks_b = 47,
        particle_stone = 48,
        Stardust = 49,
        particle_trash_a = 50,
        particle_trash_b = 51,
        particle_glare = 52,
        smoke_field = 53,
        Explosion_pieces = 54,
        particle_laser = 55,
        particle_nuclear = 56,
        Explosion_line = 57,
        particle_flash_a = 58,
        particle_flash_b = 59,
        particle_flash_c = 60,
        snap_point = 61,

        MissileGlare = 62,
        SolarMapNavigationMark = 63,
        DecalGlare = 64,

        Impostor_StaticAsteroid20m_A = 65,
        Impostor_StaticAsteroid20m_C = 66,
        Impostor_StaticAsteroid50m_D = 67,
        Impostor_StaticAsteroid50m_E = 68,

        GPS = 69,
        GPSBack = 70,

        ShotgunParticle = 71,
        snap_point_depth = 72,

        ObjectiveDummyFace = 73,
        ObjectiveDummyLine = 74,

        FactionSlavers = 75,
        SunDisk = 76,

        VolumetricGlare = 77,

        SolarMapDust = 78,
        scanner_01 = 79,
        Smoke_square = 80,
        Smoke_lit = 81,
        Smoke_square_unlit = 83,

        SolarMapTemplateMission = 86,
        SolarMapSideMission = 87,
        SolarMapStoryMission = 88,
        SolarMapPlayer = 89,

        Smoke_Ignore_Depth = 90,

        LightGlare_WithDepth = 91,
    }    

    public static class MyTransparentMaterialConstants
    {
        static Dictionary<int, MyTransparentMaterialProperties> MaterialProperties = new Dictionary<int, MyTransparentMaterialProperties>();

        public static string[] MyTransparentMaterialStrings =
        {
            "Explosion",
            "ExplosionSmokeDebrisLine",
            "Smoke",
            "CockpitSmoke",
            "GunSmoke",
            "Test",
            "EngineThrustMiddle",
            "ReflectorCone",
            "ReflectorGlareAdditive",
            "ReflectorGlareAlphaBlended",
            "IlluminatingShell",
            "MuzzleFlashMachineGunFront",
            "MuzzleFlashMachineGunSide",
            "ProjectileTrailLine",
            "DebrisTrailLine",
            "ContainerBorder",
            "Dust",
            "Crosshair",
            "Sun",
            "LightRay",
            "LightGlare",

            "SolarMapOrbitLine",
            "SolarMapSun",
            "SolarMapAsteroidField",
            "SolarMapFactionMap",
            "SolarMapAsteroid",
            "SolarMapZeroPlaneLine",
            "SolarMapSmallShip",
            "SolarMapLargeShip",
            "SolarMapOutpost",

            "Grid",
            "ContainerBorderSelected",

            "FactionRussia",
            "FactionChina",
            "FactionJapan",
            "FactionUnitedKorea",
            "FactionFreeAsia",
            "FactionSaudi",
            "FactionEAC",
            "FactionCSR",
            "FactionIndia",
            "FactionChurch",
            "FactionOmnicorp",
            "FactionFourthReich",
            "Smoke_b",
            "Smoke_c",
            "Sparks_a",
            "Sparks_b",
            "particle_stone",
            "Stardust",
            "particle_trash_a",
            "particle_trash_b",
            "particle_glare",
            "smoke_field",
            "Explosion_pieces",
            "particle_laser",
            "particle_nuclear",
            "Explosion_line",
            "particle_flash_a",
            "particle_flash_b",
            "particle_flash_c",
            "snap_point",

            "MissileGlare",

            "SolarMapNavigationMark",

            "DecalGlare",

            "Impostor_StaticAsteroid20m_A",
            "Impostor_StaticAsteroid20m_C",
            "Impostor_StaticAsteroid50m_D",
            "Impostor_StaticAsteroid50m_E",

            "GPS",
            "GPSBack",

            "ShotgunParticle",
            "snap_point_depth",

            "ObjectiveDummyFace",
            "ObjectiveDummyLine",
            
            "FactionSlavers",

            "SunDisk",

            "VolumetricGlare",

            "SolarMapDust",

            "scanner_01",
            "Smoke_square",
            "Smoke_lit",
            "Smoke_square_unlit",

            "SolarMapTemplateMission",
            "SolarMapSideMission",
            "SolarMapStoryMission",
            "SolarMapPlayer",

            "Smoke_Ignore_Depth",
            "LightGlare_WithDepth",
        };

        static MyTransparentMaterialConstants()
        {
            System.Diagnostics.Debug.Assert(System.Enum.GetValues(typeof(MyTransparentMaterialEnum)).Length == MyTransparentMaterialStrings.Length);

            //Explosion material
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Explosion, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Explosion,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                IgnoreDepth: false,
                UseAtlas: true
           ));

            //Explosion debris line material
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ExplosionSmokeDebrisLine, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ExplosionSmokeDebrisLine,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));

            //Smoke
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Smoke, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Smoke,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable:  true,
                SoftParticleDistanceScale:  MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));

            //Cockpit smoke
            MaterialProperties.Add((int)MyTransparentMaterialEnum.CockpitSmoke, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Smoke,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: true
            ));

            //Gun smoke
            MaterialProperties.Add((int)MyTransparentMaterialEnum.GunSmoke, new MyTransparentMaterialProperties(
                Texture:  MyTransparentGeometryTexturesEnum.Smoke,
                CanBeAffectedByOtherLights:  true,
                AlphaMistingEnable:  true,
                SoftParticleDistanceScale:  MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));

            //Test
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Test, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Test,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Engine thrust middle
            MaterialProperties.Add((int)MyTransparentMaterialEnum.EngineThrustMiddle, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.EngineThrustMiddle,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                NeedSort: false
            ));

            //Reflector Cone
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ReflectorCone, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ReflectorCone,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: 0.1f // Special value for reflector cone to look good and quite soft
            ));

            //Reflector Glare Additive
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ReflectorGlareAdditive, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ReflectorGlareAdditive,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                Emissivity: 0.15f,
                SoftParticleDistanceScale: 0.5f, // Special value for this 'circle' billboard
                UseAtlas: true
            ));

            //Reflector Glare Blended
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ReflectorGlareAlphaBlended,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: true
            ));

            //Illuminating shell
            MaterialProperties.Add((int)MyTransparentMaterialEnum.IlluminatingShell, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ReflectorGlareAlphaBlended,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));

            //Muzzle Flash Machine Gun Front
            MaterialProperties.Add((int)MyTransparentMaterialEnum.MuzzleFlashMachineGunFront, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.MuzzleFlashMachineGunFront,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                NeedSort: false,
                UseAtlas: true
            ));

            //Muzzle Flash Machine Gun Side
            MaterialProperties.Add((int)MyTransparentMaterialEnum.MuzzleFlashMachineGunSide, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.MuzzleFlashMachineGunSide,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Projectile Trail Line
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ProjectileTrailLine, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ProjectileTrailLine,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                NeedSort: false
            ));

            //Debris Trail Line
            MaterialProperties.Add((int)MyTransparentMaterialEnum.DebrisTrailLine, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ProjectileTrailLine,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Container Border
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ContainerBorder, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ContainerBorder,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Dust
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Dust, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Dust,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true,
                Emissivity: 0,
                NeedSort: false
            ));

            //Crosshair (Third person view)
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Crosshair, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Crosshair,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                NeedSort: false                
            ));

            //Sun Glare
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Sun, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Sun,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Light Ray
            MaterialProperties.Add((int)MyTransparentMaterialEnum.LightRay, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.LightRay,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false //problems with DXT1
            ));

            //Light Glare
            MaterialProperties.Add((int)MyTransparentMaterialEnum.LightGlare, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.LightGlare,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false, //problems with DXT1
                AlphaSaturation: 0.85f
            ));

            //Light Glare With depth (distant glares can hide just by depth
            MaterialProperties.Add((int)MyTransparentMaterialEnum.LightGlare_WithDepth, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.LightGlare,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false, //problems with DXT1
                AlphaSaturation: 0.85f
            ));

            //MissileGlare
            MaterialProperties.Add((int)MyTransparentMaterialEnum.MissileGlare, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.LightGlare,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false //problems with DXT1
            ));

            //Solar map orbit line
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapOrbitLine, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapOrbitLine,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map sun
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapSun, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapSun,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map asteroid field
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapAsteroidField, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapAsteroidField,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map faction area
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapFactionMap, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapFactionMap,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map asteroid
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapAsteroid, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapAsteroid,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map zero plane line
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapZeroPlaneLine, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapZeroPlaneLine,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar small ship
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapSmallShip, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapSmallShip,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map large ship
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapLargeShip, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapLargeShip,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map outpost
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapOutpost, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapOutpost,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map grid
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Grid, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Grid,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Solar map navigation mark
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapNavigationMark, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapNavigationMark,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapSideMission, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapSideMission,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapStoryMission, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapStoryMission,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapTemplateMission, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapTemplateMission,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapPlayer, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SolarMapPlayer,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            MaterialProperties.Add((int)MyTransparentMaterialEnum.Smoke_Ignore_Depth, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Smoke,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: false,
                IgnoreDepth: true,
                UseAtlas: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //entered selected container bounding box
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ContainerBorderSelected, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ContainerBorderSelected,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionChina
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionChina, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.FactionChina,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionChurch
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionChurch, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionChurch,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionCSR
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionCSR, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionCSR,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionEAC
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionEAC, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionEAC,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionFourthReich
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionFourthReich, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionFourthReich,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionFreeAsia
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionFreeAsia, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionFreeAsia,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionIndia
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionIndia, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionIndia,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionJapan
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionJapan, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionJapan,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionOmnicorp
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionOmnicorp, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionOmnicorp,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionRussia
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionRussia, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionRussia,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionRussia
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionSlavers, new MyTransparentMaterialProperties(

                Texture: MyTransparentGeometryTexturesEnum.FactionSlavers,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionSaudi
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionSaudi, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionSaudi,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // FactionUnitedKorea
            MaterialProperties.Add((int)MyTransparentMaterialEnum.FactionUnitedKorea, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.FactionUnitedKorea,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            //Smoke_b
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Smoke_b, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.Smoke_b,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));

            //Smoke_c
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Smoke_c, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.Smoke_c,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));

            //Sparks_a
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Sparks_a, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.Sparks_a,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: true
            ));
            //Sparks_b
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Sparks_b, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.Sparks_b,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: true
            ));
            //particle_stone
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_stone, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_stone,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));
            //Stardust
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Stardust, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.Stardust,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                NeedSort: false,
                Emissivity: 0
            ));
            //particle_trash_a
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_trash_a, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_trash_a,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: true
            ));
            //particle_trash_b
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_trash_b, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_trash_b,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: true
            ));
            //particle_glare
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_glare, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_glare,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: false
            ));
            //smoke_field
            MaterialProperties.Add((int)MyTransparentMaterialEnum.smoke_field, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.smoke_field,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));
            //Explosion_pieces
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Explosion_pieces, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.Explosion_pieces,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));
            //particle_laser
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_laser, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_laser,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE
            ));

            //particle_nuclear
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_nuclear, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_nuclear,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE
            ));

            //Explosion_line
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Explosion_line, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.Explosion_line,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));
            //snap_point
            MaterialProperties.Add((int)MyTransparentMaterialEnum.snap_point, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.snap_point,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE
            ));

            //particle_flash_a
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_flash_a, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_flash_a,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));

            //particle_flash_b
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_flash_b, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_flash_b,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));

            //particle_flash_c
            MaterialProperties.Add((int)MyTransparentMaterialEnum.particle_flash_c, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.particle_flash_c,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: true
            ));


            //Decal Glare
            MaterialProperties.Add((int)MyTransparentMaterialEnum.DecalGlare, new MyTransparentMaterialProperties(
            
                Texture: MyTransparentGeometryTexturesEnum.LightGlare,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_DECAL_PARTICLES,
                UseAtlas: false //problems with DXT1
            ));

            //Impostor material
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Impostor_StaticAsteroid20m_A, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Impostor_StaticAsteroid20m_A,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false
            ));

            //Impostor material
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Impostor_StaticAsteroid20m_C, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Impostor_StaticAsteroid20m_C,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false
            ));

            //Impostor materials
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Impostor_StaticAsteroid50m_D, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Impostor_StaticAsteroid50m_D,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false
            ));

            //Impostor materials
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Impostor_StaticAsteroid50m_E, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Impostor_StaticAsteroid50m_E,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false
            ));

            //GPS
            MaterialProperties.Add((int)MyTransparentMaterialEnum.GPS, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.GPS,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false,
                NeedSort: false,
                Emissivity: 0.15f,
                AlphaMistingStart: 10,
                AlphaMistingEnd: 80
            ));

            //GPSBack
            MaterialProperties.Add((int)MyTransparentMaterialEnum.GPSBack, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.GPSBack,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false,
                NeedSort: false,
                Emissivity: 0.15f,
                AlphaMistingStart: 10,
                AlphaMistingEnd: 80
            ));

            //ShotgunParticle
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ShotgunParticle, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ShotgunParticle,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: false,
                NeedSort: false,
                Emissivity: 0.0f,
                AlphaMistingStart: 10,
                AlphaMistingEnd: 80
            ));
            //snap_point_depth
            MaterialProperties.Add((int)MyTransparentMaterialEnum.snap_point_depth, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.snap_point,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE
            ));
            // Objective cube face
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ObjectiveDummyFace, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ObjectiveDummyFace,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));
            // Objective cube line
            MaterialProperties.Add((int)MyTransparentMaterialEnum.ObjectiveDummyLine, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.ObjectiveDummyLine,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                NeedSort: false
            ));

            //Sun Disk
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SunDisk, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.SunDisk,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                Emissivity: 0f,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES
            ));

            // Volumetric glare (not affected by occlusion)
            MaterialProperties.Add((int)MyTransparentMaterialEnum.VolumetricGlare, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.LightGlare,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                IgnoreDepth: false,
                SoftParticleDistanceScale: 0.2f
            ));

            //Solar map dust
            MaterialProperties.Add((int)MyTransparentMaterialEnum.SolarMapDust, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Smoke,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,
                UseAtlas: true
            ));

            // Scanner 01
            MaterialProperties.Add((int)MyTransparentMaterialEnum.scanner_01, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.scanner_01,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: false,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES,                
                Emissivity: 0.15f
            ));

            //Smoke  square
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Smoke_square, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Smoke_square,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: false
            ));

            //Smoke  lit
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Smoke_lit, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Smoke_lit,
                CanBeAffectedByOtherLights: true,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: false
            ));

            //Smoke  square unlit
            MaterialProperties.Add((int)MyTransparentMaterialEnum.Smoke_square_unlit, new MyTransparentMaterialProperties(
                Texture: MyTransparentGeometryTexturesEnum.Smoke_square,
                CanBeAffectedByOtherLights: false,
                AlphaMistingEnable: true,
                SoftParticleDistanceScale: MyTransparentGeometryConstants.SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE,
                UseAtlas: false
            ));
        }

        public static MyTransparentMaterialProperties GetMaterialProperties(MyTransparentMaterialEnum materialType)
        {
            return MaterialProperties[(int)materialType];
        }
    }
}
