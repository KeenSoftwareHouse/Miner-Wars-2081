using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventDirectIntroductionResponse : IMyEvent
    {
        public int UserId;
        public IPEndPoint EndPoint;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt32(ref UserId)
                && msg.ReadIPEndPoint(ref EndPoint);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32(UserId);
            msg.WriteIPEndPoint(EndPoint);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.DIRECT_INTRODUCTION_RESPONSE; } }
    }
}
