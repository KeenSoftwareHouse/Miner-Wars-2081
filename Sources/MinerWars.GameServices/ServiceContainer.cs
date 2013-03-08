using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MinerWars.GameServices
{
    public class ServiceContainer
    {
        private static string m_appDir;

        public readonly IMySteam Steam;
        public readonly bool RunningMod;
        public readonly string ModDir;

        public ServiceContainer(IMySteam steam, string modDir)
        {
            this.Steam = steam;
            this.ModDir = modDir;
            this.RunningMod = modDir != null;
        }

        public static string AppDir
        {
            get
            {
                if (m_appDir == null)
                {
                    string directoryName = string.Empty;
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly == null)
                    {
                        entryAssembly = Assembly.GetCallingAssembly();
                    }
                    if (entryAssembly != null)
                    {
                        directoryName = System.IO.Path.GetDirectoryName(entryAssembly.Location);
                    }
                    m_appDir = directoryName;
                }
                return m_appDir;
            }
        }
    }
}
