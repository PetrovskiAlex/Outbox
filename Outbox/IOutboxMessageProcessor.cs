using System.Threading.Tasks;

namespace Outbox
{
    public interface IOutboxMessageProcessor
    {
        Task ProcessMessage(Outbox outbox);
    }
}