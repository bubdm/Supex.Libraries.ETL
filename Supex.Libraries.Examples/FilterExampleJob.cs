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
    public class FilterExampleJob : Job
    {
        public FilterExampleJob(string name) : base(name)
        {
            var link1 = new Link<Employee>("link1");
            var link2 = new Link<Employee>("link2");

            var input = new RestInputStage<Employee>("input", "http://dummy.restapiexample.com/api/v1/employees");
            input.LinkTo(link1);

            var stage = new FilterStage<Employee>("filter", employee => employee.Salary > 2500000);
            stage.LinkFrom(link1);
            stage.LinkTo(link2);

            var output = new ConsoleOutputStage("output");
            output.LinkFrom(link2);

            AddStages(input, output, stage);
        }
    }
}
