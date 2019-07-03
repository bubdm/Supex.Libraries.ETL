using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public class KeyMapping
    {
        public KeyMapping(string inputKey, string referenceKey)
        {
            InputKey = inputKey;
            ReferenceKey = referenceKey;
        }

        public string ReferenceKey { get; }

        public string InputKey { get; }
    }
}
