using System;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Render.SecondaryCamera;
using MinerWars.AppCode.Game.Utils;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


namespace MinerWars.AppCode.Game.Entities.Ships.SubObjects
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

    public enum MySecondaryCameraAttachedTo
    {
        RearMirror,
        Drone,
        Missile,
        RemoteCamera,
        PlayerShip
    }

    class MySecondaryCamera
    {
        private enum Mode
        {
            BackCamera,
            PlayerShip,
            Entity,
        }

        private static MySecondaryCamera m_instance;
        public static MySecondaryCamera Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MySecondaryCamera();
                }
                return m_instance;
            }
        }

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        public BoundingFrustum BoundingFrustum { get; private set; }

        private Mode m_mode = Mode.BackCamera;

        // For Mode.Entity, specifies the source of the camera view matrix
        private MyEntity m_cameraSource;

        public MySecondaryCameraAttachedTo SecondaryCameraAttachedTo { get; set; }

        public MySecondaryCameraRenderer SecondaryCameraRenderer { get; private set; }

        public bool MirrorImage { get; set; }

        public bool IsInsidePlayerShip
        {
            get
            {
                return m_mode == Mode.BackCamera || m_mode == Mode.PlayerShip;
            }
        }

        public bool IsCurrentlyRendering { get; set; }

        public Texture GetRenderedTexture()
        {
            return SecondaryCameraRenderer.GetRenderedTexture();
        }

        private MySecondaryCamera()
        {
            SecondaryCameraRenderer = MySecondaryCameraRenderer.Instance;
            BoundingFrustum = new BoundingFrustum(Matrix.Identity);
        }

        public void SetRearMirror()
        {
            m_mode = Mode.BackCamera;
            MirrorImage = true;
        }

        public void SetPlayerShip()
        {
            m_mode = Mode.PlayerShip;
            MirrorImage = false;
        }

        public MyEntity GetCameraEntity()
        {
            if (m_mode == Mode.Entity) return m_cameraSource;
            else return null;
        }

        public void SetEntityCamera(MyEntity cameraSource)
        {
            m_mode = Mode.Entity;
            m_cameraSource = cameraSource;
            MirrorImage = false;
        }

        public void Render()
        {
            switch (m_mode)
            {
                case Mode.BackCamera:
                    ViewMatrix = MySession.PlayerShip.GetViewMatrix() * Matrix.CreateRotationY(MathHelper.Pi);
                    break;
                case Mode.PlayerShip:
                    ViewMatrix = MySession.PlayerShip.GetViewMatrix();
                    break;
                case Mode.Entity:
                    ViewMatrix = Matrix.CreateLookAt(
                        m_cameraSource.GetPosition(),
                        m_cameraSource.GetPosition() +
                        m_cameraSource.WorldMatrix.Forward,
                        m_cameraSource.WorldMatrix.Up);
                        break;
                default:
                    throw new ArgumentOutOfRangeException();
            }            

            SecondaryCameraRenderer.ViewMatrix = ViewMatrix;
            this.IsCurrentlyRendering = true;
            SecondaryCameraRenderer.Render();
            this.IsCurrentlyRendering = false;
            ProjectionMatrix = SecondaryCameraRenderer.ProjectionMatrix;

            BoundingFrustum.Matrix = ViewMatrix * ProjectionMatrix;
        }

        public void DebugDraw()
        {
            MyDebugDraw.DrawBoundingFrustum(BoundingFrustum, Color.Green);
        }
    }
}
