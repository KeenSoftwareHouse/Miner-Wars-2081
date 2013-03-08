using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.WayPoints;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// MySelectionTool
    /// </summary>
    class MySelectionTool
    {
        /// <summary>
        /// GetSelectableEntity - return top most parent which can be selectable (in case of weapons slection return ship!)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static MyEntity GetSelectableEntity(MyEntity entity)
        {
            if (entity == null)
                return null;

            if (entity.Parent == null)
            {
                if (entity.IsSelectable())
                    return entity;
                else
                    return null;
            }

            MyEntity currEntity = entity;
            MyEntity topMostSelectable = (currEntity.IsSelectable()) ? currEntity : null; //store
            while (currEntity.Parent != null)
            {
                if (currEntity.IsSelectableAsChild() && currEntity.IsSelectable())
                    return currEntity;

                currEntity = currEntity.Parent;
                if (currEntity.IsSelectable())
                    topMostSelectable = currEntity;
            }

            return topMostSelectable;
        }
    }
}
