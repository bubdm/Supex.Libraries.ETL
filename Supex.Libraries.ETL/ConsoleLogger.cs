using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public class ConsoleLogger : ILogger
    {
        public void Write(Guid correlationId, string jobName, string message)
        {
            Console.WriteLine($"{DateTime.Now} | {correlationId} | {jobName} | {message}");
        }
    }
}
