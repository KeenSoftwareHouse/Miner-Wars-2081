using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MinerWars.AppCode.Toolkit.Input
{
    [StructLayout(LayoutKind.Explicit)]
    struct RawInputInner0
    {
        [FieldOffset(0)]
        public RawMouse Mouse;
        
        [FieldOffset(0)]
        public RawKeyboard Keyboard;
        
        [FieldOffset(0)]
        public RawHid Hid;
    }
}
