using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace IGift.Application.Serialization.JsonConverters
{
    /// <summary>
    /// La clase TimeSpanJsonConverter es un convertidor personalizado para manejar la serialización y deserialización de objetos de tipo TimeSpan en formato JSON, utilizando un formato específico: Days.Hours:Minutes:Seconds. Sin este convertidor, la representación predeterminada de un TimeSpan en JSON puede ser poco intuitiva o variar dependiendo de la biblioteca de serialización. Para APIs que exponen duraciones como parte de sus respuestaas o consumen duraciones en sus solicitudes.
    /// </summary>
    public class TimespanJsonConverter : JsonConverter<TimeSpan>
    {
        /// <summary>
        /// Format: Days.Hours:Minutes:Seconds:Milliseconds
        /// </summary>
        public const string TimeSpanFormatString = @"d\.hh\:mm\:ss\:FFF";

        /// <summary>
        /// Este método se usa para deserializar una cadena en formato TimeSpanFormatString desde JSON y convertirla a un objeto TimeSpan. Entrar para mas documentación
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns>Si el formato no coincide con TimeSpanFormatString, lanza una excepción con un mensaje claro para el desarrollador.</returns>
        /// <exception cref="FormatException">asd</exception>
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            #region Ejemplo
            /* 
              Ejemplo de Json:
              { 
                "duration": "1.12:30:15.250"
              }
              Si se encuentra este valor en el JSON, será convertido a un objeto TimeSpan que representa:
              1 día 12 horas 30 minutos 15 segundos 250 milisegundos 
          */
            #endregion

            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
            {
                return TimeSpan.Zero;
            }

            if (!TimeSpan.TryParseExact(s, TimeSpanFormatString, null, out var parsedTimeSpan))
            {
                throw new FormatException($"Input timespan is not in an expected format : expected {Regex.Unescape(TimeSpanFormatString)}. Please retrieve this key as a string and parse manually.");
            }

            return parsedTimeSpan;
        }

        /// <summary>
        /// Serializa un objeto TimeSpan al formato especificado  (d\.hh\:mm\:ss\:FFF). 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            var timespanFormatted = $"{value.ToString(TimeSpanFormatString)}";
            writer.WriteStringValue(timespanFormatted);
        }
    }
}
