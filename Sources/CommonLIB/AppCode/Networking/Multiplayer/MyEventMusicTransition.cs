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
    public enum MyMusicEventEnum
    {
        APPLY_TRANSITION,
        STOP_TRANSITION,
        STOP_MUSIC,
    }

    public struct MyEventMusicTransition : IMyEvent
    {
        public MyMusicEventEnum MusicEventType;
        public int? TransitionEnum;
        public int Priority;
        public string Category;
        public bool Loop;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadEnum(ref MusicEventType)
                && msg.ReadInt32Nullable(ref TransitionEnum)
                && msg.ReadInt32(ref Priority)
                && msg.ReadStringNullable(ref Category)
                && msg.ReadBool(ref Loop);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteEnum(MusicEventType);
            msg.WriteInt32Nullable(TransitionEnum);
            msg.WriteInt32(Priority);
            msg.WriteStringNullable(Category);
            msg.WriteBool(Loop);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.MUSIC_TRANSITION; } }
    }
}
