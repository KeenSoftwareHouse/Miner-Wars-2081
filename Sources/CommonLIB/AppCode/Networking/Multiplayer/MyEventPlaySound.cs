using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventPlaySound : IMyEvent
    {
        public int SoundEnum;
        public Vector3? Position;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt32(ref SoundEnum)
                && msg.ReadVector3Nullable(ref Position);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32(SoundEnum);
            msg.WriteVector3Nullable(Position);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.PLAY_SOUND; } }
    }
}
