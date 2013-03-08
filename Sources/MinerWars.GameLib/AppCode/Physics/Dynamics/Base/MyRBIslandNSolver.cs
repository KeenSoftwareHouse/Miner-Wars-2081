
#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Trace;
#endregion


namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Simulation solver for the rigid body island. This solver is collecting constraints for given rigids and in substeps iterates the bodies and constraints
    /// </summary>

    class MyRBIslandNSolver : ParallelTasks.IWork
    {
        public ParallelTasks.WorkOptions Options { get { return new ParallelTasks.WorkOptions() { MaximumThreads = 1 }; } }
        
        private const int m_defaultMaxCollidingElements = 16;

        public MyRBIslandNSolver()
        {
            m_Island = null;
            m_SolverConstraints = new List<MyRBSolverConstraint>(m_defaultMaxCollidingElements);
            //m_SolverConstaintPool = new MyObjectsPool<MyRBSolverConstraint>(m_defaultMaxCollidingElements);
            m_SolverConstaintPool = new MyDynamicObjectPool<MyRBSolverConstraint>(m_defaultMaxCollidingElements);
            m_SolverBodies = new Dictionary<MyRigidBody, MyRBSolverBody>(m_defaultMaxCollidingElements);
            //m_SolverBodiesPool = new MyObjectsPool<MyRBSolverBody>(m_defaultMaxCollidingElements);
            m_SolverBodiesPool = new MyDynamicObjectPool<MyRBSolverBody>(m_defaultMaxCollidingElements);
        }

        public void Clear()
        {
            m_Island = null;
            foreach (MyRBSolverConstraint rbSolverConstraint in m_SolverConstraints)
            {
                m_SolverConstaintPool.Deallocate(rbSolverConstraint);
            }
            m_SolverConstraints.Clear();
            //m_SolverConstaintPool.DeallocateAll();

            foreach (KeyValuePair<MyRigidBody, MyRBSolverBody> rbSolverBodyKvp in m_SolverBodies)
            {
                rbSolverBodyKvp.Value.m_RigidBody = null;
                m_SolverBodiesPool.Deallocate(rbSolverBodyKvp.Value);
            }
            m_SolverBodies.Clear();
            //m_SolverBodiesPool.DeallocateAll();
        }

        public void SetRBIsland(MyRigidBodyIsland island)
        {
            m_Island = island;
        }

        //public override void Execute()
        public void DoWork()
        {
            float dt = MyPhysics.physicsSystem.GetRigidBodyModule().CurrentTimeStep;

            UpdateVelocities(dt);

            PrepareRigidBodies();

            PrepareConstraints();

            Solve(dt);

            UpdatePositions(dt);

            Clear();
        }

        /// <summary>
        /// Updates velocities of rigids from external forces and gravitation
        /// </summary>
        private void UpdateVelocities(float dt)
        {
            foreach (var rbo in m_Island.GetRigids())
            {
                if (!rbo.IsStatic() && !rbo.IsKinematic())
                    rbo.UpdateVelocity(dt);
            }

        }

        /// <summary>
        /// Updates rigid body position after solver has computed new velocities
        /// </summary>
        private void UpdatePositions(float dt)
        {
            foreach (KeyValuePair<MyRigidBody, MyRBSolverBody> kvp in m_SolverBodies)
            {
                MyRBSolverBody body = kvp.Value;
                MyRigidBody rbo = kvp.Key;


                if (body.m_State == MyRBSolverBody.SolverBodyState.SBS_Dynamic)
                {
                    rbo.LinearAcceleration = rbo.ExternalLinearAcceleration;
                    rbo.AngularAcceleration = rbo.ExternalAngularAcceleration;

                    rbo.ExternalAngularAcceleration = Vector3.Zero;
                    rbo.ExternalLinearAcceleration = Vector3.Zero;

#if PHYSICS_CHECK
                    MyCommonDebugUtils.AssertDebug(float.IsNaN(body.m_LinearVelocity.X) == false);
                    MyCommonDebugUtils.AssertDebug(float.IsNaN(body.m_Matrix.Translation.X) == false);
#endif

                    rbo.LinearVelocity = body.m_LinearVelocity;
                    rbo.AngularVelocity = body.m_AngularVelocity;

                    /*Vector3 pos, scale, scale2;Quaternion rot;
                    rbo.Matrix.Decompose(out scale, out rot, out pos);
                    body.m_Matrix.Decompose(out scale2, out rot, out pos);
                    rbo.Matrix = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rot) * Matrix.CreateTranslation(pos);*/
                    rbo.Matrix = body.m_Matrix;

                    rbo.ApplyDamping(dt);

                    foreach (var el in rbo.GetRBElementList())
                    {
                        el.UpdateAABB();
                    }
                }

                if (body.m_State == MyRBSolverBody.SolverBodyState.SBS_Kinematic)
                {
                    rbo.LinearAcceleration = Vector3.Zero;
                    rbo.AngularAcceleration = Vector3.Zero;

                    rbo.ExternalAngularAcceleration = Vector3.Zero;
                    rbo.ExternalLinearAcceleration = Vector3.Zero;

                    rbo.LinearVelocity = body.m_LinearVelocity;
                    rbo.AngularVelocity = body.m_AngularVelocity;

                    rbo.SetMatrix(body.m_Matrix);
                }
            }

            for (int i = 0; i < m_SolverConstraints.Count; i++)
            {
                MyRBSolverConstraint sc = m_SolverConstraints[i];

                if (sc.m_Magnitude != 0.0f)
                {
                    sc.m_RBConstraint.m_Magnitude = sc.m_Magnitude;
                }
            }
        }

        /// <summary>
        /// collects all rigid bodies 
        /// </summary>
        private void PrepareRigidBodies()
        {
            for (int j = 0; j < m_Island.GetRigids().Count; j++)
            {
                MyRigidBody rbo = m_Island.GetRigids()[j];
                AddRigidBody(rbo);
            }
        }

        /// <summary>
        /// Collects all constraints
        /// </summary>
        private void PrepareConstraints()
        {
            var cl = MyPhysics.physicsSystem.GetContactConstraintModule().GetActiveRBContactConstraints();

            for (int i = 0; i < cl.Count; i++)
            {
                MyRBContactConstraint cc = cl[i];
                MyRigidBody testRbo = cc.GetRBElementInteraction().GetRigidBody1();
                if (testRbo.IsStatic() || testRbo.IsKinematic())
                    testRbo = cc.GetRBElementInteraction().GetRigidBody2();

                for (int j = 0; j < m_Island.GetRigids().Count; j++)
                {
                    MyRigidBody rbo = m_Island.GetRigids()[j];
                    if (rbo == testRbo)
                    {
                        AddConstraint(cc);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a solver body from a rbo and prepares solver data
        /// </summary>
        private MyRBSolverBody AddRigidBody(MyRigidBody rbo)
        {
            MyRBSolverBody sb = m_SolverBodiesPool.Allocate();
            sb.Clear();
            sb.m_RigidBody = rbo;
            sb.m_Matrix = rbo.Matrix;
            sb.m_LinearVelocity = rbo.LinearVelocity;
            sb.m_AngularVelocity = rbo.AngularVelocity;
            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(rbo.AngularVelocity);

            sb.m_LinearAcceleration = rbo.ExternalLinearAcceleration;
            sb.m_AngularAcceleration = rbo.ExternalAngularAcceleration;
            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(rbo.ExternalAngularAcceleration);
            sb.m_MaxAngularVelocity = rbo.MaxAngularVelocity;
            sb.m_MaxLinearVelocity = rbo.MaxLinearVelocity;
            if (rbo.IsStatic() || rbo.IsKinematic())
            {
                sb.m_OneOverMass = 0;
                sb.m_InertiaTensor = MyPhysicsUtils.ZeroInertiaTensor;
                sb.m_InvertedInertiaTensor = MyPhysicsUtils.ZeroInertiaTensor;
                if (rbo.IsStatic())
                {
                    sb.m_State = MyRBSolverBody.SolverBodyState.SBS_Static;
                }
                else
                {
                    sb.m_State = MyRBSolverBody.SolverBodyState.SBS_Kinematic;
                }
            }
            else
            {
                sb.m_OneOverMass = rbo.GetOneOverMass();

                Matrix matrix = rbo.Matrix;
                matrix.Translation = Vector3.Zero;

                sb.m_InertiaTensor = Matrix.Transpose(matrix) * rbo.InertiaTensor * matrix;
                // not sure if I can use directly inverted
                sb.m_InvertedInertiaTensor = Matrix.Transpose(matrix) * rbo.InvertInertiaTensor * matrix;
                sb.m_State = MyRBSolverBody.SolverBodyState.SBS_Dynamic;
            }

            m_SolverBodies.Add(rbo, sb);

            return sb;
        }

        /// <summary>
        /// creates and prepare the solver constraint
        /// </summary>
        private void AddConstraint(MyRBContactConstraint rbc)
        {
            if (rbc.GetRBElementInteraction().GetRigidBody1().IsStatic() && rbc.GetRBElementInteraction().GetRigidBody2().IsStatic())
            {
                return;
            }

            for (int i = 0; i < rbc.m_NumCollPts; i++)
            {
                MyRBSolverConstraint rbsc = m_SolverConstaintPool.Allocate();
                rbsc.Clear();

                MyCollPointInfo pointInfo = rbc.m_PointInfo[i];

                MyRBSolverBody body1 = null;
                MyRBSolverBody body2 = null;

                MyRigidBody rbo1 = rbc.GetRBElementInteraction().GetRigidBody1();
                MyRigidBody rbo2 = rbc.GetRBElementInteraction().GetRigidBody2();

                if (!m_SolverBodies.TryGetValue(rbo1, out body1))
                {
                    body1 = AddRigidBody(rbo1);
                }

                if (!m_SolverBodies.TryGetValue(rbo2, out body2))
                {
                    body2 = AddRigidBody(rbo2);
                }

                if ((rbo1.GetFlags() & RigidBodyFlag.RBF_DISABLE_COLLISION_RESPONCE) > 0)
                {
                    continue;
                }

                if ((rbo2.GetFlags() & RigidBodyFlag.RBF_DISABLE_COLLISION_RESPONCE) > 0)
                {
                    continue;
                }

                rbsc.m_SolverBody1 = body1;
                rbsc.m_SolverBody2 = body2;

                MyRBMaterial material1 = rbc.GetRBElementInteraction().RBElement1.RBElementMaterial;
                MyRBMaterial material2 = rbc.GetRBElementInteraction().RBElement2.RBElementMaterial;

                float restitution = material1.NominalRestitution * material2.NominalRestitution;

                rbsc.m_RBConstraint = rbc;

                rbsc.m_Magnitude = rbc.Magnitude * m_SolutionDamping;

                //This is a contact constraint
                Vector3 globalVel1 = new Vector3();
                Vector3 globalVel2 = new Vector3();

                rbc.GetRBElementInteraction().GetRigidBody1().GetGlobalPointVelocity(ref pointInfo.m_Info.m_WorldPosition, out globalVel1);
                rbc.GetRBElementInteraction().GetRigidBody2().GetGlobalPointVelocity(ref pointInfo.m_Info.m_WorldPosition, out globalVel2);

                Vector3 relativeVelocity = globalVel2 - globalVel1;
                float aRelVel = Vector3.Dot(pointInfo.m_Info.m_Normal, relativeVelocity);

                float diff = aRelVel;
                rbsc.m_Target = diff >= 0.0f ? 0.0f : (-diff * restitution);

                if(pointInfo.m_Info.m_InitialPenetration < -0.01f)
                    rbsc.m_Target -= pointInfo.m_Info.m_InitialPenetration * m_DepenetrationCoeficient;

                rbsc.m_Restitution = restitution;

                rbsc.m_Normal = pointInfo.m_Info.m_Normal;
                rbsc.m_ContactPoint = pointInfo.m_Info.m_WorldPosition;

                rbsc.m_Body1LocalPoint = pointInfo.m_Info.m_R0;
                rbsc.m_Body2LocalPoint = pointInfo.m_Info.m_R1;

                Vector3 cross1 = Vector3.Cross(pointInfo.m_Info.m_R0, pointInfo.m_Info.m_Normal);
                Vector3 cross2 = Vector3.Cross(pointInfo.m_Info.m_R1, pointInfo.m_Info.m_Normal);

                rbsc.m_Body1LocalPointCrossedNormal = Vector3.Transform(cross1, body1.m_InvertedInertiaTensor);
                rbsc.m_Body2LocalPointCrossedNormal = Vector3.Transform(cross2, body2.m_InvertedInertiaTensor);

                rbsc.m_StaticFriction = material1.NominalStaticFriction * material2.NominalStaticFriction;
                rbsc.m_DynamicFriction = material1.NominalDynamicFriction * material2.NominalDynamicFriction;
                rbsc.m_Affection = 0.0f;
                if (body1.m_State == MyRBSolverBody.SolverBodyState.SBS_Dynamic)
                {
                    rbsc.m_Affection += Vector3.Dot(rbsc.m_Normal, (rbsc.m_Normal * body1.m_OneOverMass + Vector3.Cross(rbsc.m_Body1LocalPointCrossedNormal, rbsc.m_Body1LocalPoint)));
                    body1.m_LinearVelocity -= rbsc.m_Normal * (body1.m_OneOverMass * rbsc.m_Magnitude);
                    body1.m_AngularVelocity -= rbsc.m_Body1LocalPointCrossedNormal * (rbsc.m_Magnitude);
                    MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(body1.m_AngularVelocity);
                }
                if (body2.m_State == MyRBSolverBody.SolverBodyState.SBS_Dynamic)
                {
                    rbsc.m_Affection += Vector3.Dot(rbsc.m_Normal, (rbsc.m_Normal * body2.m_OneOverMass + Vector3.Cross(rbsc.m_Body2LocalPointCrossedNormal, rbsc.m_Body2LocalPoint)));
                    body2.m_LinearVelocity -= rbsc.m_Normal * (body2.m_OneOverMass * rbsc.m_Magnitude);
                    body2.m_AngularVelocity -= rbsc.m_Body2LocalPointCrossedNormal * (rbsc.m_Magnitude);
                    MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(body2.m_AngularVelocity);
                }

                if (rbo1.ContactModifyNotificationHandler != null)
                {
                    if (!rbo1.ContactModifyNotificationHandler.OnContact(ref rbsc))
                    {
                        m_SolverConstaintPool.Deallocate(rbsc);
                        continue;
                    }
                }

                if (rbo2.ContactModifyNotificationHandler != null)
                {
                    if (!rbo2.ContactModifyNotificationHandler.OnContact(ref rbsc))
                    {
                        m_SolverConstaintPool.Deallocate(rbsc);
                        continue;
                    }
                }

                m_SolverConstraints.Add(rbsc);
            }
        }

        /// <summary>
        /// Solves the solver rbos and constraints through iterations given by the island. This is a newton based solver.
        /// </summary>
        private void Solve(float dt)
        {
            Vector3 v1 = new Vector3();
            Vector3 v2 = new Vector3();

            for (int iteration = 0; iteration < m_Island.IterationCount; iteration++)
            {
                for (int i = 0; i < m_SolverConstraints.Count; i++)
                {
                    MyRBSolverConstraint sc = m_SolverConstraints[i];
                    MyRBSolverBody b1 = sc.m_SolverBody1;
                    MyRBSolverBody b2 = sc.m_SolverBody2;

                    // compute relative constraint velocity
                    if (b1.m_State == MyRBSolverBody.SolverBodyState.SBS_Static)
                    {
                        v1 = Vector3.Zero;
                    }
                    else
                    {
                        v1 = b1.m_LinearVelocity + Vector3.Cross(b1.m_AngularVelocity, sc.m_Body1LocalPoint);
                    }

                    if (b2.m_State == MyRBSolverBody.SolverBodyState.SBS_Static)
                    {
                        v2 = Vector3.Zero;
                    }
                    else
                    {
                        v2 = b2.m_LinearVelocity + Vector3.Cross(b2.m_AngularVelocity, sc.m_Body2LocalPoint);
                    }

                    float current = Vector3.Dot(v2, sc.m_Normal) - Vector3.Dot(v1, sc.m_Normal);
                    float diff = sc.m_Target - current;

                    if (sc.m_Magnitude < sc.m_MinMagnitude || sc.m_Magnitude > sc.m_MaxMagnitude ||
                        (diff < -m_CollisionEpsilon && sc.m_Magnitude > sc.m_MinMagnitude) ||
                        (diff > m_CollisionEpsilon && sc.m_Magnitude < sc.m_MaxMagnitude))
                    {

                        // Alter magnitude

                        float alter = (diff / sc.m_Affection) * 0.5f;

                        sc.m_Magnitude += alter;

                        //Ensure magnitude is in valid range
                        if (sc.m_Magnitude > sc.m_MaxMagnitude)
                        {
                            alter -= sc.m_Magnitude - sc.m_MaxMagnitude;
                            sc.m_Magnitude = sc.m_MaxMagnitude;
                        }
                        else if (sc.m_Magnitude < sc.m_MinMagnitude)
                        {
                            alter -= sc.m_Magnitude - sc.m_MinMagnitude;
                            sc.m_Magnitude = sc.m_MinMagnitude;
                        }

                        //Apply impulse
                        if (b1.m_State == MyRBSolverBody.SolverBodyState.SBS_Dynamic)
                        {
                            b1.m_LinearVelocity -= sc.m_Normal * (b1.m_OneOverMass * alter);
                            Vector3 tempV = b1.m_LinearVelocity;
                            tempV.Normalize();
                            if (b2.m_State == MyRBSolverBody.SolverBodyState.SBS_Dynamic)
                            {
                                float massRatio = 1.0f / (b1.m_OneOverMass / b2.m_OneOverMass);
                                //If mass of object1 is more than object2, do not apply restitution at all
                                float rest = MathHelper.Lerp(sc.m_Restitution, 1, massRatio - 0.5f);
                                sc.m_Restitution = MathHelper.Clamp(rest, sc.m_Restitution, 1);
                            }

                            b1.m_AngularVelocity -= sc.m_Body1LocalPointCrossedNormal * (alter);
                            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(b1.m_AngularVelocity);
                        }
                        if (b2.m_State == MyRBSolverBody.SolverBodyState.SBS_Dynamic)
                        {
                            b2.m_LinearVelocity += sc.m_Normal * (b2.m_OneOverMass * alter);
                            Vector3 tempV = b2.m_LinearVelocity;
                            tempV.Normalize();

                            if (b1.m_State == MyRBSolverBody.SolverBodyState.SBS_Dynamic)
                            {
                                float massRatio = 1.0f / (b2.m_OneOverMass / b1.m_OneOverMass);
                                //If mass of object2 is more than object1, do not apply restitution at all
                                float rest = MathHelper.Lerp(sc.m_Restitution, 1, massRatio - 0.5f);
                                sc.m_Restitution = MathHelper.Clamp(rest, sc.m_Restitution, 1);
                            }

                            b2.m_AngularVelocity += sc.m_Body2LocalPointCrossedNormal * (alter);
                            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(b2.m_AngularVelocity);
                        }
                    }
                }

                foreach (KeyValuePair<MyRigidBody, MyRBSolverBody> kvp in m_SolverBodies)
                {
                    MyRBSolverBody body = kvp.Value;
                    float cdt = dt / (float)(iteration + 1);
                    if (body.m_State == MyRBSolverBody.SolverBodyState.SBS_Dynamic)
                    {
                        //Integrate velocities
                        body.m_LinearVelocity += body.m_LinearAcceleration * (cdt);
                        body.m_AngularVelocity += body.m_AngularAcceleration * (cdt);
                        MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(body.m_AngularVelocity);

                        if (body.m_MaxAngularVelocity > 0.0f && body.m_AngularVelocity.Length() > body.m_MaxAngularVelocity)
                        {
                            body.m_AngularVelocity = MyMwcUtils.Normalize(body.m_AngularVelocity);
                            body.m_AngularVelocity *= body.m_MaxAngularVelocity;
                        }

                        if (body.m_MaxLinearVelocity > 0.0f && body.m_LinearVelocity.Length() > body.m_MaxLinearVelocity)
                        {
                            body.m_LinearVelocity = MyMwcUtils.Normalize(body.m_LinearVelocity);
                            body.m_LinearVelocity *= body.m_MaxLinearVelocity;
                        }
                    }
                }
            }

            //Integrate position and orientation
            foreach (KeyValuePair<MyRigidBody, MyRBSolverBody> kvp in m_SolverBodies)
            {
                MyRBSolverBody body = kvp.Value;
                if (body.m_State != MyRBSolverBody.SolverBodyState.SBS_Static)
                {
                    Vector3 transl = body.m_Matrix.Translation + body.m_LinearVelocity * (dt);

                    Vector3 dir = body.m_AngularVelocity;
                    float ang = dir.Length();

                    if (ang > 0.0f)
                    {
                        body.m_Matrix.Translation = Vector3.Zero;

                        Vector3.Divide(ref dir, ang, out dir);  // dir /= ang;
                        ang *= dt;
                        Matrix rot;
                        Matrix.CreateFromAxisAngle(ref dir, ang, out rot);
                        Matrix.Multiply(ref body.m_Matrix, ref rot, out body.m_Matrix);

                        MyPhysicsUtils.Orthonormalise(ref body.m_Matrix);
                    }

                    body.m_Matrix.Translation = transl;
                }
            }
        }


        private static float m_CollisionEpsilon = MyPhysicsConfig.CollisionEpsilon;
       // private static float m_ErrorReduction = 20.0f;		//Geometrical constraint error reduction parameter (higher more reduction, more unstable)        
        private static float m_SolutionDamping = 0.5f;	//Damp solution by this value while restoring (to avoid instability)        
        //private static float m_DefaultRestVelocity = 0.3f;	//Relative velocity below this threshold is handled as static contact (restitution=0 and static friction)
        private static float m_DepenetrationCoeficient = 20.0f; // 0.6f * 0.6f;

        private MyRigidBodyIsland m_Island;
        //private MyObjectsPool<MyRBSolverConstraint> m_SolverConstaintPool;
        //private MyObjectsPool<MyRBSolverBody> m_SolverBodiesPool;
        private MyDynamicObjectPool<MyRBSolverConstraint> m_SolverConstaintPool;
        private MyDynamicObjectPool<MyRBSolverBody> m_SolverBodiesPool;
        private List<MyRBSolverConstraint> m_SolverConstraints;
        private Dictionary<MyRigidBody, MyRBSolverBody> m_SolverBodies;
    }
}