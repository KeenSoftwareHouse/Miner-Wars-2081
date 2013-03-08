using System;
using System.Collections.Generic;
using System.ComponentModel;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Entities.FoundationFactory
{
    /// <summary>
    /// Building specification for building any object in foundation factory
    /// </summary>
    class MyBuildingSpecification
    {
        /// <summary>
        /// Building requirementes collection
        /// </summary>
        public IList<IMyBuildingRequirement> BuildingRequirements { get; set; }

        /// <summary>
        /// Building time in ms
        /// </summary>
        public int BuildingTime { get; set; }

        /// <summary>
        /// Creates new instance of building specification
        /// </summary>
        public MyBuildingSpecification() : this(0, new BindingList<IMyBuildingRequirement>())
        {
        }

        /// <summary>
        /// Creates new instance of building specification
        /// </summary>
        /// <param name="buildingTime">Building time</param>
        /// <param name="buildingRequirements">Building requirements</param>
        public MyBuildingSpecification(int buildingTime, IList<IMyBuildingRequirement> buildingRequirements)
        {
            BuildingTime = buildingTime;
            BuildingRequirements = buildingRequirements;
        }                

        /// <summary>
        /// Returns if player can build this object
        /// </summary>
        /// <param name="foundationFactory">Foundation factory</param>
        /// <returns></returns>
        public bool CanBuild(MyPrefabFoundationFactory foundationFactory)
        {            
            foreach (IMyBuildingRequirement myBuildingRequirement in BuildingRequirements)
            {
                if (!myBuildingRequirement.Check(foundationFactory))
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Building specification for all objects which player can build in foundation factory
    /// </summary>
    internal static class MyBuildingSpecifications
    {
        static readonly MyBuildingSpecification[][] m_buildingSpecifications = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1][];
        static readonly IMyBuildingRequirement[] m_blueprintRequirements = new IMyBuildingRequirement[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Blueprint_TypesEnum>() + 1];
        static readonly List<IMyBuildingRequirement>[] m_blueprintRequirementDependencies = new List<IMyBuildingRequirement>[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Blueprint_TypesEnum>() + 1];

        static MyBuildingSpecifications()
        {
            MyMwcLog.WriteLine("MyBuildingSpecifications()");

            #region Blueprint's definition
            m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit]
                = new MyBuildingRequirementBlueprint(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit);
            m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit]
                = new MyBuildingRequirementBlueprint(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit);
            m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit]
                = new MyBuildingRequirementBlueprint(MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit);
            m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit]
                = new MyBuildingRequirementBlueprint(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit);
            m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit]
                = new MyBuildingRequirementBlueprint(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit);
            m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit]
                = new MyBuildingRequirementBlueprint(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit);
            m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit]
                = new MyBuildingRequirementBlueprint(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit);

            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit] = new List<IMyBuildingRequirement>();
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit].Add(m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit]);

            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit] = new List<IMyBuildingRequirement>();            
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit].AddRange(m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit]);
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit].Add(m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit]);

            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit] = new List<IMyBuildingRequirement>();
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit].AddRange(m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit]);
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit].Add(m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit]);

            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit] = new List<IMyBuildingRequirement>();
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit].AddRange(m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit]);
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit].Add(m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit]);

            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit] = new List<IMyBuildingRequirement>();
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit].AddRange(m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit]);
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit].Add(m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit]);

            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit] = new List<IMyBuildingRequirement>();
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit].AddRange(m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit]);
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit].Add(m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit]);

            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit] = new List<IMyBuildingRequirement>();
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit].AddRange(m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit]);
            m_blueprintRequirementDependencies[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit].Add(m_blueprintRequirements[(int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit]);
            #endregion

            #region prefabs
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Prefab_TypesEnum>() + 1];
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P430_A01_PASSAGE_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P430_A02_PASSAGE_40M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P424_A01_PIPE_BASE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P423_A01_PIPE_JUNCTION] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P422_A01_PIPE_TURN_90] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P421_A01_PIPE_STRAIGHT_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P421_A02_PIPE_STRAIGHT_40M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P421_A03_PIPE_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_G01_JUNCTION_6AXES] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_G02_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P410_G01_TURN_90_RIGHT_0M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_G01_STRAIGHT_1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_G02_STRAIGHT_2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_G03_STRAIGHT_3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_G04_STRAIGHT_4] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_F02_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F21_TURN_S_UP] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F22_TURN_S_LEFT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F23_TURN_S_RIGHT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F24_TURN_S_DOWN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F01_TURN_90_UP_230M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F02_TURN_90_LEFT_230M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F03_TURN_90_RIGHT_230M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F04_TURN_90_DOWN_230M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_F01_STRAIGHT_1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_F02_STRAIGHT_2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_F03_STRAIGHT_3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_E01_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_E01_STRAIGHT_1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_E02_STRAIGHT_2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_E03_STRAIGHT_3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_E04_STRAIGHT_4] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_E05_STRAIGHT_5] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P415_D01_DOORCASE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P415_D02_DOOR1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P415_D03_DOOR2_A] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P415_D03_DOOR2_B] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_D01_JUNCTION_T_HORIZONTAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_D03_JUNCTION_X_HORIZONTAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_D01_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_D01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_D02_STRAIGHT_40M_WITH_HOLE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_D03_STRAIGHT_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_D04_STRAIGHT_120M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_D05_STRAIGHT_180M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_C01_JUNCTION_T_HORIZONTAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_C01_JUNCTION_X_HORIZONTAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_C01_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_C01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_C02_STRAIGHT_40M_WITH_HOLE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_C03_STRAIGHT_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_C04_STRAIGHT_120M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_C05_STRAIGHT_180M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P415_B01_DOORCASE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P415_B02_DOOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_B01_JUNCTION_T_HORIZONTAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_B02_JUNCTION_T_VERTICAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_B02_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B21_TURN_S_UP] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B22_TURN_S_LEFT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B23_TURN_S_RIGHT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B24_TURN_S_DOWN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B11_TURN_90_UP_160M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B12_TURN_90_LEFT_160M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B13_TURN_90_RIGHT_160M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B14_TURN_90_DOWN_160M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B01_TURN_90_UP_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B02_TURN_90_LEFT_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B03_TURN_90_RIGHT_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_B04_TURN_90_DOWN_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_B01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B02_STRAIGHT_30M_YELLOW] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B04_STRAIGHT_80M_WITH_SIDE_GRATES] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B03_STRAIGHT_320M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B05_STRAIGHT_80M_WITH_SIDE_OPEN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));                                    
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_A01_JUNCTION_T_HORIZONTAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_A02_JUNCTION_T_VERTICAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_A01_ENTRANCE_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_A02_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A21_TURN_S_UP] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A22_TURN_S_LEFT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A23_TURN_S_RIGHT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A24_TURN_S_DOWN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A11_TURN_90_UP_160M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A12_TURN_90_LEFT_160M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A13_TURN_90_RIGHT_160M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A14_TURN_90_DOWN_160M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A01_TURN_90_UP_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A02_TURN_90_LEFT_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A03_TURN_90_RIGHT_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_A04_TURN_90_DOWN_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_A01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_A02_STRAIGHT_60M_WITH_HOLE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_A03_STRAIGHT_120M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_A04_STRAIGHT_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_A05_STRAIGHT_80M_WITH_EXTENSION] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P382_E01_BRIDGE5] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P382_D01_BRIDGE4] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P382_C01_BRIDGE3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P382_B01_BRIDGE2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P382_A01_BRIDGE1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING4] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P381_A01_BUILDING1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P362_A01_SHORT_DISTANCE_ANTENNA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P361_A01_LONG_DISTANCE_ANTENNA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P351_A01_WEAPON_MOUNT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_REFINERY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P344_A01_CONTAINER_ARM_FILLED] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P344_A02_CONTAINER_ARM_EMPTY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P343_A01_ORE_STORAGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P342_A01_LOADING_BAY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P341_B01_OPEN_DOCK_VARIATION1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P341_B02_OPEN_DOCK_VARIATION2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P341_A02_OPEN_DOCK_VARIATION2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P333_A01_HYDROPONIC_BUILDING] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P332_A01_OXYGEN_STORAGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P331_A01_OXYGEN_GENERATOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P324B01_FUEL_STORAGE_B] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P324A01_FUEL_STORAGE_A] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P323A01_FUEL_GENERATOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P322A01_BATTERY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P321B01_NUCLEAR_REACTOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P321A01_SOLAR_PANEL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P312A01_SHORT_TERM_THRUSTER_LATITUDE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P312A02_SHORT_TERM_THRUSTER_LATERAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P311A01_LONG_TERM_THRUSTER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A01_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A02_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A03_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A04_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A05_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A06_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A07_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A08_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A09_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A10_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A11_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A12_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A13_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A14_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A15_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A16_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A17_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P231A18_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221E01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221D01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221C01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221B01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221A01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211G01_PANEL_120MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211H01_PANEL_535MX130M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211G02_PANEL_60MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211G03_PANEL_60MX30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211F01_PANEL_120MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211F02_PANEL_60MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211F03_PANEL_60MX30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211E01_PANEL_120MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211E02_PANEL_60MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211E03_PANEL_60MX30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211D01_PANEL_120MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211D02_PANEL_60MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211D03_PANEL_60MX30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211C01_PANEL_120MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211C02_PANEL_60MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211C03_PANEL_60MX30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211B01_PANEL_120MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211B03_PANEL_60MX30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211A01_PANEL_120MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211A02_PANEL_60MX60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P211A03_PANEL_60MX30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142B01_CAGE_EMPTY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142B02_CAGE_HALFCUT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142B03_CAGE_WITH_CORNERS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142B11_CAGE_PILLAR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142B12_CAGE_EDGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142A01_CAGE_EMPTY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142A02_CAGE_HALFCUT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142A03_CAGE_WITH_CORNERS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142A11_CAGE_PILLAR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P142A12_CAGE_EDGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141B01_THICK_FRAME_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141B02_THICK_FRAME_STRAIGHT_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141B11_THICK_FRAME_EDGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141B12_THICK_FRAME_CORNER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141B31_THICK_FRAME_JOINT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141A01_THICK_FRAME_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141A02_THICK_FRAME_STRAIGHT_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141A11_THICK_FRAME_EDGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141A12_THICK_FRAME_CORNER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P141A31_THICK_FRAME_JOINT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120J01_J_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130J02_J_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120I01_I_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130I02_I_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120H01_H_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130H02_H_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120G01_G_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130G02_G_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120F01_F_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130F02_F_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120E01_E_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130E02_E_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120D01_D_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130D02_D_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120C01_C_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130C02_C_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120B01_B_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130B02_B_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A01_A_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130A02_A_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120D01_D_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120D02_D_STRAIGHT_40M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120C01_C_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120C02_C_STRAIGHT_40M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120B01_B_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120B02_B_STRAIGHT_40M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A01_STRONG_LATTICE_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A02_STRONG_LATTICE_STRAIGHT_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A03_STRONG_LATTICE_STRAIGHT_120M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A21_STRONG_LATTICE_JUNCTION_T_STRONG] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A22_STRONG_LATTICE_JUNCTION_T_WEAK] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A23_STRONG_LATTICE_JUNCTION_T_ROTATED] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A51_STRONG_TO_WEAK_LATTICE_2TO1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A52_STRONG_TO_WEAK_LATTICE_1TO2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P120A61_WEAK_LATTICE_JUNCTION_T_ROTATED] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110B01_LATTICE_BEAM_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110B02_LATTICE_BEAM_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_60M_WITH_PANELS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110B21_LATTICE_BEAM_JUNCTION_T_STRONG] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110B22_LATTICE_BEAM_JUNCTION_T_WEAK] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110B31_LATTICE_BEAM_JOINT_HORIZONTAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110B32_LATTICE_BEAM_JOINT_VERTICAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A01_SOLID_BEAM_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A02_SOLID_BEAM_STRAIGHT_20M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A03_SOLID_BEAM_STRAIGHT_40M_WITH_HOLE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A04_SOLID_BEAM_STRAIGHT_40M_LATTICE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A05_SOLID_BEAM_STRAIGHT_80M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A11_SOLID_BEAM_JUNCTION_X_STRONG] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A12_SOLID_BEAM_JUNCTION_X_WEAK] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A13_SOLID_BEAM_JUNCTION_X_ROTATED] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A21_SOLID_BEAM_JUNCTION_T_STRONG] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A22_SOLID_BEAM_JUNCTION_T_WEAK] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A23_SOLID_BEAM_JUNCTION_T_ROTATED] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A31_SOLID_BEAM_JOINT_HORIZONTAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A32_SOLID_BEAM_JOINT_VERTICAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A33_SOLID_BEAM_JOINT_LONGITUDINAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P110A41_SOLID_BEAM_SUPERJOINT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));                        
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A01_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A02_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A03_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A04_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A05_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A06_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A07_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A08_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A09_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A10_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A11_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A12_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A14_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A15_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P511_A16_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A01_SIGN1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A02_SIGN2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A03_SIGN3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A04_SIGN4] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A05_SIGN5] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A06_SIGN6] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A07_SIGN7] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A08_SIGN8] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A09_SIGN9] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A10_SIGN10] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A11_SIGN11] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P531_A12_SIGN12] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ADMINISTRATIVE_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARMORY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_L] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_R] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_STR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_CARGO_BAY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMAND_CENTER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMERCIAL_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMUNICATIONS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_DEFENSES] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_DOCKS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EMERGENCY_EXIT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ENGINEERING_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EXIT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EXPERIMENTAL_LABS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_FOUNDRY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_HABITATS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_HANGARS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_INDUSTRIAL_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_LANDING_BAY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MAINTENANCE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MILITARY_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MINES] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ORE_PROCESSING] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_OUTER_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_PRISON] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_PUBLIC_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_REACTOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_RESEARCH] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_RESTRICTED_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_SECURITY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_STORAGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_TECHNICAL_AREA] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_TRADE_PORT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221_A02_CHAMBER_V2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221_B02_CHAMBER_V2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221_C02_CHAMBER_V2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221_D02_CHAMBER_V2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P221_E02_CHAMBER_V2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_A01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_A02_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_B01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_B02_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_C01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_C02_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_D01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_D02_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_E01_STRAIGHT_10M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P130_E02_STRAIGHT_30M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P321_B01_SOLAR_PANEL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P321_C01_SOLAR_PANEL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P321_D01_BIG_SOLAR_PANEL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_D02_JUNCTION_T_VERTICAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P413_D04_JUNCTION_X_VERTICAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F11_TURN_90_UP_230M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F12_TURN_90_LEFT_230M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F13_TURN_90_RIGHT_230M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P412_F14_TURN_90_DOWN_230M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_F04_STRAIGHT_4] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_F05_STRAIGHT_5] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P411_G05_STRAIGHT_5] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_F01_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P414_G01_ENTRANCE_60M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_A01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_B01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_BOX01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_BOX02_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_C01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_D01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_E01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_F01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_G01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_H01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_I01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_J01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_K01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P571_L01_TRAFFIC_SIGN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.SimpleObject] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            //m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.AsteroidPrefabTest] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));                        
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B06_STRAIGHT_180M_CONCRETE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B06_STRAIGHT_200M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B07_STRAIGHT_180M_BLUE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B09_STRAIGHT_30M_GRAY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B11_STRAIGHT_220M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B12_STRAIGHT_160M_DARK_METAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.p411_B13_STRAIGHT_100M_TUBE_INSIDE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P385_A01_TEMPLE_900M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P383_A01_CHURCH] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P334_A01_FOOD_GROW] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_BIO_EXP] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_BIO_MACH_EXP] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_RECYCLE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));            
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P541_ESCAPE_POD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P541_ESCAPE_POD_BASE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR_BODY] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR_PROPELLER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P349_A_TOWER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P349_B_TOWER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int) MyMwcObjectBuilderTypeEnum.Prefab][(int) MyMwcObjectBuilder_Prefab_TypesEnum.P349_C_TOWER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));            
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_B_FACTION] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_B_FACTION_HOLO] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ARMOR_HULL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_LARGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_MEDIUM] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_SMALL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_MEDIUM] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_SMALL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_MEDIUM] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_SMALL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_MEDIUM] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_SMALL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_MEDIUM] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_SMALL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221F01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221G01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221H01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221J01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221K01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_C01_CLOSED_DOCK_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_LARGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_LARGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_LARGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_LARGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212F01_PANEL_LARGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P391_ALIEN_GATE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit));                        
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_D03_HOSPITAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));            
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_D05_FOOD_GROW] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));                        
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_CORNER_25M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_S_45M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_180] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_45] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_90] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CONNECTION_BOX] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));

            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P361_B01_LONG_DISTANCE_ANTENNA_BIG] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.FOURTH_REICH_WRECK] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P344_A03_CONTAINER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_CORNER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_EDGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_HOLE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit));

            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A03_SHELF_1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A02_SHELF_1X2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A01_SHELF_1X3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));

            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P611_ASTEROID_PART_A] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P611_ASTEROID_PART_B] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P363_A01_BIG_ANTENNA_300M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130D02_D_STRAIGHT_300M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130J01_J_STRAIGHT_300M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C02_C_STRAIGHT_400M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_420M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_420M_WITH_PANELS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P363_A01_BIG_ANTENNA_1500M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4221_A01_COOLING_DEVICE_WALL_340X400] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4222_A01_PIPES_CONNECTOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4223_A01_OPEN_PIPE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P311B01_LONG_TERM_THRUSTER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221K01_CHAMBER_V2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));

            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_BARREL_BIOHAZARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_BARREL_BIOHAZARD_2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_NUKE_BARREL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_RED_BARREL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL_2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_A] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_B] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_C] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            //m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_D] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            //m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_E] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CANNONBALL_CAPSULE_1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.GATTLING_AMMO_BELT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PACK01] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PACK02] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PLAZMA01] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_STACK_BIOCHEM01] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_O2_BARREL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.NUCLEAR_WARHEAD_CLOSED] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.NUCLEAR_WARHEAD_OPEN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));            
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_NUKE_BARREL_1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL_3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221L01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));

            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_BOTTOM_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_CENTER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_TOP_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321E01_SOLAR_PANEL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A06_SOLID_BEAM_STRAIGHT_420M] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P311B01_CUT_THRUSTER] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312B01_CUT_THRUSTER_LATERAL] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312B02_CUT_THRUSTER_LATITUDE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ALIEN_DETECTOR_UNIT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING5] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING6] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING7] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221N01_CHAMBER_V1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_D_MEDIC_CROSS] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ALIEN_ARTEFACT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BOMB] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.RAIL_GUN] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_RECYCLE_SPHERE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.PRISON] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A17_BILLBOARD_PORTRAIT_1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A18_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A19_BILLBOARD] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.DEAD_PILOT] = new MyBuildingSpecification(100, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.Prefab][(ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A17_BILLBOARD_PORTRAIT_2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            #endregion

            #region ammo
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Armor_Piercing_Incendiary]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_SAPHEI]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Armor_Piercing_Incendiary]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Armor_Piercing_Incendiary]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Ammo][(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit));
            #endregion

            #region small ship weapons
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.SmallShip_Weapon][(int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            #endregion

            #region Create Prefab Light Helpers (also included in Prefab helpers)
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLight] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabLight_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.DEFAULT_LIGHT_0]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A01_LIGHT1]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A02_LIGHT2]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A03_LIGHT3]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLight][(int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A04_LIGHT4]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit) );            
            #endregion

            #region Prefab particles
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabParticles_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.DEFAULT_PARTICLE_PREFAB_0]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_A01_PARTICLES]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_B01_PARTICLES]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_C01_PARTICLES]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabParticles][(int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_D01_PARTICLES]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );            
            #endregion

            #region Prefab sound
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabSound] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabSound_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.DEFAULT_SOUND_PREFAB_0]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_A01_SOUND]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_B01_SOUND]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_C01_SOUND]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_D01_SOUND]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.MOTHERSHIP_SOUND]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabSound][(int)MyMwcObjectBuilder_PrefabSound_TypesEnum.MADELINE_MOTHERSHIP_SOUND]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));            
            #endregion

            #region Prefab kinematic
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_Blueprint_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR2] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR3] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR4] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematic][(int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_A01_DOORCASE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));            
            #endregion

            #region prefab largeship
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabLargeShip_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_ARDANT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_KAI]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_SAYA]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.FOURTH_REICH_MOTHERSHIP]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.FOURTH_REICH_MOTHERSHIP_B]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.RUS_MOTHERSHIP]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.RUSSIAN_MOTHERSHIP_HUMMER]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_BODY]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_ENGINE]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_LARGE_LEFT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_LARGE_RIGHT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_SMALL_LEFT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_SMALL_RIGHT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_LARGE_LEFT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_LARGE_RIGHT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL_LEFT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL_RIGHT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL02_LEFT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeShip][(int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL02_RIGHT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            #endregion

            #region prefab largeship weapons
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_CIWS]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon][(int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit) );                   
            #endregion

            #region prefab hangars
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabHangar] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabHangar_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabHangar][(int)MyMwcObjectBuilder_PrefabHangar_TypesEnum.VENDOR]
                = new MyBuildingSpecification(7000,  GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit) );
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabHangar][(int)MyMwcObjectBuilder_PrefabHangar_TypesEnum.HANGAR]
                = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            #endregion

            #region prefab kinematic part
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C02_DOOR1_RIGHT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C02_DOOR1_LEFT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A_LEFT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A_RIGHT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B_LEFT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B_RIGHT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORLEFT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORRIGHT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_A02_DOOR_LEFT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabKinematicPart][(int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_A02_DOOR_RIGHT] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit));
            #endregion

            #region prefab foundation factory
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory][(int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT]
                = new MyBuildingSpecification(7000, new BindingList<IMyBuildingRequirement>());
            #endregion

            #region prefab generator
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator] = new MyBuildingSpecification[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_PrefabGenerator_TypesEnum>() + 1];
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C01_INERTIA_GENERATOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C03_CENTRIFUGE] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C04_BOX_GENERATOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C05_CENTRIFUGE_BIG] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C02_GENERATOR_WALL_BIG] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C06_INERTIA_GENERATOR_B] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));
            m_buildingSpecifications[(int)MyMwcObjectBuilderTypeEnum.PrefabGenerator][(int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C07_GENERATOR] = new MyBuildingSpecification(7000, GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit));            
            #endregion            

            foreach (MyMwcObjectBuilder_Blueprint_TypesEnum blueprint in Enum.GetValues(typeof(MyMwcObjectBuilder_Blueprint_TypesEnum)))
            {
                MyCommonDebugUtils.AssertDebug(GetBlueprintRequirements(blueprint) != null);
            }

            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.Prefab);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.PrefabLight);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.PrefabParticles);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.PrefabSound);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.PrefabKinematic);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.PrefabLargeShip);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.PrefabHangar);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo);
            AssertObjectBuilder(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon);
        }

        // this method is there because we need load and check asserts when the game started
        public static void Check()
        {
            MyMwcLog.WriteLine("MyBuildingSpecifications.Check()");
        }

        private static void AssertObjectBuilder(MyMwcObjectBuilderTypeEnum objectBuilderType)
        {
            // check if there are valid ObjectBuilderIds for concrete ObjectBuilderType
            var buildingSpecificationsForType = m_buildingSpecifications[(int)objectBuilderType];
            MyCommonDebugUtils.AssertDebug(buildingSpecificationsForType != null);
            for (int objectBuilderId = 0; objectBuilderId < buildingSpecificationsForType.Length; objectBuilderId++)
            {
                var buildingSpecification = buildingSpecificationsForType[objectBuilderId];
                if (buildingSpecification != null)
                {
                    MyCommonDebugUtils.AssertDebug(MyMwcObjectBuilder_Base.IsObjectBuilderIdValid(objectBuilderType, objectBuilderId));
                }
            }

            // check if there are all objectbuilder's types and ids
            int[] objectBuilderIDs = MyMwcObjectBuilder_Base.GetObjectBuilderIDs(objectBuilderType);
            foreach (int objectBuilderId in objectBuilderIDs)
            {
                MyCommonDebugUtils.AssertDebug(buildingSpecificationsForType[objectBuilderId] != null);
            }            
        }

        private static List<IMyBuildingRequirement> GetBlueprintRequirements(MyMwcObjectBuilder_Blueprint_TypesEnum blueprintType)
        {
            try
            {
                return m_blueprintRequirementDependencies[(int)blueprintType];
            }
            catch
            {
                return null;                
            }
        }

        public static MyBuildingSpecification GetBuildingSpecification(MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderId)
        {
            try
            {
                return m_buildingSpecifications[(int)objectBuilderType][objectBuilderId];
            }
            catch
            {
                return null;
            }
        }

        public static MyBuildingSpecification GetBuildingSpecification(MyMwcObjectBuilder_Base objectBuilder)
        {
            int objectBuilderId = objectBuilder.GetObjectBuilderId() != null
                                      ? objectBuilder.GetObjectBuilderId().Value
                                      : 0;
            return GetBuildingSpecification(objectBuilder.GetObjectBuilderType(), objectBuilderId);
        }
    }
}
