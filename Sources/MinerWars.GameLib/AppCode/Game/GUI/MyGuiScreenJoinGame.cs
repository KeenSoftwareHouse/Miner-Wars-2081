using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Textures;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Networking;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenJoinGame : MyGuiScreenBase
    {
        enum MyGameTableHeaderEnum
        {
            //Ping,
            HostName,
            SectorName,
            GameType,
            PlayerCount,
            //JoinMode,
        }

        static readonly MyGameTableHeaderEnum[] m_gameTableHeaders = (MyGameTableHeaderEnum[])Enum.GetValues(typeof(MyGameTableHeaderEnum));

        static int HeaderCount
        {
            get { return m_gameTableHeaders.Length; }
        }

        enum MySectorType
        {
            Official,
            Players,
            Friends,
        }



        struct MyGameExtendedInfo
        {
            public readonly MyGameInfo GameInfo;
            public int Ping;
            public MySectorType SectorType;
            public MyGameTypes GameType;

            public MyGameExtendedInfo(MyGameInfo gameInfo)
            {
                GameInfo = gameInfo;
                Ping = MyMwcUtils.GetRandomInt(10, 20);
                SectorType = MySectorType.Official;
                GameType = gameInfo.GameType;
            }
        }

        MyGuiScreenBase m_closeAfterSuccessfulEnter;

        readonly MyGuiControlListbox m_gameList;
        readonly MyGuiControlTextbox m_searchTextbox;
        readonly List<MyGameExtendedInfo> m_games;
        readonly MyGuiControlCheckbox m_deathCheck;
        readonly MyGuiControlCheckbox m_storyCheck;


        int? m_selectRow = null;
        int? m_selectColumn = null;

        MyGameTableHeaderEnum m_orderByHeader = MyGameTableHeaderEnum.PlayerCount;
        bool m_orderAsc = false;

        int m_selectedGameIndex;

        /// <summary>
        /// Locker used for populating the listbox with games.
        /// </summary>
        private readonly object m_repopulateLocker = new object();

        MyGuiScreenWaiting m_waitingScreen;

        private MyGameTypes m_gameTypeFilter;

        private ConnectionHandler m_serverDisconnectedHandler;
        private MyGuiScreenWaiting m_waitDialog;

        public MyGuiScreenJoinGame(MyGuiScreenBase closeAfterSuccessfulEnter, MyGameTypes gameTypeFilter)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.95f, 0.8f))
        {
            m_size = new Vector2(0.95f, 0.85f);
            m_serverDisconnectedHandler = new ConnectionHandler(Static_ServerDisconnected);

            m_gameTypeFilter = gameTypeFilter;
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\VideoBackground", flags: TextureFlags.IgnoreQuality);

            m_closeAfterSuccessfulEnter = closeAfterSuccessfulEnter;
            m_enableBackgroundFade = true;
            m_games = new List<MyGameExtendedInfo>();
            AddCaption(MyTextsWrapperEnum.JoinGame, new Vector2(0, 0.0075f));

            Vector2 menuPositionOrigin = new Vector2(-0.31f, -m_size.Value.Y / 2.0f + 0.15f);
            Vector2 buttonDelta = new Vector2(0.22f, 0);
            const MyGuiControlButtonTextAlignment menuButtonTextAlignement = MyGuiControlButtonTextAlignment.CENTERED;

            Controls.Add(new MyGuiControlLabel(this, menuPositionOrigin, null, MyTextsWrapperEnum.SearchGameToJoin, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

            m_searchTextbox = new MyGuiControlTextbox(this, menuPositionOrigin + buttonDelta, MyGuiControlPreDefinedSize.LARGE, "", 40, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            m_searchTextbox.TextChanged += OnSearchTextChanged;
            Controls.Add(m_searchTextbox);

            menuPositionOrigin += new Vector2(0.395f, 0);

            var refreshButton = new MyGuiControlButton(
                this,
                menuPositionOrigin + buttonDelta,
                MyGuiConstants.BACK_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Refresh,
                MyGuiConstants.BUTTON_TEXT_COLOR,
                MyGuiConstants.BUTTON_TEXT_SCALE,
                OnRefreshButtonClick,
                menuButtonTextAlignement,
                true,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true);
            Controls.Add(refreshButton);

            menuPositionOrigin.Y += 0.052f;
            menuPositionOrigin.X = -0.33f;

            var storyLabel = new MyGuiControlLabel(this, menuPositionOrigin, null, MyTextsWrapperEnum.Story, Vector4.One, 1.0f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(storyLabel);

            menuPositionOrigin.X = -0.35f;
            m_storyCheck = new MyGuiControlCheckbox(this, menuPositionOrigin, true, Vector4.One, null);
            m_storyCheck.OnCheck = CheckChanged;
            Controls.Add(m_storyCheck);

            menuPositionOrigin.X = -0.2f;
            var deathLabel = new MyGuiControlLabel(this, menuPositionOrigin, null, MyTextsWrapperEnum.Deathmatch, Vector4.One, 1.0f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            Controls.Add(deathLabel);

            menuPositionOrigin.X = -0.22f;
            m_deathCheck = new MyGuiControlCheckbox(this, menuPositionOrigin, true, Vector4.One, null);
            m_deathCheck.OnCheck = CheckChanged;
            Controls.Add(m_deathCheck);

            //var storyButton = new MyGuiControlButton(this, menuPositionOrigin, null, Vector4.One, MyTextsWrapperEnum.Story, Vector4.One, 1.0f, 

            menuPositionOrigin.Y += 0.25f;
            menuPositionOrigin.X = 0;
            m_gameList = new MyGuiControlListbox(this,
                menuPositionOrigin,
                new Vector2(0.22f, 0.04f),
                MyGuiConstants.LISTBOX_BACKGROUND_COLOR,
                MyTextsWrapper.Get(MyTextsWrapperEnum.JoinGame),
                MyGuiConstants.LABEL_TEXT_SCALE,
                HeaderCount, 10, HeaderCount,
                true, true, false,
                null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 2, 1, MyGuiConstants.LISTBOX_BACKGROUND_COLOR_BLUE, 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);
            m_gameList.ItemSelect += OnGamesItemSelect;
            m_gameList.DisplayHighlight = true;
            m_gameList.MultipleSelection = true;
            m_gameList.ItemDoubleClick += GameListOnItemDoubleClick;
            m_gameList.HighlightHeadline = true;
            m_gameList.EnableAllRowHighlightWhileMouseOver(true, true);
            m_gameList.SetCustomCollumnsWidths(new List<float>()
            {
                0.15f, 0.30f, 0.15f, 0.10f
            });
            Controls.Add(m_gameList);

            buttonDelta = new Vector2(0.1f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f - 0.06f);
            Controls.Add(new MyGuiControlButton(this, new Vector2(-buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR, MyTextsWrapperEnum.Ok, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnOkClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));
            Controls.Add(new MyGuiControlButton(this, new Vector2(+buttonDelta.X, buttonDelta.Y), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE,
                MyGuiConstants.BUTTON_BACKGROUND_COLOR, MyTextsWrapperEnum.Cancel, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                OnCancelClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true));

            AddHeaders();
        }

        public void CheckChanged(MyGuiControlCheckbox sender)
        {
            m_gameTypeFilter &= ~MyGameTypes.Deathmatch;
            m_gameTypeFilter &= ~MyGameTypes.Story;

            if (m_deathCheck.Checked)
                m_gameTypeFilter |= MyGameTypes.Deathmatch;

            if (m_storyCheck.Checked)
                m_gameTypeFilter |= MyGameTypes.Story;

            GetDataFromServer(true);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            GetDataFromServer(true);
        }

        void OnRefreshButtonClick(MyGuiControlButton sender)
        {
            GetDataFromServer(true);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenJoinGame";
        }

        void OnOkClick(MyGuiControlButton sender)
        {
            TryJoinGame();
        }

        void GameListOnItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            if (eventArgs.RowIndex > 0)
            {
                TryJoinGame();
            }
        }

        void TryJoinGame()
        {
            if (m_selectedGameIndex < m_games.Count)
            {
                var gameInfo = m_games[m_selectedGameIndex].GameInfo;

                if (!MinerWars.AppCode.Game.World.MyClientServer.MW25DEnabled && (gameInfo.Name.ToUpper().Contains("2,5D") || gameInfo.Name.ToUpper().Contains("2.5D")))
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, MyTextsWrapperEnum.YouNeed25D, MyTextsWrapperEnum.MessageBoxCaptionFeatureDisabled, MyTextsWrapperEnum.Ok, null));
                    return;
                }

                MyMultiplayerLobby.Static.JoinGame(gameInfo, String.Empty, OnGameJoined, OnGameEnterDisallowed, OnDownloadingSector);
                // Longer join timeout...to be able to join after loading
                m_waitingScreen = new MyGuiScreenWaiting(MyTextsWrapperEnum.JoiningGame, OnJoiningCancelOrTimeout, TimeSpan.FromSeconds(120));
                MyGuiManager.AddScreen(m_waitingScreen);
            }
        }

        void OnDownloadingSector()
        {
            if (m_waitingScreen != null)
            {
                m_waitingScreen.CloseScreenNow();
                m_waitingScreen = new MyGuiScreenWaiting(MyTextsWrapperEnum.DownloadingData, OnJoiningCancelOrTimeout);
                MyGuiManager.AddModalScreen(m_waitingScreen, null);
            }
        }

        void OnJoiningCancelOrTimeout(object sender, WaitingCanceledArgs waitingCanceledArgs)
        {
            MyMultiplayerPeers.Static.Shutdown();

            switch (waitingCanceledArgs.CancelReason)
            {
                case CancelReasonEnum.UserCancel:
                    // do nothing
                    break;
                case CancelReasonEnum.Timeout:
                    MyGuiScreenMessageBox.Show(MyTextsWrapperEnum.JoinGameTimeout, MyTextsWrapperEnum.JoinGame, MyMessageBoxType.ERROR);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GetDataFromServer(true);
        }

        void OnGameEnterDisallowed()
        {
            Debug.Assert(m_waitingScreen != null);
            m_waitingScreen.CloseScreen();

            MyGuiManager.AddModalScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.DenyEnter, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null), null);
        }

        void OnGameJoined(MyGameInfo game, MyResultCodeEnum resultCode, MyMwcObjectBuilder_Checkpoint checkpointBuilder)
        {
            Debug.Assert(m_waitingScreen != null);
            m_waitingScreen.CloseScreen();

            if (resultCode == MyResultCodeEnum.OK)
            {
                var loadingScreen = MySession.StartJoinMultiplayerSession(game.GameType, game.Difficulty, checkpointBuilder);
                loadingScreen.Closed += new MyGuiScreenBase.ScreenHandler(OnLoadFinished);
            }
            else if (resultCode == MyResultCodeEnum.GAME_NOT_EXISTS)
            {
                MyGuiManager.AddModalScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.MP_GameHasEnded, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null), null);
                GetDataFromServer(true);
            }
            else
            {
                MyGuiManager.AddModalScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.ErrorCreatingNetworkConnection, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null), null);
                GetDataFromServer(true);
            }
        }

        void OnLoadFinished(MyGuiScreenBase screen)
        {

        }


        void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        private void GetDataFromServer(bool withWaitDialog)
        {
            m_games.Clear();

            if (MySteam.IsActive && !MySteam.IsOnline)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.SteamInvalidTicketText, MyTextsWrapperEnum.SteamInvalidTicketCaption, MyTextsWrapperEnum.Ok, null));
                return;
            }

            try
            {
                if (m_gameTypeFilter != MyGameTypes.None)
                {
                    MyMultiplayerPeers.Static.ServerDisconnected -= m_serverDisconnectedHandler;
                    MyMultiplayerLobby.Static.GetGames(OnGetGames, m_searchTextbox.Text, m_gameTypeFilter);
                    if (withWaitDialog)
                    {
                        MyMultiplayerPeers.Static.ServerDisconnected += m_serverDisconnectedHandler;
                        m_waitDialog = new MyGuiScreenWaiting(MyTextsWrapperEnum.LoadFromServer, new EventHandler<WaitingCanceledArgs>(WaitCanceled), TimeSpan.FromSeconds(30));
                        MyGuiManager.AddModalScreen(m_waitDialog, null);
                    }
                }
                else
                {
                    OnGetGames(new List<MyGameInfo>());
                }
            }
            catch (Exception exception)
            {
                HandleError(new StringBuilder("Cannot connect to server"));
                MyMwcLog.WriteLine("Cannot connect to server:");
                MyMwcLog.WriteLine(exception);
                MyMultiplayerPeers.Static.Shutdown();
            }
        }

        void WaitCanceled(object sender, WaitingCanceledArgs args)
        {
            MyMultiplayerPeers.Static.ServerDisconnected -= m_serverDisconnectedHandler;
            MyMultiplayerLobby.Static.CancelGetGames();

            if (args.CancelReason == CancelReasonEnum.Timeout)
            {
                ShowNetworkError();
            }
        }

        void Static_ServerDisconnected(Lidgren.Network.NetConnection connection)
        {
            MySteam.RefreshSessionTicket();

            if (m_waitDialog != null)
            {
                m_waitDialog.CloseScreen();
                m_waitDialog = null;
            }
            MyMultiplayerPeers.Static.ServerDisconnected -= m_serverDisconnectedHandler;
            ShowNetworkError();
        }

        private static void ShowNetworkError()
        {
            var caption = MyTextsWrapper.Get(MyTextsWrapperEnum.MessageBoxNetworkErrorCaption);
            var msg = new StringBuilder(MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.MP_CannotConnectServerJoin, MinerWars.CommonLIB.AppCode.Utils.MyMwcNetworkingConstants.NETWORKING_PORT_MASTER_SERVER));
            MyGuiManager.AddModalScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, msg, caption, MyTextsWrapperEnum.Ok, null), null);
        }

        void OnGetGames(List<MyGameInfo> gamesList)
        {
            MyMultiplayerPeers.Static.ServerDisconnected -= m_serverDisconnectedHandler;
            if (m_waitDialog != null)
            {
                m_waitDialog.CloseScreen();
                m_waitDialog = null;
            }

            lock (m_repopulateLocker)
            {
                if (!FillGamesFromLobbiesInfo(gamesList))
                    return;

                OrderGames();
                RefreshGameList();
            }
        }

        void HandleError(StringBuilder text)
        {
            MyGuiManager.AddModalScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.ErrorCreatingNetworkConnection, MyTextsWrapperEnum.MessageBoxCaptionError, MyTextsWrapperEnum.Ok, null), null);
        }

        private bool RefreshGameList()
        {
            m_gameList.RemoveAllRows();
            AddHeaders();

            foreach (MyGameExtendedInfo gameInfo in m_games)
            {
                AddGame(gameInfo);
            }

            return true;
        }

        private void OrderGames()
        {
            if (m_games == null) return;

            m_games.Sort(CompareByHeaderType);
        }

        int CompareByHeaderType(MyGameExtendedInfo x, MyGameExtendedInfo y)
        {
            int resultForAscending;

            switch (m_orderByHeader)
            {
                //case MyGameTableHeaderEnum.Ping:
                //    resultForAscending = x.Ping.CompareTo(y.Ping);
                //    break;
                case MyGameTableHeaderEnum.HostName:
                    resultForAscending = String.CompareOrdinal(x.GameInfo.HostDisplayName, y.GameInfo.HostDisplayName);
                    break;
                case MyGameTableHeaderEnum.SectorName:
                    resultForAscending = String.CompareOrdinal(x.GameInfo.Name, y.GameInfo.Name);
                    break;
                case MyGameTableHeaderEnum.GameType:
                    resultForAscending = ((int)x.GameType).CompareTo(((int)y.GameType));
                    break;
                case MyGameTableHeaderEnum.PlayerCount:
                    resultForAscending = x.GameInfo.PlayerCount.CompareTo(y.GameInfo.PlayerCount);
                    break;
                    //case MyGameTableHeaderEnum.JoinMode:
                    //    resultForAscending = x.GameInfo.JoinMode.CompareTo(y.GameInfo.JoinMode);
                    break;
                default:
                    throw new IndexOutOfRangeException("m_orderByHeader");
            }

            if (m_orderAsc)
            {
                return resultForAscending;
            }
            else
            {
                return -resultForAscending;
            }
        }

        private bool FillGamesFromLobbiesInfo(List<MyGameInfo> games)
        {
            if (games == null || m_games == null) return false;

            m_games.Clear();

            foreach (MyGameInfo game in games)
            {
                if (game.Name.StartsWith("**"))
                    continue;

                if (MyMwcSectorIdentifier.Is25DSector(game.Name))
                {
                    if (MyClientServer.MW25DEnabled)
                    {
                        m_games.Add(new MyGameExtendedInfo(game));
                    }
                }
                else
                {
                    if (MyClientServer.HasFullGame)
                    {
                        m_games.Add(new MyGameExtendedInfo(game));
                    }
                }
            }

            return true;
        }

        private void AddHeaders()
        {
            int rowIndex = m_gameList.AddRow();

            for (int currentHeader = 0; currentHeader < HeaderCount; currentHeader++)
            {
                AddHeader(rowIndex, currentHeader);
            }
        }

        void AddHeader(int rowIndex, int currentColumn)
        {
            m_gameList.AddItem(
                GetHeaderKey(currentColumn, rowIndex),
                GetHeaderText(m_gameTableHeaders[currentColumn]),
                null,
                rowIndex,
                currentColumn);
        }

        static StringBuilder GetHeaderText(MyGameTableHeaderEnum headerType)
        {
            switch (headerType)
            {
                //case MyGameTableHeaderEnum.Ping:
                //    return MyTextsWrapper.Get(MyTextsWrapperEnum.HeaderPing);
                case MyGameTableHeaderEnum.HostName:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.HeaderHostName);
                case MyGameTableHeaderEnum.SectorName:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.HeaderSectorName);
                case MyGameTableHeaderEnum.GameType:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.HeaderType);
                case MyGameTableHeaderEnum.PlayerCount:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.HeaderPlayerCount);
                //case MyGameTableHeaderEnum.JoinMode:
                //    return new StringBuilder("JoinMode");
                default:
                    throw new ArgumentOutOfRangeException("headerType");
            }
        }

        static int GetHeaderKey(int currentColumn, int rowIndex)
        {
            return rowIndex * HeaderCount + currentColumn;
        }

        private bool AddGame(MyGameExtendedInfo gameInfo)
        {
            if (m_gameList == null) return false;

            StringBuilder pingText = new StringBuilder(6);
            pingText.Append("? "); // TODO: measure ping
            pingText.Append("ms");
            int rowIndex = m_gameList.AddRow();
            //m_gameList.AddItem(rowIndex * (HeaderCount) + 0, pingText, null, rowIndex, 0);
            m_gameList.AddItem(rowIndex * (HeaderCount) + 0, new StringBuilder(gameInfo.GameInfo.HostDisplayName), null, rowIndex, 0);
            m_gameList.AddItem(rowIndex * (HeaderCount) + 1, new StringBuilder(gameInfo.GameInfo.Name), GetSectorIcon(gameInfo.GameInfo), rowIndex, 1);
            m_gameList.AddItem(rowIndex * (HeaderCount) + 2, GetGameType(gameInfo.GameInfo), null, rowIndex, 2);
            m_gameList.AddItem(rowIndex * (HeaderCount) + 3, new StringBuilder(gameInfo.GameInfo.PlayerCount + "/" + gameInfo.GameInfo.MaxPlayerCount), null, rowIndex, 3);
            //m_gameList.AddItem(rowIndex * (HeaderCount-1) + 5, new StringBuilder(gameInfo.GameInfo.JoinMode.ToString()), null, rowIndex, 5);

            return true;
        }

        private MyTexture2D GetSectorIcon(MyGameInfo gameInfo)
        {
            switch (0)
            {
                default:
                    return null;
            }
        }

        private StringBuilder GetGameType(MyGameInfo gameInfo)
        {
            switch (gameInfo.GameType)
            {
                case MyGameTypes.Story:
                    return new StringBuilder(MyTextsWrapper.Get(MyTextsWrapperEnum.Story) + " (" + MyTextsWrapper.Get(MyGameplayConstants.GetGameplayDifficultyProfile(gameInfo.Difficulty).DifficultyName) + ")");
                    break;
                case MyGameTypes.Deathmatch:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Deathmatch);
                    break;
                default:
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.UNKNOWN);
                    break;
            }
        }

        void OnSearchTextChanged(object sender, EventArgs e)
        {
            if (sender == m_searchTextbox)
            {
                GetDataFromServer(false);
            }
        }

        void OnGamesItemSelect(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            if (eventArgs.RowIndex == 0)
            {
                //m_selectColumn = eventArgs.ItemIndex;

                //order items
                if (m_gameTableHeaders[eventArgs.ItemIndex] == m_orderByHeader)
                {
                    m_orderAsc = !m_orderAsc;
                }
                else
                {
                    m_orderAsc = true;
                    m_orderByHeader = m_gameTableHeaders[eventArgs.ItemIndex];
                }
                OrderGames();
                RefreshGameList();
            }
            else
            {
                m_selectedGameIndex = eventArgs.RowIndex - 1;
                m_selectRow = eventArgs.RowIndex;
            }

        }

        public override bool Update(bool hasFocus)
        {
            if (m_selectColumn.HasValue)
            {
                m_gameList.SelectColumn(m_selectColumn.Value);
                m_selectColumn = null;
            }
            if (m_selectRow.HasValue)
            {
                m_gameList.SelectRow(m_selectRow.Value);
                m_selectRow = null;
            }
            return base.Update(hasFocus);
        }
    }
}