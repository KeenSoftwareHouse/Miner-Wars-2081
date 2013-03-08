using System;

namespace MinerWars.AppCode.Game.Managers
{
    static class MyGarbageCollectionManager
    {
        static int m_lastGarbageCollectionsCount = 0;

        //  Total count of garbage collections from the start of this application
        public static int GetTotalGarbageCollectionsCount()
        {
            int count = 0;
            for (int i = 0; i <= GC.MaxGeneration; ++i)
            {
                count += GC.CollectionCount(i);
            }
            return count;
        }

        //  Count of garbage collections from last call to GetGarbageCollectionsCountFromLastCall()
        public static int GetGarbageCollectionsCountFromLastCall()
        {
            int total = GetTotalGarbageCollectionsCount();
            int result = total - m_lastGarbageCollectionsCount;
            m_lastGarbageCollectionsCount = total;
            return result;
        }
    }
}
