using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Prefabs;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Entity transformation editor action
    /// </summary>
    class MyEditorActionEntityTransform : MyEditorActionBase
    {
        private List<MyEditorTransformData> m_startTransformData;
        private List<MyEditorTransformData> m_endTransformData;
        private bool m_containsPrefab;

        public MyEditorActionEntityTransform()
            : base()
        {
            m_startTransformData = new List<MyEditorTransformData>();
            m_endTransformData = new List<MyEditorTransformData>();
            m_containsPrefab = false;
        }

        /// <summary>
        /// Transform entities to latest transformation state
        /// </summary>
        /// <returns></returns>
        public override bool Perform()
        {
            if (m_containsPrefab) MyEditor.Static.EditPrefabContainer(m_activeContainer);
            foreach (MyEditorTransformData transformData in m_endTransformData)
            {
                MyEditorGizmo.MoveAndRotateObject(transformData.GetPosition(), transformData.GetOrientation(), transformData.GetEntity());
            }
            MyEditor.Static.CheckAllCollidingObjects();
            return true;
        }

        /// <summary>
        /// Transform entities to starting transformation state
        /// </summary>
        /// <returns></returns>
        public override bool Rollback()
        {
            if (m_containsPrefab) MyEditor.Static.EditPrefabContainer(m_activeContainer);
            foreach (MyEditorTransformData transformData in m_startTransformData)
            {
                MyEditorGizmo.MoveAndRotateObject(transformData.GetPosition(), transformData.GetOrientation(), transformData.GetEntity());
            }
            MyEditor.Static.CheckAllCollidingObjects();
            return true;
        }

        public void AddStartData(MyEditorTransformData startData)
        {
            if (startData.GetEntity() is MyPrefabBase) m_containsPrefab = true;
            m_startTransformData.Add(startData);
        }

        public void AddEndData(MyEditorTransformData endData)
        {
            m_endTransformData.Add(endData);
        }
    }
}