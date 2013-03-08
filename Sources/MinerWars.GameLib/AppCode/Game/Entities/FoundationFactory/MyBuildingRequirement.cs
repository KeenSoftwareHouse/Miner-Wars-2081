using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Entities.FoundationFactory
{
    /// <summary>
    /// Building requirement's interface
    /// </summary>
    interface IMyBuildingRequirement
    {
        /// <summary>
        /// Checks if player meet this requirement
        /// </summary>
        /// <param name="foundationFactory">Foundation factory</param>
        /// <returns></returns>
        bool Check(MyPrefabFoundationFactory foundationFactory);

        /// <summary>
        /// Returns description of bulding requirement
        /// </summary>
        /// <returns></returns>
        StringBuilder GetDescription();
    }

    /// <summary>
    /// This requirement means, that player must have some item in FF's inventory
    /// </summary>
    class MyBuildingRequirementInventoryItem : IMyBuildingRequirement
    {
        private static readonly StringBuilder m_amountAndDescriptionSeparator = new StringBuilder("x ");
        private StringBuilder m_description;

        /// <summary>
        /// Object builder's type
        /// </summary>
        public MyMwcObjectBuilderTypeEnum ObjectBuilderType { get; set; }

        /// <summary>
        /// Object builder's Id
        /// </summary>
        public int ObjectBuilderId { get; set; }

        /// <summary>
        /// Needed amount of inventory item
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// If true, then remove it from inventory
        /// </summary>
        public bool RemoveAfterBuild { get; set; }

        /// <summary>
        /// Creates new instance of inventory item requirement
        /// </summary>
        /// <param name="objectBuilderType"></param>
        /// <param name="objectBuilderId"></param>
        /// <param name="amount"></param>
        /// <param name="removeAfterBuild"></param>
        public MyBuildingRequirementInventoryItem(MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderId, float amount, bool removeAfterBuild)
        {
            ObjectBuilderType = objectBuilderType;
            ObjectBuilderId = objectBuilderId;
            Amount = amount;
            RemoveAfterBuild = removeAfterBuild;
            m_description = new StringBuilder();
        }

        /// <summary>
        /// Checks if player meet this requirement
        /// </summary>
        /// <param name="foundationFactory">Foundation factory</param>
        /// <returns></returns>
        public bool Check(MyPrefabFoundationFactory foundationFactory)
        {
            float requiredInventoryItemAmount = foundationFactory.Player.Ship.Inventory.GetTotalAmountOfInventoryItems(ObjectBuilderType, ObjectBuilderId);
            return requiredInventoryItemAmount >= Amount;
        }

        /// <summary>
        /// Returns description of bulding requirement
        /// </summary>
        /// <returns></returns>
        public StringBuilder GetDescription()
        {
            m_description.Clear();
            m_description.Append(Amount);
            MyMwcUtils.AppendStringBuilder(m_description, m_amountAndDescriptionSeparator);
            MyMwcUtils.AppendStringBuilder(m_description, MyGuiObjectBuilderHelpers.GetGuiHelper(ObjectBuilderType, ObjectBuilderId).Description);            
            return m_description;
        }
    }

    ///// <summary>
    ///// This requirement means, that player must have builded some prefab
    ///// </summary>
    //class MyBuildingRequirementBuildedPrefab : IMyBuildingRequirement
    //{
    //    /// <summary>
    //    /// Prefabs type
    //    /// </summary>
    //    public MyMwcObjectBuilder_Prefab_TypesEnum PrefabType { get; set; }

    //    /// <summary>
    //    /// Creates new instance of builded prefab requirement
    //    /// </summary>
    //    /// <param name="prefabType"></param>
    //    public MyBuildingRequirementBuildedPrefab(MyMwcObjectBuilder_Prefab_TypesEnum prefabType)
    //    {
    //        PrefabType = prefabType;
    //    }

    //    /// <summary>
    //    /// Checks if player meet this requirement
    //    /// </summary>
    //    /// <param name="foundationFactory">Foundation factory</param>
    //    /// <returns></returns>
    //    public bool Check(MyFoundationFactory foundationFactory)
    //    {
    //        foreach (MyPrefabBase prefab in foundationFactory.PrefabContainer.GetPrefabs())
    //        {
    //            if (((MyMwcObjectBuilder_Prefab)prefab.GetObjectBuilder(true)).PrefabType == PrefabType)
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// Returns description of bulding requirement
    //    /// </summary>
    //    /// <returns></returns>
    //    public StringBuilder GetDescription()
    //    {
    //        return MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)PrefabType).Description;
    //    }
    //}

    /// <summary>
    /// This requirement means, that player must have some blueprint in checkpoint's inventory
    /// </summary>
    class MyBuildingRequirementBlueprint : IMyBuildingRequirement
    {        
        /// <summary>
        /// Blueprint's type
        /// </summary>
        public MyMwcObjectBuilder_Blueprint_TypesEnum BlueprintType { get; private set; }

        /// <summary>
        /// Creates new instance of blueprint's requirement
        /// </summary>
        /// <param name="blueprintType">Blueprint's type</param>        
        public MyBuildingRequirementBlueprint(MyMwcObjectBuilder_Blueprint_TypesEnum blueprintType)
        {
            BlueprintType = blueprintType;                        
        }

        /// <summary>
        /// Checks if player meet this requirement
        /// </summary>
        /// <param name="foundationFactory">Foundation factory</param>
        /// <returns></returns>
        public bool Check(MyPrefabFoundationFactory foundationFactory)
        {
            return MySession.Static.Inventory.Contains(MyMwcObjectBuilderTypeEnum.Blueprint, (int) BlueprintType);
        }

        /// <summary>
        /// Returns description of bulding requirement
        /// </summary>
        /// <returns></returns>
        public StringBuilder GetDescription()
        {
            return MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int) BlueprintType).Description;
        }
    }
}
