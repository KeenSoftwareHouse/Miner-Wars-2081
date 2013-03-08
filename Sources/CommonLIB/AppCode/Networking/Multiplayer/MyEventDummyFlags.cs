using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventDummyFlags : IMyEvent
    {
        public uint EntityId;
        public MyDummyPointFlags Flags;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadEnum(ref Flags);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WriteEnum(Flags);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.DUMMY_FLAGS; } }
    }
}
