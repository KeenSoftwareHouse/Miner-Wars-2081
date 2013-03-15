#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI.DebugScreens;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using SysUtils.Utils;


using MinerWarsMath;
//using MinerWarsMath.Graphics;
//using MinerWars.AppCode.Toolkit.Input

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;

#endregion

//  This is static manager for all GUI screens. Use it when adding new screens.

//  Normalized coordinates - is in interval <0..1> on horizontal axis, and now I am not 100% but vertical is maybe also <0..1> or <0..something> where
//  'something' is defined by aspect ratio.

//  Screen coordinates - standard pixel coordinate on interval e.g. <0..1280>

//  IMPORTANT FOR RENDERING:
//  We call Begin on default sprite batch as first thing in MyGuiManager.Draw method. It's OK for most of the screens and controls, but 
//  we have to call End and then again Begin inside GamePlay screen - because it does a lot of 3D rendering and state changes.
//  Same applies for controls that do stencil-mask, they need to restart our sprite batch again.
//  Advantage is that almost all screens and controls are batched and rendered in just one draw call (they are deferred)

namespace MinerWars.AppCode.Game.GUI.Core
{
    using Vector2 = MinerWarsMath.Vector2;
    using Color = MinerWarsMath.Color;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using MinerWars.AppCode.Toolkit.Input;
    using SharpDX.Toolkit.Graphics;
    using MinerWars.AppCode.Game.Utils.VertexFormats;

    //  This sort of alignement allows as to set origin at any of 9 points of rectangle (of course, we need to add these points here and customize position calc)
    enum MyGuiDrawAlignEnum
    {
        HORISONTAL_LEFT_AND_VERTICAL_TOP,
        HORISONTAL_LEFT_AND_VERTICAL_CENTER,
        HORISONTAL_LEFT_AND_VERTICAL_BOTTOM,
        HORISONTAL_CENTER_AND_VERTICAL_CENTER,
        HORISONTAL_CENTER_AND_VERTICAL_TOP,
        HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
        HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM,
        HORISONTAL_RIGHT_AND_VERTICAL_CENTER,
        HORISONTAL_RIGHT_AND_VERTICAL_TOP
    }

    static class MyGuiManager
    {
        internal class Friend
        {
            public void RemoveScreen(MyGuiScreenBase screen)
            {
                MyGuiManager.RemoveScreen(screen);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public static System.Threading.Thread AllowedThread = null;

        //  Textures for GUI elements such as screen background, etc... for wide or tall screens and backgrounds
        public class MyGuiTextureScreen
        {
            string m_filename;
            float m_aspectRatio;

            //  We don't allow default constructor for this class
            private MyGuiTextureScreen() { }

            public MyGuiTextureScreen(string filename, int width, int height)
            {
                m_filename = filename;
                m_aspectRatio = (float)width / (float)height;
            }

            public string GetFilename()
            {
                return m_filename;
            }

            public float GetAspectRatio()
            {
                return m_aspectRatio;
            }
        }

        //  Safe coordinates and size of GUI screen. It makes sure we are OK with aspect ratio and
        //  also it makes sure that if very-wide resolution is used (5000x1000 or so), we draw GUI only in the middle screen.
        static Rectangle m_safeGuiRectangle;            //  Rectangle for safe GUI, it independent from screen aspect ration so GUI elements look same on any resolution (especially their width)
        static Rectangle m_safeFullscreenRectangle;     //  Rectangle for safe fullscreen and GUI - use only when you draw fullscreen images and want to stretch it from left to right. 
        static float m_safeScreenScale;             //  Height scale of actual screen if compared to reference 1200p (calculated as real_height / 1200)
        static Rectangle m_fullscreenRectangle;         //  Real fullscreen

        static Render.MyRender.MyRenderSetup m_renderSetup;
        static Render.MyRender.MyRenderSetup m_backupRenderSetup = new MyRender.MyRenderSetup();


        //  Current debug screens
        static MyGuiScreenDebugBase m_currentStatisticsScreen;
        static MyGuiScreenDebugBase m_currentDebugScreen;
        static bool m_debugScreensEnabled = true;

        public static bool IsDebugScreenEnabled() { return m_debugScreensEnabled; }

        //  Normalized coordinates where width is always 1.0 and height something like 0.8
        //  Don't confuse with GUI normalized coordinates. They are different.
        static Vector2 m_hudSize;
        static Vector2 m_hudSizeHalf;

        //  Min and max mouse coords (in normalized units). Must be calculated from fullscreen, no GUI rectangle.
        static Vector2 m_minMouseCoord;
        static Vector2 m_maxMouseCoord;
        //  Min and max mouse coords for the fullscreen HUD (in normalized units). Must be calculated from fullscreen, no GUI rectangle.
        static Vector2 m_minMouseCoordFullscreenHud;
        static Vector2 m_maxMouseCoordFullscreenHud;

        //  Each screen can use this sprite batch and sprite font, so they don't need to allocate their own
        private static SpriteBatch m_spriteBatch;
        private static int m_spriteBatchUsageCount = 0;

        //  All fonts used in the game
        static MyGuiFont m_fontGuiImpactLarge;
        static MyGuiFont m_fontMinerWarsRed;
        static MyGuiFont m_fontMinerWarsGreen;
        static MyGuiFont m_fontMinerWarsBlue;

        static MyGuiFont m_fontMinerWarsWhite;

        static MyFullScreenQuad m_fullscreenQuad;
        //static MyGuiSpriteBatchOwn m_spriteBatchOwn;

        //  Textures / sprites
        static MyTexture2D m_blankTexture;
        //static MyTexture2D m_textboxSmallTexture;
        static MyTexture2D m_textboxMediumTexture;
        //static MyTexture2D m_textboxLargeTexture;
        static MyTexture2D m_buttonTexture;
        private static MyTexture2D m_buttonTextureBg;
        static MyTexture2D m_searchButtonTexture;
        static MyTexture2D m_smallButtonTexture;
        static MyTexture2D m_travelButtonTexture;

        static MyTexture2D m_comboboxMediumTexture;


        static MyTexture2D m_comboboxMediumTopTexture;
        static MyTexture2D m_comboboxMediumItemTexture;
        static MyTexture2D m_comboboxMediumSelectedTexture;
        static MyTexture2D m_comboboxMediumBottomTexture;

        static MyTexture2D m_scrollbarSlider;
        static MyTexture2D m_horizontalScrollbarSlider;
        static MyTexture2D m_mouseCursorTexture;
        static MyTexture2D m_mouseCursorArrowTexture;
        static System.Drawing.Bitmap m_mouseCursorBitmap;
        static MyTexture2D m_mouseCursorHandTexture;
        static MyTexture2D m_lockedButtonTexture;
        static MyTexture2D m_lockedInventoryItemTexture;
        static MyTexture2D m_sliderTexture;
        static MyTexture2D m_sliderControlTexture;
        //private static MyTexture2D m_configWheelBackground;
        //private static MyTexture2D m_configWheelHover;
        private static MyTexture2D m_configWheelBackgroundSmall;
        static MyTexture2D m_randomTexture;
        static MyTexture2D m_checkBoxOnTexture;
        static MyTexture2D m_checkBoxOffTexture;
        static MyTexture2D m_comboboxSelectedTexture;
        static MyTexture2D m_ammoToolTipTexture;
        static MyTexture2D m_ammoSelectLowBackgroundTexture;
        static MyTexture2D m_ammoSelectKeyConfirmBorderTexture;
        static MyTexture2D m_fogTexture;
        static MyTexture2D m_fogSmallTexture;
        static MyTexture2D m_fogSquareTexture;
        static MyTexture2D m_textureInterlanced;//texture drawn for sprite video player overlapping entire video
        static MyTexture2D m_expandTexture;         // Expand for Tree View
        static MyTexture2D m_collapseTexture;       // Collapse for Tree View
        static MyTexture2D m_removeItemTexture;
        static MyTexture2D m_progressBarTexture;

        static Dictionary<string, MyTexture2D> m_remoteViewWeaponsTextures;
        static Dictionary<string, MyTexture2D> m_remoteViewDroneTextures;
        static Dictionary<string, MyTexture2D> m_remoteViewCameraTextures;

        //static MyTexture2D m_inventoryScreenBackgroundTexture;

        static MyTexture2D m_dialogueNeutralBackgroundTexture;
        static MyTexture2D m_dialogueFriendBackgroundTexture;
        static MyTexture2D m_dialogueEnemyBackgroundTexture;
        //private static MyTexture2D m_SelectEditorbackground;
        //private static MyTexture2D m_SandboxBackground;
        // Editor toolbar button textures
        static MyTexture2D m_toolbarButton;
        static MyTexture2D m_toolbarButtonHover;

        // journal textures
        static MyTexture2D m_journalFilterAll;
        static MyTexture2D m_journalFilterGlobalEvents;
        static MyTexture2D m_journalFilterMissions;
        private static MyTexture2D m_journalFilterStory;
        private static MyTexture2D m_journalLine;
        private static MyTexture2D m_journalSelected;
        private static MyTexture2D m_journalCloseButton;

        // inventory screen
        static MyTexture2D m_inventoryButton;
        static MyTexture2D m_inventoryButtonBg;
        static MyTexture2D m_inventoryListbox;
        //static MyTexture2D m_inventoryListboxItem;
        //static MyTexture2D m_inventoryListboxScrollBar;
        //static MyTexture2D m_inventoryMoneyBox;
        static MyTexture2D m_inventoryButtonTakeAll;
        static MyTexture2D m_inventoryButtonTakeAllBg;

        private static MyTexture2D m_inventoryFilterSortAllOn;
        private static MyTexture2D m_inventoryFilterSortAllOff;
        private static MyTexture2D m_inventoryFilterSortConsumablesOff;
        private static MyTexture2D m_inventoryFilterSortConsumablesOn;
        private static MyTexture2D m_inventoryFilterSortEquipmentOff;
        private static MyTexture2D m_inventoryFilterSortEquipmentOn;
        private static MyTexture2D m_inventoryFilterSortGodsOn;
        private static MyTexture2D m_inventoryFilterSortGodsOff;
        private static MyTexture2D m_inventoryFilterSortOresOff;
        private static MyTexture2D m_inventoryFilterSortOresOn;
        private static MyTexture2D m_inventoryFilterSortWeaponsOff;
        private static MyTexture2D m_inventoryFilterSortWeaponsOn;
        private static MyTexture2D m_inventoryPreviousShip;
        private static MyTexture2D m_inventoryNextShip;

        private static MyTexture2D m_toltipLeft;
        private static MyTexture2D m_toltipBody;
        private static MyTexture2D m_toltipRight;

        //Hub control screen         
        //private static MyTexture2D m_hubBackground;
        private static MyTexture2D m_hubItemBackground;
        private static MyTexture2D m_hubButton;
        private static MyTexture2D m_hubButtonBg;
        private static MyTexture2D m_MessageButton;

        // Prefab previews (used in Editor - AddObjectTreeView)
        //private static Dictionary<MyMwcObjectBuilder_Prefab_TypesEnum, MyTexture2D> m_prefabPreviews;
        private static readonly MyTexture2D[][][] m_prefabPreviews = new MyTexture2D[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilderTypeEnum>() + 1][][];

        static List<MyGuiTextureScreen> m_backgroundScreenTextures;
        static MySpriteListVideoPlayer m_backgroundInterferenceSpriteVideoPlayer;
        static MySpriteListVideoPlayer m_dialoguePortraitNoiseVideoPlayer;

        static MyGuiScreenBase m_lastScreenWithFocus;

        static bool m_fullScreenHudEnabled = false;
        public static bool FullscreenHudEnabled { get { return m_fullScreenHudEnabled; } set { m_fullScreenHudEnabled = value; } }

        //static MyEffectSpriteBatchOriginal m_spriteEffect;

        static Vector2 m_oldVisPos;
        static Vector2 m_oldNonVisPos;
        static bool m_oldMouseVisibilityState;
        static public void SetMouseCursorVisibility(bool p)
        {
            if (m_oldMouseVisibilityState && p != m_oldMouseVisibilityState)
            {
                //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(p.ToString());
                //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(m_oldNonVisPos.ToString());
                //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(m_oldVisPos.ToString());

                m_oldVisPos = MyGuiInput.GetMousePosition();
                m_oldMouseVisibilityState = p;
                MyMinerGame.Static.IsMouseVisible = p;
                m_oldNonVisPos = new Vector2(MyMinerGame.ScreenSizeHalf.X, MyMinerGame.ScreenSizeHalf.Y);
                MyGuiInput.SetMousePosition((int)m_oldNonVisPos.X, (int)m_oldNonVisPos.Y);
                MyGuiInput.SetMouseToScreenCenter();
                return;
            }

            if (!m_oldMouseVisibilityState && p != m_oldMouseVisibilityState)
            {
                //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(p.ToString());
                //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(m_oldNonVisPos.ToString());
                //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall(m_oldVisPos.ToString());

                m_oldNonVisPos = MyGuiInput.GetMousePosition();
                m_oldMouseVisibilityState = p;
                MyMinerGame.Static.IsMouseVisible = p;
                MyGuiInput.SetMousePosition((int)m_oldVisPos.X, (int)m_oldVisPos.Y);
                return;
            }
        }

        //  This one cas be public and not-readonly because we may want to change it from other screens or controls
        private static Vector2 m_mouseCursorPosition;
        public static Vector2 MouseCursorPosition
        {
            get
            {
                if (MyVideoModeManager.IsHardwareCursorUsed())
                {
                    Vector2 scp = MyGuiInput.GetMousePosition();
                    m_mouseCursorPosition = GetNormalizedCoordinateFromScreenCoordinate(scp);
                    return m_mouseCursorPosition;
                }
                else
                    return m_mouseCursorPosition;
            }
            set
            {
                if (MyVideoModeManager.IsHardwareCursorUsed())
                {
                    //MyMinerGame.ScreenSizeHalf.X, MyMinerGame.ScreenSizeHalf.Y
                    Vector2 z = GetScreenCoordinateFromNormalizedCoordinate(value);
                    MyGuiInput.SetMousePosition((int)z.X, (int)z.Y);
                }
                m_mouseCursorPosition = value;
            }
        }



        //  List of screens - works like stack, on the top is screen that has focus
        static List<MyGuiScreenBase> m_screens;

        //  Used only when adding / removing screens - because we can't alter m_screens during iterator looping
        static List<MyGuiScreenBase> m_screensToRemove;
        static List<MyGuiScreenBase> m_screensToAdd;

        //  Keyboard and mouse input
        static MyGuiInput m_input;

        static MyScreenshot m_screenshot;

        // If true, all screen without focus handles input
        static bool m_inputToNonFocusedScreens = false;
        public static bool InputToNonFocusedScreens
        {
            get
            {
                return m_inputToNonFocusedScreens;
            }
            set
            {
                m_inputToNonFocusedScreens = value;
            }
        }
        //static StringBuilder m_inputSharingText = new StringBuilder("WARNING: Sharing input enabled (release ALT to disable it)");
        static StringBuilder m_inputSharingText;
        static StringBuilder m_renderOverloadedText = new StringBuilder("WARNING: Render is overloaded, optimize your scene!");

        private static MyGuiPreviewRenderer m_previewRenderer = new MyGuiPreviewRenderer();

        private static MyGuiScreenDemoAccessInfo m_demoScreen;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        static MyGuiManager()
        {
            MyMwcLog.WriteLine("MyGuiManager()");

            if (MyFakes.ALT_AS_DEBUG_KEY)
            {
                m_inputSharingText = new StringBuilder("WARNING: Sharing input enabled (release ALT to disable it)");
            }
            else
            {
                m_inputSharingText = new StringBuilder("WARNING: Sharing input enabled (release Scroll Lock to disable it)");
            }

            m_screens = new List<MyGuiScreenBase>();
            m_screensToRemove = new List<MyGuiScreenBase>();
            m_screensToAdd = new List<MyGuiScreenBase>();

            m_oldVisPos = MyGuiInput.GetMousePosition();
            m_oldNonVisPos = new Vector2(MyMinerGame.ScreenSizeHalf.X, MyMinerGame.ScreenSizeHalf.Y);

            MyClientServer.OnLoggedPlayerChanged += OnLoggedUserChanged;

            m_renderSetup = new MyRender.MyRenderSetup();
            m_renderSetup.EnableShadowInterleaving = false;
            m_renderSetup.CallerID = MyRenderCallerEnum.Main;

            MyGuiManager.AllowedThread = System.Threading.Thread.CurrentThread;
        }

        public static void SetHWCursorBitmap(System.Drawing.Bitmap b)
        {
            System.Windows.Forms.Form f = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(MyMinerGame.Static.Window.NativeWindow.Handle);
            f.Cursor = new System.Windows.Forms.Cursor(b.GetHicon());
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public static void LoadData()
        {
            m_screens.Clear();
            m_screensToRemove.Clear();
            m_screensToAdd.Clear();

            MyTextsWrapper.ActualLanguage = MyConfig.Language;
            MyDialoguesWrapper.ActualLanguage = MyConfig.Language;
            MySubtitles.Enabled = MyConfig.Subtitles;
        }

        public static bool IsSpriteBatchReady
        {
            get { return m_spriteBatch != null; }
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyGuiManager.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            MyVertexFormats.LoadContent(MyMinerGame.Static.GraphicsDevice);

            BlendState.LoadContent(MyMinerGame.Static.GraphicsDevice);
            RasterizerState.LoadContent(MyMinerGame.Static.GraphicsDevice);
            SamplerState.LoadContent(MyMinerGame.Static.GraphicsDevice);
            DepthStencilState.LoadContent(MyMinerGame.Static.GraphicsDevice);

            m_blankTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Blank", flags: TextureFlags.IgnoreQuality);
            //m_textboxSmallTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\TextboxSmall", flags: TextureFlags.IgnoreQuality);
            m_textboxMediumTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\TextboxMedium", flags: TextureFlags.IgnoreQuality);
            //m_textboxLargeTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\TextboxLarge", flags: TextureFlags.IgnoreQuality);
            m_buttonTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Button", flags: TextureFlags.IgnoreQuality);
            m_buttonTextureBg = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ButtonBg", flags: TextureFlags.IgnoreQuality);
            m_searchButtonTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\SearchButton", flags: TextureFlags.IgnoreQuality);
            m_smallButtonTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\SmallButton", flags: TextureFlags.IgnoreQuality);

            m_travelButtonTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\TravelButton", flags: TextureFlags.IgnoreQuality);

            m_comboboxMediumTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ComboboxMedium", flags: TextureFlags.IgnoreQuality);

            m_comboboxMediumItemTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ComboboxMediumItemTexture", flags: TextureFlags.IgnoreQuality);
            //m_comboboxMediumSelectedTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ComboboxMediumSelectedTexture", flags: TextureFlags.IgnoreQuality);
            m_comboboxMediumTopTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ComboboxMediumTopTexture", flags: TextureFlags.IgnoreQuality);
            m_comboboxMediumBottomTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ComboboxMediumBottomTexture", flags: TextureFlags.IgnoreQuality);

            m_scrollbarSlider = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ScrollbarSlider", flags: TextureFlags.IgnoreQuality);
            m_horizontalScrollbarSlider = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\HorizontalScrollbarSlider", flags: TextureFlags.IgnoreQuality);
            m_mouseCursorBitmap = System.Drawing.Bitmap.FromFile(Path.Combine(MyMinerGame.Static.RootDirectory, "Textures\\GUI\\MouseCursorHW.png")) as System.Drawing.Bitmap;
            SetHWCursorBitmap(m_mouseCursorBitmap);
            m_mouseCursorArrowTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MouseCursor", flags: TextureFlags.IgnoreQuality);
            SetMouseCursorTexture(m_mouseCursorArrowTexture);
            m_mouseCursorHandTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MouseCursorHand", flags: TextureFlags.IgnoreQuality);
            m_lockedButtonTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\LockedButton", flags: TextureFlags.IgnoreQuality);
            m_lockedInventoryItemTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\LockedInventoryButton", flags: TextureFlags.IgnoreQuality);
            m_sliderTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Slider", flags: TextureFlags.IgnoreQuality);
            m_sliderControlTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\SliderControl", flags: TextureFlags.IgnoreQuality);
            //m_configWheelBackground = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ConfigWheelBackground", flags: TextureFlags.IgnoreQuality);
            //m_configWheelHover = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ConfigWheelHover", flags: TextureFlags.IgnoreQuality);
            //m_configWheelBackgroundSmall = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ConfigWheelSmallBackground", flags: TextureFlags.IgnoreQuality);
            m_randomTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RandomTexture", flags: TextureFlags.IgnoreQuality);
            m_checkBoxOnTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\CheckboxOn", flags: TextureFlags.IgnoreQuality);
            m_checkBoxOffTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\CheckboxOff", flags: TextureFlags.IgnoreQuality);
            m_comboboxSelectedTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ComboboxSelected", flags: TextureFlags.IgnoreQuality);
            m_ammoToolTipTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\AmmoToolTip", flags: TextureFlags.IgnoreQuality);
            m_ammoSelectLowBackgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\LowerBackground", flags: TextureFlags.IgnoreQuality);
            m_ammoSelectKeyConfirmBorderTexture = MyTextureManager.GetTexture<MyTexture2D>("textures\\GUI\\AmmoSelectionKeyConfirm", flags: TextureFlags.IgnoreQuality);
            m_fogTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\fog", flags: TextureFlags.IgnoreQuality);
            m_fogSmallTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\FogSmall", flags: TextureFlags.IgnoreQuality);
            m_fogSquareTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\fogSquare", flags: TextureFlags.IgnoreQuality);
            m_textureInterlanced = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackGroundScreen\\InterlaceTextureOverlap", flags: TextureFlags.IgnoreQuality);
            m_expandTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Expand", flags: TextureFlags.IgnoreQuality);
            m_collapseTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Collapse", flags: TextureFlags.IgnoreQuality);
            m_toolbarButton = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Toolbar\\ToolbarButton", flags: TextureFlags.IgnoreQuality);
            m_toolbarButtonHover = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Toolbar\\ToolbarButtonHover", flags: TextureFlags.IgnoreQuality);
            m_progressBarTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ProgressBar", flags: TextureFlags.IgnoreQuality);


            m_journalFilterAll = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\JournalFilterAll", flags: TextureFlags.IgnoreQuality);
            m_journalFilterGlobalEvents = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\JournalFilterGlobalEvents", flags: TextureFlags.IgnoreQuality);
            m_journalFilterMissions = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\JournalFilterMissions", flags: TextureFlags.IgnoreQuality);
            m_journalFilterStory = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\JournalFilterStory", flags: TextureFlags.IgnoreQuality);
            m_journalLine = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\line", flags: TextureFlags.IgnoreQuality);
            m_journalSelected = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\select", flags: TextureFlags.IgnoreQuality);
            m_journalCloseButton = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Journal\\CloseButton", flags: TextureFlags.IgnoreQuality);


            //m_inventoryScreenBackgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\InventoryScreenBackground", flags: TextureFlags.IgnoreQuality);
            m_inventoryButton = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\Button", flags: TextureFlags.IgnoreQuality);
            m_inventoryButtonBg = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\ButtonBg", flags: TextureFlags.IgnoreQuality);
            m_inventoryButtonTakeAll = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\ButtonTakeAll", flags: TextureFlags.IgnoreQuality);
            m_inventoryButtonTakeAllBg = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\ButtonTakeAllBg", flags: TextureFlags.IgnoreQuality);


            m_inventoryFilterSortAllOff = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortAllOff", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortAllOn = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortAllOn", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortConsumablesOff = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortConsumablesOff", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortConsumablesOn = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortConsumablesOn", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortEquipmentOff = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortEquipmentOff", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortEquipmentOn = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortEquipmentOn", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortGodsOff = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortGodsOff", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortGodsOn = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortGodsOn", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortOresOff = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortOresOff", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortOresOn = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortOresOn", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortWeaponsOff = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortWeaponsOff", flags: TextureFlags.IgnoreQuality);
            m_inventoryFilterSortWeaponsOn = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\SortWeaponsOn", flags: TextureFlags.IgnoreQuality);
            m_inventoryPreviousShip = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\PreviousShip", flags: TextureFlags.IgnoreQuality);
            m_inventoryNextShip = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\NextShip", flags: TextureFlags.IgnoreQuality);


            //hub controls
            //m_hubBackground = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\HubBackground", flags: TextureFlags.IgnoreQuality);
            m_hubItemBackground = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\HubItemBackground", flags: TextureFlags.IgnoreQuality);
            m_hubButton = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\HubButton", flags: TextureFlags.IgnoreQuality);
            m_hubButtonBg = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\HubButtonBg", flags: TextureFlags.IgnoreQuality);
            m_MessageButton = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\MessageBoxButton", flags: TextureFlags.IgnoreQuality);

            //m_backgroundInterferenceSpriteVideoPlayer = new MySpriteListVideoPlayer("BackgroundInterferenceVid", MyMinerGame.GraphicsDeviceManager.GraphicsDevice,
            //   20, 0, 92);
            //m_backgroundInterferenceSpriteVideoPlayer.LoadContent();
            m_toltipLeft = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\TooltipLeft", flags: TextureFlags.IgnoreQuality);
            m_toltipBody = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\TooltipBody", flags: TextureFlags.IgnoreQuality);
            m_toltipRight = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\TooltipRight", flags: TextureFlags.IgnoreQuality);


            m_dialoguePortraitNoiseVideoPlayer = new MySpriteListVideoPlayer("DialoguePortraitNoise", MyMinerGame.GraphicsDeviceManager.GraphicsDevice, 10, 0, 9);
            m_dialoguePortraitNoiseVideoPlayer.LoadContent();

            m_backgroundScreenTextures = new List<MyGuiTextureScreen>
            {
                new MyGuiTextureScreen("Textures\\GUI\\BackgroundScreen\\Screen", 811, 811), 
                new MyGuiTextureScreen("Textures\\GUI\\BackgroundScreen\\ScreenWide", 811, 410), 
                new MyGuiTextureScreen("Textures\\GUI\\BackgroundScreen\\ScreenTall", 411, 810)
            };
            m_dialogueFriendBackgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\DialogueFriend", flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
            m_dialogueEnemyBackgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\DialogueEnemy", flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
            m_dialogueNeutralBackgroundTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\DialogueNeutral", flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
            //m_SelectEditorbackground = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\SelectEditorBakground", flags: TextureFlags.IgnoreQuality);
            //m_SandboxBackground = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\SandBoxbackground", flags: TextureFlags.IgnoreQuality);

            MyTextureManager.OverrideLoadingMode = LoadingMode.LazyBackground;
            MyGuiObjectBuilderHelpers.LoadContent();
            MyGuiSmallShipHelpers.LoadContent();
            MyGuiAsteroidHelpers.LoadContent();
            MyGuiLargeShipHelpers.LoadContent();
            MyGuiSmallDebrisHelpers.LoadContent();
            MyGuiPrefabHelpers.LoadContent();
            MyGuiContextMenuHelpers.LoadDefaultEditorContent();
            MyGuiInfluenceSphereHelpers.LoadContent();
            MyGuiEditorVoxelHandHelpers.LoadContent();
            MyGuiGameControlsHelpers.LoadContent();
            MyGuiEventHelpers.LoadContent();
            MyGuiInventoryTemplateTypeHelpers.LoadContent();
            MyTextureManager.OverrideLoadingMode = null;


            /*switch (MyConfig.Language)
            {
                case MyLanguagesEnum.Cesky:
                    m_fontCaptions = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\Captions\\Captions-de.xml");
                    m_fontGuiImpactLarge = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\GuiImpactLarge\\GuiImpactLarge-uni.xml");
                    break;
                case MyLanguagesEnum.Deutsch:
                    m_fontCaptions = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\Captions\\Captions-de.xml");
                    m_fontGuiImpactLarge = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\GuiImpactLarge\\GuiImpactLarge-uni.xml");
                    break;
                case MyLanguagesEnum.Slovensky:
                    m_fontCaptions = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\Captions\\Captions-de.xml");
                    m_fontGuiImpactLarge = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\GuiImpactLarge\\GuiImpactLarge-uni.xml");
                    break;
                case MyLanguagesEnum.Espanol:
                    m_fontCaptions = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\Captions\\Captions-de.xml");
                    m_fontGuiImpactLarge = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\GuiImpactLarge\\GuiImpactLarge-uni.xml");
                    break;
                case MyLanguagesEnum.France:
                    m_fontCaptions = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\Captions\\Captions-de.xml");
                    m_fontGuiImpactLarge = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\GuiImpactLarge\\GuiImpactLarge-uni.xml");
                    break;
                case MyLanguagesEnum.Italian:
                    m_fontCaptions = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\Captions\\Captions-de.xml");
                    m_fontGuiImpactLarge = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\GuiImpactLarge\\GuiImpactLarge-uni.xml");
                    break;
                case MyLanguagesEnum.English:
                default:
                    m_fontCaptions = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\Captions\\Captions.xml");
                    m_fontGuiImpactLarge = new MyGuiFont((MyCustomContentManager)MyMinerGame.Static.Content, "Fonts\\GuiImpactLarge\\GuiImpactLarge.xml");
                    break;
            }*/

            m_fontMinerWarsRed = new MyGuiFont("Fonts\\MinerWars-red\\MinerWars.xml");
            m_fontMinerWarsRed.Spacing = 1;
            m_fontMinerWarsGreen = new MyGuiFont("Fonts\\MinerWars-green\\MinerWars.xml");
            m_fontMinerWarsGreen.Spacing = 1;
            m_fontMinerWarsBlue = new MyGuiFont("Fonts\\MinerWars-blue\\MinerWars.xml");
            m_fontMinerWarsBlue.Spacing = 1;
            m_fontMinerWarsWhite = new MyGuiFont("Fonts\\MinerWars-white\\MinerWars.xml");
            m_fontMinerWarsWhite.Spacing = 1;

            if (m_input != null)
            {
                m_input.Dispose();
                m_input = null;

            }
            if (m_input == null)
                m_input = new MyGuiInput();

            MouseCursorPosition = new Vector2(0.5f, 0.5f);// new MyMwcVector2Int(MyMinerGame.ScreenSizeHalf.X, MyMinerGame.ScreenSizeHalf.Y);

            m_fullscreenQuad = new MyFullScreenQuad();
            //m_spriteBatchOwn = new MyGuiSpriteBatchOwn();
            m_spriteBatch = new SpriteBatch(MyMinerGame.Static.GraphicsDevice, "GuiManager");
            m_spriteBatchUsageCount = 0;

            //m_spriteEffect = new MyEffectSpriteBatchOriginal();

            //load/reload content of all screens in first call any screens should not exist
            foreach (MyGuiScreenBase screen in m_screens)
            {
                screen.LoadContent();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiManager.LoadContent() - END");
        }


        private static void UpdateClipForCursor(System.Windows.Forms.Form f)
        {
            if (MyMinerGame.Static.Window.NativeWindow.Handle == GetForegroundWindow() && MyGuiManager.GetScreenWithFocus() != null)
                System.Windows.Forms.Cursor.Clip = f.RectangleToScreen(f.ClientRectangle);
        }

        private static void EnableSoundsBasedOnWindowFocus()
        {
            if (MyMinerGame.Static.Window.NativeWindow.Handle == GetForegroundWindow() && MyGuiManager.GetScreenWithFocus() != null)
            { // allow
                // this works bad (	0007128: BUG B - audio sliders are broken)
                //MyAudio.SetAllVolume(MyConfig.GameVolume, MyConfig.MusicVolume);         
                MyAudio.Mute = false;
            }
            else // mute
            {
                // this works bad (	0007128: BUG B - audio sliders are broken)
                //MyAudio.SetAllVolume(0,0);
                MyAudio.Mute = true;
            }
        }


        public static void LoadPrefabPreviews()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyGuiManager.LoadPrefabPreviews");

            foreach (MyMwcObjectBuilderTypeEnum enumValue in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
            {
                int[] prefabIds = MyMwcObjectBuilder_Base.GetObjectBuilderIDs(enumValue);
                foreach (int prefabId in prefabIds)
                {
                    MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(enumValue, prefabId);
                    string fileName = MyGuiPreviewRenderer.GetPreviewFileName(config, prefabId);

                    foreach (MyMwcObjectBuilder_Prefab_AppearanceEnum appearance in MyGuiPrefabHelpers.MyMwcFactionTextureEnumValues)
                    {
                        string path = Path.Combine("Textures\\GUI\\Prefabs", "v" + String.Format("{0:00}", (ushort)appearance + 1), fileName);

                        string filePath = MyMinerGame.Static.RootDirectory + "\\" + path + ".dds";
                        if (File.Exists(filePath))
                        {
                            MyTexture2D texture = MyTextureManager.GetTexture<MyTexture2D>(path, flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
                            AddPrefabPreview(enumValue, prefabId, appearance, texture);
                        }
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void AddPrefabPreview(MyMwcObjectBuilderTypeEnum prefabType, int prefabId, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, MyTexture2D texture)
        {
            if (m_prefabPreviews[(int)prefabType] == null)
            {
                m_prefabPreviews[(int)prefabType] = new MyTexture2D[MyMwcObjectBuilder_Base.GetObjectBuilderIDs(prefabType).Max() + 1][];
            }
            if (m_prefabPreviews[(int)prefabType][prefabId] == null)
            {
                m_prefabPreviews[(int)prefabType][prefabId] = new MyTexture2D[MyGuiPrefabHelpers.MyMwcFactionTextureEnumValues.Length];
            }
            m_prefabPreviews[(int)prefabType][prefabId][(ushort)appearance] = texture;
        }

        private static MyTexture2D GetPrefabPreview(MyMwcObjectBuilderTypeEnum prefabType, int prefabId, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance)
        {
            MyTexture2D texture = null;
            // TODO: Wouldn't be better to use compound key? (for example long when int would be too small)
            if (m_prefabPreviews != null && m_prefabPreviews[(int)prefabType] != null && m_prefabPreviews[(int)prefabType][prefabId] != null)
            {
                texture = m_prefabPreviews[(int)prefabType][prefabId][(ushort)appearance];
            }
            return texture;
        }

        public static void UnloadContent()
        {
            UnloadContentOnAllScreens();

            if (m_spriteBatch != null)
            {
                m_spriteBatch.Dispose();
                m_spriteBatch = null;
            }

            if (m_fullscreenQuad != null)
            {
                m_fullscreenQuad.Dispose();
                m_fullscreenQuad = null;
            }

            MyGuiObjectBuilderHelpers.UnloadContent();
            MyGuiSmallShipHelpers.UnloadContent();
            MyGuiAsteroidHelpers.UnloadContent();
            MyGuiLargeShipHelpers.UnloadContent();
            MyGuiSmallDebrisHelpers.UnloadContent();
            MyGuiPrefabHelpers.UnloadContent();
            MyGuiContextMenuHelpers.UnloadContent();
            MyGuiInfluenceSphereHelpers.UnloadContent();
            MyGuiEditorVoxelHandHelpers.UnloadContent();
            MyGuiGameControlsHelpers.UnloadContent();
            MyGuiEventHelpers.UnloadContent();
            MyGuiInventoryTemplateTypeHelpers.UnloadContent();

            //m_backgroundInterferenceSpriteVideoPlayer.UnloadContent();
            //m_backgroundInterferenceSpriteVideoPlayer = null;
            if (m_dialoguePortraitNoiseVideoPlayer != null)
            {
                m_dialoguePortraitNoiseVideoPlayer.UnloadContent();
                m_dialoguePortraitNoiseVideoPlayer = null;
            }

            BlendState.UnloadContent();
            RasterizerState.UnloadContent();
            SamplerState.UnloadContent();
            DepthStencilState.UnloadContent();

            MyVertexFormats.UnloadContent();

            //m_spriteEffect.Dispose();
            //m_spriteEffect = null;
        }

        public static void UnloadContentOnAllScreens()
        {
            //  First unload threads that may have running background threads (because these threads may be accessing graphic device or some contents...)
            UnloadContent(true);

            //  Then all other threads
            UnloadContent(false);
        }

        static void UnloadContent(bool unloadLoadingScreens)
        {
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if ((screen is MyGuiScreenLoading) == unloadLoadingScreens)
                {
                    screen.UnloadContent();
                }
            }
        }

        public static void UpdateScreenSize()
        {
            int safeGuiSizeY = MyMinerGame.ScreenSize.Y;
            int safeGuiSizeX = (int)(safeGuiSizeY * 1.3333f);     //  This will mantain same aspect ratio for GUI elements

            int safeFullscreenSizeX = MyMinerGame.ScreenSize.X;
            int safeFullscreenSizeY = MyMinerGame.ScreenSize.Y;

            m_fullscreenRectangle = new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y);

            //  Triple head is drawn on three monitors, so we will draw GUI only on the middle one

            if (MyVideoModeManager.IsTripleHead() == true)
            {
                const int TRIPLE_SUB_SCREENS_COUNT = 3;
                //safeGuiSizeX = safeGuiSizeX / TRIPLE_SUB_SCREENS_COUNT;
                safeFullscreenSizeX = safeFullscreenSizeX / TRIPLE_SUB_SCREENS_COUNT;
            }


            m_safeGuiRectangle = new Rectangle(MyMinerGame.ScreenSize.X / 2 - safeGuiSizeX / 2, 0, safeGuiSizeX, safeGuiSizeY);

            //if (MyVideoModeManager.IsTripleHead() == true)
            //m_safeGuiRectangle.X += MyMinerGame.ScreenSize.X / 3;

            m_safeFullscreenRectangle = new Rectangle(MyMinerGame.ScreenSize.X / 2 - safeFullscreenSizeX / 2, 0, safeFullscreenSizeX, safeFullscreenSizeY);

            //  This will help as maintain scale/ratio of images, texts during in different resolution
            m_safeScreenScale = (float)safeGuiSizeY / MyGuiConstants.REFERENCE_SCREEN_HEIGHT;

            //  Min and max mouse coords (in normalized units). Must be calculated from fullscreen, no GUI rectangle.
            m_minMouseCoord = GetNormalizedCoordinateFromScreenCoordinate(new Vector2(m_safeFullscreenRectangle.Left, m_safeFullscreenRectangle.Top));
            m_maxMouseCoord = GetNormalizedCoordinateFromScreenCoordinate(new Vector2(m_safeFullscreenRectangle.Left + m_safeFullscreenRectangle.Width, m_safeFullscreenRectangle.Top + m_safeFullscreenRectangle.Height));
            m_minMouseCoordFullscreenHud = GetNormalizedCoordinateFromScreenCoordinate(new Vector2(m_fullscreenRectangle.Left, m_fullscreenRectangle.Top));
            m_maxMouseCoordFullscreenHud = GetNormalizedCoordinateFromScreenCoordinate(new Vector2(m_fullscreenRectangle.Left + m_fullscreenRectangle.Width, m_fullscreenRectangle.Top + m_fullscreenRectangle.Height));

            //  Normalized coordinates where width is always 1.0 and height something like 0.8
            //  Don't confuse with GUI normalized coordinates. They are different.
            //  HUD - get normalized screen size -> width is always 1.0, but height depends on aspect ratio, so usualy it is 0.8 or something.
            m_hudSize = CalculateHudSize();
            m_hudSizeHalf = m_hudSize / 2.0f;
        }

        //  Because only main menu's controla depends on fullscreen pixel coordinates (not normalized), after we change
        //  screen resolution we need to recreate controls too. Otherwise they will be still on old/bad positions, and
        //  for example when changing from 1920x1200 to 800x600 they would be out of screen
        public static void RecreateMainMenuControls()
        {
            //  GUI probably not initialized yet
            if (m_screens == null) return;

            for (int i = 0; i < m_screens.Count; i++)
            {
                if (m_screens[i] is MyGuiScreenMainMenu)
                {
                    ((MyGuiScreenMainMenu)m_screens[i]).RecreateControls(false);
                }
            }
        }

        public static void RecreateControls()
        {
            //  GUI probably not initialized yet
            if (m_screens == null) return;

            for (int i = 0; i < m_screens.Count; i++)
            {
                m_screens[i].RecreateControls(false);
            }
        }

        //  Close screen of specified type with fade-out effect (ignores inheritance, base class, derived classes)
        public static void CloseScreen(Type screenType)
        {
            //  GUI probably not initialized yet
            if (m_screens == null) return;

            for (int i = 0; i < m_screens.Count; i++)
            {
                if (m_screens[i].GetType() == screenType)
                {
                    m_screens[i].CloseScreen();
                }
            }
        }

        //  Close screen of specified type - instantly, without fade-out effect
        public static void CloseScreenNow(Type screenType)
        {
            //  GUI probably not initialized yet
            if (m_screens == null) return;

            for (int i = 0; i < m_screens.Count; i++)
            {
                if (m_screens[i].GetType() == screenType)
                {
                    m_screens[i].CloseScreenNow();
                }
            }
        }

        //  Normalized size of screen for HUD (we are geting it from GUI, because GUI knows safe screen)
        static Vector2 CalculateHudSize()
        {
            return new Vector2(1.0f, (float)m_safeFullscreenRectangle.Height / (float)m_safeFullscreenRectangle.Width);
        }

        public static float GetSafeScreenScale()
        {
            return m_safeScreenScale;
        }

        public static Rectangle GetSafeGuiRectangle()
        {
            return m_safeGuiRectangle;
        }

        public static Rectangle GetSafeFullscreenRectangle()
        {
            return m_safeFullscreenRectangle;
        }

        public static Rectangle GetFullscreenRectangle()
        {
            return m_fullscreenRectangle;
        }

        public static Viewport GetBackwardViewport()
        {
            Viewport ret = MyMinerGame.Static.GraphicsDevice.Viewport;
            ret.Height = (int)(m_safeFullscreenRectangle.Height * MyHudConstants.BACK_CAMERA_HEIGHT);
            ret.Y = (int)(m_safeFullscreenRectangle.Height * MyGuiConstants.HUD_FREE_SPACE.Y);
            ret.Width = (int)(ret.Height * MyHudConstants.BACK_CAMERA_ASPECT_RATIO);
            ret.X = m_safeFullscreenRectangle.Left + m_safeFullscreenRectangle.Width - ret.Width - (int)(m_safeFullscreenRectangle.Width * MyGuiConstants.HUD_FREE_SPACE.X);
            return ret;
        }

        public static Viewport GetHudViewport()
        {
            Viewport ret = MyMinerGame.Static.GraphicsDevice.Viewport;
            ret.X = m_safeFullscreenRectangle.Left;
            ret.Width = m_safeFullscreenRectangle.Width;
            return ret;
        }

        public static Viewport GetFullscreenHudViewport()
        {
            // it's the same as Forward viewport
            Viewport ret = MyMinerGame.Static.GraphicsDevice.Viewport;
            return ret;
        }

        public static MyGuiInput GetInput()
        {
            return m_input;
        }

        // Draws string with default label text color
        public static MyRectangle2D DrawString(MyGuiFont font, StringBuilder sb, Vector2 normalizedCoord, float scale, MyGuiDrawAlignEnum drawAlign, bool fullscreen = false)
        {
            return DrawString(font, sb, normalizedCoord, scale, new Color(MyGuiConstants.LABEL_TEXT_COLOR), drawAlign, fullscreen);
        }

        //  Draws string (string builder) at specified position
        //  normalizedPosition -> X and Y are within interval <0..1>
        //  scale -> scale for original texture, it's not in pixel/texels, but multiply of original size. E.g. 1 means unchanged size, 2 means double size. Scale is uniform, preserves aspect ratio.
        //  RETURN: Method returns rectangle where was string drawn in normalized coordinates
        public static MyRectangle2D DrawString(MyGuiFont font, StringBuilder sb, Vector2 normalizedCoord, float scale, Color color, MyGuiDrawAlignEnum drawAlign, bool fullscreen = false)
        {
            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord, fullscreen);

            //  Fix the scale for screen resolution
            float fixedScale = scale * m_safeScreenScale;

            //  IMPORTANT: MeasureString is weird method, because for example it returns height 56 pixels, but DrawString actualy draws 33 pixels tall characters
            //  And SpriteFont.LineSpacing gives me 53 pixels... so am quite sure MeasureString returns height of line and not of characters, but then why is it less than LineSpacing???
            Vector2 sizeInPixelsScaled = font.MeasureString(sb, fixedScale);

            screenCoord = GetAlignedCoordinate(screenCoord, sizeInPixelsScaled, drawAlign);

            //  IMPORTANT: We don't draw shadows under text here because shadows are already contained in font texture!

            //MinerWars.AppCode.App.MyMinerGame.GraphicsDeviceManager.GraphicsDevice.

            //font.DrawString(screenCoord, /*color*/ Color.White, sb, fixedScale);
            font.DrawString(screenCoord, color, sb, fixedScale);

            return new MyRectangle2D(GetNormalizedCoordinateFromScreenCoordinate(screenCoord), GetNormalizedSizeFromScreenSize(sizeInPixelsScaled));
        }

        //  Draws string (string builder) at specified position
        //  normalizedPosition -> X and Y are within interval <0..1>
        //  scale -> scale for original texture, it's not in pixel/texels, but multiply of original size. E.g. 1 means unchanged size, 2 means double size. Scale is uniform, preserves aspect ratio.
        //  RETURN: Method returns rectangle where was string drawn in normalized coordinates
        public static MyRectangle2D DrawStringCentered(MyGuiFont font, StringBuilder sb, Vector2 normalizedCoord, float scale, Color color, MyGuiDrawAlignEnum drawAlign)
        {
            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord);

            //  Fix the scale for screen resolution
            float fixedScale = scale * m_safeScreenScale;

            //  IMPORTANT: We don't draw shadows under text here because shadows are already contained in font texture!

            int lineCount = 0;
            foreach (var line in sb.Split('\n'))
            {
                line.TrimTrailingWhitespace();
                var lineSizeInPixelsScaled = font.MeasureString(line, fixedScale);
                var lineCoord = GetAlignedCoordinate(screenCoord + new Vector2(0, lineCount * font.LineHeight * fixedScale), lineSizeInPixelsScaled, drawAlign);
                font.DrawString(lineCoord, color, line, fixedScale);
                lineCount++;
            }

            //  IMPORTANT: MeasureString is weird method, because for example it returns height 56 pixels, but DrawString actualy draws 33 pixels tall characters
            //  And SpriteFont.LineSpacing gives me 53 pixels... so am quite sure MeasureString returns height of line and not of characters, but then why is it less than LineSpacing???
            Vector2 sizeInPixelsScaled = font.MeasureString(sb, fixedScale);
            screenCoord = GetAlignedCoordinate(screenCoord, sizeInPixelsScaled, drawAlign);
            return new MyRectangle2D(GetNormalizedCoordinateFromScreenCoordinate(screenCoord), GetNormalizedSizeFromScreenSize(sizeInPixelsScaled));
        }

        public static MyRectangle2D MeasureString(MyGuiFont font, StringBuilder sb, Vector2 normalizedCoord, float scale, MyGuiDrawAlignEnum drawAlign)
        {
            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord);

            //  Fix the scale for screen resolution
            float fixedScale = scale * m_safeScreenScale;

            //  IMPORTANT: MeasureString is weird method, because for example it returns height 56 pixels, but DrawString actualy draws 33 pixels tall characters
            //  And SpriteFont.LineSpacing gives me 53 pixels... so am quite sure MeasureString returns height of line and not of characters, but then why is it less than LineSpacing???
            Vector2 sizeInPixelsScaled = font.MeasureString(sb, fixedScale);
            screenCoord = GetAlignedCoordinate(screenCoord, sizeInPixelsScaled, drawAlign);

            return new MyRectangle2D(GetNormalizedCoordinateFromScreenCoordinate(screenCoord), GetNormalizedSizeFromScreenSize(sizeInPixelsScaled));
        }

        //  Draws "our own" sprite at specified SCREEN position (in screen coordinates, not normalized coordinates).
        //  Doesn't use XNA's sprite batch because that one messes up with render states and we don't want to restore them everytime time.
        //  Uses our own sprite effect and quad renderer.
        public static void DrawSpriteFast(Texture texture, int x, int y, int width, int height, Color color)
        {
            //m_spriteBatchOwn.Draw(texture, new Rectangle(x, y, width, height), color);
            m_spriteBatch.Begin(SpriteSortMode.Immediate);
            m_spriteBatch.Draw(texture, new SharpDX.DrawingRectangle(x, y, width, height), SharpDXHelper.ToSharpDX(color));
            m_spriteBatch.End();
        }

        public static void DrawSpriteFast(CubeTexture texture, CubeMapFace face, int x, int y, int width, int height, Color color)
        {
            m_spriteBatch.Begin(SpriteSortMode.Immediate);
            m_spriteBatch.Draw(texture, face, new SharpDX.DrawingRectangle(x, y, width, height), SharpDXHelper.ToSharpDX(color));
            m_spriteBatch.End();
        }

        //  Draws sprite batch at specified SCREEN position (in screen coordinates, not normalized coordinates).
        public static void DrawSpriteBatch(Texture texture, int x, int y, int width, int height, Color color)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            m_spriteBatch.Draw(texture, new DrawingRectangle(x, y, width, height), SharpDXHelper.ToSharpDX(color));
        }

        //  Draws sprite batch at specified SCREEN position (in screen coordinates, not normalized coordinates).
        public static void DrawSpriteBatch(Texture texture, Rectangle dest, Color color)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            m_spriteBatch.Draw(texture, SharpDXHelper.ToSharpDX(dest), SharpDXHelper.ToSharpDX(color));
        }

        //  Draws sprite batch at specified SCREEN position (in screen coordinates, not normalized coordinates).
        public static void DrawSpriteBatch(Texture texture, Vector2 pos, Color color)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            m_spriteBatch.Draw(texture, SharpDXHelper.ToSharpDX(pos), SharpDXHelper.ToSharpDX(color));
        }

        //  Draws sprite batch at specified SCREEN position (in screen coordinates, not normalized coordinates).
        public static void DrawSpriteBatch(Texture texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            m_spriteBatch.Draw(texture, SharpDXHelper.ToSharpDX(position), SharpDXHelper.ToSharpDX(sourceRectangle), SharpDXHelper.ToSharpDX(color), rotation, SharpDXHelper.ToSharpDX(origin), SharpDXHelper.ToSharpDX(scale), effects, layerDepth);
        }

        //  Draws sprite batch at specified SCREEN position (in screen coordinates, not normalized coordinates).
        public static void DrawSpriteBatch(Texture texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            m_spriteBatch.Draw(texture, SharpDXHelper.ToSharpDX(position), SharpDXHelper.ToSharpDX(sourceRectangle), SharpDXHelper.ToSharpDX(color), rotation, SharpDXHelper.ToSharpDX(origin), scale, effects, layerDepth);
        }
        /*
//  Draws sprite batch at specified SCREEN position (in screen coordinates, not normalized coordinates).
public static void DrawSpriteBatch(Texture texture, Rectangle dest, Rectangle source, Color color)
{
System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
if (texture == null)
  return;

m_spriteBatch.Draw(texture, SharpDXHelper.ToSharpDX(dest), SharpDXHelper.ToSharpDX(source), SharpDXHelper.ToSharpDX(color));
}
          */
        //  Draws sprite batch at specified NORMALIZED position, with SCREEN-PIXEL width, but NORMALIZED height
        //  Use if you want to draw rectangle where width is in screen coords, but rest is in normalized coord (e.g. textbox carriage blinker)
        public static void DrawSpriteBatch(Texture texture, Vector2 normalizedCoord, int screenWidth, float normalizedHeight, Color color, MyGuiDrawAlignEnum drawAlign)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord);
            Vector2 screenSize = GetScreenSizeFromNormalizedSize(new Vector2(0, normalizedHeight));
            screenSize.X = screenWidth; //  Replace with desired value
            screenCoord = GetAlignedCoordinate(screenCoord, screenSize, drawAlign);
            m_spriteBatch.Draw(texture, new DrawingRectangle((int)screenCoord.X, (int)screenCoord.Y, (int)screenSize.X, (int)screenSize.Y), SharpDXHelper.ToSharpDX(color));
        }

        //  Draws sprite batch at specified NORMALIZED position, with NORMALIZED width, but SCREEN-PIXEL height.
        //  Use if you want to draw rectangle where height is in screen coords, but rest is in normalized coord (e.g. slider long line)
        public static void DrawSpriteBatch(Texture texture, Vector2 normalizedCoord, float normalizedWidth, int screenHeight, Color color, MyGuiDrawAlignEnum drawAlign)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord);
            Vector2 screenSize = GetScreenSizeFromNormalizedSize(new Vector2(normalizedWidth, 0));
            screenSize.Y = screenHeight; //  Replace with desired value
            screenCoord = GetAlignedCoordinate(screenCoord, screenSize, drawAlign);
            m_spriteBatch.Draw(texture, new DrawingRectangle((int)screenCoord.X, (int)screenCoord.Y, (int)screenSize.X, (int)screenSize.Y), SharpDXHelper.ToSharpDX(color));
        }

        //  Draws sprite batch at specified position
        //  normalizedPosition -> X and Y are within interval <0..1>
        //  size -> size of destination rectangle (normalized). Don't forget that it may be distorted by aspect ration, so rectangle size [1,1] can make larger wide than height on your screen.
        //  rotation -> angle in radians. Rotation is always around "origin" coordinate
        //  originNormalized -> the origin of the sprite. Specify (0,0) for the upper-left corner.
        //  RETURN: Method returns rectangle where was sprite/texture drawn in normalized coordinates

        public static void DrawSpriteBatch(Texture texture, Vector2 normalizedCoord, Vector2 normalizedSize, Color color, MyGuiDrawAlignEnum drawAlign, bool fullscreen = false)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);

            if (texture == null)
                return;

            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord, fullscreen);

            Vector2 screenSize = fullscreen ? new Vector2((m_safeFullscreenRectangle.Width + 1) * normalizedSize.X, m_safeFullscreenRectangle.Height * normalizedSize.Y) : GetScreenSizeFromNormalizedSize(normalizedSize);
            //Vector2 screenSize = GetScreenSizeFromNormalizedSize(normalizedSize);
            screenCoord = GetAlignedCoordinate(screenCoord, screenSize, drawAlign);
            m_spriteBatch.Draw(texture, new DrawingRectangle((int)screenCoord.X, (int)screenCoord.Y, (int)screenSize.X, (int)screenSize.Y), SharpDXHelper.ToSharpDX(color));

            //return new MyRectangle2D(GetNormalizedCoordinateFromScreenCoordinate(screenCoord), normalizedSize);
        }

        // different rounding of coords
        public static void DrawSpriteBatchRoundUp(Texture texture, Vector2 normalizedCoord, Vector2 normalizedSize, Color color, MyGuiDrawAlignEnum drawAlign)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord);
            Vector2 screenSize = GetScreenSizeFromNormalizedSize(normalizedSize);
            screenCoord = GetAlignedCoordinate(screenCoord, screenSize, drawAlign);
            m_spriteBatch.Draw(texture, new DrawingRectangle((int)Math.Floor(screenCoord.X), (int)Math.Floor(screenCoord.Y), (int)Math.Ceiling(screenSize.X), (int)Math.Ceiling(screenSize.Y)), SharpDXHelper.ToSharpDX(color));
            //return new MyRectangle2D(GetNormalizedCoordinateFromScreenCoordinate(screenCoord), normalizedSize);
        }


        /// <summary>
        ///  Draws sprite batch at specified position
        ///  normalizedPosition -> X and Y are within interval <0..1>
        ///  size -> size of destination rectangle (normalized). Don't forget that it may be distorted by aspect ration, so rectangle size [1,1] can make larger wide than height on your screen.
        ///  rotation -> angle in radians. Rotation is always around "origin" coordinate
        ///  originNormalized -> the origin of the sprite. Specify (0,0) for the upper-left corner.
        ///  RETURN: Method returns rectangle where was sprite/texture drawn in normalized coordinates
        /// </summary>
        /// <returns></returns>
        public static void DrawSpriteBatch(Texture texture, Vector2 normalizedCoord, Vector2 normalizedSize, Color color, MyGuiDrawAlignEnum drawAlign, float rotation)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord);
            Vector2 screenSize = GetScreenSizeFromNormalizedSize(normalizedSize);
            screenCoord = GetAlignedCoordinate(screenCoord, screenSize, drawAlign);

            Vector2 origin;
            origin.X = texture.GetLevelDescription(0).Width / 2f;
            origin.Y = texture.GetLevelDescription(0).Height / 2f;

            m_spriteBatch.Draw(texture, new DrawingRectangle((int)screenCoord.X, (int)screenCoord.Y, (int)screenSize.X, (int)screenSize.Y), null, SharpDXHelper.ToSharpDX(color), rotation, SharpDXHelper.ToSharpDX(origin), SpriteEffects.None, 0);

            //return new MyRectangle2D(GetNormalizedCoordinateFromScreenCoordinate(screenCoord), normalizedSize);
        }

        //  Draws sprite batch at specified position
        //  normalizedPosition -> X and Y are within interval <0..1>
        //  scale -> scale for original texture, it's not in pixel/texels, but multiply of original size. E.g. 1 means unchanged size, 2 means double size. Scale is uniform, preserves aspect ratio.
        //  rotation -> angle in radians. Rotation is always around "origin" coordinate
        //  originNormalized -> the origin of the sprite. Specify (0,0) for the upper-left corner.
        //  RETURN: Method returns rectangle where was sprite/texture drawn in normalized coordinates
        public static void DrawSpriteBatch(Texture texture, Vector2 normalizedCoord, float scale, Color color, MyGuiDrawAlignEnum drawAlign, float rotation, Vector2 originNormalized)
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            if (texture == null)
                return;

            Vector2 screenCoord = GetScreenCoordinateFromNormalizedCoordinate(normalizedCoord);

            //  Fix the scale for screen resolution
            float fixedScale = scale * m_safeScreenScale;

            Vector2 sizeInPixels = new Vector2(texture.GetLevelDescription(0).Width, texture.GetLevelDescription(0).Height);
            Vector2 sizeInPixelsScaled = sizeInPixels * fixedScale;

            screenCoord = GetAlignedCoordinate(screenCoord, sizeInPixelsScaled, drawAlign);

            m_spriteBatch.Draw(texture, SharpDXHelper.ToSharpDX(screenCoord), null, SharpDXHelper.ToSharpDX(color), rotation, SharpDXHelper.ToSharpDX(originNormalized * sizeInPixels), fixedScale, SpriteEffects.None, 0);
        }

        //  Find screen texture that best matches aspect ratio of current GUI screen
        public static string GetBackgroundTextureFilenameByAspectRatio(Vector2 normalizedSize)
        {
            Vector2 screenSize = GetScreenSizeFromNormalizedSize(normalizedSize);
            float screenAspectRatio = screenSize.X / screenSize.Y;
            float minDelta = float.MaxValue;
            string ret = null;
            foreach (MyGuiTextureScreen texture in m_backgroundScreenTextures)
            {
                float delta = Math.Abs(screenAspectRatio - texture.GetAspectRatio());
                if (delta < minDelta)
                {
                    minDelta = delta;
                    ret = texture.GetFilename();
                }
            }
            return ret;
        }

        //  Get size of sprite/texture in normalized coordinate <0..1>
        public static Vector2 GetNormalizedSize(MyTexture2D texture, float scale)
        {
            Vector2 sizeInPixels = new Vector2(texture.Width, texture.Height);
            Vector2 sizeScaled = sizeInPixels * scale * m_safeScreenScale;
            return GetNormalizedSizeFromScreenSize(sizeScaled);
        }


        //  Get size of sprite/texture in normalized coordinate <0..1>
        public static Vector2 GetNormalizedSize(Vector2 size, float scale)
        {
            Vector2 sizeScaled = size * scale * m_safeScreenScale;
            return GetNormalizedSizeFromScreenSize(sizeScaled);
        }



        //  Get size of string in normalized coordinate <0..1>
        public static Vector2 GetNormalizedSize(MyGuiFont font, StringBuilder sb, float scale)
        {
            if (font == null)
            {
                return Vector2.Zero;
            }

            Vector2 sizeInPixelsScaled = font.MeasureString(sb, scale * m_safeScreenScale);
            return GetNormalizedSizeFromScreenSize(sizeInPixelsScaled);
        }

        //  Convertes normalized size <0..1> to screen size (pixels)
        public static Vector2 GetScreenSizeFromNormalizedSize(Vector2 normalizedSize)
        {
            return new Vector2((m_safeGuiRectangle.Width + 1) * normalizedSize.X, m_safeGuiRectangle.Height * normalizedSize.Y);
        }

        //  Convertes normalized coodrinate <0..1> to screen coordinate (pixels)
        public static Vector2 GetScreenCoordinateFromNormalizedCoordinate(Vector2 normalizedCoord, bool fullscreen = false)
        {
            if (fullscreen)
            {
                return new Vector2(
                    m_safeFullscreenRectangle.Left + m_safeFullscreenRectangle.Width * normalizedCoord.X,
                    m_safeFullscreenRectangle.Top + m_safeFullscreenRectangle.Height * normalizedCoord.Y);
            }
            else
            {
                return new Vector2(
                    m_safeGuiRectangle.Left + m_safeGuiRectangle.Width * normalizedCoord.X,
                    m_safeGuiRectangle.Top + m_safeGuiRectangle.Height * normalizedCoord.Y);
            }
        }

        //  Convertes screen coordinate (pixels) to normalized coodrinate <0..1>
        public static Vector2 GetNormalizedCoordinateFromScreenCoordinate(Vector2 screenCoord)
        {
            return new Vector2(
                (screenCoord.X - (float)m_safeGuiRectangle.Left) / (float)m_safeGuiRectangle.Width,
                (screenCoord.Y - (float)m_safeGuiRectangle.Top) / (float)m_safeGuiRectangle.Height);
        }

        //  Convertes fullscreen screen coordinate (pixels) to safe-GUI normalized coodrinate <0..1>
        public static Vector2 GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(Vector2 fullScreenCoord)
        {
            return GetNormalizedCoordinateFromScreenCoordinate(
                new Vector2(m_safeFullscreenRectangle.Left + fullScreenCoord.X, m_safeFullscreenRectangle.Top + fullScreenCoord.Y));
        }

        //  Convertes screen size (pixels) to normalized size <0..1>
        public static Vector2 GetNormalizedSizeFromScreenSize(Vector2 screenSize)
        {
            return new Vector2(screenSize.X / (float)m_safeGuiRectangle.Width, screenSize.Y / (float)m_safeGuiRectangle.Height);
        }

        //  This is for HUD, therefore not GUI normalized coordinates
        public static Vector2 GetHudNormalizedCoordFromPixelCoord(Vector2 pixelCoord)
        {
            return new Vector2(
                (pixelCoord.X - m_safeFullscreenRectangle.Left) / (float)m_safeFullscreenRectangle.Width,
                ((pixelCoord.Y - m_safeFullscreenRectangle.Top) / (float)m_safeFullscreenRectangle.Height) * m_hudSize.Y);
        }

        //  This is for HUD, therefore not GUI normalized coordinates
        public static Vector2 GetHudNormalizedSizeFromPixelSize(Vector2 pixelSize)
        {
            return new Vector2(
                pixelSize.X / (float)m_safeFullscreenRectangle.Width,
                (pixelSize.Y / (float)m_safeFullscreenRectangle.Height) * m_hudSize.Y);
        }

        //  This is for HUD, therefore not GUI normalized coordinates
        public static Vector2 GetHudPixelCoordFromNormalizedCoord(Vector2 normalizedCoord)
        {
            return new Vector2(
                normalizedCoord.X * (float)m_safeFullscreenRectangle.Width,
                normalizedCoord.Y * (float)m_safeFullscreenRectangle.Height);
        }

        public static Vector2 GetMinMouseCoord()
        {
            return FullscreenHudEnabled ? m_minMouseCoordFullscreenHud : m_minMouseCoord;
        }

        public static Vector2 GetMaxMouseCoord()
        {
            return FullscreenHudEnabled ? m_maxMouseCoordFullscreenHud : m_maxMouseCoord;
        }

        //  Aligns rectangle, works in screen coordinates / texture / pixel... (not normalized coordinates)
        public static Vector2 GetAlignedCoordinate(Vector2 screenCoord, Vector2 size, MyGuiDrawAlignEnum drawAlign)
        {
            Vector2 alignedScreenCoord = screenCoord;

            if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP)
            {
                //  Nothing to do as position is already at this point
            }
            else if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER)
            {
                //  Move position to the texture center
                alignedScreenCoord -= size / 2.0f;
            }
            else if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP)
            {
                alignedScreenCoord.X -= size.X / 2.0f;
            }
            else if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM)
            {
                alignedScreenCoord.X -= size.X / 2.0f;
                alignedScreenCoord.Y -= size.Y;
            }
            else if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM)
            {
                alignedScreenCoord -= size;
            }
            else if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER)
            {
                alignedScreenCoord.Y -= size.Y / 2.0f;
            }
            else if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER)
            {
                alignedScreenCoord.X -= size.X;
                alignedScreenCoord.Y -= size.Y / 2.0f;
            }
            else if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM)
            {
                alignedScreenCoord.Y -= size.Y;// *0.75f;
            }
            else if (drawAlign == MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP)
            {
                alignedScreenCoord.X -= size.X;
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            return alignedScreenCoord;
        }

        public static void GetSafeHeightFullScreenPictureSize(MyMwcVector2Int originalSize, out Rectangle outRect)
        {
            GetSafeHeightPictureSize(originalSize, m_safeFullscreenRectangle, out outRect);
        }

        public static void GetSafeAspectRatioFullScreenPictureSize(MyMwcVector2Int originalSize, out Rectangle outRect)
        {
            GetSafeAspectRatioPictureSize(originalSize, m_safeFullscreenRectangle, out outRect);
        }

        //  This method scales picture to bounding area according to bounding area's height. We don't care about width, so sometimes if picture has wide
        //  aspect ratio, borders of image may be outisde of the bounding area. This method is used when we want to have picture covering whole screen
        //  and don't care if part of picture is invisible. Also, aspect ration is unchanged too.
        static void GetSafeHeightPictureSize(MyMwcVector2Int originalSize, Rectangle boundingArea, out Rectangle outRect)
        {
            outRect.Height = boundingArea.Height;
            outRect.Width = (int)(((float)outRect.Height / (float)originalSize.Y) * originalSize.X);
            outRect.X = boundingArea.Left + (boundingArea.Width - outRect.Width) / 2;
            outRect.Y = boundingArea.Top + (boundingArea.Height - outRect.Height) / 2;
        }

        //  Return picture size that is safe for displaying in bounding area and doesn't distort the aspect ratio of original picture or bounding area.
        //  Example: picture of video frame is scaled to size of screen, so it fits it as much as possible, but still maintains aspect ration of
        //  original video frame or screen. So if screen's heigh is not enouch, video frame is scaled down to fit height, but also width is scaled
        //  according to original video frame.
        //  It's used whenever we need to scale picture/texture to some area, usually to screen size.
        //  Also this method calculated left/top coordinates, so it's always centered.
        static void GetSafeAspectRatioPictureSize(MyMwcVector2Int originalSize, Rectangle boundingArea, out Rectangle outRect)
        {
            outRect.Width = boundingArea.Width;
            outRect.Height = (int)(((float)outRect.Width / (float)originalSize.X) * originalSize.Y);

            if (outRect.Height > boundingArea.Height)
            {
                outRect.Height = boundingArea.Height;
                outRect.Width = (int)(outRect.Height * ((float)originalSize.X / (float)originalSize.Y));
            }

            outRect.X = boundingArea.Left + (boundingArea.Width - outRect.Width) / 2;
            outRect.Y = boundingArea.Top + (boundingArea.Height - outRect.Height) / 2;
        }

        public static Vector2 GetScreenTextRightTopPosition()
        {
            float deltaPixels = 25 * GetSafeScreenScale();
            return GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(GetSafeFullscreenRectangle().Width - deltaPixels, deltaPixels));
        }

        public static Vector2 GetScreenTextRightBottomPosition()
        {
            float deltaPixels = 25 * GetSafeScreenScale();
            Vector2 rightAlignedOrigin = GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(GetSafeFullscreenRectangle().Width - deltaPixels, deltaPixels));
            return GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(GetSafeFullscreenRectangle().Width - deltaPixels, GetSafeFullscreenRectangle().Height - (2 * deltaPixels)));
        }

        public static Vector2 GetScreenTextLeftBottomPosition()
        {
            float deltaPixels = 25 * GetSafeScreenScale();
            return GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(deltaPixels, GetSafeFullscreenRectangle().Height - (2 * deltaPixels)));
        }

        public static Vector2 GetScreenTextLeftTopPosition()
        {
            float deltaPixels = 25 * GetSafeScreenScale();
            return GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(deltaPixels, deltaPixels));
        }

        public static int GetScreensCount()
        {
            return m_screens.Count;
        }


        //  Add screen to top of the screens stack, so it becomes active (will have focus)
        public static void AddScreen(MyGuiScreenBase screen)
        {
            Debug.Assert(screen != null);

            // Hide tooltips
            var screenWithFocus = GetScreenWithFocus();
            if (screenWithFocus != null)
            {
                screenWithFocus.HideTooltips();
            }

            //  When adding new screen and previous screen is configured to hide(not close), find it and hide it now
            MyGuiScreenBase previousCanHideScreen = null;
            for (int i = GetScreensCount() - 1; i > 0; i--)
            {
                MyGuiScreenBase scr = m_screens[i];
                if (scr.CanHide())
                {
                    previousCanHideScreen = scr;
                    break;
                }
            }

            if (previousCanHideScreen != null && previousCanHideScreen.GetState() != MyGuiScreenState.CLOSING && !(screen is MyGuiScreenDebugBase))
            {
                previousCanHideScreen.HideScreen();
            }

            m_screensToAdd.Add(screen);
        }

        /// <summary>
        /// Adds the modal screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="modelClosed">The model closed.</param>
        public static void AddModalScreen(MyGuiScreenBase screen, MyGuiScreenBase.ScreenHandler modelClosed)
        {
            screen.Closed += modelClosed;

            AddScreen(screen);
        }

        //  Remove screen from the stack
        public static void RemoveScreen(MyGuiScreenBase screen, bool nullActualScreen = false)
        {
            Debug.Assert(screen != null);

            if (IsAnyScreenOpening() == false)
            {
                MyGuiScreenBase previousCanHideScreen = GetPreviousScreen(screen, x => x.CanHide());
                if (previousCanHideScreen != null &&
                    (previousCanHideScreen.GetState() == MyGuiScreenState.HIDDEN || previousCanHideScreen.GetState() == MyGuiScreenState.HIDING))
                {
                    previousCanHideScreen.UnhideScreen();
                }
            }

            m_screensToRemove.Add(screen);

            if (nullActualScreen)
            {
                m_currentDebugScreen = null;
            }
        }

        //  Find screen on top of screens, that has status HIDDEN or HIDING
        public static MyGuiScreenBase GetTopHiddenScreen()
        {
            MyGuiScreenBase hiddenScreen = null;
            for (int i = GetScreensCount() - 1; i > 0; i--)
            {
                MyGuiScreenBase screen = m_screens[i];
                if (screen.GetState() == MyGuiScreenState.HIDDEN || screen.GetState() == MyGuiScreenState.HIDING)
                {
                    hiddenScreen = screen;
                    break;
                }
            }
            return hiddenScreen;
        }

        //  Find previous screen to screen in screens stack
        public static MyGuiScreenBase GetPreviousScreen(MyGuiScreenBase screen, Predicate<MyGuiScreenBase> condition)
        {
            MyGuiScreenBase previousScreen = null;
            int currentScreenIndex = -1;
            for (int i = GetScreensCount() - 1; i > 0; i--)
            {
                MyGuiScreenBase tempScreen = m_screens[i];
                if (screen == tempScreen)
                {
                    currentScreenIndex = i;
                }
                if (i < currentScreenIndex)
                {
                    if (condition(tempScreen))
                    {
                        previousScreen = tempScreen;
                        break;
                    }
                }
            }
            return previousScreen;
        }

        //  Remove all screens except the one!
        public static void RemoveAllScreensExcept(MyGuiScreenBase dontRemove)
        {
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if (screen != dontRemove) RemoveScreen(screen);
            }
        }

        //  Remove screens that are of type 'screenType', or those derived from 'screenType'
        //  IMPORTANT: I am not sure how will IsAssignableFrom() behave if you use class inherited from another and then another, so make sure 
        //  you understand it before you start using it and counting on this.
        public static void RemoveScreenByType(Type screenType)
        {
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if (screenType.IsAssignableFrom(screen.GetType())) RemoveScreen(screen);
            }
        }

        //  Close all screens except the one!
        //  Difference against RemoveAllScreensExcept is that this one closes using CloseScreen, and RemoveAllScreensExcept just removes from the list
        public static void CloseAllScreensExcept(MyGuiScreenBase dontRemove)
        {
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if ((screen != dontRemove) && (screen.CanCloseInCloseAllScreenCalls() == true)) screen.CloseScreen();
            }
        }

        //  Close all screens except the one NOW!
        //  Difference against RemoveAllScreensExcept is that this one closes using CloseScreen, and RemoveAllScreensExcept just removes from the list
        public static void CloseAllScreensNowExcept(MyGuiScreenBase dontRemove)
        {
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if ((screen != dontRemove) && (screen.CanCloseInCloseAllScreenCalls() == true)) screen.CloseScreenNow();
            }
        }

        //  Close all screens except one specified and all that are marked as "topmost"
        //  Difference against RemoveAllScreensExcept is that this one closes using CloseScreen, and RemoveAllScreensExcept just removes from the list
        public static void CloseAllScreensExceptThisOneAndAllTopMost(MyGuiScreenBase dontRemove)
        {
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if ((((screen == dontRemove) || (screen.IsTopMostScreen())) == false) && (screen.CanCloseInCloseAllScreenCalls() == true)) screen.CloseScreen();
            }
        }

        public static void SwitchDebugScreensEnabled()
        {
            m_debugScreensEnabled = !m_debugScreensEnabled;
        }

        //  Sends input (keyboard/mouse) to screen which has focus (top-most)
        public static void HandleInput()
        {
            if (m_input == null)
            {
                // No input support
                return;
            }

            //  Update/gather input
            bool isGameFocused = m_input.Update();

#if RENDER_PROFILING
            MyRenderProfiler.HandleInput(m_input);
#endif

            //  Screenshot(s)
            if (m_input.IsNewKeyPress(Keys.F4))
            {
                TakeScreenshot();

                //MyVoxelMap vm = MyVoxelMaps.GetVoxelMaps()[0];
                //vm.SaveVoxelContents("StaticAsteroid_A_1000m.vox");

                /*
              MyVoxelMap voxelMap = new MyVoxelMap();
              voxelMap.Init(Vector3.Zero, new MyMwcVector3Int(384, 384, 384), MyMwcVoxelMaterialsEnum.Stone_03);
              MyEntities.Add(voxelMap);

              MyEntity asteroid = MyEntities.GetEntityById(new MyEntityIdentifier(546099));
              Matrix matrix = asteroid.WorldMatrix;
              matrix.Translation = new Vector3();

              bool changed = false;
              MyVoxelImport.Run(voxelMap, MinerWars.AppCode.Game.Models.MyModels.GetModel(Models.MyModelsEnum.StaticAsteroid1000m_A_LOD0), MyvoxelImportAction.AddVoxels, matrix, 1, null, ref changed);

              BoundingBox boundingBox = voxelMap.WorldAABB;

              //  Get min and max cell coordinate where camera bounding box can fit
              MyMwcVector3Int cellCoordMin = voxelMap.GetVoxelCoordinateFromMeters(boundingBox.Min);
              MyMwcVector3Int cellCoordMax = voxelMap.GetVoxelCoordinateFromMeters(boundingBox.Max);

              voxelMap.InvalidateCache(cellCoordMin, cellCoordMax);
              voxelMap.CalcAverageDataCellMaterials();

              voxelMap.SaveVoxelContents("VoxelMap_546099.vox");*/
            }


            // hide demo access info
            if (m_input.IsAnyAltKeyPressed() && m_input.IsKeyPress(Keys.G) && m_demoScreen != null)
            {
                MyGuiManager.RemoveScreen(m_demoScreen);
            }

            //  Statistics screen
            if (m_input.IsNewKeyPress(Keys.F11))
            {
                if (!m_input.IsAnyShiftKeyPressed() && !m_input.IsAnyCtrlKeyPressed())
                {
                    if (m_input.IsAnyAltPress())
                    {
                        // ALT + F11 Dump loaded resources.
                        //MyMinerGame.GraphicsDeviceManager.DbgDumpLoadedResources(true);
                        MyTextureManager.DbgDumpLoadedTextures(true);
                    }
                    else
                    {
                        if (m_input.IsAnyCtrlKeyPressed())
                        {
                            SwitchInputScreen();
                        }
                        else
                        {
                            SwitchStatisticsScreen();
                        }
                    }
                }
                else
                {
                    if (m_input.IsAnyShiftKeyPressed())
                    {
                        SwitchTimingScreen();
                    }
                    else if (m_input.IsAnyCtrlKeyPressed())
                    {
                        SwitchInputScreen();
                    }
                }
            }

            if (MyGuiInput.ENABLE_DEVELOPER_KEYS)
            {
                // Forge GC to run
                if (m_input.IsNewKeyPress(Keys.Pause) && m_input.IsAnyShiftKeyPressed())
                {
                    //if (m_input.IsAnyAltKeyPressed())
                    //{
                    //    for (int i = 0; i < 2048; i++)
                    //    {
                    //        byte[] data = new byte[1024]; // Create gargabe to force GC to run
                    //        data[data.Length - 1] = 123;
                    //    }
                    //}
                    GC.Collect(GC.MaxGeneration);
                }


                if (m_input.IsAnyControlPress())
                {
                    //Reload textures
                    if (m_input.IsNewKeyPress(Keys.F2) && m_input.IsKeyPress(Keys.LeftShift))
                    {
                        MyTextureManager.ReloadTextures();
                    }
                }
                else
                {
                    //Reload shaders
                    if (m_input.IsNewKeyPress(Keys.F2) && m_input.IsKeyPress(Keys.LeftShift))
                    {
                        MyMinerGame.Static.RootDirectoryEffects = MyMinerGame.Static.RootDirectoryDebug;
                        MyRender.LoadEffects();
                        //m_spriteBatchOwn = new MyGuiSpriteBatchOwn(); // Reload shader in sprite batch
                        //MyTextureManager.ReloadTextures();
                    }
                }

                //WS size
                if (m_input.IsNewKeyPress(Keys.F3) && m_input.IsKeyPress(Keys.LeftShift))
                {
                    MyUtils.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
                }

                // Forced logoff
                if (m_input.IsAnyCtrlKeyPressed() && m_input.IsNewKeyPress(Keys.Subtract))
                {
                    MySectorServiceClient.HardClose();
                    MyClientServer.Logout();
                }

                //These keys are to be used just for developers or testing
                // Only for develop, on test and public, it crashes(obfuscation)
                //if (MyMwcFinalBuildConstants.IS_DEVELOP)
                {
                    //  Game debug screen
                    if (m_input.IsNewKeyPress(Keys.F9))
                    {
                        //                if (m_currentDebugScreen != null)
                        //                  RemoveScreen(m_currentDebugScreen);
                        //            AddScreen(m_currentDebugScreen = new MyGuiScreenDebugDeveloper());
                    }

                    /*
                    if (!(m_currentStatisticsScreen is MyGuiScreenDebugStatistics))
                    {
                        if (m_currentStatisticsScreen != null)
                            RemoveScreen(m_currentStatisticsScreen);
                        AddScreen(m_currentStatisticsScreen = new MyGuiScreenDebugStatistics());
                    } */


                    if (m_input.IsNewKeyPress(Keys.F12))
                    {
                        if (m_input.IsAnyCtrlKeyPressed() && m_input.IsAnyAltKeyPressed())
                        {
                            //  Simulate device lost
                            MyMinerGame.Static.UnloadContent();
                            MyMinerGame.Static.LoadContent();
                            return;
                        }
                        else
                        {
                            //   if (!MyMwcFinalBuildConstants.IS_PUBLIC)
                            {
                                //  Develop screen
                                if (!(m_currentDebugScreen is MyGuiScreenDebugDeveloper))
                                {
                                    if (m_currentDebugScreen != null)
                                        RemoveScreen(m_currentDebugScreen);
                                    AddScreen(m_currentDebugScreen = new MyGuiScreenDebugDeveloper());
                                }
                                else
                                {
                                    RemoveScreen(m_currentDebugScreen);
                                    m_currentDebugScreen = null;
                                }
                            }
                        }
                    }

                    if (MyFakes.ALT_AS_DEBUG_KEY)
                    {
                        m_inputToNonFocusedScreens = m_input.IsAnyAltKeyPressed() && !m_input.IsKeyPress(Keys.Tab);
                    }
                    else
                    {
                        m_inputToNonFocusedScreens = m_input.IsKeyPress(Keys.ScrollLock) && !m_input.IsKeyPress(Keys.Tab);
                    }
                }
            }

            //  Forward input to screens only if there are screens and if game has focus (is active)
            if ((isGameFocused == true) && (m_screens.Count > 0))
            {
                //  Get screen from top of the stack - that one has focus
                MyGuiScreenBase screenWithFocus = GetScreenWithFocus();// m_screens[m_screens.Count - 1];

                if (m_inputToNonFocusedScreens)
                {
                    bool inputIsShared = false;

                    for (int i = (m_screens.Count - 1); i >= 0; i--)
                    {
                        MyGuiScreenBase screen = m_screens[i];
                        if (screen.CanShareInput() || m_screens.Count == 1)
                        {
                            screen.HandleInput(m_input, m_lastScreenWithFocus != screenWithFocus);
                            inputIsShared = true;
                        }
                    }

                    m_inputToNonFocusedScreens &= inputIsShared;

                    UpdateMouseCursor();
                }
                else
                    if (screenWithFocus != null)
                    {
                        if (screenWithFocus.GetDrawMouseCursor() == true)
                        {
                            UpdateMouseCursor();
                        }

                        if (screenWithFocus.CanHandleInputDuringTransition() && IsScreenTransitioning(screenWithFocus))
                        {
                            screenWithFocus.HandleInput(m_input, m_lastScreenWithFocus != screenWithFocus);
                        }
                        else if ((screenWithFocus.GetState() == MyGuiScreenState.OPENED))
                        {
                            //  Disallow to enter input during transition phase of screens
                            bool handleInput = true;
                            if (IsAnyScreenInTransition())
                            {
                                handleInput = false;
                            }

                            if (handleInput)
                            {
                                //  Send input to screen which has focus - and is fully opened, thus it's not in any sort of transition
                                screenWithFocus.HandleInput(m_input, m_lastScreenWithFocus != screenWithFocus);
                            }
                        }
                    }

                m_lastScreenWithFocus = screenWithFocus;
            }
        }

        private static void SwitchFriendsScreen()
        {
            if (!(m_currentStatisticsScreen is MyGuiScreenDebugPlayerFriends))
            {
                if (m_currentStatisticsScreen != null)
                    RemoveScreen(m_currentStatisticsScreen);
                AddScreen(m_currentStatisticsScreen = new MyGuiScreenDebugPlayerFriends());
            }
            else
            {
                RemoveScreen(m_currentStatisticsScreen);
                m_currentStatisticsScreen = null;
            }
        }

        private static void SwitchTimingScreen()
        {
            if (!(m_currentStatisticsScreen is MyGuiScreenDebugTiming))
            {
                if (m_currentStatisticsScreen != null)
                    RemoveScreen(m_currentStatisticsScreen);
                AddScreen(m_currentStatisticsScreen = new MyGuiScreenDebugTiming());
            }
            else
            {
                RemoveScreen(m_currentStatisticsScreen);
                m_currentStatisticsScreen = null;
            }
        }

        private static void SwitchStatisticsScreen()
        {
            if (!(m_currentStatisticsScreen is MyGuiScreenDebugStatistics))
            {
                if (m_currentStatisticsScreen != null)
                    RemoveScreen(m_currentStatisticsScreen);
                AddScreen(m_currentStatisticsScreen = new MyGuiScreenDebugStatistics());
            }
            else
            {
                RemoveScreen(m_currentStatisticsScreen);
                m_currentStatisticsScreen = null;
            }
        }

        private static void SwitchInputScreen()
        {
            if (!(m_currentStatisticsScreen is MyGuiScreenDebugInput))
            {
                if (m_currentStatisticsScreen != null)
                    RemoveScreen(m_currentStatisticsScreen);
                AddScreen(m_currentStatisticsScreen = new MyGuiScreenDebugInput());
            }
            else
            {
                RemoveScreen(m_currentStatisticsScreen);
                m_currentStatisticsScreen = null;
            }
        }

        private static void OnLoggedUserChanged(object sender, EventArgs args)
        {
            UpdateDemoAccessScreen();
        }

        public static void UpdateDemoAccessScreen()
        {
            if (MyClientServer.LoggedPlayer != null && MyClientServer.LoggedPlayer.IsDemoUser())
            {
                if (m_demoScreen == null)
                {
                    m_demoScreen = new MyGuiScreenDemoAccessInfo();
                    m_screensToAdd.Add(m_demoScreen);
                }
            }
            else
            {
                if (m_demoScreen != null)
                {
                    m_screensToRemove.Add(m_demoScreen);
                    m_demoScreen = null;
                }
            }
        }

        static bool IsAnyScreenInTransition()
        {
            bool isTransitioning = false;
            if (m_screens.Count > 0)
            {
                //  Get screen from top of the stack - that one has focus
                //  But it can't be closed. If yes, then look for other.
                for (int i = (m_screens.Count - 1); i >= 0; i--)
                {
                    MyGuiScreenBase screen = m_screens[i];

                    isTransitioning = IsScreenTransitioning(screen);
                    if (isTransitioning) break;
                }
            }
            return isTransitioning;
        }

        public static bool IsAnyScreenOpening()
        {
            bool isOpening = false;
            if (m_screens.Count > 0)
            {
                //  Get screen from top of the stack - that one has focus
                //  But it can't be closed. If yes, then look for other.
                for (int i = (m_screens.Count - 1); i >= 0; i--)
                {
                    MyGuiScreenBase screen = m_screens[i];

                    isOpening = screen.GetState() == MyGuiScreenState.OPENING;
                    if (isOpening) break;
                }
            }
            return isOpening;
        }

        private static bool IsScreenTransitioning(MyGuiScreenBase screen)
        {
            return (screen.GetState() == MyGuiScreenState.CLOSING || screen.GetState() == MyGuiScreenState.OPENING) ||
                (screen.GetState() == MyGuiScreenState.HIDING || screen.GetState() == MyGuiScreenState.UNHIDING);
        }

        public static bool IsScreenOfTypeOpen(Type screenType)
        {
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if ((screen.GetType() == screenType) && (screen.GetState() == MyGuiScreenState.OPENED)) return true;
            }
            return false;
        }

        public static MyGuiScreenDebugStatistics GetScreenDebugNormal()
        {
            MyGuiScreenDebugStatistics debugScreen = null;
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if (screen is MyGuiScreenDebugStatistics) debugScreen = (MyGuiScreenDebugStatistics)screen;
            }
            return debugScreen;
        }

        public static MyGuiScreenDebugBot GetScreenDebugBot()
        {
            MyGuiScreenDebugBot debugScreenBot = null;
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if (screen is MyGuiScreenDebugBot) debugScreenBot = (MyGuiScreenDebugBot)screen;
            }
            return debugScreenBot;
        }

        public static MyGuiScreenDebugDeveloper GetScreenDebugDeveloper()
        {
            MyGuiScreenDebugDeveloper debugScreenDeveloper = null;
            foreach (MyGuiScreenBase screen in m_screens)
            {
                if (screen is MyGuiScreenDebugDeveloper) debugScreenDeveloper = (MyGuiScreenDebugDeveloper)screen;
            }
            return debugScreenDeveloper;
        }

        //  Update all screens
        public static void Update()
        {
            //  We remove, add, remove because sometimes in ADD when calling LoadContent some screen can be marked for remove, so we
            //  need to make sure it's really removed before we enter UPDATE or DRAW loop
            MyRender.GetRenderProfiler().StartProfilingBlock("GuiManager-RemoveScreens");
            RemoveScreens();
            MyRender.GetRenderProfiler().EndProfilingBlock();

            MyRender.GetRenderProfiler().StartProfilingBlock("GuiManager-AddScreens");
            AddScreens();
            MyRender.GetRenderProfiler().EndProfilingBlock();

            MyRender.GetRenderProfiler().StartProfilingBlock("GuiManager-RemoveScreens2");
            RemoveScreens();
            MyRender.GetRenderProfiler().EndProfilingBlock();

            MyGuiScreenBase screenWithFocus = GetScreenWithFocus();

            try
            {
                System.Windows.Forms.Form f = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(MyMinerGame.Static.Window.NativeWindow.Handle);
                UpdateClipForCursor(f);
            }
            catch
            {
                return;
            }
            EnableSoundsBasedOnWindowFocus();

            //  Update screens
            int updateScreensBlock = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("GuiManager-Update screens", ref updateScreensBlock);

            for (int i = 0; i < m_screens.Count; i++)
            {
                MyGuiScreenBase screen = m_screens[i];

                if (MyMwcFinalBuildConstants.EnableLoggingInDrawAndUpdateAndGuiLoops)
                {
                    MyMwcLog.WriteLine("Update screen: " + i.ToString() + " - " + screen.ToString());
                }

                int blockId = -1;
                MyRender.GetRenderProfiler().StartProfilingBlock("Update : " + screen.GetFriendlyName(), ref blockId);
                screen.Update(screen == screenWithFocus);
                MyRender.GetRenderProfiler().EndProfilingBlock(blockId);
            }

            MyRender.GetRenderProfiler().EndProfilingBlock(updateScreensBlock);
        }

        static int GetIndexOfLastNonTopMostNonDebugScreen()
        {
            int max = 0;
            for (int i = 0; i < m_screens.Count; i++)
            {
                MyGuiScreenBase screen = m_screens[i];
                if (screen.IsTopMostScreen() == true || screen is MyGuiScreenDebugBase)
                {
                    break;
                }
                max = i + 1;
            }
            return max;
        }

        //  Add screens - if during update-loop some screen was marked 'for add'
        static void AddScreens()
        {
            // Changed from foreach to for, to allow add screens during enumeration
            for (int i = 0; i < m_screensToAdd.Count; i++)
            {
                MyGuiScreenBase screenToAdd = m_screensToAdd[i];

                if (screenToAdd.IsLoaded == false)
                {
                    screenToAdd.SetState(MyGuiScreenState.OPENING);
                    screenToAdd.LoadContent();
                }

                m_screens.Insert(GetIndexOfLastNonTopMostNonDebugScreen(), screenToAdd);
            }
            m_screensToAdd.Clear();
        }

        public static bool IsScreenTopMostNonDebug(MyGuiScreenBase screen)
        {
            int index = GetIndexOfLastNonTopMostNonDebugScreen() - 1;
            if (index < 0 || index >= m_screens.Count) return false;
            if (m_screensToAdd.Count > 0) return false;

            return m_screens[index] == screen;
        }

        //  Remove screens - if during update-loop some screen was marked 'for remove'
        static void RemoveScreens()
        {
            foreach (MyGuiScreenBase screenToRemove in m_screensToRemove)
            {
                if (screenToRemove.IsLoaded == true)
                {
                    screenToRemove.UnloadContent();
                    screenToRemove.UnloadData();
                }
                m_screens.Remove(screenToRemove);

                // if we remove screen which is marked as screen to add, then we must remove it from screens to add
                int screenIndex = m_screensToAdd.Count - 1;
                while (screenIndex >= 0)
                {
                    if (m_screensToAdd[screenIndex] == screenToRemove)
                    {
                        m_screensToAdd.RemoveAt(screenIndex);
                    }
                    screenIndex--;
                }
            }
            m_screensToRemove.Clear();
        }

        static void UpdateMouseCursor()
        {
            //here remove only hw cursor fakes rest is ok
            if (MyVideoModeManager.IsHardwareCursorUsed())
            {
                Vector2 scp = MyGuiInput.GetMousePosition();
                MouseCursorPosition = GetNormalizedCoordinateFromScreenCoordinate(scp);
            }
            else
            {
                //this else has to be called w/o fakes either
                Vector2 screenDelta = m_input.GetCursorPosition() - MyMinerGame.ScreenSizeHalf;

                MouseCursorPosition += GetNormalizedSizeFromScreenSize(screenDelta * MyGuiConstants.MOUSE_CURSOR_SPEED_MULTIPLIER * m_safeScreenScale);

                //  Min and max mouse position
                var min = GetMinMouseCoord();
                var max = GetMaxMouseCoord();
                if (MouseCursorPosition.X < min.X) m_mouseCursorPosition.X = min.X;
                if (MouseCursorPosition.X > max.X) m_mouseCursorPosition.X = max.X;
                if (MouseCursorPosition.Y < min.Y) m_mouseCursorPosition.Y = min.Y;
                if (MouseCursorPosition.Y > max.Y) m_mouseCursorPosition.Y = max.Y;
            }
        }

        static void DrawMouseCursor(MyTexture2D mouseCursorTexture)
        {
            if (MyGuiManager.GetScreenshot() != null)
                return;

            if (mouseCursorTexture == null)
                return;
            Vector2 cursorSize = GetNormalizedSize(mouseCursorTexture, MyGuiConstants.MOUSE_CURSOR_SCALE);

            DrawSpriteBatch(mouseCursorTexture, MouseCursorPosition + (cursorSize / 2.0f), MyGuiConstants.MOUSE_CURSOR_SCALE, new Color(MyGuiConstants.MOUSE_CURSOR_COLOR),
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, 0, new Vector2(0.5f, 0.5f));
        }

        public static MyGuiScreenBase GetScreenWithFocus()
        {
            MyGuiScreenBase screenWithFocus = null;

            if (m_screens.Count > 0)
            {
                //  Get screen from top of the stack - that one has focus
                //  But it can't be closed. If yes, then look for other.
                for (int i = (m_screens.Count - 1); i >= 0; i--)
                {
                    MyGuiScreenBase screen = m_screens[i];

                    //  Only opened screens and only if it isn't a debug screen
                    bool isOpened = (screen.GetState() == MyGuiScreenState.OPENED) || (screen.CanHandleInputDuringTransition() && IsScreenTransitioning(screen));
                    bool canHaveFocus = screen.CanHaveFocus();
                    if (isOpened && canHaveFocus)
                    {
                        screenWithFocus = screen;
                        break;
                    }
                }
            }

            return screenWithFocus;
        }

        //  Draw all screens
        public static void Draw()
        {
            int profID = -1;
            MyRender.GetRenderProfiler().StartProfilingBlock("GuiManager::Draw", ref profID);
            /*
           //  Clear color, depth and stencil because at this moment I am not sure which one will be really required. But
           //  I think that stencil needs to be cleared because we use it on stencil masks for textbox, combobox, etc
           try
           {
               MyMinerGame.Static.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
           }
           catch (Exception e)
           {
               MyMwcLog.WriteLine("Unknown clear error: " + e.ToString());
           }

              */
            // PetrM told to comment out
            //System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount == 0);

            BeginSpriteBatch();

            //  Find screen with focus
            MyGuiScreenBase screenWithFocus = GetScreenWithFocus();

            //  Find top screen that has background fade
            MyGuiScreenBase screenFade = null;
            for (int i = (m_screens.Count - 1); i >= 0; i--)
            {
                MyGuiScreenBase screen = m_screens[i];
                bool screenGetEnableBackgroundFade = screen.GetEnableBackgroundFade();
                bool isScreenFade = false;
                if (screenWithFocus == screen || screen.GetDrawScreenEvenWithoutFocus())
                {
                    if ((screen.GetState() != MyGuiScreenState.CLOSED) && (screenGetEnableBackgroundFade))
                    {
                        isScreenFade = true;
                    }
                }
                else if (IsScreenTransitioning(screen) && screenGetEnableBackgroundFade)
                {
                    isScreenFade = true;
                }

                if (isScreenFade)
                {
                    screenFade = screen;
                    break;
                }
            }


            if (MyGuiManager.GetScreenshot() != null)
            {
                Render.MyRender.CreateRenderTargets();
                m_renderSetup.ShadowRenderer = MyRender.GetShadowRenderer();
                //We need depth n stencil because stencil is used for drawing hud
                var rt = new Texture(MyMinerGame.Static.GraphicsDevice, (int)(MyCamera.ForwardViewport.Width), (int)(MyCamera.ForwardViewport.Height), 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                m_renderSetup.RenderTargets = new Texture[] { rt };
                // To have render target with large size on device (screen.Draw calls MyCamera.EnableForward, which sets Viewport on device)
                MyMinerGame.SetRenderTargets(m_renderSetup.RenderTargets, null);
                Render.MyRender.PushRenderSetupAndApply(m_renderSetup, ref m_backupRenderSetup);
            }

            //  Draw all screen, from bottom to top
            for (int i = 0; i < m_screens.Count; i++)
            {
                MyGuiScreenBase screen = m_screens[i];

                bool drawScreen = false;
                if (screenWithFocus == screen || screen.GetDrawScreenEvenWithoutFocus())
                {
                    if (MyMwcFinalBuildConstants.EnableLoggingInDrawAndUpdateAndGuiLoops)
                    {
                        MyMwcLog.WriteLine("Draw screen: " + i.ToString() + " - " + screen.ToString());
                    }

                    if (screen.GetState() != MyGuiScreenState.CLOSED && screen.GetState() != MyGuiScreenState.HIDDEN)
                    {
                        drawScreen = true;
                    }
                }
                else if (IsScreenTransitioning(screen))
                {
                    drawScreen = true;
                }

                if (drawScreen)
                {
                    float backgroundFadeAlpha = (screen == screenFade) ? 1 : 0;
                    screen.Draw(backgroundFadeAlpha);
                }
            }

            if (m_inputToNonFocusedScreens)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_inputSharingText, new Vector2(0, 0), 0.6f,
                Color.Red, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

            if (Render.MyRender.IsRenderOverloaded)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), m_renderOverloadedText, new Vector2(0, 0), 0.6f,
                Color.Red, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

            /*
            if (MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip.RenderObject.CastShadow)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), new StringBuilder("SUN!!"), new Vector2(0, 0), 0.6f,
                Color.Red, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            } */


            if (((screenWithFocus != null) && (screenWithFocus.GetDrawMouseCursor() == true)) || (m_inputToNonFocusedScreens && m_screens.Count > 1))
            {
                MyGuiControlBase mouseOverControl = null;
                if (screenWithFocus != null)
                {
                    mouseOverControl = screenWithFocus.GetMouseOverControl();
                }


                if (mouseOverControl != null && mouseOverControl.GetMouseCursorTexture() != null)
                {
                    if (MyVideoModeManager.IsHardwareCursorUsed())
                    {
                        SetMouseCursorVisibility(true);
                    }
                    else
                    {
                        SetMouseCursorVisibility(false);
                        DrawMouseCursor(mouseOverControl.GetMouseCursorTexture());
                    }
                }
                else
                {
                    if (MyVideoModeManager.IsHardwareCursorUsed())
                    {
                        SetMouseCursorVisibility(true);
                    }
                    else
                    {
                        SetMouseCursorVisibility(false);
                        DrawMouseCursor(GetMouseCursorTexture());
                    }
                }
            }
            else
            {
                if (MyVideoModeManager.IsHardwareCursorUsed())
                {
                    if (MyGuiManager.GetScreenWithFocus() != null)
                    {
                        SetMouseCursorVisibility(MyGuiManager.GetScreenWithFocus().GetDrawMouseCursor());
                    }
                }
            }
            /* //We dont want cursor in screenshots
            if (MyGuiManager.GetScreenshot() != null)
            {
                DrawMouseCursor(GetMouseCursorTexture());
            }*/
            if (MyVideoModeManager.IsThereAnyChangeToApply)
            {
                MyTexture2D texture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ProgressBackground", flags: TextureFlags.IgnoreQuality);
                Vector2 size = new Vector2(598 / 1600f, 368 / 1200f);
                DrawSpriteBatch(texture, new Vector2(0.5f, 0.5f), size, Color.White, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                DrawString(GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.ApplyingPleaseWait), new Vector2(0.5f, 0.5f), 0.9f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }

            EndSpriteBatch();

            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount == 0);

            if (MyGuiManager.GetScreenshot() != null)
            {
                MyRender.PopRenderSetupAndRevert(m_backupRenderSetup);
                MyRender.TakeScreenshot("FinalScreen", MyGuiManager.GetScreenshotTexture(), Effects.MyEffectScreenshot.ScreenshotTechniqueEnum.Color);

                MyMinerGame.SetRenderTarget(null, null);
                MyRender.Blit(MyGuiManager.GetScreenshotTexture(), true, MyEffectScreenshot.ScreenshotTechniqueEnum.Color);

                MyGuiManager.GetScreenshotTexture().Dispose();
            }

            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount == 0);

            //  Screenshot object survives only one DRAW after created. We delete it immediatelly. So if 'm_screenshot'
            //  is not null we know we have to take screenshot and set it to null.
            //  Take last picture and destroy screenshot object
            if (m_screenshot != null)
            {
                StopTakingScreenshot();
            }


            MyRender.GetRenderProfiler().ProfileCustomValue("Drawcalls", MyPerformanceCounter.PerCameraDraw.TotalDrawCalls);

            MyRender.GetRenderProfiler().EndProfilingBlock(profID);

            if (MyRender.MainRendering)
            {
                MyRender.GetRenderProfiler().Draw();
            }

            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount == 0);
        }

        public static bool IsMainMenuScreenOpened()
        {
            return m_screens.OfType<MyGuiScreenMainMenu>().Any();
        }

        public static MyGuiScreenMainMenu GetMainMenuScreen()
        {
            return m_screens.OfType<MyGuiScreenMainMenu>().FirstOrDefault();
        }

        public static bool IsLoginScreenOpened()
        {
            return m_screens.OfType<MyGuiScreenLogin>().Any();
        }


        static StringBuilder m_sb = new StringBuilder(512);

        //  Only for displaying list of active GUI screens in debug console
        public static StringBuilder GetGuiScreensForDebug()
        {
            m_sb.Clear();
            m_sb.ConcatFormat("{0}{1}{2}", "GUI screens: [", m_screens.Count, "]: ");
            for (int i = 0; i < m_screens.Count; i++)
            {
                MyGuiScreenBase screen = m_screens[i];
                m_sb.Append(screen.GetFriendlyName());
                //m_sb.Replace("MyGuiScreen", ""); //This is doing allocations
                m_sb.Append(i < (m_screens.Count - 1) ? ", " : "");
                //                string[] stateString = { "o", "O", "c", "C", "h", "u", "H" };  // debug: show opening/closing state of screens
                //                sb.Append(screen.GetFriendlyName().Replace("MyGuiScreen", "") + "(" + stateString[(int)(screen.GetState())] + ")" + (i < (m_screens.Count - 1) ? ", " : ""));
            }
            return m_sb;
        }

        private static void AddIntroScreen()
        {
            if (MyFakes.ENABLE_MENU_VIDEO_BACKGROUND)
            {
#if !GPU_PROFILING
                AddScreen(MyGuiScreenIntroVideo.CreateBackgroundScreen());
#endif
            }
        }

        public static void BackToIntroLogos(Action afterLogosAction)
        {
            MyGuiManager.CloseAllScreensNowExcept(null);

            string[] logos = new string[]
            {
                "Textures\\Logo\\keen_swh",
                "Textures\\Logo\\miner_wars_2081",
                "Textures\\Logo\\vrage",
            };

            MyGuiScreenBase previousScreen = null;

            foreach (var logo in logos)
            {
                var logoScreen = new MyGuiScreenLogo(logo);
                if (previousScreen != null)
                    AddCloseHandler(previousScreen, logoScreen, afterLogosAction);
                else
                    MyGuiManager.AddScreen(logoScreen);

                previousScreen = logoScreen;
            }

            if (previousScreen != null)
                previousScreen.Closed += (screen) => afterLogosAction();
            else
                afterLogosAction();
        }

        private static void AddCloseHandler(MyGuiScreenBase previousScreen, MyGuiScreenLogo logoScreen, Action afterLogosAction)
        {
            previousScreen.Closed += (screen) =>
            {
                if (!screen.Canceled)
                    MyGuiManager.AddScreen(logoScreen);
                else
                    afterLogosAction();
            };
        }

        public static void BackToMainMenu()
        {
            if (MyMultiplayerGameplay.IsRunning)
            {
                MyMultiplayerGameplay.Static.Shutdown();
            }

            if (MyMultiplayerPeers.Static != null && MyMultiplayerPeers.Static.IsStarted)
            {
                MyMultiplayerPeers.Static.Shutdown();
            }

            if (MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.GetState() != MyGuiScreenState.CLOSING && MyGuiScreenGamePlay.Static.GetState() != MyGuiScreenState.CLOSED)
            {
                AddIntroScreen();
                MyGuiManager.AddScreen(new MyGuiScreenUnloading(MyGuiScreenGamePlay.Static, new MyGuiScreenMainMenu(true)));
            }
            else
            {
                AddIntroScreen();
                MyGuiScreenMainMenu.AddMainMenu(true);
            }
            UpdateDemoAccessScreen();
            //AddScreen(new MyGuiScreenLoading(new MyGuiScreenGamePlay(MyGuiScreenGamePlayType.MAIN_MENU, null,
            //MyTrailerConstants.DEFAULT_SECTOR_IDENTIFIER, null), MyGuiScreenGamePlay.Static));
        }

        public static MyFullScreenQuad GetFullscreenQuad()
        {
            return m_fullscreenQuad;
        }

        public static MinerWarsMath.Vector3 GetMouseCursorDirection()
        {
            Vector2 screenMouseCursorCoord = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(MouseCursorPosition);
            return MyUtils.Unproject2dCoordinateTo3dCoordinate(screenMouseCursorCoord);
        }

        public static Vector2 GetHudSize()
        {
            return m_hudSize;
        }

        public static Vector2 GetHudSizeHalf()
        {
            return m_hudSizeHalf;
        }

        public static Vector2? GetScreenWithFocusSize()
        {
            MyGuiScreenBase screen = GetScreenWithFocus();
            return screen != null ? screen.GetSize() : null;
        }


        public static MyGuiFont GetFontMinerWarsWhite()
        {
            return m_fontMinerWarsWhite;
        }


        public static MyGuiFont GetFontMinerWarsRed()
        {
            return m_fontMinerWarsRed;
        }

        public static MyGuiFont GetFontMinerWarsBlue()
        {
            return m_fontMinerWarsBlue;
        }

        public static MyGuiFont GetFontMinerWarsGreen()
        {
            return m_fontMinerWarsGreen;
        }


        public static MyTexture2D GetBlankTexture()
        {
            return m_blankTexture;
        }

        public static MyTexture2D GetButtonTexture()
        {
            return m_buttonTexture;
        }


        public static MyTexture2D GetButtonTextureBg(MyTexture2D mainTexture)
        {
            if (mainTexture == m_buttonTexture) return m_buttonTextureBg;

            if (mainTexture == m_hubButton) return m_hubButtonBg;

            if (mainTexture == m_inventoryButton) return m_inventoryButtonBg;

            if (mainTexture == m_MessageButton) return m_hubButtonBg;

            if (mainTexture == m_inventoryButtonTakeAll) return m_inventoryButtonTakeAllBg;

            return null;
        }

        public static MyTexture2D GetTravelButtonTexture()
        {
            return m_travelButtonTexture;
        }

        public static MyTexture2D GetButtonSearchTexture()
        {
            return m_searchButtonTexture;
        }


        public static MyTexture2D GetSliderTexture()
        {
            return m_sliderTexture;
        }

        public static MyTexture2D GetSmallButtonTexture()
        {
            return m_smallButtonTexture;
        }


        public static MyTexture2D GetLoadingTexture()
        {
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\Loading", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetCheckboxOnTexture()
        {
            return m_checkBoxOnTexture;
        }
        public static MyTexture2D GetCheckboxOffTexture()
        {
            return m_checkBoxOffTexture;
        }

        public static MyTexture2D GetInterlanceOverlapTexture()
        {
            return m_textureInterlanced;
        }

        public static MyTexture2D GetTextboxTexture(MyGuiControlPreDefinedSize predefinedSize)
        {
            /*
            if (predefinedSize == MyGuiControlPreDefinedSize.SMALL)
            {
                return m_textboxSmallTexture;
            }
            else if (predefinedSize == MyGuiControlPreDefinedSize.MEDIUM)
            {
                return m_textboxMediumTexture;
            }
            else
            {
                return m_textboxLargeTexture;
            }
             * */
            return m_textboxMediumTexture;
        }

        public static MyTexture2D GetComboboxTexture(MyGuiControlPreDefinedSize predefinedSize)
        {
            switch (predefinedSize)
            {
                case MyGuiControlPreDefinedSize.MEDIUM:
                    return m_comboboxMediumTexture;
                case MyGuiControlPreDefinedSize.LONGMEDIUM:
                    return m_comboboxMediumTexture;
                default:
                    throw new IndexOutOfRangeException("this combobox texture does not exist");
            }
        }

        public static MyTexture2D GetComboboxTextureTop(MyGuiControlPreDefinedSize predefinedSize)
        {
            switch (predefinedSize)
            {
                case MyGuiControlPreDefinedSize.MEDIUM:
                    return m_comboboxMediumTopTexture;
                case MyGuiControlPreDefinedSize.LONGMEDIUM:
                    return m_comboboxMediumTopTexture;
                default:
                    throw new IndexOutOfRangeException("this combobox texture does not exist");
            }
        }

        public static MyTexture2D GetComboboxTextureItem(MyGuiControlPreDefinedSize predefinedSize)
        {
            switch (predefinedSize)
            {
                case MyGuiControlPreDefinedSize.MEDIUM:
                    return m_comboboxMediumItemTexture;
                case MyGuiControlPreDefinedSize.LONGMEDIUM:
                    return m_comboboxMediumItemTexture;
                default:
                    throw new IndexOutOfRangeException("this combobox texture does not exist");
            }
        }

        public static MyTexture2D GetComboboxTextureBottom(MyGuiControlPreDefinedSize predefinedSize)
        {
            switch (predefinedSize)
            {
                case MyGuiControlPreDefinedSize.MEDIUM:
                    return m_comboboxMediumBottomTexture;
                case MyGuiControlPreDefinedSize.LONGMEDIUM:
                    return m_comboboxMediumBottomTexture;
                default:
                    throw new IndexOutOfRangeException("this combobox texture does not exist");
            }
        }

        public static MyTexture2D GetComboboxSelectedTexture()
        {
            return m_comboboxSelectedTexture;
        }


        public static MyTexture2D GetAmmoToolTip()
        {
            return m_ammoToolTipTexture;
        }


        public static MyTexture2D GetAmmoSelectLowBackground()
        {
            return m_ammoSelectLowBackgroundTexture;
        }

        public static MyTexture2D GetScrollbarSlider()
        {
            return m_scrollbarSlider;
        }

        public static MyTexture2D GetHorizontalScrollbarSlider()
        {
            return m_horizontalScrollbarSlider;
        }

        public static MyTexture2D GetMouseCursorTexture()
        {
            return m_mouseCursorTexture;
        }

        public static void SetMouseCursorTexture(MyTexture2D texture)
        {
            m_mouseCursorTexture = texture;
        }

        public static System.Drawing.Bitmap GetMouseCursorBitmap()
        {
            return m_mouseCursorBitmap;
        }

        public static MyTexture2D GetMouseCursorArrowTexture()
        {
            return m_mouseCursorArrowTexture;
        }

        public static MyTexture2D GetMouseCursorHandTexture()
        {
            return m_mouseCursorHandTexture;
        }

        public static System.Drawing.Bitmap GetMouseCursorHandBitmap()
        {
            return null;//todo
        }

        public static MyTexture2D GetLockedButtonTexture()
        {
            return m_lockedButtonTexture;
        }

        public static MyTexture2D GetLockedInventoryItem()
        {
            return m_lockedInventoryItemTexture;
        }

        public static MyTexture2D GetSliderControlTexture()
        {
            return m_sliderControlTexture;
        }

        public static MyTexture2D GetProgressBarTexture()
        {
            return m_progressBarTexture;
        }

        public static MyTexture2D GetConfigWheelBackground()
        {
            //return m_configWheelBackground;
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\ConfigWheelBackground", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetConfigWheelHover()
        {
            //return m_configWheelHover;
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ConfigWheelHover", flags: TextureFlags.IgnoreQuality);
        }

        //public static MyTexture2D GetConfigWheelBackgroundSmall()
        //{
        //    //return m_configWheelBackgroundSmall;
        //    //return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\ConfigWheelSmallBackground", flags: TextureFlags.IgnoreQuality);
        //}


        public static MyTexture2D GetRandomTexture()
        {
            return m_randomTexture;
        }

        public static MyTexture2D GetFogTexture()
        {
            return m_fogTexture;
        }

        public static MyTexture2D GetFogSmallTexture()
        {
            return m_fogSmallTexture;
        }

        public static MyTexture2D GetFogSquareTexture()
        {
            return m_fogSquareTexture;
        }

        public static MySpriteListVideoPlayer GetDialoguePortraitNoiseVideoPlayer()
        {
            return m_dialoguePortraitNoiseVideoPlayer;
        }

        public static MyTexture2D GetAmmoSelectKeyConfirmBorderTexture()
        {
            return m_ammoSelectKeyConfirmBorderTexture;
        }


        public static MyTexture2D GetExpandTexture()
        {
            return m_expandTexture;
        }

        public static MyTexture2D GetCollapseTexture()
        {
            return m_collapseTexture;
        }


        public static MyTexture2D GetToolbarButton()
        {
            return m_toolbarButton;
        }
        public static MyTexture2D GetToolbarButtonHover()
        {
            return m_toolbarButtonHover;
        }

        public static MyTexture2D GetJournalFilterAllTexture()
        {
            return m_journalFilterAll;
        }

        public static MyTexture2D GetJournalFilterGlobalEventsTexture()
        {
            return m_journalFilterGlobalEvents;
        }



        public static MyTexture2D GetJournalFilterMissionsTexture()
        {
            return m_journalFilterMissions;
        }



        public static MyTexture2D GetJournalFilterStorytexture()
        {
            return m_journalFilterStory;
        }

        public static MyTexture2D GetJournalLine()
        {
            return m_journalLine;
        }

        public static MyTexture2D GetJournalSelected()
        {
            return m_journalSelected;
        }


        public static MyTexture2D GetJournalCloseTexture()
        {
            return m_journalCloseButton;
        }

        public static MyTexture2D GetInventoryScreenBackgroundTexture()
        {
            //return m_inventoryScreenBackgroundTexture;
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\InventoryScreenBackground", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetInventoryScreenButtonTexture()
        {
            return m_inventoryButton;
            //return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\Button", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetInventoryFilterSortAllOff()
        {
            return m_inventoryFilterSortAllOff;
        }

        public static MyTexture2D GetInventoryFilterSortAllOn()
        {
            return m_inventoryFilterSortAllOn;
        }

        public static MyTexture2D GetInventoryFilterSortConsumablesOff()
        {
            return m_inventoryFilterSortConsumablesOff;
        }

        public static MyTexture2D GetInventoryFilterSortConsumablesOn()
        {
            return m_inventoryFilterSortConsumablesOn;
        }

        public static MyTexture2D GetInventoryFilterSortEquipmentOff()
        {
            return m_inventoryFilterSortEquipmentOff;
        }

        public static MyTexture2D GetInventoryFilterSortEquipmentOn()
        {
            return m_inventoryFilterSortEquipmentOn;
        }

        public static MyTexture2D GetInventoryFilterSortGodsOff()
        {
            return m_inventoryFilterSortGodsOff;
        }

        public static MyTexture2D GetInventoryFilterSortGodsOn()
        {
            return m_inventoryFilterSortGodsOn;
        }

        public static MyTexture2D GetInventoryFilterSortOresOff()
        {
            return m_inventoryFilterSortOresOff;
        }

        public static MyTexture2D GetInventoryFilterSortOresOn()
        {
            return m_inventoryFilterSortOresOn;
        }

        public static MyTexture2D GetInventoryFilterSortWeaponsOff()
        {
            return m_inventoryFilterSortWeaponsOff;
        }

        public static MyTexture2D GetInventoryFilterSortWeaponsOn()
        {
            return m_inventoryFilterSortWeaponsOn;
        }

        public static MyTexture2D GetInventoryPreviousShip()
        {
            return m_inventoryPreviousShip;
        }

        public static MyTexture2D GetInventoryNextShip()
        {
            return m_inventoryNextShip;
        }

        public static MyTexture2D GetHubBackground()
        {
            //return m_hubBackground;
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\HubBackground", flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
        }

        public static MyTexture2D GetHubItemBackground()
        {
            return m_hubItemBackground;
            //return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\HubItemBackground", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetConfirmButton()
        {
            return m_hubButton;
        }


        public static MyTexture2D GetMessageBoxButton()
        {
            return m_MessageButton;
        }

        public static MyTexture2D GetInventoryScreenButtonTextureTakeAll()
        {
            return m_inventoryButtonTakeAll;
            //return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\ButtonTakeAll", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetInventoryScreenListboxScrollBarTexture()
        {
            //return m_inventoryListboxScrollBar;
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\InventoryScreen\\ListboxScrollBar", flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
        }

        public static MyTexture2D GetDialogueFriendBackgroundTexture()
        {
            return m_dialogueFriendBackgroundTexture;
            //return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\DialogueFriend", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetDialogueEnemyBackgroundTexture()
        {
            return m_dialogueEnemyBackgroundTexture;
            //return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\DialogueEnemy", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetDialogueNeutralBackgroundTexture()
        {
            return m_dialogueNeutralBackgroundTexture;
            //return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\DialogueNeutral", flags: TextureFlags.IgnoreQuality);
        }

        public static MyTexture2D GetSelectEditorBackground()
        {
            //return m_SelectEditorbackground;
            // Changed to same texture as sandbox, because it has same size
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\SandBoxbackground", flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
        }

        public static MyTexture2D GetSandboxBackgoround()
        {
            //return m_SandboxBackground;
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\SandBoxbackground", flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
        }

        public static MyTexture2D GetMultiplayerBackground()
        {
            //return m_SandboxBackground;
            return MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\MultiplayerBackground", flags: TextureFlags.IgnoreQuality, loadingMode: LoadingMode.LazyBackground);
        }

        /// <summary>
        /// Gets the prefab preview (thumbnail) texture.
        /// </summary>
        /// <param name="enumValue">Specifies the prefab type.</param>
        public static MyTexture2D GetPrefabPreviewTexture(MyMwcObjectBuilderTypeEnum prefabType, int prefabId, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance)
        {
            MyTexture2D texture = GetPrefabPreview(prefabType, prefabId, appearance);
            if (texture == null || texture.LoadState == LoadState.Error)
            {
                //TODO
                /*
                   MyMwcLog.WriteLine("Failed to load prefab preview from disk: " + prefabType + " - " + prefabId + ", faction " + appearance + ". Rendering it on-the-fly.");

                   MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(prefabType, prefabId);
                   try
                   {
                       Texture preview = m_previewRenderer.RenderPrefabPreview(prefabId, config, appearance,
                                                                               MyHudConstants.PREFAB_PREVIEW_SIZE,
                                                                               MyHudConstants.PREFAB_PREVIEW_SIZE);
                       if (preview != null)
                       {
                           texture = preview;
                           AddPrefabPreview(prefabType, prefabId, appearance, preview);
                       }
                   }
                   catch (Exception e)
                   {
                       Debug.Assert(false, "Render preview failed " + e.Message);
                       MyMwcLog.WriteLine("Render preview failed");
                       MyMwcLog.WriteLine(e.ToString());
                   }
                 */
            }
            return texture;
        }


        //  This is default sprite batch used by all GUI screens and controls. Only GamePlay is different because renders 3D objects, and
        //  also some controls (e.g. textbox, combobox) are special cases because they use stencil-mask
        //  We can use and render more SpriteBatch objects at the same time. XNA should handle it.
        public static void BeginSpriteBatch()
        {
            BeginSpriteBatch(MyStateObjects.GuiDefault_BlendState);
        }

        public static void BeginSpriteBatch(BlendState blendState)
        {
            System.Diagnostics.Debug.Assert(System.Threading.Thread.CurrentThread == AllowedThread);

            if (m_spriteBatchUsageCount == 0)
            {
                if (m_stencilLevel > 0)
                {
                    StartStencilMask(m_stencilLevel);
                }
                else
                {
                    //  Deferred means that draw call will be send to GPU not on every Draw(), but only at the End() or if we change
                    //  a texture between Begin/End. It's faster than Immediate mode.
                    m_spriteBatch.Begin(SpriteSortMode.Deferred,
                                        blendState,
                                        SamplerState.LinearClamp,
                                        MyStateObjects.GuiDefault_DepthStencilState,
                                        RasterizerState.CullNone);
                }
            }
            m_spriteBatchUsageCount++;
        }

        public static void EndSpriteBatch()
        {
            System.Diagnostics.Debug.Assert(m_spriteBatchUsageCount > 0);
            System.Diagnostics.Debug.Assert(System.Threading.Thread.CurrentThread == AllowedThread);

            if (m_spriteBatchUsageCount == 0)
            {
                MyMwcLog.WriteLine("Sprite batch usage count is 0!");
            }

            if (m_spriteBatchUsageCount == 1)
            {
                //try
                {
                    m_spriteBatch.End();
                }

                /*
                catch(Exception e)
                {
                    MyMwcLog.WriteLine(e);

                    //This can raise disposed exception on Texture2D during loading. If we dont catch it, m_spriteBatchUsageCount will never gets corrected
                    UpdateAfterDeviceReset();
                }*/
            }
            m_spriteBatchUsageCount--;
        }

        private static int m_stencilLevel = 0;

        //  Draw the rectangle to stencil buffer, so later when we draw, only pixels with stencil=1 will be rendered
        //  IMPORTANT: Be careful about sprite batch, Begin and End
        public static void DrawStencilMaskRectangle(MyRectangle2D rectangle, MyGuiDrawAlignEnum drawAlignEnum)
        {
            //  Call begin with new stencil-mask parameters
            m_spriteBatch.Begin(SpriteSortMode.Deferred, MyStateObjects.StencilMask_Draw_BlendState, SamplerState.LinearWrap,
                MyStateObjects.StencilMask_Draw_DepthStencilState, RasterizerState.CullNone);

            //m_spriteBatch.Begin(SpriteSortMode.Deferred);

            m_spriteBatchUsageCount++;

            //  Render rectangle, but it will get written only into stencil buffer and color will remain untouched
            DrawSpriteBatch(GetBlankTexture(), rectangle.LeftTop, rectangle.Size, new Color(0, 0, 0, 255), drawAlignEnum);

            //  Call end so rectangle is rendered in this moment
            m_spriteBatch.End();
            m_spriteBatchUsageCount--;
        }

        //  Draw the rectangle to stencil buffer, so later when we draw, only pixels with stencil=1 will be rendered
        //  IMPORTANT: Be careful about sprite batch, Begin and End
        public static void DrawStencilMaskRectangleRoundUp(MyRectangle2D rectangle, MyGuiDrawAlignEnum drawAlignEnum)
        {
            // Clear stencil to avoid overflow
            //MyMinerGame.Static.GraphicsDevice.Clear(ClearOptions.Stencil, Color.White, 0.0f, 0);

            //  Call begin with new stencil-mask parameters
            m_spriteBatch.Begin(SpriteSortMode.Deferred, MyStateObjects.StencilMask_Draw_BlendState, SamplerState.LinearWrap,
                MyStateObjects.StencilMask_Draw_DepthStencilState, RasterizerState.CullNone);

            //m_spriteBatch.Begin(SpriteSortMode.Deferred);
            m_spriteBatchUsageCount++;

            //  Render rectangle, but it will get written only into stencil buffer and color will remain untouched
            DrawSpriteBatchRoundUp(GetBlankTexture(), rectangle.LeftTop, rectangle.Size, new Color(0, 0, 0, 255), drawAlignEnum);

            //  Call end so rectangle is rendered in this moment
            m_spriteBatch.End();
            m_spriteBatchUsageCount--;
        }


        //  Call this before you start drawing normal objects/scene. It will make sure that nothing will be drawn were stencil values are equal to 1.
        public static void BeginSpriteBatch_StencilMask()
        {
            m_stencilLevel++;
            StartStencilMask(m_stencilLevel);
        }

        public static void BeginSpriteBatch_StencilMask(int level)
        {
            StartStencilMask(level);
        }

        private static void StartStencilMask(int stencilLevel)
        {
            m_spriteBatch.Begin(SpriteSortMode.Deferred, MyStateObjects.GuiDefault_BlendState, SamplerState.LinearWrap,
                MyStateObjects.GetStencilMasks_TestBegin_DepthStencilState(stencilLevel), RasterizerState.CullNone);

            m_spriteBatchUsageCount++;
        }

        public static void EndSpriteBatch_StencilMask()
        {
            if (m_stencilLevel > 0)
            {
                m_stencilLevel--;
            }
            EndSpriteBatch();
            if (m_stencilLevel == 0)
            {
                MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.Stencil, new ColorBGRA(1.0f), 0.0f, 0);
            }
        }

        public static void TakeScreenshot()
        {
            //  Screenshot object survives only one DRAW after created. We delete it immediatelly. So if 'm_screenshot'
            //  is not null we know we have to take screenshot and set it to null.
            m_screenshot = new MyScreenshot(MyConfig.ScreenshotSizeMultiplier);
            MyMinerGame.UpdateScreenSize();
            MyGuiManager.UpdateScreenSize();
            MyCamera.UpdateScreenSize();
            MinerWars.AppCode.Game.Render.EnvironmentMap.MyEnvironmentMap.Reset();
        }

        public static void StopTakingScreenshot()
        {
            m_screenshot = null;
            MyMinerGame.UpdateScreenSize();
            MyGuiManager.UpdateScreenSize();
            MyCamera.UpdateScreenSize();
            Render.MyRender.CreateRenderTargets();
            MinerWars.AppCode.Game.Render.EnvironmentMap.MyEnvironmentMap.Reset();
        }


        public static Texture GetScreenshotTexture()
        {
            return m_renderSetup.RenderTargets[0];
        }

        public static MyScreenshot GetScreenshot()
        {
            return m_screenshot;
        }

        public static MyGuiPreviewRenderer GetPreviewRenderer()
        {
            return m_previewRenderer;
        }

        public static MyTexture2D GetToolTipBody()
        {
            return m_toltipBody;
        }

        public static MyTexture2D GetToolTipLeft()
        {
            return m_toltipLeft;
        }

        public static MyTexture2D GetToolTipRight()
        {
            return m_toltipRight;
        }

        public static Vector2 GetNormalizedCoordsAndPreserveOriginalSize(int width, int height)
        {
            return new Vector2((float)width / (float)MyMinerGame.ScreenSize.X, (float)height / (float)MyMinerGame.ScreenSize.Y);
        }

        // TEXTURES
        public static Dictionary<string, MyTexture2D> GetRemoteViewWeaponTextures()
        {
            if (m_remoteViewWeaponsTextures == null)
            {
                m_remoteViewWeaponsTextures = new Dictionary<string, MyTexture2D>(7);
                m_remoteViewWeaponsTextures.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_AMMO, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\LargeWeapon\\ammo", flags: TextureFlags.IgnoreQuality));
                m_remoteViewWeaponsTextures.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_BOTTOM, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\LargeWeapon\\bottom", flags: TextureFlags.IgnoreQuality));
                m_remoteViewWeaponsTextures.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_CROSS, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\LargeWeapon\\cross", flags: TextureFlags.IgnoreQuality));
                m_remoteViewWeaponsTextures.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_LEFT_SIDE, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\LargeWeapon\\left_side", flags: TextureFlags.IgnoreQuality));
                m_remoteViewWeaponsTextures.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_PULSE, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\LargeWeapon\\pulse", flags: TextureFlags.IgnoreQuality));
                m_remoteViewWeaponsTextures.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RASTR, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\LargeWeapon\\rastr", flags: TextureFlags.IgnoreQuality));
                m_remoteViewWeaponsTextures.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RIGHT_SIDE, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\LargeWeapon\\right_side", flags: TextureFlags.IgnoreQuality));
            }
            return m_remoteViewWeaponsTextures;
        }

        public static void RemoveRemoteViewWeaponTextures()
        {
            if (m_remoteViewWeaponsTextures == null) return;
            lock (m_remoteViewWeaponsTextures)
            {
                foreach (KeyValuePair<string, MyTexture2D> tex in m_remoteViewWeaponsTextures)
                {
                    if (tex.Value != null && tex.Value.LoadState == LoadState.Loaded)
                    {
                        tex.Value.Unload();
                    }
                }
            }
            m_remoteViewWeaponsTextures.Clear();
            m_remoteViewWeaponsTextures = null;
        }

        public static Dictionary<string, MyTexture2D> GetRemoteViewDroneTextures()
        {
            if (m_remoteViewDroneTextures == null)
            {
                m_remoteViewDroneTextures = new Dictionary<string, MyTexture2D>(5);
                m_remoteViewDroneTextures.Add(MyGuiConstants.REMOTE_VIEW_DRONE_BOTTOM, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Drone\\bottom", flags: TextureFlags.IgnoreQuality));
                m_remoteViewDroneTextures.Add(MyGuiConstants.REMOTE_VIEW_DRONE_CROSS, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Drone\\cross", flags: TextureFlags.IgnoreQuality));
                m_remoteViewDroneTextures.Add(MyGuiConstants.REMOTE_VIEW_DRONE_LEFT_SIDE, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Drone\\left_side", flags: TextureFlags.IgnoreQuality));
                m_remoteViewDroneTextures.Add(MyGuiConstants.REMOTE_VIEW_DRONE_RASTR, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Drone\\rastr", flags: TextureFlags.IgnoreQuality));
                m_remoteViewDroneTextures.Add(MyGuiConstants.REMOTE_VIEW_DRONE_RIGHT_SIDE, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Drone\\right_side", flags: TextureFlags.IgnoreQuality));
            }
            return m_remoteViewDroneTextures;
        }

        public static void RemoveRemoteViewDroneTextures()
        {
            if (m_remoteViewDroneTextures == null) return;
            lock (m_remoteViewDroneTextures)
            {
                foreach (KeyValuePair<string, MyTexture2D> tex in m_remoteViewDroneTextures)
                {
                    if (tex.Value != null && tex.Value.LoadState == LoadState.Loaded)
                    {
                        tex.Value.Unload();
                    }
                }
            }
            m_remoteViewDroneTextures.Clear();
            m_remoteViewDroneTextures = null;
        }

        public static Dictionary<string, MyTexture2D> GetRemoteViewCameraTextures()
        {
            if (m_remoteViewCameraTextures == null)
            {
                m_remoteViewCameraTextures = new Dictionary<string, MyTexture2D>(6);
                m_remoteViewCameraTextures.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_BOTTOM, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Camera\\bottom", flags: TextureFlags.IgnoreQuality));
                m_remoteViewCameraTextures.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_FOCUS, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Camera\\focus", flags: TextureFlags.IgnoreQuality));
                m_remoteViewCameraTextures.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_LEFT_SIDE, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Camera\\left_side", flags: TextureFlags.IgnoreQuality));
                m_remoteViewCameraTextures.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_REC, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Camera\\rec", flags: TextureFlags.IgnoreQuality));
                m_remoteViewCameraTextures.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_RIGHT_SIDE, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Camera\\right_side", flags: TextureFlags.IgnoreQuality));
                m_remoteViewCameraTextures.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_RASTR, MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\RemoteView\\Camera\\rastr", flags: TextureFlags.IgnoreQuality));
            }
            return m_remoteViewCameraTextures;
        }

        public static void RemoveRemoteViewCameraTextures()
        {
            if (m_remoteViewCameraTextures == null) return;
            lock (m_remoteViewCameraTextures)
            {
                foreach (KeyValuePair<string, MyTexture2D> tex in m_remoteViewCameraTextures)
                {
                    if (tex.Value != null && tex.Value.LoadState == LoadState.Loaded)
                    {
                        tex.Value.Unload();
                    }
                }
            }
            m_remoteViewCameraTextures.Clear();
            m_remoteViewCameraTextures = null;
        }

        // SIZES
        public static Dictionary<string, Vector2> GetRemoteViewCameraSizes()
        {
            Dictionary<string, Vector2> ret = new Dictionary<string, Vector2>(6);
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_BOTTOM, new Vector2(744, 59));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_FOCUS, new Vector2(391, 241));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_LEFT_SIDE, new Vector2(53, 971));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_RASTR, new Vector2(1920, 1200));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_REC, new Vector2(154, 47));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_RIGHT_SIDE, new Vector2(50, 971));
            return ret;
        }

        public static Dictionary<string, Vector2> GetRemoteViewDroneSizes()
        {
            Dictionary<string, Vector2> ret = new Dictionary<string, Vector2>(5);
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_BOTTOM, new Vector2(1820, 147));
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_CROSS, new Vector2(897, 487));
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_LEFT_SIDE, new Vector2(134, 1002));
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_RASTR, new Vector2(1920, 1200));
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_RIGHT_SIDE, new Vector2(130, 1002));
            return ret;
        }

        public static Dictionary<string, Vector2> GetRemoteViewWeaponSizes()
        {
            Dictionary<string, Vector2> ret = new Dictionary<string, Vector2>(7);
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_AMMO, new Vector2(189, 85));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_BOTTOM, new Vector2(760, 75));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_CROSS, new Vector2(600, 600));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_LEFT_SIDE, new Vector2(100, 988));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_PULSE, new Vector2(311, 101));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RASTR, new Vector2(1920, 1200));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RIGHT_SIDE, new Vector2(97, 986));
            return ret;
        }

        // POSITIONS
        public static Dictionary<string, Vector2> GetRemoteViewCameraPositions()
        {
            Dictionary<string, Vector2> ret = new Dictionary<string, Vector2>(6);
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_BOTTOM, new Vector2(0.5f, 1));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_FOCUS, new Vector2(0.5f, 0.5f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_LEFT_SIDE, MyVideoModeManager.IsTripleHead() ? new Vector2(0.335f, 0.5f) : new Vector2(0.005f, 0.5f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_RASTR, new Vector2(0, 0));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_REC, MyVideoModeManager.IsTripleHead() ? new Vector2(0.34f, 1f) : new Vector2(0.01f, 1f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_CAMERA_RIGHT_SIDE, MyVideoModeManager.IsTripleHead() ? new Vector2(0.66f, 0.5f) : new Vector2(1f, 0.5f));
            return ret;
        }

        public static Dictionary<string, Vector2> GetRemoteViewDronePositions()
        {
            Dictionary<string, Vector2> ret = new Dictionary<string, Vector2>(5);
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_BOTTOM, new Vector2(0.5f, 1));
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_CROSS, new Vector2(0.5f, 0.5f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_LEFT_SIDE, MyVideoModeManager.IsTripleHead() ? new Vector2(0.33f, 0.5f) : new Vector2(0, 0.5f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_RASTR, new Vector2(0, 0));
            ret.Add(MyGuiConstants.REMOTE_VIEW_DRONE_RIGHT_SIDE, MyVideoModeManager.IsTripleHead() ? new Vector2(0.67f, 0.5f) : new Vector2(1f, 0.5f));
            return ret;
        }

        public static Dictionary<string, Vector2> GetRemoteViewWeaponPositions()
        {
            Dictionary<string, Vector2> ret = new Dictionary<string, Vector2>(7);
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_AMMO, MyVideoModeManager.IsTripleHead() ? new Vector2(0.35f, 1f) : new Vector2(0.05f, 1f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_BOTTOM, new Vector2(0.5f, 1));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_CROSS, new Vector2(0.5f, 0.5f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_LEFT_SIDE, MyVideoModeManager.IsTripleHead() ? new Vector2(0.35f, 0.5f) : new Vector2(0.05f, 0.5f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_PULSE, MyVideoModeManager.IsTripleHead() ? new Vector2(0.65f, 1f) : new Vector2(0.95f, 1f));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RASTR, new Vector2(0, 0));
            ret.Add(MyGuiConstants.REMOTE_VIEW_LARGE_WEAPON_RIGHT_SIDE, MyVideoModeManager.IsTripleHead() ? new Vector2(0.65f, 0.5f) : new Vector2(0.95f, 0.5f));
            return ret;
        }

        static MyTexture2D m_bloodTexture = null;
        static MyTexture2D m_lowOxygenTexture = null;

        public static MyTexture2D GetBloodTexture()
        {
            if (m_bloodTexture == null)
            {
                m_bloodTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\LowHealthBlood", flags: TextureFlags.IgnoreQuality);
            }
            return m_bloodTexture;
        }

        public static MyTexture2D GetLowOxygenTexture()
        {
            if (m_lowOxygenTexture == null)
            {
                m_lowOxygenTexture = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\LowOxygen", flags: TextureFlags.IgnoreQuality);
            }
            return m_lowOxygenTexture;
        }

        public static void RemoveBloodTexture()
        {
            if (m_bloodTexture != null)
            {
                m_bloodTexture.Unload();
                m_bloodTexture = null;
            }
        }

        public static void RemoveNoOxygenTexture()
        {
            if (m_lowOxygenTexture != null)
            {
                m_lowOxygenTexture.Unload();
                m_lowOxygenTexture = null;
            }
        }

        public static void CloseIngameScreens()
        {
            for (int i = 0; i < m_screens.Count; i++)
            {
                if (IsIngameScreen(m_screens[i]))
                {
                    m_screens[i].CloseScreen();
                }
            }
        }

        public static bool IsIngameScreen(MyGuiScreenBase screen)
        {
            return screen is MyGuiScreenInventory
                || screen is MyGuiScreenSecurityControlHUB;
        }

        internal static void UpdateAfterDeviceReset()
        {
            //System.Diagnostics.Debug.Assert(MyMinerGame.BatchCanBeCalled);

            // Unload sprite and effect
            /*
            if (m_spriteBatch != null)
            {
                m_spriteBatch.Dispose();
                m_spriteBatch = null;
            }
              */
            /*
            if (m_spriteEffect != null)
            {
                m_spriteEffect.Dispose();
                m_spriteEffect = null;
            } */

            // Load sprite and effect
            //m_spriteBatchOwn = new MyGuiSpriteBatchOwn();
            
            /*m_spriteBatch = new SpriteBatch(MyMinerGame.Static.GraphicsDevice);
            m_spriteBatchUsageCount = 0;
            /*
        m_spriteEffect = new MyEffectSpriteBatchOriginal();
        m_spriteEffect.SetTransform(Matrix.Identity);*/
        }
    }
}