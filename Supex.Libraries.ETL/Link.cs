using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public sealed class Link<TInput> : ILink<TInput> where TInput : class
    {
        private readonly ConcurrentQueue<TInput> _queue;
        public string Name { get; }
        public int EnqueuedCount { get; private set; }
        public int ProcessedCount { get; private set; }

        public bool AllEnqueued { get; set; }

        public Link(string name)
        {
            Name = name;
            _queue = new ConcurrentQueue<TInput>();
        }

        public void Complete()
        {
            AllEnqueued = true;
        }

        public void Enqueue(TInput input)
        {
            EnqueuedCount++;
            _queue.Enqueue(input);
        }

        public void EnqueueAll(IEnumerable<TInput> inputs)
        {
            foreach (var input in inputs)
            {
                Enqueue(input);
            }
        }

        public TInput Dequeue()
        {
            if (AllEnqueued)
            {
                if (!_queue.TryDequeue(out var value))
                {
                    return null;
                }

                ProcessedCount++;
                return value;
            }

            while (true)
            {
                if (_queue.TryDequeue(out var value))
                {
                    ProcessedCount++;
                    return value;
                }

                if (AllEnqueued) return null;

                Task.Delay(1).Wait();
            }
        }

        public IEnumerable<TInput> DequeueAll()
        {
            var list = new List<TInput>();
            for (var i = 0; i < EnqueuedCount; i++)
            {
                _queue.TryDequeue(out var value);
                list.Add(value);
            }

            return list;
        }
    }
}
