using System;
using System.Collections.Generic;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    static class MyGuiLargeShipHelpers
    {

        //Dictionaries for helpers
        static Dictionary<MyMwcObjectBuilder_LargeShip_TypesEnum, MyGuiLargeShipHelper> m_largeShipHelpers = new Dictionary<MyMwcObjectBuilder_LargeShip_TypesEnum, MyGuiLargeShipHelper>();

        //Arrays of enums values
        public static Array MyMwcLargeShipTypesEnumValues { get; private set; }

        static MyGuiLargeShipHelpers()
        {
            MyMwcLargeShipTypesEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_LargeShip_TypesEnum));
        }

        public static MyGuiLargeShipHelper GetMyGuiLargeShipHelper(MyMwcObjectBuilder_LargeShip_TypesEnum largeShipEnum)
        {
            MyGuiLargeShipHelper ret;
            if (m_largeShipHelpers.TryGetValue(largeShipEnum, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {
            m_largeShipHelpers.Clear();
        }

        public static void LoadContent()
        {

            #region Create large ship helpers

            m_largeShipHelpers.Add(MyMwcObjectBuilder_LargeShip_TypesEnum.KAI,
                new MyGuiLargeShipHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeShipKai", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.LargeShipKai));

            m_largeShipHelpers.Add(MyMwcObjectBuilder_LargeShip_TypesEnum.MOTHERSHIP_SAYA,
               new MyGuiLargeShipHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeShipSaya", flags: TextureFlags.IgnoreQuality),
                   MyTextsWrapperEnum.LargeShipSaya));

            m_largeShipHelpers.Add(MyMwcObjectBuilder_LargeShip_TypesEnum.JEROMIE_INTERIOR_STATION,
                new MyGuiLargeShipHelper(null,
                    MyTextsWrapperEnum.Largeship));

            m_largeShipHelpers.Add(MyMwcObjectBuilder_LargeShip_TypesEnum.ARDANT,
                new MyGuiLargeShipHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\LargeShipArdant", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.LargeShipArdant));

            #endregion

        }
    }
}
