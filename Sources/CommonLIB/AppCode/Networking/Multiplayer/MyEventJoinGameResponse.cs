using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventJoinGameResponse : IMyEvent
    {
        public MyResultCodeEnum ResultCode;
        public int HostUserId;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadResultCode(ref ResultCode)
                && msg.ReadInt32(ref HostUserId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteResultCode(ResultCode);
            msg.WriteInt32(HostUserId);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.JOIN_GAME_RESPONSE; } }
    }
}
