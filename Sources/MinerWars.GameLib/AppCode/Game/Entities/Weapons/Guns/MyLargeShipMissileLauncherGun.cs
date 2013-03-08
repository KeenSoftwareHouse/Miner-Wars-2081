using System.Text;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.App;



namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
    using SysUtils.Utils;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

    class MyLargeShipMissileLauncherGun : MyLargeShipGunBase
    {
        public int Burst { get; private set; }

        public override void Init(StringBuilder hudLabelText, MyEntity parentObject, Vector3 position, Vector3 forwardVector, Vector3 upVector, MyMwcObjectBuilder_Base objectBuilder)
        {
            MyModelsEnum modelEnumBase;
            MyModelsEnum? modelEnumBaseCollision = null;
            MyModelsEnum modelEnumBarrel;
            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum missileType;

            switch ((MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum)objectBuilder.GetObjectBuilderId().Value)
            {
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4:
                    modelEnumBase = MyModelsEnum.LargeShipMissileLauncher4Base;
                    modelEnumBarrel = MyModelsEnum.LargeShipMissileLauncher4Barrel;
                    modelEnumBaseCollision = MyModelsEnum.LargeShipMissileLauncher4Base_COL;
                    Burst =  4;
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6:
                    modelEnumBase = MyModelsEnum.LargeShipMissileLauncher6Base;
                    modelEnumBarrel = MyModelsEnum.LargeShipMissileLauncher6Barrel;
                    modelEnumBaseCollision = MyModelsEnum.LargeShipMissileLauncher6Base_COL;
                    Burst = 6;
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9:
                    modelEnumBase = MyModelsEnum.LargeShipMissileLauncher9Base;
                    modelEnumBarrel = MyModelsEnum.LargeShipMissileLauncher9Barrel;
                    modelEnumBaseCollision = MyModelsEnum.LargeShipMissileLauncher9Base_COL;
                    Burst = 9;
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                    break;
            }

            switch ((MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum)objectBuilder.GetObjectBuilderId().Value)
            {
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9:
                        missileType = MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic;
                    break;
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6:
                case MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9:
                        missileType = MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection;
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                    break;
            }

            base.Init(hudLabelText, modelEnumBase, MyMaterialType.METAL, parentObject, position, forwardVector, upVector, objectBuilder, modelEnumBaseCollision);

            Matrix barrelMatrix = MyMath.NormalizeMatrix(ModelLod0.Dummies["axis"].Matrix);
            MyLargeShipMissileLauncherBarrel barrel = new MyLargeShipMissileLauncherBarrel();

            barrel.Init(hudLabelText, modelEnumBarrel, Burst, barrelMatrix, missileType, this);
            MountBarrel(barrel);

            // User settings:
            m_predictionIntervalConst_ms = 250;
            m_checkTargetIntervalConst_ms = 150;
            m_randomStandbyChangeConst_ms = 4000;
        }
    }
}
