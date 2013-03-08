using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Lidgren.Network;
using KeenSoftwareHouse.Library.Trace;
using KeenSoftwareHouse.Library.IO;
using System.Net;
using System.Threading;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public static class MyLidgrenExtensions
    {
        /// <summary>
        /// Wait until specified status is not set
        /// </summary>
        public static void WaitForStatus(this NetConnection connection, NetConnectionStatus status)
        {
            while (connection.Status != status)
            {
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Waits until specified status in not cleared.
        /// </summary>
        public static void WaitForStatusCleared(this NetConnection connection, NetConnectionStatus status)
        {
            while (connection.Status == status)
            {
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Wait until specified status is not set
        /// </summary>
        public static void WaitForStatus(this NetPeer peer, NetPeerStatus status)
        {
            while (peer.Status != status)
            {
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Waits until specified status in not cleared.
        /// </summary>
        public static void WaitForStatusCleared(this NetPeer peer, NetPeerStatus status, TimeSpan maxWait)
        {
            var start = DateTime.Now;
            while (peer.Status == status && (start + maxWait) > DateTime.Now)
            {
                Thread.Sleep(5);
            }
        }
    }
}
