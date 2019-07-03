using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Supex.Libraries.ETL
{
    public sealed class RemoveDuplicatesStage<TInput> : ProcessingStage where TInput : class
    {
        private ILink<TInput> _inputLink;
        private ILink<TInput> _outputLink;
        private readonly HashSet<string> _keys = new HashSet<string>();

        public RemoveDuplicatesStage(string name) : base(name)
        {
        }

        public override Task ExecuteAsync()
        {
            TInput input;
            string previousKeyComposite = null;

            while ((input = _inputLink.Dequeue()) != null)
            {
                var inputType = input.GetType();
                var inputProperties = inputType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var currentKeyComposite = JsonConvert.SerializeObject(inputProperties.OrderBy(s => s.Name)
                    .Where(s => _keys.Contains(s.Name)).Select(s => s.GetValue(input, null).ToString()));

                if (previousKeyComposite == currentKeyComposite) continue;

                previousKeyComposite = currentKeyComposite;
                _outputLink.Enqueue(input);
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

        public RemoveDuplicatesStage<TInput> WithKey(Expression<Func<TInput, object>> input)
        {
            _keys.Add(input.GetMemberName());
            return this;
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
            return exceptions;
        }
    }
}
