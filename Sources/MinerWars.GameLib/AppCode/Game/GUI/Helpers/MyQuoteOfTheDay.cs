using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    static class MyQuoteOfTheDay
    {
        public static MyTextsWrapperEnum GetQuote(int i)
        {
            i = i % m_quotes.Length; if (i < 0) i += m_quotes.Length;  // correct modulo
            return m_quotes[i];
        }

        public static MyTextsWrapperEnum GetRandomQuote()
        {
            return GetQuote(MyMwcUtils.GetRandomInt(m_quotes.Length - 1));
        }

        static MyTextsWrapperEnum[] m_quotes = new MyTextsWrapperEnum[]
        {
            MyTextsWrapperEnum.QuoteOfTheDay00,
            MyTextsWrapperEnum.QuoteOfTheDay01,
            MyTextsWrapperEnum.QuoteOfTheDay02,
            MyTextsWrapperEnum.QuoteOfTheDay03,
            MyTextsWrapperEnum.QuoteOfTheDay04,
            MyTextsWrapperEnum.QuoteOfTheDay05,
            MyTextsWrapperEnum.QuoteOfTheDay06,
            MyTextsWrapperEnum.QuoteOfTheDay07,
            MyTextsWrapperEnum.QuoteOfTheDay08,
            MyTextsWrapperEnum.QuoteOfTheDay09,
            MyTextsWrapperEnum.QuoteOfTheDay10,
            MyTextsWrapperEnum.QuoteOfTheDay11,
            MyTextsWrapperEnum.QuoteOfTheDay12,
            MyTextsWrapperEnum.QuoteOfTheDay13,
            MyTextsWrapperEnum.QuoteOfTheDay14,
            MyTextsWrapperEnum.QuoteOfTheDay15,
            MyTextsWrapperEnum.QuoteOfTheDay16,
            MyTextsWrapperEnum.QuoteOfTheDay17,
            MyTextsWrapperEnum.QuoteOfTheDay18,
            MyTextsWrapperEnum.QuoteOfTheDay19,
            MyTextsWrapperEnum.QuoteOfTheDay20,
            MyTextsWrapperEnum.QuoteOfTheDay21,
            MyTextsWrapperEnum.QuoteOfTheDay22,
            MyTextsWrapperEnum.QuoteOfTheDay23,
            MyTextsWrapperEnum.QuoteOfTheDay24,
            MyTextsWrapperEnum.QuoteOfTheDay25,
            MyTextsWrapperEnum.QuoteOfTheDay26,
            MyTextsWrapperEnum.QuoteOfTheDay27,
            MyTextsWrapperEnum.QuoteOfTheDay28,
            MyTextsWrapperEnum.QuoteOfTheDay29,
            MyTextsWrapperEnum.QuoteOfTheDay30,
            MyTextsWrapperEnum.QuoteOfTheDay31,
            MyTextsWrapperEnum.QuoteOfTheDay32,
            MyTextsWrapperEnum.QuoteOfTheDay33,
            MyTextsWrapperEnum.QuoteOfTheDay34
        };
    }
}
