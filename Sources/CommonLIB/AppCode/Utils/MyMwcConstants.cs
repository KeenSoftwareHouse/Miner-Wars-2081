using System;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using SysUtils;

namespace MinerWars.CommonLIB.AppCode.Utils
{
    public static class MyMwcMathConstants
    {
        //  IMPORTANT: If you change these constants, don't forget to change them also in MyMeshPartSolver
        public const float EPSILON = 0.00001f;
        public const float EPSILON_SQUARED = EPSILON * EPSILON;
    }

    public static class MyMwcValidationConstants
    {
        public const int USERNAME_LENGTH_MIN = 3;
        public const int USERNAME_LENGTH_MAX = 15;
        public const int EMAIL_LENGTH_MAX = 50;
        public const int PASSWORD_LENGTH_MIN = 6;
        public const int PASSWORD_LENGTH_MAX = 10;
        public const int POSITION_X_MAX = 5;
        public const int POSITION_Y_MAX = 5;
        public const int POSITION_Z_MAX = 5;
    }

    public static class MyMwcSectorConstants
    {
        //  Sector size
        public const float SECTOR_SIZE = 50000.0f;          //  Size of sector cube
        public const float SECTOR_SIZE_HALF = SECTOR_SIZE / 2.0f;
        public static readonly Vector3 SECTOR_SIZE_VECTOR3 = new Vector3(SECTOR_SIZE, SECTOR_SIZE, SECTOR_SIZE);
        public static readonly float SECTOR_DIAMETER = SECTOR_SIZE_VECTOR3.Length();
        public const float SAFE_SECTOR_SIZE = SECTOR_SIZE + 200;                //  Safe area around sector for detecting if we need to switch sectors
        public const float SECTOR_SIZE_FOR_PHYS_OBJECTS_SIZE_HALF = (SECTOR_SIZE * 0.9f) / 2.0f;
        public const float SAFE_SECTOR_SIZE_HALF = SAFE_SECTOR_SIZE / 2.0f;
        public static readonly BoundingBox SAFE_SECTOR_SIZE_BOUNDING_BOX = new BoundingBox(
            new Vector3(-SAFE_SECTOR_SIZE_HALF, -SAFE_SECTOR_SIZE_HALF, -SAFE_SECTOR_SIZE_HALF),
            new Vector3(SAFE_SECTOR_SIZE_HALF, SAFE_SECTOR_SIZE_HALF, SAFE_SECTOR_SIZE_HALF));
        public static readonly Vector3[] SAFE_SECTOR_SIZE_BOUNDING_BOX_CORNERS = SAFE_SECTOR_SIZE_BOUNDING_BOX.GetCorners();
        public static readonly BoundingBox SECTOR_SIZE_FOR_PHYS_OBJECTS_BOUNDING_BOX = new BoundingBox(
            new Vector3(-SECTOR_SIZE_FOR_PHYS_OBJECTS_SIZE_HALF, -SECTOR_SIZE_FOR_PHYS_OBJECTS_SIZE_HALF, -SECTOR_SIZE_FOR_PHYS_OBJECTS_SIZE_HALF),
            new Vector3(SECTOR_SIZE_FOR_PHYS_OBJECTS_SIZE_HALF, SECTOR_SIZE_FOR_PHYS_OBJECTS_SIZE_HALF, SECTOR_SIZE_FOR_PHYS_OBJECTS_SIZE_HALF));
        public static readonly BoundingBox SECTOR_SIZE_BOUNDING_BOX = new BoundingBox(
            new Vector3(-SECTOR_SIZE_HALF, -SECTOR_SIZE_HALF, -SECTOR_SIZE_HALF),
            new Vector3(SECTOR_SIZE_HALF, SECTOR_SIZE_HALF, SECTOR_SIZE_HALF));
    }

    public static class MyMwcUpdaterConstants
    {
        //  Public key for updater signatures. DON'T CHANGE IT!!!
        public static readonly string UPDATER_PUBLIC_KEY = "<DSAKeyValue><P>yl9hNuOfiKCg1B89VG6fCRRwp1PQXi52QL1M/3JiwWT6RCkVskY9GcaEkzWnyBxxV8V+9pCVF0F4HcFwLYisYcd6znUZW9GpMlVIX4HtOhFoginBOZ0tZeQR0MtiuV0Fo8LmznduBc/IFpILlcPrzoXi1ZCKjIBlCwJqoZWSIxU=</P><Q>166OIbJmVcAJj9V2sUqXQwtJeRk=</Q><G>Ze03E3/52qhVU/IRpW2LCF+SJ46x22b/thkYUvHoGuJvjVtKR5LgtTBjyMUzoeLcQwzP5o9aAuqT4NetVGYzvTT3tpKqDoWukscdD4vXpNo6wpxdk2Y8a8U6JF22lIgDuq8syBDK395bXejHCxZRjAeJOLG8nO2Cc7z++7Wq+Wg=</G><Y>YiLXo5PJ3b/s0jmeXNChH0OK5A0h0vnKNLQbA7AZ8VTv6esxg8isp+gmSmVfCuIjNTDwVtHaFdFZiiqZnEiBrqQL4SnLgO2AYfZnXVckZmQjVyVehRm5muPwNbw+iUOn6WAmOwkUhvRHgit/Lo1lNM7dou6HFWfZM4TxQ6UB2y4=</Y><J>8DPo3bXLcPfnkO7bIFyqhHw+qpYB0F2oGbiih5VT9oGDPDBnWhrS0QOJglIT7GMn+JODCn5N/ipZmLp13B7aM6mDJs2Nb+AFJ4C6wPyrya9riihU2H/NXSFMPalLdEHDeD7JDDZ7e/hdQRo0</J><Seed>agHDiNx5j7fa+xQcHhyXvrefm7c=</Seed><PgenCounter>ATc=</PgenCounter></DSAKeyValue>";

        public static readonly string COMMAND_LINE_ARGUMENT_UPDATER = "-updater";
        public static readonly string EXECUTABLE_NAME = "MinerWarsLauncher";
        public static readonly string P2P_TORRENT_NAME = "MinerWars.torrent";
        public static readonly string P2P_DLL_NAME = "KeenClientDLL2.CLR.dll";

        public static readonly string USER_AGENT = "Miner Wars";

        public static readonly string HASH_ALGORITHM = "sha1";
        public static readonly Encoding FILE_ENCODING = Encoding.ASCII;
        public static readonly string FILES_TXT = "Files.txt";
        public static readonly string CONFIG_XML = "MyMinerWarsLauncherConfig.xml";
        public static readonly string FILES_COLUMN_SEPARATOR = @"""";
        public const int FILES_COLUMNS_COUNT = 7;
        public static readonly string VERSION_TXT = "Version.txt";
        public const int VERSION_TXT_DOWNLOAD_TIMEOUT_IN_MILLISECONDS = 10000;
        public const int FILES_TXT_DOWNLOAD_TIMEOUT_IN_MILLISECONDS = 10000;
        public const int CONFIG_XML_DOWNLOAD_TIMEOUT_IN_MILLISECONDS = 10000;
        public static readonly string EXTENSION_EXE = ".exe";
        public static readonly string EXTENSION_DEPLOY = ".deploy";
        public static readonly string EXTENSION_HASH = ".hash";

        public static readonly string[] separatorCRLF = { MyStringUtils.C_CRLF };
        public static readonly string[] separatorColumns = { MyMwcUpdaterConstants.FILES_COLUMN_SEPARATOR };
        public const int TORRENT_CLIENT_LISTENER_PORT = 11555;
    }

    public static class MyMwcNetworkingConstants
    {
        // Connection is dropped when no messages are sent or received for this time period
        public readonly static TimeSpan WCF_TIMEOUT_INACTIVITY = MyMwcFinalBuildConstants.GetValueForBuildType(TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(15));
        public readonly static TimeSpan WCF_TIMEOUT_SEND = MyMwcFinalBuildConstants.GetValueForBuildType(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));

        public readonly static TimeSpan WCF_TIMEOUT_CLIENT_SECTOR_SERVER = TimeSpan.FromMinutes(20);

        // Timeout When user unexpectedly disconnects - keep alive is half of timeout
        public readonly static TimeSpan WCF_TIMEOUT_DISCONNECT = MyMwcFinalBuildConstants.GetValueForBuildType(TimeSpan.FromSeconds(90), TimeSpan.FromSeconds(90), TimeSpan.FromSeconds(20));
        public readonly static TimeSpan WCF_TIMEOUT_OPEN = TimeSpan.FromSeconds(20);
        public readonly static TimeSpan WCF_TIMEOUT_CLOSE = TimeSpan.FromSeconds(10);

        // Timeout for MasterServer-SectorServer connection (should be long enought)
        public readonly static TimeSpan WCF_TIMEOUT_USER_VALIDATION_SERVICE = MyMwcFinalBuildConstants.GetValueForBuildType(TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60));

        // UDP Master server port (not used now, will be used as Multiplayer host for Lobby and NAT punching)
        public static readonly int NETWORKING_PORT_MASTER_SERVER = MyMwcFinalBuildConstants.GetValueForBuildType(4112, 14112, 24112);

        // Ports for new master server 45xx
        public static readonly int NETWORKING_PORT_MASTER_SERVER_NEW = MyMwcFinalBuildConstants.GetValueForBuildType(4500, 14500, 24500);
        // Mex port - for metadata exchange (used only on develop build, it sends information about service to allow automatic class generation)
        public static readonly int NETWORKING_PORT_MASTER_SERVER_NEW_MEX = MyMwcSecrets.NETWORKING_PORT_MASTER_SERVER_NEW_MEX;
        // User validation service, sector server uses this services
        public static readonly int NETWORKING_PORT_MASTER_SERVER_USER_VALIDATION = MyMwcFinalBuildConstants.GetValueForBuildType(4510, 14510, 24510);
        // Mex port - for metadata exchange (used only on develop build, it sends information about service to allow automatic class generation)
        public static readonly int NETWORKING_PORT_MASTER_SERVER_USER_VALIDATION_MEX = MyMwcSecrets.NETWORKING_PORT_MASTER_SERVER_USER_VALIDATION_MEX;

        public static readonly int NETWORKING_PORT_MULTIPLAYER_PEER = 0;
        public static readonly string NETWORKING_MULTIPLAYER_ID = "MW_MP";

        // Sector server ports - 5xxx
        public static readonly int NETWORKING_PORT_SECTOR_SERVER_DEFAULT = MyMwcFinalBuildConstants.GetValueForBuildType(4505, 15000, 25000);
        // Mex port - for metadata exchange (used only on develop build, it sends information about service to allow automatic class generation)
        public static readonly int NETWORKING_PORT_SECTOR_SERVER_MEX = MyMwcFinalBuildConstants.GetValueForBuildType(5001, 15001, 25001);

        public static readonly string WCF_SECTOR_SERVER_CN = "SectorServer";
        public static readonly string WCF_SECTOR_SERVER_HASH = MyMwcSecrets.WCF_SECTOR_SERVER_HASH;
        public static readonly string WCF_MASTER_SERVER_CN = "MasterServer";
        public static readonly string WCF_MASTER_SERVER_HASH = MyMwcSecrets.WCF_MASTER_SERVER_HASH;

        public static readonly string WCF_MS_PUBLIC_USERNAME = MyMwcSecrets.WCF_MS_PUBLIC_USERNAME;
        public static readonly string WCF_MS_PUBLIC_PASSWORD = MyMwcSecrets.WCF_MS_PUBLIC_PASSWORD;

        //  Simple networking - Port used by simple master server (check client version, etc)
        public static readonly int NETWORKING_PORT_SIMPLE_SERVER = MyMwcFinalBuildConstants.GetValueForBuildType(7445, 17445, 27445);
        
        public static int MAX_WCF_MESSAGE_SIZE = 5 * 1024 * 1024;
        public static int MAX_WCF_MESSAGE_POOL_SIZE = MAX_WCF_MESSAGE_SIZE * 2;
        public const int MAX_WCF_CONNECTIONS = 4;

        public static int MAX_WCF_MESSAGE_SIZE_SERVER = 16 * 1024 * 1024;
        public static int MAX_WCF_MESSAGE_POOL_SIZE_SERVER = MAX_WCF_MESSAGE_SIZE_SERVER * 4;

        public static int MAX_STRING_LENGTH = 512;
    }
}
