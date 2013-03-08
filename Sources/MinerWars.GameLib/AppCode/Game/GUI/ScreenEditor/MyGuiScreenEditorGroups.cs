using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Entities.SubObjects;
using System.Text.RegularExpressions;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.GUI.Core.TreeView;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    /// <summary>
    /// Edit screen for named prefab groups 
    /// Notes:
    /// - move MyObjectGroup to separate file
    /// </summary>
    class MyGuiScreenEditorGroups : MyGuiScreenEditorDialogBase
    {
        private static readonly Vector2 BUTTON_SIZE = MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE + new Vector2(0.04f,0);
        private static readonly Vector2 BUTTON_OFFSET = new Vector2(0, BUTTON_SIZE.Y);
        private static readonly Vector2 EXTRA_BUTTON_OFFSET = new Vector2(0, 0.025f);

        // Control for list of current available groups
        MyGuiControlTreeView m_groupList;
        
        // Buttons
        MyGuiControlButton m_addObjectsButton;
        MyGuiControlButton m_removeObjectsButton;

        MyGuiControlButton m_selectGroupButton;
        MyGuiControlButton m_unselectGroupButton;

        MyGuiControlButton m_createGroupButton;
        MyGuiControlButton m_renameGroupButton;
        MyGuiControlButton m_deleteGroupButton;

        MyGuiControlButton m_loadGroupButton;

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorGroups";
        }

        public MyGuiScreenEditorGroups()
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.6f, 0.9f))
        {
            AddCaption(MyTextsWrapperEnum.EditorGroups, MyGuiConstants.LABEL_TEXT_COLOR, new Vector2(0,0.007f));

            Vector2 groupListTopLeft = new Vector2(MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X*0.7f, CAPTION_OFFSET_Y) - m_size.Value / 2;
            Vector2 groupListSize = m_size.Value - new Vector2(
                MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X * 3 + BUTTON_SIZE.X*0.5f,
                CAPTION_OFFSET_Y + MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y * 2 + MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y);

            m_groupList = new MyGuiControlTreeView(this, groupListTopLeft + groupListSize / 2, groupListSize, MyGuiConstants.TREEVIEW_BACKGROUND_COLOR, true);
            m_groupList.WholeRowHighlight = true;

            Controls.Add(m_groupList);

            int index = 0;
            m_addObjectsButton = AddButton(index++, 0, MyTextsWrapperEnum.EditorGroupsAddObjects, OnAddObjects);
            m_removeObjectsButton = AddButton(index++, 0, MyTextsWrapperEnum.EditorGroupsRemoveObjects, OnRemoveObjects);

            m_selectGroupButton = AddButton(index++, 1, MyTextsWrapperEnum.EditorGroupsSelectGroup, OnSelectGroup);
            m_unselectGroupButton = AddButton(index++, 1, MyTextsWrapperEnum.EditorGroupsUnselectGroup, OnUnselectGroup);

            m_createGroupButton = AddButton(index++, 2, MyTextsWrapperEnum.EditorGroupsCreateGroup, OnCreateGroup);
            m_renameGroupButton = AddButton(index++, 2, MyTextsWrapperEnum.EditorGroupsRenameGroup, OnRenameGroup);
            m_deleteGroupButton = AddButton(index++, 2, MyTextsWrapperEnum.EditorGroupsDeleteGroup, OnDeleteGroup);

            m_loadGroupButton = AddButton(index, 3, MyTextsWrapperEnum.EditorGroupsLoadGroup, OnLoadGroup, true);
            
            AddBackButtonControl(new Vector2(0,-0.03f));

            foreach (var group in MyEditor.Static.ObjectGroups)
            {
                AddGroup(group);
            }
        }

        private MyGuiControlButton AddButton(int index, int extraOffset, MyTextsWrapperEnum text, MyGuiControlButton.OnButtonClick onButtonClick, bool implemented = true)
        {
            Vector2 origin = new Vector2(m_size.Value.X / 2 - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X - BUTTON_SIZE.X*0.8f, CAPTION_OFFSET_Y - m_size.Value.Y / 2);
            Vector2 buttonPosition = origin + BUTTON_SIZE / 2 + BUTTON_OFFSET * index + EXTRA_BUTTON_OFFSET * extraOffset;

            var button = new MyGuiControlButton(this, buttonPosition, BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                text, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, onButtonClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, implemented);

            Controls.Add(button);

            return button;
        }

        private MyObjectGroup GetFocusedGroup()
        {
            var focusedItem = m_groupList.GetFocusedItem();
            return focusedItem != null ? focusedItem.Tag as MyObjectGroup : null;
        }

        /// <summary>
        /// Generates new unique group name i.e. "Group 03"
        /// </summary>
        private string GenerateName(string baseName)
        {
            int topNameIndex = -1;
            for (int i = 0; i < m_groupList.GetItemCount(); i++)
            {
                var item = m_groupList.GetItem(i);

                string itemName = item.Text.ToString();
                if (itemName.StartsWith(baseName))
                {
                    int nameIndex;
                    if (int.TryParse(itemName.Substring(baseName.Length), out nameIndex))
                    {
                        topNameIndex = Math.Max(topNameIndex, nameIndex);
                    }
                }
            }

            ++topNameIndex;

            return string.Format("{0} {1:00}", baseName, topNameIndex);
        }

        private MyTreeViewItem AddGroup(MyObjectGroup group)
        {
            var item = m_groupList.AddItem(group.Name, null, Vector2.Zero, null, null, Vector2.Zero);
            item.Tag = group;
            return item;
        }

        /// <summary>
        /// Try change group name, returns false on error
        /// </summary>
        public bool ChangeGroupName(MyObjectGroup group, string newName)
        {
            MyTreeViewItem groupItem = null;
            for (int i = 0; i < m_groupList.GetItemCount(); i++)
            {
                var item = m_groupList.GetItem(i);
                
                // Find group list item
                if (item.Tag == group)
                {
                    groupItem = item;
                }
                else
                {
                    if (item.Text.ToString() == newName)
                    {
                        return false;
                    }
                }
            }

            if (groupItem != null)
            {
                group.Name = new StringBuilder(newName);
                groupItem.Text = group.Name;
                return true;
            }

            return false;
        }

        public override bool Update(bool hasFocus)
        {
            bool result = base.Update(hasFocus);
            if (result)
            {
                bool groupSelected = m_groupList.GetFocusedItem() != null;

                m_addObjectsButton.Enabled = groupSelected;
                m_removeObjectsButton.Enabled = groupSelected;
                m_selectGroupButton.Enabled = groupSelected;
                m_unselectGroupButton.Enabled = groupSelected;
                m_renameGroupButton.Enabled = groupSelected;
                m_deleteGroupButton.Enabled = groupSelected;
            }
            return result;
        }

        #region Button Handlers
        private void OnAddObjects(MyGuiControlButton sender)
        {
            var group = GetFocusedGroup();
            if(group != null)
            {
                if (!group.AddObjects(MyEditorGizmo.SelectedEntities))
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, new StringBuilder("Can't add objects. Objects must be in same container."), new StringBuilder("Add Objects Error"), MyTextsWrapperEnum.Ok, null));
                }
            }
        }

        private void OnRemoveObjects(MyGuiControlButton sender)
        {
            var group = GetFocusedGroup();
            if (group != null)
            {
                group.RemoveObjects(MyEditorGizmo.SelectedEntities);
            }
        }

        private void OnSelectGroup(MyGuiControlButton sender)
        {
            var group = GetFocusedGroup();
            if (group != null && group.GetCount() > 0)
            {
                var container = group.GetContainer();
                if (MyEditor.Static.IsEditingPrefabContainer() && MyEditor.Static.GetEditedPrefabContainer() == container)
                {
                    MyEditorGizmo.AddEntitiesToSelection(group.GetEntities());
                }
                else
                {
                    var editorState = MyEditor.GetCurrentState();
                    var contextHelper = MyGuiContextMenuHelpers.GetEditorContextMenuHelper(editorState);

                    // If exit/enter to prefab container available
                    if (container != null &&
                        (editorState == MyEditorStateEnum.NOTHING_SELECTED ||
                        contextHelper.IsActionTypeAvailable(MyGuiContextMenuItemActionType.ENTER_PREFAB_CONTAINER) ||
                        contextHelper.IsActionTypeAvailable(MyGuiContextMenuItemActionType.EXIT_EDITING_MODE)))
                    {
                        // Switch to group container
                        if (MyEditor.Static.IsEditingPrefabContainer())
                        {
                            MyEditor.Static.ExitActivePrefabContainer();
                        }
                        MyEditor.Static.EditPrefabContainer(container);

                        if (MyEditor.Static.IsEditingPrefabContainer() && MyEditor.Static.GetEditedPrefabContainer() == container)
                        {
                            MyEditorGizmo.AddEntitiesToSelection(group.GetEntities());
                        }
                    }
                    else if (container == null)
                    {
                        if (MyEditor.Static.IsEditingPrefabContainer())
                        {
                            MyEditor.Static.ExitActivePrefabContainer();
                        }
                        MyEditorGizmo.ClearSelection();
                        MyEditorGizmo.AddEntitiesToSelection(group.GetEntities());
                    }
                    else
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, new StringBuilder("Can't select group."), new StringBuilder("Select Group Error"), MyTextsWrapperEnum.Ok, null));
                    }
                }
            }
        }

        private void OnUnselectGroup(MyGuiControlButton sender)
        {
            var group = GetFocusedGroup();
            if (group != null && group.GetCount() > 0)
            {
                var container = group.GetContainer();
                if (MyEditor.Static.IsEditingPrefabContainer() && MyEditor.Static.GetEditedPrefabContainer() == container)
                {
                    MyEditorGizmo.RemoveEntitiesFromSelection(group.GetEntities());
                }
            }
        }

        private void OnCreateGroup(MyGuiControlButton sender)
        {
            var generatedName = new StringBuilder(GenerateName("Group"));
            var group = new MyObjectGroup(generatedName);
            if (group.AddObjects(MyEditorGizmo.SelectedEntities))
            {
                AddGroup(group);
                MyEditor.Static.ObjectGroups.Add(group);
            }
            else
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.EditorGroupsCantCreateGroupText, MyTextsWrapperEnum.EditorGroupsCantCreateGroupCaption, MyTextsWrapperEnum.Ok, null));
            }
        }

        private void OnRenameGroup(MyGuiControlButton sender)
        {
            var group = GetFocusedGroup();
            if (group != null)
            {
                MyGuiManager.AddScreen(new MyGuiScreenEditorRenameGroup(this, group));
            }
        }

        private void OnDeleteGroup(MyGuiControlButton sender)
        {
            var item = m_groupList.GetFocusedItem();
            var group = GetFocusedGroup();
            if (group != null)
            {
                m_groupList.DeleteItem(item);
                MyEditor.Static.ObjectGroups.Remove(group);
            }
        }

        private void OnLoadGroup(MyGuiControlButton sender)
        {
            CloseScreen();
            MyGuiManager.AddScreen(new MyGuiScreenEditorSelectSector());
        }
        #endregion
    }

    class MyObjectGroup
    {
        /// <summary>
        /// Group name
        /// </summary>
        public StringBuilder Name;

        /// <summary>
        /// List of objects in this group
        /// </summary>
        private List<MyEntity> m_objectList = new List<MyEntity>();

        /// <summary>
        /// Object builder from which this group should be created
        /// </summary>
        private MyMwcObjectBuilder_ObjectGroup m_objectBuilder;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyObjectGroup(StringBuilder name)
        {
            Name = name;
            m_objectList = new List<MyEntity>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyObjectGroup(MyMwcObjectBuilder_ObjectGroup objectBuilder)
        {
            Name = new StringBuilder(objectBuilder.Name);
            m_objectList = new List<MyEntity>();
            m_objectBuilder = objectBuilder;
        }

        /// <summary>
        /// Get count of objects in this group
        /// </summary>
        public int GetCount()
        {
            return m_objectList.Count;
        }

        /// <summary>
        /// Return container of this group, null if unassigned
        /// </summary>
        public MyPrefabContainer GetContainer()
        {
            if (m_objectList.Count > 0)
            {
                //Debug.Assert(m_objectList[0].Parent is MyPrefabContainer);
                return m_objectList[0].Parent as MyPrefabContainer;
            }
            return null;
        }

        /// <summary>
        /// Add objects to group
        /// </summary>
        public bool AddObjects(List<MyEntity> entities)
        {
            //var container = GetContainer();

            // Only Prefabs
            // Only Same Container
            // We dont care anymore
            //if (!entities.TrueForAll(entity => entity is MyPrefabBase &&
            //        (container == null || entity.Parent == container)))
            //{
            //    return false;
            //}

            foreach (var entity in entities)
            {
                if (!m_objectList.Contains(entity))
                {
                    m_objectList.Add(entity);
                }
            }

            return true;
        }

        /// <summary>
        /// Remove objects from group
        /// </summary>
        public void RemoveObjects(List<MyEntity> entities)
        {
            foreach (var entity in entities)
            {
                RemoveObject(entity);
            }
        }

        /// <summary>
        /// Remove object from group
        /// </summary>
        public void RemoveObject(MyEntity entity)
        {
            m_objectList.Remove(entity);
        }

        /// <summary>
        /// Return entity list (for selection in editor)
        /// </summary>
        public List<MyEntity> GetEntities()
        {
            return m_objectList;
        }

        internal MyMwcObjectBuilder_ObjectGroup GetObjectBuilder()
        {
            var result = new MyMwcObjectBuilder_ObjectGroup(new List<uint>(), Name.ToString());            
            foreach (var entity in m_objectList)
            {
                Debug.Assert(entity.EntityId.HasValue);
                var x = MyEntities.GetEntityById(new MyEntityIdentifier(entity.EntityId.Value.NumericValue));
                var y = entity == x;
                if (entity.EntityId.HasValue)
                {
                    result.ObjectList.Add(entity.EntityId.Value.NumericValue);
                }
            }

            return result;
        }

        internal void LinkEntities()
        {
            Debug.Assert(m_objectList != null);
            Debug.Assert(m_objectList.Count == 0);
            Debug.Assert(m_objectBuilder != null);

            foreach (var entityId in m_objectBuilder.ObjectList)
            {
                MyEntity entity;
                if (MyEntities.TryGetEntityById(new MyEntityIdentifier(entityId), out entity))
                {
                    m_objectList.Add(entity);
                }
            }
            m_objectBuilder = null;
        }
    }
}
