using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MinerWars.AppCode.Toolkit.Input
{
    struct RawMouse
    {
        /// <summary>
        /// The mouse state.
        /// </summary>
        public short Flags;

        public RawMouse.RawMouseButtonsData ButtonsData;

        /// <summary>
        /// Raw button data.
        /// </summary>
        public int RawButtons;

        /// <summary>
        /// The motion in the X direction. This is signed relative motion or
        ///             absolute motion, depending on the value of usFlags.
        /// </summary>
        public int LastX;

        /// <summary>
        /// The motion in the Y direction. This is signed relative motion or absolute motion,
        ///             depending on the value of usFlags.
        /// </summary>
        public int LastY;

        /// <summary>
        /// The device-specific additional information for the event.
        /// </summary>
        public int ExtraInformation;

        [StructLayout(LayoutKind.Explicit)]
        public struct RawMouseButtonsData
        {
            [FieldOffset(0)]
            public int Buttons;

            /// <summary>
            /// Flags for the event.
            /// </summary>
            [FieldOffset(0)]
            public short ButtonFlags;
            
            /// <summary>
            /// If the mouse wheel is moved, this will contain the delta amount.
            /// </summary>
            [FieldOffset(2)]
            public short ButtonData;
        }
    }
}
