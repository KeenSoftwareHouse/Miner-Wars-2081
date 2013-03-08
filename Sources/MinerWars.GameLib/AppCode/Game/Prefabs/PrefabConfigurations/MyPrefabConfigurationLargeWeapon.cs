using MinerWars.AppCode.Game.Explosions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabConfigurationLargeWeapon : MyPrefabConfiguration
    {
        public MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum WeaponType { get; private set; }

        public MyPrefabConfigurationLargeWeapon(
            BuildTypesEnum buildType,
            CategoryTypesEnum categoryType,
            SubCategoryTypesEnum? subCategoryType,
            MyMaterialType materialType, 
            MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum weaponType,
            MyMwcObjectBuilder_Prefab_AppearanceEnum? factionSpecific = null,
            MyExplosionTypeEnum explosionType = MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION,
            float explosionRadiusMultiplier = 5,
            float explosionDamageMultiplier = 300)
            : base(buildType, categoryType, subCategoryType, materialType, PrefabTypesFlagEnum.Default, factionSpecific: factionSpecific, explosionType: explosionType, explosionRadiusMultiplier: explosionRadiusMultiplier, explosionDamageMultiplier: explosionDamageMultiplier, requiresEnergy: true, displayHud: true)
        {
            WeaponType = weaponType;
        }
    }
}
