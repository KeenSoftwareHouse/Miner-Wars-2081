using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTasks
{
    class DelegateWork: IWork
    {
        static Pool<DelegateWork> instances = new Pool<DelegateWork>();

        public Action Action { get; set; }
        public WorkOptions Options { get; set; }

        public DelegateWork()
        {
        }

        public void DoWork()
        {
            Action();
            instances.Return(System.Threading.Thread.CurrentThread, this);
        }

        internal static DelegateWork GetInstance()
        {
            return instances.Get(System.Threading.Thread.CurrentThread);
        }
    }
}
