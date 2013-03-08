using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public interface IMyEvent
    {
        bool Read(MyMessageReader msg);
        void Write(MyMessageWriter msg);

        NetConnection SenderConnection { get; set; }
        IPEndPoint SenderEndpoint { get; set; }
        MyEventEnum EventType { get; }
    }
}
