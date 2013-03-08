#region Using
using System;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.HUD;
using System.Threading;
#endregion

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenSolarSystemMap : MyGuiScreenBase
    {
        MySolarSystemMapCamera m_camera;
        MySolarSystemMapData m_data;
        MySolarMapRenderer m_solarMapRender;
        MyGuiScreenBase m_parent;

        Vector2 m_oldRotationIndicator;
        bool m_particlesEnabled;

        MyMwcSectorIdentifier m_currentSector;

        public static int UNIVERSE_SEED = 0;

        public static MyGuiScreenSolarSystemMap Static = null;

        public static List<MyMwcSectorIdentifier> SectorCache = null;
        private MyGuiControlButton m_travelButton;
        private MyMwcVector3Int? m_selectedSector;

        public MyGuiScreenSolarSystemMap(MyGuiScreenBase parent, MyMwcSectorIdentifier currentSector)
            : base(new Vector2(0.5f, 0.5f), null, Vector2.One)
        {
            m_parent = parent;
            m_enableBackgroundFade = false;
            DrawMouseCursor = true;
            m_currentSector = currentSector;
            m_closeOnEsc = true;

            Static = this;

            MySolarSystemGenerator generator = new MySolarSystemGenerator(UNIVERSE_SEED);
            generator.Generate(1024);

            m_data = generator.SolarSystemData;
            m_solarMapRender = new MySolarMapRenderer();
            m_solarMapRender.PlayerSector = currentSector.Position;

            //MyMinerGame.SwitchPause();
            m_particlesEnabled = TransparentGeometry.Particles.MyParticlesManager.Enabled;
            TransparentGeometry.Particles.MyParticlesManager.Enabled = false;

            //AddCaption(MyTextsWrapperEnum.SolarSystemMap);

            MySolarSystemMapNavigationMark playerNavigationMark =
                new MySolarSystemMapNavigationMark(
                    currentSector.Position,
                    "",
                    null,
                    MyHudConstants.SOLAR_MAP_PLAYER_MARKER_COLOR,
                    MyTransparentMaterialEnum.SolarMapPlayer)
                    {
                        VerticalLineColor = MyHudConstants.SOLAR_MAP_PLAYER_MARKER_COLOR.ToVector4(),
                        DirectionalTexture = MyHudTexturesEnum.DirectionIndicator_green,
                        Offset = new Vector3(0, 0.0f, 0),
                        Text = MyClientServer.LoggedPlayer.GetDisplayName().ToString(),
                        Importance = 50
                    };
            m_data.NavigationMarks.Add(playerNavigationMark);

            if (MyGuiScreenGamePlay.Static.IsEditorStoryActive() || MyGuiScreenGamePlay.Static.GetPreviousGameType() == MyGuiScreenGamePlayType.EDITOR_STORY)
            {
                // Loads all marks, we want it for editor
                LoadMarks(false);
            }
            else
            {
                // Load only active marks
                MyMissions.AddSolarMapMarks(m_data);
            }

            if (MyMissions.ActiveMission != null)
            {
                MyMissions.ActiveMission.AddSolarMapMarks(m_data);
            }

            m_travelButton = new MyGuiControlButton(this, new Vector2(0.0f, 0.40f),
                new Vector2(650f / 1600f, 120f / 1200f),
                MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyGuiManager.GetTravelButtonTexture(),
                MyGuiManager.GetTravelButtonTexture(),
                MyGuiManager.GetTravelButtonTexture(),
                MyTextsWrapperEnum.Travel,
                MyGuiConstants.BACK_BUTTON_TEXT_COLOR,
                MyGuiConstants.BACK_BUTTON_TEXT_SCALE,
                MyGuiControlButtonTextAlignment.CENTERED,
                OnTravelClick,
                false,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                true,
                false);
            m_travelButton.TextOffset = new Vector2(0, -0.0030f);
            m_travelButton.Visible = false;
            Controls.Add(m_travelButton);

            // target sector 0, 0, 0
            // distance is 1.7 sectors from camera
            Vector3 sector;

            var mostImportantMark = m_data.NavigationMarks.GetMostImportant();
            if (mostImportantMark != null) {
                sector = new Vector3(mostImportantMark.Sector.X, mostImportantMark.Sector.Y, mostImportantMark.Sector.Z);
            } else {
                sector = new Vector3(currentSector.Position.X, currentSector.Position.Y, currentSector.Position.Z);
            }

            sector.Y = 0;

            m_camera = new MySolarSystemMapCamera(sector * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS, 10000000.0f * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS);
            m_camera.MinDistanceToTarget = 1.7f * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            m_camera.MaxDistanceToTarget = 1.2f * MySolarSystemUtils.MillionKmToSectors(MyBgrCubeConsts.NEPTUNE_POSITION.Length()) * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            m_camera.MaxSector = MySolarSystemUtils.MillionKmToSectors(MyBgrCubeConsts.NEPTUNE_POSITION.Length());

            MyGuiManager.FullscreenHudEnabled = true;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenSolarMap";
        }

        public override void LoadContent()
        {
            base.LoadContent();

            m_solarMapRender.LoadContent();
        }

        private void LoadMarks(bool forceReload)
        {
            if (SectorCache == null || forceReload) // Load sectors, after successful load, add marks
            {
                SectorCache = new List<MyMwcSectorIdentifier>();
                var progressScreen = new MyGuiScreenLoadSectorIdentifiersProgress(MyMwcSectorTypeEnum.STORY, true, SectorIdentifiersLoaded);
                MyGuiManager.AddScreen(progressScreen);
            }
            else
            {
                AddMarks(); // Add marks now
            }
        }

        private void SectorIdentifiersLoaded(List<MyMwcSectorIdentifier> sectors)
        {
            SectorCache = sectors;
            AddMarks();
        }

        private void AddMarks()
        {
            foreach (var sector in SectorCache)
            {
                const int centerDist = 1;
                if (MyFakes.HIDE_CENTER_SECTOR_MARKS && Math.Abs(sector.Position.X) <= centerDist && Math.Abs(sector.Position.Y) <= centerDist && Math.Abs(sector.Position.Z) <= centerDist)
                {
                    continue;
                }

                var mark = new MySolarSystemMapNavigationMark(sector.Position, sector.SectorName,
                            null,
                            MyHudConstants.SOLAR_MAP_SIDE_MISSION_MARKER_COLOR,
                            TransparentGeometry.MyTransparentMaterialEnum.SolarMapSideMission)
                            {
                                DirectionalTexture = MyHudTexturesEnum.DirectionIndicator_white,
                            };


                m_data.NavigationMarks.Add(mark);
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            m_solarMapRender.UnloadContent();
        }

        public override bool UnhideScreen()
        {
            m_parent.HideScreen();
            MyGuiManager.FullscreenHudEnabled = true;
            return base.UnhideScreen();
        }

        public override bool HideScreen()
        {
            MyGuiManager.SetMouseCursorTexture(MyGuiManager.GetMouseCursorArrowTexture());
            m_parent.UnhideScreen();
            MyGuiManager.FullscreenHudEnabled = false;
            return base.HideScreen();
        }

        public override bool CloseScreen()
        {
            MyGuiManager.SetMouseCursorTexture(MyGuiManager.GetMouseCursorArrowTexture());
            m_parent.UnhideScreen();
            MyGuiManager.FullscreenHudEnabled = false;
            return base.CloseScreen();
        }

        public override void CloseScreenNow()
        {
            MyGuiManager.SetMouseCursorTexture(MyGuiManager.GetMouseCursorArrowTexture());
            TransparentGeometry.Particles.MyParticlesManager.Enabled = m_particlesEnabled;
            //MyMinerGame.SwitchPause();
            Static = null;
            MyGuiManager.FullscreenHudEnabled = false;
            base.CloseScreenNow();
        }

        //  Just close the screen
        public void OnBackClick()
        {
            CloseScreen();
        }

        public override bool Update(bool hasFocus)
        {
            if (!base.Update(hasFocus))
            {
                return false;
            }

            return true;
        }

        public override int GetTransitionOpeningTime()
        {
            return 0;
        }

        public override int GetTransitionClosingTime()
        {
            return 0;
        }

        private MySolarSystemMapNavigationMark GetNavigationMarkUnderCamera()
        {
            foreach (var sectorNavigationMarksKvp in m_data.NavigationMarks)
            {
                foreach (var navigationMark in sectorNavigationMarksKvp.Value)
                {
                    if (navigationMark.Sector == m_camera.TargetSector)
                    {
                        return navigationMark;
                    }
                }
            }
            return null;
        }

        private MySolarSystemMapNavigationMark GetNearestNavigationMarkUnderMouseCursor()
        {
            MySolarSystemMapNavigationMark nearestNavigationMarkUnderMouse = null;
            float nearestDistanceFromCamera = float.MaxValue;

            foreach (var sectorNavigationMarksKvp in m_data.NavigationMarks)
            {
                foreach (var navigationMark in sectorNavigationMarksKvp.Value)
                {
                    if (navigationMark.IsMouseOver())
                    {
                        float distanceFromCamera = navigationMark.WorldPosition.Length();
                        if (distanceFromCamera < nearestDistanceFromCamera)
                        {
                            nearestDistanceFromCamera = distanceFromCamera;
                            nearestNavigationMarkUnderMouse = navigationMark;
                        }
                    }
                }
            }

            return nearestNavigationMarkUnderMouse;
        }

        private void OnTravelClick(MyGuiControlButton sender)
        {
            TravelToSector(m_selectedNavigationMark.Sector, m_selectedNavigationMark.MissionID);
        }

        [Conditional("DEBUG")]
        private void CopySector()
        {
            string pos = String.Format("{0}, {1}, {2}", m_camera.TargetSector.X, m_camera.TargetSector.Y, m_camera.TargetSector.Z);
            var thread = new Thread(() => System.Windows.Forms.Clipboard.SetText(pos));
            thread.Name = "CopySector";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        [Conditional("DEBUG")]
        private void PasteSector()
        {
            var thread = new Thread(() =>
            {
                var text = System.Windows.Forms.Clipboard.GetText();
                var parts = text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                int x, y, z;
                if (parts.Length == 3 && int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y) && int.TryParse(parts[2], out z))
                {
                    m_camera.MoveToSector(new MyMwcVector3Int(x, y, z));
                }
            });
            thread.Name = "PasteSector";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public override void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            base.HandleInput(input, receivedFocusInThisUpdate);

            if (MyGuiInput.ENABLE_DEVELOPER_KEYS)
            {
                if (input.IsNewKeyPress(Keys.C) && input.IsAnyCtrlKeyPressed())
                {
                    CopySector();
                }
                if (input.IsNewKeyPress(Keys.V) && input.IsAnyCtrlKeyPressed())
                {
                    PasteSector();
                }
            }


            if (MyFakes.DRAW_FACTION_AREAS_IN_SOLAR_MAP)
            {
                MySolarMapAreaInput.HandleInput(m_solarMapRender, input, receivedFocusInThisUpdate);
            }

            float rollIndicator = input.GetRoll();
            Vector2 rotationIndicator = Vector2.Zero;
            if (input.IsNewRightMousePressed() && MyVideoModeManager.IsHardwareCursorUsed())
            {
                m_oldRotationIndicator = input.GetRotation();
            }

            if (input.IsRightMousePressed())
            {

                if (MyVideoModeManager.IsHardwareCursorUsed())
                {
                    rotationIndicator = m_oldRotationIndicator - input.GetRotation();
                    m_oldRotationIndicator = input.GetRotation();
                }
                else
                    rotationIndicator = input.GetRotation();
            }
            Vector3 moveIndicator = input.GetPositionDelta();
            if (input.IsKeyPress(Keys.Left))
                moveIndicator.X = -1;
            if (input.IsKeyPress(Keys.Right))
                moveIndicator.X = 1;
            if (input.IsKeyPress(Keys.Up))
                moveIndicator.Z = -1;
            if (input.IsKeyPress(Keys.Down))
                moveIndicator.Z = 1;

            m_camera.Zoom(input.DeltaMouseScrollWheelValue());

            m_camera.MoveAndRotate(moveIndicator, rotationIndicator, rollIndicator, 1);


            bool sectorChanged = false;
            if (m_lastSector != m_camera.TargetSector)
            {
                sectorChanged = true;
                m_lastSector = m_camera.TargetSector;
            }



            MySolarSystemMapNavigationMark navigationMarkUnderMouse = GetNearestNavigationMarkUnderMouseCursor();

            const float maxHeightForEnter = MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS * 16;

            if (sectorChanged) // we have moved camera so deselect sector
            {
                m_selectedSector = null;
            }


            // tool tips
            if (m_lastNavigationMarkUnderMouse != navigationMarkUnderMouse)
            {
                m_toolTip.ClearToolTips();
                if (navigationMarkUnderMouse != null)
                {
                    m_toolTip.AddToolTip(new StringBuilder(GetSectorName(navigationMarkUnderMouse)));
                    if (!String.IsNullOrEmpty(navigationMarkUnderMouse.Description))
                    {
                        m_toolTip.AddToolTip(new StringBuilder(navigationMarkUnderMouse.Description), Color.LightGray);
                    }
                    m_toolTip.AddToolTip(new StringBuilder(navigationMarkUnderMouse.Sector.ToString()), Color.LightGray);
                }
                m_lastNavigationMarkUnderMouse = navigationMarkUnderMouse;
            }



            if (navigationMarkUnderMouse != null)
            {
                MyGuiManager.SetMouseCursorTexture(MyGuiManager.GetMouseCursorHandTexture());

                if (input.IsNewLeftMousePressed() && !m_travelButton.IsMouseOver())
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseClick);
                    if (m_selectedNavigationMark != null)
                        m_selectedNavigationMark.Highlight = false;

                    m_selectedNavigationMark = navigationMarkUnderMouse;
                    m_selectedNavigationMark.Highlight = true;
                    sectorChanged = true;
                    m_slectionLocked = true;
                    m_travelButton.Visible = false;
                }
                if (input.IsNewLeftMouseDoubleClick())
                {
                    TravelToSector(navigationMarkUnderMouse.Sector, navigationMarkUnderMouse.MissionID);
                }
            }
            else if (m_camera.CameraDistance < maxHeightForEnter && !m_slectionLocked)
            {
                if (MyGuiScreenGamePlay.CanTravelToSector(m_camera.TargetSector))
                {
                    //MyGuiManager.SetMouseCursorTexture(MyGuiManager.GetMouseCursorHandTexture());
                    if (m_selectedNavigationMark != null)
                        m_selectedNavigationMark.Highlight = false;

                    var navigationMarkUnderCamera = GetNavigationMarkUnderCamera();
                    if (navigationMarkUnderCamera != null && navigationMarkUnderCamera.Sector == m_camera.TargetSector)
                    {
                        m_selectedNavigationMark = navigationMarkUnderCamera;
                    }
                    else
                    {
                        m_selectedNavigationMark = new MySolarSystemMapNavigationMark(m_camera.TargetSector, "");
                        m_travelButton.Visible = false;
                    }
                }
                else
                {
                    m_selectedNavigationMark = null;
                }
            }
            else if (input.IsNewLeftMousePressed())
            {
                if (m_selectedNavigationMark != null)
                    m_selectedNavigationMark.Highlight = false;
                if (!m_travelButton.IsMouseOver())
                {
                    m_selectedNavigationMark = null;
                }
                m_slectionLocked = false;
            }
            else if (sectorChanged && m_camera.CameraDistance > maxHeightForEnter && !m_slectionLocked)
            {
                m_selectedNavigationMark = null;
            }
            else
            {
                MyGuiManager.SetMouseCursorTexture(MyGuiManager.GetMouseCursorArrowTexture());
            }



            if (m_selectedNavigationMark != null)
            {
                if (!m_travelButton.Visible)
                {
                    m_travelButton.Text.Clear();
                    string text = GetSectorName(m_selectedNavigationMark);
                    if (text.Length > 21)
                    {
                        text = text.Substring(0, 20);
                        text += "…";
                    }
                    m_travelButton.Text.Append(MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.TravelTo, text));
                    //                    float width = MyGuiManager.GetNormalizedSize( MyGuiManager.GetFontMinerWarsBlue(), m_travelButton.Text, 1).X + 0.05f;
                    //  m_travelButton.SetSize(new Vector2(width, MyGuiConstants.BACK_BUTTON_SIZE.Y));
                    m_travelButton.Visible = true;
                }
            }
            else
            {
                m_travelButton.Visible = false;
            }

        }

        private bool m_slectionLocked;
        private MyMwcVector3Int m_lastSector;
        private MySolarSystemMapNavigationMark m_lastNavigationMarkUnderMouse = null;
        private MySolarSystemMapNavigationMark m_selectedNavigationMark = null;

        private void TravelToSector(MyMwcVector3Int sector, MyMissionID? missionID = null)
        {
            if (MyGuiScreenGamePlay.Static.IsCurrentSector(sector))
            {
                CloseScreen();
                return;
            }

            if (!MyGuiScreenGamePlay.CanTravelToSector(sector))
            {
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, MyTextsWrapperEnum.ForbiddenSolarMapAreaWarning, MyTextsWrapperEnum.Warning, MyTextsWrapperEnum.Ok, null));
                return;
            }

            MyMwcVector3Int targetSector = sector;
            m_currentSector.Position = targetSector;
            m_currentSector.UserId = MyClientServer.LoggedPlayer.GetUserId();
            this.CloseScreenNow();
            MyGuiScreenGamePlay.Static.TravelToSector(m_currentSector, MyMwcTravelTypeEnum.SOLAR, MySession.PlayerShip.GetPosition(), missionID);
        }

        private string GetSectorName(MySolarSystemMapNavigationMark navigationMark)
        {
            string sectorName = navigationMark.Name;

            if (String.IsNullOrWhiteSpace(sectorName))
                sectorName = navigationMark.Text;

            if (String.IsNullOrWhiteSpace(sectorName))
                sectorName = navigationMark.Sector.ToString();

            return sectorName;
        }
        public override bool Draw(float backgroundFadeAlpha)
        {
            foreach (var sectorNavigationMarksKvp in m_data.NavigationMarks)
            {
                foreach (var navigationMark in sectorNavigationMarksKvp.Value)
                {
                    navigationMark.Update(m_camera);
                }
            }

            m_solarMapRender.Draw(m_camera, m_data);

            MyHud.DrawOnlyMissionObjectives();

            MySolarSystemMapNavigationMark navigationMarkUnderMouse = GetNearestNavigationMarkUnderMouseCursor();

            if (navigationMarkUnderMouse != null)
            {
                var toolTipPosition = MyGuiManager.MouseCursorPosition + MyGuiConstants.TOOL_TIP_RELATIVE_DEFAULT_POSITION;
                m_toolTip.Draw(toolTipPosition);
            }

            base.Draw(backgroundFadeAlpha);

            return true;
        }

        MyToolTips m_toolTip = new MyToolTips();
    }
}


internal static class MySolarMapAreaInput
{
    internal static MyMwcVector3Int center;
    internal static MyMwcVector3Int point;

    internal static void HandleInput(MySolarMapRenderer renderer, MyGuiInput input, bool receivedFocusInThisUpdate)
    {
        if (input.IsNewKeyPress(Keys.NumPad7))
        {
            center = renderer.GetTargetSector();
        }

        if (input.IsNewKeyPress(Keys.NumPad9))
        {
            point = renderer.GetTargetSector();
        }

        if (input.IsNewKeyPress(Keys.NumPad8))
        {
            System.Diagnostics.Debug.WriteLine(string.Format("MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, new MyMwcVector3Int({0}), new MyMwcVector3Int({1}));", center, point));
            MyFactions.AddFactionArea(MyMwcObjectBuilder_FactionEnum.China, center, point);
        }

        if (input.IsNewKeyPress(Keys.NumPad4))
        {
            Debug.WriteLine(MyFactions.GetFactionBySector(renderer.GetTargetSector()));
        }
    }


}