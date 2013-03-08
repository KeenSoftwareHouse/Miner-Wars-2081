using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiSmallShipHelperArmor : MyGuiHelperBase
    {
        public MyGuiSmallShipHelperArmor(MyTextsWrapperEnum description)
            : base(description)
        {
        }

        public MyGuiSmallShipHelperArmor(MyTexture2D icon, MyTextsWrapperEnum description) 
            : base (icon, description)
        {
        }
    }
}
