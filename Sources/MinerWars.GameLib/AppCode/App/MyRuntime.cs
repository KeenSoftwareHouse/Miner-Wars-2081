using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.GameServices;

namespace MinerWars.AppCode.App
{
    public static class MyRuntime
    {
        public static void Run(ServiceContainer services)
        {
            using (MyMinerGame game = new MyMinerGame(services))
            {
                if (game.IsGraphicsSupported())
                    game.Run();
            }
        }

        public static bool InvokeRequired
        {
            get
            {
                return MyMinerGame.Static != null && MyMinerGame.Static.InvokeRequired;
            }
        }

        public static void Invoke(Action action, bool waitForComplete)
        {
            MyMinerGame.Static.Invoke(action, waitForComplete);
        }
    }
}
