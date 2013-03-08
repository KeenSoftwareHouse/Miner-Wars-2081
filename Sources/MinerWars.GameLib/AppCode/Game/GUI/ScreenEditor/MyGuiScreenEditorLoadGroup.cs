using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Editor;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorLoadGroup : MyGuiScreenEditorDialogBase
    {
        MyGuiControlTreeView m_groupList;

        List<MyMwcObjectBuilder_ObjectGroup> m_groups;
        List<MyMwcObjectBuilder_Base> m_entities;

        public MyGuiScreenEditorLoadGroup(List<MyMwcObjectBuilder_ObjectGroup> groups, List<MyMwcObjectBuilder_Base> entities)
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.33f, 0.85f))
        {
            m_groups = groups;
            m_entities = entities;
            m_size = new Vector2(0.48f, 0.85f);

            AddCaption(new StringBuilder("Load Group"), MyGuiConstants.LABEL_TEXT_COLOR, new Vector2(0, 0.005f));

            Vector2 groupListTopLeft = new Vector2(MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X - 0.025f, 0.1f) - m_size.Value / 2;
            Vector2 groupListSize = m_size.Value - new Vector2(
                MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X * 2 - 0.05f,
                0.1f + MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y * 2 + MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y);

            m_groupList = new MyGuiControlTreeView(this, groupListTopLeft + groupListSize / 2, groupListSize, MyGuiConstants.TREEVIEW_BACKGROUND_COLOR, true);
            m_groupList.WholeRowHighlight = true;

            Controls.Add(m_groupList);

            AddOkAndCancelButtonControls(new Vector2(0, -0.01f));

            foreach (var group in groups)
            {
                var listItem = m_groupList.AddItem(new StringBuilder(group.Name), null, Vector2.Zero, null, null, Vector2.Zero);
                listItem.Tag = group;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorLoadGroup";
        }

        public MyGuiScreenEditorLoadGroup()
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.33f, 0.85f))
        {
        }

        protected override void OnOkClick(MyGuiControlButton sender)
        {
            CloseScreen();
            var focused = m_groupList.GetFocusedItem();

            if (focused != null)
            {
                var group = focused.Tag as MyMwcObjectBuilder_ObjectGroup;

                IEnumerable<MyMwcObjectBuilder_PrefabBase> prefabs = group.GetPrefabBuilders(m_entities);
                IEnumerable<MyMwcObjectBuilder_Base> rootObjects = group.GetRootBuilders(m_entities);
                var objects3d = rootObjects.OfType<MyMwcObjectBuilder_Object3dBase>();

                if (objects3d.Any())
                {
                    MyEditorGizmo.ClearSelection();

                    var basePos = MyCamera.Position + Matrix.Invert(MyCamera.ViewMatrix).Forward * 100;
                    var firstPos = objects3d.First().PositionAndOrientation.Position;
                    var offset = basePos - firstPos;

                    foreach (var b in objects3d)
                    {
                        b.PositionAndOrientation.Position += offset;
                        b.ClearEntityId();
                        MyEntity entity = MyEntities.CreateFromObjectBuilderAndAdd(null, b, b.PositionAndOrientation.GetMatrix());
                        MyEditorGizmo.AddEntityToSelection(entity);
                    }
                }

                // Prefabs
                var editedContainer = MyEditor.Static.GetEditedPrefabContainer();
                if (editedContainer == null)
                {
                    MyMwcObjectBuilder_PrefabContainer prefabContainerBuilder = new MyMwcObjectBuilder_PrefabContainer(
                        null, MyMwcObjectBuilder_PrefabContainer_TypesEnum.INSTANCE, new List<MyMwcObjectBuilder_PrefabBase>(), MyClientServer.LoggedPlayer.GetUserId(),
                        MyMwcObjectBuilder_FactionEnum.Euroamerican, new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), MyInventory.DEFAULT_MAX_ITEMS));

                    editedContainer = MyEntities.CreateFromObjectBuilderAndAdd(
                        null, prefabContainerBuilder,
                        Matrix.CreateTranslation(MyCamera.Position + Matrix.Invert(MyCamera.ViewMatrix).Forward * 100)) as MyPrefabContainer;

                    MyEditor.Static.EditPrefabContainer(editedContainer);
                }

                if (editedContainer != null)
                {
                    MyEditorGizmo.ClearSelection();
                    List<MyEntity> addedEntities = new List<MyEntity>();

                    foreach (var prefabBuilder in prefabs)
                    {
                        prefabBuilder.EntityId = null;
                        var entity = editedContainer.CreateAndAddPrefab(null, prefabBuilder);
                        addedEntities.Add(entity);
                    }

                    // Select newly added objects
                    foreach (var entity in addedEntities)
                    {
                        MyEditorGizmo.AddEntityToSelection(entity);
                    }
                }
            }
        }

        protected override void OnCancelClick(MyGuiControlButton sender)
        {
            base.OnCancelClick(sender);
            MyGuiManager.AddScreen(new MyGuiScreenEditorGroups());
        }
    }
}
