using MinerWarsMath;
using System;
namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Sphere vs sphere sensor interaction
    /// </summary>
    class MySphereSphereSensorInteraction : MySensorInteraction
    {
        public MySphereSphereSensorInteraction()
        {
        }

        public override MySensorInteractionEnum GetInteractionType()
        {
            return MySensorInteractionEnum.SI_SPHERE_SPHERE;
        }

        public override void DoWork()
        {
            Matrix sensorMatrix = m_SensorElement.GetGlobalTransformation();
            Matrix elementMatrix = m_RBElement.GetGlobalTransformation();

            MySphereSensorElement sphereSensorElement = (MySphereSensorElement)m_SensorElement;

            float sradius = sphereSensorElement.Radius;
            float eradius = ((MyRBSphereElement) m_RBElement).Radius;

            Vector3 dv = sensorMatrix.Translation - elementMatrix.Translation;

            float distance = dv.LengthSquared();

            if (distance <= (sradius + eradius) * (sradius + eradius))
            {
                if (sphereSensorElement.SpecialDetectingAngle == null || distance <= ((sradius + eradius) / 2f) * ((sradius + eradius) / 2f))
                {
                    m_IsInside = true;
                }
                else if (sphereSensorElement.SpecialDetectingAngle != null)
                {
                    Vector3 normalizeDirectionToRBElement = Vector3.Normalize(elementMatrix.Translation - sensorMatrix.Translation);
                    float cosAngle = Vector3.Dot(normalizeDirectionToRBElement, sensorMatrix.Forward);
                    m_IsInside = Math.Abs(cosAngle) >= sphereSensorElement.SpecialDetectingAngle.Value;
                }                
            }
            else
            {
                m_IsInside = false;
            }
        }
    }
}
