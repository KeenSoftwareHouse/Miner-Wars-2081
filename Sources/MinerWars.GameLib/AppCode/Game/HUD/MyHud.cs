using System;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Radar;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.CommonLIB.AppCode.Generics;
using SysUtils.Utils;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.World.Global;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Textures;
using SysUtils;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Missions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Audio;
using ParallelTasks;
using MinerWars.AppCode.Game.Entities.Tools;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Renders;

using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Utils.VertexFormats;

//using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


//  This class draws crosshair, angles and all HUD stuff. 
//
//  Coordinates are ALMOST in <0..1> range, but think about this:
//  Screen size -> width is always 1.0, but height depends on aspect ratio, so usualy it is 0.8 or something.
//
//  Every line is composed of three rectangles. Middle rectangle is for the main line and two other represents circles at the end.
//  IMPORTANT: Above line apply only if DRAW_LINE_ENDS enables it. Reason why I don't use it is because I had problems making that 
//  end texture correct and without ends it's even faster (less rectangles to draw)

namespace MinerWars.AppCode.Game.HUD
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MathHelper = MinerWarsMath.MathHelper;
    


    enum MyHudDrawPassEnum
    {
        FIRST,
        SECOND
    }


    enum MyGuitargetMode
    {
        Neutral,
        Enemy,
        Friend,
        Objective,
        ObjectiveOptional,
        CargoBox,
    }

    [Flags]
    enum MyHudDrawElementEnum 
    {
        NONE = 0,
        DIRECTION_INDICATORS = 1 << 0,
        CROSSHAIR = 1 << 1,        
        DAMAGE_INDICATORS = 1 << 2,
        AMMO = 1 << 3,
        HARVEST_MATERIAL = 1 << 4,
        BARGRAPHS_PLAYER_SHIP = 1 << 6,
        BARGRAPHS_LARGE_WEAPON = 1 << 7,
        DIALOGUES = 1 << 8,
        MISSION_OBJECTIVES = 1 << 9,
        BACK_CAMERA = 1 << 10,
        WHEEL_CONTROL = 1 << 11,
        CROSSHAIR_DYNAMIC = 1 << 12,
    }

    [Flags]
    enum MyHudIndicatorFlagsEnum
    {
        NONE = 0,
        SHOW_TEXT = 1 << 0,
        SHOW_BORDER_INDICATORS = 1 << 1,
        SHOW_HEALTH_BARS = 1 << 2,
        SHOW_ONLY_IF_DETECTED_BY_RADAR = 1 << 3,
        SHOW_DISTANCE = 1 << 4,
        ALPHA_CORRECTION_BY_DISTANCE = 1 << 5,
        SHOW_MISSION_MARKER = 1 << 6,
        SHOW_FACTION_RELATION_MARKER = 1 << 7,
        SHOW_LOCKED_TARGET = 1 << 8,
        SHOW_LOCKED_SIDE_TARGET = 1 << 9,
        SHOW_ICON = 1 << 10,

        SHOW_ALL = SHOW_TEXT | SHOW_BORDER_INDICATORS | SHOW_HEALTH_BARS | SHOW_ONLY_IF_DETECTED_BY_RADAR | SHOW_DISTANCE | 
                   ALPHA_CORRECTION_BY_DISTANCE | SHOW_MISSION_MARKER | SHOW_FACTION_RELATION_MARKER,
        
    }

    struct MyHudDisplayFactionRelation 
    {
        public bool Friend;
        public bool Neutral;
        public bool Enemy;

        public static MyHudDisplayFactionRelation Default 
        {
            get 
            {
                return new MyHudDisplayFactionRelation(true, true, true);
            }
        }        

        public MyHudDisplayFactionRelation(bool friend, bool neutral, bool enemy) 
        {
            Friend = friend;
            Neutral = neutral;
            Enemy = enemy;
        }

        public bool CanBeDisplayed(MyFactionRelationEnum factionRelation) 
        {
            switch(factionRelation)
            {
                case MyFactionRelationEnum.Friend:
                    return Friend;
                case MyFactionRelationEnum.Neutral:
                    return Neutral;
                case MyFactionRelationEnum.Enemy:
                    return Enemy;
            }
            throw new MyMwcExceptionApplicationShouldNotGetHere();
        }
    }

    enum MyHudMaxDistanceMultiplerTypes
    {
        CargoBoxAmmo,
        CargoBoxMedkit,
        CargoBoxOxygen,
        CargoBoxEnergy,
        CargoBoxFuel,
        CargoBoxRepair,
    }

    struct MyHudEntityParams
    {
        public StringBuilder Text { get; set; }
        public MyHudTexturesEnum? Icon { get; set; }
        public Vector2 IconOffset { get; set; }
        public Vector2 IconSize { get; set; }
        public Color? IconColor { get; set; }
        public MyHudIndicatorFlagsEnum FlagsEnum { get; set; }
        public MyGuitargetMode? TargetMode { get; set; }
        public float MaxDistance { get; set; }
        public MyHudDisplayFactionRelation DisplayFactionRelation { get; set; }
        public MyHudMaxDistanceMultiplerTypes? MaxDistanceMultiplerType;

        public MyHudEntityParams(StringBuilder text, MyGuitargetMode? targetMode, float maxDistance, MyHudIndicatorFlagsEnum flagsEnum, MyHudDisplayFactionRelation? displayFactionRelation = null, MyHudMaxDistanceMultiplerTypes? maxDistanceMultiplerType = null)
            : this()
        {
            this.Text = text;
            this.FlagsEnum = flagsEnum;
            this.MaxDistance = maxDistance;
            this.TargetMode = targetMode;
            this.MaxDistanceMultiplerType = maxDistanceMultiplerType;

            if (displayFactionRelation.HasValue)
            {
                DisplayFactionRelation = displayFactionRelation.Value;
            }
            else 
            {
                DisplayFactionRelation = MyHudDisplayFactionRelation.Default;
            }
        }
    }

    //  This enums must have same name as source texture files used to create texture atlas
    //  And only ".tga" files are supported.
    //  IMPORTANT: If you change order or names in this enum, update it also in MyEnumsToStrings
    enum MyHudTexturesEnum : byte
    {
        Line,
        DirectionIndicator_blue,
        DirectionIndicator_green,
        DirectionIndicator_red,
        DirectionIndicator_white,
        Target,
        Crosshair_locked,
        Crosshair_side_locked,
        Crosshair01,
        HorizontalLineLeft,
        HorizontalLineRight,
        damage_direction,
        BackCameraOverlay,
        //RadarOverlay,
        Rectangle,

        hudStatusArmor_blue,
        hudStatusArmor_red,
        hudStatusFuel_blue,
        hudStatusFuel_red,
        hudStatusOxygen_blue,
        hudStatusOxygen_red,
        hudStatusPlayerHealth_blue,
        hudStatusPlayerHealth_red,
        hudStatusShipDamage_blue,
        hudStatusShipDamage_red,
        hudStatusSpeed,
        hudStatusBar_blue,
        hudStatusBar_red,
        hudUnderbarSmall,
        hudUnderbarBig,
        TargetBlue,
        TargetGreen,
        TargetRed,
        CargoBoxIndicator,
        Unpowered_blue,
        Unpowered_green,
        Unpowered_red,
        Unpowered_white,
        HudOre,
        crosshair_nazzi,
        crosshair_omnicorp,
        crosshair_russian,
        crosshair_templary,
        Sun,
        crosshair_nazzi_red,
        crosshair_omnicorp_red,
        crosshair_russian_red,
        crosshair_templary_red,
        Crosshair01_red,
    }

    static class MyHud
    {

        class MyHudSetting
        {
            public MyGuiFont Font { get; set; }
            public MyHudTexturesEnum TextureDirectionIndicator { get; set; }
            public MyHudTexturesEnum TextureTarget { get; set; }
            public Color Color { get; set; }
            public float TextureTargetRotationSpeed { get; set; }
            public float TextureTargetScale { get; set; }
            public MyHudTexturesEnum TextureUnpowered { get; set; }

            public MyHudSetting(MyGuiFont font, MyHudTexturesEnum textureDirectionIndicator, MyHudTexturesEnum textureTarget, MyHudTexturesEnum textureUnpowered, Color color, float textureTargetRotationSpeed = 0f, float textureTargetScale = 1f) 
            {
                Font = font;
                TextureDirectionIndicator = textureDirectionIndicator;
                TextureTarget = textureTarget;
                this.Color = color;
                TextureTargetRotationSpeed = textureTargetRotationSpeed;
                TextureTargetScale = textureTargetScale;
                TextureUnpowered = textureUnpowered;
            }

        }

        class MyHudAlphaCorrection
        {
            public float MinDistance { get; set; }
            public float MaxDistance { get; set; }
            public float MinDistanceAlpha { get; set; }
            public float MaxDistanceAlpha { get; set; }

            public MyHudAlphaCorrection(float minDistance, float maxDistance, float minDistanceAlpha, float maxDistanceAlpha)
            {
                MinDistance = minDistance;
                MaxDistance = maxDistance;
                MinDistanceAlpha = minDistanceAlpha;
                MaxDistanceAlpha = maxDistanceAlpha;
            }

            public byte GetAlphaByDistance(float distance)
            {
                float fixedDistance = Math.Min(Math.Max(MinDistance, distance), MaxDistance);
                float colorAlpha = MathHelper.Lerp(MinDistanceAlpha, MaxDistanceAlpha, (fixedDistance - MinDistance) / (MaxDistance - MinDistance));
                byte colorAlhpaInByte = (byte)MathHelper.Lerp(byte.MinValue, byte.MaxValue, colorAlpha);
                return colorAlhpaInByte;
            }
        }        

        public const MyHudIndicatorFlagsEnum DEFAULT_FLAGS = MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER | MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER;

        static MyTexture2D m_texture;
        static MyAtlasTextureCoordinate[] m_textureCoords;
        static MyVertexFormatPositionTextureColor[] m_vertices;
        static Matrix m_orthographicProjectionMatrix;
        static int m_quadsCount;
        static MyObjectsPoolSimple<MyHudText> m_texts;

        public static MyTexture2D Texture
        {
            get { return m_texture; }
        }

        private static float m_missionHighlight;
        private static bool m_missionHighlightIncreasing;

        /// <summary>
        /// List of entitis for which we show some custom hud text.
        /// </summary>
        private static List<Tuple<WeakReference, MyHudEntityParams>> HudEntities;

        // THIS IS USED ONLY FOR DEBUG HUD
        private static List<Tuple<Vector3, MyHudEntityParams>> HudDebugPoints;

        //  String Builder helpers for creating and filling texts - do not reuse them for different texts, because string builders
        //  work by reference so then you could overwrite your previous text. Instead, make new sb-helper for each drawn text
        static readonly StringBuilder m_sbHelperAmountOfAmmoTextSeparator = new StringBuilder(": ");
        static readonly StringBuilder m_sbHelperRadarZoomEndX = new StringBuilder(" x");
        static readonly StringBuilder m_sbSpaceSeparator = new StringBuilder(" ");
        static readonly StringBuilder m_sbSpeed = new StringBuilder("m/s");
        static readonly StringBuilder m_sbPercentage = new StringBuilder("%");

        static readonly StringBuilder m_sbControlling = MyTextsWrapper.Get(MyTextsWrapperEnum.Controlling);

        static StringBuilder m_missionDescriptions;
        static MyGuiControlMultilineText m_dialogueTextAreaControl;
        static MyGuiControlMultilineText m_dialogueActorNameControl;
        static MyDialogueSentence m_lastDialogueSentence = null;

        // debug hud
        public static bool Visible = true;
        public static bool ShowDebugHud = false;
        public static bool ShowDebugWaypoints = false;
        public static bool ShowDebugWaypointsCollisions = false;
        public static bool ShowDebugGeneratedWaypoints = false;
        private static StringBuilder m_hudDebugText = new StringBuilder();
        private static StringBuilder m_textIndestructible = new StringBuilder("Indestructible");
        private const char m_hudHealthSeparator = '/';
        private static StringBuilder m_hudHealthAndArmorSeparator = new StringBuilder(" : ");

        // enemies in HUD
        private static MyHudEnemies m_hudEnemies;

        private static MyHudDrawElementEnum[] m_drawForCameraAttachedTo;

        private static MyHudEntityParams m_sunHudParams;
        private static MyHudEntityParams m_madelynHudParams;
        private static float m_sunHudIconColorAlpha = 0f;
        private static bool m_sunHudIconColorAlphaIncreasing = true;
        private const float SUN_BLINK_ALPHA_INCREMENT = 0.1f;

        private static MyHudAlphaCorrection[] m_hudAlphaCorrection;
        private static MyHudSetting[] m_hudSettings;

        //private static List<IMyObjectToDetect> m_detectedObjects = new List<IMyObjectToDetect>();

        private static bool m_wasMothershipHudDisplayed = false;

        /// <summary>
        /// Initializes the <see cref="MyHud"/> class.
        /// </summary>
        static MyHud()
        {
            HudEntities = new List<Tuple<WeakReference, MyHudEntityParams>>();
            m_hudEnemies = new MyHudEnemies();

            HudMaxDistanceMultiplers = new float[MyMwcUtils.GetMaxValueFromEnum<MyHudMaxDistanceMultiplerTypes>() + 1];
            for (int i = 0; i < HudMaxDistanceMultiplers.Length; i++)
            {
                HudMaxDistanceMultiplers[i] = 1.0f;
            }

#if DEBUG
            HudDebugPoints = new List<Tuple<Vector3, MyHudEntityParams>>();
#endif
            m_drawForCameraAttachedTo = new MyHudDrawElementEnum[MyMwcUtils.GetMaxValueFromEnum<MyCameraAttachedToEnum>() + 1];
            m_hudAlphaCorrection = new MyHudAlphaCorrection[MyMwcUtils.GetMaxValueFromEnum<MyGuitargetMode>() + 1];
            m_hudSettings = new MyHudSetting[MyMwcUtils.GetMaxValueFromEnum<MyGuitargetMode>() + 1];
            LoadDrawForCameraAttachedTo();

            DamageIndicators = new DamageIndicator[MAX_DAMAGE_INDICATORS];
            for (int i = 0; i < DamageIndicators.Length; i++)
            {
                DamageIndicators[i] = new DamageIndicator();
            }
        }

        static void LoadDrawForCameraAttachedTo()
        {
            MyHudDrawElementEnum drawForPlayerShip
                = MyHudDrawElementEnum.AMMO | MyHudDrawElementEnum.BARGRAPHS_PLAYER_SHIP |
                  MyHudDrawElementEnum.DIALOGUES | MyHudDrawElementEnum.DIRECTION_INDICATORS |
                  MyHudDrawElementEnum.DAMAGE_INDICATORS |
                  MyHudDrawElementEnum.MISSION_OBJECTIVES |
                  MyHudDrawElementEnum.BACK_CAMERA | MyHudDrawElementEnum.WHEEL_CONTROL;

            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.BotMinerShip] = MyHudDrawElementEnum.NONE;
            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.Camera] = MyHudDrawElementEnum.MISSION_OBJECTIVES | MyHudDrawElementEnum.DIALOGUES;
            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.Drone] = MyHudDrawElementEnum.DIRECTION_INDICATORS | MyHudDrawElementEnum.MISSION_OBJECTIVES | MyHudDrawElementEnum.DIALOGUES;
            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.LargeWeapon] = MyHudDrawElementEnum.BARGRAPHS_LARGE_WEAPON | MyHudDrawElementEnum.DIALOGUES | MyHudDrawElementEnum.MISSION_OBJECTIVES | MyHudDrawElementEnum.DIRECTION_INDICATORS | MyHudDrawElementEnum.DAMAGE_INDICATORS;
            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.PlayerMinerShip] = drawForPlayerShip | MyHudDrawElementEnum.CROSSHAIR;
            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic] = drawForPlayerShip | MyHudDrawElementEnum.CROSSHAIR_DYNAMIC;
            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonFollowing] = MyHudDrawElementEnum.NONE;
            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonStatic] = MyHudDrawElementEnum.NONE;
            m_drawForCameraAttachedTo[(int)MyCameraAttachedToEnum.Spectator] = MyHudDrawElementEnum.DIRECTION_INDICATORS;
        }

        public static void LoadData()
        {
            m_sunHudParams = new MyHudEntityParams(MyTextsWrapper.Get(MyTextsWrapperEnum.Sun), null, 0f, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_ICON);
            m_sunHudParams.Icon = MyHudTexturesEnum.Sun;
            m_sunHudParams.IconColor = Color.White;
            m_sunHudParams.IconOffset = new Vector2(-0.02f, 0f);
            m_sunHudParams.IconSize = new Vector2(MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 6f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 6f);

            if (MySession.Static != null)
            {
                m_madelynHudParams = new MyHudEntityParams(MySession.Static.MotherShipPosition.Name, MyGuitargetMode.Friend, 0,
                    MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER);
            }

            m_oreHudParams = new MyHudEntityParams(null, MyGuitargetMode.Neutral, 2000f, MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_TEXT);

            m_hudAlphaCorrection[(int)MyGuitargetMode.Friend] = new MyHudAlphaCorrection(MyHudConstants.HUD_MIN_DISTANCE_TO_ALPHA_CORRECT, MyHudConstants.HUD_MAX_DISTANCE_TO_ALPHA_CORRECT_NORMAL, MyHudConstants.HUD_MIN_DISTANCE_ALPHA, MyHudConstants.HUD_MAX_DISTANCE_ALPHA);
            m_hudAlphaCorrection[(int)MyGuitargetMode.Neutral] = new MyHudAlphaCorrection(MyHudConstants.HUD_MIN_DISTANCE_TO_ALPHA_CORRECT, MyHudConstants.HUD_MAX_DISTANCE_TO_ALPHA_CORRECT_NORMAL, MyHudConstants.HUD_MIN_DISTANCE_ALPHA, MyHudConstants.HUD_MAX_DISTANCE_ALPHA);
            m_hudAlphaCorrection[(int)MyGuitargetMode.Enemy] = new MyHudAlphaCorrection(MyHudConstants.HUD_MIN_DISTANCE_TO_ALPHA_CORRECT, MyHudConstants.HUD_MAX_DISTANCE_TO_ALPHA_CORRECT_ENEMIES, MyHudConstants.HUD_MIN_DISTANCE_ALPHA, MyHudConstants.HUD_MAX_DISTANCE_ALPHA);
            m_hudAlphaCorrection[(int)MyGuitargetMode.Objective] = null;
            m_hudAlphaCorrection[(int)MyGuitargetMode.ObjectiveOptional] = null;
            m_hudAlphaCorrection[(int)MyGuitargetMode.CargoBox] = null;

            m_oreHudSetting = new MyHudSetting(MyGuiManager.GetFontMinerWarsWhite(), MyHudTexturesEnum.DirectionIndicator_white, MyHudTexturesEnum.HudOre, MyHudTexturesEnum.Unpowered_white, Color.White);
            m_hudSettings[(int)MyGuitargetMode.Friend] = new MyHudSetting(MyHudConstants.FRIEND_FONT, MyHudTexturesEnum.DirectionIndicator_green, MyHudTexturesEnum.TargetGreen, MyHudTexturesEnum.Unpowered_green, MyHudConstants.FRIEND_MARKER_COLOR);
            m_hudSettings[(int)MyGuitargetMode.Neutral] = new MyHudSetting(MyHudConstants.NEUTRAL_FONT, MyHudTexturesEnum.DirectionIndicator_white, MyHudTexturesEnum.Target, MyHudTexturesEnum.Unpowered_white, MyHudConstants.NEUTRAL_MARKER_COLOR);
            m_hudSettings[(int)MyGuitargetMode.Enemy] = new MyHudSetting(MyHudConstants.ENEMY_FONT, MyHudTexturesEnum.DirectionIndicator_red, MyHudTexturesEnum.TargetRed, MyHudTexturesEnum.Unpowered_red, MyHudConstants.BOT_MARKER_COLOR);
            m_hudSettings[(int)MyGuitargetMode.Objective] = new MyHudSetting(MyHudConstants.MISSION_FONT, MyHudTexturesEnum.DirectionIndicator_blue, MyHudTexturesEnum.TargetBlue, MyHudTexturesEnum.Unpowered_blue, MyHudConstants.MISSION_MARKER_COLOR_BLUE);
            m_hudSettings[(int)MyGuitargetMode.ObjectiveOptional] = new MyHudSetting(MyHudConstants.MISSION_FONT, MyHudTexturesEnum.DirectionIndicator_blue, MyHudTexturesEnum.TargetBlue, MyHudTexturesEnum.Unpowered_blue, MyHudConstants.MISSION_MARKER_COLOR_BLUE);
            m_hudSettings[(int)MyGuitargetMode.CargoBox] = new MyHudSetting(MyHudConstants.NEUTRAL_FONT, MyHudTexturesEnum.DirectionIndicator_white, MyHudTexturesEnum.CargoBoxIndicator, MyHudTexturesEnum.Unpowered_white, MyHudConstants.NEUTRAL_MARKER_COLOR, 0.75f, 1.5f);
        }

        public static void LoadContent(MyGuiScreenBase parent)
        {     
            MyMwcLog.WriteLine("MyHud.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyHud::LoadContent");

            MyUtils.LoadTextureAtlas(MyEnumsToStrings.HudTextures, "Textures\\HUD\\", MyMinerGame.Static.RootDirectory + "\\Textures\\HUD\\HudAtlas.tai", out m_texture, out m_textureCoords);
            Debug.Assert(m_texture.LevelCount > 1, "HudAtlas does not have mip maps geneated");

            m_texts = new MyObjectsPoolSimple<MyHudText>(MyHudConstants.MAX_HUD_TEXTS_COUNT);
            m_vertices = new MyVertexFormatPositionTextureColor[MyHudConstants.MAX_HUD_QUADS_COUNT * MyHudConstants.VERTEXES_PER_HUD_QUAD];

            Vector2 size = new Vector2(0.4f, 0.25f);
            Vector2 origin = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(new Vector2(MyMinerGame.ScreenSize.X, 0f))
                + new Vector2(-size.X / 2f, size.Y / 2f);
            m_missionDescriptions = new StringBuilder();
            new MyGuiControlMultilineText(parent, origin, size, MyGuiConstants.MULTILINE_LABEL_BACKGROUND_COLOR,
                                          MyGuiManager.GetFontMinerWarsWhite(), 0.6f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP, m_missionDescriptions);

            m_dialogueTextAreaControl = new MyGuiControlMultilineText(parent, MyHudConstants.DIALOGUE_TEXTAREA_POSITION, MyHudConstants.DIALOGUE_TEXTAREA_SIZE,
                Vector4.Zero, MyGuiManager.GetFontMinerWarsBlue(), MyHudConstants.DIALOGUE_TEXTAREA_FONT_SIZE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP, new StringBuilder(), false, false);

            m_dialogueActorNameControl = new MyGuiControlMultilineText(parent, MyHudConstants.DIALOGUE_ACTORNAME_POSITION, MyHudConstants.DIALOGUE_ACTORNAME_SIZE,
                Vector4.Zero, MyGuiManager.GetFontMinerWarsBlue(), MyHudConstants.DIALOGUE_ACTORNAME_FONT_SIZE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP, new StringBuilder(), false, false);


            for (int i = 0; i < DamageIndicators.Length; i++)
            {
                DamageIndicators[i].Used = false;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyHud.LoadContent() - END");       
        }

        public static void UpdateScreenSize()
        {
            m_orthographicProjectionMatrix = Matrix.CreateOrthographicOffCenter(0.0f, MyGuiManager.GetHudSize().X, MyGuiManager.GetHudSize().Y, 0.0f, 0.0f, 1000);//-1.0f);
        }

        public static void UnloadData()
        {
            HudEntities.Clear();
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyHud.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            if (m_missionDescriptions != null)
            {
                m_missionDescriptions.Clear();
                m_missionDescriptions = null;
            }

            m_dialogueTextAreaControl = null;
            m_lastDialogueSentence = null;

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyHud.UnloadContent - END");
        }        

        //  Draw class is in two passes, because first pass is drawn before we draw back camera. So it's about having sorting.
        public static void Draw(MyHudDrawPassEnum pass, MyCameraAttachedToEnum cameraAttachedTo, bool backCamera)
        {
            MyRender.GetRenderProfiler().StartProfilingBlock("Clear");
            MyCamera.EnableHud();

            ClearQuads();
            ClearTexts();

            MyRender.GetRenderProfiler().EndProfilingBlock();

            MyHudDrawElementEnum drawForCameraAttachedTo = m_drawForCameraAttachedTo[(int)cameraAttachedTo];

            if (pass == MyHudDrawPassEnum.FIRST)
            {
                if ((drawForCameraAttachedTo & MyHudDrawElementEnum.DIRECTION_INDICATORS) != 0)
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddDirectionIndicators");
                    AddDirectionIndicators();
                    MyRender.GetRenderProfiler().EndProfilingBlock();

                    MyRender.GetRenderProfiler().StartProfilingBlock("AddOreMarkers");
                    if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
                    {
                        AddOreMarkers();
                    }
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                if ((drawForCameraAttachedTo & MyHudDrawElementEnum.DAMAGE_INDICATORS) != 0)
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddDamageIndicators");
                    AddDamageIndicators();
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }
            }
            else if (pass == MyHudDrawPassEnum.SECOND)
            {

                if ((drawForCameraAttachedTo & MyHudDrawElementEnum.CROSSHAIR) != 0 || MyFakes.CUBE_EDITOR)
                {
                    if (!MySession.Is25DSector)
                    {
                        MyRender.GetRenderProfiler().StartProfilingBlock("AddHorisontalAngleLine");
                        AddHorisontalAngleLine();
                        MyRender.GetRenderProfiler().EndProfilingBlock();

                        MyRender.GetRenderProfiler().StartProfilingBlock("AddCrosshair");
                        if (CanDrawCrossHair())
                        {
                            AddCrosshair();
                        }
                        MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }
                if ((drawForCameraAttachedTo & MyHudDrawElementEnum.CROSSHAIR_DYNAMIC) != 0)
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddCrosshairDynamic");
                    if (CanDrawCrossHair())
                    {
                        AddCrosshairDynamic();
                    }
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                if (backCamera && (drawForCameraAttachedTo & MyHudDrawElementEnum.BACK_CAMERA) != 0)
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddBackCameraBorders");
                    AddBackCameraBorders();
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                if ((drawForCameraAttachedTo & MyHudDrawElementEnum.AMMO) != 0)
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddAmountOfAmmo");
                    AddAmountOfAmmo();
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                if ((drawForCameraAttachedTo & MyHudDrawElementEnum.HARVEST_MATERIAL) != 0)
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddHarvestedMaterial");
                    AddHarvestedMaterial();
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                bool drawBargraphsForPlayerShip = (drawForCameraAttachedTo & MyHudDrawElementEnum.BARGRAPHS_PLAYER_SHIP) != 0;
                bool drawBargraphsForLargeWeapon = (drawForCameraAttachedTo & MyHudDrawElementEnum.BARGRAPHS_LARGE_WEAPON) != 0;
                if (drawBargraphsForPlayerShip || drawBargraphsForLargeWeapon) 
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddBargraphsHUD");
                    AddBargraphsHUD(drawBargraphsForPlayerShip, drawBargraphsForLargeWeapon);
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                if ((drawForCameraAttachedTo & MyHudDrawElementEnum.DIALOGUES) != 0 && MySubtitles.Enabled) 
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddDialogues");
                    AddDialogues();
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                if ((drawForCameraAttachedTo & MyHudDrawElementEnum.MISSION_OBJECTIVES) != 0) 
                {
                    MyRender.GetRenderProfiler().StartProfilingBlock("AddMissionObjectives");
                    AddMissionObjectives();
                    MyRender.GetRenderProfiler().EndProfilingBlock();
                }

                MyRender.GetRenderProfiler().StartProfilingBlock("MyHudWarnings.Update");
                MyHudWarnings.Update();
                MyRender.GetRenderProfiler().EndProfilingBlock();

                MyRender.GetRenderProfiler().StartProfilingBlock("MyHudWarnings.Draw");
                MyHudWarnings.Draw();
                MyRender.GetRenderProfiler().EndProfilingBlock();
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            MyRender.GetRenderProfiler().StartProfilingBlock("DrawVertices");
            DrawVertices();
            MyRender.GetRenderProfiler().EndProfilingBlock();

            MyRender.GetRenderProfiler().StartProfilingBlock("DrawTexts");
            DrawTexts();
            MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static bool CanDrawCrossHair()
        {
            bool isPlayerShipDrillActive = 
                MyGuiScreenGamePlay.Static.ControlledEntity == MySession.PlayerShip &&
                (MySession.PlayerShip.Weapons.GetMountedDrill() != null &&
                MySession.PlayerShip.Weapons.GetMountedDrill().CurrentState != MyDrillStateEnum.InsideShip)
                ||
                (MySession.PlayerShip.Weapons.GetMountedHarvestingDevice() != null
                && MySession.PlayerShip.Weapons.GetMountedHarvestingDevice().IsHarvesterActive);

            bool isAlive = MySession.PlayerShip != null && !MySession.PlayerShip.IsDead() && !MySession.PlayerShip.IsPilotDead();

            return !isPlayerShipDrillActive && isAlive && !MySession.Static.Is2DSector;
        }

        public static void DrawOnlyMissionObjectives()
        {
            if (!MyHud.Visible)
            {
                return;
            }

            MyCamera.EnableHud();

            ClearQuads();
            ClearTexts();

            AddMissionObjectives();

            DrawVertices();
            DrawTexts();
        }

        public static void DrawAsControlledEntity()
        {
            MyCamera.EnableHud();

            ClearQuads();
            ClearTexts();

            if (MyGuiScreenGamePlay.Static.ControlledCamera != null)
            {
                MyHudText text = m_texts.Allocate();
                text.Start(MyHudConstants.NEUTRAL_FONT, new Vector2(MyGuiManager.GetHudSizeHalf().X, 0.99f), Color.White, 1.0f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);
                text.Append(m_sbControlling);
                text.Append(MyGuiScreenGamePlay.Static.ControlledCamera.DisplayName);
            }

            MyHudNotification.Draw();
            

            DrawVertices();
            DrawTexts();
        }

        public static void DrawOnlyBackCameraBorders()
        {
            MyCamera.EnableHud();

            ClearQuads();
            ClearTexts();

            AddBackCameraBorders();

            DrawVertices();
            DrawTexts();
        }

        public static bool EnableDrawingMissionObjectives = true;

        private static int m_missionLastBlinkTime = 0;
        private static bool m_missionBlinkOn = true;
        private static void AddMissionObjectives()
        {
            if (MySession.Static == null || !EnableDrawingMissionObjectives)
            {
                return;
            }

            int deltaTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_missionLastBlinkTime;
            if (deltaTime >= MyMissionsConstants.NEW_OBJECTIVE_BLINK_ON_TIME && m_missionBlinkOn ||
               deltaTime >= MyMissionsConstants.NEW_OBJECTIVE_BLINK_OFF_TIME && !m_missionBlinkOn) 
            {
                m_missionBlinkOn = !m_missionBlinkOn;
                m_missionLastBlinkTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }

            bool newMissionFound = false;
            if (MyMissions.ActiveMission == null)
            {
                newMissionFound = MyMissions.GetAvailableStoryMissions().Count > 0;
                
                if (!newMissionFound)
                {
                    return;
                }
            }
            
            //if (m_missionHighlightIncreasing)
            //    m_missionHighlight += 0.05f;
            //else
            //    m_missionHighlight -= 0.05f;

            //if (m_missionHighlight > 1)
            //    m_missionHighlightIncreasing = false;
            //else if (m_missionHighlight < 0.4f)
            //    m_missionHighlightIncreasing = true;
            if (m_missionBlinkOn)
            {
                m_missionHighlight = MyMissionsConstants.NEW_OBJECTIVE_BLINK_ON_HIGHLIGHT;
            }
            else 
            {
                m_missionHighlight = MyMissionsConstants.NEW_OBJECTIVE_BLINK_OFF_HIGHLIGHT;
            }
            
            float aspectRatio = MyGuiManager.GetHudSize().Y * 2.0f;
            //Vector2 origin = new Vector2(-0.5f, 0.01f) * aspectRatio + new Vector2(MyGuiManager.GetHudSize().X, 0);
            Vector2 origin = new Vector2(0.5f, 0.01f);

            MyHudText text = m_texts.Allocate();
            if (text != null)
            {
                //origin.X += 0.02f;
                Color missionMarkerColor = Color.White;// MyHudConstants.MISSION_MARKER_COLOR;
                if (MyMissions.IsNewMissionOrObjectiveAvailable())
                {
                    missionMarkerColor *= m_missionHighlight;
                }
                text.Start(MyHudConstants.MISSION_FONT, origin, missionMarkerColor, 1.1f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);

                if (newMissionFound)
                {
                    text.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.HudNewMissionAvailable));
                }
                else
                {
                    text.Append(MyMissions.ActiveMission.NameTemp);
                    foreach (var submission in MyMissions.ActiveMission.ActiveObjectives)
                    {
                        if (submission.NameTemp != null && submission.NameTemp.Length != 0 && !MySession.Static.EventLog.IsMissionFinished(submission.ID))
                        {
                            text.AppendLine();
                            text.Append("- ");
                            text.Append(submission.NameTemp);
                            if (submission.AdditionalHudInformation != null && submission.AdditionalHudInformation.Length > 0)
                            {
                                text.Append(" ");
                                text.Append(submission.AdditionalHudInformation);
                            }
                        }
                    }
                }
            }

        }

        enum MyHudStatusEnum : int
        {
            MyHudStatusShipDamage = 0,
            MyHudStatusPlayerHealth,
            MyHudStatusArmor,
            MyHudStatusFuel,
            MyHudStatusElektricity,
            MyHudStatusOxygen,
            MyHudStatusSpeed
        };        

        private static void AddBargraphsHUD(bool addPlayerShip, bool addLargeWeapon)
        {
            Color hudDefaultColor = MyHudConstants.HUD_STATUS_DEFAULT_COLOR;
            Vector2 hudStatusIconSize = MyHudConstants.HUD_STATUS_ICON_SIZE;
            Vector2 hudStatusBarSize = MyHudConstants.HUD_STATUS_BAR_SIZE;
            float hudStatusIconDistance = MyHudConstants.HUD_STATUS_ICON_DISTANCE;
            float hudStatusBarDistance = MyHudConstants.HUD_STATUS_BAR_DISTANCE;
            Vector2 hudStatusSpeedSize = MyHudConstants.HUD_STATUS_SPEED_ICON_SIZE;
            MyHudTexturesEnum hudStatusIcon;

            //get aspect ratio from GUIHudSize 
            float aspectRatio = MyGuiManager.GetHudSize().Y * 2.0f;

            hudStatusIconSize *= aspectRatio;
            hudStatusBarSize *= aspectRatio;
            hudStatusIconDistance *= aspectRatio;
            hudStatusBarDistance *= aspectRatio;
            hudStatusSpeedSize *= aspectRatio;

            Vector2 origin = MyHudConstants.HUD_STATUS_POSITION * aspectRatio + new Vector2(0.0f, MyGuiManager.GetHudSize().Y);
            Vector2 position = origin;

            //// Draw backgrounds:
            //Vector2 backposition = origin;
            //Vector2 backSize = new Vector2(hudStatusIconSize.X * 6 + hudStatusIconDistance * 5, hudStatusBarSize.Y * 0.8f);

            //AddTexturedQuad(MyHudTexturesEnum.hudUnderbarSmall, backposition, MyHudConstants.HUD_VECTOR_UP, MyHudConstants.HUD_STATUS_BACKGROUND_COLOR, backSize.X, hudStatusIconSize.Y);

            //backposition.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f;
            //for (int i = 0; i < 7; ++i)
            //{
            //    AddTexturedQuad(MyHudTexturesEnum.hudUnderbarSmall, backposition, MyHudConstants.HUD_VECTOR_UP, MyHudConstants.HUD_STATUS_BACKGROUND_COLOR, backSize.X, backSize.Y);
            //    backposition.Y -= hudStatusBarSize.Y + hudStatusBarDistance;
            //}

            // Don't draw bars if anything is null, see bug #1756
            if (MyGuiScreenGamePlay.Static == null || MySession.PlayerShip == null || MyClientServer.LoggedPlayer == null)
            {
                return;
            }

            // Compute bar values (percents 0-100)
            var playerShip = MySession.PlayerShip;

            var player = MySession.Static.Player;

            float shipHealth = (1 - playerShip.GetDamageRatio()) * 100.0f;
            float playerHealth = MathHelper.Clamp(player.Health / player.MaxHealth * 100f, 0f, 100f);
            float armor = MathHelper.Clamp(playerShip.ArmorHealth / playerShip.MaxArmorHealth * 100f, 0f, 100f);
            float fuel = MathHelper.Clamp(playerShip.Fuel / playerShip.MaxFuel * 100f, 0f, 100f);
            float oxygen = MathHelper.Clamp(playerShip.Oxygen / playerShip.MaxOxygen * 100f, 0f, 100f);

            bool hasLowHealth = playerShip.IsDamagedForWarnignAlert();
            bool hasLowPlayerHealth = playerShip.HasLowHealthLevel();
            bool hasLowArmor = playerShip.HasLowArmorLevel();
            bool hasLowFuel = playerShip.HasLowFuelLevel();
            bool hasLowOxygen = playerShip.HasLowOxygenLevel();

            if (addPlayerShip)
            {   

                 //  Ship speed
                origin = position;
                origin.X -= 0.011f;
                origin.X *= aspectRatio;
                origin.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f + (MyHudConstants.HUD_STATUS_BAR_MAX_PIECES_COUNT + 1) * (hudStatusBarSize.Y + hudStatusBarDistance);
                DrawSpeedText(origin, new Color(1.0f, 1.0f, 1.0f, 1.0f));

                //  Ship Afterburner Status
                origin.X += 0.082f * aspectRatio;
                DrawAfterburnerPercentage(origin, new Color(1.0f, 1.0f, 1.0f, 1.0f));

                origin = position;
                origin.X += 0.005f;
                origin.X *= aspectRatio;
                origin.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f + (MyHudConstants.HUD_STATUS_BAR_MAX_PIECES_COUNT + 3) * (hudStatusBarSize.Y + hudStatusBarDistance);
                hudDefaultColor = MyHudConstants.HUD_STATUS_DEFAULT_COLOR;
                //AddTexturedQuad(MyHudTexturesEnum.hudStatusSpeed, origin, MyHudConstants.HUD_VECTOR_UP,
                //    hudDefaultColor, hudStatusSpeedSize.X, hudStatusSpeedSize.Y);


                origin = position;
                // Draw status HUD textures:
                hudStatusIcon = !hasLowHealth ? MyHudTexturesEnum.hudStatusShipDamage_blue : MyHudTexturesEnum.hudStatusShipDamage_red;
                AddTexturedQuad(hudStatusIcon, origin, MyHudConstants.HUD_VECTOR_UP,
                        Color.White, hudStatusIconSize.X, hudStatusIconSize.Y);
                origin.X += hudStatusIconDistance + hudStatusIconSize.X;

                hudStatusIcon = !hasLowPlayerHealth ? MyHudTexturesEnum.hudStatusPlayerHealth_blue : MyHudTexturesEnum.hudStatusPlayerHealth_red;
                AddTexturedQuad(hudStatusIcon, origin, MyHudConstants.HUD_VECTOR_UP,
                        Color.White, hudStatusIconSize.X, hudStatusIconSize.Y);
                origin.X += hudStatusIconDistance + hudStatusIconSize.X;
                
                hudStatusIcon = !hasLowArmor ? MyHudTexturesEnum.hudStatusArmor_blue : MyHudTexturesEnum.hudStatusArmor_red;
                AddTexturedQuad(hudStatusIcon, origin, MyHudConstants.HUD_VECTOR_UP,
                        Color.White, hudStatusIconSize.X, hudStatusIconSize.Y);
                origin.X += hudStatusIconDistance + hudStatusIconSize.X;
                
                hudStatusIcon = !hasLowFuel ? MyHudTexturesEnum.hudStatusFuel_blue : MyHudTexturesEnum.hudStatusFuel_red;
                AddTexturedQuad(hudStatusIcon, origin, MyHudConstants.HUD_VECTOR_UP,
                        Color.White, hudStatusIconSize.X, hudStatusIconSize.Y);
                origin.X += hudStatusIconDistance + hudStatusIconSize.X;
                
                hudStatusIcon = !hasLowOxygen ? MyHudTexturesEnum.hudStatusOxygen_blue : MyHudTexturesEnum.hudStatusOxygen_red;
                AddTexturedQuad(hudStatusIcon, origin, MyHudConstants.HUD_VECTOR_UP,
                        Color.White, hudStatusIconSize.X, hudStatusIconSize.Y);
                origin.X += 4f * hudStatusIconDistance + hudStatusIconSize.X;



                origin = position;
                position.X += hudStatusIconSize.X + hudStatusIconDistance;
                origin.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f;
                DrawBars(origin, hudStatusBarSize, MyHudConstants.HUD_STATUS_BAR_MAX_PIECES_COUNT,
                    shipHealth, hasLowHealth, hudStatusBarDistance);

                origin = position;
                position.X += hudStatusIconSize.X + hudStatusIconDistance;
                origin.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f;
                DrawBars(origin, hudStatusBarSize, MyHudConstants.HUD_STATUS_BAR_MAX_PIECES_COUNT,
                    playerHealth, hasLowPlayerHealth, hudStatusBarDistance);

                origin = position;
                position.X += hudStatusIconSize.X + hudStatusIconDistance;
                origin.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f;
                DrawBars(origin, hudStatusBarSize, MyHudConstants.HUD_STATUS_BAR_MAX_PIECES_COUNT,
                    armor, hasLowArmor, hudStatusBarDistance);

                origin = position;
                position.X += hudStatusIconSize.X + hudStatusIconDistance;
                origin.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f;
                DrawBars(origin, hudStatusBarSize, MyHudConstants.HUD_STATUS_BAR_MAX_PIECES_COUNT,
                    fuel, hasLowFuel, hudStatusBarDistance);

                origin = position;
                position.X += 3.0f * hudStatusIconSize.X + hudStatusIconDistance;
                origin.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f;
                DrawBars(origin, hudStatusBarSize, MyHudConstants.HUD_STATUS_BAR_MAX_PIECES_COUNT,
                    oxygen, hasLowOxygen, hudStatusBarDistance);



                //  Medicine
                origin = position;
                origin.X -= 0.0225f;
                origin.Y -= hudStatusIconSize.Y * 2.5f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f;
                foreach (var medicine in MySession.Static.Player.Medicines)
                {
                    if (medicine.IsActive())
                    {
                        DrawMedicineTypeAndDuration(medicine, origin, new Color(1.0f, 1.0f, 1.0f, 1.0f));
                        origin.Y -= 0.01f;
                    }
                }
            }
            
            if (addLargeWeapon)
            {
                Vector2 barPosition = new Vector2(MyGuiManager.GetHudSize().X / 2, MyGuiManager.GetHudSize().Y - 0.02f * aspectRatio);
                var largeWeapon = MyGuiScreenGamePlay.Static.ControlledLargeWeapon;
                float largeWeaponHealth = MathHelper.Clamp(largeWeapon.HealthRatio * 100, 0, 100);
                bool largeWeaponLowHealth = largeWeapon.IsDamagedForWarnignAlert();
                if (m_customEntityForHealth!=null)
                {
                    largeWeaponHealth = MathHelper.Clamp(m_customEntityForHealth.HealthRatio * 100, 0, 100);
                    largeWeaponLowHealth = false;
                }

                //origin = barPosition;
                origin = position;
                hudStatusIcon = !largeWeaponLowHealth ? MyHudTexturesEnum.hudStatusShipDamage_blue : MyHudTexturesEnum.hudStatusShipDamage_red;
                AddTexturedQuad(hudStatusIcon, origin, MyHudConstants.HUD_VECTOR_UP,
                        Color.White, hudStatusIconSize.X, hudStatusIconSize.Y);

                //origin = barPosition;
                origin = position;
                origin.Y -= hudStatusIconSize.Y / 2.0f + hudStatusIconDistance + hudStatusBarDistance + hudStatusBarSize.Y / 2.0f;
                DrawBars(origin, hudStatusBarSize, MyHudConstants.HUD_STATUS_BAR_MAX_PIECES_COUNT,
                    largeWeaponHealth, largeWeaponLowHealth, hudStatusBarDistance);
            }
        }

        private static MyEntity m_customEntityForHealth; 
        public static void DrawHealthOfCustomPrefabInLargeWeapon(MyEntity e)
        {
            m_customEntityForHealth = e;
        }

        public static void DetachHealthOfCustomPrefab()
        {
            m_customEntityForHealth = null;
        }


        private static void DrawBars(Vector2 position, Vector2 HudSize, int maxBarRows, float percent, bool isLow, float emptySpace = 0.01f)
        {
            int bars = (int)Math.Ceiling(maxBarRows / 100.0f * percent);
            MyHudTexturesEnum statusBarTexture = !isLow ? MyHudTexturesEnum.hudStatusBar_blue : MyHudTexturesEnum.hudStatusBar_red;

            for (int i = 0; i < maxBarRows; ++i)
            {
                Color color = Color.White;
                if (i >= bars) 
                {
                    color.A = 51;
                }
                AddTexturedQuad(statusBarTexture, position, MyHudConstants.HUD_VECTOR_UP, color, HudSize.X, HudSize.Y);
                position.Y -= HudSize.Y + emptySpace;
            }
        }

        private static Color GetBarIconColorByPercent(int maxbarRows, float Percent, ref int barsCorrect)
        {
            Color color = MyHudConstants.HUD_STATUS_BAR_COLOR_GREEN_STATUS;
            int bars = (int)Math.Ceiling(maxbarRows / 100.0f * Percent);

            if (bars <= 1)
            {
                color = MyHudConstants.HUD_STATUS_BAR_COLOR_RED_STATUS;
            }
            else if (bars > 1 && bars <= 2)
            {
                color = MyHudConstants.HUD_STATUS_BAR_COLOR_ORANGE_STATUS;
            }
            else if (bars > 2 && bars <= 3)
            {
                color = MyHudConstants.HUD_STATUS_BAR_COLOR_YELLOW_STATUS;
            }

            barsCorrect = bars;
            return color;
        }

        private static bool IsBarIconBlueByPercent(int maxbarRows, float Percent, ref int barsCorrect)
        {            
            int bars = (int)Math.Ceiling(maxbarRows / 100.0f * Percent);
            barsCorrect = bars;

            if (bars <= 3)
            {
                return false;
            }

            return true;
        }

        private static void DrawSpeedText(Vector2 position, Color color)
        {
            MyHudText hudText = m_texts.Allocate();
            if (hudText != null)
            {
                hudText.Start(MyGuiManager.GetFontMinerWarsBlue(), position, color, 0.7f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                hudText.AppendInt32((int)MySession.PlayerShip.Physics.Speed);
                hudText.Append(m_sbSpeed);
            }
        }

        private static void DrawAfterburnerPercentage(Vector2 position, Color color)
        {
            MyHudText hudText = m_texts.Allocate();
            if (hudText != null)
            {
                hudText.Start(MyGuiManager.GetFontMinerWarsBlue(), position, color, 0.7f, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER);
                hudText.AppendInt32((int)(MySession.PlayerShip.AfterburnerStatus * 100));
                hudText.Append(m_sbPercentage);
            }
        }


        private static void DrawMedicineTypeAndDuration(MyMedicine medicine, Vector2 position, Color color)
        {
            StringBuilder name;
            bool percent;
            float count = 0;
            switch (medicine.Type())
            {
                case MyMedicineType.MEDIKIT:
                    name = MyTextsWrapper.Get(MyTextsWrapperEnum.MedikitActive);
                    percent = false;
                    count = MySession.Static.Player.Ship.Inventory.GetTotalAmountOfInventoryItems(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.MEDIKIT);
                    break;
                case MyMedicineType.ANTIRADIATION_MEDICINE:
                    name = MyTextsWrapper.Get(MyTextsWrapperEnum.AntiradiationMedicineActive);
                    percent = true;
                    //count = MySession.Static.Player.Ship.Inventory.GetTotalAmountOfInventoryItems(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ANTIRADIATION_MEDICINE);
                    break;
                case MyMedicineType.HEALTH_ENHANCING_MEDICINE:
                    name = MyTextsWrapper.Get(MyTextsWrapperEnum.HealthEnhancingMedicineActive);
                    percent = true;
                    break;
                case MyMedicineType.PERFORMANCE_ENHANCING_MEDICINE:
                    name = MyTextsWrapper.Get(MyTextsWrapperEnum.PerformanceEnhancingMedicineActive);
                    percent = true;
                    break;
                default:
                    return;
            }
            
            MyHudText hudText = m_texts.Allocate();
            if (hudText != null)
            {
                hudText.Start(MyGuiManager.GetFontMinerWarsBlue(), position, color, 0.7f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                hudText.Append(name);
                if (percent)
                {
                    // display percent count
                    hudText.Append(": ");
                    hudText.AppendInt32(100 - (int)(medicine.TimeSinceActivation() / medicine.Duration() * 100));
                    hudText.Append(m_sbPercentage);
                }
                else
                {
                    // display count of doses left
                    hudText.Append(": ");
                    hudText.AppendInt32((int)count);
                    hudText.Append(" doses left");
                }
            }
        }

        private static StringBuilder m_dialogIdSB = new StringBuilder();
        private static void AddDialogues()
        {
          //  if (MyGuiScreenGamePlay.Static == null || MyDialogues.CurrentSentence == null)
            //    return;

             if (MyDialogues.CurrentSentence == null)
                return;

            if (MyDialogues.IsCurrentCuePausedAndHidden())
                return;

            MyGuiManager.BeginSpriteBatch();
            var offset = new Vector2(VideoMode.MyVideoModeManager.IsTripleHead() ? -1 : 0, 0);

            //need to be -1 on triple mon (big resolution)
            var offsetTextArea = new Vector2(VideoMode.MyVideoModeManager.IsTripleHead() ? -1 : 0, 0);

            if (m_lastDialogueSentence != MyDialogues.CurrentSentence)
            {
                m_lastDialogueSentence = MyDialogues.CurrentSentence;

                m_dialogueActorNameControl.Clear();
                if (MyFakes.ENABLE_DEBUG_DIALOGS)
                {
                    m_dialogIdSB.Clear();
                    m_dialogIdSB.Append(MyDialogues.CurrentSentence.Text.ToString());
                    m_dialogueActorNameControl.AppendText(m_dialogIdSB,
                        MyDialogues.CurrentSentence.Font, MyHudConstants.DIALOGUE_ACTORNAME_FONT_SIZE, Vector4.One);
                }
                else 
                {
                    m_dialogueActorNameControl.AppendText(MyTextsWrapper.Get(MyActorConstants.GetActorProperties(MyDialogues.CurrentSentence.Actor).DisplayName),
                        MyDialogues.CurrentSentence.Font, MyHudConstants.DIALOGUE_ACTORNAME_FONT_SIZE, Vector4.One);
                }
                m_dialogueTextAreaControl.Clear();                                
                m_dialogueTextAreaControl.AppendText(MyDialoguesWrapper.Get(MyDialogues.CurrentSentence.Text),
                    MyDialogues.CurrentSentence.Font, MyHudConstants.DIALOGUE_ACTORNAME_FONT_SIZE, Vector4.One);
            }

            float noise = MyDialogues.CurrentSentence.Noise;

            // draw actor texture
            if (MyActorConstants.GetActorProperties(MyDialogues.CurrentSentence.Actor).AvatarImage != null)
            {
                MyGuiManager.DrawSpriteBatch(MyActorConstants.GetActorProperties(MyDialogues.CurrentSentence.Actor).AvatarImage, MyHudConstants.DIALOGUE_ACTORTEXTURE_POSITION + offset,
                    MyHudConstants.DIALOGUE_ACTORTEXTURE_SIZE, Color.White, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyVideoModeManager.IsTripleHead());
            }
            else
            {
                noise = 1;
            }

            // draw noise
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetDialoguePortraitNoiseVideoPlayer().GetCurrentFrame(), MyHudConstants.DIALOGUE_ACTORTEXTURE_POSITION + offset,
                MyHudConstants.DIALOGUE_ACTORTEXTURE_SIZE, new Color(noise * Vector4.One), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyVideoModeManager.IsTripleHead());
            // draw background
            MyGuiManager.DrawSpriteBatch(MyDialogues.CurrentSentence.BackgroundTexture, MyHudConstants.DIALOGUE_BACKGROUND_POSITION + offset,
                MyHudConstants.DIALOGUE_BACKGROUND_SIZE, Color.White, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyVideoModeManager.IsTripleHead());

            // draw text area
            m_dialogueTextAreaControl.SetPosition(MyHudConstants.DIALOGUE_TEXTAREA_POSITION + offsetTextArea);
            m_dialogueTextAreaControl.Draw();

            // draw actor name
            m_dialogueActorNameControl.SetPosition(MyHudConstants.DIALOGUE_ACTORNAME_POSITION + offsetTextArea);
            m_dialogueActorNameControl.Draw();

            MyGuiManager.EndSpriteBatch();
        }

        static MyHudSetting m_oreHudSetting;
        static MyHudEntityParams m_oreHudParams;
        static MyVoxelMapOreDepositCell[] m_nearestOreDeposits = new MyVoxelMapOreDepositCell[MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelMaterialsEnum>() + 1];
        static float[] m_nearestDistanceSquared = new float[MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelMaterialsEnum>() + 1];        

        private static void AddOreMarkers() 
        {
            for (int i = 0; i < m_nearestOreDeposits.Length; i++) 
            {
                m_nearestOreDeposits[i] = null;
                m_nearestDistanceSquared[i] = float.MaxValue;
            }

            //MyRadar.GetDetectedObjects(ref m_detectedObjects);            

            using (var detectedObjects = MyRadar.DetectedObjects)
            {
                foreach (IMyObjectToDetect detectedObject in detectedObjects.Collection)
                {
                    if (detectedObject is MyVoxelMapOreDepositCell)
                    {
                        MyVoxelMapOreDepositCell oreDeposit = detectedObject as MyVoxelMapOreDepositCell;
                        Debug.Assert(oreDeposit.GetTotalRareOreContent() > 0);
                        // we must check content, because Radar is running in background thread and content change in main thread
                        //oreDeposit.SortByContent();
                        foreach (MyMwcVoxelMaterialsEnum oreMaterial in oreDeposit.GetOreWithContent())
                        {
                            var orePosition = oreDeposit.GetPosition(oreMaterial);
                            if (orePosition == null) continue;
                            float distanceSquared = Vector3.DistanceSquared(orePosition.Value, MyGuiScreenGamePlay.Static.ControlledEntity.WorldMatrix.Translation);
                            float nearestDistanceSquared = m_nearestDistanceSquared[(int)oreMaterial];
                            if (distanceSquared < nearestDistanceSquared)
                            {
                                m_nearestOreDeposits[(int)oreMaterial] = oreDeposit;
                                m_nearestDistanceSquared[(int)oreMaterial] = distanceSquared;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < m_nearestOreDeposits.Length; i++) 
            {
                MyVoxelMapOreDepositCell nearestOreDeposit = m_nearestOreDeposits[i];
                if (nearestOreDeposit != null) 
                {
                    MyMwcObjectBuilder_Ore_TypesEnum oreType = MyVoxelMapOreMaterials.GetOreFromVoxelMaterial((MyMwcVoxelMaterialsEnum)i)[0].OreType;
                    MyGuiOreHelper oreHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, (int)oreType) as MyGuiOreHelper;                    
                    m_oreHudParams.Text = oreHelper.Name;
                    AddDirectionIndicator(m_oreHudSetting, nearestOreDeposit.GetPosition((MyMwcVoxelMaterialsEnum)i).Value, m_oreHudParams, 0f, 0f, Color.White);
                }
            }
        }

        /// <summary>
        /// Adds the direction indicators.
        /// </summary>
        private static void AddDirectionIndicators()
        {
            m_wasMothershipHudDisplayed = false;
            m_hudEnemies.Clear();
            if (ShowDebugHud)
            {
                float maxDistance = 299;
                int? showOnlyThisID = null;// 107955;
				HashSet<Type> showOnlyThisTypes = null;
                //HashSet<Type> showOnlyThisTypes = new HashSet<Type>();
                //showOnlyThisTypes.Add(typeof(MinerWars.AppCode.Game.Entities.CargoBox.MyCargoBox));                

                HashSet<MyEntity> entities = MyEntities.GetEntities();
                foreach (MyEntity entity in entities)
                {     /*
                    foreach (MyEntity ch in entity.Children)
                    {
                        if ((showOnlyThisType == null || (showOnlyThisType != null && showOnlyThisType == ch.GetType()))
                            &&
                            (showOnlyThisID == null || (showOnlyThisID != null && (showOnlyThisID == (ch.EntityId.HasValue ? ch.EntityId.Value.NumericValue : 0))))
                            )
                            ShowEntityInDebugHud(ch);
                    }   */

                    if(entity is MyStaticAsteroid || entity is MyWayPoint)
                    {
                        continue;
                    }
                    else if(entity is MyPrefabContainer)
                    {
                        if ((showOnlyThisTypes == null || (showOnlyThisTypes != null && showOnlyThisTypes.Contains(entity.GetType())) )
                            &&
                            (showOnlyThisID == null || (showOnlyThisID != null && (showOnlyThisID == (entity.EntityId.HasValue ? entity.EntityId.Value.NumericValue : 0))))
                            )
                            ShowEntityInDebugHud(entity);

                        foreach (var prefab in ((MyPrefabContainer)entity).GetPrefabs())
                        {
                            if (Vector3.Distance(MyCamera.Position, prefab.GetPosition()) < maxDistance)
                            {
                                if ((showOnlyThisTypes == null || (showOnlyThisTypes != null && showOnlyThisTypes.Contains(prefab.GetType())))
                                                                &&
                            (showOnlyThisID == null || (showOnlyThisID != null && (showOnlyThisID == (prefab.EntityId.HasValue ? prefab.EntityId.Value.NumericValue : 0)))))

                                    ShowEntityInDebugHud(prefab);
                            }
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(MyCamera.Position, entity.GetPosition()) < maxDistance)
                        {
                            if ((showOnlyThisTypes == null || (showOnlyThisTypes != null && showOnlyThisTypes.Contains(entity.GetType())))
                                                            &&
                            (showOnlyThisID == null || (showOnlyThisID != null && (showOnlyThisID == (entity.EntityId.HasValue ? entity.EntityId.Value.NumericValue : 0)))))

                                ShowEntityInDebugHud(entity);
                        }
                    }
                }
            }

            if (MyGuiManager.GetScreenDebugBot() != null)
            {
                HashSet<MyEntity> entities = MyEntities.GetEntities();
                foreach (MyEntity entity in entities)
                {
                    if (entity is MySmallShipBot && MyGuiManager.GetScreenDebugBot() != null)
                    {
                        ShowBotInDebug(entity as MySmallShipBot);
                    }
                }                
            }

            MyRender.GetRenderProfiler().StartProfilingBlock("foreach HudEntities");

            using (var detectedObjects = MyRadar.DetectedObjects)
            {
                foreach (var hudEntity in HudEntities)
                {
                    var entity = hudEntity.Item1.Target;
                    MyHudEntityParams hudParams = hudEntity.Item2;

                    if (entity == null || entity == MyGuiScreenGamePlay.Static.ControlledLargeWeapon || entity is MyVoxelMapOreDepositCell)
                    {
                        continue;
                    }

                    if (entity is MyEntity && !ShowDebugHud && MyGuiManager.GetScreenDebugBot() == null)
                    {
                        MyEntity entityObject = entity as MyEntity;
                        if (entityObject.Name == MyMotherShipPosition.MADELYN_NAME)
                        {
                            m_wasMothershipHudDisplayed = true;
                        }

                        if (!((MyEntity)entity).IsVisible())
                        {
                            continue;
                        }

                        /*
                        // we don't want display empty hud description
                        if(hudParams.Text == null || hudParams.Text.Length == 0)
                        {
                            continue;
                        } */

                       // MyRender.GetRenderProfiler().StartProfilingBlock("displayEntity");

                        bool displayEntity = false;
                        if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR) != 0)
                        {
                            //MyRadar.GetDetectedObjects(ref m_detectedObjects);
                            /*
                            foreach (IMyObjectToDetect detectedObject in detectedObjects.Collection)
                            {
                                if (detectedObject == entity)
                                {
                                    displayEntity = true;
                                    break;
                                }
                            } */

                            if (detectedObjects.Collection.Contains((IMyObjectToDetect)entity))
                            {
                                displayEntity = true;
                            }
                        }
                        else
                        {
                            displayEntity = true;
                        }

                        //MyRender.GetRenderProfiler().EndProfilingBlock();
                        //MyRender.GetRenderProfiler().StartProfilingBlock("AddDirectionIndicator");

                        if (displayEntity)
                        {
                            AddDirectionIndicator((MyEntity)entity, hudParams);
                        }

                        //MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }
            }
            MyRender.GetRenderProfiler().EndProfilingBlock();

            if (!m_wasMothershipHudDisplayed) 
            {
                AddMadelyn();
            }

            // Add Sun HUD            
            if (MySunWind.IsActiveForHudWarning())
            {
                AddSun();
            }

#if DEBUG
            if (MyMwcFinalBuildConstants.DrawCollisionSpotsInHud)
            {
                foreach (var hudDebugPoint in HudDebugPoints)
                {
                    var debugPoint = hudDebugPoint.Item1;
                    var debugHudParams = hudDebugPoint.Item2;

                    if (debugPoint == null)
                    {
                        continue;
                    }

                    AddDirectionIndicator(m_hudSettings[(int)MyGuitargetMode.Neutral], (Vector3)debugPoint, (MyHudEntityParams)debugHudParams, -1, -1, Color.White);
                }
            }
            else
            {
                if (HudDebugPoints.Count > 0)
                {
                    HudDebugPoints.Clear();
                }
            }
#endif
        }

        private static void AddSun() 
        {
            if (m_sunHudIconColorAlphaIncreasing)
            {
                m_sunHudIconColorAlpha += SUN_BLINK_ALPHA_INCREMENT;
                if (m_sunHudIconColorAlpha >= 1f)
                {
                    m_sunHudIconColorAlpha = 1f;
                    m_sunHudIconColorAlphaIncreasing = false;
                }
            }
            else
            {
                m_sunHudIconColorAlpha -= SUN_BLINK_ALPHA_INCREMENT;
                if (m_sunHudIconColorAlpha <= 0f)
                {
                    m_sunHudIconColorAlpha = 0f;
                    m_sunHudIconColorAlphaIncreasing = true;
                }
            }

            Vector3 sunPos = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized() * MyMwcSectorConstants.SECTOR_SIZE * 99;
            AddDirectionIndicator(m_hudSettings[(int)MyGuitargetMode.Neutral], sunPos, m_sunHudParams, 0f, 0f, Color.Yellow, showNameInWithoutOffset: true, drawFocusMark: false, alphaMultiplifier: m_sunHudIconColorAlpha);
        }

        private static void AddMadelyn() 
        {
            if (MySession.Static.MotherShipPosition.Exist && MySession.Static.MotherShipPosition.IsNotInSameSector())
            {                
                AddDirectionIndicator(m_hudSettings[(int)MyGuitargetMode.Objective], MySession.Static.MotherShipPosition.HudPosition, m_madelynHudParams, 0f, 0f, Color.White);
            }
        }

        private static void ShowBotInDebug(MySmallShipBot bot)
        {
            MyHudEntityParams hudParams = new MyHudEntityParams();
            hudParams.DisplayFactionRelation = new MyHudDisplayFactionRelation(true, true, true);

            MyMwcUtils.ClearStringBuilder(m_hudDebugText);
            if (!string.IsNullOrEmpty(bot.DisplayName))
            {
                m_hudDebugText.AppendLine(bot.DisplayName);
            }

            if (MyGuiManager.GetScreenDebugBot() != null)
            {
                MyMwcUtils.AppendStringBuilder(m_hudDebugText, bot.GetDebugHudString());
            }

            hudParams.Text = m_hudDebugText;
            hudParams.MaxDistance = 0f;
            hudParams.FlagsEnum = MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER | MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER;

            AddDirectionIndicator(bot, hudParams);
            
        }

        private static void ShowEntityInDebugHud(MyEntity entity)
        {
            MyHudEntityParams hudParams = new MyHudEntityParams();
            hudParams.DisplayFactionRelation = new MyHudDisplayFactionRelation(true, true, true);

            MyMwcUtils.ClearStringBuilder(m_hudDebugText);
            if (!string.IsNullOrEmpty(entity.DisplayName))
            {
                m_hudDebugText.AppendLine(entity.DisplayName);
            }
            if (entity.EntityId.HasValue)
            {
                m_hudDebugText.Append("ID: " + entity.EntityId.Value.NumericValue.ToString() + ", ");
            }
            if (entity.IsDestructible)
            {
                m_hudDebugText.AppendDecimal(entity.Health, 0);
                m_hudDebugText.Append(m_hudHealthSeparator);
                m_hudDebugText.AppendDecimal(entity.MaxHealth, 0);
                if(entity is MySmallShip)
                {
                    MySmallShip smallShip = entity as MySmallShip;
                    MyMwcUtils.AppendStringBuilder(m_hudDebugText, m_hudHealthAndArmorSeparator);
                    m_hudDebugText.AppendDecimal(smallShip.ArmorHealth, 0);
                    m_hudDebugText.Append(m_hudHealthSeparator);
                    m_hudDebugText.AppendDecimal(smallShip.MaxArmorHealth, 0);
                    MyMwcUtils.AppendStringBuilder(m_hudDebugText, new StringBuilder(smallShip.Faction.ToString()));
                }
            }
            else
            {
                MyMwcUtils.AppendStringBuilder(m_hudDebugText, m_textIndestructible);
            }
            m_hudDebugText.AppendLine();
            switch(entity.MaxDifficultyToActivated)
            {
                case MyGameplayDifficultyEnum.EASY:
                    MyMwcUtils.AppendStringBuilder(m_hudDebugText, MyTextsWrapper.Get(MyTextsWrapperEnum.DifficultyEasy));
                    break;
                case MyGameplayDifficultyEnum.NORMAL:
                    MyMwcUtils.AppendStringBuilder(m_hudDebugText, MyTextsWrapper.Get(MyTextsWrapperEnum.DifficultyNormal));
                    break;
                case MyGameplayDifficultyEnum.HARD:
                    MyMwcUtils.AppendStringBuilder(m_hudDebugText, MyTextsWrapper.Get(MyTextsWrapperEnum.DifficultyHard));
                    break;
            }

            m_hudDebugText.AppendLine();

            if (!entity.Enabled)
            {
                 m_hudDebugText.AppendLine(" Disabled");
            }

            if (entity is MyPrefabBase)
            {
                MyPrefabBase prefab = entity as MyPrefabBase;
                m_hudDebugText.AppendLine("Electricity " + prefab.ElectricCapacity.ToString("#,###0.000") + " (" + prefab.GetConfiguration().MinElectricCapacity.ToString("#,###0.000") + "/" + prefab.GetConfiguration().MaxElectricCapacity.ToString("#,###0.000"));
                m_hudDebugText.AppendLine("Faction " + prefab.Faction.ToString());
            }
                


            hudParams.Text = m_hudDebugText;
            hudParams.MaxDistance = 0f;
            hudParams.FlagsEnum = MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER | MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER;

            AddDirectionIndicator(entity, hudParams);            
        }

        public static void ShowIndestructableAsteroidNotification()
        {
            if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_lastIdestructibleAsteroidWarningShowTime + 2050)
            {
                m_lastIdestructibleAsteroidWarningShowTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                MyHudNotification.AddNotification(new MyHudNotification.MyNotification(Localization.MyTextsWrapperEnum.IndestructibleAsteroidNotification, 2000));
            }
        }

        private static float m_lastIdestructibleAsteroidWarningShowTime = 0;

        public static void DrawCameraCrosshair()
        {
            MyCamera.EnableHud();
            ClearQuads();
            AddCrosshair();
            DrawVertices();
        }

        public static bool ContainsEntity(MyEntity entity)
        {
            foreach (Tuple<WeakReference, MyHudEntityParams> tuple in HudEntities)
            {
                if (tuple.Item1.Target == entity) return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the text that is displayed per entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="text">The text to display</param>
        public static void AddText(MyEntity entity, StringBuilder text, MyGuitargetMode? targetMode = null, float maxDistance = 0, MyHudIndicatorFlagsEnum flagsEnum = MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER | MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER, MyHudDisplayFactionRelation? displayFactinRelation = null, MyHudMaxDistanceMultiplerTypes? maxDistanceMultiplerType = null)
        {
            //Why assert?
            //Debug.Assert(text.Length != 0);
            HudEntities.Add(new Tuple<WeakReference, MyHudEntityParams>(new WeakReference(entity), new MyHudEntityParams(text, targetMode, maxDistance, flagsEnum, displayFactinRelation, maxDistanceMultiplerType)));
        }

        [Conditional("DEBUG")]
        public static void DebugClearAndAddText(Vector3 point, StringBuilder text, float maxDistance = 0, MyHudIndicatorFlagsEnum flagsEnum = MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER | MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER, MyHudDisplayFactionRelation? displayFactinRelation = null)
        {
            Debug.Assert(text.Length != 0);
            HudDebugPoints.Clear();
            HudDebugPoints.Add(new Tuple<Vector3, MyHudEntityParams>(point, new MyHudEntityParams(text, MyGuitargetMode.Enemy, maxDistance, flagsEnum, displayFactinRelation)));
        }

        /// <summary>
        /// Removes all text that is displayed per specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static void RemoveText(MyEntity entity)
        {
            int index = -1;

            for (int i = 0; i < HudEntities.Count; i++)
            {
                var pair = HudEntities[i];
                if (pair.Item1.Target == entity)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                HudEntities.RemoveAtFast(index);
            }
        }

        //  Change the text that is displayed per entity - if entity is not included in HudEntities list, add it.
        public static void ChangeText(MyEntity entity, StringBuilder text)
        {
            ChangeText(entity, text, null);
        }

        public static void ChangeText(MyEntity entity, StringBuilder text, MyGuitargetMode? targetMode, float maxDistance = 0, MyHudIndicatorFlagsEnum flagsEnum = DEFAULT_FLAGS, MyHudDisplayFactionRelation? displayFactinRelation = null, MyHudMaxDistanceMultiplerTypes? maxDistanceMultiplerType = null)
        {
            RemoveText(entity);

            AddText(entity, text, targetMode, maxDistance, flagsEnum, displayFactinRelation, maxDistanceMultiplerType);
        }

        public static void RenameHudEntity(MyEntity entity, StringBuilder newText)
        {
            int index = HudEntities.FindIndex(t => t.Item1.Target == entity);

            Debug.Assert(index != -1);
            
            MyHudEntityParams hudParams = HudEntities[index].Item2;
            RemoveText(entity);
            AddText(entity, newText, hudParams.TargetMode, hudParams.MaxDistance, hudParams.FlagsEnum, hudParams.DisplayFactionRelation, hudParams.MaxDistanceMultiplerType);
        }

        public static MyHudEntityParams? GetHudParams(MyEntity entity) 
        {
            MyHudEntityParams? hudParams = null;
            int index = HudEntities.FindIndex(t => t.Item1.Target == entity);

            // not found
            if (index != -1)
            {
                hudParams = HudEntities[index].Item2;
            }
            return hudParams;
        }

        //  Draw HUD texts. This list is deleted at the beginning of every MyHud.Draw() call
        static void DrawTexts()
        {
            if (m_texts.GetAllocatedCount() <= 0) return;

            //  MyHud.Draw is always called from within game-play-screen Draw, so that's while the main sprite batch is not active (not in draw/end) state,
            //  so we have to restart here for a while
            MyGuiManager.BeginSpriteBatch();

            for (int i = 0; i < m_texts.GetAllocatedCount(); i++)
            {
                MyHudText text = m_texts.GetAllocatedItem(i);

                //  Fix the scale for screen resolution
                float fixedScale = text.Scale * MyGuiManager.GetSafeScreenScale();
                Vector2 sizeInPixelsScaled = text.Font.MeasureString(text.GetStringBuilder(), fixedScale);

                Vector2 screenCoord = MyGuiManager.GetHudPixelCoordFromNormalizedCoord(text.Position / MyGuiManager.GetHudSize());
                screenCoord = MyGuiManager.GetAlignedCoordinate(screenCoord, sizeInPixelsScaled, text.Alignement);

                text.Font.DrawString(screenCoord, text.Color, text.GetStringBuilder(), fixedScale/*, text.Rotation*/);
            }

            MyGuiManager.EndSpriteBatch();
        }

        private static bool CanDisplayLockedTarget() 
        {
            return MyGuiScreenGamePlay.Static.ControlledShip != null ||
                   MyGuiScreenGamePlay.Static.ControlledLargeWeapon != null && MyGuiScreenGamePlay.Static.ControlledLargeWeapon.IsGuided();
        }

        //  Adds direction indicators (arrows) and their corresponding text with distance to object
        //  Right now it displays only 1 object - because it's only for testing purpose
        private static void AddDirectionIndicator(MyEntity target, MyHudEntityParams hudParams)
        {
            if ((target == null) || target == MySession.PlayerShip)
                return;

            //MyRender.GetRenderProfiler().StartProfilingBlock("CanDisplayLockedTarget");

            MyEntity targetEntity = MySession.PlayerShip.TargetEntity;
            bool isLockedSideTarget = false;
            bool isLockedTarget = false;
            if (CanDisplayLockedTarget())
            {
                isLockedSideTarget = MySession.PlayerShip.SideTargets.Contains(target);
                isLockedTarget = targetEntity == target;
            }

           // MyRender.GetRenderProfiler().StartNextBlock("GetGuiTargetMode");

            hudParams.FlagsEnum &= ~MyHudIndicatorFlagsEnum.SHOW_LOCKED_TARGET;
            hudParams.FlagsEnum &= ~MyHudIndicatorFlagsEnum.SHOW_LOCKED_SIDE_TARGET;
            if (isLockedTarget)
            {
                hudParams.FlagsEnum |= MyHudIndicatorFlagsEnum.SHOW_LOCKED_TARGET;
            }
            else
                if (isLockedSideTarget)
                {
                    hudParams.FlagsEnum |= MyHudIndicatorFlagsEnum.SHOW_LOCKED_SIDE_TARGET;
                }

            float targetDamageRatio = -1.0f;
            float targetArmorRatio = -1.0f;
            bool addToHudEnemies = false;
            float alphaMultiplifier = 1f;
            MyHudSetting hudSettings = null;
            MyHudAlphaCorrection hudAlphaCorrection = null;

            MyGuitargetMode hudSettingsTargetMode;
            MyGuitargetMode alphaCorrectionTargetMode;
            GetGuiTargetMode(target, hudParams, out hudSettingsTargetMode, out alphaCorrectionTargetMode);
            MyFactionRelationEnum status = MyFactions.GetFactionsRelation(target, MySession.PlayerShip);

            //MyRender.GetRenderProfiler().StartNextBlock("IsMissionEntity");

            if (MyMissions.IsMissionEntity(target))
            {
                if (MyMissions.IsNewMissionOrObjectiveAvailable())
                {
                    alphaMultiplifier *= m_missionHighlight;
                }
            }
            else
            {
                // some entities can have displaying dependents on faction relation
                if (!hudParams.DisplayFactionRelation.CanBeDisplayed(status))
                {
                    //MyRender.GetRenderProfiler().EndProfilingBlock();
                    return;
                }                               
            }

           // MyRender.GetRenderProfiler().StartNextBlock("GetHUDDamageRatio");

            hudSettings = m_hudSettings[(int)hudSettingsTargetMode];
            hudAlphaCorrection = m_hudAlphaCorrection[(int)alphaCorrectionTargetMode];

            if (status == MyFactionRelationEnum.Enemy)
            {
                addToHudEnemies = MyMissile.CanBeTargeted(target);
                if (target is MySmallShip) 
                {
                    hudSettings = m_hudSettings[(int)MyGuitargetMode.Enemy];
                }
            }
            
            targetDamageRatio = target.GetHUDDamageRatio();
            MySmallShip targetShip = target as MySmallShip;
            if (targetShip != null)
            {
                if (targetShip.Armor != null)
                {
                    targetArmorRatio = MathHelper.Clamp(targetShip.ArmorHealth / targetShip.MaxArmorHealth, 0, 1);
                }
            }

            //// when target is hangar, we want display healthbar of large ship from his prefab container
            //MyPrefabHangar targetHangar = target as MyPrefabHangar;
            //if(targetHangar != null)
            //{
            //    List<MyPrefabBase> largeShips = targetHangar.GetOwner().GetPrefabs(CategoryTypesEnum.LARGE_SHIPS);
            //    if(largeShips.Count > 0)
            //    {
            //        targetDamageRatio = largeShips[0].GetDamageRatio();
            //    }
            //}

            //MyRender.GetRenderProfiler().StartNextBlock("IsElectrified");

            bool displayUnpowered = false;
            MyPrefabBase targetPrefab = target as MyPrefabBase;
            if (targetPrefab != null) 
            {
                displayUnpowered = !targetPrefab.IsElectrified();
            }
            
            if (isLockedTarget)
            {
                hudSettings = m_hudSettings[(int)MyGuitargetMode.Enemy];                
            }

            //MyRender.GetRenderProfiler().StartNextBlock("GetHUDMarkerPosition");

            // Show in center of objects (pivots are not centered atm.)
            Vector3 position = target.GetHUDMarkerPosition();

            //MyRender.GetRenderProfiler().StartNextBlock("AddDirectionIndicator");

            float? distanceFromScreenCenter = AddDirectionIndicator(hudSettings, position, hudParams, targetDamageRatio, targetArmorRatio, Color.White, hudAlphaCorrection, displayUnpowered, alphaMultiplifier);

            if (addToHudEnemies && distanceFromScreenCenter != null) 
            {
                m_hudEnemies.Add(target, distanceFromScreenCenter.Value);
            }

           // MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private static void GetGuiTargetMode(MyEntity target, MyHudEntityParams hudParams, out MyGuitargetMode hudSettingsTargetMode, out MyGuitargetMode alphaCorrectionTargetMode) 
        {
            hudSettingsTargetMode = hudParams.TargetMode != null ? hudParams.TargetMode.Value : MyGuitargetMode.Neutral;
            alphaCorrectionTargetMode = MyGuitargetMode.Neutral;

            MyFactionRelationEnum status = MyFactions.GetFactionsRelation(target, MySession.PlayerShip);
            if (MyMissions.IsMissionEntity(target))
            {                
                var mission = MyMissions.GetSubmissionByEntity(target);
                if (mission != null && mission.ShowAsOptional)
                {
                    if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER) != 0)
                    {
                        hudSettingsTargetMode = MyGuitargetMode.ObjectiveOptional;
                    }
                    alphaCorrectionTargetMode = MyGuitargetMode.ObjectiveOptional;
                }
                else
                {
                    if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER) != 0)
                    {
                        hudSettingsTargetMode = MyGuitargetMode.Objective;
                    }
                    alphaCorrectionTargetMode = MyGuitargetMode.Objective;
                }                
            }
            else
            {                
                switch (status)
                {
                    case MyFactionRelationEnum.Neutral:
                        if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER) != 0)
                        {
                            hudSettingsTargetMode = MyGuitargetMode.Neutral;
                        }
                        alphaCorrectionTargetMode = MyGuitargetMode.Neutral;
                        break;
                    case MyFactionRelationEnum.Friend:
                        if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER) != 0)
                        {
                            hudSettingsTargetMode = MyGuitargetMode.Friend;
                        }
                        alphaCorrectionTargetMode = MyGuitargetMode.Friend;
                        break;
                    case MyFactionRelationEnum.Enemy:
                        if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER) != 0)
                        {
                            hudSettingsTargetMode = MyGuitargetMode.Enemy;
                        }
                        alphaCorrectionTargetMode = MyGuitargetMode.Enemy;
                        break;
                }
            }
        }

        public static float[] HudMaxDistanceMultiplers;
        public const int MAX_DAMAGE_INDICATORS = 5;
        public const int DAMAGE_INDICATOR_TIMEOUT = 1 * 1000;
        public static DamageIndicator[] DamageIndicators;

        public class DamageIndicator
        {
            public bool Used;

            public MyEntity Entity;
            public Vector3 Position;
            public float Damage;
            public int Time;
        }

        public static void SetDamageIndicator(MyEntity source, Vector3 position, float damage)
        {
            int index = 0;
            float time = float.MaxValue;
            for (int i = 0; i < DamageIndicators.Length; i++)
            {
                if (!DamageIndicators[i].Used)
                {
                    index = i;
                    break;
                }
                else if (DamageIndicators[i].Entity == source)
                {
                    index = i;
                    break;
                }
                else
                {
                    if (DamageIndicators[i].Time < time)
                    {
                        time = DamageIndicators[i].Time;
                        index = i;
                    }
                }
            }

            DamageIndicators[index].Used = true;
            DamageIndicators[index].Entity = source;
            DamageIndicators[index].Position = position;
            DamageIndicators[index].Damage = damage;
            DamageIndicators[index].Time = DAMAGE_INDICATOR_TIMEOUT;
        }

        private static void AddDamageIndicators()
        {
            for (int i = 0; i < DamageIndicators.Length; i++)
            {
                DamageIndicators[i].Time -= (int)(MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS / (DamageIndicators[i].Time / (float)(DAMAGE_INDICATOR_TIMEOUT)));
                if (DamageIndicators[i].Time < 0)
                {
                    DamageIndicators[i].Used = false;
                }
            }

            for (int i = 0; i < DamageIndicators.Length; i++)
            {
                if (DamageIndicators[i].Used)
                {
                    AddDamageIndicator(DamageIndicators[i].Position, DamageIndicators[i].Time / (float)(DAMAGE_INDICATOR_TIMEOUT));
                }
            }
        }

        private static void AddDamageIndicator(Vector3 position, float alpha)
        {
            Vector3 transformedPoint = Vector3.Transform(position, MyCamera.ViewMatrix);
            Vector4 projectedPoint = Vector4.Transform(transformedPoint, MyCamera.ProjectionMatrix);

            if (transformedPoint.Z > 0)
            {
                projectedPoint.X *= -1;
                projectedPoint.Y *= -1;
            }

            Vector2 projectedPoint2D = new Vector2(projectedPoint.X / projectedPoint.W / 2.0f + 0.5f, -projectedPoint.Y / projectedPoint.W / 2.0f + 0.5f);
            if (MyVideoModeManager.IsTripleHead())
            {
                projectedPoint2D.X = (projectedPoint2D.X - (1.0f / 3.0f)) / (1.0f / 3.0f);
            }

            Vector2 direction = projectedPoint2D - MyHudConstants.DIRECTION_INDICATOR_SCREEN_CENTER;
            
            if (direction.LengthSquared() > MyMwcMathConstants.EPSILON_SQUARED)
                direction = MyMwcUtils.Normalize(direction);
            else
                direction = new Vector2(1f, 0f);

            projectedPoint2D = MyHudConstants.DIRECTION_INDICATOR_SCREEN_CENTER + direction * 0.03f;
            projectedPoint2D.Y *= MyGuiManager.GetHudSize().Y;

            AddTexturedQuad(MyHudTexturesEnum.damage_direction, projectedPoint2D + direction * MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 2.5f, direction,
                    new Color(1, 1, 1, alpha), 0.006667f * 3.6f, 0.006667f * 1.6f);
        }

        /// <summary>
        /// Adds direction indicator and returns distance from screen center
        /// </summary>        
        /// <returns></returns>
        private static float? AddDirectionIndicator(MyHudSetting hudSetting, Vector3 position, MyHudEntityParams hudParams, float targetDamageRatio, float targetArmorRatio, Color hudColor, MyHudAlphaCorrection alphaCorrection = null, bool displayUnpowered = false, float alphaMultiplifier = 1f, bool showNameInWithoutOffset = false, bool drawFocusMark = true)
        {
            float? distanceFromScreenCenter = null;
            float distance = Vector3.Distance(position, MyGuiScreenGamePlay.Static.ControlledEntity.WorldMatrix.Translation);
            if (showNameInWithoutOffset && distance > 100 * MyMwcSectorConstants.SECTOR_SIZE)
                return null;
            bool isAlphaCorrectionEnabled = alphaCorrection != null && (hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE) != 0;

            float maxDistance = hudParams.MaxDistance;
            if (hudParams.MaxDistanceMultiplerType != null)
            {
                maxDistance *= HudMaxDistanceMultiplers[(int)hudParams.MaxDistanceMultiplerType];
            }
            if (isAlphaCorrectionEnabled && distance >= alphaCorrection.MaxDistance || (maxDistance > 0) && (distance > maxDistance) || distance < 1f || Vector3.DistanceSquared(position, MyCamera.Position) <= 1)
                return distanceFromScreenCenter;
            
            byte colorAlphaInByte;
            if (isAlphaCorrectionEnabled)
            {
                colorAlphaInByte = alphaCorrection.GetAlphaByDistance(distance);
            }
            else
            {
                colorAlphaInByte = 255;// hudParams.Color.A;
            }
            
            hudColor.A = (byte)(colorAlphaInByte * alphaMultiplifier);

            //  Transform point to camera space, so Z = -1 is always forward and then do projective transformation
            Vector3 transformedPoint = Vector3.Transform(position, MyCamera.ViewMatrix);
            Vector4 projectedPoint = Vector4.Transform(transformedPoint, MyCamera.ProjectionMatrix);

            //  If point is behind camera we swap X and Y (this is mirror-like transformation)
            if (transformedPoint.Z > 0)
            {
                projectedPoint.X *= -1;
                projectedPoint.Y *= -1;
            }

            if (projectedPoint.W == 0)
            {
                return null;
            }

            //  Calculate centered coordinates in range <0..1>
            Vector2 projectedPoint2D = new Vector2(projectedPoint.X / projectedPoint.W / 2.0f + 0.5f, -projectedPoint.Y / projectedPoint.W / 2.0f + 0.5f);
            if (MyVideoModeManager.IsTripleHead())
            {
                projectedPoint2D.X = (projectedPoint2D.X - (1.0f / 3.0f)) / (1.0f / 3.0f);
            }

            float objectNameYOffset = 0.0f; //offset to direction indicator

            //  This will bound the rectangle in circle, although it isn't real circle because we work in [1,1] dimensions, 
            //  but number of horizontal pixels is bigger, so at the end it's more elypse
            //  It must be done when point is out of circle or behind the camera
            Vector2 direction = projectedPoint2D - MyHudConstants.DIRECTION_INDICATOR_SCREEN_CENTER;            
            if ((direction.Length() > MyHudConstants.DIRECTION_INDICATOR_MAX_SCREEN_DISTANCE) || (transformedPoint.Z > 0))
            {
                distanceFromScreenCenter = direction.LengthSquared() + MyHudConstants.DIRECTION_INDICATOR_MAX_SCREEN_DISTANCE;
                if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS) == 0)    // this code doesn't allocate anything
                {
                    return distanceFromScreenCenter;
                }

                if (direction.LengthSquared() > MyMwcMathConstants.EPSILON_SQUARED)
                {
                    direction = MyMwcUtils.Normalize(direction);
                }
                else 
                {
                    direction = new Vector2(1f, 0f);
                }
                projectedPoint2D = MyHudConstants.DIRECTION_INDICATOR_SCREEN_CENTER + direction * MyHudConstants.DIRECTION_INDICATOR_MAX_SCREEN_DISTANCE;

                //  Fix vertical scale
                projectedPoint2D.Y *= MyGuiManager.GetHudSize().Y;

                AddTexturedQuad(hudSetting.TextureDirectionIndicator, projectedPoint2D + direction * MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 2.5f, direction,
                       hudColor, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.2f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 0.8f);

                if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_LOCKED_TARGET) > 0)
                {
                    AddTexturedQuad(hudSetting.TextureDirectionIndicator, projectedPoint2D + direction * MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 2.0f, direction,
                           hudColor, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.2f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 0.8f);
                    AddTexturedQuad(hudSetting.TextureDirectionIndicator, projectedPoint2D + direction * MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, direction,
                           hudColor, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.2f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 0.8f);
                }
                else
                    if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_LOCKED_SIDE_TARGET) > 0)
                {
                    AddTexturedQuad(hudSetting.TextureDirectionIndicator, projectedPoint2D + direction * MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 2.0f, direction,
                           hudColor, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.2f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 0.8f);
                    AddTexturedQuad(hudSetting.TextureDirectionIndicator, projectedPoint2D + direction * MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, direction,
                           hudColor, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.2f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 0.8f);
                }

            }
            else
            {
                //  Fix vertical scale
                projectedPoint2D.Y *= MyGuiManager.GetHudSize().Y;

                distanceFromScreenCenter = direction.LengthSquared();



                Color rectangleColor = Color.White;// hudParams.Color;
                rectangleColor.A = colorAlphaInByte;

                if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_LOCKED_TARGET) > 0)
                {
                    if (displayUnpowered)
                    {
                        //if (hudParams.DisplayFactionRelation.Enemy)
                        //    AddTexturedQuad(MyHudTexturesEnum.Unpowered_red, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                        //else if (hudParams.DisplayFactionRelation.Friend)
                        //    AddTexturedQuad(MyHudTexturesEnum.Unpowered_green, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                        //else if (hudParams.DisplayFactionRelation.Neutral)
                        //    AddTexturedQuad(MyHudTexturesEnum.Unpowered_white, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                        //else
                        //    AddTexturedQuad(MyHudTexturesEnum.Unpowered_blue, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                        AddTexturedQuad(hudSetting.TextureUnpowered, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                    }
                    else
                    {
                        AddTexturedQuad(MyHudTexturesEnum.Crosshair_locked, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE);
                    }
                }
                else
                    if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_LOCKED_SIDE_TARGET) > 0)
                    {
                        if (displayUnpowered)
                        {
                            //if (hudParams.DisplayFactionRelation.Enemy)
                            //    AddTexturedQuad(MyHudTexturesEnum.Unpowered_red, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                            //else if (hudParams.DisplayFactionRelation.Friend)
                            //    AddTexturedQuad(MyHudTexturesEnum.Unpowered_green, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                            //else if (hudParams.DisplayFactionRelation.Neutral)
                            //    AddTexturedQuad(MyHudTexturesEnum.Unpowered_white, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                            //else
                            //    AddTexturedQuad(MyHudTexturesEnum.Unpowered_blue, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                            AddTexturedQuad(hudSetting.TextureUnpowered, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                        }
                        else
                        {
                            AddTexturedQuad(MyHudTexturesEnum.Crosshair_side_locked, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE);
                        }                        
                    }
                    else
                    {
                        if (drawFocusMark)
                        {
                            //X = value.X * (int)Math.Cos(this.Rotation);
                            //Y = value.Y * (int)Math.Cos(this.Rotation);
                            Vector2 upVector = new Vector2(0, -1);
                            if (hudSetting.TextureTargetRotationSpeed != 0)
                            {
                                upVector = new Vector2((float)Math.Cos(MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000f * hudSetting.TextureTargetRotationSpeed * MathHelper.Pi),
                                                       (float)Math.Sin(MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000f * hudSetting.TextureTargetRotationSpeed * MathHelper.Pi));
                            }

                            if (displayUnpowered)
                            {
                                AddTexturedQuad(hudSetting.TextureUnpowered, projectedPoint2D, new Vector2(0, -1), Color.White, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * 1.5f);
                            }
                            else
                            {
                                AddTexturedQuad(hudSetting.TextureTarget, projectedPoint2D, upVector, hudColor, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * hudSetting.TextureTargetScale, MyHudConstants.HUD_DIRECTION_INDICATOR_SIZE * hudSetting.TextureTargetScale);
                            }
                        }
                    }

                //if (hudParams.FlagsEnum.IsFlagSet(MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS))
                if ((hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS) != 0)
                {   
                    if (targetDamageRatio > -1.0f && targetDamageRatio < 0.99f)
                    {
                        //color of halth bar is same as color of target icon !
                        AddLine2D(projectedPoint2D - new Vector2(MyHudConstants.HUD_TEXTS_OFFSET, MyHudConstants.HUD_TEXTS_OFFSET), projectedPoint2D + new Vector2(2.0f * (1.0f - targetDamageRatio) * MyHudConstants.HUD_TEXTS_OFFSET - MyHudConstants.HUD_TEXTS_OFFSET, -MyHudConstants.HUD_TEXTS_OFFSET), hudSetting.Color, hudSetting.Color, 0.006f);
                    }
                }

                objectNameYOffset = -MyHudConstants.HUD_TEXTS_OFFSET * 1.75f;
            }

            if (displayUnpowered) 
            {
                objectNameYOffset -= MyHudConstants.HUD_UNPOWERED_OFFSET;
            }
            
            //if (hudParams.Text != null && hudParams.Text.Length > 0 && (hudParams.FlagsEnum.IsFlagSet(MyHudIndicatorFlagsEnum.SHOW_TEXT)))
            if (hudParams.Text != null && hudParams.Text.Length > 0 && (hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_TEXT) != 0 && MyFakes.SHOW_HUD_NAMES)
            {
                //  Add object's name
                MyHudText objectName = m_texts.Allocate();
                if (objectName != null)
                {

                    objectName.Start(hudSetting.Font, projectedPoint2D + new Vector2(0, showNameInWithoutOffset ? 0 : objectNameYOffset),
                        hudColor, 0.8f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                    objectName.Append(hudParams.Text);

                    //if (target is MyPhysObjectBot)
                    //{
                    //    MyPhysObjectBot bot = (MyPhysObjectBot)target;
                    //    objectName.Append(bot.HudLabelText);
                    //}
                    //else
                    //{
                    //    objectName.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.Object));
                    //}
                }
            }

            // display hud icon
            if (hudParams.Icon != null && (hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_ICON) != 0) 
            {
                Color iconColor = hudParams.IconColor.Value;
                iconColor.A = (byte)(colorAlphaInByte * alphaMultiplifier);

                AddTexturedQuad(hudParams.Icon.Value, projectedPoint2D + hudParams.IconOffset, new Vector2(0, -1), iconColor, hudParams.IconSize.X / 2f, hudParams.IconSize.Y / 2f);
            }

            if (displayUnpowered) 
            {
                //  Add object's name
                MyHudText objectName = m_texts.Allocate();
                if (objectName != null)
                {

                    objectName.Start(hudSetting.Font, projectedPoint2D + new Vector2(0, objectNameYOffset + MyHudConstants.HUD_UNPOWERED_OFFSET),
                        hudColor, 0.8f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                    //objectName.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.HudUnpowered));                    
                }
            }

            if (MyFakes.SHOW_HUD_DISTANCES && (hudParams.FlagsEnum & MyHudIndicatorFlagsEnum.SHOW_DISTANCE) != 0)
            {
                //  Add distance to object
                MyHudText objectDistance = m_texts.Allocate();
                if (objectDistance != null)
                {
                    objectDistance.Start(hudSetting.Font, projectedPoint2D + new Vector2(0, MyHudConstants.HUD_TEXTS_OFFSET * 1.75f),
                        hudColor, 0.8f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

                    //  Create string builder with distance in metres, e.g. "123m"                    
                    objectDistance.AppendInt32((int)Math.Round(distance));
                    objectDistance.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.MetersShort));
                }
            }

            return distanceFromScreenCenter;
        }


        public static int LastGPS = Int32.MinValue;
        static bool m_GPSPathDirty = true;
        private static List<Vector3> m_GPSPath;
        private static StringBuilder m_GPSMessage;
        private static MySoundCue? m_GPSCue;

        private static MyComputeGPSJob m_computeGPSJob = new MyComputeGPSJob();
        private static Task m_computeGPSTask;
        private static bool m_computingGPS = false;
        
        private static int m_gpsStarted_ms;  // for debugging

        public static void MakeGPSPathDirty()
        {
            m_GPSPathDirty = true;
        }

        private static void ComputeGPSPath()
        {
            if (!m_computingGPS)
            {
                m_gpsStarted_ms = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                m_GPSPathDirty = false;
                m_GPSPath = new List<Vector3>();

                // move startPos a little bit below and to the front so that it's visible from the cockpit
                var startEntity = MyGuiScreenGamePlay.Static.ControlledEntity;
                Vector3 playerPos = startEntity.GetPosition();
                Vector3 startPos = playerPos + MyHudConstants.GPS_START_POSITION_FRONT * startEntity.GetWorldRotation().Forward + MyHudConstants.GPS_START_POSITION_UP * startEntity.GetWorldRotation().Up;

                // if startPos is blocked by some object, move it into a visible position
                var line = new MyLine(playerPos, playerPos + 2 * (startPos - playerPos), true);
                var intersection = MyEntities.GetIntersectionWithLine(ref line, startEntity, MySession.PlayerShip, true, true);
                if (intersection != null)
                    startPos = (playerPos + intersection.Value.IntersectionPointInWorldSpace) * 0.5f;  // halfway between the player and the intersection

                // find the goal position
                var goalEntity = GetGPSGoalEntity(startPos);

                Vector3? goalPos = goalEntity != null ? goalEntity.WorldAABB.GetCenter() : (Vector3?)null;

                if (goalPos == null)
                {
                    m_GPSMessage = MyTextsWrapper.Get(MyTextsWrapperEnum.GPSNoObjectives);
                    return;  // nothing found
                }
                
                m_computeGPSJob.Start(startPos, goalPos.Value, goalEntity);
                m_computeGPSTask = Parallel.Start(m_computeGPSJob);
                m_computingGPS = true;
            }
            else if (m_computeGPSTask.IsComplete)
            {
                m_computingGPS = false;
                LastGPS = MyMinerGame.TotalGamePlayTimeInMilliseconds;

                m_GPSPath = m_computeGPSJob.Path;
                m_GPSMessage = m_computeGPSJob.Message;
            }
            else
            {
                // commented because it was evil
                /*
                if (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_gpsStarted_ms > 1000)
                {
                    // one sec was enough, do it NOW
                    m_computeGPSJob.DoWork();
                    m_computingGPS = false;
                    LastGPS = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    m_GPSPath = m_computeGPSJob.Path;
                    m_GPSMessage = m_computeGPSJob.Message;
                }
                */
            }
        }

        public static MyEntity GetGPSGoalEntity(Vector3 startPos)
        {
            // try a submission location dummy
            MyEntity goalEntity = GetNextObjectiveDummy();

            // try the closest mission entity that's not optional
            if (goalEntity == null)
            {
                goalEntity = GetClosestMissionEntity(startPos);
            }

            return goalEntity;
        }

        private static MyEntity GetClosestMissionEntity(Vector3 position)
        {
            MyEntity goalEntity = null;

            //Vector3? closestActualEntityPos = null;

            foreach (var hudEntity in HudEntities)
            {
                var target = hudEntity.Item1.Target;

                if (target != null)
                {
                    var entity = target as MyEntity;

                    if (entity != null && MyMissions.IsMissionEntity(entity) &&
                        (MyMissions.GetSubmissionByEntity(entity) == null ||
                         !MyMissions.GetSubmissionByEntity(entity).ShowAsOptional))
                    {
                        return entity;
                        /*
                        var entityPosition = (entity).WorldAABB.GetCenter();
                        if (closestActualEntityPos == null ||
                            Vector3.Distance(position, entityPosition) <
                            Vector3.Distance(position, closestActualEntityPos.Value))
                        {
                            closestActualEntityPos = entityPosition;
                            goalEntity = entity;
                        }
                        */
                    }
                }
            }

            return goalEntity;
        }

        private static MyEntity GetNextObjectiveDummy()
        {
            if (MyMissions.ActiveMission != null && MyMissions.ActiveMission.ActiveObjectives != null)
                foreach (var submission in MyMissions.ActiveMission.ActiveObjectives)
                    if (submission.HasLocationEntity() && !submission.ShowAsOptional)
                    {
                        return submission.Location.Entity;
                    }

            return null;
        }

        private static MyHudNotification.MyNotification m_GPSNotification = null;

        public static void DrawGPS()
        {
            float timeSinceLastGPS = (float)MyMinerGame.TotalGamePlayTimeInMilliseconds - LastGPS;
            if (!m_computingGPS && timeSinceLastGPS > MyHudConstants.GPS_DURATION)
            {
                m_GPSPathDirty = true;
                return;
            }

            if ((m_GPSPathDirty || m_computingGPS) && MyMinerGame.IsGameReady)
            {
                ComputeGPSPath();
                if (m_computingGPS == true) return;  // still computing

                if (m_GPSNotification == null)
                    m_GPSNotification = new MyHudNotification.MyNotification("GPS{0}", MyHudConstants.MISSION_FONT, MyHudConstants.GPS_DURATION, null);
                m_GPSNotification.SetTextFormatArguments(new object[] { m_GPSMessage.ToString() });
                //m_GPSNotification.SetTextFormatArguments(new object[] { m_GPSMessage.ToString() + " (took " + (LastGPS - m_gpsStarted_ms) + "ms)" });

                MyHudNotification.AddNotification(m_GPSNotification);
                if (m_GPSCue != null && m_GPSCue.Value.IsPlaying)
                {
                    m_GPSCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
                if (m_GPSPath == null || m_GPSPath.Count == 0)
                {
                    m_GPSCue = MyAudio.AddCue2D(MySoundCuesEnum.SfxGpsFail);
                }
                else
                {
                    m_GPSCue = MyAudio.AddCue2D(MySoundCuesEnum.SfxGps);
                }
            }

            // draw all waypoints close to the player (for testing)
            /*
            foreach (var v in MyWayPointGraph.GetAllWaypointsInSphere(MySession.PlayerShip.GetPosition(), 1000))
            {
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.GPSBack, v.Save ? new Vector4(0.00f, 0.05f, 0.01f, 0.05f) : new Vector4(0.05f, 0.005f, 0.01f, 0.05f), v.Position, 20, 0);
                // draw edges
                foreach (var n in v.Neighbors)
                    for (float i = 0.0625f; i < 1; i += 0.0625f)
                        MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.GPSBack, new Vector4(0.001f, 0.001f, 0.02f, 0.05f), i * v.Position + (1 - i) * n.Position, 5, 0) ;
            }
            */

            if (m_GPSPath == null)
                return;

            if (m_GPSPath.Count < 2)
                return;

            float timeFadeOut = Math.Min(1, 1 - ((timeSinceLastGPS - (MyHudConstants.GPS_DURATION - MyHudConstants.GPS_FADE_OUT_DURATION)) / MyHudConstants.GPS_FADE_OUT_DURATION));
            float fadeInDistMax = timeSinceLastGPS * MyHudConstants.GPS_FADE_IN_SPEED * 1e-3f;
            float fadeInDistMin = fadeInDistMax - MyHudConstants.GPS_FADE_IN_TAIL_LENGTH;

            var pointPoses = new List<Vector3>();
            var pointDirs = new List<Vector3>();
            var pointFades = new List<float>();
            var pointWidthMultipliers = new List<float>();

            // draw arrows
            {
                float distToDo = MyHudConstants.GPS_FADE_OUT_DISTANCE_END;
                float totalDist = MyHudConstants.GPS_START_POSITION_FRONT;

                Vector4 color = new Vector4(0.3f, 0.3f, 0.3f, 0.1f);
                for (int i = 1; i < m_GPSPath.Count; i++)
                {
                    Vector3 dir = m_GPSPath[i] - m_GPSPath[i - 1];
                    float edgeLength = dir.Length();
                    dir.Normalize();

                    int arrowsInSegment = (int)Math.Round(edgeLength / MyHudConstants.GPS_SEGMENT_ADVANCE);
                    if (arrowsInSegment == 0)
                    {
                        continue;
                    }

                    float segmentAdvance = edgeLength / arrowsInSegment;
                    float segmentLength = segmentAdvance * MyHudConstants.GPS_SEGMENT_LENGTH / MyHudConstants.GPS_SEGMENT_ADVANCE;

                    for (int j = 0; j < arrowsInSegment; j++)
                    {
                        Vector3 pos = m_GPSPath[i - 1] + (j + 1) * dir * segmentAdvance;

                        if (totalDist >= MyHudConstants.GPS_FADE_OUT_DISTANCE_END) break;

                        float distFadeOut = Math.Min(1, 1 - (totalDist - MyHudConstants.GPS_FADE_OUT_DISTANCE_START) / (MyHudConstants.GPS_FADE_OUT_DISTANCE_END - MyHudConstants.GPS_FADE_OUT_DISTANCE_START));
                        float timeFadeIn = Math.Max(0, Math.Min(1, 1 - (MyHudConstants.GPS_FADE_OUT_DISTANCE_END - distToDo - fadeInDistMin) / (fadeInDistMax - fadeInDistMin)));

                        float pulseT = (float)Math.IEEERemainder(totalDist * MyHudConstants.GPS_PULSE_LENGTH_FREQ - timeSinceLastGPS * MyHudConstants.GPS_PULSE_TIME_FREQ + MyHudConstants.GPS_PULSE_TIME_PHASE, 2 * Math.PI);  // -pi..pi
                        pulseT = (float)Math.Sqrt((pulseT / (2 * Math.PI)) + 0.5f) * (float)(2 * Math.PI);
                        float pulsePhase = (float)Math.Cos(pulseT);
                        float pulseFadeOut = MyHudConstants.GPS_FADE_PULSE_DC + MyHudConstants.GPS_FADE_PULSE_AMPLITUDE * pulsePhase;
                        pulseFadeOut *= pulseFadeOut;
                        float widthMultiplier = MyHudConstants.GPS_WIDTH_PULSE_DC + MyHudConstants.GPS_WIDTH_PULSE_AMPLITUDE * pulsePhase;

                        Matrix mat = MyMath.MatrixFromDir(dir);

                        var camToPos = Vector3.Normalize(pos - MyCamera.Position);
                        var left = Math.Abs(Vector3.Dot(camToPos, mat.Left));
                        var up = Math.Abs(Vector3.Dot(camToPos, mat.Up));
                        var forward = Math.Abs(Vector3.Dot(camToPos, mat.Forward));

                        float fadeout = timeFadeIn * timeFadeOut * distFadeOut * pulseFadeOut;

                        // static X+O, fades when parallel to view direction
                        if (left * timeFadeIn * timeFadeOut * distFadeOut > 0)
                            MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.GPS, color * left * fadeout, pos - 0.5f * dir * segmentLength, mat.Up, mat.Forward, MyHudConstants.GPS_SEGMENT_WIDTH * widthMultiplier, 0.5f * segmentLength);
                        if (up * timeFadeIn * timeFadeOut * distFadeOut > 0)
                            MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.GPS, color * up * fadeout, pos - 0.5f * dir * segmentLength, mat.Left, mat.Forward, MyHudConstants.GPS_SEGMENT_WIDTH * widthMultiplier, 0.5f * segmentLength);
                        
                        // draw the Os later to save draw calls
                        if (forward * fadeout > 0)
                        {
                            pointFades.Add(forward * fadeout);
                            pointDirs.Add(dir);
                            pointPoses.Add(pos + MyHudConstants.GPS_DOT_OFFSET * dir * segmentLength);
                            pointWidthMultipliers.Add(widthMultiplier);
                        }

                        totalDist += segmentAdvance;
                    }
                }
            }

            // draw the Os here
            for (int i = 0; i < pointPoses.Count; i++)
            {
                Vector4 color = MyHudConstants.MISSION_MARKER_COLOR.ToVector4() * new Vector4(0.15f, 0.15f, 0.15f, 0.05f);
                Matrix mat = MyMath.MatrixFromDir(pointDirs[i]);
                MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.GPSBack, color * pointFades[i], pointPoses[i], mat.Left, mat.Up, MyHudConstants.GPS_DOT_WIDTH * pointWidthMultipliers[i], MyHudConstants.GPS_DOT_WIDTH * pointWidthMultipliers[i]);
            }
        }

        
        static byte GetHudColorAlphaByDistance(float distance)
        {            
            float fixedDistance = Math.Min(Math.Max(MyHudConstants.HUD_MIN_DISTANCE_TO_ALPHA_CORRECT, distance), MyHudConstants.HUD_MAX_DISTANCE_TO_ALPHA_CORRECT_NORMAL);

            float colorAlpha = MathHelper.Lerp(MyHudConstants.HUD_MIN_DISTANCE_ALPHA, MyHudConstants.HUD_MAX_DISTANCE_ALPHA,
                                                (fixedDistance - MyHudConstants.HUD_MIN_DISTANCE_TO_ALPHA_CORRECT) / 
                                                (MyHudConstants.HUD_MAX_DISTANCE_TO_ALPHA_CORRECT_NORMAL - MyHudConstants.HUD_MIN_DISTANCE_TO_ALPHA_CORRECT));
            byte colorAlhpaInByte = (byte)MathHelper.Lerp(byte.MinValue, byte.MaxValue, colorAlpha);

            return colorAlhpaInByte;
        }


        static MyHudTexturesEnum GetCrosshairTexture()
        {
            MyHudTexturesEnum crosshairTexture;
            if (MySession.PlayerShip != null && MySession.PlayerShip.GetShipCockpit() != null)
            {
                crosshairTexture = MySession.PlayerShip.GetShipCockpit().Crosshair;
            }
            else
            {
                crosshairTexture = MyHudTexturesEnum.Crosshair01;
            }
            return crosshairTexture;
        }

        static float GetCrosshairSize()
        {
            MyHudTexturesEnum crosshairTexture = GetCrosshairTexture();

            if (crosshairTexture == MyHudTexturesEnum.Crosshair01)
            {
                return MyHudConstants.DEFAULT_CROSSHAIR_SIZE;
            }
            else if (crosshairTexture == MyHudTexturesEnum.crosshair_omnicorp)
            {
                return MyHudConstants.OMNICORP_CROSSHAIR_SIZE;
            }
            else
            {
                return MyHudConstants.SPECIAL_CROSSHAIR_SIZE;
            }
        }

        static void AddCrosshairDynamic()
        {
            var crosshairPosition = MyThirdPersonSpectator.GetCrosshair();
           
            Vector3 transformedPoint = Vector3.Transform(crosshairPosition, MyCamera.ViewMatrix);
            Vector4 projectedPoint = Vector4.Transform(transformedPoint, MyCamera.ProjectionMatrix);

            if (transformedPoint.Z > 0)
            {
                projectedPoint.X *= -1;
                projectedPoint.Y *= -1;
            }
            if (projectedPoint.W == 0) return;

            Vector2 projectedPoint2D = new Vector2(projectedPoint.X / projectedPoint.W / 2.0f + 0.5f, -projectedPoint.Y / projectedPoint.W / 2.0f + 0.5f);
            if (MyVideoModeManager.IsTripleHead())
            {
                projectedPoint2D.X = (projectedPoint2D.X - (1.0f / 3.0f)) / (1.0f / 3.0f);
            }
            
            projectedPoint2D.Y *= MyGuiManager.GetHudSize().Y;

            AddTexturedQuad(GetCrosshairTexture(), projectedPoint2D, MyHudConstants.HUD_VECTOR_UP,
                MyHudConstants.HUD_COLOR_LIGHT, GetCrosshairSize(), GetCrosshairSize());
        }

        static void AddCrosshair()
        {
            AddTexturedQuad(GetCrosshairTexture(), MyGuiManager.GetHudSizeHalf(), MyHudConstants.HUD_VECTOR_UP,
                MyHudConstants.HUD_COLOR_LIGHT, GetCrosshairSize(), GetCrosshairSize());
        }

        static void AddHorisontalAngleLine()
        {
            if (!MyFakes.DRAW_CROSSHAIR_HORIZONTAL_LINE) return;
            Vector3 cameraRight = -MyCamera.LeftVector;
            Vector3 horizontal = Vector3.Up;

            float angle = MyUtils.GetAngleBetweenVectors(cameraRight, horizontal);
            angle -= MathHelper.PiOver2;
            angle *= -1;

            float deltaX = (float)Math.Cos(angle) * MyHudConstants.HORISONTAL_ANGLE_LINE_POSITON_DELTA;
            float deltaY = (float)Math.Sin(angle) * MyHudConstants.HORISONTAL_ANGLE_LINE_POSITON_DELTA;

            Vector2 positionLeft = new Vector2(MyGuiManager.GetHudSizeHalf().X - deltaX, MyGuiManager.GetHudSizeHalf().Y - deltaY);
            Vector2 positionRight = new Vector2(MyGuiManager.GetHudSizeHalf().X + deltaX, MyGuiManager.GetHudSizeHalf().Y + deltaY);

            Vector2 upVector = MyMwcUtils.Normalize(new Vector2(deltaY, -deltaX));

            //  Left line
            AddTexturedQuad(MyHudTexturesEnum.HorizontalLineLeft, positionLeft, upVector, MyHudConstants.HUD_COLOR_LIGHT, 0.75f * 128.0f / 2000.0f, 0.75f * 16.0f / 2000.0f);

            //  Right line
            AddTexturedQuad(MyHudTexturesEnum.HorizontalLineRight, positionRight, upVector, MyHudConstants.HUD_COLOR_LIGHT, 0.75f * 128.0f / 2000.0f, 0.75f * 16.0f / 2000.0f);
        }

        static void AddBackCameraBorders()
        {
            Vector2 origin = MyGuiManager.GetHudNormalizedCoordFromPixelCoord(new Vector2(MyCamera.BackwardViewport.X - 5, MyCamera.BackwardViewport.Y - 6));
            Vector2 size = MyGuiManager.GetHudNormalizedSizeFromPixelSize(new Vector2(MyCamera.BackwardViewport.Width + 10, MyCamera.BackwardViewport.Height + 14));

            AddTexturedQuad(MyHudTexturesEnum.BackCameraOverlay, origin + size / 2.0f, MyHudConstants.HUD_VECTOR_UP,
                MyHudConstants.HUD_COLOR_DARKER,
                size.X / 2.0f + .0045f, size.Y / 2.0f + .003f);
        }

        /*static void AddRadarBorders()
        {
            Vector2 origin = MyGuiManager.GetHudNormalizedCoordFromPixelCoord(new Vector2(MyCamera.HudRadarViewport.X, MyCamera.HudRadarViewport.Y));
            Vector2 size = MyGuiManager.GetHudNormalizedSizeFromPixelSize(new Vector2(MyCamera.HudRadarViewport.Width, MyCamera.HudRadarViewport.Height));

            AddTexturedQuad(MyHudTexturesEnum.RadarOverlay, origin + size / 2.0f, MyHudConstants.HUD_VECTOR_UP, MyHudConstants.HUD_COLOR_DARKER, size.X / 2.0f, size.Y / 2.0f);
        }*/

        static void AddHarvestedMaterial()
        {
            if (MySession.PlayerShip != null && MySession.PlayerShip.Inventory != null)
            {
                MyHudText hudText = m_texts.Allocate();
                if (hudText != null)
                {
                    hudText.Start(MyGuiManager.GetFontMinerWarsBlue(),
                                  MyGuiManager.GetHudSize()*new Vector2(0.01f, 0.8f),
                                  Color.White, 1f, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM);

                    foreach (MyMwcObjectBuilder_Ore_TypesEnum key in MyGuiAsteroidHelpers.MyMwcOreTypesEnumValues)
                    {
                        int val = (int)MySession.PlayerShip.Inventory.GetTotalAmountOfInventoryItems(MyMwcObjectBuilderTypeEnum.Ore, (int) key);
                        
                        if (val > 0)
                        {
                            hudText.Append(
                                MyGuiObjectBuilderHelpers.GetGuiHelper(
                                    CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilderTypeEnum.Ore, (int) key).
                                    Description);
                            hudText.Append(m_sbHelperAmountOfAmmoTextSeparator);
                            hudText.AppendInt32(val);
                            hudText.AppendLine();
                        }
                    }
                }
            }
        }

        static void AddAmountOfAmmo()
        {
            if (!MyGuiScreenGamePlay.Static.IsSelectAmmoVisible() && MySession.PlayerShip != null && MySession.PlayerShip.Weapons != null)
            {
                // Get selected weapons & ammo:
                MyGuiSmallShipHelperAmmo.Reload();
                Vector2 iconPosition = new Vector2(0.0f, 0.0f);
                /*
                if (MyFakes.MW25D)
                {
                    iconPosition.Y += MyGuiConstants.AMMO_SELECTION_DEFAULT_AFTERSELECT_ICON_SCALE.Y * (MyGuiSmallShipHelperAmmo.ItemHeight + MyGuiSmallShipHelperAmmo.ItemDistance);
                } */
                
                foreach (MyMwcObjectBuilder_FireKeyEnum key in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_AssignmentOfAmmo_FireKeyEnumValues)
                {
                    // we want display icon's only for primary and secondary key
                    if (key != MyMwcObjectBuilder_FireKeyEnum.Primary && key != MyMwcObjectBuilder_FireKeyEnum.Secondary) 
                    {
                        continue;
                    }

                    if (MySession.Is25DSector && key != MyMwcObjectBuilder_FireKeyEnum.Primary)
                    {
                        continue;
                    }
                

                    //MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType = MySession.PlayerShip.Weapons.AmmoAssignments.GetAmmoType(key);
                    var ammoAssignment = MySession.PlayerShip.Weapons.AmmoAssignments.GetAmmoAssignment(key);
                    
                    MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum? weaponType = MyGuiSmallShipHelpers.GetWeaponType(ammoAssignment.AmmoType, ammoAssignment.AmmoGroup);
                    if (weaponType == null && MySession.Is25DSector)
                        continue;

                    Debug.Assert(weaponType != null);

                    // we must check if assignet weapon type is mounted
                    if (MySession.PlayerShip.Weapons.IsMounted(weaponType.Value)) 
                    {                        
                        //MyGuiSmallShipHelperAmmo ammoHelper = MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(ammoType);
                        MyGuiSmallShipHelperAmmo ammoHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(
                            MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)ammoAssignment.AmmoType) as MyGuiSmallShipHelperAmmo;

                        if (ammoHelper != null)
                        {
                            int amount = MySession.PlayerShip.Weapons.GetAmountOfAmmo(key);
                            Vector2 guiOffset = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(MyGuiConstants.AMMO_SELECT_LEFT_TOP_POSITION);
                            var ammoSpecialText = MySession.PlayerShip.Weapons.AmmoAssignments.GetAmmoSpecialText(key);
                            ammoHelper.HudDrawActualAmmo(iconPosition + guiOffset,
                                                         MyGuiConstants.AMMO_SELECTION_DEFAULT_AFTERSELECT_ICON_SCALE,
                                                         amount,
                                                         MyGuiConstants.SELECT_AMMO_BACKGROUND_COLOR *
                                                         MyGuiConstants.SELECT_AMMO_ACTIVE_COLOR_MULTIPLIER, null);
                            if (ammoSpecialText != null)
                            {
                                ammoHelper.HudDrawActualAmmoComment(MyGuiConstants.SELECT_AMMO_BACKGROUND_COLOR * MyGuiConstants.SELECT_AMMO_ACTIVE_COLOR_MULTIPLIER, ammoSpecialText);
                            }
                        }
                    }
                    
                    //next icon & text line:
                    iconPosition.Y += MyGuiConstants.AMMO_SELECTION_DEFAULT_AFTERSELECT_ICON_SCALE.Y * (MyGuiSmallShipHelperAmmo.ItemHeight + MyGuiSmallShipHelperAmmo.ItemDistance);
                }
            }
        }

        static void ClearQuads()
        {
            m_quadsCount = 0;
        }

        static void ClearTexts()
        {
            m_texts.ClearAllAllocated();
        }

        public static MyAtlasTextureCoordinate GetTextureCoord(MyHudTexturesEnum texture)
        {
            return m_textureCoords[(int)texture];
        }

        //  Add 2D line into the lines list. Before that, we convert line to quad.
        static void AddLine2D(Vector2 vertex0, Vector2 vertex1, Color color0, Color color1, float lineThicknessHalf)
        {
            if (m_quadsCount >= MyHudConstants.MAX_HUD_QUADS_COUNT)
                return;

            //  Up vector is 2D cross product
            Vector2 direction = MyMwcUtils.Normalize(vertex1 - vertex0);
            Vector2 upVector = new Vector2(direction.Y, -direction.X);

            //  Rectangle representing the middle of the line
            int vertexIndexMiddle = m_quadsCount * MyHudConstants.VERTEXES_PER_HUD_QUAD;
            m_vertices[vertexIndexMiddle + 0].Position = new Vector3(vertex0 + upVector * lineThicknessHalf, 0);
            m_vertices[vertexIndexMiddle + 1].Position = new Vector3(vertex1 + upVector * lineThicknessHalf, 0);
            m_vertices[vertexIndexMiddle + 2].Position = new Vector3(vertex0 - upVector * lineThicknessHalf, 0);
            m_vertices[vertexIndexMiddle + 3].Position = new Vector3(vertex1 + upVector * lineThicknessHalf, 0);
            m_vertices[vertexIndexMiddle + 4].Position = new Vector3(vertex1 - upVector * lineThicknessHalf, 0);
            m_vertices[vertexIndexMiddle + 5].Position = new Vector3(vertex0 - upVector * lineThicknessHalf, 0);

            //  Calculating texture coordinates for 2D line is little hack. Because we use texture atlas, we must have some border
            //  arround line texture. Another hack is that horizontal texture coordinate is always 0.5 (in texture) as we
            //  don't want to interpolate between dark border and real line texture.
            MyAtlasTextureCoordinate textureCoord = GetTextureCoord(MyHudTexturesEnum.Line);
            m_vertices[vertexIndexMiddle + 0].TexCoord = textureCoord.Offset + new Vector2(textureCoord.Size.X * 0.5f, textureCoord.Size.Y * 0.25f);
            m_vertices[vertexIndexMiddle + 1].TexCoord = new Vector2(textureCoord.Offset.X + textureCoord.Size.X * 0.5f, textureCoord.Offset.Y + textureCoord.Size.Y * 0.25f);
            m_vertices[vertexIndexMiddle + 2].TexCoord = new Vector2(textureCoord.Offset.X + textureCoord.Size.X * 0.5f, textureCoord.Offset.Y + textureCoord.Size.Y * 0.75f);
            m_vertices[vertexIndexMiddle + 3].TexCoord = m_vertices[vertexIndexMiddle + 1].TexCoord;
            m_vertices[vertexIndexMiddle + 4].TexCoord = new Vector2(textureCoord.Offset.X + textureCoord.Size.X * 0.5f, textureCoord.Offset.Y + textureCoord.Size.Y * 0.75f);
            m_vertices[vertexIndexMiddle + 5].TexCoord = m_vertices[vertexIndexMiddle + 2].TexCoord;

            m_vertices[vertexIndexMiddle + 0].Color = color0.ToVector4();
            m_vertices[vertexIndexMiddle + 1].Color = color1.ToVector4();
            m_vertices[vertexIndexMiddle + 2].Color = color0.ToVector4();
            m_vertices[vertexIndexMiddle + 3].Color = color1.ToVector4();
            m_vertices[vertexIndexMiddle + 4].Color = color1.ToVector4();
            m_vertices[vertexIndexMiddle + 5].Color = color0.ToVector4();

            m_quadsCount++;
        }

        //  Add textured quad with specified UP direction and width/height
        public static void AddTexturedQuad(MyHudTexturesEnum texture, Vector2 position, Vector2 upVector, Color color, float halfWidth, float halfHeight)
        {
            //  Left vector is 2D cross product
            Vector2 leftVector = new Vector2(upVector.Y, -upVector.X);

            if (m_quadsCount >= MyHudConstants.MAX_HUD_QUADS_COUNT)
                return;

            //  Rectangle representing the middle of the line
            int vertexIndexMiddle = m_quadsCount * MyHudConstants.VERTEXES_PER_HUD_QUAD;
            m_vertices[vertexIndexMiddle + 0].Position = new Vector3(position + leftVector * halfWidth + upVector * halfHeight, 0);
            m_vertices[vertexIndexMiddle + 1].Position = new Vector3(position - leftVector * halfWidth + upVector * halfHeight, 0);
            m_vertices[vertexIndexMiddle + 2].Position = new Vector3(position + leftVector * halfWidth - upVector * halfHeight, 0);
            m_vertices[vertexIndexMiddle + 3].Position = m_vertices[vertexIndexMiddle + 1].Position;
            m_vertices[vertexIndexMiddle + 4].Position = new Vector3(position - leftVector * halfWidth - upVector * halfHeight, 0);
            m_vertices[vertexIndexMiddle + 5].Position = m_vertices[vertexIndexMiddle + 2].Position;

            MyAtlasTextureCoordinate textureCoord = GetTextureCoord(texture);
            m_vertices[vertexIndexMiddle + 0].TexCoord = textureCoord.Offset;
            m_vertices[vertexIndexMiddle + 1].TexCoord = new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y);
            m_vertices[vertexIndexMiddle + 2].TexCoord = new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y);
            m_vertices[vertexIndexMiddle + 3].TexCoord = m_vertices[vertexIndexMiddle + 1].TexCoord;
            m_vertices[vertexIndexMiddle + 4].TexCoord = new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y + textureCoord.Size.Y);
            m_vertices[vertexIndexMiddle + 5].TexCoord = m_vertices[vertexIndexMiddle + 2].TexCoord;

            m_vertices[vertexIndexMiddle + 0].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 1].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 2].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 3].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 4].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 5].Color = color.ToVector4();

            m_quadsCount++;
        }

        //  Add textured quad with specified UP direction
        static void AddTexturedQuad(MyHudTexturesEnum texture, Vector2 position, Vector2 upVector, Color color, float scale)
        {
            //  Left vector is 2D cross product
            Vector2 leftVector = new Vector2(upVector.Y, -upVector.X);
            MyAtlasTextureCoordinate textureCoord = GetTextureCoord(texture);
            float halfWidth = textureCoord.Size.X / 2f * scale;
            float halfHeight = textureCoord.Size.Y / 2f * scale * (3f/4f);

            //  Rectangle representing the middle of the line
            int vertexIndexMiddle = m_quadsCount * MyHudConstants.VERTEXES_PER_HUD_QUAD;
            m_vertices[vertexIndexMiddle + 0].Position = new Vector3(position + leftVector * halfWidth + upVector * halfHeight, 0);
            m_vertices[vertexIndexMiddle + 1].Position = new Vector3(position - leftVector * halfWidth + upVector * halfHeight, 0);
            m_vertices[vertexIndexMiddle + 2].Position = new Vector3(position + leftVector * halfWidth - upVector * halfHeight, 0);
            m_vertices[vertexIndexMiddle + 3].Position = m_vertices[vertexIndexMiddle + 1].Position;
            m_vertices[vertexIndexMiddle + 4].Position = new Vector3(position - leftVector * halfWidth - upVector * halfHeight, 0);
            m_vertices[vertexIndexMiddle + 5].Position = m_vertices[vertexIndexMiddle + 2].Position;

            m_vertices[vertexIndexMiddle + 0].TexCoord = textureCoord.Offset;
            m_vertices[vertexIndexMiddle + 1].TexCoord = new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y);
            m_vertices[vertexIndexMiddle + 2].TexCoord = new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y);
            m_vertices[vertexIndexMiddle + 3].TexCoord = m_vertices[vertexIndexMiddle + 1].TexCoord;
            m_vertices[vertexIndexMiddle + 4].TexCoord = new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y + textureCoord.Size.Y);
            m_vertices[vertexIndexMiddle + 5].TexCoord = m_vertices[vertexIndexMiddle + 2].TexCoord;

            m_vertices[vertexIndexMiddle + 0].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 1].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 2].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 3].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 4].Color = color.ToVector4();
            m_vertices[vertexIndexMiddle + 5].Color = color.ToVector4();

            m_quadsCount++;
        }

        //  Finally draw all lines
        static void DrawVertices()
        {          
            if (m_quadsCount <= 0) return;

            RasterizerState.CullNone.Apply();
            BlendState.NonPremultiplied.Apply();
            DepthStencilState.None.Apply();
            //SamplerState.LinearWrap;

            Device device = MyMinerGame.Static.GraphicsDevice;
            device.VertexDeclaration = MyVertexFormatPositionTextureColor.VertexDeclaration;

            MyEffectHud effect = (MyEffectHud)MyRender.GetEffect(MyEffects.Hud);
            effect.SetProjectionMatrix(m_orthographicProjectionMatrix);
            effect.SetBillboardTexture(m_texture);
            
            effect.Begin();
            device.DrawUserPrimitives(PrimitiveType.TriangleList, 0, m_quadsCount * MyHudConstants.TRIANGLES_PER_HUD_QUAD, m_vertices);
            effect.End();
        }

        public static IList<MyEntity> GetHudEnemiesOnScreen() 
        {
            return m_hudEnemies.GetEnemiesOnScreen();
        }        

        public static int GetHudEnemiesCount(float maxDistanceSqr) 
        {
            int count = 0;
            foreach (MyEntity enemy in m_hudEnemies.GetEnemiesAll())
            {
                float distanceSqr = Vector3.DistanceSquared(MyGuiScreenGamePlay.Static.ControlledEntity.WorldMatrix.Translation, enemy.GetPosition());
                if (distanceSqr <= maxDistanceSqr)
                {
                    count++;
                }
            }
            return count;
        }

        public static IList<MyEntity> GetHudEnemies() 
        {
            return m_hudEnemies.GetEnemiesAll();
        }

        public static void ShowGPSPathToNextObjective(bool showMessageIfNotAvailable)
        {
            var entity = MyGuiScreenGamePlay.Static.ControlledEntity;
            if (!showMessageIfNotAvailable &&
                GetGPSGoalEntity(entity.GetPosition()) == null)
            {
                return;
            }

            MakeGPSPathDirty();
            LastGPS = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        public static bool IsGPSWorking()
        {
            return m_computingGPS;
        }

        public static bool CanDrawElement(MyCameraAttachedToEnum cameraAttachedTo, MyHudDrawElementEnum element)
        {
            return (m_drawForCameraAttachedTo[(int)cameraAttachedTo] & element) != 0;
        }

        public static float GetClosestOreDistanceSquared()
        {
            float closestDistanceSquared = float.MaxValue;
            foreach(var ds in  m_nearestDistanceSquared)
            {
                if (ds < closestDistanceSquared)
                    closestDistanceSquared = ds;
            }
            return closestDistanceSquared;
        }

    }

    class MyHudEnemies 
    {
        private List<float> m_distances;
        private List<MyEntity> m_enemiesOnScreen;
        private List<MyEntity> m_enemiesAll;

        public MyHudEnemies() 
        {
            m_distances = new List<float>();
            m_enemiesOnScreen = new List<MyEntity>();
            m_enemiesAll = new List<MyEntity>();
        }

        public void Add(MyEntity enemy, float distance) 
        {
            if (distance <= MyHudConstants.DIRECTION_INDICATOR_MAX_SCREEN_TARGETING_DISTANCE)
            {
                AddEnemyOnScreen(enemy, distance);                
            }
            m_enemiesAll.Add(enemy);
        }

        private void AddEnemyOnScreen(MyEntity enemy, float distance) 
        {
            int count = m_enemiesOnScreen.Count;
            int index = 0;
            int left = 0;
            int right = count - 1;
            int diff = right - left;

            if(count > 0)
            {                
                while (diff > 1) 
                {
                    index = GetHalfIndex(left, right);
                    if (distance <= m_distances[index])
                    {
                        right = index;
                    }
                    else
                    {
                        left = index;
                    }
                    diff = right - left;
                }
                for (index = left; index <= right; index++) 
                {
                    if (distance <= m_distances[index]) 
                    {
                        break;
                    }
                }
            }
            AddEnemyOnScreen(enemy, distance, index);
        }

        private bool IsRightIndex(int index, float distance) 
        {
            bool testLeft = index == 0 || m_distances[index - 1] < distance;
            bool testRight = index == m_enemiesOnScreen.Count - 1 || m_distances[index] >= distance;
            return testLeft && testRight;
        }

        private int GetHalfIndex(int from, int to) 
        {
            int diff = to - from;
            if (diff == 1) 
            {

            }
            float half = (float)(to - from) / 2f;
            int index = from + (int)half;
            return index;
        }

        private void AddEnemyOnScreen(MyEntity enemy, float distance, int index) 
        {
            m_distances.Insert(index, distance);
            m_enemiesOnScreen.Insert(index, enemy);
        }

        public void Clear() 
        {
            m_distances.Clear();
            m_enemiesOnScreen.Clear();
            m_enemiesAll.Clear();
        }

        public List<MyEntity> GetEnemiesOnScreen() 
        {
            return m_enemiesOnScreen;
        }

        public List<MyEntity> GetEnemiesAll() 
        {
            return m_enemiesAll;
        }
    }
}