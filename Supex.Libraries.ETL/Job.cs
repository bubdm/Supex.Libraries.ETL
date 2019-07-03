using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public abstract class Job
    {
        protected readonly string Name;
        private readonly List<string> _linkNames = new List<string>();
        private readonly List<Stage> _stages = new List<Stage>();
        private ILogger _logger;

        public Guid CorrelationId { get; } = Guid.NewGuid();

        protected Job(string name)
        {
            Name = name;
        }

        protected void AddStages(params Stage[] stages)
        {
            _stages.AddRange(stages);
        }

        public Task ExecuteAsync()
        {
            if (_logger == null)
            {
                _logger = new ConsoleLogger();
            }

            _logger.Write(CorrelationId, Name, "Validation Started");

            var exceptions = new List<StageValidationException>();
            foreach (var stage in _stages)
            {   
                if (stage is ProcessingStage processingStage)
                {
                    var validationExceptions = processingStage.Validate();
                    if (validationExceptions != null && validationExceptions.Any())
                    {
                        exceptions.AddRange(validationExceptions);
                    }
                }
            }

            if (exceptions.Any())
            {
                _logger.Write(CorrelationId, Name, string.Join("\n", exceptions.Select(s => s.Message)));
                throw new Exception();
            }

            _logger.Write(CorrelationId, Name, "Job Started");
            var tasks = new List<Task>();
            foreach (var stage in _stages)
            {
                var task = Task.Factory.StartNew(() => stage.ExecuteAsync());
                _logger.Write(CorrelationId, Name, $"{stage.Name} has started with task ID {task.Id}");
                tasks.Add(task);
            }

            return Task.WhenAll(tasks.ToArray()).ContinueWith(task => _logger.Write(CorrelationId, Name, "Job Finished"));
        }

        public void AppendLogger(ILogger logger)
        {
            _logger = logger;
        }
    }
}
