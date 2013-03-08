using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventSpawnBot : IMyEvent
    {
        public uint SpawnPointId;
        public uint DesiredBotId;
        public int BotsIdx;
        public Vector3 SpawnPosition;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref SpawnPointId)
                && msg.ReadUInt32(ref DesiredBotId)
                && msg.ReadInt32(ref BotsIdx)
                && msg.ReadVector3(ref SpawnPosition);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(SpawnPointId);
            msg.WriteUInt32(DesiredBotId);
            msg.WriteInt32(BotsIdx);
            msg.WriteVector3(SpawnPosition);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.SPAWN_BOT; } }
    }
}
