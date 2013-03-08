using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.GUI.Core
{
    interface IMyGuiControlsParent
    {
        MyGuiControls Controls { get; }
        Vector2 GetPositionAbsolute();
        Vector2? GetSize();
        float GetTransitionAlpha();
    }
}
