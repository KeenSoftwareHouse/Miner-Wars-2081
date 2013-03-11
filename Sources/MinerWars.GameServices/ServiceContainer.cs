using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MinerWars.GameServices
{
    public class ServiceContainer
    {
        public readonly IMySteam Steam;
        public readonly bool RunningMod;
        public readonly string ModDir;
        public readonly string AppDir;

        public ServiceContainer(IMySteam steam, string appDir, string modDir)
        {
            this.Steam = steam;
            this.AppDir = appDir;
            this.ModDir = modDir;
            this.RunningMod = modDir != null;
        }
    }
}
