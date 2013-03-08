using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventJoinGame: IMyEvent
    {
        public uint GameId;
        public string Password;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref GameId)
                && msg.ReadString(ref Password);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(GameId);
            msg.WriteString(Password);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.JOIN_GAME; } }
    }
}
