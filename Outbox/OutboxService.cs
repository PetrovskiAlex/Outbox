using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Outbox
{
    public class OutboxService<TDbContext> : IOutboxService<TDbContext>
    {
        private readonly IOutboxDataProvider<TDbContext> _outboxDataProvider;
        private readonly IOutboxSerializer _serializer;

        public OutboxService([NotNull] IOutboxDataProvider<TDbContext> outboxDataProvider, [NotNull] IOutboxSerializer serializer)
        {
            _outboxDataProvider = outboxDataProvider ?? throw new ArgumentNullException(nameof(outboxDataProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task Publish<T>(T message)
        {
            return InnerSend(message, OutboxMessageType.Event);
        }

        public Task Send<T>(T message)
        {
            return InnerSend(message, OutboxMessageType.Command);
        }

        private Task InnerSend<T>(T message, OutboxMessageType messageType)
        {
            var outbox = new Outbox
            {
                Name = message.GetType().Name,
                Status = OutboxMessageStatus.New,
                OutboxMessageType = messageType,
                Content = _serializer.Serialize(message)
            };

            return _outboxDataProvider.Save(outbox);
        }
    }
}