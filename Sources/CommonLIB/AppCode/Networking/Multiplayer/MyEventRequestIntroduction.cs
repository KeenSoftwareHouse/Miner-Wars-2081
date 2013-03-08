using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventRequestIntroduction : IMyEvent
    {
        static Action<MyMessageWriter, int> m_writer = (msg, value) => msg.WriteInt32(value);
        static ReadHandler<int> m_reader = (msg) => { int result = 0; return msg.ReadInt32(ref result) ? result : (int?)null; };

        public uint GameId;
        public List<int> UserList;
        public bool RelayIntroduction;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref GameId)
                && msg.ReadList(ref UserList, m_reader)
                && msg.ReadBool(ref RelayIntroduction);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(GameId);
            msg.WriteList(UserList, m_writer);
            msg.WriteBool(RelayIntroduction);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.REQUEST_INTRODUCTION; } }
    }
}
