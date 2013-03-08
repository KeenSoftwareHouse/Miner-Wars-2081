using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabKinematicRotatingPart : MyPrefabKinematicPartBase
    {        
        private float m_rotatingVelocity;
        private float m_rotatingAngle;
        private Vector3 m_rotationVector;

        private const float SoundDistanceSquared = 650 * 650;

        public MyPrefabKinematicRotatingPart(MyPrefabContainer owner)
            : base(owner)
        {
        
        }

        public void Init(MyPrefabBase rotatingOwner, MyModelsEnum modelEnum, MyMaterialType matType, Matrix rotatingLocal, float rotatingVelocityMax, MySoundCuesEnum? loopSound, MySoundCuesEnum? loopDamagedSound, MySoundCuesEnum? startSound, MySoundCuesEnum? endSound, Vector3 rotationVector, bool activated) 
        {
            m_rotationVector = rotationVector;
            base.Init(rotatingOwner, MyMwcObjectBuilderTypeEnum.PrefabKinematicRotatingPart, (int)MyMwcObjectBuilder_PrefabKinematicRotatingPart_TypesEnum.DEFAULT,
                modelEnum, matType, rotatingLocal, rotatingVelocityMax, loopSound, loopDamagedSound, startSound, endSound, 1f, 1f);
            if (activated)
            {
                PersistentFlags &= ~MyPersistentEntityFlags.Deactivated;
            }
            else 
            {
                PersistentFlags |= MyPersistentEntityFlags.Deactivated;
            }
        }

        protected override bool IsKinematicActive()
        {
            return RotatingVelocityAbs > 0f;
        }

        protected float RotatingVelocityAbs
        {
            get { return Math.Abs(m_rotatingVelocity); }
        }

        protected override void UpdateAfterSimulationKinematic(float dt)
        {
            float previousVelocity = m_rotatingVelocity;

            if (On)
            {
                m_rotatingVelocity = m_rotatingVelocity + (m_kinematicVelocityMax / m_kinematicStart) * dt;
            }
            else
            {
                m_rotatingVelocity = m_rotatingVelocity - (m_kinematicVelocityMax / m_kinematicEnd) * dt;
            }
            if (m_kinematicVelocityMax >= 0f)
            {
                m_rotatingVelocity = MathHelper.Clamp(m_rotatingVelocity, 0f, m_kinematicVelocityMax);
            }
            else 
            {
                m_rotatingVelocity = MathHelper.Clamp(m_rotatingVelocity, m_kinematicVelocityMax, 0f);
            }
            m_rotatingAngle += m_rotatingVelocity * dt;
            
            if (Math.Abs(m_rotatingAngle) > Math.PI * 2)
            {
                m_rotatingAngle = 0f;
            }

            if (previousVelocity == 0f && RotatingVelocityAbs > 0f)
            {
                PlayKinematicStartSound();
            }
            else if (previousVelocity == m_kinematicVelocityMax && RotatingVelocityAbs < KinematicVelocityMaxAbs)
            {
                PlayKinematicEndSound();
            }
            else if (m_rotatingVelocity == m_kinematicVelocityMax)
            {
                if (MySession.PlayerShip != null && (Vector3.DistanceSquared(MySession.PlayerShip.GetPosition(), this.m_owner.GetPosition()) < SoundDistanceSquared))
                {
                    PlayKinematicLoopingSound();
                }
                else
                {
                    StopKinematicSounds();
                }
                
            }

            if (RotatingVelocityAbs > 0f)
            {
                Matrix rotMatrix = Matrix.CreateFromAxisAngle(m_rotationVector, m_rotatingAngle);
                Matrix tranMatrix = Matrix.CreateTranslation(-m_kinematicLocalMatrix.Translation);                
                Matrix localMatrix = Matrix.Multiply(tranMatrix, rotMatrix);
                localMatrix.Translation += m_kinematicLocalMatrix.Translation;
                LocalMatrix = localMatrix;
            } 
        }                
    }
}
