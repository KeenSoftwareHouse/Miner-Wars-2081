using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventCreateGame : IMyEvent
    {
        public string SectorName;
        public string Password;
        public MyGameTypes Type;
        public MyJoinMode JoinMode;
        public MyGameplayDifficultyEnum Difficulty;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadString(ref SectorName)
                && msg.ReadString(ref Password)
                && msg.ReadEnum(ref Type)
                && msg.ReadEnum(ref JoinMode)
                && msg.ReadEnum(ref Difficulty);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteString(SectorName);
            msg.WriteString(Password);
            msg.WriteEnum(Type);
            msg.WriteEnum(JoinMode);
            msg.WriteEnum(Difficulty);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.CREATE_GAME; } }
    }
}
