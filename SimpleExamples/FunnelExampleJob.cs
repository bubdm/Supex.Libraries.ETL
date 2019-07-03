using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleExamples.Models;
using SimpleExamples.Stages;
using Supex.Libraries.ETL;

namespace SimpleExamples
{
    public class FunnelExampleJob : Job
    {
        public FunnelExampleJob(string name) : base(name)
        {
            var link1 = new Link<Employee>("link1");
            var link2 = new Link<Employee>("link2");
            var link3 = new Link<Employee>("link3");

            var input1 = new RestInputStage<Employee>("input1", "http://dummy.restapiexample.com/api/v1/employees");
            input1.LinkTo(link1);

            var input2 = new RestInputStage<Employee>("input2", "http://dummy.restapiexample.com/api/v1/employees");
            input2.LinkTo(link1);

            var funnel = new FunnelStage<Employee>("funnel");
            funnel.LinkFrom(link1);
            funnel.LinkFrom(link2);
            funnel.LinkTo(link3);

            var output = new ConsoleOutputStage("output");
            output.LinkFrom(link3);

            AddStages(output, input1, input2, funnel);
        }
    }
}
