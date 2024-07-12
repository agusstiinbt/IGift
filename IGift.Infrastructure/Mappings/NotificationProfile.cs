using AutoMapper;
using IGift.Application.Models;
using IGift.Application.Responses.Chat;

namespace IGift.Infrastructure.Mappings
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationResponse>();
        }
    }
}
