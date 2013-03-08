using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MyGlobalFlagsEnum: byte
    {
        REGENERATE_WAYPOINTS,
        REQUEST_INFO,
    }

    public struct MyEventGlobalFlag : IMyEvent
    {
        public MyGlobalFlagsEnum Flag;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadEnum(ref Flag);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteEnum(Flag);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.GLOBAL_FLAGS; } }
    }
}
