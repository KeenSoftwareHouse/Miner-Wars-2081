using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventEnterGame : IMyEvent
    {
        static Action<MyMessageWriter, int> m_writer = (msg, value) => msg.WriteInt32(value);
        static ReadHandler<int> m_reader = (msg) => { int result = 0; return msg.ReadInt32(ref result) ? result : (int?)null; };

        public MyPlayerInfo PlayerInfo;
        public List<int> ConnectedPlayers;

        public bool Read(MyMessageReader msg)
        {
            return PlayerInfo.Read(msg)
                && msg.ReadList(ref ConnectedPlayers, m_reader);
        }

        public void Write(MyMessageWriter msg)
        {
            PlayerInfo.Write(msg);
            msg.WriteList(ConnectedPlayers, m_writer);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.ENTER_GAME; } }
    }
}
