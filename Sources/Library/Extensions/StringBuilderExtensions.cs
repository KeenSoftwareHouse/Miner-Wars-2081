namespace KeenSoftwareHouse.Library.Extensions
{
    using System;
    using System.Text;
    using Debugging;
    using System.Globalization;
    using System.Collections.Generic;

    /// <summary>
    /// Usefull StringBuilder extensions
    /// </summary>
    public static partial class StringBuilderExtensions
    {
        static private NumberFormatInfo m_numberFormatInfoHelper;

        static StringBuilderExtensions()
        {
            if (m_numberFormatInfoHelper == null)
            {
                m_numberFormatInfoHelper = new NumberFormatInfo();
                m_numberFormatInfoHelper.NumberDecimalSeparator = ".";
                m_numberFormatInfoHelper.NumberGroupSeparator = " ";
            }
        }

        /// <summary>
        /// Removes the specified number of characters from the end.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="length">The length.</param>
        public static void TrimEnd(this StringBuilder sb, int length)
        {
            Exceptions.ThrowIf<ArgumentException>(length > sb.Length, "String builder contains less characters then requested number!");

            sb.Length -= length;
        }

        public static StringBuilder GetFormatedLong(this StringBuilder sb, string before, long value, string after = "")
        {
            sb.Clear();
            sb.ConcatFormat("{0}{1: #,0}{2}", before, value, after);
            return sb;
        }

        public static StringBuilder GetFormatedInt(this StringBuilder sb, string before, int value, string after = "")
        {
            sb.Clear();
            sb.ConcatFormat("{0}{1: #,0}{2}", before, value, after);
            return sb;
        }

        public static StringBuilder GetFormatedFloat(this StringBuilder sb, string before, float value, string after = "")
        {
            sb.Clear();
            sb.ConcatFormat("{0}{1: #,0}{2}", before, value, after);
            return sb;
        }

        public static StringBuilder GetFormatedBool(this StringBuilder sb, string before, bool value, string after = "")
        {
            sb.Clear();
            sb.ConcatFormat("{0}{1}{2}", before, value, after);
            return sb;
        }

        public static StringBuilder GetFormatedDateTimeOffset(this StringBuilder sb, string before, DateTimeOffset value, string after = "")
        {
            return GetFormatedDateTimeOffset(sb, before, value.DateTime, after);
        }

        public static StringBuilder GetFormatedDateTimeOffset(this StringBuilder sb, string before, DateTime value, string after = "")
        {
            sb.Clear();
            //sb.ConcatFormat("{0}{1: yyyy-MM-dd HH:mm:ss.fff}{2}", before, value, after);
            sb.Append(before);
            sb.Concat(value.Year, 4, '0', 10, false);
            sb.Append("-");
            sb.Concat(value.Month, 2);
            sb.Append("-");
            sb.Concat(value.Day, 2);
            sb.Append(" ");
            sb.Concat(value.Hour, 2);
            sb.Append(":");
            sb.Concat(value.Minute, 2);
            sb.Append(":");
            sb.Concat(value.Second, 2);
            sb.Append(".");
            sb.Concat(value.Millisecond, 3);
            sb.Append(after);
            return sb;
        }


        public static StringBuilder GetFormatedDateTime(this StringBuilder sb, DateTime value)
        {
            sb.Clear();
            //sb.ConcatFormat("{0}{1: yyyy-MM-dd HH:mm:ss.fff}{2}", before, value, after);
            sb.Concat(value.Day, 2);
            sb.Append("/");
            sb.Concat(value.Month, 2);
            sb.Append("/");
            sb.Concat(value.Year, 0, '0', 10, false);
            sb.Append(" ");
            sb.Concat(value.Hour, 2);
            sb.Append(":");
            sb.Concat(value.Minute, 2);
            sb.Append(":");
            sb.Concat(value.Second, 2);
            return sb;
        }


        public static StringBuilder GetFormatedTimeSpan(this StringBuilder sb, string before, TimeSpan value, string after = "")
        {
            sb.Clear();
            //sb.ConcatFormat("{0}{1}{2}", before, value, after);
            sb.Clear();
            //sb.ConcatFormat("{0}{1: yyyy-MM-dd HH:mm:ss.fff}{2}", before, value, after);
            sb.Append(before);
            sb.Concat(value.Hours, 2);
            sb.Append(":");
            sb.Concat(value.Minutes, 2);
            sb.Append(":");
            sb.Concat(value.Seconds, 2);
            sb.Append(".");
            sb.Concat(value.Milliseconds, 3);
            sb.Append(after);
            return sb;
        }

        public static StringBuilder GetStrings(this StringBuilder sb, StringBuilder second)
        {
            //sb.Clear();
            //Unfortunatelly there is no other method without garbage
            for (int i = 0; i < second.Length; i++)
            {
                sb.Append(second[i]);
            }
            return sb;
        }

        public static StringBuilder GetStrings(this StringBuilder sb, string before, string value = "", string after = "")
        {
            sb.Clear();
            sb.ConcatFormat("{0}{1}{2}", before, value, after);
            return sb;
        }

        public static StringBuilder GetFormatedDecimal(this StringBuilder sb, string before, decimal value, int decimalDigits, string after = "")
        {
            sb.Clear();
            m_numberFormatInfoHelper.NumberDecimalDigits = decimalDigits;
            sb.ConcatFormat("{0}{1 }{2}", before, value, after, m_numberFormatInfoHelper);
            return sb;
        }

        public static StringBuilder AppendInt32(this StringBuilder sb, int number)
        {
            sb.ConcatFormat("{0}", number);
            return sb;
        }

        public static StringBuilder AppendDecimal(this StringBuilder sb, float number, int decimals)
        {
            m_numberFormatInfoHelper.NumberDecimalDigits = decimals;
            sb.ConcatFormat("{0}", number, m_numberFormatInfoHelper);
            return sb;
        }

        public static List<StringBuilder> Split(this StringBuilder sb, char separator)
        {
            List<StringBuilder> result = new List<StringBuilder>();

            StringBuilder current = new StringBuilder();
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == separator)
                {
                    result.Add(current);
                    current = new StringBuilder();
                }
                else
                    current.Append(sb[i]);
            }

            if (current.Length > 0)
                result.Add(current);

            return result;
        }

        /// <summary>
        /// Removes whitespace from the end.
        /// </summary>
        public static void TrimTrailingWhitespace(this StringBuilder sb)
        {
            int i = sb.Length;
            while (i > 0 && sb[i - 1] == ' ' || sb[i - 1] == '\r' || sb[i - 1] == '\n')
                i--;
            sb.Length = i;
        }
    }
}