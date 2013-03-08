using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Net;
using Lidgren.Network;
using SysUtils;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventStatsUpdate : IMyEvent
    {
        public MyMwcObjectBuilder_PlayerStatistics StatsBuilder;

        public bool Read(MyMessageReader msg)
        {            
            StatsBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(msg.Reader, msg.EndPoint) as MyMwcObjectBuilder_PlayerStatistics;
            return StatsBuilder != null && StatsBuilder.Read(msg.Reader, msg.EndPoint, MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION);
        }

        public void Write(MyMessageWriter msg)
        {
            StatsBuilder.Write(msg.Writer);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.STATS_UPDATE; } }
    }
}
