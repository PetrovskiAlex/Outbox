using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Outbox.Providers.Ef
{
    public class EfOutboxDataProvider<TDbContext> : BaseOutboxDataProvider<TDbContext> where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly OutboxOptions _outboxOptions;

        public EfOutboxDataProvider(TDbContext dbContext, IOptions<OutboxOptions> outboxOptions)
        {
            _dbContext = dbContext;
            _outboxOptions = outboxOptions.Value;
        }

        public override async Task<Outbox> Save([NotNull] Outbox outbox)
        {
            if (outbox == null) throw new ArgumentNullException(nameof(outbox));

            var entityEntry = _dbContext.Set<Outbox>().Add(outbox);
            await _dbContext.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public override async Task<Outbox[]> GetWaitingMessages()
        {
            var outboxes = await _dbContext.Set<Outbox>()
                .Where(o => o.Retries < _outboxOptions.Retries && (o.Status == OutboxMessageStatus.Failed || o.Status == OutboxMessageStatus.New))
                .OrderBy(o => o.Created)
                .Take(_outboxOptions.MessagesToProcess)
                .ToArrayAsync();

            return outboxes;
        }

        protected override async Task UpdateOutbox(Outbox outbox)
        {
            _dbContext.Set<Outbox>().Update(outbox);
            await _dbContext.SaveChangesAsync();
        }
    }
}