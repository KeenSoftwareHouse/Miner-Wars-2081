using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.CompilerServices;
using SharpDX.Win32;
namespace MinerWars.AppCode.Toolkit.Input
{
    static class MyWindowsMouse
    {
        public class MouseMessageFilter : System.Windows.Forms.IMessageFilter
        {
            private const int WmMouseWheel = 0x20a;

            public bool PreFilterMessage(ref System.Windows.Forms.Message m)
            {
                if (m.Msg == WmMouseWheel)
                {
                    unsafe
                    {
                        int num = MyWindowsMouse.GET_WHEEL_DELTA_WPARAM(m.WParam);
                        MyWindowsMouse.m_currentWheel += num;
                    }
                }
                return false;
            }
        }

        static ushort HIWORD(IntPtr dwValue)
        {
            return (ushort)((((long)dwValue) >> 0x10) & 0xffff);
        }

        static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam)
        {
            return (short)HIWORD(wParam);
        }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 8)]
        struct POINT
        {
            public int X, Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int keyCode);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern unsafe int GetCursorPos(POINT* point);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern unsafe int ScreenToClient(void* handle, POINT* point);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern unsafe bool ClientToScreen(void* handle, POINT* lpPoint);

        static int m_currentWheel;
        static IntPtr m_windowHandle;

        public static void SetWindow(IntPtr windowHandle)
        {
            m_windowHandle = windowHandle;
            MessageFilterHook.AddMessageFilter(windowHandle, new MouseMessageFilter());
        }

        public static void SetPosition(int x, int y)
        {
            POINT pt = new POINT(x, y);

            if (m_windowHandle != IntPtr.Zero)
            {
                unsafe
                {
                    ClientToScreen(m_windowHandle.ToPointer(), &pt);
                }
            }
            SetCursorPos(pt.X, pt.Y);
        }

        public static void GetPosition(out int x, out int y)
        {
            POINT pt;

            unsafe
            {
                GetCursorPos(&pt);

                if (m_windowHandle != IntPtr.Zero)
                {
                    ScreenToClient(m_windowHandle.ToPointer(), &pt);
                }
            }
            x = pt.X;
            y = pt.Y;
        }

        public static MyMouseState GetCurrentState()
        {
            int x, y;
            GetPosition(out x, out y);

            bool left = (((ushort)GetAsyncKeyState(1)) >> 15) != 0;
            bool middle = (((ushort)GetAsyncKeyState(4)) >> 15) != 0;
            bool right = (((ushort)GetAsyncKeyState(2)) >> 15) != 0;
            bool x1 = (((ushort)GetAsyncKeyState(5)) >> 15) != 0;
            bool x2 = (((ushort)GetAsyncKeyState(6)) >> 15) != 0;

            return new MyMouseState(x, y, m_currentWheel, left, middle, right, x1, x2);
        }
    }
}
