using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventChooseFactionResponse : IMyEvent
    {
        public MyMwcObjectBuilder_FactionEnum AssignedFaction;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadObjectBuilder_FactionEnum(ref AssignedFaction);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteObjectBuilder_FactionEnum(AssignedFaction);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.CHOOSE_FACTION_RESPONSE; } }
    }
}
