using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventPilotDie : IMyEvent
    {
        public uint EntityId;
        public byte? KillerId;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadByteNullable(ref KillerId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WriteByteNullable(KillerId);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.PILOT_DIE; } }
    }
}
