using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Outbox.AspNetCore
{
    public class OutboxDispatcher<TDbContext> : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly OutboxOptions _outboxOptions;

        public OutboxDispatcher([NotNull] IServiceScopeFactory scopeFactory, [NotNull] IOptions<OutboxOptions> options)
        {
            _serviceScopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _outboxOptions = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var logger = scope.ServiceProvider.GetService<ILogger<OutboxDispatcher<TDbContext>>>();
                var outboxDataProvider = scope.ServiceProvider.GetService<IOutboxDataProvider<TDbContext>>();
                var messageProcessorFactory = scope.ServiceProvider.GetService<IOutboxMessageProcessorFactory>();
                var processorLogger = scope.ServiceProvider.GetService<ILogger<OutboxProcessor<TDbContext>>>();
                var outboxProcessor = new OutboxProcessor<TDbContext>(outboxDataProvider, messageProcessorFactory, processorLogger);

                try
                {
                    await outboxProcessor.Process();
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                }

                await Task.Delay(_outboxOptions.ProcessorDelay, stoppingToken);
            }
        }
    }
}