using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventDoDamage : IMyEvent
    {
        public uint TargetEntityId;
        public float PlayerDamage;
        public float Damage;
        public float EmpDamage;
        public byte DamageType;
        public byte AmmoType;
        public uint? DamageSource;
        public float NewHealthRatio;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref TargetEntityId)
                && msg.ReadFloat(ref PlayerDamage)
                && msg.ReadFloat(ref Damage)
                && msg.ReadFloat(ref EmpDamage)
                && msg.ReadByte(ref DamageType)
                && msg.ReadByte(ref AmmoType)
                && msg.ReadUInt32Nullable(ref DamageSource)
                && msg.ReadFloat(ref NewHealthRatio);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(TargetEntityId);
            msg.WriteFloat(PlayerDamage);
            msg.WriteFloat(Damage);
            msg.WriteFloat(EmpDamage);
            msg.WriteByte((byte)DamageType);
            msg.WriteByte((byte)AmmoType);
            msg.WriteUInt32Nullable(DamageSource);
            msg.WriteFloat(NewHealthRatio);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }

        public MyEventEnum EventType { get { return MyEventEnum.DO_DAMAGE; } }
    }
}
