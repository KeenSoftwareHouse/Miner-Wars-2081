using MinerWars.CommonLIB.AppCode.Generics;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    using System;
    using System.Collections.Generic;
    using MinerWarsMath;
    using SysUtils.Utils;
    using Utils;
using MinerWars.AppCode.Game.Sessions;

    interface IUniversalLauncherShell
    {
        /// <summary>
        /// Gets or sets an optional time to activate the shell, in milliseconeds.
        /// </summary>
        int? TimeToActivate { get; set; }

        void Init();
        void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner);
    }

    interface IUniversalLauncherShellsPool
    {
        void InitAll();
        void ReloadAll();

        T Allocate<T>() where T : class, IUniversalLauncherShell, new();
        void Remove(IUniversalLauncherShell item);
        int GetActiveCount();
    }

    /// <summary>
    /// Preallocates various ammo types used in universal launcher
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class MyUniversalLauncherShellsPool<T> : MyObjectsPool<T>, IUniversalLauncherShellsPool where T : class, IUniversalLauncherShell, new() 
    {
        public MyUniversalLauncherShellsPool(int capacity):base(capacity)
        { }

        public void InitAll()
        {
            foreach (LinkedListNode<T> item in GetPreallocatedItemsArray())
            {
                item.Value.Init();
            }
        }

        public void ReloadAll()
        {
            foreach (LinkedListNode<T> item in GetPreallocatedItemsArray())
            {
                var entity = item.Value as MyEntity;
                if (entity != null)
                {
                    if (entity.ModelLod0 != null)
                        entity.ModelLod0.LoadData();
                    if (entity.ModelLod1 != null)
                        entity.ModelLod1.LoadData();
                    if (entity.ModelLod2 != null)
                        entity.ModelLod2.LoadData();
                }
            }
        }

        public U Allocate<U>() where U : class, IUniversalLauncherShell, new()
        {
            return Allocate(true) as U;
        }

        public void Remove(IUniversalLauncherShell item) 
        {
            Deallocate(item as T);
        }
    }

    static class MyUniversalLauncherShells
    {
        static Dictionary<Type, IUniversalLauncherShellsPool>[] m_objectsPools;
        static bool poolsCreated = false;

        public static void LoadData(bool isMultiplayer)
        {
            int poolsCount;
            if (isMultiplayer)
                poolsCount = MyMultiplayerGameplay.MaxPlayers;
            else
                poolsCount = 1;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyUniversalLauncherShells.LoadData");

            MyMwcLog.WriteLine("MyPhysObjectUniversalLauncherLoads.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            if (!poolsCreated || m_objectsPools.Length < poolsCount)
            {
                m_objectsPools = new Dictionary<Type, IUniversalLauncherShellsPool>[poolsCount];

                for (int i = 0; i < m_objectsPools.Length; i++)
                {
                    var item = m_objectsPools[i] = new Dictionary<Type, IUniversalLauncherShellsPool>();
                    createShellsPool<MyMineBasic>(MyUniversalLauncherConstants.MAX_BASICMINES_COUNT, item);
                    createShellsPool<MyMineBioChem>(MyUniversalLauncherConstants.MAX_BIOCHEMMINES_COUNT, item);
                    createShellsPool<MyMineSmart>(MyUniversalLauncherConstants.MAX_SMARTMINES_COUNT, item);
                    createShellsPool<MySphereExplosive>(MyUniversalLauncherConstants.MAX_SPHEREEXPLOSIVES_COUNT, item);
                    createShellsPool<MyDecoyFlare>(MyUniversalLauncherConstants.MAX_DECOYFLARES_COUNT, item);
                    createShellsPool<MyFlashBomb>(MyUniversalLauncherConstants.MAX_FLASHBOMBS_COUNT, item);
                    createShellsPool<MyIlluminatingShell>(MyUniversalLauncherConstants.MAX_ILLUMINATINGSHELLS_COUNT, item);
                    createShellsPool<MySmokeBomb>(MyUniversalLauncherConstants.MAX_SMOKEBOMBS_COUNT, item);
                    createShellsPool<MyAsteroidKiller>(MyUniversalLauncherConstants.MAX_ASTEROIDKILLERS_COUNT, item);
                    createShellsPool<MyDirectionalExplosive>(MyUniversalLauncherConstants.MAX_DIRECTIONALEXPLOSIVES_COUNT, item);
                    createShellsPool<MyTimeBomb>(MyUniversalLauncherConstants.MAX_TIMEBOMBS_COUNT, item);
                    createShellsPool<MyRemoteBomb>(MyUniversalLauncherConstants.MAX_REMOTEBOMBS_COUNT, item);
                    createShellsPool<MyGravityBomb>(MyUniversalLauncherConstants.MAX_GRAVITYBOMBS_COUNT, item);
                    createShellsPool<MyHologram>(MyUniversalLauncherConstants.MAX_HOLOGRAMS_COUNT, item);
                    createShellsPool<MyRemoteCamera>(MyUniversalLauncherConstants.MAX_REMOTECAMERAS_COUNT, item);
                    createShellsPool<MyEMPBomb>(MyUniversalLauncherConstants.MAX_EMPBOMB_COUNT, item);
                }
                poolsCreated = true;
            }
            else
            {
                foreach (var item in m_objectsPools)
                {
                    reloadShellsPool<MyMineBasic>(item);
                    reloadShellsPool<MyMineBioChem>(item);
                    reloadShellsPool<MyMineSmart>(item);
                    reloadShellsPool<MySphereExplosive>(item);
                    reloadShellsPool<MyDecoyFlare>(item);
                    reloadShellsPool<MyFlashBomb>(item);
                    reloadShellsPool<MyIlluminatingShell>(item);
                    reloadShellsPool<MySmokeBomb>(item);
                    reloadShellsPool<MyAsteroidKiller>(item);
                    reloadShellsPool<MyDirectionalExplosive>(item);
                    reloadShellsPool<MyTimeBomb>(item);
                    reloadShellsPool<MyRemoteBomb>(item);
                    reloadShellsPool<MyGravityBomb>(item);
                    reloadShellsPool<MyHologram>(item);
                    reloadShellsPool<MyRemoteCamera>(item);
                    reloadShellsPool<MyEMPBomb>(item);
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyUniversalLauncherShells.LoadContent() - END");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        public static void UnloadData()
        {
        }

        private static void createShellsPool<T>(int capacity, Dictionary<Type, IUniversalLauncherShellsPool> item) where T : class, IUniversalLauncherShell, new() 
        {
            IUniversalLauncherShellsPool pool = new MyUniversalLauncherShellsPool<T>(capacity);
            item.Add(typeof(T), pool);

            pool.InitAll();
        }

        private static void reloadShellsPool<T>(Dictionary<Type, IUniversalLauncherShellsPool> item) where T : class, IUniversalLauncherShell
        {
            var pool = item[typeof(T)];

            pool.ReloadAll();
        }

        //  Add new shell to the list
        public static T Allocate<T>(byte userId) where T : class, IUniversalLauncherShell, new() 
        {
            var pools = m_objectsPools[userId];
            IUniversalLauncherShellsPool pool = pools[typeof(T)];

            T newItem = pool.Allocate<T>();

            return newItem;
        }

        public static void Remove(IUniversalLauncherShell shell, byte userId)
        {
            var pools = m_objectsPools[userId];
            IUniversalLauncherShellsPool pool = pools[shell.GetType()];
            pool.Remove(shell);
            MyEntities.RemoveFromClosedEntities(shell as MyEntity);
        }

    }
}
