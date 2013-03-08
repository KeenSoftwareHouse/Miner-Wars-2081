using MinerWars.AppCode.Game.Entities;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Prefabs;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Base class for all editor undoable/redoable actions
    /// </summary>
    abstract class MyEditorActionBase
    {
        public List<MyEntity> ActionEntities;
        protected MyPrefabContainer m_activeContainer;

        public MyEditorActionBase()
        {
            Init(1);
        }

        private void Init(int entitiesCount)
        {
            m_activeContainer = MyEditor.Static.GetEditedPrefabContainer();
            if (ActionEntities == null) ActionEntities = new List<MyEntity>(entitiesCount);
        }

        public MyEditorActionBase(MyEntity entity)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEditorActionBase::ctor");
            Init(1);
            AddEntity(entity);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public MyEditorActionBase(List<MyEntity> entities)
        {
            Init(entities.Count);
            AddEntities(entities);
        }

        /// <summary>
        /// Performs the action
        /// </summary>
        public abstract bool Perform();

        /// <summary>
        /// Rollbacks the action
        /// </summary>
        public abstract bool Rollback();

        /// <summary>
        /// Registers action in action history and performs it
        /// </summary>
        internal void RegisterAndDoAction()
        {
            MyEditorActions.AddAction(this);
        }

        /// <summary>
        /// Adds entity to action entities list
        /// </summary>
        public void AddEntity(MyEntity entity)
        {
            if (entity != null)
            {
                ActionEntities.Add(entity);
            }
        }

        /// <summary>
        /// Adss entities to action entities list
        /// </summary>
        public void AddEntities(IEnumerable<MyEntity> entities)
        {
            foreach (MyEntity entity in entities)
            {
                AddEntity(entity);
            }
        }

        protected virtual MyPrefabBase GetAnyPrefabInvolved()
        {
            if (ActionEntities.Count > 0)
            {
                foreach (MyEntity entity in ActionEntities)
                {
                    if (entity is MyPrefabBase) return (MyPrefabBase)entity;
                }
            }
            return null;
        }
    }
}