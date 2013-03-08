using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using Lidgren.Network;
using System.Net;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventSavePlayer : IMyEvent
    {
        public MyMwcObjectBuilder_Player PlayerObjectBuilder;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadObjectBuilder(ref PlayerObjectBuilder);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteObjectBuilder(PlayerObjectBuilder);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.SAVE_PLAYER; } }
    }
}
