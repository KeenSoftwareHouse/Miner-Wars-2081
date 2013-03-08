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
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Explosions;

namespace MinerWars.AppCode.Game.Sessions
{
    partial class MyMultiplayerGameplay
    {
        public void AddExplosion(Vector3 position, MyExplosionTypeEnum explosionType, float damage, float radius, bool forceDebris, bool createDecals)
        {
            var msg = new MyEventAddExplosion();
            msg.CreateDecals = createDecals;
            msg.ForceDebris = forceDebris;
            msg.Damage = damage;
            msg.Position = position;
            msg.EntityId = null;
            msg.ExplosionType = (byte)explosionType;
            msg.Radius = radius;
            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void AddExplosion(MyEntity entity, MyExplosionTypeEnum explosionType, float damage, float radius, bool forceDebris, bool createDecals, MyParticleEffectsIDEnum? particleIDOverride = null)
        {
            Debug.Assert(entity != null && entity.EntityId.HasValue);

            var msg = new MyEventAddExplosion();
            msg.Damage = damage;
            msg.EntityId = entity.EntityId.Value.NumericValue;
            msg.ExplosionType = (byte)explosionType;
            msg.Radius = radius;
            msg.ParticleIDOverride = (int?)particleIDOverride;
            msg.CreateDecals = createDecals;
            msg.ForceDebris = forceDebris;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        void OnAddExplosion(ref MyEventAddExplosion msg)
        {
            if (!MyMwcEnums.IsValidValue(msg.ExplosionFlags))
            {
                Alert("Invalid explosion", msg.SenderEndpoint, msg.EventType);
                return;
            }

            MyEntity entity;
            if (msg.EntityId.HasValue && MyEntities.TryGetEntityById(msg.EntityId.Value.ToEntityId(), out entity))
            {
                MyScriptWrapper.AddExplosion(entity, (MyExplosionTypeEnum)msg.ExplosionType, msg.Damage, msg.Radius, msg.ForceDebris, msg.CreateDecals, (MyParticleEffectsIDEnum?)msg.ParticleIDOverride);
            }
            else if (msg.Position.HasValue)
            {
                MyScriptWrapper.AddExplosion(msg.Position.Value, (MyExplosionTypeEnum)msg.ExplosionType, msg.Radius, msg.Damage, msg.ForceDebris, msg.CreateDecals);
            }
            else
            {
                Alert("Invalid explosion", msg.SenderEndpoint, msg.EventType);
            }
        }

        public void UpdateDummyFlags(MyDummyPoint dummyPoint)
        {
            Debug.Assert(dummyPoint.EntityId.HasValue);

            var msg = new MyEventDummyFlags();
            msg.Flags = dummyPoint.DummyFlags;
            msg.EntityId = dummyPoint.EntityId.Value.NumericValue;
        }

        void OnUpdateDummyFlags(ref MyEventDummyFlags msg)
        {
            MyDummyPoint dummy;
            if (MyEntities.TryGetEntityById<MyDummyPoint>(msg.EntityId.ToEntityId(), out dummy))
            {
                dummy.DummyFlags = msg.Flags;
            }
            else
            {
                Alert("Update dummy flags, but no dummy found", msg.SenderEndpoint, msg.EventType);
            }
        }

        public void AddVoxelHand(uint voxelMapId, uint entityId, float radius, MyMwcVoxelHandModeTypeEnum handMode, MyMwcVoxelMaterialsEnum? material)
        {
            var msg = new MyEventAddVoxelHand();
            msg.EntityId = entityId;
            msg.Radius = radius;
            msg.HandMode = handMode;
            msg.VoxelMaterial = material;
            msg.VoxelMapEntityId = voxelMapId;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        void OnAddVoxelHand(ref MyEventAddVoxelHand msg)
        {
            MyScriptWrapper.AddVoxelHand(msg.VoxelMapEntityId, msg.EntityId, msg.Radius, msg.VoxelMaterial, msg.HandMode);
        }
    }
}
