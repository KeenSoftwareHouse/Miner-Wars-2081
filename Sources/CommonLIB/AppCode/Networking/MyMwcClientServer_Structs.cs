using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System.Runtime.Serialization;
using System.Reflection;
using SysUtils;
using System;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public static partial class MyMwcClientServer
    {
        internal static Nullable<T> GetEnumFromNumber<T, U>(U val, EndPoint senderEndPoint) where T : struct
        {
            Nullable<T> ret = MyMwcUtils.GetEnumFromNumber<T, U>(val);
            if (ret == null)
            {
                //  Return null so caller will ignore this packet!
                MyMwcCheaterAlert.AddAlert(MyMwcCheaterAlertType.UNKNOWN_ENUM_VALUE, senderEndPoint, "val: " + val.ToString());
                return null;
            }
            else
            {
                return ret.Value;
            }
        }

        // Obtain sector type based on the session type parameter
        public static MyMwcSectorTypeEnum GetSectorTypeFromSessionType(MyMwcStartSessionRequestTypeEnum sessionType)
        {
            MyMwcSectorTypeEnum? sectorType = null;
            if ((sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_SANDBOX) ||
                (sessionType == MyMwcStartSessionRequestTypeEnum.SANDBOX_FRIENDS) ||
                (sessionType == MyMwcStartSessionRequestTypeEnum.SANDBOX_OWN) ||
                (sessionType == MyMwcStartSessionRequestTypeEnum.SANDBOX_RANDOM))
            {
                sectorType = MyMwcSectorTypeEnum.SANDBOX;
            }
            else if (
                (sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_STORY) ||
                (sessionType == MyMwcStartSessionRequestTypeEnum.NEW_STORY) ||
                (sessionType == MyMwcStartSessionRequestTypeEnum.LOAD_CHECKPOINT))
            {
                sectorType = MyMwcSectorTypeEnum.STORY;
            }
            else if (
                (sessionType == MyMwcStartSessionRequestTypeEnum.EDITOR_MMO) ||
                (sessionType == MyMwcStartSessionRequestTypeEnum.MMO))
            {
                sectorType = MyMwcSectorTypeEnum.MMO;
            }

            MyCommonDebugUtils.AssertDebug(sectorType.HasValue);

            return sectorType.Value;
        }
    }

    public enum MyMwcServerCommandRequestTypesEnum : byte
    {
        GET_INFO,
        CHECK_ALIVE,
        SERVER_SHUTDOWN_NOTIFICATION
    }
    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyMwcSessionStateEnum
    {
        NEW_GAME = 1,
        LOAD_CHECKPOINT = 2,
        JOIN_GAME = 3,
        EDITOR = 4,
    }

    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyMwcTravelTypeEnum
    {
        NEIGHBOUR = 1,
        SOLAR = 2,
    }

    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyMwcLeaveSectorReasonEnum : byte
    {
        LOGOUT,
        EXIT_TO_MAIN_MENU,
        // Travel handled in separate message
        TRAVEL,
        RELOAD,
        //TRAVEL_IN_SOLAR_MAP
    }

    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyMwcRegisterResponseResultEnum : byte
    {
        OK,
        USERNAME_FORMAT_INVALID,
        USERNAME_ALREADY_USED,
        PASSWORD_FORMAT_INVALID,
        EMAIL_FORMAT_INVALID,
        WRONG_CLIENT_VERSION,
        UNKNOWN_ERROR
    }

    [System.Obsolete("Use SessionStartTypeEnum & SessionTypeEnum instead")]
    public enum MyMwcStartSessionRequestTypeEnum : byte
    {
        NEW_STORY,                  //  Play new single-player story, from beginning (or from first chapter)
        LOAD_CHECKPOINT,            //  Play single-player story from last checkpoint or from any chapter checkpoint
        JOIN_FRIEND_STORY,          //  Join friend in his active single-player story
        MMO,                        //  Play MMO game, common for all players on the server
        SANDBOX_OWN,                //  Play your sandbox sectors
        SANDBOX_FRIENDS,            //  Play sandbox sectors of your friend
        JOIN_SANDBOX_FRIEND,        //  Join friend in his active sandbox game
        EDITOR_SANDBOX,             //  Edit your sandbox sectors
        EDITOR_STORY,               //  Edit story sectors
        EDITOR_MMO,                 //  Edit MMO sectors
        SANDBOX_RANDOM              //  Play random sandbox - without data from the server. Just local random sector.
    }

    [System.Obsolete("Use SessionStartTypeEnum & SessionTypeEnum instead")]
    public enum MyMwcStartSessionResponseTypeEnum : byte
    {
        OK                     //  OK - game started successfuly
        //NO_PERMISSION,          //  Player can't start this game because he doesn't have permissions (e.g. when demo user starts MMO or when regular user tries to launch MMO editor)
        //WRONG_PARAMETERS,       //  E.g. when CheckpointId was wrong
        //USER_NOT_LOGGED_IN      //  User isn't logged in but he send this START GAME message
    }

    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyMwcSelectSectorRequestTypeEnum : byte
    {
        RANDOM_FRIENDS,                 //request for loading random friend sectors from server 
        FIND_BY_PLAYER_NAME,             //request for loading friend sectors by entered player name
        FIND_BY_PLAYER_NAME_FULLTEXT,
        STORY,                          // request for loading story sectors
    }

    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyMwcLoginResponseResultEnum : byte
    {
        OK,
        WRONG_USERNAME_OR_PASSWORD,
        USER_ALREADY_LOGGED_IN,
        WRONG_CLIENT_VERSION,
        ACCESS_RESTRICTED,
        GENERAL_FAILURE,
        INVALID_TIME,
        USERNAME_OR_PASSWORD_EMPTY,
    }

    //  Must be struct because is used as key for dictionary and there only values are important not reference in memory
    [DataContract]
    public struct MyMwcSectorIdentifier
    {
        [DataMember(Name = "SectorType")]
        public MyMwcSectorTypeEnum SectorType;          //  Type of sector(mmo, story, sandbox)

        [DataMember(Name = "Position")]
        public MyMwcVector3Int Position;                //  Position of this sector within this sector group

        [DataMember(Name = "UserId")]
        public int? UserId;                             //  Id of the user who saved the sector

        [DataMember(Name = "SectorName")]
        public string SectorName;                       //  Name of sector

        public MyMwcSectorIdentifier(MyMwcSectorTypeEnum sectorType, int? userId, MyMwcVector3Int position, string sectorName)
        {
            SectorType = sectorType;
            UserId = userId;
            Position = position;
            SectorName = sectorName == null ? string.Empty : sectorName;
        }

        public bool CanBeCheckpointSaved()
        {
            return true;
        }

        public bool CanBeSaved()
        {
            // Story sectors can be saved only on zero plane
            return SectorType != MyMwcSectorTypeEnum.STORY || Position.Y == 0;
        }

        public override string ToString()
        {
            string sectorType;
            bool useUserId;
            switch (SectorType)
            {
                case MyMwcSectorTypeEnum.STORY:
                    sectorType = "STORY";
                    useUserId = false;
                    break;
                case MyMwcSectorTypeEnum.MMO:
                    sectorType = "MMO";
                    useUserId = false;
                    break;
                case MyMwcSectorTypeEnum.SANDBOX:
                    sectorType = "SANDBOX";
                    useUserId = true;
                    break;
                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            if (useUserId)
            {
                useUserId = UserId.HasValue;
            }

            return sectorType + (useUserId ? " - " + UserId.Value + " - " + Position.ToString() : " - " + Position.ToString());
        }

        public int CompareTo(MyMwcSectorIdentifier compareWith)
        {
            int xA = Position.X;
            int yA = Position.Y;
            int zA = Position.Z;

            int xB = compareWith.Position.X;
            int yB = compareWith.Position.Y;
            int zB = compareWith.Position.Z;

            if (xA > xB) return -1;
            if (xA < xB) return 1;
            if (xA == xB)
            {
                if (yA > yB) return -1;
                if (yA < yB) return 1;
                if (yA == yB)
                {
                    if (zA > zB) return -1;
                    if (zA < zB) return 1;
                    if (zA == zB) return 0;
                }
            }

            return 0;
        }

        public static bool Is25DSector(string sectorName)
        {
            return sectorName != null && (sectorName.ToLower().Contains("2.5d") || sectorName.ToLower().Contains("2,5d"));
        }
    }

    //  Must be struct because we don't want to provocate garbage collector when received in MW game-client 
    public struct MyMwcPositionAndOrientation
    {
        public Vector3 Position;        //  Position within sector
        public Vector3 Forward;         //  Forward vector (for orientation)
        public Vector3 Up;              //  Up vector (for orientation)

        public MyMwcPositionAndOrientation(Vector3 position, Vector3 forward, Vector3 up)
        {
            Position = position;
            Forward = forward;
            Up = up;
        }

        // Optimized version
        public MyMwcPositionAndOrientation(ref Matrix matrix)
        {
            Position = matrix.Translation;
            Forward = matrix.Forward;
            Up = matrix.Up;
        }

        public MyMwcPositionAndOrientation(Matrix matrix)
            : this(matrix.Translation, matrix.Forward, matrix.Up)
        {

        }

        public Matrix GetMatrix()
        {
            return Matrix.CreateWorld(Position, Forward, Up);
        }

        public override string ToString()
        {
            return Position.ToString() + "; " + Forward.ToString() + "; " + Up.ToString();
        }
    }

    [DataContract(Name = "MyMwcUserDetail")]
    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public struct MyMwcUserDetail
    {
        [DataMember(Name = "UserId")]
        public int UserId;

        [DataMember(Name = "DisplayName")]
        public string DisplayName;

        public MyMwcUserDetail(int userId, string displayName)
        {
            UserId = userId;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return UserId.ToString() + "; " + DisplayName.ToString();
        }
    }

    public enum MyMwcVoxelHandModeTypeEnum : byte
    {
        ADD = 0,
        SUBTRACT = 1,
        SOFTEN = 2,
        SET_MATERIAL = 3,
        WRINKLE = 4
    }

    //  IMPORTANT: Never delete or change numeric values from this enum. They are used in database too and we don't want inconsistency.
    public enum MyMwcVoxelMapMergeTypeEnum : byte
    {
        ADD = 0,                    //  Merged voxels are added to actual voxel map only if they have more content than current, so empty voxels are ignored and full are added
        INVERSE_AND_SUBTRACT = 1    //  Merged voxels are subtracted from actual voxel map, so where was full voxel and merged is empty, nothing happens. But if merged is full or mixed, empty/mixed voles is created.
    }
    
    //  IMPORTANT: Never delete or change numeric values from this enum. They are used in database too and we don't want inconsistency.
    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyMwcVoxelMaterialsEnum : byte
    {
        Indestructible_01 = 0,
        Indestructible_02 = 1,
        Treasure_01 = 2,
        Treasure_02 = 3,
        Iron_01 = 4,
        Iron_02 = 5,
        Helium3_01 = 6,
        Helium4_01 = 7,
        Stone_01 = 8,
        Stone_02 = 9,
        Stone_03 = 10,
        Stone_04 = 11,
        Stone_05 = 12,
        Stone_06 = 13,
        Stone_07 = 14,
        Stone_08 = 15,
        Indestructible_03 = 16,
        Stone_10 = 17,
        Indestructible_04 = 18,
        Indestructible_05_Craters_01 = 19,
        Stone_13_Wall_01 = 20,
        Ice_01 = 21,
        Gold_01 = 22,
        Silver_01 = 23,
        Organic_01 = 24,
        Nickel_01 = 25,
        Magnesium_01 = 26,
        Platinum_01 = 27,
        Uranite_01 = 28,
        Silicon_01 = 29,
        Cobalt_01 = 30,
        Snow_01 = 31,
        Lava_01 = 32,
        Concrete_01 = 33,
        Concrete_02 = 34,
        Sandstone_01 = 35,
        Stone_Red = 36
    }

    //  IMPORTANT: Never delete or change numeric values from this enum. They are used in database too and we don't want inconsistency.
    public enum MyMwcVoxelFilesEnum : short
    {
        PerfectSphereWithMassiveTunnels_512x512x512 = 0,
        TorusWithSmallTunnel_256x128x256 = 1,
        VerticalIsland_128x256x128 = 2,
        PerfectSphereWithMassiveTunnels2_512x512x512 = 3,
        PerfectSphereWithRaceTunnel2_512x512x512 = 4,
        TorusWithManyTunnels_256x128x256 = 5,
        PerfectSphereSplitted_512x512x512 = 6,
        PerfectSphereWithFewTunnels_512x512x512 = 7,
        PerfectSphereWithRaceTunnel_512x512x512 = 8,
        SphereWithLargeCutOut_128x128x128 = 9,
        VerticalIsland_128x128x128 = 10,
        TorusStorySector_256x128x256 = 11,
        VerticalIslandStorySector_128x256x128 = 12,
        DeformedSphere1_64x64x64 = 13,
        DeformedSphere2_64x64x64 = 14,
        DeformedSphereWithCorridor_128x64x64 = 15,
        DeformedSphereWithCorridor_256x256x256 = 16,
        DeformedSphereWithCraters_128x128x128 = 17,
        ScratchedBoulder_128x128x128 = 18,
        DeformedSphereWithHoles_64x128x64 = 19,
        TorusWithManyTunnels_2_256x128x256 = 20,
        VoxelImporterTest = 21,
        Fortress = 22,
        AsteroidWithSpaceStationStartStorySector = 23,

        Cube_128x128x128 = 24,
        Cube_128x128x256 = 25,
        Cube_128x128x64 = 26,
        Cube_128x256x128 = 27,
        Cube_128x256x256 = 28,
        Cube_128x256x64 = 29,
        Cube_128x64x128 = 30,
        Cube_128x64x256 = 31,
        Cube_128x64x64 = 32,
        Cube_256x128x128 = 33,
        Cube_256x128x256 = 34,
        Cube_256x128x512 = 35,
        Cube_256x256x128 = 36,
        Cube_256x256x256 = 37,
        Cube_256x256x512 = 38,
        Cube_256x512x128 = 39,
        Cube_256x512x256 = 40,
        Cube_256x512x512 = 41,
        Cube_512x256x256 = 42,
        Cube_512x256x512 = 43,
        Cube_512x512x256 = 44,
        Cube_512x512x512 = 45,
        Cube_64x128x128 = 46,
        Cube_64x128x64 = 47,
        Cube_64x64x128 = 48,
        Cube_64x64x64 = 49,

        Story02 = 50,
        Mission01_01 = 51,
        Mission01_02 = 52,
        Mission07_01 = 53,

        Mission01_asteroid_big = 54,
        Mission01_asteroid_mine = 55,

        SphereFull_64 = 56,
        SphereFull_128 = 57,
        SphereFull_256 = 58,
        SphereFull_512 = 59,

        EacPrisonAsteroid = 60,
        piratebase_export = 61,

        Chinese_Mines_CenterAsteroid = 62,
        Chinese_Mines_FrontAsteroid = 63,
        Chinese_Mines_FrontRightAsteroid = 64,
        Chinese_Mines_LeftAsteroid = 65,
        Chinese_Mines_MainAsteroid = 66,
        Chinese_Mines_RightAsteroid = 67,

        TowerWithConcreteBlock1 = 68,
        TowerWithConcreteBlock2 = 69,

        Warehouse = 70,

        Barths_moon_base = 71,
        Barths_moon_satelite = 72,
        Fort_valiant_base = 73,
        Fort_valiant_dungeon = 74,

        JunkYardInhabited_256x128x256 = 75,
        JunkYardToxic_128x128x128 = 76,

        Empty_512x512x512 = 77,

        JunkYardForge_256x256x256 = 78,

        Barths_moon_camp = 79,

        rift_base_bigger = 80,
        rift_base_smaller = 81,

        barths_moon_lab1 = 82,
        barths_moon_lab2 = 83,

        fort_val_box_128 = 84,

        rift = 85,
        rift_small_1 = 86,
        rift_small_2 = 87,

        Junkyard_Race_256x256x256 = 88,
        Junkyard_RaceAsteroid_256x256x256 = 89,

        ChineseRefinery_First_128x128x128 = 90,
        ChineseRefinery_Second_128x128x128 = 91,
        ChineseRefinery_Third_128x256x128 = 92,

        Chinese_Corridor_Last_126x126x126 = 93,
        Chinese_Corridor_Tunnel_256x256x256 = 94,

        Bioresearch = 95,
        
        small2_asteroids = 96,
        small3_asteroids = 97,
        many_medium_asteroids = 98,
        many_small_asteroids = 99,
        many2_small_asteroids = 100,
        rus_attack = 101,
        RussianWarehouse =102,
        MothershipFacility = 103,
        SlaverBase = 104,
        ResearchVessel =105,
        BilitaryBase =106,
        reef_ast = 107,

        hopebase512 = 108,
        hopefood128 = 109,
        hopevault128 = 110,

        New_Jerusalem_Asteroid = 111,
        Chinese_Transmitter_Asteroid = 112,
        Slaver_Base_2_Asteroid = 113,
        Small_Pirate_Base_2_Asteroid = 114,
        Small_Pirate_Base_Asteroid = 115,
        Solar_Factory_EAC_Asteroid = 116,
        Mines_Asteroid = 117,
        Small_Pirate_Base_3_1 = 118,
        Small_Pirate_Base_3_2 = 119,
        Small_Pirate_Base_3_3 = 120,
        Voxel_Arena_Tunnels = 121,
        EACSurveyBigger_512_256_256 = 122,
        EACSurvaySmaller_256_256_256 = 123,
        Laika1_128_128_128 = 124,
        Laika2_64_64_64 = 125,
        Laika3_64_64_64 = 126,
        Laika4_256_128_128 = 127,
        Laika5_128_128_128 = 128,
        Laika9_128_128_128 = 129,
        Laika6_64_64_64 = 130,
        Laika8_128_128_128 = 131,
        Laika7_64_64_64 = 132,
        Novaja_Zemlja = 133,

        Arabian_Border_1 = 134,
        Arabian_Border_2 = 135,
        Arabian_Border_3 = 136,
        Arabian_Border_4 = 137,
        Arabian_Border_5 = 138,
        Arabian_Border_6 = 139,
        Arabian_Border_7 = 140,
        Arabian_Border_Arabian = 141,
        Arabian_Border_EAC = 142,

        Chinese_Corridor_1 = 143,

        Grave_Skull = 144,

        Pirate_Base_1 = 145,
        Pirate_Base_2 = 146,
        Pirate_Base_3 = 147,

        Plutonium_Mines = 148,

        Fragile_Sector = 149,

        Chinese_Solar_Array_Bottom = 150,
        Chinese_Solar_Array_Main = 151,

        Chinese_Mines_Small = 152,
        Chinese_Mines_Side = 153,

        Hippie_Outpost_Base = 154,
        Hippie_Outpost_Tree = 155,

        Laika_1 = 156,
        Laika_2 = 157,
        Laika_3 = 158,

        Fortress_Sanc_1 = 159,
        Fortress_Sanc_2 = 160,
        Fortress_Sanc_3 = 161,
        Fortress_Sanc_4 = 162,
        Fortress_Sanc_5 = 163,
        Fortress_Sanc_6 = 164,
        Fortress_Sanc_7 = 165,
        Fortress_Sanc_8 = 166,
        Fortress_Sanc_9 = 167,
        Fortress_Sanc_10 = 168,
        Fortress_Sanc_11 = 169,

        Russian_Transmitter_1 = 170,
        Russian_Transmitter_2 = 171,
        Russian_Transmitter_3 = 172,

        Russian_Warehouse = 173,

        ReichStag_1 = 174,
        ReichStag_2 = 175,

        New_Singapore = 176,

        HallOfFame = 177,

        IceCaveDeathmatch = 178,

        WarehouseDeathmatch = 179,

        Nearby_Station_1 = 180,
        Nearby_Station_2 = 181,
        Nearby_Station_3 = 182,
        Nearby_Station_4 = 183,
        Nearby_Station_5 = 184,
        Nearby_Station_6 = 185,
        Nearby_Station_7 = 186,
        Nearby_Station_8 = 187,
        Nearby_Station_9 = 188,
        Nearby_Station_10 = 189,
        Nearby_Station_11 = 190,
        Nearby_Station_12 = 191,

        VoxelArenaDeathmatch = 192,
        
        RiftStationSmaller = 193,
     
        Flat_256x64x256 = 194,
        Flat_128x64x128 = 195,
        Flat_512x64x512 = 196,

        PirateBaseStaticAsteroid_A_1000m = 197,
        PirateBaseStaticAsteroid_A_5000m_1 = 198,
        PirateBaseStaticAsteroid_A_5000m_2 = 199,

        d25asteroid_field = 200,
        d25city_fight = 201,
        d25gates_ofhell = 202,
        d25junkyard = 203,
        d25military_area = 204,
        d25miner_outpost = 205,
        d25plain = 206,
        d25radioactive = 207,

        Russian_Transmitter_Main = 208,
        SphereFull_1024 = 209,
    }
}
