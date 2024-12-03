using IGift.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace IGift.Infrastructure.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}
