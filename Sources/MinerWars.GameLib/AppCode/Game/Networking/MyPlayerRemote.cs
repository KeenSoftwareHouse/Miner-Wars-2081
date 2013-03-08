using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Net;
using Lidgren.Network;

namespace MinerWars.AppCode.Game.World
{
    class MyPlayerRemote : MyPlayerBase
    {
        public NetConnection Connection;
        public byte PlayerId;
        public int UserId;
        public MyMwcObjectBuilder_FactionEnum Faction;
        public bool IsRelayed;

        public MySmallShip Ship;

        public MyPlayerRemote(StringBuilder displayName, int userId, byte playerId)
            : base(displayName, userId)
        {
            PlayerId = playerId;
            UserId = userId;
            Statistics = new MyPlayerStatistics();
            Faction = MyMwcObjectBuilder_FactionEnum.None;
        }
    }
}
