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
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Sessions
{

    partial class MyMultiplayerGameplay
    {
        public void UpdateCoopTarget()
        {
            if (IsHost)
                return;

            MyPlayerRemote host;
            if(Peers.TryGetPlayer(Peers.HostUserId, out host))
            {
                var ship = host.Ship;
                if (IsStory() && ship != null && m_followMission != null)
                {
                    m_followMission.MainObjective.Location = new MyMissionBase.MyMissionLocation(MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position, ship.EntityId.Value.NumericValue);
                    m_followMission.MainObjective.SetLocationVisibility(true);
                }
            }
        }

        public void UpdateMission()
        {
            if (IsHost && IsStory())
            {
                if (MyMissions.ActiveMission != null && MyMissions.ActiveMission.ActiveObjectives.Count > 0)
                {
                    SendMissionProgress(MyMissions.ActiveMission.ActiveObjectives[0], MyMissionProgressType.NewObjective);
                }
                else
                {
                    SendMissionProgress(null, MyMissionProgressType.NewObjective);
                }
            }
        }

        public void SendMissionProgress(MyMissionBase mission, MyMissionProgressType progressType, MyTextsWrapperEnum? messageEnum = null)
        {
            MyEventMissionProgress message = new MyEventMissionProgress();
            message.MissionId = mission != null ? (int)mission.ID : (int?)null;
            message.ProgressType = progressType;
            message.MessageEnum = messageEnum.HasValue ? (int)messageEnum.Value : (int?)null;

            Peers.SendToAll(ref message, NetDeliveryMethod.ReliableOrdered);
        }

        public void OnMissionProgress(ref MyEventMissionProgress msg)
        {
            if (IsStory() && !IsHost)
            {
                ClearCountdownNotification();

                Debug.Assert(m_followMission != null);

                switch (msg.ProgressType)
                {
                    case MyMissionProgressType.Success:
                        MyMission.ShowObjectiveCompleted();
                        m_followMission.SetObjectives(null);
                        break;
                    case MyMissionProgressType.Fail:
                        {
                            MyTextsWrapperEnum? message = null;
                            if (msg.MessageEnum.HasValue)
                            {
                                message = (MyTextsWrapperEnum)msg.MessageEnum.Value;
                                if (!MyMwcEnums.IsValidValue(message.Value))
                                {
                                    message = null;
                                }
                            }
                            MySession.Static.GameOver(message);
                        }
                        break;
                    case MyMissionProgressType.NewObjective:
                        m_followMission.SetObjectives(msg.MissionId);
                        break;
                    default:
                        Debug.Fail("Unknown MyMissionProgressType");
                        break;
                }
            }
        }

        public void SendPlayDialogue(MyDialogueEnum id)
        {
            var msg = new MyEventPlayDialogue();
            msg.DialogueEnum = (int)id;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        void OnPlayDialogue(ref MyEventPlayDialogue msg)
        {
            if (!MyMwcEnums.IsValidValue((MyDialogueEnum)msg.DialogueEnum))
            {
                Alert("Invalid dialogue enum", msg.SenderEndpoint, msg.EventType);
                return;
            }

            MyScriptWrapper.PlayDialogue((MyDialogueEnum)msg.DialogueEnum);
        }

        public void SendApplyTransition(MyMusicTransitionEnum transitionEnum, int priority, string category, bool loop)
        {
            SendMusicTransition(MyMusicEventEnum.APPLY_TRANSITION, priority, transitionEnum, category, loop);
        }

        public void SendStopMusic()
        {
            SendMusicTransition(MyMusicEventEnum.STOP_MUSIC, null, null, null, false);
        }

        public void SendStopTransition(int priority)
        {
            SendMusicTransition(MyMusicEventEnum.STOP_TRANSITION, priority, null, null, false);
        }

        private void SendMusicTransition(MyMusicEventEnum musicEventType, int? priority, MyMusicTransitionEnum? transitionEnum, string category, bool loop)
        {
            var msg = new MyEventMusicTransition();
            msg.MusicEventType = musicEventType;
            msg.TransitionEnum = transitionEnum.HasValue ? (int)transitionEnum.Value : 0;
            msg.Priority = priority ?? 0;
            msg.Category = category;
            msg.Loop = loop;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        void OnMusicTransition(ref MyEventMusicTransition msg)
        {
            switch (msg.MusicEventType)
            {
                case MyMusicEventEnum.STOP_MUSIC:
                    MyScriptWrapper.StopMusic();
                    break;

                case MyMusicEventEnum.STOP_TRANSITION:
                    MyScriptWrapper.StopTransition(msg.Priority);
                    break;

                case MyMusicEventEnum.APPLY_TRANSITION:
                    {
                        var transition = (MyMusicTransitionEnum)msg.TransitionEnum;

                        if (!MyMwcEnums.IsValidValue(transition))
                        {
                            Alert("Invalid MyMusicTransitionEnum", msg.SenderEndpoint, msg.EventType);
                            return;
                        }

                        MyScriptWrapper.ApplyTransition(transition, msg.Priority, msg.Category);
                    }
                    break;

                default:
                    Alert("Unknown music transition type", msg.SenderEndpoint, msg.EventType);
                    break;
            }
        }

        public void SendPlaySound(Vector3? position, MySoundCuesEnum id)
        {
            var msg = new MyEventPlaySound();
            msg.Position = position;
            msg.SoundEnum = (int)id;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        void OnPlaySound(ref MyEventPlaySound msg)
        {
            if (!MyMwcEnums.IsValidValue((MySoundCuesEnum)msg.SoundEnum))
            {
                Alert("Invalid sound enum", msg.SenderEndpoint, msg.EventType);
                return;
            }

            if (msg.Position.HasValue)
            {
                MyScriptWrapper.PlaySound3D(msg.Position.Value, (MySoundCuesEnum)msg.SoundEnum);
            }
            else
            {
                MyScriptWrapper.PlaySound2D((MySoundCuesEnum)msg.SoundEnum);
            }
        }

        public void SendHeadshake(float amount)
        {
            var msg = new MyEventHeadshake();
            msg.Amount = amount;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableSequenced, 3);
        }

        void OnHeadshake(ref MyEventHeadshake msg)
        {
            MyScriptWrapper.IncreaseHeadShake(msg.Amount);
        }

        public void SendSetEntityFaction(MyEntity entity, MyMwcObjectBuilder_FactionEnum faction)
        {
            Debug.Assert(entity.EntityId.HasValue, "Entity must have id");

            var msg = new MyEventSetEntityFaction();
            msg.Faction = faction;
            msg.EntityId = entity.EntityId.Value.NumericValue;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        void OnSetEntityFaction(ref MyEventSetEntityFaction msg)
        {
            MyEntity entity;
            if (MyEntities.TryGetEntityById(msg.EntityId.ToEntityId(), out entity))
            {
                entity.Faction = msg.Faction;
            }
            else
            {
                Alert("Entity not found: " + msg.EntityId, msg.SenderEndpoint, msg.EventType);
            }
        }

        public void SendSetActorFaction(MyActorEnum actor, MyMwcObjectBuilder_FactionEnum faction)
        {
            var msg = new MyEventSetActorFaction();
            msg.ActorId = (int)actor;
            msg.Faction = faction;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        void OnSetActorFaction(ref MyEventSetActorFaction msg)
        {
            var actorEnum = (MyActorEnum)msg.ActorId;
            if (MyMwcEnums.IsValidValue(actorEnum))
            {
                MyScriptWrapper.SetActorFaction(actorEnum, msg.Faction);
            }
            else
            {
                Alert("Actor enum invalid", msg.SenderEndpoint, msg.EventType);
            }
        }

        public void SendSetFactionRelation(MyMwcObjectBuilder_FactionEnum factionA, MyMwcObjectBuilder_FactionEnum factionB, float relation)
        {
            var msg = new MyEventSetFactionRelation();
            msg.FactionA = factionA;
            msg.FactionB = factionB;
            msg.Relation = relation;

            Peers.SendToAll(ref msg, NetDeliveryMethod.ReliableOrdered);
        }

        void OnSetFactionRelation(ref MyEventSetFactionRelation msg)
        {
            MyScriptWrapper.SetFactionRelation(msg.FactionA, msg.FactionB, msg.Relation);
        }
    }
}
