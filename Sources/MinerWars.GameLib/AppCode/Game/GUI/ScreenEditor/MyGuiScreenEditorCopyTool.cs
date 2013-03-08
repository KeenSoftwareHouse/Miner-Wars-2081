using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Prefabs;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorCopyTool : MyGuiScreenEditorDialogBase
    {
        private MyGuiControlListbox m_objectList;
        private MyMwcObjectBuilder_Sector m_sectorBuilder;

        public MyGuiScreenEditorCopyTool(MyMwcObjectBuilder_Sector sectorBuilder)
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.8f, 0.88f))
        {
            m_size = new Vector2(0.815f, 0.95f);
            m_sectorBuilder = sectorBuilder;

            //Vector2 controlsDelta = new Vector2(0, 0.0525f);
            //Vector2 controlsColumn1Origin = new Vector2(-m_size.Value.X / 2.0f + 0.03f, -m_size.Value.Y / 2.0f + 0.08f);

            AddCaption(MyTextsWrapperEnum.CopyTool, MyGuiConstants.LABEL_TEXT_COLOR, new Vector2(0, 0.01f));

            m_objectList = new MyGuiControlListbox(this, 
                new Vector2(0f, -0.02f),
                MyGuiConstants.LISTBOX_LONGMEDIUM_SIZE, 
                MyGuiConstants.DEFAULT_CONTROL_BACKGROUND_COLOR, 
                null, .6f, 1, 17, 1, false, true, false,
                null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 2, 1, MyGuiConstants.LISTBOX_BACKGROUND_COLOR_BLUE, 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);

            m_objectList.MultipleSelection = true;

            Controls.Add(m_objectList);

            //AddOkAndCancelButtonControls();
            Vector2 buttonPosition = new Vector2(0, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y - 0.02f);
            Vector2 buttonSize = new Vector2(0.25f, MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y);
            
            Controls.Add(new MyGuiControlButton(this,
                new Vector2(MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X - 0.05f - m_size.Value.X / 2, 0) + buttonPosition + buttonSize / 2, 
                buttonSize, 
                MyGuiConstants.BUTTON_BACKGROUND_COLOR, MyTextsWrapperEnum.SelectAllNone, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnSelectAllNone, MyGuiControlButtonTextAlignment.CENTERED, true, 
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            Controls.Add(new MyGuiControlButton(this,
                new Vector2(m_size.Value.X / 2 - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.X - buttonSize.X + 0.01f, 0) + buttonPosition + buttonSize / 2,
                buttonSize,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR, MyTextsWrapperEnum.AddSelectedToSector, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnAddSelected, MyGuiControlButtonTextAlignment.CENTERED, true,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            Controls.Add(new MyGuiControlButton(this,
                new Vector2(m_size.Value.X / 2 - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_X - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.X +0.05f, 0) + buttonPosition + MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE / 2,
                MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR, MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancel, MyGuiControlButtonTextAlignment.CENTERED, true,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            FillControls();
        }

        void OnSelectAllNone(MyGuiControlButton sender)
        {
            if (m_objectList.GetSelectedItems().Count > 0)
            {
                m_objectList.DeselectAll();
            }
            else
            {
                m_objectList.SelectAll();
            }
        }

        void OnAddSelected(MyGuiControlButton sender)
        {
            CloseScreen();

            // Holds ids for remap
            IMyEntityIdRemapContext remapContext = new MyEntityIdRemapContext();

            var newEntities = new List<MyEntity>();

            foreach (var item in m_objectList.GetSelectedItems())
            {
                var builder = m_sectorBuilder.SectorObjects[item.Key];
                builder.RemapEntityIds(remapContext);

                //builder.EntityId = null;                                        // reset entityId because it's new object
                //builder.Name = null;
                
                //var container = builder as MyMwcObjectBuilder_PrefabContainer;  // reset entityIds also on prefabs in container
                //if (container != null)
                //{
                //    foreach (var prefabBuilder in container.Prefabs)
                //    {
                //        prefabBuilder.EntityId = null;
                //        prefabBuilder.Name = null;
                //    }
                //}

                var entity = CreateFromObjectBuilder(builder);
                if (entity != null)
                {
                    newEntities.Add(entity);
                }
            }

            foreach (var e in newEntities)
            {
                e.Link();
            }

            // recreate snap point links
            var snapPointLinks = m_sectorBuilder.SnapPointLinks;
            foreach (var item in snapPointLinks)
            {
                List<MySnapPointLink> group = new List<MySnapPointLink>();

                foreach (var link in item.Links)
                {
                    MyEntity entity;
                    if (!MyEntities.TryGetEntityById(new MyEntityIdentifier(remapContext.RemapEntityId(link.EntityId).Value), out entity))
                        continue;
                    var prefab = entity as MyPrefabBase;
                    if (prefab == null)
                        continue;

                    if (string.IsNullOrEmpty(link.SnapPointName))
                    {
                        if (prefab != null && link.Index < prefab.SnapPoints.Count)
                            group.Add(new MySnapPointLink(prefab.SnapPoints[link.Index]));
                    }
                    else
                    {
                        foreach (var snapPoint in prefab.SnapPoints)
                            if (snapPoint.Name == link.SnapPointName)
                                group.Add(new MySnapPointLink(snapPoint));
                    }
                }

                if (group.Count >= 2)
                    MyEditor.Static.AddLinkedSnapPoints(group);
            }

            MyWayPointGraph.DeleteNullVerticesFromPaths();
        }

        private MyEntity CreateFromObjectBuilder(MyMwcObjectBuilder_Base objectBuilder)
        {
            if (objectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.VoxelMap)
            {
                MyMwcObjectBuilder_VoxelMap voxelMapObjectBuilder = objectBuilder as MyMwcObjectBuilder_VoxelMap;
                return MyEntities.CreateFromObjectBuilderAndAdd(null, voxelMapObjectBuilder, Matrix.Identity);
            }
            else if (objectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour)
            {
                //  Voxel map neighbours are handled in its static classe, so ignore it here
            }
            else
            {
                if (objectBuilder is MyMwcObjectBuilder_Object3dBase || objectBuilder is MyMwcObjectBuilder_InfluenceSphere)
                {
                    MyEntity temporaryEntity = null;
                    Matrix matrix = Matrix.Identity;

                    if (objectBuilder is MyMwcObjectBuilder_Object3dBase)
                    {
                        var object3d = objectBuilder as MyMwcObjectBuilder_Object3dBase;

                        matrix = Matrix.CreateWorld(object3d.PositionAndOrientation.Position, object3d.PositionAndOrientation.Forward, object3d.PositionAndOrientation.Up);
                        MyUtils.AssertIsValid(matrix);
                    }

                    temporaryEntity = MyEntities.CreateFromObjectBuilderAndAdd(null, objectBuilder, matrix);

                    MyEntities.TestEntityAfterInsertionForCollision(temporaryEntity);
                    return temporaryEntity;
                }
            }
            return null;
        }

        void OnCancel(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        private void FillControls()
        {
            string commonPrefix = "MyMwcObjectBuilder_";
            m_objectList.RemoveAllItems();
            if (m_sectorBuilder != null)
            {
                int index = 0;
                foreach (var objectBuilder in m_sectorBuilder.SectorObjects)
                {
                    string builderTypeName = objectBuilder.GetType().Name.ToString();
                    string text = string.Format("{0}, {1}, {2}",
                        string.IsNullOrEmpty(objectBuilder.Name) ? "no name" : objectBuilder.Name,
                        objectBuilder.EntityId,
                        builderTypeName.StartsWith(commonPrefix) ? builderTypeName.Substring(commonPrefix.Length) : builderTypeName);

                    m_objectList.AddItem(index++, new StringBuilder(text));
                }
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorCopyTool";
        }
    }
}
