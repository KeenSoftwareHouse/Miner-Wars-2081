using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.Sessions;

namespace MinerWars.AppCode.Game.Entities
{
    internal static class MyEntityIdentifierExtension
    {
        public static uint? ToNullableUInt(this MyEntityIdentifier? entityId)
        {
            return MyEntityIdentifier.ToNullableInt(entityId);
        }

        public static MyEntityIdentifier ToEntityId(this uint entityId)
        {
            return new MyEntityIdentifier(entityId);
        }

        public static MyEntityIdentifier? ToEntityId(this uint? entityId)
        {
            return entityId.HasValue ? new MyEntityIdentifier(entityId.Value) : (MyEntityIdentifier?)null;
        }
    }

    internal struct MyEntityIdentifier
    {
        public const uint MAX_PER_USER_ENTITY_ID = 256 * 256 * 256;

        const int DEFAULT_DICTIONARY_SIZE = 30000;
        const uint MAX_PER_USER_ENTITY_ID_COULD_BE_ALLOCATED = MAX_PER_USER_ENTITY_ID - 7216;

        public uint NumericValue
        {
            get
            {
                return m_numericValue;
            }
        }

        public byte PlayerId
        {
            get
            {
                return (byte)(NumericValue / MAX_PER_USER_ENTITY_ID);
            }
        }

        public uint PerUserEntityId
        {
            get
            {
                return NumericValue % MAX_PER_USER_ENTITY_ID;
            }
        }

        public MyEntityIdentifier(uint entityId)
        {
            m_numericValue = entityId;
        }

        public MyEntityIdentifier(byte userId, uint perUserEntityId)
        {
            if (perUserEntityId > MAX_PER_USER_ENTITY_ID)
            {
                throw new ArgumentException("Per user entity id cannot be larger than 256^3 (16777216)", "perUserEntityId");
            }
            m_numericValue = userId * MAX_PER_USER_ENTITY_ID + perUserEntityId;
        }

        public static MyEntityIdentifier? FromNullableInt(uint? value)
        {
            if (value.HasValue)
            {
                return new MyEntityIdentifier(value.Value);
            }
            else
            {
                return null;
            }
        }

        public static uint? ToNullableInt(MyEntityIdentifier? entityId)
        {
            if (entityId.HasValue)
            {
                return entityId.Value.NumericValue;
            }
            else
            {
                return null;
            }
        }

        public static byte CurrentPlayerId
        {
            get
            {
                return m_currentUserId;
            }
            set
            {
                m_currentUserId = value;
                m_lastId = m_currentUserId * MAX_PER_USER_ENTITY_ID;
            }
        }

        uint m_numericValue;

        static Dictionary<uint, MyEntity> m_entityList = new Dictionary<uint, MyEntity>(DEFAULT_DICTIONARY_SIZE);
        static uint m_lastId = 1;
        static bool m_allocationSuspended = false;
        static byte m_currentUserId = 1;

        /// <summary>
        /// Freezes allocating entity ids.
        /// This is important, because during load, no entity cannot allocate new id, because it could allocate id which already has entity which will be loaded soon.
        /// </summary>
        public static bool AllocationSuspended
        {
            get
            {
                return m_allocationSuspended;
            }
            set
            {
                m_allocationSuspended = value;
                //MyTrace.Send(TraceWindow.EntityId, value ? "Allocation suspended" : "Allocation resumed" ); 
            }
        }

        /// <summary>
        /// Adds entity with existing ID
        /// </summary>
        /// <param name="entity"></param>
        public static void AddEntityWithId(MyEntity entity)
        {
            Debug.Assert(entity.EntityId.HasValue, "Entity must have id!, use MyEntityIdentifier.Allocate() to get free id");
            Debug.Assert(!m_entityList.ContainsKey(entity.EntityId.Value.NumericValue), "Entity with this key already exists in entity list! This can't happen.");

            m_entityList.Add(entity.EntityId.Value.NumericValue, entity);
            //MyTrace.Send(TraceWindow.EntityId, "Added entity: " + entity.EntityId.ToString());
            if (entity.EntityId.Value.PlayerId == CurrentPlayerId && m_lastId < entity.EntityId.Value.NumericValue && entity.EntityId.Value.PerUserEntityId <= MAX_PER_USER_ENTITY_ID_COULD_BE_ALLOCATED)
            {
                m_lastId = entity.EntityId.Value.NumericValue;
                //MyTrace.Send(TraceWindow.EntityId, "New last id: " + new MyEntityIdentifier(m_lastId).ToString());
            }
        }

        private static bool TestAllocation(byte? playerId)
        {
            if (playerId.HasValue && MyMultiplayerGameplay.IsRunning)
            {
                if (MyMultiplayerGameplay.Static.IsHost)
                {
                    return playerId.Value == 0 || playerId.Value == CurrentPlayerId;
                }
                else
                {
                    return playerId.Value == CurrentPlayerId;
                }
            }
            return true;
        }

        /// <summary>
        /// Allocated new entity ID (won't add to list)
        /// Entity with this ID should be added immediatelly
        /// </summary>
        public static MyEntityIdentifier AllocateId(byte? playerId = null)
        {
            Debug.Assert(!AllocationSuspended, "Cannot allocate new entity id, becase allocation is suspended - probably because of load.");
            Debug.Assert(TestAllocation(playerId), "PlayerId is invalid!");

            byte currentPlayerId = playerId ?? CurrentPlayerId;

            // Find next free id
            uint perUserId = m_lastId % MAX_PER_USER_ENTITY_ID;
            uint newId;
            do
            {
                perUserId++;
                if (perUserId > MAX_PER_USER_ENTITY_ID_COULD_BE_ALLOCATED)
                {
                    perUserId = 1;
                }
                newId = currentPlayerId * MAX_PER_USER_ENTITY_ID + perUserId;
            }
            while (m_entityList.ContainsKey(newId));
            m_lastId = newId;
            //do
            //{
            //    m_lastId++;                
            //    m_lastId = CurrentUserId * MaxPerUserEntityId + m_lastId % MaxPerUserEntityId; // Clamp between 0 and MaxPerUserEntityId
            //    //m_lastId = CurrentUserId * MaxPerUserEntityId + m_lastId % MAX_PER_USER_ENTITY_ID_COULD_BE_ALLOCATED; // Clamp between 0 and MAX_PER_USER_ENTITY_ID_COULD_BE_ALLOCATED
            //}
            //while (m_entityList.ContainsKey(m_lastId));

            var result = new MyEntityIdentifier(m_lastId);
            //MyTrace.Send(TraceWindow.EntityId, "Allocated id (and also last id): " + result.ToString());
            return result;
        }

        public static MyEntityIdentifier AllocatePlayershipId()
        {
            uint perUserId = MAX_PER_USER_ENTITY_ID_COULD_BE_ALLOCATED;
            MyEntityIdentifier entityId;
            do
            {
                perUserId++;
                entityId = new MyEntityIdentifier(CurrentPlayerId, perUserId);
            } while (m_entityList.ContainsKey(entityId.NumericValue));
            return entityId;
        }

        public static void RemoveEntity(MyEntityIdentifier entityIdentifier)
        {
            //MyTrace.Send(TraceWindow.EntityId, "Remove entity: " + entityIdentifier.ToString());
            m_entityList.Remove(entityIdentifier.NumericValue);
        }

        public static MyEntity GetEntityByIdOrNull(MyEntityIdentifier entityId)
        {
            MyEntity entity;
            m_entityList.TryGetValue(entityId.NumericValue, out entity);
            return entity;
        }

        public static bool TryGetEntity(MyEntityIdentifier entityId, out MyEntity entity)
        {
            return m_entityList.TryGetValue(entityId.NumericValue, out entity);
        }

        public static bool TryGetEntity<T>(MyEntityIdentifier entityId, out T entity) where T : MyEntity
        {
            MyEntity e;
            bool result = m_entityList.TryGetValue(entityId.NumericValue, out e);
            entity = e as T;
            return result && entity != null;
        }

        public static MyEntity GetEntityById(MyEntityIdentifier entityId)
        {
            return m_entityList[entityId.NumericValue];
        }

        public static bool ExistsById(MyEntityIdentifier entityId)
        {
            return m_entityList.ContainsKey(entityId.NumericValue);
        }

        public static void Clear()
        {
            m_entityList.Clear();
            m_lastId = 1;
        }

        public override string ToString()
        {
            return String.Format("Value: {0}, User: {1}, Per user id: {2}", m_numericValue, PlayerId, PerUserEntityId);
        }

        internal static void Reset()
        {
            m_entityList = new Dictionary<uint, MyEntity>(DEFAULT_DICTIONARY_SIZE);
            m_lastId = 1;
            m_allocationSuspended = false;
            m_currentUserId = 1;
        }
    }
}
