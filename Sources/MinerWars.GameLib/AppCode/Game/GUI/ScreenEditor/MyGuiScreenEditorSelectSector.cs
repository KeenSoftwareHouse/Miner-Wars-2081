using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
//using MinerWars.CommonLIB.MasterServerService;
using System.IO;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;
using MinerWars.AppCode.Networking.SectorService;
using System.ServiceModel;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorSelectSector : MyGuiScreenBase
    {
        MyGuiControlCombobox m_mapsCombobox;
        List<MyMwcSectorIdentifier> m_sectorIdentifiers;
        List<MyMwcUserDetail> m_userDetails;
        MyGuiControlTextbox m_findPlayerName;

        public MyGuiScreenEditorSelectSector()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.5f, 0.5f))
        {
            m_enableBackgroundFade = true;
            m_sectorIdentifiers = null;
            RecreateControls();
        }

        private void AddSectorToCombo(MyMwcSectorIdentifier sectorIdentifier, int index, string username)
        {
            if (string.IsNullOrEmpty(sectorIdentifier.SectorName))
            {
                m_mapsCombobox.AddItem(index, new StringBuilder(string.Format("{0} ({1})",
                    username, sectorIdentifier.Position.ToString())));
            }
            else
            {
                m_mapsCombobox.AddItem(index, new StringBuilder(string.Format("{0} {1} ({2})",
                    username, sectorIdentifier.SectorName, sectorIdentifier.Position.ToString())));
            }
        }

        private void RecreateControls()
        {
            Controls.Clear();

            AddCaption(new StringBuilder("Select Sector"), MyGuiConstants.SCREEN_CAPTION_TEXT_COLOR);

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.04f, -m_size.Value.Y / 2.0f + 0.08f);
            Vector2 controlsDelta = new Vector2(0, 0.0525f);

            // controls for typing friend name to search
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 1 * controlsDelta, null, MyTextsWrapperEnum.FriendName, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_findPlayerName = new MyGuiControlTextbox(this, controlsOriginLeft + 2 * controlsDelta + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_findPlayerName);

            // search button
            Controls.Add(new MyGuiControlButton(this, m_findPlayerName.GetPosition() + new Vector2(0.2f, 0), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Search, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnSearchClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            // friend maps available for selection
            Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 4 * controlsDelta, null, MyTextsWrapperEnum.Map, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            if (m_sectorIdentifiers != null && m_userDetails != null)
            {
                m_mapsCombobox = new MyGuiControlCombobox(this, controlsOriginLeft + 5 * controlsDelta + new Vector2(MyGuiConstants.COMBOBOX_LONGMEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.LONGMEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
                for (int i = 0; i < m_sectorIdentifiers.Count; i++)
                {
                    MyMwcSectorIdentifier sectorIdentifier = m_sectorIdentifiers[i];
                    if (!sectorIdentifier.UserId.HasValue)
                    {
                        AddSectorToCombo(sectorIdentifier, i, "STORY");
                    }
                    else
                    {
                        foreach (MyMwcUserDetail userDetail in m_userDetails)
                        {
                            if (sectorIdentifier.UserId.HasValue && sectorIdentifier.UserId.Value == userDetail.UserId)
                            {
                                AddSectorToCombo(sectorIdentifier, i, userDetail.DisplayName);
                            }
                        }
                    }
                }

                SortSectors();

                m_mapsCombobox.SelectItemByIndex(0);
                Controls.Add(m_mapsCombobox);
            }
            else
            {
                Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft + 5 * controlsDelta, null, MyTextsWrapperEnum.NoSectorsAvailable, MyGuiConstants.LABEL_TEXT_COLOR,
                MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            }

            //  Buttons OK and CANCEL
            Vector2 buttonDelta = new Vector2(0.1f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f);
            if (m_mapsCombobox != null)
            {
                Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
                Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            }
            else
            {
                Controls.Add(new MyGuiControlButton(this, new Vector2(0, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            }
        }

        private void SortSectors()
        {
            // Named sectors first
            m_mapsCombobox.CustomSortItems((a, b) =>
            {
                bool aName = string.IsNullOrEmpty(m_sectorIdentifiers[a.Key].SectorName);
                bool bName = string.IsNullOrEmpty(m_sectorIdentifiers[b.Key].SectorName);

                return aName == bName ?
                    a.Value.ToString().CompareTo(b.Value.ToString()) :
                    (aName ? 1 : -1);
            });
        }

        public void AddSectorsResponse(List<MyMwcSectorIdentifier> sectorIdentifiers, List<MyMwcUserDetail> userDetails)
        {
            if (sectorIdentifiers != null && sectorIdentifiers.Count > 0)
            {
                m_sectorIdentifiers = sectorIdentifiers;
                m_userDetails = userDetails;
                RecreateControls();
            }
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorSelectSector";
        }

        public void OnSearchClick(MyGuiControlButton sender)
        {
            if (m_findPlayerName != null && m_findPlayerName.Text != null && m_findPlayerName.Text.Length > 0)
            {
                MyGuiManager.AddScreen(new MyGuiScreenSelectSandboxProgress(MyMwcSelectSectorRequestTypeEnum.FIND_BY_PLAYER_NAME, MyTextsWrapperEnum.LoadingPleaseWait,
                    this, m_findPlayerName.Text, AddSectorsResponse));
            }
            else
            {
                MyGuiManager.AddScreen(new MyGuiScreenSelectSandboxProgress(MyMwcSelectSectorRequestTypeEnum.STORY, MyTextsWrapperEnum.LoadingPleaseWait,
                    this, null, AddSectorsResponse));
            }
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            CloseScreen();
            MyMwcSectorIdentifier sectorIdentifier = m_sectorIdentifiers[m_mapsCombobox.GetSelectedKey()];
            MyGuiManager.AddScreen(new MyGuiScreenEditorLoadObjectGroupsProgress(MyTextsWrapperEnum.LoadingPleaseWait, sectorIdentifier));
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
            MyGuiManager.AddScreen(new MyGuiScreenEditorGroups());
        }
    }

    class MyGuiScreenEditorLoadObjectGroupsProgress : MyGuiScreenSectorServiceCallProgress
    {
        MyMwcSectorIdentifier m_sectorIdentifier;

        //MyMasterServerServiceClient client = MyMasterServerServiceClient.CreateInstance();
        //System.IAsyncResult saveResult;

        //  Using this static public property client-server tells us about login response
        public static MyGuiScreenEditorLoadObjectGroupsProgress CurrentScreen = null;    //  This is always filled with reference to actual instance of this scree. If there isn't, it's null.

        public MyGuiScreenEditorLoadObjectGroupsProgress(MyTextsWrapperEnum loadingText, MyMwcSectorIdentifier sectorIdentifier)
            : base(loadingText, false, TimeSpan.FromSeconds(360))
        {
            CurrentScreen = this;
            m_sectorIdentifier = sectorIdentifier;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorLoadObjectGroupsProgress";
        }

        protected override void ServiceProgressStart(MySectorServiceClient client)
        {
            //  Send save request and wait for callback
            AddAction(client.BeginLoadObjectGroups(m_sectorIdentifier.SectorType, m_sectorIdentifier.UserId, m_sectorIdentifier.Position, null, client));
        }

        protected override void OnActionCompleted(IAsyncResult asyncResult, MySectorServiceClient client)
        {
            try
            {
                var response = client.EndLoadObjectGroups(asyncResult);

                List<MyMwcObjectBuilder_ObjectGroup> groups = new List<MyMwcObjectBuilder_ObjectGroup>();
                List<MyMwcObjectBuilder_Base> entities = new List<MyMwcObjectBuilder_Base>();

                var fakeEndpoint = new System.Net.IPEndPoint(0, MyClientServer.LoggedPlayer.GetUserId());

                var sectorGroupBuilder = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_SectorObjectGroups>(response);
                if (sectorGroupBuilder == null)
                {
                    throw new InvalidOperationException("Cannot deserialize SectorObjectGroups object builder");
                }
                MyGuiManager.AddScreen(new MyGuiScreenEditorLoadGroup(sectorGroupBuilder.Groups, sectorGroupBuilder.Entities));
            }
            catch (Exception)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.PleaseTryAgain, MyTextsWrapperEnum.MessageBoxNetworkErrorCaption, MyTextsWrapperEnum.Ok, null));
            }
            CloseScreen();
        }

        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            //  Only continue if this screen is really open (not closing or closed)
            if (GetState() != MyGuiScreenState.OPENED) return false;

            return true;
        }

        public override bool CloseScreen()
        {
            bool ret = base.CloseScreen();

            if (ret == true)
            {
                CurrentScreen = null;
            }

            return ret;
        }
    }
}

