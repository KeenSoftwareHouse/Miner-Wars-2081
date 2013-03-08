using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.PluginAPI
{
    public interface MyIPlugin
    {
        MyIPluginHost Host { get; set; }
        
        string GetAudioFolder();
    }
}
