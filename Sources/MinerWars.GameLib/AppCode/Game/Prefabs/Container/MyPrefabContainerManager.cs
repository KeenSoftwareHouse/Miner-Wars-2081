using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Debugging;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Entities.Prefabs
{
    class MyPrefabContainerManager
    {
        private static MyPrefabContainerManager m_Instance = null;
        private List<MyPrefabContainer> m_containerList = new List<MyPrefabContainer>();
        private bool m_debugDraw = false;
        private bool m_enableAABBUnderMouse = false;
        private Vector4 m_dbgColor = new Vector4(0.6f, 0.6f, 0.3f, 0.6f);
        private Vector4 m_dbgColorNodes = new Vector4(1f, 0.5f, 0.1f, 0.6f);

        private static Guid C_DBG_DRAW_ID = Guid.NewGuid();

        //////////////////////////////////////////////////////////////////////////
        private MyPrefabContainerManager() 
        {
            short modifier = (short)MyShortcut.ModifierValue.Control + (short)MyShortcut.ModifierValue.Shift;
            string category = "MyPrefabContainerManager";

            //@ debug draw Container
            MyShortcut shortcut = new MyShortcut(Keys.P, modifier);
            MyDebugConsole.GetInstance().RegisterShortcut(MyDebugSystem.Editor, category, "Debug draw container", shortcut, OnToggleDebugDraw);
            
            //@ invalidate nodes
            shortcut = new MyShortcut(Keys.L, modifier);
            MyDebugConsole.GetInstance().RegisterShortcut(MyDebugSystem.Editor, category, "ShowPhysicsAABBUnderCursor", shortcut, OnShowPhysAABB);



            MyRender.RegisterRenderModule(MyRenderModuleEnum.PrefabContainerManager, "Prefab container manager", DebugDraw, MyRenderStage.DebugDraw);
            //@ check res if duplicity registration
        }

        public static MyPrefabContainerManager GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new MyPrefabContainerManager();

            return m_Instance;
        }


        /// <summary>
        /// Enable/Disable debugdraw called form debugConsole
        /// </summary>
        /// <param name="shortcut"></param>
        public void OnToggleDebugDraw(MyDebugConsole.MyDebugConsoleItem sender)
        {
            m_debugDraw = !m_debugDraw;
            return;
        }

        /// <summary>
        /// OnShowPhysAABB
        /// </summary>
        /// <param name="sender"></param>
        public void OnShowPhysAABB(MyDebugConsole.MyDebugConsoleItem sender)
        {
            m_enableAABBUnderMouse = !m_enableAABBUnderMouse;
            MyEditor.Static.EnablePhysAABBUnderMouse(m_enableAABBUnderMouse);
        }

        /// <summary>
        ///  returns cont number
        /// </summary>
        /// <returns></returns>
        public int GetContainerCount()
        {
            return m_containerList.Count;
        }

        /// <summary>
        /// AddContainer
        /// </summary>
        /// <param name="container"></param>
        public void AddContainer(MyPrefabContainer container)
        {
            m_containerList.Add(container);
        }


        /// <summary>
        /// RemoveContainer
        /// </summary>
        /// <param name="container"></param>
        public void RemoveContainer(MyPrefabContainer container)
        {
            m_containerList.Remove(container);
        }


        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            m_containerList.Clear();
        }

        /// <summary>
        /// Draw bounding area of all containers(helps for overall orientation)
        /// </summary>
        public void DrawContainersBoundingArea()
        {
            if (MyEditor.Static.GetEditedPrefabContainer() == null)
            {
                foreach (MyPrefabContainer container in m_containerList)
                {
                    container.DrawSelectionBoxAndBounding(MyEditorGizmo.IsEntityOrItsParentSelected(container), false);

                    /*
                    if (MyEditor.DisplayPrefabContainerBounding == true)
                        container.DrawContainerBounding();
                    container.DrawSelectionBoxAndBounding(MyEditorGizmo.IsEntityOrItsParentSelected(container), false);
                    */
                }
            }
            else
            {
                MyEditor.Static.GetEditedPrefabContainer().DrawSelectionBoxAndBounding(false, true);
                /*
                MyEditor.Static.GetEditedPrefabContainer().DrawSelectionBox(false, true);
                if (MyEditor.DisplayPrefabContainerBounding == true)
                    MyEditor.Static.GetEditedPrefabContainer().DrawContainerBounding();
                */
            }
        }

        /// <summary>
        /// "Multi"DebugDraw
        /// </summary>
        public void DebugDraw()
        {
            if (m_debugDraw)
            {
                foreach (MyPrefabContainer container in m_containerList)
                {
                    //@ TODO wrong -> need to rotate!
                    container.UpdateAABBHr();
                    BoundingBox aabb = container.WorldAABBHr;
                    if (MyCamera.IsInFrustum(ref aabb) == true)
                        container.DebugDraw();
                }
            }
        }

        /// <summary>
        /// UnloadContent
        /// </summary>
        public void UnloadContent()
        {
            MyMwcLog.WriteLine("MyPrefabContainerManager.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyPrefabContainerManager.UnloadContent - END");
        }

        /// <summary>
        /// UnloadContent
        /// </summary>
        public void UnloadData()
        {
            foreach (MyPrefabContainer container in m_containerList)
            {
                //container.ClearSubCube();
            }
        }
    }
}
