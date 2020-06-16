using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Outbox.MassTransit;
using Outbox.Providers.Ef;

namespace Outbox.AspNetCore.EntityFramework
{
    public static class OutboxServiceProviderExtensions
    {
        public static IServiceCollection AddEfOutbox<TDbContext>([NotNull] this IServiceCollection services) where TDbContext : DbContext
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IHostedService, OutboxDispatcher<TDbContext>>();

            services.AddSingleton<IOutboxSerializer, DefaultOutboxSerializer>();
            services.AddScoped<IOutboxMessageProcessorFactory, OutboxMessageProcessorFactory>();
            services.AddScoped<IOutboxService<TDbContext>, OutboxService<TDbContext>>();
            services.AddScoped<IOutboxDataProvider<TDbContext>, EfOutboxDataProvider<TDbContext>>();

            return services;
        }
    }
}