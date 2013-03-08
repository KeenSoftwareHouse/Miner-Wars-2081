using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiInfluenceSphereHelper : MyGuiHelperBase
    {
        public MyGuiInfluenceSphereHelper(MyTexture2D icon, MyTextsWrapperEnum description)
            : base(icon, description)
        {
        }

        public MyGuiInfluenceSphereHelper(MyTexture2D icon, string description)
            : base(icon, description)
        {
        }
    }
}
