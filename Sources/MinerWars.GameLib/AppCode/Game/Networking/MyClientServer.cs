using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;

namespace MinerWars.AppCode.Game.World
{
    static class MyClientServer
    {
        static MyClientServer()
        {
            ResetUser();
        }

        // TODO: Find better place and move LoggedPlayer there
        // Local player (if logged in)
        private static MyPlayerLocal m_loggedPlayer;
        public static MyPlayerLocal LoggedPlayer
        {
            get { return m_loggedPlayer; }
            set
            {
                bool playerChanged = m_loggedPlayer != value;
                m_loggedPlayer = value;
                if (playerChanged && OnLoggedPlayerChanged != null)
                {
                    OnLoggedPlayerChanged(null, EventArgs.Empty);
                }
            }
        }

        public static void Logout()
        {
            ResetUser();
        }

        private static void ResetUser()
        {
            if (MinerWars.AppCode.Networking.MySteam.IsActive)
            {
                LoggedPlayer = null;
            }
            else
            {
                LoggedPlayer = MyPlayerLocal.CreateOfflineDemoUser();
            }
        }

        public static event EventHandler OnLoggedPlayerChanged;

        public static bool IsMwAccount
        {
            get
            {
                return LoggedPlayer != null && LoggedPlayer.GetUserId() > 0 && LoggedPlayer.GetUserId() != MyPlayerLocal.OFFLINE_MODE_USERID;
            }
        }

        public static bool HasFullGame
        {
            get
            {
                return LoggedPlayer != null && !LoggedPlayer.IsDemoUser();
            }
        }

        public static bool MW25DEnabled
        {
            get
            {
                return LoggedPlayer != null && LoggedPlayer.GetCanAccess25d();
            }
        }

    }
}
