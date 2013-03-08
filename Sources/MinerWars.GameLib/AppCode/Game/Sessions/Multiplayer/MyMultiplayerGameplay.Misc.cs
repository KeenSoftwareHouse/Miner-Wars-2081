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
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Renders;
using System.Net;

namespace MinerWars.AppCode.Game.Sessions
{
    partial class MyMultiplayerGameplay
    {
        #region CHAT
        public void SendChatMessageToTeam(string message, MyMwcObjectBuilder_FactionEnum? senderFaction)
        {
            SendChatMessage(message, senderFaction);
        }

        public void SendChatMessage(string message)
        {
            SendChatMessage(message, null);
        }

        private void SendChatMessage(string message, MyMwcObjectBuilder_FactionEnum? senderFaction)
        {
            MyEventChat msg = new MyEventChat();
            msg.Message = message;
            if (senderFaction.HasValue)
            {
                Peers.SendToTeam(ref msg, senderFaction.Value, NetDeliveryMethod.ReliableUnordered);
            }
            else
            {
                Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableUnordered);
            }
        }

        void OnChatMessage(ref MyEventChat msg)
        {
            var player = (MyPlayerRemote)msg.SenderConnection.Tag;
            if (player != null)
            {
                MyGuiScreenGamePlay.Static.AddChatMessage(player.UserId, msg.Message);
            }
        }
        #endregion

        #region STATS
        public void UpdateStats()
        {
            MyEventStatsUpdate msg = new MyEventStatsUpdate();
            msg.StatsBuilder = PlayerStatistics.GetObjectBuilder();
            Peers.SendToAll(ref msg, NetDeliveryMethod.Unreliable);
        }

        void OnStatsUpdate(ref MyEventStatsUpdate msg)
        {
            MyPlayerRemote sender = (MyPlayerRemote)msg.SenderConnection.Tag;
            if (sender != null)
            {
                sender.Statistics = new MyPlayerStatistics();
                sender.Statistics.Init(msg.StatsBuilder);
            }
        }
        #endregion

        #region FACTION
        public void SendFaction(MyMwcObjectBuilder_FactionEnum faction)
        {
            Log("Set faction: " + MyFactionConstants.GetFactionProperties(faction).Name);
            var factionMsg = new MyEventSetFaction();
            factionMsg.Faction = faction;
            Peers.SendToAll(ref factionMsg, NetDeliveryMethod.ReliableOrdered);
        }

        public static void OnSetFaction(ref MyEventSetFaction msg)
        {
            Log("On set faction for : " + msg.SenderConnection.GetPlayerName() + ", faction: " + MyFactionConstants.GetFactionProperties(msg.Faction).Name);

            // Change only player faction, his ship can't change faction until respawn
            var player = (MyPlayerRemote)msg.SenderConnection.Tag;
            player.Faction = msg.Faction;

            if (IsStory())
            {
                MySession.Static.Player.Faction = msg.Faction;
            }
        }

        public void RequestFaction(MyMwcObjectBuilder_FactionEnum preferredFaction)
        {
            Log("Choose faction");

            var msg = new MyEventChooseFaction();
            msg.PreferredFaction = preferredFaction;
            Peers.SendHost(ref msg);
        }

        private void OnChooseFaction(ref MyEventChooseFaction msg)
        {
            var response = new MyEventChooseFactionResponse();
            response.AssignedFaction = ChooseFaction(msg.PreferredFaction);

            Log("On choose faction for: " + msg.SenderConnection.GetPlayerName() + ", chosen faction: " + MyFactionConstants.GetFactionProperties(response.AssignedFaction).Name);

            Peers.NetworkClient.Send(ref response, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void OnChooseFactionResponse(ref MyEventChooseFactionResponse msg)
        {
            Log("On choose faction response, faction: " + MyFactionConstants.GetFactionProperties(msg.AssignedFaction).Name);

            //            if (MySession.Static.Player.Faction != msg.AssignedFaction)
            {
                MySession.Static.Player.Faction = msg.AssignedFaction;
                SendFaction(msg.AssignedFaction);
            }
        }
        #endregion

        #region NOTIFICATIONS
        void OnNotify(ref MyEventNotification msg)
        {
            var handler = OnNotification;
            if (handler != null)
            {
                handler(msg.Type, (MyTextsWrapperEnum)msg.Text, new object[] { msg.Arg0, msg.Arg1, msg.Arg2, msg.Arg3 });
            }
        }

        public void SendNotification(MyNotificationType type, MyTextsWrapperEnum text, string arg0 = null, string arg1 = null, string arg2 = null, string arg3 = null)
        {
            var msg = new MyEventNotification();
            msg.Text = (int)text;
            msg.Type = type;
            msg.Arg0 = arg0;
            msg.Arg1 = arg1;
            msg.Arg2 = arg2;
            msg.Arg3 = arg3;
            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }
        #endregion

        #region CUT_OUT
        public void CutOut(MyVoxelMap voxelMap, ref BoundingSphere cutOutSphere)
        {
            Debug.Assert(voxelMap.EntityId.HasValue);

            var msg = new MyEventCutOut();
            msg.Position = cutOutSphere.Center;
            msg.Radius = cutOutSphere.Radius;
            msg.VoxelMapEntityId = voxelMap.EntityId.Value.NumericValue;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        void OnCutOut(ref MyEventCutOut msg)
        {
            MyVoxelMap voxelMap;
            if (MyEntities.TryGetEntityById<MyVoxelMap>(msg.VoxelMapEntityId.ToEntityId(), out voxelMap))
            {
                var sphere = new BoundingSphere(msg.Position, msg.Radius);

                //remove decals
                MyDecals.HideTrianglesAfterExplosion(voxelMap, ref sphere);

                //cut off 
                MyVoxelGenerator.CutOutSphereFast(voxelMap, sphere);
            }
        }
        #endregion

        #region FLAGS
        public void UpdateFlags(MyEntity entity, MyFlagsEnum flag, bool param = false)
        {
            Debug.Assert(entity.EntityId.HasValue, "Entity ID must have value!");
            LogDevelop("UPDATE FLAGS");

            if (!entity.EntityId.HasValue) return; // Nothing to report

            var msg = new MyEventFlags();
            msg.Flag = flag;
            msg.EntityId = entity.EntityId.Value.NumericValue;
            msg.Param = param;

            if (IsHost)
            {
                Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0);
            }
            else
            {
                Peers.SendHost(ref msg);
            }
        }

        void OnUpdateFlags(ref MyEventFlags msg)
        {
            LogDevelop("ON FLAGS");

            MyEntity entity;
            if (MyEntities.TryGetEntityById(msg.EntityId.ToEntityId(), out entity))
            {
                var old = entity.IsDummy;
                entity.IsDummy = true;
                ProcessFlag(entity, msg.Flag, msg.Param);
                entity.IsDummy = old;
            }

            if (IsHost)
            {
                Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        void ProcessFlag(MyEntity entity, MyFlagsEnum flag, bool param)
        {
            switch (flag)
            {
                case MyFlagsEnum.ENABLE:
                    entity.Enabled = true;
                    break;

                case MyFlagsEnum.DISABLE:
                    entity.Enabled = false;
                    break;

                case MyFlagsEnum.HIDE:
                    entity.Activate(false, param);
                    break;

                case MyFlagsEnum.UNHIDE:
                    entity.Activate(true, param);
                    break;

                case MyFlagsEnum.PARK_SHIP:
                    {
                        var ship = entity as MySmallShip;
                        if (ship != null)
                        {
                            ship.SetParked(param);
                        }
                    }
                    break;

                case MyFlagsEnum.CLOSE:
                    MyScriptWrapper.CloseEntity(entity);
                    break;

                case MyFlagsEnum.PARTICLE:
                    MyScriptWrapper.SetParticleEffect(entity, param);
                    break;

                case MyFlagsEnum.NUCLEAR_EXPLOSION:
                    MyScriptWrapper.MakeNuclearExplosion(entity);
                    break;

                case MyFlagsEnum.INDESTRUCTIBLE:
                    entity.IsDestructible = false;
                    break;

                case MyFlagsEnum.DESTRUCTIBLE:
                    entity.IsDestructible = true;
                    break;

                case MyFlagsEnum.PREPARE_MOVE:
                    MyScriptWrapper.PrepareMotherShipForMove(entity);
                    break;

                case MyFlagsEnum.RETURN_FROM_MOVE:
                    MyScriptWrapper.ReturnMotherShipFromMove(entity);
                    break;

                default:
                    Debug.Fail("Unknown flag");
                    break;
            }
        }
        #endregion

        #region NewEntity
        public void NewEntity(MyMwcObjectBuilder_Base objectBuilder, Matrix matrix)
        {
            Debug.Assert(objectBuilder.EntityId.HasValue, "EntityId must be set");

            var msg = new MyEventNewEntity();
            msg.ObjectBuilder = objectBuilder;
            msg.Position = new MyMwcPositionAndOrientation(matrix);
            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0, DEFAULT_LARGE_MESSAGE_SIZE);
        }

        void OnNewEntity(ref MyEventNewEntity msg)
        {
            var entityId = msg.ObjectBuilder.EntityId.ToEntityId();
            if (entityId.HasValue && MyEntities.GetEntityByIdOrNull(entityId.Value) != null)
            {
                return;
            }

            var entity = MyEntities.CreateFromObjectBuilderAndAdd(null, msg.ObjectBuilder, msg.Position.GetMatrix());
            HookEntity(entity);
        }
        #endregion

        #region MISSION_VARS
        public void UpdateMissionVars()
        {
            if (MyMissions.ActiveMission != null)
            {
                foreach (var objective in MyMissions.ActiveMission.ActiveObjectives)
                {
                    // Objective timer, update once per 3s
                    var msg = new MyEventMissionUpdateVars();
                    msg.ElapsedTime = (int)objective.MissionTimer.GetElapsedTime().TotalMilliseconds;
                    msg.MissionId = (int)MyMissions.ActiveMission.ID;
                    Peers.SendToAll(ref msg, Multiplayer.MyUpdateTypeId.Mission, (uint)objective.ID, 0.33f, NetDeliveryMethod.ReliableOrdered);
                }

                // Mission timer, update once per 10s
                var msgMission = new MyEventMissionUpdateVars();
                msgMission.ElapsedTime = (int)MyMissions.ActiveMission.MissionTimer.GetElapsedTime().TotalMilliseconds;
                msgMission.MissionId = (int)MyMissions.ActiveMission.ID;
                Peers.SendToAll(ref msgMission, Multiplayer.MyUpdateTypeId.Mission, (uint)MyMissions.ActiveMission.ID, 0.1f, NetDeliveryMethod.ReliableOrdered);
            }
        }

        void OnMissionUpdateVars(ref MyEventMissionUpdateVars msg)
        {
            var missionId = (MyMissionID)msg.MissionId;
            if (!MyMwcEnums.IsValidValue<MyMissionID>(missionId))
            {
                Alert("Invalid mission id!", msg.SenderEndpoint, msg.EventType);
                return;
            }

            var mission = MyMissions.GetMissionByID((MyMissionID)msg.MissionId);
            mission.MissionTimer.SetElapsedTime(TimeSpan.FromMilliseconds(msg.ElapsedTime));
        }
        #endregion

        #region COUNTDOWN
        public void SendCountdown(TimeSpan countdown, bool forceSend = false)
        {
            MyEventCountdown msg = new MyEventCountdown();
            msg.Timespan = countdown;

            if (forceSend)
            {
                Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
            }
            else
            {
                Peers.SendToAll(ref msg, Multiplayer.MyUpdateTypeId.MissionTimer, 0, 1.2f, NetDeliveryMethod.ReliableOrdered);
            }
        }

        MyHudNotification.MyNotification m_countdownNotification;

        void OnCountdown(ref MyEventCountdown msg)
        {
            ClearCountdownNotification();
            if (msg.Timespan.Ticks > 0)
            {
                m_countdownNotification = new MyHudNotification.MyNotification(MyTextsWrapperEnum.Countdown, MyGuiManager.GetFontMinerWarsBlue());
                m_countdownNotification.SetTextFormatArguments(new object[] { String.Format("{0:00}", msg.Timespan.Minutes) + ":" + String.Format("{0:00}", msg.Timespan.Seconds) });
                MyHudNotification.AddNotification(m_countdownNotification);
            }
        }

        void ClearCountdownNotification()
        {
            if (m_countdownNotification != null)
            {
                m_countdownNotification.Disappear();
                m_countdownNotification = null;
            }
        }
        #endregion

        #region FRIENDLY_FIRE

        public void FriendlyFire(MyFriendlyFireEnum type)
        {
            var msg = new MyEventFriendlyFire();
            msg.FriendlyFireType = type;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        void OnFriendlyFire(ref MyEventFriendlyFire msg)
        {
            switch(msg.FriendlyFireType)
            {
                case MyFriendlyFireEnum.AGGRO:
                    MyFriendlyFire.MakeEnemy(MySession.Static.Player.Faction);
                    MyFriendlyFire.StartGameoverTimer();
                    break;

                case MyFriendlyFireEnum.GAME_FAILED:
                    MyFriendlyFire.Fail();
                    break;

                default:
                    Alert("Unknown friendly fire type", msg.SenderEndpoint, msg.EventType);
                    break;
            }
        }

        #endregion

        #region EVENTS

        public void SendEvent(Vector3 position, MyGlobalEventEnum eventType, int seed)
        {
            SendEvent(position, eventType, seed, Vector3.Up, Vector3.Forward);
        }

        public void SendEvent(Vector3 position, MyGlobalEventEnum eventType, int seed, Vector3 up, Vector3 forward)
        {
            var msg = new MyEventEvent();
            msg.Position = new MyMwcPositionAndOrientation(position, forward, up);
            msg.EventTypeEnum = (int)eventType;
            msg.Seed = seed;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        void OnEvent(ref MyEventEvent msg)
        {
            var eventType = (MyGlobalEventEnum)msg.EventTypeEnum;
            if (!MyMwcEnums.IsValidValue(eventType))
            {
                Alert("Invalid global event type", msg.SenderEndpoint, msg.EventType);
            }

            // TODO: When required, add position and seed
            MyGlobalEvents.StartGlobalEvent(eventType);
        }

        #endregion

        #region GLOBAL_FLAGS
        public void SendGlobalFlag(MyGlobalFlagsEnum eventFlag)
        {
            var msg = new MyEventGlobalFlag();
            msg.Flag = eventFlag;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        private void OnGlobalFlag(ref MyEventGlobalFlag msg)
        {
            switch(msg.Flag)
            {
                case MyGlobalFlagsEnum.REGENERATE_WAYPOINTS:
                    MyScriptWrapper.RegenerateWaypointGraph();
                    break;

                case MyGlobalFlagsEnum.REQUEST_INFO:
                    if(MySession.Static != null && MySession.Static.Player.Faction != MyMwcObjectBuilder_FactionEnum.None)
                    {
                        SendFaction(MySession.Static.Player.Faction);
                        UpdateStats();
                    }
                    break;

                default:
                    Alert("Unknown global flag", msg.SenderEndpoint, msg.EventType);
                    break;
            }
        }
        #endregion
    }
}
