using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.BackgroundCube;

namespace MinerWars.AppCode.Game.SolarSystem
{
    partial class MySolarSystemConstants
    {
        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> Hell25PrimaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Sandstone_01, 1},
            { MyMwcVoxelMaterialsEnum.Stone_03, 1},
        };

        public readonly static Dictionary<MyMwcVoxelMaterialsEnum, float> Hell25SecondaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>()
        {
            { MyMwcVoxelMaterialsEnum.Lava_01, 1},
            { MyMwcVoxelMaterialsEnum.Treasure_01, 1},
        };

        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateHell25DArea()
        {
            Areas[MySolarSystemAreaEnum.Hell25D] = new MySolarSystemAreaOrbit()
            {
                Name = "Hell 25D",
                AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                OrbitProperties = new MyOrbitProperties()
                {
                    OrbitCenter = Vector3.Zero,
                    AreaCenter = MyBgrCubeConsts.LAIKA_POSITION * MyBgrCubeConsts.MILLION_KM * 1000,
                    LongSpread = 0.001f,
                    MaxDistanceFromOrbitLow = MyBgrCubeConsts.MERCURY_RADIUS * MyBgrCubeConsts.MILLION_KM * 100,
                    MaxDistanceFromOrbitHigh = MyBgrCubeConsts.MERCURY_RADIUS * MyBgrCubeConsts.MILLION_KM * 200,
                },
                SolarMapData = new MySolarMapData()
                {
                    DustColor = new Vector3(0.9f, 0.60f, 0.37f),
                    DustColorVariability = new Vector4(0.15f),
                    TemplateGroups = new MyTemplateGroupInfo[]
                    {
                        new MyTemplateGroupInfo{ Importance = 1, Count = 5, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                        new MyTemplateGroupInfo{ Importance = 0.5f, Count = 20, TemplateGroup = MyTemplateGroupEnum.RandomStations},
                    }
                },
                SectorData = new MySolarSectorData()
                {
                    BackgroundTexture = "BackgroundCube",
                    SunProperties = LaikaSunProperties,
                    FogProperties = LaikaFogProperties,
                    ImpostorProperties = LaikaImpostorsProperties,
                    DebrisProperties = LaikaDebrisProperties,

                    SectorObjectsCounts = LaikaObjectsProperties,
                    PrimaryAsteroidMaterials = Hell25PrimaryAsteroidMaterials,
                    SecondaryAsteroidMaterials = Hell25SecondaryAsteroidMaterials,

                    ParticleDustProperties = LaikaDustProperties,
                },
            };
        }
    }
}
