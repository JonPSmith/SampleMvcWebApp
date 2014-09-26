#region licence
// The MIT License (MIT)
// 
// Filename: InternalsInfo.cs
// Date Created: 2014/07/14
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
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