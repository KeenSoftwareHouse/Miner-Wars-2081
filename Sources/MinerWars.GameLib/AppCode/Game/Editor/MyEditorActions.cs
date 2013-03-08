using System.Collections.Generic;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Editor actions history - used for Undo/Redo operations
    /// </summary>
    static class MyEditorActions
    {
        private static Stack<MyEditorActionBase> m_undoStack;
        private static Stack<MyEditorActionBase> m_redoList;

        /// <summary>
        /// Loads action history
        /// </summary>
        public static void LoadData()
        {
            m_undoStack = new Stack<MyEditorActionBase>();
            m_redoList = new Stack<MyEditorActionBase>();
        }

        /// <summary>
        /// Unloads action history
        /// </summary>
        public static void UnloadData()
        {
            ResetActionHistory();
        }

        /// <summary>
        /// Clears action history
        /// </summary>
        public static void ResetActionHistory()
        {
            if (m_undoStack != null) m_undoStack.Clear();
            if (m_redoList != null) m_redoList.Clear();
        }

        /// <summary>
        /// Adds action to action history
        /// </summary>
        /// <param name="editorAction"></param>
        public static void AddAction(MyEditorActionBase editorAction)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("editorAction.Perform()");
            editorAction.Perform();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("m_redoList.Clear()");
            m_redoList.Clear();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("m_undoStack.Push()");
            m_undoStack.Push(editorAction);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("if..");
            //While doing new actions, we must make room for new UNDO operations by removing old REDO actions
            if ((m_redoList != null && m_redoList.Count > 0) && (m_undoStack.Count == MyEditorConstants.MAX_UNDO_REDO_HISTORY_LIMIT))
            {
               // m_redoList.RemoveLast();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Undo action
        /// </summary>
        public static void Undo()
        {
            //if there are no actions, there is nothing to UNDO
            if (m_undoStack != null && m_undoStack.Count > 0)
            {
                //rollback last action performed and put it into REDO stack
                MyEditorActionBase editorAction = m_undoStack.Pop();
                editorAction.Rollback();
                m_redoList.Push(editorAction);
            }

        }

        /// <summary>
        /// Redo action
        /// </summary>
        public static void Redo()
        {
            if (m_redoList != null && m_redoList.Count > 0)
            {
                MyEditorActionBase editorAction = m_redoList.Pop();
                editorAction.Perform();
                m_undoStack.Push(editorAction);
            }
        }

        /// <summary>
        /// Nothing to undo/redo
        /// </summary>
        /// <returns></returns>
        public static bool IsActionHistoryEmpty()
        {
            if (m_undoStack == null || m_undoStack.Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}