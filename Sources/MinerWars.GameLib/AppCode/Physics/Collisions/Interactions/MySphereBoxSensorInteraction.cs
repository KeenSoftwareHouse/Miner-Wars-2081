using MinerWarsMath;
using System;

namespace MinerWars.AppCode.Physics
{
    class MySphereBoxSensorInteraction : MySensorInteraction
    {
        /// <summary>
        /// Sphere vs box sensor interaction
        /// </summary>

        public MySphereBoxSensorInteraction()
        {
        }

        public override MySensorInteractionEnum GetInteractionType()
        {
            return MySensorInteractionEnum.SI_SPHERE_BOX;
        }

        public override void DoWork()
        {
            MyRBBoxElement box = (MyRBBoxElement) m_RBElement; 
            MySphereSensorElement sphere = (MySphereSensorElement) m_SensorElement;            

            Matrix boxMatrix = box.GetGlobalTransformation();
            Vector3 sphereCenter = sphere.GetGlobalTransformation().Translation;

            Matrix invBoxMatrix = Matrix.Invert(boxMatrix);

            Vector3 boxLocalsphereCenter = Vector3.Transform(sphereCenter, invBoxMatrix);

            bool penetration = false;
            Vector3 normal =  new Vector3();
            Vector3 closestPos =  new Vector3();
            uint customData = 0;

            box.GetClosestPoint(boxLocalsphereCenter, ref closestPos, ref normal, ref penetration, ref customData);

            if (penetration)
            {
                m_IsInside = true;
                return;
            }

            closestPos = Vector3.Transform(closestPos, boxMatrix);            

            float vLength = (sphereCenter - closestPos).LengthSquared();

            if (vLength <= sphere.Radius * sphere.Radius)
            {
                if (vLength <= (sphere.Radius / 2f) * (sphere.Radius / 2f) || sphere.SpecialDetectingAngle == null) 
                {
                    m_IsInside = true;
                }
                else if (sphere.SpecialDetectingAngle != null)
                {
                    Vector3 normalizeDirectionToRBElement = Vector3.Normalize(boxMatrix.Translation - sphereCenter);
                    float cosAngle = Vector3.Dot(normalizeDirectionToRBElement, sphere.GetGlobalTransformation().Forward);
                    m_IsInside = Math.Abs(cosAngle) >= sphere.SpecialDetectingAngle.Value;
                }                
            }
            else
            {
                m_IsInside = false;
            }            
        }
    }
}
