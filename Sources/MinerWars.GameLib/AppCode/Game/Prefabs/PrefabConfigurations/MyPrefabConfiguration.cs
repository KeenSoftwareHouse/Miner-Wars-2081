using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Models;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfiguration
    {
        public const float DEFAULT_ROTATING_VELOCITY = (float)(Math.PI * 2);   // in rad/sec

        private readonly MyModelsEnum m_modelLod0Enum;
        private readonly MyModelsEnum? m_modelLod1Enum;   //if lod m_modelLod1Enum is same as Lod0 -> entity has no lod1
        private readonly MyModelsEnum? m_collisionModelEnum; //collision model for prefab

        public readonly BuildTypesEnum BuildType;
        public readonly CategoryTypesEnum CategoryType;
        public readonly SubCategoryTypesEnum? SubCategoryType;
        public readonly MyMaterialType MaterialType;
        public readonly PrefabTypesFlagEnum PrefabTypeFlag;

        /// <summary>
        /// If not null, indicates the faction for which this prefab is exclusive.
        /// </summary>
        public readonly MyMwcObjectBuilder_Prefab_AppearanceEnum? FactionSpecific;

        /// <summary>
        /// Indicates if the prefab type can be added from the editor.
        /// </summary>
        public readonly bool EnabledInEditor;

        /// <summary>
        /// Indicates whether the preview image for this prefab should be from a different angle than normal.
        /// Use for flat objects that have incorrect default previews (such as signs).
        /// </summary>
        public readonly MyPreviewPointOfView PreviewPointOfView;

        /// <summary>
        /// Indicates what explosion type the prefab generates after it is destroyed.
        /// </summary>
        public MyExplosionTypeEnum ExplosionType;

        /// <summary>
        /// The multiplier for the radius of the destruction explosion effect.
        /// </summary>
        public readonly float ExplosionRadiusMultiplier;

        /// <summary>
        /// The multiplier for the damaged caused by the explosion of the prefab.
        /// </summary>
        public float ExplosionDamage;

        /// <summary>
        /// The multiplier for the scale of the particle effect created when the prefab explodes.
        /// </summary>
        public readonly float ExplosionParticleEffectScale;

        /// <summary>
        /// Min object's size for create explosion
        /// </summary>
        public readonly float MinSizeForExplosion;

        /// <summary>
        /// True if destroying of this prefab raises alarm in container
        /// </summary>
        public readonly bool CausesAlarm;

        /// <summary>
        /// True if prefab requires energy to operate
        /// </summary>
        public readonly bool? RequiresEnergy;

        public MyModelsEnum ModelLod0Enum { get { return m_modelLod0Enum; } }
        public MyModelsEnum? ModelLod1Enum { get { return m_modelLod1Enum; } }
        public MyModelsEnum? ModelCollisionEnum { get { return m_collisionModelEnum; } }

        /// <summary>
        /// Minimum electric capacity, emp weapons limit
        /// </summary>
        public readonly float MinElectricCapacity;
        public readonly float MaxElectricCapacity;

        public readonly bool NeedsUpdate;
        public readonly bool InitPhysics;
        public readonly float RotatingVelocity;

        // These are used only when prefab has rotating parts.
        public readonly MySoundCuesEnum? StartRotatingCue;
        public readonly MySoundCuesEnum? LoopRotatingCue;
        public readonly MySoundCuesEnum? LoopRotatingDamagedCue;
        public readonly MySoundCuesEnum? EndRotatingCue;

        public readonly bool DisplayHud;

        /// <summary>
        /// Creates new instance of prefab configuration
        /// </summary>
        /// <param name="modelLod0Enum">Model LOD0</param>
        /// <param name="modelLod1Enum">Model LOD2</param>
        /// <param name="buildType">Build type</param>
        /// <param name="categoryType">Category type</param>
        /// <param name="subCategoryType">Subcategory type</param>
        /// <param name="materialType">Material type</param>
        /// <param name="prefabTypeFlag">Prefab type flags</param>
        protected MyPrefabConfiguration(
            MyModelsEnum modelLod0Enum,
            MyModelsEnum? modelLod1Enum,
            BuildTypesEnum buildType,
            CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType,
            MyMaterialType materialType,
            PrefabTypesFlagEnum prefabTypeFlag,
            MyModelsEnum? collisionModelEnum = null,
            MyMwcObjectBuilder_Prefab_AppearanceEnum? factionSpecific = null,
            bool needsUpdate = false,
            bool initPhysics = true,
            bool enabledInEditor = true,
            float rotatingVelocity = DEFAULT_ROTATING_VELOCITY,
            MySoundCuesEnum? startRotatingCue = null,
            MySoundCuesEnum? loopRotatingCue = null,
            MySoundCuesEnum? loopRotatingDamagedCue = null,
            MySoundCuesEnum? endRotatingCue = null,
            MyPreviewPointOfViewEnum previewAngle = MyPreviewPointOfViewEnum.Front,
            float minElectricCapacity = MyGameplayConstants.DEFAULT_MIN_ELECTRIC_CAPACITY,
            float maxElectricCapacity = MyGameplayConstants.DEFAULT_MAX_ELECTRIC_CAPACITY,
            MyExplosionTypeEnum explosionType = MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION,
            float explosionRadiusMultiplier = 1,
            float explosionDamage = 1,
            float minSizeForExplosion = MyExplosionsConstants.MIN_OBJECT_SIZE_TO_CAUSE_EXPLOSION_AND_CREATE_DEBRIS,
            bool causesAlarm = false,
            bool requiresEnergy = false,
            float explosionParticleEffectScale = 1,
            bool displayHud = false)
            : this(buildType, categoryType, subCategoryType, materialType, prefabTypeFlag, factionSpecific, needsUpdate, initPhysics, enabledInEditor, rotatingVelocity, 
            startRotatingCue, loopRotatingCue, loopRotatingDamagedCue, endRotatingCue,
            previewAngle, minElectricCapacity, maxElectricCapacity, explosionType, explosionRadiusMultiplier, explosionDamage, minSizeForExplosion, causesAlarm, requiresEnergy: requiresEnergy, explosionParticleEffectScale: explosionParticleEffectScale, displayHud: displayHud)
        {
            m_modelLod0Enum = modelLod0Enum;
            m_modelLod1Enum = modelLod1Enum;
            m_collisionModelEnum = collisionModelEnum;
        }

        /// <summary>
        /// Creates new instance of prefab configuration with default prefab type flags
        /// </summary>
        /// <param name="modelLod0Enum">Model LOD0</param>
        /// <param name="modelLod1Enum">Model LOD2</param>
        /// <param name="buildType">Build type</param>
        /// <param name="categoryType">Category type</param>
        /// <param name="subCategoryType">Subcategory type</param>
        /// <param name="materialType">Material type</param>
        /// <param name="factionSpecific">To which faction is this prefab specific. Null if prefab is for all factions. Used for material (texture) set availability. </param>     
        public MyPrefabConfiguration(
            MyModelsEnum modelLod0Enum,
            MyModelsEnum? modelLod1Enum,
            BuildTypesEnum buildType,
            CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType,
            MyMaterialType materialType,
            MyMwcObjectBuilder_Prefab_AppearanceEnum? factionSpecific = null,
            MyModelsEnum? collisionModelEnum = null,
            bool needsUpdate = false,
            bool initPhysics = true,
            bool enabledInEditor = true,
            float rotatingVelocity = DEFAULT_ROTATING_VELOCITY,
            MySoundCuesEnum? startRotatingCue = null,
            MySoundCuesEnum? loopRotatingCue = null,
            MySoundCuesEnum? loopRotatingDamagedCue = null,
            MySoundCuesEnum? endRotatingCue = null,
            MyPreviewPointOfViewEnum previewAngle = MyPreviewPointOfViewEnum.Front,
            MyExplosionTypeEnum explosionType = MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION,
            float explosionRadiusMultiplier = 1,
            float explosionDamageMultiplier = 1,
            float minSizeForExplosion = MyExplosionsConstants.MIN_OBJECT_SIZE_TO_CAUSE_EXPLOSION_AND_CREATE_DEBRIS,
            bool causesAlarm = false,
            bool requiresEnergy = false,
            float explosionParticleEffectScale = 1,
            bool displayHud = false)
            : this(modelLod0Enum, modelLod1Enum, buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Default, collisionModelEnum, factionSpecific, needsUpdate, initPhysics, enabledInEditor, rotatingVelocity,
            startRotatingCue, loopRotatingCue, loopRotatingDamagedCue, endRotatingCue,
            previewAngle, explosionType: explosionType, explosionRadiusMultiplier: explosionRadiusMultiplier, explosionDamage: explosionDamageMultiplier, minSizeForExplosion: minSizeForExplosion, causesAlarm: causesAlarm, requiresEnergy: requiresEnergy, explosionParticleEffectScale: explosionParticleEffectScale, displayHud: displayHud)
        {
            System.Diagnostics.Debug.Assert(modelLod0Enum != modelLod1Enum, "LOD0 and LOD1 models are the same!");
            if (collisionModelEnum != null)
            {
                System.Diagnostics.Debug.Assert(modelLod0Enum != collisionModelEnum, "LOD0 and COL models are the same!");
                System.Diagnostics.Debug.Assert(modelLod1Enum != collisionModelEnum, "LOD1 and COL models are the same!");
            }
        }

        /// <summary>
        /// Creates new instance of prefab configuration without model
        /// </summary>
        /// <param name="buildType">Build type</param>
        /// <param name="categoryType">Category type</param>
        /// <param name="subCategoryType">Subcategory type</param>
        /// <param name="materialType">Material type</param>
        /// <param name="prefabTypeFlag">Prefab type flags</param>
        /// <param name="factionSpecific">To which faction is this prefab specific. Null if prefab is for all factions. Used for material (texture) set availability. </param>
        /// <param name="previewPointOfView">Indicates whether the preview image for this prefab should be from a different angle than normal. Use for flat objects that have incorrect default previews (such as signs). </param>
        protected MyPrefabConfiguration(
            BuildTypesEnum buildType,
            CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType,
            MyMaterialType materialType,
            PrefabTypesFlagEnum prefabTypeFlag,
            MyMwcObjectBuilder_Prefab_AppearanceEnum? factionSpecific = null,
            bool needsUpdate = false,
            bool initPhysics = true,
            bool enabledInEditor = true,
            float rotatingVelocity = DEFAULT_ROTATING_VELOCITY,
            MySoundCuesEnum? startRotatingCue = null,
            MySoundCuesEnum? loopRotatingCue = null,
            MySoundCuesEnum? loopRotatingDamagedCue = null,
            MySoundCuesEnum? endRotatingCue = null,
            MyPreviewPointOfViewEnum previewPointOfView = MyPreviewPointOfViewEnum.Front,
            float minElectricCapacity = MyGameplayConstants.DEFAULT_MIN_ELECTRIC_CAPACITY,
            float maxElectricCapacity = MyGameplayConstants.DEFAULT_MAX_ELECTRIC_CAPACITY,
            MyExplosionTypeEnum explosionType = MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION,
            float explosionRadiusMultiplier = 1.5f,
            float explosionDamageMultiplier = 1,
            float minSizeForExplosion = MyExplosionsConstants.MIN_OBJECT_SIZE_TO_CAUSE_EXPLOSION_AND_CREATE_DEBRIS,
            bool causesAlarm = false,
            bool requiresEnergy = false,
            float explosionParticleEffectScale = 1,
            bool displayHud = false)
        {
            //m_modelLod0Enum = null;
            m_modelLod1Enum = null;

            BuildType = buildType;
            CategoryType = categoryType;
            SubCategoryType = subCategoryType;
            MaterialType = materialType;
            PrefabTypeFlag = prefabTypeFlag;
            FactionSpecific = factionSpecific;
            EnabledInEditor = enabledInEditor;
            RotatingVelocity = rotatingVelocity;
            StartRotatingCue = startRotatingCue;
            LoopRotatingCue = loopRotatingCue;
            LoopRotatingDamagedCue = loopRotatingDamagedCue;
            EndRotatingCue = endRotatingCue;
            PreviewPointOfView = new MyPreviewPointOfView(previewPointOfView);
            MinElectricCapacity = minElectricCapacity;
            MaxElectricCapacity = maxElectricCapacity;
            NeedsUpdate = needsUpdate;
            InitPhysics = initPhysics;
            ExplosionType = explosionType;
            ExplosionRadiusMultiplier = explosionRadiusMultiplier;
            ExplosionDamage = explosionDamageMultiplier;
            ExplosionParticleEffectScale = explosionParticleEffectScale;
            MinSizeForExplosion = minSizeForExplosion;
            CausesAlarm = causesAlarm;
            RequiresEnergy = requiresEnergy;
            DisplayHud = displayHud;
        }
    }

    public enum MyPreviewPointOfViewEnum
    {
        Front,
        Top,
        Bottom,
        FrontLeft,
        Left,
        BottomLeft,
        RearLeft,
        RearLeftLeft
    }

    public struct MyPreviewPointOfView
    {
        private readonly MyPreviewPointOfViewEnum m_pointOfView;

        private readonly Matrix m_transform;

        public Matrix Transform
        {
            get { return m_transform; }
        }

        public MyPreviewPointOfView(MyPreviewPointOfViewEnum pointOfView)
        {
            m_pointOfView = pointOfView;

            switch (pointOfView)
            {
                case MyPreviewPointOfViewEnum.Front:
                    m_transform = Matrix.Identity;
                    break;
                case MyPreviewPointOfViewEnum.Top:
                    m_transform = Matrix.CreateRotationX(MathHelper.PiOver2);
                    break;
                case MyPreviewPointOfViewEnum.Bottom:
                    m_transform = Matrix.CreateRotationX(MathHelper.PiOver2);
                    m_transform *= Matrix.CreateRotationY(MathHelper.Pi);
                    break;
                case MyPreviewPointOfViewEnum.FrontLeft:
                    m_transform = Matrix.CreateRotationX(MathHelper.ToRadians(10));
                    m_transform *= Matrix.CreateRotationY(MathHelper.ToRadians(15));
                    break;
                case MyPreviewPointOfViewEnum.Left:
                    m_transform = Matrix.CreateRotationY(MathHelper.ToRadians(45));
                    break;
                case MyPreviewPointOfViewEnum.BottomLeft:
                    m_transform = Matrix.CreateRotationX(MathHelper.PiOver2);
                    m_transform *= Matrix.CreateRotationY(MathHelper.PiOver4);
                    break;
                case MyPreviewPointOfViewEnum.RearLeft:
                    m_transform = Matrix.CreateRotationY(-MathHelper.PiOver2);
                    break;
                case MyPreviewPointOfViewEnum.RearLeftLeft:
                    m_transform = Matrix.CreateRotationY(MathHelper.ToRadians(67));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("pointOfView");
            }
        }

        public override string ToString()
        {
            return m_pointOfView.ToString();
        }
    }

}
