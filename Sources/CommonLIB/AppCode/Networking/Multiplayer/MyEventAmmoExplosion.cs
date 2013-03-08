using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventAmmoExplosion : IMyEvent
    {
        public uint AmmoBaseEntityId;
        public MyMwcPositionAndOrientation Position;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref AmmoBaseEntityId)
                && msg.ReadPositionAndOrientation(ref Position);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(AmmoBaseEntityId);
            msg.WritePositionAndOrientation(Position);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.AMMO_EXPLOSION; } }
    }
}
