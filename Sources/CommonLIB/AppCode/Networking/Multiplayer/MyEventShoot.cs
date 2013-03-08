using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWarsMath;
using System.Net;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventShoot : IMyEvent
    {
        public uint ShooterEntityId;
        public uint? ProjectileEntityId;
        public uint? TargetEntityId;
        public MyMwcPositionAndOrientation Position;
        public MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum Weapon;
        public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum Ammo;

        public bool Read(MyMessageReader msg)
        {
            // 46B-54B
            return msg.ReadUInt32(ref ShooterEntityId) // 4B
                && msg.ReadUInt32Nullable(ref ProjectileEntityId) // 1B/5B
                && msg.ReadUInt32Nullable(ref TargetEntityId) // 1B/5B
                && msg.ReadPositionAndOrientation(ref Position) // 36B
                && msg.ReadObjectBuilderSmallShipWeaponTypesEnum(ref Weapon) // 2B
                && msg.ReadObjectBuilderSmallShipAmmoTypesEnum(ref Ammo); // 2B
                
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(ShooterEntityId);
            msg.WriteUInt32Nullable(ProjectileEntityId);
            msg.WriteUInt32Nullable(TargetEntityId);
            msg.WritePositionAndOrientation(Position);
            msg.WriteObjectBuilderSmallShipWeaponTypesEnum(Weapon);
            msg.WriteObjectBuilderSmallShipAmmoTypesEnum(Ammo);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.SHOOT; } }
    }
}
