using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public abstract class OutputStage<TInput> : Stage where TInput : class, new()
    {
        private Link<TInput> _inputLink;

        protected OutputStage(string name) : base(name)
        {
        }

        public void LinkFrom(Link<TInput> inputLink)
        {
            _inputLink = inputLink;
        }

        protected Link<TInput> GetInputLink()
        {
            return _inputLink;
        }
    }
}
