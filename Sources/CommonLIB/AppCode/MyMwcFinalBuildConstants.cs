using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtils.Utils;
using System.Net;
using MinerWars.CommonLIB.AppCode.Utils;

namespace SysUtils
{
    //  IMPORTANT: These are constants that must be checked before every official FINAL BUILD!
    //  They must be set to proper values, some must be increased. See comments for each one.
    public class MyMwcFinalBuildConstants
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  Add sector servers to MyMwcServers constructor. It should contain master server and list of all sector servers
        //
        //  Check project properties for MinerWarsCommonLIB -> conditional symbol ENABLE_NETWORK_VERBOSE_LOGGING must un-defined in final build
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  This is version of application or game used by PLAYER / SS / MS. Must be same on all running servers and clients!
        //  FINAL BUILD VALUE: For production must be PUBLIC, for test build must be TEST
        //  THIS  has been made private, use IsDevelop, IsTest or IsPublic instead (there are multiple public/test builds, so that's the reason)
        //  DONT DELETE IFDEFs, IT'S FOR AUTOMATIC BUILD SCRIPTS!!!
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if PUBLIC
        private const MyMwcFinalBuildType TYPE = MyMwcFinalBuildType.PUBLIC;
#elif TEST
        private const MyMwcFinalBuildType TYPE = MyMwcFinalBuildType.TEST;
#elif DEVELOP
        private const MyMwcFinalBuildType TYPE = MyMwcFinalBuildType.DEVELOP;
#else
        private const MyMwcFinalBuildType TYPE = MyMwcFinalBuildType.PUBLIC;
#endif

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  This is version of application
        //  FINAL BUILD VALUE: Increase before every major build.
        public const int APP_VERSION = 01109000;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  This is version of server network protocol, must be same on client and all servers!
        //  When changing this value set it to same value as APP_VERSION (increase APP_VERSION when necessary)
        //  FINAL BUILD VALUE: Increase ONLY when something changes in communication with server, e.g. object builder, server method.
        public const int SERVER_PROTOCOL_VERSION = 01092000;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Address of master server (e.g. masterserver.minerwars.com)
        //  FINAL BUILD VALUE: masterserver.minerwars.com
        public static string MASTER_SERVER_ADDRESS = GetValueForBuildType("masterserver.minerwars.com", "masterserver.test.minerwars.com", MyMwcSecrets.MASTER_SERVER_ADDRESS_DEVELOP);
        // LOCAL TEST // public static string MASTER_SERVER_ADDRESS = "masterserver.develop.minerwars.com";
        //public static string MASTER_SERVER_ADDRESS = "127.0.0.1";
        //public static string MASTER_SERVER_ADDRESS = "192.168.1.2";
        //public static string MASTER_SERVER_ADDRESS = "192.168.1.31";

        // When this is specified, it overrides url received from master server, set to null to use address received from master server
        public static string SECTOR_SERVER_ADDRESS = null; // Null means received from master server
        // LOCAL TEST 
        //public static string SECTOR_SERVER_ADDRESS = MASTER_SERVER_ADDRESS + ":" + GetValueForBuildType("4505", "15000", "25000"); // This means same as Master server with proper sector server port

        // Address of multiplayer host, when null, uses master server
        public static string MULTIPLAYER_HOST_ADDRESS = null; // Same as master server, always
        //public static string MULTIPLAYER_HOST_ADDRESS = "127.0.0.1";
        //public static string MULTIPLAYER_HOST_ADDRESS = GetValueForBuildType(null, null, "masterserver.minerwars.com"); // On ignum for develop

        // Set to true when building game for steam
        public const bool STEAM_BUILD = true;

        // Set to true when building steam demo
        public const bool STEAM_DEMO = false;

        // For OnLive, CiiNOW and other cloud gaming services (disabled HW cursor, editor, multiplayer)
        public const bool IS_CLOUD_GAMING = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If not null, then loading screen and first main menu screen is skiped and we go directly to 
        //  the game (story, editor, ...). Use it during programming for saving your time.
        //  FINAL BUILD VALUE: null
        public static readonly MyMwcQuickLaunchType? QUICK_LAUNCH_VALUE = null;
        public static readonly MyMwcQuickLaunchType? QUICK_LAUNCH = (IS_DEVELOP ? QUICK_LAUNCH_VALUE : null as MyMwcQuickLaunchType?);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  FINAL BUILD VALUE: Increase before every major build.
        public const bool USE_P2P = true;

        public static readonly StringBuilder APP_VERSION_STRING = new StringBuilder(MyMwcBuildNumbers.ConvertBuildNumberFromIntToString(MyMwcFinalBuildConstants.APP_VERSION));
        public static readonly StringBuilder SERVER_PROTOCOL_VERSION_STRING = new StringBuilder(MyMwcBuildNumbers.ConvertBuildNumberFromIntToString(MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION));
        public static readonly string TYPE_AS_STRING = GetValueForBuildType("PUBLIC", "TEST", "DEVELOP");

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Simulation of lost messages sent out (not sent, not delivered). If null, simulation is off. If filled, it defines probability of lost messages.
        //  E.g. number 10 means every tenth sent message is lost.
        //  FINAL BUILD VALUE: null
        public static int? SIMULATE_LOST_MESSAGES_SENT_OUT = null;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If true, then debug draw rendering is enabled
        //  FINAL BUILD VALUE: false
        public static bool EnableDebugDraw = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If true then every object draws its normal vectors as wireframe lines. Voxel's don't draw yet.
        //  FINAL BUILD VALUE: false
        public static bool DrawNormalVectors = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If true then every JLX object draws its collision primitives as wireframe object (sphere, box, capsule, static mesh). Voxel's don't draw.
        //  FINAL BUILD VALUE: false
        public static bool DrawJLXCollisionPrimitives = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  For extended logging in main update and draw methods
        //  FINAL BUILD VALUE: false
        public static bool EnableLoggingInDrawAndUpdateAndGuiLoops = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  When true, every update will contain few miliseconds of delay - use only for testing/debugging
        //  FINAL BUILD VALUE: false
        public static bool SimulateSlowUpdate = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  When true, every draw will contain few miliseconds of delay - use only for testing/debugging
        //  FINAL BUILD VALUE: false
        public static bool SimulateSlowDraw = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Draws Vocel content as billboards.
        //  FINAL BUILD VALUE: false
        public static bool DrawVoxelContentAsBillboards = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Drawing Helper Primitives
        //  FINAL BUILD VALUE: false
        public static bool DrawHelperPrimitives = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Disables/Enables enemy bots
        //  FINAL BUILD VALUE: false
        public static bool DisableEnemyBots = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Disables/Enables drawing of spots, where collisions occur in HUD
        //  FINAL BUILD VALUE: false
        public static bool DrawCollisionSpotsInHud = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Disables/Enables Fast Aproximate Antialiasing
        //  FINAL BUILD VALUE: true
        public static bool EnableFxaa = true;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  From where will the launcher download updates
        //  FINAL BUILD VALUE: "http://update2.minerwars.com/"
        //public static readonly string UPDATER_URL = "http://192.168.24.100/UpdateMinerWarsCom/";
        public static readonly string UPDATER_URL = GetValueForBuildType("http://update2.minerwars.com/", "http://update2.test.minerwars.com/", MyMwcSecrets.UPDATER_URL_DEVELOP);

        //  For upload to S3
        public static readonly string UPDATE_BUCKET_NAME = GetValueForBuildType("/update2.minerwars.com/", "/update2.test.minerwars.com/", MyMwcSecrets.UPDATE_BUCKET_NAME_DEVELOP);

        //  For upload to S3 Download mirror
        public static readonly string DOWNLOAD_BUCKET_NAME = GetValueForBuildType("/mirror1.minerwars.com/Downloads/Public/", "/mirror1.minerwars.com/Downloads/Test/", MyMwcSecrets.DOWNLOAD_BUCKET_NAME_DEVELOP);

        public static readonly string P2P_CONTROLlER_ADDRESS = "p2p.keenswh.com";
        public static readonly int P2P_CONTROLLER_PORT = GetValueForBuildType(11989, 11987, 11988);

        public static readonly string ANNOUNCE_URL = GetValueForBuildType("http://p2p.keenswh.com:6969/announce", "http://p2p.keenswh.com:6967/announce", MyMwcSecrets.ANNOUNCE_URL_DEVELOP);
        public static readonly string TRACKER_URL = "http://p2p.keenswh.com";
        public static readonly int TRACKER_PORT = GetValueForBuildType(6969, 6967, MyMwcSecrets.TRACKER_PORT_DEVELOP);

        public static readonly string UPDATER_CDN_MIRROR_URL = GetValueForBuildType("http://cdn77.update2.minerwars.com", "http://cdn77.update2.test.minerwars.com", MyMwcSecrets.UPDATER_CDN_MIRROR_URL_DEVELOP);

        //  Url of MinerWarsSetup.torrent file
        public static readonly string SETUPTORRENT_URL = GetValueForBuildType("http://mirror1.minerwars.com/Downloads/Public/", "http://mirror1.minerwars.com/Downloads/Test/", MyMwcSecrets.SETUPTORRENT_URL_DEVELOP);
        public static readonly string SETUPTORRENTMIRROR_URL = GetValueForBuildType("http://mirror2.update2.minerwars.com/", "http://mirror2.update2.test.minerwars.com/", MyMwcSecrets.SETUPTORRENTMIRROR_URL_DEVELOP);
        public static readonly string SETUPTORRENT_BUCKET_NAME = GetValueForBuildType("/mirror1.minerwars.com/Downloads/Public/", "/mirror1.minerwars.com/Downloads/Test/", MyMwcSecrets.SETUPTORRENT_BUCKET_NAME_DEVELOP);

        public static readonly int[] UNSUPPORTED_GPU_DEVICE_IDS = new int[] 
        { 
            0x791E, // Radeon X1200, only DX 9.0b, SM 2.0
            0x791F, // Mobility radeon X1100, only DX 9.0b, SM 2.0
        };

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If we log garbage collection. It's usefull only for debuging, not in final build.
        //  FINAL BUILD VALUE: false
        public static bool EnableLoggingGarbageCollectionCalls = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If not null than it specified how many millisecond will server wait after receiving message from player before processing it
        //  FINAL BUILD VALUE: null
        public static readonly int? SIMULATED_DELAY_ON_SERVER_IN_MILLISECONDS = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Some socket exception are better when ignored, because they can happen when hacker/cheater sends incorrect packet or something
        //  else I can't predict. So normaly we ignore them. But during testing/debugging, it's better if they are unhandled, thus catched by debugger.
        //  IMPORTANT: It's called "ignore socket exceptions" but actually it ignores other exception types too. Basically/
        //  every exception that originates from sending or receiving networking data.            
        //  FINAL BUILD VALUE: true
        public const bool IGNORE_SOCKET_EXCEPTIONS = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  For saving phys-object positions and gunshots when creating trailer - it will create the tracking file at application end in user's folder
        //  FINAL BUILD VALUE: false
        public const bool ENABLE_TRAILER_SAVE = false;        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If true, the game screen that normaly runs story/MMO will also run animation from trailer.xmlx
        //  FINAL BUILD VALUE: false
        public const bool ENABLE_TRAILER_ANIMATION_IN_GAMEPLAY_SCREEN = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If true then PERFHUD (NVIDIA's debugging tool) can access this application. Final build of game should have it set to FALSE!!!
        //  FINAL BUILD VALUE: false
        public const bool ENABLE_PERFHUD = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If true then collision particles and cues/sounds are generated when two objects collide
        //  FINAL BUILD VALUE: true
        public const bool ENABLE_COLLISION_CUES_AND_PARTICLES = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If true, application accepts changes through developer keys so extended information or control is allowed
        //  FINAL BUILD VALUE: false
        //public const bool ENABLE_DEVELOPER_KEYS = IS_DEVELOP;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  If false, objects are not highlighted in editor (useful when debugging visual appearance)
        //  FINAL BUILD VALUE: true
        public const bool ENABLE_OBJECT_HIGHLIGHT = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  We encrypt/decrypt all packets between player and player, or player and master server, or player and sector server using this password
        //  For master server to sector server packets we use PRIVATE_CRYPTO
        //  FINAL BUILD VALUE: Randomly change values, but preserve number of elements.
        public static readonly MyMwcSingleCrypto PUBLIC_CRYPTO = new MyMwcSingleCrypto(new byte[] { 244, 169, 21, 76, 179, 201, 34, 187 });

        /// <summary>
        /// Enable bot debug mode (like showing lines to their targets)
        /// </summary>
        public static bool BOT_DEBUG_MODE = false;

        /// <summary>
        /// Enable debug draw of vertex normals, if false then vertex buffer is destroyed after vertex buffer has been created
        /// </summary>
        public static bool ENABLE_VERTEX_NORMALS_DEBUG_DRAW = false;

        /// <summary>
        /// Sets events to log, default value is all
        /// </summary>
        public static MyMwcLogEventEnum LogType = MyMwcLogEventEnum.All;

        /// <summary>
        /// The version of the voxel cache file format. Increase everytime you change the format of the voxel cache.
        /// </summary>
        public const int VOXEL_CACHE_FILE_VERSION = 0002;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public const bool IS_DEVELOP = TYPE == MyMwcFinalBuildType.DEVELOP;
        public const bool IS_TEST = TYPE == MyMwcFinalBuildType.TEST;
        public const bool IS_PUBLIC = TYPE == MyMwcFinalBuildType.PUBLIC;
        public const bool REQUIRES_LAUNCHER = !IS_DEVELOP && !IS_CLOUD_GAMING;

        public static T GetValueForBuildType<T>(T ifPublic, T ifTest, T ifDevelop)
        {
            if (TYPE == MyMwcFinalBuildType.PUBLIC)
            {                
                return ifPublic;
            }
            else if (TYPE == MyMwcFinalBuildType.TEST)
            {
                return ifTest;
            }
            else if (TYPE == MyMwcFinalBuildType.DEVELOP)
            {
                return ifDevelop;
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        public const string OBFUSCATION_EXCEPTION_FEATURE = "cw symbol renaming";

        public const LoggingOptions LOGGING_OPTIONS = LoggingOptions.NONE;
    }
}


