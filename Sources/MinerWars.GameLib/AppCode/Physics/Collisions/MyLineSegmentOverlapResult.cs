using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Physics
{
    struct MyLineSegmentOverlapResult<T>
    {
        public class MyLineSegmentOverlapResultComparer : IComparer<MyLineSegmentOverlapResult<T>>
        {
            public int Compare(MyLineSegmentOverlapResult<T> x, MyLineSegmentOverlapResult<T> y)
            {
                return x.Distance.CompareTo(y.Distance);
            }
        }

        public static MyLineSegmentOverlapResultComparer DistanceComparer = new MyLineSegmentOverlapResultComparer();

        public float Distance;
        public T Element;
    }
}
