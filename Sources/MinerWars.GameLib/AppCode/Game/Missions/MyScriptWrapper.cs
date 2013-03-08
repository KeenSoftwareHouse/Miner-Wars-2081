#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions.Components;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWarsMath;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Entities.WayPoints;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Entities.CargoBox;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.TransparentGeometry;

#endregion

namespace MinerWars.AppCode.Game.Missions
{
    static class MyScriptWrapper
    {
        #region Event & delegates

        public delegate void EntityInventoryContentChangeHandler(MyEntity entity, MyInventory inventory);
        public delegate void EntityInventoryItemAmountChangeHandler(MyEntity entity, MyInventory inventory, MyInventoryItem inventoryItem, float amountChanged);
        public delegate void EntityHandler(MyEntity entity);
        public delegate void PrefabHandler(MyPrefabBase prefab);
        public delegate void CancelHandler(CancelEventArgs cancelEventArgs);
        public delegate void SpawnPointHandler(MySpawnPoint spawnPoint);
        public delegate void EntityEntityHandler(MyEntity entity1, MyEntity entity2);
        public delegate void DialogueHandler(MyDialogueEnum dialogue, bool interrupted);
        public delegate void SentenceHandler(MyDialogueEnum dialogue, MyDialoguesWrapperEnum sentence);
        public delegate void AttackHandler(MyEntity attacker, MyEntity target);

        public static event EntityInventoryContentChangeHandler EntityInventoryContentChanged;
        public static event EntityInventoryItemAmountChangeHandler EntityInventoryItemAmountChanged;
        public static event EntityEntityHandler EntityDeath;
        public static event EntityHandler EntityClose;
        public static event EntityHandler EntityClosing;
        public static event SpawnPointHandler SpawnpointBotsKilled;
        public static event PrefabHandler PrefabBuilt;
        public static event EntityEntityHandler OnBotReachedWaypoint;
        public static event EntityEntityHandler OnSpawnpointBotSpawned;
        public static event EntityEntityHandler OnEntityAtacked;
        public static event EntityEntityHandler AlarmLaunched;
        public static event Action OnUseKeyPress;
        public static event Action OnHarvesterUse;
        public static event DialogueHandler OnDialogueFinished;
        public static event SentenceHandler OnSentenceStarted;
        public static event EntityHandler EntityHacked;
        public static event AttackHandler EntityAttackedByBot;

        public static event Action SwitchTowerPrevious;
        public static event Action SwitchTowerNext;

        #endregion

        static MyScriptWrapper()
        {
            MyDialogues.OnDialogueFinished += DialogueFinished;
            MyDialogues.OnSentenceStarted += SentenceStarted;
        }

        #region Event Methods (Called from code)

        public static void DialogueFinished(MyDialogueEnum dialogue, bool interrupted)
        {
            if (OnDialogueFinished != null)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyScriptWrapper::DialogueFinished");
                OnDialogueFinished(dialogue, interrupted);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        public static void SentenceStarted(MyDialogueEnum dialogue, MyDialoguesWrapperEnum sentence)
        {
            if (OnSentenceStarted != null)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyScriptWrapper::SentenceStarted");
                OnSentenceStarted(dialogue, sentence);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        public static void OnEntityInventoryContentChange(MyEntity entity, MyInventory inventory)
        {
            if (EntityInventoryContentChanged != null)
            {
                EntityInventoryContentChanged(entity, inventory);
            }
        }

        public static void OnEntityInventoryAmountChange(MyEntity entity, MyInventory inventory, MyInventoryItem inventoryItem, float amountChanged)
        {
            if (EntityInventoryItemAmountChanged != null)
            {
                EntityInventoryItemAmountChanged(entity, inventory, inventoryItem, amountChanged);
            }
        }

        public static void OnEntityClose(MyEntity entity)
        {
            if (EntityClose != null)
            {
                EntityClose(entity);
            }
        }

        public static void OnEntityClosing(MyEntity entity)
        {
            if (EntityClosing != null)
            {
                EntityClosing(entity);
            }
        }

        public static void OnSpawnpointBotsKilled(MySpawnPoint spawnPoint)
        {
            if (SpawnpointBotsKilled != null)
            {
                SpawnpointBotsKilled(spawnPoint);
            }
        }

        public static void OnAlarmLaunched(MyPrefabContainer prefabContainer, MyEntity enemyEntity)
        {
            if (AlarmLaunched != null)
            {
                AlarmLaunched(prefabContainer, enemyEntity);
            }
        }

        public static void OnEntityDeath(MyEntity entity, MyEntity killedBy = null)
        {
            if (EntityDeath != null && entity.EntityId.HasValue)
            {
                EntityDeath(entity, killedBy);
            }
        }

        public static void OnEntityCreated(MyEntity entity)
        {
            if (MyGuiScreenGamePlay.Static.IsIngameEditorActive() && entity is MyPrefabBase)
            {
                if (PrefabBuilt != null)
                {
                    PrefabBuilt(entity as MyPrefabBase);
                }
            }
        }

        public static void OnEntityHacked(MyEntity entity)
        {
            if (EntityHacked != null)
            {
                EntityHacked(entity);
            }
        }

        public static void OnEntityAttackedByBot(MyEntity attacker, MyEntity target)
        {
            if (EntityAttackedByBot != null)
            {
                EntityAttackedByBot(attacker, target);
            }
        }

        public static void UseKeyPressed()
        {
            if (OnUseKeyPress != null)
            {
                OnUseKeyPress();
            }

        }

        public static void HarvesterUse()
        {
            if (OnHarvesterUse != null)
            {
                OnHarvesterUse();
            }

        }


        public static MyGameControlEnums GetUseControlKey()
        {
            return MyGameControlEnums.USE;
        }

        public static void RollLeftPressed()
        {

            if (SwitchTowerPrevious != null && MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.LargeWeapon)
            {
                SwitchTowerPrevious();
            }

        }

        public static void RollRightPressed()
        {

            if (SwitchTowerNext != null && MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.LargeWeapon)
            {
                SwitchTowerNext();
            }

        }

        public static bool IsFoundationFactoryDeployable()
        {
            return true;
        }

        public static void SetCanDropFoundationFactory(bool canDrop)
        {
            MyGuiScreenGamePlay.Static.FoundationFactoryDropEnabled = canDrop;
        }

        public static void BotReachedWaypoint(MyEntity bot, MyEntity waypoint)
        {
            if (OnBotReachedWaypoint != null)
            {
                OnBotReachedWaypoint(bot, waypoint);
            }
        }

        public static void SpawnpointBotSpawned(MyEntity spawnpoint, MyEntity bot)
        {
            if (OnSpawnpointBotSpawned != null)
            {
                OnSpawnpointBotSpawned(spawnpoint, bot);
            }
        }

        public static void EntityAtacked(MyEntity atacker, MyEntity target)
        {
            if (OnEntityAtacked != null)
            {
                OnEntityAtacked(atacker, target);
            }
        }
        #endregion

        public static bool IsSmallShip(MyEntity entity)
        {
            return entity is MySmallShip;
        }

        public static bool IsLargeShip(MyEntity entity)
        {
            return entity is MyPrefabLargeShip;
        }

        public static bool IsFactionMember(MyEntity entity, MyMwcObjectBuilder_FactionEnum faction)
        {
            var ship = entity as MyShip;
            return ship != null && ship.Faction == faction;
        }

        public static bool IsPlayerShip(MyEntity entity)
        {
            return MySession.IsPlayerShip(entity);
        }

        public static MyInventory GetPlayerInventory()
        {
            return MySession.PlayerShip != null ? MySession.PlayerShip.Inventory : null;
        }

        public static MyInventory GetCentralInventory()
        {
            return MySession.Static != null ? MySession.Static.Inventory : null;
        }

        public static void EnsureInventoryItem(MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId, float amount = 1)
        {
            if (GetPlayerInventory().Contains(objectBuilderType, objectBuilderId) || GetCentralInventory().Contains(objectBuilderType, objectBuilderId))
            {
                return;
            }

            GetCentralInventory().AddInventoryItem(objectBuilderType, objectBuilderId, amount, true);
        }

        public static void EnsureInventoryItem(MyInventory inventory, MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId, float amount = 1)
        {
            if (inventory.Contains(objectBuilderType, objectBuilderId))
            {
                return;
            }

            inventory.AddInventoryItem(objectBuilderType, objectBuilderId, amount, true);
        }

        public static void AddInventoryItems(MyInventory inventory, List<MyInventoryItem> inventoryItems, MyInventory fallbackInventory)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                var inventoryItem = inventoryItems[i];

                if (inventory.IsFull)
                {
                    Debug.Assert(fallbackInventory != null);

                    var newList = inventoryItems.GetRange(i, inventoryItems.Count - i);
                    AddInventoryItems(fallbackInventory, newList, null);
                    break;
                }
                else
                {
                    inventory.AddInventoryItem(inventoryItem);
                }
            }
        }

        public static void AddInventoryItem(MyInventory inventory, MyMwcObjectBuilderTypeEnum objectBuilderType, int? objectBuilderId, float amount = 1f, bool increaseCapacityIfInventoryIsFull = false, bool playInventoryIsFull = false, bool removeAnotherItemsIfFull = false)
        {
            float amountLeft = inventory.AddInventoryItem(objectBuilderType, objectBuilderId, amount, true, increaseCapacityIfInventoryIsFull);

            if (amountLeft > 0f)
            {
                if (playInventoryIsFull)
                {
                    MyScriptWrapper.PlaySound2D(MySoundCuesEnum.HudInventoryFullWarning);
                }
                if (removeAnotherItemsIfFull)
                {
                    var gamePlayProp = MyGameplayConstants.GetGameplayProperties(objectBuilderType, objectBuilderId != null ? objectBuilderId.Value : 0, MyMwcObjectBuilder_FactionEnum.Euroamerican);
                    int itemsAddNeed = (int)Math.Ceiling(amountLeft / gamePlayProp.MaxAmount);
                    Debug.Assert(itemsAddNeed <= inventory.MaxItems, "We can't add more items, than is inventory capacity!");
                    List<MyInventoryItem> itemsToRemove = GetInventoryItemsToRemove(inventory, itemsAddNeed);
                    inventory.RemoveInventoryItems(itemsToRemove, true);
                    amountLeft = inventory.AddInventoryItem(objectBuilderType, objectBuilderId, amountLeft, true, increaseCapacityIfInventoryIsFull);
                    Debug.Assert(amountLeft <= 0f);
                }
            }
        }

        private static List<MyInventoryItem> GetInventoryItemsToRemove(MyInventory inventory, int count)
        {
            Debug.Assert(inventory.GetInventoryItems().Count >= count);
            List<MyInventoryItem> items = new List<MyInventoryItem>();

            // try find ammo
            inventory.GetInventoryItems(ref items, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, null);
            if (items.Count < count)
            {
                // try find ore
                inventory.GetInventoryItems(ref items, MyMwcObjectBuilderTypeEnum.Ore, null);
            }
            // try find random item
            while (items.Count < count)
            {
                int randomIndex = MyMwcUtils.GetRandomInt(inventory.GetInventoryItems().Count - 1);
                MyInventoryItem randomItem = inventory.GetInventoryItems()[randomIndex];
                if (!items.Contains(randomItem))
                {
                    items.Add(randomItem);
                }
            }
            items = items.GetRange(0, Math.Min(count, items.Count));
            return items;
        }

        public static int GetInventoryItemCount(MyInventory inventory, MyMwcObjectBuilderTypeEnum objectbuilderType, int? objectBuilderId)
        {
            return inventory.GetInventoryItemsCount(objectbuilderType, objectBuilderId);
        }

        public static float GetInventoryItemAmount(MyInventory inventory, MyMwcObjectBuilderTypeEnum objectbuilderType, int? objectBuilderId)
        {
            return inventory.GetTotalAmountOfInventoryItems(objectbuilderType, objectBuilderId);
        }

        public static bool RemoveInventoryItemAmount(MyInventory inventory, MyMwcObjectBuilderTypeEnum objectbuilderType, int? objectBuilderId, float amount)
        {
            return inventory.RemoveInventoryItemAmount(objectbuilderType, objectBuilderId, amount);
        }

        public static bool IsPlayerNearLocation(MyMissionBase.MyMissionLocation location, float distance)
        {
            return
                MySession.PlayerShip != null &&
                location != null &&
                MyGuiScreenGamePlay.Static.IsCurrentSector(location.Sector) &&
                location.Entity != null &&
                (MySession.PlayerShip.GetPosition() - location.Entity.GetPosition()).LengthSquared() <= distance * distance;
        }

        public static void ShowMessageBox(MyMessageBoxType type, MyTextsWrapperEnum text, MyTextsWrapperEnum caption)
        {
            MyGuiManager.AddScreen(new MyGuiScreenMessageBox(type, text, caption, MyTextsWrapperEnum.Ok, null));
        }

        public static bool IsGameActive()
        {
            return MyGuiScreenGamePlay.Static.IsGameActive();
        }

        public static List<MyEntity> GetSmallShips(MyMwcObjectBuilder_FactionEnum faction)
        {
            List<MyEntity> entities = new List<MyEntity>();
            MyEntities.FindEntities(a =>
                {
                    MySmallShip smallShip = a as MySmallShip;
                    return smallShip != null && smallShip.Faction == faction;
                }, entities);
            return entities;
        }

        public static MyPrefabLargeShip GetMotherShip()
        {
            List<MyEntity> largeShips = new List<MyEntity>();
            if (MyEntities.FindEntitiesRecursive(a => a is MyPrefabLargeShip && a.Faction == MySession.PlayerShip.Faction, largeShips))
            {
                return largeShips.FirstOrDefault() as MyPrefabLargeShip;
            }
            return null;
        }

        public static MyPrefabLargeShip GetLargeShip(string name)
        {
            List<MyEntity> largeShips = new List<MyEntity>();
            if (MyEntities.FindEntitiesRecursive(a => a is MyPrefabLargeShip && a.DisplayName.Equals(name, StringComparison.InvariantCultureIgnoreCase), largeShips))
            {
                return largeShips.FirstOrDefault() as MyPrefabLargeShip;
            }
            return null;
        }


        /// <summary>
        /// Finds an entity by id, and fails when there is no such entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MyEntity GetEntity(uint id)
        {
            MyEntity entity;
            MyEntities.TryGetEntityById(new MyEntityIdentifier(id), out entity);
            if (entity == null)
            {
                Log("script: did not find entity with ID: " + id);
                Debug.Fail("script: did not find entity with ID: " + id);
            }
            
            return entity;
        }

        /// <summary>
        /// Finds an entity by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MyEntity GetEntity(string name)
        {
            MyEntity entity;
            MyEntities.TryGetEntityByName(name, out entity);
            if (entity == null)
            {
#if DETECT_LEAKS
                entity = new MySmallShipBot();
                entity.EntityId = new MyEntityIdentifier(56156);
                return entity;
#endif

                Log("script: did not find entity with Name: " + name);
                Debug.Fail("script: did not find entity with Name: " + name);
            }
            return entity;
        }

        /// <summary>
        /// Finds an entity by mission location entity identifier (id or name)
        /// </summary>
        /// <param name="missionLocationIdentifier"></param>
        /// <returns></returns>
        public static MyEntity GetEntity(MyMissionBase.MyMissionLocationEntityIdentifier missionLocationIdentifier)
        {
            if (missionLocationIdentifier.LocationEntityId != null)
            {
                return GetEntity(missionLocationIdentifier.LocationEntityId.Value);
            }
            else
            {
                return GetEntity(missionLocationIdentifier.LocationEntityName);
            }
        }

        /// <summary>
        /// Finds an entity by mission location entity identifier (id or name), returns null if there is none
        /// </summary>
        /// <param name="missionLocationIdentifier"></param>
        /// <returns></returns>
        public static MyEntity TryGetEntity(MyMissionBase.MyMissionLocationEntityIdentifier missionLocationIdentifier)
        {
            if (missionLocationIdentifier.LocationEntityId != null)
            {
                return TryGetEntity(missionLocationIdentifier.LocationEntityId.Value);
            }
            else
            {
                return TryGetEntity(missionLocationIdentifier.LocationEntityName);
            }
        }

        /// <summary>
        /// Finds an entity by id, returns null if there is none
        /// </summary>
        /// <param name="id">Id of entity you wish to find</param>
        /// <returns>Entity with specified id</returns>
        public static MyEntity TryGetEntity(uint id)
        {
            MyEntity entity;
            MyEntities.TryGetEntityById(new MyEntityIdentifier(id), out entity);
            return entity;
        }

        /// <summary>
        /// Finds an entity by name, returns null if there is none
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MyEntity TryGetEntity(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            MyEntity entity;
            MyEntities.TryGetEntityByName(name, out entity);
            return entity;
        }

        public static MyEntityDetector GetDetector(uint id)
        {
            return GetDetector(GetEntity(id));
        }

        public static MyPrefabBase GetPrefab(uint id)
        {
            return GetEntity(id) as MyPrefabBase;
        }

        public static MyPrefabBase TryGetPrefab(uint id)
        {
            return TryGetEntity(id) as MyPrefabBase;
        }

        public static MyEntityDetector GetDetector(MyEntity entity)
        {
            MyDummyPoint dummy = entity as MyDummyPoint;
            return dummy != null ? dummy.GetDetector() : null;
        }


        public static MyHudNotification.MyNotification CreateNotification(MyTextsWrapperEnum text, MyGuiFont font, MyEntity owner = null)
        {
            return new MyHudNotification.MyNotification(text, font, owner);
        }

        public static MyHudNotification.MyNotification CreateNotification(MyTextsWrapperEnum text, MyGuiFont font, int disaperTime, object[] textFormatArguments = null, MyEntity owner = null)
        {
            return new MyHudNotification.MyNotification(text, font, disaperTime, owner, textFormatArguments);
        }

        public static MyHudNotification.MyNotification CreateNotification(String text, MyGuiFont font, int disaperTime, MyEntity owner = null)
        {
            return new MyHudNotification.MyNotification(text, font, disaperTime, owner);
        }

        public static void AddNotification(MyHudNotification.MyNotification notification)
        {
            MyHudNotification.AddNotification(notification, MyGuiScreenGamePlayType.GAME_STORY);
        }

        public static bool IsCurrentSector(MyMwcVector3Int sector)
        {
            return MyGuiScreenGamePlay.Static.IsCurrentSector(sector);
        }

        public static void DeactivateBot(uint id)
        {
            var bot = MyScriptWrapper.GetEntity(id) as MySmallShipBot;
            Debug.Assert(bot != null);
            if (bot != null)
            {
                bot.Visible = false;

                if (bot.Physics.Enabled)
                {
                    bot.Physics.Enabled = false;
                }
            }
        }

        public static void PrepareMotherShipForMove(MyEntity motherShip)
        {
            Debug.Assert(motherShip != null);

            MyScriptWrapper.EnablePhysics(motherShip.EntityId.Value.NumericValue, false);

            MyPrefabContainer motherShipContainer = motherShip as MyPrefabContainer;
            Debug.Assert(motherShipContainer != null);
            motherShipContainer.Physics.Enabled = true;
            motherShipContainer.InitBoxPhysics(MyMaterialType.METAL, Vector3.Zero, motherShipContainer.WorldAABBHr.Size(), 10000, 0, 0, RigidBodyFlag.RBF_KINEMATIC);
            motherShipContainer.Physics.Enabled = true;

            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.UpdateFlags(motherShip, MyFlagsEnum.PREPARE_MOVE);
            }
        }

        public static void ReturnMotherShipFromMove(MyEntity motherShip)
        {
            Debug.Assert(motherShip != null);
            MyPrefabContainer motherShipContainer = motherShip as MyPrefabContainer;
            Debug.Assert(motherShipContainer != null);
            MyScriptWrapper.EnablePhysics(motherShip.EntityId.Value.NumericValue, true);
            motherShipContainer.InitBoxPhysics(MyMaterialType.METAL, Vector3.Zero, motherShipContainer.WorldAABBHr.Size(), 10000, 0, MyConstants.COLLISION_LAYER_UNCOLLIDABLE, RigidBodyFlag.RBF_RBO_STATIC);

            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.UpdateFlags(motherShip, MyFlagsEnum.RETURN_FROM_MOVE);
            }
        }


        public static void ActivateBot(uint id)
        {
            var bot = MyScriptWrapper.GetEntity(id) as MySmallShipBot;
            Debug.Assert(bot != null);
            if (bot != null)
            {
                bot.Visible = true;
                bot.Physics.Enabled = true;
            }
        }

        public static void ActivateSpawnPoint(uint id)
        {
            var spawnPoint = MyScriptWrapper.GetEntity(id) as MySpawnPoint;
            Debug.Assert(spawnPoint != null);
            if (spawnPoint != null && !spawnPoint.IsActive())
            {
                spawnPoint.Activate();
            }
        }

        public static void DeactivateSpawnPoint(uint id)
        {
            var spawnPoint = MyScriptWrapper.GetEntity(id) as MySpawnPoint;
            Debug.Assert(spawnPoint != null);
            if (spawnPoint != null && spawnPoint.IsActive())
            {
                spawnPoint.Deactivate();
            }
        }

        public static void ActivateSpawnPoints(List<uint> spawnPoints)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                MyScriptWrapper.ActivateSpawnPoint(spawnPoint);
            }
        }


        public static void DeactivateSpawnPoints(List<uint> spawnPoints)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                MyScriptWrapper.DeactivateSpawnPoint(spawnPoint);
            }
        }

        public static void SetEntityEnabled(MyEntity entity, bool enabled, bool checkEntityExist = false)
        {
            if (checkEntityExist)
            {
                Debug.Assert(entity != null);
            }
            if (entity != null)
            {
                if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost)
                {
                    MyMultiplayerGameplay.Static.UpdateFlags(entity, enabled ? MyFlagsEnum.ENABLE : MyFlagsEnum.DISABLE);
                }

                entity.Enabled = enabled;
            }
        }


        public static void SetEntityEnabled(uint entityId, bool enabled, bool checkEntityExist = false)
        {
            var entity = TryGetEntity(entityId);
            SetEntityEnabled(entity, enabled, checkEntityExist);
        }

        public static void SetEntitiesEnabled(List<MyEntity> entities, bool enabled, bool checkEntityExist = false)
        {
            foreach (MyEntity entity in entities)
            {
                SetEntityEnabled(entity, enabled, checkEntityExist);
            }
        }

        public static void SetEntitiesEnabled(List<uint> entities, bool enabled, bool checkEntityExist = false)
        {
            foreach (var id in entities)
            {
                MyEntity entity = TryGetEntity(id); 
                SetEntityEnabled(entity, enabled, checkEntityExist);
            }
        }

        public static void SetEntityDestructible(MyEntity entity, bool destructible)
        {
            //Avoid crashes caused bad designed scripts/spawnpoints..
            if (entity == null)
                return;

            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.UpdateFlags(entity, destructible ? MyFlagsEnum.DESTRUCTIBLE : MyFlagsEnum.INDESTRUCTIBLE);
            }

            entity.IsDestructible = destructible;
        }

        public static void MarkEntity(MyEntity entity, string text, MyHudIndicatorFlagsEnum flags = MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER | MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER, MyGuitargetMode guiTargetMode = MyGuitargetMode.Objective)
        {
            if (entity != null && (!(entity is MySpawnPoint)))
            {
                MyHud.ChangeText(entity, new StringBuilder(text), guiTargetMode, 0, flags);
            }
        }

        public static void RemoveEntityMark(MyEntity entity)
        {
            if (entity != null)
            {
                MyHud.RemoveText(entity);
            }
        }

        public static void SetFactionRelation(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2, float status)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendSetFactionRelation(faction1, faction2, status);
            }

            MyFactions.SetFactionStatus(faction1, faction2, status, false, true);
        }

        [Obsolete("Don't use this! Enable/disable particle using SetEntityEnabled and set particle flag in editor")]
        public static void SetParticleEffect(MyEntity entity, bool enabled)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.UpdateFlags(entity, MyFlagsEnum.PARTICLE, enabled);
            }

            var dummy = entity as MyDummyPoint;
            Debug.Assert(dummy != null);
            if (dummy != null)
            {
                dummy.DummyFlags = enabled ? dummy.DummyFlags | MyDummyPointFlags.PARTICLE : dummy.DummyFlags & ~MyDummyPointFlags.PARTICLE;
            }
        }

        public static void PlaySound2D(MySoundCuesEnum soundCueEnum)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendPlaySound(null, soundCueEnum);
            }

            MyAudio.AddCue2D(soundCueEnum);
        }

        public static void ApplyTransition(MyMusicTransitionEnum transitionEnum, int priority = 0, string category = null, bool loop = true)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendApplyTransition(transitionEnum, priority, category, loop);
            }

            MyAudio.ApplyTransition(transitionEnum, priority, category, loop);
        }

        public static void StopTransition(int priority)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendStopTransition(priority);
            }

            MyAudio.StopTransition(priority);
        }

        public static void StopMusic()
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendStopMusic();
            }

            MyAudio.StopMusic();
        }

        public static void PlaySound3D(uint entityId, MySoundCuesEnum soundCueEnum)
        {
            PlaySound3D(GetEntity(entityId), soundCueEnum);
        }

        public static void PlaySound3D(MyEntity entity, MySoundCuesEnum soundCueEnum)
        {
            Debug.Assert(entity != null);
            PlaySound3D(entity.GetPosition(), soundCueEnum);
        }

        public static void PlaySound3D(Vector3 position, MySoundCuesEnum soundCueEnum)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendPlaySound(position, soundCueEnum);
            }

            MyAudio.AddCue3D(soundCueEnum, position, Vector3.Forward, Vector3.Up, Vector3.Zero);
        }

        public static void SetSpawnPointLeader(uint id, MySmallShip leader)
        {
            var spawnPoint = MyScriptWrapper.GetEntity(id) as MySpawnPoint;
            Debug.Assert(spawnPoint != null);
            if (spawnPoint != null)
            {
                spawnPoint.Leader = leader;
            }
        }

        public static List<MySpawnPoint.Bot> GetSpawnPointBots(uint id)
        {
            var spawnPoint = MyScriptWrapper.GetEntity(id) as MySpawnPoint;
            Debug.Assert(spawnPoint != null);
            if (spawnPoint != null)
            {
                return spawnPoint.GetBots();
            }
            return null;
        }

        public static void SetEntityPriority(MyEntity entity, int priority, bool setForChildren = false)
        {
            Debug.Assert(entity != null);
            if (entity != null)
            {
                entity.AIPriority = priority;
            }

            if (setForChildren)
            {
                if (entity.Children != null)
                {
                    foreach (var child in entity.Children)
                    {
                        SetEntityPriority(child, priority, true);
                    }
                }
            }
        }

        public static int GetGameTime()
        {
            return MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        public static void SetHealth(MyEntity entity, float percents)
        {
            if (entity != null && entity.IsDestructible)
            {
                entity.Health = entity.Health * percents;
            }
        }

        public static void DamageEntity(MyEntity entity, float percents)
        {
            if (entity != null && entity.IsDestructible)
            {
                entity.DoDamage(0, (entity.MaxHealth * percents), 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
            }
        }

        public static void SetMaxHealth(MyEntity entity, float percents)
        {
            if (entity != null && entity.IsDestructible)
            {
                entity.Health = entity.MaxHealth * percents;
            }
        }

        public static uint GetEntityId(string entityName)
        {
            return GetEntity(entityName).EntityId.Value.NumericValue;
        }

        public static uint GetEntityId(MyEntity entity)
        {
            Debug.Assert(entity != null && entity.EntityId.HasValue);
            return entity.EntityId.HasValue ? entity.EntityId.Value.NumericValue : (uint)0;
        }

        public static void AddExplosions(List<uint> entities, MyExplosionTypeEnum type, float damage, float radius = 0, bool forceDebris = false, bool createDecals = false, MyParticleEffectsIDEnum? particleIDOverride = null)
        {
            foreach (uint id in entities)
            {
                MyEntity entity = GetEntity(id);
                AddExplosion(entity, type, damage, radius, forceDebris, createDecals, particleIDOverride);
            }
        }

        public static void AddExplosion(MyEntity entity, MyExplosionTypeEnum type, float damage, float radius = 0, bool forceDebris = false, bool createDecals = false, MyParticleEffectsIDEnum? particleIDOverride = null)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddExplosion");

            if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost)
            {
                MyMultiplayerGameplay.Static.AddExplosion(entity, type, damage, radius, forceDebris, createDecals, particleIDOverride);
            }

            MyDummyPoint dummyPoint = entity as MyDummyPoint;
            if (dummyPoint != null)
            {
                dummyPoint.DummyFlags |= MyDummyPointFlags.SURIVE_PREFAB_DESTRUCTION;
                dummyPoint.DummyFlags |= MyDummyPointFlags.PARTICLE;
                dummyPoint.Save = false; //We dont want to reuse explosion dummies, moreover it causes bugs when saved during explosion
            }

            BoundingSphere boundingSphere = new BoundingSphere(entity.WorldVolume.Center, entity.WorldVolume.Radius);

            if (radius > 0)
            {
                boundingSphere.Radius = radius;
            }

            MyExplosionInfo explosionInfo = new MyExplosionInfo(0, damage, 0, boundingSphere, type, true, dummyPoint, checkIntersection: false);
            explosionInfo.CreateDecals = createDecals;
            explosionInfo.ForceDebris = forceDebris;
            //explosionInfo.AffectVoxels = false;

            if (particleIDOverride.HasValue && dummyPoint != null)
            {
                dummyPoint.ParticleID = (float)(int)particleIDOverride.Value;
            }
            if (dummyPoint == null)
            {   //because default explosions need to be scaled
                explosionInfo.ParticleScale = entity.WorldVolume.Radius / 5.0f;
            }
            MyExplosion explosion = MyExplosions.AddExplosion();
            if (explosion != null)
            {
                explosion.Start(ref explosionInfo);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void AddExplosion(Vector3 position, MyExplosionTypeEnum explosionType, float radius, float damage, bool forceDebris = false, bool createDecals = false)
        {
            if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost)
            {
                MyMultiplayerGameplay.Static.AddExplosion(position, explosionType, damage, radius, forceDebris, createDecals);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyScriptWrapper.AddExplosion");

            MyExplosion explosion = MyExplosions.AddExplosion();
            if (explosion != null)
            {
                explosion.Start(0, damage, 0, explosionType, new BoundingSphere(position, radius), MyExplosionsConstants.EXPLOSION_LIFESPAN, 0, null, 1.0f, null, forceDebris, createDecals);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

        }

        public static void FixBotNames()
        {
            // disable because 0004748: Disable all HUD names for generic Friend, Enemy, Neutral
            return;
            if (MySession.PlayerShip != null)
            {
                string friendName = MyTextsWrapper.Get(MyTextsWrapperEnum.Friend).ToString();
                string neutralName = MyTextsWrapper.Get(MyTextsWrapperEnum.Neutral).ToString();
                string enemyName = MyTextsWrapper.Get(MyTextsWrapperEnum.Enemy).ToString();

                foreach (var entity in MyEntities.GetEntities())
                {
                    MySmallShipBot bot = entity as MySmallShipBot;
                    if (bot != null &&
                        (bot.DisplayName == null ||
                         bot.DisplayName == friendName ||
                         bot.DisplayName == neutralName ||
                         bot.DisplayName == enemyName))
                    {
                        var relationToPlayer = MyFactions.GetFactionsRelation(MySession.PlayerShip, bot);
                        switch (relationToPlayer)
                        {
                            case MyFactionRelationEnum.Neutral:
                                bot.DisplayName = neutralName;
                                break;
                            case MyFactionRelationEnum.Friend:
                                bot.DisplayName = friendName;
                                break;
                            case MyFactionRelationEnum.Enemy:
                                bot.DisplayName = enemyName;
                                break;
                        }
                    }
                }
            }
        }

        public static void SetBotReflectors(bool longRange)
        {
            foreach (var entity in MyEntities.GetEntities())
            {
                MySmallShipBot bot = entity as MySmallShipBot;
                if (bot != null)
                {
                    bot.Config.ReflectorLongRange.SetValue(longRange);
                }
            }
        }

        public static void SetPlayerFaction(MyMwcObjectBuilder_FactionEnum faction)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendFaction(faction);
            }

            Debug.Assert(MySession.Static != null && MySession.Static.Player != null);
            if (MySession.Static != null && MySession.Static.Player != null)
            {
                MySession.Static.Player.Faction = faction;
            }
        }

        public static MyEntity GetPrefabContainer(MyEntity entity)
        {
            MyPrefab prefab = entity as MyPrefab;
            if (prefab != null)
            {
                return prefab.Parent;
            }
            return null;
        }

        public static void AddVoxelHand(uint voxelMapId, uint entityId, float radius, MyMwcVoxelMaterialsEnum? voxelMaterial = null, MyMwcVoxelHandModeTypeEnum voxelHandType = MyMwcVoxelHandModeTypeEnum.ADD)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddVoxelHand");

            if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost)
            {
                MyMultiplayerGameplay.Static.AddVoxelHand(voxelMapId, entityId, radius, voxelHandType, voxelMaterial);
            }

            MyEntity entity = MyScriptWrapper.GetEntity(entityId);
            MyVoxelMap asteroid = (MyVoxelMap)MyScriptWrapper.GetEntity(voxelMapId);
            MyVoxelHandSphere voxelHandSphere = new MyVoxelHandSphere();
            voxelHandSphere.Init(new MyMwcObjectBuilder_VoxelHand_Sphere(new MyMwcPositionAndOrientation(entity.WorldMatrix), radius, voxelHandType), null);
            voxelHandSphere.Material = voxelMaterial != null ? voxelMaterial.Value : asteroid.VoxelMaterial;
            asteroid.AddVoxelHandShape(voxelHandSphere, false);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void MakeNuclearExplosion(MyEntity entity)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.UpdateFlags(entity, MyFlagsEnum.NUCLEAR_EXPLOSION);
            }

            MyNuclearExplosion.MakeExplosion(entity.GetPosition());
        }

        /// <summary>
        /// Destroys entities specified by ids, if entities are already destroyed or do not exists, nothing happens
        /// </summary>
        /// <param name="objectEntityIds"></param>
        public static void DestroyEntities(List<uint> objectEntityIds)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("DestroyEntities");
            if (objectEntityIds != null)
            {
                foreach (var entityId in objectEntityIds)
                {
                    DestroyEntity(entityId);
                }
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void DestroyEntity(uint entityId)
        {
            MyEntity entity = TryGetEntity(entityId);
            if (entity != null && entity.Physics != null)
            {
                entity.DoDamage(0, 1000000, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
            }
        }

        public static void Follow(MyEntity leader, MyEntity follower)
        {
            MySmallShipBot followerShip = follower as MySmallShipBot;
            Debug.Assert(followerShip != null);

            if (leader != null && followerShip != null)
            {
                followerShip.Follow(leader);
            }
        }

        public static void SetLight(MyEntity entity, bool enabled, bool checkEntityExist = false)
        {
            //MyPrefabLight light = entity as MyPrefabLight;
            //Debug.Assert(light != null);
            //if (light != null)
            //{
            //    light.GetLight().LightOn = enabled;
            //}
            SetEntityEnabled(entity, enabled, checkEntityExist);
        }

        public static void SetLightEffect(MyEntity entity, MyLightEffectTypeEnum effect)
        {
            MyPrefabLight light = entity as MyPrefabLight;
            Debug.Assert(light != null);
            if (light != null)
            {
                light.Effect = effect;
            }
        }

        public static void AddFalseIdToPlayersInventory(MyMwcObjectBuilder_FactionEnum faction)
        {
            var inventory = MyScriptWrapper.GetPlayerInventory();
            var falseIdItem = inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(faction));
            if (falseIdItem == null)
            {
                inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.FalseId, MyMwcFactionsByIndex.GetFactionIndex(faction), 1.0f, true);
            }
        }

        public static void AddHackingToolToPlayersInventory(int level)
        {
            Debug.Assert(level >= 1 && level <= 5);
            var inventory = MyScriptWrapper.GetCentralInventory();
            var item = inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, level);
            if (item == null)
            {
                inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, level, 1.0f, true);
            }
        }

        public static bool IsEntityControlledByPlayer(MyEntity entity)
        {
            MyPrefabLargeWeapon largeWeapon = entity as MyPrefabLargeWeapon;
            if (largeWeapon != null)
            {
                return MyGuiScreenGamePlay.Static.IsControlledByPlayer(largeWeapon);
            }
            else
            {
                return false;
            }
        }

        public static void TakeControlOfLargeWeapon(MyEntity entity)
        {
            Debug.Assert(entity != null);
            Debug.Assert(!entity.Closed);
            MyPrefabLargeWeapon largeWeapon = entity as MyPrefabLargeWeapon;
            if (largeWeapon != null)
            {
                MyGuiScreenGamePlay.Static.TakeControlOfLargeWeapon(largeWeapon);
            }
        }

        public static void DrawHealthOfCustomPrefabInLargeWeapon(MyEntity entity)
        {
            MyHud.DrawHealthOfCustomPrefabInLargeWeapon(entity);
        }

        public static void DetachHealthOfCustomPrefabInLargeWeapon()
        {
            MyHud.DetachHealthOfCustomPrefab();
        }

        public static void EnableGlobalEvent(MyGlobalEventEnum globalEvent, bool enable)
        {
            MyGlobalEvents.Enable(globalEvent, enable);
        }

        public static void SetRateForGlobalEvent(MyGlobalEventEnum globalEvent, float ratePerHour)
        {
            MyGlobalEvents.SetRatePerHour(globalEvent, ratePerHour);
        }

        public static void IncreaseHeadShake(float amount)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendHeadshake(amount);
            }

            MySession.PlayerShip.IncreaseHeadShake(amount);
        }

        public static void Log(string message, params object[] args)
        {
            string formattedMessage = String.Format(message, args);
            Debug.WriteLine(formattedMessage);
            MyMwcLog.WriteLine(formattedMessage);
        }

        public static void CloseEntity(MyEntity entity)
        {
            if (entity != null)
            {
                if (MyMultiplayerGameplay.IsHosting)
                {
                    MyMultiplayerGameplay.Static.UpdateFlags(entity, MyFlagsEnum.CLOSE);
                }

                entity.MarkForClose();
            }
        }

        public static void ForbideDetaching()
        {
            MyGuiScreenGamePlay.Static.DetachingForbidden = true;
        }

        public static void EnableDetaching()
        {
            MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static.ReleaseControlOfLargeWeapon();
            MyGuiScreenGamePlay.Static.DetachingForbidden = false;
        }

        public static void HideEntity(uint entityId, bool buffered = false)
        {
            var entity = TryGetEntity(entityId);
            HideEntity(entity, buffered);
        }

        public static void HideEntity(MyEntity entity, bool buffered = false)
        {
            Debug.Assert(entity != null);
            if (entity != null)
            {
                if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost)
                {
                    MyMultiplayerGameplay.Static.UpdateFlags(entity, CommonLIB.AppCode.Networking.Multiplayer.MyFlagsEnum.HIDE);
                }
                entity.Activate(false, buffered);
            }
        }

        public static void TryHide(uint id, bool buffered = false)
        {
            var e = TryGetEntity(id);
            if (e != null)
            {
                MyScriptWrapper.HideEntity(e, buffered);
            }
        }


        public static void TryUnhide(uint entityId, bool buffered = true, bool destroyGeneratedWaypointEdges = false)
        {
            MyEntity entity = TryGetEntity(entityId);
            if (entity != null)
            {
                UnhideEntity(entity, destroyGeneratedWaypointEdges ? false : buffered);  // if we want to destroy waypoint edges, we need to have it NOW (can't be buffered)
                if (destroyGeneratedWaypointEdges)
                    MyWayPointGraph.RemoveAllObstructedGeneratedEdgesAround(entity);
            }
        }

        public static void UnhideEntity(MyEntity entity, bool buffered = true)
        {
            if (entity != null)
            {
                if (MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsHost)
                {
                    MyMultiplayerGameplay.Static.UpdateFlags(entity, CommonLIB.AppCode.Networking.Multiplayer.MyFlagsEnum.UNHIDE);
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("UnhideEntity");
                entity.Activate(true, buffered);
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                if (entity.Name == "Madelyn")
                {
                    RegenerateWaypointGraph();
                }
            }
        }

        public static void PlayDialogue(MyDialogueEnum id)
        {
            if (MyMinerGame.IsGameReady)
            {
                PlayDialogueNow(id);
            }
            else
            {
                // When game is not ready, play dialogue after load is finished
                MyGuiScreenGamePlay.Static.OnGameReady += new MyGuiScreenBase.ScreenHandler((screen) => PlayDialogueNow(id));
            }
        }

        private static void PlayDialogueNow(MyDialogueEnum id)
        {
            MyDialogues.Play(id);

            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendPlayDialogue(id);
            }
        }

        public static void SetActorFaction(MyActorEnum actorId, MyMwcObjectBuilder_FactionEnum newFaction)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendSetActorFaction(actorId, newFaction);
            }

            MyActorConstants.SetActorFaction(actorId, newFaction);
        }

        public static void SetWaypointGroupSecrecy(string groupName, bool newSecrecy)
        {
            MyWayPointGraph.SetSecrecyForPath(groupName, newSecrecy);
        }

        public static void SetWaypointListSecrecy(List<uint> entityIds, bool newSecrecy)
        {
            var list = new List<MyWayPoint>();

            foreach (var i in entityIds)
            {
                var entity = GetEntity(i);
                var wp = entity as MyWayPoint;
                MyCommonDebugUtils.AssertDebug(wp != null, string.Format("Entity with id={0} is not a waypoint!", i));
                list.Add(wp);
            }
            MyWayPointGraph.SetSecrecyForWaypoints(list, newSecrecy);
        }

        public static void AddAudioImpShipQuake()
        {
            PlaySound2D(MySoundCuesEnum.ImpShipQuake);
        }


        public static void GetEntitiesInDummyPoint(uint dummyPointId, List<MyEntity> resultEntities)
        {
            MyDummyPoint dummyPoint = MyScriptWrapper.GetEntity(dummyPointId) as MyDummyPoint;
            Debug.Assert(dummyPoint != null, "Entity Id does not belong to dummy point");

            MyEntities.GetIntersectionsWithAABB(dummyPoint.WorldAABB, resultEntities);
        }

        public static void GetSmallShipBotsInDummyPoint(uint dummyPointId, List<MySmallShipBot> resultEntities, MyFactionRelationEnum? relation = null)
        {
            MyDummyPoint dummyPoint = MyScriptWrapper.GetEntity(dummyPointId) as MyDummyPoint;
            Debug.Assert(dummyPoint != null, "Entity Id does not belong to dummy point");

            MyEntities.GetIntersectionsWithAABBOfType<MySmallShipBot>(dummyPoint.WorldAABB, resultEntities);
            if (relation != null)
            {
                int i = 0;
                while (i < resultEntities.Count)
                {
                    MySmallShip smallShip = resultEntities[i];
                    if (MyFactions.GetFactionsRelation(smallShip.Faction, MySession.PlayerShip.Faction) != relation)
                    {
                        resultEntities.RemoveAtFast(i);
                    }
                    i++;
                }
            }
        }

        internal static bool IsEntityDead(uint entityId)
        {
            MyEntity entity;
            bool result = MyEntities.TryGetEntityById(new MyEntityIdentifier(entityId), out entity);
            if (entity == null)
                return true;
            return entity.IsDead();
        }

        internal static void DisableAllGlobalEvents()
        {
            MyGlobalEvents.DisableAllGlobalEvents();
        }

        internal static void EnableAllGlobalEvents()
        {
            MyGlobalEvents.EnableAllGlobalEvents();
        }

        internal static void SetBotAITemplate(MyEntity entity, MyAITemplateEnum template)
        {
            MySmallShipBot bot = entity as MySmallShipBot;
            Debug.Assert(bot != null);
            if (bot != null)
            {
                bot.SetAITemplate(template);
            }
        }

        internal static void ForceFindGPSPathToNextObjective()
        {
            MyHud.ShowGPSPathToNextObjective(false);
        }

        public static void SetAlarmMode(uint entityId, bool enabled)
        {
            SetAlarmMode(GetEntity(entityId), enabled);
        }

        public static void SetAlarmMode(MyEntity entity, bool enabled)
        {
            MyPrefabContainer prefabContainer = entity as MyPrefabContainer;
            Debug.Assert(prefabContainer != null);
            if (prefabContainer != null)
            {
                prefabContainer.AlarmOn = enabled;
            }
        }

        public static MySmallShip GetSpawnPointLeader(uint entityId)
        {
            return GetSpawnPointLeader(TryGetEntity(entityId));
        }

        public static MySmallShip GetSpawnPointLeader(MyEntity entity)
        {
            MySpawnPoint spawnPoint = entity as MySpawnPoint;
            Debug.Assert(spawnPoint != null);
            return spawnPoint.GetLeader();
        }

        public static void StopFollow(uint followerId)
        {
            StopFollow(TryGetEntity(followerId));
        }

        public static void StopFollow(MyEntity bot)
        {
            MySmallShipBot smallShipBot = bot as MySmallShipBot;
            Debug.Assert(smallShipBot != null);

            if (smallShipBot != null)
            {
                if (smallShipBot.Leader != null)
                {
                    smallShipBot.StopFollow();
                }
            }
        }

        public static void Move(MyEntity entity, MyEntity target)
        {
            Debug.Assert(entity != null);
            Debug.Assert(target != null);
            Move(entity, target.GetPosition(), target.GetForward(), target.GetUp());
        }

        public static void Move(MyEntity entity, Vector3 position, Vector3? forward = null, Vector3? up = null)
        {
            Debug.Assert(entity != null);
            Matrix newWorld = Matrix.CreateWorld(position, forward ?? entity.WorldMatrix.Forward, up ?? entity.WorldMatrix.Up);
            entity.SetWorldMatrix(newWorld);

            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.UpdatePositionFast(entity, position/*, forward, up*/); //TODO: why is not forward and up transmitted?
            }
        }

        public static void MoveWithVelocity(MyEntity entity, Vector3 position, Vector3 velocity)
        {
            Debug.Assert(entity != null);
            Matrix newWorld = Matrix.CreateWorld(position, entity.WorldMatrix.Forward, entity.WorldMatrix.Up);
            entity.SetWorldMatrix(newWorld);

            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.UpdatePosition(entity, entity.WorldMatrix, velocity, Vector3.Zero);
            }
        }

        public static void TravelToMission(MyMissionID missionID)
        {
            MyMission mission = (MyMission)MyMissions.GetMissionByID(missionID);
            Debug.Assert(mission.Location != null);

            MyGuiScreenGamePlay.Static.TravelToSector(new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.STORY, null, mission.Location.Sector, ""), MyMwcTravelTypeEnum.SOLAR, MySession.PlayerShip.GetPosition(), mission.ID);
        }

        public static bool IsMissionFinished(MyMissionID missionID)
        {
            return MySession.Static.EventLog.IsMissionFinished(missionID);
        }

        public static bool AreMissionFinished(MyMissionID[] missionIDs)
        {
            return MySession.Static.EventLog.AreMissionsFinished(missionIDs);
        }

        public static void SetSleepDistance(MyEntity botEntity, float sleepDistance)
        {
            MySmallShipBot bot = botEntity as MySmallShipBot;
            Debug.Assert(bot != null);

            if (bot != null)
            {
                bot.SleepDistance = sleepDistance;
            }

        }

        public static void SetBotTarget(MyEntity bot, MyEntity target)
        {
            MySmallShipBot smallShipBot = bot as MySmallShipBot;
            Debug.Assert(smallShipBot != null);

            if (smallShipBot != null)
            {
                smallShipBot.Attack(target);
            }
        }

        public static MyEntity GetMothershipHangar(MyEntity mothership)
        {
            foreach (MyPrefabBase prefab in ((MyPrefabContainer)mothership).GetPrefabs())
            {
                if (prefab is MyPrefabHangar)
                {
                    return prefab;
                }
            }

            return null;
        }

        public static void Highlight(uint id, bool hightlight, MyMissionBase misson)
        {
            if (hightlight)
            {
                bool hasComponent = false;
                foreach (var components in misson.Components)
                {
                    if (components is MyBlinkingObjects)
                    {
                        var blinkingObjects = components as MyBlinkingObjects;
                        blinkingObjects.AddBlinkingObject(TryGetEntity(id));
                        hasComponent = true;
                        break;
                    }
                }
                if (!hasComponent)
                {
                    var blink = new MyBlinkingObjects(new List<uint>());
                    blink.AddBlinkingObject(TryGetEntity(id));
                    misson.Components.Add(blink);
                }
            }
            else
            {
                foreach (var components in misson.Components)
                {
                    if (components is MyBlinkingObjects)
                    {
                        var blinkingObjects = components as MyBlinkingObjects;
                        MyEntity entity = MyScriptWrapper.TryGetEntity(id);
                        if (entity != null)
                        {
                            blinkingObjects.RemoveBlinkingObject(entity);
                        }
                        break;
                    }
                }
            }
        }



        public static void EnablePhysics(uint id, bool enable, bool ignoreLargeWeapons = false)
        {
            var entity = MyScriptWrapper.GetEntity(id);
            Debug.Assert(entity != null);
            if (entity != null)
            {
                EnablePhysicsRecursive(entity, enable, ignoreLargeWeapons);
            }
        }

        private static void EnablePhysicsRecursive(MyEntity entity, bool enable, bool ignoreLargeWeapons = false)
        {
            if (ignoreLargeWeapons && entity is MyPrefabLargeWeapon)
                return;

            if (entity.Physics != null && entity.Physics.Enabled != enable)
                entity.Physics.Enabled = enable;

            foreach (var myEntity in entity.Children)
            {
                EnablePhysicsRecursive(myEntity, enable);
            }
        }

        /// <summary>
        /// Disable ship - turn off lights and engine, disable AI
        /// </summary>
        public static void DisableShip(MyEntity entity, bool staticPhysics = true, bool destructible = true)
        {
            MySmallShip ship = entity as MySmallShip;
            if (ship != null)
            {
                if (MyMultiplayerGameplay.IsHosting)
                {
                    MyFlagsEnum flags = MyFlagsEnum.PARK_SHIP | (destructible ? MyFlagsEnum.DESTRUCTIBLE : MyFlagsEnum.INDESTRUCTIBLE);
                    MyMultiplayerGameplay.Static.UpdateFlags(entity, MyFlagsEnum.PARK_SHIP, staticPhysics);
                }

                ship.SetParked(staticPhysics);
                ship.IsDestructible = destructible;
            }
        }

        public static event Action FadedIn
        {
            add { MyGuiScreenGamePlay.Static.FadedIn += value; }
            remove { MyGuiScreenGamePlay.Static.FadedIn -= value; }
        }

        public static event Action FadedOut
        {
            add { MyGuiScreenGamePlay.Static.FadedOut += value; }
            remove { MyGuiScreenGamePlay.Static.FadedOut -= value; }
        }

        public static void FadeIn(float speed = 0.02f)
        {
            MyGuiScreenGamePlay.Static.FadeIn(speed);
        }

        public static void FadeOut(float speed = 0.02f)
        {
            MyGuiScreenGamePlay.Static.FadeOut(speed);
        }

        public static void SetEntityDisplayName(MyEntity entity, string displayName)
        {
            Debug.Assert(entity != null);
            entity.DisplayName = displayName;
        }

        public static void SetEntityDisplayName(uint entityId, string displayName)
        {
            SetEntityDisplayName(GetEntity(entityId), displayName);
        }

        public static void GenerateMinesField<T>(MyEntity en, MyMwcObjectBuilder_FactionEnum faction, int count, string name, MyHudIndicatorFlagsEnum hudparams) where T : MyMineBase, IUniversalLauncherShell, new()
        {
            //if (! is MyMineBase) 
            en.Faction = MyMwcObjectBuilder_FactionEnum.Russian_KGB;
            var radius = en.WorldVolume.Radius;
            for (int i = 0; i < count; i++)
            {
                Vector3 vec = new Vector3();
                while (true)
                {
                    vec.X = MyMwcUtils.GetRandomFloat(-radius, radius);
                    vec.Y = MyMwcUtils.GetRandomFloat(-radius, radius);
                    vec.Z = MyMwcUtils.GetRandomFloat(-radius, radius);
                    if (vec.Length() <= radius) break;
                }

                MyMineBase mine = MyUniversalLauncherShells.Allocate<T>(en.EntityId.Value.PlayerId);
                if (mine != null)
                {
                    mine.Init();
                    mine.Start(en.GetPosition() + vec, Vector3.Zero, Vector3.One, en.EntityId.Value.PlayerId, en);
                    var relation = MyFactions.GetFactionsRelation(faction, MySession.PlayerShip.Faction);

                    var mode = MyGuitargetMode.Neutral;

                    switch (relation)
                    {
                        case MyFactionRelationEnum.Enemy: mode = MyGuitargetMode.Enemy;
                            break;
                        case MyFactionRelationEnum.Friend: mode = MyGuitargetMode.Friend;
                            break;
                        default:
                            break;
                    }


                    if (hudparams != MyHudIndicatorFlagsEnum.NONE)
                    {
                        MyScriptWrapper.MarkEntity(mine, name, hudparams, mode);
                    }
                }
                else
                {

                }
            }
        }

        public static void DisableShipBackCamera()
        {
            MyGuiScreenGamePlay.Static.DisableBackCamera = true;
        }

        public static void EnableShipBackCamera()
        {
            MyGuiScreenGamePlay.Static.DisableBackCamera = false;
        }


        internal static void SetCargoRespawn(uint cargobox, TimeSpan respawnTime)
        {
            var e = GetEntity(cargobox) as MyCargoBox;
            Debug.Assert(e != null);
            e.RespawnTime = respawnTime;
        }

        public static void SetEntitySaveFlagDisabled(MyEntity entity)
        {
            if (entity != null) entity.Save = false;
        }


        public static void KillAllEnemy()
        {
            // We need player ship to recognize friends
            if (MySession.PlayerShip == null)
            {
                return;
            }

            foreach (var entity in MyEntities.GetEntities().ToArray())
            {
                MySmallShip smallShip = entity as MySmallShip;
                if (smallShip != null &&
                    smallShip.Visible &&
                    smallShip != MySession.PlayerShip &&
                    MyFactions.GetFactionsRelation(MySession.PlayerShip, smallShip) == MyFactionRelationEnum.Enemy)
                {
                    entity.DoDamage(0, 1000000, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
                }

                MyPrefabContainer container = entity as MyPrefabContainer;
                if (container != null &&
                    MyFactions.GetFactionsRelation(MySession.PlayerShip, container) == MyFactionRelationEnum.Enemy)
                {
                    foreach (var prefab in container.GetPrefabs().ToArray())
                    {
                        MyPrefabLargeWeapon largeWeapon = prefab as MyPrefabLargeWeapon;
                        if (largeWeapon != null)
                        {
                            prefab.DoDamage(0, 1000000, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
                        }
                    }
                }
            }
        }

        //TODO: this is used in rift and will not work in multiplayer
        public static MyEntity GenerateMeteor(float size, Vector3 position, MyMwcVoxelMaterialsEnum material, Vector3 velocity, MyParticleEffectsIDEnum? trailEffect)
        {
            Matrix worldMatrix = Matrix.CreateFromAxisAngle(MyMwcUtils.GetRandomVector3Normalized(), MyMwcUtils.GetRandomFloat(0, MathHelper.Pi));
            worldMatrix.Translation = position;

            MyMeteor meteor = MyMeteor.GenerateMeteor(size, worldMatrix, position, material);

            meteor.Start(velocity, (int?)trailEffect);

            return meteor;
        }



        public static void ChangeShip(MySmallShip changeForShip)
        {
            MyMwcObjectBuilder_SmallShip_Player originalBuilder = MySession.PlayerShip.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Player;
            var originalShipType = originalBuilder.ShipType;
            var originalPosition = MySession.PlayerShip.WorldMatrix;

            MySession.PlayerShip.Physics.Clear();
            MySession.PlayerShip.MarkForClose();

            //creating new Player ship
            MyMwcObjectBuilder_SmallShip changeForShipOB = (MyMwcObjectBuilder_SmallShip)changeForShip.GetObjectBuilder(true);
            MyMwcObjectBuilder_SmallShip_Player newPlayerShipBuilder = new MyMwcObjectBuilder_SmallShip_Player(changeForShipOB.ShipType,
                changeForShipOB.Inventory,
                changeForShipOB.Weapons,
                changeForShipOB.Engine,
                changeForShipOB.AssignmentOfAmmo,
                changeForShipOB.Armor,
                changeForShipOB.Radar,
                changeForShipOB.ShipMaxHealth,
                changeForShipOB.ShipHealthRatio,
                changeForShipOB.ArmorHealth,
                100,
                changeForShipOB.Oxygen,
                changeForShipOB.Fuel,
                changeForShipOB.ReflectorLight,
                changeForShipOB.ReflectorLongRange,
                changeForShipOB.ReflectorShadowDistance,
                changeForShipOB.AIPriority);
            newPlayerShipBuilder.Faction = MySession.PlayerShip.Faction;
                
/*
            originalBuilder.ShipType = changeForShip.ShipType;
            MyEntities.CreateFromObjectBuilderAndAdd(null, originalBuilder, changeForShip.WorldMatrix);
                   */

            MyEntities.CreateFromObjectBuilderAndAdd(null, newPlayerShipBuilder, changeForShip.WorldMatrix);

            //adding old ship to inventory - garage
            var playerShipInventory = MyInventory.CreateInventoryItemFromObjectBuilder(originalBuilder);
            MySession.Static.Inventory.AddInventoryItem(playerShipInventory);
            changeForShip.MarkForClose();


            MyMwcObjectBuilder_SmallShip fakeShipBuilder = new MyMwcObjectBuilder_SmallShip(
                originalShipType,
                null,
                originalBuilder.Weapons,
                originalBuilder.Engine,
                originalBuilder.AssignmentOfAmmo,
                originalBuilder.Armor,
                originalBuilder.Radar,
                originalBuilder.ShipMaxHealth,
                originalBuilder.ShipHealthRatio,
                originalBuilder.ArmorHealth,
                originalBuilder.Oxygen,
                originalBuilder.Fuel,
                originalBuilder.ReflectorLight,
                originalBuilder.ReflectorLongRange,
                originalBuilder.ReflectorShadowDistance,
                originalBuilder.AIPriority);
            fakeShipBuilder.Faction = originalBuilder.Faction;

            //creating new ship, replacement of mine old ship
            fakeShipBuilder.EntityId = MyEntityIdentifier.AllocateId().NumericValue;
            fakeShipBuilder.PositionAndOrientation = new MyMwcPositionAndOrientation(originalPosition);
            var myFakeShip = new MySmallShip();
            myFakeShip.Init(null, fakeShipBuilder);
            MyEntities.Add(myFakeShip);
            DisableShip(myFakeShip);
        }

        public static void RevertShipFromInventory()
        {
            if (MySession.PlayerShip.ShipType == MyMwcObjectBuilder_SmallShip_TypesEnum.VIRGINIA)
                return;

            MyMwcObjectBuilder_SmallShip_Player originalBuilder = MySession.PlayerShip.GetObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Player;

            var originalShipType = originalBuilder.ShipType;
            var originalPosition = MySession.PlayerShip.WorldMatrix;

            MyInventoryItem playerSmallShip = null;
            foreach (var item in MySession.Static.Inventory.GetInventoryItems())
            {
                if (item.ObjectBuilderType == MyMwcObjectBuilderTypeEnum.SmallShip_Player)
                {   //We have found stored player ship

                    MySession.PlayerShip.Physics.Clear();
                    MySession.PlayerShip.MarkForClose();

                    playerSmallShip = item;

                    var builder = item.GetInventoryItemObjectBuilder(true) as MyMwcObjectBuilder_SmallShip_Player;
                    MyEntities.CreateFromObjectBuilderAndAdd(null, builder, MySession.PlayerShip.WorldMatrix);

                    break;
                }
            }

            if (playerSmallShip == null)
                return; //already changed



            //backing up current ship
            MyMwcObjectBuilder_SmallShip_Player inventoryShipBuilder = new MyMwcObjectBuilder_SmallShip_Player(originalBuilder.ShipType,
                originalBuilder.Inventory,
                originalBuilder.Weapons,
                originalBuilder.Engine,
                originalBuilder.AssignmentOfAmmo,
                originalBuilder.Armor,
                originalBuilder.Radar,
                originalBuilder.ShipMaxHealth,
                originalBuilder.ShipHealthRatio,
                originalBuilder.ArmorHealth,
                100,
                originalBuilder.Oxygen,
                originalBuilder.Fuel,
                originalBuilder.ReflectorLight,
                originalBuilder.ReflectorLongRange,
                originalBuilder.ReflectorShadowDistance,
                originalBuilder.AIPriority);
            inventoryShipBuilder.Faction = originalBuilder.Faction;

            //adding old ship to inventory - garage
            var playerShipInventory = MyInventory.CreateInventoryItemFromObjectBuilder(inventoryShipBuilder);
            
            MySession.Static.Inventory.AddInventoryItem(playerShipInventory);

            if (playerSmallShip != null)
            {
                MySession.Static.Inventory.RemoveInventoryItem(playerSmallShip);
            }
        }


        public static MySmallShipBot InsertFriend(MyActorEnum actorEnum, MyMwcObjectBuilder_SmallShip_TypesEnum? shipType = null)
        {
            return MyMissionBase.InsertFriend(actorEnum, shipType);
        }

        public static void TryHideEntities(List<uint> toHide, bool buffered = false)
        {
            foreach (var u in toHide)
            {
                MyScriptWrapper.TryHide(u, buffered);
            }
        }

        public static void HideEntities(List<uint> toHide, bool buffered = false)
        {
            foreach (var u in toHide)
            {
                MyScriptWrapper.HideEntity(u, buffered);
            }
        }

        public static void HideEntities(List<MyEntity> toHide, bool buffered = false)
        {
            foreach (var u in toHide)
            {
                MyScriptWrapper.HideEntity(u, buffered);
            }
        }

        public static void TryUnhideEntities(List<uint> toHide, bool buffered = false)
        {
            foreach (var u in toHide)
            {
                MyScriptWrapper.TryUnhide(u, buffered);
            }
        }

        public static void UnhideEntities(List<MyEntity> toUnhide, bool buffered = false)
        {
            foreach (var u in toUnhide)
            {
                MyScriptWrapper.UnhideEntity(u, buffered);
            }
        }

        public static void DestroyPlayerShip()
        {
            MySession.PlayerShip.DoDamage(0, 1000000, 0, MyDamageType.Unknown, MyAmmoType.Unknown, null);
        }

        public static void AllowMusic(bool allow)
        {
            MyAudio.MusicAllowed = allow;
        }

        public static void MovePlayerAndFriendsToHangar(MyActorEnum[] requiredActors)
        {
            MyMissionBase.MovePlayerAndFriendsToHangar(requiredActors);
        }

        public static void ChangeFaction(MyEntity entity, MyMwcObjectBuilder_FactionEnum newFaction)
        {
            if (entity != null)
            {
                Debug.Assert(entity.EntityId.HasValue);
                ChangeFaction(entity.EntityId.Value.NumericValue, newFaction);
            }
        }

        public static void ChangeFaction(uint entityId, MyMwcObjectBuilder_FactionEnum newFaction)
        {
            var entity = GetEntity(entityId);
            if (entity != null)
            {
                if (MyMultiplayerGameplay.IsHosting)
                {
                    MyMultiplayerGameplay.Static.SendSetEntityFaction(entity, newFaction);
                }

                entity.Faction = newFaction;
            }
        }

        public static void RegenerateWaypointGraph()
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendGlobalFlag(MyGlobalFlagsEnum.REGENERATE_WAYPOINTS);
            }

            MyWayPointGraph.RecreateWaypointsAroundMadelyn();
        }

        public static void Refill()
        {
            MySession.Static.Player.RestoreHealth();
            MySession.PlayerShip.Refill();
        }

        public static void SetLargeWeaponRotation(uint entityId, float angle, float elevation)
        {
            MyPrefabLargeWeapon weapon = GetEntity(entityId) as MyPrefabLargeWeapon;
            Debug.Assert(weapon != null);
            if (weapon != null)
            {
                weapon.SetRotation(angle, elevation);
            }
        }

        [Conditional("DEBUG")]
        internal static void DebugSetFactions(MyMission mission)
        {
            var currentMissionNumber = MyMissions.GetMissionNumber(mission.ID);
            if (currentMissionNumber == -1)
                return;

            MyFactions.SetDefaultFactionRelations();
            if (MyMissions.GetMissionNumber(MyMissionID.PIRATE_BASE) < currentMissionNumber) //4
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.RUSSIAN_WAREHOUSE) < currentMissionNumber) //5
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_BEST);
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Pirates, MyMwcObjectBuilder_FactionEnum.Russian, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.LAST_HOPE) < currentMissionNumber) //7
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Slavers, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.CHINESE_ESCAPE) < currentMissionNumber) //8d
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.China, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.RESEARCH_VESSEL) < currentMissionNumber) //12
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.JUNKYARD_EAC_AMBUSH) < currentMissionNumber) //13
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.RUSSIAN_TRANSMITTER) < currentMissionNumber) // 17
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Russian, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.REICHSTAG_A) < currentMissionNumber) //18a
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_NEUTRAL);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.NAZI_BIO_LAB) < currentMissionNumber) //18b
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.REICHSTAG_C) < currentMissionNumber) //18c
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_BEST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.TWIN_TOWERS) < currentMissionNumber) //19
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_BEST);
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.WhiteWolves, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.EAC_PRISON) < currentMissionNumber) //20
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.FourthReich, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_BEST);
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }
            if (MyMissions.GetMissionNumber(MyMissionID.EAC_TRANSMITTER) < currentMissionNumber) //21
            {
                MyScriptWrapper.SetFactionRelation(MyMwcObjectBuilder_FactionEnum.Euroamerican, MyMwcObjectBuilder_FactionEnum.Rainiers, MyFactions.RELATION_WORST);
            }

        }

        public static float GetGameVolume()
        {
            return MyAudio.VolumeGame;
        }

        public static void SetGameVolume(float volume)
        {
            MyAudio.VolumeGame = volume;
        }

        public static void SetGameVolumeExceptDialogues(float volume)
        {
            MyAudio.SetVolumeExceptDialogues(volume);
        }


        internal static void AddMoney(int money)
        {
            var notification = MyScriptWrapper.CreateNotification(MyTextsWrapperEnum.MoneyObtained, MyGuiManager.GetFontMinerWarsBlue(), 5000);
            notification.SetTextFormatArguments(new object[] { money.ToString() });
            MySession.Static.Player.Money += money;
            MyScriptWrapper.AddNotification(notification);
            notification.Appear();
        }
    }



    public class CancelEventArgs
    {
        public bool Cancel { get; set; }
    }
}
