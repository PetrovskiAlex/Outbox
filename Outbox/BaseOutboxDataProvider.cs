using System;
using System.Threading.Tasks;

namespace Outbox
{
    public abstract class BaseOutboxDataProvider<TDbContext> : IOutboxDataProvider<TDbContext>
    {
        public abstract Task<Outbox> Save(Outbox outbox);

        public abstract Task<Outbox[]> GetWaitingMessages();

        public virtual async Task Fail(Outbox outbox, string errorMessage = null, Exception exception = null)
        {
            outbox.Status = OutboxMessageStatus.Failed;
            outbox.Updated = DateTime.UtcNow;
            outbox.Retries++;
            outbox.ErrorMessage = errorMessage;
            outbox.Error = exception?.ToString();

            await UpdateOutbox(outbox);
        }

        public virtual async Task Succeed(Outbox outbox)
        {
            outbox.Status = OutboxMessageStatus.Succeeded;
            outbox.Updated = DateTime.UtcNow;
            outbox.Retries++;

            await UpdateOutbox(outbox);
        }

        protected abstract Task UpdateOutbox(Outbox outbox);
    }
}