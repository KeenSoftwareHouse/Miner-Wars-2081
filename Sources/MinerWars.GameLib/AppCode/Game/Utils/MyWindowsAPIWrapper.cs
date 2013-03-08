using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace MinerWars.AppCode.Game.Utils
{
    static public class MyWindowsAPIWrapper
    {
        static Func<long> m_workingSetDelegate;

        public static long WorkingSet
        {
            get
            {
                if (m_workingSetDelegate == null)
                {
                    // To properly initialize security permission
                    long testVal = Environment.WorkingSet;

                    var info = typeof(System.Environment).GetMethod("GetWorkingSet", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                    m_workingSetDelegate = (Func<long>)Delegate.CreateDelegate(typeof(Func<long>), info);
                }                
                return m_workingSetDelegate();
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWndle, String text, String caption, int buttons);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        //Declare the mouse hook constant.
        //For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public delegate int HookProc(int nCode, int wParam, int lParam);

        // Event ids
        public const int WM_DEVICECHANGE = 0x0219;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        // Install a thread-specific or global hook.
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        // Uninstall a hook.
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(int hhk);

        // Call the next hook in the hook sequence.
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int hhk, int nCode, int wParam, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceChangeHookStruct
        {
            public int lParam;
            public int wParam;
            public int message;
            public int hwnd;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static uint MapVirtualKeyEx(uint key, MinerWars.AppCode.Game.GUI.Core.MyGuiLocalizedKeyboardState.MAPVK mappingType, IntPtr keyboardLayout);
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static IntPtr LoadKeyboardLayout(string keyboardLayoutID, uint flags);
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static bool UnloadKeyboardLayout(IntPtr handle);
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static IntPtr GetKeyboardLayout(IntPtr threadId);
        [DllImport("kernel32.dll")]
        internal extern static IntPtr LoadLibrary(string lpFileName);

#if PROFILING
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long perfcount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long freq);
#endif //PROFILING


        public enum NTSTATUS : uint
        {
            STATUS_SUCCESS = 0x00000000,
            STATUS_TIMER_RESOLUTION_NOT_SET = 0xC0000245
        }


        // marshaling system functions:
        // For getting system options:
        [DllImport("ntdll.dll", EntryPoint = "NtQueryTimerResolution")]
        public static extern NTSTATUS NtQueryTimerResolution(ref uint MinimumResolution, ref uint MaximumResolution, ref uint CurrentResolution);
        // For setting system options:
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        public static extern NTSTATUS NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
    }
}
