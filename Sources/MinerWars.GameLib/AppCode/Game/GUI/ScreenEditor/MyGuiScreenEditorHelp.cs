using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Toolkit.Input;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenEditorHelp : MyGuiScreenBase
    {
        public MyGuiScreenEditorHelp()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.75f, 0.9f))
        {
            m_enableBackgroundFade = true;

            m_size = new Vector2(0.85f, 1f);

            AddCaption(MyTextsWrapperEnum.EditorHelpCaption, new Vector2(0, 0.013f));

            List<string> texts = new List<string>();
            texts.Add("Prefab - Add multiple prefabs to create complex structures. Prefabs must be in a prefab container.");
            texts.Add("Prefab Container (green box). Contains prefabs. You can edit only prefabs in selected container.");
            texts.Add("In editor options, you may enable/disable locked prefabs rotation by 90 degrees or by 1 degree.");
            texts.Add("");

            texts.Add("W,S,A,D,E,C,F or RMB - camera movement");
            texts.Add("V - Toggle Voxel Hand (add/delete/shape asteroids)");

            texts.Add("Insert - add new object");
            texts.Add("Hold Ctrl - slow down camera movement");
            texts.Add("MouseWheel - changes camera movement speed");
            texts.Add("+/- - increase/decrease grid step size");

            texts.Add("Shift + RMB - creates object copy and immediately attaches copy to camera");
            texts.Add("Ctrl + RMB - attach selection to camera");
            texts.Add("Alt + RMB - move camera to selection");
            texts.Add("Ctrl + A/D - select/unselect all objects");

            texts.Add("O - Repair Object");
            texts.Add("Ctrl + M - cut voxels to make space for selected prefabs");
            texts.Add("Delete - removes all selected entities");
            texts.Add("Ctrl + 1-9 - store selection/camera position ; 1-9 - restore selection/camera position");
            texts.Add("LMB on prefab - enter into prefab container edit mode immediately and select prefab");
            texts.Add("LMB on green box - select whole container");

            texts.Add("=== while in \"voxel hand\" mode ===");
            texts.Add("Shift + MouseWheel - change voxel hand size");
            texts.Add("Ctrl + MouseWheel - change voxel hand distance");

            texts.Add("=== while in prefab container edit mode ===");
            texts.Add("P - show/hide snap points");
            texts.Add("Ctrl + RMB - exit prefab container edit mode");
            texts.Add("Enter - commits current container changes (optimizes it) (happens automatically every 50 seconds)");
            texts.Add("LMB on center box - exit edit mode and select whole container");

            texts.Add("=== while object is attached to camera ===");
            texts.Add("LMB - detach from camera and place object at its current position");
            texts.Add("Ctrl + MouseWheel - change distance of attached objects or Voxel Hand from camera");

            texts.Add("=== gizmo editor ===");
            texts.Add("Shift + L - toggle transformation lock");
            texts.Add("G - toggle grid snapping");
            texts.Add("Ctrl + T - toggle rotation snapping");
            texts.Add("Shift + Hold LMB - creates copy of object, drag to move new copy in selected gizmo's axis direction");
            texts.Add("Hold LMB on gizmo axis or Numpad1-9 - move/rotate");
            texts.Add("T - switch gizmo mode (Translation, Rotation)");
            texts.Add("Space - switch gizmo space (World, Local)");

            texts.Add("=== editor options ===");
            texts.Add("MiddleMouseButton - switches camera mode (RMB controlled rotation / FPS crosshair mode)");
            texts.Add("Ctrl + Y - enable/disable light in editor");
            texts.Add("Shift + R - switch free and locked prefab rotations");
            texts.Add("Shift + C - enable/disable prefab container maximum area displaying");
            texts.Add("Ctrl + Space - attach the player ship to the spectrator position");
            texts.Add("Shift + Space - move spectrator camera to the near position of the player ship");

            

            //texts.Add("Home - undo action");
            //texts.Add("End - redo action");
            //texts.Add("F10 - switch camera - only for programmers");
            //texts.Add("F11 - save all voxel maps into user folder - only for programmers");
            //texts.Add("F12 - debug screen - only for programmers");
            //texts.Add("Shift-F12 - bot debug screen - only for programmers");

            Vector2 position = -m_size.Value / 2.0f + new Vector2(0.055f, 0.095f);

            foreach (string s in texts)
            {
                Controls.Add(new MyGuiControlLabel(this, position, null, new StringBuilder(s),
                    MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE * 0.7f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                position.Y += 0.01725f;
            }

            Controls.Add(new MyGuiControlButton(this, -m_size.Value / 2.0f + m_size.Value - MyGuiConstants.BACK_BUTTON_SIZE / 2.0f - new Vector2(0.055f, 0.085f), MyGuiConstants.BACK_BUTTON_SIZE,
                MyGuiConstants.BACK_BUTTON_BACKGROUND_COLOR, MyTextsWrapperEnum.Back, MyGuiConstants.BACK_BUTTON_TEXT_COLOR,
                MyGuiConstants.BACK_BUTTON_TEXT_SCALE, OnBackClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorHelp";
        }

        //  Just close the screen
        public void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if (input.IsNewKeyPress(Keys.F1))
            {
                OnBackClick(null);
            }
        }
    }
}
