using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MyJoinMode
    {
        Open,
        Closed,
    }

    public struct MyGameInfo : IReadWriteMessage
    {
        public uint GameId;
        public string Name;

        public int PlayerCount;
        public int MaxPlayerCount;
        public int HostId;
        public string HostDisplayName;
        public MyGameTypes GameType;
        public MyGameplayDifficultyEnum Difficulty;
        public MyJoinMode JoinMode;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref GameId)
                && msg.ReadString(ref Name)
                && msg.ReadInt32(ref PlayerCount)
                && msg.ReadInt32(ref MaxPlayerCount)
                && msg.ReadInt32(ref HostId)
                && msg.ReadString(ref HostDisplayName)
                && msg.ReadEnum(ref GameType)
                && msg.ReadEnum(ref Difficulty)
                && msg.ReadEnum(ref JoinMode);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(GameId);
            msg.WriteString(Name);
            msg.WriteInt32(PlayerCount);
            msg.WriteInt32(MaxPlayerCount);
            msg.WriteInt32(HostId);
            msg.WriteString(HostDisplayName);
            msg.WriteEnum(GameType);
            msg.WriteEnum(Difficulty);
            msg.WriteEnum(JoinMode);
        }
    }
}
