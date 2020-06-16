using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace Outbox.MassTransit
{
    public class MassTransitSender : IMassTransitSender
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitSender([NotNull] ISendEndpointProvider sendEndpointProvider, [NotNull] IPublishEndpoint publishEndpoint)
        {
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public Task Send<T>(T message) where T : class
        {
            return Send(message, CreateDefaultCancellationToken());
        }

        public Task Send<T>(T message, CancellationToken cancellationToken) where T : class
        {
            return Send((object) message, cancellationToken);
        }

        public Task Send([NotNull] object message)
        {
            return Send(message, CreateDefaultCancellationToken());
        }

        public Task Send([NotNull] object message, CancellationToken cancellationToken)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            return _sendEndpointProvider.Send(message, cancellationToken);
        }


        public Task Publish<T>(T message) where T : class
        {
            return Publish(message, CreateDefaultCancellationToken());
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken) where T : class
        {
            return Publish((object) message, cancellationToken);
        }

        public Task Publish(object message)
        {
            return Publish(message, CreateDefaultCancellationToken());
        }

        public Task Publish(object message, CancellationToken cancellationToken)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            return _publishEndpoint.Publish(message, cancellationToken);
        }

        private CancellationToken CreateDefaultCancellationToken()
        {
            return CancellationToken.None;
        }
    }
}