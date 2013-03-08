using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventChooseFaction : IMyEvent
    {
        public MyMwcObjectBuilder_FactionEnum PreferredFaction;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadObjectBuilder_FactionEnum(ref PreferredFaction);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteObjectBuilder_FactionEnum(PreferredFaction);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.CHOOSE_FACTION; } }
    }
}
