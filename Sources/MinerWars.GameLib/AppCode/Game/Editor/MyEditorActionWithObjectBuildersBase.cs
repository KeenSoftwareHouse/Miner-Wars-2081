using System;
using System.Collections.Generic;
using System.Linq;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.GUI;
using System.Threading;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Audio;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Editor
{
    using App;
    using Managers;
    using Managers.EntityManager;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.Entities.WayPoints;
    using MinerWars.AppCode.Game.Entities.InfluenceSpheres;

    /// <summary>
    /// Base class for all actions, that create/remove some/all entities
    /// </summary>
    abstract class MyEditorActionWithObjectBuildersBase : MyEditorActionBase
    {
        #region Fields

        protected class ObjectBuilderCreate
        {
            public Matrix Matrix;
            public MyMwcObjectBuilder_Base ObjectBuilder;
            public Vector2? ScreenPosition { get; set; }

            public ObjectBuilderCreate(MyEntity entity, bool getExactCopy = false)
            {
                Matrix = entity.WorldMatrix;
                ObjectBuilder = entity.GetObjectBuilder(getExactCopy);
            }

            public ObjectBuilderCreate(MyMwcObjectBuilder_Base objectBuilder, Matrix matrix)
            {
                Matrix = matrix;
                ObjectBuilder = objectBuilder;
            }
        }

        protected List<ObjectBuilderCreate> ActionObjectBuilders; // Holds object builders actual for entities at the moment of creation of this action

        #endregion

        #region Methods
        public MyEditorActionWithObjectBuildersBase()
        {
        }

        protected void Init(int objectBuildersCount)
        {
            if(ActionObjectBuilders == null) ActionObjectBuilders = new List<ObjectBuilderCreate>(objectBuildersCount);
        }

        public MyEditorActionWithObjectBuildersBase(MyEntity actionEntity, bool getExactCopy = false)
            : base(actionEntity)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyEditorActionWithObjectBuildersBase::ctor");
            this.Init(1);
            ActionObjectBuilders.Add(new ObjectBuilderCreate(actionEntity, getExactCopy));
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public MyEditorActionWithObjectBuildersBase(List<MyEntity> actionEntities, bool getExactCopies = false)
            : base(actionEntities)
        {
            this.Init(actionEntities.Count);
            foreach (MyEntity actionEntity in actionEntities)
            {
                ActionObjectBuilders.Add(new ObjectBuilderCreate(actionEntity, getExactCopies));
            }
        }

        public MyEditorActionWithObjectBuildersBase(MyMwcObjectBuilder_Base actionObjectBuilder, Matrix matrix)
            : base((MyEntity)null)
        {
            this.Init(1);
            ActionObjectBuilders.Add(new ObjectBuilderCreate(actionObjectBuilder, matrix));
        }

        /// <summary>
        /// Adds object builder to current action object builders list
        /// </summary>
        /// <param name="actionObjectBuilder"></param>
        public void AddEntityObjectBuilder(MyMwcObjectBuilder_Base actionObjectBuilder, Matrix matrix)
        {
            ActionObjectBuilders.Add(new ObjectBuilderCreate(actionObjectBuilder, matrix));
        }

        /// <summary>
        /// Add or create entities(in separate thread if needed - voxelmaps)
        /// </summary>
        internal void AddOrCreateInBackgroundThread()
        {
            // Perform in separate thread only for voxel maps
            if (ActionObjectBuilders.Find(a => a.ObjectBuilder is MyMwcObjectBuilder_VoxelMap) != null)
            {
                Thread thread = new Thread(new ThreadStart(this.AddOrCreate));
                thread.Name = "AddOrCreate";
                MyEditor.Static.StartBackgroundThread(thread);
                MyGuiManager.AddScreen(new MyGuiScreenEditorProgress(MyTextsWrapperEnum.LoadingPleaseWait, false));
            }
            else
            {
                AddOrCreate();
            }
        }

        /// <summary>
        /// Removes entities(in separate thread if needed - voxelmaps)
        /// </summary>
        internal void RemoveInBackgroundThread()
        {
            // Remove in separate thread only for voxel maps
            if (ActionEntities.OfType<MyVoxelMap>().Any())
            {
                Thread thread = new Thread(new ThreadStart(this.RemoveFromScene));
                thread.Name = "RemoveFromScene";
                MyEditor.Static.StartBackgroundThread(thread);
                MyGuiManager.AddScreen(new MyGuiScreenEditorProgress(MyTextsWrapperEnum.LoadingPleaseWait, false));
            }
            else
            {
                RemoveFromScene();
            }
            
        }

        /// <summary>
        /// RemoveAndCreateInBackgroundThread
        /// </summary>
        internal void RemoveAndCreateInBackgroundThread()
        {
            Thread thread = new Thread(new ThreadStart(this.RemoveAndCreate));
            thread.Name = "RemoveAndCreate";
            MyEditor.Static.StartBackgroundThread(thread);
            MyGuiManager.AddScreen(new MyGuiScreenEditorProgress(MyTextsWrapperEnum.LoadingPleaseWait, false));
        }

        /// <summary>
        /// RemoveAndCreate
        /// </summary>
        void RemoveAndCreate()
        {
            RemoveFromScene();
            AddOrCreate();
        }

        /// <summary>
        /// Remove all entities, possibly in a separate thread
        /// </summary>
        internal void RemoveAllInBackgroundThread()
        {
            //MyGuiManager.AddScreen(new MyGuiScreenEditorProgress(MyTextsWrapperEnum.LoadingPleaseWait, false));
            //MyEditor.Static.StartBackgroundThread(new Thread(new ThreadStart(this.RemoveAllEntities)));
            
            // Just do it in the foreground.
            RemoveAllEntities();
        }

        /// <summary>
        /// Create entities from action object builders
        /// </summary>
        void AddOrCreate()
        {
            MyEditorGizmo.ClearSelection();

            // Once entities has been created in this action, they remain and if needed, are only removed/added to scene
            if (ActionEntities != null && ActionEntities.Count > 0)
            {
                foreach (MyEntity actionEntity in ActionEntities)
                {
                    AddToScene(actionEntity);
                }
            }
            else
            {
                // If no ActionEntites are present, create them from provided object builders
                foreach (ObjectBuilderCreate crate in ActionObjectBuilders)
                {                    
                    CreateFromObjectBuilder(crate.ObjectBuilder, crate.Matrix, crate.ScreenPosition);
                }
            }
            
            // Link the new entities and clean up waypoint vertices
            foreach (var e in ActionEntities)
                e.Link();
            MyWayPointGraph.DeleteNullVerticesFromPaths();

            // When copying a single waypoint, connect it to its source
            if (this is MyEditorActionEntityCopy && ActionEntities.Count == 1 && ActionEntities[0] is MyWayPoint)
            {
                var source = (this as MyEditorActionEntityCopy).SourceEntities[0] as MyWayPoint;
                if (source != null)
                    MyWayPoint.Connect(source, ActionEntities[0] as MyWayPoint);
            }

            // When copying prefabs, connect snap points connections
            if (this is MyEditorActionEntityCopy)
            {
                MyEditor.Static.CopySnapPointLinks((this as MyEditorActionEntityCopy).SourceEntities, (this as MyEditorActionEntityCopy).RemapContext);
            }

            MyEditor.Static.IssueCheckAllCollidingObjects();
        }

        void CreateFromObjectBuilder(MyMwcObjectBuilder_Base objectBuilder, Matrix matrix, Vector2? screenPosition)
        {
            MyEntity actionEntity = CreateEntity(null, objectBuilder, matrix, screenPosition);
            if (actionEntity != null)
            {
                AddEntity(actionEntity);
                AddToScene(actionEntity);
            }
        }

        /// <summary>
        /// Adds entity to scene
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void AddToScene(MyEntity entity)
        {
            //add entity into world if possible to ensure we manipulate with object in scene
            if (!(entity is MyPrefabBase))
            {
                // Quit container edit mode if entity was created in non-edit mode
                if (m_activeContainer == null && MyEditor.Static.IsEditingPrefabContainer()) 
                    MyEditor.Static.ExitActivePrefabContainer();

                MyEntities.Add(entity);
            }
            else
            {
                // In case undo/redo is performed on prefab, make sure that its parent container is switched to edit mode
                MyPrefabBase prefab = (MyPrefabBase)entity;
                MyEditor.Static.EditPrefabContainer(m_activeContainer);
                MyEditor.Static.GetEditedPrefabContainer().AddPrefab(prefab);
            }

            // adding an entity will make its type selectable
            Type type = null;
            if (entity is MyPrefabBase || entity is MyPrefabContainer) type = typeof(MyPrefabBase);
            else if (entity is MyWayPoint) type = typeof(MyWayPoint);
            else if (entity is MyVoxelMap) type = typeof(MyVoxelMap);
            else if (entity is MyDummyPoint) type = typeof(MyDummyPoint);
            else if (entity is MySpawnPoint) type = typeof(MySpawnPoint);
            else if (entity is MyInfluenceSphere) type = typeof(MyInfluenceSphere);
            if (type != null)
            {
                MyEntities.SetTypeHidden(type, false);
                MyEntities.SetTypeSelectable(type, true);
            }
            
            MyEditorGizmo.AddEntityToSelection(entity);
        }

        /// <summary>
        /// Remove all action entities
        /// </summary>
        protected virtual void RemoveFromScene()
        {
            foreach (MyEntity actionEntity in ActionEntities)
            {
                //dont delete player ship
                if (actionEntity == MySession.PlayerShip)
                {
                    continue;
                }

                MyEditorGizmo.RemoveEntityFromSelection(actionEntity);

                if (actionEntity == MyEditor.Static.GetEditedPrefabContainer())
                {
                    MyEditor.Static.ResetActivePrefabContainer();
                }

                MyEditor.Static.DeleteEntityFromCollidingList(actionEntity);

                if (!(actionEntity is MyPrefabBase))
                {
                    if (m_activeContainer == null && MyEditor.Static.IsEditingPrefabContainer())
                        MyEditor.Static.ExitActivePrefabContainer();
                }
                else
                {
                    MyEditor.Static.EditPrefabContainer(m_activeContainer);
                }

                actionEntity.MarkForClose();
                
                MyEditor.Static.RecheckAllColidingEntitesAndClearNonColiding();
            }

            ActionEntities.Clear();

            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectDelete);
        }

        /// <summary>
        /// Creates new entity based on its object builder
        /// </summary>
        /// <param name="hudLabelText"></param>
        /// <param name="objectBuilder"></param>
        /// <returns></returns>
        protected virtual MyEntity CreateEntity(string hudLabelText, MyMwcObjectBuilder_Base objectBuilder, Matrix matrix, Vector2? screenPosition)
        {
            MyEntity entity = null;
            if (objectBuilder is MyMwcObjectBuilder_PrefabBase)
            {
                //When creating prefabs, it will never happen that active container does not exist
                entity = m_activeContainer.CreateAndAddPrefab(hudLabelText, objectBuilder as MyMwcObjectBuilder_PrefabBase);
            }
            else
            {
                entity = MyEntities.CreateFromObjectBuilder(hudLabelText, objectBuilder, matrix);
            }
            
            return entity;
        }

        /// <summary>
        /// Removes all entities - TODO - this will have to be unified in MyEntities.cs
        /// </summary>
        void RemoveAllEntities()
        {
            //MyEditor.Static.DeleteAllFromCollidingList();            
            MyEditor.Static.CollidingElements.Clear();
            MyEntities.CollisionsElements.Clear();
            MyEditorGizmo.ClearSelection();
            MyEntities.CloseAll(false);
            MyVoxelMaps.RemoveAll();
        }

        #endregion
    }
}
