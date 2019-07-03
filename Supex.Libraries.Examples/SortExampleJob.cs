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
    public class SortExampleJob : Job
    {
        public SortExampleJob(string name) : base(name)
        {
            var link1 = new Link<Employee>("link1");
            var link2 = new Link<Employee>("link2");

            var input = new RestInputStage<Employee>("input1", "http://dummy.restapiexample.com/api/v1/employees");
            input.LinkTo(link1);

            var sort = new SortStage<Employee>("sort")
                .WithKey(s => s.Name, SortDirection.Ascending)
                .WithKey(s => s.Age, SortDirection.Descending);
            sort.LinkFrom(link1);
            sort.LinkTo(link2);

            var output = new ConsoleOutputStage("output");
            output.LinkFrom(link2);

            AddStages(output, input, sort);
        }
    }
}
