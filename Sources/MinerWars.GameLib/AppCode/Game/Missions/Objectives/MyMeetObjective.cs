using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWarsMath;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Missions
{
    class MyMeetObjective : MyObjective
    {
        private uint? m_botDetectorId;
        private uint? m_botToTalkId;
        private float m_slowdown;
        private float m_distanceToTalk;

        private MySmallShipBot m_botToTalk;
        private MyEntityDetector m_botDetector;
        //private MyDialogueEnum? m_detectorDialog;
        private bool m_stopFollow;
        public string PathName { get; set; }

        public uint BotToTalkId
        {
            set
            {
                m_botToTalkId = value;
                UpdateBotToTalk();
            }
        }

        public bool FollowMe { get; set; }
        private bool m_success;
        private bool m_constructorFromName;
        private string m_botName;
        protected bool m_detectorReached;

        public MyMeetObjective(MyTextsWrapperEnum Name, MyMissionID ID, MyTextsWrapperEnum Description, MyMission ParentMission, MyMissionID[] RequiredMissions, uint? detectorId, string botName, float distanceToTalk = 50, float slowdown = 0.25f, MyDialogueEnum? successDialogueId = null, MyDialogueEnum? startDialogueId = null, bool stopFollow = true)
            : base(Name, ID, Description, null, ParentMission, RequiredMissions, null, null, successDialogueId, startDialogueId)
        {
            m_botDetectorId = detectorId;
            m_distanceToTalk = distanceToTalk;
            m_slowdown = slowdown;
            //m_detectorDialog = detectorDialogId;
            m_stopFollow = stopFollow;
            m_constructorFromName = true;
            m_botName = botName;
            FollowMe = true;
        }


        public MyMeetObjective(MyTextsWrapperEnum Name, MyMissionID ID, MyTextsWrapperEnum Description, MyMission ParentMission, MyMissionID[] RequiredMissions, uint? detectorId, uint? botId, float distanceToTalk = 50, float slowdown = 0.25f, MyDialogueEnum? successDialogueId = null, MyDialogueEnum? startDialogueId = null, bool stopFollow = true)
            : base(Name, ID, Description, null, ParentMission, RequiredMissions, null, null, successDialogueId, startDialogueId)
        {
            m_botToTalkId = botId;
            m_botDetectorId = detectorId;
            m_distanceToTalk = distanceToTalk;
            m_slowdown = slowdown;
            //m_detectorDialog = detectorDialogId;

            m_stopFollow = stopFollow;
            FollowMe = true;
        }


        [Obsolete]
        public MyMeetObjective(StringBuilder Name, MyMissionID ID, StringBuilder Description, MyMission ParentMission, MyMissionID[] RequiredMissions, uint? detectorId, string botName, float distanceToTalk = 50, float slowdown = 0.25f, MyDialogueEnum? successDialogueId = null, MyDialogueEnum? startDialogueId = null, bool stopFollow = true)
            : base(Name, ID, Description, null, ParentMission, RequiredMissions, null, null, successDialogueId, startDialogueId)
        {
            m_botDetectorId = detectorId;
            m_distanceToTalk = distanceToTalk;
            m_slowdown = slowdown;
            //m_detectorDialog = detectorDialogId;
            m_stopFollow = stopFollow;
            m_constructorFromName = true;
            m_botName = botName;
            FollowMe = true;
        }

        [Obsolete]
        public MyMeetObjective(StringBuilder Name, MyMissionID ID, StringBuilder Description, MyMission ParentMission, MyMissionID[] RequiredMissions, uint? detectorId, uint? botId, float distanceToTalk = 50, float slowdown = 0.25f, MyDialogueEnum? successDialogueId = null, MyDialogueEnum? startDialogueId = null, bool stopFollow = true)
            : base(Name, ID, Description, null, ParentMission, RequiredMissions, null, null, successDialogueId, startDialogueId)
        {
            m_botToTalkId = botId;
            m_botDetectorId = detectorId;
            m_distanceToTalk = distanceToTalk;
            m_slowdown = slowdown;
            //m_detectorDialog = detectorDialogId;

            m_stopFollow = stopFollow;
            FollowMe = true;
        }

        public override void Load()
        {
            MyMwcLog.WriteLine("MyMeetObjective::Load - Start");

            base.Load();

            if (!m_constructorFromName && !m_botToTalkId.HasValue)
            {
                MyMwcLog.WriteLine("MyMeetObjective::Load - End null");
                return;
            }

            UpdateBotToTalk();

            MyMwcLog.WriteLine("MyMeetObjective::Load - End");
        }

        private void UpdateBotToTalk()
        {

            m_success = false;

            if (m_constructorFromName)
            {
                MyMwcLog.WriteLine("Bot name: " + m_botName);
                m_botToTalk = (MySmallShipBot)MyScriptWrapper.GetEntity(m_botName);
                m_botToTalkId = m_botToTalk.EntityId.Value.NumericValue;
                MissionEntityIDs.Add(m_botToTalkId.Value);
            }
            else
            {
                MyMwcLog.WriteLine("Bot ID: " + m_botToTalkId.Value.ToString());
                MissionEntityIDs.Add(m_botToTalkId.Value);
                m_botToTalk = (MySmallShipBot)MyScriptWrapper.GetEntity(m_botToTalkId.Value);
            }

            m_detectorReached = false;
            Debug.Assert(m_botToTalk != null);

            if (m_botToTalk.WaypointPath != null)
            {
                PathName = m_botToTalk.WaypointPath.Name;
            }
            m_botToTalk.SpeedModifier = m_slowdown;

            if (m_botDetectorId.HasValue)
            {
                m_botDetector = MyScriptWrapper.GetDetector(m_botDetectorId.Value);
                m_botDetector.On();
                m_botDetector.OnEntityEnter += m_botDetector_OnEntityEnter;
            }
            else if (FollowMe)
            {
                MyScriptWrapper.Follow(MySession.PlayerShip, m_botToTalk);
            }
        }



        private void StopFollow()
        {
            if (m_stopFollow)
            {
                MyScriptWrapper.StopFollow(m_botToTalk);
            }
            else if (PathName != null) //PATROL MODE
            {
                if (m_botToTalk != null)
                {
                    m_botToTalk.SetWaypointPath(PathName);
                    m_botToTalk.PatrolMode = MyPatrolMode.CYCLE;
                    m_botToTalk.SpeedModifier = m_slowdown;
                    m_botToTalk.Patrol();
                }
            }

            if (m_botToTalk != null)
            {
                m_botToTalk.LookTarget = null;
            }
        }

        void m_botDetector_OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            if (entity == MySession.PlayerShip)
            {
                m_botToTalk.LookTarget = MySession.PlayerShip;
                MyScriptWrapper.Follow(MySession.PlayerShip, m_botToTalk);
                m_success = true;
                sender.Off();
                m_detectorReached = true;
            }
        }

        public override bool IsSuccess()
        {
            if (m_success)
                return true;

            if (m_botToTalk != null && Vector3.DistanceSquared(m_botToTalk.GetPosition(), MySession.PlayerShip.GetPosition()) <= m_distanceToTalk * m_distanceToTalk)
            {
                MyScriptWrapper.Follow(MySession.PlayerShip, m_botToTalk);
                m_botToTalk.LookTarget = MySession.PlayerShip;
                return true;
            }
            return false;
        }

        public override void Unload()
        {
            StopFollow();

            MissionEntityIDs.Clear();

            if (m_botToTalk != null)
            {
                m_botToTalk.SpeedModifier = 1.0f;
                m_botToTalk = null;

                if (m_constructorFromName)
                {
                    m_botToTalkId = null;
                }
            }

            base.Unload();
            if (m_botDetector != null)
            {
                m_botDetector.Off();
                m_botDetector.OnEntityEnter -= m_botDetector_OnEntityEnter;
            }
        }
    }
}
