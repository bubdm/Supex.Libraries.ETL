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
    public class TransformerExampleJob : Job
    {
        public TransformerExampleJob(string name) : base(name)
        {
            var link1 = new Link<Employee>("link1");
            var link2 = new Link<Employee>("link2");
            var link3 = new Link<Employee>("link3");

            var input = new RestInputStage<Employee>("input1", "http://dummy.restapiexample.com/api/v1/employees");
            input.LinkTo(link1);

            var stage = new TransformerStage<Employee>("transform");
            stage.LinkFrom(link1);
            stage.LinkTo(link2)
                .ForMember(s => s.Name, d => d.Name + " Transformed");
            stage.LinkTo(link3, employee => employee.Age < 20)
                .ForMember(s => s.Salary, d => 10m);

            var output1 = new ConsoleOutputStage("output1");
            output1.LinkFrom(link2);
            var output2 = new ConsoleOutputStage("output2");
            output2.LinkFrom(link3);

            AddStages(input, output1, output2, stage);
        }
    }
}
