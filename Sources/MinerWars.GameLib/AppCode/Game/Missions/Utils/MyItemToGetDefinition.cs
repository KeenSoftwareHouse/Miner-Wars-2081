using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.AppCode.Game.Missions.Utils
{
    class MyItemToGetDefinition
    {
        public MyMwcObjectBuilderTypeEnum ItemType { get; set; }
        public int? ItemId { get; set; }
        public int Count { get; set; }

        public MyItemToGetDefinition(MyMwcObjectBuilderTypeEnum itemType, int? itemId, int count) 
        {
            ItemType = itemType;
            ItemId = itemId;
            Count = count;
        }

        public MyItemToGetDefinition(MyMwcObjectBuilderTypeEnum itemType, int? itemId)
            : this(itemType, itemId, 1) 
        {

        }
    }
}
