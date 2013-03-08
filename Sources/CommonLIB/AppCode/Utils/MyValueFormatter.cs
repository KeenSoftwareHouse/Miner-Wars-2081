using System;
using System.Globalization;

namespace SysUtils.Utils
{
    public class MyValueFormatter
    {
        static private NumberFormatInfo m_numberFormatInfoHelper;
        
        static MyValueFormatter()
        {
            m_numberFormatInfoHelper = new NumberFormatInfo();
            m_numberFormatInfoHelper.NumberDecimalSeparator = ".";
            m_numberFormatInfoHelper.NumberGroupSeparator = " ";
        }


        //  Formats to: "1 234.123"
        public static string GetFormatedFloat(float num, int decimalDigits)
        {
            m_numberFormatInfoHelper.NumberDecimalDigits = decimalDigits;
            return num.ToString("N", m_numberFormatInfoHelper);
        }        

        //  Same as GetFormatedFloat() but allow to specify group separator
        public static string GetFormatedFloat(float num, int decimalDigits, string groupSeparator)
        {
            string originalGroupSeparator = m_numberFormatInfoHelper.NumberGroupSeparator;
            m_numberFormatInfoHelper.NumberGroupSeparator = groupSeparator;
            m_numberFormatInfoHelper.NumberDecimalDigits = decimalDigits;
            string ret = num.ToString("N", m_numberFormatInfoHelper);
            m_numberFormatInfoHelper.NumberGroupSeparator = originalGroupSeparator;
            return ret;
        }

        public static string GetFormatedDouble(double num, int decimalDigits)
        {
            m_numberFormatInfoHelper.NumberDecimalDigits = decimalDigits;
            return num.ToString("N", m_numberFormatInfoHelper);
        }

        public static string GetFormatedQP(decimal num)
        {
            return GetFormatedDecimal(num, 1);
        }

        public static string GetFormatedDecimal(decimal num, int decimalDigits)
        {
            m_numberFormatInfoHelper.NumberDecimalDigits = decimalDigits;
            return num.ToString("N", m_numberFormatInfoHelper);
        }

        public static string GetFormatedGameMoney(decimal num)
        {
            return GetFormatedDecimal(num, 2);
        }

        public static decimal GetDecimalFromString(string number, int decimalDigits)
        {
            try
            {
                m_numberFormatInfoHelper.NumberDecimalDigits = decimalDigits;
                return System.Convert.ToDecimal(number, m_numberFormatInfoHelper);
            }
            catch
            {
            }
            return 0;
        }

        public static float? GetFloatFromString(string number, int decimalDigits, string groupSeparator) 
        {
            float? result = null;

            string originalGroupSeparator = m_numberFormatInfoHelper.NumberGroupSeparator;
            m_numberFormatInfoHelper.NumberGroupSeparator = groupSeparator;
            m_numberFormatInfoHelper.NumberDecimalDigits = decimalDigits;
            try
            {
                result = (float)System.Convert.ToDouble(number, m_numberFormatInfoHelper);
            }
            catch 
            {
            }            
            m_numberFormatInfoHelper.NumberGroupSeparator = originalGroupSeparator;

            return result;
        }

        public static string GetFormatedLong(long l)
        {
            //  By Marek Rosa at 28.4.2010: Changed according to implementation int GetFormatedInt()
            return l.ToString("#,0", CultureInfo.InvariantCulture);
        }

        public static String GetFormatedDateTimeOffset(DateTimeOffset dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo);
        }

        public static String GetFormatedDateTime(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo);
        }

        public static String GetFormatedDateTimeForFilename(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd-HH-mm-ss-fff", DateTimeFormatInfo.InvariantInfo);
        }

        //  Especially for displaying the price on the web site
        public static string GetFormatedPriceEUR(decimal num)
        {
            return GetFormatedDecimal(num, 2) + " €";
        }
        
        //  Especially for displaying the price on the web site
        public static string GetFormatedPriceUSD(decimal num)
        {
            return "$" + GetFormatedDecimal(num, 2);
        }

        //  Especially for displaying the price on the web site - in USD
        //  Input price is in EUR and we convert it here to USD, rounding to two decimal points
        public static string GetFormatedPriceUSD(decimal priceInEur, decimal exchangeRateEurToUsd)
        {
            return "~" + GetFormatedDecimal(decimal.Round(exchangeRateEurToUsd * priceInEur, 2), 2) + " $";
        }

        public static string GetFormatedInt(int i)
        {
            //  By Marek Rosa at 20.4.2008: This is my last try to have working int formating with group separator ",".
            //  Now it display '0' as '0' and any higher positive/negative number with correct grouping.
            return i.ToString("#,0", CultureInfo.InvariantCulture);
        }

        public static string GetFormatedArray<T>(T[] array)
        {
            string s = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                s += array[i].ToString();
                if (i < (array.Length - 1)) s += ", ";
            }
            return s;
        }
    }
}
