using System.ComponentModel.DataAnnotations.Schema;

namespace IGift.Application.Interfaces
{
    public interface IChatUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureDataUrl { get; set; }
    }
}
