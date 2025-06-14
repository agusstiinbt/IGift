using AutoMapper;
using IGift.Application.Models.MongoDBModels;
using IGift.Application.Responses.Notification;

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
