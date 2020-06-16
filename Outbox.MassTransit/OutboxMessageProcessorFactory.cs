using System;
using System.Diagnostics.CodeAnalysis;

namespace Outbox.MassTransit
{
    public class OutboxMessageProcessorFactory : IOutboxMessageProcessorFactory
    {
        private readonly IMassTransitSender _massTransitSender;
        private readonly IOutboxSerializer _outboxSerializer;

        public OutboxMessageProcessorFactory([NotNull] IMassTransitSender massTransitSender, [NotNull] IOutboxSerializer outboxSerializer)
        {
            _massTransitSender = massTransitSender ?? throw new ArgumentNullException(nameof(massTransitSender));
            _outboxSerializer = outboxSerializer ?? throw new ArgumentNullException(nameof(outboxSerializer));
        }

        public IOutboxMessageProcessor GetMessageProcessor(Outbox outbox)
        {
            switch (outbox.OutboxMessageType)
            {
                case OutboxMessageType.Event:
                    return new QueueMessageProcessor(_massTransitSender, _outboxSerializer);
                case OutboxMessageType.Command:
                    return new QueueMessageProcessor(_massTransitSender, _outboxSerializer);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}