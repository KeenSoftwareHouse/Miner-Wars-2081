using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MinerWars.CommonLIB.AppCode.Utils;

//  Sector group is a origin/pivot point for a group of sectors in some area. In fact we don't
//  need this if all sector will be relative to sun. But because I want more flexibility when
//  size of sector changes (can happen in future during game-play-testing), I want to have areas
//  where are sectors and those sectors are relative to their sector group.
//  I assume player can't travel in his ship from sector group to other sector group. Distance will
//  be just too large (assuming that traveling from sun to earth will take 7 years).

namespace MinerWars.AppCode.Game.World
{
    //  IMPORTANT: Never change numeric values because they are also used in database
    //  IMPORTANT: Never delete any enum item
    enum MySectorGroupEnum : short
    {
        NEAR_SUN = 0,
        MIDDLE_EAST = 1,
        MIDDLE_WEST = 2,
        NEAR_EARTH = 3,
        ABOVE_SUN = 4
    }

    class MySectorGroup
    {
        public MySectorGroupEnum GroupEnum;         //  Group enum
        public Vector3 Position;                    //  Position within solar syste, relative to sun

        private MySectorGroup() { }

        public MySectorGroup(MySectorGroupEnum groupEnum, Vector3 position)
        {
            GroupEnum = groupEnum;
            Position = position;
        }
    }

    static class MySectorGroups
    {
        static MySectorGroup[] m_groups;

        static MySectorGroups()
        {
            m_groups = new MySectorGroup[MyMwcUtils.GetMaxAsShortValueFromEnum<MySectorGroupEnum>()];
            Add(MySectorGroupEnum.NEAR_SUN, new Vector3(0, 0, 0));
            Add(MySectorGroupEnum.MIDDLE_EAST, new Vector3(0, 0, 0));
            Add(MySectorGroupEnum.MIDDLE_WEST, new Vector3(0, 0, 0));
            Add(MySectorGroupEnum.NEAR_EARTH, new Vector3(0, 0, 0));
            Add(MySectorGroupEnum.NEAR_EARTH, new Vector3(0, 0, 0));

            //  Assert for whether we didn't forget to assign
            for (int i = 0; i < m_groups.Length; i++)
            {
                MyMwcUtils.AssertRelease(m_groups[i] != null);
            }
        }

        static void Add(MySectorGroupEnum groupEnum, Vector3 position)
        {
            m_groups[(int)groupEnum] = new MySectorGroup(groupEnum, position);
        }
    }
}
