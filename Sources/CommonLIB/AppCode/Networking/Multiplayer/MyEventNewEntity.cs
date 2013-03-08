using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventNewEntity : IMyEvent
    {
        public MyMwcPositionAndOrientation Position;
        public MyMwcObjectBuilder_Base ObjectBuilder;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadPositionAndOrientation(ref Position)
                && msg.ReadObjectBuilder(ref ObjectBuilder);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WritePositionAndOrientation(Position);
            msg.WriteObjectBuilder(ObjectBuilder);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.NEW_ENTITY; } }
    }
}
