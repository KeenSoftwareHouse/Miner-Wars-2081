using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Entities
{
    interface IMyEditableEntity
    {
        /// <summary>
        /// Set diffuse color to selected color
        /// </summary>
        void HighlightEntity(ref Vector3 vctColor);
        
        /// <summary>
        /// HighlightEntity
        /// </summary>
        /// <param name="?"></param>
        void ClearHighlightning();

        /// <summary>
        /// Return if entity can be selected in editor - guns under 
        /// </summary>
        /// <returns></returns>
        bool IsSelectable();
    }
}
