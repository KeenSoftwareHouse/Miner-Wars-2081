using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventLockResult : IMyEvent
    {
        public uint EntityId;
        public bool IsSuccess;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadBool(ref IsSuccess);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WriteBool(IsSuccess);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.LOCK_RESULT; } }
    }
}
