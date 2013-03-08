using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventUpdateGame : IMyEvent
    {
        public string Name;
        public string Password;
        public MyJoinMode JoinMode;


        public bool Read(MyMessageReader msg)
        {
            return msg.ReadStringNullable(ref Name)
                && msg.ReadStringNullable(ref Password)
                && msg.ReadEnum(ref JoinMode);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteStringNullable(Name);
            msg.WriteStringNullable(Password);
            msg.WriteEnum(JoinMode);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.UPDATE_GAME; } }
    }
}
