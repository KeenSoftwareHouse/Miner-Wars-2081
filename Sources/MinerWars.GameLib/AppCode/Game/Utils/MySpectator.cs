using System;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Physics.Collisions;

namespace MinerWars.AppCode.Game.Utils
{
    //  Player with movements like 6DOF camera
    static class MySpectator
    {
        static Vector3 m_position;
        public static Vector3 Position
        {
            get { return m_position; }
            set
            {
                MyUtils.AssertIsValid(value);
                m_position = value;
            }
        }

        public static bool ReflectorOn = true;
        public static float SpeedMode
        {
            get { return m_speedMode; }
            set { m_speedMode = value; }
        }

        private static float m_speedMode = MyConstants.DEFAULT_SPECTATOR_SPEED;
        static Matrix m_baseRotation = Matrix.Identity;
        static bool m_rotationEnabled = true;


        //  Direction to which player is looking. Normalized vector.
        public static Vector3 Orientation
        {
            get
            {
                //Matrix cameraRotation = Matrix.CreateRotationX(m_angles.X) * Matrix.CreateRotationY(m_angles.Y);
                return Vector3.Transform(Vector3.Forward, m_baseRotation);
            }
        }

        //  Gets or sets camera's target.
        //  You can set target as point where camera will be looking from it's current position. Angles are calculated automatically.
        public static Vector3 Target
        {
            get
            {
                //Matrix cameraRotation = Matrix.CreateRotationX(m_angles.X) * Matrix.CreateRotationY(m_angles.Y);
                return Position + Vector3.Transform(Vector3.Forward, m_baseRotation);
            }
            set
            {
                //Vector3 forward = MyMwcUtils.Normalize(m_initialSunWindPosition - value);
                Vector3 forward = MyMwcUtils.Normalize(value - Position);
                forward = forward.LengthSquared() > 0 ? forward : Vector3.Forward;

                Vector3 unnormalizedRight = Vector3.Cross(forward, Vector3.Up);
                Vector3 right = unnormalizedRight.LengthSquared() > 0 ? MyMwcUtils.Normalize(unnormalizedRight) : Vector3.Right;

                Vector3 up = MyMwcUtils.Normalize(Vector3.Cross(right, forward));
                
                m_baseRotation = Matrix.Identity;
                m_baseRotation.Forward = forward;
                m_baseRotation.Right = right;
                m_baseRotation.Up = up;
                
                var m = Matrix.CreateLookAt(Position, value, Vector3.Up);
                var mi = Matrix.Invert(m);
                /*
                m_angles.X = -(float)Math.Asin(test.M32);
                m_angles.Y = -(float)Math.Asin(test.M13);
                */
            }
        }

        //  Moves and rotates player by specified vector and angles
        public static void MoveAndRotate(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator, float afterburner)
        {
            /*
            if (rotationIndicator.Length() > 0)
                System.Diagnostics.Debugger.Break();
            if (rotationIndicator.Length() > 0)
                System.Diagnostics.Debugger.Break();
            if (rollIndicator > 0)
                System.Diagnostics.Debugger.Break();
            */

            Vector3 oldPosition = Position;
                /*
            if (MyFakes.MWBUILDER)
            {
                rollIndicator = 0;

                MinerWars.AppCode.Game.GUI.MyGuiInput mi = MinerWars.AppCode.Game.GUI.Core.MyGuiManager.GetInput();
                if (mi.IsAnyCtrlKeyPressed())
                {
                    rotationIndicator.X = 0;
                }
            } */

            moveIndicator *= afterburner * m_speedMode;

            //  Physical movement and rotation is based on constant time, therefore is indepedent of time delta
            //  This formulas works even if FPS is low or high, or if step size is 1/10 or 1/10000
            float amountOfMovement = MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 100;
            float amountOfRotation = 0.0025f;

            if (m_rotationEnabled)
            {

                if (rollIndicator * MathHelper.ToRadians(1) != 0)
                {
                    Vector3 r, u;
                    MyUtils.VectorPlaneRotation(m_baseRotation.Up, m_baseRotation.Right, out u, out r, rollIndicator * MathHelper.ToRadians(1));
                    m_baseRotation.Right = r;
                    m_baseRotation.Up = u;
                }

                if (rotationIndicator.X != 0)
                {
                    Vector3 u,f;
                    MyUtils.VectorPlaneRotation(m_baseRotation.Up, m_baseRotation.Forward, out u, out f, rotationIndicator.X * amountOfRotation);
                    m_baseRotation.Up = u;
                    m_baseRotation.Forward = f;
                }

                if (rotationIndicator.Y != 0)
                {
                    Vector3 r, f;
                    MyUtils.VectorPlaneRotation(m_baseRotation.Right, m_baseRotation.Forward, out r, out f, -rotationIndicator.Y * amountOfRotation);
                   
                    /*
                    if (MyFakes.MWBUILDER)
                    {
                        Matrix rotMatrix = Matrix.CreateRotationY(-rotationIndicator.Y * amountOfRotation);
                        r = Vector3.TransformNormal(m_baseRotation.Right, rotMatrix);
                        f = Vector3.TransformNormal(m_baseRotation.Forward, rotMatrix);
                        Vector3 u = Vector3.TransformNormal(m_baseRotation.Up, rotMatrix);
                        m_baseRotation.Up = u;
                    } */  

                    m_baseRotation.Right = r;
                    m_baseRotation.Forward = f;
                }

               // m_baseRotation = Matrix.Identity;
            }

            Vector3 moveVector = moveIndicator * amountOfMovement;

            
            //dp not allow spectator to move outside sector borders
            //Position += Vector3.Transform(moveVector, Matrix.CreateRotationX(m_angles.X) * Matrix.CreateRotationY(m_angles.Y) * Matrix.CreateRotationZ(m_roll));
            Position += Vector3.Transform(moveVector, m_baseRotation);
            if (!MyFakes.DISABLE_SPECTATOR_IN_BOUNDARIES)
            {
                Position = Vector3.Clamp(Position, -MyMwcSectorConstants.SECTOR_SIZE_VECTOR3 / 2, MyMwcSectorConstants.SECTOR_SIZE_VECTOR3 / 2);
            }
           // Position = Vector3.Zero;
                          /*
            if (MyFakes.MWBUILDER)
            {      
                MyVoxelMap voxelMap = MyVoxelMaps.GetVoxelMaps()[0];
                Position = Vector3.Clamp(Position, voxelMap.WorldAABB.Min, voxelMap.WorldAABB.Max);
                   
                //Let spectator levitate but dont allow him to go inside voxels
                MyLine collLine = new MyLine(Position, new Vector3(Position.X, -10000, Position.Z));
                MyIntersectionResultLineTriangleEx? res = MyEntities.GetIntersectionWithLine(ref collLine, null, null, true, true, true, true, true, IntersectionFlags.DIRECT_TRIANGLES);
                if (res.HasValue)
                {
                    //It is ok, we are above ground
                    if (Vector3.Distance(res.Value.IntersectionPointInWorldSpace, Position) > 20)
                        return;
                }    

                //Snap on the ground
                collLine = new MyLine(new Vector3(Position.X, 10000, Position.Z), new Vector3(Position.X, -10000, Position.Z));
                res = MyEntities.GetIntersectionWithLine(ref collLine, null, null, true, true, true, true, true, IntersectionFlags.DIRECT_TRIANGLES);
               
               // System.Diagnostics.Debug.Assert(res.HasValue);
                if (res.HasValue)
                {
                    Vector3 newPosition = res.Value.IntersectionPointInWorldSpace + new Vector3(0, 20, 0);
                    if (Vector3.Distance(oldPosition, newPosition) < 100)
                        Position = newPosition;
                    else
                        Position = oldPosition;
                }    
            }     */

            //if (MyMinerGame.DisplayDebuggingData == true)
            //{
            //    MyFpsManager.AddToFrameDebugText("Spectator position: " + MyUtils.GetFormatedVector3(m_initialSunWindPosition, 3));
            //}
        }

        public static Matrix GetViewMatrix()
        {
            Vector3 targetPos = Position + Vector3.Transform(Vector3.Forward, m_baseRotation);

            Vector3 upVector = Vector3.Transform(Vector3.Up, m_baseRotation);

            return Matrix.CreateLookAt(Position, targetPos, upVector);
        }

        public static void SetViewMatrix(Matrix viewMatrix)
        {
            MyUtils.AssertIsValid(viewMatrix);
            
            Matrix inverted = Matrix.Invert(viewMatrix);
            Position = inverted.Translation;
            m_baseRotation = Matrix.Identity;
            m_baseRotation.Right = inverted.Right;
            m_baseRotation.Up = inverted.Up;
            m_baseRotation.Forward = inverted.Forward;
        }

        public static void EnableRotation()
        {
            m_rotationEnabled = true;
        }

        public static void DisableRotation()
        {
            m_rotationEnabled = false;
        }

        /// <summary>
        /// Reset position and orientation of spectator view matrix
        /// </summary>
        public static void ResetSpectatorView()
        {
            if (!MyFakes.MWBUILDER)
            {
                Position = Vector3.Zero;
            }
            m_baseRotation = Matrix.Identity;

        }
    }
}
