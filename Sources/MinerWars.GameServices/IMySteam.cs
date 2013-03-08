using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.GameServices
{
    public interface IMySteam
    {
        bool IsActive { get; }
        bool IsOnline { get; }

        Int64 UserId { get; }
        string UserName { get; }

        byte[] SessionTicket { get; }
        String SerialKey { get; }
        void RefreshSessionTicket();

        void StoreStats();
        void SetAchievement(string name, bool immediatellyUpload);
        void IndicateAchievementProgress(string achievement, uint value, uint maxValue);
        void SetStat(string statName, int value);
        int GetStatInt(string statName);
    }
}
