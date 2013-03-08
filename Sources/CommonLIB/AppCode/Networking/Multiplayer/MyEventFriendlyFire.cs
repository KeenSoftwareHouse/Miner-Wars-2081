using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MyFriendlyFireEnum
    {
        AGGRO, // Turn bots to enemies
        GAME_FAILED, // Shows gameover screen
    }

    public struct MyEventFriendlyFire : IMyEvent
    {
        public MyFriendlyFireEnum FriendlyFireType;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadEnum(ref FriendlyFireType);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteEnum(FriendlyFireType);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.FRIENDLY_FIRE; } }
    }
}
