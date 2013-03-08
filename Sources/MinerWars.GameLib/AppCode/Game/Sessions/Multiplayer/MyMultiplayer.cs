using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Net;
using SysUtils;
using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers.Session;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI.Core;
using System.Threading;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Weapons;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Managers.PhysicsManager.Physics;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Managers.EntityManager;
using System.IO;
using Lidgren.Network;
using MinerWars.AppCode.Game.World.Global;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.Sessions.Multiplayer;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Prefabs;
using MinerWars.AppCode.Game.Prefabs;

namespace MinerWars.AppCode.Game.Sessions
{
    // File was too large, split into multiple files
    partial class MyMultiplayer
    {
        private static MyMultiplayer m_instance;

        public static MyMultiplayer Static
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MyMultiplayer();
                }
                return m_instance;
            }
        }

        public MyMultiplayerPeers Peers = MyMultiplayerPeers.Static;

        public Action<MyTextsWrapperEnum, object[]> OnNotification;

        List<MyEntity> m_spawnPoints = new List<MyEntity>(100);
        object[] m_textArgs = new object[4];


        void Log(string message)
        {
            MyTrace.Send(TraceWindow.Multiplayer, message);
            MyMwcLog.WriteLine("MP - " + message);
        }

        private void Notify(MyTextsWrapperEnum text, object[] args)
        {
            var handler = OnNotification;
            if (handler != null)
            {
                handler(text, args);
            }
        }

        private void Notify(MyTextsWrapperEnum text, object arg0 = null, object arg1 = null, object arg2 = null, object arg3 = null)
        {
            var handler = OnNotification;
            if (handler != null)
            {
                m_textArgs[0] = arg0;
                m_textArgs[1] = arg1;
                m_textArgs[2] = arg2;
                m_textArgs[3] = arg3;

                handler(text, m_textArgs);
            }
        }


        // Sends and receives messages
        public static void Update()
        {
            if (MyMultiplayer.m_instance != null)
            {
                m_instance.UpdateInternal();
            }
        }

        private void UpdateInternal()
        {
            UpdateLobby();
            MyMultiplayerPeers.Static.Receive();
        }

        void Alert(string alertFormat, IPEndPoint endpoint, MyEventEnum eventType)
        {
            AlertVerbose(alertFormat, endpoint, eventType);
        }

        [Conditional("DEBUG")]
        void AlertVerbose(string alertFormat, IPEndPoint endpoint, MyEventEnum eventType)
        {
            var player = MyMultiplayerPeers.Static[endpoint];
            string playerInfo = String.Format(" UserId: {0}, GameUserId: {1}, EndPoint: {2}", player.UserId, player.GameId, player.EndPoint);
            MyTrace.Send(TraceWindow.MultiplayerAlerts, eventType.ToString() + ": " + alertFormat + playerInfo);
        }

        internal static void Shutdown()
        {
            MyMultiplayerPeers.Static.Shutdown();
        }
    }
}
