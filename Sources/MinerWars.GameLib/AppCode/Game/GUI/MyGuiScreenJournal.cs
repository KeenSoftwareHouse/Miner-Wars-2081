using System;
using System.Diagnostics;
using System.Text;
using KeenSoftwareHouse.Library.Extensions;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Journal;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.GUI
{        
    enum MyJournalCategory
    {                
        GlobalEvents = 1,
        CurrentMission = 2,
        AccomplishedMissions = 3,
        AllMissions = 4,
        Story = 5,
        AllEvents = 6,
    }    

    //Journal removed
    /*
    /// <summary>
    /// TODO filter
    /// Will be finished when we have new GUI elements
    /// </summary>
    class MyGuiScreenJournal : MyGuiScreenBase
    {        
        private readonly MyGuiControlLabel m_gameTimeLabel;
        private MyGuiControlMultilineText m_descriptionText;

        private MyGuiControlListbox m_journalListbox;
        private int? m_selectedEventKey;

        private MyGuiControlCheckbox m_globalEventsCheckbox;
        //private MyGuiControlCheckbox m_currentMissionCheckbox;
        private MyGuiControlCheckbox m_missionsCheckbox;
        //private MyGuiControlCheckbox m_accomplishedCheckbox;
        private MyGuiControlCheckbox m_storyCheckbox;
        private MyGuiControlCheckbox m_allEventsCheckbox;

        private MyEventLog m_eventLog;        

        private const string EVENT_TIME_FORMAT = "[{0}] ";
        private const string SUBMISSION_SEPARATOR = "- ";
        private static readonly Vector2 CHECK_BOX_SIZE = new Vector2(68/1600f,68/1200f);
        private static readonly Vector2 CHECK_BOX_INNER_SIZE = new Vector2(60 / 1600f, 60 / 1200f);
        private static readonly Vector4 CHECK_BOX_COLOR = new Vector4(0.75f, 0.75f, 0.75f, 0.9f);
        private static readonly Vector4 BLUEPRINT_COLOR = new Vector4(1f, 1f, 1f, 0.9f);
        private static readonly Vector4 BLUEPRINT_BORDER_COLOR = new Vector4(0.75f, 0.75f, 0.75f, 0.9f);
        private static Predicate<MyEventLogEntry>[] m_filterConditions;        

        static MyGuiScreenJournal()
        {
            m_filterConditions = new Predicate<MyEventLogEntry>[MyMwcUtils.GetMaxValueFromEnum<MyJournalCategory>()+1];            
            // show all
            m_filterConditions[(int) MyJournalCategory.AllEvents] = 
                eventLog => 
                    true;
            // show only global events
            m_filterConditions[(int)MyJournalCategory.GlobalEvents] = 
                eventLog => 
                    eventLog.EventType == EventTypeEnum.GlobalEvent;
            // show only current mission and submissions
            m_filterConditions[(int) MyJournalCategory.CurrentMission] =
                eventLog =>
                    MyMissions.ActiveMission != null &&
                    ((int)MyMissions.ActiveMission.ID == eventLog.EventTypeID ||
                    MyMissions.ActiveMission.Objectives.Exists(subM => (int)subM.ID == eventLog.EventTypeID));
            // show all missions (started, finished)
            m_filterConditions[(int)MyJournalCategory.AllMissions] = 
                eventLog => 
                    eventLog.EventType == EventTypeEnum.MissionStarted || 
                    eventLog.EventType == EventTypeEnum.MissionFinished || 
                    eventLog.EventType == EventTypeEnum.SubmissionAvailable || 
                    eventLog.EventType == EventTypeEnum.SubmissionFinished||
                    eventLog.EventType == EventTypeEnum.Story;
            // show finished missions
            m_filterConditions[(int)MyJournalCategory.AccomplishedMissions] =
                eventLog =>                  
                    eventLog.EventType == EventTypeEnum.MissionFinished ||                    
                    eventLog.EventType == EventTypeEnum.SubmissionFinished;

            // show finished missions
            m_filterConditions[(int) MyJournalCategory.Story] =
                eventLog =>
                eventLog.EventType == EventTypeEnum.Story;
        }

        public MyGuiScreenJournal(MyEventLog eventLog)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(1f, 1f))
        {
            Debug.Assert(eventLog != null);

            m_eventLog = eventLog;

            if (eventLog.Events.Count>0)
            {
                m_selectedEventKey = eventLog.Events.Count-1;
            }

            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\JournalBackground", flags: TextureFlags.IgnoreQuality);
            m_size = new Vector2(987/1600f,845/1200f);
            // Title
            AddCaption(MyTextsWrapperEnum.JournalDialogTitle, new Vector2(0, 0.005f));



            var m_cancelButton = new MyGuiControlButton(this, new Vector2(0.2491f, -0.3064f), new Vector2(131/1600f,86/1200f), 
           MyGuiConstants.BUTTON_BACKGROUND_COLOR,
           MyGuiManager.GetJournalCloseTexture(), null, null, MyTextsWrapperEnum.EmptyDescription,
           MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnClose_Click,
           true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            m_cancelButton.Text = null;
            Controls.Add(m_cancelButton);

            // Current date & time (clock)
            m_gameTimeLabel = new MyGuiControlLabel(
                this, new Vector2(-0.2229f,-0.2431f), null, 
                new StringBuilder(MySession.Static.GameDateTime.ToString()), 
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, 
                MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            Controls.Add(m_gameTimeLabel);



            var checkBoxDeltaX = 55/1600f;

            // filtering checkboxes
            Vector2 filterCheckboxesPosition = new Vector2(-0.1922f,0.2438f);
            MyGuiEventHelper allEventsHelper = MyGuiEventHelpers.GetJournalCategoryHelper(MyJournalCategory.AllEvents);            
            m_allEventsCheckbox = new MyGuiControlCheckbox(this, filterCheckboxesPosition, CHECK_BOX_SIZE,
                                                           allEventsHelper.Icon, allEventsHelper.Icon,
                                                           allEventsHelper.Description, true, CHECK_BOX_COLOR, true,false,CHECK_BOX_INNER_SIZE);
            m_allEventsCheckbox.OnCheck += OnFilterChanged;
            filterCheckboxesPosition.X += checkBoxDeltaX;
            Controls.Add(m_allEventsCheckbox);

            MyGuiEventHelper globalEventsHelper = MyGuiEventHelpers.GetJournalCategoryHelper(MyJournalCategory.GlobalEvents);            
            m_globalEventsCheckbox = new MyGuiControlCheckbox(this, filterCheckboxesPosition, CHECK_BOX_SIZE,
                                                              globalEventsHelper.Icon, globalEventsHelper.Icon,
                                                              globalEventsHelper.Description, false, CHECK_BOX_COLOR, true, false, CHECK_BOX_INNER_SIZE);
            m_globalEventsCheckbox.OnCheck += OnFilterChanged;
            filterCheckboxesPosition.X += checkBoxDeltaX;
            Controls.Add(m_globalEventsCheckbox);


            MyGuiEventHelper allMissionsHelper = MyGuiEventHelpers.GetJournalCategoryHelper(MyJournalCategory.AllMissions);            
            m_missionsCheckbox = new MyGuiControlCheckbox(this, filterCheckboxesPosition, CHECK_BOX_SIZE,
                                                              allMissionsHelper.Icon, allMissionsHelper.Icon,
                                                              allMissionsHelper.Description, false, CHECK_BOX_COLOR, true,false, CHECK_BOX_INNER_SIZE);
            m_missionsCheckbox.OnCheck += OnFilterChanged;
            filterCheckboxesPosition.X += checkBoxDeltaX;
            Controls.Add(m_missionsCheckbox);


            MyGuiEventHelper storyHelper = MyGuiEventHelpers.GetJournalCategoryHelper(MyJournalCategory.Story);
            m_storyCheckbox = new MyGuiControlCheckbox(this, filterCheckboxesPosition, CHECK_BOX_SIZE,
                                                              storyHelper.Icon, storyHelper.Icon,
                                                              storyHelper.Description, false, CHECK_BOX_COLOR, true, false, CHECK_BOX_INNER_SIZE);

            m_storyCheckbox.OnCheck += OnFilterChanged;
            filterCheckboxesPosition.X += checkBoxDeltaX;
            Controls.Add(m_storyCheckbox);




            // ----- description -----
            m_descriptionText = new MyGuiControlMultilineText(
                this,
                new Vector2(0.1162f - 7/1600f, -0.0025f),
                new Vector2(375/1600f, 506/1200f),
                //new Vector2(0.27f, 0.3f),
                Vector4.Zero,
                MyGuiManager.GetFontMinerWarsBlue(), 0.66f,
                MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                MyTextsWrapper.Get(MyTextsWrapperEnum.Blank),false,true);
            Controls.Add(m_descriptionText);
            m_descriptionText.ScrollbarOffset = 0.0292f +21/1600f;


            m_journalListbox = new MyGuiControlListbox(this,
                                                  new Vector2(-0.1287f, -0.01f),
                                                  new Vector2(359/1600f, 69/1200f),
                                                  Vector4.Zero,
                                                  null,
                                                  0.2f,
                                                  1, 7, 1, true, true, false,
                                                  null, null, MyGuiManager.GetScrollbarSlider(), null,
                                                  0, 0, Vector4.Zero, 0, 0, 0, 0, 0, 0, -0.007f);


            m_journalListbox.UseFullItemSizeIcon = true;
            m_journalListbox.TextOffset = new Vector2(0.005f, 0);
            m_journalListbox.InnerItemSize = new Vector2(325/1600f, 69/1200f);
            m_journalListbox.ItemMouseOverTexture = MyGuiManager.GetJournalSelected();

            LoadJournalListbox();

            m_journalListbox.ItemSelect += EventSelected;
            Controls.Add(m_journalListbox);                       

            LoadDescription();
        }                

        private void LoadJournalListbox()
        {
            m_journalListbox.RemoveAllRows();

            List<MyJournalCategory> categoryFilter = GetSelectedCategoryFilter();
            bool sameKey = false;
            for (int index = m_eventLog.Events.Count - 1; index >= 0; index--)
            {
                var logEntry = m_eventLog.Events[index];
                if (CanBeDisplayed(logEntry, categoryFilter))
                {
                    if (index == m_selectedEventKey)
                    {
                        sameKey = true;
                    }
                    StringBuilder name = logEntry.GetName();
                    if (name != null && name.Length != 0)
                    {
                        StringBuilder logEntryName = new StringBuilder();
                        logEntryName.Append(name);
                        const int maxEntryLength = 25;
                        if (logEntryName.Length > maxEntryLength)
                        {
                            logEntryName.Remove(maxEntryLength, logEntryName.Length - maxEntryLength);
                            logEntryName.Append("...");
                        }
                        //MyColoredText coloredText = new MyColoredText(logEntryName);
                        MyGuiControlListboxItem listboxItem = new MyGuiControlListboxItem(index, logEntryName, GetEventIcon(logEntry), 0.65f);
                        listboxItem.BackgroundColor = GetEventBackgroundColor(logEntry);

                        m_journalListbox.AddItem(listboxItem);
                        SetJournalItemColor(ref listboxItem, logEntry.Status);
                    }
                }
            }                        

            if(!sameKey)
            {
                m_selectedEventKey = null;
                LoadDescription();
            }
            else if(m_selectedEventKey != null)
            {
                m_journalListbox.SetSelectedItem(m_selectedEventKey.Value);
            }
        }

        private MyTexture2D GetEventIcon(MyEventLogEntry eventLogEntry)
        {

            return MyGuiEventHelpers.GetEventHelper(eventLogEntry.EventType).Icon;
        }

        private Vector4 GetEventBackgroundColor(MyEventLogEntry eventLogEntry)
        {

            return MyGuiEventHelpers.GetEventHelper(eventLogEntry.EventType).BackgroundColor;
        }


        private void SetJournalItemColor(ref MyGuiControlListboxItem listboxItem, StatusEnum logEntryStatus)
        {
            Color textColor = logEntryStatus == StatusEnum.Read ? new Color(MyGuiConstants.LISTBOX_TEXT_COLOR) : Color.LightYellow;
            Color highlightColor = new Color(textColor.ToVector4()*MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER);

            listboxItem.ColoredText.NormalColor = textColor;
            listboxItem.ColoredText.HighlightColor = highlightColor;
        }

        private List<MyJournalCategory> GetSelectedCategoryFilter()
        {
            List<MyJournalCategory> categoryFilter = new List<MyJournalCategory>();
            if (m_allEventsCheckbox.Checked)
            {
                categoryFilter.Add(MyJournalCategory.AllEvents);
            }
            if(m_globalEventsCheckbox.Checked)
            {
                categoryFilter.Add(MyJournalCategory.GlobalEvents);
            }
            //if(m_currentMissionCheckbox.Checked)
            //{
            //    categoryFilter.Add(MyJournalCategory.CurrentMission);                
            //}
            if(m_missionsCheckbox.Checked)
            {
                categoryFilter.Add(MyJournalCategory.AllMissions);
            }
            //if(m_accomplishedCheckbox.Checked)
            //{
            //    categoryFilter.Add(MyJournalCategory.AccomplishedMissions);                
            //}

            return categoryFilter;
        }

        private bool CanBeDisplayed(MyEventLogEntry eventLogEntry, List<MyJournalCategory> categoryFilter)
        {
            foreach (MyJournalCategory filter in categoryFilter)
            {
                Predicate<MyEventLogEntry> filterPredicate = m_filterConditions[(int) filter];
                if(filterPredicate(eventLogEntry))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnFilterChanged(MyGuiControlCheckbox sender)
        {
            if (sender == m_allEventsCheckbox && m_allEventsCheckbox.Checked)
            {
                m_globalEventsCheckbox.UnCheck();
                m_storyCheckbox.UnCheck();
                m_missionsCheckbox.UnCheck();
            }

            if (sender != m_allEventsCheckbox && sender.Checked)
            {
                m_allEventsCheckbox.UnCheck();
            }
            LoadJournalListbox();
        }

        private void LoadDescription()
        {
            if(m_selectedEventKey != null)
            { 
                m_descriptionText.Clear();
                MyEventLogEntry eventLogEntry = m_eventLog.Events[m_selectedEventKey.Value];
                //var name = logEntry.GetName();

                var name = eventLogEntry.GetName();


                StringBuilder logEntryName = new StringBuilder();
                //logEntryName.AppendFormat("{0}\n", eventLogEntry.Time);
                logEntryName.Clear();
                logEntryName.Append(name);
                logEntryName.AppendLine();

                m_descriptionText.AppendText(logEntryName, MyGuiManager.GetFontMinerWarsBlue(), 0.80f, Vector4.One);
                m_descriptionText.AppendImage(MyGuiManager.GetJournalLine(), new Vector2(370 / 1600f, 13 / 1200f), Vector4.One);

                StringBuilder description = new StringBuilder();
                StringBuilder desc = eventLogEntry.GetDescription();
                description.Append(desc != null ? desc.ToString() : "");
                description.AppendLine();

                if (MyMissions.ActiveMission != null && (int)MyMissions.ActiveMission.ID == eventLogEntry.EventTypeID)
                {
                    description.AppendLine();
                    foreach (MyObjective activeSubmission in MyMissions.ActiveMission.ActiveObjectives)
                    {
                        if (activeSubmission.NameTemp != null && activeSubmission.NameTemp.Length != 0)
                        {
                            description.Append(SUBMISSION_SEPARATOR);
                            description.AppendLine(activeSubmission.Name.ToString());
                        }
                    }
                }

                m_descriptionText.AppendText(description);
            }
            else
            {
                if(m_journalListbox.GetItemsCount() == 0)
                {
                    m_descriptionText.SetText(MyTextsWrapper.Get(MyTextsWrapperEnum.NoRecords));                    
                }
                else
                {
                    m_descriptionText.SetText(new StringBuilder());
                }                
            }
        }

        private void EventSelected(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            m_selectedEventKey = eventArgs.Key;

            // change color from unread to read
            MyEventLogEntry eventLogEntry = m_eventLog.Events[m_selectedEventKey.Value];            
            if (eventLogEntry.Status == StatusEnum.Unread)
            {
                eventLogEntry.Status = StatusEnum.Read;
                MyGuiControlListboxItem eventLbi = m_journalListbox.GetItem(m_selectedEventKey.Value);
                SetJournalItemColor(ref eventLbi, StatusEnum.Read);
            }    

            LoadDescription();
        }

        static List<StringBuilder> m_statsStrings = new List<StringBuilder>();
        static int m_stringIndex = 0;

        public static StringBuilder StringBuilderCache
        {
            get
            {
                if (m_stringIndex >= m_statsStrings.Count)
                    m_statsStrings.Add(new StringBuilder(1024));

                StringBuilder sb = m_statsStrings[m_stringIndex++];
                return sb.Clear();
            }
        }

        private static StringBuilder time = new StringBuilder(1024);

        public override bool Update(bool hasFocus)
        {
            time.GetFormatedDateTime(MySession.Static.GameDateTime);
            m_gameTimeLabel.UpdateText(time);

            //String.Format("{0:M/d/yyyy h:mm:ss}", MySession.Static.GameDateTime)
            return base.Update(hasFocus);
        }

        public void OnClose_Click(MyGuiControlButton sender)
        {            
            CloseScreen();
        }



        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);
            if (input.IsNewGameControlPressed(MyGameControlEnums.MISSION_DIALOG))
            {
                base.Canceling();
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenJournal";
        }
    }*/
}
