using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public sealed class FunnelStage<TInput> : ProcessingStage where TInput : class
    {
        private readonly List<ILink<TInput>> _inputLinks;
        private ILink<TInput> _outputLink;

        public FunnelStage(string name) : base(name)
        {
            _inputLinks = new List<ILink<TInput>>();
        }

        public override Task ExecuteAsync()
        {
            var tasks = new List<Task>();

            foreach (var inputLink in _inputLinks)
            {
                tasks.Add(Task.Factory.StartNew(() => ProcessQueue(inputLink)));
            }

            Task.WaitAll(tasks.ToArray());
            _outputLink.Complete();

            return Task.CompletedTask;
        }

        private void ProcessQueue(ILink<TInput> inputLink)
        {
            TInput input;

            while ((input = inputLink.Dequeue()) != null)
            {
                _outputLink.Enqueue(input);
            }
        }

        public void LinkFrom(ILink<TInput> link)
        {
            _inputLinks.Add(link);
        }

        public void LinkTo(ILink<TInput> link)
        {
            _outputLink = link;
        }

        public override IReadOnlyCollection<StageValidationException> Validate()
        {
            var exceptions = new List<StageValidationException>();
            if (!_inputLinks.Any())
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have any input link."));
            }

            if (_outputLink == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the output link."));
            }
            return exceptions;
        }
    }
}
