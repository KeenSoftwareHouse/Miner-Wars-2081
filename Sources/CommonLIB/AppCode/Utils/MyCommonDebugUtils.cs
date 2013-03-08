using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace SysUtils.Utils
{
    //  Thrown when release assert is triggered (in debug or release mode)
    public class MyMwcExceptionAssertRelease : Exception
    {
    }


    //  This exception is thrown when we reach code 'Application should not get here'
    public class MyMwcExceptionApplicationShouldNotGetHere : Exception
    {
    }

    public class MyMaterialNotAvailableException : Exception
    {
        public string AssetName { get; set; }
        public int MaterialIndex { get; set; }

        public MyMaterialNotAvailableException(string assetName, int materialIndex)
        {
            AssetName = assetName;
            MaterialIndex = materialIndex;
        }
    }


    public class MyCommonDebugUtils
    {
        /// <summary>
        /// This "assert" is executed in DEBUG and RELEASE modes. Use it in code that that won't suffer from more work (e.g. loading), not in frequently used loops
        /// </summary>
        /// <param name="condition"></param>
        public static void AssertRelease(bool condition)
        {
            AssertRelease(condition, "Assert release occured");
        }

        /// <summary>
        /// This "assert" is executed in DEBUG and RELEASE modes. Use it in code that that won't suffer from more work (e.g. loading), not in frequently used loops
        /// </summary>
        /// <param name="condition"></param>
        public static void AssertRelease(bool condition, string assertMessage)
        {
            if (condition == false)
            {
                MyMwcLog.WriteLine("Assert: " + assertMessage);
            }

            System.Diagnostics.Trace.Assert(condition, assertMessage);
        }

        /// <summary>
        /// This "assert" is executed in DEBUG mode. Because people dont know how to use AssertRelease!
        /// </summary>
        /// <param name="condition"></param>
        [Conditional("DEBUG")]
        public static void AssertDebug(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition);
        }

        /// <summary>
        /// This "assert" is executed in DEBUG mode. Because people dont know how to use AssertRelease!
        /// </summary>
        /// <param name="condition"></param>
        [Conditional("DEBUG")]
        public static void AssertDebug(bool condition, string assertMessage)
        {
            System.Diagnostics.Debug.Assert(condition, assertMessage);
        }

        /// <summary>
        /// Returns true if float is valid
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool IsValid(float f)
        {
            return !float.IsNaN(f) && !float.IsInfinity(f);
        }

        /// <summary>
        /// Returns true if double is valid
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool IsValid(double d)
        {
            return !double.IsNaN(d) && !double.IsInfinity(d);
        }

        //  C# enums allow duplicite values, there's not compilation error or warning. It's very dangerous. That's why we made
        //  this assert that will go over all values on an enum, and crash if there's a duplicity
        //  http://stackoverflow.com/questions/1425777/how-to-prevent-duplicate-values-in-enum
        public static void AssertEnumNotDuplicities<T, U>()
        {
            Dictionary<U, bool> valid = new Dictionary<U, bool>();

            foreach (U key in Enum.GetValues(typeof(T)))
            {
                bool val;
                if (valid.TryGetValue(key, out val) == true)
                {
                    throw new Exception("Duplicate enum found: " + key + " in " + typeof(T).Name);
                }
                else
                {
                    valid[key] = true;
                }
            }
        }

        public static void AssertEnumNotDuplicities(Type enumType)
        {
            Dictionary<object, bool> valid = new Dictionary<object, bool>();

            foreach (object key in Enum.GetValues(enumType))
            {
                bool val;
                if (valid.TryGetValue(key, out val) == true)
                {
                    throw new Exception("Duplicate enum found: " + key + " in " + enumType.AssemblyQualifiedName);
                }
                else
                {
                    valid[key] = true;
                }
            }
        }                
    }
}
