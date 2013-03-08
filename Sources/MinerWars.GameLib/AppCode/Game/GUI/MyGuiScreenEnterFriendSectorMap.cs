using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenEnterFriendSectorMap : MyGuiScreenBase
    {
        MyGuiControlListbox m_mapsCombobox;
        MyTextsWrapperEnum m_startSessionProgressText;
        MyMwcStartSessionRequestTypeEnum m_startSessionType;
        List<MyMwcSectorIdentifier> m_sectorIdentifiers;
        List<MyMwcUserDetail> m_userDetails;
        MyGuiControlTextbox m_findPlayerName;
        MyGuiScreenBase m_closeAfterSuccessfulEnter;

        public Action<MyMwcSectorIdentifier> CustomLoadAction { get; set; }

        public MyGuiScreenEnterFriendSectorMap(MyMwcStartSessionRequestTypeEnum startSessionType, MyTextsWrapperEnum startSessionProgressText, MyGuiScreenBase closeAfterSuccesfullEnter)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(700f / 1600f, 700 / 1200f))
        {
            m_startSessionType = startSessionType;
            m_startSessionProgressText = startSessionProgressText;
            m_enableBackgroundFade = true;
            m_sectorIdentifiers = null;
            m_closeAfterSuccessfulEnter = closeAfterSuccesfullEnter;
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ConfigWheelBackground", flags: TextureFlags.IgnoreQuality);
            RecreateControls();

        }

        private void RecreateControls()
        {
            m_size = new Vector2(1100f / 1600f, 1200 / 1200f);

            Controls.Clear();

            string prevName = MyConfig.LastFriendName;
            if (m_findPlayerName != null && m_findPlayerName.Text != null)
            {
                prevName = m_findPlayerName.Text;
            }

            AddCaption(MyTextsWrapperEnum.FriendsSectorMap, new Vector2(0, 0.04f));

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.1f, -m_size.Value.Y / 2.0f + 0.1f);
            Vector2 controlsDelta = new Vector2(0, 0.0525f);

            // controls for typing friend name to search
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1f * controlsDelta, null, MyTextsWrapperEnum.FriendName, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_findPlayerName = new MyGuiControlTextbox(this, controlsOriginLeft + 1f * controlsDelta + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f + 0.15f, 0), MyGuiControlPreDefinedSize.MEDIUM, prevName, 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_findPlayerName);

            var searchButton = new MyGuiControlButton(this, m_findPlayerName.GetPosition() + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f - 0.0208f, 0), MyGuiConstants.SEARCH_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetButtonSearchTexture(), null, null, MyTextsWrapperEnum.Search,
                MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE * 0.8f, MyGuiControlButtonTextAlignment.CENTERED, OnSearchClick,
                true, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, true, true);
            Controls.Add(searchButton);

            // friend maps available for selection
            //Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 3* controlsDelta, null, MyTextsWrapperEnum.Map, MyGuiConstants.LABEL_TEXT_COLOR,
            //    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            if (m_sectorIdentifiers != null && m_userDetails != null && m_sectorIdentifiers.Count > 0)
            {
                m_mapsCombobox = new MyGuiControlListbox(this, new Vector2(0, -0.010f), new Vector2(0.43f, 0.1f), MyGuiConstants.LISTBOX_BACKGROUND_COLOR, null, MyGuiConstants.LABEL_TEXT_SCALE, 1, 6, 1, true, true, false,
                    null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 1, 0, MyGuiConstants.LISTBOX_BACKGROUND_COLOR_BLUE, 0f, 0f, 0f, 0f, 0, 0f, -0.01f, -0.01f, -0.02f, 0.02f);

                for (int i = 0; i < m_sectorIdentifiers.Count; i++)
                {
                    MyMwcSectorIdentifier sectorIdentifier = m_sectorIdentifiers[i];
                    foreach (MyMwcUserDetail userDetail in m_userDetails)
                    {
                        if (sectorIdentifier.UserId.HasValue && sectorIdentifier.UserId.Value == userDetail.UserId)
                        {
                            if (string.IsNullOrEmpty(sectorIdentifier.SectorName))
                            {
                                m_mapsCombobox.AddItem(i, new StringBuilder(string.Format("{0} - {1}", userDetail.DisplayName, sectorIdentifier.Position.ToString())), null);
                            }
                            else
                            {
                                m_mapsCombobox.AddItem(i, new StringBuilder(string.Format("{0} - {1} ({2})", userDetail.DisplayName, sectorIdentifier.SectorName, sectorIdentifier.Position.ToString())), null);
                            }
                        }
                    }
                }

                SortSectors();
                m_mapsCombobox.ItemDoubleClick += OnItemDoubleClick;
                Controls.Add(m_mapsCombobox);
            }
            else
            {
                Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 3 * controlsDelta + new Vector2(0.21f, 0), null, MyTextsWrapperEnum.NoSectorsAvailable, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            }

            //  Buttons OK and CANCEL
            var m_okButton = new MyGuiControlButton(this, new Vector2(-0.0879f, 0.35f), MyGuiConstants.OK_BUTTON_SIZE * 0.75f,
                       MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                       MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Ok,
                       MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnOkClick,
                       true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_okButton);

            var m_cancelButton = new MyGuiControlButton(this, new Vector2(0.0879f, 0.35f), MyGuiConstants.OK_BUTTON_SIZE * 0.75f,
                       MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                       MyGuiManager.GetInventoryScreenButtonTexture(), null, null, MyTextsWrapperEnum.Cancel,
                       MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlButtonTextAlignment.CENTERED, OnCancelClick,
                       true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true, true);
            Controls.Add(m_cancelButton);

            m_findPlayerName.MoveCartrigeToEnd();
            this.SetControlIndex(Controls.IndexOf(m_findPlayerName));
        }

        private void SortSectors()
        {
            // Named sectors first
            m_mapsCombobox.CustomSortRows((a, b) =>
            {
                var sectorA = m_sectorIdentifiers[a.Key];
                var sectorB = m_sectorIdentifiers[b.Key];

                if (sectorA.UserId != sectorB.UserId)
                {
                    return sectorA.UserId.Value.CompareTo(sectorB.UserId.Value);
                }

                bool aName = string.IsNullOrEmpty(m_sectorIdentifiers[a.Key].SectorName);
                bool bName = string.IsNullOrEmpty(m_sectorIdentifiers[b.Key].SectorName);

                return aName == bName ?
                    a.Value.ToString().CompareTo(b.Value.ToString()) :
                    (aName ? 1 : -1);
            }, 0);
        }

        public void AddFriendSectorsResponse(List<MyMwcSectorIdentifier> sectorIdentifiers, List<MyMwcUserDetail> userDetails)
        {
            m_sectorIdentifiers = sectorIdentifiers;
            m_userDetails = userDetails;
            RecreateControls();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEnterFriendSectorMap";
        }

        public void OnSearchClick(MyGuiControlButton sender)
        {
            if (m_findPlayerName.Text.Length == 1)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.EnterAtLeastTwoCharacters, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, MsgBoxCallback));
                return;
            }

            MyConfig.LastFriendName = m_findPlayerName.Text ?? String.Empty;
            MyConfig.Save();

            var criterium = MyMwcSelectSectorRequestTypeEnum.FIND_BY_PLAYER_NAME_FULLTEXT;

            if (String.IsNullOrEmpty(m_findPlayerName.Text))
            {
                criterium = MyMwcSelectSectorRequestTypeEnum.RANDOM_FRIENDS;
            }

            MyGuiManager.AddScreen(new MyGuiScreenSelectSandboxProgress(criterium, MyTextsWrapperEnum.SelectSandboxInProgressPleaseWait,
                this, m_findPlayerName.Text, AddFriendSectorsResponse));

            //CloseScreen();
        }

        void MsgBoxCallback(MyGuiScreenMessageBoxCallbackEnum result)
        {
            RecreateControls();
        }

        public void OnItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            OnOkClick(null);
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            if (m_mapsCombobox.GetSelectedItemKey() == null)
                return;

            //AddFriendSectorsResponse not called?
            if (m_sectorIdentifiers == null)
                return;

            MyMwcSectorIdentifier sectorIdentifier = m_sectorIdentifiers[m_mapsCombobox.GetSelectedItemKey().Value];
            MyConfig.LastFriendName = m_findPlayerName.Text;
            MyConfig.LastFriendSectorPosition = sectorIdentifier.Position;
            MyConfig.LastFriendSectorUserId = sectorIdentifier.UserId.Value;
            MyConfig.Save();

            MyGuiManager.CloseAllScreensNowExcept(MyGuiScreenGamePlay.Static);

            if (CustomLoadAction != null)
            {
                CustomLoadAction(sectorIdentifier);
            }
            else
            {
                MyGuiManager.AddScreen(new MyGuiScreenStartSessionProgress(m_startSessionType, m_startSessionProgressText, sectorIdentifier, MyGameplayDifficultyEnum.EASY, null, m_closeAfterSuccessfulEnter));
            }
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }
}