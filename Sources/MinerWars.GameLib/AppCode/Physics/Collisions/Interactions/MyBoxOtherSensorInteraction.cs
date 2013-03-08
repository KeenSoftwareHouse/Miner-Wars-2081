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
    class MyBoxOtherSensorInteraction : MySensorInteraction
    {
        /// <summary>
        /// Box vs other element type for box sensor interaction
        /// </summary>

        public MyBoxOtherSensorInteraction()
        {
        }

        public override MySensorInteractionEnum GetInteractionType()
        {
            return MySensorInteractionEnum.SI_BOX_OTHER;
        }

        public override void DoWork()
        {            
            MyBoxSensorElement seBoxElement = (MyBoxSensorElement)m_SensorElement;
            
            Matrix seBoxMatrix = seBoxElement.GetGlobalTransformation();

            BoundingBox oeAABB = m_RBElement.GetWorldSpaceAABB();
            BoundingBox seBB = new BoundingBox(-seBoxElement.Extent, seBoxElement.Extent);

            MyOrientedBoundingBox seBoxOriented = MyOrientedBoundingBox.CreateFromBoundingBox(seBB).Transform(seBoxMatrix);

            m_IsInside = seBoxOriented.Intersects(ref oeAABB);
        }
    }
}
