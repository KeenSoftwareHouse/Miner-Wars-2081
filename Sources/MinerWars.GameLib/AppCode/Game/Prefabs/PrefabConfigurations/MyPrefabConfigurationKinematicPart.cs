using MinerWars.AppCode.Game.Models;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfigurationKinematicPart : MyPrefabConfiguration
    {
        public MyModelsEnum? m_modelMovingEnum;
        public string m_open;
        public string m_close;
        public DamageTypesEnum m_damageType;
        public int PrefabId { get; private set; }

        public MyPrefabConfigurationKinematicPart(int prefabId, MyModelsEnum modelMovingEnum, string open, string close, BuildTypesEnum buildType,
            CategoryTypesEnum categoryType, SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, DamageTypesEnum damageType, MyModelsEnum? modelLod1 = null, MyModelsEnum? collisionModelEnum = null)
            : base(modelMovingEnum, modelLod1, buildType, categoryType, subCategoryType, materialType,
                                                                   //0.5 because of door parts, to not take also second part
                   PrefabTypesFlagEnum.Default, needsUpdate: true, explosionRadiusMultiplier: 0.5f, explosionDamage: 0.33f, collisionModelEnum: collisionModelEnum, explosionParticleEffectScale: 2.0f)
        {
            m_modelMovingEnum = modelMovingEnum;
            m_open = open;
            m_close = close;
            m_damageType = damageType;
            PrefabId = prefabId;
        }
    }
}
