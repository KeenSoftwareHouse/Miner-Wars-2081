using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceTool;

namespace KeenSoftwareHouse.Library.Trace
{
    public class WintraceWrapper: ITrace
    {
        WinTrace m_trace;

        public WintraceWrapper(string id, string name)
        {
            m_trace = new WinTrace(id, name);
            m_trace.ClearAll();
        }

        public void Send(string msg, string comment = null)
        {
            m_trace.Debug.Send(msg, comment);
        }

        public void Indent(string msg, string comment = null)
        {
            m_trace.Indent(msg, comment);
        }

        public void UnIndent(string msg = null, string comment = null)
        {
            m_trace.UnIndent(msg, comment);
        }

        public void SendWatch(string watchName, object watchValue, int depth = 1)
        {
            lock (TTrace.Watches)
            {
                int oldDepth = TTrace.Options.ObjectTreeDepth;

                TTrace.Options.ObjectTreeDepth = depth;
                TTrace.Watches.Send(watchName, watchValue);
                TTrace.Options.ObjectTreeDepth = oldDepth;
            }
        }
    }
}
