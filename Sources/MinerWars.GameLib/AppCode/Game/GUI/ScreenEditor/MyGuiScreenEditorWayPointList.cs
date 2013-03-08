
using System;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Editor;
using MinerWarsMath;
using System.Text;
using MinerWars.AppCode.Game.Entities.WayPoints;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorWayPointList : MyGuiScreenBase
    {
        public MyGuiScreenEditorWayPointList()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.7f, 0.75f);

            AddCaption(MyTextsWrapperEnum.Waypoints, new Vector2(0,0.015f));

            var wayPointsListBox = new MyGuiControlListbox(this, new Vector2(0, -0.01f), new Vector2(0.55f, 0.1f), MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
                MyTextsWrapper.Get(MyTextsWrapperEnum.Waypoints), MyGuiConstants.LABEL_TEXT_SCALE, 1, 5, 1, false, true, false,
                null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 2, 1, MyGuiConstants.LISTBOX_BACKGROUND_COLOR_BLUE, 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);

            wayPointsListBox.ItemSelect += new OnListboxItemSelect(wayPointsListBox_ItemSelect);


            MyWayPointGraph.StoredPaths.Sort(Waypoint_Sort);
            
            for (int i = 0; i < MyWayPointGraph.StoredPaths.Count; i++)
			{
                wayPointsListBox.AddItem(i, new StringBuilder(MyWayPointGraph.StoredPaths[i].Name));
            }

            Controls.Add(wayPointsListBox);


            Controls.Add(new MyGuiControlButton(this, new Vector2(0.2080f, 0.28f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
        }

        private int Waypoint_Sort(MyWayPointPath myWayPointPath, MyWayPointPath wayPointPath)
        {
            return myWayPointPath.Name.CompareTo(wayPointPath.Name);
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        void wayPointsListBox_ItemSelect(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            var selectedPath = MyWayPointGraph.StoredPaths[eventArgs.Key];

            MyEditorGizmo.ClearSelection();

            MyWayPointGraph.SelectedPath = selectedPath;
            MyEditorGizmo.AddEntitiesToSelection(MyWayPointGraph.SelectedPath.WayPoints);
        }        

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorWayPointList";
        }

    }
}
