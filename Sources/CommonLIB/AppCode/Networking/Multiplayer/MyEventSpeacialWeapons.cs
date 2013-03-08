using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using Lidgren.Network;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MySpecialWeaponEventEnum: byte
    {
        HARVESTER_FIRE,
        DRILL_ACTIVATED,
        DRILL_DEACTIVATED,
        DRILL_DRILLING,
    }

    public struct MyEventSpeacialWeapon : IMyEvent
    {
        public uint ShipEntityId; // 4B
        public MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum Weapon; // 2B
        public MySpecialWeaponEventEnum WeaponEvent; // 1B

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref ShipEntityId) 
                && msg.ReadObjectBuilderSmallShipWeaponTypesEnum(ref Weapon)
                && msg.ReadSpecialWeaponEventEnum(ref WeaponEvent);

        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(ShipEntityId);
            msg.WriteObjectBuilderSmallShipWeaponTypesEnum(Weapon);
            msg.WriteSpecialWeaponEventEnum(WeaponEvent);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.WEAPON_EVENT; } }
    }
}
