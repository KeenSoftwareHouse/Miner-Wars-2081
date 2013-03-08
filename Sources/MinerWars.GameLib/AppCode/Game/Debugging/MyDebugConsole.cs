using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.GUI;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Debugging
{
    public class MyShortcut
    {
        public enum ModifierValue
        {
            Alt = 0x00000001,
            Shift = 0x00000002,
            Control = 0x00000004,
        };

        private Keys m_key;
        private short m_modifier;


        /// <summary>
        /// c-tor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifier"></param>
        public MyShortcut(Keys key, short modifier)
        {
            m_key = key;
            m_modifier = modifier;
        }


        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = m_key.GetHashCode();
                result = (result*397) ^ m_modifier.GetHashCode();
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (obj.GetType() != typeof(MyShortcut)) 
                return false;
            return GetHashCode() == obj.GetHashCode();
        }
    }

    internal enum MyDebugSystem
    {
        Any,
        Editor,
        Game
    }
   
    class MyDebugConsole
    {
        public delegate void DebugItemHandler(MyDebugConsoleItem debugItem);
        
        public struct MyDebugConsoleItem
        {
            public MyDebugSystem System;
            public string Category;
            public string Name;
            public MyShortcut Shortcut;
            public DebugItemHandler Action;
        }


        private static MyDebugConsole m_Instance = null;

        Dictionary<MyShortcut, MyDebugConsoleItem> m_shortcutMap = new Dictionary<MyShortcut, MyDebugConsoleItem>();
        Dictionary<string, List<MyDebugConsoleItem>> m_categoriesMap = new Dictionary<string, List<MyDebugConsoleItem>>();
        List<Toolkit.Input.Keys> m_pressedKeys = new List<Toolkit.Input.Keys>(10);

        private MyDebugConsole() { ;}

        public static MyDebugConsole GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new MyDebugConsole();

            return m_Instance;
        }


        /// <summary>
        /// RegisterShortcut
        /// </summary>
        [Conditional("DEBUG"), Conditional("DEVELOP")]
        public void RegisterShortcut(MyDebugSystem system, string category, string name, MyShortcut shortcut, DebugItemHandler action)
        {
            MyDebugConsoleItem item = new MyDebugConsoleItem
            {
                System = system,
                Category = category,
                Name = name,
                Shortcut = shortcut,
                Action = action
            };

            //if (m_shortcutMap.ContainsKey(shortcut))
              //  return false;
            if (shortcut != null)
            {
                m_shortcutMap.Add(shortcut, item);
            }

            List<MyDebugConsoleItem> items;
            m_categoriesMap.TryGetValue(category.ToString(), out items);
            if (items == null)
            {
                items = new List<MyDebugConsoleItem>();
                m_categoriesMap.Add(category.ToString(), items);
            }

            items.Add(item);
        }


        /// <summary>
        /// UnregisterShortcut
        /// </summary>
        /// <param name="system"></param>
        /// <param name="?"></param>
        public void UnregisterShortcut(MyShortcut shortcut)
        {
            if (shortcut == null)
                return;

            if (!m_shortcutMap.ContainsKey(shortcut))
                return;

            MyDebugConsoleItem item = m_shortcutMap[shortcut];

            m_shortcutMap.Remove(shortcut);

            List<MyDebugConsoleItem> items;
            m_categoriesMap.TryGetValue(item.Category.ToString(), out items);
            System.Diagnostics.Debug.Assert(items != null);

            items.Remove(item);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="input"></param>
        [Conditional("DEBUG"), Conditional("DEVELOP")]
        public void Update(MyDebugSystem system, MyGuiInput input)
        {
            input.GetPressedKeys(m_pressedKeys);
            short modifier = 0;
            if (input.IsAnyAltKeyPressed()) modifier |= (short)MyShortcut.ModifierValue.Alt;
            if (input.IsAnyShiftKeyPressed()) modifier |= (short)MyShortcut.ModifierValue.Shift;
            if (input.IsAnyCtrlKeyPressed()) modifier |= (short)MyShortcut.ModifierValue.Control;

            foreach(Keys k in m_pressedKeys)
            {
                var key = (Keys)k;
                if (key == Keys.LeftAlt || key == Keys.RightAlt || key == Keys.LeftShift || key == Keys.RightShift || key == Keys.LeftControl || key == Keys.RightControl)
                    continue;

                if (input.IsNewKeyPress(key))
                    OnKeyPressed(system, key, modifier);
            }
        }



        /// <summary>
        /// OnKeyDown
        /// </summary>
        /// <param name="key"></param>
        private void OnKeyPressed(MyDebugSystem system, Keys key, short modifier)
        {
            MyShortcut shortcut = new MyShortcut(key, modifier);

            if (m_shortcutMap.ContainsKey(shortcut))
            {
                MyDebugConsoleItem debugItem = m_shortcutMap[shortcut];
                if ((debugItem.System == system) || (debugItem.System == MyDebugSystem.Any))
                {
                    debugItem.Action(debugItem);
                }
            }
        }
    }
}
