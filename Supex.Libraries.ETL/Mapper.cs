using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Supex.Libraries.ETL
{
    public sealed class Mapper<TInput, TOutput> where TInput : class where TOutput : class
    {
        private readonly Dictionary<string, Expression<Func<TInput, object>>> _mappings = new Dictionary<string, Expression<Func<TInput, object>>>();
        
        public Mapper<TInput, TOutput> ForMember(Expression<Func<TOutput, object>> output, Expression<Func<TInput, object>> input)
        {
            var property = output.GetMemberName();
            if (_mappings.ContainsKey(property))
            {
                throw new StageValidationException("The output property is allowed to have one mapping.");
            }
            _mappings.Add(property, input);
            return this;
        }

        public TOutput Map(TInput input)
        {
            var inputType = input.GetType();

            var outputProperties = typeof(TOutput).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var inputProperties = inputType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var output = Activator.CreateInstance<TOutput>();

            foreach (var prop in outputProperties)
            {
                var inputProp = inputProperties.FirstOrDefault(x => x.Name == prop.Name);
                if (inputProp != null)
                {
                    prop.SetValue(output, _mappings.ContainsKey(prop.Name)
                            ? _mappings[prop.Name].Compile()(input)
                            : inputProp.GetValue(input, null), null);
                }
            }

            return output;
        }
    }
}
