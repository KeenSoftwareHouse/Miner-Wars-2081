using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using Microsoft.Win32;
using SysUtils;
using MinerWars.AppCode.App;

namespace MinerWars.AppCode.Networking
{
    public static class MySteam
    {
        public static bool IsActive { get { return MyMinerGame.Services.Steam.IsActive; } }

        public static bool IsOnline { get { return MyMinerGame.Services.Steam.IsOnline; } }

        public static Int64 UserId { get { return MyMinerGame.Services.Steam.UserId; } }
        public static string UserName { get { return MyMinerGame.Services.Steam.UserName; } }

        public static byte[] SessionTicket { get { return MyMinerGame.Services.Steam.SessionTicket; } }

        public static String SerialKey { get { return MyMinerGame.Services.Steam.SerialKey; } }

        public static void RefreshSessionTicket()
        {
            MyMinerGame.Services.Steam.RefreshSessionTicket();
        }
    }
}
