using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public sealed class CopyStage<TInput> : ProcessingStage where TInput : class
    {
        private ILink<TInput> _inputLink;
        private readonly List<ILink<TInput>> _outputLinks;

        public CopyStage(string name) : base(name)
        {
            _outputLinks = new List<ILink<TInput>>();
        }

        public override Task ExecuteAsync()
        {
            TInput input;

            while ((input = _inputLink.Dequeue()) != null)
            {
                foreach (var outputLink in _outputLinks)
                {
                    outputLink.Enqueue(input);
                }
            }

            _outputLinks.ForEach(outputLink => outputLink.Complete());

            return Task.CompletedTask;
        }

        public void LinkFrom(ILink<TInput> link)
        {
            _inputLink = link;
        }

        public void LinkTo(ILink<TInput> link)
        {
            _outputLinks.Add(link);
        }

        public override IReadOnlyCollection<StageValidationException> Validate()
        {
            var exceptions = new List<StageValidationException>();
            if (_inputLink == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the input link."));
            }

            if (!_outputLinks.Any())
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have any output link."));
            }
            return exceptions;
        }
    }
}
