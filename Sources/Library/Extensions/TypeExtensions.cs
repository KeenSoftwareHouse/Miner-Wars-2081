using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KeenSoftwareHouse.Library.Debugging;

namespace KeenSoftwareHouse.Library.Extensions
{
    /// <summary>
    /// Helper for runtime member info.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MemberHelper<T>
    {
        /// <summary>
        ///  Gets the memberinfo of field/property on instance class.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static MemberInfo GetMember<TValue>(Expression<Func<T,TValue>> selector)
        {
            Exceptions.ThrowIf<ArgumentNullException>(selector == null, "selector");

            var me = selector.Body as MemberExpression;

            Exceptions.ThrowIf<ArgumentNullException>(me == null, "Selector must be a member access expression", "selector");

            return me.Member;
        }
    }

    /// <summary>
    /// Helper for runtime member info.
    /// </summary>
    public static class MemberHelper
    {
        /// <summary>
        /// Gets the memberinfo of field/property on static class.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static MemberInfo GetMember<TValue>(Expression<Func<TValue>> selector)
        {
            Exceptions.ThrowIf<ArgumentNullException>(selector == null, "selector");

            var me = selector.Body as MemberExpression;

            Exceptions.ThrowIf<ArgumentNullException>(me == null, "Selector must be a member access expression", "selector");

            return me.Member;
        }
    }
}