using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.Weapons;
using SysUtils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;

namespace MinerWars.AppCode.Game.Editor
{

    /// <summary>
    /// MyMouseInputTool
    /// </summary>
    class MyMouseInputTool
    {
        private List<MyRBElement> m_physOverlapElemList = new List<MyRBElement>();
        private Vector2 m_lastPos;
        private bool m_bHandleInput = true;

        MyEntity m_mouseOverEntity = null;

        bool m_bEnableAABBUnderMouse = false;

        /// <summary>
        /// EnableInputHandling
        /// </summary>
        public void EnableInputHandling(bool bEnable)
        {
            m_bHandleInput = bEnable;
            if (m_bHandleInput == false)
            {
                HighlightEntity(m_mouseOverEntity, false);
            }
        }


        public MyEntity GetMouseOverEntity() { return m_mouseOverEntity; }

        /// <summary>
        /// EnableAABBUnderMouse
        /// </summary>
        /// <param name="bEnable"></param>
        public void EnableAABBUnderMouse(bool bEnable)
        {
            m_bEnableAABBUnderMouse = bEnable;
        }

        public void HandleInput(MyGuiInput input)
        {
            if (!m_bHandleInput)
                return;

            Vector2 currPos = MyGuiManager.MouseCursorPosition;// new MyPoint(input.GetMouseX(), input.GetMouseY());

            if (currPos == m_lastPos && MyConfig.EditorUseCameraCrosshair == false)
                return;

            m_lastPos = currPos;

            MyEntity entity = GetIntersectedEntity();
            if (entity == null && m_mouseOverEntity != null)
            {
                //@ TurnOff highlightning
                HighlightEntity(m_mouseOverEntity, false);
                return;
            }

            entity = MySelectionTool.GetSelectableEntity(entity);   //this needs to be tweaked

            //@ entity is selected do not highlight!
            foreach (MyEntity sel in MyEditorGizmo.SelectedEntities)
            {
                if (sel == entity)
                    return;
            }

            //@ filter - if editor is in mode EditPrefabContainer (so it's not allowed to select other entity outside container)
            //@ i do not allow highlight entity outside of prefab container
            //@ - when editor ingamemode - dont allow to select/mouseover anything except my own container or entities in it
            MyPrefabContainer container = MyEditor.Static.GetEditedPrefabContainer();
            if (container != null && entity != null)
            {
                if (container.IsEntityFromContainer(entity) == false)
                {
                    BoundingSphere sphere = entity.WorldVolume;
                    if (container.GetIntersectionWithMaximumBoundingBox(ref sphere) == false)
                        return;
                }
            }

            if (entity == null)
            {
                //@ TurnOff highlightning
                HighlightEntity(m_mouseOverEntity, false);
                return;
            }

            if (entity != m_mouseOverEntity)
            {
                if (m_mouseOverEntity != null)
                    HighlightEntity(m_mouseOverEntity, false);
                HighlightEntity(entity, true);
            }
        }

        /// <summary>
        /// GetIntersectedEntity
        /// </summary>
        /// <returns></returns>
        private MyEntity GetIntersectedEntity()
        {
            MyLine mouseSelectionLine = MyUtils.ConvertMouseToLine();
            var result = MyEntities.GetIntersectionWithLine(ref mouseSelectionLine, null, null);
            MyEntity entity = result.HasValue ? result.Value.Entity : null;

            if (entity is MyLargeShipGunBase) entity = ((MyLargeShipGunBase)entity).PrefabParent;

            if (m_bEnableAABBUnderMouse)
            {
                m_physOverlapElemList.Clear();
                MyEntities.GetIntersectedElements(ref mouseSelectionLine, m_physOverlapElemList);
            }

            return entity;
        }


        /// <summary>
        /// HighlightEntity - just clear internal handle on entity - coz of every update particle building
        /// </summary>
        private void HighlightEntity(MyEntity entity, bool bEnabled)
        {
            if (bEnabled)
                m_mouseOverEntity = entity;
            else
            {
                if (m_mouseOverEntity != null)
                    m_mouseOverEntity.ClearHighlightning();
                m_mouseOverEntity = null;
            }
        }


        /// <summary>
        /// RenderHighlitedEntity - has to be called from render?
        /// </summary>
        public void DrawMouseOver()
        {
            if (m_bEnableAABBUnderMouse && m_physOverlapElemList.Count != 0)
            {
                //@ render of Ales rbElements
                /*foreach (MyRBElement elem in m_physOverlapElemList)
                {
                    BoundingBox elemBox = elem.GetWorldSpaceAABB();

                    //@ Draw AABB
                    Vector4 vctColorAABB = new Vector4(0.1f, 0.8f, 0.8f, 0.5f);
                    Matrix mtWorldAABB = Matrix.Identity;
                    MyDebugDraw.DrawWireFramedBox(ref mtWorldAABB, ref elemBox, ref vctColorAABB, 0.01f, 1);
                }*/
            }
            if (MyEditorGizmo.IsAnyAxisSelected()) HighlightEntity(m_mouseOverEntity, false);

            if (m_mouseOverEntity == null)
                return;

            //@ mozny hack pro optimalizaci viditelnosti don't use IsVisible, nedoresena koncepce viditelnosti render/engine/game
            /*BoundingSphere bSphere = m_highlitedEntity.WorldVolumeHr;
            if (!MyCamera.IsInFrustum(ref bSphere))
                return;*/

            m_mouseOverEntity.DrawMouseOver(ref MyEditorConstants.MOUSE_OVER_HIGHLIGHT_COLOR);
        }
    }
}
