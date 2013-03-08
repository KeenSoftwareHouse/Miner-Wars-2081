using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using Lidgren.Network;
using System.Net;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.World;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Sessions
{
    class MyNatIntroduction
    {
        class NatInfo
        {
            public IPEndPoint IntroductionEndpoint;
            public NetConnection Connection;
        }

        public MyNatIntroduction(MyLidgrenPeer peer)
        {
            m_peer = peer;
        }

        private MyLidgrenPeer m_peer;
        private Dictionary<int, MyPlayerRemote> m_playersToIntroduce;

        public void SetRequiredPlayers(List<MyPlayerRemote> requiredPlayers)
        {
            m_playersToIntroduce = requiredPlayers.ToDictionary(s => s.UserId, s => s);
        }

        public void OnIntroduce(IPEndPoint endpoint, string token)
        {
            Debug.Assert(m_playersToIntroduce != null);

            int userId;
            MyPlayerRemote player;
            if (int.TryParse(token, out userId) && m_playersToIntroduce.TryGetValue(userId, out player))
            {
                if (player.Connection == null)
                {
                    player.Connection = m_peer.Connect(endpoint);
                    player.Connection.Tag = player;
                }
            }
        }

        public void OnDirectIntroduce(int userId, MyRelayedConnection relayedConnection)
        {
            Debug.Assert(m_playersToIntroduce != null);

            MyPlayerRemote player;
            if(m_playersToIntroduce.TryGetValue(userId, out player))
            {
                if(player.Connection == null)
                {
                    player.Connection = relayedConnection;
                    player.Connection.Tag = player;
                }
            }
        }
        
        public bool IsAllConnected()
        {
            if (m_playersToIntroduce == null)
                return false;

            return m_playersToIntroduce.All(s => s.Value.Connection != null && (s.Value.Connection.Status == NetConnectionStatus.Connected || s.Value.Connection is MyRelayedConnection));
        }
    }
}
