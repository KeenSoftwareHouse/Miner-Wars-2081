using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text;

namespace SysUtils.Utils
{
    public enum MyMwcLogEventEnum
    {
        All,
        SequencedMessageDebugging,
    }

    public static class MyMwcLog
    {
        struct MyLogIndentKey
        {
            public int ThreadId;
            public int Indent;     //  Can be 0, 1, 2, 3, ...

            public MyLogIndentKey(int threadId, int indent)
            {
                ThreadId = threadId;
                Indent = indent;
            }
        }

        struct MyLogIndentValue
        {
            public long LastGcTotalMemory;
            public long LastWorkingSet;
            public DateTimeOffset LastDateTimeOffset;     //  DateTimeOffset.Now doesn't do garbage (DateTime.Now does internal allocations)

            public MyLogIndentValue(long lastGcTotalMemory, long lastWorkingSet, DateTimeOffset lastDateTimeOffset)
            {
                LastGcTotalMemory = lastGcTotalMemory;
                LastWorkingSet = lastWorkingSet;
                LastDateTimeOffset = lastDateTimeOffset;
            }
        }

        static bool LogForMemoryProfiler = false;
        static bool m_enabled = false;          //  Must be false, beuuase MW web site must not write into log file
        static FileStream m_fileStream;			//	Used for opening and closing the file
        static StreamWriter m_streamWriter;		//	Used for writing into the file
        static readonly Object m_lock = new Object();
        static Dictionary<int, int> m_indentsByThread;
        private static Dictionary<MyLogIndentKey, MyLogIndentValue> m_indents;
        static string m_filepath;
        static MyMwcLogEventEnum m_eventType;

        public static void Init(string applicationFolder, string logFileName, StringBuilder appVersionString, string buildTypeString)
        {
            Init(applicationFolder, logFileName, appVersionString, buildTypeString, MyMwcLogEventEnum.All);
        }

        public static void Init(string applicationFolder, string logFileName, StringBuilder appVersionString, string buildTypeString, MyMwcLogEventEnum eventType)
        {
            lock (m_lock)
            {
                try
                {
                    //  Get path to application user-data folder, create new subfolder for MinerWars application and place here log file
                    m_filepath = Path.Combine(applicationFolder, logFileName);
                    MyFileSystemUtils.CreateFolderForFile(m_filepath);
                    m_fileStream = new FileStream(m_filepath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    m_streamWriter = new StreamWriter(m_fileStream);
                    m_enabled = true;
                    m_eventType = eventType;
                }
                catch (Exception)
                {
                    //  Ignore exception - the game must run even if log file can't be used
                }

                m_indentsByThread = new Dictionary<int, int>();
                m_indents = new Dictionary<MyLogIndentKey, MyLogIndentValue>();

                int timezone = (int) Math.Round((DateTime.Now - DateTime.UtcNow).TotalHours);

                WriteLine("Log Started");
                WriteLine(String.Format("Timezone (local - UTC): {0}h", timezone));
                WriteLine("App Version: " + appVersionString);
                WriteLine("Build Type: " + buildTypeString);
            }
        }

        /// <summary>
        /// Gets or sets value representing information written to log.
        /// When EventType is MyMwcLogEventEnum.All, everything is logged.
        /// When EventType is anything else, only there events are logged.
        /// </summary>
        public static MyMwcLogEventEnum EventType
        {
            get
            {
                lock (m_lock)
                {
                    return m_eventType;
                }
            }
            set
            {
                lock (m_lock)
                {
                    m_eventType = value;
                }
            }
        }

        public static string GetFilePath()
        {
            lock (m_lock)
            {
                return m_filepath;
            }
        }

        public static void IncreaseIndent(LoggingOptions option)
        {
            if (LogFlag(option))
            {
                IncreaseIndent();
            }
        }

        public static void IncreaseIndent()
        {
            if (m_enabled == false) return;

            lock (m_lock)
            {
                int threadId = GetThreadId();
                m_indentsByThread[threadId] = GetIdentByThread(threadId) + 1;

                MyLogIndentKey indentKey = new MyLogIndentKey(threadId, m_indentsByThread[threadId]);
                m_indents[indentKey] = new MyLogIndentValue(GetManagedMemory(), GetSystemMemory(), DateTimeOffset.Now);

                if (LogForMemoryProfiler)
                    MyMemoryLogs.StartEvent();
            }

            LogMemoryInfo(null);
        }

        public static bool IsIndentKeyIncreased()
        {
            if (m_enabled == false) return false;

            lock (m_lock)
            {
                int threadId = GetThreadId();
                MyLogIndentKey indentKey = new MyLogIndentKey(threadId, GetIdentByThread(threadId));

                //  If this fails, then order of IncreaseIndent/DecreaseIndent was wrong, or duplicate, etc
                return m_indents.ContainsKey(indentKey);
            }
        }

        public static void DecreaseIndent(LoggingOptions option)
        {
            if (LogFlag(option))
            {
                DecreaseIndent();
            }
        }

        public static void DecreaseIndent()
        {
            if (m_enabled == false) return;

            MyLogIndentValue indentValue;

            lock (m_lock)
            {
                int threadId = GetThreadId();
                MyLogIndentKey indentKey = new MyLogIndentKey(threadId, GetIdentByThread(threadId));

                //  If this fails, then order of IncreaseIndent/DecreaseIndent was wrong, or duplicate, etc
                MyCommonDebugUtils.AssertDebug(m_indents.ContainsKey(indentKey));

                indentValue = m_indents[indentKey];

                if (LogForMemoryProfiler)
                {
                    MyMemoryLogs.MyMemoryEvent memEvent = new MyMemoryLogs.MyMemoryEvent();
                    memEvent.DeltaTime = (float)(DateTimeOffset.Now - indentValue.LastDateTimeOffset).TotalMilliseconds / 1000.0f;
                    memEvent.ManagedEndSize = GetManagedMemory();
                    memEvent.ProcessEndSize = GetSystemMemory();
                    memEvent.ManagedStartSize = indentValue.LastGcTotalMemory;
                    memEvent.ProcessStartSize = indentValue.LastWorkingSet;
                    MyMemoryLogs.EndEvent(memEvent);
                }
                
            }
            LogMemoryInfo(indentValue);

            lock (m_lock)
            {
                int threadId = GetThreadId();
                m_indentsByThread[threadId] = GetIdentByThread(threadId) - 1;

            }
        }

        //  Log memory info
        static void LogMemoryInfo(MyLogIndentValue? indentValue)
        {
            if (m_enabled == false) return;

            if (MyMwcFinalBuildConstants.EnableLoggingGarbageCollectionCalls == false)
                return;

            WriteLine("*** Managed memory: " + GetFormatedMemorySize(GetManagedMemory()));
            WriteLine("*** System memory: " + GetFormatedMemorySize(GetSystemMemory()));

            if (indentValue != null)
            {
                WriteLine("******* Delta - Managed memory: " + GetFormatedMemorySize(GetManagedMemory() - indentValue.Value.LastGcTotalMemory));
                WriteLine("******* Delta - System memory: " + GetFormatedMemorySize(GetSystemMemory() - indentValue.Value.LastWorkingSet));
                WriteLine("******* Delta - Time: " + MyValueFormatter.GetFormatedFloat((float)(DateTimeOffset.Now - indentValue.Value.LastDateTimeOffset).TotalMilliseconds / 1000.0f, 4) + " sec");
            }
        }

        static string GetFormatedMemorySize(long bytesCount)
        {
            return MyValueFormatter.GetFormatedFloat(bytesCount / 1024.0f / 1024.0f, 3) + " Mb (" +
                   MyValueFormatter.GetFormatedLong(bytesCount) + " bytes)";
        }

        static long GetManagedMemory()
        {
            return GC.GetTotalMemory(false);
        }

        static long GetSystemMemory()
        {
            return Environment.WorkingSet;
        }

        //  Log memory info
        public static void LogMemoryInfo()
        {
            LogMemoryInfo(null);
        }

        //	Must be called before application ends
        public static void Close()
        {
            //Debug.Close();

            if (m_enabled == false) return;

            lock (m_lock)
            {
                WriteLine("Log Closed");

                m_streamWriter.Close();
                m_fileStream.Close();

                //	Only for making sure that nobody will call WriteLine after Close
                m_fileStream = null;
                m_streamWriter = null;

                m_enabled = false;
            }
        }

        public static bool LogFlag(LoggingOptions option)
        {
            long lValue = Convert.ToInt64(MyMwcFinalBuildConstants.LOGGING_OPTIONS);
            long lFlag = Convert.ToInt64(option);

            return (lValue & lFlag) != 0;
        }

        public static void WriteLine(string message, LoggingOptions option)
        {
            if (LogFlag(option))
            {
                WriteLine(message);
            }
        }

        public static void WriteLine(Exception ex)
        {
            WriteLine(MyMwcLogEventEnum.All, ex);
        }

        //  Write an exception on new line
        public static void WriteLine(MyMwcLogEventEnum eventType, Exception ex)
        {
            if (m_enabled == false) return;

            WriteLine(eventType, "Exception occured: " + ((ex == null) ? "null" : ex.ToString()));
            if (ex != null && ex.InnerException != null)
            {
                WriteLine(eventType, "Inner Exception: " + ex.InnerException.ToString());
            }
        }

        public static void WriteLine(string msg)
        {
            WriteLine(MyMwcLogEventEnum.All, msg);

            if (LogForMemoryProfiler)
                MyMemoryLogs.AddConsoleLine(msg);
        }

        //  Write a string on new line
        public static void WriteLine(MyMwcLogEventEnum eventType, string msg)
        {
            if (m_enabled == false) return;

            lock (m_lock)
            {
                // If write all or write this event type
                if (m_eventType == MyMwcLogEventEnum.All || eventType == m_eventType)
                {
                    m_streamWriter.WriteLine(GetWithDateTimeAndThreadId(msg));
                    m_streamWriter.Flush();
                }
            }

            //Debug.WriteLine(msg);
        }

        public static void Write(string msg)
        {
            Write(MyMwcLogEventEnum.All, msg);
        }

        //  Write a string on EXISTING line
        public static void Write(MyMwcLogEventEnum eventType, string msg)
        {
            if (m_enabled == false) return;

            lock (m_lock)
            {
                // If write all or write this event type
                if (m_eventType == MyMwcLogEventEnum.All || eventType == m_eventType)
                {
                    m_streamWriter.Write(GetWithDateTimeAndThreadId(msg));
                    m_streamWriter.Flush();
                }
            }

            //Debug.Write(msg);
        }

        //  Network verbose logging will work only ENABLE_NETWORK_VERBOSE_LOGGING is defined in MinerWarsCommonLIB project properties
        //  Otherwise it won't be even in the exe - I have checked it through Reflector. It's same behaviour as Debug.Assert
        [Conditional("ENABLE_NETWORK_VERBOSE_LOGGING")]
        public static void IfNetVerbose_AddToLog(string msg)
        {
            if (m_enabled == false) return;

            WriteLine(msg);
        }

        //  Network verbose logging will work only ENABLE_NETWORK_VERBOSE_LOGGING is defined in MinerWarsCommonLIB project properties
        //  Otherwise it won't be even in the exe - I have checked it through Reflector. It's same behaviour as Debug.Assert
        [Conditional("ENABLE_NETWORK_VERBOSE_LOGGING")]
        public static void IfNetVerbose_IncreaseIndent()
        {
            IncreaseIndent();
        }

        //  Network verbose logging will work only ENABLE_NETWORK_VERBOSE_LOGGING is defined in MinerWarsCommonLIB project properties
        //  Otherwise it won't be even in the exe - I have checked it through Reflector. It's same behaviour as Debug.Assert
        [Conditional("ENABLE_NETWORK_VERBOSE_LOGGING")]
        public static void IfNetVerbose_DecreaseIndent()
        {
            DecreaseIndent();
        }

        //  Log info about ThreadPool
        public static void LogThreadPoolInfo()
        {
            if (m_enabled == false) return;

            WriteLine("LogThreadPoolInfo - START");
            IncreaseIndent();

            int workerThreads;
            int completionPortThreads;

            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            WriteLine("GetMaxThreads.WorkerThreads: " + workerThreads);
            WriteLine("GetMaxThreads.CompletionPortThreads: " + completionPortThreads);

            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            WriteLine("GetMinThreads.WorkerThreads: " + workerThreads);
            WriteLine("GetMinThreads.CompletionPortThreads: " + completionPortThreads);

            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            WriteLine("GetAvailableThreads.WorkerThreads: " + workerThreads);
            WriteLine("GetAvailableThreads.WompletionPortThreads: " + completionPortThreads);

            DecreaseIndent();
            WriteLine("LogThreadPoolInfo - END");
        }

        //	Return message with included datetime information. We are using when logging.
        static String GetWithDateTimeAndThreadId(string msg)
        {
            return MyValueFormatter.GetFormatedDateTimeOffset(DateTimeOffset.Now) + " - " +
                "Thread: " + string.Format("{0,4}", GetThreadId()) + " ->  " +
                new String(' ', GetIdentByThread(GetThreadId()) * 3) +
                msg;
        }

        static int GetThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        //  Get actual ident for specified thread. If not specified yet, we assume it's zero.
        static int GetIdentByThread(int threadId)
        {
            int retVal;
            if (m_indentsByThread.TryGetValue(threadId, out retVal) == false)
            {
                retVal = 0;
            }

            //  If retVal is negative, then someone used wrong order of increase ident and decrease ident
            //  E.g. used MyMwcLog.DecreaseIndent(); at the start of a method whereas there should be MyMwcLog.IncreaseIndent();
            MyCommonDebugUtils.AssertDebug(retVal >= 0);

            return retVal;
        }
    }
}