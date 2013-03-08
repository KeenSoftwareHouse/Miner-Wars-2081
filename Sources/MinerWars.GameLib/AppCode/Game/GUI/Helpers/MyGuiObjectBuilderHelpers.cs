using System;
using System.Collections.Generic;
using System.Linq;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.LargeShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System.Text;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    static class MyGuiObjectBuilderHelpers
    {
        //Dictionary for helpers        
        static MyGuiHelperBase[][] m_buildTypeHelpers = new MyGuiHelperBase[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1][];

        static List<Tuple<MyMwcObjectBuilder_Prefab_TypesEnum, MyGuiHelperBase>> m_onTheFlyPrefabHelpers = new List<Tuple<MyMwcObjectBuilder_Prefab_TypesEnum, MyGuiHelperBase>>();

        static MyGuiObjectBuilderHelpers()
        {
            //LoadContent();
        }

        public static MyGuiHelperBase GetGuiHelper(MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderID)
        {            
            return m_buildTypeHelpers[(int) objectBuilderType][objectBuilderID];
        }

        public static MyGuiHelperBase GetGuiHelper(MyMwcObjectBuilder_Base objectBuilder) 
        {
            MyMwcObjectBuilderTypeEnum objectBuilderType = objectBuilder.GetObjectBuilderType();
            int objectBuilderId = objectBuilder.GetObjectBuilderId() != null ? objectBuilder.GetObjectBuilderId().Value : 0;
            return GetGuiHelper(objectBuilderType, objectBuilderId);
        }

        public static void AddGuiHelper(MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderID, MyGuiHelperBase guiHelper)
        {            
            if (m_buildTypeHelpers[(int)objectBuilderType] == null)
            {
                m_buildTypeHelpers[(int)objectBuilderType] = new MyGuiHelperBase[MyMwcObjectBuilder_Base.GetObjectBuilderIDs(objectBuilderType).Max() + 1];
            }
            m_buildTypeHelpers[(int) objectBuilderType][objectBuilderID] = guiHelper;
        }

        public static void LoadContent()
        {            
            #region Create small debris helpers
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Cistern,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris1,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris2,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris3,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris4,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris5,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris6,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris7,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris8,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris9,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris10,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris11,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris12,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris13,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris14,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris15,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris16,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris17,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris18,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris19,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris20,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris21,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris22,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris23,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris24,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris25,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris26,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris27,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris28,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris29,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris30,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris31,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris32_pilot,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.pipe_bundle,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BundleOfPipes", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.pipe_bundle));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_1,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\StandardContainer01", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.Standard_Container_1));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\StandardContainer02", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.Standard_Container_2));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_3,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\StandardContainer03", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.Standard_Container_3));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_4,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\StandardContainer04", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.Standard_Container_4));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallDebris, (int)MyMwcObjectBuilder_SmallDebris_TypesEnum.UtilityVehicle_1,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\UtilityVehicle01", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.UtilityVehicle_1));
            #endregion

            #region LargeDebrisField
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeDebrisField, (int)MyMwcObjectBuilder_LargeDebrisField_TypesEnum.Debris84,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Create large ship helpers
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip, (int)MyMwcObjectBuilder_LargeShip_TypesEnum.KAI,
                new MyGuiLargeShipHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeShipKai", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.LargeShipKai));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip, (int)MyMwcObjectBuilder_LargeShip_TypesEnum.MOTHERSHIP_SAYA,
               new MyGuiLargeShipHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeShipSaya", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.LargeShipSaya));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip, (int)MyMwcObjectBuilder_LargeShip_TypesEnum.JEROMIE_INTERIOR_STATION,
                new MyGuiLargeShipHelper(null,
                    MyTextsWrapperEnum.Largeship));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip, (int)MyMwcObjectBuilder_LargeShip_TypesEnum.CRUISER_SHIP,
                new MyGuiLargeShipHelper(null,
                    MyTextsWrapperEnum.Largeship));            

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip, (int)MyMwcObjectBuilder_LargeShip_TypesEnum.ARDANT,
                new MyGuiLargeShipHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeShipArdant", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.LargeShipArdant));
            #endregion

            #region Create engine helpers

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineChemical1", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineChemical1));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_2,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineChemical2", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineChemical2));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_3,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineChemical3", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineChemical3));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_4,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineChemical4", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineChemical4));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_5,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineChemical5", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineChemical5));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_1,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineNuclear1", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineNuclear1));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_2,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineNuclear2", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineNuclear2));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_3,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineNuclear3", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineNuclear3));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_4,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineNuclear4", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineNuclear4));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_5,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EngineNuclear5", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EngineNuclear5));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EnginePowerCells1", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EnginePowerCells1));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_2,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EnginePowerCells2", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EnginePowerCells2));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_3,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EnginePowerCells3", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EnginePowerCells3));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_4,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EnginePowerCells4", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EnginePowerCells4));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_5,
                new MyGuiSmallShipHelperEngine(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\EnginePowerCells5", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.EnginePowerCells5));

            #endregion

            #region Create small ship armor helpers
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic,
                new MyGuiSmallShipHelperArmor(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ArmorBasic", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.BasicArmor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Advanced,
                new MyGuiSmallShipHelperArmor(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ArmorAdvanced", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AdvancedArmor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.High_Endurance,
                new MyGuiSmallShipHelperArmor(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ArmorHighEndurance", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.HighEnduranceArmor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Solar_Wind,
                new MyGuiSmallShipHelperArmor(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ArmorSolarWind", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.SolarWindArmor));
            #endregion

            #region SmallShip_Player
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip01", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Liberator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip01, MyTextsWrapperEnum.SmallShipMinerShip01Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip02", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Enforcer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip02, MyTextsWrapperEnum.SmallShipMinerShip02Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip03", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Kammler", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip03, MyTextsWrapperEnum.SmallShipMinerShip03Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip04", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Gettysburg", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip04, MyTextsWrapperEnum.SmallShipMinerShip04Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip05", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Virginia", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip05, MyTextsWrapperEnum.SmallShipMinerShip05Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.BAER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip06", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Baer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip06, MyTextsWrapperEnum.SmallShipMinerShip06Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip07", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hewer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip07, MyTextsWrapperEnum.SmallShipMinerShip07Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip08", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Razorclaw", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip08, MyTextsWrapperEnum.SmallShipMinerShip08Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip09", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Greiser", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip09, MyTextsWrapperEnum.SmallShipMinerShip09Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip10", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Tracer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip10, MyTextsWrapperEnum.SmallShipMinerShip10Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Jacknife", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Jacknife", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipJackKnife, MyTextsWrapperEnum.SmallShipJackKnifeName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.DOON,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Doon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Doon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipDoon, MyTextsWrapperEnum.SmallShipDoonName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Hammer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hammer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipHammer, MyTextsWrapperEnum.SmallShipHammerName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ORG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_ORG", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\ORG", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipOrg, MyTextsWrapperEnum.SmallShipOrgName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.YG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_YG_Closed", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\YG_Closed", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipYg, MyTextsWrapperEnum.SmallShipYgName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Hawk", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hawk", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipHawk, MyTextsWrapperEnum.SmallShipHawkName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Phoenix", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Phoenix", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipPhoenix, MyTextsWrapperEnum.SmallShipPhoenixName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Leviathan", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Leviathan", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipLeviathan, MyTextsWrapperEnum.SmallShipLeviathanName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Rockheater", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Rockheater", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipRockheater, MyTextsWrapperEnum.SmallShipRockheaterName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_SteelHead", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\SteelHead", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipSteelHead, MyTextsWrapperEnum.SmallShipSteelHeadName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Talon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Talon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipTalon, MyTextsWrapperEnum.SmallShipTalonName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Player, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Stanislav", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Stanislav", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipStanislav, MyTextsWrapperEnum.SmallShipStanislavName));
            #endregion

            #region SmallShip_Bot
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip01", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Liberator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip01, MyTextsWrapperEnum.SmallShipMinerShip01Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip02", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Enforcer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip02, MyTextsWrapperEnum.SmallShipMinerShip02Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip03", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Kammler", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip03, MyTextsWrapperEnum.SmallShipMinerShip03Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip04", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Gettysburg", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip04, MyTextsWrapperEnum.SmallShipMinerShip04Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip05", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Virginia", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip05, MyTextsWrapperEnum.SmallShipMinerShip05Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.BAER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip06", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Baer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip06, MyTextsWrapperEnum.SmallShipMinerShip06Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip07", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hewer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip07, MyTextsWrapperEnum.SmallShipMinerShip07Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip08", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Razorclaw", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip08, MyTextsWrapperEnum.SmallShipMinerShip08Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip09", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Greiser", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip09, MyTextsWrapperEnum.SmallShipMinerShip09Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip10", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Tracer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip10, MyTextsWrapperEnum.SmallShipMinerShip10Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Jacknife", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Jacknife", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipJackKnife, MyTextsWrapperEnum.SmallShipJackKnifeName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.DOON,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Doon", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Doon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipDoon, MyTextsWrapperEnum.SmallShipDoonName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Hammer", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hammer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipHammer, MyTextsWrapperEnum.SmallShipHammerName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ORG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_ORG", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\ORG", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipOrg, MyTextsWrapperEnum.SmallShipOrgName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.YG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_YG_Closed", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\YG_Closed", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipYg, MyTextsWrapperEnum.SmallShipYgName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Hawk", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hawk", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipHawk, MyTextsWrapperEnum.SmallShipHawkName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Phoenix", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Phoenix", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipPhoenix, MyTextsWrapperEnum.SmallShipPhoenixName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Leviathan", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Leviathan", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipLeviathan, MyTextsWrapperEnum.SmallShipLeviathanName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Rockheater", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Rockheater", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipRockheater, MyTextsWrapperEnum.SmallShipRockheaterName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_SteelHead", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\SteelHead", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipSteelHead, MyTextsWrapperEnum.SmallShipSteelHeadName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Talon", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Talon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipTalon, MyTextsWrapperEnum.SmallShipTalonName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Bot, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Stanislav", flags: TextureFlags.IgnoreQuality),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Stanislav", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipStanislav, MyTextsWrapperEnum.SmallShipStanislavName));
            #endregion

            #region Sector
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Sector, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region VoxelMap
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.VoxelMap, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region VoxelMap_MergeMaterial
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.VoxelMap_MergeMaterial, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region VoxelMap_MergeContent
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.VoxelMap_MergeContent, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region VoxelMap_Neighbour
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region VoxelHand_Sphere
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.VoxelHand_Sphere, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region VoxelHand_Box
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.VoxelHand_Box, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region VoxelHand_Cuboid
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.VoxelHand_Cuboid, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region VoxelHand_Cylinder
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.VoxelHand_Cylinder, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Create Small Ship Ammo Helpers
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutomaticRifleWithSilencerHighVelocity", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoAutomaticRifleWithSilencerHighVelocity));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_SAPHEI,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutomaticRifleWithSilencerSAPHEI", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoAutomaticRifleWithSilencerSAPHEI));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutomaticRifleWithSilencerBioChem", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoAutomaticRifleWithSilencerBioChem));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoSniperHighVelocity", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoSniperHighVelocity));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoSniperSAPHEI", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoSniperSAPHEI));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoSniperBioChem", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoSniperBioChem));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutocannonBasic", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoAutocannonBasic));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutocannonHighVelocity", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoAutocannonHighVelocity));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Armor_Piercing_Incendiary,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutocannonArmorPiercingIncendiary", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoAutocannonArmorPiercingIncendiary));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutocannonSAPHEI", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoAutocannonSAPHEI));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutocannonBioChem", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoAutocannonBioChem));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoMachineGunBasic", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoMachineGunBasic));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoMachineGunHighVelocity", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoMachineGunHighVelocity));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Armor_Piercing_Incendiary,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoMachineGunArmorPiercingIncendiary", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoMachineGunArmorPiercingIncendiary));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoMachineGunSAPHEI", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoMachineGunSAPHEI));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoMachineGunBioChem", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoMachineGunBioChem));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoShotgunBasic", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoShotgunBasic));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoShotgunHighVelocity", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoShotgunHighVelocity));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoShotgunExplosive", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoShotgunExplosive));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoShotgunArmorPiercing", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoShotgunArmorPiercing));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoCannonBasic", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoCannonBasic));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoCannonHighVelocity", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoCannonHighVelocity));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Armor_Piercing_Incendiary,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoCannonArmorPiercingIncendiary", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoCannonArmorPiercingIncendiary));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoCannonSAPHEI", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoCannonSAPHEI));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoCannonProximityExplosive", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoCannonProximityExplosive));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoCannonTunnelBuster", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoCannonTunnelBuster));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherAsteroidKiller", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherAsteroidKiller));


            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherDecoyFlare", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherDecoyFlare));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherDirectionalExplosive", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherDirectionalExplosive));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherFlashBomb", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherFlashBomb));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherGravityBomb", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherGravitationBomb));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherHologram", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherHologram));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherIlluminatingShell", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherIlluminatingShell));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherMineBasic", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherMineBasic, MyInventoryAmountTextAlign.BottomRight));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherMineSmart", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherMineSmart, MyInventoryAmountTextAlign.BottomRight));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherRemoteBomb", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherRemoteBomb));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherRemoteCamera", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherRemoteCamera));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherSmokeBomb", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherSmokeBomb));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherSphereExplosive", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherSphereExplosive));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherTimeBomb", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoUniversalLauncherTimeBomb));


            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoGuidedMissileVisualDetection", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoGuidedVisualDetection));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoGuidedMissileEngineDetection", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoGuidedEngineDetection));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoGuidedMissileRadarDetection", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoGuidedRadarDetection));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo,(int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic,
               new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoGuidedMissileBasic", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.AmmoMissileBasic));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem,
                   new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherMineBioChem", flags: TextureFlags.IgnoreQuality),
                       MyTextsWrapperEnum.AmmoUniversalLauncherMineBioChem));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem,
                   new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoGuidedMissileBioChem", flags: TextureFlags.IgnoreQuality),
                       MyTextsWrapperEnum.AmmoMissileBioChem));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem,
                   new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoCannonBioChem", flags: TextureFlags.IgnoreQuality),
                       MyTextsWrapperEnum.AmmoCannonBiochem));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP,
               new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoAutocannonEMP", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.AmmoAutocannonEMP));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP,
               new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoSniperEMP", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.AmmoSniperEMP));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP,
               new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoMachineGunEMP", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.AmmoMachineGunEMP));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP,
               new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoGuidedMissileEMP", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.AmmoMissileEMP));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP,
               new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoCannonEMP", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.AmmoCannonEMP));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb,
               new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\AmmoUniversalLauncherEMPBomb", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.AmmoUniversalLauncherEMPBomb));
            #endregion

            #region LargeShip_Weapon
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.CIWS,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.MachineGun,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.Autocannon,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.MissileLauncherType4Basic,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.MissileLauncherType6Basic,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.MissileLauncherType9Basic,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.MissileLauncherType4Guided,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.MissileLauncherType6Guided,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Weapon, (int)MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.MissileLauncherType9Guided,
            //    new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Create Small Ship Weapon Helpers

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponAutocannon", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponAutocannon));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponAutomaticRifleWithSilencer", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponAutomaticRifleWithSilencer));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponDrillingDeviceCrusher", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponDrillingDeviceCrusher));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponDrillingDeviceLaser", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponDrillingDeviceLaser));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponDrillingDeviceNuclear", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponDrillingDeviceNuclear));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponDrillingDeviceSaw", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponDrillingDeviceSaw));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponDrillingDeviceThermal", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponDrillingDeviceThermal));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponDrillingDevicePressure", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponDrillingDevicePressure));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponGuidedMissileLauncher", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponGuidedMissileLauncher));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponHarvestingDevice", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponHarvestingDevice));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponMachineGun", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponMachineGun));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponCannon", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponCannon));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponShotgun", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponShotgun));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponSniper", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponSniper));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponUniversalLauncherBack", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponUniversalLauncherBack));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front,
                new MyGuiSmallShipHelperWeapon(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\WeaponUniversalLauncherFront", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.WeaponUniversalLauncherFront));
            #endregion

            #region Create Static Asteroid Helpers

            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid,(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_A, new MyGuiAsteroidHelper(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Jeromie", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.StaticAsteroidContextMenuCaption));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid40000m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_B, 
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_A,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_B,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_C,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_D,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid, (int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_E,
                new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));

            /*
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid,(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.STATIC_ASTEROID_02, new MyGuiAsteroidHelper(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\DeformedSphereSmall", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.StaticAsteroid02));
            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.StaticAsteroid,(int)MyMwcObjectBuilder_StaticAsteroid_TypesEnum.STATIC_ASTEROID_03, new MyGuiAsteroidHelper(
                MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\DeformedSphereSmallest", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.StaticAsteroid03));
             */

            #endregion

            #region Meteor
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Meteor, (int)MyMwcObjectBuilder_Meteor_TypesEnum.DEFAULT, new MyGuiAsteroidHelper(null, MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Create Small Ship Tool helpers

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REAR_CAMERA,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRearCamera", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolRearCamera));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.LASER_POINTER,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolLaserPointer", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolLaserPointer));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.AUTO_TARGETING,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolAutoTargeting", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolAutoTargeting));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NIGHT_VISION,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolNightVision", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolNightVision));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolNanoRepairTool", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolNanoRepairTool));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.MEDIKIT,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolMedikit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolMedikit));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.XRAY,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolXRay", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolXRay));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ANTIRADIATION_MEDICINE,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolAntiradiationMedicine", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolAntiradiationMedicine));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRadarJammer", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolRadarJammer));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.PERFORMANCE_ENHANCING_MEDICINE,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolPerformanceEnhancingMedicine", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolPerformanceEnhancingMedicine));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_ENHANCING_MEDICINE,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolHealthEnhancingMedicine", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolHealthEnhancingMedicine));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_FUEL_CONTAINER_DISABLED,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolExtraFuelContainer", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolExtraFuelContainer));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_ELECTRICITY_CONTAINER,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolExtraFuelContainer", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolExtraElectricityContainer));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.EXTRA_OXYGEN_CONTAINER_DISABLED,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolExtraOxygenContainer", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolExtraOxygenContainer));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_CONVERTER,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolOxygenConverter", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolOxygenConverter));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_CONVERTER,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolFuelConverter", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolFuelConverter));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.SOLAR_PANEL,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolSolarPanel", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolSolarPanel));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.BOOBY_TRAP,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolBoobyTrap", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolBoobyTrap));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.SENSOR,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolSensor", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolSensor));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REMOTE_CAMERA,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRemoteCamera", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolRemoteCamera));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REMOTE_CAMERA_ON_DRONE,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRemoteCameraOnDrone", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolRemoteCameraOnDrone));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ALIEN_OBJECT_DETECTOR,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolAlienObjectDetector", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ToolAlienObjectDetector));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_UNUSED, new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRadar", flags: TextureFlags.IgnoreQuality),
                     MyTextsWrapperEnum.Radar1));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Radar, (int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRadar", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Radar1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Radar, (int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_2,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRadar", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Radar2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Radar, (int)MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_3,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRadar", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Radar3));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_KIT,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolHealthKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.HealthKit));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REPAIR_KIT,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolRepairKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.RepairKit));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_KIT,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolOxygenKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OxygenKit));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_KIT,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolFuelKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.FuelKit));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ELECTRICITY_KIT,
                new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\ToolFuelKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.ElectricityKit));

            #endregion

            #region Create Small Ship Helpers

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip01", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Liberator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip01, MyTextsWrapperEnum.SmallShipMinerShip01Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip02", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Enforcer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip02, MyTextsWrapperEnum.SmallShipMinerShip02Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip03", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Kammler", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip03, MyTextsWrapperEnum.SmallShipMinerShip03Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip04", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Gettysburg", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip04, MyTextsWrapperEnum.SmallShipMinerShip04Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip05", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Virginia", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip05, MyTextsWrapperEnum.SmallShipMinerShip05Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.BAER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip06", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Baer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip06, MyTextsWrapperEnum.SmallShipMinerShip06Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip07", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hewer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip07, MyTextsWrapperEnum.SmallShipMinerShip07Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip08", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Razorclaw", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip08, MyTextsWrapperEnum.SmallShipMinerShip08Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip09", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Greiser", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip09, MyTextsWrapperEnum.SmallShipMinerShip09Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_MinerShip10", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Tracer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipMinerShip10, MyTextsWrapperEnum.SmallShipMinerShip10Name));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Jacknife", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Jacknife", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipJackKnife, MyTextsWrapperEnum.SmallShipJackKnifeName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.DOON,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Doon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Doon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipDoon, MyTextsWrapperEnum.SmallShipDoonName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Hammer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hammer", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipHammer, MyTextsWrapperEnum.SmallShipHammerName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ORG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_ORG", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\ORG", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipOrg, MyTextsWrapperEnum.SmallShipOrgName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.YG,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_YG_Closed", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\YG_Closed", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                     MyTextsWrapperEnum.SmallShipYg, MyTextsWrapperEnum.SmallShipYgName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Hawk", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Hawk", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipHawk, MyTextsWrapperEnum.SmallShipHawkName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Phoenix", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Phoenix", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipPhoenix, MyTextsWrapperEnum.SmallShipPhoenixName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Leviathan", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Leviathan", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipLeviathan, MyTextsWrapperEnum.SmallShipLeviathanName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Rockheater", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Rockheater", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipRockheater, MyTextsWrapperEnum.SmallShipRockheaterName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_SteelHead", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\SteelHead", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipSteelHead, MyTextsWrapperEnum.SmallShipSteelHeadName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Talon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Talon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipTalon, MyTextsWrapperEnum.SmallShipTalonName));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV,
                new MyGuiSmallShipHelperSmallShip(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\SmallShip_Stanislav", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Ships\\Stanislav", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.SmallShipStanislav, MyTextsWrapperEnum.SmallShipStanislavName));

            #endregion

            #region SmallShip_AssignmentOfAmmo
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_AssignmentOfAmmo, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region LargeShip_Ammo
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Ammo, (int)MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum.CIWS_Armor_Piercing_Incendiary,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Ammo, (int)MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum.CIWS_High_Explosive_Incendiary,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.LargeShip_Ammo, (int)MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum.CIWS_SAPHEI,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));            
            #endregion

            #region Create Prefab Helpers

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P430_A01_PASSAGE_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p430_a01_passage_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P430_A02_PASSAGE_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p430_a02_passage_40m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P424_A01_PIPE_BASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p424_a01_pipe_base));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P423_A01_PIPE_JUNCTION, new MyGuiPrefabHelper(MyTextsWrapperEnum.p423_a01_pipe_junction));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P422_A01_PIPE_TURN_90, new MyGuiPrefabHelper(MyTextsWrapperEnum.p422_a01_pipe_turn_90));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P421_A01_PIPE_STRAIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p421_a01_pipe_straight_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P421_A02_PIPE_STRAIGHT_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p421_a02_pipe_straight_40m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P421_A03_PIPE_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p421_a03_pipe_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_G01_JUNCTION_6AXES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_g01_junction_6axes));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_G02_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_g02_entrance_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P410_G01_TURN_90_RIGHT_0M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p410_g01_turn_90_right_0m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G01_STRAIGHT_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g01_straight_1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G02_STRAIGHT_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g02_straight_2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G03_STRAIGHT_3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g03_straight_3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G04_STRAIGHT_4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g04_straight_4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_F02_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_f02_entrance_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F21_TURN_S_UP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f21_turn_s_up));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F22_TURN_S_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f22_turn_s_left));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F23_TURN_S_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f23_turn_s_right));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F24_TURN_S_DOWN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f24_turn_s_down));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F01_TURN_90_UP_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f01_turn_90_up_230m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F02_TURN_90_LEFT_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f02_turn_90_left_230m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F03_TURN_90_RIGHT_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f03_turn_90_right_230m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F04_TURN_90_DOWN_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f04_turn_90_down_230m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F01_STRAIGHT_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f01_straight_1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f02_straight_2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F03_STRAIGHT_3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f03_straight_3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_E01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_e01_entrance_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E01_STRAIGHT_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e01_straight_1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E02_STRAIGHT_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e02_straight_2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E03_STRAIGHT_3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e03_straight_3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E04_STRAIGHT_4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e04_straight_4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_E05_STRAIGHT_5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_e05_straight_5));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D01_DOORCASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d01_doorcase));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D02_DOOR1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d02_door1));            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D03_DOOR2_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d03_door2_a));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D03_DOOR2_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d03_door2_b));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_D01_JUNCTION_T_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_d01_junction_t_horizontal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_D03_JUNCTION_X_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_d03_junction_x_horizontal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_D01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_d01_entrance_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d01_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D02_STRAIGHT_40M_WITH_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d02_straight_40m_with_hole));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D03_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d03_straight_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D04_STRAIGHT_120M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d04_straight_120m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_D05_STRAIGHT_180M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_d05_straight_180m));            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_C01_JUNCTION_T_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_c01_junction_t_horizontal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_C01_JUNCTION_X_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_c01_junction_x_horizontal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_C01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_c01_entrance_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c01_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C02_STRAIGHT_40M_WITH_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c02_straight_40m_with_hole));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C03_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c03_straight_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C04_STRAIGHT_120M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c04_straight_120m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_C05_STRAIGHT_180M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_c05_straight_180m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_B02_DOOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_b02_door));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_B01_JUNCTION_T_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_b01_junction_t_horizontal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_B02_JUNCTION_T_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_b02_junction_t_vertical));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_B02_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_b02_entrance_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B21_TURN_S_UP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b21_turn_s_up));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B22_TURN_S_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b22_turn_s_left));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B23_TURN_S_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b23_turn_s_right));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B24_TURN_S_DOWN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b24_turn_s_down));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B11_TURN_90_UP_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b11_turn_90_up_160m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B12_TURN_90_LEFT_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b12_turn_90_left_160m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B13_TURN_90_RIGHT_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b13_turn_90_right_160m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B14_TURN_90_DOWN_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b14_turn_90_down_160m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B01_TURN_90_UP_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b01_turn_90_up_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B02_TURN_90_LEFT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b02_turn_90_left_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B03_TURN_90_RIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b03_turn_90_right_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_B04_TURN_90_DOWN_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_b04_turn_90_down_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_B01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b01_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B02_STRAIGHT_30M_YELLOW, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b02_straight_30m_yellow));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B03_STRAIGHT_320M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b03_straight_320m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B04_STRAIGHT_80M_WITH_SIDE_GRATES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b04_straight_80m_with_side_grates));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B05_STRAIGHT_80M_WITH_SIDE_OPEN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b05_straight_80m_with_side_open));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B06_STRAIGHT_180M_CONCRETE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b06_straight_180m_concrete));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B06_STRAIGHT_200M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b06_straight_200m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B07_STRAIGHT_180M_BLUE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b07_straight_180m_blue));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B09_STRAIGHT_30M_GRAY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b09_straight_30m_gray));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B11_STRAIGHT_220M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b11_straight_220m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B12_STRAIGHT_160M_DARK_METAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b12_straight_160m_dark_metal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.p411_B13_STRAIGHT_100M_TUBE_INSIDE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_b13_straight_100m_tube_inside));            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_A01_JUNCTION_T_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_a01_junction_t_horizontal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_A02_JUNCTION_T_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_a02_junction_t_vertical));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_A01_ENTRANCE_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_a01_entrance_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_A02_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_a02_entrance_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A21_TURN_S_UP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a21_turn_s_up));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A22_TURN_S_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a22_turn_s_left));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A23_TURN_S_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a23_turn_s_right));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A24_TURN_S_DOWN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a24_turn_s_down));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A11_TURN_90_UP_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a11_turn_90_up_160m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A12_TURN_90_LEFT_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a12_turn_90_left_160m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A13_TURN_90_RIGHT_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a13_turn_90_right_160m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A14_TURN_90_DOWN_160M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a14_turn_90_down_160m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A01_TURN_90_UP_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a01_turn_90_up_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A02_TURN_90_LEFT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a02_turn_90_left_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A03_TURN_90_RIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a03_turn_90_right_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_A04_TURN_90_DOWN_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_a04_turn_90_down_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a01_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A02_STRAIGHT_60M_WITH_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a02_straight_60m_with_hole));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A03_STRAIGHT_120M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a03_straight_120m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A04_STRAIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a04_straight_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_A05_STRAIGHT_80M_WITH_EXTENSION, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_a05_straight_80m_with_extension));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_E01_BRIDGE5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_e01_bridge5));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_D01_BRIDGE4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_d01_bridge4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_C01_BRIDGE3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_c01_bridge3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_B01_BRIDGE2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_b01_bridge2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P382_A01_BRIDGE1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p382_a01_bridge1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_c01_building3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_B01_BUILDING2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_b01_building2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_A01_BUILDING1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_a01_building1));            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P362_A01_SHORT_DISTANCE_ANTENNA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p362_a01_short_distance_antenna));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P361_A01_LONG_DISTANCE_ANTENNA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p361_a01_long_distance_antenna));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_B01_DOORCASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_b01_doorcase));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P351_A01_WEAPON_MOUNT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p351_a01_weapon_mount));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_REFINERY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_refinery));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P344_A01_CONTAINER_ARM_FILLED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p344_a01_container_arm_filled));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P344_A02_CONTAINER_ARM_EMPTY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p344_a02_container_arm_empty));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P343_A01_ORE_STORAGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p343_a01_ore_storage));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P342_A01_LOADING_BAY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p342_a01_loading_bay));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_B01_OPEN_DOCK_VARIATION1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p341_b01_open_dock_variation1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_B02_OPEN_DOCK_VARIATION2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p341_b02_open_dock_variation2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_A02_OPEN_DOCK_VARIATION2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p341_a02_open_dock_variation2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P333_A01_HYDROPONIC_BUILDING, new MyGuiPrefabHelper(MyTextsWrapperEnum.p333_a01_hydroponic_building));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P332_A01_OXYGEN_STORAGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p332_a01_oxygen_storage));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P331_A01_OXYGEN_GENERATOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p331_a01_oxygen_generator));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P324B01_FUEL_STORAGE_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.p324b01_fuel_storage_b));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P324A01_FUEL_STORAGE_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.p324a01_fuel_storage_a));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P323A01_FUEL_GENERATOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p323a01_fuel_generator));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P322A01_BATTERY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p322a01_battery));            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321B01_NUCLEAR_REACTOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321b01_nuclear_reactor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321A01_SOLAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321a01_solar_panel));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312A01_SHORT_TERM_THRUSTER_LATITUDE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p312a01_short_term_thruster_latitude));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312A02_SHORT_TERM_THRUSTER_LATERAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p312a02_short_term_thruster_lateral));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P311A01_LONG_TERM_THRUSTER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p311a01_long_term_thruster));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A01_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a01_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A02_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a02_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A03_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a03_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A04_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a04_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A05_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a05_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A06_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a06_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A07_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a07_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A08_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a08_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A09_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a09_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A10_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a10_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A11_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a11_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A12_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a12_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A13_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a13_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A14_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a14_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A15_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a15_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A16_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a16_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A17_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a17_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231A18_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231a18_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221E01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221e01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221D01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221d01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221C01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221c01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221B01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221b01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221A01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221a01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211G01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211g01_panel_120mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211H01_PANEL_535MX130M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211h01_panel_535mx130m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211G02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211g02_panel_60mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211G03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211g03_panel_60mx30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211F01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211f01_panel_120mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211F02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211f02_panel_60mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211F03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211f03_panel_60mx30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211E01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211e01_panel_120mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211E02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211e02_panel_60mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211E03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211e03_panel_60mx30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211D01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211d01_panel_120mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211D02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211d02_panel_60mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211D03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211d03_panel_60mx30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211C01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211c01_panel_120mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211C02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211c02_panel_60mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211C03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211c03_panel_60mx30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211B01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211b01_panel_120mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211B02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211b02_panel_60mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211B03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211b03_panel_60mx30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211A01_PANEL_120MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211a01_panel_120mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211A02_PANEL_60MX60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211a02_panel_60mx60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P211A03_PANEL_60MX30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p211a03_panel_60mx30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B01_CAGE_EMPTY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b01_cage_empty));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B02_CAGE_HALFCUT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b02_cage_halfcut));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B03_CAGE_WITH_CORNERS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b03_cage_with_corners));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B11_CAGE_PILLAR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b11_cage_pillar));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142B12_CAGE_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142b12_cage_edge));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A01_CAGE_EMPTY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a01_cage_empty));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A02_CAGE_HALFCUT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a02_cage_halfcut));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A03_CAGE_WITH_CORNERS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a03_cage_with_corners));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A11_CAGE_PILLAR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a11_cage_pillar));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P142A12_CAGE_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p142a12_cage_edge));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B01_THICK_FRAME_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b01_thick_frame_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B02_THICK_FRAME_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b02_thick_frame_straight_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B11_THICK_FRAME_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b11_thick_frame_edge));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B12_THICK_FRAME_CORNER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b12_thick_frame_corner));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141B31_THICK_FRAME_JOINT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141b31_thick_frame_joint));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A01_THICK_FRAME_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a01_thick_frame_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A02_THICK_FRAME_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a02_thick_frame_straight_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A11_THICK_FRAME_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a11_thick_frame_edge));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A12_THICK_FRAME_CORNER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a12_thick_frame_corner));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P141A31_THICK_FRAME_JOINT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p141a31_thick_frame_joint));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120J01_J_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120j01_j_straight_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130J02_J_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130j02_j_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120I01_I_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120i01_i_straight_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130I02_I_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130i02_i_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120H01_H_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120h01_h_straight_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130H02_H_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130h02_h_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120G01_G_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120g01_g_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130G02_G_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130g02_g_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120F01_F_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120f01_f_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130F02_F_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130f02_f_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120E01_E_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120e01_e_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130E02_E_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130e02_e_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120D01_D_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120d01_d_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130D02_D_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130d02_d_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C01_C_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120c01_c_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130C02_C_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130c02_c_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120B01_B_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120b01_b_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130B02_B_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130b02_b_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A01_A_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a01_a_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130A02_A_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130a02_a_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120D01_D_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120d01_d_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120D02_D_STRAIGHT_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120d02_d_straight_40m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C01_C_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120c01_c_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C02_C_STRAIGHT_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120c02_c_straight_40m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120B01_B_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120b01_b_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120B02_B_STRAIGHT_40M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120b02_b_straight_40m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A01_STRONG_LATTICE_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a01_strong_lattice_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A02_STRONG_LATTICE_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a02_strong_lattice_straight_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A03_STRONG_LATTICE_STRAIGHT_120M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a03_strong_lattice_straight_120m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A21_STRONG_LATTICE_JUNCTION_T_STRONG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a21_strong_lattice_junction_t_strong));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A22_STRONG_LATTICE_JUNCTION_T_WEAK, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a22_strong_lattice_junction_t_weak));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A23_STRONG_LATTICE_JUNCTION_T_ROTATED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a23_strong_lattice_junction_t_rotated));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A51_STRONG_TO_WEAK_LATTICE_2TO1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a51_strong_to_weak_lattice_2to1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A52_STRONG_TO_WEAK_LATTICE_1TO2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a52_strong_to_weak_lattice_1to2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120A61_WEAK_LATTICE_JUNCTION_T_ROTATED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120a61_weak_lattice_junction_t_rotated));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B01_LATTICE_BEAM_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b01_lattice_beam_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B02_LATTICE_BEAM_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b02_lattice_beam_straight_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b03_lattice_beam_straight_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_60M_WITH_PANELS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b04_lattice_beam_straight_60m_with_panels));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B21_LATTICE_BEAM_JUNCTION_T_STRONG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b21_lattice_beam_junction_t_strong));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B22_LATTICE_BEAM_JUNCTION_T_WEAK, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b22_lattice_beam_junction_t_weak));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B31_LATTICE_BEAM_JOINT_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b31_lattice_beam_joint_horizontal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B32_LATTICE_BEAM_JOINT_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b32_lattice_beam_joint_vertical));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A01_SOLID_BEAM_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a01_solid_beam_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A02_SOLID_BEAM_STRAIGHT_20M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a02_solid_beam_straight_20m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A03_SOLID_BEAM_STRAIGHT_40M_WITH_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a03_solid_beam_straight_40m_with_hole));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A04_SOLID_BEAM_STRAIGHT_40M_LATTICE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a04_solid_beam_straight_40m_lattice));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A05_SOLID_BEAM_STRAIGHT_80M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a05_solid_beam_straight_80m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A11_SOLID_BEAM_JUNCTION_X_STRONG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a11_solid_beam_junction_x_strong));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A12_SOLID_BEAM_JUNCTION_X_WEAK, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a12_solid_beam_junction_x_weak));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A13_SOLID_BEAM_JUNCTION_X_ROTATED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a13_solid_beam_junction_x_rotated));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A21_SOLID_BEAM_JUNCTION_T_STRONG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a21_solid_beam_junction_t_strong));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A22_SOLID_BEAM_JUNCTION_T_WEAK, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a22_solid_beam_junction_t_weak));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A23_SOLID_BEAM_JUNCTION_T_ROTATED, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a23_solid_beam_junction_t_rotated));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A31_SOLID_BEAM_JOINT_HORIZONTAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a31_solid_beam_joint_horizontal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A32_SOLID_BEAM_JOINT_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a32_solid_beam_joint_vertical));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A33_SOLID_BEAM_JOINT_LONGITUDINAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a33_solid_beam_joint_longitudinal));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A41_SOLID_BEAM_SUPERJOINT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a41_solid_beam_superjoint));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A01_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a01_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A02_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a02_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A03_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a03_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A04_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a04_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A05_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a05_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A06_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a06_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A07_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a07_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A08_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a08_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A09_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a09_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A10_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a10_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A11_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a11_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A12_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a12_billboard));            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A14_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a14_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A15_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a15_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A16_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a16_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A01_SIGN1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a01_sign1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A02_SIGN2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a02_sign2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A03_SIGN3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a03_sign3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A04_SIGN4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a04_sign4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A05_SIGN5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a05_sign5));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A06_SIGN6, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a06_sign6));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A07_SIGN7, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a07_sign7));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A08_SIGN8, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a08_sign8));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A09_SIGN9, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a09_sign9));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A10_SIGN10, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a10_sign10));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A11_SIGN11, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a11_sign11));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_A12_SIGN12, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_a12_sign12));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_A02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221e02_chamber_v2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_B02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221e02_chamber_v2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_C02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221e02_chamber_v2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_D02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221e02_chamber_v2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221_E02_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221e02_chamber_v2));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_A01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130a01_a_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_A02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130a02_a_straight_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_B01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130b01_b_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_B02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130b02_b_straight_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_C01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130c01_c_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_C02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130c02_c_straight_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_D01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130d01_d_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_D02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130d02_d_straight_30m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_E01_STRAIGHT_10M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130e01_e_straight_10m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130_E02_STRAIGHT_30M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130e02_e_straight_30m));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321_B01_SOLAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321b01_solar_panel));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321_C01_SOLAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321c01_solar_panel));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_D02_JUNCTION_T_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_d02_junction_t_vertical));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P413_D04_JUNCTION_X_VERTICAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p413_d04_junction_x_vertical));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F11_TURN_90_UP_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f11_turn_90_up_230m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F12_TURN_90_LEFT_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f12_turn_90_left_230m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F13_TURN_90_RIGHT_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f13_turn_90_right_230m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P412_F14_TURN_90_DOWN_230M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p412_f14_turn_90_down_230m));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F04_STRAIGHT_4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f04_straight_4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_F05_STRAIGHT_5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_f05_straight_5));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P411_G05_STRAIGHT_5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p411_g05_straight_5));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_F01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_f01_entrance_60m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P414_G01_ENTRANCE_60M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p414_g01_entrance_60m));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_A01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_a01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_B01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_b01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_BOX01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_box01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_BOX02_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_box02_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_C01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_c01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_D01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_d01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_E01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_e01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_F01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_f01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_G01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_g01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_H01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_h01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_I01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_i01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_J01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_j01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_K01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_k01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P571_L01_TRAFFIC_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));

            // HACK: No text for simple object!
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.SimpleObject, new MyGuiPrefabHelper(MyTextsWrapperEnum.CustomObject));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.AsteroidPrefabTest, new MyGuiPrefabHelper(MyTextsWrapperEnum.UnknownControl));            

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P385_A01_TEMPLE_900M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p385_a01_temple_900m));            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P383_A01_CHURCH, new MyGuiPrefabHelper(MyTextsWrapperEnum.p383_a01_church));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P334_A01_FOOD_GROW, new MyGuiPrefabHelper(MyTextsWrapperEnum.p334_a01_food_grow));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_BIO_EXP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_bio_exp));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_BIO_MACH_EXP, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_bio_mach_exp));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_RECYCLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_recycle));
            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_ESCAPE_POD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_escape_pod));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_ESCAPE_POD_BASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_escape_pod_base));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR_BODY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_ventilator_body));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR_PROPELLER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_ventilator_propeller));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P541_VENTILATOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p541_ventilator));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P349_A_TOWER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p349_a_tower));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P349_B_TOWER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p349_b_tower));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P349_C_TOWER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p349_c_tower));
            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_B01_DOORCASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_b01_doorcase));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_D01_DOORCASE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_d01_doorcase));
            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_B_FACTION, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_b_faction));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_B_FACTION_HOLO, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_b_faction_holo));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ARMOR_HULL, new MyGuiPrefabHelper(MyTextsWrapperEnum.Armor_hull));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_LARGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212a01_panel_large));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_MEDIUM, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212a01_panel_medium));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212A01_PANEL_SMALL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212a01_panel_small));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_MEDIUM, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212b02_panel_medium));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_SMALL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212b02_panel_small));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_MEDIUM, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212c03_panel_medium));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_SMALL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212c03_panel_small));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_MEDIUM, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212d04_panel_medium));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_SMALL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212d04_panel_small));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_MEDIUM, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212e05_panel_medium));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_SMALL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212e05_panel_small));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221F01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221f01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P341_C01_CLOSED_DOCK_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p341_c01_closed_dock_v1));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221G01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221g01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221H01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221h01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221J01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221j01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221K01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221k01_chamber_v1));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212B02_PANEL_LARGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212b02_panel_large));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212C03_PANEL_LARGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212c03_panel_large));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212D04_PANEL_LARGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212d04_panel_large));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212E05_PANEL_LARGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212e05_panel_large));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P212F01_PANEL_LARGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p212f01_panel_large));
            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_D03_HOSPITAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_d03_hospital));            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_D05_FOOD_GROW, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_d05_food_grow));                        
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_c01_building4));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_CORNER_25M, new MyGuiPrefabHelper(MyTextsWrapperEnum.Cable_corner_25m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_S_45M, new MyGuiPrefabHelper(MyTextsWrapperEnum.Cable_S_45m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_180, new MyGuiPrefabHelper(MyTextsWrapperEnum.Cable_straight_180));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_45, new MyGuiPrefabHelper(MyTextsWrapperEnum.Cable_straight_45));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CABLE_STRAIGHT_90, new MyGuiPrefabHelper(MyTextsWrapperEnum.Cable_straight_90));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CONNECTION_BOX, new MyGuiPrefabHelper(MyTextsWrapperEnum.Connection_box));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ADMINISTRATIVE_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_administrative_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARMORY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_armory));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_L, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_arrow_l));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_R, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_arrow_r));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ARROW_STR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_arrow_str));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_CARGO_BAY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_cargo_bay));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMAND_CENTER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_command_center));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMERCIAL_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_commercial_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_COMMUNICATIONS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_communications));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_DEFENSES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_defenses));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_DOCKS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_docks));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EMERGENCY_EXIT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_emergency_exit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ENGINEERING_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_engineering_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EXIT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_exit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_EXPERIMENTAL_LABS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_experimental_labs));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_FOUNDRY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_foundry));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_HABITATS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_habitats));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_HANGARS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_hangars));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_INDUSTRIAL_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_industrial_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_LANDING_BAY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_landing_bay));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MAINTENANCE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_maintenance));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MILITARY_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_military_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_MINES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_mines));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_ORE_PROCESSING, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_ore_processing));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_OUTER_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_outer_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_PRISON, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_prison));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_PUBLIC_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_public_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_REACTOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_reactor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_RESEARCH, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_research));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_RESTRICTED_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_restricted_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_SECURITY, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_security));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_SIGN, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_STORAGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_storage));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_TECHNICAL_AREA, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_technical_area));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_C_TRADE_PORT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_c_trade_port));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321_D01_BIG_SOLAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321_d01_big_solar_panel));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P361_B01_LONG_DISTANCE_ANTENNA_BIG, new MyGuiPrefabHelper(MyTextsWrapperEnum.p361_b01_long_distance_antenna_big));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.FOURTH_REICH_WRECK, new MyGuiPrefabHelper(MyTextsWrapperEnum.Fourth_reich_wreck));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P344_A03_CONTAINER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p344_a03_container));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231b01_armor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_CORNER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231b01_armor_corner));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_EDGE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231b01_armor_edge));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P231B01_ARMOR_HOLE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p231b01_armor_hole));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A03_SHELF_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p150a03_shelf_1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A02_SHELF_1X2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p150a02_shelf_1X2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P150A01_SHELF_1X3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p150a01_shelf_1X3));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P611_ASTEROID_PART_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.p611_asteroid_part_A));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P611_ASTEROID_PART_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.p611_asteroid_part_B));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P363_A01_BIG_ANTENNA_300M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p363_a01_big_antenna_300m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130D02_D_STRAIGHT_300M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130d02_d_straight_300m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P130J01_J_STRAIGHT_300M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p130j01_j_straight_300m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P120C02_C_STRAIGHT_400M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p120c02_c_straight_400m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_420M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b03_lattice_beam_straight_420m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_420M_WITH_PANELS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110b04_lattice_beam_straight_420m_with_panels));
            
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P363_A01_BIG_ANTENNA_1500M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p363_a01_big_antenna_1500m));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4221_A01_COOLING_DEVICE_WALL_340X400, new MyGuiPrefabHelper(MyTextsWrapperEnum.p4221_a01_cooling_device_wall_340x400));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4222_A01_PIPES_CONNECTOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.p4222_a01_pipes_connector));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P4223_A01_OPEN_PIPE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p4223_a01_open_pipe));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P311B01_LONG_TERM_THRUSTER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p311b01_long_term_thruster));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221K01_CHAMBER_V2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221k01_chamber_v2));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_BARREL_BIOHAZARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_barrel_biohazard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_BARREL_BIOHAZARD_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_barrel_biohazard_2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_NUKE_BARREL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_nuke_barrel));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_RED_BARREL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_red_barrel));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_simple_barrel));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_simple_barrel_2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.Barrel_prop_A));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.Barrel_prop_B));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_C, new MyGuiPrefabHelper(MyTextsWrapperEnum.Barrel_prop_C));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_D, new MyGuiPrefabHelper(MyTextsWrapperEnum.Barrel_prop_D));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BARREL_PROP_E, new MyGuiPrefabHelper(MyTextsWrapperEnum.Barrel_prop_E));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.CANNONBALL_CAPSULE_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.CannonBall_Capsule_1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.GATTLING_AMMO_BELT, new MyGuiPrefabHelper(MyTextsWrapperEnum.Gattling_ammo_belt));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PACK01, new MyGuiPrefabHelper(MyTextsWrapperEnum.Missile_pack01));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PACK02, new MyGuiPrefabHelper(MyTextsWrapperEnum.Missile_pack02));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_PLAZMA01, new MyGuiPrefabHelper(MyTextsWrapperEnum.Missile_plazma01));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.MISSILE_STACK_BIOCHEM01, new MyGuiPrefabHelper(MyTextsWrapperEnum.Missile_stack_biochem01));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_O2_BARREL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_o2_barrel));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.NUCLEAR_WARHEAD_CLOSED, new MyGuiPrefabHelper(MyTextsWrapperEnum.Nuclear_Warhead_closed));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.NUCLEAR_WARHEAD_OPEN, new MyGuiPrefabHelper(MyTextsWrapperEnum.Nuclear_Warhead_open));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_NUKE_BARREL_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_nuke_barrel_1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P581_A01_SIMPLE_BARREL_3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p581_a01_simple_barrel_3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221L01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221L01_chamber_v1));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_BOTTOM_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221m01_chamber_bottom_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_CENTER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221m01_chamber_center_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221M01_CHAMBER_TOP_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221m01_chamber_top_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P321E01_SOLAR_PANEL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p321e01_solar_panel));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P110A06_SOLID_BEAM_STRAIGHT_420M, new MyGuiPrefabHelper(MyTextsWrapperEnum.p110a06_solid_beam_straight_420m));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P311B01_CUT_THRUSTER, new MyGuiPrefabHelper(MyTextsWrapperEnum.p311b01_cut_thruster));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312B01_CUT_THRUSTER_LATERAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.p312b01_cut_thruster_lateral));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P312B02_CUT_THRUSTER_LATITUDE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p312b02_cut_thruster_latitude));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ALIEN_DETECTOR_UNIT, new MyGuiPrefabHelper(MyTextsWrapperEnum.alien_detector_unit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING5, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_c01_building5));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING6, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_c01_building6));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P381_C01_BUILDING7, new MyGuiPrefabHelper(MyTextsWrapperEnum.p381_c01_building7));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P221N01_CHAMBER_V1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p221n01_chamber_v1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P531_D_MEDIC_CROSS, new MyGuiPrefabHelper(MyTextsWrapperEnum.p531_d_medic_cross));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.ALIEN_ARTEFACT, new MyGuiPrefabHelper(MyTextsWrapperEnum.alien_artefact));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.BOMB, new MyGuiPrefabHelper(MyTextsWrapperEnum.Bomb));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.RAIL_GUN, new MyGuiPrefabHelper(MyTextsWrapperEnum.rail_gun));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P345_A01_RECYCLE_SPHERE, new MyGuiPrefabHelper(MyTextsWrapperEnum.p345_a01_recycle_sphere));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.PRISON, new MyGuiPrefabHelper(MyTextsWrapperEnum.prison));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A17_BILLBOARD_PORTRAIT_1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a17_billboard_portrait_1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A17_BILLBOARD_PORTRAIT_2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a17_billboard_portrait_2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A18_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a18_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P511_A19_BILLBOARD, new MyGuiPrefabHelper(MyTextsWrapperEnum.p511_a19_billboard));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.DEAD_PILOT, new MyGuiPrefabHelper(MyTextsWrapperEnum.DeadPilot));


            #endregion

            #region PrefabContainer
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabContainer, (int)MyMwcObjectBuilder_PrefabContainer_TypesEnum.TEMPLATE,
                new MyGuiHelperBase(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Alarm", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.EmptyDescription));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabContainer, (int)MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE,
                new MyGuiHelperBase(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Alarm", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region InfluenceSpheres

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.InfluenceSphere, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));

            #endregion

            #region Create Prefab Light Helpers (also included in Prefab helpers)

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLight, (int)MyMwcObjectBuilder_PrefabLight_TypesEnum.DEFAULT_LIGHT_0, new MyGuiPrefabHelper(MyTextsWrapperEnum.default_light_0));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLight, (int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A01_LIGHT1, new MyGuiPrefabHelper(MyTextsWrapperEnum.p521_a01_light1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLight, (int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A02_LIGHT2, new MyGuiPrefabHelper(MyTextsWrapperEnum.p521_a02_light2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLight, (int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A03_LIGHT3, new MyGuiPrefabHelper(MyTextsWrapperEnum.p521_a03_light3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLight, (int)MyMwcObjectBuilder_PrefabLight_TypesEnum.P521_A04_LIGHT4, new MyGuiPrefabHelper(MyTextsWrapperEnum.p521_a04_light4));

            #endregion

            #region Prefab particles
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabParticles, (int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.DEFAULT_PARTICLE_PREFAB_0, new MyGuiPrefabHelper(MyTextsWrapperEnum.default_particles_prefab_0));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabParticles, (int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_A01_PARTICLES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p551_a01_particles));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabParticles, (int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_B01_PARTICLES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p551_b01_particles));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabParticles, (int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_C01_PARTICLES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p551_c01_particles));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabParticles, (int)MyMwcObjectBuilder_PrefabParticles_TypesEnum.P551_D01_PARTICLES, new MyGuiPrefabHelper(MyTextsWrapperEnum.p551_d01_particles));
            #endregion

            #region Prefab sound
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSound, (int)MyMwcObjectBuilder_PrefabSound_TypesEnum.DEFAULT_SOUND_PREFAB_0, new MyGuiPrefabHelper(MyTextsWrapperEnum.default_sound_prefab_0));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSound, (int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_A01_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.p561_a01_sound));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSound, (int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_B01_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.p561_b01_sound));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSound, (int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_C01_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.p561_c01_sound));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSound, (int)MyMwcObjectBuilder_PrefabSound_TypesEnum.P561_D01_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.p561_d01_sound));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSound, (int)MyMwcObjectBuilder_PrefabSound_TypesEnum.MOTHERSHIP_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.mothership_sound));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSound, (int)MyMwcObjectBuilder_PrefabSound_TypesEnum.MADELINE_MOTHERSHIP_SOUND, new MyGuiPrefabHelper(MyTextsWrapperEnum.madeline_mothership_sound));
            #endregion

            #region Prefab kinematic
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematic, (int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR1, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Doors", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p415_c01_door1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematic, (int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR2, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Doors", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p415_c01_door2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematic, (int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR3, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Doors", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p415_c01_door3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematic, (int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_C01_DOOR4, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Doors", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p415_c01_door4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematic, (int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Doors", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p341_a01_open_dock_variation1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematic, (int)MyMwcObjectBuilder_PrefabKinematic_TypesEnum.P415_A01_DOORCASE, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Doors", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p415_a01_doorcase));
            #endregion

            #region Create Ore Helpers
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.COBALT,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreCobalt", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreCobalt, MyTextsWrapperEnum.OreCobaltName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.CONCRETE,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreConcrete", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreConcrete, MyTextsWrapperEnum.OreConcreteName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.GOLD,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreGold", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreGold, MyTextsWrapperEnum.OreGoldName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.HELIUM,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreHelium", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreHelium, MyTextsWrapperEnum.OreHeliumName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.ICE,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreIce", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreIce, MyTextsWrapperEnum.OreIceName));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.INDESTRUCTIBLE,
            //    new MyGuiSmallShipHelperOre(MyTextsWrapperEnum.OreIndestructible));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.IRON,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreIron", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreIron, MyTextsWrapperEnum.OreIronName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.MAGNESIUM,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreMagnesium", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreMagnesium, MyTextsWrapperEnum.OreMagnesiumName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.NICKEL,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreNickel", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreNickel, MyTextsWrapperEnum.OreNickelName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.ORGANIC,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreOrganic", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreOrganic, MyTextsWrapperEnum.OreOrganicName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.PLATINUM,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OrePlatinum", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OrePlatinum, MyTextsWrapperEnum.OrePlatinumName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.SANDSTONE,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreSandstone", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreSandstone, MyTextsWrapperEnum.OreSandstoneName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.SILICON,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreSilicon", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreSilicon, MyTextsWrapperEnum.OreSiliconName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.SILVER,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreSilver", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreSilver, MyTextsWrapperEnum.OreSilverName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.STONE,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreStone", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreStone, MyTextsWrapperEnum.OreStoneName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.TREASURE,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreTreasure", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreTreasure, MyTextsWrapperEnum.OreTreasureName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.URANITE,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreUranite", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreUranite, MyTextsWrapperEnum.OreUraniteName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.XENON,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreXenon", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreXenon, MyTextsWrapperEnum.None));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.LAVA,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreLava", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreLava, MyTextsWrapperEnum.OreLavaName));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)MyMwcObjectBuilder_Ore_TypesEnum.SNOW,
                new MyGuiOreHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\OreSnow", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.OreSnow, MyTextsWrapperEnum.OreSnowName));
            #endregion

            #region Create Blueprints Helpers
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P430_A01_PASSAGE_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p430_a01_passage_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P430_A02_PASSAGE_40M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p430_a02_passage_40m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P424_A01_PIPE_BASE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p424_a01_pipe_base));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P423_A01_PIPE_JUNCTION, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p423_a01_pipe_junction));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P422_A01_PIPE_TURN_90, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p422_a01_pipe_turn_90));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P421_A01_PIPE_STRAIGHT_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p421_a01_pipe_straight_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P421_A02_PIPE_STRAIGHT_40M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p421_a02_pipe_straight_40m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P421_A03_PIPE_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p421_a03_pipe_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_G01_JUNCTION_6AXES, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_g01_junction_6axes));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_G02_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_g02_entrance_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P410_G01_TURN_90_RIGHT_0M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p410_g01_turn_90_right_0m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G01_STRAIGHT_1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_g01_straight_1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_g02_straight_2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G03_STRAIGHT_3, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_g03_straight_3));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G04_STRAIGHT_4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_g04_straight_4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_F02_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_f02_entrance_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F21_TURN_S_UP, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f21_turn_s_up));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F22_TURN_S_LEFT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f22_turn_s_left));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F23_TURN_S_RIGHT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f23_turn_s_right));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F24_TURN_S_DOWN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f24_turn_s_down));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F01_TURN_90_UP_230M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f01_turn_90_up_230m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F02_TURN_90_LEFT_230M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f02_turn_90_left_230m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F03_TURN_90_RIGHT_230M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f03_turn_90_right_230m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F04_TURN_90_DOWN_230M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f04_turn_90_down_230m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F01_STRAIGHT_1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_f01_straight_1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_f02_straight_2));

            /*
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C02_DOOR1_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C02_DOOR1_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C03_DOOR2_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C03_DOOR2_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P415_C03_DOOR2_A_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C03_DOOR2_A_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C03_DOOR2_B_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C03_DOOR2_B_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORLEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORRIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.No));
            */
            /*
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.No));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.No));
            */
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F03_STRAIGHT_3, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_f03_straight_3));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_E01_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_e01_entrance_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E01_STRAIGHT_1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_e01_straight_1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E02_STRAIGHT_2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_e02_straight_2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E03_STRAIGHT_3, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_e03_straight_3));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E04_STRAIGHT_4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_e04_straight_4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_E05_STRAIGHT_5, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_e05_straight_5));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_D01_DOORCASE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_d01_doorcase));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_D02_DOOR1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_d02_door1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_D03_DOOR2_A, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_d03_door2_a));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_D03_DOOR2_B, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_d03_door2_b));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_D01_JUNCTION_T_HORIZONTAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_d01_junction_t_horizontal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_D03_JUNCTION_X_HORIZONTAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_d03_junction_x_horizontal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_D01_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_d01_entrance_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_d01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D02_STRAIGHT_40M_WITH_HOLE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_d02_straight_40m_with_hole));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D03_STRAIGHT_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_d03_straight_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D04_STRAIGHT_120M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_d04_straight_120m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_D05_STRAIGHT_180M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_d05_straight_180m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C01_DOOR1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_c01_door1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C01_DOOR2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_c01_door2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C01_DOOR3, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_c01_door3));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_C01_DOOR4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_c01_door4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_C01_JUNCTION_T_HORIZONTAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_c01_junction_t_horizontal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_C01_JUNCTION_X_HORIZONTAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_c01_junction_x_horizontal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_C01_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_c01_entrance_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_c01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C02_STRAIGHT_40M_WITH_HOLE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_c02_straight_40m_with_hole));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C03_STRAIGHT_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_c03_straight_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C04_STRAIGHT_120M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_c04_straight_120m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_C05_STRAIGHT_180M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_c05_straight_180m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_B01_DOORCASE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_b01_doorcase));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_B02_DOOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_b02_door));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_B01_JUNCTION_T_HORIZONTAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_b01_junction_t_horizontal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_B02_JUNCTION_T_VERTICAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_b02_junction_t_vertical));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_B02_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_b02_entrance_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B21_TURN_S_UP, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b21_turn_s_up));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B22_TURN_S_LEFT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b22_turn_s_left));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B23_TURN_S_RIGHT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b23_turn_s_right));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B24_TURN_S_DOWN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b24_turn_s_down));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B11_TURN_90_UP_160M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b11_turn_90_up_160m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B12_TURN_90_LEFT_160M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b12_turn_90_left_160m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B13_TURN_90_RIGHT_160M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b13_turn_90_right_160m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B14_TURN_90_DOWN_160M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b14_turn_90_down_160m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B01_TURN_90_UP_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b01_turn_90_up_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B02_TURN_90_LEFT_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b02_turn_90_left_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B03_TURN_90_RIGHT_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b03_turn_90_right_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_B04_TURN_90_DOWN_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_b04_turn_90_down_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_B01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_b01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B02_STRAIGHT_30M_YELLOW, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b02_straight_30m_yellow));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B03_STRAIGHT_320M, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b03_straight_320m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B04_STRAIGHT_80M_WITH_SIDE_GRATES, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b04_straight_80m_with_side_grates));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B05_STRAIGHT_80M_WITH_SIDE_OPEN, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b05_straight_80m_with_side_open));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B06_STRAIGHT_180M_CONCRETE, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b06_straight_180m_concrete));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B06_STRAIGHT_200M, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b06_straight_200m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B07_STRAIGHT_180M_BLUE, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b07_straight_180m_blue));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B09_STRAIGHT_30M_GRAY, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b09_straight_30m_gray));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B11_STRAIGHT_220M, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b11_straight_220m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B12_STRAIGHT_160M_DARK_METAL, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b12_straight_160m_dark_metal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.p411_B13_STRAIGHT_100M_TUBE_INSIDE, new MyGuiPrefabHelper(MyTextsWrapperEnum.Blueprint_p411_b13_straight_100m_tube_inside));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_A01_DOORCASE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_a01_doorcase));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P415_A02_DOOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p415_a02_door));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_A01_JUNCTION_T_HORIZONTAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_a01_junction_t_horizontal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_A02_JUNCTION_T_VERTICAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_a02_junction_t_vertical));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_A01_ENTRANCE_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_a01_entrance_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_A02_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_a02_entrance_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A21_TURN_S_UP, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a21_turn_s_up));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A22_TURN_S_LEFT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a22_turn_s_left));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A23_TURN_S_RIGHT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a23_turn_s_right));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A24_TURN_S_DOWN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a24_turn_s_down));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A11_TURN_90_UP_160M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a11_turn_90_up_160m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A12_TURN_90_LEFT_160M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a12_turn_90_left_160m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A13_TURN_90_RIGHT_160M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a13_turn_90_right_160m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A14_TURN_90_DOWN_160M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a14_turn_90_down_160m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A01_TURN_90_UP_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a01_turn_90_up_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A02_TURN_90_LEFT_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a02_turn_90_left_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A03_TURN_90_RIGHT_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a03_turn_90_right_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_A04_TURN_90_DOWN_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_a04_turn_90_down_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_a01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A02_STRAIGHT_60M_WITH_HOLE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_a02_straight_60m_with_hole));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A03_STRAIGHT_120M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_a03_straight_120m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A04_STRAIGHT_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_a04_straight_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_A05_STRAIGHT_80M_WITH_EXTENSION, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_a05_straight_80m_with_extension));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_E01_BRIDGE5, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p382_e01_bridge5));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_D01_BRIDGE4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p382_d01_bridge4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_C01_BRIDGE3, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p382_c01_bridge3));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_B01_BRIDGE2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p382_b01_bridge2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P382_A01_BRIDGE1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p382_a01_bridge1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P381_C01_BUILDING3, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p381_c01_building3));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P381_B01_BUILDING2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p381_b01_building2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P381_A01_BUILDING1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p381_a01_building1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P361_A01_SMALL_HANGAR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p361_a01_small_hangar));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P362_A01_SHORT_DISTANCE_ANTENNA, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p362_a01_short_distance_antenna));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P361_A01_LONG_DISTANCE_ANTENNA, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p361_a01_long_distance_antenna));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P351_A01_WEAPON_MOUNT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p351_a01_weapon_mount));            
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a01_largeship_autocannon));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_CIWS, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a01_largeship_ciws));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a01_largeship_machinegun));            
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a01_largeship_missile_basic4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a01_largeship_missile_basic6));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a01_largeship_missile_basic9));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a02_largeship_missile_guided4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a02_largeship_missile_guided6));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p352_a02_largeship_missile_guided9));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P345_A01_REFINERY, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p345_a01_refinery));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P344_A01_CONTAINER_ARM_FILLED, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p344_a01_container_arm_filled));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P344_A02_CONTAINER_ARM_EMPTY, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p344_a02_container_arm_empty));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P343_A01_ORE_STORAGE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p343_a01_ore_storage));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P342_A01_LOADING_BAY, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p342_a01_loading_bay));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_B01_OPEN_DOCK_VARIATION1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p341_b01_open_dock_variation1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_B02_OPEN_DOCK_VARIATION2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p341_b02_open_dock_variation2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p341_a01_open_dock_variation1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P341_A02_OPEN_DOCK_VARIATION2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p341_a02_open_dock_variation2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P333_A01_HYDROPONIC_BUILDING, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p333_a01_hydroponic_building));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P332_A01_OXYGEN_STORAGE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p332_a01_oxygen_storage));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P331_A01_OXYGEN_GENERATOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p331_a01_oxygen_generator));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P324B01_FUEL_STORAGE_B, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p324b01_fuel_storage_b));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P324A01_FUEL_STORAGE_A, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p324a01_fuel_storage_a));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P323A01_FUEL_GENERATOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p323a01_fuel_generator));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P322A01_BATTERY, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p322a01_battery));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321C01_INERTIA_GENERATOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p321c01_inertia_generator));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321B01_NUCLEAR_REACTOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p321b01_nuclear_reactor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321A01_SOLAR_PANEL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p321a01_solar_panel));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P312A01_SHORT_TERM_THRUSTER_LATITUDE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p312a01_short_term_thruster_latitude));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P312A02_SHORT_TERM_THRUSTER_LATERAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p312a02_short_term_thruster_lateral));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P311A01_LONG_TERM_THRUSTER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p311a01_long_term_thruster));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A01_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a01_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A02_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a02_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A03_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a03_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A04_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a04_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A05_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a05_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A06_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a06_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A07_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a07_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A08_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a08_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A09_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a09_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A10_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a10_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A11_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a11_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A12_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a12_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A13_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a13_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A14_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a14_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A15_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a15_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A16_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a16_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A17_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a17_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P231A18_ARMOR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p231a18_armor));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221E01_CHAMBER_V1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221e01_chamber_v1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221D01_CHAMBER_V1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221d01_chamber_v1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221C01_CHAMBER_V1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221c01_chamber_v1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221B01_CHAMBER_V1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221b01_chamber_v1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221A01_CHAMBER_V1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221a01_chamber_v1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211G01_PANEL_120MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211g01_panel_120mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211G02_PANEL_60MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211g02_panel_60mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211G03_PANEL_60MX30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211g03_panel_60mx30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211F01_PANEL_120MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211f01_panel_120mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211F02_PANEL_60MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211f02_panel_60mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211F03_PANEL_60MX30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211f03_panel_60mx30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211E01_PANEL_120MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211e01_panel_120mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211E02_PANEL_60MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211e02_panel_60mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211E03_PANEL_60MX30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211e03_panel_60mx30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211D01_PANEL_120MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211d01_panel_120mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211D02_PANEL_60MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211d02_panel_60mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211D03_PANEL_60MX30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211d03_panel_60mx30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211C01_PANEL_120MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211c01_panel_120mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211C02_PANEL_60MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211c02_panel_60mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211C03_PANEL_60MX30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211c03_panel_60mx30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211B01_PANEL_120MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211b01_panel_120mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211B02_PANEL_60MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211b02_panel_60mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211B03_PANEL_60MX30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211b03_panel_60mx30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211A01_PANEL_120MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211a01_panel_120mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211A02_PANEL_60MX60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211a02_panel_60mx60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P211A03_PANEL_60MX30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p211a03_panel_60mx30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B01_CAGE_EMPTY, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142b01_cage_empty));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B02_CAGE_HALFCUT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142b02_cage_halfcut));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B03_CAGE_WITH_CORNERS, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142b03_cage_with_corners));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B11_CAGE_PILLAR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142b11_cage_pillar));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142B12_CAGE_EDGE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142b12_cage_edge));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A01_CAGE_EMPTY, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142a01_cage_empty));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A02_CAGE_HALFCUT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142a02_cage_halfcut));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A03_CAGE_WITH_CORNERS, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142a03_cage_with_corners));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A11_CAGE_PILLAR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142a11_cage_pillar));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P142A12_CAGE_EDGE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p142a12_cage_edge));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B01_THICK_FRAME_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141b01_thick_frame_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B02_THICK_FRAME_STRAIGHT_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141b02_thick_frame_straight_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B11_THICK_FRAME_EDGE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141b11_thick_frame_edge));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B12_THICK_FRAME_CORNER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141b12_thick_frame_corner));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141B31_THICK_FRAME_JOINT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141b31_thick_frame_joint));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A01_THICK_FRAME_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141a01_thick_frame_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A02_THICK_FRAME_STRAIGHT_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141a02_thick_frame_straight_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A11_THICK_FRAME_EDGE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141a11_thick_frame_edge));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A12_THICK_FRAME_CORNER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141a12_thick_frame_corner));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P141A31_THICK_FRAME_JOINT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p141a31_thick_frame_joint));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120J01_J_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120j01_j_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130J02_J_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130j02_j_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120I01_I_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120i01_i_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130I02_I_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130i02_i_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120H01_H_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120h01_h_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130H02_H_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130h02_h_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120G01_G_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120g01_g_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130G02_G_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130g02_g_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120F01_F_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120f01_f_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130F02_F_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130f02_f_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120E01_E_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120e01_e_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130E02_E_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130e02_e_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120D01_D_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120d01_d_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130D02_D_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130d02_d_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120C01_C_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120c01_c_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130C02_C_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130c02_c_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120B01_B_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120b01_b_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130B02_B_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130b02_b_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A01_A_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a01_a_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130A02_A_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130a02_a_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120D01_D_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120d01_d_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120D02_D_STRAIGHT_40M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120d02_d_straight_40m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120C01_C_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120c01_c_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120C02_C_STRAIGHT_40M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120c02_c_straight_40m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120B01_B_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120b01_b_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120B02_B_STRAIGHT_40M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120b02_b_straight_40m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A01_STRONG_LATTICE_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a01_strong_lattice_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A02_STRONG_LATTICE_STRAIGHT_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a02_strong_lattice_straight_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A03_STRONG_LATTICE_STRAIGHT_120M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a03_strong_lattice_straight_120m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A21_STRONG_LATTICE_JUNCTION_T_STRONG, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a21_strong_lattice_junction_t_strong));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A22_STRONG_LATTICE_JUNCTION_T_WEAK, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a22_strong_lattice_junction_t_weak));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A23_STRONG_LATTICE_JUNCTION_T_ROTATED, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a23_strong_lattice_junction_t_rotated));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A51_STRONG_TO_WEAK_LATTICE_2TO1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a51_strong_to_weak_lattice_2to1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A52_STRONG_TO_WEAK_LATTICE_1TO2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a52_strong_to_weak_lattice_1to2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P120A61_WEAK_LATTICE_JUNCTION_T_ROTATED, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p120a61_weak_lattice_junction_t_rotated));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B01_LATTICE_BEAM_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110b01_lattice_beam_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B02_LATTICE_BEAM_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110b02_lattice_beam_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B03_LATTICE_BEAM_STRAIGHT_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110b03_lattice_beam_straight_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B04_LATTICE_BEAM_STRAIGHT_60M_WITH_PANELS, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110b04_lattice_beam_straight_60m_with_panels));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B21_LATTICE_BEAM_JUNCTION_T_STRONG, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110b21_lattice_beam_junction_t_strong));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B22_LATTICE_BEAM_JUNCTION_T_WEAK, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110b22_lattice_beam_junction_t_weak));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B31_LATTICE_BEAM_JOINT_HORIZONTAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110b31_lattice_beam_joint_horizontal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110B32_LATTICE_BEAM_JOINT_VERTICAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110b32_lattice_beam_joint_vertical));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A01_SOLID_BEAM_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a01_solid_beam_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A02_SOLID_BEAM_STRAIGHT_20M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a02_solid_beam_straight_20m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A03_SOLID_BEAM_STRAIGHT_40M_WITH_HOLE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a03_solid_beam_straight_40m_with_hole));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A04_SOLID_BEAM_STRAIGHT_40M_LATTICE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a04_solid_beam_straight_40m_lattice));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A05_SOLID_BEAM_STRAIGHT_80M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a05_solid_beam_straight_80m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A11_SOLID_BEAM_JUNCTION_X_STRONG, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a11_solid_beam_junction_x_strong));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A12_SOLID_BEAM_JUNCTION_X_WEAK, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a12_solid_beam_junction_x_weak));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A13_SOLID_BEAM_JUNCTION_X_ROTATED, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a13_solid_beam_junction_x_rotated));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A21_SOLID_BEAM_JUNCTION_T_STRONG, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a21_solid_beam_junction_t_strong));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A22_SOLID_BEAM_JUNCTION_T_WEAK, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a22_solid_beam_junction_t_weak));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A23_SOLID_BEAM_JUNCTION_T_ROTATED, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a23_solid_beam_junction_t_rotated));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A31_SOLID_BEAM_JOINT_HORIZONTAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a31_solid_beam_joint_horizontal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A32_SOLID_BEAM_JOINT_VERTICAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a32_solid_beam_joint_vertical));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A33_SOLID_BEAM_JOINT_LONGITUDINAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a33_solid_beam_joint_longitudinal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P110A41_SOLID_BEAM_SUPERJOINT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p110a41_solid_beam_superjoint));
            ////@ Lights
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.DEFAULT_LIGHT_0, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_default_light_0));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P521_A01_LIGHT1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p521_a01_light1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P521_A02_LIGHT2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p521_a02_light2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P521_A03_LIGHT3, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p521_a03_light3));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P521_A04_LIGHT4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p521_a04_light4));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.DEFAULT_PARTICLE_PREFAB_0, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_default_particle_prefab_0));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P551_A01_PARTICLES, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p551_a01_particles));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P551_B01_PARTICLES, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p551_b01_particles));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P551_C01_PARTICLES, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p551_c01_particles));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P551_D01_PARTICLES, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p551_d01_particles));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.DEFAULT_SOUND_PREFAB_0, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_default_sound_prefab_0));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P561_A01_SOUND, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p561_a01_sound));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P561_B01_SOUND, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p561_b01_sound));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P561_C01_SOUND, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p561_c01_sound));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P561_D01_SOUND, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p561_d01_sound));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A01_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a01_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A02_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a02_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A03_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a03_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A04_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a04_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A05_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a05_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A06_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a06_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A07_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a07_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A08_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a08_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A09_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a09_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A10_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a10_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A11_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a11_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A12_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a12_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A13_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a13_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A14_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a14_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A15_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a15_billboard));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P511_A16_BILLBOARD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p511_a16_billboard));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A01_SIGN1, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a01_sign1));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A02_SIGN2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a02_sign2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A03_SIGN3, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a03_sign3));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A04_SIGN4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a04_sign4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A05_SIGN5, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a05_sign5));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A06_SIGN6, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a06_sign6));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A07_SIGN7, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a07_sign7));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A08_SIGN8, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a08_sign8));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A09_SIGN9, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a09_sign9));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A10_SIGN10, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a10_sign10));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A11_SIGN11, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a11_sign11));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P531_A12_SIGN12, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p531_a12_sign12));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_A02_CHAMBER_V2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221_a02_chamber_v2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_B02_CHAMBER_V2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221_b02_chamber_v2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_C02_CHAMBER_V2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221_c02_chamber_v2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_D02_CHAMBER_V2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221_d02_chamber_v2));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P221_E02_CHAMBER_V2, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p221_e02_chamber_v2));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_A01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_a01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_A02_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_a02_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_B01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_b01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_B02_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_b02_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_C01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_c01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_C02_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_c02_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_D01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_d01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_D02_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_d02_straight_30m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_E01_STRAIGHT_10M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_e01_straight_10m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P130_E02_STRAIGHT_30M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p130_e02_straight_30m));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321_B01_SOLAR_PANEL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p321_b01_solar_panel));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P321_C01_SOLAR_PANEL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p321_c01_solar_panel));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_D02_JUNCTION_T_VERTICAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_d02_junction_t_vertical));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P413_D04_JUNCTION_X_VERTICAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p413_d04_junction_x_vertical));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F11_TURN_90_UP_230M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f11_turn_90_up_230m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F12_TURN_90_LEFT_230M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f12_turn_90_left_230m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F13_TURN_90_RIGHT_230M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f13_turn_90_right_230m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P412_F14_TURN_90_DOWN_230M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p412_f14_turn_90_down_230m));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F04_STRAIGHT_4, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_f04_straight_4));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_F05_STRAIGHT_5, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_f05_straight_5));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P411_G05_STRAIGHT_5, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p411_g05_straight_5));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_F01_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_f01_entrance_60m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P414_G01_ENTRANCE_60M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p414_g01_entrance_60m));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_A01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_a01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_B01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_b01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_BOX01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_box01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_BOX02_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_box02_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_C01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_c01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_D01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_d01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_E01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_e01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_F01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_f01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_G01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_g01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_H01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_h01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_I01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_i01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_J01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_j01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_K01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_k01_traffic_sign));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P571_L01_TRAFFIC_SIGN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p571_l01_traffic_sign));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_AUTOCANNON, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_Autocanon));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_AUTOMATIC_RIFLE_WITH_SILENCER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_AutomaticRifleWithSilencer));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_CANNON, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_Cannon));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_CRUSHER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_DrillingDeviceCrusher));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_LASER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_DrillingDeviceLaser));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_NUCLEAR, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_DrillingDeviceNuclear));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_SAW, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_DrillingDeviceSaw));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_THERMAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_DrillingDeviceThermal));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_DRILLING_DEVICE_PRESSURE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_DrillingDevicePressure));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_HARVESTING_DEVICE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_HarvestingDevice));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_MACHINE_GUN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_MachineGun));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_MISSILE_LAUNCHER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_MissileLauncher));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_SHOTGUN, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_Shotgun));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_SNIPER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_Sniper));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_UNIVERSAL_LAUNCHER_BACK, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_UniversalLauncherBack));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SMALL_SHIP_WEAPON_UNIVERSAL_LAUNCHER_FRONT, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_SmallShipWeapon_UniversalLauncherFront));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P385_A01_TEMPLE_900M, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p385_a01_temple_900m));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P384_A01_HOSPITAL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p384_a01_hospital));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P383_A01_CHURCH, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p383_a01_church));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P334_A01_FOOD_GROW, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p334_a01_food_grow));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P345_A01_BIO_EXP, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p345_a01_bio_exp));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P345_A01_BIO_MACH_EXP, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p345_a01_bio_mach_exp));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P345_A01_RECYCLE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p345_a01_recycle));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P541_ESCAPE_POD, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p541_escape_pod));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P541_ESCAPE_POD_BASE, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p541_escape_pod_base));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P541_VENTILATOR_BODY, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p541_ventilator_body));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P541_VENTILATOR_PROPELLER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p541_ventilator_propeller));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P349_A_TOWER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p349_a_tower));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P349_B_TOWER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p349_b_tower));
            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P349_C_TOWER, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.Blueprint_p349_c_tower));

            //AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.P361_A02_HANGAR_PANEL, new MyGuiSmallShipHelperBlueprint(MyTextsWrapperEnum.p361_a02_hangar_panel));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit, 
                new MyGuiSmallShipHelperBlueprint(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BlueprintAdvancedConstructionKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.BlueprintAdvancedConstructionKit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit,
                new MyGuiSmallShipHelperBlueprint(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BlueprintBasicConstructionKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.BlueprintBasicConstructionKit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit,
                new MyGuiSmallShipHelperBlueprint(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BlueprintFortificationKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.BlueprintFortificationKit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit,
                new MyGuiSmallShipHelperBlueprint(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BlueprintHonorableKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.BlueprintHonorableKit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit,
                new MyGuiSmallShipHelperBlueprint(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BlueprintSuperiorConstructionKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.BlueprintSuperiorConstructionKit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit,
                new MyGuiSmallShipHelperBlueprint(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BlueprintUtilitiesKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.BlueprintUtilitiesKit));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit,
                new MyGuiSmallShipHelperBlueprint(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BlueprintWeaponKit", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.BlueprintWeaponKit));
            #endregion

            #region Foundation factory
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FoundationFactory, 0, new MyGuiFoundationFactoryHelper(MyTextsWrapperEnum.FoundationFactory));
            #endregion

            #region Spawn point
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SpawnPoint, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Inventory
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Inventory, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Inventory item
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.InventoryItem, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Checkpoint
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Checkpoint, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Player
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Player, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Player statistics
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PlayerStatistics, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Session
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Session, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Event
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Event, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Object to build
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.ObjectToBuild, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region Object Group
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.ObjectGroup, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region prefab largeship
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip, (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_ARDANT,
            new MyGuiPrefabHelper(MyTextsWrapperEnum.LargeShipArdant));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip, (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_KAI,
            new MyGuiPrefabHelper(MyTextsWrapperEnum.LargeShipKai));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip, (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.LARGESHIP_SAYA,
            new MyGuiPrefabHelper(MyTextsWrapperEnum.LargeShipSaya));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                         (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.FOURTH_REICH_MOTHERSHIP,
                         new MyGuiPrefabHelper(MyTextsWrapperEnum.FourthReichMothership));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                         (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.FOURTH_REICH_MOTHERSHIP_B,
                         new MyGuiPrefabHelper(MyTextsWrapperEnum.FourthReichMothership_B));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                         (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.RUS_MOTHERSHIP,
                         new MyGuiPrefabHelper(MyTextsWrapperEnum.RusMothership));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.RUSSIAN_MOTHERSHIP_HUMMER,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.RussianMothershipHummer));

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_BODY,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_body));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_ENGINE,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_engine));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_LARGE_LEFT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_back_large_left));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_LARGE_RIGHT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_back_large_right));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_SMALL_LEFT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_back_small_left));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_BACK_SMALL_RIGHT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_back_small_right));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_LARGE_LEFT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_front_large_left));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_LARGE_RIGHT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_front_large_right));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL_LEFT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_front_small_left));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL_RIGHT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_front_small_right));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL02_LEFT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_front_small02_left));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeShip,
                        (int)MyMwcObjectBuilder_PrefabLargeShip_TypesEnum.MSHIP_SHIELD_FRONT_SMALL02_RIGHT,
                        new MyGuiPrefabHelper(MyTextsWrapperEnum.Mship_shield_front_small02_right));
            #endregion

            #region prefab largeship weapons
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_AUTOCANNON,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponAutocannon", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextsWrapperEnum.p351_a01_largeship_autocannon));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_CIWS,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponCIWS", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextsWrapperEnum.p351_a01_largeship_ciws));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MACHINEGUN,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponMachinegun", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextsWrapperEnum.p351_a01_largeship_machinegun));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC4,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponMissile4", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextsWrapperEnum.p351_a01_largeship_missile_gun4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC6,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponMissile6", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextsWrapperEnum.p351_a01_largeship_missile_gun6));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A01_LARGESHIP_MISSILE_BASIC9,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponMissile9", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextsWrapperEnum.p351_a01_largeship_missile_gun9));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponMissile4", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponMissile6", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), 
                    MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided6));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabLargeWeapon, (int)MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeWeaponMissile9", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy),
                    MyTextsWrapperEnum.p351_a02_largeship_missile_gun_guided9));
            #endregion

            #region prefab hangars
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabHangar, (int)MyMwcObjectBuilder_PrefabHangar_TypesEnum.VENDOR, new MyGuiPrefabHelper(MyTextsWrapperEnum.vendor));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabHangar, (int)MyMwcObjectBuilder_PrefabHangar_TypesEnum.HANGAR, new MyGuiPrefabHelper(MyTextsWrapperEnum.Hangar));
            #endregion

            #region prefab kinematic parts
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_A02_DOOR_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_a02_door));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_A02_DOOR_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p415_a02_door));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C02_DOOR1_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C02_DOOR1_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_B_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A_LEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P415_C03_DOOR2_A_RIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORLEFT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, (int)MyMwcObjectBuilder_PrefabKinematicPart_TypesEnum.P341_A01_OPEN_DOCK_VARIATION1_DOORRIGHT, new MyGuiPrefabHelper(MyTextsWrapperEnum.p571_l01_traffic_sign));            
            #endregion

            #region dummy point
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.DummyPoint, 0, new MyGuiPrefabHelper(MyTextsWrapperEnum.DummyPoint));
            #endregion

            #region Snap Point Links
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SnapPointLink, 0, new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region entity detector
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.EntityDetector, (int)MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere, new MyGuiPrefabHelper(MyTextsWrapperEnum.DummyPoint));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.EntityDetector, (int)MyMwcObjectBuilder_EntityDetector_TypesEnum.Box, new MyGuiPrefabHelper(MyTextsWrapperEnum.DummyPoint));
            #endregion

            #region ship config
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.ShipConfig, 0, new MyGuiPrefabHelper(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region prefab foundation factory
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory, (int)MyMwcObjectBuilder_PrefabFoundationFactory_TypesEnum.DEFAULT,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\FoundationFactory", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.FoundationFactory));            
            #endregion

            #region prefab security control HUB
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB, (int)MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum.DEFAULT,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\FoundationFactory", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.SecurityControlHUB));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB, (int)MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum.P541_SCREEN_A,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\FoundationFactory", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.p541_screen_A));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB, (int)MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum.P541_SCREEN_B,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\FoundationFactory", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.p541_screen_B));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabSecurityControlHUB, (int)MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum.P541_TERMINAL_A,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\FoundationFactory", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.p541_terminal_A));            
            #endregion

            #region Drones

            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Drone, (int) MyMwcObjectBuilder_Drone_TypesEnum.DroneUS,
                         new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\DroneUS",
                                                                                               flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.DroneUS));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Drone, (int) MyMwcObjectBuilder_Drone_TypesEnum.DroneCN,
                         new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\DroneCN",
                                                                                               flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.DroneCN));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Drone, (int) MyMwcObjectBuilder_Drone_TypesEnum.DroneSS,
                         new MyGuiSmallShipHelperTool(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\DroneSS",
                                                                                               flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.DroneSS));

            #endregion

            #region false Id
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.China), new MyGuiHelperBase(MyTextsWrapperEnum.FactionChinese));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Church), new MyGuiHelperBase(MyTextsWrapperEnum.FactionChurch));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.CSR), new MyGuiHelperBase(MyTextsWrapperEnum.FactionCSR));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Euroamerican), new MyGuiHelperBase(MyTextsWrapperEnum.FactionEuroamerican));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.FourthReich), new MyGuiHelperBase(MyTextsWrapperEnum.FactionFourthReich));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.FreeAsia), new MyGuiHelperBase(MyTextsWrapperEnum.FactionFreeAsia));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Freelancers), new MyGuiHelperBase(MyTextsWrapperEnum.FactionFreelancers));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.FSRE), new MyGuiHelperBase(MyTextsWrapperEnum.FactionFSRE));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.India), new MyGuiHelperBase(MyTextsWrapperEnum.FactionIndia));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Japan), new MyGuiHelperBase(MyTextsWrapperEnum.FactionJapan));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Miners), new MyGuiHelperBase(MyTextsWrapperEnum.FactionMiners));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.None), new MyGuiHelperBase(MyTextsWrapperEnum.FactionNone));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Omnicorp), new MyGuiHelperBase(MyTextsWrapperEnum.FactionOmnicorp));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Pirates), new MyGuiHelperBase(MyTextsWrapperEnum.FactionPirate));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Rangers), new MyGuiHelperBase(MyTextsWrapperEnum.FactionRangers));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Ravens), new MyGuiHelperBase(MyTextsWrapperEnum.FactionRavens));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Russian), new MyGuiHelperBase(MyTextsWrapperEnum.FactionRussian));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Saudi), new MyGuiHelperBase(MyTextsWrapperEnum.FactionSaudi));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.SMLtd), new MyGuiHelperBase(MyTextsWrapperEnum.FactionSMLtd));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Syndicate), new MyGuiHelperBase(MyTextsWrapperEnum.FactionSyndicate));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Templars), new MyGuiHelperBase(MyTextsWrapperEnum.FactionTemplars));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Traders), new MyGuiHelperBase(MyTextsWrapperEnum.FactionTraders));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.TTLtd), new MyGuiHelperBase(MyTextsWrapperEnum.FactionTTLtd));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Russian_KGB), new MyGuiHelperBase(MyTextsWrapperEnum.FactionRussianKGB));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Slavers), new MyGuiHelperBase(MyTextsWrapperEnum.FactionSlavers));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.WhiteWolves), new MyGuiHelperBase(MyTextsWrapperEnum.FactionWhiteWolves));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(MyMwcObjectBuilder_FactionEnum.Rainiers), new MyGuiHelperBase(MyTextsWrapperEnum.FactionRainiers));            
            #endregion

            #region hacking tool
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_1,
                new MyGuiHelperBase(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\HackingTool1", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.HackingTool_Level1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2,
                new MyGuiHelperBase(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\HackingTool2", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.HackingTool_Level2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_3,
                new MyGuiHelperBase(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\HackingTool3", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.HackingTool_Level3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_4,
                new MyGuiHelperBase(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\HackingTool4", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.HackingTool_Level4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_5,
                new MyGuiHelperBase(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\HackingTool5", flags: TextureFlags.IgnoreQuality), MyTextsWrapperEnum.HackingTool_Level5));
            #endregion

            #region prefab bank node
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabBankNode, (int)MyMwcObjectBuilder_PrefabBankNode_TypesEnum.DEFAULT,
                new MyGuiPrefabHelper(MyTextsWrapperEnum.BankNode));            
            #endregion

            #region prefab use properties
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.EntityUseProperties, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region generator
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabGenerator, (int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C01_INERTIA_GENERATOR, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Generator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.Generator));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabGenerator, (int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C03_CENTRIFUGE, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Generator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p321c03_centrifuge));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabGenerator, (int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C04_BOX_GENERATOR, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Generator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p321c04_box_generator));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabGenerator, (int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C05_CENTRIFUGE_BIG, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Generator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p321c05_centrifuge_big));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabGenerator, (int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C02_GENERATOR_WALL_BIG, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Generator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p321c02_generator_wall_big));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabGenerator, (int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C06_INERTIA_GENERATOR_B, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Generator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p321c06_inertia_generator_B));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabGenerator, (int)MyMwcObjectBuilder_PrefabGenerator_TypesEnum.P321C07_GENERATOR, 
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Generator", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.p321c07_generator));
            #endregion

            #region cargo box
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type1,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType1));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type2,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType2));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type3,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType3));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type4,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType4));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type5,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType5));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type6,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType6));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type7,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType7));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type8,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType8));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type9,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType9));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type10,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType10));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type11,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType11));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.Type12,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxType12));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_A,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxTypeProp_A));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_B,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxTypeProp_B));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_C,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxTypeProp_C));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.TypeProp_D,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxTypeProp_D));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.CargoBox, (int)MyMwcObjectBuilder_CargoBox_TypesEnum.DroppedItems,
                new MyGuiHelperBase(MyTextsWrapperEnum.CargoBoxDroppedItems));            
            #endregion

            #region Mysterious cube
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.MysteriousCube, (int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type1,
                new MyGuiHelperBase(MyTextsWrapperEnum.MysteriousCube_01));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.MysteriousCube, (int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type2,
                new MyGuiHelperBase(MyTextsWrapperEnum.MysteriousCube_02));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.MysteriousCube, (int)MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type3,
                new MyGuiHelperBase(MyTextsWrapperEnum.MysteriousCube_03));
            #endregion

            #region SectorObjectGroups
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SectorObjectGroups, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region WaypointNew
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.WaypointNew, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region prefab Scanner
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabScanner, (int)MyMwcObjectBuilder_PrefabScanner_TypesEnum.Plane,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Scanner", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.ScannerPlane));
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabScanner, (int)MyMwcObjectBuilder_PrefabScanner_TypesEnum.Rays,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Scanner", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.ScannerRays));
            #endregion

            #region prefab Camera
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabCamera, (int)MyMwcObjectBuilder_PrefabCamera_TypesEnum.DEFAULT,
                new MyGuiPrefabHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Camera", flags: TextureFlags.IgnoreQuality, loadingMode: Managers.LoadingMode.Lazy), MyTextsWrapperEnum.Camera));
            #endregion

            #region prefab alarm
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabAlarm, (int)MyMwcObjectBuilder_PrefabAlarm_TypesEnum.DEFAULT,
                new MyGuiPrefabHelper(MyTextsWrapperEnum.Alarm));            
            #endregion

            #region alien gate
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.Prefab, (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.P391_ALIEN_GATE,
                new MyGuiPrefabHelper(MyTextsWrapperEnum.AlienGate));
            #endregion            

            #region prefab kinematic rotating part
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.PrefabKinematicRotatingPart, (int)MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum.DEFAULT,
                new MyGuiPrefabHelper(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region small ship template
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShipTemplate, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region small ship templates
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShipTemplates, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region faction relation changes
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.FactionRelationChange, 0,
                new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region RemotePlayer
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.RemotePlayer, 0, new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            #region GlobalData
            AddGuiHelper(MyMwcObjectBuilderTypeEnum.GlobalData, 0, new MyGuiHelperBase(MyTextsWrapperEnum.EmptyDescription));
            #endregion

            foreach (ushort objectBuilderType in Enum.GetValues(typeof(MyMwcObjectBuilderTypeEnum)))
            {
                // check if there are valid ObjectBuilderIds for concrete ObjectBuilderType
                var guiHelpersForType = m_buildTypeHelpers[objectBuilderType];
                MyCommonDebugUtils.AssertDebug(guiHelpersForType != null, "Missing GUI helper for: " + (MyMwcObjectBuilderTypeEnum)objectBuilderType + ", use AddGuiHelper() in this file");
                for (int objectBuilderId = 0; objectBuilderId < guiHelpersForType.Length; objectBuilderId++)
                {
                    var guiHelper = guiHelpersForType[objectBuilderId];
                    if (guiHelper != null)
                    {
                        MyCommonDebugUtils.AssertDebug(MyMwcObjectBuilder_Base.IsObjectBuilderIdValid((MyMwcObjectBuilderTypeEnum)objectBuilderType, objectBuilderId));
                    }
                }

                // check if there are all objectbuilder's types and ids
                int[] objectBuilderIDs = MyMwcObjectBuilder_Base.GetObjectBuilderIDs((MyMwcObjectBuilderTypeEnum)objectBuilderType);
                foreach (int objectBuilderId in objectBuilderIDs)
                {
                    MyCommonDebugUtils.AssertDebug(guiHelpersForType[objectBuilderId] != null, string.Format("Object builder {0} does not have gui helper", (MyMwcObjectBuilderTypeEnum)objectBuilderId));
                }                
            }
        }

        public static void UnloadContent()
        {
            for (int i = 0; i < m_buildTypeHelpers.Length; i++)
            {
                m_buildTypeHelpers[i] = null;
            }
        }

        public static void ReloadTexts()
        {
            for (int i = 0; i < m_buildTypeHelpers.Length; i++)
            {
                if (m_buildTypeHelpers[i] != null)
                {
                    for (int j = 0; j < m_buildTypeHelpers[i].Length; j++)
                    {
                        if (m_buildTypeHelpers[i][j] != null)
                            m_buildTypeHelpers[i][j].IsNameDirty = true;
                    }
                }
            }
        }
         
        /// <summary>
        /// Because those icons that are not fetched from content are generated on-the-fly, 
        /// resolving prefab helper icons is done AFTER loading (so that GPU is free on main thread).
        /// </summary>
        internal static void UpdatePrefabHelperIcons()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("UpdatePrefabHelperIcons");
            MyMwcLog.WriteLine("GUpdatePrefabHelperIcons - START");

            // this whole thing here will need to be updated to take into account FactionSpecific attribute of a prefab configuration

            foreach (MyMwcObjectBuilderTypeEnum prefabType in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
            {
                // hotfix to stop rendering on-the-fly of fourth reich largeships for neutral faction
                if (prefabType == MyMwcObjectBuilderTypeEnum.PrefabLargeShip)
                    continue;

                foreach (int prefabId in MyMwcObjectBuilder_Base.GetObjectBuilderIDs(prefabType))
                {
                    if (prefabId == 477)
                        continue;

                    if (m_buildTypeHelpers[(int)prefabType] != null &&
                        m_buildTypeHelpers[(int)prefabType][prefabId] != null)
                    {
                        //m_buildTypeHelpers[(int)prefabType][prefabId].Icon = 
                        //    MyGuiManager.GetPrefabPreviewTexture(prefabType, prefabId, MyMwcObjectBuilder_Prefab_AppearanceEnum.None);
                        ((MyGuiPrefabHelper)m_buildTypeHelpers[(int)prefabType][prefabId]).Preview = 
                            MyGuiManager.GetPrefabPreviewTexture(prefabType, prefabId, MyMwcObjectBuilder_Prefab_AppearanceEnum.None);
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.WriteLine("GUpdatePrefabHelperIcons - END");
        }
    }
}
