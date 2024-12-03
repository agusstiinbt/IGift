using System.Text.Json;
using IGift.Application.Interfaces.Serialization.Options;
using Microsoft.Extensions.Options;

namespace IGift.Infrastructure.Serialization.Serializers
{
    /// <summary>
    /// Implementacion que se puede especificar si usar System.Text.Json
    /// </summary>
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonSerializer(IOptions<SystemTextJsonOptions> options)
        {
            _options = options.Value.JsonSerializerOptions;
        }

        public T Deserialize<T>(string data) => JsonSerializer.Deserialize<T>(data, _options);

        public string Serialize<T>(T data) => JsonSerializer.Serialize(data, _options);
    }
}
