using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Toolkit.Input
{
    /// <summary>
    /// <p>Describes the format of the raw input from a Human Interface Device (HID). </p>
    /// </summary>
    /// 
    /// <remarks>
    /// <p>Each <strong>WM_INPUT</strong> can indicate several inputs, but all of the inputs come from the same HID. The size of the <strong>bRawData</strong> array is <strong>dwSizeHid</strong> *	<strong>dwCount</strong>.</p>
    /// </remarks>
    struct RawHid
    {
        /// <summary>
        /// <dd><p>The size, in bytes, of each HID input in <strong>bRawData</strong>. </p></dd>
        /// </summary>
        public int SizeHid;

        /// <summary>
        /// <dd><p>The number of HID inputs in <strong>bRawData</strong>.</p></dd>
        /// </summary>
        public int Count;

        /// <summary>
        /// <dd><p>The raw input data, as an array of bytes. </p></dd>
        /// When message processing is finished this data became invalid!
        /// </summary>
        public int RawData;
    }
}
