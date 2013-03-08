using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public struct MyMessageQueueItem
    {
        public IPEndPoint EndPoint;
        public NetOutgoingMessage Message;
        public NetDeliveryMethod DeliveryMethod;
        public int SequenceChannel;
    }
}
