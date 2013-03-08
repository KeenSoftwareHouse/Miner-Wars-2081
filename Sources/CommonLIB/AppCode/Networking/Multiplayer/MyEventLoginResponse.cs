using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventLoginResponse : IMyEvent
    {
        public MyResultCodeEnum ResultCode;
        public int MultiplayerUserId;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadResultCode(ref ResultCode)
                && msg.ReadInt32(ref MultiplayerUserId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteResultCode(ResultCode);
            msg.WriteInt32(MultiplayerUserId);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.LOGIN_RESPONSE; } }
    }
}
