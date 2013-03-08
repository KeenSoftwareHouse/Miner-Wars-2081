using System;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Cockpit;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using SysUtils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Physics;

namespace MinerWars.AppCode.Game.Utils
{
    static class MyConstants
    {
        //  Place here all asserts related to consts
        static MyConstants()
        {
            MyCommonDebugUtils.AssertRelease(MyAutocanonConstants.SMOKES_MIN < MyAutocanonConstants.SMOKES_MAX);
            MyCommonDebugUtils.AssertRelease(MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MAX <= MyExplosionsConstants.EXPLOSION_RADIUS_MAX);
        }

        // Maximum number of inventory items in objects pool
        public static readonly int INVENTORY_ITEM_POOL = 8192;

        //  This is recommended or preferred anti-aliasing
        public const MyAntiAliasingEnum PREFERRED_ANTI_ALIASING = MyAntiAliasingEnum.FOUR_SAMPLES;

        //public static readonly Vector3 REFLECTOR_POSITION_DELTA = (new Vector3(0, -1, 10));
        public static readonly Vector3 REFLECTOR_POSITION_DELTA = (new Vector3(0, 2, 2));

        // Size of preallocated buffers.
        // NEVER increase this from 20000 without discussing it first.
        public const int MAX_ENTITIES = 20000;

        //  If this is true, JLX make sphere vs. voxel map collision detection using voxel values and not triangles (it is faster).
        //  If false, it uses triangles.
        public static bool SPHERE_VOXELMAP_COLDET_THROUGH_VOXELS = true;

        //  How many physical forces (body, world, impulse, etc) can be applied to one physical object in one update call
        public const int MAX_EXTERNAL_FORCES_PER_PHYSICAL_OBJECT = 10;

        //  Phys object created near/inside the external 'explosion force' are making JLX freeze, so we don't create debris too close to the center of explosion
        //  This applies to every add force, because we can't be sure we aren't adding force to objects that are already in place.
        public const float SAFE_DISTANCE_FOR_ADD_WORLD_FORCE_JLX = 0.05f;

        // Sun visibility check is not performed more often then specified by this interval
        public static TimeSpan VISIBLE_FROM_SUN_MIN_CHECK_INTERVAL = TimeSpan.FromMilliseconds(500);

        //  Camera
        //  Lucky thing about XNA's Matrix.CreatePerspectiveFieldOfView() method is that it creates good perspective matrix for any resolution, and still calculates good FOV.
        //  So I don't have to change this angle. I am assuming that this FOV is vertical angle, and above method calculates horizontal angle on aspect ratio. Thus users with
        //  wider aspect ratio (e.g. 3x16:10) will see bigger horizontal FOV, but same vertical FOV. And that's good.
        public static readonly float FIELD_OF_VIEW_CONFIG_MIN = MathHelper.ToRadians(40);
        public static readonly float FIELD_OF_VIEW_CONFIG_MAX = MathHelper.ToRadians(90);
        public static readonly float FIELD_OF_VIEW_CONFIG_DEFAULT = MathHelper.ToRadians(60);
        public static readonly float FIELD_OF_VIEW_CONFIG_MAX_DUAL_HEAD = MathHelper.ToRadians(80);
        public static readonly float FIELD_OF_VIEW_CONFIG_MAX_TRIPLE_HEAD = MathHelper.ToRadians(70);

        // FIELD_OF_VIEW_MAX replaced by MyCamera.FieldOfView (could be changed in video options)
        //public static readonly float FIELD_OF_VIEW_MAX = MathHelper.ToRadians(70);
        public static readonly float FIELD_OF_VIEW_MIN = MathHelper.ToRadians(5);

        //  Two times bigger than sector's diameter because we want to draw impostor voxel maps in surrounding sectors
        //  According to information from xna creators site, far plane distance doesn't have impact on depth buffer precission, but near plane has.
        //  Therefore far plane distance can be any large number, but near plane distance can't be too small.

        //  This value is 60 seconds in the past. It used to setup 'last time' values during initialization to some time that is far in the past.
        //  I can't set it to int.MinValue, because than I will get overflow problems.
        public const int FAREST_TIME_IN_PAST = -60 * 1000;

        //  Physics
        public const float PHYSICS_STEPS_PER_SECOND = 60;       //  Looks like if I set it bellow 100 (e.g. to 60), mouse rotation seems not-seamless...
        public const float PHYSICS_STEP_SIZE_IN_SECONDS = 1.0f / PHYSICS_STEPS_PER_SECOND;
        public const int PHYSICS_STEP_SIZE_IN_MILLISECONDS = (int)(1000.0f / PHYSICS_STEPS_PER_SECOND);

        public const float PHYSICS_STANDARD_DYNAMIC_FRICTION = 0.65f;
        public const float PHYSICS_STANDARD_STATIC_FRICTION = 0.65f;
        public const float PHYSICS_STANDARD_RESTITUTION = 0.4f;
        public const float PHYSICS_AMMO_RESTITUTION = 0.01f;
        public const float PHYSICS_BOT_RESTITUTION = 0.1f;

        //RigidBody types for collision events
        public const ushort RIGIDBODY_TYPE_DEFAULT = 0;
        public const ushort RIGIDBODY_TYPE_SHIP = 1;

        //  Ambient light coming from stars, not from sun. 
        //  This value must be used for all drawing: voxels, models, particles, glasses, etc.
        //  If you change it here, change it also in MyCommonEffect.fx in constant: AmbientColor
        public const float AMBIENT_COLOR = 0.07f;

        //  Vector2 default values
        public static readonly Vector2 VECTOR2_X0_Y0 = Vector2.Zero;
        public static readonly Vector2 VECTOR2_X1_Y0 = new Vector2(1, 0);
        public static readonly Vector2 VECTOR2_X0_Y1 = new Vector2(0, 1);
        public static readonly Vector2 VECTOR2_X1_Y1 = new Vector2(1, 1);

        //  Vector3 constants needed for "ref" optimizations (because XNA's default (e.g. Vector3.Zero) have no setter so we can't use them with 'ref')
        public static Vector3 VECTOR3_UP = Vector3.Up;

        public const int DEFAULT_SPECTATOR_SPEED = 30;

        public static readonly Vector3 GAME_PRUNING_STRUCTURE_AABB_EXTENSION = new Vector3(3.0f);
        public const int GAME_PRUNING_STRUCTURE_PROXY_ID_NOT_INSERTED = -1;


        //Collion layers for enabling/disabling collisions per type
        public const ushort COLLISION_LAYER_DEFAULT = 0;
        public const ushort COLLISION_LAYER_UNCOLLIDABLE = 1;
        public const ushort COLLISION_LAYER_MISSILE = 2;
        public const ushort COLLISION_LAYER_VOXEL_DEBRIS = 3;
        public const ushort COLLISION_LAYER_MODEL_DEBRIS = 4;
        public const ushort COLLISION_LAYER_PREFAB = 5;
        public const ushort COLLISION_LAYER_PREFAB_KINEMATIC_PART = 6;
        public const ushort COLLISION_LAYER_METEOR = 7;
        public const ushort COLLISION_LAYER_PREFAB_ALARM = 8;
        public const ushort COLLISION_LAYER_SMALL_SHIP = 9;
        public const ushort COLLISION_LAYER_LIGHT = 10;

        internal static void InitializeCollisionLayers()
        {
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_DEFAULT, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_SMALL_SHIP, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_MISSILE, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_METEOR, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_MODEL_DEBRIS, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_VOXEL_DEBRIS, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_ALARM, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC_PART, true);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_LIGHT, true);


            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_UNCOLLIDABLE, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_METEOR, MyConstants.COLLISION_LAYER_VOXEL_DEBRIS, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_METEOR, MyConstants.COLLISION_LAYER_METEOR, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_MODEL_DEBRIS, MyConstants.COLLISION_LAYER_MODEL_DEBRIS, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_MODEL_DEBRIS, MyConstants.COLLISION_LAYER_MISSILE, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_MODEL_DEBRIS, MyConstants.COLLISION_LAYER_SMALL_SHIP, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_VOXEL_DEBRIS, MyConstants.COLLISION_LAYER_VOXEL_DEBRIS, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_VOXEL_DEBRIS, MyConstants.COLLISION_LAYER_MODEL_DEBRIS, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_VOXEL_DEBRIS, MyConstants.COLLISION_LAYER_SMALL_SHIP, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_VOXEL_DEBRIS, MyConstants.COLLISION_LAYER_MISSILE, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC_PART, MyConstants.COLLISION_LAYER_PREFAB, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB, MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC_PART, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_ALARM, MyConstants.COLLISION_LAYER_DEFAULT, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_ALARM, MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC_PART, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_PREFAB_ALARM, MyConstants.COLLISION_LAYER_SMALL_SHIP, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_SMALL_SHIP, MyConstants.COLLISION_LAYER_MODEL_DEBRIS, false);
            MyPhysics.physicsSystem.GetRigidBodyModule().EnableCollisionInLayers(MyConstants.COLLISION_LAYER_SMALL_SHIP, MyConstants.COLLISION_LAYER_LIGHT, false);
        }
    }

    //  These are texts that I don't want or I can't have in localized text wrapper. They are pure system messages.
    public static class MyTextConstants
    {
        public const string APP_NAME = "Miner Wars";
        public const string APP_ERROR_CAPTION = "Miner Wars - Application Error";
        public const string APP_ERROR_MESSAGE = "Miner Wars - application error occured. For more information please see application log at {0} " + MyStringUtils.C_CRLF + MyStringUtils.C_CRLF +
            "If you want to help us make Miner Wars a better game, please send the application log to support@minerwars.com" + MyStringUtils.C_CRLF + MyStringUtils.C_CRLF +
                "Thank You!" + MyStringUtils.C_CRLF +
                    "Keen Software House";
        public const string NOT_LAUNCHED_FROM_LAUNCHER = "Please run Miner Wars only from the Miner Wars Launcher, not MinerWars.exe directly.";
        public const string ON_NO_SUITABLE_GRAPHICS_DEVICE_EXCEPTION = "Miner Wars requires graphic adapter that supports Pixel Shader / Vertex Shader 3.0 or higher. Please make sure DirectX 9.0c or higher is installed and your video drivers are up to date.";
        public const string REQUIRE_PIXEL_SHADER_3 = "Miner Wars requires graphic adapter that supports Pixel Shader 3.0 or higher. Please make sure DirectX 9.0c or higher is installed and your video drivers are up to date.";
        public const string REQUIRE_VERTEX_SHADER_3 = "Miner Wars requires graphic adapter that supports Vertex Shader 3.0 or higher. Please make sure DirectX 9.0c or higher is installed and your video drivers are up to date.";
    }

    static class MyTrailerConstants
    {
        //  We store max 10 minutes
        public const int MAX_TRACKED_TICKS = 10 * 60 * (int)MyConstants.PHYSICS_STEPS_PER_SECOND;
        //Make sure that names of these animations are same as in Trailer.xmlx file!
        public const string SHIP_ATTACK_ANIMATION = "ShipAttack";
        public const string RACE_ANIMATION = "RaceInTheTunnels";
        public const string FIGHT_ANIMATION = "Fight";
        public const string ICEFIGHT_ANIMATION = "IceFight";
        public const string MENU_ANIMATION = "Menu";

        public static readonly MyMwcSectorIdentifier DEFAULT_SECTOR_IDENTIFIER = new MyMwcSectorIdentifier(
            MyMwcSectorTypeEnum.STORY, null, new MyMwcVector3Int(0, 0, 0), null);

        public static readonly int DEFAULT_SECTOR_VERSION = 0;
    }

    static class MyGuiConstants
    {
        // General gui constants
        public const int GAME_PLAY_SCREEN_FADEIN_IN_MILLISECONDS = 50;
        public const float MOUSE_CURSOR_SPEED_MULTIPLIER = 1.3f;
        public const int VIDEO_OPTIONS_CONFIRMATION_TIMEOUT_IN_MILISECONDS = 60 * 1000;
        public static readonly Vector2 SHADOW_OFFSET = new Vector2(0.000f, 0.000f);//new Vector2(0.0045f, 0.005f);//new Vector2(0.003f, 0.003f);
        public static readonly Vector2 SHADOW_SIZE = new Vector2(1.05f, 1.05f);
        public static readonly Vector2 CONTROL_SHADOW_OFFSET = new Vector2(0.000f, 0.000f);//new Vector2(0.0045f, 0.005f);//new Vector2(0.003f, 0.003f);
        public static readonly Vector2 CONTROL_SHADOW_SIZE = new Vector2(1.02f, 1.09f);
        //public static readonly Vector4 CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER = new Vector4(1.67f, 0.64f, 0.0f, 1.0f);
        public static readonly Vector4 CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER = new Vector4(1.2f, 1.2f, 1.2f, 1.2f);
        public static readonly Vector4 CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER_SOLAR_MAP = new Vector4(2f, 2f, 2f, 1.0f);
        public static readonly Vector2 CONTROLS_DELTA = new Vector2(0, 0.0525f);
        public static readonly Vector4 ROTATING_WHEEL_COLOR = Vector4.One;
        public const float ROTATING_WHEEL_DEFAULT_SCALE = 0.54f;
        public static readonly int SHOW_CONTROL_TOOLTIP_DELAY = 300; //in milliseconds - after this period, show tooltip on control
        public static readonly float TOOLTIP_DISTANCE_FROM_BORDER = 0.003f; //in normalized coordinates
        public static Color DEFAULT_CONTROL_FOREGROUND_COLOR = new Color(0.77f, 0.97f, 0.99f, 0.8f);
        public static readonly Vector4 DEFAULT_CONTROL_BACKGROUND_COLOR = new Vector4(0.9f, 0.9f, 0.9f, 0.95f);/*new Vector4(0.65f, 0.65f, 0.65f, 0.7f);*///new Vector4(0.5f, 0.5f, 0.5f, 0.7f);
        public static readonly Vector4 DEFAULT_CONTROL_NONACTIVE_COLOR = new Vector4(0.9f, 0.9f, 0.9f, 0.95f);
        public static readonly Vector4 DEFAULT_CONTROL_HIGHLIGHT_TEXT_COLOR = new Vector4(1f, 1f, 1f, 1f);
        public static readonly Vector4 DEFAULT_CONTROL_NONACTIVE_COLOR_HALF = new Vector4(0.25f, 0.35f, 0.375f, 0.9f);
        public static Color DISABLED_BUTTON_COLOR = new Color(87, 127, 147, 210);
        public static Vector4 DISABLED_BUTTON_COLOR_VECTOR = new Vector4(0.52f, 0.6f, 0.63f, 0.9f);//0.43,0,0,0.824f
        public static Vector4 DISABLED_BUTTON_TEXT_COLOR = new Vector4(0.4f, 0.47f, 0.5f, 0.8f);//new Vector4(0.6f, 0.0f, 0.0f, 1.0f);//Color.Red.ToVector4();
        public static Vector4 DISABLED_BUTTON_NON_RED_MULTIPLIER = new Vector4(0.7f, 0.7f, 0.7f, 1);
        public static float LOCKBUTTON_SIZE_MODIFICATION = 0.85f;

        public static MyGuiFont DEFAULT_CONTROL_FONT 
        {
            get { return MyGuiManager.GetFontMinerWarsWhite(); }
        }

        public const float APP_VERSION_TEXT_SCALE = 0.95f;
        public const float APP_GLOBAL_TEXT_SCALE = 0.90f;
        public const float LOGED_PLAYER_NAME_TEXT_SCALE = 0.90f;

        public static readonly Vector2 HUD_FREE_SPACE = new Vector2(0.01f, 0.01f);

        public static readonly MyMwcVector3Int SOLAR_SYSTEM_FORBIDDEN_AREA = new MyMwcVector3Int(100, 100, 100);

        // Inline text colors
        public static readonly Color INLINE_TEXT_COLOR_1 = new Color(1.0f, 1.0f, 1.0f, 1.0f);  // White
        public static readonly Color INLINE_TEXT_COLOR_2 = new Color(1.0f, 0.647f, 0.0f, 1.0f);// Orange
        public static readonly Color INLINE_TEXT_COLOR_3 = new Color(1.0f, 0.0f, 0.0f, 1.0f);  // Red
        public static readonly Color INLINE_TEXT_COLOR_4 = new Color(1.0f, 1.0f, 0.0f, 1.0f);  // Yellow
        public static readonly Color INLINE_TEXT_COLOR_5 = new Color(0.0f, 1.0f, 0.0f, 1.0f);  // Green
        public static readonly Color INLINE_TEXT_COLOR_6 = new Color(0.0f, 1.0f, 1.0f, 1.0f);  // Cyan
        public static readonly Color INLINE_TEXT_COLOR_7 = new Color(0.0f, 0.0f, 1.0f, 1.0f);  // Blue
        public static readonly Color INLINE_TEXT_COLOR_8 = new Color(1.0f, 0.0f, 1.0f, 1.0f);  // Magenta

        // Screen gui constants
        public const float SCREEN_CAPTION_TEXT_SCALE = 0.95f;
        public static readonly Vector4 SCREEN_CAPTION_TEXT_COLOR = Vector4.One;//new Vector4(0.77f, 0.97f, 0.97f, 1); //new Vector4(0.65f, 0.675f, 0.7f, 1);
        public static readonly Vector4 SCREEN_BACKGROUND_FADE_BLANK_DARK = new Vector4(0.03f, 0.04f, 0.05f, 0.7f);
        public static readonly Vector4 SCREEN_BACKGROUND_TRANSPARENT = new Vector4(0.00f, 0.00f, 0.00f, 0.0f);
        public static readonly Vector4 SCREEN_BACKGROUND_FADE_BLANK_DARK_PROGRESS_SCREEN = new Vector4(0.03f, 0.04f, 0.05f, 0.4f);
        public static readonly Vector4 SCREEN_BACKGROUND_FADE_BLANK_DARK_LIGHT_PROGRESS_SCREEN = new Vector4(0.03f, 0.04f, 0.05f, 0.2f);
        public static readonly float SCREEN_CAPTION_DELTA_Y = 0.05f;
        public static readonly Vector4 SCREEN_BACKGROUND_COLOR = Vector4.One;
        //  This is screen height we use as reference, so all fonts, textures, etc are made for it and if this height resolution used, it will be 1.0
        //  If e.g. we use vertical resolution 600, then averything must by scaled by 600 / 1200 = 0.5
        public const float REFERENCE_SCREEN_HEIGHT = 1080;
        //public static readonly Vector4 SCREEN_BACKGROUND_FADE_DIRT_DEFAULT = new Vector4(0.0f, 0.0f, 0.0f, 0.1f);//new Vector4(0.0f, 0.0f, 0.0f, 0.5f);//Vector4.One;// new Vector4(0.03f, 0.04f, 0.05f, 1f);
        //public static readonly Vector4 SCREEN_BACKGROUND_FADE_BLANK_FROM = Vector4.Zero;
        //public static readonly Vector2 SCREEN_CAPTION_DELTA = new Vector2(0.05f, 0.04f);

        public const float LOADING_PLEASE_WAIT_SCALE = 1.1f;
        public static readonly Vector2 LOADING_PLEASE_WAIT_POSITION = new Vector2(0.5f, 0.95f);
        //public static readonly Vector4 LOADING_PLEASE_WAIT_COLOR = LABEL_TEXT_COLOR;
        public static readonly Vector4 LOADING_PLEASE_WAIT_COLOR = Vector4.One;

        // Textbox gui constants
        public const int TEXTBOX_MOVEMENT_DELAY = 100;
        //  Delay between we accept same key press (e.g. when user holds left key, or X key for a longer period)
        public const int TEXTBOX_CHANGE_DELAY = 500;
        public const string TEXTBOX_FALLBACK_CHARACTER = "#";
        public static readonly Vector2 TEXTBOX_TEXT_OFFSET = new Vector2(0.015f, 0);

        public static readonly Vector2 TEXTBOX_SMALL_SIZE = new Vector2(0.225f, 0.04f);
        public static readonly Vector2 TEXTBOX_MEDIUM_SIZE = new Vector2(404f / 1600f, 66f / 1200f);
        public static readonly Vector2 TEXTBOX_MEDIUM_LONG_SIZE = new Vector2(808f / 1600f, 66f / 1200f);
        public static readonly Vector2 TEXTBOX_LARGE_SIZE = TEXTBOX_MEDIUM_SIZE;
        public static readonly Vector4 TEXTBOX_BACKGROUND_COLOR = DEFAULT_CONTROL_BACKGROUND_COLOR;

        // Checkbox gui constants
        public static readonly Vector2 CHECKBOX_SIZE = new Vector2(0.03f, 0.04f) * 0.85f;
        public static readonly Vector2 CHECKBOX_WITH_GLOW_SIZE = CHECKBOX_SIZE * 1.5f;
        public static readonly Vector4 CHECKBOX_BACKGROUND_COLOR = new Vector4(0.85f,0.85f,0.85f,1.0f);

        // RadioButton gui constants
        public static readonly Vector2 RADIOBUTTON_SIZE = new Vector2(0.03f, 0.04f) * 0.85f;
        public static readonly Vector4 RADIOBUTTON_BACKGROUND_COLOR = DEFAULT_CONTROL_BACKGROUND_COLOR;

        // Label gui constants
        public const float LABEL_TEXT_SCALE = 1.0f;//0.73f
        public const float EDITOR_LABEL_TEXT_SCALE = 0.9f;//0.73f
        public const float LABEL_TEXT_SCALE_OPTIONS = 0.85f;//0.73f
        public static readonly Vector4 LABEL_TEXT_COLOR = new Vector4(1.00f, 1.00f, 1.00f, 1.0f);//new Vector4(0.7f, 0.7f, 0.7f, 1);

        // Mouse gui constants
        //public static readonly Vector4 MOUSE_CURSOR_COLOR = new Vector4(0.8f, 0, 0, 1);
        public static readonly Vector4 MOUSE_CURSOR_COLOR = Vector4.One;
        public const float MOUSE_CURSOR_SCALE = 1;
        public static readonly Vector2 MOUSE_CURSOR_CENTER_POSITION = new Vector2(0.5f, 0.5f);

        // Rotation constants
        public const float MOUSE_ROTATION_INDICATOR_MULTIPLIER = 0.075f;
        public const float ROTATION_INDICATOR_MULTIPLIER = 0.15f;  // empirical value for nice keyboard rotation: mouse/joystick/gamepad sensitivity can be tweaked by the user
        public const float PREFAB_CAMERA_ROTATION_SENSITIVITY = 0.0025f;

        // Button gui constants
        public static readonly Vector4 BUTTON_BACKGROUND_COLOR = DEFAULT_CONTROL_BACKGROUND_COLOR;
        public static readonly Vector4 BUTTON_TEXT_COLOR = DEFAULT_CONTROL_NONACTIVE_COLOR;//LABEL_TEXT_COLOR;
        public const float BUTTON_TEXT_SCALE = LABEL_TEXT_SCALE;
        public const float BUTTON_TEXT_SCALE_SMALLER = BUTTON_TEXT_SCALE*0.8f;
        public static readonly Vector2 BUTTON_TEXT_OFFSET = new Vector2(0.02f, 0);
        public const float BUTTON_MOUSE_OVER_TEXT_SCALE = 1.05f;
        public static readonly Vector2 MENU_BUTTONS_POSITION_DELTA = new Vector2(0, 0.0702f);
        public static readonly Vector4 BACK_BUTTON_BACKGROUND_COLOR = BUTTON_BACKGROUND_COLOR;// - new Vector4(0.05f, 0.05f, 0.05f, 0);
        public static readonly Vector4 BACK_BUTTON_TEXT_COLOR = DEFAULT_CONTROL_NONACTIVE_COLOR;//LABEL_TEXT_COLOR;// - new Vector4(0.1f, 0.1f, 0.1f, 0);
        public const float BACK_BUTTON_TEXT_SCALE = LABEL_TEXT_SCALE;
        public static readonly Vector2 BACK_BUTTON_SIZE = new Vector2(260f / 1600f, 70f / 1200f); //new Vector2(0.167f, 0.0596f);
        public static readonly Vector2 OK_BUTTON_SIZE = new Vector2(0.177f, 0.0765f);

        public static readonly Vector2 PROGRESS_CANCEL_BUTTON_SIZE = new Vector2(0.11f, 0.048f);
        public static readonly Vector2 MAIN_MENU_BUTTON_SIZE = new Vector2(409f/1600f, 90f/1200f);
        public const float BUTTON_HOVER_SCALE = 1.0f;
        public const float BUTTON_PRESSED_SCALE = 1.0f;
        public const int BUTTON_HOVER_SCALE_TIME = 200;
        public static readonly float BUTTON_DEFAULTS_WIDTH = MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2;

        //Journal 
        public static readonly Vector2 JOURNAL_BUTTON_SIZE = new Vector2(90f / 1600f, 90f / 1200f);
        // TreeView gui constants
        public static readonly Vector4 TREEVIEW_BACKGROUND_COLOR = new Vector4(33 / 255f, 89 / 255f, 142 / 255f, 0.85f);
        public static Vector4 TREEVIEW_SELECTED_ITEM_COLOR = new Vector4(0.03f, 0.02f, 0.03f, 0.4f);
        public static Vector4 TREEVIEW_DISABLED_ITEM_COLOR = new Vector4(1.0f, 0.3f, 0.3f, 1.0f);
        public static readonly Vector4 TREEVIEW_TEXT_COLOR = DEFAULT_CONTROL_NONACTIVE_COLOR;
        public static readonly Vector4 TREEVIEW_VERTICAL_LINE_COLOR = new Vector4(158 / 255f, 208 / 255f, 1, 1);
        public static readonly Vector2 TREEVIEW_VSCROLLBAR_SIZE = new Vector2(20 * 3, 159 * 4) / 3088;
        public static readonly Vector2 TREEVIEW_HSCROLLBAR_SIZE = new Vector2(159 * 3, 20 * 4) / 3088;
        public static readonly Vector2 TREEVIEW_SCROLLBAR_SIZE = new Vector2(MyGuiConstants.TREEVIEW_VSCROLLBAR_SIZE.X, MyGuiConstants.TREEVIEW_HSCROLLBAR_SIZE.Y);

        // Combobox gui constants                                                                                                   
        public static readonly Vector2 COMBOBOX_TEXT_OFFSET = new Vector2(0.022f, 0);
        public static readonly Vector4 COMBOBOX_TEXT_COLOR = new Vector4(1f, 1f, 1f, 1f);
        public static readonly Vector2 COMBOBOX_SMALL_SIZE = new Vector2(0.125f, 0.04f);
        public static readonly Vector2 COMBOBOX_MEDIUM_SIZE = new Vector2(0.318f, 0.053f);
        public static readonly Vector2 COMBOBOX_LONGMEDIUM_SIZE = new Vector2(0.318f, 0.060f);
        public static readonly Vector2 COMBOBOX_LONGMEDIUM_ELEMENT_SIZE = new Vector2(0.318f, 0.045f);

        //public static readonly Vector2 COMBOBOX_MEDIUM_SIZE_OFFSET = new Vector2(0.00f, 0.023f);
        public static readonly Vector2 COMBOBOX_MEDIUM_ELEMENT_SIZE = new Vector2(0.318f, 0.03f);
        public static readonly float COMBOBOX_MEDIUM_GLOW_SIZE= 0.010f;
        public static readonly float COMBOBOX_MEDIUM_DROPBOX_TOP_OFFSET = 0.011f;

        public static readonly float COMBOBOX_LONGMEDIUM_DROPBOX_TOP_OFFSET = 0.020f;

        
        public static readonly Vector2 COMBOBOX_LARGE_SIZE = new Vector2(0.65f, 0.04f);
        public static readonly Vector4 COMBOBOX_BACKGROUND_COLOR = DEFAULT_CONTROL_BACKGROUND_COLOR;//new Vector4(SCREEN_BACKGROUND_COLOR.X, SCREEN_BACKGROUND_COLOR.Y, SCREEN_BACKGROUND_COLOR.Z, 0.8f);
        public static readonly Vector4 COMBOBOX_VERTICAL_LINE_COLOR = new Vector4(0.34f, 0.41f, 0.42f, 0.7f);
        public const float COMBOBOX_SCROLLBAR_MIN_HEIGHT = 0.128f;
        public static Color COMBOBOX_SCROLLBAR_COLOR = Color.White;
        public static Vector4 COMBOBOX_SELECTED_ITEM_COLOR = new Vector4(0.03f, 0.02f, 0.03f, 0.4f);
        public static Vector2 COMBOBOX_ICON_SIZE = new Vector2(0.15f, 0.15f);
        public const float COMBOBOX_TEXT_SCALE = LABEL_TEXT_SCALE * 0.7f;
        public static readonly Vector2 COMBOBOX_VSCROLLBAR_SIZE = new Vector2(0.0383005f, 0.0805958545f);
        public static readonly Vector2 COMBOBOX_HSCROLLBAR_SIZE = new Vector2(0.0805958545f, 0.0383005f);

        // Listbox gui constants
        public static readonly Vector2 LISTBOX_TEXT_OFFSET = new Vector2(0.01f, 0);
        public static readonly Vector4 LISTBOX_TEXT_COLOR = DEFAULT_CONTROL_NONACTIVE_COLOR;
        //public static readonly Vector2 LISTBOX_SMALL_SIZE = new Vector2(0.125f, 0.04f);
        public static readonly Vector2 LISTBOX_SMALL_SIZE = new Vector2(LISTBOX_ICON_SIZE_X, LISTBOX_ICON_SIZE_Y);  // used for icon's listbox (inventory etc)
        public static readonly Vector2 LISTBOX_MEDIUM_SIZE = new Vector2(0.25f, 0.04f);
        public static readonly Vector2 LISTBOX_LONGMEDIUM_SIZE = new Vector2(0.65f, 0.04f);
        public static readonly Vector2 LISTBOX_LARGE_SIZE = new Vector2(0.45f, LISTBOX_ICON_SIZE_Y);
        public static readonly Vector4 LISTBOX_BACKGROUND_COLOR = DEFAULT_CONTROL_BACKGROUND_COLOR;
        public static readonly Vector4 LISTBOX_BACKGROUND_COLOR_BLUE = new Color(21, 73, 120, 160).ToVector4();
        public static readonly Vector4 LISTBOX_DISABLED_COLOR = new Vector4(1f, 0f, 0f, 0.7f);
        public static readonly Vector4 LISTBOX_LINE_COLOR = Color.CornflowerBlue.ToVector4(); //new Vector4(0.34f, 0.41f, 0.42f, 0.7f);
        public static Color LISTBOX_SCROLLBAR_COLOR = Color.White;
        public static Vector4 LISTBOX_SELECTED_ITEM_COLOR = new Vector4(0.03f, 0.02f, 0.03f, 0.4f);
        public static Vector4 LISTBOX_ITEM_COLOR = new Vector4(0.13f, 0.12f, 0.13f, 0.4f);
        public static Vector4 LISTBOX_EMPTY_SLOT_COLOR = new Vector4(0.23f, 0.22f, 0.23f, 0.4f);
        public static Vector4 LISTBOX_HIGHLIGHT_MULTIPLIER = new Vector4(0.033f, 0.022f, 0.033f, 0.42f);
        //public const float LISTBOX_ICON_SIZE = 0.15f;
        //public const float LISTBOX_ICON_SIZE = 0.075f;
        public const float LISTBOX_ICON_SIZE_X = 0.07395f;
        public const float LISTBOX_ICON_SIZE_Y = LISTBOX_ICON_SIZE_X * 4 / 3;
        public const float LISTBOX_SCROLLBAR_MIN_SIZE = 0.08059f;
        public const float LISTBOX_SCROLLBAR_MAX_SIZE = 0.08059f;
        public const float LISTBOX_SCROLLBAR_WIDTH = 0.0383005f;

        // Drag and drop gui constants
        public static readonly Vector2 DRAG_AND_DROP_TEXT_OFFSET = new Vector2(0.01f, 0);
        public static readonly Vector4 DRAG_AND_DROP_TEXT_COLOR = DEFAULT_CONTROL_NONACTIVE_COLOR;
        //public static readonly Vector2 DRAG_AND_DROP_SMALL_SIZE = new Vector2(0.125f, 0.04f);
        public static readonly Vector2 DRAG_AND_DROP_SMALL_SIZE = new Vector2(DRAG_AND_DROP_ICON_SIZE_X, DRAG_AND_DROP_ICON_SIZE_Y);
        public static readonly Vector2 DRAG_AND_DROP_MEDIUM_SIZE = new Vector2(0.2f, 0.04f);
        public static readonly Vector2 DRAG_AND_DROP_LONGMEDIUM_SIZE = new Vector2(0.65f, 0.04f);
        public static readonly Vector2 DRAG_AND_DROP_LARGE_SIZE = new Vector2(0.45f, DRAG_AND_DROP_ICON_SIZE_Y);
        public static readonly Vector4 DRAG_AND_DROP_BACKGROUND_COLOR = new Vector4(1f, 1f, 1f, 1f);
        //public const float DRAG_AND_DROP_ICON_SIZE = 0.15f;
        //public const float DRAG_AND_DROP_ICON_SIZE = 0.075f;
        public const float DRAG_AND_DROP_ICON_SIZE_X = LISTBOX_ICON_SIZE_X;
        public const float DRAG_AND_DROP_ICON_SIZE_Y = DRAG_AND_DROP_ICON_SIZE_X * 4 / 3;

        // Slider gui constants
        public const float SLIDER_HEIGHT = 0.06f;
        public static readonly float SLIDER_INSIDE_OFFSET_X = 0.017f;
        public static readonly Vector4 SLIDER_BACKGROUND_COLOR = DEFAULT_CONTROL_BACKGROUND_COLOR;//new Vector4(0.4f, 0.45f, 0.50f, 0.75f);
        public const float SLIDER_WIDTH_LABEL = 0.0f;
        public const float SLIDER_WIDTH = 0.25f - SLIDER_WIDTH_LABEL;

        // Messagebox gui constants
        public const float MESSAGE_BOX_BORDER_AREA_X = 0.1f;
        public const float MESSAGE_BOX_BORDER_AREA_Y = 0.05f;
        public const float MESSAGE_BOX_TEXT_SCALE = LABEL_TEXT_SCALE;
        public static readonly Vector2 MESSAGE_BOX_BUTTON_SIZE = BACK_BUTTON_SIZE;
        public static readonly Vector2 SEARCH_BUTTON_SIZE = new Vector2(134 / 1600f, 70f / 1200f);
        public static readonly Vector2 MESSAGE_BOX_BUTTON_SIZE_SMALL = new Vector2(190f / 1600f, 65f / 1200f);
        //public static readonly Vector4 MESSAGE_BOX_BACKGROUND_COLOR = new Vector4(0.5f, 0, 0, 0.95f);//(new Color(50, 4, 1, 220)).ToVector4();
        //public static readonly Vector4 MESSAGE_BOX_BUTTON_BACKGROUND_COLOR = new Vector4(0.5f, 0, 0, 0.4f);//(new Color(60, 6, 1, 191)).ToVector4();
        //public static readonly Vector4 MESSAGE_BOX_TEXT_COLOR = (new Color(255, 0, 0, 240)).ToVector4();
        //public static readonly Vector4 MESSAGE_BOX_ROTATING_WHEEL_COLOR = MESSAGE_BOX_TEXT_COLOR;
        public static readonly Vector4 MESSAGE_BOX_ERROR_BACKGROUND_COLOR = new Vector4(0.7f, 0.2f, 0.2f, 0.95f);
        public static readonly Vector4 MESSAGE_BOX_ERROR_BUTTON_BACKGROUND_COLOR = new Vector4(0.85f, 0, 0, 0.75f);
        public static readonly Vector4 MESSAGE_BOX_ERROR_TEXT_COLOR = (new Color(255, 0, 0, 240)).ToVector4();
        public static readonly Vector4 MESSAGE_BOX_ERROR_ROTATING_WHEEL_COLOR = MESSAGE_BOX_ERROR_TEXT_COLOR;

        public static readonly Vector4 MESSAGE_BOX_MESSAGE_BACKGROUND_COLOR = new Vector4(1f, 1f, 1f, 1f);
        public static readonly Vector4 MESSAGE_BOX_MESSAGE_BUTTON_BACKGROUND_COLOR = (new Color(240, 240, 240, 255)).ToVector4();
        public static readonly Vector4 MESSAGE_BOX_MESSAGE_TEXT_COLOR = (new Color(240, 240, 240, 255)).ToVector4();
        public static readonly Vector4 MESSAGE_BOX_MESSAGE_ROTATING_WHEEL_COLOR = MESSAGE_BOX_MESSAGE_TEXT_COLOR;


        public static readonly Vector4 MESSAGE_BOX_NULL_TEXT_COLOR = Vector4.One;
        public static readonly Vector4 MESSAGE_BOX_NULL_BACKGROUND_COLOR = new Vector4(1f, 1f, 1f, 0.95f);
        public static readonly Vector4 MESSAGE_BOX_NULL_BUTTON_BACKGROUND_COLOR = Vector4.One;
        public static readonly Vector4 MESSAGE_BOX_NULL_ROTATING_WHEEL_COLOR = Vector4.One;
        public static readonly Vector4 MESSAGE_BOX_NULL_BACKGROUND_INTERFERENCE_VIDEO_COLOR = Vector4.One;



        //inventory 
        public static readonly Vector2 INVENTORY_FILTER_BUTTON_INNER_SIZE = new Vector2(50 / 1600f, 50 / 1200f);
        public static readonly Vector2 INVENTORY_FILTER_BUTTON_SIZE = new Vector2(64 / 1600f, 64 / 1200f);
        public const float INVENTORY_LABEL_TEXT_SCALE = 0.8f;//0.73f

        // Editor Toolbar constants
        public static readonly Vector2 TOOLBAR_PADDING = new Vector2(0.03f, 0.04f) / 7.5f;
        public static readonly Vector2 TOOLBAR_BUTTON_SIZE = new Vector2(0.03f, 0.04f) * 1.38f;
        public const float TOOLBAR_BUTTON_OFFSET = -0.0035f;
        public static readonly Vector4 TOOLBAR_TEXT_COLOR = new Color(255, 255, 255, 255).ToVector4();
        public static readonly float TOOLBAR_TEXT_SCALE = 0.60f;

        // Editor Snap Points Constants
        public static readonly Vector4 SNAP_POINT_COLOR = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        public static readonly Vector4 SNAP_POINT_LINKED_COLOR = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        public static readonly Vector4 SNAP_POINT_MULTI_LINKED_COLOR = new Vector4(1.0f, 0.55f, 0.0f, 1.0f);
        public static readonly Vector4 SNAP_POINT_SELECTED_COLOR = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);

        // Tool tips constants:
        public static Vector2 TOOL_TIP_RELATIVE_DEFAULT_POSITION = new Vector2(0.03f, 0.03f);
        public const float TOOL_TIP_RELATIVE_FOG_POSITION_DIFF = 0.1f;
        public const float TOOL_TIP_TEXT_SCALE = 0.7f;
        public static Vector2 TOOL_TIP_BACKGROUND_SCALE = new Vector2(1.6f, 4.6f/*4.0f*/);//new Vector2(2.5f,2.0f);
        public static Vector2 TOOL_TIP_BACKGROUND_FADE_SCALE = new Vector2(1.1f, 1.2f);
        public static Color TOOL_TIP_TEXT_COLOR = Color.AntiqueWhite;
        public static Color TOOL_TIP_BACKGROUND_COLOR = new Color(0.02f, 0.24f, 116/255f, 0.5f);
        public static Color TOOL_TIP_BORDER_COLOR = Color.CornflowerBlue; // new Color(120/255f, 205/255f, 245/255f, 0.95f);
        //new Color(MyGuiConstants.LABEL_TEXT_COLOR);//new Color(200, 200, 200, 160);//new Color(255, 255, 255, 80);
        public static Color TOOL_TIP_BACKGROUND_FADE_COLOR = new Color(0, 0, 0, 255);

        // Control wheel constants
        public static Color CONTROL_WHEEL_FOG_FADE_COLOR = new Color(0, 0, 0, 255);
        public static Color CONTROL_WHEEL_FOG_COLOR = new Color(44, 44, 44, 228);
        public static Vector2 CONTROL_WHEEL_RESCALE = new Vector2(1.9795f, 1.35433431f);

        // Select ammo select constants
        public static readonly Vector4 SELECT_AMMO_BACKGROUND_COLOR = new Vector4(0.949f, 0.949f, 0.941f, 0.9f);//new Vector4(0.77f, 0.97f, 0.97f, 0.5f);
        public static readonly Vector4 AMMO_SELECT_BACKGROUND_COLOR_NONSELECT = new Vector4();
        public const float SELECT_AMMO_ACTIVE_COLOR_MULTIPLIER = 5.0f;//1.8f;
        public static Color SELECT_AMMO_TIP_BACKGROUND_FADE_COLOR = new Color(0, 0, 0, 255);
        public static Color SELECT_AMMO_TIP_BACKGROUND_FADE_COLOR_FOG = new Color(0, 0, 0, 140);
        public static Color SELECT_AMMO_TIP_BACKGROUND = new Color(220, 220, 220, 140);
        public static Color SELECT_AMMO_TIP_TEXT_COLOR = new Color(193,136,36,255);
        public static Vector2 SELECT_AMMO_TIP_RELATIVE_DEFAULT_POSITION = new Vector2(0.025f, -0.045f);
        public const float AMMO_SELECT_MENU_ITEM_WIDTH = 0.05f;
        public const float  AMMO_SELECT_SCALE = 0.75f;
        public const float AMMO_SELECT_BACKGROUND_SCALE = 1.8745f;
        public const float AMMO_SELECT_MENUITEM_DISTANCE_NORMALIZED = 0.01f;
        public const float AMMO_SELECT_ITEM_DISTANCE_NORMALIZED = 0.001f;//0.01f;
        public static Vector2 AMMO_SELECT_ITEM_TEXT_RELATIVE_POSITION = new Vector2(0.89f, 0.93f);//at right bottom new Vector4(1.0f,1.0f)
        public static Vector2 AMMO_SELECT_LEFT_TOP_POSITION = new Vector2(1.0f, 1.0f);
        public const float AMMO_SELECTION_CONFIRMATION_TEXT_SCALE = 0.8f;//0.7f;
        public const float AMMO_SELECTION_CONFIRMATION_TEXT_INFO_SCALE = 0.8f;//0.7f;
        public const float AMMO_SELECTION_TOOL_TIP_TEXT_SCALE = 0.7f; // 0.46f
        public static Vector4 AMMO_SELECTION_DEFAULT_AFTERSELECT_ICON_COLOR = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public static Vector4 AMMO_SELECTION_DEFAULT_AFTERSELECT_TEXT_COLOR = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public static Vector2 AMMO_SELECTION_DEFAULT_AFTERSELECT_ICON_SCALE = new Vector2(0.75f, 0.75f);
        public const float AMMO_SELECTION_CONFIRM_BORDER_INFO_SCALE = 0.7f;//0.64f;
        public static Vector2 AMMO_SELECTION_CONFIRM_BORDER_SCALE = new Vector2(/*1.3f, 0.975f*/1.0f, 1.0f);
        public static Vector2 AMMO_SELECTION_CONFIRM_BORDER_TEXT_INFO_POSITION = new Vector2(0.367f, 0.457f);
        public static Vector2 AMMO_SELECTION_CONFIRM_BORDER_TEXT_POSITION = new Vector2(0.632f, 0.5f);
        public static Vector2 AMMO_SELECTION_CONFIRM_BORDER_POSITION = new Vector2(0.5f, 0.5f);
        public static Color AMMO_SELECTION_CONFIRM_INFO_TEXT_COLOR = new Color(MyGuiConstants.LABEL_TEXT_COLOR);//Color.Red;
        public static Color AMMO_SELECTION_CONFIRM_BORDER_COLOR = new Color(255, 255, 255, 200);
        public const float AMMO_SELECTION_TIP_BACKGROUND_FADE_HEIGHT_SCALE = 0.48986f;

        //  How long takes transition of opening and closing of the screen - in miliseconds
        public const int TRANSITION_OPENING_TIME = 100;
        public const int TRANSITION_CLOSING_TIME = 100;

        //  Min and max values for transition alpha, where max is alpha when screen is fully active
        public const float TRANSITION_ALPHA_MIN = 0;
        public const float TRANSITION_ALPHA_MAX = 1;

        //  Loading - random screen index (this is about Background001.png...). Max is the number of highest file.
        public const int LOADING_RANDOM_SCREEN_INDEX_MIN = 1;
        public const int LOADING_RANDOM_SCREEN_INDEX_MAX = 1;

        //  This is the size of background's texture. I hardcoded it here because these textures are resized to power of two, so getting
        //  height/width from texture will return not usable results. So, if you ever change this texture, change this param too.
        public static readonly MyMwcVector2Int LOADING_BACKGROUND_TEXTURE_REAL_SIZE = new MyMwcVector2Int(1920, 1080);

        //  This is the size of Miner War's logo texture. I hardcoded it here because these textures are resized to power of two, so getting
        //  height/width from texture will return not usable results. So, if you ever change this texture, change this param too.
        //public static readonly MyMwcVector2Int MINER_WARS_LOGO_TEXTURE_REAL_SIZE = new MyMwcVector2Int(1500, 330);

        //  Slow down drawing of "loading..." screens, so background thread who is actualy right now loading content will have more time
        //  Plus these two threads won't fight for graphic device. It's not big difference, just 10% or so.
        public const int LOADING_THREAD_DRAW_SLEEP_IN_MILISECONDS = 10;

        // Gui Video constants
        public const int BACKGROUND_INTERFERENCE_VIDEO_REPEAT_MIN = 6000;
        public const int BACKGROUND_INTERFERENCE_VIDEO_REPEAT_MAX = 6000;
        public const byte BACKGROUND_INTERFERENCE_VIDEO_PIXELS_EXTENSION = 25;
        public static readonly Vector4 MESSAGE_BOX_ERROR_BACKGROUND_INTERFERENCE_VIDEO_COLOR = new Vector4(1f, 0f, 0f, 1f);
        public static readonly Vector4 MESSAGE_BOX_MESSAGE_BACKGROUND_INTERFERENCE_VIDEO_COLOR = Vector4.One;
        public static readonly Vector4 DEFAULT_BLUE_BACKGROUND_INTERFERENCE_VIDEO_COLOR = new Vector4(0.77f, 0.97f, 0.99f, 0.8f);

        // Color of notification "You can trade with..."
        public static readonly Vector4 TRADE_NOTIFICATION_FRIEND_COLOR = new Vector4(0.1f, 0.7f, 0.1f, 0.5f);
        public static readonly Vector4 TRADE_NOTIFICATION_NEUTRAL_COLOR = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        public static readonly Vector4 LOOT_NOTIFICATION_COLOR = new Vector4(0.7f, 0.1f, 0.1f, 0.5f);

        // Colored texts contstants
        public const float COLORED_TEXT_DEFAULT_TEXT_SCALE = 0.75f;
        public static readonly Color COLORED_TEXT_DEFAULT_COLOR = new Color(DEFAULT_CONTROL_NONACTIVE_COLOR);
        public static readonly Color COLORED_TEXT_DEFAULT_HIGHLIGHT_COLOR = new Color(MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER * DEFAULT_CONTROL_NONACTIVE_COLOR);

        // Multiline label constants
        public static readonly Vector2 MULTILINE_LABEL_BORDER = new Vector2(0.01f, 0.0050f);
        public static readonly Vector4 MULTILINE_LABEL_BACKGROUND_COLOR = new Vector4(0.25f, 0.35f, 0.375f, 0.9f);

        public static readonly float DEBUG_LABEL_TEXT_SCALE = 1.0f;
        public static readonly float DEBUG_BUTTON_TEXT_SCALE = 0.8f;
        public static readonly float DEBUG_STATISTICS_TEXT_SCALE = 0.75f;
        public static readonly float DEBUG_STATISTICS_ROW_DISTANCE = 0.020f;

        public static readonly string REMOTE_VIEW_LARGE_WEAPON_AMMO = "ammo";
        public static readonly string REMOTE_VIEW_LARGE_WEAPON_BOTTOM = "bottom";
        public static readonly string REMOTE_VIEW_LARGE_WEAPON_CROSS = "cross";
        public static readonly string REMOTE_VIEW_LARGE_WEAPON_LEFT_SIDE = "left_side";
        public static readonly string REMOTE_VIEW_LARGE_WEAPON_PULSE = "pulse";
        public static readonly string REMOTE_VIEW_LARGE_WEAPON_RASTR = "rastr";
        public static readonly string REMOTE_VIEW_LARGE_WEAPON_RIGHT_SIDE = "right_side";

        public static readonly string REMOTE_VIEW_DRONE_BOTTOM = "bottom";
        public static readonly string REMOTE_VIEW_DRONE_CROSS = "cross";
        public static readonly string REMOTE_VIEW_DRONE_LEFT_SIDE = "left_side";
        public static readonly string REMOTE_VIEW_DRONE_RASTR = "rastr";
        public static readonly string REMOTE_VIEW_DRONE_RIGHT_SIDE = "right_side";

        public static readonly string REMOTE_VIEW_CAMERA_BOTTOM = "bottom";
        public static readonly string REMOTE_VIEW_CAMERA_FOCUS = "focus";
        public static readonly string REMOTE_VIEW_CAMERA_LEFT_SIDE = "left_side";
        public static readonly string REMOTE_VIEW_CAMERA_RASTR = "rastr";
        public static readonly string REMOTE_VIEW_CAMERA_REC = "rec";
        public static readonly string REMOTE_VIEW_CAMERA_RIGHT_SIDE = "right_side";

        public static readonly int CHAT_WINDOW_MAX_MESSAGES_COUNT = 10;
        public static readonly int CHAT_WINDOW_MAX_MESSAGE_LENGTH = 28;
        public static readonly float CHAT_WINDOW_MESSAGE_SCALE = 0.75f;
        public static readonly Vector2 CHAT_WINDOW_POSITION = new Vector2(0.5f, 0.96f);
        public static readonly Color CHAT_WINDOW_BACKGROUND_COLOR = new Color(0.25f, 0.25f, 0.25f, 0.25f);
        public static readonly Color CHAT_WINDOW_TEXT_COLOR = new Color(0.75f, 0.75f, 0.75f, 0.75f);
        public static readonly float CHAT_WINDOW_MESSAGE_TTL = 60000;

        public const float FONT_SCALE = 28.8f / 37f;  // Ratio between font size and line height has changed: old was 28, new is 37 (28.8 makes it closer to the font size change 18->23)
        public const float FONT_TOP_SIDE_BEARING = 3 * 23f / 18f;  // This is exact: old font size was 18, new font size 23, X padding is 7 and Y padding is 4, so (7-4)*23/18
    }

    //  Objects in the background sphere (aka skysphere)
    static class MyBgrCubeConsts
    {
        // TODO: Remove, use MyMwcSectorConstants
        public const float SECTOR_SIZE = MyMwcSectorConstants.SECTOR_SIZE / 1000;
        public const float BILLBOARD_SCALE = 1.0f;       //  We scale every background billboard by this factor, because objects seemed to be smaller than in reality. And it looks nice.
        public const int STARS_COUNT = 100000;
        public const int STARS_BRIGHT_COUNT = 10000;

        public const int DUST_MILKEY_WAY_COUNT = 2000;
        public const int DUST_SMALL_COUNT = 1000;
        public const int DUST_COUNT_TOTAL = DUST_MILKEY_WAY_COUNT + DUST_SMALL_COUNT;

        public const int TEXTURE_WIDTH_AND_HEIGHT = 2048;

        public const float MILLION_KM = 1000 * 1000;
        //  Object positions and radiuses are in millions of km (e.g. earth to sun is 150.000.000 km, therefore here its just 150)
        //  Don't forget these are radiuses, not diameters!!!
        //  Positions are in cartezian system (x, y, z), relative to solar system center (sun)
        //  Player isn't in the middle (point 0, 0, 0). There is sun. So player is between sun and earth.
        //  Distance from earth to moon is 384,000 km
        public static readonly Vector3 MERCURY_POSITION = new Vector3(-39, 0.0f, 46);
        public static readonly Vector3 LAIKA_POSITION = new Vector3(-186, 0.0f, -115);
        //public static readonly Vector3 MERCURY_POSITION = new Vector3(60, 0.2f, -0.105f);
        //public static readonly Vector3 VENUS_POSITION = new Vector3(-2, -0.5f, 108);
        //public static readonly Vector3 VENUS_POSITION = new Vector3(-2, -0.5f, -108);
        //public static readonly Vector3 EARTH_POSITION = new Vector3(105, 0, -105);
        public static readonly Vector3 VENUS_POSITION = new Vector3(-2, 0.0f, 108);
        public static readonly Vector3 EARTH_POSITION = new Vector3(101, 0, -111);
        public static readonly Vector3 MOON_POSITION = EARTH_POSITION + new Vector3(-0.015f, 0.0f, -0.2f);
        public static readonly Vector3 MARS2_POSITION = new Vector3(-140, 0, -230);
        public static readonly Vector3 MARS_POSITION = new Vector3(-182, 0, 114);
        public static readonly Vector3 NEBULA_POSITION = new Vector3(209f, 0f, -110f);
        public static readonly Vector3 RUSSIAN_POSITION = new Vector3(-249.4f, 0f, -43.28f);
        public static readonly Vector3 MARS3_POSITION = new Vector3(177, 0, -118);
        public static readonly Vector3 PIRATES2_POSITION = new Vector3(485, 0, -105);
        public static readonly Vector3 SLAVERS2_POSITION = new Vector3(102, 0, -526);
        public static readonly Vector3 SLAVERS_POSITION = new Vector3(208f, 0f, -410f);
        public static readonly Vector3 JUPITER_POSITION = new Vector3(-778f, 0.0f, 155.6f);
        public static readonly Vector3 JUPITERBORDER_POSITION = new Vector3(0f, 0.0f, -800.6f);
        public static readonly Vector3 JUPITER2_POSITION = new Vector3(719f, 0.0f, 146.9f);
        public static readonly Vector3 SATURN_POSITION = new Vector3(1120f, 0.0f, -840f);
        public static readonly Vector3 URANUS_POSITION = new Vector3(-2700f, 0.0f, -1500f);
        public static readonly Vector3 NEPTUNE_POSITION = new Vector3(1350f, 0.0f, 4050f);
        public static readonly Vector3 CHINESE_POSITION = new Vector3(-59.3f, 0.0f, 191.2f);
        public static readonly Vector3 MEDINA622_POSITION = new Vector3(201.4f, 0.0f, 39.5f);
        public static readonly MyMwcVector3Int JUNKYARD_SECTOR = new MyMwcVector3Int(2567538, 0, -172727);
        public static readonly MyMwcVector3Int RIME_SECTOR = new MyMwcVector3Int(-1922856, 0, -2867519);
        public static readonly MyMwcVector3Int CHINESERAFINARY_SECTOR = new MyMwcVector3Int(-2716080, 0, 4951053);
        public static readonly MyMwcVector3Int CHINESEMINES_SECTOR = new MyMwcVector3Int(-4274372, 0, 4874227);

        // Default sun color (when not influented by any dust/asteroid field or fog)
        public static readonly Vector3 SUN_COLOR = new Vector3(1, 1, 1);

        // Anywhere on the belt
        public static readonly float ASTEROID_BELT_DISTANCE = (MyBgrCubeConsts.MARS_POSITION.Length() + MyBgrCubeConsts.JUPITER_POSITION.Length()) / 2.6f;
        public static readonly Vector3 ASTEROID_BELT_POSITION = -Vector3.UnitZ * ASTEROID_BELT_DISTANCE;

        public static readonly float SUN_RADIUS = 230 / MILLION_KM;
        public static readonly float MERCURY_RADIUS = 2439 / MILLION_KM;
        public static readonly float VENUS_RADIUS = 6000 / MILLION_KM;
        public static readonly float EARTH_RADIUS = 6371 / MILLION_KM;
        public static readonly float MOON_RADIUS = 1737 / MILLION_KM;
        public static readonly float MARS_RADIUS = 3376 / MILLION_KM;
        public static readonly float MARS3_RADIUS = 2354 / MILLION_KM;
        public static readonly float JUPITER_RADIUS = 69911 / MILLION_KM;
        public static readonly float JUPITER2_RADIUS = 3987/ MILLION_KM;
        public static readonly float SMALLPIRATEBASE2_RADIUS = 1524 / MILLION_KM;
        public static readonly float SLAVERSBASE2_RADIUS = 1524 / MILLION_KM;
        public static readonly float SATURN_RADIUS = 60268 / MILLION_KM;
        public static readonly float URANUS_RADIUS = 25559 / MILLION_KM;
        public static readonly float NEPTUNE_RADIUS = 24764 / MILLION_KM;
        public static readonly float NEBULA_RADIUS = 1737 / MILLION_KM;
        public static readonly float RUSSIANTRANSMITTER_RADIUS = 1735 / MILLION_KM;
        public static readonly float SLAVERBASE_RADIUS = 1736 / MILLION_KM;

        // Double the real size (gravity stuff etc...)
        //public static readonly float SUN_RADIUS = EARTH_RADIUS * 109 * 2;

        /*
        public static readonly float MARS_SUN_DISTANCE = 230;
        public static readonly float JUPITER_SUN_DISTANCE = 778;
        public static readonly float SATURN_SUN_DISTANCE = 1400;
        public static readonly float URANUS_SUN_DISTANCE = 3000;
        public static readonly float NEPTUNE_SUN_DISTANCE = 4500;
        */

        //  This is sphere we map stars to (we don't use real distances because I am afraid of far clipping plane and float precision problems)
        //  In millions of km
        public const float CELESTIAL_SPHERE_RADIUS = 500;

        public const float NEAR_PLANE_DISTANCE = 0.01f;
        public const float FAR_PLANE_DISTANCE = CELESTIAL_SPHERE_RADIUS * 2;
    }

    static class MyHeadShakeConstants
    {
        public const float HEAD_SHAKE_AMOUNT_AFTER_GUN_SHOT = 2.6f;
        public const float HEAD_SHAKE_AMOUNT_AFTER_EXPLOSION = 20;
        public const float HEAD_SHAKE_AMOUNT_AFTER_PROJECTILE_HIT = 5;
        public const float HEAD_SHAKE_AMOUNT_AFTER_SMOKE_PUSH = 2;

        //  Sun wind head shaking
        public const float HEAD_SHAKE_AMOUNT_DURING_SUN_WIND_MIN = 0;
        public const float HEAD_SHAKE_AMOUNT_DURING_SUN_WIND_MAX = 1;
    }

    public enum MyDamageType
    {
        Unknown,
        Explosion,
        Rocket,
        Bullet,
        Mine,
        Sunwind,
        Drill,
        Radioactivity
    }

    public enum MyAmmoType
    {
        Unknown,
        Basic,
        HighSpeed,
        Piercing,
        Biochem,
        Explosive,
        EMP,
        None,
    }

    static class MyProjectilesConstants
    {
        //  Max count of active (aka flying) projectiles
        public const int MAX_PROJECTILES_COUNT = 8192;
        public const float HIT_STRENGTH_IMPULSE = 500;

        public static readonly Vector3 EXPLOSIVE_PROJECTILE_TRAIL_COLOR = new Vector3(1.0f, 0.5f, 0.5f);
        public static readonly Vector3 HIGH_SPEED_PROJECTILE_TRAIL_COLOR = new Vector3(10.0f, 10.0f, 10.0f);
        public static readonly Vector3 BIOCHEM_PROJECTILE_TRAIL_COLOR = new Vector3(0.5f, 2.5f, 0.5f);
        public static readonly Vector3 PIERCING_PROJECTILE_TRAIL_COLOR = new Vector3(0.5f, 0.5f, 1.5f);
        public static readonly Vector3 EMP_PROJECTILE_TRAIL_COLOR = new Vector3(0.5f, 0.5f, 2.5f);

        public static readonly float AUTOAIMING_PRECISION = 500.0f;
    }

    static class MyAutocanonConstants
    {
        //  How many times machine gun rotates while firing.
        public const float ROTATION_SPEED_PER_SECOND = 2 * MathHelper.TwoPi;

        //  How long it takes until autocanon stops rotating after last shot
        public const int ROTATION_TIMEOUT = 2000;

        //  Interval between two machine gun shots
        public const int SHOT_INTERVAL_IN_MILISECONDS = 95;

        //  Interval between two machine gun shots
        public const int MIN_TIME_RELEASE_INTERVAL_IN_MILISECONDS = 204;

        public static readonly float SHOT_PROJECTILE_DEBRIS_MAX_DEVIATION_ANGLE = MathHelper.ToRadians(30);

        public static readonly float COCKPIT_GLASS_PROJECTILE_DEBRIS_MAX_DEVIATION_ANGLE = MathHelper.ToRadians(10);

        public const int SMOKE_INCREASE_PER_SHOT = SHOT_INTERVAL_IN_MILISECONDS * 2 / SMOKES_INTERVAL_IN_MILISECONDS;
        public const int SMOKE_DECREASE = 1;
        public const int SMOKES_MAX = 50;
        public const int SMOKES_MIN = 40;

        //  This number is not dependent on rate of shoting. It tells how often we generate new smoke (if large, it will look ugly)
        public const int SMOKES_INTERVAL_IN_MILISECONDS = 10;
    }

    static class MyMachineGunConstants
    {
        //Interval between two shots
        public const float SHOT_INTERVAL_IN_MILISECONDS = 137;

        //  this is time when we stop looping sound and play rel sound for machine gun
        public const float RELEASE_TIME_AFTER_FIRE = 250;

        public const int MUZZLE_FLASH_MACHINE_GUN_LIFESPAN = 40;
    }

    //Automatic_Rifle_With_Silencer
    static class MyARSConstants
    {
        //Interval between two shots
        public const float SHOT_INTERVAL_IN_MILISECONDS = 500;
    }

    //shotgun
    static class MyShotgunConstants
    {
        // Interval beween two shots
        public const float SHOT_INTERVAL_IN_MILISECONDS = 500.0f;

        // Percentual reliability of explosive projectiles. Failed projectiles just smoke, not explode.
        public const float EXPLOSIVE_PROJECTILE_RELIABILITY = 0.85f;

        public const int PROJECTILE_GROUP_SIZE = 2;
    }

    static class MySniperConstants
    {
        //Interval between two shots
        public const float SHOT_INTERVAL_IN_MILISECONDS = 1000;

        //  Sniper gun should be very precise so add only minimal deviation
        public static readonly float RIFLE_DEVIATE_PROJECTILE_DIRECTION_RANDOM_ANGLE = MathHelper.ToRadians(0.01f);
    }

    static class MyMissileConstants
    {
        //  We will generate smoke trail particles on missile's way. This number tells us how many particles per 1 meter.
        public const float GENERATE_SMOKE_TRAIL_PARTICLE_DENSITY_PER_METER = 4f;

        //  This number needs to be calculated in regard to max count of player, max count of missiles fired per second and timeout of each missile
        public const int MAX_MISSILES_COUNT = 500;

        public const int MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS = 900;            //  Interval between two missile launcher shots
        public const float MISSILE_BLEND_VELOCITIES_IN_MILISECONDS = 100.0f;
        public const int MISSILE_TIMEOUT = 3 * 1000;       //  Max time missile can survive without hiting any object

        public static readonly Vector4 MISSILE_LIGHT_COLOR = new Vector4(1.5f, 1.5f, 1.0f, 1.0f);       //  Alpha should be 1, because we draw flare billboard with it

        public const int MISSILE_INIT_TIME = 10; //ms
        public static readonly Vector3 MISSILE_INIT_DIR = MyMwcUtils.Normalize(new Vector3(0, 0, -1));

        public const float MISSILE_MINIMAL_HIT_DOT = 0.8f; //1 is direct full hit, 0 is hit by side and -1 is hit by missile back hit

        public const float HIT_STRENGTH_IMPULSE = 700000;

        /// <summary>
        /// The distance to check whether missile will collide soon after launch with something.
        /// For more info, see ticket 3422.
        /// </summary>
        public const int DISTANCE_TO_CHECK_MISSILE_CORRECTION = 10;

        public const float MISSILE_LIGHT_RANGE = 70;
    }

    class MyGuidedMissileConstants
    {
        //  We will generate smoke trail particles on missile's way. This number tells us how many particles per 1 meter.
        public const float GENERATE_SMOKE_TRAIL_PARTICLE_DENSITY_PER_METER = 4f;

        //  This number needs to be calculated in regard to max count of player, max count of missiles fired per second and timeout of each missile
        public const int MAX_MISSILES_COUNT = 50;

        public const int MISSILE_LAUNCHER_SHOT_INTERVAL_IN_MILISECONDS = 1000;            //  Interval between two missile launcher shots
        public const float MISSILE_BLEND_VELOCITIES_IN_MILISECONDS = 400.0f; //time to get full speed from init speed
        public const int MISSILE_TIMEOUT = 15 * 1000;       //  Max time missile can survive without hiting any object

        public static readonly Vector4 MISSILE_LIGHT_COLOR = new Vector4(1.5f, 1.5f, 1.0f, 1.0f);       //  Alpha should be 1, because we draw flare billboard with it

        public static float MISSILE_TURN_SPEED = 5.0f; //max radians per second

        public const int MISSILE_INIT_TIME = 500; //ms
        public static readonly Vector3 MISSILE_INIT_DIR = new Vector3(0, -0.5f, -10.0f);

        public const float MISSILE_PREDICATION_TIME_TRESHOLD = 0.1f; //if time to hit is lower than this treshold, missille navigates directly to target

        public const int MISSILE_TARGET_UPDATE_INTERVAL_IN_MS = 100;
        public const float VISUAL_GUIDED_MISSILE_FOV = 40.0f;
        public const float VISUAL_GUIDED_MISSILE_RANGE = 1000.0f;
        public const float ENGINE_GUIDED_MISSILE_RADIUS = 200.0f;
    }

    static class MyMissileLauncherConstants
    {
        public const int GENERATE_SMOKE_TRAIL_PARTICLES_FOR_MILISECONDS = 250;
        public const float GENERATE_SMOKE_TRAIL_PARTICLES_DENSITY_PER_METER = 0.5f;
    }

    static class MyCannonConstants
    {
        public const float DEVIATE_DIRECTION_RANDOM_ANGLE = 0.01f;
        public const int SHOT_TIMEOUT = 5 * 1000;       //  Max time shot can survive without hiting any object
        public const int SMOKE_TIMEOUT = SHOT_TIMEOUT;
        public const float GENERATE_SMOKE_TRAIL_PARTICLE_DENSITY_PER_METER = 4f;
        public const int SHOT_INTERVAL_IN_MILISECONDS = 500; // Interval between two shots 
    }

    static class MyCannonShotConstants
    {
        //Special
        public const float PROXIMITY_DETECTION_RADIUS = 5f;
        public const int PROXIMITY_SHRAPNELS_COUNT = 20;
        public const float BUSTER_PENETRATION_LENGTH = 50;//pz: adjusted from value 60 @ http://mantis.keenswh.com/view.php?id=938

        public static readonly Vector4 LIGHT_COLOR = new Vector4(0.3f, 1.4f, 1.5f, 1.0f);       //  Alpha should be 1, because we draw flare billboard with it
    }

    static class MyUniversalLauncherConstants
    {
        public const bool USE_SPHERE_PHYSICS = true;
        public const int MAX_SMARTMINES_COUNT = 20;
        public const int MAX_BASICMINES_COUNT = 20;
        public const int MAX_BIOCHEMMINES_COUNT = 20;
        public const int MAX_SPHEREEXPLOSIVES_COUNT = 20;
        public const int MAX_DECOYFLARES_COUNT = 20;
        public const int MAX_FLASHBOMBS_COUNT = 20;
        public const int MAX_ILLUMINATINGSHELLS_COUNT = 20;
        public const int MAX_SMOKEBOMBS_COUNT = 20;
        public const int MAX_ASTEROIDKILLERS_COUNT = 20;
        public const int MAX_DIRECTIONALEXPLOSIVES_COUNT = 20;
        public const int MAX_TIMEBOMBS_COUNT = 20;
        public const int MAX_REMOTEBOMBS_COUNT = 20;
        public const int MAX_GRAVITYBOMBS_COUNT = 20;
        public const int MAX_HOLOGRAMS_COUNT = 10;
        public const int MAX_REMOTECAMERAS_COUNT = 20;
        public const int MAX_EMPBOMB_COUNT = 20;
    }

    static class MyMinerShipConstants
    {
        public static readonly Vector3 PLAYER_HEAD_MAX_SPRING_DISTANCE = new Vector3(0.10f, 0.05f, 0.1f);
        public static readonly Vector3 PLAYER_HEAD_MAX_DISTANCE = new Vector3(0.15f, 0.08f, 0.2f);
        public static readonly float PLAYER_HEAD_MAX_DISTANCE_LENGTH = PLAYER_HEAD_MAX_DISTANCE.Length();

        public const float GUNS_SEPARATION_DISTANCE = 1.6f;
        public const float GUNS_MOVE_BACKWARDS_DISTANCE = 2.0f;

        public static readonly float MINER_SHIP_NEAR_LIGHT_RANGE = 160.0f;
        public static readonly float MINER_SHIP_NEAR_LIGHT_INTENSITY = 1.0f;
        public static readonly float MINER_SHIP_LIGHT_FALLOFF = 10;
        public static readonly float MINER_SHIP_LIGHT_RADIUS_OUTSIDE_MODIFIER = 0.5f;

        // Texture is applied to reflector, so texture is used to set proper intensity
        public static readonly float MINER_SHIP_NEAR_REFLECTOR_INTENSITY = 1.0f;
        public static readonly float MINER_SHIP_NEAR_REFLECTOR_FALLOFF = 5.0f;

        public static readonly float MINER_SHIP_PLAYER_NEAR_LIGHT_RANGE_MULTIPLIER = 1.0f;

        //public static readonly Vector4 MINER_SHIP_MUZZLE_FLASH_LIGHT_COLOR = new Vector4(4 * 1.0f, 4 * 0.7f, 4 * 0.0f, 1.0f);
        public static readonly Vector4 MINER_SHIP_MUZZLE_FLASH_LIGHT_COLOR = new Vector4(3 * 255.0f / 255.0f, 3 * 227.0f / 255.0f, 3 * 125.0f / 255.0f, 1.0f);

        public static readonly float MINER_SHIP_MUZZLE_FLASH_LIGHT_RANGE = MINER_SHIP_NEAR_LIGHT_RANGE;//60.0f;

        public static readonly int MINER_SHIP_ENGINE_IDLE_CUE_DELAY_IN_MILLIS = 1340;//1480
        public static readonly int MINER_SHIP_ENGINE_SWITCH_MIN_REPEAT_TRESHOLD = 1792;

        public static readonly float MINER_SHIP_PROJECTILE_HIT_MULTIPLIER = 6.0f;
        public static readonly float MINER_SHIP_PLAYER_PROJECTILE_HIT_MULTIPLIER = 10.0f;

        /// <summary>
        /// Represents player health loss value per second, if player doesn't have enough oxygen
        /// </summary>
        public static readonly float MINER_SHIP_PLAYER_NO_OXYGEN_HEALTH_LOSS = 2.0f;
    }

    static class MyModelsConstants
    {
        public const int MAX_ENTITIES_TO_DRAW = 2000;
        public const float DEBRIS_FIELD_MAX_RENDER_DISTANCE = 3000;
    }

    static class MyInstancingConstants
    {
        public const int POINT_LIGHT_BUFFER_SIZE = 350;
        public const int HEMISPHERIC_LIGHT_BUFFER_SIZE = 128;
        public const int SPOT_LIGHT_BUFFER_SIZE = 128;
        public const int SPOTSHIP_LIGHT_BUFFER_SIZE = 32;
    }

    static class MyExplosionsConstants
    {
        //  Max possible radius for explosions. This is for caching voxels during calculating explosions.
        public const float EXPLOSION_RADIUS_MAX = 100;

        //  Max number of explosions we can have in a scene. This number doesn't mean we will update/draw this explosions. It's just that we can hold so many explosions.
        public const int MAX_EXPLOSIONS_COUNT = 256;

        public const int EXPLOSION_DEBRIS_LIVING_MAX_IN_MILISECONDS = 7000;//4000;//8000;                                                  //  How long explosion debris object lives (in miliseconds)
        public const int EXPLOSION_DEBRIS_LIVING_MIN_IN_MILISECONDS = 5000;//EXPLOSION_DEBRIS_VOXEL_LIVING_MAX_IN_MILISECONDS - 500;//2000;     //  How long explosion debris object lives (in miliseconds)

        public const int MAX_EXPLOSION_DEBRIS_OBJECTS = MyFakes.MANY_DEBRIS ? 200 : 32;
        public const int MIN_OBJECT_SIZE_TO_CAUSE_EXPLOSION_AND_CREATE_DEBRIS = 5;

        // only approximate, will always be higher (see MyExplosionDebrisModel.GeneratePositions for usage)
        public const int APPROX_NUMBER_OF_DEBRIS_OBJECTS_PER_MODEL_EXPLOSION = 3;
        public const float EXPLOSION_DEBRIS_SPEED = 200.0f;

        public const float OFFSET_LINE_FOR_DIRT_DECAL = 0.5f;

        public const float EXPLOSION_STRENGTH_IMPULSE = 1000000;
        public const float EXPLOSION_STRENGTH_ANGULAR_IMPULSE = 50000000;
        public const float EXPLOSION_STRENGTH_ANGULAR_IMPULSE_PLAYER_MULTIPLICATOR = 0.25f;
        public const float EXPLOSION_RADIUS_MULTPLIER_FOR_IMPULSE = 1f;          //  If we multiply this number by explosion radius (which is used for cuting voxels and drawing particles), we get radius for applying throwing force to surounding objects
        public const float EXPLOSION_RADIUS_MULTPLIER_FOR_DIRT_GLASS_DECALS = 3;          //  If we multiply this number by explosion radius (which is used for cuting voxels and drawing particles), we get radius for applying dirt decals on ship glass
        public const float EXPLOSION_RANDOM_RADIUS_MAX = 25;
        public const float EXPLOSION_RANDOM_RADIUS_MIN = EXPLOSION_RANDOM_RADIUS_MAX * 0.8f;
        public const int EXPLOSION_LIFESPAN = 700;
        public const float EXPLOSION_CASCADE_FALLOFF = 0.33f; // for cascading explosions (e.g. missiles in a row), this is the explosion influence radius multiplier for each level of the cascade

        //public static readonly Vector4 EXPLOSION_LIGHT_COLOR = new Vector4(154.0f / 255.0f * 6.0f, 83.0f / 255.0f * 6.0f, 63.0f / 255.0f * 6.0f, 1)
        public static readonly Vector4 EXPLOSION_LIGHT_COLOR = new Vector4(3 * 248.0f / 255.0f, 3 * 179.0f / 255.0f, 3 * 12.0f / 255.0f, 1);

        public const float CLOSE_EXPLOSION_DISTANCE = 15; // in meters. explosions closer than this value will look different

        public const int FRAMES_PER_SPARK = 30; // Applies for full damage ratio (100%). 50% has half frequency of sparks etc. Lower value - more frequent sparks
        public const float DAMAGE_SPARKS = 0.4f; // percentage for damage, above which spark effects get generated (e.g. for 0.4 -> when health is below 60%)
        public const float EXPLOSION_FORCE_RADIUS_MULTIPLIER = 0.33f;

        // prefabs that are supposed to have their explosion larger than this will have the 'huge' explosion particle effect
        public const int EXPLOSION_EFFECT_SIZE_FOR_HUGE_EXPLOSION = 300;
    }

    static class MyLodConstants
    {
        //  After this distance we won't draw guns and gun muzzle. It is optimalization. You must be careful especially for muzzle, becuase that billboard is visible more than gun's model.
        public const float MAX_DISTANCE_FOR_DRAWING_MINER_SHIP_GUNS = 100;
        public const float MAX_DISTANCE_FOR_RANDOM_ROTATING_LARGESHIP_GUNS = 600;
    }

    static class MyLightsConstants
    {
        //  Max number of lights we can have in a scene. This number doesn't mean we will draw this lights. It's just that we can hold so many lights.
        public const int MAX_LIGHTS_COUNT = 4000;

        //  Max number of lights we use for sorting them. Only this many lights can be in influence distance.
        //  This number doesn't have to be same as 'max for effect'. It should be more, so sorting can be nice.
        //  Put there: 2 * 'max for effect'
        public const int MAX_LIGHTS_COUNT_WHEN_DRAWING = 16;

        //  This number tells us how many light can be enabled during drawing using one effect. 
        //  IMPORTANT: This number is also hardcoded inside of hlsl effect file.
        //  IMPORTANT: So if you change it here, change it too in MyCommonEffects.fxh
        //  It means, how many lights can player see (meaning light as lighted triangleVertexes, not light flare, etc).
        public const int MAX_LIGHTS_FOR_EFFECT = 8;

        // Maximum radius for all types of point lights. Any bigger value will assert
        public const int MAX_POINTLIGHT_RADIUS = 120;

        // Maximum bounding box diagonal for all types of spot lights. Any bigger value will assert
        // Diagonal size is influented by spot range and spot cone (and also by current camera angle - because of AABB)
        //public const int MAX_SPOTLIGHT_AABB_DIAGONAL = 2500;
        public const float MAX_SPOTLIGHT_RANGE = 1200;
        public const float MAX_SPOTLIGHT_SHADOW_RANGE = 200;
        public const float MAX_SPOTLIGHT_SHADOW_RANGE_SQUARED = MAX_SPOTLIGHT_SHADOW_RANGE * MAX_SPOTLIGHT_SHADOW_RANGE;
        public static readonly float MAX_SPOTLIGHT_ANGLE = 80;
        public static readonly float MAX_SPOTLIGHT_ANGLE_COS = 1.0f - (float)Math.Cos(MathHelper.ToRadians(MAX_SPOTLIGHT_ANGLE));
    }

    static class MyGlobalEventsConstants
    {
        public const int GLOBAL_EVENTS_HOUR_RATIO = 100; //how many times will global event update per hour
    }

    static class MySunWindConstants
    {
        public const float SUN_COLOR_INCREASE_DISTANCE = 10000;
        public const float SUN_COLOR_INCREASE_STRENGTH_MIN = 3;
        public const float SUN_COLOR_INCREASE_STRENGTH_MAX = 4;

        //  This is half of the sun wind's length, or in other words, it is distance from camera where sun wind starts, then 
        //  travels through camera and travels again to disapear. So full travel distance is two times this number.
        public const float SUN_WIND_LENGTH_TOTAL = 60000;//MyConstants.FAR_PLANE_DISTANCE * 2;        
        public const float SUN_WIND_LENGTH_HALF = SUN_WIND_LENGTH_TOTAL / 2;// MyConstants.FAR_PLANE_DISTANCE;

        public static readonly MyMwcVector2Int LARGE_BILLBOARDS_SIZE = new MyMwcVector2Int(10, 10);
        public static readonly MyMwcVector2Int LARGE_BILLBOARDS_SIZE_HALF = new MyMwcVector2Int(LARGE_BILLBOARDS_SIZE.X / 2, LARGE_BILLBOARDS_SIZE.Y / 2);
        public const float LARGE_BILLBOARD_RADIUS_MIN = 10000;
        public const float LARGE_BILLBOARD_RADIUS_MAX = 15000;
        public const float LARGE_BILLBOARD_DISTANCE = 7500; //LARGE_BILLBOARD_RADIUS_MIN * 2;
        public const float LARGE_BILLBOARD_POSITION_DELTA_MIN = -50;
        public const float LARGE_BILLBOARD_POSITION_DELTA_MAX = 50;
        public const float LARGE_BILLBOARD_ROTATION_SPEED_MIN = 0.5f;
        public const float LARGE_BILLBOARD_ROTATION_SPEED_MAX = 1.2f;
        public const float LARGE_BILLBOARD_DISAPEAR_DISTANCE = SUN_WIND_LENGTH_HALF * 0.9f;

        public static readonly MyMwcVector2Int SMALL_BILLBOARDS_SIZE = new MyMwcVector2Int(20, 20);
        public static readonly MyMwcVector2Int SMALL_BILLBOARDS_SIZE_HALF = new MyMwcVector2Int(SMALL_BILLBOARDS_SIZE.X / 2, SMALL_BILLBOARDS_SIZE.Y / 2);
        public const float SMALL_BILLBOARD_RADIUS_MIN = 300;
        public const float SMALL_BILLBOARD_RADIUS_MAX = 600;
        public const float SMALL_BILLBOARD_DISTANCE = 330;
        public const float SMALL_BILLBOARD_POSITION_DELTA_MIN = -50;
        public const float SMALL_BILLBOARD_POSITION_DELTA_MAX = 50;
        //public const float SMALL_BILLBOARD_MAX_DISTANCE_FROM_CENTER = LARGE_BILLBOARD_MAX_DISTANCE_FROM_CENTER;

        public const float SMALL_BILLBOARD_ROTATION_SPEED_MIN = 1.4f;//0.06f;
        public const float SMALL_BILLBOARD_ROTATION_SPEED_MAX = 2.5f;//0.10f;
        public const int SMALL_BILLBOARD_TAIL_COUNT_MIN = 8;//28
        public const int SMALL_BILLBOARD_TAIL_COUNT_MAX = 10;//30
        public const float SMALL_BILLBOARD_TAIL_DISTANCE_MIN = 400;//300
        public const float SMALL_BILLBOARD_TAIL_DISTANCE_MAX = 550;//450

        public const float PARTICLE_DUST_DECREAS_DISTANCE = LARGE_BILLBOARD_DISAPEAR_DISTANCE;

        public const float SWITCH_LARGE_AND_SMALL_BILLBOARD_RADIUS = 7000;
        public const float SWITCH_LARGE_AND_SMALL_BILLBOARD_DISTANCE = 10000;//SMALL_BILLBOARD_TAIL_COUNT_MAX * SMALL_BILLBOARD_TAIL_DISTANCE_MAX * 0.8f;
        public const float SWITCH_LARGE_AND_SMALL_BILLBOARD_DISTANCE_HALF = SWITCH_LARGE_AND_SMALL_BILLBOARD_DISTANCE / 3.0f;

        public static readonly float FORCE_ANGLE_RANDOM_VARIATION_IN_RADIANS = MathHelper.ToRadians(70);
        public const float FORCE_IMPULSE_RANDOM_MAX = 500000f;         //  This is only MAX value of random impulse, not exact impulse value.
        public const float FORCE_IMPULSE_POSITION_DISTANCE = 1000;       //  This tells us how far from phys object is source. Too far means low throw impulse. Always in oposite direction of force.

        public const float SPEED_MIN = 1300;
        public const float SPEED_MAX = 1500;

        public const float HEALTH_DAMAGE = 80;
        public const float SHIP_DAMAGE = 50;

        public const float SECONDS_FOR_SMALL_BILLBOARDS_INITIALIZATION = 1.0f;  // This is time in which all small billboards will have MaxDistance initialized
        public const float RAY_CAST_DISTANCE = 30000; // We ignore all entities in ray cast except those that are x meters away from player
    }

    static class MyAudioConstants
    {
        public const float MUSIC_MASTER_VOLUME_MIN = 0;
        public const float MUSIC_MASTER_VOLUME_MAX = 1;
        public const float GAME_MASTER_VOLUME_MIN = 0;
        public const float GAME_MASTER_VOLUME_MAX = 1;

        public const float REVERB_MAX = 100;

        // Multiple explosions sounds weird when set to 2
        // Multiple explosions sounds ok when set to 7
        public const int MAX_SAME_CUES_PLAYED = 7;

        public const int PREALLOCATED_UNITED_SOUNDS_PER_PHYS_OBJECT = 100;

        public const bool LIMIT_MAX_SAME_CUES = false;

        public const int MAX_COLLISION_SOUNDS = 3; // per contact
        public const int MAX_COLLISION_SOUNDS_PER_SECOND = 5; // per second

        //  How many cues of same type can be played simultaneously. E.g. if 10 bullet hit cues should be played at once, only this number will be really played.
        //public const int MAX_SAME_CUES_PLAYED = 3;

        //  It doesn't seem to be good to limit cues... so I disabled it for a while.
        //public const bool LIMIT_MAX_SAME_CUES = false;

        //  Constants for calculating collision sound pitch
        public const float MIN_DECELERATION_FOR_COLLISION_SOUND = 0;//-0.1f;
        public const float MAX_DECELERATION = -1f;
        public const float DECELERATION_MIN_VOLUME = 0.95f;//0.85f;
        public const float DECELERATION_MAX_VOLUME = 1.0f;

        //  This value must correspond to 'DistanceCurve' from XACT file.
        public const float OCCLUSION_INTERVAL = 200.0f;
        public const float MAIN_MENU_DECREASE_VOLUME_LEVEL = 0.5f;

        //  This is used to duck player ship engine sound little bit, so that AI ships with same engine sounds are heard well(still, there is a bit of phasing happening, but it is ok at this decrease)
        public const float PLAYER_SHIP_ENGINE_VOLUME_DECREASE_AMOUNT = 0.1f;
        
    }

    static class MyDecalsConstants
    {
        public const int DECAL_BUFFERS_COUNT = 10;
        public const int DECALS_FADE_OUT_INTERVAL_MILISECONDS = 1000;

        public const int MAX_DECAL_TRIANGLES_IN_BUFFER = 128;
        public const int MAX_DECAL_TRIANGLES_IN_BUFFER_SMALL = 128;
        public const int MAX_DECAL_TRIANGLES_IN_BUFFER_LARGE = 32;

        public const int TEXTURE_LARGE_MAX_NEIGHBOUR_TRIANGLES = 36;
        public const float TEXTURE_LARGE_FADING_OUT_START_LIMIT_PERCENT = 0.7f;     //  Number of decal triangles for large texture (explosion smut). It's used for voxels and phys objects too.
        public const float TEXTURE_LARGE_FADING_OUT_MINIMAL_TRIANGLE_COUNT_PERCENT = 1 - TEXTURE_LARGE_FADING_OUT_START_LIMIT_PERCENT;

        public const int TEXTURE_SMALL_MAX_NEIGHBOUR_TRIANGLES = 32;
        public const float TEXTURE_SMALL_FADING_OUT_START_LIMIT_PERCENT = 0.7f;      //  Number of decal triangles for small texture (bullet hole). It's used for voxels and phys objects too.
        public const float TEXTURE_SMALL_FADING_OUT_MINIMAL_TRIANGLE_COUNT_PERCENT = 1 - TEXTURE_SMALL_FADING_OUT_START_LIMIT_PERCENT;

        public const int VERTEXES_PER_DECAL = 3;
        public static readonly float MAX_NEIGHBOUR_ANGLE = MathHelper.ToRadians(80);

        //  This will how far or distance decals we have to draw. Every decal that is two times farest than reflector spot won't be drawn.
        public const float MAX_DISTANCE_FOR_DRAWING_DECALS_MULTIPLIER_FOR_REFLECTOR = 2.0f;

        //  Don't create decals if it is farther than this distance
        public const float MAX_DISTANCE_FOR_ADDING_DECALS = 500;

        //  Don't draw decals if it is farther than this distance
        public const float MAX_DISTANCE_FOR_DRAWING_DECALS = 200;

        //  We will draw large decals in larger distance
        public const float DISTANCE_MULTIPLIER_FOR_LARGE_DECALS = 3.5f;

        //  This value isn't really needed, because models doesn't have sun defined in triangles, but in shade per object
        //  It's only because some parts of decals are same for voxels and I want this information not lost.
        //public const byte SUN_FOR_MODEL_DECALS = 255;

        public static readonly Vector4 PROJECTILE_DECAL_COLOR = new Vector4(1.0f, 0.6f, 0.1f, 0);

        // These values give the percentage of how much we move decals in the direction of the dominant normal.
        public const float DECAL_OFFSET_BY_NORMAL = 0.10f;
        public const float DECAL_OFFSET_BY_NORMAL_FOR_SMUT_DECALS = 0.25f;
    }

    static class MyCockpitGlassConstants
    {
        //  This is min/max alpha to which we scale or lerp miner ship glass dirt level before rendering (we scale it from interval <0..1> to this one)
        public const float GLASS_DIRT_MIN = 0.0f;//1.25f;
        public const float GLASS_DIRT_MAX = 2.0f;//3.0f;
    }

    static class MyCockpitGlassDecalsConstants
    {
        public const int VERTEXES_PER_DECAL = 3;
        public const int MAX_DECAL_TRIANGLES_IN_BUFFER = 3000;
        public const int MAX_NEIGHBOUR_TRIANGLES = 2500;
        public static readonly float MAX_NEIGHBOUR_ANGLE = MathHelper.ToRadians(30);        //  This angle is used only for bullet holes

        //public static readonly Vector4 AMBIENT_GLASS_DECAL = new Vector4(0.15f, 0.15f, 0.15f, 1);
        public const float NEAR_LIGHT_COLOR_MULTIPLIER = 0.25f;
    }

    static class MyCockpitInteriorConstants
    {
        public const float NEAR_LIGHT_COLOR_MULTIPLIER = 0.2f;
        public const float SPECULAR_SHININESS = 0.5f;
        public const float SPECULAR_SPECULAR_POWER = 10;
    }

    /*  Moved to MySector.ParticleDustProperties
    static class MyParticlesDustFieldConstants
    {
        public const int DUST_FIELD_COUNT_IN_DIRECTION_HALF = 5;
        public const int DUST_FIELD_COUNT_IN_DIRECTION = DUST_FIELD_COUNT_IN_DIRECTION_HALF * 2 + 1;
        public const float DISTANCE_BETWEEN = 180;//30.0f;
        public const float DISTANCE_BETWEEN_HALF = DISTANCE_BETWEEN / 2.0f;
    } */

    static class MyTransparentGeometryConstants
    {
        public const int MAX_TRANSPARENT_GEOMETRY_COUNT = 50000;

        public const int MAX_PARTICLES_COUNT = (int)(MAX_TRANSPARENT_GEOMETRY_COUNT * 0.05f);
        public const int MAX_NEW_PARTICLES_COUNT = (int)(MAX_TRANSPARENT_GEOMETRY_COUNT * 0.7f);
        public const int MAX_COCKPIT_PARTICLES_COUNT = 30;      //  We don't need much cockpit particles

        public const int TRIANGLES_PER_TRANSPARENT_GEOMETRY = 2;
        public const int VERTICES_PER_TRIANGLE = 3;
        public const int INDICES_PER_TRANSPARENT_GEOMETRY = TRIANGLES_PER_TRANSPARENT_GEOMETRY * VERTICES_PER_TRIANGLE;
        //public const int VERTICES_PER_TRANSPARENT_GEOMETRY = INDICES_PER_TRANSPARENT_GEOMETRY;
        public const int VERTICES_PER_TRANSPARENT_GEOMETRY = 4;
        public const int MAX_TRANSPARENT_GEOMETRY_VERTICES = MAX_TRANSPARENT_GEOMETRY_COUNT * VERTICES_PER_TRANSPARENT_GEOMETRY;
        public const int MAX_TRANSPARENT_GEOMETRY_INDICES = MAX_TRANSPARENT_GEOMETRY_COUNT * TRIANGLES_PER_TRANSPARENT_GEOMETRY * VERTICES_PER_TRIANGLE;

        //  Use this for all SOFT particles: dust, explosions, smoke, etc. Value was hand-picked.
        public const float SOFT_PARTICLE_DISTANCE_SCALE_DEFAULT_VALUE = 0.05f;

        //  Use this for all particles that will be near an object and you practically don't want soft-particle effect on them. 
        //  It will make them HARD particles. Value was hand-picked.
        public const float SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES = 1000;

        //Use this only for decal particles, which reside always close to depth, but not cut into it
        public const float SOFT_PARTICLE_DISTANCE_DECAL_PARTICLES = 10000;
    }

    enum MyParticleEffectsIDEnum
    {
        Dummy = 0,

        Prefab_LeakingFire_x2 = 8,
        Prefab_LeakingBiohazard = 11,
        Prefab_LeakingBiohazard2 = 12,
        Prefab_LeakingSmoke = 14,
        Prefab_Fire_Field = 15,

        MeteorTrail_Smoke = 100,
        MeteorTrail_FireAndSmoke = 101,

        // damage effects
        Damage_Sparks = 200,
        Damage_Smoke = 201,
        Damage_SmokeDirectionalA = 202,
        Damage_SmokeDirectionalB = 203,
        Damage_SmokeDirectionalC = 204,
        Damage_SmokeBiochem = 205,

        // prefab particle effects
        Prefab_LeakingSteamWhite = 300,
        Prefab_LeakingSteamGrey = 301,
        Prefab_LeakingSteamBlack = 302,
        Prefab_DustyArea = 303,
        Prefab_EMP_Storm = 304,
        Prefab_LeakingElectricity = 305,
        Prefab_LeakingFire = 306,

        // special ammunition
        UniversalLauncher_DecoyFlare = 400,
        UniversalLauncher_IlluminatingShell = 401,
        UniversalLauncher_SmokeBomb = 402,

        // drills
        Drill_Laser = 450,
        Drill_Saw = 451,
        Drill_Nuclear_Original = 452,
        Drill_Thermal = 453,
        Drill_Nuclear = 454,        
        Drill_Pressure_Charge = 455,
        Drill_Pressure_Fire = 456,
        Drill_Pressure_Impact = 457,
        Drill_Pressure_Impact_Metal = 458,

        // smoke
        Smoke_Autocannon = 500,
        Smoke_CannonShot = 501,
        Smoke_Missile = 502,
        Smoke_MissileStart = 503,
        Smoke_LargeGunShot = 504,
        Smoke_SmallGunShot = 505,
        Smoke_DrillDust = 506,

        // drilling and harvesting
        Harvester_Harvesting = 550,
        Harvester_Finished = 551,

        // explosions
        Explosion_Ammo = 600,
        Explosion_Blaster = 601,
        Explosion_Smallship = 604,
        Explosion_Bomb = 605,
        Explosion_Missile = 666,
        Explosion_SmallPrefab = 607,
        Explosion_Plasma = 630,
        Explosion_Nuclear = 640,
        Explosion_BioChem = 667,
        Explosion_EMP = 669,
        Explosion_Large = 3,
        Explosion_Huge = 4,
        Explosion_Asteroid = 6,
        Explosion_Medium = 7,
        

        // Close versions of explosions
        Explosion_Missile_Close = 616,

        // asteroid reaction to explosion (billboard debris)
        MaterialExplosion_Destructible = 650,

        // projectile impact effects except for autocannon and shotgun
        Hit_ExplosiveAmmo = 700,
        Hit_ChemicalAmmo = 701,
        Hit_HighSpeedAmmo = 702,
        Hit_PiercingAmmo = 703,
        Hit_BasicAmmo = 704,
        Hit_EMPAmmo = 710,

        // projectile impact for autocannon and shotgun
        Hit_AutocannonBasicAmmo = 705,
        Hit_AutocannonChemicalAmmo = 706,
        Hit_AutocannonHighSpeedAmmo = 707,
        Hit_AutocannonPiercingAmmo = 708,
        Hit_AutocannonExplosiveAmmo = 709,
        Hit_AutocannonEMPAmmo = 711,

        // material reaction to projectile impact
        MaterialHit_Destructible = 720,
        MaterialHit_Indestructible = 721,
        MaterialHit_Metal = 722,
        MaterialHit_Autocannon_Destructible = 730,
        MaterialHit_Autocannon_Indestructible = 731,
        MaterialHit_Autocannon_Metal = 732,

        // collisions
        Collision_Smoke = 800,
        Collision_Sparks = 801,

        // thrusters
        EngineThrust = 900,

        // projectile trails
        Trail_Shotgun = 950,

        Explosion_Meteor = 951,
    }

    static class MyDummyPointConstants
    {
        /// <summary>
        /// Default size of a dummy point when added in the editor.
        /// </summary>
        public const int DEFAULT_DUMMYPOINT_SIZE = 20;

        /// <summary>
        /// Multiplier for push force done by pushing particle effects (leaking steam, etc).
        /// </summary>
        public const float PUSH_STRENGTH = 1000;

        /// <summary>
        /// The size of the influence sphere of all particle effects that is used for detecting entities and for
        /// determining the magnitude of the effect (push force, damage).
        /// </summary>
        public const int PARTICLE_DETECTOR_SIZE = 25;

        /// <summary>
        /// The maximum distance (squared) in meters for which the sound associated with the particle effect will still play.
        /// </summary>
        public const float MAX_SOUND_DISTANCE_SQUARED = 650 * 650;
    }

    static class MyNotificationConstants
    {
        public const int MAX_HUD_NOTIFICATIONS_COUNT = 100;
        public static Vector2 DEFAULT_NOTIFICATION_MESSAGE_NORMALIZED_POSITION = new Vector2(0.5f, 0.75f);
        public const int MAX_DISPLAYED_NOTIFICATIONS_COUNT = 5;
    }

    static class MyHudConstants
    {
        public const int MAX_HUD_RADAR_QUADS_COUNT = 16384;//2000;
        public const int MAX_HUD_QUADS_COUNT = 2000;
        public const int MAX_HUD_TEXTS_COUNT = 2000;//300;
        public const int TRIANGLES_PER_HUD_QUAD = 2;            //  These are triangles for representing middle of the line
        public const int VERTEXES_PER_HUD_QUAD = TRIANGLES_PER_HUD_QUAD * 3;
        public const float HUD_LINE_THICKNESS_HALF = 0.0015f;//0.0025f;         //  Half of line thickness. It is something like radius.
        //public const float HUD_BACK_CAMERA_LINE_THICKNESS_HALF = 1.5f * 0.0025f;         //  Half of line thickness. It is something like radius.

        //public const float CROSSHAIR_SIZE = 0.0175f;
        public const float DEFAULT_CROSSHAIR_SIZE = 0.035f * 0.75f;
        public const float SPECIAL_CROSSHAIR_SIZE = 0.035f * 0.75f * 2f;
        public const float OMNICORP_CROSSHAIR_SIZE = 0.035f * 0.75f * 2f * 0.8f;
        public static readonly Vector2 RADAR_ZOOM_SIZE = new Vector2(0.055f / 2, 0.035f);

        public const float HORISONTAL_ANGLE_LINE_POSITON_DELTA = 0.1f;

        public const float DISTANCE_FOR_SECTOR_BORDER_WARNING = 3333;
        public const float DISTANCE_FOR_SECTOR_BORDER_DRAW = 1.2f * DISTANCE_FOR_SECTOR_BORDER_WARNING;

        public static readonly Color HUD_COLOR = new Color(180, 180, 180, 180);

        public static readonly Color HUD_COLOR_DARKER = new Color(180, 180, 180, 180);//new Color(HUD_COLOR.ToVector4() * 0.7f);

        public static readonly Color HUD_COLOR_LIGHT = new Color(255, 255, 255, 255);

        //public static readonly Color HUD_TEXT_COLOR = HUD_COLOR;
        public static readonly Color HUD_SECTOR_BOUNDARIES_DISTANCE_NOTIFICATION_COLOR = new Color(180, 20, 40, 180);

        public static readonly Color HUD_BORDER_COLOR_BRIGHT = new Color(HUD_COLOR.ToVector4().X * 0.7f, HUD_COLOR.ToVector4().Y * 0.7f, HUD_COLOR.ToVector4().Z * 0.7f, 1);
        public static readonly Color HUD_BORDER_COLOR_DARK = new Color(HUD_COLOR.ToVector4().X * 0.65f, HUD_COLOR.ToVector4().Y * 0.65f, HUD_COLOR.ToVector4().Z * 0.65f, 1);
        public const float HUD_BORDER_THICK = 0.0015f;
        public const float HUD_BORDER_THIN = 0.0015f;
        public const float HUD_BORDER_STRIP = 0.05f;

        public static readonly Color HUD_BACK_CAMERA_OVERLAY = new Color(HUD_COLOR.ToVector4().X * 0.3f, HUD_COLOR.ToVector4().Y * 0.3f, HUD_COLOR.ToVector4().Z * 0.3f, 0.65f);

        public const float BACK_CAMERA_HEIGHT = 0.18f;
        public const float BACK_CAMERA_ASPECT_RATIO = 1.6f;

        public const float HUD_DIRECTION_INDICATOR_SIZE = 0.006667f;
        public const float HUD_TEXTS_OFFSET = 0.01f;
        public const float HUD_UNPOWERED_OFFSET = 0.01f;
        public const float DIRECTION_INDICATOR_MAX_SCREEN_DISTANCE = 0.425f;
        public const float DIRECTION_INDICATOR_MAX_SCREEN_TARGETING_DISTANCE = 0.25f;
        public static readonly Vector2 DIRECTION_INDICATOR_SCREEN_CENTER = new Vector2(0.5f, 0.5f);

        public static readonly Vector2 HUD_VECTOR_UP = new Vector2(0, -1);

        //  HUD Radar        
        public const float RADAR_MOVEMENT_DETECTOR_MIN_SPEED_TO_DETECT = 0.1f;
        public const float RADAR_JAM_FROM_SUN_WIND_RADIUS = 500;
        public const float RADAR_JAMMER_RANGE = 800;        
        public const float RADAR_BLINKING_RANGE = 100;        
        public const float RADAR_PLANE_RADIUS = 59;
        public const float RADAR_ZOOM_STEP = 0.2f;
        public const float RADAR_SECTOR_BORDER_REPOSITION = 0.8f;
        public const float RADAR_ZOOM_MIN = 5f;      //  Scale 1 means without scaling, 3 means 3 times smaller
        public const float RADAR_ZOOM_MAX = 100f;     //  Everything is 100 times smaller
        public static readonly float RADAR_FIELD_OF_VIEW = MathHelper.ToRadians(45);//MathHelper.ToRadians(25);
        public const float RADAR_ASPECT_RATIO = 1.3f;
        public const float RADAR_PHYS_OBJECT_SIZE = 14f;
        public const float RADAR_SECTOR_BORDER_BILLBOARD_SIZE = 105;
        public const float RADAR_SUN_BILLBOARD_SIZE = 30;
        public static readonly Vector3 ORIGINAL_CAMERA_POSITON = new Vector3(0, 50f, 170f);//new Vector3(0, 50f, 100f);//new Vector3(0, 100f, 300f);
        public static readonly Color RADAR_SECTOR_BORDER_COLOR = new Color(0, 110, 0, 75);//Color.Green * (100.0f / 255.0f);

        public static readonly Color PLAYER_MARKER_COLOR = new Color(0.0f, 0.0f, 0.7f, 1.0f);           //= Color.Blue; = new Color(1.0f,1.0f,1.0f,1.0f) = new Color(255,255,255,255)
        public static readonly Color BOT_MARKER_COLOR = new Color(0.8f, 0.3f, 0.3f, 1.0f);              //= Color.Red;
        public static readonly Color NEUTRAL_MARKER_COLOR = new Color(0.8f, 0.8f, 0.8f, 1.0f);          //= Color.Red;
        public static readonly Color LARGESHIP_MARKER_COLOR = new Color(1.0f, 1.0f, 1.0f, 1.0f);        //= Color.White;
        public static readonly Color FRIEND_MARKER_COLOR = new Color(0.0f, 0.7f, 0.0f, 1.0f);           //= Color.Green;
        public static readonly Color MISSION_MARKER_COLOR = new Color(1,1,1, 1.0f);    //menim to z modre na bilou, Filip puvodni--->(0.0f, 0.55f, 1.0f, 1.0f);          .. Dark Orange 
        public static readonly Color MISSION_OPTIONAL_MARKER_COLOR = new Color(1.0f, 0.55f, 0.0f, 1.0f);    //menim to z modre na bilou, Filip puvodni--->(0.0f, 0.55f, 1.0f, 1.0f);          .. Dark Orange 
        public static readonly Color MISSION_MARKER_COLOR_BLUE = new Color(40, 141, 227, 255); 


        public static readonly Color ACTIVE_MISSION_SOLAR_MAP_COLOR = new Color(0.0f, 0.55f, 0.0f, 1.0f);      // Green marker for active mission
        public static readonly Vector4 MISSION_CUBE_COLOR = new Vector4(1.0f, 1.0f, 1.0f, 1.0f)/* * 0.2f*/; // premultiplied alfa color for bounding cube
        public static readonly Vector4 FRIEND_CUBE_COLOR = new Vector4(0.0f, 0.7f, 0.0f, 1.0f) * 0.2f;   // premultiplied alfa color for bounding cube
        public static readonly Vector4 ENEMY_CUBE_COLOR = new Vector4(0.8f, 0.3f, 0.3f, 1.0f) * 0.2f;    // premultiplied alfa color for bounding cube
        public static readonly Vector4 NEUTRAL_CUBE_COLOR = new Vector4(0.8f, 0.8f, 0.8f, 1.0f) * 0.2f;  // premultiplied alfa color for bounding cube

        public static MyGuiFont MISSION_FONT
        {
            get { return MyGuiManager.GetFontMinerWarsBlue(); }
        }
        public static MyGuiFont FRIEND_FONT
        {
            get { return MyGuiManager.GetFontMinerWarsGreen(); }
        }
        public static MyGuiFont NEUTRAL_FONT
        {
            get { return MyGuiManager.GetFontMinerWarsWhite(); }
        }
        public static MyGuiFont ENEMY_FONT
        {
            get { return MyGuiManager.GetFontMinerWarsRed(); }
        }

        public const float PLAYER_MARKER_MULTIPLIER = 0.3f;
        //public static readonly Vector3 ORIGINAL_CAMERA_POSITON_2D_SECOND_DRAW = new Vector3(0, 300f, 300f);
        public const float RADAR_BOUNDING_BOX_SIZE = 3000;
        public const float RADAR_BOUNDING_BOX_SIZE_HALF = RADAR_BOUNDING_BOX_SIZE / 2.0f;
        public const float DIRECTION_TO_SUN_LINE_LENGTH = 100;
        public const float DIRECTION_TO_SUN_LINE_LENGTH_HALF = DIRECTION_TO_SUN_LINE_LENGTH / 0.7f;
        public const float DIRECTION_TO_SUN_LINE_THICKNESS = 0.7f;
        public const float NAVIGATION_MESH_LINE_THICKNESS = 3f;
        public const float NAVIGATION_MESH_DISTANCE = 100;
        public const int NAVIGATION_MESH_LINES_COUNT_HALF = 10;
        public const float RADAR_SPHERE_RADIUS = MyHudConstants.RADAR_PHYS_OBJECT_SIZE * 3.8f;
        public static readonly Color HUD_RADAR_BACKGROUND_COLOR = new Color(HUD_COLOR.ToVector4().X * 0.1f, HUD_COLOR.ToVector4().Y * 0.1f, HUD_COLOR.ToVector4().Z * 0.1f, 0.9f);
        public static readonly Color HUD_RADAR_BACKGROUND_COLOR2D = Color.White;
        public static readonly Vector3 HUD_RADAR_PHYS_OBJECT_POINT_DELTA = new Vector3(0, 0, 1);    //  Every phys object point on radar must be a bit closer to camera so then it's not behind the vertical line
        public const int MIN_RADAR_TYPE_SWITCH_TIME_MILLISECONDS = 500;

        public static Color HUD_STATUS_BACKGROUND_COLOR = new Color(0.482f, 0.635f, 0.643f, 0.35f);
        public static Color HUD_STATUS_DEFAULT_COLOR = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public static Color HUD_STATUS_BAR_COLOR_GREEN_STATUS = new Color(124, 174, 125, 205);//new Color(57, 113, 73, 255);
        public static Color HUD_STATUS_BAR_COLOR_YELLOW_STATUS = new Color(218, 213, 125, 205);//new Color(218, 213, 125, 255);//new Color(238, 190, 15, 255);
        public static Color HUD_STATUS_BAR_COLOR_ORANGE_STATUS = new Color(236, 163, 97, 205);//new Color(247, 208, 123, 255);//new Color(247, 143, 43, 255);
        public static Color HUD_STATUS_BAR_COLOR_RED_STATUS = new Color(187, 0, 0, 205);//new Color(187, 62, 56, 255);//new Color(187, 30, 35, 255);
        public const float HUD_STATUS_BAR_COLOR_GRADIENT_OFFSET = 2.0f;

        public static Vector2 HUD_STATUS_POSITION = new Vector2(0.02f, -0.02f);
        public static Vector2 HUD_MISSIONS_POSITION = new Vector2(-0.02f, 0.02f);
        //        public static Vector2 HUD_STATUS_SIZE = new Vector2(0.05f, 0.05f);
        public static Vector2 HUD_STATUS_ICON_SIZE = new Vector2(0.012f, 0.01f) * 0.85f;
        public static Vector2 HUD_STATUS_SPEED_ICON_SIZE = new Vector2(0.0145f, 0.0145f);
        //public static Vector2 HUD_STATUS_BAR_SIZE = new Vector2(HUD_STATUS_ICON_SIZE.X / 2.0f, HUD_STATUS_ICON_SIZE.Y / 2.0f);
        public static Vector2 HUD_STATUS_BAR_SIZE = new Vector2(0.0095f, 0.006f);
        public const float HUD_STATUS_ICON_DISTANCE = 0.0075f;//0.012f / 2.0f;
        //public const float HUD_STATUS_BAR_DISTANCE = 0.005f;
        public const float HUD_STATUS_BAR_DISTANCE = 0.0010f;
        public const int HUD_STATUS_BAR_MAX_PIECES_COUNT = 5;

        public const int PREFAB_PREVIEW_SIZE = 128;

        public static readonly Vector2 LOW_FUEL_WARNING_POSITION = new Vector2(0.5f, .65f);

        public const float HUD_MAX_DISTANCE_ENEMIES = 2500f;
        public const float HUD_MAX_DISTANCE_NORMAL = 800f;
        public const float HUD_MAX_DISTANCE_ALPHA = 0.2f;
        public const float HUD_MIN_DISTANCE_ALPHA = 1f;
        public const float HUD_MAX_DISTANCE_TO_ALPHA_CORRECT_NORMAL = HUD_MAX_DISTANCE_NORMAL;
        public const float HUD_MAX_DISTANCE_TO_ALPHA_CORRECT_ENEMIES = HUD_MAX_DISTANCE_ENEMIES;
        public const float HUD_MIN_DISTANCE_TO_ALPHA_CORRECT = 50f;

        public const float GPS_SEGMENT_ADVANCE = 13;
        public const float GPS_SEGMENT_LENGTH = 14;
        public const float GPS_SEGMENT_WIDTH = 2;
        public const float GPS_DOT_WIDTH = 2.25f; // upravuju velikost gps koule, Filip  // taky, Honza K
        public const float GPS_DOT_OFFSET = 0.18f - 1; // 0.07f;  // upravuju posun gps koule, Filip  // taky, Honza K
        public const int GPS_DURATION = 7000;
        public const int GPS_FADE_OUT_DURATION = 2000;     // part of GPS_DURATION
        public const float GPS_FADE_IN_SPEED = 1000;        // m/s
        public const float GPS_FADE_IN_TAIL_LENGTH = 400;  // m
        public const float GPS_FADE_OUT_DISTANCE_START = 800f;
        public const float GPS_FADE_OUT_DISTANCE_END = 4000f;

        public const float GPS_PULSE_LENGTH_FREQ = 0.0162f;
        public const float GPS_PULSE_TIME_FREQ = 0.0054f;
        public const float GPS_PULSE_TIME_PHASE = 2.5f;
        public const float GPS_FADE_PULSE_DC = 0.7f;
        public const float GPS_FADE_PULSE_AMPLITUDE = 1 - GPS_FADE_PULSE_DC;
        public const float GPS_WIDTH_PULSE_DC = 1f;
        public const float GPS_WIDTH_PULSE_AMPLITUDE = 0.2f;

        //public static readonly Vector4 GPS_COLOR = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);
        public const float GPS_START_POSITION_FRONT = 80;
        public const float GPS_START_POSITION_UP = -5;


        public static readonly Vector2 DIALOGUE_ACTORTEXTURE_POSITION = new Vector2(0.2145f, 0.924f);
        public static readonly Vector2 DIALOGUE_ACTORTEXTURE_SIZE = new Vector2(0.0525f, 0.087f);
        
        public static readonly Vector2 DIALOGUE_BACKGROUND_POSITION = new Vector2(0.5f, 0.93f);
        public static readonly Vector2 DIALOGUE_BACKGROUND_SIZE = new Vector2(0.6551868f, 0.15367033f);

        public const float DIALOGUE_TEXTAREA_FONT_SIZE = 0.75f;
        public static readonly Vector2 DIALOGUE_TEXTAREA_POSITION = new Vector2(0.502f, 0.934f);
        public static readonly Vector2 DIALOGUE_TEXTAREA_SIZE = new Vector2(0.500f, 0.08f);

        public const float DIALOGUE_ACTORNAME_FONT_SIZE = 0.75f;
        public static readonly Vector2 DIALOGUE_ACTORNAME_POSITION = new Vector2(0.380f, 0.886f);
        public static readonly Vector2 DIALOGUE_ACTORNAME_SIZE = new Vector2(0.25f, 0.03f);

        public static Color SOLAR_MAP_STORY_MISSION_MARKER_COLOR = Color.CornflowerBlue;
        public static Color SOLAR_MAP_SIDE_MISSION_MARKER_COLOR = Color.LightGray;
        public static Color SOLAR_MAP_TEMPLATE_MISSION_MARKER_COLOR = Color.LightGray;
        public static Color SOLAR_MAP_PLAYER_MARKER_COLOR = Color.LightGreen;
    }

    static class MyShadowConstants
    {
        //  This is value in shadow map when it has no shadow, so full sun light is present at that place
        public const byte SHADOW_EMPTY_AS_BYTE = 255;
        public const float SHADOW_EMPTY_AS_FLOAT = (float)SHADOW_EMPTY_AS_BYTE;

        //  How many voxels make ony shadow cell
        public const int SHADOW_ENTITY_SIZE_IN_VOXELS = 2;

        public const float SHADOW_ENTITY_SIZE_IN_METERS = SHADOW_ENTITY_SIZE_IN_VOXELS * MyVoxelConstants.VOXEL_SIZE_IN_METRES;
        public const float SHADOW_ENTITY_SIZE_IN_METERS_HALF = SHADOW_ENTITY_SIZE_IN_METERS / 2.0f;

        //  How many meters will shadow area exceed behind occluder's end (in positive Z direction)
        //  In fact it should be infinite, but I just don't want extremely long shadows
        public const float SHADOW_EXCEEDS_IN_POSITIVE_Z_DIRECTION_IN_METERS = 10000;

        //  How many shadow values we average when calculating averaged shadows. This number is for each side of shadow-grid, thus final grid is e.g. 4x4x4
        public const int AVERAGED_SHADOW_SIZE_IN_SHADOW_ENTITIES = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS / SHADOW_ENTITY_SIZE_IN_VOXELS;
    }

    static class MyVoxelMapImpostorsConstants
    {
        public const float RANDOM_COLOR_MULTIPLIER_MIN = 0.9f;
        public const float RANDOM_COLOR_MULTIPLIER_MAX = 1.0f;

        public const int TRIANGLES_PER_IMPOSTOR = 2;
        public const int VERTEXES_PER_IMPOSTOR = TRIANGLES_PER_IMPOSTOR * 3;
    }

    static class MyDistantObjectsImpostorsConstants
    {
        public const int MAX_NUMBER_DISTANT_OBJECTS = 50;
        public const float MAX_MOVE_DISTANCE = .00045f;
        public static readonly float DISTANT_OBJECTS_SPHERE_RADIUS = MyMwcSectorConstants.SECTOR_DIAMETER * 2.5f;
        public const float RANDOM_COLOR_MULTIPLIER_MIN = 0.9f;
        public const float RANDOM_COLOR_MULTIPLIER_MAX = 1.0f;
        public const float BLINKER_FADE = .005f;
        public const float EXPLOSION_FADE = .02f;
        public const float EXPLOSION_WAIT_MILLISECONDS = 1000f;
        public const float BLINKER_WAIT_MILLISECONDS = 1500f;
        public const float EXPLOSION_MOVE_DISTANCE = .01f;
        public const int TRIANGLES_PER_IMPOSTOR = 2;
        public const int VERTEXES_PER_IMPOSTOR = TRIANGLES_PER_IMPOSTOR * 3;
    }

    static class MyVoxelConstants
    {
        // Size of dictionary for storing finished multimaterials
        // This number should be between 1 and NumberOfMaterials^3
        // Good number is to think about maximal number of materials per voxel map in normal case, like 10, and than put here number^3
        // When this number is insufficient, there will be one-time reallocation
        public const int DEFAULT_MULTIMATERIAL_CACHE_SIZE = 1000;

        //  This is max number of cell contents (mixed cells).
        // Decreased to 120K, 150K eat to much memory, best would be to decrease even more
        public const int PREALLOCATED_CELL_CONTENTS_COUNT = MyFakes.MWBUILDER ? 300000 : 120000;//30000;//75000;//100000;

        //  How many batches (combination of texture / vertex buffer / m_notCompressedIndex buffer) is preallocated for each render cell.
        //  If we will need more, list will be grown automatically, so this is just initial capacity.
        public const int PREALLOCATED_RENDER_CELL_BATCHES = 32;

        //  This is max number of triangles we can hold in temporal buffer when calculating JLX collisions
        //  between box/sphere and voxel maps. This are potential triangles where bounding box was intersected.
        public const int MAX_POTENTIAL_COLDET_TRIANGLES_COUNT = 2048;

        //  Size of voxel's cell cache
        public const int VOXEL_DATA_CELL_CACHE_SIZE = 32768;//50 * 256 / 8 * 256 / 8 * 256 / 8 * 2;
        public const int VOXEL_RENDER_CELL_CACHE_SIZE = 16384;//10000;//256 / 8 * 256 / 8 * 256 / 8 * 2;

        //  This is the value that says if voxel is full or not (but only for marching cubes algorithm, not path-finding, etc)
        //  It's the middle of 0 and 255
        public const byte VOXEL_ISO_LEVEL = 127;
        public const float VOXEL_ISO_LEVEL_FLOAT = (float)VOXEL_ISO_LEVEL / 255.0f;

        //  Value of voxel's content if voxel is empty
        public const byte VOXEL_CONTENT_EMPTY = 0;
        public const float VOXEL_CONTENT_EMPTY_FLOAT = (float)VOXEL_CONTENT_EMPTY;

        //  Value of voxel's content if voxel is full
        public const byte VOXEL_CONTENT_FULL = 255;
        public const float VOXEL_CONTENT_FULL_FLOAT = (float)VOXEL_CONTENT_FULL;

        //  This is the scale we multiply voxel size when drawing it as cube (it's because if we set it to 1.0, we won't see holes between voxels)
        public const float VOXEL_DRAW_AS_CUBE_SCALE = 0.8f;

        //  Size of a voxel in metres
        public const float VOXEL_SIZE_IN_METRES = (MyFakes.MWBUILDER ? 2.0f : 15);//7.5f;//5f;//1.0f;//10.0f;
        public const float VOXEL_SIZE_IN_METRES_HALF = VOXEL_SIZE_IN_METRES / 2.0f;
        public static readonly Vector3 VOXEL_SIZE_VECTOR = new Vector3(VOXEL_SIZE_IN_METRES, VOXEL_SIZE_IN_METRES, VOXEL_SIZE_IN_METRES);
        public static readonly Vector3 VOXEL_SIZE_VECTOR_HALF = VOXEL_SIZE_VECTOR / 2.0f;
        public static readonly float VOXEL_RADIUS = VOXEL_SIZE_VECTOR_HALF.Length();

        //  How many data cells can fit in one render cell, in one direction
        public const int VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE = MyFakes.REDUCED_RENDER_CELL_SIZE ? 4 : 8;

        //  Total number of data cell in render cell (something like 8x8x8)
        public const int VOXEL_RENDER_CELL_SIZE_IN_DATA_CELLS_TOTAL = VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE * VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE * VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE;

        //  Max. count of triangles in a voxel cell. This number is used to pre-allocate triangleVertexes arrays and vertex buffers.
        //  Number 5 comes from marching cubes. It's max count of triangles in a poligonization cube.
        public const int MAX_TRIANGLES_COUNT_IN_VOXEL_DATA_CELL = VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_DATA_CELL_SIZE_IN_VOXELS * 5;

        //  Size of a voxel data cell in voxels (count of voxels in a voxel data cell) - in one direction
        //  Assume it's a power of two!
        public const int VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS = 3;
        public const int VOXEL_DATA_CELL_SIZE_IN_VOXELS = 1 << VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS;
        public const int VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK = VOXEL_DATA_CELL_SIZE_IN_VOXELS - 1;

        //  Total number of voxel in one data cell
        public const int VOXEL_DATA_CELL_SIZE_IN_VOXELS_TOTAL = VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_DATA_CELL_SIZE_IN_VOXELS;

        //  Size of a voxel data cell in metres (and its half version)
        public const float VOXEL_DATA_CELL_SIZE_IN_METRES = VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_SIZE_IN_METRES;
        public static readonly Vector3 VOXEL_DATA_CELL_SIZE_VECTOR_IN_METRES = new Vector3(VOXEL_DATA_CELL_SIZE_IN_METRES, VOXEL_DATA_CELL_SIZE_IN_METRES, VOXEL_DATA_CELL_SIZE_IN_METRES);
        public static readonly Vector3 VOXEL_DATA_CELL_SIZE_HALF_VECTOR_IN_METRES = VOXEL_DATA_CELL_SIZE_VECTOR_IN_METRES / 2.0f;
        public const float VOXEL_DATA_CELL_SIZE_IN_METRES_HALF = VOXEL_DATA_CELL_SIZE_IN_METRES / 2.0f;
        public static readonly float VOXEL_DATA_CELL_RADIUS = VOXEL_DATA_CELL_SIZE_HALF_VECTOR_IN_METRES.Length();

        //  Size of a voxel render cell in voxels (count of voxels in a voxel render cell)
        public const int VOXEL_RENDER_CELL_SIZE_IN_VOXELS = VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE;

        //  Size of a voxel render cell in metres (and its half version)
        public const float VOXEL_RENDER_CELL_SIZE_IN_METRES = VOXEL_RENDER_CELL_SIZE_IN_VOXELS * VOXEL_SIZE_IN_METRES;
        public const float VOXEL_RENDER_CELL_SIZE_IN_METRES_HALF = VOXEL_RENDER_CELL_SIZE_IN_METRES / 2.0f;
        public static readonly Vector3 VOXEL_RENDER_CELL_SIZE_VECTOR_IN_METRES = new Vector3(VOXEL_RENDER_CELL_SIZE_IN_METRES, VOXEL_RENDER_CELL_SIZE_IN_METRES, VOXEL_RENDER_CELL_SIZE_IN_METRES);
        public static readonly Vector3 VOXEL_RENDER_CELL_SIZE_HALF_VECTOR_IN_METRES = VOXEL_RENDER_CELL_SIZE_VECTOR_IN_METRES / 2.0f;
        public static readonly float VOXEL_RENDER_CELL_RADIUS = VOXEL_RENDER_CELL_SIZE_HALF_VECTOR_IN_METRES.Length();

        //  Max number of materials we can have in one voxel map (so max material m_notCompressedIndex is one less than this number)
        //public const int MAX_MATERIALS_PER_VOXEL_MAP = 8;//31;//4;//10;//32;//6;

        //  Initial sum of all voxels in a cell
        public const int VOXEL_CELL_CONTENT_SUM_TOTAL = VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_DATA_CELL_SIZE_IN_VOXELS * VOXEL_CONTENT_FULL;

        //  How many voxels can have one voxel map in one direction. This const isn't really needed, we just need some 
        //  offsets for voxel cell hash code calculations, so if you need to enlarge it, do so.
        public const int MAX_VOXEL_MAP_SIZE_IN_VOXELS = 10 * 1024;
        public const double MAX_VOXEL_MAPS_DATA_CELL_COUNT = MyFakes.MWBUILDER ? 1024*1024*1024 :  1.5 * (512 / VOXEL_DATA_CELL_SIZE_IN_VOXELS * 512 / VOXEL_DATA_CELL_SIZE_IN_VOXELS * 512 / VOXEL_DATA_CELL_SIZE_IN_VOXELS);
        public const Int64 MAX_VOXEL_MAP_ID = 1024;
        public const int MAX_SORTED_VOXEL_MAPS_COUNT = 100;

        //  This is the version of actually supported voxel file
        public const int VOXEL_FILE_ACTUAL_VERSION = 1;

        public static int MAX_VOXEL_HAND_SHAPES_COUNT = MyMwcFinalBuildConstants.GetValueForBuildType(5000, 5000, 30000);

        // When softening voxels using voxel hand, this is default soften weight amount
        public const float DEFAULT_SOFTEN_WEIGHT = 0.5f;

        // When wrinkling voxels using voxel hand, this is default wrinkle weight amount
        public const float DEFAULT_WRINKLE_WEIGHT_ADD = 0.5f;
        public const float DEFAULT_WRINKLE_WEIGHT_REMOVE = 0.45f;

        //public const int MIN_VOXEL_HAND_SPHERE_RADIUS = 1;
        //public const int MAX_VOXEL_HAND_SPHERE_RADIUS = 500;
        public const float VOXEL_HAND_SIZE_STEP = 2.0f;
        public const float MIN_VOXEL_HAND_SIZE = 1.0f;
        public const float MAX_VOXEL_HAND_SIZE = MyFakes.MWBUILDER ? 1000.0f : 1000.0f;
        public const float DEFAULT_VOXEL_HAND_SIZE = 50.0f;

        public const float VOXEL_HAND_DISTANCE_STEP = 0.05f;
        public const float MAX_VOXEL_HAND_DISTANCE = 1000.0f;
        public const float MIN_VOXEL_HAND_DISTANCE = 0.5f;
        public const float DEFAULT_VOXEL_HAND_DISTANCE = 200.0f;
        public const float MAX_PROJECTED_VOXEL_HAND_OFFSET = 1.5f;
        public const float MIN_PROJECTED_VOXEL_HAND_OFFSET = -1.5f;
        public const float DEFAULT_PROJECTED_VOXEL_HAND_OFFSET = 0.0f;

        public const float VOXEL_HAND_CUBOID_LENGT_SIZE_RATIO = 3.0f;
        public const bool VOXEL_HAND_DRAW_CONE = true;

        public const int VOXEL_HAND_SHAPING_INTERVAL = 500; //in ms

        //public const int VOXEL_MAP_MAX_ORE_DEPOSITS = 1024*64*64*64;
        public const int VOXEL_MAP_ORE_DEPOSIT_CELL_IN_DATA_CELLS = 1;
    }

    static class MyVertexCompression    // These values must be changed also in shader
    {
        public const int VOXEL_OFFSET = 32767;                           // Offset to add to coordinates when mapping voxel from float<0, 8191> to short<-32767, 32767>.
        public const int VOXEL_MULTIPLIER = 8;                           // Multiplier for mapping voxel from float to short.
        public const float INV_VOXEL_MULTIPLIER = 1.0f / VOXEL_MULTIPLIER;
        public const float VOXEL_COORD_EPSILON = INV_VOXEL_MULTIPLIER / 2;  // Due to rounding errors we must add VOXEL_COORD_EPSILON to coordinates when converting from float to short.
        public const int AMBIENT_MULTIPLIER = 32767;                     // Multiplier for mapping value from float<-1, 1> to short<-32767, 32767>.
        public const float INV_AMBIENT_MULTIPLIER = 1.0f / AMBIENT_MULTIPLIER;
    }

    static class MyLargeShipConstants
    {
        public const int MAX_LARGE_SHIPS_COUNT_IN_SECTOR = 20; //final value should be around 30
    }

    static class MyLargeShipWeaponsConstants
    {
        public static bool Enabled = true;
        public const string MUZZLE_FLASH_NAME_ONE = "MUZZLE_FLASH";
        public const string MUZZLE_FLASH_NAME_MODE = "MUZZLE_FLASH_";
        public const float WARNING_DAMAGE_ALERT_LEVEL = 0.3f;           // precentage
        public const float MAX_ROTATION_UPDATE_DISTANCE = 200.0f;
        public const float MIN_SEARCHING_DISTANCE = 10.0f; //meters
        public const float MAX_SEARCHING_DISTANCE = 2500.0f; //meters
        public const float AIMING_SOUND_DELAY = 120.0f; //ms
        public const float ROTATION_AND_ELEVATION_MIN_CHANGE = 0.007f; //rad
        public const float ROTATION_SPEED = 0.002f; //rad per update
        public const float ELEVATION_SPEED = 0.002f; //rad per update
        public const float MAX_HUD_DISTANCE = 1000;
    }

    static class MyWaypointConstants
    {
        public const int MAXIMUM_WAYPOINT_PATH_NAME_LENGTH = 16;
        public const int MINIMUM_ASTEROID_DIAGONAL_LENGTH_TO_GENERATE_WAYPOINTS = 200;
        public const int MAXIMUM_BOX_DISTANCE_TO_INTERESTING_STUFF_TO_GENERATE_WAYPOINTS = 3000;
        // NEVER increase this from 15000 without discussing it first.
        public const int MAX_WAYPOINTS = 15000;
    }


    static class MyEditorConstants
    {
        public static readonly Vector2 EDITOR_BUTTONS_POSITION_DELTA = new Vector2(0, 0.03f);
        public static readonly Vector2 EDITOR_BUTTON_SIZE = new Vector2(0.135f, 0.025f);
        public const float EDITOR_BUTTON_LABEL_TEXT_SCALE = 0.4f;
        public const int MAX_UNDO_REDO_HISTORY_LIMIT = 10;
        public const int MOVE_DEBRIS_SPEED = 4;
        public const int MOVE_OBJECT_SPEED = 50;
        public const int DEFAULT_CAMERA_SPEED = 6;
        public const float MIN_EDITOR_CAMERA_MOVE_MULTIPLIER = 0.2f;
        public const float MAX_EDITOR_CAMERA_MOVE_MULTIPLIER = 150.0f;
        public const int DEFAULT_EDITOR_CAMERA_MOVE_MULTIPLIER = 8;

        public const int GRID_QUADS_COUNT_ONE_DIRECTION = 4;
        public const int MAX_GRID_QUADS_A_COUNT = 10;
        public const int MAX_GRID_QUADS_B_COUNT = MAX_GRID_QUADS_A_COUNT;
        public static Vector4 DEFAULT_GRID_COLOR = new Vector4(0.6f, 0.6f, 0.6f, 1);
        public static Vector4 COLOR_WHITE = new Vector4(1, 1, 1, 1);
        public static Vector4 COLOR_BLACK = new Vector4(0, 0, 0, 1);

        public const float MAX_DISTANCE_TO_DRAW_GRID = 100.0f;

        public static Vector3 MOUSE_OVER_HIGHLIGHT_COLOR = new Vector3(0.066f, 0.1f, 0.02f);
        public static Vector3 SELECTED_OBJECT_DIFFUSE_COLOR_ADDITION = new Vector3(0.1f, 0.15f, 0.03f);
        public static Vector3 LINKED_OBJECT_DIFFUSE_COLOR_ADDITION = new Vector3(0.25f, 0.06f, 0.06f);
        public static Vector3 INVALID_PREFAB_DIFFUSE_COLOR_ADDITION = new Vector3(0.62f, 0.2f, 0.0f);
        public static Vector3 COLLIDING_OBJECT_DIFFUSE_COLOR_ADDITION = new Vector3(0.22f, 0.0f, 0.0f);

        public const float COLOR_COMPONENT_MIN_VALUE = 0;
        public const float COLOR_COMPONENT_MAX_VALUE = 255;
        public const float EDITOR_DEFAULT_COMPONENT_MIN_VALUE = 0;
        public const float EDITOR_DEFAULT_COMPONENT_MAX_VALUE = 255;
        public const float GAME_MASTER_VOLUME_MIN = 0;

        public const int DELAY_FOR_COLLISION_TIME_FLASHING_IN_MILLIS = 500;
        public const int DELAY_FOR_SMOOTH_OBJECT_MOVEMENT_IN_MILLIS = 500;
        public const int DELAY_OBJECT_MOVEMENT_SOUND_IN_MILLIS = 100;

        public const short GRID_CONVERSION_UNIT = 10;
        public const float BASE_ROTATION_DELTA_ANGLE_IN_DEGREES = 1;
        public const float ROTATION_DELTA_RIGHT_ANGLE_IN_DEGREES = 90;

        public const short CARVING_TOOL_DISTANCE_FROM_CAMERA_MULTIPLIER = 80;
        public const int SECTOR_BORDER_REACHED_WARNING_DELAY = 1000;
        public const int MAX_EDITOR_ENTITIES_LIMIT = 1000;

        public const int MAX_CONTAINER_NUMBER = 50;
    }

    static class MyPrefabContainerConstants
    {
        public const short CONTAINER_CONVERSION_UNIT = 10;
        public const int MAX_PREFABS_IN_CONTAINER = 4096;
        public const float MAX_DISTANCE_FROM_CONTAINER_CENTER = 3200.0f;
        public static Vector3 MAX_CONTAINER_SIZE = new Vector3(MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER * 2, MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER * 2, MyPrefabContainerConstants.MAX_DISTANCE_FROM_CONTAINER_CENTER * 2);
        public const int PREFAB_CONTAINER_COMMIT_AND_RELOAD_DELAY = 50000;
        public const int MAX_ALARM_PLAYING_TIME_IN_MS = 60000;      // in ms, -1... no limit, others... limit in ms
    }

    static class MyInfluenceSphereConstants
    {
        public const short POSITION_CONVERSION_UNIT = 5;
        public const int MAX_SOUND_SPHERES_COUNT_FOR_SORT = 10;
        public const int MAX_DUST_SPHERES_COUNT_FOR_SORT = 10;
        public const int MAX_SOUND_SPHERES_COUNT = 50;
        public const int MAX_DUST_SPHERES_COUNT = 50;
        public static readonly Color DEFAULT_SPHERE_COLOR = Color.Blue;
    }

    static class MyConfigConstants
    {
        //  This password is used for saving/loading some string into config file (e.g. plain password)
        public static readonly string SYMMETRIC_PASSWORD = "63Gasjh4fqA";
    }

    static class MyJoystickConstants
    {
        public const int MAX_AXIS = 65535;
        public const int MIN_AXIS = 0;
        public const int CENTER_AXIS = (MAX_AXIS - MIN_AXIS) / 2;
        public const float ANALOG_PRESSED_THRESHOLD = 0.5f;  // 0 = neutral, 1 = fully to one side
    }

    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    //  IMPORTANT: If you change order or names in this enum, update it also in MyEnumsToStrings
    enum MyGameControlEnums : byte
    {
        FIRE_PRIMARY = 0,
        FIRE_SECONDARY = 1,
        FIRE_THIRD = 2,
        FIRE_FOURTH = 3,
        FIRE_FIFTH = 4,
        FORWARD = 5,
        REVERSE = 6,
        STRAFE_LEFT = 7,
        STRAFE_RIGHT = 8,
        UP_THRUST = 9,
        DOWN_THRUST = 10,
        ROLL_LEFT = 11,
        ROLL_RIGHT = 12,
        WHEEL_CONTROL = 13,
        HEADLIGHTS = 14,
        HEADLIGTHS_DISTANCE = 15,
        HARVEST = 16,
        DRILL = 17,
        USE = 18,
        INVENTORY = 19,
        GPS = 20,
        MISSION_DIALOG = 21,
        TRAVEL = 22,
        QUICK_ZOOM = 23,
        ZOOM_IN = 24,
        ZOOM_OUT = 25,
        SELECT_AMMO_BULLET = 26,
        SELECT_AMMO_MISSILE = 27,
        SELECT_AMMO_CANNON = 28,
        SELECT_AMMO_UNIVERSAL_LAUNCHER_FRONT = 29,
        AUTO_LEVEL = 30,
        REAR_CAM = 31,
        SELECT_AMMO_UNIVERSAL_LAUNCHER_BACK = 32,
        MOVEMENT_SLOWDOWN = 33,
        VIEW_MODE = 34,
        WEAPON_SPECIAL = 35,
        CHANGE_DRONE_MODE = 36,
        ROTATION_LEFT = 37,
        ROTATION_RIGHT = 38,
        ROTATION_UP = 39,
        ROTATION_DOWN = 40,
        AFTERBURNER = 41,
        PREVIOUS_CAMERA = 42,
        NEXT_CAMERA = 43,
        FIRE_HOLOGRAM_FRONT = 44,
        FIRE_HOLOGRAM_BACK = 45,
        FIRE_BASIC_MINE_FRONT = 46,
        FIRE_BASIC_MINE_BACK = 47,
        FIRE_SMART_MINE_FRONT = 48,
        FIRE_SMART_MINE_BACK = 49,
        FIRE_FLASH_BOMB_FRONT = 50,
        FIRE_FLASH_BOMB_BACK = 51,
        FIRE_DECOY_FLARE_FRONT = 52,
        FIRE_DECOY_FLARE_BACK = 53,
        FIRE_SMOKE_BOMB_FRONT = 54,
        FIRE_SMOKE_BOMB_BACK = 55,
        DRONE_DEPLOY = 56,
        DRONE_CONTROL = 57,
        CONTROL_SECONDARY_CAMERA = 58,
        NOTIFICATION_CONFIRMATION = 59,
        PREV_TARGET = 60,
        NEXT_TARGET = 61,
        CHAT = 62,
        SCORE = 63,
    }

    enum MyEditorControlEnums : byte
    {
        PRIMARY_ACTION_KEY = 0,
        SECONDARY_ACTION_KEY = 1,
        INCREASE_GRID_SCALE = 2,
        DECREASE_GRID_SCALE = 3,
        VOXEL_HAND = 4,
        SWITCH_GIZMO_SPACE = 5,
        SWITCH_GIZMO_MODE = 6,
    }

    static class MyDebugDrawCachedLinesConstants
    {
        public const int MAX_LINES_IN_CACHE = 1000;
    }

    static class MyHarvestingTubeConstants
    {
        //  Max distance harvesting device can go from ship
        public const int MAX_DISTANCE_OF_HARVESTING_DEVICE = 30;

        public const float SHAKE_DURING_EJECTION = 2.25f;
        public const float SHAKE_DURING_IN_VOXELS = 3.0f;
        public const float EJECTION_SPEED_IN_METERS_PER_SECOND = 10 * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
        public const float FAST_PULL_BACK_IN_METERS_PER_SECOND = 100 * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
        public const int DUST_PARTICLE_GENERATOR_TIME_DELTA_IN_MILLISECONSD = 100;
        public const float DISTANCE_TO_PLUG_IN_THE_TUBE = 0.1f;
        public const float INTERVAL_TO_CHECK_FOR_VOXEL_CONNECTION_IN_MILISECONDS = 100;

        public const float MINED_CONTENT_RATIO = 0.0002f;
    }

    static class MyDrillDeviceConstants
    {
        public const float ROTATION_SPEED_PER_SECOND = 1f * MathHelper.Pi * 60 * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
        public const float ROTATION_ACCELERATION = 0.02f * 60 * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
        public const float ROTATION_DECELERATION = ROTATION_ACCELERATION * .5f;
        public const float DRILL_INTERVAL_IN_MILISECONDS = 1000;
        public const float DRILL_EJECT_INTERVAL_IN_MILISECONDS = 1000;
        public static float DRILL_EJECTING_SPEED = 0.20f;
        public const float EJECT_DISTANCE_MULTIPLIER = 0.5f;
        public const float BIG_SPHERE_RADIUS_MULTIPLIER = 2f;
        public const float TIME_TO_DRILL_VOXEL_IN_MILISECONDS = 1500;
        public const float PARTICLES_ADDING_INTERVAL_IN_MILISECONDS = 150;
        public const float SHAKE_DURING_ROTATION = 2.3f;
        public const float SHAKE_DURING_IN_VOXELS = 4f;
        public const float MAX_RADIUS_RANDOM_MULTIPLIER = 1.1f; // maximum multiplier for directional drills' radius
    }

    static class MyCrusherDrillDeviceConstants
    {
        public const float RADIUS = 2f;
        public const float RANGE = 2f;
        public const float MAX_ROTATING_SPEED_DRILLING = 3f;
        public const float MAX_ROTATING_SPEED_IDLE = 0.15f;
        public const float DAMAGE_PER_SECOND = 70;
        public const int MIN_DRILLING_DURATION = 800;
    }

    static class MyLaserDrillDeviceConstants
    {
        public const float RANGE = 15f;
        public const float RADIUS = 6.5f;
        public const float DAMAGE_PER_SECOND = 50;
        public const int MIN_DRILLING_DURATION = 600;
    }

    static class MyNuclearDrillDeviceConstants
    {
        public const float RADIUS = 3f;
        public const float RANGE = 3f;
        public const float MAX_ROTATING_SPEED_DRILLING = 3f;
        public const float MAX_ROTATING_SPEED_IDLE = 0.4f;
        public const float DAMAGE_PER_SECOND = 100;
        public const int MIN_DRILLING_DURATION = 200;
    }

    static class MyPressureDrillDeviceConstants
    {
        public const float RADIUS = 20f;
        public const int SHOT_INTERVAL_IN_MILISECONDS = 6000;
        public const float DAMAGE = 500;
    }

    static class MySawDrillDeviceConstants
    {
        public const float RANGE = 25f;
        public const float RADIUS = 5f;
        public const float DAMAGE_PER_SECOND = 50;
        public const int MIN_DRILLING_DURATION = 200;
    }

    static class MyThermalDrillDeviceConstants
    {
        public const float RADIUS = 2.5f;
        public const float RANGE = 2.5f;
        public const float MAX_ROTATING_SPEED_DRILLING = 1.5f;
        public const float MAX_ROTATING_SPEED_IDLE = 0.4f;
        public const float DAMAGE_PER_SECOND = 85;
        public const int MIN_DRILLING_DURATION = 200;
    }

    static class MyReflectorConstants
    {
        public static readonly Vector4 SHORT_REFLECTOR_LIGHT_COLOR = new Color(255, 255, 255, 255).ToVector4();
        public static readonly float SHORT_REFLECTOR_RANGE_FORWARD = 750;
        public static readonly float SHORT_REFLECTOR_RANGE_BACKWARD = 200;
        public static readonly float SHORT_REFLECTOR_CONE_ANGLE_FORWARD = 1.0f - (float)Math.Cos(3.14f / 8.5f * 1.5f);
        public static readonly float SHORT_REFLECTOR_CONE_ANGLE_BACKWARD = 1.0f - (float)Math.Cos(3.14f / 8.5f * 4.0f);
        public static readonly float SHORT_REFLECTOR_BILLBOARD_LENGTH = 40f;
        public static readonly float SHORT_REFLECTOR_BILLBOARD_THICKNESS = 6f;

        public static readonly Vector4 LONG_REFLECTOR_LIGHT_COLOR = new Color(255, 235, 255, 255).ToVector4();
        public static readonly float LONG_REFLECTOR_RANGE_FORWARD = SHORT_REFLECTOR_RANGE_FORWARD * 2;
        public static readonly float LONG_REFLECTOR_RANGE_BACKWARD = SHORT_REFLECTOR_RANGE_BACKWARD * 1;
        public static readonly float LONG_REFLECTOR_CONE_ANGLE_FORWARD = 1.0f - (float)Math.Cos((3.14f / 8.5f) * 0.6f);
        public static readonly float LONG_REFLECTOR_CONE_ANGLE_BACKWARD = 1.0f - (float)Math.Cos((3.14f / 8.5f * 4.0f) * 0.6f);
        public static readonly float LONG_REFLECTOR_BILLBOARD_LENGTH = SHORT_REFLECTOR_BILLBOARD_LENGTH * 1.5f;
        public static readonly float LONG_REFLECTOR_BILLBOARD_THICKNESS = SHORT_REFLECTOR_BILLBOARD_THICKNESS * 0.6f;

        public static readonly float CHANGE_RANGE_INTERVAL_IN_MILISECONDS = 1000;
    }

    static class MySecondaryCameraConstants
    {
        public const float SECONDARY_CAMERA_DESCRIPTION_SCALE = 0.7f;
        public const float NEAR_PLANE_DISTANCE = 1.0f;
        public const int FIELD_OF_VIEW = 50;
    }

    static class MyDroneConstants
    {
        public const float NEAR_PLANE_DISTANCE = 0.3f;

        /// <summary>
        /// Drone models are too small, so they need a bigger physics entity to represent them.
        /// This gives the multiplier for the radius of the sphere representing the physics body
        /// of the drone compared to the default (model.boundingSphere.radius).
        /// </summary>
        public const float DRONE_PHYSICS_SIZE_MULTIPLIER = 2.5f;
    }

    static class MyMineBaseConstants
    {
        public const float TIME_TO_ACTIVATE_MINE_IN_MILISECONDS = 1000;
    }

    static class MyFlashBombConstants
    {
        public const int FADE_IN_TIME = 100; //ms
        public const int FLASH_TIME = 1000; //ms
        public const int FADE_OUT_TIME = 1000; //ms

        public const float FLASH_RADIUS = 150; //m - if you see the explosion in this radius, you get blind
        public const int TIME_TO_ACTIVATE = 1000; //ms
    }

    static class MyDecoyFlareConstants
    {
        public const int MAX_LIVING_TIME = 11000; //ms
        public const float DECOY_ATTRACT_RADIUS = 300; //m - How far will decoy attract missiles
        public const float DECOY_KILL_RADIUS = 50; //m - When missile explode
        public const int TIME_TO_ACTIVATE = 1000; //ms
        public const int FLARES_COUNT = 32;
    }

    static class MyHologramConstants
    {
        public const int TIME_TO_ACTIVATE = 50;    // ms
        public const int TIME_TO_DEACTIVATE = 60000; // ms
        public const float APPEAR_SPEED = 0.1f;      // percentage per frame
        public const int FLICKER_FREQUENCY = 120;    // number of frames per flicker (the lower, the more frequent)
        public const float FLICKER_DURATION = 60;    // length of a flicker in ms
        public const float FLICKER_MAX_SIZE = 1.1f;  // multiplier of max flicker scale of the hologram
    }

    static class MyIlluminatingShellsConstants
    {
        public const int MAX_LIVING_TIME = 20000; //ms
        public const int DIYNG_TIME = 2000; //ms
        public const float LIGHT_RADIUS = MyLightsConstants.MAX_POINTLIGHT_RADIUS; //m
        public static Vector4 LIGHT_COLOR = Vector4.One;
    }

    static class MySmokeBombConstants
    {
        public const int TIME_TO_ACTIVATE = 1000; //ms
        public const float SMOKE_TIME = 1000; //ms
        public const int MAX_LIVING_TIME = 20000; //ms
    }

    public class MyMineSmartConstants
    {
        public const float INTERVAL_TO_SEARCH_FOR_ENEMY_IN_MILISECONDS = 500;
        public const float CHASE_SPEED_MULTIPLIER = 0.02f;
    }

    static class MySphereExplosiveConstants
    {
        public const int TIME_TO_ACTIVATE = 3000; //ms
    }

    static class MyDirectionalExplosiveConstants
    {
        public const int TIME_TO_ACTIVATE = 3000; //ms
        public const float EXPLOSION_LENGTH = 400; //m
    }

    static class MyTimeBombConstants
    {
        public const float EXPLOSION_RADIUS = 40; //m

        public static readonly int[] TIMEOUT_ARRAY = new int[] { 1, 3, 10, 30 }; // seconds
    }

    static class MyRemoteBombConstants
    {
        public const int MAXIMUM_LIVING_TIME = 60 * 1000; //ms
    }

    static class MyRemoteCameraConstants
    {
        public const float WEIGHT = 30; //kg
        public const float SPEED = 2; //m/s
        public const float ROTATION_SENSITIVITY_NON_MOUSE = 4.0f;
        public const float TIME_TO_ACTIVATE_GROUP_MASK = 1000; // ms
        public const float MAX_ANGLE_SQUARED = MathHelper.PiOver4 * MathHelper.PiOver4; // radians squared - 45 degrees
    }


    static class MyMissionsConstants
    {
        //public const float MISSION_SPHERE_RADIUS = 100;
        //public const float MISSION_SPHERE_RADIUS_SQUARED = MISSION_SPHERE_RADIUS * MISSION_SPHERE_RADIUS;
        public static Vector3 OBJECT_HIGHTLIGHT_COLOR = new Vector3(0.01f, 0.07f, 0.31f);
        public const int NEW_OBJECTIVE_FOR_TIME = 10000;    // in ms
        public const float NEW_OBJECTIVE_BLINK_ON_HIGHLIGHT = 1f;
        public const float NEW_OBJECTIVE_BLINK_OFF_HIGHLIGHT = 0f;
        public const int NEW_OBJECTIVE_BLINK_ON_TIME = 300;     // in ms
        public const int NEW_OBJECTIVE_BLINK_OFF_TIME = 50;     // in ms
    }

    static class MySunConstants
    {
        // for zoom values lower than this, there are no glare effects, because occlusion doesn't work well for some reason
        public const float ZOOM_LEVEL_GLARE_END = 0.6f;

        // for screen edge falloff of glare - when center of sun is this many pixels away from the border of the screen, glare starts to fall off
        public const float SCREEN_BORDER_DISTANCE_THRESHOLD = 25;

        // for changing the maximum intensity of the sun (when it is in the centre of the screen). Acceptable values are [0, 10]
        public const float MAX_GLARE_MULTIPLIER = 2.5f;

        // distance in which render sun
        public const float RENDER_SUN_DISTANCE = 50000;

        // sun glow and glare size multiplier
        public const float SUN_SIZE_MULTIPLIER = 1;

        // minimum and maximum sun glow and glare sizes (in no specific unit)
        public const float MIN_SUN_SIZE = 250;  // will be used in sectors distant from sun (e.g. post-uranus)
        public const float MAX_SUN_SIZE = 4000; // will be used in sectors close to sun

        // for sun occlusion query
        public static readonly int MIN_QUERY_SIZE = 1000;

        // maximum number of occlusion query pixels - higher numbers are discarded as faulty query results
        public const int MAX_SUNGLARE_PIXELS = 100000;
    }

    static class MySmallShipConstants
    {
        public const float LOW_ELECTRICITY_FOR_RADAR = 0.05f;       // percentage        
        public const float DETECT_INTERVAL = 200;                   // in miliseconds
        public const float DETECT_SHIP_RADIUS = 50;                 // in meters
        public const float DETECT_FOUNDATION_FACTORY_RADIUS = 500;  // in meters
        public const float COLLISION_FRICTION_MULTIPLIER = 0.6f;    // ratio (0 no change in speed on collision, 1 full stop on collision)
        public const float MAX_UPDATE_DISTANCE = 200;                 // in meters

        // HUD waring's levels
        public const float WARNING_ELECTRICITY_CRITICAL_LEVEL = 0.2f;   // precentage
        public const float WARNING_ELECTRICITY_LOW_LEVEL = 0.4f;        // precentage
        public const float WARNING_ARMOR_CRITICAL_LEVEL = 0.2f;         // precentage
        public const float WARNING_ARMOR_LOW_LEVEL = 0.4f;              // precentage
        public const float WARNING_AMMO_CRITICAL_LEVEL = 0.2f;          // precentage
        public const float WARNING_AMMO_LOW_LEVEL = 0.4f;               // precentage
        public const float WARNING_FUEL_CRITICAL_LEVEL = 0.2f;          // precentage
        public const float WARNING_FUEL_LOW_LEVEL = 0.4f;               // precentage
        public const float WARNING_OXYGEN_CRITICAL_LEVEL = 0.2f;        // precentage
        public const float WARNING_OXYGEN_LOW_LEVEL = 0.4f;             // precentage
        public const float WARNING_DAMAGE_CRITICAL_LEVEL = 0.2f;        // precentage
        public const float WARNING_DAMAGE_ALERT_LEVEL = 0.4f;           // precentage
        public const float WARNING_HEALTH_CRITICAL_LEVEL = 0.2f;        // precentage
        public const float WARNING_HEALTH_LOW_LEVEL = 0.4f;             // precentage
        public const float WARNING_RADIATION_DAMAGE_PER_SECOND = MyPlayer.DEFAULT_PLAYER_MAX_HEALTH / 20f;  // damage per second
        public const float WARNING_SUN_WIND_MAX_DISTANCE_BEHIND_PLAYER_SQR = 1000f * 1000f; // in meters

        // HUD warning's sound intervals
        public const int WARNING_ELECTRICITY_CRITICAL_INVERVAL = 10000;   // in miliseconds
        public const int WARNING_ELECTRICITY_LOW_INVERVAL = 20000;        // in miliseconds
        public const int WARNING_ARMOR_NO_INVERVAL = 10000;               // in miliseconds
        public const int WARNING_ARMOR_CRITICAL_INVERVAL = 10000;         // in miliseconds
        public const int WARNING_ARMOR_LOW_INVERVAL = 20000;              // in miliseconds
        public const int WARNING_AMMO_NO_INVERVAL = 10000;                // in miliseconds
        public const int WARNING_AMMO_CRITICAL_INVERVAL = 10000;          // in miliseconds
        public const int WARNING_AMMO_LOW_INVERVAL = 20000;               // in miliseconds
        public const int WARNING_FUEL_NO_INVERVAL = 10000;                // in miliseconds
        public const int WARNING_FUEL_CRITICAL_INVERVAL = 10000;          // in miliseconds
        public const int WARNING_FUEL_LOW_INVERVAL = 20000;               // in miliseconds
        public const int WARNING_NO_OXYGEN_INVERVAL = 10000;              // in miliseconds
        public const int WARNING_OXYGEN_CRITICAL_INVERVAL = 10000;        // in miliseconds
        public const int WARNING_OXYGEN_LOW_INVERVAL = 20000;             // in miliseconds
        public const int WARNING_OXYGEN_LEAKING_INVERVAL = 20000;         // in miliseconds
        public const int WARNING_DAMAGE_CRITICAL_INVERVAL = 10000;        // in miliseconds
        public const int WARNING_DAMAGE_ALERT_INVERVAL = 20000;           // in miliseconds
        public const int WARNING_RADAR_JAMMED_INVERVAL = 20000;           // in miliseconds
        public const int WARNING_ENEMY_ALERT_INVERVAL = 15000;            // in miliseconds
        public const int WARNING_HEALTH_CRITICAL_INVERVAL = 10000;        // in miliseconds
        public const int WARNING_HEALTH_LOW_INVERVAL = 20000;             // in miliseconds
        public const int WARNING_MISSILE_ALERT_INTERVAL = 1200;           // in miliseconds
        public const int WARNING_SOLAR_WIND_INTERVAL = 5000;              // in miliseconds
        public const int WARNING_HEALTH_CONSTANT1 = 0;
        public const int WARNING_HEALTH_CONSTANT2 = 0;
        public const int WARNING_HEALTH_CONSTANT3 = 0;
        public const int WARNING_HEALTH_CONSTANT4 = 0;
        public const int WARNING_HEALTH_CONSTANT5 = 0;

        public const int WARNING_FRIENDLY_FIRE_INTERVAL = 3000;           // in miliseconds
        public const int WARNING_RADIATION_INTERVAL = 5000;               // in miliseconds
        public const int GEIGER_BEEP_INTERVAL = 2000;                     // in miliseconds
        public const int RADIATION_DAMAGE_MAX_TIME = 1000;                // in miliseconds

        public const int WARNING_EXPLANATION_INITIAL_DELAY = 30000;       // in miliseconds
        public const int WARNING_EXPLANATION_INTERVAL = 180000;           // in miliseconds

        public const float WARNING_ENEMY_ALERT_MAX_DISTANCE_SQR = 500f * 500f;  // in meters

        public const int LOCK_TARGET_TIME = 750;                          // in miliseconds
        public const int LOCK_TARGET_CHECK_TIME = 500;                    // in miliseconds
        public const float LOCK_TARGET_OVERLAP = 1.7f;                    // percents

        public static float DRILL_FUEL_CONSUMPTION = 0.1f;                // fuel per sec
        public static float HARVESTER_FUEL_CONSUMPTION = 0.1f;            // fuel per sec
        public static float WEAPON_ELECTRICITY_CONSUMPTION = 0.001f;      // kWh per one shot

        public static float MINIMUM_ELECTRICITY = -10.0f;
        public static float ELECTRICITY_ENGINE_PRODUCTION = -10.0f;

        public static float NO_SLOWDOWN_MAX_SPEED = 120f;
        public static float NO_SLOWDOWN_SLOWDOWN_FORCE = 4200f;

        public static int POSITION_MEMORY_SIZE = 1000;

        public static float EXPLOSION_SHOCK_TIME = 1.0f;
        public static float SHOCK_TIME_VAR = 0.2f; // Shock time variability

        public static bool INCLUDE_CARGO_WEIGHT = false;

        public const int DRONE_RELEASE_INTERVAL = 1500; // ms
        public const float EMP_SMALLSHIP_DISABLE_DURATION_MULTIPLIER = 0.05f;

        public const float SHIP_HEALTH_RATIO_TO_OXYGEN_LEAKING_MIN = 0.4f;      // in percents
        public const float SHIP_HEALTH_RATIO_TO_OXYGEN_LEAKING_MAX = 0f;        // in percents
        public const float OXYGEN_LEFT_AT_MIN_DAMAGE_LEVEL = 600f;              // in sec 
        public const float OXYGEN_LEFT_AT_MAX_DAMAGE_LEVEL = 120f;              // in sec

        public static float DAMAGE_OVER_TIME_INTERVAL = 1.0f;                   // in sec
        public static float MAX_FRIENDLY_DAMAGE = 125.0f;                       // aggro damage for attacked friend

        // Gives the scaling factor of all the smallships model in the content processor (1 corresponds to 0.01 scaling factor)
        // Has to be changed if the factors in the content processor changes.
        public const float ALL_SMALL_SHIP_MODEL_SCALE = 1.5f;
        public const float FRIEND_SMALL_SHIP_MODEL_SCALE = 2.0f;

        public const int SMALL_CARGO_CAPACITY = 24;
        public const int NORMAL_CARGO_CAPACITY = 36;
        public const int LARGE_CARGO_CAPACITY = 60;
        public const float MAX_HUD_DISTANCE = 1000;
    }

    static class MyShipConstants
    {
        public const float CRIPPLE_HEALTH = 0.05f; // percentage
        public const float DAMAGED_HEALTH = 0.5f; // percentage
    }

    static class MyMedicineConstants
    {
        public const float MEDIKIT_HEALTH_TO_ACTIVATE = 99; // triggers when health below this
        public const float MEDIKIT_DURATION = 50;
        public const float MEDIKIT_HEALTH_RESTORED_PER_SECOND = 20;  // one dose heals 1 health and depletes in 50 ms

        public const float ANTIRADIATION_MEDICINE_DURATION = 100000;  // one dose for one 100 seconds of resistance
        public const float ANTIRADIATION_MEDICINE_RADIATION_DAMAGE_MULTIPLIER = 0.02f;

        public const float PERFORMANCE_ENHANCING_MEDICINE_DURATION = 60000;  // 1 minute duration
        public const float PERFORMANCE_ENHANCING_MEDICINE_OXYGEN_CONSUMPTION_MULTIPLIER = 0.2f;

        public const float HEALTH_ENHANCING_MEDICINE_DURATION = 30000;  // half a minute duration
        public const float HEALTH_ENHANCING_MEDICINE_DAMAGE_MULTIPLIER = 0.2f;
    }

    static class MyFoundationFactoryConstants
    {
        public const float RETURN_AMOUNT_RATIO = 0.5f;
        public const float SPHERE_RADIUS = 2f;
        public const int MAX_ITEMS_IN_INVENTORY = 1000;
    }

    static class MyMainMenuConstants
    {
        public const string BUY_NOW_URL = "http://www.minerwars.com/Store/?aid=IngameBuy";
        public const string IE_PROCESS = "IExplore.exe";
        public const int BLINK_INTERVAL = 500;                                              // in ms
        public const float BUY_BUTTON_SIZE_MULTIPLICATOR = 1.2f;
        public const float BUY_BUTTON_WIDTH_MULTIPLICATOR = 0.9f;
        public static readonly Vector4 BUY_BUTTON_BACKGROUND_COLOR = new Vector4(0.8f, 0.8f, 0.8f, 0.95f);
        public static readonly Vector4 BUY_BUTTON_TEXT_COLOR = new Vector4(0.7f, 0.45f, 0f, 0.7f);
    }

    public enum MyLodTypeEnum
    {
        LOD0,     //  Use when cell contains data without LOD, so they are as they are
        LOD1,         //  Use when cell contains LOD-ed data (less detail, ...)
        LOD_NEAR    // Used for cockpit and weapons
    }

    public static class MyItemFilterConstants
    {
        public static ItemCategory[] CategoryAmmo = {ItemCategory.AMMO, ItemCategory.WEAPON,};
        public static ItemCategory[] Devices = {ItemCategory.DEVICE, ItemCategory.DEFAULT};
        public static ItemCategory[] ConsumAndMedical = { ItemCategory.CONSUMABLE, ItemCategory.MEDICAL,  };
        public static ItemCategory[] Ores = { ItemCategory.ORE };
        public static ItemCategory[] GoodsAndIllegal = { ItemCategory.GOOODS, ItemCategory.ILLEGAL, };
        public static ItemCategory[] All = (ItemCategory[])Enum.GetValues(typeof(ItemCategory));
    }

    static class MyAIConstants
    {
        public const float SLEEP_DISTANCE_FROM_PATH_SQUARED = 1000 * 1000;

        public const float PATHFINDING_SHIP_RADIUS = 5f;
        public const int PATHFINDING_MAX_START_RAYCASTS = 12;       // must be >= 4 (see MyWayPointGraph.GetClosestWaypointReachableByShip).
        public const int PATHFINDING_MAX_END_RAYCASTS = 12;         // must be >= 4 (see MyWayPointGraph.GetClosestWaypointReachableByShip).
        
        //If bot is further than this distance he has to return to his leader
        public const float MAX_LEADER_DISTANCE_SQR = 500 * 500;    // max distance for follower bot from leader (MySmallShipBot.LeaderLostEnabled)
        //If bot is further than this distance, he cannot start any attack (but can finish current attack if not further than MAX_LEADER_DISTANCE_SQR)
        public const float FAR_LEADER_DISTANCE_SQR = MAX_LEADER_DISTANCE_SQR * 0.75f;

        public static float BOT_FOV = MathHelper.ToRadians(150.0f);
        public static float BOT_FOV_COS = (float)Math.Cos(MyAIConstants.BOT_FOV / 2);
        public const float BOT_FOV_RANGE = 1000;                  // Normal range of view
        public const float BOT_FOV_RANGE_HIDDEN = 200;             // Range of view for targets with disabled engines and turned off lights
        public const float FORMATION_SPACING = 30;
        public const int MIN_AFTERBURNER_OFF_TIME = 500;
        public const int MIN_AFTERBURNER_ON_TIME = 500;
    }

    public static class MyModelsStatisticsConstants
    {
        public static bool MODEL_STATISTICS_WRONG_LODS_ONLY = true;

        public static char MODEL_STATISTICS_CSV_SEPARATOR = ',';

        public static bool GET_MODEL_STATISTICS_AUTOMATICALLY = false;

        public static MyMissionID[] MISSIONS_TO_GET_MODEL_STATISTICS_FROM = { };

        public static int ACTUAL_MISSION_FOR_MODEL_STATISTICS = 0;
    }

    public static class MyMissionConstants
    {
        public const float MADELYN_REFILL_TIME = 3 * 60;
    }

    public static class MyPlayerConstants 
    {
        public const float MONEY_MAX = 1000000000f;
        public const float MONEY_MIN = -MONEY_MAX;
    }

    public static class MyGamePlayCheatsConstants 
    {
        public const int CHEAT_INCREASE_CARGO_CAPACITY_MAX_ITEMS = 1000;
    }
}