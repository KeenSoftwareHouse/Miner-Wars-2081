using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventEvent : IMyEvent
    {
        public MyMwcPositionAndOrientation Position;
        public int EventTypeEnum;
        public int Seed;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadPositionAndOrientation(ref Position)
                && msg.ReadInt32(ref EventTypeEnum)
                && msg.ReadInt32(ref Seed);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WritePositionAndOrientation(Position);
            msg.WriteInt32(EventTypeEnum);
            msg.WriteInt32(Seed);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.EVENT; } }
    }
}
