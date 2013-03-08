using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.App;

namespace MinerWars.AppCode.Networking
{
    class MySteamStatNames
    {
        public const string FoundSecrets = "FoundSecrets";
    }

    class MySteamAchievementNames
    {
        /// <summary>
        /// Player found all 21 secret rooms
        /// </summary>
        public const string SecretRooms = "SecretRooms";

        /// <summary>
        /// Player completed first mission
        /// </summary>
        public const string Mission01_EacSS = "Mission01";
        public const string Mission03_Barths = "Mission03";
        public const string Mission08_JunkyardDuncan = "Mission08";
        public const string Mission12_JunkyardRacing = "Mission12";
        public const string Mission17_FortValiantDungeons = "Mission17";
        public const string Mission21_Rift = "Mission21";
        public const string Mission26_BioResearch = "Mission26";
        public const string Mission27_Reichstag2 = "Mission27";
        public const string Mission29_EacPrison = "Mission29";
        public const string Mission31_AlienGate = "Mission31";

        public const string ShipStanislav = "ShipStanislav";
    }

    class MySteamStats
    {
        public static void StoreState()
        {
            MyMinerGame.Services.Steam.StoreStats();
        }

        public static void IndicateAchievementProgress(string achievement, uint value, uint maxValue)
        {
            MyMinerGame.Services.Steam.IndicateAchievementProgress(achievement, value, maxValue);
        }
        
        public static void SetStat(string statName, int value)
        {
            MyMinerGame.Services.Steam.SetStat(statName, value);
        }

        public static int GetStatInt(string statName)
        {
            return MyMinerGame.Services.Steam.GetStatInt(statName);
        }

        public static void SetAchievement(string name, bool immediatellyUpload = true)
        {
            MyMinerGame.Services.Steam.SetAchievement(name, immediatellyUpload);
        }
    }
}
