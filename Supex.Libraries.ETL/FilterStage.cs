using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public sealed class FilterStage<TInput> : ProcessingStage where TInput : class
    {
        private ILink<TInput> _inputLink;
        private ILink<TInput> _outputLink;
        private readonly Func<TInput, bool> _filter;

        public FilterStage(string name, Func<TInput, bool> filter) : base(name)
        {
            _filter = filter;
        }

        public override Task ExecuteAsync()
        {
            TInput input;

            while ((input = _inputLink.Dequeue()) != null)
            {
                var passFilter = _filter(input);
                if (passFilter)
                {
                    _outputLink.Enqueue(input);
                }
            }

            _outputLink.Complete();

            return Task.CompletedTask;
        }

        public void LinkFrom(ILink<TInput> link)
        {
            _inputLink = link;
        }

        public void LinkTo(ILink<TInput> link)
        {
            _outputLink = link;
        }

        public override IReadOnlyCollection<StageValidationException> Validate()
        {
            var exceptions = new List<StageValidationException>();
            if (_inputLink == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the input link."));
            }

            if (_outputLink == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the output link."));
            }

            if (_filter == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the filter."));
            }

            return exceptions;
        }
    }
}
