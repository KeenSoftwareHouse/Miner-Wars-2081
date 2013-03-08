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
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Sessions
{
    partial class MyMultiplayerGameplay
    {
        public void Shoot(MyEntity entity, Matrix shooterMatrix, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weapon, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo, MyEntity target, MyEntityIdentifier? projectileId)
        {
            if (!IsControlledByMe(entity))
            {
                return;
            }

            MyEventShoot msg = new MyEventShoot();
            msg.Position = new MyMwcPositionAndOrientation(shooterMatrix);
            msg.ShooterEntityId = entity.EntityId.Value.NumericValue;
            msg.ProjectileEntityId = MyEntityIdentifier.ToNullableInt(projectileId);
            msg.Ammo = ammo;
            msg.TargetEntityId = (target != null && target.EntityId.HasValue) ? target.EntityId.Value.NumericValue : (uint?)null;
            msg.Weapon = weapon;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableUnordered);
        }

        void OnShoot(ref MyEventShoot msg)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Shoot");
            MyEntity parent;
            if (MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.ShooterEntityId), out parent))
            {
                if (parent is MySmallShip)
                {
                    var ship = (MySmallShip)parent;
                    ship.InitGroupMaskIfNeeded();
                    ship.WorldMatrix = msg.Position.GetMatrix();
                    if (msg.TargetEntityId.HasValue)
                    {
                        ship.TargetEntity = MyEntityIdentifier.GetEntityByIdOrNull(new MyEntityIdentifier(msg.TargetEntityId.Value));
                    }
                    var msgWeapon = msg.Weapon;
                    var weapon = GetWeapon(ship, msgWeapon);
                    if (weapon == null)
                    {
                        weapon = ship.Weapons.AddWeapon(new MyMwcObjectBuilder_SmallShip_Weapon(msg.Weapon));
                    }
                    Debug.Assert(weapon.Parent != null, "Weapon parent is null, something is wrong");
                    weapon.IsDummy = true;
                    weapon.Shot(new MyMwcObjectBuilder_SmallShip_Ammo(msg.Ammo));
                    MyEntity projectile;
                    if (msg.ProjectileEntityId.HasValue && weapon.LastShotId.HasValue && MyEntities.TryGetEntityById(weapon.LastShotId.Value, out projectile))
                    {
                        MyEntityIdentifier.RemoveEntity(weapon.LastShotId.Value);
                        projectile.EntityId = new MyEntityIdentifier(msg.ProjectileEntityId.Value);
                        if (!MyEntityIdentifier.ExistsById(projectile.EntityId.Value))
                        {
                            MyEntityIdentifier.AddEntityWithId(projectile);
                        }
                    }
                }
                else if (parent is MyPrefabLargeWeapon)
                {
                    var gun = ((MyPrefabLargeWeapon)parent).GetGun();
                    if (msg.TargetEntityId.HasValue)
                    {
                        var target = MyEntityIdentifier.GetEntityByIdOrNull(new MyEntityIdentifier(msg.TargetEntityId.Value));
                        gun.SetTarget(target);
                    }
                    
                    gun.GetBarell().IsDummy = true;
                    gun.RotateImmediately(gun.GetPosition() + msg.Position.GetMatrix().Forward * 5000);
                    gun.IsDummy = true;
                    gun.Shot(new MyMwcObjectBuilder_SmallShip_Ammo(msg.Ammo));
                    MyEntity projectile;
                    if (msg.ProjectileEntityId.HasValue && gun.LastShotId.HasValue && MyEntities.TryGetEntityById(gun.LastShotId.Value, out projectile))
                    {
                        MyEntityIdentifier.RemoveEntity(gun.LastShotId.Value);
                        projectile.EntityId = new MyEntityIdentifier(msg.ProjectileEntityId.Value);
                        if (!MyEntityIdentifier.ExistsById(projectile.EntityId.Value))
                        {
                            MyEntityIdentifier.AddEntityWithId(projectile);
                        }
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private static MySmallShipGunBase GetWeapon(MySmallShip ship, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum msgWeapon)
        {
            return MyMwcUtils.GetRandomItemOrNull(ship.Weapons.GetMountedWeaponsWithHarvesterAndDrill().Where((s) => s.WeaponType == msgWeapon).ToList());
        }

        public void ExplodeAmmo(MyAmmoBase ammo)
        {
            // Explode only my missiles to prevent circullar SEND/RECEIVE
            if (IsControlledByMe(ammo))
            {
                var msg = new MyEventAmmoExplosion();
                msg.AmmoBaseEntityId = ammo.EntityId.Value.NumericValue;
                msg.Position = new MyMwcPositionAndOrientation(ammo.WorldMatrix);
                Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableUnordered);
            }
        }

        void OnAmmoExplosion(ref MyEventAmmoExplosion msg)
        {
            MyEntity ammoBase;
            if (MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.AmmoBaseEntityId), out ammoBase) && ammoBase is MyAmmoBase)
            {
                var ammo = (MyAmmoBase)ammoBase;
                ammo.WorldMatrix = msg.Position.GetMatrix();
                ammo.IsDummy = false;
                ammo.Explode();
            }
        }

        public void DoDamage(MyEntity target, float player, float normal, float emp, MyDamageType damageType, MyAmmoType ammoType, MyEntity source, float newHealthRatio)
        {
            if (IsControlledByMe(target))
            {
                MyEventDoDamage msg = new MyEventDoDamage();
                msg.TargetEntityId = target.EntityId.Value.NumericValue;
                msg.PlayerDamage = player;
                msg.Damage = normal;
                msg.EmpDamage = emp;
                msg.DamageType = (byte)damageType;
                msg.AmmoType = (byte)ammoType;
                msg.DamageSource = source != null ? MyEntityIdentifier.ToNullableInt(source.EntityId) : null;
                msg.NewHealthRatio = newHealthRatio;
                Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableUnordered);
            }
        }

        void OnDoDamage(ref MyEventDoDamage msg)
        {
            MyEntity target;
            if (MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.TargetEntityId), out target) && CheckSenderId(msg, msg.TargetEntityId))
            {
                MyEntity source;
                if (!msg.DamageSource.HasValue || !MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.DamageSource.Value), out source))
                {
                    source = null;
                }

                Debug.Assert(target.IsDummy);
                target.IsDummy = false;
                // This is stupid, but necessary, DoDamage should be fixed
                if (msg.NewHealthRatio <= 0)
                {
                    target.HealthRatio = 0.0001f;
                    target.DoDamage(msg.PlayerDamage, msg.Damage, msg.EmpDamage, (MyDamageType)msg.DamageType, (MyAmmoType)msg.AmmoType, source);

                    //Closes entity 
                    target.UpdateBeforeSimulation();
                }
                else
                {
                    target.HealthRatio = 100.0f;
                    target.DoDamage(msg.PlayerDamage, msg.Damage, msg.EmpDamage, (MyDamageType)msg.DamageType, (MyAmmoType)msg.AmmoType, source);
                    target.HealthRatio = msg.NewHealthRatio;
                }
                target.IsDummy = true;
            }
            else
            {
                Alert("Call 'OnDamage' on invalid entity", msg.SenderEndpoint, msg.EventType);
            }
        }

        public void ProjectileHit(MyEntity target, Vector3 position, Vector3 direction, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType, MyEntity source)
        {
            if (target.EntityId.HasValue)
            {
                MyEventProjectileHit msg = new MyEventProjectileHit();
                msg.TargetEntityId = target.EntityId.Value.NumericValue;
                msg.Position = position;
                msg.Direction = direction;
                msg.AmmoType = ammoType;

                msg.SourceEntityId = source != null ? MyEntityIdentifier.ToNullableInt(source.EntityId) : null;
                Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableUnordered);
            }

        }

        void OnProjectileHit(ref MyEventProjectileHit msg)
        {
            MyEntity target;
            if (MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.TargetEntityId), out target))
            {
                MyEntity source;
                if (!msg.SourceEntityId.HasValue || !MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.SourceEntityId.Value), out source))
                {
                    source = null;
                }
                var ammo = MyAmmoConstants.GetAmmoProperties(msg.AmmoType);

                target.DoDamage(ammo.HealthDamage, ammo.ShipDamage, ammo.EMPDamage, ammo.DamageType, ammo.AmmoType, source);
                MyProjectile.ApplyProjectileForce(target, msg.Position, Vector3.Normalize(msg.Direction), !(target is MySmallShipBot));
            }
        }

        public void UpdateAmmo(MyAmmoBase ammo)
        {
            Debug.Assert(ammo.EntityId.HasValue, "All guided projectiles must have entity ID, this type hasn't: " + ammo.GetType().Name);
            if (ammo.EntityId.Value.PlayerId == MyEntityIdentifier.CurrentPlayerId) // Make sure to update only my ammo
            {
                MyEventAmmoUpdate msg = new MyEventAmmoUpdate();
                msg.EntityId = ammo.EntityId.Value.NumericValue;
                msg.Velocity = ammo.Physics.LinearVelocity;
                msg.Position = new MyMwcPositionAndOrientation(ammo.WorldMatrix);
                Peers.SendToAll(ref msg, ammo.EntityId.Value, m_multiplayerConfig.ProjectilesTickRate, NetDeliveryMethod.Unreliable);
            }
        }

        public void OnAmmoUpdate(ref MyEventAmmoUpdate msg)
        {
            var player = (MyPlayerRemote)msg.SenderConnection.Tag;
            if (player != null)
            {
                MyEntity projectile;
                if (MyEntityIdentifier.TryGetEntity(new MyEntityIdentifier(msg.EntityId), out projectile))
                {
                    projectile.WorldMatrix = msg.Position.GetMatrix();
                    projectile.Physics.LinearVelocity = msg.Velocity;
                }
            }
        }

        public void SpeacialWeaponEvent(MySpecialWeaponEventEnum eventType, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weapon)
        {
            MyEventSpeacialWeapon msg = new MyEventSpeacialWeapon();
            msg.ShipEntityId = MySession.PlayerShip.EntityId.Value.NumericValue;
            msg.Weapon = weapon;
            msg.WeaponEvent = eventType;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableUnordered);
        }

        public void OnSpecialWeaponEvent(ref MyEventSpeacialWeapon msg)
        {
            var player = (MyPlayerRemote)msg.SenderConnection.Tag;
            if (player != null)
            {
                MyEntity entity;
                if (MyEntityIdentifier.TryGetEntity(new MyEntityIdentifier(msg.ShipEntityId), out entity) && entity is MySmallShip)
                {
                    MySmallShip ship = (MySmallShip)entity;

                    switch (msg.WeaponEvent)
                    {
                        case MySpecialWeaponEventEnum.HARVESTER_FIRE:
                            OnFireHarvester(msg.Weapon, ship);
                            break;

                        case MySpecialWeaponEventEnum.DRILL_ACTIVATED:
                            EnsureDrill(msg.Weapon, ship).CurrentState = MyDrillStateEnum.Activated;
                            break;

                        case MySpecialWeaponEventEnum.DRILL_DEACTIVATED:
                            EnsureDrill(msg.Weapon, ship).CurrentState = MyDrillStateEnum.Deactivated;
                            break;

                        case MySpecialWeaponEventEnum.DRILL_DRILLING:
                            {
                                var drill = EnsureDrill(msg.Weapon, ship);
                                drill.Shot(null);
                                drill.CurrentState = MyDrillStateEnum.Drilling;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private static MyDrillBase EnsureDrill(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weapon, MySmallShip ship)
        {
            var drill = ship.Weapons.GetMountedDrill();
            if (drill == null || drill.WeaponType != weapon)
            {
                ship.Weapons.RemoveDrill();
                ship.Weapons.AddDrill(new MyMwcObjectBuilder_SmallShip_Weapon(weapon));
                drill = ship.Weapons.GetMountedDrill();
            }
            drill.IsDummy = true;
            return drill;
        }

        private static void OnFireHarvester(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weapon, MySmallShip ship)
        {
            var harvester = ship.Weapons.GetMountedHarvestingDevice();
            if (harvester == null || harvester.WeaponType != weapon)
            {
                ship.Weapons.RemoveHarvestingDevice();
                ship.Weapons.AddHarvestingDevice(new MyMwcObjectBuilder_SmallShip_Weapon(weapon));
                harvester = ship.Weapons.GetMountedHarvestingDevice();
            }
            harvester.IsDummy = true;
            ship.Weapons.FireHarvester();
        }

        public void ResetEntity(MyEntity entity)
        {
            Debug.Assert(entity.EntityId.HasValue);

            if(IsControlledByMe(entity))
            {
                MyEventEntityReset msg = new MyEventEntityReset();
                msg.EntityId = entity.EntityId.Value.NumericValue;
                Peers.SendToAll(ref msg);
            }
        }

        private void OnReset(ref MyEventEntityReset msg)
        {
            MyEntity entity;
            if(MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.EntityId), out entity) && entity.IsDummy)
            {
                var resetableEntity = entity as IResetable;
                entity.IsDummy = false;
                resetableEntity.Reset();
                entity.IsDummy = true;
            }
        }
    }
}
