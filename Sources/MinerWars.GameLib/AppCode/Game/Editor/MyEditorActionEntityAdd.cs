using MinerWars.AppCode.Game.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Add entity editor action
    /// </summary>
    class MyEditorActionEntityAdd : MyEditorActionWithObjectBuildersBase
    {

        public MyEditorActionEntityAdd(MyMwcObjectBuilder_Base actionObjectBuilder, Matrix matrix, Vector2? screenPosition)
            : base(actionObjectBuilder, matrix)
        {
            ActionObjectBuilders.ForEach(a => a.ScreenPosition = screenPosition);
        }

        public override bool Perform()
        {
            AddOrCreateInBackgroundThread();            
            return true;
        }

        public override bool Rollback()
        {
            RemoveInBackgroundThread();            
            return true;
        }

        protected override MyEntity CreateEntity(string hudLabelText, MyMwcObjectBuilder_Base objectBuilder, Matrix matrix, Vector2? screenPosition)
        {
            MyEntity entity = base.CreateEntity(hudLabelText, objectBuilder, matrix, screenPosition);
            
            // add waypoints
            if (entity is MyPrefabBase)
                (entity as MyPrefabBase).InitWaypoints();
            else if (entity is MyPrefabContainer)
                foreach (var prefab in (entity as MyPrefabContainer).GetPrefabs())
                    (prefab as MyPrefabBase).InitWaypoints();

            float distanceFromCamera = entity.WorldVolume.Radius * (entity is MyPrefabBase ? 4 : 2);

            Vector3 newPosition;
            if (screenPosition.HasValue)
            {
                Ray ray = MyUtils.ConvertMouseToRay(MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(screenPosition.Value));
                newPosition = ray.Position + ray.Direction * distanceFromCamera;
            }
            else
            {
                newPosition = MySpectator.Position + distanceFromCamera * MySpectator.Orientation;
            }

            entity.MoveAndRotate(newPosition, entity.GetWorldRotation());
            
            return entity;
        }

        protected override void AddToScene(MyEntity entity)
        {
            base.AddToScene(entity);

            // in case we are adding new prefab container, enter directly into edit mode of this new container
            // but we dont want enter into edit mode when containers are duplicated
            if (entity is MyPrefabContainer)
            {
                MyEditor.Static.EditPrefabContainer((MyPrefabContainer)entity);
                foreach (MyPrefabBase child in (entity as MyPrefabContainer).Children)
                    MyEditorGizmo.AddEntityToSelection(child);

            }
        }
    }
}