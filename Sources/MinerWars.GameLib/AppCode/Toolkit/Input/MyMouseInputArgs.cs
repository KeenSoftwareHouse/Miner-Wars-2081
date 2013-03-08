using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.RawInput;

namespace MinerWars.AppCode.Toolkit.Input
{
    struct MyMouseInputArgs
    {
        public IntPtr Device;

        /// <summary>
        /// Gets or sets the mode.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The mode.
        /// 
        /// </value>
        public MouseMode Mode;

        /// <summary>
        /// Gets or sets the button flags.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The button flags.
        /// 
        /// </value>
        public MouseButtonFlags ButtonFlags;

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
        /// Gets or sets the raw buttons.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The raw buttons.
        /// 
        /// </value>
        public int Buttons;

        /// <summary>
        /// Gets or sets the wheel delta.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The wheel delta.
        /// 
        /// </value>
        public int WheelDelta;

        /// <summary>
        /// Gets or sets the X.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The X.
        /// 
        /// </value>
        public int X;

        /// <summary>
        /// Gets or sets the Y.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The Y.
        /// 
        /// </value>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SharpDX.RawInput.MouseInputEventArgs"/> class.
        /// 
        /// </summary>
        /// <param name="rawInput">The raw input.</param>
        internal MyMouseInputArgs(ref RawInput rawInput)
        {
            Device = rawInput.Header.Device;
            Mode = (MouseMode)rawInput.Data.Mouse.Flags;
            ButtonFlags = (MouseButtonFlags)rawInput.Data.Mouse.ButtonsData.ButtonFlags;
            WheelDelta = (int)rawInput.Data.Mouse.ButtonsData.ButtonData;
            Buttons = rawInput.Data.Mouse.RawButtons;
            X = rawInput.Data.Mouse.LastX;
            Y = rawInput.Data.Mouse.LastY;
            ExtraInformation = rawInput.Data.Mouse.ExtraInformation;
        }
    }
}
