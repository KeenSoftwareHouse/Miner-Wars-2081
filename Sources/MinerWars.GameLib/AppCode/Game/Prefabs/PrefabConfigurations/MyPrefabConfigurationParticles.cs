using System;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using SysUtils.Utils;


namespace MinerWars.AppCode.Game.Prefabs
{
    using Audio;

    class MyPrefabConfigurationParticles : MyPrefabConfiguration
    {
        public MyParticleEffectsIDEnum EffectID { get; private set; }

        public MyPrefabConfigurationParticles(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MyParticleEffectsIDEnum effectID, PrefabTypesFlagEnum prefabTypeFlag)
            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypeFlag)
        {
            EffectID = effectID;
        }

        public MyPrefabConfigurationParticles(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MyParticleEffectsIDEnum effectID)
            : this(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, effectID, PrefabTypesFlagEnum.Default)
        {            
        }
    }
}
