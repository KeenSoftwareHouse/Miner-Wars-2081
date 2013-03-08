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
    public struct MyEventSetActorFaction : IMyEvent
    {
        public MyMwcObjectBuilder_FactionEnum Faction;
        public int ActorId;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadEnum(ref Faction)
                && msg.ReadInt32(ref ActorId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteEnum(Faction);
            msg.WriteInt32(ActorId);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.SET_ACTOR_FACTION; } }
    }
}
