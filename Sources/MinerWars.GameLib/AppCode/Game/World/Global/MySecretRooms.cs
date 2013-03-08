using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Game.World.Global
{
    static class MySecretRooms
    {
        // Never change numbers, it's ID in database
        // Name is SteamStatName
        public static readonly Dictionary<int, string> SecretRooms = new Dictionary<int, string>()
        {
            {1,"SecretEacSurvey"},
            {2,"SecretLaika"},
            {3,"SecretBarthsMoon"},
            {4,"SecretPirateBase"},
            {5,"SecretRussianWarehouse"},
            {6,"SecretLastHope"},
            {7,"SecretJunkyard"},
            {8,"SecretChineseTranpsort"},
            {9,"SecretChineseRefinery"},
            {10,"SecretFortValiant"},
            {11,"SecretSlaverBase"},
            {12,"SecretSlaverBase2"},
            {13,"SecretRime"},
            {14,"SecretResearchVessel"},
            {15,"SecretRift"},
            {16,"SecretChineseTransmitter"},
            {17,"SecretReichstag"},
            {18,"SecretBioResearch"},
            {19,"SecretTwinTowers"},
            {20,"SecretEacPrison"},
            {21,"SecretEacTransmitter"},
        };
    }
}
