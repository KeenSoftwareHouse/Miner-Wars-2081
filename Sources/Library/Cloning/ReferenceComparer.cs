using System;
using System.Collections.Generic;

namespace KeenSoftwareHouse.Library.Cloning
{
    /// <summary>
    /// Dictionary comparer for comparing objects by reference reguardless of having
    /// the GetHashCode / Equals override implemented on an object.
    /// </summary>
    public sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        /// <summary>
        /// Returns true if the two objects are the same instance
        /// </summary>
        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return Object.ReferenceEquals(x, y);
        }

        /// <summary>
        /// Returns a hash code the instance of the object
        /// </summary>
        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
