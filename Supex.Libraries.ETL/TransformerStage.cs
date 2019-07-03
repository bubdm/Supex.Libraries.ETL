using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public sealed class TransformerStage<TInput> : ProcessingStage where TInput : class
    {
        private ILink<TInput> _inputLink;
        private readonly List<dynamic> _outputLinks;
        private readonly Dictionary<string, dynamic> _mapper;
        private readonly Dictionary<string, Func<TInput, bool>> _filters;

        public TransformerStage(string name) : base(name)
        {
            _outputLinks = new List<dynamic>();
            _mapper = new Dictionary<string, dynamic>();
            _filters = new Dictionary<string, Func<TInput, bool>>();
        }

        public override Task ExecuteAsync()
        {
            TInput input;

            while ((input = _inputLink.Dequeue()) != null)
            {
                foreach (var outputLink in _outputLinks)
                {
                    var passFilter = true;

                    if (_filters.ContainsKey(outputLink.Name))
                    {
                        passFilter = (bool) _filters[outputLink.Name](input);
                    }

                    if (passFilter)
                    {
                        outputLink.Enqueue(_mapper.ContainsKey(outputLink.Name)
                            ? _mapper[outputLink.Name].Map(input)
                            : input);
                    }
                }
            }

            _outputLinks.ForEach(outputLink => outputLink.Complete());

            return Task.CompletedTask;
        }

        public void LinkFrom(ILink<TInput> link)
        {
            _inputLink = link;
        }

        public Mapper<TInput, TOutput> LinkTo<TOutput>(ILink<TOutput> link) where TOutput : class
        {
            _outputLinks.Add(link);

            var mapper = new Mapper<TInput, TOutput>();
            _mapper.Add(link.Name, mapper);

            return mapper;
        }

        public Mapper<TInput, TOutput> LinkTo<TOutput>(ILink<TOutput> link, Func<TInput, bool> filter) where TOutput : class
        {
            _outputLinks.Add(link);

            _filters.Add(link.Name, filter);
            var mapper = new Mapper<TInput, TOutput>();
            _mapper.Add(link.Name, mapper);

            return mapper;
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
