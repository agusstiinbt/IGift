using AutoMapper;
using IGift.Application.Models;
using IGift.Application.Responses.Chat;
using IGift.Application.Responses.Identity.Users;
using IGift.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
