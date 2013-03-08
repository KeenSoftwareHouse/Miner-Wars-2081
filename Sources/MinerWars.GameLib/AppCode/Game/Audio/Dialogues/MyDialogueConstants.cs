using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;
using System.IO;

namespace MinerWars.AppCode.Game.Audio.Dialogues
{
    enum MyDialogueEnum
    {
        NONE,
        TEST,

        EAC_SURVEY_SITE_0200_ACTIONSTARTS,
        EAC_SURVEY_SITE_0300_CIVILIANSDESTROYED,
        EAC_SURVEY_SITE_0400_BARRICADE,
        EAC_SURVEY_SITE_0500_TOBASE,
        EAC_SURVEY_SITE_0450_MOTHERSHIPS,
        EAC_SURVEY_SITE_0700_MADELYN,
        EAC_SURVEY_SITE_0800_PICKUP,
        EAC_SURVEY_SITE_0550_COMMANDOFFLINE,
        EAC_SURVEY_SITE_0560_GENERATORUP,
        EAC_SURVEY_SITE_1000_RESEARCHERS_1,
        EAC_SURVEY_SITE_1100_RESEARCHERS_2,
        EAC_SURVEY_SITE_1200_RUSSIANCHAT_01,
        EAC_SURVEY_SITE_1300_RUSSIANCHAT_02,
        EAC_SURVEY_SITE_1400_RUSSIANCHAT_03,
        EAC_SURVEY_SITE_1500_RUSSIANCHAT_04,
        EAC_SURVEY_SITE_1600_RUSSIANCHAT_05,
        EAC_SURVEY_SITE_0205_ACTION,
        EAC_SURVEY_SITE_0705_MADELYN_2,
        EAC_SURVEY_SITE_0750_NEAREND,
        EAC_SURVEY_SITE_1900_RUSSIANCHAT_06,
        EAC_SURVEY_SITE_2000_RUSSIANCHAT_07,
        EAC_SURVEY_SITE_2100_RUSSIANCHAT_08,


        LAIKA_0100_ARRIVAL,
        LAIKA_0200_HANGAR,
        LAIKA_0250_WAYTOHANGAR,
        LAIKA_0275_INSIDEHANGAR,
        LAIKA_0277_DESTROYGENERATOR,
        LAIKA_0300_AFTERHANGAR,
        LAIKA_0350_GPS,
        LAIKA_0400_MARKETPLACE,
        LAIKA_0480_COMMANDARRIVE,
        LAIKA_0500_COMMAND,
        LAIKA_0600_TOPIPE,
        LAIKA_0700_LEFTHUB,
        LAIKA_0700_LEFTHUB2,
        LAIKA_0720_AFTERLEFTHUB,
        LAIKA_0800_RIGHTHUB,
        LAIKA_0900_WARHEAD,
        LAIKA_0950_WARHEAD_HACK,
        LAIKA_1800_WARHEAD_DONE,
        LAIKA_1800_WARHEAD_DONE2,
        LAIKA_1900_ESCAPED,

    
        BARTHS_MOON_CONVINCE_0100,
        BARTHS_MOON_CONVINCE_0200,
        BARTHS_MOON_CONVINCE_0300,
        BARTHS_MOON_CONVINCE_0400,
        BARTHS_MOON_CONVINCE_0500,
        BARTHS_MOON_CONVINCE_0600,
        BARTHS_MOON_CONVINCE_0700,

 
        PIRATE_BASE_0100,
        PIRATE_BASE_0200,
        PIRATE_BASE_0300,
        PIRATE_BASE_0400,
        PIRATE_BASE_0500,
        PIRATE_BASE_0600,
        PIRATE_BASE_0700,
        PIRATE_BASE_0800,
        PIRATE_BASE_0900,
        PIRATE_BASE_1000,
        PIRATE_BASE_1100,
        PIRATE_BASE_1200,
        PIRATE_BASE_1300,
        PIRATE_BASE_1400,
        PIRATE_BASE_1500,


        RUSSIAN_WAREHOUSE_0100,
        RUSSIAN_WAREHOUSE_0200,
        RUSSIAN_WAREHOUSE_0300,
        RUSSIAN_WAREHOUSE_0400,
        RUSSIAN_WAREHOUSE_0500,
        RUSSIAN_WAREHOUSE_0600,
        RUSSIAN_WAREHOUSE_0700,
        RUSSIAN_WAREHOUSE_0800,
        RUSSIAN_WAREHOUSE_0900,
        RUSSIAN_WAREHOUSE_1000,
        RUSSIAN_WAREHOUSE_1100,

        BARTHS_MOON_TRANSMITTER_0100_INTRO = 1900,
        BARTHS_MOON_TRANSMITTER_0200,
        BARTHS_MOON_TRANSMITTER_0300,
        BARTHS_MOON_TRANSMITTER_0400,
        BARTHS_MOON_TRANSMITTER_0500,
        BARTHS_MOON_TRANSMITTER_0600,
        BARTHS_MOON_TRANSMITTER_0700,
        BARTHS_MOON_TRANSMITTER_0800,
        BARTHS_MOON_TRANSMITTER_0900,
        BARTHS_MOON_TRANSMITTER_1000,
        BARTHS_MOON_TRANSMITTER_1100,
        BARTHS_MOON_TRANSMITTER_1200,
        BARTHS_MOON_TRANSMITTER_1300,
        BARTHS_MOON_TRANSMITTER_1400,
        BARTHS_MOON_TRANSMITTER_1500,
        BARTHS_MOON_TRANSMITTER_1600,
        BARTHS_MOON_TRANSMITTER_1700,
        BARTHS_MOON_TRANSMITTER_1800,
        BARTHS_MOON_TRANSMITTER_1900,
        BARTHS_MOON_TRANSMITTER_2000,
        BARTHS_MOON_TRANSMITTER_2100,
        BARTHS_MOON_TRANSMITTER_2200,
        BARTHS_MOON_TRANSMITTER_2300,
        BARTHS_MOON_TRANSMITTER_2400,
        BARTHS_MOON_TRANSMITTER_2500,
        BARTHS_MOON_TRANSMITTER_2600,
        BARTHS_MOON_TRANSMITTER_2700,
        BARTHS_MOON_TRANSMITTER_2800,
        BARTHS_MOON_TRANSMITTER_2900,
        BARTHS_MOON_TRANSMITTER_3000,
        BARTHS_MOON_TRANSMITTER_3100,
        BARTHS_MOON_TRANSMITTER_3200,


        LAST_HOPE_0100,
        LAST_HOPE_0200,
        LAST_HOPE_0300,
        LAST_HOPE_0400,
        LAST_HOPE_0500,
        LAST_HOPE_0600,
        LAST_HOPE_0700,
        LAST_HOPE_0800,
        LAST_HOPE_1100,
        LAST_HOPE_1200,
        LAST_HOPE_1300,
        LAST_HOPE_1400,
        LAST_HOPE_1500,
        LAST_HOPE_1600,
        LAST_HOPE_1700,
        LAST_HOPE_1800,
        LAST_HOPE_1900,
        LAST_HOPE_2000,


        JUNKYARD_CONVINCE_0100_INTRODUCE,
        JUNKYARD_CONVINCE_0200_INFORMATOR,
        JUNKYARD_CONVINCE_0300_RUN,
        JUNKYARD_CONVINCE_0500_CATCHED,
        JUNKYARD_CONVINCE_0600_BEFORE_FIGHT,
        JUNKYARD_CONVINCE_0650_MET_ZAPPA_GUARD,
        JUNKYARD_CONVINCE_0800_MOMO_ARRIVE,
        JUNKYARD_CONVINCE_0900_THE_MOMO,
        JUNKYARD_CONVINCE_0950_MOMO_FIGHT,
        JUNKYARD_CONVINCE_1000_LAST_OF_THEM,
        JUNKYARD_CONVINCE_1100_FIGHT_ENDS,
        JUNKYARD_CONVINCE_1200_FLY_TO_DEALER,
        JUNKYARD_CONVINCE_1300_BOMB_DEALER,
        JUNKYARD_CONVINCE_1400_ARRIVED_AT_MARCUS,
        JUNKYARD_CONVINCE_1500_GANGSTER_FIGHT_STARTED,
        JUNKYARD_CONVINCE_1600_GO_TO_MS,
        JUNKYARD_CONVINCE_1700_FINALE,

        CHINESE_TRANSPORT_0100_INTRODUCE,
        CHINESE_TRANSPORT_0200_FIRST_DEVICE_HACKED,
        CHINESE_TRANSPORT_0300_SHOOTING_ON_ME,
        CHINESE_TRANSPORT_0400_THEY_FOUND_ME,
        CHINESE_TRANSPORT_0500_MARCUS_IS_HERE,
        CHINESE_TRANSPORT_0600_DESTROY_THE_TRANSMITTER,
        CHINESE_TRANSPORT_0650_MARCUS_IS_LEAVING,
        CHINESE_TRANSPORT_0670_disabling_the_terminals,
        CHINESE_TRANSPORT_0700_RUN,
        CHINESE_TRANSPORT_0800_GO_TO_SECOND_BASE,
        CHINESE_TRANSPORT_0850_INSIDE_TUNNEL,
        CHINESE_TRANSPORT_0900_SURRENDER,
        CHINESE_TRANSPORT_1000_DOOR_BLOCKED,
        CHINESE_TRANSPORT_1050_REACHED_COMPUTER,
        CHINESE_TRANSPORT_1075_HACKED_COMPUTER,
        CHINESE_TRANSPORT_1100_HELP_MARCUS,
        CHINESE_TRANSPORT_1200_GENERAL_ARRIVAL,
        CHINESE_TRANSPORT_1300_LAND_IN,
        CHINESE_TRANSPORT_1400_DOORS_UNLOCKED,


        CHINESE_REFINERY_0100_GO_CLOSER,
        CHINESE_REFINERY_0150_GET_INSIDE,
        CHINESE_REFINERY_0200_LABORATORY,
        CHINESE_REFINERY_0300_DEACTIVATE_BOMB,
        CHINESE_REFINERY_0400_GO_TO_SECOND_ASTEROID,
        CHINESE_REFINERY_0500_FIND_THE_COMPUTER,
        CHINESE_REFINERY_0600_GO_TO_THIRD_ASTEROID,
        CHINESE_REFINERY_0700_SNEAK_INSIDE_THE_STATION,
        CHINESE_REFINERY_0800_FIND_THE_OLD_PATH,
        CHINESE_REFINERY_0900_HACK_THE_COMPUTER,
        CHINESE_REFINERY_1000_GET_OUT,



        CHINESE_ESCAPE_0100_INTRODUCTION,
        CHINESE_ESCAPE_0200_IT_IS_TRAP,
        CHINESE_ESCAPE_0300_ON_THIRD,
        CHINESE_ESCAPE_0400_ON_NINE,
        CHINESE_ESCAPE_0500_WATCH_BACK,
        CHINESE_ESCAPE_0600_WATCH_FRONT,
        CHINESE_ESCAPE_0700_ON_THE_RIGHT,
        CHINESE_ESCAPE_0800_MADELYN_IN_SIGHT,
        CHINESE_ESCAPE_0900_KILL_THOSE_BASTARDS,
        CHINESE_ESCAPE_1000_LAST_OF_THEM,


        JUNKYARD_RETURN_0100,
        JUNKYARD_RETURN_0200,
        JUNKYARD_RETURN_0250,
        JUNKYARD_RETURN_0300,
        JUNKYARD_RETURN_0400,
        JUNKYARD_RETURN_0500,
        JUNKYARD_RETURN_0600,
        JUNKYARD_RETURN_0700,
        JUNKYARD_RETURN_0800,
        JUNKYARD_RETURN_0900,
        JUNKYARD_RETURN_1000,
        JUNKYARD_RETURN_1100,
        JUNKYARD_RETURN_1200,
        JUNKYARD_RETURN_1300,
        JUNKYARD_RETURN_1400,
        RACING_CHALLENGER_0100_FRONT01,
        RACING_CHALLENGER_0200_FRONT02,
        RACING_CHALLENGER_0300_FRONT03,
        RACING_CHALLENGER_0400_FRONT04,
        RACING_CHALLENGER_0500_FRONT05,
        RACING_CHALLENGER_0600_BEHIND01,
        RACING_CHALLENGER_0700_BEHIND02,
        RACING_CHALLENGER_0800_BEHIND03,
        RACING_CHALLENGER_0900_BEHIND04,
        RACING_CHALLENGER_1000_BEHIND05,


        
        SLAVERBASE_0100_INTRODUCE,
        SLAVERBASE_0200_GENERATORS_DESTROYED,
        SLAVERBASE_0300_BATTERIES_DESTROYED,
        SLAVERBASE_0350_SLAVES_FOUND,
        SLAVERBASE_0400_SLAVES_SAVED,
        SLAVERBASE_0500_DOORS_OPENED,
        SLAVERBASE_0600_MOTHERSHIP_EMPTY,
        SLAVERBASE_0700_PIT_EMPTY,
        SLAVERBASE_0800_ENEMIES_DESTROYED,
        SLAVERBASE_0900_NUKE_HACKED,
        //SLAVERBASE2_1000_PRISON_FOUND,
        SLAVERBASE_1100_FAKE_ESCAPED,


        FORT_VALIANT_A_0100,
        FORT_VALIANT_A_0200,
        FORT_VALIANT_A_0300,

        FORT_VALIANT_B_0100,
        FORT_VALIANT_B_0200,

        FORT_VALIANT_C_0100_BEGIN,
        FORT_VALIANT_C_0100,
        FORT_VALIANT_C_0200,
        FORT_VALIANT_C_0300,
        FORT_VALIANT_C_0400,
        FORT_VALIANT_C_0500,
        FORT_VALIANT_C_0600,
        FORT_VALIANT_C_0700,
        FORT_VALIANT_C_0800,
        FORT_VALIANT_C_0900,
        FORT_VALIANT_C_1000,
        //FORT_VALIANT_C_1100,
        FORT_VALIANT_C_1200,
        FORT_VALIANT_C_1300,
        FORT_VALIANT_C_1400,
        FORT_VALIANT_C_1500,
        FORT_VALIANT_C_1600,
        FORT_VALIANT_C_1700,
        FORT_VALIANT_C_1800,
        FORT_VALIANT_C_1900,
        FORT_VALIANT_C_2000,
        FORT_VALIANT_C_2100,
        FORT_VALIANT_C_2200,
        FORT_VALIANT_C_2300,
        FORT_VALIANT_C_2400,
        FORT_VALIANT_C_2500,
        FORT_VALIANT_C_2600,

        SLAVERBASE2_0100_INTRO,
        SLAVERBASE2_0200_DESTROY_TURRETS,
        SLAVERBASE2_0201_SLAVER_TALK,
        SLAVERBASE2_0300_TURRETS_DESTROYED,
        SLAVERBASE2_0400_FIRST_HUB_DESTROYED,
        SLAVERBASE2_0500_BOTH_HUBS_DESTROYED,
        SLAVERBASE2_0600_GENERATOR_REACHED,
        SLAVERBASE2_0700_GENERATOR_DESTROYED,
        SLAVERBASE2_0800_FIRST_CELL_UNLOCKED,
        SLAVERBASE2_0900_SECOND_CELL_UNLOCKED,
        SLAVERBASE2_1000_THIRD_CELL_UNLOCKED,
        SLAVERBASE2_1100_FOURTH_CELL_UNLOCKED,
        SLAVERBASE2_1200_MISSION_COMPLETE,


        RIME_0100_INTRODUCTION,
        RIME_0150_HEAD_TO_REEF,
        RIME_0200_REEF_REACHED,
        RIME_0300_TALK_TO_REEF,
        //RIME_0400_ON_THE_WAY,
        RIME_0500_LISTEN_TO_SUSPICIOUS,
        RIME_0600_CLIENTS_TALK,
        RIME_0700_DUPLEX_BOUNCER,
        RIME_0800_CONTACT_APPEARS,
        RIME_0900_FOLLOW_INSTRUCTION,
        RIME_1000_FACTORY_FOUND,
        //RIME_1100_WE_GOT_YOUR_BACK,
        RIME_1100_GRAB_THE_ALCOHOL,
        RIME_1200_GET_TO_THE_VESSEL,
        RIME_1300_ON_THE_WAY_TO_VESSEL,
        RIME_1400_HANDLE,
        RIME_1500_WAIT_FOR_THE_SIGNAL,
        RIME_1600_THIS_IS_OUR_CHANCE,
        RIME_1650_PLACE,
        RIME_1700_HURRY_UP,
        RIME_1800_CARGO_PLANTED,
        RIME_1900_BACK_TO_THE_REEF,
        RIME_2000_REEF_TALK,
        RIME_2100_HE_IS_GONE,
        RIME_2200_HE_SPOTTED_US,

        RESEARCH_VESSEL_0100_INTRO,
        RESEARCH_VESSEL_0200_INCOMINGSHIPS,
        RESEARCH_VESSEL_0250_YOUASKEDFORIT,
        RESEARCH_VESSEL_0300_FORTIFIED,
        RESEARCH_VESSEL_0400_LOOKATTHIS,
        RESEARCH_VESSEL_0500_FIRSTPARTS,
        RESEARCH_VESSEL_0600_SECONDPARTS,
        RESEARCH_VESSEL_0700_THIRDPARTS,
        RESEARCH_VESSEL_0800_FORCEFIELDDOWN,
        RESEARCH_VESSEL_0900_FOURTHPARTS,
        RESEARCH_VESSEL_1000_FIRSTHUB,
        RESEARCH_VESSEL_1100_SECONDHUB,
        RESEARCH_VESSEL_1200_THIRDHUB,

        EAC_AMBUSH_0100_INTRO,
        EAC_AMBUSH_0200_MANJEET,
        EAC_AMBUSH_0300_GUYS_HURRY_UP,
        EAC_AMBUSH_0400_MARCUS_TO_EAC,
        EAC_AMBUSH_0500_ONE_LITTLE_ISSUE,
        EAC_AMBUSH_0700_SPLIT_TO_DESTROY_GENERATORS,
        EAC_AMBUSH_0800,
        EAC_AMBUSH_0900,
        EAC_AMBUSH_1000,
        EAC_AMBUSH_1100,
        EAC_AMBUSH_1200_1300,
        EAC_AMBUSH_1400,
        EAC_AMBUSH_1500,
        EAC_AMBUSH_1600,
        EAC_AMBUSH_1650,
        EAC_AMBUSH_1700,



        RIFT_0050_INTRO,
        RIFT_0100_INTRO2,
        RIFT_0200_STATION,
        RIFT_0300_TOURISTS,
        RIFT_0400_SHOPPINGDONE,
        RIFT_0500_ENTERINGRIFT,
        RIFT_0600_MINING,
        RIFT_0700_MINING_COLOR,
        RIFT_0800_MINING_TUNE,
        RIFT_0900_MINING_TUNE_2,
        RIFT_1000_MINING_DONE,
        RIFT_1100_LEAVING,


        BARTHS_MOON_PLANT_0100,
        BARTHS_MOON_PLANT_0200,
        BARTHS_MOON_PLANT_0300,
        BARTHS_MOON_PLANT_0400,
        BARTHS_MOON_PLANT_0500,
        BARTHS_MOON_PLANT_0600,
        BARTHS_MOON_PLANT_0700,
        BARTHS_MOON_PLANT_0800,
        BARTHS_MOON_PLANT_0900,
        BARTHS_MOON_PLANT_1000,
        BARTHS_MOON_PLANT_1100,
        BARTHS_MOON_PLANT_1200,



        CHINESE_TRANSMITTER_0100_INTRODUCE,
        CHINESE_TRANSMITTER_0200_CARGO_BAY,
        CHINESE_TRANSMITTER_0300_CARGO_BAY_2,
        CHINESE_TRANSMITTER_0400_SECURITY_ROOM = 903,
        CHINESE_TRANSMITTER_0500_GENERATOR_DESTROYED,
        CHINESE_TRANSMITTER_0600_CIC_FOUND,
        CHINESE_TRANSMITTER_0700_HUB_HACKED,
        CHINESE_TRANSMITTER_0800_TRANSMITTER_PLACED,
        CHINESE_TRANSMITTER_0900_ESCAPE_2,
        CHINESE_TRANSMITTER_1000_MISSION_COMPLETE,


        RUSSIAN_TRANSMITTER_0100_INTRO,
        RUSSIAN_TRANSMITTER_0200_BACKDOOR,
        RUSSIAN_TRANSMITTER_0300_FINDHUB,
        RUSSIAN_TRANSMITTER_0400_HUBFOUND,
        RUSSIAN_TRANSMITTER_0500_0600_HACKPROBLEM_STRANGERCALLS,
        RUSSIAN_TRANSMITTER_0700_STRANGERPROPOSAL,
        RUSSIAN_TRANSMITTER_0700_VOLODIAINTRO,
        RUSSIAN_TRANSMITTER_0800_NEARCARGO,
        RUSSIAN_TRANSMITTER_0900_VOLODIA_RANT,
        RUSSIAN_TRANSMITTER_1000_VOLODIA_FOUND,
        RUSSIAN_TRANSMITTER_1100_MADELYNSCARED,
        RUSSIAN_TRANSMITTER_1300_APOLLOSCARED,
        RUSSIAN_TRANSMITTER_1400_RETREAT,
        RUSSIAN_TRANSMITTER_1500_IFITDOESNOTWORK,
        RUSSIAN_TRANSMITTER_1600_ITSWORKING,
        RUSSIAN_TRANSMITTER_1700_UPLOADINGSIGNAL,
        RUSSIAN_TRANSMITTER_1800_BADROUTE,
        RUSSIAN_TRANSMITTER_1900_PLACEDEVICE,
        RUSSIAN_TRANSMITTER_2000_DEVICEWORKING,
        RUSSIAN_TRANSMITTER_2100_WEMADEIT,
        RUSSIAN_TRANSMITTER_2200_THOMASCHAT,

        REICHSTAG_A_0100_INTRODUCTION,
        REICHSTAG_A_0200_ON_THE_WAY,
        REICHSTAG_A_0300_REACHING_REICHSTAG,
        REICHSTAG_A_0400_OFFICER_DIALOGUE,
        REICHSTAG_A_0500_ON_THE_WAY_BACK,


        WHITEWOLVES_RESEARCH_0100_COLLECT,
        WHITEWOLVES_RESEARCH_0200_ENTER,
        WHITEWOLVES_RESEARCH_0300_COLLECT2,
        WHITEWOLVES_RESEARCH_0350_SICKEN,
        WHITEWOLVES_RESEARCH_0400_FIND,
        WHITEWOLVES_RESEARCH_0500_GET_OUT,
        WHITEWOLVES_RESEARCH_0550_GET_OUT_SUCCESS,
        WHITEWOLVES_RESEARCH_0600_DESTROY,
        WHITEWOLVES_RESEARCH_0700_RETURN,


   

        REICHSTAG_C_0100_OFFICER_TALK,
        REICHSTAG_C_0200_ON_THE_WAY,
        REICHSTAG_C_0300_REACHING_SHIPYARD,
        REICHSTAG_C_0400_SUPPLY_OFFICER,
        REICHSTAG_C_0500_REACHING_SHIPS,
        REICHSTAG_C_0600_SHIPS_PICKUPED,
        REICHSTAG_C_0700_SHIPYARD_SHOP,
        REICHSTAG_C_0800_SHOPPING_FINISHED,
        REICHSTAG_C_0900_TRANSPORTER_REACHED,


        TWIN_TOWERS_0100_INTRO,
        TWIN_TOWERS_0200_PLACE_EXPLOSIVES,
        TWIN_TOWERS_0300_player_reaching_the_main_electricity_supply,
        TWIN_TOWERS_0400_reaching_a_hangar_with_unmanned_enemy_small_ships,
        TWIN_TOWERS_0500_reaching_electricity_distribution_HUB,
        TWIN_TOWERS_0600_after_all_the_sabotages_are_done,
        TWIN_TOWERS_0700_Meeting_point,
        TWIN_TOWERS_0800_command_center_cleared,
        TWIN_TOWERS_0900_hacker_reaches_computer,
        TWIN_TOWERS_1000_through_the_fight,
        TWIN_TOWERS_1100_hacking_gets_jammed,
        TWIN_TOWERS_1200_killing_jammer,
        TWIN_TOWERS_1300_hacking_done,
        TWIN_TOWERS_1400_clearing_first_control_room,
        TWIN_TOWERS_1500_clearing_second_control_room,
        TWIN_TOWERS_1600_in_tower_B,
        TWIN_TOWERS_1700_reactor_shut_down,
        TWIN_TOWERS_1800_computer_hacked,
        TWIN_TOWERS_1900_motherships_arrived,
        TWIN_TOWERS_2000_destroying_the_generator,
        TWIN_TOWERS_2100_destroying_batteries,





        EAC_PRISON_0100,
        EAC_PRISON_0200,
        EAC_PRISON_0300,
        EAC_PRISON_0400,
        EAC_PRISON_0500,
        EAC_PRISON_0600,
        EAC_PRISON_0700,
        EAC_PRISON_0800,
        EAC_PRISON_0900,
        EAC_PRISON_1000,
        EAC_PRISON_1100,
        EAC_PRISON_1200,
        EAC_PRISON_1250,
        EAC_PRISON_1300,
        EAC_PRISON_1400,
        EAC_PRISON_1500,
        EAC_PRISON_1600,



        EAC_TRANSMITTER_0100,
        EAC_TRANSMITTER_0200,
        EAC_TRANSMITTER_0300,
        EAC_TRANSMITTER_0400,
        EAC_TRANSMITTER_0500,
        EAC_TRANSMITTER_0600,
        EAC_TRANSMITTER_0700,
        EAC_TRANSMITTER_0800,
        EAC_TRANSMITTER_0900,
        EAC_TRANSMITTER_1000,
        EAC_TRANSMITTER_1100,
        EAC_TRANSMITTER_1200,
        EAC_TRANSMITTER_1300,
        EAC_TRANSMITTER_1400,




        ALIEN_GATE_0100,
        ALIEN_GATE_0200,
        ALIEN_GATE_0300,
        ALIEN_GATE_0400,
        ALIEN_GATE_0500,
        ALIEN_GATE_0600,
        ALIEN_GATE_0700,
        ALIEN_GATE_0800,
        ALIEN_GATE_0800B,
        ALIEN_GATE_0900,
        ALIEN_GATE_1000,
        ALIEN_GATE_1100,
        ALIEN_GATE_1200,
        ALIEN_GATE_1300,
        ALIEN_GATE_1400,
        ALIEN_GATE_1500,
        ALIEN_GATE_1600,
        ALIEN_GATE_1700,
        ALIEN_GATE_1800,
        ALIEN_GATE_1900,
        ALIEN_GATE_2000,
        //ALIEN_GATE_2100,
        ALIEN_GATE_2200,
        ALIEN_GATE_2300,
        ALIEN_GATE_2400,
        ALIEN_GATE_2500,
        ALIEN_GATE_2600,
        ALIEN_GATE_2700,
        ALIEN_GATE_2800,
        ALIEN_GATE_2900,
        ALIEN_GATE_3000,
        //ALIEN_GATE_3100,
        ALIEN_GATE_3200,
        ALIEN_GATE_3300,
    }

    static class MyDialogueConstants
    {
        public const int MAX_SENTENCE_LENGTH = 120;

        static Dictionary<int, MyDialogue> m_dialogues = new Dictionary<int, MyDialogue>();

        public static MyDialogue GetDialogue(MyDialogueEnum id)
        {
            MyDialogue result = null;
            m_dialogues.TryGetValue((int)id, out result);
            return result;
        }

        static void Validate()
        {
            // has every enum its added dialogue?
            foreach (var o in Enum.GetValues(typeof (MyDialogueEnum)))
            {
                var id = (o as MyDialogueEnum?).Value;
                var dialogue = GetDialogue(id);
                MyCommonDebugUtils.AssertDebug(
                    dialogue != null, string.Format("Dialogue with id={0} wasn't added!", (int) id));
            }

            // has every added dialogue its enum?
            foreach (var dialogue in m_dialogues)
            {
                MyCommonDebugUtils.AssertDebug(
                    Enum.IsDefined(typeof (MyDialogueEnum), dialogue.Key),
                    string.Format("Dialogue with id={0} doesn't have a corresponding enum value!", dialogue.Key));
            }

            // are the texts short enough?
            foreach (var dialogue in m_dialogues)
            {
                if (dialogue.Key != (int) MyDialogueEnum.TEST)
                {
                    foreach (var sentence in dialogue.Value.Sentences)
                    {
                        // TODO: we should try rendering it instead
                        MyCommonDebugUtils.AssertDebug(
                            MyDialoguesWrapper.Get(sentence.Text).Length <= MAX_SENTENCE_LENGTH,
                            string.Format(
                                "Sentence too long in dialogue id={0} (length = {2} > {3}): \"{1}\"",
                                dialogue.Key,
                                MyDialoguesWrapper.Get(sentence.Text),
                                MyDialoguesWrapper.Get(sentence.Text).Length,
                                MAX_SENTENCE_LENGTH));
                    }
                }
            }
        }

        static MyDialogueConstants()
        {
            MyMwcLog.WriteLine("MyDialogueConstants()");

            m_dialogues.Add((int)MyDialogueEnum.NONE, new MyDialogue(
                new MyDialogueSentence[] { },
                MyDialoguePriorityEnum.LOW
            ));

            m_dialogues.Add((int)MyDialogueEnum.TEST, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, null, MyDialoguesWrapperEnum.Dlg_Test01, noise: 0.5f),
                new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_Test02, noise: 0.25f),
                new MyDialogueSentence(MyActorEnum.APOLLO, null, MyDialoguesWrapperEnum.Dlg_Test03),
            }
            ));

            #region EAC Survey Site
            /*
            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0100_BEGINING, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0001, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0100),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0002, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0101),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0003, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0102),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0004, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0103),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0005, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0104),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0006, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0105),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0007, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0106),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0008, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0107),

            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0150_BORINGCHAT, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0009, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1101),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0010, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1102),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0011, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1103),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0012_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1104),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0012_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1105),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0013, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1106),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0014_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1107),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0014_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1108),
            }
            ));
            */
            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0200_ACTIONSTARTS, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.EacSurveySite_StationOperator, MySoundCuesEnum.Dlg_EACSurveySite_0015, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0200),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0016, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0201),
                new MyDialogueSentence(MyActorEnum.EacSurveySite_StationOperator, MySoundCuesEnum.Dlg_EACSurveySite_0017_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0202),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0205_ACTION, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.EacSurveySite_StationOperator, MySoundCuesEnum.Dlg_EACSurveySite_0017_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0202),
                new MyDialogueSentence(MyActorEnum.EacSurveySite_MilitaryOfficer, MySoundCuesEnum.Dlg_EACSurveySite_0018_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0203),
                new MyDialogueSentence(MyActorEnum.EacSurveySite_MilitaryOfficer, MySoundCuesEnum.Dlg_EACSurveySite_0018_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0204),
                new MyDialogueSentence(MyActorEnum.EacSurveySite_MilitaryOfficer, MySoundCuesEnum.Dlg_EACSurveySite_0018_03, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0205),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0019, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0206),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0020, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0207),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0021, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0208),
                new MyDialogueSentence(MyActorEnum.EacSurveySite_StationOperator, MySoundCuesEnum.Dlg_EACSurveySite_0022, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0209),
                new MyDialogueSentence(MyActorEnum.EacSurveySite_MilitaryCaptain, MySoundCuesEnum.Dlg_EACSurveySite_0023, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0210),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0024_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0211),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0024_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0212),
                new MyDialogueSentence(MyActorEnum.EacSurveySite_MilitaryCaptain, MySoundCuesEnum.Dlg_EACSurveySite_0025_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0213), //Marcus, dont engage..
                new MyDialogueSentence(MyActorEnum.EacSurveySite_MilitaryCaptain, MySoundCuesEnum.Dlg_EACSurveySite_1003, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0213b), //We are overhelmed
                new MyDialogueSentence(MyActorEnum.EacSurveySite_MilitaryCaptain, MySoundCuesEnum.Dlg_EACSurveySite_0025_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0214), //Make your way
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0026, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0215),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0027_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0216),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0027_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0217),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0028, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0218),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0300_CIVILIANSDESTROYED, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0029, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0301),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_1004, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0301b),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0030, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0302),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0400_BARRICADE, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0031, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0401),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0450_MOTHERSHIPS, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0034, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0601),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0035, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0602),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0036, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0603),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0037, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0604),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0038, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0605),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0500_TOBASE, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0032_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0501),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0032_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0502),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0033, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0503),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0550_COMMANDOFFLINE, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0039, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0901),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0040_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0902),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0040_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0903),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0041, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0904),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0560_GENERATORUP, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0042, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1001),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0043, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1002),  //Looks like it worked..
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0044, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1003), //Well, we could
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_1000, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1003b), //Too bad
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_1001, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1004), //Thats Traders..
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_1002, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1005), //Check if...
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0700_MADELYN, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0045, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0701),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0046, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0702),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0047, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0703),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0048, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0704),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0049_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0705),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0049_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0706),
                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0050, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0707),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0051, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0708),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0052, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0709),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0053_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0710),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0053_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0711),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0705_MADELYN_2, new MyDialogue(
                new MyDialogueSentence[] {
                //Removed sentences about turrets and stupid guys
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0053_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0710),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0053_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0711),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0053_03, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0712),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0750_NEAREND, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0065, MyDialoguesWrapperEnum.Dlg_EACSurveySite_2101),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0066, MyDialoguesWrapperEnum.Dlg_EACSurveySite_2102),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_0800_PICKUP, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0067, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0801),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0068, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0802),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0069, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0803),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0069_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0804),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EACSurveySite_0069_03, MyDialoguesWrapperEnum.Dlg_EACSurveySite_0805),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_1000_RESEARCHERS_1, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.Researcher1, MySoundCuesEnum.Dlg_EACSurveySite_0054, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1201),
                new MyDialogueSentence(MyActorEnum.Researcher2, MySoundCuesEnum.Dlg_EACSurveySite_0055_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1202),
                new MyDialogueSentence(MyActorEnum.Researcher2, MySoundCuesEnum.Dlg_EACSurveySite_0055_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1203),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0056, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1204),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_1100_RESEARCHERS_2, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.Researcher2, MySoundCuesEnum.Dlg_EACSurveySite_0057, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1301),
                new MyDialogueSentence(MyActorEnum.Researcher1, MySoundCuesEnum.Dlg_EACSurveySite_0058, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1302),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0059, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1303),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0060_01, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1304),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0060_02, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1305),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EACSurveySite_0061, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1306),
                new MyDialogueSentence(MyActorEnum.Researcher1, MySoundCuesEnum.Dlg_EACSurveySite_0062, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1307),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EACSurveySite_0063, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1308),
                new MyDialogueSentence(MyActorEnum.Researcher2, MySoundCuesEnum.Dlg_EACSurveySite_0064, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1309),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_1200_RUSSIANCHAT_01, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RussianGeneral, MySoundCuesEnum.Dlg_EACSurveySite_0070, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1401, noise: 0.9f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_1300_RUSSIANCHAT_02, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RussianGeneral, MySoundCuesEnum.Dlg_EACSurveySite_0071, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1501, noise: 0.8f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_1400_RUSSIANCHAT_03, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RussianGeneral, MySoundCuesEnum.Dlg_EACSurveySite_0073, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1601, noise: 0.9f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_1500_RUSSIANCHAT_04, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RussianCaptain, MySoundCuesEnum.Dlg_EACSurveySite_0076, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1701, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RussianGeneral, MySoundCuesEnum.Dlg_EACSurveySite_0077, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1702, noise: 0.9f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_1600_RUSSIANCHAT_05, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RussianGeneral, MySoundCuesEnum.Dlg_EACSurveySite_0078, MyDialoguesWrapperEnum.Dlg_EACSurveySite_1801, noise: 0.9f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_1900_RUSSIANCHAT_06, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RussianCaptain, MySoundCuesEnum.Dlg_EACSurveySite_0074, MyDialoguesWrapperEnum.Dlg_EACSurveySite_2201, noise: 0.9f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_2000_RUSSIANCHAT_07, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RussianGeneral, MySoundCuesEnum.Dlg_EACSurveySite_0075, MyDialoguesWrapperEnum.Dlg_EACSurveySite_2301, noise: 0.9f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_SURVEY_SITE_2100_RUSSIANCHAT_08, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RussianGeneral, MySoundCuesEnum.Dlg_EACSurveySite_0072, MyDialoguesWrapperEnum.Dlg_EACSurveySite_2401, noise: 0.9f),
            }
            ));
            #endregion

            #region Russian Warehouse
            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0100, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1000, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0101),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1001, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0102),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1002, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0103),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1003, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0104),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1004, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0105),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1005, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0106),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1006, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0107),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0200, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1007, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0201),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1008, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0202),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1009, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0203),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1010, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0204),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1011, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0205),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1012, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0206),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1013, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0207),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1014, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0208),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1015, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0209),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0300, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1016, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0301),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1017, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0302),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1018, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0303),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1019, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0304),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1020, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0305),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0400, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1021, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0401),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0500, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1022, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0501),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1023, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0502),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0600, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1024, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0601),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1025, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0602),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1026, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0603),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1027, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0604),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1028, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0605),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0700, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1029, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0701),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1030, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0702),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1031, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0703),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1032, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0704),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0800, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1033, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0801),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1034, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0802),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1035, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0803),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1036, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0804),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1037, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0805, pauseBefore_ms: 3000),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1038, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0806),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1039, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0807),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1040, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0808/*, pauseBefore_ms: 300*/),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1041, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0809),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1042, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0810),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1043, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0811),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1044, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0812),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1045, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0813),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_0900, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1046, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0901),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1047, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0902),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1048, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_0903),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_1000, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1049, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1001),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1050, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1002),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianWarehouse_1051, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1003),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1052, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1004),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_WAREHOUSE_1100, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1053, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1101),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1054, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1102),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1055, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1103),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1056, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1104),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1057, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1105),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_RussianWarehouse_1058, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1106),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1059, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1107),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1060, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1108),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianWarehouse_1061, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1109),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RussianWarehouse_1062, MyDialoguesWrapperEnum.Dlg_RussianWarehouse_1110),
                }
            ));

            #endregion

            #region Laika

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0100_ARRIVAL, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1000, MyDialoguesWrapperEnum.Dlg_Laika_0100),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1001, MyDialoguesWrapperEnum.Dlg_Laika_0102),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1002, MyDialoguesWrapperEnum.Dlg_Laika_0103),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1003, MyDialoguesWrapperEnum.Dlg_Laika_0104),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1004, MyDialoguesWrapperEnum.Dlg_Laika_0105),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1005, MyDialoguesWrapperEnum.Dlg_Laika_0106),
            }
            ));
                                                
            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0200_HANGAR, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1006, MyDialoguesWrapperEnum.Dlg_Laika_0200),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1007, MyDialoguesWrapperEnum.Dlg_Laika_0201),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1008, MyDialoguesWrapperEnum.Dlg_Laika_0202),
                    
            }
            ));                                   

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0250_WAYTOHANGAR, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1009, MyDialoguesWrapperEnum.Dlg_Laika_1501),
                new MyDialogueSentence(MyActorEnum.LAIKA_OPERATOR, MySoundCuesEnum.Dlg_Laika_1010, MyDialoguesWrapperEnum.Dlg_Laika_1502),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1011, MyDialoguesWrapperEnum.Dlg_Laika_1503),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1012, MyDialoguesWrapperEnum.Dlg_Laika_1504),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1013, MyDialoguesWrapperEnum.Dlg_Laika_1505),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1014, MyDialoguesWrapperEnum.Dlg_Laika_1506),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1015, MyDialoguesWrapperEnum.Dlg_Laika_1507),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1016, MyDialoguesWrapperEnum.Dlg_Laika_1508),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0275_INSIDEHANGAR, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.LAIKA_OPERATOR, MySoundCuesEnum.Dlg_Laika_1017, MyDialoguesWrapperEnum.Dlg_Laika_1601),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0277_DESTROYGENERATOR, new MyDialogue(
    new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1018, MyDialoguesWrapperEnum.Dlg_Laika_1607),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1019, MyDialoguesWrapperEnum.Dlg_Laika_1608),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1020, MyDialoguesWrapperEnum.Dlg_Laika_1602),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1021, MyDialoguesWrapperEnum.Dlg_Laika_1603),
                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1022, MyDialoguesWrapperEnum.Dlg_Laika_1604),
                new MyDialogueSentence(MyActorEnum.LAIKA_OPERATOR, MySoundCuesEnum.Dlg_Laika_1023, MyDialoguesWrapperEnum.Dlg_Laika_1605),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1024, MyDialoguesWrapperEnum.Dlg_Laika_1606),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0300_AFTERHANGAR, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1025, MyDialoguesWrapperEnum.Dlg_Laika_0300),
                //new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_Laika_0301),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1026, MyDialoguesWrapperEnum.Dlg_Laika_0302),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1027, MyDialoguesWrapperEnum.Dlg_Laika_0303),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1028, MyDialoguesWrapperEnum.Dlg_Laika_0303a),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1029, MyDialoguesWrapperEnum.Dlg_Laika_0303b),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1030, MyDialoguesWrapperEnum.Dlg_Laika_0303c),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1031, MyDialoguesWrapperEnum.Dlg_Laika_0304),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1032, MyDialoguesWrapperEnum.Dlg_Laika_0304a),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1033, MyDialoguesWrapperEnum.Dlg_Laika_0305),
              //  new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1034, MyDialoguesWrapperEnum.Dlg_Laika_0306),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1035, MyDialoguesWrapperEnum.Dlg_Laika_0307),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1036, MyDialoguesWrapperEnum.Dlg_Laika_0308),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0350_GPS, new MyDialogue(
              new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1037, MyDialoguesWrapperEnum.Dlg_Laika_0309),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1038, MyDialoguesWrapperEnum.Dlg_Laika_0310),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1039, MyDialoguesWrapperEnum.Dlg_Laika_0311),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1040, MyDialoguesWrapperEnum.Dlg_Laika_0312),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1041, MyDialoguesWrapperEnum.Dlg_Laika_0313),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1042, MyDialoguesWrapperEnum.Dlg_Laika_0314),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1043, MyDialoguesWrapperEnum.Dlg_Laika_0314a),
                //new MyDialogueSentence(MyActorEnum.MADELYN, null, MyDialoguesWrapperEnum.Dlg_Laika_0315),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1044, MyDialoguesWrapperEnum.Dlg_Laika_0316),
            }
          ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0400_MARKETPLACE, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1045, MyDialoguesWrapperEnum.Dlg_Laika_0400),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1046, MyDialoguesWrapperEnum.Dlg_Laika_0401),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0480_COMMANDARRIVE, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1047, MyDialoguesWrapperEnum.Dlg_Laika_1701),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0500_COMMAND, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1048, MyDialoguesWrapperEnum.Dlg_Laika_0500),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1049, MyDialoguesWrapperEnum.Dlg_Laika_0501),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1050, MyDialoguesWrapperEnum.Dlg_Laika_0502),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1051, MyDialoguesWrapperEnum.Dlg_Laika_0503),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1052, MyDialoguesWrapperEnum.Dlg_Laika_0504),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1053, MyDialoguesWrapperEnum.Dlg_Laika_0505),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0600_TOPIPE, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1054, MyDialoguesWrapperEnum.Dlg_Laika_0600),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1055, MyDialoguesWrapperEnum.Dlg_Laika_0601),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1082, MyDialoguesWrapperEnum.Dlg_Laika_0602),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0700_LEFTHUB, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1056, MyDialoguesWrapperEnum.Dlg_Laika_0700),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1057, MyDialoguesWrapperEnum.Dlg_Laika_0701),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1058, MyDialoguesWrapperEnum.Dlg_Laika_0702),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1059, MyDialoguesWrapperEnum.Dlg_Laika_0703),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1060, MyDialoguesWrapperEnum.Dlg_Laika_0704),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1061, MyDialoguesWrapperEnum.Dlg_Laika_0705),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0700_LEFTHUB2, new MyDialogue(
              new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1062, MyDialoguesWrapperEnum.Dlg_Laika_0706),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1063, MyDialoguesWrapperEnum.Dlg_Laika_0707),

                new MyDialogueSentence(MyActorEnum.RussianGeneralRecording, MySoundCuesEnum.Dlg_Laika_1064, MyDialoguesWrapperEnum.Dlg_Laika_0708, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RussianGeneralRecording, MySoundCuesEnum.Dlg_Laika_1065, MyDialoguesWrapperEnum.Dlg_Laika_0709, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RussianGeneralRecording, MySoundCuesEnum.Dlg_Laika_1066, MyDialoguesWrapperEnum.Dlg_Laika_0710, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RussianGeneralRecording, MySoundCuesEnum.Dlg_Laika_1067, MyDialoguesWrapperEnum.Dlg_Laika_0711, noise: 0.9f),

                new MyDialogueSentence(MyActorEnum.RussianHeadquartersRecording, MySoundCuesEnum.Dlg_Laika_1068, MyDialoguesWrapperEnum.Dlg_Laika_0712, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RussianHeadquartersRecording, MySoundCuesEnum.Dlg_Laika_1069, MyDialoguesWrapperEnum.Dlg_Laika_0712a, noise: 0.9f/*, pauseBefore_ms: 200*/),
                new MyDialogueSentence(MyActorEnum.RussianHeadquartersRecording, MySoundCuesEnum.Dlg_Laika_1070, MyDialoguesWrapperEnum.Dlg_Laika_0713, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RussianHeadquartersRecording, MySoundCuesEnum.Dlg_Laika_1071, MyDialoguesWrapperEnum.Dlg_Laika_0714, noise: 0.9f),

                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1072, MyDialoguesWrapperEnum.Dlg_Laika_0715),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1073, MyDialoguesWrapperEnum.Dlg_Laika_0716),
                
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1074, MyDialoguesWrapperEnum.Dlg_Laika_0717),


                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1075, MyDialoguesWrapperEnum.Dlg_Laika_0718),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1076, MyDialoguesWrapperEnum.Dlg_Laika_0719),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1077, MyDialoguesWrapperEnum.Dlg_Laika_0720),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1078, MyDialoguesWrapperEnum.Dlg_Laika_0721),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1079, MyDialoguesWrapperEnum.Dlg_Laika_0722),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1080, MyDialoguesWrapperEnum.Dlg_Laika_0723),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1081, MyDialoguesWrapperEnum.Dlg_Laika_0724),
                
            }
          ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0720_AFTERLEFTHUB, new MyDialogue(
                new MyDialogueSentence[] {
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1082, MyDialoguesWrapperEnum.Dlg_Laika_0723),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1083, MyDialoguesWrapperEnum.Dlg_Laika_0724),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0800_RIGHTHUB, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1084, MyDialoguesWrapperEnum.Dlg_Laika_0800),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1085, MyDialoguesWrapperEnum.Dlg_Laika_0801),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1086, MyDialoguesWrapperEnum.Dlg_Laika_0802),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1087, MyDialoguesWrapperEnum.Dlg_Laika_0803),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1088, MyDialoguesWrapperEnum.Dlg_Laika_0804),
                //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1089, MyDialoguesWrapperEnum.Dlg_Laika_0805),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1090, MyDialoguesWrapperEnum.Dlg_Laika_0806),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1091, MyDialoguesWrapperEnum.Dlg_Laika_0807),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1092, MyDialoguesWrapperEnum.Dlg_Laika_0808),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1093, MyDialoguesWrapperEnum.Dlg_Laika_0809),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1094, MyDialoguesWrapperEnum.Dlg_Laika_0810),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1095, MyDialoguesWrapperEnum.Dlg_Laika_0811),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1096, MyDialoguesWrapperEnum.Dlg_Laika_0812),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0900_WARHEAD, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1097, MyDialoguesWrapperEnum.Dlg_Laika_0900),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1098, MyDialoguesWrapperEnum.Dlg_Laika_0901),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1099, MyDialoguesWrapperEnum.Dlg_Laika_0902),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1100, MyDialoguesWrapperEnum.Dlg_Laika_0903),
           }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_0950_WARHEAD_HACK, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1101, MyDialoguesWrapperEnum.Dlg_Laika_0950),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_1800_WARHEAD_DONE, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1102, MyDialoguesWrapperEnum.Dlg_Laika_1800),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1103, MyDialoguesWrapperEnum.Dlg_Laika_1801),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_1800_WARHEAD_DONE2, new MyDialogue(
               new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1104, MyDialoguesWrapperEnum.Dlg_Laika_1802),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1105, MyDialoguesWrapperEnum.Dlg_Laika_1803),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1106, MyDialoguesWrapperEnum.Dlg_Laika_1804, pauseBefore_ms: 1500),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1107, MyDialoguesWrapperEnum.Dlg_Laika_1805),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1108, MyDialoguesWrapperEnum.Dlg_Laika_1805a),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1109, MyDialoguesWrapperEnum.Dlg_Laika_1806),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1110, MyDialoguesWrapperEnum.Dlg_Laika_1807),
                new MyDialogueSentence(MyActorEnum.RUSSIAN_GENERAL, MySoundCuesEnum.Dlg_Laika_1126, MyDialoguesWrapperEnum.Dlg_Laika_1913),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Laika_1111, MyDialoguesWrapperEnum.Dlg_Laika_1808, pauseBefore_ms: 1500),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_Laika_1112, MyDialoguesWrapperEnum.Dlg_Laika_1809),
            }
           ));

            m_dialogues.Add((int)MyDialogueEnum.LAIKA_1900_ESCAPED, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1113, MyDialoguesWrapperEnum.Dlg_Laika_1900),
                new MyDialogueSentence(MyActorEnum.MARCUS,  MySoundCuesEnum.Dlg_Laika_1114, MyDialoguesWrapperEnum.Dlg_Laika_1901),
                new MyDialogueSentence(MyActorEnum.APOLLO,  MySoundCuesEnum.Dlg_Laika_1115, MyDialoguesWrapperEnum.Dlg_Laika_1902),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1116, MyDialoguesWrapperEnum.Dlg_Laika_1903),
                new MyDialogueSentence(MyActorEnum.MARCUS,  MySoundCuesEnum.Dlg_Laika_1117, MyDialoguesWrapperEnum.Dlg_Laika_1904),
                new MyDialogueSentence(MyActorEnum.MARCUS,  MySoundCuesEnum.Dlg_Laika_1118, MyDialoguesWrapperEnum.Dlg_Laika_1905),
                new MyDialogueSentence(MyActorEnum.APOLLO,  MySoundCuesEnum.Dlg_Laika_1119, MyDialoguesWrapperEnum.Dlg_Laika_1906),
                new MyDialogueSentence(MyActorEnum.APOLLO,  MySoundCuesEnum.Dlg_Laika_1120, MyDialoguesWrapperEnum.Dlg_Laika_1907),
                new MyDialogueSentence(MyActorEnum.APOLLO,  MySoundCuesEnum.Dlg_Laika_1121, MyDialoguesWrapperEnum.Dlg_Laika_1908),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1122, MyDialoguesWrapperEnum.Dlg_Laika_1909),
                new MyDialogueSentence(MyActorEnum.APOLLO,  MySoundCuesEnum.Dlg_Laika_1123, MyDialoguesWrapperEnum.Dlg_Laika_1910),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1124, MyDialoguesWrapperEnum.Dlg_Laika_1911),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Laika_1125, MyDialoguesWrapperEnum.Dlg_Laika_1912),
            }
));



            /*
  m_dialogues.Add((int)MyDialogueEnum.LAIKA_0905_STRESS01, new MyDialogue(
      new MyDialogueSentence[] {
      new MyDialogueSentence(MyActorEnum.MADELYN, null, MyDialoguesWrapperEnum.Dlg_Laika_1000),
      new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_Laika_1001),
  }
  ));

  m_dialogues.Add((int)MyDialogueEnum.LAIKA_0910_STRESS02, new MyDialogue(
      new MyDialogueSentence[] {
      new MyDialogueSentence(MyActorEnum.MADELYN, null, MyDialoguesWrapperEnum.Dlg_Laika_1100),
  }
  ));

  m_dialogues.Add((int)MyDialogueEnum.LAIKA_0915_STRESS03, new MyDialogue(
      new MyDialogueSentence[] {
      new MyDialogueSentence(MyActorEnum.MADELYN, null, MyDialoguesWrapperEnum.Dlg_Laika_1200),
  }
  ));

  m_dialogues.Add((int)MyDialogueEnum.LAIKA_0920_STRESS04, new MyDialogue(
      new MyDialogueSentence[] {
      new MyDialogueSentence(MyActorEnum.MADELYN, null, MyDialoguesWrapperEnum.Dlg_Laika_1300),
      new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_Laika_1301),
      new MyDialogueSentence(MyActorEnum.MADELYN, null, MyDialoguesWrapperEnum.Dlg_Laika_1302),
      new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_Laika_1303),
  }    
  ));
             */


            #endregion

            #region Chinese Transmitter
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0100_INTRODUCE, new MyDialogue(
                new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1000, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1001, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_02),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1002, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_03),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1003, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1004, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_05),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1005, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_06),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1006, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_07),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1007, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_08),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1008, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_09),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1009, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_10),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1010, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_11),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1011, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_12),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1012, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_13),
new MyDialogueSentence(MyActorEnum.CHINESE_OFFICER, MySoundCuesEnum.Dlg_ChineseTransmitter_1013, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_14),
new MyDialogueSentence(MyActorEnum.CHINESE_OFFICER, MySoundCuesEnum.Dlg_ChineseTransmitter_1014, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_15),
new MyDialogueSentence(MyActorEnum.CHINESE_OFFICER, MySoundCuesEnum.Dlg_ChineseTransmitter_1015, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_16),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1016, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_17),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1017, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_18),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ChineseTransmitter_1018, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_19),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1019, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_20),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1020, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_21),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1021, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_22),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1022, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_01_23),

            }
                ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0200_CARGO_BAY, new MyDialogue(
                new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1023, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_02_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1024, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_02_02),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1025, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_02_03),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1026, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_02_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1027, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_02_05),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1028, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_02_06),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1029, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_02_07),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1030, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_02_08),

    }
        ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0300_CARGO_BAY_2, new MyDialogue(
                new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1031, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_03_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1032, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_03_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1033, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_03_03),
    }
        ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0400_SECURITY_ROOM, new MyDialogue(
                new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1034, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_04_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1035, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_04_02),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1036, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_04_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1037, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_04_04),
    }
        ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0500_GENERATOR_DESTROYED, new MyDialogue(
                new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1038, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_05_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1039, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_05_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1040, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_05_03),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1041, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_05_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1042, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_05_05),
    }
        ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0600_CIC_FOUND, new MyDialogue(
            new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1043, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_06_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1044, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_06_02),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1045, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_06_03),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1046, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_06_04),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransmitter_1047, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_06_05),

    }
    ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0700_HUB_HACKED, new MyDialogue(
            new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1048, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_07_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1049, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_07_02),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1050, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_07_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1051, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_07_04),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1052, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_07_05),

    }
    ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0800_TRANSMITTER_PLACED, new MyDialogue(
                        new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1053, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_08_01),

    }
                ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_0900_ESCAPE_2, new MyDialogue(
                        new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1054, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1055, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1056, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_03),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1057, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1058, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_05),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1059, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_06),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1060, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_07),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1061, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_08),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransmitter_1062, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_09),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1063, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_09_10),

    }
                ));
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSMITTER_1000_MISSION_COMPLETE, new MyDialogue(
                        new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1064, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_10_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1065, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_10_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1066, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_10_03),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ChineseTransmitter_1067, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_10_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransmitter_1068, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_10_05),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseTransmitter_1069, MyDialoguesWrapperEnum.Dlg_ChineseTransmitter_10_06),

    }
                ));
            #endregion

            #region Slaver base

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0100_INTRODUCE,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1000, MyDialoguesWrapperEnum.Dlg_SlaverBase_01_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1001, MyDialoguesWrapperEnum.Dlg_SlaverBase_01_02),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1002, MyDialoguesWrapperEnum.Dlg_SlaverBase_01_03),
//new MyDialogueSentence(MyActorEnum.SLAVER_LEADER, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1003, MyDialoguesWrapperEnum.Dlg_SlaverBase_01_04),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1004, MyDialoguesWrapperEnum.Dlg_SlaverBase_01_05),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1005, MyDialoguesWrapperEnum.Dlg_SlaverBase_01_06),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1006, MyDialoguesWrapperEnum.Dlg_SlaverBase_01_07),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0200_GENERATORS_DESTROYED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1007, MyDialoguesWrapperEnum.Dlg_SlaverBase_02_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1008, MyDialoguesWrapperEnum.Dlg_SlaverBase_02_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1009, MyDialoguesWrapperEnum.Dlg_SlaverBase_02_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0300_BATTERIES_DESTROYED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1010, MyDialoguesWrapperEnum.Dlg_SlaverBase_03_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1011, MyDialoguesWrapperEnum.Dlg_SlaverBase_03_02),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1012, MyDialoguesWrapperEnum.Dlg_SlaverBase_03_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0350_SLAVES_FOUND,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1013, MyDialoguesWrapperEnum.Dlg_SlaverBase_04_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1014, MyDialoguesWrapperEnum.Dlg_SlaverBase_04_02),

                        }
                    ));

            /*m_dialogues.Add((int)MyDialogueEnum.SLAVERBASE2_1000_PRISON_FOUND, new MyDialogue(
                new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.Madelyn, null, MyDialoguesWrapperEnum.Dlg_SlaverBase2_0301),
                }
                ));*/

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0400_SLAVES_SAVED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.CAPTIVE, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1015, MyDialoguesWrapperEnum.Dlg_SlaverBase_05_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1016, MyDialoguesWrapperEnum.Dlg_SlaverBase_05_02),
new MyDialogueSentence(MyActorEnum.CAPTIVE, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1017, MyDialoguesWrapperEnum.Dlg_SlaverBase_05_03),
new MyDialogueSentence(MyActorEnum.CAPTIVE, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1018, MyDialoguesWrapperEnum.Dlg_SlaverBase_05_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1019, MyDialoguesWrapperEnum.Dlg_SlaverBase_05_05),
new MyDialogueSentence(MyActorEnum.CAPTIVE, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1020, MyDialoguesWrapperEnum.Dlg_SlaverBase_05_06),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1021, MyDialoguesWrapperEnum.Dlg_SlaverBase_05_07),


                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0500_DOORS_OPENED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1022, MyDialoguesWrapperEnum.Dlg_SlaverBase_06_01),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0600_MOTHERSHIP_EMPTY,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.CAPTIVE, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1023, MyDialoguesWrapperEnum.Dlg_SlaverBase_07_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1024, MyDialoguesWrapperEnum.Dlg_SlaverBase_07_02),
new MyDialogueSentence(MyActorEnum.CAPTIVE, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1025, MyDialoguesWrapperEnum.Dlg_SlaverBase_07_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1026, MyDialoguesWrapperEnum.Dlg_SlaverBase_07_04),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0700_PIT_EMPTY,
                new MyDialogue(
                    new[]
                        {
//new MyDialogueSentence(MyActorEnum.CAPTIVE_PILOT, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1027, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_01),
new MyDialogueSentence(MyActorEnum.CAPTIVE_PILOT, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1028, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_02),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1029, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1030, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1031, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_05),
new MyDialogueSentence(MyActorEnum.CAPTIVE_PILOT, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1032, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_06),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1033, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_07),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1034, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_08),
new MyDialogueSentence(MyActorEnum.CAPTIVE_PILOT, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1035, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_09),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1036, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_10),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1037, MyDialoguesWrapperEnum.Dlg_SlaverBase_08_11),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0800_ENEMIES_DESTROYED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.CAPTIVE_PILOT, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1038, MyDialoguesWrapperEnum.Dlg_SlaverBase_09_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1039, MyDialoguesWrapperEnum.Dlg_SlaverBase_09_02),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_1100_FAKE_ESCAPED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1040, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_01),
new MyDialogueSentence(MyActorEnum.CAPTIVE_PILOT, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1041, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_02),
new MyDialogueSentence(MyActorEnum.CAPTIVE_PILOT, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1042, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_03),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1043, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_04),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1044, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_05),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1045, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_06),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1046, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_07),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1047, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_08),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1048, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_09),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1049, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_10),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1050, MyDialoguesWrapperEnum.Dlg_SlaverBase_10_11),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE_0900_NUKE_HACKED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1051, MyDialoguesWrapperEnum.Dlg_SlaverBase_11_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBaseWipeout_1052, MyDialoguesWrapperEnum.Dlg_SlaverBase_11_02),

                        }
                    ));

            #endregion

            #region Slaver base 2

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0100_INTRO,
                new MyDialogue(
                    new[]
                        {
                            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1000, MyDialoguesWrapperEnum.Dlg_SlaverBase2_01_01),
                            new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1001, MyDialoguesWrapperEnum.Dlg_SlaverBase2_01_02),
                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0200_DESTROY_TURRETS,
                new MyDialogue(
                    new[]
                        {
                            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1002, MyDialoguesWrapperEnum.Dlg_SlaverBase2_02_01),
                            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBase2_1003, MyDialoguesWrapperEnum.Dlg_SlaverBase2_02_02),
                            new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_SlaverBase2_1004, MyDialoguesWrapperEnum.Dlg_SlaverBase2_02_03),
                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0201_SLAVER_TALK,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.SLAVER_BASE_CAPTAIN, MySoundCuesEnum.Dlg_SlaverBase2_1005, MyDialoguesWrapperEnum.Dlg_SlaverBase2_03_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1006, MyDialoguesWrapperEnum.Dlg_SlaverBase2_03_02),
new MyDialogueSentence(MyActorEnum.SLAVER_BASE_CAPTAIN, MySoundCuesEnum.Dlg_SlaverBase2_1007, MyDialoguesWrapperEnum.Dlg_SlaverBase2_03_03),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1008, MyDialoguesWrapperEnum.Dlg_SlaverBase2_03_04),
new MyDialogueSentence(MyActorEnum.SLAVER_BASE_CAPTAIN, MySoundCuesEnum.Dlg_SlaverBase2_1009, MyDialoguesWrapperEnum.Dlg_SlaverBase2_03_05),
                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0300_TURRETS_DESTROYED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1010, MyDialoguesWrapperEnum.Dlg_SlaverBase2_04_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBase2_1011, MyDialoguesWrapperEnum.Dlg_SlaverBase2_04_02),
                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0400_FIRST_HUB_DESTROYED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1012, MyDialoguesWrapperEnum.Dlg_SlaverBase2_05_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0500_BOTH_HUBS_DESTROYED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBase2_1013, MyDialoguesWrapperEnum.Dlg_SlaverBase2_06_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBase2_1014, MyDialoguesWrapperEnum.Dlg_SlaverBase2_06_02),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBase2_1015, MyDialoguesWrapperEnum.Dlg_SlaverBase2_06_03),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1016, MyDialoguesWrapperEnum.Dlg_SlaverBase2_06_04),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_SlaverBase2_1017, MyDialoguesWrapperEnum.Dlg_SlaverBase2_06_05),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1018, MyDialoguesWrapperEnum.Dlg_SlaverBase2_06_06),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0600_GENERATOR_REACHED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1019, MyDialoguesWrapperEnum.Dlg_SlaverBase2_07_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBase2_1020, MyDialoguesWrapperEnum.Dlg_SlaverBase2_07_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1021, MyDialoguesWrapperEnum.Dlg_SlaverBase2_07_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0700_GENERATOR_DESTROYED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1022, MyDialoguesWrapperEnum.Dlg_SlaverBase2_08_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1023, MyDialoguesWrapperEnum.Dlg_SlaverBase2_08_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBase2_1024, MyDialoguesWrapperEnum.Dlg_SlaverBase2_08_03),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1025, MyDialoguesWrapperEnum.Dlg_SlaverBase2_08_04),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1026, MyDialoguesWrapperEnum.Dlg_SlaverBase2_08_05),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1027, MyDialoguesWrapperEnum.Dlg_SlaverBase2_08_06),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0800_FIRST_CELL_UNLOCKED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBase2_1028, MyDialoguesWrapperEnum.Dlg_SlaverBase2_09_01),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_0900_SECOND_CELL_UNLOCKED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1029, MyDialoguesWrapperEnum.Dlg_SlaverBase2_10_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1030, MyDialoguesWrapperEnum.Dlg_SlaverBase2_10_02),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1031, MyDialoguesWrapperEnum.Dlg_SlaverBase2_10_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_1000_THIRD_CELL_UNLOCKED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_SlaverBase2_1032, MyDialoguesWrapperEnum.Dlg_SlaverBase2_11_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1033, MyDialoguesWrapperEnum.Dlg_SlaverBase2_11_02),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_1100_FOURTH_CELL_UNLOCKED,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBase2_1034, MyDialoguesWrapperEnum.Dlg_SlaverBase2_12_01),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.SLAVERBASE2_1200_MISSION_COMPLETE,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_SlaverBase2_1035, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_SlaverBase2_1036, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1037, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_03),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1038, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_04),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1039, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_05),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1040, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_06),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1041, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_07),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1042, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_08),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_SlaverBase2_1043, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_09),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_SlaverBase2_1044, MyDialoguesWrapperEnum.Dlg_SlaverBase2_13_10),

                        }
                    ));

            #endregion

            #region Twin Towers - Doppelburg

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0100_INTRO,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.TRANSPORTER_CAPTAIN, MySoundCuesEnum.Dlg_TwinTowers_1000, MyDialoguesWrapperEnum.Dlg_TwinTowers_01_01),
new MyDialogueSentence(MyActorEnum.TRANSPORTER_CAPTAIN, MySoundCuesEnum.Dlg_TwinTowers_1001, MyDialoguesWrapperEnum.Dlg_TwinTowers_01_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1002, MyDialoguesWrapperEnum.Dlg_TwinTowers_01_03),
//new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1003, MyDialoguesWrapperEnum.Dlg_TwinTowers_01_04),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1004, MyDialoguesWrapperEnum.Dlg_TwinTowers_02_01, pauseBefore_ms:1500),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1005, MyDialoguesWrapperEnum.Dlg_TwinTowers_02_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1006, MyDialoguesWrapperEnum.Dlg_TwinTowers_02_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1007, MyDialoguesWrapperEnum.Dlg_TwinTowers_02_04),
                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0200_PLACE_EXPLOSIVES,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1008, MyDialoguesWrapperEnum.Dlg_TwinTowers_02_05),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1009, MyDialoguesWrapperEnum.Dlg_TwinTowers_02_06),
                        }
                    ));


            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0300_player_reaching_the_main_electricity_supply,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1011, MyDialoguesWrapperEnum.Dlg_TwinTowers_03_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1012, MyDialoguesWrapperEnum.Dlg_TwinTowers_03_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1013, MyDialoguesWrapperEnum.Dlg_TwinTowers_03_03),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1014, MyDialoguesWrapperEnum.Dlg_TwinTowers_03_04),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0400_reaching_a_hangar_with_unmanned_enemy_small_ships,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1015, MyDialoguesWrapperEnum.Dlg_TwinTowers_04_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1016, MyDialoguesWrapperEnum.Dlg_TwinTowers_04_02),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0500_reaching_electricity_distribution_HUB,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1017, MyDialoguesWrapperEnum.Dlg_TwinTowers_05_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1018, MyDialoguesWrapperEnum.Dlg_TwinTowers_05_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1019, MyDialoguesWrapperEnum.Dlg_TwinTowers_05_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0600_after_all_the_sabotages_are_done,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1020, MyDialoguesWrapperEnum.Dlg_TwinTowers_06_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1021, MyDialoguesWrapperEnum.Dlg_TwinTowers_06_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1022, MyDialoguesWrapperEnum.Dlg_TwinTowers_06_03),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1023, MyDialoguesWrapperEnum.Dlg_TwinTowers_06_04),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1024, MyDialoguesWrapperEnum.Dlg_TwinTowers_06_05),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1025, MyDialoguesWrapperEnum.Dlg_TwinTowers_06_06),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1026, MyDialoguesWrapperEnum.Dlg_TwinTowers_06_07),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1027, MyDialoguesWrapperEnum.Dlg_TwinTowers_06_08),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0700_Meeting_point,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1028, MyDialoguesWrapperEnum.Dlg_TwinTowers_07_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1029, MyDialoguesWrapperEnum.Dlg_TwinTowers_07_02),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_TwinTowers_1030, MyDialoguesWrapperEnum.Dlg_TwinTowers_07_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0800_command_center_cleared,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1031, MyDialoguesWrapperEnum.Dlg_TwinTowers_08_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1032, MyDialoguesWrapperEnum.Dlg_TwinTowers_08_02),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_0900_hacker_reaches_computer,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1033, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1034, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1035, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_03),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1036, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_04),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1037, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_05),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1038, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_06),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1039, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_07),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1040, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_08),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_TwinTowers_1041, MyDialoguesWrapperEnum.Dlg_TwinTowers_09_09),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1000_through_the_fight,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1042, MyDialoguesWrapperEnum.Dlg_TwinTowers_10_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1043, MyDialoguesWrapperEnum.Dlg_TwinTowers_10_02),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1100_hacking_gets_jammed,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1044, MyDialoguesWrapperEnum.Dlg_TwinTowers_11_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1045, MyDialoguesWrapperEnum.Dlg_TwinTowers_11_02),
//new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1046, MyDialoguesWrapperEnum.Dlg_TwinTowers_11_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1047, MyDialoguesWrapperEnum.Dlg_TwinTowers_11_04),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1048, MyDialoguesWrapperEnum.Dlg_TwinTowers_11_05),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1049, MyDialoguesWrapperEnum.Dlg_TwinTowers_11_06),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1050, MyDialoguesWrapperEnum.Dlg_TwinTowers_11_07),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_TwinTowers_1051, MyDialoguesWrapperEnum.Dlg_TwinTowers_11_08),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1200_killing_jammer,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1052, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1053, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1054, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_03),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1055, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1056, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_05),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1057, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_06),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1058, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_07),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1059, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_08),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1060, MyDialoguesWrapperEnum.Dlg_TwinTowers_12_09),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1300_hacking_done,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1061, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1062, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1063, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1064, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_04),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1065, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_05),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1066, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_06),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1067, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_07),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1068, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_08),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1069, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_09),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1070, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_10),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1071, MyDialoguesWrapperEnum.Dlg_TwinTowers_13_11),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1400_clearing_first_control_room,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1072, MyDialoguesWrapperEnum.Dlg_TwinTowers_14_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1073, MyDialoguesWrapperEnum.Dlg_TwinTowers_14_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1074, MyDialoguesWrapperEnum.Dlg_TwinTowers_14_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1075, MyDialoguesWrapperEnum.Dlg_TwinTowers_14_04),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1500_clearing_second_control_room,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1076, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1077, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_02),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1078, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_03),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1079, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1080, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_05),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1081, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_06),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1082, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_07),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1083, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_08),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1084, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_09),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1085, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_10),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1086, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_11),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1087, MyDialoguesWrapperEnum.Dlg_TwinTowers_15_12),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1600_in_tower_B,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1088, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1089, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_02),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1090, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_03),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1091, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_04),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1092, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_05),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1093, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_06),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1094, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_07),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1095, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_08),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1096, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_09),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_TwinTowers_1097, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_10),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1098, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_11),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_TwinTowers_1099, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_12),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1100, MyDialoguesWrapperEnum.Dlg_TwinTowers_16_13),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1700_reactor_shut_down,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1101, MyDialoguesWrapperEnum.Dlg_TwinTowers_17_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1102, MyDialoguesWrapperEnum.Dlg_TwinTowers_17_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1103, MyDialoguesWrapperEnum.Dlg_TwinTowers_17_03),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1104, MyDialoguesWrapperEnum.Dlg_TwinTowers_17_04),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1105, MyDialoguesWrapperEnum.Dlg_TwinTowers_17_05),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1106, MyDialoguesWrapperEnum.Dlg_TwinTowers_17_06),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1107, MyDialoguesWrapperEnum.Dlg_TwinTowers_17_07),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1108, MyDialoguesWrapperEnum.Dlg_TwinTowers_17_08),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1800_computer_hacked,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1109, MyDialoguesWrapperEnum.Dlg_TwinTowers_18_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1110, MyDialoguesWrapperEnum.Dlg_TwinTowers_18_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1111, MyDialoguesWrapperEnum.Dlg_TwinTowers_18_03),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1112, MyDialoguesWrapperEnum.Dlg_TwinTowers_18_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1113, MyDialoguesWrapperEnum.Dlg_TwinTowers_18_05),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1114, MyDialoguesWrapperEnum.Dlg_TwinTowers_18_06),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_1900_motherships_arrived,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.FOR_CAPTAIN, MySoundCuesEnum.Dlg_TwinTowers_1115, MyDialoguesWrapperEnum.Dlg_TwinTowers_19_01),
new MyDialogueSentence(MyActorEnum.FOR_CAPTAIN, MySoundCuesEnum.Dlg_TwinTowers_1116, MyDialoguesWrapperEnum.Dlg_TwinTowers_19_02),
new MyDialogueSentence(MyActorEnum.FOR_CAPTAIN, MySoundCuesEnum.Dlg_TwinTowers_1117, MyDialoguesWrapperEnum.Dlg_TwinTowers_19_03),
new MyDialogueSentence(MyActorEnum.FOR_CAPTAIN, MySoundCuesEnum.Dlg_TwinTowers_1118, MyDialoguesWrapperEnum.Dlg_TwinTowers_19_04),
new MyDialogueSentence(MyActorEnum.FOR_CAPTAIN, MySoundCuesEnum.Dlg_TwinTowers_1119, MyDialoguesWrapperEnum.Dlg_TwinTowers_19_05),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1120, MyDialoguesWrapperEnum.Dlg_TwinTowers_19_06),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1121, MyDialoguesWrapperEnum.Dlg_TwinTowers_19_07),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1122, MyDialoguesWrapperEnum.Dlg_TwinTowers_19_08),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_2000_destroying_the_generator,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1123, MyDialoguesWrapperEnum.Dlg_TwinTowers_20_01),
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1124, MyDialoguesWrapperEnum.Dlg_TwinTowers_20_02),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.TWIN_TOWERS_2100_destroying_batteries,
                new MyDialogue(
                    new[]
                        {
new MyDialogueSentence(MyActorEnum.ERHARD , MySoundCuesEnum.Dlg_TwinTowers_1125, MyDialoguesWrapperEnum.Dlg_TwinTowers_21_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_TwinTowers_1126, MyDialoguesWrapperEnum.Dlg_TwinTowers_21_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1127, MyDialoguesWrapperEnum.Dlg_TwinTowers_21_03),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_TwinTowers_1128, MyDialoguesWrapperEnum.Dlg_TwinTowers_21_04),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_TwinTowers_1129, MyDialoguesWrapperEnum.Dlg_TwinTowers_21_05),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_TwinTowers_1130, MyDialoguesWrapperEnum.Dlg_TwinTowers_21_06),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_TwinTowers_1131, MyDialoguesWrapperEnum.Dlg_TwinTowers_21_07),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_TwinTowers_1132, MyDialoguesWrapperEnum.Dlg_TwinTowers_21_08),

                        }
                    ));

            #endregion

            #region Junkyard Convince

            m_dialogues.Add(
                (int)MyDialogueEnum.JUNKYARD_CONVINCE_0100_INTRODUCE,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1000, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_01_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1001, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_01_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1002, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_01_03),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1003, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_01_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1004, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_01_05),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1005, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_01_06),

new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1006, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_02_01, pauseBefore_ms:3000),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1007, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_02_02),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1008, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_02_03),

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_0200_INFORMATOR,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1009, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_01),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1010, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1011, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_03),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1012, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_04),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1013, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_05),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1014, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_06),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1015, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_07),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1016, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_08),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1017, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_09),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1018, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_10),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1019, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_11),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1020, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_12),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1021, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_13),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1022, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_14),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1023, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_15),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1024, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_16),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1025, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_03_17),

                        }
                    ));


            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_0300_RUN,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1026, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_04_01),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1027, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_04_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1028, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_04_03),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1029, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_04_04),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1030, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_04_05),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1031, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_04_06),

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_0500_CATCHED,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1032, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1034, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_02),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1035, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_03),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1036, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_04),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1037, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_05),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1038, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_06),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1039, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_07),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1040, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_08),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1041, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_05_09),

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_0600_BEFORE_FIGHT,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1042, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1043, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1044, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_03),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1045, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_04),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1046, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_05),

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_0650_MET_ZAPPA_GUARD,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1047, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_06),
new MyDialogueSentence(MyActorEnum.ZAPPAS_GANGMAN, MySoundCuesEnum.Dlg_JunkyardConvince_1048, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_07),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1049, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_08),
new MyDialogueSentence(MyActorEnum.ZAPPAS_GANGMAN, MySoundCuesEnum.Dlg_JunkyardConvince_1050, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_09),
new MyDialogueSentence(MyActorEnum.ZAPPAS_GANGMAN, MySoundCuesEnum.Dlg_JunkyardConvince_1051, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_10),
new MyDialogueSentence(MyActorEnum.ZAPPAS_GANGMAN, MySoundCuesEnum.Dlg_JunkyardConvince_1052, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_11),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1053, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_12),
new MyDialogueSentence(MyActorEnum.ZAPPAS_GANGMAN, MySoundCuesEnum.Dlg_JunkyardConvince_1054, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_06_13),

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_0800_MOMO_ARRIVE,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MOMO_ZAPPA, MySoundCuesEnum.Dlg_JunkyardConvince_1055, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_07_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1056, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_07_02),

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_0900_THE_MOMO,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MOMO_ZAPPA, MySoundCuesEnum.Dlg_JunkyardConvince_1055, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_07_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1056, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_07_02),
                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_0950_MOMO_FIGHT,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MOMO_ZAPPA, MySoundCuesEnum.Dlg_JunkyardConvince_1055, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_07_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1056, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_07_02),
                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_1000_LAST_OF_THEM,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_1100_FIGHT_ENDS,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1057, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1058, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_02),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1059, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_03),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1060, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_04),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1061, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_05),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1062, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_06),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1063, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_07),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1064, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_08),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1065, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_09),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1066, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_10),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1067, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_11),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1068, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_12),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1069, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_13),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1070, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_14),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1071, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_15),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1072, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_16),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1073, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_17),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1074, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_18),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1075, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_19),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1076, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_20),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1077, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_21),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1078, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_22),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1079, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_23),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1080, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_24),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1081, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_25),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1082, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_26),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1083, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_27),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1084, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_28),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1085, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_29),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1086, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_30),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1087, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_31),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1088, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_32),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1089, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_33),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1090, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_34),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1091, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_35),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1092, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_36),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1093, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_37),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1094, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_38),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1095, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_39),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1096, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_40),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1097, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_41),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1098, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_42),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1099, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_43),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1100, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_44),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1101, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_45),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1102, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_46),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardConvince_1103, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_47),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1104, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_48),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1105, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_08_49),
                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_1200_FLY_TO_DEALER,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {

                        }
                    ));
            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_1300_BOMB_DEALER,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.DEALER, MySoundCuesEnum.Dlg_JunkyardConvince_1106, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1107, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_02),
new MyDialogueSentence(MyActorEnum.DEALER, MySoundCuesEnum.Dlg_JunkyardConvince_1108, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_03),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1109, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_04),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1110, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_05),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1111, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_06),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1112, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_07),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1113, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_08),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1114, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_09),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1115, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_09_10),

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_1400_ARRIVED_AT_MARCUS,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1116, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1117, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1118, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_03),
new MyDialogueSentence(MyActorEnum.GANGSTER, MySoundCuesEnum.Dlg_JunkyardConvince_1119, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_04),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardConvince_1120, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_05),
new MyDialogueSentence(MyActorEnum.GANGSTER, MySoundCuesEnum.Dlg_JunkyardConvince_1121, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_06),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardConvince_1122, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_07),
new MyDialogueSentence(MyActorEnum.GANGSTER, MySoundCuesEnum.Dlg_JunkyardConvince_1123, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_08),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardConvince_1124, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_09),
new MyDialogueSentence(MyActorEnum.GANGSTER, MySoundCuesEnum.Dlg_JunkyardConvince_1125, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_10),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardConvince_1126, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_11),
new MyDialogueSentence(MyActorEnum.GANGSTER, MySoundCuesEnum.Dlg_JunkyardConvince_1127, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_10_12),

                        }
                    ));

            m_dialogues.Add(
                (int) MyDialogueEnum.JUNKYARD_CONVINCE_1500_GANGSTER_FIGHT_STARTED,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1128, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_11_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1129, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_11_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1130, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_11_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.JUNKYARD_CONVINCE_1600_GO_TO_MS,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardConvince_1131, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1132, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_02),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardConvince_1133, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_03),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1134, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_04),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1135, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_05),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardConvince_1136, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_06),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1137, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_07),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardConvince_1138, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_08),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardConvince_1139, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_09),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1140, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_10),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1141, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_11),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1142, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_12),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardConvince_1143, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_12_13),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.JUNKYARD_CONVINCE_1700_FINALE,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1144, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_13_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1145, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_13_02),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1146, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_13_03),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardConvince_1147, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_13_04),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardConvince_1148, MyDialoguesWrapperEnum.Dlg_JunkyardConvince_13_05),

                        }
                    ));

            #endregion

            #region Chinese Transport

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0100_INTRODUCE,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1000, MyDialoguesWrapperEnum.Dlg_ChineseTransport_01_01),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseTransport_1001, MyDialoguesWrapperEnum.Dlg_ChineseTransport_01_02),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseTransport_1002, MyDialoguesWrapperEnum.Dlg_ChineseTransport_01_03),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseTransport_1003, MyDialoguesWrapperEnum.Dlg_ChineseTransport_01_04),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1004, MyDialoguesWrapperEnum.Dlg_ChineseTransport_01_05),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1005, MyDialoguesWrapperEnum.Dlg_ChineseTransport_01_06),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ChineseTransport_1006, MyDialoguesWrapperEnum.Dlg_ChineseTransport_01_07),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0200_FIRST_DEVICE_HACKED,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1007, MyDialoguesWrapperEnum.Dlg_ChineseTransport_02_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1008, MyDialoguesWrapperEnum.Dlg_ChineseTransport_02_02),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1009, MyDialoguesWrapperEnum.Dlg_ChineseTransport_02_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0300_SHOOTING_ON_ME,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1010, MyDialoguesWrapperEnum.Dlg_ChineseTransport_03_01),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0400_THEY_FOUND_ME,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1011, MyDialoguesWrapperEnum.Dlg_ChineseTransport_04_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1012, MyDialoguesWrapperEnum.Dlg_ChineseTransport_04_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1013, MyDialoguesWrapperEnum.Dlg_ChineseTransport_04_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0500_MARCUS_IS_HERE,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1014, MyDialoguesWrapperEnum.Dlg_ChineseTransport_05_01),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0600_DESTROY_THE_TRANSMITTER,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1015, MyDialoguesWrapperEnum.Dlg_ChineseTransport_06_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1016, MyDialoguesWrapperEnum.Dlg_ChineseTransport_06_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1017, MyDialoguesWrapperEnum.Dlg_ChineseTransport_06_03),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0650_MARCUS_IS_LEAVING,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransport_1018, MyDialoguesWrapperEnum.Dlg_ChineseTransport_07_01),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0670_disabling_the_terminals
,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1019, MyDialoguesWrapperEnum.Dlg_ChineseTransport_08_01),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_0700_RUN,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1020, MyDialoguesWrapperEnum.Dlg_ChineseTransport_09_01),

                        }
                    ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSPORT_0800_GO_TO_SECOND_BASE, new MyDialogue(
            new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1023, MyDialoguesWrapperEnum.Dlg_ChineseTransport_11_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1024, MyDialoguesWrapperEnum.Dlg_ChineseTransport_11_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1025, MyDialoguesWrapperEnum.Dlg_ChineseTransport_11_03),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransport_1026, MyDialoguesWrapperEnum.Dlg_ChineseTransport_11_04),

            }
            ));


            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSPORT_0850_INSIDE_TUNNEL, new MyDialogue(
            new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1027, MyDialoguesWrapperEnum.Dlg_ChineseTransport_12_01),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransport_1028, MyDialoguesWrapperEnum.Dlg_ChineseTransport_12_02),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ChineseTransport_1029, MyDialoguesWrapperEnum.Dlg_ChineseTransport_12_03),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ChineseTransport_1030, MyDialoguesWrapperEnum.Dlg_ChineseTransport_12_04),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ChineseTransport_1031, MyDialoguesWrapperEnum.Dlg_ChineseTransport_12_05),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransport_1032, MyDialoguesWrapperEnum.Dlg_ChineseTransport_12_06),
 
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSPORT_0900_SURRENDER, new MyDialogue(
            new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ChineseTransport_1033, MyDialoguesWrapperEnum.Dlg_ChineseTransport_13_01),

            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSPORT_1000_DOOR_BLOCKED, new MyDialogue(
            new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseTransport_1034, MyDialoguesWrapperEnum.Dlg_ChineseTransport_14_01),

            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSPORT_1400_DOORS_UNLOCKED, new MyDialogue(
            new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1035, MyDialoguesWrapperEnum.Dlg_ChineseTransport_15_01),

            }
            ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_1050_REACHED_COMPUTER,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1036, MyDialoguesWrapperEnum.Dlg_ChineseTransport_16_01),

                        }
                    ));

            m_dialogues.Add(
                (int)MyDialogueEnum.CHINESE_TRANSPORT_1075_HACKED_COMPUTER,
                new MyDialogue(
                    new MyDialogueSentence[]
                        {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1037, MyDialoguesWrapperEnum.Dlg_ChineseTransport_17_01),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseTransport_1038, MyDialoguesWrapperEnum.Dlg_ChineseTransport_17_02),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1039, MyDialoguesWrapperEnum.Dlg_ChineseTransport_17_03),

                        }
                    ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSPORT_1100_HELP_MARCUS, new MyDialogue(
            new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1040, MyDialoguesWrapperEnum.Dlg_ChineseTransport_18_01),

            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSPORT_1200_GENERAL_ARRIVAL, new MyDialogue(
            new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.CHINESE_COMMANDO, MySoundCuesEnum.Dlg_ChineseTransport_1041, MyDialoguesWrapperEnum.Dlg_ChineseTransport_19_01),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1042, MyDialoguesWrapperEnum.Dlg_ChineseTransport_19_02),

            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_TRANSPORT_1300_LAND_IN, new MyDialogue(
            new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1043, MyDialoguesWrapperEnum.Dlg_ChineseTransport_20_01),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseTransport_1044, MyDialoguesWrapperEnum.Dlg_ChineseTransport_20_02),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseTransport_1045, MyDialoguesWrapperEnum.Dlg_ChineseTransport_20_03),

            }
            ));

            #endregion

            #region Chinese Refinery

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0100_GO_CLOSER, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseRafinery_1000, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1000),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseRafinery_1001, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1001),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseRafinery_1002, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1002),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1003, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1003),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0150_GET_INSIDE, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseRafinery_1004, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1004),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0200_LABORATORY, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseRafinery_1005, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1005),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1006, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1006),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseRafinery_1007, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1007),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0300_DEACTIVATE_BOMB, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1008, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1008),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1009, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1009),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1010, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1010),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1011, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1011),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1012, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1012),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1013, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1013),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1014, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1014),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1015, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1015),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1016, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1016),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0400_GO_TO_SECOND_ASTEROID, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1017, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1017),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1018, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1018),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1019, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1019),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1020, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1020),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1021, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1021),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1022, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1022),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0500_FIND_THE_COMPUTER, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1023, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1023),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1024, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1024),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1025, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1025),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1026, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1026),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1027, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1027),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1028, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1028),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1029, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1029),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1030, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1030),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1031, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1031),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1032, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1032),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0600_GO_TO_THIRD_ASTEROID, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1033, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1033),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1034, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1034),
                new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1035, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1035),
           }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0700_SNEAK_INSIDE_THE_STATION, new MyDialogue(
            new MyDialogueSentence[] {
               new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_ChineseRafinery_1036, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1036),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseRafinery_1037, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1037),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0800_FIND_THE_OLD_PATH, new MyDialogue(
            new MyDialogueSentence[] {
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_0900_HACK_THE_COMPUTER, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ChineseRafinery_1038, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1038),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_REFINERY_1000_GET_OUT, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseRafinery_1039, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1039),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseRafinery_1040, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1040),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseRafinery_1041, MyDialoguesWrapperEnum.Dlg_ChineseRefinery_1041),
            }
            ));

            #endregion

            #region Cover Cargoship Retreat
            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0100_INTRODUCTION, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1000, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1000),
                new MyDialogueSentence(MyActorEnum.CHINESE_PILOT, MySoundCuesEnum.Dlg_ChineseEscape_1001, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1001, noise: 0.9f, pauseBefore_ms: 2000),
                new MyDialogueSentence(MyActorEnum.CHINESE_PILOT, MySoundCuesEnum.Dlg_ChineseEscape_1002, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1002, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseEscape_1003, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1003, pauseBefore_ms: 2000),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1004, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1004),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseEscape_1005, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1005),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1006, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1006),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1007, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1007),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseEscape_1008, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1008),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1009, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1009),
                new MyDialogueSentence(MyActorEnum.CHINESE_PILOT, MySoundCuesEnum.Dlg_ChineseEscape_1010, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1010, noise: 0.9f, pauseBefore_ms: 1000),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1011, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1011),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0200_IT_IS_TRAP, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.CHINESE_PILOT, MySoundCuesEnum.Dlg_ChineseEscape_1012, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1012, noise: 0.9f, pauseBefore_ms: 2000),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1013, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1013, noise: 0.9f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0300_ON_THIRD, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.CHINESE_PILOT, MySoundCuesEnum.Dlg_ChineseEscape_1014, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1014, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1015, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1015),
                new MyDialogueSentence(MyActorEnum.CHINESE_PILOT, MySoundCuesEnum.Dlg_ChineseEscape_1016, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1016, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.CHINESE_PILOT, MySoundCuesEnum.Dlg_ChineseEscape_1017, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1017, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.CHINESE_PILOT, MySoundCuesEnum.Dlg_ChineseEscape_1018, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1018, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseEscape_1019, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1019),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1020, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1020),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1021, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1021),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0400_ON_NINE, new MyDialogue(
            new MyDialogueSentence[] {
                //new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_ChineseEscape_04_01),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0500_WATCH_BACK, new MyDialogue(
            new MyDialogueSentence[] {
                //new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_ChineseEscape_05_01),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0600_WATCH_FRONT, new MyDialogue(
            new MyDialogueSentence[] {
                //new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_ChineseEscape_06_01),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0700_ON_THE_RIGHT, new MyDialogue(
            new MyDialogueSentence[] {
                //new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_ChineseEscape_07_01),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0800_MADELYN_IN_SIGHT, new MyDialogue(
            new MyDialogueSentence[] {   
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseEscape_1022, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1022),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1023, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1023),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseEscape_1024, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1024),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ChineseEscape_1025, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1025),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_0900_KILL_THOSE_BASTARDS, new MyDialogue(
            new MyDialogueSentence[] {
                //new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_ChineseEscape_09_01),
                //new MyDialogueSentence(MyActorEnum.MARCUS, null, MyDialoguesWrapperEnum.Dlg_DialoguePlaceholder),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.CHINESE_ESCAPE_1000_LAST_OF_THEM, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ChineseEscape_1026, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1026),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ChineseEscape_1027, MyDialoguesWrapperEnum.Dlg_ChineseEscape_1027),
            }
            ));

            #endregion
            
            #region Junkyard Race
            


            m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0100, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1000, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0100, listener: MyActorEnum.APOLLO),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1001, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0101, listener: MyActorEnum.MARCUS),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1002, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0102, listener:MyActorEnum.MARCUS),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1003, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0103, listener:MyActorEnum.APOLLO),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1004, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0104, listener:MyActorEnum.APOLLO),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0200, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1005, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0200, listener: MyActorEnum.APOLLO),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1006, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0201, listener: MyActorEnum.APOLLO),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1007, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0202),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1008, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0203),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1009, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0204),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1010, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0205),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1011, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0206),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1012, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0207),
                                                                                         }                                                                             
                                                                                         ));

            m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0250, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1013, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0208, listener: MyActorEnum.APOLLO),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1014, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0209),
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1015, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0210, listener: MyActorEnum.APOLLO),
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1016, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0211, listener: MyActorEnum.APOLLO),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1017, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0212),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1018, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0213),
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1019, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0214, listener: MyActorEnum.APOLLO),
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1020, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0215, listener: MyActorEnum.APOLLO),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1021, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0216),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1022, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0217),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1023, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0218),
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1024, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0219),
                    new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1025, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0220),

                    }
                ));
            m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0300, new MyDialogue(
                new MyDialogueSentence[]{
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1026, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0300),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1027, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0301),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0400, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1038, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0400),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1039, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0401),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0500, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1040, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0500),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1041, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0501),

                                                                                             }));

m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0600, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1042, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0600),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0700, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1043, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0700),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0800, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1044, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0800),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_0900, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1045, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_0900),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_1000, new MyDialogue(new MyDialogueSentence[] {
        new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1046, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1000),
    }
));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_1100, new MyDialogue(new MyDialogueSentence[]{
        new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1047, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1100),
    }
));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_1200, new MyDialogue(new MyDialogueSentence[] {
        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1048, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1200),
    }
));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_1300, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1049, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1300),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1050, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1301),
new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1051, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1302, listener: MyActorEnum.APOLLO),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1052, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1303),
new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1053, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1304),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1054, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1305),
new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1055, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1306),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardReturn_1056, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1307),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardReturn_1057, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1308),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardReturn_1058, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1309),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardReturn_1059, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1310),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.JUNKYARD_RETURN_1400, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1060, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1400, listener: MyActorEnum.APOLLO),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1061, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1401),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1062, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1402),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardReturn_1063, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1403),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardReturn_1064, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1404),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1065, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1405),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardReturn_1066, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1406),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardReturn_1067, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1407),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1068, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1408),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1069, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1409),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1070, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1410),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1071, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1411),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1072, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1412),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardReturn_1073, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1413),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1074, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1414),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1075, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1415),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_JunkyardReturn_1076, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1416),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1077, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1417),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_JunkyardReturn_1078, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1418),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1079, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1419),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_JunkyardReturn_1080, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1420),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardReturn_1081, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1421),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_JunkyardReturn_1082, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1422),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_JunkyardReturn_1083, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1423),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1084, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1424),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_JunkyardReturn_1085, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1425),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_JunkyardReturn_1086, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1426),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1087, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1427),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardReturn_1088, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1428),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1089, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1429),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardReturn_1090, MyDialoguesWrapperEnum.Dlg_JunkyardReturn_1430),

                                                                                             }
                                                                                         ));



            #endregion
            
            #region Racing Quotes
            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0100_FRONT01, new MyDialogue(
                new MyDialogueSentence[] 
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1028, MyDialoguesWrapperEnum.Dlg_Challenger_0100_FRONT01, 1f),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0200_FRONT02, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1029, MyDialoguesWrapperEnum.Dlg_Challenger_0100_FRONT02, 1f),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0300_FRONT03, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1030, MyDialoguesWrapperEnum.Dlg_Challenger_0100_FRONT03, 1f),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0400_FRONT04, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1031, MyDialoguesWrapperEnum.Dlg_Challenger_0100_FRONT04, 1f),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0500_FRONT05, new MyDialogue(
                new MyDialogueSentence[] 
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1032, MyDialoguesWrapperEnum.Dlg_Challenger_0100_FRONT05, 1f),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0600_BEHIND01, new MyDialogue(
                new MyDialogueSentence[] 
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1033, MyDialoguesWrapperEnum.Dlg_Challenger_0100_BEHIND01, 1f),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0700_BEHIND02, new MyDialogue(
                    new MyDialogueSentence[] 
                    {
                        new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1034, MyDialoguesWrapperEnum.Dlg_Challenger_0100_BEHIND02, 1f),
                    }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0800_BEHIND03, new MyDialogue(
                new MyDialogueSentence[] 
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1035, MyDialoguesWrapperEnum.Dlg_Challenger_0100_BEHIND03, 1f),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_0900_BEHIND04, new MyDialogue(
                new MyDialogueSentence[] 
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1036, MyDialoguesWrapperEnum.Dlg_Challenger_0100_BEHIND04, 1f),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RACING_CHALLENGER_1000_BEHIND05, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.SPEEDSTER, MySoundCuesEnum.Dlg_JunkyardReturn_1037, MyDialoguesWrapperEnum.Dlg_Challenger_0100_BEHIND05, 1f),
                }
            ));
            #endregion

            #region Rime

            m_dialogues.Add((int)MyDialogueEnum.RIME_0100_INTRODUCTION, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1000, MyDialoguesWrapperEnum.Dlg_Rime_0101),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1001, MyDialoguesWrapperEnum.Dlg_Rime_0102),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1002, MyDialoguesWrapperEnum.Dlg_Rime_0103),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1003, MyDialoguesWrapperEnum.Dlg_Rime_0104),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1004, MyDialoguesWrapperEnum.Dlg_Rime_0105),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1005, MyDialoguesWrapperEnum.Dlg_Rime_0106),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1006, MyDialoguesWrapperEnum.Dlg_Rime_0107),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1007, MyDialoguesWrapperEnum.Dlg_Rime_0108),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1008, MyDialoguesWrapperEnum.Dlg_Rime_0109),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1009, MyDialoguesWrapperEnum.Dlg_Rime_0110),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_0150_HEAD_TO_REEF, new MyDialogue(
         new MyDialogueSentence[] {
                                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_RimeConvince_1010, MyDialoguesWrapperEnum.Dlg_Rime_0111),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1011, MyDialoguesWrapperEnum.Dlg_Rime_0112),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1012, MyDialoguesWrapperEnum.Dlg_Rime_0113),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_RimeConvince_1013, MyDialoguesWrapperEnum.Dlg_Rime_0114),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1014, MyDialoguesWrapperEnum.Dlg_Rime_0115),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1015, MyDialoguesWrapperEnum.Dlg_Rime_0116),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1016, MyDialoguesWrapperEnum.Dlg_Rime_0117),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1017, MyDialoguesWrapperEnum.Dlg_Rime_0118),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1018, MyDialoguesWrapperEnum.Dlg_Rime_0119),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1019, MyDialoguesWrapperEnum.Dlg_Rime_0120),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1020, MyDialoguesWrapperEnum.Dlg_Rime_0121),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1021, MyDialoguesWrapperEnum.Dlg_Rime_0122),

            }
         ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_0200_REEF_REACHED, new MyDialogue(
 new MyDialogueSentence[] {       
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1022, MyDialoguesWrapperEnum.Dlg_Rime_0201),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1023, MyDialoguesWrapperEnum.Dlg_Rime_0202),
            }
 ));


            m_dialogues.Add((int)MyDialogueEnum.RIME_0300_TALK_TO_REEF, new MyDialogue(
            new MyDialogueSentence[] {
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1024, MyDialoguesWrapperEnum.Dlg_Rime_0203),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1025, MyDialoguesWrapperEnum.Dlg_Rime_0204),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1026, MyDialoguesWrapperEnum.Dlg_Rime_0205),
        new MyDialogueSentence(MyActorEnum.MARCUS,  MySoundCuesEnum.Dlg_RimeConvince_1027, MyDialoguesWrapperEnum.Dlg_Rime_0206),
        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1028, MyDialoguesWrapperEnum.Dlg_Rime_0207),
        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1029, MyDialoguesWrapperEnum.Dlg_Rime_0208),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1030, MyDialoguesWrapperEnum.Dlg_Rime_0209),
        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1031, MyDialoguesWrapperEnum.Dlg_Rime_0210),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1032, MyDialoguesWrapperEnum.Dlg_Rime_0211),
        new MyDialogueSentence(MyActorEnum.MARCUS,          MySoundCuesEnum.Dlg_RimeConvince_1033, MyDialoguesWrapperEnum.Dlg_Rime_0212),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1034, MyDialoguesWrapperEnum.Dlg_Rime_0213),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1035, MyDialoguesWrapperEnum.Dlg_Rime_0214),
        //new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1036, MyDialoguesWrapperEnum.Dlg_Rime_0215),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1037, MyDialoguesWrapperEnum.Dlg_Rime_0216),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1038, MyDialoguesWrapperEnum.Dlg_Rime_0217),
        new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1039, MyDialoguesWrapperEnum.Dlg_Rime_0218),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1040, MyDialoguesWrapperEnum.Dlg_Rime_0219),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1041, MyDialoguesWrapperEnum.Dlg_Rime_0220),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1042, MyDialoguesWrapperEnum.Dlg_Rime_0221),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1043, MyDialoguesWrapperEnum.Dlg_Rime_0222),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1044, MyDialoguesWrapperEnum.Dlg_Rime_0223),
        new MyDialogueSentence(MyActorEnum.VALENTIN,     MySoundCuesEnum.Dlg_RimeConvince_1045, MyDialoguesWrapperEnum.Dlg_Rime_0224),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1046, MyDialoguesWrapperEnum.Dlg_Rime_0225),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1047, MyDialoguesWrapperEnum.Dlg_Rime_0226),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1048, MyDialoguesWrapperEnum.Dlg_Rime_0227),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1049, MyDialoguesWrapperEnum.Dlg_Rime_0228),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1050, MyDialoguesWrapperEnum.Dlg_Rime_0229),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1051, MyDialoguesWrapperEnum.Dlg_Rime_0230),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1052, MyDialoguesWrapperEnum.Dlg_Rime_0231),
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1053, MyDialoguesWrapperEnum.Dlg_Rime_0232),
        new MyDialogueSentence(MyActorEnum.TARJA,       MySoundCuesEnum.Dlg_RimeConvince_1054, MyDialoguesWrapperEnum.Dlg_Rime_0233),
        new MyDialogueSentence(MyActorEnum.MARCUS,      MySoundCuesEnum.Dlg_RimeConvince_1055, MyDialoguesWrapperEnum.Dlg_Rime_0234),
        new MyDialogueSentence(MyActorEnum.MADELYN,     MySoundCuesEnum.Dlg_RimeConvince_1056, MyDialoguesWrapperEnum.Dlg_Rime_0235),  
        new MyDialogueSentence(MyActorEnum.FRANCIS_REEF,MySoundCuesEnum.Dlg_RimeConvince_1057, MyDialoguesWrapperEnum.Dlg_Rime_0236),
        }
            ));


            m_dialogues.Add((int)MyDialogueEnum.RIME_0500_LISTEN_TO_SUSPICIOUS, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1058, MyDialoguesWrapperEnum.Dlg_Rime_0501),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_0600_CLIENTS_TALK, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RIME_CLIENT1, MySoundCuesEnum.Dlg_RimeConvince_1059, MyDialoguesWrapperEnum.Dlg_Rime_0601, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_CLIENT2, MySoundCuesEnum.Dlg_RimeConvince_1060, MyDialoguesWrapperEnum.Dlg_Rime_0602, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_CLIENT2, MySoundCuesEnum.Dlg_RimeConvince_1061, MyDialoguesWrapperEnum.Dlg_Rime_0602a, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1062, MyDialoguesWrapperEnum.Dlg_Rime_0603),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_0700_DUPLEX_BOUNCER, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RIME_BOUNCER, MySoundCuesEnum.Dlg_RimeConvince_1063, MyDialoguesWrapperEnum.Dlg_Rime_0701, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_CLIENT3, MySoundCuesEnum.Dlg_RimeConvince_1064, MyDialoguesWrapperEnum.Dlg_Rime_0702, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_BOUNCER, MySoundCuesEnum.Dlg_RimeConvince_1065, MyDialoguesWrapperEnum.Dlg_Rime_0703, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_BOUNCER, MySoundCuesEnum.Dlg_RimeConvince_1066, MyDialoguesWrapperEnum.Dlg_Rime_0703a, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1067, MyDialoguesWrapperEnum.Dlg_Rime_0704),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1068, MyDialoguesWrapperEnum.Dlg_Rime_0705),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_0800_CONTACT_APPEARS, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RIME_BARKEEPER, MySoundCuesEnum.Dlg_RimeConvince_1069, MyDialoguesWrapperEnum.Dlg_Rime_0801, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_MITCHEL, MySoundCuesEnum.Dlg_RimeConvince_1070, MyDialoguesWrapperEnum.Dlg_Rime_0802, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_BARKEEPER, MySoundCuesEnum.Dlg_RimeConvince_1071, MyDialoguesWrapperEnum.Dlg_Rime_0803, noise: 0.9f),
                //new MyDialogueSentence(MyActorEnum.RIME_MITCHEL, MySoundCuesEnum.Dlg_RimeConvince_1072, MyDialoguesWrapperEnum.Dlg_Rime_0804, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1073, MyDialoguesWrapperEnum.Dlg_Rime_0805),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_0900_FOLLOW_INSTRUCTION, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1074, MyDialoguesWrapperEnum.Dlg_Rime_0901),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_1000_FACTORY_FOUND, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RIME_SMUGGLER, MySoundCuesEnum.Dlg_RimeConvince_1075, MyDialoguesWrapperEnum.Dlg_Rime_1001, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_MITCHEL, MySoundCuesEnum.Dlg_RimeConvince_1076, MyDialoguesWrapperEnum.Dlg_Rime_1002, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.RIME_SMUGGLER, MySoundCuesEnum.Dlg_RimeConvince_1077, MyDialoguesWrapperEnum.Dlg_Rime_1003, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1078, MyDialoguesWrapperEnum.Dlg_Rime_1004, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1079, MyDialoguesWrapperEnum.Dlg_Rime_1005, noise: 0.9f),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_1100_GRAB_THE_ALCOHOL, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1080, MyDialoguesWrapperEnum.Dlg_Rime_1101),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1081, MyDialoguesWrapperEnum.Dlg_Rime_1101a),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1082, MyDialoguesWrapperEnum.Dlg_Rime_1102),
            }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_1200_GET_TO_THE_VESSEL, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1083, MyDialoguesWrapperEnum.Dlg_Rime_1201),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1084, MyDialoguesWrapperEnum.Dlg_Rime_1202),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_1300_ON_THE_WAY_TO_VESSEL, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1085, MyDialoguesWrapperEnum.Dlg_Rime_1301),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1086, MyDialoguesWrapperEnum.Dlg_Rime_1302),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1087, MyDialoguesWrapperEnum.Dlg_Rime_1303),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_1400_HANDLE, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1088, MyDialoguesWrapperEnum.Dlg_Rime_1401),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1089, MyDialoguesWrapperEnum.Dlg_Rime_1402),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1090, MyDialoguesWrapperEnum.Dlg_Rime_1403),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1091, MyDialoguesWrapperEnum.Dlg_Rime_1404),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_1500_WAIT_FOR_THE_SIGNAL, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1092, MyDialoguesWrapperEnum.Dlg_Rime_1501),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1093, MyDialoguesWrapperEnum.Dlg_Rime_1502),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1094, MyDialoguesWrapperEnum.Dlg_Rime_1503),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1095, MyDialoguesWrapperEnum.Dlg_Rime_1504),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1096, MyDialoguesWrapperEnum.Dlg_Rime_1505),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1097, MyDialoguesWrapperEnum.Dlg_Rime_1506),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_1600_THIS_IS_OUR_CHANCE, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1098, MyDialoguesWrapperEnum.Dlg_Rime_1601),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1099, MyDialoguesWrapperEnum.Dlg_Rime_1602),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_1650_PLACE, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1100, MyDialoguesWrapperEnum.Dlg_Rime_1603),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1101, MyDialoguesWrapperEnum.Dlg_Rime_1604),
            }
           ));

            m_dialogues.Add((int)MyDialogueEnum.RIME_1700_HURRY_UP, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1102, MyDialoguesWrapperEnum.Dlg_Rime_1701),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1103, MyDialoguesWrapperEnum.Dlg_Rime_1702),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_1800_CARGO_PLANTED, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1104, MyDialoguesWrapperEnum.Dlg_Rime_1801),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1105, MyDialoguesWrapperEnum.Dlg_Rime_1802),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1106, MyDialoguesWrapperEnum.Dlg_Rime_1803),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1107, MyDialoguesWrapperEnum.Dlg_Rime_1804),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1108, MyDialoguesWrapperEnum.Dlg_Rime_1805),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RimeConvince_1109, MyDialoguesWrapperEnum.Dlg_Rime_1806),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1110, MyDialoguesWrapperEnum.Dlg_Rime_1807),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_1900_BACK_TO_THE_REEF, new MyDialogue(
           new MyDialogueSentence[] {
              
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_2000_REEF_TALK, new MyDialogue(
           new MyDialogueSentence[] {
                  new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1111, MyDialoguesWrapperEnum.Dlg_Rime_1901),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1112, MyDialoguesWrapperEnum.Dlg_Rime_1902),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1113, MyDialoguesWrapperEnum.Dlg_Rime_1902a),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1114, MyDialoguesWrapperEnum.Dlg_Rime_1903),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1115, MyDialoguesWrapperEnum.Dlg_Rime_1904),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1116, MyDialoguesWrapperEnum.Dlg_Rime_1905),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1117, MyDialoguesWrapperEnum.Dlg_Rime_1906),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1118, MyDialoguesWrapperEnum.Dlg_Rime_1907),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1119, MyDialoguesWrapperEnum.Dlg_Rime_1908),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1120, MyDialoguesWrapperEnum.Dlg_Rime_1909),
                //new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1121, MyDialoguesWrapperEnum.Dlg_Rime_1910),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1122, MyDialoguesWrapperEnum.Dlg_Rime_1911),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1123, MyDialoguesWrapperEnum.Dlg_Rime_1912),
                //new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1124, MyDialoguesWrapperEnum.Dlg_Rime_1913),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1125, MyDialoguesWrapperEnum.Dlg_Rime_1914),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1126, MyDialoguesWrapperEnum.Dlg_Rime_1915),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1127, MyDialoguesWrapperEnum.Dlg_Rime_1916),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1128, MyDialoguesWrapperEnum.Dlg_Rime_1917),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1129, MyDialoguesWrapperEnum.Dlg_Rime_1918),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1130, MyDialoguesWrapperEnum.Dlg_Rime_1919),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1131, MyDialoguesWrapperEnum.Dlg_Rime_1920),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1132, MyDialoguesWrapperEnum.Dlg_Rime_1921),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1133, MyDialoguesWrapperEnum.Dlg_Rime_1922),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1134, MyDialoguesWrapperEnum.Dlg_Rime_1923),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1135, MyDialoguesWrapperEnum.Dlg_Rime_1924),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1136, MyDialoguesWrapperEnum.Dlg_Rime_1925),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RimeConvince_1137, MyDialoguesWrapperEnum.Dlg_Rime_1926),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1138, MyDialoguesWrapperEnum.Dlg_Rime_1927),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1139, MyDialoguesWrapperEnum.Dlg_Rime_1927a),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1140, MyDialoguesWrapperEnum.Dlg_Rime_1929),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1141, MyDialoguesWrapperEnum.Dlg_Rime_1928),
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1142, MyDialoguesWrapperEnum.Dlg_Rime_1930),                
                new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_RimeConvince_1143, MyDialoguesWrapperEnum.Dlg_Rime_1932),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1144, MyDialoguesWrapperEnum.Dlg_Rime_1933),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1145, MyDialoguesWrapperEnum.Dlg_Rime_1934),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RimeConvince_1146, MyDialoguesWrapperEnum.Dlg_Rime_1935),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_2100_HE_IS_GONE, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1147, MyDialoguesWrapperEnum.Dlg_Rime_2101),
            }
           ));
            m_dialogues.Add((int)MyDialogueEnum.RIME_2200_HE_SPOTTED_US, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_RimeConvince_1148, MyDialoguesWrapperEnum.Dlg_Rime_2201),
            }
           ));
            #endregion

            #region Rift

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0050_INTRO, new MyDialogue(
           new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1000, MyDialoguesWrapperEnum.Dlg_Rift_2000),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_Rift_1001, MyDialoguesWrapperEnum.Dlg_Rift_2001),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1002, MyDialoguesWrapperEnum.Dlg_Rift_2002),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1003, MyDialoguesWrapperEnum.Dlg_Rift_2003),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1004, MyDialoguesWrapperEnum.Dlg_Rift_2004),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1005, MyDialoguesWrapperEnum.Dlg_Rift_2005),
                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1006, MyDialoguesWrapperEnum.Dlg_Rift_2006),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1007, MyDialoguesWrapperEnum.Dlg_Rift_2007),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1008, MyDialoguesWrapperEnum.Dlg_Rift_2008),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1009, MyDialoguesWrapperEnum.Dlg_Rift_2009),
            }
           ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0100_INTRO2, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1010, MyDialoguesWrapperEnum.Dlg_Rift_0102, pauseBefore_ms:1000),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1011, MyDialoguesWrapperEnum.Dlg_Rift_0103),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1012, MyDialoguesWrapperEnum.Dlg_Rift_0105),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_Rift_1013, MyDialoguesWrapperEnum.Dlg_Rift_0106),
                //new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_Rift_1014, MyDialoguesWrapperEnum.Dlg_Rift_0107),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0200_STATION, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1015, MyDialoguesWrapperEnum.Dlg_Rift_0201),
                new MyDialogueSentence(MyActorEnum.RiftOperator, MySoundCuesEnum.Dlg_Rift_1016, MyDialoguesWrapperEnum.Dlg_Rift_0202, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1017, MyDialoguesWrapperEnum.Dlg_Rift_0203),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1018, MyDialoguesWrapperEnum.Dlg_Rift_0204),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1019, MyDialoguesWrapperEnum.Dlg_Rift_0205),
                new MyDialogueSentence(MyActorEnum.RiftOperator, MySoundCuesEnum.Dlg_Rift_1020, MyDialoguesWrapperEnum.Dlg_Rift_0206, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1021, MyDialoguesWrapperEnum.Dlg_Rift_0208),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0300_TOURISTS, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.RiftTourist, MySoundCuesEnum.Dlg_Rift_1022, MyDialoguesWrapperEnum.Dlg_Rift_0301, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1023, MyDialoguesWrapperEnum.Dlg_Rift_0302),
                new MyDialogueSentence(MyActorEnum.RiftTourist, MySoundCuesEnum.Dlg_Rift_1024, MyDialoguesWrapperEnum.Dlg_Rift_0303, noise: 0.9f),
                //new MyDialogueSentence(MyActorEnum.RiftTourist, MySoundCuesEnum.Dlg_Rift_1025, MyDialoguesWrapperEnum.Dlg_Rift_0304, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1026, MyDialoguesWrapperEnum.Dlg_Rift_0305),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1027, MyDialoguesWrapperEnum.Dlg_Rift_0306),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1028, MyDialoguesWrapperEnum.Dlg_Rift_0307),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_Rift_1029, MyDialoguesWrapperEnum.Dlg_Rift_0308),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1030, MyDialoguesWrapperEnum.Dlg_Rift_0309),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1031, MyDialoguesWrapperEnum.Dlg_Rift_0310),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1032, MyDialoguesWrapperEnum.Dlg_Rift_0311),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_Rift_1033, MyDialoguesWrapperEnum.Dlg_Rift_0312),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_Rift_1034, MyDialoguesWrapperEnum.Dlg_Rift_0313),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1035, MyDialoguesWrapperEnum.Dlg_Rift_0314),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0400_SHOPPINGDONE, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1036, MyDialoguesWrapperEnum.Dlg_Rift_0401),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1037, MyDialoguesWrapperEnum.Dlg_Rift_0402),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0500_ENTERINGRIFT, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1038, MyDialoguesWrapperEnum.Dlg_Rift_0501),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1039, MyDialoguesWrapperEnum.Dlg_Rift_0502),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1040, MyDialoguesWrapperEnum.Dlg_Rift_0503),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1041, MyDialoguesWrapperEnum.Dlg_Rift_0504),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1042, MyDialoguesWrapperEnum.Dlg_Rift_0505),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0600_MINING, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1043, MyDialoguesWrapperEnum.Dlg_Rift_0601),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0700_MINING_COLOR, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1044, MyDialoguesWrapperEnum.Dlg_Rift_0701),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0800_MINING_TUNE, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1045, MyDialoguesWrapperEnum.Dlg_Rift_0801),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_0900_MINING_TUNE_2, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1046, MyDialoguesWrapperEnum.Dlg_Rift_0901),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_1000_MINING_DONE, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1047, MyDialoguesWrapperEnum.Dlg_Rift_1001),
                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1048, MyDialoguesWrapperEnum.Dlg_Rift_1001a),
            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RIFT_1100_LEAVING, new MyDialogue(
            new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1049, MyDialoguesWrapperEnum.Dlg_Rift_1101),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1050, MyDialoguesWrapperEnum.Dlg_Rift_1102),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1051, MyDialoguesWrapperEnum.Dlg_Rift_1103),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1052, MyDialoguesWrapperEnum.Dlg_Rift_1104),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1053, MyDialoguesWrapperEnum.Dlg_Rift_1106),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1054, MyDialoguesWrapperEnum.Dlg_Rift_1107),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_Rift_1055, MyDialoguesWrapperEnum.Dlg_Rift_1108),
                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1056, MyDialoguesWrapperEnum.Dlg_Rift_1109),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_Rift_1057, MyDialoguesWrapperEnum.Dlg_Rift_1110),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_Rift_1058, MyDialoguesWrapperEnum.Dlg_Rift_1111),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_Rift_1059, MyDialoguesWrapperEnum.Dlg_Rift_1112),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_Rift_1060, MyDialoguesWrapperEnum.Dlg_Rift_1113),
            }
            ));


            #endregion

            #region Russian Transmitter
            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0100_INTRO, new MyDialogue(
                new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RussianTransmitter_1000, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1000),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1001, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1001),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1002, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1002),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1003, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1003),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1004, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1004),
                        new MyDialogueSentence(MyActorEnum.MADELYN , MySoundCuesEnum.Dlg_RussianTransmitter_1005, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1005),
                        new MyDialogueSentence(MyActorEnum.MADELYN , MySoundCuesEnum.Dlg_RussianTransmitter_1006, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1006),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1007, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1007),
                        new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_RussianTransmitter_1008, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1008),
                    }
           ));


            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0200_BACKDOOR, new MyDialogue(
                  new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.APOLLO , MySoundCuesEnum.Dlg_RussianTransmitter_1009, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1009),
                        new MyDialogueSentence(MyActorEnum.APOLLO , MySoundCuesEnum.Dlg_RussianTransmitter_1010, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1010),
                        new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RussianTransmitter_1011, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1011),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1012, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1012),
                    }
             ));


            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0300_FINDHUB, new MyDialogue(
                  new MyDialogueSentence[] {
                      new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1013, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1013),
                      new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1014, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1014),
                      new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1015, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1015),
                      new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_RussianTransmitter_1016, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1016),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0400_HUBFOUND, new MyDialogue(
                new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1017, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1017),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1018, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1018),
                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0500_0600_HACKPROBLEM_STRANGERCALLS, new MyDialogue(
                  new MyDialogueSentence[] {
                        //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1019, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1019),
                        //new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RussianTransmitter_1020, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1020),
                        //new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RussianTransmitter_1021, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1021),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1022, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1022),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1023, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1023),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1024, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1024),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1025, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1025),

                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1026, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1026, pauseBefore_ms:2500f),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1027, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1027),
                        new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_RussianTransmitter_1028, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1028),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1029, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1029),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1030, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1030),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0700_STRANGERPROPOSAL, new MyDialogue(
                  new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.VOLODIA_STRANGER , MySoundCuesEnum.Dlg_RussianTransmitter_1031, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1031),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1032, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1032),
                        new MyDialogueSentence(MyActorEnum.VOLODIA_STRANGER, MySoundCuesEnum.Dlg_RussianTransmitter_1033, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1033),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1034, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1034),
                        new MyDialogueSentence(MyActorEnum.VOLODIA_STRANGER, MySoundCuesEnum.Dlg_RussianTransmitter_1035, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1035),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0700_VOLODIAINTRO, new MyDialogue(
                  new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.VOLODIA_STRANGER, MySoundCuesEnum.Dlg_RussianTransmitter_1036, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1036),
                        new MyDialogueSentence(MyActorEnum.VOLODIA , MySoundCuesEnum.Dlg_RussianTransmitter_1037, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1037),
                        new MyDialogueSentence(MyActorEnum.VOLODIA , MySoundCuesEnum.Dlg_RussianTransmitter_1038, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1038),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1039, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1039),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1040, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1040),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1041, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1041),
                        new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RussianTransmitter_1042, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1042),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1043, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1043),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1044, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1044),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1045, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1045),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0800_NEARCARGO, new MyDialogue(
                  new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1046, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1046),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1047, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1047),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_0900_VOLODIA_RANT, new MyDialogue(
                  new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1048, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1048),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1049, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1049),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1050, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1050),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1051, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1051),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1052, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1052),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1053, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1053),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1054, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1054),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1055, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1055),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1056, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1056),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1057, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1057),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1058, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1058),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1059, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1059),
                        new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RussianTransmitter_1060, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1060),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1061, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1061),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1000_VOLODIA_FOUND, new MyDialogue(
                  new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1062, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1062),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1063, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1063),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1064, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1064),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1065, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1065),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1066, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1066),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1067, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1067),
                        new MyDialogueSentence(MyActorEnum.RussianCaptain, MySoundCuesEnum.Dlg_RussianTransmitter_1068, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1068),
                        new MyDialogueSentence(MyActorEnum.VOLODIA, MySoundCuesEnum.Dlg_RussianTransmitter_1069, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1069),
                        new MyDialogueSentence(MyActorEnum.RussianCaptain, MySoundCuesEnum.Dlg_RussianTransmitter_1070, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1070),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1100_MADELYNSCARED, new MyDialogue(
                  new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1071, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1071),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1300_APOLLOSCARED, new MyDialogue(
                  new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1072, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1072),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1073, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1073),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1074, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1074),
                    }
             ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1400_RETREAT, new MyDialogue(
                 new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1075, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1075),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1076, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1076),
                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1500_IFITDOESNOTWORK, new MyDialogue(
                 new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1077, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1077),
                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1600_ITSWORKING, new MyDialogue(
                 new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1078, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1078),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1079, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1079),
                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1700_UPLOADINGSIGNAL, new MyDialogue(
                     new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1080, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1080),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1081, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1081),

                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1800_BADROUTE, new MyDialogue(
                     new MyDialogueSentence[] {
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_RussianTransmitter_1082, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1082),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_RussianTransmitter_1083, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1083),
                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_1900_PLACEDEVICE, new MyDialogue(
                     new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1084, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1084),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1085, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1085),
                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_2000_DEVICEWORKING, new MyDialogue(
                                 new MyDialogueSentence[] {
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1086, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1086),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1087, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1087),
                        new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_RussianTransmitter_1088, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1088),
                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_2100_WEMADEIT, new MyDialogue(
                     new MyDialogueSentence[] {
                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1089, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1089),

                    }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RUSSIAN_TRANSMITTER_2200_THOMASCHAT, new MyDialogue(
                     new MyDialogueSentence[] {
                            //new MyDialogueSentence(MyActorEnum.THOMAS , MySoundCuesEnum.Dlg_RussianTransmitter_1090, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1090),
                            //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1091, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1091),
                            new MyDialogueSentence(MyActorEnum.THOMAS , MySoundCuesEnum.Dlg_RussianTransmitter_1092, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1092),
                            new MyDialogueSentence(MyActorEnum.THOMAS , MySoundCuesEnum.Dlg_RussianTransmitter_1093, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1093),
                            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1094, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1094),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1095, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1095),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1096, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1096),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1097, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1097),
                            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1098, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1098),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1099, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1099),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1100, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1100),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1101, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1101),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1102, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1102),
                            new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_RussianTransmitter_1103, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1103),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1104, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1104),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1105, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1105),
                            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1106, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1106),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1107, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1107),
                            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_RussianTransmitter_1108, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1108),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1109, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1109),
                            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_RussianTransmitter_1110, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1110),
                            new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_RussianTransmitter_1111, MyDialoguesWrapperEnum.Dlg_RussianTransmitter_1111),

                    }
            ));
            //m_dialogues.Add((int)MyDialogueEnum., new MyDialogue(
            //      new MyDialogueSentence[] {

            //    }
            // ));

            #endregion

            #region White Volves Research

            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0100_COLLECT, new MyDialogue(
                new MyDialogueSentence[] { 
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1000, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1000),
                        new MyDialogueSentence(MyActorEnum.APOLLO,  MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1001, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1001),
                        new MyDialogueSentence(MyActorEnum.APOLLO,  MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1002, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1002),
                        new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1003, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1003),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1004, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1004),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1005, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1005),
                    }
                ));

            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0200_ENTER, new MyDialogue(
                new MyDialogueSentence[] {    
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1006, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1006),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1007, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1007),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1008, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1008),
                        new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1009, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1009),
                    }
                ));

            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0300_COLLECT2, new MyDialogue(
                    new MyDialogueSentence[] {  
                        new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1010, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1010),
                        new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1011, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1011),
                    }
                    ));


            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0350_SICKEN, new MyDialogue(
                    new MyDialogueSentence[] {   
                       new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1012, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1012),
                        new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1013, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1013),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1014, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1014),
                        new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1015, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1015),
                        new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1016, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1016),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1017, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1017),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1018, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1018),
                        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1019, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1019),
                    }
                    ));

            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0400_FIND, new MyDialogue(
                        new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1020, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1020),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1021, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1021),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1022, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1022),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1023, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1023),
                 }
                        ));

            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0500_GET_OUT, new MyDialogue(
                       new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1024, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1024),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1025, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1025),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1026, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1026),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1027, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1027),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1028, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1028),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1029, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1029),
                 }
                       ));

            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0550_GET_OUT_SUCCESS, new MyDialogue(
                       new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1030, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1030),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1031, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1031),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1032, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1032),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1033, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1033),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1034, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1034),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1035, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1035),
                 }
                       ));

            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0600_DESTROY, new MyDialogue(
                       new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1036, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1036),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1037, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1037),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1038, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1038),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1039, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1039),
                    //new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1040, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1040),
                 }
                       ));

            m_dialogues.Add((int)MyDialogueEnum.WHITEWOLVES_RESEARCH_0700_RETURN, new MyDialogue(
                       new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1041, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1041),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1042, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1042),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1043, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1043),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1044, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1044),
                 }
                       ));

            /*
       m_dialogues.Add((int)MyDialogueEnum.WHITEVOLVES_RESEARCH_0400_FIND, new MyDialogue(
               new MyDialogueSentence[] {
            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1020, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1020),
           new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1021, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1021),
           new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1022, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1022),
           new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1023, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1023),
        }
               ));

       m_dialogues.Add((int)MyDialogueEnum.WHITEVOLVES_RESEARCH_0500_GET_OUT, new MyDialogue(
                  new MyDialogueSentence[] {
            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1024, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1024),
           new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1025, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1025),
           new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1026, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1026),
           new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1027, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1027),
           new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1028, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1028),
           new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1029, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1029),
        }
                  ));

       m_dialogues.Add((int)MyDialogueEnum.WHITEVOLVES_RESEARCH_0550_GET_OUT_SUCCESS, new MyDialogue(
                  new MyDialogueSentence[] {
            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1030, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1030),
           new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1031, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1031),
           new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1032, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1032),
           new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1033, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1033),
           new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1034, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1034),
           new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1035, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1035),
        }
                  ));

       m_dialogues.Add((int)MyDialogueEnum.WHITEVOLVES_RESEARCH_0600_DESTROY, new MyDialogue(
                  new MyDialogueSentence[] {
           new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1036, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1036),
           new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1037, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1037),
           new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1038, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1038),
           new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1039, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1039),
           new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1040, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1040),
        }
                  ));

       m_dialogues.Add((int)MyDialogueEnum.WHITEVOLVES_RESEARCH_0700_RETURN, new MyDialogue(
                  new MyDialogueSentence[] {
           new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1041, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1041),
           new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1042, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1042),
           new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1043, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1043),
           new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_WhiteWolvesBioResearch_1044, MyDialoguesWrapperEnum.Dlg_WhitewolvesResearch_1044),
        }
                  ));  */

            #endregion

            #region BarthsMoon2

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0100_INTRO, new MyDialogue(
                new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1001, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0101),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1002, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0102),
                //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1003, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0103),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1004, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0104),
                //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1005, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0105),
                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1006, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0106),
                //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1007, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0107),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1008, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0108),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1009, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0109),
                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1010, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0110),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1011, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0111),
                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1012, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0112),
                //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1013, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0113),
                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1014, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0114),
                //new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1015, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0115),
                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1016, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0116),

            }
            ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0200, new MyDialogue(
                                                                                         new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1018, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0201),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1019, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0202),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1020, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0203),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1021, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0204),
                                                                                                //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1022, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0205),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1023, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0206),

                                                                                             }
                                                                                         ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0300, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1024, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0301),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1025, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0302),
                                                                                                //new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1026, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0303),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1027, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0304),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1028, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0305),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1029, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0306),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1030, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0307),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1031, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0308),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1032, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0309),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1033, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0310),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1034, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0311),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1035, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0312),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1036, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0313),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1037, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0314),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1038, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0315),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1039, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0316),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1040, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0317),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1041, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0318),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1042, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0319),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1043, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0320),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1044, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0321/*, pauseBefore_ms: 300*/),
                                                                                                //new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1045, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0322),
                                                                                                //new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1046, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0323),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1047, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0324),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0400, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1048, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0401),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1049, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0402),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1050, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0403),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1051, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0404),
                                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1052, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0405),
                                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1053, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0406/*, pauseBefore_ms: 300*/),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1054, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0407),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1055, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0408),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1056, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0409),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1057, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0410),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1058, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0411),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1059, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0412),
                                                                                                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1060, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0413),
                                                                                                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1061, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0414),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1062, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0415),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1063, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0416),
                                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1064, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0417),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1065, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0418),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1066, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0419),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1067, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0420),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1068, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0421),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1069, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0422),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0500, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1070, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0501),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0600, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1071, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0601),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1072, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0602),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1073, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0603),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1074, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0604),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0700, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1075, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0701),
                                                                                                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1076, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0702),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1077, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0703),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1078, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0704),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1079, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0705),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0800, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1079b, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0801),
                                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1080, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0802),
                                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1081, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0803),
                                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1082, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0804),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_0900, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1083, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_0901),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1000, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1084, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1001),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1100, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1085, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1101),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1086, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1102),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1200, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1087, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1201),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1088, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1202),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1300, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1089, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1301),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1400, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1090, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1401),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1091, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1402),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1092, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1403),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1093, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1404),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1500, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1094, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1501),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1095, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1502),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1096, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1503),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1097, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1504),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1098, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1505),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1099, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1506),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1100, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1507),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1101, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1508),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1102, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1509),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1600, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1103, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1601),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1104, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1602),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1700, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1105, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1701),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1106, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1702),

                                                                                             }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1800, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1107, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1801),

                                                                                             }
                                                                             ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_1900, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1108, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1901),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1109, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1902),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1110, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1903),
                                                                                                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1111, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1904),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1112, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_1905),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2000, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1113, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2001),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1114, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2002),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2100, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1115, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2101),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1116, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2102),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1117, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2103),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1118, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2104),
                                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1119, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2105),
                                                                                                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1120, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2106),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1121, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2107),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2200, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1122, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2201),
                                                                                                new MyDialogueSentence(MyActorEnum.BLONDI, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1123, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2202),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1124, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2203),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1125, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2204),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1126, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2205),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1127, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2206),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1128, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2207),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1129, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2208),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1130, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2209),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1131, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2210),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1132, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2211),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1133, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2212),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1134, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2213),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1135, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2214),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1136, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2215),
                                                                                                new MyDialogueSentence(MyActorEnum.BLONDI, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1137, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2216),
                                                                                                new MyDialogueSentence(MyActorEnum.BLONDI, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1138, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2217),
                                                                                                new MyDialogueSentence(MyActorEnum.BLONDI, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1139, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2218),
                                                                                                new MyDialogueSentence(MyActorEnum.BLONDI, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1140, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2219),
                                                                                                new MyDialogueSentence(MyActorEnum.BLONDI, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1141, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2220),
                                                                                                new MyDialogueSentence(MyActorEnum.BLONDI, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1142, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2221),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1143, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2222),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1144, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2223),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1145, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2224),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1146, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2225),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1147, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2226),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1148, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2227),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1149, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2228),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1150, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2229),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1151, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2230),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2300, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1152, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2301),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1153, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2302),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2400, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.BLONDI, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1154, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2401),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2500, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1155, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2501),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1156, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2502),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1157, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2503),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1158, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2504),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1159, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2505),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1160, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2506),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1161, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2507),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1162, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2508),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1163, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2509),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1164, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2510),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1165, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2511),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1166, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2512),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1167, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2513),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1168, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2514),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2600, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1169, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2601),
                                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1170, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2602),

                                                                                             }
                                                                 ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2700, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.RAIDER, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1171, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2701),
                                                                                                    new MyDialogueSentence(MyActorEnum.RAIDER, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1172, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2702),
                                                                                                new MyDialogueSentence(MyActorEnum.RAIDER, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1173, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2703),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1174, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2704),

                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2800, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1175, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2801),
                                                                                                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1176, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2802),

                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_2900, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1177, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2901),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1178, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2902),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1179, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2903),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1180, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2904),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1181, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2905),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1182, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2906),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1183, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_2907),

                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_3000, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1184, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3001),
                                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1185, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3002),

                                                                                             }
                                                     ));


            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_3100, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1186, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3101),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1187, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3102),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1188, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3103),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1189, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3104),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1190, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3105),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1191, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3106),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1192, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3107),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1193, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3108),

                                                                                             }
                                                     ));


            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_TRANSMITTER_3200, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1194, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3201),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1195, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3202),
                                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1196, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3203),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1197, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3204),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1198, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3205),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1199, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3206),
                                                                                                //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1200, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3207),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1201, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3208),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1202, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3209),
                                                                                                new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1203, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3210),
                                                                                                new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1204, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3211),
                                                                                                new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1205, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3212),
                                                                                                new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1206, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3213),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1207, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3214),
                                                                                                new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1208, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3215),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1209, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3216),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1210, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3217),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1211, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3218),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1212, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3219),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1213, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3220),
                                                                                                new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1214, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3221),
                                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonTransmitter_1215, MyDialoguesWrapperEnum.Dlg_BarthsMoon2_3222),

                                                                                             }
                                                     ));

            #endregion

            #region Fort Valiant A


            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_A_0100, new MyDialogue(
                                                                                         new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1000, MyDialoguesWrapperEnum.Dlg_FortValiantA_0100),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiant_1001, MyDialoguesWrapperEnum.Dlg_FortValiantA_0101),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiant_1002, MyDialoguesWrapperEnum.Dlg_FortValiantA_0102),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiant_1003, MyDialoguesWrapperEnum.Dlg_FortValiantA_0103),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1004, MyDialoguesWrapperEnum.Dlg_FortValiantA_0104),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiant_1005, MyDialoguesWrapperEnum.Dlg_FortValiantA_0105),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiant_1006, MyDialoguesWrapperEnum.Dlg_FortValiantA_0106),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1007, MyDialoguesWrapperEnum.Dlg_FortValiantA_0107),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiant_1008, MyDialoguesWrapperEnum.Dlg_FortValiantA_0108),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiant_1009, MyDialoguesWrapperEnum.Dlg_FortValiantA_0109),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiant_1010, MyDialoguesWrapperEnum.Dlg_FortValiantA_0110),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiant_1011, MyDialoguesWrapperEnum.Dlg_FortValiantA_0111),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiant_1012, MyDialoguesWrapperEnum.Dlg_FortValiantA_0112),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1013, MyDialoguesWrapperEnum.Dlg_FortValiantA_0113),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_FortValiant_1014, MyDialoguesWrapperEnum.Dlg_FortValiantA_0114),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_FortValiant_1015, MyDialoguesWrapperEnum.Dlg_FortValiantA_0115),


                                                                                             }
                                                                                         ));

            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_A_0200, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.GATEKEEPER, MySoundCuesEnum.Dlg_FortValiant_1016, MyDialoguesWrapperEnum.Dlg_FortValiantA_0200),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiant_1017, MyDialoguesWrapperEnum.Dlg_FortValiantA_0201),
new MyDialogueSentence(MyActorEnum.GATEKEEPER, MySoundCuesEnum.Dlg_FortValiant_1018, MyDialoguesWrapperEnum.Dlg_FortValiantA_0202),
new MyDialogueSentence(MyActorEnum.GATEKEEPER, MySoundCuesEnum.Dlg_FortValiant_1019, MyDialoguesWrapperEnum.Dlg_FortValiantA_0203),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiant_1020, MyDialoguesWrapperEnum.Dlg_FortValiantA_0204),


                                                                                             }
                                                                             ));

            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_A_0300, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1021, MyDialoguesWrapperEnum.Dlg_FortValiantA_0300),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiant_1022, MyDialoguesWrapperEnum.Dlg_FortValiantA_0301),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1023, MyDialoguesWrapperEnum.Dlg_FortValiantA_0302),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1024, MyDialoguesWrapperEnum.Dlg_FortValiantA_0303),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1025, MyDialoguesWrapperEnum.Dlg_FortValiantA_0304),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1026, MyDialoguesWrapperEnum.Dlg_FortValiantA_0305),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1027, MyDialoguesWrapperEnum.Dlg_FortValiantA_0306),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1028, MyDialoguesWrapperEnum.Dlg_FortValiantA_0307),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1029, MyDialoguesWrapperEnum.Dlg_FortValiantA_0308),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_FortValiant_1030, MyDialoguesWrapperEnum.Dlg_FortValiantA_0309),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1031, MyDialoguesWrapperEnum.Dlg_FortValiantA_0310),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1032, MyDialoguesWrapperEnum.Dlg_FortValiantA_0311),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiant_1033, MyDialoguesWrapperEnum.Dlg_FortValiantA_0312),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1034, MyDialoguesWrapperEnum.Dlg_FortValiantA_0313),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1035, MyDialoguesWrapperEnum.Dlg_FortValiantA_0314),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiant_1036, MyDialoguesWrapperEnum.Dlg_FortValiantA_0315),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiant_1037, MyDialoguesWrapperEnum.Dlg_FortValiantA_0316),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1038, MyDialoguesWrapperEnum.Dlg_FortValiantA_0317),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1039, MyDialoguesWrapperEnum.Dlg_FortValiantA_0318),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1040, MyDialoguesWrapperEnum.Dlg_FortValiantA_0319),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiant_1041, MyDialoguesWrapperEnum.Dlg_FortValiantA_0320),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiant_1042, MyDialoguesWrapperEnum.Dlg_FortValiantA_0321),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_FortValiant_1043, MyDialoguesWrapperEnum.Dlg_FortValiantA_0322),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1044, MyDialoguesWrapperEnum.Dlg_FortValiantA_0323),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1045, MyDialoguesWrapperEnum.Dlg_FortValiantA_0324),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiant_1046, MyDialoguesWrapperEnum.Dlg_FortValiantA_0325),


                                                                                             }
                                                                             ));


            #endregion

            #region Fort Valiant B

            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_B_0100, new MyDialogue(
                                                                                         new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantB_1000, MyDialoguesWrapperEnum.Dlg_FortValiantB_0100),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantB_1001, MyDialoguesWrapperEnum.Dlg_FortValiantB_0101),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantB_1002, MyDialoguesWrapperEnum.Dlg_FortValiantB_0102),

                                                                                             }
                                                                                         ));



            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_B_0200, new MyDialogue(
                                                                                         new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1003, MyDialoguesWrapperEnum.Dlg_FortValiantB_0201),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1004, MyDialoguesWrapperEnum.Dlg_FortValiantB_0202),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1005, MyDialoguesWrapperEnum.Dlg_FortValiantB_0203),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1006, MyDialoguesWrapperEnum.Dlg_FortValiantB_0204),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiantB_1007, MyDialoguesWrapperEnum.Dlg_FortValiantB_0205),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1008, MyDialoguesWrapperEnum.Dlg_FortValiantB_0207),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1009, MyDialoguesWrapperEnum.Dlg_FortValiantB_0208),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiantB_1010, MyDialoguesWrapperEnum.Dlg_FortValiantB_0209),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1011, MyDialoguesWrapperEnum.Dlg_FortValiantB_0210),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1012, MyDialoguesWrapperEnum.Dlg_FortValiantB_0211),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1013, MyDialoguesWrapperEnum.Dlg_FortValiantB_0212),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1014, MyDialoguesWrapperEnum.Dlg_FortValiantB_0213),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1015, MyDialoguesWrapperEnum.Dlg_FortValiantB_0214),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantB_1016, MyDialoguesWrapperEnum.Dlg_FortValiantB_0215),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantB_1017, MyDialoguesWrapperEnum.Dlg_FortValiantB_0216),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1018, MyDialoguesWrapperEnum.Dlg_FortValiantB_0217),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantB_1019, MyDialoguesWrapperEnum.Dlg_FortValiantB_0218),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantB_1020, MyDialoguesWrapperEnum.Dlg_FortValiantB_0219),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1021, MyDialoguesWrapperEnum.Dlg_FortValiantB_0220),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1022, MyDialoguesWrapperEnum.Dlg_FortValiantB_0221),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantB_1023, MyDialoguesWrapperEnum.Dlg_FortValiantB_0222),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantB_1024, MyDialoguesWrapperEnum.Dlg_FortValiantB_0223),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantB_1025, MyDialoguesWrapperEnum.Dlg_FortValiantB_0224),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1026, MyDialoguesWrapperEnum.Dlg_FortValiantB_0225),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantB_1027, MyDialoguesWrapperEnum.Dlg_FortValiantB_0226),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantB_1028, MyDialoguesWrapperEnum.Dlg_FortValiantB_0227),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1029, MyDialoguesWrapperEnum.Dlg_FortValiantB_0228),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1030, MyDialoguesWrapperEnum.Dlg_FortValiantB_0229),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantB_1031, MyDialoguesWrapperEnum.Dlg_FortValiantB_0230),

                                                                                             }
                                                                                         ));
            #endregion

            #region Reichstag A

            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_A_0100_INTRODUCTION, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ReichstagA_1000, MyDialoguesWrapperEnum.Dlg_ReichstagA_1000),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ReichstagA_1001, MyDialoguesWrapperEnum.Dlg_ReichstagA_1001),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ReichstagA_1002, MyDialoguesWrapperEnum.Dlg_ReichstagA_1002),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ReichstagA_1003, MyDialoguesWrapperEnum.Dlg_ReichstagA_1003),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1004, MyDialoguesWrapperEnum.Dlg_ReichstagA_1004),
            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_A_0200_ON_THE_WAY, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ReichstagA_1005, MyDialoguesWrapperEnum.Dlg_ReichstagA_1005),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagA_1006, MyDialoguesWrapperEnum.Dlg_ReichstagA_1006),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ReichstagA_1007, MyDialoguesWrapperEnum.Dlg_ReichstagA_1007),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1008, MyDialoguesWrapperEnum.Dlg_ReichstagA_1008),
                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ReichstagA_1009, MyDialoguesWrapperEnum.Dlg_ReichstagA_1009),
            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_A_0300_REACHING_REICHSTAG, new MyDialogue(
          new MyDialogueSentence[] {
                //new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagA_1010, MyDialoguesWrapperEnum.Dlg_ReichstagA_1010),  // bad pronunciation, no cookie
                //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1011, MyDialoguesWrapperEnum.Dlg_ReichstagA_1011),
            }
          ));
          
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_A_0400_OFFICER_DIALOGUE, new MyDialogue(
          new MyDialogueSentence[] {
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1012, MyDialoguesWrapperEnum.Dlg_ReichstagA_1012),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1013, MyDialoguesWrapperEnum.Dlg_ReichstagA_1013),
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1014, MyDialoguesWrapperEnum.Dlg_ReichstagA_1014),
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1015, MyDialoguesWrapperEnum.Dlg_ReichstagA_1015),
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1016, MyDialoguesWrapperEnum.Dlg_ReichstagA_1016),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1017, MyDialoguesWrapperEnum.Dlg_ReichstagA_1017),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1018, MyDialoguesWrapperEnum.Dlg_ReichstagA_1018),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1019, MyDialoguesWrapperEnum.Dlg_ReichstagA_1019),
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1020, MyDialoguesWrapperEnum.Dlg_ReichstagA_1020),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1021, MyDialoguesWrapperEnum.Dlg_ReichstagA_1021),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1022, MyDialoguesWrapperEnum.Dlg_ReichstagA_1022),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1023, MyDialoguesWrapperEnum.Dlg_ReichstagA_1023),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1024, MyDialoguesWrapperEnum.Dlg_ReichstagA_1024),
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1025, MyDialoguesWrapperEnum.Dlg_ReichstagA_1025),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1026, MyDialoguesWrapperEnum.Dlg_ReichstagA_1026),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1027, MyDialoguesWrapperEnum.Dlg_ReichstagA_1027),
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1028, MyDialoguesWrapperEnum.Dlg_ReichstagA_1028),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1029, MyDialoguesWrapperEnum.Dlg_ReichstagA_1029),
            new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagA_1030, MyDialoguesWrapperEnum.Dlg_ReichstagA_1030),
            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_A_0500_ON_THE_WAY_BACK, new MyDialogue(
            new MyDialogueSentence[] {
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1031, MyDialoguesWrapperEnum.Dlg_ReichstagA_1031),
            new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagA_1032, MyDialoguesWrapperEnum.Dlg_ReichstagA_1032),
            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ReichstagA_1033, MyDialoguesWrapperEnum.Dlg_ReichstagA_1033),
            new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ReichstagA_1034, MyDialoguesWrapperEnum.Dlg_ReichstagA_1034),
            new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagA_1035, MyDialoguesWrapperEnum.Dlg_ReichstagA_1035),
            new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ReichstagA_1036, MyDialoguesWrapperEnum.Dlg_ReichstagA_1036),
            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ReichstagA_1037, MyDialoguesWrapperEnum.Dlg_ReichstagA_1037),
            new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ReichstagA_1038, MyDialoguesWrapperEnum.Dlg_ReichstagA_1038),            }
          ));
            #endregion

            #region Reichstag C
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0100_OFFICER_TALK, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1000, MyDialoguesWrapperEnum.Dlg_ReichstagC_1000),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1001, MyDialoguesWrapperEnum.Dlg_ReichstagC_1001),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1002, MyDialoguesWrapperEnum.Dlg_ReichstagC_1002),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1003, MyDialoguesWrapperEnum.Dlg_ReichstagC_1003),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1004, MyDialoguesWrapperEnum.Dlg_ReichstagC_1004),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1005, MyDialoguesWrapperEnum.Dlg_ReichstagC_1005),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1006, MyDialoguesWrapperEnum.Dlg_ReichstagC_1006),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1007, MyDialoguesWrapperEnum.Dlg_ReichstagC_1007),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1008, MyDialoguesWrapperEnum.Dlg_ReichstagC_1008),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1009, MyDialoguesWrapperEnum.Dlg_ReichstagC_1009),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1010, MyDialoguesWrapperEnum.Dlg_ReichstagC_1010),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1011, MyDialoguesWrapperEnum.Dlg_ReichstagC_1011),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1012, MyDialoguesWrapperEnum.Dlg_ReichstagC_1012),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1013, MyDialoguesWrapperEnum.Dlg_ReichstagC_1013),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1014, MyDialoguesWrapperEnum.Dlg_ReichstagC_1014),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1015, MyDialoguesWrapperEnum.Dlg_ReichstagC_1015),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1016, MyDialoguesWrapperEnum.Dlg_ReichstagC_1016),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1017, MyDialoguesWrapperEnum.Dlg_ReichstagC_1017),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1018, MyDialoguesWrapperEnum.Dlg_ReichstagC_1018),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1019, MyDialoguesWrapperEnum.Dlg_ReichstagC_1019),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1020, MyDialoguesWrapperEnum.Dlg_ReichstagC_1020),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1021, MyDialoguesWrapperEnum.Dlg_ReichstagC_1021),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1022, MyDialoguesWrapperEnum.Dlg_ReichstagC_1022),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1022_2, MyDialoguesWrapperEnum.Dlg_ReichstagC_1022_2),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1023, MyDialoguesWrapperEnum.Dlg_ReichstagC_1023),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1024, MyDialoguesWrapperEnum.Dlg_ReichstagC_1024),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1025, MyDialoguesWrapperEnum.Dlg_ReichstagC_1025),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1026, MyDialoguesWrapperEnum.Dlg_ReichstagC_1026),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1027, MyDialoguesWrapperEnum.Dlg_ReichstagC_1027),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1028, MyDialoguesWrapperEnum.Dlg_ReichstagC_1028),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1029, MyDialoguesWrapperEnum.Dlg_ReichstagC_1029),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1030, MyDialoguesWrapperEnum.Dlg_ReichstagC_1030),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1031, MyDialoguesWrapperEnum.Dlg_ReichstagC_1031),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1032, MyDialoguesWrapperEnum.Dlg_ReichstagC_1032),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1033, MyDialoguesWrapperEnum.Dlg_ReichstagC_1033),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1034, MyDialoguesWrapperEnum.Dlg_ReichstagC_1034),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1035, MyDialoguesWrapperEnum.Dlg_ReichstagC_1035),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1036, MyDialoguesWrapperEnum.Dlg_ReichstagC_1036),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1037, MyDialoguesWrapperEnum.Dlg_ReichstagC_1037),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1038, MyDialoguesWrapperEnum.Dlg_ReichstagC_1038),
            }
          ));

            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0200_ON_THE_WAY, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1039, MyDialoguesWrapperEnum.Dlg_ReichstagC_1039),
                new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_ReichstagC_1040, MyDialoguesWrapperEnum.Dlg_ReichstagC_1040),
            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0300_REACHING_SHIPYARD, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1041, MyDialoguesWrapperEnum.Dlg_ReichstagC_1041),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagC_1042, MyDialoguesWrapperEnum.Dlg_ReichstagC_1042),
            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0400_SUPPLY_OFFICER, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.REICHSTAG_OFFICER, MySoundCuesEnum.Dlg_ReichstagC_1043, MyDialoguesWrapperEnum.Dlg_ReichstagC_1043, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1044, MyDialoguesWrapperEnum.Dlg_ReichstagC_1044),
                new MyDialogueSentence(MyActorEnum.REICHSTAG_OFFICER, MySoundCuesEnum.Dlg_ReichstagC_1045, MyDialoguesWrapperEnum.Dlg_ReichstagC_1045, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.REICHSTAG_OFFICER, MySoundCuesEnum.Dlg_ReichstagC_1046, MyDialoguesWrapperEnum.Dlg_ReichstagC_1046, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1047, MyDialoguesWrapperEnum.Dlg_ReichstagC_1047),
                new MyDialogueSentence(MyActorEnum.REICHSTAG_OFFICER, MySoundCuesEnum.Dlg_ReichstagC_1048, MyDialoguesWrapperEnum.Dlg_ReichstagC_1048, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.REICHSTAG_OFFICER, MySoundCuesEnum.Dlg_ReichstagC_1049, MyDialoguesWrapperEnum.Dlg_ReichstagC_1049, noise: 0.9f),

            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0500_REACHING_SHIPS, new MyDialogue(
          new MyDialogueSentence[] {
               new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagC_1050, MyDialoguesWrapperEnum.Dlg_ReichstagC_1050),
            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0600_SHIPS_PICKUPED, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagC_1051, MyDialoguesWrapperEnum.Dlg_ReichstagC_1051),
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ReichstagC_1052, MyDialoguesWrapperEnum.Dlg_ReichstagC_1052),
                new MyDialogueSentence(MyActorEnum.REICHSTAG_OFFICER, MySoundCuesEnum.Dlg_ReichstagC_1053, MyDialoguesWrapperEnum.Dlg_ReichstagC_1053, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.REICHSTAG_OFFICER, MySoundCuesEnum.Dlg_ReichstagC_1054, MyDialoguesWrapperEnum.Dlg_ReichstagC_1054, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1055, MyDialoguesWrapperEnum.Dlg_ReichstagC_1055),
            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0700_SHIPYARD_SHOP, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ReichstagC_1056, MyDialoguesWrapperEnum.Dlg_ReichstagC_1056),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagC_1057, MyDialoguesWrapperEnum.Dlg_ReichstagC_1057),
            }
          ));
            m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0800_SHOPPING_FINISHED, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagC_1058, MyDialoguesWrapperEnum.Dlg_ReichstagC_1058),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagC_1059, MyDialoguesWrapperEnum.Dlg_ReichstagC_1059),
            }
          ));
                        m_dialogues.Add((int)MyDialogueEnum.REICHSTAG_C_0900_TRANSPORTER_REACHED, new MyDialogue(
          new MyDialogueSentence[] {
                new MyDialogueSentence(MyActorEnum.REICHSTAG_CAPTAIN, MySoundCuesEnum.Dlg_ReichstagC_1060, MyDialoguesWrapperEnum.Dlg_ReichstagC_1060, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1061, MyDialoguesWrapperEnum.Dlg_ReichstagC_1061),
                new MyDialogueSentence(MyActorEnum.REICHSTAG_CAPTAIN, MySoundCuesEnum.Dlg_ReichstagC_1062, MyDialoguesWrapperEnum.Dlg_ReichstagC_1062, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.REICHSTAG_CAPTAIN, MySoundCuesEnum.Dlg_ReichstagC_1063, MyDialoguesWrapperEnum.Dlg_ReichstagC_1063, noise: 0.9f),
                new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ReichstagC_1064, MyDialoguesWrapperEnum.Dlg_ReichstagC_1064),
                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ReichstagC_1065, MyDialoguesWrapperEnum.Dlg_ReichstagC_1065),
            }
          ));
            #endregion

            #region EAC prison
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0100, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1000, MyDialoguesWrapperEnum.Dlg_EACPrison_0100),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1001, MyDialoguesWrapperEnum.Dlg_EACPrison_0101),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1002, MyDialoguesWrapperEnum.Dlg_EACPrison_0102),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1003, MyDialoguesWrapperEnum.Dlg_EACPrison_0103),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1004, MyDialoguesWrapperEnum.Dlg_EACPrison_0104),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1005, MyDialoguesWrapperEnum.Dlg_EACPrison_0105),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1006, MyDialoguesWrapperEnum.Dlg_EACPrison_0106),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1007, MyDialoguesWrapperEnum.Dlg_EACPrison_0107),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1008, MyDialoguesWrapperEnum.Dlg_EACPrison_0108),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1009, MyDialoguesWrapperEnum.Dlg_EACPrison_0109),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1010, MyDialoguesWrapperEnum.Dlg_EACPrison_0110),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1011, MyDialoguesWrapperEnum.Dlg_EACPrison_0111),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1012, MyDialoguesWrapperEnum.Dlg_EACPrison_0112),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1013, MyDialoguesWrapperEnum.Dlg_EACPrison_0113),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1014, MyDialoguesWrapperEnum.Dlg_EACPrison_0114),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1015, MyDialoguesWrapperEnum.Dlg_EACPrison_0115),


                                                                                             }
                                                                             ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0200, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {

                                                                                                 new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1016, MyDialoguesWrapperEnum.Dlg_EACPrison_0200),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1017, MyDialoguesWrapperEnum.Dlg_EACPrison_0201),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0300, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {

                                                                                                 new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1018, MyDialoguesWrapperEnum.Dlg_EACPrison_0300),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1019, MyDialoguesWrapperEnum.Dlg_EACPrison_0301),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1020, MyDialoguesWrapperEnum.Dlg_EACPrison_0302),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0400, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
                                                                                                 new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1021, MyDialoguesWrapperEnum.Dlg_EACPrison_0400),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1022, MyDialoguesWrapperEnum.Dlg_EACPrison_0401),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1023, MyDialoguesWrapperEnum.Dlg_EACPrison_0402),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1024, MyDialoguesWrapperEnum.Dlg_EACPrison_0403),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1025, MyDialoguesWrapperEnum.Dlg_EACPrison_0404),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1026, MyDialoguesWrapperEnum.Dlg_EACPrison_0405),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1027, MyDialoguesWrapperEnum.Dlg_EACPrison_0406),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1028, MyDialoguesWrapperEnum.Dlg_EACPrison_0407),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacPrison_1029, MyDialoguesWrapperEnum.Dlg_EACPrison_0408),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1030, MyDialoguesWrapperEnum.Dlg_EACPrison_0409),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1031, MyDialoguesWrapperEnum.Dlg_EACPrison_0410),


                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0500, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {

new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1032, MyDialoguesWrapperEnum.Dlg_EACPrison_0500),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1033, MyDialoguesWrapperEnum.Dlg_EACPrison_0501),
new MyDialogueSentence(MyActorEnum.WALTHER, MySoundCuesEnum.Dlg_EacPrison_1034, MyDialoguesWrapperEnum.Dlg_EACPrison_0502),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1035, MyDialoguesWrapperEnum.Dlg_EACPrison_0503),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0600, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {

new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1036, MyDialoguesWrapperEnum.Dlg_EACPrison_0600),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1037, MyDialoguesWrapperEnum.Dlg_EACPrison_0601),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1038, MyDialoguesWrapperEnum.Dlg_EACPrison_0602),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1039, MyDialoguesWrapperEnum.Dlg_EACPrison_0603),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1040, MyDialoguesWrapperEnum.Dlg_EACPrison_0604),

                                                                                             }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0700, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1041, MyDialoguesWrapperEnum.Dlg_EACPrison_0700),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1042, MyDialoguesWrapperEnum.Dlg_EACPrison_0701),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1043, MyDialoguesWrapperEnum.Dlg_EACPrison_0702),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1044, MyDialoguesWrapperEnum.Dlg_EACPrison_0703),


                                                                                             }
                                                                 ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0800, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1045, MyDialoguesWrapperEnum.Dlg_EACPrison_0800),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1046, MyDialoguesWrapperEnum.Dlg_EACPrison_0801),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1047, MyDialoguesWrapperEnum.Dlg_EACPrison_0802),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1048, MyDialoguesWrapperEnum.Dlg_EACPrison_0803),


                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_0900, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1049, MyDialoguesWrapperEnum.Dlg_EACPrison_0900),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1050, MyDialoguesWrapperEnum.Dlg_EACPrison_0901),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1051, MyDialoguesWrapperEnum.Dlg_EACPrison_0902),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacPrison_1052, MyDialoguesWrapperEnum.Dlg_EACPrison_0903),
    

                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_1000, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1053, MyDialoguesWrapperEnum.Dlg_EACPrison_1000),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1054, MyDialoguesWrapperEnum.Dlg_EACPrison_1001),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1055, MyDialoguesWrapperEnum.Dlg_EACPrison_1002),


                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_1100, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1056, MyDialoguesWrapperEnum.Dlg_EACPrison_1100),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1057, MyDialoguesWrapperEnum.Dlg_EACPrison_1101),


                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_1200, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1058, MyDialoguesWrapperEnum.Dlg_EACPrison_1200),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1059, MyDialoguesWrapperEnum.Dlg_EACPrison_1201),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1060, MyDialoguesWrapperEnum.Dlg_EACPrison_1202),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1061, MyDialoguesWrapperEnum.Dlg_EACPrison_1203),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1062, MyDialoguesWrapperEnum.Dlg_EACPrison_1204),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1063, MyDialoguesWrapperEnum.Dlg_EACPrison_1205),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1064, MyDialoguesWrapperEnum.Dlg_EACPrison_1206),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1065, MyDialoguesWrapperEnum.Dlg_EACPrison_1207),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1066, MyDialoguesWrapperEnum.Dlg_EACPrison_1208),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1067, MyDialoguesWrapperEnum.Dlg_EACPrison_1209),

                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_1250, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
//Shh we have company
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacPrison_1068, MyDialoguesWrapperEnum.Dlg_EACPrison_1210),
                                                                                             }));

            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_1300, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1069, MyDialoguesWrapperEnum.Dlg_EACPrison_1300),


                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_1400, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1070, MyDialoguesWrapperEnum.Dlg_EACPrison_1400),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1071, MyDialoguesWrapperEnum.Dlg_EACPrison_1401),


                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_1500, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1072, MyDialoguesWrapperEnum.Dlg_EACPrison_1500),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1073, MyDialoguesWrapperEnum.Dlg_EACPrison_1501),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1074, MyDialoguesWrapperEnum.Dlg_EACPrison_1502),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1075, MyDialoguesWrapperEnum.Dlg_EACPrison_1503),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1076, MyDialoguesWrapperEnum.Dlg_EACPrison_1504),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacPrison_1077, MyDialoguesWrapperEnum.Dlg_EACPrison_1505),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1078, MyDialoguesWrapperEnum.Dlg_EACPrison_1506),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacPrison_1079, MyDialoguesWrapperEnum.Dlg_EACPrison_1507),
//new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1080, MyDialoguesWrapperEnum.Dlg_EACPrison_1508),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1081, MyDialoguesWrapperEnum.Dlg_EACPrison_1509),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1082, MyDialoguesWrapperEnum.Dlg_EACPrison_1510),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacPrison_1083, MyDialoguesWrapperEnum.Dlg_EACPrison_1511),


                                                                                             }
                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.EAC_PRISON_1600, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                             {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1084, MyDialoguesWrapperEnum.Dlg_EACPrison_1600),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1085, MyDialoguesWrapperEnum.Dlg_EACPrison_1601),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacPrison_1086, MyDialoguesWrapperEnum.Dlg_EACPrison_1602),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1087, MyDialoguesWrapperEnum.Dlg_EACPrison_1603),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacPrison_1088, MyDialoguesWrapperEnum.Dlg_EACPrison_1604),


                                                                                             }
                                                     ));


            #endregion

            #region Research Vessel
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0100_INTRO, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1000, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1000),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1001, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1001),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1002, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1002),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1003, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1003),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1004, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1004),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1005, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1005),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1006, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1006),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ResearchVessel_1007, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1007),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1008, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1008),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1009, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1009),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1010, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1010),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1011, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1011),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1012, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1012),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0200_INCOMINGSHIPS, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.RESEARCH_VESSEL_CAPTAIN, MySoundCuesEnum.Dlg_ResearchVessel_1013, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1013),
                    new MyDialogueSentence(MyActorEnum.RESEARCH_VESSEL_CAPTAIN, MySoundCuesEnum.Dlg_ResearchVessel_1014, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1014),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1015, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1015),
                    new MyDialogueSentence(MyActorEnum.RESEARCH_VESSEL_CAPTAIN, MySoundCuesEnum.Dlg_ResearchVessel_1016, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1016),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1017, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1017),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0250_YOUASKEDFORIT, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.RESEARCH_VESSEL_CAPTAIN, MySoundCuesEnum.Dlg_ResearchVessel_1018, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1018),
                }
            ));
                                
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0300_FORTIFIED, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1019, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1019),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1020, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1020),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ResearchVessel_1021, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1021),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1022, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1022),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1023, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1023),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1024, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1024),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0400_LOOKATTHIS, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1025, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1025),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1026, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1026),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1027, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1027),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ResearchVessel_1028, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1028),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0500_FIRSTPARTS, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1029, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1029),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1030, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1030),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1031, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1031),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0600_SECONDPARTS, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1032, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1032),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1033, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1033),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0700_THIRDPARTS, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1034, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1034),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1035, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1035),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1036, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1036),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1037, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1037),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ResearchVessel_1038, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1038),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1039, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1039),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ResearchVessel_1040, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1040),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0800_FORCEFIELDDOWN, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1041, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1041),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_0900_FOURTHPARTS, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1042, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1042),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1043, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1043),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1044, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1044),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1045, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1045),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1046, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1046),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_ResearchVessel_1047, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1047),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1048, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1048),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ResearchVessel_1049, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1049),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_1000_FIRSTHUB, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ResearchVessel_1050, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1050),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1051, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1051),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1052, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1052),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_1100_SECONDHUB, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_ResearchVessel_1053, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1053),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_ResearchVessel_1054, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1054),                
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.RESEARCH_VESSEL_1200_THIRDHUB, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_ResearchVessel_1055, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1055),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_ResearchVessel_1056, MyDialoguesWrapperEnum.Dlg_ResearchVessel_1056),
                }
            ));
            #endregion

            #region Barths Moon 3

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0100, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1000, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0100),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1001, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0101),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_BarthsMoonPlant_1002, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0102),
new MyDialogueSentence(MyActorEnum.RAIDER_LEADER, MySoundCuesEnum.Dlg_BarthsMoonPlant_1003, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0103),
new MyDialogueSentence(MyActorEnum.RAIDER_NAVIGATOR, MySoundCuesEnum.Dlg_BarthsMoonPlant_1004, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0104),
new MyDialogueSentence(MyActorEnum.RAIDER_LEADER, MySoundCuesEnum.Dlg_BarthsMoonPlant_1005, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0105),
new MyDialogueSentence(MyActorEnum.RAIDER_LEADER, MySoundCuesEnum.Dlg_BarthsMoonPlant_1006, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0106),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1007, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0107),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1008, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0108),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1009, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0109),

                                                                                 }
                                                                             ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0200, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
                                                                                     new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1010, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0200),
                                                                                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1011, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0201),
                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1012, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0202),
                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1013, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0203),
                                                                                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1014, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0204),
                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1015, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0205),
                                                                                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1016, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0206),
                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1017, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0207),
                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1018, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0208),
                                                                                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1019, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0209),
                                                                                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1020, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0210),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0300, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1021, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0300),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1022, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0301),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1023, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0302),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1024, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0303),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1025, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0304),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1026, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0305),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1027, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0306),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1028, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0307),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1029, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0308),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1030, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0309),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1031, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0310),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1032, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0311),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0400, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1033, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0400),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1034, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0401),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_BarthsMoonPlant_1035, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0402),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1036, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0403),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1037, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0404),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0500, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
                                                                                     new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1038, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0500),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1039, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0501),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1040, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0502),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1041, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0503),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0600, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.RAIDER_LEADER, MySoundCuesEnum.Dlg_BarthsMoonPlant_1042, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0600),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1043, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0601),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1044, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0602),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1045, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0603),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1046, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0604),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1047, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0605),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1048, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0606),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1049, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0607),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_BarthsMoonPlant_1050, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0608),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_BarthsMoonPlant_1051, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0609),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1052, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0610),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0700, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1053, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0700),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0800, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1054, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0800),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1055, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0801),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1056, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0802),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1057, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0803),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1058, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0804),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1059, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0805),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_0900, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1060, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0900),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1061, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_0901),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_1000, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1062, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1000),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1063, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1001),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1064, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1002),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1065, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1003),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1066, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1004),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1067, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1005),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1068, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1006),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1069, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1007),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1070, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1008),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1071, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1009),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1072, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1010),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1073, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1011),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1074, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1012),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1075, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1013),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1076, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1014),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1077, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1015),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1078, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1016),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1079, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1017),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1080, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1018),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1081, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1019),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1082, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1020),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1083, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1021),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1084, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1022),

                                                                                 }
                                                                 ));
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_1100, new MyDialogue(
                                                                 new MyDialogueSentence[]
                                                                                 {
                                                                                     new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1085, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1100),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1086, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1101),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1087, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1102),

                                                                                 }
                                                                 ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_PLANT_1200, new MyDialogue(
                                                     new MyDialogueSentence[]
                                                                                 {
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1088, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1200),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1089, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1201),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1090, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1202),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1091, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1203),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1092, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1204),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1093, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1205),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1094, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1206),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1095, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1207),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1096, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1208),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1097, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1209),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1098, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1210),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1099, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1211),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1100, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1212),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1101, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1213),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1102, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1214),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1103, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1215),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1104, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1216),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1105, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1217),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1106, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1218),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1107, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1219),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1108, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1220),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1109, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1221),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1110, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1222),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1111, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1223),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1112, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1224),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1113, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1225),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1114, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1226),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1115, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1227),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1116, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1228),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1117, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1229),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1118, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1230),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1119, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1231),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1120, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1232),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1121, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1233),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonPlant_1122, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1234),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonPlant_1123, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1235),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1124, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1236),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_BarthsMoonPlant_1125, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1237),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_BarthsMoonPlant_1126, MyDialoguesWrapperEnum.Dlg_BarthsMoonPlant_1238),

                                                                                 }
                                                     ));
            #endregion

            #region Fort Valiant C

            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0100_BEGIN, new MyDialogue(
                                                                             new MyDialogueSentence[]
                                                                             {
                                                                                new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1000, MyDialoguesWrapperEnum.Dlg_FortValiantC_0100),
                                                                                new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1001, MyDialoguesWrapperEnum.Dlg_FortValiantC_0101),
                                                                             }
                                                                             ));


            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0100, new MyDialogue(
                                                                                         new MyDialogueSentence[]
                                                                                         {
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1002, MyDialoguesWrapperEnum.Dlg_FortValiantC_0102),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantC_1003, MyDialoguesWrapperEnum.Dlg_FortValiantC_0103),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1004, MyDialoguesWrapperEnum.Dlg_FortValiantC_0104),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantC_1005, MyDialoguesWrapperEnum.Dlg_FortValiantC_0105),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantC_1006, MyDialoguesWrapperEnum.Dlg_FortValiantC_0106),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1007, MyDialoguesWrapperEnum.Dlg_FortValiantC_0107),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1008, MyDialoguesWrapperEnum.Dlg_FortValiantC_0108),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantC_1009, MyDialoguesWrapperEnum.Dlg_FortValiantC_0109),
new MyDialogueSentence(MyActorEnum.CEDRIC, MySoundCuesEnum.Dlg_FortValiantC_1010, MyDialoguesWrapperEnum.Dlg_FortValiantC_0110),
                                                                                             }
                                                                                         ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0200, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1011, MyDialoguesWrapperEnum.Dlg_FortValiantC_0200),
new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1012, MyDialoguesWrapperEnum.Dlg_FortValiantC_0201),
//new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1013, MyDialoguesWrapperEnum.Dlg_FortValiantC_0202),
//new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1014, MyDialoguesWrapperEnum.Dlg_FortValiantC_0203),
//new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_FortValiantC_1015, MyDialoguesWrapperEnum.Dlg_FortValiantC_0204),
//new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1016, MyDialoguesWrapperEnum.Dlg_FortValiantC_0205),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1017, MyDialoguesWrapperEnum.Dlg_FortValiantC_0206),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0300, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1018, MyDialoguesWrapperEnum.Dlg_FortValiantC_0300),
new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1019, MyDialoguesWrapperEnum.Dlg_FortValiantC_0301),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0400, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.PATRIARCH_DAGONET, MySoundCuesEnum.Dlg_FortValiantC_1020, MyDialoguesWrapperEnum.Dlg_FortValiantC_0400),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1021, MyDialoguesWrapperEnum.Dlg_FortValiantC_0401),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1022, MyDialoguesWrapperEnum.Dlg_FortValiantC_0402),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1023, MyDialoguesWrapperEnum.Dlg_FortValiantC_0403),
new MyDialogueSentence(MyActorEnum.PATRIARCH_LAMORAK, MySoundCuesEnum.Dlg_FortValiantC_1024, MyDialoguesWrapperEnum.Dlg_FortValiantC_0404),
new MyDialogueSentence(MyActorEnum.PATRIARCH_CARADOC, MySoundCuesEnum.Dlg_FortValiantC_1025, MyDialoguesWrapperEnum.Dlg_FortValiantC_0405),
new MyDialogueSentence(MyActorEnum.PATRIARCH_CARADOC, MySoundCuesEnum.Dlg_FortValiantC_1026, MyDialoguesWrapperEnum.Dlg_FortValiantC_0406),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1027, MyDialoguesWrapperEnum.Dlg_FortValiantC_0407),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1028, MyDialoguesWrapperEnum.Dlg_FortValiantC_0408),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1029, MyDialoguesWrapperEnum.Dlg_FortValiantC_0409),
new MyDialogueSentence(MyActorEnum.PATRIARCH_LAMORAK, MySoundCuesEnum.Dlg_FortValiantC_1030, MyDialoguesWrapperEnum.Dlg_FortValiantC_0410),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1031, MyDialoguesWrapperEnum.Dlg_FortValiantC_0411),
new MyDialogueSentence(MyActorEnum.PATRIARCH_CARADOC, MySoundCuesEnum.Dlg_FortValiantC_1032, MyDialoguesWrapperEnum.Dlg_FortValiantC_0412),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0500, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.UNKNOWN , MySoundCuesEnum.Dlg_FortValiantC_1033, MyDialoguesWrapperEnum.Dlg_FortValiantC_0500),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1034, MyDialoguesWrapperEnum.Dlg_FortValiantC_0501),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1035, MyDialoguesWrapperEnum.Dlg_FortValiantC_0502),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1036, MyDialoguesWrapperEnum.Dlg_FortValiantC_0503),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0600, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1037, MyDialoguesWrapperEnum.Dlg_FortValiantC_0600),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1038, MyDialoguesWrapperEnum.Dlg_FortValiantC_0601),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1039, MyDialoguesWrapperEnum.Dlg_FortValiantC_0602),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1040, MyDialoguesWrapperEnum.Dlg_FortValiantC_0603),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1041, MyDialoguesWrapperEnum.Dlg_FortValiantC_0604),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1042, MyDialoguesWrapperEnum.Dlg_FortValiantC_0605),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1043, MyDialoguesWrapperEnum.Dlg_FortValiantC_0606),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1044, MyDialoguesWrapperEnum.Dlg_FortValiantC_0607),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiantC_1045, MyDialoguesWrapperEnum.Dlg_FortValiantC_0608),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1046, MyDialoguesWrapperEnum.Dlg_FortValiantC_0609),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1047, MyDialoguesWrapperEnum.Dlg_FortValiantC_0610),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1048, MyDialoguesWrapperEnum.Dlg_FortValiantC_0611),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1049, MyDialoguesWrapperEnum.Dlg_FortValiantC_0612),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1050, MyDialoguesWrapperEnum.Dlg_FortValiantC_0613),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1051, MyDialoguesWrapperEnum.Dlg_FortValiantC_0614),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1052, MyDialoguesWrapperEnum.Dlg_FortValiantC_0615),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1053, MyDialoguesWrapperEnum.Dlg_FortValiantC_0616),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1054, MyDialoguesWrapperEnum.Dlg_FortValiantC_0617),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1055, MyDialoguesWrapperEnum.Dlg_FortValiantC_0618),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1056, MyDialoguesWrapperEnum.Dlg_FortValiantC_0619),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1057, MyDialoguesWrapperEnum.Dlg_FortValiantC_0620),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1058, MyDialoguesWrapperEnum.Dlg_FortValiantC_0621),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1059, MyDialoguesWrapperEnum.Dlg_FortValiantC_0622),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1060, MyDialoguesWrapperEnum.Dlg_FortValiantC_0623),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1061, MyDialoguesWrapperEnum.Dlg_FortValiantC_0624),
//new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1062, MyDialoguesWrapperEnum.Dlg_FortValiantC_0625),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1063, MyDialoguesWrapperEnum.Dlg_FortValiantC_0626),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1064, MyDialoguesWrapperEnum.Dlg_FortValiantC_0627),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1065, MyDialoguesWrapperEnum.Dlg_FortValiantC_0628),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0700, new MyDialogue(
                new MyDialogueSentence[]{
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1066, MyDialoguesWrapperEnum.Dlg_FortValiantC_0700),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_FortValiantC_1067, MyDialoguesWrapperEnum.Dlg_FortValiantC_0701),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0800, new MyDialogue(
                new MyDialogueSentence[]{
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1068, MyDialoguesWrapperEnum.Dlg_FortValiantC_0800),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_0900, new MyDialogue(
                new MyDialogueSentence[]{
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1069, MyDialoguesWrapperEnum.Dlg_FortValiantC_0900),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1000, new MyDialogue(
                new MyDialogueSentence[]{
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1070, MyDialoguesWrapperEnum.Dlg_FortValiantC_1000),
                }
            ));
            //m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1100, new MyDialogue(
            //    new MyDialogueSentence[] {
            //        new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1071, MyDialoguesWrapperEnum.Dlg_FortValiantC_1100),
            //    }
            //));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1200, new MyDialogue(
                new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1072, MyDialoguesWrapperEnum.Dlg_FortValiantC_1200),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1300, new MyDialogue(
                new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1073, MyDialoguesWrapperEnum.Dlg_FortValiantC_1300),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1400, new MyDialogue(
                new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1074, MyDialoguesWrapperEnum.Dlg_FortValiantC_1400),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1075, MyDialoguesWrapperEnum.Dlg_FortValiantC_1401),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1076, MyDialoguesWrapperEnum.Dlg_FortValiantC_1402),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1500, new MyDialogue(
                new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1077, MyDialoguesWrapperEnum.Dlg_FortValiantC_1500),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1078, MyDialoguesWrapperEnum.Dlg_FortValiantC_1501),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1600, new MyDialogue(
                new MyDialogueSentence[] {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1079, MyDialoguesWrapperEnum.Dlg_FortValiantC_1600),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1080, MyDialoguesWrapperEnum.Dlg_FortValiantC_1601),
                }
            ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1700, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1081, MyDialoguesWrapperEnum.Dlg_FortValiantC_1700),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1082, MyDialoguesWrapperEnum.Dlg_FortValiantC_1701),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1800, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1083, MyDialoguesWrapperEnum.Dlg_FortValiantC_1800),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_1900, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1084, MyDialoguesWrapperEnum.Dlg_FortValiantC_1900),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_2000, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1085, MyDialoguesWrapperEnum.Dlg_FortValiantC_2000),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_2100, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1086, MyDialoguesWrapperEnum.Dlg_FortValiantC_2100),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1087, MyDialoguesWrapperEnum.Dlg_FortValiantC_2101),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_2200, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                        

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_2300, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                                         new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1088, MyDialoguesWrapperEnum.Dlg_FortValiantC_2201),
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1089, MyDialoguesWrapperEnum.Dlg_FortValiantC_2300),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1090, MyDialoguesWrapperEnum.Dlg_FortValiantC_2301),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1091, MyDialoguesWrapperEnum.Dlg_FortValiantC_2302),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1092, MyDialoguesWrapperEnum.Dlg_FortValiantC_2303),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1093, MyDialoguesWrapperEnum.Dlg_FortValiantC_2304),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1094, MyDialoguesWrapperEnum.Dlg_FortValiantC_2305),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_2400, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1095, MyDialoguesWrapperEnum.Dlg_FortValiantC_2400),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1096, MyDialoguesWrapperEnum.Dlg_FortValiantC_2401),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1097, MyDialoguesWrapperEnum.Dlg_FortValiantC_2402),
new MyDialogueSentence(MyActorEnum.SIR_GERAINT, MySoundCuesEnum.Dlg_FortValiantC_1098, MyDialoguesWrapperEnum.Dlg_FortValiantC_2403),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1099, MyDialoguesWrapperEnum.Dlg_FortValiantC_2404),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1100, MyDialoguesWrapperEnum.Dlg_FortValiantC_2405),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1101, MyDialoguesWrapperEnum.Dlg_FortValiantC_2406),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1102, MyDialoguesWrapperEnum.Dlg_FortValiantC_2407),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1103, MyDialoguesWrapperEnum.Dlg_FortValiantC_2408),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_2500, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1104, MyDialoguesWrapperEnum.Dlg_FortValiantC_2500),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1105, MyDialoguesWrapperEnum.Dlg_FortValiantC_2501),
new MyDialogueSentence(MyActorEnum.COUNCIL_GUARD, MySoundCuesEnum.Dlg_FortValiantC_1106, MyDialoguesWrapperEnum.Dlg_FortValiantC_2502),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.FORT_VALIANT_C_2600, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1107, MyDialoguesWrapperEnum.Dlg_FortValiantC_2600),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1108, MyDialoguesWrapperEnum.Dlg_FortValiantC_2601),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1109, MyDialoguesWrapperEnum.Dlg_FortValiantC_2602),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1110, MyDialoguesWrapperEnum.Dlg_FortValiantC_2603),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1111, MyDialoguesWrapperEnum.Dlg_FortValiantC_2604),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1112, MyDialoguesWrapperEnum.Dlg_FortValiantC_2605),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1113, MyDialoguesWrapperEnum.Dlg_FortValiantC_2606),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1114, MyDialoguesWrapperEnum.Dlg_FortValiantC_2607),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1115, MyDialoguesWrapperEnum.Dlg_FortValiantC_2608),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1116, MyDialoguesWrapperEnum.Dlg_FortValiantC_2609),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1117, MyDialoguesWrapperEnum.Dlg_FortValiantC_2610),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1118, MyDialoguesWrapperEnum.Dlg_FortValiantC_2611),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1119, MyDialoguesWrapperEnum.Dlg_FortValiantC_2612),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1120, MyDialoguesWrapperEnum.Dlg_FortValiantC_2613),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1121, MyDialoguesWrapperEnum.Dlg_FortValiantC_2614),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1122, MyDialoguesWrapperEnum.Dlg_FortValiantC_2615),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1123, MyDialoguesWrapperEnum.Dlg_FortValiantC_2616),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1124, MyDialoguesWrapperEnum.Dlg_FortValiantC_2617),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1125, MyDialoguesWrapperEnum.Dlg_FortValiantC_2618),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1126, MyDialoguesWrapperEnum.Dlg_FortValiantC_2619),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1127, MyDialoguesWrapperEnum.Dlg_FortValiantC_2620),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_FortValiantC_1128, MyDialoguesWrapperEnum.Dlg_FortValiantC_2621),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_FortValiantC_1129, MyDialoguesWrapperEnum.Dlg_FortValiantC_2622),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_FortValiantC_1130, MyDialoguesWrapperEnum.Dlg_FortValiantC_2623),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1131, MyDialoguesWrapperEnum.Dlg_FortValiantC_2624),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1132, MyDialoguesWrapperEnum.Dlg_FortValiantC_2625),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1133, MyDialoguesWrapperEnum.Dlg_FortValiantC_2626),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_FortValiantC_1134, MyDialoguesWrapperEnum.Dlg_FortValiantC_2627),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_FortValiantC_1135, MyDialoguesWrapperEnum.Dlg_FortValiantC_2628),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1136, MyDialoguesWrapperEnum.Dlg_FortValiantC_2629),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_FortValiantC_1137, MyDialoguesWrapperEnum.Dlg_FortValiantC_2630),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_FortValiantC_1138, MyDialoguesWrapperEnum.Dlg_FortValiantC_2631),

                                                                                             }
                                                                                                     ));

            #endregion

            #region Alien Gate
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0100, new MyDialogue(
                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1000, MyDialoguesWrapperEnum.Dlg_AlienGate_0100),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1001, MyDialoguesWrapperEnum.Dlg_AlienGate_0101),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1002, MyDialoguesWrapperEnum.Dlg_AlienGate_0102),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1003, MyDialoguesWrapperEnum.Dlg_AlienGate_0103),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1004, MyDialoguesWrapperEnum.Dlg_AlienGate_0104),

                                                                                             }
                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0200, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1005, MyDialoguesWrapperEnum.Dlg_AlienGate_0200),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1006, MyDialoguesWrapperEnum.Dlg_AlienGate_0201),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0300, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1007, MyDialoguesWrapperEnum.Dlg_AlienGate_0300),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1008, MyDialoguesWrapperEnum.Dlg_AlienGate_0301),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0400, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1009, MyDialoguesWrapperEnum.Dlg_AlienGate_0400),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1010, MyDialoguesWrapperEnum.Dlg_AlienGate_0401),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1011, MyDialoguesWrapperEnum.Dlg_AlienGate_0402),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1012, MyDialoguesWrapperEnum.Dlg_AlienGate_0403),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1013, MyDialoguesWrapperEnum.Dlg_AlienGate_0404),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0500, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1014, MyDialoguesWrapperEnum.Dlg_AlienGate_0500),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1015, MyDialoguesWrapperEnum.Dlg_AlienGate_0501),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1016, MyDialoguesWrapperEnum.Dlg_AlienGate_0502),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1017, MyDialoguesWrapperEnum.Dlg_AlienGate_0503),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1018, MyDialoguesWrapperEnum.Dlg_AlienGate_0504),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1019, MyDialoguesWrapperEnum.Dlg_AlienGate_0505),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1020, MyDialoguesWrapperEnum.Dlg_AlienGate_0506),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1021, MyDialoguesWrapperEnum.Dlg_AlienGate_0507),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1022, MyDialoguesWrapperEnum.Dlg_AlienGate_0508),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1023, MyDialoguesWrapperEnum.Dlg_AlienGate_0509),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1024, MyDialoguesWrapperEnum.Dlg_AlienGate_0510),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1025, MyDialoguesWrapperEnum.Dlg_AlienGate_0511),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1026, MyDialoguesWrapperEnum.Dlg_AlienGate_0512),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1027, MyDialoguesWrapperEnum.Dlg_AlienGate_0513),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1028, MyDialoguesWrapperEnum.Dlg_AlienGate_0514),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1029, MyDialoguesWrapperEnum.Dlg_AlienGate_0515),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1030, MyDialoguesWrapperEnum.Dlg_AlienGate_0516),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1031, MyDialoguesWrapperEnum.Dlg_AlienGate_0517),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1032, MyDialoguesWrapperEnum.Dlg_AlienGate_0518),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1033, MyDialoguesWrapperEnum.Dlg_AlienGate_0519),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1034, MyDialoguesWrapperEnum.Dlg_AlienGate_0520),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1035, MyDialoguesWrapperEnum.Dlg_AlienGate_0521),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1036, MyDialoguesWrapperEnum.Dlg_AlienGate_0522),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1037, MyDialoguesWrapperEnum.Dlg_AlienGate_0523),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1038, MyDialoguesWrapperEnum.Dlg_AlienGate_0524),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1038_2, MyDialoguesWrapperEnum.Dlg_AlienGate_0524_2),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1039, MyDialoguesWrapperEnum.Dlg_AlienGate_0525),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1040, MyDialoguesWrapperEnum.Dlg_AlienGate_0526),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1041, MyDialoguesWrapperEnum.Dlg_AlienGate_0527),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1042, MyDialoguesWrapperEnum.Dlg_AlienGate_0528),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1043, MyDialoguesWrapperEnum.Dlg_AlienGate_0529),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1044, MyDialoguesWrapperEnum.Dlg_AlienGate_0530),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1045, MyDialoguesWrapperEnum.Dlg_AlienGate_0531),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1046, MyDialoguesWrapperEnum.Dlg_AlienGate_0532),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1047, MyDialoguesWrapperEnum.Dlg_AlienGate_0533),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1048, MyDialoguesWrapperEnum.Dlg_AlienGate_0534),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1049, MyDialoguesWrapperEnum.Dlg_AlienGate_0535),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1050, MyDialoguesWrapperEnum.Dlg_AlienGate_0536),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1051, MyDialoguesWrapperEnum.Dlg_AlienGate_0537),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1052, MyDialoguesWrapperEnum.Dlg_AlienGate_0538),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1053, MyDialoguesWrapperEnum.Dlg_AlienGate_0539),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1054, MyDialoguesWrapperEnum.Dlg_AlienGate_0540),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1055, MyDialoguesWrapperEnum.Dlg_AlienGate_0541),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1056, MyDialoguesWrapperEnum.Dlg_AlienGate_0542),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1057, MyDialoguesWrapperEnum.Dlg_AlienGate_0543),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1058, MyDialoguesWrapperEnum.Dlg_AlienGate_0544),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1059, MyDialoguesWrapperEnum.Dlg_AlienGate_0545),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1060, MyDialoguesWrapperEnum.Dlg_AlienGate_0546),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1061, MyDialoguesWrapperEnum.Dlg_AlienGate_0547),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1062, MyDialoguesWrapperEnum.Dlg_AlienGate_0548),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1063, MyDialoguesWrapperEnum.Dlg_AlienGate_0549),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1064, MyDialoguesWrapperEnum.Dlg_AlienGate_0550),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0600, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1065, MyDialoguesWrapperEnum.Dlg_AlienGate_0600),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1066, MyDialoguesWrapperEnum.Dlg_AlienGate_0601),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1067, MyDialoguesWrapperEnum.Dlg_AlienGate_0602),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1067_2, MyDialoguesWrapperEnum.Dlg_AlienGate_0602_2),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1068, MyDialoguesWrapperEnum.Dlg_AlienGate_0603),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1069, MyDialoguesWrapperEnum.Dlg_AlienGate_0604),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0700, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1070, MyDialoguesWrapperEnum.Dlg_AlienGate_0700),


                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0800, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1071, MyDialoguesWrapperEnum.Dlg_AlienGate_0800),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1072, MyDialoguesWrapperEnum.Dlg_AlienGate_0801),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1073, MyDialoguesWrapperEnum.Dlg_AlienGate_0802),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1074, MyDialoguesWrapperEnum.Dlg_AlienGate_0803),



                                                                                             }
                                                                                         ));


m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0800B, new MyDialogue(
                                                                             new MyDialogueSentence[]{

new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1075, MyDialoguesWrapperEnum.Dlg_AlienGate_0804),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1076, MyDialoguesWrapperEnum.Dlg_AlienGate_0805),


                                                                                             }
                                                                             ));


m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_0900, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1077, MyDialoguesWrapperEnum.Dlg_AlienGate_0900),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1078, MyDialoguesWrapperEnum.Dlg_AlienGate_0901),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1079, MyDialoguesWrapperEnum.Dlg_AlienGate_0902),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1080, MyDialoguesWrapperEnum.Dlg_AlienGate_0903),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1081, MyDialoguesWrapperEnum.Dlg_AlienGate_0904),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1082, MyDialoguesWrapperEnum.Dlg_AlienGate_0905),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1083, MyDialoguesWrapperEnum.Dlg_AlienGate_0906),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1084, MyDialoguesWrapperEnum.Dlg_AlienGate_0907),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1085, MyDialoguesWrapperEnum.Dlg_AlienGate_0908),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1086, MyDialoguesWrapperEnum.Dlg_AlienGate_0909),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1087, MyDialoguesWrapperEnum.Dlg_AlienGate_0910),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1088, MyDialoguesWrapperEnum.Dlg_AlienGate_0911),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1089, MyDialoguesWrapperEnum.Dlg_AlienGate_0912),
new MyDialogueSentence(MyActorEnum.EVERYONE, MySoundCuesEnum.Dlg_AlienGate_1090, MyDialoguesWrapperEnum.Dlg_AlienGate_0913),


                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1000, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
          new MyDialogueSentence(MyActorEnum.RUSSIAN_GENERAL, MySoundCuesEnum.Dlg_AlienGate_1091, MyDialoguesWrapperEnum.Dlg_AlienGate_1000),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1092, MyDialoguesWrapperEnum.Dlg_AlienGate_1001),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1093, MyDialoguesWrapperEnum.Dlg_AlienGate_1002),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1094, MyDialoguesWrapperEnum.Dlg_AlienGate_1003),


                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1100, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1095, MyDialoguesWrapperEnum.Dlg_AlienGate_1100),



                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1200, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.RUSSIAN_GENERAL, MySoundCuesEnum.Dlg_AlienGate_1096, MyDialoguesWrapperEnum.Dlg_AlienGate_1200),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1097, MyDialoguesWrapperEnum.Dlg_AlienGate_1201),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1098, MyDialoguesWrapperEnum.Dlg_AlienGate_1202),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1099, MyDialoguesWrapperEnum.Dlg_AlienGate_1203),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1100, MyDialoguesWrapperEnum.Dlg_AlienGate_1204),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1300, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1101, MyDialoguesWrapperEnum.Dlg_AlienGate_1300),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1102, MyDialoguesWrapperEnum.Dlg_AlienGate_1301),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1103, MyDialoguesWrapperEnum.Dlg_AlienGate_1302),



                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1400, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1104, MyDialoguesWrapperEnum.Dlg_AlienGate_1400),


                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1500, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1105, MyDialoguesWrapperEnum.Dlg_AlienGate_1500),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1106, MyDialoguesWrapperEnum.Dlg_AlienGate_1501),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1107, MyDialoguesWrapperEnum.Dlg_AlienGate_1502),


                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1600, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1108, MyDialoguesWrapperEnum.Dlg_AlienGate_1600),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1109, MyDialoguesWrapperEnum.Dlg_AlienGate_1601),


                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1700, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1110, MyDialoguesWrapperEnum.Dlg_AlienGate_1700),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1111, MyDialoguesWrapperEnum.Dlg_AlienGate_1701),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1112, MyDialoguesWrapperEnum.Dlg_AlienGate_1702),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1113, MyDialoguesWrapperEnum.Dlg_AlienGate_1703),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1114, MyDialoguesWrapperEnum.Dlg_AlienGate_1704),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1800, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1115, MyDialoguesWrapperEnum.Dlg_AlienGate_1800),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_1900, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1116, MyDialoguesWrapperEnum.Dlg_AlienGate_1900),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2000, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1117, MyDialoguesWrapperEnum.Dlg_AlienGate_2000),
new MyDialogueSentence(MyActorEnum.EVERYONE, MySoundCuesEnum.Dlg_AlienGate_1118, MyDialoguesWrapperEnum.Dlg_AlienGate_2001),

                                                                                             }
                                                                                                     ));
//            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2100, new MyDialogue(
//                                                                                                     new MyDialogueSentence[]{
//                                                                                             new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1119, MyDialoguesWrapperEnum.Dlg_AlienGate_2100),
//new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1120, MyDialoguesWrapperEnum.Dlg_AlienGate_2101),

//                                                                                             }
//                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2200, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1121, MyDialoguesWrapperEnum.Dlg_AlienGate_2200),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1122, MyDialoguesWrapperEnum.Dlg_AlienGate_2201),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1123, MyDialoguesWrapperEnum.Dlg_AlienGate_2202),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1124, MyDialoguesWrapperEnum.Dlg_AlienGate_2203),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2300, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1125, MyDialoguesWrapperEnum.Dlg_AlienGate_2300),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1126, MyDialoguesWrapperEnum.Dlg_AlienGate_2301),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2400, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1127, MyDialoguesWrapperEnum.Dlg_AlienGate_2400),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1128, MyDialoguesWrapperEnum.Dlg_AlienGate_2401),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_AlienGate_1129, MyDialoguesWrapperEnum.Dlg_AlienGate_2402),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1130, MyDialoguesWrapperEnum.Dlg_AlienGate_2403),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2500, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1131, MyDialoguesWrapperEnum.Dlg_AlienGate_2500),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1132, MyDialoguesWrapperEnum.Dlg_AlienGate_2501),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1133, MyDialoguesWrapperEnum.Dlg_AlienGate_2502),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2600, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1134, MyDialoguesWrapperEnum.Dlg_AlienGate_2600),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1135, MyDialoguesWrapperEnum.Dlg_AlienGate_2601),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1136, MyDialoguesWrapperEnum.Dlg_AlienGate_2602),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2700, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1137, MyDialoguesWrapperEnum.Dlg_AlienGate_2700),
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1138, MyDialoguesWrapperEnum.Dlg_AlienGate_2701),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1139, MyDialoguesWrapperEnum.Dlg_AlienGate_2702),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1140, MyDialoguesWrapperEnum.Dlg_AlienGate_2703),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1141, MyDialoguesWrapperEnum.Dlg_AlienGate_2704),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1142, MyDialoguesWrapperEnum.Dlg_AlienGate_2705),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2800, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1143, MyDialoguesWrapperEnum.Dlg_AlienGate_2800),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1144, MyDialoguesWrapperEnum.Dlg_AlienGate_2801),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1145, MyDialoguesWrapperEnum.Dlg_AlienGate_2802),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1146, MyDialoguesWrapperEnum.Dlg_AlienGate_2803),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_2900, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1147, MyDialoguesWrapperEnum.Dlg_AlienGate_2900),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1148, MyDialoguesWrapperEnum.Dlg_AlienGate_2901),

                                                                                             }
                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_3000, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1149, MyDialoguesWrapperEnum.Dlg_AlienGate_3000),

                                                                                             }
                                                                                                     ));
//            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_3100, new MyDialogue(
//                                                                                                     new MyDialogueSentence[]{
//                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1150, MyDialoguesWrapperEnum.Dlg_AlienGate_3100),
//new MyDialogueSentence(MyActorEnum.FRANCIS_REEF, MySoundCuesEnum.Dlg_AlienGate_1151, MyDialoguesWrapperEnum.Dlg_AlienGate_3101),

//                                                                                             }
//                                                                                                     ));
            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_3200, new MyDialogue(
                                                                                                     new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1152, MyDialoguesWrapperEnum.Dlg_AlienGate_3200),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1153, MyDialoguesWrapperEnum.Dlg_AlienGate_3201),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_AlienGate_1154, MyDialoguesWrapperEnum.Dlg_AlienGate_3202),

                                                                                             }
                                                                                                     ));

            m_dialogues.Add((int)MyDialogueEnum.ALIEN_GATE_3300, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1155, MyDialoguesWrapperEnum.Dlg_AlienGate_3300),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1156, MyDialoguesWrapperEnum.Dlg_AlienGate_3301),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1157, MyDialoguesWrapperEnum.Dlg_AlienGate_3302),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_AlienGate_1158, MyDialoguesWrapperEnum.Dlg_AlienGate_3303),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_AlienGate_1159, MyDialoguesWrapperEnum.Dlg_AlienGate_3304),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_AlienGate_1160, MyDialoguesWrapperEnum.Dlg_AlienGate_3305),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1161, MyDialoguesWrapperEnum.Dlg_AlienGate_3306),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_AlienGate_1162, MyDialoguesWrapperEnum.Dlg_AlienGate_3307),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_AlienGate_1163, MyDialoguesWrapperEnum.Dlg_AlienGate_3308),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_AlienGate_1164, MyDialoguesWrapperEnum.Dlg_AlienGate_3309),
new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_AlienGate_1165, MyDialoguesWrapperEnum.Dlg_AlienGate_3310),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_AlienGate_1166, MyDialoguesWrapperEnum.Dlg_AlienGate_3311),


                                                                                             }
                                                                                         ));

            #endregion

            #region Pirate Base
            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0100, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1000, MyDialoguesWrapperEnum.Dlg_PirateBase_0101),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1001, MyDialoguesWrapperEnum.Dlg_PirateBase_0102),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1002, MyDialoguesWrapperEnum.Dlg_PirateBase_0103),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1003, MyDialoguesWrapperEnum.Dlg_PirateBase_0104),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0200, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1004, MyDialoguesWrapperEnum.Dlg_PirateBase_0201),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1005, MyDialoguesWrapperEnum.Dlg_PirateBase_0202),
                    new MyDialogueSentence(MyActorEnum.PIRATE_MALE, MySoundCuesEnum.Dlg_PirateBase_1006, MyDialoguesWrapperEnum.Dlg_PirateBase_0203),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1007, MyDialoguesWrapperEnum.Dlg_PirateBase_0204),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1008, MyDialoguesWrapperEnum.Dlg_PirateBase_0205),
                    new MyDialogueSentence(MyActorEnum.PIRATE_MALE, MySoundCuesEnum.Dlg_PirateBase_1009, MyDialoguesWrapperEnum.Dlg_PirateBase_0206),
                    new MyDialogueSentence(MyActorEnum.PIRATE_MALE, MySoundCuesEnum.Dlg_PirateBase_1010, MyDialoguesWrapperEnum.Dlg_PirateBase_0207),
                    new MyDialogueSentence(MyActorEnum.PIRATE_MALE, MySoundCuesEnum.Dlg_PirateBase_1011, MyDialoguesWrapperEnum.Dlg_PirateBase_0208),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1012, MyDialoguesWrapperEnum.Dlg_PirateBase_0209),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1013, MyDialoguesWrapperEnum.Dlg_PirateBase_0210),
                    //new MyDialogueSentence(MyActorEnum.PIRATE_MALE, MySoundCuesEnum.Dlg_PirateBase_1014, MyDialoguesWrapperEnum.Dlg_PirateBase_0211),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0300, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1015, MyDialoguesWrapperEnum.Dlg_PirateBase_0301),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1016, MyDialoguesWrapperEnum.Dlg_PirateBase_0302),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0400, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1017, MyDialoguesWrapperEnum.Dlg_PirateBase_0401),
                    new MyDialogueSentence(MyActorEnum.PIRATE2_MALE, MySoundCuesEnum.Dlg_PirateBase_1018, MyDialoguesWrapperEnum.Dlg_PirateBase_0402),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1019, MyDialoguesWrapperEnum.Dlg_PirateBase_0403),
                    new MyDialogueSentence(MyActorEnum.PIRATE2_MALE, MySoundCuesEnum.Dlg_PirateBase_1020, MyDialoguesWrapperEnum.Dlg_PirateBase_0404),
                    new MyDialogueSentence(MyActorEnum.PIRATE2_MALE, MySoundCuesEnum.Dlg_PirateBase_1021, MyDialoguesWrapperEnum.Dlg_PirateBase_0405),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1022, MyDialoguesWrapperEnum.Dlg_PirateBase_0406),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1023, MyDialoguesWrapperEnum.Dlg_PirateBase_0407),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1024, MyDialoguesWrapperEnum.Dlg_PirateBase_0408),
                    new MyDialogueSentence(MyActorEnum.PIRATE2_MALE, MySoundCuesEnum.Dlg_PirateBase_1025, MyDialoguesWrapperEnum.Dlg_PirateBase_0409),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0500, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1026, MyDialoguesWrapperEnum.Dlg_PirateBase_0501),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1027, MyDialoguesWrapperEnum.Dlg_PirateBase_0502),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1028, MyDialoguesWrapperEnum.Dlg_PirateBase_0503),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1029, MyDialoguesWrapperEnum.Dlg_PirateBase_0504),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1030, MyDialoguesWrapperEnum.Dlg_PirateBase_0505, pauseBefore_ms: 300),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1031, MyDialoguesWrapperEnum.Dlg_PirateBase_0506),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1032, MyDialoguesWrapperEnum.Dlg_PirateBase_0507),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1033, MyDialoguesWrapperEnum.Dlg_PirateBase_0508),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1034, MyDialoguesWrapperEnum.Dlg_PirateBase_0509),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1035, MyDialoguesWrapperEnum.Dlg_PirateBase_0510),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0600, new MyDialogue(
                new MyDialogueSentence[]
                {
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1036, MyDialoguesWrapperEnum.Dlg_PirateBase_0601),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1037, MyDialoguesWrapperEnum.Dlg_PirateBase_0602),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1038, MyDialoguesWrapperEnum.Dlg_PirateBase_0603),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1039, MyDialoguesWrapperEnum.Dlg_PirateBase_0604),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1040, MyDialoguesWrapperEnum.Dlg_PirateBase_0605),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1041, MyDialoguesWrapperEnum.Dlg_PirateBase_0606),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1042, MyDialoguesWrapperEnum.Dlg_PirateBase_0607),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1043, MyDialoguesWrapperEnum.Dlg_PirateBase_0608),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1044, MyDialoguesWrapperEnum.Dlg_PirateBase_0609),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1045, MyDialoguesWrapperEnum.Dlg_PirateBase_0610),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1046, MyDialoguesWrapperEnum.Dlg_PirateBase_0611),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1047, MyDialoguesWrapperEnum.Dlg_PirateBase_0612),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1048, MyDialoguesWrapperEnum.Dlg_PirateBase_0613),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1049, MyDialoguesWrapperEnum.Dlg_PirateBase_0614),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1050, MyDialoguesWrapperEnum.Dlg_PirateBase_0615),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1051, MyDialoguesWrapperEnum.Dlg_PirateBase_0616),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1052, MyDialoguesWrapperEnum.Dlg_PirateBase_0617),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1053, MyDialoguesWrapperEnum.Dlg_PirateBase_0618),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1054, MyDialoguesWrapperEnum.Dlg_PirateBase_0619),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1055, MyDialoguesWrapperEnum.Dlg_PirateBase_0620),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1056, MyDialoguesWrapperEnum.Dlg_PirateBase_0621),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1057, MyDialoguesWrapperEnum.Dlg_PirateBase_0622),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1058, MyDialoguesWrapperEnum.Dlg_PirateBase_0623),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1059, MyDialoguesWrapperEnum.Dlg_PirateBase_0624),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1060, MyDialoguesWrapperEnum.Dlg_PirateBase_0625),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1061, MyDialoguesWrapperEnum.Dlg_PirateBase_0626),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1062, MyDialoguesWrapperEnum.Dlg_PirateBase_0627/*, pauseBefore_ms: 300*/),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1063, MyDialoguesWrapperEnum.Dlg_PirateBase_0628),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0700, new MyDialogue(
                new MyDialogueSentence[]
                {
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1064, MyDialoguesWrapperEnum.Dlg_PirateBase_0701),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1065, MyDialoguesWrapperEnum.Dlg_PirateBase_0702),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1066, MyDialoguesWrapperEnum.Dlg_PirateBase_0703),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1067, MyDialoguesWrapperEnum.Dlg_PirateBase_0704),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1068, MyDialoguesWrapperEnum.Dlg_PirateBase_0705),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1069, MyDialoguesWrapperEnum.Dlg_PirateBase_0706),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1070, MyDialoguesWrapperEnum.Dlg_PirateBase_0707),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1071, MyDialoguesWrapperEnum.Dlg_PirateBase_0708),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1072, MyDialoguesWrapperEnum.Dlg_PirateBase_0709),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0800, new MyDialogue(
                new MyDialogueSentence[]
                {
                    // ** dialogue changes
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1064, MyDialoguesWrapperEnum.Dlg_PirateBase_0701),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1068, MyDialoguesWrapperEnum.Dlg_PirateBase_0705),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1069, MyDialoguesWrapperEnum.Dlg_PirateBase_0706),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1070, MyDialoguesWrapperEnum.Dlg_PirateBase_0707),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1071, MyDialoguesWrapperEnum.Dlg_PirateBase_0708),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1072, MyDialoguesWrapperEnum.Dlg_PirateBase_0709),
                    // ****

                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1073, MyDialoguesWrapperEnum.Dlg_PirateBase_0801),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1074, MyDialoguesWrapperEnum.Dlg_PirateBase_0802),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1075, MyDialoguesWrapperEnum.Dlg_PirateBase_0803),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1076, MyDialoguesWrapperEnum.Dlg_PirateBase_0804),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1077, MyDialoguesWrapperEnum.Dlg_PirateBase_0805),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_0900, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1078, MyDialoguesWrapperEnum.Dlg_PirateBase_0901),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1079, MyDialoguesWrapperEnum.Dlg_PirateBase_0902),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1080, MyDialoguesWrapperEnum.Dlg_PirateBase_0903),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1081, MyDialoguesWrapperEnum.Dlg_PirateBase_0904),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1082, MyDialoguesWrapperEnum.Dlg_PirateBase_0905),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_1000, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1083, MyDialoguesWrapperEnum.Dlg_PirateBase_1001),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1084, MyDialoguesWrapperEnum.Dlg_PirateBase_1002),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1085, MyDialoguesWrapperEnum.Dlg_PirateBase_1003),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1086, MyDialoguesWrapperEnum.Dlg_PirateBase_1004),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_1100, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.CLEGG, MySoundCuesEnum.Dlg_PirateBase_1087, MyDialoguesWrapperEnum.Dlg_PirateBase_1101),
                    new MyDialogueSentence(MyActorEnum.VANE, MySoundCuesEnum.Dlg_PirateBase_1088, MyDialoguesWrapperEnum.Dlg_PirateBase_1102),
                    new MyDialogueSentence(MyActorEnum.GORG, MySoundCuesEnum.Dlg_PirateBase_1089, MyDialoguesWrapperEnum.Dlg_PirateBase_1103),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1090, MyDialoguesWrapperEnum.Dlg_PirateBase_1104),
                    new MyDialogueSentence(MyActorEnum.CLEGG, MySoundCuesEnum.Dlg_PirateBase_1091, MyDialoguesWrapperEnum.Dlg_PirateBase_1105),
                    new MyDialogueSentence(MyActorEnum.VANE, MySoundCuesEnum.Dlg_PirateBase_1092, MyDialoguesWrapperEnum.Dlg_PirateBase_1106),
                    new MyDialogueSentence(MyActorEnum.GORG, MySoundCuesEnum.Dlg_PirateBase_1093, MyDialoguesWrapperEnum.Dlg_PirateBase_1107),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1094, MyDialoguesWrapperEnum.Dlg_PirateBase_1108),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1095, MyDialoguesWrapperEnum.Dlg_PirateBase_1109),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1096, MyDialoguesWrapperEnum.Dlg_PirateBase_1110),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_1200, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1097, MyDialoguesWrapperEnum.Dlg_PirateBase_1201),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1098, MyDialoguesWrapperEnum.Dlg_PirateBase_1202),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1099, MyDialoguesWrapperEnum.Dlg_PirateBase_1203),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1100, MyDialoguesWrapperEnum.Dlg_PirateBase_1204),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1101, MyDialoguesWrapperEnum.Dlg_PirateBase_1205),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1102, MyDialoguesWrapperEnum.Dlg_PirateBase_1206),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1103, MyDialoguesWrapperEnum.Dlg_PirateBase_1207),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1104, MyDialoguesWrapperEnum.Dlg_PirateBase_1208),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1105, MyDialoguesWrapperEnum.Dlg_PirateBase_1209),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1106, MyDialoguesWrapperEnum.Dlg_PirateBase_1210),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_1300, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1107, MyDialoguesWrapperEnum.Dlg_PirateBase_1301),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1108, MyDialoguesWrapperEnum.Dlg_PirateBase_1302),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1109, MyDialoguesWrapperEnum.Dlg_PirateBase_1303),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_1400, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1110, MyDialoguesWrapperEnum.Dlg_PirateBase_1401),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1111, MyDialoguesWrapperEnum.Dlg_PirateBase_1402),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1112, MyDialoguesWrapperEnum.Dlg_PirateBase_1403),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1113, MyDialoguesWrapperEnum.Dlg_PirateBase_1404),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1114, MyDialoguesWrapperEnum.Dlg_PirateBase_1405),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1115, MyDialoguesWrapperEnum.Dlg_PirateBase_1406),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1116, MyDialoguesWrapperEnum.Dlg_PirateBase_1407),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1117, MyDialoguesWrapperEnum.Dlg_PirateBase_1408),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1118, MyDialoguesWrapperEnum.Dlg_PirateBase_1409),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1119, MyDialoguesWrapperEnum.Dlg_PirateBase_1410),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1120, MyDialoguesWrapperEnum.Dlg_PirateBase_1411),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1121, MyDialoguesWrapperEnum.Dlg_PirateBase_1412),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1122, MyDialoguesWrapperEnum.Dlg_PirateBase_1413),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1123, MyDialoguesWrapperEnum.Dlg_PirateBase_1414),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1124, MyDialoguesWrapperEnum.Dlg_PirateBase_1415),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1125, MyDialoguesWrapperEnum.Dlg_PirateBase_1416),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1126, MyDialoguesWrapperEnum.Dlg_PirateBase_1417),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1127, MyDialoguesWrapperEnum.Dlg_PirateBase_1418),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1128, MyDialoguesWrapperEnum.Dlg_PirateBase_1419),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1129, MyDialoguesWrapperEnum.Dlg_PirateBase_1420),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1130, MyDialoguesWrapperEnum.Dlg_PirateBase_1421),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1131, MyDialoguesWrapperEnum.Dlg_PirateBase_1422),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1132, MyDialoguesWrapperEnum.Dlg_PirateBase_1423),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1133, MyDialoguesWrapperEnum.Dlg_PirateBase_1424),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1134, MyDialoguesWrapperEnum.Dlg_PirateBase_1425),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1135, MyDialoguesWrapperEnum.Dlg_PirateBase_1426),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1136, MyDialoguesWrapperEnum.Dlg_PirateBase_1427),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1137, MyDialoguesWrapperEnum.Dlg_PirateBase_1428),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1138, MyDialoguesWrapperEnum.Dlg_PirateBase_1429),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1139, MyDialoguesWrapperEnum.Dlg_PirateBase_1430),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1140, MyDialoguesWrapperEnum.Dlg_PirateBase_1431),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1141, MyDialoguesWrapperEnum.Dlg_PirateBase_1432),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1142, MyDialoguesWrapperEnum.Dlg_PirateBase_1433),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1143, MyDialoguesWrapperEnum.Dlg_PirateBase_1434),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1144, MyDialoguesWrapperEnum.Dlg_PirateBase_1435),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1145, MyDialoguesWrapperEnum.Dlg_PirateBase_1436),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1146, MyDialoguesWrapperEnum.Dlg_PirateBase_1437),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1147, MyDialoguesWrapperEnum.Dlg_PirateBase_1438),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1148, MyDialoguesWrapperEnum.Dlg_PirateBase_1439),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1149, MyDialoguesWrapperEnum.Dlg_PirateBase_1440),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1150, MyDialoguesWrapperEnum.Dlg_PirateBase_1441),
                    new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1151, MyDialoguesWrapperEnum.Dlg_PirateBase_1442),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1152, MyDialoguesWrapperEnum.Dlg_PirateBase_1443),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1153, MyDialoguesWrapperEnum.Dlg_PirateBase_1444),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1154, MyDialoguesWrapperEnum.Dlg_PirateBase_1445),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1155, MyDialoguesWrapperEnum.Dlg_PirateBase_1446),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1156, MyDialoguesWrapperEnum.Dlg_PirateBase_1447),
                    //new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_PirateBase_1157, MyDialoguesWrapperEnum.Dlg_PirateBase_1448),
               }
            ));

            m_dialogues.Add((int)MyDialogueEnum.PIRATE_BASE_1500, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_PirateBase_1158, MyDialoguesWrapperEnum.Dlg_PirateBase_1501),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_PirateBase_1159, MyDialoguesWrapperEnum.Dlg_PirateBase_1502),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_PirateBase_1160, MyDialoguesWrapperEnum.Dlg_PirateBase_1503),
               }
            ));
            #endregion

            #region Barths Moon Convince
            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_CONVINCE_0100, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1000, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0101),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1001, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0102),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1002, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0103),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1003, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0104),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1004, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0105),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1005, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0106),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1006, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0107),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1007, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0108),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1008, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0109),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1009, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0110),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1010, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0111),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1011, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0112),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1012, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0113),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1013, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0114),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1014, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0115),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1015, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0116),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1016, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0117),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1017, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0118),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1018, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0119),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1019, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0120),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1020, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0121),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1021, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0122),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_CONVINCE_0200, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1022, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0201),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1023, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0202),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1024, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0203),
                    //new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1025, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0204),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_CONVINCE_0300, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1026, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0301),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1027, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0302),
                    //new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1028, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0303),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1029, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0304),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1030, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0305),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1031, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0306),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1032, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0307),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1033, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0308),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1034, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0309),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1035, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0310),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1036, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0311),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1037, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0312),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1038, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0313),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1039, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0314),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1040, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0315),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1041, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0316),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1042, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0317),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1043, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0318),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1044, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0319),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1045, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0320),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1046, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0321),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1047, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0322),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1048, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0323),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1049, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0324),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1050, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0325),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1051, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0326),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1052, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0327),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1053, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0328),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1054, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0329),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1055, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0330),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1056, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0331),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1057, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0332),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1058, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0333),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1059, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0334),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1060, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0335),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1061, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0336/*, pauseBefore_ms: 300*/),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1062, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0337),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1063, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0338/*, pauseBefore_ms: 300*/),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1064, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0339),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1065, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0340),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1066, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0341),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1067, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0342),
                    //new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1068, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0343),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1069, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0344),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1070, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0345),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_CONVINCE_0400, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1071, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0401),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1072, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0402),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1073, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0403),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1074, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0404),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1075, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0405),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1076, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0406),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1077, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0407),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1078, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0408),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1079, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0409),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1080, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0410),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1081, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0411),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1082, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0412),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1083, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0413),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1084, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0414),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1085, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0415),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1086, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0416),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1087, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0417),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1088, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0418),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1089, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0419),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1090, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0420),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1091, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0421),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1092, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0422),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1093, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0423),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1094, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0424),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1095, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0425),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1096, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0426),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1097, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0427),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_CONVINCE_0500, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1098, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0501),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1099, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0502),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1100, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0503),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1101, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0504),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_CONVINCE_0600, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.RAIDER_LEADER, MySoundCuesEnum.Dlg_BarthsMoonConvince_1102, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0601),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1103, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0602),
                    new MyDialogueSentence(MyActorEnum.RAIDER_LEADER, MySoundCuesEnum.Dlg_BarthsMoonConvince_1104, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0603),
                    new MyDialogueSentence(MyActorEnum.RAIDER_LEADER, MySoundCuesEnum.Dlg_BarthsMoonConvince_1105, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0604),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1106, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0605),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.BARTHS_MOON_CONVINCE_0700, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1107, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0701),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1108, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0702),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_BarthsMoonConvince_1109, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0703),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_BarthsMoonConvince_1110, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0704),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_BarthsMoonConvince_1111, MyDialoguesWrapperEnum.Dlg_BarthsMoonConvince_0705),
                }
            ));
            #endregion

            #region Last Hope
            m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0100, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1000, MyDialoguesWrapperEnum.Dlg_LastHope_0100),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1001, MyDialoguesWrapperEnum.Dlg_LastHope_0101),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1002, MyDialoguesWrapperEnum.Dlg_LastHope_0102),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1003, MyDialoguesWrapperEnum.Dlg_LastHope_0103),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0200, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.SLAVER_BASE_CAPTAIN, MySoundCuesEnum.Dlg_LastHope_1004, MyDialoguesWrapperEnum.Dlg_LastHope_0200),
new MyDialogueSentence(MyActorEnum.SLAVER_BASE_CAPTAIN, MySoundCuesEnum.Dlg_LastHope_1005, MyDialoguesWrapperEnum.Dlg_LastHope_0201),
//new MyDialogueSentence(MyActorEnum.SLAVER_LEADER, MySoundCuesEnum.Dlg_LastHope_1006, MyDialoguesWrapperEnum.Dlg_LastHope_0202),
//new MyDialogueSentence(MyActorEnum.SLAVER_LEADER, MySoundCuesEnum.Dlg_LastHope_1007, MyDialoguesWrapperEnum.Dlg_LastHope_0203),
//new MyDialogueSentence(MyActorEnum.SLAVER_LEADER, MySoundCuesEnum.Dlg_LastHope_1008, MyDialoguesWrapperEnum.Dlg_LastHope_0204),
//new MyDialogueSentence(MyActorEnum.SLAVER_LEADER, MySoundCuesEnum.Dlg_LastHope_1009, MyDialoguesWrapperEnum.Dlg_LastHope_0205),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1010, MyDialoguesWrapperEnum.Dlg_LastHope_0206),
new MyDialogueSentence(MyActorEnum.SLAVER_LEADER, MySoundCuesEnum.Dlg_LastHope_1011, MyDialoguesWrapperEnum.Dlg_LastHope_0207),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1012, MyDialoguesWrapperEnum.Dlg_LastHope_0208),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1013, MyDialoguesWrapperEnum.Dlg_LastHope_0209),
new MyDialogueSentence(MyActorEnum.SLAVER_LEADER, MySoundCuesEnum.Dlg_LastHope_1014, MyDialoguesWrapperEnum.Dlg_LastHope_0210),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0300, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1015, MyDialoguesWrapperEnum.Dlg_LastHope_0300),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1016, MyDialoguesWrapperEnum.Dlg_LastHope_0301),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1017, MyDialoguesWrapperEnum.Dlg_LastHope_0302),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1018, MyDialoguesWrapperEnum.Dlg_LastHope_0303),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1019, MyDialoguesWrapperEnum.Dlg_LastHope_0304),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1020, MyDialoguesWrapperEnum.Dlg_LastHope_0305),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0400, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1021, MyDialoguesWrapperEnum.Dlg_LastHope_0400),
//new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1022, MyDialoguesWrapperEnum.Dlg_LastHope_0401),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1023, MyDialoguesWrapperEnum.Dlg_LastHope_0402),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1024, MyDialoguesWrapperEnum.Dlg_LastHope_0403),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1025, MyDialoguesWrapperEnum.Dlg_LastHope_0404),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1026, MyDialoguesWrapperEnum.Dlg_LastHope_0405),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1027, MyDialoguesWrapperEnum.Dlg_LastHope_0406),
//new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1028, MyDialoguesWrapperEnum.Dlg_LastHope_0407),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1029, MyDialoguesWrapperEnum.Dlg_LastHope_0408),
//new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1030, MyDialoguesWrapperEnum.Dlg_LastHope_0409),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1031, MyDialoguesWrapperEnum.Dlg_LastHope_0410),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0500, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1032, MyDialoguesWrapperEnum.Dlg_LastHope_0500),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1033, MyDialoguesWrapperEnum.Dlg_LastHope_0501),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1034, MyDialoguesWrapperEnum.Dlg_LastHope_0502),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1035, MyDialoguesWrapperEnum.Dlg_LastHope_0503),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0600, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1036, MyDialoguesWrapperEnum.Dlg_LastHope_0600),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1037, MyDialoguesWrapperEnum.Dlg_LastHope_0601),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1038, MyDialoguesWrapperEnum.Dlg_LastHope_0602),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1039, MyDialoguesWrapperEnum.Dlg_LastHope_0603),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1040, MyDialoguesWrapperEnum.Dlg_LastHope_0604),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1041, MyDialoguesWrapperEnum.Dlg_LastHope_0605),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1042, MyDialoguesWrapperEnum.Dlg_LastHope_0606),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1043, MyDialoguesWrapperEnum.Dlg_LastHope_0607),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1044, MyDialoguesWrapperEnum.Dlg_LastHope_0608),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1045, MyDialoguesWrapperEnum.Dlg_LastHope_0609),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0700, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1046, MyDialoguesWrapperEnum.Dlg_LastHope_0700),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1047, MyDialoguesWrapperEnum.Dlg_LastHope_0701),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1048, MyDialoguesWrapperEnum.Dlg_LastHope_0702),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1049, MyDialoguesWrapperEnum.Dlg_LastHope_0703),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1050, MyDialoguesWrapperEnum.Dlg_LastHope_0704),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1051, MyDialoguesWrapperEnum.Dlg_LastHope_0705),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1052, MyDialoguesWrapperEnum.Dlg_LastHope_0706),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1053, MyDialoguesWrapperEnum.Dlg_LastHope_0707),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1054, MyDialoguesWrapperEnum.Dlg_LastHope_0708),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1055, MyDialoguesWrapperEnum.Dlg_LastHope_0709),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1056, MyDialoguesWrapperEnum.Dlg_LastHope_0710),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1057, MyDialoguesWrapperEnum.Dlg_LastHope_0711),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1058, MyDialoguesWrapperEnum.Dlg_LastHope_0712),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1059, MyDialoguesWrapperEnum.Dlg_LastHope_0713),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1060, MyDialoguesWrapperEnum.Dlg_LastHope_0714),
//new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1061, MyDialoguesWrapperEnum.Dlg_LastHope_0715),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0800, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1061, MyDialoguesWrapperEnum.Dlg_LastHope_0800),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1062, MyDialoguesWrapperEnum.Dlg_LastHope_0801),
new MyDialogueSentence(MyActorEnum.SLAVER_COLE, MySoundCuesEnum.Dlg_LastHope_1063, MyDialoguesWrapperEnum.Dlg_LastHope_0900),
new MyDialogueSentence(MyActorEnum.SLAVER_JEFF, MySoundCuesEnum.Dlg_LastHope_1064, MyDialoguesWrapperEnum.Dlg_LastHope_0901),
new MyDialogueSentence(MyActorEnum.SLAVER_COLE, MySoundCuesEnum.Dlg_LastHope_1065, MyDialoguesWrapperEnum.Dlg_LastHope_0902),
/*new MyDialogueSentence(MyActorEnum.SLAVER_JEFF, MySoundCuesEnum.Dlg_LastHope_1066, MyDialoguesWrapperEnum.Dlg_LastHope_0903),
new MyDialogueSentence(MyActorEnum.SLAVER_JEFF, MySoundCuesEnum.Dlg_LastHope_1067, MyDialoguesWrapperEnum.Dlg_LastHope_0904),
//new MyDialogueSentence(MyActorEnum.SLAVER_JEFF, MySoundCuesEnum.Dlg_LastHope_1069, MyDialoguesWrapperEnum.Dlg_LastHope_0905),
new MyDialogueSentence(MyActorEnum.SLAVER_COLE, MySoundCuesEnum.Dlg_LastHope_1070, MyDialoguesWrapperEnum.Dlg_LastHope_0906),
new MyDialogueSentence(MyActorEnum.SLAVER_JEFF, MySoundCuesEnum.Dlg_LastHope_1071, MyDialoguesWrapperEnum.Dlg_LastHope_0907),
new MyDialogueSentence(MyActorEnum.SLAVER_COLE, MySoundCuesEnum.Dlg_LastHope_1072, MyDialoguesWrapperEnum.Dlg_LastHope_0908),
 */
new MyDialogueSentence(MyActorEnum.SLAVER_JEFF, MySoundCuesEnum.Dlg_LastHope_1073, MyDialoguesWrapperEnum.Dlg_LastHope_0909),
new MyDialogueSentence(MyActorEnum.SLAVER_JEFF, MySoundCuesEnum.Dlg_LastHope_1074, MyDialoguesWrapperEnum.Dlg_LastHope_0910),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1075, MyDialoguesWrapperEnum.Dlg_LastHope_1000),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1076, MyDialoguesWrapperEnum.Dlg_LastHope_1001),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1077, MyDialoguesWrapperEnum.Dlg_LastHope_1002),




new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1082, MyDialoguesWrapperEnum.Dlg_LastHope_1007),

                                                                                             }
                                                                                         ));
            /*
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_0900, new MyDialogue(
                                                                                         new MyDialogueSentence[]{

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1000, new MyDialogue(
                                                                                         new MyDialogueSentence[]{

                                                                                             }
                                                                                         ));*/
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1100, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.SLAVER_JEFF, MySoundCuesEnum.Dlg_LastHope_1083, MyDialoguesWrapperEnum.Dlg_LastHope_1100),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1200, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1088, MyDialoguesWrapperEnum.Dlg_LastHope_1200),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1300, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1089, MyDialoguesWrapperEnum.Dlg_LastHope_1300),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1090, MyDialoguesWrapperEnum.Dlg_LastHope_1301),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1091, MyDialoguesWrapperEnum.Dlg_LastHope_1302),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1400, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1092, MyDialoguesWrapperEnum.Dlg_LastHope_1400),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1093, MyDialoguesWrapperEnum.Dlg_LastHope_1401),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1094, MyDialoguesWrapperEnum.Dlg_LastHope_1402),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1095, MyDialoguesWrapperEnum.Dlg_LastHope_1403),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1500, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1096, MyDialoguesWrapperEnum.Dlg_LastHope_1500),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1600, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1097, MyDialoguesWrapperEnum.Dlg_LastHope_1600),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1700, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1098, MyDialoguesWrapperEnum.Dlg_LastHope_1700),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1099, MyDialoguesWrapperEnum.Dlg_LastHope_1701),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1100, MyDialoguesWrapperEnum.Dlg_LastHope_1702),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1800, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1101, MyDialoguesWrapperEnum.Dlg_LastHope_1800),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_1900, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
                                                                                             new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1102, MyDialoguesWrapperEnum.Dlg_LastHope_1900),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1103, MyDialoguesWrapperEnum.Dlg_LastHope_1901),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1104, MyDialoguesWrapperEnum.Dlg_LastHope_1902),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1105, MyDialoguesWrapperEnum.Dlg_LastHope_1903),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1106, MyDialoguesWrapperEnum.Dlg_LastHope_1904),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1107, MyDialoguesWrapperEnum.Dlg_LastHope_1905),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1108, MyDialoguesWrapperEnum.Dlg_LastHope_1906),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1109, MyDialoguesWrapperEnum.Dlg_LastHope_1907),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1110, MyDialoguesWrapperEnum.Dlg_LastHope_1908),

                                                                                             }
                                                                                         ));

m_dialogues.Add((int)MyDialogueEnum.LAST_HOPE_2000, new MyDialogue(
                                                                             new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1111, MyDialoguesWrapperEnum.Dlg_LastHope_2000),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1112, MyDialoguesWrapperEnum.Dlg_LastHope_2001),
//new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1113, MyDialoguesWrapperEnum.Dlg_LastHope_2002),
//new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1114, MyDialoguesWrapperEnum.Dlg_LastHope_2003),
//new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1115, MyDialoguesWrapperEnum.Dlg_LastHope_2004),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1116, MyDialoguesWrapperEnum.Dlg_LastHope_2005),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1117, MyDialoguesWrapperEnum.Dlg_LastHope_2006),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1118, MyDialoguesWrapperEnum.Dlg_LastHope_2007),
new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1119, MyDialoguesWrapperEnum.Dlg_LastHope_2008),
//new MyDialogueSentence(MyActorEnum.FATHER_TOBIAS, MySoundCuesEnum.Dlg_LastHope_1120, MyDialoguesWrapperEnum.Dlg_LastHope_2009),
new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_LastHope_1121, MyDialoguesWrapperEnum.Dlg_LastHope_2010),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1122, MyDialoguesWrapperEnum.Dlg_LastHope_2011),
new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_LastHope_1123, MyDialoguesWrapperEnum.Dlg_LastHope_2012),
new MyDialogueSentence(MyActorEnum.LorraineCardin, MySoundCuesEnum.Dlg_LastHope_1124, MyDialoguesWrapperEnum.Dlg_LastHope_2013),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1125, MyDialoguesWrapperEnum.Dlg_LastHope_2014),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1126, MyDialoguesWrapperEnum.Dlg_LastHope_2015),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1127, MyDialoguesWrapperEnum.Dlg_LastHope_2016),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1128, MyDialoguesWrapperEnum.Dlg_LastHope_2017),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1129, MyDialoguesWrapperEnum.Dlg_LastHope_2018),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1130, MyDialoguesWrapperEnum.Dlg_LastHope_2019),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1131, MyDialoguesWrapperEnum.Dlg_LastHope_2020),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_LastHope_1132, MyDialoguesWrapperEnum.Dlg_LastHope_2021),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_LastHope_1133, MyDialoguesWrapperEnum.Dlg_LastHope_2022),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_LastHope_1134, MyDialoguesWrapperEnum.Dlg_LastHope_2023),


                                                                                             }
                                                                             ));

            #endregion


#region EAC AMBUSH
            m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_0100_INTRO, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1000, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0100),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1001, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0101),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1002, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0102),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1003, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0103),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1004, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0104),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1005, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0105),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1006, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0106),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_0200_MANJEET, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1007, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0200, listener: MyActorEnum.APOLLO),
//new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1008, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0201),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1009, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0202),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1010, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0203),
//new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1011, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0204),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1012, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0205),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1013, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0206),
//new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1014, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0207),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1015, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0208),
//new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1016, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0209),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1017, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0210),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1018, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0211),
//new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1019, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0212),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1020, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0213),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1021, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0214),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1022, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0215),
//new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1023, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0216),
new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1024, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0217),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1025, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0218),
//new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1026, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0219),
//new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1027, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0220),
//new MyDialogueSentence(MyActorEnum.MANJEET, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1028, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0221),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1029, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0222),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1030, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0223),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_0300_GUYS_HURRY_UP, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1031, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0300, pauseBefore_ms:1000),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1032, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0301),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1033, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0302),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1034, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0303),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1035, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0304),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_0400_MARCUS_TO_EAC, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1036, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0400),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1037, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0401),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1038, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0402),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1039, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0403),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1040, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0404),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1041, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0405),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1042, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0406, pauseBefore_ms:2000),
new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1043, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0407),
//new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1044, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0408),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1045, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0409),
//new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1046, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0410),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1047, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0411),
new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN_FEMALE, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1048, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0412),
//new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN_FEMALE, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1049, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0413),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1050, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0414),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1051, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0415),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1052, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0416),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1053, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0417),
new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1054, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0418),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1055, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0419),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1056, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0420),
new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN_FEMALE, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1057, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0421),
new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN_FEMALE, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1058, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0422),
//new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1059, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0423),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1060, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0424),
                                                                                             }
                                                                                         ));


m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_0500_ONE_LITTLE_ISSUE, new MyDialogue(
                                                                                         new MyDialogueSentence[]{

//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1061, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0425),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1062, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0426),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1063, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0427),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1064, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0428),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1065, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0429),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1066, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0430),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1067, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0431),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1068, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0432),
//new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1069, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0500),
//new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1070, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0501),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1071, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0502),
//new MyDialogueSentence(MyActorEnum.EAC_CAPTAIN_FEMALE, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1072, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0503),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1073, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0504),


                                                                                             }
                                                                                         ));

m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_0700_SPLIT_TO_DESTROY_GENERATORS, new MyDialogue(
                                                                                         new MyDialogueSentence[]{

new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1078, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0702),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1079, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0703),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1080, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0704),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1081, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0705),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1082, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0706),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1083, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0707),

new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1076, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0700, pauseBefore_ms:12000),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1077, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0701),
                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_0800, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1084, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0800),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1085, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0801),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1086, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0802),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_0900, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1087, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0900),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1088, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_0901),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_1000, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1089, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1000),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1090, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1001),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1091, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1002),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1092, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1003),

new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1093, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1100),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1094, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1101),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1095, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1102),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1096, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1103),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1097, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1104),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1098, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1105),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_1100, new MyDialogue(
                                                                                         new MyDialogueSentence[]{


                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_1200_1300, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1099, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1200),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1100, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1300, pauseBefore_ms:5000),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1101, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1301),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1102, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1302),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1103, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1303),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1104, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1304),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_1400, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1105, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1400),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1106, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1401),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_1500, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1107, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1500),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1108, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1501, pauseBefore_ms:2000),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1109, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1502),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1110, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1503),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1111, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1504),
//new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1112, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1505),
//new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1113, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1506),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_1600, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1114, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1600),
//new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1115, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1601),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1116, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1602),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1117, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1603),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1118, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1604),
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1119, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1605),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1120, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1606),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1121, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1607),
                                                                                             }
                                                                                         ));

m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_1650, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1122, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1608),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1123, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1609),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1124, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1610),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1125, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1611),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1126, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1612),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1127, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1613),
new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1128, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1614),

                                                                                             }
                                                                                         ));
m_dialogues.Add((int)MyDialogueEnum.EAC_AMBUSH_1700, new MyDialogue(
                                                                                         new MyDialogueSentence[]{
new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1130, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1701),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1129, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1700, pauseBefore_ms: 1000),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1132, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1703, pauseBefore_ms: 2000),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1133, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1704),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1134, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1705),
new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1135, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1706),
new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1136, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1707),
new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_JunkyardEacAmbush_1137, MyDialoguesWrapperEnum.Dlg_JunkyardEacAmbush_1708),

                                                                                             }
                                                                                         ));
#endregion  
#region EAC transmitter
m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0100, new MyDialogue(
                new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1000, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0101),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacTransmitter_1001, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0102),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1002, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0103),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1003, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0104),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1004, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0105),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1005, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0106),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1006, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0107),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacTransmitter_1007, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0108),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacTransmitter_1008, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0109),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacTransmitter_1009, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0110),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacTransmitter_1010, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0111),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1011, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0112),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0200, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacTransmitter_1012, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0201),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1013, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0202),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1014, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0203),
                }
            ));
            
            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0300, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1015, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0301),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1016, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0302),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1017, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0303),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1018, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0304),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1019, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0305),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0400, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1020, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0401),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1021, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0402),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1022, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0403),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1023, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0404),
                    //new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1024, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0405),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacTransmitter_1025, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0406),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1026, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0407),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1027, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0408),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0500, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1028, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0501),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1029, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0502),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1030, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0503),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1031, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0504),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1032, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0505),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1033, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0506),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0600, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1034, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0601),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacTransmitter_1035, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0602),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1036, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0603),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1037, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0604),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1038, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0605),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0700, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1039, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0701),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1040, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0702),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1041, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0703),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0800, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1042, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0801),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1043, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0802),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_0900, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1044, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0901),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1045, MyDialoguesWrapperEnum.Dlg_EacTransmitter_0902),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_1000, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1046, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1001),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1047, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1002),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1048, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1003),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1049, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1004),
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1050, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1005),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_1100, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1051, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1101),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1052, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1102),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_1200, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1053, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1201),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1054, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1202),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1055, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1203),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_1300, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MARCUS, MySoundCuesEnum.Dlg_EacTransmitter_1056, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1301),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1057, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1302),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1058, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1303),
                    new MyDialogueSentence(MyActorEnum.TARJA, MySoundCuesEnum.Dlg_EacTransmitter_1059, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1304),
                }
            ));

            m_dialogues.Add((int)MyDialogueEnum.EAC_TRANSMITTER_1400, new MyDialogue(
            new MyDialogueSentence[]
                {
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1060, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1401),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1061, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1402),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1062, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1403),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1063, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1404),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1064, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1405),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_EacTransmitter_1065, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1406),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_EacTransmitter_1066, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1407),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_EacTransmitter_1067, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1408),
                    new MyDialogueSentence(MyActorEnum.VALENTIN, MySoundCuesEnum.Dlg_EacTransmitter_1068, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1409),
                    new MyDialogueSentence(MyActorEnum.MADELYN, MySoundCuesEnum.Dlg_EacTransmitter_1069, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1410),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_EacTransmitter_1070, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1411),
                    new MyDialogueSentence(MyActorEnum.THOMAS, MySoundCuesEnum.Dlg_EacTransmitter_1071, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1412),
                    new MyDialogueSentence(MyActorEnum.APOLLO, MySoundCuesEnum.Dlg_EacTransmitter_1072, MyDialoguesWrapperEnum.Dlg_EacTransmitter_1413),
                }
            ));
            #endregion

            Validate();
        }
    }
}
