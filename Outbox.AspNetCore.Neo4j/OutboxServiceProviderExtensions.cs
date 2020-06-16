using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4jClient.Transactions;
using Outbox.MassTransit;
using Outbox.Providers.Neo4j;

namespace Outbox.AspNetCore.Neo4j
{
    public static class OutboxServiceProviderExtensions
    {
        public static IServiceCollection AddNeo4JOutbox([NotNull] this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, OutboxDispatcher<ITransactionalGraphClient>>();            

            services.AddSingleton<IOutboxSerializer, DefaultOutboxSerializer>();
            services.AddScoped<IOutboxMessageProcessorFactory, OutboxMessageProcessorFactory>();
            services.AddScoped<IOutboxService<ITransactionalGraphClient>, OutboxService<ITransactionalGraphClient>>();
            services.AddScoped<IOutboxDataProvider<ITransactionalGraphClient>, Neo4JOutboxDataProvider<ITransactionalGraphClient>>();

            return services;
        }
    }
}