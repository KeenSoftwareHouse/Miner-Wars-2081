using System;
using System.Text;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.World
{
    abstract class MyPlayerBase
    {
        StringBuilder m_displayName;
        int m_userId;

        public MyPlayerStatistics Statistics;

        private MyPlayerBase() { }

        protected MyPlayerBase(StringBuilder displayName, int userId)
        {
            m_displayName = displayName;
            m_userId = userId;
        }

        public StringBuilder GetDisplayName()
        {
            return m_displayName;
        }

        public void SetDisplayName(StringBuilder displayName)
        {
            m_displayName = displayName;
        }

        public void SetMultiplayerUserId(int userId)
        {
            m_userId = userId;
        }

        public int GetUserId()
        {
            return m_userId;
        }
    }
}