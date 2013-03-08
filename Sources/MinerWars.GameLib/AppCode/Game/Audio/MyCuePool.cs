using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XACT3;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Game.Utils;
using System.Threading;
using System.Collections.Concurrent;

namespace MinerWars.AppCode.Game.Audio
{
    sealed class MyCuePool : IDisposable
    {
        AudioEngine m_audioEngine;
        MyObjectsPool<MyCueProxy.Private> m_pool;
        Dictionary<Int64, MyCueProxy.Private> m_activeCues;

        List<IntPtr> m_cuesToDestroy;

        RawNotificationDescription m_notificationStopDesc = new RawNotificationDescription()
        {
            CuePointer = IntPtr.Zero,
            SoundBankPointer = IntPtr.Zero,
            CueIndex = -1,
            Flags = 1,
            Type = NotificationType.CueStop,
        };

        RawNotificationDescription m_notificationDestroyDesc = new RawNotificationDescription()
        {
            CuePointer = IntPtr.Zero,
            SoundBankPointer = IntPtr.Zero,
            CueIndex = -1,
            Flags = 1,
            Type = NotificationType.CueDestroyed,
        };

        RawNotificationCallback m_notificationCallback;

        public int Capacity { get { return m_pool.GetCapacity(); } }
        public int Count { get { return m_pool.GetActiveCount(); } }

        public MyCuePool(AudioEngine engine, int capacity = 500)
        {
            m_audioEngine = engine;
            m_pool = new MyObjectsPool<MyCueProxy.Private>(capacity);
            m_activeCues = new Dictionary<long, MyCueProxy.Private>(capacity);
            m_cuesToDestroy = new List<IntPtr>(capacity);

            m_notificationCallback = new RawNotificationCallback(OnNotify);
            m_audioEngine.RegisterNotificationRaw(ref m_notificationStopDesc, m_notificationCallback);
            //m_audioEngine.RegisterNotificationRaw(ref m_notificationDestroyDesc, m_notificationCallback);
        }

        public MySoundCue CreateCue(Cue xactCue, MySoundCuesEnum cueEnum, bool apply3d)
        {
            var cue = m_pool.Allocate();
            if (cue == null)
                throw new InvalidOperationException("Cue pool is empty!");

            cue.OnInit(xactCue, cueEnum);
            m_activeCues.Add(xactCue.NativePointer.ToInt64(), cue);

            return new MySoundCue(cue.Cue, cueEnum, apply3d);
        }

        public void Dispose()
        {
            m_audioEngine.UnregisterNotificationRaw(ref m_notificationStopDesc);
            //m_audioEngine.UnregisterNotificationRaw(ref m_notificationDestroyDesc);

            foreach (var cue in m_activeCues)
            {
                cue.Value.Destroy();
                cue.Value.OnRelease();
            }
            m_pool = null;
            m_activeCues = null;
            m_audioEngine = null;
        }

        public void Update()
        {
            IntPtr cuePtr;
            while (PopDestroyList(out cuePtr))
            {
                var xactCue = new Cue(cuePtr);

                var cue = GetCueByPointer(cuePtr);
                m_activeCues.Remove(cuePtr.ToInt64());
                cue.OnRelease();
                m_pool.Deallocate(cue);
                xactCue.Destroy();
            }
        }

        void PushDestroyList(IntPtr cueToDestroy)
        {
            lock (m_cuesToDestroy)
            {
                m_cuesToDestroy.Add(cueToDestroy);
            }
        }

        bool PopDestroyList(out IntPtr value)
        {
            lock (m_cuesToDestroy)
            {
                int index = m_cuesToDestroy.Count - 1;
                if (index >= 0)
                {
                    value = m_cuesToDestroy[index];
                    m_cuesToDestroy.RemoveAt(index);
                    return true;
                }
            }
            value = IntPtr.Zero;
            return false;
        }

        MyCueProxy.Private GetCueByPointer(IntPtr cue)
        {
            MyCueProxy.Private result;
            if (!m_activeCues.TryGetValue(cue.ToInt64(), out result))
            {
                throw new InvalidOperationException("Cue was not created from this pool!");
            }
            return result;
        }

        void OnNotify(ref RawNotification notification)
        {
            if (notification.Type == NotificationType.CueStop)
            {
                PushDestroyList(notification.Data.Cue.CuePointer);
            }
            else if (notification.Type == NotificationType.CueDestroyed)
            {
                //var ptr = notification.Data.Cue.CuePointer;
                //if (m_activeCues.ContainsKey(ptr.ToInt64()))
                //{
                //    PushDestroyList(notification.Data.Cue.CuePointer);
                //}
            }
        }

        public void StopAll(StopFlags stopMode)
        {
            foreach (var item in m_activeCues)
            {
                if (item.Value.Cue.State != CueState.Stopped && item.Value.Cue.State != CueState.Stopped)
                    item.Value.Cue.Stop(stopMode);
            }
        }

        public void WriteDebugInfo(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Playing:");
            foreach (var item in m_activeCues)
            {
                if ((item.Value.Cue.State & CueState.Playing) != 0 && (item.Value.Cue.State & CueState.Paused) == 0)
                    stringBuilder.AppendLine(MyEnumsToStrings.Sounds[(int)item.Value.Cue.CueEnum]);
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Paused:");
            foreach (var item in m_activeCues)
            {
                if ((item.Value.Cue.State & CueState.Paused) != 0)
                    stringBuilder.AppendLine(MyEnumsToStrings.Sounds[(int)item.Value.Cue.CueEnum]);
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Stopped:");
            foreach (var item in m_activeCues)
            {
                if ((item.Value.Cue.State & CueState.Stopping) != 0 || (item.Value.Cue.State & CueState.Stopped) != 0)
                    stringBuilder.AppendLine(MyEnumsToStrings.Sounds[(int)item.Value.Cue.CueEnum]);
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Other:");
            foreach (var item in m_activeCues)
            {
                if (!(
                    ((item.Value.Cue.State & CueState.Playing) != 0 && (item.Value.Cue.State & CueState.Paused) == 0)
                    ||
                    ((item.Value.Cue.State & CueState.Paused) != 0)
                    ||
                    ((item.Value.Cue.State & CueState.Stopping) != 0 || (item.Value.Cue.State & CueState.Stopped) != 0)
                    ))
                    stringBuilder.AppendLine(MyEnumsToStrings.Sounds[(int)item.Value.Cue.CueEnum]);
            }
            stringBuilder.AppendLine();
        }
    }
}
