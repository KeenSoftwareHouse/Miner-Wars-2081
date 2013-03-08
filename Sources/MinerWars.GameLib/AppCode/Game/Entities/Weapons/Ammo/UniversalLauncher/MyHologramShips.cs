using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using SysUtils.Utils;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Sessions;

namespace MinerWars.AppCode.Game.Entities.Weapons.Ammo.UniversalLauncher
{
    /// <summary>
    /// This class is responsible for holding list of small ships that are used as holograms only!
    /// Modeled after MyMissiles.cs.
    /// </summary>
    static class MyHologramShips
    {
        private static MyObjectsPool<MySmallShip> m_hologramShips = null;

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyHologramShips.LoadData");

            MyMwcLog.WriteLine("MyHologramShips.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            if (m_hologramShips == null)
                m_hologramShips = new MyObjectsPool<MySmallShip>(MyUniversalLauncherConstants.MAX_HOLOGRAMS_COUNT * MyMultiplayerGameplay.MaxPlayers);
            m_hologramShips.DeallocateAll();

            foreach (var item in m_hologramShips.GetPreallocatedItemsArray())
            {
                // TODO simon - do we need something here?
                //item.Value.Init();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyHologramShips.LoadContent() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
        }

        //  Add new ship to the list
        public static MySmallShip Add(string displayName, MyMwcObjectBuilder_Ship objectBuilder)
        {
            MySmallShip newShip = m_hologramShips.Allocate();
            if (newShip != null)
            {
                newShip.Init(displayName, objectBuilder);

                if (newShip.Physics.Enabled)
                    newShip.Physics.Enabled = false;
                /*
                newShip.Physics.Type = MyConstants.RIGIDBODY_TYPE_DEFAULT;
                newShip.Physics.CollisionLayer = MyConstants.COLLISION_LAYER_UNCOLLIDABLE;
                 */
                newShip.IsHologram = true;

                // we dont want the hologram to have a light
                newShip.RemoveLight();
                newShip.RemoveSubObjectLights();

                newShip.Config.Engine.Level = 1;
                newShip.Config.ReflectorLight.Level = 1;
                newShip.GetReflectorProperties().CurrentBillboardLength = 40;
                newShip.GetReflectorProperties().CurrentBillboardThickness = 6;

                newShip.CastShadows = false;
            }
            return newShip;
        }

        public static void Remove(MySmallShip ship)
        {
            m_hologramShips.Deallocate(ship);
        }

        public static int GetActiveCount()
        {
            return m_hologramShips.GetActiveCount();
        }
    }
}
