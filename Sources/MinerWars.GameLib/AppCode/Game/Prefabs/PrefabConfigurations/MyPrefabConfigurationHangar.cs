using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Models;

namespace MinerWars.AppCode.Game.Prefabs
{    
    class MyPrefabConfigurationHangar : MyPrefabConfiguration
    {
        public MyPrefabConfigurationHangar(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MyModelsEnum? collisionModel = null)
            : this(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Hangar, collisionModel: collisionModel)
        {
        }

        public MyPrefabConfigurationHangar(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, PrefabTypesFlagEnum prefabTypeFlag, MyModelsEnum? collisionModel = null)
            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypeFlag, displayHud: true, collisionModelEnum: collisionModel)
        {
        }
    } 
}
