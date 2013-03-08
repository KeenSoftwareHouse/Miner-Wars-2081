using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventNewPlayer : IMyEvent
    {
        public MyPlayerInfo PlayerInfo;

        public bool Read(MyMessageReader msg)
        {
            return PlayerInfo.Read(msg);
        }

        public void Write(MyMessageWriter msg)
        {
            PlayerInfo.Write(msg);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.NEW_PLAYER; } }
    }
}
