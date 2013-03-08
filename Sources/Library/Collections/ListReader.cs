using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime;

namespace KeenSoftwareHouse.Library.Collections
{
    public struct ListReader<T>: IEnumerable<T>, IEnumerable
    {
        private readonly List<T> m_list;

        public ListReader(List<T> list)
        {
            m_list = list;
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public List<T>.Enumerator GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
