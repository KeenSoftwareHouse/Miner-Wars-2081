using System;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Utils;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.Editor
{
    using Byte4 = MinerWarsMath.Graphics.PackedVector.Byte4;
    using HalfVector2 = MinerWarsMath.Graphics.PackedVector.HalfVector2;
    using HalfVector4 = MinerWarsMath.Graphics.PackedVector.HalfVector4;
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Quaternion = MinerWarsMath.Quaternion;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MathHelper = MinerWarsMath.MathHelper;
    using Ray = MinerWarsMath.Ray;
    using Plane = MinerWarsMath.Plane;

    #region Enums

    public enum GizmoAxis
    {
        X,
        Y,
        Z,
        XY,
        ZX,
        YZ,
        NONE
    }

    public enum GizmoMode
    {
        TRANSLATE,
        ROTATE,
    }

    public enum PivotType
    {
        MODEL_CENTER,
        OBJECT_CENTER,
        SELECTION_CENTER
    }

    public enum TransformSpace
    {
        LOCAL,
        WORLD
    }


    public enum RotateSnapping
    {
        NONE,
        FIVE_DEGREES,
        FIFTEEN_DEGREES,
        FORTYFIVE_DEGREES,
        NINETY_DEGREES
    }

    #endregion

    /// <summary>
    /// This class represents transformation gizmo, that provides users of editor translation and rotation functionality on
    /// selected objects(both mouse and keyboard input way)
    /// </summary>
    static class MyEditorGizmo
    {

        struct MyPositionColor
        {
            public MyPositionColor(Vector3 pos, Color col)
            {
                Position = pos;
                Color = col;
            }

            public Vector3 Position;
            public Color Color;
        }

        #region Properties

        /// <summary>
        /// Action created when transformation was done with some entities
        /// </summary>
        private static MyEditorActionEntityTransform m_transformAction;

        /// <summary>
        /// Enables/Disables precision mode
        /// </summary>
        private static bool m_precisionMode = false;

        /// <summary>
        /// The value to adjust all transformation when precisionMode is active.
        /// </summary>
        private static float m_precisionModeScale = 0.1f;

        /// <summary>
        /// Gizmo quaternion rotation
        /// </summary>
        private static Quaternion m_rotation { get { return Quaternion.CreateFromRotationMatrix(Rotation); } }

        /// <summary>
        /// Translation scale snap delta
        /// </summary>
        private static Vector3 m_translationScaleSnapDelta;

        /// <summary>
        /// Rotation snap delta
        /// </summary>
        private static float m_rotationSnapDelta;

        /// <summary>
        /// When moving objects with keys, wait this time before it will be move smoothly by holding the key
        /// </summary>
        private static int m_delayObjectMovementInMillis;

        /// <summary>
        /// When moving objects with keys, wait this time before playing movement sound
        /// </summary>
        private static int m_delayMovementSoundInMillis;

        /// <summary>
        /// Used when we need to modify collection during iterating of its entities
        /// </summary>
        private static List<MyEntity> m_safeIterationHelper;

        /// <summary>
        /// Last cursor position
        /// </summary>
        private static Vector2 m_lastCursorPosition;

        /// <summary>
        /// Used for calculation of rotation delta to be displayed in editor info
        /// </summary>
        private static float m_deltaYaw, m_deltaPitch, m_deltaRoll;

        /// <summary>
        /// 
        /// </summary>
        private static Vector2 m_oldCursorPosition;

        /// <summary>
        /// Local gizmo forward
        /// </summary>
        private static Vector3 m_localForward = Vector3.Forward;

        /// <summary>
        /// Local gizmo up
        /// </summary>
        private static Vector3 m_localUp = Vector3.Up;

        /// <summary>
        /// Local gizmo right
        /// </summary>
        private static Vector3 m_localRight;

        /// <summary>
        /// Screen scale matrix
        /// </summary>
        private static Matrix m_screenScaleMatrix;

        /// <summary>
        /// Screen scale
        /// </summary>
        private static float m_screenScale;

        /// <summary>
        /// Input scale
        /// </summary>
        private static float m_inputScale;

        /// <summary>
        /// Object oriented world
        /// </summary>
        private static Matrix m_objectOrientedWorld;

        /// <summary>
        /// Axis-aligned world
        /// </summary>
        private static Matrix m_axisAlignedWorld;

        /// <summary>
        /// Local space of models used for gizmo
        /// </summary>
        private static Matrix[] m_modelLocalSpace;

        /// <summary>
        /// Translation model
        /// </summary>
        private static MyModel m_translationModel;

        /// <summary>
        /// Rotation model
        /// </summary>
        private static MyModel m_rotationModel;

        /// <summary>
        /// Used for all drawing, assigned by local- or world-space matrices
        /// </summary>
        private static Matrix m_gizmoWorld = Matrix.Identity;

        /// <summary>
        /// The matrix used to apply to your whole scene, usually matrix.identity (default scale, origin on 0,0,0 etc.)
        /// </summary>
        private static Matrix m_sceneWorld;

        /// <summary>
        /// Translation Line vertices
        /// </summary>
        private static MyPositionColor[] m_translationLineVertices;

        /// <summary>
        /// Translation line length
        /// </summary>
        private static float m_lineLength = 3f;

        /// <summary>
        /// Translation line offset
        /// </summary>
        private static float m_lineOffset = 0.5f;

        /// <summary>
        /// Colors used for each gizmo axis
        /// </summary>
        private static Color[] m_axisColors;

        /// <summary>
        /// Highlighted gizmo axis color
        /// </summary>
        private static Color m_highlightColor;

        /// <summary>
        /// Offset of text drawn for each axis(X, Y, Z)
        /// </summary>
        private static Vector3 m_axisTextOffset = new Vector3(0, 0.5f, 0);

        /// <summary>
        /// Text Wrappers for axis texts
        /// </summary>
        private static List<MyTextsWrapperEnum> m_axisTexts;

        /// <summary>
        /// Translation delta vector
        /// </summary>
        private static Vector3 m_translationDelta;

        /// <summary>
        /// Last intersection position with plane of currently selected axis
        /// </summary>
        private static Vector3 m_lastIntersectionPosition;

        /// <summary>
        /// Current intersection position with plane of currently selected axis
        /// </summary>
        private static Vector3 m_intersectPosition;

        /// <summary>
        /// Gizmo axis bounding box thickness
        /// </summary>
        private static float m_boxThickness = 0.3f;

        /// <summary>
        /// Gizmo axis sphere radius
        /// </summary>
        private static float m_boundingSphereRadius = 1f;

        /// <summary>
        /// Vector from gizmo center to mouse intersection position (user never clicks to the center of gizmo)
        /// </summary>
        private static Vector3? m_gizmoDelta = null;

        private static Vector3? m_gizmoStartPosition = null;
        private static Matrix? m_gizmoStartRotation = null;

        private static Vector3 m_startCameraForward;
        private static Vector3 m_startCameraUp;
        private static Vector3 m_startObjectPosition;

        private static Vector2 m_mouseStartPosition;

        // -- BoundingBoxes -- //
        #region BoundingBoxes

        private static MyOrientedBoundingBox X_Box
        {
            get
            {
                return new MyOrientedBoundingBox(Vector3.Transform(new Vector3((m_lineLength / 2) + (m_lineOffset * 2), 0, 0), m_gizmoWorld),
                    Vector3.Transform(new Vector3(m_lineLength / 2, 0.2f, 0.2f), m_screenScaleMatrix), m_rotation);
            }
        }

        private static MyOrientedBoundingBox Y_Box
        {
            get
            {
                return new MyOrientedBoundingBox(Vector3.Transform(new Vector3(0, (m_lineLength / 2) + (m_lineOffset * 2), 0), m_gizmoWorld),
                    Vector3.Transform(new Vector3(0.2f, m_lineLength / 2, 0.2f), m_screenScaleMatrix), m_rotation);
            }
        }

        private static MyOrientedBoundingBox Z_Box
        {
            get
            {
                return new MyOrientedBoundingBox(Vector3.Transform(new Vector3(0, 0, (m_lineLength / 2) + (m_lineOffset * 2)), m_gizmoWorld),
                    Vector3.Transform(new Vector3(0.2f, 0.2f, m_lineLength / 2), m_screenScaleMatrix), m_rotation);
            }
        }

        private static MyOrientedBoundingBox XZ_Box
        {
            get
            {
                return new MyOrientedBoundingBox(Vector3.Transform(new Vector3(m_lineOffset, 0, m_lineOffset), m_gizmoWorld),
                    Vector3.Transform(new Vector3(m_lineOffset, m_boxThickness, m_lineOffset), m_screenScaleMatrix), m_rotation);
            }
        }

        private static MyOrientedBoundingBox XY_Box
        {
            get
            {
                return new MyOrientedBoundingBox(Vector3.Transform(new Vector3(m_lineOffset, m_lineOffset, 0), m_gizmoWorld),
                    Vector3.Transform(new Vector3(m_lineOffset, m_lineOffset, m_boxThickness), m_screenScaleMatrix), m_rotation);
            }
        }

        private static MyOrientedBoundingBox YZ_Box
        {
            get
            {
                return new MyOrientedBoundingBox(Vector3.Transform(new Vector3(0, m_lineOffset, m_lineOffset), m_gizmoWorld),
                    Vector3.Transform(new Vector3(m_boxThickness, m_lineOffset, m_lineOffset), m_screenScaleMatrix), m_rotation);
            }
        }

        #endregion

        // -- BoundingSpheres -- //
        #region BoundingSpheres

        private static BoundingSphere X_Sphere
        {
            get { return new BoundingSphere(Vector3.Transform(m_translationLineVertices[1].Position, m_gizmoWorld), m_boundingSphereRadius * m_screenScale); }
        }

        private static BoundingSphere Y_Sphere
        {
            get { return new BoundingSphere(Vector3.Transform(m_translationLineVertices[7].Position, m_gizmoWorld), m_boundingSphereRadius * m_screenScale); }
        }

        private static BoundingSphere Z_Sphere
        {
            get { return new BoundingSphere(Vector3.Transform(m_translationLineVertices[13].Position, m_gizmoWorld), m_boundingSphereRadius * m_screenScale); }
        }
        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// Selected entities
        /// </summary>
        public static List<MyEntity> SelectedEntities = new List<MyEntity>();

        private static MyPrefabSnapPoint m_selectedSnapPoint;
        /// <summary>
        /// SelectedSnapPoint
        /// </summary>
        public static MyPrefabSnapPoint SelectedSnapPoint
        {
            get
            {
                return m_selectedSnapPoint;
            }
            set
            {
                if (m_selectedSnapPoint != null)
	            {
                    // Clear highlighting on previous snap point objects
                    m_selectedSnapPoint.Prefab.ClearHighlightning();

                    List<MyEntity> linkedEntities = new List<MyEntity>();
                    MyEditor.Static.GetLinkedEntities(m_selectedSnapPoint.Prefab, linkedEntities, null);
                    linkedEntities.ForEach(a => a.ClearHighlightning());
	            }

                if (value != null)
                {
                    // Set highlighting on selected snap point objects
                    value.Prefab.HighlightEntity(ref MyEditorConstants.LINKED_OBJECT_DIFFUSE_COLOR_ADDITION);
                    
                    List<MyEntity> linkedEntities = new List<MyEntity>();
                    MyEditor.Static.GetLinkedEntities(value.Prefab, linkedEntities, null);
                    linkedEntities.ForEach(a => a.HighlightEntity(ref MyEditorConstants.LINKED_OBJECT_DIFFUSE_COLOR_ADDITION));
                }

                m_selectedSnapPoint = value;
            }
        }

        /// <summary>
        /// Enabled only when some object is selected
        /// </summary>
        public static bool Enabled;

        /// <summary>
        /// True if any gizmo transformation is active
        /// </summary>
        public static bool TransformationActive;

        /// <summary>
        /// True if any gizmo transformation was active
        /// </summary>
        public static bool TransformationActivePrevious;

        /// <summary>
        /// Currently active axis
        /// </summary>
        private static GizmoAxis _activeAxis;
        public static GizmoAxis ActiveAxis
        {
            get
            {
                return _activeAxis;
            }
            set
            {
                if (_activeAxis != GizmoAxis.NONE)
                    LastActiveAxis = _activeAxis;
                _activeAxis = value;
            }
        }
        private static GizmoAxis LastActiveAxis;

        /// <summary>
        /// Currently active mode
        /// </summary>
        public static GizmoMode ActiveMode = GizmoMode.TRANSLATE;

        /// <summary>
        /// Currently active mode
        /// </summary>
        public static RotateSnapping ActiveRotateSnapping = RotateSnapping.NINETY_DEGREES;

        /// <summary>
        /// Currently active space
        /// </summary>
        public static TransformSpace ActiveSpace = TransformSpace.WORLD;

        /// <summary>
        /// Currently active pivot
        /// </summary>
        public static PivotType ActivePivot;

        /// <summary>
        /// Gizmo position
        /// </summary>
        public static Vector3 Position;

        /// <summary>
        /// Gizmo rotation
        /// </summary>
        public static Matrix Rotation = Matrix.Identity;

        /// <summary>
        /// Rotation amount in degrees
        /// </summary>
        public static int RotationAmountInDegrees;

        /// <summary>
        /// Snapping enabled/disabled
        /// </summary>
        public static bool SnapEnabled = true;

        /// <summary>
        /// Translation snap value
        /// </summary>
        public static float TranslationSnapValue = 5;

        /// <summary>
        /// Rotation snap value
        /// </summary>
        public static float RotationSnapValue = 45;

        public static readonly float TRANSLATION_MAX_DISTANCE_FROM_CAMERA = MyCamera.FAR_PLANE_DISTANCE / 2;

        #endregion

        #region Load/Unload Methods


        public static void LoadData()
        {
            ActiveSpace = TransformSpace.LOCAL;
            SelectedEntities.Clear();
            m_safeIterationHelper = new List<MyEntity>(100);

            m_sceneWorld = Matrix.Identity;

            // -- Set local-space offset -- //
            m_modelLocalSpace = new Matrix[3];
            m_modelLocalSpace[0] = Matrix.CreateWorld(new Vector3(m_lineLength, 0, 0), Vector3.Up, Vector3.Right);
            m_modelLocalSpace[1] = Matrix.CreateWorld(new Vector3(0, m_lineLength, 0), Vector3.Left, Vector3.Up);
            m_modelLocalSpace[2] = Matrix.CreateWorld(new Vector3(0, 0, m_lineLength), Vector3.Right, Vector3.Backward);

            // -- Colors: X,Y,Z,Highlight -- //
            m_axisColors = new Color[3];
            m_axisColors[0] = Color.Red;
            m_axisColors[1] = Color.Green;
            m_axisColors[2] = Color.Blue;
            m_highlightColor = Color.Gold;

            m_axisTexts = new List<MyTextsWrapperEnum>();
            m_axisTexts.Add(MyTextsWrapperEnum.AxisX);
            m_axisTexts.Add(MyTextsWrapperEnum.AxisY);
            m_axisTexts.Add(MyTextsWrapperEnum.AxisZ);

            // fill array with vertex-data
            #region Fill Axis-Line array
            List<MyPositionColor> vertexList = new List<MyPositionColor>(18);

            // helper to apply colors
            Color xColor = m_axisColors[0];
            Color yColor = m_axisColors[1];
            Color zColor = m_axisColors[2];

            float doubleLineOffset = m_lineOffset * 2;

            // -- X Axis -- // index 0 - 5
            vertexList.Add(new MyPositionColor(new Vector3(m_lineOffset, 0, 0), xColor));
            vertexList.Add(new MyPositionColor(new Vector3(m_lineLength, 0, 0), xColor));

            vertexList.Add(new MyPositionColor(new Vector3(doubleLineOffset, 0, 0), xColor));
            vertexList.Add(new MyPositionColor(new Vector3(doubleLineOffset, doubleLineOffset, 0), xColor));

            vertexList.Add(new MyPositionColor(new Vector3(doubleLineOffset, 0, 0), xColor));
            vertexList.Add(new MyPositionColor(new Vector3(doubleLineOffset, 0, doubleLineOffset), xColor));

            // -- Y Axis -- // index 6 - 11
            vertexList.Add(new MyPositionColor(new Vector3(0, m_lineOffset, 0), yColor));
            vertexList.Add(new MyPositionColor(new Vector3(0, m_lineLength, 0), yColor));

            vertexList.Add(new MyPositionColor(new Vector3(0, doubleLineOffset, 0), yColor));
            vertexList.Add(new MyPositionColor(new Vector3(doubleLineOffset, doubleLineOffset, 0), yColor));

            vertexList.Add(new MyPositionColor(new Vector3(0, doubleLineOffset, 0), yColor));
            vertexList.Add(new MyPositionColor(new Vector3(0, doubleLineOffset, doubleLineOffset), yColor));

            // -- Z Axis -- // index 12 - 17
            vertexList.Add(new MyPositionColor(new Vector3(0, 0, m_lineOffset), zColor));
            vertexList.Add(new MyPositionColor(new Vector3(0, 0, m_lineLength), zColor));

            vertexList.Add(new MyPositionColor(new Vector3(0, 0, doubleLineOffset), zColor));
            vertexList.Add(new MyPositionColor(new Vector3(doubleLineOffset, 0, doubleLineOffset), zColor));

            vertexList.Add(new MyPositionColor(new Vector3(0, 0, doubleLineOffset), zColor));
            vertexList.Add(new MyPositionColor(new Vector3(0, doubleLineOffset, doubleLineOffset), zColor));
            #endregion

            // -- Convert to array -- //
            m_translationLineVertices = vertexList.ToArray();
        }

        /// <summary>
        /// Load gizmo content
        /// </summary>
        public static void LoadContent()
        {

            m_translationModel = MyModels.GetModelOnlyData(MyModelsEnum.GizmoTranslation);
            m_rotationModel = MyModels.GetModelOnlyData(MyModelsEnum.GizmoRotation);

        }

        /// <summary>
        /// Unloads gizmo content
        /// </summary>
        public static void UnloadData()
        {
            if (SelectedEntities != null) SelectedEntities.Clear();
            if (m_safeIterationHelper != null) m_safeIterationHelper.Clear();
            if (m_axisTexts != null) m_axisTexts.Clear();
        }

        #endregion

        #region Handle Input Methods
        public static void GizmoTranslation(Vector3 planeVector, Vector3? secondPlaneVector, ref Vector3 worldPosition)
        {
            Matrix transform = Matrix.Invert(Rotation);
            Vector3 localPosition = Vector3.Transform(Position, transform);

            Ray ray;

            bool oneAxisMove = !secondPlaneVector.HasValue;

            Vector3 planeMove = planeVector;

            if (oneAxisMove)
            {
                // Choose "better" plane: better is that which has lower dot (bigger angle) in the 2D projection
                var sideDot = MyUtils.Project3dCoordinateTo2dCoordinate(planeVector, m_gizmoWorld).X; // side
                var upDot = MyUtils.Project3dCoordinateTo2dCoordinate(planeVector, m_gizmoWorld).Y; // up
                if (Math.Abs(sideDot) < Math.Abs(upDot))
                {
                    secondPlaneVector = Vector3.Cross(planeVector, m_startCameraUp);
                }
                else
                {
                    secondPlaneVector = Vector3.Cross(planeVector, Vector3.Cross(m_startCameraForward, m_startCameraUp));
                }
                
                ray = CalculateRayProjected(planeVector);
                if (float.IsNaN(ray.Position.X)) return;  // there's no valid axis: don't do anything
            }
            else
            {
                planeMove += secondPlaneVector.Value;
                ray = CalculateRay();
            }

            ray.Position = Vector3.Transform(ray.Position, transform);
            ray.Direction = Vector3.TransformNormal(ray.Direction, transform);

            if (!m_gizmoStartPosition.HasValue)
            {
                m_gizmoStartPosition = localPosition;
                
            }

            Plane plane = new Plane(m_gizmoStartPosition.Value, m_gizmoStartPosition.Value + planeVector, m_gizmoStartPosition.Value + secondPlaneVector.Value);

            float? intersection = ray.Intersects(plane);
            if (intersection.HasValue)
            {
                m_intersectPosition = (ray.Position + (ray.Direction * intersection.Value));
                if (!m_gizmoDelta.HasValue)
                {
                    Vector3 gizmo_delta = m_intersectPosition - localPosition;

                    /*
                    if (planeMove.X != 0) gizmo_delta.X = 0;
                    if (planeMove.Y != 0) gizmo_delta.Y = 0;
                    if (planeMove.Z != 0) gizmo_delta.Z = 0;
                    */
                    m_gizmoDelta = gizmo_delta;
                }

                Vector3 newLocalPosition = (m_intersectPosition - m_gizmoDelta.Value);

                //if (oneAxisMove)
                //{
                //    Vector3 move = newLocalPosition - localPosition;
                //    if (move.Length() > 0)
                //    {
                //        float lenMoveInAxis = Vector3.Dot(move, planeVector);
                //        newLocalPosition = localPosition + planeVector * lenMoveInAxis;
                //    }
                //    else
                //    {
                //        return;
                //    }
                //}

                if (planeMove.X == 0) newLocalPosition.X = m_gizmoStartPosition.Value.X;
                if (planeMove.Y == 0) newLocalPosition.Y = m_gizmoStartPosition.Value.Y;
                if (planeMove.Z == 0) newLocalPosition.Z = m_gizmoStartPosition.Value.Z;

                worldPosition = Vector3.Transform(newLocalPosition, Rotation);
            }
        }

        public static void SnapToGrid(ref Vector3 position, float gridSize)
        {   
            position /= gridSize;
            
            if (m_gizmoStartPosition != null)
            {
                
                if (!MyMwcUtils.IsZero(m_gizmoStartPosition.Value.X - m_gizmoWorld.Translation.X)) position.X = (float) Math.Round(position.X);
                if (!MyMwcUtils.IsZero(m_gizmoStartPosition.Value.Y - m_gizmoWorld.Translation.Y)) position.Y = (float) Math.Round(position.Y);
                if (!MyMwcUtils.IsZero(m_gizmoStartPosition.Value.Z - m_gizmoWorld.Translation.Z)) position.Z = (float) Math.Round(position.Z);
            }
            

            position *= gridSize;
        }


        /*
        private static void SnapRotation(ref Matrix rotation, MyEntity entity)
        {
            if (m_gizmoStartRotation != null)
            {
                float yawFrom = 0;
                float pitchFrom = 0;
                float rollFrom = 0;

                
                Matrix start = m_gizmoStartRotation.Value;
                MyUtils.RotationMatrixToYawPitchRoll(ref start, out yawFrom, out pitchFrom, out rollFrom);

                yawFrom = float.IsNaN(yawFrom) ? 0 : yawFrom;
                pitchFrom = float.IsNaN(pitchFrom) ? 0 : pitchFrom;
                rollFrom = float.IsNaN(rollFrom) ? 0 : rollFrom;

                float yawTo = 0;
                float pitchTo = 0;
                float rollTo = 0;
                MyUtils.RotationMatrixToYawPitchRoll(ref m_gizmoWorld, out yawTo, out pitchTo, out rollTo);

                yawTo = float.IsNaN(yawTo) ? 0 : yawTo;
                pitchTo = float.IsNaN(pitchTo) ? 0 : pitchTo;
                rollTo = float.IsNaN(rollTo) ? 0 : rollTo;

                Vector3 to = new Vector3(yawTo, pitchTo, rollTo);


                var fifteenDegsInRadians = MathHelper.ToRadians(15);

                if (ActiveRotateSnapping == RotateSnapping.FIFTEEN_DEGREES)
                {
                    to /= fifteenDegsInRadians;
                    if (!MyMwcUtils.IsZero(yawFrom - yawTo)) to.X = (float)Math.Round(to.X);
                    if (!MyMwcUtils.IsZero(pitchFrom - pitchTo)) to.Y = (float)Math.Round(to.Y);
                    if (!MyMwcUtils.IsZero(rollFrom - rollTo)) to.Z = (float)Math.Round(to.Z);
                    to *= fifteenDegsInRadians;
                }

                var ninghtyDegsInRadians = MathHelper.ToRadians(90);
                if (ActiveRotateSnapping == RotateSnapping.NINETY_DEEGREES)
                {
                    to /= ninghtyDegsInRadians;
                    if (!MyMwcUtils.IsZero(yawFrom - yawTo)) to.X = (float)Math.Round(to.X);
                    if (!MyMwcUtils.IsZero(pitchFrom - pitchTo)) to.Y = (float)Math.Round(to.Y);
                    if (!MyMwcUtils.IsZero(rollFrom - rollTo)) to.Z = (float)Math.Round(to.Z);
                    to *= ninghtyDegsInRadians;
                }
                


                rotation = Matrix.CreateFromYawPitchRoll(to.X, to.Y, to.Z);


                // use gizmo position for all PivotTypes except for object-center, it should use the entity.position instead.
                Vector3 newPosition;
                Matrix newRotation;
                GetRotationAndPosition(entity, rotation, out newPosition, out newRotation);


                Matrix normalizedMat = new Matrix();
                normalizedMat.Right = Vector3.Normalize(newRotation.Right);
                normalizedMat.Forward = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newRotation.Up), normalizedMat.Right));
                normalizedMat.Up = Vector3.Normalize(Vector3.Cross(normalizedMat.Right, normalizedMat.Forward));
                normalizedMat.Translation = newRotation.Translation;
                normalizedMat.M44 = newRotation.M44;

                MoveAndRotateObject(newPosition, newRotation, entity);


                MyUtils.AssertIsValid(rotation);
            }

        }

         * */


        public static Ray CalculateRay()
        {
            return MyUtils.ConvertMouseToRay();
        }

        public static Ray CalculateRayProjected(Vector3 moveAxis)
        {
            // Get mouse ray and consider only one move axis
            Vector2 dir = MyUtils.Project3dCoordinateTo2dCoordinate(moveAxis, m_gizmoWorld);
            Vector3 start3D = SharpDXHelper.ToXNA(MyMinerGame.Static.GraphicsDevice.Viewport.Project(SharpDXHelper.ToSharpDX(m_gizmoWorld.Translation), SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(Matrix.Identity)));
            Vector2 start = new Vector2(start3D.X, start3D.Y);
                           
            if (float.IsNaN(dir.X))
            {
                return new MinerWarsMath.Ray(new Vector3(float.NaN, float.NaN, float.NaN), new Vector3(float.NaN, float.NaN, float.NaN));  // can't project, return NaNs
            }
            Vector2 mousePos = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(MyGuiManager.MouseCursorPosition);
            return MyUtils.ConvertMouseToRay(start + dir * Vector2.Dot(mousePos - start, dir));
        }

        public static void HandleInput(MyGuiInput input)
        {
            // -- Select Gizmo Mode -- //
            if (input.IsEditorControlNewPressed(MyEditorControlEnums.SWITCH_GIZMO_MODE) && !input.IsAnyCtrlKeyPressed())
            {
                SwitchGizmoMode();
            }

            // -- Select Gizmo Mode -- //
            if (input.IsEditorControlNewPressed(MyEditorControlEnums.SWITCH_GIZMO_MODE) && input.IsAnyCtrlKeyPressed() )
            {
                SwitchRotateSnapping();
            }



            // -- Cycle TransformationSpaces -- //
            if (input.IsEditorControlNewPressed(MyEditorControlEnums.SWITCH_GIZMO_SPACE))
            {
                if (ActiveSpace == TransformSpace.LOCAL)
                    ActiveSpace = TransformSpace.WORLD;
                else
                {
                    ActiveSpace = TransformSpace.LOCAL;
                }
            }

            // Below are options, that we can add later, but gizmo is prepared for them
            // however, key mappings will be probably changed
            // -- Cycle PivotTypes -- //
            //if (input.IsNewKeyPress(Keys.P))
            //{
            //    ActivePivot++;
            //}

            // -- Toggle PrecisionMode -- //
            //if (input.IsKeyPress(Keys.K))
            //{
            //    m_precisionMode = true;
            //}
            //else
            //{
            //    m_precisionMode = false;
            //}

            // -- Toggle Snapping -- //
            //if (MyEditor.Static.IsEditingPrefabContainer() == false)
            //{
            if (input.IsNewKeyPress(Keys.G))            
            {
                SnapEnabled = !SnapEnabled;
            }
            //}

            if (Enabled)
            {
                if (input.IsEditorControlNewPressed(MyEditorControlEnums.PRIMARY_ACTION_KEY))
                {
                    // reset for intersection (plane vs. ray)
                    m_translationDelta = Vector3.Zero;
                    m_intersectPosition = Vector3.Zero;
                    // reset for snapping
                    m_translationScaleSnapDelta = Vector3.Zero;
                    m_rotationSnapDelta = 0;
                    m_lastCursorPosition = MyGuiManager.MouseCursorPosition;
                    m_oldCursorPosition = MyGuiManager.MouseCursorPosition;
                    m_gizmoDelta = null;
                    m_gizmoStartPosition = null;
                    m_mouseStartPosition = m_lastCursorPosition;

                    m_startCameraForward = MyCamera.ForwardVector;
                    m_startCameraUp = MyCamera.UpVector;
                    m_startObjectPosition = Position;
                }

                m_lastIntersectionPosition = m_intersectPosition;
                TransformationActive = false;

                if (input.IsEditorControlPressed(MyEditorControlEnums.PRIMARY_ACTION_KEY))
                {
                    if (MyEditor.TransformLocked)
                    {
                        ActiveAxis = LastActiveAxis;
                    }

                    if (ActiveAxis != GizmoAxis.NONE)
                    {

                        //this will help to disable rectangular selection of multiple objects during object transformation with mouse
                        TransformationActive = true;

                        if (ActiveMode == GizmoMode.TRANSLATE)
                        {
                            #region Translate

                            if (HasTransformationStarted())
                            {
                                StartTransformationData();
                            }
                            //if (input.IsAnyShiftKeyPressed() && input.IsNewLeftMousePressed())
                            //{
                            //    MyEditor.Static.CopySelected(false);
                            //}
                            //else if (HasTransformationStarted())
                            //{
                            //    StartTransformationData();
                            //}

                            Vector3 delta = Vector3.Zero;

                            Vector3 worldPosition = Position;

                            if (ActiveAxis == GizmoAxis.XY)
                            {
                                GizmoTranslation(Vector3.Left, Vector3.Up, ref worldPosition);
                            }
                            else if (ActiveAxis == GizmoAxis.YZ)
                            {
                                GizmoTranslation(Vector3.Up, Vector3.Forward, ref worldPosition);
                            }
                            else if (ActiveAxis == GizmoAxis.ZX)
                            {
                                GizmoTranslation(Vector3.Left, Vector3.Forward, ref worldPosition);
                            }
                            else if (ActiveAxis == GizmoAxis.X)
                            {
                                GizmoTranslation(Vector3.Left, null, ref worldPosition);
                            }
                            else if (ActiveAxis == GizmoAxis.Y)
                            {
                                GizmoTranslation(Vector3.Up, null, ref worldPosition);
                            }
                            else if (ActiveAxis == GizmoAxis.Z)
                            {
                                GizmoTranslation(Vector3.Forward, null, ref worldPosition);
                            }

                            // When object is half distance to far, stop it's movement
                            Plane maxMovePlane = MyCamera.GetBoundingFrustum().Far;
                            maxMovePlane.D *= TRANSLATION_MAX_DISTANCE_FROM_CAMERA / MyCamera.FAR_PLANE_DISTANCE;

                            Vector3 moveVector = worldPosition - Position;
                            Ray moveRay = new Ray(Position, -moveVector);

                            float? intersection = moveRay.Intersects(maxMovePlane);

                            Vector3 cam = MyCamera.Position;

                            // Intersection found and moving object towards far clip plane
                            if (intersection.HasValue && Vector3.Dot(MyCamera.ForwardVector, moveVector) > 0 && (worldPosition - cam).Length() > Math.Abs(maxMovePlane.D))
                            {
                                Vector3 intersectionPoint = (moveRay.Position + (moveRay.Direction * intersection.Value));
                                worldPosition = intersectionPoint;
                            }

                            moveVector = worldPosition - Position;

                            // copy selected object only when moved from his original position + LMB + Shift
                            bool copySelected = (moveVector.LengthSquared() >= 0f &&
                                input.IsAnyShiftKeyPressed() &&
                                input.IsNewLeftMousePressed());

                            if (copySelected)
                            {
                                MyEditor.Static.CopySelected(false);
                            }

                            bool applyTranslation = true;

                            PrepareSafeSelectedEntitiesIterationHelper();
                            foreach (var entity in m_safeIterationHelper)
                            {
                                Vector3 newPosition = entity.GetPosition() + moveVector;

                                BoundingSphere sphere = new BoundingSphere(newPosition + entity.LocalVolumeOffset, entity.WorldVolume.Radius);
                                if (!entity.CanMoveAndRotate(newPosition, entity.GetWorldRotation()) || MyMwcSectorConstants.SECTOR_SIZE_FOR_PHYS_OBJECTS_BOUNDING_BOX.Contains(sphere) != MinerWarsMath.ContainmentType.Contains)
                                {
                                    applyTranslation = false;
                                    break;
                                }
                            }

                            if (applyTranslation)
                            {
                                PrepareSafeSelectedEntitiesIterationHelper();

                                List<MyEntity> transformedEntities = new List<MyEntity>();

                                // apply
                                foreach (MyEntity entity in m_safeIterationHelper)
                                {
                                    // skip already transformed linked entities
                                    if (transformedEntities.Contains(entity))
                                    {
                                        continue;
                                    }

                                    Vector3 newPosition = entity.GetPosition() + moveVector;

                                    //snaping after mouse over
                                    /*
                                    // Snap to grid
                                    if (SnapEnabled && ActiveSpace == TransformSpace.WORLD && !MyEditor.Static.IsLinked(entity))
                                    {
                                        SnapToGrid(ref newPosition, TranslationSnapValue);
                                    }
                                    */


                                    MoveAndRotateObject(newPosition, entity.GetWorldRotation(), entity);

                                    MyEditor.Static.FixLinkedEntities(entity, transformedEntities, null);
                                    transformedEntities.Add(entity);
                                }

                                // test if some entities are intersection
                                //CheckCollisions(m_safeIterationHelper); 
                                //MyEditor.Static.CheckAllCollidingObjects();                                
                                MyEditor.Static.HighlightCollisions(m_safeIterationHelper);

                            }

                            #endregion
                        }
                        else if (ActiveMode == GizmoMode.ROTATE)
                        {
                            #region Rotate

                            if (HasTransformationStarted()) StartTransformationData();
                            Vector2 deltaCursorPosition;
                            float delta;
                            if (!MyVideoModeManager.IsHardwareCursorUsed())
                            {
                                Vector2 minMouseCoord = MyGuiManager.GetMinMouseCoord();
                                Vector2 maxMouseCoord = MyGuiManager.GetMaxMouseCoord();

                                Vector2 newCursorPosition = MyGuiManager.MouseCursorPosition;
                                if (MyGuiManager.MouseCursorPosition.X <= minMouseCoord.X) newCursorPosition = new Vector2(maxMouseCoord.X, MyGuiManager.MouseCursorPosition.Y);
                                if (MyGuiManager.MouseCursorPosition.X >= maxMouseCoord.X) newCursorPosition = new Vector2(minMouseCoord.X, MyGuiManager.MouseCursorPosition.Y);
                                if (MyGuiManager.MouseCursorPosition.Y <= minMouseCoord.Y) newCursorPosition = new Vector2(MyGuiManager.MouseCursorPosition.X, maxMouseCoord.Y);
                                if (MyGuiManager.MouseCursorPosition.Y >= maxMouseCoord.Y) newCursorPosition = new Vector2(MyGuiManager.MouseCursorPosition.X, minMouseCoord.Y);
                                MyGuiManager.MouseCursorPosition = newCursorPosition;
                                //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(MyGuiManager.MouseCursorPosition.ToString() + " " + MyGuiManager.MouseCursorPosition.ToString());

                                float deltaX = MathHelper.ToRadians(input.GetMouseXForGamePlay() - MyMinerGame.ScreenSizeHalf.X);
                                float deltaY = MathHelper.ToRadians(input.GetMouseYForGamePlay() - MyMinerGame.ScreenSizeHalf.Y);

                                deltaX = MathHelper.Clamp(deltaX, -1, 1);
                                deltaY = MathHelper.Clamp(deltaY, -1, 1);

                                delta = deltaY + deltaX;
                                Vector2 cursorDelta = Vector2.Zero;
                                if (MyGuiManager.MouseCursorPosition.Y != 0.0 && MyGuiManager.MouseCursorPosition.Y != 1.0)
                                {
                                    cursorDelta = MyGuiManager.MouseCursorPosition - m_lastCursorPosition;
                                }
                                m_inputScale = MathHelper.Clamp(Math.Abs(cursorDelta.Y) * 100, 0, 1);
                                delta *= m_inputScale;
                                m_oldCursorPosition = MyGuiManager.MouseCursorPosition;

                            }
                            else
                            {

                                Vector2 minMouseCoord = MyGuiManager.GetMinMouseCoord();
                                Vector2 maxMouseCoord = MyGuiManager.GetMaxMouseCoord();
                                Vector2 newCursorPosition = MyGuiManager.MouseCursorPosition;

                                deltaCursorPosition = MyGuiManager.MouseCursorPosition - m_oldCursorPosition;

                                if (MyGuiManager.MouseCursorPosition.X - 0.03f <= minMouseCoord.X)
                                {
                                    deltaCursorPosition = new Vector2(0, 0);
                                    newCursorPosition = new Vector2(maxMouseCoord.X - 0.05f, MyGuiManager.MouseCursorPosition.Y);
                                }
                                if (MyGuiManager.MouseCursorPosition.X + 0.03f >= maxMouseCoord.X * 0.98f)
                                {
                                    deltaCursorPosition = new Vector2(0, 0);
                                    newCursorPosition = new Vector2(minMouseCoord.X + 0.05f, MyGuiManager.MouseCursorPosition.Y);
                                }
                                if (MyGuiManager.MouseCursorPosition.Y - 0.03f <= minMouseCoord.Y)
                                {
                                    deltaCursorPosition = new Vector2(0, 0);
                                    newCursorPosition = new Vector2(MyGuiManager.MouseCursorPosition.X, maxMouseCoord.Y - 0.05f);
                                }
                                if (MyGuiManager.MouseCursorPosition.Y + 0.03f >= maxMouseCoord.Y)
                                {
                                    deltaCursorPosition = new Vector2(0, 0);
                                    newCursorPosition = new Vector2(MyGuiManager.MouseCursorPosition.X, minMouseCoord.Y + 0.05f);
                                }
                                MyGuiManager.MouseCursorPosition = newCursorPosition;

                                delta = (deltaCursorPosition.X + deltaCursorPosition.Y) * 4.0f;
                                delta = MathHelper.Clamp(delta, -2.0f, 2.0f);
                                m_oldCursorPosition = MyGuiManager.MouseCursorPosition;
                            }

                            // Allow snapping of gizmo to make it move depending on the selected grid scale
                            if (ActiveRotateSnapping!=RotateSnapping.NONE)
                            {
                                float snapValue = MathHelper.ToRadians(GetRotateSnapValue());
                                if (m_precisionMode)
                                {
                                    delta *= m_precisionModeScale;
                                    snapValue *= m_precisionModeScale;
                                }

                                m_rotationSnapDelta += delta;

                                float snapped = (int)(m_rotationSnapDelta / snapValue) * snapValue;
                                m_rotationSnapDelta -= snapped;
                                delta = snapped;
                            }
                            else if (m_precisionMode)
                            {
                                delta *= m_precisionModeScale;
                            }

                            // rotation matrix to transform - if more than one objects selected, always use world-space.
                            Matrix rot = Matrix.Identity;
                            rot.Forward = m_sceneWorld.Forward;
                            rot.Up = m_sceneWorld.Up;
                            rot.Right = m_sceneWorld.Right;

                            // Create rotation delta matrix
                            if (ActiveAxis == GizmoAxis.X)
                            {
                                rot *= Matrix.CreateFromAxisAngle(Rotation.Right, delta);
                            }
                            else if (ActiveAxis == GizmoAxis.Y)
                            {
                                rot *= Matrix.CreateFromAxisAngle(Rotation.Up, delta);
                            }
                            else if (ActiveAxis == GizmoAxis.Z)
                            {
                                rot *= Matrix.CreateFromAxisAngle(Rotation.Forward, delta);
                            }

                            // store rotation parameters so that we can calculate the difference in rotation of "before" rotation and "after"
                            CalculateYawPitchRollFromRotationDelta(rot);

                            PrepareSafeSelectedEntitiesIterationHelper();

                            List<MyEntity> transformedEntities = new List<MyEntity>();

                            // -- Apply rotation -- //
                            foreach (MyEntity entity in m_safeIterationHelper)
                            {
                                // skip already transformed linked entities
                                if (transformedEntities.Contains(entity))
                                {
                                    continue;
                                }

                                // VoxelMaps cannot be rotated
                                if (entity is MyVoxelMap == false)
                                {
                                    // use gizmo position for all PivotTypes except for object-center, it should use the entity.position instead.
                                    Vector3 newPosition;
                                    Matrix newRotation;
                                    GetRotationAndPosition(entity, rot, out newPosition, out newRotation);


                                    Matrix normalizedMat = new Matrix();
                                    normalizedMat.Right = Vector3.Normalize(newRotation.Right);
                                    normalizedMat.Forward = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newRotation.Up), normalizedMat.Right));
                                    normalizedMat.Up = Vector3.Normalize(Vector3.Cross(normalizedMat.Right, normalizedMat.Forward));
                                    normalizedMat.Translation = newRotation.Translation;
                                    normalizedMat.M44 = newRotation.M44;

                                    //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall("{");
                                    //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(normalizedMat.ToString());
                                    //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(newRotation.ToString());
                                    //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall("}");

                                    MoveAndRotateObject(newPosition, normalizedMat, entity);

                                    MyEditor.Static.FixLinkedEntities(entity, transformedEntities, null);
                                    transformedEntities.Add(entity);
                                }
                            }

                            SetPosition();
                            Vector3 moveVector = m_startObjectPosition - Position;
                            PrepareSafeSelectedEntitiesIterationHelper();
                            transformedEntities = new List<MyEntity>();
                            // apply
                            foreach (MyEntity entity in m_safeIterationHelper)
                            {
                                // skip already transformed linked entities
                                if (transformedEntities.Contains(entity))
                                {
                                    continue;
                                }

                                Vector3 newPosition = entity.GetPosition() + moveVector;

                                MoveAndRotateObject(newPosition, entity.GetWorldRotation(), entity);

                                MyEditor.Static.FixLinkedEntities(entity, transformedEntities, null);
                                transformedEntities.Add(entity);
                            }

                            #endregion
                        }
                    }
                }
                else
                {
                    PrepareSafeSelectedEntitiesIterationHelper();

                    foreach (MyEntity entity in m_safeIterationHelper)
                    {

                        float moveObjectDistanceInMeters = MyEditorGrid.GetGridStepInMeters();
                        float rotationAngleInRadians = MathHelper.ToRadians(GetRotationDelta());
                        Matrix entityOrientation = entity.GetWorldRotation();

                        bool isVoxelMap = entity is MyVoxelMap;

                        Vector3 entityPosition = entity.GetPosition();

                        #region Translate and Rotate with keyboard

                        Vector3 newPosition = entityPosition;
                        Matrix newRotation = entityOrientation;

                        // Keyboard translation
                        if (IsTransformationKeyPressed(input, Keys.NumPad1))
                        {
                            newPosition = GetTransformInAxis(GizmoAxis.X, entityPosition, Rotation, -moveObjectDistanceInMeters);
                        }
                        if (IsTransformationKeyPressed(input, Keys.NumPad3))
                        {
                            newPosition = GetTransformInAxis(GizmoAxis.X, entityPosition, Rotation, +moveObjectDistanceInMeters);
                        }
                        if (IsTransformationKeyPressed(input, Keys.NumPad2))
                        {
                            newPosition = GetTransformInAxis(GizmoAxis.Z, entityPosition, Rotation, +moveObjectDistanceInMeters);
                        }
                        if (IsTransformationKeyPressed(input, Keys.NumPad5))
                        {
                            newPosition = GetTransformInAxis(GizmoAxis.Z, entityPosition, Rotation, -moveObjectDistanceInMeters);

                        }
                        if (IsTransformationKeyPressed(input, Keys.NumPad9))
                        {
                            newPosition = GetTransformInAxis(GizmoAxis.Y, entityPosition, Rotation, +moveObjectDistanceInMeters);
                        }
                        if (IsTransformationKeyPressed(input, Keys.NumPad6))
                        {
                            newPosition = GetTransformInAxis(GizmoAxis.Y, entityPosition, Rotation, -moveObjectDistanceInMeters);
                        }

                        // Keyboard rotation
                        Matrix oneStepRotationDelta = Matrix.Identity;
                        if (IsTransformationKeyPressed(input, Keys.NumPad7) && !isVoxelMap)
                        {
                            oneStepRotationDelta = Matrix.CreateFromAxisAngle(Rotation.Right, rotationAngleInRadians);
                            GetRotationAndPosition(entity, oneStepRotationDelta, out newPosition, out newRotation);
                        }
                        if (IsTransformationKeyPressed(input, Keys.NumPad4) && !isVoxelMap)
                        {
                            oneStepRotationDelta = Matrix.CreateFromAxisAngle(Rotation.Up, rotationAngleInRadians);
                            GetRotationAndPosition(entity, oneStepRotationDelta, out newPosition, out newRotation);
                        }
                        if (IsTransformationKeyPressed(input, Keys.NumPad8) && !isVoxelMap)
                        {
                            oneStepRotationDelta = Matrix.CreateFromAxisAngle(Rotation.Forward, rotationAngleInRadians);
                            GetRotationAndPosition(entity, oneStepRotationDelta, out newPosition, out newRotation);
                        }

                        if (HasTransformationStarted()) StartTransformationData();

                        // Transform
                        bool translated = newPosition != entityPosition;
                        bool rotated = newRotation != entityOrientation;

                        if (translated || rotated)
                        {
                            if (translated) ActiveMode = GizmoMode.TRANSLATE;
                            if (rotated)
                            {
                                ActiveMode = GizmoMode.ROTATE;
                                if (IsDelayForSmoothMovementReached())
                                {
                                    RotationAmountInDegrees += (int)GetRotationDelta();
                                }
                                else
                                {
                                    RotationAmountInDegrees = (int)GetRotationDelta();
                                }
                            }

                            MoveAndRotateObject(newPosition, newRotation, entity);
                        }
                        #endregion
                    }


                    UpdateAxisSelection(MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(MyGuiManager.MouseCursorPosition));

                    /*
                    if (GetSnapPoint(true) == null)
                    {
                        
                    }
                    else
                    {
                        ActiveAxis = GizmoAxis.NONE;
                    }
                    */
                }

                if (HasTransformationEnded()) EndTransformationData();

                // previous is used to detect, when is right time to create undo/redo editor action(start of transform, and end of transform)
                TransformationActivePrevious = TransformationActive;
            }
            else
            {
                ActiveAxis = GizmoAxis.NONE;
            }
        }

        private static float GetRotateSnapValue()
        {
            switch (ActiveRotateSnapping)
            {
                case RotateSnapping.NONE:
                    return 1;
                    break;
                case RotateSnapping.FIVE_DEGREES:
                    return 5;
                    break;
                case RotateSnapping.FIFTEEN_DEGREES:
                    return 15;
                    break;
                case RotateSnapping.FORTYFIVE_DEGREES:
                    return 45;
                    break;
                case RotateSnapping.NINETY_DEGREES:
                    return 90;
                    break;
                default:
                    {
                        System.Diagnostics.Debug.Assert(false);
                        return 1;
                    }
            }
        }

        #endregion

        #region Draw Methods

        /// <summary>
        /// Draw gizmo
        /// </summary>
        public static void Draw()
        {
            if (!MyConfig.EditorUseCameraCrosshair && MyCamera.IsInFrustum(ref Position))
            {
                Device graphics = MyMinerGame.Static.GraphicsDevice;
                m_translationModel.LoadInDraw();
                m_rotationModel.LoadInDraw();

                if (Enabled)
                {

                    #region Draw: Axis-Lines
                    for (int c = 0; c < m_translationLineVertices.Length; c += 2)
                    {
                        MyDebugDraw.DrawLine3D(
                            Vector3.Transform(m_translationLineVertices[c + 0].Position, m_gizmoWorld),
                            Vector3.Transform(m_translationLineVertices[c + 1].Position, m_gizmoWorld),
                            m_translationLineVertices[c + 0].Color,
                            m_translationLineVertices[c + 1].Color);
                    }

                    #endregion


                    if (ActiveMode == GizmoMode.TRANSLATE)
                    {
                        #region Translate
                        RasterizerState.CullNone.Apply();
                        // -- Draw Cones -- //
                        for (int i = 0; i < 3; i++) // 3 = nr. of axis (order: x, y, z)
                        {
                            /*
                            //Vector3 color = m_axisColors[i].ToVector3();
                            Matrix worldMatrix = m_modelLocalSpace[i] * m_gizmoWorld;
                            MyRender.DrawModel(m_translationModel, GetWorldMatrixForDraw(ref worldMatrix), null);
                            */
                            Matrix worldMatrix = m_modelLocalSpace[i] * m_gizmoWorld;
                            //loadn new model from prefab config
                            MyModel mymodel = m_translationModel;

                            //create new Effects
                            MyEffectRenderGizmo effectRenderGizmo = MyRender.GetEffect(MyEffects.Gizmo) as MyEffectRenderGizmo;
                            Matrix view = Matrix.Identity;

                            worldMatrix = GetWorldMatrixForDraw(ref worldMatrix);
                            effectRenderGizmo.SetWorldViewProjectionMatrix(worldMatrix * MyCamera.ViewMatrixAtZero * MyCamera.ProjectionMatrix);

                            Color color = m_axisColors[i];
                            if (i == 0)
                            {
                                if (ActiveAxis == GizmoAxis.X || ActiveAxis == GizmoAxis.XY || ActiveAxis == GizmoAxis.ZX)
                                {
                                    color = m_highlightColor;
                                }
                            }
                            else if (i == 1)
                            {
                                if (ActiveAxis == GizmoAxis.Y || ActiveAxis == GizmoAxis.XY || ActiveAxis == GizmoAxis.YZ)
                                {
                                    color = m_highlightColor;
                                }
                            }
                            else
                            {
                                if (ActiveAxis == GizmoAxis.Z || ActiveAxis == GizmoAxis.YZ || ActiveAxis == GizmoAxis.ZX)
                                {
                                    color = m_highlightColor;
                                }
                            }

                            effectRenderGizmo.SetDiffuseColor(color.ToVector3());
                            //effectRenderGizmo.Apply();


                            MyMinerGame.Static.GraphicsDevice.VertexDeclaration = mymodel.GetVertexDeclaration();
                            MyMinerGame.Static.GraphicsDevice.SetStreamSource(0, mymodel.VertexBuffer, 0, mymodel.GetVertexStride());
                            MyMinerGame.Static.GraphicsDevice.Indices = mymodel.IndexBuffer;
                            effectRenderGizmo.Begin();
                            foreach (MyMesh mesh in mymodel.GetMeshList())
                            {
                                mesh.Render(MyMinerGame.Static.GraphicsDevice, mymodel.GetVerticesCount());
                            }
                            effectRenderGizmo.End();
                        }
                        #endregion
                    }
                    else if (ActiveMode == GizmoMode.ROTATE)
                    {

                        #region Rotation
                        RasterizerState.CullNone.Apply();
                        //draw rottiaons
                        for (int i = 0; i < 3; i++) // 3 = nr. of axis (order: x, y, z)
                        {
                            Matrix worldMatrix = m_modelLocalSpace[i] * m_gizmoWorld;
                            //loadn new model from prefab config
                            MyModel mymodel = m_rotationModel;

                            //create new Effects
                            MyEffectRenderGizmo effectRenderGizmo = MyRender.GetEffect(MyEffects.Gizmo) as MyEffectRenderGizmo;
                            Matrix view = Matrix.Identity;

                            worldMatrix = GetWorldMatrixForDraw(ref worldMatrix);
                            effectRenderGizmo.SetWorldViewProjectionMatrix(worldMatrix * MyCamera.ViewMatrixAtZero * MyCamera.ProjectionMatrix);

                            Color color = m_axisColors[i];
                            if (i == 0)
                            {
                                if (ActiveAxis == GizmoAxis.X || ActiveAxis == GizmoAxis.XY || ActiveAxis == GizmoAxis.ZX)
                                {
                                    color = m_highlightColor;
                                }
                            }
                            else if (i == 1)
                            {
                                if (ActiveAxis == GizmoAxis.Y || ActiveAxis == GizmoAxis.XY || ActiveAxis == GizmoAxis.YZ)
                                {
                                    color = m_highlightColor;
                                }
                            }
                            else
                            {
                                if (ActiveAxis == GizmoAxis.Z || ActiveAxis == GizmoAxis.YZ || ActiveAxis == GizmoAxis.ZX)
                                {
                                    color = m_highlightColor;
                                }
                            }

                            effectRenderGizmo.SetDiffuseColor(color.ToVector3());
                            //effectRenderGizmo.Apply();


                            MyMinerGame.Static.GraphicsDevice.VertexDeclaration = mymodel.GetVertexDeclaration();
                            MyMinerGame.Static.GraphicsDevice.SetStreamSource(0, mymodel.VertexBuffer, 0, mymodel.GetVertexStride());
                            MyMinerGame.Static.GraphicsDevice.Indices = mymodel.IndexBuffer;

                            effectRenderGizmo.Begin();
                            foreach (MyMesh mesh in mymodel.GetMeshList())
                            {
                                mesh.Render(MyMinerGame.Static.GraphicsDevice, mymodel.GetVerticesCount());
                            }
                            effectRenderGizmo.End();

                        }
                        #endregion
                    }

                    #region Draw: Axis-texts


                    MyGuiManager.BeginSpriteBatch();

                    // -- Draw Axis Text ("X","Y","Z") -- //
                    for (int i = 0; i < 3; i++)
                    {
                        Vector3 screenPos = SharpDXHelper.ToXNA(graphics.Viewport.Project(SharpDXHelper.ToSharpDX(m_modelLocalSpace[i].Translation + m_modelLocalSpace[i].Up + m_axisTextOffset), SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(m_gizmoWorld)));

                        Color color = m_axisColors[i];
                        if (i == 0)
                        {
                            if (ActiveAxis == GizmoAxis.X || ActiveAxis == GizmoAxis.XY || ActiveAxis == GizmoAxis.ZX)
                            {
                                color = m_highlightColor;
                            }
                        }
                        else if (i == 1)
                        {
                            if (ActiveAxis == GizmoAxis.Y || ActiveAxis == GizmoAxis.XY || ActiveAxis == GizmoAxis.YZ)
                            {
                                color = m_highlightColor;
                            }
                        }
                        else
                        {
                            if (ActiveAxis == GizmoAxis.Z || ActiveAxis == GizmoAxis.YZ || ActiveAxis == GizmoAxis.ZX)
                            {
                                color = m_highlightColor;
                            }
                        }

                        const float TEXT_SCALE = 0.70f;

                        MyGuiManager.GetFontMinerWarsWhite().DrawString(new Vector2(screenPos.X, screenPos.Y), color, MyTextsWrapper.Get(m_axisTexts[i]), TEXT_SCALE);
                    }

                    MyGuiManager.EndSpriteBatch();

                    #endregion
                }
            }
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Main update
        /// </summary>
        public static void Update()
        {
            //if (SelectedEntities.Count > 0)
            //{
            //    var x = SelectedEntities[6];

            //    SelectedEntities.Clear();

            //    SelectedEntities.Add(x);
            //}

            //if (SelectedEntities.Count == 1)
            //{
            //    var dummy = SelectedEntities[0] as MyDummyPoint;
            //    if (dummy != null)
            //    {
            //        SelectedEntities[0].SetPosition(new Vector3(5423, -952, 4686));
            //    }
            //}

            if (!Enabled)
                return;

            // scale for mouse.delta
            m_inputScale = (float)MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000;

            SetActivePivot();
            SetPosition();


            TranslationSnapValue = MyEditorGrid.GetGridStepInMeters();
            RotationSnapValue = GetRotationDelta();

            // -- Scale Gizmo to fit on-screen -- //
            Vector3 vLength = MyCamera.Position - Position;
            float scaleFactor = 25;

            m_screenScale = vLength.Length() / scaleFactor;
            m_screenScaleMatrix = Matrix.CreateScale(new Vector3(m_screenScale));

            var worldMatrix = GetWorldMatrix();
            m_localForward = worldMatrix.Forward;
            m_localUp = worldMatrix.Up;
            m_localRight = worldMatrix.Right;
            //// -- Vector Rotation (Local/World) -- //
            //m_localForward = MyMwcUtils.Normalize(m_localForward);
            //m_localRight = Vector3.Cross(m_localForward, m_localUp);
            //m_localUp = Vector3.Cross(m_localRight, m_localForward);
            //m_localRight = MyMwcUtils.Normalize(m_localRight);
            //m_localUp = MyMwcUtils.Normalize(m_localUp);

            m_objectOrientedWorld = m_screenScaleMatrix * Matrix.CreateWorld(Position, m_localForward, m_localUp);
            m_axisAlignedWorld = m_screenScaleMatrix * Matrix.CreateWorld(Position, m_sceneWorld.Forward, m_sceneWorld.Up);

            // Assign World
            if (ActiveSpace == TransformSpace.WORLD ||
                (ActiveMode == GizmoMode.ROTATE && SelectedEntities.Count > 1))
            {
                m_gizmoWorld = m_axisAlignedWorld;

                // align lines, boxes etc. with the grid-lines
                Rotation.Forward = m_sceneWorld.Forward;
                Rotation.Up = m_sceneWorld.Up;
                Rotation.Right = m_sceneWorld.Right;
            }
            else
            {
                m_gizmoWorld = m_objectOrientedWorld;

                // align lines, boxes etc. with the selected object
                Rotation.Forward = m_localForward;
                Rotation.Up = m_localUp;
                Rotation.Right = m_localRight;
            }

            // -- Reset Colors to default -- //
            ApplyColor(GizmoAxis.X, m_axisColors[0]);
            ApplyColor(GizmoAxis.Y, m_axisColors[1]);
            ApplyColor(GizmoAxis.Z, m_axisColors[2]);

            // -- Apply Highlight -- //
            ApplyColor(ActiveAxis, m_highlightColor);

            if (IsRotationActive())
            {

                float value = GetHigherAbsValue(m_deltaPitch, m_deltaYaw);
                value = GetHigherAbsValue(value, m_deltaRoll);
                RotationAmountInDegrees = (int)value;
            }
            else
            {
                m_deltaPitch = 0;
                m_deltaYaw = 0;
                m_deltaRoll = 0;
            }
        }

        /// <summary>
        /// Helper method for applying color to the gizmo lines.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="color"></param>
        static void ApplyColor(GizmoAxis axis, Color color)
        {
            if (ActiveMode == GizmoMode.TRANSLATE)
            {
                switch (axis)
                {
                    case GizmoAxis.X:
                        ApplyLineColor(0, 6, color);
                        break;
                    case GizmoAxis.Y:
                        ApplyLineColor(6, 6, color);
                        break;
                    case GizmoAxis.Z:
                        ApplyLineColor(12, 6, color);
                        break;
                    case GizmoAxis.XY:
                        ApplyLineColor(0, 4, color);
                        ApplyLineColor(6, 4, color);
                        break;
                    case GizmoAxis.YZ:
                        ApplyLineColor(6, 2, color);
                        ApplyLineColor(12, 2, color);
                        ApplyLineColor(10, 2, color);
                        ApplyLineColor(16, 2, color);
                        break;
                    case GizmoAxis.ZX:
                        ApplyLineColor(0, 2, color);
                        ApplyLineColor(4, 2, color);
                        ApplyLineColor(12, 4, color);
                        break;
                }
            }
            else if (ActiveMode == GizmoMode.ROTATE)
            {
                switch (axis)
                {
                    case GizmoAxis.X:
                        ApplyLineColor(0, 6, color);
                        break;
                    case GizmoAxis.Y:
                        ApplyLineColor(6, 6, color);
                        break;
                    case GizmoAxis.Z:
                        ApplyLineColor(12, 6, color);
                        break;
                }
            }
        }

        /// <summary>
        /// Apply color on the lines associated with translation mode (re-used in Scale)
        /// </summary>
        /// <param name="startindex"></param>
        /// <param name="count"></param>
        /// <param name="color"></param>
        static void ApplyLineColor(int startindex, int count, Color color)
        {
            for (int i = startindex; i < (startindex + count); i++)
            {
                m_translationLineVertices[i].Color = color;
            }
        }

        /// <summary>
        /// Per-frame check to see if mouse is hovering over any axis.
        /// </summary>
        /// <param name="mousePosition"></param>
        static void UpdateAxisSelection(Vector2 mousePosition)
        {
            float closestintersection = float.MaxValue;
            MinerWarsMath.Ray ray = MyUtils.ConvertMouseToRay();

            closestintersection = float.MaxValue;
            float? intersection;

            if (ActiveMode == GizmoMode.TRANSLATE)
            {
                #region BoundingBoxes

                intersection = XY_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.XY;
                        closestintersection = intersection.Value;
                    }
                }
                intersection = XZ_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.ZX;
                        closestintersection = intersection.Value;
                    }
                }
                intersection = YZ_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.YZ;
                        closestintersection = intersection.Value;
                    }
                }

                intersection = X_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.X;
                        closestintersection = intersection.Value;
                    }
                }
                intersection = Y_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.Y;
                        closestintersection = intersection.Value;
                    }
                }
                intersection = Z_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.Z;
                        closestintersection = intersection.Value;
                    }
                }

                #endregion
            }

            if (ActiveMode == GizmoMode.ROTATE || ActiveMode == GizmoMode.TRANSLATE)
            {
                #region BoundingSpheres

                intersection = X_Sphere.Intersects(ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.X;
                        closestintersection = intersection.Value;
                    }
                }
                intersection = Y_Sphere.Intersects(ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.Y;
                        closestintersection = intersection.Value;
                    }
                }
                intersection = Z_Sphere.Intersects(ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.Z;
                        closestintersection = intersection.Value;
                    }
                }


                #endregion
            }

            if (ActiveMode == GizmoMode.ROTATE)
            {
                #region X,Y,Z Boxes
                intersection = X_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.X;
                        closestintersection = intersection.Value;
                    }
                }
                intersection = Y_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.Y;
                        closestintersection = intersection.Value;
                    }
                }
                intersection = Z_Box.Intersects(ref ray);
                if (intersection.HasValue)
                {
                    if (intersection.Value < closestintersection)
                    {
                        ActiveAxis = GizmoAxis.Z;
                        closestintersection = intersection.Value;
                    }
                }
                #endregion
            }

            if (closestintersection == float.MaxValue)
            {
                ActiveAxis = GizmoAxis.NONE;
            }
        }

        /// <summary>
        /// Probably temporary - we will want to have possibility to choose pivot type by some key maybe?
        /// </summary>
        static void SetActivePivot()
        {
            //Debug.Assert(!(SelectedSnapPoint != null && SelectedEntities.Count != 0));

            if (SelectedEntities.Count == 1 || SelectedSnapPoint != null)
            {
                ActivePivot = PivotType.OBJECT_CENTER;
                if (MyEditor.EnableObjectPivot)
                {
                    ActivePivot = PivotType.OBJECT_CENTER;
                }
                else
                {
                    ActivePivot = PivotType.MODEL_CENTER;
                }
            }
            else if (SelectedEntities.Count > 1)
            {
                ActivePivot = PivotType.SELECTION_CENTER;
            }
        }

        /// <summary>
        /// Set position of the gizmo, position will be center of all selected entities.
        /// </summary>
        static void SetPosition()
        {
            switch (ActivePivot)
            {
                case PivotType.MODEL_CENTER:
                    {
                        if (SelectedEntities.Count > 0 || SelectedSnapPoint != null)
                        {
                            Position = GetWorldMatrix().Translation;
                        }

                    }
                    break;
                case PivotType.OBJECT_CENTER:
                    {
                        if (SelectedEntities.Count > 0)
                        {
                            Position = Vector3.Transform(SelectedEntities[0].LocalAABB.GetCenter(), GetWorldMatrix());
                        }
                        else if (SelectedSnapPoint != null)
                        {
                            Position = GetWorldMatrix().Translation;
                        }
                    }
                    break;
                case PivotType.SELECTION_CENTER:
                    {
                        Position = GetSelectedObjectsCenter();
                    }
                    break;
            }
        }

        #endregion

        #region Selection Methods

        /// <summary>
        /// Prepare safe selected entities iteration helper
        /// </summary>
        static void PrepareSafeSelectedEntitiesIterationHelper()
        {
            m_safeIterationHelper.Clear();
            if (SelectedSnapPoint != null)
            {
                m_safeIterationHelper.Add(SelectedSnapPoint.Prefab);
            }
            else
            {
                foreach (MyEntity entity in SelectedEntities)
                {
                    m_safeIterationHelper.Add(entity);
                }
            }
        }

        /// <summary>
        /// Add entities to selection
        /// </summary>
        /// <param name="entities"></param>
        public static void AddEntitiesToSelection(IEnumerable<MyEntity> entities)
        {            
            foreach (MyEntity entity in entities)
            {
                AddEntityToSelection(entity);
            }            

            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectSelect);
        }

        // check if some entities are intersecting
        public static void CheckCollisions(IEnumerable<MyEntity> entities)
        {
            // Compute AABB of all selected objects
            BoundingBox aabb;
            aabb.Min = new Vector3(1e38f);
            aabb.Max = new Vector3(-1e38f);

            foreach (MyEntity entity in entities)
            {
                if (entity is MyPrefab)
                {
                    BoundingBoxHelper.AddBBox(entity.WorldAABB, ref aabb);
                }
            }

            // Get all elements from AABB
            List<MyRBElement> allRBelements = new List<MyRBElement>();
            MyEntities.GetCollisionsInBoundingBox(ref aabb, allRBelements);

            MyEditor.Static.ClearCollidingElements();

            // Check non prefab elements for collision
            foreach (MyRBElement worldCollidingElement in allRBelements)
            {
                MyEntity worldCollidingEntity = ((MyPhysicsBody)worldCollidingElement.GetRigidBody().m_UserData).Entity;

                if (worldCollidingEntity is MyPrefab)
                    continue;

                MyEditor.Static.CheckAllCollidingObjectsForEntity(worldCollidingEntity);
            }
        }

        private static void UnregistreEntityFromUpdateRecursive(MyEntity entity)
        {
            foreach (MyEntity child in entity.Children)
            {
                UnregistreEntityFromUpdateRecursive(child);
            }
            MyEntities.UnregisterForUpdate(entity);
        }

        /// <summary>
        /// Adds entity to selection
        /// </summary>
        /// <param name="entity"></param>
        public static void AddEntityToSelection(MyEntity entity)
        {
            ////if animated - disable animation
            //MyEntities.RegisterForStopAnimations(entity);
            if (!(entity is MinerWars.AppCode.Game.Entities.InfluenceSpheres.MyInfluenceSphere))
            {
                UnregistreEntityFromUpdateRecursive(entity);
            }
            

            if (MyFakes.POST_MYENTITYANIMATOR_VALUES)
            {
                Quaternion rotation = Quaternion.CreateFromRotationMatrix(entity.WorldMatrix);
                Debug.WriteLine(string.Format("animator.AddKey(,new Vector3({0:0.0##}f, {1:0.0##}f, {2:0.0##}f), new Quaternion({3:0.0##}f, {4:0.0##}f, {5:0.0##}f, {6:0.0##}f));",
                    entity.WorldMatrix.Translation.X, entity.WorldMatrix.Translation.Y, entity.WorldMatrix.Translation.Z,
                    rotation.X, rotation.Y, rotation.Z, rotation.W));
            }

            // disable selecting another prefabs or object except ones in prefab container
            if (MyEditor.Static.GetEditedPrefabContainer() != null)
            {
                MyPrefabContainer selectedPrefabContainer = null;
                MyEntity selectedEntity = entity;

                while (selectedEntity != null)
                {
                    selectedPrefabContainer = selectedEntity.Parent as MyPrefabContainer;
                    if (selectedPrefabContainer != null)
                        break;

                    selectedEntity = selectedEntity.Parent;
                }

                if (selectedPrefabContainer != MyEditor.Static.GetEditedPrefabContainer())
                    return;
            }

            if (entity == MyEditor.Static.GetEditedPrefabContainer()) return;

            if (SelectedSnapPoint != null) return;

            entity = MySelectionTool.GetSelectableEntity(entity);
            
            if (entity != null)
            {
                /*
                if (entity is MyPrefabLargeWeapon)
                {
                    ((MyPrefabLargeWeapon)entity).GetGun().ForceDeactivate();
                    ((MyPrefabLargeWeapon)entity).GetGun().HighlightEntity(ref MyEditorConstants.SELECTED_OBJECT_DIFFUSE_COLOR_ADDITION);
                    entity.HighlightEntity(ref MyEditorConstants.SELECTED_OBJECT_DIFFUSE_COLOR_ADDITION);
                }  */                               
                
                //@ get selectable entity
                if (SelectedEntities.Contains(entity) == false)
                {                    
                    //@ hack coz prefab containers
                    if (MyEditor.Static.GetEditedPrefabContainer() != null && MyEditor.Static.GetEditedPrefabContainer().HasEditState())
                        MyEditor.Static.GetEditedPrefabContainer().UpdateEditedNode(entity);                    

                    SelectedEntities.Add(entity);                    
                }                

                if (entity is MyVoxelMap) SwitchGizmoMode();

                entity.HighlightEntity(ref MyEditorConstants.SELECTED_OBJECT_DIFFUSE_COLOR_ADDITION);                
            }
        }

        /// <summary>
        /// Remove multiple entities from selection
        /// </summary>
        /// <param name="entities"></param>
        public static void RemoveEntitiesFromSelection(List<MyEntity> entities)
        {
            foreach (MyEntity entity in entities)
            {
                RemoveEntityFromSelection(entity);
            }            
            // play sound:
            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectSelect);
        }

        /// <summary>
        /// Removes entity from selection
        /// </summary>
        /// <param name="entity"></param>
        public static void RemoveEntityFromSelection(MyEntity entity)
        {
            ////enable object again
            //MyEntities.UnregisterForStopAnimations(entity);
            if (!(entity is MinerWars.AppCode.Game.Entities.InfluenceSpheres.MyInfluenceSphere))
            {
                MyEntities.RegisterForUpdate(entity);
            }

            // first try to remove the entity on its own (the entity might have become non-selectable)
            if (SelectedEntities.Contains(entity) == true)
            {
                SelectedEntities.Remove(entity);
                entity.ClearHighlightning();
            }

            // do selectable entities
            entity = MySelectionTool.GetSelectableEntity(entity);

            if (entity != null)
            {
                if (SelectedEntities.Contains(entity) == true)
                {                    
                    SelectedEntities.Remove(entity);
                    entity.ClearHighlightning();

                    /*
                    if (entity is MyPrefabLargeWeapon)
                    {
                        ((MyPrefabLargeWeapon)entity).GetGun().ClearHighlightning();
                    } */                                                       
                }

            }
        }

        /// <summary>
        /// Resets rotation of selected entities
        /// </summary>
        public static void ResetSelectedRotation()
        {
            Matrix rot = Matrix.Invert(Rotation);
            
            PrepareSafeSelectedEntitiesIterationHelper();

            List<MyEntity> transformedEntities = new List<MyEntity>();

            // -- Apply rotation -- //
            foreach (MyEntity entity in m_safeIterationHelper)
            {
                // skip already transformed linked entities
                if (transformedEntities.Contains(entity))
                {
                    continue;
                }

                // VoxelMaps cannot be rotated
                if (entity is MyVoxelMap == false)
                {
                    // use gizmo position for all PivotTypes except for object-center, it should use the entity.position instead.
                    Vector3 newPosition;
                    Matrix newRotation;
                    GetRotationAndPosition(entity, rot, out newPosition, out newRotation);
                    MoveAndRotateObject(newPosition, newRotation, entity);

                    MyEditor.Static.FixLinkedEntities(entity, transformedEntities, null);
                    transformedEntities.Add(entity);
                }
            } 
        }

        /// <summary>
        /// Clears gizmo selection
        /// </summary>
        public static void ClearSelection()
        {
            if (SelectedEntities.Count != 0)
            {
                for (int ci = SelectedEntities.Count; ci-- != 0; )
                {
                    /*
                    if (SelectedEntities[ci] is MyPrefabLargeWeapon)
                    {
                        MyLargeShipGunBase weapon = ((MyPrefabLargeWeapon)SelectedEntities[ci]).GetGun();
                        if (weapon != null)
                        {
                            weapon.ForceActivate();
                        }
                    } */                        
                    RemoveEntityFromSelection(SelectedEntities[ci]);                        
                }
            }                
            // play sound:
            if (MyGuiScreenGamePlay.Static != null && !MyGuiScreenGamePlay.Static.IsGameActive())
                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectSelect);

            // unselect waypoint paths
            MyWayPointGraph.SelectedPath = null;
        }

        /// <summary>
        /// Get center position of currently selected objects
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetSelectedObjectsCenter()
        {
            PrepareSafeSelectedEntitiesIterationHelper();
            return MyEntities.GetEntitiesCenter(m_safeIterationHelper);
        }

        /// <summary>
        /// This method can be usefull sometimes, when we know that there is only one selected object at the moment
        /// </summary>
        /// <returns></returns>
        public static MyEntity GetFirstSelected()
        {
            if (SelectedEntities.Count > 0)
            {
                foreach (MyEntity entity in SelectedEntities)
                {
                    return entity;
                }
            }
            return null;
        }

        static int snapPointSelectionCounter = 0;
        /// <summary>
        /// Get selected prefab snap point
        /// </summary>
        public static MyPrefabSnapPoint GetSnapPoint(bool getFirst)
        {
            if (!MyEditor.Static.ShowSnapPoints)
            {
                return null;
            }

            List<MyPrefabSnapPoint> snapPoints = new List<MyPrefabSnapPoint>();
            MyPrefabSnapPoint selectedSnapPoint = null;
            Vector2 mouseScreenPosition = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(MyGuiManager.MouseCursorPosition);
            var snapSizeSquared = MyPrefabSnapPoint.GetFixedSnapSize() * MyPrefabSnapPoint.GetFixedSnapSize();
            Ray ray = MyUtils.ConvertMouseToRay();

            var container = MyEditor.Static.GetEditedPrefabContainer();
            if (container != null)
            {
                foreach (var child in container.Children)
                {
                    var prefab = child as MyPrefabBase;
                    if (prefab != null)
                    {
                        foreach (var snapPoint in prefab.SnapPoints)
                        {
                            // Forbid selection of snap points which are invisible or can't be attached
                            if (!snapPoint.Visible || 
                                (SelectedSnapPoint != null && SelectedSnapPoint != snapPoint && !SelectedSnapPoint.CanAttachTo(snapPoint)))
                            {
                                continue;
                            }

                            bool hitTest = false;
                            if (MyEditor.Static.FixedSizeSnapPoints)
                            {
                                Vector3 proj = SharpDXHelper.ToXNA(MyMinerGame.Static.GraphicsDevice.Viewport.Project(SharpDXHelper.ToSharpDX(snapPoint.Matrix.Translation),
                                    SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix),
                                    SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(prefab.WorldMatrix)));
                                Vector2 screenCoord = new Vector2(proj.X, proj.Y);

                                hitTest = (mouseScreenPosition - screenCoord).LengthSquared() < snapSizeSquared;
                            }
                            else
                            {
                                var snapPointPosition = (snapPoint.Matrix * snapPoint.Prefab.WorldMatrix).Translation;
                                var hit = ray.Intersects(new BoundingSphere(snapPointPosition, MyPrefabSnapPoint.GetRealSnapSize()));
                                hitTest = hit.HasValue;
                            }

                            if (hitTest)
                            {
                                if (getFirst)
                                {
                                    return snapPoint;
                                }
                                snapPoints.Add(snapPoint);
                            }
                        }
                    }
                }

                snapPoints.Sort((x, y) =>
                    (MyCamera.Position - Vector3.Transform(x.Matrix.Translation, x.Prefab.WorldMatrix)).LengthSquared().CompareTo(
                    (MyCamera.Position - Vector3.Transform(y.Matrix.Translation, y.Prefab.WorldMatrix)).LengthSquared()));

                ++snapPointSelectionCounter;
                selectedSnapPoint = snapPoints.Count > 0 ? snapPoints[snapPointSelectionCounter % snapPoints.Count] : null;
            }

            if (selectedSnapPoint == null)
            {
                snapPointSelectionCounter = 0;
            }

            return selectedSnapPoint;
        }

        /// <summary>
        /// Selects objects under mouse cursor
        /// </summary>
        public static void SelectByIntersectionLine(MyGuiInput input)
        {
            List<MyEntity> beforeSelectionObjects = SelectedEntities;
            MyEntity selectedNow = MyEntities.GetEntityUnderMouseCursor();

            //TODO solve better
            if (selectedNow != null && selectedNow.Parent is MyPrefabBase && !(selectedNow is MyWayPoint))
            {
                selectedNow = selectedNow.Parent;
            }

            //while transforming objects with mouse(using gizmo), do not deselect current selection
            if (TransformationActive == false && ActiveAxis == GizmoAxis.NONE  && !(MyGuiScreenGamePlay.Static.IsIngameEditorActive() && MyEditor.Static.GetEditedPrefabContainer() == null))
            {
                
                //Get selected prefab snap point
                // IMPORTANT: first assign snap point to local field because setter affects highlighting (ClearSelection clears highlight)
                var snapPoint = GetSnapPoint(false);
                if (snapPoint != null)
                {
                    ClearSelection();
                    SelectedSnapPoint = snapPoint;
                    return;
                }
                else
                {
                    SelectedSnapPoint = snapPoint;
                }
                
                //if nothing is selected now, clear selection
                if (selectedNow == null && !input.IsAnyCtrlKeyPressed() && !input.IsAnyShiftKeyPressed())
                {
                    ClearSelection();
                }
                else
                {
                    //if we are in conteiner editing  dont allow changes to selection outside the prefab container
                    //allow selection of container if exit is allowed
                    if (MyEditor.Static.IsEditingPrefabContainer())
                    {
                        if (selectedNow is MyPrefabBase)
                        {
                            if ((selectedNow as MyPrefabBase).Parent != MyEditor.Static.GetEditedPrefabContainer())
                            {
                                return;
                            }
                        }
                        else if (!(MyEditor.Static.GetEditedPrefabContainer() == selectedNow && !MyGuiScreenGamePlay.Static.IsIngameEditorActive()))
                        {
                            return;
                        }
                    }

                    // if CTRL isn't held, clear selection (otherwise add/remove from selection)
                    if (input.IsAnyControlPress() == false && input.IsAnyShiftPress() == false)
                    {
                        // click a waypoint multiple times: cycle among paths
                        if (selectedNow is MyWayPoint && beforeSelectionObjects.Contains(selectedNow))
                        {
                            var pathsContainingSelectedWayPoint = (selectedNow as MyWayPoint).Paths();
                            int indexOfLastPath = pathsContainingSelectedWayPoint.IndexOf(MyWayPointGraph.SelectedPath);
                            
                            ClearSelection();

                            if (pathsContainingSelectedWayPoint.Count == 0)
                            {
                                MyWayPointGraph.SelectedPath = null;
                            }
                            else
                            {
                                // select next path (or, if it was the last one, select just the vertex)
                                if (indexOfLastPath == pathsContainingSelectedWayPoint.Count - 1)
                                    MyWayPointGraph.SelectedPath = null;
                                else if (indexOfLastPath == -1)
                                    MyWayPointGraph.SelectedPath = pathsContainingSelectedWayPoint[0];
                                else
                                    MyWayPointGraph.SelectedPath = pathsContainingSelectedWayPoint[indexOfLastPath + 1];
                            }

                            if (MyWayPointGraph.SelectedPath != null)
                            {
                                // a path is selected: select waypoints from path in the correct order
                                // the clicked waypoint will try to be added again later, but will be prevented from it
                                AddEntitiesToSelection(MyWayPointGraph.SelectedPath.WayPoints);
                            }
                        }
                        else
                        {
                            ClearSelection();
                        }
                    }

                    //When editing container(selecting prefabs inside it), it should not be possible to select any objects outside container
                    if (MyEditor.Static.IsEditingPrefabContainer())
                    {
                        //Allow selection of objects inside container, but disallow all outside
                        BoundingSphere vol = selectedNow.WorldVolume;
                        if (MyEditor.Static.GetEditedPrefabContainer().GetIntersectionWithMaximumBoundingBox(ref vol) == false && (selectedNow is MyPrefabBase == false))
                        {
                            selectedNow = null;
                        }

                        // Allow selection of edited container only if exit editing mode action is allowed
                        if (selectedNow == MyEditor.Static.GetEditedPrefabContainer())
                        {
                            MyEditor.Static.ExitActivePrefabContainer();
                            AddEntityToSelection(selectedNow);
                            return;
                        }
                    }


                    // If selected item is child of the prefab and is not a prefab switch selection to the parent prefab:
                    // for weapon case:
                    if (selectedNow is MyLargeShipGunBase || selectedNow is MyLargeShipBarrelBase)
                    {
                        //selectedNow = ((MyLargeShipGunBase)selectedNow).PrefabParent;
                        selectedNow = MySelectionTool.GetSelectableEntity(selectedNow);
                    }

                    // When selected object is prefab, switch to edit mode of prefab container immediately
                    if (selectedNow is MyPrefabBase)
                    {
                        MyPrefabContainer parentContainer = (MyPrefabContainer)selectedNow.Parent;
                        MyEditor.Static.EditPrefabContainer(parentContainer);
                    }                    

                    // CTRL held: add/remove entities to/from selection
                    if (input.IsAnyControlPress() == true)
                    {
                        if (selectedNow != null)
                        {
                            if (IsEntitySelected(selectedNow))
                            {
                                RemoveEntityFromSelection(selectedNow);
                            }
                            else
                            {
                                AddEntityToSelection(selectedNow);
                            }

                            // add/remove waypoints from selected waypoint path
                            MyWayPointGraph.UpdateSelectedPath();
                        }
                    }
                    else if (!input.IsAnyShiftPress())
                    {
                        if (selectedNow != null)
                        {
                            AddEntityToSelection(selectedNow);
                            /*
                            if (selectedNow is MyPrefabLargeWeapon)
                            {
                                AddEntityToSelection(((MyPrefabLargeWeapon)selectedNow).GetGun());
                            } */
                        }
                    }
                }
                // play sound:
                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectSelect);
            }


            //There is new selection made compared to previous selection, reload control buttons
            if ((beforeSelectionObjects.Count > 0 && beforeSelectionObjects.Contains(selectedNow) == false) ||
                (beforeSelectionObjects.Count == 0 && selectedNow != null) ||
                (beforeSelectionObjects.Count > 0 && selectedNow == null))
            {
                MyEditor.Static.ReloadEditorControls = true;
            }
        }

        #endregion

        #region Move And Rotate Methods

        /// <summary>
        /// HasTransformationStarted
        /// </summary>
        /// <returns></returns>
        static bool HasTransformationStarted()
        {
            return TransformationActivePrevious == false && TransformationActive == true;
        }

        /// <summary>
        /// HasTransformationEnded
        /// </summary>
        /// <returns></returns>
        static bool HasTransformationEnded()
        {
            return TransformationActivePrevious == true && TransformationActive == false;
        }

        /// <summary>
        /// When transformation starts, prepare transformation action data
        /// </summary>
        public static void StartTransformationData()
        {

            m_gizmoStartRotation = m_gizmoWorld;

            m_transformAction = new MyEditorActionEntityTransform();
            foreach (MyEntity entity in SelectedEntities)
            {
                MyEditorTransformData data = new MyEditorTransformData(entity, entity.GetPosition(), entity.GetWorldRotation());
                m_transformAction.AddStartData(data);
            }
        }

        /// <summary>
        /// When transformation finished, register and do transformation action
        /// </summary>
        public static void EndTransformationData()
        {
            if (m_transformAction != null)
            {
                Vector3? snapMovement = null;
                foreach (MyEntity entity in SelectedEntities)
                {
                    Matrix rotation = entity.GetWorldRotation();

                    if (SnapEnabled)
                    {
                        Vector3 pos = entity.GetPosition();

                        if (snapMovement == null) // We snap only first entities
                        {
                            Vector3 oldPosition = pos;
                            SnapToGrid(ref pos, TranslationSnapValue);
                            snapMovement = pos - oldPosition;
                        }
                        else // We move other entites based only on movement of first entity
                        {
                            pos += snapMovement.Value;
                        }



                        if (!MyEditor.Static.IsLinked(entity))
                        {
                            entity.MoveAndRotate(pos, rotation);
                        }
                        else if (SelectedEntities.Count == 1)
                        {
                            List<MyEntity> temp = new List<MyEntity>();
                            entity.MoveAndRotate(pos, rotation);
                            MyEditor.Static.FixLinkedEntities(entity, temp, null);
                        }
                    }

                    MyEditorTransformData data = new MyEditorTransformData(entity, entity.GetPosition(), entity.GetWorldRotation());
                    m_transformAction.AddEndData(data);
                }

                m_transformAction.RegisterAndDoAction();
                m_transformAction = null;
            }
            m_gizmoStartRotation = null;
        }



        /// <summary>
        /// Calculates yaw, pitch, roll by provided rotation delta matrix
        /// </summary>
        /// <param name="rotationDelta"></param>
        static void CalculateYawPitchRollFromRotationDelta(Matrix rotationDelta)
        {
            float yaw = 0;
            float pitch = 0;
            float roll = 0;

            MyUtils.RotationMatrixToYawPitchRoll(ref rotationDelta, out yaw, out pitch, out roll);

            // store rotation parameters so that we can calculate the difference in rotation of "before" rotation and "after"
            m_deltaYaw += MathHelper.ToDegrees(yaw);
            m_deltaPitch += MathHelper.ToDegrees(pitch);
            m_deltaRoll += MathHelper.ToDegrees(roll);
        }

        /// <summary>
        /// Get delta vector transformation around provided rotation matrix
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        static Vector3 GetTransformInAxis(GizmoAxis axis, Vector3 position, Matrix rotation, float delta)
        {
            Vector3 deltaVec = Vector3.Zero;
            if (axis == GizmoAxis.X)
            {
                deltaVec = new Vector3(delta, 0, 0);
                deltaVec = Vector3.Transform(deltaVec, rotation);
            }
            else if (axis == GizmoAxis.Y)
            {
                deltaVec = new Vector3(0, delta, 0);
                deltaVec = Vector3.Transform(deltaVec, rotation);
            }
            else if (axis == GizmoAxis.Z)
            {
                deltaVec = new Vector3(0, 0, delta);
                deltaVec = Vector3.Transform(deltaVec, rotation);
            }

            position += deltaVec;

            return position;
        }

        /// <summary>
        /// This method calculates new object position when its being rotated - in case only one object is selected and rotated
        /// only rotation is changed, but for multiple selected object, rotation is calculated based on the objects selected center
        /// </summary>
        static void GetRotationAndPosition(MyEntity entity, Matrix rotation, out Vector3 newPosition, out Matrix newRotation)
        {
            Matrix entityOrientation = entity.GetWorldRotation();
            Vector3 entityPosition = entity.GetPosition();
            Vector3 gizmoPosition = Position;

            if (ActivePivot == PivotType.OBJECT_CENTER)
            {
                //gizmoPosition = entityPosition;
            }

            Matrix localRot = Matrix.Identity;
            localRot.Forward = entityOrientation.Forward;
            localRot.Up = entityOrientation.Up;
            localRot.Right = entityOrientation.Right;
            m_localRight = MyMwcUtils.Normalize(m_localRight);
            localRot.Translation = entityPosition - gizmoPosition;

            Matrix rot = localRot * rotation;
            newPosition = rot.Translation + gizmoPosition;
            entityOrientation.Forward = rot.Forward;
            entityOrientation.Up = rot.Up;
            entityOrientation.Right = rot.Right;
            entityOrientation.Translation = Vector3.Zero;
            newRotation = entityOrientation;
        }

        /// <summary>
        /// Moves and Rotates entity
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="newOrientation"></param>
        /// <param name="entity"></param>
        public static void MoveAndRotateObject(Vector3 newPosition, Matrix newOrientation, MyEntity entity)
        {
            BoundingSphere sphere = new BoundingSphere(newPosition + entity.LocalVolumeOffset, entity.WorldVolume.Radius);

            if (MyMwcSectorConstants.SECTOR_SIZE_FOR_PHYS_OBJECTS_BOUNDING_BOX.Contains(sphere) == MinerWarsMath.ContainmentType.Contains)
            {
                Matrix entityBeforeMoveOrientation = entity.GetWorldRotation();
                Vector3 entityBeforeMovePosition = entity.GetPosition();

                if (entityBeforeMovePosition != newPosition || entityBeforeMoveOrientation != newOrientation)
                {
                    // Move object
                    bool moveSuccesfull = entity.MoveAndRotate(newPosition, newOrientation);
                    Vector3 currentPosition = entity.GetPosition();
                    Matrix currentOrientation = entity.GetWorldRotation();

                    if (moveSuccesfull)
                    {
                        //entity.UpdateWorldMatrix();
                        //  Play movement or rotation sounds
                        if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_delayMovementSoundInMillis)
                        {
                            if (entityBeforeMovePosition != currentPosition)
                            {
                                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveStep);
                            }

                            if (entityBeforeMoveOrientation != currentOrientation)
                            {
                                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectRotateStep);
                            }

                            m_delayMovementSoundInMillis = MyMinerGame.TotalGamePlayTimeInMilliseconds + MyEditorConstants.DELAY_OBJECT_MOVEMENT_SOUND_IN_MILLIS;
                        }

                        MyEditor.Static.RecheckAllColidingEntitesAndClearNonColiding();//check all coliding entites to see if any went out of collision state
                        
                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CheckAllCollidingObjectsForEntity");

                        // This was taking up to 170ms for 800 prefabs, so its replaced by void CheckCollisions(IEnumerable<MyEntity> entities) in HandleInput().
                        //MyEditor.Static.CheckAllCollidingObjectsForEntity(entity);//check all colliding state of entity after move

                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock(); 
                    }
                    else
                    {
                        if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_delayMovementSoundInMillis)
                        {
                            if ((entityBeforeMovePosition != currentPosition) || (entityBeforeMoveOrientation != currentOrientation))
                            {
                                MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorObjectMoveInvalid);
                                m_delayMovementSoundInMillis = MyMinerGame.TotalGamePlayTimeInMilliseconds + 1000;
                            }
                        }
                    }

                }
            }
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Checks if entity or its parent is selected
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsEntityOrItsParentSelected(MyEntity entity)
        {
            MyEntity actualentity = entity;
            while (actualentity != null)
            {
                if (SelectedEntities.Contains(actualentity))
                {
                    return true;
                }
                actualentity = actualentity.Parent;
            }
            return false;
        }

        /// <summary>
        /// Checks if entity is selected
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsEntitySelected(MyEntity entity)
        {
            if (SelectedEntities.Contains(entity) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Is only one entity selected
        /// </summary>
        /// <returns></returns>
        public static bool IsOnlyOneEntitySelected()
        {
            return SelectedEntities.Count == 1;
        }

        /// <summary>
        /// Is any entity selected
        /// </summary>
        /// <returns></returns>
        public static bool IsAnyEntitySelected()
        {
            return SelectedEntities != null && SelectedEntities.Count > 0;
        }

        /// <summary>
        /// Is more than one entity selected
        /// </summary>
        /// <returns></returns>
        public static bool IsMoreThanOneEntitySelected()
        {
            return SelectedEntities != null && SelectedEntities.Count > 1;
        }

        /// <summary>
        /// Is only one entity type selected
        /// </summary>
        /// <returns></returns>
        public static bool IsOnlyOneEntityTypeSelected() 
        {            
            if (IsAnyEntitySelected()) 
            {
                Type selectedType = null;
                foreach (MyEntity entity in SelectedEntities)
                {
                    if (selectedType == null)
                    {
                        selectedType = entity.GetType();
                    }
                    else if(selectedType != entity.GetType())
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is any voxel map selected
        /// </summary>
        /// <returns></returns>
        static bool IsAnyVoxelMapSelected()
        {
            if (IsAnyEntitySelected())
            {
                foreach (MyEntity entity in SelectedEntities)
                {
                    if (entity is MyVoxelMap) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Is any transformation key pressed
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static bool IsTransformationKeyPressed(MyGuiInput input, Keys key)
        {
            bool result = false;
            MyEntity entity = GetFirstSelected();
            //when the key is pressed for a longer period, allow to move object constantly in chosen direction and not only in one step
            if (input.IsKeyPress(key))
            {
                if (input.WasKeyPressed(key) == false)
                {
                    m_delayObjectMovementInMillis = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    result = input.IsNewKeyPress(key);
                }
                else
                {
                    if (IsDelayForSmoothMovementReached())
                    {
                        result = true;
                    }
                }
            }

            if (result) TransformationActive = true;

            return result;
        }

        /// <summary>
        /// Is delay for smooth movement of entities reached
        /// </summary>
        /// <returns></returns>
        static bool IsDelayForSmoothMovementReached()
        {
            return m_delayObjectMovementInMillis + MyEditorConstants.DELAY_FOR_SMOOTH_OBJECT_MOVEMENT_IN_MILLIS < MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        /// <summary>
        /// Is rotation active
        /// </summary>
        /// <returns></returns>
        public static bool IsRotationActive()
        {
            return MyEditorGizmo.TransformationActive == true && MyEditorGizmo.ActiveMode == GizmoMode.ROTATE;
        }

        /// <summary>
        /// Is any axis active
        /// </summary>
        /// <returns></returns>
        public static bool IsAnyAxisSelected()
        {
            return MyEditorGizmo.ActiveAxis != GizmoAxis.NONE;
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Switch gizmo mode
        /// </summary>
        public static void SwitchGizmoMode()
        {
            if (ActiveMode == GizmoMode.TRANSLATE)
            {
                if (IsAnyVoxelMapSelected() == false)
                {
                    ActiveMode = GizmoMode.ROTATE;
                }
                else
                {
                    ActiveMode = GizmoMode.TRANSLATE;
                }
            }
            else
            {
                ActiveMode = GizmoMode.TRANSLATE;
            }
        }

        public static void SwitchRotateSnapping()
        {
   
                    // Obtain the values of all the elements within myEnum 
                var rotateSnappings = Enum.GetValues( typeof( RotateSnapping ) );
                    

                    var index = 0;
                    foreach (RotateSnapping rotateSnapping in rotateSnappings)
                    {
                        if (rotateSnapping == ActiveRotateSnapping)
                        {
                            var newIndex = (index + 1)%rotateSnappings.Length;
                            ActiveRotateSnapping = (RotateSnapping)rotateSnappings.GetValue(newIndex);
                            break;
                        }
                        index++;
                    }
        }



        /// <summary>
        /// Get predefined rotation delta value
        /// </summary>
        /// <returns></returns>
        public static float GetRotationDelta()
        {
            float rotationAngleInDegrees = MyEditorConstants.BASE_ROTATION_DELTA_ANGLE_IN_DEGREES;
            if ((GetFirstSelected() is MyPrefabBase  || SelectedSnapPoint != null) && MyConfig.EditorLockedPrefab90DegreesRotation)
            {
                rotationAngleInDegrees = MyEditorConstants.ROTATION_DELTA_RIGHT_ANGLE_IN_DEGREES;
            }
            
            return rotationAngleInDegrees;
        }

        /// <summary>
        /// Only for drawing gizmo models, because some objects need to use special world matrix
        /// </summary>
        /// <param name="worldMatrix"></param>
        /// <returns></returns>
        static Matrix GetWorldMatrixForDraw(ref Matrix worldMatrix)
        {
            Matrix outMatrix;
            Matrix.Multiply(ref worldMatrix, ref MyCamera.InversePositionTranslationMatrix, out outMatrix);
            return outMatrix;
        }

        /// <summary>
        /// Gets higher value from two floats
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static float GetHigherAbsValue(float a, float b)
        {
            float result = a;
            float absValueA = Math.Abs(a);
            float absValueB = Math.Abs(b);
            if (absValueA < absValueB) result = b;
            return result;
        }

        static Matrix GetWorldMatrix()
        {
            if (SelectedSnapPoint != null)
            {
                return SelectedSnapPoint.Matrix * SelectedSnapPoint.Prefab.WorldMatrix;
            }
            else
            {
                Debug.Assert(SelectedEntities.Count > 0);
                return SelectedEntities[0].WorldMatrix;
            }
        }

        #endregion
    }
}
