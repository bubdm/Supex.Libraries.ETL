using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public interface ILogger
    {
        void Write(Guid correlationId, string jobName, string message);
    }
}
