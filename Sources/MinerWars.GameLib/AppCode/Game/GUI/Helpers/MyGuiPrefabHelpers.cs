using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Prefabs;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    static class MyGuiPrefabHelpers
    {
        //Dictionaries for helpers
        static Dictionary<BuildTypesEnum, MyGuiPrefabBuildTypeHelper> m_buildTypeHelpers = new Dictionary<BuildTypesEnum, MyGuiPrefabBuildTypeHelper>();
        static Dictionary<CategoryTypesEnum, MyGuiPrefabCategoryTypeHelper> m_categoryTypeHelpers = new Dictionary<CategoryTypesEnum, MyGuiPrefabCategoryTypeHelper>();
        static Dictionary<SubCategoryTypesEnum, MyGuiPrefabSubCategoryTypeHelper> m_subCategoryTypeHelpers = new Dictionary<SubCategoryTypesEnum, MyGuiPrefabSubCategoryTypeHelper>();
        //static Dictionary<MyMwcObjectBuilder_Prefab_TypesEnum, MyGuiPrefabHelper> m_prefabTypeHelpers = new Dictionary<MyMwcObjectBuilder_Prefab_TypesEnum, MyGuiPrefabHelper>();
        
        //Arrays of enums values
        public static Array MyMwcBuildTypesEnumValues;
        public static Array MyMwcCategoryTypesEnumValues;
        public static Array MyMwcSubCategoryTypesEnumValues;
        public static MyMwcObjectBuilderTypeEnum[] MyMwcPrefabTypesEnumValues;  //Cannot be Array because obfuscator crash (he doesnt like foreach+Array)
        public static Array MyMwcFactionTextureEnumValues;
        
        static MyGuiPrefabHelpers()
        {
            MyMwcLog.WriteLine("MyGuiPrefabHelpers()");
            
            MyMwcBuildTypesEnumValues = Enum.GetValues(typeof(BuildTypesEnum));
            MyMwcCategoryTypesEnumValues = Enum.GetValues(typeof(CategoryTypesEnum));
            MyMwcSubCategoryTypesEnumValues = Enum.GetValues(typeof(SubCategoryTypesEnum));
            //MyMwcPrefabTypesEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_Prefab_TypesEnum));
            MyMwcPrefabTypesEnumValues = new MyMwcObjectBuilderTypeEnum[]
                                         {
                                             MyMwcObjectBuilderTypeEnum.Prefab,
                                             MyMwcObjectBuilderTypeEnum.PrefabHangar,
                                             MyMwcObjectBuilderTypeEnum.PrefabKinematic,
                                             MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon,
                                             MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                                             MyMwcObjectBuilderTypeEnum.PrefabKinematicPart,
                                             MyMwcObjectBuilderTypeEnum.PrefabLight,
                                             MyMwcObjectBuilderTypeEnum.PrefabParticles,
                                             MyMwcObjectBuilderTypeEnum.PrefabSound,
                                             MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory,
                                             MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB,
                                             MyMwcObjectBuilderTypeEnum.PrefabBankNode,
                                             MyMwcObjectBuilderTypeEnum.PrefabGenerator,
                                             MyMwcObjectBuilderTypeEnum.PrefabScanner,
                                             MyMwcObjectBuilderTypeEnum.PrefabAlarm,
                                             MyMwcObjectBuilderTypeEnum.PrefabCamera,
                                         };
            MyMwcFactionTextureEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_Prefab_AppearanceEnum));
        }

        public static StringBuilder GetFactionName(MyMwcObjectBuilder_Prefab_AppearanceEnum appearanceTextureEnum)
        {
            switch (appearanceTextureEnum)
            {
                case MyMwcObjectBuilder_Prefab_AppearanceEnum.None: return MyTextsWrapper.Get(MyTextsWrapperEnum.FactionNeutralShort);
                case MyMwcObjectBuilder_Prefab_AppearanceEnum.FourthReich: return MyTextsWrapper.Get(MyTextsWrapperEnum.FactionFourthReichShort);
                case MyMwcObjectBuilder_Prefab_AppearanceEnum.Chinese: return MyTextsWrapper.Get(MyTextsWrapperEnum.FactionChineseShort);
                case MyMwcObjectBuilder_Prefab_AppearanceEnum.Euroamerican: return MyTextsWrapper.Get(MyTextsWrapperEnum.FactionEuroamericanShort);
                case MyMwcObjectBuilder_Prefab_AppearanceEnum.Omnicorp: return MyTextsWrapper.Get(MyTextsWrapperEnum.FactionOmnicorpShort);
                case MyMwcObjectBuilder_Prefab_AppearanceEnum.Russian: return MyTextsWrapper.Get(MyTextsWrapperEnum.FactionRussianShort);
                case MyMwcObjectBuilder_Prefab_AppearanceEnum.Church: return MyTextsWrapper.Get(MyTextsWrapperEnum.FactionChurchShort);
                case MyMwcObjectBuilder_Prefab_AppearanceEnum.Saudi: return MyTextsWrapper.Get(MyTextsWrapperEnum.FactionSaudiShort);
            }

            return null;
        }

        public static MyGuiPrefabBuildTypeHelper GetMyGuiBuildTypeHelper(BuildTypesEnum buildType)
        {
            MyGuiPrefabBuildTypeHelper ret;
            if (m_buildTypeHelpers.TryGetValue(buildType, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiPrefabCategoryTypeHelper GetMyGuiCategoryTypeHelper(CategoryTypesEnum categoryType)
        {
            MyGuiPrefabCategoryTypeHelper ret;
            if (m_categoryTypeHelpers.TryGetValue(categoryType, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiPrefabSubCategoryTypeHelper GetMyGuiSubCategoryTypeHelper(SubCategoryTypesEnum subCategoryType)
        {
            MyGuiPrefabSubCategoryTypeHelper ret;
            if (m_subCategoryTypeHelpers.TryGetValue(subCategoryType, out ret))
                return ret;
            else
                return null;
        }

        //public static MyGuiPrefabHelper GetMyGuiPrefabTypeHelper(MyMwcObjectBuilder_Prefab_TypesEnum prefabTypeEnum)
        //{
        //    MyGuiPrefabHelper ret;
        //    if (m_prefabTypeHelpers.TryGetValue(prefabTypeEnum, out ret))
        //        return ret;
        //    else
        //        return null;
        //}

        public static void UnloadContent()
        {

            m_buildTypeHelpers.Clear();
            m_categoryTypeHelpers.Clear();
            m_subCategoryTypeHelpers.Clear();
            //m_prefabTypeHelpers.Clear();

        }

        public static void LoadContent()
        {
            #region prefab build type helpers

            m_buildTypeHelpers.Add(BuildTypesEnum.BEAMS,
                new MyGuiPrefabBuildTypeHelper(MyTextsWrapperEnum.buildTypeBeam));

            m_buildTypeHelpers.Add(BuildTypesEnum.CONNECTIONS,
                new MyGuiPrefabBuildTypeHelper(MyTextsWrapperEnum.buildTypeConnection));

            m_buildTypeHelpers.Add(BuildTypesEnum.DETAILS,
                new MyGuiPrefabBuildTypeHelper(MyTextsWrapperEnum.buildTypeDetail));

            m_buildTypeHelpers.Add(BuildTypesEnum.MODULES,
                new MyGuiPrefabBuildTypeHelper(MyTextsWrapperEnum.buildTypeModule));

            m_buildTypeHelpers.Add(BuildTypesEnum.SHELLS,
                new MyGuiPrefabBuildTypeHelper(MyTextsWrapperEnum.buildTypePanel));

            m_buildTypeHelpers.Add(BuildTypesEnum.CUSTOM,
                new MyGuiPrefabBuildTypeHelper(MyTextsWrapperEnum.buildTypeCustom));

            #endregion

            #region prefab category type helpers

            m_categoryTypeHelpers.Add(CategoryTypesEnum.COMMUNICATIONS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeCommunications));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.FLIGHT,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeFlight));

            /*m_categoryTypeHelpers.Add(CategoryTypesEnum.FUEL,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeFuel));*/

            m_categoryTypeHelpers.Add(CategoryTypesEnum.HANGARS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeHangars));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.INDUSTRY,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeIndustry));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.LARGE,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeLarge));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.LIFE_SUPPORT,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeLifeSupport));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.MANNED_OBJECTS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeMannedObjects));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.MEDIUM,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeMedium));

            /*m_categoryTypeHelpers.Add(CategoryTypesEnum.OXYGEN,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeOxygen));*/

            m_categoryTypeHelpers.Add(CategoryTypesEnum.PIPES,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypePipe));

            /*m_categoryTypeHelpers.Add(CategoryTypesEnum.POWER,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypePower));*/

            m_categoryTypeHelpers.Add(CategoryTypesEnum.SMALL,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeSmall));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.SUPPLY,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeSupply));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.TUNNELS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeTunnel));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.WEAPONRY,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeWeaponry));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.PASSAGES,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypePassages));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.ARMORS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeArmors));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.CHAMBERS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeChambers));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.PANELS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypePanels));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.FRAME,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeFrame));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.LIGHTS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeLights));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.PARTICLES,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeParticles));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.SOUNDS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeSounds));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.BILLBOARDS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeBillboards));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.SIGNS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeSigns));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.TRAFFIC_SIGNS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeTrafficSigns));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.OTHER,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeOther));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.DOORS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeDoors));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.DOOR_CASES,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeDoorCases));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.LARGE_SHIPS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeLargeShips));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.FOUNDATION_FACTORY,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.FoundationFactory));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.SECURITY_CONTROL_HUB,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.SecurityControlHUB));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.BANK_NODE,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.BankNode));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.GENERATOR,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.Generator));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.SCANNER,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.Scanner));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.ALARM,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.Alarm));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.MSHIP,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeMship));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.CABLES,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.categoryTypeCables));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.CAMERA,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.Camera));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.SHELVES,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeShelves));

            m_categoryTypeHelpers.Add(CategoryTypesEnum.BARRELS,
                new MyGuiPrefabCategoryTypeHelper(MyTextsWrapperEnum.CategoryTypeBarrels));            

            foreach (CategoryTypesEnum prefabTypeEnum in Enum.GetValues(typeof(CategoryTypesEnum)))
            {
                MyGuiPrefabCategoryTypeHelper prefabGuiHelper = null;
                m_categoryTypeHelpers.TryGetValue(prefabTypeEnum, out prefabGuiHelper);

                MyCommonDebugUtils.AssertDebug(prefabGuiHelper != null);
            }


            #endregion

            #region prefab subcategory type helpers

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.AMMUNITION_BOX,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeAmmunitionBox));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.BIODOME,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeBiodome));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.BRIDGE,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeBridge));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.DOCKS,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeDocks));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.ESCAPE_PODS,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeEscapePods));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.FACTORY,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeFactory));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.FUEL_GENERATION,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeFuelGeneration));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.FUEL_STORAGE,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeFuelStorage));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.GAS_STORAGE,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeGasStorage));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.JUNCTION,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeJunction));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.LARGE,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeLarge));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.LIVING_QUARTERS,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeLivingQuarters));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.LOADING_UNLOADING,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeLoadingUnloading));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.LONG_DISTANCE_ANTENNA,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeLongDistanceAntenna));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.LONG_TERM_THRUSTERS,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeLongTermThrusters));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.ORE_STORAGE,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeOreStorage));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.OTHER,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeOther));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.OXYGEN_GENERATION,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeOxygenGeneration));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.OXYGEN_STORAGE,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeOxygenStorage));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.PASSAGE,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypePassage));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.POWER_GENERATION,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypePowerGeneration));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.POWER_STORAGE,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypePowerStorage));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.REFINERY,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeRefinery));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.SHORT_DISTANCE_ANTENNA,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeShortDistanceAntenna));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.SHORT_TERM_THRUSTERS,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeShortTermThrusters));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.SMALL,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeSmall));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.STRAIGHT,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeStraight));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.TURN,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeTurn));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.WEAPON_MOUNT,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeWeaponMount));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.WIRES,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeWires));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.CABLE_HOLDERS,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeCableHolders));

            m_subCategoryTypeHelpers.Add(SubCategoryTypesEnum.CONNECTION_BOXES,
                new MyGuiPrefabSubCategoryTypeHelper(MyTextsWrapperEnum.subCategoryTypeConnectionBoxes));            


            foreach (SubCategoryTypesEnum prefabTypeEnum in Enum.GetValues(typeof(SubCategoryTypesEnum)))
            {
                MyGuiPrefabSubCategoryTypeHelper prefabGuiHelper = null;
                m_subCategoryTypeHelpers.TryGetValue(prefabTypeEnum, out prefabGuiHelper);

                MyCommonDebugUtils.AssertDebug(prefabGuiHelper != null);
            }


            #endregion

            #region Create prefab module helpers

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P430_A01_PASSAGE_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p430_a01_passage_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P430_A02_PASSAGE_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p430_a02_passage_40m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P424_A01_PIPE_BASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p424_a01_pipe_base));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P423_A01_PIPE_JUNCTION, new MyGuiPrefabHelper(MyTextsWrapperEnum.p423_a01_pipe_junction));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P422_A01_PIPE_TURN_90, new MyGuiPrefabHelper(MyTextsWrapperEnum.p422_a01_pipe_turn_90));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P421_A01_PIPE_STRAIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p421_a01_pipe_straight_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P421_A02_PIPE_STRAIGHT_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p421_a02_pipe_straight_40m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P421_A03_PIPE_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p421_a03_pipe_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_G01_JUNCTION_6AXES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_g01_junction_6axes));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_G02_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_g02_entrance_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P410_G01_TURN_90_RIGHT_0M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p410_g01_turn_90_right_0m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_G01_STRAIGHT_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g01_straight_1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_G02_STRAIGHT_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g02_straight_2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_G03_STRAIGHT_3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g03_straight_3));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_G04_STRAIGHT_4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g04_straight_4));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_F02_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_f02_entrance_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F21_TURN_S_UP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f21_turn_s_up));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F22_TURN_S_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f22_turn_s_left));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F23_TURN_S_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f23_turn_s_right));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F24_TURN_S_DOWN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f24_turn_s_down));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F01_TURN_90_UP_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f01_turn_90_up_230m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F02_TURN_90_LEFT_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f02_turn_90_left_230m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F03_TURN_90_RIGHT_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f03_turn_90_right_230m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F04_TURN_90_DOWN_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f04_turn_90_down_230m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_F01_STRAIGHT_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f01_straight_1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f02_straight_2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_F03_STRAIGHT_3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f03_straight_3));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_E01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_e01_entrance_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_E01_STRAIGHT_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e01_straight_1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_E02_STRAIGHT_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e02_straight_2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_E03_STRAIGHT_3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e03_straight_3));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_E04_STRAIGHT_4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e04_straight_4));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_E05_STRAIGHT_5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e05_straight_5));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_D01_DOORCASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d01_doorcase));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_D02_DOOR1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d02_door1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_D03_DOOR2_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d03_door2_a));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_D03_DOOR2_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d03_door2_b));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_D01_JUNCTION_T_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_d01_junction_t_horizontal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_D03_JUNCTION_X_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_d03_junction_x_horizontal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_D01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_d01_entrance_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_D01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d01_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_D02_STRAIGHT_40M_WITH_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d02_straight_40m_with_hole));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_D03_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d03_straight_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_D04_STRAIGHT_120M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d04_straight_120m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_D05_STRAIGHT_180M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d05_straight_180m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C01_DOOR1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_c02_door1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C01_DOOR2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_c01_door2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C01_DOOR3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_c01_door3));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C01_DOOR4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_c01_door4));            
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_C01_JUNCTION_T_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_c01_junction_t_horizontal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_C01_JUNCTION_X_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_c01_junction_x_horizontal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_C01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_c01_entrance_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_C01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c01_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_C02_STRAIGHT_40M_WITH_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c02_straight_40m_with_hole));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_C03_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c03_straight_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_C04_STRAIGHT_120M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c04_straight_120m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_C05_STRAIGHT_180M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c05_straight_180m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_B01_DOORCASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_b01_doorcase));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_B02_DOOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_b02_door));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_B01_JUNCTION_T_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_b01_junction_t_horizontal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_B02_JUNCTION_T_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_b02_junction_t_vertical));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_B02_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_b02_entrance_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B21_TURN_S_UP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b21_turn_s_up));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B22_TURN_S_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b22_turn_s_left));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B23_TURN_S_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b23_turn_s_right));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B24_TURN_S_DOWN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b24_turn_s_down));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B11_TURN_90_UP_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b11_turn_90_up_160m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B12_TURN_90_LEFT_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b12_turn_90_left_160m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B13_TURN_90_RIGHT_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b13_turn_90_right_160m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B14_TURN_90_DOWN_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b14_turn_90_down_160m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B01_TURN_90_UP_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b01_turn_90_up_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B02_TURN_90_LEFT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b02_turn_90_left_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B03_TURN_90_RIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b03_turn_90_right_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_B04_TURN_90_DOWN_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b04_turn_90_down_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_B01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b01_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B02_STRAIGHT_30M_YELLOW, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b02_straight_30m_yellow));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B03_STRAIGHT_320M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b03_straight_320m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B04_STRAIGHT_80M_WITH_SIDE_GRATES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b04_straight_80m_with_side_grates));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B05_STRAIGHT_80M_WITH_SIDE_OPEN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b05_straight_80m_with_side_open));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B06_STRAIGHT_180M_CONCRETE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b06_straight_180m_concrete));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B06_STRAIGHT_200M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b06_straight_200m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B07_STRAIGHT_180M_BLUE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b07_straight_180m_blue));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B09_STRAIGHT_30M_GRAY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b09_straight_30m_gray));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B11_STRAIGHT_220M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b11_straight_220m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B12_STRAIGHT_160M_DARK_METAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b12_straight_160m_dark_metal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.p411_B13_STRAIGHT_100M_TUBE_INSIDE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b13_straight_100m_tube_inside));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_A01_DOORCASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_a01_doorcase));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_A02_DOOR_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_a02_door));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_A02_DOOR_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_a02_door));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_A01_JUNCTION_T_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_a01_junction_t_horizontal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_A02_JUNCTION_T_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_a02_junction_t_vertical));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_A01_ENTRANCE_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_a01_entrance_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_A02_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_a02_entrance_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A21_TURN_S_UP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a21_turn_s_up));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A22_TURN_S_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a22_turn_s_left));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A23_TURN_S_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a23_turn_s_right));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A24_TURN_S_DOWN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a24_turn_s_down));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A11_TURN_90_UP_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a11_turn_90_up_160m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A12_TURN_90_LEFT_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a12_turn_90_left_160m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A13_TURN_90_RIGHT_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a13_turn_90_right_160m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A14_TURN_90_DOWN_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a14_turn_90_down_160m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A01_TURN_90_UP_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a01_turn_90_up_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A02_TURN_90_LEFT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a02_turn_90_left_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A03_TURN_90_RIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a03_turn_90_right_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_A04_TURN_90_DOWN_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a04_turn_90_down_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_A01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a01_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_A02_STRAIGHT_60M_WITH_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a02_straight_60m_with_hole));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_A03_STRAIGHT_120M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a03_straight_120m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_A04_STRAIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a04_straight_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_A05_STRAIGHT_80M_WITH_EXTENSION, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a05_straight_80m_with_extension));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P382_E01_BRIDGE5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_e01_bridge5));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P382_D01_BRIDGE4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_d01_bridge4));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P382_C01_BRIDGE3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_c01_bridge3));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P382_B01_BRIDGE2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_b01_bridge2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P382_A01_BRIDGE1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_a01_bridge1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_c01_building3));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_b01_building2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P381_A01_BUILDING1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_a01_building1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P361_A01_SMALL_HANGAR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p361_a01_small_hangar));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P362_A01_SHORT_DISTANCE_ANTENNA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p362_a01_short_distance_antenna));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P361_A01_LONG_DISTANCE_ANTENNA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p361_a01_long_distance_antenna));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P351_A01_WEAPON_MOUNT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a01_weapon_mount));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a01_largeship_autocannon));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_CIWS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a01_largeship_ciws));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a01_largeship_machinegun));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a01_largeship_missile_gun4));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a01_largeship_missile_gun6));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a01_largeship_missile_gun9));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided4));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided6));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided9));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_REFINERY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_refinery));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P344_A01_CONTAINER_ARM_FILLED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p344_a01_container_arm_filled));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P344_A02_CONTAINER_ARM_EMPTY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p344_a02_container_arm_empty));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P343_A01_ORE_STORAGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p343_a01_ore_storage));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P342_A01_LOADING_BAY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p342_a01_loading_bay));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P341_B01_OPEN_DOCK_VARIATION1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p341_b01_open_dock_variation1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P341_B02_OPEN_DOCK_VARIATION2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p341_b02_open_dock_variation2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p341_a01_open_dock_variation1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P341_A02_OPEN_DOCK_VARIATION2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p341_a02_open_dock_variation2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P333_A01_HYDROPONIC_BUILDING, new MyGuiPrefabHelper(MyTextsWrapperEnum.p333_a01_hydroponic_building));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P332_A01_OXYGEN_STORAGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p332_a01_oxygen_storage));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P331_A01_OXYGEN_GENERATOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p331_a01_oxygen_generator));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P324B01_FUEL_STORAGE_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.p324b01_fuel_storage_b));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P324A01_FUEL_STORAGE_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.p324a01_fuel_storage_a));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P323A01_FUEL_GENERATOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p323a01_fuel_generator));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P322A01_BATTERY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p322a01_battery));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P321C01_INERTIA_GENERATOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321c01_inertia_generator));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P321B01_NUCLEAR_REACTOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321b01_nuclear_reactor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P321A01_SOLAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321a01_solar_panel));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P312A01_SHORT_TERM_THRUSTER_LATITUDE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p312a01_short_term_thruster_latitude));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P312A02_SHORT_TERM_THRUSTER_LATERAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p312a02_short_term_thruster_lateral));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P311A01_LONG_TERM_THRUSTER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p311a01_long_term_thruster));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A01_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a01_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A02_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a02_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A03_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a03_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A04_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a04_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A05_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a05_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A06_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a06_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A07_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a07_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A08_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a08_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A09_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a09_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A10_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a10_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A11_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a11_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A12_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a12_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A13_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a13_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A14_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a14_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A15_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a15_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A16_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a16_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A17_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a17_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P231A18_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a18_armor));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221E01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221e01_chamber_v1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221D01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221d01_chamber_v1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221C01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221c01_chamber_v1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221B01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221b01_chamber_v1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221A01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221a01_chamber_v1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211G01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211g01_panel_120mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211G02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211g02_panel_60mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211G03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211g03_panel_60mx30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211F01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211f01_panel_120mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211F02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211f02_panel_60mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211F03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211f03_panel_60mx30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211E01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211e01_panel_120mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211E02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211e02_panel_60mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211E03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211e03_panel_60mx30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211D01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211d01_panel_120mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211D02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211d02_panel_60mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211D03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211d03_panel_60mx30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211C01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211c01_panel_120mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211C02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211c02_panel_60mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211C03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211c03_panel_60mx30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211B01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211b01_panel_120mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211b02_panel_60mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211B03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211b03_panel_60mx30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211A01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211a01_panel_120mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211A02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211a02_panel_60mx60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P211A03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211a03_panel_60mx30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142B01_CAGE_EMPTY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b01_cage_empty));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142B02_CAGE_HALFCUT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b02_cage_halfcut));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142B03_CAGE_WITH_CORNERS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b03_cage_with_corners));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142B11_CAGE_PILLAR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b11_cage_pillar));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142B12_CAGE_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b12_cage_edge));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142A01_CAGE_EMPTY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a01_cage_empty));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142A02_CAGE_HALFCUT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a02_cage_halfcut));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142A03_CAGE_WITH_CORNERS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a03_cage_with_corners));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142A11_CAGE_PILLAR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a11_cage_pillar));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P142A12_CAGE_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a12_cage_edge));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141B01_THICK_FRAME_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b01_thick_frame_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141B02_THICK_FRAME_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b02_thick_frame_straight_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141B11_THICK_FRAME_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b11_thick_frame_edge));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141B12_THICK_FRAME_CORNER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b12_thick_frame_corner));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141B31_THICK_FRAME_JOINT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b31_thick_frame_joint));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141A01_THICK_FRAME_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a01_thick_frame_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141A02_THICK_FRAME_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a02_thick_frame_straight_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141A11_THICK_FRAME_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a11_thick_frame_edge));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141A12_THICK_FRAME_CORNER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a12_thick_frame_corner));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P141A31_THICK_FRAME_JOINT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a31_thick_frame_joint));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120J01_J_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120j01_j_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130J02_J_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130j02_j_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120I01_I_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120i01_i_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130I02_I_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130i02_i_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120H01_H_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120h01_h_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130H02_H_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130h02_h_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120G01_G_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120g01_g_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130G02_G_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130g02_g_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120F01_F_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120f01_f_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130F02_F_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130f02_f_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120E01_E_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120e01_e_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130E02_E_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130e02_e_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120D01_D_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120d01_d_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130D02_D_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130d02_d_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120C01_C_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120c01_c_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130C02_C_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130c02_c_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120B01_B_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120b01_b_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130B02_B_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130b02_b_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A01_A_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a01_a_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130A02_A_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130a02_a_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120D01_D_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120d01_d_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120D02_D_STRAIGHT_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120d02_d_straight_40m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120C01_C_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120c01_c_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120C02_C_STRAIGHT_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120c02_c_straight_40m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120B01_B_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120b01_b_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120B02_B_STRAIGHT_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120b02_b_straight_40m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A01_STRONG_LATTICE_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a01_strong_lattice_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A02_STRONG_LATTICE_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a02_strong_lattice_straight_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A03_STRONG_LATTICE_STRAIGHT_120M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a03_strong_lattice_straight_120m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A21_STRONG_LATTICE_JUNCTION_T_STRONG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a21_strong_lattice_junction_t_strong));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A22_STRONG_LATTICE_JUNCTION_T_WEAK, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a22_strong_lattice_junction_t_weak));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A23_STRONG_LATTICE_JUNCTION_T_ROTATED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a23_strong_lattice_junction_t_rotated));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A51_STRONG_TO_WEAK_LATTICE_2TO1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a51_strong_to_weak_lattice_2to1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A52_STRONG_TO_WEAK_LATTICE_1TO2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a52_strong_to_weak_lattice_1to2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P120A61_WEAK_LATTICE_JUNCTION_T_ROTATED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a61_weak_lattice_junction_t_rotated));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110B01_LATTICE_BEAM_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b01_lattice_beam_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110B02_LATTICE_BEAM_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b02_lattice_beam_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b03_lattice_beam_straight_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_60M_WITH_PANELS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b04_lattice_beam_straight_60m_with_panels));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110B21_LATTICE_BEAM_JUNCTION_T_STRONG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b21_lattice_beam_junction_t_strong));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110B22_LATTICE_BEAM_JUNCTION_T_WEAK, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b22_lattice_beam_junction_t_weak));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110B31_LATTICE_BEAM_JOINT_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b31_lattice_beam_joint_horizontal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110B32_LATTICE_BEAM_JOINT_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b32_lattice_beam_joint_vertical));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A01_SOLID_BEAM_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a01_solid_beam_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A02_SOLID_BEAM_STRAIGHT_20M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a02_solid_beam_straight_20m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A03_SOLID_BEAM_STRAIGHT_40M_WITH_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a03_solid_beam_straight_40m_with_hole));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A04_SOLID_BEAM_STRAIGHT_40M_LATTICE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a04_solid_beam_straight_40m_lattice));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A05_SOLID_BEAM_STRAIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a05_solid_beam_straight_80m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A11_SOLID_BEAM_JUNCTION_X_STRONG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a11_solid_beam_junction_x_strong));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A12_SOLID_BEAM_JUNCTION_X_WEAK, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a12_solid_beam_junction_x_weak));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A13_SOLID_BEAM_JUNCTION_X_ROTATED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a13_solid_beam_junction_x_rotated));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A21_SOLID_BEAM_JUNCTION_T_STRONG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a21_solid_beam_junction_t_strong));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A22_SOLID_BEAM_JUNCTION_T_WEAK, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a22_solid_beam_junction_t_weak));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A23_SOLID_BEAM_JUNCTION_T_ROTATED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a23_solid_beam_junction_t_rotated));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A31_SOLID_BEAM_JOINT_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a31_solid_beam_joint_horizontal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A32_SOLID_BEAM_JOINT_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a32_solid_beam_joint_vertical));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A33_SOLID_BEAM_JOINT_LONGITUDINAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a33_solid_beam_joint_longitudinal));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P110A41_SOLID_BEAM_SUPERJOINT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a41_solid_beam_superjoint));
            ////@ Lights
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.DEFAULT_LIGHT_0, new MyGuiPrefabHelper(MyTextsWrapperEnum.default_light_0));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P521_A01_LIGHT1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p521_a01_light1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P521_A02_LIGHT2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p521_a02_light2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P521_A03_LIGHT3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p521_a03_light3));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P521_A04_LIGHT4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p521_a04_light4));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.DEFAULT_PARTICLE_PREFAB_0, new MyGuiPrefabHelper(MyTextsWrapperEnum.default_particles_prefab_0));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P551_A01_PARTICLES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p551_a01_particles));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P551_B01_PARTICLES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p551_b01_particles));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P551_C01_PARTICLES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p551_c01_particles));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P551_D01_PARTICLES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p551_d01_particles));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.DEFAULT_SOUND_PREFAB_0, new MyGuiPrefabHelper(MyTextsWrapperEnum.default_sound_prefab_0));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P561_A01_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.p561_a01_sound));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P561_B01_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.p561_b01_sound));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P561_C01_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.p561_c01_sound));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P561_D01_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.p561_d01_sound));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A01_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a01_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A02_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a02_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A03_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a03_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A04_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a04_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A05_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a05_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A06_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a06_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A07_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a07_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A08_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a08_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A09_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a09_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A10_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a10_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A11_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a11_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A12_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a12_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A13_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a13_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A14_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a14_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A15_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a15_billboard));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P511_A16_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a16_billboard));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A01_SIGN1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a01_sign1));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A02_SIGN2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a02_sign2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A03_SIGN3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a03_sign3));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A04_SIGN4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a04_sign4));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A05_SIGN5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a05_sign5));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A06_SIGN6, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a06_sign6));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A07_SIGN7, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a07_sign7));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A08_SIGN8, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a08_sign8));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A09_SIGN9, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a09_sign9));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A10_SIGN10, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a10_sign10));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A11_SIGN11, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a11_sign11));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_A12_SIGN12, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a12_sign12));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221_A02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221_a02_chamber_v2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221_B02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221_b02_chamber_v2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221_C02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221_c02_chamber_v2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221_D02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221_d02_chamber_v2));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P221_E02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221_e02_chamber_v2));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_A01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130a01_a_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_A02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130a02_a_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_B01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130b01_b_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_B02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130b02_b_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_C01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130c01_c_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_C02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130c02_c_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_D01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130d01_d_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_D02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130d02_d_straight_30m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_E01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130e01_e_straight_10m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P130_E02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130e02_e_straight_30m));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P321_B01_SOLAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321b01_solar_panel));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P321_C01_SOLAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321c01_solar_panel));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_D02_JUNCTION_T_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_d02_junction_t_vertical));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P413_D04_JUNCTION_X_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_d04_junction_x_vertical));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F11_TURN_90_UP_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f11_turn_90_up_230m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F12_TURN_90_LEFT_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f12_turn_90_left_230m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F13_TURN_90_RIGHT_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f13_turn_90_right_230m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P412_F14_TURN_90_DOWN_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f14_turn_90_down_230m));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_F04_STRAIGHT_4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f04_straight_4));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_F05_STRAIGHT_5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f05_straight_5));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P411_G05_STRAIGHT_5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g05_straight_5));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_F01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_f01_entrance_60m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P414_G01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_g01_entrance_60m));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_A01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_a01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_B01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_b01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_BOX01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_box01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_BOX02_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_box02_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_C01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_c01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_D01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_d01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_E01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_e01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_F01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_f01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_G01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_g01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_H01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_h01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_I01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_i01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_J01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_j01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_K01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_k01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P571_L01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C02_DOOR1_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C02_DOOR1_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C03_DOOR2_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C03_DOOR2_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C03_DOOR2_A_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C03_DOOR2_A_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C03_DOOR2_B_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P415_C03_DOOR2_B_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORLEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORRIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.SimpleObject, new MyGuiPrefabHelper(MyTextsWrapperEnum.customObject));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P385_A01_TEMPLE_900M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p385_a01_temple_900m));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P384_A01_HOSPITAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p384_a01_hospital));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P383_A01_CHURCH, new MyGuiPrefabHelper(MyTextsWrapperEnum.p383_a01_church));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P334_A01_FOOD_GROW, new MyGuiPrefabHelper(MyTextsWrapperEnum.p334_a01_food_grow));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_BIO_EXP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_bio_exp));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_BIO_MACH_EXP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_bio_mach_exp));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_RECYCLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_recycle));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.LARGESHIP_KAI, new MyGuiPrefabHelper(MyTextsWrapperEnum.LargeShipKai));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.LARGESHIP_SAYA, new MyGuiPrefabHelper(MyTextsWrapperEnum.LargeShipSaya));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.LARGESHIP_ARDANT, new MyGuiPrefabHelper(MyTextsWrapperEnum.LargeShipArdant));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P541_ESCAPE_POD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_escape_pod));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P541_ESCAPE_POD_BASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_escape_pod_base));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR_BODY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_ventilator_body));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR_PROPELLER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_ventilator_propeller));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P349_A_TOWER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p349_a_tower));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P349_B_TOWER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p349_b_tower));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P349_C_TOWER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p349_c_tower));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P361_A02_HANGAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p361_a02_hangar_panel));

            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_B_FACTION, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_b_faction));
            //m_prefabTypeHelpers.Add(MyMwcObjectBuilder_Prefab_TypesEnum.P531_B_FACTION_HOLO, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_b_faction_holo));

            //foreach (MyMwcObjectBuilder_Prefab_TypesEnum prefabTypeEnum in Enum.GetValues(typeof(MyMwcObjectBuilder_Prefab_TypesEnum)))
            //{
            //    MyGuiPrefabHelper prefabGuiHelper = null;
            //    m_prefabTypeHelpers.TryGetValue(prefabTypeEnum, out prefabGuiHelper);

            //    MyCommonDebugUtils.AssertDebug(prefabGuiHelper != null);
            //}

            #endregion
        }
    }
}
