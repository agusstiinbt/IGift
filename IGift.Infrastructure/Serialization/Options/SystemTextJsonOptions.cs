using System.Text.Json;
using IGift.Application.Interfaces.Serialization.Options;

namespace IGift.Infrastructure.Serialization
{
    /// <summary>
    /// Encapsula las configuraciones que System.Text.Json usará para serializar y deserializar
    /// </summary>
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}
