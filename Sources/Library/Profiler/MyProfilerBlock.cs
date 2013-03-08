#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

#endregion

namespace KeenSoftwareHouse.Library.Profiler
{
    public partial class MyProfiler
    {
        public class MyProfilerBlock
        {
            public Stopwatch Stopwatch = new Stopwatch();
            public StringBuilder Name;
            public long StartManagedMB = 0;
            public long EndManagedMB = 0;

            public float DeltaManagedB = 0; //?
            public float TotalManagedMB = 0; //?


            public long StartProcessMB = 0;
            public long EndProcessMB = 0;
            public float DeltaProcessB = 0;       //?
            public float TotalProcessMB = 0;      //?

            public bool Invalid = false;

            public float CustomValue = 0;

            public float ManagedDeltaMB
            {
                //conversion to MB
                get { return (DeltaManagedB) * 0.000000953674f; }
            }

            public float ProcessDeltaMB
            {
                //conversion to MB
                get { return (DeltaProcessB) * 0.000000953674f; }
            }


            public float[] ProcessMemory = new float[MAX_FRAMES];

            public uint color;
            public float[] Miliseconds = new float[MAX_FRAMES];
            public float[] ManagedMemory = new float[MAX_FRAMES];
            public float[] CustomValues = new float[MAX_FRAMES];
            public int[] NumCallsArray = new int[MAX_FRAMES];

            public bool Enabled = false;
            public bool DrawGraph = true;
            public bool Assigned = false;
            public float averagePctg = 0;
            public float averageMiliseconds = 0;
            public static int IndexCounter = 0;

            public List<MyProfilerBlock> Children = new List<MyProfilerBlock>();
            public List<MyProfilerBlock> StackChildren = new List<MyProfilerBlock>();
            public MyProfilerBlock Parent = null;
            public MyProfilerBlock StackParent = null;

            public int NumCalls = 0;
            public int NumChildCalls = 0;
            public int Cooldown = 1000;
            public int ThreadId = -1;
            public int Id;

            public int BlockHash = 0;

            public Stack<string> DebugStackTrace = new Stack<string>();

            public MyProfilerBlock(string name)
            {
                Name = new StringBuilder(name);
                BlockHash = Name.GetHashCode();
                ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            }

            public void Start(bool memoryProfiling)
            {
                NumCalls++;

                if (memoryProfiling)
                {
                    StartManagedMB = System.GC.GetTotalMemory(false);   // About 1ms per 2000 calls
                }

                if (StackChecking && Name.ToString() != "FPS")
                {
                    DebugStackTrace.Push(GetStackTrace());
                }

                if (memoryProfiling)
                {
                    StartProcessMB = System.Environment.WorkingSet;   // WorkingSet is allocating memory in each call, also its expensive (about 7ms per 2000 calls).
                }

                Stopwatch.Start();
            }

            private string GetStackTrace()
            {
                var stack = new StackTrace();
                string result = String.Empty;
                int i;
                for (i = stack.FrameCount - 1; i >= 0; i--)
                {
                    var declaringType = stack.GetFrame(i).GetMethod().DeclaringType;
                    if (typeof(MyProfiler).IsAssignableFrom(declaringType))
                    {
                        break;
                    }
                }
                return new StackTrace(i + 1).ToString();
            }

            public void End(bool memoryProfiling)
            {
                Stopwatch.Stop();

                //if(MyProfiler.EnableAsserts)
                //    System.Diagnostics.Debug.Assert(threadId == System.Threading.Thread.CurrentThread.ManagedThreadId, "You must run MyProfilerBlock within same thread!");



                if (memoryProfiling)
                {
                    EndManagedMB = System.GC.GetTotalMemory(false);
                    EndProcessMB = System.Environment.WorkingSet;
                }

                if (StackChecking && Name.ToString() != "FPS")
                {
                    string currentStact = GetStackTrace();
                    Debug.Assert(DebugStackTrace.Peek() == currentStact, "Stack traces does not match, probably unclosed block");
                    DebugStackTrace.Pop();
                }

                DeltaManagedB += EndManagedMB - StartManagedMB;
                if (memoryProfiling)
                {
                    DeltaProcessB += EndProcessMB - StartProcessMB;
                }
            }

            public int GetNumChildCalls(int recursionLevel = 0)
            {
                int calls = 0;

                if (recursionLevel > 1000)
                {
                    // We are probably in endless loop. 

                    if (EnableAsserts)
                        System.Diagnostics.Trace.Assert(false, "We are probably in endless loop. Check tree hierarchy!");

                    return 1000000;
                }

                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].NumChildCalls == -1)
                    {
                        Children[i].NumChildCalls = Children[i].GetNumChildCalls(++recursionLevel);
                    }

                    calls += Children[i].NumChildCalls;
                }

                return calls + NumCalls;
            }

            public override string ToString()
            {
                return Name + " (" + NumCalls.ToString() + " calls)";
            }
        }
    }
}
