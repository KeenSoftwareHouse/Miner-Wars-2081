using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers {
    class MyGuiEventHelper : MyGuiHelperBase
    {
        private Vector4 m_backgroundColor;


        public MyGuiEventHelper(MyTexture2D icon, MyTextsWrapperEnum description, Vector4 color)
            : base(icon, description)
        {
            BackgroundColor = color;
        }

        public Vector4 BackgroundColor
        {
            get { return m_backgroundColor; }
            set { m_backgroundColor = value; }
        }

    }
}
