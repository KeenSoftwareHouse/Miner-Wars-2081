using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.RawInput;

namespace MinerWars.AppCode.Toolkit.Input
{
    static class MyKeyTranslationTable
    {
        class KeyTranslation
        {
            public ScanCodeFlags? ScanCodeFlags;
            public int? MakeCode;
            public Keys DestKey;
        }

        static List<KeyTranslation>[] m_translationTable = new List<KeyTranslation>[256];

        static MyKeyTranslationTable()
        {
            AddTranslation(Keys.Control, ScanCodeFlags.Make, Keys.LeftControl);
            AddTranslation(Keys.Control, ScanCodeFlags.E0, Keys.RightControl);
            AddTranslation(Keys.Alt, ScanCodeFlags.Make, Keys.LeftAlt);
            AddTranslation(Keys.Alt, ScanCodeFlags.E0, Keys.RightAlt);
            AddTranslation(Keys.Shift, 0x2a, Keys.LeftShift);
            AddTranslation(Keys.Shift, 0x36, Keys.RightShift);
            //AddTranslation(Keys.Return ScanCodeFlags.E0, Keys.NumPadEnter);
        }

        static void AddTranslation(Keys srcKey, KeyTranslation translation)
        {
            byte index = (byte)srcKey;
            if (m_translationTable[index] == null)
                m_translationTable[index] = new List<KeyTranslation>(2);

            m_translationTable[index].Add(translation);
        }

        public static void AddTranslation(Keys srcKey, ScanCodeFlags scanCodeFlags, Keys destKey)
        {
            AddTranslation(srcKey, new KeyTranslation() { ScanCodeFlags = scanCodeFlags, MakeCode = null, DestKey = destKey });
        }

        public static void AddTranslation(Keys srcKey, int makeCode, Keys destKey)
        {
            AddTranslation(srcKey, new KeyTranslation() { ScanCodeFlags = null, MakeCode = makeCode, DestKey = destKey });
        }

        public static Keys Translate(Keys srcKey, ScanCodeFlags scanCodeFlags, int MakeCode)
        {
            var list = m_translationTable[(byte)srcKey];
            if (list != null)
            {
                foreach (var item in list)
                {
                    if ((item.ScanCodeFlags == null || item.ScanCodeFlags.Value == scanCodeFlags)
                        && (item.MakeCode == null || item.MakeCode.Value == MakeCode))
                    {
                        return item.DestKey;
                    }
                }
            }
            return Keys.None;
        }
    }
}
