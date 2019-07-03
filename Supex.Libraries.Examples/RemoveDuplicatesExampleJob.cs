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
    public class RemoveDuplicatesExampleJob : Job
    {
        public RemoveDuplicatesExampleJob(string name) : base(name)
        {
            var link1 = new Link<Employee>("link1");
            var link2 = new Link<Employee>("link2");

            var input = new RestInputStage<Employee>("input1", "http://dummy.restapiexample.com/api/v1/employees");
            input.LinkTo(link1);

            var stage = new RemoveDuplicatesStage<Employee>("removeDuplicates")
                .WithKey(s => s.Name);
            stage.LinkFrom(link1);
            stage.LinkTo(link2);

            var output = new ConsoleOutputStage("output");
            output.LinkFrom(link2);

            AddStages(input, output, stage);
        }
    }
}
