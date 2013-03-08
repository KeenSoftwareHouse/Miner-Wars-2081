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
    public struct MyEventCheckpoint : IMyEvent
    {
        public MyMwcObjectBuilder_Checkpoint Checkpoint;

        public bool Read(MyMessageReader msg)
        {
            Checkpoint = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(msg.Reader, msg.EndPoint) as MyMwcObjectBuilder_Checkpoint;
            return Checkpoint != null
                && Checkpoint.Read(msg.Reader, msg.EndPoint, MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION);
        }

        public void Write(MyMessageWriter msg)
        {
            Checkpoint.Write(msg.Writer);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.CHECKPOINT; } }
    }
}
