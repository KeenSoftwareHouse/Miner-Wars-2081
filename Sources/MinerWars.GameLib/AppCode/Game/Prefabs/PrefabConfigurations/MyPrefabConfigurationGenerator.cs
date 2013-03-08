using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfigurationGenerator : MyPrefabConfiguration
    {
        public float Range { get; set; }

        public MyPrefabConfigurationGenerator(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, float range, float rotatingVelocity = DEFAULT_ROTATING_VELOCITY, MyModelsEnum? modelCol = null,
            float explosionParticleEffectScale = 1)
            : this(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Generator, range, rotatingVelocity,
            MySoundCuesEnum.Amb3D_GenXstart, MySoundCuesEnum.Amb3D_GenXloop, MySoundCuesEnum.Amb3D_GenXend, modelCol, explosionParticleEffectScale:explosionParticleEffectScale)
        {

        }

        public MyPrefabConfigurationGenerator(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, PrefabTypesFlagEnum prefabTypeFlag, float range, float rotatingVelocity = DEFAULT_ROTATING_VELOCITY,
            MySoundCuesEnum? startRotatingCue = MySoundCuesEnum.Amb3D_GenXstart, MySoundCuesEnum? looptRotatingCue = MySoundCuesEnum.Amb3D_GenXloop, MySoundCuesEnum? endRotatingCue = MySoundCuesEnum.Amb3D_GenXend, MyModelsEnum? modelCol = null,
            float explosionParticleEffectScale = 1)
            : base(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypeFlag, minElectricCapacity: MyGameplayConstants.DEFAULT_MAX_ELECTRIC_CAPACITY, rotatingVelocity: rotatingVelocity, collisionModelEnum: modelCol,
            startRotatingCue: startRotatingCue, loopRotatingCue: looptRotatingCue, endRotatingCue: endRotatingCue, displayHud: true, explosionParticleEffectScale:explosionParticleEffectScale)
        {
            Range = range;
        }
    }
}
