using System;
using System.Linq;
using System.Runtime.InteropServices;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Toolkit.Input;
using System.Collections.Generic;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiLocalizedKeyboardState
    {
        static HashSet<byte> m_localKeys;

        internal enum MAPVK : uint
        {
            VK_TO_VSC = 0,
            VSC_TO_VK = 1,
            VK_TO_CHAR = 2
        }

        internal const uint KLF_NOTELLSHELL = 0x00000080;

        public struct KeyboardLayout : IDisposable
        {
            public readonly IntPtr Handle;

            public KeyboardLayout(IntPtr handle)
                : this()
            {
                Handle = handle;
            }

            public KeyboardLayout(string keyboardLayoutID)
                : this(MyWindowsAPIWrapper.LoadKeyboardLayout(keyboardLayoutID, KLF_NOTELLSHELL))
            {
            }

            public bool IsDisposed
            {
                get;
                private set;
            }

            public void Dispose()
            {
                if (IsDisposed)
                    return;

                MyWindowsAPIWrapper.UnloadKeyboardLayout(Handle);
                IsDisposed = true;
            }

            public static KeyboardLayout US_English = new KeyboardLayout("00000409");

            public static KeyboardLayout Active
            {
                get
                {
                    return new KeyboardLayout(MyWindowsAPIWrapper.GetKeyboardLayout(IntPtr.Zero));
                }
            }
        }

        private MyKeyboardState m_previousKeyboardState;
        private MyKeyboardState m_actualKeyboardState;

        public MyGuiLocalizedKeyboardState()
        {
            m_actualKeyboardState = MyWindowsKeyboard.GetCurrentState();

            if (m_localKeys == null)
            {
                m_localKeys = new HashSet<byte>();

                AddLocalKey(Keys.LeftControl);
                AddLocalKey(Keys.LeftAlt);
                AddLocalKey(Keys.LeftShift);
                AddLocalKey(Keys.RightAlt);
                AddLocalKey(Keys.RightControl);
                AddLocalKey(Keys.RightShift);
                AddLocalKey(Keys.Delete);
                AddLocalKey(Keys.NumPad0);
                AddLocalKey(Keys.NumPad1);
                AddLocalKey(Keys.NumPad2);
                AddLocalKey(Keys.NumPad3);
                AddLocalKey(Keys.NumPad4);
                AddLocalKey(Keys.NumPad5);
                AddLocalKey(Keys.NumPad6);
                AddLocalKey(Keys.NumPad7);
                AddLocalKey(Keys.NumPad8);
                AddLocalKey(Keys.NumPad9);
                AddLocalKey(Keys.Decimal);
                AddLocalKey(Keys.LeftWindows);
                AddLocalKey(Keys.RightWindows);
                AddLocalKey(Keys.Apps);
                AddLocalKey(Keys.Pause);
                AddLocalKey(Keys.Divide);
            }
        }

        void AddLocalKey(Keys key)
        {
            m_localKeys.Add((byte)key);
        }

        public void UpdateStates()
        {
            m_previousKeyboardState = m_actualKeyboardState;
            m_actualKeyboardState = MyWindowsKeyboard.GetCurrentState();
        }

        public MyKeyboardState GetActualKeyboardState()
        {
            return m_actualKeyboardState;
        }

        public MyKeyboardState GetPreviousKeyboardState()
        {
            return m_previousKeyboardState;
        }

        public bool IsPreviousKeyDown(Keys key, bool isLocalKey)
        {
            if (!isLocalKey)
                key = LocalToUSEnglish(key);

            return m_previousKeyboardState.IsKeyDown((Keys)key);
        }

        public bool IsPreviousKeyDown(Keys key)
        {
            return IsPreviousKeyDown(key, IsKeyLocal(key));
        }

        public bool IsPreviousKeyUp(Keys key, bool isLocalKey)
        {
            if (!isLocalKey)
                key = LocalToUSEnglish(key);

            return m_previousKeyboardState.IsKeyUp((Keys)key);
        }

        public bool IsPreviousKeyUp(Keys key)
        {
            return IsPreviousKeyUp(key, IsKeyLocal(key));
        }


        public bool IsKeyDown(Keys key, bool isLocalKey)
        {
            if (!isLocalKey)
                key = LocalToUSEnglish(key);

            return m_actualKeyboardState.IsKeyDown((Keys)key);
        }

        public bool IsKeyUp(Keys key, bool isLocalKey)
        {
            if (!isLocalKey)
                key = LocalToUSEnglish(key);

            return m_actualKeyboardState.IsKeyUp((Keys)key);
        }

        public bool IsKeyDown(Keys key)
        {
            return IsKeyDown(key, IsKeyLocal(key));
        }

        bool IsKeyLocal(Keys key)
        {
            return m_localKeys.Contains((byte)key);
        }

        public bool IsKeyUp(Keys key)
        {
            return IsKeyUp(key, IsKeyLocal(key));
        }

        // Maps a localized character like 'S' to the virtual scan code
        //  for that key on the user's keyboard ('O' in dvorak, for example)
        public static Keys USEnglishToLocal(Keys key)
        {
            return key;

            //var activeScanCode = MyWindowsAPIWrapper.MapVirtualKeyEx((uint)key, MAPVK.VK_TO_VSC, KeyboardLayout.US_English.Handle);
            //var nativeVirtualCode = MyWindowsAPIWrapper.MapVirtualKeyEx(activeScanCode, MAPVK.VSC_TO_VK, KeyboardLayout.Active.Handle);

            //return (Keys)nativeVirtualCode;
        }

        public static Keys LocalToUSEnglish(Keys key)
        {
            return key;

            /*
            var activeScanCode = MyWindowsAPIWrapper.MapVirtualKeyEx((uint)key, MAPVK.VK_TO_VSC, KeyboardLayout.US_English.Handle);
            var nativeVirtualCode = MyWindowsAPIWrapper.MapVirtualKeyEx(activeScanCode, MAPVK.VSC_TO_VK, KeyboardLayout.Active.Handle);
              */

            //var activeScanCode = MyWindowsAPIWrapper.MapVirtualKeyEx((uint)key, MAPVK.VK_TO_VSC, KeyboardLayout.Active.Handle);
            //var nativeVirtualCode = MyWindowsAPIWrapper.MapVirtualKeyEx(activeScanCode, MAPVK.VSC_TO_VK, KeyboardLayout.US_English.Handle);

            //return (Keys)nativeVirtualCode;
        }

        public bool IsAnyKeyPressed()
        {
            return m_actualKeyboardState.IsAnyKeyPressed();
        }

        public void GetActualPressedKeys(List<Keys> keys)
        {
            m_actualKeyboardState.GetPressedKeys(keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if (!IsKeyLocal((Keys)keys[i]))
                    keys[i] = (Keys)USEnglishToLocal((Keys)keys[i]);
            }
        }
    }
}
