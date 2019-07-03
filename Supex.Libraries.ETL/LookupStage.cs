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
    public sealed class LookupStage<TInput, TReference, TOutput> : ProcessingStage where TInput : class where TReference : class where TOutput : class
    {
        private ILink<TInput> _inputLink;
        private ILink<TReference> _referenceLink;
        private ILink<TOutput> _outputLink;
        private ILink<TInput> _rejetLink;
        private readonly Mapper<TInput, TOutput> _mapper;
        private readonly Dictionary<string, TReference> _references;
        private readonly List<KeyMapping> _keyMappings;
        private readonly Dictionary<string, Expression<Func<TReference, object>>> _mappings;

        public LookupStage(string name) : base(name)
        {
            _mapper = new Mapper<TInput, TOutput>();
            _references = new Dictionary<string, TReference>();
            _keyMappings = new List<KeyMapping>();
            _mappings = new Dictionary<string, Expression<Func<TReference, object>>>();
        }

        public override Task ExecuteAsync()
        {
            while (!_referenceLink.AllEnqueued)
            {
                Task.Delay(1).Wait();
            }

            var references = _referenceLink.DequeueAll();
            foreach (var reference in references)
            {
                _references.Add(CreateKey(reference, _keyMappings.Select(s => s.ReferenceKey)), reference);
            }

            TInput input;
            while ((input = _inputLink.Dequeue()) != null)
            {
                var key = CreateKey(input, _keyMappings.Select(s => s.InputKey));
                if (!_references.ContainsKey(key))
                {
                    if (_rejetLink == null) continue;

                    _rejetLink.Enqueue(input);
                }

                var output = _mapper.Map(input);

                var outputProperties = output.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in outputProperties.Where(s => _mappings.Keys.Contains(s.Name)))
                {
                    var reference = _references[key];
                    var propName = _mappings[prop.Name].GetMemberName();
                    var referenceProp = reference.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => x.Name == propName);
                    if (referenceProp != null)
                    {
                        prop.SetValue(output, referenceProp.GetValue(reference, null), null);
                    }
                }

                _outputLink.Enqueue(output);
            }

            _outputLink.Complete();

            return Task.CompletedTask;
        }

        private string CreateKey(dynamic obj, IEnumerable<string> properties)
        {
            var type = obj.GetType();
            var typeProperties = (PropertyInfo[])type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var key = JsonConvert.SerializeObject(typeProperties.OrderBy(s => s.Name)
                .Where(s => properties.Contains(s.Name)).Select(s => s.GetValue(obj, null).ToString()));

            return key;
        }

        public LookupStage<TInput, TReference, TOutput> WithKey(Expression<Func<TInput, object>> input, Expression<Func<TReference, object>> reference)
        {
            _keyMappings.Add(new KeyMapping(input.GetMemberName(), reference.GetMemberName()));
            return this;
        }

        public LookupStage<TInput, TReference, TOutput> WithMapping(Expression<Func<TOutput, object>> output, Expression<Func<TReference, object>> reference)
        {
            var property = output.GetMemberName();
            if (_mappings.ContainsKey(property))
            {
                throw new StageValidationException("The output property is allowed to have one mapping.");
            }
            _mappings.Add(property, reference);
            return this;
        }

        public void LinkFrom(ILink<TInput> inputLink, ILink<TReference> referenceLink)
        {
            _inputLink = inputLink;
            _referenceLink = referenceLink;
        }

        public void LinkTo(ILink<TOutput> link)
        {
            _outputLink = link;
        }

        public void RejectTo(ILink<TInput> link)
        {
            _rejetLink = link;
        }

        public override IReadOnlyCollection<StageValidationException> Validate()
        {
            var exceptions = new List<StageValidationException>();
            if (_inputLink == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the input link."));
            }
            
            if (_referenceLink == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the lookup reference link."));
            }

            if (_outputLink == null)
            {
                exceptions.Add(new StageValidationException($"{Name} doesn't have the output link."));
            }

            return exceptions;
        }
    }
}
