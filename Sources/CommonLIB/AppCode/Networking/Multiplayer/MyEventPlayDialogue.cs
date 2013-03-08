using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventPlayDialogue : IMyEvent
    {
        public int DialogueEnum;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt32(ref DialogueEnum);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32(DialogueEnum);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.PLAY_DIALOGUE; } }
    }
}
