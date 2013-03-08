using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MyFlagsEnum: byte
    {
        ENABLE,
        DISABLE,
        HIDE,
        UNHIDE,
        PARK_SHIP,
        CLOSE,
        PARTICLE,
        NUCLEAR_EXPLOSION,
        DESTRUCTIBLE,
        INDESTRUCTIBLE,
        PREPARE_MOVE,
        RETURN_FROM_MOVE,
    }

    public struct MyEventFlags : IMyEvent
    {
        public uint EntityId;
        public MyFlagsEnum Flag;
        public bool Param;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadEnum(ref Flag)
                && msg.ReadBool(ref Param);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WriteEnum(Flag);
            msg.WriteBool(Param);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.FLAGS; } }
    }
}
