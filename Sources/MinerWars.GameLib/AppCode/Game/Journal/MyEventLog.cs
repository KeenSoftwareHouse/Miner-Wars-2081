using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.AppCode.Game.Journal
{
    /// <summary>
    /// Event log stores info about missions and about global game events, 
    /// such as faction changes, or important player achievements / impacts on the game.
    /// </summary>
    class MyEventLog
    {
        public List<MyEventLogEntry> Events { get; set; }        

        public MyEventLog()
        {
            Events = new List<MyEventLogEntry>();
        }

        private void AddEvent(int eventID, EventTypeEnum eventTypeEnum)
        {
            Events.Add(new MyEventLogEntry(MySession.Static.GameDateTime)
            {
                EventType = eventTypeEnum,
                EventTypeID = eventID,
            });
        }

        public void AddMissionStarted(MyMissionID missionID)
        {

            bool contains = false;
            foreach (MyEventLogEntry myEventLogEntry in Events)
            {
                if (myEventLogEntry.EventType == EventTypeEnum.MissionStarted && myEventLogEntry.EventTypeID == (int)missionID)
                {
                    contains = true; // sometimes there was duplicates of entries
                    break;
                }
            }

            if (!contains) AddEvent((int)missionID, EventTypeEnum.MissionStarted);
        }

        public void MissionFinished(MyMissionID missionID)
        {
            //AddEvent((int)missionID, EventTypeEnum.MissionFinished);
            foreach (MyEventLogEntry myEventLogEntry in Events)
            {
                if(myEventLogEntry.EventType ==EventTypeEnum.MissionStarted && myEventLogEntry.EventTypeID ==(int)missionID)
                {
                    myEventLogEntry.EventType = EventTypeEnum.MissionFinished;
                }
            }
        }

        public void AddSubmissionAvailable(MyMissionID missionID)
        {
            bool contains = false;
            foreach (MyEventLogEntry myEventLogEntry in Events)
            {
                if (myEventLogEntry.EventType == EventTypeEnum.SubmissionAvailable && myEventLogEntry.EventTypeID == (int)missionID)
                {
                    contains = true; // sometimes there was duplicates of entries
                    break;
                }
            }

            if (!contains) AddEvent((int)missionID, EventTypeEnum.SubmissionAvailable);

        }

        public void SubmissionFinished(MyMissionID missionID)
        {
            //AddEvent((int)missionID, EventTypeEnum.SubmissionFinished);
            foreach (MyEventLogEntry myEventLogEntry in Events)
            {
                if (myEventLogEntry.EventType == EventTypeEnum.SubmissionAvailable && myEventLogEntry.EventTypeID == (int)missionID)
                {
                    myEventLogEntry.EventType = EventTypeEnum.SubmissionFinished;
                }
            }

        }

        public void AddGlobalEvent(MyGlobalEventEnum eventID)
        {
            AddEvent((int)eventID, EventTypeEnum.GlobalEvent);
        }


        public void AddStoryEvent()
        {
            //AddEvent((int)eventID, EventTypeEnum.GlobalEvent);
        }


        public bool IsMissionFinished(MyMissionID missionID)
        {
            foreach (var logEntry in Events)
            {
                if ((logEntry.EventType == EventTypeEnum.MissionFinished ||
                     logEntry.EventType == EventTypeEnum.SubmissionFinished) &&
                     logEntry.EventTypeID == (int) missionID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AreMissionsFinished(MyMissionID[] missions)
        {
            foreach (MyMissionID missionID in missions)
            {
                if (!IsMissionFinished(missionID))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetFinishedMissionCount(IEnumerable<MyMissionID> missions)
        {
            int count = 0;
            foreach (MyMissionID missionID in missions)
            {
                if (IsMissionFinished(missionID))
                {
                    ++count;
                }
            }
            return count;
        }

        internal void Init(MyMwcObjectBuilder_Checkpoint checkpointObjectBuilder)
        {
            Debug.Assert(checkpointObjectBuilder != null);

            if (checkpointObjectBuilder.EventLogObjectBuilder == null) return;

            Events.Clear();
            foreach (MyMwcObjectBuilder_Event eventLogEntryObjectBuilder in checkpointObjectBuilder.EventLogObjectBuilder)
            {
                var eventLogEntry = new MyEventLogEntry();
                eventLogEntry.Init(eventLogEntryObjectBuilder);
                Events.Add(eventLogEntry);
            }
        }

        internal List<MyMwcObjectBuilder_Event> GetObjectBuilder()
        {
            return Events.Select(item => item.GetObjectBuilder()).ToList();
        }
    }
}
