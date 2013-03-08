using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Prefabs
{
    abstract class MyGuiSceenEntityUseBase : MyGuiScreenBase
    {
        protected MyGuiSceenEntityUseBase(Vector2 position, Vector4? backgroundColor, Vector2? size) 
            : base(position, backgroundColor, size)
        {
            CanBeUnhidden = true;
        }

        protected MyGuiSceenEntityUseBase(Vector2 position, Vector4? backgroundColor, Vector2? size, bool isTopMostScreen, MyTexture2D backgroundTexture) 
            : base(position, backgroundColor, size, isTopMostScreen, backgroundTexture)
        {
            CanBeUnhidden = true;
        }

        public bool CanBeUnhidden { get; set; }

        public override bool UnhideScreen()
        {
            if ((m_state == MyGuiScreenState.UNHIDING) || (m_state == MyGuiScreenState.OPENED) || !CanBeUnhidden)
            {
                return false;
            }
            else
            {
                m_state = MyGuiScreenState.OPENED;
                m_transitionAlpha = MyGuiConstants.TRANSITION_ALPHA_MAX;
                OnShow();
                return true;
            }
        }
    }
}
