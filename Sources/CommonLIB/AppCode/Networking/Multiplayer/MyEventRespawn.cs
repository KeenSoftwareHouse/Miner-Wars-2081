using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventRespawn: IMyEvent
    {
        public uint EntityId;
        public MyMwcPositionAndOrientation Position;
        public MyMwcObjectBuilder_SmallShip_TypesEnum ShipType;
        public MyMwcObjectBuilder_Inventory Inventory;
        public MyMwcObjectBuilder_FactionEnum Faction;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadPositionAndOrientation(ref Position)
                && msg.ReadObjectBuilder(ref Inventory)
                && msg.ReadSmallShipType(ref ShipType)
                && msg.ReadEnum(ref Faction);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WritePositionAndOrientation(Position);
            msg.WriteObjectBuilder(Inventory);
            msg.WriteSmallShipType(ShipType);
            msg.WriteEnum(Faction);
        }

        public NetConnection SenderConnection { get; set; }
		public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.RESPAWN; } }
    }
}
