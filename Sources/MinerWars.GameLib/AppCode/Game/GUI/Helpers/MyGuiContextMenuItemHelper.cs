using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiContextMenuItemHelper : MyGuiHelperBase
    {
        public MyGuiContextMenuItemActionType ActionType;
        MyTextsWrapperEnum? m_toolTip;
        public bool Enabled;
        public StringBuilder ToolTip { get { if (m_toolTip.HasValue) { return MyTextsWrapper.Get(m_toolTip.Value); } return null; } }

        public MyGuiContextMenuItemHelper(MyTextsWrapperEnum description, MyGuiContextMenuItemActionType actionType, bool enabled, MyTextsWrapperEnum? toolTip)
            : base(description)
        {
            ActionType = actionType;
            Enabled = enabled;
            m_toolTip = toolTip;
        }
    }
}
