using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MyMissionProgressType
    {
        Success,
        Fail,
        NewObjective,
    }

    public struct MyEventMissionProgress : IMyEvent
    {
        public int? MissionId;
        public MyMissionProgressType ProgressType;
        public int? MessageEnum;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadInt32Nullable(ref MissionId) 
                && msg.ReadEnum<MyMissionProgressType>(ref ProgressType)
                && msg.ReadInt32Nullable(ref MessageEnum);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteInt32Nullable(MissionId);
            msg.WriteEnum<MyMissionProgressType>(ProgressType);
            msg.WriteInt32Nullable(MessageEnum);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.MISSION_PROGRESS; } }


        public override string ToString()
        {
            return string.Format("MissionId: {0}, ProgressType: {1}, SenderConnection: {2}, SenderEndpoint: {3}", MissionId, ProgressType, SenderConnection, SenderEndpoint);
        }
    }
}
