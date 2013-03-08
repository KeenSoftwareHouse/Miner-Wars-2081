using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventDie : IMyEvent
    {
        public uint EntityId;
        public MyMwcPositionAndOrientation Position;
        public byte? KillerId;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadPositionAndOrientation(ref Position)
                && msg.ReadByteNullable(ref KillerId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WritePositionAndOrientation(Position);
            msg.WriteByteNullable(KillerId);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.DIE; } }
    }
}
