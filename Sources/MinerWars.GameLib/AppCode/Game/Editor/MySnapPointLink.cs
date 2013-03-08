using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Prefabs;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Holds linked snap point data
    /// </summary>
    class MySnapPointLink
    {
        /// <summary>
        /// Snap Point
        /// </summary>
        public MyPrefabSnapPoint SnapPoint { get; set; }

        /// <summary>
        /// Link Time Transformation (World)
        /// </summary>
        public Matrix LinkTransformation { get; set; }

        public MySnapPointLink(MyPrefabSnapPoint snapPoint)
        {
            SnapPoint = snapPoint;
            LinkTransformation = snapPoint.Prefab.WorldMatrix;
        }
    }
}
