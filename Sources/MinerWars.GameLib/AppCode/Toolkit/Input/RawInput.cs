using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Toolkit.Input
{
    /// <summary>
    /// <p>Contains the raw input from a device. </p>
    /// </summary>
    /// 
    /// <remarks>
    /// <p>The handle to this structure is passed in the <em>lParam</em> parameter of <strong>WM_INPUT</strong>.</p><p>To get detailed information -- such as the header and the content of the raw input -- call <strong>GetRawInputData</strong>.</p><p>To read the <strong><see cref="T:SharpDX.RawInput.RawInput"/></strong> in the message loop as a buffered read, call <strong>GetRawInputBuffer</strong>. </p><p>To get device specific information, call <strong>GetRawInputDeviceInfo</strong> with the <em>hDevice</em> from <strong><see cref="T:SharpDX.RawInput.RawInputHeader"/></strong>.</p><p>Raw input is available only when the application calls <strong>RegisterRawInputDevices</strong> with valid device specifications. </p>
    /// </remarks>
    struct RawInput
    {
        /// <summary>
        /// <dd><p>The raw input data. </p></dd>
        /// </summary>
        public RawInputHeader Header;

        /// <summary>
        /// <dd><dl><dt><strong>mouse</strong></dt><dd><p/></dd><dd><p>If the data comes from a mouse, this is the raw input data. </p></dd><dt><strong>keyboard</strong></dt><dd><p/></dd><dd><p>If the data comes from a keyboard, this is the raw input data. </p></dd><dt><strong>hid</strong></dt><dd><p/></dd><dd><p>If the data comes from an HID, this is the raw input data. </p></dd></dl></dd>
        /// </summary>
        public RawInputInner0 Data;
    }
}
