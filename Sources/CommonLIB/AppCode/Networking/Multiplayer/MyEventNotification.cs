using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MyNotificationType
    {
        Text,
        WaitStart,
        WaitEnd,
    }

    public struct MyEventNotification : IMyEvent
    {
        public MyNotificationType Type;
        public int Text;
        public string Arg0;
        public string Arg1;
        public string Arg2;
        public string Arg3;

        public bool Read(MyMessageReader msg)
        {
            return  msg.ReadEnum(ref Type)
                && msg.ReadInt32(ref Text)
                && msg.ReadStringNullable(ref Arg0)
                && msg.ReadStringNullable(ref Arg1)
                && msg.ReadStringNullable(ref Arg2)
                && msg.ReadStringNullable(ref Arg3);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteEnum(Type);
            msg.WriteInt32(Text);
            msg.WriteStringNullable(Arg0);
            msg.WriteStringNullable(Arg1);
            msg.WriteStringNullable(Arg2);
            msg.WriteStringNullable(Arg3);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.NOTIFICATION; } }
    }
}
