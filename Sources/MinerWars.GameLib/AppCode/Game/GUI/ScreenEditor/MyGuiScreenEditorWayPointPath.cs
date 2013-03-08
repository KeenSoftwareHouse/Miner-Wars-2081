using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorWayPointPath : MyGuiScreenEditorDialogBase
    {
        MyWayPointPath m_waypointPath;
        MyGuiControlTextbox m_groupNameTextbox;
        bool m_newlyAdded;

        Vector2? m_screenPosition;

        public MyGuiScreenEditorWayPointPath(MyWayPointPath path, bool newlyAdded)
            : base(new Vector2(0.5f, 0.5f), newlyAdded ? new Vector2(0.4f, 0.25f) : new Vector2(0.32f, 0.25f))
        {
            m_waypointPath = path;
            m_newlyAdded = newlyAdded;
            Init();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorWayPointPath";
        }

        void Init()
        {
            string name = m_waypointPath != null ? m_waypointPath.Name : string.Empty;

            m_size = m_newlyAdded ? new Vector2(0.42f, 0.35f) : new Vector2(0.42f, 0.35f);

            Vector2 controlsOriginLeft = GetControlsOriginLeftFromScreenSize();

            // Add screen title
            AddCaption(m_newlyAdded ? MyTextsWrapperEnum.WaypointPathNameCreateCaption : MyTextsWrapperEnum.WaypointPathNameCaption);

            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 0 * MyGuiConstants.CONTROLS_DELTA, null, MyTextsWrapperEnum.WaypointPathName, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_groupNameTextbox = new MyGuiControlTextbox(this,
                controlsOriginLeft + 0.75f * MyGuiConstants.CONTROLS_DELTA + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2, 0),
                MyGuiControlPreDefinedSize.MEDIUM,
                  name, MyWaypointConstants.MAXIMUM_WAYPOINT_PATH_NAME_LENGTH,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_groupNameTextbox);

            AddOkAndCancelButtonControls();
        }

        protected override void OnOkClick(MyGuiControlButton sender)
        {
            if (m_waypointPath != null)
            {
                string name = m_groupNameTextbox.Text;

                // empty or duplicate
                if (m_waypointPath.ChangeName(name) == false)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(
                        MyMessageBoxType.ERROR,
                        name == null || name.Length == 0 ? new StringBuilder("The name can't be empty.") : new StringBuilder().AppendFormat("The name {0} is already used.", name),
                        new StringBuilder("Error"),
                        MyTextsWrapperEnum.Ok,
                        r => { }
                    ));
                    return;
                }
            }
            CloseScreen();
        }

        protected override void OnCancelClick(MyGuiControlButton sender)
        {
            if (m_waypointPath != null)
                if (m_newlyAdded)
                    MyWayPointGraph.RemovePath(MyWayPointGraph.SelectedPath);
            CloseScreen();
        }

        private Vector2 GetControlsOriginLeftFromScreenSize()
        {
            return new Vector2(-m_size.Value.X / 2.0f + 0.03f, -m_size.Value.Y / 2.0f + 0.09f);
        }
    }
}
