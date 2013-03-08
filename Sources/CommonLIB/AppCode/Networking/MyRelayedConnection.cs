using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public class MyRelayedConnection : NetConnection
    {
        public IPEndPoint RelayTargetEp;

        public MyRelayedConnection(NetPeer peer)
            :base(peer, null)
        {
        }
    }
}
