using System.Diagnostics;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.EntityManager.Notifications;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Linq;
using System.Collections.Generic;

namespace MinerWars.AppCode.Game.Physics
{
    /// <summary>
    /// Abstract engine physics body object.
    /// </summary>
    internal class MyPhysicsBody : MyResource, IMyNotifyMotion, IMyNotifyContact, IMyNotifyEntityChanged, IMyContactModifyNotifications
    {
        #region Fields

        private bool m_enabled;

        /// <summary>
        /// This was moved here from MyGameRigidBody.
        /// </summary>
        protected readonly AppCode.Physics.MyRigidBody rigidBody;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public MyEntity Entity { get; private set; }

        public MyRigidBody RigidBody { get { return rigidBody; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MyGameRigidBody"/> is collideable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if collideable; otherwise, <c>false</c>.
        /// </value>
        public bool Collideable
        {
            get
            {
                return !this.rigidBody.ReadFlag(RigidBodyFlag.RBF_DISABLE_COLLISION_RESPONCE);
            }
            set
            {
                if (!value)
                {
                    this.rigidBody.RaiseFlag(RigidBodyFlag.RBF_DISABLE_COLLISION_RESPONCE);
                }
                else
                {
                    this.rigidBody.ClearFlag(RigidBodyFlag.RBF_DISABLE_COLLISION_RESPONCE);
                }

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MyGameRigidBody"/> is static.
        /// </summary>
        /// <value>
        ///   <c>true</c> if static; otherwise, <c>false</c>.
        /// </value>
        public bool Static
        {
            get
            {
                return this.rigidBody.IsStatic();
            }
            set
            {
                if (value)
                {
                    this.rigidBody.RaiseFlag(RigidBodyFlag.RBF_RBO_STATIC);
                }
                else
                {
                    this.rigidBody.ClearFlag(RigidBodyFlag.RBF_RBO_STATIC);
                }

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MyGameRigidBody"/> is kinematic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if kinematic; otherwise, <c>false</c>.
        /// </value>
        public bool Kinematic
        {
            get
            {
                return this.rigidBody.IsKinematic();
            }
            set
            {
                if (value)
                {
                    this.rigidBody.RaiseFlag(RigidBodyFlag.RBF_KINEMATIC);
                }
                else
                {
                    this.rigidBody.ClearFlag(RigidBodyFlag.RBF_KINEMATIC);
                }

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MyPhysicsBody"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get
            {
                return this.m_enabled;
            }
            set
            {
                if (this.m_enabled != value)
                {
                    this.m_enabled = value;
                    if (value) 
                        Activate();
                    else 
                        Deactivate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [play collision cue enabled].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [play collision cue enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool PlayCollisionCueEnabled { get; set; }

        /// <summary>
        /// Gets or sets the type of the material.
        /// </summary>
        /// <value>
        /// The type of the material.
        /// </value>
        public MyMaterialType MaterialType { get; set; }


        /// <summary>
        /// Gets or sets the mass.
        /// </summary>
        /// <value>
        /// The mass.
        /// </value>
        public float Mass
        {
            get
            {
                return this.rigidBody.GetMass();
            }
            set
            {
                this.rigidBody.SetMass(value, true);
            }
        }

        /// <summary>
        /// Gets or sets the linear velocity.
        /// </summary>
        /// <value>
        /// The linear velocity.
        /// </value>
        public Vector3 LinearVelocity
        {
            get
            {
                return this.rigidBody.LinearVelocity;
            }
            set
            {
                MyUtils.AssertIsValid(value);
                this.rigidBody.LinearVelocity = value;
            }
        }

        /// <summary>
        /// Gets or sets max linear velocity.
        /// </summary>
        /// <value>
        /// The max linear velocity.
        /// </value>
        public float MaxLinearVelocity
        {
            get
            {
                return this.rigidBody.MaxLinearVelocity;
            }
            set
            {
                MyUtils.AssertIsValid(value);
                this.rigidBody.MaxLinearVelocity = value;
            }
        }

        /// <summary>
        /// Gets or sets the linear acceleration.
        /// </summary>
        /// <value>
        /// The linear acceleration.
        /// </value>
        public Vector3 LinearAcceleration
        {
            get
            {
                return rigidBody.LinearAcceleration;
            }
            set
            {
                Debug.Assert(!float.IsNaN(value.X));
                this.rigidBody.LinearAcceleration = value;
            }
        }

        /// <summary>
        /// Gets or sets the linear damping.
        /// </summary>
        /// <value>
        /// The linear damping.
        /// </value>
        public float LinearDamping
        {
            get
            {
                return this.rigidBody.LinearDamping;
            }
            set
            {
                Debug.Assert(!float.IsNaN(value));
                this.rigidBody.LinearDamping = value;
            }
        }

        /// <summary>
        /// Gets or sets the angular damping.
        /// </summary>
        /// <value>
        /// The angular damping.
        /// </value>
        public float AngularDamping
        {
            get
            {
                return this.rigidBody.AngularDamping;
            }
            set
            {
                Debug.Assert(!float.IsNaN(value));
                this.rigidBody.AngularDamping = value;
            }
        }

        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        /// <value>
        /// The angular velocity.
        /// </value>
        public Vector3 AngularVelocity
        {
            get
            {
                return this.rigidBody.AngularVelocity;
            }
            set
            {
                Debug.Assert(!float.IsNaN(value.X));
                this.rigidBody.AngularVelocity = value;
            }
        }

        /// <summary>
        /// Gets or sets the max angular velocity.
        /// </summary>
        public float MaxAngularVelocity
        {
            get
            {
                return this.rigidBody.MaxAngularVelocity;
            }
            set
            {
                MyUtils.AssertIsValid(value);
                this.rigidBody.MaxAngularVelocity = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public float Speed
        {
            get
            {
                return this.rigidBody.LinearVelocity.Length();
            }
        }

        /// <summary>
        /// Gets or sets the group mask.
        /// </summary>
        /// <value>
        /// The group mask.
        /// </value>
        public MyGroupMask GroupMask
        {
            set
            {
                this.rigidBody.SetGroupMask(value);
            }
            get
            {
                var elements = this.rigidBody.GetRBElementList();

                if (elements.Count > 0)
                {
                    MyGroupMask groupMask = this.rigidBody.GetRBElementList()[0].GroupMask;

                    Debug.Assert(elements.All(element => element.GroupMask == groupMask),
                                 "All elements should have same group mask!");

                    return groupMask;
                }

                return new MyGroupMask();
            }
        }

        /// <summary>
        /// Gets or sets the collision layer for all elements.
        /// </summary>
        /// <value>
        /// The collision layer.
        /// </value>
        public ushort CollisionLayer
        {
            get
            {
                var elements = this.rigidBody.GetRBElementList();

                if (elements.Count > 0)
                    return elements[0].CollisionLayer;

                return 0;
            }
            set
            {
                var elements = this.rigidBody.GetRBElementList();

                foreach (MyRBElement el in elements)
                {
                    el.CollisionLayer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of rigid body.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ushort Type
        {
            get
            {
                return this.rigidBody.Type;
            }
            set
            {
                this.rigidBody.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MyGameRigidBody"/> is immovable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if immovable; otherwise, <c>false</c>.
        /// </value>
        public bool Immovable
        {
            get
            {
                return this.rigidBody.ReadFlag(RigidBodyFlag.RBF_RBO_STATIC);
            }
            set
            {
                if (value)
                {
                    this.rigidBody.RaiseFlag(RigidBodyFlag.RBF_RBO_STATIC);
                }
                else
                {
                    this.rigidBody.ClearFlag(RigidBodyFlag.RBF_RBO_STATIC);
                }
                MyPhysicsUtils.ComputeIntertiaTensor(this.rigidBody);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no response].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no response]; otherwise, <c>false</c>.
        /// </value>
        public bool NoResponse { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MyPhysicsBody"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public MyPhysicsBody(MyEntity entity, float mass, RigidBodyFlag flags)
        {
            Debug.Assert(entity != null);

            this.m_enabled = false;
            this.Entity = entity;

            MyPhysicsObjects physobj = AppCode.Physics.MyPhysics.physicsSystem.GetPhysicsObjects();
            MyRigidBodyDesc rboDesc = physobj.GetRigidBodyDesc();

            rboDesc.SetToDefault();
            rboDesc.m_Mass = mass;
            rboDesc.m_Matrix = entity.WorldMatrix;
            rboDesc.m_Flags |= flags;

            this.rigidBody = physobj.CreateRigidBody(rboDesc);
            this.rigidBody.m_UserData = this;
        }

        /// <summary>
        /// Gets RB elements list
        /// </summary>
        public List<MyRBElement> GetRBElementList()
        {
            return this.rigidBody.GetRBElementList();
        }

        public void Close()
        {

            Debug.Assert(this.rigidBody != null);
            MyPhysicsObjects physobj = AppCode.Physics.MyPhysics.physicsSystem.GetPhysicsObjects();

            physobj.DestroyRigidBody(this.rigidBody);
        }

        /// <summary>
        /// Applies external force to the physics object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="force">The force.</param>
        /// <param name="position">The position.</param>
        /// <param name="torque">The torque.</param>
        public void AddForce(MyPhysicsForceType type, Vector3? force, Vector3? position, Vector3? torque)
        {
            MyUtils.AssertIsValid(force);
            MyUtils.AssertIsValid(position);
            MyUtils.AssertIsValid(torque);

            switch (type)
            {
                case MyPhysicsForceType.ADD_BODY_FORCE_AND_BODY_TORQUE:
                    {
                        Matrix tempM = rigidBody.Matrix;
                        tempM.Translation = Vector3.Zero;

                        if (force != null && !MyMwcUtils.IsZero(force.Value))
                        {
                            Vector3 tmpForce = Vector3.Transform(force.Value, tempM);

                            this.rigidBody.ApplyForce(tmpForce);
                        }

                        if (torque != null && !MyMwcUtils.IsZero(torque.Value))
                        {
                            Vector3 tmpTorque = Vector3.Transform(torque.Value, tempM);

                            this.rigidBody.ApplyTorque(tmpTorque);
                        }
                    }
                    break;
                case MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE:
                    {
                        if (force.HasValue && position.HasValue)
                        {
                            this.rigidBody.ApplyImpulse(force.Value, position.Value);
                        }

                        if (torque.HasValue)
                        {
                            this.rigidBody.ApplyTorque(torque.Value);
                        }
                    }
                    break;
                default:
                    {
                        Debug.Fail("Unhandled enum!");
                    }
                    break;
            }
        }

        /// <summary>
        /// Applies the impulse.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="pos">The pos.</param>
        public void ApplyImpulse(Vector3 dir, Vector3 pos)
        {
            MyUtils.AssertIsValid(dir);
            MyUtils.AssertIsValid(pos);

            this.rigidBody.ApplyImpulse(dir, pos);
        }

        /// <summary>
        /// Wakes up rigid body.
        /// </summary>
        public void WakeUp()
        {
            this.rigidBody.WakeUp();
        }

        /// <summary>
        /// Clear all current forces.
        /// </summary>
        public virtual void ClearForces()
        {
        }

        /// <summary>
        /// Clears the speeds.
        /// </summary>
        public void ClearSpeed()
        {
            rigidBody.Clear();
            rigidBody.LinearAcceleration = Vector3.Zero;
            rigidBody.LinearVelocity = Vector3.Zero;
            rigidBody.AngularVelocity = Vector3.Zero;
            rigidBody.AngularAcceleration = Vector3.Zero;
        }

        /// <summary>
        /// Clear all dynamic values of physics object.
        /// </summary>
        public void Clear()
        {
            ClearForces();
            ClearSpeed();
        }

        /// <summary>
        /// Adds the skin element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="b">if set to <c>true</c> [b].</param>
        public void AddElement(MyRBElement element, bool recomputeInertia)
        {
            Debug.Assert(element != null);

            this.rigidBody.AddElement(element, recomputeInertia);

        }

        /// <summary>
        /// Removes the elements.
        /// </summary>
        public void RemoveAllElements()
        {
            this.rigidBody.RemoveAllElements();
        }

        /// <summary>
        /// Debug draw of this physics object.
        /// </summary>
        public void DebugDraw()
        {
            const float alpha = 0.3f;

            if (!Enabled)
                return;

            foreach (var primitive in this.rigidBody.GetRBElementList())
            {
                MyRBElementType type = primitive.GetElementType();

                switch (type)
                {
                    case MyRBElementType.ET_BOX:
                        {
                            var box = (MyRBBoxElement)primitive;

                            MyDebugDraw.DrawHiresBoxWireframe(
                                Matrix.CreateScale(box.Size) * box.GetGlobalTransformation(),
                                Color.Green.ToVector3(), alpha);
                        }
                        break;
                    case MyRBElementType.ET_SPHERE:
                        {
                            var sphere = (MyRBSphereElement)primitive;

                            MyDebugDraw.DrawSphereWireframe(
                                Matrix.CreateScale(sphere.Radius) * sphere.GetGlobalTransformation(),
                                Color.Green.ToVector3(), alpha);
                        }
                        break;
                    case MyRBElementType.ET_TRIANGLEMESH:
                        {
                            var triMesh = (MyRBTriangleMeshElement)primitive;
                            var model = triMesh.Model;

                            Matrix transformMatrix = this.rigidBody.Matrix;

                            MyDebugDrawCachedLines.Clear();

                            //  This is just a reserve
                            const int numberOfAddTrianglesInLoop = 3;

                            int triangleIndex = 0;
                            while (true)
                            {
                                //bool finished = triangleIndex >= mesh.GetNumTriangles();
                                bool finished = triangleIndex >= model.GetTrianglesCount();

                                if ((MyDebugDrawCachedLines.IsFull(-numberOfAddTrianglesInLoop)) || (finished))
                                {
                                    MyDebugDrawCachedLines.DrawLines();
                                    MyDebugDrawCachedLines.Clear();
                                }

                                if (finished)
                                {
                                    break;
                                }

                                MyTriangleVertexIndices triangle = model.Triangles[triangleIndex];

                                //  We now transform the triangleVertexes into world space (we could keep leave the mesh alone
                                //  but at this point 3 vector transforms is probably not a major slow down)
                                Vector3 triVec0 = model.GetVertex(triangle.I0);
                                Vector3 triVec1 = model.GetVertex(triangle.I2);
                                Vector3 triVec2 = model.GetVertex(triangle.I1);

                                // Move triangle into world space                        
                                Vector3.Transform(ref triVec0, ref transformMatrix, out triVec0);
                                Vector3.Transform(ref triVec1, ref transformMatrix, out triVec1);
                                Vector3.Transform(ref triVec2, ref transformMatrix, out triVec2);
                                                
                                MyDebugDrawCachedLines.AddLine(triVec0, triVec1, Color.Green, Color.Green);
                                MyDebugDrawCachedLines.AddLine(triVec1, triVec2, Color.Green, Color.Green);
                                MyDebugDrawCachedLines.AddLine(triVec2, triVec0, Color.Green, Color.Green);
                                   /*   
                                MyTriangle_Vertexes tv = new MyTriangle_Vertexes();
                                tv.Vertex0 = triVec0;
                                tv.Vertex1 = triVec1;
                                tv.Vertex2 = triVec2;
                                Vector3 calculatedTriangleNormal = MyUtils.GetNormalVectorFromTriangle(ref tv);
                                Vector3 center = (triVec0 + triVec1 + triVec2)/3.0f;
                                MyDebugDrawCachedLines.AddLine(center, center + calculatedTriangleNormal * 5, Color.Red, Color.Red);
                                     */   

                                //MyDebugDraw.AddDrawTriangle(triVec0, triVec1, triVec2, new Color(0,0.8f, 0, 0.1f));

                                triangleIndex++;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Activates this rigid body in physics.
        /// </summary>
        protected void Activate()
        {
            MyPhysics.physicsSystem.GetRigidBodyModule().Insert(rigidBody);
            rigidBody.Matrix = this.Entity.WorldMatrix;

            this.rigidBody.NotifyMotionHandler = this;
            this.rigidBody.NotifyContactHandler = this;
            this.rigidBody.ContactModifyNotificationHandler = this;
        }

        /// <summary>
        /// Deactivates this rigid body in physics.
        /// </summary>
        protected void Deactivate()
        {
            MyPhysics.physicsSystem.GetRigidBodyModule().Remove(rigidBody);

            this.rigidBody.NotifyMotionHandler = null;
            this.rigidBody.NotifyContactHandler = null;
            this.rigidBody.ContactModifyNotificationHandler = null;
        }

        public void Update()
        {
        }

        #endregion
        
        #region Implementation of IMyNotifyMotion

        /// <summary>
        /// Called when [motion].
        /// </summary>
        /// <param name="rbo">The rbo.</param>
        /// <param name="step">The step.</param>
        public void OnMotion(AppCode.Physics.MyRigidBody rbo, float step)
        {
            Debug.Assert(this.Entity.Physics == this, "Fatal resorce inconsistancy! Now is time to implement real engine resource management.");

            if (this.NoResponse)
            {
                return;
            }

            this.Entity.SetWorldMatrix(this.rigidBody.Matrix, this);
        }

        #endregion

        #region Implementation of IMyNotifyContact

        /// <summary>
        /// Called when [contact start].
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        public virtual void OnContactStart(MyContactEventInfo contactInfo)
        {
            // Notify entity(script) about contact.
            this.Entity.NotifyContactStart(contactInfo);
        }

        /// <summary>
        /// Called when [contact end].
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        public virtual void OnContactEnd(MyContactEventInfo contactInfo)
        {
            this.Entity.NotifyContactEnd(contactInfo);
        }

        /// <summary>
        /// Called when [contact touch].
        /// </summary>
        /// <param name="contactInfo">The contact info.</param>
        public virtual void OnContactTouch(MyContactEventInfo contactInfo)
        {
            this.Entity.NotifyContactTouch(contactInfo);
        }

        /// <summary>
        /// Called when contact
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns>false if contact has to be refused</returns>
        public virtual bool OnContact(ref MyRBSolverConstraint constraint)
        {
            return this.Entity.NotifyContact(ref constraint);
        }

        #endregion

        #region Implementation of IMyNotifyEntityChanged

        /// <summary>
        /// Called when [world position changed].
        /// </summary>
        /// <param name="source">The source object that caused this event.</param>
        public void OnWorldPositionChanged(object source)
        {
            Debug.Assert(this != source, "Recursion!");

            this.rigidBody.Matrix = this.Entity.WorldMatrix;// - pivot is not center of volume (CenterOfMass)
        }    

        #endregion
    }
}