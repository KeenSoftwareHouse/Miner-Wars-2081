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

    class MyPrefabConfigurationKinematic : MyPrefabConfiguration
    {
        public MySoundCuesEnum? m_soundOpening; public MySoundCuesEnum? m_soundLooping; public MySoundCuesEnum? m_soundClosing;
        public float m_openTime;
        public float m_closeTime;

        public List<MyPrefabConfigurationKinematicPart> KinematicParts { get; private set; }

        public MyPrefabConfigurationKinematic(MyModelsEnum modelBaseLod0Enum, MyModelsEnum? modelBaseLod1Enum, 
                List<MyPrefabConfigurationKinematicPart> kinematicParts, MySoundCuesEnum? soundOpening, MySoundCuesEnum? soundLooping, 
                MySoundCuesEnum? soundClosing, float openTime, float closeTime, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
                SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, PrefabTypesFlagEnum prefabTypeFlag, MyModelsEnum? collisionModelEnum = null)
            : base(modelBaseLod0Enum, modelBaseLod1Enum, buildType, categoryType, subCategoryType, materialType, prefabTypeFlag, collisionModelEnum: collisionModelEnum, displayHud: true)
        {
            m_openTime = openTime;
            m_closeTime = closeTime;
            KinematicParts = kinematicParts;
            m_soundOpening = soundOpening;
            m_soundLooping = soundLooping;
            m_soundClosing = soundClosing;
        }

        public MyPrefabConfigurationKinematic(MyModelsEnum modelBaseLod0Enum, MyModelsEnum? modelBaseLod1Enum,
                List<MyPrefabConfigurationKinematicPart> kinematicParts, MySoundCuesEnum? soundOpening, MySoundCuesEnum? soundLooping, 
                MySoundCuesEnum? soundClosing, float openTime, float closeTime, BuildTypesEnum buildType, CategoryTypesEnum categoryType,
                SubCategoryTypesEnum? subCategoryType, MyMaterialType materialType, MyModelsEnum? collisionModelEnum = null)
            : this(modelBaseLod0Enum, modelBaseLod1Enum, kinematicParts, soundOpening, soundLooping, soundClosing, openTime, closeTime,
                   buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Default, collisionModelEnum)
        {
        }
    }
}
