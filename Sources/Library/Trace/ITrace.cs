using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeenSoftwareHouse.Library.Trace
{
    public interface ITrace
    {
        void Send(string msg, string comment = null);
        void SendWatch(string watchName, object watchValue, int depth = 1);
        void Indent(string msg, string comment = null);
        void UnIndent(string msg = null, string comment = null);
    }
}
