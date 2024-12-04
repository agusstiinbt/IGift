using System.Text.Json.Serialization;
using IGift.Application.Enums;

namespace IGift.Application.CQRS.Files
{
    /// <summary>
    /// Esta clase se usa de manera 'genérica' para poder subir todo tipo de archivos, desde fotos de perfil hasta archivos de excel
    /// </summary>
    public class UploadRequest
    {
        /// <summary>
        /// Nombre del archivo a guardar
        /// </summary>
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// Extension para guardar el archivo
        /// </summary>
        [JsonPropertyName("extension")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de archivo según UploadType Enum. Ver para más información
        /// </summary>
        [JsonPropertyName("uploadType")]
        public UploadType UploadType { get; set; }
        /// <summary>
        /// Array de bytes para guardar el archivo
        /// </summary>
        [JsonPropertyName("data")]
        public byte[] Data { get; set; }
    }
}
