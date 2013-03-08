using System.Collections.Generic;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Explosions
{
    static class MyExplosions
    {
        static MyObjectsPool<MyExplosion> m_explosions = null;

        static MyExplosions()
        {
            //if (MyExplosion.DEBUG_EXPLOSIONS)
            {
                MyRender.RegisterRenderModule(MyRenderModuleEnum.Explosions, "Explosions", DebugDraw, MyRenderStage.DebugDraw, true);
            }
        }


        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyExplosions.LoadData");
            MyMwcLog.WriteLine("MyExplosions.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            if (m_explosions == null)
            {
                m_explosions = new MyObjectsPool<MyExplosion>(MyExplosionsConstants.MAX_EXPLOSIONS_COUNT);
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyExplosions.LoadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            foreach (LinkedListNode<MyExplosion> explosion in m_explosions)
            {
                explosion.Value.Close();
            }

            m_explosions.DeallocateAll();
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyExplosions.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyExplosions.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyExplosions.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyExplosions.UnloadContent - END");
        }

        //  Add new explosion to the list, but caller needs to start it using Start() method
        public static MyExplosion AddExplosion()
        {
            return m_explosions.Allocate();
        }

        //  We have only Update method for explosions, because drawing of explosion is mantained by particles and lights itself
        public static void Update()
        {
            //  Go over every active explosion and draw it, unless it isn't dead.
            foreach (LinkedListNode<MyExplosion> item in m_explosions)
            {
                if (item.Value.Update() == false)
                {
                    m_explosions.MarkForDeallocate(item);
                }
            }

            //  Deallocate/delete all lights that are turned off
            m_explosions.DeallocateAllMarked();
        }

        public static void DebugDraw()
        {
            //  Go over every active explosion and draw it, unless it isn't dead.
            foreach (LinkedListNode<MyExplosion> item in m_explosions)
            {
                item.Value.DebugDraw();
            }
        }
    }
}
