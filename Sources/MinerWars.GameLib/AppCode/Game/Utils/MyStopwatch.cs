using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Utils
{
    class MyStopwatch
    {
        private int m_startTime;
        private bool m_isRunning;
        private int m_elapsedTimeBeforePause;

        /// <summary>
        /// The inclusive lower bound of the random time interval.
        /// </summary>
        public int MinTimeMilliseconds { get; set; }
        /// <summary>
        /// The exclusive upper bound of the random time interval.
        /// </summary>
        public int MaxTimeMilliseconds { get; set; }
        /// <summary>
        /// Return true if <see cref="ElapsedMilliseconds"/> is bigger than random time between <see cref="MinTimeMilliseconds"/>
        /// and <see cref="MaxTimeMilliseconds"/>. This random time is generated on <see cref="Start"/> method call.
        /// </summary>
        public bool IsTimeUp
        {
            get
            {
                return ElapsedMilliseconds >= m_currentTimeInterval;
            }
        }
        private int m_currentTimeInterval;

        /// <summary>
        /// Initializes a new instance of the Stopwatch class.
        /// </summary>
        public MyStopwatch()
        {
            m_startTime = 0;
            m_isRunning = false;
            m_elapsedTimeBeforePause = 0;
            MinTimeMilliseconds = 0;
            MaxTimeMilliseconds = 0;
            m_currentTimeInterval = 0;
        }

        /// <summary>
        /// Initializes a new instance of the Stopwatch class.
        /// </summary>
        /// <param name="minTimeMilliseconds">The inclusive lower bound of the random time interval.</param>
        /// <param name="maxTimeMilliseconds">The exclusive upper bound of the random time interval.</param>
        public MyStopwatch(int minTimeMilliseconds, int maxTimeMilliseconds)
            : this()
        {
            MinTimeMilliseconds = minTimeMilliseconds;
            MaxTimeMilliseconds = maxTimeMilliseconds;
        }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds.
        /// WARNING: Elapsed time is changed only once per game Update().
        /// </summary>
        /// <returns>A read-only int integer representing the total number of milliseconds measured by the current instance.</returns>
        public int ElapsedMilliseconds
        {
            get { return m_elapsedTimeBeforePause + MyMinerGame.TotalGamePlayTimeInMilliseconds - m_startTime; }
        }

        /// <summary>
        /// Gets a value indicating whether the Stopwatch timer is running.
        /// </summary>
        /// <returns>true if the Stopwatch instance is currently running and measuring elapsed time for an interval; otherwise, false.</returns>
        public bool IsRunning
        {
            get { return m_isRunning; }
        }

        /// <summary>
        /// Stops time interval measurement and resets the elapsed time to zero.
        /// </summary>
        public void Reset()
        {
            m_isRunning = false;
            m_elapsedTimeBeforePause = 0;
        }

        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval.
        /// </summary>
        public void Start()
        {
            if (!m_isRunning) m_startTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_isRunning = true;
            m_currentTimeInterval = MyMwcUtils.GetRandomInt(MinTimeMilliseconds, MaxTimeMilliseconds);
        }

        /// <summary>
        /// Stops measuring elapsed time for an interval.
        /// </summary>
        public void Stop()
        {
            if (m_isRunning) m_elapsedTimeBeforePause += MyMinerGame.TotalGamePlayTimeInMilliseconds - m_startTime;
            m_isRunning = false;
        }

        /// <summary>
        /// Create new instance of MyStopwatch and start it.
        /// </summary>
        /// <returns></returns>
        public static MyStopwatch StartNew()
        {
            MyStopwatch ret = new MyStopwatch();
            ret.Start();
            return ret;
        }
    }

}
