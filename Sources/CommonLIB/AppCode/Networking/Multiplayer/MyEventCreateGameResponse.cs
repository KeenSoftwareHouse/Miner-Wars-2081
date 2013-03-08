using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Net;
using System.IO;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventCreateGameResponse : IMyEvent
    {
        public bool Read(MyMessageReader msg)
        {
            return true;
        }

        public void Write(MyMessageWriter msg)
        {
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.CRAETE_GAME_RESPONSE; } }
    }
}
