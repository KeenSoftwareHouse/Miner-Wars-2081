using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventSetEntityFaction : IMyEvent
    {
        public MyMwcObjectBuilder_FactionEnum Faction;
        public uint EntityId;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadEnum(ref Faction)
                && msg.ReadUInt32(ref EntityId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteEnum(Faction);
            msg.WriteUInt32(EntityId);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.SET_ENTITY_FACTION; } }
    }
}
