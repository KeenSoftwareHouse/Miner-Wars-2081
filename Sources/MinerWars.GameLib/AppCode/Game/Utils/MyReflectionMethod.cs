namespace MinerWars.AppCode.Game.Utils
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    internal static class MyReflectionMethod
    {
        // for basic use m_method:
        private const string m_methodname = "m_method";


        // Returns method handle:
        private static RuntimeMethodHandle GetDynMethodRuntimeHandle(MethodBase method, String methodName)
        {
            if (method is DynamicMethod)
            {
                return (RuntimeMethodHandle)typeof(DynamicMethod).GetField(methodName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(method);
            }
            return method.MethodHandle;
        }


        // Return offsetted pointer for the current version of the .NET:
        private static unsafe IntPtr GetDynMethodAddr(MethodBase method)
        {
            RuntimeMethodHandle dynMethodRuntimeHandle = GetDynMethodRuntimeHandle(method, m_methodname);
            byte* bptr = (byte*)dynMethodRuntimeHandle.Value.ToPointer();
            if (testNET20sp20orGreater())
            {
                RuntimeHelpers.PrepareMethod(dynMethodRuntimeHandle);
                if (IntPtr.Size == 8)
                {
                    ulong* ulPtr2 = (ulong*)bptr;
                    ulPtr2 = (ulong*)ulPtr2[5];
                    return new IntPtr((void*)(ulPtr2 + 12));
                }
                uint* numPtr3 = (uint*)bptr;
                numPtr3 = (uint*)numPtr3[5];
                return new IntPtr((void*)(numPtr3 + 12));
            }
            if (IntPtr.Size == 8)
            {
                ulong* ulPtr4 = (ulong*)bptr;
                ulPtr4 += 6;
                return new IntPtr((void*)ulPtr4);
            }
            uint* numPtr5 = (uint*)bptr;
            numPtr5 += 6;
            return new IntPtr((void*)numPtr5);
        }


        // Test of .NET version. Offsets depends on it:
        private static bool testNET20sp20orGreater()
        {
            //Version Net20 = new Version(2, 0, 0xc627, 0x2a);
            //Version Net20SP1 = new Version(2, 0, 0xc627, 0x599);
            Version Net20SP2 = new Version(2, 0, 0xc627, 0xbed);
            //Version Net30 = new Version(3, 0, 0x119a, 30);
            //Version Net30SP1 = new Version(3, 0, 0x119a, 0x288);
            //Version Net30SP2 = new Version(3, 0, 0x119a, 0x868);
            //Version Net35 = new Version(3, 5, 0x521e, 8);
            //Version Net35SP1 = new Version(3, 5, 0x7809, 1);
            return !(Environment.Version.Major <= Net20SP2.Major && (
                (Environment.Version.Major != Net20SP2.Major) ||
                Environment.Version.MinorRevision < Net20SP2.MinorRevision));
        }


        // Returns method pointer in .NET 2.0 SP 2 version:
        private static IntPtr GetMethodAddr20SP2(MethodBase method)
        {
            //return new IntPtr((method.MethodHandle.Value.ToPointer() + (void*)8));
            return IntPtr.Add(method.MethodHandle.Value, 8);
        }


        // Returns pointer of the method (system address):
        public static unsafe IntPtr GetMethodAddr(MethodBase method)
        {
            if (method is DynamicMethod)
            {
                return GetDynMethodAddr(method);
            }
            RuntimeHelpers.PrepareMethod(method.MethodHandle);
            if (testNET20sp20orGreater())
            {
                return GetMethodAddr20SP2(method);
            }
            int num = (int)((*(((long*)method.MethodHandle.Value.ToPointer())) >> 0x20) & 0xffL);
            if (IntPtr.Size == 8)
            {
                ulong* numPtr3 = (ulong*)IntPtr.Add(IntPtr.Add(method.DeclaringType.TypeHandle.Value, (num * 8)), 80).ToPointer();
                return new IntPtr((void*)numPtr3);
            }
            uint* numPtr5 = (uint*)IntPtr.Add(IntPtr.Add(method.DeclaringType.TypeHandle.Value, (num * 4)), 40).ToPointer();
            return new IntPtr((void*)numPtr5);
        }


        // Return the type of the returned value of the method:
        private static Type GetMethodReturnType(MethodBase method)
        {
            MethodInfo info = method as MethodInfo;
            if (info == null)
            {
                throw new ArgumentException("Unsupported MethodBase : " + method.GetType().Name, "method");
            }
            return info.ReturnType;
        }


        // Test signature of the two base methods:
        private static bool MethodSignaturesEqual(MethodBase x, MethodBase y)
        {
            if (x.CallingConvention != y.CallingConvention)
            {
                return false;
            }
            Type methodReturnType = GetMethodReturnType(x);
            Type type2 = GetMethodReturnType(y);
            if (methodReturnType != type2)
            {
                return false;
            }
            ParameterInfo[] parameters = x.GetParameters();
            ParameterInfo[] infoArray2 = y.GetParameters();
            if (parameters.Length != infoArray2.Length)
            {
                return false;
            }
            return true;
        }


        // Replace the source method in the assembly with MethodBase of the new function:
        public static unsafe void ReplaceMethod(IntPtr srcAdr, MethodBase dest)
        {
            IntPtr methodAddress = GetMethodAddr(dest);
            if (IntPtr.Size == 8) // depends on the .NET version
            {
                *((long*)methodAddress.ToPointer()) = *((long*)srcAdr.ToPointer());
            }
            else
            {
                *((int*)methodAddress.ToPointer()) = (int)*((uint*)srcAdr.ToPointer());
            }
        }


        // Safe variant of the replace source method in the assembly with MethodBase of the function:
        public static void ReplaceMethod(MethodBase source, MethodBase dest)
        {
            if (!MethodSignaturesEqual(source, dest))
            {
                throw new ArgumentException("The method signatures are not the same.", "source");
            }
            ReplaceMethod(GetMethodAddr(source), dest);
        }
    }
}
