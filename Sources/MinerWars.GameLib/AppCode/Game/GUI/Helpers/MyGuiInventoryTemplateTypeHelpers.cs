using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiInventoryTemplateTypeHelpers
    {
        static Dictionary<int, MyGuiHelperBase> m_inventoryTemplateTypeHelpers = new Dictionary<int, MyGuiHelperBase>();

        //Arrays of enums values
        public static Array MyInventoryTemplateTypeValues { get; private set; }

        static MyGuiInventoryTemplateTypeHelpers()
        {
            MyMwcLog.WriteLine("MyGuiInventoryTemplateHelpers()");

            MyInventoryTemplateTypeValues = Enum.GetValues(typeof(MyMwcInventoryTemplateTypeEnum));
        }

        public static MyGuiHelperBase GetInventoryTemplateTypeHelper(MyMwcInventoryTemplateTypeEnum templateTypeEnum)
        {
            MyGuiHelperBase ret;
            if (m_inventoryTemplateTypeHelpers.TryGetValue((short)templateTypeEnum, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {
            m_inventoryTemplateTypeHelpers.Clear();
        }

        public static void LoadContent()
        {
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantBlueprint));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, new MyGuiHelperBase(MyTextsWrapperEnum.MPMerchantFrontLine));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, new MyGuiHelperBase(MyTextsWrapperEnum.MPMerchantSupport));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_1, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_1));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_2, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_2));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_3, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_3));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_4, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_4));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_5, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_5));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_6, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_6));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_7, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_7));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_8, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_8));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_9, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_9));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_Special, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMixed_Tier_Special));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_1));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_2));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_3));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_4));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_5));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_6));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_7, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_7));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_8, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_8));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_9, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_9));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_Special, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantArmy_Tier_Special));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_1, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_1));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_2, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_2));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_3, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_3));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_4, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_4));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_5, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_5));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_6, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_6));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_7, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_7));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_8, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_8));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_9, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_9));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_Special, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantMedicine_Tier_Special));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_1));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_2, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_2));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_3, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_3));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_4));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_5, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_5));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_6, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_6));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_7, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_7));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_8, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_8));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_9, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_9));
            m_inventoryTemplateTypeHelpers.Add((int)MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_Special, new MyGuiHelperBase(MyTextsWrapperEnum.MerchantTools_Tier_Special));
        }
    }
}
