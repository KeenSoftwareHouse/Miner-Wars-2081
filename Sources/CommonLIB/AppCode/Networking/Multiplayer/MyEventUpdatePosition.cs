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
    public struct MyEventUpdatePosition : IMyEvent
    {
        // 64 B
        public uint EntityId; // 4B
        public MyMwcPositionAndOrientation Position; // 36B
        public Vector3 Velocity; // 12B
        public Vector3 Acceleration; // 12B

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId) 
                && msg.ReadPositionAndOrientation(ref Position)
                && msg.ReadVector3(ref Velocity)
                && msg.ReadVector3(ref Acceleration);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WritePositionAndOrientation(Position);
            msg.WriteVector3(Velocity);
            msg.WriteVector3(Acceleration);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.UPDATE_POSITION; } }
    }
}
