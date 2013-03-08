#region Using

using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.TransparentGeometry;

#endregion

namespace MinerWars.AppCode.Game.World.Global
{
    class MyGlobalEvent
    {
        /// <summary>
        /// If this is 0, event can only by activated via <code>ActivationDateTime</code> property.
        /// </summary>
        public float RatePerHour { get; set; }
        public MyTextsWrapperEnum Name { private set; get; }
        public MyTextsWrapperEnum Description { private set; get; }
        public MyGlobalEventEnum Type { private set; get; }
        public DateTime? ActivationDateTime { private set; get; }
        public bool Activated { get; set; }
        public MyTexture2D Icon { private set; get; }
        public EventHandler Action { private set; get; }
        public bool WriteToEventLog { private set; get; }
        public bool Enabled { get; set; }

        public MyGlobalEvent(MyGlobalEventEnum Type, MyTextsWrapperEnum Name, MyTextsWrapperEnum Description, float RatePerHour, MyTexture2D Icon, EventHandler Action, bool WriteToEventLog, bool Enabled)
        {
            this.Type = Type;

            this.Name = Name;
            this.Description = Description;

            this.RatePerHour = RatePerHour; //occurences per hour
            this.ActivationDateTime = null;
            this.Icon = Icon;
            this.Action = Action;
            this.WriteToEventLog = WriteToEventLog;
            this.Enabled = Enabled;
        }

        public MyGlobalEvent(MyGlobalEventEnum Type, MyTextsWrapperEnum Name, MyTextsWrapperEnum Description, DateTime activationDateTime, MyTexture2D Icon, EventHandler Action, bool WriteToEventLog, bool Enabled)
            : this(Type, Name, Description, 0, Icon, Action, WriteToEventLog, Enabled)
        {
            ActivationDateTime = activationDateTime;
        }
    }

    public enum MyGlobalEventEnum
    {
        SunWind = 0,
        FractionStatusChange = 1,
        MeteorWind = 2,
        IceStorm = 3,
        IceComet = 4,
    }

    class MyGlobalEvents
    {
        static MyGlobalEvent[] m_globalEvents = new MyGlobalEvent[Enum.GetValues(typeof(MyGlobalEventEnum)).Length];
        static int m_elapsedTimeInMilliseconds = 0;

        static readonly int GLOBAL_EVENT_UPDATE_RATIO_IN_MS = 3600 * 1000 / MyGlobalEventsConstants.GLOBAL_EVENTS_HOUR_RATIO;

        static MyGlobalEvents()
        {
            m_globalEvents[(int)MyGlobalEventEnum.SunWind] =
                new MyGlobalEvent(
                    Type: MyGlobalEventEnum.SunWind,
                    Name: MyTextsWrapperEnum.GlobalEventSunWindName,
                    Description: MyTextsWrapperEnum.GlobalEventSunWindDescription,
                    RatePerHour: 12.0f,
                    Icon: null,
                    Enabled: true,
                    Action: delegate(object o, EventArgs e)
                    {
                        //dont allow sunwind in god editor on or when the game is paused
                        if (!MySunWind.IsActive && !(MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive()) && !MyMinerGame.IsPaused())
                        {
                            //MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.GlobalEventSunWindDescription, 5000));
                            MySunWind.Start();
                            //MyAudio.AddCue2D(MySoundCuesEnum.HudSolarFlareWarning);
                        }
                    },
                    WriteToEventLog: false
            );

            m_globalEvents[(int)MyGlobalEventEnum.FractionStatusChange] =
                new MyGlobalEvent(
                    Type: MyGlobalEventEnum.FractionStatusChange,
                    Name: MyTextsWrapperEnum.GlobalEventFactionChangeName,
                    Description: MyTextsWrapperEnum.GlobalEventFactionChangeDescription,
                    RatePerHour: 10.0f,
                    Icon: null,
                    Enabled: false,
                    Action: delegate(object o, EventArgs e)
                    {
                        float statusChange = MyMwcUtils.GetRandomFloat(MyFactions.RELATION_WORST, MyFactions.RELATION_BEST) / 10.0f;

                        int[] enumValues = MyMwcFactionsByIndex.GetFactionsIndexes();
                        System.Diagnostics.Debug.Assert(enumValues.Length > 3);
                        
                        MyMwcObjectBuilder_FactionEnum faction1;
                        do
                        {
                            faction1 = MyMwcFactionsByIndex.GetFaction(MyMwcUtils.GetRandomInt(enumValues.Length));
                        }
                        while (faction1 == MyMwcObjectBuilder_FactionEnum.None);

                        MyMwcObjectBuilder_FactionEnum faction2;
                        do
                        {
                            faction2 = MyMwcFactionsByIndex.GetFaction(MyMwcUtils.GetRandomInt(enumValues.Length));
                        }
                        while ((faction1 == faction2) || (faction2 == MyMwcObjectBuilder_FactionEnum.None));

                        MyFactions.ChangeFactionStatus(faction1, faction2, statusChange);
                    },
                    WriteToEventLog: false
            );

            m_globalEvents[(int)MyGlobalEventEnum.MeteorWind] =
                new MyGlobalEvent(
                    Type: MyGlobalEventEnum.SunWind,
                    Name: MyTextsWrapperEnum.GlobalEventMeteorWindName,
                    Description: MyTextsWrapperEnum.GlobalEventSunWindDescription,
                    RatePerHour: MyFakes.ENABLE_RANDOM_METEOR_SHOWER ? 1.0f : 0.0f,
                    Icon: null,
                    Enabled: false,
                    Action: delegate(object o, EventArgs e)
                    {
                        //dont allow sunwind in god editor on or when the game is paused
                        if (!(MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive()) && !MyMinerGame.IsPaused())
                        {
                            //MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.GlobalEventSunWindDescription, 5000));
                            MyMeteorWind.Start();
                            //MyAudio.AddCue2D(MySoundCuesEnum.SfxSolarFlareWarning);
                        }
                    },
                    WriteToEventLog: false
            );


            // todo implement localization strings
            m_globalEvents[(int)MyGlobalEventEnum.IceStorm] =
               new MyGlobalEvent(
                   Type: MyGlobalEventEnum.IceStorm,
                   Name: MyTextsWrapperEnum.GlobalEventIceStormName, //Name: MyTextsWrapperEnum.GlobalEvent_IceStorm_Name,
                   Description: MyTextsWrapperEnum.GlobalEventSunWindDescription,//IceStorm_Description,
                   RatePerHour: MyFakes.ENABLE_RANDOM_ICE_STORM ? 1.0f : 0.0f,
                   Icon: null,
                   Enabled: false,
                   Action: delegate(object o, EventArgs e)
                   {
                       //dont allow sunwind in god editor on or when the game is paused
                       if (!(MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive()) && !MyMinerGame.IsPaused())
                       {
                           MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.GlobalEventSunWindDescription, 5000, null)); // MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.GlobalEvent_IceStorm_Description, 5000));
                           MyIceStorm.Start();
                           MyAudio.AddCue2D(MySoundCuesEnum.HudSolarFlareWarning);
                       }
                   },
                   WriteToEventLog: false
           );

            m_globalEvents[(int)MyGlobalEventEnum.IceComet] =
                new MyGlobalEvent(
                    Type: MyGlobalEventEnum.IceComet,
                    Name: MyTextsWrapperEnum.GlobalEventIceCometName,
                    Description: MyTextsWrapperEnum.GlobalEventIceCometDescription,
                    RatePerHour: 0.0f,
                    Icon: null,
                    Enabled: false,
                    Action: delegate(object o, EventArgs e)
                    {
                        if (!(MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive()) && !MyMinerGame.IsPaused())
                        {
                            MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.GlobalEventSunWindDescription, 5000));
                            MyIceComet.Start();
                            MyAudio.AddCue2D(MySoundCuesEnum.HudSolarFlareWarning);
                        }
                    },
                    WriteToEventLog: false
            );

            foreach (MyGlobalEvent e in m_globalEvents)
            {
                System.Diagnostics.Debug.Assert(e != null);
            }

            MyFactions.OnFactionStatusChanged += new MyFactionStatusChangeHandler(MyFactions_OnFactionStatusChanged);
        }

        public static MyGlobalEvent GetGlobalEventByType(MyGlobalEventEnum value)
        {
            return m_globalEvents[(int) value];
        }

        static void MyFactions_OnFactionStatusChanged(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2, MyFactionRelationEnum previousRelation, bool display, bool save)
        {
            if (!display) 
            {
                return;
            }

            MyTextsWrapperEnum relationChangeEnum = MyTextsWrapperEnum.General;
            MyFactionRelationEnum newRelation = MyFactions.GetFactionsRelation(faction1, faction2);

            switch (newRelation)
            {
                case MyFactionRelationEnum.Friend:
                    relationChangeEnum = MyTextsWrapperEnum.GlobalEventFactionRelationChange_NeutralToFriends;
                    break;

                case MyFactionRelationEnum.Neutral:
                    if (previousRelation == MyFactionRelationEnum.Friend)
                        relationChangeEnum = MyTextsWrapperEnum.GlobalEventFactionRelationChange_FriendsToNeutral;
                    else
                        relationChangeEnum = MyTextsWrapperEnum.GlobalEventFactionRelationChange_EnemyToNeutral;
                    break;

                case MyFactionRelationEnum.Enemy:
                    relationChangeEnum = MyTextsWrapperEnum.GlobalEventFactionRelationChange_NeutralToEnemy;
                    break;
            }

            MyHudNotification.AddNotification(new MyHudNotification.MyNotification(relationChangeEnum, 5000, null, new object[] { MyFactionConstants.GetFactionProperties(faction1).Name.ToString(), MyFactionConstants.GetFactionProperties(faction2).Name.ToString() }));
        }

        public static void Update()
        {
            if (!MyFakes.ENABLE_GLOBAL_EVENTS)
                return;

#if RENDER_PROFILING
            return;
#endif
            m_elapsedTimeInMilliseconds += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
            if (m_elapsedTimeInMilliseconds < GLOBAL_EVENT_UPDATE_RATIO_IN_MS)
                return;
            
            float hoursElapsed = m_elapsedTimeInMilliseconds / (3600 * 1000.0f);

            foreach (MyGlobalEvent globalEvent in m_globalEvents)
            {
                if (!globalEvent.Enabled)
                {
                    continue;
                }

                if (!globalEvent.Activated && globalEvent.ActivationDateTime.HasValue && globalEvent.ActivationDateTime.Value < MySession.Static.GameDateTime)
                {
                    globalEvent.Activated = true;
                    StartGlobalEvent(globalEvent);
                }
                else
                {
                    float ratio = hoursElapsed * globalEvent.RatePerHour;
                    if (MyMwcUtils.GetRandomFloat(0.0f, 1.0f) < ratio)
                    {
                        StartGlobalEvent(globalEvent);
                    }
                }
            }

            m_elapsedTimeInMilliseconds = 0;
        }

        public static void StartGlobalEvent(MyGlobalEvent globalEvent)
        {
            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.SendEvent(MyCamera.Position, globalEvent.Type, MyMwcUtils.GetRandomInt(int.MaxValue));
            }

            globalEvent.Action(globalEvent, null);
            AddGlobalEventToEventLog(globalEvent);
        }

        public static void StartGlobalEvent(MyGlobalEventEnum globalAction)
        {
            MyGlobalEvent globalEvent = m_globalEvents[(int)globalAction];
            StartGlobalEvent(globalEvent);
        }

        private static void AddGlobalEventToEventLog(MyGlobalEvent globalEvent)
        {
            if (globalEvent.WriteToEventLog)
            {
                MySession.Static.EventLog.AddGlobalEvent(globalEvent.Type);
            }
        }

        public static void Enable(MyGlobalEventEnum globalAction, bool enabled)
        {
            MyGlobalEvent globalEvent = m_globalEvents[(int)globalAction];
            globalEvent.Enabled = enabled;
        }

        public static void SetRatePerHour(MyGlobalEventEnum globalAction, float ratePerHour)
        {
            MyGlobalEvent globalEvent = m_globalEvents[(int)globalAction];
            globalEvent.RatePerHour = ratePerHour;
        }

        internal static void EnableAllGlobalEvents()
        {
            foreach (var globalEvent in Enum.GetValues(typeof(World.Global.MyGlobalEventEnum)))
            {
                Enable((World.Global.MyGlobalEventEnum)globalEvent, true);
            }
        }

        internal static void DisableAllGlobalEvents()
        {
            foreach (var globalEvent in Enum.GetValues(typeof(World.Global.MyGlobalEventEnum)))
            {
                Enable((World.Global.MyGlobalEventEnum)globalEvent, false);
            }
        }
    }
}
