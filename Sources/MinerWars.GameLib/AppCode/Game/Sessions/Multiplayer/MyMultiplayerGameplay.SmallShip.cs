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
using MinerWars.AppCode.Physics;
using SysUtils;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.Entities.Weapons.Ammo;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Cockpit;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Sessions
{
    partial class MyMultiplayerGameplay
    {
        static Dictionary<int, List<MyMwcObjectBuilder_SmallShip_TypesEnum>> m_factionShips;

        int m_respawnTime;
        DieHandler m_onEntityDie;

        Action<MySmallShip> m_onConfigChanged;
        Action<MyShip> m_onInventoryChanged;
        DateTime m_lastPositionUpdate;
        bool m_positionDirty;

        MyMwcObjectBuilder_ShipConfig m_lastConfig;
        List<MyMwcObjectBuilder_AssignmentOfAmmo> m_lastAmmoAssignment;

        MyCameraAttachedToEnum m_lastCamera = MyCameraAttachedToEnum.PlayerMinerShip;

        static void InitFactionShipTypes()
        {
            m_factionShips = new Dictionary<int, List<MyMwcObjectBuilder_SmallShip_TypesEnum>>();
            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.Euroamerican] = new List<MyMwcObjectBuilder_SmallShip_TypesEnum>()
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR,
                MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG,
                MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA,
                MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER,
            };

            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.Russian] = new List<MyMwcObjectBuilder_SmallShip_TypesEnum>()
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum.ORG,
                MyMwcObjectBuilder_SmallShip_TypesEnum.YG,
                MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN,
                MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD,
                MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV,
            };
            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.Russian_KGB] = m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.Russian];
            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.China] = m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.Russian];

            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.FourthReich] = new List<MyMwcObjectBuilder_SmallShip_TypesEnum>()
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.BAER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER,
            };

            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.Omnicorp] = new List<MyMwcObjectBuilder_SmallShip_TypesEnum>()
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK,
                MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX,
                MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER,
            };

            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.Templars] = new List<MyMwcObjectBuilder_SmallShip_TypesEnum>()
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW,
            };

            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.Freelancers] = new List<MyMwcObjectBuilder_SmallShip_TypesEnum>()
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum.DOON,
                MyMwcObjectBuilder_SmallShip_TypesEnum.HAMMER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.HAWK,
                MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE,
                MyMwcObjectBuilder_SmallShip_TypesEnum.LEVIATHAN,
                MyMwcObjectBuilder_SmallShip_TypesEnum.LIBERATOR,                
                MyMwcObjectBuilder_SmallShip_TypesEnum.ENFORCER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.KAMMLER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG,
                MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA,
                MyMwcObjectBuilder_SmallShip_TypesEnum.BAER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.HEWER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW,
                MyMwcObjectBuilder_SmallShip_TypesEnum.GREISER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.TRACER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.ORG,
                MyMwcObjectBuilder_SmallShip_TypesEnum.PHOENIX,
                MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.STANISLAV,
                MyMwcObjectBuilder_SmallShip_TypesEnum.STEELHEAD,
                MyMwcObjectBuilder_SmallShip_TypesEnum.FEDER,
                MyMwcObjectBuilder_SmallShip_TypesEnum.YG,
            };

            // Default ships for all other factions
            m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.None] = new List<MyMwcObjectBuilder_SmallShip_TypesEnum>()
            {
                MyMwcObjectBuilder_SmallShip_TypesEnum.JACKNIFE,
                MyMwcObjectBuilder_SmallShip_TypesEnum.DOON,
                MyMwcObjectBuilder_SmallShip_TypesEnum.ROCKHEATER,
            };
        }

        static MyMwcObjectBuilder_SmallShip_TypesEnum GetFactionShip(MyMwcObjectBuilder_FactionEnum faction)
        {
            Debug.Assert(m_factionShips.ContainsKey((int)MyMwcObjectBuilder_FactionEnum.None));
            List<MyMwcObjectBuilder_SmallShip_TypesEnum> shipList;
            if (!m_factionShips.TryGetValue((int)faction, out shipList))
            {
                shipList = m_factionShips[(int)MyMwcObjectBuilder_FactionEnum.None];
            }
            Debug.Assert(shipList.Count > 0);
            return MyMwcUtils.GetRandomItem(shipList);
        }

        void TestRespawn()
        {
            // Can't respawn until game is ready
            if (!MyMinerGame.IsGameReady)
            {
                return;
            }

            if (!IsStory() && MySession.PlayerShip.Faction == MyMwcObjectBuilder_FactionEnum.None)
            {
                // Faction not assigned, wait for set faction event
                return;
            }

            if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_respawnTime)
            {
                Respawn();
                m_respawnTime = Int32.MaxValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Return false when all players are dead</returns>
        public bool GetSafeRespawnPositionNearPlayer(MyMwcObjectBuilder_SmallShip_TypesEnum shipType, out MyMwcPositionAndOrientation positionAndOrientation)
        {
            foreach (var player in Peers.Players)
            {
                if (player.Ship != null && !player.Ship.IsDead())
                {
                    Vector3 position;
                    if (GetSafeRespawnPositionNearEntity(shipType, player.Ship, out position))
                    {
                        positionAndOrientation = new MyMwcPositionAndOrientation(position, player.Ship.GetForward(), player.Ship.GetUp());
                        return true;
                    }
                    else
                    {
                        // Inside ship
                        positionAndOrientation = new MyMwcPositionAndOrientation(player.Ship.GetPosition(), player.Ship.GetForward(), player.Ship.GetUp());
                        return true;
                    }
                }
            }

            positionAndOrientation = default(MyMwcPositionAndOrientation);
            return false;
        }

        private bool GetSafeRespawnPositionNearEntity(MyMwcObjectBuilder_SmallShip_TypesEnum shipType, MyEntity entity, out Vector3 position)
        {
            float dist = entity.WorldVolume.Radius * 2;

            for (int c = 15; c-- != 0; )
            {
                Vector3 randomPointInSphere = MyMwcUtils.GetRandomVector3Normalized() * MyMwcUtils.GetRandomFloat(0, 1) * dist; // Random point in sphere
                Vector3 newTestPos = entity.GetPosition() + randomPointInSphere;

                var shipRadius = MinerWars.AppCode.Game.Models.MyModels.GetModelOnlyData(MyShipTypeConstants.GetShipTypeProperties(shipType).Visual.ModelLod0Enum).BoundingSphere.Radius;

                BoundingSphere bsphere = new BoundingSphere(newTestPos, shipRadius);
                MyEntity col = MyEntities.GetIntersectionWithSphere(ref bsphere);

                MyLine line = new MyLine(entity.GetPosition(), newTestPos);

                if (col == null && MyEntities.GetAnyIntersectionWithLine(ref line, entity, null, false, true, false, false) == null)
                {
                    position = newTestPos;
                    return true;
                }
            }
            position = default(Vector3);
            return false;
        }

        public void Respawn(MyMwcObjectBuilder_SmallShip shipBuilder, Matrix respawnPosition)
        {
            Log("Respawn faction: " + MyFactionConstants.GetFactionProperties(MySession.PlayerShip.Faction).Name);

            int oldPriority = 0;

            if (m_lastConfig == null && MySession.PlayerShip != null)
            {
                m_lastConfig = MySession.PlayerShip.Config.GetObjectBuilder();
            }

            if (MySession.PlayerShip != null)
            {
                oldPriority = MySession.PlayerShip.AIPriority;
                if (MySession.PlayerShip.IsDummy || MySession.PlayerShip.IsExploded())
                {
                    MySession.PlayerShip.Activate(false, false);
                    MySession.PlayerShip.MarkForClose();
                    MySession.PlayerShip = null;
                }
            }

            MySession.Static.Player.RestoreHealth();

            // To proper respawn
            shipBuilder.ShipHealthRatio = MathHelper.Clamp(shipBuilder.ShipHealthRatio, 0.2f, 1.0f);
            shipBuilder.Oxygen = MathHelper.Clamp(shipBuilder.Oxygen, 100.0f, float.MaxValue);

            //Debug.Assert(!shipBuilder.EntityId.HasValue);
            var playership = (MySmallShip)MyEntities.CreateFromObjectBuilderAndAdd(MyClientServer.LoggedPlayer.GetDisplayName().ToString(), shipBuilder, respawnPosition);
            playership.AIPriority = oldPriority; // Restore bot priority

            if (IsSandBox())
            {
                if (m_lastConfig != null)
                {
                    playership.Config.Init(m_lastConfig);
                }
            }
            else
            {
                if (MyGuiScreenGamePlay.Static.Checkpoint.PlayerObjectBuilder.ShipConfigObjectBuilder != null)
                {
                    playership.Config.Init(MyGuiScreenGamePlay.Static.Checkpoint.PlayerObjectBuilder.ShipConfigObjectBuilder);
                }
            }

            playership.ConfigChanged += m_onConfigChanged;
            playership.OnDie += m_onEntityDie;
            playership.Activate(true);
            MyCockpitGlassDecals.Clear();

            var respawnMsg = new MyEventRespawn();
            respawnMsg.EntityId = playership.EntityId.Value.NumericValue;
            respawnMsg.Position = new MyMwcPositionAndOrientation(playership.WorldMatrix);
            respawnMsg.Inventory = GetInventory(playership, true);
            respawnMsg.ShipType = playership.ShipType;
            respawnMsg.Faction = playership.Faction;

            float ratio = (IsStory() && StoredShip != null) ? 0.2f : 1.0f;
            playership.ArmorHealth = MathHelper.Clamp(playership.ArmorHealth, playership.MaxArmorHealth * ratio, playership.MaxArmorHealth);
            playership.Fuel = MathHelper.Clamp(playership.Fuel, playership.MaxFuel * ratio, playership.MaxFuel);
            playership.HealthRatio = MathHelper.Clamp(playership.HealthRatio, ratio, 1.0f);
            playership.Oxygen = MathHelper.Clamp(playership.Oxygen, playership.MaxOxygen * ratio, playership.MaxOxygen);

            MySession.Static.Player.RestoreHealth();
            playership.PilotHealth = MySession.Static.Player.MaxHealth;

            if (MyMultiplayerGameplay.IsSandBox())
            {
                MakeInventoryItemsNontradeable(playership);
            }

            if (m_lastAmmoAssignment != null)
            {
                playership.Weapons.AmmoAssignments.Init(m_lastAmmoAssignment);
            }

            // As last to prevent many changes when doing something
            playership.InventoryChanged += m_onInventoryChanged;

            LogDevelop("RESPAWN end");
            Peers.SendToAll(ref respawnMsg, NetDeliveryMethod.ReliableOrdered);
        }

        void Respawn()
        {
            Log("Respawn");

            MyGuiScreenGamePlay.Static.CameraAttachedTo = MySession.Is25DSector ? MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic : m_lastCamera;

            Matrix respawnPosition = Matrix.Identity;

            MyMwcObjectBuilder_SmallShip shipBuilder;

            if (IsStory())
            {
                shipBuilder = StoredShip ?? MyGuiScreenGamePlay.Static.Checkpoint.PlayerObjectBuilder.ShipObjectBuilder;

                MyMwcPositionAndOrientation positionAndOrientation;
                if (GetSafeRespawnPositionNearPlayer(shipBuilder.ShipType, out positionAndOrientation))
                {
                    respawnPosition = positionAndOrientation.GetMatrix();
                }
                else // When return false, it means, no player alive
                {
                    MySession.Static.GameOver();
                    return;
                }
            }
            else
            {
                Debug.Assert(MySession.Static.Player.Faction != MyMwcObjectBuilder_FactionEnum.None, "Invalid faction! Faction can't be NONE");
                shipBuilder = CreateDeathmatchShip(MySession.Static.Player.Faction);

                List<MyDummyPoint> respawnPointList;

                if (m_respawnPoints.TryGetValue((int)MySession.Static.Player.Faction, out respawnPointList) && respawnPointList.Count > 0)
                {
                    int index = MyMwcUtils.GetRandomInt(respawnPointList.Count);
                    respawnPosition = respawnPointList[index].WorldMatrix;
                }
            }

            Respawn(shipBuilder, respawnPosition);
        }

        private static void MakeInventoryItemsNontradeable(MySmallShip playership)
        {
            MyMwcObjectBuilder_SmallShip ship = (MyMwcObjectBuilder_SmallShip)playership.GetObjectBuilder(true);
            if (ship.Engine != null)
            {
                ship.Engine.PersistentFlags |= MyPersistentEntityFlags.NotTradeable;
            }
            if (ship.Armor != null)
            {
                ship.Armor.PersistentFlags |= MyPersistentEntityFlags.NotTradeable;
            }
            foreach (var weapon in playership.Weapons.GetMountedWeaponsWithHarvesterAndDrill())
            {
                weapon.PersistentFlags |= MyPersistentEntityFlags.NotTradeable;
                weapon.GetObjectBuilder(true).PersistentFlags |= MyPersistentEntityFlags.NotTradeable;
            }
            foreach (var item in playership.Inventory.GetInventoryItems())
            {
                item.GetInventoryItemObjectBuilder(false).PersistentFlags |= MyPersistentEntityFlags.NotTradeable;
            }
        }

        public static void MakeInventoryItemsTradeable(MySmallShip playership)
        {
            MyMwcObjectBuilder_SmallShip ship = (MyMwcObjectBuilder_SmallShip)playership.GetObjectBuilder(true);
            if (ship.Engine != null)
            {
                ship.Engine.PersistentFlags &= ~MyPersistentEntityFlags.NotTradeable;
            }
            if (ship.Armor != null)
            {
                ship.Armor.PersistentFlags &= ~MyPersistentEntityFlags.NotTradeable;
            }
            foreach (var weapon in playership.Weapons.GetMountedWeaponsWithHarvesterAndDrill())
            {
                weapon.PersistentFlags &= ~MyPersistentEntityFlags.NotTradeable;
                weapon.GetObjectBuilder(true).PersistentFlags &= ~MyPersistentEntityFlags.NotTradeable;
            }
            foreach (var item in playership.Inventory.GetInventoryItems())
            {
                item.GetInventoryItemObjectBuilder(false).PersistentFlags &= ~MyPersistentEntityFlags.NotTradeable;
            }
        }


        void Playership_InventoryChanged(MyShip ship)
        {
            UpdateInventory(ship, true);
            m_savePlayer = true;
        }

        private void OnRespawn(ref MyEventRespawn msg)
        {
            var player = (MyPlayerRemote)msg.SenderConnection.Tag;
            if (player != null)
            {
                player.Faction = msg.Faction;

                MySmallShip playerShip;
                MyEntityIdentifier.TryGetEntity<MySmallShip>(new MyEntityIdentifier(msg.EntityId), out playerShip);

                // Close old player ship
                if (player.Ship != null)
                {
                    player.Ship.MarkForClose();
                    player.Ship = null;
                }

                // If ship exists
                if (playerShip != null)
                {
                    Log("OnRespawn: " + player.GetDisplayName().ToString() + ", ship exists, faction: " + MyFactionConstants.GetFactionProperties(player.Faction).Name);

                    player.Ship = playerShip;
                    OnNewPlayerShip(player.Ship);
                }
                else
                {
                    Log("OnRespawn: " + player.GetDisplayName().ToString() + ", new ship, faction: " + MyFactionConstants.GetFactionProperties(player.Faction).Name);

                    var ship = CreateShip(ChooseShip(player.Faction), player.Faction);
                    ship.EntityId = msg.EntityId;
                    ship.DisplayName = player.GetDisplayName().ToString();
                    ship.Inventory = msg.Inventory;
                    ship.ShipType = msg.ShipType;
                    player.Ship = (MySmallShip)MyEntities.CreateFromObjectBuilderAndAdd(player.GetDisplayName().ToString(), ship, msg.Position.GetMatrix());
                    OnNewPlayerShip(player.Ship);
                }

                // Don't save, when sending checkpoint to other players, ingame ships are added manually
                player.Ship.Save = false;

                // When I'm host, set AI priority for respawned player same as I have
                if (IsStory() && IsHost)
                {
                    player.Ship.AIPriority = MySession.PlayerShip.AIPriority;
                }

                UpdateCoopTarget();
            }
            else
            {
                Log("ON RESPAWN, UNKNOWN PLAYER: " + msg.SenderEndpoint.ToString());
            }
        }

        private void OnNewPlayerShip(MySmallShip ship)
        {
            ship.Physics.GroupMask = MyGroupMask.Empty;
            if (ship.Health > 0)
            {
                ship.IsDestructible = false;
            }
            ship.OnClose += m_playerShipClose;

            if (IsStory())
            {
                MySession.PlayerFriends.Add(ship);
            }
        }

        public void PilotDie(MySmallShip deadPilotEntity, MyEntity killer)
        {
            Debug.Assert(deadPilotEntity.EntityId.HasValue);

            Log("Pilot died: " + deadPilotEntity.DisplayName);

            MyEventPilotDie msg = new MyEventPilotDie();
            msg.EntityId = deadPilotEntity.EntityId.Value.NumericValue;
            msg.KillerId = killer != null && killer.EntityId != null ? killer.EntityId.Value.PlayerId : (byte?)null;
            m_respawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds + (int)m_multiplayerConfig.RespawnTime.TotalMilliseconds;
            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);

            if (deadPilotEntity == MySession.PlayerShip)
            {
                MyHudWarnings.Remove(deadPilotEntity);
                OnMeDied(msg.KillerId);
            }

            if (!deadPilotEntity.IsPilotDead())
            {
                deadPilotEntity.DisplayName = MyClientServer.LoggedPlayer.GetDisplayName() + " (dead)";
            }
            
            deadPilotEntity.PilotHealth = 0;

            // Send inventory on die to allow looting
            if (deadPilotEntity is IMyInventory)
            {
                Inventory_OnInventoryContentChange(((IMyInventory)deadPilotEntity).Inventory, deadPilotEntity);
            }

            deadPilotEntity.InventoryChanged += new Action<MyShip>(s => Inventory_OnInventoryContentChange(s.Inventory, s));
        }

        public void OnPilotDie(ref MyEventPilotDie msg)
        {
            MyPlayerRemote sender = (MyPlayerRemote)msg.SenderConnection.Tag;

            MyEntity deadPilotEntity;
            if (MyEntities.TryGetEntityById(msg.EntityId.ToEntityId(), out deadPilotEntity) && deadPilotEntity is MySmallShip)
            {
                Log("On pilot died: " + deadPilotEntity.DisplayName);

                var ship = (MySmallShip)deadPilotEntity;

                if (!ship.IsPilotDead())
                {
                    ship.DisplayName += " (dead)";
                }

                ship.PilotHealth = 0;

                ship.InventoryChanged += new Action<MyShip>(s => Inventory_OnInventoryContentChange(s.Inventory, s));

                if (sender.Ship == deadPilotEntity)
                {
                    OnPeerDied(sender, msg.KillerId);
                }
            }
            else
            {
                Alert("Dead pilot entity not found", msg.SenderEndpoint, msg.EventType);
            }
        }

        public void Die(MyEntity deadEntity, MyEntity killer)
        {
            Debug.Assert(deadEntity != null && deadEntity.EntityId.HasValue);

            Log("Died: " + deadEntity.DisplayName);

            var msg = new MyEventDie();
            msg.EntityId = deadEntity.EntityId.Value.NumericValue;
            msg.Position = new MyMwcPositionAndOrientation(deadEntity.WorldMatrix);
            msg.KillerId = killer != null && killer.EntityId != null ? killer.EntityId.Value.PlayerId : (byte?)null;
            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);

            deadEntity.OnDie -= m_onEntityDie;

            if (deadEntity == MySession.PlayerShip)
            {
                OnMeDied(msg.KillerId);
            }
        }

        void OnDie(ref MyEventDie msg)
        {
            MyPlayerRemote senderPlayer = (MyPlayerRemote)msg.SenderConnection.Tag;

            MyEntity deadEntity;
            if (MyEntities.TryGetEntityById(msg.EntityId.ToEntityId(), out deadEntity))
            {
                Log("On Die: " + deadEntity.DisplayName);
                deadEntity.IsDestructible = true;
                deadEntity.Kill(null);

                if (deadEntity == senderPlayer.Ship)
                {
                    OnPeerDied(senderPlayer, msg.KillerId);
                }
            }
            else
            {
                Log("ON DIE, UNKNOWN ENTITY, " + msg.SenderEndpoint.ToString() + ", ENTITY ID: " + msg.EntityId);
            }
        }

        void OnMeDied(byte? killerId)
        {
            if (IsStory())
            {
                // Player has same ship and inventory, with minimum of 20% armor, health, oxygen...
                StoredShip = MySession.PlayerShip.GetObjectBuilder(false) as MyMwcObjectBuilder_SmallShip;
                StoredShip.ClearEntityId();
            }

            m_lastConfig = MySession.PlayerShip.Config.GetObjectBuilder();
            m_lastAmmoAssignment = MySession.PlayerShip.Weapons.AmmoAssignments.GetObjectBuilder();
            m_lastCamera = MyGuiScreenGamePlay.Static.CameraAttachedTo;
            if (m_lastCamera != MyCameraAttachedToEnum.PlayerMinerShip
                && m_lastCamera != MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic
                && m_lastCamera != MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing
                && m_lastCamera != MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonStatic)
            {
                m_lastCamera = MyCameraAttachedToEnum.PlayerMinerShip;
            }

            PlayerStatistics.Deaths++;
            UpdateStats();

            DisplayDeathNotification(killerId);

            // This is really SICK!
            MyGuiManager.CloseIngameScreens();

            m_respawnTime = MyMinerGame.TotalGamePlayTimeInMilliseconds + (int)m_multiplayerConfig.RespawnTime.TotalMilliseconds;
            Log("DIE, respawn in: " + m_multiplayerConfig.RespawnTime.TotalMilliseconds + " ms");
        }

        void OnPeerDied(MyPlayerRemote deadPlayer, byte? killerId)
        {
            deadPlayer.Ship.SetAfterburner(false);
            deadPlayer.Ship = null;

            if (IsHost)
            {
                ClearLocks(deadPlayer.PlayerId);
            }

            if (killerId == MyEntityIdentifier.CurrentPlayerId)
            {
                PlayerStatistics.PlayersKilled++;
                UpdateStats();
            }

            DisplayKillNotification(deadPlayer, killerId);
        }

        void DisplayDeathNotification(byte? killerId)
        {
            MyPlayerRemote killerPlayer;
            if (killerId.HasValue && Peers.TryGetPlayer(killerId.Value, out killerPlayer))
            {
                Notify(Localization.MyTextsWrapperEnum.MP_YouHaveBeenKilledByX, killerPlayer.GetDisplayName());
            }
            else if (killerId == MyEntityIdentifier.CurrentPlayerId)
            {
                // Until entity ids fixed
                //Notify(Localization.MyTextsWrapperEnum.MP_YouHaveKilledYourself);
                Notify(Localization.MyTextsWrapperEnum.MP_YouHaveBeenKilled);
            }
            else
            {
                Notify(Localization.MyTextsWrapperEnum.MP_YouHaveBeenKilled);
            }
        }

        void DisplayKillNotification(MyPlayerRemote deadPlayer, byte? killerId)
        {
            MyPlayerRemote killer;

            if (killerId == MyEntityIdentifier.CurrentPlayerId)
            {
                Notify(Localization.MyTextsWrapperEnum.MP_YouHaveKilledX, deadPlayer.GetDisplayName());
            }
            else if (killerId == deadPlayer.PlayerId)
            {
                // Until entity ids fixed
                //Notify(Localization.MyTextsWrapperEnum.MP_XKilledHimself, deadPlayer.GetDisplayName());
                Notify(Localization.MyTextsWrapperEnum.MP_XKilled, deadPlayer.GetDisplayName());
            }
            else if (killerId.HasValue && killerId != 0 && Peers.TryGetPlayer(killerId.Value, out killer))
            {
                Notify(Localization.MyTextsWrapperEnum.MP_XHasBeenKilledByY, deadPlayer.GetDisplayName(), killer.GetDisplayName());
            }
            else
            {
                Notify(Localization.MyTextsWrapperEnum.MP_XKilled, deadPlayer.GetDisplayName());
            }
        }

        public void UpdatePosition(MyEntity entity, Matrix worldMatrix, Vector3 velocity, Vector3 acceleration)
        {
            Debug.Assert(entity != null);
            Debug.Assert(!entity.Closed);

            var posMsg = new MyEventUpdatePosition();
            posMsg.EntityId = entity.EntityId.Value.NumericValue;
            posMsg.Position = new MyMwcPositionAndOrientation(worldMatrix);
            posMsg.Velocity = velocity;
            posMsg.Acceleration = acceleration;
            Peers.SendToAll(ref posMsg, entity.EntityId.Value, m_multiplayerConfig.PositionTickRateMax, NetDeliveryMethod.UnreliableSequenced, 1);
        }

        public void UpdatePosition(MyEntity entity)
        {
            Debug.Assert(entity != null);
            Debug.Assert(!entity.Closed);

            var posMsg = new MyEventUpdatePosition();
            MyEntity.FillMessage(entity, ref posMsg);
            Peers.SendToAll(ref posMsg, entity.EntityId.Value, m_multiplayerConfig.PositionTickRateMax, NetDeliveryMethod.UnreliableSequenced, 1);
        }

        void OnUpdatePosition(ref MyEventUpdatePosition msg)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Update position");
            MyEntityIdentifier entityId = new MyEntityIdentifier(msg.EntityId);

            if (!CheckSenderId(msg, msg.EntityId))
            {
                Alert("Player is updating entity which is not his", msg.SenderEndpoint, MyEventEnum.UPDATE_POSITION);
                return;
            }

            MyEntity entity;
            if (MyEntities.TryGetEntityById(entityId, out entity))
            {
                entity.WorldMatrix = msg.Position.GetMatrix();
                if (entity.Physics != null)
                {
                    entity.Physics.LinearVelocity = msg.Velocity;
                    entity.Physics.LinearAcceleration = msg.Acceleration;
                }
                entity.Physics.AngularVelocity = Vector3.Zero;
                //FixPlayerPosition(entity);
            }
            else
            {
                Alert("Entity to update not found", msg.SenderEndpoint, MyEventEnum.UPDATE_POSITION);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public void UpdatePositionFast(MyEntity entity, Vector3 position, Vector3? up = null, Vector3? forward = null)
        {
            Debug.Assert(entity != null);
            Debug.Assert(!entity.Closed);

            var posMsg = new MyEventUpdatePositionFast();
            posMsg.EntityId = entity.EntityId.Value.NumericValue;
            posMsg.Position = position;
            posMsg.Up = up;
            posMsg.Forward = forward;
            Peers.SendToAll(ref posMsg, NetDeliveryMethod.ReliableSequenced, 2);
        }

        void OnUpdatePositionFast(ref MyEventUpdatePositionFast msg)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Update position fast");
            MyPlayerRemote player = (MyPlayerRemote)msg.SenderConnection.Tag;
            MyEntityIdentifier entityId = new MyEntityIdentifier(msg.EntityId);

            if (!CheckSenderId(msg, msg.EntityId))
            {
                Alert("Player is updating entity which is not his", msg.SenderEndpoint, msg.EventType);
                return;
            }

            MyEntity entity;
            if (MyEntities.TryGetEntityById(entityId, out entity))
            {
                var m = entity.WorldMatrix;
                m.Translation = msg.Position;
                if (msg.Up != null)
                    m.Up = msg.Up.Value;
                if (msg.Forward != null)
                    m.Forward = msg.Forward.Value;
                entity.WorldMatrix = m;
            }
            else
            {
                Alert("Entity to update not found", msg.SenderEndpoint, msg.EventType);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public void UpdateRotationFast(MyEntity entity, Vector3 rotation)
        {
            Debug.Assert(entity != null);
            Debug.Assert(!entity.Closed);
            Debug.Assert(entity.EntityId.HasValue);

            var posMsg = new MyEventUpdateRotationFast();
            posMsg.EntityId = entity.EntityId.Value.NumericValue;
            posMsg.Rotation = rotation;
            Peers.SendToAll(ref posMsg, entity.EntityId.Value, m_multiplayerConfig.RotationTickRate, NetDeliveryMethod.ReliableSequenced, 4);
        }

        void OnUpdateRotationFast(ref MyEventUpdateRotationFast msg)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Update rotation fast");
            MyPlayerRemote player = (MyPlayerRemote)msg.SenderConnection.Tag;
            MyEntityIdentifier entityId = new MyEntityIdentifier(msg.EntityId);

            if (!CheckSenderId(msg, msg.EntityId))
            {
                Alert("Player is updating entity which is not his", msg.SenderEndpoint, msg.EventType);
                return;
            }

            MyEntity entity;
            if (MyEntities.TryGetEntityById(entityId, out entity))
            {
                if (entity is MyPrefabLargeWeapon)
                {
                    var gun = ((MyPrefabLargeWeapon)entity).GetGun();
                    gun.SetRotationAndElevation(msg.Rotation);
                }
            }
            else
            {
                Alert("Entity to update not found", msg.SenderEndpoint, msg.EventType);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// When other player sends position update, and is in collision with player ship,
        /// player ship is moved away from collision (iteratively).
        /// Physics definitelly needs to handle collisions properly (when 2 objects collide, move them away)
        /// </summary>
        void FixPlayerPosition(MyEntity collidingEntity)
        {
            MyEntities.CollisionsElements.Clear();
            MyEntities.GetCollisionListForElement(collidingEntity.Physics.GetRBElementList()[0]);
            foreach (var e in MyEntities.CollisionsElements)
            {
                var playerShip = ((MinerWars.AppCode.Game.Physics.MyPhysicsBody)e.GetRigidBody().m_UserData).Entity;
                if (playerShip == MySession.PlayerShip)
                {
                    var dir = playerShip.WorldMatrix.Translation - collidingEntity.WorldMatrix.Translation;
                    if (!MyMwcUtils.HasValidLength(dir))
                    {
                        dir = Vector3.Up; // any vector
                    }
                    dir = Vector3.Normalize(dir);

                    do
                    {
                        var matrix = playerShip.WorldMatrix;
                        matrix.Translation += dir * 0.1f; // Move away from collision by 10cm
                        playerShip.WorldMatrix = matrix;
                    } while (MyPhysics.physicsSystem.GetRBInteractionModule().DoStaticTestInteraction(collidingEntity.Physics.GetRBElementList()[0], playerShip.Physics.GetRBElementList()[0]));
                }
            }
        }

        public void SpawnBot(MySpawnPoint spawnPoint, MySmallShipBot bot, int botsIdx, Vector3 spawnPosition)
        {
            MyEventSpawnBot msg = new MyEventSpawnBot();
            msg.SpawnPointId = (uint)spawnPoint.EntityId.Value.NumericValue;
            msg.DesiredBotId = (uint)bot.EntityId.Value.NumericValue;
            msg.BotsIdx = botsIdx;
            msg.SpawnPosition = spawnPosition;

            LogDevelop(string.Format("SpawnBot(SpawnPointId = {0}, DesiredBotId = {1}, BotsIdx = {2})", msg.SpawnPointId, msg.DesiredBotId, msg.BotsIdx));

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void OnSpawnBot(ref MyEventSpawnBot msg)
        {
            LogDevelop(string.Format("OnSpawnBot(SpawnPointId = {0}, DesiredBotId = {1}, BotsIdx = {2})", msg.SpawnPointId, msg.DesiredBotId, msg.BotsIdx));

            MySpawnPoint spawnPoint;
            if (!MyEntityIdentifier.TryGetEntity<MySpawnPoint>(new MyEntityIdentifier(msg.SpawnPointId), out spawnPoint))
            {
                Alert("Spawn point not found", msg.SenderEndpoint, MyEventEnum.SPAWN_BOT);
                return;
            }

            MyEntity existingEntity;
            if (MyEntities.TryGetEntityById(msg.DesiredBotId.ToEntityId(), out existingEntity))
            {
                Debug.Fail("Spawning bot, but another entity with same id already exists");
            }
            else
            {
                spawnPoint.SpawnShip(msg.BotsIdx, msg.SpawnPosition, msg.DesiredBotId);
            }

        }

        #region SHIP_CONFIG
        void Config_ConfigChanged(MySmallShip ship)
        {
            UpdateConfig(ship);
        }

        void UpdateConfig(MySmallShip ship)
        {
            Debug.Assert(ship.EntityId.HasValue);

            MyEventShipConfigUpdate msg = new MyEventShipConfigUpdate();
            msg.ShipId = ship.EntityId.Value.NumericValue;
            msg.Autoleveling = ship.Config.AutoLeveling.On;
            msg.EngineOn = ship.Config.Engine.On;
            msg.ReflectorLongRange = ship.Config.ReflectorLongRange.On;
            msg.ReflectorOn = ship.Config.ReflectorLight.On;
            msg.Slowdown = ship.Config.MovementSlowdown.On;
            msg.TimeBombTimer = ship.Config.TimeBombTimer.CurrentValue;
            msg.RadarJammer = ship.Config.RadarJammer.On;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        void OnUpdateConfig(ref MyEventShipConfigUpdate msg)
        {
            var ship = (MySmallShip)MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier(msg.ShipId));
            if (ship != null && CheckSenderId(msg, msg.ShipId))
            {
                ship.Config.AutoLeveling.SetValue(msg.Autoleveling);
                ship.Config.Engine.SetValue(msg.EngineOn);
                ship.Config.ReflectorLongRange.SetValue(msg.ReflectorLongRange);
                ship.Config.ReflectorLight.SetValue(msg.ReflectorOn);
                ship.Config.MovementSlowdown.SetValue(msg.Slowdown);
                ship.Config.TimeBombTimer.SetValue(msg.TimeBombTimer);
                ship.Config.RadarJammer.SetValue(msg.RadarJammer);
            }
            else
            {
                Alert("Updating config, but ship not found or is invalid", msg.SenderEndpoint, msg.EventType);
            }
        }
        #endregion

        #region INVENTORY

        void UpdateInventory(MyEntity entity, bool gameMechanicsItemsOnly)
        {
            Debug.Assert(entity is IMyInventory);
            Debug.Assert(entity.EntityId.HasValue);

            var inventoryEntity = (IMyInventory)entity;
            if (inventoryEntity.Inventory.IsDummy) return;

            var msg = new MyEventInventoryUpdate();
            msg.EntityId = entity.EntityId.Value.NumericValue;
            msg.InventoryBuilder = GetInventory(inventoryEntity, gameMechanicsItemsOnly);
            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0, DEFAULT_LARGE_MESSAGE_SIZE);
        }

        /// <summary>
        /// Gets inventory, only items which affect multiplayer
        /// </summary>
        MyMwcObjectBuilder_Inventory GetInventory(IMyInventory ship, bool gameMechanicsItemsOnly)
        {
            var builder = ship.Inventory.GetObjectBuilder(false);
            builder.InventoryItems.RemoveAll(ItemPredicateRemoveShips);
            if (gameMechanicsItemsOnly)
            {
                builder.InventoryItems.RemoveAll(ItemPredicateRemove);
            }
            return builder;
        }

        bool ItemPredicateRemoveShips(MyMwcObjectBuilder_InventoryItem item)
        {
            return item.ItemObjectBuilder != null && item.ItemObjectBuilder is MyMwcObjectBuilder_Ship;
        }

        bool ItemPredicateRemove(MyMwcObjectBuilder_InventoryItem item)
        {
            bool isJammer = item.ItemObjectBuilder.GetObjectBuilderType() == MyMwcObjectBuilderTypeEnum.SmallShip_Tool && item.ItemObjectBuilder.GetObjectBuilderId() == (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER;
            return !isJammer;
        }

        void OnInventoryUpdate(ref MyEventInventoryUpdate msg)
        {
            var inventory = MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier(msg.EntityId)) as IMyInventory;
            if (inventory != null)
            {
                inventory.Inventory.IsDummy = true;
                inventory.Inventory.Init(msg.InventoryBuilder);
                inventory.Inventory.IsDummy = false;
            }
        }

        void SavePlayer()
        {
            if (!IsHost && GameType == MyGameTypes.Story)
            {
                MyEventSavePlayer msg = new MyEventSavePlayer();
                msg.PlayerObjectBuilder = MySession.Static.Player.GetObjectBuilder(false);

                // It's called in shutdown
                Peers.TrySendHost(ref msg, MyMultiplayerGameplay.DEFAULT_LARGE_MESSAGE_SIZE);
            }
        }

        void OnSavePlayer(ref MyEventSavePlayer msg)
        {
            var player = (MyPlayerRemote)msg.SenderConnection.Tag;
            if (player != null)
            {
                MyGuiScreenGamePlay.Static.Checkpoint.StoreCoopPlayer(msg.PlayerObjectBuilder, player.GetDisplayName().ToString());
            }
        }
        #endregion

        #region UTILITY
        private static MyMwcObjectBuilder_SmallShip_TypesEnum ChooseShip(MyMwcObjectBuilder_FactionEnum faction)
        {
            return GetFactionShip(faction);
        }
        #endregion

        #region SHIP_TEMPLATES
        private MyMwcObjectBuilder_SmallShip CreateShip(MyMwcObjectBuilder_SmallShip_TypesEnum shipType, MyMwcObjectBuilder_FactionEnum faction)
        {
            var ship = new MyMwcObjectBuilder_SmallShip(shipType, new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>()
                {
                    new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Armor_Piercing_Incendiary), 10000),
                    new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic), 10000),
                    new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection), 10000)
                }, 100),
                new List<MyMwcObjectBuilder_SmallShip_Weapon>()
                {
                    new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon),
                    new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon),
                    new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher),
                }
                , new CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools.MyMwcObjectBuilder_SmallShip_Engine(CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools.MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1),
                new List<CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools.MyMwcObjectBuilder_AssignmentOfAmmo>()
                {
                    new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic),
                    new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection),
                }
                , new CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools.MyMwcObjectBuilder_SmallShip_Armor(CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools.MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic),
                null, 1000, 1, 100, 100, 100, true, false, 50, 0);
            ship.Faction = faction;
            ship.IsDummy = true;
            return ship;
        }

        public static MyMwcObjectBuilder_SmallShip_Player CreateDeathmatchShip(MyMwcObjectBuilder_FactionEnum faction)
        {
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            List<MyMwcObjectBuilder_SmallShip_Ammo> ammo = new List<MyMwcObjectBuilder_SmallShip_Ammo>();
            List<MyMwcObjectBuilder_AssignmentOfAmmo> assignments = new List<MyMwcObjectBuilder_AssignmentOfAmmo>(
                new MyMwcObjectBuilder_AssignmentOfAmmo[]
            {
                new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic),
                new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic),
                new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Third, MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic),
                new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fourth, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic),
                new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fifth, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic),
            }
            );

            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun));
            //weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front));
            weapons.Add(new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic));
            ammo.Add(new MyMwcObjectBuilder_SmallShip_Ammo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic));

            MyMwcObjectBuilder_Inventory inventory = new MyMwcObjectBuilder_Inventory(new List<MyMwcObjectBuilder_InventoryItem>(), 1000);
            foreach (MyMwcObjectBuilder_SmallShip_Ammo ammoItem in ammo)
            {
                inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(ammoItem, 1000));
            }
            inventory.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1), 1));
            var ship = new MyMwcObjectBuilder_SmallShip_Player(
                ChooseShip(faction),
                inventory,
                weapons,
                new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1),
                assignments,
                null,
                new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1),
                null,
                MyGameplayConstants.HEALTH_RATIO_MAX,
                100f,
                float.MaxValue,
                float.MaxValue,
                float.MaxValue,
                true, false, MyLightsConstants.MAX_SPOTLIGHT_SHADOW_RANGE, 0);

            ship.Faction = faction;
            return ship;
        }
        #endregion

        #region HEALTH
        public void UpdateHealth(MyEntity entity, float newHealthRatio)
        {
            if (!IsControlledByMe(entity)) return;

            Debug.Assert(entity.EntityId.HasValue);
            MyEventHealthUpdate msg = new MyEventHealthUpdate();
            msg.EntityId = entity.EntityId.Value.NumericValue;
            msg.NewHealthRatio = entity.HealthRatio;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void OnHealthUpdate(ref MyEventHealthUpdate msg)
        {
            MyEntity entity;
            if (MyEntities.TryGetEntityById(new MyEntityIdentifier(msg.EntityId), out entity))
            {
                entity.HealthRatio = msg.NewHealthRatio;
            }
            else
            {
                Alert("Update health on nonexistent entity", msg.SenderEndpoint, msg.EventType);
            }
        }
        #endregion

        #region AFTERBURNER
        public void Afterburner(bool enabled)
        {
            Debug.Assert(MySession.PlayerShip.EntityId.HasValue);

            var msg = new MyEventAfterburner();
            msg.EntityId = MySession.PlayerShip.EntityId.Value.NumericValue;
            msg.Enabled = enabled;
            Peers.SendToAll(ref msg);
        }

        void OnAfterburner(ref MyEventAfterburner msg)
        {
            MyEntity entity;
            if (MyEntities.TryGetEntityById(msg.EntityId.ToEntityId(), out entity) && entity is MySmallShip)
            {
                var ship = (MySmallShip)entity;
                ship.SetAfterburner(msg.Enabled);
            }
        }
        #endregion
    }
}
