using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Journal;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.Helpers {
    static class MyGuiEventHelpers 
    {
        static Dictionary<EventTypeEnum, MyGuiEventHelper> m_eventHelpers = new Dictionary<EventTypeEnum, MyGuiEventHelper>();        
        static Dictionary<MyJournalCategory, MyGuiEventHelper> m_journalCategoryHelpers = new Dictionary<MyJournalCategory, MyGuiEventHelper>();

        //Arrays of enums values
        public static Array MyEventHelperTypesEnumValues { get; private set; }
        public static Array MyJournalCategoryTypesEnumValues { get; private set; }

        private static readonly Vector4 ITEM_BACKGROUND_COLOR = new Vector4(0.9f,0.9f,0.9f,1);

        static MyGuiEventHelpers()
        {
            MyMwcLog.WriteLine("MyGuiEventHelpers()");

            MyEventHelperTypesEnumValues = Enum.GetValues(typeof(EventTypeEnum));
            MyJournalCategoryTypesEnumValues = Enum.GetValues(typeof (MyJournalCategory));
        }

        public static MyGuiEventHelper GetEventHelper(EventTypeEnum eventType)
        {
            MyGuiEventHelper ret;
            if(m_eventHelpers.TryGetValue(eventType, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiEventHelper GetJournalCategoryHelper(MyJournalCategory journalCategory)
        {
            MyGuiEventHelper ret;
            if (m_journalCategoryHelpers.TryGetValue(journalCategory, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {
            m_eventHelpers.Clear();            
            m_journalCategoryHelpers.Clear();
        }
        
        /*
        public static MyTexture2D GetCurrentMissionTexture()
        {
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\EventsMissionCurrent");
        }
        */


        public static void LoadContent()
        {
            var darkColor = ITEM_BACKGROUND_COLOR*0.6f;
            darkColor.W = 0.9f;
            #region Event types
            m_eventHelpers.Add(EventTypeEnum.GlobalEvent,
                new MyGuiEventHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\EventsGlobalEvent"), MyTextsWrapperEnum.GlobalEvents, ITEM_BACKGROUND_COLOR));
            m_eventHelpers.Add(EventTypeEnum.MissionFinished,
                new MyGuiEventHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\EventsMissionStarted"), MyTextsWrapperEnum.FinishedMissions, darkColor));
            m_eventHelpers.Add(EventTypeEnum.MissionStarted,
                new MyGuiEventHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\EventsMissionStarted"), MyTextsWrapperEnum.StartedMissions, ITEM_BACKGROUND_COLOR));
            m_eventHelpers.Add(EventTypeEnum.SubmissionFinished,
                new MyGuiEventHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\EventsMissionStarted"), MyTextsWrapperEnum.FinishedSubmissions, darkColor));
            m_eventHelpers.Add(EventTypeEnum.SubmissionAvailable,
                new MyGuiEventHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\EventsMissionStarted"), MyTextsWrapperEnum.AvailableSubmissions, ITEM_BACKGROUND_COLOR));
            m_eventHelpers.Add(EventTypeEnum.Story,
                new MyGuiEventHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\EventsStory"), MyTextsWrapperEnum.AvailableSubmissions,ITEM_BACKGROUND_COLOR));
            #endregion            

            #region Journal categories
            m_journalCategoryHelpers.Add(MyJournalCategory.AllEvents, new MyGuiEventHelper(MyGuiManager.GetJournalFilterAllTexture(), MyTextsWrapperEnum.AllEvents, ITEM_BACKGROUND_COLOR));
            m_journalCategoryHelpers.Add(MyJournalCategory.GlobalEvents, new MyGuiEventHelper(MyGuiManager.GetJournalFilterGlobalEventsTexture(), MyTextsWrapperEnum.GlobalEvents, ITEM_BACKGROUND_COLOR));
            //m_journalCategoryHelpers.Add(MyJournalCategory.CurrentMission, new MyGuiEventHelper(MyGuiManager.GetJournalFilterCurrentMissionTexture(), MyTextsWrapperEnum.CurrentMission, ITEM_BACKGROUND_COLOR));
            m_journalCategoryHelpers.Add(MyJournalCategory.AllMissions, new MyGuiEventHelper(MyGuiManager.GetJournalFilterMissionsTexture(), MyTextsWrapperEnum.AllMissions, ITEM_BACKGROUND_COLOR));
           // m_journalCategoryHelpers.Add(MyJournalCategory.AccomplishedMissions, new MyGuiEventHelper(MyGuiManager.GetJournalFilterAccomplishedMissionsTexture(), MyTextsWrapperEnum.AccomplishedMissions, ITEM_BACKGROUND_COLOR));
            m_journalCategoryHelpers.Add(MyJournalCategory.Story, new MyGuiEventHelper(MyGuiManager.GetJournalFilterStorytexture(), MyTextsWrapperEnum.Story, ITEM_BACKGROUND_COLOR));
            #endregion
        }
    }
}
