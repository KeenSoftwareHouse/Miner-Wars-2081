using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Game.Entities.Tools
{
    class MyNanoRepairToolEntity : MyNanoRepairToolBase
    {
        private MyEntity m_entity;

        public MyNanoRepairToolEntity(MyEntity entity)            
        {
            Debug.Assert(entity != null);
            m_entity = entity;            
        }        

        public override void Use()
        {
            UseForEntity(m_entity);
        }
    }
}
