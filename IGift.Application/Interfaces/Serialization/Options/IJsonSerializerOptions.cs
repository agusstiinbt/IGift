using System.Text.Json;

namespace IGift.Application.Interfaces.Serialization.Options
{
    /// <summary>
    /// Interfaz que define las opciones para System.Text.Json. Ideal para microservicios  donde los mensajes JSON son sencillo y no necesitamos configuraciones avanzadas. Ejemplo: Un microservice que recibe datos de sensores (temperatura, humedad, etc) en un formato JSON simple y los procesa rápidamente
    /// </summary>
    public interface IJsonSerializerOptions
    {
        /// <summary>
        /// Options for <see cref="System.Text.Json"/>.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; }
    }
}
