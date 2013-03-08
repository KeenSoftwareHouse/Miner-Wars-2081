using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Debugging;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Editor
{
    class MyEditorShortcutManager
    {
        private static MyEditorShortcutManager m_Instance = null;


        /// <summary>
        /// GetInstance
        /// </summary>
        /// <returns></returns>
        public static MyEditorShortcutManager GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new MyEditorShortcutManager();

            return m_Instance;
        }


        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            short modifier = (short)MyShortcut.ModifierValue.Control + (short)MyShortcut.ModifierValue.Shift;

            //@ reset camera
            MyShortcut shortcut = new MyShortcut(Keys.R, modifier);
            MyDebugConsole.GetInstance().RegisterShortcut(MyDebugSystem.Editor, "EditorShortcutManager", "Reset camera", shortcut, OnResetCamera);

            //@ reset object rotation
            short resObjMod = (short)MyShortcut.ModifierValue.Control;
            shortcut = new MyShortcut(Keys.R, resObjMod);
            MyDebugConsole.GetInstance().RegisterShortcut(MyDebugSystem.Editor, "EditorShortcutManager", "Reset object rotation", shortcut, OnRotationScaleReset);
        }


        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            short modifier = (short)MyShortcut.ModifierValue.Control + (short)MyShortcut.ModifierValue.Shift;
            MyShortcut shortcut = new MyShortcut(Keys.R, modifier);
            MyDebugConsole.GetInstance().UnregisterShortcut(shortcut);

            short resObjMod = (short)MyShortcut.ModifierValue.Control;
            shortcut = new MyShortcut(Keys.R, resObjMod);
            MyDebugConsole.GetInstance().UnregisterShortcut(shortcut);
        }



        /// <summary>
        /// OnResetCamera
        /// </summary>
        /// <param name="sender"></param>
        public void OnResetCamera(MyDebugConsole.MyDebugConsoleItem sender)
        {
            MySpectator.ResetSpectatorView();
        }


        /// <summary>
        /// OnResetObjectRotation
        /// </summary>
        /// <param name="sender"></param>
        public void OnRotationScaleReset(MyDebugConsole.MyDebugConsoleItem sender)
        {
            if (MyEditor.Static.IsActive())
            {
                //@ getSelection - reset rot on selected objects!
                List<MyEntity> selection = MyEditorGizmo.SelectedEntities;
                if (selection.Count == 0)
                    return;

                foreach(MyEntity entity in selection)
                {
                    Matrix mat = Matrix.Identity;
                    mat.Translation = entity.WorldMatrix.Translation;
                    entity.WorldMatrix = mat;
                }
            }
        }

    }
}
