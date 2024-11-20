using System.Text.Json;

namespace IGift.Application.Interfaces.Serialization.Options
{
    /// <summary>
    /// Interfaz que define las opciones para System.Text.Json
    /// </summary>
    public interface IJsonSerializerOptions
    {
        /// <summary>
        /// Options for <see cref="System.Text.Json"/>.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; }
    }
}
