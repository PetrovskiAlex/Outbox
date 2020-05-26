using System.Threading.Tasks;

namespace Outbox
{
    public interface IOutboxProcessor
    {
        Task Process();
    }
}