using System;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Render;
using SysUtils.Utils;
using MinerWars.AppCode.Game.World;
using System.Diagnostics;

using SharpDX;

namespace MinerWars.AppCode.Game.Utils
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;

    //  IMPORTANT: If you change this enum, don't forget to change it also in MyEnumsToStrings
    enum MyCameraDirection : byte
    {
        FORWARD,
        SHIP_CUSTOMIZATION_SCREEN
    }

    static class MyCamera
    {
        //  This is camera position we use to remove floating point precision problems. The idea is this: 32bit floats
        //  are precise when positions are less then 100.000 meters. After that, calculation error start to be noticeable.
        //  Less in physics, but MORE in depth buffer. Result is that close surface start z-fighting. I solved this problem
        //  by moving all phys objetcs, voxels and particles near to camera, so from DirectX point of view, camera is always
        //  in point zero (0, 0, 0). Thanks to this, we don't send large positions to GPU and therefore not to depth buffer.
        //  But physics and rest of the code uses absolute and high numbers. Physics seems to have tolerable precision up to 200 km 
        //  (graphic only 25 km).
        //  E.g. when players reaches position (100 km, 0, 0) - his camera is at that position, before we sent real/absolute world
        //  matrix and camera position to GPU, we subtract camera position from it, so in this case if there will be object at
        //  position (105 km, 0, 0), we will say its world matrix is in position (5 km, 0, 0) - because we subtracted (100 km, 0, 0).
        //  Also light positions are subtracted.
        public static readonly Vector3 PositionZero = Vector3.Zero;

        //  Original was 0.5, but I changed it to 0.35 so in extreme-wide screen resolution cockpit glass isn't truncated!
        // Previous distance was 0.35, changed to 0.27 so fov 100 degrees is displayed properly (otherwise cockpit would be truncated)
        // Lowered even more to 0.13 to solve near clip problems with cockpit in triple-head
        public static float NEAR_PLANE_DISTANCE = 2.0f;
        //  Two times bigger than sector's diameter because we want to draw impostor voxel maps in surrounding sectors
        //  According to information from xna creators site, far plane distance doesn't have impact on depth buffer precission, but near plane has.
        //  Therefore far plane distance can be any large number, but near plane distance can't be too small.
        public static float FAR_PLANE_DISTANCE = 70000;

        // Near clip plane for "near" objects, near objects are cockpit, cockpit glass and weapons
        public static float NEAR_PLANE_FOR_NEAR_OBJECTS = 0.08f;

        // For 3rd person camera to be able to zoom in (closer to ship)
        public static float NEAR_PLANE_FOR_3RD_PERSON = 0.5f;

        // Far clip plane for "near" objects, near objects are cockpit, cockpit glass and weapons
        public static float FAR_PLANE_FOR_NEAR_OBJECTS = 100.0f;

        public static float NEAR_PLANE_FOR_PARTICLES = NEAR_PLANE_FOR_NEAR_OBJECTS;
        public static float FAR_PLANE_FOR_PARTICLES = 50000;

        // When LOD transition distances are more than FAR PLANE, it must be adjusted in way where LOD near < LOD far < background start < background end
        // these distances must have different values in depth buffer, this threshold make sure they will
        public static readonly float FAR_DISTANCE_THRESHOLD = 100;

        //  This are ACTUAL public properties of a camera. If we are looking forward, it contains related values.
        public static Vector3 Position;
        public static void SetPosition(Vector3 value)
        {
            MyUtils.AssertIsValid(value);
            Position = value;
        }


        public static Vector3 ForwardVector = Vector3.Forward;
        public static Vector3 LeftVector = Vector3.Left;
        public static Vector3 UpVector = Vector3.Up;
        public static Vector3 Velocity;
        public static Vector3 PreviousPosition;
        public static Viewport Viewport;                    //  Current viewport
        public static Matrix InversePositionTranslationMatrix;  //  This is: Matrix.CreateTranslation(-MyCamera.Position);
        public static Matrix ViewMatrix;                    //  This is view matrix when camera in real position
        public static Matrix ViewMatrixAtZero;              //  This is view matrix when camera at zero position [0,0,0]
        public static Matrix ProjectionMatrix;
        public static Matrix ProjectionMatrixForNearObjects;
        public static Matrix ViewProjectionMatrix;          //  This is view-projection matrix when camera in real position
        public static Matrix ViewProjectionMatrixAtZero;    //  This is view-projection matrix when camera at zero position [0,0,0]
        public static BoundingBox BoundingBox;              //    Bounding box calculated from bounding frustum, updated every draw
        public static BoundingSphere BoundingSphere;        //    Bounding sphere calculated from bounding frustum, updated every draw
        public static float AspectRatio;
        public static Vector3 CornerFrustum;

        public static float FieldOfView = (float)(Math.PI / 2.0);
        public static float FieldOfViewForNearObjects = (float)(MathHelper.ToRadians(70));

        public static float FieldOfViewAngle
        {
            get 
            {
                return MathHelper.ToDegrees(FieldOfView);
            }
            set
            {
                FieldOfView = MathHelper.ToRadians(value);
            }
        }

        public static MyCameraDirection ActualCameraDirection;      //  This will tell us if right now camera is looking forward or backward (thus drawing backward camera)

        public static Viewport ForwardViewport;
        public static Viewport BackwardViewport;
        public static Viewport HudViewport;
        public static Viewport FullscreenHudViewport;

        public static MyCameraZoomProperties Zoom;

        //  Calculated or constants parameters of this camera
        public static float ForwardAspectRatio;

        static Matrix m_forwardViewMatrix;
        static Matrix m_forwardProjectionMatrix;
        static Matrix m_forwardProjectionMatrixForNearObjects;

        static float m_lodTransitionDistanceNear;
        static float m_lodTransitionDistanceFar;
        static float m_lodTransitionDistanceBackgroundStart;
        static float m_lodTransitionDistanceBackgroundEnd;
        static float m_lodTransitionDistanceBackgroundStartWithoutZoom;
        static float m_lodTransitionDistanceBackgroundEndWithoutZoom;

        private static BoundingFrustum BoundingFrustum;
        private static float m_zoomDivider = 1.0f;

        public static bool EnableZoom = true;

        public static Matrix? backupMatrix = null;

        /// <summary>
        /// Gets current fov with considering if zoom is enabled
        /// </summary>
        public static float FovWithZoom
        {
            get
            {
                return EnableZoom ? Zoom.GetFOV() : MyCamera.FieldOfView;
            }
        }

        /// <summary>
        /// Gets current fov with considering if zoom is enabled
        /// </summary>
        public static float FovWithZoomForNearObjects
        {
            get
            {
                return EnableZoom ? Zoom.GetFOVForNearObjects() : MyCamera.FieldOfViewForNearObjects;
            }   
        }

        /// <summary>
        /// Return zoom divider with considering if zoom is enabled
        /// </summary>
        public static float ZoomDivider
        {
            get
            {
                return EnableZoom ? m_zoomDivider : 1.0f;
            }
        }

        /// <summary>
        /// GetBoundingFrustum
        /// </summary>
        /// <returns></returns>
        public static BoundingFrustum GetBoundingFrustum()
        {
            return BoundingFrustum;
        }


        public static void LoadContent()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyCamera::LoadContent");
            Zoom = new MyCameraZoomProperties();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void Update()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyCamera-Update");
            Zoom.Update();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UpdateScreenSize()
        {
            ForwardViewport = MyMinerGame.Static.GraphicsDevice.Viewport;
            BackwardViewport = MyGuiManager.GetBackwardViewport();
            HudViewport = MyGuiManager.GetHudViewport();
            FullscreenHudViewport = MyGuiManager.GetFullscreenHudViewport();

            PreviousPosition = Vector3.Zero;
            BoundingFrustum = new BoundingFrustum(Matrix.Identity);

            ForwardAspectRatio = (float)ForwardViewport.Width / (float)ForwardViewport.Height;

            if (MyGuiManager.GetScreenshot() != null)
            {
                ForwardViewport = ScaleViewport(ForwardViewport, MyGuiManager.GetScreenshot().SizeMultiplier);
                BackwardViewport = ScaleViewport(BackwardViewport, MyGuiManager.GetScreenshot().SizeMultiplier);
                HudViewport.Y = (int)(HudViewport.Y * MyGuiManager.GetScreenshot().SizeMultiplier);
                HudViewport.Height = (int)(HudViewport.Height * MyGuiManager.GetScreenshot().SizeMultiplier);
                FullscreenHudViewport = ScaleViewport(FullscreenHudViewport, MyGuiManager.GetScreenshot().SizeMultiplier);
            }
        }

        private static Viewport ScaleViewport(Viewport viewport, float scale)
        {
            return new Viewport((int)(viewport.X * scale), (int)(viewport.Y * scale), (int)(viewport.Width * scale), (int)(viewport.Height * scale));
        }

        public static void SetViewMatrix(Matrix value)
        {
            m_forwardViewMatrix = value;

            Matrix invertedViewMatrix;
            Matrix.Invert(ref m_forwardViewMatrix, out invertedViewMatrix);
            SetPosition(invertedViewMatrix.Translation);
            InversePositionTranslationMatrix = Matrix.CreateTranslation(-Position);
            
            Velocity = (Position - PreviousPosition) / MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            PreviousPosition = Position;

            //  Projection matrix according to zoom level
            m_forwardProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(FovWithZoom, ForwardAspectRatio,
                GetSafeNear(),
                MyCamera.FAR_PLANE_DISTANCE);

            //  Projection matrix according to zoom level
            float near = System.Math.Min(MyCamera.NEAR_PLANE_DISTANCE, MyCamera.NEAR_PLANE_FOR_NEAR_OBJECTS / ZoomDivider); //minimum cockpit distance 
            m_forwardProjectionMatrixForNearObjects = Matrix.CreatePerspectiveFieldOfView(FovWithZoomForNearObjects, ForwardAspectRatio,
                near,
                MyCamera.FAR_PLANE_FOR_NEAR_OBJECTS);

        }

        /// <summary>
        /// Changes FOV for ForwardCamera (updates projection matrix)
        /// SetViewMatrix overwrites this changes
        /// </summary>
        /// <param name="fov"></param>
        public static void ChangeFov(float fov)
        {
            //  Projection matrix according to zoom level
            m_forwardProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(fov, ForwardAspectRatio,
                GetSafeNear(),
                MyCamera.FAR_PLANE_DISTANCE);
        }

        public static void ChangeClipPlanes(float near, float far, bool applyNow = false)
        {
            Debug.Assert(!backupMatrix.HasValue, "Reset clip planes before changing clip planes again");
            backupMatrix = m_forwardProjectionMatrix;
            m_forwardProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(FovWithZoom, ForwardAspectRatio, near, far);
            if (applyNow)
            {
                EnableForward();
            }
        }

        public static void SetParticleClipPlanes(bool applyNow = false)
        {
            float near = System.Math.Min(MyCamera.NEAR_PLANE_DISTANCE, MyCamera.NEAR_PLANE_FOR_PARTICLES / ZoomDivider); //minimum cockpit distance            
            ChangeClipPlanes(near, MyCamera.FAR_PLANE_FOR_PARTICLES, applyNow);
        }

        public static void SetNearObjectsClipPlanes(bool applyNow = false)
        {
            float near = System.Math.Min(MyCamera.NEAR_PLANE_DISTANCE, MyCamera.NEAR_PLANE_FOR_NEAR_OBJECTS / ZoomDivider); //minimum cockpit distance            
            ChangeClipPlanes(near, MyCamera.FAR_PLANE_FOR_NEAR_OBJECTS, applyNow);
        }

        static float GetSafeNear()
        {
            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                return System.Math.Min(4, MyCamera.NEAR_PLANE_FOR_NEAR_OBJECTS / ZoomDivider); //minimum cockpit distance            

            if (MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static != null && MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static.CameraAttachedTo == MinerWars.AppCode.Game.GUI.MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic)
                return System.Math.Min(4, MyCamera.NEAR_PLANE_FOR_3RD_PERSON / ZoomDivider); //minimum cockpit distance
            else
                return System.Math.Min(4, MyCamera.NEAR_PLANE_DISTANCE / ZoomDivider); //minimum cockpit distance            
        }

        public static void ResetClipPlanes(bool applyNow = false)
        {
            Debug.Assert(backupMatrix.HasValue, "Nothing to reset, use change clip planes first");
            m_forwardProjectionMatrix = backupMatrix.Value;
            backupMatrix = null;
            if (applyNow)
            {
                EnableForward();
            }
            //ChangeClipPlanes(GetSafeNear(), MyCamera.FAR_PLANE_DISTANCE, applyNow);
        }

        public static void SetCustomProjection(Matrix projection)
        {
            m_forwardProjectionMatrix = projection;
        }

        //  Distances for LOD transition, near and far. Zoom is applied only of forward camera.
        static void UpdateLodTransitionDistances()
        {
            m_lodTransitionDistanceNear = MyRenderConstants.RenderQualityProfile.LodTransitionDistanceNear;
            m_lodTransitionDistanceFar = MyRenderConstants.RenderQualityProfile.LodTransitionDistanceFar;
            m_lodTransitionDistanceBackgroundStart = MyRenderConstants.RenderQualityProfile.LodTransitionDistanceBackgroundStart;
            m_lodTransitionDistanceBackgroundEnd = MyRenderConstants.RenderQualityProfile.LodTransitionDistanceBackgroundEnd;

            if (ActualCameraDirection == MyCameraDirection.FORWARD)
            {
                m_lodTransitionDistanceNear /= ZoomDivider;
                m_lodTransitionDistanceFar /= ZoomDivider;
                m_lodTransitionDistanceBackgroundStartWithoutZoom = m_lodTransitionDistanceBackgroundStart;
                m_lodTransitionDistanceBackgroundEndWithoutZoom = m_lodTransitionDistanceBackgroundEnd;
                m_lodTransitionDistanceBackgroundStart /= ZoomDivider;
                m_lodTransitionDistanceBackgroundEnd /= ZoomDivider;

                // Make sure all distances are smaller than FAR_PLANE_DISTANCE (otherwise it would broke LOD transition effect and background blending)
                if (m_lodTransitionDistanceBackgroundEnd > FAR_PLANE_DISTANCE)
                {
                    m_lodTransitionDistanceBackgroundEnd = FAR_PLANE_DISTANCE - FAR_DISTANCE_THRESHOLD;

                    if (m_lodTransitionDistanceBackgroundStart > m_lodTransitionDistanceBackgroundEnd)
                    {
                        m_lodTransitionDistanceBackgroundStart = m_lodTransitionDistanceBackgroundEnd - FAR_DISTANCE_THRESHOLD;

                        if (m_lodTransitionDistanceFar > m_lodTransitionDistanceBackgroundStart)
                        {
                            m_lodTransitionDistanceFar = m_lodTransitionDistanceBackgroundStart - FAR_DISTANCE_THRESHOLD;

                            if (m_lodTransitionDistanceNear > m_lodTransitionDistanceFar)
                            {
                                m_lodTransitionDistanceNear = m_lodTransitionDistanceFar - FAR_DISTANCE_THRESHOLD;
                            }
                        }
                    }
                }
            }
        }

        //  Call before drawing forward look
        public static void EnableForward()
        {
            EnableCamera(MyCameraDirection.FORWARD, ForwardViewport, ForwardAspectRatio, m_forwardViewMatrix, m_forwardProjectionMatrix);
        }

        static void EnableCamera(MyCameraDirection cameraDirection, Viewport viewport, float aspectRatio, Matrix viewMatrix, Matrix projectionMatrix)
        {
            ActualCameraDirection = cameraDirection;
            Viewport = viewport;
            MyMinerGame.Static.SetDeviceViewport(viewport);
            AspectRatio = aspectRatio;
            ViewMatrix = viewMatrix;
            ProjectionMatrix = projectionMatrix;
            ProjectionMatrixForNearObjects = m_forwardProjectionMatrixForNearObjects;
            ViewProjectionMatrix = ViewMatrix * ProjectionMatrix;

            UpdateVectors();
            UpdateBoundingFrustum();

            ViewMatrixAtZero = Matrix.CreateLookAt(Vector3.Zero, ForwardVector, UpVector);
            
            ViewProjectionMatrixAtZero = ViewMatrixAtZero * ProjectionMatrix;

            UpdateLodTransitionDistances();
            CornerFrustum = CalculateCornerFrustum();
        }

        //  Call before drawing HUD
        public static void EnableHud()
        {
            MyMinerGame.Static.SetDeviceViewport(MyGuiManager.FullscreenHudEnabled ? FullscreenHudViewport : HudViewport);
        }

        static void UpdateVectors()
        {
            m_zoomDivider = System.Math.Max(0.01f,(FovWithZoom - MyConstants.FIELD_OF_VIEW_MIN) / (MyCamera.FieldOfView - MyConstants.FIELD_OF_VIEW_MIN));
            //System.Diagnostics.Debug.Assert(m_zoomDivider > 0 && m_zoomDivider <= 1.0f, "Zoom divider cannot move objects far (just closer)");

            Matrix invertedViewMatrix;
            Matrix.Invert(ref ViewMatrix, out invertedViewMatrix);
            ForwardVector = invertedViewMatrix.Forward;
            LeftVector = invertedViewMatrix.Left;
            UpVector = invertedViewMatrix.Up;
        }

        static void UpdateBoundingFrustum()
        {
            //  Update frustum
            BoundingFrustum.Matrix = ViewProjectionMatrix;

            //  Update bounding box
            BoundingBox = new BoundingBox(new Vector3(float.PositiveInfinity), new Vector3(float.NegativeInfinity));            
            BoundingBoxHelper.AddFrustum(ref BoundingFrustum, ref BoundingBox);

            //  Update bounding sphere
            BoundingSphere = MyUtils.GetBoundingSphereFromBoundingBox(ref BoundingBox);
        }

        //  Checks if specified bounding box is in actual bounding frustum
        //  IMPORTANT: If you observe bad result of this test, check how you transform your bounding box.
        //  Don't use BoundingBox.Transform. Instead transform box manualy and then create new box.
        public static bool IsInFrustum(ref BoundingBox boundingBox)
        {
            MinerWarsMath.ContainmentType result;
            BoundingFrustum.Contains(ref boundingBox, out result);
            return result != MinerWarsMath.ContainmentType.Disjoint;
        }

        public static bool IsInFrustum(BoundingBox boundingBox)
        {
            return IsInFrustum(ref boundingBox);
        }

        //  Checks if specified bounding sphere is in actual bounding frustum
        //  IMPORTANT: If you observe bad result of this test, check how you transform your bounding sphere.
        //  Don't use BoundingSphere.Transform. Instead transform sphere center manualy and then create new sphere.
        public static bool IsInFrustum(ref BoundingSphere boundingSphere)
        {
            MinerWarsMath.ContainmentType result;
            BoundingFrustum.Contains(ref boundingSphere, out result);
            return result != MinerWarsMath.ContainmentType.Disjoint;
        }

        //  Checks if specified Vector3 is in actual bounding frustum
        public static bool IsInFrustum(ref Vector3 point)
        {
            MinerWarsMath.ContainmentType result;
            BoundingFrustum.Contains(ref point, out result);
            return result != MinerWarsMath.ContainmentType.Disjoint;
        }

        // Should not be used elsewhere than MyRender.ApplySetups, others should use MyRender.CurrentRenderSetup...
        public static float GetLodTransitionDistanceNear()
        {
            return m_lodTransitionDistanceNear;
        }

        // Should not be used elsewhere than MyRender.ApplySetups, others should use MyRender.CurrentRenderSetup...
        public static float GetLodTransitionDistanceFar()
        {
            return m_lodTransitionDistanceFar;
        }

        // Should not be used elsewhere than MyRender.ApplySetups, others should use MyRender.CurrentRenderSetup...
        public static float GetLodTransitionDistanceBackgroundStart()
        {
            return m_lodTransitionDistanceBackgroundStart;
        }

        // Should not be used elsewhere than MyRender.ApplySetups, others should use MyRender.CurrentRenderSetup...
        public static float GetLodTransitionDistanceBackgroundEnd()
        {
            return m_lodTransitionDistanceBackgroundEnd;
        }

        public static float GetLodTransitionDistanceBackgroundStartWithoutZoom()
        {
            return m_lodTransitionDistanceBackgroundStartWithoutZoom;
        }

        public static float GetLodTransitionDistanceBackgroundEndWithoutZoom()
        {
            return m_lodTransitionDistanceBackgroundEndWithoutZoom;
        }

        static Vector3 CalculateCornerFrustum()
        {
            float farY = (float)Math.Tan(Math.PI / 3.0 / 2.0) * MyCamera.FAR_PLANE_DISTANCE;
            float farX = farY * AspectRatio;
            return new Vector3(farX, farY, MyCamera.FAR_PLANE_DISTANCE);
        }

        public static void SetupBaseEffect(MyEffectBase effect, float fogMultiplierMult = 1.0f)
        {
            if (MyRender.EnableFog)
            {
                effect.SetFogDistanceFar(MySector.FogProperties.FogFar);
                effect.SetFogDistanceNear(MySector.FogProperties.FogNear);
                effect.SetFogColor(MySector.FogProperties.FogColor);
                effect.SetFogMultiplier(MySector.FogProperties.FogMultiplier * fogMultiplierMult);
                effect.SetFogBacklightMultiplier(MySector.FogProperties.FogBacklightMultiplier);
            }
            else
            {
                effect.SetFogMultiplier(0);
            }

            //if (MyRenderConstants.RenderQualityProfile.ForwardRender || !MyRender.EnableLODBlending)
            {
                if (MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD0)
                    effect.SetLodCut((GetLodTransitionDistanceFar() + GetLodTransitionDistanceNear()) / 2.0f);
                else
                   if (MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD1)
                    effect.SetLodCut(-(GetLodTransitionDistanceFar() + GetLodTransitionDistanceNear()) / 2.0f);
                   else
                       effect.SetLodCut(0);

                effect.SetLodBackgroundCut(GetLodTransitionDistanceBackgroundEnd());
            }

            /*
            if (!MyRenderConstants.RenderQualityProfile.ForwardRender && MyRender.EnableLODBlending)
            {
                if (MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD1)
                {
                    //effect.SetLodCut(-(GetLodTransitionDistanceFar() + GetLodTransitionDistanceNear()) / 2.0f);
                    //effect.SetLodCut(-(GetLodTransitionDistanceFar() + GetLodTransitionDistanceNear()) / 2.0f);
                    effect.SetLodCut(0);
                }
                else
                {
                    effect.SetLodCut(0);
                }
            }  */
        }

        public static float GetDistanceWithFOV(Vector3 position)
        {
            return GetDistanceWithFOV(Vector3.Distance(Position, position));
        }

        public static float GetDistanceWithFOV(float distance)
        {
            return distance * ZoomDivider;
        }
    }
}