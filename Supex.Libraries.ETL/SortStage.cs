using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public sealed class SortStage<TInput> : ProcessingStage where TInput : class
    {
        private ILink<TInput> _inputLink;
        private ILink<TInput> _outputLink;
        private readonly List<SortDescriptor> _sortDescriptors;

        public SortStage(string name) : base(name)
        {
            _sortDescriptors = new List<SortDescriptor>();
        }

        public override Task ExecuteAsync()
        {
            while (!_inputLink.AllEnqueued)
            {
                Task.Delay(1).Wait();
            }

            var inputs = _inputLink.DequeueAll();
            inputs = ApplySort(inputs);
            _outputLink.EnqueueAll(inputs);

            _outputLink.Complete();

            return Task.CompletedTask;
        }

        private IEnumerable<TInput> ApplySort(IEnumerable<TInput> inputs)
        {
            var results = inputs.AsQueryable().OrderBy(o => 0);
            foreach (var sortDescriptor in _sortDescriptors)
            {
                var lambda = (dynamic)CreateExpression(typeof(TInput), sortDescriptor.Property);

                results = sortDescriptor.Direction == SortDirection.Ascending 
                    ? Queryable.ThenBy(results, lambda) 
                    : Queryable.ThenByDescending(results, lambda);
            }

            return results;
        }

        private static LambdaExpression CreateExpression(Type type, string property)
        {
            var param = Expression.Parameter(type, "x");
            Expression body = param;
            body = Expression.PropertyOrField(body, property);
            return Expression.Lambda(body, param);
        }

        public SortStage<TInput> WithKey(Expression<Func<TInput, object>> input, SortDirection direction)
        {
            _sortDescriptors.Add(new SortDescriptor(input.GetMemberName(), direction));
            return this;
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
            return exceptions;
        }
    }
}
