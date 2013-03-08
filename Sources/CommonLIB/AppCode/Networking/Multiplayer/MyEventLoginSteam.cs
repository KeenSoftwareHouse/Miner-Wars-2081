using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventLoginSteam : IMyEvent
    {
        public int AppVersion;
        public string DisplayName;
        public long SteamUserId;
        public byte[] SteamTicket;
        public IPEndPoint InternalEndpoint;

        public bool Read(MyMessageReader msg)
        {
            ushort ticketLength = 0;
            SteamTicket = null;

            return msg.ReadInt32(ref AppVersion)
                && msg.ReadString(ref DisplayName)
                && msg.ReadInt64(ref SteamUserId)
                && msg.ReadUInt16(ref ticketLength)
                && msg.ReadBytes(ref SteamTicket, ticketLength)
                && msg.ReadIPEndPoint(ref InternalEndpoint);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32(AppVersion);
            msg.WriteString(DisplayName);
            msg.WriteInt64(SteamUserId);
            msg.WriteUInt16((ushort)SteamTicket.Length);
            msg.WriteBytes(SteamTicket);
            msg.WriteIPEndPoint(InternalEndpoint);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.LOGIN_STEAM; } }
    }
}
