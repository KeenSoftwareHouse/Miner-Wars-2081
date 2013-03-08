using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MinerWars.AppCode.Networking.SectorService
{
    class MySectorServerCallback : IMySectorServiceCallback
    {
        class CompletedAsyncResult : IAsyncResult
        {
            object data;

            public CompletedAsyncResult(object data)
            { this.data = data; }

            #region IAsyncResult Members
            public object AsyncState
            { get { return (object)data; } }

            public WaitHandle AsyncWaitHandle
            { get { return null; } }

            public bool CompletedSynchronously
            { get { return true; } }

            public bool IsCompleted
            { get { return true; } }
            #endregion
        }

        public delegate void ShutdownHandler(TimeSpan shutdownOn, TimeSpan shutdownLength, string shutdownMessage);
        public static event ShutdownHandler ShutdownNotification;

        public static void ClearEvents()
        {
            ShutdownNotification = null;
        }

        public void NotifyShutdown(TimeSpan shutdownOn, TimeSpan shutdownLength, string shutdownMessage)
        {
            var handler = ShutdownNotification;
            if (handler != null)
            {
                handler(shutdownOn, shutdownLength, shutdownMessage);
            }
        }

        public IAsyncResult BeginNotifyShutdown(TimeSpan shutdownOn, TimeSpan shutdownLength, string shutdownMessage, AsyncCallback callback, object asyncState)
        {
            NotifyShutdown(shutdownOn, shutdownLength, shutdownMessage);
            return new CompletedAsyncResult(asyncState);
        }

        public void EndNotifyShutdown(IAsyncResult result)
        {
            // Nothing to do
        }
    }
}
