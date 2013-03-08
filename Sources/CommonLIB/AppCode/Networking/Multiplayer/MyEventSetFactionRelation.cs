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
    public struct MyEventSetFactionRelation : IMyEvent
    {
        public MyMwcObjectBuilder_FactionEnum FactionA;
        public MyMwcObjectBuilder_FactionEnum FactionB;
        public float Relation;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadEnum(ref FactionA)
                && msg.ReadEnum(ref FactionB)
                && msg.ReadFloat(ref Relation);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteEnum(FactionA);
            msg.WriteEnum(FactionB);
            msg.WriteFloat(Relation);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.SET_FACTION_RELATION; } }
    }
}
