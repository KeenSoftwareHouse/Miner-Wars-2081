using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventProjectileHit : IMyEvent
    {
        public uint TargetEntityId;
        public Vector3 Position;
        public Vector3 Direction;
        public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType;
        public uint? SourceEntityId;

        public bool Read(MyMessageReader msg)
        {
            

            return msg.ReadUInt32(ref TargetEntityId)
                && msg.ReadVector3(ref Position)
                && msg.ReadVector3(ref Direction)
                && msg.ReadObjectBuilderSmallShipAmmoTypesEnum(ref AmmoType)
                && msg.ReadUInt32Nullable(ref SourceEntityId);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(TargetEntityId);
            msg.WriteVector3(Position);
            msg.WriteVector3(Direction);
            msg.WriteObjectBuilderSmallShipAmmoTypesEnum(AmmoType);
            msg.WriteUInt32Nullable(SourceEntityId);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.PROJECTILE_HIT; } }
    }
}
