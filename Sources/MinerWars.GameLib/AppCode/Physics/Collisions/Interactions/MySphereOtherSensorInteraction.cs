using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;

namespace MinerWars.AppCode.Physics
{
    class MySphereOtherSensorInteraction : MySensorInteraction
    {
        /// <summary>
        /// Sphere vs other element type sensor interaction
        /// </summary>

        public MySphereOtherSensorInteraction()
        {
        }

        public override MySensorInteractionEnum GetInteractionType()
        {
            return MySensorInteractionEnum.SI_SPHERE_OTHER;
        }

        public override void DoWork()
        {
            MySphereSensorElement sphere = (MySphereSensorElement) m_SensorElement;

            BoundingSphere seSphere = new BoundingSphere(sphere.GetGlobalTransformation().Translation, sphere.Radius);
            BoundingBox oeAABB = m_RBElement.GetWorldSpaceAABB();
            seSphere.Intersects(ref oeAABB, out m_IsInside);
        }
    }
}
