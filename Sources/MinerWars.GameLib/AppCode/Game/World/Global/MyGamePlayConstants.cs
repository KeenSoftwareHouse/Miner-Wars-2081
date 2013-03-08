using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.LargeShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using SysUtils.Utils;
using MinerWarsMath;
using System.Diagnostics;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Gameplay
{
    struct MyGameplayDifficultyProfile
    {
        public MyGameplayDifficultyEnum GameplayDifficulty;

        //Bots
        //always 0 super easy 1 max hard
        // how much will bot strafe during combat  (not its speed)
        public float BotStrafingSpeed;
        //how much will bot strafe during combat  (not its speed)
        public float BotMovingSpeed;
        // affect speed of shooting (delays between all shooting)
        public float BotFireRatio;
        // what guns/slots will be used during fight / shoting
        public float BotGunUsageRatio;
        // how often will be raid attack executed
        public float BotRaidAttackOccurrence;
        // how often will be fly around exectued
        public float BotFlyAroundTargetOccurrence;
        //how far will bot react on enemy presence and from what distance he will shoot ( harder - more far )        
        public float BotAttackReactDistance;

        //Aiming
        //Deviating angle addition for player shooting on enemy
        public float DeviatingAnglePlayerOnEnemy;
        //Deviating angle addition for enemy bot shooting on player
        public float DeviatingAngleEnemyBotOnPlayer;
        //Deviating angle addition for enemy large weapon shooting on player
        public float DeviatingAngleEnemyLargeWeaponOnPlayer;
        //Enable aim correction
        public bool EnableAimCorrection;

        // Damage to player (weapons, sunwind) is multiplicated by this constant
        public float DamageToPlayerMultiplicator;

        // Damage to player from enemy is multiplicated by this constant
        public float DamageToPlayerFromEnemyMultiplicator;

        // Damage from player to enemy is multiplicated by this constant
        public float DamageFromPlayerToEnemyMultiplicator;

        // Player properties multiplicators
        public float PlayerFuelConsumptionMultiplicator;
        public float PlayerOxygenConsumptionMultiplicator;
        public float PlayerElectricityConsumptionMultiplicator;
        //Ship properties

        public MyTextsWrapperEnum DifficultyName;

        //Large weapons modifiers
        public float LargeWeaponMaxAttackingDistanceForPlayer;
    }

    public enum ItemCategory
    {
        DEFAULT,
        WEAPON,
        AMMO,
        DEVICE,
        CONSUMABLE,
        MEDICAL,
        ORE,
        GOOODS,
        ILLEGAL
    }

    public class MyGameplayProperties
    {
        public readonly float PricePerUnit;
        public readonly float WeightPerUnit;
        public readonly int MaxAmount;  //Max amount in one inventory slot
        public readonly int UsedSlots;  //Used slots in inventory        
        public ItemCategory ItemCategory;
        public readonly float MaxHealth;
        public readonly bool IsDestructible;     


        public MyGameplayProperties(float PricePerUnit, float WeightPerUnit, int MaxAmount, int UsedSlots, float MaxHealth, bool IsDestructible = true, ItemCategory itemCategory = ItemCategory.DEFAULT)
        {
            this.PricePerUnit = PricePerUnit;
            this.WeightPerUnit = WeightPerUnit;
            this.MaxAmount = MaxAmount;
            this.UsedSlots = UsedSlots;
            this.MaxHealth = MaxHealth;
            this.ItemCategory = itemCategory;
            this.IsDestructible = IsDestructible;
        }
    }

    static class MyGameplayConstants
    {
        public static readonly float MAXHEALTH_INDESTRUCTIBLE = 100000;
        public static readonly float MAXHEALTH_SMALLSHIP = 100;
        public static readonly float MAXHEALTH_DRONE = 30;
        public static readonly float MAXHEALTH_PREFAB_LIGHT = 2;
        public static readonly float MAXHEALTH_PREFAB = 350;
        public static readonly float MAXHEALTH_PREFAB_LARGE_WEAPON = 140;
        public static readonly float MAXHEALTH_PREFAB_TINY = 35;
        public static readonly float MAXHEALTH_PREFAB_SMALL = 750;
        public static readonly float MAXHEALTH_PREFAB_MEDIUM = 2000;
        public static readonly float MAXHEALTH_PREFAB_LARGE = 3500;
        public static readonly float MAXHEALTH_PREFAB_CHAMBER = 15000;
        public static readonly float MAXHEALTH_PREFAB_DOCK = 25000;
        public static readonly float MAXHEALTH_PREFAB_ARMOR = 10000;
        public static readonly float MAXHEALTH_PREFAB_DOORCASE = 4000;
        public static readonly float MAXHEALTH_PREFAB_DOORCASE_LARGE = 50000;

        public static readonly float MAXHEALTH_PREFAB_EXPLOSIVES = 20;

        public static readonly float MAXHEALTH_PANEL_SMALL = 350;
        public static readonly float MAXHEALTH_PANEL_MEDIUM = 750;
        public static readonly float MAXHEALTH_PANEL_LARGE = 2000;

        public static readonly float MAXHEALTH_TUNNEL_LONG = 5000;
        public static readonly float MAXHEALTH_TUNNEL_SHORT = 2500;
        public static readonly float MAXHEALTH_TUNNEL_VERYSHORT = 750;
                
        public static readonly float MAXHEALTH_PREFAB_BUILDING_SMALL = 1000;
        public static readonly float MAXHEALTH_PREFAB_BUILDING_MEDIUM = 3000;
        public static readonly float MAXHEALTH_PREFAB_BUILDING_LARGE = 15000;
        public static readonly float MAXHEALTH_PREFAB_BUILDING_HUGE = 25000;

        public static readonly float MAXHEALTH_DOOR = 200;
        public static readonly float MAXHEALTH_LARGESHIP = MAXHEALTH_SMALLSHIP * 50;


        public static readonly float HEALTH_BASIC = 100.0f;
        public static readonly float HEALTH_RATIO_DEATH = 0.0f;
        public static readonly float HEALTH_RATIO_MAX = 1.0f;
        public static readonly float MAX_HEALTH_MAX = 3.40282e+38f;
        public static readonly float NANO_REPAIR_TOOL_REPAIR_TO_HEALTH_MAX = 0.75f;
        public static readonly float NANO_REPAIR_TOOL_REPAIR_HEALTH_RATIO_PER_SEC = 0.004f;

        public static readonly float SHIP_WITHOUT_PILOT_DESTRUCTION_TIME = 120;             // seconds
        public static readonly float SHIP_DESTRUCTION_TIME_AFTER_LOOT = 10;                 // seconds

        public const float DEFAULT_MIN_ELECTRIC_CAPACITY = -10;
        public const float DEFAULT_MAX_ELECTRIC_CAPACITY = 5;

        public static readonly int AMMO_CLIP_CAPACITY_1_SMALL = 25;
        public static readonly int AMMO_CLIP_CAPACITY_2_MEDIUM = 30;
        public static readonly int AMMO_CLIP_CAPACITY_3_BIG = 100;
        public static readonly int AMMO_CLIP_CAPACITY_4_HUGE = 500;

        public static readonly int AMMO_SPECIAL_1_SINGLE = 3;
        public static readonly int AMMO_SPECIAL_2_FEW = 5;
        public static readonly int AMMO_SPECIAL_3_PACK = 24;

        public static readonly int AMMO_CANNON_1_BIG = 30;
        public static readonly int AMMO_CANNON_2_MEDIUM = 20;
        public static readonly int AMMO_CANNON_3_SMALL = 10;

        public static readonly int AMMO_ROCKET_3_BIG = 24;
        public static readonly int AMMO_ROCKET_2_MEDIUM = 12;
        public static readonly int AMMO_ROCKET_1_SMALL = 6;


        static readonly MyGameplayProperties[][] m_itemProperties = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1][];

        static readonly Dictionary<int, MyGameplayProperties[][]> m_itemPropertiesPerFaction = new Dictionary<int, MyGameplayProperties[][]>();

        static MyGameplayDifficultyProfile[] m_gameplayDifficultyProfiles = new MyGameplayDifficultyProfile[Enum.GetValues(typeof(MyGameplayDifficultyEnum)).Length];

        public static MyGameplayDifficultyProfile GameplayDifficultyProfile { get; private set; }
        private static MyGameplayDifficultyEnum m_difficulty;

        static MyGameplayConstants()
        {
            MyMwcLog.WriteLine("MyGameplayConstants()");

            m_gameplayDifficultyProfiles[(int)MyGameplayDifficultyEnum.EASY] = new MyGameplayDifficultyProfile()
            {
                GameplayDifficulty = MyGameplayDifficultyEnum.EASY,

                //Deviating angle addition for player shooting on enemy
                DeviatingAnglePlayerOnEnemy = MathHelper.ToRadians(0.0f),
                //Deviating angle addition for enemy bot shooting on player
                DeviatingAngleEnemyBotOnPlayer = MathHelper.ToRadians(2.5f),
                //Deviating angle addition for enemy large weapon shooting on player
                DeviatingAngleEnemyLargeWeaponOnPlayer = MathHelper.ToRadians(2.5f),
                //Enable aim correction
                EnableAimCorrection = true,

                //Bot settings
                BotStrafingSpeed = 0.0f,
                BotMovingSpeed = 0.0f,
                BotFireRatio = 0.0f,
                BotGunUsageRatio = 0.0f,
                BotRaidAttackOccurrence = 0.0f,
                BotFlyAroundTargetOccurrence = 0.0f,
                BotAttackReactDistance = 0.0f,

                PlayerFuelConsumptionMultiplicator = 1.0f,
                PlayerOxygenConsumptionMultiplicator = 1.0f,
                PlayerElectricityConsumptionMultiplicator = 1.0f,

                DamageToPlayerMultiplicator = 1.0f,
                DamageToPlayerFromEnemyMultiplicator = 0.2f,
                DamageFromPlayerToEnemyMultiplicator = 3.0f,

                LargeWeaponMaxAttackingDistanceForPlayer = MyLargeShipWeaponsConstants.MAX_HUD_DISTANCE * 0.9f,

                DifficultyName = MyTextsWrapperEnum.DifficultyEasy,
            };

            m_gameplayDifficultyProfiles[(int)MyGameplayDifficultyEnum.NORMAL] = new MyGameplayDifficultyProfile()
            {
                GameplayDifficulty = MyGameplayDifficultyEnum.NORMAL,

                //Deviating angle addition for player shooting on enemy
                DeviatingAnglePlayerOnEnemy = MathHelper.ToRadians(0.2f),
                //Deviating angle addition for enemy bot shooting on player
                DeviatingAngleEnemyBotOnPlayer = MathHelper.ToRadians(0.1f),
                //Deviating angle addition for enemy large weapon shooting on player
                DeviatingAngleEnemyLargeWeaponOnPlayer = MathHelper.ToRadians(0.1f),
                //Enable aim correction
                EnableAimCorrection = true,

                //Bot settings
                BotStrafingSpeed = 0.5f,
                BotMovingSpeed = 0.5f,
                BotFireRatio = 0.5f,
                BotGunUsageRatio = 0.5f,
                BotRaidAttackOccurrence = 0.5f,
                BotFlyAroundTargetOccurrence = 0.5f,
                BotAttackReactDistance = 0.5f,

                
                PlayerFuelConsumptionMultiplicator = 1.1f,
                PlayerOxygenConsumptionMultiplicator = 1.0f,
                PlayerElectricityConsumptionMultiplicator = 1.1f,

                DamageToPlayerMultiplicator = 1.5f,
                DamageToPlayerFromEnemyMultiplicator = 0.35f,
                DamageFromPlayerToEnemyMultiplicator = 1.0f,

                LargeWeaponMaxAttackingDistanceForPlayer = MyLargeShipWeaponsConstants.MAX_HUD_DISTANCE * 0.9f,

                DifficultyName = MyTextsWrapperEnum.DifficultyNormal,
            };

            m_gameplayDifficultyProfiles[(int)MyGameplayDifficultyEnum.HARD] = new MyGameplayDifficultyProfile()
            {
                GameplayDifficulty = MyGameplayDifficultyEnum.HARD,

                //Deviating angle addition for player shooting on enemy
                DeviatingAnglePlayerOnEnemy = MathHelper.ToRadians(1.0f),
                //Deviating angle addition for enemy bot shooting on player
                DeviatingAngleEnemyBotOnPlayer = MathHelper.ToRadians(0.0f),
                //Deviating angle addition for enemy large weapon shooting on player
                DeviatingAngleEnemyLargeWeaponOnPlayer = MathHelper.ToRadians(0.0f),
                //Enable aim correction
                EnableAimCorrection = false,

                //Bot settings
                BotStrafingSpeed = 1.0f,
                BotMovingSpeed = 1.0f,
                BotFireRatio = 1.0f,
                BotGunUsageRatio = 1.0f,
                BotRaidAttackOccurrence = 1.0f,
                BotFlyAroundTargetOccurrence = 1.0f,
                BotAttackReactDistance = 1.0f,

                
                PlayerFuelConsumptionMultiplicator = 1.5f,
                PlayerOxygenConsumptionMultiplicator = 1.0f,
                PlayerElectricityConsumptionMultiplicator = 1.5f,

                DamageToPlayerMultiplicator = 1.5f,
                DamageToPlayerFromEnemyMultiplicator = 1.0f,
                DamageFromPlayerToEnemyMultiplicator = 0.8f,

                LargeWeaponMaxAttackingDistanceForPlayer = MyLargeShipWeaponsConstants.MAX_SEARCHING_DISTANCE,

                DifficultyName = MyTextsWrapperEnum.DifficultyHard,
            };


            //Default value
            GameplayDifficultyProfile = m_gameplayDifficultyProfiles[(int)MyGameplayDifficultyEnum.NORMAL];

            #region small debris
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallDebris_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Cistern] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.UtilityVehicle_1] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.pipe_bundle] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false 
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris1] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris2] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris3] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris4] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris5] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris6] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris7] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris8] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris9] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris10] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris11] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris12] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris13] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris14] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris15] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris16] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris17] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris18] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris19] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris20] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris21] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris22] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris23] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris24] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris25] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris26] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris27] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris28] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris29] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris30] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris31] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris32_pilot] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_1] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_3] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallDebris][(int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_4] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 1,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY
                   );
            #endregion

            #region smallShip tools
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Tool_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REAR_CAMERA] =
                new MyGameplayProperties(
                    PricePerUnit: 1000,
                    WeightPerUnit: 30,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.LASER_POINTER] =
                new MyGameplayProperties(
                    PricePerUnit: 2000,
                    WeightPerUnit: 5,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.AUTO_TARGETING] =
                new MyGameplayProperties(
                    PricePerUnit: 10000,
                    WeightPerUnit: 3,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NIGHT_VISION] =
                new MyGameplayProperties(
                    PricePerUnit: 8000,
                    WeightPerUnit: 10,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL] =
                new MyGameplayProperties(
                    PricePerUnit: 100000,
                    WeightPerUnit: 20,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.MEDIKIT] =
                new MyGameplayProperties(
                    PricePerUnit: 60,
                    WeightPerUnit: 0.15f,
                    MaxAmount: 100,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.CONSUMABLE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.XRAY] =
                new MyGameplayProperties(
                    PricePerUnit: 300000,
                    WeightPerUnit: 30,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            /*
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL] =
                new MyGameplayProperties(
                    PricePerUnit: 300000,
                    WeightPerUnit: 30,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false 
                   );
            */
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ANTIRADIATION_MEDICINE] =
                new MyGameplayProperties(
                    PricePerUnit: 2000,
                    WeightPerUnit: 5,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.MEDICAL
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER] =
                new MyGameplayProperties(
                    PricePerUnit: 150000,
                    WeightPerUnit: 10,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.PERFORMANCE_ENHANCING_MEDICINE] =
                new MyGameplayProperties(
                    PricePerUnit: 10000,
                    WeightPerUnit: 5,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.MEDICAL
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_ENHANCING_MEDICINE] =
                new MyGameplayProperties(
                    PricePerUnit: 2000,
                    WeightPerUnit: 5,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.MEDICAL
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_FUEL_CONTAINER_DISABLED] =
                new MyGameplayProperties(
                    PricePerUnit: 10000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_ELECTRICITY_CONTAINER] =
                new MyGameplayProperties(
                    PricePerUnit: 15000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_OXYGEN_CONTAINER_DISABLED] =
                new MyGameplayProperties(
                    PricePerUnit: 8000,
                    WeightPerUnit: 90,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_CONVERTER] =
                new MyGameplayProperties(
                    PricePerUnit: 56000,
                    WeightPerUnit: 230,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_CONVERTER] =
                new MyGameplayProperties(
                    PricePerUnit: 130000,
                    WeightPerUnit: 670,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.SOLAR_PANEL] =
                new MyGameplayProperties(
                    PricePerUnit: 34000,
                    WeightPerUnit: 80,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.BOOBY_TRAP] =
                new MyGameplayProperties(
                    PricePerUnit: 10000,
                    WeightPerUnit: 100,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.SENSOR] =
                new MyGameplayProperties(
                    PricePerUnit: 20000,
                    WeightPerUnit: 30,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REMOTE_CAMERA] =
                new MyGameplayProperties(
                    PricePerUnit: 20000,
                    WeightPerUnit: 50,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REMOTE_CAMERA_ON_DRONE] =
                new MyGameplayProperties(
                    PricePerUnit: 30000,
                    WeightPerUnit: 120,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ALIEN_OBJECT_DETECTOR] =
                new MyGameplayProperties(
                    PricePerUnit: 0,
                    WeightPerUnit: 2000,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_UNUSED] = new MyGameplayProperties(
                    PricePerUnit: 5000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_KIT] =
                new MyGameplayProperties(
                    PricePerUnit: 5000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.MEDICAL
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REPAIR_KIT] =
                new MyGameplayProperties(
                    PricePerUnit: 5000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_KIT] =
                new MyGameplayProperties(
                    PricePerUnit: 5000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_KIT] =
                new MyGameplayProperties(
                    PricePerUnit: 5000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool][(int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ELECTRICITY_KIT] =
                new MyGameplayProperties(
                    PricePerUnit: 5000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.GOOODS
                   );
            #endregion

            #region smallship armors
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Armor] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Armor_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Armor][(int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic] =
                new MyGameplayProperties(
                    PricePerUnit: 3000,
                    WeightPerUnit: 1000,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Armor][(int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Advanced] =
                new MyGameplayProperties(
                    PricePerUnit: 5000,
                    WeightPerUnit: 2000,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Armor][(int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.High_Endurance] =
                new MyGameplayProperties(
                    PricePerUnit: 8000,
                    WeightPerUnit: 3000,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Armor][(int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Solar_Wind] =
                new MyGameplayProperties(
                    PricePerUnit: 90000,
                    WeightPerUnit: 2500,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            #endregion

            #region smallship radars
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Radar] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Radar_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Radar][(int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1] =
             new MyGameplayProperties(
                 PricePerUnit: 190000,
                 WeightPerUnit: 1,
                 MaxAmount: 1,
                 UsedSlots: 1,
                 MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false,
                 itemCategory: ItemCategory.DEVICE
                );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Radar][(int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_2] =
             new MyGameplayProperties(
                 PricePerUnit: 190000,
                 WeightPerUnit: 1,
                 MaxAmount: 1,
                 UsedSlots: 1,
                 MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false,
                 itemCategory: ItemCategory.DEVICE
                );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Radar][(int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_3] =
             new MyGameplayProperties(
                 PricePerUnit: 190000,
                 WeightPerUnit: 1,
                 MaxAmount: 1,
                 UsedSlots: 1,
                 MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false,
                 itemCategory: ItemCategory.DEVICE
                );
            #endregion

            #region smallship weapons
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer] =
                new MyGameplayProperties(
                    PricePerUnit: 20000,
                    WeightPerUnit: 13,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper] =
                new MyGameplayProperties(
                    PricePerUnit: 70000,
                    WeightPerUnit: 8,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon] =
                new MyGameplayProperties(
                    PricePerUnit: 15000,
                    WeightPerUnit: 20,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun] =
                new MyGameplayProperties(
                    PricePerUnit: 10000,
                    WeightPerUnit: 13,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun] =
                new MyGameplayProperties(
                    PricePerUnit: 20000,
                    WeightPerUnit: 5,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon] =
                new MyGameplayProperties(
                    PricePerUnit: 25000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher] =
                new MyGameplayProperties(
                    PricePerUnit: 40000,
                    WeightPerUnit: 13,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front] =
                new MyGameplayProperties(
                    PricePerUnit: 30000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back] =
                new MyGameplayProperties(
                    PricePerUnit: 30000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.WEAPON
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher] =
                new MyGameplayProperties(
                    PricePerUnit: 15000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser] =
                new MyGameplayProperties(
                    PricePerUnit: 30000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear] =
                new MyGameplayProperties(
                    PricePerUnit: 25000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure] =
                new MyGameplayProperties(
                    PricePerUnit: 35000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw] =
                new MyGameplayProperties(
                    PricePerUnit: 20000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal] =
                new MyGameplayProperties(
                    PricePerUnit: 20000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device] =
                new MyGameplayProperties(
                    PricePerUnit: 20000,
                    WeightPerUnit: 40,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            #endregion

            #region smallship ammo
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed] =
                new MyGameplayProperties(
                    PricePerUnit: 1 * 5,
                    WeightPerUnit: 0.05f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_SAPHEI] =
                new MyGameplayProperties(
                    PricePerUnit: 1.8f * 5,
                    WeightPerUnit: 0.08f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem] =
                new MyGameplayProperties(
                    PricePerUnit: 1.5f * 5,
                    WeightPerUnit: 0.08f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed] =
                new MyGameplayProperties(
                    PricePerUnit: 3 * 15,
                    WeightPerUnit: 0.008f,
                    MaxAmount: AMMO_CLIP_CAPACITY_2_MEDIUM,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI] =
                new MyGameplayProperties(
                    PricePerUnit: 3.5f * 15,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_CLIP_CAPACITY_1_SMALL,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem] =
                new MyGameplayProperties(
                    PricePerUnit: 4 * 15,
                    WeightPerUnit: 0.15f,
                    MaxAmount: AMMO_CLIP_CAPACITY_1_SMALL,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic] =
                new MyGameplayProperties(
                    PricePerUnit: 0.5f * 5,
                    WeightPerUnit: 0.006f,
                    MaxAmount: AMMO_CLIP_CAPACITY_4_HUGE,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed] =
                new MyGameplayProperties(
                    PricePerUnit: 0.75f * 5,
                    WeightPerUnit: 0.008f,
                    MaxAmount: AMMO_CLIP_CAPACITY_4_HUGE,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Armor_Piercing_Incendiary] =
                new MyGameplayProperties(
                    PricePerUnit: 1 * 5,
                    WeightPerUnit: 0.01f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI] =
                new MyGameplayProperties(
                    PricePerUnit: 1.5f * 5,
                    WeightPerUnit: 0.012f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem] =
                new MyGameplayProperties(
                    PricePerUnit: 3 * 5,
                    WeightPerUnit: 0.014f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic] =
                new MyGameplayProperties(
                    PricePerUnit: 0.5f * 5,
                    WeightPerUnit: 0.06f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed] =
                new MyGameplayProperties(
                    PricePerUnit: 0.75f * 5,
                    WeightPerUnit: 0.08f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Armor_Piercing_Incendiary] =
                new MyGameplayProperties(
                    PricePerUnit: 1 * 5,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI] =
                new MyGameplayProperties(
                    PricePerUnit: 1.5f * 5,
                    WeightPerUnit: 0.12f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem] =
                new MyGameplayProperties(
                    PricePerUnit: 3 * 5,
                    WeightPerUnit: 0.14f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic] =
                new MyGameplayProperties(
                    PricePerUnit: 30f,
                    WeightPerUnit: 0.05f,
                    MaxAmount: AMMO_CLIP_CAPACITY_2_MEDIUM,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed] =
                new MyGameplayProperties(
                    PricePerUnit: 45f,
                    WeightPerUnit: 0.06f,
                    MaxAmount: AMMO_CLIP_CAPACITY_2_MEDIUM,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive] =
                new MyGameplayProperties(
                    PricePerUnit: 70f,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_CLIP_CAPACITY_2_MEDIUM,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing] =
                new MyGameplayProperties(
                    PricePerUnit: 60f,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_CLIP_CAPACITY_2_MEDIUM,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic] =
                new MyGameplayProperties(
                    PricePerUnit: 50,
                    WeightPerUnit: 0.07f,
                    MaxAmount: AMMO_CANNON_1_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed] =
                new MyGameplayProperties(
                    PricePerUnit: 80,
                    WeightPerUnit: 0.09f,
                    MaxAmount: AMMO_CANNON_1_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Armor_Piercing_Incendiary] =
                new MyGameplayProperties(
                    PricePerUnit: 100,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_CANNON_3_SMALL,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI] =
                new MyGameplayProperties(
                    PricePerUnit: 360,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_CANNON_2_MEDIUM,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive] =
                new MyGameplayProperties(
                    PricePerUnit: 300,
                    WeightPerUnit: 0.15f,
                    MaxAmount: AMMO_CANNON_2_MEDIUM,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster] =
                new MyGameplayProperties(
                    PricePerUnit: 300,
                    WeightPerUnit: 0.2f,
                    MaxAmount: AMMO_CANNON_2_MEDIUM,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection] =
                new MyGameplayProperties(
                    PricePerUnit: 500,
                    WeightPerUnit: 0.2f,
                    MaxAmount: AMMO_ROCKET_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection] =
                new MyGameplayProperties(
                    PricePerUnit: 200,
                    WeightPerUnit: 0.2f,
                    MaxAmount: AMMO_ROCKET_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection] =
                new MyGameplayProperties(
                    PricePerUnit: 200,
                    WeightPerUnit: 0.2f,
                    MaxAmount: AMMO_ROCKET_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic] =
                new MyGameplayProperties(
                    PricePerUnit: 50,
                    WeightPerUnit: 0.2f,
                    MaxAmount: AMMO_ROCKET_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive] =
                new MyGameplayProperties(
                    PricePerUnit: 100,
                    WeightPerUnit: 0.3f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive] =
                new MyGameplayProperties(
                    PricePerUnit: 100,
                    WeightPerUnit: 0.3f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb] =
                new MyGameplayProperties(
                    PricePerUnit: 120,
                    WeightPerUnit: 0.3f,
                    MaxAmount: AMMO_SPECIAL_1_SINGLE,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb] =
                new MyGameplayProperties(
                    PricePerUnit: 130,
                    WeightPerUnit: 0.3f,
                    MaxAmount: AMMO_SPECIAL_1_SINGLE,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer] =
                new MyGameplayProperties(
                    PricePerUnit: 250,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_1_SINGLE,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart] =
                new MyGameplayProperties(
                    PricePerUnit: 300,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_3_PACK,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic] =
                new MyGameplayProperties(
                    PricePerUnit: 100,
                    WeightPerUnit: 0.12f,
                    MaxAmount: AMMO_SPECIAL_3_PACK,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb] =
                new MyGameplayProperties(
                    PricePerUnit: 1000,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram] =
                new MyGameplayProperties(
                    PricePerUnit: 500,
                    WeightPerUnit: 0.3f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare] =
                new MyGameplayProperties(
                    PricePerUnit: 1000,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_3_PACK,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb] =
                new MyGameplayProperties(
                    PricePerUnit: 500,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell] =
                new MyGameplayProperties(
                    PricePerUnit: 100,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_3_PACK,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb] =
                new MyGameplayProperties(
                    PricePerUnit: 200,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera] =
                new MyGameplayProperties(
                    PricePerUnit: 1000,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem] =
                new MyGameplayProperties(
                    PricePerUnit: 300,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem] =
                new MyGameplayProperties(
                    PricePerUnit: 250,
                    WeightPerUnit: 0.2f,
                    MaxAmount: AMMO_ROCKET_1_SMALL,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem] =
                new MyGameplayProperties(
                    PricePerUnit: 250,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_CANNON_3_SMALL,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP] =
                new MyGameplayProperties(
                    PricePerUnit: 14,
                    WeightPerUnit: 0.014f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP] =
                new MyGameplayProperties(
                    PricePerUnit: 30,
                    WeightPerUnit: 0.008f,
                    MaxAmount: AMMO_CLIP_CAPACITY_1_SMALL,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP] =
                new MyGameplayProperties(
                    PricePerUnit: 12.0f,
                    WeightPerUnit: 0.06f,
                    MaxAmount: AMMO_CLIP_CAPACITY_3_BIG,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP] =
                new MyGameplayProperties(
                    PricePerUnit: 800,
                    WeightPerUnit: 0.2f,
                    MaxAmount: AMMO_ROCKET_1_SMALL,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP] =
                new MyGameplayProperties(
                    PricePerUnit: 500,
                    WeightPerUnit: 0.1f,
                    MaxAmount: AMMO_CANNON_3_SMALL,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb] =
                new MyGameplayProperties(
                    PricePerUnit: 2000,
                    WeightPerUnit: 0.3f,
                    MaxAmount: AMMO_SPECIAL_2_FEW,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.AMMO
                   );
            #endregion

            #region smallship engines
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Engine_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1] =
                new MyGameplayProperties(
                    PricePerUnit: 15000,
                    WeightPerUnit: 150,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_2] =
                new MyGameplayProperties(
                    PricePerUnit: 21000,
                    WeightPerUnit: 210,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_3] =
                new MyGameplayProperties(
                    PricePerUnit: 30000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_4] =
                new MyGameplayProperties(
                    PricePerUnit: 32000,
                    WeightPerUnit: 320,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_5] =
                new MyGameplayProperties(
                    PricePerUnit: 40000,
                    WeightPerUnit: 400,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_1] =
                new MyGameplayProperties(
                    PricePerUnit: 30000,
                    WeightPerUnit: 3000,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_2] =
                new MyGameplayProperties(
                    PricePerUnit: 35000,
                    WeightPerUnit: 350,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_3] =
                new MyGameplayProperties(
                    PricePerUnit: 58000,
                    WeightPerUnit: 580,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_4] =
                new MyGameplayProperties(
                    PricePerUnit: 70000,
                    WeightPerUnit: 700,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_5] =
                new MyGameplayProperties(
                    PricePerUnit: 88000,
                    WeightPerUnit: 880,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1] =
                new MyGameplayProperties(
                    PricePerUnit: 20000,
                    WeightPerUnit: 200,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_2] =
                new MyGameplayProperties(
                    PricePerUnit: 25000,
                    WeightPerUnit: 250,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_3] =
                new MyGameplayProperties(
                    PricePerUnit: 28000,
                    WeightPerUnit: 280,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_4] =
                new MyGameplayProperties(
                    PricePerUnit: 30000,
                    WeightPerUnit: 300,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Engine][(int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_5] =
                new MyGameplayProperties(
                    PricePerUnit: 38000,
                    WeightPerUnit: 380,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.DEVICE
                   );
            #endregion

            #region ore
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Ore_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.IRON] =
                new MyGameplayProperties(
                    PricePerUnit: 1250,
                    WeightPerUnit: 0.078f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.URANITE] =
                new MyGameplayProperties(
                    PricePerUnit: 13333,
                    WeightPerUnit: 0.195f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.HELIUM] =
                new MyGameplayProperties(
                    PricePerUnit: 5000,
                    WeightPerUnit: 0.1f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.ICE] =
                new MyGameplayProperties(
                    PricePerUnit: 25,
                    WeightPerUnit: 0.1f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.GOLD] =
                new MyGameplayProperties(
                    PricePerUnit: 6667,
                    WeightPerUnit: 0.3f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.SILVER] =
                new MyGameplayProperties(
                    PricePerUnit: 4667,
                    WeightPerUnit: 0.2f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.COBALT] =
                new MyGameplayProperties(
                    PricePerUnit: 750,
                    WeightPerUnit: 0.65f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.SILICON] =
                new MyGameplayProperties(
                    PricePerUnit: 125,
                    WeightPerUnit: 0.4f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.PLATINUM] =
                new MyGameplayProperties(
                    PricePerUnit: 5667,
                    WeightPerUnit: 0.3f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.NICKEL] =
                new MyGameplayProperties(
                    PricePerUnit: 1000,
                    WeightPerUnit: 0.6f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.COBALT] =
                new MyGameplayProperties(
                    PricePerUnit: 750,
                    WeightPerUnit: 0.65f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.MAGNESIUM] =
                new MyGameplayProperties(
                    PricePerUnit: 1000,
                    WeightPerUnit: 0.15f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.TREASURE] =
                new MyGameplayProperties(
                    PricePerUnit: 200000,
                    WeightPerUnit: 0.1f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.ORGANIC] =
                new MyGameplayProperties(
                    PricePerUnit: 66667,
                    WeightPerUnit: 0.1f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.XENON] =
                new MyGameplayProperties(
                    PricePerUnit: 1000000,
                    WeightPerUnit: 0.1f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.SNOW] =
                new MyGameplayProperties(
                    PricePerUnit: 14,
                    WeightPerUnit: 0.1f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.LAVA] =
                new MyGameplayProperties(
                    PricePerUnit: 333,
                    WeightPerUnit: 0.1f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.STONE] =
                new MyGameplayProperties(
                    PricePerUnit: 5,
                    WeightPerUnit: 0.1f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.SANDSTONE] =
                new MyGameplayProperties(
                    PricePerUnit: 13,
                    WeightPerUnit: 0.65f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Ore][(int)MyMwcObjectBuilder_Ore_TypesEnum.CONCRETE] =
                new MyGameplayProperties(
                    PricePerUnit: 13,
                    WeightPerUnit: 0.65f,
                    MaxAmount: 1,
                    UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false ,
                    itemCategory: ItemCategory.ORE
                    );
            #endregion

            #region blueprints
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Blueprint_TypesEnum>() + 1];
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P430_A01_PASSAGE_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P430_A02_PASSAGE_40M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P424_A01_PIPE_BASE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P423_A01_PIPE_JUNCTION] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P422_A01_PIPE_TURN_90] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P421_A01_PIPE_STRAIGHT_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P421_A02_PIPE_STRAIGHT_40M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P421_A03_PIPE_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_G01_JUNCTION_6AXES] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_G02_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P410_G01_TURN_90_RIGHT_0M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G01_STRAIGHT_1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G02_STRAIGHT_2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G03_STRAIGHT_3] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G04_STRAIGHT_4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_F02_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F21_TURN_S_UP] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F22_TURN_S_LEFT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F23_TURN_S_RIGHT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F24_TURN_S_DOWN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F01_TURN_90_UP_230M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F02_TURN_90_LEFT_230M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F03_TURN_90_RIGHT_230M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F04_TURN_90_DOWN_230M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F01_STRAIGHT_1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F02_STRAIGHT_2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F03_STRAIGHT_3] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_E01_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E01_STRAIGHT_1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E02_STRAIGHT_2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E03_STRAIGHT_3] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E04_STRAIGHT_4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E05_STRAIGHT_5] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_D01_DOORCASE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_D02_DOOR1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_D03_DOOR2_A] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_D03_DOOR2_B] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_D01_JUNCTION_T_HORIZONTAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_D03_JUNCTION_X_HORIZONTAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_D01_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D02_STRAIGHT_40M_WITH_HOLE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D03_STRAIGHT_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D04_STRAIGHT_120M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D05_STRAIGHT_180M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C01_DOOR1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C01_DOOR2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C01_DOOR3] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C01_DOOR4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_C01_JUNCTION_T_HORIZONTAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_C01_JUNCTION_X_HORIZONTAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_C01_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C02_STRAIGHT_40M_WITH_HOLE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C03_STRAIGHT_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C04_STRAIGHT_120M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C05_STRAIGHT_180M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_B01_DOORCASE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_B02_DOOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_B01_JUNCTION_T_HORIZONTAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_B02_JUNCTION_T_VERTICAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_B02_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B21_TURN_S_UP] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B22_TURN_S_LEFT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B23_TURN_S_RIGHT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B24_TURN_S_DOWN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B11_TURN_90_UP_160M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B12_TURN_90_LEFT_160M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B13_TURN_90_RIGHT_160M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B14_TURN_90_DOWN_160M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B01_TURN_90_UP_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B02_TURN_90_LEFT_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B03_TURN_90_RIGHT_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B04_TURN_90_DOWN_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_B01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );


            //  m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B02_STRAIGHT_30M_YELLOW] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            // m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B03_STRAIGHT_320M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            // m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B04_STRAIGHT_80M_WITH_SIDE_GRATES] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            // m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B05_STRAIGHT_80M_WITH_SIDE_OPEN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            // m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B06_STRAIGHT_180M_CONCRETE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            // m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B06_STRAIGHT_200M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            // m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B07_STRAIGHT_180M_BLUE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            // m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B09_STRAIGHT_30M_GRAY] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B11_STRAIGHT_220M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B12_STRAIGHT_160M_DARK_METAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B13_STRAIGHT_100M_TUBE_INSIDE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_A01_DOORCASE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_A02_DOOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_A01_JUNCTION_T_HORIZONTAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_A02_JUNCTION_T_VERTICAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_A01_ENTRANCE_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_A02_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A21_TURN_S_UP] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A22_TURN_S_LEFT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A23_TURN_S_RIGHT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A24_TURN_S_DOWN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A11_TURN_90_UP_160M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A12_TURN_90_LEFT_160M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A13_TURN_90_RIGHT_160M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A14_TURN_90_DOWN_160M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A01_TURN_90_UP_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A02_TURN_90_LEFT_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A03_TURN_90_RIGHT_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A04_TURN_90_DOWN_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A02_STRAIGHT_60M_WITH_HOLE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A03_STRAIGHT_120M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A04_STRAIGHT_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A05_STRAIGHT_80M_WITH_EXTENSION] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_E01_BRIDGE5] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_D01_BRIDGE4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_C01_BRIDGE3] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_B01_BRIDGE2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_A01_BRIDGE1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P381_C01_BUILDING3] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P381_B01_BUILDING2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P381_A01_BUILDING1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P361_A01_SMALL_HANGAR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P362_A01_SHORT_DISTANCE_ANTENNA] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P361_A01_LONG_DISTANCE_ANTENNA] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P351_A01_WEAPON_MOUNT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P345_A01_REFINERY] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P344_A01_CONTAINER_ARM_FILLED] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P344_A02_CONTAINER_ARM_EMPTY] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P343_A01_ORE_STORAGE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P342_A01_LOADING_BAY] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_B01_OPEN_DOCK_VARIATION1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_B02_OPEN_DOCK_VARIATION2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_A02_OPEN_DOCK_VARIATION2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P333_A01_HYDROPONIC_BUILDING] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P332_A01_OXYGEN_STORAGE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P331_A01_OXYGEN_GENERATOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P324B01_FUEL_STORAGE_B] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P324A01_FUEL_STORAGE_A] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P323A01_FUEL_GENERATOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P322A01_BATTERY] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321C01_INERTIA_GENERATOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321B01_NUCLEAR_REACTOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321A01_SOLAR_PANEL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P312A01_SHORT_TERM_THRUSTER_LATITUDE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P312A02_SHORT_TERM_THRUSTER_LATERAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P311A01_LONG_TERM_THRUSTER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A01_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A02_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A03_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A04_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A05_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A06_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A07_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A08_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A09_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A10_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A11_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A12_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A13_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A14_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A15_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A16_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A17_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A18_ARMOR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221E01_CHAMBER_V1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221D01_CHAMBER_V1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221C01_CHAMBER_V1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221B01_CHAMBER_V1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221A01_CHAMBER_V1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211G01_PANEL_120MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211G02_PANEL_60MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211G03_PANEL_60MX30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211F01_PANEL_120MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211F02_PANEL_60MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211F03_PANEL_60MX30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211E01_PANEL_120MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211E02_PANEL_60MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211E03_PANEL_60MX30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211D01_PANEL_120MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211D02_PANEL_60MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211D03_PANEL_60MX30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211C01_PANEL_120MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211C02_PANEL_60MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211C03_PANEL_60MX30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211B01_PANEL_120MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211B02_PANEL_60MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211B03_PANEL_60MX30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211A01_PANEL_120MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211A02_PANEL_60MX60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211A03_PANEL_60MX30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B01_CAGE_EMPTY] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B02_CAGE_HALFCUT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B03_CAGE_WITH_CORNERS] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B11_CAGE_PILLAR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B12_CAGE_EDGE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A01_CAGE_EMPTY] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A02_CAGE_HALFCUT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A03_CAGE_WITH_CORNERS] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A11_CAGE_PILLAR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A12_CAGE_EDGE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B01_THICK_FRAME_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B02_THICK_FRAME_STRAIGHT_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B11_THICK_FRAME_EDGE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B12_THICK_FRAME_CORNER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B31_THICK_FRAME_JOINT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A01_THICK_FRAME_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A02_THICK_FRAME_STRAIGHT_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A11_THICK_FRAME_EDGE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A12_THICK_FRAME_CORNER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A31_THICK_FRAME_JOINT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120J01_J_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130J02_J_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120I01_I_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130I02_I_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120H01_H_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130H02_H_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120G01_G_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130G02_G_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120F01_F_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130F02_F_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120E01_E_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130E02_E_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120D01_D_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130D02_D_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120C01_C_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130C02_C_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120B01_B_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130B02_B_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A01_A_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130A02_A_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120D01_D_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120D02_D_STRAIGHT_40M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120C01_C_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120C02_C_STRAIGHT_40M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120B01_B_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120B02_B_STRAIGHT_40M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A01_STRONG_LATTICE_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A02_STRONG_LATTICE_STRAIGHT_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A03_STRONG_LATTICE_STRAIGHT_120M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A21_STRONG_LATTICE_JUNCTION_T_STRONG] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A22_STRONG_LATTICE_JUNCTION_T_WEAK] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A23_STRONG_LATTICE_JUNCTION_T_ROTATED] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A51_STRONG_TO_WEAK_LATTICE_2TO1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A52_STRONG_TO_WEAK_LATTICE_1TO2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A61_WEAK_LATTICE_JUNCTION_T_ROTATED] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B01_LATTICE_BEAM_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B02_LATTICE_BEAM_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_60M_WITH_PANELS] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B21_LATTICE_BEAM_JUNCTION_T_STRONG] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B22_LATTICE_BEAM_JUNCTION_T_WEAK] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B31_LATTICE_BEAM_JOINT_HORIZONTAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B32_LATTICE_BEAM_JOINT_VERTICAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A01_SOLID_BEAM_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A02_SOLID_BEAM_STRAIGHT_20M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A03_SOLID_BEAM_STRAIGHT_40M_WITH_HOLE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A04_SOLID_BEAM_STRAIGHT_40M_LATTICE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A05_SOLID_BEAM_STRAIGHT_80M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A11_SOLID_BEAM_JUNCTION_X_STRONG] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A12_SOLID_BEAM_JUNCTION_X_WEAK] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A13_SOLID_BEAM_JUNCTION_X_ROTATED] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A21_SOLID_BEAM_JUNCTION_T_STRONG] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A22_SOLID_BEAM_JUNCTION_T_WEAK] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A23_SOLID_BEAM_JUNCTION_T_ROTATED] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A31_SOLID_BEAM_JOINT_HORIZONTAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A32_SOLID_BEAM_JOINT_VERTICAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A33_SOLID_BEAM_JOINT_LONGITUDINAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A41_SOLID_BEAM_SUPERJOINT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.DEFAULT_LIGHT_0] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P521_A01_LIGHT1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P521_A02_LIGHT2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P521_A03_LIGHT3] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P521_A04_LIGHT4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.DEFAULT_PARTICLE_PREFAB_0] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P551_A01_PARTICLES] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P551_B01_PARTICLES] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P551_C01_PARTICLES] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P551_D01_PARTICLES] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.DEFAULT_SOUND_PREFAB_0] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P561_A01_SOUND] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P561_B01_SOUND] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P561_C01_SOUND] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P561_D01_SOUND] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_CIWS] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A01_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A02_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A03_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A04_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A05_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A06_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A07_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A08_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A09_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A10_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A11_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A12_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A13_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A14_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A15_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A16_BILLBOARD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A01_SIGN1] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A02_SIGN2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A03_SIGN3] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A04_SIGN4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A05_SIGN5] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A06_SIGN6] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A07_SIGN7] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A08_SIGN8] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A09_SIGN9] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A10_SIGN10] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A11_SIGN11] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A12_SIGN12] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_A02_CHAMBER_V2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_B02_CHAMBER_V2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_C02_CHAMBER_V2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_D02_CHAMBER_V2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_E02_CHAMBER_V2] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_A01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_A02_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_B01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_B02_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_C01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_C02_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_D01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_D02_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_E01_STRAIGHT_10M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_E02_STRAIGHT_30M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321_B01_SOLAR_PANEL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321_C01_SOLAR_PANEL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_D02_JUNCTION_T_VERTICAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_D04_JUNCTION_X_VERTICAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F11_TURN_90_UP_230M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F12_TURN_90_LEFT_230M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F13_TURN_90_RIGHT_230M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F14_TURN_90_DOWN_230M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F04_STRAIGHT_4] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F05_STRAIGHT_5] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G05_STRAIGHT_5] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_F01_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_G01_ENTRANCE_60M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_A01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_B01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_BOX01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_BOX02_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_C01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_D01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_E01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_F01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_G01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_H01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_I01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_J01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_K01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_L01_TRAFFIC_SIGN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_AUTOCANNON] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_AUTOMATIC_RIFLE_WITH_SILENCER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_CANNON] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_CRUSHER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_LASER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_NUCLEAR] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_PRESSURE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_SAW] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_THERMAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_HARVESTING_DEVICE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_MACHINE_GUN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_MISSILE_LAUNCHER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_SHOTGUN] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_SNIPER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_UNIVERSAL_LAUNCHER_BACK] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_UNIVERSAL_LAUNCHER_FRONT] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P385_A01_TEMPLE_900M] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P384_A01_HOSPITAL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P383_A01_CHURCH] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P334_A01_FOOD_GROW] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P345_A01_BIO_EXP] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P345_A01_BIO_MACH_EXP] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P345_A01_RECYCLE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P541_ESCAPE_POD] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P541_ESCAPE_POD_BASE] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P541_VENTILATOR_BODY] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P541_VENTILATOR_PROPELLER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P349_A_TOWER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P349_B_TOWER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P349_C_TOWER] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.P361_A02_HANGAR_PANEL] =
            //    new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );  

            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit] =
                new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit] =
                new MyGameplayProperties(PricePerUnit: 50000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit] =
                new MyGameplayProperties(PricePerUnit: 50000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit] =
                new MyGameplayProperties(PricePerUnit: 50000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Blueprint][(int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit] =
                new MyGameplayProperties(PricePerUnit: 50000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefabs
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Prefab_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P430_A01_PASSAGE_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P430_A02_PASSAGE_40M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P424_A01_PIPE_BASE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P423_A01_PIPE_JUNCTION] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P422_A01_PIPE_TURN_90] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P421_A01_PIPE_STRAIGHT_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P421_A02_PIPE_STRAIGHT_40M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P421_A03_PIPE_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_G01_JUNCTION_6AXES] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_G02_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P410_G01_TURN_90_RIGHT_0M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G01_STRAIGHT_1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G02_STRAIGHT_2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G03_STRAIGHT_3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G04_STRAIGHT_4] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            /*m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_F02_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);*/
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F21_TURN_S_UP] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F22_TURN_S_LEFT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F23_TURN_S_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F24_TURN_S_DOWN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F01_TURN_90_UP_230M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F02_TURN_90_LEFT_230M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F03_TURN_90_RIGHT_230M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F04_TURN_90_DOWN_230M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F01_STRAIGHT_1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F02_STRAIGHT_2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F03_STRAIGHT_3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_E01_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E01_STRAIGHT_1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E02_STRAIGHT_2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E03_STRAIGHT_3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E04_STRAIGHT_4] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E05_STRAIGHT_5] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D01_DOORCASE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D02_DOOR1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D03_DOOR2_A] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D03_DOOR2_B] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_D01_JUNCTION_T_HORIZONTAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_D03_JUNCTION_X_HORIZONTAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_D01_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D02_STRAIGHT_40M_WITH_HOLE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D03_STRAIGHT_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D04_STRAIGHT_120M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D05_STRAIGHT_180M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_C01_JUNCTION_T_HORIZONTAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_C01_JUNCTION_X_HORIZONTAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_C01_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C02_STRAIGHT_40M_WITH_HOLE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C03_STRAIGHT_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C04_STRAIGHT_120M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C05_STRAIGHT_180M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_B01_DOORCASE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE);
            /*m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_B02_DOOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);*/
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_B01_JUNCTION_T_HORIZONTAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_B02_JUNCTION_T_VERTICAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_B02_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B21_TURN_S_UP] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B22_TURN_S_LEFT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B23_TURN_S_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B24_TURN_S_DOWN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B11_TURN_90_UP_160M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B12_TURN_90_LEFT_160M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B13_TURN_90_RIGHT_160M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B14_TURN_90_DOWN_160M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B01_TURN_90_UP_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B02_TURN_90_LEFT_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B03_TURN_90_RIGHT_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B04_TURN_90_DOWN_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_B01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B02_STRAIGHT_30M_YELLOW] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B03_STRAIGHT_320M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B04_STRAIGHT_80M_WITH_SIDE_GRATES] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B05_STRAIGHT_80M_WITH_SIDE_OPEN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B06_STRAIGHT_180M_CONCRETE] =
    new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
        MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B06_STRAIGHT_200M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B07_STRAIGHT_180M_BLUE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B09_STRAIGHT_30M_GRAY] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B11_STRAIGHT_220M] =
    new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
        MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B12_STRAIGHT_160M_DARK_METAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B13_STRAIGHT_100M_TUBE_INSIDE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_A01_JUNCTION_T_HORIZONTAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_A02_JUNCTION_T_VERTICAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            /*m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_A01_ENTRANCE_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);*/
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_A02_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A21_TURN_S_UP] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A22_TURN_S_LEFT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A23_TURN_S_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A24_TURN_S_DOWN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A11_TURN_90_UP_160M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A12_TURN_90_LEFT_160M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A13_TURN_90_RIGHT_160M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A14_TURN_90_DOWN_160M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A01_TURN_90_UP_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A02_TURN_90_LEFT_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A03_TURN_90_RIGHT_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A04_TURN_90_DOWN_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A02_STRAIGHT_60M_WITH_HOLE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A03_STRAIGHT_120M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A04_STRAIGHT_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A05_STRAIGHT_80M_WITH_EXTENSION] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_E01_BRIDGE5] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_D01_BRIDGE4] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_C01_BRIDGE3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_B01_BRIDGE2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_A01_BRIDGE1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING4] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_A01_BUILDING1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P362_A01_SHORT_DISTANCE_ANTENNA] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P361_A01_LONG_DISTANCE_ANTENNA] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P351_A01_WEAPON_MOUNT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_REFINERY] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P344_A01_CONTAINER_ARM_FILLED] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P344_A02_CONTAINER_ARM_EMPTY] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P343_A01_ORE_STORAGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P342_A01_LOADING_BAY] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOCK * 1.5f);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_B01_OPEN_DOCK_VARIATION1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOCK);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_B02_OPEN_DOCK_VARIATION2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOCK);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_A02_OPEN_DOCK_VARIATION2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOCK);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P333_A01_HYDROPONIC_BUILDING] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P332_A01_OXYGEN_STORAGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P331_A01_OXYGEN_GENERATOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P324B01_FUEL_STORAGE_B] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P324A01_FUEL_STORAGE_A] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P323A01_FUEL_GENERATOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P322A01_BATTERY] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);            
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321B01_NUCLEAR_REACTOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);
            //m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321A01_SOLAR_PANEL] =
            //    new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
            //        MaxHealth: MAXHEALTH_PREFAB_TINY * 3);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312A01_SHORT_TERM_THRUSTER_LATITUDE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312A02_SHORT_TERM_THRUSTER_LATERAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P311A01_LONG_TERM_THRUSTER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A01_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A02_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A03_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A04_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A05_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A06_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A07_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A08_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A09_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A10_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A11_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A12_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A13_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A14_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A15_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A16_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A17_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A18_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221E01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221D01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221C01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221B01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221A01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211G01_PANEL_120MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211H01_PANEL_535MX130M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211G02_PANEL_60MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211G03_PANEL_60MX30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211F01_PANEL_120MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE / 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211F02_PANEL_60MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM / 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211F03_PANEL_60MX30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL / 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211E01_PANEL_120MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE / 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211E02_PANEL_60MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM / 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211E03_PANEL_60MX30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL / 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211D01_PANEL_120MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211D02_PANEL_60MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211D03_PANEL_60MX30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211C01_PANEL_120MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211C02_PANEL_60MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211C03_PANEL_60MX30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211B01_PANEL_120MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211B03_PANEL_60MX30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211A01_PANEL_120MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211A02_PANEL_60MX60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211A03_PANEL_60MX30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B01_CAGE_EMPTY] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B02_CAGE_HALFCUT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B03_CAGE_WITH_CORNERS] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B11_CAGE_PILLAR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B12_CAGE_EDGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A01_CAGE_EMPTY] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A02_CAGE_HALFCUT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A03_CAGE_WITH_CORNERS] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A11_CAGE_PILLAR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A12_CAGE_EDGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B01_THICK_FRAME_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B02_THICK_FRAME_STRAIGHT_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B11_THICK_FRAME_EDGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B12_THICK_FRAME_CORNER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B31_THICK_FRAME_JOINT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A01_THICK_FRAME_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A02_THICK_FRAME_STRAIGHT_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A11_THICK_FRAME_EDGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A12_THICK_FRAME_CORNER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A31_THICK_FRAME_JOINT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120J01_J_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130J02_J_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120I01_I_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130I02_I_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120H01_H_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130H02_H_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            /*m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120G01_G_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130G02_G_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120F01_F_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130F02_F_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120E01_E_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130E02_E_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120D01_D_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130D02_D_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C01_C_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130C02_C_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120B01_B_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130B02_B_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A01_A_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130A02_A_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);*/
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120D01_D_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120D02_D_STRAIGHT_40M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C01_C_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C02_C_STRAIGHT_40M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120B01_B_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120B02_B_STRAIGHT_40M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A01_STRONG_LATTICE_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A02_STRONG_LATTICE_STRAIGHT_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A03_STRONG_LATTICE_STRAIGHT_120M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A21_STRONG_LATTICE_JUNCTION_T_STRONG] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A22_STRONG_LATTICE_JUNCTION_T_WEAK] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A23_STRONG_LATTICE_JUNCTION_T_ROTATED] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A51_STRONG_TO_WEAK_LATTICE_2TO1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A52_STRONG_TO_WEAK_LATTICE_1TO2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A61_WEAK_LATTICE_JUNCTION_T_ROTATED] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B01_LATTICE_BEAM_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B02_LATTICE_BEAM_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_60M_WITH_PANELS] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B21_LATTICE_BEAM_JUNCTION_T_STRONG] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B22_LATTICE_BEAM_JUNCTION_T_WEAK] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B31_LATTICE_BEAM_JOINT_HORIZONTAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B32_LATTICE_BEAM_JOINT_VERTICAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A01_SOLID_BEAM_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A02_SOLID_BEAM_STRAIGHT_20M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A03_SOLID_BEAM_STRAIGHT_40M_WITH_HOLE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A04_SOLID_BEAM_STRAIGHT_40M_LATTICE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A05_SOLID_BEAM_STRAIGHT_80M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A11_SOLID_BEAM_JUNCTION_X_STRONG] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A12_SOLID_BEAM_JUNCTION_X_WEAK] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A13_SOLID_BEAM_JUNCTION_X_ROTATED] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A21_SOLID_BEAM_JUNCTION_T_STRONG] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A22_SOLID_BEAM_JUNCTION_T_WEAK] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A23_SOLID_BEAM_JUNCTION_T_ROTATED] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A31_SOLID_BEAM_JOINT_HORIZONTAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A32_SOLID_BEAM_JOINT_VERTICAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A33_SOLID_BEAM_JOINT_LONGITUDINAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A41_SOLID_BEAM_SUPERJOINT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A01_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A02_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A03_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A04_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A05_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A06_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A07_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A08_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A09_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A10_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A11_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A12_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);            
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A14_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A15_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A16_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A01_SIGN1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A02_SIGN2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A03_SIGN3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A04_SIGN4] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A05_SIGN5] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A06_SIGN6] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A07_SIGN7] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A08_SIGN8] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A09_SIGN9] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A10_SIGN10] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A11_SIGN11] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A12_SIGN12] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_A02_CHAMBER_V2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_B02_CHAMBER_V2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_C02_CHAMBER_V2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_D02_CHAMBER_V2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_E02_CHAMBER_V2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_A01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_A02_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_B01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_B02_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_C01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_C02_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_D01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_D02_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_E01_STRAIGHT_10M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_E02_STRAIGHT_30M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321_B01_SOLAR_PANEL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321_C01_SOLAR_PANEL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY * 3);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_D02_JUNCTION_T_VERTICAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_D04_JUNCTION_X_VERTICAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F11_TURN_90_UP_230M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F12_TURN_90_LEFT_230M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F13_TURN_90_RIGHT_230M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F14_TURN_90_DOWN_230M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F04_STRAIGHT_4] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_VERYSHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F05_STRAIGHT_5] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_SHORT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G05_STRAIGHT_5] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_TUNNEL_LONG);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_F01_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_G01_ENTRANCE_60M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_A01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_B01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_BOX01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_BOX02_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_C01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_D01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_E01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_F01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_G01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_H01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_I01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_J01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_K01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_L01_TRAFFIC_SIGN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.SimpleObject] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
//            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.AsteroidPrefabTest] =
//                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
//                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P385_A01_TEMPLE_900M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_LARGE);            
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P383_A01_CHURCH] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P334_A01_FOOD_GROW] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_BIO_EXP] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_BIO_MACH_EXP] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_RECYCLE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_ESCAPE_POD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_ESCAPE_POD_BASE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR_BODY] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR_PROPELLER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P349_A_TOWER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_HUGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P349_B_TOWER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_HUGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P349_C_TOWER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_HUGE * 1.5f);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_B_FACTION] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_B_FACTION_HOLO] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ARMOR_HULL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_LARGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_MEDIUM] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_SMALL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_MEDIUM] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_SMALL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_MEDIUM] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_SMALL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_MEDIUM] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_SMALL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_MEDIUM] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_SMALL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PANEL_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221F01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221G01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221H01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221J01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221K01_CHAMBER_V1] =
                            new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                                MaxHealth: MAXHEALTH_PREFAB_CHAMBER * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_C01_CLOSED_DOCK_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_LARGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_LARGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_LARGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_LARGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212F01_PANEL_LARGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);            
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_D03_HOSPITAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_MEDIUM);            
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_D05_FOOD_GROW] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);            
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_CORNER_25M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_S_45M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_180] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_45] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_90] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CONNECTION_BOX] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ADMINISTRATIVE_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARMORY] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_L] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_R] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_STR] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_CARGO_BAY] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMAND_CENTER] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMERCIAL_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMUNICATIONS] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_DEFENSES] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_DOCKS] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EMERGENCY_EXIT] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ENGINEERING_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EXIT] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EXPERIMENTAL_LABS] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_FOUNDRY] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_HABITATS] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_HANGARS] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_INDUSTRIAL_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_LANDING_BAY] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MAINTENANCE] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MILITARY_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MINES] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ORE_PROCESSING] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_OUTER_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_PRISON] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_PUBLIC_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_REACTOR] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_RESEARCH] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_RESTRICTED_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_SECURITY] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_SIGN] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_STORAGE] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_TECHNICAL_AREA] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_TRADE_PORT] = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.DEAD_PILOT] = new MyGameplayProperties(PricePerUnit: 100, WeightPerUnit: 100, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321_D01_BIG_SOLAR_PANEL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P361_B01_LONG_DISTANCE_ANTENNA_BIG] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.FOURTH_REICH_WRECK] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE * 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P344_A03_CONTAINER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_CORNER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_EDGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_HOLE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A03_SHELF_1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A02_SHELF_1X2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A01_SHELF_1X3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P611_ASTEROID_PART_A] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P611_ASTEROID_PART_B] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P363_A01_BIG_ANTENNA_300M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE * 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130D02_D_STRAIGHT_300M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130J01_J_STRAIGHT_300M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C02_C_STRAIGHT_400M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_420M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_420M_WITH_PANELS] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P363_A01_BIG_ANTENNA_1500M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE * 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4221_A01_COOLING_DEVICE_WALL_340X400] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4222_A01_PIPES_CONNECTOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4223_A01_OPEN_PIPE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P311B01_LONG_TERM_THRUSTER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE * 4);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221K01_CHAMBER_V2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_HUGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_BARREL_BIOHAZARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_BARREL_BIOHAZARD_2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_NUKE_BARREL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_RED_BARREL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL_2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_A] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_B] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_C] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            /*m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_D] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_E] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);*/
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CANNONBALL_CAPSULE_1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.GATTLING_AMMO_BELT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PACK01] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PACK02] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PLAZMA01] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_STACK_BIOCHEM01] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_O2_BARREL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.NUCLEAR_WARHEAD_CLOSED] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.NUCLEAR_WARHEAD_OPEN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_ARMOR);            
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_NUKE_BARREL_1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL_3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221L01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_HUGE);

            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_BOTTOM_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_CENTER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_TOP_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_LARGE / 2);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321E01_SOLAR_PANEL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A06_SOLID_BEAM_STRAIGHT_420M] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P311B01_CUT_THRUSTER] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312B01_CUT_THRUSTER_LATERAL] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312B02_CUT_THRUSTER_LATITUDE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ALIEN_DETECTOR_UNIT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING5] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_HUGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING6] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_HUGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING7] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_HUGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221N01_CHAMBER_V1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_BUILDING_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_D_MEDIC_CROSS] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ALIEN_ARTEFACT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BOMB] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_EXPLOSIVES);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.RAIL_GUN] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_RECYCLE_SPHERE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.PRISON] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_CHAMBER);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A17_BILLBOARD_PORTRAIT_1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A17_BILLBOARD_PORTRAIT_2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A18_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A19_BILLBOARD] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            #endregion

            #region foundation factory
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FoundationFactory] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FoundationFactory][0] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region large debris field
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeDebrisField] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_LargeDebrisField_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeDebrisField][(int)MyMwcObjectBuilder_LargeDebrisField_TypesEnum.Debris84] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region large ship
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_LargeShip_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip][(int)MyMwcObjectBuilder_LargeShip_TypesEnum.KAI] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 100000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip][(int)MyMwcObjectBuilder_LargeShip_TypesEnum.MOTHERSHIP_SAYA] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 100000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip][(int)MyMwcObjectBuilder_LargeShip_TypesEnum.JEROMIE_INTERIOR_STATION] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 100000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip][(int)MyMwcObjectBuilder_LargeShip_TypesEnum.ARDANT] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 100000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip][(int)MyMwcObjectBuilder_LargeShip_TypesEnum.CRUISER_SHIP] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 100000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_LARGESHIP);
            #endregion

            #region small ship player
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.BAER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.DOON] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ORG] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.YG] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            #endregion

            #region small ship bot
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.BAER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.DOON] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ORG] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.YG] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            #endregion

            #region sector
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Sector] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Sector][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region voxel map
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelMap] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelMap][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region voxel map merge material
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelMap_MergeMaterial] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelMap_MergeMaterial][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region voxel map merge content
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelMap_MergeContent] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelMap_MergeContent][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region voxel map neighbour
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region voxel hand sphere
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelHand_Sphere] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelHand_Sphere][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region voxel hand box
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelHand_Box] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelHand_Box][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region voxel hand cuboid
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelHand_Cuboid] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelHand_Cuboid][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region voxel hand cylinder
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelHand_Cylinder] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.VoxelHand_Cylinder][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            #endregion

            #region static asteroid
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid40000m_A] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_B] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );

            //Removed support
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_C] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_D] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.StaticAsteroid][(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_E] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region Meteor
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Meteor] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Meteor_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Meteor][(int)MyMwcObjectBuilder_Meteor_TypesEnum.DEFAULT] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region small ship
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR] = new MyGameplayProperties(PricePerUnit: 1200000, WeightPerUnit: 7000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER] = new MyGameplayProperties(PricePerUnit: 1200000, WeightPerUnit: 8000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER] = new MyGameplayProperties(PricePerUnit: 1200000, WeightPerUnit: 8500, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG] = new MyGameplayProperties(PricePerUnit: 1200000, WeightPerUnit: 6600, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA] = new MyGameplayProperties(PricePerUnit: 760000, WeightPerUnit: 5000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.BAER] = new MyGameplayProperties(PricePerUnit: 760000, WeightPerUnit: 5500, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER] = new MyGameplayProperties(PricePerUnit: 560000, WeightPerUnit: 4300, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW] = new MyGameplayProperties(PricePerUnit: 760000, WeightPerUnit: 4800, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER] = new MyGameplayProperties(PricePerUnit: 960000, WeightPerUnit: 6200, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER] = new MyGameplayProperties(PricePerUnit: 960000, WeightPerUnit: 5900, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE] = new MyGameplayProperties(PricePerUnit: 960000, WeightPerUnit: 6100, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.DOON] = new MyGameplayProperties(PricePerUnit: 760000, WeightPerUnit: 5200, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER] = new MyGameplayProperties(PricePerUnit: 1400000, WeightPerUnit: 7500, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ORG] = new MyGameplayProperties(PricePerUnit: 1600000, WeightPerUnit: 9900, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.YG] = new MyGameplayProperties(PricePerUnit: 1040000, WeightPerUnit: 7500, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK] = new MyGameplayProperties(PricePerUnit: 1160000, WeightPerUnit: 6600, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX] = new MyGameplayProperties(PricePerUnit: 1040000, WeightPerUnit: 6300, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN] = new MyGameplayProperties(PricePerUnit: 1160000, WeightPerUnit: 9000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER] = new MyGameplayProperties(PricePerUnit: 640000, WeightPerUnit: 5500, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD] = new MyGameplayProperties(PricePerUnit: 1200000, WeightPerUnit: 6000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER] = new MyGameplayProperties(PricePerUnit: 1360000, WeightPerUnit: 25000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip][(int)MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV] = new MyGameplayProperties(PricePerUnit: 1600000, WeightPerUnit: 23000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_SMALLSHIP);
            #endregion

            #region small ship assignment of ammo
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_AssignmentOfAmmo] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_AssignmentOfAmmo][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region large ship ammo
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip_Ammo] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip_Ammo][(int)MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum.CIWS_Armor_Piercing_Incendiary] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip_Ammo][(int)MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum.CIWS_High_Explosive_Incendiary] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.LargeShip_Ammo][(int)MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum.CIWS_SAPHEI] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab container
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabContainer] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabContainer_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabContainer][(int)MyMwcObjectBuilder_PrefabContainer_TypesEnum.TEMPLATE] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabContainer][(int)MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region influence spheres

            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.InfluenceSphere] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.InfluenceSphere][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );

            #endregion

            #region prefab light
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLight] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabLight_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.DEFAULT_LIGHT_0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A01_LIGHT1] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A02_LIGHT2] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A03_LIGHT3] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A04_LIGHT4] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LIGHT);
            #endregion

            #region spawn point
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SpawnPoint] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SpawnPoint][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region Drones
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Drone] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Drone_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Drone][(int)MyMwcObjectBuilder_Drone_TypesEnum.DroneCN] = new MyGameplayProperties(PricePerUnit: 5500, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DRONE, itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Drone][(int)MyMwcObjectBuilder_Drone_TypesEnum.DroneSS] = new MyGameplayProperties(PricePerUnit: 5000, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DRONE, itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Drone][(int)MyMwcObjectBuilder_Drone_TypesEnum.DroneUS] = new MyGameplayProperties(PricePerUnit: 4500, WeightPerUnit: 1, MaxAmount: 10, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DRONE, itemCategory: ItemCategory.DEVICE);
            #endregion

            #region way point
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.WaypointNew] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.WaypointNew][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region inventory
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Inventory] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Inventory][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region inventory item
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.InventoryItem] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.InventoryItem][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region checkpoint
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Checkpoint] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Checkpoint][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region player
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Player] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Player][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region player statistics
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PlayerStatistics] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PlayerStatistics][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab sound
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSound] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabSound_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.DEFAULT_SOUND_PREFAB_0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_A01_SOUND] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_B01_SOUND] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_C01_SOUND] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_D01_SOUND] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.MOTHERSHIP_SOUND] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.MADELINE_MOTHERSHIP_SOUND] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            #endregion

            #region prefab particles
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabParticles_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.DEFAULT_PARTICLE_PREFAB_0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_A01_PARTICLES] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_B01_PARTICLES] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_C01_PARTICLES] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_D01_PARTICLES] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB);
            #endregion

            #region prefab kinematic
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabKinematic_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOORCASE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR2] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOORCASE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR3] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOORCASE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR4] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOORCASE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOORCASE_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_A01_DOORCASE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_DOORCASE);


            #endregion

            #region session
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Session] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Session][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );

            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Player] = m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Bot] = m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip];

            #endregion

            #region event
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Event] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Event][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region object to build
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.ObjectToBuild] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.ObjectToBuild][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab largeship
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabLargeShip_TypesEnum>() + 1];

            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_KAI] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_ARDANT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_SAYA] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.FOURTH_REICH_MOTHERSHIP] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.FOURTH_REICH_MOTHERSHIP_B] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.RUS_MOTHERSHIP] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.RUSSIAN_MOTHERSHIP_HUMMER] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_BODY] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_ENGINE] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_LARGE_LEFT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_LARGE_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_SMALL_LEFT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_SMALL_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_LARGE_LEFT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_LARGE_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL_LEFT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL02_LEFT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL02_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_LARGESHIP);

            #endregion

            #region object group
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.ObjectGroup] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.ObjectGroup][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab large weapons
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum>() + 1];

            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1200, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1200, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_CIWS] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1200, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9] = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE_WEAPON);
            #endregion

            #region prefab hangar panel
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabHangar] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabHangar_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabHangar][(int)MyMwcObjectBuilder_PrefabHangar_TypesEnum.VENDOR]
                = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabHangar][(int)MyMwcObjectBuilder_PrefabHangar_TypesEnum.HANGAR]
                = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 120000, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            #endregion

            #region dummy point
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.DummyPoint] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.DummyPoint][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region snap point link
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SnapPointLink] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SnapPointLink][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab kinematic part
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_A02_DOOR_LEFT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_A02_DOOR_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C02_DOOR1_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C02_DOOR1_LEFT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A_LEFT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B_LEFT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B_RIGHT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORLEFT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR * 3);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORRIGHT] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_DOOR * 3);
            #endregion

            #region entity detector
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.EntityDetector] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_EntityDetector_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.EntityDetector][(int)MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.EntityDetector][(int)MyMwcObjectBuilder_EntityDetector_TypesEnum.Box] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region ship config
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.ShipConfig] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.ShipConfig][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region foundation factory
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory][(int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region security control HUB
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB][(int)MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum.DEFAULT]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB][(int)MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum.P541_SCREEN_A]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB][(int)MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum.P541_SCREEN_B]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB][(int)MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum.P541_TERMINAL_A]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            #endregion

            #region false Id
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId] = new MyGameplayProperties[MyMwcFactionsByIndex.GetFactionsIndexes().Length];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.China)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Church)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.CSR)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Euroamerican)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.FourthReich)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.FreeAsia)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Freelancers)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.FSRE)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.India)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Japan)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Miners)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.None)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Omnicorp)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Pirates)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Rangers)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Ravens)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Russian)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Saudi)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.SMLtd)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Syndicate)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Templars)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Traders)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.TTLtd)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Russian_KGB)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Slavers)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.WhiteWolves)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FalseId][MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Rainiers)] =
                new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            #endregion

            #region hacking tool
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool][(int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_1]
                = new MyGameplayProperties(PricePerUnit: 1000, WeightPerUnit: 10, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool][(int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 10, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool][(int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_3]
                = new MyGameplayProperties(PricePerUnit: 20000, WeightPerUnit: 10, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool][(int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_4]
                = new MyGameplayProperties(PricePerUnit: 50000, WeightPerUnit: 10, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool][(int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_5]
                = new MyGameplayProperties(PricePerUnit: 100000, WeightPerUnit: 10, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false , itemCategory: ItemCategory.DEVICE);
            #endregion

            #region prefab bank node
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabBankNode] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabBankNode_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabBankNode][(int)MyMwcObjectBuilder_PrefabBankNode_TypesEnum.DEFAULT]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab generator
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabGenerator_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C01_INERTIA_GENERATOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C03_CENTRIFUGE] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C04_BOX_GENERATOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C05_CENTRIFUGE_BIG] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C02_GENERATOR_WALL_BIG] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_MEDIUM);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C06_INERTIA_GENERATOR_B] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_LARGE);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C07_GENERATOR] =
                new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 2000, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_PREFAB_SMALL);
            #endregion

            #region prefab use properties
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.EntityUseProperties] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.EntityUseProperties][0] = new MyGameplayProperties(PricePerUnit: 0, WeightPerUnit: 1, MaxAmount: 1, UsedSlots: 1,
                    MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region cargo box
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_CargoBox_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type1]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type2]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type3]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type4]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type5]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type6]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type7]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type8]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type9]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type10]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type11]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type12]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_A]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_B]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_C]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_D]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.CargoBox][(int)MyMwcObjectBuilder_CargoBox_TypesEnum.DroppedItems]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: 200);
            #endregion

            #region mysterious cube
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.MysteriousCube] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_MysteriousCube_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.MysteriousCube][(int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type1]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.MysteriousCube][(int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type2]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.MysteriousCube][(int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type3]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_TINY);
            #endregion

            #region prefab Scanner
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabScanner] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabScanner_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabScanner][(int)MyMwcObjectBuilder_PrefabScanner_TypesEnum.Plane]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabScanner][(int)MyMwcObjectBuilder_PrefabScanner_TypesEnum.Rays]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab Camera
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabCamera] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabCamera_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabCamera][(int)MyMwcObjectBuilder_PrefabCamera_TypesEnum.DEFAULT]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab alarm
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabAlarm] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabAlarm_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabAlarm][(int)MyMwcObjectBuilder_PrefabAlarm_TypesEnum.DEFAULT]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_PREFAB_LIGHT, IsDestructible: true );
            #endregion

            #region alien gate
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P391_ALIEN_GATE]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region prefab kinematic rotating part
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicRotatingPart] = new MyGameplayProperties[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum>() + 1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicRotatingPart][(int)MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum.DEFAULT]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region small ship template
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShipTemplate] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShipTemplate][0]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            //GenerateItemCategory();
            #region small ship templates
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShipTemplates] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.SmallShipTemplates][0]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false );
            #endregion

            #region faction relation changes
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FactionRelationChange] = new MyGameplayProperties[1];
            m_itemProperties[(int)MyMwcObjectBuilderTypeEnum.FactionRelationChange][0]
                = new MyGameplayProperties(PricePerUnit: 10000, WeightPerUnit: 200, MaxAmount: 1, UsedSlots: 1, MaxHealth: MAXHEALTH_INDESTRUCTIBLE, IsDestructible: false);
            #endregion


            // gameplay properties per faction (DEFAULT)

            //Cannot be foreach here because obfuscator crashes
            m_itemPropertiesPerFaction.Clear();
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.None, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Euroamerican, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.China, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.FourthReich, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Omnicorp, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Russian, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Japan, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.India, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Saudi, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Church, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.FSRE, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.FreeAsia, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Pirates, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Miners, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Freelancers, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Ravens, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Traders, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Syndicate, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Templars, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Rangers, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.TTLtd, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.SMLtd, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.CSR, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Russian_KGB, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Slavers, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.WhiteWolves, m_itemProperties);
            m_itemPropertiesPerFaction.Add((int)MyMwcObjectBuilder_FactionEnum.Rainiers, m_itemProperties);




            Trace.Assert(m_itemPropertiesPerFaction.Count == Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum)).Length);

            /*
            foreach (MyMwcObjectBuilder_FactionEnum faction in Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum)))
            {
                m_itemPropertiesPerFaction.Add((int)faction, m_itemProperties);
            }*/

            /*
            foreach (MyMwcObjectBuilder_FactionEnum faction in Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum)))
            {
                foreach (ushort objectBuilderType in Enum.GetValues(typeof (MyMwcObjectBuilderTypeEnum)))
                {
                    // check if there are valid ObjectBuilderIds for concrete ObjectBuilderType
                    var gameplayPropertiesForType = m_itemPropertiesPerFaction[(int) faction][objectBuilderType];
                    MyCommonDebugUtils.AssertDebug(gameplayPropertiesForType != null);
                    for(int objectBuilderId = 0; objectBuilderId < gameplayPropertiesForType.Length; objectBuilderId++)
                    {
                        var gameplayProterties = gameplayPropertiesForType[objectBuilderId];
                        if(gameplayProterties != null)
                        {
                            MyCommonDebugUtils.AssertDebug(MyMwcObjectBuilder_Base.IsObjectBuilderIdValid((MyMwcObjectBuilderTypeEnum) objectBuilderType, objectBuilderId));
                        }
                    }

                    // check if there are all objectbuilder's types and ids
                    int[] objectBuilderIDs = MyMwcObjectBuilder_Base.GetObjectBuilderIDs((MyMwcObjectBuilderTypeEnum) objectBuilderType);
                    foreach (int objectBuilderId in objectBuilderIDs)
                    {
                        MyCommonDebugUtils.AssertDebug(gameplayPropertiesForType[objectBuilderId] != null);
                    }
                }
            }*/



            MyMwcLog.WriteLine("MyGameplayConstants() - nd");

        }

        private static void GenerateItemCategory()
        {
            int index = (int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo;
            foreach (var gameplayProperty in m_itemProperties[index])
            {
                if (gameplayProperty != null) gameplayProperty.ItemCategory = ItemCategory.AMMO;
            }

            index = (int)MyMwcObjectBuilderTypeEnum.Ore;
            foreach (var gameplayProperty in m_itemProperties[index])
            {
                if (gameplayProperty != null) gameplayProperty.ItemCategory = ItemCategory.ORE;
            }

            index = (int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon;
            foreach (var gameplayProperty in m_itemProperties[index])
            {
                if (gameplayProperty != null) gameplayProperty.ItemCategory = ItemCategory.WEAPON;
            }

            index = (int)MyMwcObjectBuilderTypeEnum.SmallShip_Tool;
            foreach (var gameplayProperty in m_itemProperties[index])
            {
                if (gameplayProperty != null) gameplayProperty.ItemCategory = ItemCategory.DEVICE;
            }
        }

        // this method is there because we need load and check asserts when the game started
        public static void Check()
        {
            MyMwcLog.WriteLine("MyGamePlayConstants.Check()");
        }

        public static void SetGameplayDifficulty(MyGameplayDifficultyEnum difficulty)
        {
            m_difficulty = difficulty;
            GameplayDifficultyProfile = m_gameplayDifficultyProfiles[(int)difficulty];
        }

        public static MyGameplayDifficultyEnum GetGameplayDifficulty()
        {
            return m_difficulty;
        }

        public static MyGameplayDifficultyProfile GetGameplayDifficultyProfile(MyGameplayDifficultyEnum difficulty)
        {
            return m_gameplayDifficultyProfiles[(int)difficulty];
        }

        public static MyGameplayProperties GetGameplayProperties(MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderId, MyMwcObjectBuilder_FactionEnum faction)
        {
            //if (m_itemProperties[(int)objectBuilderType] == null)
            //{
            //    return null;
            //}
            //return m_itemProperties[(int)objectBuilderType][objectBuilderId];
            if (m_itemPropertiesPerFaction[(int)faction] == null)
            {
                return null;
            }
            return m_itemPropertiesPerFaction[(int)faction][(int)objectBuilderType][objectBuilderId];
        }

        public static MyGameplayProperties GetGameplayProperties(MyMwcObjectBuilder_Base objectBuilder, MyMwcObjectBuilder_FactionEnum faction)
        {
            int objectBuilderId = objectBuilder.GetObjectBuilderId() != null
                                      ? objectBuilder.GetObjectBuilderId().Value
                                      : 0;
            return GetGameplayProperties(objectBuilder.GetObjectBuilderType(), objectBuilderId, faction);
        }
    }
}
