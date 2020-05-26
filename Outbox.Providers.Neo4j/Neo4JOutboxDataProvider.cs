using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Neo4jClient.Transactions;

namespace Outbox.Providers.Neo4j
{
    public class Neo4JOutboxDataProvider<TDbContext> : BaseOutboxDataProvider<TDbContext>
    {
        private readonly ITransactionalGraphClient _graphClient;
        private readonly OutboxOptions _outboxOptions;

        public Neo4JOutboxDataProvider(ITransactionalGraphClient graphClient, IOptions<OutboxOptions> outboxOptions)
        {
            _graphClient = graphClient;
            _outboxOptions = outboxOptions.Value;
        }

        public override async Task<Outbox> Save(Outbox outbox)
        {
            await _graphClient.Cypher
                .Create($"(outbox:{nameof(Outbox)})")
                .Set("outbox = {outbox}").WithParam("outbox", outbox)
                .ExecuteWithoutResultsAsync();

            return outbox;
        }

        public override async Task<Outbox[]> GetWaitingMessages()
        {
            var failedStatus = OutboxMessageStatus.Failed;
            var newStatus = OutboxMessageStatus.New;

            var outboxes = await _graphClient.Cypher.Match($"(o:{nameof(Outbox)})")
                .Where((Outbox o) => o.Retries < _outboxOptions.Retries)
                .AndWhere((Outbox o) => o.Status == failedStatus || o.Status == newStatus)
                .Return<Outbox>("o")
                .OrderBy("o.Created")
                .Limit(_outboxOptions.MessagesToProcess)
                .ResultsAsync;

            return outboxes.ToArray();
        }

        protected override async Task UpdateOutbox(Outbox outbox)
        {
            await _graphClient.Cypher.Merge($"(o:{nameof(Outbox)} {{ Id: {{id}} }})")
                .WithParam("id", outbox.Id)
                .Set("o = {outbox}").WithParam("outbox", outbox)
                .ExecuteWithoutResultsAsync();
        }
    }
}