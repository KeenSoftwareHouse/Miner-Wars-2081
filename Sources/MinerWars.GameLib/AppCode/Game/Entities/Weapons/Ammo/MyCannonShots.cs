using MinerWars.CommonLIB.AppCode.Generics;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System.Collections.Generic;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWarsMath;
    using SysUtils.Utils;
    using Utils;

    static class MyCannonShots
    {
        static MyObjectsPool<MyCannonShot> m_missiles = null;

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyCannonShots.LoadData");

            MyMwcLog.WriteLine("MyMissiles.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            if (m_missiles == null)
                m_missiles = new MyObjectsPool<MyCannonShot>(MyMissileConstants.MAX_MISSILES_COUNT);
            m_missiles.DeallocateAll();

            foreach (LinkedListNode<MyCannonShot> item in m_missiles.GetPreallocatedItemsArray())
            {
                item.Value.Init();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyMissiles.LoadContent() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //  Add new missile to the list
        public static void UnloadData()
        {
        }

        public static void Add(Vector3 position, Vector3 initialVelocity, Vector3 direction, MyMwcObjectBuilder_SmallShip_Ammo usedAmmo, MySmallShip ignoreMinerShip)
        {
            MyCannonShot newMissile = m_missiles.Allocate();
            if (newMissile != null)
            {
                newMissile.Start(position, initialVelocity, direction, usedAmmo, ignoreMinerShip);
            }
        }

        //  Update active missiles. If missile dies/timeouts, remove it from the list.
        public static void Remove(MyCannonShot item)
        {
            m_missiles.Deallocate(item);
        }

        public static int GetActiveCount()
        {
            return m_missiles.GetActiveCount();
        }
    }
}