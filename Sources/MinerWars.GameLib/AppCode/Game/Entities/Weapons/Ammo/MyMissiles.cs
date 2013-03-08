using MinerWars.CommonLIB.AppCode.Generics;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System.Collections.Generic;
    using MinerWarsMath;
    using SysUtils.Utils;
    using Utils;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

    static class MyMissiles
    {
        static MyObjectsPool<MyMissile> m_missiles = null;

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMissiles.LoadData");

            MyMwcLog.WriteLine("MyMissiles.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            if (m_missiles == null)
                m_missiles = new MyObjectsPool<MyMissile>(MyMissileConstants.MAX_MISSILES_COUNT);
            m_missiles.DeallocateAll();

            foreach (LinkedListNode<MyMissile> item in m_missiles.GetPreallocatedItemsArray())
            {
                item.Value.Init();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMissiles.LoadContent() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

       
        public static void UnloadData()
        {
        }

        //  Add new missile to the list
        public static MyMissile Add(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum type, Vector3 position, Vector3 initialVelocity, Vector3 direction, Vector3 relativePos, MyEntity ignoreMinerShip, MyEntity target, float customMaxDistance = 0, bool isDummy = false, bool isLightWeight = false)
        {
            MyMissile newMissile = m_missiles.Allocate();
            if (newMissile != null)
            {
                newMissile.Start(type, position, initialVelocity, direction, relativePos, ignoreMinerShip, target, customMaxDistance, isDummy, isLightWeight);
            }
            return newMissile;
        }

        public static void Remove(MyMissile missile)
        {
            m_missiles.Deallocate(missile);
        }

        public static int GetActiveCount()
        {
            return m_missiles.GetActiveCount();
        }
    }
}
