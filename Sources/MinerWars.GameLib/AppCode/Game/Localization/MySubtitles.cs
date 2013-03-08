using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Localization
{
    static class MySubtitles
    {
        static bool m_subtitlesEnabled;


        public static bool Enabled
        {
            set
            {
                m_subtitlesEnabled = value;
                MyConfig.Subtitles = m_subtitlesEnabled;
                MyConfig.Save();
            }

            get
            {
                return m_subtitlesEnabled;
            }
        }
    }
}
