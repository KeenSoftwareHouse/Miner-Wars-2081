using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventLogin : IMyEvent
    {
        public int AppVersion;
        public string Username;
        public string PasswordHash;
        public IPEndPoint InternalEndpoint;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt32(ref AppVersion)
                && msg.ReadString(ref Username)
                && msg.ReadString(ref PasswordHash)
                && msg.ReadIPEndPoint(ref InternalEndpoint);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32(AppVersion);
            msg.WriteString(Username);
            msg.WriteString(PasswordHash);
            msg.WriteIPEndPoint(InternalEndpoint);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.LOGIN; } }
    }
}
