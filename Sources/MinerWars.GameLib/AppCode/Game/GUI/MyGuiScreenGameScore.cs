using System;
using System.Text;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Toolkit.Input;

namespace MinerWars.AppCode.Game.GUI
{
    struct MyFactionStats
    {
        public int Kills;
        public int Deaths;
        public MyMwcObjectBuilder_FactionEnum Faction;

        public MyFactionStats(MyMwcObjectBuilder_FactionEnum faction)
        {
            Faction = faction;
            Kills = 0;
            Deaths = 0;
        }

        public MyFactionStats(MyMwcObjectBuilder_FactionEnum faction, int kills, int deaths)
        {
            Faction = faction;
            Kills = kills;
            Deaths = deaths;
        }
    }

    struct MyStatsRow
    {
        public StringBuilder Name;
        public MyMwcObjectBuilder_FactionEnum Faction;
        public int? Ping;
        public int Kills;
        public int Deaths;
        public int Id;
        public bool IsFaction;
    }
    class MyGuiScreenGameScore : MyGuiScreenBase
    {
        const int LONG_SB_LENGTH = 10;
        const int SHORT_SB_LENGTH = 2;
        const int POOL_INCREMENT = 10;

        Dictionary<MyMwcObjectBuilder_FactionEnum, MyFactionStats> m_statsTableFactions; //all factions stats
        List<MyStatsRow> m_stats;
        StringBuilder m_stringBuilderForText = new StringBuilder();
        MyMwcObjectBuilder_FactionEnum m_playerFaction;
        private MyGuiControlListbox m_statsListbox;
        byte m_playerId;
        private Dictionary<MyMwcObjectBuilder_FactionEnum, Color> m_usedFactions;
        List<StringBuilder> m_sbLongPool;
        List<StringBuilder> m_sbShortPool;
        int m_sbLongPoolIndex;
        int m_sbShortPoolIndex;

        public MyGuiScreenGameScore()
            : base(Vector2.Zero, null, null)
        {
            m_closeOnEsc = true;
            m_enableBackgroundFade = true;

            m_sbLongPool = new List<StringBuilder>();
            m_sbShortPool = new List<StringBuilder>();
            NullPoolIndexes();

            m_statsListbox = new MyGuiControlListbox(this, new Vector2(0.5f, 0.5f), new Vector2(0.35f, 0.04f), new Vector4(0,0,0,0.5f), null, 0.65f, 4, 20, 4, true, false, false, null, null,
                MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 0, 0, Color.Transparent.ToVector4(), 0f, 0f, 0f, 0f, 0, 0, -0.01f, -0.01f, -0.02f, 0.02f);
            m_statsListbox.DisplayHighlight = false;
            m_statsListbox.HideToolTip();
            m_statsListbox.SetCustomCollumnsWidths(new List<float>()
            {
                0, 0.1f, 0.1f, 0.1f
            });
            m_statsListbox.ShowScrollBarOnlyWhenNeeded = true;
            Controls.Add(m_statsListbox);

            GetStats();

            MyGuiScreenGamePlay.Static.DrawHud = false;
        }

        #region SbPool stuff

        private void NullPoolIndexes()
        {
            m_sbLongPoolIndex = m_sbShortPoolIndex = 0;
        }

        private void InitPool()
        {
            NullPoolIndexes();
            List<MyMwcObjectBuilder_FactionEnum> usedFactions = new List<MyMwcObjectBuilder_FactionEnum>(2);
            usedFactions.Add(MySession.Static.Player.Faction);
            foreach (MyPlayerRemote player in MyMultiplayerPeers.Static.Players)
            {
                if (!usedFactions.Contains(player.Faction))
                {
                    usedFactions.Add(player.Faction);
                }
            }

            int assumedShortSbCount = (usedFactions.Count + MyMultiplayerPeers.Static.Players.Count + 1) * 2;
            int assumedLongSbCount = usedFactions.Count + MyMultiplayerPeers.Static.Players.Count + 1;
            usedFactions.Clear();

            RealocatePool(assumedShortSbCount, assumedLongSbCount);
        }

        private void RealocatePoolShort(int increment)
        {
            RealocatePool(m_sbShortPool.Capacity + increment, 0);
        }

        private void RealocatePoolLong(int increment)
        {
            RealocatePool(0, m_sbLongPool.Capacity + increment);
        }

        private void RealocatePool(int shortCount, int longCount)
        {
            int oldLong = m_sbLongPool.Capacity;
            if (longCount > oldLong)
            {
                m_sbLongPool.Capacity = longCount;
                for (int i = oldLong; i < longCount; i++)
                {
                    m_sbLongPool.Add(new StringBuilder(LONG_SB_LENGTH));
                }
            }

            int oldShort = m_sbShortPool.Capacity;
            if (shortCount > oldShort)
            {
                m_sbShortPool.Capacity = shortCount;
                for (int i = oldShort; i < shortCount; i++)
                {
                    m_sbShortPool.Add(new StringBuilder(SHORT_SB_LENGTH));
                }
            }
        }

        private void CheckSbLongPoolCapacity()
        {
            while (m_sbLongPoolIndex >= m_sbLongPool.Capacity)
            {
                RealocatePoolLong(POOL_INCREMENT);
            }
        }

        private void CheckSbShortPoolCapacity()
        {
            while (m_sbShortPoolIndex >= m_sbShortPool.Capacity)
            {
                RealocatePoolShort(POOL_INCREMENT);
            }
        }

        private StringBuilder GetStringBuilderLong()
        {
            CheckSbLongPoolCapacity();
            StringBuilder ret = m_sbLongPool[m_sbLongPoolIndex++];
            ret.Clear();
            return ret;
        }

        private StringBuilder GetStringBuilderShort()
        {
            CheckSbShortPoolCapacity();
            StringBuilder ret = m_sbShortPool[m_sbShortPoolIndex++];
            ret.Clear();
            return ret;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenGameScore";
        }

        #endregion

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if (!input.IsKeyPress(Keys.Tab))
            {
                CloseScreen();
            }
        }

        protected override void OnClosed()
        {
            MyGuiScreenGamePlay.Static.DrawHud = true;
            base.OnClosed();
        }

        private bool GetStats()
        {
            if (m_stats == null)
            {
                m_stats = new List<MyStatsRow>();
            }
            m_stats.Clear();

            if (m_statsTableFactions == null)
            {
                m_statsTableFactions = new Dictionary<MyMwcObjectBuilder_FactionEnum, MyFactionStats>();
            }
            m_statsTableFactions.Clear();


            m_statsListbox.RemoveAllRows();

            //add player first
            m_playerFaction = MySession.Static.Player.Faction;
            m_playerId = MyEntityIdentifier.CurrentPlayerId;
            AddPlayer(MyClientServer.LoggedPlayer.GetDisplayName(), 0, MyClientServer.LoggedPlayer.Statistics.Deaths, MyClientServer.LoggedPlayer.Statistics.PlayersKilled, m_playerId, m_playerFaction);

            //add other players
            foreach (MyPlayerRemote player in MyMultiplayerPeers.Static.Players)
            {
                AddPlayer(player);
            }

            //Debug add random stuff
            //foreach (MinerWars.AppCode.Game.Entities.MyEntity entity in MinerWars.AppCode.Game.Entities.MyEntities.GetEntities())
            //{
            //    //var bot = entity as MinerWars.AppCode.Game.Entities.MySmallShipBot;
            //    //if (bot != null)
            //    //{
            //        //AddPlayer(new StringBuilder("Bot"), null, 0, 0, 0, bot.Faction);
            //    //}
            //    AddPlayer(new StringBuilder(entity.GetType().Name), null, 0, 0, 0, entity.Faction);
            //}

            //add faction titles
            if (m_statsTableFactions.Count > 1) // No reason to display just one faction (ie Rainers or Freelancers)
            {
                foreach (KeyValuePair<MyMwcObjectBuilder_FactionEnum, MyFactionStats> faction in m_statsTableFactions)
                {
                    AddPlayer(MyFactionConstants.GetFactionProperties(faction.Value.Faction).Name, null, faction.Value.Deaths, faction.Value.Kills, (byte)faction.Value.Faction, faction.Value.Faction, true);
                }
            }

            
            // order
            m_stats.Sort(StatsRowComparer);
            AddHeaders();
            foreach (MyStatsRow row in m_stats)
            {
                if (row.IsFaction)
                    m_statsListbox.AddRow();
                AddStatRowToListbox(row);
            }

            return true;
        }

        private void AddStatRowToListbox(MyStatsRow statsRow)
        {
            StringBuilder pingText = GetStringBuilderLong();
            StringBuilder killsText = GetStringBuilderShort();
            StringBuilder deathsText = GetStringBuilderShort();

            pingText.Append(statsRow.Ping.HasValue ? statsRow.Ping.Value.ToString() + "ms" : "");
            killsText.Append(statsRow.Kills.ToString());
            deathsText.Append(statsRow.Deaths.ToString());

            Vector4? rowColor = null;

            if (statsRow.IsFaction)
            {
                if (MySession.Static.Player.Faction == statsRow.Faction)
                    rowColor = new Vector4(0, 1f, 0, 0.07f);
                else
                    rowColor = new Vector4(1f, 0, 0, 0.07f);
            }
            else if (statsRow.Id == (byte)MyClientServer.LoggedPlayer.GetUserId())
                rowColor = new Vector4(1f, 1f, 1f, 0.02f);

            int rowIndex = m_statsListbox.AddRow(rowColor);

            var textColor = Color.White;


            MyGuiFont textFont;
            if (MyFactions.GetFactionsRelation(MySession.Static.Player.Faction, statsRow.Faction) == MyFactionRelationEnum.Friend || statsRow.Id == m_playerId)
                textFont = MyGuiManager.GetFontMinerWarsGreen();
            else
                textFont = MyGuiManager.GetFontMinerWarsRed();

            m_statsListbox.AddItem(new MyGuiControlListboxItem(rowIndex * 4 + 0,
                new MyColoredText(statsRow.Name, textColor, textFont, GetRowScale(statsRow)),
                null, new MyToolTips(statsRow.Name)), rowIndex, 0);

            m_statsListbox.AddItem(new MyGuiControlListboxItem(rowIndex * 4 + 1,
                new MyColoredText(killsText, textColor, textFont, GetRowScale(statsRow)),
                null, new MyToolTips(killsText)), rowIndex, 1);

            m_statsListbox.AddItem(new MyGuiControlListboxItem(rowIndex * 4 + 2,
                new MyColoredText(deathsText, textColor, textFont, GetRowScale(statsRow)),
                null, new MyToolTips(deathsText)), rowIndex, 2);

            m_statsListbox.AddItem(new MyGuiControlListboxItem(rowIndex * 4 + 3,
                new MyColoredText(pingText, textColor, textFont, GetRowScale(statsRow)), 
                null, new MyToolTips(pingText)), rowIndex, 3);
        }

        int StatsRowComparer(MyStatsRow x, MyStatsRow y)
        {
            if (x.Faction != y.Faction) // First order by factions
            {
                if (x.Faction == m_playerFaction) return -1;
                else if (y.Faction == m_playerFaction) return 1;
                else return ((int)x.Faction).CompareTo(((int)y.Faction));
            }
            else if (x.IsFaction != y.IsFaction) // First are factions then players
            {
                return -x.IsFaction.CompareTo(y.IsFaction);
            }
            else if (x.Kills != y.Kills) // Then order by kills
            {
                return -x.Kills.CompareTo(y.Kills);
            }
            else if (x.Deaths != y.Deaths) // Then by deaths
            {
                return x.Deaths.CompareTo(y.Deaths);
            }
            else // Then by name
            {
                return x.Name.ToString().CompareTo(y.Name.ToString());
            }
        }
        
        private void AddPlayer(MyPlayerRemote player, bool isFaction = false)
        {
            AddPlayer(player.GetDisplayName(), (int)(player.Connection.AverageRoundtripTime * 1000), player.Statistics.Deaths, player.Statistics.PlayersKilled, (byte)player.PlayerId, player.Faction, isFaction);
        }

        private void AddPlayer(StringBuilder name, int? ping, int deaths, int kills, byte id, MyMwcObjectBuilder_FactionEnum faction, bool isFaction = false){
            if (faction == MyMwcObjectBuilder_FactionEnum.None) // Dont add players without factions
                return;

            if (!m_statsTableFactions.ContainsKey(faction))
            {
                m_statsTableFactions.Add(faction, new MyFactionStats(faction));
            }
            if (!isFaction)
            {
                m_statsTableFactions[faction] = new MyFactionStats(faction, m_statsTableFactions[faction].Kills+kills, m_statsTableFactions[faction].Deaths+deaths);
            }
            m_stats.Add(new MyStatsRow()
            {
                Name = isFaction? name : new StringBuilder("    ").Append(name),
                Ping = ping,
                Deaths = deaths,
                Kills = kills,
                Id = id,
                Faction = faction,
                IsFaction = isFaction
            });
        }

        private float GetRowScale(MyStatsRow row)
        {
            //if(row.Id == m_playerId){
            //    return 0.7f;
            //}
            //else 
            if (row.IsFaction)
            {
                return 0.9f;
            }
            else
            {
                return 0.7f;
            }
        }



        private void AddHeaders()
        {
            int rowIndex = m_statsListbox.AddRow();
            var textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            float textScale = 0.9f;
            m_statsListbox.AddItem(new MyGuiControlListboxItem(rowIndex * 4 + 0,
                new MyColoredText(MyTextsWrapper.Get(MyTextsWrapperEnum.ScoreName), textColor, MyGuiManager.GetFontMinerWarsWhite(), textScale),
                null, new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.ScoreName))), rowIndex, 0);
            m_statsListbox.AddItem(new MyGuiControlListboxItem(rowIndex * 4 + 1,
                new MyColoredText(MyTextsWrapper.Get(MyTextsWrapperEnum.ScoreKills), textColor, MyGuiManager.GetFontMinerWarsWhite(), textScale),
                null, new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.ScoreKills))) { BackgroundColor = Color.Red.ToVector4() }, rowIndex, 1);
            m_statsListbox.AddItem(new MyGuiControlListboxItem(rowIndex * 4 + 2,
                new MyColoredText(MyTextsWrapper.Get(MyTextsWrapperEnum.ScoreDeaths), textColor, MyGuiManager.GetFontMinerWarsWhite(), textScale),
                null, new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.ScoreDeaths))) { BackgroundColor = Color.Red.ToVector4() }, rowIndex, 2);
            m_statsListbox.AddItem(new MyGuiControlListboxItem(rowIndex * 4 + 3,
                new MyColoredText(MyTextsWrapper.Get(MyTextsWrapperEnum.ScorePing), textColor, MyGuiManager.GetFontMinerWarsWhite(), textScale),
                null, new MyToolTips(MyTextsWrapper.Get(MyTextsWrapperEnum.ScorePing))) { BackgroundColor = Color.Red.ToVector4() }, rowIndex, 3);

        }
    }
}