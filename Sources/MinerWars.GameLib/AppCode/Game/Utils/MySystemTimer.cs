using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using MinerWars.AppCode.Game.GUI;

namespace MinerWars.AppCode.Game.Utils
{
    public static class MySystemTimer
    {
        // all values in ns:
        private static uint m_systemTimerNormalResolution;
        private static uint m_desiredSystemTimerResolution;
        private static uint m_minimumSystemTimerResolution;
        private static uint m_maximumSystemTimerResolution;
        private static uint m_currentSystemTimerResolution;
        private static bool m_initialized = false;

        private static bool m_isMaxResolution = false;
        private static bool m_isMinResolution = false;


        public static void Initialize()
        {
            m_systemTimerNormalResolution = GetSystemResolution();
            m_desiredSystemTimerResolution = m_systemTimerNormalResolution;
            m_initialized = true;

            // Write to log file informaiton about the timer:
            // ..
        }


        public static uint GetUserRequestResolution()
        {
            uint tmp = 0;
            MyWindowsAPIWrapper.NtSetTimerResolution(0, false, ref tmp);

            return m_currentSystemTimerResolution;
        }

        public static uint GetSystemResolution()
        {
            // Only default system settings???
            MyWindowsAPIWrapper.NTSTATUS result = MyWindowsAPIWrapper.NtQueryTimerResolution(ref m_minimumSystemTimerResolution, ref m_maximumSystemTimerResolution, ref m_currentSystemTimerResolution);

            return m_currentSystemTimerResolution;
        }

        public static void SetNormalResolution()
        {
            if (!m_initialized) Initialize();

            MyWindowsAPIWrapper.NTSTATUS result = MyWindowsAPIWrapper.NtSetTimerResolution(m_systemTimerNormalResolution, true, ref m_currentSystemTimerResolution);
        }

        public static void GetPossibleResolutionBoundaries(out uint Minimun_ns, out uint Maximum_ns)
        {
            if (!m_initialized) Initialize();

            Minimun_ns = m_minimumSystemTimerResolution;
            Maximum_ns = m_maximumSystemTimerResolution;
        }

        public static void SetResolution(uint Resolution_ns)
        {
            if (!m_initialized) Initialize();

            m_desiredSystemTimerResolution = Resolution_ns;

            if (Resolution_ns < m_maximumSystemTimerResolution)
            {
                Resolution_ns = m_maximumSystemTimerResolution;
            }
            else if (Resolution_ns > m_minimumSystemTimerResolution)
            {
                Resolution_ns = m_minimumSystemTimerResolution;
            }

            // Try to disable previous:
            MyWindowsAPIWrapper.NTSTATUS result = MyWindowsAPIWrapper.NtSetTimerResolution((uint)Resolution_ns, false, ref m_currentSystemTimerResolution);
            
            // Try to set current:
            result = MyWindowsAPIWrapper.NtSetTimerResolution((uint)Resolution_ns, true, ref m_currentSystemTimerResolution);
            uint set = GetSystemResolution();

            m_isMaxResolution = false;
            m_isMinResolution = false;
            // Write to log:
            // .. change resolution timer to ...
        }

        public static void SetMaximalResolution()
        {
            if (!m_initialized) Initialize();

            m_isMaxResolution = true;
            m_isMinResolution = false;

            SetResolution(m_maximumSystemTimerResolution);
        }

        public static void SetMinimalResolution()
        {
            if (!m_initialized) Initialize();

            m_isMaxResolution = false;
            m_isMinResolution = true;

            SetResolution(m_minimumSystemTimerResolution);
        }

        public static bool IsSetMaxResolution()
        {
            return m_isMaxResolution;
        }

        public static bool IsSetMinResolution()
        {                       
            return m_isMinResolution;
        }

        public static void SetByType(MyGuiScreenGamePlayType type)
        {
            switch (type)
            {
                // For normal resolution:
                case MyGuiScreenGamePlayType.EDITOR_MMO:
                case MyGuiScreenGamePlayType.EDITOR_SANDBOX:
                case MyGuiScreenGamePlayType.EDITOR_STORY:
                case MyGuiScreenGamePlayType.GAME_MMO:
                case MyGuiScreenGamePlayType.GAME_SANDBOX:
                case MyGuiScreenGamePlayType.GAME_STORY:
                case MyGuiScreenGamePlayType.INGAME_EDITOR:
                    //SetNormalResolution();
                    {
                    }
                    break;

                // For maximum resolution:
                case MyGuiScreenGamePlayType.CREDITS:
                case MyGuiScreenGamePlayType.MAIN_MENU:
                case MyGuiScreenGamePlayType.PURE_FLY_THROUGH:
                    SetMaximalResolution();
                    //SetMinimalResolutution();
                    {
                    }
                    break;
            }
        }
    }
}
