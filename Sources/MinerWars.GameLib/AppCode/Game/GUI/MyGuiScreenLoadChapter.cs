using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;
using MinerWars.AppCode.Networking.SectorService;
using System.ServiceModel;
using MinerWars.CommonLIB.AppCode.Networking.Services;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Journal;
using MinerWars.AppCode.Game.Gameplay;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenLoadChapter : MyGuiScreenBase
    {
        private MyGuiControlListbox m_listbox;

        public MyGuiScreenLoadChapter()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.95f, 0.8f))
        {
            m_enableBackgroundFade = true;

            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\VideoBackground", flags: TextureFlags.IgnoreQuality);
            AddCaption(MyTextsWrapperEnum.LoadChapter, new Vector2(0, 0.0075f));

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.05f, -m_size.Value.Y / 2.0f + 0.145f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.185f, -m_size.Value.Y / 2.0f + 0.145f);
            controlsOriginRight.X += 0.04f;

            /*
            // Checkpoint list
            m_listbox = new MyGuiControlListbox(this, new Vector2(-MyGuiConstants.LISTBOX_SCROLLBAR_WIDTH/2, -0.015f),
                MyGuiConstants.LISTBOX_LONGMEDIUM_SIZE, Vector4.Zero, null, 0.5f, 1, 18, 1, false, true, false);
            */

            m_listbox = new MyGuiControlListbox(this,
                                      new Vector2(0, -0.0125f),
                                      new Vector2(0.76f, 0.04f),
                                      MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
                                      MyTextsWrapper.Get(MyTextsWrapperEnum.LoadChapter),
                                      MyGuiConstants.LABEL_TEXT_SCALE,
                                      1, 13, 1,
                                      false, true, false,
                                      null, null, MyGuiManager.GetScrollbarSlider(), null,
                                      1, 1, MyGuiConstants.LISTBOX_BACKGROUND_COLOR_BLUE, 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);

            m_listbox.ItemDoubleClick += OnListboxItemDoubleClick;
            Controls.Add(m_listbox);


            Vector2 buttonSize = 0.75f * MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE;
            // Buttons (Load, Rename, Delete, Back)
            Vector2 buttonOrigin = new Vector2(-buttonSize.X * 1.5f - 0.001f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - buttonSize.Y / 2.0f - 0.033f);
            Vector2 buttonDelta = new Vector2(buttonSize.X + 0.001f, 0);
            Controls.Add(new MyGuiControlButton(this, buttonOrigin + buttonDelta * 0, buttonSize, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Load, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE * 0.8f, OnLoadClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, buttonOrigin + buttonDelta * 3, buttonSize, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Back, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE * 0.8f, OnBackClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            FillList();
        }

        private List<Tuple<string, DateTime, MyMwcObjectBuilder_Checkpoint>> m_chapters = new List<Tuple<string, DateTime, MyMwcObjectBuilder_Checkpoint>>();

        void FillList()
        {
            m_chapters = MinerWars.AppCode.Networking.MyLocalCache.LoadChapters().OrderByDescending(c => c.Item2).ToList();

            m_listbox.DeselectAll();
            m_listbox.RemoveAllRows();
            m_listbox.RemoveAllItems();
            int index = 0;
            for (int ind = 0; ind < m_chapters.Count; ind++)
            {
                var chapter = m_chapters[ind].Item3;

                string time = MyUtils.GetDatetimeAsSpentTime(m_chapters[ind].Item2);

                var name = new StringBuilder(time);
                name.Append(" - ");
                if (chapter.ActiveMissionID != -1)
                {
                    if (MyMissions.GetMissionByID((MyMissionID)chapter.ActiveMissionID) != null)
                    {
                        name.Append(MyTextsWrapper.Get(MyMissions.GetMissionByID((MyMissionID)chapter.ActiveMissionID).Name));
                    }
                    else
                        name.Append("<mission deleted>");
                }
                else
                {
                    name.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.FreeRoaming));
                }

                var lastEvent = GetLastEvent(chapter);
                if (lastEvent != null)
                {
                    name.Append(" - ");
                    if (MyMissions.GetMissionByID((MyMissionID)lastEvent.EventTypeID) != null)
                    {
                        name.Append(MyMissions.GetMissionByID((MyMissionID)lastEvent.EventTypeID).NameTemp);
                    }
                }

                var difficultyName = MyGameplayConstants.GetGameplayDifficultyProfile(chapter.SessionObjectBuilder.Difficulty).DifficultyName;

                name.Append(" - ");
                name.Append(MyTextsWrapper.Get(difficultyName));

                m_listbox.AddItem(index, name);
                index++;

            }
        }

        private static CommonLIB.AppCode.ObjectBuilders.SubObjects.MyMwcObjectBuilder_Event GetLastEvent(MyMwcObjectBuilder_Checkpoint chapter)
        {
            var lastEvent = chapter.EventLogObjectBuilder.Where(e => e.EventType == (int)EventTypeEnum.SubmissionFinished).OrderByDescending(e => e.Time).FirstOrDefault();
            return lastEvent;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenLoadCheckpoint";
        }

        public void OnLoadClick(MyGuiControlButton sender)
        {
            LoadCheckpoint();
        }

        public void OnBackClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        void OnListboxItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            LoadCheckpoint();
        }

        string GetCheckpointNameFromItem(MyGuiControlListboxItem item)
        {
            return item.Value.ToString();
        }

        private void LoadCheckpoint()
        {
            var item = m_listbox.GetSelectedItem();
            if (item != null && m_chapters.Count >= item.Key)
            {
                MyGuiManager.CloseAllScreensExcept(MyGuiScreenGamePlay.Static);

                string chapterName = m_chapters[item.Key].Item1; // TODO: dont construct the name, just take it from some array
                MinerWars.AppCode.Networking.MyLocalCache.ReplaceCurrentChapter(chapterName);
                MySession.StartLastCheckpoint();
            }
        }
    }
}
