using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyPlayerInfo : IReadWriteMessage
    {
        public int UserId;
        public byte PlayerId;
        public string DisplayName;
        public MyMwcObjectBuilder_FactionEnum Faction;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt32(ref UserId)
                && msg.ReadString(ref DisplayName)
                && msg.ReadByte(ref PlayerId)
                && msg.ReadObjectBuilder_FactionEnum(ref Faction);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32(UserId);
            msg.WriteString(DisplayName);
            msg.WriteByte(PlayerId);
            msg.WriteObjectBuilder_FactionEnum(Faction);
        }
    }
}
