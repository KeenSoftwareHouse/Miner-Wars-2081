using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.WayPoints;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D
{
    class MyGuiScreenEditorWaypoint : MyGuiScreenEditorObject3DBase
    {
        MyGuiControlCheckbox m_secretCheckbox;
        List<MyWayPoint> m_waypoints;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorWayPoint";
        }

        public MyGuiScreenEditorWaypoint(MyWayPoint waypoint)
            : base(null, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.WayPoint)
        {
            m_waypoints = new List<MyWayPoint>();
            m_waypoints.Add(waypoint);
            Init();
        }

        public MyGuiScreenEditorWaypoint(List<MyWayPoint> waypoints)
            : base(null, new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null, MyTextsWrapperEnum.WayPoint)
        {
            System.Diagnostics.Debug.Assert(waypoints.Count >= 1);
            m_waypoints = waypoints;
            Init();
        }

        void Init()
        {
            m_enableBackgroundFade = true;
            m_size = new Vector2(0.5f, 0.3f);
            AddCaption();

            Vector2 left = new Vector2(-m_size.Value.X / 2.0f + 0.07f, -m_size.Value.Y / 2.0f + 0.11f);
            Vector2 right = new Vector2(m_size.Value.X / 2.0f - 0.07f, -m_size.Value.Y / 2.0f + 0.11f);

            bool anySecret = false;
            foreach (var v in m_waypoints) if (v.IsSecret)
            {
                anySecret = true;
                break;
            }

            Controls.Add(new MyGuiControlLabel(this, left, null, MyTextsWrapperEnum.Secret, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(m_secretCheckbox = new MyGuiControlCheckbox(this, right - new Vector2(MyGuiConstants.CHECKBOX_SIZE.X, 0), anySecret, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR));

            AddOkAndCancelButtonControls();
        }

        public override void OnOkClick(MyGuiControlButton sender)
        {
            base.OnOkClick(sender);
            foreach (var v in m_waypoints)
                v.IsSecret = m_secretCheckbox.Checked;
            MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);
        }
    }
}
