#region Using Statements

using System;
using System.Collections.Generic;
using MinerWarsMath;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Parallelization;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Rigid body represents the simulation body. It hold the dynamic properties of the body and holds the element (shape) information.
    /// </summary>
    partial class MyRigidBody
    {
        #region members
        private Matrix m_Matrix;

        private Vector3 m_Velocity; //[m/s]
        private Vector3 m_LinearAcceleration; //[m/s^2]
        private Vector3 m_ExternalLinearAcceleration; //[m/s^2]
        private Vector3 m_AngularVelocity; //[rad/s]
        private Vector3 m_AngularAcceleration; //[rad/s^2]
        private Vector3 m_ExternalAngularAcceleration; //[rad/s^2]

        private float m_Mass; //weight [kg]
        private float m_OneOverMass; // 1/weight [kg^-1]
        private Matrix m_InertiaTensor;
        private Matrix m_InvertInertiaTensor;

        private List<MyRBElement> m_RBElementList;

        private IMyContactModifyNotifications m_ContactModify;
        private IMyNotifyContact m_ContactEvent;
        //private MyRigidBodyEvent        m_RigidBodyEvent;        				
        private IMyNotifyMotion m_MotionEvent;

        private RigidBodyFlag m_Flags;

        private bool m_IsSleeping;

        private uint m_IterationCount;

        private ushort m_Type;

        private ushort m_Guid;

        private float m_MaxAngularVelocity;
        private float m_MaxLinearVelocity;
        private float m_SleepEnergyThreshold;
        private float m_LinearDamping;
        private float m_AngularDamping;

        private float m_LastKineticEnergy;
        private float m_DeactivationTimer;

        private static float GLOBAL_DEACTIVATION_TIMER = 0.5f;
        private static ushort GUID_COUNTER = 0;

        public object m_UserData;

        /// <summary>
        /// When body is kinematic, it allows setting position and velocity
        /// </summary>
        public bool KinematicLinear = true;

        #endregion

        #region events
        // public event MyRigidBodyEvent OnActivated;
        //public event MyRigidBodyEvent OnDeactivated;


        public FastNoArgsEvent OnActivated = new FastNoArgsEvent();
        public FastNoArgsEvent OnDeactivated = new FastNoArgsEvent();


        #endregion

        #region interface
        public MyRigidBody()
        {
            m_RBElementList = new List<MyRBElement>(2);

            m_Matrix = new Matrix();

            m_InertiaTensor = Matrix.Identity;

            m_IsSleeping = false;

            m_ContactModify = null;
            m_ContactEvent = null;
            m_MotionEvent = null;
            //m_RigidBodyEvent = null;

            m_IterationCount = MyPhysicsConfig.DefaultIterationCount;

            m_LinearDamping = MyPhysicsConfig.DefaultLinearDamping;
            m_AngularDamping = MyPhysicsConfig.DefaultAngularDamping;

            m_SleepEnergyThreshold = MyPhysicsConfig.DefaultEnergySleepThreshold;

            m_MaxLinearVelocity = -1.0f;
            m_MaxAngularVelocity = -1.0f;

            m_Guid = GUID_COUNTER;
            if (GUID_COUNTER > 65000)
            {
                GUID_COUNTER = 0;
            }
            GUID_COUNTER++;
        }

        /// <summary>
        /// Loads properties of rbo from descriptor
        /// </summary>
        public bool LoadFromDesc(MyRigidBodyDesc desc)
        {
            if ((m_Flags & RigidBodyFlag.RBF_INSERTED) > 0)
                return false;

            if (!desc.IsValid())
                return false;

            m_Mass = desc.m_Mass;
            m_OneOverMass = 1.0f / m_Mass;
            m_Matrix = desc.m_Matrix;

            m_IterationCount = desc.m_IterationCount;
            m_Type = desc.m_Type;

            m_MaxLinearVelocity = desc.m_MaxLinearVelocity;
            m_MaxAngularVelocity = desc.m_MaxAngularVelocity;
            m_SleepEnergyThreshold = desc.m_SleepEnergyThreshold;
            m_LinearDamping = desc.m_LinearDamping;
            m_AngularDamping = desc.m_AngularDamping;

            SetInitialFlags(desc.m_Flags);

            m_InvertInertiaTensor.M11 = MyPhysicsUtils.FLT_MAX;
            m_InvertInertiaTensor.M22 = MyPhysicsUtils.FLT_MAX;
            m_InvertInertiaTensor.M33 = MyPhysicsUtils.FLT_MAX;

            m_Guid = GUID_COUNTER;
            if (GUID_COUNTER > 65000)
            {
                GUID_COUNTER = 0;
            }

            GUID_COUNTER++;

            return true;
        }

        /// <summary>
        /// Activated notification
        /// </summary>
        public void ActivateNotification()
        {
            OnActivated.Raise();
        }

        /// <summary>
        /// Deactivated notification
        /// </summary>
        public void DeactivateNotification()
        {
            OnDeactivated.Raise();
        }

        /// <summary>
        /// Gets or sets the matrix.
        /// </summary>
        /// <value>
        /// The matrix.
        /// </value>
        public Matrix Matrix
        {
            get
            {
                return this.m_Matrix;
            }
            set
            {
                if (ReadFlag(RigidBodyFlag.RBF_KINEMATIC))
                {
                    if (!KinematicLinear)
                    {
                        SetMatrix(value);
                    }
                    m_Velocity = (value.Translation - m_Matrix.Translation) / (MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep);
                }
                else
                {
                    SetMatrix(value);
                }
            }
        }

        public Vector3 Position
        {
            get { return this.m_Matrix.Translation; }
        }

        public bool IsStatic() { return ((m_Flags & RigidBodyFlag.RBF_RBO_STATIC) > 0); }

        public bool IsKinematic() { return ((m_Flags & RigidBodyFlag.RBF_KINEMATIC) > 0); }

        public bool IsSleeping() { return m_IsSleeping; }

        /// <summary>
        /// Wakes up rigid body making it active for collision detection and solver
        /// </summary>
        public void WakeUp()
        {
            if ((m_Flags & RigidBodyFlag.RBF_INSERTED) > 0)
                MyPhysics.physicsSystem.GetRigidBodyModule().AddActiveRigid(this);
            m_IsSleeping = false;
        }

        /// <summary>
        /// Sleeps the rigid body, so its not parsed by solver and collision detection
        /// </summary>
        public void PutToSleep()
        {
            m_IsSleeping = true;
            if ((m_Flags & RigidBodyFlag.RBF_INSERTED) > 0)
                MyPhysics.physicsSystem.GetRigidBodyModule().RemoveActiveRigid(this);
        }

        public Vector3 LinearVelocity { get { return this.m_Velocity; } set { this.SetLinearVelocity(value); } }

        public Vector3 LinearAcceleration
        {
            get { return this.m_LinearAcceleration; }
            set
            {
                MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(value);
                m_LinearAcceleration = value;
                m_ExternalLinearAcceleration = (value);
            }
        }

        public Vector3 ExternalLinearAcceleration
        {
            get { return this.m_ExternalLinearAcceleration; }
            set
            {
                m_ExternalLinearAcceleration = (value);
                MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(value);
            }
        }

        public Vector3 ExternalAngularAcceleration
        {
            get { return this.m_ExternalAngularAcceleration; }
            set
            {
                m_ExternalAngularAcceleration = (value);
                MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(value);
            }
        }

        public Vector3 AngularVelocity { get { return this.m_AngularVelocity; } set { this.SetAngularVelocity(value); } }

        public float MaxAngularVelocity { get { return this.m_MaxAngularVelocity; } set { this.m_MaxAngularVelocity = value; } }

        public float MaxLinearVelocity { get { return this.m_MaxLinearVelocity; } set { this.m_MaxLinearVelocity = value; } }

        public Vector3 AngularAcceleration
        {
            get { return this.m_AngularAcceleration; }
            set
            {
                m_AngularAcceleration = value; m_ExternalAngularAcceleration = (value);
                MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(value);
            }
        }

        public float GetMass() { return this.m_Mass; }

        public float GetOneOverMass()
        {
            return m_OneOverMass;
        }

        public void SetMass(float mass, bool recomputeInertiaTensor)
        {
            System.Diagnostics.Debug.Assert(mass > 0);
            m_Mass = mass;
            m_OneOverMass = 1.0f / m_Mass;
            if (recomputeInertiaTensor)
                MyPhysicsUtils.ComputeIntertiaTensor(this);
        }

        public Matrix InertiaTensor
        {
            get { return this.m_InertiaTensor; }
            set
            {
                this.m_InertiaTensor = value;
                if (m_InertiaTensor.M11 == float.MaxValue)
                {
                    m_InvertInertiaTensor.M11 = MyPhysicsUtils.FLT_MAX;
                    m_InvertInertiaTensor.M22 = MyPhysicsUtils.FLT_MAX;
                    m_InvertInertiaTensor.M33 = MyPhysicsUtils.FLT_MAX;
                    m_InvertInertiaTensor.M44 = 0;
                }
                else
                    m_InvertInertiaTensor = Matrix.Invert(m_InertiaTensor);
                MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(m_InvertInertiaTensor);
            }
        }
        public Matrix InvertInertiaTensor { get { return this.m_InvertInertiaTensor; } set { this.m_InvertInertiaTensor = value; } }

        /// <summary>
        /// Apply world space force to the center of mass
        /// </summary>
        public void ApplyForce(Vector3 force)
        {
            if (IsStatic() || IsKinematic())
                return;

            m_ExternalLinearAcceleration += force * (1 / m_Mass);

            WakeUp();
        }

        /// <summary>
        /// Apply world space force to the given world space position
        /// </summary>
        public void ApplyForce(Vector3 force, Vector3 pos)
        {
            if (IsStatic() || IsKinematic())
                return;

            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(force);
            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(pos);

            Vector3 dAngAccel = Vector3.Cross(pos - m_Matrix.Translation, force);
            dAngAccel = Vector3.Transform(dAngAccel, m_InvertInertiaTensor);

            m_ExternalLinearAcceleration += force * (1 / m_Mass);
            m_ExternalAngularAcceleration += dAngAccel;

            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(m_ExternalAngularAcceleration);

            WakeUp();
        }

        /// <summary>
        /// Apply world space impulse to the given world space position
        /// </summary>
        public void ApplyImpulse(Vector3 impulse, Vector3 pos)
        {
            if (IsStatic() || IsKinematic())
                return;

            Vector3 dAngVel = new Vector3();
            Vector3 cpos = pos - m_Matrix.Translation;
            Vector3.Cross(ref cpos, ref impulse, out dAngVel);
            Matrix invInt = m_InvertInertiaTensor;
            dAngVel = Vector3.Transform(dAngVel, m_InvertInertiaTensor);
            m_Velocity += impulse * (1 / m_Mass);
            m_AngularVelocity += dAngVel;

            WakeUp();
        }

        /// <summary>
        /// Apply world space torgue
        /// </summary>
        public void ApplyTorque(Vector3 torque)
        {
            if (IsStatic() || IsKinematic())
                return;

            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(torque);

            Vector3.Transform(ref torque, ref m_InvertInertiaTensor, out torque);
            Vector3.Add(ref m_ExternalAngularAcceleration, ref torque, out m_ExternalAngularAcceleration);

            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(m_ExternalAngularAcceleration);

            WakeUp();
        }

        /// <summary>
        /// Type is used for contact report filtering
        /// </summary>
        public ushort Type { get { return this.m_Type; } set { MyCommonDebugUtils.AssertDebug(value < 32); this.m_Type = value; } }

        public ushort GUID { get { return this.m_Guid; } }

        /// <summary>
        /// Number of iterations for the rigid body, the more the more precise simulation
        /// </summary>
        public uint IterationCount { get { return this.m_IterationCount; } set { if (value == 0) MyCommonDebugUtils.AssertDebug(false); this.m_IterationCount = value; } }

        /// <summary>
        /// Under given threshold the rbo is put to sleep
        /// </summary>
        public float SleepEnergyThreshold { get { return this.m_SleepEnergyThreshold; } set { this.m_SleepEnergyThreshold = value; } }

        public float LinearDamping { get { return this.m_LinearDamping; } set { this.m_LinearDamping = value; } }

        public float AngularDamping { get { return this.m_AngularDamping; } set { this.m_AngularDamping = value; } }

        public RigidBodyFlag GetFlags() { return m_Flags; }

        public void RaiseFlag(RigidBodyFlag flag)
        {
            if ((flag & RigidBodyFlag.RBF_ACTIVE) > 0)
                m_DeactivationTimer = GLOBAL_DEACTIVATION_TIMER;

            m_Flags |= flag;
        }

        public bool ReadFlag(RigidBodyFlag flags) { return (flags & m_Flags) != 0; }

        public void ClearFlag(RigidBodyFlag flag)
        {
            m_Flags &= ~flag;
        }

        /// <summary>
        /// Adds element to the rigid body, you have to specify if you want to compute the inertia tensor, yes in case you will not insert another one or change mass
        /// </summary>
        public bool AddElement(MyRBElement element, bool recomputeInertia)
        {
            m_RBElementList.Add(element);
            element.SetRigidBody(this);

            if ((m_Flags & RigidBodyFlag.RBF_INSERTED) > 0)
            {
                MyPhysics.physicsSystem.GetRigidBodyModule().GetBroadphase().CreateVolume(element);
            }

            if (recomputeInertia)
                MyPhysicsUtils.ComputeIntertiaTensor(this);
            return true;
        }

        /// <summary>
        /// Removes element to the rigid body, you have to specify if you want to compute the inertia tensor, yes in case you will not remove another one or change mass
        /// </summary>
        public void RemoveElement(MyRBElement element, bool recomputeInertia)
        {
            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();

            for (int i = 0; i < m_RBElementList.Count; i++)
            {
                if (m_RBElementList[i] == element)
                {
                    element.SetRigidBody(null);
                    physobj.RemoveRBElement(element);
                    m_RBElementList.RemoveAt(i);
                    break;
                }
            }

            if ((m_Flags & RigidBodyFlag.RBF_INSERTED) > 0)
            {
                MyPhysics.physicsSystem.GetRigidBodyModule().GetBroadphase().DestroyVolume(element);

            }

            if (recomputeInertia && m_RBElementList.Count > 0)
                MyPhysicsUtils.ComputeIntertiaTensor(this);
        }

        public void RemoveAllElements()
        {
            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();

            for (int i = m_RBElementList.Count - 1; i >= 0; i--)
            {
                if ((m_Flags & RigidBodyFlag.RBF_INSERTED) > 0)
                {
                    MyPhysics.physicsSystem.GetRigidBodyModule().GetBroadphase().DestroyVolume(m_RBElementList[i]);
                }

                m_RBElementList[i].SetRigidBody(null);
                physobj.RemoveRBElement(m_RBElementList[i]);
                m_RBElementList.RemoveAt(i);
            }
        }

        public List<MyRBElement> GetRBElementList() { return m_RBElementList; }

        public IMyNotifyContact NotifyContactHandler { get { return this.m_ContactEvent; } set { this.m_ContactEvent = value; } }

        //public MyRigidBodyEvent RigidBodyEventHandler { get { return this.m_RigidBodyEvent; } set { this.m_RigidBodyEvent = value; }}

        public IMyNotifyMotion NotifyMotionHandler { get { return this.m_MotionEvent; } set { this.m_MotionEvent = value; } }

        public IMyContactModifyNotifications ContactModifyNotificationHandler { get { return m_ContactModify; } set { m_ContactModify = value; } }

        #endregion

        #region implementation

        public void Clear()
        {
            m_Velocity = Vector3.Zero;
            m_AngularVelocity = Vector3.Zero;
        }

        private void SetLinearVelocity(Vector3 vel)
        {
            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(vel);

            if ((ReadFlag(RigidBodyFlag.RBF_KINEMATIC) && KinematicLinear) || ReadFlag(RigidBodyFlag.RBF_RBO_STATIC))
            {
                return;
            }

            if (MyMwcUtils.IsZero(m_Velocity - vel))
                return;

            m_Velocity = vel;
            WakeUp();
        }

        private void SetAngularVelocity(Vector3 vel)
        {
            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(vel);

            if (ReadFlag(RigidBodyFlag.RBF_KINEMATIC) || ReadFlag(RigidBodyFlag.RBF_RBO_STATIC))
            {
                return;
            }

            if (MyMwcUtils.IsZero(m_AngularVelocity - vel))
                return;

            m_AngularVelocity = vel;
            WakeUp();
        }

        private void SetInitialFlags(RigidBodyFlag flags)
        {
            m_Flags = flags;
        }

        /// <summary>
        /// Internal set, calculation of kinematic speed is handled in public property set.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        internal void SetMatrix(Matrix matrix)
        {
            m_Matrix = matrix;

            foreach (var el in GetRBElementList())
            {
                el.Flags |= MyElementFlag.EF_AABB_DIRTY;
                MyPhysics.physicsSystem.GetRigidBodyModule().GetBroadphase().MoveVolumeFast(el);
            }

            if (IsStatic()) //We dont want to update static object (performance)
            {
                return;
            }

            m_Flags |= RigidBodyFlag.RBF_DIRTY;
            WakeUp();
        }

        /*
        private void SetPosition(Vector3 pos)
        {
            if (ReadFlag(RigidBodyFlag.RBF_KINEMATIC))
            {
                m_Velocity = (pos - m_Matrix.Translation) / (MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep);
            }

            m_Matrix.Translation = pos;
            m_Flags |= RigidBodyFlag.RBF_DIRTY;
            WakeUp();
        } */

        public void GetLocalPointVelocity(ref Vector3 relPos, out Vector3 result)
        {
            result.X = m_Velocity.X + m_AngularVelocity.Y * relPos.Z - m_AngularVelocity.Z * relPos.Y;
            result.Y = m_Velocity.Y + m_AngularVelocity.Z * relPos.X - m_AngularVelocity.X * relPos.Z;
            result.Z = m_Velocity.Z + m_AngularVelocity.X * relPos.Y - m_AngularVelocity.Y * relPos.X;
        }

        public void GetGlobalPointVelocity(ref Vector3 globalPos, out Vector3 result)
        {
            Vector3 relPos = globalPos - m_Matrix.Translation;
            result.X = m_Velocity.X + m_AngularVelocity.Y * relPos.Z - m_AngularVelocity.Z * relPos.Y;
            result.Y = m_Velocity.Y + m_AngularVelocity.Z * relPos.X - m_AngularVelocity.X * relPos.Z;
            result.Z = m_Velocity.Z + m_AngularVelocity.X * relPos.Y - m_AngularVelocity.Y * relPos.X;
        }


        public void SetGroupMask(MyGroupMask grMask)
        {
            for (int i = 0; i < m_RBElementList.Count; i++)
            {
                m_RBElementList[i].GroupMask = grMask;
            }
        }

        /// <summary>
        /// Updates velocity from external accel and gravitation 
        /// </summary>=
        public void UpdateVelocity(float dt)
        {
            // apply directional gravity
            m_ExternalLinearAcceleration += MyPhysics.physicsSystem.Gravitation * dt;

            // apply point gravity
            Vector3 accelerationFromPoints = Vector3.Zero;
            foreach (var gravityPoint in MyPhysics.physicsSystem.GravitationPoints)
            {
                float distance = Vector3.Distance(Position, gravityPoint.Item1.Center);
                float power = MathHelper.Clamp(1 - distance / gravityPoint.Item1.Radius, 0, 1);
                Vector3 dirToCenter = Vector3.Normalize(gravityPoint.Item1.Center - Position);
                accelerationFromPoints += dirToCenter * power * gravityPoint.Item2;
            }

            m_ExternalLinearAcceleration += accelerationFromPoints * dt;


            m_Velocity += m_ExternalLinearAcceleration * dt;

            m_AngularVelocity += m_ExternalAngularAcceleration * dt;

            if (m_MaxAngularVelocity > 0.0f && m_AngularVelocity.Length() > m_MaxAngularVelocity)
            {
                m_AngularVelocity = MyMwcUtils.Normalize(m_AngularVelocity);
                m_AngularVelocity *= m_MaxAngularVelocity;
            }

            if (m_MaxLinearVelocity > 0.0f && m_Velocity.Length() > m_MaxLinearVelocity)
            {
                m_Velocity = MyMwcUtils.Normalize(m_Velocity);
                m_Velocity *= m_MaxLinearVelocity;
            }

        }


        static readonly float SleepAngularVelocityThreshold = new Vector3(2.0f).LengthSquared();
        /// <summary>
        /// Applies damping to reduce speeds
        /// </summary>=
        public void ApplyDamping(float dt)
        {
            float ne = GetKineticEnergy() * m_OneOverMass;
            float ld = (1.0f - m_LinearDamping);
            float ad = (1.0f - m_AngularDamping);

            if ((ne >= m_SleepEnergyThreshold || ne > m_LastKineticEnergy) || AngularVelocity.LengthSquared() > SleepAngularVelocityThreshold)
            {
                m_DeactivationTimer += dt;
                m_DeactivationTimer = m_DeactivationTimer < GLOBAL_DEACTIVATION_TIMER ? m_DeactivationTimer : GLOBAL_DEACTIVATION_TIMER;
            }
            else
            {
                m_DeactivationTimer -= dt;
                m_DeactivationTimer = m_DeactivationTimer < 0.0f ? 0.0f : m_DeactivationTimer;

                /*  Do we really need this, when default AD is set?
			    //Extra damping
			    float d = m_DeactivationTimer / GLOBAL_DEACTIVATION_TIMER;
			    d = d < 0.01 ? 0.01f : d;

			    //Damping
			    float d2 = d*d;

			    ld = ld * d2;
			    ad = ad * d2;*/
            }

            m_Velocity = m_Velocity * (float)Math.Pow(ld, dt);
            m_AngularVelocity = m_AngularVelocity * (float)Math.Pow(ad, dt);
            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(m_AngularVelocity);

            m_LastKineticEnergy = ne;

            if (m_AngularVelocity.LengthSquared() < (0.001f * 0.001f))
                m_AngularVelocity = Vector3.Zero;

            if (m_Velocity.LengthSquared() < (0.02f * 0.02f))
                m_Velocity = Vector3.Zero;
        }

        /// <summary>
        /// Updates matrix from velocities used by old solver
        /// </summary>=
        public void UpdateMatrix(float dt)
        {
            m_LinearAcceleration = Vector3.Zero;
            m_AngularAcceleration = Vector3.Zero;

            m_ExternalAngularAcceleration = Vector3.Zero;
            m_ExternalLinearAcceleration = Vector3.Zero;

            Vector3 translation = m_Matrix.Translation + m_Velocity * dt;

            m_Matrix.Translation = Vector3.Zero;

            Vector3 dir = AngularVelocity;
            float ang = dir.Length();

            if (ang > 0.0f)
            {
                Vector3.Divide(ref dir, ang, out dir);  // dir /= ang;
                ang *= dt;
                Matrix rot;
                Matrix.CreateFromAxisAngle(ref dir, ang, out rot);
                Matrix.Multiply(ref m_Matrix, ref rot, out m_Matrix);
            }

            MyPhysicsUtils.Orthonormalise(ref m_Matrix);

            m_Matrix.Translation = translation;

            m_LinearAcceleration = Vector3.Zero;
            m_AngularAcceleration = Vector3.Zero;
        }

        public float GetKineticEnergy()
        {
            if (IsStatic())
                return 0.0f;

            float le = m_Velocity.LengthSquared() * m_Mass;
            Matrix tempM = m_Matrix;
            tempM.Translation = Vector3.Zero;

            Matrix tempM2 = tempM;
            tempM = MinerWarsMath.Matrix.Transpose(tempM);
            tempM = tempM * m_InvertInertiaTensor * tempM2;
            Vector3 angV = Vector3.Transform(m_AngularVelocity, tempM);
            float ae = Vector3.Dot(m_AngularVelocity, angV);

            return (le + ae) * 0.5f;
        }

        /// <summary>
        /// Checks if rigid can or cannot be deactivated
        /// </summary>=
        public bool CanDeactivate()
        {
            if (IsStatic())
                return true;

            if (m_DeactivationTimer <= 0.0f)
                return true;
            else
                return false;
        }

        #endregion
    }
}