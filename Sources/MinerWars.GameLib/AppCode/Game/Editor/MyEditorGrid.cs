using System;
using System.Linq;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;

namespace MinerWars.AppCode.Game.Editor
{
    static class MyEditorGrid
    {
        public enum GridOrientation
        {
            ORIENTATION_XY,
            ORIENTATION_XZ,
            ORIENTATION_YZ
        }

        public enum GridStep : short
        {
            STEP_LEVEL_1 = 1,   //0,1m
            STEP_LEVEL_2 = 5,
            STEP_LEVEL_3 = 10,
            STEP_LEVEL_4 = 50,
            STEP_LEVEL_5 = 100,
            STEP_LEVEL_6 = 1000  //100m
        }

        static GridStep m_gridStepMode;
        static GridOrientation m_gridOrientation;
        static Vector4 m_gridColor;
        static bool m_isGridVisible;
        static bool m_isGridActive;
        static float m_gridStepInMeters;

        public static void LoadData()
        {
            m_gridStepMode = MyFakes.MWBUILDER ? GridStep.STEP_LEVEL_4 : GridStep.STEP_LEVEL_3;
            m_gridOrientation = GridOrientation.ORIENTATION_XY;
            m_gridColor = MyEditorConstants.COLOR_WHITE;
            m_isGridVisible = false;
            m_gridStepInMeters = GetGridStepInMeters();
        }

        public static float GetGridStepInMeters()
        {
            return (((short)m_gridStepMode) / (float)MyEditorConstants.GRID_CONVERSION_UNIT);
        }

        public static bool IsGridActive
        {
            get { return m_isGridActive; }
            set { m_isGridActive = value; }
        }

        public static void IncreaseGridStep()
        {
            if (IsMaxGridStep() == false)
            {
                m_gridStepMode = MyUtils.GetNextOrPreviousEnumValue(m_gridStepMode, true);
            }
        }

        public static void DecreaseGridStep()
        {
            if (IsMinGridStep() == false)
            {
                m_gridStepMode = MyUtils.GetNextOrPreviousEnumValue(m_gridStepMode, false);
            }
        }

        public static void SwitchGridOrientation()
        {
            if (m_isGridActive)
            {
                m_gridOrientation = MyUtils.GetNextOrPreviousEnumValue(m_gridOrientation, true);
            }
        }

        public static bool IsMaxGridStep()
        {
            return m_gridStepMode == GridStep.STEP_LEVEL_6;
        }

        public static bool IsMinGridStep()
        {
            return m_gridStepMode == GridStep.STEP_LEVEL_1;
        }

        public static void Update()
        {
            if (m_isGridActive)
            {
                if (MyEditorGizmo.SelectedEntities.Count > 0)
                {
                    MyEntity selectedPhysObject = MyEditorGizmo.SelectedEntities.ElementAt(0);
                    if (selectedPhysObject != null)
                    {
                        BoundingSphere vol = selectedPhysObject.WorldVolume;
                        var spectatorPosition = MySpectator.Position;
                        float distance = Math.Abs(MyUtils.GetSmallestDistanceToSphere(ref spectatorPosition, ref vol));
                        //float maxDistance = MyEditorConstants.MAX_DISTANCE_TO_DRAW_GRID * m_gridStepInMeters;
                        //float normalizedDistance = MathHelper.Clamp(distance - maxDistance, 0, 1);
                        //Vector4.Lerp(ref MyEditorConstants.COLOR_WHITE, ref MyEditorConstants.COLOR_BLACK, 1, out CurrentGridColor);

                        if (distance > MyEditorConstants.MAX_DISTANCE_TO_DRAW_GRID * m_gridStepInMeters)
                        {
                            ShadeOutGrid();
                        }
                        else
                        {
                            ShadeInGrid();
                        }
                    }
                    else
                    {
                        //Vector4.Lerp(ref MyEditorConstants.COLOR_WHITE, ref MyEditorConstants.COLOR_BLACK, 1, out CurrentGridColor);
                        ShadeOutGrid();
                    }
                }
                else
                {
                    ShadeInGrid();
                }
            }
        }

        private static void ShadeOutGrid()
        {
            if (m_gridColor.X < 0.01f && m_gridColor.Y < 0.01f && m_gridColor.Z < 0.01f)
            {
                m_isGridVisible = false;
            }

            if (m_isGridVisible)
            {   
                //CurrentGridColor = Color.Lerp(MyEditorConstants.COLOR_WHITE, MyEditorConstants.COLOR_BLACK,
			    m_gridColor.X -= 0.01f;
                m_gridColor.Y -= 0.01f;
                m_gridColor.Z -= 0.01f;
            }
        }

        private static void ShadeInGrid()
        {
            m_isGridVisible = true; 

            if (m_gridColor.X < MyEditorConstants.DEFAULT_GRID_COLOR.X && m_gridColor.Y < MyEditorConstants.DEFAULT_GRID_COLOR.Y &&
                m_gridColor.Z < MyEditorConstants.DEFAULT_GRID_COLOR.Z)
            {
                m_gridColor.X += 0.01f;
                m_gridColor.Y += 0.01f;
                m_gridColor.Z += 0.01f;
            }
        }

        public static void Draw()
        {
            if (m_isGridActive && m_isGridVisible)
            {
                float billboardDelta = m_gridStepInMeters * 10;
          
                Vector3 selectedObjectPosition = new Vector3(0,0,0);
                Vector3 selectedObjectSize = MinerWars.CommonLIB.AppCode.Utils.MyMwcSectorConstants.SECTOR_SIZE_VECTOR3;

                if (MyEditorGizmo.SelectedEntities.Count > 0)
                {
                    MyEntity selectedPhysObject = MyEditorGizmo.SelectedEntities.ElementAt(0);
                    selectedObjectPosition = selectedPhysObject.GetPosition();
                    selectedObjectSize = selectedPhysObject.WorldAABB.Size();
                }

                Vector3 gridStartPosition;
                Vector3 billboardPositionDelta;
                Vector3 orientationA;
                Vector3 orientationB;
                float sizeA;
                float sizeB;

                if (m_gridOrientation == GridOrientation.ORIENTATION_XY)
                {
                    orientationA = Vector3.Right;
                    orientationB = Vector3.Up;
                    sizeA = selectedObjectSize.X;
                    sizeB = selectedObjectSize.Y;
                    gridStartPosition = selectedObjectPosition - new Vector3(sizeA / 2, sizeB / 2, 0);
                    billboardPositionDelta = new Vector3(billboardDelta, billboardDelta, 0);
                }
                else if (m_gridOrientation == GridOrientation.ORIENTATION_XZ)
                {
                    orientationA = -Vector3.Forward;
                    orientationB = Vector3.Right;
                    sizeA = selectedObjectSize.X;
                    sizeB = selectedObjectSize.Z;
                    gridStartPosition = selectedObjectPosition - new Vector3(sizeA / 2, 0, sizeB / 2);
                    billboardPositionDelta = new Vector3(billboardDelta, 0, billboardDelta);
                }
                else
                {
                    orientationA = Vector3.Up;
                    orientationB = -Vector3.Forward;
                    sizeA = selectedObjectSize.Y;
                    sizeB = selectedObjectSize.Z;
                    gridStartPosition = selectedObjectPosition - new Vector3(0, sizeA / 2, sizeB / 2);
                    billboardPositionDelta = new Vector3(0, billboardDelta, billboardDelta);
                }


                for (float gridADirection = 0; gridADirection < sizeA; gridADirection += billboardDelta)
                {
                    Vector3 startPosition = gridStartPosition + orientationA * gridADirection;
                    Vector3 endPosition = gridStartPosition + orientationA * gridADirection + orientationB * sizeB;

                    MyTransparentGeometry.AddLineBillboard2(MyTransparentMaterialEnum.ProjectileTrailLine, m_gridColor, 
                        startPosition, endPosition, 1);
                }

                for (float gridBDirection = 0; gridBDirection < sizeB; gridBDirection += billboardDelta)
                {
                    Vector3 startPosition = gridStartPosition + orientationB * gridBDirection;
                    Vector3 endPosition = gridStartPosition + orientationB * gridBDirection + orientationA * sizeA;

                    MyTransparentGeometry.AddLineBillboard2(MyTransparentMaterialEnum.ProjectileTrailLine, m_gridColor,
                        startPosition, endPosition, 1);
                }
            }
        }
    }
}
