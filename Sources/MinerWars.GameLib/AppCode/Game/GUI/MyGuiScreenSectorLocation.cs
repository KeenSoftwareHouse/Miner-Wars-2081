using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenSectorLocation : MyGuiScreenBase
    {
        public override string GetFriendlyName()
        {
            return "Map";
        }

        public MyGuiScreenSectorLocation()
            : base(new Vector2(.5f, .5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.35f, 0.5f))
        {
            AddCaption(MyTextsWrapperEnum.SectorName, Color.Yellow.ToVector4());

            m_backgroundColor = MyGuiConstants.SCREEN_BACKGROUND_COLOR;
            m_enableBackgroundFade = true;
            m_canHaveFocus = true;
            m_closeOnEsc = true;
            m_isTopMostScreen = true;
            m_drawEvenWithoutFocus = true;


        }
    }
}
