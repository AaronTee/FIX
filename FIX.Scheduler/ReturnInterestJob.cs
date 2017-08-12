using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIX.Scheduler
{
    [DisallowConcurrentExecution]
    class ReturnInterestJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Job Executed");
        }
    }
}
