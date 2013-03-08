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
    public struct MyEventAddVoxelHand : IMyEvent
    {
        public uint VoxelMapEntityId;
        public uint EntityId;
        public float Radius;
        public MyMwcVoxelHandModeTypeEnum HandMode;
        public MyMwcVoxelMaterialsEnum? VoxelMaterial;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref VoxelMapEntityId)
                && msg.ReadUInt32(ref EntityId)
                && msg.ReadFloat(ref Radius)
                && msg.ReadEnum(ref HandMode)
                && msg.ReadEnumNullable(ref VoxelMaterial);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(VoxelMapEntityId);
            msg.WriteUInt32(EntityId);
            msg.WriteFloat(Radius);
            msg.WriteEnum(HandMode);
            msg.WriteEnumNullable(VoxelMaterial);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.ADD_VOXEL_HAND; } }
    }
}
