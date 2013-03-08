using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventUpdatePositionFast : IMyEvent
    {
        // 16 B
        public uint EntityId; // 4B
        public Vector3 Position; // 12B
        public Vector3? Up;
        public Vector3? Forward;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadVector3(ref Position)
                && msg.ReadVector3Nullable(ref Up)
                && msg.ReadVector3Nullable(ref Forward);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WriteVector3(Position);
            msg.WriteVector3Nullable(Up);
            msg.WriteVector3Nullable(Forward);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.UPDATE_POSITION_FAST; } }
    }
}
