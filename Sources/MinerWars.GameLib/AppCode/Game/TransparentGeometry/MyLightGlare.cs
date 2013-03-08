using System;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using System.Diagnostics;
using ParallelTasks;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit.Graphics;

namespace MinerWars.AppCode.Game.TransparentGeometry
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
    using MathHelper = MinerWarsMath.MathHelper;

    class MyLightGlare : IDisposable
    {
        public static bool EnableLightGlares = true;
        public const float MAX_GLARE_DISTANCE = 2000;

        #region Enums

        public enum State
        {
            IssueOcc,
            IssueMeasure,
            WaitOcc,
            WaitOcc1,
            WaitOcc2,
            WaitOcc3,
            WaitMeasure,
            WaitMeasure1,
            WaitMeasure2,
            WaitMeasure3,
            CheckOcc,
            CheckMeasure
        }

        public enum SizeFunctionEnum
        {
            NoChange,
            IncreasingWithDistance,
        }

        public enum GlareTypeEnum
        {
            /// <summary>
            /// This is the glare that is dependent on occlusion queries.
            /// Physically, this phenomenon originates in the lens.
            /// </summary>
            Normal,

            /// <summary>
            /// This is the glare that you see even if the light itself is occluded.
            /// It gives the impression of scattering in a medium (like fog).
            /// </summary>
            Distant,
        }

        #endregion

        #region Private fields

        private readonly MyLight m_parent;

        // An occlusion query is used to detect when the light source is hidden behind scenery.
        // !IMPORTANT! Every other frame (m_occlusionMeasurement), we render the occlusion box without depth testing in order to find the
        // pixelCount (m_occlusionMeasurementResult) for a fully unoccluded object. When doing the actual occlusion test with depth
        // testing turned on, we divide the final pixel count by m_occlusionMeasurementResult to find out the occlusion ratio.
        private MyOcclusionQuery m_occlusionQuery;
        private MyOcclusionQuery m_measurementQuery;
        private State m_state = State.IssueOcc;
        private BoundingBox m_occlusionBox;
        private float m_occlusionRatio = 1; // 0 - fully occluded, 1 - fully visible

        MyRayCastOcclusionJob m_rayCastOcclusionJob = new MyRayCastOcclusionJob();
        bool m_castingRay;

        public float? Intensity;

        #endregion

        #region Properties

        public Vector3 Position { get { return m_parent.Position; } }

        public GlareTypeEnum Type { get; set; }

        //public bool UseOcclusionQuery { get { return Type == GlareTypeEnum.Normal; } }
        public bool UseOcclusionQuery { get { return true; } }

        /// <summary>
        /// Size of the object used for the occlusion query.
        /// </summary>
        public float QuerySize { get; set; }

        #endregion

        public MyLightGlare(MyLight light)
        {
            m_parent = light;
            m_occlusionBox = new BoundingBox();

            m_occlusionQuery = MyOcclusionQueries.Get();
            m_measurementQuery = MyOcclusionQueries.Get();
        }

        #region Creating and disposing

        public void Dispose()
        {
            MyOcclusionQueries.Return(m_occlusionQuery);
            MyOcclusionQueries.Return(m_measurementQuery);
        }

        #endregion

        public void Start()
        {
            m_castingRay = false;
        }

        public void Clear()
        {
            m_rayCastOcclusionJob.Clear();
            m_castingRay = false;
        }

        public void Draw()
        {
            if (!EnableLightGlares)
            {
                return;
            }

            Vector3 position = this.Position;
            Vector3 cameraToLight = MyCamera.Position - position;
            float distance = cameraToLight.Length();
            const float maxDistance = MyRenderConstants.MAX_GPU_OCCLUSION_QUERY_DISTANCE;

            bool canBeDiscardedIfTooFar = Type != GlareTypeEnum.Distant;
            if (canBeDiscardedIfTooFar && distance > maxDistance)
            {
                return;
            }

            // This is absolute maximum for light glares
            if (distance > MAX_GLARE_DISTANCE)
            {
                return;
            }

            if (UseOcclusionQuery)
            {
                float querySizeMultiplier = distance < maxDistance ? 1 : distance / maxDistance;

                bool isFar = distance > maxDistance;

                // Occlusion is calculated only when closer than 200m, further visibility is handled by depth test
                if(!isFar)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Light glare update occlusion");
                    UpdateOcclusion(querySizeMultiplier * QuerySize, isFar);
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
            }

            switch (Type)
            {
                case GlareTypeEnum.Normal:
                    DrawNormalGlare(distance, maxDistance, position, cameraToLight);
                    break;
                case GlareTypeEnum.Distant:
                    DrawVolumetricGlare(distance, position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawNormalGlare(float distance, float maxDistance, Vector3 position, Vector3 cameraToLight)
        {
            if (m_occlusionRatio <= MyMwcMathConstants.EPSILON)
                return;

            var intensity = GetIntensity();

            float alpha = m_occlusionRatio * intensity;

            const int minGlareRadius = 20;
            const int maxGlareRadius = 40;
            float radius = MathHelper.Clamp(m_parent.Range, minGlareRadius, maxGlareRadius);

            float drawingRadius = radius;

            cameraToLight = (1.0f / distance) * cameraToLight;
            float dot = Vector3.Dot(cameraToLight, m_parent.ReflectorDirection);
            alpha *= MathHelper.Min(1, 6 * dot);

            if (alpha <= MyMwcMathConstants.EPSILON)
                return;

            if (distance > maxDistance * .5f)
            {
                // distance falloff
                float falloff = (distance - .5f * maxDistance) / (.5f * maxDistance);
                falloff = 1 - falloff;
                if (falloff < 0)
                    falloff = 0;
                drawingRadius *= falloff;
                alpha *= falloff;
            }

            if (drawingRadius <= float.Epsilon)
                return;

            drawingRadius *= 0.01f * distance;
            if (drawingRadius < radius)
                drawingRadius = radius;

            var color = m_parent.Color;
            color.W = 0;

            MyTransparentGeometry.AddBillboardOriented(
                MyTransparentMaterialEnum.LightGlare, color * alpha, position,
                MyCamera.LeftVector, MyCamera.UpVector, drawingRadius);
        }

        private void DrawVolumetricGlare(float distance, Vector3 position)
        {
            var intensity = GetIntensity();

            float alpha = m_occlusionRatio * intensity;

            if (alpha < MyMwcMathConstants.EPSILON)
                return;

            const int minGlareRadius = 5;
            const int maxGlareRadius = 150;
            float radius = MathHelper.Clamp(m_parent.Range, minGlareRadius, maxGlareRadius);

            float drawingRadius = radius;

            Debug.Assert(MyRender.CurrentRenderSetup.LodTransitionBackgroundStart != null, "lod transition is not set in render setup");
            var startFadeout = MyRender.CurrentRenderSetup.LodTransitionBackgroundStart.Value;

            Debug.Assert(MyRender.CurrentRenderSetup.LodTransitionBackgroundEnd != null, "lod transition is not set in render setup");
            var endFadeout = MyRender.CurrentRenderSetup.LodTransitionBackgroundEnd.Value;

            if (distance > startFadeout)
            {
                var fade = (distance - startFadeout) / (endFadeout - startFadeout);
                alpha *= (1 - fade);
            }

            if (alpha < MyMwcMathConstants.EPSILON)
                return;

            var color = m_parent.Color;
            color.W = 0;

            var material = distance > MyRenderConstants.MAX_GPU_OCCLUSION_QUERY_DISTANCE ? MyTransparentMaterialEnum.LightGlare_WithDepth : MyTransparentMaterialEnum.LightGlare;

            MyTransparentGeometry.AddBillboardOriented(
                material, color * alpha, position,
                MyCamera.LeftVector, MyCamera.UpVector, drawingRadius);
        }

        private float GetIntensity()
        {
            float intensity;

            if (Intensity.HasValue)
            {
                intensity = MathHelper.Clamp(Intensity.Value, 0, 1);
            }
            else
            {
                var maxParentIntensity = 0.5f * MathHelper.Max(m_parent.Intensity, m_parent.ReflectorIntensity);
                intensity = MathHelper.Clamp(maxParentIntensity, 0, 1);
            }
            return intensity;
        }

        private void UpdateOcclusion(float querySize, bool rayCast)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("update occ 1");

            if (rayCast)
            {
                UpdateRayCastOcclusion();
            }
            else
            {
                m_castingRay = false;
                UpdateGpuOcclusion(querySize);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void UpdateRayCastOcclusion()
        {
            if (!m_castingRay)
            {
                Debug.Assert(MyRender.CurrentRenderSetup.CameraPosition != null, "camera position is not set in render setup");

                m_rayCastOcclusionJob.Start(
                    this,
                    MyRender.CurrentRenderSetup.CameraPosition.Value,
                    GUI.MyGuiScreenGamePlay.Static.CameraAttachedTo == GUI.MyCameraAttachedToEnum.PlayerMinerShip
                        ? MySession.PlayerShip
                        : null,
                    null
                    );

                Parallel.Start(m_rayCastOcclusionJob);
                m_castingRay = true;
            }
            else if (m_rayCastOcclusionJob.IsDone)
            {
                m_castingRay = false;
                m_occlusionRatio = m_rayCastOcclusionJob.Visible ? 1 : 0;
                m_rayCastOcclusionJob.Clear();
            } 
        }

        private void UpdateGpuOcclusion(float querySize)
        {
            switch (m_state)
            {
                case State.IssueOcc:
                    m_occlusionBox.Min = this.Position - new Vector3(querySize);
                    m_occlusionBox.Max = this.Position + new Vector3(querySize);
                    IssueOcclusionQuery(m_occlusionQuery, true);
                    m_state = State.IssueMeasure;
                    break;

                case State.IssueMeasure:
                    IssueOcclusionQuery(m_measurementQuery, false);
                    m_state = State.WaitOcc;
                    break;

                case State.WaitOcc:
                    m_state = State.WaitMeasure;
                    break;

                case State.WaitMeasure:
                    m_state = State.CheckOcc;
                    break;

                case State.CheckOcc:
                    if (m_occlusionQuery.IsComplete)
                    {
                        m_state = State.CheckMeasure;
                    }
                    break;

                case State.CheckMeasure:
                    if (m_measurementQuery.IsComplete)
                    {
                        m_state = State.IssueOcc;
                        m_occlusionRatio = CalcRatio();
                    }
                    break;
            }
        }

        private void IssueOcclusionQuery(MyOcclusionQuery query, bool depthTest)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Issue query");

            BlendState previousBlendState = BlendState.Current; ;
            MyStateObjects.DisabledColorChannels_BlendState.Apply();
            RasterizerState.CullNone.Apply();
            DepthStencilState.None.Apply();

            query.Begin();

            //generate and draw bounding box of our renderCell in occlusion query 
            MySimpleObjectDraw.DrawOcclusionBoundingBox(m_occlusionBox, 1.0f, depthTest);

            previousBlendState.Apply();

            query.End();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private float CalcRatio()
        {
            float measPixels = m_measurementQuery.PixelCount;
            if (measPixels <= 0) measPixels = 1;

            return Math.Min(m_occlusionQuery.PixelCount / measPixels, 1);
        }
    }
}
