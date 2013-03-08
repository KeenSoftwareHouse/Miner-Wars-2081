using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.AppCode.Game.SolarSystem
{
    class MySolarSectorData
    {
        public MySunProperties SunProperties = MySolarSystemConstants.DefaultSunProperties;
        public MyFogProperties FogProperties = MySolarSystemConstants.DefaultFogProperties;
        public MyDebrisProperties DebrisProperties = MySolarSystemConstants.DefaultDebrisProperties;
        public MyParticleDustProperties ParticleDustProperties = MySolarSystemConstants.DefaultParticleDustProperties;
        public MyGodRaysProperties GodRaysProperties = MySolarSystemConstants.DefaultGodRaysProperties;
        public MyImpostorProperties[] ImpostorProperties = MySolarSystemConstants.DefaultImpostorsProperties;
        public string BackgroundTexture = "BackgroundCube";

        public string AreaName;

        /// <summary>
        /// Key is asteroid material, value is weight (sum of weights do not need to be 1)
        /// </summary>
        public Dictionary<MyMwcVoxelMaterialsEnum, float> PrimaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>();

        /// <summary>
        /// Secondary materials (added to voxels using voxel hand)
        /// </summary>
        public Dictionary<MyMwcVoxelMaterialsEnum, float> SecondaryAsteroidMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, float>();


        /// <summary>
        /// Allowed voxel materials in area
        /// </summary>
        public List<MyMwcVoxelMaterialsEnum> AllowedAsteroidMaterials = new List<MyMwcVoxelMaterialsEnum>();

        /// <summary>
        /// Key is size in km, value number of asteroids per sector (in areas with highest density)
        /// Mean is applied to values when generating sector
        /// </summary>
        public MySectorObjectCounts SectorObjectsCounts = new MySectorObjectCounts();


        /// <summary>
        /// Interpolates dictionary which contains weights
        /// </summary>
        /// <param name="interpolator">1 means use second</param>
        private Dictionary<TKey, float> InterpolateDictionary<TKey>(Dictionary<TKey, float> first, Dictionary<TKey, float> second, float interpolator)
        {
            var result = new Dictionary<TKey, float>();
            var keys = first.Keys.Union(second.Keys).Distinct();
            foreach (var k in keys)
            {
                float firstValue;
                float secondValue;
                if (!first.TryGetValue(k, out firstValue))
                {
                    firstValue = 0;
                }
                if (!second.TryGetValue(k, out secondValue))
                {
                    secondValue = 0;
                }
                result[k] = MathHelper.Lerp(firstValue, secondValue, interpolator);
            }
            return result;
        }

        /// <param name="interpolator">0 - use this object, 1 - use other object</param>
        public MySolarSectorData InterpolateWith(MySolarSectorData otherProperties, float interpolator)
        {
            var result = new MySolarSectorData();
            result.AreaName = this.AreaName; // Area name cannot be interpolated, use current
            result.BackgroundTexture = this.BackgroundTexture;
            
            result.SunProperties = this.SunProperties.InterpolateWith(otherProperties.SunProperties, interpolator);
            result.FogProperties = this.FogProperties.InterpolateWith(otherProperties.FogProperties, interpolator);
            result.SectorObjectsCounts = this.SectorObjectsCounts.InterpolateWith(otherProperties.SectorObjectsCounts, interpolator);
            result.DebrisProperties = this.DebrisProperties.InterpolateWith(otherProperties.DebrisProperties, interpolator);
            result.ParticleDustProperties = this.ParticleDustProperties.InterpolateWith(otherProperties.ParticleDustProperties, interpolator);
            result.GodRaysProperties = this.GodRaysProperties.InterpolateWith(otherProperties.GodRaysProperties, interpolator);
            result.PrimaryAsteroidMaterials = InterpolateDictionary(this.PrimaryAsteroidMaterials, otherProperties.PrimaryAsteroidMaterials, interpolator);
            result.SecondaryAsteroidMaterials = InterpolateDictionary(this.SecondaryAsteroidMaterials, otherProperties.SecondaryAsteroidMaterials, interpolator);
            result.AllowedAsteroidMaterials = this.AllowedAsteroidMaterials;
            if (interpolator > 0.5f)
            {
                result.ImpostorProperties = otherProperties.ImpostorProperties;
            }
            else
            {
                result.ImpostorProperties = this.ImpostorProperties;
            }
            return result;
        }
    }

    class MyTemplateGroupInfo
    {
        /// <summary>
        /// 1 = full importance, show always
        /// 0.5 = half importance, show at half zoom
        /// 0 = no importance, show only at full zoom
        /// </summary>
        public float Importance;

        /// <summary>
        /// 
        /// </summary>
        public float Count;

        /// <summary>
        /// From which group load asteroids
        /// </summary>
        public MyTemplateGroupEnum TemplateGroup;
    }

    class MySolarMapData
    {
        public Vector3 DustColor { get; set; }
        public Vector4 DustColorVariability { get; set; }
        public MyTemplateGroupInfo[] TemplateGroups { get; set; }
    }

    class MySolarSystemArea
    {
        [Flags]
        public enum AreaEnum
        {
            None = 0x00,
            PostPlanet = 0x02,
            Sun = 0x04,
        }

        public string Name;
        public AreaEnum AreaType;


        public MySolarMapData SolarMapData = new MySolarMapData(); // For drawing solar dust field, can be null (no dust field)
        public MySolarSectorData SectorData = new MySolarSectorData();

        public MyMwcVoxelMaterialsEnum? SecondaryStaticAsteroidMaterial;

        /// <summary>
        /// Returns how much is sector influented by area
        /// </summary>
        /// <param name="sectorPosition">Position of sector</param>
        /// <returns>Zero when sector is not influented, one when sector is fully influented, number between 0 and 1 when partially influented.</returns>
        public virtual float GetSectorInterpolator(MyMwcVector3Int sectorPosition)
        {
            return 0;
        }

        /// <summary>
        /// Add universe entities, like dust field billboard, asteroid field billboards etc
        /// </summary>
        /// <param name="entities"></param>
        public virtual void AddUniverseEntities(MySolarSystemMapData data)
        {
             // Do nothing 
        }

        /// <summary>
        /// Position in km
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 GetCenter()
        {
            return new Vector3(0, 0, 0);
        }
    }
}
