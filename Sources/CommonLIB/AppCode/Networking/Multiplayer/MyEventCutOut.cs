using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventCutOut : IMyEvent
    {
        public uint VoxelMapEntityId;
        public Vector3 Position;
        public float Radius;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref VoxelMapEntityId)
                && msg.ReadVector3(ref Position)
                && msg.ReadFloat(ref Radius);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(VoxelMapEntityId);
            msg.WriteVector3(Position);
            msg.WriteFloat(Radius);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.CUT_OUT; } }
    }
}
