using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Outbox
{
    public class OutboxProcessor<TDbContext> : IOutboxProcessor
    {
        private readonly IOutboxDataProvider<TDbContext> _dataProvider;
        private readonly IOutboxMessageProcessorFactory _messageProcessorFactory;
        private readonly ILogger<OutboxProcessor<TDbContext>> _logger;

        public OutboxProcessor(
            IOutboxDataProvider<TDbContext> dataProvider, 
            IOutboxMessageProcessorFactory messageProcessorFactory, 
            ILogger<OutboxProcessor<TDbContext>> logger)
        {
            _dataProvider = dataProvider;
            _messageProcessorFactory = messageProcessorFactory;
            _logger = logger;
        }

        public async Task Process()
        {
            _logger.LogInformation("Outbox processor has been started");

            var messages = await _dataProvider.GetWaitingMessages();
            _logger.LogInformation("Messages to process", messages.Length);

            foreach (var message in messages)
            {
                _logger.LogInformation("Message has been started to process", message.Id);

                var messageProcessor = _messageProcessorFactory.GetMessageProcessor(message);
                try
                {
                    await messageProcessor.ProcessMessage(message);
                    await _dataProvider.Succeed(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message, message.Id);
                    await _dataProvider.Fail(message, e.Message, e);
                }

                _logger.LogInformation("Message has been processed", message.Id);
            }

            _logger.LogInformation("Outbox processor finished");
        }
    }
}