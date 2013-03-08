using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Models;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfigurationAlarm : MyPrefabConfiguration
    {
        public MyModelsEnum ModelLod0EnumOn;
        public MyModelsEnum? ModelLod1EnumOn;

        public MyPrefabConfigurationAlarm(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, MyModelsEnum modelLod0EnumOn, MyModelsEnum? modelLod1EnumOn, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType)
            : this(modelLod0Enum, modelLod1Enum, modelLod0EnumOn, modelLod1EnumOn, buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Alarm)
        {
        }

        public MyPrefabConfigurationAlarm(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, MyModelsEnum modelLod0EnumOn, MyModelsEnum? modelLod1EnumOn, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, PrefabTypesFlagEnum prefabTypeFlag)
            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypeFlag, needsUpdate: true, requiresEnergy: false)
        {
            ModelLod0EnumOn = modelLod0EnumOn;
            ModelLod1EnumOn = modelLod1EnumOn;
        }    
    }
}
