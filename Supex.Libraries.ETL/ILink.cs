using System;
using System.Collections.Generic;
using System.Text;

namespace Supex.Libraries.ETL
{
    public interface ILink<TInput> where TInput : class
    {
        string Name { get; }

        bool AllEnqueued { get; set; }

        void Complete();

        void Enqueue(TInput input);

        void EnqueueAll(IEnumerable<TInput> inputs);

        TInput Dequeue();

        IEnumerable<TInput> DequeueAll();
    }
}
