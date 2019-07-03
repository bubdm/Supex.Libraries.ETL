using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            //var copyExampleJob = new CopyExampleJob("Copy Example Job");
            //copyExampleJob.ExecuteAsync().Wait();

            //var filterExampleJob = new FilterExampleJob("Filter Example Job");
            //filterExampleJob.ExecuteAsync().Wait();

            //var funnelExampleJob = new FunnelExampleJob("Funnel Example Job");
            //funnelExampleJob.ExecuteAsync().Wait();

            //var removeDuplicatesExampleJob = new RemoveDuplicatesExampleJob("RemoveDeuplicates Example Job");
            //removeDuplicatesExampleJob.ExecuteAsync().Wait();

            //var sortExampleJob = new SortExampleJob("Sort Example Job");
            //sortExampleJob.ExecuteAsync().Wait();

            var transformerExampleJob = new TransformerExampleJob("Transformer Example Job");
            transformerExampleJob.ExecuteAsync().Wait();

            Console.ReadKey();
        }
    }
}
