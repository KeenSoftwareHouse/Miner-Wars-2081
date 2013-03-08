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
    // This is part of MyRenderProfiler. I have moved it here, to be accessible from MinerWarsCommonLIB.
    public partial class MyProfiler
    {
        static public bool EnableAsserts = true;
        static public bool StackChecking = false;
        public static readonly int MAX_FRAMES = 1024;

        protected Dictionary<int, MyProfilerBlock> m_profilingBlocks = new Dictionary<int, MyProfilerBlock>(65536);
        protected Stack<MyProfilerBlock> m_currentProfilingStack = new Stack<MyProfilerBlock>(1024);
        protected MyProfiler.MyProfilerBlock m_selectedRoot = null;

        public MyProfiler()
        {
            m_managedThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public Dictionary<int, MyProfilerBlock> ProfilingBlocks
        {
            get { return m_profilingBlocks; }
        }

        public MyProfiler.MyProfilerBlock SelectedRoot
        {
            get { return m_selectedRoot; }
            set { m_selectedRoot = value; }
        }

        public Stack<MyProfilerBlock> CurrentProfilingStack
        {
            get { return m_currentProfilingStack; }
        }

        public int LevelSkipCount = 0;

        int m_managedThreadId;

        StringBuilder GetParentName()
        {
            if(m_currentProfilingStack.Count > 0)
                return m_currentProfilingStack.Peek().Name;

            return new StringBuilder("<root>");
        }

        int GetParentHash()
        {
            if (m_currentProfilingStack.Count > 0)
                return m_currentProfilingStack.Peek().BlockHash;

            return 0;
        }

        static StringBuilder m_nameComposer = new StringBuilder(1024);
        static StringBuilder m_nameSplitter = new StringBuilder(" : ");

        public int StartMyProfilingBlock(string name, bool memoryProfiling, float customValue = 0)
        {
            if (m_managedThreadId != Thread.CurrentThread.ManagedThreadId)
                return -1;

            //lock (m_lock)
            {
                if (MyProfiler.EnableAsserts)
                {        /*
                    foreach (MyProfilerBlock block in m_currentProfilingStack)
                    {
                        if (block.Name.ToString() == name)
                        {
                            System.Diagnostics.Debug.Assert(false, "Event is already in stack!");
                            return -1;
                        }
                    }      */
                }
                 
                MyProfilerBlock profilingBlock = null;
                int key;

                unchecked
                {
                    key = (397 * name.GetHashCode()) ^ GetParentHash();
                }

                int id = 1;
                
                //if (!m_profilingBlocks.TryGetValue(m_nameComposer.ToString(), out profilingBlock))
                if (!m_profilingBlocks.TryGetValue(key, out profilingBlock))
                {
                    profilingBlock = new MyProfilerBlock(name/*m_nameComposer.ToString()*/);

                    if (m_currentProfilingStack.Count > 0)
                    {
                        MyProfilerBlock parentBlock = m_currentProfilingStack.Peek();
                        if (!parentBlock.Children.Contains(profilingBlock))
                            parentBlock.Children.Add(profilingBlock);

                        profilingBlock.Parent = m_currentProfilingStack.Peek();
                    }

                    m_profilingBlocks.Add(key, profilingBlock);
                    profilingBlock.Id = m_profilingBlocks.Count - 1;
                }

                id = profilingBlock.Id;
                
                profilingBlock.Start(memoryProfiling);
                profilingBlock.CustomValue = customValue;
                                   
                m_currentProfilingStack.Push(profilingBlock);

                return id;
            }
        }

        public void EndMyProfilingBlock(int id, bool memoryProfiling)
        {
            if (EnableAsserts) //Check that thread which begun this block is same which ends it
                System.Diagnostics.Debug.Assert(m_managedThreadId == Thread.CurrentThread.ManagedThreadId);

            if (LevelSkipCount > 0)
            {
                LevelSkipCount--;
                return;
            }

            MyProfilerBlock profilingBlock = m_currentProfilingStack.Pop();
            if (profilingBlock != null)
            {
                if (EnableAsserts && id != -1)
                {
                    System.Diagnostics.Debug.Assert(profilingBlock.Id == id, "Corrupted profilling stack, mismatched IDs");
                }

                profilingBlock.End(memoryProfiling);
            }
            else
            if (EnableAsserts)
            {
                System.Diagnostics.Debug.Assert(false, "Corrupted profilling stack");
            }             
        }

        public void MyProfileCustomValue(string name, float value, bool memoryProfiling)
        {
            int id = StartMyProfilingBlock(name, memoryProfiling, value);
            EndMyProfilingBlock(id, memoryProfiling);
        }
    }
}
