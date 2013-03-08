using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.SolarSystem
{
    enum MyTemplateGroupEnum
    {
        RandomStations, 
        MiningStations,
        Madelyn
    }

    class MyTemplateGroups
    {
        static Dictionary<int, MyMwcVector3Int> TemplateGroups = new Dictionary<int, MyMwcVector3Int>();

        static MyTemplateGroups()
        {
            TemplateGroups.Add((int)MyTemplateGroupEnum.RandomStations, new MyMwcVector3Int(0, 0, 0));
            TemplateGroups.Add((int)MyTemplateGroupEnum.MiningStations, new MyMwcVector3Int(0, 0, 1));
            TemplateGroups.Add((int)MyTemplateGroupEnum.Madelyn, new MyMwcVector3Int(1, 0, 1));
        }

        public static MyMwcVector3Int GetGroupSector(MyTemplateGroupEnum group)
        {
            return TemplateGroups[(int)group];
        }
    }
}
