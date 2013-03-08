using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    [Flags]
    public enum MyGameTypes
    {
        None       = 0,
        Story      = 1 << 0,
        Deathmatch = 1 << 1,
        Editor     = 1 << 2,
    }

    public struct MyEventGetGames: IMyEvent
    {
        public string NameFilter;
        public MyGameTypes GameTypeFilter;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadString(ref NameFilter)
                && msg.ReadEnum<MyGameTypes>(ref GameTypeFilter);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteString(NameFilter);
            msg.WriteEnum(GameTypeFilter);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.GET_GAMES; } }
    }
}
