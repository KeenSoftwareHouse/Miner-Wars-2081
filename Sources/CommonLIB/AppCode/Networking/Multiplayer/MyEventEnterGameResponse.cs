using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventEnterGameResponse : IMyEvent
    {
        public bool Allowed;
        public byte PlayerId;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadBool(ref Allowed) 
                && msg.ReadByte(ref PlayerId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteBool(Allowed);
            msg.WriteByte(PlayerId);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.ENTER_GAME_RESPONSE; } }
    }
}
