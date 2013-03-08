using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KeenSoftwareHouse.Library.Trace
{
    public enum TraceWindow
    {
        Default,
        Saving,
        ParallelParticles,
        Server,
        EntityId,
        Multiplayer,
        MultiplayerAlerts,
    }

    public delegate ITrace InitTraceHandler(string traceId, string traceName);

    public static class MyTrace
    {
        class NullTrace : ITrace
        {
            public void Send(string msg, string comment = null) { }
            public void SendWatch(string watchName, object watchValue, int depth = 1) { }
            public void Indent(string msg, string comment = null) { }
            public void UnIndent(string msg = null, string comment = null) { }
        }

        const string MinerWarsWindowPrefix = "MW_ID";

        static Dictionary<int, ITrace> m_traces;
        static NullTrace m_nullTrace = new NullTrace();

        public static void Init(InitTraceHandler handler)
        {
            InitInternal(handler);
        }

        [Conditional("DEBUG"), Conditional("DEVELOP")]
        private static void InitInternal(InitTraceHandler handler)
        {
            m_traces = new Dictionary<int, ITrace>();

            var processName = Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");

            foreach (var e in Enum.GetValues(typeof(TraceWindow)))
            {
                var name = ((TraceWindow)e == TraceWindow.Default) ? processName : (processName + "_" + e.ToString());

                //var wnd = new WinTrace(String.Format("{0}_{1}", MinerWarsWindowPrefix, name), name);
                //wnd.ClearAll();

                string id = String.Format("{0}_{1}", MinerWarsWindowPrefix, name);
                m_traces[(int)e] = handler(id, name);
            }
        }

        [Conditional("DEBUG"), Conditional("DEVELOP")]
        public static void Send(TraceWindow window, string msg, string comment = null)
        {
            GetTrace(window).Send(msg, comment);
        }

        [Conditional("DEBUG"), Conditional("DEVELOP")]
        public static void SendWatch(string watchName, object watchValue, int depth = 1)
        {
            (m_traces.FirstOrDefault().Value ?? m_nullTrace).SendWatch(watchName, watchValue, depth);
        }

        [Conditional("DEBUG"), Conditional("DEVELOP")]
        public static void Indent(TraceWindow window, string msg, string comment = null)
        {
            GetTrace(window).Indent(msg, comment);
        }

        [Conditional("DEBUG"), Conditional("DEVELOP")]
        public static void UnIndent(TraceWindow window, string msg = null, string comment = null)
        {
            GetTrace(window).UnIndent(msg, comment);
        }

        public static ITrace GetTrace(TraceWindow window)
        {
            ITrace trace;
            if (m_traces == null || !m_traces.TryGetValue((int)window, out trace))
            {
                trace = m_nullTrace;
            }
            return trace;
        }
    }
}
