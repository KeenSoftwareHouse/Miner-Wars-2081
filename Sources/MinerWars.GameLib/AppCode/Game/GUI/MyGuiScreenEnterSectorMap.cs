using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Linq;
using System;
using MinerWars.AppCode.Game.World;
using ParallelTasks;

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenEnterSectorMap : MyGuiScreenBase
    {
        //MyGuiControlCombobox m_sectorsCombobox;
        MyGuiControlListbox m_sectorsListbox;
        MyGuiScreenBase m_closeAfterSuccessfulEnter;
        List<MyMwcSectorIdentifier> m_sectors;
        MyTextsWrapperEnum m_startSessionProgressText;
        MyMwcStartSessionRequestTypeEnum m_startSessionType;

        MyMwcVector3Int? m_lastSandboxSector;
        Future<List<MyMwcSectorIdentifier>>? m_loadingSectors;

        Dictionary<MyMwcVector3Int, string> m_sectorPositionsToTextures;

        public Action<MyMwcSectorIdentifier> CustomLoadAction { get; set; }

        public MyGuiScreenEnterSectorMap(MyGuiScreenBase closeAfterSuccessfulEnter, MyMwcStartSessionRequestTypeEnum startSessionType, MyTextsWrapperEnum startSessionProgressText)
            : this(closeAfterSuccessfulEnter, startSessionType, startSessionProgressText, null)
        {
        }

        public MyGuiScreenEnterSectorMap(MyGuiScreenBase closeAfterSuccessfulEnter, MyMwcStartSessionRequestTypeEnum startSessionType, MyTextsWrapperEnum startSessionProgressText, MyMwcVector3Int? lastSandboxSector)
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(700f / 1600f, 700 / 1200f))
        {
            m_closeAfterSuccessfulEnter = closeAfterSuccessfulEnter;
            m_startSessionType = startSessionType;
            m_startSessionProgressText = startSessionProgressText;
            m_enableBackgroundFade = true;
            m_sectors = new List<MyMwcSectorIdentifier>();
            m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ConfigWheelBackground", flags: TextureFlags.IgnoreQuality);
            m_lastSandboxSector = lastSandboxSector;

            RecreateControls();

            CreateSectorPositionsToTexturesTable();
        }

        private void CreateSectorPositionsToTexturesTable()
        {
            m_sectorPositionsToTextures = new Dictionary<MyMwcVector3Int, string>();

            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(93, 0, 73), "MothershipFacility");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(88, 0, -58), "FragileDeathmatch");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(83, 0, 46), "Rafinery");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(76, 0, 29), "NewNanjing");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(64, 0, -20), "UraniteMines");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(61, 0, -65), "IceCaveDeathmatch");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(53, 0, 84), "HallOfFame");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(47, 0, -24), "VoxelArena");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(19, 0, -92), "NerbayStations");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(18, 0, 96), "ArabianBorder");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(3, 0, 9), "HubShowcase");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-30, 0, -61), "Warehouse");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-46, 0, 48), "StBarbara");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-66, 0, -44), "MilitaryOutpostDeathmatch");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-100, 0, 20), "ResearchVessel");

            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-78, 0, 35), "NewNYCDeathmatch");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-93, 0, 46), "RiftDeathmatch");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-4, 0, 23), "IndustrialDeathmatch");


            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(17, 0, 67), "25DAsteroidField");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(93, 0, 14), "25DCityFight");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-38, 0, 71), "GatesOfHell");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-15, 0, -42), "25DJunkyard");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-79, 0, 70), "25DMilitaryArea");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-8, 0, -5), "25DMinerOutpost");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(-43, 0, 86), "25DPlain");
            m_sectorPositionsToTextures.Add(new MyMwcVector3Int(1, 0, -58), "25DRadioactive");
        }

        private void RecreateControls()
        {
            Controls.Clear();

            m_size = new Vector2(1100f / 1600f, 1200 / 1200f);

            Vector2 controlsOriginLeft = new Vector2(-m_size.Value.X / 2.0f + 0.07f, -m_size.Value.Y / 2.0f + 0.2f - 0.0360f);
            Vector2 controlsOriginRight = new Vector2(-m_size.Value.X / 2.0f + 0.07f + 0.355f, -m_size.Value.Y / 2.0f + 0.2f - 0.0360f);



            AddCaption(MyTextsWrapperEnum.TemporarySectorMap, new Vector2(0, 0.035f));

            //Controls.Add(new MyGuiControlLabel(this, controlsOriginLeft, null, MyTextsWrapperEnum.Map, MyGuiConstants.LABEL_TEXT_COLOR,
            //    MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            if (m_sectors != null && m_sectors.Count > 0)
            {
                // When editing global story sectors, add some unused sector
                AddUnusedSector();

                int selectedIndex = 0;

                /*m_sectorsCombobox = new MyGuiControlCombobox(this, new Vector2(0, -0.17f), MyGuiControlPreDefinedSize.LONGMEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE, 6, true, false, true);
                for (int i = 0; i < m_sectors.Count; i++)
                {
                    MyMwcSectorIdentifier sectorIdentifier = m_sectors[i];

                    if (m_lastSandboxSector.HasValue && sectorIdentifier.Position.Equals(m_lastSandboxSector.Value))
                    {
                        selectedIndex = i;
                    }

                    if (string.IsNullOrEmpty(sectorIdentifier.SectorName))
                    {
                        m_sectorsCombobox.AddItem(i, GetSectorScreenshot(sectorIdentifier), new StringBuilder(string.Format("{0}", sectorIdentifier.Position.ToString())));
                    }
                    else
                    {
                        m_sectorsCombobox.AddItem(i, GetSectorScreenshot(sectorIdentifier), new StringBuilder(string.Format("{0} ({1})", sectorIdentifier.SectorName, sectorIdentifier.Position.ToString())));
                    }
                }

                SortSectors();

                m_sectorsCombobox.SelectItemByKey(selectedIndex);
                m_sectorsCombobox.OnSelectItemDoubleClick += OnItemDoubleClick;
                Controls.Add(m_sectorsCombobox);*/

                //Listbox

                m_sectorsListbox = new MyGuiControlListbox(this, new Vector2(0, -0.025f), new Vector2(0.43f, 0.1f), MyGuiConstants.LISTBOX_BACKGROUND_COLOR, null, MyGuiConstants.LABEL_TEXT_SCALE, 1, 6, 1, true, true, false,
                    null, null, MyGuiManager.GetScrollbarSlider(), MyGuiManager.GetHorizontalScrollbarSlider(), 1, 0, MyGuiConstants.LISTBOX_BACKGROUND_COLOR_BLUE, 0f, 0f, 0f, 0f, 0, 0f, -0.01f, -0.01f, -0.02f, 0.02f) { IconScale = new Vector2(0.97f) };
                for (int i = 0; i < m_sectors.Count; i++)
                {
                    MyMwcSectorIdentifier sectorIdentifier = m_sectors[i];

                    if (m_lastSandboxSector.HasValue && sectorIdentifier.Position.Equals(m_lastSandboxSector.Value))
                    {
                        selectedIndex = i;
                    }

                    if (string.IsNullOrEmpty(sectorIdentifier.SectorName))
                    {
                        m_sectorsListbox.AddItem(i, new StringBuilder(string.Format("{0}", sectorIdentifier.Position.ToString())), GetSectorScreenshot(sectorIdentifier));
                    }
                    else
                    {
                        m_sectorsListbox.AddItem(i, new StringBuilder(string.Format("{0} ({1})", sectorIdentifier.SectorName, sectorIdentifier.Position.ToString())), GetSectorScreenshot(sectorIdentifier));
                    }
                }

                SortSectors();

                m_sectorsListbox.SetSelectedItem(selectedIndex);
                m_sectorsListbox.ItemDoubleClick += OnItemDoubleClick;
                Controls.Add(m_sectorsListbox);
            }
            else
            {
                var text = m_loadingSectors.HasValue ? MyTextsWrapperEnum.LoadingPleaseWait : MyTextsWrapperEnum.NoSectorsAvailable;

                Controls.Add(new MyGuiControlLabel(this, new Vector2(0, -0.03f), null, text, MyGuiConstants.LABEL_TEXT_COLOR,
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
        }

        private void AddUnusedSector()
        {
            if (m_startSessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX && m_sectors[0].UserId == null)
            {
                MyMwcVector3Int position;
                do
                {
                    position = new MyMwcVector3Int(MyMwcUtils.GetRandomInt(-100, 100), 0, MyMwcUtils.GetRandomInt(-100, 100));
                } while (m_sectors.Any(s => s.Position == position));
                m_sectors.Add(new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.SANDBOX, null, position, "<unused>"));
            }
        }

        //returns SectorScreenshot texture based on UserId, SectorPosition and Sector Type
        private MyTexture2D GetSectorScreenshot(MyMwcSectorIdentifier sectorIdentifier)
        {
            if (sectorIdentifier.UserId == null && sectorIdentifier.SectorType == MyMwcSectorTypeEnum.SANDBOX)
            {
                if (m_sectorPositionsToTextures.ContainsKey(sectorIdentifier.Position))
                {
                    return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\SectorScreens\\" + m_sectorPositionsToTextures[sectorIdentifier.Position], flags: TextureFlags.IgnoreQuality);
                }
                else
                {
                    return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\SectorScreens\\NoPhoto", flags: TextureFlags.IgnoreQuality);
                }
            }
            else
            {
                return null;
            }
        }

        private void SortSectors()
        {
            // Named sectors first
            m_sectorsListbox.CustomSortRows((a, b) =>
            {
                bool aName = string.IsNullOrEmpty(m_sectors[a.Key].SectorName);
                bool bName = string.IsNullOrEmpty(m_sectors[b.Key].SectorName);

                return aName == bName ?
                    a.Value.ToString().CompareTo(b.Value.ToString()) :
                    (aName ? 1 : -1);
            }, 0);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEnterSectorMap";
        }

        public void OnItemDoubleClick(object sender, MyGuiControlListboxItemEventArgs eventArgs)
        {
            OnOkClick(null);
        }

        public void OnOkClick(MyGuiControlButton sender)
        {
            if (m_sectors.Count == 0)
                return;

            MyMwcSectorIdentifier sectorIdentifier = m_sectors[m_sectorsListbox.GetSelectedItemKey().Value];

            if (m_startSessionType == MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS)
            {
                //Remove 25D sectors from 2081 game and non 25D sectors from 25D game.
                if (sectorIdentifier.SectorName.ToUpper().Contains("2.5D") || sectorIdentifier.SectorName.ToUpper().Contains("2,5D"))
                {
                    if (!MyClientServer.MW25DEnabled)
                    {
                        MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, MyTextsWrapperEnum.YouNeed25D, MyTextsWrapperEnum.MessageBoxCaptionFeatureDisabled, MyTextsWrapperEnum.Ok, null));
                        return;
                    }
                }
                else if (!MyClientServer.HasFullGame)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, MyTextsWrapperEnum.YouNeedFullGame, MyTextsWrapperEnum.MessageBoxCaptionFeatureDisabled, MyTextsWrapperEnum.Ok, null));
                    return;
                }
            }


            MyConfig.LastSandboxSector = sectorIdentifier.Position;
            MyConfig.Save();

            MyGuiManager.CloseAllScreensNowExcept(MyGuiScreenGamePlay.Static);

            if (CustomLoadAction != null)
            {
                CustomLoadAction(sectorIdentifier);
            }
            else
            {
                MyGuiManager.AddScreen(new MyGuiScreenStartSessionProgress(m_startSessionType, m_startSessionProgressText,
                    sectorIdentifier,
                    CommonLIB.AppCode.ObjectBuilders.MyGameplayDifficultyEnum.EASY,
                    null,
                    m_closeAfterSuccessfulEnter));
            }
        }

        public void OnCancelClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }

        public override bool Update(bool hasFocus)
        {
            if (m_loadingSectors.HasValue && m_loadingSectors.Value.IsComplete)
            {
                AddLoadSectorIdentifiersResponse(m_loadingSectors.Value.GetResult());
                m_loadingSectors = null;
            }

            return base.Update(hasFocus);
        }

        public void SetSectorSourceAction(Func<List<MyMwcSectorIdentifier>> sectorLoadFunc)
        {
            m_loadingSectors = Parallel.Start(sectorLoadFunc);
            RecreateControls();
        }

        public void AddLoadSectorIdentifiersResponse(List<MyMwcSectorIdentifier> sectorIdentifiers)
        {
            m_sectors.Clear();
            m_sectors.AddRange(sectorIdentifiers);
            RecreateControls();
        }
    }
}