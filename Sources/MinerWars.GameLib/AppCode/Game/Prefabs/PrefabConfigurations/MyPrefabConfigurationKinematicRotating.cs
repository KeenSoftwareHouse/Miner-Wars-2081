using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Models;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfigurationKinematicRotating : MyPrefabConfiguration
    {
        public MySoundCuesEnum? SoundOpening; 
        public MySoundCuesEnum? SoundLooping; 
        public MySoundCuesEnum? SoundClosing;
        public float RotatingVelocity;   

        public MyPrefabConfigurationKinematicRotating(MyModelsEnum modelBaseLod0Enum, MyModelsEnum? modelBaseLod1Enum, 
                MySoundCuesEnum? soundOpening, MySoundCuesEnum? soundLooping, 
                MySoundCuesEnum? soundClosing, float rotatingVelocity, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
                SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, PrefabTypesFlagEnum prefabTypeFlag)
            : base(modelBaseLod0Enum, modelBaseLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypeFlag)
        {
            SoundOpening = soundOpening;
            SoundLooping = soundLooping;
            SoundClosing = soundClosing;
            RotatingVelocity = rotatingVelocity;
        }

        public MyPrefabConfigurationKinematicRotating(MyModelsEnum modelBaseLod0Enum, MyModelsEnum? modelBaseLod1Enum,
                MySoundCuesEnum? soundOpening, MySoundCuesEnum? soundLooping, 
                MySoundCuesEnum? soundClosing, float rotatingVelocity, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
                SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType)
            : this(modelBaseLod0Enum, modelBaseLod1Enum, soundOpening, soundLooping, soundClosing, rotatingVelocity, 
                   buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Default)
        {
        }    
    }
}
