using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiSmallShipHelperRadar : MyGuiHelperBase
    {
        public MyGuiSmallShipHelperRadar(MyTextsWrapperEnum description)
            : base(description)
        {
        }

        public MyGuiSmallShipHelperRadar(MyTexture2D icon, MyTextsWrapperEnum description) 
            : base (icon, description)
        {
        }
    }
}
