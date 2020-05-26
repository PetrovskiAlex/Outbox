using Newtonsoft.Json;

namespace Outbox
{
    public class DefaultOutboxSerializer : IOutboxSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public DefaultOutboxSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public string Serialize<T>(T message)
        {
            return JsonConvert.SerializeObject(message, _settings);
        }

        public T Deserialize<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message, _settings);
        }

        public object Deserialize(string message)
        {
            return JsonConvert.DeserializeObject(message, _settings);
        }
    }
}