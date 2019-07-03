using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supex.Libraries.ETL;

namespace Supex.Libraries.Examples.Stages
{
    public class DeptInputStage : InputStage
    {
        public DeptInputStage(string name) : base(name)
        {
        }

        public override Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
