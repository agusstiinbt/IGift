using IGift.Application.Enums;

namespace IGift.Application.Requests.Files
{
    public class UploadRequest//TODO si esta clase se usa dentro de un mediatR ponerle el nombre con la nomenclatura correcta
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; }
    }
}
