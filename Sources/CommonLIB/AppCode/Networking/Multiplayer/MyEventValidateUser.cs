using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventValidateUser: IMyEvent
    {
        public string Token;
        public long SteamUserId;
        public byte[] SteamTicket;

        public bool Read(MyMessageReader msg)
        {
            ushort ticketLength = 0;
            SteamTicket = null;

            return msg.ReadString(ref Token)
                && msg.ReadInt64(ref SteamUserId)
                && msg.ReadUInt16(ref ticketLength)
                && msg.ReadBytes(ref SteamTicket, ticketLength);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteString(Token);
            msg.WriteInt64(SteamUserId);
            msg.WriteUInt16((ushort)SteamTicket.Length);
            msg.WriteBytes(SteamTicket);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.VALIDATE_USER; } }
    }
}
