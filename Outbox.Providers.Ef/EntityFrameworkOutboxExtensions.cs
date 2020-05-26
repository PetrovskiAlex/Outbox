using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Outbox.Providers.Ef
{
    public static class EntityFrameworkOutboxExtensions
    {
        /// <summary>
        /// Регистрируем в контексте EF объект Outbox 
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void OutboxModelCreating([NotNull] this ModelBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Entity<Outbox>()
                .HasIndex(o => o.Created);

            builder.Entity<Outbox>()
                .HasIndex(o => o.Status);

            builder.Entity<Outbox>()
                .HasIndex(o => o.OutboxMessageType);

            builder.Entity<Outbox>()
                .HasIndex(o => o.Retries);
        }
    }
}