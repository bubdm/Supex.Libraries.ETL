using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public abstract class InputStage : Stage
    {
        private dynamic _outputLink;

        public void LinkTo<TOutput>(Link<TOutput> outputLink) where TOutput : class, new()
        {
            _outputLink = outputLink;
        }

        protected dynamic GetOutputLink()
        {
            return _outputLink;
        }

        protected InputStage(string name) : base(name)
        {
        }
    }
}
