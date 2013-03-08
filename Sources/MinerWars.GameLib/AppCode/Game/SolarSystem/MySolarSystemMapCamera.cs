using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.SolarSystem
{
    class MySolarSystemMapCamera
    {
        Vector3 m_position;
        MyMwcVector3Int m_positionSector;

        Matrix m_baseRotation;

        Vector3 m_target;
        MyMwcVector3Int m_targetSector;

        Vector3 m_cameraToTarget;

        float m_cameraDistance;

        public float CameraDistance
        {
            get { return m_cameraDistance; }
        }

        Vector2 m_rotation;

        public const float SECTOR_SIZE_GAMEUNITS = 0.0001f;

        public float PitchMin = MathHelper.ToRadians(-89.999f);
        public float PitchMax = MathHelper.ToRadians(-10);
        public float MinDistanceToTarget = 16;
        public float MaxDistanceToTarget = 15e6f;

        /// <summary>
        /// Maximal sector number.
        /// For solar system 1000 x 1000 sectors it will be 1000.
        /// </summary>
        public int MaxSector;

        private MySolarSystemMapCamera() { }

        public Vector3 Forward { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Left { get; private set; }

        public MySolarSystemMapCamera(Vector3 target, float distanceFromTarget)
        {
            m_target = target;
            m_cameraDistance = distanceFromTarget;
            m_rotation = new Vector2(MathHelper.ToRadians(-30), 0);
            UpdateRotation();
        }

        public void Move(Vector3 target)
        {
            m_target = target;
            m_targetSector = new MyMwcVector3Int(0, 0, 0);
            UpdateRotation();
        }

        public void MoveToSector(MyMwcVector3Int targetSector)
        {
            var target = new Vector3(targetSector.X * SECTOR_SIZE_GAMEUNITS, targetSector.Y * SECTOR_SIZE_GAMEUNITS, targetSector.Z * SECTOR_SIZE_GAMEUNITS);
            Move(target);
        }

        void Modulo(ref double x, ref double y, ref double z, double modulo)
        {
            x %= modulo;
            y %= modulo;
            z %= modulo;
        }

        /// <summary>
        /// Need to calculate position modulo to move grid.
        /// Everything must be in doubles and each part separate.
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public Vector3 CalculatePositionModulo(double modulo)
        {
            double tx = m_target.X;
            double ty = m_target.Y;
            double tz = m_target.Z;
            Modulo(ref tx, ref ty, ref tz, modulo);

            double tsx = m_targetSector.X;
            double tsy = m_targetSector.Y;
            double tsz = m_targetSector.Z;
            Modulo(ref tsx, ref tsy, ref tsz, modulo / SECTOR_SIZE_GAMEUNITS);
            tsx *= SECTOR_SIZE_GAMEUNITS;
            tsy *= SECTOR_SIZE_GAMEUNITS;
            tsz *= SECTOR_SIZE_GAMEUNITS;

            double rx = m_baseRotation.Forward.X * m_cameraDistance;
            double ry = m_baseRotation.Forward.Y * m_cameraDistance;
            double rz = m_baseRotation.Forward.Z * m_cameraDistance;
            Modulo(ref rx, ref ry, ref rz, modulo);

            double x = rx - (tx + tsx);
            double y = ry - (ty + tsy);
            double z = rz - (tz + tsz);
            Modulo(ref x, ref y, ref z, modulo);

            return new Vector3((float)x, (float)y, (float)z);
        }
        
        /// <summary>
        /// Need to update position from target, camera direction and distance.
        /// Calculation must be done in doubles.
        /// </summary>
        void UpdatePosition()
        {
            m_cameraToTarget = m_baseRotation.Forward * m_cameraDistance;

            // Absolute target position
            double tx = m_targetSector.X * (double)SECTOR_SIZE_GAMEUNITS;
            double ty = m_targetSector.Y * (double)SECTOR_SIZE_GAMEUNITS;
            double tz = m_targetSector.Z * (double)SECTOR_SIZE_GAMEUNITS;

            tx += m_target.X;
            ty += m_target.Y;
            tz += m_target.Z;

            double rx = m_baseRotation.Forward.X * (double)m_cameraDistance;
            double ry = m_baseRotation.Forward.Y * (double)m_cameraDistance;
            double rz = m_baseRotation.Forward.Z * (double)m_cameraDistance;

            double px = tx - rx;
            double py = ty - ry;
            double pz = tz - rz;
            
            m_positionSector.X = (int)(px / (double)SECTOR_SIZE_GAMEUNITS);
            m_positionSector.Y = (int)(py / (double)SECTOR_SIZE_GAMEUNITS);
            m_positionSector.Z = (int)(pz / (double)SECTOR_SIZE_GAMEUNITS);

            m_position.X = (float)(px % (double)SECTOR_SIZE_GAMEUNITS);
            m_position.Y = (float)(py % (double)SECTOR_SIZE_GAMEUNITS);
            m_position.Z = (float)(pz % (double)SECTOR_SIZE_GAMEUNITS);
        }

        /// <summary>
        /// Update rotation AND position
        /// Camera rotation changes position
        /// </summary>
        void UpdateRotation()
        {
            m_rotation.X = MathHelper.Clamp(m_rotation.X, PitchMin, PitchMax);
            m_baseRotation = Matrix.CreateFromYawPitchRoll(m_rotation.Y, m_rotation.X, 0);

            Forward = Vector3.Transform(Vector3.Forward, m_baseRotation);
            Up = Vector3.Transform(Vector3.Up, m_baseRotation);
            Left = Vector3.Transform(Vector3.Left, m_baseRotation);

            // Rotation always updates position, because camera is locked to target and rotates around, therefore it's changing position
            UpdatePosition(); 
        }

        public void Zoom(float zoomIndicator)
        {
            if (zoomIndicator != 0)
            {
                const float zoomMult = -120 * 2;
                zoomIndicator /= zoomMult;

                float val = 1 + Math.Abs(zoomIndicator);

                // zoom in
                if(zoomIndicator < 0)
                {
                    val = 1 / val;
                }
                m_cameraDistance *= val;

                m_cameraDistance = MathHelper.Clamp(m_cameraDistance, MinDistanceToTarget, MaxDistanceToTarget);

                UpdatePosition();
            }
        }

        public Vector3 Position
        {
            get
            {
                return m_position;
            }
        }

        public MyMwcVector3Int PositionSector
        {
            get
            {
                return m_positionSector;
            }
        }

        public Vector3 Target
        {
            get
            {
                return m_target;
            }
        }

        public MyMwcVector3Int TargetSector 
        { 
            get
            {
                return m_targetSector;
            }
        }

        public Vector3 CameraToTarget
        {
            get
            {
                return m_baseRotation.Forward * m_cameraDistance;
            }
        }

        private void UpdateSector(ref Vector3 sectorOffset, ref MyMwcVector3Int sector)
        {
            MyMwcVector3Int offset;
            offset.X = (int)Math.Round(sectorOffset.X / SECTOR_SIZE_GAMEUNITS);
            offset.Y = (int)Math.Round(sectorOffset.Y / SECTOR_SIZE_GAMEUNITS);
            offset.Z = (int)Math.Round(sectorOffset.Z / SECTOR_SIZE_GAMEUNITS);

            sectorOffset.X -= offset.X * SECTOR_SIZE_GAMEUNITS;
            sectorOffset.Y -= offset.Y * SECTOR_SIZE_GAMEUNITS;
            sectorOffset.Z -= offset.Z * SECTOR_SIZE_GAMEUNITS;

            sector.X += offset.X;
            sector.Y += offset.Y;
            sector.Z += offset.Z;
        }

        private void ClampTargetSector()
        {
            if (m_targetSector.X < -MaxSector) m_targetSector.X = -MaxSector;
            if (m_targetSector.Y < -MaxSector) m_targetSector.Y = -MaxSector;
            if (m_targetSector.Z < -MaxSector) m_targetSector.Z = -MaxSector;

            if (m_targetSector.X > MaxSector) m_targetSector.X = MaxSector;
            if (m_targetSector.Y > MaxSector) m_targetSector.Y = MaxSector;
            if (m_targetSector.Z > MaxSector) m_targetSector.Z = MaxSector;
        }

        //  Moves and rotates player by specified vector and angles
        public void MoveAndRotate(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator, float afterburner)
        {
            //  Physical movement and rotation is based on constant time, therefore is indepedent of time delta
            //  This formulas works even if FPS is low or high, or if step size is 1/10 or 1/10000
            float amountOfMovement = MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 1;
            float amountOfRotation = 0.0025f;
            
            if (rotationIndicator.Y != 0)
            {
                m_rotation.Y += rotationIndicator.Y * amountOfRotation;
                UpdateRotation();
            }

            if (rotationIndicator.X != 0)
            {
                m_rotation.X += rotationIndicator.X * amountOfRotation;
                UpdateRotation();
            }
            
            moveIndicator.Y = 0;

            Matrix yRotation = Matrix.CreateFromAxisAngle(Vector3.Up, m_rotation.Y);
            Vector3 moveVector = Vector3.Transform(moveIndicator, yRotation) * amountOfMovement * m_cameraDistance;

            m_position += moveVector;
            m_target += moveVector;

            UpdateSector(ref m_target, ref m_targetSector);
            UpdateSector(ref m_position, ref m_positionSector);
            ClampTargetSector();
        }

        public Matrix GetViewMatrixAtZero()
        {
            return Matrix.CreateLookAt(Vector3.Zero, Forward, Up);
        }

        public Matrix GetProjectionMatrix()
        {
            float nearClip = 0.1f * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            float farClip = 100000000 * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            return Matrix.CreatePerspectiveFieldOfView(MyCamera.FieldOfView, MyCamera.ForwardAspectRatio, nearClip, farClip);
        }
    }
}
