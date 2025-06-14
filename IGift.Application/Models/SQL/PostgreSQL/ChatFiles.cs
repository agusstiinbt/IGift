namespace IGift.Application.Models.SQL.PostgreSQL
{
    public class ChatFiles
    {
        public Guid Id { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;//.pdf,.jpg,etc
        public long Size { get; set; }
        public string MimeType { get; set; } = string.Empty;//application/pdf, image/jpeg
        public DateTime UploadDate { get; set; }
        public bool IsPicture { get; set; }

    }

    // Recomendación: Organizar los archivos por estructura de carpetas como:
    ///ArchivosChat/{UsuarioId//}/{ Año}/{ Mes}/ archivo.ext
    

    ///Desde el cliente al servidor lo ideal seria con enviarlo con MultipartFormData
}
