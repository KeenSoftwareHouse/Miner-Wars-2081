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
 
    class MyPrefabConfigurationSound : MyPrefabConfiguration
    {

        public MySoundCuesEnum Sound { get; private set; }

        public MyPrefabConfigurationSound(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MySoundCuesEnum sound, PrefabTypesFlagEnum prefabTypesFlag)
            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypesFlag, needsUpdate: true)
        {
            Sound = sound;
        }

        public MyPrefabConfigurationSound(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MySoundCuesEnum sound)
            : this(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, sound, PrefabTypesFlagEnum.Default)
        {            
        }
    }
}
