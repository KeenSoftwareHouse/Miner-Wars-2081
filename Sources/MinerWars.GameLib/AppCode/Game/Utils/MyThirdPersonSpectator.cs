using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.GUI;
using MinerWarsMath.Graphics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Render;
namespace MinerWars.AppCode.Game.Utils
{
    static class MyThirdPersonSpectator
    {
        // Minimum distance camera-ship (also used for quick zoom)
        public static readonly float DEFAULT_MIN_DISTANCE = 7.0f;

        // Minimum distance camera-ship (also used for quick zoom)
        public static readonly float MAX_DISTANCE = 200.0f;

        public static readonly float HEADSHAKE_POWER = 0.5f;

        public static readonly float COLLISION_STEP = 0.05f;

        public static readonly float CAMERA_RADIUS = 1.0f;

        public static readonly float BACKWARD_CUTOFF = 3.0f;

        #region Definitions
        // Helper class for spring physics parameters
        // Critical damping = 2*sqrt(Stiffness * Mass)
        public class SpringInfo
        {
            // Spring physics properties
            public float Stiffness;
            public float Damping;
            public float Mass;

            public SpringInfo(float stiffness, float damping, float mass)
            {
                Stiffness = stiffness;
                Damping = damping;
                Mass = mass;
            }

            public SpringInfo(SpringInfo spring)
            {
                Setup(spring);
            }

            public void Setup(SpringInfo spring)
            {
                Stiffness = spring.Stiffness;
                Damping = spring.Damping;
                Mass = spring.Mass;
            }

            public void Setup(SpringInfo a, SpringInfo b, float springChangeTime)
            {
                Stiffness = MathHelper.SmoothStep(a.Stiffness, b.Stiffness, springChangeTime);
                Damping = MathHelper.SmoothStep(a.Damping, b.Damping, springChangeTime);
                Mass = MathHelper.SmoothStep(a.Mass, b.Mass, springChangeTime);
            }
        }
        #endregion

        // Quick zoom
        public static bool QuickZoom;

        // Vector defining position between Target and Spectator
        public static Vector3 LookAt { get; set; }

        public static Vector3 Target;
        public static Matrix TargetOrientation;

        // Current spectator position
        public static Vector3 Position;
        public static Vector3 DesiredPosition;

        // Spring physics properties
        public static SpringInfo NormalSpring;
        public static SpringInfo StrafingSpring;
        public static SpringInfo AngleSpring;

        // Desired spring parameters
        private static SpringInfo m_targetSpring;

        private static float m_springChangeTime;
        // Current spring parameters, we interpolate values in case StrafingSpring->NormalSpring
        private static SpringInfo m_currentSpring;

        private static Vector3 m_velocity;
        private static float m_angleVelocity;
        private static Quaternion m_orientation;
        private static Matrix m_orientationMatrix;

        static MyThirdPersonSpectator()
        {
            LookAt = MySession.Is25DSector ? new Vector3(0, 130, 65) : new Vector3(0, 20, 50);

            NormalSpring = new SpringInfo(10000, 1414, 50);
            StrafingSpring = new SpringInfo(36000, 2683, 50);
            AngleSpring = new SpringInfo(30, 14.5f, 2);

            m_targetSpring = NormalSpring;
            m_currentSpring = new SpringInfo(NormalSpring);
        }

        // Handles interpolation of spring parameters
        private static void UpdateCurrentSpring()
        {
            if (m_targetSpring == StrafingSpring)
            {
                m_currentSpring.Setup(m_targetSpring);
            }
            else if (m_targetSpring == NormalSpring)
            {
                m_currentSpring.Setup(StrafingSpring, NormalSpring, m_springChangeTime);
            }
            m_springChangeTime += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
        }

        // Initialization required for proper spring function
        public static void Init(Vector3 desiredPosition, Matrix orientation)
        {
            DesiredPosition = desiredPosition;
            Position = desiredPosition;
            m_velocity = Vector3.Zero;
            m_orientation = Quaternion.CreateFromRotationMatrix(orientation);
            m_orientationMatrix = orientation;
        }

        // Updates spectator position (spring connected to desired position)
        public static void Update()
        {
            UpdateCurrentSpring();

            Vector3 transformedLookAt = Vector3.Transform(LookAt, TargetOrientation);
            DesiredPosition = Target + transformedLookAt;

            // Calculate spring force
            Vector3 stretch = Position - DesiredPosition;
            Vector3 force = -m_currentSpring.Stiffness * stretch - m_currentSpring.Damping * m_velocity;

            // Apply acceleration
            Vector3 acceleration = force / m_currentSpring.Mass;
            m_velocity += acceleration * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            // Apply velocity
            Position += m_velocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            if (MySession.Is25DSector)
			{
            	Position = DesiredPosition;
			}

            // Limit backward distance from target
            float backward = Vector3.Dot(TargetOrientation.Backward, Target - Position);
            if (backward > -BACKWARD_CUTOFF)
            {
                Position += TargetOrientation.Backward * (backward + BACKWARD_CUTOFF);
            }

            // Roll spring
            Quaternion targetOrientation = Quaternion.CreateFromRotationMatrix(TargetOrientation);

            // Computes angle difference between current and target orientation
            var angleDifference = (float)Math.Acos(MathHelper.Clamp(Quaternion.Dot(m_orientation, targetOrientation), -1, 1));
            // Normalize angle
            angleDifference = angleDifference > MathHelper.PiOver2 ? MathHelper.Pi - angleDifference : angleDifference;

            if (MySession.Is25DSector)
			{
	            angleDifference = 0;
            	m_angleVelocity = 0;
			}

            // Compute spring physics
            float angleForce = -AngleSpring.Stiffness * angleDifference - AngleSpring.Damping * m_angleVelocity;
            m_angleVelocity += angleForce / AngleSpring.Mass * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            if (angleDifference > 0)
            {
                float factor = Math.Abs(m_angleVelocity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS / angleDifference);
                if (angleDifference > MathHelper.PiOver4)
                {
                    factor = Math.Max(factor, 1.0f - MathHelper.PiOver4/angleDifference);
                }
                factor = MathHelper.Clamp(factor, 0, 1);
                m_orientation = Quaternion.Slerp(m_orientation, targetOrientation, factor);
                m_orientationMatrix = Matrix.CreateFromQuaternion(m_orientation);
            }
        }

        public static void HandleIntersection(MyEntity ship, bool shakeActive, Vector3 headPosition, Vector3 headDirection)
        {
            // Direction vector
            Vector3 direction = Target - Position;
            direction.Normalize();

            // Compute line collision - Current Position <-> Target
            var line = new MyLine(Target, Position, true);
            var result = MyEntities.GetIntersectionWithLine(ref line, MySession.PlayerShip, null, true, true);
            float distance;

            if (result.HasValue)
            {
                distance = result.Value.Triangle.Distance;
                float t = distance / line.Length;
                Position = Target + (Position - Target) * t;
            }
            else
            {
                distance = (Position - Target).Length();
            }


            // Move closer near plane of camera is in collision with enviroment (approximation with bounding sphere)
            Vector3 currentPosition = Position;
            Vector3 lastBadPosition = Position;
            int stepCount = 4;
            for (int stepSize = stepCount; stepSize >= 1; stepSize--)
            {
                float collisionStep = COLLISION_STEP * (1 << (stepSize - 1));
                bool distanceChanged = false;
                while (distance > MySession.PlayerShip.WorldVolume.Radius /*GetMinDistance()*/)
                {
                    // Headshake is based on distance
                    Vector3 shakePosition = currentPosition;
                    if (shakeActive)
                    {
                        shakePosition = currentPosition + Vector3.Transform(headPosition, TargetOrientation) * distance * HEADSHAKE_POWER;
                    }

                    // Test current camera position for near plane collision
                    var cameraBS = new BoundingSphere(shakePosition, CAMERA_RADIUS);
                    MyEntity entity = MyEntities.GetIntersectionWithSphere(ref cameraBS, ship, null);
                    if (entity == null)
                    {
                        break;
                    }

                    // Move closer (Measure distance from ship to avoid camera shake)
                    lastBadPosition = currentPosition;
                    currentPosition = Target - direction * (int)((distance - COLLISION_STEP / 10) / collisionStep) * collisionStep;
                    distance = (currentPosition - Target).Length();
                    distanceChanged = true;
                }

                if (!distanceChanged && stepSize == stepCount)
                {
                    break;
                }

                if (distanceChanged && stepSize > 1)
                {
                    currentPosition = lastBadPosition;
                    distance = (currentPosition - Target).Length();
                }
            }

            float minDistance = MySession.PlayerShip.WorldVolume.Radius;
            /*
            if (distance < minDistance)
            {
                Position = Target - minDistance * direction;
            }
            else*/
            {
                Position = currentPosition;
            }
        }

        public static Matrix GetViewMatrix(float fov, float zoomLevel, bool shakeActive, Vector3 headPosition, Vector3 headDirection)
        {
            Vector3 position = QuickZoom ? (Target + Vector3.Transform(LookAt, TargetOrientation) / LookAt.Length() * GetMinDistance()) : Position;
            Matrix orientation = QuickZoom ? TargetOrientation : m_orientationMatrix;

            float distance = (Target - position).Length();

            // Push ship down (crosshair approx in middle of screen)
            float shipVerticalShift = (float)Math.Tan(fov / 2) * 0.6f * distance;
            Vector3 lookVector = Target + orientation.Up * shipVerticalShift - position;

            float zoomPhase = MathHelper.Clamp((1.0f - zoomLevel) * 4, 0, 1);

            if (zoomLevel != 1)
            {
                // Normalize directions for more linear interpolation
                Vector3 lookDirection = Vector3.Normalize(lookVector);
                Vector3 crosshairDirection = Vector3.Normalize(GetCrosshair() - position);
                lookVector = Vector3.Lerp(lookDirection, crosshairDirection, zoomPhase);
            }

            // Apply headshake
            if (shakeActive)
            {
                position += Vector3.Transform(headPosition, orientation) * distance * HEADSHAKE_POWER;
                Matrix matrixRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, headDirection.Z) * Matrix.CreateFromAxisAngle(Vector3.Right, headDirection.X);
                lookVector = Vector3.Transform(lookVector, matrixRotation);
            }

            return Matrix.CreateLookAt(position, position + lookVector, TargetOrientation.Up);
        }

        // Returns squared distance between spectator position and chased target
        public static float GetTargetDistanceSquared()
        {
            return (Position - Target).LengthSquared();
        }

        // Sets inner state (like target spring properties) which depends on ship movement type i.e. strafe
        public static void SetState(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator)
        {
            if (rollIndicator < float.Epsilon &&
                (Math.Abs(moveIndicator.X) > float.Epsilon || Math.Abs(moveIndicator.Y) > float.Epsilon) &&
                Math.Abs(moveIndicator.Z) < float.Epsilon)
            {
                if (m_targetSpring != StrafingSpring)
                {
                    m_springChangeTime = 0;
                    m_targetSpring = StrafingSpring;
                }
            }
            else if (m_targetSpring != NormalSpring)
            {
                m_springChangeTime = 0;
                m_targetSpring = NormalSpring;
            }
        }

        // Returns 3D crosshair position
        public static Vector3 GetCrosshair()
        {
            return Target + TargetOrientation.Forward * 25000;
        }

        // Returns spectator orientation
        public static Matrix GetOrientation()
        {
            return m_orientationMatrix;
        }

        public static float GetMinDistance()
        {
            return MySession.PlayerShip != null ? MySession.PlayerShip.WorldVolume.Radius * 1.3f : MyThirdPersonSpectator.DEFAULT_MIN_DISTANCE;
        }
    }
}
