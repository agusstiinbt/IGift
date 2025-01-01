using IGift.Domain.Contracts;

namespace IGift.Application.Models
{
    /// <summary>
    /// Metadata de foto perfil. PostgreSQL
    /// </summary>
    public class ProfilePicture : Entity<string>
    {
        public required string IdUser { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string? FileType { get; set; } = string.Empty;
    }
}
