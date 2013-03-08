using System;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    static class MyGuiSmallDebrisHelpers
    {
        //Dictionaries for helpers
        static Dictionary<MyMwcObjectBuilder_SmallDebris_TypesEnum, MyGuiSmallDebrisHelper> m_smallDebrisHelpers = new Dictionary<MyMwcObjectBuilder_SmallDebris_TypesEnum, MyGuiSmallDebrisHelper>();

        //Arrays of enums values
        public static Array MyMwcSmallDebrisEnumValues { get; private set; }

        static MyGuiSmallDebrisHelpers()
        {
            MyMwcSmallDebrisEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_SmallDebris_TypesEnum));
        }

        public static MyGuiSmallDebrisHelper GetMyGuiSmallDebrisHelper(MyMwcObjectBuilder_SmallDebris_TypesEnum smallDebrisEnum)
        {
            MyGuiSmallDebrisHelper ret;
            if (m_smallDebrisHelpers.TryGetValue(smallDebrisEnum, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {
            m_smallDebrisHelpers.Clear();
        }

        public static void LoadContent()
        {        /*
            #region Create small debris helpers

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Cistern,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Cistern));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris1,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Debris1));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris2,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Debris2));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris3,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Debris3));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris4,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Debris4));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris5,
                new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.Debris5));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris6,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris6));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris7,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris7));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris8,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris8));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris9,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris9));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris10,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris10));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris11,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris11));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris12,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris12));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris13,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris13));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris14,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris14));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris15,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris15));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris16,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris16));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris17,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris17));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris18,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris18));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris19,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris19));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris20,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris20));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris21,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris21));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris22,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris22));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris23,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris23));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris24,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris24));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris25,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris25));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris26,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris26));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris27,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris27));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris28,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris28));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris29,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris29));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris30,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris30));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris31,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris31));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Debris32_pilot,
               new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\Cistern", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.Debris32_pilot));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.pipe_bundle,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\BundleOfPipes", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.pipe_bundle));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_1,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\StandardContainer01", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.Standard_Container_1));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_2,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\StandardContainer02", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.Standard_Container_2));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_3,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\StandardContainer03", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.Standard_Container_3));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.Standard_Container_4,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\StandardContainer04", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.Standard_Container_4));

            m_smallDebrisHelpers.Add(MyMwcObjectBuilder_SmallDebris_TypesEnum.UtilityVehicle_1,
              new MyGuiSmallDebrisHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\UtilityVehicle01", flags: TextureFlags.IgnoreQuality),
                  MyTextsWrapperEnum.UtilityVehicle_1));

            #endregion
        */
        }
    }
}
