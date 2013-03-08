using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MinerWarsMath;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventAmmoUpdate : IMyEvent
    {
        public uint EntityId; // 4B
        public MyMwcPositionAndOrientation Position; // 36B
        public Vector3 Velocity; // 12B

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadPositionAndOrientation(ref Position)
                && msg.ReadVector3(ref Velocity);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WritePositionAndOrientation(Position);
            msg.WriteVector3(Velocity);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.AMMO_UPDATE; } }
    }
}
