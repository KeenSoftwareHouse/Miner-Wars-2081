using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventChat : IMyEvent
    {
        public string Message;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadString(ref Message);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteString(Message);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.CHAT; } }
    }
}
