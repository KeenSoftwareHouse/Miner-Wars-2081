using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Physics.Utils;

namespace MinerWars.AppCode.Physics
{
    class MyBoxSphereSensorInteraction : MySensorInteraction
    {
        /// <summary>
        /// Box vs sphere for box sensor interaction
        /// </summary>

        public MyBoxSphereSensorInteraction()
        {
        }

        public override MySensorInteractionEnum GetInteractionType()
        {
            return MySensorInteractionEnum.SI_BOX_SPHERE;
        }

        public override void DoWork()
        {
            MyRBSphereElement sphere = (MyRBSphereElement)m_RBElement;
            MyBoxSensorElement box = (MyBoxSensorElement)m_SensorElement;            

            Matrix boxMatrix = box.GetGlobalTransformation();
            Vector3 sphereCenter = sphere.GetGlobalTransformation().Translation;

            Matrix invBoxMatrix = Matrix.Invert(boxMatrix);

            Vector3 boxLocalsphereCenter = Vector3.Transform(sphereCenter, invBoxMatrix);

            bool penetration = false;
            Vector3 normal =  new Vector3();
            Vector3 closestPos =  new Vector3();
            uint customData = 0;

            MyElementHelper.GetClosestPointForBox(box.Extent, boxLocalsphereCenter, ref closestPos, ref normal, ref penetration, ref customData);

            if (penetration)
            {
                m_IsInside = true;
                return;
            }

            closestPos = Vector3.Transform(closestPos, boxMatrix);            

            float vLength = (sphereCenter - closestPos).LengthSquared();

            if (vLength <= sphere.Radius * sphere.Radius)
            {
                m_IsInside = true;
            }
            else
            {
                m_IsInside = false;
            }            
        }
    }
}
