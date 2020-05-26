namespace Outbox
{
    public interface IOutboxMessageProcessorFactory
    {
        IOutboxMessageProcessor GetMessageProcessor(Outbox outbox);
    }
}