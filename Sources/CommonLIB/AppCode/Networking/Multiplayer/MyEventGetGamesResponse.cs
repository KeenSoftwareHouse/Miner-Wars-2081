using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventGetGamesResponse : IMyEvent
    {
        public List<MyGameInfo> Games;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadList(ref Games);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteList(Games);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.GET_GAMES_RESPONSE; } }
    }
}
