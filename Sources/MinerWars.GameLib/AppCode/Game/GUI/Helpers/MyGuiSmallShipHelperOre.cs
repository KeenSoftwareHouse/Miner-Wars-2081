using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiOreHelper : MyGuiHelperBase
    {
        private MyTextsWrapperEnum m_nameTextEnum;        

        public MyGuiOreHelper(MyTextsWrapperEnum description, MyTextsWrapperEnum name)
            : base(description)
        {
            m_nameTextEnum = name;
        }

        public MyGuiOreHelper(MyTexture2D icon, MyTextsWrapperEnum description, MyTextsWrapperEnum name) 
            : base (icon, description)
        {
            m_nameTextEnum = name;
        }

        protected override void UpdateName()
        {
            m_name = MyTextsWrapper.Get(m_nameTextEnum);
        }
    }
}
