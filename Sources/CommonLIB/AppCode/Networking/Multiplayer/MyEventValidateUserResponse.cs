using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventValidateUserResponse : IMyEvent
    {
        public byte[] Signature;

        public bool Read(MyMessageReader msg)
        {
            ushort ticketLength = 0;
            Signature = null;

            return msg.ReadUInt16(ref ticketLength)
                && msg.ReadBytes(ref Signature, ticketLength);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt16((ushort)Signature.Length);
            msg.WriteBytes(Signature);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.VALIDATE_USER_RESPONSE; } }
    }
}
