using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.RawInput;

namespace MinerWars.AppCode.Toolkit.Input
{
    struct RawInputHeader
    {
        public DeviceType Type;
        public int Size;
        public IntPtr Device;
        public IntPtr Param;
    }
}
