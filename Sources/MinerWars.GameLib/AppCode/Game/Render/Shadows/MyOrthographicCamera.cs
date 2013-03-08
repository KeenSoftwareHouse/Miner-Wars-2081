using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Render
{
    /// <summary>
    /// Camera that uses an orthographic projection
    /// </summary>
    public class MyOrthographicCamera : MyRenderCamera
    {
        float m_width;
        float m_height;

        float m_xMin;
        float m_xMax;
        float m_yMin;
        float m_yMax;

        BoundingFrustum m_unscaledFrustum = new BoundingFrustum(Matrix.Identity);
        public Matrix CameraSubfrustum;
        Vector3[] m_unscaledCorners = new Vector3[8];
        BoundingBox m_unscaledBoundingBox = new BoundingBox();
        BoundingBox m_boundingBox = new BoundingBox();
        List<MyRender.MyRenderElement> m_castingRenderElements = new List<MyRender.MyRenderElement>(1024);

        internal List<MyRender.MyRenderElement> CastingRenderElements
        {
            set
            {
                m_castingRenderElements.Clear();
                for (int i = 0; i < value.Count; i++)
                {
                    MyRender.MyRenderElement element = value[i];
                    m_castingRenderElements.Add(element);
                }
            }

            get
            {
                return m_castingRenderElements;
            }
        }


        public float Width
        {
            get { return m_width; }
        }

        public float Height
        {
            get { return m_height; }
        }

        public float XMin
        {
            get { return m_xMin; }
            set
            {
                m_xMin = value;
                m_width = m_xMax - m_xMin;
                Matrix.CreateOrthographicOffCenter(m_xMin, m_xMax, m_yMin, m_yMax, m_nearClip, m_farClip, out m_projectionMatrix);
                Update();
            }
        }

        public float XMax
        {
            get { return m_xMax; }
            set
            {
                m_xMax = value;
                m_width = m_xMax - m_xMin;
                Matrix.CreateOrthographicOffCenter(m_xMin, m_xMax, m_yMin, m_yMax, m_nearClip, m_farClip, out m_projectionMatrix);
                Update();
            }
        }

        public float YMin
        {
            get { return m_xMin; }
            set
            {
                m_yMin = value;
                m_height = m_yMax - m_yMin;
                Matrix.CreateOrthographicOffCenter(m_xMin, m_xMax, m_yMin, m_yMax, m_nearClip, m_farClip, out m_projectionMatrix);
                Update();
            }
        }

        public float YMax
        {
            get { return m_xMin; }
            set
            {
                m_yMax = value;
                m_height = m_yMax - m_yMin;
                Matrix.CreateOrthographicOffCenter(m_xMin, m_xMax, m_yMin, m_yMax, m_nearClip, m_farClip, out m_projectionMatrix);
                Update();
            }
        }

        public override float NearClip
        {
            get { return m_nearClip; }
            set
            {
                m_nearClip = value;
                Matrix.CreateOrthographicOffCenter(m_xMin, m_xMax, m_yMin, m_yMax, m_nearClip, m_farClip, out m_projectionMatrix);
                Update();
            }
        }

        public override float FarClip
        {
            get { return m_farClip; }
            set
            {
                m_farClip = value;
                Matrix.CreateOrthographicOffCenter(m_xMin, m_xMax, m_yMin, m_yMax, m_nearClip, m_farClip, out m_projectionMatrix);
                Update();
            }
        }

        /// <summary>
        /// Creates a camera using an orthographic projection
        /// </summary>
        /// <param name="width">Width of the projection volume</param>
        /// <param name="height">Height of the projection volume</param>
        /// <param name="nearClip">Distance to near clip plane</param>
        /// <param name="farClip">Distance to far clip plane</param>
        public MyOrthographicCamera(float width, float height, float nearClip, float farClip)
            : base()
        {
            this.m_width = width;
            this.m_height = height;
            this.m_nearClip = nearClip;
            this.m_farClip = farClip;
            this.m_xMax = width / 2;
            this.m_yMax = height / 2;
            this.m_xMin = -width / 2;
            this.m_yMin = -height / 2;
            Matrix.CreateOrthographic(width, height, nearClip, farClip, out m_projectionMatrix);
            Update();
        }

        public MyOrthographicCamera(float xMin, float xMax, float yMin, float yMax, float nearClip, float farClip)
            : base()
        {
            Update(xMin, xMax, yMin, yMax, nearClip, farClip);
        }

        public void Update(float xMin, float xMax, float yMin, float yMax, float nearClip, float farClip)
        {
            this.m_xMin = xMin;
            this.m_yMin = yMin;
            this.m_xMax = xMax;
            this.m_yMax = yMax;
            this.m_width = xMax - xMin;
            this.m_height = yMax - yMin;
            this.m_nearClip = nearClip;
            this.m_farClip = farClip;
            Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out m_projectionMatrix);
            Update();

            Debug.Assert(xMax > xMin && yMax > yMin, "Invalid ortho camera params");
        }

        public void UpdateUnscaled(float xMin, float xMax, float yMin, float yMax, float nearClip, float farClip)
        {
            this.m_xMin = xMin;
            this.m_yMin = yMin;
            this.m_xMax = xMax;
            this.m_yMax = yMax;
            this.m_width = xMax - xMin;
            this.m_height = yMax - yMin;
            this.m_nearClip = nearClip;
            this.m_farClip = farClip;
            Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out m_unscaledProjectionMatrix);

            Debug.Assert(xMax > xMin && yMax > yMin, "Invalid ortho camera params");
        }


        protected void UpdateUnscaled()
        {
            // Make our view matrix
            Matrix.Invert(ref m_worldMatrix, out m_viewMatrix);

            // Create the combined view-projection matrix
            Matrix.Multiply(ref m_viewMatrix, ref m_unscaledProjectionMatrix, out m_viewProjMatrix);

            // Create the bounding frustum
            m_unscaledFrustum.Matrix = m_viewProjMatrix;
        }

        public void SetViewMatrixUnscaled(ref Matrix viewMatrix)
        {
            this.m_viewMatrix = viewMatrix;
            Matrix.Invert(ref viewMatrix, out m_worldMatrix);
            UpdateUnscaled();
        }

        public BoundingFrustum UnscaledBoundingFrustum
        {
            get { return m_unscaledFrustum; }
        }

        public BoundingBox UnscaledBoundingBox
        {
            get
            {
                UnscaledBoundingFrustum.GetCorners(m_unscaledCorners);
                m_unscaledBoundingBox = m_unscaledBoundingBox.CreateInvalid();
                foreach (Vector3 corner in m_unscaledCorners)
                {
                    Vector3 cornerInst = corner;
                    m_unscaledBoundingBox = m_unscaledBoundingBox.Include(ref cornerInst);
                }

                return m_unscaledBoundingBox;
            }
        }


        public BoundingBox BoundingBox
        {
            get
            {
                BoundingFrustum.GetCorners(m_unscaledCorners);
                m_boundingBox = m_boundingBox.CreateInvalid();
                foreach (Vector3 corner in m_unscaledCorners)
                {
                    Vector3 cornerInst = corner;
                    m_boundingBox = m_boundingBox.Include(ref cornerInst);
                }

                return m_boundingBox;
            }
        }
    }
}
