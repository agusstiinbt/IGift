using IGift.Application.Enums;
using System.Text.Json.Serialization;

namespace IGift.Application.Requests.Files
{
    public class UploadRequest
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("extension")]
        public string Extension { get; set; } = string.Empty;

        [JsonPropertyName("uploadType")]
        public UploadType UploadType { get; set; }

        [JsonPropertyName("data")]
        public byte[] Data { get; set; }
    }
}
