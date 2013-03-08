using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX.RawInput;

namespace MinerWars.AppCode.Toolkit.Input
{

    /// <summary>
    /// RawInput Keyboard event.
    /// 
    /// </summary>
    struct MyKeyboardInputArgs
    {
        public IntPtr Device;

        /// <summary>
        /// Gets or sets the key.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The key.
        /// 
        /// </value>
        public Keys Key;

        /// <summary>
        /// Gets or sets the make code.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The make code.
        /// 
        /// </value>
        public int MakeCode;

        /// <summary>
        /// Gets or sets the scan code flags.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The scan code flags.
        /// 
        /// </value>
        public ScanCodeFlags ScanCodeFlags;

        /// <summary>
        /// Gets or sets the state.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The state.
        /// 
        /// </value>
        public KeyState State;

        /// <summary>
        /// Gets or sets the extra information.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The extra information.
        /// 
        /// </value>
        public int ExtraInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SharpDX.RawInput.KeyboardInputEventArgs"/> class.
        /// 
        /// </summary>
        /// <param name="rawInput">The raw input.</param>
        internal MyKeyboardInputArgs(ref RawInput rawInput)
        {
            Device = rawInput.Header.Device;
            Key = (Keys)rawInput.Data.Keyboard.VKey;
            MakeCode = (int)rawInput.Data.Keyboard.MakeCode;
            ScanCodeFlags = rawInput.Data.Keyboard.Flags;
            State = rawInput.Data.Keyboard.Message;
            ExtraInformation = rawInput.Data.Keyboard.ExtraInformation;
        }
    }
}
