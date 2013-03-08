#region Using

using System.Collections.Generic;
using System.Linq;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using System;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Entities.Prefabs;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Sessions;

#endregion

namespace MinerWars.AppCode.Game.Missions
{
    #region Enums

    public enum MyMissionID
    {
        M01_Intro = 1000,
        M01_Intro_Mining = 1001,
        M01_Intro_Sell = 1002,
        M01_Intro_Buy = 1003,
        M01_Intro_Kill = 1004,

        NEW_STORY_M001 = 10000,
        NEW_STORY_M001_DEFEND = 10001,

        NEW_STORY_M002_ADDITIONAL_MISSION = 20000,
        NEW_STORY_M002_GET_FOUNDATION_FACTORY = 20001,
        NEW_STORY_M002_DEPLOY_FOUNDATION_FACTORY = 20002,
        NEW_STORY_M002_BUILD_PREFAB = 20003,

        NEW_STORY_M003 = 30000,
        NEW_STORY_M003_FIND_ENTRANCE = 30001,
        NEW_STORY_M003_FIND_AND_DESTROY_GENERATORS = 30002,
        NEW_STORY_M003_STEAL_CASH = 30003,

        PIRATE_BASE = 40000,
        PIRATE_BASE_TRAVEL_TO_BASE = 40001,
        PIRATE_BASE_DEFENSE_SETUP = 40002,
        PIRATE_BASE_DEFEND = 40003,
        PIRATE_BASE_RETURN_BACK_TO_MOTHERSHIP = 40004,

        PIRATE_BASE_SPEAK_WITH_PIRATES = 40005,
        PIRATE_BASE_LISTEN_TO_CAPTAIN = 40006,
        PIRATE_BASE_PREPARE_FOR_DEFENSE = 40007,
        PIRATE_BASE_GET_TURRETS = 40008,
        PIRATE_BASE_ALLY_ARRIVED = 40009,

        EAC_SURVEY_SITE = 700000,
        EAC_SURVEY_SITE_FOLLOWMARCUS_1 = 700005,
        EAC_SURVEY_SITE_FOLLOWMARCUS_2 = 700007,
        EAC_SURVEY_SITE_GOTO_10 = 700010,
        EAC_SURVEY_SITE_WAVES = 700015,
        EAC_SURVEY_SITE_CLEAR_THE_WAY = 700020,
        EAC_SURVEY_SITE_GOTO_30 = 700030,
        EAC_SURVEY_SITE_GOTO_40 = 700040,
        EAC_SURVEY_SITE_GENERATOR = 700050,
        EAC_SURVEY_SITE_GOTO_60 = 700060,
        EAC_SURVEY_SITE_GOTO_65 = 700065,
        EAC_SURVEY_SITE_SAVEMINERS = 700066,
        EAC_SURVEY_SITE_GOTO_70 = 700070,
        EAC_SURVEY_SITE_SURVIVE = 700080,
        EAC_SURVEY_SITE_GOTO_90 = 700090,
        EAC_SURVEY_SITE_TURRETS_RIGHT = 700100,
        EAC_SURVEY_SITE_TURRETS_LEFT = 700110,

        LAIKA = 800000,
        LAIKA_GOTO_10 = 800010,
        LAIKA_GOTO_11 = 800011,
        LAIKA_GOTO_12 = 800012,
        LAIKA_GOTO_GENERATOR = 800015,
        LAIKA_GOTO_COMMAND = 800020,
        LAIKA_GOTO_COMMUNICATION_01 = 800030,
        LAIKA_GOTO_COMMUNICATION_02 = 800040,
        LAIKA_WARHEAD = 800045,
        LAIKA_RETURN = 800050,
        LAIKA_LASTSTAND = 800060,
        LAIKA_AWAY = 800070,

        TWIN_TOWERS = 1000000,
        TWIN_TOWERS_INTRO = 1000003,
        TWIN_TOWERS_SABOTAGE = 1000010,
        TWIN_TOWERS_RANDEVOUZ = 1000020,
        TWIN_TOWERS_ASSAULT = 1000030,
        TWIN_TOWERS_HACKING = 1000033,
        TWIN_TOWERS_JAMMER = 1000036,
        TWIN_TOWERS_HACKING_CONTINUE = 1000038,
        TWIN_TOWERS_RAILGUN1 = 1000040,
        TWIN_TOWERS_RAILGUN2 = 1000050,
        TWIN_TOWERS_GOTO_RIGHT = 1000060,
        TWIN_TOWERS_GENERATOR = 1000070,
        TWIN_TOWERS_COMMAND = 1000080,
        TWIN_TOWERS_WAIT = 1000085,
        TWIN_TOWERS_MOTHERSHIP1 = 1000090,
        TWIN_TOWERS_MOTHERSHIP1_V2 = 1000093,
        TWIN_TOWERS_MOTHERSHIP2 = 1000095,
        TWIN_TOWERS_GOTO_MADELYN = 1000100,

        CHINESE_TRANSPORT = 900000,
        CHINESE_TRANSPORT_GET_SECURITY_KEY = 900001,
        CHINESE_TRANSPORT_REACH_TUNNEL_1 = 900002,
        CHINESE_TRANSPORT_REACH_TRANSMITTER = 900003,
        CHINESE_TRANSPORT_KILL_GUARDS = 900004,
        CHINESE_TRANSPORT_HACK_TRANSMITTER = 900005,
        CHINESE_TRANSPORT_PLACE_BOMB = 900007,
        CHINESE_TRANSPORT_RUN_EXPLOSION = 900008,
        CHINESE_TRANSPORT_LOOK_ON_EXPLOSION = 900009,
        CHINESE_TRANSPORT_REACH_TUNNEL_2 = 900010,
        CHINESE_TRANSPORT_PAST_TUNNEL_2 = 900011,
        CHINESE_TRANSPORT_REACH_HANGAR_HACK = 900012,
        CHINESE_TRANSPORT_OPEN_HANGAR_SERVICE_ROOM = 900013,
        CHINESE_TRANSPORT_HACK_HANGAR_SERVICE_PC = 900014,
        CHINESE_TRANSPORT_DEFEND_MARCUS = 900015,
        CHINESE_TRANSPORT_KILL_BOSS = 900016,
        CHINESE_TRANSPORT_LAND_IN = 900017,

        SIDE_MISSION_01_ASSASSINATION = 1000001,
        SIDE_MISSION_01_ASSASSINATION_KILL = 1000002,

        CHINESE_ESCAPE = 1100000,
        CHINESE_ESCAPE_GET_CLOSER = 1100001,
        CHINESE_ESCAPE_DEFEND_SHIP = 1100002,
        CHINESE_ESCAPE_SPEAK_WITH_MADELYN = 1100003,

        RUSSIAN_WAREHOUSE = 1500000,
        RUSSIAN_WAREHOUSE_SNEAKINMAINBASE = 1500010,
        RUSSIAN_WAREHOUSE_TURNOFFCAMS = 1500020,
        RUSSIAN_WAREHOUSE_CTRLTURRET = 1500030,
        RUSSIAN_WAREHOUSE_GETOUTMAINBASE = 1500040,
        RUSSIAN_WAREHOUSE_BREAKOLDENTRANCE = 1500050,
        RUSSIAN_WAREHOUSE_LOCATEWAREHOUSE = 1500060,
        RUSSIAN_WAREHOUSE_FINDTRANSMITTER = 1500070,
        RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART1 = 1500080,
        RUSSIAN_WAREHOUSE_GETOUTWAREHOUSE = 1500090,
        RUSSIAN_WAREHOUSE_CRUSHREMAINGSHIPS = 1500100,
        RUSSIAN_WAREHOUSE_MEETINGPOINT = 1500110,
        RUSSIAN_WAREHOUSE_OPEN_DOORS = 1500111,
        RUSSIAN_WAREHOUSE_DOWNLOAD_DATA = 1500112,
        RUSSIAN_WAREHOUSE_TURNOFFMAINDEF_PART2 = 15000113,
        RUSSIAN_WAREHOUSE_DOWNDATADIALOGUE = 1500114,

        EAC_PRISON = 1900000,
        EAC_PRISON_THRUSWARM = 1900005, //new one
        EAC_PRISON_SOLARDEF = 1900010,
        EAC_PRISON_SOLAROFF = 1900020, //should be splitted between SOLAROFF1 and SOLAROFF2
        EAC_PRISON_SOLAROFF1 = 1900022, //part 1 of exSOLAROFF
        EAC_PRISON_MOTHERSHIPHELP = 1900025, //new one
        EAC_PRISON_SOLAROFF2 = 1900027, //part 2 of exSOLAROFF
        EAC_PRISON_BREAKIN = 1900030,
        EAC_PRISON_LOCINTEL = 1900040,
        EAC_PRISON_ACQUIREIDCARD = 1900042, //new one
        EAC_PRISON_LOCINTEL2 = 1900043,
        EAC_PRISON_OPENACCESS = 1900045, //new one
        EAC_PRISON_SECURITYOFF = 1900050,
        EAC_PRISON_MARCUSCELL = 1900060,
        EAC_PRISON_MARCUSDIALOG = 1900065,
        EAC_PRISON_WE_HAVE_COMPANY = 1900066,
        EAC_PRISON_MEETMARCUS = 1900070, //can be deleted or commented out
        EAC_PRISON_COVERMARCUS = 1900080,
        EAC_PRISON_GETARMS = 1900090,
        EAC_PRISON_FIGHTOUT = 1900100,
        EAC_PRISON_CRUSHREINFORCEMENTS = 1900105, //new one
        EAC_PRISON_MEETINGPOINT = 1900110,

        LAST_HOPE = 2000000,
        LAST_HOPE_REACH_COLONY = 2000001,
        LAST_HOPE_DESTROY_SLAVER_RIDERS = 2000002,
        LAST_HOPE_CATCH_SLAVER_RIDERS = 20000021,
        LAST_HOPE_FLY_KILL_WAVES = 20000022,
        LAST_HOPE_STOP_SLAVER_RIDERS = 2000003,
        LAST_HOPE_REACH_UNDEGROUND_CAVES = 2000004,
        LAST_HOPE_SPEAK_WITH_FATHER = 20000041,
        LAST_HOPE_FIND_MAINTANCE_TUNELL = 20000042,
        LAST_HOPE_KILL_SQUAD = 2000005,
        LAST_HOPE_DEACTIVATE_BOMB = 2000006,
        LAST_HOPE_REPAIR_PIPES= 2000007,
        LAST_HOPE_STABILIZE_NUCLEAR_CORE = 2000008,
        LAST_HOPE_LEAVE_UNDERGROUND = 2000009,
        LAST_HOPE_GET_REVARD = 20000010,
        LAST_HOPE_RETURN_TO_MOTHER_SHIP = 20000012,

        CHINESE_TRANSMITTER = 2100000,
        CHINESE_TRANSMITTER_FIND_CIC = 2100001,
        CHINESE_TRANSMITTER_OPEN_CIC = 2100002,
        CHINESE_TRANSMITTER_PLACE_DEVICE = 2100003,
        CHINESE_TRANSMITTER_LOOT_CARGO = 2100004,
        CHINESE_TRANSMITTER_ESCAPE = 2100005,
        CHINESE_TRANSMITTER_FIND_SECURITY_CONTROL = 2100006,
        CHINESE_TRANSMITTER_DESTROY_SECURITY_CONTROL = 2100007,
        CHINESE_TRANSMITTER_ESCAPE2 = 2100008,
        CHINESE_TRANSMITTER_CHECKPOINT = 2100009,
        CHINESE_TRANSMITTER_DIALOGUE_1 = 2100010,
        CHINESE_TRANSMITTER_DIALOGUE_2 = 2100011,
        CHINESE_TRANSMITTER_DIALOGUE_3 = 2100012,
        CHINESE_TRANSMITTER_DIALOGUE_4 = 2100013,
        CHINESE_TRANSMITTER_DIALOGUE_5 = 2100014,
        CHINESE_TRANSMITTER_DIALOGUE_6 = 2100015,
        CHINESE_TRANSMITTER_DIALOGUE_7 = 2100016,
        CHINESE_TRANSMITTER_DIALOGUE_8 = 2100017,
        CHINESE_TRANSMITTER_DIALOGUE_9 = 2100018,
        CHINESE_TRANSMITTER_ESCAPE3 = 2100019,
     
        NAZI_BIO_LAB = 2200000,
        NAZI_BIO_LAB_GETMISSION = 220005,
        NAZI_BIO_LAB_SAMPLES_BIOMACH_1 = 2200010,
        NAZI_BIO_LAB_GET_INSIDE = 2200020,
        NAZI_BIO_LAB_SAMPLES_BIOMACH_2 = 2200030,
        NAZI_BIO_LAB_SAMPLES_ORGANIC = 2200040,
        NAZI_BIO_LAB_BLUEPRINTS_BIOMACH = 2200050,
        NAZI_BIO_LAB_GET_BIOMACH_PARTS = 2200060,
        NAZI_BIO_LAB_GET_OUT = 2200070,
        NAZI_BIO_LAB_DESTROY= 2200071,
        NAZI_BIO_LAB_REACH_MEETING_POINT = 2200080,

        // Play ground - small pirate base
        PIRATE_BASE_1 = 2300000,
        PIRATE_BASE_1_DESTROY_PIPES_1 = 2300001,
        PIRATE_BASE_1_DESTROY_GENERATOR = 2300002,
        PIRATE_BASE_1_ESCAPE = 2300003,

        SMALL_PIRATE_BASE_2 = 2800000,
        SMALL_PIRATE_BASE_2_DESTROY_MOTHERSHIP = 2800001,
        SMALL_PIRATE_BASE_2_KILL_GENERAL = 2800003,
        SMALL_PIRATE_BASE_2_DESTROY_GENERATOR = 2800002,
        SMALL_PIRATE_BASE_2_DESTROY_ENEMIES = 2800010,

        RIFT = 2500000,
        RIFT_INTRO = 2500003,
        RIFT_GOTO_GETSUPPLIES1 = 2500005,
        RIFT_GOTO_GETSUPPLIES2 = 2500006,
        RIFT_GOTO_GETSUPPLIES3 = 2500007,
        RIFT_GOTO_10 = 2500010,
        RIFT_URANITE = 2500020,
        RIFT_GOTO_30 = 2500030,

        JUNKYARD_CONVINCE = 2600000,
        JUNKYARD_CONVINCE_FIND_INFORMATOR = 2600001,
        JUNKYARD_CONVINCE_D_FIND_INFORMATOR = 2600002,
        JUNKYARD_CONVINCE_FIND_SMUGGLER = 2600003,
        JUNKYARD_CONVINCE_FIGHT_COMPANIONS = 2600004,
        JUNKYARD_CONVINCE_FOLLOW_SMUGGLER = 2600005,
        JUNKYARD_CONVINCE_D_FOLLOW_SMUGGLER = 2600006,
        JUNKYARD_CONVINCE_MEET_GANGMAN = 2600021,
        JUNKYARD_CONVINCE_FLY_TO_ENEMY = 2600007,
        JUNKYARD_CONVINCE_KILL_WAVES = 2600008,
        JUNKYARD_CONVINCE_SPEAK_WITH_MOMO = 2600009,
        JUNKYARD_CONVINCE_D_SPEAK_WITH_MOMO = 2600010,
        JUNKYARD_CONVINCE_FIGHT_MOMO = 2600011,
        JUNKYARD_CONVINCE_RETURN_TO_SMUGGLER = 2600012,
        JUNKYARD_CONVINCE_D_RETURN_TO_SMUGGLER = 2600013,
        JUNKYARD_CONVINCE_FIGHT_ENEMY_WAVES = 2600014,
        JUNKYARD_CONVINCE_GO_TO_BOMB_DEALER = 2600015,
        JUNKYARD_CONVINCE_D_GO_TO_BOMB_DEALER = 2600016,
        JUNKYARD_CONVINCE_GO_TO_MARCUS = 2600017,
        JUNKYARD_CONVINCE_D_GO_TO_MARCUS = 2600018,
        JUNKYARD_CONVINCE_BR_FIGHT = 2600019,
        JUNKYARD_CONVINCE_RETURN_TO_MS = 2600020,

        RUSSIAN_TRANSMITTER = 3200000,
        RUSSIAN_TRANSMITTER_REACH_SIDE_ENTRANCE = 3200001,
        RUSSIAN_TRANSMITTER_ENTER_THE_BASE = 3200002,
        RUSSIAN_TRANSMITTER_FIND_FREQUENCY = 3200003,
        RUSSIAN_TRANSMITTER_MEET_STRANGER = 3200004,
        RUSSIAN_TRANSMITTER_REACH_WAREHOUSE = 3200005,
        RUSSIAN_TRANSMITTER_STEAL_MILITARY_SUPPLY = 3200006,
        RUSSIAN_TRANSMITTER_TRADE_WITH_VOLODIA = 3200007,
        RUSSIAN_TRANSMITTER_FIGHT_RUSSIAN_COMMANDO = 3200008,
        RUSSIAN_TRANSMITTER_SURVIVE = 3200009,
        RUSSIAN_TRANSMITTER_BACK_TO_TRANSMITTER = 3200010,
        RUSSIAN_TRANSMITTER_DECRYPT_FREQUENCY = 320011,
        RUSSIAN_TRANSMITTER_UPLOAD_DATA = 3200012,
        RUSSIAN_TRANSMITTER_FIND_MAIN_ROOM_ENTRANCE = 3200013,
        RUSSIAN_TRANSMITTER_PLACE_DEVICE_ON_TRANSMITTER = 3200014,
        RUSSIAN_TRANSMITTER_OPEN_DOORS = 3200015,
        RUSSIAN_TRANSMITTER_BACK_TO_MADELYN = 3200016,
        RUSSIAN_TRANSMITTER_INTRO_DIALOGUE = 3200017,
        RUSSIAN_TRANSMITTER_STRANGER_CONTACT = 3200018,
        RUSSIAN_TRANSMITTER_HACKING_FAILED = 3200019,

        BARTHS_MOON_CONVINCE = 2900000,
        BARTHS_MOON_CONVINCE_MEET_THOMAS_BART = 2900001,
        BARTHS_MOON_CONVINCE_TALK_WITH_THOMAS_BART = 2900002,
        BARTHS_MOON_CONVINCE_RETURN_BACK_TO_MADELYN = 2900003,
        BARTHS_MOON_CONVINCE_FLY_TO_ENEMY_BASE = 2900004,
        BARTHS_MOON_CONVINCE_DESTROY_SHIP = 2900005,
        BARTHS_MOON_CONVINCE_DESTROY_GENERATOR = 2900006,
        BARTHS_MOON_CONVINCE_FIND_TRANSMITTER = 2900007,
        BARTHS_MOON_CONVINCE_FLY_BACK_TO_BARTH = 2900008,

        BARTHS_MOON_TRANSMITTER = 2910000,
        BARTHS_MOON_TRANSMITTER_MEET_BARTH = 2910001,
        BARTHS_MOON_TRANSMITTER_TALK_TO_BARTH = 2910002,
        BARTHS_MOON_TRANSMITTER_FIND_WAY_TO_MOON= 2910003,
        BARTHS_MOON_TRANSMITTER_DESTROY_LAB = 2910004,
        BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR= 2910005,
        BARTHS_MOON_TRANSMITTER_LOOK_HUBS = 2910006,
        BARTHS_MOON_TRANSMITTER_GET_ITEMS = 2910007,
        BARTHS_MOON_TRANSMITTER_FAN = 29100071,
        BARTHS_MOON_TRANSMITTER_DRONES = 29100072,
        BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE1 = 2910008,
        BARTHS_MOON_TRANSMITTER_ENABLE_GENERATOR_DRONE2 = 2910009,
        BARTHS_MOON_TRANSMITTER_LOOK_HUBS2 = 29100091,
        BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA1 = 2910010,
        BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA2 = 2910011,
        BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA3= 2910012,
        BARTHS_MOON_TRANSMITTER_DOWNLOAD_DATA4 = 2910013,
        BARTHS_MOON_TRANSMITTER_ENTER_MAINLAB = 2910014,
        BARTHS_MOON_TRANSMITTER_DESTROY_COMPUTER = 2910015,
        BARTHS_MOON_TRANSMITTER_FIND_PART1= 2910016,
        BARTHS_MOON_TRANSMITTER_FIND_PART2= 2910017,
        BARTHS_MOON_TRANSMITTER_FIND_PART3 = 2910018,
        BARTHS_MOON_TRANSMITTER_FIND_PART4 = 2910019,
        BARTHS_MOON_TRANSMITTER_FIND_WAY_OUT = 2910020,
        BARTHS_MOON_TRANSMITTER_MEET_THOMAS2 = 29100201,
        BARTHS_MOON_TRANSMITTER_TALK_TO_BARTH_WAY_BACK = 2910021,


        BARTHS_MOON_TRANSMITTER_BUILD_TRANSMITTER = 29100030,
        BARTHS_MOON_TRANSMITTER_TALK_WITH_THOMAS_BARTH_END = 2910031,
        BARTHS_MOON_TRANSMITTER_RETURN_BACK_TO_MADELYN = 2910033,        

        BARTHS_MOON_PLANT = 2920000,
        BARTHS_MOON_PLANT_SAVE_BARTH = 2920001,
        BARTHS_MOON_PLANT_KILL_ATTACKERS= 2920002,
        BARTHS_MOON_PLANT_DEFENCE= 2920003,
        BARTHS_MOON_PLANT_BUILD_DEFENCE_LINE= 2920004,
        BARTHS_MOON_PLANT_PROTECT_BARTH = 2920005,
        BARTHS_MOON_PLANT_PROTECT_MADELYN = 2920006,
        BARTHS_MOON_PLANT_ENEMY = 2920007,
        BARTHS_MOON_PLANT_DESTROY_GENERATORS = 2920008,
        BARTHS_MOON_PLANT_GET_NEEDED_COMPONENTS = 29200081,
        BARTHS_MOON_PLANT_CONSTRUCT_DETECTORS = 29200082,
        BARTHS_MOON_PLANT_TALK_BARTH = 2920009,
        BARTHS_MOON_PLANT_START_GENERATOR = 2920010,
        BARTHS_MOON_PLANT_BUILD_PLANT= 2920011,

        BARTHS_MOON_PLANT_BUILD_MANUFACTURING_PLANT = 2920031,
        BARTHS_MOON_PLANT_TALK_WITH_THOMAS_BARTH = 2920032,
        BARTHS_MOON_PLANT_RETURN_BACK_TO_MADELYN = 2920033,
        
        NEW_STORY_M331 = 3100000,
        NEW_STORY_M331_REACH_SIDE_ENTRANCE = 3100001,
        NEW_STORY_M331_PLACE_POWER_JAMMER_ON_REACTOR = 3100002,
        NEW_STORY_M331_ENTER_THE_BASE = 3100003,
        NEW_STORY_M331_SHUT_DOWN_ENERGY_1 = 3100004,
        NEW_STORY_M331_SHUT_DOWN_ENERGY_2 = 3100005,
        NEW_STORY_M331_PLACE_DEVICE_ON_TRANSMITTER = 3100006,
        NEW_STORY_M331_ESCAPE_THE_OUTPOST_1 = 3100007,
        NEW_STORY_M331_ESCAPE_THE_OUTPOST_2 = 3100008,

        EAC_TRANSMITTER = 3500000,
        EAC_TRANSMITTER_OPEN_CARGO = 3500010,
        EAC_TRANSMITTER_CENTRAL_ROOM = 3500020,
        EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_A = 3500030,
        EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_B = 3500031,
        EAC_TRANSMITTER_UNLOCK_WAY_TO_SATELLITE_C = 3500032,
        EAC_TRANSMITTER_HACK_SATELLITE_A = 3500040,
        EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_ESCAPE = 3500045,
        EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HACK = 3500046,
        EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_DESTROY = 3500047,
        EAC_TRANSMITTER_BLOCK_AUTODESTRUCT_HELP = 3500048,
        EAC_TRANSMITTER_HACK_SATELLITE_B = 3500050,
        EAC_TRANSMITTER_HACK_SATELLITE_C = 3500060,
        EAC_TRANSMITTER_START_TRANSMISSION = 3500070,
        EAC_TRANSMITTER_OPEN_SOLAR_ARM = 3500080,
        EAC_TRANSMITTER_FIND_REPAIR = 3500082,
        EAC_TRANSMITTER_FIX_GENERATOR = 3500085,
        EAC_TRANSMITTER_ACTIVATE_SOLARPANELS = 3500090,
        EAC_TRANSMITTER_RESTART_TRANSMISSION = 3500095,
        EAC_TRANSMITTER_MEETMS = 3500100,

        SLAVER_BASE_1 = 3600000,
        SLAVER_BASE_1_FIND_SLAVES = 3600001,
        SLAVER_BASE_1_FREE_SLAVES = 3600002,
        SLAVER_BASE_1_FREE_SLAVES_2 = 3600003,
        SLAVER_BASE_1_FREE_SLAVES_3 = 3600004,
        SLAVER_BASE_1_FREE_SLAVES_4 = 3600005,
        SLAVER_BASE_1_FREE_SLAVES_5 = 3600006,
        SLAVER_BASE_1_FIND_SLAVES_2 = 3600007,
        SLAVER_BASE_1_DESTROY_GENERATOR = 3600008,
        SLAVER_BASE_1_DESTROY_BATTERIES = 3600009,
        SLAVER_BASE_1_RETURN = 3600010,
        SLAVER_BASE_1_FIND_PRISON = 3600011,
        SLAVER_BASE_1_DESTROY_ENEMIES = 3600012,  
        SLAVER_BASE_1_HACK_HANGAR = 3600013,
        SLAVER_BASE_1_RETURN_FAKE = 3600014,
        SLAVER_BASE_1_HACK_NUKE = 3600015 ,
        SLAVER_BASE_1_NAVIGATION = 3600016,
        SLAVER_BASE_1_DIALOG_1 = 3600017,
        SLAVER_BASE_1_DIALOG_2 = 3600018,
        SLAVER_BASE_1_DIALOG_3 = 3600019,
        SLAVER_BASE_1_DIALOG_4 = 3600020,
        SLAVER_BASE_1_DIALOG_5 = 3600021,
        SLAVER_BASE_1_DIALOG_6 = 3600022,
        SLAVER_BASE_1_DIALOG_7 = 3600023,
        SLAVER_BASE_1_DIALOG_8 = 3600024,
        SLAVER_BASE_1_DIALOG_9 = 3600025,
        SLAVER_BASE_1_DIALOG_10 = 3600026,

        SLAVER_BASE_2 = 3700000,
        SLAVER_BASE_2_PARALYZE_DEFENSE = 3700001,
        UNLOCK_PRISON_1 = 3700002,
        UNLOCK_PRISON_2 = 3700003,
        SLAVER_BASE_2_BREAK_THE_CHAINS = 3700004,
        SLAVER_BASE_2_FREE_SLAVES = 3700005,
        SLAVER_BASE_2_FINAL_FREEDOM = 3700010,
        SLAVER_BASE_2_RETURN = 3700011,
        SLAVER_BASE_2_INTRO = 3700012,
        SLAVER_BASE_2_TALK_ABOUT_GENERATOR = 3700013,
        SLAVER_BASE_2_TALK_ABOUT_PRISONERS = 3700014,

        RIME_BLUEPRINTS = 3800000,
        RIME_BLUEPRINTS_ENTRANCE1 = 3800001,
        RIME_BLUEPRINTS_ENTRANCE2 = 3800002,
        RIME_BLUEPRINTS_TARGET = 3800003,
        RIME_BLUEPRINTS_EXIT = 3800004,
        RIME_BLUEPRINTS_RETURN_TO_FRANCIS = 3800005,
        RIME_BLUEPRINTS_RETURN_TO_MOTHERSHIP = 3800006,
            
        RESEARCH_VESSEL = 38100000,
        RESEARCH_VESSEL_REACH_SHIP = 38100001,
        RESEARCH_VESSEL_CHECK_CARGO = 38100002,
        RESEARCH_VESSEL_CHECK_COMMAND_ROOM = 38100003,
        RESEARCH_VESSEL_TAKE_FIRST = 38100004,
        RESEARCH_VESSEL_USE_HUB_1 = 38100005,
        RESEARCH_VESSEL_CHECK_LABORATORY = 38100006,
        RESEARCH_VESSEL_CHECK_WAREHOUSE = 38100007,
        RESEARCH_VESSEL_CHECK_DRILL_ROOM = 38100008,
        RESEARCH_VESSEL_CHECK_FIRST_HANGAR = 38100009,
        RESEARCH_VESSEL_TAKE_SECOND = 38100010,
        RESEARCH_VESSEL_USE_HUB_2 = 38100011,
        RESEARCH_VESSEL_CHECK_SECOND_HANGAR = 38100012,
        RESEARCH_VESSEL_CHECK_THIRD_HANGAR = 38100013,
        RESEARCH_VESSEL_CHECK_SECOND_WAREHOUSE = 38100014,
        RESEARCH_VESSEL_CHECK_GENERATOR = 38100015,
        RESEARCH_VESSEL_TAKE_THIRD = 38100016,
        RESEARCH_VESSEL_USE_HUB_3 = 38100017,
        RESEARCH_VESSEL_CHECK_CARGO_AGAIN = 38100018,
        RESEARCH_VESSEL_TAKE_FOURTH = 38100019,
        RESEARCH_VESSEL_INTRO = 38100020,
        RESEARCH_VESSEL_TAKE_THIRD_DIALOGUE = 38100021,
        
       

        TRADE_STATION_EAC = 3900000,
        TRADE_STATION_EAC_HOSPITAL = 3900001,
        TRADE_STATION_EAC_CASINO = 3900002,
        TRADE_STATION_EAC_RETURN = 3900003,
        TRADE_STATION_EAC_DIALOGUE = 3900004,

        FORT_VALIANT = 4000000,
        FORT_VALIANT_B = 5500000,
        FORT_VALIANT_C = 560000,
        FORT_VALIANT_FLY_TARGET = 4000001,
        FORT_VALIANT_MEET_TEMPLAR_REPRESENTATIVES = 4000002,
        FORT_VALIANT_RETURN_BACK_TO_MADELYN = 4000003,
        FORT_VALIANT_TALK_WITH_TEMPLAR = 4000004,
        FORT_VALIANT_FIND_VENT_ENTRANCE = 4000005,
        FORT_VALIANT_GET_TO_ELEVATOR_SHAFT = 4000006,
        FORT_VALIANT_GET_TO_CAVE_ENTRANCE = 4000007,
        FORT_VALIANT_FIND_THE_ARTIFACT = 4000008,
        FORT_VALIANT_LEAVE_VAULT_OF_VALIANT = 4000009,
        FORT_VALIANT_RETURN_BACK_TO_MADELYN_2 = 4000010,
        FORT_VALIANT_ACTIVATE_SLAVE_MISSIONS = 4000011,

        FORT_VALIANT_FLY_ONE = 4000012,
        //FORT_VALIANT_FLY_REACH_GATE = 4000013,
        FORT_VALIANT_SPEAK_GATE_KEEPER = 4000014,
        FORT_VALIANT_MEET_CAPTAIN = 4000015,
        FORT_VALIANT_VISIT_VENDOR = 4000016,
        FORT_VALIANT_FLY_BACK_MADELYN = 4000017,





        FORT_VALIANT_C_VALUT = 5600016,
        FORT_VALIANT_C_TURN_OFF_SCANNER = 5600017,
        FORT_VALIANT_C_SCANNERS3 = 56000171,
        FORT_VALIANT_C_SCANNERS4 = 56000172,
        FORT_VALIANT_C_TOP_ELEVATOR = 5600018,
        FORT_VALIANT_C_VENT_SYSTEM = 5600019,
        FORT_VALIANT_C_CATACOMBS = 5600020,
        FORT_VALIANT_C_PICK_UP_EQUIP = 5600021,
        FORT_VALIANT_C_FLY_BACK_MADELYN = 5600022,


        //FORT_VALIANT_C_FOLLOW_TURN_OFF_GATE = 5600012,




        TRADE_STATION_ARABS = 4100000,
        //TRADE_STATION_ARABS_FLY_TARGET = 4100001,

        SOLAR_PLANT_CHINA = 4200000,
        SOLAR_PLANT_CHINA_DESTROY_TARGET_1 = 4200001,
        SOLAR_PLANT_CHINA_DESTROY_TARGET_2 = 4200002,
        SOLAR_PLANT_CHINA_FLY_TARGET = 4200003,

        CHINESE_REFINERY = 4300000,
        CHINESE_REFINERY_01_GET_CLOSER = 4300001,
        CHINESE_REFINERY_02_GET_IN = 4300002,
        CHINESE_REFINERY_03_FIND_SECRET_ROOM = 4300003,
        CHINESE_REFINERY_03_D_FIND_SECRET_ROOM = 4300004,
        CHINESE_REFINERY_04_SET_VIRUS = 4300005,
        CHINESE_REFINERY_05_DEACTIVATE_BOMB = 4300006,
        CHINESE_REFINERY_06_GET_TO_FIRST_TUNNEL = 4300007,
        CHINESE_REFINERY_07_PAST_FIRST_TUNNEL = 4300008,
        CHINESE_REFINERY_08_SET_BUG_IN_COMPUTER = 4300009,
        CHINESE_REFINERY_09_GET_TO_SECOND_TUNNEL = 4300010,
        CHINESE_REFINERY_10_PAST_SECOND_TUNNEL = 4300011,
        CHINESE_REFINERY_11_SNEAK_INSIDE_THE_STATION = 4300012,
        CHINESE_REFINERY_12_GET_TO_OLD_PATH = 4300013,
        CHINESE_REFINERY_13_HACK_REFINARY_COMPUTER = 4300014,
        CHINESE_REFINERY_14_GET_OUT_OF_THE_STATION = 4300015,
        CHINESE_REFINERY_15_LAND_INSIDE_THE_TRANSPORTER = 4300016,

        TRADE_STATION_CHINA = 4400000,
        TRADE_STATION_CHINA_FLY_INSIDE = 4400001,

        NEW_SINGAPOUR = 4500000,
        //NEW_SINGAPOUR_FLY = 4500001,

        REICHSTAG_C = 4600000,
        //REICHSTAG_C_MEETING = 4600010,
        REICHSTAG_C_FOR = 4600015,
        REICHSTAG_C_COLONEL_DIALOGUE = 4600016,
        REICHSTAG_C_GO_TO_SHIPYARD = 4600017,
        REICHSTAG_C_TALK_TO_SUPPLY_OFFICER = 4600018,
        REICHSTAG_C_CHANGESHIP = 4600020,
        REICHSTAG_C_SHIP_CHANGED_DIALOGUE = 4600021,
        REICHSTAG_C_WEAPONS = 4600025,
        REICHSTAG_C_GO_TO_HANGAR = 4600026,
        REICHSTAG_C_TRANSPORTER_CAPTAIN_DIALOGUE = 4600027,
        REICHSTAG_C_MOTHERSHIP = 4600030,
        REICHSTAG_C_RETURN = 4600035,

        SOLAR_PLANT_EAC = 4700000,
        SOLAR_PLANT_EAC_DESTROY = 4700001,
        SOLAR_PLANT_EAC_RETURN = 4700002,

        JUNKYARD_RETURN = 4800000,
        JUNKYARD_RETURN_MEET_SMUGGLER = 4800001,
        JUNKYARD_RETURN_SMUGGLER_DIALOGUE = 4800002,
        JUNKYARD_RETURN_FLY_TO_START = 4800003,
        JUNKYARD_RETURN_RACE = 4800004,
        JUNKYARD_RETURN_WIN = 4800005,
        JUNKYARD_RETURN_GO_TO_SMUGGLER = 4800006,
        JUNKYARD_RETURN_DIALOGUE_RETURN = 4800007,
        JUNKYARD_RETURN_SPEEDSTER_DIALOGUE = 4800009,

        RIME_CONVINCE = 4900000,
        RIME_CONVINCE_INTRODUCTION = 4900001,
        RIME_CONVINCE_GET_FRANCIS_REEF = 4900002,
        RIME_CONVINCE_TALK_FRANCIS_REEF = 4900003,
        RIME_CONVINCE_GO_TO_DUPLEX = 4900004,
        RIME_CONVINCE_CLIENTS_TALK = 4900005,
        RIME_CONVINCE_DUPLEX_BOUNCER = 4900020,
        RIME_CONVINCE_CONTACT_APPEARS = 4900021,
        RIME_CONVINCE_RETURN_TO_POSITION = 4900006,
        RIME_CONVINCE_FOLLOW_CONTACT = 4900007,
        RIME_CONVINCE_FACTORY_FOUND_DIALOGUE = 4900008,
        RIME_CONVINCE_DESTROY_FACTORY_BOTS = 4900009,
        RIME_CONVINCE_COLLECT_CARGO = 4900010,
        RIME_CONVINCE_CARGO_COLLECTED_DIALOG = 4900011,
        RIME_CONVINCE_FLY_TO_VESSEL = 4900012,
        RIME_CONVINCE_WAIT_FOR_THE_MOMENT = 4900013,
        RIME_CONVINCE_PLANT_CARGO = 4900014,
        RIME_CONVINCE_GET_OUT_OF_THE_VESSEL = 4900015,
        RIME_CONVINCE_CARGO_PLANTED_OBJECTIVE_DIALOGUE = 4900016,
        RIME_CONVINCE_GO_BACK_TO_FRANCIS = 4900017,
        RIME_CONVINCE_TALK_TO_FRANCIS= 4900018,
        RIME_CONVINCE_GO_BACK_TO_MADELYN = 4900019,

        ALIEN_GATE = 5000000,
        ALIEN_GATE_RIGHT_WING = 500000001,
        ALIEN_GATE_FOLLOW_COORDINATES = 50000001,
        ALIEN_GATE_CONTINUE_SEARCHING = 50000002,
        ALIEN_GATE_FOLLOW_DIRECTION = 50000003,
        ALIEN_GATE_COUGHT_IN_TRAP = 50000004,
        ALIEN_GATE_RUN_FOR_LIFE = 50000005,
        ALIEN_GATE_REGROUP_WITH_MADELYN = 50000006,
        ALIEN_GATE_BOARD_MOTHER_SHIP = 50000007,
        ALIEN_GATE_HACK_GENERATOR = 50000008,
        ALIEN_GATE_HACK_ENGINE = 50000009,
        ALIEN_GATE_LEAVE_SHIP = 50000010,
        ALIEN_GATE_REGROPUP_WITH_MADELYN = 50000011,
        ALIEN_GATE_BOARD_SECOND = 500000111,
        ALIEN_GATE_HACK_GENERATOR2 = 50000012,
        ALIEN_GATE_HACK_ENGINE_2 = 50000013,
        ALIEN_GATE_ENABLE_DOORS = 50000014,
        ALIEN_GATE_ENTER_LAB = 50000015,
        ALIEN_GATE_16 = 50000016,
        ALIEN_GATE_17 = 50000017,
        ALIEN_GATE_18 = 50000018,
        ALIEN_GATE_19 = 50000019,
        ALIEN_GATE_20 = 50000020,
        ALIEN_GATE_21 = 50000021,
        ALIEN_GATE_22 = 50000022,
        ALIEN_GATE_23 = 50000023,
        ALIEN_GATE_EXPLORE_THE_ALIEN_GATE = 5000001,
        ALIEN_GATE_DESTROY_INCOMING_ENEMY_FORCES = 5000002,
        ALIEN_GATE_DESTROY_GENERATOR_OF_ENEMY_MOTHER_SHIP = 5000003,

        JUNKYARD_EAC_AMBUSH = 51000000,
        JUNKYARD_EAC_AMBUSH_FLY_MANJEET= 5100001,
        JUNKYARD_EAC_AMBUSH_TALK_RANIJT = 51000002,
        JUNKYARD_EAC_AMBUSH_GO_BACK_TO_MADELYN = 51000003,
        JUNKYARD_EAC_AMBUSH_SPEAK_POLICE = 51000004,
        JUNKYARD_EAC_AMBUSH_DEFEND_MADELYN_1 =510000041, 
        JUNKYARD_EAC_AMBUSH_DESTROY_GENERATOR = 51000005,
        JUNKYARD_EAC_AMBUSH_RETUR_TO_MADELYN =  51000006,
        JUNKYARD_EAC_AMBUSH_DEFEND_MADELYN =  51000007,


        URANITE_MINE = 5200000,

        ARABIAN_BORDER = 5300000,

        CKD_MOTHERSHIP_FACILITY = 5400000,

        FORT_VALIANT_B_FLY_ONE = 5500012,
        //FORT_VALIANT_FLY_REACH_GATE = 4000013,
        FORT_VALIANT_B_SPEAK_GATE_KEEPER = 5500014,
        FORT_VALIANT_B_MEET_CAPTAIN = 5500015,
        FORT_VALIANT_B_VISIT_VENDOR = 5500016,
        FORT_VALIANT_B_FLY_BACK_MADELYN = 5500017,


        FORT_VALIANT_C_CAPTAIN = 560001,
        FORT_VALIANT_C_UPPER_FLOOR = 560002,
        FORT_VALIANT_C_EQUIP_TALK = 5600041,
        FORT_VALIANT_C_EQUIP = 5600042,
        FORT_VALIANT_C_MEET_OFFICIALS = 560005,
        FORT_VALIANT_C_LEAVE_OFFICIALS = 560006,
        FORT_VALIANT_C_LEAVE_FOLLOW = 5600061,
        FORT_VALIANT_C_SPEAK_SIR = 560008,
        FORT_VALIANT_C_GET_EQUP_CARGO = 560009,
        FORT_VALIANT_C_FOLLOW_FIND_VENTILATION = 5600010,
        FORT_VALIANT_C_FOLLOW_ENTER_VENTILATION = 5600011,
        FORT_VALIANT_C_FOLLOW_TURN_OFF_GATE = 5600012,
        FORT_VALIANT_C_SCANNERS1 = 5600013,
        FORT_VALIANT_C_SCANNERS2 = 5600014,
        FORT_VALIANT_C_SCANNERS23 = 56000141,
        FORT_VALIANT_C_TAKE_ARTEFACT = 5600015,


        HIPPIE_OUTPOST = 5700000,
        REACH_OUTPOST = 5700001,
        KILL_ALL = 5700002,


        MILITARY_OUTPOST = 5800000,

        NEW_JERUSALEM_MISSION = 590000,

        NEW_NANJING = 600000,

        REICHSTAG_A = 610000,
        REICHSTAG_A_INTRODUCTION = 610001,
        REICHSTAG_A_GET_TO_MAIN_BUILDING = 610002,
        REICHSTAG_A_MEET_COLONEL = 610003,
        REICHSTAG_A_COLONEL_DIALOGUE = 610004,

        CHINESE_CAPITAL = 620000,

        RUSSIAN_CAPITAL = 630000,

        ARABIAN_CAPITAL = 640000,

        EAC_CAPITAL = 650000,

        ASTEROID_COMPLEX = 660000,

        MOTHERSHIP_CRASH = 670000,

        INTERGALACTIC_HIGHWAY = 680000,

        CONVOY = 690000,

        BIOFACILITY = 711001,

        AUTONOMOUS_OUTPOST = 711002,

        GATES_OF_HELL = 711003,

        MINER_OUTPOST = 711004,

        MINER_UPRISING = 711005,

        INDUSTRIAL_SECTOR = 711006,
        
        FACTORY_AMBUSH = 711007,

        ASTEROID_FIELD = 711008,

        DUMPING_GROUND = 711009,

        FORGOTTEN_FACILITY = 711010,

        ZOMBIE_LEVEL = 711011,

        // TO DELETE
        PLAYGROUND = 999999,
        PLAYGROUND_SUBMISSION_01 = 999998,
        PLAYGROUND_SUBMISSION_02 = 999997,

        HUB_SHOWCASE = 3000000,         // In main menu play playground

        STEALTH_PLAYGROUND = 3000001,

        FAST_TEST_MISSION = 9900000,
        FAST_TEST_MISSION_TUNNEL1 = 9900010,
        FAST_TEST_MISSION_TUNNEL2 = 9900020,
        FAST_TEST_MISSION_TUNNEL3 = 9900030,

        SMART_WAVES_TEST_MISSION = 9901000,
        SMART_WAVES_TEST_MISSION_OBJECTIVE = 9901010,

        COOP_FOLLOW_HOST = 9902000,
        COOP_FOLLOW_HOST_OBJECTIVE = 9902001,

        TEST_MISSION = 9904000,
        TEST_MISSION_OBJECTIVE1 = 9904001,
        TEST_MISSION_OBJECTIVE2 = 9904002,

        TEST_ATTACK_MISSION = 9905000,
        TEST_ATTACK_MISSION_OBJECTIVE1 = 9905001,
        TEST_ATTACK_MISSION_OBJECTIVE2 = 9905002,
    }

    #endregion

    static class MyMissions
    {
        static readonly Dictionary<int, MyMissionBase> m_missions = new Dictionary<int, MyMissionBase>();
        static readonly Dictionary<MyMwcVector3Int, MyMission> m_sandboxMissions = new Dictionary<MyMwcVector3Int, MyMission>();

        static readonly List<MyMission> m_availableMissions = new List<MyMission>();
        static readonly List<MyMission> m_availableStoryMissions = new List<MyMission>();
        static bool m_inMissionScreen;
        /*
        static MyHudNotification.MyNotification m_missionNotification = new MyHudNotification.MyNotification(
                    MyTextsWrapperEnum.NotificationMissionAvailableInRange,
                    MyHudNotification.GetCurrentScreen(),
                    1f,
                    MyGuiManager.GetFontMinerWarsBlue(),
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                    (int)MySmallShipConstants.DETECT_INTERVAL + 50,
                    null,
                    false,
                    new[] { MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.MISSION_DIALOG) });
          */
                
        private static int m_newMissionAvailableLastTime = 0;
        private static MyMission m_availableMissionLastTime = null;

        static bool m_needsSave = false;
        public static string SaveName;
        public static bool NeedsSave
        {
            get { return m_needsSave; }
            set { m_needsSave = value; }
        }
        public static bool CreateChapter;

        private static MyMission _activeMission;
        public static MyMission ActiveMission 
        {
            get { return _activeMission; }
            set { _activeMission = value; }
        }

        static bool m_canSaveObjectives = true;

        static MyMissions()
        {
            AddMissions();
            //AddSandboxMissions();

            //Render.MyRender.RegisterRenderModule("Missions", Draw, Render.MyRenderStage.PrepareForDraw);

            //OutputMissionTree();

            MyScriptWrapper.EntityClosing += new MyScriptWrapper.EntityHandler(MyScriptWrapper_EntityClosing);
        }

        static void MyScriptWrapper_EntityClosing(MyEntity entity)
        {
            if (entity.EntityId.HasValue && (ActiveMission != null) && ActiveMission.Objectives.Count > 0)
            {
                foreach (var objective in ActiveMission.Objectives)
                {
                    if (objective.FailIfEntityDestroyed && !MySession.Static.EventLog.IsMissionFinished(objective.ID))
                    {
                        FailIfRequiredEntity(entity, objective.RequiredEntityIDs);
                        FailIfRequiredEntity(entity, objective.MissionEntityIDs);
                    }
                }
            }
        }

        static void FailIfRequiredEntity(MyEntity entity, List<uint> entityIDs)
        {
            foreach (var entityID in entityIDs)
            {
                if (entity.EntityId.Value.NumericValue == entityID)
                {
                    ActiveMission.Fail(MyTextsWrapperEnum.MissionFail_RequiredObjectDestroyed);
                    return;
                }
            }
        }

        //private static void OutputMissionTree()
        //{
        //    List<MyMissionBase> rootMissions = new List<MyMissionBase>();
        //    foreach (var mission in m_missions)
        //    {
        //        if (mission.Value.RequiredMissions.Length == 0)
        //        {
        //            rootMissions.Add(mission.Value);
        //        }
        //    }
        //    System.IO.StreamWriter writer = new System.IO.StreamWriter(System.IO.File.OpenWrite("missions.txt"));
        //    foreach (var item in rootMissions)
        //    {
        //        WriteDependentMissions(writer, item.ID, 0);
        //        writer.WriteLine();
        //    }
        //    writer.Flush();
        //    writer.Close();
        //}

        //private static void WriteDependentMissions(System.IO.StreamWriter writer, MyMissionID parentId, int intedationLevel)
        //{
        //    var parent = m_missions[(int)parentId];

        //    for (int i = 0; i < intedationLevel; i++)
        //    {
        //        writer.Write(">");
        //    }
        //    writer.Write(parent.DebugName ?? parent.NameTemp);
        //    writer.WriteLine();

        //    foreach (var item in m_missions.Values)
        //    {
        //        if (item.RequiredMissions.Contains(parent.ID))
        //        {
        //            WriteDependentMissions(writer, item.ID, intedationLevel + 1);
        //        }
        //    }
        //}

        private static void AddMissions()
        {
            //AddMission(new SinglePlayer.MyIntroMission());
            AddMission(new SinglePlayer.MyEACSurveySiteMission());
            AddMission(new SinglePlayer.MyPirateBaseMission());
            AddMission(new SinglePlayer.MyTwinTowersMission());
            AddMission(new SinglePlayer.MyChineseTransportMission());
            AddMission(new SinglePlayer.MyEACPrisonMission());
            AddMission(new SinglePlayer.MyRussianWarehouseMission());
            AddMission(new SinglePlayer.MyLaikaMission());
            AddMission(new SinglePlayer.MyChineseEscapeMission());
            AddMission(new SinglePlayer.MyLastHopeMission());
            AddMission(new SinglePlayer.MyPlaygroundMission());
            AddMission(new SinglePlayer.MyChineseTransmitterMission());
            AddMission(new SinglePlayer.MyWhiteWolvesResearchMission());
            AddMission(new SinglePlayer.MyRiftMission());
            AddMission(new SinglePlayer.MyJunkyardConvinceMission());
            AddMission(new SinglePlayer.MyStealthPlayground());
            AddMission(new SinglePlayer.MySmallPirateBaseMission());
            AddMission(new SinglePlayer.MySmallPirateBase2Mission());
            AddMission(new SinglePlayer.MyRussianTransmitterMission());
            AddMission(new SinglePlayer.MyBarthsMoonConvinceMission());
            AddMission(new SinglePlayer.MyEACTransmitterMission());



            /*AddMission(new SinglePlayer.MyRimeBlueprintsMission());*/
            AddMission(new SinglePlayer.MyResearchVesselMission());
            AddMission(new SinglePlayer.MyTradeStationEACMission());
            AddMission(new SinglePlayer.MyTradeStationChinaMission());
            AddMission(new SinglePlayer.MyNewSingaporeIndustryMission());
            AddMission(new SinglePlayer.MyChineseSolarArrayAttackMission());
            AddMission(new SinglePlayer.MyMedina622Mission());
            AddMission(new SinglePlayer.MyFortValiantMission());
            AddMission(new SinglePlayer.MySlaverBase2Mission());
            AddMission(new SinglePlayer.MyFortValiantMissionB());
            AddMission(new SinglePlayer.MySlaverBaseMission());
            AddMission(new SinglePlayer.MyFortValiantMissionC());
            AddMission(new SinglePlayer.MyReichstagAMission());
            AddMission(new SinglePlayer.MyReichstagCMission());
            AddMission(new SinglePlayer.MySolarfactoryEACMission());
            AddMission(new SinglePlayer.MyChineseRefineryMission());
            AddMission(new SinglePlayer.MyBarthsMoonTransmitterMission());
            AddMission(new SinglePlayer.MyBarthsMoonPlantMission());
            AddMission(new SinglePlayer.MyAlienGateMission());
            AddMission(new SinglePlayer.MyJunkyardReturnMission());
            AddMission(new SinglePlayer.MyRimeConvinceMission());
            AddMission(new SinglePlayer.MyJunkyardEACAmbushMission());
            AddMission(new SinglePlayer.MyPlutoniumMineMission());
            AddMission(new SinglePlayer.MyArabianBorderMission());
            AddMission(new SinglePlayer.MyCKDMothershipFacilityMission());
            AddMission(new SinglePlayer.MyHippieOutpostMission());
            AddMission(new SinglePlayer.MyMilitaryOutpostMission());
            AddMission(new SinglePlayer.MyNewJerusalemMission());
            AddMission(new SinglePlayer.MyNewNanjingMission());
            AddMission(new SinglePlayer.MyChineseCapitalMission());
            AddMission(new SinglePlayer.MyRussianCapitalMission());
            AddMission(new SinglePlayer.MyArabianCapitalMission());
            AddMission(new SinglePlayer.MyEACCapitalMission());
            AddMission(new SinglePlayer.MyAsteroidComplexMission());
            AddMission(new SinglePlayer.MyMothershipCrashMission());
            AddMission(new SinglePlayer.IntergalacticHighway());
            AddMission(new SinglePlayer.MyConvoyMission());
            AddMission(new SinglePlayer.MyBiofacilityMission());
            AddMission(new SinglePlayer.MyAutonomousOutpostMission());
            AddMission(new SinglePlayer.MyGatesOfHellMission());
            AddMission(new SinglePlayer.MyHeliumMinesMission());
            AddMission(new SinglePlayer.MyMinerUprisingtMission());
            AddMission(new SinglePlayer.MyIndustrialSectorMission());
            AddMission(new SinglePlayer.MyFactoryAmbushMission());
            AddMission(new SinglePlayer.MyAsteroidResearchFieldMission());
            AddMission(new SinglePlayer.MyDmpingGroundMission());
            AddMission(new SinglePlayer.MyForgottenFacilityMission());
            AddMission(new SinglePlayer.MyZombieLevelMission());


            AddMission(new SinglePlayer.MyTestMission());
            AddMission(new SinglePlayer.MyTestAttackMission());

        }

        public static void AddSandboxMissions()
        {
            AddSandboxMission(new Sandbox.MyHubShowcaseMission());
        }

        public static void AddSandboxMission(MyMission mission)
        {
            Debug.Assert(mission.Location != null, "Sandbox mission must have one location (one sector)");
            m_sandboxMissions.Add(mission.Location.Sector, mission);
        }

        public static void ReloadTexts()
        {
            foreach (var mission in m_missions)
            {
                mission.Value.ReloadName();
            }
        }


        /// <summary>
        /// Starts mission for current sandbox sector, when mission exists.
        /// Otherwise do nothing.
        /// </summary>
        /// <returns>Accepted mission or null.</returns>
        public static MyMission StartSandboxMission(MyMwcVector3Int sectorPosition)
        {
            MyMission mission;
            if (m_sandboxMissions.TryGetValue(sectorPosition, out mission))
            {
                mission.Accept();
                return mission;
            }
            return null;
        }

        //static void Draw()
        //{
        //    // Dont draw cubes in editor etc.
        //    if (!MyGuiScreenGamePlay.Static.IsGameActive())
        //    {
        //        return;
        //    }

        //    Matrix world = Matrix.Identity;
        //    Vector4 vctColorPoly = MyHudConstants.MISSION_CUBE_COLOR;

        //    BoundingBox boundingBox = new BoundingBox(-Vector3.One * MyMissionsConstants.MISSION_CUBE_SIZE_HALF, Vector3.One * MyMissionsConstants.MISSION_CUBE_SIZE_HALF); 
            
        //    // Draw cubes for available missions
        //    var availableMissions = GetAvailableMissions();
        //    foreach (var mission in availableMissions)
        //    {
        //        if (mission.Location != null && 
        //            (MySession.Static == null || mission != MySession.Static.ActiveMission) && 
        //            MyGuiScreenGamePlay.Static.IsCurrentSector(mission.Location.Sector))
        //        {
        //            world = Matrix.CreateTranslation(mission.Location.Location);
        //            MySimpleObjectDraw.DrawTransparentBox(ref world, ref boundingBox, ref vctColorPoly, true, 1);
        //        }
        //    }

        //    // Draw cubes for active submissions
        //    if (MySession.Static != null && MySession.Static.ActiveMission != null)
        //    {
        //        foreach (var activeSubMission in MySession.Static.ActiveMission.ActiveSubmissions)
        //        {
        //            if (activeSubMission.Location != null && MyGuiScreenGamePlay.Static.IsCurrentSector(activeSubMission.Location.Sector))
        //            {
        //                world = Matrix.CreateTranslation(activeSubMission.Location.Location);
        //                MySimpleObjectDraw.DrawTransparentBox(ref world, ref boundingBox, ref vctColorPoly, true, 1);
        //            }
        //        }
        //    }
        //}

        public static void AddMission(MyMission mission)
        {
            Debug.Assert(mission.RequiredMissions != null, "Required mission cannot be null");
            Debug.Assert(mission.IsValid(), "Mission is not valid! For mission without objectives use MyMissionSandboxBase and for mission with objectives use MyMission!");

            // Write a list of missions and objectives into a file.

            //System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\mission_list.txt", append:true);
            //file.WriteLine(MyTextsWrapper.Get(mission.Name) + " (" + mission.DebugName + ")");

            m_missions.Add((int)mission.ID, mission);
            foreach (var objective in mission.Objectives)
            {
                System.Diagnostics.Debug.Assert(!m_missions.ContainsKey((int)objective.ID), string.Format("Objective {0} already added!", objective.NameTemp));
                m_missions.Add((int)objective.ID, objective);
                //file.WriteLine(objective.ID + "\t" + objective.GetType() + "\t" + objective.NameTemp);
            }
            //file.WriteLine("");
            //file.Close();
        }

        public static void ClearSideMissions()
        {
            // Make copy to allow foreach
            foreach (var mission in m_missions.ToArray())
            {
                if (mission.Value.IsSideMission)
                {
                    m_missions.Remove(mission.Key);
                }
            }
        }

        /// <summary>
        /// Returns a MyMission or MySubmission object with the specified ID.
        /// </summary>
        public static MyMissionBase GetMissionByID(MyMissionID ID)
        {
            if (m_missions.ContainsKey((int)ID))
                return m_missions[(int)ID];

            return null;
        }


        public static void Update()
        {
            if (NeedsSave)
            {
                var session = MySession.Static;
                bool canSave = session != null 
                    && MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.GAME_STORY // save only in story
                    && !MyFakes.DISABLE_AUTO_SAVE
                    && (!MyMultiplayerGameplay.IsRunning || MyMultiplayerGameplay.Static.IsHost) // Save only when multiplayer is not running or i am the host
                    && (MySession.PlayerShip != null && !MySession.PlayerShip.IsDead()); // Save only when i am alive

                if (canSave)
                {
                    if (MyFakes.CHAPTER_ON_EACH_MISSION)
                    {
                        session.SaveLastCheckpoint(true);
                    }
                    else
                    {
                        session.SaveLastCheckpoint(CreateChapter);
                    }
                }
                CreateChapter = false;
                NeedsSave = false;
            }

            var mission = MyMissions.ActiveMission;
            if (mission != null)
            {
                if (!mission.MarkedForUnload)
                    mission.Update();

                if (mission.MarkedForUnload)
                {
                    mission.Unload();
                }
            }
        }

        /// <summary>
        /// Refreshes the list of available missions. Call after some change in missions (success, fail, sector change).
        /// </summary>
        public static void RefreshAvailableMissions()
        {
            //MyMwcLog.WriteLine("MyMissions::RefreshAvailableMissions - START");
            //MyMwcLog.IncreaseIndent();

            m_availableMissions.Clear();
            m_availableStoryMissions.Clear();

            // Run missions only on host
            if (MyMultiplayerGameplay.IsRunning && !MyMultiplayerGameplay.Static.IsHost)
                return;

            if (MyGuiScreenGamePlay.Static == null || MyGuiScreenGamePlay.Static.IsEditorActive() || MySession.Static == null || MySession.Static.EventLog == null
                || (MyGuiScreenGamePlay.Static.GetPreviousGameType().HasValue && (MyGuiScreenGamePlay.Static.GetPreviousGameType().Value == MyGuiScreenGamePlayType.EDITOR_STORY || MyGuiScreenGamePlay.Static.GetPreviousGameType().Value == MyGuiScreenGamePlayType.EDITOR_SANDBOX)
                || MyGuiScreenGamePlay.Static.GetGameType() == MyGuiScreenGamePlayType.GAME_SANDBOX)
                || (MySession.PlayerShip == null) || (MySession.PlayerShip.IsDead()))
                return;
            
            // because we dont want unnecessary memory allocation
            foreach (KeyValuePair<int, MyMissionBase> missionKeyValuePair in m_missions)
            {
                MyMission mission = missionKeyValuePair.Value as MyMission;
                if (mission != null)
                {
                    bool available = missionKeyValuePair.Value.IsAvailable() && !MySession.Static.EventLog.IsMissionFinished(missionKeyValuePair.Value.ID);
                    if (available)
                    {
                        m_availableMissions.Add(mission);

                        if (mission.RequiredMissions.Length > 0)
                            m_availableStoryMissions.Add(mission);
                    }

                    // Show or hide location dummy
                    mission.SetLocationVisibility(available);
                }
            }

            //MyMwcLog.DecreaseIndent();
            //MyMwcLog.WriteLine("MyMissions::RefreshAvailableMissions - END");            
        }

        /// <summary>
        /// Returns the list of available missions. Can be out of date - use RefreshAvailableMissions() in that case.
        /// </summary>
        /// <returns></returns>
        public static List<MyMission> GetAvailableMissions()
        {
            return m_availableMissions;
        }

        /// <summary>
        /// Returns the list of story missions. 
        /// </summary>
        /// <returns></returns>
        public static List<MyMission> GetAvailableStoryMissions()
        {
            return m_availableStoryMissions;
        } 

        public static bool IsNewMissionOrObjectiveAvailable()
        {
            if (ActiveMission != null)
            {
                if(ActiveMission.ActiveObjectives.Count > 0 &&
                    (ActiveMission.ActiveObjectives[0].GetObjectiveStartTime() + MyMissionsConstants.NEW_OBJECTIVE_FOR_TIME) >= MyMinerGame.TotalGamePlayTimeInMilliseconds)
                {
                    return true;
                }
            }
            else
            {
                MyMission firstAvailableMission = GetAvailableMissions().Count > 0 ? GetAvailableMissions()[0] : null;
                if (firstAvailableMission != m_availableMissionLastTime)
                {
                    m_availableMissionLastTime = firstAvailableMission;
                    m_newMissionAvailableLastTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                }
                if (m_availableMissionLastTime != null && m_newMissionAvailableLastTime + MyMissionsConstants.NEW_OBJECTIVE_FOR_TIME >= MyMinerGame.TotalGamePlayTimeInMilliseconds)
                {
                    return true;
                }
            }
            return false;
        }

        private static MyMission GetAvailableMissionAtPosition()
        {
            var boundingSphere = MySession.PlayerShip.WorldVolume;
            RefreshAvailableMissions();
            foreach (var mission in m_availableMissions)
            {
                if (mission.HasLocationEntity())
                {
                    if (mission.Location.Entity.GetIntersectionWithSphere(ref boundingSphere))
                    {
                        return mission;
                    }
                }
            }
            return null;
        }

        public static void CheckMissionProximity()
        {
            if (m_inMissionScreen)
                return;

            /*
            var mission = GetAvailableMissionAtPosition();
            if (mission != null)
            {
                MyHudNotification.AddNotification(m_missionNotification); 
            } */
        }

        public static bool RequestMissionDialog()
        {
            /*
            var mission = GetAvailableMissionAtPosition();
            if (mission != null)
            {
                MyGuiManager.AddScreen(new MyGuiScreenMission(mission));
                m_missionNotification.Disappear();
                m_inMissionScreen = true;
                return true;
            } */
            return false;
        }

        public static void HandleCloseMissionScreen()
        {
            m_inMissionScreen = false;
            CheckMissionProximity();
            RefreshAvailableMissions();
            //m_missionNotification.Appear();
        }

        public static void Unload()
        {
            ClearSideMissions();

            if (ActiveMission != null)
                ActiveMission.Unload();

            m_canSaveObjectives = true;

            /*
            foreach (var missionItem in m_missions)
            {
                // CleanUp only missions because they automaticaly CleanUp theyr submissions
                if (missionItem.Value is MyMission)
                {
                    missionItem.Value.Unload();
                }
            } */
        }

        public static Dictionary<int, MyMissionBase> Missions
        {
            get
            {
                return m_missions;
            }
        }

        public static MyObjective GetSubmissionByEntity(MyEntity target)
        {            
            if (ActiveMission != null)
            {
                foreach (var sub in ActiveMission.ActiveObjectives)
                {
                    if (sub.IsMissionEntity(target))
                    {
                        return sub;
                    }
                }
            }            
            return null;
        }

        public static bool IsMissionEntity(MyEntity target)
        {
            if (ActiveMission != null)
            {
                return ActiveMission.IsMissionEntity(target);
            }
            else
            {
                foreach (var mission in GetAvailableMissions())
                {
                    if (mission.HasLocationEntity() && mission.Location.Entity == target)
                    {
                        return true;
                    }
                }
            }            

            //No mission entity in the sector, try to find hangar to visit solar map
            MyPrefabHangar hangar = target as MinerWars.AppCode.Game.Entities.Prefabs.MyPrefabHangar;
            if (hangar != null && MyFactions.GetFactionsRelation(MySession.Static.Player.Faction, hangar.Faction) == World.Global.MyFactionRelationEnum.Friend && hangar.Name == MyMissionBase.MyMissionLocation.MADELYN_HANGAR)
            {
                return true;
            }

            return false;
        }

        public static bool IsMissionEntityNotification(MyEntity entity, MySmallShipInteractionActionEnum action)
        {
            
            return ActiveMission != null && ActiveMission.IsMissionEntityNotification(entity, action)
                || ActiveMission == null && action == MySmallShipInteractionActionEnum.Travel && MyMissions.IsMissionEntity(entity);
        }

        public static void AddSolarMapMarks(MySolarSystemMapData data)
        {
            foreach (var mission in m_missions.Values.OfType<MyMission>())
            {
                if (mission.Location != null && !MyGuiScreenGamePlay.Static.IsCurrentSector(mission.Location.Sector) && mission.ShowNavigationMark)
                {
                    if (!mission.Flags.HasFlag(MyMissionFlags.HiddenInSolarMap) && mission.IsAvailable())
                    {
                        bool isStory = mission.Flags.HasFlag(MyMissionFlags.Story);

                        var missionMark = new MySolarSystemMapNavigationMark(
                            mission.Location.Sector,
                            MyTextsWrapper.Get(mission.Name).ToString(),
                            mission.ID,
                            /*isStory ? MyHudConstants.SOLAR_MAP_STORY_MISSION_MARKER_COLOR : */MyHudConstants.SOLAR_MAP_SIDE_MISSION_MARKER_COLOR,
                            isStory ? TransparentGeometry.MyTransparentMaterialEnum.SolarMapStoryMission : TransparentGeometry.MyTransparentMaterialEnum.SolarMapSideMission)
                            {
                                //Description = mission.DescriptionTemp.ToString(),
                                DirectionalTexture = mission.Flags.HasFlag(MyMissionFlags.Story) ? MyHudTexturesEnum.DirectionIndicator_blue : MyHudTexturesEnum.DirectionIndicator_white,
                                IsBlinking = isStory,
                                Text = isStory ? MyTextsWrapper.Get(MyTextsWrapperEnum.NewMission).ToString() : null,
                                TextSize = isStory ? 0.9f : 0.7f,
                                Font = isStory ? MyGuiManager.GetFontMinerWarsBlue() : MyGuiManager.GetFontMinerWarsWhite(),
                                Importance = isStory ? 100 : 1
                            };
                        missionMark.VerticalLineColor = missionMark.Color.ToVector4();

                        data.NavigationMarks.Add(missionMark);
                    }
                    else
                    {
                        bool any = false;
                        foreach (MyMissionBase value in m_missions.Values)
                        {
                            MyMission m = value as MyMission;
                            if (m != null)
                            {
                                if (m.Location.Sector == mission.Location.Sector && m.IsAvailable())
                                {
                                    any = true;
                                    break;
                                }
                            }
                        }
                        if (!any && mission.IsCompleted() && !data.NavigationMarks.Contains(mission.Location.Sector))
                        {
                            var missionMark = new MySolarSystemMapNavigationMark(
                                mission.Location.Sector,
                                MyTextsWrapper.Get(mission.Name).ToString(),
                                null,
                                Color.Gray,
                                TransparentGeometry.MyTransparentMaterialEnum.SolarMapSideMission)
                                                  {
                                                      //Description = mission.DescriptionTemp.ToString(),
                                                      DirectionalTexture = MyHudTexturesEnum.DirectionIndicator_white,
                                                      Text = "Completed",
                                                      TextSize = 0.6f,
                                                  };
                            missionMark.VerticalLineColor = missionMark.Color.ToVector4();
                            data.NavigationMarks.Add(missionMark);
                        }
                    }
                }
            }
        }

        public static bool IsGameStoryCompleted()
        {
            return GetMissionByID(MyMissionID.ALIEN_GATE).IsCompleted();
        }

        public static void DisableSaveObjectives()
        {
            m_canSaveObjectives = false;
        }

        public static bool CanSaveObjectives()
        {
            return m_canSaveObjectives;
        }

        public static int GetMissionNumber(MyMissionID missionID)
        {
            List<MyMissionID> missions = new List<MyMissionID>
            {
                MyMissionID.EAC_SURVEY_SITE,            // 1 - Tier 1
                MyMissionID.LAIKA,                      // 2
                MyMissionID.BARTHS_MOON_CONVINCE,       // 3
                MyMissionID.PIRATE_BASE,                // 4
                MyMissionID.RUSSIAN_WAREHOUSE,          // 5 - Tier 2
                MyMissionID.BARTHS_MOON_TRANSMITTER,    // 6
                MyMissionID.LAST_HOPE,                  // 7
                MyMissionID.JUNKYARD_CONVINCE,          // 8 - Tier 3
                MyMissionID.CHINESE_TRANSPORT,          // 9
                MyMissionID.CHINESE_REFINERY,           // 10
                MyMissionID.CHINESE_ESCAPE,             // 11
                MyMissionID.JUNKYARD_RETURN,            // 12 - Tier 4
                MyMissionID.FORT_VALIANT,               // 13
                MyMissionID.SLAVER_BASE_1,              // 14
                MyMissionID.FORT_VALIANT_B,             // 15
                MyMissionID.SLAVER_BASE_2,              // 16
                MyMissionID.FORT_VALIANT_C,             // 17 - Tier 5
                MyMissionID.RIME_CONVINCE,              // 18
                MyMissionID.RESEARCH_VESSEL,            // 19 - Tier 6
                MyMissionID.JUNKYARD_EAC_AMBUSH,        // 20
                MyMissionID.RIFT,                       // 21
                MyMissionID.BARTHS_MOON_PLANT,          // 22 - Tier 7
                MyMissionID.CHINESE_TRANSMITTER,        // 23
                MyMissionID.RUSSIAN_TRANSMITTER,        // 24
                MyMissionID.REICHSTAG_A,                // 25
                MyMissionID.NAZI_BIO_LAB,               // 26 - Tier 8
                MyMissionID.REICHSTAG_C,                // 27
                MyMissionID.TWIN_TOWERS,                // 28 - Tier 9
                MyMissionID.EAC_PRISON,                 // 29
                MyMissionID.EAC_TRANSMITTER,            // 30
                MyMissionID.ALIEN_GATE,                 // 31
            };

            int index = missions.IndexOf(missionID);
            return index < 0 ? index : index + 1;       // first mission is 1
        }

        public static int GetCurrentTier()
        {
            var missionNumber = ActiveMission != null ? GetMissionNumber(ActiveMission.ID) : -1;
            int[] tierLevels = { 4, 7, 11, 16, 18, 21, 25, 27 };

            int tier = 0;
            if (missionNumber > 0)
            {
                while (tier < tierLevels.Length && missionNumber > tierLevels[tier])
                {
                    ++tier;
                }
                ++tier; // first tier is 1
            }
            
            return tier;
        }
    }
}
