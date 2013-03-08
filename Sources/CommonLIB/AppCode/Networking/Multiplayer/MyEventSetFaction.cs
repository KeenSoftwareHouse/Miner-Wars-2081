using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventSetFaction : IMyEvent
    {
        public MyMwcObjectBuilder_FactionEnum Faction;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadObjectBuilder_FactionEnum(ref Faction);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteObjectBuilder_FactionEnum(Faction);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.SET_FACTION; } }
    }
}
