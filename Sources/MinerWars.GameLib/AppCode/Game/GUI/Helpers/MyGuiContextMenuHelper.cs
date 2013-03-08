using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiContextMenuHelper : MyGuiHelperBase
    {
        public List<MyGuiContextMenuItemHelper> MenuItemHelpers;

        public MyGuiContextMenuHelper(MyTextsWrapperEnum description, List<MyGuiContextMenuItemHelper> menuItemHelpers)
            : base(description)
        {
            MenuItemHelpers = menuItemHelpers;
        }

        public bool IsActionTypeAvailable(MyGuiContextMenuItemActionType actionType)
        {
            return MenuItemHelpers.Exists(a => a.ActionType == actionType);
        }

    }
}
