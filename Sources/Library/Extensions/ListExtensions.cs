using System;
using System.Collections;
using System.Collections.Generic;

namespace KeenSoftwareHouse.Library.Extensions
{
    public static class ListExtensions
    {
        #region IList Methods

        /// <summary>
        /// Remove element at index by replacing it with last element in list.
        /// Removing is very fast but it breaks order of items in list!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        public static void RemoveAtFast<T>(this IList<T> list, int index)
        {
            int lastPos = list.Count - 1;

            list[index] = list[lastPos];
            list.RemoveAt(lastPos);
        }

        #endregion
    }
}