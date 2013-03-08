using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Utils
{
    class MyUtilKeyToString
    {
        public Keys Key;
        public string Character;                //  Char/string when shift isn't pressed
        public string CharacterWhenShift;       //  Char/string when shift IS pressed
        public string Name;                     //  Name of this Key

        public MyUtilKeyToString(Keys key, string character, string characterWhenShift, string name)
        {
            Key = key;
            Character = character;
            CharacterWhenShift = characterWhenShift;
            Name = name;
        }
    }
    static class MyKeysToString
    {
        static String[] m_systemKeyNamesLower = new String[256];
        static String[] m_systemKeyNamesUpper = new String[256];

        static MyKeysToString()
        {
            for (int i = 0; i < m_systemKeyNamesLower.Length; i++)
            {
                //m_systemKeyNamesLower[i] = ((Keys)i).ToString().ToLower();
                //m_systemKeyNamesUpper[i] = ((Keys)i).ToString().ToUpper();

                m_systemKeyNamesLower[i] = ((char)i).ToString().ToLower();
                m_systemKeyNamesUpper[i] = ((char)i).ToString().ToUpper();
            }
        }

        static MyUtilKeyToString[] m_keyToString = new MyUtilKeyToString[]{
            new MyUtilKeyToString(Keys.Left, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.Left).ToString()),
            new MyUtilKeyToString(Keys.Right, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.Right).ToString()),
            new MyUtilKeyToString(Keys.Up, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.Up).ToString()),
            new MyUtilKeyToString(Keys.Down, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.Down).ToString()),
            new MyUtilKeyToString(Keys.Home, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.Home).ToString()),
            new MyUtilKeyToString(Keys.End, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.End).ToString()),
            new MyUtilKeyToString(Keys.Delete, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.Delete).ToString()),
            new MyUtilKeyToString(Keys.Back, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.Backspace).ToString()),
            new MyUtilKeyToString(Keys.Insert, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.Insert).ToString()),
            new MyUtilKeyToString(Keys.PageDown, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.PageDown).ToString()),
            new MyUtilKeyToString(Keys.PageUp,null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.PageUp).ToString()),
            new MyUtilKeyToString(Keys.LeftAlt, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.LeftAlt).ToString()),
            new MyUtilKeyToString(Keys.LeftControl, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.LeftControl).ToString()),
            new MyUtilKeyToString(Keys.LeftShift, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.LeftShift).ToString()),
            new MyUtilKeyToString(Keys.RightAlt, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.RightAlt).ToString()),
            new MyUtilKeyToString(Keys.RightControl, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.RightControl).ToString()),
            new MyUtilKeyToString(Keys.RightShift, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.RightShift).ToString()),
            
            new MyUtilKeyToString(Keys.CapsLock, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.KeysCapsLock).ToString()),
            new MyUtilKeyToString(Keys.Enter, null, null, MyTextsWrapper.Get(MyTextsWrapperEnum.KeysEnter).ToString()),
            new MyUtilKeyToString(Keys.Tab, "   ", "    ", MyTextsWrapper.Get(MyTextsWrapperEnum.Tab).ToString()),
            new MyUtilKeyToString(Keys.OemOpenBrackets, "[", "{", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysOpenBracket).ToString()),
            new MyUtilKeyToString(Keys.OemCloseBrackets, "]", "}", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysCloseBracket).ToString()),
            new MyUtilKeyToString(Keys.Multiply, "*", "*", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysMultiply).ToString()),
            new MyUtilKeyToString(Keys.Subtract, "-", "-", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysSubtract).ToString()),
            new MyUtilKeyToString(Keys.Add, "+", "+", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysAdd).ToString()),
            new MyUtilKeyToString(Keys.Divide, "/", "/", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysDivide).ToString()),
            new MyUtilKeyToString(Keys.NumPad0, "0", "0", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad0).ToString()),
            new MyUtilKeyToString(Keys.NumPad1, "1", "1", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad1).ToString()),
            new MyUtilKeyToString(Keys.NumPad2, "2", "2", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad2).ToString()),
            new MyUtilKeyToString(Keys.NumPad3, "3", "3", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad3).ToString()),
            new MyUtilKeyToString(Keys.NumPad4, "4", "4", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad4).ToString()),
            new MyUtilKeyToString(Keys.NumPad5, "5", "5", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad5).ToString()),
            new MyUtilKeyToString(Keys.NumPad6, "6", "6", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad6).ToString()),
            new MyUtilKeyToString(Keys.NumPad7, "7", "7", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad7).ToString()),
            new MyUtilKeyToString(Keys.NumPad8, "8", "8", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad8).ToString()),
            new MyUtilKeyToString(Keys.NumPad9, "9", "9", MyTextsWrapper.Get(MyTextsWrapperEnum.NumPad9).ToString()),
            new MyUtilKeyToString(Keys.Decimal, ".", ".", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysDecimal).ToString()),
            new MyUtilKeyToString(Keys.OemBackslash, "\\", "|", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysBackslash).ToString()),
            new MyUtilKeyToString(Keys.OemComma, ",", "<", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysComma).ToString()),
            new MyUtilKeyToString(Keys.OemMinus, "-", "_", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysMinus).ToString()),
            new MyUtilKeyToString(Keys.OemPeriod, ".", ">", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysPeriod).ToString()),
            new MyUtilKeyToString(Keys.OemPipe, "\\", "|", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysPipe).ToString()),
            new MyUtilKeyToString(Keys.OemPlus, "=", "+", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysPlus).ToString()),
            new MyUtilKeyToString(Keys.OemQuestion, "/", "?", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysQuestion).ToString()),
            new MyUtilKeyToString(Keys.OemQuotes, "\'", "\"", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysQuotes).ToString()),
            new MyUtilKeyToString(Keys.OemSemicolon, ";", ":", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysSemicolon).ToString()),
            new MyUtilKeyToString(Keys.OemTilde, "`", "~", MyTextsWrapper.Get(MyTextsWrapperEnum.KeysTilde).ToString()),
            new MyUtilKeyToString(Keys.Space, " ", " ", MyTextsWrapper.Get(MyTextsWrapperEnum.Space).ToString()),
            new MyUtilKeyToString(Keys.D0, "0", ")", "0"),
            new MyUtilKeyToString(Keys.D1, "1", "!", "1"),
            new MyUtilKeyToString(Keys.D2, "2", "@", "2"),
            new MyUtilKeyToString(Keys.D3, "3", "#", "3"),
            new MyUtilKeyToString(Keys.D4, "4", "$", "4"),
            new MyUtilKeyToString(Keys.D5, "5", "%", "5"),
            new MyUtilKeyToString(Keys.D6, "6", "^", "6"),
            new MyUtilKeyToString(Keys.D7, "7", "&", "7"),
            new MyUtilKeyToString(Keys.D8, "8", "*", "8"),
            new MyUtilKeyToString(Keys.D9, "9", "(", "9"),
            new MyUtilKeyToString(Keys.F1, null, null, "F1"),
            new MyUtilKeyToString(Keys.F2, null, null, "F2"),
            new MyUtilKeyToString(Keys.F3, null, null, "F3"),
            new MyUtilKeyToString(Keys.F4, null, null, "F4"),
            new MyUtilKeyToString(Keys.F5, null, null, "F5"),
            new MyUtilKeyToString(Keys.F6, null, null, "F6"),
            new MyUtilKeyToString(Keys.F7, null, null, "F7"),
            new MyUtilKeyToString(Keys.F8, null, null, "F8"),
            new MyUtilKeyToString(Keys.F9, null, null, "F9"),
            new MyUtilKeyToString(Keys.F10, null, null, "F10"),
            new MyUtilKeyToString(Keys.F11, null, null, "F11"),
            new MyUtilKeyToString(Keys.F12, null, null, "F12"),
            new MyUtilKeyToString(Keys.F13, null, null, "F13"),
            new MyUtilKeyToString(Keys.F14, null, null, "F14"),
            new MyUtilKeyToString(Keys.F15, null, null, "F15"),
            new MyUtilKeyToString(Keys.F16, null, null, "F16"),
            new MyUtilKeyToString(Keys.F17, null, null, "F17"),
            new MyUtilKeyToString(Keys.F18, null, null, "F18"),
            new MyUtilKeyToString(Keys.F19, null, null, "F19"),
            new MyUtilKeyToString(Keys.F20, null, null, "F20"),
            new MyUtilKeyToString(Keys.F21, null, null, "F21"),
            new MyUtilKeyToString(Keys.F22, null, null, "F22"),
            new MyUtilKeyToString(Keys.F23, null, null, "F23"),
            new MyUtilKeyToString(Keys.F24, null, null, "F24")};
        
        //  Return lowercase representation of key.
        public static String GetKeyText(Keys key)
        {
            if ((int)key >= m_systemKeyNamesLower.Length)
                return null;

            String retVal = m_systemKeyNamesLower[(int)key];
            if (retVal.Length > 1)
            {
                retVal = null;
            }
            for (int j = 0; j < m_keyToString.Length; j++)
            {
                if (m_keyToString[j].Key == key)
                {
                    retVal = m_keyToString[j].Character;
                    break;
                }
            }

            return retVal;
        }

        //  Return uppercase representation of key.
        public static String GetShiftedKeyText(Keys key)
        {
            if ((int)key >= m_systemKeyNamesUpper.Length)
                return null;

            String retVal = m_systemKeyNamesUpper[(int)key];
            if (retVal.Length > 1)
            {
                retVal = null;
            }
            for (int j = 0; j < m_keyToString.Length; j++)
            {
                if (m_keyToString[j].Key == key)
                {
                    retVal = m_keyToString[j].CharacterWhenShift;
                    break;
                }
            }
            return retVal;
        }

        //  Return the name of this key.
        public static String GetKeyName(Keys key)
        {
            if ((int)key >= m_systemKeyNamesUpper.Length)
                return null;

            String retVal = m_systemKeyNamesUpper[(int)key];
            for (int j = 0; j < m_keyToString.Length; j++)
            {
                if (m_keyToString[j].Key == key)
                {
                    retVal = m_keyToString[j].Name;
                    break;
                }
            }
            return retVal;
        }
    }
}
