using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventEntityReset: IMyEvent
    {
        public uint EntityId;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.ENTITY_RESET; } }
    }
}
