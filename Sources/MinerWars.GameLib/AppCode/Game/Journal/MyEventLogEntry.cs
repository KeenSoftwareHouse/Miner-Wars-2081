using System;
using System.Text;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Journal
{
    public enum StatusEnum
    {
        Unread = 1,
        Read = 2,
    }

    public enum EventTypeEnum
    {
        GlobalEvent = 1,
        MissionStarted = 2,
        MissionFinished = 3,
        SubmissionFinished = 4,
        SubmissionAvailable = 5,
        Story = 6,
    }

    class MyEventLogEntry
    {
        public StatusEnum Status { get; set; }

        public EventTypeEnum EventType { get; set; }

        public DateTime Time { get; set; }

        public int EventTypeID { get; set; }

        public MyEventLogEntry() { Time = new DateTime(2081, 1, 1); }

        public MyEventLogEntry(DateTime time)
        {
            Time = time;
            Status = StatusEnum.Unread;
        }

        public void Init(MyMwcObjectBuilder_Event objectBuilder)
        {
            Status = (StatusEnum)objectBuilder.Status;
            EventType = (EventTypeEnum)objectBuilder.EventType;
            Time = objectBuilder.Time;
            EventTypeID = objectBuilder.EventTypeID;
        }

        internal MyMwcObjectBuilder_Event GetObjectBuilder()
        {
            MyMwcObjectBuilder_Event builder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.Event, null) as MyMwcObjectBuilder_Event;
            builder.Status = (byte) Status;
            builder.EventType = (byte) EventType;
            builder.Time = Time;
            builder.EventTypeID = EventTypeID;
            return builder;
        }

        public StringBuilder GetName()
        {
            switch (EventType)
            {
                case EventTypeEnum.GlobalEvent:
                    var globalEvent = MyGlobalEvents.GetGlobalEventByType((MyGlobalEventEnum)EventTypeID);
                    if (globalEvent != null)
                    {
                        return MyTextsWrapper.Get(globalEvent.Name);
                    }
                    break;
                case EventTypeEnum.MissionStarted:
                case EventTypeEnum.MissionFinished:
                case EventTypeEnum.SubmissionAvailable:
                case EventTypeEnum.SubmissionFinished:
                    //case EventTypeEnum.Story:
                    var mission = MyMissions.GetMissionByID((MyMissionID)EventTypeID);
                    if (mission != null)
                    {
                        return mission.NameTemp;
                    }
                    break;
            }
            return null;
            //return MyTextsWrapperEnum.Null;
        }

        public StringBuilder GetDescription() //TODO: change this to MyTextsWrapperEnum
        {
            switch (EventType)
            {
                case EventTypeEnum.GlobalEvent:
                    var globalEvent = MyGlobalEvents.GetGlobalEventByType((MyGlobalEventEnum)EventTypeID);
                    return MyTextsWrapper.Get(globalEvent.Description);
                case EventTypeEnum.MissionStarted:
                case EventTypeEnum.MissionFinished:
                case EventTypeEnum.SubmissionAvailable:
                case EventTypeEnum.SubmissionFinished:
                //case EventTypeEnum.Story:
                    var mission = MyMissions.GetMissionByID((MyMissionID)EventTypeID);
                    return mission.DescriptionTemp;
            }

            return null;
        }
    }
}
