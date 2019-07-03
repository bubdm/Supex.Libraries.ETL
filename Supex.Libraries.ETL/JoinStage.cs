using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public sealed class JoinStage<TInput1, TInput2, TOutput> : ProcessingStage where TInput1 : class where TInput2 : class where TOutput : class, new()
    {
        private ILink<TInput1> _inputLink1;
        private ILink<TInput2> _inputLink2;
        private ILink<TOutput> _outputLink;

        private readonly Func<TInput1, object> _input1Selector;
        private readonly Func<TInput2, object> _input2Selector;
        private readonly Func<TInput1, TInput2, TOutput> _resultSelector;

        public JoinStage(string name, Func<TInput1, object> input1Selector, Func<TInput2, object> input2Selector,
            Func<TInput1, TInput2, TOutput> resultSelector) : base(name)
        {
            _input1Selector = input1Selector;
            _input2Selector = input2Selector;
            _resultSelector = resultSelector;
        }

        public override Task ExecuteAsync()
        {
            while (!_inputLink1.AllEnqueued || !_inputLink2.AllEnqueued)
            {
                Task.Delay(1).Wait();
            }

            var input1S = new List<TInput1>();
            var input2S = new List<TInput2>();

            input1S.AddRange(_inputLink1.DequeueAll());
            input2S.AddRange(_inputLink2.DequeueAll());

            var outputs = input1S.Join(input2S, _input1Selector, _input2Selector, _resultSelector).ToList();
            outputs.ForEach(output => _outputLink.Enqueue(output));

            _outputLink.Complete();

            return Task.CompletedTask;
        }

        public void LinkFrom(ILink<TInput1> inputLink1, ILink<TInput2> inputLink2)
        {
            _inputLink1 = inputLink1;
            _inputLink2 = inputLink2;
        }

        public void LinkTo(ILink<TOutput> link)
        {
            _outputLink = link;
        }

        public override IReadOnlyCollection<StageValidationException> Validate()
        {
            var exceptions = new List<StageValidationException>();
            if (_inputLink1 == null || _inputLink2 == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have two input links."));
            }

            if (_outputLink == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the output link."));
            }

            return exceptions;
        }
    }
}
