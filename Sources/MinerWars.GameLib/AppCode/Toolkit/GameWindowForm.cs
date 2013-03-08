// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#if !W8CORE
using System;
using System.Drawing;
using System.Windows.Forms;

using SharpDX.Windows;
using SharpDX.Win32;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Utils;

namespace SharpDX.Toolkit
{
    internal class GameWindowForm : Form, IMessageFilter
    {
        private bool allowUserResizing;

        private bool m_enableMessageBypass = true;
        private bool m_isLoaded = false;

        /// <summary>
        /// When true, message processing in WinForms is disabled.
        /// No events like KeyDown, MouseMove... are raised.
        /// Activated and Deactivated is still raised.
        /// Prevent creating garbage.
        /// </summary>
        public bool EnableMessageBypass
        {
            get
            {
                return m_enableMessageBypass;
            }
            set
            {
                if (m_isLoaded)
                    throw new InvalidOperationException("Cannot enable bypass when form is loaded");

                m_enableMessageBypass = value;
            }
        }

        /// <summary>
        /// Messages which are bypassed.
        /// These messages are handled by DefWindowProc before it arrives to window WndProc.
        /// It prevents allocations in System.Windows.Forms
        /// </summary>
        public HashSet<int> BypassedMessages { get; private set; }

        public GameWindowForm()
            : this("SharpDX")
        {
        }

        public GameWindowForm(string text)
        {
            // By default, non resizable
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BypassedMessages = new HashSet<int>();

            BypassedMessages.Add((int)WM.KEYDOWN);
            BypassedMessages.Add((int)WM.KEYUP);
            BypassedMessages.Add((int)WM.CHAR);
            BypassedMessages.Add((int)WM.DEADCHAR);
            BypassedMessages.Add((int)WM.SYSKEYDOWN);
            BypassedMessages.Add((int)WM.SYSKEYUP);
            BypassedMessages.Add((int)WM.SYSCHAR);
            BypassedMessages.Add((int)WM.SYSDEADCHAR);

            BypassedMessages.Add((int)WM.MOUSEWHEEL);
            BypassedMessages.Add((int)WM.MOUSEMOVE);
            BypassedMessages.Add((int)WM.LBUTTONDOWN);
            BypassedMessages.Add((int)WM.LBUTTONUP);
            BypassedMessages.Add((int)WM.LBUTTONDBLCLK);
            BypassedMessages.Add((int)WM.RBUTTONDOWN);
            BypassedMessages.Add((int)WM.RBUTTONUP);
            BypassedMessages.Add((int)WM.RBUTTONDBLCLK);
            BypassedMessages.Add((int)WM.MBUTTONDOWN);
            BypassedMessages.Add((int)WM.MBUTTONUP);
            BypassedMessages.Add((int)WM.MBUTTONDBLCLK);
            BypassedMessages.Add((int)WM.XBUTTONDBLCLK);
            BypassedMessages.Add((int)WM.XBUTTONDOWN);
            BypassedMessages.Add((int)WM.XBUTTONUP);

            BypassedMessages.Add((int)WM.ERASEBKGND);
            BypassedMessages.Add((int)WM.SHOWWINDOW);
            BypassedMessages.Add((int)WM.ACTIVATE);
            BypassedMessages.Add((int)WM.SETFOCUS);
            BypassedMessages.Add((int)WM.KILLFOCUS);

            BypassedMessages.Add((int)WM.IME_NOTIFY);
        }
        
        internal bool AllowUserResizing
        {
            get
            {
                return allowUserResizing;
            }
            set
            {
                if (allowUserResizing != value)
                {
                    allowUserResizing = value;
                    MaximizeBox = allowUserResizing;
                    FormBorderStyle = allowUserResizing ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            m_isLoaded = true;
            if (EnableMessageBypass)
            {
                MessageFilterHook.AddMessageFilter(this.Handle, this);
            }
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (EnableMessageBypass)
            {
                MessageFilterHook.RemoveMessageFilter(this.Handle, this);
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (BypassedMessages.Contains(m.Msg))
            {
                if (m.Msg == (int)WM.ACTIVATE)
                {
                    if (m.WParam == IntPtr.Zero)
                        OnDeactivate(EventArgs.Empty);
                    else
                        OnActivated(EventArgs.Empty);
                }
                m.Result = MyWindowsAPIWrapper.DefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam);
                return true;
            }
            return false;
        }
    }
}
#endif