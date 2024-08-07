using AutoMapper;
using IGift.Application.Responses.Identity.Users;
using IGift.Infrastructure.Models;

namespace IGift.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<IGiftUser, UserResponse>().ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.ProfilePictureDataUrl.Url));
        }
    }
}
