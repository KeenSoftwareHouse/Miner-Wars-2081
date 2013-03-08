using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Editor;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Physics
{
    class MyBoxBoxSensorInteraction : MySensorInteraction
    {
        /// <summary>
        /// Box vs box for box sensor interaction
        /// </summary>

        public MyBoxBoxSensorInteraction()
        {
        }

        public override MySensorInteractionEnum GetInteractionType()
        {
            return MySensorInteractionEnum.SI_BOX_BOX;
        }

        public override void DoWork()
        {
            MyRBBoxElement rbBoxElement = (MyRBBoxElement)m_RBElement;
            MyBoxSensorElement seBoxElement = (MyBoxSensorElement)m_SensorElement;

            Matrix rbBoxMatrix = rbBoxElement.GetGlobalTransformation();
            Matrix seBoxMatrix = seBoxElement.GetGlobalTransformation();

            BoundingBox rbBB = new BoundingBox(-rbBoxElement.Size / 2f, rbBoxElement.Size / 2f);
            BoundingBox seBB = new BoundingBox(-seBoxElement.Extent, seBoxElement.Extent);

            MyOrientedBoundingBox rbBoxOriented = MyOrientedBoundingBox.CreateFromBoundingBox(rbBB).Transform(rbBoxMatrix);
            MyOrientedBoundingBox seBoxOriented = MyOrientedBoundingBox.CreateFromBoundingBox(seBB).Transform(seBoxMatrix);

            m_IsInside = rbBoxOriented.Intersects(ref seBoxOriented);
        }
    }
}
