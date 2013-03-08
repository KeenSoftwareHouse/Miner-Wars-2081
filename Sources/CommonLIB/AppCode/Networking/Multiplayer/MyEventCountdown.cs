using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventCountdown: IMyEvent
    {
        public long m_ticks;

        public TimeSpan Timespan
        {
            get
            {
                return TimeSpan.FromTicks(m_ticks);
            }
            set
            {
                m_ticks = value.Ticks;
            }
        }

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt64(ref m_ticks);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt64(m_ticks);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.COUNTDOWN; } }
    }
}
