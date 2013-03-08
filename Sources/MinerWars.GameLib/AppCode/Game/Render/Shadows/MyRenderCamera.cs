using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Render
{
    /// <summary>
    /// Abstract base class for all camera types
    /// </summary>
    public abstract class MyRenderCamera
    {
        protected Matrix m_viewMatrix = Matrix.Identity;
        protected Matrix m_worldMatrix = Matrix.Identity;
        protected Matrix m_projectionMatrix = Matrix.Identity;
        protected Matrix m_unscaledProjectionMatrix = Matrix.Identity;
        protected Matrix m_viewProjMatrix = Matrix.Identity;

        protected BoundingFrustum m_boundingFrustum;
      
        protected float m_nearClip;
        protected float m_farClip;

        public void GetWorldMatrix(out Matrix worldMatrix)
        {
            worldMatrix = this.m_worldMatrix;
        }

        public void SetWorldMatrix(ref Matrix worldMatrix)
        {
            this.m_worldMatrix = worldMatrix;
            Update();
        }

        public void GetViewMatrix(out Matrix viewMatrix)
        {
            viewMatrix = this.m_viewMatrix;
        }

        public void SetViewMatrix(ref Matrix viewMatrix)
        {
            this.m_viewMatrix = viewMatrix;
            Matrix.Invert(ref viewMatrix, out m_worldMatrix);
            Update();
        }

        public void GetProjectionMatrix(out Matrix projectionMatrix)
        {
            projectionMatrix = this.m_projectionMatrix;
        }

        public void GetViewProjMatrix(out Matrix viewProjMatrix)
        {
            viewProjMatrix = this.m_viewProjMatrix;
        }

        public Matrix WorldMatrix
        {
            get { return m_worldMatrix; }
            set
            {
                m_worldMatrix = value;
                Update();
            }
        }

        public Matrix ViewMatrix
        {
            get { return m_viewMatrix; }
            set
            {
                m_viewMatrix = value;
                Matrix.Invert(ref m_viewMatrix, out m_worldMatrix);
                Update();
            }
        }

        public Matrix ProjectionMatrix
        {
            get { return m_projectionMatrix; }
            set { m_projectionMatrix = value; }
        }

        public Matrix ViewProjectionMatrix
        {
            get { return m_viewProjMatrix; }
            set { m_viewProjMatrix = value; }
        }

        public virtual float NearClip
        {
            get { return m_nearClip; }
            set { }
        }

        public virtual float FarClip
        {
            get { return m_farClip; }
            set { }
        }

        public Vector3 Position
        {
            get { return m_worldMatrix.Translation; }
            set
            {
                m_worldMatrix.Translation = value;
                Update();
            }
        }

        public BoundingFrustum BoundingFrustum
        {
            get { return m_boundingFrustum; }
        }

        public Quaternion Orientation
        {
            get
            {
                Quaternion orientation;
                Quaternion.CreateFromRotationMatrix(ref m_worldMatrix, out orientation);
                return orientation;
            }
            set
            {
                Quaternion orientation = value;
                Vector3 position = m_worldMatrix.Translation;
                Matrix.CreateFromQuaternion(ref orientation, out m_worldMatrix);
                m_worldMatrix.Translation = position;
                Update();
            }
        }

        public Matrix FrustumProjectionMatrix;

        /// <summary>
        /// Base constructor
        /// </summary>
        public MyRenderCamera()
        {
            m_boundingFrustum = new BoundingFrustum(m_viewProjMatrix);
            m_worldMatrix = Matrix.Identity;
            m_viewMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Applies a transform to the camera's world matrix,
        /// with the new transform applied first
        /// </summary>
        /// <param name="transform">The transform to be applied</param>
        public void PreTransform(ref Matrix transform)
        {
            Matrix.Multiply(ref transform, ref m_worldMatrix, out m_worldMatrix);
            Update();
        }

        /// <summary>
        /// Applies a transform to the camera's world matrix,
        /// with the new transform applied second
        /// </summary>
        /// <param name="transform">The transform to be applied</param>
        public void PostTransform(ref Matrix transform)
        {
            Matrix.Multiply(ref m_worldMatrix, ref transform, out m_worldMatrix);
            Update();
        }

        /// <summary>
        /// Updates the view-projection matrix and frustum coordinates based on
        /// the current camera position/orientation and projection parameters.
        /// </summary>
        protected void Update()
        {
            // Make our view matrix
            Matrix.Invert(ref m_worldMatrix, out m_viewMatrix);

            // Create the combined view-projection matrix
            Matrix.Multiply(ref m_viewMatrix, ref m_projectionMatrix, out m_viewProjMatrix);

            // Create the bounding frustum
            m_boundingFrustum.Matrix = m_viewProjMatrix;
        }
    }
}
