using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Models;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfigurationCamera : MyPrefabConfiguration
    {

        public MyPrefabConfigurationCamera(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType)
            : this(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Scanner)
        {
        }

        public MyPrefabConfigurationCamera(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
                SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, PrefabTypesFlagEnum prefabTypeFlag)
            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypeFlag, requiresEnergy: true)
        {
        }
    }
}
