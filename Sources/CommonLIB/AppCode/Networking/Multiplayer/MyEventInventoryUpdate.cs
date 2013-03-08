using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Lidgren.Network;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public struct MyEventInventoryUpdate : IMyEvent
    {
        public uint EntityId;
        public MyMwcObjectBuilder_Inventory InventoryBuilder;

        public bool Read(MyMessageReader msg)
        {
            return msg.ReadUInt32(ref EntityId)
                && msg.ReadObjectBuilder(ref InventoryBuilder);
        }

        public void Write(MyMessageWriter msg)
        {
            msg.WriteUInt32(EntityId);
            msg.WriteObjectBuilder(InventoryBuilder);
        }

        public NetConnection SenderConnection { get; set; }
        public IPEndPoint SenderEndpoint { get; set; }
        public MyEventEnum EventType { get { return MyEventEnum.INVENTORY_UPDATE; } }
    }
}
