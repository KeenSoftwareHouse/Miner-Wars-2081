
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiPrefabHelper : MyGuiHelperBase
    {
        private MyTexture2D m_preview;
        public MyTexture2D Preview { get { return m_preview; } set { m_preview = value; } }

        public override MyTexture2D Icon
        {
            get
            {
                // if has no or empty icon and has some preview, then return preview
                if ((base.Icon == null || base.Icon == MyGuiManager.GetBlankTexture()) && 
                    Preview != null && Preview != MyGuiManager.GetBlankTexture())
                {
                    return Preview;
                }
                else
                {
                    return base.Icon;
                }
            }
            set
            {
                base.Icon = value;
            }
        }

        public MyGuiPrefabHelper(MyTextsWrapperEnum description)
            : base(description)
        {
            Icon = MyGuiManager.GetBlankTexture();
        }

        public MyGuiPrefabHelper(MyTexture2D icon, MyTextsWrapperEnum description)
            : base(icon, description)
        {
        }
    }
}
