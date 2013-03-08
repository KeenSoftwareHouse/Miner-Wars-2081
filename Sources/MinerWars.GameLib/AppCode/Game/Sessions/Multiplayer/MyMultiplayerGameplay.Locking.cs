using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.Weapons;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.CommonLIB.AppCode.Networking;
using Lidgren.Network;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWarsMath;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.World;
using KeenSoftwareHouse.Library.Parallelization.Threading;

namespace MinerWars.AppCode.Game.Sessions
{
    delegate void LockResponseHandler(MyEntity entity, bool success);

    partial class MyMultiplayerGameplay
    {
        private Dictionary<uint, byte> m_lockedEntities = new Dictionary<uint, byte>();
        public LockResponseHandler LockReponse;
        private FastResourceLock m_lockedEntitiesLock = new FastResourceLock();

        private Action<MyEntity> m_unlockOnClosing;

        public bool IsLockedByMe(MyEntity entity)
        {
            byte playerId;
            return entity.EntityId.HasValue && m_lockedEntities.TryGetValue(entity.EntityId.Value.NumericValue, out playerId) && playerId == MyEntityIdentifier.CurrentPlayerId;
        }
        
        public bool IsLockedByOtherPlayer(MyEntity entity)
        {
            byte playerId;
            return entity.EntityId.HasValue && m_lockedEntities.TryGetValue(entity.EntityId.Value.NumericValue, out playerId) && playerId != MyEntityIdentifier.CurrentPlayerId;
        }

        public bool IsLockedByAny(MyEntity entity)
        {
            byte playerId;
            return entity.EntityId.HasValue && m_lockedEntities.TryGetValue(entity.EntityId.Value.NumericValue, out playerId);
        }

        public void ClearLocks(byte playerId)
        {
            foreach (var l in m_lockedEntities.ToArray())
            {
                if (l.Value == playerId)
                {
                    m_lockedEntities.Remove(l.Key);

                    // Announce to all
                    MyEventLock unlockMsg = new MyEventLock();
                    unlockMsg.EntityId = l.Key;
                    unlockMsg.LockType = MyLockEnum.UNLOCK;
                    Peers.SendToAll(ref unlockMsg);
                }
            }
        }

        public void Lock(MyEntityIdentifier entityId, bool enable)
        {
            if (IsHost)
            {
                // Host just sends lock to all
                bool success = TryLockEntity(entityId.NumericValue, MyEntityIdentifier.CurrentPlayerId, enable);

                if (success)
                {
                    LogDevelop(MyEntities.GetEntityById(entityId).Name + " " + (enable ? "LOCKED" : "UNLOCKED"));
                }

                MyEntity entity;
                if (enable && MyEntities.TryGetEntityById(entityId, out entity))
                {
                    RaiseLockResponse(entity, success);
                }
            }
            else
            {
                // Send request to host
                MyEventLock msg = new MyEventLock();
                msg.EntityId = entityId.NumericValue;
                msg.LockType = enable ? MyLockEnum.LOCK : MyLockEnum.UNLOCK;

                // Sometimes can be called after host already disconnected (closing screens etc)
                Peers.TrySendHost(ref msg);
            }
        }

        public void Lock(MyEntity entity, bool enable)
        {
            Debug.Assert(entity.EntityId.HasValue);
            Lock(entity.EntityId.Value, enable);
        }

        private void OnLock(ref MyEventLock msg)
        {
            var player = (MyPlayerRemote)msg.SenderConnection.Tag;
            if (player != null)
            {
                if (IsHost)
                {
                    // Someone wants to lock entity
                    TryLockEntity(msg.EntityId, player.PlayerId, msg.LockType == MyLockEnum.LOCK);
                }
                else if (player.UserId == Peers.HostUserId)
                {
                    using (m_lockedEntitiesLock.AcquireExclusiveUsing())
                    {
                        if (msg.LockType == MyLockEnum.LOCK)
                        {
                            // Locked by host, just accept it
                            m_lockedEntities[msg.EntityId] = 0;
                        }
                        else
                        {
                            // Unlocked by host
                            m_lockedEntities.Remove(msg.EntityId);
                        }
                    }
                }
                else
                {
                    Alert("Lock came from other player than host", msg.SenderEndpoint, msg.EventType);
                }
            }
        }

        private void OnLockResult(ref MyEventLockResult msg)
        {
            if (msg.IsSuccess)
            {
                m_lockedEntities[msg.EntityId] = MyEntityIdentifier.CurrentPlayerId;
            }

            MyEntity entity;
            if (MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.EntityId), out entity))
            {
                RaiseLockResponse(entity, msg.IsSuccess);
            }
        }

        private void RaiseLockResponse(MyEntity entity, bool success)
        {
            LogDevelop("Raised lock response: " + entity.Name);

            var handler = LockReponse;
            if (handler != null)
            {
                handler(entity, success);
            }
        }

        /// <summary>
        /// Try lock entity and announce to other players (runs only on host)
        /// </summary>
        bool TryLockEntity(uint entityId, byte playerId, bool enable)
        {
            bool success = true;
            if (enable)
            {
                using (m_lockedEntitiesLock.AcquireExclusiveUsing())
                {
                    success = !m_lockedEntities.ContainsKey(entityId);

                    if (success)
                    {
                        m_lockedEntities[entityId] = playerId;
                        MyEntities.GetEntityById(entityId.ToEntityId()).OnClosing += m_unlockOnClosing;
                    }
                }
            }
            else
            {
                using (m_lockedEntitiesLock.AcquireExclusiveUsing())
                {
                    success = m_lockedEntities.Remove(entityId);
                    MyEntity entity;
                    if (MyEntities.TryGetEntityById(entityId.ToEntityId(), out entity))
                    {
                        entity.OnClosing -= m_unlockOnClosing;
                    }
                }
            }

            if (success)
            {
                // Send lock to all
                MyEventLock response = new MyEventLock();
                response.EntityId = entityId;
                response.LockType = enable ? MyLockEnum.LOCK : MyLockEnum.UNLOCK;
                Peers.SendToAll(ref response, NetDeliveryMethod.ReliableOrdered, 0);
            }

            if (enable)
            {
                // Send response to player
                MyPlayerRemote player;
                if (Peers.TryGetPlayer(playerId, out player))
                {
                    MyEventLockResult response = new MyEventLockResult();
                    response.EntityId = entityId;
                    response.IsSuccess = success;
                    Peers.NetworkClient.Send(ref response, player.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }

            return success;
        }

        void UnlockOnClosing(MyEntity entity)
        {
            Lock(entity, false);
        }
    }
}
