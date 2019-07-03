using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public sealed class SortDescriptor
    {
        public string Property { get; }
        public SortDirection Direction { get; }

        public SortDescriptor(string property, SortDirection direction)
        {
            Property = property;
            Direction = direction;
        }
    }
}
