using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SampleWebApp.Models
{
    public class AboutInfo
    {

        public int WorkerThreads { get; private set; }

        public int AvailableThreads { get; private set; }

        public AboutInfo()
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

            int availableThreads;
            ThreadPool.GetAvailableThreads(out availableThreads, out completionPortThreads);

            WorkerThreads = workerThreads;
            AvailableThreads = availableThreads;
        }
    }
}