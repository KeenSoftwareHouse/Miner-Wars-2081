using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    [Flags]
    public enum ExplosionFlags : byte
    {
        FORCE_DEBRIS,
        CREATE_DECALS,
    }

    public struct MyEventAddExplosion : IMyEvent
    {
        public uint? EntityId;
        public Vector3? Position;
        public byte ExplosionType;
        public float Radius;
        public float Damage;
        public ExplosionFlags ExplosionFlags;
        public int? ParticleIDOverride;

        public bool CreateDecals
        {
            get
            {
                return (ExplosionFlags & Multiplayer.ExplosionFlags.CREATE_DECALS) != 0;
            }
            set
            {
                if (value)
                    ExplosionFlags |= Multiplayer.ExplosionFlags.CREATE_DECALS;
                else
                    ExplosionFlags &= ~Multiplayer.ExplosionFlags.CREATE_DECALS;
            }
        }

        public bool ForceDebris
        {
            get
            {
                return (ExplosionFlags & Multiplayer.ExplosionFlags.FORCE_DEBRIS) != 0;
            }
            set
            {
                if (value)
                    ExplosionFlags |= Multiplayer.ExplosionFlags.FORCE_DEBRIS;
                else
                    ExplosionFlags &= ~Multiplayer.ExplosionFlags.FORCE_DEBRIS;
            }
        }

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32Nullable(ref EntityId)
                && msg.ReadByte(ref ExplosionType)
                && msg.ReadVector3Nullable(ref Position)
                && msg.ReadFloat(ref Radius)
                && msg.ReadFloat(ref Damage)
                && msg.ReadEnum(ref ExplosionFlags)
                && msg.ReadInt32Nullable(ref ParticleIDOverride);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32Nullable(EntityId);
            msg.WriteByte(ExplosionType);
            msg.WriteVector3Nullable(Position);
            msg.WriteFloat(Radius);
            msg.WriteFloat(Damage);
            msg.WriteEnum(ExplosionFlags);
            msg.WriteInt32Nullable(ParticleIDOverride);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.ADD_EXPLOSION; } }
    }
}
