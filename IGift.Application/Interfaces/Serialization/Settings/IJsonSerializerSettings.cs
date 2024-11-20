using Newtonsoft.Json;

namespace IGift.Application.Interfaces.Serialization.Settings
{
    /// <summary>
    /// Ideal para aplicaciones que trabajan con JSON altamente personalizado o estructuras complejas.Permite trabajar con JObject y JToken para manipular estructuras JSON dinámicas sin necesidad de deserializarlas completamente en clases. Ejemplo: Un microservicio que integra APIs externas que devuelven respuestas JSON complejas, donde se necesitan ajustes personalizados como ignorar propiedades nulas, modificar nombres de propiedades, o manejar formatos de fecha específicos.
    /// </summary>
    public interface IJsonSerializerSettings
    {
        /// <summary>
        /// Settings for <see cref="Newtonsoft.Json"/>.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; }
    }
}
