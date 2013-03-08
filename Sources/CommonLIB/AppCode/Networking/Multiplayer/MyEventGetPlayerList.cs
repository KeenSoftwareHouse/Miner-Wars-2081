using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventGetPlayerList: IMyEvent
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
        public MyEventEnum EventType { get { return MyEventEnum.GET_PLAYER_LIST; } }
    }
}
