using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace MinerWars.AppCode.Toolkit.Input
{
    /// <summary>
    /// Describes the format of the raw input from a Human Interface Device (HID).
    /// 
    /// </summary>
    struct MyHidInputArgs
    {
        public IntPtr Device;

        /// <summary>
        /// Gets or sets the number of Hid structure in the <see cref="P:SharpDX.RawInput.HidInputEventArgs.RawData"/>.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The count.
        /// 
        /// </value>
        public int Count;

        /// <summary>
        /// Gets or sets the size of the Hid structure in the <see cref="P:SharpDX.RawInput.HidInputEventArgs.RawData"/>.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The size of the data.
        /// 
        /// </value>
        public int DataSize;

        /// <summary>
        /// Gets or sets the raw data.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The raw data.
        /// 
        /// </value>
        public byte[] RawData;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SharpDX.RawInput.HidInputEventArgs"/> class.
        /// 
        /// </summary>
        /// <param name="rawInput">The raw input.</param>
        internal MyHidInputArgs(ref RawInput rawInput)
        {
            Device = rawInput.Header.Device;
            Count = rawInput.Data.Hid.Count;
            DataSize = rawInput.Data.Hid.SizeHid;
            RawData = new byte[this.Count * this.DataSize];
            if (this.RawData.Length <= 0)
                return;
            unsafe
            {
                fixed (byte* numPtr1 = this.RawData)
                fixed (int* numPtr2 = &rawInput.Data.Hid.RawData)
                    Utilities.CopyMemory((IntPtr)((void*)numPtr1), (IntPtr)((void*)numPtr2), this.RawData.Length);
            }
        }
    }
}
