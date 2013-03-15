using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils;
using System;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Missions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.AppCode.Game.Utils
{
    static class MyFakes
    {
        //  For testing float precisions on large positions
        public static bool TEST_STORY_MISSION_OBJECTS_AT_SECTOR_BORDER_FOR_LARGE_POSITION_TEST = false;
        public static Vector3 TEST_STORY_MISSION_OBJECTS_LARGE_POSITION_OFFSET = new Vector3(60000, 60000, 60000);

        public const bool PLAY_MMO_BUTTON_IMPLEMENTED = true;
        public const bool PLAY_SANDBOX_BUTTON_IMPLEMENTED = true;// MyMwcFinalBuildConstants.TYPE == MyMwcFinalBuildType.TEST;
        public const bool EDITOR_BUTTON_IMPLEMENTED = true;//MyMwcFinalBuildConstants.TYPE == MyMwcFinalBuildType.TEST;
        public const bool PLAY_STORY_BUTTON_IMPLEMENTED = true;//MyMwcFinalBuildConstants.TYPE == MyMwcFinalBuildType.TEST;
        public const bool PROFILE_BUTTON_IMPLEMENTED = true;//MyMwcFinalBuildConstants.TYPE == MyMwcFinalBuildType.TEST;
        public const bool CREDITS_BUTTON_IMPLEMENTED = true;
        public const bool LOAD_LAST_CHECKPOINT_ENABLED = true;
        //public const bool LOAD_CHAPTER = false;
        public const bool JOIN_FRIENDS_GAME = false;
        public static bool DRAW_PLAYER_MINER_SHIP = true;
        public const bool SHOW_NEW_INVENTORY_SCREEN = true;
        public const bool DETECT_ORE_DEPOSITS_IN_VOXEL_MAPS = true;
        public const bool RAPID_VOXEL_HAND_SHAPING_ENABLED = false;

        //public const bool MAREK_TESTING_PROCEDURAL_GENERATOR = false;
        public const bool VOXEL_IMPORT = false;
        public static readonly string VOXEL_IMPORT_MODEL = "Models2\\ObjectsStatic\\LargeShips\\LargeShip_Kai";
        public static readonly MyMwcVector3Int VOXEL_IMPORT_SIZE = new MyMwcVector3Int(128, 128, 128);

        public static bool DEBUGDRAW_SPAWN_POINT = false;
        public static bool SPAWN_FRIENDS = false;
        public static bool SPAWN_POINT_INITIAL_INSERTION_IN_SCENE = false;

        public const bool STRANGE_PARTICLES_WHEN_DUST_ON_STATIC_ASTEROIDS = false;

        public const bool EDITOR_UNDO_REDO_IMPLEMENTED = false;
        public const bool EDITOR_CREATE_ASTEROID_IMPLEMENTED = true;
        public const bool EDITOR_ADD_PREFAB_CONTAINER_IMPLEMENTED = true;
        public const bool EDITOR_ADD_PREFAB_MODULE_IMPLEMENTED = true;
        public const bool EDITOR_FOG_SETTINGS_IMPLEMENTED = false;
        public const bool EDITOR_SUN_SETTINGS_IMPLEMENTED = false;
        public const bool EDITOR_SAVE_ASTEROID_TO_FILE_IMPLEMENTED = false;
        public const bool EDITOR_CLEAR_ASTEROID_CONTENT_IMPLEMENTED = false;
        public const bool EDITOR_CLEAR_ASTEROID_MATERIALS_IMPLEMENTED = false;
        public const bool EDITOR_COPY_SELECTED_IMPLEMENTED = true;
        public const bool EDITOR_DISABLE_UNDO_REDO = false;
        public const bool EDITOR_ENABLE_HOWTO_NOTIFICATION = false;

        public static bool SMALL_SHIPS_GLARE = false; // simon - turn on glare small ship reflectors

        //If enabled, you can always travel with solarmap (only for debugging)
        public static bool ENABLE_SOLAR_MAP = false;

        public static bool USE_LARGE_SHIP_HANGAR_DETECTION = true;

        public static readonly MyMwcVoxelMaterialsEnum? SINGLE_VOXEL_MATERIAL = null;//MyMwcVoxelMaterialsEnum.Ice_01;

        // Allows to draw circle faction areas in solar maps, also activates tool for input of these areas
        public static bool DRAW_FACTION_AREAS_IN_SOLAR_MAP = false;

        // Loads all model textures immediately
        public const bool LOAD_TEXTURES_IMMEDIATELY = true;
        public const bool LOAD_MODELS_IMMEDIATELY = true;

        // Tests multiple load/unload of sectors and writes result in the log
        public const bool TEST_MULTIPLE_LOAD_UNLOAD = false;
        public const Missions.MyMissionID TEST_MULTIPLE_LOAD_UNLOAD_MISSION = MyMissionID.FORT_VALIANT;

        // Tests multiple save/load and checks whether all entities in scene remains (by comparing EntityId sets), go to editor and load STORY sector you want to test
        public const bool TEST_MULTIPLE_SAVE_LOAD = false;

        // Tests mission gameplay
        // Starts in Russian Assault, going with ctrl+del through all objectives, then next mission
        // until game end. 
        public const bool TEST_MISSION_GAMEPLAY = false;
        public const int TEST_MISSION_GAMEPLAY_AUTO_KILLS = 2;
        public const int TEST_MISSION_GAMEPLAY_DURATION = 3000; //duration of one test in ms

        // Default cheats
        public static MyGameplayCheatsEnum? DEFAULT_CHEATS = null;//MyGameplayCheatsEnum.ALL_WEAPONS;

        // Disables auto save in story, still can save by pressing CTRL+F5 when developer keys are enabled
        public static bool DISABLE_AUTO_SAVE = false;

        // Disable keeping spectator in sector boundaries
        public static bool DISABLE_SPECTATOR_IN_BOUNDARIES = false;

        public const bool MANY_DEBRIS = false;

        // Disables player head shake (good when you're testing shooting)
        public static bool DISABLE_CAMERA_HEADSHAKE = false;

        public static bool ALT_AS_DEBUG_KEY = true;
        public static bool CONTROLS_MOVE_ENABLED = false;

        public static bool DEBUG_DRAW_COLLIDING_ENTITIES = false;

        public static bool TEST_MULTILINE_CONTROL = false;

        public static bool TEST_DNS_UNAVAILABLE = false;

        public static bool OPTIMIZATION_FOR_300_SMALLSHIPS = true;

        public static bool VOXEL_MAP_SMALLER_BOUNDARIES = true;

        public static bool FAKE_SCREEN_ENABLED = false;

        public static bool ENABLE_OBJECT_COUNTS_LIMITS = false;

        public static bool SHOW_UNPOWERED_PREFABS = false;

        public static bool SIMPLIFY_VOXEL_MESH = false;

        public static bool ADD_DRONES_TO_INVENTORY = false;

        public static bool HIDE_CENTER_SECTOR_MARKS = false;

        public static bool ENABLE_LOADING_AFTER_TRADING = false;

        public static bool DRAW_WEAPONS = true;

        public static bool ENABLE_PREFABS_AUTO_CHARGING = false;

        public static bool ENABLE_REPLAY_ANIMATION_IN_LOADED_SECTOR = false;

        public static bool ENABLE_GENERATED_WAYPOINTS_IN_EDITOR = false;

        public static bool ENABLE_BOTS_FOV_WHEN_RADAR_JAMMER = true;

        public static bool INDESTRUCTIBLE_PREFABS = false;

        public static bool GPS_ALWAYS_ON = false;

        public static bool ENABLE_RANDOM_METEOR_SHOWER = false;

        public static bool ENABLE_RANDOM_ICE_STORM = false;

        public static bool ENABLE_EXTRACT_PREFABS = true;

        //public static bool SIMPLE_DEBUG_SCREEN = MyMwcFinalBuildConstants.GetValueForBuildType(true, true, false);
        public static bool SIMPLE_DEBUG_SCREEN = false; //enable everywhere, there is no obfuscation now

        public static bool DISABLE_BOT_MANEUVERING = false;

        public static bool ENABLE_MENU_VIDEO_BACKGROUND = true;

        public static bool SHOW_HUD_NAMES = true;

        public static bool SHOW_HUD_DISTANCES = true;

        public static bool POST_MYENTITYANIMATOR_VALUES = false;

        public static bool ENABLE_DETECTORS_IN_EDITOR_GAME = true;

        public static bool ENABLE_BACK_CAMERA = true;

        public static bool ENABLE_BOT_MISSILES_AND_CANNON = true;

        // This will save mission checkpoints as templates on each save
        public static bool CHAPTER_ON_EACH_MISSION = true;

        public static bool ENABLE_MULTIPLAYER = true;

        public static bool HIGHLIGHT_WRONG_MODELS = false;

        public static bool ENABLE_GENERATED_ASTEROIDS = true;

        public static bool ENABLE_AUTOSKIPPING_ENDMISSION_DIALOGUE = false;

        public static bool ENABLE_VOXEL_TRIANGLE_CACHING = false;

        /// <summary>
        /// Use this when you want to render gui previews with the renderer and you want to have 0 alpha for the background.
        /// </summary>
        public static bool RENDER_PREVIEWS_WITH_CORRECT_ALPHA = false;

        public static bool DRAW_TESTED_TRIANGLES_IN_VOXEL_LINE_INTERSECTION = false;
        public static bool DRAW_TESTED_CELLS_IN_VOXEL_LINE_INTERSECTION = false;

        public const bool REDUCED_RENDER_CELL_SIZE = false;

        public static bool ENABLE_BUILDER_MODE = false;

        public static float BOT_MISSILE_FIRING_RANGE_MIN = 200;

        public static bool USE_LONG_SOUND_DISTANCE = false;

        public const float LONG_SOUND_DISTANCE = 20000.0f;

        public const bool CULL_EVERY_RENDER_CELL = false;
        public static bool USE_DOMINANT_NORMAL_OFFSET_FOR_MODELS = false;

        public static bool MULTIPLAYER_LONG_TIMEOUT = false; // for debugging to prevent disconnect
        
        public static CommonLIB.AppCode.ObjectBuilders.MyGameplayDifficultyEnum DIFFICULTY_FOR_F12_MISSIONS = CommonLIB.AppCode.ObjectBuilders.MyGameplayDifficultyEnum.NORMAL;

        public static bool ENABLE_VISIBLE_SPAWNPOINT_DEACTIVATION = false;

        public static bool ENABLE_RANDOM_STATIONS_IN_SOLAR_SYSTEM = false;

        public static bool ENABLE_REFILL_PLAYER_TO_MAX = false;

        public static bool MULTIPLAYER_DISABLED = false;

        public static bool DRAW_CROSSHAIR_HORIZONTAL_LINE = false;

        public static bool ENABLE_REFILL_PLAYER_IN_MOTHERSHIP = false;

        public static MyMwcObjectBuilder_FactionEnum? SHOUTS_PREFERED_FACTION = null;//MyMwcObjectBuilder_FactionEnum.None; // null to disable

        public static bool ENABLE_SHOUT = true;

        public static bool ENABLE_DISPLAYING_ORE_ON_HUD = true && !MyFakes.MWBUILDER;

        //public static bool MW25D = true;
        public static bool MW25DCorrectVoxelPosition = false;

        public static bool ENABLE_MULTIPLAYER_RELAY = true;
        public static bool MULTIPLAYER_RELAY_TEST = false;
        public static bool MULTIPLAYER_SIMULATE_LAGS = false;

        public static bool MULTIPLAYER_CHEATS_ENABLED = false;
        public static bool ENABLE_DEBUG_DIALOGS = false;

        public static bool SET_ACTOR_PROGRESS = true;

        public static bool BOT_USE_FLASH_BOMBS = true;
        public static bool BOT_USE_SMOKE_BOMBS = true;
        public static bool BOT_USE_HOLOGRAMS = false;

        public const bool MWBUILDER = false;
        public static readonly MyMwcVector3Int MWBUILDER_SECTOR = new MyMwcVector3Int(18,18,18);
        public const bool MWCURIOSITY = false;

        public static Vector3? GRAVITATION = null;

        public static bool DUMP_MISSING_OBJECTS = true;
        public static bool SHOW_MISSING_OBJECTS = true;

        public static bool ENABLE_DEBUG_INFLUENCE_SPHERES_SOUNDS = false;

        public static bool ENABLE_ENTITY_ID_CHANGE = false;

        public const bool ENABLE_WARNING_EXPLANATION = false;

        public static bool ENABLE_LOGOS = true;

        public static bool ENABLE_INTRO = true;

        /// <summary>
        /// Resets steam stats when game is started
        /// MUST BE FALSE in PUBLIC build.
        /// </summary>
        public static bool RESET_STEAM_STATS = false;

        // Models used in new scene arent unloaded
        public static bool UNLOAD_OPTIMIZATION_KEEP_USED_MODELS = true;

        // When true, static asteroid models are not unloaded
        public static bool UNLOAD_OPTIMIZATION_KEEP_STATIC_ASTEROIDS = false;

        // When true, weapons, debris, cargo boxes and projectiles are not unloaded
        public static bool UNLOAD_OPTIMIZATION_KEEP_COMMON_MODELS = false;

        //Fixed by default, unfixed for profiling
        public static bool FIXED_TIMESTEP = true;

        //Defines how much content will be removed from voxel in explosion
        //0.0f = nothing removed, 1.0f = everything removed (default)
        public static float VOXELS_REMOVE_RATIO = 1.0f;

        public static bool ENABLE_GLOBAL_EVENTS = true;

        public static bool CUBE_EDITOR = false;
    }
}
