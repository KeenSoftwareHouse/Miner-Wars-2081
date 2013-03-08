using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    abstract class MyRichLabelPart
    {
        public abstract Vector2 GetSize();        

        public abstract bool Draw(Vector2 position);
    }
}
