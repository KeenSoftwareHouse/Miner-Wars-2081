using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using System.Text;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiSmallShipHelperSmallShip : MyGuiHelperBase
    {
        public MyTexture2D Preview;
        private MyTextsWrapperEnum m_nameTextEnum;        

        public MyGuiSmallShipHelperSmallShip(MyTexture2D icon, MyTexture2D preview, MyTextsWrapperEnum description, MyTextsWrapperEnum name)
            : base(icon, description)
        {
            Preview = preview;
            m_nameTextEnum = name;
        }

        protected override void UpdateName()
        {
            m_name = MyTextsWrapper.Get(m_nameTextEnum);
        }
    }
}
