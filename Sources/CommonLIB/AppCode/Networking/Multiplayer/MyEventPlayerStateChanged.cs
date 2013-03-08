using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MyMultiplayerStateEnum: byte
    {
        Joined,
        Playing,
        Disconnected,
    }

    public struct MyEventPlayerStateChanged : IMyEvent
    {
        public int UserId;
        public MyMultiplayerStateEnum NewState;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt32(ref UserId)
                && msg.ReadMultiplayerStateEnum(ref NewState);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32(UserId);
            msg.WriteMultiplayerStateEnum(NewState);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.PLAYER_STATE_CHANGED; } }
    }
}
