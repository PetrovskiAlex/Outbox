using System;
using System.Threading.Tasks;

namespace Outbox
{
    public interface IOutboxDataProvider<TDbContext>
    {
        Task<Outbox> Save(Outbox outbox);
        Task<Outbox[]> GetWaitingMessages();
        Task Fail(Outbox outbox, string errorMessage = null, Exception exception = null);
        Task Succeed(Outbox outbox);
    }
}