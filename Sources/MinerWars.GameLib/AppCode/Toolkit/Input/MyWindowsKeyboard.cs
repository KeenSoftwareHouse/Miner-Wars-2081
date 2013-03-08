using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Security;
using System.Runtime.InteropServices;

namespace MinerWars.AppCode.Toolkit.Input
{
    static class MyWindowsKeyboard
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern unsafe bool GetKeyboardState(byte* data);

        public static MyKeyboardState GetCurrentState()
        {
            MyKeyboardBuffer buffer = new MyKeyboardBuffer();

            unsafe
            {
                byte* keyData = stackalloc byte[256];
                if (!GetKeyboardState(keyData))
                {
                    throw new InvalidOperationException("Could not read keyboard");
                }
                CopyBuffer(keyData, ref buffer);
            }
            return MyKeyboardState.FromBuffer(buffer);
        }

        static unsafe void CopyBuffer(byte* windowsKeyData, ref MyKeyboardBuffer buffer)
        {
            for (int i = 0; i < 256; i++)
            {
                if ((windowsKeyData[i] & 0x80) != 0)
                {
                    buffer.SetBit((byte)i, true);
                }
            }
        }
    }
}
