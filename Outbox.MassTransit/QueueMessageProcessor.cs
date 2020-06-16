using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Outbox.MassTransit
{
    public class QueueMessageProcessor : IOutboxMessageProcessor
    {
        private readonly IMassTransitSender _massTransitSender;
        private readonly IOutboxSerializer _outboxSerializer;

        public QueueMessageProcessor([NotNull] IMassTransitSender massTransitSender, [NotNull] IOutboxSerializer outboxSerializer)
        {
            _massTransitSender = massTransitSender ?? throw new ArgumentNullException(nameof(massTransitSender));
            _outboxSerializer = outboxSerializer ?? throw new ArgumentNullException(nameof(outboxSerializer));
        }

        public async Task ProcessMessage(Outbox outbox)
        {
            var message = _outboxSerializer.Deserialize(outbox.Content);

            switch (outbox.OutboxMessageType)
            {
                case OutboxMessageType.Event:
                    await _massTransitSender.Publish(message);
                    break;
                case OutboxMessageType.Command:
                    await _massTransitSender.Send(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}