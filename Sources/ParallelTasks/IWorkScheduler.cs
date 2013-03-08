using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTasks
{
    /// <summary>
    /// An interface defining a task scheduler.
    /// </summary>
    public interface IWorkScheduler
    {
        /// <summary>
        /// Schedules a task for execution.
        /// </summary>
        /// <param name="item">The task to schedule.</param>
        void Schedule(Task item);
    }
}