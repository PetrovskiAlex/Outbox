using System.Threading.Tasks;

namespace Outbox
{
    public interface IOutboxService<TDbContext>
    {
        Task Publish<T>(T message);
        Task Send<T>(T message);
    }
}