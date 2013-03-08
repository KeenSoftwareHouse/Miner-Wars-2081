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
  
    class MyPrefabConfigurationLight : MyPrefabConfiguration
    {
        public MyLightPrefabTypeEnum LightType { get; private set; }

        public MyPrefabConfigurationLight(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MyLightPrefabTypeEnum type, MyModelsEnum? collisionModelEnum = null)
            : this(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, type, PrefabTypesFlagEnum.Default, collisionModelEnum)
        {            
        }

        public MyPrefabConfigurationLight(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MyLightPrefabTypeEnum type, PrefabTypesFlagEnum prefabTypesFlag, MyModelsEnum? collisionModelEnum = null)
            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypesFlag, previewAngle: MyPreviewPointOfViewEnum.Bottom, needsUpdate: true, collisionModelEnum: collisionModelEnum)
        {
            LightType = type;
        }
    }
}
