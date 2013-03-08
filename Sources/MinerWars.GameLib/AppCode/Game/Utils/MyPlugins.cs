using System;
using System.IO;
using MinerWars.AppCode.App;
using MinerWars.PluginAPI;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Utils
{
    public static class MyPlugins
    {
        static readonly string PLUGIN_FOLDER_RELATIVE = @"\SimplePlugin\bin\x86\Debug\";
        static MyPluginLoader m_pluginLoader;
        static string m_pluginFolderAbsolute = Environment.CurrentDirectory + PLUGIN_FOLDER_RELATIVE;
        
        public static void LoadContent()
        {
            return;

            MyMwcLog.WriteLine("MyPlugins.LoadContent - START");
            MyMwcLog.IncreaseIndent();

            string pluginFile = m_pluginFolderAbsolute + "SimplePlugin.dll";
            MyMwcLog.WriteLine("PluginFile: " + pluginFile);

            if (File.Exists(pluginFile) == false)
            {
                MyMwcLog.WriteLine("Plugin file not found, therefore skipping plugin loading");
            }
            else
            {
                //  There can be an exception during loading the plugin, so we must check it, but continue even if exception occurs
                try
                {
                    m_pluginLoader = new MyPluginLoader(pluginFile);
                }
                catch (Exception ex)
                {
                    MyMwcLog.WriteLine("Exception during loading the plugin (BUT APPLICATION WILL CONTINUE): " + ex.ToString());
                    m_pluginLoader = null;  //  Because plugin wasn't really loaded!!
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyPlugins.LoadContent - END");
        }

        public static string GetAudioFolder()
        {
            if ((m_pluginLoader == null) || (m_pluginLoader.Plugin.GetAudioFolder() == null))
            {
                return MyMinerGame.Static.RootDirectory + @"\Audio\";
            }
            else
            {
                return m_pluginFolderAbsolute + m_pluginLoader.Plugin.GetAudioFolder();
            }
        }
    }
}
