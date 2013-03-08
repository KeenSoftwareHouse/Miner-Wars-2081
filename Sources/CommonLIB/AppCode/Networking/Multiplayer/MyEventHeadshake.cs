using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventHeadshake : IMyEvent
    {
        public float Amount;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadFloat(ref Amount);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteFloat(Amount);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.HEADSHAKE; } }
    }
}
