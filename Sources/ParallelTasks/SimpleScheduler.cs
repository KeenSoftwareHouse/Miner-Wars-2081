using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ParallelTasks
{
    /// <summary>
    /// A simple work scheduler class, implemented with
    /// a blocking queue (producer-consumer).
    /// </summary>
    public class SimpleScheduler
        :IWorkScheduler
    {
#if XBOX
        static int affinityIndex;
#endif

        Stack<Task> scheduledItems;
        //Semaphore semaphore;
        AutoResetEvent fastLock;

        /// <summary>
        /// Creates a new instance of the <see cref="SimpleScheduler"/> class.
        /// </summary>
        public SimpleScheduler()
#if XBOX
            : this(3)     // MartinG@DigitalRune: I recommend using 3 hardware threads on the Xbox 360 
                          // (hardware threads 3, 4, 5). Hardware thread 1 usually runs the main game 
                          // logic and will automatically pick up a Task if it is idle and a Task is 
                          // still queued. My performance experiments (using an actual game) have shown 
                          // that using all 4 hardware threads is not optimal.
#elif WINDOWS_PHONE
            : this(1)     // Cannot access Environment.ProcessorCount on WP7. (Security issue.)
#else
            : this(Environment.ProcessorCount)
#endif
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SimpleScheduler"/> class.
        /// </summary>
        /// <param name="threadCount">The number of worker threads to create.</param>
        public SimpleScheduler(int threadCount)
        {
            scheduledItems = new Stack<Task>();
           // semaphore = new Semaphore(0);
            fastLock = new AutoResetEvent(false);

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(new ThreadStart(WorkerLoop));
                thread.IsBackground = true;
                thread.Name = "ParallelTasks Worker";
                thread.Start();
            }
        }

        void WorkerLoop()
        {
#if XBOX
            int i = Interlocked.Increment(ref affinityIndex) - 1;
            int affinity = Parallel.ProcessorAffinity[i % Parallel.ProcessorAffinity.Length];
            Thread.CurrentThread.SetProcessorAffinity((int)affinity);
#endif
            //semaphore.WaitOne();
            //mujevent.WaitOne();
            Task work = new Task();

            while (true)
            {
                fastLock.WaitOne();

                if (scheduledItems.Count > 0)
                {
                    bool foundWork = false;
                    lock (scheduledItems)
                    {
                        if (scheduledItems.Count > 0)
                        {
                            work = scheduledItems.Pop();
                            foundWork = true;
                        }
                    }

                    if (foundWork)
                    {
                        work.DoWork();
                        //semaphore.WaitOne();
                    }
                }  
                else
                {
                    var replicable = WorkItem.Replicable;
                    if (replicable.HasValue)
                        replicable.Value.DoWork();
                    WorkItem.SetReplicableNull(replicable);
                }
            }
        }

        /// <summary>
        /// Schedules a task for execution.
        /// </summary>
        /// <param name="work">The task to schedule.</param>
        public void Schedule(Task work)
        {
            //Unfortunatelly this can happen, who knows why
            if (work.Item.Work == null)
                return;

            int threads = work.Item.Work.Options.MaximumThreads;
            lock (scheduledItems)
                scheduledItems.Push(work);
            if (threads > 0)
                WorkItem.Replicable = work;
           // semaphore.Release();
           // semaphore.Release();
            fastLock.Set();
        }
    }
}
