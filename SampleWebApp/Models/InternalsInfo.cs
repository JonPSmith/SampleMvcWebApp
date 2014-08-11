using System;
using System.Diagnostics;
using System.Threading;

namespace SampleWebApp.Models
{
    public class InternalsInfo
    {

        public int WorkerThreads { get; private set; }

        public int AvailableThreads { get; private set; }

        public int AvailableMbytes { get; private set; }

        public int HeapMemoryUsedKbytes { get; private set; }

        public InternalsInfo()
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

            int availableThreads;
            ThreadPool.GetAvailableThreads(out availableThreads, out completionPortThreads);

            WorkerThreads = workerThreads;
            AvailableThreads = availableThreads;

            AvailableMbytes = (int)new PerformanceCounter("Memory", "Available MBytes", true).RawValue;

            HeapMemoryUsedKbytes = (int)(GC.GetTotalMemory(true)/1000);
        }
    }
}