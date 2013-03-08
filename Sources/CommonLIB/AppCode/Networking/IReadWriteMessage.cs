using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public interface IReadWriteMessage
    {
        bool Read(MyMessageReader msg);
        void Write(MyMessageWriter msg);
    }
}
