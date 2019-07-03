using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public abstract class ProcessingStage : Stage
    {
        public abstract IReadOnlyCollection<StageValidationException> Validate();
        protected ProcessingStage(string name) : base(name)
        {
        }
    }
}
