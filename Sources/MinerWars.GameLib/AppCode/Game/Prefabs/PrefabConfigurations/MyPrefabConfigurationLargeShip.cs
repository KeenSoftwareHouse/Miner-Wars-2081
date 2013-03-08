using MinerWars.AppCode.Game.Models;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Explosions;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfigurationLargeShip : MyPrefabConfiguration
    {
        public MyPrefabConfigurationLargeShip(
            MyModelsEnum modelLod0Enum,
            MyModelsEnum? modelLod1Enum,
            BuildTypesEnum buildType,
            CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType,
            MyMaterialType materialType,
            MyMwcObjectBuilder_Prefab_AppearanceEnum? factionSpecific = null,
            MyModelsEnum? collisionModelEnum = null,
            MyExplosionTypeEnum explosionType = MyExplosionTypeEnum.BOMB_EXPLOSION,
            float particleScale = 1)

            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.LargeShip, factionSpecific: factionSpecific, needsUpdate: true, collisionModelEnum: collisionModelEnum, explosionType: explosionType, explosionParticleEffectScale: particleScale)
        {
        }
    }   
}
