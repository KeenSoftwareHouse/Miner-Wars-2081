using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    // Synces mission variabled (timer so far)
    public struct MyEventMissionUpdateVars : IMyEvent
    {
        public int MissionId;
        public int ElapsedTime;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt32(ref MissionId)
                && msg.ReadInt32(ref ElapsedTime);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32(MissionId);
            msg.WriteInt32(ElapsedTime);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.MISSION_UPDATE_VARS; } }


        public override string ToString()
        {
            return string.Format("MissionId: {0}, SenderConnection: {1}, SenderEndpoint: {2}", MissionId, SenderConnection, SenderEndpoint);
        }
    }
}
