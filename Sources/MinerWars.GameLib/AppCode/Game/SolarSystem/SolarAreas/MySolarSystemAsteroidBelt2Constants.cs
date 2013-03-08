using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.SolarSystem
{
    partial class MySolarSystemConstants
    {
       

        
        /// <summary>
        /// Rebuild the areas
        /// </summary>
        public static void CreateAsteroidBelt2Area()
        {
            Areas[MySolarSystemAreaEnum.AsteroidBelt2] = new MySolarSystemAreaOrbit()
              {
                  Name = "Asteroid belt 2",
                  AreaType = MySolarSystemArea.AreaEnum.PostPlanet,
                  OrbitProperties = new MyOrbitProperties()
                  {
                      OrbitCenter = Vector3.Zero,
                      AreaCenter = MySolarMapRenderer.SectorToKm(new MyMwcVector3Int(-2371982, 0, -971832)), //player start position

                      LongSpread = 0.5f,
                      MaxDistanceFromOrbitLow = MyBgrCubeConsts.VENUS_RADIUS * MyBgrCubeConsts.MILLION_KM * 300,
                      MaxDistanceFromOrbitHigh = MyBgrCubeConsts.VENUS_RADIUS * MyBgrCubeConsts.MILLION_KM * 1200,
                  },
                  SolarMapData = new MySolarMapData()
                  {
                      DustColor = new Vector3(0.278f, 0.5f, 0.21f),
                      DustColorVariability = new Vector4(0.15f),
                  },
                  SectorData = new MySolarSectorData()
                  {
                      SectorObjectsCounts = DefaultObjectsProperties,
                      PrimaryAsteroidMaterials = DefaultAsteroidMaterials,
                      SunProperties = DefaultSunProperties,
                      ImpostorProperties = DefaultImpostorsProperties,
                      FogProperties = DefaultFogProperties,
                      DebrisProperties = DefaultDebrisProperties
                  },
              };
        }
    }
}
