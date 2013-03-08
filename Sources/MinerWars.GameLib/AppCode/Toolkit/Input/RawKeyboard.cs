using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SharpDX.RawInput;

namespace MinerWars.AppCode.Toolkit.Input
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct RawKeyboard
    {
        /// <summary>
        /// <dd><p>The scan code from the key depression. The scan code for keyboard overrun is <strong>KEYBOARD_OVERRUN_MAKE_CODE</strong>. </p></dd>
        /// </summary>
        public short MakeCode;

        /// <summary>
        /// <dd><p>Flags for scan code information. It can be one or more of the following.</p><table><tr><th>Value</th><th>Meaning</th></tr><tr><td><dl><dt><strong><see cref="F:SharpDX.RawInput.ScanCodeFlags.Break"/></strong></dt><dt>1</dt></dl></td><td><p>The key is up.</p></td></tr><tr><td><dl><dt><strong><see cref="F:SharpDX.RawInput.ScanCodeFlags.E0"/></strong></dt><dt>2</dt></dl></td><td><p>This is the left version of the key.</p></td></tr><tr><td><dl><dt><strong><see cref="F:SharpDX.RawInput.ScanCodeFlags.E1"/></strong></dt><dt>4</dt></dl></td><td><p>This is the right version of the key.</p></td></tr><tr><td><dl><dt><strong><see cref="F:SharpDX.RawInput.ScanCodeFlags.Make"/></strong></dt><dt>0</dt></dl></td><td><p>The key is down.</p></td></tr></table><p>?</p></dd>
        /// </summary>
        public ScanCodeFlags Flags;

        /// <summary>
        /// <dd><p>Reserved; must be zero. </p></dd>
        /// </summary>
        public short Reserved;

        /// <summary>
        /// <dd><p>Windows message compatible virtual-key code. For more information, see Virtual Key Codes. </p></dd>
        /// </summary>
        public short VKey;

        /// <summary>
        /// <dd><p>The corresponding window message, for example <strong><see cref="F:SharpDX.RawInput.KeyState.KeyDown"/></strong>, <strong><see cref="F:SharpDX.RawInput.KeyState.SystemKeyDown"/></strong>, and so forth. </p></dd>
        /// </summary>
        public KeyState Message;

        /// <summary>
        /// <dd><p>The device-specific additional information for the event. </p></dd>
        /// </summary>
        public int ExtraInformation;
    }
}
