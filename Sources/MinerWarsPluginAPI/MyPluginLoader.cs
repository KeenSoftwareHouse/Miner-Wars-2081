using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MinerWars.PluginAPI
{
    public class MyPluginLoader : MyIPluginHost
    {
        public MyIPlugin Plugin;


        private MyPluginLoader() { }

        //  IMPORTANT: Caller of this constructor must check if dll file exists and better put it into try/catch
        public MyPluginLoader(string dllPath)
        {
            Type objType = null;

            Assembly ass = Assembly.LoadFile(dllPath);
            objType = ass.GetType("MinerWars.SimplePlugin.MySimplePlugin");

            Plugin = (MyIPlugin)Activator.CreateInstance(objType);
            Plugin.Host = this;
        }
    }
}
