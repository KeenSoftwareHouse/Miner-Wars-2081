using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.GUI;    //TODO TEST JK

namespace MinerWars.AppCode.Game.Editor
{
    partial class MyEditor
    {
        private bool m_active = false;  //flag if user is in editable mode (in game/editor)

        /// <summary>
        /// EditorDebugDraw
        /// </summary>
        public void EditorDebugDraw()
        {
            if (!MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive())
                return;

            DrawSelectionRectangle();
            DrawSelectedBounding();

            //@ JK just for debugging selection
            //DrawDebugSelection();
        }


        // This method takes care of drawing rectangle, that is drawn by dragging mouse to select multiple objects inside rectangle
        /// <summary>
        /// DrawSelectionRectangle
        /// </summary>
        private void DrawSelectionRectangle()
        {
            if (!m_selectionRectangle.IsEmpty)
            {
                MyDebugDraw.DrawLine2D(new Vector2(m_selectionRectangle.Left, m_selectionRectangle.Top), new Vector2(m_selectionRectangle.Right, m_selectionRectangle.Top), Color.Green, Color.Green);
                MyDebugDraw.DrawLine2D(new Vector2(m_selectionRectangle.Right, m_selectionRectangle.Top), new Vector2(m_selectionRectangle.Right, m_selectionRectangle.Bottom), Color.Green, Color.Green);
                MyDebugDraw.DrawLine2D(new Vector2(m_selectionRectangle.Right, m_selectionRectangle.Bottom), new Vector2(m_selectionRectangle.Left, m_selectionRectangle.Bottom), Color.Green, Color.Green);
                MyDebugDraw.DrawLine2D(new Vector2(m_selectionRectangle.Left, m_selectionRectangle.Bottom), new Vector2(m_selectionRectangle.Left, m_selectionRectangle.Top), Color.Green, Color.Green);
            }
        }


        /// <summary>
        /// DrawSelectedBounding
        /// </summary>
        private void DrawSelectedBounding()
        {
            if (MyConfig.EditorDisplayUnselectedBounding)
            {
                Vector4 color = new Vector4(0.6f, 0.6f, 0.6f, 0.2f);
                foreach (MyEntity entity in MyEntities.GetEntities())
                {
                    entity.DebugDrawBox(color, false);
                }

                foreach (MyVoxelMap voxelMap in MyVoxelMaps.GetVoxelMaps())
                {
                    MyDebugDraw.DrawHiresBoxWireframe(Matrix.CreateScale(voxelMap.GetSize()) * voxelMap.WorldMatrix, new Vector3(0, 0.4f, 0), 0.3f);
                }
            }

            if (IsEditingPrefabContainer())
            {
                m_activePrefabContainer.DebugDrawBox(new Vector4(1, 1, 0, 0.4f));
            }

            foreach (MyEntity entity in MyEditorGizmo.SelectedEntities)
            {
                if (entity is MyPrefabContainer)
                {
                    MyPrefabContainer container = (MyPrefabContainer)entity;
                    container.DebugDrawBox(new Vector4(0, 1, 0, 0.4f));
                }
            }
        }



        //@ This stuff is here just for debugging selection
        /*private MyLine m_testSelectLine;
        private List<BoundingSphere> m_spheres = new List<BoundingSphere>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mouseSelectionLine"></param>
        private void FillDebugDrawOfSelection(ref MyLine mouseSelectionLine)
        {
            m_spheres.Clear();
            m_testSelectLine = mouseSelectionLine;
            MyPruningStructure m_PruningStructure = MyPhysics.physicsSystem.GetRigidBodyModule().GetPruningStructure();

            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddLine(ref m_testSelectLine, ref boundingBox);
            List<MyRBElement> elements = new List<MyRBElement>(256);
            m_PruningStructure.OverlapRBAllBoundingBox(ref boundingBox, ref elements);

            foreach (MyRBElement element in elements)
            {
                MyEntity entity = ((MinerWars.AppCode.Game.Managers.PhysicsManager.Physics.MyRigidBody)element.GetRigidBody().m_UserData).Entity;
                BoundingSphere vol = entity.WorldVolume;
                m_spheres.Add(vol);

                Matrix mat = Matrix.CreateTranslation(entity.OffsetToVolumeCenter);
                mat = mat * entity.WorldMatrix;
                vol = new BoundingSphere(mat.Translation, vol.Radius);

                m_spheres.Add(vol);
            }
        }


        /// <summary>
        /// DrawDebugSelection
        /// </summary>
        private void DrawDebugSelection()
        {
            Vector3 vctStart = m_testSelectLine.From;
            Vector3 vctEnd = m_testSelectLine.To;
            MyDebugDraw.DrawLine3D(vctStart, vctEnd, Color.Green, Color.Green);

            Vector3 vctColor = new Vector3(1f,0.6f,1f);
            foreach(BoundingSphere sphere in m_spheres)
            {
                Matrix worldMatrix = Matrix.Identity;
                worldMatrix.Translation = sphere.Center;
                worldMatrix = Matrix.CreateScale(sphere.Radius) * worldMatrix;

                MyDebugDraw.DrawSphereWireframe(worldMatrix, vctColor, 1f);
            }
        }*/

        public void SetActive(bool bEnable) 
        { 
            m_active = bEnable;
            if (m_active == false)
            {
                SwitchToGameplay();
            }
        }

        public bool IsActive() 
        { 
            return m_active; 
        }
    }
}
