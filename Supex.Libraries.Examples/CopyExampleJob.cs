using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supex.Libraries.Examples.Models;
using Supex.Libraries.Examples.Stages;
using Supex.Libraries.ETL;

namespace Supex.Libraries.Examples
{
    public class CopyExampleJob : Job
    {
        public CopyExampleJob(string name) : base(name)
        {
            try
            {
                var link1 = new Link<Employee>("link1");
                var link2 = new Link<Employee>("link2");
                var link3 = new Link<Employee>("link3");

                var input = new RestInputStage<Employee>("input", "http://dummy.restapiexample.com/api/v1/employees");
                input.LinkTo(link1);

                var copy = new CopyStage<Employee>("copy");
                copy.LinkFrom(link1);
                copy.LinkTo(link2);
                copy.LinkTo(link3);

                var output1 = new ConsoleOutputStage("output1");
                output1.LinkFrom(link2);

                var output2 = new ConsoleOutputStage("output2");
                output2.LinkFrom(link3);

                AddStages(output1, output2, input, copy);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
