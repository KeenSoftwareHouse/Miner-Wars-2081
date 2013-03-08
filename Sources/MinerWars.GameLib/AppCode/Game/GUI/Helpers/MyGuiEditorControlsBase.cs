using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI.Core.TreeView;
using MinerWars.AppCode.Game.Entities.SubObjects;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    abstract class MyGuiEditorControlsBase
    {
        private static float BUTTON_SCALE = 0.58f;
        private static Vector4 BUTTON_TEXT_COLOR = new Vector4(0.9f, 1, 1, 0.9f);
        private static Vector4 BUTTON_BACKGROUND_COLOR = new Vector4(0.77f, 0.97f, 0.97f, 0.6f);                        

        protected enum EditorToolBarButtonType
        {
            AddObjectButton,
            EditObjectsButton,
            ResetRotationButton,
            CopySelectedButton,
            SwitchGizmoSpaceButton,
            SwitchGizmoModeButton,
            LinkSnapPoints,
            UnlinkSnapPoints,
            ToggleSnapPoints,
            SelectAllObjectsButton,
            ExitSelectedButton,
            EnterPrefabContainerButton,
            AttachToCameraButton,
            EnterVoxelHandButton,
            ClearWholeSectorButton,
            LoadSectorButton,
            SaveSectorButton,
            AdjustGridButton,
            SunSettingsButton,
            GroupsButton,
            CopyToolButton,
            OptionsButton,
            EditProperties,
            SelectAndHideButton,
            ListWayPoints,
            CorrectSnappedPrefabsButton,
            SmallShipTemplates,
            SelectDeactivatedEntityButton,
            FindEntityButton,
        }

        // Add Object Controls
        protected MyGuiControlPanel m_controlPanel;
        protected MyGuiControlTreeView m_addObjectTreeView;
        protected MyTreeViewItemDragAndDrop m_addObjectTreeViewdragDrop;
        protected MyGuiControlTextbox m_filterTextbox;
        protected MyGuiControlCheckbox m_snapPointFilterActive;

        // Toolbar buttons
        MyGuiControlButton m_addObjectButton;
        MyGuiControlButton m_editObjectsButton;
        MyGuiControlButton m_resetRotationButton;
        MyGuiControlButton m_copySelectedButton;
        MyGuiControlButton m_switchGizmoSpaceButton;
        MyGuiControlButton m_switchGizmoModeButton;
        MyGuiControlButton m_linkSnapPoints;
        MyGuiControlButton m_toggleSnapPoints;
        MyGuiControlButton m_selectAllObjectsButton;
        MyGuiControlButton m_exitSelectedButton;
        MyGuiControlButton m_enterPrefabContainerButton;
        MyGuiControlButton m_attachToCameraButton;
        MyGuiControlButton m_enterVoxelHandButton;
        MyGuiControlButton m_clearWholeSectorButton;
        MyGuiControlButton m_loadSectorButton;
        MyGuiControlButton m_saveSectorButton;
        //MyGuiControlButton m_adjustGridButton;
        //MyGuiControlButton m_sunSettingsButton;
        MyGuiControlButton m_groupsButton;
        MyGuiControlButton m_copyToolButton;
        MyGuiControlButton m_optionsButton;
        MyGuiControlButton m_selectAndHideButton;
        MyGuiControlButton m_editPropertiesButton;
        MyGuiControlButton m_selectDeactivatedEntityButton;
        MyGuiControlButton m_findEntityButton;
        MyGuiControlButton m_listWayPointsButton;
        MyGuiControlButton m_correctSnappedPrefabsButton;
        MyGuiControlButton m_smallShipTemplates;

        protected MyGuiControlButton[] m_toolbarButtons;

        protected MyGuiControlEditVoxelHand m_editVoxelHand;

        protected MyGuiControlEditLights m_editLights;

        protected List<MyGuiControlBase> m_editorControls;
        protected MyGuiScreenBase m_parentScreen;
        protected MyPrefabSnapPoint m_activeSnapPointFilter;
        protected string m_activeTextFilter;

        private bool m_loadInDraw;

        private bool m_isDemoUser;
        private bool m_canAccessStoryEditor;

        public MyGuiEditorControlsBase(MyGuiScreenBase parentScreen)
        {
            m_parentScreen = parentScreen;
            m_editorControls = new List<MyGuiControlBase>(10);
            m_toolbarButtons = new MyGuiControlButton[Enum.GetValues(typeof(EditorToolBarButtonType)).Length];

            m_isDemoUser = MyClientServer.LoggedPlayer == null || MyClientServer.LoggedPlayer.IsDemoUser();
            m_canAccessStoryEditor = MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.GetCanAccessEditorForStory();
        }

        private MyGuiControlButton AddToolbarButton(EditorToolBarButtonType buttonType, MyTextsWrapperEnum text, StringBuilder tooltip, MyGuiControlButton.OnButtonClick onButtonClick)
        {
            var screenZero = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(0, 0));
            int index = 0;
            foreach (var item in m_toolbarButtons)
            {
                if (item != null)
                    index++;
            }
            
            Vector2 buttonPosition = screenZero + MyGuiConstants.TOOLBAR_PADDING;
            Vector2 buttonSize = MyGuiConstants.TOOLBAR_BUTTON_SIZE;
            Vector2 buttonOffset = new Vector2(buttonSize.X + MyGuiConstants.TOOLBAR_BUTTON_OFFSET, 0);

            var button = new MyGuiControlButton(
                m_parentScreen,
                buttonPosition + buttonOffset * index,
                buttonSize,
                Vector4.One,
                MyGuiManager.GetToolbarButton(),
                MyGuiManager.GetToolbarButtonHover(),
                MyGuiManager.GetToolbarButtonHover(),
                text,
                MyGuiConstants.TOOLBAR_TEXT_COLOR,
                MyGuiConstants.TOOLBAR_TEXT_SCALE,
                MyGuiControlButtonTextAlignment.CENTERED,
                onButtonClick,
                false,
                MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                true,
                false);
            button.SetToolTip(tooltip);
            button.SetHightlightType(MyGuiControlHighlightType.WHEN_CURSOR_OVER);
            
            if (GetEnabledButtons().Contains(buttonType) && button != null)
            {
                m_editorControls.Add(button);
                m_toolbarButtons[(int) buttonType] = button;
            }

            return button;
        }

        protected abstract EditorToolBarButtonType[] GetEnabledButtons();

        private void CreateButtons()
        {
            // Toolbar buttons init
            m_addObjectButton = AddToolbarButton(EditorToolBarButtonType.AddObjectButton, MyTextsWrapperEnum.ToolbarAddObject, MyTextsWrapper.Get(MyTextsWrapperEnum.AddObject), OnAddObject);
            m_editObjectsButton = AddToolbarButton(EditorToolBarButtonType.EditObjectsButton, MyTextsWrapperEnum.ToolbarEditObjects, new StringBuilder(MyTextsWrapper.Get(MyTextsWrapperEnum.EditObjects) + " \\ " + MyTextsWrapper.Get(MyTextsWrapperEnum.EditVoxelHand)), OnEditObjects);
            m_resetRotationButton = AddToolbarButton(EditorToolBarButtonType.ResetRotationButton, MyTextsWrapperEnum.ToolbarResetRotation, MyTextsWrapper.Get(MyTextsWrapperEnum.ResetRotation), OnResetRotation);
            m_copySelectedButton = AddToolbarButton(EditorToolBarButtonType.CopySelectedButton, MyTextsWrapperEnum.ToolbarCopySelected, MyTextsWrapper.Get(MyTextsWrapperEnum.CopySelected), OnCopySelected);
            m_switchGizmoSpaceButton = AddToolbarButton(EditorToolBarButtonType.SwitchGizmoSpaceButton, MyTextsWrapperEnum.ToolbarSwitchGizmoSpace, MyTextsWrapper.Get(MyTextsWrapperEnum.SwitchGizmoSpace), OnSwitchGizmoSpace);
            m_switchGizmoModeButton = AddToolbarButton(EditorToolBarButtonType.SwitchGizmoModeButton, MyTextsWrapperEnum.ToolbarSwitchGizmoMode, MyTextsWrapper.Get(MyTextsWrapperEnum.SwitchGizmoMode), OnSwitchGizmoMode);
            m_linkSnapPoints = AddToolbarButton(EditorToolBarButtonType.LinkSnapPoints, MyTextsWrapperEnum.ToolbarLinkSnapPoints, new StringBuilder(MyTextsWrapper.Get(MyTextsWrapperEnum.LinkSnapPoints) + " \\ " + MyTextsWrapper.Get(MyTextsWrapperEnum.UnlinkSnapPoints)), OnLinkSnapPoints);
            m_toggleSnapPoints = AddToolbarButton(EditorToolBarButtonType.ToggleSnapPoints, MyTextsWrapperEnum.ToolbarToggleSnapPoints, MyTextsWrapper.Get(MyTextsWrapperEnum.ToggleSnapPoints), OnToggleSnapPoints);
            m_selectAllObjectsButton = AddToolbarButton(EditorToolBarButtonType.SelectAllObjectsButton, MyTextsWrapperEnum.ToolbarSelectAllObjects, MyTextsWrapper.Get(MyTextsWrapperEnum.SelectAllObjects), OnSelectAllObjects);
            m_exitSelectedButton = AddToolbarButton(EditorToolBarButtonType.ExitSelectedButton, MyTextsWrapperEnum.ToolbarExitSelected, MyTextsWrapper.Get(MyTextsWrapperEnum.ExitSelected), OnExitSelected);
            m_enterPrefabContainerButton = AddToolbarButton(EditorToolBarButtonType.EnterPrefabContainerButton, MyTextsWrapperEnum.ToolbarEnterPrefabContainer, new StringBuilder(MyTextsWrapper.Get(MyTextsWrapperEnum.EnterPrefabContainer) + " \\ " + MyTextsWrapper.Get(MyTextsWrapperEnum.ExitEditingMode)), OnEnterPrefabContainer);
            m_attachToCameraButton = AddToolbarButton(EditorToolBarButtonType.AttachToCameraButton, MyTextsWrapperEnum.ToolbarAttachToCamera, MyTextsWrapper.Get(MyTextsWrapperEnum.AttachToCamera), OnAttachToCamera);
            m_enterVoxelHandButton = AddToolbarButton(EditorToolBarButtonType.EnterVoxelHandButton, MyTextsWrapperEnum.ToolbarEnterVoxelHand, new StringBuilder(MyTextsWrapper.Get(MyTextsWrapperEnum.EnterVoxelHand) + " \\ " + MyTextsWrapper.Get(MyTextsWrapperEnum.ExitVoxelHand)), OnEnterVoxelHand);
            m_clearWholeSectorButton = AddToolbarButton(EditorToolBarButtonType.ClearWholeSectorButton, MyTextsWrapperEnum.ToolbarClearWholeSector, MyTextsWrapper.Get(MyTextsWrapperEnum.ClearWholeSector), OnClearWholeSector);
            m_loadSectorButton = AddToolbarButton(EditorToolBarButtonType.LoadSectorButton, MyTextsWrapperEnum.ToolbarLoadSector, MyTextsWrapper.Get(MyTextsWrapperEnum.LoadSector), OnLoadSector);
            m_saveSectorButton = AddToolbarButton(EditorToolBarButtonType.SaveSectorButton, MyTextsWrapperEnum.ToolbarSaveSector, MyTextsWrapper.Get(MyTextsWrapperEnum.SaveSector), OnSaveSector);
            //m_adjustGridButton          = AddToolbarButton(EditorToolBarButtonType.AdjustGridButton, MyTextsWrapper.Get(MyTextsWrapperEnum.AdjustGrid), MyGuiManager.GetToolbarAdjustGridTexture(), OnAdjustGrid);
            //m_sunSettingsButton = AddToolbarButton(EditorToolBarButtonType.SunSettingsButton, MyTextsWrapperEnum.ToolbarSunSettings, MyTextsWrapper.Get(MyTextsWrapperEnum.SunSettings), OnSettings);
            m_groupsButton = AddToolbarButton(EditorToolBarButtonType.GroupsButton, MyTextsWrapperEnum.ToolbarEditorGroups, MyTextsWrapper.Get(MyTextsWrapperEnum.EditorGroups), OnGroups);
            m_copyToolButton = AddToolbarButton(EditorToolBarButtonType.CopyToolButton, MyTextsWrapperEnum.ToolbarCopyTool, MyTextsWrapper.Get(MyTextsWrapperEnum.CopyTool), OnCopyTool);
            m_optionsButton = AddToolbarButton(EditorToolBarButtonType.OptionsButton, MyTextsWrapperEnum.ToolbarEditorOptions, MyTextsWrapper.Get(MyTextsWrapperEnum.EditorOptions), OnOptions);
            m_selectAndHideButton = AddToolbarButton(EditorToolBarButtonType.SelectAndHideButton, MyTextsWrapperEnum.ToolbarEditorSelectAndHide, MyTextsWrapper.Get(MyTextsWrapperEnum.EditorSelectAndHide), OnSelectAndHide);
            m_editPropertiesButton = AddToolbarButton(EditorToolBarButtonType.EditProperties, MyTextsWrapperEnum.ToolbarEditProperties, MyTextsWrapper.Get(MyTextsWrapperEnum.EditProperties), OnEditProperties);
            m_listWayPointsButton = AddToolbarButton(EditorToolBarButtonType.ListWayPoints, MyTextsWrapperEnum.ToolbarWaypoints, MyTextsWrapper.Get(MyTextsWrapperEnum.Waypoints), OnListWaypoints);
            m_correctSnappedPrefabsButton = AddToolbarButton(EditorToolBarButtonType.CorrectSnappedPrefabsButton, MyTextsWrapperEnum.ToolbarCorrectSnappedPrefabs, MyTextsWrapper.Get(MyTextsWrapperEnum.CorrectSnappedPrefabs), OnCorrectSnappedPrefabs);
            m_smallShipTemplates = AddToolbarButton(EditorToolBarButtonType.SmallShipTemplates, MyTextsWrapperEnum.ToolbarSmallShipTemplates, MyTextsWrapper.Get(MyTextsWrapperEnum.SmallShipTemplates), OnSmallShipTemplates);
            m_selectDeactivatedEntityButton = AddToolbarButton(EditorToolBarButtonType.SelectDeactivatedEntityButton, MyTextsWrapperEnum.ToolbarSelectDeactivatedEntity, MyTextsWrapper.Get(MyTextsWrapperEnum.SelectDeactivatedEntity), OnSelectDeactivatedEntity);
            m_findEntityButton = AddToolbarButton(EditorToolBarButtonType.FindEntityButton, MyTextsWrapperEnum.FindEntityByID, MyTextsWrapper.Get(MyTextsWrapperEnum.FindEntityByID), OnFindEntity);

            foreach (MyGuiControlBase control in m_editorControls)
            {
                control.DrawWhilePaused = false;
            }
            UpdateToolbarButtonsSizeAndPosition();
        }

        protected virtual void CreateControls()
        {            
            CreateButtons();                     
            var screenZero = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(0, 0));
            var screenMax = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y));

            var textBoxSize = MyGuiConstants.TEXTBOX_MEDIUM_SIZE + new Vector2(30f / 1600f,0);
            var treeViewSize = new Vector2(textBoxSize.X, screenMax.Y - (MyGuiConstants.TOOLBAR_PADDING.Y * 6 + textBoxSize.Y + MyGuiConstants.TOOLBAR_BUTTON_SIZE.Y));
            var panelSize = treeViewSize + new Vector2(MyGuiConstants.TOOLBAR_PADDING.X * 2, MyGuiConstants.TOOLBAR_PADDING.Y * 3) + new Vector2(0, textBoxSize.Y);

            var textBoxPosition = screenZero + new Vector2(MyGuiConstants.TOOLBAR_PADDING.Y * 2, MyGuiConstants.TOOLBAR_PADDING.Y * 3 + MyGuiConstants.TOOLBAR_BUTTON_SIZE.Y);
            var treeViewPosition = textBoxPosition + new Vector2(0, textBoxSize.Y + MyGuiConstants.TOOLBAR_PADDING.Y);
            var panelPosition = textBoxPosition - new Vector2(MyGuiConstants.TOOLBAR_PADDING.Y, MyGuiConstants.TOOLBAR_PADDING.Y);

            // Add treeview and add button panel
            m_controlPanel = new MyGuiControlPanel(m_parentScreen,
                    panelPosition + panelSize / 2,
                    panelSize,
                    MyGuiConstants.TREEVIEW_BACKGROUND_COLOR,
                    null, null, null, null,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            m_editorControls.Add(m_controlPanel);

            m_addObjectTreeViewdragDrop = new MyTreeViewItemDragAndDrop(m_parentScreen);
            m_addObjectTreeViewdragDrop.Drop = OnDragDrop;

            // Add object treeView
            m_addObjectTreeView = new MyGuiControlTreeView(
                m_parentScreen,
                treeViewPosition + treeViewSize / 2,
                treeViewSize,
                MyGuiConstants.TREEVIEW_BACKGROUND_COLOR, 
                false);
            m_editorControls.Add(m_addObjectTreeView);

            // Add treeView filter textBox
            m_filterTextbox = new MyGuiControlTextbox(m_parentScreen,
                textBoxPosition + textBoxSize / 2,
                MyGuiControlPreDefinedSize.MEDIUM,
                string.Empty, 80,
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL, false);
            m_editorControls.Add(m_filterTextbox);
            MyGuiScreenGamePlay.Static.SetControlIndex(1);

            // Dragdrop Control must be last (draw over other controls)
            m_editorControls.Add(m_addObjectTreeViewdragDrop);

            Vector2 editVoxelHandPanelSize = new Vector2(panelSize.X * 1.4f, panelSize.Y);
            m_editVoxelHand = new MyGuiControlEditVoxelHand(m_parentScreen, panelPosition + editVoxelHandPanelSize / 2f, editVoxelHandPanelSize, MyGuiConstants.TREEVIEW_BACKGROUND_COLOR);
            m_editorControls.Add(m_editVoxelHand);

            Vector2 editLightsSize = new Vector2(0.48f, 0.925f);
            m_editLights = new MyGuiControlEditLights(m_parentScreen, panelPosition + editLightsSize / 2, editLightsSize, MyGuiConstants.TREEVIEW_BACKGROUND_COLOR);
            m_editorControls.Add(m_editLights);
            m_editLights.Visible = false;
            foreach (MyGuiControlBase control in m_editorControls)
            {
                control.DrawWhilePaused = false;
            }
        }

        private void UpdateToolbarButtonsSizeAndPosition()
        {
            int buttonCount = 0;
            for (int index = 0; index < m_toolbarButtons.Length; index++)
            {
                MyGuiControlButton button = m_toolbarButtons[index];
                if (button != null)
                {
                    buttonCount++;
                }
            }
            Vector2 screenCoords = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(new Vector2(MyMinerGame.ScreenSize.X, 0f));

            var screenZero = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(0, 0));
            
            Vector2 buttonPosition = screenZero + MyGuiConstants.TOOLBAR_PADDING;
            float offset = MyGuiConstants.TOOLBAR_BUTTON_OFFSET;
            Vector2 buttonSize = MyGuiConstants.TOOLBAR_BUTTON_SIZE;
            float toolbarLength = buttonPosition.X + buttonCount * (buttonSize.X + offset);

            float scale = 1;
            if (toolbarLength > 1)
            {
                scale = screenCoords.X / toolbarLength;
            }
            //offset *= scale;
            buttonSize *= scale;

            Vector2 buttonOffset = new Vector2(buttonSize.X + offset, 0);
            int buttonIndexOffset = 0;
            for (int index = 0; index < m_toolbarButtons.Length; index++)
            {                 
                MyGuiControlButton button = m_toolbarButtons[index];
                if (button != null)
                {
                    button.SetPosition(buttonPosition + buttonOffset * buttonIndexOffset + buttonSize / 2);
                    button.SetSize(buttonSize);
                    button.SetTextScale(MyGuiConstants.TOOLBAR_TEXT_SCALE * scale);
                    buttonIndexOffset++;
                }
            }
        }

        protected abstract void FillTreeView();

        #region TreeView helpers methods
        protected void AddPrefabType(MyTreeViewItem parentItem, MyMwcObjectBuilder_Prefab_AppearanceEnum appearanceTextureEnum, MyTextsWrapperEnum typeText, BuildTypesEnum buildTypesEnum, 
            CategoryTypesEnum[] categories, MyTexture2D icon, Vector2 iconSize, MyTexture2D expand, MyTexture2D collapse, 
            Vector2 expandIconSize)
        {
            var prefabTypeItem = parentItem == null ?
                m_addObjectTreeView.AddItem(MyTextsWrapper.Get(typeText), icon, iconSize, expand, collapse, expandIconSize) :
                parentItem.AddItem(MyTextsWrapper.Get(typeText), icon, iconSize, expand, collapse, expandIconSize);

            foreach (var categoryTypesEnum in categories)
            {
                var categoryItem = prefabTypeItem.AddItem(MyGuiPrefabHelpers.GetMyGuiCategoryTypeHelper(categoryTypesEnum).Description, icon, 
                    iconSize, expand, collapse, expandIconSize);
                AddPrefabItems(categoryItem, appearanceTextureEnum, buildTypesEnum, categoryTypesEnum);
            }
        }

        protected abstract void AddPrefabItems(MyTreeViewItem parentItem, MyMwcObjectBuilder_Prefab_AppearanceEnum appearanceTextureEnum, BuildTypesEnum buildType, CategoryTypesEnum categoryType);                   

        protected void TryExpand(StringBuilder[] nodePath)
        {
            MyTreeViewItem item = null;
            for (int i = 0; i < nodePath.Length; i++)
            {
                if (item == null)
                {
                    item = m_addObjectTreeView.GetItem(nodePath[i]);
                }
                else
                {
                    item = item.GetItem(nodePath[i]);
                }

                if (item != null && item.GetItemCount() > 0)
                {
                    item.IsExpanded = true;
                }
            }
        }
        #endregion

        public void LoadData()
        {
        }

        public virtual void LoadContent()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiEditorControlsBase::LoadContent");

            CreateControls();
            SetAddObjectVisibility(true);
            
            m_loadInDraw = true;

            if (MyFakes.MWBUILDER)
            {
                OnAddObject(m_addObjectButton);
                OnEnterVoxelHand(m_enterVoxelHandButton);

                foreach (var editorControl in m_editorControls)
                {
                    //editorControl.Visible = false;
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public virtual void UnloadContent()
        {
            for (int i = 0; i < m_toolbarButtons.Length; i++)
            {
                m_toolbarButtons[i] = null;
            }
            if (m_editorControls != null) m_editorControls.Clear();
        }

        public virtual void Draw()
        {
            if (m_loadInDraw)
            {
                FillTreeView();
                m_loadInDraw = false;
            }
        }

        private bool CheckContextAction(MyGuiContextMenuHelper contextHelper, MyGuiContextMenuItemActionType action)
        {
            return contextHelper != null && contextHelper.IsActionTypeAvailable(action);
        }

        private bool CheckContextAction(MyGuiContextMenuItemActionType action)
        {
            var editorState = MyEditor.GetCurrentState();
            var contextHelper = MyGuiContextMenuHelpers.GetEditorContextMenuHelper(editorState);
            return contextHelper != null && contextHelper.IsActionTypeAvailable(action);
        }

        public void RefreshTreeView()
        {
            FilterAddObjectTree(MyEditorGizmo.SelectedSnapPoint);
        }

        public virtual void Update()
        {
            FilterAddObjectTree(MyEditorGizmo.SelectedSnapPoint);

            // Get editor state and allowed context action
            var notDragging = !m_addObjectTreeViewdragDrop.Dragging;
            var editorState = MyEditor.GetCurrentState();
            var contextHelper = MyGuiContextMenuHelpers.GetEditorContextMenuHelper(editorState);

            // Choose editor state dependent textures
            var cameraText = editorState == MyEditorStateEnum.ATTACHED ? MyTextsWrapperEnum.ToolbarDetachFromCamera : MyTextsWrapperEnum.ToolbarAttachToCamera;
            var enterPrefabContainerText = CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EXIT_EDITING_MODE) ? MyTextsWrapperEnum.ToolbarExitEditingMode : MyTextsWrapperEnum.ToolbarEnterPrefabContainer;
            var enterVoxelHandText = editorState == MyEditorStateEnum.VOXEL_HAND_ENABLED ? MyTextsWrapperEnum.ToolbarExitVoxelHand : MyTextsWrapperEnum.ToolbarEnterVoxelHand;
            var showSnapPointsText = MyEditor.Static.ShowSnapPoints ? MyTextsWrapperEnum.ToolbarHideSnapPoints : MyTextsWrapperEnum.ToolbarShowSnapPoints;
            var linkSnapPointsText = MyEditor.Static.CanUnlinkSnapPoints(MyEditorGizmo.SelectedSnapPoint) ? MyTextsWrapperEnum.ToolbarUnlinkSnapPoints : MyTextsWrapperEnum.ToolbarLinkSnapPoints;

            // Change look of editor state dependent buttons
            m_attachToCameraButton.SetTextEnum(cameraText);
            m_enterPrefabContainerButton.SetTextEnum(enterPrefabContainerText);
            m_enterVoxelHandButton.SetTextEnum(enterVoxelHandText);
            m_linkSnapPoints.SetTextEnum(linkSnapPointsText);

            // Enable/Disable buttons (context)
            m_addObjectButton.Enabled = notDragging &&
               (CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.ADD_OBJECT) ||
                CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EXIT_VOXEL_HAND));
            //m_editObjectsButton.Enabled = notDragging &&
            //   ((CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EDIT_SELECTED) && MyEditor.Static.CanEditSelected()) ||
            //    CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EDIT_VOXEL_HAND));
            m_editObjectsButton.Enabled = notDragging &&
               (CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EDIT_SELECTED) && MyEditor.Static.CanEditSelected());
            m_resetRotationButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.RESET_ROTATION);
            m_copySelectedButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.COPY_SELECTED);
            m_switchGizmoSpaceButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.SWITCH_GIZMO_SPACE);
            m_switchGizmoModeButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.SWITCH_GIZMO_MODE);
            m_linkSnapPoints.Enabled = notDragging && MyEditorGizmo.SelectedSnapPoint != null;
            m_toggleSnapPoints.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.TOGGLE_SNAP_POINTS);
            m_selectAllObjectsButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.SELECT_ALL_OBJECTS);
            m_exitSelectedButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EXIT_SELECTED);
            m_enterPrefabContainerButton.Enabled = notDragging &&
               (CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.ENTER_PREFAB_CONTAINER) ||
                CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EXIT_EDITING_MODE));
            m_attachToCameraButton.Enabled = notDragging &&
               (CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.ATTACH_TO_CAMERA) ||
                CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.DETACH_FROM_CAMERA));
            m_enterVoxelHandButton.Enabled = notDragging &&
               (CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND) ||
                CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EXIT_VOXEL_HAND));
            m_clearWholeSectorButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.CLEAR_WHOLE_SECTOR);
            m_loadSectorButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.LOAD_SECTOR) && !m_isDemoUser;
            m_saveSectorButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.SAVE_SECTOR) && MySession.CanBeSaved(false, MyGuiScreenGamePlay.Static.GetSectorIdentifier(), true);
            MyTextsWrapperEnum? isDemoS = null;
            if (MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.IsDemoUser()) isDemoS = MyTextsWrapperEnum.NotAvailableInDemoMode;
            m_saveSectorButton.AccessForbiddenReason = isDemoS;
            m_loadSectorButton.AccessForbiddenReason = isDemoS;
            //m_adjustGridButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.ADJUST_GRID);
            //m_sunSettingsButton.Enabled = false; // atm. disable, SUN_SETTINGS + FOG_SETTINGS
            m_groupsButton.Enabled = notDragging;
            m_copyToolButton.Enabled = notDragging;
            m_optionsButton.Enabled = notDragging;
            m_selectAndHideButton.Enabled = notDragging;
            m_editPropertiesButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EDIT_PROPERTIES) && 
                MyGuiScreenEditorEditProperties.CanEditProperties(MyEditorGizmo.SelectedEntities);
            m_correctSnappedPrefabsButton.Enabled = notDragging && CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.CORRECT_SNAPPED_PREFABS);
            m_selectDeactivatedEntityButton.Enabled = notDragging;
            m_findEntityButton.Enabled = notDragging;

            if (!CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.ADD_OBJECT) && m_addObjectTreeView.Visible)
            {
                SetAddObjectVisibility(false);
            }
            SetEditVoxelHandVisibility(CheckContextAction(contextHelper, MyGuiContextMenuItemActionType.EDIT_VOXEL_HAND));
            if (m_editVoxelHand.Visible && MyEditorGizmo.IsAnyEntitySelected())
            {
                if (!MyFakes.MWBUILDER)
                {
                    if (!(MyEditorVoxelHand.SelectedVoxelMap != null && MyEditorGizmo.SelectedEntities.Count == 1 && MyEditorGizmo.SelectedEntities[0] == MyEditorVoxelHand.SelectedVoxelMap))
                    {
                        MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.EXIT_SELECTED);

                        if (MyEditorVoxelHand.SelectedVoxelMap != null)
                        {
                            MyEditorGizmo.AddEntityToSelection(MyEditorVoxelHand.SelectedVoxelMap);
                        }
                    }
                }
            }
        }

        protected virtual void SetAddObjectVisibility(bool visible)
        {
            m_addObjectTreeView.Visible = visible;
            m_controlPanel.Visible = visible;
            m_filterTextbox.Visible = visible;

            if (visible && m_editLights.Visible)
            {
                m_editLights.RestoreOriginalValues();
                m_editLights.Visible = false;
            }
        }

        private void SetEditVoxelHandVisibility(bool visible)
        {           
            m_editVoxelHand.Visible = visible;
        }        

        //public void AddEditorControlsToList(ref List<MyGuiControlBase> addToControlsList)
        public void AddEditorControlsToList(MyGuiControls addToControlsList)
        {
            if (addToControlsList != null)
            {
                foreach (MyGuiControlBase control in m_editorControls)
                {
                    addToControlsList.Add(control);
                }
            }
        }

        //public void RemoveEditorControlsFromList(ref List<MyGuiControlBase> removeFromControlsList)
        public void RemoveEditorControlsFromList(MyGuiControls removeFromControlsList)
        {
            if (removeFromControlsList != null)
            {
                foreach (MyGuiControlBase control in m_editorControls)
                {
                    removeFromControlsList.Remove(control);
                }
            }
        }                

        #region Toolbar button Handlers
        public void OnAddObject(MyGuiControlButton sender)
        {
            if (CheckContextAction(MyGuiContextMenuItemActionType.EXIT_VOXEL_HAND))
            {
                MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.EXIT_VOXEL_HAND);
            }
            SetAddObjectVisibility(!m_addObjectTreeView.Visible);
        }

        private void OnEditObjects(MyGuiControlButton sender)
        {            
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.EDIT_SELECTED);
            //MyEditor.Static.PerformContextMenuAction(CheckContextAction(MyGuiContextMenuItemActionType.EDIT_VOXEL_HAND) ?
            //    MyGuiContextMenuItemActionType.EDIT_VOXEL_HAND : MyGuiContextMenuItemActionType.EDIT_SELECTED);
        }

        private void OnResetRotation(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.RESET_ROTATION);
        }

        private void OnCopySelected(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.COPY_SELECTED);
        }

        private void OnSwitchGizmoSpace(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.SWITCH_GIZMO_SPACE);
        }

        private void OnSwitchGizmoMode(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.SWITCH_GIZMO_MODE);
        }

        private void OnShowSnapPoints(MyGuiControlButton sender)
        {
            MyEditor.Static.ShowSnapPoints = !MyEditor.Static.ShowSnapPoints;
        }

        private void OnLinkSnapPoints(MyGuiControlButton sender)
        {
            if (MyEditorGizmo.SelectedSnapPoint != null)
	        {
                if (MyEditor.Static.CanUnlinkSnapPoints(MyEditorGizmo.SelectedSnapPoint))
	            {
                    MyEditor.Static.UnlinkSnapPoints();
	            }
                else
                {
                    MyEditor.Static.LinkSnapPoints();
                }
	        }
        }

        private void OnToggleSnapPoints(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.TOGGLE_SNAP_POINTS);
        }

        private void OnSelectAllObjects(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.SELECT_ALL_OBJECTS);
        }

        private void OnExitSelected(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.EXIT_SELECTED);
        }

        private void OnEnterPrefabContainer(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(CanExitSelectionMode() ?
                MyGuiContextMenuItemActionType.EXIT_EDITING_MODE : MyGuiContextMenuItemActionType.ENTER_PREFAB_CONTAINER);
        }

        private void OnAttachToCamera(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyEditor.GetCurrentState() == MyEditorStateEnum.ATTACHED ?
                MyGuiContextMenuItemActionType.DETACH_FROM_CAMERA : MyGuiContextMenuItemActionType.ATTACH_TO_CAMERA);
        }

        private void OnEnterVoxelHand(MyGuiControlButton sender)
        {            
            MyEditor.Static.PerformContextMenuAction(MyEditor.GetCurrentState() == MyEditorStateEnum.VOXEL_HAND_ENABLED ?
                MyGuiContextMenuItemActionType.EXIT_VOXEL_HAND : MyGuiContextMenuItemActionType.ENTER_VOXEL_HAND);
        }

        private void OnClearWholeSector(MyGuiControlButton sender)
        {

            if (MyEntities.GetEntities().Count > 1)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.EditorClearWholeSector, MyTextsWrapperEnum.ClearWholeSector, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No,
                    mbReturn =>
                    {
                        if (mbReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
                        {
                            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.CLEAR_WHOLE_SECTOR);
                        }
                    }));
            }
            else
                MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.CLEAR_WHOLE_SECTOR);
        }

        private void OnLoadSector(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.EditorLoadSectorMessage, MyTextsWrapperEnum.LoadSector, MyTextsWrapperEnum.Yes, MyTextsWrapperEnum.No,
                mbReturn =>
                {
                    if(mbReturn == MyGuiScreenMessageBoxCallbackEnum.YES)
                    {
                        MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.LOAD_SECTOR); 
                    }
                }));
        }

        private void OnSaveSector(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenSaveSector());
        }

        private void OnAdjustGrid(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.ADJUST_GRID);
        }

        private void OnSettings(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.SUN_SETTINGS);
        }

        private void OnGroups(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorGroups());
        }

        private void OnCopyTool(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorCopyToolSelectSector());
        }

        private void OnFindEntity(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenInputString(EntityIDInput, MyTextsWrapperEnum.Find));
        }

        void EntityIDInput(ScreenResult result, string resultText)
        {
            if (result == ScreenResult.Ok)
            {
                uint entityID;
                if (uint.TryParse(resultText, out entityID))
                {
                    var entity = MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier(entityID));
                    if (entity != null)
                    {
                        var position = entity.GetPosition() - entity.WorldVolume.Radius * entity.GetForward();
                        MySpectator.SetViewMatrix(Matrix.CreateLookAt(position, entity.GetPosition(), entity.GetUp()));

                        MyEditorGizmo.AddEntityToSelection(entity);
                    }
                    else
                    {
                        MyGuiScreenMessageBox.Show(MyTextsWrapperEnum.EntityIsNotExist, type: MyMessageBoxType.ERROR);
                    }
                }
                else
                {
                    MyGuiScreenMessageBox.Show(MyTextsWrapperEnum.WrongNumberFormat, type:MyMessageBoxType.ERROR);
                }
            }
        }

        private void OnOptions(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorOptions());
        }

        private void OnSelectAndHide(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorSelectAndHide());
        }

        private void OnCorrectSnappedPrefabs(MyGuiControlButton sender)
        {
            MyEditor.Static.PerformContextMenuAction(MyGuiContextMenuItemActionType.CORRECT_SNAPPED_PREFABS);
        }

        private void OnSmallShipTemplates(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenSmallShipTemplates());
        }

        private void OnEditProperties(MyGuiControlButton sender)
        {
            if (MyEditorGizmo.SelectedEntities.Count > 0) 
            {
                MyGuiManager.AddScreen(new MyGuiScreenEditorEditProperties(MyEditorGizmo.SelectedEntities));
            }
            else if (MyEditor.Static.GetEditedPrefabContainer() != null) 
            {
                MyGuiManager.AddScreen(new MyGuiScreenEditorEditProperties(MyEditor.Static.GetEditedPrefabContainer()));
            }            
        }

        private void OnSelectDeactivatedEntity(MyGuiControlButton sender) 
        {
            MyGuiManager.AddScreen(new MyGuiScreenSelectDeactivatedEntity());
        }

        private void OnListWaypoints(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorWayPointList());
        }

        #endregion

        protected abstract void OnDragDrop(object sender, EventArgs ea);        

        protected abstract void FilterAddObjectTree(MyPrefabSnapPoint myPrefabSnapPoint);

        private bool CanExitSelectionMode()
        {
            return CheckContextAction(MyGuiContextMenuHelpers.GetEditorContextMenuHelper(MyEditor.GetCurrentState()), MyGuiContextMenuItemActionType.EXIT_EDITING_MODE);
        }

        public bool TryExitSelected()
        {
            if (m_exitSelectedButton.Enabled)
            {
                OnExitSelected(null);
                return true;
            }
            return false;
        }

        public bool TryExitSelectionMode()
        {
            if (CanExitSelectionMode())
            {
                OnEnterPrefabContainer(null);
                return true;
            }
            return false;
        }

        private void MirrorSelectedObjects()
        {
            if (MyEditorGizmo.SelectedEntities.Count == 0)
            {
                return;
            }

            Vector3 center = Vector3.Zero;
            foreach (var entity in MyEditorGizmo.SelectedEntities)
            {
                center += entity.GetPosition();
            }
            center = center / MyEditorGizmo.SelectedEntities.Count;

            foreach (var entity in MyEditorGizmo.SelectedEntities)
            {
                var deltaPosition = entity.GetPosition() - center;
                entity.WorldMatrix *= Matrix.CreateTranslation(-center) *
                    Matrix.CreateScale(-1, 1, 1) *
                    Matrix.CreateTranslation(center);
            }
        }

        public virtual void UpdateScreenSize()
        {
            var screenZero = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(0, 0));
            var screenMax = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y));

            var textBoxSize = MyGuiConstants.TEXTBOX_MEDIUM_SIZE;
            var treeViewSize = new Vector2(textBoxSize.X, screenMax.Y - (MyGuiConstants.TOOLBAR_PADDING.Y * 6 + textBoxSize.Y + MyGuiConstants.TOOLBAR_BUTTON_SIZE.Y));
            var panelSize = treeViewSize + new Vector2(MyGuiConstants.TOOLBAR_PADDING.X * 2, MyGuiConstants.TOOLBAR_PADDING.Y * 3) + new Vector2(0, textBoxSize.Y);

            var textBoxPosition = screenZero + new Vector2(MyGuiConstants.TOOLBAR_PADDING.Y * 2, MyGuiConstants.TOOLBAR_PADDING.Y * 3 + MyGuiConstants.TOOLBAR_BUTTON_SIZE.Y);
            var treeViewPosition = textBoxPosition + new Vector2(0, textBoxSize.Y + MyGuiConstants.TOOLBAR_PADDING.Y);
            var panelPosition = textBoxPosition - new Vector2(MyGuiConstants.TOOLBAR_PADDING.Y, MyGuiConstants.TOOLBAR_PADDING.Y);

            if (m_controlPanel == null)
            {
                CreateControls();
            }
            
            m_controlPanel.SetPosition(panelPosition + panelSize / 2);
            m_controlPanel.SetSize(panelSize);

            m_addObjectTreeView.SetPosition(treeViewPosition + treeViewSize / 2);
            m_addObjectTreeView.SetSize(treeViewSize);

            m_filterTextbox.SetPosition(textBoxPosition + textBoxSize / 2);

            Vector2 editVoxelHandPanelSize = new Vector2(panelSize.X * 1.4f, panelSize.Y);
            m_editVoxelHand.SetPosition(panelPosition + editVoxelHandPanelSize / 2);

            UpdateToolbarButtonsSizeAndPosition();
        }

        public void Focused(bool focused)
        {
            if (!focused)
            {
                var treeViewIndex = m_parentScreen.Controls.IndexOf(m_addObjectTreeView);
                m_parentScreen.SetControlIndex(treeViewIndex);
            }
        }

        public virtual void LoadEditorData()
        {
        }

        public virtual void UnloadEditorData()
        {
        }

        public void EditLights(List<MyPrefabLight> lights)
        {
            SetAddObjectVisibility(false);            

            m_editLights.SetLights(lights);
            m_editLights.Visible = true;
        }
    }
}
