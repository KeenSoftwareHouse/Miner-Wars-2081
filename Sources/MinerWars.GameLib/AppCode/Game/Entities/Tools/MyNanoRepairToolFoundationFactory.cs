//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using MinerWars.AppCode.Game.Gameplay;
//using MinerWars.AppCode.Game.Entities.FoundationFactory;
//using MinerWars.AppCode.Game.Entities.Prefabs;
//using MinerWars.AppCode.Game.Prefabs;
//using MinerWars.AppCode.Game.Utils;

//namespace MinerWars.AppCode.Game.Entities.Tools
//{
//    class MyNanoRepairToolFoundationFactory : MyNanoRepairToolBase
//    {
//        private MyFoundationFactory m_foundationFactory;

//        public MyNanoRepairToolFoundationFactory(MyFoundationFactory foundationFactory)            
//        {
//            Debug.Assert(foundationFactory != null);
//            m_foundationFactory = foundationFactory;            
//        }        

//        public override void Use()
//        {
//            foreach (MyPrefabBase prefab in m_foundationFactory.PrefabContainer.GetPrefabs())
//            {
//                UseForEntity(prefab);
//            }
//        }
//    }
//}
