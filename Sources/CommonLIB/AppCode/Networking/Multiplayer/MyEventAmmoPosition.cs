using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.Xna.Framework;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventAmmoPosition : IMyEvent
    {
        // 64 B
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

        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.AMMO_POSITION; } }
    }
}
