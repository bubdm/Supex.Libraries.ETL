using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public class StageValidationException : Exception
    {
        public StageValidationException(string message) : base(message)
        {
        }
    }
}
