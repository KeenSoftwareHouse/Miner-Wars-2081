using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventGetPlayerListResponse : IMyEvent
    {
        public List<MyPlayerInfo> PlayerList;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadList(ref PlayerList);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteList(PlayerList);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.GET_PLAYER_LIST_RESPONSE; } }
    }
}
