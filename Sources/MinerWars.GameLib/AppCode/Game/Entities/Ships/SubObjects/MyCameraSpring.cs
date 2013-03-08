using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Camera spring like binding with rigid body
    /// </summary>
    class MyCameraSpring
    {
        private MyPhysicsBody m_GameRigidBody0;

        //m_MaxVelocity - maximalni rychlost jakou se hlava pohybuje vuci lodi
        //m_MaxAccel - max zrychleni hlavy vuci lodi
        //m_MaxDistanceSpeed - maximalni rychlost do ktere se zvetsuje kam se muze hlava hybat

        //Utlum rychlosti z duvody zmeny pohybu. Tohle tam vicemene drzi setrvacnost, ale musi se ta rychlost redukovat jinak by se nedala ta rychlost zmenit dostatecne rychle. Proto se ta rychlost redukuje vyrazne a v dalsim snimku znova nastavuje.
        // m_LinearVelocity *= 0.8f;

        //Kdyz je:
        //  m_LinearVelocity = 0.0f; znamena to, ze je vyrazna zmena pohybu lode - podminka, ze zrychleni je jinym smerem nez rychlost, v tom pripade zacnu tu rychlost kumulovat znova

        //Navrat hlavy je reseni utlumem toho vectoru:
        // m_LocalTranslation *= 0.98f; 

        public float m_MaxVelocity = 0.2f;        //maximum head speed according to ship
        public float m_MaxAccel = 38.0f;           //maximum acceleration according to ship
        private Vector3 m_LinearVelocity;   
        private Vector3 m_LimitedVelocity;
        public float m_MaxDistanceSpeed = 250f;   //maximum speed limit of head
        public float m_linearVelocityDumping = 0.20f;//speed reduction 
        public float m_linearVelocityAdditionalDumping = 0.60f;//speed reduction according to direction
        public float m_localTranslationDumping = 0.98f;//returing head into initial position, local dif position reduction

        private Vector3 m_LocalTranslation;

        private MySmallShip m_Ship;

        public MyCameraSpring(MySmallShip ship)
        {
            m_LimitedVelocity = Vector3.Zero;

            m_Ship = ship;
            m_GameRigidBody0 = ship.Physics;

            m_LinearVelocity = Vector3.Zero;        

            m_LocalTranslation = Vector3.Zero;
        }

        /// <summary>
        /// Maximum local velocity of the camera
        /// </summary>
        public float MaxVelocitySquered { get { return m_MaxVelocity; } set { m_MaxVelocity = value; } }
        /// <summary>
        /// Maximum local acceleration of the camera
        /// </summary>
        public float MaxAccelSquered { get { return m_MaxAccel; } set { m_MaxAccel = value; } }
        /// <summary>
        /// Max speed for the distance, until this distance the position limit of local camera space will increase
        /// </summary>
        public float MaxDistanceSpeedSquered { get { return m_MaxDistanceSpeed; } set { m_MaxDistanceSpeed = value; } }

        public void Update(float timeStep, Matrix inverseRboMatrix, ref Vector3 headLocal)
        {
            Vector3 localVelocity = Vector3.TransformNormal(m_GameRigidBody0.LinearVelocity, inverseRboMatrix);

            if(Vector3.Dot(m_GameRigidBody0.LinearAcceleration, m_GameRigidBody0.LinearVelocity) > 0.6f)
            {                
                m_LinearVelocity -= localVelocity;

                Vector3 localAccel = Vector3.TransformNormal(m_GameRigidBody0.LinearAcceleration, inverseRboMatrix);

                Vector3 oldVel = m_LimitedVelocity;
                m_LimitedVelocity = m_LinearVelocity;

                if (m_LimitedVelocity.LengthSquared() > m_MaxVelocity)
                {
                    m_LimitedVelocity.Normalize();
                    m_LimitedVelocity *= m_MaxVelocity;
                }

                Vector3 accel = (m_LimitedVelocity - oldVel) / timeStep;

                if (accel.Length() > m_MaxAccel)
                {
                    accel.Normalize();
                    m_LimitedVelocity = oldVel + accel * (m_MaxAccel * timeStep);
                }

                Vector3 oldTranslation = m_LocalTranslation;
                m_LocalTranslation += m_LimitedVelocity * timeStep;

                float speedFactor = MathHelper.Clamp(localVelocity.Length() / m_MaxDistanceSpeed, 0.3f, 1.0f);

                Vector3 distanceLimit = MyMinerShipConstants.PLAYER_HEAD_MAX_DISTANCE * 0.7f*speedFactor;

                //  Limiting head movement
                if (m_LocalTranslation.X > distanceLimit.X && m_LimitedVelocity.X >= 0.0f)
                {
                    if (m_LimitedVelocity.X * localAccel.X < 0.0f)
                    {
                        m_LocalTranslation.X = distanceLimit.X;
                    }
                    else
                    {
                        m_LocalTranslation.X = oldTranslation.X;
                    }
                }

                if (m_LocalTranslation.X < -distanceLimit.X && m_LimitedVelocity.X < 0.0f)
                {
                    if (m_LimitedVelocity.X * localAccel.X < 0.0f)
                    {
                        m_LocalTranslation.X = -distanceLimit.X;
                    }
                    else
                    {
                        m_LocalTranslation.X = oldTranslation.X;
                    }                    
                }

                if (m_LocalTranslation.Y > distanceLimit.Y && m_LimitedVelocity.Y > 0.0f)
                {
                    if (m_LimitedVelocity.Y * localAccel.Y < 0.0f)
                    {
                        m_LocalTranslation.Y = distanceLimit.Y;
                    }
                    else
                    {
                        m_LocalTranslation.Y = oldTranslation.Y;
                    }                    
                }

                if (m_LocalTranslation.Y < -distanceLimit.Y && m_LimitedVelocity.Y < 0.0f)
                {
                    if (m_LimitedVelocity.Y * localAccel.Y < 0.0f)
                    {
                        m_LocalTranslation.Y = -distanceLimit.Y;
                    }
                    else
                    {
                        m_LocalTranslation.Y = oldTranslation.Y;
                    }                    
                }

                if (m_LocalTranslation.Z > distanceLimit.Z && m_LimitedVelocity.Z > 0.0f)
                {
                    if (m_LimitedVelocity.Z * localAccel.Z < 0.0f)
                    {
                        m_LocalTranslation.Z = distanceLimit.Z;
                    }
                    else
                    {
                        m_LocalTranslation.Z = oldTranslation.Z;
                    }                                        
                }

                if (m_LocalTranslation.Z < -distanceLimit.Z && m_LimitedVelocity.Z < 0.0f)
                {
                    if (m_LimitedVelocity.Z * localAccel.Z < 0.0f)
                    {
                        m_LocalTranslation.Z = -distanceLimit.Z;
                    }
                    else
                    {
                        m_LocalTranslation.Z = oldTranslation.Z;
                    }
                }

                Vector3 velDir = m_LimitedVelocity;
                velDir.Normalize();

                m_LinearVelocity *= m_linearVelocityDumping + m_linearVelocityAdditionalDumping*(Math.Abs(Vector3.Dot(m_Ship.WorldMatrix.Forward,velDir)));
            }
            else
            {
                m_LinearVelocity = Vector3.Zero;
            }            

            m_LocalTranslation *= m_localTranslationDumping;

            float velocityLength = localVelocity.Length();

            if (velocityLength > MyMwcMathConstants.EPSILON)
            {
                float velocityFactor = MathHelper.Clamp(velocityLength * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * 0.5f, 0, 3);
                m_Ship.IncreaseHeadShake(velocityFactor);
            }

            headLocal = m_LocalTranslation;
        }

    }
}
