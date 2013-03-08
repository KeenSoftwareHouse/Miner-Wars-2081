using System;
using System.Globalization;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.Resources;
using SysUtils.Utils;

//  This is wrapper for MyTexts. Reasons I made him are these:
//      - MyTexts returns string, but I need to work with string builders
//      - I can't make own text resources, because I need that functionality that is in MyLocalizationPipelnie (importing only characters that are found in RES files)

//  IMPORTANT: Add all new texts here. Never use directly MyTexts in the code. Always use this wrapper!!!


namespace MinerWars.AppCode.Game.Localization
{
    static partial class MyTextsWrapper
    {
        static StringBuilder[] m_sb;
        static MyLanguagesEnum m_actualLanguage;
        static StringBuilder m_helperSb;

        public static void Init()
        {
            m_sb = new StringBuilder[Enum.GetValues(typeof(MyTextsWrapperEnum)).Length];
            m_helperSb = new StringBuilder();
            Reload();
        }

        static string ConvertAsciiToUnicode(string theAsciiString)
        {
            // Create two different encodings. 
            Encoding aAsciiEncoding = Encoding.ASCII;
            Encoding aUnicodeEncoding = Encoding.UTF8;
            // Convert the string into a byte[]. 
            byte[] aAsciiBytes = aAsciiEncoding.GetBytes(theAsciiString);
            // Perform the conversion from one encoding to the other. 
            byte[] aUnicodeBytes = Encoding.Convert(aAsciiEncoding, aUnicodeEncoding,
            aAsciiBytes);
            // Convert the new byte[] into a char[] and then into a string. 
            char[] aUnicodeChars = new char[aUnicodeEncoding.GetCharCount(aUnicodeBytes, 0, aUnicodeBytes.Length)];
            aUnicodeEncoding.GetChars(aUnicodeBytes, 0, aUnicodeBytes.Length, aUnicodeChars, 0);
            string aUnicodeString = new string(aUnicodeChars);
            return aUnicodeString;
        }

        static string ConvertUnicodeToAscii(string theUnicodeString)
        {
            // Create two different encodings. 
            Encoding aAsciiEncoding = Encoding.ASCII;
            Encoding aUnicodeEncoding = Encoding.UTF7;
            // convert the string into byte[]
            byte[] aUnicodeBytes = aUnicodeEncoding.GetBytes(theUnicodeString);
            byte[] aAsciiBytes = Encoding.Convert(aUnicodeEncoding, aAsciiEncoding, aUnicodeBytes);
            char[] aAsciiChars = new char[aAsciiEncoding.GetCharCount(aAsciiBytes, 0, aAsciiBytes.Length) * 2];
            aAsciiEncoding.GetChars(aAsciiBytes, 0, aAsciiBytes.Length, aAsciiChars, 0);
            string aAsciiString = new string(aAsciiChars);
            return aAsciiString;
        }

        public static StringBuilder Get(MyTextsWrapperEnum textEnum)
        {
            if (m_sb == null)
                return MyMwcUtils.EmptyStringBuilder;

            //return m_sb[(int)textEnum];
            return m_sb[(int)textEnum];
        }


        public static String GetFormatString(MyTextsWrapperEnum textEnum)
        {
            return m_sb[(int)textEnum].ToString();
        }

        public static String GetFormatString(MyTextsWrapperEnum textEnum, object arg0)
        {
            m_helperSb.Clear();
            m_helperSb.AppendFormat(GetFormatString(textEnum), arg0);
            return m_helperSb.ToString();
        }
       
        public static String GetFormatString(MyTextsWrapperEnum textEnum, object[] formatArgs)
        {
            m_helperSb.Clear();
            m_helperSb.AppendFormat(GetFormatString(textEnum), formatArgs);
            return m_helperSb.ToString();
        }
       
        public static string LanguageToString(MyLanguagesEnum lang)
        {
            return MyEnumsToStrings.LanguageEnums[(int)lang];
        }


        public static string GetActualLanguageString()
        {
            return LanguageToString(ActualLanguage);
        }

        public static MyLanguagesEnum ActualLanguage
        {
            set
            {
                //  Change current culture so this will switch text resources to new language
                MyTexts.Culture = new CultureInfo(LanguageToString(value), true);

                Reload();

                //  Save into config
                m_actualLanguage = value;
                MyConfig.Language = m_actualLanguage;
                MyConfig.Save();
            }

            get
            {
                return m_actualLanguage;
            }
        }
    }
}