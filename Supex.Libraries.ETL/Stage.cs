using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public abstract class Stage
    {
        public string Name { get; }

        protected Stage(string name)
        {
            Name = name;
        }

        public abstract Task ExecuteAsync();
    }
}
